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
    [DefaultProperty(nameof(Datum))]

    public class Importbericht : BaseObject
    { 
        public Importbericht(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Datum = DateTime.Now;
        }


        private DateTime _Datum;
        [XafDisplayName("Importdatum")]
        public DateTime Datum
        {
            get { return _Datum; }
            set { SetPropertyValue<DateTime>(nameof(Datum), ref _Datum, value); }
        }


        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { SetPropertyValue<string>(nameof(Text), ref _Text, value); }
        }


        private Enum_Autoimport _AutoimportArt;
        public Enum_Autoimport AutoimportArt
        {
            get { return _AutoimportArt; }
            set { SetPropertyValue<Enum_Autoimport>(nameof(AutoimportArt), ref _AutoimportArt, value); }
        }


    }
}