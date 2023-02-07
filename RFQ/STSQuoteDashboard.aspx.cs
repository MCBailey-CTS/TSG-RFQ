using Microsoft.SharePoint.Client;
using RFQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class STSQuoteDashboard : System.Web.UI.Page
    {
        public int pageIndex
        {
            get
            {
                int retVal = 0;
                if (ViewState["pageIndex"] != null)
                {
                    retVal = System.Convert.ToInt32(ViewState["pageIndex"].ToString());
                }
                return retVal;
            }
            set { ViewState["pageIndex"] = value; }
        }
        public string sort
        {
            get
            {
                string retVal = "";
                if (ViewState["sort"] != null)
                {
                    retVal = ViewState["sort"].ToString();
                }
                return retVal;
            }
            set { ViewState["sort"] = value; }
        }
        public string order
        {
            get
            {
                string retVal = "";
                if (ViewState["order"] != null)
                {
                    retVal = ViewState["order"].ToString();
                }
                return retVal;
            }
            set { ViewState["order"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            if (!IsPostBack)
            {
                sql.CommandText = "Select concat(estFirstName, ' ', estLastName) as Name, estEstimatorID from pktblEstimators where estCompanyID = 13 or estCompanyID = 20 order by Name ";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                ddlEstimator.DataSource = dr;
                ddlEstimator.DataValueField = "estEstimatorID";
                ddlEstimator.DataTextField = "Name";
                ddlEstimator.DataBind();
                dr.Close();
                ddlEstimator.Items.Insert(0, "Please Select");

                sql.CommandText = "Select Name, TSGSalesmanID from TSGSalesman where tsaActive = 1 order by Name ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlSalesman.DataSource = dr;
                ddlSalesman.DataValueField = "TSGSalesmanID";
                ddlSalesman.DataTextField = "Name";
                ddlSalesman.DataBind();
                dr.Close();
                ddlSalesman.Items.Insert(0, "Please Select");

                sql.CommandText = "Select CustomerID, CustomerName from Customer where cusInactive = 0 or cusInactive is NULL order by CustomerName ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlCustomer.DataSource = dr;
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataTextField = "CustomerName";
                ddlCustomer.DataBind();
                dr.Close();
                ddlCustomer.Items.Insert(0, "Please Select");

                sql.CommandText = "Select Name, ProjectManagerID from ProjectManager where pmaTSGCompanyID = 13 or pmaTSGCompanyID = 20 order by Name ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlProjectManager.DataSource = dr;
                ddlProjectManager.DataTextField = "Name";
                ddlProjectManager.DataValueField = "ProjectManagerID";
                ddlProjectManager.DataBind();
                ddlProjectManager.Items.Insert(0, "Please Select");
                dr.Close();

                sort = "squSTSQuoteID";
                order = "desc";

                int quoteId = 0;
                if (Request["quoteId"] != null)
                {
                    try
                    {
                        quoteId = System.Convert.ToInt32(Request["quoteId"].ToString());
                    }
                    catch
                    {

                    }
                }

                plants();
                PopulatePage(quoteId);
            }

            connection.Close();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            plants();
        }

        private void plants()
        {
            ddlPlant.Items.Clear();
            if (ddlCustomer.SelectedItem.ToString() == "Please Select")
            {
                ddlPlant.Items.Insert(0, "Please Select");
                return;
            }
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            sql.CommandText = "Select ShipToName, CustomerLocationID from CustomerLocation where CustomerID = @customerId order by ShipToName ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customerId", ddlCustomer.SelectedValue.ToString());
            SqlDataReader dr = sql.ExecuteReader();
            ddlPlant.DataSource = dr;
            ddlPlant.DataTextField = "ShipToName";
            ddlPlant.DataValueField = "CustomerLocationID";
            ddlPlant.DataBind();
            dr.Close();
            ddlPlant.Items.Insert(0, "Please Select");

            connection.Close();
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            PopulatePage();
        }

        private void PopulatePage(int quoteId = 0)
        {
            gvResults.DataSource = null;
            gvResults.DataBind();
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            List<StsQuote> quotes = new List<StsQuote>();
            sql.Parameters.Clear();
            sql.CommandText = "Select squSTSQuoteID, squQuoteNumber, squQuoteVersion, squECQuote, squECBaseQuoteId, squECQuoteNumber, squPartNumber, squPartName, Name as Salesman, concat(estFirstName, ' ', estLastName) as Estimator, ";
            sql.CommandText += "qtrRFQID, prtRFQLineNumber, CustomerName, ShipToName, squCustomerRFQNum, assLineNumber, ";
            sql.CommandText += "(Select sum(pwnCostNote) from pktblPreWordedNote inner join linkPWNToSTSQuote on psqPreWordedNoteID = pwnPreWordedNoteID where psqSTSQuoteID = squSTSQuoteID) as cost, ";
            sql.CommandText += "(Select sum(sqnToolingCosts + sqnCapitalCosts) from pktblSTSQuoteNotes where sqnQuoteID = squSTSQuoteID) as newCost, ";
            sql.CommandText += "perName, EmailAddress, stat.sqsStepStartedDate, stat.sqsStepFinishedDate, sasApprovalStep, stat.sqsApproved, sasQuoteReady, sasQuoteSend, squCreatedBy, TSGCompanyAbbrev, prtPARTID, ";
            sql.CommandText += "case when sqs.sqsSTSQuoteStatusID is null then 0 else 1 end as approvalNeeded, squCreated ";
            sql.CommandText += "from tblSTSQuote ";
            sql.CommandText += "inner join Customer on Customer.CustomerID = squCustomerID ";
            sql.CommandText += "inner join CustomerLocation on CustomerLocationID = squPlantID ";
            sql.CommandText += "inner join TSGSalesman on TSGSalesman.TSGSalesmanID = squSalesmanID ";
            sql.CommandText += "inner join pktblEstimators on estEstimatorID = squEstimatorID ";
            sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
            sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 and(ptqPartID = (Select min(ptqPartID) from linkPartToQuote where ptqQuoteID = squSTSQuoteID and ptqSTS = 1)) ";
            sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
            sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteID = squSTSQuoteID and atqSTS = 1 ";
            sql.CommandText += "left outer join tblAssembly on assAssemblyID = atqAssemblyId ";
            sql.CommandText += "left outer join tblSTSQuoteStatus stat on stat.sqsSTSQuoteStatusID = (Select top 1 sqsSTSQuoteStatusID from tblSTSQuoteStatus where sqsSTSQuoteID = squSTSQuoteID order by sqsSTSQuoteStatusID desc) ";
            sql.CommandText += "left outer join Permissions on UID = sqsApprovalTo ";
            sql.CommandText += "left outer join pktblSTSApprovalSteps on sasSTSApprovalStepsID = sqsStepID ";
            sql.CommandText += "left outer join TSGCompany on TSGCompanyID = squCompanyId ";
            sql.CommandText += "left outer join tblSTSQuoteStatus sqs on sqs.sqsSTSQuoteID = squSTSQuoteID and sqs.sqsApprovalTo = @user and sqs.sqsStepFinishedDate is null ";
            sql.CommandText += "where (qtrRFQID like @rfq ";
            if (txtRFQ.Text.Trim() == "")
            {
                sql.CommandText += "or qtrRFQID is null ";
            }
            sql.CommandText += ")";
            if (ddlEstimator.SelectedValue.ToString() != "Please Select")
            {
                sql.CommandText += "and estEstimatorID = @estimator ";
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue.ToString());
            }
            if (ddlSalesman.SelectedValue.ToString() != "Please Select")
            {
                sql.CommandText += "and TSGSalesman.TSGSalesmanID = @salesman ";
                sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
            }
            if (ddlCustomer.SelectedValue.ToString() != "Please Select")
            {
                sql.CommandText += "and Customer.CustomerID = @customer ";
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue.ToString());
            }
            if (ddlPlant.SelectedValue.ToString() != "Please Select")
            {
                sql.CommandText += "and CustomerLocationID = @plant ";
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue.ToString());
            }
            if (quoteId != 0)
            {
                sql.CommandText += "and squSTSQuoteID = @quoteId ";
                sql.Parameters.AddWithValue("@quoteId", quoteId);
            }

            sql.CommandText += "order by " + sort + " " + order;
            sql.Parameters.AddWithValue("@rfq", "%" + txtRFQ.Text + "%");
            sql.Parameters.AddWithValue("@user", master.getUserID());

            string user = master.getUserName();

            SqlDataReader dr = sql.ExecuteReader();
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            while (dr.Read())
            {
                StsQuote s = new StsQuote();
                s.Rfq = dr["qtrRFQID"].ToString();
                s.QuoteID = dr["squSTSQuoteID"].ToString();
                if ((dr["squQuoteNumber"].ToString().Contains("-")) && !dr["squQuoteNumber"].ToString().Contains("EC"))
                {
                    s.QuoteNumber = dr["squQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                    //                    s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squQuoteNumber"].ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                    s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";

                }
                else if ((dr["qtrRFQID"].ToString() == "") && ((dr["squECQuote"].ToString() == "False") || (dr["squECQuote"].ToString() == "")))
                {
                    if (dr["squQuoteNumber"].ToString() == "")
                    {
                        s.QuoteNumber = dr["squSTSQuoteID"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString();
                        //                        s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                        s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                    }
                    else
                    {
                        s.QuoteNumber = dr["squQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString();
                        //                        s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                        s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                    }
                }
                else if ((dr["qtrRFQID"].ToString() == "") && (dr["squECQuote"].ToString() == "True"))
                {
                    if (dr["squQuoteNumber"].ToString() == "")
                    {
                        s.QuoteNumber = dr["squSTSQuoteID"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                    }
                    else
                    {
                        s.QuoteNumber = dr["squQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                    }
                }
                else if ((dr["qtrRFQID"].ToString() != "") && (dr["squECQuote"].ToString() == "True"))
                {
                    s.QuoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                }
                else if (dr["assLineNumber"].ToString() != "")
                {
                    s.QuoteNumber = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString()  + "-" + dr["squQuoteVersion"].ToString();
                }
                else
                {
                    s.QuoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                    s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                    // BD 1-6-2020 s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squSTSQuoteID"].ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";
                    //                    s.CreateECButton = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr["squQuoteNumber"].ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr["prtPARTID"].ToString() + "&createEC=true " + "'>Create EC</a>";

                }

                s.PartNumber = dr["squPartNumber"].ToString().Trim();
                if (s.PartName != "" && s.PartName != null)
                {
                    s.PartNumber = "Number: " + s.PartNumber;
                }
                s.PartName = dr["squPartName"].ToString().Trim();
                if (s.PartName != "" && s.PartName != null)
                {
                    if (s.PartNumber != "")
                    {
                        s.PartNumber += "<br>";
                    }
                    s.PartNumber += "Name: " + s.PartName;
                }
                s.Salesman = dr["Salesman"].ToString();
                s.Estimator = dr["Estimator"].ToString();
                s.Customer = dr["CustomerName"].ToString();
                s.Plant = dr["ShipToName"].ToString();
                s.DateCreated = System.Convert.ToDateTime(dr["squCreated"].ToString()).ToShortDateString();
                if (s.Customer != s.Plant)
                {
                    s.Customer += "<br>" + s.Plant;
                }
                s.CustomerRFQ = dr["squCustomerRFQNum"].ToString();
                if (dr["cost"].ToString().Trim() != "")
                {
                    s.Cost = System.Convert.ToDouble(dr["cost"].ToString()).ToString("$###,###,###,###.##");
                }
                else if (dr["newCost"].ToString().Trim() != "")
                {
                    s.Cost = System.Convert.ToDouble(dr["newCost"].ToString()).ToString("$###,###,###,###.##");
                }
                else
                {
                    s.Cost = "$0";
                }
                if (s.Cost.Trim() == "$")
                {
                    s.Cost = "$0";
                }
                //perName, EmailAddress, sqsStepStartedDate, sqsStepFinishedDate, sasApprovalStep
                if (dr["sqsApproved"].ToString() != "")
                {
                    Boolean approved = System.Convert.ToBoolean(dr["sqsApproved"].ToString());
                    DateTime endDate = System.Convert.ToDateTime(dr["sqsStepFinishedDate"].ToString());
                    Boolean quoteReady = System.Convert.ToBoolean(dr["sasQuoteReady"].ToString());
                    Boolean quoteSend = System.Convert.ToBoolean(dr["sasQuoteSend"].ToString());
                    if (approved)
                    {
                        if (quoteSend)
                        {
                            s.ApprovalButton = "Quote sent on " + TimeZoneInfo.ConvertTimeFromUtc(endDate, est).ToString();
                        }
                        else if (quoteReady)
                        {
                            s.ApprovalButton = "Quote ready to send on " + TimeZoneInfo.ConvertTimeFromUtc(endDate, est).ToString();
                        }
                        else
                        {
                            s.ApprovalButton = "Approved by " + dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(endDate, est).ToString();
                        }
                    }
                    else if (dr["squCreatedBy"].ToString().ToLower() == user.ToLower())
                    {
                        s.ApprovalButton = "<button id='btnApproval" + dr["squSTSQuoteID"].ToString() + "' class='ui-widget mybutton' onclick='submitForApproval(" + dr["squSTSQuoteID"].ToString() + ");return false;'>Submit for Approval</button>";
                    }
                    else
                    {
                        s.ApprovalButton = "Rejected by " + dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(endDate, est).ToString();
                    }
                }
                else if (dr["approvalNeeded"].ToString() == "1")
                {
                    s.ApprovalButton = "<button id='btnApproval" + dr["squSTSQuoteID"].ToString() + "' class='ui-widget mybutton' onclick='approval(" + dr["squSTSQuoteID"].ToString() + ");return false;'>Submit Approval</button>";
                }
                else if (dr["sqsStepFinishedDate"].ToString() != "")
                {
                    DateTime endDate = System.Convert.ToDateTime(dr["sqsStepFinishedDate"].ToString());
                    s.ApprovalButton = "Approved by " + dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(endDate, est).ToString();
                }
                else if (dr["sqsStepStartedDate"].ToString() != "")
                {
                    DateTime startDate = System.Convert.ToDateTime(dr["sqsStepStartedDate"].ToString());
                    s.ApprovalButton = "Submitted to " + dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(startDate, est).ToString();
                }
                else if (dr["squCreatedBy"].ToString() == user)
                {
                    s.ApprovalButton = "<button id='btnApproval" + dr["squSTSQuoteID"].ToString() + "' class='ui-widget mybutton' onclick='submitForApproval(" + dr["squSTSQuoteID"].ToString() + ");return false;'>Submit for Approval</button>";
                }
                quotes.Add(s);
            }
            dr.Close();
            gvResults.DataSource = quotes;
            gvResults.DataBind();

            connection.Close();
        }

        protected void btnApproval_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            string quoteID = hdnQuoteID.Value;

            Boolean firm = false;
            sql.CommandText = "Select squFirmQuote from tblSTSQuote where squSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteID);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {

            }
            dr.Close();

            int step = 0;
            string defaultApprover = "";
            sql.CommandText = "Select top 1 sasSTSApprovalStepsID, sasDefaultApprover from pktblSTSApprovalSteps where sasFirmQuote = @firm and sasActive = 1 order by sasOrder ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@firm", firm);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                step = System.Convert.ToInt32(dr["sasSTSApprovalStepsID"].ToString());
                defaultApprover = dr["sasDefaultApprover"].ToString();
            }
            dr.Close();

            int attempt = 1;
            sql.CommandText = "Select top 1 sqsAttemptNumber from tblSTSQuoteStatus where sqsStepId = @step and sqsSTSQuoteID = @quote order by sqsAttemptNumber desc ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@step", step);
            sql.Parameters.AddWithValue("@quote", quoteID);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                attempt = System.Convert.ToInt32(dr["sqsAttemptNumber"].ToString());
                attempt++;
            }
            dr.Close();

            //lblStatus.ForeColor = System.Drawing.Color.Red;
            string user = master.getUserName();

            string url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/STS%20Quote%20Attachments/";
            int count = 0;
            foreach (var f in fuQuote.PostedFiles)
            {
                if (f.FileName != "")
                {
                    sql.CommandText = "insert into linkAttachmentToQuote (atqAttachmentUrl, atqQuoteID, atqFilename, atqAttempt, atqHTS, atqSTS, atqUGS, atqCreated, atqCreatedBy) ";
                    sql.CommandText += "values(@url, @quote, @filename, @attempt, 0, 1, 0, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@url", url + quoteID + " " + count + " " + attempt.ToString() + System.IO.Path.GetExtension(f.FileName));
                    sql.Parameters.AddWithValue("@quote", quoteID);
                    sql.Parameters.AddWithValue("@filename", f.FileName);
                    sql.Parameters.AddWithValue("@attempt", attempt);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");

                    Microsoft.SharePoint.Client.ClientContext ctx = new Microsoft.SharePoint.Client.ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/");
                    ctx.Credentials = master.getSharePointCredentials();
                    Microsoft.SharePoint.Client.Web web = ctx.Web;
                    // if this does not exist we will get an error 
                    //var mainfolder = web.GetFolderByServerRelativeUrl(url);
                    ctx.Load(web);

                    Microsoft.SharePoint.Client.List list = ctx.Web.Lists.GetByTitle("Documents");
                    Microsoft.SharePoint.Client.ListItem list2 = web.GetFolderByServerRelativeUrl(url).ListItemAllFields;

                    ctx.Load(list);
                    ctx.Load(list.RootFolder);
                    ctx.Load(list.RootFolder.Folders);
                    ctx.Load(list.RootFolder.Files);
                    ctx.ExecuteQuery();

                    Microsoft.SharePoint.Client.Folder fo = list2.Folder;
                    Microsoft.SharePoint.Client.FileCollection files = fo.Files;

                    ctx.Load(files);
                    ctx.ExecuteQuery();

                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.ContentStream = f.InputStream;
                    newFile.Url = url + quoteID + " " + count + " " + attempt.ToString() + System.IO.Path.GetExtension(f.FileName);
                    newFile.Overwrite = true;

                    Microsoft.SharePoint.Client.File file = list.RootFolder.Files.Add(newFile);
                    list.Update();

                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    count++;
                }
            }

            if (step == 0)
            {
                //lblStatus.Text = "There was a problem sending the notification.  Please contact an administrator.";
                connection.Close();
                return;
            }
            STSNotificaiton n = new STSNotificaiton();
            try
            {
                //lblStatus.Text = "Your quote has been submitted for approval.";

                sql.CommandText = "insert into tblSTSQuoteStatus (sqsSTSQuoteID, sqsStepID, sqsApprovalTo, sqsAttemptNumber, sqsStepStartedDate, sqsCreated, sqsCreatedBy) ";
                sql.CommandText += "values (@quoteId, @stepId, @approvalTo, @attempt, GETDATE(), GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                sql.Parameters.AddWithValue("@stepId", step);
                sql.Parameters.AddWithValue("@approvalTo", defaultApprover);
                sql.Parameters.AddWithValue("@attempt", attempt);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "STS Notification");

                n.sendNotificaiton(quoteID, step);
            }
            catch (Exception ex)
            {
                //lblStatus.Text = "There was a problem sending the notification.  Please contact an administrator.";
                sql.CommandText = "delete from linkAttachmentToQuote where atqQuoteID = @quoteID and atqAttempt = @attempt ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@attempt", attempt);
                master.ExecuteNonQuery(sql, "STS Edit Quote");
                connection.Close();
                return;
            }

            sql.CommandText = "update tblSTSQuote set squLocked = 1, squProjectManagerID = @projectManager where squSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteID);
            sql.Parameters.AddWithValue("@projectManager", ddlProjectManager.SelectedValue);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            //foreach (var email in txtApproverEmail.Text.Split(';'))
            //{
            //    if (!email.Contains("@"))
            //    {
            //        continue;
            //    }
            //    sql.CommandText = "insert into linkCustomerEmailToQuote (ceqCustomerEmail, ceqQuoteID, ceqHTS, ceqSTS, ceqUGS, ceqCreated, ceqCreatedBy) ";
            //    sql.CommandText += "values(@email, @quote, 0, 1, 0, GETDATE(), @user) ";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@email", email.Trim());
            //    sql.Parameters.AddWithValue("@quote", quoteID);
            //    sql.Parameters.AddWithValue("@user", user);
            //    master.ExecuteNonQuery(sql, "STS Edit Quote");
            //}

            sql.CommandText = "Select top 1 sqsStepStartedDate, perName ";
            sql.CommandText += "from tblSTSQuoteStatus ";
            sql.CommandText += "inner join Permissions on UID = sqsApprovalTo ";
            sql.CommandText += "where sqsSTSQuoteID = @id ";
            sql.CommandText += "order by sqsSTSQuoteStatusID DESC ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", quoteID);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                DateTime startDate = System.Convert.ToDateTime(dr["sqsStepStartedDate"].ToString());
                TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                //lblStatus.Text += dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(startDate, est).ToString();
            }
            dr.Close();

            connection.Close();
        }

        protected void OnSort(object sender, GridViewSortEventArgs e)
        {
            if (sort == e.SortExpression)
            {
                if (order == "asc")
                {
                    order = "desc";
                }
                else
                {
                    order = "asc";
                }
            }
            else
            {
                order = "asc";
            }
            sort = e.SortExpression;
            gvResults.PageIndex = pageIndex;
            PopulatePage();
        }

        protected void OnPaging(object sender, GridViewPageEventArgs e)
        {
            pageIndex = e.NewPageIndex;
            gvResults.PageIndex = pageIndex;
            PopulatePage();
        }

        protected void btnSaveSTSInfo_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            connection.Close();
        }
    }
    partial class StsQuote
    {
        public string Rfq { get; set; }
        public string QuoteNumber { get; set; }
        public string QuoteID { get; set; }
        public string ECQuoteID { get; set; }
        public string ECBaseQuoteID { get; set; }
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public string Salesman { get; set; }
        public string Estimator { get; set; }
        public string Customer { get; set; }
        public string Plant { get; set; }
        public string CustomerRFQ { get; set; }
        public string Cost { get; set; }
        public string ApprovalButton { get; set; }
        public string CreateECButton { get; set; }
        public string DateCreated { get; set; }
    }
}