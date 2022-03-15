using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Win.Helper
{
    public static class Sql_helper
    {
        public static MySqlConnection Connection { get; set; }

        public static string KonvertiereZu_mysqlDatetime(DateTime dateTime)
        {
            string mysqlDatetime = (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
            return mysqlDatetime;
        }

        public static DateTime KonvertiereZu_Datetime(double millisekunden)
        {
            DateTime ausgabe = new DateTime(1970, 1, 1).AddHours(1).AddMilliseconds(millisekunden);
            return ausgabe;
        }
    }
}
