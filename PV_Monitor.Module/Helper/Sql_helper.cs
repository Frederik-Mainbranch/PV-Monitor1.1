using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV_Monitor.Module.Helper
{
    public static class Sql_helper
    {
        public static MySqlConnection Connection { get; set; }
    }
}
