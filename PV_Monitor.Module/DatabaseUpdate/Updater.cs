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

            #region -- PV-Modul ---------------------------------------------------------------
            if (App_helper.IstEntwicklungsmodus)
            {
                PV_Modul pv_l1 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID_Leistung == 94);
                if (pv_l1 == null)
                {
                    pv_l1 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l1.Beschreibung = "L1";
                    pv_l1.DatenbankID_Leistung = 94;
                    pv_l1.DatenbankID_kWh = 96;
                }
                PV_Modul pv_l2 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID_Leistung == 98);
                if (pv_l2 == null)
                {
                    pv_l2 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l2.Beschreibung = "L2";
                    pv_l2.DatenbankID_Leistung = 98;
                    pv_l2.DatenbankID_kWh = 97;
                }
                PV_Modul pv_l3 = ObjectSpace.FirstOrDefault<PV_Modul>(u => u.DatenbankID_Leistung == 93);
                if (pv_l3 == null)
                {
                    pv_l3 = ObjectSpace.CreateObject<PV_Modul>();
                    pv_l3.Beschreibung = "L3";
                    pv_l3.DatenbankID_Leistung = 93;
                    pv_l3.DatenbankID_kWh = 95;
                }
            }
            #endregion PV-Modul

            #region -- Autoimport Einstellung -----------------------------------------------------
            AutoimportEinstellung autoimport_jaehrlich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Jaehrlich);
            if (autoimport_jaehrlich == null)
            {
                autoimport_jaehrlich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_jaehrlich.Beschreibung = "jährlich";
                autoimport_jaehrlich.ImportiereBeiProgrammstart = false;
                autoimport_jaehrlich.ImportiereManuell = false;
                autoimport_jaehrlich.Enum_AutoimportX = Enum_Autoimport.Jaehrlich;
            }

            AutoimportEinstellung autoimport_monatlich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Monatlich);
            if (autoimport_monatlich == null)
            {
                autoimport_monatlich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_monatlich.Beschreibung = "monatlich";
                autoimport_monatlich.ImportiereBeiProgrammstart = false;
                autoimport_monatlich.ImportiereManuell = false;
                autoimport_monatlich.Enum_AutoimportX = Enum_Autoimport.Monatlich;
            }

            AutoimportEinstellung autoimport_woechentlich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Woechentlich);
            if (autoimport_woechentlich == null)
            {
                autoimport_woechentlich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_woechentlich.Beschreibung = "wöchentlich";
                autoimport_woechentlich.ImportiereBeiProgrammstart = false;
                autoimport_woechentlich.ImportiereManuell = false;
                autoimport_woechentlich.Enum_AutoimportX = Enum_Autoimport.Woechentlich;
            }

            AutoimportEinstellung autoimport_taeglich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Taeglich);
            if (autoimport_taeglich == null)
            {
                autoimport_taeglich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_taeglich.Beschreibung = "täglich";
                autoimport_taeglich.ImportiereBeiProgrammstart = false;
                autoimport_taeglich.ImportiereManuell = false;
                autoimport_taeglich.Enum_AutoimportX = Enum_Autoimport.Taeglich;
            }

            AutoimportEinstellung autoimport_stuendlich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Stuendlich);
            if (autoimport_stuendlich == null)
            {
                autoimport_stuendlich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_stuendlich.Beschreibung = "stündlich";
                autoimport_stuendlich.ImportiereBeiProgrammstart = false;
                autoimport_stuendlich.ImportiereManuell = false;
                autoimport_stuendlich.Enum_AutoimportX = Enum_Autoimport.Stuendlich;
            }

            AutoimportEinstellung autoimport_viertelstuendlich = ObjectSpace.FirstOrDefault<AutoimportEinstellung>(u => u.Enum_AutoimportX == Enum_Autoimport.Viertelstuendlich);
            if (autoimport_viertelstuendlich == null)
            {
                autoimport_viertelstuendlich = ObjectSpace.CreateObject<AutoimportEinstellung>();
                autoimport_viertelstuendlich.Beschreibung = "viertelstündlich";
                autoimport_viertelstuendlich.ImportiereBeiProgrammstart = false;
                autoimport_viertelstuendlich.ImportiereManuell = false;
                autoimport_viertelstuendlich.Enum_AutoimportX = Enum_Autoimport.Viertelstuendlich;
            }

            #endregion --Autoimport Einstellung

            #region Einstellung

            Einstellung zeigeAutoimportbenachrichtigung = ObjectSpace.FirstOrDefault<Einstellung>(x => x.AuswahlEinstellungX == Einstellung.Enum_AuswahlEinstellung_bool.ZeigeAutoimportBenachrichtigung);
            if (zeigeAutoimportbenachrichtigung == null)
            {
                zeigeAutoimportbenachrichtigung = ObjectSpace.CreateObject<Einstellung>();
                zeigeAutoimportbenachrichtigung.Name = "Zeige Autoimportbenachrichtigung";
                zeigeAutoimportbenachrichtigung.Beschreibung = "Wenn aktiviert: Zeigt beim Programmstart die Benachrichtigung, wie viele Messwerte durch den automatischen Import beim Programmstart importiert wurden";
                zeigeAutoimportbenachrichtigung.AuswahlEinstellungX = Einstellung.Enum_AuswahlEinstellung_bool.ZeigeAutoimportBenachrichtigung;
                zeigeAutoimportbenachrichtigung.Wert_bool = false;
            }

            #endregion Einstellung

            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
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
