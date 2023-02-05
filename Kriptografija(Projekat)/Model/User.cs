using Kriptografija_Projekat_.Service;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kriptografija_Projekat_.Model
{
    public class User
    {
        private X509Certificate _cert;
        private AsymmetricCipherKeyPair _keyPair;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        public AsymmetricCipherKeyPair KeyPair { get { return _keyPair; } }
        public X509Certificate Cert { get { return _cert; } }

        public User(string username, string password, string email, X509Certificate cert)
        {
            _cert = cert;
            Username = username;
            Password = password;
            Email = email;
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Users"]! + @"\" + username + @"\" + username + ".pem"))
                _keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
        }

        public string GetPassword()
        {
            string filePath = ConfigurationManager.AppSettings["Users"]! + @"\" + Username + @"\" + Username + ".pass";
            CryptoService cryptoService = new CryptoService();
            byte[] input = File.ReadAllBytes(filePath);
            byte[] password = cryptoService.RsaDecryptWithPrivate(input, KeyPair.Private);
            return Encoding.UTF8.GetString(password);
        }
    }
}
