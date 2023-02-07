using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private X509Certificate _cert;
        private User _user;
        private int _numOfAttempts = 0;
        private string _errorTmp = "Login with correct credientals to withdrawn your certificate!";
        private string _errorMessage = "";
        public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged("Username"); } }
        public string Password { get { return _password; } set { _password = value; NotifyPropertyChanged("Password"); } }
        public string ErrorMessage { get { return _errorMessage; } set { _errorMessage = value; NotifyPropertyChanged("ErrorMessage"); } }
        public ICommand NavigateBackCommand { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand NavigateFileOvereviewCommand { get; set; }
        public ICommand NavigateRegistrationCommand { get; set; }
        public LoginViewModel(NavigationStore navigationStore, X509Certificate cert)
        {
            _navigationStore = navigationStore;
            NavigateFileOvereviewCommand = new NavigateCommand<FilesOverviewViewModel>(navigationStore, () => new FilesOverviewViewModel(navigationStore, _user!));
            NavigateBackCommand = new NavigateCommand<CheckSertificateViewModel>(navigationStore, () => new CheckSertificateViewModel(navigationStore));
            LoginCommand = new RelayCommand(() => login());
            NavigateRegistrationCommand = new NavigateCommand<RegisterViewModel>(navigationStore, () => new RegisterViewModel(navigationStore));
            _cert = cert;
        }

        public void login()
        {
            _numOfAttempts++;
            if(_numOfAttempts<4)
            {
                DataBaseService dbService = new DataBaseService();
                if (dbService.UserExists(Username, Password))
                {
                    _user = new User(Username, Password, "mejl", _cert);
                    NavigateFileOvereviewCommand.Execute(null);
                }
                else
                {
                    if(_numOfAttempts==3)
                    {
                        CertificateConfigService service = new CertificateConfigService();
                        service.RevokeCertificate(_cert);
                        MessageBox.Show("Wrong credentials! Your certificate has been revoked!");
                        ErrorMessage = _errorTmp;
                        return;
                    }
                    MessageBox.Show("Wrong credentials! Try again!");
                }
                return;
            }
            if(_numOfAttempts>3)
            {
                
                DataBaseService dbService = new DataBaseService();
                if (dbService.UserExists(Username, Password))
                {
                    _user = new User(Username, Password, "mejl", _cert);
                    CertificateConfigService service = new CertificateConfigService();
                    if(service.WithdrawnRevocation(_cert))
                    {
                        NavigateFileOvereviewCommand.Execute(null);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Error while removing revocation!");
                        NavigateBackCommand.Execute(null);
                        return;
                    }
                    
                }
                else
                {
                    NavigateRegistrationCommand.Execute(null);
                }
            }   
            //_numOfAttempts++;
            //DataBaseService dbService = new DataBaseService();
            //if(dbService.UserExists(Username, Password) && _numOfAttempts<4)
            //{
            //    _user = new User(Username, Password, "mejl", _cert);
            //    NavigateFileOvereviewCommand.Execute(null);
            //}
            //else
            //{
            //    if(_numOfAttempts == 3)
            //    {
            //        CertificateConfigService service = new CertificateConfigService();
            //        service.RevokeCertificate(_cert);
            //        MessageBox.Show("Wrong credentials! Your certificate is revoked!");
            //    }
            //}
        }
    }
}
