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
            Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            byte[] key = Encoding.Default.GetBytes("sigurnostsigurno");
            CbcBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher);
            cipher.Init(true, new ParametersWithIV(new KeyParameter(key), key, 0, key.Length));
            byte[] symmetricOutput = new byte[cipher.GetOutputSize(contentArr.Length)];
            int len = cipher.ProcessBytes(contentArr, symmetricOutput, 0);
            cipher.DoFinal(symmetricOutput, len);
            //encryptEngine.Init(true, user.KeyPair.Public);
            
            //byte[] outputArr = encryptEngine.ProcessBlock(contentArr, 0, contentArr.Length);
            File.WriteAllText(ConfigurationManager.AppSettings["Users"] + @"\" + user.Username +
                @"\info.txt", Convert.ToBase64String(symmetricOutput));
        }

        public void SetupUserEnvironment(string username)
        {
            string folderName = createUserFolder(username);
            File.Create(folderName + @"\info.txt").Close();
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
            byte[] textArr = Convert.FromBase64String(text);
            if(textArr.Length==0)
            {
                return userFiles;
            }
            byte[] key = Encoding.Default.GetBytes("sigurnostsigurno");
            CbcBlockCipher blockCipher = new CbcBlockCipher(new AesEngine());
            PaddedBufferedBlockCipher cipher = new PaddedBufferedBlockCipher(blockCipher);
            cipher.Init(false, new ParametersWithIV(new KeyParameter(key), key, 0, key.Length));
            byte[] inputArr = new byte[cipher.GetOutputSize(textArr.Length)];
            int len = cipher.ProcessBytes(textArr, inputArr, 0);
            cipher.DoFinal(inputArr, len);
            //Pkcs1Encoding encryptEngine = new Pkcs1Encoding(new RsaEngine());
            //encryptEngine.Init(false, user.KeyPair.Private);
            //string content = Encoding.Default.GetString(encryptEngine.ProcessBlock(textArr, 0, textArr.Length));
            string content = Encoding.Default.GetString(inputArr);
            string[] list = content.Split("$");
            Debug.WriteLine(list.Length);
            Debug.WriteLine(list[1]);
            foreach(string l in list)
            {
                string tmp = l.Trim();
                if(tmp=="")
                {
                    continue;
                }
                string[] sublist = tmp.Split("*");
                string fileName = sublist[0];
                List<Segment> segments = new List<Segment>();
                for(int i = 1; i < sublist.Length; i++)
                {
                    string[] data = sublist[i].Split("|");
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
