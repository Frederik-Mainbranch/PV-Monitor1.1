using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PV_Monitor.Module.BusinessObjects;

namespace PV_Monitor.Module.Win.Helper
{
    public static class Einstellung_helper
    {
        public static bool BesorgeEinstellung(Einstellung.Enum_AuswahlEinstellung_bool enum_AuswahlEinstellung)
        {
            DevExpress.ExpressApp.IObjectSpace os = WinApp_helper.App.CreateObjectSpace();
            Einstellung einstellung = os.FirstOrDefault<Einstellung>(x => x.AuswahlEinstellungX == enum_AuswahlEinstellung);
            if (einstellung == null)
            {
                WinApp_helper.Erstelle_Fehlermeldung($"Die gesuchte Einstellung '{enum_AuswahlEinstellung.ToString()}' wurde nicht gefunden. Bitte wenden Sie sich an Ihren Entwickler!.", Enum_FehlerSchweregrad.Schwer, Enum_Fehlergruppe.Allgemein);
                WinApp_helper.App.Exit();
            }
            bool result = einstellung.Wert_bool;
            return result;
        }
    }
}
