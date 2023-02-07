using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class Admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           // todo - get their security level
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME  like 'pktbl%' or TABLE_NAME in ('Customer','tblUserNotification', 'CustomerLocation', 'CustomerContact')  order by TABLE_NAME ";
            SqlDataReader dr = sql.ExecuteReader();
            lblTasks.Text = "";
            lblTasks.Text += "<BR><BR>Click on a table name to see its contents.<BR><BR>";
            lblTasks.Text += "<table cellpadding='4' width='50%'>";
            int colcount = 0;
            while (dr.Read()) 
            {
                if (colcount == 0) 
                {
                    lblTasks.Text += "<tr>";
                }
                String TableName = dr.GetValue(0).ToString();
                // put a space between uppercase letters
                String Title = System.Text.RegularExpressions.Regex.Replace(TableName.Replace("TSG", "").Replace("pktbl",""), "[A-Z]", " $0").Trim();
                lblTasks.Text += "<td><a href='ListRecords?table=" + TableName + "'>" + Title + "</a></td>";
                colcount++;
                if (colcount > 2)
                {
                    lblTasks.Text += "</tr>";
                    colcount = 0;
                }
            }
            if (colcount > 0)
            {
                lblTasks.Text += "</tr>";
            }
            lblTasks.Text += "</table>";
            dr.Close();
            connection.Close();
        }
    }
}