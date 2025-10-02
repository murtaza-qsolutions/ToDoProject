using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.DAL.DAO;
using ToDo.DAL.ObjectModel;

namespace ToDo.App.Model
{
    public class ToDoItemModel
    {
        public ToDoItemModel() { }

        public int Create(ToDoItem item)
        {
            ToDoActions toDoActions = new ToDoActions();
            return toDoActions.Create(item);
        }
        public ToDoItem Get(int id)
        {
            ToDoActions toDoActions = new ToDoActions();
            return toDoActions.Get(id);
        }
        public IEnumerable<ToDoItem> GetAll()
        {
            ToDoActions toDoActions = new ToDoActions();
            return toDoActions.GetAll();
        }
        public void Update(ToDoItem item)
        {
            ToDoActions toDoActions = new ToDoActions();
           toDoActions.Update(item);
        }
        public void Delete(int id)
        {
            ToDoActions toDoActions = new ToDoActions();
            toDoActions.Delete(id);
        }
        public void ColorChange(ToDoItem item)
        {
            ToDoActions toDoActions = new ToDoActions();
            toDoActions.ColorChange(item);
        }
        public void DisplayOrderChange(ToDoItem item)
        {
            ToDoActions toDoActions = new ToDoActions();
            toDoActions.DisplayOrderChange(item);
        }
        public void IsDoneChange(ToDoItem item)
        {
            ToDoActions toDoActions = new ToDoActions();
            toDoActions.IsDoneChange(item);
        }
    }
}
