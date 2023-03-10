using Microsoft.SharePoint.Client;
using RFQ.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class STSEditQuote : System.Web.UI.Page
    {
        int rfqID = 0;
        string quoteID = "";
        string ECBaseQuoteID = "";
        int ECQuoteNumber = 0;
        string ECBaseQuoteVersion;
        string partID = "";
        string quoteType;
        string createEC;

        public Boolean IsMasterCompany = false;
        public long UserCompanyID = 0;
        string assemblyId = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
                try
                {
                    quoteID = Request["id"].ToString();
                }
                catch
                {

                }
                try
                {
                    quoteType = Request["quoteType"].ToString();
                }
                catch
                {

                }
                try
                {
                    rfqID = System.Convert.ToInt32(Request["rfq"].ToString());
                }
                catch
                {

                }
                try
                {
                    partID = Request["PartID"].ToString();
                }
                catch
                {

                }
                try
                {
                    assemblyId = Request["assemblyId"].ToString();
                }
                catch
                {

                }
                try
                {
                    createEC = Request["createEC"].ToString();
                }
                catch
                {

                }

            assemblyId = assemblyId.Replace("A", "");

            if (quoteID == "")
            {
                btnDelete_Click.Visible = false;
                //btnNewVersion_Click.Visible = false;
                btnSaveQuote_Click.Visible = false;
                btnSave_Click.Visible = true;
                litScript.Text = "<script>$('#btnCreateSharePoint').hide();$('#btnApp').hide();$('#btnCreateVersion').hide();</script>";
            }
            else
            {
                btnDelete_Click.Visible = true;
                //btnNewVersion_Click.Visible = true;
                btnSaveQuote_Click.Visible = true;
                btnSave_Click.Visible = true;
            }

            if (!IsPostBack)
            {
                txtAnnualVolume.Text = "0";
                txtDaysPerYear.Text = "0";
                txtHoursPerShift.Text = "0";
                txtShiftsPerDay.Text = "0";
                txtEfficiency.Text = "0";
                txtSecondsPerHour.Text = "3600";
                txtTactTime.Text = "0";
                txtNetPartsPerHour.Text = "0";
                txtGrossPartsPerHour.Text = "0";
                txtNetPartsPerDay.Text = "0";

                sql.CommandText = "Select qstQuoteStatusID, qstQuoteStatus from pktblQuoteStatus order by qstQuoteStatus";
                sql.Parameters.Clear();
                SqlDataReader qsDR = sql.ExecuteReader();
                ddlStatus.DataSource = qsDR;
                ddlStatus.DataTextField = "qstQuoteStatus";
                ddlStatus.DataValueField = "qstQuoteStatusID";
                ddlStatus.DataBind();
                qsDR.Close();
                ddlStatus.SelectedValue = "2";

                sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer where cusInactive = 0 or cusInactive is null order by CustomerName ";
                SqlDataReader CustomerDR = sql.ExecuteReader();
                ddlCustomer.DataSource = CustomerDR;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "Please Select");
                CustomerDR.Close();

                sql.CommandText = "Select CONCAT(estFirstName, ' ', estLastName) as 'name', estEstimatorID from pktblEstimators where estCompanyID = 13";
                SqlDataReader estimatorDR = sql.ExecuteReader();
                ddlEstimator.DataSource = estimatorDR;
                ddlEstimator.DataTextField = "name";
                ddlEstimator.DataValueField = "estEstimatorID";
                ddlEstimator.DataBind();
                estimatorDR.Close();

                sql.CommandText = "select steShippingTermsID, steShippingTerms from pktblShippingTerms order by steShippingTerms";
                sql.Parameters.Clear();
                SqlDataReader stDR = sql.ExecuteReader();
                ddlShipping.DataSource = stDR;
                ddlShipping.DataTextField = "steShippingTerms";
                ddlShipping.DataValueField = "steShippingTermsID";
                ddlShipping.DataBind();
                stDR.Close();

                sql.CommandText = "select ptePaymentTermsID, ptePaymentTerms from pktblPaymentTerms order by ptePaymentTerms";
                sql.Parameters.Clear();
                SqlDataReader payDR = sql.ExecuteReader();
                ddlPayment.DataSource = payDR;
                ddlPayment.DataTextField = "ptePaymentTerms";
                ddlPayment.DataValueField = "ptePaymentTermsID";
                ddlPayment.DataBind();
                payDR.Close();

                sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID = 13 or TSGCompanyID = 20 or TSGCompanyID = 21";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                ddlCompany.DataSource = dr;
                ddlCompany.DataTextField = "TSGCompanyAbbrev";
                ddlCompany.DataValueField = "TSGCompanyID";
                ddlCompany.DataBind();
                dr.Close();


                sql.CommandText = "Select Name, ProjectManagerID from ProjectManager where pmaTSGCompanyID = 13 or pmaTSGCompanyID = 20 or pmaTSGCompanyID = 21 order by Name ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlProjectManager.DataSource = dr;
                ddlProjectManager.DataTextField = "Name";
                ddlProjectManager.DataValueField = "ProjectManagerID";
                ddlProjectManager.DataBind();
                dr.Close();
                ddlProjectManager.Items.Insert(0, "Please Select");
                if (master.getCompanyId() == 20)
                {
                    ddlCompany.SelectedValue = "20";
                }
                else
                {
                    ddlCompany.SelectedValue = "13";
                }

                populate_header();
            }
            connection.Close();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }

        protected void btnDeleteClick(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            List<string> pwnIDs = new List<string>();

            sql.CommandText = "Select psqPreWordedNoteID from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            SqlDataReader dr = sql.ExecuteReader();
            while(dr.Read())
            {
                pwnIDs.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            sql.CommandText = "Delete from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            for(int i = 0; i < pwnIDs.Count; i++)
            {
                sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @ID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ID", pwnIDs[i]);
                master.ExecuteNonQuery(sql, "STS Edit Quote");
            }

            sql.CommandText = "Delete from linkGeneralNoteToSTSQuote where gnsSTSQuoteID = @id";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", quoteID);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            sql.CommandText = "delete from linkQuoteToRFQ where qtrQuoteID = @id and qtrSTS = 1 ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", quoteID);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            if (partID != "")
            {
                sql.CommandText = "delete from linkPartToQuote where ptqQuoteID = @id and ptqSTS = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");
            }
            else if (assemblyId != "")
            {
                sql.CommandText = "Delete from linkAssemblyToQuote where atqQuoteId = @id and atqSTS = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");
            }

            sql.CommandText = "delete from pktblSTSQuoteNotes where sqnQuoteID = @quoteId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteId", quoteID);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            sql.CommandText = "Delete from tblSTSQuote where squSTSQuoteID = @id";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", quoteID);
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            Response.Redirect("https://tsgrfq.azurewebsites.net/STSEditQuote?id=0");
        }

        protected void populate_Plants()
        {
            if (ddlCustomer.SelectedValue == "Please Select")
            {
                return;
            }
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select CustomerLocationID, Concat(ShipToName, ' (',ShipCode,')') as Location from CustomerLocation where CustomerID=@customer  order by Location";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            SqlDataReader plantDR = sql.ExecuteReader();
            ddlPlant.DataSource = plantDR;
            ddlPlant.DataTextField = "Location";
            ddlPlant.DataValueField = "CustomerLocationID";
            ddlPlant.SelectedIndex = -1;
            ddlPlant.DataBind();
            plantDR.Close();
            ddlPlant.SelectedIndex = 0;

            sql.CommandText = "select rfqPlantID ";
            sql.CommandText += " from tblRFQ ";
            sql.CommandText += " where rfqID=@rfq";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", rfqID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    ddlPlant.SelectedValue = dr.GetValue(0).ToString();
                }
                catch
                {

                }
            }
            dr.Close();
            connection.Close();
            setSalesmanAndRank();
        }

        protected void setSalesmanAndRank()
        {
            Site master = new RFQ.Site();
            IsMasterCompany = master.getMasterCompany();
            UserCompanyID = master.getCompanyId();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            lblSalesman.Text = "Not Set";
            sql.Parameters.Clear();
            sql.CommandText = "select Rank, Name from customerlocation, customerrank, tsgsalesman where customerlocation.customerrankid = customerrank.customerrankid and customerlocation.tsgsalesmanid=tsgsalesman.tsgsalesmanid and customerlocationID=@plant";
            sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                lblSalesman.Text = dr.GetValue(1).ToString();
            }
            dr.Close();
            connection.Close();
        }

        protected void populate_header()
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            SqlDataReader dr;
            List<Label> generalNote = new List<Label>();
            generalNote.Add(lblGeneralNote1);
            generalNote.Add(lblGeneralNote2);
            generalNote.Add(lblGeneralNote3);
            generalNote.Add(lblGeneralNote4);
            generalNote.Add(lblGeneralNote5);
            generalNote.Add(lblGeneralNote6);
            generalNote.Add(lblGeneralNote7);
            generalNote.Add(lblGeneralNote8);
            generalNote.Add(lblGeneralNote9);

            List<CheckBox> cb = new List<CheckBox>();
            cb.Add(cbGeneralNote1);
            cb.Add(cbGeneralNote2);
            cb.Add(cbGeneralNote3);
            cb.Add(cbGeneralNote4);
            cb.Add(cbGeneralNote5);
            cb.Add(cbGeneralNote6);
            cb.Add(cbGeneralNote7);
            cb.Add(cbGeneralNote8);
            cb.Add(cbGeneralNote9);

            txtSecondsPerHour.Text = "3600";

            sql.CommandText = "Select concat(gnoGeneralNoteID, '-', gnoGeneralNote), gnoDefault from pktblGeneralNote where gnoCompany = 'STS' ";
            if (quoteID == "")
            {
                sql.CommandText += "and gnoActive = 1 ";
                cbGeneralNote9.Visible = false;
            }
            else
            {
                var qID = System.Convert.ToInt32(quoteID);
                if (qID > 1941)
                {
                    sql.CommandText += "and gnoActive = 1 ";
                    cbGeneralNote9.Visible = false;
                }
                else
                {
                }
            }
            SqlDataReader gnodr = sql.ExecuteReader();
            int j = 0;
            while (gnodr.Read())
            {
                generalNote[j].Text = gnodr.GetValue(0).ToString();
                cb[j].Checked = System.Convert.ToBoolean(gnodr["gnoDefault"].ToString());
                j++;
            }
            gnodr.Close();


            if(rfqID != 0 && partID != "")
            {
                string cust = "", plant = "";
                sql.CommandText = "Select prtPartDescription, rfqCustomerRFQNumber, rfqCustomerID, rfqPlantID, TSGSalesman.Name, rfqProductTypeID, rfqOEMID, prtPartTypeID, prtPartNumber, CustomerContact.Name ";
                sql.CommandText += "from tblRFQ, Customer, CustomerLocation, tblPart, linkPartToRFQ, TSGSalesman, CustomerContact ";
                sql.CommandText += "where rfqPlantID = CustomerLocationID and rfqCustomerID = Customer.CustomerID and prtPARTID = @partID and rfqID = @rfq and prtPartID = ptrPartID and ptrRFQID = @rfq ";
                sql.CommandText += "and TSGSalesman.TSGSalesmanID = CustomerLocation.TSGSalesManID and CustomerContactID = rfqCustomerContact ";

                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfq", rfqID);
                dr = sql.ExecuteReader();

                if (dr.Read())
                {
                    txtPartNumber.Text = dr.GetValue(8).ToString();
                    txtRFQNumber.Text = rfqID.ToString();
                    txtRFQNumber.ReadOnly = true;
                    txtPartName.Text = dr.GetValue(0).ToString();
                    txtCustomerRFQ.Text = dr.GetValue(1).ToString();
                    ddlCustomer.Text = dr.GetValue(2).ToString();
                    cust = dr.GetValue(2).ToString();
                    if (ddlCustomer.SelectedValue == cust)
                    {
                        populate_Plants();
                    }
                    ddlPlant.Text = dr.GetValue(3).ToString();
                    plant = dr.GetValue(3).ToString();
                    lblSalesman.Text = dr.GetValue(4).ToString();
                    ddlCustomer.Enabled = false;
                    ddlPlant.Enabled = false;
                    txtCustomerContact.Text = dr.GetValue(9).ToString();
                    txtCustomerContact.ReadOnly = true;
                }
                dr.Close();

                //This is to re load the original customer list when the Customer is inactive
                if (ddlCustomer.SelectedValue != cust && cust != "")
                {
                    sql.CommandText = "select CustomerID, concat(CustomerName,' (', CustomerNumber, ')') as Name from Customer ";
                    sql.CommandText += "order by CustomerName";

                    SqlDataReader CustomerDR = sql.ExecuteReader();
                    ddlCustomer.DataSource = CustomerDR;
                    ddlCustomer.DataTextField = "Name";
                    ddlCustomer.DataValueField = "CustomerID";
                    ddlCustomer.DataBind();
                    ddlCustomer.Items.Insert(0, "Please Select");
                    ddlCustomer.SelectedValue = cust;
                    CustomerDR.Close();
                    populate_Plants();
                    ddlPlant.SelectedValue = plant;
                }
            }
            else if (assemblyId != "")
            {
                string cust = "";
                string plant = "";

                sql.CommandText = "Select a.assDescription, r.rfqCustomerRFQNumber, c.CustomerID, cl.CustomerLocationID, ts.Name as TSGSalesman, r.rfqProductTypeId, ";
                sql.CommandText += "r.rfqOEMID, a.assType, a.assNumber, cc.Name as CustomerContact ";
                sql.CommandText += "from tblRFQ r ";
                sql.CommandText += "inner join Customer c on c.CustomerID = r.rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation cl on cl.CustomerLocationID = r.rfqPlantID ";
                sql.CommandText += "inner join linkAssemblyToRFQ atr on atr.atrRfqId = r.rfqID ";
                sql.CommandText += "inner join tblAssembly a on a.assAssemblyId  = atr.atrAssemblyId ";
                sql.CommandText += "inner join TSGSalesman ts on ts.TSGSalesmanID = r.rfqSalesman ";
                sql.CommandText += "inner join CustomerContact cc on cc.CustomerContactID = r.rfqCustomerContact ";
                sql.CommandText += "where r.rfqID = @rfq and a.assAssemblyId = @assemblyId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    txtPartNumber.Text = dr["assNumber"].ToString();
                    txtRFQNumber.Text = rfqID.ToString();
                    txtRFQNumber.ReadOnly = true;
                    txtPartName.Text = dr["assDescription"].ToString();
                    txtCustomerRFQ.Text = dr["rfqCustomerRFQNumber"].ToString();
                    ddlCustomer.Text = dr["CustomerID"].ToString();
                    cust = dr["CustomerID"].ToString();
                    if (ddlCustomer.SelectedValue == cust)
                    {
                        populate_Plants();
                    }
                    ddlPlant.Text = dr["CustomerLocationID"].ToString();
                    plant = dr["CustomerLocationID"].ToString();
                    lblSalesman.Text = dr["TSGSalesman"].ToString();
                    ddlCustomer.Enabled = false;
                    ddlPlant.Enabled = false;
                    txtCustomerContact.Text = dr["CustomerContact"].ToString();
                    txtCustomerContact.ReadOnly = true;
                }
                dr.Close();
                //This is to re load the original customer list when the Customer is inactive
                if (ddlCustomer.SelectedValue != cust && cust != "")
                {
                    sql.CommandText = "select CustomerID, concat(CustomerName,' (', CustomerNumber, ')') as Name from Customer ";
                    sql.CommandText += "order by CustomerName";

                    SqlDataReader CustomerDR = sql.ExecuteReader();
                    ddlCustomer.DataSource = CustomerDR;
                    ddlCustomer.DataTextField = "Name";
                    ddlCustomer.DataValueField = "CustomerID";
                    ddlCustomer.DataBind();
                    ddlCustomer.Items.Insert(0, "Please Select");
                    ddlCustomer.SelectedValue = cust;
                    CustomerDR.Close();
                    populate_Plants();
                    ddlPlant.SelectedValue = plant;
                }
            }

            if(quoteID != "")
            {
                hdnQuoteNumber.Value = quoteID;

                string cust = "", plant = "";
                Boolean locked = false;

                sql.CommandText = "select squQuoteNumber, squQuoteVersion, squStatusID, squPartNumber, squPartName, squRFQNum, squCustomerID, ";
                sql.CommandText += "squPlantID, squCustomerContact, squSalesmanID, squCustomerRFQNum, squEstimatorID, squEAV, squProcess, squMachineTime, ";
                sql.CommandText += "squShippingID, squPaymentID, squLeadTime, squJobNum, squUseTSG, squAnnualVolume, squDaysPerYear, squHoursPerShift, ";
                sql.CommandText += "squShiftsPerDay, squEfficiency, squSecondsPerHour, squTactTime, squNetPartsPerHour, squGrossPartsPerHour, squNetPartsPerDay, ";
                sql.CommandText += "squLocked, squFirmQuote, squCreatedBy, squFinalized, squECQuote, squDetailedQuotePdf, squDetailedQuoteOrigFn ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "Where squSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);

                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    lblquoteID.Text = quoteID;
                    lblQuoteNumber.Text = dr.GetValue(0).ToString();
                    lblVersion.Text = dr.GetValue(1).ToString();
                    ddlStatus.SelectedValue = dr.GetValue(2).ToString();

                    txtPartNumber.Text = dr.GetValue(3).ToString();
                    txtPartName.Text = dr.GetValue(4).ToString();
                    txtRFQNumber.Text = dr.GetValue(5).ToString();
                    lblSalesman.Text = dr.GetValue(9).ToString();
                    ddlCustomer.SelectedValue = dr.GetValue(6).ToString();
                    cust = dr.GetValue(6).ToString();
                    populate_Plants();
                    ddlPlant.SelectedValue = dr.GetValue(7).ToString();
                    plant = dr.GetValue(7).ToString();
                    txtCustomerContact.Text = dr.GetValue(8).ToString();
                    txtCustomerRFQ.Text = dr.GetValue(10).ToString();
                    ddlEstimator.SelectedValue = dr.GetValue(11).ToString();
                    txtEAV.Text = dr.GetValue(12).ToString();
                    txtProcess.Text = dr.GetValue(13).ToString();
                    txtMachineTime.Text = dr.GetValue(14).ToString();
                    ddlShipping.SelectedValue = dr.GetValue(15).ToString();
                    ddlPayment.SelectedValue = dr.GetValue(16).ToString();
                    txtLeadTime.Text = dr.GetValue(17).ToString();
                    txtJobNumber.Text = dr.GetValue(18).ToString();
                    cbUseTSG.Checked = dr.GetBoolean(19);
                    txtAnnualVolume.Text = dr["squAnnualVolume"].ToString();
                    txtDaysPerYear.Text = dr["squDaysPerYear"].ToString();
                    txtHoursPerShift.Text = dr["squHoursPerShift"].ToString();
                    txtShiftsPerDay.Text = dr["squShiftsPerDay"].ToString();
                    txtEfficiency.Text = dr["squEfficiency"].ToString();
                    txtSecondsPerHour.Text = dr["squSecondsPerHour"].ToString();
                    txtTactTime.Text = dr["squTactTime"].ToString();
                    txtNetPartsPerHour.Text = dr["squNetPartsPerHour"].ToString();
                    txtGrossPartsPerHour.Text = dr["squGrossPartsPerHour"].ToString();
                    txtNetPartsPerDay.Text = dr["squNetPartsPerDay"].ToString();
                    // BD - New EC quote will not be locked 
                    if ((dr["squLocked"].ToString() != "") && (createEC == ""))
                    {
                        locked = System.Convert.ToBoolean(dr["squLocked"].ToString());
                    }
                    else if (createEC == "true")
                    {
                        locked = false;
                    }
                    if (dr["squFirmQuote"].ToString() != "")
                    {
                        cbFirmQuote.Checked = System.Convert.ToBoolean(dr["squFirmQuote"].ToString());
                    }
                    if (dr["squECQuote"].ToString() != "")
                    {
                        cbECQuote.Checked = System.Convert.ToBoolean(dr["squECQuote"].ToString());
                    }
                    if (createEC == "true")
                    {
                        cbECQuote.Checked = true;
                    }
                    if (dr["squCreatedBy"].ToString().ToLower() != master.getUserName().ToLower())
                    {
                        //btnApproval.Visible = false;
                        litScript.Text = "<script>$('#btnApp').hide();</script>";
                    }
                    // BD - New EC quote will not be finalized
                    if ((dr["squFinalized"].ToString() == "True")  && (createEC == ""))
                    {
                        litScript.Text = "<script>$('#MainContent_btnSave').hide();$('#MainContent_btnDelete_Click').hide();</script>";
                        //btnSaveQuote_Click.Visible = false;
                        btnFinalize.Visible = false;
                        lblStatus.Text = "This quote has been finalized and is not editable.";
                    }
                    else
                    {
                        btnFinalize.Visible = true;
                    }
                    if (dr["squDetailedQuotePdf"].ToString() != "")
                    {
                        txtDetailedQuote.Text = dr["squDetailedQuoteOrigFn"].ToString();
                    }
                    else
                    {
                        txtDetailedQuote.Text = dr["squDetailedQuotePdf"].ToString() + "No STS Detailed Quote exists.";
                    }
                }
                dr.Close();

                if (locked)
                {
                    btnDelete_Click.Visible = false;
                    //btnNewVersion_Click.Visible = false;
                    //btnSaveQuote_Click.Visible = false;
                    btnSave_Click.Visible = false;
                    //btnApproval.Visible = false;
                    lblStatus.Text = "";
                    litScript.Text = "<script>$('#btnCreateSharePoint').hide();$('#btnApp').hide();</script>";
                    lblStatus.ForeColor = System.Drawing.Color.Red;
                    lblStatus.Text = "This quote is locked and has been submitted for approval to ";
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
                        lblStatus.Text += dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(startDate, est).ToString();
                    }
                    dr.Close();
                }

                //This is to re load the original customer list when the Customer is inactive
                if (ddlCustomer.SelectedValue != cust && cust != "")
                {
                    sql.CommandText = "select CustomerID, concat(CustomerName,' (', CustomerNumber, ')') as Name from Customer ";
                    sql.CommandText += "order by CustomerName";

                    SqlDataReader CustomerDR = sql.ExecuteReader();
                    ddlCustomer.DataSource = CustomerDR;
                    ddlCustomer.DataTextField = "Name";
                    ddlCustomer.DataValueField = "CustomerID";
                    ddlCustomer.DataBind();
                    ddlCustomer.Items.Insert(0, "Please Select");
                    ddlCustomer.SelectedValue = cust;
                    CustomerDR.Close();
                    populate_Plants();
                    ddlPlant.SelectedValue = plant;
                }

                if (rfqID == 0)
                {
                    ddlCustomer.Enabled = false;
                    ddlPlant.Enabled = false;
                    txtRFQNumber.ReadOnly = true;
                }

                sql.CommandText = "Select pwnPreWordedNote, pwnCostNote, pwnPreWordedNoteID from pktblPreWordedNote, linkPWNToSTSQuote where psqSTSQuoteID = @quoteID and psqPreWordedNoteID = pwnPreWordedNoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);

                dr = sql.ExecuteReader();
                int i = 0;
                double total = 0;
                while (dr.Read())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr.GetValue(0).ToString()) + "','" + HttpUtility.JavaScriptStringEncode(dr.GetValue(1).ToString()) + "');", true);
                    try
                    {
                        total += System.Convert.ToDouble(dr.GetValue(1).ToString());
                    }
                    catch
                    {

                    }
                    if (i == 0)
                    {
                        hdnNoteOrder.Value = dr.GetValue(2).ToString();
                    }
                    else
                    {
                        hdnNoteOrder.Value += "," + dr.GetValue(2).ToString();
                    }
                    i++;
                }
                dr.Close();

                if (i == 0)
                {
                    sql.CommandText = "Select sqnSTSQuoteNotesID, sqnDescription, sqnToolingCosts, sqnCapitalCosts from pktblSTSQuoteNotes where sqnQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNewNoteRow" + i.ToString(), "addNewNoteRow('" + HttpUtility.JavaScriptStringEncode(dr["sqnDescription"].ToString()) + "','" + 
                            HttpUtility.JavaScriptStringEncode(dr["sqnToolingCosts"].ToString()) + "','" + HttpUtility.JavaScriptStringEncode(dr["sqnCapitalCosts"].ToString()) + "');", true);

                        if (dr["sqnToolingCosts"].ToString() != "")
                        {
                            total += System.Convert.ToDouble(dr["sqnToolingCosts"].ToString());
                        }
                        if (dr["sqnCapitalCosts"].ToString() != "")
                        {
                            total += System.Convert.ToDouble(dr["sqnCapitalCosts"].ToString());
                        }

                        hdnNoteOrder.Value = (i == 0 ? dr["sqnSTSQuoteNotesID"].ToString() : $",{dr["sqnSTSQuoteNotesID"].ToString()}");

                        i++;
                    }
                    dr.Close();
                }
                else
                {
                    lblToolingCosts.Visible = false;
                    lblCapitalCosts.Visible = false;
                    lblSubtotal.Text = "Cost";
                    hdnQuoteType.Value = "old";
                }


                txtTotalCost.Text = "Total: $" + total.ToString();

                sql.CommandText = "Select gnsGeneralNoteID from linkGeneralNoteToSTSQuote where gnsSTSQuoteID = @quote";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    for (int k = 0; k < generalNote.Count; k++)
                    {
                        if (dr.GetValue(0).ToString() == generalNote[k].Text.ToString().Split('-')[0])
                        {
                            cb[k].Checked = true;
                        }
                        else
                        {
                            cb[k].Checked = false;
                        }
                    }
                }
                if ( createEC == "true")
                {
                    // BD - New EC Quote - get the base quote info but set quoteID to null
                    lblquoteID.Text = "";
                    lblQuoteNumber.Text = "";
                    lblVersion.Text = "";
                    ddlStatus.SelectedValue = "2";
                    ECBaseQuoteID = quoteID;
                    hdnQuoteNumber.Value = quoteID;
                    quoteID = "";
                }
            }
        }

        protected void btnFinalize_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            
            sql.CommandText = "update tblSTSQuote set squFinalized = 1, squModified = GETDATE(), squModifiedBy = @user where squSTSQuoteID = @quoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            master.ExecuteNonQuery(sql, "Edit Quote");

            connection.Close();
            populate_header();
        }

        protected void btncreateNewVersionClick(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            String CompanyId = master.getCompanyId().ToString();
            if (CompanyId != "13" || CompanyId != "20")
            {
                CompanyId = "13";
            }

            sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @customer ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlPlant.SelectedValue);
            int salesman = 0;
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
            }
            dr.Close();

            sql.CommandText = "INSERT INTO tblSTSQuote(squCompanyID, squQuoteVersion, squStatusID, squPartNumber, squPartName, squRFQNum, squCustomerID, ";
            sql.CommandText += "squPlantID, squCustomerContact, squSalesmanID, squCustomerRFQNum, squEstimatorID, squEAV, squProcess, squMachineTime, ";
            sql.CommandText += "squShippingID, squPaymentID, squLeadTime, squJobNum, squCreated, squCreatedBy, squUseTSG, squAnnualVolume, squDaysPerYear, ";
            sql.CommandText += "squHoursPerShift, squShiftsPerDay, squEfficiency, squSecondsPerHour, squTactTime, squNetPartsPerHour, squGrossPartsPerHour, ";
            sql.CommandText += "squNetPartsPerDay, squFirmQuote, squRevisionDesc) ";
            sql.CommandText += "output inserted.squSTSQuoteID ";
            sql.CommandText += "VALUES(@companyid, @version, @status, @partNum, @partName, @rfqNum, @customer, @plant, @contact, @salesman, @customerRFQ, ";
            sql.CommandText += "@estimator, @eav, @process, @machineTime, @shipping, @payment, @leadtime, @jobNum, GETDATE(), @user, @useTSG, @annualVolume, ";
            sql.CommandText += "@daysPerYear, @hoursPerShift, @shiftsPerDay, @efficiency, @secondsPerHour, @tactTime, @netPartsPerHour, @grossPartsPerHour, ";
            sql.CommandText += "@netPartsPerDay, @firmQuote, @revisionDesc) ";

            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(lblVersion.Text) + 1).ToString("000"));
            sql.Parameters.AddWithValue("@companyid", CompanyId);
            sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
            sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
            sql.Parameters.AddWithValue("@partName", txtPartName.Text);
            sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text);
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
            sql.Parameters.AddWithValue("@contact", txtCustomerContact.Text);
            sql.Parameters.AddWithValue("@salesman", salesman);
            sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text);
            sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
            sql.Parameters.AddWithValue("@eav", txtEAV.Text);
            sql.Parameters.AddWithValue("@process", txtProcess.Text);
            sql.Parameters.AddWithValue("@machineTime", txtMachineTime.Text);
            sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
            sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
            sql.Parameters.AddWithValue("@leadtime", txtLeadTime.Text);
            sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
            sql.Parameters.AddWithValue("@annualVolume", txtAnnualVolume.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@daysPerYear", txtDaysPerYear.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@hoursPerShift", txtHoursPerShift.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@shiftsPerDay", txtShiftsPerDay.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@efficiency", txtEfficiency.Text.Replace(",", "").Replace("%", ""));
            sql.Parameters.AddWithValue("@secondsPerHour", txtSecondsPerHour.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@tactTime", txtTactTime.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@netPartsPerHour", txtNetPartsPerHour.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@grossPartsPerHour", txtGrossPartsPerHour.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@netPartsPerDay", txtNetPartsPerDay.Text.Replace(",", ""));
            sql.Parameters.AddWithValue("@firmQuote", cbFirmQuote.Checked);
            sql.Parameters.AddWithValue("@revisionDesc", Request.Form["RevisionDescription"].ToString().Trim());

            quoteID = master.ExecuteScalar(sql, "STS Edit Quote").ToString();


            sql.CommandText = "Update tblSTSQuote set squQuoteNumber = @quoteNumber, squPicture = @picture where squSTSQuoteID = @quoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            sql.Parameters.AddWithValue("@quoteNumber", lblQuoteNumber.Text);
            sql.Parameters.AddWithValue("@picture", "STS-" + lblquoteID.Text + ".png");
            master.ExecuteNonQuery(sql, "STS Edit Quote");

            if (Request.Form["notes0"] != null && Request.Form["price0"] != null)
            {
                List<int> insertedNotes = new List<int>();
                //loop
                try
                {
                    sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                    sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    int count = 0;
                    for (int k = 0; k < 100; k++)
                    {
                        if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                        {
                            sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                            sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString());
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                            int noteID = 0;
                            insertedNotes.Add(noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote")));
                            sql.Parameters.Clear();

                            //totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                        }
                        count++;
                    }
                }
                catch
                {

                }

                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToSTSQuote (psqSTSQuoteID, psqPreWordedNoteID, psqCreated, psqCreatedBy) ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }
            }
            else
            {
                try
                {
                    var user = master.getUserName();
                    sql.CommandText = "insert into pktblSTSQuoteNotes (sqnQuoteID, sqnDescription, sqnToolingCosts, sqnCapitalCosts, sqnCreated, sqnCreatedBy) ";
                    sql.CommandText += "Values (@quoteId, @description, @toolingCosts, @capitalCosts, GETDATE(), @user)";
                    sql.Parameters.Clear();
                    int count = 0;
                    for (int k = 0; k < 100; k++)
                    {
                        if (Request.Form["notes" + count].ToString() != "" || Request.Form["tooling" + count].ToString() != "")
                        {
                            var note = Request.Form["notes" + count].ToString();
                            var tooling = Request.Form["tooling" + count].ToString() == "" ? "0" : Request.Form["tooling" + count].ToString();
                            var capital = Request.Form["capital" + count].ToString() == "" ? "0" : Request.Form["capital" + count].ToString();
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteId", quoteID);
                            sql.Parameters.AddWithValue("@description", note);
                            sql.Parameters.AddWithValue("@toolingCosts", tooling);
                            sql.Parameters.AddWithValue("@capitalCosts", capital);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "STS Edit Quote");
                        }
                        count++;
                    }
                }
                catch (Exception err)
                {

                }
            }


            List<Label> generalNote = new List<Label>();
            generalNote.Add(lblGeneralNote1);
            generalNote.Add(lblGeneralNote2);
            generalNote.Add(lblGeneralNote3);
            generalNote.Add(lblGeneralNote4);
            generalNote.Add(lblGeneralNote5);
            generalNote.Add(lblGeneralNote6);
            generalNote.Add(lblGeneralNote7);
            generalNote.Add(lblGeneralNote8);
            generalNote.Add(lblGeneralNote9);


            List<CheckBox> cb = new List<CheckBox>();
            cb.Add(cbGeneralNote1);
            cb.Add(cbGeneralNote2);
            cb.Add(cbGeneralNote3);
            cb.Add(cbGeneralNote4);
            cb.Add(cbGeneralNote5);
            cb.Add(cbGeneralNote6);
            cb.Add(cbGeneralNote7);
            cb.Add(cbGeneralNote8);
            cb.Add(cbGeneralNote9);


            for (int i = 0; i < cb.Count; i++)
            {
                if (cb[i].Checked)
                {
                    sql.CommandText = "insert into linkGeneralNoteToSTSQuote (gnsGeneralNoteID, gnsSTSQuoteID, gnsCreated, gnsCreatedBy) ";
                    sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }
            }

            if (rfqID != 0)
            {
                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @createdBy, 0, 1, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "STS Edit Quote");
                List<string> partids = new List<string>();
                sql.CommandText = "select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", partID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partids.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                if(partids.Count == 0)
                {
                    partids.Add(partID);
                }

                for (int i = 0; i < partids.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 1, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partids[i]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                }
            }

            connection.Close();

            Response.Redirect("https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + quoteID);
        }


        protected void btnSaveClick(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            String CompanyId = master.getCompanyId().ToString();
            if (CompanyId != "13" || CompanyId != "20")
            {
                CompanyId = "13";
            }

            sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @customer ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlPlant.SelectedValue);
            int salesman = 0;
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
            }
            dr.Close();
