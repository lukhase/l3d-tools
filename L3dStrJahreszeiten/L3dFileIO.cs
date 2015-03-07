using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace L3dStrJahreszeiten
{
    class L3dFileIO
    {
        public static IEnumerable<L3dFilePath> ReadFromStrFile(string filePath)
        {
            /*
            XDocument doc = XDocument.Load(filePath);
            var root = doc.Root;
            var defRailFiles = root.Elements("DefaulRail").Elements("Props").Attributes("File").
                Select(a => a.Value);
            var objFiles = root.Elements("GLEIS").Elements("Objecte").Elements("Object").
                Elements("Eintrag").Elements("Props").Attributes("Datei").Select(a => a.Value);
            var railFiles = root.Elements("GLEIS").Elements("EIGENSCHAFTEN").Elements("VOR").Elements("RAIL")
                .Elements("Value").Elements("Props").Attributes("RAILVALUE").Select(a => a.Value);
            var sceneFiles = root.Elements("GLEIS").Elements("Landschaft").Elements("Landschaftsobjekt").
                Elements("Props").Attributes("TextureFile").Select(a => a.Value);

            return defRailFiles.Union(objFiles).Union(railFiles).Union(sceneFiles).Select(s
                => L3dFilePath.CreateRelativeToL3dDir(s));
             */

            var strPath = new L3dFilePath(filePath);

            List<string> fileEndings = new List<string>();
            fileEndings.Add("bmp");
            fileEndings.Add("tga");
            fileEndings.Add("png");
            fileEndings.Add("l3dobj");
            fileEndings.Add("l3dgrp");
            fileEndings.Add("l3drail");

            HashSet<L3dFilePath> ret = new HashSet<L3dFilePath>();

            string content = File.ReadAllText(filePath);
            Regex rgx = new Regex("=\\s*\"([^\"]+)\"");
            foreach (Match m in rgx.Matches(content))
            {
                string p = m.Groups[1].Value.ToLower();
                foreach (var e in fileEndings)
                {
                    if (p.EndsWith(e))
                    {
                        ret.Add(L3dFilePath.CreateRelativeToFile(m.Groups[1].Value, strPath));
                    }
                }
            }
            return ret.ToList();
        }

        private static void ReplaceValues(IEnumerable<XAttribute> elems, Dictionary<string, string> replacements)
        {
            string rep;
            foreach (var xe in elems)
            {
                if (replacements.TryGetValue(xe.Value, out rep))
                {
                    xe.Value = rep;
                }
            }
        }

        public static void ReplaceFilesInStr(string inPath, string outPath, Dictionary<L3dFilePath, L3dFilePath> replacements)
        {
            string content = File.ReadAllText(inPath);
            StringBuilder sb = new StringBuilder(content);
            foreach (var en in replacements)
            {
                sb.Replace(en.Key.PathRelativeToL3dDir, en.Value.PathRelativeToL3dDir);
            }
            File.WriteAllText(outPath, sb.ToString(), Encoding.UTF8);

            /*
            XDocument doc = XDocument.Load(inPath);
            var root = doc.Root;
            ReplaceValues(root.Elements("DefaulRail").Elements("Props").Attributes("File"), replacements);
            ReplaceValues(root.Elements("GLEIS").Elements("Objecte").Elements("Object").
                Elements("Eintrag").Elements("Props").Attributes("Datei"), replacements);
            ReplaceValues(root.Elements("GLEIS").Elements("EIGENSCHAFTEN").Elements("VOR").Elements("RAIL")
               .Elements("Value").Elements("Props").Attributes("RAILVALUE"), replacements);
            ReplaceValues(root.Elements("GLEIS").Elements("EIGENSCHAFTEN").Elements("VOR").Elements("RAIL")
               .Elements("Value").Elements("Props").Attributes("RAILVALUE"), replacements);

            using (StringWriter tw = new StringWriter())
            {
                doc.Save(tw, SaveOptions.None);
                string outContent = tw.ToString().Replace("></", ">\r\n</");
                File.WriteAllText(outPath, outContent, Encoding.UTF8);
            }
             */
        }
    }
}
