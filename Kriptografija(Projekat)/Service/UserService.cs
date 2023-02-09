using Kriptografija_Projekat_.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Diagnostics;
using System.Windows.Input;

namespace Kriptografija_Projekat_.Service
{
    public class UserService
    {
        public UserService() { }

        public void UploadInfo(User user, UserFile userFile)
        {
            List<UserFile> files = this.GetUserFiles(user).ToList();
            files.Add(userFile);
            string content = "";
            files.ForEach(s => content += s);
            byte[] contentArr = Encoding.UTF8.GetBytes(content);
            CryptoService cryptoService = new CryptoService();
            byte[] symmetricOutput = cryptoService.AesSystemEncrypt(contentArr, user.GetPassword());
            File.WriteAllText(ConfigurationManager.AppSettings["Users"] + @"\" + user.Username +
                @"\info.txt", Convert.ToBase64String(symmetricOutput));
        }

        public void SetupUserEnvironment(string username)
        {
            string folderName = createUserFolder(username);
            File.Create(folderName + @"\info.txt").Close();
        }

        public void SetUserPassword(User user)
        {
            Random rand = new Random();
            string folderPath = ConfigurationManager.AppSettings["Users"]! + @"\" + user.Username;
            string password = user.Password + rand.Next(1000).ToString();
            while(password.Length <16)
            {
                password += password;
            }
            byte[] passwordArr = Encoding.UTF8.GetBytes(password, 0, 16);
            CryptoService cryptoService = new CryptoService();
            byte[] encrypted = cryptoService.RsaEncryptWithPublic(passwordArr, user.KeyPair.Public);
            File.WriteAllBytes(folderPath + @"\" + user.Username + ".pass", encrypted);
        }

        public IEnumerable<UserFile> GetUserFiles(User user)
        {
            ObservableCollection<UserFile> userFiles = new ObservableCollection<UserFile>();
            string userFolderName = ConfigurationManager.AppSettings["Users"] + @"\" + user.Username;
            string text = File.ReadAllText(userFolderName+@"\info.txt");
            if (string.IsNullOrEmpty(text))
            {
                return userFiles;
            }
            if(text.Length==0)
            {
                return userFiles;
            }

            CryptoService cryptoService = new CryptoService();
            byte[] inputArr = cryptoService.AesSystemDecrypt(text, user.GetPassword());
            
            string content = Encoding.UTF8.GetString(inputArr);
            string[] list = content.Split("$", StringSplitOptions.TrimEntries);
            Debug.WriteLine(list.Length);
            for (int i = 0; i < list.Length-1; i++)
            {
                string tmp = list[i];
                if(tmp=="")
                {
                    continue;
                }
                string[] sublist = tmp.Split("*");
                string fileName = sublist[0];
                List<Segment> segments = new List<Segment>();
                for(int j = 1; j < sublist.Length; j++)
                {
                    string[] data = sublist[j].Split("|");
                    segments.Add(new Segment(data[0], data[1]));
                }
                userFiles.Add(new UserFile(fileName, segments));
            }
            //Debug.WriteLine(content);

            return userFiles;
        }

        private string createUserFolder(string username)
        {
            string userFolderName = ConfigurationManager.AppSettings["Users"] + @"\" + username;
            if (!Directory.Exists(userFolderName)) 
            {
                Directory.CreateDirectory(userFolderName);
            }
            return userFolderName;
        }
         
    }
}
