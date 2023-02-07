using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net;

namespace RFQ
{
    public partial class NotifyCompanyRFQ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // parameters
            // rfq - the rfq we are sending or re-sending about
            // company - company to notify - canb be list separted by commas
            // remove - 1 = remove this notification rather than send it
            // reallySend - used to re-send notifications
            string rfq = Request["rfq"];
            string companylist = Request["company"];
            string remove = Request["remove"];
            string reallySend = Request["really"];
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            SqlDataReader dr;
            foreach (String company in companylist.Split(','))
            {
                if (remove == "1")
                {
                    sql.CommandText = "Select rtqCompanyID from linkRFQToCompany where rtqRFQID = @rfq and rtqCompanyID = @co ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfq);
                    sql.Parameters.AddWithValue("@co", company);
                    dr = sql.ExecuteReader();
                    Boolean exists = false;
                    if(dr.Read())
                    {
                        exists = true;
                    }
                    dr.Close();

                    if(exists)
                    {
                        // remove the notification for this rfq and this company
                        sql.CommandText = "delete from linkRFQToCompany where rtqCompanyID = @co and rtqRFQID = @rfq ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@co", company);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        master.ExecuteNonQuery(sql, "NotifyCompanyRFQ");
                    }
                    else
                    {
                        sql.CommandText = "insert into linkRFQToCompany (rtqCompanyID, rtqRFQID, rtqCreated, rtqCreatedBy) ";
                        sql.CommandText += "values (@co, @rfq, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@co", company);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "Notify Company RFQ");
                    }
                    connection.Close();
                    return;
                }
                else
                {
                    // see if the notification was already sent.
                    // if not, add the link so we know it was sent, and set the reallySend flag so we send the notification no matter what
                    Boolean alreadyThere = false;
                    sql.CommandText = "select * from linkRFQToCompany where rtqCompanyID=@co and rtqRFQID=@rfq";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@co", company);
                    sql.Parameters.AddWithValue("@rfq", rfq);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        alreadyThere = true;
                    }
                    dr.Close();

                    if (!alreadyThere)
                    {
                        sql.CommandText = "insert into linkRFQToCompany (rtqCompanyID, rtqRFQID, rtqCreated, rtqCreatedBy) values (@co, @rfq, current_timestamp, @by) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@co", company);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        sql.Parameters.AddWithValue("@by", Context.User.Identity.Name);
                        master.ExecuteNonQuery(sql, "NotifyCompanyRFQ");
                        reallySend = "1";
                        // mark as reallysend because it is new
                    }
                    if (reallySend == "1")
                    {

                    }
                    else
                    {
                        //Response.Write(":" + reallySend + ":");
                    }
                }
            }

            sql.CommandText = "select nreNotificationReasonID from pktblNotificationReason where nreNotificationReason='New RFQ' ";
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                String notificationReason = dr.GetValue(0).ToString();
                RFQ.Models.Notification notification = new Models.Notification();
                notification.SendNotifications(companylist, rfq, notificationReason, master.getUserName());
            }
            dr.Close();

            connection.Close();
        }
    }
}