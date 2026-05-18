using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Fleet_Management_Rental
{
    public static class DbHelper
    {
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(DbConfig.ConnectionString);
        }
    }

    public static class DbConfig
    {
        public static string ConnectionString = "Host=smart1-fleetdb-25755.j77.aws-ap-southeast-1.cockroachlabs.cloud;" +
                "Port=26257;" +
                "Database=fms_rental;" +
                "Username=joohn;" +
                "Password=XANnoM1UEQoQ2IJ2-Jp5aw;" +
                "SSL Mode=VerifyFull;" +
                "Trust Server Certificate=true";
    }
}
