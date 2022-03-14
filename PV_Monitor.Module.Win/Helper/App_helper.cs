using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB;
using PV_Monitor.Module.Helper;
using System.Windows.Forms;
using System.Configuration;

namespace PV_Monitor.Module.Win.Helper
{
    public static class App_helper
    {
        public static XafApplication App { get; set; }
        public static bool Status_App_istInitialisiert { get; set; }
        public static string MySQL_Connectionstring { get; set; }
        public static bool IstRootApplication { get; set; }
        public static bool IstEntwicklungsModus { get; set; } = true;
        public static string Einstellungspfad { get; set; }


        public static void Schreibe_sqlLiteConnectionstring()
        {
            #region alt

            //if (string.IsNullOrEmpty(connectionstring))
            //{
            //    connectionstring = SQLiteConnectionProvider.GetConnectionString(Path.Combine(AppContext.BaseDirectory, "DB_PV-Monitor.mdf"));
            //}
            //else
            //{
            //    return;
            //}

            //schreibe Connectionstring in Config
            //try
            //{
            //    ////Configuration configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //    //var test3 = ConfigurationManager.AppSettings;
            //    ////KeyValueConfigurationCollection confCollection = configManager.AppSettings.Settings;

            //    ////confCollection["ConnectionString"].Value = connectionstring;


            //    ////configManager.Save(ConfigurationSaveMode.Modified);
            //    ////ConfigurationManager.RefreshSection(configManager.AppSettings.SectionInformation.Name);

            //    //string test = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            //    string datei = "";
            //    using (StreamReader reader = new StreamReader(Einstellungspfad))
            //    {
            //        if (File.Exists(Einstellungspfad) == false)
            //        {
            //            MessageBox.Show("Beim auslesen der Einstellungsdatei ist ein Fehler aufgetreten. Sie wurde nicht gefunden.");
            //        }

            //        datei = reader.ReadToEnd();
            //    }

            //    using (StreamWriter writer = new StreamWriter(Einstellungspfad))
            //    {
            //        string dummy_datei = datei;
            //        datei = datei.Replace("<add name=\"ConnectionString\" connectionString=\"\" />", $"<add name=\"ConnectionString\" connectionString=\"{connectionstring}\" />)");
            //        if (dummy_datei == datei)
            //        {
            //            MessageBox.Show("Interner Fehler beim schreiben des Connection Strings: Die alte Datei entspricht der neuen Datei.");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show("Fehler beim schreiben des ermittelten Connectionstrings. Stacktrace: \r\n" + e);
            //}
            #endregion alt
            if (IstRootApplication)
            {
                string connectionstring = Multi_helper.Besorge_ZeileAusConfig("\"ConnectionString\"");
                if (string.IsNullOrEmpty(connectionstring)) //Connectionstring muss ermittelt werden und geschrieben werden
                {
                        connectionstring = SQLiteConnectionProvider.GetConnectionString(Path.Combine(AppContext.BaseDirectory, "DB_PV-Monitor.mdf"));
                }
                else
                {
                    return;
                }

                try
                {
                    string datei = "";
                    using (StreamReader reader = new StreamReader(Einstellungspfad))
                    {
                        if (File.Exists(Einstellungspfad) == false)
                        {
                            MessageBox.Show("Beim auslesen der Einstellungsdatei ist ein Fehler aufgetreten. Sie wurde nicht gefunden.");
                        }

                        datei = reader.ReadToEnd();
                        reader.Close();
                    }

                    using (StreamWriter writer = new StreamWriter(Einstellungspfad))
                    {
                        string dummy_datei = datei;
                        datei = datei.Replace("<add name=\"ConnectionString\" connectionString=\"\" />", $"<add name=\"ConnectionString\" connectionString=\"{connectionstring}\" />)");
                        writer.Write(datei);
                        if (dummy_datei == datei)
                        {
                            MessageBox.Show("Interner Fehler beim schreiben des Connection Strings: Die alte Datei entspricht der neuen Datei.");
                        }

                        writer.Close();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Fehler beim schreiben des ermittelten Connectionstrings. Stacktrace: \r\n" + e);
                }

            }
        }

        public static void Initialisiere_Anwendung()
        {
            //0. Das Event des Callen der Agnostic Messagebox abonieren
            //1. Setzten des Pfades zum Einstellungspfad
            //2. Überprüfen, ob die Application eine "Root Application" ist oder sie vom Modelaeditor oder anderen build prozessen gestartet wurde, weil dies zu problemen bei der pfad Ermittlung führen kann
            //3. Überprüfen, ob in der Einstellungsdatei der Sqllite Connection String hinterlegt ist, wenn nicht, wird dieser ermittelt und geschrieben
            Agnostic_Caller_Helper.Event_Mitteilung += Agnostic_Caller_Helper_messageEvent;
            Einstellungspfad = Path.Combine(AppContext.BaseDirectory, "PV_Monitor.Win.dll.config");

            if (File.Exists(Einstellungspfad) == true)
            {
                IstRootApplication = true;
            }

            Schreibe_sqlLiteConnectionstring();
        }

        private static void Agnostic_Caller_Helper_messageEvent(object sender, NotfallMitteilungEventArgs e)
        {
            MessageBox.Show(e.mitteilung);
        }
    }
}
