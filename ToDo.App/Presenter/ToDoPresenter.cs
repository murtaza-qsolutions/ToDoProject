using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.App.EventArg;
using ToDo.App.Model;
using ToDo.App.View;
using ToDo.DAL.ObjectModel;

namespace ToDo.App.Presenter
{
    public class ToDoPresenter
    {
        private readonly IToDoView _view;
        public ToDoPresenter(IToDoView view)
        {
            this._view = view;
            _view.GetAll += new EventHandler<ToDoEventArguments>(GetAll);
            _view.GetAllByID += new EventHandler<ToDoEventArguments>(GetAllByID);
            _view.Insert += new EventHandler<ToDoEventArguments>(Insert);
            _view.Update += new EventHandler<ToDoEventArguments>(Update);
            _view.Delete += new EventHandler<ToDoEventArguments>(Delete);
            _view.ColorChange += new EventHandler<ToDoEventArguments>(ColorChange);
            _view.DisplayOrderChange += new EventHandler<ToDoEventArguments>(DisplayOrderChange);
            _view.IsDoneChanged += new EventHandler<ToDoEventArguments>(IsDoneChange);
            //_view.AttachPresenter(this);
        }
        private void GetAll(object sender, ToDoEventArguments e)
        {
         ToDoItemModel model = new ToDoItemModel();
            var items = model.GetAll();
            e.Items = items.ToList();
        }
        private ToDoItem setObject(int listId,string title, string color, bool isDone)
        {
            ToDoItem item = new ToDoItem();
            item.ListId = listId;
            item.ItemText = title;
            item.ItemColor = color;
            item.IsDone = isDone;
            item.CreatedAt = DateTime.Now;
            return item;
        }
        private ToDoItem setObject(int ItemId, string title)
        {
            ToDoItem item = new ToDoItem();
            item.ItemId = ItemId;
            item.ItemText = title;
            item.CreatedAt = DateTime.Now;
            return item;
        }
        private ToDoItem setObject(int ItemId, bool IsDone)
        {
            ToDoItem item = new ToDoItem();
            item.ItemId = ItemId;
            item.IsDone = IsDone;
            return item;
        }
        //private ToDoItem setObject(int ItemId, int listId)
        //{
        //    ToDoItem item = new ToDoItem();
        //    item.ItemId = ItemId;
        //    item.ListId = listId;
        //    return item;
        //}
        private ToDoItem setDisplayOrder(int ItemId, int DisplayOrder)
        {
            ToDoItem item = new ToDoItem();
            item.ItemId = ItemId;
            item.DisplayOrder = DisplayOrder;
            return item;

        }
        private ToDoItem setColor(int ItemId, string color)
        {
            ToDoItem item = new ToDoItem();
            item.ItemId = ItemId;
            item.ItemColor = color;
            return item;
        }
        private void GetAllByID(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            var items = model.Get(e.ListId);
            if(items != null)
            {
                e.Items = new List<ToDoItem> { items };
            }
            else
            {
                e.Items = new List<ToDoItem> { items };
            }
        }
        private void Insert(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            ToDoItem item = setObject(e.ListId, e.ItemText, e.ItemColor, e.IsDone);
            model.Create(item);
        }
        private void Update(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            ToDoItem item = setObject(e.ItemId, e.ItemText);
            model.Update(item);
        }
        private void Delete(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            //ToDoItem item = setObject(e.ItemId, e.ListId);
            model.Delete(e.ItemId);
        }
        private void ColorChange(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            ToDoItem item = setColor(e.ItemId, e.ItemColor);
            model.ColorChange(item);
        }
        private void DisplayOrderChange(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            ToDoItem item = setDisplayOrder(e.ItemId, e.DisplayOrder);
            model.DisplayOrderChange(item);
        }
        public void IsDoneChange(object sender, ToDoEventArguments e)
        {
            ToDoItemModel model = new ToDoItemModel();
            ToDoItem item = setObject(e.ItemId, e.IsDone);
            model.IsDoneChange(item);
        }

    }
}
