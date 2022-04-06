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
using PV_Monitor.Module.BusinessObjects;
using PV_Monitor.Module.Win.Controllers;
using PV_Monitor.Module.Win.Waitforms;

namespace PV_Monitor.Module.Win.Helper
{
    public static class WinApp_helper
    {
        public static XafApplication App { get; set; }
        public static bool Status_App_istInitialisiert { get; set; }
        public static string MySQL_Connectionstring { get; set; }
        public static bool IstRootApplication { get; set; }
        //public static bool IstEntwicklungsModus { get; set; } = true;
        public static string Einstellungspfad { get; set; }
        public static string Path_appData { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PV Monitor");
        public static string path_logDatei { get; set; }
        public static string path_datenbank { get; set; }
        public static VC_PV_Monitor _Controller { get; set; }
        public static DemoWaitForm2 _WaitingForm { get; set; }


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
                    path_datenbank = Path.Combine(Path_appData, "DB_PV-Monitor.mdf");
                    connectionstring = SQLiteConnectionProvider.GetConnectionString(path_datenbank);
                    connectionstring = connectionstring.Replace("\"", ""); //Es kann sonst vorkommen, dass der Connectionstring den Path mit zu viel "" übergibt und dies dann nicht mehr eingelesen werden kann von xpo
                    //MessageBox.Show("abc " + connectionstring + " def");
                    WinApp_helper.Schreibe_log("Neuer SQL Lite Connection String gesetzt: " + connectionstring);
                }
                else
                {
                    WinApp_helper.Schreibe_log("Vorhandener SQL Lite Connection String gefunden");
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
                        WinApp_helper.Schreibe_log("Einstellungsdatei eingelesen");
                    }

                    using (StreamWriter writer = new StreamWriter(Einstellungspfad))
                    {
                        string dummy_datei = datei;
                        //string test = $"<add name=\"ConnectionString\" connectionString=\"{connectionstring}\" />)";
                        //string test2 = test.Replace("\\\\", "\\");
                        //MessageBox.Show(test2);
                        datei = datei.Replace("<add name=\"ConnectionString\" connectionString=\"\" />", $"<add name=\"ConnectionString\" connectionString=\"{connectionstring}\" />");
                        writer.Write(datei);
                        if (dummy_datei == datei)
                        {
                            MessageBox.Show("Interner Fehler beim schreiben des Connection Strings: Die alte Datei entspricht der neuen Datei.");
                        }

                        writer.Close();
                    }

                    WinApp_helper.Schreibe_log("Neuen SQLLite Connectionstring geschrieben");
                }
                catch (Exception e)
                {
                    WinApp_helper.Schreibe_log("Fehler beim schreiben des neuen SQLLite Connectionstring");
                    MessageBox.Show("Fehler beim schreiben des ermittelten Connectionstrings.\r\n\r\n----- wichtig -----\r\n\r\nBei dem ersten Start der Anwendung muss diese bitte als Administrator gestartet werden, damit der Pfad zur Anwendung in der Config Datei gespeichert werden kann.\r\n\r\n----- wichtig -----\r\n\r\nStacktrace: \r\n" + e);
                    App.Exit();
                }

            }
        }

        public static void Schreibe_log(string text)
        {
            using (StreamWriter writer = new StreamWriter(WinApp_helper.path_logDatei, true))
            {
                writer.WriteLine(text);
            }
        }

        public static void Initialisiere_Anwendung()
        {
            if (Directory.Exists(Path_appData) == false)
            {
                Directory.CreateDirectory(Path_appData);
            }

            path_logDatei = Path.Combine(Path_appData, @"Start-log.txt");

            using (StreamWriter writer = new StreamWriter(WinApp_helper.path_logDatei))
            {
                writer.Write("");
            }

            //0. Das Event des Callen der Agnostic Messagebox abonieren
            //1. Setzten des Pfades zum Einstellungspfad
            //2. Überprüfen, ob die Application eine "Root Application" ist oder sie vom Modelaeditor oder anderen build prozessen gestartet wurde, weil dies zu problemen bei der pfad Ermittlung führen kann
            //3. Überprüfen, ob in der Einstellungsdatei der Sqllite Connection String hinterlegt ist, wenn nicht, wird dieser ermittelt und geschrieben
            Agnostic_Caller_Helper.Event_Mitteilung += Agnostic_Caller_Helper_messageEvent;
            Einstellungspfad = Path.Combine(AppContext.BaseDirectory, "PV_Monitor.Win.dll.config");

            if (File.Exists(Einstellungspfad) == true)
            {
                IstRootApplication = true;
                Schreibe_log("Ist Root Application");
            }

            Schreibe_sqlLiteConnectionstring();
            Schreibe_log("WinHelper: Initialisiere App abgeschlossen");
        }

        private static void Agnostic_Caller_Helper_messageEvent(object sender, NotfallMitteilungEventArgs e)
        {
            MessageBox.Show(e.mitteilung);
        }

        public static void Erstelle_Fehlermeldung(string beschreibung, Enum_FehlerSchweregrad schweregrad, Enum_Fehlergruppe fehlergruppe)
        {
            IObjectSpace obj_space = App.CreateObjectSpace(typeof(Fehlerprotokoll));
            Fehlerprotokoll fehlerprotokoll = obj_space.CreateObject<Fehlerprotokoll>();
            fehlerprotokoll.Beschreibung = beschreibung;
            fehlerprotokoll.Schweregrad = schweregrad;
            fehlerprotokoll.Fehlergruppe = fehlergruppe;
            obj_space.CommitChanges();
            obj_space.Dispose();

            MessageBox.Show(beschreibung);
        }
    }
}
