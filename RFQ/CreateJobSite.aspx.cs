using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;

namespace RFQ
{
    public partial class CreateJobSite : System.Web.UI.Page
    {
        string quoteID = "";
        string customerContactId = "";
        string company = "";
        Boolean SA = false;
        Boolean EC = false;
        Boolean noQuote = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = master.getConnectionString();
            connection.Open();
            sql.Connection = connection;

            string customer = "", plant = "";
            if (Request["id"] != null)
            {
                quoteID = Request["id"].ToString();
            }
            if(Request["company"] != null)
            {
                company = Request["company"].ToString();
            }
            if(Request["SA"] != null)
            {
                SA = System.Convert.ToBoolean(Request["SA"].ToString());
            }
            if (Request["EC"] != null)
            {
                EC = System.Convert.ToBoolean(Request["EC"].ToString());
            }
            if (Request["noQuote"] != null)
            {
                noQuote = System.Convert.ToBoolean(Request["noQuote"].ToString());
                company = master.getCompanyId().ToString();
            }

            if (!IsPostBack)
            {
                if (noQuote)
                {
                    chkMasterboard.Visible = false;
                    chkMasterboard.Checked = false;
                    chkSharepoint.Checked = false;
                }
                if (company == "15")
                {
                    chkMasterboard.Checked = false;
                }
                //if (company == "13")
                //{
                //    chkSharepoint.Checked = false;
                //    chkSharepoint.Visible = false;
                //}
                //Populating dropdown lists
                sql.CommandText = " Select ProjectManagerID, Name from ProjectManager where pmaTSGCompanyID = @company and (pmaInactive = 0 or pmaInactive is null) order by Name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                SqlDataReader dr = sql.ExecuteReader();
                ddlProjectManager.DataSource = dr;
                ddlProjectManager.DataTextField = "Name";
                ddlProjectManager.DataValueField = "ProjectManagerID";
                ddlProjectManager.DataBind();
                dr.Close();

                sql.CommandText = "Select top 1 ProjectManagerID, Name from ProjectManager where pmaTSGCompanyID = @company and perDefaultJob = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    ddlProjectManager.SelectedValue = dr["ProjectManagerID"].ToString();
                }
                dr.Close();

                sql.CommandText = "Select CustomerID, CustomerName from Customer where cusInactive <> 1 or cusInactive is null order by CustomerName";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlCustomer.DataSource = dr;
                ddlCustomer.DataTextField = "CustomerName";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                dr.Close();
                if (!noQuote)
                {
                    ddlCustomer.Enabled = false;
                }

                if (master.getCompanyId() != 15)
                {
                    ddlLinkedJob.Visible = false;
                }
                sql.CommandText = "Select jdaJobName, jdaJobDashboardID from tblJobDashboard order by jdaCreated desc ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlLinkedJob.DataSource = dr;
                ddlLinkedJob.DataTextField = "jdaJobName";
                ddlLinkedJob.DataValueField = "jdaJobDashboardID";
                ddlLinkedJob.DataBind();
                dr.Close();
                ddlLinkedJob.Items.Insert(0, "No Linked Job");


                lblSalesman.Text = "";

                if (company == "13" || company == "20")
                {
                    sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID = 13 or TSGCompanyID = 20 ";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    ddlTSGCompany.DataSource = dr;
                    ddlTSGCompany.DataTextField = "TSGCompanyAbbrev";
                    ddlTSGCompany.DataValueField = "TSGCompanyID";
                    ddlTSGCompany.DataBind();
                    dr.Close();
                    ddlTSGCompany.SelectedValue = company;
                }
                else
                {
                    sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID < 16 and TSGCompanyID <> 4 and TSGCompanyID <> 6 and ";
                    sql.CommandText += "TSGCompanyID <> 11 and TSGCompanyID <> 1 order by TSGCompanyAbbrev";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    ddlTSGCompany.DataSource = dr;
                    ddlTSGCompany.DataTextField = "TSGCompanyAbbrev";
                    ddlTSGCompany.DataValueField = "TSGCompanyID";
                    ddlTSGCompany.DataBind();
                    dr.Close();
                    //ddlTSGCompany.Enabled = false;
                    ddlTSGCompany.SelectedValue = company;
                    ddlTSGCompany.Enabled = false;
                }
                

                

                sql.CommandText = "Select estEstimatorID, concat(estFirstName, ' ', estLastName) as Name from pktblEstimators where estCompanyID = @company order by Name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                dr = sql.ExecuteReader();
                ddlEstimator.DataSource = dr;
                ddlEstimator.DataTextField = "Name";
                ddlEstimator.DataValueField = "estEstimatorID";
                ddlEstimator.DataBind();
                dr.Close();

                sql.CommandText = "Select OEMName, OEMID from OEM order by OEMName";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlOEM.DataSource = dr;
                ddlOEM.DataTextField = "OEMName";
                ddlOEM.DataValueField = "OEMID";
                ddlOEM.DataBind();
                dr.Close();
                // Set to TBD by default
                ddlOEM.SelectedValue = "39";
                

