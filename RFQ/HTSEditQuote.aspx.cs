using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class HTSEditQuote : System.Web.UI.Page
    {
        public Int64 quoteID = 0;
        public string partID = "";
        public Int64 rfqID = 0;
        public Boolean IsMasterCompany = false;
        public long UserCompanyID = 0;
        public int quoteType = 0;
        public int quoteNumber = 0;
        public int version = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtRFQNumber.Visible = false;
            try
            {
                rfqID = System.Convert.ToInt64(Request["rfq"]);
            }
            catch
            {

            }
            try
            {
                partID = Request["partID"];
            }
            catch
            {

            }
            try
            {
                quoteID = System.Convert.ToInt32(Request["id"]);
            }
            catch
            {

            }

            if (rfqID != 0)
            {
                //btnCopyQuote_Click.Visible = false;
                btnCopyQuote_Click.Text = "Copy Quote to Standalone";
            }
            else
            {
                //btnCopyQuote_Click.Visible = true;
            }

            if (!IsPostBack)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                try
                {
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
                        sql.CommandText = "Select CONCAT (dtyFullName, ', ', TSGCompanyAbbrev) as name, DieTypeID from DieType, TSGCompany where DieType.TSGCompanyID = TSGCompany.TSGCompanyID Order by DieTypeID";
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

                    sql.CommandText = "Select qtyQuoteType, qtyQuoteTypeID from pktblQuoteType";
                    sql.Parameters.Clear();
                    stDR = sql.ExecuteReader();
                    ddlQuoteType.DataSource = stDR;
                    ddlQuoteType.DataTextField = "qtyQuoteType";
                    ddlQuoteType.DataValueField = "qtyQuoteTypeID";
                    ddlQuoteType.DataBind();
                    stDR.Close();

                    sql.CommandText = "Select ptyPartTypeDescription, ptyPartTypeID from pktblPartType";
                    sql.Parameters.Clear();
                    stDR = sql.ExecuteReader();
                    ddlPartType.DataSource = stDR;
                    ddlPartType.DataTextField = "ptyPartTypeDescription";
                    ddlPartType.DataValueField = "ptyPartTypeID";
                    ddlPartType.DataBind();
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

                    sql.CommandText = "Select CONCAT(estFirstName, ' ', estLastName) as 'name', estEstimatorID from pktblEstimators where estCompanyID = 9";
                    SqlDataReader estimatorDR = sql.ExecuteReader();
                    ddlEstimator.DataSource = estimatorDR;
                    ddlEstimator.DataTextField = "name";
                    ddlEstimator.DataValueField = "estEstimatorID";
                    ddlEstimator.DataBind();
                    estimatorDR.Close();

                    sql.CommandText = "Select curCurrencyID, curCurrency from pktblCurrency order by curCurrency";
                    SqlDataReader dr = sql.ExecuteReader();
                    ddlCurrency.DataSource = dr;
                    ddlCurrency.DataTextField = "curCurrency";
                    ddlCurrency.DataValueField = "curCurrencyID";
                    ddlCurrency.DataBind();
                    dr.Close();
                    ddlCurrency.SelectedValue = "1";
                }
                catch
                {

                }
                connection.Close();
                populate_Header();

            }
        }

        protected void populate_Header()
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
            generalNote.Add(lblGeneralNote10);
            generalNote.Add(lblGeneralNote11);

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
            cb.Add(cbGeneralNote10);
            cb.Add(cbGeneralNote11);

            sql.CommandText = "Select concat(gnoGeneralNoteID, '-', gnoGeneralNote) from pktblGeneralNote where gnoCompany = 'HTS'";
            SqlDataReader gnodr = sql.ExecuteReader();
            int j = 0;
            while(gnodr.Read())
            {
                generalNote[j].Text = gnodr.GetValue(0).ToString();
                j++;
            }
            gnodr.Close();


            if (rfqID != 0)
            {
                string cust = "", plant = "";
                sql.CommandText = "Select prtPartNumber, prtpartDescription, rfqID, rfqCustomerRFQNumber, rfqCustomerID, rfqPlantID, CustomerCOntact.Name from tblRFQ, linkPartToRFQ, tblPart, CustomerContact where rfqID = @rfqID and prtPARTID = @partID and ptrRFQID = rfqID and ptrPartID = prtPARTID and rfqCustomerContact = CustomerContactID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@partID", partID);
                //txtPartNumber.Visible = false;
                //txtPartName.Visible = false;
                txtRFQNumber.Visible = false;
                //txtCustomerRFQ.Visible = false;
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    txtPartNumber.Text = dr.GetValue(0).ToString();
                    txtPartName.Text = dr.GetValue(1).ToString();
                    lblRfqNumber.Text = dr.GetValue(2).ToString();
                    //lblCustomerRFQ.Text = dr.GetValue(3).ToString();
                    ddlCustomer.SelectedValue = dr.GetValue(4).ToString();
                    cust = dr.GetValue(4).ToString();
                    if(ddlCustomer.SelectedValue == cust)
                    {
                        populate_Plants();
                    }
                    ddlPlant.SelectedValue = dr.GetValue(5).ToString();
                    plant = dr.GetValue(5).ToString();
                    txtCustomerContact.Text = dr.GetValue(6).ToString();
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

                sql.CommandText = "Select distinct prtPartNumber from linkPartToPartDetail, tblPart where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and prtPARTID = ppdPartID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                int count = 0;
                while(dr.Read())
                {
                    if(count == 0)
                    {
                        txtPartNumber.Text = dr.GetValue(0).ToString();
                    }
                    else
                    {
                        txtPartNumber.Text += " - " + dr.GetValue(0).ToString();
                    }
                    count++;
                }
                dr.Close();
            }

            if(quoteID != 0)
            {
                hdnQuoteNumber.Value = quoteID.ToString();

                sql.CommandText = "Select hquVersion, hquStatusID, hquPartNumbers, hquPartName, hquRFQID, hquUseTSGLogo, hquUseTSGName, hquCustomerID, hquCustomerLocationID, hquSalesman, hquCustomerQuoteNumber, ";
                sql.CommandText += "hquEstimatorID, hquQuoteTypeID, hquPartTypeID, hquProcess, hquCavity, hquLeadTime, hquShippingTerms, hquPaymentTerms, hquAccess, hquCustomerContactName, hquPartName, ";
                sql.CommandText += " hquCustomerQuoteNumber, hquCustomerRFQNum, hquJobNumberID, hquMaterialType, hquNumber, hquFinalized ";
                sql.CommandText += "from tblHTSQuote where hquHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                string cust = "", plant = "";
                if(dr.Read())
                {
                    lblquoteID.Text = quoteID.ToString();
                    lblQuoteNumber.Text = dr.GetValue(26).ToString();
                    lblVersion.Text = dr.GetValue(0).ToString();
                    ddlStatus.SelectedValue = dr.GetValue(1).ToString();
                    txtPartNumber.Text = dr.GetValue(2).ToString();
                    if(rfqID != 0)
                    {
                        txtPartName.Text = dr.GetValue(3).ToString();
                        //txtCustomerRFQ.Text = dr.GetValue(10).ToString();
                    }
                    //txtCustomerRFQ.Text = dr.GetValue(4).ToString();
                    cbUseTSGLogo.Checked = dr.GetBoolean(5);
                    cbUseTSGName.Checked = dr.GetBoolean(6);
                    ddlCustomer.SelectedValue = dr.GetValue(7).ToString();
                    cust = dr.GetValue(7).ToString();
                    populate_Plants();
                    ddlPlant.SelectedValue = dr.GetValue(8).ToString();
                    plant = dr.GetValue(8).ToString();
                    ddlEstimator.SelectedValue = dr.GetValue(11).ToString();
                    ddlQuoteType.SelectedValue = dr.GetValue(12).ToString();
                    ddlPartType.SelectedValue = dr.GetValue(13).ToString();
                    ddlProcess.SelectedValue = dr.GetValue(14).ToString();
                    ddlCavity.SelectedValue = dr.GetValue(15).ToString();
                    txtLeadTime.Text = dr.GetValue(16).ToString();
                    ddlShipping.SelectedValue = dr.GetValue(17).ToString();
                    ddlPayment.SelectedValue = dr.GetValue(18).ToString();
                    txtAccess.Text = dr.GetValue(19).ToString();
                    txtCustomerContact.Text = dr.GetValue(20).ToString();
                    txtPartName.Text = dr.GetValue(21).ToString();
                    //txtCustomerRFQ.Text = dr.GetValue(22).ToString();
                    txtCustomerRFQ.Text = dr.GetValue(23).ToString();
                    txtJobNum.Text = dr.GetValue(24).ToString();
                    txtMaterialType.Text = dr.GetValue(25).ToString();
                    if (dr["hquFinalized"].ToString() == "True")
                    {
                        //litScript.Text = "<script>$('#btnCheck').hide();</script>";
                        btnSave_Click.Visible = false;
                        //btnFinalize.Visible = false;
                        litStatus.Text = "<h3><font color='red'>This quote has been finalized and is not editable.</font></h3>";
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

                sql.CommandText = "Select hpwNote, hpwQuantity, hpwUnitPrice from pktblHTSPreWordedNote, linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID and pthHTSPWNID = hpwHTSPreWordedNoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                int i = 0;
                double total = 0;
                while(dr.Read())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr.GetValue(0).ToString().Replace("\'", "")) + "','" + dr.GetValue(1).ToString().Replace("\'", "") + "','" + System.Convert.ToDouble(dr.GetValue(2).ToString().Replace("\'", "")).ToString("0.00") + "');", true);
                    i++;
                    total += System.Convert.ToDouble(dr.GetValue(1).ToString()) * System.Convert.ToDouble(dr.GetValue(2).ToString());
                }
                dr.Close();
                txtTotalCost.Text = "Total: $" + total.ToString();

                sql.CommandText = "Select gnqGeneralNoteID from linkGeneralNoteToQuote where gnqQuoteID = @quote and gnqHTS = 1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    for(int k = 0; k < generalNote.Count; k++)
                    {
                        if (dr.GetValue(0).ToString() == generalNote[k].Text.ToString().Split('-')[0])
                        {
                            cb[k].Checked = true;
                        }

                    }
                }

            }
            connection.Close();
        }

        protected void btnCopyQuote (object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            List<string> insertedNotes = new List<string>();


            int count = 0;
            double totalCost = 0;
            try
            {
                sql.CommandText = "insert into pktblHTSPreWordedNote(hpwCompanyID, hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                sql.CommandText += "values (@company, @note, @quantity, @unit, GETDATE(), @createdBy) ";

                for(int k = 0; k < 150; k++)
                {
                    try
                    {
                        if (Request.Form["notes" + count].ToString() != "" || (Request.Form["qty" + count].ToString() != "" && Request.Form["qty" + count].ToString() != "0") || (Request.Form["unit" + count].ToString() != "" && Request.Form["unit" + count].ToString() != "0.0000"))
                        {
                            sql.Parameters.AddWithValue("@company", master.getCompanyId());
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                            sql.Parameters.AddWithValue("@quantity", Request.Form["qty" + count].ToString());
                            sql.Parameters.AddWithValue("@unit", Request.Form["unit" + count].ToString());
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            try
                            {
                                totalCost += System.Convert.ToDouble(Request.Form["price" + count].ToString());
                            }
                            catch
                            {

                            }

                            string noteID = "";
                            insertedNotes.Add(noteID = master.ExecuteScalar(sql, "EditQuote").ToString());
                            sql.Parameters.Clear();
                        }
                        count++;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch
            {
                Response.Write("<script>alert('Stoped at " + count + "');</script>");
            }


            double totalAmount = 0;
            string productType = "", program = "", oem = "", vehicle = "", dueDate = "", Customer = "", plant = "", salesman = "";
            if (rfqID != 0)
            {
                sql.CommandText = "Select rfqProductTypeID, rfqProgramID, rfqOEMID, rfqVehicleID, rfqDueDate, rfqCustomerID, rfqPlantID from tblRFQ where rfqID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                SqlDataReader rfqDR = sql.ExecuteReader();
                if (rfqDR.Read())
                {
                    productType = rfqDR.GetValue(0).ToString();
                    program = rfqDR.GetValue(1).ToString();
                    oem = rfqDR.GetValue(2).ToString();
                    vehicle = rfqDR.GetValue(3).ToString();
                    dueDate = rfqDR.GetValue(4).ToString();
                    Customer = rfqDR.GetValue(5).ToString();
                    plant = rfqDR.GetValue(6).ToString();
                }
                rfqDR.Close();
            }

            string picture = "";
            if (partID != "" && partID != null)
            {
                sql.CommandText = "Select prtPicture from tblPart where prtPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader rdr = sql.ExecuteReader();
                if (rdr.Read())
                {
                    picture = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }
            else
            {
                //picture = "HTS"
            }


            sql.CommandText = "Select TSGSalesmanID from TSGSalesman where Name = @salesman ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                salesman = dr.GetValue(0).ToString();
            }
            dr.Close();


            sql.CommandText = "INSERT INTO tblHTSQuote(hquRFQID, hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, ";
            sql.CommandText += "hquShippingTerms, hquTotalAmount, hquAnnualVolume, hquProductTypeID, hquProgramCodeID, hquOEM, hquVehicleID, ";
            sql.CommandText += "hquDueDate, hquQuoteTypeID, hquPartTypeID, hquCreated, hquCreatedBy, ";
            sql.CommandText += "hquWinLossID, hquDescription, hquLeadTime, hquSalesman, hquNumber, hquUseTSGLogo, hquUseTSGName, ";
            sql.CommandText += "hquPartNumbers, hquCurrencyID, hquCustomerID, hquCustomerLocationID, hquProcess, hquCavity, hquPartName, hquPicture, hquAccess, hquCustomerContactName, hquCustomerRFQNum, hquMaterialType) ";
            sql.CommandText += "output inserted.hquHTSQuoteID ";
            sql.CommandText += "VALUES (@rfqID, @estID, @version, @jobNum, @status, @payment, @shipping, ";
            sql.CommandText += "@totalAmount, @annualAmount, @productType, @program, @oem, @vehicle, @dueDate, @quoteType, ";
            sql.CommandText += "@partType, GETDATE(), @createdBy, @winLoss, @desc, ";
            sql.CommandText += "@leadTime, @salesman, @number, @useTSGLogo, @useTSGName, @partNums, @currency, @cust, @custLoc, @process, @cavity, @partName, @picture, @access, @customerContact, @custRFQ, @matType)";
            sql.Parameters.Clear();

            if (txtRFQNumber.Visible == false)
            {
                sql.Parameters.AddWithValue("@rfqID", lblRfqNumber.Text);
            }
            else
            {
                sql.Parameters.AddWithValue("@rfqID", txtRFQNumber.Text);
            }
            sql.Parameters.AddWithValue("@estID", ddlEstimator.SelectedValue);
            sql.Parameters.AddWithValue("@version", "001");
            sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
            sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
            sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
            sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
            sql.Parameters.AddWithValue("@totalAmount", totalAmount);
            sql.Parameters.AddWithValue("@annualAmount", 0);
            sql.Parameters.AddWithValue("@productType", productType);
            sql.Parameters.AddWithValue("@program", program);
            sql.Parameters.AddWithValue("@oem", oem);
            sql.Parameters.AddWithValue("@vehicle", vehicle);
            sql.Parameters.AddWithValue("@dueDate", dueDate);
            sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
            sql.Parameters.AddWithValue("@partType", ddlPartType.SelectedValue);
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            sql.Parameters.AddWithValue("@winLoss", "");
            sql.Parameters.AddWithValue("@desc", "");
            sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text);
            sql.Parameters.AddWithValue("@salesman", salesman);
            sql.Parameters.AddWithValue("@number", lblQuoteNumber.Text);
            //sql.Parameters.AddWithValue("@custQuoteNum", txtCustQuoteNumber.Text);
            sql.Parameters.AddWithValue("@useTSGLogo", cbUseTSGLogo.Checked);
            sql.Parameters.AddWithValue("@useTSGName", cbUseTSGName.Checked);
            if (txtPartNumber.Text == "")
            {
                sql.Parameters.AddWithValue("@partNums", lblPartNumber.Text);
            }
            else
            {
                sql.Parameters.AddWithValue("@partNums", txtPartNumber.Text);
            }
            //sql.Parameters.AddWithValue("@currency", 1);
            sql.Parameters.AddWithValue("@cust", ddlCustomer.SelectedValue);
            sql.Parameters.AddWithValue("@custLoc", ddlPlant.SelectedValue);
            sql.Parameters.AddWithValue("@process", ddlProcess.SelectedValue);
            sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
            sql.Parameters.AddWithValue("@partName", txtPartName.Text);
            sql.Parameters.AddWithValue("@picture", picture);
            sql.Parameters.AddWithValue("@access", txtAccess.Text);
            sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
            sql.Parameters.AddWithValue("@custRFQ", txtCustomerRFQ.Text);
            //sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
            sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
            sql.Parameters.AddWithValue("@currency", ddlCurrency.SelectedValue);

            quoteID = System.Convert.ToInt64(master.ExecuteScalar(sql, "HTSEditQuote"));

            sql.CommandText = "update tblHTSQuote set hquPicture = @picture, hquNumber = @number where hquHTSQuoteID = @quoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@picture", "HTS-" + quoteID + ".png");
            sql.Parameters.AddWithValue("@number", quoteID);
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            master.ExecuteNonQuery(sql, "HTS Edit Quote");
            sql.Parameters.Clear();

            newPicture("HTS-" + quoteID + ".png");

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
            cb.Add(cbGeneralNote10);
            cb.Add(cbGeneralNote11);

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
            generalNote.Add(lblGeneralNote10);
            generalNote.Add(lblGeneralNote11);


            for (int i = 0; i < cb.Count; i++)
            {
                if (cb[i].Checked)
                {
                    sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                    sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy, 1)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }
            }


            for (int i = 0; i < insertedNotes.Count; i++)
            {
                sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                sql.CommandText += "VALUES(@quoteID, @pwnID, GETDATE(), @createdBy)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@pwnID", insertedNotes[i]);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "HTSEditQuote");
            }

            sql.CommandText = "";

            //if (rfqID != 0 && (partID != "0" && partID != "" && partID != null))
            //{
            //    sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
            //    sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@partID", partID);
            //    sql.Parameters.AddWithValue("@quoteID", quoteID);
            //    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            //    sql.Parameters.AddWithValue("@hts", true);
            //    master.ExecuteNonQuery(sql, "HTS Edit Quote");

            //    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
            //    sql.CommandText += "Values (@quoteID, @rfqID, GETDATE(), @createdBy, @hts, 0, 0) ";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@quoteID", quoteID);
            //    sql.Parameters.AddWithValue("@rfqID", rfqID);
            //    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            //    sql.Parameters.AddWithValue("@hts", true);
            //    master.ExecuteNonQuery(sql, "HTS Edit Quote");

            //    Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID + "&rfq=" + rfqID + "&partID=" + partID);
            //}
            //else
            //{
                Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID);
            //}
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }


        protected void populate_Plants()
        {
            //populate_Header();
            if(ddlCustomer.SelectedValue != "Please Select")
            {
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
                setSalesmanAndRank();
                connection.Close();
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

        protected void btncreateNewVersionClick(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            List<string> insertedNotes = new List<string>();

            long originalQuoteNumber = quoteID;

            if (quoteID != 0)
            {
                int count = 0;
                double totalCost = 0;
                try
                {
                    sql.CommandText = "insert into pktblHTSPreWordedNote(hpwCompanyID, hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                    sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                    sql.CommandText += "values (@company, @note, @quantity, @unit, GETDATE(), @createdBy) ";

                    for (int k = 0; k < 150; k++)
                    {
                        try
                        {
                            if (Request.Form["notes" + count].ToString() != "" || (Request.Form["qty" + count].ToString() != "" && Request.Form["qty" + count].ToString() != "0") || (Request.Form["unit" + count].ToString() != "" && Request.Form["unit" + count].ToString() != "0.0000"))
                            {
                                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                                sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                                sql.Parameters.AddWithValue("@quantity", Request.Form["qty" + count].ToString());
                                sql.Parameters.AddWithValue("@unit", Request.Form["unit" + count].ToString());
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                try
                                {
                                    totalCost += System.Convert.ToDouble(Request.Form["price" + count].ToString());
                                }
                                catch
                                {

                                }

                                string noteID = "";
                                insertedNotes.Add(noteID = master.ExecuteScalar(sql, "EditQuote").ToString());
                                sql.Parameters.Clear();
                            }
                            count++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Stoped at " + count + "');</script>");
                }

                sql.CommandText = "Select hquNumber from tblHTSQuote where hquHTSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", originalQuoteNumber);
                SqlDataReader sdr = sql.ExecuteReader();
                if(sdr.Read())
                {
                    originalQuoteNumber = System.Convert.ToInt64(sdr.GetValue(0).ToString());
                }
                sdr.Close();

                string version = (System.Convert.ToInt32(lblVersion.Text) + 1).ToString("000");

                if (partID != "0" && partID != "" && partID != null) {
                    sql.CommandText = "Select max(hquVersion) from linkPartToquote, tblHTSQuote where ptqQuoteID = hquHTSQuoteID and ";
                    sql.CommandText += "ptqHTS = 1 and ptqSTS = 0 and ptqUGS = 0 and ptqPartID = @partID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    sdr = sql.ExecuteReader();
                    if (sdr.Read())
                    {
                        version = (System.Convert.ToInt32(sdr.GetValue(0).ToString()) + 1).ToString("000");
                    }
                    sdr.Close();
                }
                else
                {
                    sql.CommandText = "Select hquVersion from tblHTSQuote where hquNumber = @num ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@num", originalQuoteNumber.ToString());
                    sdr = sql.ExecuteReader();
                    while (sdr.Read())
                    {
                        if (System.Convert.ToInt32(sdr.GetValue(0).ToString()) + 1 > System.Convert.ToInt32(sdr.GetValue(0).ToString()))
                        {
                            version = (System.Convert.ToInt32(sdr.GetValue(0).ToString()) + 1).ToString("000");
                        }
                    }
                    sdr.Close();
                }

                string picture = "";
                sql.CommandText = "Select hquPicture from tblHTSQuote where hquHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    picture = dr.GetValue(0).ToString();
                }
                dr.Close();

                double totalAmount = 0;
                string productType = "", program = "", oem = "", vehicle = "", dueDate = "", Customer = "", plant = "", salesman = "";
                if (rfqID != 0)
                {
                    sql.CommandText = "Select rfqProductTypeID, rfqProgramID, rfqOEMID, rfqVehicleID, rfqDueDate, rfqCustomerID, rfqPlantID, rfqSalesman from tblRFQ where rfqID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    SqlDataReader rfqDR = sql.ExecuteReader();
                    if (rfqDR.Read())
                    {
                        productType = rfqDR.GetValue(0).ToString();
                        program = rfqDR.GetValue(1).ToString();
                        oem = rfqDR.GetValue(2).ToString();
                        vehicle = rfqDR.GetValue(3).ToString();
                        dueDate = rfqDR.GetValue(4).ToString();
                        Customer = rfqDR.GetValue(5).ToString();
                        plant = rfqDR.GetValue(6).ToString();
                        salesman = rfqDR.GetValue(7).ToString();
                    }
                    rfqDR.Close();
                }
                else
                {
                    sql.CommandText = "Select TSGSalesmanID from TSGSalesman where Name = @salesman ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        salesman = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                }


                sql.CommandText = "INSERT INTO tblHTSQuote(hquRFQID, hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, ";
                sql.CommandText += "hquShippingTerms, hquTotalAmount, hquAnnualVolume, hquProductTypeID, hquProgramCodeID, hquOEM, hquVehicleID, ";
                sql.CommandText += "hquDueDate, hquQuoteTypeID, hquPartTypeID, hquCreated, hquCreatedBy, ";
                sql.CommandText += "hquWinLossID, hquDescription, hquLeadTime, hquSalesman, hquNumber, hquUseTSGLogo, hquUseTSGName, ";
                sql.CommandText += "hquPartNumbers, hquCurrencyID, hquCustomerID, hquCustomerLocationID, hquProcess, hquCavity, hquPartName, hquPicture, hquAccess, hquCustomerContactName, hquCustomerRFQNum, hquMaterialType) ";
                sql.CommandText += "output inserted.hquHTSQuoteID ";
                sql.CommandText += "VALUES (@rfqID, @estID, @version, @jobNum, @status, @payment, @shipping, ";
                sql.CommandText += "@totalAmount, @annualAmount, @productType, @program, @oem, @vehicle, @dueDate, @quoteType, ";
                sql.CommandText += "@partType, GETDATE(), @createdBy, @winLoss, @desc, ";
                sql.CommandText += "@leadTime, @salesman, @number, @useTSGLogo, @useTSGName, @partNums, @currency, @cust, @custLoc, @process, @cavity, @partName, @picture, @access, @customerContact, @custRFQ, @matType)";
                sql.Parameters.Clear();

                if (txtRFQNumber.Visible == false)
                {
                    sql.Parameters.AddWithValue("@rfqID", lblRfqNumber.Text);
                }
                else
                {
                    sql.Parameters.AddWithValue("@rfqID", txtRFQNumber.Text);
                }
                sql.Parameters.AddWithValue("@estID", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@version", version);
                sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@totalAmount", totalAmount);
                sql.Parameters.AddWithValue("@annualAmount", 0);
                sql.Parameters.AddWithValue("@productType", productType);
                sql.Parameters.AddWithValue("@program", program);
                sql.Parameters.AddWithValue("@oem", oem);
                sql.Parameters.AddWithValue("@vehicle", vehicle);
                sql.Parameters.AddWithValue("@dueDate", dueDate);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
                sql.Parameters.AddWithValue("@partType", ddlPartType.SelectedValue);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@winLoss", "");
                sql.Parameters.AddWithValue("@desc", "");
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@number", "");
                //sql.Parameters.AddWithValue("@custQuoteNum", txtCustQuoteNumber.Text);
                sql.Parameters.AddWithValue("@useTSGLogo", cbUseTSGLogo.Checked);
                sql.Parameters.AddWithValue("@useTSGName", cbUseTSGName.Checked);
                if (txtPartNumber.Text == "")
                {
                    sql.Parameters.AddWithValue("@partNums", lblPartNumber.Text);
                }
                else
                {
                    sql.Parameters.AddWithValue("@partNums", txtPartNumber.Text);
                }
                //sql.Parameters.AddWithValue("@currency", 1);
                sql.Parameters.AddWithValue("@cust", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@custLoc", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@process", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@picture", picture);
                sql.Parameters.AddWithValue("@access", txtAccess.Text);
                sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@custRFQ", txtCustomerRFQ.Text);
                //sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                sql.Parameters.AddWithValue("@currency", ddlCurrency.SelectedValue);

                quoteID = System.Convert.ToInt64(master.ExecuteScalar(sql, "HTSEditQuote"));

                sql.CommandText = "update tblHTSQuote set hquNumber = @number where hquHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                //sql.Parameters.AddWithValue("@picture", "HTS-" + quoteID + ".png");
                sql.Parameters.AddWithValue("@number", originalQuoteNumber);
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "HTS Edit Quote");
                sql.Parameters.Clear();

                newPicture("HTS-" + quoteID + ".png");

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
                cb.Add(cbGeneralNote10);
                cb.Add(cbGeneralNote11);

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
                generalNote.Add(lblGeneralNote10);
                generalNote.Add(lblGeneralNote11);


                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy, 1)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }


                for (int i = 0; i < insertedNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                    sql.CommandText += "VALUES(@quoteID, @pwnID, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@pwnID", insertedNotes[i]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }

                sql.CommandText = "";

                if (rfqID != 0 && (partID != "0" && partID != "" && partID != null))
                {
                    sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
                    sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@hts", true);
                    master.ExecuteNonQuery(sql, "HTS Edit Quote");

                    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                    sql.CommandText += "Values (@quoteID, @rfqID, GETDATE(), @createdBy, @hts, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@hts", true);
                    master.ExecuteNonQuery(sql, "HTS Edit Quote");

                    Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID + "&rfq=" + rfqID + "&partID=" + partID);
                }
                else
                {
                    Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID);
                }
            }
        }

        //protected void btnFinalize_Click(object sender, EventArgs e)
        //{
        //    Site master = new Site();
        //    SqlConnection connection = new SqlConnection(master.getConnectionString());
        //    connection.Open();
        //    SqlCommand sql = new SqlCommand();
        //    sql.Connection = connection;

        //    sql.CommandText = "update tblHTSQuote set hquFinalized = 1, hquModified = GETDATE(), hquModifiedBy = @user where hquHTSQuoteID = @quoteID";
        //    sql.Parameters.Clear();
        //    sql.Parameters.AddWithValue("@quoteID", quoteID);
        //    sql.Parameters.AddWithValue("@user", master.getUserName());
        //    master.ExecuteNonQuery(sql, "Edit Quote");


        //    connection.Close();
        //    populate_Header();
        //}

        protected void btnSaveClick(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            List<string> insertedNotes = new List<string>();

            if(quoteID == 0)
            {
                int count = 0;
                double totalCost = 0;
                try
                {
                    sql.CommandText = "insert into pktblHTSPreWordedNote(hpwCompanyID, hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                    sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                    sql.CommandText += "values (@company, @note, @quantity, @unit, GETDATE(), @createdBy) ";

                    for (int k = 0; k < 150; k++)
                    {
                        try
                        {
                            if (Request.Form["notes" + count].ToString() != "" || (Request.Form["qty" + count].ToString() != "" && Request.Form["qty" + count].ToString() != "0") || (Request.Form["unit" + count].ToString() != "" && Request.Form["unit" + count].ToString() != "0.0000"))
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                                sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                                if (Request.Form["qty" + count].ToString() == "")
                                {
                                    sql.Parameters.AddWithValue("@quantity", 0);
                                }
                                else {
                                    sql.Parameters.AddWithValue("@quantity", Request.Form["qty" + count].ToString());
                                }
                                if (Request.Form["unit" + count].ToString() == "")
                                {
                                    sql.Parameters.AddWithValue("@unit", 0);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@unit", Request.Form["unit" + count].ToString());
                                }

                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                                string noteID = "";
                                insertedNotes.Add(noteID = master.ExecuteScalar(sql, "EditQuote").ToString());
                                sql.Parameters.Clear();
                            }
                            count++;
                        }
                        catch (Exception err)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Stoped at " + count + "');</script>");
                }


                double totalAmount = 0;
                string productType = "", program = "", oem = "", vehicle = "", dueDate = "", Customer = "", plant = "", salesman = "";
                if(rfqID != 0)
                {
                    sql.CommandText = "Select rfqProductTypeID, rfqProgramID, rfqOEMID, rfqVehicleID, rfqDueDate, rfqCustomerID, rfqPlantID, rfqSalesman from tblRFQ where rfqID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    SqlDataReader rfqDR = sql.ExecuteReader();
                    if (rfqDR.Read())
                    {
                        productType = rfqDR.GetValue(0).ToString();
                        program = rfqDR.GetValue(1).ToString();
                        oem = rfqDR.GetValue(2).ToString();
                        vehicle = rfqDR.GetValue(3).ToString();
                        dueDate = rfqDR.GetValue(4).ToString();
                        Customer = rfqDR.GetValue(5).ToString();
                        plant = rfqDR.GetValue(6).ToString();
                        salesman = rfqDR.GetValue(7).ToString();
                    }
                    rfqDR.Close();
                }
                else
                {
                    sql.CommandText = "Select TSGSalesmanID from TSGSalesman where Name = @salesman ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
                    SqlDataReader rdr = sql.ExecuteReader();
                    if (rdr.Read())
                    {
                        salesman = rdr.GetValue(0).ToString();
                    }
                    rdr.Close();
                }

                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", ddlPlant.SelectedValue);
                SqlDataReader sdr = sql.ExecuteReader();
                if (sdr.Read())
                {
                    salesman = sdr.GetValue(0).ToString();
                }
                sdr.Close();

                string picture = "";
                if(partID != "" && partID != null)
                {
                    sql.CommandText = "Select prtPicture from tblPart where prtPartID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    SqlDataReader rdr = sql.ExecuteReader();
                    if(rdr.Read())
                    {
                        picture = rdr.GetValue(0).ToString();
                    }
                    rdr.Close();
                }
                else
                {
                    //picture = "HTS"
                }


                sql.CommandText = "INSERT INTO tblHTSQuote(hquRFQID, hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, ";
                sql.CommandText += "hquShippingTerms, hquTotalAmount, hquAnnualVolume, hquProductTypeID, hquProgramCodeID, hquOEM, hquVehicleID, ";
                sql.CommandText += "hquDueDate, hquQuoteTypeID, hquPartTypeID, hquCreated, hquCreatedBy, ";
                sql.CommandText += "hquWinLossID, hquDescription, hquLeadTime, hquSalesman, hquNumber, hquUseTSGLogo, hquUseTSGName, ";
                sql.CommandText += "hquPartNumbers, hquCurrencyID, hquCustomerID, hquCustomerLocationID, hquProcess, hquCavity, hquPartName, hquPicture, hquAccess, hquCustomerContactName, hquCustomerRFQNum, hquMaterialType) ";
                sql.CommandText += "output inserted.hquHTSQuoteID ";
                sql.CommandText += "VALUES (@rfqID, @estID, @version, @jobNum, @status, @payment, @shipping, ";
                sql.CommandText += "@totalAmount, @annualAmount, @productType, @program, @oem, @vehicle, @dueDate, @quoteType, ";
                sql.CommandText += "@partType, GETDATE(), @createdBy, @winLoss, @desc, ";
                sql.CommandText += "@leadTime, @salesman, @number, @useTSGLogo, @useTSGName, @partNums, @currency, @cust, @custLoc, @process, @cavity, @partName, @picture, @access, @customerContact, @custRFQ, @matType)";
                sql.Parameters.Clear();

                if(txtRFQNumber.Visible == false)
                {
                    sql.Parameters.AddWithValue("@rfqID", lblRfqNumber.Text);
                }
                else
                {
                    sql.Parameters.AddWithValue("@rfqID", txtRFQNumber.Text);
                }
                sql.Parameters.AddWithValue("@estID", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@version", "001");
                sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@totalAmount", totalAmount);
                sql.Parameters.AddWithValue("@annualAmount", 0);
                sql.Parameters.AddWithValue("@productType", productType);
                sql.Parameters.AddWithValue("@program", program);
                sql.Parameters.AddWithValue("@oem", oem);
                sql.Parameters.AddWithValue("@vehicle", vehicle);
                sql.Parameters.AddWithValue("@dueDate", dueDate);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
                sql.Parameters.AddWithValue("@partType", ddlPartType.SelectedValue);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@winLoss", "");
                sql.Parameters.AddWithValue("@desc", "");
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@number", lblQuoteNumber.Text);
                //sql.Parameters.AddWithValue("@custQuoteNum", txtCustQuoteNumber.Text);
                sql.Parameters.AddWithValue("@useTSGLogo", cbUseTSGLogo.Checked);
                sql.Parameters.AddWithValue("@useTSGName", cbUseTSGName.Checked);
                if(txtPartNumber.Text == "")
                {
                    sql.Parameters.AddWithValue("@partNums", txtPartNumber.Text);
                }
                else
                {
                    sql.Parameters.AddWithValue("@partNums", lblPartNumber.Text);
                }
                //sql.Parameters.AddWithValue("@currency", 1);
                sql.Parameters.AddWithValue("@cust", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@custLoc", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@process", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@picture", picture);
                sql.Parameters.AddWithValue("@access", txtAccess.Text);
                sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@custRFQ", txtCustomerRFQ.Text);
                //sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                sql.Parameters.AddWithValue("@currency", ddlCurrency.SelectedValue);

                quoteID = System.Convert.ToInt64(master.ExecuteScalar(sql, "HTSEditQuote"));

                sql.CommandText = "update tblHTSQuote set hquPicture = @picture, hquNumber = @number where hquHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@picture", "HTS-" + quoteID + ".png");
                sql.Parameters.AddWithValue("@number", quoteID);
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "HTS Edit Quote");
                sql.Parameters.Clear();

                newPicture("HTS-" + quoteID + ".png");

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
                cb.Add(cbGeneralNote10);
                cb.Add(cbGeneralNote11);

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
                generalNote.Add(lblGeneralNote10);
                generalNote.Add(lblGeneralNote11);


                for (int i = 0; i < cb.Count; i++)
                {
                    if(cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy, 1)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }


                for (int i = 0; i < insertedNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                    sql.CommandText += "VALUES(@quoteID, @pwnID, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@pwnID", insertedNotes[i]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }

                sql.CommandText = "";

                if(rfqID != 0 && (partID != "0" && partID != "" && partID != null))
                {
                    sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
                    sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@hts", true);
                    master.ExecuteNonQuery(sql, "HTS Edit Quote");

                    List<string> partIDs = new List<string>();
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail ";
                    sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    SqlDataReader dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        partIDs.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    for (int i = 0; i < partIDs.Count; i++)
                    {
                        sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
                        sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partIDs[i]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        sql.Parameters.AddWithValue("@hts", true);
                        master.ExecuteNonQuery(sql, "HTS Edit Quote");
                    }

                    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                    sql.CommandText += "Values (@quoteID, @rfqID, GETDATE(), @createdBy, @hts, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@hts", true);
                    master.ExecuteNonQuery(sql, "HTS Edit Quote");

                    Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID + "&rfq=" + rfqID + "&partID=" + partID);
                }
                else
                {
                    Response.Redirect("https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + quoteID);
                }
            }
            else
            {
                string salesman = "";

                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", ddlPlant.SelectedValue);
                SqlDataReader sdr = sql.ExecuteReader();
                if (sdr.Read())
                {
                    salesman = sdr.GetValue(0).ToString();
                }
                sdr.Close();



                sql.CommandText = "update tblHTSQuote set hquRFQID = @rfqID, hquStatusID = @status, hquPaymentTerms = @payment, hquShippingTerms = @shipping, hquQuoteTypeID = @quoteType, hquPartTypeID = @partType, hquLeadTime = @leadtime, ";
                sql.CommandText += " hquNumber = @number, hquUseTSGLogo = @logo, hquUseTSGName = @name, hquPartNumbers = @nums, hquCustomerID = @cust, hquCustomerLocationID = @loc, ";
                sql.CommandText += "hquProcess = @process, hquCavity = @cavity, hquPartName = @partName, hquModified = GETDATE(), hquModifiedBy = @modifiedBy, hquAccess = @access, hquCustomerContactName = @customerContact, hquCustomerRFQNum = @custRFQ, ";
                sql.CommandText += "hquJobNumberID = @jobNum, hquMaterialType = @matType, hquSalesman = @salesman, hquCurrencyID = @currency, hquEstimatorID = @estimator where hquHTSQuoteID = @quoteID";

                sql.Parameters.Clear();

                if (txtRFQNumber.Visible == false)
                {
                    sql.Parameters.AddWithValue("@rfqID", lblRfqNumber.Text);
                }
                else
                {
                    //sql.Parameters.AddWithValue("@rfqID", txtRFQNumber.Text);
                }
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
                sql.Parameters.AddWithValue("@partType", ddlPartType.SelectedValue);
                sql.Parameters.AddWithValue("@leadTime", txtLeadTime.Text);
                //sql.Parameters.AddWithValue("@custQuote", txtCustQuoteNumber.Text);
                sql.Parameters.AddWithValue("@number", lblQuoteNumber.Text);
                sql.Parameters.AddWithValue("@logo", cbUseTSGLogo.Checked);
                sql.Parameters.AddWithValue("@name", cbUseTSGName.Checked);
                //if(txtWBPartNumber.Text == "")
                //{
                    if(lblPartNumber.Text == "")
                    {
                        sql.Parameters.AddWithValue("@nums", txtPartNumber.Text);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@nums", lblPartNumber.Text);
                    }
                //}
                //else
                //{
                //    sql.Parameters.AddWithValue("@nums", txtWBPartNumber.Text);

                //}
                sql.Parameters.AddWithValue("@cust", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@loc", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@process", ddlProcess.SelectedValue);
                sql.Parameters.AddWithValue("@cavity", ddlCavity.SelectedValue);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@modifiedBy", master.getUserName());
                sql.Parameters.AddWithValue("@access", txtAccess.Text);
                sql.Parameters.AddWithValue("@customerContact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@custRFQ", txtCustomerRFQ.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNum.Text);
                sql.Parameters.AddWithValue("@matType", txtMaterialType.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@currency", ddlCurrency.SelectedValue);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                master.ExecuteNonQuery(sql, "HTS Edit Quote");

                newPicture("HTS-" + quoteID + ".png");

                sql.CommandText = "Select pthHTSPWNID from linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                List<string> pwnIDs = new List<string>();
                SqlDataReader dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    pwnIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "HTSEditQuote");



                sql.CommandText = "Delete from linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "HTSEditQuote");

                for(int i = 0; i < pwnIDs.Count; i++)
                {
                    sql.CommandText = "delete from pktblHTSPreWordedNote where hpwHTSPreWordedNoteID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", pwnIDs[i]);
                    master.ExecuteNonQuery(sql, "HTS Edit Quote");
                }
                pwnIDs.Clear();

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
                cb.Add(cbGeneralNote10);
                cb.Add(cbGeneralNote11);

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
                generalNote.Add(lblGeneralNote10);
                generalNote.Add(lblGeneralNote11);

                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy, 1)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }

                insertedNotes.Clear();
                int count = 0;
                try
                {
                    sql.CommandText = "insert into pktblHTSPreWordedNote(hpwCompanyID, hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                    sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                    sql.CommandText += "values (@company, @note, @quantity, @unit, GETDATE(), @createdBy) ";

                    for (int k = 0; k < 150; k++)
                    {
                        try
                        {
                            if (Request.Form["notes" + count].ToString() != "" || (Request.Form["qty" + count].ToString() != "" && Request.Form["qty" + count].ToString() != "0") || (Request.Form["unit" + count].ToString() != "" && Request.Form["unit" + count].ToString() != "0.0000"))
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                                sql.Parameters.AddWithValue("@note", Request.Form["notes" + count].ToString());
                                if (Request.Form["qty" + count].ToString() == "")
                                {
                                    sql.Parameters.AddWithValue("@quantity", 0);
                                }
                                else {
                                    sql.Parameters.AddWithValue("@quantity", Request.Form["qty" + count].ToString());
                                }
                                if (Request.Form["unit" + count].ToString() == "")
                                {
                                    sql.Parameters.AddWithValue("@unit", 0);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@unit", Request.Form["unit" + count].ToString());
                                }
                                
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                                string noteID = "";
                                insertedNotes.Add(noteID = master.ExecuteScalar(sql, "EditQuote").ToString());
                                sql.Parameters.Clear();
                            }
                            count++;
                        }
                        catch (Exception err)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                    Response.Write("<script>alert('Stoped at " + count + "');</script>");
                }

                for (int i = 0; i < insertedNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                    sql.CommandText += "VALUES(@quoteID, @pwnID, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@pwnID", insertedNotes[i]);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }
            }

            connection.Close();
            populate_Header();

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
                Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("HTS quote pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = newStream;
                newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/HTS quote pictures/" + pictureName;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                partPicturesList.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                // set the Attributes
                Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                newItem.Update();
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "update tblHTSQuote set hquPicture = @pic where hquHTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@pic", pictureName);
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "HTS Edit Quote");
            }
        }
    }
}