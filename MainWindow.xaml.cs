using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAlbom.Models;
using WpfAlbom.Services;

namespace WpfAlbom
{

    //public class MainViewModel : INotifyPropertyChanged
    //{
    //    private readonly DataService dataService;
    //    private Album selectedAlbum;
    //    private ObservableCollection<Album> albums;
    //    private ObservableCollection<Photo> photos;

    //    public MainViewModel()
    //    {
    //        dataService = new DataService();
    //        LoadData();
    //    }

    //    public ObservableCollection<Album> Albums
    //    {
    //        get => albums;
    //        set
    //        {
    //            albums = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public ObservableCollection<Photo> Photos
    //    {
    //        get => photos;
    //        set
    //        {
    //            photos = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public Album SelectedAlbum
    //    {
    //        get => selectedAlbum;
    //        set
    //        {
    //            selectedAlbum = value;
    //            OnPropertyChanged();
    //            LoadPhotos();
    //        }
    //    }

    public partial class MainWindow : Window
    {
        private List<Album> albums = new List<Album>();
        private List<Photo> photos = new List<Photo>();
        private readonly DataService dataService;

        public MainWindow()
        {
            InitializeComponent();
            dataService = new DataService();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                this.albums = await dataService.GetAlbumsAndPhotoAsync<Album>("https://jsonplaceholder.typicode.com/albums");
                this.photos = await dataService.GetAlbumsAndPhotoAsync<Photo>("https://jsonplaceholder.typicode.com/photos");

                AlbumListBox.ItemsSource = albums;
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumListBox.SelectedItem is Album selectedAlbum)
            {
                PhotoListBox.ItemsSource = photos.FindAll(photo => photo.AlbumId == selectedAlbum.Id);
            }
        }
    }
}

