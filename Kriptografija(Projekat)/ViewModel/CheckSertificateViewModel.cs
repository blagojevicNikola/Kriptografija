using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Stores;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public ICommand NavigateMainView { get; set; }
        public ICommand NavigateRegisterCommand { get; set; }
        public ICommand FindSertificateCommand { get; set; }
        public CheckSertificateViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateMainView = new NavigateCommand<LoginViewModel>(navigationStore, () => new LoginViewModel(navigationStore));
            CheckCommand = new RelayCommand(() => checkSertificate());
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

        private void checkSertificate()
        {
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(SertificatePath));
            X509Certificate2 ca = new X509Certificate2(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Clear();
            chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.VerificationTime = DateTime.Now;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

            chain.ChainPolicy.ExtraStore.Add(ca);

            bool isChainValid = chain.Build(cert);

            var valid = chain.ChainElements.Cast<X509ChainElement>().Any(x => x.Certificate.Thumbprint == ca.Thumbprint);

            if(valid && isChainValid)
            {
                NavigateMainView.Execute(null);
            }
            else
            {
                MessageBox.Show("Sertificate is not valid!");
            }
        }
    }
}
