using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using RFQ.Models;
using System.Net.Mail;

namespace RFQ
{
    public partial class STSApprovals : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            string quoteId = "";
            if (Request["quote"] != null)
            {
                quoteId = Request["quote"].ToString();
            }

            string history = "";
            if (Request["history"] != null)
            {
                history = Request["history"].ToString();
            }

            bool approved = true;
            if (Request["approved"] != null)
            {
                approved = System.Convert.ToBoolean(Request["approved"].ToString());
            }

            string approvalComments = "";
            if (Request["approvalComments"] != null)
            {
                approvalComments = Request["approvalComments"].ToString();
            }

            string submit = "";
            if (Request["submit"] != null)
            {
                submit = Request["submit"].ToString();
            }

            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            if (history != "")
            {
                string results = "<div class='col-lg-12' style='padding-bottom: 15px'><center>Approval History</center></div><div class='row' style='padding-bottom: 5px;'><div class='col-lg-2'>Submitted by</div><div class='col-lg-2'>Start Date</div><div class='col-lg-2'>Finished Date</div><div class='col-lg-2'>Approver</div><div class='col-lg-2'>Comments</div><div class='col-lg-2'>Attempt</div></div>";
                sql.CommandText = "Select approver.perName as approver, creator.perName as creator, sqsAttemptNumber, sqsStepStartedDate, sqsStepFinishedDate, sqsApproved, sqsGeneralComments ";
                sql.CommandText += "from tblSTSQuoteStatus ";
                sql.CommandText += "inner join pktblSTSApprovalSteps on sasSTSApprovalStepsID = sqsStepID ";
                sql.CommandText += "inner join Permissions approver on approver.UID = sqsApprovalTo ";
                sql.CommandText += "inner join Permissions creator on creator.EmailAddress = sqsCreatedBy ";
                sql.CommandText += "where sqsSTSQuoteID = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteId);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    results += "<div class='row' style='padding: 5px;'><div class='col-lg-2'>" + dr["creator"].ToString() + "</div>";
                    results += "<div class='col-lg-2'>" + dr["sqsStepStartedDate"].ToString() + "</div>";
                    results += "<div class='col-lg-2'>" + dr["sqsStepFinishedDate"].ToString() + "</div>";
                    results += "<div class='col-lg-2'>" + dr["approver"].ToString() + "</div>";
                    results += "<div class='col-lg-2'>" + dr["sqsGeneralComments"].ToString() + "</div>";
                    results += "<div class='col-lg-2'>" + dr["sqsAttemptNumber"].ToString() + "</div></div>";
                }
                dr.Close();
                litResults.Text = results;
            }
            else if (submit != "")
            {
                int step = 0;
                bool firm = false;

                sql.CommandText = "Select squFirmQuote from tblSTSQuote where squSTSQuoteID = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteId);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["squFirmQuote"].ToString() != "")
                    {
                        firm = System.Convert.ToBoolean(dr["squFirmQuote"].ToString());
                    }
                }
                dr.Close();

                List<string> defaultApprover = new List<string>();
                sql.CommandText = "Select top 1 sasSTSApprovalStepsID, sasDefaultApprover from pktblSTSApprovalSteps where sasFirmQuote = @firm and sasActive = 1 order by sasOrder ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@firm", firm);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    step = System.Convert.ToInt32(dr["sasSTSApprovalStepsID"].ToString());
                    defaultApprover.Add(dr["sasDefaultApprover"].ToString());
                }
                dr.Close();

                if (step == 0)
                {
                    connection.Close();
                    return;
                }
                STSNotificaiton n = new STSNotificaiton();
                try
                {
                    n.sendNotificaiton(quoteId, step);

                    int attempt = 1;
                    sql.CommandText = "Select top 1 sqsAttemptNumber from tblSTSQuoteStatus where sqsStepId = @step and sqsSTSQuoteID = @quote order by sqsAttemptNumber desc ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@step", step);
                    sql.Parameters.AddWithValue("@quote", quoteId);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        attempt = System.Convert.ToInt32(dr["sqsAttemptNumber"].ToString());
                        attempt++;
                    }
                    dr.Close();

                    for (int i = 0; i < defaultApprover.Count; i++)
                    {
                        sql.CommandText = "insert into tblSTSQuoteStatus (sqsSTSQuoteID, sqsStepID, sqsApprovalTo, sqsAttemptNumber, sqsStepStartedDate, sqsCreated, sqsCreatedBy) ";
                        sql.CommandText += "values (@quoteId, @stepId, @approvalTo, @attempt, GETDATE(), GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteId", quoteId);
                        sql.Parameters.AddWithValue("@stepId", step);
                        sql.Parameters.AddWithValue("@approvalTo", defaultApprover[i]);
                        sql.Parameters.AddWithValue("@attempt", attempt);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "STS Notification");
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    return;
                }

                sql.CommandText = "update tblSTSQuote set squLocked = 1 where squSTSQuoteID = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteId);
                master.ExecuteNonQuery(sql, "STS Edit Quote");
            }
            else 
            {
                if (!approved)
                {
                    sql.CommandText = "update tblSTSQuote set squLocked = 1 where squSTSQuoteID = @quoteId ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteId", quoteId);
                    master.ExecuteNonQuery(sql, "STS Approvals");
                }

                sql.CommandText = "update tblSTSQuoteStatus set sqsApproved = @approved, sqsStepFinishedDate = GETDATE(), sqsGeneralComments = @comments, sqsModified = GETDATE(), sqsModifiedBy = @userName ";
                sql.CommandText += "where sqsStepFinishedDate is null and sqsApprovalTo = @userId and sqsSTSQuoteID = @quote ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteId);
                sql.Parameters.AddWithValue("@approved", approved);
                sql.Parameters.AddWithValue("@comments", approvalComments);
                sql.Parameters.AddWithValue("@userName", master.getUserName());
                sql.Parameters.AddWithValue("@userId", master.getUserID());

                master.ExecuteNonQuery(sql, "STS Approvals");

                SendStatusUpdates(quoteId, approved);
            }

            connection.Close();
        }

        public void SendStatusUpdates (string quoteId, bool approved)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            bool firm = false;
            sql.CommandText = "Select squFirmQuote from tblSTSQuote where squSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                if (dr["squFirmQuote"].ToString() != "")
                {
                    firm = System.Convert.ToBoolean(dr["squFirmQuote"].ToString());
                }
            }
            dr.Close();

            Boolean nextStep = true;
            sql.CommandText = "Select top 1 1 from tblSTSQuoteStatus where sqsSTSQuoteID = @quoteId and sqsStepFinishedDate is null ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                nextStep = false;
            }
            dr.Close();

            STSNotificaiton n = new STSNotificaiton();


            bool quoteReady = false;
            bool quoteSend = false;
            List<int> step = new List<int>();
            int attempt = 1;
            List<string> defaultApprover = new List<string>();

            sql.CommandText = "Select max(sqsAttemptNumber) as attempt from tblSTSQuoteStatus where sqsSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                attempt = System.Convert.ToInt32(dr["attempt"].ToString());
            }
            dr.Close();

            double cost = 0;
            sql.CommandText = "Select sum(pwnCostNote) as cost from linkPWNToSTSQuote inner join pktblPreWordedNote on pwnPreWordedNoteID = psqPreWordedNoteID where psqSTSQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                if (dr["cost"].ToString() != "")
                {
                    cost = System.Convert.ToDouble(dr["cost"].ToString());
                }
            }
            dr.Close();

            string order = "";
            sql.CommandText = "Select sasQuoteReady, sasQuoteSend, sasDefaultApprover, sasSTSApprovalStepsID, sqsStepFinishedDate, EmailAddress, sqsAttemptNumber, sasOrder, sasThreshold ";
            sql.CommandText += "from pktblSTSApprovalSteps ";
            sql.CommandText += "left outer join tblSTSQuoteStatus on sqsStepID = sasSTSApprovalStepsID and sqsSTSQuoteID = @quoteId and sqsAttemptNumber = @attempt ";
            sql.CommandText += "left outer join Permissions on UID = sqsApprovalTo ";
            sql.CommandText += "where sasFirmQuote = @firm and sasActive = 1 ";
            sql.CommandText += "order by sqsAttemptNumber desc, sasOrder asc ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@firm", firm);
            sql.Parameters.AddWithValue("@quoteId", quoteId);
            sql.Parameters.AddWithValue("@attempt", attempt);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                // If it was rejected we want the first step every time
                if (dr["sqsStepFinishedDate"].ToString() == "" || !(bool)approved)
                {
                    double threshold;
                    if (Double.TryParse(dr["sasThreshold"].ToString(), out threshold))
                    {
                        // If there is a threshold but it is greater than the cost we want to skip and go to the next step
                        if (threshold > cost)
                        {
                            continue;
                        }
                    }
                    // This will allow us to have multiple steps for one order (multiple approvals at the same time)
                    if (order != "" && order != dr["sasOrder"].ToString())
                    {
                        break;
                    }
                    order = dr["sasOrder"].ToString();
                    quoteReady = System.Convert.ToBoolean(dr["sasQuoteReady"].ToString());
                    quoteSend = System.Convert.ToBoolean(dr["sasQuoteSend"].ToString());
                    step.Add(System.Convert.ToInt32(dr["sasSTSApprovalStepsID"].ToString()));
                    defaultApprover.Add(dr["sasDefaultApprover"].ToString());
                    //break;
                }
            }

            dr.Close();

            if (nextStep)
            {
                for (int i = 0; i < step.Count; i++)
                {
                    n.sendNotificaiton(quoteId, step[i], approved);
                }

                if (quoteReady)
                {
                    foreach (var s in step)
                    {
                        sql.CommandText = "insert into tblSTSQuoteStatus(sqsSTSQuoteID, sqsStepID, sqsAttemptNumber, sqsStepStartedDate, sqsStepFinishedDate, sqsApproved, sqsCreated, sqsCreatedBy) ";
                        sql.CommandText += "values (@quoteId, @step, @attempt, GETDATE(), GETDATE(), 1, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteId", quoteId);
                        sql.Parameters.AddWithValue("@step", s);
                        sql.Parameters.AddWithValue("@attempt", attempt);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "STS Notification");
                    }
                }
                else if ((bool)approved)
                {
                    int count = 0;
                    foreach (var s in step)
                    {
                        sql.CommandText = "insert into tblSTSQuoteStatus (sqsSTSQuoteID, sqsStepID, sqsApprovalTo, sqsAttemptNumber, sqsStepStartedDate, sqsCreated, sqsCreatedBy) ";
                        sql.CommandText += "values (@quoteId, @stepId, @approvalTo, @attempt, GETDATE(), GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteId", quoteId);
                        sql.Parameters.AddWithValue("@stepId", s);
                        sql.Parameters.AddWithValue("@approvalTo", defaultApprover[count]);
                        sql.Parameters.AddWithValue("@attempt", attempt);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "STS Notification");
                        count++;
                    }
                }
            }

            connection.Close();
        }
    }
}