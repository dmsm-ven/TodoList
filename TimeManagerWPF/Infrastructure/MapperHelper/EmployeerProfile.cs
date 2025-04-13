using TodoList.WPF.Models;
using TodoList.WPF.Models.TodoList;
using TodoListApp.DataAccess.Entities;

public static class EmployeerMapperHelper
{
    public static EmployeerEntity ToEntity(this EmployeerViewModel x)
    {
        return new EmployeerEntity
        {
            id = x.Id,
            name = x.Name,
        };
    }

    public static EmployeerViewModel ToViewModel(this EmployeerEntity x)
    {
        return new EmployeerViewModel()
        {
            Id = x.id,
            Name = x.name,
            Created = x.created
        };
    }

    public static EmployeerPaymentViewModel ToViewModel(this EmployeerPaymentEntity x)
    {
        return new EmployeerPaymentViewModel
        {
            Id = x.id,
            Amount = x.amount,
            EmployeerId = x.employeer_id,
            TransferArrivalDate = x.transfer_arrival_date
        };
    }

    public static EmployeerPaymentEntity ToEntity(this EmployeerPaymentViewModel x)
    {
        return new EmployeerPaymentEntity
        {
            id = x.Id,
            amount = x.Amount,
            employeer_id = x.EmployeerId,
            transfer_arrival_date = x.TransferArrivalDate
        };
    }
}
