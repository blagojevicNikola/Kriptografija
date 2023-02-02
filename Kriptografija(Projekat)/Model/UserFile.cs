using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.Model
{
    public class UserFile
    {   
        private List<Segment> _segments;
        public string Name { get; set; }
       
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
    }
}
