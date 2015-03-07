using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L3dStrJahreszeiten
{
    public class ReplacementEntry
    {
        public L3dFilePath Original { get; set; }
        public L3dFilePath Replacement { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsVisible { get; set; }
    }

    class ReplacementEntryComparer : IEqualityComparer<ReplacementEntry>
    {

        public bool Equals(ReplacementEntry x, ReplacementEntry y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return L3dFilePath.Equals(x.Original, y.Original);
        }

        public int GetHashCode(ReplacementEntry obj)
        {
            if (obj == null || obj.Original == null)
            {
                return 0;
            }
            return obj.Original.GetHashCode();
        }
    }
}
