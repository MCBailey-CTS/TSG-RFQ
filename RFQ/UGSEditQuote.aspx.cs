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
    public partial class UGSEditQuote : System.Web.UI.Page
    {
        int rfqID = 0;
        string quoteID = "";
        string partID = "";
        public Boolean IsMasterCompany = false;
        public long UserCompanyID = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if(master.getCompanyId() != 15)
            {
                litHideBtn.Text = "<script>$('#btnBudget').hide();</script>";
            }

            try
            {
                quoteID = Request["id"].ToString();
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
            litKeyUp.Text = "<script>keyup(" + Total.Text + ");</script>";

            if (!IsPostBack)
            {
                sql.CommandText = "Select qstQuoteStatusID, qstQuoteStatus from pktblQuoteStatus order by qstQuoteStatus";
                sql.Parameters.Clear();
                SqlDataReader qsDR = sql.ExecuteReader();
                ddlStatus.DataSource = qsDR;
                ddlStatus.DataTextField = "qstQuoteStatus";
                ddlStatus.DataValueField = "qstQuoteStatusID";
                ddlStatus.DataBind();
                qsDR.Close();
                ddlStatus.SelectedValue = "2";

                sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer where cusInactive <> 1 or cusInactive is null order by CustomerName ";
                SqlDataReader CustomerDR = sql.ExecuteReader();
                ddlCustomer.DataSource = CustomerDR;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "Please Select");
                CustomerDR.Close();

                sql.CommandText = "Select dtyFullName as name, DieTypeID from DieType where TSGCompanyID = 15 Order by DieTypeID";
                SqlDataReader processDR = sql.ExecuteReader();
                ddlQuoteType.DataSource = processDR;
                ddlQuoteType.DataTextField = "name";
                ddlQuoteType.DataValueField = "DieTypeID";
                ddlQuoteType.DataBind();
                processDR.Close();

                sql.CommandText = "Select CONCAT(estFirstName, ' ', estLastName) as 'name', estEstimatorID from pktblEstimators where estCompanyID = 15";
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

                populate_header();

                if (quoteID == "")
                {
                    sql.CommandText = "Select ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ";
                    sql.CommandText += "ucoCNC, ucoCertification, ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins,  ";
                    sql.CommandText += "ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ucoBasePlate, ucoAluminum, ucoSteel, ";
                    sql.CommandText += "ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ucoIndStorCase, ";
                    sql.CommandText += "ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ucoToolingBalls, ";
                    sql.CommandText += "ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ucoHeatTreat, ";
                    sql.CommandText += "ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ucoWelding, ucoWireBurn, ucoRebates from pktblUGSCost ";
                    sql.CommandText += "where ucoUGSCostID = 1";
                    sql.Parameters.Clear();
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        tManagement.Text = System.Convert.ToDouble(dr["ucoManagement"].ToString()).ToString().Replace(",", ".");
                        tProjectEng.Text = System.Convert.ToDouble(dr["ucoProjectEng"].ToString()).ToString().Replace(",", ".");
                        tReadData.Text = System.Convert.ToDouble(dr["ucoReadData"].ToString()).ToString().Replace(",", ".");
                        t3DModel.Text = System.Convert.ToDouble(dr["uco3DModel"].ToString()).ToString().Replace(",", ".");
                        tDrawing.Text = System.Convert.ToDouble(dr["ucoDrawing"].ToString()).ToString().Replace(",", ".");
                        tUpdates.Text = System.Convert.ToDouble(dr["ucoUpdates"].ToString()).ToString().Replace(",", ".");
                        tProgramming.Text = System.Convert.ToDouble(dr["ucoProgramming"].ToString()).ToString().Replace(",", ".");
                        tCNC.Text = System.Convert.ToDouble(dr["ucoCNC"].ToString()).ToString().Replace(",", ".");
                        tCertification.Text = System.Convert.ToDouble(dr["ucoCertification"].ToString()).ToString().Replace(",", ".");
                        tPartLayouts.Text = System.Convert.ToDouble(dr["ucoPartLayouts"].ToString()).ToString().Replace(",", ".");
                        tBase.Text = System.Convert.ToDouble(dr["ucoBase"].ToString()).ToString().Replace(",", ".");
                        tDetails.Text = System.Convert.ToDouble(dr["ucoDetails"].ToString()).ToString().Replace(",", ".");
                        tLocationPins.Text = System.Convert.ToDouble(dr["ucoLocationPins"].ToString()).ToString().Replace(",", ".");
                        tGoNoGoPins.Text = System.Convert.ToDouble(dr["ucoGoNoGoPins"].ToString()).ToString().Replace(",", ".");
                        tSPC.Text = System.Convert.ToDouble(dr["ucoSPC"].ToString()).ToString().Replace(",", ".");
                        tAssemble.Text = System.Convert.ToDouble(dr["ucoAssemble"].ToString()).ToString().Replace(",", ".");
                        tPallets.Text = System.Convert.ToDouble(dr["ucoPallets"].ToString()).ToString().Replace(",", ".");
                        tTransportation.Text = System.Convert.ToDouble(dr["ucoTransportation"].ToString()).ToString().Replace(",", ".");
                        tBasePlate.Text = System.Convert.ToDouble(dr["ucoBasePlate"].ToString()).ToString().Replace(",", ".");
                        tAluminum.Text = System.Convert.ToDouble(dr["ucoAluminum"].ToString()).ToString().Replace(",", ".");
                        tSteel.Text = System.Convert.ToDouble(dr["ucoSteel"].ToString()).ToString().Replace(",", ".");
                        tFixturePlank.Text = System.Convert.ToDouble(dr["ucoFixturePlank"].ToString()).ToString().Replace(",", ".");
                        tWood.Text = System.Convert.ToDouble(dr["ucoWood"].ToString()).ToString().Replace(",", ".");
                        tBushings.Text = System.Convert.ToDouble(dr["ucoBushings"].ToString()).ToString().Replace(",", ".");
                        tDrillBlanks.Text = System.Convert.ToDouble(dr["ucoDrillBlanks"].ToString()).ToString().Replace(",", ".");
                        tClamps.Text = System.Convert.ToDouble(dr["ucoClamps"].ToString()).ToString().Replace(",", ".");
                        tIndicator.Text = System.Convert.ToDouble(dr["ucoIndicator"].ToString()).ToString().Replace(",", ".");
                        tIndCollar.Text = System.Convert.ToDouble(dr["ucoIndCollar"].ToString()).ToString().Replace(",", ".");
                        tIndStorCase.Text = System.Convert.ToDouble(dr["ucoIndStorCase"].ToString()).ToString().Replace(",", ".");
                        tZeroSet.Text = System.Convert.ToDouble(dr["ucoZeroSet"].ToString()).ToString().Replace(",", ".");
                        tSpcTriggers.Text = System.Convert.ToDouble(dr["ucoSpcTriggers"].ToString()).ToString().Replace(",", ".");
                        tTempDrops.Text = System.Convert.ToDouble(dr["ucoTempDrops"].ToString()).ToString().Replace(",", ".");
                        tHingeDrops.Text = System.Convert.ToDouble(dr["ucoHingeDrops"].ToString()).ToString().Replace(",", ".");
                        tRisers.Text = System.Convert.ToDouble(dr["ucoRisers"].ToString()).ToString().Replace(",", ".");
                        tHandles.Text = System.Convert.ToDouble(dr["ucoHandles"].ToString()).ToString().Replace(",", ".");
                        tJigFeet.Text = System.Convert.ToDouble(dr["ucoJigFeet"].ToString()).ToString().Replace(",", ".");
                        tToolingBalls.Text = System.Convert.ToDouble(dr["ucoToolingBalls"].ToString()).ToString().Replace(",", ".");
                        tTBCovers.Text = System.Convert.ToDouble(dr["ucoTBCovers"].ToString()).ToString().Replace(",", ".");
                        tTBPads.Text = System.Convert.ToDouble(dr["ucoTBPads"].ToString()).ToString().Replace(",", ".");
                        tSlides.Text = System.Convert.ToDouble(dr["ucoSlides"].ToString()).ToString().Replace(",", ".");
                        tMagnets.Text = System.Convert.ToDouble(dr["ucoMagnets"].ToString()).ToString().Replace(",", ".");
                        tHardware.Text = System.Convert.ToDouble(dr["ucoHardware"].ToString()).ToString().Replace(",", ".");
                        tLMI.Text = System.Convert.ToDouble(dr["ucoLMI"].ToString()).ToString().Replace(",", ".");
                        tAnnodizing.Text = System.Convert.ToDouble(dr["ucoAnnodizing"].ToString()).ToString().Replace(",", ".");
                        tBlackOxide.Text = System.Convert.ToDouble(dr["ucoBlackOxide"].ToString()).ToString().Replace(",", ".");
                        tHeatTreat.Text = System.Convert.ToDouble(dr["ucoHeatTreat"].ToString()).ToString().Replace(",", ".");
                        tEngrvdTags.Text = System.Convert.ToDouble(dr["ucoEngrvdTags"].ToString()).ToString().Replace(",", ".");
                        tCNCServices.Text = System.Convert.ToDouble(dr["ucoCNCServices"].ToString()).ToString().Replace(",", ".");
                        tGrinding.Text = System.Convert.ToDouble(dr["ucoGrinding"].ToString()).ToString().Replace(",", ".");
                        tShipping.Text = System.Convert.ToDouble(dr["ucoShipping"].ToString()).ToString().Replace(",", ".");
                        tThirdPartyCMM.Text = System.Convert.ToDouble(dr["ucoThirdPartyCMM"].ToString()).ToString().Replace(",", ".");
                        tWelding.Text = System.Convert.ToDouble(dr["ucoWelding"].ToString()).ToString().Replace(",", ".");
                        tWireBurn.Text = System.Convert.ToDouble(dr["ucoWireBurn"].ToString()).ToString().Replace(",", ".");
                        tGageRRCMM.Text = System.Convert.ToDouble(dr["ucoGageRRCMM"].ToString()).ToString().Replace(",", ".");
                        tGageRRFixtures.Text = System.Convert.ToDouble(dr["ucoGageRRFixtures"].ToString()).ToString().Replace(",", ".");
                        tRebates.Text = System.Convert.ToDouble(dr["ucoRebates"].ToString()).ToString().Replace(",", ".");
                    }
                    dr.Close();
                    if (txtLength.Text == "")
                    {
                        txtLength.Text = "0";
                    }
                    if (txtWidth.Text == "") 
                    {
                        txtWidth.Text = "0";
                    }
                    if (txtHeight.Text == "")
                    {
                        txtHeight.Text = "0";
                    }

                    txtNotes.Value = "QUOTE DESCRIPTION\n";
                    txtNotes.Value += "Design / Build / Certify (1) attribute check fixture for checking of the above part.\n ";

                    txtNotes.Value += "GENERAL CONTENT\n";
                    txtNotes.Value += "•	Read / Log In latest math data\n";
                    txtNotes.Value += "•	3 – D Solid Works fixture design. (approval required)\n";
                    txtNotes.Value += "•	Parts to be held on fixture in car position\n";
                    txtNotes.Value += "•	Parts to be held on fixture 90 deg to car position\n";
                    txtNotes.Value += "•	1.00” aluminum plate base with jig feet, (3) tool balls and handles\n";
                    txtNotes.Value += "•	Welded rib aluminum base with lift rings, (3) tool balls and body lines\n";
                    txtNotes.Value += "•	Datum scheme (X) – A – net, RFS / MMC – B - & RFS / MMC – C –\n";
                    txtNotes.Value += "•	Aluminum construction with steel nets, locators and location pins\n";
                    txtNotes.Value += "•	(X) Destaco clamps\n";
                    txtNotes.Value += "•	Feeler to check part form\n";
                    txtNotes.Value += "•	(1) Go / nogo feeler\n";
                    txtNotes.Value += "•	Tolerance Groove to check part trim edge\n";
                    txtNotes.Value += "•	Flush check to check part trim edge\n";
                    txtNotes.Value += "•	(X) SPC checks\n";
                    txtNotes.Value += "•	Mitutoyo indicator, zero set block and indicator storage case provided\n";
                    txtNotes.Value += "•	(X) Location checks for holes / slots\n";
                    txtNotes.Value += "•	(X) Go / nogo pin to check hole size\n";
                    txtNotes.Value += "•	Go / nogo feeler for flatness check\n";
                    txtNotes.Value += "•	CMM Certified (ISO 17025)\n";
                    txtNotes.Value += "•	Steel welded gage cart\n";
                    txtNotes.Value += "•	Gage R & R 5x3x3\n";
                    txtNotes.Value += "•	3rd Party Certification\n";
                    //txtNotes.Value += "•	3rd Party Certification\n";

                    string user = master.getUserName();

                    //if(user == "sjelsma@toolingsystemsgroup.com")
                    //{
                    //    ddlEstimator.SelectedValue = "70";
                    //}
                    if (user == "jmomber@toolingsystemsgroup.com")
                    {
                        ddlEstimator.SelectedValue = "72";
                    }
                    //else if (user == "cgould@toolingsystemsgroup.com")
                    //{
                    //    ddlEstimator.SelectedValue = "73";
                    //}

                    ddlPayment.SelectedValue = "1";
                    ddlQuoteType.SelectedValue = "109";
                    ddlShipping.SelectedValue = "1";
                    txtShipping.Text = "GRR";
                }
            }

            connection.Close();
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
            //generalNote.Add(lblGeneralNote6);
            //generalNote.Add(lblGeneralNote7);
            //generalNote.Add(lblGeneralNote8);
            //generalNote.Add(lblGeneralNote9);


            List<CheckBox> cb = new List<CheckBox>();
            cb.Add(cbGeneralNote1);
            cb.Add(cbGeneralNote2);
            cb.Add(cbGeneralNote3);
            cb.Add(cbGeneralNote4);
            cb.Add(cbGeneralNote5);
            //cb.Add(cbGeneralNote6);
            //cb.Add(cbGeneralNote7);
            //cb.Add(cbGeneralNote8);
            //cb.Add(cbGeneralNote9);



            sql.CommandText = "Select concat(gnoGeneralNoteID, '-', gnoGeneralNote) from pktblGeneralNote where gnoCompany = 'UGS'";
            SqlDataReader gnodr = sql.ExecuteReader();
            int j = 0;
            while (gnodr.Read())
            {
                generalNote[j].Text = gnodr.GetValue(0).ToString();
                j++;
            }
            gnodr.Close();

            if (rfqID != 0 && partID != "")
            {
                string cust = "", plant = "";
                sql.CommandText = "Select prtPartDescription, rfqCustomerRFQNumber, rfqCustomerID, rfqPlantID, TSGSalesman.Name, rfqProductTypeID, rfqOEMID, prtPartTypeID, ";
                sql.CommandText += "prtPartNumber, CustomerContact.Name, prtPartLength, prtPartWidth, prtPartHeight ";
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
                    populate_Plants();
                    ddlPlant.Text = dr.GetValue(3).ToString();
                    plant = dr.GetValue(3).ToString();
                    lblSalesman.Text = dr.GetValue(4).ToString();
                    ddlCustomer.Enabled = false;
                    //ddlPlant.Enabled = false;
                    txtCustomerContact.Text = dr.GetValue(9).ToString();
                    txtCustomerContact.ReadOnly = true;
                    txtLength.Text = dr.GetValue(10).ToString();
                    txtWidth.Text = dr.GetValue(11).ToString();
                    txtHeight.Text = dr.GetValue(12).ToString();
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


            if (quoteID != "")
            {
                cbGeneralNote1.Checked = false;
                cbGeneralNote2.Checked = false;
                cbGeneralNote3.Checked = false;
                cbGeneralNote4.Checked = false;
                cbGeneralNote5.Checked = false;

                hdnQuoteNumber.Value = quoteID;
                string costID = "";
                string cust = "", plant = "";

                sql.CommandText = "Select uquQuoteNumber, uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, ";
                sql.CommandText += "uquCustomerContact, uquSalesmanID, uquCustomerRFQNumber, ";
                sql.CommandText += "uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquUseTSG, uquNotes, uquTotalPrice, uquDieType, ";
                sql.CommandText += "uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC,  ";
                sql.CommandText += "uquCertification, uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins,  ";
                sql.CommandText += "uquSPC, uquGageRRFixtures, uquAssemble, uquPallets, uquTransportation, uquBasePlate, uquAluminum,  ";
                sql.CommandText += "uquSteel, uquFixturePlank, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar,  ";
                sql.CommandText += "uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles,  ";
                sql.CommandText += "uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI,  ";
                sql.CommandText += "uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping,  ";
                sql.CommandText += "uquThirdPartyCMM, uquWelding, uquWireBurn, uquRebates, uquUGSCostID, uquShippingLocation, uquPartLength, ";
                sql.CommandText += "uquPartWidth, uquPartHeight, uquFinalized ";
                sql.CommandText += "from tblUGSQuote where uquUGSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    lblquoteID.Text = quoteID;
                    lblQuoteNumber.Text = dr.GetValue(0).ToString();
                    lblVersion.Text = dr.GetValue(1).ToString();
                    ddlStatus.SelectedValue = dr.GetValue(2).ToString();
                    txtPartNumber.Text = dr.GetValue(3).ToString();
                    txtPartName.Text = dr.GetValue(4).ToString();
                    txtRFQNumber.Text = dr.GetValue(5).ToString();
                    ddlCustomer.SelectedValue = dr.GetValue(6).ToString();
                    cust = dr.GetValue(6).ToString();
                    if (ddlCustomer.SelectedValue == cust)
                    {
                        populate_Plants();
                    }
                    ddlPlant.SelectedValue = dr.GetValue(7).ToString();
                    plant = dr.GetValue(7).ToString();
                    txtCustomerContact.Text = dr.GetValue(8).ToString();
                    txtCustomerRFQ.Text = dr.GetValue(10).ToString();
                    ddlEstimator.SelectedValue = dr.GetValue(11).ToString();
                    ddlShipping.SelectedValue = dr.GetValue(12).ToString();
                    ddlPayment.SelectedValue = dr.GetValue(13).ToString();
                    txtLeadTime.Text = dr.GetValue(14).ToString();
                    txtJobNumber.Text = dr.GetValue(15).ToString();
                    cbUseTSG.Checked = dr.GetBoolean(16);
                    txtNotes.Value = dr.GetValue(17).ToString();
                    txtTotalCost.Text = System.Convert.ToDouble(dr.GetValue(18).ToString()).ToString("###,###,##0.00");
                    ddlQuoteType.SelectedValue = dr.GetValue(19).ToString();
                    txtManagement.Text = dr["uquManagement"].ToString();
                    txtProjectEng.Text = dr["uquProjectEng"].ToString();
                    txtReadData.Text = dr["uquReadData"].ToString();
                    txt3DModel.Text = dr["uqu3DModel"].ToString();
                    txtDrawings.Text = dr["uquDrawing"].ToString();
                    txtUpdates.Text = dr["uquUpdates"].ToString();
                    txtProgramming.Text = dr["uquPrograming"].ToString();
                    txtCNC.Text = dr["uquCNC"].ToString();
                    txtCertification.Text = dr["uquCertification"].ToString();
                    txtGageRR.Text = dr["uquGageRRCMM"].ToString();
                    txtPartLayouts.Text = dr["uquPartLayouts"].ToString();
                    txtBase.Text = dr["uquBase"].ToString();
                    txtDetails.Text = dr["uquDetails"].ToString();
                    txtLocationPins.Text = dr["uquLocationPins"].ToString();
                    txtGoNoGoPins.Text = dr["uquGoNoGoPins"].ToString();
                    txtSPC.Text = dr["uquSPC"].ToString();
                    txtGageRRF.Text = dr["uquGageRRFixtures"].ToString();
                    txtAssemble.Text = dr["uquAssemble"].ToString();
                    txtPallets.Text = dr["uquPallets"].ToString();
                    txtTransportation.Text = dr["uquTransportation"].ToString();
                    txtBasePlate.Text = dr["uquBasePlate"].ToString();
                    txtAluminum.Text = dr["uquAluminum"].ToString();
                    txtSteel.Text = dr["uquSteel"].ToString();
                    txtFixturePlank.Text = dr["uquFixturePlank"].ToString();
                    txtWood.Text = dr["uquWood"].ToString();
                    txtBushings.Text = dr["uquBushings"].ToString();
                    txtDrillBlanks.Text = dr["uquDrillBlanks"].ToString();
                    txtClamps.Text = dr["uquClamps"].ToString();
                    txtIndicator.Text = dr["uquIndicator"].ToString();
                    txtIndCollar.Text = dr["uquIndCollar"].ToString();
                    txtIndStorCase.Text = dr["uquIndStorCase"].ToString();
                    txtZeroSet.Text = dr["uquZeroSet"].ToString();
                    txtSpcTriggers.Text = dr["uquSpcTriggers"].ToString();
                    txtTempDrops.Text = dr["uquTempDrops"].ToString();
                    txtHingeDrops.Text = dr["uquHingeDrops"].ToString();
                    txtRisers.Text = dr["uquRisers"].ToString();
                    txtHandles.Text = dr["uquHandles"].ToString();
                    txtJigFeet.Text = dr["uquJigFeet"].ToString();
                    txtToolingBalls.Text = dr["uquToolingBalls"].ToString();
                    txtTBCovers.Text = dr["uquTBCovers"].ToString();
                    txtTBPads.Text = dr["uquTBPads"].ToString();
                    txtSlides.Text = dr["uquSlides"].ToString();
                    txtMagnets.Text = dr["uquMagnets"].ToString();
                    txtHardware.Text = dr["uquHardware"].ToString();
                    txtLMI.Text = dr["uquLMI"].ToString();
                    txtAnnodizing.Text = dr["uquAnnodizing"].ToString();
                    txtBlackOxide.Text = dr["uquBlackOxide"].ToString();
                    txtHeatTreat.Text = dr["uquHeatTreat"].ToString();
                    txtEngrvdTags.Text = dr["uquEngrvdTags"].ToString();
                    txtCNCServices.Text = dr["uquCNCServices"].ToString();
                    txtGrinding.Text = dr["uquGrinding"].ToString();
                    txtShippingCalc.Text = dr["uquShipping"].ToString();
                    txtThirdPartyCMM.Text = dr["uquThirdPartyCMM"].ToString();
                    txtWelding.Text = dr["uquWelding"].ToString();
                    txtWireBurn.Text = dr["uquWireBurn"].ToString();
                    txtRebates.Text = dr["uquRebates"].ToString();
                    costID = dr["uquUGSCostID"].ToString();
                    txtShipping.Text = dr["uquShippingLocation"].ToString();
                    txtLength.Text = dr["uquPartLength"].ToString();
                    txtWidth.Text = dr["uquPartWidth"].ToString();
                    txtHeight.Text = dr["uquPartHeight"].ToString();
                    if (dr["uquFinalized"].ToString() == "True")
                    {
                        //litStatus.Text = "<script>$('#btnCheck').hide();</script>";
                        btnSave_Click.Visible = false;
                        //btnFinalize.Visible = false;
                        litStatus.Text = "<h3><font color='red'>This quote has been finalized and is not editable.</font></h3>";
                    }
                }
                dr.Close();


                sql.CommandText = "select pwnPreWordedNote, pwnCostNote, pwnPreWordedNoteID from linkPWNToUGSQuote, pktblPreWordedNote ";
                sql.CommandText += "where puqPreWordedNoteID = pwnPreWordedNoteID and puqUGSQuoteID = @id order by pwnPreWordedNoteID ASC ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);

                int i = 0;

                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "addNoteRow" + i.ToString(), "addNoteRow('" + HttpUtility.JavaScriptStringEncode(dr.GetValue(0).ToString().Replace("\'", "")) + "','" + System.Convert.ToDouble(dr.GetValue(1).ToString()).ToString("0.00") + "');", true);

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

                if (costID != "")
                {
                    sql.CommandText = "Select ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ";
                    sql.CommandText += "ucoCNC, ucoCertification, ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins,  ";
                    sql.CommandText += "ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ucoBasePlate, ucoAluminum, ucoSteel, ";
                    sql.CommandText += "ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ucoIndStorCase, ";
                    sql.CommandText += "ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ucoToolingBalls, ";
                    sql.CommandText += "ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ucoHeatTreat, ";
                    sql.CommandText += "ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ucoWelding, ucoWireBurn, ucoRebates, ucoCost from pktblUGSCost ";
                    sql.CommandText += "where ucoUGSCostID = @costID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@costID", costID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        tManagement.Text = dr["ucoManagement"].ToString().Replace(",", ".").Replace(".00", "");
                        tProjectEng.Text = dr["ucoProjectEng"].ToString().Replace(",", ".").Replace(".00", "");
                        tReadData.Text = dr["ucoReadData"].ToString().Replace(",", ".").Replace(".00", "");
                        t3DModel.Text = dr["uco3DModel"].ToString().Replace(",", ".").Replace(".00", "");
                        tDrawing.Text = dr["ucoDrawing"].ToString().Replace(",", ".").Replace(".00", "");
                        tUpdates.Text = dr["ucoUpdates"].ToString().Replace(",", ".").Replace(".00", "");
                        tProgramming.Text = dr["ucoProgramming"].ToString().Replace(",", ".").Replace(".00", "");
                        tCNC.Text = dr["ucoCNC"].ToString().Replace(",", ".").Replace(".00", "");
                        tCertification.Text = dr["ucoCertification"].ToString().Replace(",", ".").Replace(".00", "");
                        tPartLayouts.Text = dr["ucoPartLayouts"].ToString().Replace(",", ".").Replace(".00", "");
                        tBase.Text = dr["ucoBase"].ToString().Replace(",", ".").Replace(".00", "");
                        tDetails.Text = dr["ucoDetails"].ToString().Replace(",", ".").Replace(".00", "");
                        tLocationPins.Text = dr["ucoLocationPins"].ToString().Replace(",", ".").Replace(".00", "");
                        tGoNoGoPins.Text = dr["ucoGoNoGoPins"].ToString().Replace(",", ".").Replace(".00", "");
                        tSPC.Text = dr["ucoSPC"].ToString().Replace(",", ".").Replace(".00", "");
                        tAssemble.Text = dr["ucoAssemble"].ToString().Replace(",", ".").Replace(".00", "");
                        tPallets.Text = dr["ucoPallets"].ToString().Replace(",", ".").Replace(".00", "");
                        tTransportation.Text = dr["ucoTransportation"].ToString().Replace(",", ".").Replace(".00", "");
                        tBasePlate.Text = dr["ucoBasePlate"].ToString().Replace(",", ".").Replace(".00", "");
                        tAluminum.Text = dr["ucoAluminum"].ToString().Replace(",", ".").Replace(".00", "");
                        tSteel.Text = dr["ucoSteel"].ToString().Replace(",", ".").Replace(".00", "");
                        tFixturePlank.Text = dr["ucoFixturePlank"].ToString().Replace(",", ".").Replace(".00", "");
                        tWood.Text = dr["ucoWood"].ToString().Replace(",", ".").Replace(".00", "");
                        tBushings.Text = dr["ucoBushings"].ToString().Replace(",", ".").Replace(".00", "");
                        tDrillBlanks.Text = dr["ucoDrillBlanks"].ToString().Replace(",", ".").Replace(".00", "");
                        tClamps.Text = dr["ucoClamps"].ToString().Replace(",", ".").Replace(".00", "");
                        tIndicator.Text = dr["ucoIndicator"].ToString().Replace(",", ".").Replace(".00", "");
                        tIndCollar.Text = dr["ucoIndCollar"].ToString().Replace(",", ".").Replace(".00", "");
                        tIndStorCase.Text = dr["ucoIndStorCase"].ToString().Replace(",", ".").Replace(".00", "");
                        tZeroSet.Text = dr["ucoZeroSet"].ToString().Replace(",", ".").Replace(".00", "");
                        tSpcTriggers.Text = dr["ucoSpcTriggers"].ToString().Replace(",", ".").Replace(".00", "");
                        tTempDrops.Text = dr["ucoTempDrops"].ToString().Replace(",", ".").Replace(".00", "");
                        tHingeDrops.Text = dr["ucoHingeDrops"].ToString().Replace(",", ".").Replace(".00", "");
                        tRisers.Text = dr["ucoRisers"].ToString().Replace(",", ".").Replace(".00", "");
                        tHandles.Text = dr["ucoHandles"].ToString().Replace(",", ".").Replace(".00", "");
                        tJigFeet.Text = dr["ucoJigFeet"].ToString().Replace(",", ".").Replace(".00", "");
                        tToolingBalls.Text = dr["ucoToolingBalls"].ToString().Replace(",", ".").Replace(".00", "");
                        tTBCovers.Text = dr["ucoTBCovers"].ToString().Replace(",", ".").Replace(".00", "");
                        tTBPads.Text = dr["ucoTBPads"].ToString().Replace(",", ".").Replace(".00", "");
                        tSlides.Text = dr["ucoSlides"].ToString().Replace(",", ".").Replace(".00", "");
                        tMagnets.Text = dr["ucoMagnets"].ToString().Replace(",", ".").Replace(".00", "");
                        tHardware.Text = dr["ucoHardware"].ToString().Replace(",", ".").Replace(".00", "");
                        tLMI.Text = dr["ucoLMI"].ToString().Replace(",", ".").Replace(".00", "");
                        tAnnodizing.Text = dr["ucoAnnodizing"].ToString().Replace(",", ".").Replace(".00", "");
                        tBlackOxide.Text = dr["ucoBlackOxide"].ToString().Replace(",", ".").Replace(".00", "");
                        tHeatTreat.Text = dr["ucoHeatTreat"].ToString().Replace(",", ".").Replace(".00", "");
                        tEngrvdTags.Text = dr["ucoEngrvdTags"].ToString().Replace(",", ".").Replace(".00", "");
                        tCNCServices.Text = dr["ucoCNCServices"].ToString().Replace(",", ".").Replace(".00", "");
                        tGrinding.Text = dr["ucoGrinding"].ToString().Replace(",", ".").Replace(".00", "");
                        tShipping.Text = dr["ucoShipping"].ToString().Replace(",", ".").Replace(".00", "");
                        tThirdPartyCMM.Text = dr["ucoThirdPartyCMM"].ToString().Replace(",", ".").Replace(".00", "");
                        tWelding.Text = dr["ucoWelding"].ToString().Replace(",", ".").Replace(".00", "");
                        tWireBurn.Text = dr["ucoWireBurn"].ToString().Replace(",", ".").Replace(".00", "");
                        tGageRRCMM.Text = dr["ucoGageRRCMM"].ToString().Replace(",", ".").Replace(".00", "");
                        tGageRRFixtures.Text = dr["ucoGageRRFixtures"].ToString().Replace(",", ".").Replace(".00", "");
                        tRebates.Text = dr["ucoRebates"].ToString().Replace(",", ".").Replace(".00", "");
                        Total.Text = dr["ucoCost"].ToString().Replace(",", ".").Replace(".00", "");
                        litKeyUp.Text = "<script>keyup(" + Total.Text + ");</script>";
                    }
                    dr.Close();
                }

                sql.CommandText = "Select gnuGeneralNoteID from linkGeneralNoteToUGSQuote where gnuUGSQuoteID = @quote";
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
                    }
                }
                dr.Close();
            }
            connection.Close();
        }

        //protected void btnFinalize_Click(object sender, EventArgs e)
        //{
        //    Site master = new Site();
        //    SqlConnection connection = new SqlConnection(master.getConnectionString());
        //    connection.Open();
        //    SqlCommand sql = new SqlCommand();
        //    sql.Connection = connection;

        //    sql.CommandText = "update tblUGSQuote set uquFinalized = 1, uquModified = GETDATE(), uquModifiedBy = @user where uquUGSQuoteID = @quoteID";
        //    sql.Parameters.Clear();
        //    sql.Parameters.AddWithValue("@quoteID", quoteID);
        //    sql.Parameters.AddWithValue("@user", master.getUserName());
        //    master.ExecuteNonQuery(sql, "Edit Quote");


        //    connection.Close();
        //    populate_header();
        //}

        protected void btnSaveClick(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

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


            if (quoteID == "")
            {
                sql.CommandText = "INSERT INTO pktblUGSCost(ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ucoCNC, ";
                sql.CommandText += "ucoCertification, ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ";
                sql.CommandText += "ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ucoBasePlate, ucoAluminum, ";
                sql.CommandText += "ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ";
                sql.CommandText += "ucoIndStorCase, ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ";
                sql.CommandText += "ucoToolingBalls, ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ";
                sql.CommandText += "ucoBlackOxide, ucoHeatTreat, ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ";
                sql.CommandText += "ucoWelding, ucoWireBurn, ucoRebates, ucoCreated, ucoCreatedBy, ucoCost) ";
                sql.CommandText += "output inserted.ucoUGSCostID ";
                sql.CommandText += "VALUES(@Management, @ProjectEng, @ReadData, @3DModel, @Drawing, @Updates, @Programming, @CNC, @Certification, ";
                sql.CommandText += "@GageRRCMM, @PartLayouts, @Base, @Details, @LocationPins, @GoNoGoPins, @SPC, @GageRRFixtures, @Assemble, ";
                sql.CommandText += "@Pallets, @Transportation, @BasePlate, @Aluminum, @Steel, @FixturePlank, @Wood, @Bushings, @DrillBlanks, ";
                sql.CommandText += "@Clamps, @Indicator, @IndCollar, @IndStorCase, @ZeroSet, @SpcTriggers, @TempDrops, @HingeDrops, @Risers, ";
                sql.CommandText += "@Handles, @JigFeet, @ToolingBalls, @TBCovers, @TBPads, @Slides, @Magnets, @Hardware, @LMI, @Annodizing, ";
                sql.CommandText += "@BlackOxide, @HeatTreat, @EngrvdTags, @CNCServices, @Grinding, @Shipping, @ThirdPartyCMM, @Welding, ";
                sql.CommandText += "@WireBurn, @Rebates, GETDATE(), @CreatedBy, @Cost) ";

                sql.Parameters.AddWithValue("@Management", tManagement.Text);
                sql.Parameters.AddWithValue("@ProjectEng", tProjectEng.Text);
                sql.Parameters.AddWithValue("@ReadData", tReadData.Text);
                sql.Parameters.AddWithValue("@3DModel", t3DModel.Text);
                sql.Parameters.AddWithValue("@Drawing", tDrawing.Text);
                sql.Parameters.AddWithValue("@Updates", tUpdates.Text);
                sql.Parameters.AddWithValue("@Programming", tProgramming.Text);
                sql.Parameters.AddWithValue("@CNC", tCNC.Text);
                sql.Parameters.AddWithValue("@Certification", tCertification.Text);
                sql.Parameters.AddWithValue("@PartLayouts", tPartLayouts.Text);
                sql.Parameters.AddWithValue("@Base", tBase.Text);
                sql.Parameters.AddWithValue("@Details", tDetails.Text);
                sql.Parameters.AddWithValue("@LocationPins", tLocationPins.Text);
                sql.Parameters.AddWithValue("@GoNoGoPins", tGoNoGoPins.Text);
                sql.Parameters.AddWithValue("@SPC", tSPC.Text);
                sql.Parameters.AddWithValue("@Assemble", tAssemble.Text);
                sql.Parameters.AddWithValue("@Pallets", tPallets.Text);
                sql.Parameters.AddWithValue("@Transportation", tTransportation.Text);
                sql.Parameters.AddWithValue("@BasePlate", tBasePlate.Text);
                sql.Parameters.AddWithValue("@Aluminum", tAluminum.Text);
                sql.Parameters.AddWithValue("@Steel", tSteel.Text);
                sql.Parameters.AddWithValue("@FixturePlank", tFixturePlank.Text);
                sql.Parameters.AddWithValue("@Wood", tWood.Text);
                sql.Parameters.AddWithValue("@Bushings", tBushings.Text);
                sql.Parameters.AddWithValue("@DrillBlanks", tDrillBlanks.Text);
                sql.Parameters.AddWithValue("@Clamps", tClamps.Text);
                sql.Parameters.AddWithValue("@Indicator", tIndicator.Text);
                sql.Parameters.AddWithValue("@IndCollar", tIndCollar.Text);
                sql.Parameters.AddWithValue("@IndStorCase", tIndStorCase.Text);
                sql.Parameters.AddWithValue("@ZeroSet", tZeroSet.Text);
                sql.Parameters.AddWithValue("@SpcTriggers", tSpcTriggers.Text);
                sql.Parameters.AddWithValue("@TempDrops", tTempDrops.Text);
                sql.Parameters.AddWithValue("@HingeDrops", tHingeDrops.Text);
                sql.Parameters.AddWithValue("@Risers", tRisers.Text);
                sql.Parameters.AddWithValue("@Handles", tHandles.Text);
                sql.Parameters.AddWithValue("@JigFeet", tJigFeet.Text);
                sql.Parameters.AddWithValue("@ToolingBalls", tToolingBalls.Text);
                sql.Parameters.AddWithValue("@TBCovers", tTBCovers.Text);
                sql.Parameters.AddWithValue("@TBPads", tTBPads.Text);
                sql.Parameters.AddWithValue("@Slides", tSlides.Text);
                sql.Parameters.AddWithValue("@Magnets", tMagnets.Text);
                sql.Parameters.AddWithValue("@Hardware", tHardware.Text);
                sql.Parameters.AddWithValue("@LMI", tLMI.Text);
                sql.Parameters.AddWithValue("@Annodizing", tAnnodizing.Text);
                sql.Parameters.AddWithValue("@BlackOxide", tBlackOxide.Text);
                sql.Parameters.AddWithValue("@HeatTreat", tHeatTreat.Text);
                sql.Parameters.AddWithValue("@EngrvdTags", tEngrvdTags.Text);
                sql.Parameters.AddWithValue("@CNCServices", tCNCServices.Text);
                sql.Parameters.AddWithValue("@Grinding", tGrinding.Text);
                sql.Parameters.AddWithValue("@Shipping", tShipping.Text);
                sql.Parameters.AddWithValue("@ThirdPartyCMM", tThirdPartyCMM.Text);
                sql.Parameters.AddWithValue("@Welding", tWelding.Text);
                sql.Parameters.AddWithValue("@WireBurn", tWireBurn.Text);
                sql.Parameters.AddWithValue("@GageRRCMM", tGageRRCMM.Text);
                sql.Parameters.AddWithValue("@GageRRFixtures", tGageRRFixtures.Text);
                sql.Parameters.AddWithValue("@Rebates", tRebates.Text);
                if(Total.Text == "")
                {
                    sql.Parameters.AddWithValue("@Cost", 0);
                }
                else
                {
                    sql.Parameters.AddWithValue("@Cost", Total.Text.Replace("$", ""));
                }
                sql.Parameters.AddWithValue("@CreatedBy", master.getUserName());

                string cost = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();




                sql.CommandText = "INSERT INTO tblUGSQuote (uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, uquCustomerContact, ";
                sql.CommandText += "uquSalesmanID, uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquTotalPrice, uquUseTSG, uquNotes, ";
                sql.CommandText += "uquCreated, uquCreatedBy, uquShippingLocation, uquDieType, ";
                sql.CommandText += "uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC,  ";
                sql.CommandText += "uquCertification, uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins,  ";
                sql.CommandText += "uquSPC, uquGageRRFixtures, uquAssemble, uquPallets, uquTransportation, uquBasePlate, uquAluminum,  ";
                sql.CommandText += "uquSteel, uquFixturePlank, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar,  ";
                sql.CommandText += "uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles,  ";
                sql.CommandText += "uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI,  ";
                sql.CommandText += "uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping,  ";
                sql.CommandText += "uquThirdPartyCMM, uquWelding, uquWireBurn, uquRebates, uquUGSCostID, uquPartLength, uquPartWidth, uquPartHeight) ";
                sql.CommandText += "output inserted.uquUGSQuoteID ";
                sql.CommandText += "VALUES (@version, @status, @partNum, @partName, @rfq, @customer, @plant, @contact, @salesman, @custRFQNum, @estimator, ";
                sql.CommandText += "@shipping, @payment, @leadtime, @jobNum, @total, @logo, @notes, GETDATE(), @createdBy, @shippingLocation, @quoteType, ";
                sql.CommandText += "@management, @projectEng, @readData, @model, @drawing, @updates, @programming, @cnc, @certification, @gageRRCMM, @partLayouts, ";
                sql.CommandText += "@base, @details, @locationPins, @goNoGoPins, @spc, @gageRRFixtures, @assemble, @pallets, @transportation, @basePlate, ";
                sql.CommandText += "@aluminum, @steel, @fixturePlank, @wood, @bushings, @drillBlanks, @clamps, @indicator, @indCollar, @indStorCase, @zeroSet, ";
                sql.CommandText += "@spcTriggers, @tempDrops, @hingeDrops, @risers, @handles, @jigFeet, @toolingBalls, @tbCovers, @tbPads, @slides, @magnets, ";
                sql.CommandText += "@hardware, @LMI, @annodizing, @blackOxide, @heatTreat, @engrvdTags, @cncServices, @grinding, @shippingCalc, @thirdPartyCMM, ";
                sql.CommandText += "@welding, @wireBurn, @rebates, @ugsCostID, @partLength, @partWidth, @partHeight )";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@version", "001");
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@rfq", txtRFQNumber.Text);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@contact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custRFQNum", txtCustomerRFQ.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@leadtime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@total", txtTotalCost.Text);
                if(cbUseTSG.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                }
                sql.Parameters.AddWithValue("@notes", txtNotes.InnerText);
                string test = txtNotes.Value;
                string test2 = txtNotes.InnerText;
                string test3 = txtNotes.InnerHtml;
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@shippingLocation", txtShipping.Text);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);

                sql.Parameters.AddWithValue("@management", txtManagement.Text);
                sql.Parameters.AddWithValue("@projectEng", txtProjectEng.Text);
                sql.Parameters.AddWithValue("@readData", txtReadData.Text);
                sql.Parameters.AddWithValue("@model", txt3DModel.Text);
                sql.Parameters.AddWithValue("@drawing", txtDrawings.Text);
                sql.Parameters.AddWithValue("@updates", txtUpdates.Text);
                sql.Parameters.AddWithValue("@programming", txtProgramming.Text);
                sql.Parameters.AddWithValue("@cnc", txtCNC.Text);
                sql.Parameters.AddWithValue("@certification", txtCertification.Text);
                sql.Parameters.AddWithValue("@gageRRCMM", txtGageRR.Text);
                sql.Parameters.AddWithValue("@partLayouts", txtPartLayouts.Text);
                sql.Parameters.AddWithValue("@base", txtBase.Text);
                sql.Parameters.AddWithValue("@details", txtDetails.Text);
                sql.Parameters.AddWithValue("@locationPins", txtLocationPins.Text);
                sql.Parameters.AddWithValue("@goNoGoPins", txtGoNoGoPins.Text);
                sql.Parameters.AddWithValue("@spc", txtSPC.Text);
                sql.Parameters.AddWithValue("@gageRRFixtures", txtGageRRF.Text);
                sql.Parameters.AddWithValue("@assemble", txtAssemble.Text);
                sql.Parameters.AddWithValue("@pallets", txtPallets.Text);
                sql.Parameters.AddWithValue("@transportation", txtTransportation.Text);
                sql.Parameters.AddWithValue("@basePlate", txtBasePlate.Text);
                sql.Parameters.AddWithValue("@aluminum", txtAluminum.Text);
                sql.Parameters.AddWithValue("@steel", txtSteel.Text);
                sql.Parameters.AddWithValue("@fixturePlank", txtFixturePlank.Text);
                sql.Parameters.AddWithValue("@wood", txtWood.Text);
                sql.Parameters.AddWithValue("@bushings", txtBushings.Text);
                sql.Parameters.AddWithValue("@drillBlanks", txtDrillBlanks.Text);
                sql.Parameters.AddWithValue("@clamps", txtClamps.Text);
                sql.Parameters.AddWithValue("@indicator", txtIndicator.Text);
                sql.Parameters.AddWithValue("@indCollar", txtIndCollar.Text);
                sql.Parameters.AddWithValue("@indStorCase", txtIndStorCase.Text);
                sql.Parameters.AddWithValue("@zeroSet", txtZeroSet.Text);
                sql.Parameters.AddWithValue("@spcTriggers", txtSpcTriggers.Text);
                sql.Parameters.AddWithValue("@tempDrops", txtTempDrops.Text);
                sql.Parameters.AddWithValue("@hingeDrops", txtHingeDrops.Text);
                sql.Parameters.AddWithValue("@risers", txtRisers.Text);
                sql.Parameters.AddWithValue("@handles", txtHandles.Text);
                sql.Parameters.AddWithValue("@jigFeet", txtJigFeet.Text);
                sql.Parameters.AddWithValue("@toolingBalls", txtToolingBalls.Text);
                sql.Parameters.AddWithValue("@tBCovers", txtTBCovers.Text);
                sql.Parameters.AddWithValue("@tBPads", txtTBPads.Text);
                sql.Parameters.AddWithValue("@slides", txtSlides.Text);
                sql.Parameters.AddWithValue("@magnets", txtMagnets.Text);
                sql.Parameters.AddWithValue("@hardware", txtHardware.Text);
                sql.Parameters.AddWithValue("@lmi", txtLMI.Text);
                sql.Parameters.AddWithValue("@annodizing", txtAnnodizing.Text);
                sql.Parameters.AddWithValue("@blackOxide", txtBlackOxide.Text);
                sql.Parameters.AddWithValue("@heatTreat", txtHeatTreat.Text);
                sql.Parameters.AddWithValue("@engrvdTags", txtEngrvdTags.Text);
                sql.Parameters.AddWithValue("@cncServices", txtCNCServices.Text);
                sql.Parameters.AddWithValue("@grinding", txtGrinding.Text);
                sql.Parameters.AddWithValue("@shippingCalc", txtShippingCalc.Text);
                sql.Parameters.AddWithValue("@thirdPartyCMM", txtThirdPartyCMM.Text);
                sql.Parameters.AddWithValue("@welding", txtWelding.Text);
                sql.Parameters.AddWithValue("@wireBurn", txtWireBurn.Text);
                sql.Parameters.AddWithValue("@rebates", txtRebates.Text);
                sql.Parameters.AddWithValue("@ugsCostID", cost);
                sql.Parameters.AddWithValue("@partLength", txtLength.Text);
                sql.Parameters.AddWithValue("@partWidth", txtWidth.Text);
                sql.Parameters.AddWithValue("@partHeight", txtHeight.Text);


                quoteID = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

                String FileName = "";
                string pictureName = "";
                try
                {
                    FileName = filePicture.PostedFile.FileName;
                }
                catch
                {
                    
                }
                if(FileName == "")
                {
                    sql.CommandText = "Select prtPicture from tblPart where prtPARTID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    SqlDataReader dr2 = sql.ExecuteReader();
                    string lineNumber = "";
                    while (dr2.Read())
                    {
                        pictureName = dr2.GetValue(0).ToString();
                    }
                    dr2.Close();

                    //pictureName = "RFQ" + rfqID + "_" + lineNumber + "_" + txtPartNumber.Text.Trim() + ".png";
                }
                if (pictureName == "")
                {
                    pictureName = "UGS - " + quoteID + ".png";
                }


                sql.CommandText = "Update tblUGSQuote set uquQuoteNumber = @number, uquPicture = @picture where uquUGSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                sql.Parameters.AddWithValue("@number", quoteID);
                sql.Parameters.AddWithValue("@picture", pictureName);
                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                newPicture("UGS-" + quoteID + ".png");
                string user = master.getUserName();

                for (int i = 0; i < 200; i++)
                {
                    try
                    {
                        if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                        {
                            sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                            sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                            sql.CommandText += "Values (@company, @note, @cost, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@company", 15);
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + i].ToString());
                            sql.Parameters.AddWithValue("@cost", Request.Form["price" + i].ToString());
                            sql.Parameters.AddWithValue("@user", user);
                            string noteID = master.ExecuteScalar(sql, "UGS Quote").ToString();

                            sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                            sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@note", noteID);
                            sql.Parameters.AddWithValue("@quote", quoteID);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "UGS Quote");
                        }
                    }
                    catch
                    {

                    }
                }

                if(rfqID != 0 && partID != "")
                {
                    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                    sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @createdBy, 0, 0, 1)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "UGS Edit Quote");

                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 1)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }

                List<string> partIDs = new List<string>();
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail ";
                sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for(int i = 0; i < partIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 1)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    master.ExecuteNonQuery(sql, "UGS Edit Quote");
                }


                List<Label> generalNote = new List<Label>();
                generalNote.Add(lblGeneralNote1);
                generalNote.Add(lblGeneralNote2);
                generalNote.Add(lblGeneralNote3);
                generalNote.Add(lblGeneralNote4);
                generalNote.Add(lblGeneralNote5);
                //generalNote.Add(lblGeneralNote6);
                //generalNote.Add(lblGeneralNote7);
                //generalNote.Add(lblGeneralNote8);
                //generalNote.Add(lblGeneralNote9);


                List<CheckBox> cb = new List<CheckBox>();
                cb.Add(cbGeneralNote1);
                cb.Add(cbGeneralNote2);
                cb.Add(cbGeneralNote3);
                cb.Add(cbGeneralNote4);
                cb.Add(cbGeneralNote5);
                //cb.Add(cbGeneralNote6);
                //cb.Add(cbGeneralNote7);
                //cb.Add(cbGeneralNote8);
                //cb.Add(cbGeneralNote9);


                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }


                connection.Close();
                Response.Redirect("https://tsgrfq.azurewebsites.net/UGSEditQuote?id=" + quoteID);
            }
            else
            {
                sql.CommandText = "update tblUGSQuote set uquStatusID = @status, uquPartNumber = @partNum, uquPartName = @partName, uquRFQID = @rfq, uquCustomerID = @customer, uquPlantID = @plant, uquCustomerContact = @contact, ";
                sql.CommandText += "uquSalesmanID = @salesman, uquCustomerRFQNumber = @custRFQNum, uquEstimatorID = @estimator, uquShippingID = @shipping, uquPaymentID = @payment, uquLeadTime = @leadtime, uquJobNumber = @jobNum, ";
                sql.CommandText += "uquManagement = @management, uquProjectEng = @projectEng, uquReadData = @readData, uqu3DModel = @model, ";
                sql.CommandText += "uquDrawing = @drawing, uquUpdates = @updates, uquPrograming = @programming, uquCNC = @cnc,  ";
                sql.CommandText += "uquCertification = @certification, uquGageRRCMM = @gageRRCMM, uquPartLayouts = @partLayouts, ";
                sql.CommandText += "uquBase = @base, uquDetails = @details, uquLocationPins = @locationPins, uquGoNoGoPins = @goNoGoPins,  ";
                sql.CommandText += "uquSPC = @spc, uquGageRRFixtures = @gageRRFixtures, uquAssemble = @assemble, uquPallets = @pallets, ";
                sql.CommandText += "uquTransportation = @transportation, uquBasePlate = @basePlate, uquAluminum = @aluminum,  ";
                sql.CommandText += "uquSteel = @steel, uquFixturePlank = @fixturePlank, uquWood = @wood, uquBushings = @bushings, ";
                sql.CommandText += "uquDrillBlanks = @drillBlanks, uquClamps = @clamps, uquIndicator = @indicator, uquIndCollar = @indCollar,  ";
                sql.CommandText += "uquIndStorCase = @indStorCase, uquZeroSet = @zeroSet, uquSpcTriggers = @spcTriggers, ";
                sql.CommandText += "uquTempDrops = @tempDrops, uquHingeDrops = @hingeDrops, uquRisers = @risers, uquHandles = @handles,  ";
                sql.CommandText += "uquJigFeet = @jigFeet, uquToolingBalls = @toolingBalls, uquTBCovers = @tBCovers, uquTBPads = @tBPads, ";
                sql.CommandText += "uquSlides = @slides, uquMagnets = @magnets, uquHardware = @hardware, uquLMI = @lmi,  ";
                sql.CommandText += "uquAnnodizing = @annodizing, uquBlackOxide = @blackOxide, uquHeatTreat = @heatTreat, uquEngrvdTags = @engrvdTags, ";
                sql.CommandText += "uquCNCServices = @cncServices, uquGrinding = @grinding, uquShipping = @shippingCalc,  ";
                sql.CommandText += "uquThirdPartyCMM = @thirdPartyCMM, uquWelding = @welding, uquWireBurn = @wireBurn, uquRebates = @rebates, ";
                sql.CommandText += "uquTotalPrice = @total, uquUseTSG = @logo, uquNotes = @notes, uquModified = GETDATE(), uquModifiedBy = @modified, uquShippingLocation = @shippingLocation, ";
                sql.CommandText += "uquDieType = @quoteType, uquPartLength = @partLength, uquPartWidth = @partWidth, uquPartHeight = @partHeight where uquUGSQuoteID = @quoteID ";
                sql.Parameters.Clear();

                sql.Parameters.AddWithValue("@quoteID", quoteID);

                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
                sql.Parameters.AddWithValue("@partName", txtPartName.Text);
                sql.Parameters.AddWithValue("@rfq", txtRFQNumber.Text);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@contact", txtCustomerContact.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custRFQNum", txtCustomerRFQ.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@leadtime", txtLeadTime.Text);
                sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
                sql.Parameters.AddWithValue("@total", txtTotalCost.Text);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
                if (cbUseTSG.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                }
                sql.Parameters.AddWithValue("@notes", txtNotes.InnerText);
                sql.Parameters.AddWithValue("@modified", master.getUserName());
                sql.Parameters.AddWithValue("@shippingLocation", txtShipping.Text);
                sql.Parameters.AddWithValue("@management", txtManagement.Text);
                sql.Parameters.AddWithValue("@projectEng", txtProjectEng.Text);
                sql.Parameters.AddWithValue("@readData", txtReadData.Text);
                sql.Parameters.AddWithValue("@model", txt3DModel.Text);
                sql.Parameters.AddWithValue("@drawing", txtDrawings.Text);
                sql.Parameters.AddWithValue("@updates", txtUpdates.Text);
                sql.Parameters.AddWithValue("@programming", txtProgramming.Text);
                sql.Parameters.AddWithValue("@cnc", txtCNC.Text);
                sql.Parameters.AddWithValue("@certification", txtCertification.Text);
                sql.Parameters.AddWithValue("@gageRRCMM", txtGageRR.Text);
                sql.Parameters.AddWithValue("@partLayouts", txtPartLayouts.Text);
                sql.Parameters.AddWithValue("@base", txtBase.Text);
                sql.Parameters.AddWithValue("@details", txtDetails.Text);
                sql.Parameters.AddWithValue("@locationPins", txtLocationPins.Text);
                sql.Parameters.AddWithValue("@goNoGoPins", txtGoNoGoPins.Text);
                sql.Parameters.AddWithValue("@spc", txtSPC.Text);
                sql.Parameters.AddWithValue("@gageRRFixtures", txtGageRRF.Text);
                sql.Parameters.AddWithValue("@assemble", txtAssemble.Text);
                sql.Parameters.AddWithValue("@pallets", txtPallets.Text);
                sql.Parameters.AddWithValue("@transportation", txtTransportation.Text);
                sql.Parameters.AddWithValue("@basePlate", txtBasePlate.Text);
                sql.Parameters.AddWithValue("@aluminum", txtAluminum.Text);
                sql.Parameters.AddWithValue("@steel", txtSteel.Text);
                sql.Parameters.AddWithValue("@fixturePlank", txtFixturePlank.Text);
                sql.Parameters.AddWithValue("@wood", txtWood.Text);
                sql.Parameters.AddWithValue("@bushings", txtBushings.Text);
                sql.Parameters.AddWithValue("@drillBlanks", txtDrillBlanks.Text);
                sql.Parameters.AddWithValue("@clamps", txtClamps.Text);
                sql.Parameters.AddWithValue("@indicator", txtIndicator.Text);
                sql.Parameters.AddWithValue("@indCollar", txtIndCollar.Text);
                sql.Parameters.AddWithValue("@indStorCase", txtIndStorCase.Text);
                sql.Parameters.AddWithValue("@zeroSet", txtZeroSet.Text);
                sql.Parameters.AddWithValue("@spcTriggers", txtSpcTriggers.Text);
                sql.Parameters.AddWithValue("@tempDrops", txtTempDrops.Text);
                sql.Parameters.AddWithValue("@hingeDrops", txtHingeDrops.Text);
                sql.Parameters.AddWithValue("@risers", txtRisers.Text);
                sql.Parameters.AddWithValue("@handles", txtHandles.Text);
                sql.Parameters.AddWithValue("@jigFeet", txtJigFeet.Text);
                sql.Parameters.AddWithValue("@toolingBalls", txtToolingBalls.Text);
                sql.Parameters.AddWithValue("@tBCovers", txtTBCovers.Text);
                sql.Parameters.AddWithValue("@tBPads", txtTBPads.Text);
                sql.Parameters.AddWithValue("@slides", txtSlides.Text);
                sql.Parameters.AddWithValue("@magnets", txtMagnets.Text);
                sql.Parameters.AddWithValue("@hardware", txtHardware.Text);
                sql.Parameters.AddWithValue("@lmi", txtLMI.Text);
                sql.Parameters.AddWithValue("@annodizing", txtAnnodizing.Text);
                sql.Parameters.AddWithValue("@blackOxide", txtBlackOxide.Text);
                sql.Parameters.AddWithValue("@heatTreat", txtHeatTreat.Text);
                sql.Parameters.AddWithValue("@engrvdTags", txtEngrvdTags.Text);
                sql.Parameters.AddWithValue("@cncServices", txtCNCServices.Text);
                sql.Parameters.AddWithValue("@grinding", txtGrinding.Text);
                sql.Parameters.AddWithValue("@shippingCalc", txtShippingCalc.Text);
                sql.Parameters.AddWithValue("@thirdPartyCMM", txtThirdPartyCMM.Text);
                sql.Parameters.AddWithValue("@welding", txtWelding.Text);
                sql.Parameters.AddWithValue("@wireBurn", txtWireBurn.Text);
                sql.Parameters.AddWithValue("@rebates", txtRebates.Text);
                sql.Parameters.AddWithValue("@partLength", txtLength.Text);
                sql.Parameters.AddWithValue("@partWidth", txtWidth.Text);
                sql.Parameters.AddWithValue("@partHeight", txtHeight.Text);

                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                newPicture("UGS-" + quoteID + ".png");

                sql.CommandText = "Delete from linkGeneralNoteToUGSQuote where gnuUGSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "UGS Edit Quote");


                List<Label> generalNote = new List<Label>();
                generalNote.Add(lblGeneralNote1);
                generalNote.Add(lblGeneralNote2);
                generalNote.Add(lblGeneralNote3);
                generalNote.Add(lblGeneralNote4);
                generalNote.Add(lblGeneralNote5);
                //generalNote.Add(lblGeneralNote6);
                //generalNote.Add(lblGeneralNote7);
                //generalNote.Add(lblGeneralNote8);
                //generalNote.Add(lblGeneralNote9);


                List<CheckBox> cb = new List<CheckBox>();
                cb.Add(cbGeneralNote1);
                cb.Add(cbGeneralNote2);
                cb.Add(cbGeneralNote3);
                cb.Add(cbGeneralNote4);
                cb.Add(cbGeneralNote5);
                //cb.Add(cbGeneralNote6);
                //cb.Add(cbGeneralNote7);
                //cb.Add(cbGeneralNote8);
                //cb.Add(cbGeneralNote9);

                List<string> pwnID = new List<string>();
                sql.CommandText = "Select pwnPreWordedNoteID from pktblPreWordedNote, linkPWNToUGSQuote where puqPreWordedNoteID = pwnPreWordedNoteID and puqUGSQuoteID = @id order by pwnPreWordedNoteID ASC ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    pwnID.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "delete from linkPWNToUGSQuote where puqUGSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                for (int i = 0; i < pwnID.Count; i++)
                {
                    sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", pwnID[i]);
                    master.ExecuteNonQuery(sql, "UGS Edit Quote");
                }


                string user = master.getUserName();
                for (int i = 0; i < 200; i++)
                {
                    try
                    {
                        if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                        {
                            sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                            sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                            sql.CommandText += "Values (@company, @note, @cost, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@company", 15);
                            sql.Parameters.AddWithValue("@note", Request.Form["notes" + i].ToString());
                            sql.Parameters.AddWithValue("@cost", Request.Form["price" + i].ToString());
                            sql.Parameters.AddWithValue("@user", user);
                            string noteID = master.ExecuteScalar(sql, "UGS Quote").ToString();

                            sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                            sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@note", noteID);
                            sql.Parameters.AddWithValue("@quote", quoteID);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "UGS Quote");
                        }
                    }
                    catch
                    {

                    }
                }


                for (int i = 0; i < cb.Count; i++)
                {
                    if (cb[i].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
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

        protected void btncreateNewVersionClick(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

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

            List<string> partID = new List<string>();
            string rfqID = "";
            string picture = "";
            sql.CommandText = "Select ptqPartID, qtrRFQID, uquPicture from linkPartToQuote, linkQuoteToRFQ, tblUGSQuote where ptqQuoteID = @quoteID and ptqUGS = 1 and ptqUGS = qtrUGS and ptqQuoteID = qtrQuoteID and qtrQuoteID = uquUGSQuoteID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            dr = sql.ExecuteReader();
            while(dr.Read())
            {
                partID.Add(dr.GetValue(0).ToString());
                rfqID = dr.GetValue(1).ToString();
                if (picture == "")
                {
                    picture = dr.GetValue(2).ToString();
                }
            }
            dr.Close();


            sql.CommandText = "INSERT INTO pktblUGSCost(ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ucoCNC, ";
            sql.CommandText += "ucoCertification, ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ";
            sql.CommandText += "ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ucoBasePlate, ucoAluminum, ";
            sql.CommandText += "ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ";
            sql.CommandText += "ucoIndStorCase, ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ";
            sql.CommandText += "ucoToolingBalls, ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ";
            sql.CommandText += "ucoBlackOxide, ucoHeatTreat, ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ";
            sql.CommandText += "ucoWelding, ucoWireBurn, ucoRebates, ucoCreated, ucoCreatedBy, ucoCost) ";
            sql.CommandText += "output inserted.ucoUGSCostID ";
            sql.CommandText += "VALUES(@Management, @ProjectEng, @ReadData, @3DModel, @Drawing, @Updates, @Programming, @CNC, @Certification, ";
            sql.CommandText += "@GageRRCMM, @PartLayouts, @Base, @Details, @LocationPins, @GoNoGoPins, @SPC, @GageRRFixtures, @Assemble, ";
            sql.CommandText += "@Pallets, @Transportation, @BasePlate, @Aluminum, @Steel, @FixturePlank, @Wood, @Bushings, @DrillBlanks, ";
            sql.CommandText += "@Clamps, @Indicator, @IndCollar, @IndStorCase, @ZeroSet, @SpcTriggers, @TempDrops, @HingeDrops, @Risers, ";
            sql.CommandText += "@Handles, @JigFeet, @ToolingBalls, @TBCovers, @TBPads, @Slides, @Magnets, @Hardware, @LMI, @Annodizing, ";
            sql.CommandText += "@BlackOxide, @HeatTreat, @EngrvdTags, @CNCServices, @Grinding, @Shipping, @ThirdPartyCMM, @Welding, ";
            sql.CommandText += "@WireBurn, @Rebates, GETDATE(), @CreatedBy, @cost) ";
            sql.Parameters.Clear();

            sql.Parameters.AddWithValue("@Management", tManagement.Text);
            sql.Parameters.AddWithValue("@ProjectEng", tProjectEng.Text);
            sql.Parameters.AddWithValue("@ReadData", tReadData.Text);
            sql.Parameters.AddWithValue("@3DModel", t3DModel.Text);
            sql.Parameters.AddWithValue("@Drawing", tDrawing.Text);
            sql.Parameters.AddWithValue("@Updates", tUpdates.Text);
            sql.Parameters.AddWithValue("@Programming", tProgramming.Text);
            sql.Parameters.AddWithValue("@CNC", tCNC.Text);
            sql.Parameters.AddWithValue("@Certification", tCertification.Text);
            sql.Parameters.AddWithValue("@PartLayouts", tPartLayouts.Text);
            sql.Parameters.AddWithValue("@Base", tBase.Text);
            sql.Parameters.AddWithValue("@Details", tDetails.Text);
            sql.Parameters.AddWithValue("@LocationPins", tLocationPins.Text);
            sql.Parameters.AddWithValue("@GoNoGoPins", tGoNoGoPins.Text);
            sql.Parameters.AddWithValue("@SPC", tSPC.Text);
            sql.Parameters.AddWithValue("@Assemble", tAssemble.Text);
            sql.Parameters.AddWithValue("@Pallets", tPallets.Text);
            sql.Parameters.AddWithValue("@Transportation", tTransportation.Text);
            sql.Parameters.AddWithValue("@BasePlate", tBasePlate.Text);
            sql.Parameters.AddWithValue("@Aluminum", tAluminum.Text);
            sql.Parameters.AddWithValue("@Steel", tSteel.Text);
            sql.Parameters.AddWithValue("@FixturePlank", tFixturePlank.Text);
            sql.Parameters.AddWithValue("@Wood", tWood.Text);
            sql.Parameters.AddWithValue("@Bushings", tBushings.Text);
            sql.Parameters.AddWithValue("@DrillBlanks", tDrillBlanks.Text);
            sql.Parameters.AddWithValue("@Clamps", tClamps.Text);
            sql.Parameters.AddWithValue("@Indicator", tIndicator.Text);
            sql.Parameters.AddWithValue("@IndCollar", tIndCollar.Text);
            sql.Parameters.AddWithValue("@IndStorCase", tIndStorCase.Text);
            sql.Parameters.AddWithValue("@ZeroSet", tZeroSet.Text);
            sql.Parameters.AddWithValue("@SpcTriggers", tSpcTriggers.Text);
            sql.Parameters.AddWithValue("@TempDrops", tTempDrops.Text);
            sql.Parameters.AddWithValue("@HingeDrops", tHingeDrops.Text);
            sql.Parameters.AddWithValue("@Risers", tRisers.Text);
            sql.Parameters.AddWithValue("@Handles", tHandles.Text);
            sql.Parameters.AddWithValue("@JigFeet", tJigFeet.Text);
            sql.Parameters.AddWithValue("@ToolingBalls", tToolingBalls.Text);
            sql.Parameters.AddWithValue("@TBCovers", tTBCovers.Text);
            sql.Parameters.AddWithValue("@TBPads", tTBPads.Text);
            sql.Parameters.AddWithValue("@Slides", tSlides.Text);
            sql.Parameters.AddWithValue("@Magnets", tMagnets.Text);
            sql.Parameters.AddWithValue("@Hardware", tHardware.Text);
            sql.Parameters.AddWithValue("@LMI", tLMI.Text);
            sql.Parameters.AddWithValue("@Annodizing", tAnnodizing.Text);
            sql.Parameters.AddWithValue("@BlackOxide", tBlackOxide.Text);
            sql.Parameters.AddWithValue("@HeatTreat", tHeatTreat.Text);
            sql.Parameters.AddWithValue("@EngrvdTags", tEngrvdTags.Text);
            sql.Parameters.AddWithValue("@CNCServices", tCNCServices.Text);
            sql.Parameters.AddWithValue("@Grinding", tGrinding.Text);
            sql.Parameters.AddWithValue("@Shipping", tShipping.Text);
            sql.Parameters.AddWithValue("@ThirdPartyCMM", tThirdPartyCMM.Text);
            sql.Parameters.AddWithValue("@Welding", tWelding.Text);
            sql.Parameters.AddWithValue("@WireBurn", tWireBurn.Text);
            sql.Parameters.AddWithValue("@GageRRCMM", tGageRRCMM.Text);
            sql.Parameters.AddWithValue("@GageRRFixtures", tGageRRFixtures.Text);
            sql.Parameters.AddWithValue("@Rebates", tRebates.Text);
            if (Total.Text == "")
            {
                sql.Parameters.AddWithValue("@cost", 0);
            }
            else
            {
                sql.Parameters.AddWithValue("@cost", Total.Text.Replace("$", ""));
            }
            sql.Parameters.AddWithValue("@CreatedBy", master.getUserName());

            string cost = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

            sql.CommandText = "INSERT INTO tblUGSQuote (uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, uquCustomerContact, ";
            sql.CommandText += "uquSalesmanID, uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquTotalPrice, uquUseTSG, uquNotes, ";
            sql.CommandText += "uquCreated, uquCreatedBy, uquShippingLocation, uquDieType, uquUGSCostID, uquPartLength, uquPartWidth, uquPartHeight, "; 
            sql.CommandText += "uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC,  ";
            sql.CommandText += "uquCertification, uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins,  ";
            sql.CommandText += "uquSPC, uquGageRRFixtures, uquAssemble, uquPallets, uquTransportation, uquBasePlate, uquAluminum,  ";
            sql.CommandText += "uquSteel, uquFixturePlank, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar,  ";
            sql.CommandText += "uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles,  ";
            sql.CommandText += "uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI,  ";
            sql.CommandText += "uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping,  ";
            sql.CommandText += "uquThirdPartyCMM, uquWelding, uquWireBurn, uquRebates, uquPicture) ";
            sql.CommandText += "output inserted.uquUGSQuoteID ";
            sql.CommandText += "VALUES (@version, @status, @partNum, @partName, @rfq, @customer, @plant, @contact, @salesman, @custRFQNum, @estimator, ";
            sql.CommandText += "@shipping, @payment, @leadtime, @jobNum, @total, @logo, @notes, GETDATE(), @createdBy, @shippingLocation, @quoteType, @cost, ";
            sql.CommandText += "@length, @width, @height, ";
            sql.CommandText += "@management, @projectEng, @readData, @model, @drawing, @updates, @programming, @cnc, @certification, @gageRRCMM, @partLayouts, ";
            sql.CommandText += "@base, @details, @locationPins, @goNoGoPins, @spc, @gageRRFixtures, @assemble, @pallets, @transportation, @basePlate, ";
            sql.CommandText += "@aluminum, @steel, @fixturePlank, @wood, @bushings, @drillBlanks, @clamps, @indicator, @indCollar, @indStorCase, @zeroSet, ";
            sql.CommandText += "@spcTriggers, @tempDrops, @hingeDrops, @risers, @handles, @jigFeet, @toolingBalls, @tbCovers, @tbPads, @slides, @magnets, ";
            sql.CommandText += "@hardware, @LMI, @annodizing, @blackOxide, @heatTreat, @engrvdTags, @cncServices, @grinding, @shippingCalc, @thirdPartyCMM, ";
            sql.CommandText += "@welding, @wireBurn, @rebates, @picture)";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(lblVersion.Text) + 1).ToString("000"));
            sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
            sql.Parameters.AddWithValue("@partNum", txtPartNumber.Text);
            sql.Parameters.AddWithValue("@partName", txtPartName.Text);
            sql.Parameters.AddWithValue("@rfq", txtRFQNumber.Text);
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
            sql.Parameters.AddWithValue("@contact", txtCustomerContact.Text);
            sql.Parameters.AddWithValue("@salesman", salesman);
            sql.Parameters.AddWithValue("@custRFQNum", txtCustomerRFQ.Text);
            sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
            sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
            sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
            sql.Parameters.AddWithValue("@leadtime", txtLeadTime.Text);
            sql.Parameters.AddWithValue("@jobNum", txtJobNumber.Text);
            sql.Parameters.AddWithValue("@total", txtTotalCost.Text);
            sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);
            if (cbUseTSG.Checked)
            {
                sql.Parameters.AddWithValue("@logo", 1);
            }
            else
            {
                sql.Parameters.AddWithValue("@logo", 0);
            }
            sql.Parameters.AddWithValue("@notes", txtNotes.InnerText);
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
            sql.Parameters.AddWithValue("@shippingLocation", txtShipping.Text);
            sql.Parameters.AddWithValue("@cost", cost);
            sql.Parameters.AddWithValue("@length", txtLength.Text);
            sql.Parameters.AddWithValue("@width", txtWidth.Text);
            sql.Parameters.AddWithValue("@height", txtHeight.Text);
            sql.Parameters.AddWithValue("@management", txtManagement.Text);
            sql.Parameters.AddWithValue("@projectEng", txtProjectEng.Text);
            sql.Parameters.AddWithValue("@readData", txtReadData.Text);
            sql.Parameters.AddWithValue("@model", txt3DModel.Text);
            sql.Parameters.AddWithValue("@drawing", txtDrawings.Text);
            sql.Parameters.AddWithValue("@updates", txtUpdates.Text);
            sql.Parameters.AddWithValue("@programming", txtProgramming.Text);
            sql.Parameters.AddWithValue("@cnc", txtCNC.Text);
            sql.Parameters.AddWithValue("@certification", txtCertification.Text);
            sql.Parameters.AddWithValue("@gageRRCMM", txtGageRR.Text);
            sql.Parameters.AddWithValue("@partLayouts", txtPartLayouts.Text);
            sql.Parameters.AddWithValue("@base", txtBase.Text);
            sql.Parameters.AddWithValue("@details", txtDetails.Text);
            sql.Parameters.AddWithValue("@locationPins", txtLocationPins.Text);
            sql.Parameters.AddWithValue("@goNoGoPins", txtGoNoGoPins.Text);
            sql.Parameters.AddWithValue("@spc", txtSPC.Text);
            sql.Parameters.AddWithValue("@gageRRFixtures", txtGageRRF.Text);
            sql.Parameters.AddWithValue("@assemble", txtAssemble.Text);
            sql.Parameters.AddWithValue("@pallets", txtPallets.Text);
            sql.Parameters.AddWithValue("@transportation", txtTransportation.Text);
            sql.Parameters.AddWithValue("@basePlate", txtBasePlate.Text);
            sql.Parameters.AddWithValue("@aluminum", txtAluminum.Text);
            sql.Parameters.AddWithValue("@steel", txtSteel.Text);
            sql.Parameters.AddWithValue("@fixturePlank", txtFixturePlank.Text);
            sql.Parameters.AddWithValue("@wood", txtWood.Text);
            sql.Parameters.AddWithValue("@bushings", txtBushings.Text);
            sql.Parameters.AddWithValue("@drillBlanks", txtDrillBlanks.Text);
            sql.Parameters.AddWithValue("@clamps", txtClamps.Text);
            sql.Parameters.AddWithValue("@indicator", txtIndicator.Text);
            sql.Parameters.AddWithValue("@indCollar", txtIndCollar.Text);
            sql.Parameters.AddWithValue("@indStorCase", txtIndStorCase.Text);
            sql.Parameters.AddWithValue("@zeroSet", txtZeroSet.Text);
            sql.Parameters.AddWithValue("@spcTriggers", txtSpcTriggers.Text);
            sql.Parameters.AddWithValue("@tempDrops", txtTempDrops.Text);
            sql.Parameters.AddWithValue("@hingeDrops", txtHingeDrops.Text);
            sql.Parameters.AddWithValue("@risers", txtRisers.Text);
            sql.Parameters.AddWithValue("@handles", txtHandles.Text);
            sql.Parameters.AddWithValue("@jigFeet", txtJigFeet.Text);
            sql.Parameters.AddWithValue("@toolingBalls", txtToolingBalls.Text);
            sql.Parameters.AddWithValue("@tBCovers", txtTBCovers.Text);
            sql.Parameters.AddWithValue("@tBPads", txtTBPads.Text);
            sql.Parameters.AddWithValue("@slides", txtSlides.Text);
            sql.Parameters.AddWithValue("@magnets", txtMagnets.Text);
            sql.Parameters.AddWithValue("@hardware", txtHardware.Text);
            sql.Parameters.AddWithValue("@lmi", txtLMI.Text);
            sql.Parameters.AddWithValue("@annodizing", txtAnnodizing.Text);
            sql.Parameters.AddWithValue("@blackOxide", txtBlackOxide.Text);
            sql.Parameters.AddWithValue("@heatTreat", txtHeatTreat.Text);
            sql.Parameters.AddWithValue("@engrvdTags", txtEngrvdTags.Text);
            sql.Parameters.AddWithValue("@cncServices", txtCNCServices.Text);
            sql.Parameters.AddWithValue("@grinding", txtGrinding.Text);
            sql.Parameters.AddWithValue("@shippingCalc", txtShippingCalc.Text);
            sql.Parameters.AddWithValue("@thirdPartyCMM", txtThirdPartyCMM.Text);
            sql.Parameters.AddWithValue("@welding", txtWelding.Text);
            sql.Parameters.AddWithValue("@wireBurn", txtWireBurn.Text);
            sql.Parameters.AddWithValue("@rebates", txtRebates.Text);
            sql.Parameters.AddWithValue("@picture", picture);

            quoteID = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

            String user = master.getUserName();

            for (int i = 0; i < 200; i++)
            {
                try
                {
                    if (Request.Form["notes" + i].ToString() != "" || Request.Form["price" + i].ToString() != "")
                    {
                        sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                        sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                        sql.CommandText += "Values (@company, @note, @cost, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", 15);
                        sql.Parameters.AddWithValue("@note", Request.Form["notes" + i].ToString());
                        sql.Parameters.AddWithValue("@cost", Request.Form["price" + i].ToString());
                        sql.Parameters.AddWithValue("@user", user);
                        string noteID = master.ExecuteScalar(sql, "UGS Quote").ToString();

                        sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                        sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@note", noteID);
                        sql.Parameters.AddWithValue("@quote", quoteID);
                        sql.Parameters.AddWithValue("@user", user);
                        master.ExecuteNonQuery(sql, "UGS Quote");
                    }
                }
                catch
                {

                }
            }


            sql.CommandText = "Update tblUGSQuote set uquQuoteNumber = @number where uquUGSQuoteID = @id";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", quoteID);
            sql.Parameters.AddWithValue("@number", quoteID);
            //sql.Parameters.AddWithValue("@picture", "UGS-" + quoteID + ".png");
            master.ExecuteNonQuery(sql, "UGS Edit Quote");

            List<Label> generalNote = new List<Label>();
            generalNote.Add(lblGeneralNote1);
            generalNote.Add(lblGeneralNote2);
            generalNote.Add(lblGeneralNote3);
            generalNote.Add(lblGeneralNote4);
            generalNote.Add(lblGeneralNote5);
            //generalNote.Add(lblGeneralNote6);
            //generalNote.Add(lblGeneralNote7);
            //generalNote.Add(lblGeneralNote8);
            //generalNote.Add(lblGeneralNote9);


            List<CheckBox> cb = new List<CheckBox>();
            cb.Add(cbGeneralNote1);
            cb.Add(cbGeneralNote2);
            cb.Add(cbGeneralNote3);
            cb.Add(cbGeneralNote4);
            cb.Add(cbGeneralNote5);
            //cb.Add(cbGeneralNote6);
            //cb.Add(cbGeneralNote7);
            //cb.Add(cbGeneralNote8);
            //cb.Add(cbGeneralNote9);


            for (int i = 0; i < cb.Count; i++)
            {
                if (cb[i].Checked)
                {
                    sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                    sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@noteID", generalNote[i].Text.Split('-')[0]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    master.ExecuteNonQuery(sql, "HTSEditQuote");
                }
            }

            if (rfqID != "" && partID.Count != 0)
            {
                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @createdBy, 0, 0, 1)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                for (int i = 0; i < partID.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 1)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID[i]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                }

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
            }


            newPicture("UGS-" + quoteID + ".png");



            connection.Close();
            Response.Redirect("https://tsgrfq.azurewebsites.net/UGSEditQuote?id=" + quoteID);
            //Response.Redirect("http://localhost:52154/UGSEditQuote?id=" + quoteID);
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
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "Update tblUGSQuote set uquPicture = @picture where uquUGSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                sql.Parameters.AddWithValue("@number", quoteID);
                sql.Parameters.AddWithValue("@picture", "UGS-" + quoteID + ".png");
                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/");
                ctx.Credentials = master.getSharePointCredentials();
                Web web = ctx.Web;
                ctx.Load(web);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("UGS Quote Pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = newStream;
                newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/UGS Pictures/" + pictureName;
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
    }
}