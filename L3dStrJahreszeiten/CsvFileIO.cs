using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L3dStrJahreszeiten
{
    class CsvFileIO
    {
        public static IEnumerable<ReplacementEntry> ReadFromCsv(string filePath)
        {
            List<ReplacementEntry> entries = new List<ReplacementEntry>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    var cols = line.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                    if (cols.Count() > 2)
                    {
                        bool vis = true;
                        if (cols.Count() > 3) 
                        {
                            bool.TryParse(cols[3], out vis);
                        }
                        entries.Add(new ReplacementEntry
                            {
                                DateAdded = DateTime.Parse(cols[0]),
                                Original = L3dFilePath.CreateRelativeToL3dDir(cols[1]),
                                Replacement = L3dFilePath.CreateRelativeToL3dDir(cols[2]),
                                IsVisible = vis
                            });
                    }
                }
            }
            return entries;
        }

        public static void WriteToCsv(IEnumerable<ReplacementEntry> entries, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach(var en in entries)
                {
                    if (en.Original != null)
                    {
                        sw.WriteLine("{0} {1};{2};{3};{4}", en.DateAdded.ToShortDateString(), en.DateAdded.ToShortTimeString(),
                            en.Original.PathRelativeToL3dDir, en.Replacement != null ? en.Replacement.PathRelativeToL3dDir : string.Empty, en.IsVisible);
                    }
                }
            }
        }

    }
}
