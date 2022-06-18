using MahApps.Metro.IconPacks;
using System;
using System.IO;
using System.Net;
using System.Windows.Input;
using TodoList.WPF.Infrastructure.Extensions;

namespace TodoList.WPF.ViewModels;

public class BookToReadViewModel : ViewModelBase
{
    public static readonly string PLACEHOLDER_IMAGE = "Assets/Image/ReadList/book-placeholder.png";
    public int Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }

    string image;
    public string Image
    {
        get => image;
        set
        {
            if(Set(ref image, value))
            {
                RaisePropertyChanged(nameof(CachedImage));
            }
        }
    }

    //Тут идет скачивание удаленного файла по URL из [Image], если еще не закачан
    public string CachedImage
    {
        get => GetCacheImage();
    }

    public DateTime DateAdded { get; set; }
    
    bool isAlreadyReaded;
    public bool IsAlreadyReaded
    {
        get => isAlreadyReaded;
        set
        {
            if(Set(ref isAlreadyReaded, value))
            {
                DateEnded = value ? (DateTime?)DateTime.Now : null;
                RaisePropertyChanged(nameof(StateIcon));
            }
        }
    }
    public DateTime? DateEnded { get; set; }

    public ICommand ToggleStatusCommand { get; }

    public BookToReadViewModel()
    {
        ToggleStatusCommand = new LambdaCommand(e => IsAlreadyReaded = !IsAlreadyReaded);
    }

    public PackIconFontAwesomeKind StateIcon
    {
        get => IsAlreadyReaded ? PackIconFontAwesomeKind.EyeSlashRegular : PackIconFontAwesomeKind.EyeRegular;
    }

    private string GetCacheImage()
    {
        if (string.IsNullOrWhiteSpace(Image))
        {
            return PLACEHOLDER_IMAGE;
        }

        try
        {
            string cacheDirectory = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "cache");
            string cacheFile = Path.Combine(cacheDirectory, Image.GetMD5() + Path.GetExtension(Image));

            if (!File.Exists(cacheFile))
            {
                if (!Directory.Exists(cacheFile))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }
                new WebClient().DownloadFile(Image, cacheFile);              
            }
            return cacheFile;
        }
        catch
        {
            return PLACEHOLDER_IMAGE;
        }
        
    }
}

