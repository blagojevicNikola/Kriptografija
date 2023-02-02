using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Kriptografija_Projekat_.ViewModel
{
    public class FilesOverviewViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;

        public ICommand AddWindowCommand { get; set; }
        public FilesOverviewViewModel(NavigationStore navigationStore) 
        {
            _navigationStore = navigationStore;
            AddWindowCommand = new RelayCommand(() => addWindow());
        }

        private void addWindow()
        {
            AddFileWindow win = new AddFileWindow();
            AddFileViewModel vm = (AddFileViewModel)win.DataContext;
            vm.CloseWindow += (a, e) => win.Close();
            win.Show();
        }
    }
}
