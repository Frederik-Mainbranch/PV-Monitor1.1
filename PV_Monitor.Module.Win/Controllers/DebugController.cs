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

namespace PV_Monitor.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DebugController : ViewController
    {

        SimpleAction SA_setzteLetztesImportDatumZurueck;
        public DebugController()
        {
            InitializeComponent();
            if (App_helper.IstEntwicklungsmodus == false)
            {
                this.Active["keinEntwicklungsmodus"] = false;
            }

            SA_setzteLetztesImportDatumZurueck = new SimpleAction(this, nameof(SA_setzteLetztesImportDatumZurueck), PredefinedCategory.Edit);
            SA_setzteLetztesImportDatumZurueck.Caption = "Importdatum zurücksetzen";
            SA_setzteLetztesImportDatumZurueck.TargetObjectType = typeof(PV_Modul);
            SA_setzteLetztesImportDatumZurueck.Execute += SA_setzteLetztesImportDatumZurueck_Execute;
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
            for (int i = messwertliste.Count - 1; i >= 0 ; i--)
            {
                messwertliste[i].Delete();
            }

            ObjectSpace.CommitChanges();
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
