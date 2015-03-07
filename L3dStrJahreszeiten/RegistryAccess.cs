using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L3dStrJahreszeiten
{
    public static class RegistryAccess
    {
        public static string GetLoksimDataDir()
        {
            string ret = string.Empty;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Loksim-Group\Install"))
            {
                if (key != null)
                {
                    ret = key.GetValue("InstallDataDirPath", string.Empty).ToString();
                }
            }
            return ret;
        }
    }
}
