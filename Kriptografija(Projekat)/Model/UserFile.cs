using Kriptografija_Projekat_.Service;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Model
{
    public class UserFile: INotifyPropertyChanged
    {   
        private List<Segment> _segments;
        private bool _isSelected = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; set; }
        public bool IsSelected { get {  return _isSelected; } set { _isSelected = value; NotifyPropertyChanged("IsSelected"); } }
        public UserFile(string name)
        {
            Name = name;
            _segments = new List<Segment>();
        }

        public UserFile(string name, List<Segment> segments)
        {
            Name = name;
            _segments = segments;
        }

        public void AddSegment(Segment segment)
        {
            _segments.Add(segment);
        }

        public bool DownloadFile(string path, AsymmetricCipherKeyPair keyPair)
        {
            byte[]? result = GetContent(keyPair);
            if(result == null) 
            {
                return false;
            }
            File.WriteAllBytes(path+@"\"+Name, result);
            return true;
        }

        public byte[]? GetContent(AsymmetricCipherKeyPair keyPair)
        {
            CryptoService cryptoService = new CryptoService();
            List<byte[]> arr = new List<byte[]>();
            foreach (Segment segment in _segments)
            {
                byte[]? tmp = segment.Load(keyPair, cryptoService);
                if (tmp == null)
                {
                    return null;
                }
                arr.Add(tmp);
            }
            int len = arr.Select(s => s.Length).Sum();
            byte[] result = new byte[len];
            int offset = 0;
            foreach (byte[] array in arr)
            {
                System.Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }
            return result;
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(name, new PropertyChangedEventArgs(name));
        }

        override
        public string ToString()
        {
            string result = Name;
            foreach(Segment segment in _segments)
            {
                result += "*" + segment.ToString();
            }
            return result + "$";
        }
    }
}
