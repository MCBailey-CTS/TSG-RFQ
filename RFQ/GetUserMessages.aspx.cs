using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class GetUserMessages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            master.setGlobalVariables();
            sql.CommandText = "select msgMessage, msgMessageID, msgSent from tblMessage where msgActiveMessage=1 and msgUID=@user and msgViewed is null order by msgSent";
            sql.Parameters.AddWithValue("@user", master.UserID);
            SqlDataReader dr = sql.ExecuteReader();
            litMessages.Text = "<table class='table table-striped col-sm-11' ><thead><tr><th>Message</th><th>When Sent</th><th>Viewed</th></tr>";
            while (dr.Read())
            {
                litMessages.Text += "<tr><td valign='top'>" + dr.GetValue(0).ToString() + "</td>";
                litMessages.Text += "<td valign='top'>" + dr.GetDateTime(2).ToString("d") + "</td>";
                litMessages.Text += "<td valign='top'><input type='checkbox' name='viewed' onclick='setMessageViewed(this.value,this.checked);' value='" + dr.GetValue(1).ToString() + "'></td></tr>";
            }
            litMessages.Text += "<p>Check All </ p><input type='checkbox' id='cbCheckAll' onclick=\"$(\'#btnCheckAll\').click();\" />";
            dr.Close();
            connection.Close();
            litMessages.Text += "</table>";
        }
    }
}