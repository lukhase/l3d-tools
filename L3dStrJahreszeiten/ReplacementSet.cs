using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L3dStrJahreszeiten
{
    public class ReplacementSet
    {
        public class Replacement
        {
            public string OriginalName { get; set; }
            public string NewName { get; set; }

            public override string ToString()
            {
                return OriginalName + " => " + NewName;
            }
        }

        public ReplacementSet()
        {
            Fahrplaene = new List<Replacement>();
            Kursbuchstrecken = new List<Replacement>();
            Strecken = new List<Replacement>();
            Ersetzungen = new List<Replacement>();
            CsvFilename = System.IO.Path.GetRandomFileName() + ".csv";
        }

        public string Name { get; set; }
        public List<Replacement> Ersetzungen { get; set; }
        public List<Replacement> Fahrplaene { get; set; }
        public List<Replacement> Kursbuchstrecken { get; set; }
        public List<Replacement> Strecken { get; set; }

        public string CsvFilename { get; set; }
    }
}
