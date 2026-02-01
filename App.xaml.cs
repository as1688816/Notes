using System.Windows;
using NotesApp.Services;
using NotesApp.ViewModels;

namespace NotesApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var storageService = new NoteStorageService();
        var vm = new MainViewModel(storageService);

        var window = new MainWindow();
        window.DataContext = vm;
        window.Show();
    }
}
