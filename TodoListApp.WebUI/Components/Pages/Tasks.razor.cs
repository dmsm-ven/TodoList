using Microsoft.AspNetCore.Components;
using TodoListApp.Core.Entities;

namespace TodoListApp.WebUI.Components.Pages;

public partial class Tasks
{
    private const int ITEMS_PER_PAGE = 100;
    [SupplyParameterFromQuery(Name = nameof(EmployeerFilter))]
    public int EmployeerFilter { get; set; } = 1;

    private readonly Dictionary<int, EmployeerEntity> employeerMap = new();
    private List<JobItemEntity>? jobItems = null;
    private JobItemEntity newJob = new();
    private JobItemEntity? editJob = null;
    private EmployeerEntity? newEmployeer = null;
    private bool inProgress = false;
    private bool isDeleteEmployeerDialogVisible = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var employeers = await EmployeerRepository.GetAllEmployeer();
            foreach (var emp in employeers)
            {
                employeerMap[emp.id] = emp;
            }
            await RefreshSource();
        }
    }
    private async Task RefreshFilter(int empId)
    {
        jobItems = null;
        StateHasChanged();
        EmployeerFilter = empId;
        await RefreshSource();
        StateHasChanged();
    }
    private async Task RefreshSource()
    {
        jobItems = new();
        List<JobItemEntity> tmpJobList = new();
        foreach (var emp in employeerMap)
        {
            if (EmployeerFilter == 0 || (EmployeerFilter == emp.Key))
            {
                var empJobs = await JobItemRepository.GetAllJobItems(emp.Key);
                tmpJobList.AddRange(empJobs);
            }
        }

        jobItems.AddRange(tmpJobList.OrderByDescending(j => j.start_date).Take(ITEMS_PER_PAGE).ToArray());
        StateHasChanged();
    }
    private async Task AddNewJob()
    {

        if (string.IsNullOrWhiteSpace(newJob.title))
        {
            return;
        }

        inProgress = true;
        newJob.employeer_id = EmployeerFilter;

        jobItems!.Insert(0, newJob);
        try
        {
            await JobItemRepository.AddOrUpdateJobItem(newJob);
            newJob = new();
        }
        finally
        {
            inProgress = false;
        }
    }
    private async Task AddNewEmployeer()
    {
        if (!string.IsNullOrWhiteSpace(newEmployeer?.name) && newEmployeer.name != "Название/компания")
        {
            var newEmpId = await EmployeerRepository.AddEmployeer(newEmployeer);
            newEmployeer.id = newEmpId;
            employeerMap[newEmpId] = newEmployeer;
            EmployeerFilter = newEmpId;
            newEmployeer = new();
            await RefreshSource();
        }
        else
        {
            newEmployeer = (newEmployeer == null ? new() { name = "Название/компания" } : null);
        }
    }
    private void ChangeEditJob(JobItemEntity selectedItem)
    {
        editJob = selectedItem;
    }
    private async Task SaveEditJob()
    {
        if (editJob == null)
        {
            return;
        }

        await JobItemRepository.AddOrUpdateJobItem(editJob);
        editJob = null;

    }
    private async Task DeleteEditJob()
    {
        if (editJob == null)
        {
            return;
        }

        await JobItemRepository.DeleteJobItem(editJob.id);
        jobItems.Remove(editJob);
        editJob = null;


    }
    private async Task HandleDeleteEmployeerDialog(bool okPressed)
    {
        isDeleteEmployeerDialogVisible = false;

        if (!okPressed || EmployeerFilter == 0)
        {
            return;
        }
        await EmployeerRepository.DeleteEmployeer(EmployeerFilter);

        employeerMap.Remove(EmployeerFilter);

        EmployeerFilter = 0;
    }
    private string BoolToStatusSymbol(bool val)
    {
        return val ? "✅" : "❌";
    }

}