using DevExpress.XtraSplashScreen;
using PV_Monitor.Module.Win.Waitforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Win.Helper
{
    public static class SplashScreen_helper
    {
        public static string Text { get; set; }
        public static string StammCaption { get; set; }
        public static int Anzahl_Schritte { get; set; }
        public static int Aktueller_Schritt { get; set; }

        public static void Zeige_Splashscreen(string stamm_caption, string description)
        {
            StammCaption = stamm_caption;
            Anzahl_Schritte = 0;
            Aktueller_Schritt = 0;
            //SplashScreenManager.ShowDefaultWaitForm(stamm_caption, description);
            SplashScreenManager.ShowForm(typeof(DemoWaitForm2));
            SplashScreenManager.Default.Properties.ClosingDelay = 1500;
        }

        public static void UpdateCation_Schritt()
        {
            Aktueller_Schritt++;
            SplashScreenManager.Default.SetWaitFormCaption(StammCaption + $" {Aktueller_Schritt}/{Anzahl_Schritte}");
        }

        public static void UpdateDescription_Schritt(int indexModul, string autoimporttyp)
        {
            SplashScreenManager.Default.SetWaitFormDescription($"PV Modul {indexModul + 1} {autoimporttyp}");
        }

        public static void Update_Splashscreen_description(string text)
        {
            if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    Text = text;
                }
                else
                {
                    Text = Text + "\r\n" + text;
                }

                SplashScreenManager.Default.SetWaitFormDescription(Text);
            }
        }

        public static void Update_Splashscreen_caption(string text)
        {
            if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
            {
                SplashScreenManager.Default.SetWaitFormCaption(text);
            }
        }

        public static void Close_Splashscreen()
        {
            if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
            {
                Text = "";
                SplashScreenManager.CloseDefaultWaitForm();
            }
        }


    }
}
