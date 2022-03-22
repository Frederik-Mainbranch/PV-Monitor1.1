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

namespace PV_Monitor.Module.BusinessObjects
{
    [DefaultClassOptions]

    public class Fehlerprotokoll : BaseObject
    { 
        public Fehlerprotokoll(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Datum = DateTime.Now;
            Schweregrad = Enum_FehlerSchweregrad.Hinweis;
            Fehlergruppe = Enum_Fehlergruppe.Allgemein;
        }


        private DateTime _Datum;
        public DateTime Datum
        {
            get { return _Datum; }
            set { SetPropertyValue<DateTime>(nameof(Datum), ref _Datum, value); }
        }


        private string _Beschreibung;
        public string Beschreibung
        {
            get { return _Beschreibung; }
            set { SetPropertyValue<string>(nameof(Beschreibung), ref _Beschreibung, value); }
        }



        private Enum_Fehlergruppe _Fehlergruppe;
        public Enum_Fehlergruppe Fehlergruppe
        {
            get { return _Fehlergruppe; }
            set { SetPropertyValue<Enum_Fehlergruppe>(nameof(Fehlergruppe), ref _Fehlergruppe, value); }
        }




        private Enum_FehlerSchweregrad _Schweregrad;
        public Enum_FehlerSchweregrad Schweregrad
        {
            get { return _Schweregrad; }
            set { SetPropertyValue<Enum_FehlerSchweregrad>(nameof(Schweregrad), ref _Schweregrad, value); }
        }


    }

    public enum Enum_Fehlergruppe
    {
        Allgemein,
        Datenbankzugriff,
        Typkonvertierung,
        Autoimport
    }

    public enum Enum_FehlerSchweregrad
    {
        Hinweis,
        Leicht,
        Mittel,
        Schwer
    }
}