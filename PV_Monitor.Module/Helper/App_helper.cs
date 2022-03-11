using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Helper
{
    public static class App_helper
    {
        public static XafApplication App { get; set; }
        public static string MySQL_Connectionstring { get; set; }
        public static bool IstRootApplication { get; set; }
        public static bool IstEntwicklungsModus { get; set; } = true;
    }
}
