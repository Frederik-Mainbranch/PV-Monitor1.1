using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Helper
{
    public static class Agnostic_Caller_Helper
    {
        public delegate void NotfallMitteilung_handler(object sender, NotfallMitteilungEventArgs e);
        public static event NotfallMitteilung_handler Event_Mitteilung;

        public static void ZeigeMessageBox(string text)
        {
            Event_Mitteilung.Invoke(typeof(Agnostic_Caller_Helper), new NotfallMitteilungEventArgs(text));
        }
    }

    public class NotfallMitteilungEventArgs : EventArgs
    {
        public NotfallMitteilungEventArgs(string text)
        {
            mitteilung = text;
        }
        public string mitteilung;
    }
}
