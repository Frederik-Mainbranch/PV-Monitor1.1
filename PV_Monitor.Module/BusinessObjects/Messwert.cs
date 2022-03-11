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

        private DateTime _Uhrzeit;
        public DateTime Uhrzeit
        {
            get { return _Uhrzeit; }
            set { SetPropertyValue<DateTime>(nameof(Uhrzeit), ref _Uhrzeit, value); }
        }


        private double _Watt;
        public double Watt
        {
            get { return _Watt; }
            set { SetPropertyValue<double>(nameof(Watt), ref _Watt, value); }
        }


        private PV_Modul _PV_Modul;
        [XafDisplayName("PV Modul")]
        [DevExpress.Xpo.Aggregated, Association("Messwert-PV_Modul")]
        public PV_Modul PV_Modul
        {
            get { return _PV_Modul; }
            set { SetPropertyValue<PV_Modul>(nameof(PV_Modul), ref _PV_Modul, value); }
        }


    }
}