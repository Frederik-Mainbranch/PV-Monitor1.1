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

            //Dictionary<Enum_Autoimport, DateTime> datum_letzterImport = new Dictionary<Enum_Autoimport, DateTime>();
            //datum_letzterImport[Enum_Autoimport.Jaehrlich] = DateTime.MinValue;
            //datum_letzterImport[Enum_Autoimport.Monatlich] = DateTime.MinValue;
            //datum_letzterImport[Enum_Autoimport.Woechentlich] = DateTime.MinValue;
            //datum_letzterImport[Enum_Autoimport.Taeglich] = DateTime.MinValue;
            //datum_letzterImport[Enum_Autoimport.Stuendlich] = DateTime.MinValue;
            //datum_letzterImport[Enum_Autoimport.Viertelstuendlich] = DateTime.MinValue;
            //Datum_letzterImport = datum_letzterImport;
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading && !IsSaving)
            {
                if ((propertyName == nameof(DatenbankID_Leistung) || propertyName == nameof(DatenbankID_ExportKwh)) && App_helper.IstEingeloggt && App_helper.WarnmeldungDatenbankIDWurdeGezeigt == false)
                {
                    App_helper.WarnmeldungDatenbankIDWurdeGezeigt = true;
                    Agnostic_Caller_Helper.ZeigeMessageBox("Das Eintragen einer falschen Datenbank ID kann zu kritischen Fehlern führen. Bitte vergewissern Sie sich, dass die ID korrekt ist vor dem speichern!");
                }
            }
        }


        //private Dictionary<Enum_Autoimport, DateTime> _Datum_letzterImport;
        //[Browsable(false)]
        //public Dictionary<Enum_Autoimport, DateTime> Datum_letzterImport
        //{
        //    get
        //    {
        //        if (_Datum_letzterImport == null)
        //        {
        //            _Datum_letzterImport = new Dictionary<Enum_Autoimport, DateTime>();
        //            _Datum_letzterImport[Enum_Autoimport.Jaehrlich] = DateTime.MinValue;
        //            _Datum_letzterImport[Enum_Autoimport.Monatlich] = DateTime.MinValue;
        //            _Datum_letzterImport[Enum_Autoimport.Woechentlich] = DateTime.MinValue;
        //            _Datum_letzterImport[Enum_Autoimport.Taeglich] = DateTime.MinValue;
        //            _Datum_letzterImport[Enum_Autoimport.Stuendlich] = DateTime.MinValue;
        //            _Datum_letzterImport[Enum_Autoimport.Viertelstuendlich] = DateTime.MinValue;
        //        }
        //        return _Datum_letzterImport;
        //    }

        //    set { SetPropertyValue<Dictionary<Enum_Autoimport, DateTime>>(nameof(Datum_letzterImport), ref _Datum_letzterImport, value); }
        //}


        private DateTime _Datum_letzterImport_jaehrlich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_jaehrlich
        {
            get { return _Datum_letzterImport_jaehrlich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_jaehrlich), ref _Datum_letzterImport_jaehrlich, value); }
        }


        private DateTime _Datum_letzterImport_monatlich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_monatlich
        {
            get { return _Datum_letzterImport_monatlich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_monatlich), ref _Datum_letzterImport_monatlich, value); }
        }


        private DateTime _Datum_letzterImport_woechentlich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_woechentlich
        {
            get { return _Datum_letzterImport_woechentlich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_woechentlich), ref _Datum_letzterImport_woechentlich, value); }
        }


        private DateTime _Datum_letzterImport_taeglich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_taeglich
        {
            get { return _Datum_letzterImport_taeglich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_taeglich), ref _Datum_letzterImport_taeglich, value); }
        }


        private DateTime _Datum_letzterImport_stuendlich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_stuendlich
        {
            get { return _Datum_letzterImport_stuendlich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_stuendlich), ref _Datum_letzterImport_stuendlich, value); }
        }


        private DateTime _Datum_letzterImport_viertelstuendlich;
        [Browsable(false)]
        public DateTime Datum_letzterImport_viertelstuendlich
        {
            get { return _Datum_letzterImport_viertelstuendlich; }
            set { SetPropertyValue<DateTime>(nameof(Datum_letzterImport_viertelstuendlich), ref _Datum_letzterImport_viertelstuendlich, value); }
        }




        private string _Beschreibung;
        public string Beschreibung
        {
            get { return _Beschreibung; }
            set { SetPropertyValue<string>(nameof(Beschreibung), ref _Beschreibung, value); }
        }


        private int _DatenbankID_Leistung;
        [XafDisplayName("Datenbank ID Leistung")]
        public int DatenbankID_Leistung
        {
            get { return _DatenbankID_Leistung; }
            set { SetPropertyValue<int>(nameof(DatenbankID_Leistung), ref _DatenbankID_Leistung, value); }
        }


        private int _DatenbankID_ExportKwh;
        [XafDisplayName("Datenbank ID Export kW/H")]
        public int DatenbankID_ExportKwh
        {
            get { return _DatenbankID_ExportKwh; }
            set { SetPropertyValue<int>(nameof(DatenbankID_ExportKwh), ref _DatenbankID_ExportKwh, value); }
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