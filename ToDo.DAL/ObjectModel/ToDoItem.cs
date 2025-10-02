using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.DAL.ObjectModel
{
    public class ToDoItem
    {
        public int ItemId { get; set; }
        public int ListId { get; set; }
        public string ItemText { get; set; }
        public string ItemColor { get; set; }
        public bool IsDone { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
