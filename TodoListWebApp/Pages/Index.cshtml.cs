using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoListApp.DataAccess.Entities;
using TodoListApp.DataAccess.Repositories.Interfaces;


namespace TodoListWebApp.Pages;
public class IndexModel : PageModel
{

    private readonly IEmployeerRepository empRepo;
    private readonly IJobItemRepository jobRepo;

    [BindProperty]
    public JobItemEntity? UpdatedTask { get; set; }

    public EmployeerEntity? SelectedEmployeer { get; set; }
    public IEnumerable<EmployeerEntity>? Employeers { get; set; }
    public IEnumerable<JobItemEntity>? SelectedEmployeerTasks { get; set; }

    public IndexModel(IEmployeerRepository empRepo, IJobItemRepository jobRepo)
    {
        this.empRepo = empRepo;
        this.jobRepo = jobRepo;
    }

    public async Task OnGetAsync()
    {
        Employeers = await empRepo.GetAllEmployeer();
        SelectedEmployeer = Employeers.FirstOrDefault();
        SelectedEmployeerTasks = (await jobRepo.GetAllJobItems(SelectedEmployeer.id))
            .Where(i => IsCurrentMonth(i.start_date))
            .OrderByDescending(t => t.start_date)
            .ToArray();
    }

    public async Task<IActionResult> OnGetChangeEmployeerAsync(int id)
    {
        Employeers = await empRepo.GetAllEmployeer();
        SelectedEmployeer = Employeers.Single(emp => emp.id == id);
        SelectedEmployeerTasks = (await jobRepo.GetAllJobItems(id))
            .Where(i => IsCurrentMonth(i.start_date))
            .OrderByDescending(t => t.start_date)
            .ToArray();

        return Page();
    }

    private bool IsCurrentMonth(DateTimeOffset dt)
    {
        return dt.Date.Month == DateTime.Now.Month && dt.Date.Year == DateTime.Now.Year;
    }

}
