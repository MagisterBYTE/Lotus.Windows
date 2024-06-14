using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Win32;

namespace Lotus.App.FileSystem
{
    public class Folder : ObservableObject
    {
        bool _isSelected;
        string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
             }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ParentPath { get; set; }

        public ObservableCollection<Folder> Folders { get; set; } = new ObservableCollection<Folder>();

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void buttonSetParentFolder_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if(openFolderDialog.ShowDialog().GetValueOrDefault())
            {
                ParentPath = openFolderDialog.FolderName;
                textParentFolder.Text = openFolderDialog.FolderName;
            }

        }

        private void buttonFindEmptyFolders_Click(object sender, RoutedEventArgs e)
        {
            Folders.Clear();
            foreach (var dir in Directory.EnumerateDirectories(ParentPath, "*", SearchOption.AllDirectories))
            {
                bool empty = (Directory.GetDirectories(dir).Length == 0 && Directory.GetFiles(dir).Length == 0);
                if(empty)
                {
                    Folder folder = new Folder();
                    folder.Name = dir;
                    Folders.Add(folder);
                }
            }

            listBoxEmptyFolders.ItemsSource = Folders;
        }

        private void buttonRemoveEmptyFolders_Click(object sender, RoutedEventArgs e)
        {
            foreach (var folder in Folders)
            {
                if(folder.IsSelected)
                {
                    Directory.Delete(folder.Name);
                }
            }
        }

        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var folder in Folders)
            {
                folder.IsSelected = true;
            }
        }

        private void buttonInverseSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (var folder in Folders)
            {
                folder.IsSelected = !folder.IsSelected;
            }
        }
    }
}