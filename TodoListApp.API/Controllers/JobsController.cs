using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobItemRepository repo;
    private readonly IEmployeerRepository empRepo;
    private readonly IValidator<CreateJobDto> createJobValidator;
    private readonly IValidator<UpdateJobDto> updateJobValidator;

    public JobsController(IJobItemRepository repo,
        IEmployeerRepository empRepo,
        IValidator<CreateJobDto> createJobValidator,
        IValidator<UpdateJobDto> updateJobValidator)
    {
        this.empRepo = empRepo;
        this.repo = repo;
        this.createJobValidator = createJobValidator;
        this.updateJobValidator = updateJobValidator;

    }
    [HttpGet("api/jobs")]
    public async Task<ActionResult<IEnumerable<JobItemEntity>>> GetJobsForEmployeer(
        [FromQuery] int employeer_id)
    {
        var empExists = await empRepo.GetEmployeer(employeer_id);
        if (empExists is null)
        {
            return NotFound();
        }
        var jobs = await repo.GetAllJobItems(employeer_id);
        return jobs != null ? Ok(jobs) : NoContent();
    }

    [HttpGet("api/jobs/{id}")]
    public async Task<ActionResult<JobItemEntity>> GetJobById([FromRoute] int id)
    {
        var item = await repo.GetJobItem(id);
        return item != null ? Ok(item) : NotFound();
    }

    [HttpDelete("api/jobs/{id}")]
    public async Task<IActionResult> DeleteJobById([FromRoute] int id)
    {
        var item = await repo.GetJobItem(id);
        if (item is null)
        {
            return NotFound();
        }
        await repo.DeleteJobItem(id);
        return NoContent();
    }

    [HttpGet("api/jobs/{id}/changes-log")]
    public async Task<ActionResult<IEnumerable<JobItemHistoryEntity>>> GetJobChangesLog([FromRoute] int id)
    {
        if (await repo.GetJobItem(id) is null)
        {
            return NotFound();
        }
        var result = await repo.GetHistoryChangesForJobItem(id);

        if (result.Count == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }

    [HttpPut("api/jobs/{id}")]
    public async Task<IActionResult> UpdateJob([FromRoute] int id, [FromBody] UpdateJobDto item)
    {
        var validationResult = await updateJobValidator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await repo.AddOrUpdateJobItem(item.ToEntity());

        return Ok();
    }

    [HttpPost("api/jobs")]
    public async Task<IActionResult> CreateJob(CreateJobDto item)
    {
        var validationResult = await createJobValidator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var newJobItemId = await repo.AddOrUpdateJobItem(item.ToEntity());

        return CreatedAtRoute(
            nameof(GetJobById),
            new { id = newJobItemId },
            null);
    }
}
