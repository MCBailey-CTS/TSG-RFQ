using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class MarkMessageViewed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This will simply mark the message as viewed or unmark it depending on the value sent
            // messageid is the key field of the table
            // ck is either a zero or a one
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            int messageID = 0;
            try
            {
                messageID = System.Convert.ToInt32(Request["messageid"]);
            }
            catch  {

            }
            int ckval = 0;
            try
            {
                ckval = System.Convert.ToInt32(Request["ck"]);
            }
            catch 
            {

            }
            if (ckval == 0)
            {
                sql.CommandText = "update tblMessage set msgViewed = null where msgMessageID=@msg";

            }
            else
            {
                sql.CommandText = "update tblMessage set msgViewed =current_timestamp where msgMessageID=@msg";

            }
            sql.Parameters.AddWithValue("@msg", messageID);
            master.ExecuteNonQuery(sql, "MarkMessageViewed");
            connection.Close();
        }
    }
}