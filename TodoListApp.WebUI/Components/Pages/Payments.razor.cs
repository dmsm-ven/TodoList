using Microsoft.AspNetCore.Components;
using TodoListApp.Core.Entities;

namespace TodoListApp.WebUI.Components.Pages;

public partial class Payments
{
    private const int ITEMS_PER_PAGE = 100;

    private List<EmployeerPaymentEntity>? payments = null;
    private readonly Dictionary<int, EmployeerEntity> employeerMap = new();

    private int? currentPage = 1;

    [SupplyParameterFromQuery(Name = nameof(EmployeerFilter))]
    public int EmployeerFilter { get; set; } = 1;

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
        payments = null;
        StateHasChanged();
        EmployeerFilter = empId;
        await RefreshSource();
        StateHasChanged();
    }

    private async Task RefreshSource()
    {
        payments = new();
        List<EmployeerPaymentEntity> tmpPayments = new();
        foreach (var emp in employeerMap)
        {
            if (EmployeerFilter == 0 || (EmployeerFilter == emp.Key))
            {
                var empJobs = await EmployeerRepository.GetAllPaymentsForEmployeer(emp.Key);
                tmpPayments.AddRange(empJobs);
            }
        }

        payments.AddRange(tmpPayments.OrderByDescending(j => j.transfer_arrival_date).Take(ITEMS_PER_PAGE).ToArray());
        StateHasChanged();
    }
}