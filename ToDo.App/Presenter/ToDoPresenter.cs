using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.App.EventArg;
using ToDo.App.Model;
using ToDo.App.View;
using ToDo.DAL.DAO;
using ToDo.DAL.ObjectModel;
using ToDo.DAL.Repository;

namespace ToDo.App.Presenter
{
    public class ToDoPresenter
    {
        private readonly IToDoView _view;
        private readonly ToDoItemModel _model;

        public ToDoPresenter(IToDoView view)
        {
            _view = view;
            _model = new ToDoItemModel(new ToDoRepository());

            _view.GetAll += new EventHandler<ToDoEventArguments>(GetAll);
            _view.GetAllByID += new EventHandler<ToDoEventArguments>(GetAllByID);
            _view.Insert += new EventHandler<ToDoEventArguments>(Insert);
            _view.Update += new EventHandler<ToDoEventArguments>(Update);
            _view.Delete += new EventHandler<ToDoEventArguments>(Delete);
            _view.ColorChange += new EventHandler<ToDoEventArguments>(ColorChange);
            _view.DisplayOrderChange += new EventHandler<ToDoEventArguments>(DisplayOrderChange);
            _view.IsDoneChanged += new EventHandler<ToDoEventArguments>(IsDoneChange);
        }

        private void GetAll(object sender, ToDoEventArguments e)
        {
            e.Items = _model.GetAll().ToList();
        }

        private void GetAllByID(object sender, ToDoEventArguments e)
        {
            var item = _model.Get(e.ListId);
            e.Items = item != null ? new List<ToDoItem> { item } : new List<ToDoItem>();
        }

        private void Insert(object sender, ToDoEventArguments e)
        {
            var item = BuildItem(e.ListId, e.ItemText, e.ItemColor, e.IsDone);
            _model.Create(item);
        }

        private void Update(object sender, ToDoEventArguments e)
        {
            var item = BuildItemForUpdate(e.ItemId, e.ItemText);
            _model.Update(item);
        }

        private void Delete(object sender, ToDoEventArguments e)
        {
            _model.Delete(e.ItemId);
        }

        private void ColorChange(object sender, ToDoEventArguments e)
        {
            var item = new ToDoItem { ItemId = e.ItemId, ItemColor = e.ItemColor };
            _model.ColorChange(item);
        }

        private void DisplayOrderChange(object sender, ToDoEventArguments e)
        {
            var item = new ToDoItem { ItemId = e.ItemId, DisplayOrder = e.DisplayOrder };
            _model.DisplayOrderChange(item);
        }

        private void IsDoneChange(object sender, ToDoEventArguments e)
        {
            var item = new ToDoItem { ItemId = e.ItemId, IsDone = e.IsDone };
            _model.IsDoneChange(item);
        }

        // helper builders instead of multiple setObject overloads
        private ToDoItem BuildItem(int listId, string text, string color, bool isDone) =>
            new ToDoItem
            {
                ListId = listId,
                ItemText = text,
                ItemColor = color,
                IsDone = isDone,
                CreatedAt = DateTime.Now
            };

        private ToDoItem BuildItemForUpdate(int itemId, string text) =>
            new ToDoItem
            {
                ItemId = itemId,
                ItemText = text,
                CreatedAt = DateTime.Now
            };
    }
}
