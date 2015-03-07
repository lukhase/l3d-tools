using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace L3dStrJahreszeiten
{
    public class Ersetzungsprojekt
    {
        public Ersetzungsprojekt()
        {
            Sets = new List<ReplacementSet>();
        }

        public string ProjectDirectory { get; set; }
        public List<ReplacementSet> Sets { get; set; }
        public string LoksimDirectory { get; set; }

        [XmlIgnore()]
        public string Name
        {
            get
            {
                return System.IO.Path.GetFileName(ProjectDirectory);
            }
        }
    }
}
