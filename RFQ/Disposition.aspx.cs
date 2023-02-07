using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class Disposition : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Made it to the disposition code");
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            SqlDataReader dr;

            List<string> rfqIDs = new List<string>();
            string message = "";
            string subject = "";
            string companyID = "";
            Boolean all = false;
            Boolean individualPDF = false;
            //Boolean myCompanies = true;
            Boolean updated = true;
            Boolean sendAsMe = false;
            List<string> emails = new List<string>();
            List<string> ccEmails = new List<string>();
            List<string> bccEmails = new List<string>();
            Boolean noquote = false;
            string CustomerRfq = "";
            string CustomerContact = "";

            if (Request["emails"] != "" && Request["emails"] != null)
            {
                emails.AddRange(Request["emails"].Trim(' ', ',').Split(','));
            }

            if (Request["all"] != "" && Request["all"] != null && System.Convert.ToBoolean(Request["all"]))
            {
                string asdfjklp = Request["all"];
                all = System.Convert.ToBoolean(Request["all"]);
                if (all)
                {
                    updated = false;
                }
            }
            if (Request["updated"] != "" && Request["updated"] != null)
            {
                updated = System.Convert.ToBoolean(Request["updated"]);
                //all = false;
            }
            if (Request["individual"] != "" && Request["individual"] != null)
            {
                individualPDF = System.Convert.ToBoolean(Request["individual"]);
            }
            if (Request["me"] != "" && Request["me"] != null)
            {
                sendAsMe = System.Convert.ToBoolean(Request["me"]);
            }
            if (Request["Message"] != "" && Request["Message"] != null)
            {
                message = Request["Message"];
            }
            if (Request["company"] != "" && Request["company"] != null)
            {
                companyID = Request["company"];
            }
            if (Request["cc"] != "" && Request["cc"] != null)
            {
                ccEmails.AddRange(Request["cc"].Trim(' ', ',').Split(','));
            }
            if (Request["bcc"] != "" && Request["bcc"] != null)
            {
                bccEmails.AddRange(Request["bcc"].Trim(' ', ',').Split(','));
            }
            if (Request["subject"] != "" && Request["subject"] != null)
            {
                subject = Request["subject"];
            }
            if (Request["rfq"] != "" && Request["rfq"] != null)
            {
                rfqIDs.Add(Request["rfq"]);
            }
            if (Request["Cusrfq"] != "" && Request["Cusrfq"] != null)
            {
                CustomerRfq = Request["Cusrfq"];
            }
            if (Request["noquote"] != "" && Request["noquote"] != null)
            {
                noquote = System.Convert.ToBoolean(Request["noquote"]);
            }
            if (Request["customer"] != "" && Request["customer"] != null)
            {
                CustomerContact = Request["customer"];
            }
            else
            {
                //This would be used if we ever set up a bundler (select all RFQs that are ready for distribution)
                //sql.CommandText = "Select rfqID from tblRFQ where rfqStatus = 10";
                //dr = sql.ExecuteReader();
                //while (dr.Read())
                //{
                //    rfqIDs.Add(dr.GetValue(0).ToString());
                //}
                //dr.Close();
            }

            for (int j = 0; j < emails.Count; j++)
            {
                sql.CommandText = "Select Email from CustomerContact where Email = @emails";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@emails", emails[j].Trim().ToLower());
                dr = sql.ExecuteReader();
                if (dr.Read()) { dr.Close(); }
                    else {
                        dr.Close();
                            sql.CommandText = "Select cccEmail from CustomCustomerContact where cccEmail = @emails and cccCreatedBy = @createdby ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@emails", emails[j].Trim().ToLower());
                            sql.Parameters.AddWithValue("@createdby", master.getUserName());
                            dr = sql.ExecuteReader();
                            if (dr.Read()) { dr.Close(); }
                    else
                    {
                        dr.Close();
                        sql.CommandText = "insert into customcustomercontact (cccEmail, cccCreated, cccCreatedBy) ";
                        sql.CommandText += "values(@emails, GetDate(), @createdby) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@emails", emails[j].Trim().ToLower());
                        sql.Parameters.AddWithValue("@createdby", master.getUserName());
                        master.ExecuteNonQuery(sql, "Add Custom Customer");
                    }
                }
            }
            for (int j = 0; j < ccEmails.Count; j++)
            {
                sql.CommandText = "Select Email from CustomerContact where Email = @emails";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@emails", ccEmails[j].Trim().ToLower());
                dr = sql.ExecuteReader();
                if (dr.Read()) { dr.Close(); }
                else
                {
                    dr.Close();
                    sql.CommandText = "Select cccEmail from CustomCustomerContact where cccEmail = @emails and cccCreatedBy = @createdby ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@emails", ccEmails[j].Trim().ToLower());
                    sql.Parameters.AddWithValue("@createdby", master.getUserName());
                    dr = sql.ExecuteReader();
                    if (dr.Read()) { dr.Close(); }
                    else
                    {
                        dr.Close();
                        sql.CommandText = "insert into customcustomercontact (cccEmail, cccCreated, cccCreatedBy) ";
                        sql.CommandText += "values(@emails, GetDate(), @createdby) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@emails", ccEmails[j].Trim().ToLower());
                        sql.Parameters.AddWithValue("@createdby", master.getUserName());
                        master.ExecuteNonQuery(sql, "Add Custom Customer");
                    }
                }
            }
            for (int j = 0; j < bccEmails.Count; j++)
            {
                sql.CommandText = "Select Email from CustomerContact where Email = @emails";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@emails", bccEmails[j].Trim().ToLower());
                dr = sql.ExecuteReader();
                if (dr.Read()) { dr.Close(); }
                else
                {
                    dr.Close();
                    sql.CommandText = "Select cccEmail from CustomCustomerContact where cccEmail = @emails and cccCreatedBy = @createdby ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@emails", bccEmails[j].Trim().ToLower());
                    sql.Parameters.AddWithValue("@createdby", master.getUserName());
                    dr = sql.ExecuteReader();
                    if (dr.Read()) { dr.Close(); }
                    else
                    {
                        dr.Close();
                        sql.CommandText = "insert into customcustomercontact (cccEmail, cccCreated, cccCreatedBy) ";
                        sql.CommandText += "values(@emails, GetDate(), @createdby) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@emails", bccEmails[j].Trim().ToLower());
                        sql.Parameters.AddWithValue("@createdby", master.getUserName());
                        master.ExecuteNonQuery(sql, "Add Custom Customer");
                    }
                }
            }

            if (noquote)
            {
                System.Diagnostics.Debug.WriteLine("Made it to the no quote section");
                //Site master = new Site();
                //SqlCommand sql = new SqlCommand();
                //SqlConnection connection = new SqlConnection(master.getConnectionString());
                //connection.Open();
                //sql.Connection = connection;

                string customerEmail = "";
                sql.CommandText = "Select Email from CustomerContact where CustomerContactID = @contact";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@contact", CustomerContact);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    customerEmail = dr.GetValue(0).ToString().ToLower();
                }
                dr.Close();
                
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
                mail.To.Add(customerEmail);
                var currentUserName = master.getUserEmailAddress(master.getUserID().ToString());
                mail.CC.Add(new MailAddress(currentUserName));
                if (currentUserName != "")
                {
                    mail.CC.Add(new MailAddress("jdalman@toolingsystemsgroup.com"));
                }
                mail.Bcc.Add("dmaguire@toolingsystemsgroup.com");
                //mail.Bcc.Add("rmumford@toolingsystemsgroup.com");
                mail.Subject = "No Quote " + CustomerRfq;
                mail.Body = "Thank you for considering Tooling Systems Group.  After further review it has been determined that we will not be submitting a formal quote to your company on the above mentioned RFQ.  Please feel free to call or e-mail with any questions.<br/><br/>";

                mail.IsBodyHtml = true;

                if (string.IsNullOrWhiteSpace(customerEmail))
                {
                    mail.CC.Clear();
                    mail.To.Add("jdalman@toolingsystemsgroup.com");
                    mail.Body = "There is no email for this contact to send no quotes to <br/><br/>";
                }

                    server.Send(mail);

                connection.Close();

            }
            else {
                //System.Diagnostics.Debug.WriteLine("Made it to the quote section");
                sql.CommandText = "insert into tblQuoteSendLog (qslEmails, qslAll, qslUpdated, qslIndividual, qslMessage, qslCompany, qslCC, qslBCC, qslSubject, qslRFQ, qslCreated, qslCreatedBy) ";
                sql.CommandText += "values(@emails, @all, @updated, @individual, @message, @company, @cc, @bcc, @subject, @rfq, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@emails", String.Join(", ", emails.ToArray()));
                sql.Parameters.AddWithValue("@all", all);
                sql.Parameters.AddWithValue("@updated", updated);
                sql.Parameters.AddWithValue("@individual", individualPDF);
                sql.Parameters.AddWithValue("@message", message);
                sql.Parameters.AddWithValue("@company", companyID);
                sql.Parameters.AddWithValue("@cc", String.Join(", ", ccEmails.ToArray()));
                sql.Parameters.AddWithValue("@bcc", String.Join(", ", bccEmails.ToArray()));
                sql.Parameters.AddWithValue("@subject", subject);
                sql.Parameters.AddWithValue("@rfq", String.Join(", ", rfqIDs.ToArray()));
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Send Quote");

                for (int i = 0; i < rfqIDs.Count; i++)
                {
                    //Getting customer RFQ Number for attachment name
                    sql.CommandText = "Select Email, rfqCustomerRFQNumber from tblRFQ, CustomerContact where rfqID = @rfqID and rfqCustomerContact = CustomerContactID";
                    sql.Parameters.AddWithValue("@rfqID", rfqIDs[i]);
                    dr = sql.ExecuteReader();
                    string custRFQ = "";
                    if (dr.Read())
                    {
                        custRFQ = dr.GetValue(1).ToString();
                    }
                    dr.Close();

                    string plantId = "";
                    //Auto CC the salesman
                    sql.CommandText = "Select Email, rfqPlantID from tblRFQ, TSGSalesman where rfqSalesman = TSGSalesmanID and rfqID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqIDs[i]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        ccEmails.Add(dr.GetValue(0).ToString().ToLower());
                        plantId = dr["rfqPlantID"].ToString();
                    }
                    dr.Close();

                    sql.CommandText = "Select Email ";
                    sql.CommandText += "from CustomerLocation ";
                    sql.CommandText += "left join TSGSalesman ON CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID ";
                    sql.CommandText += "where CustomerLocationID = @plant ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@plant", plantId);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        if (!ccEmails.Contains(dr["Email"].ToString().ToLower()))
                        {
                            ccEmails.Add(dr["Email"].ToString().ToLower());
                        }
                    }
                    dr.Close();

                    //adding myself so i make sure the notifications are going out correctly

                    bccEmails.Add("dmaguire@toolingsystemsgroup.com");
                    bccEmails.Add("bduemler@toolingsystemsgroup.com");
                    bccEmails.Add("tsgrfqadmin@toolingsystemsgroup.com");
                    //bccEmails.Add("djennings@toolingsystemsgroup.com");
                    //if (companyID == "12")
                    //{
                    //    bccEmails.Add("msymanski@toolingsystemsgroup.com");
                    //}
                    if (companyID == "13")
                    {
                        ccEmails.Add("djennings@toolingsystemsgroup.com");
                        ccEmails.Add("jmoore@toolingsystemsgroup.com");
                    }
                    bccEmails.Add(master.getUserName());


                    //this tells if there is actually anything to send out and if there isnt we just return without doing anything
                    List<string> quoteList = new List<string>();
                    List<Boolean> htsList = new List<Boolean>();
                    List<Boolean> stsList = new List<Boolean>();
                    List<Boolean> ugsList = new List<Boolean>();
                    sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS from linkQuoteToRFQ where qtrRFQID = @rfqID ";
                    if (updated && !all)
                    {
                        sql.CommandText += "and qtrSent is null";
                    }
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqIDs[i]);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        quoteList.Add(dr.GetValue(0).ToString());
                        htsList.Add(dr.GetBoolean(1));
                        stsList.Add(dr.GetBoolean(2));
                        ugsList.Add(dr.GetBoolean(3));
                    }
                    dr.Close();

                    List<string> quotesToUpdate = new List<string>();

                    int quotes = 0;
                    //This is so we can timestamp when the quotes were sent
                    if (all)
                    {
                        quotesToUpdate = quoteList;
                        companyID = "1";
                    }
                    else
                    {
                        if (companyID != "1" && companyID != "9" && companyID != "13" && companyID != "15")
                        {
                            for (int j = 0; j < quoteList.Count; j++)
                            {
                                if (!htsList[j] && !stsList[j] && !ugsList[j])
                                {
                                    sql.CommandText = "Select quoTSGCompanyID from tblQuote where quoQuoteID = @quoteID";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@quoteID", quoteList[j]);
                                    dr = sql.ExecuteReader();
                                    if (dr.Read())
                                    {
                                        if (dr.GetValue(0).ToString() == companyID)
                                        {
                                            quotesToUpdate.Add(quoteList[j]);
                                        }
                                    }
                                    dr.Close();
                                }
                            }
                        }
                        else if (companyID == "1")
                        {
                            quotesToUpdate = quoteList;
                        }
                        else if (companyID == "9" && htsList.Count > 0)
                        {
                            for (int j = 0; j < quoteList.Count; j++)
                            {
                                if (htsList[j])
                                {
                                    quotesToUpdate.Add(quoteList[j]);
                                }
                            }
                        }
                        else if (companyID == "13" && stsList.Count > 0)
                        {
                            for (int j = 0; j < quoteList.Count; j++)
                            {
                                if (stsList[j])
                                {
                                    quotesToUpdate.Add(quoteList[j]);
                                }
                            }
                        }
                        else if (companyID == "15" && ugsList.Count > 0)
                        {
                            for (int j = 0; j < quoteList.Count; j++)
                            {
                                if (ugsList[j])
                                {
                                    quotesToUpdate.Add(quoteList[j]);
                                }
                            }
                        }

                        if (quotesToUpdate.Count == 0)
                        {
                            connection.Close();
                            return;
                        }
                    }
                    Attachment attach;
                    CreateQuote createQuote = new CreateQuote();
                    //Creating mail message so we can add attachments
                    MailMessage mail = new MailMessage();

                    //If the sender selected individual PDFs
                    if (individualPDF)
                    {
                        for (int j = 0; j < quotesToUpdate.Count; j++)
                        {
                            if (htsList[j] && (companyID == "1" || companyID == "9"))
                            {
                                createQuote = new CreateQuote();

                                attach = createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quotesToUpdate[j]), "9", System.Convert.ToInt32(rfqIDs[0]));
                                mail.Attachments.Add(attach);
                            }
                            else if (stsList[j] && (companyID == "1" || companyID == "13" || companyID == "20"))
                            {

                                createQuote = new CreateQuote();

                                sql.Parameters.Clear();
                                sql.CommandText = "Select squCompanyID from tblSTSQuote where squSTSQuoteID = @quoteID ";
                                if (companyID != "1")
                                {
                                    sql.CommandText += "and squCompanyID = @company";
                                    sql.Parameters.AddWithValue("@company", companyID);
                                }
                                sql.Parameters.AddWithValue("@quoteID", quotesToUpdate[j]);
                                dr = sql.ExecuteReader();
                                while (dr.Read())
                                {
                                    //quotesToUpdate.Add(quoteList[j]);
                                    attach = createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quotesToUpdate[j]), dr.GetValue(0).ToString(), System.Convert.ToInt32(rfqIDs[0]));
                                    mail.Attachments.Add(attach);
                                }
                                dr.Close();


                                //createQuote = new CreateQuote();

                                //attach = createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quotesToUpdate[j]), "13", System.Convert.ToInt32(rfqIDs[0]));
                                //mail.Attachments.Add(attach);
                            }
                            else if (ugsList[j] && (companyID == "1" || companyID == "15"))
                            {
                                createQuote = new CreateQuote();

                                attach = createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quotesToUpdate[j]), "15", System.Convert.ToInt32(rfqIDs[0]));
                                mail.Attachments.Add(attach);
                            }
                            else
                            {
                                createQuote = new CreateQuote();

                                sql.Parameters.Clear();
                                sql.CommandText = "Select quoTSGCompanyID from tblQuote where quoQuoteID = @quoteID and quoStatusID <> 9 ";
                                if (companyID != "1")
                                {
                                    sql.CommandText += "and quoTSGCompanyID = @company";
                                    sql.Parameters.AddWithValue("@company", companyID);
                                }
                                sql.Parameters.AddWithValue("@quoteID", quotesToUpdate[j]);
                                dr = sql.ExecuteReader();
                                while (dr.Read())
                                {
                                    //quotesToUpdate.Add(quoteList[j]);
                                    attach = createQuote.getIndividualPDFAtachment(System.Convert.ToInt32(quotesToUpdate[j]), dr.GetValue(0).ToString(), System.Convert.ToInt32(rfqIDs[0]));
                                    mail.Attachments.Add(attach);
                                }
                                dr.Close();
                            }
                        }
                    }
                    //Sending all quotes in the RFQ regardless of company
                    else if (all)
                    {
                        attach = createQuote.getPDFAtachment(System.Convert.ToInt32(rfqIDs[i]), companyID, false);
                        mail.Attachments.Add(attach);
                    }
                    //Sending quotes from the user's company
                    else
                    {
                        attach = createQuote.getPDFAtachment(System.Convert.ToInt32(rfqIDs[i]), companyID, updated);
                        mail.Attachments.Add(attach);
                    }

                    //Old code incase we want to include the RFQ summary in the future
                    //RFQSummary rfqSummary = new RFQSummary();
                    //Attachment att = rfqSummary.getWorkbookAttachment(System.Convert.ToInt32(rfqIDs[i]));




                    //setting up mail server
                    System.Net.Mail.SmtpClient server = new SmtpClient("smtp.office365.com");
                    server.UseDefaultCredentials = false;
                    server.Port = 587;
                    server.EnableSsl = true;
                    server.Credentials = master.getNetworkCredentials();
                    server.Timeout = 120000;
                    server.TargetName = "STARTTLS/smtp.office365.com";
                    if (!sendAsMe)
                    {
                        mail.From = master.getFromAddress();
                    }
                    else
                    {
                        mail.From = new MailAddress(master.getUserName(), master.getName());
                    }

                    try
                    {
                        for (int j = 0; j < emails.Count; j++)
                        {
                            mail.To.Add(new MailAddress(emails[j].Trim().ToLower()));
                        }
                        for (int j = 0; j < ccEmails.Count; j++)
                        {
                            mail.CC.Add(new MailAddress(ccEmails[j].Trim().ToLower()));
                        }
                        for (int j = 0; j < bccEmails.Count; j++)
                        {
                            mail.Bcc.Add(new MailAddress(bccEmails[j].Trim().ToLower()));
                        }
                    }
                    catch
                    {
                        mail.To.Clear();
                        mail.CC.Clear();
                        mail.Bcc.Clear();
                        if (master.getUserName() == "chris@netinflux.com")
                        {
                            //mail.To.Add("rmumford@toolingsystemsgroup.com");
                            mail.To.Add("dmaguire@toolingsystemsgroup.com");
                        }
                        else
                        {
                            mail.To.Add(master.getUserName());
                        }
                        mail.Body = "There was a problem with the email addresses that were entered. ";
                        mail.Body += "Please check to make sure all email address entered are valid. ";
                        server.Send(mail);
                        return;
                    }

                    if (custRFQ != "")
                    {
                        if (subject != "")
                        {
                            mail.Subject = subject;
                        }
                        else
                        {
                            mail.Subject = "TSG RFQ '" + custRFQ + "' Response";
                        }
                    }
                    else
                    {
                        //Default subject
                        mail.Subject = "TSG Response for RFQ";
                    }
                    //Default message
                    if (message == "")
                    {
                        mail.Body = "Thank you for your request for quote. The attached files contain our response.";
                    }
                    else
                    {
                        mail.Body = message.Replace("\n", "<br />");
                    }
                    mail.IsBodyHtml = true;
                    //if (att.Name != "Failed")
                    //{
                    //    mail.Attachments.Add(att);
                    //}

                    //Setting up sharepoint to get a list of file names to send with our email
                    List<string> fileNames = new List<string>();

                    Microsoft.SharePoint.Client.ClientContext ctx = new Microsoft.SharePoint.Client.ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/");
                    ctx.Credentials = master.getSharePointCredentials();
                    Microsoft.SharePoint.Client.Web web = ctx.Web;
                    // if this does not exist we will get an error 
                    var mainfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + rfqIDs[i]);
                    ctx.Load(web);

                    Microsoft.SharePoint.Client.ListItem list2 = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + rfqIDs[i]).ListItemAllFields;
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                    Microsoft.SharePoint.Client.Folder fo = list2.Folder;
                    Microsoft.SharePoint.Client.FileCollection files = fo.Files;

                    ctx.Load(files);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                    //getting the file names of the attachments that are going out with the email
                    foreach (Microsoft.SharePoint.Client.File file in files)
                    {
                        fileNames.Add(file.Name);
                    }

                    //Getting the actuall files to attach by accessing from direct url
                    for (int j = 0; j < fileNames.Count; j++)
                    {
                        String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/";
                        String sharepointLibrary = "RFQ%20Email%20Attachments/" + rfqIDs[i];
                        using (var clientContext = new Microsoft.SharePoint.Client.ClientContext(siteUrl))
                        {
                            clientContext.Credentials = master.getSharePointCredentials();
                            var url = new Uri(siteUrl);
                            var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, fileNames[j]);
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
                                mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, fileNames[j]));
                            }
                            catch
                            {

                            }
                        }
                    }

                    //This stops any email from being sent to a customer when there are no attachments (possibly because of an error)
                    if (mail.Attachments.Count < 1)
                    {
                        mail.To.Clear();
                        mail.CC.Clear();
                        mail.Bcc.Clear();
                        if (master.getUserName() == "chris@netinflux.com")
                        {
                            //mail.To.Add("rmumford@toolingsystemsgroup.com");
                            mail.To.Add("dmaguire@toolingsystemsgroup.com");
                        }
                        else
                        {
                            mail.To.Add(master.getUserName());
                        }
                        mail.Body = "There may have been an error sending this email to the customer or getting the attachment. ";
                        mail.Body += "Please contact the admin or check the send checkboxes to make sure they are correctly configered.";
                        server.Send(mail);
                        return;
                    }
                    //Send message
                    try
                    {
                        server.Send(mail);
                        Response.Write("It Worked");
                    }
                    catch (Exception err)
                    {
                        Response.Write(err.Message);

                        mail.To.Clear();
                        mail.CC.Clear();
                        mail.Bcc.Clear();
                        if (master.getUserName() == "chris@netinflux.com")
                        {
                            //mail.To.Add("rmumford@toolingsystemsgroup.com");
                            mail.To.Add("dmaguire@toolingsystemsgroup.com");
                        }
                        else
                        {
                            mail.To.Add(master.getUserName());
                        }
                        mail.Body = "There may have been an error sending this email to the customer or getting the attachment. ";
                        mail.Body += "Please contact the admin or check the send checkboxes to make sure they are correctly configered.";
                        server.Send(mail);
                        return;
                    }
                    //return;

                    for (int j = 0; j < quotesToUpdate.Count; j++)
                    {
                        sql.CommandText = "update linkQuoteToRFQ set qtrSent = GETDATE() where qtrQuoteID = @quoteID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", quotesToUpdate[j]);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr.GetValue(0).ToString() == companyID)
                            {
                                quotes++;
                                break;
                            }
                        }
                        dr.Close();

                        if (companyID == "7")
                        {
                            sql.CommandText = "update tblQuote set quoStatusID = 3 where quoQuoteID = @id";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", quotesToUpdate[j]);
                            master.ExecuteNonQuery(sql, "Disposition");
                        }
                    }

                    sql.Parameters.Clear();

                    sql.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                    sql.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                    sql.Parameters.AddWithValue("@rfqID", rfqIDs[i]);
                    string companyList = "";
                    dr = sql.ExecuteReader();

                    int count = 0;
                    while (dr.Read())
                    {
                        if (count != 0)
                        {
                            companyList += ",";
                        }
                        else
                        {

                        }
                        companyList += dr.GetValue(2).ToString();

                        count++;
                    }
                    dr.Close();
                    sql.Parameters.Clear();

                    RFQ.Models.Notification notification = new Models.Notification();
                    notification.SendNotifications(companyList, rfqIDs[i], "2", master.getUserName());

                    sql.Parameters.Clear();
                    sql.CommandText = "Update tblRFQ set rfqCheckBit = 0, rfqStatus = 12 where rfqID = @rfq";
                    sql.Parameters.AddWithValue("@rfq", rfqIDs[i]);

                    master.ExecuteNonQuery(sql, "Disposition");
                }
                connection.Close();
            }
        }
    }
}