using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Microsoft.Win32;
using Org.BouncyCastle.Asn1.Cmp;
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
        private X509Certificate? _cert;
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
            if(!File.Exists(SertificatePath))
            {
                MessageBox.Show("Selected file dosen't exist!");
                return;
            }
            //X509Certificate bc = new X509Certificate(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!));
            //X509V3CertificateGenerator gen = new X509V3CertificateGenerator();
            CertificateConfigService service = new CertificateConfigService();
            //X509V2CrlGenerator crlgen = new X509V2CrlGenerator();
            //X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(SertificatePath), "sigurnost");
            //X509Certificate bc = new Org.BouncyCastle.X509.X509Certificate();
            //X509Certificate2 ca = new X509Certificate2(File.ReadAllBytes(ConfigurationManager.AppSettings["Path"]!), "sigurnost");
            ////TestService service = new TestService(ca);
            ////service.SignNewCert();
            //Debug.WriteLine(cert.ToString());
            //X509Chain chain = new X509Chain();
            //chain.ChainPolicy.ExtraStore.Clear();
            //chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            //chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            //chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            //chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
            //chain.ChainPolicy.VerificationTime = DateTime.Now;
            //chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

            //chain.ChainPolicy.ExtraStore.Add(ca);

            //bool isChainValid = chain.Build(cert);

            //var valid = chain.ChainElements.Cast<X509ChainElement>().Any(x => x.Certificate.Thumbprint == ca.Thumbprint);
            //Debug.WriteLine(isChainValid + " - " + valid);
            //for (int i = 0; i < chain.ChainStatus.Length; i++)
            //{
            //    Debug.WriteLine(chain.ChainStatus[i].StatusInformation);
            //}
            Org.BouncyCastle.X509.X509Certificate? cert = service.ValidateCertificate(SertificatePath);
            if (cert!=null)
            {
                _cert = cert;
                NavigateMainView.Execute(null);
            }
            else
            {
                MessageBox.Show("Sertificate is not valid!");
            }
        }
    }
}
