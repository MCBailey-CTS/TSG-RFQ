using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class DeleteRecord : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string table = "";
            string column = "";
            string value = "";
            try
            {
                table = Request["table"].ToString();
            } catch { } 
            try
            {
                column = Request["key"].ToString();
            } catch { }
            try
            {
                value = Request["id"].ToString();
            } catch { }

            sql.CommandText = "Delete from " + table + " where " + column + " = @value";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@table", table);
            sql.Parameters.AddWithValue("@column", column);
            sql.Parameters.AddWithValue("@value", value);
            master.ExecuteNonQuery(sql, "Delete Record");

            connection.Close();
        }
    }
}