using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Service
{
    public class DataBaseService
    {
        public DataBaseService() { }

        public bool UserExists(string username, string password, string mail)
        {
            string text = GetDbContent();
            if (text == "")
            {
                return false;
            }
            string[] lines = text.Split("#");
            for(int i = 0; i < lines.Length-1; i++)
            {
                string[] data = lines[i].Split("$");
                if (data[0].Equals(username) && data[1].Equals(password) && data[2].Equals(mail))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddCredentials(string username, string password, string email)
        {
            if(!checkUsername(username))
            {
                string content = GetDbContent();
                content += username + "$" + password + "$" + email + "#";
                WriteDbContent(content);
                return true;
            }
            return false;
        }

        private string GetDbContent()
        {
            string base64 = File.ReadAllText(ConfigurationManager.AppSettings["DB"]!);
            byte[] input = Convert.FromBase64String(base64);
            CryptoService cryptService = new CryptoService();
            byte[] output = cryptService.AesSystemDecrypt(base64, "sigurnost");
            string text = Encoding.UTF8.GetString(output);
            return text;
        }

        private void WriteDbContent(string content)
        {
            CryptoService cryptService = new CryptoService();
            byte[] input = cryptService.AesSystemEncrypt(Encoding.UTF8.GetBytes(content), "sigurnost");
            File.WriteAllText(ConfigurationManager.AppSettings["DB"]!, Convert.ToBase64String(input));
        }

        private bool checkUsername(string username)
        {
            string text = GetDbContent();
            if (text == "")
            {
                return false;
            }
            string[] lines = text.Split("#");
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] data = lines[i].Split("$");
                if (data[0].Equals(username))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
