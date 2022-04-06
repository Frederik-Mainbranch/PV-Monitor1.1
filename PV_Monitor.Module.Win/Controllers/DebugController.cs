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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PV_Monitor.Module.Helper;
using PV_Monitor.Module.BusinessObjects;
using DevExpress.XtraSplashScreen;
using PV_Monitor.Module.Win.Helper;
//using System.Threading;

namespace PV_Monitor.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DebugController : ViewController
    {
        //SimpleAction SA_ZeigeWaitingform;
        SimpleAction SA_setzteLetztesImportDatumZurueck;
        SimpleAction SA_testeAsync;
        SimpleAction SA_ErstelleTestMesswerte;

        public DebugController()
        {
            InitializeComponent();
            if (App_helper.IstEntwicklungsmodus == false)
            {
                this.Active["IstEntwicklungsmodus"] = false;
            }

            SA_setzteLetztesImportDatumZurueck = new SimpleAction(this, nameof(SA_setzteLetztesImportDatumZurueck), PredefinedCategory.Edit);
            SA_setzteLetztesImportDatumZurueck.Caption = "Messwerte zurücksetzen";
            SA_setzteLetztesImportDatumZurueck.TargetObjectType = typeof(PV_Modul);
            SA_setzteLetztesImportDatumZurueck.ConfirmationMessage = "Wollen Sie wirklich alle Messwerte löschen? Es werden nur die Daten im PV Monitor gelöscht, die Daten des ioBrokers bleiben davon unberührt.";
            SA_setzteLetztesImportDatumZurueck.Execute += SA_setzteLetztesImportDatumZurueck_Execute;


            SA_testeAsync = new SimpleAction(this, nameof(SA_testeAsync), PredefinedCategory.Edit);
            SA_testeAsync.Caption = "Teste Splashscreen";
            SA_testeAsync.TargetObjectType = typeof(PV_Modul);
            SA_testeAsync.Execute += SA_testeAsync_Execute;


            SA_ErstelleTestMesswerte = new SimpleAction(this, nameof(SA_ErstelleTestMesswerte), PredefinedCategory.Edit);
            SA_ErstelleTestMesswerte.Caption = "Erstelle Testmesswerte";
            SA_ErstelleTestMesswerte.Execute += SA_ErstelleTestMesswerte_Execute;


            //SA_ZeigeWaitingform = new SimpleAction(this, nameof(SA_ZeigeWaitingform), PredefinedCategory.Edit);
            //SA_ZeigeWaitingform.Caption = "Zeige Waitingform";
            //SA_ZeigeWaitingform.Execute += SA_ZeigeWaitingform_Execute;
        }



        private void SA_ErstelleTestMesswerte_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Messwert messwert1 = ObjectSpace.CreateObject<Messwert>();
            messwert1.DatumVon = new DateTime(2021, 5, 11, 12, 15, 0);
            messwert1.DatumBis = new DateTime(2021, 5, 11, 12, 30, 0);
            messwert1.PV_Modul = ObjectSpace.FindObject<PV_Modul>(new BinaryOperator(nameof(PV_Modul.Beschreibung), "L1"));

            Messwert messwert2 = ObjectSpace.CreateObject<Messwert>();
            messwert2.DatumVon = new DateTime(2021, 5, 12, 0, 0, 0);
            messwert2.DatumBis = new DateTime(2021, 5, 12, 0, 15, 0);
            messwert2.PV_Modul = ObjectSpace.FindObject<PV_Modul>(new BinaryOperator(nameof(PV_Modul.Beschreibung), "L2"));

            Messwert messwert3 = ObjectSpace.CreateObject<Messwert>();
            messwert3.DatumVon = new DateTime(2021, 5, 13, 23, 45, 0);
            messwert3.DatumBis = new DateTime(2021, 5, 14, 0, 0, 0);
            messwert3.PV_Modul = ObjectSpace.FindObject<PV_Modul>(new BinaryOperator(nameof(PV_Modul.Beschreibung), "L3"));

            ObjectSpace.CommitChanges();
            View.Refresh(true);
        }

        int counter = 0;
        System.Timers.Timer _timer;

        private void SA_testeAsync_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //Autoimport_helper.StarteAutoimportV2_multiT();
            _timer = new System.Timers.Timer();
            _timer.Elapsed += Timer_Elapsed;
            _timer.Interval = 1500;
            _timer.Enabled = true;
            SplashScreen_helper.Zeige_Splashscreen("Importiere Messwerte", "");
            SplashScreen_helper.Update_Splashscreen_description("Importiere Jahreswerte...");
            counter = 0;
        }

        //private void SA_ZeigeWaitingform_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    SplashScreenManager.ShowDefaultWaitForm("Starte Import...");
        //    _timer = new System.Timers.Timer();
        //    _timer.Elapsed += Timer_Elapsed;
        //    _timer.Interval = 1000;
        //    _timer.Enabled = true;

        //    //Action callback = () =>
        //    //{
        //    //    if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
        //    //        SplashScreenManager.CloseDefaultWaitForm();
        //    //};
        //}

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (counter == 1)
            {
                SplashScreen_helper.Update_Splashscreen_description("Importiere Monatswerte...");
            }
            else if (counter == 3)
            {
                SplashScreen_helper.Update_Splashscreen_description("Importiere Wochenwerte...");
            }
            else if (counter == 6)
            {
                SplashScreen_helper.Update_Splashscreen_description("Importiere Tageswerte...");
            }
            else if (counter == 10)
            {
                SplashScreen_helper.Update_Splashscreen_description("Importiere Stundenwerte...");
            }
            else if (counter == 15)
            {
                _timer.Stop();
                _timer.Elapsed -= Timer_Elapsed;
                _timer.Dispose();
                SplashScreen_helper.Update_Splashscreen_description("Importiere Viertelstundenwerte...");
                SplashScreen_helper.Close_Splashscreen();
                return;
            }

            counter++;



        }

        private void SA_setzteLetztesImportDatumZurueck_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IList<PV_Modul> modulliste = ObjectSpace.GetObjects<PV_Modul>();
            foreach (PV_Modul modul in modulliste)
            {
                modul.Datum_letzterImport_jaehrlich = DateTime.MinValue;
                modul.Datum_letzterImport_monatlich = DateTime.MinValue;
                modul.Datum_letzterImport_woechentlich = DateTime.MinValue;
                modul.Datum_letzterImport_taeglich = DateTime.MinValue;
                modul.Datum_letzterImport_stuendlich = DateTime.MinValue;
                modul.Datum_letzterImport_viertelstuendlich = DateTime.MinValue;

                modul.DatumInbetriebnahme = DateTime.MinValue;
            }


            IList<Messwert> messwertliste = ObjectSpace.GetObjects<Messwert>();
            ObjectSpace.Delete(messwertliste);

            //for (int i = messwertliste.Count - 1; i >= 0 ; i--)
            //{
            //    messwertliste[i].Delete();
            //}

            ObjectSpace.CommitChanges();
            View.Refresh(true);
        }

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
    }
}
