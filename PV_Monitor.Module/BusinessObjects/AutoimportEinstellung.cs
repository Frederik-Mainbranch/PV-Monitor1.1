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
    [DefaultProperty(nameof(Beschreibung))]

    public class AutoimportEinstellung : BaseObject
    { 
        public AutoimportEinstellung(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Enum_AutoimportX = Enum_Autoimport.Manuell;
        }


        //private bool _IstAktiv;
        //public bool IstAktiv
        //{
        //    get { return _IstAktiv; }
        //    set { SetPropertyValue<bool>(nameof(IstAktiv), ref _IstAktiv, value); }
        //}


        private bool _ImportiereBeiProgrammstart;
        [XafDisplayName("Importiere bei Programmstart")]
        public bool ImportiereBeiProgrammstart
        {
            get { return _ImportiereBeiProgrammstart; }
            set { SetPropertyValue<bool>(nameof(ImportiereBeiProgrammstart), ref _ImportiereBeiProgrammstart, value); }
        }


        private bool _ImportiereManuell;
        [ImmediatePostData]
        [XafDisplayName("Importiere manuell")]
        public bool ImportiereManuell
        {
            get { return _ImportiereManuell; }
            set { SetPropertyValue<bool>(nameof(ImportiereManuell), ref _ImportiereManuell, value); }
        }


        private string _Beschreibung;
        public string Beschreibung
        {
            get { return _Beschreibung; }
            set { SetPropertyValue<string>(nameof(Beschreibung), ref _Beschreibung, value); }
        }


        private Enum_Autoimport _Enum_AutoimportX;
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        [VisibleInDashboards(false)]
        public Enum_Autoimport Enum_AutoimportX
        {
            get { return _Enum_AutoimportX; }
            set { SetPropertyValue<Enum_Autoimport>(nameof(Enum_AutoimportX), ref _Enum_AutoimportX, value); }
        }
    }

    public enum Enum_Autoimport
    {
        [XafDisplayName("manuell")]
        Manuell,
        [XafDisplayName("viertelstündlich")]
        Viertelstuendlich,
        [XafDisplayName("stündlich")]
        Stuendlich,
        [XafDisplayName("täglich")]
        Taeglich,
        [XafDisplayName("wöchentlich")]
        Woechentlich,
        [XafDisplayName("monatlich")]
        Monatlich,
        [XafDisplayName("jährlich")]
        Jaehrlich
    }
}