                // Loading information from the quote into the page
                if (company != "9" && company != "13" && company != "15" && company != "20" && !noQuote)
                {
                    if(!SA)
                    {
                        sql.CommandText = "Select rfqID, prtRFQLineNumber, TSGCompanyAbbrev, quoVersion, rfqCustomerID, rfqPlantID, quoEstimatorID, ";
                        sql.CommandText += "CustomerContact.Name, prtPartNumber, ProgramName, rfqOEMID, TSGSalesman.Name, quoOldQuoteNumber, ";
                        sql.CommandText += "(select sum(pwnCostNote) from linkPWNToQuote, pktblPreWordedNote where pwqPreWordedNoteID = pwnPreWordedNoteID ";
                        sql.CommandText += "and pwqQuoteID = quoQuoteID) as cost, prtpartDescription, quoPartNumbers, quoPartName, quoPartNumbers  ";
                        sql.CommandText += "from linkPartToQuote, linkPartToRFQ, tblRFQ, tblPart, tblQuote, TSGCompany, ";
                        sql.CommandText += "CustomerContact, Program, CustomerLocation, TSGSalesman ";
                        sql.CommandText += "where ptqQuoteID = @id and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS <> 1 and ptqPartID = ptrPartID ";
                        sql.CommandText += "and rfqProgramID = ProgramID and rfqID = ptrRFQID and prtPARTID = ptrPartID and quoQuoteID = ptqQuoteID ";
                        sql.CommandText += "and TSGCompanyID = quoTSGCompanyID and CustomerContactID = rfqCustomerContact and CustomerLocationID = rfqPlantID ";
                        sql.CommandText += "and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", quoteID);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr.GetValue(12).ToString() != "")
                            {
                                txtQuoteNum.Text = dr.GetValue(12).ToString() + "-" + dr.GetValue(2).ToString() + "-" + dr.GetValue(3).ToString();
                            }
                            else
                            {
                                txtQuoteNum.Text = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(2).ToString() + "-" + dr.GetValue(3).ToString();
                            }
                            customer = dr.GetValue(4).ToString();
                            plant = dr.GetValue(5).ToString();
                            ddlEstimator.SelectedValue = dr.GetValue(6).ToString();
                            txtCustomerContact.Text = dr.GetValue(7).ToString();
                            if (dr.GetValue(16).ToString() != "")
                            {
                                txtPartName.Text = dr.GetValue(16).ToString();
                            }
                            else
                            {
                                txtPartName.Text = dr.GetValue(14).ToString();
                            }
                            txtProgram.Text = dr.GetValue(9).ToString();
                            ddlOEM.SelectedValue = dr.GetValue(10).ToString();
                            lblSalesman.Text = dr.GetValue(11).ToString();
                            customerContactId = dr.GetValue(12).ToString();
                            txtAmount.Text = System.Convert.ToDouble(dr.GetValue(13).ToString()).ToString("0.00");
                            if (dr.GetValue(17).ToString() != "")
                            {
                                txtCustomerPartNumber.Text = dr.GetValue(17).ToString();
                            }
                            else
                            {
                                txtCustomerPartNumber.Text = dr.GetValue(8).ToString();
                            }
                        }
                        dr.Close();
                    }
                    else
                    {
                        sql.CommandText = "Select TSGCompanyAbbrev, ecqVersion, ecqEstimator, ecqCustomer, ecqCustomerLocation, TSGSalesman.Name, ecqCustomerContactName, ";
                        sql.CommandText += "ecqPartNumber, ecqPartName, (select sum(pwnCostNote) from pktblPreWordedNote, linkPWNToECQuote where peqECQuoteID = ecqECQuoteID ";
                        sql.CommandText += "and peqPreWordedNoteID = pwnPreWordedNoteID), ecqQuoteNumber ";
                        sql.CommandText += "from tblECQuote, TSGCompany, TSGSalesman, CustomerLocation ";
                        sql.CommandText += "where TSGCompanyID = ecqTSGCompanyID and CustomerLocationID = ecqCustomerLocation and ";
                        sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and ecqECQuoteID = @id ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", quoteID);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr.GetValue(10).ToString() != "")
                            {
                                txtQuoteNum.Text = dr.GetValue(10).ToString() + "-" + dr.GetValue(0).ToString() + "-SA-" + dr.GetValue(1).ToString();
                            }
                            else
                            {
                                txtQuoteNum.Text = quoteID + "-" + dr.GetValue(0).ToString() + "-SA-" + dr.GetValue(1).ToString();
                            }
                            customer = dr.GetValue(3).ToString();
                            plant = dr.GetValue(4).ToString();
                            ddlEstimator.SelectedValue = dr.GetValue(2).ToString();
                            txtCustomerContact.Text = dr.GetValue(6).ToString();
                            txtPartName.Text = dr.GetValue(8).ToString();
                            txtCustomerPartNumber.Text = dr.GetValue(7).ToString();
                            lblSalesman.Text = dr.GetValue(5).ToString();
                            txtAmount.Text = System.Convert.ToDouble(dr.GetValue(9).ToString()).ToString("0.00");
                        }
                        dr.Close();
                    }
                    
                }
                else if (company == "9" && !noQuote)
                {
                    sql.CommandText = "Select rfqID, prtRFQLineNumber, TSGCompanyAbbrev, hquVersion, rfqCustomerID, rfqPlantID, hquEstimatorID, ";
                    sql.CommandText += "CustomerContact.Name, prtPartNumber, ProgramName, rfqOEMID, TSGSalesman.Name, CustomerContactID, ";
                    sql.CommandText += "(Select sum(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote where ";
                    sql.CommandText += "pthHTSQuoteID = hquHTSQuoteID and pthHTSPWNID = hpwHTSPreWordedNoteID) as cost ";
                    sql.CommandText += "from linkPartToQuote, linkPartToRFQ, tblRFQ, tblPart, tblHTSQuote, TSGCompany, ";
                    sql.CommandText += "CustomerContact, Program, CustomerLocation, TSGSalesman ";
                    sql.CommandText += "where ptqQuoteID = @id and ptqHTS = 1 and ptqSTS <> 1 and ptqUGS <> 1 and ptqPartID = ptrPartID and rfqProgramID = ProgramID ";
                    sql.CommandText += "and rfqID = ptrRFQID and prtPARTID = ptrPartID and hquHTSQuoteID = ptqQuoteID and TSGCompanyID = 9 ";
                    sql.CommandText += "and CustomerContactID = rfqCustomerContact  and CustomerLocationID = rfqPlantID and ";
                    sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", quoteID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        txtQuoteNum.Text = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(2).ToString() + "-" + dr.GetValue(3).ToString();
                        customer = dr.GetValue(4).ToString();
                        plant = dr.GetValue(5).ToString();
                        ddlEstimator.SelectedValue = dr.GetValue(6).ToString();
                        txtCustomerContact.Text = dr.GetValue(7).ToString();
                        txtPartName.Text = dr.GetValue(8).ToString();
                        txtProgram.Text = dr.GetValue(9).ToString();
                        ddlOEM.SelectedValue = dr.GetValue(10).ToString();
                        lblSalesman.Text = dr.GetValue(11).ToString();
                        customerContactId = dr.GetValue(12).ToString();
                        txtAmount.Text = System.Convert.ToDouble(dr.GetValue(13).ToString()).ToString("0.00");
                    }
                    dr.Close();

                    if(txtQuoteNum.Text == "")
                    {
                        sql.CommandText = "Select hquVersion, hquCustomerID, hquCustomerLocationID, hquEstimatorID, hquCustomerContactName, hquPartNumbers, hquPartName, ";
                        sql.CommandText += "(Select sum(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote where ";
                        sql.CommandText += "pthHTSQuoteID = hquHTSQuoteID and pthHTSPWNID = hpwHTSPreWordedNoteID) as cost, Name, hquNumber ";
                        sql.CommandText += "from tblHTSQuote, Customer, CustomerLocation, TSGSalesman ";
                        sql.CommandText += "where Customer.CustomerId = hquCustomerID and CustomerLocationID = hquCustomerLocationID and ";
                        sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and hquHTSQuoteID = @id ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", quoteID);
                        dr = sql.ExecuteReader();
                        if(dr.Read())
                        {
                            if(dr.GetValue(9).ToString() != "")
                            {
                                txtQuoteNum.Text = dr.GetValue(9).ToString() + "-HTS-SA-" + dr.GetValue(0).ToString();
                            }
                            else
                            {
                                txtQuoteNum.Text = quoteID + "-HTS-SA-" + dr.GetValue(0).ToString();
                            }
                            customer = dr.GetValue(1).ToString();
                            plant = dr.GetValue(2).ToString();
                            ddlEstimator.SelectedValue = dr.GetValue(3).ToString();
                            txtCustomerContact.Text = dr.GetValue(4).ToString();
                            txtCustomerPartNumber.Text = dr.GetValue(5).ToString();
                            txtPartName.Text = dr.GetValue(6).ToString();
                            txtAmount.Text = System.Convert.ToDouble(dr.GetValue(7).ToString()).ToString("0.00");
                            lblSalesman.Text = dr.GetValue(8).ToString();
                        }
                        dr.Close();
                    }
                }
                else if ((company == "13" || company == "20") && !noQuote)
                {
                    //chkSharepoint.Checked = false;
                    //chkSharepoint.Enabled = false;
                    sql.CommandText = "Select rfqID, prtRFQLineNumber, TSGCompanyAbbrev, squQuoteVersion, rfqCustomerID, rfqPlantID, squEstimatorID, ";
                    sql.CommandText += "CustomerContact.Name, prtPartNumber, ProgramName, rfqOEMID, TSGSalesman.Name, CustomerContactID, ";
                    sql.CommandText += "(select sum(sqnToolingCosts + sqnCapitalCosts) from pktblSTSQuoteNotes where sqnQuoteID = squSTSQuoteID) ";
                    sql.CommandText += "from linkPartToQuote, linkPartToRFQ, tblRFQ, tblPart, tblSTSQuote, TSGCompany, ";
                    sql.CommandText += "CustomerContact, Program, CustomerLocation, TSGSalesman ";
                    sql.CommandText += "where ptqQuoteID = @id and ptqHTS <> 1 and ptqSTS = 1 and ptqUGS <> 1 and ptqPartID = ptrPartID and rfqProgramID = ProgramID ";
                    sql.CommandText += "and rfqID = ptrRFQID and prtPARTID = ptrPartID and squSTSQuoteID = ptqQuoteID and TSGCompanyID = 13 ";
                    sql.CommandText += "and CustomerContactID = rfqCustomerContact  and CustomerLocationID = rfqPlantID and ";
                    sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", quoteID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        //txtQuoteNum.Text = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(2).ToString() + "-" + dr.GetValue(3).ToString();
                        customer = dr.GetValue(4).ToString();
                        plant = dr.GetValue(5).ToString();
                        ddlEstimator.SelectedValue = dr.GetValue(6).ToString();
                        txtCustomerContact.Text = dr.GetValue(7).ToString();
                        txtPartName.Text = dr.GetValue(8).ToString();
                        txtProgram.Text = dr.GetValue(9).ToString();
                        ddlOEM.SelectedValue = dr.GetValue(10).ToString();
                        lblSalesman.Text = dr.GetValue(11).ToString();
                        customerContactId = dr.GetValue(12).ToString();
                        txtAmount.Text = System.Convert.ToDouble(dr.GetValue(13).ToString()).ToString("0.00");
                    }
                    dr.Close();

                    sql.CommandText = "Select squQuoteNumber, squQuoteVersion, squCustomerId, squPlantID, squEstimatorID, squCustomerContact, squPartNumber, squPartName,  ";
                    sql.CommandText += "Name, (select sum(sqnToolingCosts + sqnCapitalCosts) from pktblSTSQuoteNotes where sqnQuoteID = squSTSQuoteID) ";
                    sql.CommandText += " as cost, TSGCompanyAbbrev, qtrRFQID, prtRFQLineNumber, assLineNumber ";
                    sql.CommandText += "from tblSTSQuote ";
                    sql.CommandText += "inner join Customer on CustomerID = squCustomerID ";
                    sql.CommandText += "inner join CustomerLocation on CustomerLocationID = squPlantID ";
                    sql.CommandText += "inner join TSGSalesman on TSGSalesman.TSGSalesmanID = squSalesmanID ";
                    sql.CommandText += "inner join TSGCompany on TSGCompanyID = squCompanyID ";
                    sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                    sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                    sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                    sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = squSTSQuoteID and atqSTS = 1 ";
                    sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                    sql.CommandText += "where squSTSQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", quoteID);
                    dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        if (dr["squQuoteNumber"].ToString().Contains("-"))
                        {
                            txtQuoteNum.Text = dr["squQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                        }
                        else if (dr["qtrRFQID"].ToString() == "")
                        {
                            if (dr["squQuoteNumber"].ToString() == "")
                            {
                                txtQuoteNum.Text = quoteID + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString();
                            }
                            else
                            {
                                txtQuoteNum.Text = dr["squQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                        else if (dr["assLineNumber"].ToString() != "")
                        {
                            txtQuoteNum.Text = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                        }
                        else
                        {
                            txtQuoteNum.Text = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                        }
                        customer = dr["squCustomerId"].ToString();
                        plant = dr["squPlantID"].ToString();
                        ddlEstimator.SelectedValue = dr["squEstimatorID"].ToString();
                        txtCustomerContact.Text = dr["squCustomerContact"].ToString();
                        txtCustomerPartNumber.Text = dr["squPartNumber"].ToString();
                        txtPartName.Text = dr["squPartName"].ToString();
                        lblSalesman.Text = dr["Name"].ToString();
                        txtAmount.Text = System.Convert.ToDouble(dr["cost"].ToString()).ToString("0.00");
                    }
                    dr.Close();
                    
                }
                else if (company == "15" && !noQuote)
                {
                    sql.CommandText = "Select rfqID, prtRFQLineNumber, TSGCompanyAbbrev, uquQuoteVersion, rfqCustomerID, rfqPlantID, uquEstimatorID, ";
                    sql.CommandText += "CustomerContact.Name, prtPartNumber, ProgramName, rfqOEMID, TSGSalesman.Name, CustomerContactID, uquTotalPrice ";
                    sql.CommandText += "from linkPartToQuote, linkPartToRFQ, tblRFQ, tblPart, tblUGSQuote, TSGCompany, ";
                    sql.CommandText += "CustomerContact, Program, CustomerLocation, TSGSalesman ";
                    sql.CommandText += "where ptqQuoteID = @id and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS = 1 and ptqPartID = ptrPartID and rfqProgramID = ProgramID ";
                    sql.CommandText += "and rfqID = ptrRFQID and prtPARTID = ptrPartID and uquUGSQuoteID = ptqQuoteID and TSGCompanyID = 15 ";
                    sql.CommandText += "and CustomerContactID = rfqCustomerContact  and CustomerLocationID = rfqPlantID and ";
                    sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", quoteID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        txtQuoteNum.Text = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString() + "-" + dr.GetValue(2).ToString() + "-" + dr.GetValue(3).ToString();
                        customer = dr.GetValue(4).ToString();
                        plant = dr.GetValue(5).ToString();
                        ddlEstimator.SelectedValue = dr.GetValue(6).ToString();
                        txtCustomerContact.Text = dr.GetValue(7).ToString();
                        txtPartName.Text = dr.GetValue(8).ToString();
                        txtProgram.Text = dr.GetValue(9).ToString();
                        ddlOEM.SelectedValue = dr.GetValue(10).ToString();
                        lblSalesman.Text = dr.GetValue(11).ToString();
                        customerContactId = dr.GetValue(12).ToString();
                        txtAmount.Text = System.Convert.ToDouble(dr.GetValue(13).ToString()).ToString("0.00");
                    }
                    dr.Close();

                    if(txtQuoteNum.Text == "")
                    {
                        sql.CommandText = "Select uquQuoteNumber, uquQuoteVersion, uquCustomerID, uquPlantID, uquEstimatorID, uquCustomerContact, ";
                        sql.CommandText += "uquPartNumber, uquPartName, Name, uquTotalPrice ";
                        sql.CommandText += "from tblUGSQuote, Customer, CustomerLocation, TSGSalesman ";
                        sql.CommandText += "where uquCustomerID = Customer.CustomerID and uquPlantID = CustomerLocationID and ";
                        sql.CommandText += "CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and uquUGSQuoteID = @id ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", quoteID);
                        dr = sql.ExecuteReader();
                        if(dr.Read())
                        {
                            if(dr.GetValue(0).ToString() != "")
                            {
                                txtQuoteNum.Text = dr.GetValue(0).ToString() + "-UGS-SA-" + dr.GetValue(1).ToString();
                            }
                            else
                            {
                                txtQuoteNum.Text = quoteID + "-UGS-SA-" + dr.GetValue(1).ToString();
                            }
                            customer = dr.GetValue(2).ToString();
                            plant = dr.GetValue(3).ToString();
                            ddlEstimator.SelectedValue = dr.GetValue(4).ToString();
                            txtCustomerContact.Text = dr.GetValue(5).ToString();
                            txtCustomerPartNumber.Text = dr.GetValue(6).ToString();
                            txtPartName.Text = dr.GetValue(7).ToString();
                            lblSalesman.Text = dr.GetValue(8).ToString();
                            txtAmount.Text = System.Convert.ToDouble(dr.GetValue(9).ToString()).ToString("0.00");
                        }
                        dr.Close();
                    }
                }

                //Getting new job number and setting display for UGS
                if (company == "15" && EC is false)
                {
                    txtJobNumber.ReadOnly = true;

                    sql.CommandText = "Select MAX(jnuID) from tblUgsJobNumber where jnuQuoteNumber = @quoteNum ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteNum", txtQuoteNum.Text);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        string temp = dr.GetValue(0).ToString();
                        //txtJobNumber.Text = "UGS-" + temp.PadLeft(6 , '0');
                        txtJobNumber.Text = temp;
                        //int tempJobNumber = dr.GetInt32(0);
                        //txtJobNumber.Text = ddlTSGCompany.SelectedItem + "-" + tempJobNumber.ToString("000000");
                        //txtJobNumber.Text = tempJobNumber.ToString("000000");
                    }
                    dr.Close();

                    //if (txtJobNumber.Text == "UGS-000000")
                    if (txtJobNumber.Text == "")
                    {

                        sql.CommandText = "insert into tblUgsJobNumber (jnuQuoteNumber, jnuCompany, jnuTimeCreated) Values (@quoteNum, @Company, GETDATE() ) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteNum", txtQuoteNum.Text);
                        sql.Parameters.AddWithValue("@Company", company);
                        master.ExecuteNonQuery(sql, "Reserve Job Number");

                        sql.CommandText = "Select MAX(jnuID) from tblUgsJobNumber where jnuQuoteNumber = @quoteNum ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteNum", txtQuoteNum.Text);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            string temp = dr.GetValue(0).ToString();
                            //txtJobNumber.Text = "UGS-" + temp.PadLeft(6, '0');
                            txtJobNumber.Text = temp;
                            //int tempJobNumber = dr.GetInt32(0);
                            //txtJobNumber.Text = ddlTSGCompany.SelectedItem + "-" + tempJobNumber.ToString("000000");
                            //txtJobNumber.Text = tempJobNumber.ToString("000000");
                        }
                        dr.Close();


                    }

                }

                //Getting new job number and setting display for ATS
                //lblShortJobNum.Style.Add("display", "none");
                //lblApndText.Style.Add("display", "none");
                //if (company == "2")
                //{
                //    sql.CommandText = "insert into tblJobNumber (jnuQuoteNumber, jnuCompany, jnuTimeCreated) Values (@quoteNum, @Company, GETDATE() ) ";
                //    sql.Parameters.Clear();
                //    sql.Parameters.AddWithValue("@quoteNum", txtQuoteNum.Text);
                //    sql.Parameters.AddWithValue("@Company", company);
                //    master.ExecuteNonQuery(sql, "Reserve Job Number");

                //    sql.CommandText = "Select MAX(jnuID) from tblJobNumber where jnuQuoteNumber = @quoteNum ";
                //    sql.Parameters.Clear();
                //    sql.Parameters.AddWithValue("@quoteNum", txtQuoteNum.Text);
                //    dr = sql.ExecuteReader();
                //    if (dr.Read())
                //    {
                //        int tempJobNumber = dr.GetInt32(0);
                //        lblJobNumber.Text = ddlTSGCompany.SelectedItem + "-" + tempJobNumber.ToString("000000");
                //        lblShortJobNum.Text = tempJobNumber.ToString();
                //    }
                //    dr.Close();

                //    txtJobNumber.Style.Add("display", "none");
                //}
                //else
                //{
                //    lblJobNumber.Style.Add("display", "none");
                //    txtJobNumApnd.Style.Add("display", "none");
                //}

                if (customer != "")
                {
                    ddlCustomer.SelectedValue = customer;

                    sql.CommandText = "Select CustomerLocationID, ShipToName from CustomerLocation where CustomerID = @customer order by ShipToName";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@customer", customer);
                    dr = sql.ExecuteReader();
                    ddlPlant.DataSource = dr;
                    ddlPlant.DataTextField = "ShipToName";
                    ddlPlant.DataValueField = "CustomerLocationID";
                    ddlPlant.DataBind();
                    dr.Close();
                }
                if (plant != "")
                {
                    ddlPlant.SelectedValue = plant;
                }
            }
           

            connection.Close();
        }

        protected void createJob (object sender, EventArgs e)
        {
            if (chkMasterboard.Checked)
            {
                createCapacityJob();
                lblSiteCreated.Text = "<font color='Red'>The Job has been created</font></ br>";
            }
            if (chkSharepoint.Checked)
            {
                if (company == "15")
                {
                    createUGSFolderStructure();
                }
                else
                {
                    createSharePointSite();
                }
                //lblSiteCreated.Text = "<font color='Red'>The Job has been created</font></ br>";
            }
            else if (noQuote)
            {
                createJobEntry();
            }
        }

        protected void createUGSFolderStructure()
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string customer = ddlCustomer.SelectedItem.ToString().Replace(".", "").Replace(",", "").Trim();

            //string url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + customer + "/" + txtJobNumber.Text;
            string url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/"  + txtJobNumber.Text;

            sql.CommandText = "update tblUGSQuote set uquStatusID = 7, uquWinLossID = 1, uquAwardedAmount = @amount, uquJobSiteUrl = @url, uquDateWon = GETDATE() ";
            sql.CommandText += "where uquUGSQuoteID = @id ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@amount", txtAmount.Text);
            sql.Parameters.AddWithValue("@id", quoteID);
            sql.Parameters.AddWithValue("@url", url);
            master.ExecuteNonQuery(sql, "Create Job Site");

            ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs");
            ctx.Credentials = master.getSharePointCredentials();
            Web web = ctx.Web;
            List list = web.Lists.GetByTitle("UGS Jobs");
            ctx.Load(list);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            
            var folder = list.RootFolder;
            ctx.Load(folder);
            
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //var customerFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + customer);
            var customerFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/");
            try
            {
                ctx.Load(customerFolder);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                
            }
            catch
            {
                ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                newItemInfo.LeafName = customer;
                var newFolder = list.AddItem(newItemInfo);
                newFolder["Title"] = customer;
                newFolder.Update();
                
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                ctx.Load(customerFolder);
                
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }

            customerFolder.Folders.Add(customerFolder.ServerRelativeUrl + "/" + txtJobNumber.Text);


            //customerFolder.AddSubFolder(txtJobNumber.Text);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //var jobFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + customer + "/" + txtJobNumber.Text);
            var jobFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + txtJobNumber.Text);
            ctx.Load(jobFolder);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            

            var templateFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/UGS%20New%20Job%20Template");
            ctx.Load(templateFolder);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            
            ctx.Load(templateFolder.Folders);
            ctx.Load(templateFolder.Files);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            


            connection.Close();


            lblSiteCreated.Text += "<a href='" + url + txtJobNumber.Text + "'> Click Here for the SharePoint Site</a>";

            //lblSiteCreated.Text = " <a href='" + "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + customer + "/" + txtJobNumber.Text + "'> Click Here for the SharePoint Site</a>";
            lblSiteCreated.Text = " <a href='" + "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + txtJobNumber.Text + "'> Click Here for the SharePoint Site</a>";

            copyFoldersAndFiles(jobFolder, templateFolder, ctx);

            createJobEntry();
        }

        protected void copyFoldersAndFiles(Folder jobFolder, Folder templateFolder, ClientContext ctx)
        {
            ctx.Load(templateFolder.Files);
            ctx.Load(templateFolder.Folders);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            
            foreach (var file in templateFolder.Files)
            {
                var targetFileUrl = file.ServerRelativeUrl.Replace(templateFolder.ServerRelativeUrl, jobFolder.ServerRelativeUrl);
                file.CopyTo(targetFileUrl, true);
            }
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            

            foreach (var subFolder in templateFolder.Folders)
            {
                var folderUrl = subFolder.ServerRelativeUrl.Replace(templateFolder.ServerRelativeUrl, jobFolder.ServerRelativeUrl);
                jobFolder.Folders.Add(jobFolder.ServerRelativeUrl + "/" + subFolder.Name);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                
                var nextTemplateFolder = ctx.Web.GetFolderByServerRelativeUrl(templateFolder.ServerRelativeUrl + "/" + subFolder.Name);


                var nextJobFolder = ctx.Web.GetFolderByServerRelativeUrl(jobFolder.ServerRelativeUrl + "/" + subFolder.Name);
                ctx.Load(nextTemplateFolder);
                ctx.Load(nextJobFolder);
                ctx.Load(nextTemplateFolder.ListItemAllFields);
                ctx.Load(nextTemplateFolder.ListItemAllFields.RoleAssignments);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                
                try
                {
                    nextJobFolder.ListItemAllFields.BreakRoleInheritance(false, false);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    
                    foreach (var role in nextTemplateFolder.ListItemAllFields.RoleAssignments)
                    {
                        ctx.Load(role.Member);
                        ctx.Load(role.RoleDefinitionBindings);
                        try
                        {
                            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                        }
                        catch (Exception err)
                        { }
                        if (role.Member.Title == "Limited Access System Group" || role.Member.Title == "UGS Job Creation" || role.Member.Title == "UGS Design Members")
                        {
                            continue;
                        }
                        nextJobFolder.ListItemAllFields.RoleAssignments.Add(role.Member, role.RoleDefinitionBindings);
                    }
                    try
                    {
                        SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    }
                    catch (Exception err)
                    { }

                }
                catch (Exception err)
                {

                }
                copyFoldersAndFiles(nextJobFolder, nextTemplateFolder, ctx);
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }

        protected void populate_Plants()
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select CustomerLocationID, ShipToName as Location from CustomerLocation where CustomerID=@customer  order by Location";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            SqlDataReader plantDR = sql.ExecuteReader();
            ddlPlant.DataSource = plantDR;
            ddlPlant.DataTextField = "Location";
            ddlPlant.DataValueField = "CustomerLocationID";
            ddlPlant.DataBind();
            plantDR.Close();
            //ddlPlant.SelectedValue = "0";
            
            connection.Close();
        }

        protected void createSharePointSite()
        {
            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = master.getConnectionString();
            connection.Open();
            sql.Connection = connection;

            string customer = ddlCustomer.SelectedItem.ToString().Replace(".", "").Replace(",", "").Trim();
            string company = ddlTSGCompany.SelectedItem.ToString();
            if (company == "GTS")
            {
                company = "Guo%20Ji";
            }
            if (company == "NIA")
            {
                company = "STS";
            }
            //if (company == "ATS")
            //{
            //    txtJobNumber.Text = lblJobNumber.Text + txtJobNumApnd.Text;
            //}
            string url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/" + company + "%20Jobs/Shared%20Documents/" + txtJobNumber.Text;

            ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/" + company  + "%20Jobs");
            ctx.Credentials = master.getSharePointCredentials();
            Web web = ctx.Web;
            List list = web.Lists.GetByTitle("ATP Files");
            try
            {
                ctx.Load(list);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                var folder = list.RootFolder;
                ctx.Load(folder);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                var jobFolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/" + company + "%20Jobs/ATP%20Files/" + txtJobNumber.Text);
                try
                {
                    ctx.Load(jobFolder);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                }
                catch
                {
                    ListItemCreationInformation newItemInfo = new ListItemCreationInformation();
                    newItemInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                    newItemInfo.LeafName = txtJobNumber.Text;
                    var newFolder = list.AddItem(newItemInfo);
                    newFolder["Title"] = txtJobNumber.Text;
                    newFolder.Update();
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    ctx.Load(jobFolder);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                }
            }
            catch
            {

            }

            string search = "";
            string templateName = "";
            url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/";

            if (ddlTSGCompany.SelectedItem.ToString() == "GTS")
            {
                url += "Guo Ji Jobs/";
                search = "Guo Ji" + "PM2.0";
            }
            else
            {
                url += ddlTSGCompany.SelectedItem.ToString() + " Jobs/";
                search = ddlTSGCompany.SelectedItem.ToString() + "PM2.0";
            }

            if (!noQuote)
            {
                if (company != "HTS" && company != "STS" && company != "UGS" && !SA)
                {
                    //Setting the disposition to won and sets the awarded amount
                    sql.CommandText = "update tblQuote set quoStatusID = 7, quoWinLossID = 1, quoAwardedAmount = @amount, quoJobSiteUrl = @url, quoDateWon = GETDATE() ";
                    sql.CommandText += "where quoQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sql.Parameters.AddWithValue("@id", quoteID);
                    sql.Parameters.AddWithValue("@url", url + txtJobNumber.Text);
                    master.ExecuteNonQuery(sql, "Create Job Site");
                }
                else if (company != "HTS" && company != "STS" && company != "UGS" && SA)
                {
                    sql.CommandText = "update tblECQuote set ecqStatus = 7, ecqWinLossID = 1, ecqAwardedAmount = @amount, ecqJobSiteUrl = @url, ecqDateWon = GETDATE() ";
                    sql.CommandText += "where ecqECQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sql.Parameters.AddWithValue("@id", quoteID);
                    sql.Parameters.AddWithValue("@url", url + txtJobNumber.Text);
                    master.ExecuteNonQuery(sql, "Create Job Site");
                }
                else if (company == "HTS")
                {
                    sql.CommandText = "update tblHTSQuote set hquStatusID = 7, hquWinLossID = 1, hquAwardedAmount = @amount, hquJobSiteUrl = @url, hquDateWon = GETDATE() ";
                    sql.CommandText += "where hquHTSQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sql.Parameters.AddWithValue("@id", quoteID);
                    sql.Parameters.AddWithValue("@url", url + txtJobNumber.Text);
                    master.ExecuteNonQuery(sql, "Create Job Site");
                }
                else if (company == "STS")
                {
                    sql.CommandText = "update tblSTSQuote set squStatusID = 7, squWinLossID = 1, squAwardedAmount = @amount, squJobSiteUrl = @url, squDateWon = GETDATE() ";
                    sql.CommandText += "where squSTSQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sql.Parameters.AddWithValue("@id", quoteID);
                    sql.Parameters.AddWithValue("@url", url + txtJobNumber.Text);
                    master.ExecuteNonQuery(sql, "Create Job Site");
                }
                else if (company == "UGS")
                {
                    sql.CommandText = "update tblUGSQuote set uquStatusID = 7, uquWinLossID = 1, uquAwardedAmount = @amount, uquJobSiteUrl = @url, uquDateWon = GETDATE() ";
                    sql.CommandText += "where uquUGSQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@amount", txtAmount.Text);
                    sql.Parameters.AddWithValue("@id", quoteID);
                    sql.Parameters.AddWithValue("@url", url + txtJobNumber.Text);
                    master.ExecuteNonQuery(sql, "Create Job Site");
                }
            }

            ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/");
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;
            WebTemplateCollection templates = web.GetAvailableWebTemplates(1033, true);
            ctx.Load(templates);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            
            foreach (WebTemplate template in templates)
            {
                if (template.Name.Contains(search))
                {
                    templateName = template.Name;
                    string test = template.Description;
                }
            }
            //This will create the actual site
            WebCreationInformation wc = new WebCreationInformation();
            wc.Description = txtJobNumber.Text + " Web Site.";
            wc.Language = 1033;
            wc.Title = txtJobNumber.Text;
            wc.Url = txtJobNumber.Text;
            wc.UseSamePermissionsAsParentSite = true;
            wc.WebTemplate = templateName;
            ctx = new ClientContext(url);
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;
            Web newWeb = web.Webs.Add(wc);
            ctx.Load(newWeb);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //This will add the job to the list
            ctx = new ClientContext(url);
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            list = ctx.Web.Lists.GetByTitle("Job List");
            ListItemCreationInformation info = new ListItemCreationInformation();
            ctx.Load(list);
            Microsoft.SharePoint.Client.ListItem item = list.AddItem(info);
            item["Title"] = "Edit";
            item["Customer"] = ddlCustomer.SelectedItem.ToString();
            item["Customer_x0020_Location"] = ddlPlant.SelectedItem.ToString();
            item["Job_x0020_Name"] = txtJobNumber.Text;
            FieldUrlValue urlValue = new FieldUrlValue();
            urlValue.Description = txtJobNumber.Text;
            urlValue.Url = url + txtJobNumber.Text;
            item["JobSiteUrl"] = urlValue;
            item["Company"] = ddlTSGCompany.SelectedItem.ToString();
            item["Program"] = txtProgram.Text;
            item["ProgramUrl"] = "";
            item["Job_x0020_Status"] = "Active";
            item["Part_x0020_Name"] = txtPartName.Text;
            item["Customer_x0020_Part_x0020_Number"] = txtCustomerPartNumber.Text;
            item["TSG_x0020_Project_x0020_Manager"] = ddlProjectManager.SelectedItem.ToString();
            item["ATP_x0020_Approval"] = "";
            item["Approver"] = "";
            item["Job_x0020_Created"] = DateTime.Now.ToShortDateString();
            item.Update();
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //PM2.0 Job site creation log list
            ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/TSG/PM20/");
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;
            list = ctx.Web.Lists.GetByTitle("Job Site log");
            info = new ListItemCreationInformation();
            ctx.Load(list);
            item = list.AddItem(info);
            item["Title"] = "Edit";
            item["Customer"] = ddlCustomer.SelectedItem.ToString();
            item["Customer_x0020_Location"] = ddlPlant.SelectedValue.ToString();
            item["Job_x0020_Name"] = txtJobNumber.Text;
            urlValue = new FieldUrlValue();
            urlValue.Description = txtJobNumber.Text;
            urlValue.Url = url + txtJobNumber.Text;
            item["JobSiteUrl"] = urlValue;
            item["Company"] = ddlTSGCompany.SelectedItem.ToString();
            item["Program"] = txtProgram.Text;
            FieldUrlValue programUrl = new FieldUrlValue();
            programUrl.Description = txtProgram.Text;
            programUrl.Url = "";
            item["ProgramUrl"] = programUrl;
            item["Job_x0020_Status"] = "Active";
            item["Part_x0020_Name"] = txtPartName.Text;
            item["Customer_x0020_Part_x0020_Number"] = txtCustomerPartNumber.Text;
            item["TSG_x0020_Project_x0020_Manager"] = ddlProjectManager.SelectedItem.ToString();
            item["Job_x0020_Created"] = DateTime.Now.ToShortDateString();
            item.Update();
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //Hidden item in the sharepoint Job Site
            ctx = new ClientContext(url + txtJobNumber.Text);
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            list = ctx.Web.Lists.GetByTitle("Project Number");
            info = new ListItemCreationInformation();
            ctx.Load(list);
            item = list.AddItem(info);
            item["Title"] = txtJobNumber.Text;
            item["ProjectNumber"] = txtJobNumber.Text;
            if (ddlTSGCompany.SelectedValue.ToString() != "8")
            {
                item["TSG_x0020_Company"] = ddlTSGCompany.SelectedItem.ToString();
            }
            else
            {
                item["TSG_x0020_Company"] = "Guo Ji";
            }
            item["Project_x0020_Manager"] = ddlProjectManager.SelectedItem.ToString();
            sql.CommandText = "Select OfficePhone, MobilePhone, Email from ProjectManager where ProjectManagerID = @id";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", ddlProjectManager.SelectedValue.ToString());
            SqlDataReader sdr = sql.ExecuteReader();
            if (sdr.Read())
            {
                item["PM_x0020_Phone"] = sdr.GetValue(0).ToString();
                item["PM_x0020_Email"] = sdr.GetValue(2).ToString();
            }
            sdr.Close();
            item["TSG_x0020_Estimating_x0020_Conta"] = ddlEstimator.SelectedItem.ToString();

            sql.CommandText = "Select estEmail, estOfficePhone from pktblEstimators where estEstimatorID = @id";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", ddlEstimator.SelectedValue.ToString());
            sdr = sql.ExecuteReader();
            if (sdr.Read())
            {
                item["TEC_x0020_Phone"] = sdr.GetValue(1).ToString();
                item["TEC_x0020_Email"] = sdr.GetValue(0).ToString();
            }
            sdr.Close();

            item["Customer"] = ddlCustomer.SelectedItem.ToString();
            item["Customer_x0020_Location"] = ddlPlant.SelectedItem.ToString();
            item["TSG_x0020_Sales_x0020_Rep"] = lblSalesman.Text;
            item["Customer_x0020_Contact"] = txtCustomerContact.Text;

            sql.CommandText = "Select MobilePhone, Email, OfficePhone from CustomerContact where CustomerContactID = @id ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", customerContactId);
            sdr = sql.ExecuteReader();
            if (sdr.Read())
            {
                if (sdr.GetValue(2).ToString() == "")
                {
                    item["CC_x0020_Phone"] = sdr.GetValue(0).ToString();
                }
                else
                {
                    item["CC_x0020_Phone"] = sdr.GetValue(2).ToString();
                }
                item["CC_x0020_Email"] = sdr.GetValue(1).ToString();
            }
            sdr.Close();

            item["Quote_x0020_Number"] = txtQuoteNum.Text;
            item["Amount"] = txtAmount.Text;
            item["Part_x0020_Name"] = txtPartName.Text;
            item["Customer_x0020_Part_x0020_Number"] = txtCustomerPartNumber.Text;
            item["Program_x0020_Name"] = txtProgram.Text;
            item["OEM"] = txtProgram.Text + "/" + ddlOEM.SelectedItem.ToString();
            FieldUrlValue programUrlValue = new FieldUrlValue();
            programUrlValue.Description = txtProgram.Text;
            programUrlValue.Url = "";
            item["ProgramUrl"] = programUrlValue;
            item.Update();
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);


            ctx = new ClientContext(url + txtJobNumber.Text);
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;

            //GTS Groups
            int gtsDesign = 0, gtsProjectManagers = 0, gtsEveryone = 0, gtsChinaProjectManagers = 0;
            //UGS Groups
            int ugsEstimating = 0, ugsOperations = 0, ugsPurchasing = 0, ugsQuality = 0, ugsShipping = 0;
            //Everyone else's Groups
            int estimating = 0, operations = 0, purchasing = 0, quality = 0, tsgPurchasing = 0, tsgLeadership = 0, tsgFinancial = 0;
            //ETS Extra Groups
            int etsTeamComputers = 0, etsShipping = 0, ctsDesign = 0;
            //BTS Groups
            int btsDesign = 0, btsShipping = 0, btsQuality = 0, btsFloor = 0;
            //ATS Groups
            int atsFloor = 0, atsQuality = 0, atsDesign = 0;
            //HTS Groups
            int htsShop = 0;

            GroupCollection collGroup = ctx.Web.SiteGroups;
            ctx.Load(collGroup);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            foreach (Group group in collGroup)
            {
                //Getting Guo Ji Groups
                if (ddlTSGCompany.SelectedValue.ToString() == "8")
                {
                    if (group.Title == "Guo Ji Design")
                    {
                        gtsDesign = group.Id;
                    }
                    else if (group.Title == "Guo Ji Project Managers")
                    {
                        gtsProjectManagers = group.Id;
                    }
                    else if (group.Title == "Guo Ji Everyone")
                    {
                        gtsEveryone = group.Id;
                    }
                    else if (group.Title == "Guo Ji China Project Managers")
                    {
                        gtsChinaProjectManagers = group.Id;
                    }
                }
                else if (ddlTSGCompany.SelectedValue.ToString() == "15")
                {
                    if (group.Title == "UGS Estimating")
                    {
                        ugsEstimating = group.Id;
                    }
                    else if (group.Title == "UGS Operations")
                    {
                        ugsOperations = group.Id;
                    }
                    else if (group.Title == "UGS Purchasing")
                    {
                        ugsPurchasing = group.Id;
                    }
                    else if (group.Title == "UGS Quality")
                    {
                        ugsQuality = group.Id;
                    }
                    else if (group.Title == "UGS Shipping")
                    {
                        ugsShipping = group.Id;
                    }
                }
                //new section added 07-20-17 for bts permissions   
                else if (ddlTSGCompany.SelectedValue.ToString() == "3")
                {
                    if (group.Title == "BTS Design")
                    {
                        btsDesign = group.Id;
                    }
                    else if (group.Title == "BTS Quality")
                    {
                        btsQuality = group.Id;
                    }
                    else if (group.Title == "BTS Shipping")
                    {
                        btsShipping = group.Id;
                    }
                    else if (group.Title == "BTS Shop Floor")
                    {
                        btsFloor = group.Id;
                    }
                }
                //New Section added 07-20-17 for ats permissions
                else if (ddlTSGCompany.SelectedValue.ToString() == "2")
                {
                    if (group.Title == "ATS Floor")
                    {
                        atsFloor = group.Id;
                    }
                    else if (group.Title == "ATS Quality")
                    {
                        atsQuality = group.Id;
                    }
                    else if (group.Title == "ATS Design")
                    {
                        atsDesign = group.Id;
                    }
                }
                else if (ddlTSGCompany.SelectedValue.ToString() == "13")
                {

                }
                else if (ddlTSGCompany.SelectedValue.ToString() == "9")
                {
                    if (group.Title == "HTS Shop")
                    {
                        htsShop = group.Id;
                    }
                }
                else
                {
                    if (ddlTSGCompany.SelectedValue.ToString() == "7")
                    {
                        if (group.Title == "ETS Team computers")
                        {
                            etsTeamComputers = group.Id;
                        }
                        else if (group.Title == "ETS Shipping")
                        {
                            etsShipping = group.Id;
                        }
                    }
                    if (group.Title == ddlTSGCompany.SelectedItem.ToString() + " Estimating")
                    {
                        estimating = group.Id;
                    }
                    else if (group.Title == ddlTSGCompany.SelectedItem.ToString() + " Operations")
                    {
                        operations = group.Id;
                    }
                    else if (group.Title == ddlTSGCompany.SelectedItem.ToString() + " Purchasing")
                    {
                        purchasing = group.Id;
                    }
                    else if (group.Title == ddlTSGCompany.SelectedItem.ToString() + " Quality")
                    {
                        quality = group.Id;
                    }
                }
                if (group.Title == "TSG Purchasing")
                {
                    tsgPurchasing = group.Id;
                }
                else if (group.Title == "TSG Leadership Members")
                {
                    tsgLeadership = group.Id;
                }
                else if (group.Title == "TSG Financial")
                {
                    tsgFinancial = group.Id;
                }
                else if (group.Title == "CTS Design")
                {
                    ctsDesign = group.Id;
                }
            }

            ctx = new ClientContext(url + txtJobNumber.Text);
            ctx.RequestTimeout = 5 * 60 * 1000;
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;
            if (ddlTSGCompany.SelectedValue.ToString() == "8")
            {
                //string folderName = "1 - Financials";

                //Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                //ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                //newFolder.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsDesign);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsProjectManagers);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsEveryone);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsChinaProjectManagers);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(tsgLeadership);

                //newFolder.ListItemAllFields.Update();
                //newFolder.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                //folderName = "7 - Purchasing and Outsourcing";
                //newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                //ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                //newFolder.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsDesign);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsProjectManagers);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsEveryone);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(gtsChinaProjectManagers);
                //newFolder.ListItemAllFields.Update();
                //newFolder.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.BreakRoleInheritance(true, false);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.RoleAssignments.Groups.RemoveById(gtsDesign);
                l.RoleAssignments.Groups.RemoveById(gtsProjectManagers);
                l.RoleAssignments.Groups.RemoveById(gtsChinaProjectManagers);
                l.RoleAssignments.Groups.RemoveById(gtsEveryone);
                //l.RoleAssignments.Groups.RemoveById(tsgPurchasing);
                l.RoleAssignments.Groups.RemoveById(ctsDesign);
                l.RoleAssignments.Groups.RemoveById(tsgFinancial);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            }
            //New section for HTS 02-26-19
            else if (ddlTSGCompany.SelectedValue.ToString() == "9")
            {
                string folderName = "1 - Financials";

                Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(htsShop);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.BreakRoleInheritance(true, false);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.RoleAssignments.Groups.RemoveById(htsShop);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
            //New section for BTS 07-20-17
            else if (ddlTSGCompany.SelectedValue.ToString() == "3")
            {
                string folderName = "1 - Financials";

                Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsShipping);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "7 - Purchasing and Outsourcing";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsShipping);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsFloor);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "13 - Data";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsShipping);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(btsFloor);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                //List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
                //ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //l.BreakRoleInheritance(true, false);
                //l.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //l.RoleAssignments.Groups.RemoveById(btsDesign);
                //l.RoleAssignments.Groups.RemoveById(btsShipping);
                //l.RoleAssignments.Groups.RemoveById(btsQuality);
                //l.RoleAssignments.Groups.RemoveById(btsFloor);
                //l.RoleAssignments.Groups.RemoveById(ctsDesign);
                //l.RoleAssignments.Groups.RemoveById(tsgFinancial);
                //l.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            }
            //New section for ATS 07-20-17
            else if (ddlTSGCompany.SelectedValue.ToString() == "2")
            {
                string folderName = "1 - Financials";

                Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "3 - Timelines";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "5 - Open Issues";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "6 - Tryout Material";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "7 - Purchasing and Outsourcing";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "1 - PO's";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/7 - Purchasing and Outsourcing/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "2 - Quotes";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/7 - Purchasing and Outsourcing/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "10 - Shippers and Packing Slips";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "11 - Pictures";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                //newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "Communications";

                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsFloor);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(atsQuality);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.BreakRoleInheritance(true, false);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.RoleAssignments.Groups.RemoveById(atsFloor);
                l.RoleAssignments.Groups.RemoveById(atsDesign);
                l.RoleAssignments.Groups.RemoveById(atsQuality);
                l.RoleAssignments.Groups.RemoveById(ctsDesign);
                l.RoleAssignments.Groups.RemoveById(tsgFinancial);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            }
            //else if (ddlTSGCompany.SelectedValue.ToString() == "15")
            //{
            //    string folderName = "1 - Financials";
            //    Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
            //    ctx.Load(web);
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
            //    newFolder.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsEstimating);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsOperations);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsPurchasing);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsQuality);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsShipping);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(tsgPurchasing);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(tsgLeadership);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
            //    newFolder.ListItemAllFields.Update();
            //    newFolder.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //    folderName = "7 - Purchasing and Outsourcing";
            //    newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
            //    ctx.Load(web);
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
            //    newFolder.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsEstimating);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsOperations);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ugsQuality);
            //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
            //    newFolder.ListItemAllFields.Update();
            //    newFolder.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //    List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
            //    ctx.Load(web);
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    l.BreakRoleInheritance(true, false);
            //    l.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    l.RoleAssignments.Groups.RemoveById(ugsEstimating);
            //    l.RoleAssignments.Groups.RemoveById(ugsOperations);
            //    l.RoleAssignments.Groups.RemoveById(ugsPurchasing);
            //    l.RoleAssignments.Groups.RemoveById(ugsQuality);
            //    l.RoleAssignments.Groups.RemoveById(ugsShipping);
            //    l.RoleAssignments.Groups.RemoveById(tsgPurchasing);
            //    l.RoleAssignments.Groups.RemoveById(ctsDesign);
            //    l.RoleAssignments.Groups.RemoveById(tsgFinancial);
            //    l.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //}
            else if (ddlTSGCompany.SelectedValue.ToString() != "13")
            {
                string folderName = "1 - Financials";
                Folder newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                if (ddlTSGCompany.SelectedValue.ToString() != "3")
                {
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(estimating);
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(operations);
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(quality);
                }
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(tsgPurchasing);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(tsgLeadership);

                if (ddlTSGCompany.SelectedValue.ToString() == "7")
                {
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(etsTeamComputers);
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(etsShipping);
                }
                else if (ddlTSGCompany.SelectedValue.ToString() != "3")
                {
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(purchasing);
                }
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "3 - Timelines";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                folderName = "5 - Open Issues";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                folderName = "6 - Tryout Material";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                folderName = "7 - Purchasing and Outsourcing";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                if (ddlTSGCompany.SelectedValue.ToString() != "3")
                {
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(estimating);
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(operations);
                    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(quality);
                }

                //if (ddlTSGCompany.SelectedValue.ToString() == "7")
                //{
                //    newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(etsShipping);
                //}
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                folderName = "8 - Check Fixture";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                folderName = "9 - Quality";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                folderName = "10 - Shippers and Packing Slips";
                newFolder = web.GetFolderByServerRelativeUrl(url + txtJobNumber.Text + "/Shared Documents/" + folderName);
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.BreakRoleInheritance(true, false);
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                newFolder.ListItemAllFields.RoleAssignments.Groups.RemoveById(ctsDesign);
                newFolder.ListItemAllFields.Update();
                newFolder.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                List l = web.GetList(url + txtJobNumber.Text + "/ATP Form");
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                l.BreakRoleInheritance(true, false);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                
                
                if (ddlTSGCompany.SelectedValue.ToString() == "7")
                {
                    l.RoleAssignments.Groups.RemoveById(etsTeamComputers);
                    l.RoleAssignments.Groups.RemoveById(etsShipping);
                }
                else if (ddlTSGCompany.SelectedValue.ToString() != "3")
                {
                    l.RoleAssignments.Groups.RemoveById(purchasing);
                    l.RoleAssignments.Groups.RemoveById(estimating);
                    l.RoleAssignments.Groups.RemoveById(operations);
                    l.RoleAssignments.Groups.RemoveById(quality);
                }
                l.RoleAssignments.Groups.RemoveById(tsgPurchasing);
                l.RoleAssignments.Groups.RemoveById(ctsDesign);
                //l.RoleAssignments.Groups.RemoveById(tsgFinancial);
                l.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
            lblSiteCreated.Text += "<a href='" + url + txtJobNumber.Text + "'> Click Here for the SharePoint Site</a>";

            sql.CommandText = "insert into tblJobDashboard (jdaJobName, jdaJobUrl, jdaCustomer, jdaCompany, jdaProgram, jdaProgramUrl, jdaPartName, ";
            sql.CommandText += "jdaCustomerPartName, jdaProjectManager, jdaJobStatus, jdaCreated, jdaCreatedBy, jdaCustomerLocation, jdaQuoteNumber, jdaSA ) ";
            sql.CommandText += "values (@jobName, @jobUrl, @customer, @company, @program, @programUrl, @partName, @custPartName, @projectManager, ";
            sql.CommandText += "@jobStatus, GETDATE(), @createdBy, @customerLocation, @quoteID, @sa ) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@jobName", txtJobNumber.Text);
            //sql.Parameters.AddWithValue("@newJobName", lblShortJobNum.Text);
            sql.Parameters.AddWithValue("@jobUrl", url + txtJobNumber.Text);
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedItem.ToString());
            if (ddlTSGCompany.SelectedItem.ToString() == "GTS")
            {
                sql.Parameters.AddWithValue("@company", "Guo Ji");
            }
            else
            {
                sql.Parameters.AddWithValue("@company", ddlTSGCompany.SelectedItem.ToString());
            }
            sql.Parameters.AddWithValue("@program", txtProgram.Text);
            sql.Parameters.AddWithValue("@programUrl", "");
            sql.Parameters.AddWithValue("@partName", txtPartName.Text);
            sql.Parameters.AddWithValue("@custPartName", txtCustomerPartNumber.Text);
            sql.Parameters.AddWithValue("@projectManager", ddlProjectManager.SelectedItem.ToString());
            sql.Parameters.AddWithValue("@jobStatus", "Active");
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            sql.Parameters.AddWithValue("@customerLocation", ddlPlant.SelectedItem.ToString());
            if (noQuote)
            {
                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
            }
            else
            {
                sql.Parameters.AddWithValue("@quoteID", quoteID);
            }
            sql.Parameters.AddWithValue("@sa", SA);
            master.ExecuteNonQuery(sql, "Created Job Site");

            connection.Close();
        }

        private void createJobEntry()
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            sql.CommandText = "insert into tblJobDashboard (jdaJobName, jdaJobUrl, jdaCustomer, jdaCompany, jdaProgram, jdaProgramUrl, jdaPartName, ";
            sql.CommandText += "jdaCustomerPartName, jdaProjectManager, jdaJobStatus, jdaCreated, jdaCreatedBy, jdaCustomerLocation, jdaSA, jdaQuoteNumber ) ";
            sql.CommandText += "output inserted.jdaJobDashboardID ";
            sql.CommandText += "values (@jobName, @jobUrl, @customer, @company, @program, @programUrl, @partName, @custPartName, @projectManager, ";
            sql.CommandText += "@jobStatus, GETDATE(), @createdBy, @customerLocation, @sa, @quoteID ) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@jobName", txtJobNumber.Text);
            //sql.Parameters.AddWithValue("@newJobName", lblShortJobNum.Text);
            if (ddlTSGCompany.SelectedValue == "15")
            {
                string customer = ddlCustomer.SelectedItem.ToString().Replace(".", "").Replace(",", "").Trim();
                //string url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" + customer + "/" + txtJobNumber.Text;
                string url = "https://toolingsystemsgroup.sharepoint.com/TSG/PM20/Ugs%20Jobs/Shared%20Documents/" +  txtJobNumber.Text;
                sql.Parameters.AddWithValue("@jobUrl", url);
            }
            else
            {
                sql.Parameters.AddWithValue("@jobUrl", "");
            }
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedItem.ToString());
            if (ddlTSGCompany.SelectedItem.ToString() == "gts")
            {
                sql.Parameters.AddWithValue("@company", "Guo Ji");
            }
            else
            {
                sql.Parameters.AddWithValue("@company", ddlTSGCompany.SelectedItem.ToString());
            }
            //sql.Parameters.AddWithValue("@company", ddlTSGCompany.SelectedItem.ToString());
            sql.Parameters.AddWithValue("@program", txtProgram.Text);
            sql.Parameters.AddWithValue("@programUrl", "");
            sql.Parameters.AddWithValue("@partName", txtPartName.Text);
            sql.Parameters.AddWithValue("@custPartName", txtCustomerPartNumber.Text);
            sql.Parameters.AddWithValue("@projectManager", ddlProjectManager.SelectedItem.ToString());
            sql.Parameters.AddWithValue("@jobStatus", "Active");
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            sql.Parameters.AddWithValue("@customerLocation", ddlPlant.SelectedItem.ToString());
            sql.Parameters.AddWithValue("@sa", SA);
            if (noQuote)
            {
                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
            }
            else
            {
                sql.Parameters.AddWithValue("@quoteID", quoteID);
            }
            string jobID = master.ExecuteScalar(sql, "Created Job Site").ToString();

            if (ddlTSGCompany.SelectedValue == "15" && ddlLinkedJob.SelectedValue != "0" && ddlLinkedJob.SelectedValue != "No Linked Job")
            {
                sql.CommandText = "update tblJobDashboard set jdaUGSJob = @ugsJob where jdaJobDashboardID = @jobID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@jobID", ddlLinkedJob.SelectedValue.ToString());
                sql.Parameters.AddWithValue("@ugsJob", jobID);
                master.ExecuteNonQuery(sql, "Create Job Site");
            }

            connection.Close();
        }

        private void createCapacityJob()
        {
            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = master.getConnectionString();
            connection.Open();
            sql.Connection = connection;

            //At the moment we only want to do this if it is ATS, DTS and ETS
            if(ddlTSGCompany.SelectedValue.ToString() == "15")
            {
                connection.Close();
                return;
            }

            SqlDataReader dr;
            String PartNumber = txtPartName.Text;
            String dieType = "";
            //String pictureURL = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/_layouts/15/start.aspx#/Part Pictures/";
            String pictureURL = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/";

            if (ddlTSGCompany.SelectedValue.ToString() == "9")
            {
                sql.CommandText = "Select hquProcess, hquPicture from tblHTSQuote where hquHTSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dieType = dr.GetValue(0).ToString();
                    pictureURL = dr.GetValue(1).ToString();
                }
                dr.Close();
            }
            else if (ddlTSGCompany.SelectedValue.ToString() == "15")
            {
                sql.CommandText = "Select uquDieType, uquPicture from tblUGSQuote where uquUGSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dieType = dr["uquDieType"].ToString();
                    pictureURL = dr["uquPicture"].ToString();
                }
                dr.Close();
            }
            else if (ddlTSGCompany.SelectedValue.ToString() == "13" || ddlTSGCompany.SelectedValue.ToString() == "20")
            {
                sql.CommandText = "Select squPicture from tblSTSQuote where squSTSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    pictureURL = "http://toolingsystemsgroup.sharepoint.com/sites/estimating/STS Quote pictures/" + dr["squPicture"].ToString();
                    // 105 is assembly
                    dieType = "105";
                }
                dr.Close();
            }
            else if(!SA)
            {
                sql.CommandText = "Select dinDieType, prtPartNumber, quoPartNumbers, prtPicture ";
                sql.CommandText += "from tblQuote, tblDieInfo, linkDieInfoToQuote, linkPartToQuote, tblPart where diqQuoteID = @id ";
                sql.CommandText += "and diqQuoteID = quoQuoteID and diqDieInfoID = dinDieInfoID and ptqQuoteID = quoQuoteID and ptqPartID = prtPARTID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dieType = dr.GetValue(0).ToString();
                    pictureURL += dr.GetValue(3).ToString();
                }
                dr.Close();
            }
            else
            {
                sql.CommandText = "Select ecqDieType, ecqPicture from tblECQuote where ecqECQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dieType = dr.GetValue(0).ToString();
                    pictureURL = dr.GetValue(1).ToString();
                }
                dr.Close();
            }


            Int64 CustomerId = System.Convert.ToInt64(ddlCustomer.SelectedValue.ToString());
            Int64 CompanyID = System.Convert.ToInt64(ddlTSGCompany.SelectedValue.ToString());
            DateTime NoDate = DateTime.Now.AddDays(-180);
            Int64 LocationID = System.Convert.ToInt64(ddlPlant.SelectedValue.ToString());
            string Number = txtJobNumber.Text;

            //Always set the version to scheduled
            Int64 VersionID = 1;
            DateTime DieDueDate = DateTime.Now;
            Decimal Margin = 0;
            Decimal Material = 0;


            List<CompanyProcess> Processes = new List<CompanyProcess>();

            sql.CommandText = "Select ProcessName from Process where DieTypeID = @dieType";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@dieType", dieType);
            dr = sql.ExecuteReader();
            while(dr.Read())
            {
                CompanyProcess newCP = new CompanyProcess();
                newCP.ProcessName = dr.GetValue(0).ToString();
                newCP.StartDate = NoDate;
                newCP.EndDate = NoDate;
                newCP.NextStart = NoDate;
                newCP.Hours = -1;
                Processes.Add(newCP);
            }
            dr.Close();


            Int64 PartNumberId = 0;
            sql.CommandText = "Select PartNameID from PartName where PartName = @name";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@name", PartNumber);
            dr = sql.ExecuteReader();
            if(dr.Read())
            {
                PartNumberId = System.Convert.ToInt64(dr.GetValue(0).ToString());
            }
            dr.Close();

            if (PartNumberId == 0)
            {
                sql.CommandText = "insert into PartName (PartName, partPictureURL) ";
                sql.CommandText += "output inserted.PartNameID ";
                sql.CommandText += "values (@name, @url) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", PartNumber);
                sql.Parameters.AddWithValue("@url", pictureURL);
                PartNumberId = System.Convert.ToInt64(master.ExecuteScalar(sql, "Create Job Site"));
            }

            string CustomerPartNumber = txtCustomerPartNumber.Text;
            Int64 CustomerPartNumberId = 0;
            sql.CommandText = "Select CustomerPartnumberID from CustomerPartNumber where CustomerPartNumber = @num ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@num", CustomerPartNumber);
            dr = sql.ExecuteReader();
            if(dr.Read())
            {
                CustomerPartNumberId = System.Convert.ToInt64(dr.GetValue(0).ToString());
            }
            dr.Close();

            if(CustomerPartNumberId == 0)
            {
                sql.CommandText = "Insert into CustomerPartNumber (CustomerPartNumber) ";
                sql.CommandText += "output inserted.CustomerPartnumberID ";
                sql.CommandText += "values (@num) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@num", CustomerPartNumber);
                CustomerPartNumberId = System.Convert.ToInt64(master.ExecuteScalar(sql, "Create Job Site"));
            }



            Int64 OEMId = System.Convert.ToInt64(ddlOEM.SelectedValue.ToString());


            // check for Program
            Int64 ProgramId = 0;

            sql.CommandText = "Select ProgramID from Program where ProgramName = @name";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@name", txtProgram.Text);
            dr = sql.ExecuteReader();
            if(dr.Read())
            {
                ProgramId = System.Convert.ToInt64(dr.GetValue(0).ToString());
            }
            dr.Close();

            if(ProgramId == 0)
            {
                sql.CommandText = "insert into Program (ProgramName) ";
                sql.CommandText += "output inserted.ProgramID ";
                sql.CommandText += "values (@name) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", txtProgram.Text);
                ProgramId = System.Convert.ToInt64(master.ExecuteScalar(sql, "Create Job Site"));
            }


            string Designer = "";
            string Engineer = "";
            // step 1 - save the job


            sql.CommandText = "insert into Job  (TSGCompanyID, Number, CustomerLocationID, QuoteNumber, PartNameId, ProjectManagerId, Designer, CustomerEngineer, ";
            sql.CommandText += "DieDeliveryDate, DieTypeId, JobVersionID, MaterialCost, Margin, TotalJobCost, OEMId, ProgramId, CustomerPartNumberId, jobActive, ";
            sql.CommandText += "jobCreated, JobCreatedBy) ";
            sql.CommandText += "OUTPUT INSERTED.JobId ";
            sql.CommandText += "values (@company, @number, @location, @quote, @part, @manager, @designer, @engineer, @deliverydate, @dietype, @version, @material, ";
            sql.CommandText += "@margin, @total, @oem, @program, @custpart, 1, GETDATE(), @user) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@company", CompanyID);
            sql.Parameters.AddWithValue("@dietype", dieType);
            sql.Parameters.AddWithValue("@number", Number);
            sql.Parameters.AddWithValue("@location", LocationID);
            sql.Parameters.AddWithValue("@quote", txtQuoteNum.Text);
            sql.Parameters.AddWithValue("@part", PartNumberId);
            sql.Parameters.AddWithValue("@custpart", CustomerPartNumberId);
            sql.Parameters.AddWithValue("@oem", OEMId);
            sql.Parameters.AddWithValue("@program", ProgramId);
            sql.Parameters.AddWithValue("@manager", ddlProjectManager.SelectedValue.ToString());
            sql.Parameters.AddWithValue("@designer", Designer);
            sql.Parameters.AddWithValue("@engineer", Engineer);
            sql.Parameters.AddWithValue("@deliverydate", DieDueDate.ToString("d"));
            sql.Parameters.AddWithValue("@version", VersionID);
            sql.Parameters.AddWithValue("@material", Material);
            sql.Parameters.AddWithValue("@margin", Margin);
            sql.Parameters.AddWithValue("@total", System.Convert.ToDecimal(txtAmount.Text));
            sql.Parameters.AddWithValue("@user", master.getUserName());

            Int64 newJobId = 0;

            newJobId = System.Convert.ToInt64(sql.ExecuteScalar());

            if (Request["atp"] != null)
            {
                var atp = Request["atp"].ToString();
                sql.CommandText = "insert into linkATPToJob (atjATPID, atjJobID, atjCreated, atjCreatedBy) ";
                sql.CommandText += "values(@atp, @job, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@atp", atp);
                sql.Parameters.AddWithValue("@job", newJobId);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Job Site Creation");
            }



            Decimal DemandCost = 0;
            // step 2 - save the job cost factors
            sql.Parameters.Clear();
            sql.CommandText = "select CostId, Percentage, CostName from cost  where TSGCompanyID=@company and DieTypeID=@die ";
            sql.Parameters.AddWithValue("@company", CompanyID);
            sql.Parameters.AddWithValue("@die", dieType);

            SqlConnection updconnection = new SqlConnection(master.getConnectionString());
            updconnection.Open();
            SqlCommand updsql = new SqlCommand();
            updsql.Connection = updconnection;

            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                Int64 codeid = System.Convert.ToInt64(dr.GetValue(0));
                Decimal pct = System.Convert.ToDecimal(dr.GetValue(1));
                Decimal Amount = System.Convert.ToDecimal(txtAmount.Text) * pct / 100;
                Amount = Math.Round(Amount, 2);
                if (dr.GetValue(2).ToString() == "Margin")
                {
                    if (Margin > 0)
                    {
                        Amount = Margin;
                    }
                }
                if (dr.GetValue(2).ToString() == "Material")
                {
                    if (Material > 0)
                    {
                        Amount = Material;
                    }
                }
                updsql.CommandText = "insert into jobcost (CostId, JobId, JobVersionId, BudgetedDollars, ActualDollars, HitDate) values (@cost, @job, @version, @amount, 0, @due) ";
                updsql.Parameters.Clear();
                updsql.Parameters.AddWithValue("@cost", codeid);
                updsql.Parameters.AddWithValue("@job", newJobId);
                updsql.Parameters.AddWithValue("@version", VersionID);
                updsql.Parameters.AddWithValue("@amount", Amount);
                updsql.Parameters.AddWithValue("@due", DieDueDate.ToString("d"));
                updsql.ExecuteNonQuery();
                if (dr.GetValue(2).ToString() != "Margin")
                {
                    if (dr.GetValue(2).ToString() != "Material")
                    {
                        DemandCost = DemandCost + Amount;
                    }
                    else
                    {
                        if (Material == 0)
                        {
                            updsql.CommandText = "update Job set MaterialCost = @amount where JobId= @job ";
                            updsql.Parameters.Clear();
                            updsql.Parameters.AddWithValue("@job", newJobId);
                            updsql.Parameters.AddWithValue("@amount", Amount);
                            updsql.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    if (Margin == 0)
                    {
                        updsql.CommandText = "update Job set Margin = @amount where JobId= @job ";
                        updsql.Parameters.Clear();
                        updsql.Parameters.AddWithValue("@job", newJobId);
                        updsql.Parameters.AddWithValue("@amount", Amount);
                        updsql.ExecuteNonQuery();
                    }
                }
            }
            dr.Close();

            updconnection.Close();
            connection.Close();
        }
    }

    public class CurrentDemand
    {
        public Int64 DemandID { get; set; }
        public Int64 JobID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal AverageDemand { get; set; }
    }
    public class CurrentCapacity
    {
        public String ProcessName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Capacity { get; set; }
    }
    public class CompanyProcess
    {
        public string ProcessName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Hours { get; set; }
        public DateTime NextStart { get; set; }
    }
}