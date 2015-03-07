using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L3dStrJahreszeiten
{
    public static class ReplacementUtilities
    {
        public static string GetAutoReplacement(IEnumerable<L3dStrJahreszeiten.ReplacementSet.Replacement> ersetzungen, string str)
        {
            foreach (var r in ersetzungen)
            {
                if (str.Contains(r.OriginalName))
                {
                    str = str.Replace(r.OriginalName, r.NewName);
                }
            }
            return str;
        }
    }
}
