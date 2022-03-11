using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PV_Monitor.Module.Helper
{
    public static class Multi_helper
    {
        public static string Besorge_Stammverzeichnis()
        {
            return Environment.CurrentDirectory;
        }

        public static string Besorge_ZeileAusConfig(string speicherort, string objectvalueToSearchFor)
        {
            string[] file = File.ReadAllLines(speicherort);
            foreach (string zeile in file)
            {
                if (zeile.Contains(objectvalueToSearchFor))
                {
                    int unwichtige_posi1 = zeile.IndexOf("\"");
                    int unwichtige_posi2 = zeile.IndexOf("\"", unwichtige_posi1 + 1);
                    int posi_start = zeile.IndexOf("\"", unwichtige_posi2 + 1);
                    int posi_ende = zeile.IndexOf("\"", posi_start + 1);
                    return zeile.Substring(posi_start + 1, posi_ende - posi_start - 1);
                }
            }

            return null;
        }
    }
}
