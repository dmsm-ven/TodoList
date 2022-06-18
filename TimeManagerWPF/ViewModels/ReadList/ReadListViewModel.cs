using AutoMapper;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TodoList.WPF.DataAccess.Entities;
using TodoList.WPF.DataAccess.Repositories;

namespace TodoList.WPF.ViewModels;

public class ReadListViewModel : ViewModelBase
{
    public readonly string default_book_name = "Новая книга";
    public List<BookToReadViewModel> FilteredBooks
    {
        get
        {
            return Books
                .Where(book => IsShowHidden ? true : !book.IsAlreadyReaded)
                .OrderByDescending(book => book.DateAdded)
                .ToList();
        }
    }
    public ObservableCollection<BookToReadViewModel> Books { get; }

    
    private readonly IBookToReadRepository repository;
    private readonly IMapper mapper;
    public string ShowHiddenTitle
    {
        get => IsShowHidden ? "Скрыть прочитанные" : "Отобразить прочитаные";
    }

    bool isAddNewBookPanelShow;
    public bool IsAddNewBookPanelShow
    {
        get => isAddNewBookPanelShow;
        set
        {
            if(Set(ref isAddNewBookPanelShow, value))
            {
                if (IsAddNewBookPanelShow)
                {
                    NewBook = new BookToReadViewModel()
                    {
                        Name = default_book_name,
                        Image = "https://google.com/",
                        DateAdded = DateTime.Now
                    };
                    NewBook.PropertyChanged += Item_PropertyChanged;
                }
                RaisePropertyChanged(nameof(AddBookIconStateKind));
            }
        }
    }
    bool isShowHidden;
    public bool IsShowHidden
    {
        get => isShowHidden;
        set
        {
            if(Set(ref isShowHidden, value))
            {
                RaisePropertyChanged(nameof(FilteredBooks));
                RaisePropertyChanged(nameof(ShowHiddenIconState));
                RaisePropertyChanged(nameof(ShowHiddenTitle));
            }
        }
    }

    BookToReadViewModel newBook;
    public BookToReadViewModel NewBook
    {
        get => newBook;
        set => Set(ref newBook, value);
    }  
    public PackIconFontAwesomeKind AddBookIconStateKind
    {
        get => IsAddNewBookPanelShow ? PackIconFontAwesomeKind.CheckSolid : PackIconFontAwesomeKind.PlusSolid;
    }
    public PackIconFontAwesomeKind ShowHiddenIconState
    {
        get => IsShowHidden ? PackIconFontAwesomeKind.EyeSlashSolid : PackIconFontAwesomeKind.EyeSolid;
    }
    public ICommand AddBookCommand { get; }
    public ICommand ShowHiddenToggleCommand { get; }
    public ICommand LoadedCommand { get; }
    public ReadListViewModel()
    {
        AddBookCommand = new LambdaCommand(AddBook, e => NewBook == null || NewBook?.Name != default_book_name);
        ShowHiddenToggleCommand = new LambdaCommand(e => IsShowHidden = !IsShowHidden);
        LoadedCommand = new LambdaCommand(Loaded);
        Books = new ObservableCollection<BookToReadViewModel>()
        {
            new BookToReadViewModel() { Name = "Книга 1" },
            new BookToReadViewModel() { Name = "Книга 2" },
            new BookToReadViewModel() { Name = "Книга 3" },
            new BookToReadViewModel() { Name = "Книга 5" },
            new BookToReadViewModel() { Name = "Книга 6" },
            new BookToReadViewModel() { Name = "Книга 7" },
        };
        Books.CollectionChanged += (o, e) => RaisePropertyChanged(nameof(FilteredBooks));
    }
    public ReadListViewModel(IBookToReadRepository repository, IMapper mapper) : this()
    {
        this.repository = repository;
        this.mapper = mapper;
    }
    private void Loaded(object obj)
    {
        Books?.ToList().ForEach(item => item.PropertyChanged -= Item_PropertyChanged);

        Books?.Clear();

        repository.GetAll().ToList()
            .ForEach(item =>
            {
                var vmItem = mapper.Map<BookToReadViewModel>(item);
                Books.Add(vmItem);
                vmItem.PropertyChanged += Item_PropertyChanged;
            });
    }
    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BookToReadViewModel.IsAlreadyReaded))
        {
            RaisePropertyChanged(nameof(FilteredBooks));
            repository.AddOrUpdate(mapper.Map<BookToReadEntity>(sender as BookToReadViewModel));
        }
    }
    private void AddBook(object obj)
    {
        if (IsAddNewBookPanelShow)
        {
            NewBook.Id = repository.AddOrUpdate(mapper.Map<BookToReadEntity>(NewBook));
            Books.Insert(0, NewBook);
        }

        IsAddNewBookPanelShow = !IsAddNewBookPanelShow;

    }
}

