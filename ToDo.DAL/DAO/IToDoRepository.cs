using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.DAL.ObjectModel;

namespace ToDo.DAL.DAO
{
    public interface IToDoRepository
    {
        int Create(ToDoItem item);
        ToDoItem Get(int id);
        List<ToDoItem> GetAll();
        void Update(ToDoItem item);
        void Delete(int id);
        void ColorChange(ToDoItem item);
        void DisplayOrderChange(ToDoItem item);
        void IsDoneChange(ToDoItem item);
    }
}
