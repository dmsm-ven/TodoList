using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
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
        [FromQuery] int employeer_id,
        [FromQuery] int take_max_years,
        [FromQuery] bool only_this_month)
    {
        var jobs = await repo.GetAllJobItems(employeer_id, take_max_years, only_this_month);
        return jobs != null ? Ok(jobs) : NoContent();
    }

    [HttpGet("api/jobs/{id}")]
    public async Task<ActionResult<JobItemEntity>> GetJobById([FromQuery] int id)
    {
        var item = await repo.GetJobItem(id);
        return item != null ? Ok(item) : NotFound();
    }

    [HttpDelete("api/jobs/{id}")]
    public async Task<IActionResult> DeleteJobById([FromQuery] int id)
    {
        var item = await repo.GetJobItem(id);
        if (item is null)
        {
            return NotFound();
        }
        await repo.DeleteJobItem(id);
        return Ok();
    }

    [HttpGet("api/jobs/{JobItemId}/changes-log")]
    public async Task<ActionResult<IEnumerable<JobItemHistoryEntity>>> GetJobChangesLog([FromRoute] int JobItemId)
    {
        if (repo.GetJobItem(JobItemId) is null)
        {
            return NotFound();
        }
        var result = await repo.GetHistoryChangesForJobItem(JobItemId);

        if (result.Count == 0)
        {
            return NoContent();
        }

        return Ok(result);
    }

    [HttpPost("api/jobs/{JobItemId}/changed")]
    public async Task<IActionResult> UpdateJobItem([FromRoute] int JobItemId, [FromBody] JobItemHistoryChangeRequest payload)
    {
        if (repo.GetJobItem(JobItemId) is null)
        {
            return NotFound();
        }
        await repo.AddHistoryChanges(payload);
        return Ok();
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
