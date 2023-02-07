using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class DeleteFilter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // How To Call
            // DeleteFilter.aspx?filter=0
            String filterID = Request["filter"];
            Site master = new Site();
            master.setGlobalVariables();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            Boolean TheyOwnIt = false;
            if (master.getUserRole() != 5)
            {
                sql.CommandText = "select fltFilterName from tblFilter where fltFilterID=@filter and  filCreatedBy=@user ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", filterID);
                sql.Parameters.AddWithValue("@user", master.getUserID());
                SqlDataReader dr = sql.ExecuteReader();
                TheyOwnIt = dr.HasRows;
                dr.Close();
            }
            else
            {
                TheyOwnIt = true;
            }
            if (TheyOwnIt)
            {
                sql.Parameters.Clear();
                sql.CommandText = "delete from pktblFilterCondition where fcnFilterID=@filter";
                sql.Parameters.AddWithValue("@filter", filterID);
                sql.ExecuteNonQuery();
                sql.CommandText = "delete from pktblFilterField where ffiFilterID=@filter";
                sql.ExecuteNonQuery();
                sql.CommandText = "delete from tblFilter where fltFilterID=@filter";
                sql.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}