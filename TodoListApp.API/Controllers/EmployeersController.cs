using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TodoListApp.Core.Dtos;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Models;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.API.Controllers;

[ApiController]
public class EmployeersController(IEmployeerRepository repo, IValidator<CreateEmployeerDto> empValidator, ILogger<EmployeersController> logger)
    : ControllerBase
{
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
        return NoContent();
    }

    [HttpGet("api/employeers/{employeer_id}/payments")]
    public async Task<ActionResult<IEnumerable<EmployeerPaymentEntity>>> GetAllPaymentsForEmployeer([FromRoute] int employeer_id)
    {
        var payments = await repo.GetAllPaymentsForEmployeer(employeer_id);
        return payments != null ? Ok(payments) : NoContent();
    }

    [HttpPost("api/employeers/{employeer_id}/payments")]
    public async Task<IActionResult> AddPaymentFromEmployeer([FromRoute] int employeer_id, [FromBody] EmployeerPaymentPayload payment)
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

    [HttpPost("api/employeers")]
    public async Task<IActionResult> AddEmployeer([FromBody] CreateEmployeerDto employeer)
    {
        var isValid = await empValidator.ValidateAsync(employeer);

        if (!isValid.IsValid)
        {
            return BadRequest(isValid.Errors);
        }

        var empId = await repo.AddEmployeer(employeer.ToEntity());
        var createdEmp = await repo.GetEmployeer(empId);

        logger.LogInformation("Создан заказчик с ID {id}. Data[{data}]", empId, JsonSerializer.Serialize(createdEmp));

        return CreatedAtAction(nameof(GetById), new { id = empId }, createdEmp);
    }
}
