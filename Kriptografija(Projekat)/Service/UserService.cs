using Kriptografija_Projekat_.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Diagnostics;

namespace Kriptografija_Projekat_.Service
{
    public class UserService
    {
        public UserService() { }

        public UserFile? UploadFile(User user, string filePath)
        {
            if(!File.Exists(filePath)) 
            {
                return null;
            }
            FileSystemService fsService = new FileSystemService();
            UserFile result = fsService.AddFile(user.Username, filePath, user.KeyPair);
            return result;
        }

        public void SetupUserEnvironment(User user)
        {
            string folderName = createUserFolder(user.Username);
            File.Create(folderName + @"\" + "info.txt");
        }

        public IEnumerable<UserFile> GetUserFiles(User user) 
        {
            ObservableCollection<UserFile> userFiles= new ObservableCollection<UserFile>();
            string userFolderName = ConfigurationManager.AppSettings["Users"] + @"\" + user.Username;
            string text = File.ReadAllText(userFolderName);
            byte[] textArr = Convert.FromBase64String(text);
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            encryptEngine.Init(false, user.KeyPair.Private);
            string content = Encoding.Default.GetString(encryptEngine.ProcessBlock(textArr, 0, textArr.Length));
            string[] list = content.Split("#");
            
        }

        private string createUserFolder(string username)
        {
            string userFolderName = ConfigurationManager.AppSettings["Users"] + @"\" + username;
            if (!File.Exists(userFolderName)) 
            {
                Directory.CreateDirectory(userFolderName);
            }
            return userFolderName;
        }
         
    }
}
