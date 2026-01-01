using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class JobsController : ControllerBase
{
    private readonly IJobItemRepository repo;

    public JobsController(IJobItemRepository repo)
    {
        this.repo = repo;
    }
    [HttpGet("api/jobs")]
    public async Task<ActionResult<IEnumerable<JobItemEntity>>> GetJobsForEmployeer(
        [FromQuery] int employeer_id)
    {
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

    [HttpPut("api/jobs/{JobItemId}/updated")]
    public async Task<IActionResult> JobUpdated([FromRoute] int JobItemId, [FromBody] JobItemEntity item)
    {
        if (repo.GetJobItem(JobItemId) is null)
        {
            return NotFound();
        }

        var id = await repo.AddOrUpdateJobItem(item);
        return Ok(id);
    }
}
