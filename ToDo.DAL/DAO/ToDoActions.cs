using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.DAL.ObjectModel;

namespace ToDo.DAL.DAO
{
    public class ToDoActions : IToDoRepository
    {
        private readonly Database _database;

        public ToDoActions()
        {
            try
            {
                _database = DatabaseFactory.CreateDatabase("ToDoDB");
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                {
                    message += " | Inner: " + ex.InnerException.Message;
                }
                throw new ApplicationException("Database initialization failed: " + message, ex);
            }
        }
        public int Create(ToDoItem item)
        {
            const string sql = "INSERT INTO ToDoItems (ListId, ItemText, ItemColor, IsDone, DisplayOrder, CreatedAt) " +
                               "VALUES (@ListId, @ItemText, @ItemColor, @IsDone," +
                               " (SELECT ISNULL(MAX(DisplayOrder),0)+1 FROM ToDoItems), @CreatedAt); " +
                               "SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ListId", System.Data.DbType.Int32, item.ListId);
            _database.AddInParameter(command, "@ItemText", System.Data.DbType.String, item.ItemText);
            _database.AddInParameter(command, "@ItemColor", System.Data.DbType.String, item.ItemColor);
            _database.AddInParameter(command, "@IsDone", System.Data.DbType.Boolean, item.IsDone);
            var safeDate = (item.CreatedAt < new DateTime(1753, 1, 1))
                    ? DateTime.Now
                    : item.CreatedAt;

            _database.AddInParameter(command, "@CreatedAt", DbType.DateTime, safeDate);
            var result = _database.ExecuteScalar(command);
            return (result != null) ? Convert.ToInt32(result) : 0;

        }

        public ToDoItem Get(int id)
        {
            const string sql = "SELECT ItemId, ListId, ItemText, ItemColor, IsDone, DisplayOrder, CreatedAt " +
                               "FROM ToDoItems WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, id);
            using (var rdr = _database.ExecuteReader(command))
            {
                if (rdr.Read())
                {
                    return Map(rdr);
                }
                return null;
            }
        }
        public List<ToDoItem> GetAll()
        {
            const string sql = "SELECT ItemId, ListId, ItemText, ItemColor, IsDone, DisplayOrder, CreatedAt " +
                               "FROM ToDoItems ORDER BY DisplayOrder";
            var command = _database.GetSqlStringCommand(sql);
            var items = new List<ToDoItem>();
            using (var rdr = _database.ExecuteReader(command))
            {
                while (rdr.Read())
                {
                    items.Add(Map(rdr));
                }
            }
            return items;
        }
        public void Update(ToDoItem item)
        {
            const string sql = "UPDATE ToDoItems SET ItemText = @ItemText WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ItemText", DbType.String, item.ItemText);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, item.ItemId);
            _database.ExecuteNonQuery(command);

        }
        public void Delete(int id)
        {
            const string sql = "DELETE FROM ToDoItems WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, id);
            _database.ExecuteNonQuery(command);
        }

        public void ColorChange(ToDoItem item)
        {
            const string sql = "UPDATE ToDoItems SET ItemColor = @ItemColor WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ItemColor", DbType.String, item.ItemColor);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, item.ItemId);
            _database.ExecuteNonQuery(command);
        }

        public void DisplayOrderChange(ToDoItem item)
        {
            const string sql = "UPDATE ToDoItems SET DisplayOrder = @DisplayOrder WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@DisplayOrder", DbType.Int32, item.DisplayOrder);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, item.ItemId);
            _database.ExecuteNonQuery(command);
        }
        public void IsDoneChange(ToDoItem item)
        {
            const string sql = "UPDATE ToDoItems SET IsDone = 1 WHERE ItemId = @ItemId";
            var command = _database.GetSqlStringCommand(sql);
            _database.AddInParameter(command, "@ItemId", DbType.Int32, item.ItemId);
            _database.ExecuteNonQuery(command);
        }   

        private ToDoItem Map(IDataReader rdr)
        {
            return new ToDoItem
            {
                ItemId = Convert.ToInt32(rdr["ItemId"]),
                ListId = 1,
                ItemText = rdr["ItemText"] as string,
                IsDone = Convert.ToBoolean(rdr["IsDone"]),
                CreatedAt = Convert.ToDateTime(rdr["CreatedAt"]),
                DisplayOrder = Convert.ToInt32(rdr["DisplayOrder"]),
                ItemColor = rdr["ItemColor"] as string
            };
        }
    }
}
