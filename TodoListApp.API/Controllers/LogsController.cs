using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class LogsController : ControllerBase
{
    private readonly IAppLogger logger;
    private const int MAX_LOGS_ITEMS = 1000;

    public LogsController(IAppLogger logger)
    {
        this.logger = logger;
    }
    [HttpGet("api/logs")]
    public async Task<ActionResult<IEnumerable<JobItemEntity>>> GetAllJobItems([FromQuery]int take_count) 
    {
        int count = take_count <= 0 ? 1 : take_count;
        count = count >= MAX_LOGS_ITEMS ? MAX_LOGS_ITEMS : count;

        var logs = await logger.GetLastRows(count);
        return logs != null ? Ok(logs) : NoContent();
    }
}
