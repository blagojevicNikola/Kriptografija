using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Model
{
    public class Segment
    {
        private string _filePath;
        private string _signaturePath;

        public Segment(string filePath, string signaturePath)
        {
            _filePath = filePath;
            _signaturePath = signaturePath;
        }

        override
        public string ToString() 
        {
            return _filePath + "|" + _signaturePath;
        }
    }
}
