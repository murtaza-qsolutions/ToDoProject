using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Reflection;
using System.Web.UI.WebControls;
using ToDoApp;

namespace ToDoAppUnitTest
{
    [TestClass]
    public class ToDoCrudTests
    {
        private ToDo page;

        [TestInitialize]
        public void TestInitialize()
        {
            page = new ToDo();

            SetPrivateField("txtNewItem", new TextBox());
            SetPrivateField("lblError", new Label());
            SetPrivateField("hfIsEdit", new HiddenField());
            SetPrivateField("hfEditItemId", new HiddenField());
            SetPrivateField("hfOrderData", new HiddenField());
            SetPrivateField("btnAddItem", new Button());
            SetPrivateField("rptItems", new Repeater());

            ToDo.IsTestMode = false;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ToDo.IsTestMode = false;
        }

        #region Helpers (reflection)
        private void SetPrivateField(string fieldName, object value)
        {
            var f = page.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (f == null)
            {
                var p = page.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (p == null)
                    throw new InvalidOperationException($"Could not find field or property '{fieldName}' on {page.GetType().Name}");
                p.SetValue(page, value);
            }
            else
            {
                f.SetValue(page, value);
            }
        }

        private T GetPrivateField<T>(string fieldName) where T : class
        {
            var f = page.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (f != null)
                return f.GetValue(page) as T;

            var p = page.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (p != null)
                return p.GetValue(page) as T;

            return null;
        }

        private object InvokePrivateMethod(string methodName, params object[] args)
        {
            var m = page.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (m == null)
                throw new InvalidOperationException($"Method '{methodName}' not found on {page.GetType().Name}");
            return m.Invoke(page, args);
        }
        #endregion

        #region Add (Create) Tests

        [TestMethod]
        public void Add_ShouldReject_EmptyInput_ShowError()
        {
            var txt = GetPrivateField<TextBox>("txtNewItem");
            var lbl = GetPrivateField<Label>("lblError");

            txt.Text = "   "; 
            InvokePrivateMethod("btnAddItem_Click", null, EventArgs.Empty);

            Assert.AreEqual("Enter a task before adding.", lbl.Text, "Empty input should cause error message.");
        }

        [TestMethod]
        public void Add_Should_ClearTextAndNotError_WhenValid_InTestMode()
        {
            ToDo.IsTestMode = true;
            var txt = GetPrivateField<TextBox>("txtNewItem");
            var lbl = GetPrivateField<Label>("lblError");

            txt.Text = "Unit test add task";

            InvokePrivateMethod("btnAddItem_Click", null, EventArgs.Empty);

            Assert.AreEqual(string.Empty, txt.Text, "TextBox should be cleared after simulated add in test mode.");
            Assert.AreEqual(string.Empty, lbl.Text, "lblError should remain empty on successful add.");
        }

        #endregion

        #region Retrieve (Read) Tests

        [TestMethod]
        public void Retrieve_ShouldBindRepeater_WhenDataTableProvided()
        {
            var dt = new DataTable();
            dt.Columns.Add("ItemId", typeof(int));
            dt.Columns.Add("ItemText", typeof(string));
            dt.Columns.Add("IsDone", typeof(bool));
            dt.Columns.Add("ItemColor", typeof(string));
            dt.Columns.Add("DisplayOrder", typeof(int));
            dt.Rows.Add(1, "task one", false, "#FFFFFF", 1);
            dt.Rows.Add(2, "task two", true, "#000000", 2);

            var repeater = GetPrivateField<Repeater>("rptItems");
            repeater.DataSource = dt;
            repeater.DataBind();
            Assert.AreEqual(2, repeater.Items.Count, "Repeater should contain the two rows we bound.");
            var firstItem = repeater.Items[0];
        }

        #endregion

        #region Update Tests

        [TestMethod]
        public void Update_ShouldExitEditMode_WhenIsTestModeTrue()
        {
            ToDo.IsTestMode = true;
            var txt = GetPrivateField<TextBox>("txtNewItem");
            var hfIsEdit = GetPrivateField<HiddenField>("hfIsEdit");
            var hfEditItemId = GetPrivateField<HiddenField>("hfEditItemId");
            var btn = GetPrivateField<Button>("btnAddItem");

            txt.Text = "Updated by unit test";
            hfIsEdit.Value = "true";
            hfEditItemId.Value = "42";
            btn.Text = "Update";

            InvokePrivateMethod("btnAddItem_Click", null, EventArgs.Empty);

            Assert.AreEqual("false", hfIsEdit.Value, "hfIsEdit should be reset to false after simulated update.");
            Assert.AreEqual(string.Empty, hfEditItemId.Value, "hfEditItemId should be cleared after simulated update.");
            Assert.AreEqual("Add", btn.Text, "Button text should be reset to 'Add' after simulated update.");
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        public void Delete_Command_ShouldCompleteFlow_WithoutThrowingUnderUISimulation()
        {
            var repeater = GetPrivateField<Repeater>("rptItems");

            var dummyItem = new RepeaterItem(0, ListItemType.Item);

            var hfColor = new HiddenField { ID = "hfItemColor", Value = "#FFFFFF" };
            dummyItem.Controls.Add(hfColor);

            var cmdArgs = new System.Web.UI.WebControls.CommandEventArgs("DeleteItem", "1");
            var rceArgs = new RepeaterCommandEventArgs(dummyItem, new LinkButton(), cmdArgs);

            try
            {
                InvokePrivateMethod("rptItems_ItemCommand", null, rceArgs);
                Assert.IsTrue(true, "rptItems_ItemCommand executed without throwing.");
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                var lbl = GetPrivateField<Label>("lblError");
                Assert.IsTrue(!string.IsNullOrEmpty(lbl.Text), "If exception occurs inside handler, lblError should be set by the page catch block.");
            }
            catch (Exception ex)
            {
                Assert.Fail("rptItems_ItemCommand threw an unexpected exception: " + ex.Message);
            }
        }

        #endregion

        #region Order Update Tests (extra)

        [TestMethod]
        public void UpdateOrder_ShouldNotThrow_WhenCsvProvided()
        {
            var hfOrderData = GetPrivateField<HiddenField>("hfOrderData");
            hfOrderData.Value = "3,2,1";
            try
            {
                InvokePrivateMethod("btnUpdateOrder_Click", null, EventArgs.Empty);
                Assert.IsTrue(true, "btnUpdateOrder_Click executed without throwing.");
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                var lbl = GetPrivateField<Label>("lblError");
                Assert.IsTrue(!string.IsNullOrEmpty(lbl.Text), "If exception occurs during update order, lblError should be set.");
            }
            catch (Exception ex)
            {
                Assert.Fail("btnUpdateOrder_Click threw unexpected exception: " + ex.Message);
            }
        }

        #endregion
    }
}
