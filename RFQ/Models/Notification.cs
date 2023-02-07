using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RFQ.Models
{
    public class Notification
    {
        //MAKE SURE TO UPDATE BUNDLER WEB JOB WHENEVER THIS IS UPDATED


        // send notificaitions 
        // based on which users from each company have subscribed to the reason that we are notifying for.
        // and the methods that user has selected to be notified by.
        // company can be a list of companies separated with commas
        // rfq is rfq id
        // reason is reason id
        public void SendNotifications(String companylist, String rfq, String reason, String UserName)
        {
            Site master = new Site();
            master.setGlobalVariables();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            List<NotificationLog> usersToLog = new List<NotificationLog>();

            //Company 1 should only be notified the first time around
            int count = 0;
            if (companylist != "")
            {
                foreach (String company in companylist.Split(','))
                {
                    //Company 1 should only be notified the first time around
                    usersToLog.Clear();
                    if(companylist.Length > 1)
                    {
                        if (count == 0)
                        {
                            sql.CommandText = "select ntyNotificationType, unrUserID from tblUserNotificationReasons, Permissions, tblUserNotification, pktblNotificationReason, pktblNotificationType where nreNotificationReasonID=@reason and nreNotificationReasonID=unrReasonID and ( CompanyID=@co or CompanyID=1)  and unrUserID = unoUID and unoUID = UID and unoUserNotificationTypeID = ntyNotificationTypeID ";
                        }
                        else if (company != "1")
                        {
                            sql.CommandText = "select ntyNotificationType, unrUserID from tblUserNotificationReasons, Permissions, tblUserNotification, pktblNotificationReason, pktblNotificationType where nreNotificationReasonID=@reason and nreNotificationReasonID=unrReasonID and ( CompanyID=@co )  and unrUserID = unoUID and unoUID = UID and unoUserNotificationTypeID = ntyNotificationTypeID ";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        sql.CommandText = "select ntyNotificationType, unrUserID from tblUserNotificationReasons, Permissions, tblUserNotification, pktblNotificationReason, pktblNotificationType where nreNotificationReasonID=@reason and nreNotificationReasonID=unrReasonID and ( CompanyID=@co )  and unrUserID = unoUID and unoUID = UID and unoUserNotificationTypeID = ntyNotificationTypeID ";
                    }

                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@co", company);
                    sql.Parameters.AddWithValue("@reason", reason);
                    SqlDataReader dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        try
                        {
                            String customer = "", plant = "", program = "", partNumber = "", customerRFQ = "", dueDate = "", salesmanID = "", notes = "", customerId = "";

                            String notificationType = dr.GetValue(0).ToString();
                            string userID = dr.GetValue(1).ToString();
                            NotificationLog userToLog = new NotificationLog();
                            userToLog.NotificationReason = reason;
                            userToLog.NotificationType = notificationType;
                            userToLog.NotificationUser = userID;

                            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
                            SqlCommand sql2 = new SqlCommand();
                            connection2.Open();
                            sql2.Connection = connection2;

                            sql2.CommandText = "Select CustomerName, ShipToName, ProgramName, count(ptrPartID), rfqCustomerRFQNumber, CONVERT(date, rfqDueDate), TSGSalesmanID, rfqNotes, CustomerLocationID from tblRFQ, linkPartToRFQ, CustomerLocation, Customer, Program ";
                            sql2.CommandText += "where ptrRFQID = rfqID and rfqPlantID = CustomerLocationID and Customer.CustomerID = rfqCustomerID and rfqProgramID = ProgramID and rfqID = @rfqID ";
                            sql2.CommandText += "Group by rfqProgramID, rfqCustomerRFQNumber, rfqDueDate, CustomerName, ShipToName, ProgramName, TSGSalesmanID, rfqNotes, CustomerLocationID";

                            sql2.Parameters.AddWithValue("@rfqID", rfq);
                            SqlDataReader dr2 = sql2.ExecuteReader();
                            if (dr2.Read())
                            {
                                customer = dr2.GetValue(0).ToString();
                                plant = dr2.GetValue(1).ToString();
                                program = dr2.GetValue(2).ToString();
                                partNumber = dr2.GetValue(3).ToString();
                                customerRFQ = dr2.GetValue(4).ToString();
                                dueDate = dr2.GetValue(5).ToString();
                                salesmanID = dr2.GetValue(6).ToString();
                                customerId = dr2.GetValue(8).ToString();
                                if(reason == "7")
                                {
                                    notes = dr2.GetValue(7).ToString();
                                }
                            }
                            dr2.Close();

                            Boolean cont = false;
                            sql2.CommandText = "Select TSGSalesmanID from TSGSalesman where Email = @email";
                            sql2.Parameters.Clear();
                            sql2.Parameters.AddWithValue("@email", master.getUserEmailAddress(userID));
                            dr2 = sql2.ExecuteReader();
                            if (dr2.Read())
                            {
                                if (dr2.GetValue(0).ToString() != salesmanID)
                                {
                                    cont = true;
                                }
                            }
                            dr2.Close();

                            sql2.CommandText = "Select 1 from linkSalesmanToCustomerLocation inner join TSGSalesman on TSGSalesmanID = sclSalesmanID where Email = @email and sclCustomerLocationId = @location ";
                            sql2.Parameters.Clear();
                            sql2.Parameters.AddWithValue("@email", master.getUserEmailAddress(userID));
                            sql2.Parameters.AddWithValue("@location", customerId);
                            dr2 = sql2.ExecuteReader();
                            if (dr2.Read())
                            {
                                cont = false;
                            }
                            dr2.Close();

                            if (userID == "55" && company == "15")
                            {
                                cont = false;
                            }
                            else if (cont)
                            {
                                continue;
                            }

                            sql2.CommandText = "Select nreNotificationReason from pktblNotificationReason where nreNotificationReasonID = @reason";
                            sql2.Parameters.AddWithValue("@reason", reason);

                            dr2 = sql2.ExecuteReader();
                            string reasonText = "";
                            if (dr2.Read())
                            {
                                reasonText = dr2.GetValue(0).ToString();
                            }
                            dr2.Close();

                            string rfqLink = "\nhttps://tsgrfq.azurewebsites.net/EditRFQ?id=" + rfq;
                            userToLog.NotificationMessage = reasonText + " RFQ # " + rfq;
                            userToLog.NotificationResult = "Notification Type Not Implemented Yet";
                            if (notificationType == "Email")
                            {
                                SmtpClient server = new SmtpClient("smtp.office365.com");
                                server.UseDefaultCredentials = false;
                                server.Port = 587;
                                server.EnableSsl = true;
                                // TODO send as another user
                                server.Credentials = master.getNetworkCredentials();
                                server.Timeout = 120000;
                                server.TargetName = "STARTTLS/smtp.office365.com";
                                MailMessage mail = new MailMessage();
                                mail.From = master.getFromAddress();
                                mail.To.Add(new MailAddress(master.getUserEmailAddress(userID)));
                                //mail.Bcc.Add("dmaguire@toolingsystemsgroup.com");
                                mail.Subject = userToLog.NotificationMessage;
                                mail.Body = userToLog.NotificationMessage;
                                mail.Body += "<br />Customer - " + customer + "<br />";
                                mail.Body += "Plant - " + plant + "<br />";
                                mail.Body += "Program - " + program + "<br />";
                                mail.Body += "# of parts - " + partNumber + "<br />";
                                mail.Body += "Customer RFQ # - " + customerRFQ + "<br />";
                                mail.Body += "Due Date - " + dueDate + "<br />";
                                if(reason == "7")
                                {
                                    mail.Body += "Notes - " + notes.Replace("\n", "<br />") + "<br />";
                                }
                                mail.Body += rfqLink;
                                if ((company == "13" || company == "20") && reason == "1")
                                {
                                    sql2.CommandText = "Select spiAnnualVolume, spiProductionDaysPerYear, spiShiftsPerDay, spiHoursPerShift, spiOEE, spiAwardDate, spiRunoff, spiDeliveryDate, ";
                                    sql2.CommandText += "spiPointOfInstallation, spiUnionWorkplace, spiAvailableData, spiAvailableGDT, spiControlsPLC, spiRobots, spiWelders, spiPositioners, spiCNCMachine ";
                                    sql2.CommandText += "from tblSTSPartInfo where spiRFQID = @id ";
                                    sql2.Parameters.Clear();
                                    sql2.Parameters.AddWithValue("@id", rfq);
                                    dr2 = sql2.ExecuteReader();
                                    if (dr2.Read())
                                    {
                                        mail.Body += "<br><br><table>";
                                        mail.Body += "<tr><td>Annual Volume</td><td>" + dr2["spiAnnualVolume"].ToString() + "</td><td>Production Days Per Year</td><td>" + dr2["spiProductionDaysPerYear"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>Shifts Per Day</td><td>" + dr2["spiShiftsPerDay"].ToString() + "</td><td>Hours Per Shift</td><td>" + dr2["spiHoursPerShift"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>OEE</td><td>" + dr2["spiOEE"].ToString() + "</td><td>Award Date</td><td>";
                                        if (dr2["spiAwardDate"].ToString() != "")
                                        {
                                            mail.Body += System.Convert.ToDateTime(dr2["spiAwardDate"].ToString()).ToShortDateString();
                                        }
                                        mail.Body += "</td></tr>";
                                        mail.Body += "<tr><td>Runoff</td><td>" + dr2["spiRunoff"].ToString() + "</td><td>";
                                        if (dr2["spiDeliveryDate"].ToString() != "")
                                        {
                                            mail.Body += System.Convert.ToDateTime(dr2["spiDeliveryDate"].ToString()).ToShortDateString();
                                        }
                                        mail.Body += "</td></tr>";
                                        mail.Body += "<tr><td>Point of Installation</td><td>" + dr2["spiPointOfInstallation"].ToString() + "</td><td>Union Workplace</td><td>" + dr2["spiUnionWorkplace"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>Available Data</td><td>" + dr2["spiAvailableData"].ToString() + "</td><td>Available GDT</td><td>" + dr2["spiAvailableGDT"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>Controls PLC</td><td>" + dr2["spiControlsPLC"].ToString() + "</td><td>Robots</td><td>" + dr2["spiRobots"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>Welders</td><td>" + dr2["spiWelders"].ToString() + "</td><td>positioners</td><td>" + dr2["spiPositioners"].ToString() + "</td></tr>";
                                        mail.Body += "<tr><td>CNC Machine</td><td>" + dr2["spiCNCMachine"].ToString() + "</td></tr>";
                                        mail.Body += "</table";
                                    }
                                    else
                                    {
                                        mail.Body += "<br><br>STS's RFQ info has not been filled out.";
                                    }
                                    dr2.Close();
                                }
                                mail.IsBodyHtml = true;
                                try
                                {
                                    server.Send(mail);
                                    userToLog.NotificationResult = "Success";
                                }
                                catch
                                {
                                    try
                                    {
                                        server.Send(mail);
                                        userToLog.NotificationResult = "Success";
                                    }
                                    catch (Exception err)
                                    {
                                        userToLog.NotificationResult = err.Message;
                                    }
                                }
                            }
                            if (notificationType == "Messaging")
                            {
                                // because of the open data reader, process this one in the next section
                            }
                            if ((notificationType == "Texting") && (master.getUserTextAddress(userID) != ""))
                            {
                                SmtpClient server = new SmtpClient("smtp.office365.com");
                                server.UseDefaultCredentials = false;
                                server.Port = 587;
                                server.EnableSsl = true;
                                server.Credentials = master.getNetworkCredentials();
                                server.Timeout = 50000;
                                server.TargetName = "STARTTLS/smtp.office365.com";
                                MailMessage mail = new MailMessage();
                                mail.From = master.getFromAddress();
                                mail.To.Add(new MailAddress(master.getUserTextAddress(userID)));
                                mail.Subject = userToLog.NotificationMessage;
                                mail.Body = userToLog.NotificationMessage;
                                mail.IsBodyHtml = true;
                                try
                                {
                                    server.Send(mail);
                                    userToLog.NotificationResult = "Success";
                                }
                                catch (Exception err)
                                {
                                    userToLog.NotificationResult = err.Message;
                                }
                            }
                            usersToLog.Add(userToLog);
                            connection2.Close();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    dr.Close();
                    foreach (NotificationLog user in usersToLog)
                    {
                        if (user.NotificationType == "Messaging")
                        {
                            // we process this here because we needed to close out the reader
                            sql.CommandText = "insert into tblMessage (msgUID, msgMessage, msgSent, msgActiveMessage, mesCreated, mesCreatedBy) ";
                            sql.CommandText += " values (@user, @msg, current_timestamp, 1, current_timestamp, @sysuser) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@user", user.NotificationUser);
                            sql.Parameters.AddWithValue("msg", user.NotificationMessage + " https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id=" + rfq);
                            sql.Parameters.AddWithValue("@sysuser", master.UserID);
                            master.ExecuteNonQuery(sql, "Notification.cs");
                            user.NotificationResult = "Success";
                        }
                        sql.CommandText = "select ntyNotificationTypeID from pktblNotificationType where ntyNotificationType=@type";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@type", user.NotificationType);
                        Int64 ntype = 0;
                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            ntype = System.Convert.ToInt64(dr.GetValue(0));
                        }
                        dr.Close();
                        sql.CommandText = "insert into tblNotificationLog (nloNotification, nloNotificationTypeId, nloNotificationReasonID, nloUID, nloMessage, nloResult)   ";
                        sql.CommandText += " values (current_timestamp, @type, @reason, @user, @message, @result) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@type", ntype);
                        sql.Parameters.AddWithValue("@reason", user.NotificationReason);
                        sql.Parameters.AddWithValue("@user", user.NotificationUser);
                        sql.Parameters.AddWithValue("@message", user.NotificationMessage);
                        sql.Parameters.AddWithValue("@result", user.NotificationResult);
                        try
                        {
                            master.ExecuteNonQuery(sql, "Notification.cs");
                        }
                        catch (Exception err)
                        {

                        }
                    }
                    if (reason == "1")
                    {
                        sql.CommandText = "insert into tblCompanyNotified (cnoRFQID, cnoTSGCompanyID, cnoCreated, cnoCreatedBy) values (@rfq, @co, current_timestamp, @by) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@co", company);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        sql.Parameters.AddWithValue("@by", UserName);
                        master.ExecuteNonQuery(sql, "Notification");
                    }
                    //Incrementing count so company 1 will not get notified
                    count++;
                }
            }
            connection.Close();
        }
    }
    public class NotificationLog
    {
        public String NotificationType { get; set; }
        public String NotificationReason { get; set; }
        public String NotificationUser { get; set; }
        public String NotificationMessage { get; set; }
        public String NotificationResult { get; set; }
    }

}