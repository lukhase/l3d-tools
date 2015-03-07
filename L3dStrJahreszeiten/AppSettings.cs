using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace L3dStrJahreszeiten
{
    public class AppSettings
    {
        public List<Ersetzungsprojekt> RecentProjekte { get; set; }

        public void SaveToDefaultXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            string appPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            string settingsPath = Path.Combine(Path.GetDirectoryName(appPath), "l3dstrjahreszeitensettings.xml");
            using (StreamWriter sw = new StreamWriter(settingsPath))
            {
                serializer.Serialize(sw, this);
            }
        }

        [XmlIgnore()]
        private static AppSettings default_;
        public static AppSettings DefaultSettings
        {
            get
            {
                try
                {
                    if (default_ == null)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
                        string appPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                        string settingsPath = Path.Combine(Path.GetDirectoryName(appPath), "l3dstrjahreszeitensettings.xml");
                        using (StreamReader sr = new StreamReader(settingsPath))
                        {
                            default_ = (AppSettings)serializer.Deserialize(sr);
                        }
                    }
                } catch (Exception)
                {
                    default_ = new AppSettings();
                    default_.RecentProjekte = new List<Ersetzungsprojekt>();
                }
                return default_;

            }
        }
    }
}
