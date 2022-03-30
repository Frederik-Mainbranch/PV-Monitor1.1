using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Archiv
{
    class Async_konvertierung
    {
        private void async_import()
        {
            //konvertierte_rohdaten_watt.Start();

            //Task<(DateTime uhrzeit, double wert)[]> konvertierte_rohdaten_kWh = new Task<(DateTime uhrzeit, double wert)[]>(() =>
            //{
            //    int anzahl = result_rohdaten_kWh.Count;
            //    (DateTime uhrzeit, double wert)[] konvertierteMesswerte = new (DateTime uhrzeit, double wert)[anzahl];
            //    for (int i = 0; i < anzahl - 1; i++)
            //    {
            //        konvertierteMesswerte[i] = new(Sql_helper.KonvertiereZu_Datetime((long)result_rohdaten_kWh[i][0]), (double)result_rohdaten_kWh[i][1]);
            //    }

            //    return konvertierteMesswerte;
            //});
            ////konvertierte_rohdaten_kWh.Start();

            //var taskliste = new List<Task<(DateTime uhrzeit, double wert)[]>>();
            //for (int i = 0; i < 1000; i++)
            //{
            //    Task<(DateTime uhrzeit, double wert)[]> konvertierte_rohdaten_watt = new Task<(DateTime uhrzeit, double wert)[]>(() =>
            //    {
            //        int anzahl = result_rohdaten_watt.Count;
            //        (DateTime uhrzeit, double wert)[] konvertierteMesswerte = new (DateTime uhrzeit, double wert)[anzahl];
            //        for (int i = 0; i < anzahl - 1; i++)
            //        {
            //            konvertierteMesswerte[i] = new(Sql_helper.KonvertiereZu_Datetime((long)result_rohdaten_watt[i][0]), (double)result_rohdaten_watt[i][1]);
            //        }

            //        return konvertierteMesswerte;
            //    });

            //    taskliste.Add(konvertierte_rohdaten_watt);
            //    konvertierte_rohdaten_watt.Start();
            //}




            ////konvertierte_rohdaten_watt.Start();
            ////taskliste.Add(konvertierte_rohdaten_kWh);
            ////konvertierte_rohdaten_kWh.Start();


            //await Task.WhenAll(taskliste);
        }
    }
}
