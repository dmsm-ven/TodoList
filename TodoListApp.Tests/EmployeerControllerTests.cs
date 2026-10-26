using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoListApp.API.Controllers;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Tests;

public class EmployeerControllerTests
{
    [Fact]
    public async Task EmployeersController_GetAll_ReturnsNoContent_WhenEmpty()
    {
        var repoMock = new Mock<IEmployeerRepository>();
        repoMock.Setup(r => r.GetAllEmployeer()).ReturnsAsync(new List<EmployeerEntity>());

        var controller = new EmployeersController(repoMock.Object);

        var result = await controller.GetAll();
        var actionResult = Assert.IsType<ActionResult<IEnumerable<EmployeerEntity>>>(result);
        Assert.IsType<OkObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task EmployeersController_AddEmployeer_ReturnsConflict_WhenDuplicate()
    {
        var existing = new EmployeerEntity { id = 1, name = "Acme" };
        var repoMock = new Mock<IEmployeerRepository>();
        repoMock.Setup(r => r.GetAllEmployeer()).ReturnsAsync(new List<EmployeerEntity> { existing });

        var controller = new EmployeersController(repoMock.Object);

        var payload = new EmployeerEntity { name = "Acme" };
        var result = await controller.AddEmployeer(payload);

        var actionResult = Assert.IsType<ActionResult<int>>(result);
        var conflict = Assert.IsType<ConflictObjectResult>(actionResult.Result);

        Assert.Equal("Employeer with the same name already exists.", conflict.Value);
    }
}
