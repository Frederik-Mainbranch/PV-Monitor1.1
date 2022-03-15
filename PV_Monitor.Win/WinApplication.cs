using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Xpo;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.IO;
using PV_Monitor.Module.Win.Helper;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using PV_Monitor.Module.Helper;
//using DevExpress.DataAccess.Native.Json;

namespace PV_Monitor.Win {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
    public partial class PV_MonitorWindowsFormsApplication : WinApplication {
        public PV_MonitorWindowsFormsApplication() {
			InitializeComponent();
            WinApp_helper.Status_App_istInitialisiert = true;
            try
            {
                SetMySqlConnectionstring();
            }
            catch (Exception e)
            {
                MessageBox.Show("Fehler beim auslesen des MySql Connectionstring. Stacktrace:\r\n" + e);
            }
            SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
        }

        private void SetMySqlConnectionstring()
        {
            if (WinApp_helper.IstRootApplication == false)
            {
                return;
            }

            string connectionstring = Multi_helper.Besorge_ZeileAusConfig("\"ConnectionString-mysql\"");
            if (string.IsNullOrEmpty(connectionstring) == false) //Erstellen und testen der Verbindung zum Mysql Server
            {
                WinApp_helper.MySQL_Connectionstring = connectionstring;
                MySqlConnection connection = new MySqlConnection(connectionstring);
                Sql_helper.Connection = connection;
                connection.Open();
                connection.Close();
            }
            else
            {
                XtraMessageBox.Show($"Der Mysql Connectionstring wurde nicht gefunden in der {Assembly.GetExecutingAssembly().GetName().Name + ".dll.config"}! Das Programm wird daher nun geschlossen.");
            }
        }

        protected override void OnLoggedOn(LogonEventArgs args)
        {
            base.OnLoggedOn(args);
            App_helper.IstEingeloggt = true;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true), false));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void PV_MonitorWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
                e.Languages.Add(userLanguageName);
            }
        }
        private void PV_MonitorWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
            e.Updater.Update();
            e.Handled = true;
#else
            if(System.Diagnostics.Debugger.IsAttached) {
                e.Updater.Update();
                e.Handled = true;
            }
            else {
				string message = "The application cannot connect to the specified database, " +
					"because the database doesn't exist, its version is older " +
					"than that of the application or its schema does not match " +
					"the ORM data model structure. To avoid this error, use one " +
					"of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

				if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
					message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
				}
				throw new InvalidOperationException(message);
            }
#endif
        }
    }
}
