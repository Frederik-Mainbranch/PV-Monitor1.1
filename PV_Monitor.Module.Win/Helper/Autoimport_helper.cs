using DevExpress.ExpressApp;
using DevExpress.XtraSplashScreen;
using PV_Monitor.Module.BusinessObjects;
using PV_Monitor.Module.Win.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PV_Monitor.Module.Win.Helper
{
    public static class Autoimport_helper
    {

        public static void StarteAutoimportV1_multiT(bool istProgrammstart)
        {
            Thread importThread = new Thread(() => StarteAutoimportV1(istProgrammstart));
            importThread.Start();
        }

        public static void StarteAutoimportV2_multiT(bool istProgrammstart)
        {
            Task.Run(() => StarteAutoimportV2_async(istProgrammstart));
        }

        public static async void StarteAutoimportV2_async(bool istProgrammstart)
        {
            IObjectSpace os = WinApp_helper.App.CreateObjectSpace();

            IEnumerable<AutoimportEinstellung> autoimportEinstellungen_liste = null;
            if (istProgrammstart)
            {
                autoimportEinstellungen_liste = os.GetObjects<AutoimportEinstellung>().Where(x => x.ImportiereBeiProgrammstart == true);
            }
            else
            {
                autoimportEinstellungen_liste = os.GetObjects<AutoimportEinstellung>().Where(x => x.ImportiereManuell == true);
            }

            IList<PV_Modul> pv_modul_liste = os.GetObjects<PV_Modul>();
            List<string> antwortliste_importierteDatensaetze = new List<string>();
            double dauer_import_rohdaten = 0;
            double dauer_konvertierung = 0;
            int counter_jaehrlich = 0; //Zum ermitteln, wie viele Objekte erstellt wurden
            int counter_monatlich = 0;
            int counter_woechentlich = 0;
            int counter_taeglich = 0;
            int counter_stuendlich = 0;
            int counter_viertelstuendlich = 0;
            Stopwatch stopwatch_import_gesamt = Stopwatch.StartNew();


            #region besorgen aller Rohdaten
            //Ermitteln des Importzeitraumes


            #endregion besorgen aller Rohdaten

            foreach (PV_Modul modul in pv_modul_liste) //durchgehen aller module
            {
                DateTime datum_von = modul.Datum_letzterImport_viertelstuendlich;
                if (datum_von == DateTime.MinValue)
                {
                    datum_von = new DateTime(2000, 1, 1, 0, 0, 0);
                }

                int minuten = 0;
                int minuten_now = DateTime.Now.Minute;
                if (minuten_now >= 15 && minuten_now < 30)
                {
                    minuten = 15;
                }
                else if (minuten_now >= 30 && minuten_now < 45)
                {
                    minuten = 30;
                }
                else if (minuten_now >= 45)
                {
                    minuten = 45;
                }

                DateTime datum_bis = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, minuten, 0);


                if (datum_von.AddMinutes(15) < datum_bis)
                {
                    #region -- Besorgen der Rohdaten --
                    Stopwatch stopwatch_importRohdaten = Stopwatch.StartNew();
                    string start = Sql_helper.KonvertiereZu_mysqlDatetime(datum_von);
                    string ende = Sql_helper.KonvertiereZu_mysqlDatetime(datum_bis);
                    string query_watt = $"select ts, val from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0 order by ts asc";
                    string query_kWh = $"select ts, val from ts_number where id = {modul.DatenbankID_kWh} and ts > {start} and ts < {ende} and q = 0 order by ts asc";
                    List<object[]> result_rohdaten_watt = Sql_helper.Query_select(query_watt);
                    List<object[]> result_rohdaten_kWh = Sql_helper.Query_select(query_kWh);
                    stopwatch_importRohdaten.Stop();
                    dauer_import_rohdaten += Math.Round((stopwatch_importRohdaten.ElapsedMilliseconds / 1000.0d), 1);
                    #endregion -- Besorgen der Rohdaten --

                    #region -- Konvertierung der Rohdaten in benutzbare Daten --
                    Stopwatch stopwatch_konvertierung = Stopwatch.StartNew();

                    int anzahl_watt = result_rohdaten_watt.Count;
                    (DateTime uhrzeit, double wert)[] konvertierteMesswerte_watt = new (DateTime uhrzeit, double wert)[anzahl_watt];
                    for (int i = 0; i < anzahl_watt - 1; i++)
                    {
                        konvertierteMesswerte_watt[i] = new(Sql_helper.KonvertiereZu_Datetime((long)result_rohdaten_watt[i][0]), (double)result_rohdaten_watt[i][1]);
                    }

                    int anzahl_kWh = result_rohdaten_kWh.Count;
                    (DateTime uhrzeit, double wert)[] konvertierteMesswerte_kWh = new (DateTime uhrzeit, double wert)[anzahl_kWh];
                    for (int i = 0; i < anzahl_kWh; i++)
                    {
                        konvertierteMesswerte_kWh[i] = new(Sql_helper.KonvertiereZu_Datetime((long)result_rohdaten_kWh[i][0]), (double)result_rohdaten_kWh[i][1]);
                    }
                    stopwatch_konvertierung.Stop();

                    dauer_konvertierung += Math.Round((stopwatch_konvertierung.ElapsedMilliseconds / 1000.0d), 1);
                    #endregion -- Konvertierung der Rohdaten in benutzbare Daten --

                    #region -- Erstellen von neuen Messwerten --
                    Stopwatch stopwatch_messwerteErstellen = Stopwatch.StartNew();

                    //watt
                    DateTime erster_messwert_datum = konvertierteMesswerte_watt[0].uhrzeit;
                    int minuten_erstesDatum = erster_messwert_datum.Minute;
                    int relevante_minuten = 0;
                    if (minuten_erstesDatum >= 15 && minuten_erstesDatum < 30)
                    {
                        relevante_minuten = 15;
                    }
                    else if (minuten_erstesDatum >= 30 && minuten_erstesDatum < 45)
                    {
                        relevante_minuten = 30;
                    }
                    else if (minuten_erstesDatum >= 45)
                    {
                        relevante_minuten = 45;
                    }
                    DateTime intervall_start = new DateTime(erster_messwert_datum.Year, erster_messwert_datum.Month, erster_messwert_datum.Day, erster_messwert_datum.Hour, relevante_minuten, 0);
                    DateTime intervall_ende = intervall_start.AddMinutes(15);
                    int index_start = 0;
                    int index_ende = 0;

                    for (int i = 0; i < konvertierteMesswerte_watt.Length - 1; i++)
                    {
                        if(konvertierteMesswerte_watt[i].uhrzeit >= intervall_ende)
                        {
                            Messwert messwert = os.CreateObject<Messwert>();
                            index_ende = i - 1;
                            messwert.AnzahlMesswerteX = index_ende - index_start;
                            index_start = i;
                            messwert.PV_Modul = modul;

                            messwert.DatumVon = intervall_start;
                            messwert.DatumBis = intervall_ende;

                        }
                    }







                    stopwatch_messwerteErstellen.Stop();

                    #endregion -- Erstellen von neuen Messwerten --
                }
            }

            stopwatch_import_gesamt.Stop();

            MessageBox.Show($"Dauer Import der Rohdaten: {dauer_import_rohdaten} s, Dauer Konvertierung: {dauer_konvertierung} s.\r\nDer gesammte Import hat {stopwatch_import_gesamt.ElapsedMilliseconds / 1000.0d} s gedauert");
        }


        public static void StarteAutoimportV1(bool istProgrammstart)
        {
            IObjectSpace os = WinApp_helper.App.CreateObjectSpace();

            IEnumerable<AutoimportEinstellung> autoimportEinstellungen_liste = null;
            if (istProgrammstart)
            {
                autoimportEinstellungen_liste = os.GetObjects<AutoimportEinstellung>().Where(x => x.ImportiereBeiProgrammstart == true);
            }
            else
            {
                autoimportEinstellungen_liste = os.GetObjects<AutoimportEinstellung>().Where(x => x.ImportiereManuell == true);
            }

            IList<PV_Modul> pv_modul_liste = os.GetObjects<PV_Modul>();
            List<string> antwortliste_importierteDatensaetze = new List<string>();
            int counter_jaehrlich = 0; //Zum ermitteln, wie viele Objekte erstellt wurden
            int counter_monatlich = 0;
            int counter_woechentlich = 0;
            int counter_taeglich = 0;
            int counter_stuendlich = 0;
            int counter_viertelstuendlich = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (PV_Modul modul in pv_modul_liste)
            {
                DateTime datumErsterMesswert = Sql_helper.Besorge_datumErsterMesswert(modul);
                int anzahlImportMesswerte_start = counter_jaehrlich + counter_monatlich + counter_woechentlich + counter_taeglich + counter_stuendlich + counter_viertelstuendlich;

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
                            while (relevantesDatum < aktuellesJahr)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddYears(1).AddMilliseconds(-1), Enum_Autoimport.Jaehrlich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_jaehrlich++;
                                    relevantesDatum = relevantesDatum.AddYears(1);

                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddYears(1);
                                }
                                else
                                {
                                    return; //Fehler
                                }
                            }

                           // modul.Datum_letzterImport_jaehrlich = relevantesDatum;
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
                            while (relevantesDatum < aktuellerMonat)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddMonths(1).AddMilliseconds(-1), Enum_Autoimport.Monatlich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_monatlich++;
                                    relevantesDatum = relevantesDatum.AddMonths(1);
                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddMonths(1);
                                }
                                else
                                {
                                    return;
                                }

                                //}
                            }

                          //  modul.Datum_letzterImport_monatlich = relevantesDatum;
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
                            while (relevantesDatum < aktuelleWoche_wochenstart)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddDays(7).AddMilliseconds(-1), Enum_Autoimport.Woechentlich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_woechentlich++;
                                    relevantesDatum = relevantesDatum.AddDays(7);
                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(7);
                                }
                                else
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }

                                //}
                            }

                           // modul.Datum_letzterImport_woechentlich = relevantesDatum;
                        }
                    }

                    #endregion -- wöchentlich ------------------------------------

                    #region -- täglich ------------------------------------

                    else if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Taeglich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_taeglich;
                        DateTime datum_aktuellerTag = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);


                        if (datum_aktuellerTag >= datum_letzterImport.AddDays(1)) //nur Wochenwerte importieren, wenn die letzte Woche abgeschlossen ist
                        {
                            DateTime relevantesDatum = datum_letzterImport;
                            if (datum_letzterImport == DateTime.MinValue)
                            {
                                relevantesDatum = new DateTime(datumErsterMesswert.Year, datumErsterMesswert.Month, datumErsterMesswert.Day);
                            }

                            while (relevantesDatum < datum_aktuellerTag)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddDays(1).AddMilliseconds(-1), Enum_Autoimport.Taeglich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_taeglich++;
                                    relevantesDatum = relevantesDatum.AddDays(1);
                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddDays(1);
                                }
                                else
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }
                            }

                           // modul.Datum_letzterImport_taeglich = relevantesDatum;
                        }
                    }

                    #endregion -- täglich ------------------------------------

                    #region -- stündlich ------------------------------------

                    else if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Stuendlich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_stuendlich;
                        DateTime datum_aktuelleUhrzeit = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);


                        if (datum_aktuelleUhrzeit >= datum_letzterImport.AddHours(1)) //nur Stundenwerte importieren, wenn die letzte Stunde um ist
                        {
                            DateTime relevantesDatum = datum_letzterImport;
                            if (datum_letzterImport == DateTime.MinValue)
                            {
                                relevantesDatum = new DateTime(datumErsterMesswert.Year, datumErsterMesswert.Month, datumErsterMesswert.Day, datumErsterMesswert.Hour, 0, 0);
                            }

                            while (relevantesDatum < datum_aktuelleUhrzeit)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddHours(1).AddMilliseconds(-1), Enum_Autoimport.Stuendlich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_stuendlich++;
                                    relevantesDatum = relevantesDatum.AddHours(1);
                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddHours(1);
                                }
                                else
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }
                            }

                         ///   modul.Datum_letzterImport_stuendlich = relevantesDatum;
                        }
                    }

                    #endregion -- stündlich ------------------------------------

                    #region -- viertelstündlich ------------------------------------

                    else if (autoimportEinstellung.Enum_AutoimportX == Enum_Autoimport.Viertelstuendlich)
                    {
                        DateTime datum_letzterImport = modul.Datum_letzterImport_viertelstuendlich;
                        DateTime datum_aktuelleUhrzeit = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 1);


                        if (datum_aktuelleUhrzeit >= datum_letzterImport.AddMinutes(15)) //nur Viertelstundenwerte importieren, wenn die letzte Viertelstunde um ist
                        {
                            DateTime relevantesDatum = datum_letzterImport;
                            if (datum_letzterImport == DateTime.MinValue)
                            {
                                relevantesDatum = new DateTime(datumErsterMesswert.Year, datumErsterMesswert.Month, datumErsterMesswert.Day, datumErsterMesswert.Hour, 0, 0); //Ausnahme: fängt nicht beim ersten Wert an, sondern bei Minute 1 der Stunde des Startwertes, damit 4 Zeitintervalle immer nur eine "Zeitstunde" abdecken, z.B. 12:00 - 13:00 und nicht 12:06 - 13:06
                            }

                            while (relevantesDatum < datum_aktuelleUhrzeit)
                            {
                                Enum_DatenimportErfolgreich erfolgtest = Importiere_Werte(modul, relevantesDatum, relevantesDatum.AddMinutes(15).AddMilliseconds(-1), Enum_Autoimport.Viertelstuendlich, os);
                                if (erfolgtest == Enum_DatenimportErfolgreich.MesswertImportiert)
                                {
                                    counter_viertelstuendlich++;
                                    relevantesDatum = relevantesDatum.AddMinutes(15);
                                }
                                else if (erfolgtest == Enum_DatenimportErfolgreich.KeinMesswertImportiert)
                                {
                                    relevantesDatum = relevantesDatum.AddMinutes(15);
                                }
                                else
                                {
                                    return; //Fehler, Fehlerprotokoll wurde schon erstellt
                                }
                            }

                            modul.Datum_letzterImport_stuendlich = relevantesDatum;
                        }
                    }

                    #endregion -- viertelstündlich ------------------------------------
                }

                int anzahlImportMesswerte_ende = counter_jaehrlich + counter_monatlich + counter_woechentlich + counter_taeglich + counter_stuendlich + counter_viertelstuendlich;

                if (anzahlImportMesswerte_ende > anzahlImportMesswerte_start)
                {
                    if (modul.DatumInbetriebnahme == DateTime.MinValue)
                    {
                      //  modul.DatumInbetriebnahme = datumErsterMesswert;
                    }
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
            else if (counter_woechentlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Wochen Messwert importiert.");
            }

            if (counter_taeglich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_taeglich} Tages Messwerte importiert.");
            }
            else if (counter_taeglich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Tages Messwert importiert.");
            }

            if (counter_stuendlich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_stuendlich} Stunden Messwerte importiert.");
            }
            else if (counter_stuendlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Stunden Messwert importiert.");
            }

            if (counter_viertelstuendlich > 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurden {counter_viertelstuendlich} Viertelstunden Messwerte importiert.");
            }
            else if (counter_viertelstuendlich == 1)
            {
                antwortliste_importierteDatensaetze.Add($"Es wurde 1 Viertelstunden Messwert importiert.");
            }

            #endregion Counterauswertung

            if (antwortliste_importierteDatensaetze.Count > 0)
            {

                os.CommitChanges();
                stopwatch.Stop();

                if (istProgrammstart)
                {
                    if (Einstellung_helper.BesorgeEinstellung(Einstellung.Enum_AuswahlEinstellung_bool.ZeigeAutoimportBenachrichtigung) == true)
                    {
                        ZeigeBenachrichtigung();
                    }
                }
                else
                {
                    ZeigeBenachrichtigung();
                }

                StarteMethodenende();
            }
            else
            {
                if (istProgrammstart == false)
                {
                    WinApp_helper.Erstelle_Fehlermeldung("Es wurden keine neuen Messwerte importiert, weil keine neuen Messwerte gefunden wurden.", Enum_FehlerSchweregrad.Hinweis, Enum_Fehlergruppe.Autoimport);
                }
                StarteMethodenende();
            }

            //if (callback != null)
            //{
            //    callback.Invoke();
            //}

            void StarteMethodenende()
            {
                if (SplashScreenManager.Default != null && SplashScreenManager.Default.IsSplashFormVisible)
                    SplashScreenManager.CloseDefaultWaitForm();
            }
            void ZeigeBenachrichtigung()
            {
                long benoetigteZeit = stopwatch.ElapsedMilliseconds;
                double benoetigteZeit_s = Math.Round((benoetigteZeit + 0.0) / 1000, 2);
                antwortliste_importierteDatensaetze.Add($"Der Importvorgang hat {benoetigteZeit_s} Sekunden gedauert.");
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string line in antwortliste_importierteDatensaetze)
                {
                    stringBuilder.Append(line);
                    stringBuilder.Append("\r\n");
                }
                //stringBuilder.Remove(stringBuilder.Length - 5, 4); //Um das letzte "\r\n" zu entfernen
                MessageBox.Show(stringBuilder.ToString());
            }
        }

        private enum Enum_DatenimportErfolgreich
        {
            MesswertImportiert,
            KeinMesswertImportiert,
            Fehler
        }

        private static Enum_DatenimportErfolgreich Importiere_Werte(PV_Modul modul, DateTime dateTime_start, DateTime dateTime_ende, Enum_Autoimport enum_Autoimport, IObjectSpace os)
        {
            string start = Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_start); //Anfang des Jahres
            string ende = Sql_helper.KonvertiereZu_mysqlDatetime(dateTime_ende); //Ende des Jahres (1 milliSek vor Jahreswechsel)

            #region Maximale Stromproduktion
            string query_minWatt = $"select min(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_minWatt = Sql_helper.Query_select(query_minWatt);
            double maximale_stromproduktion = 0;

            if (result_minWatt != null)
            {
                var typetest = result_minWatt[0][0].GetType();

                try
                {
                    if (result_minWatt[0][0].GetType() != typeof(System.DBNull)) //nur Wert setzen, wenn es ein SQL Ergebnis gibt
                    {
                        maximale_stromproduktion = (double)result_minWatt[0][0];
                    }
                    else
                    {
                        return Enum_DatenimportErfolgreich.KeinMesswertImportiert;
                    }
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Die maximale Stromproduktion konnte nicht ermittelt werden. Grund:\r\nFehler beim parsen des Wertes '{result_minWatt[0][0]}' zu dem Typ 'double' in der Methode '{nameof(Importiere_Werte)}'.\r\n Query:\r\n  {query_minWatt}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return Enum_DatenimportErfolgreich.Fehler;
                }
            }
            #endregion Maximale Stromproduktion

            #region Durchnittliche Stromproduktion
            string query_avgWatt = $"select avg(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_avgWatt = Sql_helper.Query_select(query_avgWatt);
            double avg_stromproduktion = 0;

            if (result_avgWatt != null)
            {
                var typetest = result_avgWatt[0][0].GetType();

                try
                {
                    if (result_avgWatt[0][0].GetType() != typeof(System.DBNull)) //nur Wert setzen, wenn es ein SQL Ergebnis gibt
                    {
                        avg_stromproduktion = (double)result_avgWatt[0][0];
                    }
                    else
                    {
                        return Enum_DatenimportErfolgreich.KeinMesswertImportiert;
                    }
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Die durchnittliche Stromproduktion konnte nicht ermittelt werden. Grund:\r\nFehler beim parsen des Wertes '{result_avgWatt[0][0]}' zu dem Typ 'double' in der Methode '{nameof(Importiere_Werte)}'.\r\n Query:\r\n  {query_avgWatt}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return Enum_DatenimportErfolgreich.Fehler;
                }
            }
            #endregion Durchnittliche Stromproduktion

            #region Anzahl Messwerte
            string query_anzahlMesswerte = $"select count(val) from ts_number where id = {modul.DatenbankID_Leistung} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_anzahlMesswerte = Sql_helper.Query_select(query_anzahlMesswerte);
            long anzahlMesswerte = 0;
            if (result_anzahlMesswerte != null)
            {
                try
                {
                    if (result_anzahlMesswerte[0][0].GetType() != typeof(System.DBNull)) //nur Wert setzen, wenn es ein SQL Ergebnis gibt
                    {
                        anzahlMesswerte = (long)result_anzahlMesswerte[0][0];
                    }
                    else
                    {
                        return Enum_DatenimportErfolgreich.KeinMesswertImportiert;
                    }
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Die Anzahl der Messwerte konnte nicht ermittelt werden. Grund:\r\nFehler beim parsen des Wertes '{result_anzahlMesswerte[0][0]}' zu dem Typ 'int' in der Methode '{nameof(Importiere_Werte)}'.\r\n Query:\r\n  {query_anzahlMesswerte}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return Enum_DatenimportErfolgreich.Fehler;
                }
            }
            #endregion Anzahl Messwerte

            #region Export kWh
            string query_min_kWh = $"select min(val) from ts_number where id = {modul.DatenbankID_kWh} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_min_kWh = Sql_helper.Query_select(query_min_kWh);
            double min_kWh = 0;

            if (result_min_kWh != null)
            {
                try
                {
                    if (result_min_kWh[0][0].GetType() != typeof(System.DBNull)) //nur Wert setzen, wenn es ein SQL Ergebnis gibt
                    {
                        min_kWh = (double)result_min_kWh[0][0];
                    }
                    else
                    {
                        return Enum_DatenimportErfolgreich.KeinMesswertImportiert;
                    }
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Die Höhe der minimale Export kWh konnte nicht ermittelt werden. Grund:\r\nFehler beim parsen des Wertes '{result_anzahlMesswerte[0][0]}' zu dem Typ 'double' in der Methode '{nameof(Importiere_Werte)}'.\r\n Query:\r\n  {query_anzahlMesswerte}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return Enum_DatenimportErfolgreich.Fehler;
                }
            }

            string query_max_kWh = $"select max(val) from ts_number where id = {modul.DatenbankID_kWh} and ts > {start} and ts < {ende} and val > -10000 and q = 0";
            List<object[]> result_max_kWh = Sql_helper.Query_select(query_max_kWh);
            double max_kWh = 0;

            if (result_max_kWh != null)
            {
                try
                {
                    if (result_max_kWh[0][0].GetType() != typeof(System.DBNull)) //nur Wert setzen, wenn es ein SQL Ergebnis gibt
                    {
                        max_kWh = (double)result_max_kWh[0][0];
                    }
                    else
                    {
                        return Enum_DatenimportErfolgreich.KeinMesswertImportiert;
                    }
                }
                catch (Exception e)
                {
                    string fehlermeldung = $"Die Höhe der maximalen Export kWh konnte nicht ermittelt werden. Grund:\r\nFehler beim parsen des Wertes '{result_anzahlMesswerte[0][0]}' zu dem Typ 'double' in der Methode '{nameof(Importiere_Werte)}'.\r\n Query:\r\n  {query_anzahlMesswerte}\r\n  Stacktrace: \r\n'" + e;
                    WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Typkonvertierung);
                    return Enum_DatenimportErfolgreich.Fehler;
                }
            }
            #endregion Export kWh

            #region Erstellen des Objektes

            Messwert messwert = os.CreateObject<Messwert>();
            messwert.DatumVon = dateTime_start;
            messwert.DatumBis = dateTime_ende;
            messwert.Watt = maximale_stromproduktion;
            messwert.DurchschnittWatt = avg_stromproduktion;
            messwert.AnzahlMesswerteX = anzahlMesswerte;
            messwert.ExportKwH = max_kWh - min_kWh;
            messwert.PV_Modul = modul;
            messwert.AutoimportX = enum_Autoimport;

            #endregion Erstellen des Objektes
            return Enum_DatenimportErfolgreich.MesswertImportiert;
        }
    }
}
