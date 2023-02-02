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
            using (var reader = File.OpenText(ConfigurationManager.AppSettings["Main"]! + @"\" + username + ".pem"))
                _keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
        }
    }
}
