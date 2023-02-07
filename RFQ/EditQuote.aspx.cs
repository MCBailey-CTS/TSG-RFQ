using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Optimization;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;
using System.Security;
using Microsoft.SharePoint.Client;

namespace RFQ
{
    public partial class EditQuote : System.Web.UI.Page
    {
        public Int64 quoteID = 0;
        public string partID = "";
        public Int64 rfqID = 0;
        public Boolean IsMasterCompany = false;
        public long UserCompanyID = 0;
        public int quoteType = 0;
        public int quoteNumber = 0;
        public int version = 0;
        public string historicalQuoteNumber = "";
        public int notes = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.GetPostBackEventReference(this, string.Empty);

            rfqID = System.Convert.ToInt64(Request["rfq"]);
            partID = Request["partID"];
            try
            {
                quoteType = System.Convert.ToInt32(Request["quoteType"]);
            }
            catch
            {

            }
            quoteID = System.Convert.ToInt64(Request["id"]);


            if(Request["quoteNumber"] != null)
            {
                try
                {
                    quoteNumber = System.Convert.ToInt32(Request["quoteNumber"]);
                }
                catch
                {
                    historicalQuoteNumber = Request["quoteNumber"];
                }
            }
            if (Request["version"] != null)
            {
                version = System.Convert.ToInt32(Request["version"]);
            }
            if (Request["notes"] != null)
            {
                notes = System.Convert.ToInt32(Request["notes"]);
            }

            hdnQuoteType.Value = Request["quoteType"];
            hdnQuoteNumber.Value = quoteID.ToString();

            if(historicalQuoteNumber != "")
            {
                btnSave_Click.Visible = false;
                btnNewVersion_Click.Visible = true;
            }
            if(quoteType == 1)
            {
                btnDeleteQuote_Click.Visible = true;
            }
            else
            {
                btnDeleteQuote_Click.Visible = false;
            }

            if (!IsPostBack)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                hdnCompany.Value = "1";

