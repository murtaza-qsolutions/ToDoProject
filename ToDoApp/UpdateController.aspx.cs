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

                Response.Write("{\"status\":\"success\"}");
            }
            catch (Exception ex)
            {
                Response.Write("{\"status\":\"error\",\"msg\":\"" + ex.Message + "\"}");
            }
        }
    }
}