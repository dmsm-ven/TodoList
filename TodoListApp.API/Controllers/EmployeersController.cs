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
    public async Task<ActionResult<IEnumerable<EmployeerEntity>>> GetAll()
    {
        var employeers = await repo.GetAllEmployeer();
        return employeers != null ? Ok(employeers) : NoContent();
    }

    [HttpGet("api/employeers/{id}")]
    public async Task<ActionResult<EmployeerEntity>> GetById([FromRoute] int id)
    {
        var emp = await repo.GetEmployeer(id);
        return emp != null ? Ok(emp) : NotFound();
    }

    [HttpDelete("api/employeers/{id}")]
    public async Task<IActionResult> DeleteEmployeerById([FromQuery] int id)
    {
        var item = await repo.GetEmployeer(id);
        if (item is null)
        {
            return NotFound();
        }
        await repo.DeleteEmployeer(id);
        return Ok();
    }

    [HttpGet("api/employeers/{employeer_id}/payments")]
    public async Task<ActionResult<IEnumerable<EmployeerPaymentEntity>>> GetAllPaymentsForEmployeer([FromRoute] int employeer_id)
    {
        var payments = await repo.GetAllPaymentsForEmployeer(employeer_id);
        return payments != null ? Ok(payments) : NoContent();
    }

    [HttpPost("api/employeers/{employeer_id}/payments")]
    public async Task<IActionResult> AddPaymentFromEmployeer([FromRoute] int employeer_id, [FromBody] EmployeerPaymentEntity payment)
    {
        if (payment is null || (payment?.amount ?? 0) <= 0 || employeer_id == 0)
        {
            return BadRequest();
        }
        var emp = await repo.GetEmployeer(employeer_id);
        if (emp is null)
        {
            return NotFound();
        }

        await repo.AddPayment(payment!);
        return Ok();
    }

    [HttpPost("api/employeers/add")]
    public async Task<ActionResult<int>> AddOrUpdateEmployeer([FromBody] EmployeerEntity payload)
    {
        if(payload is null || string.IsNullOrWhiteSpace(payload?.name))
        {
            return BadRequest();
        }
        if(payload.id != 0)
        { 
            var emp = await repo.GetEmployeer(payload.id);
            if(emp is null)
            {
                return NotFound();
            }
        }
        var empId = await repo.AddOrUpdateEmployeer(payload);
        return empId;
    }
}
