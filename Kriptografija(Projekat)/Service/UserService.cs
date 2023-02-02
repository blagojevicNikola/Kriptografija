using Kriptografija_Projekat_.Model;
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

namespace Kriptografija_Projekat_.Service
{
    public class UserService
    {
        public UserService() { }

        public UserFile UploadFile(User user, string filePath)
        {
            FileSystemService fsService = new FileSystemService();
            UserFile result = fsService.AddFile(user.Username, filePath, user.KeyPair);
            return result;
        }

         
    }
}
