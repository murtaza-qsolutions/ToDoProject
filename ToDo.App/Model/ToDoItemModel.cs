using System.Collections.Generic;
using ToDo.DAL.DAO;
using ToDo.DAL.ObjectModel;

namespace ToDo.App.Model
{
    public class ToDoItemModel
    {
        private readonly IToDoRepository _repository;

        public ToDoItemModel(IToDoRepository repository)
        {
            _repository = repository;
        }

        public int Create(ToDoItem item) => _repository.Create(item);
        public ToDoItem Get(int id) => _repository.Get(id);
        public IEnumerable<ToDoItem> GetAll() => _repository.GetAll();
        public void Update(ToDoItem item) => _repository.Update(item);
        public void Delete(int id) => _repository.Delete(id);
        public void ColorChange(ToDoItem item) => _repository.ColorChange(item);
        public void DisplayOrderChange(ToDoItem item) => _repository.DisplayOrderChange(item);
        public void IsDoneChange(ToDoItem item) => _repository.IsDoneChange(item);
    }
}
