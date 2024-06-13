using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class ConnectionString
{
    public static string CONNECTION_STRING = $"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};" +
                                             $"Port={Environment.GetEnvironmentVariable("DATABASE_PORT")};" +
                                             $"Username={Environment.GetEnvironmentVariable("DATABASE_USERNAME")};" +
                                             $"Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")};" +
                                             $"Database={Environment.GetEnvironmentVariable("DATABASE_NAME")};";
    //$"sslmode=Require;" +
    //$"Trust Server Certificate=true;";
}

