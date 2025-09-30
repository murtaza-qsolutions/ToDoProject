using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToDoApp
{
    public partial class UpdateController : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            try
            {
                var body = new StreamReader(Request.InputStream).ReadToEnd();
                var serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, string>>(body);

                int itemId = int.Parse(data["itemId"]);
                string color = data["color"];
                //int itemId = Convert.ToInt32(Request.Form["itemId"]);
                //string color = Request.Form["color"];

                // ---- Update DB here ----
                // Example:
                // using (SqlConnection con = new SqlConnection("YourConnectionString"))
                // {
                //     con.Open();
                //     var cmd = new SqlCommand("UPDATE YourTable SET Color=@color WHERE ItemId=@id", con);
                //     cmd.Parameters.AddWithValue("@color", color);
                //     cmd.Parameters.AddWithValue("@id", itemId);
                //     cmd.ExecuteNonQuery();
                // }

                Response.Write("{\"status\":\"success\"}");
            }
            catch (Exception ex)
            {
                Response.Write("{\"status\":\"error\",\"msg\":\"" + ex.Message + "\"}");
            }
        }
    }
}