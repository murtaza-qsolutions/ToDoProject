using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.App.EventArg;
using ToDo.App.Presenter;

namespace ToDo.App.View
{
    public interface IToDoView
    {   
        event EventHandler<ToDoEventArguments> GetAll;
        event EventHandler<ToDoEventArguments> GetAllByID;
        event EventHandler<ToDoEventArguments> Insert;
        event EventHandler<ToDoEventArguments> Update;
        event EventHandler<ToDoEventArguments> Delete;
        event EventHandler<ToDoEventArguments> ColorChange;
        event EventHandler<ToDoEventArguments> DisplayOrderChange;
        event EventHandler<ToDoEventArguments> IsDoneChanged;

        void AttachPresenter(ToDoPresenter presenter);
    }
}
