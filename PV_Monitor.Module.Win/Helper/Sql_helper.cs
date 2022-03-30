using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PV_Monitor.Module.BusinessObjects;
using System.Windows.Forms;
using PV_Monitor.Module.Win.Helper;
using System.Data;

namespace PV_Monitor.Module.Win.Helper
{
    public static class Sql_helper
    {
        public static MySqlConnection Connection { get; set; }

        public static string KonvertiereZu_mysqlDatetime(DateTime dateTime)
        {
            DateTime relevantesDatum = dateTime;
            if (relevantesDatum.IsDaylightSavingTime() == true) //Sommerzeit
            {
                relevantesDatum = relevantesDatum.AddHours(-2);
            }
            else
            {
                relevantesDatum = relevantesDatum.AddHours(-1); //Winterzeit
            }
            string mysqlDatetime = (relevantesDatum - new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
            return mysqlDatetime;
        }

        public static DateTime KonvertiereZu_Datetime(long millisekunden)
        {
            DateTime ausgabe = new DateTime(1970, 1, 1).AddMilliseconds(millisekunden);
            if (ausgabe.IsDaylightSavingTime() == true)
            {
                ausgabe = ausgabe.AddHours(2); //Sommerzeit
            }
            else
            {
                ausgabe = ausgabe.AddHours(1);  //Winterzeit
            }
            return ausgabe;
        }

        /// <summary>
        /// Gibt eine List<object[] zurück mit so vielen Plätzen im Array wie Spalten des Query Ergebnisses
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<object[]> Query_select (string query)
        {
            MySqlConnection mySqlConnection = Connection;
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            List<object[]> object_list = new List<object[]>();
            bool fehler = false;

            try
            {
                mySqlConnection.Open();
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                int anzahl_columns = mySqlDataReader.FieldCount;

                //if (export_schema)
                //{
                //    DataTable schema = mySqlDataReader.GetSchemaTable();
                //    object_list.Add(schema);

                //    object[] line_types = new object[anzahl_columns];
                //    for (int i = 0; i < anzahl_columns; i++)
                //    {
                //        line_types[i] = mySqlDataReader.GetDataTypeName(i);
                //    }
                //    object_list.Add(line_types);
                //}

                while (mySqlDataReader.Read())
                {
                    object[] line = new object[anzahl_columns];
                    for (int i = 0; i < anzahl_columns; i++)
                    {
                        line[i] = mySqlDataReader.GetValue(i);
                    }
                    object_list.Add(line);
                }
            }
            catch (Exception e)
            {
                fehler = true;
                string fehlermeldung = $"Fehler beim ausführen der Query '{query}. Stacktrace: \r\n" + e;
                WinApp_helper.Erstelle_Fehlermeldung(fehlermeldung, Enum_FehlerSchweregrad.Mittel, Enum_Fehlergruppe.Datenbankzugriff);
            }
            finally
            {
                mySqlConnection.Close();
            }

            if (fehler)
            {
                return null;
            }

            return object_list;
        }

        public static DateTime Besorge_datumErsterMesswert(PV_Modul modul) //Besorgt aus der Datenbank das Datum des ersten Messwertes
        {
            string query = $"select min(ts) from ts_number where id = {modul.DatenbankID_Leistung} and q = 0";
            MySqlConnection mySqlConnection = Connection;
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            long millisek = 0;

            try
            {
                mySqlConnection.Open();
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    string line = mySqlDataReader.GetValue(0).ToString();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }

                    if (long.TryParse(line, out millisek) == false)
                    { 
                        MessageBox.Show($"Fehler beim Importieren des Wertes '{line}'");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Fehler beim Auslesen der Messwerte aus der Mysql Datenbank. Stacktrace: \r\n" + e);
            }
            finally
            {
                mySqlConnection.Close();
            }

            if (millisek == 0) //Fehler
            {
                return DateTime.MinValue;
            }
            else
            {
                DateTime dateTime = KonvertiereZu_Datetime(millisek);
                return dateTime; //Erwarteter Wert
            }
        }
    }
}
