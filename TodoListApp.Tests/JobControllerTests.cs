using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoListApp.API.Controllers;
using TodoListApp.Core.Entities;
using TodoListApp.Core.Repositories.Interfaces;

namespace TodoListApp.Tests;

public class JobControllerTests
{
    [Fact]
    public async Task JobsController_GetJobsForEmployeer_ReturnsOk_WhenJobsExist()
    {
        // arrange
        var jobs = new List<JobItemEntity>
        {
            new JobItemEntity { id = 1, employeer_id = 10, title = "Job 1" },
            new JobItemEntity { id = 2, employeer_id = 10, title = "Job 2" }
        };

        var repoMock = new Mock<IJobItemRepository>();
        repoMock.Setup(r => r.GetAllJobItems(10)).ReturnsAsync(jobs);

        var controller = new JobsController(repoMock.Object);

        // act
        var result = await controller.GetJobsForEmployeer(10);

        // assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<JobItemEntity>>>(result);
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var items = Assert.IsAssignableFrom<IEnumerable<JobItemEntity>>(ok.Value);
        Assert.Equal(2, items.Count());
    }

    [Fact]
    public async Task JobsController_GetJobById_ReturnsNotFound_WhenMissing()
    {
        var repoMock = new Mock<IJobItemRepository>();
        repoMock.Setup(r => r.GetJobItem(It.IsAny<int>())).ReturnsAsync((JobItemEntity?)null);

        var controller = new JobsController(repoMock.Object);

        var result = await controller.GetJobById(5);
        var actionResult = Assert.IsType<ActionResult<JobItemEntity>>(result);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

}