                try
                {
                    if(master.getCompanyId() == 1 && quoteType == 1)
                    {
                        sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID <> 6 and TSGCompanyID <> 4 and TSGCompanyID <> 11 and TSGCompanyID <> 9 and TSGCompanyID < 13";
                        sql.Parameters.Clear();
                        SqlDataReader companyDR = sql.ExecuteReader();
                        ddlTSGCompanyQuoting.DataSource = companyDR;
                        ddlTSGCompanyQuoting.DataTextField = "TSGCompanyAbbrev";
                        ddlTSGCompanyQuoting.DataValueField = "TSGCompanyID";
                        ddlTSGCompanyQuoting.DataBind();
                        companyDR.Close();
                        if(quoteID != 0)
                        {
                            sql.CommandText = "Select ecqTSGCompanyID from tblECQuote where ecqECQuoteID = @id";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", quoteID);
                            SqlDataReader sdr = sql.ExecuteReader();
                            if(sdr.Read())
                            {
                                ddlTSGCompanyQuoting.SelectedValue = sdr.GetValue(0).ToString();
                            }
                            sdr.Close();
                        }
                        lblTSGQuoting.Text = "TSG Company Quoting as";
                    }
                    else
                    {
                        ddlTSGCompanyQuoting.Visible = false;
                        lblTSGQuoting.Text = "";
                    }

                    if(quoteType != 1)
                    {
                        btnDuplicateQuote_Click.Visible = false;
                    }
                    else
                    {
                        btnDuplicateQuote_Click.Visible = true;
                    }

                    sql.CommandText = "select ptePaymentTermsID, ptePaymentTerms from pktblPaymentTerms order by ptePaymentTerms";
                    sql.Parameters.Clear();
                    SqlDataReader payDR = sql.ExecuteReader();
                    ddlPayment.DataSource = payDR;
                    ddlPayment.DataTextField = "ptePaymentTerms";
                    ddlPayment.DataValueField = "ptePaymentTermsID";
                    ddlPayment.DataBind();
                    payDR.Close();

                    if (master.getCompanyId() == 1 || master.getCompanyId() == 0)
                    {
                        sql.CommandText = "Select CONCAT (dtyFullName, ', ', TSGCompanyAbbrev) as name, DieTypeID from DieType, TSGCompany where DieType.TSGCompanyID = TSGCompany.TSGCompanyID and DieType.TSGCompanyID < 13 and DieType.TSGCompanyID <> 9 Order by TSGCompanyAbbrev";
                    }
                    else
                    {
                        sql.CommandText = "Select dtyFullName as name, DieTypeID from DieType where TSGCompanyID = @company Order by DieTypeID";
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    }
                    SqlDataReader processDR = sql.ExecuteReader();
                    ddlProcess.DataSource = processDR;
                    ddlProcess.DataTextField = "name";
                    ddlProcess.DataValueField = "DieTypeID";
                    ddlProcess.DataBind();
                    processDR.Close();

                    sql.CommandText = "select steShippingTermsID, steShippingTerms from pktblShippingTerms order by steShippingTerms";
                    sql.Parameters.Clear();
                    SqlDataReader stDR = sql.ExecuteReader();
                    ddlShipping.DataSource = stDR;
                    ddlShipping.DataTextField = "steShippingTerms";
                    ddlShipping.DataValueField = "steShippingTermsID";
                    ddlShipping.DataBind();
                    stDR.Close();

                    sql.CommandText = "Select qstQuoteStatusID, qstQuoteStatus from pktblQuoteStatus order by qstQuoteStatus";
                    sql.Parameters.Clear();
                    SqlDataReader qsDR = sql.ExecuteReader();
                    ddlStatus.DataSource = qsDR;
                    ddlStatus.DataTextField = "qstQuoteStatus";
                    ddlStatus.DataValueField = "qstQuoteStatusID";
                    ddlStatus.DataBind();
                    qsDR.Close();
                    ddlStatus.SelectedValue = "2";

                    sql.CommandText = "Select tcyToolCountry, tcyToolCountryID from pktblToolCountry";
                    sql.Parameters.Clear();
                    SqlDataReader countryDR = sql.ExecuteReader();
                    ddlCountry.DataSource = countryDR;
                    ddlCountry.DataTextField = "tcyToolCountry";
                    ddlCountry.DataValueField = "tcyToolCountryID";
                    ddlCountry.DataBind();
                    countryDR.Close();

                    sql.CommandText = "Select cavCavityName, cavCavityID from pktblCavity order by cavCavityID";
                    SqlDataReader cavDR = sql.ExecuteReader();
                    ddlCavity.DataSource = cavDR;
                    ddlCavity.DataTextField = "cavCavityName";
                    ddlCavity.DataValueField = "cavCavityID";
                    ddlCavity.DataBind();
                    cavDR.Close();

                    sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer where cusInactive <> 1 or cusInactive is null order by CustomerName ";
                    SqlDataReader CustomerDR = sql.ExecuteReader();
                    ddlCustomer.DataSource = CustomerDR;
                    ddlCustomer.DataTextField = "Name";
                    ddlCustomer.DataValueField = "CustomerID";
                    ddlCustomer.DataBind();
                    ddlCustomer.Items.Insert(0, "Please Select");
                    CustomerDR.Close();

                    sql.CommandText = "Select curCurrencyID, curCurrency from pktblCurrency order by curCurrency";
                    SqlDataReader dr = sql.ExecuteReader();
                    ddlCurrency.DataSource = dr;
                    ddlCurrency.DataTextField = "curCurrency";
                    ddlCurrency.DataValueField = "curCurrencyID";
                    ddlCurrency.DataBind();
                    dr.Close();
                    ddlCurrency.SelectedValue = "1";

                    if (historicalQuoteNumber == "")
                    {
                        sql.Parameters.Clear();
                        sql.CommandText = "Select CONCAT(estFirstName, ' ', estLastName) as 'name', estEstimatorID from pktblEstimators ";
                        if (master.getCompanyId() != 1)
                        {
                            sql.CommandText += "where estCompanyID = @company";
                            sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        }
                        SqlDataReader estimatorDR = sql.ExecuteReader();
                        ddlEstimator.DataSource = estimatorDR;
                        ddlEstimator.DataTextField = "name";
                        ddlEstimator.DataValueField = "estEstimatorID";
                        ddlEstimator.DataBind();
                        estimatorDR.Close();

                        sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @id";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", master.getUserName());
                        estimatorDR = sql.ExecuteReader();
                        if(estimatorDR.Read())
                        {
                            ddlEstimator.SelectedValue = estimatorDR.GetValue(0).ToString();
                        }
                        estimatorDR.Close();
                    }
                    else
                    {
                        ddlEstimator.Visible = false;
                    }
                }
                catch
                {

                }
                if (quoteID == 0 && rfqID != 0 && quoteType == 2)
                {
                    lblquoteID.Text = "New Quote";
                    try
                    {
                        SqlDataReader dr;


                        sql.CommandText = "Select prtPartDescription, rfqCustomerRFQNumber, customerName, ShipToName, CustomerLocation.TSGSalesManID, rfqProductTypeID, rfqOEMID, prtPartTypeID, prtPartNumber ";
                        sql.CommandText += "from tblRFQ, Customer, CustomerLocation, tblPart, linkPartToRFQ ";
                        sql.CommandText += "where rfqPlantID = CustomerLocationID and rfqCustomerID = Customer.CustomerID and prtPARTID = @partID and rfqID = @rfq and prtPartID = ptrPartID and ptrRFQID = @rfq";

                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@rfq", rfqID);
                        dr = sql.ExecuteReader();

                        if (dr.Read())
                        {
                            lblPartNumber.Text = dr.GetValue(8).ToString();
                            lblRfqNumber.Text = rfqID.ToString();
                            lblPartName.Text = dr.GetValue(0).ToString();
                            lblCustomerRFQ.Text = dr.GetValue(1).ToString();
                            lblCustomer.Text = dr.GetValue(2).ToString();
                            lblPlant.Text = dr.GetValue(3).ToString();
                            lblSalesman.Text = dr.GetValue(4).ToString();
                            hdnproductTypeID.Value = dr.GetValue(5).ToString();
                            hdnoemID.Value = dr.GetValue(6).ToString();
                            hdnpartTypeID.Value = dr.GetValue(7).ToString();
                            hdnpartID.Value = partID;
                        }

                        
                        dr.Close();

                        sql.CommandText = "binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialThicknessEnglish, binMaterialThicknessMetric ";
                        sql.CommandText += "from tblQuote, pktblBlankInfo ";
                        sql.CommandText += "where quoQuoteID = @quoteID and quoBlankInfoID = binBlankInfoID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", quoteID);

                        dr = sql.ExecuteReader();

                        if (dr.Read())
                        {
                            txtBlankWidthIn.Text = dr.GetValue(1).ToString();
                            txtBlankWidthMm.Text = dr.GetValue(2).ToString();
                            txtBlankPitchIn.Text = dr.GetValue(3).ToString();
                            txtBlankPitchMm.Text = dr.GetValue(4).ToString();
                            txtMaterialThkIn.Text = dr.GetValue(5).ToString();
                            txtMaterialThkMm.Text = dr.GetValue(6).ToString();
                        }
                        else
                        {
                            Response.Write("<script>alert('Could not locate blank information');</script>");
                        }
                    }
                    catch
                    {

                    }
                }
                //EC quote stuff atuo fill
                else if (quoteType == 1 && quoteID == 0)
                {
                    lblquoteID.Text = "New EC Quote";

                    ddlCavity.SelectedValue = 18.ToString();
                    txtBlankPitchIn.Text = 0.ToString();
                    txtBlankPitchMm.Text = 0.ToString();
                    txtBlankWidthIn.Text = 0.ToString();
                    txtBlankWidthMm.Text = 0.ToString();
                    txtMaterialThkIn.Text = 0.ToString();
                    txtMaterialThkMm.Text = 0.ToString();
                    txtFBIn.Text = 0.ToString();
                    txtFBMm.Text = 0.ToString();
                    txtLRIn.Text = 0.ToString();
                    txtLRMm.Text = 0.ToString();
                    txtShutIn.Text = 0.ToString();
                    txtShutMm.Text = 0.ToString();
                    txtStations.Text = 0.ToString();
                    if (master.getCompanyId() == 2)
                    {
                        ddlProcess.SelectedValue = 25.ToString();
                    }
                    else if (master.getCompanyId() == 5)
                    {
                        ddlProcess.SelectedValue = 27.ToString();
                    }
                    else if (master.getCompanyId() == 7)
                    {
                        ddlProcess.SelectedValue = 35.ToString();
                    }
                }
                else
                {

                }
                if (quoteID != 0)
                {
                    populate_Header();
                }

                // RTS wants to load default notes when they are creating a new quote for stand alone
                if (quoteID == 0 && notes == 0)
                {
                    int count = 0;

                    sql.CommandText = "Select dqnDefaultQuoteNote, dqnOrder, dqnCost from pktblDefaultQuoteNotes where dqnCompanyID = @company and (dqnQuoteType = 0 or dqnQuoteType = @quoteType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@quoteType", quoteType);
                    SqlDataReader dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + dr["dqnOrder"].ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr["dqnDefaultQuoteNote"].ToString()) + "','" + HttpUtility.JavaScriptStringEncode(dr["dqnCost"].ToString()) + "');", true);
                    }
                    dr.Close();
                }
                else if (quoteID == 0 && notes == 1)
                {
                    sql.CommandText = "Select dqnDefaultQuoteNote, dqnOrder, dqnCost from pktblDefaultQuoteNotes where dqnCompanyID = @company and dqnQuoteType = @quoteType order by dqnOrder ASC ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@quoteType", quoteType);
                    SqlDataReader dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + dr["dqnOrder"].ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr["dqnDefaultQuoteNote"].ToString()) + "','" + HttpUtility.JavaScriptStringEncode(dr["dqnCost"].ToString()) + "');", true);
                    }
                    dr.Close();
                }

                connection.Close();
            }
            else
            {
                if (quoteID == 0 && rfqID != 0)
                {
                    lblquoteID.Text = "New Quote";

                    try
                    {
                        Site master = new RFQ.Site();
                        SqlConnection connection = new SqlConnection(master.getConnectionString());
                        connection.Open();
                        SqlCommand sql = new SqlCommand();
                        sql.Connection = connection;

                        SqlDataReader dr;
                        sql.CommandText = "Select prtPartDescription, rfqCustomerRFQNumber, customerName, ShipToName, CustomerLocation.TSGSalesManID, binMaterialWidthEnglish, binMaterialWidthMetric, ";
                        sql.CommandText += "binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialThicknessEnglish, binMaterialThicknessMetric, prtPartNumber ";
                        sql.CommandText += "from tblRFQ, Customer, CustomerLocation, tblPart, linkPartToRFQ, pktblBlankInfo ";
                        sql.CommandText += "where rfqPlantID = CustomerLocationID and rfqCustomerID = Customer.CustomerID and prtPARTID = @partID and rfqID = @rfq and prtPartID = ptrPartID and ptrRFQID = @rfq and ";
                        sql.CommandText += "prtBlankInfoID = binBlankInfoID";

                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@rfq", rfqID);

                        dr = sql.ExecuteReader();

                        if (dr.Read())
                        {
                            lblPartNumber.Text = dr.GetValue(11).ToString();
                            lblRfqNumber.Text = rfqID.ToString();
                            lblPartName.Text = dr.GetValue(0).ToString();
                            lblCustomerRFQ.Text = dr.GetValue(1).ToString();
                            lblCustomer.Text = dr.GetValue(2).ToString();
                            lblPlant.Text = dr.GetValue(3).ToString();
                            lblSalesman.Text = dr.GetValue(4).ToString();
                            txtBlankWidthIn.Text = dr.GetValue(5).ToString();
                            txtBlankWidthMm.Text = dr.GetValue(6).ToString();
                            txtBlankPitchIn.Text = dr.GetValue(7).ToString();
                            txtBlankPitchMm.Text = dr.GetValue(8).ToString();
                            txtMaterialThkIn.Text = dr.GetValue(9).ToString();
                            txtMaterialThkMm.Text = dr.GetValue(10).ToString();
                        }
                        dr.Close();
                        connection.Close();
                    }
                    catch
                    {

                    }
                }
                else
                {
                    
                }
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }

        protected void populateGeneralNotes()
        {
            if (historicalQuoteNumber == "" && quoteType == 2)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "Select gnoGeneralNoteID, gnoDefault from pktblGeneralNote where gnoDefault = 1 and gnoCompany = @company";
                if (master.getCompanyId() == 3 || master.getCompanyId() == 8)
                {
                    sql.Parameters.AddWithValue("@company", "LCC");
                }
                else if (master.getCompanyId() == 9)
                {
                    sql.Parameters.AddWithValue("@company", "HTS");
                }
                else
                {
                    sql.Parameters.AddWithValue("@company", "general");
                }

                SqlDataReader dr = sql.ExecuteReader();
                List<string> genIDs = new List<string>();
                while (dr.Read())
                {
                    genIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select count(*) from linkGeneralNoteToQuote where gnqQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID);

                dr = sql.ExecuteReader();

                Boolean notes = false;
                if (dr.Read())
                {
                    if (dr.GetValue(0).ToString() != "0")
                    {
                        notes = true;
                    }
                }
                dr.Close();

                if (!notes)
                {
                    for (int k = 0; k < genIDs.Count; k++)
                    {
                        sql.Parameters.Clear();
                        sql.CommandText = "Insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                        sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@created", master.getUserName());
                        master.ExecuteNonQuery(sql, "Quote Upload");
                    }
                }
                sql.Parameters.Clear();

                sql.CommandText = "Select gnoGeneralNote as GeneralNote, (Select count(gnqGeneralNoteID) from linkGeneralNoteToQuote where gnqQuoteID = @quoteID and gnqGeneralNoteID = gnoGeneralNoteID) as num, gnoGeneralNoteID as ids from pktblGeneralNote where gnoCompany = @company";
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                if (master.getCompanyId() == 3 || master.getCompanyId() == 8)
                {
                    sql.Parameters.AddWithValue("@company", "LCC");
                }
                else if (master.getCompanyId() == 9)
                {
                    sql.Parameters.AddWithValue("@company", "HTS");
                }
                else
                {
                    sql.Parameters.AddWithValue("@company", "general");
                }

                dr = sql.ExecuteReader();
                dgGeneralNotes.DataSource = dr;
                dgGeneralNotes.DataBind();
                dgGeneralNotes.Visible = true;
                connection.Close();
            }
            else if (quoteType == 1 && historicalQuoteNumber == "")
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "Select gnoGeneralNoteID, gnoDefault from pktblGeneralNote where gnoDefault = 1 and gnoCompany = @company";
                if (master.getCompanyId() == 3 || master.getCompanyId() == 8)
                {
                    sql.Parameters.AddWithValue("@company", "LCC");
                }
                else if (master.getCompanyId() == 9)
                {
                    sql.Parameters.AddWithValue("@company", "HTS");
                }
                else
                {
                    sql.Parameters.AddWithValue("@company", "general");
                }

                SqlDataReader dr = sql.ExecuteReader();
                List<string> genIDs = new List<string>();
                while (dr.Read())
                {
                    genIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select count(*) from linkGeneralNoteToECQuote where gneECQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID);

                dr = sql.ExecuteReader();

                Boolean notes = false;
                if (dr.Read())
                {
                    if (dr.GetValue(0).ToString() != "0")
                    {
                        notes = true;
                    }
                }
                dr.Close();

                if (!notes)
                {
                    for (int k = 0; k < genIDs.Count; k++)
                    {
                        if (master.getCompanyId() == 3 || master.getCompanyId() == 8)
                        {
                            sql.Parameters.Clear();
                            sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                            sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                            sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            sql.Parameters.AddWithValue("@created", master.getUserName());
                            master.ExecuteNonQuery(sql, "Quote Upload");
                        }
                        else if (master.getCompanyId() == 9)
                        {
                            sql.Parameters.Clear();
                            sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                            sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                            sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            sql.Parameters.AddWithValue("@created", master.getUserName());
                            master.ExecuteNonQuery(sql, "Quote Upload");
                        }
                        else
                        {
                            if(k != 0 && k != 1)
                            {
                                sql.Parameters.Clear();
                                sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                                sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                                sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@created", master.getUserName());
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                        }
                    }
                }
                sql.Parameters.Clear();

                sql.CommandText = "Select gnoGeneralNote as GeneralNote, (Select count(gneGeneralNoteID) from linkGeneralNoteToECQuote where gneECQuoteID = @quoteID and gneGeneralNoteID = gnoGeneralNoteID) as num, gnoGeneralNoteID as ids from pktblGeneralNote where gnoCompany = @company";
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                if (master.getCompanyId() == 3 || master.getCompanyId() == 8)
                {
                    sql.Parameters.AddWithValue("@company", "LCC");
                }
                else if (master.getCompanyId() == 9)
                {
                    sql.Parameters.AddWithValue("@company", "HTS");
                }
                else
                {
                    sql.Parameters.AddWithValue("@company", "general");
                }
                dr = sql.ExecuteReader();
                dgGeneralNotes.DataSource = dr;
                dgGeneralNotes.DataBind();
                dgGeneralNotes.Visible = true;
                connection.Close();
            }
        }

        protected void btnFinalize_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (quoteType == 2)
            {
                sql.CommandText = "update tblQuote set quoFinalized = 1, quoModified = GETDATE(), quoModifiedBy = @user where quoQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Edit Quote");
            }
            else
            {
                sql.CommandText = "update tblECQuote set ecqFinalized = 1, ecqModified = GETDATE(), ecqModifiedBy = @user where ecqECQuoteId = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Edit Quote");
            }

            connection.Close();
            populate_Header();
        }

        protected void saveGeneralNotes_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quoteID";
            sql.Parameters.AddWithValue("@quoteID", quoteID);

            master.ExecuteNonQuery(sql, "EditQuote");

            int k = 0;

            if(quoteType == 2)
            {
                sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");
                foreach (GridViewRow row in dgGeneralNotes.Rows)
                {
                    CheckBox chk;
                    chk = (CheckBox)row.FindControl("num");
                    Label lbl = (Label)row.FindControl("id");
                    if (chk.Checked)
                    {
                        sql.Parameters.Clear();
                        sql.CommandText = "Insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                        sql.Parameters.AddWithValue("@noteID", lbl.Text);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@created", master.getUserName());
                        master.ExecuteNonQuery(sql, "Quote Upload");
                    }
                    k++;
                }
            }
            else
            {
                sql.CommandText = "Delete from linkGeneralNoteToECQuote where gneECQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");

                foreach (GridViewRow row in dgGeneralNotes.Rows)
                {
                    CheckBox chk;
                    chk = (CheckBox)row.FindControl("num");
                    Label lbl = (Label)row.FindControl("id");
                    if (chk.Checked)
                    {
                        sql.Parameters.Clear();
                        sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @created)";
                        sql.Parameters.AddWithValue("@noteID", lbl.Text);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@created", master.getUserName());
                        master.ExecuteNonQuery(sql, "Quote Upload");
                    }
                    k++;
                }
            }
            populate_Header();
            connection.Close();
        }

        protected void populate_Plants()
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;


            // Putting the notes back after the page posts back
            // We dont need to worry about the hdn note order since is not effected when the page postsback
            List<string> note = new List<string>();
            List<string> costNote = new List<string>();
            double totalCost = 0;

            try
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Request.Form["notes" + i.ToString()].ToString() != "" || Request.Form["price" + i.ToString()].ToString() != "")
                    {
                        note.Add(Request.Form["notes" + i.ToString()].ToString());
                        costNote.Add(Request.Form["price" + i.ToString()].ToString());
                        try
                        {
                            totalCost += System.Convert.ToDouble(Request.Form["price" + i.ToString()].ToString());
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
            for (int i = 0; i < note.Count; i++)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note[i].Replace("\'", "")) + "','" + HttpUtility.JavaScriptStringEncode(costNote[i].Replace("\'", "").Trim()) + "');", true);
            }
            txtTotalCost.Text = "Total: $" + totalCost.ToString();

            if (ddlCustomer.SelectedValue == "Please Select" && lblCustomer.Text != "")
            {
                sql.CommandText = "select CustomerLocationID, Concat(ShipToName, ' (',ShipCode,')', ' - ', Address1,', ', City,', ', State) as Location from CustomerLocation, Customer where Customer.CustomerID = CustomerLocation.CustomerID and CustomerName = @customer order by Location";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customer", lblCustomer.Text);
                SqlDataReader plantDR = sql.ExecuteReader();
                ddlPlant.DataSource = plantDR;
                ddlPlant.DataTextField = "Location";
                ddlPlant.DataValueField = "CustomerLocationID";
                ddlPlant.SelectedIndex = -1;
                ddlPlant.DataBind();
                plantDR.Close();
                ddlPlant.SelectedIndex = 0;

                if (lblPlant.Text != "")
                {
                    try
                    {
                        sql.CommandText = "Select CustomerLocationID from CustomerLocation where ShipToName = @customer";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@customer", lblPlant.Text);
                        plantDR = sql.ExecuteReader();
                        while (plantDR.Read())
                        {
                            ddlPlant.SelectedValue = plantDR.GetValue(0).ToString();
                        }
                        plantDR.Close();
                    }
                    catch
                    {
                        plantDR.Close();
                    }
                }

                

                lblPlant.Text = "";

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
            else if (ddlCustomer.SelectedValue != "Please Select")
            {
                sql.CommandText = "select CustomerLocationID, Concat(ShipToName, ' (',ShipCode,')', ' - ', Address1,', ', City,', ', State) as Location from CustomerLocation where CustomerID=@customer  order by Location";
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
            else
            {
                ddlPlant.Items.Clear();
            }
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

        protected void populate_Header()
        {
            Site master = new RFQ.Site();
            UserCompanyID = master.getCompanyId();

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            populateGeneralNotes();

            if (quoteType == 1)
            {
                lblquoteID.Text = quoteID.ToString().Trim() + " EC";

                sql.CommandText = "Select ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqPartNumber, ecqPartName, mtyMaterialType, ecqBlankWidthEng, ecqBlankWidthMet, ";
                sql.CommandText += "ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ecqShutHeightMet, ";
                sql.CommandText += "ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqRFQNumber, ecqTSGCompanyID, TSGSalesman.Name, ecqStatus, ecqEstimator, ecqJobNumber, ";
                sql.CommandText += "ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqVersion, ecqQuoteNumber, ecqShippingLocation, ecqTSGCompanyID, ecqFinalized ";
                sql.CommandText += "from Customer, CustomerLocation, TSGSalesman, tblECQuote ";
                sql.CommandText += "left outer join pktblMaterialType on ecqMaterialType = mtyMaterialTypeID ";
                sql.CommandText += "where ecqECQuoteID = @quoteID and ecqCustomer = Customer.CustomerID and ecqCustomerLocation = CustomerLocationID and TSGSalesman.TSGSalesmanID = CustomerLocation.TSGSalesmanID";

                sql.Parameters.AddWithValue("@quoteID", quoteID.ToString());
                SqlDataReader dr = sql.ExecuteReader();
                string cust = "", plant = "";
                if (dr.Read())
                {
                    try
                    {
                        ddlCustomer.SelectedValue = dr.GetValue(0).ToString();
                        cust = dr.GetValue(0).ToString();
                        if(ddlCustomer.SelectedValue == cust)
                        {
                            populate_Plants();
                        }
                        ddlPlant.SelectedValue = dr.GetValue(1).ToString();
                        plant = dr.GetValue(1).ToString();
                        txtCustomerRFQ.Text = dr.GetValue(2).ToString();
                        ddlProcess.SelectedValue = dr.GetValue(3).ToString();
                        ddlCavity.SelectedValue = dr.GetValue(4).ToString();
                        txtPartNumber.Text = dr.GetValue(5).ToString();
                        txtPartName.Text = dr.GetValue(6).ToString();
                        txtMaterialType.Text = dr.GetValue(7).ToString();
                        txtBlankWidthIn.Text = dr.GetValue(8).ToString();
                        txtBlankWidthMm.Text = dr.GetValue(9).ToString();
                        txtBlankPitchIn.Text = dr.GetValue(10).ToString();
                        txtBlankPitchMm.Text = dr.GetValue(11).ToString();
                        txtMaterialThkIn.Text = dr.GetValue(12).ToString();
                        txtMaterialThkMm.Text = dr.GetValue(13).ToString();
                        txtFBIn.Text = dr.GetValue(14).ToString();
                        txtFBMm.Text = dr.GetValue(15).ToString();
                        txtLRIn.Text = dr.GetValue(16).ToString();
                        txtLRMm.Text = dr.GetValue(17).ToString();
                        txtShutIn.Text = dr.GetValue(18).ToString();
                        txtShutMm.Text = dr.GetValue(19).ToString();
                        txtStations.Text = dr.GetValue(20).ToString();
                        txtLeadTime.Text = dr.GetValue(21).ToString();
                        ddlShipping.SelectedValue = dr.GetValue(22).ToString();
                        ddlPayment.SelectedValue = dr.GetValue(23).ToString();
                        ddlCountry.SelectedValue = dr.GetValue(24).ToString();
                        txtRFQNumber.Text = dr.GetValue(25).ToString();
                        Double quoteCompany = System.Convert.ToDouble(dr.GetValue(26).ToString());
                        ddlStatus.SelectedValue = dr.GetValue(28).ToString();
                        ddlEstimator.SelectedValue = dr.GetValue(29).ToString();
                        txtJobNumber.Text = dr.GetValue(30).ToString();
                        txtAccessNumber.Text = dr.GetValue(31).ToString();
                        cbUseTSG.Checked = dr.GetBoolean(32);
                        txtCustomerContact.Text = dr.GetValue(33).ToString();
                        lblVersion.Text = dr.GetValue(34).ToString();
                        lblQuoteNumber.Text = dr.GetValue(35).ToString();
                        txtShippingLocation.Text = dr.GetValue(36).ToString();
                        ddlTSGCompanyQuoting.SelectedValue = dr.GetValue(37).ToString();
                        hdnCompany.Value = dr.GetValue(37).ToString();
                        //if (quoteCompany != UserCompanyID && !master.getMasterCompany() || (ddlStatus.SelectedValue != "2" && ddlStatus.SelectedValue != "4" && ddlStatus.SelectedValue != "5"))
                        //{
                            //btnSave_Click.Visible = false;
                            //btnSaveQuote_Click.Visible = false;
                            //btnNewVersion_Click.Visible = false;
                            //btnSaveGeneralNotes.Visible = false;
                        //}
                        if (dr["ecqFinalized"].ToString() == "True")
                        {
                            litScript.Text = "<script>$('#btnCheck').hide();</script>";
                            //btnFinalize.Visible = false;
                            litStatus.Text = "<h3><font color='red'>This quote has been finalized and is not editable.</font></h3>";
                        }
                    }
                    catch (Exception e)
                    {

                    }
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






                sql.CommandText = "Select pwnPreWordedNote, pwnCostNote, pwnPreWordedNoteID from pktblPreWordedNote, linkPWNToECQuote where peqECQuoteID = @quoteID and peqPreWordedNoteID = pwnPreWordedNoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);

                dr = sql.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    //string test = dr.GetValue(0).ToString());
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr.GetValue(0).ToString().Replace("\'", "")) + "','" + HttpUtility.JavaScriptStringEncode(dr.GetValue(1).ToString().Replace("\'", "").Trim()) + "');", true);
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
            }
            else if (historicalQuoteNumber != "")
            {
                //ddlProcess.Visible = false;
                //ddlMatlType.Visible = false;
                //ddlCavity.Visible = false;
                //ddlMatlType.Visible = false;
                //ddlShipping.Visible = false;
                //ddlPayment.Visible = false;
                //ddlStatus.Visible = false;
                //ddlCustomer.Visible = false;
                //ddlPlant.Visible = false;
                //ddlCountry.Visible = false;
                txtPartName.Visible = false;
                txtPartNumber.Visible = false;
                txtRFQNumber.Visible = false;
                txtCustomerRFQ.Visible = false;
                btnSaveGeneralNotes.Visible = false;
                //txtCustomerContact.Visible = false;

                lblquoteID.Text = quoteID.ToString();
                lblQuoteNumber.Text = historicalQuoteNumber;
                try
                {
                    lblVersion.Text = historicalQuoteNumber.Split('-')[2];
                }
                catch
                {
                    lblVersion.Text = "000";
                }

                string salesOrderNum = "";
                string customer = "", plant = "", shipping = "", payment = "", process = "", cavity = "";
                sql.CommandText = "Select qhiBillToName, qhiShipToName, qhiCustomerRfqNum, qhiToolType, qhiCavity, qhiPartNumber, qhiPartDescription, qhiMaterialType, qhiDieFrontBackEng, ";
                sql.CommandText += "qhiDieFrontBackMet, qhiDieLeftRightEng, qhiDIeLeftRightMet, qhiShutHeightEng, qhiShutHeightMet, qhiNumberOfStations, qhiLeadTime, qhiShippingTerms, qhiPaymentTerms, ";
                sql.CommandText += "qhiRFQNumber, qhiPartWidthEng, qhiPartWidthMet, qhiPartPitchEng, qhiPartPitchMet, qhiSalesOrderNo, qhiDateCreated from tblQuoteHistory where qhiSalesOrderNo = @id";
                sql.Parameters.AddWithValue("@id", historicalQuoteNumber.Split('-')[0]);

                SqlDataReader dr = sql.ExecuteReader();

                if(dr.Read())
                {
                    customer = dr.GetValue(0).ToString();
                    plant = dr.GetValue(1).ToString();
                    lblCustomerRFQ.Text = dr.GetValue(2).ToString();
                    process = dr.GetValue(3).ToString();
                    cavity = dr.GetValue(4).ToString();
                    lblPartNumber.Text = dr.GetValue(5).ToString();
                    lblPartName.Text = dr.GetValue(6).ToString();
                    txtMaterialType.Text = dr.GetValue(7).ToString();
                    txtFBIn.Text = dr.GetValue(8).ToString();
                    txtFBMm.Text = dr.GetValue(9).ToString();
                    txtLRIn.Text = dr.GetValue(10).ToString();
                    txtLRMm.Text = dr.GetValue(11).ToString();
                    txtShutIn.Text = dr.GetValue(12).ToString();
                    txtShutMm.Text = dr.GetValue(13).ToString();
                    txtStations.Text = dr.GetValue(14).ToString();
                    txtLeadTime.Text = dr.GetValue(15).ToString();
                    shipping = dr.GetValue(16).ToString();
                    payment = dr.GetValue(17).ToString();
                    lblRfqNumber.Text = dr.GetValue(18).ToString();
                    txtBlankPitchIn.Text = dr.GetValue(19).ToString();
                    txtBlankPitchMm.Text = dr.GetValue(20).ToString();
                    txtBlankWidthIn.Text = dr.GetValue(21).ToString();
                    txtBlankWidthMm.Text = dr.GetValue(22).ToString();
                    salesOrderNum = dr.GetValue(23).ToString();
                    lblDateCreated.Text = dr.GetValue(24).ToString();

                    btnSaveQuote_Click.Visible = false;
                    //btnSave_Click.Visible = false;
                    //btnSaveQuote_Click.Visible = false;
                    //btnNewVersion_Click.Visible = false;
                    //btnSaveGeneralNotes.Visible = false;
                }
                dr.Close();

                sql.CommandText = "Select CustomerID from Customer where CustomerName like @customer";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customer", '%' + customer + "%");

                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    try
                    {
                        ddlCustomer.SelectedValue = dr.GetValue(0).ToString();
                    }
                    catch
                    {

                    }
                }
                dr.Close();

                populate_Plants();

                sql.CommandText = "Select CustomerLocationID from CustomerLocation where ShipToName like @plant";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@plant", '%' + plant + "%");

                dr = sql.ExecuteReader();
                if (dr.Read())
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

                sql.CommandText = "Select steShippingTermsID, ptePaymentTermsID from pktblShippingTerms, pktblPaymentTerms where steShippingTerms like @shipping and ptePaymentTerms like @payment";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@shipping", shipping);
                sql.Parameters.AddWithValue("@payment", payment);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    try
                    {
                        ddlShipping.SelectedValue = dr.GetValue(0).ToString();
                    }
                    catch
                    {

                    }
                    try
                    {
                        ddlPayment.SelectedValue = dr.GetValue(1).ToString();
                    }
                    catch
                    {

                    }
                }
                dr.Close();

                sql.CommandText = "Select DieTypeID, cavCavityID from DieType, pktblCavity where dtyFullName like @dieType and TSGCompanyID = @company and cavCavityName like @cavity";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@dieType", process);
                sql.Parameters.AddWithValue("@cavity", cavity);
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    try
                    {
                        ddlProcess.SelectedValue = dr.GetValue(0).ToString();
                    }
                    catch
                    {

                    }
                    try
                    {
                        ddlCavity.SelectedValue = dr.GetValue(1).ToString();
                    }
                    catch
                    {

                    }
                }
                dr.Close();

                try
                {
                    ddlCountry.SelectedValue = "2";
                }
                catch
                {

                }
                lblWarning.Text = " <font color='Red'>Please make sure Customer, Plant, Process, Cavity, Shipping and Payment are correct before creating new version!!!</ font>";

                sql.CommandText = "Select hpwPreWordedNote, hpwQuantityOrdered, hpwCostNote from pktblHistoricalPreWordedNote where hpwSalesOrderNo = @soNumber";
                sql.Parameters.AddWithValue("@soNumber", salesOrderNum);

                dr = sql.ExecuteReader();
                int i = 0;
                while(dr.Read())
                {
                    string note = dr.GetValue(0).ToString();
                    double cost = 0;
                    string temp = dr.GetValue(2).ToString();
                    try
                    {
                        cost = System.Convert.ToDouble(dr.GetValue(2));
                    }
                    catch
                    {

                    }

                    if (dr.GetValue(1).ToString() != "0" && dr.GetValue(1).ToString() != "1")
                    {
                        note += " Quantity: " + dr.GetValue(1).ToString();
                    }
                    try
                    {
                        cost = cost * System.Convert.ToDouble(dr.GetValue(1));
                    }
                    catch
                    {

                    }
                    string test = note.Replace("'", "\'");

                    if (cost == 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note) + "','" + null + "');\n", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note) + "','" + cost + "');\n", true);
                    }
                    i++;
                }

            }
            else if (quoteType == 2)
            {
                lblquoteID.Text = quoteID.ToString().Trim();
                txtPartNumber.Visible = false;
                //txtPartName.Visible = false;
                txtRFQNumber.Visible = false;
                ddlCustomer.Visible = false;
                //ddlPlant.Visible = false;
                txtCustomerRFQ.Visible = false;
                //txtCustomerContact.Visible = false;

                string tempPlant = "";


                sql.CommandText = "Select Distinct CustomerName, ShipToName, rfqCustomerRFQNumber, dinDieType, dinCavityID, prtPartNumber, prtpartDescription, mtyMaterialType, ";
                sql.CommandText += "dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, ";
                sql.CommandText += "dinNumberOfStations, quoLeadTime, quoShippingTermsID, quoPaymentTermsID, quoToolCountryID, rfqID, quoBlankInfoID, rfqProductTypeID, prtPartTypeID, rfqOEMID, prtPARTID, dinDieInfoID, ";
                sql.CommandText += "quoTSGCompanyID, TSGSalesman.Name, quoNumber, quoVersion, quoStatusID, quoCustomerQuoteNumber, quoPartNumbers, prtRFQLineNumber, quoPlant, quoUseTSGLogo, quoPartName, quoShippingLocation, ";
                sql.CommandText += "CustomerContact.Name, quoCustomerContact, quoAccess, quoTSGCompanyID, quoEstimatorID, quoFinalized, quoCurrencyID ";
                sql.CommandText += "from tblQuote, tblDieInfo, tblRFQ, linkQuoteToRFQ, linkDieInfoToQuote, linkPartToQuote, Customer, CustomerLocation, TSGSalesman, CustomerContact, tblPart ";
                sql.CommandText += "left outer join pktblMaterialType on prtPartMaterialType = mtyMaterialTypeID ";
                sql.CommandText += "where quoQuoteID = @quoteID and qtrQuoteID = @quoteID and qtrRFQID = rfqID and diqQuoteID = @quoteID and diqDieInfoId = dinDieInfoID and ptqQuoteID = @quoteID and ptqPartID = prtPARTID ";
                sql.CommandText += "and Customer.CustomerID = rfqCustomerID and rfqPlantID = CustomerLocationID and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and rfqCustomerContact = CustomerContactID ";

                sql.Parameters.AddWithValue("@quoteID", quoteID.ToString());
                SqlDataReader dr = sql.ExecuteReader();

                if (dr.Read())
                {
                    try
                    {
                        lblCustomer.Text = dr.GetValue(0).ToString();
                        lblPlant.Text = dr.GetValue(1).ToString();
                        lblCustomerRFQ.Text = dr.GetValue(2).ToString();
                        ddlProcess.SelectedValue = dr.GetValue(3).ToString();
                        ddlCavity.SelectedValue = dr.GetValue(4).ToString();
                        lblPartNumber.Text = dr.GetValue(5).ToString();
                        txtPartName.Text = dr.GetValue(6).ToString();
                        txtMaterialType.Text = dr.GetValue(7).ToString();
                        txtFBIn.Text = dr.GetValue(8).ToString();
                        txtFBMm.Text = dr.GetValue(9).ToString();
                        txtLRIn.Text = dr.GetValue(10).ToString();
                        txtLRMm.Text = dr.GetValue(11).ToString();
                        txtShutIn.Text = dr.GetValue(12).ToString();
                        txtShutMm.Text = dr.GetValue(13).ToString();
                        txtStations.Text = dr.GetValue(14).ToString();
                        txtLeadTime.Text = dr.GetValue(15).ToString();
                        ddlShipping.SelectedValue = dr.GetValue(16).ToString();
                        ddlPayment.SelectedValue = dr.GetValue(17).ToString();
                        ddlCountry.SelectedValue = dr.GetValue(18).ToString();
                        lblRfqNumber.Text = dr.GetValue(19).ToString();
                        hdnblankInfoID.Value = dr.GetValue(20).ToString();
                        hdnpartTypeID.Value = dr.GetValue(21).ToString();
                        hdnproductTypeID.Value = dr.GetValue(22).ToString();
                        hdnoemID.Value = dr.GetValue(23).ToString();
                        hdnpartID.Value = dr.GetValue(24).ToString();
                        hdndieInfoID.Value = dr.GetValue(25).ToString();
                        Double quoteCompany = System.Convert.ToDouble(dr.GetValue(26).ToString());
                        lblQuoteNumber.Text = dr.GetValue(28).ToString();
                        lblVersion.Text = dr.GetValue(29).ToString();
                        ddlStatus.SelectedValue = dr.GetValue(30).ToString();
                        txtCustQuoteNumber.Text = dr.GetValue(31).ToString();
                        txtWBPartNumber.Text = dr.GetValue(32).ToString();
                        ddlCurrency.SelectedValue = dr["quoCurrencyID"].ToString();
                        if ((quoteCompany != UserCompanyID && !master.getMasterCompany()) || (ddlStatus.SelectedValue != "2" && ddlStatus.SelectedValue != "4" && ddlStatus.SelectedValue != "5"))
                        {
                            //btnSave_Click.Visible = false;
                            //btnSaveQuote_Click.Visible = false;
                            //btnNewVersion_Click.Visible = false;
                            //btnSaveGeneralNotes.Visible = false;
                        }
                        tempPlant = dr.GetValue(34).ToString();
                        cbUseTSG.Checked = dr.GetBoolean(35);
                        if(dr.GetValue(36).ToString() != null && dr.GetValue(36).ToString() != "")
                        {
                            txtPartName.Text = dr.GetValue(36).ToString();
                        }
                        txtShippingLocation.Text = dr.GetValue(37).ToString();
                        if(dr.GetValue(39).ToString() != "")
                        {
                            txtCustomerContact.Text = dr.GetValue(39).ToString();
                        }
                        else
                        {
                            txtCustomerContact.Text = dr.GetValue(38).ToString();
                        }
                        txtAccessNumber.Text = dr.GetValue(40).ToString();
                        hdnCompany.Value = dr.GetValue(41).ToString();
                        ddlEstimator.SelectedValue = dr["quoEstimatorID"].ToString();
                        //if(dr.GetValue(34).ToString())
                        if (dr["quoFinalized"].ToString() == "True")
                        {
                            litScript.Text = "<script>$('#btnCheck').hide();</script>";
                            //btnFinalize.Visible = false;
                            litStatus.Text = "<h3><font color='red'>This quote has been finalized and is not editable.</font></h3>";
                        }
                    }
                    catch
                    {

                    }
                }
                dr.Close();

                if (tempPlant != "")
                {
                    sql.CommandText = "Select ShipToName from CustomerLocation where CustomerLocationID = @plant";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@plant", tempPlant);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        lblPlant.Text = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                    ddlPlant.SelectedValue = tempPlant;
                }

                populate_Plants();

                sql.CommandText = "Select binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, mtyMaterialType ";
                sql.CommandText += "from tblQuote, pktblBlankInfo ";
                sql.CommandText += "left outer join pktblMaterialType on binBlankMaterialTypeID = mtyMaterialTypeID ";
                sql.CommandText += "where quoQuoteID = @quoteID and quoBlankInfoID = binBlankInfoID";

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID.ToString());

                try
                {
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        txtMaterialThkIn.Text = System.Convert.ToDouble(dr.GetValue(0)).ToString("#.###");
                        txtMaterialThkMm.Text = dr.GetValue(1).ToString();
                        txtBlankPitchIn.Text = dr.GetValue(2).ToString();
                        txtBlankPitchMm.Text = dr.GetValue(3).ToString();
                        txtBlankWidthIn.Text = dr.GetValue(4).ToString();
                        txtBlankWidthMm.Text = dr.GetValue(5).ToString();
                        txtMaterialType.Text = dr.GetValue(6).ToString();
                    }
                    else
                    {
                        txtMaterialThkIn.Text = "";
                        txtMaterialThkMm.Text = "";
                        txtBlankPitchIn.Text = "";
                        txtBlankPitchMm.Text = "";
                        txtBlankWidthIn.Text = "";
                        txtBlankWidthMm.Text = "";
                        txtMaterialType.Text = "";
                    }

                    dr.Close();
                }
                catch
                {
                    txtMaterialThkIn.Text = "";
                    txtMaterialThkMm.Text = "";
                    txtBlankPitchIn.Text = "";
                    txtBlankPitchMm.Text = "";
                    txtBlankWidthIn.Text = "";
                    txtBlankWidthMm.Text = "";
                    txtMaterialType.Text = "";
                }
                sql.CommandText = "Select pwnPreWordedNote, pwnCostNote, pwnPreWordedNoteID from pktblPreWordedNote, linkPWNToQuote where pwqQuoteID = @quoteID and pwqPreWordedNoteID = pwnPreWordedNoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                double total = 0;

                dr = sql.ExecuteReader();
                int i = 0;
                while (dr.Read())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr.GetValue(0).ToString()) + "','" + HttpUtility.JavaScriptStringEncode(System.Convert.ToDouble(dr.GetValue(1).ToString()).ToString("0.00")) + "');", true);
                    try
                    {
                        total += System.Convert.ToDouble(dr.GetValue(1).ToString());
                    }
                    catch
                    {

                    }
                    if(i == 0)
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


                txtTotalCost.Text = "Total: $" + total.ToString();
            }
            connection.Close();
        }

        protected void btnReloadPage(object sender, EventArgs e)
        {
            populate_Header();
        }

        protected void btnDeleteQuoteClick(Object sender, EventArgs e)
        {
            if (quoteID == 0)
            {
                // Putting the notes back after the page posts back
                // We dont need to worry about the hdn note order since is not effected when the page postsback
                List<string> note = new List<string>();
                List<string> costNote = new List<string>();
                double totalC = 0;

                try
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Request.Form["notes" + i.ToString()].ToString() != "" || Request.Form["price" + i.ToString()].ToString() != "")
                        {
                            note.Add(Request.Form["notes" + i.ToString()].ToString());
                            costNote.Add(Request.Form["price" + i.ToString()].ToString());
                            try
                            {
                                totalC += System.Convert.ToDouble(Request.Form["price" + i.ToString()].ToString());
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch
                {

                }
                for (int i = 0; i < note.Count; i++)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note[i].Replace("\'", "")) + "','" + HttpUtility.JavaScriptStringEncode(costNote[i].Replace("\'", "").Trim()) + "');", true);
                }
                txtTotalCost.Text = "Total: $" + totalC.ToString();

                litQuoteScripts.Text = "<script>alert('Cannot delete quote since it has not been saved yet.');</script>";
                return;
            }
            if (quoteType == 1)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "delete from linkGeneralNoteToECQuote where gneECQuoteID = @ID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ID", quoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");

                List<string> pwnID = new List<string>();
                sql.CommandText = "Select peqPreWordedNoteID from linkPWNToECQuote where peqECQuoteID = @ID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    pwnID.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "delete from linkPWNToECQuote where peqECQuoteID = @ID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ID", quoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");

                for (int i = 0; i < pwnID.Count; i++)
                {
                    sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", pwnID[i]);
                    master.ExecuteNonQuery(sql, "Edit Quote");
                }


                sql.CommandText = "delete from tblECQuote where ecqECQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");

                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=0&quoteType=" + 1);

                connection.Close();
            }
        }

        protected void duplicateQuote(object sender, EventArgs e)
        {
            //All the same code to save the quote
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (ddlCustomer.SelectedValue == "Please Select")
            {
                // Putting the notes back after the page posts back
                // We dont need to worry about the hdn note order since is not effected when the page postsback
                List<string> note = new List<string>();
                List<string> costNote = new List<string>();
                double totalC = 0;

                try
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Request.Form["notes" + i.ToString()].ToString() != "" || Request.Form["price" + i.ToString()].ToString() != "")
                        {
                            note.Add(Request.Form["notes" + i.ToString()].ToString());
                            costNote.Add(Request.Form["price" + i.ToString()].ToString());
                            try
                            {
                                totalC += System.Convert.ToDouble(Request.Form["price" + i.ToString()].ToString());
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch
                {

                }
                for (int i = 0; i < note.Count; i++)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note[i].Replace("\'", "")) + "','" + HttpUtility.JavaScriptStringEncode(costNote[i].Replace("\'", "").Trim()) + "');", true);
                }
                txtTotalCost.Text = "Total: $" + totalC.ToString();

                litQuoteScripts.Text = "<script>alert('Please select customer before trying to duplicate quote.');</script>";
                return;
            }

            //try to execute if you can delete everything and prompt where it failed
            List<int> insertedNotes = new List<int>();
            // loop
            int totalCost = 0;
            try
            {
                int count = 0;
                for (int i = 0; i < 100; i++)
                {
                    if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                    {
                        sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                        sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                        sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                        sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                        sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString().Replace(',', '.'));
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                        int noteID = 0;
                        //string test = master.ExecuteScalar(sql, "EditQuote").ToString();
                        insertedNotes.Add(noteID = System.Convert.ToInt32(sql.ExecuteScalar().ToString()));
                        try
                        {
                            totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                        }
                        catch
                        {

                        }
                    }
                    count++;
                }
            }
            catch
            {

            }


            sql.Parameters.Clear();
            sql.CommandText = "Select TSGSalesman.TSGSalesmanID from TSGSalesman where Name = @salesman";
            sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
            int salesman = 0;
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
            }
            dr.Close();

            string matID = "";
            sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                matID = dr.GetValue(0).ToString();
            }
            dr.Close();
            if (matID == "")
            {
                sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                sql.CommandText += "output inserted.mtyMaterialTypeID ";
                sql.CommandText += "values (@matType) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                matID = sql.ExecuteScalar().ToString();
            }

            sql.Parameters.Clear();

            sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
            sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
            sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqVersion, ecqQuoteNumber, ecqShippingLocation ) ";
            sql.CommandText += "Output inserted.ecqECQuoteID ";
            sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
            sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @custContact, @version, @quoteNumber, @shippingLocation )";

            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text.ToString());
            sql.Parameters.AddWithValue("@partName", txtPartName.Text.ToString());
            sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text.ToString());
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            sql.Parameters.AddWithValue("@CustomerLocation", ddlPlant.SelectedValue);
            sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text.ToString());
            sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
            sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
            sql.Parameters.AddWithValue("@blankWidthEng", txtBlankWidthIn.Text.ToString());
            sql.Parameters.AddWithValue("@blankWidthMet", txtBlankWidthMm.Text.ToString());
            sql.Parameters.AddWithValue("@blankPitchEng", txtBlankPitchIn.Text.ToString());
            sql.Parameters.AddWithValue("@blankPitchMet", txtBlankPitchMm.Text.ToString());
            sql.Parameters.AddWithValue("@matThkEng", txtMaterialThkIn.Text.ToString());
            sql.Parameters.AddWithValue("@matThkMet", txtMaterialThkMm.Text.ToString());
            sql.Parameters.AddWithValue("@FBEng", txtFBIn.Text.ToString());
            sql.Parameters.AddWithValue("@FBMet", txtFBMm.Text.ToString());
            sql.Parameters.AddWithValue("@LREng", txtLRIn.Text.ToString());
            sql.Parameters.AddWithValue("@LRMet", txtLRMm.Text.ToString());
            sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
            sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
            sql.Parameters.AddWithValue("@matType", matID);
            sql.Parameters.AddWithValue("@stations", txtStations.Text.ToString());
            sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
            sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
            sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
            sql.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
            sql.Parameters.AddWithValue("@totalCost", totalCost);
            sql.Parameters.AddWithValue("@status", 2);
            sql.Parameters.AddWithValue("@salesman", salesman);
            sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
            sql.Parameters.AddWithValue("@jobNumber", txtJobNumber.Text);
            sql.Parameters.AddWithValue("@accessNumber", txtAccessNumber.Text);
            sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
            sql.Parameters.AddWithValue("@custContact", txtCustomerContact.Text);
            sql.Parameters.AddWithValue("@version", "001");
            sql.Parameters.AddWithValue("@quoteNumber", "");
            sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);

            //We need to insert these into table for EC quotes
            int newECQuoteID = System.Convert.ToInt32(sql.ExecuteScalar().ToString());
            sql.Parameters.Clear();
            Response.Write("<script>alert('this is the note count to be linked " + insertedNotes.Count + "');</script>");
            for (int k = 0; k < insertedNotes.Count; k++)
            {
                sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                sql.Parameters.AddWithValue("@quoteID", newECQuoteID);
                sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "EditQuote");

                sql.Parameters.Clear();
            }

            String pictureName = "EC-" + newECQuoteID + ".png";
            newPicture(pictureName);

            sql.CommandText = "Update tblECQuote set ecqPicture = @picture, ecqQuoteNumber = @quoteNum where ecqECQuoteID = @ecQuoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteNum", newECQuoteID);
            sql.Parameters.AddWithValue("@ecQuoteID", newECQuoteID);
            sql.Parameters.AddWithValue("@picture", pictureName);
            master.ExecuteNonQuery(sql, "Edit Quote");
            

            //add in quote type
            Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newECQuoteID + "&quoteType=" + 1);
            connection.Close();
        }

        //This is used to create a new revision of the current quote
        protected void btncreateNewVersionClick(object sender, EventArgs e)
        {
            //All the same code to save the quote
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (quoteID == 0 && historicalQuoteNumber == "")
            {
                // Putting the notes back after the page posts back
                // We dont need to worry about the hdn note order since is not effected when the page postsback
                List<string> note = new List<string>();
                List<string> costNote = new List<string>();
                double totalc = 0;

                try
                {
                    for (int i = 0; i < 200; i++)
                    {
                        if (Request.Form["notes" + i.ToString()].ToString() != "" || Request.Form["price" + i.ToString()].ToString() != "")
                        {
                            note.Add(Request.Form["notes" + i.ToString()].ToString());
                            costNote.Add(Request.Form["price" + i.ToString()].ToString());
                            try
                            {
                                totalc += System.Convert.ToDouble(Request.Form["price" + i.ToString()].ToString());
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch
                {

                }
                for (int i = 0; i < note.Count; i++)
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(note[i].Replace("\'", "")) + "','" + HttpUtility.JavaScriptStringEncode(costNote[i].Replace("\'", "").Trim()) + "');", true);
                }
                txtTotalCost.Text = "Total: $" + totalc.ToString();

                litQuoteScripts.Text = "<script>alert('Cannot create new version because this quote has not been saved yet');</script>";
                return;
            }


            //try to execute if you can delete everything and prompt where it failed
            List<int> insertedNotes = new List<int>();
            // loop
            int totalCost = 0;
            try
            {
                int count = 0;
                for(int i = 0; i < 100; i++)
                {
                    if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                    {
                        sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                        sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                        sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                        sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                        sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString());
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                        int noteID = 0;
                        insertedNotes.Add(noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote")));
                        try
                        {
                            totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                        }
                        catch
                        {

                        }
                    }
                    count++;
                }
            }
            catch
            {

            }

            if(historicalQuoteNumber != "")
            {
                sql.Parameters.Clear();
                sql.CommandText = "Select TSGSalesman.TSGSalesmanID from TSGSalesman where Name = @salesman";
                sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                string matID = "";
                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    matID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if (matID == "")
                {
                    sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                }

                sql.Parameters.Clear();

                int versionNum = 0;
                try
                {
                    versionNum = System.Convert.ToInt32(lblQuoteNumber.Text.Split('-')[2]) + 1;
                }
                catch
                {
                    versionNum = 1;
                }

                sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqVersion, ecqQuoteNumber, ecqMasQuote, ecqShippingLocation ) ";
                sql.CommandText += "Output inserted.ecqECQuoteID ";
                sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @custContact, @version, @quoteNumber, 1, @shippingLocation )";

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partName", txtPartName.Text.ToString());
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text.ToString());
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@CustomerLocation", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text.ToString());
                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@blankWidthEng", txtBlankWidthIn.Text.ToString());
                sql.Parameters.AddWithValue("@blankWidthMet", txtBlankWidthMm.Text.ToString());
                sql.Parameters.AddWithValue("@blankPitchEng", txtBlankPitchIn.Text.ToString());
                sql.Parameters.AddWithValue("@blankPitchMet", txtBlankPitchMm.Text.ToString());
                sql.Parameters.AddWithValue("@matThkEng", txtMaterialThkIn.Text.ToString());
                sql.Parameters.AddWithValue("@matThkMet", txtMaterialThkMm.Text.ToString());
                sql.Parameters.AddWithValue("@FBEng", txtFBIn.Text.ToString());
                sql.Parameters.AddWithValue("@FBMet", txtFBMm.Text.ToString());
                sql.Parameters.AddWithValue("@LREng", txtLRIn.Text.ToString());
                sql.Parameters.AddWithValue("@LRMet", txtLRMm.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                sql.Parameters.AddWithValue("@matType", matID);
                sql.Parameters.AddWithValue("@stations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                sql.Parameters.AddWithValue("@totalCost", totalCost);
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@jobNumber", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@accessNumber", txtAccessNumber.Text);
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@custContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@version", versionNum.ToString("000"));
                sql.Parameters.AddWithValue("@quoteNumber", System.Convert.ToInt32(lblQuoteNumber.Text.Split('-')[0]));
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                

                //We need to insert these into table for EC quotes
                int newECQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote").ToString());
                sql.Parameters.Clear();
                Response.Write("<script>alert('this is the note count to be linked " + insertedNotes.Count + "');</script>");
                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                    sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", newECQuoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }

                String pictureName = "EC-" + newECQuoteID + ".png";
                newPicture(pictureName);

                sql.CommandText = "Update tblECQuote set ecqPicture = @picture where ecqECQuoteID = @ecQuoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ecQuoteID", newECQuoteID);
                sql.Parameters.AddWithValue("@picture", pictureName);
                master.ExecuteNonQuery(sql, "Edit Quote");
                

                //add in quote type
                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newECQuoteID + "&quoteType=" + 1);
            }
            else if(quoteType == 2)
            {
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @custLoc";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@custLoc", ddlPlant.SelectedValue);

                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                int quoteVersion = 0;

                sql.CommandText = "Select max(quoVersion) from linkPartToQuote, tblQuote where ptqQuoteID = quoQuoteID and ";
                sql.CommandText += "ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and quoTSGCompanyID = @company and ptqPartID = @part ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", hdnpartID.Value);
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    quoteVersion = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "INSERT INTO tblQuote (quoTSGCompanyID,quoRFQID,quoEstimatorID,quoVersion, ";
                sql.CommandText += "quoStatusID,quoPaymentTermsID,quoShippingTermsID,quoTotalAmount,quoPartTypeID, ";
                sql.CommandText += "quoToolCountryID,quoCreated,quoCreatedBy,quoModified,quoModifiedBy,quoLeadTime,quoSalesman,quoNumber, ";
                sql.CommandText += "quoCustomerQuoteNumber,quoUseTSGLogo,quoToolingCost,quoTransferBarCost,quoFixtureCost, ";
                sql.CommandText += "quoDieSupportCost,quoShippingCost,quoAdditCostDesc,quoAdditCost,quoUseTSGName,quoPartNumbers, ";
                sql.CommandText += "quoAccess, quoShippingLocation, quoOldQuoteNumber, quoCurrencyID) ";
                sql.CommandText += "Output inserted.quoQuoteID ";
                sql.CommandText += "Select quoTSGCompanyID,quoRFQID,quoEstimatorID, @version,  ";
                sql.CommandText += "quoStatusID,quoPaymentTermsID,quoShippingTermsID,quoTotalAmount,quoPartTypeID, ";
                sql.CommandText += "quoToolCountryID,GETDATE(),quoCreatedBy,quoModified,quoModifiedBy,quoLeadTime,@salesman,quoNumber,  ";
                sql.CommandText += "quoCustomerQuoteNumber,quoUseTSGLogo,quoToolingCost,quoTransferBarCost,quoFixtureCost,  ";
                sql.CommandText += "quoDieSupportCost,quoShippingCost,quoAdditCostDesc,quoAdditCost,quoUseTSGName,quoPartNumbers,  ";
                sql.CommandText += "quoAccess,quoShippingLocation, quoOldQuoteNumber, quoCurrencyID from tblQuote where quoQuoteID = @quoteID ";
                sql.Parameters.Clear();

                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@version", (quoteVersion + 1).ToString("000"));
                sql.Parameters.AddWithValue("@salesman", salesman);

                int newQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote").ToString());
                sql.Parameters.Clear();

                string matID = "";
                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    matID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if (matID == "")
                {
                    sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                }


                sql.CommandText = "insert into pktblBlankInfo (binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialThicknessEnglish, ";
                sql.CommandText += "binMaterialThicknessMetric, binCreated, binCreatedBy, binBlankMaterialTypeID) ";
                sql.CommandText += "output inserted.binBlankInfoID ";
                sql.CommandText += "values(@matWidthEng, @matWidthMet, @matPitchEng, @matPitchMet, @matThickEng, @matThickMet, GETDATE(), @createdBy, @matType )";
                sql.Parameters.Clear();
                if (txtBlankWidthIn.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matWidthEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matWidthEng", txtBlankWidthIn.Text.ToString());
                }
                if (txtBlankWidthMm.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matWidthMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matWidthMet", txtBlankWidthMm.Text.ToString());
                }
                if (txtBlankPitchIn.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matPitchEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matPitchEng", txtBlankPitchIn.Text.ToString());
                }
                if (txtBlankPitchMm.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matPitchMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matPitchMet", txtBlankPitchMm.Text.ToString());
                }
                if (txtMaterialThkIn.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matThickEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThickEng", txtMaterialThkIn.Text.ToString());
                }
                if (txtMaterialThkMm.Text.ToString() == "")
                {
                    sql.Parameters.AddWithValue("@matThickMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThickMet", txtMaterialThkMm.Text.ToString());
                }

                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@matType", matID);
                hdnblankInfoID.Value = master.ExecuteScalar(sql, "EditQuote").ToString();

                sql.Parameters.Clear();

                sql.CommandText = "Update tblQuote set quoBlankInfoID = @blankInfoID where quoQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@blankInfoID", hdnblankInfoID.Value);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");



                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "output inserted.qtrQuoteToRFQID ";
                sql.CommandText += "Values (@quoteID, @rfq, GETDATE(), @createdBy, @hts, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@rfq", lblRfqNumber.Text.ToString());
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                if(master.getCompanyId() != 9)
                {
                    sql.Parameters.AddWithValue("@hts", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@hts", 1);
                }
                master.ExecuteNonQuery(sql, "EditQuote");
                sql.Parameters.Clear();

                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                    sql.CommandText += "output inserted.pwqPWNToQuoteID ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }

                //linking part to quote
                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "output inserted.ptqPartToQuoteID ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 0, 0);";
                sql.Parameters.AddWithValue("@partID", hdnpartID.Value);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "EditQuote");

                sql.Parameters.Clear();

                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and ppdPartID <> @partID";
                sql.Parameters.AddWithValue("@partID", hdnpartID.Value);
                dr = sql.ExecuteReader();
                List<int> partList = new List<int>();
                while (dr.Read())
                {
                    partList.Add(System.Convert.ToInt32(dr.GetValue(0)));
                }
                dr.Close();

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "output inserted.ptqPartToQuoteID ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 0, 0);";
                for (int k = 0; k < partList.Count; k++)
                {
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@partID", partList[k]);
                    master.ExecuteNonQuery(sql, "EditQuote");
                }
                sql.Parameters.Clear();


                //Inserting die info
                sql.CommandText = "insert into tblDieInfo (dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, ";
                sql.CommandText += "dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy) ";
                sql.CommandText += "Output inserted.dinDieInfoID ";
                sql.CommandText += "Values (@dieType, @cavity, @fToBEng, @fToBMet, @lToREng, @lToRMet, @shutHiehgtEng, @shutHeightMet, @numOfStations, GETDATE(), @createdBy )";

                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@fToBEng", txtFBIn.Text.ToString());
                sql.Parameters.AddWithValue("@fToBMet", txtFBMm.Text.ToString());
                sql.Parameters.AddWithValue("@lToREng", txtLRIn.Text.ToString());
                sql.Parameters.AddWithValue("@lToRMet", txtLRMm.Text.ToString());
                sql.Parameters.AddWithValue("@shutHiehgtEng", txtShutIn.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                sql.Parameters.AddWithValue("@numOfStations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                int dieInfoID = 0;
                dieInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote"));
                sql.Parameters.Clear();

                sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                sql.CommandText += "output inserted.diqDieInfoToQuoteID ";
                sql.CommandText += "values (@dieInfo, @quote, GETDATE(), @createdBy)";
                sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                sql.Parameters.AddWithValue("@quote", newQuoteID);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "EditQuote");

                quoteID = newQuoteID;
                lblquoteID.Text = newQuoteID.ToString();

                //add in quote type
                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newQuoteID + "&quoteType=" + 2);


                populate_Header();
            }
            else if (quoteType == 1)
            {
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @custLoc";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@custLoc", ddlPlant.SelectedValue);

                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                string matID = "";
                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    matID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if (matID == "")
                {
                    sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                }

                sql.Parameters.Clear();

                string quoteVersion = (System.Convert.ToInt32(lblVersion.Text) + 1).ToString("000");

                sql.CommandText = "Select max(ecqVersion) from tblECQuote where ecqQuoteNumber = @quoteNum or ecqECQuoteID = @quoteNum ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteNum", lblQuoteNumber.Text);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    quoteVersion = (System.Convert.ToInt32(dr.GetValue(0).ToString()) + 1).ToString("000");
                }
                dr.Close();

                string picture = "";
                sql.CommandText = "Select ecqPicture from tblECQuote where ecqECQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", lblquoteID.Text.Split(' ')[0]);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    picture = dr.GetValue(0).ToString();
                }
                dr.Close();

                sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqVersion, ecqQuoteNumber, ecqShippingLocation, ecqPicture ) ";
                sql.CommandText += "Output inserted.ecqECQuoteID ";
                sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @custContact, @version, @quoteNumber, @shippingLocation, @picture )";

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partName", txtPartName.Text.ToString());
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text.ToString());
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@CustomerLocation", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text.ToString());
                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@blankWidthEng", txtBlankWidthIn.Text.ToString());
                sql.Parameters.AddWithValue("@blankWidthMet", txtBlankWidthMm.Text.ToString());
                sql.Parameters.AddWithValue("@blankPitchEng", txtBlankPitchIn.Text.ToString());
                sql.Parameters.AddWithValue("@blankPitchMet", txtBlankPitchMm.Text.ToString());
                sql.Parameters.AddWithValue("@matThkEng", txtMaterialThkIn.Text.ToString());
                sql.Parameters.AddWithValue("@matThkMet", txtMaterialThkMm.Text.ToString());
                sql.Parameters.AddWithValue("@FBEng", txtFBIn.Text.ToString());
                sql.Parameters.AddWithValue("@FBMet", txtFBMm.Text.ToString());
                sql.Parameters.AddWithValue("@LREng", txtLRIn.Text.ToString());
                sql.Parameters.AddWithValue("@LRMet", txtLRMm.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                sql.Parameters.AddWithValue("@matType", matID);
                sql.Parameters.AddWithValue("@stations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                sql.Parameters.AddWithValue("@totalCost", totalCost);
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@jobNumber", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@accessNumber", txtAccessNumber.Text);
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@custContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@version", quoteVersion);
                sql.Parameters.AddWithValue("@quoteNumber", lblQuoteNumber.Text);
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                sql.Parameters.AddWithValue("@picture", picture);

                //We need to insert these into table for EC quotes
                int newECQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote").ToString());
                sql.Parameters.Clear();
                Response.Write("<script>alert('this is the note count to be linked " + insertedNotes.Count + "');</script>");
                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                    sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", newECQuoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }

                if (picture == "")
                {
                    String pictureName = "EC-" + newECQuoteID + ".png";
                    newPicture(pictureName);

                    sql.CommandText = "Update tblECQuote set ecqPicture = @picture where ecqECQuoteID = @ecQuoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@ecQuoteID", newECQuoteID);
                    sql.Parameters.AddWithValue("@picture", pictureName);
                    master.ExecuteNonQuery(sql, "Edit Quote");
                }
                


                //add in quote type
                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newECQuoteID + "&quoteType=" + 1);
            }

            connection.Close();
        }

        protected void btnSaveClick(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (quoteID == 0 && quoteType == 1)
            {
                //try to execute if you can delete everything and prompt where it failed
                List<int> insertedNotes = new List<int>();
                // loop
                int count = 0;
                double totalCost = 0;
                try
                {
                    sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                    sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                    for (int i = 0; i < 100; i++)
                    {
                        if (Request.Form["notes" + count].ToString() != "" || Request.Form["price" + count].ToString() != "")
                        {
                            if (master.getCompanyId() == 1)
                            {
                                sql.Parameters.AddWithValue("@TSGCompany", ddlTSGCompanyQuoting.SelectedValue);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@TSGCompany", master.getCompanyId());
                            }
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                            sql.Parameters.AddWithValue("@costNote", Request.Form["price" + count].ToString());
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            try
                            {
                                totalCost += System.Convert.ToDouble(Request.Form["price" + count].ToString());
                            }
                            catch
                            {

                            }

                            int noteID = 0;
                            insertedNotes.Add(noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote")));
                            sql.Parameters.Clear();
                        }
                        count++;
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Stoped at " + count + "');</script>");
                }

                sql.Parameters.Clear();
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @salesman";
                sql.Parameters.AddWithValue("@salesman", ddlPlant.SelectedValue);
                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                string matID = "";
                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    matID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if (matID == "")
                {
                    sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                }

                sql.Parameters.Clear();

                sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqVersion, ecqShippingLocation ) ";
                sql.CommandText += "Output inserted.ecqECQuoteID ";
                sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @custContact, @version, @shippingLocation )";

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partName", txtPartName.Text.ToString());
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text.ToString());
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@CustomerLocation", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text.ToString());
                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                if (txtBlankPitchIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankWidthEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankWidthEng", txtBlankWidthIn.Text.ToString());
                }
                if (txtBlankWidthMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankWidthMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankWidthMet", txtBlankWidthMm.Text.ToString());
                }
                if (txtBlankPitchIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankPitchEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankPitchEng", txtBlankPitchIn.Text.ToString());
                }
                if (txtBlankPitchMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankPitchMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankPitchMet", txtBlankPitchMm.Text.ToString());
                }
                if (txtMaterialThkIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@matThkEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThkEng", txtMaterialThkIn.Text.ToString());
                }
                if (txtMaterialThkMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@matThkMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThkMet", txtMaterialThkMm.Text.ToString());
                }
                if (txtFBIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@FBEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@FBEng", txtFBIn.Text.ToString());
                }
                if (txtFBMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@FBMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@FBMet", txtFBMm.Text.ToString());
                }
                if (txtLRIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@LREng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@LREng", txtLRIn.Text.ToString());
                }
                if (txtLRMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@LRMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@LRMet", txtLRMm.Text.ToString());
                }
                if (txtShutIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@shutHeightEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
                }
                if (txtShutMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@shutHeightMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                }
                sql.Parameters.AddWithValue("@matType", matID);
                sql.Parameters.AddWithValue("@stations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@createdby", master.getUserName());
                if (master.getCompanyId() == 1)
                {
                    sql.Parameters.AddWithValue("@companyID", ddlTSGCompanyQuoting.SelectedValue);
                }
                else
                {
                    sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                }
                sql.Parameters.AddWithValue("@totalCost", totalCost);
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@jobNumber", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@accessNumber", txtAccessNumber.Text);
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@custContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@version", "001");
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);

                //We need to insert these into table for EC quotes
                int newECQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote").ToString());
                sql.Parameters.Clear();
                Response.Write("<script>alert('this is the note count to be linked " + insertedNotes.Count + "');</script>");
                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                    sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", newECQuoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }

                String pictureName = "EC-" + newECQuoteID + ".png";
                newPicture(pictureName);

                sql.CommandText = "Update tblECQuote set ecqPicture = @picture, ecqQuoteNumber = @quoteNumber where ecqECQuoteID = @ecQuoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ecQuoteID", newECQuoteID);
                sql.Parameters.AddWithValue("@picture", pictureName);
                sql.Parameters.AddWithValue("@quoteNumber", newECQuoteID);
                master.ExecuteNonQuery(sql, "Edit Quote");
                

                //add in quote type
                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newECQuoteID + "&quoteType=" + 1);

                connection.Close();
            }
            else if (quoteType == 1)
            {
                string[] quoteNumbersToken = lblquoteID.Text.ToString().Split(' ');

                double totalCost = 0;

                string[] notes = hdnNoteOrder.Value.Split(',');
                List<string> notesToDelete = new List<string>();

                int i = 0;

                sql.Parameters.Clear();
                sql.CommandText = "delete from linkPWNToECQuote where peqECQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteNumbersToken[0]);
                master.ExecuteNonQuery(sql, "EditQuote");


                for (int k = 0; k < 200; k++)
                {
                    try {
                        if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                        {
                            sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                            sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                            sql.CommandText += "Values(@company, @note, @costNote, GETDATE(), @createdBy )";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + i]);
                            sql.Parameters.AddWithValue("costNote", Request.Form["price" + i].ToString());
                            if (master.getCompanyId() == 1)
                            {
                                sql.Parameters.AddWithValue("@company", ddlTSGCompanyQuoting.SelectedValue);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                            }
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            string temp = master.ExecuteScalar(sql, "EditQuote").ToString();
                            try
                            {
                                totalCost += System.Convert.ToDouble(Request.Form["price" + i].ToString());
                            }
                            catch
                            {

                            }
                            sql.Parameters.Clear();
                            sql.CommandText = "insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                            sql.CommandText += "values(@quoteID, @noteID, GETDATE(), @createdBy)";
                            sql.Parameters.AddWithValue("@quoteID", quoteNumbersToken[0]);
                            sql.Parameters.AddWithValue("@noteID", temp);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            master.ExecuteNonQuery(sql, "EditQuote");
                        }
                    }
                    catch
                    {
                        //break;
                    }
                    i++;
                }

                sql.Parameters.Clear();

                String pictureName = "EC-" + quoteNumbersToken[0] + ".png";

                Boolean picture = newPicture(pictureName);

                string matID = "";
                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);

                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    matID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if (matID == "")
                {
                    sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                }

                sql.Parameters.Clear();
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @salesman";
                sql.Parameters.AddWithValue("@salesman", ddlPlant.SelectedValue);
                int salesman = 0;
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "update tblECQuote set ";
                sql.CommandText += "ecqPartNumber = @partNum, ecqPartName = @partName, ecqRFQNumber = @rfqNum, ecqCustomer = @customer, ecqCustomerLocation = @customerLocation, ";
                sql.CommandText += "ecqCustomerRFQNumber = @customerRFQ, ecqDieType = @dieType, ecqCavity = @cavity, ecqBlankWidthEng = @blankWidthEng, ecqBlankWidthMet = @blankWidthMet, ";
                sql.CommandText += "ecqBlankPitchEng = @blankPitchEng, ecqBlankPitchMet = @blankPitchMet, ecqMaterialThkEng = @matThkEng, ecqMaterialThkMet = @matThkMet, ecqDieFBEng = @FBEng, ";
                sql.CommandText += "ecqDieFBMet = @FBMet, ecqDieLREng = @LREng, ecqDieLRMet = @LRMet, ecqShutHeightEng = @shutHeightEng, ecqShutHeightMet = @shutHeightMet, ";
                sql.CommandText += "ecqMaterialType = @matType, ecqNumberOfStations = @stations, ecqLeadTime = @leadTime, ecqShipping = @shipping, ecqPayment = @payment, ";
                sql.CommandText += "ecqCountryOfOrign = @country, ecqModified = GETDATE(), ecqModifiedBy = @modifiedBy, ecqTSGCompanyID = @companyID, ecqTotalCost = @totalCost, ecqStatus = @status, ecqEstimator = @estimator, ";
                sql.CommandText += "ecqJobNumber = @jobNumber, ecqAccessNumber = @accessNumber, ecqUseTSG = @useTSG, ecqCustomerContactName = @custContact, ecqShippingLocation = @shippingLocation, ecqSalesmanID = @salesman ";
                if (picture)
                {
                    sql.CommandText += ", ecqPicture = @picture ";
                    sql.Parameters.AddWithValue("@picture", pictureName);
                }
                sql.CommandText += "where ecqECQuoteID = @quote";

                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partName", txtPartName.Text.ToString());
                sql.Parameters.AddWithValue("@rfqNum", txtRFQNumber.Text.ToString());
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@CustomerLocation", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@customerRFQ", txtCustomerRFQ.Text.ToString());
                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                if (txtBlankPitchIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankWidthEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankWidthEng", txtBlankWidthIn.Text.ToString());
                }
                if (txtBlankWidthMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankWidthMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankWidthMet", txtBlankWidthMm.Text.ToString());
                }
                if (txtBlankPitchIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankPitchEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankPitchEng", txtBlankPitchIn.Text.ToString());
                }
                if (txtBlankPitchMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@blankPitchMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@blankPitchMet", txtBlankPitchMm.Text.ToString());
                }
                if (txtMaterialThkIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@matThkEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThkEng", txtMaterialThkIn.Text.ToString());
                }
                if (txtMaterialThkMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@matThkMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@matThkMet", txtMaterialThkMm.Text.ToString());
                }
                if (txtFBIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@FBEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@FBEng", txtFBIn.Text.ToString());
                }
                if (txtFBMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@FBMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@FBMet", txtFBMm.Text.ToString());
                }
                if (txtLRIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@LREng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@LREng", txtLRIn.Text.ToString());
                }
                if (txtLRMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@LRMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@LRMet", txtLRMm.Text.ToString());
                }
                if (txtShutIn.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@shutHeightEng", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
                }
                if (txtShutMm.Text.ToString().Trim() == "")
                {
                    sql.Parameters.AddWithValue("@shutHeightMet", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                }
                sql.Parameters.AddWithValue("@matType", matID);
                sql.Parameters.AddWithValue("@stations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@country", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                if (master.getCompanyId() == 1)
                {
                    sql.Parameters.AddWithValue("@companyID", ddlTSGCompanyQuoting.SelectedValue);
                }
                else
                {
                    sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                }
                sql.Parameters.AddWithValue("@totalCost", totalCost);
                sql.Parameters.AddWithValue("@quote", quoteNumbersToken[0]);
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@jobNumber", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@accessNumber", txtAccessNumber.Text);
                sql.Parameters.AddWithValue("@useTSG", cbUseTSG.Checked);
                sql.Parameters.AddWithValue("@custContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);

                master.ExecuteNonQuery(sql, "EditQuote");

                populate_Header();
            }
            else if (quoteID == 0)
            {
                //try to execute if you can delete everything and prompt where it failed
                List<int> insertedNotes = new List<int>();
                // loop
                int totalCost = 0;
                try
                {
                    sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                    sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                    int count = 0;
                    for (int k = 0; k < 200; k++)
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

                            totalCost += System.Convert.ToInt32(Request.Form["price" + count].ToString());
                        }
                        count++;
                    }
                }
                catch
                {

                }

                sql.Parameters.Clear();
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @salesman";
                sql.Parameters.AddWithValue("@salesman", ddlPlant.SelectedValue);
                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.Parameters.Clear();

                sql.CommandText = "insert into tblQuote (quoTSGCompanyID, quoRFQID, quoEstimatorID, quoJobNumberID, quoPaymentTermsID, quoShippingTermsID, ";
                sql.CommandText += "quoTotalAmount, quoProductTypeID, quoOEMID, quoPartTypeID, quoToolCountryID, quoLeadTime, quoCreated, quoCreatedBy, quoQuoteTypeID, quoStatusID, quoSalesman, quoCustomerQuoteNumber, quoPartNumbers, quoShippingLocation, quoCurrencyID ) ";
                sql.CommandText += "Output inserted.quoQuoteID ";
                sql.CommandText += "Values ( @company, @rfq, @estimator, @jobNumber, @paymentTerms, @shippingTerms, @totalAmount, @productType, @oem, @partType, @toolCountry, @leadTime, GETDATE(), @createdBy, @quoteType, @status, @salesman, @custQuoteNum, ";
                sql.CommandText += "@partNums, @shippingLocation, @currencyId )";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                sql.Parameters.AddWithValue("@rfq", lblRfqNumber.Text.ToString());
                sql.Parameters.AddWithValue("@estimator", 9);
                sql.Parameters.AddWithValue("@jobNumber", "");
                sql.Parameters.AddWithValue("@paymentTerms", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@shippingTerms", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@totalAmount", totalCost);
                if (hdnproductTypeID.Value == "0")
                {
                    sql.Parameters.AddWithValue("@productType", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@productType", hdnproductTypeID.Value);
                }
                if (hdnoemID.Value == "0")
                {
                    sql.Parameters.AddWithValue("@oem", 39);
                }
                else
                {
                    sql.Parameters.AddWithValue("@oem", hdnoemID.Value);
                }
                if (hdnpartTypeID.Value == "0")
                {
                    sql.Parameters.AddWithValue("@partType", 33);
                }
                else
                {
                    sql.Parameters.AddWithValue("@partType", hdnpartTypeID.Value);
                }
                sql.Parameters.AddWithValue("@toolCountry", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@quoteType", quoteType);
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@version", "001");
                sql.Parameters.AddWithValue("@custQuoteNum", txtCustQuoteNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partNums", txtWBPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                sql.Parameters.AddWithValue("@currencyId", ddlCurrency.SelectedValue);

                int newQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote").ToString());
                sql.Parameters.Clear();

                if (quoteNumber != 0 && version != 0)
                {
                    sql.CommandText = "Update tblQuote set quoNumber = @quoteNumber, quoVersion = @version where quoQuoteID = @quoteID";
                    sql.Parameters.AddWithValue("@quoteNumber", quoteNumber);
                    sql.Parameters.AddWithValue("@version", "00" + version);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    master.ExecuteNonQuery(sql, "EditQuote");
                }
                else
                {
                    sql.CommandText = "Update tblQuote set quoNumber = @quoteID, quoVersion = @version where quoQuoteID = @quoteID";
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    master.ExecuteNonQuery(sql, "EditQuote");
                }

                sql.Parameters.Clear();

                //sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy) ";
                //sql.CommandText += "output inserted.qtrQuoteToRFQID ";
                //sql.CommandText += "Values (@quoteID, @rfq, GETDATE(), @createdBy)";
                //sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                //sql.Parameters.AddWithValue("@rfq", lblRfqNumber.Text.ToString());
                //sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                //master.ExecuteNonQuery(sql, "EditQuote");
                //sql.Parameters.Clear();

                for (int k = 0; k < insertedNotes.Count; k++)
                {
                    sql.CommandText = "Insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                    sql.CommandText += "output inserted.pwqPWNToQuoteID ";
                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@noteID", insertedNotes[k]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "EditQuote");

                    sql.Parameters.Clear();
                }

                //linking part to quote
                //sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqWBPartNumber ptqCreated, ptqCreatedBy) ";
                //sql.CommandText += "output inserted.ptqPartToQuoteID ";
                //sql.CommandText += "values (@partID, @quoteID, @wbPartNum, GETDATE(), @createdBy);";
                //sql.Parameters.AddWithValue("@partID", hdnpartID.Value);
                //sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                //sql.Parameters.AddWithValue("@wbPartNum", txtWBPartNumber.Text.ToString());
                //sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                //master.ExecuteNonQuery(sql, "EditQuote");

                sql.Parameters.Clear();

                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and ppdPartID <> @partID";
                sql.Parameters.AddWithValue("@partID", hdnpartID.Value);
                dr = sql.ExecuteReader();
                List<int> partList = new List<int>();
                while (dr.Read())
                {
                    partList.Add(System.Convert.ToInt32(dr.GetValue(0)));
                }
                dr.Close();

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "output inserted.ptqPartToQuoteID ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 0, 0);";
                for (int k = 0; k < partList.Count; k++)
                {
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@partID", partList[k]);
                    master.ExecuteNonQuery(sql, "EditQuote");
                }
                sql.Parameters.Clear();


                //Inserting die info
                sql.CommandText = "insert into tblDieInfo (dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, ";
                sql.CommandText += "dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy) ";
                sql.CommandText += "Output inserted.dinDieInfoID ";
                sql.CommandText += "Values (@dieType, @cavity, @fToBEng, @fToBMet, @lToREng, @lToRMet, @shutHiehgtEng, @shutHeightMet, @numOfStations, GETDATE(), @createdBy )";

                sql.Parameters.AddWithValue("@dieType", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@fToBEng", txtFBIn.Text.ToString());
                sql.Parameters.AddWithValue("@fToBMet", txtFBMm.Text.ToString());
                sql.Parameters.AddWithValue("@lToREng", txtLRIn.Text.ToString());
                sql.Parameters.AddWithValue("@lToRMet", txtLRMm.Text.ToString());
                sql.Parameters.AddWithValue("@shutHiehgtEng", txtShutIn.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                sql.Parameters.AddWithValue("@numOfStations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                int dieInfoID = 0;
                dieInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditQuote"));
                sql.Parameters.Clear();

                sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                sql.CommandText += "output inserted.diqDieInfoToQuoteID ";
                sql.CommandText += "values (@dieInfo, @quote, GETDATE(), @createdBy)";
                sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                sql.Parameters.AddWithValue("@quote", newQuoteID);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "EditQuote");

                quoteID = newQuoteID;
                lblquoteID.Text = newQuoteID.ToString();

                sql.Parameters.Clear();
                sql.CommandText = "Update tblRFQ set rfqCheckBit = 1, rfqModified = GETDATE(), rfqModifiedBy = @modified where rfqID = @rfq";
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@modified", master.getUserID());

                master.ExecuteNonQuery(sql, "EditQuote");

                //add in quote type
                Response.Redirect("https://tsgrfq.azurewebsites.net/EditQuote?id=" + newQuoteID + "&quoteType=" + 2);

                sql.CommandText = "Select prtRFQLineNumber from tblPart where prtPARTID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader dr2 = sql.ExecuteReader();
                string lineNumber = "";
                while (dr2.Read())
                {
                    lineNumber = dr2.GetValue(0).ToString();
                }
                dr2.Close();

                String pictureName = "RFQ" + rfqID + "_" + lineNumber + "_" + txtPartNumber.Text.Trim() + ".png";

                newPicture(pictureName);

                populate_Header();
            }
            else
            {
                string partID = hdnpartID.Value;

                sql.Parameters.Clear();
                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @salesman";
                sql.Parameters.AddWithValue("@salesman", ddlPlant.SelectedValue);
                int salesman = 0;
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    salesman = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Update tblDieInfo set ";
                sql.CommandText += "dinDieType = @type, dinCavityID = @cavity, dinSizeFrontToBackEnglish = @fbEng, dinSizeFrontToBackMetric = @fbMet, dinSizeLeftToRightEnglish = @lrEng, ";
                sql.CommandText += "dinSizeLeftToRightMetric = @lrMet, dinSizeShutHeightEnglish = @shutHeightEng, dinSizeShutHeightMetric = @shutHeightMet, dinNumberOfStations = @numStations, ";
                sql.CommandText += "dinModified = GETDATE(), dinModifiedBy = @modifiedBy ";
                sql.CommandText += "where dinDieInfoID = @dieInfoID";

                sql.Parameters.AddWithValue("@type", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@fbEng", txtFBIn.Text.ToString());
                sql.Parameters.AddWithValue("@fbMet", txtFBMm.Text.ToString());
                sql.Parameters.AddWithValue("@lrEng", txtLRIn.Text.ToString());
                sql.Parameters.AddWithValue("@lrMet", txtLRMm.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightEng", txtShutIn.Text.ToString());
                sql.Parameters.AddWithValue("@shutHeightMet", txtShutMm.Text.ToString());
                sql.Parameters.AddWithValue("@numStations", txtStations.Text.ToString());
                sql.Parameters.AddWithValue("@dieInfoID", hdndieInfoID.Value);
                sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                master.ExecuteNonQuery(sql, "EditQuote");

                if (hdnblankInfoID.Value != "")
                {
                    string matID = "";
                    sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        matID = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                    if (matID == "")
                    {
                        sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                        sql.CommandText += "output inserted.mtyMaterialTypeID ";
                        sql.CommandText += "values (@matType) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                        matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                    }

                    sql.CommandText = "Update pktblBlankInfo set ";
                    sql.CommandText += "binMaterialWidthEnglish = @matWidthEng, binMaterialWidthMetric = @matWidthMet, binMaterialPitchEnglish = @matPitchEng, binMaterialPitchMetric = @matPitchMet, ";
                    sql.CommandText += "binMaterialThicknessEnglish = @matThickEng, binMaterialThicknessMetric = @matThickMet, binModified = GETDATE(), binModifiedBy = @modifiedBy, binBlankMaterialTypeID = @matType ";
                    sql.CommandText += "where binBlankInfoID = @blankInfoID";

                    sql.Parameters.Clear();

                    if (txtBlankWidthIn.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matWidthEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matWidthEng", txtBlankWidthIn.Text.ToString());
                    }
                    if (txtBlankWidthMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matWidthMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matWidthMet", txtBlankWidthMm.Text.ToString());
                    }
                    if (txtBlankPitchIn.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matPitchEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matPitchEng", txtBlankPitchIn.Text.ToString());
                    }
                    if (txtBlankPitchMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matPitchMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matPitchMet", txtBlankPitchMm.Text.ToString());
                    }
                    if (txtMaterialThkIn.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matThickEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matThickEng", txtMaterialThkIn.Text.ToString());
                    }
                    if (txtMaterialThkMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matThickMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matThickMet", txtMaterialThkMm.Text.ToString());
                    }

                    sql.Parameters.AddWithValue("@matType", matID);
                    sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                    sql.Parameters.AddWithValue("@blankInfoID", hdnblankInfoID.Value);

                    try
                    {
                        master.ExecuteNonQuery(sql, "EditQuote");
                    }
                    catch
                    {
                    }
                }
                else
                {
                    string matID = "";
                    sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        matID = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                    if (matID == "")
                    {
                        sql.CommandText = "insert into pktblMaterialType (mtyMaterialType) ";
                        sql.CommandText += "output inserted.mtyMaterialTypeID ";
                        sql.CommandText += "values (@matType) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                        matID = master.ExecuteScalar(sql, "Edit Quote").ToString();
                    }

                    sql.CommandText = "insert into pktblBlankInfo (binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialThicknessEnglish, ";
                    sql.CommandText += "binMaterialThicknessMetric, binCreated, binCreatedBy, binBlankMaterialTypeID) ";
                    sql.CommandText += "output inserted.binBlankInfoID ";
                    sql.CommandText += "values(@matWidthEng, @matWidthMet, @matPitchEng, @matPitchMet, @matThickEng, @matThickMet, GETDATE(), @createdBy, @matType )";
                    sql.Parameters.Clear();
                    if (txtBlankWidthIn.Text.ToString() == "")
                    {
                        sql.Parameters.AddWithValue("@matWidthEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matWidthEng", txtBlankWidthIn.Text.ToString());
                    }
                    if (txtBlankWidthMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matWidthMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matWidthMet", txtBlankWidthMm.Text.ToString());
                    }
                    if (txtBlankPitchIn.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matPitchEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matPitchEng", txtBlankPitchIn.Text.ToString());
                    }
                    if (txtBlankPitchMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matPitchMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matPitchMet", txtBlankPitchMm.Text.ToString());
                    }
                    if (txtMaterialThkIn.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matThickEng", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matThickEng", txtMaterialThkIn.Text.ToString());
                    }
                    if (txtMaterialThkMm.Text.ToString().Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@matThickMet", 0);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@matThickMet", txtMaterialThkMm.Text.ToString());
                    }

                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@matType", matID);
                    hdnblankInfoID.Value = master.ExecuteScalar(sql, "EditQuote").ToString();

                    sql.Parameters.Clear();

                    sql.CommandText = "Select prtPARTID from linkPartToQuote, tblPart where ptqQuoteID = @quoteID and ptqPartID = prtPartID order by prtRFQLineNumber";
                    sql.Parameters.AddWithValue("@quoteID", quoteID);

                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        partID = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                    if (partID != "")
                    {
                        sql.CommandText = "Update tblPart set ";
                        sql.CommandText += "prtPartMaterialType = @matType, prtBlankInfoID = @blankInfoID, prtModified = GETDATE(), prtModifiedBy = @modified where prtPARTID = @partID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", matID);
                        sql.Parameters.AddWithValue("@blankInfoID", hdnblankInfoID.Value);
                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@modified", master.getUserID());

                        master.ExecuteNonQuery(sql, "EditQuote");
                    }

                    sql.CommandText = "Update tblQuote set quoBlankInfoID = @blankInfo where quoQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@blankInfo", hdnblankInfoID.Value);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    master.ExecuteNonQuery(sql, "Edit Quote");
                }

                string[] notes = hdnNoteOrder.Value.Split(',');
                List<string> notesToDelete = new List<string>();

                sql.CommandText = "Update pktblPreWordedNote set ";
                sql.CommandText += "pwnPreWordedNote = @note, pwnCostNote = @costNote, pwnModified = GETDATE(), pwnModifiedBy = @modifiedBy ";
                sql.CommandText += "where pwnPreWordedNoteID = @noteID";
                int i = 0;
                double totalCost = 0;
                while (i < notes.Length)
                {
                    sql.Parameters.Clear();

                    try
                    {
                        if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                        {
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + i].ToString());
                            sql.Parameters.AddWithValue("@costNote", Request.Form["price" + i].ToString());
                            sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                            sql.Parameters.AddWithValue("@noteID", notes[i]);
                            master.ExecuteNonQuery(sql, "EditQuote");
                            try
                            {
                                totalCost += System.Convert.ToDouble(Request.Form["price" + i].ToString());
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            if (notes[i] != null)
                            {
                                notesToDelete.Add(notes[i]);
                            }
                        }
                    }
                    catch
                    {
                        notesToDelete.Add(notes[i]);
                    }
                    i++;
                }
                for (int j = 0; j < notesToDelete.Count; j++)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "delete from linkPWNToQuote where pwqPreWordedNoteID = @pwn";
                    sql.Parameters.AddWithValue("@pwn", notesToDelete[j].ToString());
                    master.ExecuteNonQuery(sql, "EditQuote");
                    sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @pwn";
                    master.ExecuteNonQuery(sql, "EditQuote");
                }
                //Now that we took care of either updating or deleteing all the ntoes that already existed wee want to go through the rest of the notes until
                //we dont have anymore and create an entry in the table and link it to the quote
                for (int k = 0; k < 200; k++)
                {
                    try
                    {
                        if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                        {
                            sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                            sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                            sql.CommandText += "Values(@company, @note, @costNote, GETDATE(), @createdBy )";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + i]);
                            sql.Parameters.AddWithValue("costNote", Request.Form["price" + i].ToString());
                            sql.Parameters.AddWithValue("@company", master.getCompanyId());
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            string temp = master.ExecuteScalar(sql, "EditQuote").ToString();
                            try
                            {
                                totalCost += System.Convert.ToDouble(Request.Form["price" + i].ToString());
                            }
                            catch
                            {

                            }

                            sql.Parameters.Clear();
                            sql.CommandText = "insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                            sql.CommandText += "values(@quoteID, @noteID, GETDATE(), @createdBy)";
                            sql.Parameters.AddWithValue("@quoteID", lblquoteID.Text.ToString());
                            sql.Parameters.AddWithValue("@noteID", temp);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            master.ExecuteNonQuery(sql, "EditQuote");
                        }
                    }
                    catch
                    {
                        break;
                    }
                    i++;
                }

                sql.Parameters.Clear();

                sql.CommandText = "Update tblQuote set ";
                sql.CommandText += "quoLeadTime = @leadTime, quoShippingTermsID = @shipping, quoPaymentTermsID = @payment, quoToolCountryID = @toolCountry, quoModified = GETDATE(), ";
                sql.CommandText += "quoModifiedBy = @modifiedBy, quoTotalAmount = @total, quoStatusID = @status, quoCustomerQuoteNumber = @custQuoteNum, ";
                sql.CommandText += "quoPartNumbers = @partNums, quoPlant = @plant, quoUseTSGLogo = @logo, quoUseTSGName = @name, quoPartName = @partName, ";
                sql.CommandText += "quoShippingLocation = @shippingLocation, quoCustomerContact = @customerContact, quoAccess = @access, quoJobNum = @jobNum, ";
                sql.CommandText += "quoCurrencyID = @currencyId, quoEstimatorID = @estimator, quoSalesman = @salesman where quoQuoteID = @quoteID";

                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text.ToString());
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@toolCountry", ddlCountry.SelectedValue);
                sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                sql.Parameters.AddWithValue("@total", totalCost);
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@custQuoteNum", txtCustQuoteNumber.Text.ToString());
                sql.Parameters.AddWithValue("@partNums", txtWBPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@access", txtAccessNumber.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@currencyId", ddlCurrency.SelectedValue);

                if (cbUseTSG.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                    sql.Parameters.AddWithValue("@name", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                    sql.Parameters.AddWithValue("@name", 0);
                }
                master.ExecuteNonQuery(sql, "EditQuote");
                sql.Parameters.Clear();

                //sql.CommandText = "Delete from linkPartReservedToCompany where prcPartID = @partID and prcTSGCompanyID = @tsg";
                //sql.Parameters.AddWithValue("@partID", partID);
                //sql.Parameters.AddWithValue("@tsg", master.getCompanyId());

                //try
                //{
                //    master.ExecuteNonQuery(sql, "EditQuote");
                //}
                //catch
                //{

                //}

                sql.Parameters.Clear();
                sql.CommandText = "update linkPartToQuote set ";
                sql.CommandText += "ptqWBPartNumber = @wbPartNum, ptqModified = GETDATE(), ptqModifiedBy = @modified where ptqQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@wbPartNum", txtWBPartNumber.Text.ToString());
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@modified", master.getUserID());
                try
                {
                    master.ExecuteNonQuery(sql, "EditQuote");
                }
                catch
                {

                }
                sql.CommandText = "Select quoRFQID from tblQuote where quoQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                SqlDataReader dr3 = sql.ExecuteReader();

                if (dr3.Read())
                {
                    rfqID = System.Convert.ToInt64(dr3.GetValue(0).ToString());
                }
                dr3.Close();


                sql.Parameters.Clear();
                sql.CommandText = "Update tblRFQ set rfqCheckBit = 1, rfqModified = GETDATE(), rfqModifiedBy = @modified where rfqID = @rfq";
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@modified", master.getUserID());

                master.ExecuteNonQuery(sql, "EditQuote");

                sql.CommandText = "Select prtRFQLineNumber, prtPartNumber from tblPart where prtPARTID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader dr2 = sql.ExecuteReader();
                string pictureName = "";
                while (dr2.Read())
                {
                    pictureName = "RFQ" + rfqID + "_" + dr2.GetValue(0).ToString() + "_" + dr2.GetValue(1).ToString() + ".png";
                }
                dr2.Close();

                if (newPicture(pictureName))
                {
                    sql.CommandText = "update tblPart set prtPicture = @pic where prtPARTID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@pic", pictureName);
                    sql.Parameters.AddWithValue("@partID", partID);
                    master.ExecuteNonQuery(sql, "Edit Quote");
                }


                

                populate_Header();
            }
            connection.Close();
        }

        private Boolean newPicture(string pictureName)
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
                Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("Part Pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = newStream;
                newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part Pictures/" + pictureName;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                partPicturesList.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                // set the Attributes
                Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                newItem.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                return true;
            }
            return false;
        }
    }
}