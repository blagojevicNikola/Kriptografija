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

        public bool UserExists(string username, string password) 
        {
            string[] lines = File.ReadAllLines(ConfigurationManager.AppSettings["DB"]!);
            foreach (string line in lines)
            {
                string[] data = line.Split('$');
                if (data.Length != 2) 
                {
                    continue;
                }
                if (username == data[0] && password == data[1])
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddCredentials(string username, string password)
        {
            if(!checkUsername(username))
            {
                File.AppendAllText(ConfigurationManager.AppSettings["DB"]!, username + "$" + password + Environment.NewLine);
                return true;
            }
            return false;
        }

        private bool checkUsername(string username)
        {
            string[] lines = File.ReadAllLines(ConfigurationManager.AppSettings["DB"]!);
            foreach (string line in lines)
            {
                string[] data = line.Split('$');
                if (data.Length != 2)
                {
                    continue;
                }
                if (username.Equals(data[0]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
