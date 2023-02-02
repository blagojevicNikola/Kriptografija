using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private string _username = "";
        private string _password = "";

        public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged("Username"); } }
        public string Password { get { return _password; } set { _password = value; NotifyPropertyChanged("Password"); } }
        public ICommand NavigateBackCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand NavigateFileOvereviewCommand { get; set; }
        public LoginViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateFileOvereviewCommand = new NavigateCommand<FilesOverviewViewModel>(navigationStore, () => new FilesOverviewViewModel(navigationStore));
            NavigateBackCommand = new NavigateCommand<CheckSertificateViewModel>(navigationStore, () => new CheckSertificateViewModel(navigationStore));
            LoginCommand = new RelayCommand(() => login());
        }

        public void login()
        {
            DataBaseService dbService = new DataBaseService();
            if(dbService.UserExists(Username, Password))
            {
                NavigateFileOvereviewCommand.Execute(null);
            }
            else
            {
                MessageBox.Show("Wrong credentials!");
            }
        }
    }
}
