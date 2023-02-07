using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Net.Mail;

namespace RFQ.Models
{
    public class STSNotificaiton
    {
        public void sendNotificaiton(string quoteId, int step, bool? approved = null)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            string email = "";
            string nextApproverName = "";
            string subject = "";
            string databaseBody = "";
            bool quoteReady = false;
            bool quoteSend = false;
            sql.CommandText = "Select EmailAddress, sasDefaultApprover, sasEmailSubject, sasEmailBody, perName, sasQuoteReady, sasQuoteSend from pktblSTSApprovalSteps ";
            sql.CommandText += "left outer join Permissions on UID = sasDefaultApprover ";
            sql.CommandText += "where sasSTSApprovalStepsID  = @id ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", step);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                email = dr["EmailAddress"].ToString();
                nextApproverName = dr["perName"].ToString();
                subject = dr["sasEmailSubject"].ToString();
                databaseBody = dr["sasEmailBody"].ToString();
                quoteReady = System.Convert.ToBoolean(dr["sasQuoteReady"].ToString());
                quoteSend = System.Convert.ToBoolean(dr["sasQuoteSend"].ToString());
            }
            dr.Close();

            string quoteNumber = "";
            string quoteCreator = "";
            string contactName = "";
            string salesmanID = "";
            string estimatorID = "";
            string projectManagerEmail = "";
            int rfqID = 0;
            string quoteInfo = "<br><br>";
            sql.CommandText = "Select squSTSQuoteID, squPartNumber, squPartName, CustomerName, ShipToName, prtRFQLineNumber, assLineNumber, squQuoteNumber, squQuoteVersion, qtrRFQID, ";
            sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToSTSQuote inner join pktblPreWordedNote on pwnPreWordedNoteID = psqPreWordedNoteID where psqSTSQuoteID = squSTSQuoteID) as cost, ";
            sql.CommandText += "squCreatedBy, squCustomerContact, squSalesmanID, squEstimatorID, ProjectManager.Email as projectManager ";
            sql.CommandText += "from tblSTSQuote ";
            sql.CommandText += "inner join Customer on Customer.CustomerID = squCustomerID ";
            sql.CommandText += "inner join CustomerLocation on CustomerLocationID = squPlantID ";
            sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
            sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 and(ptqPartID = (Select min(ptqPartID) from linkPartToQuote where ptqQuoteID = squSTSQuoteID and ptqSTS = 1)) ";
            sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
            sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteID = squSTSQuoteID and atqSTS = 1 ";
            sql.CommandText += "left outer join tblAssembly on assAssemblyID = atqAssemblyId ";
            sql.CommandText += "left outer join ProjectManager on ProjectManagerID = squProjectManagerID ";
            sql.CommandText += "where squSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                if (dr["qtrRFQID"].ToString() != "")
                {
                    rfqID = System.Convert.ToInt32(dr["qtrRFQID"].ToString());
                }
                if (dr["squQuoteNumber"].ToString().Contains("-"))
                {
                    quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                    quoteInfo += "<a href='https://tsgrfq.azurewebsites.net/CreateQuote.aspx?quoteType=4&quoteNumber=" + dr["squSTSQuoteID"].ToString() + "'>" + quoteNumber + "</a>";
                }
                else if (dr["qtrRFQID"].ToString() == "")
                {
                    if (dr["squQuoteNumber"].ToString() == "")
                    {
                        quoteNumber = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                        quoteInfo += "<a href='https://tsgrfq.azurewebsites.net/CreateQuote.aspx?quoteType=4&quoteNumber=" + dr["squSTSQuoteID"].ToString() + "'>" + quoteNumber + "</a>";
                    }
                    else
                    {
                        quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                        quoteInfo += "<a href='https://tsgrfq.azurewebsites.net/CreateQuote.aspx?quoteType=4&quoteNumber=" + dr["squSTSQuoteID"].ToString() + "'>" + quoteNumber + "</a>";
                    }
                }
                else if (dr["assLineNumber"].ToString() != "")
                {
                    quoteNumber = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                    quoteInfo += "<a href='https://tsgrfq.azurewebsites.net/CreateQuote.aspx?quoteType=4&quoteNumber=" + dr["squSTSQuoteID"].ToString() + "'>" + quoteNumber + "</a>";
                }
                else
                {
                    quoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                    quoteInfo += "<a href='https://tsgrfq.azurewebsites.net/CreateQuote.aspx?quoteType=4&quoteNumber=" + dr["squSTSQuoteID"].ToString() + "'>" + quoteNumber + "</a>";
                }
                quoteInfo += "<br>Customer: " + dr["CustomerName"].ToString();
                quoteInfo += "<br>Plant: " + dr["ShipToName"].ToString();
                if (dr["squPartNumber"].ToString().Trim() != "")
                {
                    quoteInfo += "<br>Part #: " + dr["squPartNumber"].ToString();
                }
                if (dr["squPartName"].ToString().Trim() != "")
                {
                    quoteInfo += "<br>Part Name: " + dr["squPartName"].ToString();
                }
                quoteInfo += "<br><a href='https://tsgrfq.azurewebsites.net/STSQuoteDashboard?quoteId=" + dr["squSTSQuoteID"].ToString() + "'>Please click here to visit the quote dashboard.</a>";
                quoteCreator = dr["squCreatedBy"].ToString();

                //quoteCreator = "rmumford@toolingsystemsgroup.com";
                contactName = dr["squCustomerContact"].ToString();
                salesmanID = dr["squSalesmanID"].ToString();
                estimatorID = dr["squEstimatorID"].ToString();
                projectManagerEmail = dr["projectManager"].ToString();

            }
            dr.Close();

            SmtpClient server = new SmtpClient("smtp.office365.com");
            server.UseDefaultCredentials = false;
            server.Port = 587;
            server.EnableSsl = true;
            server.Credentials = master.getNetworkCredentials();
            server.Timeout = 60000;
            server.TargetName = "STARTTLS/smtp.office365.com";
            MailMessage mail = new MailMessage();
            mail.From = master.getFromAddress();

            // if we are not approving or rejecting we ignore this step
            if (approved != null && quoteCreator != "")
            {
                if ((bool)approved)
                {
                    string approvedBy = "";
                    sql.CommandText = "Select top 1 perName, sqsGeneralComments from tblSTSQuoteStatus ";
                    sql.CommandText += "join Permissions on UID = sqsApprovalTo ";
                    sql.CommandText += "where sqsSTSQuoteID = @quoteId order by sqsSTSQuoteStatusID desc";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteId", quoteId);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        approvedBy = dr["perName"].ToString();
                        String GeneralComments = dr["sqsGeneralComments"].ToString();
                        if ((bool)approved)
                        {

                            mail.Body = quoteNumber + " has been approved by " + approvedBy + " " + GeneralComments;
                        }
                        else
                        {
                            mail.Body = quoteNumber + " has been rejected by " + approvedBy + " " + GeneralComments;
                        }
                    }
                    dr.Close();

                    // send approval email
                    mail.To.Add(new MailAddress(quoteCreator));
                    mail.Bcc.Add(new MailAddress("bduemler@toolingsystemsgroup.com"));
                    mail.Subject = quoteNumber + " Approved";
                    // If the quote isn't ready we are going to the next approval step
                    if (!quoteReady)
                    {
                        mail.Body += "<br>The next stage of approval has been sent to " + nextApproverName;
                    }
                    else if (quoteSend)
                    {

                    }
                    else
                    {
                        //mail.To.Add(new MailAddress("jmoore@toolingsystemsgroup.com"));
                        // If the quote is ready then we want different text
                        mail.Body = "Your quote has been approved by " + approvedBy + " and is ready to send to the customer.";
                        mail.Subject = quoteNumber + " Ready To Send";
                    }
                    mail.Body += "<br><br>";
                    mail.Body += quoteInfo;
                    mail.IsBodyHtml = true;

                    string order = "";
                    Boolean firm = false;
                    sql.CommandText = "Select sasOrder, sasFirmQuote from pktblSTSApprovalSteps where sasSTSApprovalStepsID = @stepId ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@stepId", step);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        order = dr["sasOrder"].ToString();
                        firm = System.Convert.ToBoolean(dr["sasFirmQuote"].ToString());
                    }
                    dr.Close();
                    Boolean sendApprovedEmail = true;
                    sql.CommandText = "Select sasSTSApprovalStepsID from pktblSTSApprovalSteps where sasOrder = @order and sasFirmQuote = @firm and sasActive = 1 order by sasSTSApprovalStepsID asc ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@order", order);
                    sql.Parameters.AddWithValue("@firm", firm);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["sasSTSApprovalStepsID"].ToString() != step.ToString())
                        {
                            sendApprovedEmail = false;
                        }
                    }
                    dr.Close();
                    mail = attachQuotes(mail, quoteId, rfqID);

                    if (sendApprovedEmail)
                    {
                        try
                        {
                            server.Send(mail);
                        }
                        catch (Exception err) { }
                    }
                }
                else if (!(bool)approved)
                {
                    // send rejection email
                    mail.To.Add(new MailAddress(quoteCreator));
                    mail.Bcc.Add(new MailAddress("bduemler@toolingsystemsgroup.com"));

                    string approvedBy = "";
                    string generalComments = "";
                    sql.CommandText = "Select top 1 perName, sqsGeneralComments from tblSTSQuoteStatus ";
                    sql.CommandText += "join Permissions on UID = sqsApprovalTo ";
                    sql.CommandText += "where sqsSTSQuoteID = @quoteId order by sqsSTSQuoteStatusID desc";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteId", quoteId);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        approvedBy = dr["perName"].ToString();
                        generalComments = dr["sqsGeneralComments"].ToString();
                    }
                    dr.Close();
                    mail.Subject = quoteNumber + " Rejected";
                    mail.Body = "Your quote has been rejected by " + approvedBy + ", please revise the quote and re-submit for approval.";
                    mail.Body += "<br>Comments: " + generalComments;
                    mail.Body += "<br><a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + quoteId + "' >Edit Quote</a><br><br>" + quoteInfo;
                    mail.IsBodyHtml = true;
                    mail.Attachments.Clear();
                    mail = attachQuotes(mail, quoteId, rfqID);
                    try
                    {
                        server.Send(mail);
                    }
                    catch (Exception err) { }
                    email = "";

                    sql.CommandText = "update tblSTSQuote set squLocked = 0 where squSTSQuoteID = @quoteId";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteId", quoteId);
                    master.ExecuteNonQuery(sql, "STS Notification");


                }
            }
            mail.To.Clear();
            mail.CC.Clear();
            mail.Bcc.Clear();
            mail.Subject = "";
            mail.Body = "";

            if (quoteSend)
            {
                string salesmanName = "";
                string salesmanEmail = "";
                string salesmanPhone = "";
                // logic for sending the quote
                sql.CommandText = "Select Name, Email, MobilePhone from TSGSalesman where TSGSalesmanID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", salesmanID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesmanName = dr["Name"].ToString();
                    salesmanEmail = dr["Email"].ToString();
                    salesmanPhone = dr["MobilePhone"].ToString();
                }
                dr.Close();

                string estimatorName = "";
                string estimatorEmail = "";
                string estimatorPhone = "";
                sql.CommandText = "Select concat(estFirstName, ' ', estLastName) as name, estEmail, estMobilePhone from pktblEstimators where estEstimatorID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", estimatorID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    estimatorName = dr["name"].ToString();
                    estimatorEmail = dr["estEmail"].ToString();
                    estimatorPhone = dr["estMobilePhone"].ToString();
                }
                dr.Close();
                mail.Subject = "STS Quote " + quoteNumber;
                mail.Body = contactName + ",<br><br>";
                mail.Body += "Thank you for giving Specialty Tooling Systems (STS) the opportunity to quote the enclosed tooling.   If you have any questions regarding the enclosed quotation, please feel free to contact us.<Br><br>";
                mail.Body += "Thank you again for giving us the opportunity.<br><br>";
                mail.Body += "Sincerely,<br><br>";
                mail.Body += salesmanName + "<br>" + salesmanEmail + "<br>";
                if (salesmanPhone != "")
                {
                    mail.Body += salesmanPhone + "<br>";
                }
                mail.Body += "<br>";
                mail.Body += estimatorName + "<br>" + estimatorEmail + "<br>";
                if (estimatorPhone != "")
                {
                    mail.Body += estimatorPhone + "<br>";
                }

                mail.Body += "<br>www.ToolingSystemsGroup.com<br>https://www.SpecialtyToolingSystems.com<br>https://vimeo.com/user51659858";

                sql.CommandText = "Select ceqCustomerEmail from linkCustomerEmailToQuote where ceqSTS = 1 and ceqQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteId);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    try
                    {
                        mail.To.Add(new MailAddress(dr["ceqCustomerEmail"].ToString()));
                    }
                    catch { }
                }
                dr.Close();
                if (projectManagerEmail != "")
                {
                    mail.CC.Add(new MailAddress(projectManagerEmail));
                }
                mail.CC.Add(new MailAddress(estimatorEmail));
                mail.CC.Add(new MailAddress(salesmanEmail));
                mail.IsBodyHtml = true;
                mail.Attachments.Clear();
                mail = attachQuotes(mail, quoteId, rfqID);
                try
                {
                    server.Send(mail);
                }
                catch (Exception err) 
                {
                    mail.Body = "There was an issue sending quotes to customer.  Please contact an administrator. ";
                    mail.To.Add(new MailAddress(master.getUserName()));
                    mail.Subject = "Error sending email";
                    server.Send(mail);
                }
            }
            else if (email != "")
            {
                mail.To.Add(new MailAddress(email));
                mail.CC.Add(new MailAddress(master.getUserName()));
                mail.Subject = subject;
                mail.Body = databaseBody + quoteInfo;
                mail.IsBodyHtml = true;

                mail.Attachments.Clear();
                mail = attachQuotes(mail, quoteId, rfqID);
                try
                {
                    server.Send(mail);
                }
                catch (Exception err) { }
            }

            connection.Close();
        }



        public MailMessage attachQuotes(MailMessage mail, string quoteId, int rfqID)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            int attempt = 1;
            sql.CommandText = "Select max(sqsAttemptNumber) as attempt from tblSTSQuoteStatus where sqsSTSQuoteID = @quoteID ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteId);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                if (dr["attempt"].ToString() != "")
                {
                    attempt = System.Convert.ToInt32(dr["attempt"].ToString());
                }    
            }
            dr.Close();
            
            // Attaching all files to go along with the quote that were uploaded to sharepoint
            sql.CommandText = "Select atqAttachmentUrl, atqFilename from linkAttachmentToQuote where atqSTS = 1 and atqQuoteID = @quoteID and atqAttempt = @attempt ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteId);
            sql.Parameters.AddWithValue("@attempt", attempt);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/";
                String sharepointLibrary = "RFQ%20Email%20Attachments/STS%20Quote%20Attachments";
                using (var clientContext = new Microsoft.SharePoint.Client.ClientContext(siteUrl))
                {
                    clientContext.Credentials = master.getSharePointCredentials();
                    var url = new Uri(siteUrl);
                    var relativeUrl = new Uri(dr["atqAttachmentUrl"].ToString()).AbsolutePath;
                    // open the file as binary
                    try
                    {
                        System.IO.MemoryStream ms2;
                        using (Microsoft.SharePoint.Client.FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                        using (var memstr = new System.IO.MemoryStream())
                        {
                            var buf = new byte[1024 * 16];
                            int byteSize;
                            while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                            {
                                memstr.Write(buf, 0, byteSize);
                            }
                            ms2 = new System.IO.MemoryStream(memstr.ToArray());
                        }
                        mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, dr["atqFilename"].ToString()));
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
            dr.Close();

            // Attaching the quote to the email for quick access
            CreateQuote createQuote = new CreateQuote();
            mail.Attachments.Add(createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quoteId), "13", rfqID));

            connection.Close();

            return mail;
        }
    }
}