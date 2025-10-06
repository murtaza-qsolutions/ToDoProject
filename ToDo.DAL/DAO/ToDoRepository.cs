using System.Collections.Generic;
using ToDo.DAL.DAO;
using ToDo.DAL.ObjectModel;

namespace ToDo.DAL.Repository
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoActions _actions = new ToDoActions();

        public int Create(ToDoItem item) => _actions.Create(item);
        public ToDoItem Get(int id) => _actions.Get(id);
        public List<ToDoItem> GetAll() => _actions.GetAll();
        public void Update(ToDoItem item) => _actions.Update(item);
        public void Delete(int id) => _actions.Delete(id);
        public void ColorChange(ToDoItem item) => _actions.ColorChange(item);
        public void DisplayOrderChange(ToDoItem item) => _actions.DisplayOrderChange(item);
        public void IsDoneChange(ToDoItem item) => _actions.IsDoneChange(item);
    }
}

