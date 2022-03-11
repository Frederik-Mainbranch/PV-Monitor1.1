using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using PV_Monitor.Module.BusinessObjects._np;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PV_Monitor.Module.Controllers._np
{
    public partial class Messagebox_np_VC : ViewController
    {
        SimpleAction SA_ok;
        public Messagebox_np_VC()
        {
            InitializeComponent();
            TargetObjectType = typeof(Messagebox_np);
            TargetViewType = ViewType.DetailView;

            SA_ok = new SimpleAction(this, nameof(this.SA_ok), PredefinedCategory.Edit);
            SA_ok.Caption = "Ok";
            SA_ok.Execute += SA_ok_Execute;
        }

        private void SA_ok_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            View.Close();
        }

        #region DevExpress

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        #endregion DevExpress
    }
}
