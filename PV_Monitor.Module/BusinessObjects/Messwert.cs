using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
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
    [NavigationItem("Überwachung")]

    public class Messwert : BaseObject
    { 
        public Messwert(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        //private DateTime _Uhrzeit;
        //[XafDisplayName("Uhrzeit von")]
        //public DateTime Uhrzeit
        //{
        //    get { return _Uhrzeit; }
        //    set { SetPropertyValue<DateTime>(nameof(Uhrzeit), ref _Uhrzeit, value); }
        //}


        private DateTime _DatumVon;
        public DateTime DatumVon
        {
            get { return _DatumVon; }
            set { SetPropertyValue<DateTime>(nameof(DatumVon), ref _DatumVon, value); }
        }


        private DateTime _DatumBis;
        public DateTime DatumBis
        {
            get { return _DatumBis; }
            set { SetPropertyValue<DateTime>(nameof(DatumBis), ref _DatumBis, value); }
        }


        private Enum_Autoimport _AutoimportX;
        [XafDisplayName("Autoimport Typ")]
        public Enum_Autoimport AutoimportX
        {
            get { return _AutoimportX; }
            set { SetPropertyValue<Enum_Autoimport>(nameof(AutoimportX), ref _AutoimportX, value); }
        }



        //private DateTime _UhrzeitBis;
        //[XafDisplayName("Uhrzeit bis")]
        //public DateTime UhrzeitBis
        //{
        //    get { return _UhrzeitBis; }
        //    set { SetPropertyValue<DateTime>(nameof(UhrzeitBis), ref _UhrzeitBis, value); }
        //}


        //private TimeSpan _Intervall;
        //public TimeSpan Intervall
        //{
        //    get { return _Intervall; }
        //    set { SetPropertyValue<TimeSpan>(nameof(Intervall), ref _Intervall, value); }
        //}

        private double _Watt;
        [XafDisplayName("Max Watt")]
        public double Watt
        {
            get { return _Watt; }
            set { SetPropertyValue<double>(nameof(Watt), ref _Watt, value); }
        }


        private double _DurchschnittWatt;
        [XafDisplayName("Durchschnitt Watt")]
        public double DurchschnittWatt
        {
            get { return _DurchschnittWatt; }
            set { SetPropertyValue<double>(nameof(DurchschnittWatt), ref _DurchschnittWatt, value); }
        }




        private double _ExportKwH;
        [XafDisplayName("Exportierte kW/H")]
        public double ExportKwH
        {
            get { return _ExportKwH; }
            set { SetPropertyValue<double>(nameof(ExportKwH), ref _ExportKwH, value); }
        }


        private PV_Modul _PV_Modul;
        [XafDisplayName("PV Modul")]
        [Association("Messwert-PV_Modul")]
        public PV_Modul PV_Modul
        {
            get { return _PV_Modul; }
            set { SetPropertyValue<PV_Modul>(nameof(PV_Modul), ref _PV_Modul, value); }
        }


        private long _AnzahlMesswerteX;
        [XafDisplayName("Anzahl Messwerte")]
        public long AnzahlMesswerteX
        {
            get { return _AnzahlMesswerteX; }
            set { SetPropertyValue<long>(nameof(AnzahlMesswerteX), ref _AnzahlMesswerteX, value); }
        }



    }
}