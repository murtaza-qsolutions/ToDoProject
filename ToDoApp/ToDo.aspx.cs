using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToDoApp
{
    [System.Web.Script.Services.ScriptService]
    public partial class ToDo : System.Web.UI.Page
    {
        public static bool IsTestMode { get; set; } = false;
        Database db;

        protected void Page_Load(object sender, EventArgs e)
        {
            db = new DatabaseProviderFactory().Create("ToDoDB");

            if (!IsPostBack)
            {
                BindItems(1); // Always use ListId = 1
            }
        }

        public string SafeColor(object dbValue)
        {
            string s = Convert.ToString(dbValue)?.Trim();

            if (string.IsNullOrEmpty(s))
                return "#6ec6d3";

            // Hex check
            if (Regex.IsMatch(s, "^#([0-9a-fA-F]{6})$"))
                return s.ToUpperInvariant();

            try
            {
                var c = Color.FromName(s);
                if (c.IsKnownColor || c.R + c.G + c.B > 0)
                {
                    return $"#{c.R:X2}{c.G:X2}{c.B:X2}";
                }
            }
            catch { }

            return "#6ec6d3";
        }
        private void BindItems(int listId)
        {
            try
            {
                string query = @"SELECT ItemId, ItemText, IsDone, ItemColor, DisplayOrder 
                         FROM ToDoItems WHERE ListId=@ListId 
                         ORDER BY DisplayOrder";

                using (DbCommand cmd = db.GetSqlStringCommand(query))
                {
                    db.AddInParameter(cmd, "@ListId", DbType.Int32, listId);

                    DataSet ds = db.ExecuteDataSet(cmd);
                    rptItems.DataSource = ds.Tables[0];
                    rptItems.DataBind();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "BindItems Failed");
                lblError.Text = "An error occurred while loading tasks.";
            }
        }
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                lblError.Text = "";

                string text = txtNewItem.Text?.Trim();

                if (string.IsNullOrWhiteSpace(text))
                {
                    lblError.Text = "Enter a task before adding.";
                    return;
                }

                if (IsTestMode) 
                {
                    if (hfIsEdit.Value == "true")
                    {
                        // simulate update
                        hfIsEdit.Value = "false";
                        hfEditItemId.Value = "";
                        btnAddItem.Text = "Add";
                    }
                    else
                    {
                        
                    }

                    txtNewItem.Text = ""; 
                    return;
                }

                if (hfIsEdit.Value == "true" && int.TryParse(hfEditItemId.Value, out int itemId))
                {
                    // UPDATE
                    using (DbCommand cmd = db.GetSqlStringCommand("UPDATE ToDoItems SET ItemText=@text WHERE ItemId=@id"))
                    {
                        db.AddInParameter(cmd, "@text", DbType.String, text);
                        db.AddInParameter(cmd, "@id", DbType.Int32, itemId);
                        db.ExecuteNonQuery(cmd);
                    }

                    hfIsEdit.Value = "false";
                    hfEditItemId.Value = "";
                    btnAddItem.Text = "Add";
                }
                else
                {
                    // INSERT
                    int newOrder;
                    using (DbCommand getMax = db.GetSqlStringCommand("SELECT ISNULL(MAX(DisplayOrder),0)+1 FROM ToDoItems WHERE ListId=1"))
                    {
                        object result = db.ExecuteScalar(getMax);
                        newOrder = Convert.ToInt32(result);
                    }

                    using (DbCommand cmd = db.GetSqlStringCommand("INSERT INTO ToDoItems (ListId, ItemText, DisplayOrder) VALUES (1, @ItemText, @Order)"))
                    {
                        db.AddInParameter(cmd, "@ItemText", DbType.String, text);
                        db.AddInParameter(cmd, "@Order", DbType.Int32, newOrder);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                txtNewItem.Text = ""; // ✅ clear only after success
                BindItems(1);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "btnAddItem_Click Failed");
                lblError.Text = "An error occurred while loading tasks.";
            }
        }



        // -------------------------------
        // DRAG-AND-DROP ORDER UPDATE
        // -------------------------------
        public class OrderUpdate
        {
            public int id { get; set; }
            public int pos { get; set; }
        }


        // -------------------------------
        // TOGGLE DONE / CHANGE COLOR / DELETE
        // -------------------------------
        protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {

           
            if (e.CommandName == "ToggleDone")
            {
                int itemId = Convert.ToInt32(e.CommandArgument);

                using (DbCommand cmd = db.GetSqlStringCommand(
                    @"UPDATE ToDoItems 
                      SET IsDone = 1
                      WHERE ItemId = @id AND IsDone = 0"))
                {
                    db.AddInParameter(cmd, "@id", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }

                BindItems(1);
            }
            else if (e.CommandName == "ChangeColor")
            {
                int itemId = Convert.ToInt32(e.CommandArgument);
                HiddenField hfColor = (HiddenField)e.Item.FindControl("hfItemColor");
                string newColor = hfColor.Value;

                using (DbCommand cmd = db.GetSqlStringCommand("UPDATE ToDoItems SET ItemColor=@color WHERE ItemId=@id"))
                {
                    db.AddInParameter(cmd, "@color", DbType.String, newColor);
                    db.AddInParameter(cmd, "@id", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }

                BindItems(1);
            }
            else if (e.CommandName == "DeleteItem")
            {
                int itemId = Convert.ToInt32(e.CommandArgument);

                using (DbCommand cmd = db.GetSqlStringCommand("DELETE FROM ToDoItems WHERE ItemId=@id"))
                {
                    db.AddInParameter(cmd, "@id", DbType.Int32, itemId);
                    db.ExecuteNonQuery(cmd);
                }

                BindItems(1);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "BindItems Failed");
                lblError.Text = "An error occurred while loading tasks.";
            }
        }

        // -------------------------------
        // EDIT MODE (from double click)
        // -------------------------------
        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            try
            {
            
            base.RaisePostBackEvent(sourceControl, eventArgument);

            string eventTarget = Request["__EVENTTARGET"];
            string eventArg = Request["__EVENTARGUMENT"];

            if (eventTarget == "EditItem")
            {
                int itemId = Convert.ToInt32(eventArg);

                using (DbCommand cmd = db.GetSqlStringCommand("SELECT ItemText FROM ToDoItems WHERE ItemId=@id"))
                {
                    db.AddInParameter(cmd, "@id", DbType.Int32, itemId);
                    object result = db.ExecuteScalar(cmd);

                    txtNewItem.Text = Convert.ToString(result);
                    hfEditItemId.Value = itemId.ToString();
                    hfIsEdit.Value = "true";
                    btnAddItem.Text = "Update";
                }
            }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "BindItems Failed");
                lblError.Text = "An error occurred while loading tasks.";
            }
        }

        // -------------------------------
        // BUTTON ORDER UPDATE
        // -------------------------------
        protected void btnUpdateOrder_Click(object sender, EventArgs e)
        {
            try { 
            string newOrderCsv = hfOrderData.Value;
            if (!string.IsNullOrEmpty(newOrderCsv))
            {
                string[] ids = newOrderCsv.Split(',');
                int pos = 1;
                foreach (string idStr in ids)
                {
                    if (int.TryParse(idStr, out int id))
                    {
                        using (DbCommand cmd = db.GetSqlStringCommand("UPDATE ToDoItems SET DisplayOrder=@pos WHERE ItemId=@id"))
                        {
                            db.AddInParameter(cmd, "@pos", DbType.Int32, pos++);
                            db.AddInParameter(cmd, "@id", DbType.Int32, id);
                            db.ExecuteNonQuery(cmd);
                        }
                    }
                }
                BindItems(1);
            }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "BindItems Failed");
                lblError.Text = "An error occurred while loading tasks.";
            }
        }
    }
    public static class ExceptionLogger
    {
        private static readonly LogWriter logWriter;

        static ExceptionLogger()
        {
            var config = ConfigurationSourceFactory.Create();
            logWriter = new LogWriterFactory(config).Create();
            Logger.SetLogWriter(logWriter);
        }

        public static void Log(Exception ex, string title = "Application Error")
        {
            var entry = new LogEntry
            {
                Message = ex.ToString(),
                Severity = System.Diagnostics.TraceEventType.Error,
                Title = title,
                Categories = { "General" }
            };
            Logger.Write(entry);
        }
    }
}
