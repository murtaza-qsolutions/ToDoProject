using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ToDo.DAL.Repository
{
    public static class EnvironmentAwareSettings
    {
        private static readonly string ConnectionStringName = ConfigurationManager.ConnectionStrings["ToDoDb"].ConnectionString;
        public static string GetConnectionString()
        {
            string DatabaseConnectionString = string.Empty;
            DatabaseConnectionString = ConnectionStringName;
            return DatabaseConnectionString;
        }
    }
}
