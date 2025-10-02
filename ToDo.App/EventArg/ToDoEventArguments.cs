using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.App.Model;
using ToDo.DAL.ObjectModel;

namespace ToDo.App.EventArg
{
    public class ToDoEventArguments
    {
        public int ItemId { get; set; }
        public int ListId { get; set; }
        public string ItemText { get; set; }
        public string ItemColor { get; set; }
        public bool IsDone { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ToDoItem> Items { get; set; }

    }
}
