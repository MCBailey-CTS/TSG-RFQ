using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RFQ
{
    public partial class processNotifyConfiguration : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection conn = new SqlConnection(master.getConnectionString());
            conn.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = conn;
            string id = Request["id"];
            string[] idlist=id.Split(',');
            int i = 0;
            // delete existing setup so you can set it how they sent it.
            // otherwise, you would have to check for each one before inserting
            sql.CommandText = "delete from tblUserNotificationReasons where unrUserID=@user";
            sql.Parameters.AddWithValue("@user", master.getUserID() );
            master.ExecuteNonQuery(sql, "processNotifyConfiguration");
            
            while (i < idlist.Count())
            {
                sql.Parameters.Clear();
                sql.CommandText = "insert into tblUserNotificationReasons (unrUserID, unrReasonID) values (@user, @reason) ";
                sql.Parameters.AddWithValue("@user", master.getUserID());
                sql.Parameters.AddWithValue("@reason", idlist[i]);
                master.ExecuteNonQuery(sql, "processNotifyConfiguration");
                i++;
            }
            sql.Parameters.Clear();
            sql.CommandText = "delete from tblUserNotification where unoUID=@user";
            sql.Parameters.AddWithValue("@user", master.getUserID());
            master.ExecuteNonQuery(sql, "processNotifyConfiguration");
            if (Request["email"] == "1")
            {
                sql.Parameters.Clear();
                sql.CommandText = "insert into tblUserNotification (unoUserNotificationTypeID, unoUID ) values (1, @user) ";
                sql.Parameters.AddWithValue("@user", master.getUserID());
                master.ExecuteNonQuery(sql, "processNotifyConfiguration");
            }
            if (Request["messaging"] == "1")
            {
                sql.Parameters.Clear();
                sql.CommandText = "insert into tblUserNotification (unoUserNotificationTypeID, unoUID) values (2, @user) ";
                sql.Parameters.AddWithValue("@user", master.getUserID());
                master.ExecuteNonQuery(sql, "processNotifyConfiguration");
            }
            if (Request["texting"] == "1")
            {
                sql.Parameters.Clear();
                sql.CommandText = "insert into tblUserNotification (unoUserNotificationTypeID, unoUID) values (3, @user) ";
                sql.Parameters.AddWithValue("@user", master.getUserID());
                master.ExecuteNonQuery(sql, "processNotifyConfiguration");
            }
            conn.Close();
        }
    }
}