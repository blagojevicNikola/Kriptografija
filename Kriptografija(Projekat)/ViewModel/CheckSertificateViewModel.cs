using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Microsoft.Win32;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
        private Org.BouncyCastle.X509.X509Certificate? _cert;
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
            NavigateMainView = new NavigateCommand<LoginViewModel>(navigationStore, () => new LoginViewModel(navigationStore, _cert!));
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
            if (!File.Exists(SertificatePath))
            {
                MessageBox.Show("Selected file dosen't exist!");
                return;
            }

            CertificateConfigService service = new CertificateConfigService();
            //service.CreateSelfSigned();
            //service.BuildCRL();
            Org.BouncyCastle.X509.X509Certificate? cert = service.ValidateCertificate(SertificatePath);
            if (cert != null)
            {
                _cert = cert;
                NavigateMainView.Execute(null);
            }
            else
            {
                MessageBox.Show("Sertificate is not valid!");
            }
            //Org.BouncyCastle.X509.X509Certificate issuer = new Org.BouncyCastle.X509.X509Certificate(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            //var list = issuer.GetKeyUsage();
            //foreach (var l in list)
            //{
            //    Debug.WriteLine(l);
            //}
        }
    }
}
