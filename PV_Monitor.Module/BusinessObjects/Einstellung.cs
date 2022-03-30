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

    public class Einstellung : BaseObject
    {
        public Einstellung(Session session)
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
                if (propertyName == nameof(AuswahlEinstellungX))
                {
                    if (AuswahlEinstellungX == Enum_AuswahlEinstellung_bool.ZeigeAutoimportBenachrichtigung)
                    {
                        TypDerVariablen = Enum_Einstellungstypen._bool;
                    }
                }
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetPropertyValue<string>(nameof(Name), ref _Name, value); }
        }


        private string _Beschreibung;
        public string Beschreibung
        {
            get { return _Beschreibung; }
            set { SetPropertyValue<string>(nameof(Beschreibung), ref _Beschreibung, value); }
        }

        public enum Enum_Einstellungstypen
        {
            _string,
            _int,
            _bool,
            _double
        }

        public enum Enum_AuswahlEinstellung_bool
        {
            keine,
            ZeigeAutoimportBenachrichtigung
        }

        private Enum_Einstellungstypen _TypDerVariablen;
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInDashboards(false)]
        public Enum_Einstellungstypen TypDerVariablen
        {
            get { return _TypDerVariablen; }
            set { SetPropertyValue<Enum_Einstellungstypen>(nameof(TypDerVariablen), ref _TypDerVariablen, value); }
        }


        private Enum_AuswahlEinstellung_bool _AuswahlEinstellungX;
        [Browsable(false)]
        public Enum_AuswahlEinstellung_bool AuswahlEinstellungX
        {
            get { return _AuswahlEinstellungX; }
            set { SetPropertyValue<Enum_AuswahlEinstellung_bool>(nameof(AuswahlEinstellungX), ref _AuswahlEinstellungX, value); }
        }


        private string _Wert_string;
        [XafDisplayName("Wert")]
        public string Wert_string
        {
            get { return _Wert_string; }
            set { SetPropertyValue<string>(nameof(Wert_string), ref _Wert_string, value); }
        }


        private int _Wert_int;
        [XafDisplayName("Wert")]
        public int Wert_int
        {
            get { return _Wert_int; }
            set { SetPropertyValue<int>(nameof(Wert_int), ref _Wert_int, value); }
        }


        private double _Wert_double;
        [XafDisplayName("Wert")]
        public double Wert_double
        {
            get { return _Wert_double; }
            set { SetPropertyValue<double>(nameof(Wert_double), ref _Wert_double, value); }
        }


        private bool _Wert_bool;
        [XafDisplayName("Ist aktiv")]
        public bool Wert_bool
        {
            get { return _Wert_bool; }
            set { SetPropertyValue<bool>(nameof(Wert_bool), ref _Wert_bool, value); }
        }
    }

}