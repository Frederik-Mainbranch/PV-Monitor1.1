using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB;

namespace PV_Monitor.Module.Helper
{
    public static class App_helper
    {
        public static XafApplication App { get; set; }
        public static bool Status_App_istInitialisiert { get; set; }
        public static string MySQL_Connectionstring { get; set; }
        public static bool IstRootApplication { get; set; }
        public static bool IstEntwicklungsModus { get; set; } = true;
        public static string Einstellungspfad { get; set; }
        public delegate void NotfallMitteilung_handler(object sender, NotfallMitteilungEventArgs e);
        public static event NotfallMitteilung_handler Notfallmitteilung; 

        public static void Zeige_Notfallmitteilung(string text)
        {
            Notfallmitteilung(typeof(App_helper), new NotfallMitteilungEventArgs(text));
        }

        public static void Schreibe_sqlLiteConnectionstring()
        {
            if (IstRootApplication)
            {
                string connectionstring = Multi_helper.Besorge_ZeileAusConfig("\"ConnectionString\"");
                if (string.IsNullOrEmpty(connectionstring))
                {
                    connectionstring = SQLiteConnectionProvider.GetConnectionString(Path.Combine(AppContext.BaseDirectory, "DB_PV-Monitor.mdf"));
                }

                //schreibe Connectionstring in Config
                try
                {
                    using (StreamWriter writer = new StreamWriter(Einstellungspfad))
                    {
                        string datei = "";
                        using (StreamReader reader = new StreamReader(Einstellungspfad))
                        {
                            datei = reader.ReadToEnd();
                        }

                        datei.Replace("<add name=\"ConnectionString\" connectionString=\"\" />", $"<add name=\"ConnectionString\" connectionString=\"{connectionstring}\" />)");
                    }
                }
                catch (Exception e)
                {
                    Multi_helper.Zeige_Messagebox("Fehler beim schreiben des ermittelten Connectionstrings. Stacktrace: \r\n" + e);
                }
            }
        }

        public static void Initialisiere_Anwendung()
        {
            //1. Setzten des Pfades zum Einstellungspfad
            //2. Überprüfen, ob die Application eine "Root Application" ist oder sie vom Modelaeditor oder anderen build prozessen gestartet wurde, weil dies zu problemen bei der pfad Ermittlung führen kann
            //3. Überprüfen, ob in der Einstellungsdatei der Sqllite Connection String hinterlegt ist, wenn nicht, wird dieser ermittelt und geschrieben
            Einstellungspfad = Path.Combine(AppContext.BaseDirectory, "PV_Monitor.Win.dll.config");

            if (File.Exists(Einstellungspfad) == true)
            {
                IstRootApplication = true;
            }

            Schreibe_sqlLiteConnectionstring();
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
