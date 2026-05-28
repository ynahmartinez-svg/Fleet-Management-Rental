using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Fleet_Management_Rental
{
    public static class DbConfig
    {
        public static string ConnectionString =
            "Host=fmsrental-26507.j77.aws-ap-southeast-1.cockroachlabs.cloud;" +  // ✅ updated host
            "Port=26257;" +
            "Database=fms_rental;" +
            "Username=stephen;" +
            "Password=jQPj8FQl2JF4afGOR37QxQ;" +  // ✅ updated password
            "SSL Mode=VerifyFull;" +
            "Trust Server Certificate=true";
    }

    public static class DbHelper
    {
        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(DbConfig.ConnectionString);
        }
    }
}
