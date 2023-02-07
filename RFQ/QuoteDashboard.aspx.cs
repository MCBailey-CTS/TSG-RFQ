using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFQ.Models;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;

namespace RFQ
{
    public partial class QuoteDashboard : System.Web.UI.Page
    {
        public List<QuoteItem> MasterList = new List<QuoteItem>();
        public List<ReservedItem> ReservedList = new List<ReservedItem>();
        public List<TSGCompany> CompanyList = new List<TSGCompany>();
        public List<Unreserved> UnreservedList = new List<Unreserved>();
        //List<string> partIDs = new List<string>();
        public Boolean OverDue = false;
        public Boolean HotList = true;
        SqlDataReader tsgCompanyDR;
        //int page = 0;

        public int pageIndex
        {
            get
            {
                if (ViewState["pageIndex"] != null)
                    return System.Convert.ToInt32(ViewState["pageIndex"].ToString());
                return 0;
            }
            set { ViewState["pageIndex"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            Site master = new RFQ.Site();
            //if(master.getCompanyId() != 1 && master.getCompanyId() != 2)
            //{
                //lblMessage.Text = "\n<script>$('#quoteUploadButton').hide();</script>";
            //}
            if (!IsPostBack)
            {
                SaveButton1.Visible = true;
                //SaveButton2.Visible = false;
                SaveButton3.Visible = false;
                chkReserved.Checked = true;
                dgReserved.Enabled = true;
                dgResults.Enabled = false;
                dgUnreserved.Enabled = false;
                gvUnReserved.Enabled = false;
                txtRFQ.Visible = false;

                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "Select TSGCompanyAbbrev, TSGCompanyID from TSGCompany where tcoActive = 1 ";
                SqlDataReader companyDr = sql.ExecuteReader();
                ddlCompany.DataSource = companyDr;
                ddlCompany.DataTextField = "TSGCompanyAbbrev";
                ddlCompany.DataValueField = "TSGCompanyId";
                ddlCompany.DataBind();
                companyDr.Close();
                ddlCompany.SelectedValue = master.getCompanyId().ToString();

                if (master.getUserName().ToLower() == "bpingle@toolingsystemsgroup.com" || master.getUserName().ToLower() == "rmumford@toolingsystemsgroup.com" || master.getUserName().ToLower() == "dparker@toolingsystemsgroup.com" || master.getUserName().ToLower() == "tberry@toolingsystemsgroup.com")
                {
                    sql.CommandText = "Select rfqID from tblRFQ where rfqStatus = 1 or rfqStatus = 2 or rfqStatus = 12 order by rfqID";
                    SqlDataReader rid = sql.ExecuteReader();
                    List<string> rfqs = new List<string>();
                    //ddlRFQID.Items.Add("All");
                    while (rid.Read())
                    {
                        rfqs.Add(rid.GetValue(0).ToString());
                    }
                    rid.Close();
                    //ddlRFQID.SelectedValue = "All";
                    ddlRFQID.Items.Add("All");
                    for(int i = 0; i < rfqs.Count; i++)
                    {
                        int numberOfParts = 0;
                        int numberOfPartsReserved = 0;
                        int numberOfPartsQuoted = 0;
                        int companiesNotified = 0;
                        int numberOfPartsNoQuoted = 0;
                        sql.CommandText = "select (Select count(*) from linkPartToRFQ where ptrRfqID = @rfqID), (Select count(ptqPartID) from linkQuoteToRFQ, linkPartToQuote where qtrRFQID = @rfqID and ptqQuoteID = qtrQuoteID), Count(distinct prcPartID), (Select count(distinct rtqCompanyID) - 1 from linkRFQToCompany where rtqRFQID = @rfqID), ";
                        sql.CommandText += "((Select count(distinct ppdPartID) from linkPartReservedToCompany, linkPartToPartDetail where prcRFQID = @rfqID and ppdPartToPartID = (Select distinct ppdPartToPartID from linkPartToPartDetail where ppdPartID = prcPartID)) ";
                        sql.CommandText += "+ (select count(distinct prcPartID) from linkPartReservedToCompany where prcRFQID = @rfqID and prcPartID not in (Select ppdPartID from linkPartToPartDetail))) from linkPartReservedToCompany where prcRFQID = @rfqID ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfqID", rfqs[i]);
                        SqlDataReader dr2 = sql.ExecuteReader();
                        if (dr2.Read())
                        {
                            numberOfParts = System.Convert.ToInt32(dr2.GetValue(0).ToString());
                            numberOfPartsReserved = System.Convert.ToInt32(dr2.GetValue(4).ToString());
                            numberOfPartsQuoted = System.Convert.ToInt32(dr2.GetValue(1).ToString());
                            companiesNotified = System.Convert.ToInt32(dr2.GetValue(3).ToString());
                        }
                        dr2.Close();

                        //List<string> partids = new List<string>();

                        //sql.CommandText = "Select prcPartID from linkPartReservedToCompany where prcRFQID = @rfqID";
                        //sql.Parameters.Clear();
                        //sql.Parameters.AddWithValue("@rfqID", rfqs[i]);
                        //dr2 = sql.ExecuteReader();
                        //while (dr2.Read())
                        //{
                        //    partids.Add(dr2.GetValue(0).ToString());
                        //}
                        //dr2.Close();

                        //for (int j = 0; j < partids.Count; j++)
                        //{
                        //    sql.CommandText = "Select (select count(*) from linkPartToPartDetail where ptp.ppdPartToPartID = ppdPartToPartID) from linkPartToPartDetail as ptp where ppdPartID = @partID";
                        //    sql.Parameters.Clear();
                        //    sql.Parameters.AddWithValue("@partID", partids[j]);
                        //    dr2 = sql.ExecuteReader();
                        //    while (dr2.Read())
                        //    {
                        //        numberOfPartsReserved = (System.Convert.ToInt32(dr2.GetValue(0).ToString()) - 1 + System.Convert.ToInt32(numberOfPartsReserved));
                        //    }
                        //    dr2.Close();
                        //}

                        sql.CommandText = "Select count(*) from tblNoQuote where nquRFQID = @rfqID group by nquPartID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfqID", rfqs[i]);
                        dr2 = sql.ExecuteReader();
                        while (dr2.Read())
                        {
                            if (System.Convert.ToInt32(dr2.GetValue(0).ToString()) >= companiesNotified)
                            {
                                numberOfPartsNoQuoted++;
                            }
                        }
                        dr2.Close();

                        if (numberOfParts > (numberOfPartsReserved + numberOfPartsNoQuoted) && numberOfParts > (numberOfPartsQuoted + numberOfPartsNoQuoted) && companiesNotified > 0)
                        {
                            ddlRFQID.Items.Add(rfqs[i]);
                        }
                    }
                }
                else
                {
                    sql.CommandText = "Select rfqID from tblRFQ where rfqStatus = 1 or rfqStatus = 2 or rfqStatus = 12";
                    SqlDataReader rid = sql.ExecuteReader();
                    ddlRFQID.Items.Add("All");
                    while (rid.Read())
                    {
                        ddlRFQID.Items.Add(rid.GetValue(0).ToString());
                    }
                    rid.Close();
                    ddlRFQID.SelectedValue = "All";
                }
                

                sql.CommandText = "Select qtyQuoteType, qtyQuoteTypeID from pktblQuoteType order by qtyQuoteType";
                SqlDataReader qt = sql.ExecuteReader();
                ddlQuoteType.Items.Add("All");
                while (qt.Read())
                {
                    ddlQuoteType.Items.Add(qt.GetValue(0).ToString());
                }
                qt.Close();
                ddlQuoteType.SelectedValue = "All";

                sql.CommandText = "Select qtyQuoteType, qtyQuoteTypeID from pktblQuoteType order by qtyQuoteType";
                SqlDataReader qouteType = sql.ExecuteReader();
                ddlQuoteType2.DataSource = qouteType;
                ddlQuoteType2.DataValueField = "qtyQuoteTypeID";
                ddlQuoteType2.DataTextField = "qtyQuoteType";
                ddlQuoteType2.DataBind();
                ddlQuoteType2.Items.Insert(0, new ListItem("Any", ""));
                ddlQuoteType2.SelectedIndex = 0;
                qouteType.Close();

                sql.CommandText = "select wlrWinLossReasonID, wlrWinLossReason from pktblWinLossReason";
                SqlDataReader wlDR = sql.ExecuteReader();
                ddlWinLossReason.DataSource = wlDR;
                ddlWinLossReason.DataValueField = "wlrWinLossReasonID";
                ddlWinLossReason.DataTextField = "wlrWinLossReason";
                ddlWinLossReason.DataBind();
                wlDR.Close();

                sql.CommandText = "Select wlsWinLossID, wlsWinLoss from pktblWinLoss";
                wlDR = sql.ExecuteReader();
                ddlWinLoss.DataSource = wlDR;
                ddlWinLoss.DataValueField = "wlsWinLossID";
                ddlWinLoss.DataTextField = "wlsWinLoss";
                ddlWinLoss.DataBind();
                wlDR.Close();

                sql.CommandText = "Select TSGSalesmanID, Name from TSGSalesman where tsaActive = 1 order by Name";
                sql.Parameters.Clear();
                wlDR = sql.ExecuteReader();
                ddlSalesman.DataSource = wlDR;
                ddlSalesman.DataValueField = "TSGSalesmanID";
                ddlSalesman.DataTextField = "Name";
                ddlSalesman.DataBind();
                wlDR.Close();
                ddlSalesman.Items.Insert(0, "Any");

                sql.CommandText = "select rstRFQStatus from pktblRFQStatus order by rstRFQStatus ";
                SqlDataReader st = sql.ExecuteReader();
                ddlStatus.Items.Add("All");
                while (st.Read())
                {
                    ddlStatus.Items.Add(st.GetValue(0).ToString());
                }
                st.Close();
                ddlStatus.SelectedValue = "All";
                connection.Close();
                createPage();
            }
        }

