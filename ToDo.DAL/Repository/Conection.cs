using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;

namespace ToDo.DAL.Repository
{
    public class Conection : IConection
    {
        public Database GetDatabase()
        {
            using (IUnityContainer container = new UnityContainer())
            {
                Database database;
                if (!container.IsRegistered<Database>())
                {
                    IConfigurationSource source = ConfigurationSourceFactory.Create();
                    DatabaseProviderFactory factory = new DatabaseProviderFactory(source);
                    database = factory.Create(EnvironmentAwareSettings.GetConnectionString());
                    container.RegisterType<Database>(new InjectionFactory(c => database));
                }
                else
                {
                    database = container.Resolve<Database>();
                }
                return database;
            }
        }
    }
}
