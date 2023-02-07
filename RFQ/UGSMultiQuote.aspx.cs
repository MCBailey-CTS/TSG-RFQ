using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class UGSMultiQuote : System.Web.UI.Page
    {
        int count = 0;
        int rfqID = 0;
        List<string> partID = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            SqlDataReader dr;

            
            try
            {
                rfqID = System.Convert.ToInt32(Request["rfqID"]);
            }
            catch
            {

            }

            if (!IsPostBack)
            {
                sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer, tblRFQ where rfqID = @rfqID and rfqCustomerID = CustomerID order by CustomerName ";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                SqlDataReader CustomerDR = sql.ExecuteReader();
                ddlCustomer.DataSource = CustomerDR;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Enabled = false;
                CustomerDR.Close();

                sql.CommandText = "Select concat(ShipToName, ' (', ShipCode, ')') as name, CustomerLocationID from CustomerLocation where CustomerID = @customerID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customerID", ddlCustomer.SelectedValue);
                dr = sql.ExecuteReader();
                ddlPlant.DataSource = dr;
                ddlPlant.DataTextField = "name";
                ddlPlant.DataValueField = "CustomerLocationID";
                ddlPlant.DataBind();
                dr.Close();
                sql.Parameters.Clear();

                sql.CommandText = "Select steShippingTerms, steShippingTermsID from pktblShippingTerms";
                dr = sql.ExecuteReader();
                ddlShipping.DataSource = dr;
                ddlShipping.DataTextField = "steShippingTerms";
                ddlShipping.DataValueField = "steShippingTermsID";
                ddlShipping.DataBind();
                dr.Close();

                sql.CommandText = "Select ptePaymentTerms, ptePaymentTermsID from pktblPaymentTerms";
                dr = sql.ExecuteReader();
                ddlPayment.DataSource = dr;
                ddlPayment.DataTextField = "ptePaymentTerms";
                ddlPayment.DataValueField = "ptePaymentTermsID";
                ddlPayment.DataBind();
                dr.Close();

                sql.CommandText = "Select dtyFullName, DieTypeID from DieType where TSGCompanyID = 15";
                dr = sql.ExecuteReader();
                ddlQuoteType.DataSource = dr;
                ddlQuoteType.DataTextField = "dtyFullName";
                ddlQuoteType.DataValueField = "DieTypeID";
                ddlQuoteType.DataBind();
                dr.Close();

                sql.CommandText = "Select concat(estFirstName, ' ', estLastName) as name, estEstimatorID from pktblEstimators where estCompanyID = 15";
                dr = sql.ExecuteReader();
                ddlEstimator.DataSource = dr;
                ddlEstimator.DataTextField = "name";
                ddlEstimator.DataValueField = "estEstimatorID";
                ddlEstimator.DataBind();
                dr.Close();

                sql.CommandText = "Select Name from tblRFQ, TSGSalesman where rfqID = @rfqID and rfqSalesman = TSGSalesmanID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    lblSalesman.Text = dr.GetValue(0).ToString();
                }
                dr.Close();


                txtNotes.Value = "QUOTE DESCRIPTION\n";
                txtNotes.Value += "Design / Build / Certify(1) attribute check fixture for checking of the above part.\n ";

                txtNotes.Value += "GENERAL CONTENT\n";
                txtNotes.Value += "•	Read / Log In latest math data\n";
                txtNotes.Value += "•	3 - D SolidWorks fixture design. (approval required)\n";
                txtNotes.Value += "•	Parts to be held on fixture in car position\n";
                txtNotes.Value += "•	Parts to be held on fixture 90 deg to car position\n";
                txtNotes.Value += "•	Parts to be held on fixture 180 deg to car position\n";
                txtNotes.Value += "•	1.00\" aluminum plate base with jig feet, (3) tool balls and handles\n";
                txtNotes.Value += "•	Welded rib aluminum base with lift rings, (3) tool balls and body lines\n";
                txtNotes.Value += "•	Datum scheme(X) - A - net, RFS / MMC - B - &RFS / MMC - C -\n";
                txtNotes.Value += "•	Datum scheme(X) - A - net, MMC - B - &MMC - C -\n";
                txtNotes.Value += "•	Aluminum construction with steel nets, locators and location pins\n";
                txtNotes.Value += "•	(X)Destaco clamps\n";
                txtNotes.Value += "•	5mm Feeler to check part form\n";
                txtNotes.Value += "•	(1) 3mm go / no go feeler\n";
                txtNotes.Value += "•	Tol Groove to check part trim edge\n";
                txtNotes.Value += "•	Flush check to check part trim edge\n";
                txtNotes.Value += "•	(X)SPC checks\n";
                txtNotes.Value += "•	Mitutoyo indicator, zero set block and indicator storage case provided\n";
                txtNotes.Value += "•	(X)location checks for holes / slots\n";
                txtNotes.Value += "•	(X)Go / no go pin to check hole size\n";
                txtNotes.Value += "•	Go / NoGo feeler for flatness check\n";
                txtNotes.Value += "•	Gage R & R 5x3x3\n";
                txtNotes.Value += "•	Gage R 1x10\n";
                txtNotes.Value += "•	3rd Party Certification\n";


                string user = master.getUserName();

                if (user == "sjelsma@toolingsystemsgroup.com")
                {
                    ddlEstimator.SelectedValue = "70";
                }
                else if (user == "jmomber@toolingsystemsgroup.com")
                {
                    ddlEstimator.SelectedValue = "72";
                }
                else if (user == "cgould@toolingsystemsgroup.com")
                {
                    ddlEstimator.SelectedValue = "73";
                }

                ddlPayment.SelectedValue = "1";
                ddlQuoteType.SelectedValue = "109";
                ddlShipping.SelectedValue = "1";
                txtShippingLocation.Text = "GRR";

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
                cbGeneralNote1.Checked = true;
                cbGeneralNote2.Checked = true;
                cbGeneralNote3.Checked = true;
                cbGeneralNote4.Checked = true;
                cbGeneralNote5.Checked = true;
            }

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            SqlCommand sql2 = new SqlCommand();
            sql2.Connection = connection2;

            sql.CommandText = "Select prtPartNumber, prtpartDescription, prtPicture, prtPARTID, prtPartLength, prtPartWidth, prtPartHeight from tblPart, linkPartReservedToCompany ";
            sql.CommandText += "where prcRFQID = @rfqID and prcTSGCompanyID = 15 and prcPartID = prtPARTID and ";
            sql.CommandText += "(prtPARTID = (select min(ppdPartID) from linkPartToPartDetail where ppdPartToPartID = (Select min(ppdPartToPartID) from linkPartToPartDetail where ppdPartID = prtPARTID)) ";
            sql.CommandText += "or(select min(ppdPartID) from linkPartToPartDetail where ppdPartToPartID = (Select min(ppdPartToPartID) from linkPartToPartDetail where ppdPartID = prtPARTID)) is null) ";
            sql.CommandText += "and prtPARTID not in (select ptqPartID from linkPartToQuote where ptqPartID = prtPARTID and ptqUGS = 1) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqID", rfqID);
            dr = sql.ExecuteReader();
            while(dr.Read())
            {
                TableRow tr = new TableRow();
                TableCell tc = new TableCell();
                TableCell tc2 = new TableCell();
                TableCell tc3 = new TableCell();
                TableCell tc4 = new TableCell();
                TableCell tc5 = new TableCell();
                TableCell tc6 = new TableCell();

                string partNum = "";
                int partNums = 0;

                sql2.CommandText = "Select prtPartNumber from linkPartToPartDetail, tblPart where ppdPartID = prtPARTID and ppdPartToPartID = (select ppdPartToPartID from linkPartToPartdetail ";
                sql2.CommandText += "where ppdPartID = @partID) and prtPARTID <> @partID";
                sql2.Parameters.Clear();
                sql2.Parameters.AddWithValue("@partID", dr.GetValue(3).ToString());
                SqlDataReader dr2 = sql2.ExecuteReader();
                while(dr2.Read())
                {
                    if(partNums == 0)
                    {
                        partNum += dr2.GetValue(0).ToString();
                    }
                    else
                    {
                        partNum += ", " + dr2.GetValue(0).ToString();
                    }
                    partNums++;
                }
                dr2.Close();
                partID.Add(dr.GetValue(3).ToString());

                tc.Text = "<img ID='imgPart" + count + "' src='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/" + dr.GetValue(2).ToString() + "' Width='310px' Height='230px'/>";
                //Gets the linked part names to include
                if(partNums != 0)
                {
                    tc2.Text = "<b>Part Number</b><br /><textarea style='width: 300px; height: 150px;'name='txtPartNumber" + count + "' id='txtPartNumber" + count + "' >" + dr.GetValue(0).ToString() + ", " + partNum + "</textarea>";
                    tc2.Text += "<b>Part Number 2</b><br /><textarea style='width: 300px; height: 150px;'name='txtPartNumber" + count + "_2' id='txtPartNumber" + count + "_2' >" + "</textarea>";
                }
                else
                {
                    tc2.Text = "<b>Part Number</b><br /><textarea style='width: 300px; height: 150px;'name='txtPartNumber" + count + "' id='txtPartNumber" + count + "' >" + dr.GetValue(0).ToString() + "</textarea>";
                }
                tc3.Text = "<b>Part Name</b><br /><textarea style='width: 300px; height: 150px;' name='txtPartName" + count + "' id='txtPartName" + count + "' >" + dr.GetValue(1).ToString() + "</textarea>";
                if(partNums != 0)
                {
                    tc3.Text += "<b>Part Name</b><br /><textarea style='width: 300px; height: 150px;' name='txtPartName" + count + "_2' id='txtPartName" + count + "_2' >" + "</textarea>";
                }

                tc4.Text = "<b>Lead Time</b><br />";
                tc4.Text += "<select name='leadTime" + count + "' id='leadTime" + count + "' onchange='updateLeadTime(" + count + ");' >";
                tc4.Text += "<option value='Please Select'>Please Select</ option>";

                sql2.CommandText = "Select ultUGSLeadTime from pktblUGSLeadTime";
                sql2.Parameters.Clear();
                dr2 = sql2.ExecuteReader();
                while(dr2.Read())
                {
                    tc4.Text += "<option value='" + dr2.GetValue(0).ToString() + "'>" + dr2.GetValue(0).ToString() + "</option>";
                }
                dr2.Close();

                tc4.Text += "</ select>";
                tc4.Text += "<textarea id='txtLeadTime" + count + "' name='txtLeadTime" + count + "'></textarea>";
                tc4.Text += "<br /><b>Total Price</b><br /><textarea id='txtTotalPrice" + count + "' name='txtTotalPrice" + count + "'></textarea>";
                if (partNums != 0)
                {
                    tc4.Text += "<br /><b>Individual Quotes</b><input type='checkbox' id='cbIndividualQuotes" + count + "' name='cbIndividualQuotes" + count + "'></input>";
                    tc4.Text += "<br /><b>Total Price Quote 2</b><br /><textarea id='txtTotalPrice" + count + "_2' name='txtTotalPrice" + count + "_2'></textarea>";
                }
                tc5.Text = "<b>Part Length</b><textarea name='txtPartLength" + count + "' id=txtPartLength" + count + "' >" + dr.GetValue(4).ToString() + "</textarea>";
                tc5.Text += "<br /><b>Part Width&nbsp&nbsp</b><textarea name='txtPartWidth" + count + "' id=txtPartWidth" + count + "' >" + dr.GetValue(5).ToString() + "</textarea>";
                tc5.Text += "<br /><b>Part Height&nbsp</b><textarea name='txtPartHeight" + count + "' id=txtPartHeight" + count + "' >" + dr.GetValue(6).ToString() + "</textarea>";

                tc6.Text = "<textarea id='txtNote" + count + "' cols='150' rows='15' name='txtNote" + count + "' style='max-width: 500px; width: 500px'></textarea>";

                //<textarea id="txtNotes" cols="400" rows="20" runat="server" style="max-width: 1000px; width: 1000px"></textarea><br /><br />
                
                tr.Cells.Add(tc);
                tr.Cells.Add(tc2);
                tr.Cells.Add(tc3);
                tr.Cells.Add(tc4);
                tr.Cells.Add(tc5);
                tr.Cells.Add(tc6);
                tblResults.Rows.Add(tr);
                count++;
            }
            dr.Close();

            hdnNumberOfParts.Value = count.ToString();

            connection.Close();
            connection2.Close();
        }

        public void save(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            string salesman = "4";

            sql.CommandText = "Select TSGSalesmanID from TSGSalesman where Name = @salesman";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@salesman", lblSalesman.Text);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                salesman = dr.GetValue(0).ToString();
            }
            dr.Close();
            string lastPartNum = "";
            for (int i = 0; i < count; i++)
            {
                string number = Request.Form["txtPartNumber" + i.ToString()].ToString();
                string name = Request.Form["txtPartName" + i.ToString()].ToString();
                string leadTime = "";
                leadTime = Request.Form["txtLeadTime" + i.ToString()].ToString();

                if (leadTime == "")
                {
                    if (lastPartNum == "")
                    {
                        lastPartNum = "'" + number + "'";
                    }
                    else
                    {
                        lastPartNum += ", '" + number + "'";
                    }
                    continue;
                }

                string length = Request.Form["txtPartLength" + i.ToString()].ToString();
                string width = Request.Form["txtPartWidth" + i.ToString()].ToString();
                string height = Request.Form["txtPartHeight" + i.ToString()].ToString();
                double price = 0;
                try
                {
                    price = System.Convert.ToDouble(Request.Form["txtTotalPrice" + i.ToString()].ToString().Replace("$", ""));
                }
                catch
                {
                    if (lastPartNum == "")
                    {
                        lastPartNum = "'" + number + "'";
                    }
                    else
                    {
                        lastPartNum += ", " + "'" + number + "'";
                    }
                    continue;
                }

                if(price == 0)
                {
                    if (lastPartNum == "")
                    {
                        lastPartNum = "'" + number + "'";
                    }
                    else
                    {
                        lastPartNum += ", " + "'" + number + "'";
                    }
                    continue;
                }

                string notes = "";
                if (Request.Form["txtNote" + i.ToString()].ToString().Trim() == "")
                {
                    notes = txtNotes.InnerText;
                }
                else
                {
                    notes = Request.Form["txtNote" + i.ToString()].ToString();
                }

                Boolean check = false;
                try
                {
                    if (Request.Form["cbIndividualQuotes" + i] != null && Request.Form["cbIndividualQuotes" + i] == "on")
                    {
                        check = true;
                    }
                    else
                    {
                        check = false;
                    }
                }
                catch
                {

                }

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
                sql.Parameters.Clear();

                sql.Parameters.AddWithValue("@Management", 67.33);
                sql.Parameters.AddWithValue("@ProjectEng", 67.33);
                sql.Parameters.AddWithValue("@ReadData", 65);
                sql.Parameters.AddWithValue("@3DModel", 65);
                sql.Parameters.AddWithValue("@Drawing", 65);
                sql.Parameters.AddWithValue("@Updates", 65);
                sql.Parameters.AddWithValue("@Programming", 61);
                sql.Parameters.AddWithValue("@CNC", 61);
                sql.Parameters.AddWithValue("@Certification", 63);
                sql.Parameters.AddWithValue("@GageRRCMM", 63);
                sql.Parameters.AddWithValue("@PartLayouts", 63);
                sql.Parameters.AddWithValue("@Base", 62);
                sql.Parameters.AddWithValue("@Details", 62);
                sql.Parameters.AddWithValue("@LocationPins", 62);
                sql.Parameters.AddWithValue("@GoNoGoPins", 62);
                sql.Parameters.AddWithValue("@SPC", 62);
                sql.Parameters.AddWithValue("@GageRRFixtures", 62);
                sql.Parameters.AddWithValue("@Assemble", 62);
                sql.Parameters.AddWithValue("@Pallets", 57);
                sql.Parameters.AddWithValue("@Transportation", 57);
                sql.Parameters.AddWithValue("@BasePlate", 1000);
                sql.Parameters.AddWithValue("@Aluminum", 200);
                sql.Parameters.AddWithValue("@Steel", 15);
                sql.Parameters.AddWithValue("@FixturePlank", 15);
                sql.Parameters.AddWithValue("@Wood", 30);
                sql.Parameters.AddWithValue("@Bushings", 15);
                sql.Parameters.AddWithValue("@DrillBlanks", 15);
                sql.Parameters.AddWithValue("@Clamps", 30);
                sql.Parameters.AddWithValue("@Indicator", 150);
                sql.Parameters.AddWithValue("@IndCollar", 35);
                sql.Parameters.AddWithValue("@IndStorCase", 75);
                sql.Parameters.AddWithValue("@ZeroSet", 125);
                sql.Parameters.AddWithValue("@SpcTriggers", 250);
                sql.Parameters.AddWithValue("@TempDrops", 200);
                sql.Parameters.AddWithValue("@HingeDrops", 200);
                sql.Parameters.AddWithValue("@Risers", 150);
                sql.Parameters.AddWithValue("@Handles", 8);
                sql.Parameters.AddWithValue("@JigFeet", 5);
                sql.Parameters.AddWithValue("@ToolingBalls", 25);
                sql.Parameters.AddWithValue("@TBCovers", 5);
                sql.Parameters.AddWithValue("@TBPads", 20);
                sql.Parameters.AddWithValue("@Slides", 250);
                sql.Parameters.AddWithValue("@Magnets", 25);
                sql.Parameters.AddWithValue("@Hardware", 75);
                sql.Parameters.AddWithValue("@LMI", 250);
                sql.Parameters.AddWithValue("@Annodizing", 150);
                sql.Parameters.AddWithValue("@BlackOxide", 45);
                sql.Parameters.AddWithValue("@HeatTreat", 50);
                sql.Parameters.AddWithValue("@EngrvdTags", 100);
                sql.Parameters.AddWithValue("@CNCServices", 1500);
                sql.Parameters.AddWithValue("@Grinding", 250);
                sql.Parameters.AddWithValue("@Shipping", 300);
                sql.Parameters.AddWithValue("@ThirdPartyCMM", 400);
                sql.Parameters.AddWithValue("@Welding", 300);
                sql.Parameters.AddWithValue("@WireBurn", 300);
                sql.Parameters.AddWithValue("@Rebates", 250);
                sql.Parameters.AddWithValue("@Cost", 0);
                sql.Parameters.AddWithValue("@CreatedBy", master.getUserName());

                string cost = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();


                sql.CommandText = "INSERT INTO tblUGSQuote (uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, uquCustomerContact, ";
                sql.CommandText += "uquSalesmanID, uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquTotalPrice, uquUseTSG, uquNotes, uquCreated, uquCreatedBy, uquShippingLocation, uquDieType, ";

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
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@partNum", number);
                sql.Parameters.AddWithValue("@partName", name);
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@contact", txtCustContact.Text);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custRFQNum", txtCustRFQNum.Text);
                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                sql.Parameters.AddWithValue("@leadtime", leadTime);
                sql.Parameters.AddWithValue("@jobNum", "");
                sql.Parameters.AddWithValue("@total", price);
                if (chkUseTSG.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                }
                sql.Parameters.AddWithValue("@notes", notes);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);

                sql.Parameters.AddWithValue("@management", 0);
                sql.Parameters.AddWithValue("@projectEng", 0);
                sql.Parameters.AddWithValue("@readData", 0);
                sql.Parameters.AddWithValue("@model", 0);
                sql.Parameters.AddWithValue("@drawing", 0);
                sql.Parameters.AddWithValue("@updates", 0);
                sql.Parameters.AddWithValue("@programming", 0);
                sql.Parameters.AddWithValue("@cnc", 0);
                sql.Parameters.AddWithValue("@certification", 0);
                sql.Parameters.AddWithValue("@gageRRCMM", 0);
                sql.Parameters.AddWithValue("@partLayouts", 0);
                sql.Parameters.AddWithValue("@base", 0);
                sql.Parameters.AddWithValue("@details", 0);
                sql.Parameters.AddWithValue("@locationPins", 0);
                sql.Parameters.AddWithValue("@goNoGoPins", 0);
                sql.Parameters.AddWithValue("@spc", 0);
                sql.Parameters.AddWithValue("@gageRRFixtures", 0);
                sql.Parameters.AddWithValue("@assemble", 0);
                sql.Parameters.AddWithValue("@pallets", 0);
                sql.Parameters.AddWithValue("@transportation", 0);
                sql.Parameters.AddWithValue("@basePlate", 0);
                sql.Parameters.AddWithValue("@aluminum", 0);
                sql.Parameters.AddWithValue("@steel", 0);
                sql.Parameters.AddWithValue("@fixturePlank", 0);
                sql.Parameters.AddWithValue("@wood", 0);
                sql.Parameters.AddWithValue("@bushings", 0);
                sql.Parameters.AddWithValue("@drillBlanks", 0);
                sql.Parameters.AddWithValue("@clamps", 0);
                sql.Parameters.AddWithValue("@indicator", 0);
                sql.Parameters.AddWithValue("@indCollar", 0);
                sql.Parameters.AddWithValue("@indStorCase", 0);
                sql.Parameters.AddWithValue("@zeroSet", 0);
                sql.Parameters.AddWithValue("@spcTriggers", 0);
                sql.Parameters.AddWithValue("@tempDrops", 0);
                sql.Parameters.AddWithValue("@hingeDrops", 0);
                sql.Parameters.AddWithValue("@risers", 0);
                sql.Parameters.AddWithValue("@handles", 0);
                sql.Parameters.AddWithValue("@jigFeet", 0);
                sql.Parameters.AddWithValue("@toolingBalls", 0);
                sql.Parameters.AddWithValue("@tBCovers", 0);
                sql.Parameters.AddWithValue("@tBPads", 0);
                sql.Parameters.AddWithValue("@slides", 0);
                sql.Parameters.AddWithValue("@magnets", 0);
                sql.Parameters.AddWithValue("@hardware", 0);
                sql.Parameters.AddWithValue("@lmi", 0);
                sql.Parameters.AddWithValue("@annodizing", 0);
                sql.Parameters.AddWithValue("@blackOxide", 0);
                sql.Parameters.AddWithValue("@heatTreat", 0);
                sql.Parameters.AddWithValue("@engrvdTags", 0);
                sql.Parameters.AddWithValue("@cncServices", 0);
                sql.Parameters.AddWithValue("@grinding", 0);
                sql.Parameters.AddWithValue("@shippingCalc", 0);
                sql.Parameters.AddWithValue("@thirdPartyCMM", 0);
                sql.Parameters.AddWithValue("@welding", 0);
                sql.Parameters.AddWithValue("@wireBurn", 0);
                sql.Parameters.AddWithValue("@rebates", 0);
                sql.Parameters.AddWithValue("@ugsCostID", cost);
                sql.Parameters.AddWithValue("@partLength", length);
                sql.Parameters.AddWithValue("@partWidth", width);
                sql.Parameters.AddWithValue("@partHeight", height);


                string quoteID = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

                String FileName = "";
                string pictureName = "";
                if (FileName == "")
                {
                    sql.CommandText = "Select prtPicture from tblPart where prtPARTID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID[i]);
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

                if (rfqID != 0 && partID[i] != "")
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
                    sql.Parameters.AddWithValue("@partID", partID[i]);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }


                if (!check)
                {
                    List<string> partIDs = new List<string>();
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail ";
                    sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID[i]);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        partIDs.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    for (int j = 0; j < partIDs.Count; j++)
                    {
                        sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                        sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 1)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partIDs[j]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "UGS Multi Quote");
                    }
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


                for (int j = 0; j < cb.Count; j++)
                {
                    if (cb[j].Checked)
                    {
                        sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                        sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@noteID", generalNote[j].Text.Split('-')[0]);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "HTSEditQuote");
                    }
                }


                if (check)
                {
                    number = Request.Form["txtPartNumber" + i.ToString() + "_2"].ToString();
                    name = Request.Form["txtPartName" + i.ToString() + "_2"].ToString();
                    //leadTime = Request.Form["txtLeadTime" + i.ToString() + "_2"].ToString();
                    price = 0;
                    try
                    {
                        price = System.Convert.ToDouble(Request.Form["txtTotalPrice" + i.ToString() + "_2"].ToString().Replace("$", ""));
                    }
                    catch
                    {
                        continue;
                    }


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
                    sql.Parameters.Clear();

                    sql.Parameters.AddWithValue("@Management", 67.33);
                    sql.Parameters.AddWithValue("@ProjectEng", 67.33);
                    sql.Parameters.AddWithValue("@ReadData", 65);
                    sql.Parameters.AddWithValue("@3DModel", 65);
                    sql.Parameters.AddWithValue("@Drawing", 65);
                    sql.Parameters.AddWithValue("@Updates", 65);
                    sql.Parameters.AddWithValue("@Programming", 61);
                    sql.Parameters.AddWithValue("@CNC", 61);
                    sql.Parameters.AddWithValue("@Certification", 63);
                    sql.Parameters.AddWithValue("@GageRRCMM", 63);
                    sql.Parameters.AddWithValue("@PartLayouts", 63);
                    sql.Parameters.AddWithValue("@Base", 62);
                    sql.Parameters.AddWithValue("@Details", 62);
                    sql.Parameters.AddWithValue("@LocationPins", 62);
                    sql.Parameters.AddWithValue("@GoNoGoPins", 62);
                    sql.Parameters.AddWithValue("@SPC", 62);
                    sql.Parameters.AddWithValue("@GageRRFixtures", 62);
                    sql.Parameters.AddWithValue("@Assemble", 62);
                    sql.Parameters.AddWithValue("@Pallets", 57);
                    sql.Parameters.AddWithValue("@Transportation", 57);
                    sql.Parameters.AddWithValue("@BasePlate", 1000);
                    sql.Parameters.AddWithValue("@Aluminum", 200);
                    sql.Parameters.AddWithValue("@Steel", 15);
                    sql.Parameters.AddWithValue("@FixturePlank", 15);
                    sql.Parameters.AddWithValue("@Wood", 30);
                    sql.Parameters.AddWithValue("@Bushings", 15);
                    sql.Parameters.AddWithValue("@DrillBlanks", 15);
                    sql.Parameters.AddWithValue("@Clamps", 30);
                    sql.Parameters.AddWithValue("@Indicator", 150);
                    sql.Parameters.AddWithValue("@IndCollar", 35);
                    sql.Parameters.AddWithValue("@IndStorCase", 75);
                    sql.Parameters.AddWithValue("@ZeroSet", 125);
                    sql.Parameters.AddWithValue("@SpcTriggers", 250);
                    sql.Parameters.AddWithValue("@TempDrops", 200);
                    sql.Parameters.AddWithValue("@HingeDrops", 200);
                    sql.Parameters.AddWithValue("@Risers", 150);
                    sql.Parameters.AddWithValue("@Handles", 8);
                    sql.Parameters.AddWithValue("@JigFeet", 5);
                    sql.Parameters.AddWithValue("@ToolingBalls", 25);
                    sql.Parameters.AddWithValue("@TBCovers", 5);
                    sql.Parameters.AddWithValue("@TBPads", 20);
                    sql.Parameters.AddWithValue("@Slides", 250);
                    sql.Parameters.AddWithValue("@Magnets", 25);
                    sql.Parameters.AddWithValue("@Hardware", 75);
                    sql.Parameters.AddWithValue("@LMI", 250);
                    sql.Parameters.AddWithValue("@Annodizing", 150);
                    sql.Parameters.AddWithValue("@BlackOxide", 45);
                    sql.Parameters.AddWithValue("@HeatTreat", 50);
                    sql.Parameters.AddWithValue("@EngrvdTags", 100);
                    sql.Parameters.AddWithValue("@CNCServices", 1500);
                    sql.Parameters.AddWithValue("@Grinding", 250);
                    sql.Parameters.AddWithValue("@Shipping", 300);
                    sql.Parameters.AddWithValue("@ThirdPartyCMM", 400);
                    sql.Parameters.AddWithValue("@Welding", 300);
                    sql.Parameters.AddWithValue("@WireBurn", 300);
                    sql.Parameters.AddWithValue("@Rebates", 250);
                    sql.Parameters.AddWithValue("@Cost", 0);
                    sql.Parameters.AddWithValue("@CreatedBy", master.getUserName());

                    cost = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

                    int pID = 0;
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail ";
                    sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID[i]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        pID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    string partNumber = "", partDescription = "", partLength = "", partWidth = "", partHeight = "";
                    sql.CommandText = "Select prtPartNumber, prtpartDescription, prtPartLength, prtPartWidth, prtPartHeight from tblPart where prtPARTID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", pID);
                    dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        partNumber = dr.GetValue(0).ToString();
                        partDescription = dr.GetValue(1).ToString();
                        partLength = dr.GetValue(2).ToString();
                        partWidth = dr.GetValue(3).ToString();
                        partHeight = dr.GetValue(4).ToString();
                    }
                    dr.Close();


                    sql.CommandText = "INSERT INTO tblUGSQuote (uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, uquCustomerContact, ";
                    sql.CommandText += "uquSalesmanID, uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquTotalPrice, uquUseTSG, uquNotes, uquCreated, uquCreatedBy, uquShippingLocation, uquDieType, ";

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
                    sql.CommandText += "@welding, @wireBurn, @rebates, @ugsCostID, @length, @width, @height )";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@status", 2);
                    sql.Parameters.AddWithValue("@partNum", partNumber);
                    sql.Parameters.AddWithValue("@partName", partDescription);
                    sql.Parameters.AddWithValue("@rfq", rfqID);
                    sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                    sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                    sql.Parameters.AddWithValue("@contact", txtCustContact.Text);
                    sql.Parameters.AddWithValue("@salesman", salesman);
                    sql.Parameters.AddWithValue("@custRFQNum", txtCustRFQNum.Text);
                    sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                    sql.Parameters.AddWithValue("@shipping", ddlShipping.SelectedValue);
                    sql.Parameters.AddWithValue("@payment", ddlPayment.SelectedValue);
                    sql.Parameters.AddWithValue("@leadtime", leadTime);
                    sql.Parameters.AddWithValue("@jobNum", "");
                    sql.Parameters.AddWithValue("@total", price);
                    if (chkUseTSG.Checked)
                    {
                        sql.Parameters.AddWithValue("@logo", 1);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@logo", 0);
                    }
                    sql.Parameters.AddWithValue("@notes", notes);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@shippingLocation", txtShippingLocation.Text);
                    sql.Parameters.AddWithValue("@quoteType", ddlQuoteType.SelectedValue);

                    sql.Parameters.AddWithValue("@management", 0);
                    sql.Parameters.AddWithValue("@projectEng", 0);
                    sql.Parameters.AddWithValue("@readData", 0);
                    sql.Parameters.AddWithValue("@model", 0);
                    sql.Parameters.AddWithValue("@drawing", 0);
                    sql.Parameters.AddWithValue("@updates", 0);
                    sql.Parameters.AddWithValue("@programming", 0);
                    sql.Parameters.AddWithValue("@cnc", 0);
                    sql.Parameters.AddWithValue("@certification", 0);
                    sql.Parameters.AddWithValue("@gageRRCMM", 0);
                    sql.Parameters.AddWithValue("@partLayouts", 0);
                    sql.Parameters.AddWithValue("@base", 0);
                    sql.Parameters.AddWithValue("@details", 0);
                    sql.Parameters.AddWithValue("@locationPins", 0);
                    sql.Parameters.AddWithValue("@goNoGoPins", 0);
                    sql.Parameters.AddWithValue("@spc", 0);
                    sql.Parameters.AddWithValue("@gageRRFixtures", 0);
                    sql.Parameters.AddWithValue("@assemble", 0);
                    sql.Parameters.AddWithValue("@pallets", 0);
                    sql.Parameters.AddWithValue("@transportation", 0);
                    sql.Parameters.AddWithValue("@basePlate", 0);
                    sql.Parameters.AddWithValue("@aluminum", 0);
                    sql.Parameters.AddWithValue("@steel", 0);
                    sql.Parameters.AddWithValue("@fixturePlank", 0);
                    sql.Parameters.AddWithValue("@wood", 0);
                    sql.Parameters.AddWithValue("@bushings", 0);
                    sql.Parameters.AddWithValue("@drillBlanks", 0);
                    sql.Parameters.AddWithValue("@clamps", 0);
                    sql.Parameters.AddWithValue("@indicator", 0);
                    sql.Parameters.AddWithValue("@indCollar", 0);
                    sql.Parameters.AddWithValue("@indStorCase", 0);
                    sql.Parameters.AddWithValue("@zeroSet", 0);
                    sql.Parameters.AddWithValue("@spcTriggers", 0);
                    sql.Parameters.AddWithValue("@tempDrops", 0);
                    sql.Parameters.AddWithValue("@hingeDrops", 0);
                    sql.Parameters.AddWithValue("@risers", 0);
                    sql.Parameters.AddWithValue("@handles", 0);
                    sql.Parameters.AddWithValue("@jigFeet", 0);
                    sql.Parameters.AddWithValue("@toolingBalls", 0);
                    sql.Parameters.AddWithValue("@tBCovers", 0);
                    sql.Parameters.AddWithValue("@tBPads", 0);
                    sql.Parameters.AddWithValue("@slides", 0);
                    sql.Parameters.AddWithValue("@magnets", 0);
                    sql.Parameters.AddWithValue("@hardware", 0);
                    sql.Parameters.AddWithValue("@lmi", 0);
                    sql.Parameters.AddWithValue("@annodizing", 0);
                    sql.Parameters.AddWithValue("@blackOxide", 0);
                    sql.Parameters.AddWithValue("@heatTreat", 0);
                    sql.Parameters.AddWithValue("@engrvdTags", 0);
                    sql.Parameters.AddWithValue("@cncServices", 0);
                    sql.Parameters.AddWithValue("@grinding", 0);
                    sql.Parameters.AddWithValue("@shippingCalc", 0);
                    sql.Parameters.AddWithValue("@thirdPartyCMM", 0);
                    sql.Parameters.AddWithValue("@welding", 0);
                    sql.Parameters.AddWithValue("@wireBurn", 0);
                    sql.Parameters.AddWithValue("@rebates", 0);
                    sql.Parameters.AddWithValue("@ugsCostID", cost);
                    sql.Parameters.AddWithValue("@length", partLength);
                    sql.Parameters.AddWithValue("@width", partWidth);
                    sql.Parameters.AddWithValue("@height", partHeight);


                    quoteID = master.ExecuteScalar(sql, "UGS Edit Quote").ToString();

                    


                    FileName = "";
                    pictureName = "";
                    if (FileName == "")
                    {
                        sql.CommandText = "Select prtPicture from tblPart where prtPARTID = @partID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", pID);
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


                    


                    if (rfqID != 0 && pID != 0)
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
                        sql.Parameters.AddWithValue("@partID", pID);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@user", master.getUserName());

                        master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                    }


                    generalNote = new List<Label>();
                    generalNote.Add(lblGeneralNote1);
                    generalNote.Add(lblGeneralNote2);
                    generalNote.Add(lblGeneralNote3);
                    generalNote.Add(lblGeneralNote4);
                    generalNote.Add(lblGeneralNote5);
                    //generalNote.Add(lblGeneralNote6);
                    //generalNote.Add(lblGeneralNote7);
                    //generalNote.Add(lblGeneralNote8);
                    //generalNote.Add(lblGeneralNote9);


                    cb = new List<CheckBox>();
                    cb.Add(cbGeneralNote1);
                    cb.Add(cbGeneralNote2);
                    cb.Add(cbGeneralNote3);
                    cb.Add(cbGeneralNote4);
                    cb.Add(cbGeneralNote5);
                    //cb.Add(cbGeneralNote6);
                    //cb.Add(cbGeneralNote7);
                    //cb.Add(cbGeneralNote8);
                    //cb.Add(cbGeneralNote9);


                    for (int j = 0; j < cb.Count; j++)
                    {
                        if (cb[j].Checked)
                        {
                            sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                            sql.CommandText += "Values (@noteID, @quoteID, GETDATE(), @createdBy)";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@noteID", generalNote[j].Text.Split('-')[0]);
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            master.ExecuteNonQuery(sql, "HTSEditQuote");
                        }
                    }
                }


            }
            if (lastPartNum != "")
            {
                litScript.Text = "<script>alert('Your quotes have been saved, these are the parts that still need to be quoted: ";
                litScript.Text += HttpUtility.JavaScriptStringEncode(lastPartNum) + ".  Please refresh this page to continue ";
                litScript.Text += "quoting or close this tab and refresh the RFQ');</script>";
            }
            else
            {
                litScript.Text = "<script>alert('Quotes have been saved. Please close tab and refresh the RFQ');</script>";
            }

            connection.Close();
        }
    }
}