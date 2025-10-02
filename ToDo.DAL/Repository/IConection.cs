using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.DAL.Repository
{
    public interface IConection
    {
        Database GetDatabase();
    }
}
