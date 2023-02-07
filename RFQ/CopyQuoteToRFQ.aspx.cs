using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class CopyQuoteToRFQ : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;


            string partID = "";
            string quoteID = "";
            string quoteNum = "";
            string historicalQuoteNum = "";
            string rfqID = "";
            string keepQuoteNum = "";
            if (Request["partID"] != "")
            {
                partID = Request["partID"];
            }
            if (Request["quoteID"] != "")
            {
                quoteID = Request["quoteID"];
            }
            if (Request["quoteNum"] != "")
            {
                quoteNum = Request["quoteNum"];
            }
            if(Request["rfqID"] != "")
            {
                rfqID = Request["rfqID"];
            }
            if (Request["keep"] != "")
            {
                keepQuoteNum = Request["keep"];
            }

            quoteID.Replace("'", "");
            quoteNum.Replace("'", "");
            partID.Replace("'", "");
            Boolean sa = false;
            Boolean hts = false;
            Boolean sts = false;
            Boolean ugs = false;

            if (quoteNum.Contains("-MAS"))
            {
                historicalQuoteNum = quoteNum.Split('-')[1];
            }
            else
            {
                if (quoteNum.Contains("HTS"))
                {
                    hts = true;
                }
                else if (quoteNum.Contains("STS"))
                {
                    sts = true;
                }
                else if (quoteNum.Contains("UGS"))
                {
                    ugs = true;
                }
                else if (quoteNum.Contains("SA"))
                {
                    sa = true;

                    // This is to check and see if it is actaully a quote which was copied into an RFQ with an SA name
                    string firstNum = quoteNum.Split('-')[0];
                    sql.CommandText = "Select quoQuoteID from tblQuote where quoOldQuoteNumber like @quoteNum ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteNum", firstNum + "%SA%");
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["quoQuoteID"].ToString() == quoteID)
                        {
                            sa = false;
                        }
                    }
                    dr.Close();
                }
                quoteNum = quoteNum.Split('-')[0];
            }

            if(historicalQuoteNum != "")
            {
                //quoVersion will have to be updated after the initial insert
                //payment and shipping will have to be updated after initial insert
                //Tool type will have to be updated after the initial insert
                //Estimator will have to be updated after initial insert
                string matType = "", width = "", pitch = "", thickness = "", estimator = "";

                sql.CommandText = "Select qhiMaterialType, qhiPartWidthEng, qhiPartPitchEng, qhiMaterialThickMet, qhiEstimator from tblQuoteHistory where qhiQuoteHistoryID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    matType = dr.GetValue(0).ToString();
                    width = dr.GetValue(1).ToString();
                    pitch = dr.GetValue(2).ToString();
                    thickness = dr.GetValue(3).ToString();
                    estimator = dr.GetValue(4).ToString();
                }
                dr.Close();

                string matTypeID = "";

                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType like @matType";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", matType);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    matTypeID = dr.GetValue(0).ToString();
                }
                dr.Close();
                if(matTypeID == "")
                {
                    sql.CommandText = "Insert into pktblMaterialType (mtyMaterialType) ";
                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                    sql.CommandText += "values (@matType) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@matType", matType);
                    matTypeID = master.ExecuteScalar(sql, "Copy quote to RFQ").ToString();
                }

                sql.CommandText = "insert into pktblBlankInfo (binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, binCreated, ";
                sql.CommandText += "binCreatedBy) ";
                sql.CommandText += "output inserted.binBlankInfoID ";
                sql.CommandText += "values (@matTypeID, @thickEng, @thickMet, @pitchEng, @pitchMet, @widthEng, @widthMet, GETDATE(), @createdBy)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matTypeID", matTypeID);
                sql.Parameters.AddWithValue("@thickEng", System.Convert.ToDouble(thickness) / 25.4);
                sql.Parameters.AddWithValue("@thickMet", thickness);
                sql.Parameters.AddWithValue("@pitchEng", pitch);
                sql.Parameters.AddWithValue("@pitchMet", System.Convert.ToDouble(pitch) * 25.4);
                sql.Parameters.AddWithValue("@widthEng", width);
                sql.Parameters.AddWithValue("@widthMet", System.Convert.ToDouble(width) * 25.4);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                string blankID = master.ExecuteScalar(sql, "Copy quote to RFQ").ToString();

                sql.CommandText = "Select estEstimatorID from pktblEstimators where estLastName = @estimator";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@estimator", estimator);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    estimator = dr.GetValue(0).ToString();
                }
                dr.Close();

                //default shipping terms to see notes if we cant match.
                string shippingTerms = "15";
                sql.CommandText = "select steShippingTermsID from pktblShippingTerms, tblQuoteHistory where qhiQuoteHistoryID = @id and qhiShippingTerms = steShippingTerms";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    shippingTerms = dr.GetValue(0).ToString();
                }
                dr.Close();


                //default payment terms to see notes if we cant match
                string paymentTerms = "16";
                sql.CommandText = "select ptePaymentTermsID from pktblPaymentTerms, tblQuoteHistory where qhiQuoteHistoryID = @id and qhiPaymentTerms = ptePaymentTerms";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    paymentTerms = dr.GetValue(0).ToString();
                }
                dr.Close();

                sql.CommandText = "insert into tblQuote(quoTSGCompanyID, quoTotalAmount, quoAnnualVolume, quoLeadTime, quoStatusID, quoProductTypeID, quoPartTypeID, quoToolCountryID, quoCreated, quoCreatedBy, quoCurrencyID, quoBlankInfoID, quoEstimatorID, quoUseTSGLogo, quoUseTSGName, quoPaymentTermsID, quoShippingTermsID) ";
                sql.CommandText += "output inserted.quoQuoteID ";
                sql.CommandText += "Select TSGCompanyID, qhiNonTaxableAmt, qhiAnnualVolume, qhiLeadTime, 2, 9, 33, 8, GETDATE(), @user, 1, @blankInfoID, @estimator, 0, 0, @payment, @shipping from tblQuoteHistory, TSGCompany where qhiQuoteHistoryID = @id and qhiGroupCompany = TSGCompanyAbbrev";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                sql.Parameters.AddWithValue("@blankInfoID", blankID);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@estimator", estimator);
                sql.Parameters.AddWithValue("@payment", paymentTerms);
                sql.Parameters.AddWithValue("@shipping", shippingTerms);

                int newquoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "CopyQuoteToRFQ").ToString());

                sql.Parameters.Clear();

                string version = "";
                string salesOrderNum = "";
                string toolType = "";
                string cavity = "";
                sql.CommandText = "Select qhiRFQNumber, qhiSalesOrderNo, qhiToolType, qhiCavity from tblQuoteHistory where qhiQuoteHistoryID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    version = dr.GetValue(0).ToString();
                    salesOrderNum = dr.GetValue(1).ToString();
                    toolType = dr.GetValue(2).ToString();
                    cavity = dr.GetValue(3).ToString();
                }
                dr.Close();
                string[] ver = version.Split('-');
                int verNum = 2;
                try
                {
                    verNum = System.Convert.ToInt32(ver[ver.Length - 1]);
                    verNum++;
                }
                catch
                {

                }
                sql.Parameters.Clear();
                string programID = "";
                string oemID = "";
                string vehicleID = "";
                string salesmanID = "";

                sql.CommandText = "Select rfqProgramID, rfqOEMID, rfqVehicleID, rfqSalesman from tblRFQ where rfqID = @id";
                sql.Parameters.AddWithValue("@id", rfqID);
                dr = sql.ExecuteReader();

                if(dr.Read())
                {
                    programID = dr.GetValue(0).ToString();
                    oemID = dr.GetValue(1).ToString();
                    vehicleID = dr.GetValue(2).ToString();
                    salesmanID = dr.GetValue(3).ToString();
                }

                dr.Close();
                sql.Parameters.Clear();
                sql.CommandText = "update tblQuote set quoRFQID = @rfqID, quoVersion = @version, quoProgramCodeID = @program, quoOEMID = @oem, quoVehicleID = @vehicle, quoProductTypeID = 2, quoNumber = @num, quoSalesman = @salesman where quoQuoteID = @quoteID ";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@version", verNum.ToString("00#"));
                sql.Parameters.AddWithValue("@program", programID);
                sql.Parameters.AddWithValue("@oem", oemID);
                sql.Parameters.AddWithValue("@vehicle", vehicleID);
                sql.Parameters.AddWithValue("@num", ver[0]);
                sql.Parameters.AddWithValue("@salesman", salesmanID);
                sql.Parameters.AddWithValue("@quoteID", newquoteID);
                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");

                sql.CommandText = "Select hpwPreWordedNote, hpwQuantityOrdered, hpwCostNote from pktblHistoricalPreWordedNote where hpwSalesOrderNo = @soNumber";
                sql.Parameters.AddWithValue("@soNumber", salesOrderNum);

                dr = sql.ExecuteReader();

                SqlCommand sql2 = new SqlCommand();
                SqlConnection connection2 = new SqlConnection(master.getConnectionString());
                connection2.Open();
                sql2.Connection = connection2;

                int i = 0;
                while (dr.Read())
                {
                    sql2.Parameters.Clear();
                    sql2.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                    sql2.CommandText += "output inserted.pwnPreWordedNoteID ";
                    sql2.CommandText += "values (@company, @note, @cost, GETDATE(), @user)";
                    string note = dr.GetValue(0).ToString();
                    int cost = 0;
                    string temp = dr.GetValue(2).ToString();
                    try
                    {
                        cost = System.Convert.ToInt32(dr.GetValue(2));
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
                        cost = cost * System.Convert.ToInt32(dr.GetValue(1));
                    }
                    catch
                    {
                    }
                    sql2.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql2.Parameters.AddWithValue("@note", note);
                    sql2.Parameters.AddWithValue("@cost", cost);
                    sql2.Parameters.AddWithValue("@user", master.getUserName());

                    string pwnID = master.ExecuteScalar(sql2, "CopyQuoteToRFQ").ToString();

                    sql2.Parameters.Clear();
                    sql2.CommandText = "insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                    sql2.CommandText += "values(@quoteID, @pwnID, GETDATE(), @user)";
                    sql2.Parameters.AddWithValue("@quoteID", newquoteID);
                    sql2.Parameters.AddWithValue("@pwnID", pwnID);
                    sql2.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql2, "CopyQuoteToRFQ");

                    i++;
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select DieTypeID, cavCavityID from DieType, pktblCavity where Name = @toolType and cavCavityName = @cavity";
                sql.Parameters.AddWithValue("@toolType", toolType);
                sql.Parameters.AddWithValue("@cavity", cavity);

                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    toolType = dr.GetValue(0).ToString();
                    cavity = dr.GetValue(1).ToString();
                }
                dr.Close();
                sql.Parameters.Clear();

                //FK DieType
                sql.CommandText = "insert into tblDieInfo (dinDietype, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, ";
                sql.CommandText += "dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy ) ";
                sql.CommandText += "output inserted.dinDieInfoID ";
                sql.CommandText += "Select @tool, @cavity, qhiDieFrontBackEng, qhiDieFrontBackMet, qhiDieLeftRightEng, qhiDIeLeftRightMet, ";
                sql.CommandText += "qhiShutHeightEng, qhiShutHeightMet, qhiNumberOfStations, GETDATE(), @user from tblQuoteHistory where qhiQuoteHistoryID = @id";
                sql.Parameters.AddWithValue("@tool", toolType);
                sql.Parameters.AddWithValue("@cavity", cavity);
                sql.Parameters.AddWithValue("@id", quoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                string dieInfoID = master.ExecuteScalar(sql, "CopyQuoteToRFQ").ToString();

                sql.Parameters.Clear();

                sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                sql.CommandText += "values(@dieInfo, @quoteID, GETDATE(), @user)";
                sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                sql.Parameters.AddWithValue("@quoteID", newquoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                sql.Parameters.Clear();

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 0)";
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@quoteID", newquoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                sql.Parameters.Clear();

                List<string> partIDs = new List<string>();
                partIDs.Add(partID);
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartTopartID from linkPartToPartDetail where ppdPartID = @partID)  and ppdPartID <> @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader sdr = sql.ExecuteReader();
                while (sdr.Read())
                {
                    partIDs.Add(sdr.GetValue(0).ToString());
                }
                sdr.Close();

                for (int j = 0; j < partIDs.Count; j++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 0)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[j]);
                    sql.Parameters.AddWithValue("@quoteID", newquoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }

                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values(@quoteID, @rfqID, GETDATE(), @user, 0, 0, 0)";
                sql.Parameters.AddWithValue("@quoteID", newquoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                sql.Parameters.Clear();
                connection2.Close();
            }
            else if (sa)
            {
                List<string> pwnIDs = new List<string>();
                //List<string> cost = new List<string>();
                double total = 0;

                sql.CommandText = "select pwnPreWordedNoteID, pwnCostNote ";
                sql.CommandText += "from linkPWNToECQuote, pktblPreWordedNote ";
                sql.CommandText += "where peqPreWordedNoteID = pwnPreWordedNoteID and peqECQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    pwnIDs.Add(dr.GetValue(0).ToString());
                    //cost.Add(dr.GetValue(1).ToString());
                    try
                    {
                        total += System.Convert.ToDouble(dr.GetValue(1).ToString());
                    }
                    catch
                    {

                    }
                }
                dr.Close();

                string salesman = "";
                string plant = "";
                sql.CommandText = "Select rfqSalesman, rfqPlantID from tblRFQ where rfqID = @rfq";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqID);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    salesman = dr["rfqSalesman"].ToString();
                    plant = dr["rfqPlantID"].ToString();
                }
                dr.Close();

                string user = master.getUserName();
                string partNum = "";
                string partName = "";
                string custRFQ = "";
                string accessNum = "";
                string dieType = "";
                string cavity = "";
                Boolean useTSG = false;
                string blankWidthEng = "";
                string blankWidthMet = "";
                string blankPitchEng = "";
                string blankPitchMet = "";
                string matThkEng = "";
                string matThkMet = "";
                string dieFBEng = "";
                string dieFBMet = "";
                string dieLREng = "";
                string dieLRMet = "";
                string shutHeightEng = "";
                string shutHeightMet = "";
                string matType = "";
                string numOfStations = "";
                string leadTime = "";
                string shipping = "";
                string payment = "";
                string countryOfOrign = "";
                string jobNumber = "";
                string shippingLocation = "";
                string version = "";
                string estimator = "";
                string quoteNumber = "";

                double toolingCost = 0;
                double transferBarCost = 0;
                double fixtureCost = 0;
                double dieSupportCost = 0;
                double shippingCost = 0;
                double additionalCost = 0;

                string additionalCostDesc = "";



                sql.CommandText = "select ecqPartNumber, ecqPartName, ecqCustomerRFQNumber, ecqAccessNumber, ecqDieType, ecqCavity, ecqUseTSG, ecqBlankWidthEng, ";
                sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ";
                sql.CommandText += "ecqDieLRMet, ecqShutHeightEng, ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ";
                sql.CommandText += "ecqCountryOfOrign, ecqJobNumber, ecqShippingLocation, ecqVersion, ecqEstimator, ecqQuoteNumber, TSGCompanyAbbrev, TSGCompanyID ";
                sql.CommandText += "from tblECQuote ";
                sql.CommandText += "left outer join TSGCompany on ecqTSGCompanyID = TSGCompanyID ";
                sql.CommandText += "where ecqECQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    partNum = dr["ecqPartNumber"].ToString();
                    partName = dr["ecqPartName"].ToString();
                    custRFQ = dr["ecqCustomerRFQNumber"].ToString();
                    accessNum = dr["ecqAccessNumber"].ToString();
                    dieType = dr["ecqDieType"].ToString();
                    cavity = dr["ecqCavity"].ToString();
                    useTSG = dr.GetBoolean(6);
                    blankWidthEng = dr["ecqBlankWidthEng"].ToString();
                    blankWidthMet = dr["ecqBlankWidthMet"].ToString();
                    blankPitchEng = dr["ecqBlankPitchEng"].ToString();
                    blankPitchMet = dr["ecqBlankPitchMet"].ToString();
                    matThkEng = dr["ecqMaterialThkEng"].ToString();
                    matThkMet = dr["ecqMaterialThkMet"].ToString();
                    dieFBEng = dr["ecqDieFBEng"].ToString();
                    dieFBMet = dr["ecqDieFBMet"].ToString();
                    dieLREng = dr["ecqDieLREng"].ToString();
                    dieLRMet = dr["ecqDieLRMet"].ToString();
                    shutHeightEng = dr["ecqShutHeightEng"].ToString();
                    shutHeightMet = dr["ecqShutHeightMet"].ToString();
                    matType = dr["ecqMaterialType"].ToString();
                    numOfStations = dr["ecqNumberOfStations"].ToString();
                    leadTime = dr["ecqLeadTime"].ToString();
                    shipping = dr["ecqShipping"].ToString();
                    payment = dr["ecqPayment"].ToString();
                    countryOfOrign = dr["ecqCountryOfOrign"].ToString();
                    jobNumber = dr["ecqJobNumber"].ToString();
                    shippingLocation = dr["ecqShippingLocation"].ToString();
                    version = dr["ecqVersion"].ToString();
                    estimator = dr["ecqEstimator"].ToString();
                    quoteNumber = dr["ecqQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-";
                }
                dr.Close();

                sql.CommandText = "insert into tblQuote (quoTSGCompanyID, quoRFQID, quoEstimatorID, quoPaymentTermsID, quoShippingTermsID, quoTotalAmount, ";
                sql.CommandText += "quoToolCountryID, quoLeadTime, quoCreated, quoCreatedBy, quoSalesman, quoStatusID, quoVersion, quoUseTSGLogo, ";
                sql.CommandText += "quoToolingCost, quoTransferBarCost, quoFixtureCost, quoDieSupportCost, quoShippingCost, quoAdditCostDesc, quoAdditCost, ";
                sql.CommandText += "quoUseTSGName, quoPartNumbers, quoCustomerQuoteNumber, quoCurrencyID, quoAccess, quoShippingLocation, quoOldQuoteNumber, quoPlant, quoCoppiedFromQuote, quoCoppiedFromEC) ";
                sql.CommandText += "output inserted.quoQuoteID ";
                sql.CommandText += "Values (@company, @rfq, @estimator, @paymentTerms, @shippingTerms, @totalAmount, @toolCountry, @leadTime, GETDATE(), @user, ";
                sql.CommandText += "@salesman, @status, @version, @logo, @toolingCost, @transferBar, @fixture, @dieSupport, @shippingCost, @addCostDesc, ";
                sql.CommandText += "@addCost, @tsgName, @partNumbers, @customerQuoteNumber, @currency, @accessNum, @shippingLocation, @oldQuoteNum, @plant, @quoteId, 1) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@estimator", estimator);
                sql.Parameters.AddWithValue("@paymentTerms", payment);
                sql.Parameters.AddWithValue("@shippingTerms", shipping);
                sql.Parameters.AddWithValue("@totalAmount", total);
                sql.Parameters.AddWithValue("@toolCountry", countryOfOrign);
                sql.Parameters.AddWithValue("@leadTime", leadTime);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@salesman", salesman);
                //In progress status
                sql.Parameters.AddWithValue("@status", 2);
                if(keepQuoteNum != "yes")
                {
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@oldQuoteNum", "");
                }
                else
                {
                    sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(version) + 1).ToString("00#"));
                    sql.Parameters.AddWithValue("@oldQuoteNum", quoteNumber + (System.Convert.ToInt32(version) + 1).ToString("00#"));
                }
                sql.Parameters.AddWithValue("@logo", useTSG);
                sql.Parameters.AddWithValue("@toolingCost", toolingCost);
                sql.Parameters.AddWithValue("@transferBar", transferBarCost);
                sql.Parameters.AddWithValue("@fixture", fixtureCost);
                sql.Parameters.AddWithValue("@dieSupport", dieSupportCost);
                sql.Parameters.AddWithValue("@shippingCost", shippingCost);
                sql.Parameters.AddWithValue("@addCostDesc", additionalCostDesc);
                sql.Parameters.AddWithValue("@addCost", additionalCost);
                sql.Parameters.AddWithValue("@tsgName", useTSG);
                sql.Parameters.AddWithValue("@partNumbers", partNum);
                sql.Parameters.AddWithValue("@customerQuoteNumber", custRFQ);
                //USD
                sql.Parameters.AddWithValue("@currency", 1);
                sql.Parameters.AddWithValue("@accessNum", accessNum);
                sql.Parameters.AddWithValue("@shippingLocation", shippingLocation);
                sql.Parameters.AddWithValue("@plant", plant);
                sql.Parameters.AddWithValue("@quoteId", quoteID);

                string newQuoteID = master.ExecuteScalar(sql, "Copy quote to RFQ").ToString();


                //update quote number
                sql.CommandText = "update tblQuote set quoNumber = @number where quoQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@number", newQuoteID);
                master.ExecuteNonQuery(sql, "Copy quote to RFQ");

                sql.CommandText = "Insert into pktblBlankInfo (binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, ";
                sql.CommandText += "binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialWeightEnglish, binMaterialWeightMetric, binCreated, binCreatedBy) ";
                sql.CommandText += "output inserted.binBlankInfoID ";
                sql.CommandText += "Values (@matType, @thickEng, @thickMet, @pitchEng, @pitchMet, @widthEng, @widthMet, @weightEng, @weightMet, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@matType", matType);
                sql.Parameters.AddWithValue("@thickEng", matThkEng);
                sql.Parameters.AddWithValue("@thickMet", matThkMet);
                sql.Parameters.AddWithValue("@pitchEng", blankPitchEng);
                sql.Parameters.AddWithValue("@pitchMet", blankPitchMet);
                sql.Parameters.AddWithValue("@widthEng", blankWidthEng);
                sql.Parameters.AddWithValue("@widthMet", blankWidthMet);
                sql.Parameters.AddWithValue("@weightEng", 0);
                sql.Parameters.AddWithValue("@weightMet", 0);
                sql.Parameters.AddWithValue("@user", user);
                string blankInfo = master.ExecuteScalar(sql, "Copy quote to RFQ").ToString();


                sql.CommandText = "insert into tblDieInfo (dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, ";
                sql.CommandText += "dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy) ";
                sql.CommandText += "output inserted.dinDieInfoID ";
                sql.CommandText += "values (@dieType, @cavity, @fToBEng, @fToBMet, @lToREng, @lToRMet, @shutHeightEng, @shutHeightMet, @numOfStations, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@dieType", dieType);
                sql.Parameters.AddWithValue("@cavity", cavity);
                sql.Parameters.AddWithValue("@fToBEng", dieFBEng);
                sql.Parameters.AddWithValue("@fToBMet", dieFBMet);
                sql.Parameters.AddWithValue("@lToREng", dieLREng);
                sql.Parameters.AddWithValue("@lToRMet", dieLRMet);
                sql.Parameters.AddWithValue("@shutHeightEng", shutHeightEng);
                sql.Parameters.AddWithValue("@shutHeightMet", shutHeightMet);
                sql.Parameters.AddWithValue("@numOfStations", numOfStations);
                sql.Parameters.AddWithValue("@user", user);
                string dieInfo = master.ExecuteScalar(sql, "Copy quote to RFQ").ToString();


                sql.CommandText = "update tblQuote set quoBlankInfoID = @blankInfo where quoQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@blankInfo", blankInfo);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);

                master.ExecuteNonQuery(sql, "Copy Quote To RFQ");

                List<string> newpwnIDs = new List<string>();
                for (int i = 0; i < pwnIDs.Count; i++)
                {
                    sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCreated, pwnCreatedBy, pwnCostNote) ";
                    sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "Select pwnCompanyID, pwnPreWordedNote, GETDATE(), @user, pwnCostNote from pktblPreWordedNote where pwnPreWordedNoteID = @pwnID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    sql.Parameters.AddWithValue("@pwnID", pwnIDs[i]);

                    newpwnIDs.Add(master.ExecuteScalar(sql, "CopyQuoteToRFQ").ToString());

                    sql.CommandText = "insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                    sql.CommandText += "values(@quoteID, @pwnID, GETDATE(), @user)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@pwnID", newpwnIDs[i]);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }

                sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                sql.CommandText += "values(@dieInfo, @quoteID, GETDATE(), @user)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@dieInfo", dieInfo);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");

                List<string> partIDs = new List<string>();
                partIDs.Add(partID);
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartTopartID from linkPartToPartDetail where ppdPartID = @partID)  and ppdPartID <> @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader sdr = sql.ExecuteReader();
                while (sdr.Read())
                {
                    partIDs.Add(sdr.GetValue(0).ToString());
                }
                sdr.Close();

                for (int i = 0; i < partIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 0)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }


                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values(@quoteID, @rfqID, GETDATE(), @user, 0, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");

                List<string> generalNotes = new List<string>();
                sql.CommandText = "Select * from linkGeneralNoteToECQuote where gneECQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    generalNotes.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for(int i = 0; i < generalNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                    sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    string temp2222 = generalNotes[i];
                    sql.Parameters.AddWithValue("@note", generalNotes[i].ToString());
                    sql.Parameters.AddWithValue("@quote", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

            }
            else if (hts)
            {
                List<string> note = new List<string>();
                List<string> qty = new List<string>();
                List<string> unitPrice = new List<string>();
                List<string> pwnID = new List<string>();
                string user = master.getUserName();
                double totalAmount = 0;

                sql.CommandText = "Select hpwNote, hpwQuantity, hpwUnitPrice from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote ";
                sql.CommandText += "where pthHTSPWNID = hpwHTSPreWordedNoteID and pthHTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    note.Add(dr.GetValue(0).ToString());
                    qty.Add(dr.GetValue(1).ToString());
                    unitPrice.Add(dr.GetValue(2).ToString());
                    totalAmount += System.Convert.ToDouble(dr.GetValue(1).ToString()) * System.Convert.ToDouble(dr.GetValue(2).ToString());
                }
                dr.Close();

                for (int i = 0; i < note.Count; i++)
                {
                    sql.CommandText = "insert into pktblHTSPreWordedNote (hpwCompanyId, hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                    sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                    sql.CommandText += "values (@company, @note, @qty, @unitPrice, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", 9);
                    sql.Parameters.AddWithValue("@note", note[i]);
                    sql.Parameters.AddWithValue("@qty", qty[i]);
                    sql.Parameters.AddWithValue("@unitPrice", unitPrice[i]);
                    sql.Parameters.AddWithValue("@user", user);

                    pwnID.Add(master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString());
                }

                string estimatorID = "";
                string version = "";
                string jobNumber = "";
                string status = "";
                string payment = "";
                string shipping = "";
                string annualVolume = "";
                string productType = "";
                string programCode = "";
                string oem = "";
                string vehicle = "";
                string quoteType = "";
                string partType = "";
                string description = "";
                string leadTime = "";
                string number = "";
                string customerQuoteNumber = "";
                Boolean TSGLogo = false;
                Boolean TSGName = false;
                string partNumbers = "";
                string currency = "";
                string process = "";
                string cavity = "";
                string partName = "";
                string access = "";
                string customerContactName = "";
                string customerRFQNum = "";
                string materialType = "";
                string winLoss = "";
                string oldQuoteNumber = "";

                sql.CommandText = "Select hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, hquShippingTerms, hquAnnualVolume, hquProductTypeID, ";
                sql.CommandText += "hquProgramCodeID, hquOEM, hquVehicleID, hquQuoteTypeID, hquPartTypeID, hquDescription, hquLeadTime, hquNumber, hquCustomerQuoteNumber, ";
                sql.CommandText += "hquUseTSGLogo, hquUseTSGName, hquPartNumbers, hquCurrencyID, hquProcess, hquCavity, hquPartName, hquAccess, hquCustomerContactName, ";
                sql.CommandText += "hquCustomerRFQNum, hquMaterialType, hquWinLossID, prtRFQLineNumber, qtrRFQID ";
                sql.CommandText += "from tblHTSQuote ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = hquHTSQuoteID and qtrHTS = 1 ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = hquHTSQuoteID and ptqHTS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "where hquHTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    estimatorID = dr["hquEstimatorID"].ToString();
                    version = dr["hquVersion"].ToString();
                    jobNumber = dr["hquJobNumberID"].ToString();
                    status = dr["hquStatusID"].ToString();
                    payment = dr["hquPaymentTerms"].ToString();
                    shipping = dr["hquShippingTerms"].ToString();
                    annualVolume = dr["hquAnnualVolume"].ToString();
                    productType = dr["hquProductTypeID"].ToString();
                    programCode = dr["hquProgramCodeID"].ToString();
                    oem = dr["hquOEM"].ToString();
                    vehicle = dr["hquVehicleID"].ToString();
                    quoteType = dr["hquQuoteTypeID"].ToString();
                    partType = dr["hquPartTypeID"].ToString();
                    description = dr["hquDescription"].ToString();
                    leadTime = dr["hquLeadTime"].ToString();
                    number = dr["hquNumber"].ToString();
                    customerQuoteNumber = dr["hquCustomerQuoteNumber"].ToString();
                    TSGLogo = dr.GetBoolean(17);
                    TSGName = dr.GetBoolean(18);
                    partNumbers = dr["hquPartNumbers"].ToString();
                    currency = dr["hquCurrencyID"].ToString();
                    process = dr["hquProcess"].ToString();
                    cavity = dr["hquCavity"].ToString();
                    partName = dr["hquPartName"].ToString();
                    access = dr["hquAccess"].ToString();
                    customerContactName = dr["hquCustomerContactName"].ToString();
                    customerRFQNum = dr["hquCustomerRFQNum"].ToString();
                    materialType = dr["hquMaterialType"].ToString();
                    winLoss = dr["hquWinLossID"].ToString();
                    if (keepQuoteNum == "yes")
                    {
                        int num;
                        bool results = Int32.TryParse(dr["hquNumber"].ToString(), out num);
                        if (!results)
                        {
                            oldQuoteNumber = dr["hquNumber"].ToString();
                        }
                        else
                        {
                            oldQuoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString();
                        }
                    }
                }
                dr.Close();

                string dueDate = "";
                string customer = "";
                string plant = "";
                string salesman = "";
                string picture = "";
                string customerContact = "";
                sql.CommandText = "Select rfqDueDate, rfqCustomerID, rfqPlantID, rfqSalesman, prtPicture, Name, rfqCustomerRFQNumber, prtPartNumber, prtpartDescription from tblRFQ, tblPart, linkPartToRFQ, CustomerContact "; 
                sql.CommandText += "where rfqID = @rfqID and prtPartID = prtPARTID and ptrRFQID = rfqID and prtPARTID = @partID and rfqCustomerContact = CustomerContactID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dueDate = dr["rfqDueDate"].ToString();
                    customer = dr["rfqCustomerID"].ToString();
                    plant = dr["rfqPlantID"].ToString();
                    salesman = dr["rfqSalesman"].ToString();
                    picture = dr["prtPicture"].ToString();
                    customerContact = dr["Name"].ToString();
                    customerRFQNum = dr["rfqCustomerRFQNumber"].ToString();
                    partNumbers = dr["prtPartNumber"].ToString();
                    partName = dr["prtpartDescription"].ToString();
                }
                dr.Close();                

                sql.CommandText = "insert into tblHTSQuote (hquRFQID, hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, hquShippingTerms, ";
                sql.CommandText += "hquTotalAmount, hquAnnualVolume, hquProductTypeID, hquProgramCodeID, hquOEM, hquVehicleID, hquDueDate, hquQuoteTypeID, hquPartTypeID, ";
                sql.CommandText += "hquCreated, hquCreatedBy, hquWinLossID, hquDescription, hquLeadTime, hquSalesman, hquNumber, hquCustomerQuoteNumber, hquUseTSGLogo, ";
                sql.CommandText += "hquUseTSGName, hquPartNumbers, hquCurrencyID, hquCustomerID, hquCustomerLocationID, hquProcess, hquCavity, hquPartName, hquPicture, ";
                sql.CommandText += "hquAccess, hquCustomerContactName, hquCustomerRFQNum, hquMaterialType, hquCoppiedFromQuote) ";
                sql.CommandText += "output inserted.hquHTSQuoteID ";
                sql.CommandText += "values (@rfqID, @estimator, @version, @jobNum, @status, @payment, @shipping, @totalAmount, @annualVolume, @productType, @program, @oem, ";
                sql.CommandText += "@vehicle, @dueDate, @quoteType, @partType, GETDATE(), @user, @winLoss, @description, @leadTime, @salesman, @number, @custQuoteNum, @logo, ";
                sql.CommandText += "@TSGName, @partNumbers, @currency, @customer, @plant, @process, @cavity, @partName, @picture, @access, @contactName, @rfqNum, @material, @quoteId) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@estimator", estimatorID);

                if (keepQuoteNum != "yes")
                {
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@number", "");
                }
                else
                {
                    sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(version) + 1).ToString("00#"));
                    sql.Parameters.AddWithValue("@number", oldQuoteNumber);
                }

                sql.Parameters.AddWithValue("@jobNum", jobNumber);
                //In progress status
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@payment", payment);
                sql.Parameters.AddWithValue("@shipping", shipping);
                sql.Parameters.AddWithValue("@totalAmount", totalAmount);
                sql.Parameters.AddWithValue("@annualVolume", annualVolume);
                sql.Parameters.AddWithValue("@productType", productType);
                sql.Parameters.AddWithValue("@program", programCode);
                sql.Parameters.AddWithValue("@oem", oem);
                sql.Parameters.AddWithValue("@vehicle", vehicle);
                sql.Parameters.AddWithValue("@dueDate", dueDate);
                sql.Parameters.AddWithValue("@quoteType", quoteType);
                sql.Parameters.AddWithValue("@partType", partType);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@description", description);
                sql.Parameters.AddWithValue("@leadTime", leadTime);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custQuoteNum", customerQuoteNumber);
                sql.Parameters.AddWithValue("@logo", TSGLogo);
                sql.Parameters.AddWithValue("@TSGName", TSGName);
                sql.Parameters.AddWithValue("@partNumbers", partNumbers);
                sql.Parameters.AddWithValue("@currency", currency);
                sql.Parameters.AddWithValue("@customer", customer);
                sql.Parameters.AddWithValue("@plant", plant);
                sql.Parameters.AddWithValue("@process", process);
                sql.Parameters.AddWithValue("@cavity", cavity);
                sql.Parameters.AddWithValue("@partName", partName);
                sql.Parameters.AddWithValue("@picture", picture);
                sql.Parameters.AddWithValue("@access", access);
                sql.Parameters.AddWithValue("@contactName", customerContact);
                sql.Parameters.AddWithValue("@rfqNum", customerRFQNum);
                sql.Parameters.AddWithValue("@material", materialType);
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                string newQuoteID = master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString();

                if(keepQuoteNum != "yes")
                {
                    sql.CommandText = "update tblHTSQuote set hquNumber = @quoteNumber where hquHTSQuoteID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteNumber", newQuoteID);
                    sql.Parameters.AddWithValue("@id", newQuoteID);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                for(int i = 0; i < pwnID.Count; i++)
                {
                    sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                    sql.CommandText += "values (@quoteID, @pwnID, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@pwnID", pwnID[i]);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteScalar(sql, "Copy Quote to RFQ");
                }

                List<string> generalNotes = new List<string>();
                sql.CommandText = "Select gnqGeneralNoteID from linkGeneralNoteToQuote where gnqQuoteID = @quoteID and gnqHTS = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    generalNotes.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < generalNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                    sql.CommandText += "values (@generalNote, @quote, GETDATE(), @user, 1) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@generalNote", generalNotes[i]);
                    sql.Parameters.AddWithValue("@quote", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 1, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");

                List<string> partIDs = new List<string>();
                sql.CommandText = "select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail ";
                sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < partIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 1, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @user, 1, 0, 0) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
            }
            else if (sts)
            {
                string user = master.getUserName();
                List<string> note = new List<string>();
                List<string> cost = new List<string>();

                sql.CommandText = "Select pwnPreWordedNote, pwnCostNote from pktblPreWordedNote, linkPWNToSTSQuote ";
                sql.CommandText += "where pwnPreWordedNoteID = psqPreWordedNoteID and psqSTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    note.Add(dr.GetValue(0).ToString());
                    cost.Add(dr.GetValue(1).ToString());
                }
                dr.Close();

                List<string> pwn = new List<string>();
                for(int i = 0; i < note.Count; i++)
                {
                    sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCreated, pwnCreatedBy, pwnCostNote) ";
                    sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "values (13, @note, GETDATE(), @user, @cost) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@note", note[i]);
                    sql.Parameters.AddWithValue("@user", user);
                    sql.Parameters.AddWithValue("@cost", cost[i]);
                    pwn.Add(master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString());
                }


                string version = "";
                string status = "";
                string partNumber = "";
                string partName = "";
                string customerContact = "";
                string estimator = "";
                string eav = "";
                string process = "";
                string machineTime = "";
                string shipping = "";
                string payment = "";
                string leadTime = "";
                string jobNum = "";
                string useTSG = "";
                string oldQuoteNumber = "";

                sql.CommandText = "Select squQuoteVersion, squStatusID, squPartNumber, squPartName, squCustomerContact, squEstimatorID, squEAV, squProcess, ";
                sql.CommandText += "squMachineTime, squShippingID, squPaymentID, squLeadTime, squJobNum, squUseTSG, squQuoteNumber, prtRFQLineNumber, qtrRFQID ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                sql.CommandText += "where squSTSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    version = dr["squQuoteVersion"].ToString();
                    status = dr["squStatusID"].ToString();
                    partNumber = dr["squPartNumber"].ToString();
                    partName = dr["squPartName"].ToString();
                    customerContact = dr["squCustomerContact"].ToString();
                    estimator = dr["squEstimatorID"].ToString();
                    eav = dr["squEAV"].ToString();
                    process = dr["squProcess"].ToString();
                    machineTime = dr["squMachineTime"].ToString();
                    shipping = dr["squShippingID"].ToString();
                    payment = dr["squPaymentID"].ToString();
                    leadTime = dr["squLeadTime"].ToString();
                    jobNum = dr["squJobNum"].ToString();
                    useTSG = dr["squUseTSG"].ToString();

                    if (keepQuoteNum == "yes")
                    {
                        int num;

                        bool results = Int32.TryParse(dr["squQuoteNumber"].ToString(), out num);
                        if(!results)
                        {
                            oldQuoteNumber = dr["squQuoteNumber"].ToString();
                        }
                        else
                        {
                            oldQuoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString();
                        }
                    }
                }
                dr.Close();


                string dueDate = "";
                string customer = "";
                string plant = "";
                string salesman = "";
                string picture = "";
                string name = "";
                string customerRFQNumber = "";
                sql.CommandText = "Select rfqDueDate, rfqCustomerID, rfqPlantID, rfqSalesman, prtPicture, Name, rfqCustomerRFQNumber ";
                sql.CommandText += "from tblRFQ, tblPart, linkPartToRFQ, CustomerContact ";
                sql.CommandText += "where rfqID = @rfqID and prtPartID = prtPARTID and ptrRFQID = rfqID and prtPARTID = @partID and rfqCustomerContact = CustomerContactID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dueDate = dr["rfqDueDate"].ToString();
                    customer = dr["rfqCustomerID"].ToString();
                    plant = dr["rfqPlantID"].ToString();
                    salesman = dr["rfqSalesman"].ToString();
                    picture = dr["prtPicture"].ToString();
                    name = dr["Name"].ToString();
                    customerRFQNumber = dr["rfqCustomerRFQNumber"].ToString();
                }
                dr.Close();

                sql.CommandText = "insert into tblSTSQuote (squQuoteVersion, squStatusID, squPartNumber, squPartName, squRFQNum, squCustomerID, squPlantID, ";
                sql.CommandText += "squCustomerContact, squSalesmanID, squCustomerRFQNum, squEstimatorID, squEAV, squProcess, squMachineTime, squShippingID, ";
                sql.CommandText += "squPaymentID, squLeadTime, squJobNum, squCreated, squCreatedBy, squUseTSG, squQuoteNumber, squCoppiedFromQuote) ";
                sql.CommandText += "output inserted.squSTSQuoteID ";
                sql.CommandText += "values (@version, @status, @partNum, @partName, @rfqNum, @customer, @plant, @customerContact, @salesman, @custRFQNum, ";
                sql.CommandText += "@estimator, @eav, @process, @machineTime, @shipping, @payment, @leadTime, @jobNum, GETDATE(), @user, @TSG, @quoteNumber, @quoteId) ";
                sql.Parameters.Clear();

                if (keepQuoteNum == "yes")
                {
                    sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(version) + 1).ToString("00#"));
                    sql.Parameters.AddWithValue("@quoteNumber", oldQuoteNumber);
                }
                else
                {
                    sql.Parameters.AddWithValue("@version", "001");
                    sql.Parameters.AddWithValue("@quoteNumber", "");
                }
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@partNum", partNumber);
                sql.Parameters.AddWithValue("@partName", partName);
                sql.Parameters.AddWithValue("@rfqNum", rfqID);
                sql.Parameters.AddWithValue("@customer", customer);
                sql.Parameters.AddWithValue("@plant", plant);
                sql.Parameters.AddWithValue("@customerContact", name);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@custRFQNum", customerRFQNumber);
                sql.Parameters.AddWithValue("@estimator", estimator);
                sql.Parameters.AddWithValue("@eav", eav);
                sql.Parameters.AddWithValue("@process", process);
                sql.Parameters.AddWithValue("@machineTime", machineTime);
                sql.Parameters.AddWithValue("@shipping", shipping);
                sql.Parameters.AddWithValue("@payment", payment);
                sql.Parameters.AddWithValue("@leadTime", leadTime);
                sql.Parameters.AddWithValue("@jobNum", jobNum);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@TSG", useTSG);
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                string newQuoteID = master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString();

                if (keepQuoteNum != "yes")
                {
                    sql.CommandText = "Update tblSTSQuote set squQuoteNumber = @quoteNumber where squSTSQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@quoteNumber", newQuoteID);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                for (int i = 0; i < pwn.Count; i++)
                {
                    sql.CommandText = "insert into linkPWNToSTSQuote (psqSTSQuoteID, psqPreWordedNoteID, psqCreated, psqCreatedBy) ";
                    sql.CommandText += "values (@quoteID, @pwn, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@pwn", pwn[i]);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 1, 0) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");

                List<string> partIDs = new List<string>();
                //This will give us any partIDs that we should link this quote to that we havent already linked it to
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail ";
                sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < partIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 1, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @user, 0, 1, 0) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");

                List<string> generalNote = new List<string>();
                sql.CommandText = "Select gnsGeneralNoteID from linkGeneralNoteToSTSQuote where gnsSTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    generalNote.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < generalNote.Count; i++)
                {
                    sql.CommandText = "insert into linkGeneralNoteToSTSQuote (gnsGeneralNoteID, gnsSTSQuoteID, gnsCreated, gnsCreatedBy) ";
                    sql.CommandText += "values (@generalNote, @quote, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@generalNote", generalNote[i]);
                    sql.Parameters.AddWithValue("@quote", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }
            }
            else if (ugs)
            {
                string costID = "";
                string user = master.getUserName();
                string oldQuoteNumber = "";
                string version = "";

                Boolean reserved = false;
                sql.CommandText = "Select prcPartReservedToCompanyID from linkPartReservedToCompany where prcPartID = @partID and prcRFQID = @rfqID and prcTSGCompanyID = 15 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr.GetValue(0).ToString() != "")
                    {
                        reserved = true;
                    }
                }
                dr.Close();

                if (!reserved)
                {
                    sql.CommandText = "insert into linkPartReservedToCompany (prcPartID, prcTSGCompanyID, prcCreated, prcCreatedBy, prcRFQID) ";
                    sql.CommandText += "values (@partID, @company, GETDATE(), @user, @rfqID) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    sql.Parameters.AddWithValue("@company", 15);
                    sql.Parameters.AddWithValue("@user", user);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    master.ExecuteNonQuery(sql, "Copy Quote To RFQ");
                }

                sql.CommandText = "Select uquUGSCostID, uquQuoteNumber, qtrRFQID, prtRFQLineNumber, uquQuoteVersion ";
                sql.CommandText += "from tblUGSQuote ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = uquUGSQuoteID and qtrUGS = 1 ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                sql.CommandText += "where uquUGSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    costID = dr.GetValue(0).ToString();
                    version = dr["uquQuoteVersion"].ToString();
                    int num;
                    bool results = Int32.TryParse(dr["uquQuoteNumber"].ToString(), out num);
                    if (!results)
                    {
                        oldQuoteNumber = dr["uquQuoteNumber"].ToString();
                    }
                    else
                    {
                        oldQuoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString();
                    }
                }
                dr.Close();

                sql.CommandText = "insert into pktblUGSCost (ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ";
                sql.CommandText += "ucoCNC, ucoCertification, ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ucoSPC, ucoGageRRFixtures, ";
                sql.CommandText += "ucoAssemble, ucoPallets, ucoTransportation, ucoBasePlate, ucoAluminum, ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ";
                sql.CommandText += "ucoClamps, ucoIndicator, ucoIndCollar, ucoIndStorCase, ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ";
                sql.CommandText += "ucoJigFeet, ucoToolingBalls, ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ";
                sql.CommandText += "ucoHeatTreat, ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ucoWelding, ucoWireBurn, ucoRebates, ucoCreated, ";
                sql.CommandText += "ucoCreatedBy, ucoCost) ";
                sql.CommandText += "output inserted.ucoUGSCostID ";
                sql.CommandText += "Select ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ucoCNC, ucoCertification, ";
                sql.CommandText += "ucoGageRRCMM, ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ";
                sql.CommandText += "ucoTransportation, ucoBasePlate, ucoAluminum, ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ";
                sql.CommandText += "ucoIndCollar, ucoIndStorCase, ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ucoToolingBalls, ";
                sql.CommandText += "ucoTBCovers, ucoTBPads, ucoSlides, ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ucoHeatTreat, ucoEngrvdTags, ";
                sql.CommandText += "ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ucoWelding, ucoWireBurn, ucoRebates, GETDATE(), @user, ucoCost ";
                sql.CommandText += "from pktblUGSCost where ucoUGSCostID = @costID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@costID", costID);
                sql.Parameters.AddWithValue("@user", user);
                try
                {
                    costID = master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString();
                }
                catch (Exception ex)
                {
                    return;
                }

                string customer = "";
                string plant = "";
                string salesman = "";
                string picture = "";
                string customerContactName = "";
                string customerRFQNumber = "";
                sql.CommandText = "Select rfqCustomerID, rfqPlantID, rfqSalesman, prtPicture, Name, rfqCustomerRFQNumber from tblRFQ, tblPart, linkPartToRFQ, CustomerContact ";
                sql.CommandText += "where rfqID = @rfq and ptrPartID = prtPARTID and ptrRFQID = rfqID and prtPARTID = @partID and rfqCustomerContact = CustomerContactID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    customer = dr["rfqCustomerID"].ToString();
                    plant = dr["rfqPlantID"].ToString();
                    salesman = dr["rfqSalesman"].ToString();
                    picture = dr["prtPicture"].ToString();
                    customerContactName = dr["Name"].ToString();
                    customerRFQNumber = dr["rfqCustomerRFQNumber"].ToString();
                }
                dr.Close();


                sql.CommandText = "insert into tblUGSQuote (uquRFQID, uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquCustomerContact, uquSalesmanID, ";
                sql.CommandText += "uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquUseTSG, uquNotes, uquTotalPrice, ";
                sql.CommandText += "uquDieType, uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC, uquCertification, ";
                sql.CommandText += "uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins, uquSPC, uquGageRRFixtures, uquWood, uquBushings, ";
                sql.CommandText += "uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar, uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, ";
                sql.CommandText += "uquRisers, uquHandles, uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI, uquAnnodizing, ";
                sql.CommandText += "uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping, uquThirdPartyCMM, uquWelding, uquWireBurn, ";
                sql.CommandText += "uquRebates, uquUGSCostID, uquShippingLocation, uquPartLength, uquPartWidth, uquPartHeight, uquCustomerID, uquPlantID, uquCreated, ";
                sql.CommandText += "uquCreatedBy, uquQuoteNumber, uquPicture, uquCoppiedFromQuote) ";
                sql.CommandText += "output inserted.uquUGSQuoteID ";
                sql.CommandText += "Select @rfqID, @version, @status, uquPartNumber, uquPartName, @customerContact, @salesman, @customerRFQNum, uquEstimatorID, uquShippingID, ";
                sql.CommandText += "uquPaymentID, uquLeadTime, uquJobNumber, uquUseTSG, uquNotes, uquTotalPrice, uquDieType, uquManagement, uquProjectEng, uquReadData, ";
                sql.CommandText += "uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC, uquCertification, uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, ";
                sql.CommandText += "uquLocationPins, uquGoNoGoPins, uquSPC, uquGageRRFixtures, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar, ";
                sql.CommandText += "uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles, uquJigFeet, uquToolingBalls, ";
                sql.CommandText += "uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI, uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, ";
                sql.CommandText += "uquCNCServices, uquGrinding, uquShipping, uquThirdPartyCMM, uquWelding, uquWireBurn, uquRebates, @costID, uquShippingLocation, ";
                sql.CommandText += "uquPartLength, uquPartWidth, uquPartHeight, @customer, @plant, GETDATE(), @user, @quoteNumber, @picture, uquUGSQuoteID ";
                sql.CommandText += "from tblUGSQuote where uquUGSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                if (keepQuoteNum == "yes")
                {
                    sql.Parameters.AddWithValue("@quoteNumber", oldQuoteNumber);
                    sql.Parameters.AddWithValue("@version", (System.Convert.ToInt32(version) + 1).ToString("00#"));
                }
                else
                {
                    sql.Parameters.AddWithValue("@quoteNumber", "");
                    sql.Parameters.AddWithValue("@version", "001");
                }
                // In Process status
                sql.Parameters.AddWithValue("@status", 2);
                sql.Parameters.AddWithValue("@customerContact", customerContactName);
                sql.Parameters.AddWithValue("@salesman", salesman);
                sql.Parameters.AddWithValue("@customerRFQNum", customerRFQNumber);
                sql.Parameters.AddWithValue("@costID", costID);
                sql.Parameters.AddWithValue("@customer", customer);
                sql.Parameters.AddWithValue("@plant", plant);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@picture", picture);
                string newQuoteID = "";
                try
                {
                    newQuoteID = master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString();
                }
                catch (Exception ex)
                {
                    return;
                }

                if (keepQuoteNum != "yes")
                {
                    sql.CommandText = "update tblUGSQuote set uquQuoteNumber = @quoteNumber where uquUGSQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@quoteNumber", newQuoteID);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                List<string> note = new List<string>();
                List<string> cost = new List<string>();
                sql.CommandText = "Select pwnPreWordedNote, pwnCostNote from linkPWNToUGSQuote, pktblPreWordedNote ";
                sql.CommandText += "where puqUGSQuoteID = @quoteID and puqPreWordedNoteID = pwnPreWordedNoteID and pwnCompanyID = 15 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    note.Add(dr.GetValue(0).ToString());
                    cost.Add(dr.GetValue(1).ToString());
                }
                dr.Close();

                for(int i = 0; i < note.Count; i++)
                {
                    sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCreated, pwnCreatedBy, pwnCostNote) ";
                    sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                    sql.CommandText += "values (15, @note, GETDATE(), @user, @cost) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@note", note[i]);
                    sql.Parameters.AddWithValue("@user", user);
                    sql.Parameters.AddWithValue("@cost", cost[i]);
                    string pwn = master.ExecuteScalar(sql, "Copy Quote to RFQ").ToString();

                    sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                    sql.CommandText += "values (@pwn, @quote, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@pwn", pwn);
                    sql.Parameters.AddWithValue("@quote", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 0, 1) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");

                List<string> partIDs = new List<string>();
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail ";
                sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < partIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 0, 1) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }

                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @user, 0, 0, 1) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Copy Quote to RFQ");

                List<string> generalNote = new List<string>();
                sql.CommandText = "Select gnuGeneralNoteID from linkGeneralNoteToUGSQuote where gnuUGSQuoteID = @quoteID order by gnuGeneralNoteID ASC ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    generalNote.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < generalNote.Count; i++)
                {
                    sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                    sql.CommandText += "values (@generalNote, @quote, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@generalNote", generalNote[i]);
                    sql.Parameters.AddWithValue("@quote", newQuoteID);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Copy Quote to RFQ");
                }
            }
            else
            {
                //Due date needs to be modified!!!
                sql.CommandText = "Select 1, quoNumber from tblQuote where quoQuoteID = @id";
                sql.Parameters.AddWithValue("@id", quoteID);
                //sql.Parameters.AddWithValue("@num", quoteNum);
                SqlDataReader dr = sql.ExecuteReader();

                int count = 0;
                if (dr.Read())
                {
                    count = System.Convert.ToInt32(dr.GetValue(0));
                    quoteNum = dr.GetValue(1).ToString();
                }
                dr.Close();
                sql.Parameters.Clear();

                if (count != 0)
                {
                    string dieInfoID = "";
                    string salesman = "";
                    List<string> pwnIDs = new List<string>();
                    List<string> newpwnIDs = new List<string>();
                    sql.CommandText = "Select diqDieInfoID from linkDieInfoToQuote where diqQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);

                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        dieInfoID = dr.GetValue(0).ToString();
                    }
                    dr.Close();

                    sql.CommandText = "Select rfqSalesman from tblRFQ where rfqID = @id ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", rfqID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        salesman = dr.GetValue(0).ToString();
                    }
                    dr.Close();

                    sql.CommandText = "Select pwqPreWordedNoteID from linkPWNToQuote where pwqQuoteID = @quoteID order by pwqPreWordedNoteID ASC ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        pwnIDs.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    int version = 0;
                    int blankInfoID = 0;
                    string partName = "";
                    string customerContactName = "";
                    string jobNum = "";
                    sql.CommandText = "Select quoVersion, quoBlankInfoID, quoPartName, quoCustomerContact, quoJobNum from tblQuote where quoQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        version = System.Convert.ToInt32(dr.GetValue(0));
                        blankInfoID = System.Convert.ToInt32(dr.GetValue(1));
                        partName = dr.GetValue(2).ToString();
                        customerContactName = dr.GetValue(3).ToString();
                        jobNum = dr.GetValue(0).ToString();
                    }
                    version++;
                    dr.Close();

                    string oldQuoteNumber = "";
                    if(keepQuoteNum == "yes")
                    {
                        sql.CommandText = "Select qtrRFQID, prtRFQLineNumber, quoOldQuoteNumber from linkPartToQuote, tblPart, linkQuoteToRFQ, tblQuote where qtrQuoteID = @quoteID and ";
                        sql.CommandText += "qtrQuoteID = ptqQuoteID and ptqPartID = prtPARTID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and qtrHTS = 0 ";
                        sql.CommandText += "and qtrSTS = 0 and qtrUGS = 0 and qtrQuoteID = quoQuoteID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            if (dr.GetValue(2).ToString() != "")
                            {
                                oldQuoteNumber = dr.GetValue(2).ToString();
                            }
                            else
                            {
                                oldQuoteNumber = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString();
                            }
                        }
                        dr.Close();
                    }
                    else
                    {
                        version = 1;
                    }

                    sql.CommandText = "INSERT INTO tblQuote ([quoTSGCompanyID],[quoRFQID],[quoEstimatorID],[quoVersion],[quoJobNumberID],[quoEstimatedPODate],[quoStatusID],[quoPaymentTermsID],[quoShippingTermsID],[quoCustomerPaymentTermsID],[quoTotalAmount] ";
                    sql.CommandText += ",[quoAnnualVolume],[quoProductTypeID],[quoProgramCodeID],[quoOEMID],[quoVehicleID],[quoDueDate],[quoBidDate],[quoQuoteTypeID],[quoToolTypeID],[quoPartTypeID],[quoToolCountryID],[quoNoQuote],[quoNoQuoteReasonID],[quoTargetPrice] ";
                    sql.CommandText += ",[quoDataTransaction],[quoWinLossID],[quoWinLossReasonID],[quoDescription],[quoLeadTime],[quoSalesman],[quoNumber],[quoCreated],[quoCreatedBy],[quoCustomerQuoteNumber],[quoUseTSGLogo],[quoToolingCost],[quoTransferBarCost], ";
                    sql.CommandText += "[quoFixtureCost],[quoDieSupportCost],[quoShippingCost],[quoAdditCostDesc],[quoAdditCost],[quoUseTSGName],[quoPartNumbers],[quoCurrencyID],[quoAccess],[quoShippingLocation],[quoOldQuoteNumber],[quoPartName],[quoCustomerContact],[quoJobNum], [quoCoppiedFromQuote], [quoCoppiedFromEC]) ";
                    sql.CommandText += "output inserted.quoQuoteID ";
                    sql.CommandText += "Select quoTSGCompanyID, @rfqID, quoEstimatorID, @version, quoJobNumberID, quoEstimatedPODate, quoStatusID, quoPaymentTermsID, quoShippingTermsID, quoCustomerPaymentTermsID, quoTotalAmount, quoAnnualVolume, ";
                    sql.CommandText += "quoProductTypeID, quoProgramCodeID, quoOEMID, quoVehicleID, rfqDueDate, rfqBidDate, quoQuoteTypeID, quoToolTypeID, quoPartTypeID, quoToolCountryID, quoNoQuote, quoNoQuoteReasonID, quoTargetPrice, quoDataTransaction, ";
                    sql.CommandText += "quoWinLossID, quoWinLossReasonID, quoDescription, quoLeadTime, @salesman, quoNumber, GETDATE(), @user, quoCustomerQuoteNumber, quoUseTSGLogo, quoToolingCost, quoTransferBarCost, quoFixtureCost, quoDieSupportCost, ";
                    sql.CommandText += "quoShippingCost, quoAdditCostDesc, quoAdditCost, quoUseTSGName, quoPartNumbers, quoCurrencyID, quoAccess, quoShippingLocation, @oldQuoteNumber, @quoPartName, @contactName, @jobNum, quoQuoteID, 0 from tblQuote, tblRFQ where quoQuoteID = @quoteID and rfqID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@version", version.ToString("00#"));
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    sql.Parameters.AddWithValue("@oldQuoteNumber", oldQuoteNumber);
                    sql.Parameters.AddWithValue("@quoPartName", partName);
                    sql.Parameters.AddWithValue("@contactName", customerContactName);
                    sql.Parameters.AddWithValue("@jobNum", jobNum);
                    sql.Parameters.AddWithValue("@salesman", salesman);

                    //Quote is now in we still need to link everything
                    int newQuoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "CopyQuoteToRFQ"));

                    sql.Parameters.Clear();

                    sql.CommandText = "INSERT INTO tblDieInfo ([dinDieType],[dinCavityID],[dinSizeFrontToBackEnglish],[dinSizeFrontToBackMetric],[dinSizeLeftToRightEnglish],[dinSizeLeftToRightMetric],[dinSizeShutHeightEnglish],[dinSizeShutHeightMetric] ";
                    sql.CommandText += ",[dinNumberOfStations],[dinCreated],[dinCreatedBy])";
                    sql.CommandText += "output inserted.dinDieInfoID ";
                    sql.CommandText += "Select dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, ";
                    sql.CommandText += "GETDATE(), @user from tblDieInfo where dinDieInfoID = @dieInfo";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    int newDieInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "CopyQuoteToRFQ"));

                    sql.Parameters.Clear();

                    sql.CommandText = "insert into pktblBlankInfo (binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, ";
                    sql.CommandText += "binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialWeightEnglish, binMaterialWeightMetric, binCreated, binCreatedBy) ";
                    sql.CommandText += "output inserted.binBlankInfoID ";
                    sql.CommandText += "Select binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, ";
                    sql.CommandText += "binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialWeightEnglish, binMaterialWeightMetric, GETDATE(), @user from pktblBlankInfo where binBlankInfoID = @blankInfo";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    sql.Parameters.AddWithValue("@blankInfo", blankInfoID);

                    blankInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Copy Quote To RFQ"));

                    

                    sql.CommandText = "update tblQuote set quoBlankInfoID = @blankInfo where quoQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@blankInfo", blankInfoID);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);

                    master.ExecuteNonQuery(sql, "Copy Quote To RFQ");

                    for (int i = 0; i < pwnIDs.Count; i++)
                    {
                        sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCreated, pwnCreatedBy, pwnCostNote) ";
                        sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                        sql.CommandText += "Select pwnCompanyID, pwnPreWordedNote, GETDATE(), @user, pwnCostNote from pktblPreWordedNote where pwnPreWordedNoteID = @pwnID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        sql.Parameters.AddWithValue("@pwnID", pwnIDs[i]);

                        newpwnIDs.Add(master.ExecuteScalar(sql, "CopyQuoteToRFQ").ToString());

                        sql.CommandText = "insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                        sql.CommandText += "values(@quoteID, @pwnID, GETDATE(), @user)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                        sql.Parameters.AddWithValue("@pwnID", newpwnIDs[i]);
                        sql.Parameters.AddWithValue("@user", master.getUserName());

                        master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                    }

                    sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                    sql.CommandText += "values(@dieInfo, @quoteID, GETDATE(), @user)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@dieInfo", newDieInfoID);
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");

                    List<string> partIDs = new List<string>();
                    partIDs.Add(partID);
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartTopartID from linkPartToPartDetail where ppdPartID = @partID)  and ppdPartID <> @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partID);
                    SqlDataReader sdr = sql.ExecuteReader();
                    while(sdr.Read())
                    {
                        partIDs.Add(sdr.GetValue(0).ToString());
                    }
                    sdr.Close();

                    for(int i = 0; i < partIDs.Count; i++)
                    {
                        sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                        sql.CommandText += "values(@partID, @quoteID, GETDATE(), @user, 0, 0, 0)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partIDs[i]);
                        sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                        sql.Parameters.AddWithValue("@user", master.getUserName());

                        master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                    }
                    

                    sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                    sql.CommandText += "values(@quoteID, @rfqID, GETDATE(), @user, 0, 0, 0)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", newQuoteID);
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    sql.Parameters.AddWithValue("@user", master.getUserName());

                    master.ExecuteNonQuery(sql, "CopyQuoteToRFQ");
                }
            }
            connection.Close();
        }
    }
}