// BD - Check if this is a new EC Quote
            if ((createEC == "true") && (ECBaseQuoteID == ""))
            {
                ECBaseQuoteID = quoteID;
                quoteID = "";
                    // BD - Get the next ECQuoteNumber

                sql.CommandText = "Select MAX(squECQuoteNumber) as t1 ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "where squECBaseQuoteId = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", ECBaseQuoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    //ECQuoteNumber = int.Parse(dr.GetValue(0).ToString());

                    //                    //ECQuoteNumber = System.Convert.ToInt32(dr.GetValue(0).ToString()) + 1; 

                    //                    ECQuoteNumber = int.Parse(dr["t1"].ToString()) + 1;
                    if (dr["t1"].ToString() == "")
                    {
                        ECQuoteNumber = 1;
                    }
                    else
                    {
                        ECQuoteNumber = System.Convert.ToInt32(dr["t1"]) + 1;
                    }
                    //    if (dr["squECQuoteNumber"].ToString() != "")
                }
                dr.Close();
                // BD - Get the base quote version

                sql.CommandText = "Select squQuoteVersion ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "where squSTSQuoteID = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", ECBaseQuoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    //ECQuoteNumber = int.Parse(dr.GetValue(0).ToString());

                    //                    //ECQuoteNumber = System.Convert.ToInt32(dr.GetValue(0).ToString()) + 1; 

                    //                    ECQuoteNumber = int.Parse(dr["t1"].ToString()) + 1;
                    if (dr["squQuoteVersion"].ToString() != "")
                    {
                        ECBaseQuoteVersion = dr["squQuoteVersion"].ToString();
                    }
                    else
                    {
                        ECBaseQuoteVersion = "001";
                    }
                    //    if (dr["squECQuoteNumber"].ToString() != "")
                }
                dr.Close();
            }
            else
            // Not an EC Quote
            {
                ECQuoteNumber = 0;
                ECBaseQuoteVersion = "001";
                createEC = "false";
            }

            if (quoteID == "")
            {


                sql.CommandText = "INSERT INTO tblSTSQuote(squCompanyID, squQuoteVersion, squStatusID, squPartNumber, squPartName, squRFQNum, squCustomerID, ";
                sql.CommandText += "squPlantID, squCustomerContact, squSalesmanID, squCustomerRFQNum, squEstimatorID, squEAV, squProcess, squMachineTime, ";
                sql.CommandText += "squShippingID, squPaymentID, squLeadTime, squJobNum, squCreated, squCreatedBy, squUseTSG, squAnnualVolume, squDaysPerYear, ";
                sql.CommandText += "squHoursPerShift, squShiftsPerDay, squEfficiency, squSecondsPerHour, squTactTime, squNetPartsPerHour, squGrossPartsPerHour, ";
                sql.CommandText += "squNetPartsPerDay, squFirmQuote, squECQuote, squECBaseQuoteId, squECQuoteNumber ) ";
                sql.CommandText += "output inserted.squSTSQuoteID ";
                sql.CommandText += "VALUES(@companyid, @version, @status, @partNum, @partName, @rfqNum, @customer, @plant, @contact, @salesman, @customerRFQ, ";
                sql.CommandText += "@estimator, @eav, @process, @machineTime, @shipping, @payment, @leadtime, @jobNum, GETDATE(), @user, @useTSG, @annualVolume, ";
                sql.CommandText += "@daysPerYear, @hoursPerShift, @shiftsPerDay, @efficiency, @secondsPerHour, @tactTime, @netPartsPerHour, @grossPartsPerHour, ";
                sql.CommandText += "@netPartsPerDay, @firmQuote, @ECQuote, @ECBaseQuoteID, @ECQuoteNumber) ";

                sql.Parameters.Clear();
                if ((createEC == "true") && (ECBaseQuoteID != ""))
                {
                    sql.Parameters.AddWithValue("@version", ECBaseQuoteVersion);
                    sql.Parameters.AddWithValue("@ECQuote", createEC);
                    sql.Parameters.AddWithValue("@ECBaseQuoteID", ECBaseQuoteID);
                    sql.Parameters.AddWithValue("@ECQuoteNumber", ECQuoteNumber);
                }
                else
                {
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@ECQuote", "false");
                    sql.Parameters.AddWithValue("@ECBaseQuoteID", "0");
                    sql.Parameters.AddWithValue("@ECQuoteNumber", "0");
                }
                sql.Parameters.AddWithValue("@companyid", ddlCompany.SelectedValue);
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@contact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@eav", txtEAV.Text);
                sql.Parameters.AddWithValue("@process", txtProcess.Text);
                sql.Parameters.AddWithValue("@machineTime", txtMachineTime.Text);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@leadtime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@annualVolume", txtAnnualVolume.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@daysPerYear", txtDaysPerYear.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@hoursPerShift", txtHoursPerShift.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@shiftsPerDay", txtShiftsPerDay.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@efficiency", txtEfficiency.Text.Replace(",", "").Replace("%", ""));
                sql.Parameters.AddWithValue("@secondsPerHour", txtSecondsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@tactTime", txtTactTime.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@netPartsPerHour", txtNetPartsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@grossPartsPerHour", txtGrossPartsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@netPartsPerDay", txtNetPartsPerDay.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@firmQuote", cbFirmQuote.Checked);


                quoteID = master.ExecuteScalar(sql, "STS Edit Quote").ToString();


                string quoteNumber = "";
                string stsquoteid = "";
                sql.CommandText = "Select squSTSQuoteID, squPartNumber, squPartName, CustomerName, ShipToName, prtRFQLineNumber, assLineNumber, squQuoteNumber, squQuoteVersion, qtrRFQID, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToSTSQuote inner join pktblPreWordedNote on pwnPreWordedNoteID = psqPreWordedNoteID where psqSTSQuoteID = squSTSQuoteID) as cost, ";
                sql.CommandText += "squCreatedBy, squCustomerContact, squSalesmanID, squEstimatorID, ProjectManager.Email as projectManager, squECQuote, squECBaseQuoteId, squECQuoteNumber ";
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
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["squQuoteNumber"].ToString().Contains("-"))
                    {
                        //BD modified next sectiion
                        if (System.Convert.ToBoolean(dr["squECQuote"].ToString()))
                        {
                            quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-EC-" + dr["squQuoteVersion"].ToString();
                        }
                        else
                        {
                            quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else if (dr["qtrRFQID"].ToString() == "")
                    {
                        if (dr["squQuoteNumber"].ToString() == "")
                        {
                            //BD modified next section
                            if (System.Convert.ToBoolean(dr["squECQuote"].ToString()))
                            {
                                quoteNumber = dr["squECBaseQuoteId"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                            }
                            else
                            {
                                quoteNumber = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                        else
                        {
                            //BD modified next sectiion
                            if (System.Convert.ToBoolean(dr["squECQuote"].ToString()))
                            {
                                quoteNumber = dr["squECBaseQuoteId"].ToString() + "-STS-SA" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                            }
                            else
                            {
                                quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                    }
                    else if (dr["assLineNumber"].ToString() != "")
                    {
                        if (System.Convert.ToBoolean(dr["squECQuote"].ToString()))
                        {
//BD added next line                            quoteNumber = dr["squECBaseQuoteId"].ToString() + "-STS-SA-EC-" + dr["squQuoteVersion"].ToString();
                            quoteNumber = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                        }
                        else
                        {
                            quoteNumber = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else
                    {
                        if (System.Convert.ToBoolean(dr["squECQuote"].ToString()))
                        {
                            //BD added next line                            quoteNumber = dr["squECBaseQuoteId"].ToString() + "-STS-SA-EC-" + dr["squQuoteVersion"].ToString();
                            quoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                        }
                        else
                        {
                            quoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                }
                dr.Close();


                //
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

                mail.To.Add(new MailAddress("sts-quote-time-reporting@toolingsystemsgroup.com"));
                mail.Bcc.Add("dmaguire@toolingsystemsgroup.com");
                mail.Bcc.Add("bduemler@toolingsystemsgroup.com");
                mail.Subject = "Quote " + quoteNumber + " Created";
                mail.Body = "Quote # " + quoteNumber + " has been created";
                mail.Body += "</br>";
                mail.Body += "https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + stsquoteid;

                mail.IsBodyHtml = true;

                server.Send(mail);


                sql.CommandText = "Update tblSTSQuote set squQuoteNumber = @quoteNumber, squPicture = @picture, squCellPicture = @cellPicture where squSTSQuoteID = @quoteID";
//BD                sql.CommandText = "Update tblSTSQuote set squQuoteNumber = @quoteNumber, squPicture = @picture, squCellPicture = @cellPicture, squDetailedQuotePdf = @STSDetailedQuotePdf, squDetailedQuoteOrigFn = @STSDetailedQuoteOrigFn where squSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@quoteNumber", quoteID);
//BD                sql.Parameters.AddWithValue("@quoteNumber", quoteNumber);
                sql.Parameters.AddWithValue("@picture", "STS-" + quoteID + ".png");
                sql.Parameters.AddWithValue("@cellPicture", "STS-" + quoteID + "-Cell.png");
//BD                sql.Parameters.AddWithValue("@STSDetailedQuotePdf", "STS-" + quoteID + "-Detailed.pdf"); //New add
//BD                sql.Parameters.AddWithValue("@STSDetailedQuoteOrigFn", STSDetailedQuoteUpload.PostedFile.FileName); //New add
                master.ExecuteNonQuery(sql, "STS Edit Quote");

                // Load pictures and files into SharePoint

                newPicture("STS-" + quoteID + ".png");
                cellPicture("STS-" + quoteID + "-Cell.png");
                detailedQuote("STS-" + quoteID + "-Detailed.pdf");


                String FileName = "";
                try
                {
                    FileName = filePicture.PostedFile.FileName;
                }
                catch
                {

                }
                if (FileName != "" && assemblyId != "")
                {
                    sql.CommandText = "update tblAssembly set assPicture = @picture where assAssemblyId = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@picture", "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/STS Quote pictures/STS-" + quoteID + ".png");
                    sql.Parameters.AddWithValue("@id", assemblyId);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                }


                try
                {
                    var user = master.getUserName();
                    sql.CommandText = "insert into pktblSTSQuoteNotes (sqnQuoteID, sqnDescription, sqnToolingCosts, sqnCapitalCosts, sqnCreated, sqnCreatedBy) ";
                    sql.CommandText += "Values (@quoteId, @description, @toolingCosts, @capitalCosts, GETDATE(), @user)";
                    sql.Parameters.Clear();
                    int count = 0;
                    for (int k = 0; k < 100; k++)
                    {
                        if (Request.Form["notes" + count].ToString() != "" || Request.Form["tooling" + count].ToString() != "")
                        {
                            var note = Request.Form["notes" + count].ToString();
                            var tooling = Request.Form["tooling" + count].ToString() == "" ? "0" : Request.Form["tooling" + count].ToString();
                            var capital = Request.Form["capital" + count].ToString() == "" ? "0" : Request.Form["capital" + count].ToString();
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteId", quoteID);
                            sql.Parameters.AddWithValue("@description", note);
                            sql.Parameters.AddWithValue("@toolingCosts", tooling);
                            sql.Parameters.AddWithValue("@capitalCosts", capital);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "STS Edit Quote");
                        }
                        count++;
                    }
                }
                catch (Exception err)
                {

                }

                //List<int> insertedNotes = new List<int>();
                //// loop
                ////int totalCost = 0;
                //try
                //{
                //    sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                //    sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                //    sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";
                //    sql.Parameters.Clear();
                //    int count = 0;
                //    for (int k = 0; k < 100; k++)
                //    {
                //        if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                //        {
                //            sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                //            sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                //            sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString());
                //            sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                //            int noteID = 0;
                //            insertedNotes.Add(noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote")));
                //            sql.Parameters.Clear();

                //            //totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                //        }
                //        count++;
                //    }
                //}
                //catch
                //{

                //}

                //for (int k = 0; k < insertedNotes.Count; k++)
                //{
                //    sql.CommandText = "Insert into linkPWNToSTSQuote (psqSTSQuoteID, psqPreWordedNoteID, psqCreated, psqCreatedBy) ";
                //    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                //    sql.Parameters.AddWithValue("@quoteID", quoteID);
                //    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                //    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                //    master.ExecuteNonQuery(sql, "EditQuote");

                //    sql.Parameters.Clear();
                //}

                List<Label> generalNote = new List<Label>();
                generalNote.Add(lblGeneralNote1);
                generalNote.Add(lblGeneralNote2);
                generalNote.Add(lblGeneralNote3);
                generalNote.Add(lblGeneralNote4);
                generalNote.Add(lblGeneralNote5);
                generalNote.Add(lblGeneralNote6);
                generalNote.Add(lblGeneralNote7);
                generalNote.Add(lblGeneralNote8);
                generalNote.Add(lblGeneralNote9);

                List<CheckBox> cb = new List<CheckBox>();
                cb.Add(cbGeneralNote1);
                cb.Add(cbGeneralNote2);
                cb.Add(cbGeneralNote3);
                cb.Add(cbGeneralNote4);
                cb.Add(cbGeneralNote5);
                cb.Add(cbGeneralNote6);
                cb.Add(cbGeneralNote7);
                cb.Add(cbGeneralNote8);
                cb.Add(cbGeneralNote9);


                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToSTSQuote (gnsGeneralNoteID, gnsSTSQuoteID, gnsCreated, gnsCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }

                if (rfqID != 0)
                {
                    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                    sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @createdBy, 0, 1, 0)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "STS Edit Quote");

                    if (partID != "")
                    {
                        List<string> partids = new List<string>();
                        sql.CommandText = "select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@part", partID);
                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            partids.Add(dr.GetValue(0).ToString());
                        }
                        dr.Close();

                        if (partids.Count == 0)
                        {
                            partids.Add(partID);
                        }

                        for (int i = 0; i < partids.Count; i++)
                        {
                            sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                            sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 1, 0) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partID", partids[i]);
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            master.ExecuteNonQuery(sql, "STS Edit Quote");
                        }
                    }
                    else if (assemblyId != "")
                    {
                        sql.CommandText = "insert into linkAssemblyToQuote (atqAssemblyId, atqQuoteId, atqCreated, atqCreatedBy, atqHTS, atqSTS, atqUGS) ";
                        sql.CommandText += "values (@assemblyId, @quoteId, GETDATE(), @user, 0, 1, 0) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                        sql.Parameters.AddWithValue("@quoteId", quoteID);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "STS Edit Quote");
                    }

                }
                if (assemblyId != "")
                {
                    Response.Redirect("https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + quoteID + "&assemblyId=" + assemblyId);
                }
                else
                {
                    Response.Redirect("https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + quoteID);
                }
            }
            else
            {
                //QuoteID not 0 - quote already exists - Update the quote

                sql.CommandText = "update tblSTSQuote set squStatusID = @status, squPartNumber = @partNum, squPartName = @partName, squRFQNum = @rfqNum, squCustomerID = @customer, squPlantID = @plant, ";
                sql.CommandText += "squCustomerContact = @customerContact, squSalesmanID = @salesman, squCustomerRFQNum = @custRFQ, squEstimatorID = @estimator, squEAV = @eav, squProcess = @process, ";
                sql.CommandText += "squMachineTime = @machineTime, squShippingID = @shipping, squPaymentID = @payment, squLeadTime = @leadTime, squJobNum = @jobNum, squModified = GETDATE(), ";
                sql.CommandText += "squModifiedBy = @user, squUseTSG = @useTSG, squAnnualVolume = @annualVolume, squDaysPerYear = @daysPerYear, squHoursPerShift = @hoursPerShift, ";
                sql.CommandText += "squShiftsPerDay = @shiftsPerDay, squEfficiency = @efficiency, squSecondsPerHour = @secondsPerHour, squTactTime = @tactTime, squNetPartsPerHour = @netPartsPerHour, ";
                sql.CommandText += "squGrossPartsPerHour = @grossPartsPerHour, squNetPartsPerDay = @netPartsPerDay, squFirmQuote = @firmQuote, squCellPicture = @cellPicture, squCompanyId = @company ";
//BD              sql.CommandText += "squGrossPartsPerHour = @grossPartsPerHour, squNetPartsPerDay = @netPartsPerDay, squFirmQuote = @firmQuote, squCellPicture = @cellPicture, squCompanyId = @company, squDetailedQuotePdf = @detailedquotepdf ";
                sql.CommandText += "where squSTSQuoteID = @quoteID";
                sql.Parameters.Clear();

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custRFQ", txtCustomerRFQ.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@eav", txtEAV.Text);
                sql.Parameters.AddWithValue("@process", txtProcess.Text);
                sql.Parameters.AddWithValue("@machineTime", txtMachineTime.Text);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@annualVolume", txtAnnualVolume.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@daysPerYear", txtDaysPerYear.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@hoursPerShift", txtHoursPerShift.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@shiftsPerDay", txtShiftsPerDay.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@efficiency", txtEfficiency.Text.Replace(",", "").Replace("%", ""));
                sql.Parameters.AddWithValue("@secondsPerHour", txtSecondsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@tactTime", txtTactTime.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@netPartsPerHour", txtNetPartsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@grossPartsPerHour", txtGrossPartsPerHour.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@netPartsPerDay", txtNetPartsPerDay.Text.Replace(",", ""));
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@firmQuote", cbFirmQuote.Checked);
                sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue.ToString());
                sql.Parameters.AddWithValue("@cellPicture", "STS-" + quoteID + "-Cell.png");
//BD                sql.Parameters.AddWithValue("@detailedquotepdf", "STS-" + quoteID + "-Detailed.pdf");

                master.ExecuteNonQuery(sql, "STS Edit Quote");

                List<int> insertedNotes = new List<int>();

                sql.CommandText = "Select psqPreWordedNoteID from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    insertedNotes.Add(System.Convert.ToInt32(dr.GetValue(0).ToString()));
                }
                dr.Close();
                sql.CommandText = "Delete from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");

                for(int i = 0; i < insertedNotes.Count; i++)
                {
                    sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", insertedNotes[i]);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                }
                insertedNotes.Clear();

                Boolean newForm = true;
                if (Request.Form["tooling0"] == null)
                {
                    newForm = false;
                }

                if (!newForm)
                {
                    // loop
                    //int totalCost = 0;
                    try
                    {
                        sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                        sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                        sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        int count = 0;
                        for (int k = 0; k < 100; k++)
                        {
                            if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                            {
                                sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                                sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                                sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString());
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                                int noteID = 0;
                                insertedNotes.Add(noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote")));
                                sql.Parameters.Clear();

                                //totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                            }
                            count++;
                        }
                    }
                    catch
                    {

                    }

                    for (int k = 0; k < insertedNotes.Count; k++)
                    {
                        sql.CommandText = "Insert into linkPWNToSTSQuote (psqSTSQuoteID, psqPreWordedNoteID, psqCreated, psqCreatedBy) ";
                        sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "EditQuote");

                        sql.Parameters.Clear();
                    }
                }
                else
                {
                    sql.CommandText = "delete from pktblSTSQuoteNotes where sqnQuoteID = @quoteId ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteId", quoteID);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                    try
                    {
                        var user = master.getUserName();
                        sql.CommandText = "insert into pktblSTSQuoteNotes (sqnQuoteID, sqnDescription, sqnToolingCosts, sqnCapitalCosts, sqnCreated, sqnCreatedBy) ";
                        sql.CommandText += "Values (@quoteId, @description, @toolingCosts, @capitalCosts, GETDATE(), @user)";
                        sql.Parameters.Clear();
                        int count = 0;
                        for (int k = 0; k < 100; k++)
                        {
                            if (Request.Form["notes" + count].ToString() != "" || Request.Form["tooling" + count].ToString() != "")
                            {
                                var note = Request.Form["notes" + count].ToString();
                                var tooling = Request.Form["tooling" + count].ToString() == "" ? "0" : Request.Form["tooling" + count].ToString();
                                var capital = Request.Form["capital" + count].ToString() == "" ? "0" : Request.Form["capital" + count].ToString();
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@quoteId", quoteID);
                                sql.Parameters.AddWithValue("@description", note);
                                sql.Parameters.AddWithValue("@toolingCosts", tooling);
                                sql.Parameters.AddWithValue("@capitalCosts", capital);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "STS Edit Quote");
                            }
                            count++;
                        }
                    }
                    catch (Exception err)
                    {

                    }
                }

                newPicture("STS-" + quoteID + ".png");
                cellPicture("STS-" + quoteID + "-Cell.png");
                detailedQuote("STS-" + quoteID + "-Detailed.pdf");


                String FileName = "";
                try
                {
                    FileName = filePicture.PostedFile.FileName;
                }
                catch
                {

                }
                if (FileName != "" && assemblyId != "")
                {
                    sql.CommandText = "update tblAssembly set assPicture = @picture where assAssemblyId = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@picture", "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/STS Quote pictures/STS-" + quoteID + ".png");
                    sql.Parameters.AddWithValue("@id", assemblyId);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                }

                List<Label> generalNote = new List<Label>();
                generalNote.Add(lblGeneralNote1);
                generalNote.Add(lblGeneralNote2);
                generalNote.Add(lblGeneralNote3);
                generalNote.Add(lblGeneralNote4);
                generalNote.Add(lblGeneralNote5);
                generalNote.Add(lblGeneralNote6);
                generalNote.Add(lblGeneralNote7);
                generalNote.Add(lblGeneralNote8);
                generalNote.Add(lblGeneralNote9);


                List<CheckBox> cb = new List<CheckBox>();
                cb.Add(cbGeneralNote1);
                cb.Add(cbGeneralNote2);
                cb.Add(cbGeneralNote3);
                cb.Add(cbGeneralNote4);
                cb.Add(cbGeneralNote5);
                cb.Add(cbGeneralNote6);
                cb.Add(cbGeneralNote7);
                cb.Add(cbGeneralNote8);
                cb.Add(cbGeneralNote9);

                sql.CommandText = "Delete from linkGeneralNoteToSTSQuote where gnsSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit STS Quote");

                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToSTSQuote (gnsGeneralNoteID, gnsSTSQuoteID, gnsCreated, gnsCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }

                populate_header();

            }



            connection.Close();
        }


        private void newPicture(string pictureName)
        {
            Site master = new RFQ.Site();

            String FileName = "";
            try
            {
                FileName = filePicture.PostedFile.FileName;
            }
            catch
            {

            }
            if (FileName != "")
            {
                ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
                ctx.Credentials = master.getSharePointCredentials();
                Web web = ctx.Web;
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("STS Quote pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = newStream;
                newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/STS Quote pictures/" + pictureName;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                partPicturesList.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                // set the Attributes
                Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                newItem.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
        }

        private void cellPicture(string pictureName)
        {
            Site master = new Site();
            string file = "";
            try
            {
                file = cellPictureUpload.PostedFile.FileName;
            }
            catch
            {
                return;
            }
            if (file != "")
            {
                ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
                ctx.Credentials = master.getSharePointCredentials();
                Web web = ctx.Web;
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                Microsoft.SharePoint.Client.List partPictureList = web.Lists.GetByTitle("STS Quote pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(cellPictureUpload.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)cellPictureUpload.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream stream = new System.IO.MemoryStream(fileData);
                FileCreationInformation fci = new FileCreationInformation();
                fci.ContentStream = stream;
                fci.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/STS Quote pictures/" + pictureName;
                fci.Overwrite = true;
                Microsoft.SharePoint.Client.File f = partPictureList.RootFolder.Files.Add(fci);
                partPictureList.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                //set attributes
                Microsoft.SharePoint.Client.ListItem item = f.ListItemAllFields;
                item.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
        }

        private void detailedQuote(string detailedQuoteName)
        {
            Site master = new Site();
            string file = "";
            try
            {
                file = STSDetailedQuoteUpload.PostedFile.FileName;
            }
            catch
            {
                return;
            }
            if (file != "")
            {
// Add file to SharePoint
                ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
                ctx.Credentials = master.getSharePointCredentials();
                Web web = ctx.Web;
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                Microsoft.SharePoint.Client.List detailedQuoteList = web.Lists.GetByTitle("STS Detailed Quotes");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(STSDetailedQuoteUpload.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)STSDetailedQuoteUpload.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream stream = new System.IO.MemoryStream(fileData);
                FileCreationInformation fci = new FileCreationInformation();
                fci.ContentStream = stream;
                fci.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/STS Detailed Quotes/" + detailedQuoteName;
                fci.Overwrite = true;
                Microsoft.SharePoint.Client.File f = detailedQuoteList.RootFolder.Files.Add(fci);
                detailedQuoteList.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                //set attributes
                Microsoft.SharePoint.Client.ListItem item = f.ListItemAllFields;
                item.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

// Add filename uploaded to the database
                Site master1 = new Site();
                SqlConnection connection = new SqlConnection(master1.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "update tblSTSQuote set squDetailedQuotePdf = @detailedQuoteName, squDetailedQuoteOrigFn = @filename, squModified = GETDATE(), squModifiedBy = @user where squSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@user", master1.getUserName());
           // BD
                sql.Parameters.AddWithValue("@detailedQuoteName", detailedQuoteName);
                sql.Parameters.AddWithValue("@filename", file);
                master1.ExecuteNonQuery(sql, "Edit Quote");

                // Set textbox on page
                txtDetailedQuote.Text = file;

                connection.Close();
            }
        }


        protected void btnApproval_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            int step = 0;
            string defaultApprover = "";
            sql.CommandText = "Select top 1 sasSTSApprovalStepsID, sasDefaultApprover from pktblSTSApprovalSteps where sasFirmQuote = @firm and sasActive = 1 order by sasOrder ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@firm", cbFirmQuote.Checked);
            SqlDataReader dr = sql.ExecuteReader();
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

            lblStatus.ForeColor = System.Drawing.Color.Red;
            string user = master.getUserName();

            string url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/STS%20Quote%20Attachments/";
            int count = 0;
            foreach (var f in fuQuote.PostedFiles)
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
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                Microsoft.SharePoint.Client.Folder fo = list2.Folder;
                Microsoft.SharePoint.Client.FileCollection files = fo.Files;

                ctx.Load(files);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = f.InputStream;
                newFile.Url = url + quoteID + " " + count + " " + attempt.ToString() + System.IO.Path.GetExtension(f.FileName);
                newFile.Overwrite = true;

                Microsoft.SharePoint.Client.File file = list.RootFolder.Files.Add(newFile);
                list.Update();

                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                count++;
            }

            if (step == 0)
            {
                lblStatus.Text = "There was a problem sending the notification.  Please contact an administrator.";
                connection.Close();
                return;
            }
            STSNotificaiton n = new STSNotificaiton();
            try
            {
                lblStatus.Text = "Your quote has been submitted for approval.";

                sql.CommandText = "insert into tblSTSQuoteStatus (sqsSTSQuoteID, sqsStepID, sqsApprovalTo, sqsAttemptNumber, sqsStepStartedDate, sqsCreated, sqsCreatedBy) ";
                sql.CommandText += "values (@quoteId, @stepId, @approvalTo, @attempt, GETDATE(), GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                sql.Parameters.AddWithValue("@stepId", step);
                sql.Parameters.AddWithValue("@approvalTo", defaultApprover);
                sql.Parameters.AddWithValue("@attempt", attempt);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "STS Notification");

                n.sendNotificaiton(quoteID, step);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "There was a problem sending the notification.  Please contact an administrator.";
                sql.CommandText = "delete from linkAttachmentToQuote where atqQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
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

            btnDelete_Click.Visible = false;
            //btnNewVersion_Click.Visible = false;
            //btnSaveQuote_Click.Visible = false;
            btnSave_Click.Visible = false;
            //btnApproval.Visible = false;
            lblStatus.Text = "";
            litScript.Text = "<script>$('#btnCreateSharePoint').hide();$('#btnApp').hide();</script>";
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Text = "This quote is locked and has been submitted for approval to ";
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
                lblStatus.Text += dr["perName"].ToString() + " on " + TimeZoneInfo.ConvertTimeFromUtc(startDate, est).ToString();
            }
            dr.Close();

            connection.Close();
        }
    }
}
