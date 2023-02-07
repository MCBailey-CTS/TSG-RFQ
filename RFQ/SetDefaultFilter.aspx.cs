using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class SetDefaultFilter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String filter = Request["filter"];
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "update permissions set perDefaultFilterID=@filter where UID=@user";
            sql.Parameters.AddWithValue("@filter", filter);
            sql.Parameters.AddWithValue("@user", master.getUserID());
            sql.ExecuteNonQuery();
            connection.Close();
        }
    }
}