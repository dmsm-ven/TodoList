using Microsoft.AspNetCore.Mvc;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IEmployeerPaymentRepository repo;

    public PaymentsController(IEmployeerPaymentRepository repo)
    {
        this.repo = repo;
    }
    [HttpGet("api/payments/{employeer_id}")]
    public async Task<ActionResult<IEnumerable<EmployeerPaymentEntity>>> GetAllPaymentsForEmployeer([FromRoute] int employeer_id)
    {
        var payments = await repo.GetAllPaymentsForEmployeer(employeer_id);
        return payments != null ? Ok(payments) : NoContent();
    }
}
