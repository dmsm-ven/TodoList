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
    public async Task<ActionResult<IEnumerable<JobItemEntity>>> GetAllJobItems([FromQuery]int employeer_id, [FromQuery] int take_max_years) 
    {
        var jobs = await repo.GetAllJobItems(employeer_id, take_max_years);
        return jobs != null ? Ok(jobs) : NoContent();
    }
}
