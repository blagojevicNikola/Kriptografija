using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private string _name="";
        private string _email="";
        private string _username = "";
        private string _password = "";
        private X509Certificate? _cert;

        public string Name { get { return _name; } set { _name = value; NotifyPropertyChanged("Name"); } }
        public string Email { get { return _email; } set { _email = value;NotifyPropertyChanged("Email"); } }
        public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged("Username"); } }
        public string Password { get { return _password; } set { _password=value; NotifyPropertyChanged("Password"); } }

        public ICommand NavigateBackCommand { get; set; }
        public ICommand NavigateLoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public RegisterViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateBackCommand = new NavigateCommand<CheckSertificateViewModel>(navigationStore, () => new CheckSertificateViewModel(navigationStore));
            NavigateLoginCommand = new NavigateCommand<LoginViewModel>(navigationStore, () => new LoginViewModel(navigationStore, _cert));
            RegisterCommand = new RelayCommand(() => register());
        }

        public void register()
        {
            CertificateConfigService certService = new CertificateConfigService();
            DataBaseService dbService = new DataBaseService();
            try
            {
                if(dbService.AddCredentials(Username, Password))
                {
                    _cert = certService.SignCert(Username);
                    NavigateLoginCommand.Execute(null);
                }
                else
                {
                    MessageBox.Show("Username is used!");
                }
                
            }catch(Exception e)
            {
                MessageBox.Show("Error while registrating!");
            }
        }
    }
}
