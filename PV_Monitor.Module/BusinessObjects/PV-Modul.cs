using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PV_Monitor.Module.Helper;

namespace PV_Monitor.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Beschreibung))]

    public class PV_Modul : BaseObject
    { 
        public PV_Modul(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading && !IsSaving)
            {
                if (propertyName == nameof(DatenbankID) && App_helper.IstEingeloggt)
                {
                    Agnostic_Caller_Helper.ZeigeMessageBox("Das Eintragen einer falschen Datenbank ID kann zu kritischen Fehlern führen. Bitte vergewissern Sie sich, dass die ID korrekt ist vor dem speichern!");
                }
            }
        }


        private string _Beschreibung;
        public string Beschreibung
        {
            get { return _Beschreibung; }
            set { SetPropertyValue<string>(nameof(Beschreibung), ref _Beschreibung, value); }
        }


        private int _DatenbankID;
        [XafDisplayName("Datenbank ID")]
        public int DatenbankID
        {
            get { return _DatenbankID; }
            set { SetPropertyValue<int>(nameof(DatenbankID), ref _DatenbankID, value); }
        }


        [DevExpress.Xpo.Aggregated, Association("Messwert-PV_Modul")]
        [XafDisplayName("Messwerte")]
        public XPCollection<Messwert> Messwert_Liste
        {
            get { return GetCollection<Messwert>(nameof(Messwert_Liste)); }
        }


        private DateTime _DatumInbetriebnahme;
        public DateTime DatumInbetriebnahme
        {
            get { return _DatumInbetriebnahme; }
            set { SetPropertyValue<DateTime>(nameof(DatumInbetriebnahme), ref _DatumInbetriebnahme, value); }
        }


    }
}