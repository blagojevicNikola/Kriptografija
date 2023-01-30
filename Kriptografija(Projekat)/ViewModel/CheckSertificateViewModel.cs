using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class CheckSertificateViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private string _sertificatePath = "";
        public string SertificatePath { 
            get 
            { 
                return _sertificatePath; 
            } 
            set
            { 
                _sertificatePath = value; 
                NotifyPropertyChanged("SertificatePath"); 
            } 
        }
        public ICommand CheckCommand { get; set; }
        public ICommand NavigateRegisterCommand { get; set; }
        public ICommand FindSertificateCommand { get; set; }
        public CheckSertificateViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            CheckCommand = new NavigateCommand<LoginViewModel>(navigationStore, () => new LoginViewModel(navigationStore));
            NavigateRegisterCommand = new NavigateCommand<RegisterViewModel>(navigationStore, () => new RegisterViewModel(navigationStore));
            FindSertificateCommand = new RelayCommand(() => findSertificate());
        }


        private void findSertificate()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                SertificatePath = dialog.FileName; 
            }
        }
    }
}
