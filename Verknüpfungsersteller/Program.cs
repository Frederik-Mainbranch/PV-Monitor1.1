using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Verknüpfungsersteller
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutLocation = System.IO.Path.Combine(deskDir, "PV Monitor" + ".lnk");

            string targetPath = Path.Combine(AppContext.BaseDirectory, "PV_Monitor.Win.exe");

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Öffnet den PV Monitor";   // The description of the shortcut
            shortcut.IconLocation = targetPath;           // The icon of the shortcut
            //shortcut.IconLocation = Path.Combine(AppContext.BaseDirectory, "ExpressApp.ico");           // The icon of the shortcut
            shortcut.TargetPath = targetPath;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();
        }
    }
}