        protected void savePartNotes(Object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            int i = 0;
            foreach (GridViewRow row in dgReserved.Rows)
            {
                //string value1 = dgReserved.Rows[i].Cells[0].Text;
                //string value2 = row.Cells[1].Value.ToString();
                string txt = ((TextBox)dgReserved.Rows[i].FindControl("txtPartNote")).Text;
                string partID = dgReserved.Rows[i].Cells[3].Text;

                if (partID.Contains("A"))
                {
                    string assemblyId = partID.Split('A')[1];

                    sql.CommandText = "update tblAssembly set assNotes = @note where assAssemblyId = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@note", txt);
                    sql.Parameters.AddWithValue("@id", assemblyId);
                    master.ExecuteNonQuery(sql, "Quote Dashboard");
                }
                else
                {
                    sql.CommandText = "update tblPart set prtNote = @note, prtModified = GETDATE(), prtModifiedBy = @modified where prtPartID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@note", txt);
                    sql.Parameters.AddWithValue("@partID", partID);
                    sql.Parameters.AddWithValue("@modified", master.getUserName());

                    master.ExecuteNonQuery(sql, "Quote Dashboard");
                }

                i++;
            }

            connection.Close();
        }

        protected void ReservedChecked(object sender, EventArgs e)
        {
            if(chkReserved.Checked)
            {
                companiesNotified.Text = "";
                nextButton.Visible = false;
                chkOpenQuotes.Checked = false;
                chkUnreserved.Checked = false;
                chkUnReservedList.Checked = false;
                //chkDisposition.Checked = false;
                SaveButton1.Visible = true;
                //SaveButton2.Visible = false;
                SaveButton3.Visible = false;
                ddlRFQID.SelectedIndex = 0;
                createPage();

                dgReserved.Enabled = true;

                dgResults.Enabled = false;
                dgResults.DataSource = null;
                dgResults.DataBind();
                dgUnreserved.Enabled = false;
                dgUnreserved.DataSource = null;
                dgUnreserved.DataBind();
                dgDisposition.Enabled = false;
                dgDisposition.DataSource = null;
                dgDisposition.DataBind();
                gvUnReserved.Visible = false;
                gvUnReserved.Enabled = false;
                gvUnReserved.DataSource = null;
                gvUnReserved.DataBind();
                txtRFQ.Visible = false;
                ddlRFQID.Visible = true;
            }
        }

        protected void OpenChecked(object sender, EventArgs e)
        {
            if(chkOpenQuotes.Checked)
            {
                companiesNotified.Text = "";
                nextButton.Visible = false;
                chkUnreserved.Checked = false;
                chkReserved.Checked = false;
                chkUnReservedList.Checked = false;
                //chkDisposition.Checked = false;
                SaveButton1.Visible = false;
                //SaveButton2.Visible = true;
                SaveButton3.Visible = false;
                ddlRFQID.SelectedIndex = 0;
                createPage();

                dgResults.Enabled = true;

                dgReserved.Enabled = false;
                dgReserved.DataSource = null;
                dgReserved.DataBind();
                dgUnreserved.Enabled = false;
                dgUnreserved.DataSource = null;
                dgUnreserved.DataBind();
                dgDisposition.Enabled = false;
                dgDisposition.DataSource = null;
                dgDisposition.DataBind();
                gvUnReserved.Visible = false;
                gvUnReserved.Enabled = false;
                gvUnReserved.DataSource = null;
                gvUnReserved.DataBind();
                txtRFQ.Visible = false;
                ddlRFQID.Visible = true;
            }
        }

        protected void UnreservedChecked(object sender, EventArgs e)
        {
            if(chkUnreserved.Checked)
            {
                nextButton.Visible = true;
                chkReserved.Checked = false;
                chkOpenQuotes.Checked = false;
                chkUnReservedList.Checked = false;
                //chkDisposition.Checked = false;
                SaveButton1.Visible = false;
                //SaveButton2.Visible = false;
                SaveButton3.Visible = true;
                ddlRFQID.SelectedIndex = 1;
                createPage();

                dgUnreserved.Enabled = true;

                dgReserved.Enabled = false;
                dgReserved.DataSource = null;
                dgReserved.DataBind();
                dgResults.Enabled = false;
                dgResults.DataSource = null;
                dgResults.DataBind();
                dgDisposition.Enabled = false;
                dgDisposition.DataSource = null;
                dgDisposition.DataBind();
                gvUnReserved.Visible = false;
                gvUnReserved.Enabled = false;
                gvUnReserved.DataSource = null;
                gvUnReserved.DataBind();
                txtRFQ.Visible = false;
                ddlRFQID.Visible = true;
            }
        }

        protected void chkUnReservedList_CheckedChanged(object sender, EventArgs e)
        {
            if(chkUnReservedList.Checked)
            {
                ddlCompany.SelectedValue = "1";
                companiesNotified.Text = "";
                nextButton.Visible = false;
                chkReserved.Checked = false;
                chkOpenQuotes.Checked = false;
                chkUnreserved.Checked = false;
                SaveButton1.Visible = false;
                SaveButton3.Visible = false;
                ddlRFQID.SelectedIndex = 0;
                createPage();

                gvUnReserved.Visible = true;
                gvUnReserved.Enabled = true;

                dgReserved.Enabled = false;
                dgReserved.DataSource = null;
                dgReserved.DataBind();
                dgResults.Enabled = false;
                dgResults.DataSource = null;
                dgResults.DataBind();
                dgUnreserved.Enabled = false;
                dgUnreserved.DataSource = null;
                dgUnreserved.DataBind();
                dgDisposition.Enabled = false;
                dgDisposition.DataSource = null;
                dgDisposition.DataBind();

                txtRFQ.Visible = true;
                ddlRFQID.Visible = false;
            }
        }

        protected void next(object sender, EventArgs e)
        {
            try
            {
                ddlRFQID.SelectedIndex = ddlRFQID.SelectedIndex + 1;
            }
            catch
            {

            }

            createPage();
        }

        protected void populateNotes()
        {
            //Site master = new Site();
            //SqlConnection connection = new SqlConnection(master.getConnectionString());
            //connection.Open();
            //SqlCommand sql = new SqlCommand();
            //sql.Connection = connection;

            //SqlDataReader dr;

            //foreach (GridViewRow row in dgReserved.Rows)
            //{
            //    sql.CommandText = "Select prtNote, prtPartID from tblPart where prtPartID = @partID";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@partID", row.Cells[4].Text);

            //    dr = sql.ExecuteReader();

            //    if (dr.Read())
            //    {
            //        if(reservedIDs.Contains(dr.GetValue(1).ToString())) {
            //            string test = dr.GetValue(0).ToString();
            //            ((TextBox)row.FindControl("txtPartNote")).Text = dr.GetValue(0).ToString();
            //        }
            //    }
            //    dr.Close();
            //}

            //foreach (GridViewRow row in dgResults.Rows)
            //{
            //    sql.CommandText = "Select prtNote from tblPart where prtPartID = @partID";
            //    sql.Parameters.Clear();
            //    string[] words = row.Cells[5].Text.Split(',');
            //    sql.Parameters.AddWithValue("@partID", words[0]);

            //    try
            //    {
            //        dr = sql.ExecuteReader();

            //        if (dr.Read())
            //        {
            //            ((TextBox)row.FindControl("txtPartNote")).Text = dr.GetValue(0).ToString();
            //        }
            //        dr.Close();
            //    }
            //    catch
            //    {

            //    }

            //    //if(row.Cells[12].Text == "")
            //    //{
            //    //    row.Cells[1].Text = "" + row.Cells[1].Text;
            //    //}
            //}
            //connection.Close();
        }

        

        protected void colorDates(GridView gv, int dueDateIndex)
        {
            //foreach (GridViewRow row in gv.Rows)
            //{
            //    try
            //    {
            //        DateTime dueDate = System.Convert.ToDateTime(row.Cells[dueDateIndex].Text);

            //        if (dueDate < DateTime.Now.ToUniversalTime())
            //        {
            //            row.Cells[dueDateIndex].BackColor = Color.Red;
            //        }
            //        else if (dueDate > DateTime.Now.ToUniversalTime() && dueDate < DateTime.Now.ToUniversalTime().AddDays(7))
            //        {
            //            row.Cells[dueDateIndex].BackColor = Color.Yellow;
            //        }
            //        else
            //        {
            //            row.Cells[dueDateIndex].BackColor = Color.LawnGreen;
            //        }
            //        //string value2 = row.Cells[1].Value.ToString();
            //    }
            //    catch
            //    {

            //    }
            //}
        }

        protected void reserve(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            int i = 0;
            foreach (GridViewRow row in dgUnreserved.Rows)
            {
                string value1 = dgUnreserved.Rows[i].Cells[0].Text;
                //string value2 = row.Cells[1].Value.ToString();
                //string txt = ((TextBox)dgUnreserved.Rows[i].FindControl("txtPartNote")).Text;
                string partID = dgUnreserved.Rows[i].Cells[3].Text;

                //sql.CommandText = "update tblPart set prtNote = @note, prtModified = GETDATE(), prtModifiedBy = @modified where prtPartID = @partID";
                //sql.Parameters.Clear();
                //sql.Parameters.AddWithValue("@note", txt);
                //sql.Parameters.AddWithValue("@partID", partID);
                //sql.Parameters.AddWithValue("@modified", master.getUserID());

                //master.ExecuteNonQuery(sql, "Quote Dashboard");

                string companyRes = ((DropDownList)dgUnreserved.Rows[i].FindControl("ddlReserve")).SelectedValue;
                if (companyRes != "Res")
                {
                    litReserve.Text += "<script>url = 'processNoQUote.aspx?rfq=" + dgUnreserved.Rows[i].Cells[1].Text + "&reserve=yes&remove=no&applies=" + partID + "&company=" + companyRes + "&rand=' + Math.random();";
                    litReserve.Text += "$.ajax({ url: url , success: function(data) { }})</script>";
                }

                string noQuoteReason = ((DropDownList)dgUnreserved.Rows[i].FindControl("ddlNoQuote")).SelectedValue;
                if(noQuoteReason != "No Quote")
                {
                    processNoQuote pnq = new processNoQuote();
                    sql.CommandText = "Select cnoTSGCompanyID, TSGCompanyAbbrev from tblCompanyNotified, TSGCompany where cnoRFQID = @rfqID and TSGCompanyID = cnoTSGCompanyID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", ddlRFQID.SelectedValue);
                    List<string> companies = new List<string>();
                    SqlDataReader dr = sql.ExecuteReader();
                    while(dr.Read())
                    {
                        if(companyRes != dr.GetValue(1).ToString() && dr.GetValue(0).ToString() != "1")
                        {
                            pnq.ApplyNoQuote(dgUnreserved.Rows[i].Cells[1].Text, partID, noQuoteReason.Split('-')[0].Trim(), dr.GetValue(0).ToString());
                        }
                    }
                    dr.Close();
                }

                i++;
            }
            i = 0;
            foreach (GridViewRow row in dgUnreserved.Rows)
            {
                //string value1 = dgUnreserved.Rows[i].Cells[0].Text;
                //string value2 = row.Cells[1].Value.ToString();
                string txt = ((TextBox)dgUnreserved.Rows[i].FindControl("txtPartNote")).Text;
                string partID = dgUnreserved.Rows[i].Cells[3].Text;

                sql.CommandText = "update tblPart set prtNote = @note, prtModified = GETDATE(), prtModifiedBy = @modified where prtPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@note", txt);
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@modified", master.getUserID());

                master.ExecuteNonQuery(sql, "Quote Dashboard");
                i++;
            }


            connection.Close();
            System.Threading.Thread.Sleep(100);
            ddlRFQID.SelectedIndex = ddlRFQID.SelectedIndex + 1;
            
            createPage();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Only selecting companies that are going to quote it
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;
                sql.CommandText = "Select TSGCompanyAbbrev, TSGCompanyID from TSGCompany where TSGCompanyID = 2 or TSGCompanyID = 3 or TSGCompanyID = 5  or TSGCompanyID = 7 or TSGCompanyID = 8 or TSGCompanyID = 9 or TSGCompanyID = 12";
                tsgCompanyDR = sql.ExecuteReader();
                
                //Find the DropDownList in the Row
                DropDownList ddlReserve = (e.Row.FindControl("ddlReserve") as DropDownList);
                ddlReserve.DataSource = tsgCompanyDR;
                ddlReserve.DataTextField = "TSGCompanyAbbrev";
                ddlReserve.DataValueField = "TSGCompanyID";
                ddlReserve.DataBind();

                //Add Default Item in the DropDownList
                ddlReserve.Items.Insert(0, new ListItem("Res"));
                tsgCompanyDR.Close();

                DropDownList ddlNoQuote = (e.Row.FindControl("ddlNoQuote") as DropDownList);
                sql.CommandText = "select nqrNoQuoteReasonID, nqrNoQuoteReason, nqrNoQuoteReasonNumber from pktblNoQuoteReason where nqrActive = 1 order by nqrNoQuoteReasonNumber";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    ddlNoQuote.Items.Add(new System.Web.UI.WebControls.ListItem((dr.GetValue(2).ToString() + " - " + dr.GetValue(1).ToString()), dr.GetValue(0).ToString()));
                }
                ddlNoQuote.Items.Insert(0, new ListItem("No Quote"));
                dr.Close();


                connection.Close();
            }
        }

        protected void createPage()
        {
            //Make it so it only displays for that company or if you are TSG then everything
            populateList();

            dgReserved.DataSource = null;
            dgReserved.DataBind();
            dgResults.DataSource = null;
            dgResults.DataBind();
            dgUnreserved.DataSource = null;
            dgUnreserved.DataBind();

            DataTable dt = new DataTable();

            Site master = new RFQ.Site();
            List<ReservedItem> resList = new List<ReservedItem>();
            foreach (ReservedItem item in ReservedList)
            {
                if ((ddlCompany.SelectedValue == "1") || (item.tsgCompanyNum.ToString() == ddlCompany.SelectedValue))
                {
                    if ((txtCustomer.Text.Trim() == "") || (item.customer.ToUpper().Contains(txtCustomer.Text.ToUpper().Trim())))
                    {
                        if ((ddlRFQID.SelectedValue.Trim() == "All") || (item.rfqID.ToUpper().Contains(ddlRFQID.SelectedValue.ToUpper().Trim())))
                        {

                            resList.Add(item);
                        }
                    }
                }
            }


            List<QuoteItem> quoteList = new List<QuoteItem>();
            foreach (QuoteItem item in MasterList)
            {
                if ((ddlCompany.SelectedValue == "1") || (item.tsgCompanyNum.ToString() == ddlCompany.SelectedValue))
                {
                    if ((ddlStatus.SelectedValue == "All") || (item.status == ddlStatus.SelectedValue))
                    {
                        if ((txtCustomer.Text.Trim() == "") || (item.customer.ToUpper().Contains(txtCustomer.Text.ToUpper().Trim())))
                        {
                            if ((ddlRFQID.SelectedValue.Trim() == "All") || (item.rfqID.ToUpper().Contains(ddlRFQID.SelectedValue.ToUpper().Trim())))
                            {
                                if((ddlQuoteType.SelectedValue == "All") || (item.quoteType == (ddlQuoteType.SelectedValue)))
                                {
                                    if (txtQuoteNumber.Text == "" || item.quoteID.Contains(txtQuoteNumber.Text))
                                    {
                                        quoteList.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            List<Unreserved> unresList = new List<Unreserved>();
            foreach (Unreserved item in UnreservedList)
            {
                if ((txtCustomer.Text.Trim() == "") || (item.customer.ToUpper().Contains(txtCustomer.Text.ToUpper().Trim())))
                {
                    if ((ddlRFQID.SelectedValue.Trim() == "") || (item.rfqID.ToUpper().Contains(ddlRFQID.SelectedValue.ToUpper().Trim())))
                    {
                        unresList.Add(item);
                    }
                }
            }


            //if(!chkDisposition.Checked)
            //{
            dgResults.DataSource = quoteList;
            dgResults.DataBind();
            //}
            //else
            //{
            //    dgDisposition.DataSource = quoteList;
            //    dgDisposition.DataBind();
            //}

            dgReserved.DataSource = resList;
            dgReserved.DataBind();

            dgUnreserved.DataSource = unresList;
            dgUnreserved.DataBind();

            

            //colorDates(dgReserved, 10);
            //colorDates(dgResults, 12);
            //colorDates(dgUnreserved, 8);
            

            //populateNotes();
        }

        protected void OnPaging(object sender, GridViewPageEventArgs e)
        {
            pageIndex = e.NewPageIndex;
            gvUnReserved.PageIndex = pageIndex;
            populateList();
        }

        protected void populateList()
        {
            // create master list of RFQs
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            SqlCommand sql2 = new SqlCommand();
            sql2.Connection = connection2;
            List<string> quoteIDs = new List<string>();

            string quoteidDebug = "";
            string ECQuoteDebug = "";

            dgReserved.DataSource = null;
            dgReserved.DataBind();
            dgResults.DataSource = null;
            dgResults.DataBind();
            dgUnreserved.DataSource = null;
            dgUnreserved.DataBind();

            long userCompanyID = master.getCompanyId();
            string userCompanyAbbrev = "";
            sql.Parameters.Clear();
            sql.CommandText = "Select TSGCompanyAbbrev from TSGCompany where @usercompany = TSGCompanyID";
            sql.Parameters.AddWithValue("@usercompany", userCompanyID);

            SqlDataReader dr1 = sql.ExecuteReader();
            while (dr1.Read())
            {
                userCompanyAbbrev = dr1.GetValue(0).ToString();
            };

            dr1.Close();
            sql.Parameters.Clear();

            if (chkOpenQuotes.Checked)
            {
                sql.Parameters.Clear();
                sql.CommandText = "Select quoQuoteID, TSGCompanyAbbrev, quoRFQID, estFirstName, estLastName, qstQuoteStatus, CustomerName, quoCreated, quoTSGCompanyID, prtPartNumber, prtPARTID, rfqDueDate, prtPicture, prtNote, prtRFQLineNumber, quoVersion ";
                sql.CommandText += "from tblQuote, linkQuoteToRFQ, tblRFQ, Customer, pktblEstimators, TSGCompany, linkPartToQuote, tblPart, pktblQuoteStatus ";
                sql.CommandText += "where qtrQuoteID = quoQuoteID and qtrRFQID = rfqID and rfqCustomerID = CustomerID and TSGCompanyID = quoTSGCompanyID and quoEstimatorID = estEstimatorID and ptqSTS = 0 and ptqHTS = 0 and ptqUGS = 0 and ";
                if(ddlCompany.SelectedValue != "1")
                {
                    sql.CommandText += "quoTSGCompanyID = @company and ";
                    sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                }
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "rfqSalesman = @salesman and ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                sql.CommandText += "ptqQuoteID = quoQuoteID and ptqPartID = prtPARTID and quoStatusID = qstQuoteStatusID and quoStatusID <> 3 and quoStatusID <> 7 and quoStatusID <> 8 and (quoCreated > DATEADD(MONTH, -2, GETDATE()) or quoModified > DATEADD(Month, -2, GETDATE())) ";
                if(chkDueDate.Checked)
                {
                    sql.CommandText += "order by rfqDueDate, TSGCompanyID, quoRFQID desc, prtPARTID";
                }
                else
                {
                    sql.CommandText += "order by TSGCompanyID, quoRFQID desc, prtPARTID";
                }

                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    //partIDs.Add(dr.GetValue(10).ToString());
                    if (!quoteIDs.Contains(dr.GetValue(0).ToString()))
                    {
                        quoteIDs.Add(dr.GetValue(0).ToString());
                        QuoteItem newItem = new QuoteItem();
                        newItem.quoteID = dr.GetValue(2).ToString() + "-" + dr.GetValue(14).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(15).ToString();
                        //sql2.CommandText = "Select prtPartID, prtPartNumber from linkPartToQuote, tblPart where ptqQuoteID = @quoteID and ptqPartID = prtPARTID";
                        //sql2.Parameters.Clear();
                        //sql2.Parameters.AddWithValue("@quoteID", dr.GetValue(0).ToString());
                        //SqlDataReader dr2 = sql2.ExecuteReader();
                        //int i = 0;
                        //while (dr2.Read())
                        //{
                        //    if (i != 0)
                        //    {
                        //        newItem.partNumber += ",\n";
                        //        newItem.partID += ", ";
                        //    }
                        //    newItem.partID += dr2.GetValue(0).ToString();
                        //    newItem.partNumber += dr2.GetValue(1).ToString();
                        //    i++;
                        //}
                        //dr2.Close();

                        newItem.partNumber = dr.GetValue(9).ToString();
                        newItem.partID = dr.GetValue(10).ToString();
                        newItem.tsgCompany = dr.GetValue(1).ToString();
                        newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr.GetValue(2).ToString() + "'>Edit RFQ</a>";
                        newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                        newItem.status = dr.GetValue(5).ToString();
                        newItem.customer = dr.GetValue(6).ToString();

                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + dr.GetValue(0).ToString() + "&quoteType=2" + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                        newItem.quoteType = "NEW TOOL";
                        newItem.quoteTypeNum = 2;
                        try
                        {
                            newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                        }
                        catch
                        {

                        }
                        newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                        //newItem.partNumber = dr.GetValue(9).ToString();
                        //newItem.partID = dr.GetValue(10).ToString();
                        //partIDs.Add(newItem.partID);
                        newItem.dueDate = System.Convert.ToDateTime(dr.GetValue(11)).ToString("d");
                        newItem.url = "<a href='https://tsgrfq.azurewebstites.net/EditRFQ?id=" + newItem.quoteID + "'>RFQ";
                        newItem.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(12).ToString() + "'>Picture</a>";
                        newItem.partNote = dr.GetValue(13).ToString();
                        MasterList.Add(newItem);
                    }
                }
                dr.Close();
                sql.Parameters.Clear();

                sql.CommandText = "Select ecqECQuoteID, TSGCompanyAbbrev, ecqRFQNumber, estFirstName, estLastName, qstQuoteStatus, CustomerName, ecqCreated, ecqTSGCompanyID, ecqPartNumber, ecqQuoteNumber, ecqVersion from tblECQuote, Customer, pktblEstimators, TSGCompany, pktblQuoteStatus ";
                sql.CommandText += "where ecqCustomer = CustomerID and ecqEstimator = estEstimatorID and TSGCompanyID = ecqTSGCompanyID and ecqStatus = qstQuoteStatusID and ecqStatus <> 3 and ecqStatus <> 7 and ecqStatus <> 8 and ecqCreated > DATEADD(Month, -2, GETDATE()) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and ecqSalesmanID = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                sql.CommandText += "order by ecqQuoteNumber, ecqVersion";

                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    QuoteItem newItem = new QuoteItem();
                    newItem.quoteID = dr.GetValue(10).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11);
                    newItem.tsgCompany = dr.GetValue(1).ToString();
                    newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                    newItem.status = dr.GetValue(5).ToString();
                    newItem.customer = dr.GetValue(6).ToString();
                    if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                    {
                        newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + dr.GetValue(0).ToString() + "&quoteType=1" + "'>Edit Quote</a>";
                    }
                    else
                    {
                        newItem.realQuoteID = "";
                    }
                    newItem.quoteType = "E/C";
                    newItem.quoteTypeNum = 1;
                    try
                    {
                        newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                    newItem.partNumber = dr.GetValue(9).ToString();
                    newItem.url = "";
                    MasterList.Add(newItem);
                }
                dr.Close();

                //sql.CommandText = "Select ncqquoteID, ncqCustomerRFQ, estFirstName, estLastName, CustomerName, ncqCreated,  ncqPartNumber, ncqQuotationNumber, ncqVersion from tblNcQuote, Customer, pktblEstimators ";
                //sql.CommandText += "where ncqCustomer = CustomerID and ncqCreatedBy = estEmail and ecqCreated > DATEADD(Month, -2, GETDATE()) ";
                //if (ddlSalesman.SelectedItem.ToString() != "Any")
                //{
                //    sql.CommandText += "and ecqSalesmanID = @salesman ";
                //    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                //}
                //sql.CommandText += "order by ncqQuotationNumber, ncqVersion";

                //dr = sql.ExecuteReader();

                //while (dr.Read())
                //{
                //    QuoteItem newItem = new QuoteItem();
                //    newItem.quoteID = dr.GetValue(10).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11);
                //    newItem.tsgCompany = dr.GetValue(1).ToString();
                //    newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                //    newItem.status = dr.GetValue(5).ToString();
                //    newItem.customer = dr.GetValue(6).ToString();
                //    newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + dr.GetValue(0).ToString() + "&quoteType=1" + "'>Edit Quote</a>";
                //    newItem.quoteType = "E/C";
                //    newItem.quoteTypeNum = 1;
                //    try
                //    {
                //        newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                //    }
                //    catch
                //    {

                //    }
                //    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                //    newItem.partNumber = dr.GetValue(9).ToString();
                //    newItem.url = "";
                //    MasterList.Add(newItem);
                //}
                //dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select hquHTSQuoteID, 'HTS', hquRFQID, estFirstName, estLastName, qstQuoteStatus, CustomerName, hquCreated, 9, hquPartNumbers, ptqPartID, hquVersion, prtRFQLineNumber, hquNumber ";
                sql.CommandText += "from pktblEstimators, pktblQuoteStatus, Customer, tblHTSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = hquHTSQuoteID and ptqHTS = 1 ";
                sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                sql.CommandText += "where hquEstimatorID = estEstimatorID and hquCustomerID = CustomerID and hquStatusID = qstQuoteStatusID and hquCreated > DATEADD(MONTH, -2, GETDATE()) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and hquSalesman = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                sql.CommandText += "order by hquHTSQuoteID desc ";
                
                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    QuoteItem newItem = new QuoteItem();
                    if(dr.GetValue(12).ToString() != "")
                    {
                        newItem.quoteID = dr.GetValue(2).ToString() + "-" + dr.GetValue(12).ToString() + "-HTS-" + dr.GetValue(11).ToString();
                    }
                    else if (dr.GetValue(13).ToString() != "")
                    {
                        newItem.quoteID = dr.GetValue(13).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11).ToString();
                    }
                    else
                    {
                        newItem.quoteID = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11).ToString();
                    }
                    newItem.tsgCompany = dr.GetValue(1).ToString();
                    newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                    newItem.status = dr.GetValue(5).ToString();
                    newItem.customer = dr.GetValue(6).ToString();
                    if(dr.GetValue(10).ToString() != "" && dr.GetValue(10).ToString() != null)
                    {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr.GetValue(2).ToString() + "&partID=" + dr.GetValue(10).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                        newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr.GetValue(2).ToString() + "'>Edit RFQ</a>";
                    }
                    else
                    {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + dr.GetValue(0).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                    }
                    newItem.quoteType = "Hot Stamp";
                    newItem.quoteTypeNum = 3;
                    try
                    {
                        newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                    newItem.partNumber = dr.GetValue(9).ToString();
                    newItem.url = "";
                    MasterList.Add(newItem);
                }
                dr.Close();


                sql.Parameters.Clear();
                sql.CommandText = "Select squSTSQuoteID, 'STS', squRFQNum, estFirstName, estLastName, qstQuoteStatus, CustomerName, squCreated, 13, squPartNumber, ptqPartID, squQuoteVersion, prtRFQLineNumber, prtRFQLineNumber, squQuoteNumber, squQuoteVersion, squECQuote, squECBaseQuoteId, assAssemblyId, qtrRFQID, assLineNumber ";
                sql.CommandText += "from pktblEstimators, pktblQuoteStatus, Customer, tblSTSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteId = squSTSQuoteId and qtrSTS = 1 ";
                sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = squSTSQuoteId ";
                sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                sql.CommandText += "where squEstimatorID = estEstimatorID and squCustomerID = CustomerID and squStatusID = qstQuoteStatusID and squCreated > DATEADD(MONTH, -12, GETDATE()) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and squSalesmanID = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                sql.CommandText += "order by squSTSQuoteID desc ";

                dr = sql.ExecuteReader();

                string quoteId = "";

                while (dr.Read())
                {
                    if (quoteId != dr["squSTSQuoteId"].ToString())
                    {
                        quoteId = dr["squSTSQuoteId"].ToString();
                    }
                    else
                    {
                        continue;
                    }
                    QuoteItem newItem = new QuoteItem();
                    if (dr["qtrRFQID"].ToString() != "")
                    {
                        // BD - Add EC to quoteID
                        ECQuoteDebug = dr["squECQuote"].ToString();
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            if (dr["assLineNumber"].ToString() != "")
                            {
                                newItem.quoteID = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-EC-" + dr["squQuoteVersion"].ToString();
                            }
                            else
                            {
                                newItem.quoteID = dr["qtrRFQID"].ToString() + "-" + dr.GetValue(12).ToString() + "-STS-EC-" + dr.GetValue(11).ToString();
                            }
                        }
                        else
                        {
                            if (dr["assLineNumber"].ToString() != "")
                            {
                                newItem.quoteID = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                            else
                            {
                                newItem.quoteID = dr["qtrRFQID"].ToString() + "-" + dr.GetValue(12).ToString() + "-STS-" + dr.GetValue(11).ToString();
                            }
                        }
                    }
                    else
                    {
                        if(dr.GetValue(14).ToString() != "")
                        {
                            // BD - Add EC to quoteID
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                newItem.quoteID = dr.GetValue(14).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-EC-" + dr.GetValue(15).ToString();
                            }
                            else
                            {
                                newItem.quoteID = dr.GetValue(14).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(15).ToString();
                            }
                        }
                        else
                        {
                            // BD - Add EC to quoteID
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                newItem.quoteID = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-EC-" + dr.GetValue(15).ToString();
                            }
                            else
                            {
                                newItem.quoteID = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(15).ToString();
                            }
                        }
                    }

                    quoteidDebug = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-EC-" + dr["squQuoteVersion"].ToString();
                    newItem.tsgCompany = dr.GetValue(1).ToString();
                    newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                    newItem.status = dr.GetValue(5).ToString();
                    newItem.customer = dr.GetValue(6).ToString();
                    if (dr.GetValue(10).ToString() != "" && dr.GetValue(10).ToString() != null)
                    {
                        //newItem.realQuoteID = "<a href='http://localhost:52154//STSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr.GetValue(10).ToString() + "'>Edit Quote</a>";
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&partID=" + dr.GetValue(10).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                        newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr["qtrRFQID"].ToString() + "'>Edit RFQ</a>";
                    }
                    else if (dr["assAssemblyId"].ToString() != "") {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr["qtrRFQID"].ToString() + "&assemblyId=" + dr["assAssemblyId"].ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                        newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr["qtrRFQID"].ToString() + "'>Edit RFQ</a>";
                    }
                    else
                    {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + dr.GetValue(0).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                    }
                    newItem.quoteType = "STS";
                    newItem.quoteTypeNum = 3;
                    try
                    {
                        newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                    newItem.partNumber = dr.GetValue(9).ToString();
                    newItem.url = "";
                    MasterList.Add(newItem);
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select uquUGSQuoteID, 'UGS', uquRFQID, estFirstName, estLastName, qstQuoteStatus, CustomerName, uquCreated, 15, uquPartNumber, ptqPartID, uquQuoteVersion, prtRFQLineNumber ";
                sql.CommandText += "from pktblEstimators, pktblQuoteStatus, Customer, tblUGSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                sql.CommandText += "where uquEstimatorID = estEstimatorID and uquCustomerID = CustomerID and uquStatusID = qstQuoteStatusID and uquCreated > DATEADD(MONTH, -2, GETDATE()) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and uquSalesmanID = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                sql.CommandText += "order by uquUGSQuoteID desc ";
                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    QuoteItem newItem = new QuoteItem();

                    newItem.quoteID = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11).ToString();
                    if (dr.GetValue(12).ToString() != "")
                    {
                        newItem.quoteID = dr.GetValue(2).ToString() + "-" + dr.GetValue(12).ToString() + "-UGS-" + dr.GetValue(11).ToString();
                    }
                    else
                    {
                        newItem.quoteID = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11).ToString();
                    }
                    newItem.tsgCompany = dr.GetValue(1).ToString();
                    newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
                    newItem.status = dr.GetValue(5).ToString();
                    newItem.customer = dr.GetValue(6).ToString();
                    if (dr.GetValue(10).ToString() != "" && dr.GetValue(10).ToString() != null)
                    {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/UGSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr.GetValue(2).ToString() + "&partID=" + dr.GetValue(10).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                        newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr.GetValue(2).ToString() + "'>Edit RFQ</a>";
                    }
                    else
                    {
                        if ((newItem.tsgCompany == userCompanyAbbrev) || (userCompanyAbbrev == "TSG"))
                        {
                            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/UGSEditQuote?id=" + dr.GetValue(0).ToString() + "'>Edit Quote</a>";
                        }
                        else
                        {
                            newItem.realQuoteID = "";
                        }
                    }
                    newItem.quoteType = "UGS";
                    newItem.quoteTypeNum = 3;
                    try
                    {
                        newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
                    newItem.partNumber = dr.GetValue(9).ToString();
                    newItem.url = "";
                    MasterList.Add(newItem);
                }
                dr.Close();
            }
            else if(chkReserved.Checked)
            {
                sql.Parameters.Clear();
                sql.CommandText = "Select prcRFQID, prtPartNumber, TSGCompanyAbbrev, CustomerName, prcCreatedBy, prcCreated, prcTSGCompanyID, prtPARTID, rfqDueDate, prtPicture, prtNote, ShipToName ";
                sql.CommandText += "from linkPartReservedToCompany, tblRFQ, TSGCompany, Customer, tblPart, CustomerLocation ";
                sql.CommandText += "where rfqID = prcRFQID and prcTSGCompanyID = TSGCompanyID and Customer.CustomerID = rfqCustomerID and prtPARTID = prcPartID ";
                sql.CommandText += "and not exists (Select * from linkPartToQuote, tblQuote where ptqPartID = prtPARTID and ptqQuoteID = quoQuoteID and quoTSGCompanyID = prcTSGCompanyID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0) and CustomerLocationID = rfqPlantID ";
                sql.CommandText += "and not exists (Select * from linkPartToQuote, tblQuote where ptqPartID = prtPARTID and ptqQuoteID = quoQuoteID and quoTSGCompanyID = prcTSGCompanyID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0) and ";
                sql.CommandText += "not exists (Select * from linkPartToQuote where ptqPartID = prtPARTID and(prcTSGCompanyID = 9 or prcTSGCompanyID = 13 or prcTSGCompanyID = 15) and(ptqHTS = 1 or ptqSTS = 1 or ptqUGS = 1)) ";
                sql.CommandText += "and CustomerLocationID = rfqPlantID ";
                sql.CommandText += "and (rfqStatus = 2 or rfqStatus = 1 or rfqStatus = 12) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and rfqSalesman = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                if (ddlCompany.SelectedValue != "1")
                {
                    sql.CommandText += "and TSGCompanyID = @company ";
                    sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                }
                if(chkDueDate.Checked)
                {
                    sql.CommandText += "order by rfqDueDate asc, prcTSGCompanyID, prcRFQID desc, prtPARTID ";
                }
                else
                {
                    sql.CommandText += "order by prcTSGCompanyID, prcRFQID desc, prtPARTID";
                }

                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    ReservedItem newItem = new ReservedItem();
                    newItem.rfqID = dr.GetValue(0).ToString();
                    newItem.partNumber = dr.GetValue(1).ToString();
                    newItem.tsgCompany = dr.GetValue(2).ToString();

                    newItem.customer = dr.GetValue(3).ToString() + "<br />" + dr.GetValue(11).ToString();
                    newItem.reservedBy = dr.GetValue(4).ToString();
                    int index = newItem.reservedBy.IndexOf("@");
                    if (index > 0)
                    {
                        newItem.reservedBy = newItem.reservedBy.Substring(0, index);
                    }
                    try
                    {
                        newItem.reserved = System.Convert.ToDateTime(dr.GetValue(5)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(6).ToString());
                    newItem.partID = dr.GetValue(7).ToString();
                    newItem.dueDate = System.Convert.ToDateTime(dr.GetValue(8)).ToString("d");
                    newItem.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(9).ToString() + "'>Picture";
                    newItem.partNote = dr.GetValue(10).ToString();
                    ReservedList.Add(newItem);
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select rasRfqId, assNumber, TSGCompanyAbbrev, c.CustomerName, rasCreatedBy, rasCreated, rasCompanyId, assAssemblyId, rfqDueDate, assPicture, cl.ShipToName, TSGCompanyId, assNotes ";
                sql.CommandText += "from tblRFQ ";
                sql.CommandText += "inner join Customer c on c.CustomerId = rfqCustomerId ";
                sql.CommandText += "inner join CustomerLocation cl on cl.CustomerLocationID = rfqPlantID ";
                sql.CommandText += "inner join tblReserveAssembly on rasRfqId = rfqID ";
                sql.CommandText += "inner join tblAssembly on assAssemblyId = rasAssemblyId ";
                sql.CommandText += "inner join TSGCompany on TSGCompanyID = rasCompanyId ";
                sql.CommandText += "where TSGCompanyID = 13 and not exists (Select * from linkAssemblyToQuote where atqAssemblyId = assAssemblyId) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and rfqSalesman = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                List<string> assemblies = new List<string>();
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (assemblies.Contains(dr["assAssemblyId"].ToString()))
                    {
                        continue;
                    }
                    assemblies.Add(dr["assAssemblyId"].ToString());
                    ReservedItem newItem = new ReservedItem();
                    newItem.rfqID = dr["rasRfqId"].ToString();
                    newItem.partNumber = dr["assNumber"].ToString();
                    newItem.tsgCompany = dr["TSGCompanyAbbrev"].ToString();
                    newItem.customer = dr["CustomerName"].ToString();
                    newItem.reservedBy = dr["rasCreatedBy"].ToString();
                    int index = newItem.reservedBy.IndexOf("@");
                    if (index > 0)
                    {
                        newItem.reservedBy = newItem.reservedBy.Substring(0, index);
                    }
                    try
                    {
                        newItem.reserved = System.Convert.ToDateTime(dr.GetValue(5)).ToString("d");
                    }
                    catch
                    {

                    }
                    newItem.tsgCompanyNum = System.Convert.ToInt32(dr["TSGCompanyId"].ToString());
                    newItem.partID = "A" + dr["assAssemblyId"].ToString();
                    newItem.dueDate = System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToString("d");
                    newItem.partPicture = "";
                    newItem.partNote = dr["assNotes"].ToString();
                    ReservedList.Add(newItem);
                }
                dr.Close();
            }
            else if(chkUnreserved.Checked)
            {
                //Getting all parts for rfqs that are still open
                if (ddlRFQID.SelectedValue != "All")
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Select rfqID, prtPartNumber, prtPARTID, CustomerName, prtCreated, rfqDueDate, prtPicture, ShipToName, prtPartLength, prtPartWidth, prtPartHeight, prtNote ";
                    sql.CommandText += "from tblPart, linkPartToRFQ, tblRFQ, Customer, CustomerLocation where rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocation.CustomerLocationID and ptrPartID = prtPARTID and ptrRFQID = rfqID and Customer.CustomerID = CustomerLocation.CustomerID ";
                    sql.CommandText += "and not EXISTS (Select * from linkPartToQuote where ptqPartID = prtPARTID) ";
                    sql.CommandText += "and not exists (Select * from linkPartReservedToCompany where prcPartID = prtPARTID) ";
                    sql.CommandText += "and (Select (select distinct 1 from linkPartToPartDetail, linkPartReservedToCompany where ppd.ppdPartToPartID = ppdPartToPartID and ppd.ppdPartID <> ppdPartID and ppdPartID = prcPartID) from linkPartToPartDetail as ppd where ppdPartID = prtPartID) is null ";
                    sql.CommandText += "and not exists (select 1 where (select count(nquNoQuoteID) from tblNoQuote where nquPartID = prtPARTID) >= (Select (count(*) - 1) from linkRFQToCompany where rtqRFQID = rfqID)) ";
                    sql.CommandText += "and rfqID = @rfqID ";
                    if (ddlSalesman.SelectedItem.ToString() != "Any")
                    {
                        sql.CommandText += "and rfqSalesman = @salesman ";
                        sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                    }
                    sql.CommandText += "order by rfqID ";
                    //sql.CommandText += "OFFSET 0 ROWS ";
                    //sql.CommandText += "FETCH NEXT 50 ROWS ONLY";
                    sql.Parameters.AddWithValue("@rfqID", ddlRFQID.SelectedValue);

                    SqlDataReader dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        Unreserved unres = new Unreserved();
                        unres.rfqID = dr.GetValue(0).ToString();
                        unres.partNumber = dr.GetValue(1).ToString();
                        unres.partID = dr.GetValue(2).ToString();
                        unres.customer = dr.GetValue(3).ToString();
                        unres.created = System.Convert.ToDateTime(dr.GetValue(4)).ToString("d");
                        unres.dueDate = System.Convert.ToDateTime(dr.GetValue(5)).ToString("d");
                        unres.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(6).ToString() + "'>Picture";
                        unres.plant = dr.GetValue(7).ToString();
                        unres.partLength = dr.GetValue(8).ToString();
                        unres.partWidth = dr.GetValue(9).ToString();
                        unres.partHeight = dr.GetValue(10).ToString();
                        unres.partNote = dr.GetValue(11).ToString();
                        UnreservedList.Add(unres);
                    }
                    dr.Close();
                }
                
                sql.Parameters.Clear();
            }
            else if (chkUnReservedList.Checked)
            {
                List<removedReserved> l = new List<removedReserved>();
                sql.Parameters.Clear();
                sql.CommandText = "Select rfqID, prtPartNumber, prtpartDescription, prtPartLength, prtPartWidth, prtPartHeight, CustomerName, ShipToName, prtCreated, ";
                sql.CommandText += "ptuInitialReservedDate, convert(date, rfqDueDate) as dueDate, convert(date, ptuCreated) as UnReservedDate, perName, TSGCompanyAbbrev ";
                sql.CommandText += "from linkPartToUnreserved, linkPartToRFQ, tblPart, tblRFQ, Customer, CustomerLocation, Permissions, TSGCompany ";
                sql.CommandText += "where ptuPartID = ptrPartID and ptrRFQID = rfqID and rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocationID and ";
                sql.CommandText += "prtPARTID = ptrPartID and ptuRereserved = 0 and ptuCreated > DATEADD(MONTH, -2, GETDATE()) and ptuUID = UID and TSGCompanyID = ptuCompanyUnreserved ";
                sql.CommandText += "and (@rfq = '' or @rfq = convert(varchar, rfqID)) and (CustomerName like @customer or ShipToName like @customer) and (@company = 1 or @company = TSGCompanyID) ";
                if (ddlSalesman.SelectedItem.ToString() != "Any")
                {
                    sql.CommandText += "and rfqSalesman = @salesman ";
                    sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue.ToString());
                }
                if (chkDueDate.Checked)
                {
                    sql.CommandText += "order by rfqDueDate desc ";
                }
                else
                {
                    sql.CommandText += "order by ptuCreated desc ";
                }
                sql.Parameters.AddWithValue("@rfq", txtRFQ.Text);
                sql.Parameters.AddWithValue("@customer", "%" + txtCustomer.Text + "%");
                sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    removedReserved r = new removedReserved();
                    r.rfqID = dr["rfqID"].ToString();
                    r.partNum = dr["prtPartNumber"].ToString();
                    r.partName = dr["prtpartDescription"].ToString();
                    r.partLength = dr["prtPartLength"].ToString();
                    r.partWidth = dr["prtPartWidth"].ToString();
                    r.partHeight = dr["prtPartHeight"].ToString();
                    r.customer = dr["CustomerName"].ToString();
                    r.plant = dr["ShipToName"].ToString();
                    if (dr["ptuInitialReservedDate"].ToString() != "")
                    {
                        r.firstReserved = System.Convert.ToDateTime(dr["ptuInitialReservedDate"].ToString()).ToShortDateString();
                    }
                    r.dueDate = System.Convert.ToDateTime(dr["dueDate"].ToString()).ToShortDateString();
                    r.unreservedDate = System.Convert.ToDateTime(dr["UnReservedDate"].ToString()).ToShortDateString();
                    r.company = dr["TSGCompanyAbbrev"].ToString();
                    r.name = dr["perName"].ToString();
                    l.Add(r);
                }
                dr.Close();
                gvUnReserved.DataSource = l;
                gvUnReserved.DataBind();
            }
            //else if (chkDisposition.Checked)
            //{
            //    sql.CommandText = "Select quoQuoteID, TSGCompanyAbbrev, quoRFQID, estFirstName, estLastName, qstQuoteStatus, CustomerName, quoCreated, quoTSGCompanyID, prtPartNumber, prtPARTID, rfqDueDate, prtPicture, prtNote, prtRFQLineNumber, quoVersion, TSGSalesman.Name ";
            //    sql.CommandText += "from tblQuote, linkQuoteToRFQ, tblRFQ, Customer, pktblEstimators, TSGCompany, linkPartToQuote, tblPart, pktblQuoteStatus, TSGSalesman ";
            //    sql.CommandText += "where qtrQuoteID = quoQuoteID and qtrRFQID = rfqID and rfqCustomerID = CustomerID and TSGCompanyID = quoTSGCompanyID and quoEstimatorID = estEstimatorID and ";
            //    if (ddlCompany.SelectedValue != "1")
            //    {
            //        sql.CommandText += "quoTSGCompanyID = @company and ";
            //        sql.Parameters.Clear();
            //        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
            //    }
            //    sql.CommandText += "ptqQuoteID = quoQuoteID and ptqPartID = prtPARTID and quoStatusID = qstQuoteStatusID and TSGSalesmanID = quoSalesman order by quoRFQID, prtRFQLineNumber";
            //    SqlDataReader dr = sql.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        //partIDs.Add(dr.GetValue(10).ToString());
            //        if (!quoteIDs.Contains(dr.GetValue(0).ToString()))
            //        {
            //            quoteIDs.Add(dr.GetValue(0).ToString());
            //            QuoteItem newItem = new QuoteItem();
            //            newItem.quoteID = dr.GetValue(2).ToString() + "-" + dr.GetValue(14).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(15).ToString();
            //            //sql2.CommandText = "Select prtPartID, prtPartNumber from linkPartToQuote, tblPart where ptqQuoteID = @quoteID and ptqPartID = prtPARTID";
            //            //sql2.Parameters.Clear();
            //            //sql2.Parameters.AddWithValue("@quoteID", dr.GetValue(0).ToString());
            //            //SqlDataReader dr2 = sql2.ExecuteReader();
            //            //int i = 0;
            //            //while (dr2.Read())
            //            //{
            //            //    if (i != 0)
            //            //    {
            //            //        newItem.partNumber += ",\n";
            //            //        newItem.partID += ", ";
            //            //    }
            //            //    newItem.partID += dr2.GetValue(0).ToString();
            //            //    newItem.partNumber += dr2.GetValue(1).ToString();
            //            //    i++;
            //            //}
            //            //dr2.Close();
            //            newItem.partPicture = dr.GetValue(16).ToString();
            //            newItem.tsgCompany = dr.GetValue(1).ToString();
            //            newItem.rfqID = "<a href='Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + dr.GetValue(2).ToString() + "'>Edit RFQ</a>";
            //            newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
            //            newItem.status = dr.GetValue(5).ToString();
            //            newItem.customer = dr.GetValue(6).ToString();
            //            newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + dr.GetValue(0).ToString() + "&quoteType=2" + "'>Edit Quote</a>";
            //            newItem.quoteType = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-RFQ');return false;\" >";
            //            newItem.quoteTypeNum = 2;
            //            try
            //            {
            //                newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
            //            }
            //            catch
            //            {

            //            }
            //            newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
            //            //newItem.partNumber = dr.GetValue(9).ToString();
            //            //newItem.partID = dr.GetValue(10).ToString();
            //            //partIDs.Add(newItem.partID);
            //            newItem.dueDate = System.Convert.ToDateTime(dr.GetValue(11)).ToString("d");
            //            newItem.url = "<a href='https://tsgrfq.azurewebstites.net/EditRFQ?id=" + newItem.quoteID + "'>RFQ";
            //            //newItem.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(12).ToString() + "'>Picture</a>";
            //            newItem.partNote = dr.GetValue(13).ToString();
            //            MasterList.Add(newItem);
            //        }
            //    }
            //    dr.Close();
            //    sql.Parameters.Clear();

            //    sql.CommandText = "Select ecqECQuoteID, TSGCompanyAbbrev, ecqRFQNumber, estFirstName, estLastName, qstQuoteStatus, CustomerName, ecqCreated, ecqTSGCompanyID, ecqPartNumber, ecqQuoteNumber, ecqVersion, Name ";
            //    sql.CommandText += "from tblECQuote, Customer, pktblEstimators, TSGCompany, pktblQuoteStatus, TSGSalesman, CustomerLocation  ";
            //    sql.CommandText += "where ecqCustomer = Customer.CustomerID and ecqEstimator = estEstimatorID and TSGCompanyID = ecqTSGCompanyID and ecqStatus = qstQuoteStatusID and ecqCustomerLocation = CustomerLocationID and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID ";
            //    sql.CommandText += "order by ecqQuoteNumber, ecqVersion";

            //    dr = sql.ExecuteReader();

            //    while (dr.Read())
            //    {
            //        QuoteItem newItem = new QuoteItem();
            //        newItem.quoteID = dr.GetValue(10).ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr.GetValue(11);
            //        newItem.tsgCompany = dr.GetValue(1).ToString();
            //        newItem.estimator = dr.GetValue(3).ToString() + " " + dr.GetValue(4).ToString();
            //        newItem.status = dr.GetValue(5).ToString();
            //        newItem.customer = dr.GetValue(6).ToString();
            //        newItem.realQuoteID = "<a href='https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + dr.GetValue(0).ToString() + "&quoteType=1" + "'>Edit Quote</a>";
            //        newItem.quoteType = "E/C";
            //        newItem.quoteTypeNum = 1;
            //        try
            //        {
            //            newItem.created = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
            //        }
            //        catch
            //        {

            //        }
            //        newItem.tsgCompanyNum = System.Convert.ToInt32(dr.GetValue(8).ToString());
            //        newItem.partNumber = dr.GetValue(9).ToString();
            //        newItem.url = "";
            //        newItem.quoteType = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-SA');return false;\" >";
            //        newItem.partPicture = dr.GetValue(12).ToString();
            //        MasterList.Add(newItem);
            //    }
            //    dr.Close();
            //}

            if(ddlRFQID.SelectedValue != "All")
            {
                sql.CommandText = "Select ncoNotifiedColor from pktblNotifiedColor where ncoRfqID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", ddlRFQID.SelectedValue);
                SqlDataReader colorDR = sql.ExecuteReader();
                if (colorDR.Read())
                {
                    companiesNotified.Text = colorDR.GetValue(0).ToString().Replace("</font><br />", " </font>");
                }
                colorDR.Close();
            }
            

            connection.Close();
        }

        protected void btnNewQuote_Click(object sender, EventArgs e)
        {
            if(ddlQuoteType2.SelectedValue == "8")
            {
                lblMessage.Text = "<script>window.open('HTSEditQuote?quoteType=3');</script>";
            }
            else if(ddlQuoteType2.SelectedValue == "9")
            {
                lblMessage.Text = "<script>window.open('STSEditQuote');</script>";
            }
            else if (ddlQuoteType2.SelectedValue == "10")
            {
                lblMessage.Text = "<script>window.open('UGSEditQuote');</script>";
            }
            else if (ddlQuoteType2.SelectedValue == "12")
            {
                lblMessage.Text = "<script>window.open('EditQuote?id=0&rfq=0&quoteType=1&notes=1');</script>";
            }
            else
            {
                lblMessage.Text = "<script>window.open('EditQuote?id=0&rfq=0&quoteType=1');</script>";
            }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            OverDue = false;
            HotList = false;
            createPage();
        }
    }

    public class Unreserved
    {
        public string rfqID { get; set; }
        public string partNumber { get; set; }
        public string partID { get; set; }
        public string customer { get; set; }
        public string created { get; set; }
        public string dueDate { get; set; }
        public string partPicture { get; set; }
        public string partNote { get; set; }
        public string plant { get; set; }
        public string partLength { get; set; }
        public string partWidth { get; set; }
        public string partHeight { get; set; }
        public Unreserved()
        {
            rfqID = "";
            partNumber = "";
            customer = "";
            created = "";
            partID = "";
            dueDate = "";
            partPicture = "";
            partNote = "";
            plant = "";
        }
    }

    public class removedReserved
    {
        public string rfqID { get; set; }
        public string partNum { get; set; }
        public string partName { get; set; }
        public string partLength { get; set; }
        public string partWidth { get; set; }
        public string partHeight { get; set; }
        public string customer { get; set; }
        public string plant { get; set; }
        public string firstReserved { get; set; }
        public string dueDate { get; set; }
        public string unreservedDate { get; set; }
        public string company { get; set; }
        public string name { get; set; }
    }
}
