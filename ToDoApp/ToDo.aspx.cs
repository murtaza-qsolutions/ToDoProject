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
using ToDo.App.EventArg;
using ToDo.App.Model;
using ToDo.App.Presenter;
using ToDo.App.View;
using ToDo.DAL.DAO;

namespace ToDoApp
{
    [System.Web.Script.Services.ScriptService]
    public partial class ToDo : System.Web.UI.Page, IToDoView
    {

        private ToDoPresenter presenter;

        public event EventHandler<ToDoEventArguments> GetAll;
        public event EventHandler<ToDoEventArguments> GetAllByID;
        public event EventHandler<ToDoEventArguments> Insert;
        public event EventHandler<ToDoEventArguments> Update;
        public event EventHandler<ToDoEventArguments> Delete;
        public event EventHandler<ToDoEventArguments> ColorChange;
        public event EventHandler<ToDoEventArguments> DisplayOrderChange;
        public event EventHandler<ToDoEventArguments> IsDoneChanged;
        public static bool IsTestMode { get; set; } = false;
        Database db;

        public void AttachPresenter(ToDoPresenter presenter)
        {
            this.presenter = presenter;
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            var repository = new ToDoActions();              // DAL implementation
            var model = new ToDoItemModel(repository);       // Model with abstraction
            presenter = new ToDoPresenter(this);      // Presenter with dependencies
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //presenter = new ToDoPresenter(this);

            if (!IsPostBack)
            {
                BindItems(); 
            }
        }

        public string SafeColor(object dbValue)
        {
            string s = Convert.ToString(dbValue)?.Trim();

            if (string.IsNullOrEmpty(s))
                return "#6ec6d3";
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
        private void BindItems()
        {
            try
            {
              if(GetAll != null)
                {
                    ToDoEventArguments args = new ToDoEventArguments();
                    GetAll(this, args);
                    rptItems.DataSource = args.Items;
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
                    
                        if (Update != null)
                        {
                            ToDoEventArguments args = new ToDoEventArguments
                            {
                                ItemId = itemId,
                                ItemText = text
                            };
                            Update(this, args);
                            BindItems();
                            txtNewItem.Text = "";
                        }
                    hfIsEdit.Value = "false";
                    hfEditItemId.Value = "";
                    btnAddItem.Text = "Add";


                }
                else
                {
                    // INSERT
                    if (Insert != null)
                    {
                        ToDoEventArguments args = new ToDoEventArguments
                        {
                            ItemText = text,
                            ItemColor = "#6ec6d3",
                            IsDone = false,
                            CreatedAt = DateTime.Now,
                            ListId = 1
                        };
                        Insert(this, args);
                        BindItems();
                        txtNewItem.Text = ""; 
                    }
                }

                txtNewItem.Text = ""; // ✅ clear only after success
                BindItems();
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

                    if (IsDoneChanged != null)
                    {
                        ToDoEventArguments args = new ToDoEventArguments
                        {
                            ItemId = itemId
                        };
                        IsDoneChanged(this, args);
                        BindItems();
                    }
                    BindItems();
            }
            else if (e.CommandName == "ChangeColor")
            {
                int itemId = Convert.ToInt32(e.CommandArgument);
                HiddenField hfColor = (HiddenField)e.Item.FindControl("hfItemColor");
                string newColor = hfColor.Value;

                    if (ColorChange != null)
                    {
                        ToDoEventArguments args = new ToDoEventArguments
                        {
                            ItemId = itemId,
                            ItemColor = newColor
                        };
                        ColorChange(this, args);
                        BindItems();
                    }

                BindItems();
            }
            else if (e.CommandName == "DeleteItem")
            {
                int itemId = Convert.ToInt32(e.CommandArgument);

                    if (Delete != null)
                    {
                        ToDoEventArguments args = new ToDoEventArguments
                        {
                            ItemId = itemId
                        };
                        Delete(this, args);
                        BindItems();

                    }

                BindItems();
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
                            if (DisplayOrderChange != null)
                            {
                                ToDoEventArguments args = new ToDoEventArguments
                                {
                                    ItemId = id,
                                    DisplayOrder = pos++
                                };
                                DisplayOrderChange(this, args);

                            }
                        }
                }
                BindItems();
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
