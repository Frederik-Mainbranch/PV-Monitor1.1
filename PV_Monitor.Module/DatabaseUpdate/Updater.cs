using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using PV_Monitor.Module.BusinessObjects;
using PV_Monitor.Module.Helper;

namespace PV_Monitor.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            if (App_helper.IstEntwicklungsmodus)
            {
                PV_Modul pv_l1 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID == 94);
                if (pv_l1 == null)
                {
                    pv_l1 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l1.Beschreibung = "L1";
                    pv_l1.DatenbankID = 94;
                }
                PV_Modul pv_l2 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID == 98);
                if (pv_l2 == null)
                {
                    pv_l2 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l2.Beschreibung = "L2";
                    pv_l2.DatenbankID = 98;
                }
                PV_Modul pv_l3 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID == 93);
                if (pv_l3 == null)
                {
                    pv_l3 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l3.Beschreibung = "L3";
                    pv_l3.DatenbankID = 93;
                }

                if (ObjectSpace.IsModified)
                {
                    ObjectSpace.CommitChanges();
                }
            }

            //string name = "MyName";
            //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
            //if(theObject == null) {
            //    theObject = ObjectSpace.CreateObject<DomainObject1>();
            //    theObject.Name = name;
            //}

            //ObjectSpace.CommitChanges(); //Uncomment this line to persist created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
    }
}
