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
using MySql.Data.MySqlClient;
using PV_Monitor.Module.BusinessObjects._np;
using PV_Monitor.Module.Win.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PV_Monitor.Module.BusinessObjects;
using System.Windows.Forms;
using DevExpress.Xpo;

namespace PV_Monitor.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PV_Monitor_VC : ViewController
    {
        //PopupWindowShowAction PA_importiereMessWerte;
        SimpleAction SA_importiereMesswerte;
        //readonly string defaultquery = "where id = xyz"
        public PV_Monitor_VC()
        {
            InitializeComponent();
            this.TargetObjectType = typeof(PV_Modul);

            SA_importiereMesswerte = new SimpleAction(this, nameof(SA_importiereMesswerte), PredefinedCategory.Edit);
            SA_importiereMesswerte.Caption = "Importiere Messwerte";
            SA_importiereMesswerte.Execute += SA_importiereMesswerte_Execute;

            //PA_importiereMessWerte = new PopupWindowShowAction(this, nameof(PA_importiereMessWerte), PredefinedCategory.Edit);
            //PA_importiereMessWerte.Caption = "Importiere Messwerte";
            //PA_importiereMessWerte.CustomizePopupWindowParams += PA_importiereMessWerte_CustomizePopupWindowParams;
            //PA_importiereMessWerte.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
            //PA_importiereMessWerte.Execute += PA_importiereMessWerte_Execute;
        }

        private void SA_importiereMesswerte_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IEnumerable<AutoimportEinstellung> autoimportEinstellungen_liste = ObjectSpace.GetObjects<AutoimportEinstellung>().Where(x => x.ImportiereManuell == true);
            IList<PV_Modul> pv_modul_liste = ObjectSpace.GetObjects<PV_Modul>();
            List<string> antwortliste_importierteDatensaetze = new List<string>();
            int counter_jaehrlich = 0; //Zum ermitteln, wie viele Objekte erstellt wurden
            int counter_monatlich = 0;
            int counter_woechentlich = 0;

            foreach (PV_Modul modul in pv_modul_liste)
            {
                DateTime datumErsterMesswert = Sql_helper.Besorge_datumErsterMesswert(modul);

                foreach (AutoimportEinstellung autoimportEinstellung in autoimportEinstellungen_liste)
                {

                    #region -- jährlich ------------------------------------

                    if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Jaehrlich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_jaehrlich;
                        DateTime relevantesDatum = datum_letzterImport;
                        if (datum_letzterImport == DateTime.MinValue)
                        {
                            relevantesDatum = new DateTime(datumErsterMesswert.Year, 1, 1);
                        }

                        if (DateTime.Now.Year >= (relevantesDatum.Year + 1)) //nur Jahreswerte importieren, wenn das letzte Kallenderjahr abgeschlossen ist
                        {

                            //for (int i = relevantesDatum.Year; i < DateTime.Now.Year; i++) 
                            DateTime aktuellesJahr = new DateTime(DateTime.Now.Year, 1, 1);
                            while(relevantesDatum < aktuellesJahr)
                            {
                                if (Importiere_Werte(modul, relevantesDatum , relevantesDatum.AddYears(1).AddMilliseconds(-1), Enum_Autoimport.Jaehrlich) == false)
                                {
                                    return; //Fehler
                                }
                                else
                                {
                                    counter_jaehrlich++;
                                    relevantesDatum = relevantesDatum.AddYears(1);
                                }
                            }

                            modul.Datum_letzterImport_jaehrlich = relevantesDatum;
                            modul.Save();
                            if (modul.DatumInbetriebnahme == DateTime.MinValue)
                            {
                                modul.DatumInbetriebnahme = datumErsterMesswert;
                            }
                        }
                    }

                    #endregion -- jährlich ------------------------------------

                    #region -- monatlich ------------------------------------

                    else if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Monatlich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_monatlich;
                        DateTime aktuellerMonat = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);


                        if (aktuellerMonat >= datum_letzterImport.AddMonths(1)) //nur Monatswerte importieren, wenn der letzte Monat abgeschlossen ist
                        {
                            DateTime relevantesDatum = datum_letzterImport;
                            if (datum_letzterImport == DateTime.MinValue)
                            {
                                relevantesDatum = new DateTime(datumErsterMesswert.Year, datumErsterMesswert.Month, 1); ;
                            }

                            //for (int i = relevantesDatum.Year; i < DateTime.Now.Year; i++)
                            //{
                            while (relevantesDatum.AddMonths(1) < aktuellerMonat)
                            {
                                if (Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddMonths(1).AddMilliseconds(-1), Enum_Autoimport.Monatlich) == false)
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }
                                else
                                {
                                    counter_monatlich++;
                                    relevantesDatum = relevantesDatum.AddMonths(1);
                                }

                                //}
                            }

                            modul.Datum_letzterImport_monatlich = relevantesDatum;
                        }
                    }

                    #endregion -- monatlich ------------------------------------

                    #region -- wöchentlich ------------------------------------

                    else if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Woechentlich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_woechentlich;
                        DayOfWeek aktuellerTag = DateTime.Now.DayOfWeek;
                        DateTime aktuelleWoche_wochenstart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        if (aktuellerTag == DayOfWeek.Tuesday) //Ermitteln des Startes der Woche
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-1);
                        }
                        else if (aktuellerTag == DayOfWeek.Wednesday)
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-2);
                        }
                        else if (aktuellerTag == DayOfWeek.Thursday)
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-3);
                        }
                        else if (aktuellerTag == DayOfWeek.Friday)
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-4);
                        }
                        else if (aktuellerTag == DayOfWeek.Saturday)
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-5);
                        }
                        else if (aktuellerTag == DayOfWeek.Sunday)
                        {
                            aktuelleWoche_wochenstart = aktuelleWoche_wochenstart.AddDays(-6);
                        }

                        DateTime checkWoche = datum_letzterImport.AddDays(7);
                        if (aktuelleWoche_wochenstart >= checkWoche) //nur Wochenwerte importieren, wenn die letzte Woche abgeschlossen ist
                        {
                            DateTime relevantesDatum = datum_letzterImport;
                            if (datum_letzterImport == DateTime.MinValue)
                            {
                                relevantesDatum = new DateTime(datumErsterMesswert.Year, datumErsterMesswert.Month, datumErsterMesswert.Day);
                                DayOfWeek aktuellerTag_relevant = relevantesDatum.DayOfWeek;

                                if (aktuellerTag_relevant == DayOfWeek.Tuesday) //Ermitteln des Startes der Woche
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-1);
                                }
                                else if (aktuellerTag_relevant == DayOfWeek.Wednesday)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-2);
                                }
                                else if (aktuellerTag_relevant == DayOfWeek.Thursday)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-3);
                                }
                                else if (aktuellerTag_relevant == DayOfWeek.Friday)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-4);
                                }
                                else if (aktuellerTag_relevant == DayOfWeek.Saturday)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-5);
                                }
                                else if (aktuellerTag_relevant == DayOfWeek.Sunday)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(-6);
                                }
                            }

                            //for (int i = relevantesDatum.Year; i < DateTime.Now.Year; i++)
                            //{
                            while (relevantesDatum.AddDays(7) < aktuelleWoche_wochenstart)
                            {
                                if (Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddDays(7).AddMilliseconds(-1), Enum_Autoimport.Woechentlich) == false)
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }
                                else
                                {
                                    counter_woechentlich++;
                                    relevantesDatum = relevantesDatum.AddDays(7);
                                }

                                //}
                            }

                            modul.Datum_letzterImport_woechentlich = relevantesDatum;
                        }
                    }

                    #endregion -- wöchentlich ------------------------------------
                }
            }

            #region Counterauswertung
            if (counter_jaehrlich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_jaehrlich} Jahres Messwerte importiert.");
            }
            else if (counter_jaehrlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Jahres Messwert importiert.");
            }

            if (counter_monatlich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_monatlich} Monats Messwerte importiert.");
            }
            else if (counter_monatlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Monats Messwert importiert.");
            }

            if (counter_woechentlich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_woechentlich} Wochen Messwerte importiert.");
            }
            else if(counter_woechentlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Wochen Messwert importiert.");
            }

            #endregion Counterauswertung

            if (antwortliste_importierteDatensaetze.Count > 0)
            {
                ObjectSpace.CommitChanges();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string line in antwortliste_importierteDatensaetze)
                {
                    stringBuilder.Append(line);
                    stringBuilder.Append("\r\n");
                }
                //stringBuilder.Remove(stringBuilder.Length - 5, 4); //Um das letzte "\r\n" zu entfernen
                MessageBox.Show(stringBuilder.ToString());
            }
            else
            {
                WinApp_helper.Erstelle_Fehlermeldung("Es wurden keine neuen Messwerte importiert, weil keine neuen Messwerte gefunden wurden.", Enum_FehlerSchweregrad.Hinweis, Enum_Fehlergruppe.Autoimport);
            }
        }


        private bool Importiere_Werte(PV_Modul modul, DateTime dateTime_start, DateTime dateTime_ende, Enum_Autoimport enum_Autoimport)
        {
            //DateTime dateTime_start = new DateTime(year, 1, 1);
            //DateTime dateTime_ende = new DateTime(year + 1, 1, 1).AddMilliseconds(-1);
            string start = Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_start); //Anfang des Jahres
            string ende = Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_ende); //Ende des Jahres (1 milliSek vor Jahreswechsel)

            #region Maximale Stromproduktion
            string query_minWatt = $"select min(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_minWatt = Sql_helper.Query_select(query_minWatt);
            double maximale_stromproduktion = 0;

            if (result_minWatt != null)
            {
                try
                {
                    maximale_stromproduktion = (double)result_minWatt[0][0];
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Fehler beim parsen des Wertes '{result_minWatt[0][0]}' zu dem Typ 'double' in der Methode '{nameof(PV_Monitor_VC.Importiere_Werte)}'.\r\n Query:\r\n  {query_minWatt}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return false;
                }
            }
            #endregion Maximale Stromproduktion

            #region Anzahl Messwerte
            string query_anzahlMesswerte = $"select count(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_anzahlMesswerte = Sql_helper.Query_select(query_anzahlMesswerte);
            long anzahlMesswerte = 0;
            if (result_anzahlMesswerte != null)
            {
                try
                {
                    anzahlMesswerte = (long)result_anzahlMesswerte[0][0];
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Fehler beim parsen des Wertes '{result_anzahlMesswerte[0][0]}' zu dem Typ 'int' in der Methode '{nameof(PV_Monitor_VC.Importiere_Werte)}'.\r\n Query:\r\n  {query_anzahlMesswerte}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return false;
                }
            }
            #endregion Anzahl Messwerte

            #region Erstellen des Objektes

            Messwert messwert = ObjectSpace.CreateObject<Messwert>();
            messwert.DatumVon = dateTime_start;
            messwert.DatumBis = dateTime_ende;
            messwert.Watt = maximale_stromproduktion;
            messwert.AnzahlMesswerteX = anzahlMesswerte;
            messwert.PV_Modul = modul;
            messwert.AutoimportX = enum_Autoimport;

            #endregion Erstellen des Objektes
            return true;
        }


        //private void PA_importiereMessWerte_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        //{
        //    MesswertImport_np messwertImport = new MesswertImport_np();
        //    messwertImport.Von = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1); // von Anfang letzten Monats 
        //    messwertImport.Bis = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1); // bis Ende letzten Monats
        //    messwertImport.Intervall = new TimeSpan(1, 0, 0, 0);
        //    e.View = Application.CreateDetailView(ObjectSpace.CreateNestedObjectSpace(), messwertImport);
        //}

        //private void PA_importiereMessWerte_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        //{
        //    MesswertImport_np messwertImport = e.PopupWindowView.CurrentObject as MesswertImport_np;
        //    List<PV_Modul> pv_module = new List<PV_Modul>();
        //    foreach (var item in e.SelectedObjects)
        //    {
        //        if (item is PV_Modul modul)
        //        {
        //            pv_module.Add(modul);
        //        }
        //    }
        //    ImportiereMesswerte(pv_module, messwertImport.Von, messwertImport.Bis, messwertImport.Intervall);
        //}

        //private void ImportiereMesswerte(List<PV_Modul> pv_module, DateTime startdatum, DateTime enddatum, TimeSpan intervall)
        //{
            //if (enddatum < startdatum)
            //{
            //    MessageBox.Show("Das Enddatum ist größer als das Startdatum! Bitte geben Sie ein anderes Startdatum an.");
            //    return;
            //}

            //MySqlConnection mySqlConnection = Sql_helper.Connection;
            //DateTime dateTime_runner = startdatum;
            //foreach (PV_Modul modul in pv_module)
            //{
            //    while ((dateTime_runner + intervall) <= enddatum) //volles Intervall
            //    {
            //        if(importiere_Intervall(modul, ref dateTime_runner, enddatum, intervall) == false )
            //        {
            //            return;
            //        }
            //    }

            //    if (importiere_Intervall(modul, ref dateTime_runner, enddatum, enddatum - dateTime_runner) == false) //restliches Intervall
            //    {
            //        return;
            //    }
            //}

            //if (ObjectSpace.IsModified)
            //{
            //    ObjectSpace.CommitChanges();
            //}

            //bool importiere_Intervall(PV_Modul modul, ref DateTime dateTime_runner, DateTime enddatum, TimeSpan intervall)
            //{
            //    string query = $"select sum(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts >= {Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_runner)} and ts <= {Sql_helper.KonvertiereZu_mysqlDatetime(enddatum)} and q = 0";
            //    int counter = 0;
            //    double summe_watt = 0;
            //    MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            //    try
            //    {
            //        mySqlConnection.Open();
            //        MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            //        while (mySqlDataReader.Read())
            //        {
            //            counter++;
            //            string line = mySqlDataReader.GetValue(0).ToString();
            //            if (string.IsNullOrEmpty(line))
            //            {
            //                break;
            //            }
            //            double newvalue = 0;
            //            if (double.TryParse(line, out newvalue) == true)
            //            {
            //                summe_watt += newvalue;
            //            }
            //            else
            //            {
            //                MessageBox.Show($"Fehler beim Importieren des Wertes '{line}'");
            //                return false;
            //            }
            //        }
            //    }
            //    catch(Exception e)
            //    {
            //        MessageBox.Show("Fehler beim Auslesen der Messwerte aus der Mysql Datenbank. Stacktrace: \r\n" + e);
            //    }
            //    finally
            //    {
            //        mySqlConnection.Close();
            //    }

            //    //Ende des Importes des aktuellen Intervalls
            //    Messwert messwert = ObjectSpace.CreateObject<Messwert>();
            //    messwert.Uhrzeit = dateTime_runner;
            //    messwert.UhrzeitBis = dateTime_runner + intervall;
            //    messwert.Intervall = intervall;  
            //    messwert.PV_Modul = modul;
            //    messwert.Watt = Math.Round((summe_watt / counter), 1);

            //    dateTime_runner += intervall;
            //    return true;
            //}
        //}

        #region Devexpress
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
        #endregion Devexpress
    }
}
