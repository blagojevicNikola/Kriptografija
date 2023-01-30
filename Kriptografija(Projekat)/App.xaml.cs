using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.View;
using Kriptografija_Projekat_.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Kriptografija_Projekat_
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            NavigationStore navigationStore = new();
            navigationStore.CurrentViewModel = new CheckSertificateViewModel(navigationStore);
            MainWindow win = new()
            {
                DataContext = new MainWindowViewModel(navigationStore)
            };
            win.Show();
            base.OnStartup(e);
        }
    }
}
