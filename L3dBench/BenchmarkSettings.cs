using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace L3dBench
{
    public class BenchmarkSettings
    {
        public BenchmarkSettings()
        {
            TargetSpeed = 160;
        }

        public string Fpl { get; set; }
        public string Lok { get; set; }
        public string StartBhf { get; set; }
        public int StartIndex { get; set; }
        public int WetterIndex { get; set; }

        public string L3dDir { get; set; }
        public string L3dExeToTest { get; set; }

        public string FrapsBenchmarkDir { get; set; }

        public double MeterZuFahren { get; set; }

        public string Name { get; set; }

        public int TargetSpeed { get; set; }
    }

    public class BenchmarkList
    {
        public List<BenchmarkSettings> Benchmarks { get; set; }
    }

    static class BenchmarkSettingsSerializer
    {
        public static BenchmarkList ReadFromFile(string file)
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(BenchmarkList));
                using (StreamReader sr = new StreamReader(file))
                {
                    return s.Deserialize(sr) as BenchmarkList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        public static void WriteToFile(BenchmarkList benches, string file)
        {
            try
            {
                XmlSerializer s = new XmlSerializer(typeof(BenchmarkList));
                using (StreamWriter sw = new StreamWriter(file))
                {
                    s.Serialize(sw, benches);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }
    }
}
