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

    /// <summary>Создание новой задачи. Возвращает id созданной задачи.</summary>
    [HttpPost("api/jobs")]
    public async Task<ActionResult<int>> CreateJob([FromBody] JobItemEntity item)
    {
        // id всегда назначается сервером
        item.id = 0;

        var id = await repo.AddOrUpdateJobItem(item);
        return Ok(id);
    }

    /// <summary>Обновление существующей задачи. Для несуществующей задачи возвращает 404.</summary>
    [HttpPut("api/jobs/{id}/updated")]
    public async Task<IActionResult> JobUpdated([FromRoute] int id, [FromBody] JobItemEntity item)
    {
        if (item.id != 0 && item.id != id)
        {
            return BadRequest("id в теле запроса не совпадает с id в маршруте.");
        }

        if (await repo.GetJobItem(id) is null)
        {
            return NotFound();
        }

        item.id = id;
        var updatedId = await repo.AddOrUpdateJobItem(item);
        return Ok(updatedId);
    }
}
