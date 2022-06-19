using System;
using System.Collections.ObjectModel;
using TodoList.WPF.DataAccess;
using TodoList.WPF.ViewModels;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

namespace TodoList.WPF.Models;

public class EmployeerViewModel : ViewModelBase
{
    public int ActiveTasksCount => TodoItems?.Count(t => !t.IsCompleted) ?? 0;
    public int Id { get; init; }

    string name;
    private IJobItemRepository jobItemRepository;
    private IMapper mapper;
    private IEmployeerPaymentRepository employeerPaymentRepository;

    public string Name
    {
        get => name;
        set => Set(ref name, value);
    }
    public DateTime Created { get; set; } = DateTime.Now;
    public ObservableCollection<EmployeerPaymentViewModel> Payments { get; init; }

    public ObservableCollection<JobItemViewModel> TodoItems { get; init; }

    public EmployeerViewModel(int id, string name)
    {
        TodoItems = new ObservableCollection<JobItemViewModel>();
        Payments = new ObservableCollection<EmployeerPaymentViewModel>();
        TodoItems.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(ActiveTasksCount));
        Id = id;
        Name = name;
    }

    public EmployeerViewModel(int id, string name, 
        IMapper mapper, 
        IJobItemRepository jobItemRepository, 
        IEmployeerPaymentRepository employeerPaymentRepository) : this(id, name)
    {
        this.jobItemRepository = jobItemRepository;
        this.employeerPaymentRepository = employeerPaymentRepository;
        this.mapper = mapper;

        foreach(var item in mapper.Map<IEnumerable<JobItemViewModel>>(jobItemRepository.GetAllJobItems(Id)))
        {
            TodoItems.Add(item);
        }
        foreach(var item in mapper.Map<IEnumerable<EmployeerPaymentViewModel>>(employeerPaymentRepository.GetAllPaymentsForEmployeer(Id)))
        {
            Payments.Add(item);
        }
    }
}
