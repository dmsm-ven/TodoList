using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class EmployeersController : ControllerBase
{
    private readonly IEmployeerRepository repo;

    public EmployeersController(IEmployeerRepository repo)
    {
        this.repo = repo;
    }
    [HttpGet("api/employeers")]
    public async Task<ActionResult<IEnumerable<EmployeerEntity>>> Get()
    {
        var employeers = await repo.GetAllEmployeer();
        return employeers != null ? Ok(employeers) : NoContent();
    }
}
