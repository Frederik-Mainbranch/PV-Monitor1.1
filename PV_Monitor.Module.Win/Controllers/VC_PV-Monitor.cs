using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using MySql.Data.MySqlClient;
using PV_Monitor.Module.BusinessObjects._np;
using PV_Monitor.Module.Win.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PV_Monitor.Module.BusinessObjects;
using System.Windows.Forms;
using DevExpress.Xpo;
using System.Diagnostics;
using DevExpress.XtraSplashScreen;

namespace PV_Monitor.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class VC_PV_Monitor : ViewController
    {
        //PopupWindowShowAction PA_importiereMessWerte;
        SimpleAction SA_importiereMesswerte;
        //readonly string defaultquery = "where id = xyz"
        public VC_PV_Monitor()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(PV_Modul);

            SA_importiereMesswerte = new SimpleAction(this, nameof(SA_importiereMesswerte), PredefinedCategory.Edit);
            SA_importiereMesswerte.Caption = "Importiere Messwerte";
            SA_importiereMesswerte.Execute += SA_importiereMesswerte_Execute;
        }




        private void SA_importiereMesswerte_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Action callback = () =>
            //{
            //    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
            //        SplashScreenManager.CloseDefaultWaitForm();
            //};
            WinApp_helper._Controller = this;
            Autoimport_helper.StarteAutoimportV1_multiT(false);
        }


        

        #region alt
        //private void PA_importiereMessWerte_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        //{
        //    MesswertImport_np messwertImport = new MesswertImport_np();
        //    messwertImport.Von = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1); // von Anfang letzten Monats 
        //    messwertImport.Bis = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1); // bis Ende letzten Monats
        //    messwertImport.Intervall = new TimeSpan(1, 0, 0, 0);
        //    e.View = Application.CreateDetailView(ObjectSpace.CreateNestedObjectSpace(), messwertImport);
        //}

        //private void PA_importiereMessWerte_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        //{
        //    MesswertImport_np messwertImport = e.PopupWindowView.CurrentObject as MesswertImport_np;
        //    List<PV_Modul> pv_module = new List<PV_Modul>();
        //    foreach (var item in e.SelectedObjects)
        //    {
        //        if (item is PV_Modul modul)
        //        {
        //            pv_module.Add(modul);
        //        }
        //    }
        //    ImportiereMesswerte(pv_module, messwertImport.Von, messwertImport.Bis, messwertImport.Intervall);
        //}

        //private void ImportiereMesswerte(List<PV_Modul> pv_module, DateTime startdatum, DateTime enddatum, TimeSpan intervall)
        //{
        //if (enddatum < startdatum)
        //{
        //    MessageBox.Show("Das Enddatum ist größer als das Startdatum! Bitte geben Sie ein anderes Startdatum an.");
        //    return;
        //}

        //MySqlConnection mySqlConnection = Sql_helper.Connection;
        //DateTime dateTime_runner = startdatum;
        //foreach (PV_Modul modul in pv_module)
        //{
        //    while ((dateTime_runner + intervall) <= enddatum) //volles Intervall
        //    {
        //        if(importiere_Intervall(modul, ref dateTime_runner, enddatum, intervall) == false )
        //        {
        //            return;
        //        }
        //    }

        //    if (importiere_Intervall(modul, ref dateTime_runner, enddatum, enddatum - dateTime_runner) == false) //restliches Intervall
        //    {
        //        return;
        //    }
        //}

        //if (ObjectSpace.IsModified)
        //{
        //    ObjectSpace.CommitChanges();
        //}

        //bool importiere_Intervall(PV_Modul modul, ref DateTime dateTime_runner, DateTime enddatum, TimeSpan intervall)
        //{
        //    string query = $"select sum(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts >= {Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_runner)} and ts <= {Sql_helper.KonvertiereZu_mysqlDatetime(enddatum)} and q = 0";
        //    int counter = 0;
        //    double summe_watt = 0;
        //    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
        //    try
        //    {
        //        mySqlConnection.Open();
        //        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
        //        while (mySqlDataReader.Read())
        //        {
        //            counter++;
        //            string line = mySqlDataReader.GetValue(0).ToString();
        //            if (string.IsNullOrEmpty(line))
        //            {
        //                break;
        //            }
        //            double newvalue = 0;
        //            if (double.TryParse(line, out newvalue) == true)
        //            {
        //                summe_watt += newvalue;
        //            }
        //            else
        //            {
        //                MessageBox.Show($"Fehler beim Importieren des Wertes '{line}'");
        //                return false;
        //            }
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        MessageBox.Show("Fehler beim Auslesen der Messwerte aus der Mysql Datenbank. Stacktrace: \r\n" + e);
        //    }
        //    finally
        //    {
        //        mySqlConnection.Close();
        //    }

        //    //Ende des Importes des aktuellen Intervalls
        //    Messwert messwert = ObjectSpace.CreateObject<Messwert>();
        //    messwert.Uhrzeit = dateTime_runner;
        //    messwert.UhrzeitBis = dateTime_runner + intervall;
        //    messwert.Intervall = intervall;  
        //    messwert.PV_Modul = modul;
        //    messwert.Watt = Math.Round((summe_watt / counter), 1);

        //    dateTime_runner += intervall;
        //    return true;
        //}
        //}
        #endregion alt

        #region Devexpress
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        #endregion Devexpress
    }
}
