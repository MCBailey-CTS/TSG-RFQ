using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.POIFS.FileSystem;
//using OfficeOpenXmlCrypto;

namespace RFQ
{
    public class QuoteUpload : IHttpHandler
    {
        List<string> quoteIDs = new List<string>();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            if (context.Request.Files.Count <= 0)
            {
                context.Response.Write("No file uploaded");
            }
            else
            {
                for (int i = 0; i < context.Request.Files.Count; ++i)
                {
                    HttpPostedFile file = context.Request.Files[i];
                    Int64 rfq = 0;
                    try
                    {
                        rfq = System.Convert.ToInt64(context.Request["rfqID"]);
                    }
                    catch
                    {
                        string name = context.Request.Files[i].FileName;
                        string rfqidstr = name.Substring(14);
                        rfq = System.Convert.ToInt64(rfqidstr);
                    }

                    //try
                    //{
                        
                    //    using (OfficeCryptoStream streamtest = OfficeCryptoStream.Open("C:\\Onedrive\\OneDrive - Tooling Systems Group\\downloads\\rfq password test.xlsx", "test123"))
                    //    {
                    //        // Do stuff (e.g. create System.IO.Packaging.Package or 
                    //        // ExcelPackage from the stream, make changes and save)

                    //        // Change the password (optional)
                    //        //stream.Password = "newPassword";

                    //        // Encrypt and save the file
                    //        streamtest.Save();
                    //        //NPOIFSFileSystem fs = new NPOIFSFileSystem(streamtest);
                    //        XSSFWorkbook wb = new XSSFWorkbook(streamtest);
                    //        XSSFSheet sh;
                    //        sh = (XSSFSheet)wb.GetSheet("encryption test");
                    //        if (sh != null)
                    //        {
                    //            Console.Write("great sucess");
                    //        }
                    //    }
                        


                    //}
                    //catch(Exception er)
                    //{
                    //    context.Response.Write(er);
                    //}

                    try
                    {
                        Boolean processed = false;
                        XSSFWorkbook wb = new XSSFWorkbook(file.InputStream);
                        XSSFSheet sh;
                        sh = (XSSFSheet)wb.GetSheet("RFQ Upload Sheet");
                        if(sh != null)
                        {
                            ProcessWorkSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if(!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("Quote Sheet");
                        }
                        if(!processed && sh != null)
                        {
                            processNewQuoteSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("HTS Quote Sheet");
                        }
                        if (!processed && sh != null)
                        {
                            processHTSQuoteSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("E-C Upload Sheet");
                        }
                        if (!processed && sh != null)
                        {
                            ProcessATSEC(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("EC");
                        }
                        if (!processed && sh != null)
                        {
                            processEC(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("UGS Quote Sheet");
                        }
                        if (!processed && sh != null)
                        {
                            ugsQuoteSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("SA Quote Sheet");
                        }
                        if (!processed && sh != null)
                        {
                            ProcessSAWorkSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            sh = (XSSFSheet)wb.GetSheet("NC Quote Sheet");
                        }
                        if (!processed && sh != null)
                        {
                            ProcessNCWorkSheet(context, sh, rfq, file.FileName);
                            processed = true;
                        }
                        if (!processed)
                        {
                            context.Response.Write(file.FileName + " - Please make sure this is a quote file ");
                        }
                    }
                    catch (Exception er)
                    {
                        context.Response.Write("Please make sure your upload is not password protected and is saved as a .xlsm");
                    }
                    //if(i > 20)
                    //{
                    //    break;
                    //}
                }
            }
        }

        // required for a handler apparently
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ugsQuoteSheet(HttpContext context, XSSFSheet sh, long rfq, string fileName)
        {
            int errorFlag = 0;
            int i = 6;
            Site master = new Site();

            Boolean done = false;
            try
            {
                if (sh != null)
                {
                    SqlCommand sql = new SqlCommand();
                    SqlConnection connection = new SqlConnection(master.getConnectionString());
                    connection.Open();
                    sql.Connection = connection;
                    List<string> generalNote = new List<string>();

                    if (i == 6)
                    {
                        for (int gen = 6; gen < 5000; gen++)
                        {
                            if (sh.GetRow(gen) != null)
                            {
                                try
                                {
                                    if (master.readCellString(sh.GetRow(gen).GetCell(2)) == "Select (X)")
                                    {
                                        for (int k = gen + 1; k < gen + 40; k++)
                                        {
                                            if (sh.GetRow(k) != null && master.readCellString(sh.GetRow(k).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) != null)
                                            {
                                                if (master.readCellString(sh.GetRow(k).GetCell(2)).ToLower() == "x")
                                                {
                                                    sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoGeneralNote = @genNote and gnoCompany = 'UGS' ";
                                                    sql.Parameters.Clear();
                                                    sql.Parameters.AddWithValue("@genNote", master.readCellString(sh.GetRow(k).GetCell(3)));
                                                    SqlDataReader genNoteDR = sql.ExecuteReader();
                                                    if (genNoteDR.Read())
                                                    {
                                                        generalNote.Add(genNoteDR.GetValue(0).ToString());
                                                    }
                                                    genNoteDR.Close();
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }


                    //These are the variables that will be the same for every quote
                    Boolean tsgLogo = false;
                    Boolean tsgName = false;
                    if (master.readCellString(sh.GetRow(0).GetCell(3)) == "TSG")
                    {
                        tsgLogo = true;
                    }
                    if (master.readCellString(sh.GetRow(0).GetCell(5)) == "TSG")
                    {
                        tsgName = true;
                    }

                    string user = master.getUserName();
                    string plantCode = "";
                    string plantID = "";
                    string customerID = "";
                    string customerContact = master.readCellString(sh.GetRow(3).GetCell(5));
                    string shippingTerms = "";
                    string paymentTerms = "";
                    string salesmanID = "";

                    sql.CommandText = "Select rfqCustomerID, rfqPlantID, ShipCode, rfqSalesman from tblRFQ, CustomerLocation where rfqID = @rfqID and rfqPlantID = CustomerLocationID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfq);
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        customerID = dr["rfqCustomerID"].ToString();
                        plantID = dr["rfqPlantID"].ToString();
                        plantCode = dr["ShipCode"].ToString();
                        salesmanID = dr["rfqSalesman"].ToString();
                    }
                    dr.Close();

                    if (master.readCellInt(sh.GetRow(3).GetCell(4)) != System.Convert.ToInt32(plantCode))
                    {
                        sql.CommandText = "Select CustomerLocationID from CustomerLocation where CustomerID = @customer and ShipCode = @code ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@customer", customerID);
                        sql.Parameters.AddWithValue("@code", master.readCellInt(sh.GetRow(3).GetCell(4)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            plantID = dr["CustomerLocationID"].ToString();
                        }
                        dr.Close();
                    }

                    sql.CommandText = "Select ptePaymentTermsID, steShippingTermsID from pktblPaymentTerms, pktblShippingTerms where ptePaymentTerms = @paymentTerms and steShippingTerms = @shippingTerms ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(3).GetCell(6)));
                    sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(3).GetCell(7)));
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        shippingTerms = dr["steShippingTermsID"].ToString();
                        paymentTerms = dr["ptePaymentTermsID"].ToString();
                    }
                    dr.Close();

                    if (customerID == "")
                    {
                        context.Response.Write("There was an issue finding the customer.  Please fix and reupload");
                        done = true;
                    }
                    if (plantID == "")
                    {
                        context.Response.Write("There was an issue getting the plant.  Please make sure the plant code is valid and reupload");
                        done = true;
                    }
                    if (shippingTerms == "")
                    {
                        context.Response.Write("There was an issue getting the shipping terms.  Please make sure they are filled out correctly and reupload");
                        done = true;
                    }
                    if (paymentTerms == "")
                    {
                        context.Response.Write("There was an issue getting the payment terms.  Please make sure they are filledout out correctly and reupload");
                        done = true;
                    }
                    if (salesmanID == "")
                    {
                        //context.Response.Write("There was an issue getting the salesman for this quote.  Please contact an administrator to make sure")
                    }

                    while (sh.GetRow(i) != null && !done)
                    {
                        string customerRFQNum = customerRFQNum = master.readCellString(sh.GetRow(i).GetCell(1));
                        if (customerRFQNum == "" && master.readCellInt(sh.GetRow(i).GetCell(1)) != -1)
                        {
                            customerRFQNum = master.readCellInt(sh.GetRow(i).GetCell(1)).ToString();
                        }

                        int lineNum = master.readCellInt(sh.GetRow(i).GetCell(2));
                        string partID = "";
                        string picture = "";

                        sql.CommandText = "Select prtPARTID, prtPicture from tblPart, linkPartToRFQ where prtRFQLineNumber = @lineNum and ptrPartID = prtPARTID and ptrRFQID = @rfq ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        sql.Parameters.AddWithValue("@lineNum", lineNum);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            partID = dr["prtPARTID"].ToString();
                            picture = dr["prtPicture"].ToString();
                        }
                        dr.Close();

                        string partNum = master.readCellString(sh.GetRow(i).GetCell(3));
                        if (partNum == "" && master.readCellDouble(sh.GetRow(i).GetCell(3)) != -1)
                        {
                            partNum = master.readCellDouble(sh.GetRow(i).GetCell(3)).ToString();
                        }
                        string partName = master.readCellString(sh.GetRow(i).GetCell(4));
                        if (partName == "" && master.readCellDouble(sh.GetRow(i).GetCell(4)) != -1)
                        {
                            partName = master.readCellDouble(sh.GetRow(i).GetCell(4)).ToString();
                        }

                        string quoteType = "";
                        string estimator = "";

                        sql.CommandText = "Select DieTypeID from DieType where TSGCompanyID = 15 and dtyFullName = @quoteType ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteType", master.readCellString(sh.GetRow(i).GetCell(5)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            quoteType = dr["DieTypeID"].ToString();
                        }
                        dr.Close();

                        sql.CommandText = "Select estEstimatorID from pktblEstimators where estCompanyID = 15 and concat(estFirstName, ' ', estLastName) = @estimator ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@estimator", master.readCellString(sh.GetRow(i).GetCell(6)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            estimator = dr["estEstimatorID"].ToString();
                        }
                        dr.Close();

                        string length = "0";
                        string width = "0";
                        string height = "0";
                        try
                        {
                            length = ugsReadDouble(sh.GetRow(i).GetCell(7)).ToString();
                        }
                        catch
                        {
                            
                        }
                        try
                        {
                            width = ugsReadDouble(sh.GetRow(i).GetCell(8)).ToString();
                        }
                        catch
                        {

                        }
                        try
                        {
                            height = ugsReadDouble(sh.GetRow(i).GetCell(9)).ToString();   
                        }
                        catch
                        {

                        }
                        string leadTime = "";
                        try
                        {
                            leadTime = master.readCellString(sh.GetRow(i).GetCell(10));
                            if (leadTime == "" && ugsReadDouble(sh.GetRow(i).GetCell(10)) != 0)
                            {
                                leadTime = ugsReadDouble(sh.GetRow(i).GetCell(10)).ToString();
                            }
                        }
                        catch
                        {

                        }
                        string jobNum = "";
                        try
                        {
                            jobNum = master.readCellString(sh.GetRow(i + 2).GetCell(0));
                            if (jobNum == "" && ugsReadDouble(sh.GetRow(i + 2).GetCell(0)) != 0)
                            {
                                jobNum = ugsReadDouble(sh.GetRow(i + 2).GetCell(0)).ToString();
                            }
                        }
                        catch
                        {

                        }
                        double totalCost = 0;
                        try
                        {
                            totalCost = ugsReadDouble(sh.GetRow(i + 2).GetCell(1));
                        }
                        catch
                        {

                        }
                        string shippingLocation = "";
                        try
                        {
                            shippingLocation = master.readCellString(sh.GetRow(i + 2).GetCell(2));
                        }
                        catch
                        {

                        }
                        double holes = 0;
                        try
                        {
                            holes = ugsReadDouble(sh.GetRow(i + 2).GetCell(3));
                        }
                        catch
                        {

                        }

                        if (partID == "")
                        {
                            context.Response.Write("There was an issue linking this quote properly to the part.  Please make sure that the part line item number is on the quote.");
                            break;
                        }
                        if (quoteType == "")
                        {
                            context.Response.Write("You did not sleect a valid quote type for line item " + lineNum + " please fix this and reupload.");
                            break;
                        }
                        if (estimator == "")
                        {
                            context.Response.Write("Please make sure an estimator was selected for line item " + lineNum + " please fix and reupload.");
                        }

                        Boolean newVersion = false;
                        if (master.readCellString(sh.GetRow(i + 3).GetCell(1)) == "Yes")
                        {
                            newVersion = true;
                        }
                        //Going to the start of the notes
                        i += 4;

                        string oldQuoteID = "";
                        //We want 001 for the first version
                        int version = 1;
                        string number = "";
                        sql.CommandText = "Select ptqQuoteID, uquQuoteVersion, uquQuoteNumber from linkPartToQuote, tblUGSQuote where ptqPartID = @partID and ";
                        sql.CommandText += "ptqQuoteID = uquUGSQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 1 order by uquQuoteVersion desc ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partID);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            oldQuoteID = dr["ptqQuoteID"].ToString();
                            //This will get us to the next version we want to use so we add 1 to the version we currently have
                            version = System.Convert.ToInt32(dr["uquQuoteVersion"].ToString());
                            version++;
                            number = dr["uquQuoteNumber"].ToString();
                        }
                        dr.Close();

                        //If we arn't createing a new version of the quote or we dont have a current quote uploaded to that part
                        if (newVersion || oldQuoteID == "")
                        {
                            List<string> pwnID = new List<string>();
                            for (int j = 0; j < 200; j++)
                            {
                                try
                                {
                                    if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Labor")
                                    {
                                        break;
                                    }
                                    sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                    sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                                    sql.CommandText += "values (15, @note, @cost, GETDATE(), @user) ";
                                    sql.Parameters.Clear();
                                    string note = master.readCellString(sh.GetRow(i).GetCell(2));
                                    if (note == "" && ugsReadDouble(sh.GetRow(i).GetCell(2)) != 0)
                                    {
                                        note = master.readCellDouble(sh.GetRow(i).GetCell(2)).ToString();
                                    }
                                    sql.Parameters.AddWithValue("@note", note);
                                    double cost = ugsReadDouble(sh.GetRow(i).GetCell(9));
                                    sql.Parameters.AddWithValue("@cost", cost);
                                    sql.Parameters.AddWithValue("@user", user);

                                    if (note != "" || cost != 0)
                                    {
                                        pwnID.Add(master.ExecuteScalar(sql, "Quote Upload").ToString());
                                    }
                                }
                                catch (Exception err)
                                {

                                }

                                i++;
                            }
                            i++;
                            int startBudget = i;
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
                            sql.Parameters.AddWithValue("@Management", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@BasePlate", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@Annodizing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@ProjectEng", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Aluminum", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@BlackOxide", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Steel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@HeatTreat", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@ReadData", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@FixturePlank", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@EngrvdTags", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@3dModel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Wood", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@CNCServices", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Drawing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Bushings", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@Grinding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Updates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@DrillBlanks", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@Shipping", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Clamps", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@ThirdPartyCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Programming", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Indicator", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@Welding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@CNC", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@IndCollar", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@WireBurn", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@IndStorCase", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Certification", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@ZeroSet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@gageRRCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@SpcTriggers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            sql.Parameters.AddWithValue("@Rebates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@PartLayouts", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@TempDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@HingeDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Base", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Risers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Details", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Handles", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@LocationPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@JigFeet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@GoNoGoPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@ToolingBalls", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@SPC", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@TBCovers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@GageRRFixtures", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@TBPads", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Assemble", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Slides", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Magnets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Pallets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@Hardware", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Transportation", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                            sql.Parameters.AddWithValue("@LMI", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@Cost", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(11))), 2));
                            decimal temp = Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(11))), 2);
                            sql.Parameters.AddWithValue("@CreatedBy", user);
                            int costID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Quote Upload").ToString());

                            i = startBudget;
                            sql.CommandText = "INSERT INTO tblUGSQuote (uquQuoteVersion, uquStatusID, uquPartNumber, uquPartName, uquRFQID, uquCustomerID, uquPlantID, uquCustomerContact, ";
                            sql.CommandText += "uquSalesmanID, uquCustomerRFQNumber, uquEstimatorID, uquShippingID, uquPaymentID, uquLeadTime, uquJobNumber, uquTotalPrice, uquUseTSG, uquNotes, ";
                            sql.CommandText += "uquCreated, uquCreatedBy, uquShippingLocation, uquDieType, uquPicture, ";
                            sql.CommandText += "uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC,  ";
                            sql.CommandText += "uquCertification, uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins,  ";
                            sql.CommandText += "uquSPC, uquGageRRFixtures, uquAssemble, uquPallets, uquTransportation, uquBasePlate, uquAluminum,  ";
                            sql.CommandText += "uquSteel, uquFixturePlank, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar,  ";
                            sql.CommandText += "uquIndStorCase, uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles,  ";
                            sql.CommandText += "uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, uquMagnets, uquHardware, uquLMI,  ";
                            sql.CommandText += "uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping,  ";
                            sql.CommandText += "uquThirdPartyCMM, uquWelding, uquWireBurn, uquRebates, uquUGSCostID, uquPartLength, uquPartWidth, uquPartHeight, uquHoles) ";
                            sql.CommandText += "output inserted.uquUGSQuoteID ";
                            sql.CommandText += "VALUES (@version, @status, @partNum, @partName, @rfq, @customer, @plant, @contact, @salesman, @custRFQNum, @estimator, ";
                            sql.CommandText += "@shipping, @payment, @leadtime, @jobNum, @total, @logo, @notes, GETDATE(), @createdBy, @shippingLocation, @quoteType, @picture, ";
                            sql.CommandText += "@management, @projectEng, @readData, @model, @drawing, @updates, @programming, @cnc, @certification, @gageRRCMM, @partLayouts, ";
                            sql.CommandText += "@base, @details, @locationPins, @goNoGoPins, @spc, @gageRRFixtures, @assemble, @pallets, @transportation, @basePlate, ";
                            sql.CommandText += "@aluminum, @steel, @fixturePlank, @wood, @bushings, @drillBlanks, @clamps, @indicator, @indCollar, @indStorCase, @zeroSet, ";
                            sql.CommandText += "@spcTriggers, @tempDrops, @hingeDrops, @risers, @handles, @jigFeet, @toolingBalls, @tbCovers, @tbPads, @slides, @magnets, ";
                            sql.CommandText += "@hardware, @LMI, @annodizing, @blackOxide, @heatTreat, @engrvdTags, @cncServices, @grinding, @shippingCalc, @thirdPartyCMM, ";
                            sql.CommandText += "@welding, @wireBurn, @rebates, @ugsCostID, @partLength, @partWidth, @partHeight, @holes) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@version", version.ToString("00#"));
                            sql.Parameters.AddWithValue("@status", 2);
                            sql.Parameters.AddWithValue("@partNum", partNum);
                            sql.Parameters.AddWithValue("@partName", partName);
                            sql.Parameters.AddWithValue("@rfq", rfq);
                            sql.Parameters.AddWithValue("@customer", customerID);
                            sql.Parameters.AddWithValue("@plant", plantID);
                            sql.Parameters.AddWithValue("@contact", customerContact);
                            sql.Parameters.AddWithValue("@salesman", salesmanID);
                            sql.Parameters.AddWithValue("@custRFQNum", customerRFQNum);
                            sql.Parameters.AddWithValue("@estimator", estimator);
                            sql.Parameters.AddWithValue("@shipping", shippingTerms);
                            sql.Parameters.AddWithValue("@payment", paymentTerms);
                            sql.Parameters.AddWithValue("@leadTime", leadTime);
                            sql.Parameters.AddWithValue("@jobNum", jobNum);
                            sql.Parameters.AddWithValue("@total", totalCost);
                            sql.Parameters.AddWithValue("@logo", tsgLogo);
                            sql.Parameters.AddWithValue("@notes", "");
                            sql.Parameters.AddWithValue("@createdBy", user);
                            sql.Parameters.AddWithValue("@shippingLocation", shippingLocation);
                            sql.Parameters.AddWithValue("@quoteType", quoteType);
                            sql.Parameters.AddWithValue("@picture", picture);
                            sql.Parameters.AddWithValue("@holes", holes);

                            sql.Parameters.AddWithValue("@management", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@basePlate", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@annodizing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@projectEng", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@aluminum", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@blackOxide", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@steel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@heatTreat", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@readData", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@fixturePlank", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@engrvdTags", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@model", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@wood", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@cncServices", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@drawing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@bushings", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@grinding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@updates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@drillBlanks", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@shippingCalc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@clamps", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@thirdPartyCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@programming", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@indicator", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@welding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@cnc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@indCollar", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@wireBurn", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@indStorCase", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@certification", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@zeroSet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@gageRRCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@spcTriggers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@rebates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@partLayouts", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tempDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@hingeDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@base", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@risers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@details", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@handles", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@locationPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@jigFeet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@goNoGoPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@toolingBalls", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@spc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tbCovers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@GageRRFixtures", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tbPads", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@assemble", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@slides", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@magnets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@pallets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@hardware", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@transportation", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@lmi", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));

                            sql.Parameters.AddWithValue("@ugsCostID", costID);
                            sql.Parameters.AddWithValue("@partLength", length);
                            sql.Parameters.AddWithValue("@partWidth", width);
                            sql.Parameters.AddWithValue("@partHeight", height);
                            string ugsQuoteID = master.ExecuteScalar(sql, "Quote Upload").ToString();

                            sql.CommandText = "update tblUGSQuote set uquQuoteNumber = @number where uquUGSQuoteID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@number", ugsQuoteID);
                            sql.Parameters.AddWithValue("@id", ugsQuoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            for (int j = 0; j < pwnID.Count; j++)
                            {
                                sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                                sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@note", pwnID[j]);
                                sql.Parameters.AddWithValue("@quote", ugsQuoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }

                            sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                            sql.CommandText += "values (@quoteID, @rfqID, GETDATE(), @createdBy, 0, 0, 1) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", ugsQuoteID);
                            sql.Parameters.AddWithValue("@rfqID", rfq);
                            sql.Parameters.AddWithValue("@createdBy", user);
                            master.ExecuteNonQuery(sql, "Quote Uplaod");

                            sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                            sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 0, 1) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partID", partID);
                            sql.Parameters.AddWithValue("@quoteID", ugsQuoteID);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            List<string> partIDs = new List<string>();
                            sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail ";
                            sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partID", partID);
                            dr = sql.ExecuteReader();
                            while (dr.Read())
                            {
                                partIDs.Add(dr["ppdPartID"].ToString());
                            }
                            dr.Close();

                            for (int j = 0; j < partIDs.Count; j++)
                            {
                                sql.CommandText = "insert into linkPartTOQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @user, 0, 0, 1) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@partID", partIDs[j]);
                                sql.Parameters.AddWithValue("@quoteID", ugsQuoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }

                            for (int j = 0; j < generalNote.Count; j++)
                            {
                                sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                                sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", generalNote[j]);
                                sql.Parameters.AddWithValue("@quoteID", ugsQuoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                        }
                        //Either we are entering the initial quote or updating a current quote
                        else
                        {
                            List<string> pwnIDs = new List<string>();
                            List<string> newPWNIDs = new List<string>();
                            sql.CommandText = "Select puqPreWordedNoteID from linkPWNToUGSQuote where puqUGSQuoteID = @quoteID ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", oldQuoteID);
                            dr = sql.ExecuteReader();
                            while (dr.Read())
                            {
                                pwnIDs.Add(dr["puqPreWordedNoteID"].ToString());
                            }
                            dr.Close();

                            int pwnCount = 0;
                            for (int j = 0; j < 200; j++)
                            {
                                try
                                {
                                    if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Labor")
                                    {
                                        break;
                                    }
                                    string note = master.readCellString(sh.GetRow(i).GetCell(2));
                                    if (note == "" && ugsReadDouble(sh.GetRow(i).GetCell(2)) != 0)
                                    {
                                        note = master.readCellDouble(sh.GetRow(i).GetCell(2)).ToString();
                                    }
                                    double cost = ugsReadDouble(sh.GetRow(i).GetCell(9));
                                    if (note != "" || cost != 0)
                                    {
                                        if (pwnCount < pwnIDs.Count)
                                        {
                                            sql.CommandText = "update pktblPreWordedNote set pwnPreWordedNote = @note, pwnCostNote = @cost, pwnModified = GETDATE(), pwnModifiedBy = @user where pwnPreWordedNoteID = @id ";
                                            sql.Parameters.Clear();
                                            sql.Parameters.AddWithValue("@note", note);
                                            sql.Parameters.AddWithValue("@cost", cost);
                                            sql.Parameters.AddWithValue("@user", user);
                                            sql.Parameters.AddWithValue("@id", pwnIDs[pwnCount]);
                                            master.ExecuteNonQuery(sql, "Quote Upload");
                                            pwnCount++;
                                        }
                                        else
                                        {
                                            sql.CommandText = "insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                            sql.CommandText += "output inserted.pwnPreWordedNoteID ";
                                            sql.CommandText += "values (15, @note, @cost, GETDATE(), @user) ";
                                            sql.Parameters.Clear();
                                            sql.Parameters.AddWithValue("@note", note);
                                            sql.Parameters.AddWithValue("@cost", cost);
                                            sql.Parameters.AddWithValue("@user", user);

                                            if (note != "" || cost != 0)
                                            {
                                                newPWNIDs.Add(master.ExecuteScalar(sql, "Quote Upload").ToString());
                                            }
                                        }
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                                i++;
                            }
                            if (pwnCount + 1 < pwnIDs.Count)
                            {
                                for (int j = pwnCount + 1; j < pwnIDs.Count; j++)
                                {
                                    sql.CommandText = "Delete from linkPWNToUGSQuote where puqPreWordedNoteID = @id ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@id", pwnIDs[j]);
                                    master.ExecuteNonQuery(sql, "Quote Upload");

                                    sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @id ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@id", pwnIDs[j]);
                                    master.ExecuteNonQuery(sql, "Quote Upload");
                                }
                            }

                            for (int j = 0; j < newPWNIDs.Count; j++)
                            {
                                sql.CommandText = "insert into linkPWNToUGSQuote (puqPreWordedNoteID, puqUGSQuoteID, puqCreated, puqCreatedBy) ";
                                sql.CommandText += "values (@note, @quote, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@note", newPWNIDs[j]);
                                sql.Parameters.AddWithValue("@quote", oldQuoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            i++;
                            int startBudget = i;

                            string costID = "";
                            sql.CommandText = "Select uquUGSCostID from tblUGSQuote where uquUGSQuoteID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", oldQuoteID);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                costID = dr["uquUGSCostID"].ToString();
                            }
                            dr.Close();

                            if (costID != "")
                            {
                                sql.CommandText = "update pktblUGSCost set ucoManagement = @Management, ucoProjectEng = @ProjectEng, ucoReadData = @ReadData, uco3DModel = @3DModel, ucoDrawing = @Drawing, ucoUpdates = @Updates, ucoProgramming = @Programming, ucoCNC = @CNC, ";
                                sql.CommandText += "ucoCertification = @Certification, ucoGageRRCMM = @GageRRCMM, ucoPartLayouts = @PartLayouts, ucoBase = @Base, ucoDetails = @Details, ucoLocationPins = @LocationPins, ucoGoNoGoPins = @GoNoGoPins,  ";
                                sql.CommandText += "ucoSPC = @SPC, ucoGageRRFixtures = @GageRRFixtures, ucoAssemble = @Assemble, ucoPallets = @Pallets, ucoTransportation = @Transportation, ucoBasePlate = @BasePlate, ucoAluminum = @Aluminum,  ";
                                sql.CommandText += "ucoSteel = @Steel, ucoFixturePlank = @FixturePlank, ucoWood = @Wood, ucoBushings = @Bushings, ucoDrillBlanks = @DrillBlanks, ucoClamps = @Clamps, ucoIndicator = @Indicator, ucoIndCollar = @IndCollar,  ";
                                sql.CommandText += "ucoIndStorCase = @IndStorCase, ucoZeroSet = @ZeroSet, ucoSpcTriggers = @SpcTriggers, ucoTempDrops = @TempDrops, ucoHingeDrops = @HingeDrops, ucoRisers = @Risers, ucoHandles = @Handles, ucoJigFeet = @JigFeet,  ";
                                sql.CommandText += "ucoToolingBalls = @ToolingBalls, ucoTBCovers = @TBCovers, ucoTBPads = @TBPads, ucoSlides = @Slides, ucoMagnets = @Magnets, ucoHardware = @Hardware, ucoLMI = @LMI, ucoAnnodizing = @Annodizing,  ";
                                sql.CommandText += "ucoBlackOxide = @BlackOxide, ucoHeatTreat = @HeatTreat, ucoEngrvdTags = @EngrvdTags, ucoCNCServices = @CNCServices, ucoGrinding = @Grinding, ucoShipping = @Shipping, ucoThirdPartyCMM = @ThirdPartyCMM,  ";
                                sql.CommandText += "ucoWelding = @Welding, ucoWireBurn = @WireBurn, ucoRebates = @Rebates, ucoModified = GETDATE(), ucoModifiedBy = @user, ucoCost = @cost ";
                                sql.CommandText += "where ucoUGSCostID = @id ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@Management", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@BasePlate", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@Annodizing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@ProjectEng", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Aluminum", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@BlackOxide", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Steel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@HeatTreat", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@ReadData", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@FixturePlank", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@EngrvdTags", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@3dModel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Wood", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@CNCServices", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Drawing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Bushings", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@Grinding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Updates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@DrillBlanks", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@Shipping", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Clamps", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@ThirdPartyCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Programming", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Indicator", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@Welding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@CNC", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@IndCollar", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@WireBurn", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@IndStorCase", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Certification", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@ZeroSet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@gageRRCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@SpcTriggers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                sql.Parameters.AddWithValue("@Rebates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(9))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@PartLayouts", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@TempDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@HingeDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Base", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Risers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Details", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Handles", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@LocationPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@JigFeet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@GoNoGoPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@ToolingBalls", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@SPC", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@TBCovers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@GageRRFixtures", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@TBPads", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Assemble", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Slides", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Magnets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Pallets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@Hardware", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Transportation", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(1))), 2));
                                sql.Parameters.AddWithValue("@LMI", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(5))), 2));
                                i++;
                                sql.Parameters.AddWithValue("@Cost", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(11))), 2));
                                sql.Parameters.AddWithValue("@user", user);
                                sql.Parameters.AddWithValue("@id", costID);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }

                            i = startBudget;
                            sql.CommandText = "update tblUGSQuote set uquPartNumber = @partNum, uquPartName = @partName, uquRFQID = @rfq, uquCustomerID = @customer, uquPlantID = @plant, uquCustomerContact = @contact, ";
                            sql.CommandText += "uquSalesmanID = @salesman, uquCustomerRFQNumber = @custRFQNum, uquEstimatorID = @estimator, uquShippingID = @shipping, uquPaymentID = @payment, uquLeadTime = @leadtime, uquJobNumber = @jobNum, uquTotalPrice = @total, uquUseTSG = @logo, uquNotes = @notes,  ";
                            sql.CommandText += "uquModified = GETDATE(), uquModifiedBy = @user, uquShippingLocation = @shippingLocation, uquDieType = @quoteType,  ";
                            sql.CommandText += "uquManagement = @management, uquProjectEng = @projectEng, uquReadData = @readData, uqu3DModel = @model, uquDrawing = @drawing, uquUpdates = @updates, uquPrograming = @programming, uquCNC = @cnc,   ";
                            sql.CommandText += "uquCertification = @certification, uquGageRRCMM = @gageRRCMM, uquPartLayouts = @partLayouts, uquBase = @base, uquDetails = @details, uquLocationPins = @locationPins, uquGoNoGoPins = @goNoGoPins,   ";
                            sql.CommandText += "uquSPC = @spc, uquGageRRFixtures = @gageRRFixtures, uquAssemble = @assemble, uquPallets = @pallets, uquTransportation = @transportation, uquBasePlate = @basePlate, uquAluminum = @aluminum,   ";
                            sql.CommandText += "uquSteel = @steel, uquFixturePlank = @fixturePlank, uquWood = @wood, uquBushings = @bushings, uquDrillBlanks = @drillBlanks, uquClamps = @clamps, uquIndicator = @indicator, uquIndCollar = @indCollar,   ";
                            sql.CommandText += "uquIndStorCase = @indStorCase, uquZeroSet = @zeroSet, uquSpcTriggers = @spcTriggers, uquTempDrops = @tempDrops, uquHingeDrops = @hingeDrops, uquRisers = @risers, uquHandles = @handles,   ";
                            sql.CommandText += "uquJigFeet = @jigFeet, uquToolingBalls = @toolingBalls, uquTBCovers = @tbCovers, uquTBPads = @tbPads, uquSlides = @slides, uquMagnets = @magnets, uquHardware = @hardware, uquLMI = @LMI,   ";
                            sql.CommandText += "uquAnnodizing = @annodizing, uquBlackOxide = @blackOxide, uquHeatTreat = @heatTreat, uquEngrvdTags = @engrvdTags, uquCNCServices = @cncServices, uquGrinding = @grinding, uquShipping = @shippingCalc,   ";
                            sql.CommandText += "uquThirdPartyCMM = @thirdPartyCMM, uquWelding = @welding, uquWireBurn = @wireBurn, uquRebates = @rebates, uquPartLength = @partLength, uquPartWidth = @partWidth, uquPartHeight = @partHeight, uquHoles = @holes ";
                            sql.CommandText += "where uquUGSQuoteID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@version", version.ToString("00#"));
                            sql.Parameters.AddWithValue("@status", 2);
                            sql.Parameters.AddWithValue("@partNum", partNum);
                            sql.Parameters.AddWithValue("@partName", partName);
                            sql.Parameters.AddWithValue("@rfq", rfq);
                            sql.Parameters.AddWithValue("@customer", customerID);
                            sql.Parameters.AddWithValue("@plant", plantID);
                            sql.Parameters.AddWithValue("@contact", customerContact);
                            sql.Parameters.AddWithValue("@salesman", salesmanID);
                            sql.Parameters.AddWithValue("@custRFQNum", customerRFQNum);
                            sql.Parameters.AddWithValue("@estimator", estimator);
                            sql.Parameters.AddWithValue("@shipping", shippingTerms);
                            sql.Parameters.AddWithValue("@payment", paymentTerms);
                            sql.Parameters.AddWithValue("@leadTime", leadTime);
                            sql.Parameters.AddWithValue("@jobNum", jobNum);
                            sql.Parameters.AddWithValue("@total", totalCost);
                            sql.Parameters.AddWithValue("@logo", tsgLogo);
                            sql.Parameters.AddWithValue("@notes", "");
                            sql.Parameters.AddWithValue("@user", user);
                            sql.Parameters.AddWithValue("@shippingLocation", shippingLocation);
                            sql.Parameters.AddWithValue("@quoteType", quoteType);
                            sql.Parameters.AddWithValue("@picture", picture);
                            sql.Parameters.AddWithValue("@holes", holes);

                            sql.Parameters.AddWithValue("@management", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@basePlate", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@annodizing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@projectEng", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@aluminum", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@blackOxide", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@steel", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@heatTreat", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@readData", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@fixturePlank", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@engrvdTags", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@model", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@wood", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@cncServices", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@drawing", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@bushings", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@grinding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@updates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@drillBlanks", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@shippingCalc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@clamps", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@thirdPartyCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@programming", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@indicator", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@welding", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@cnc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@indCollar", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@wireBurn", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@indStorCase", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@certification", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@zeroSet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@gageRRCMM", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@spcTriggers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            sql.Parameters.AddWithValue("@rebates", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(10))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@partLayouts", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tempDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@hingeDrops", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@base", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@risers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@details", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@handles", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@locationPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@jigFeet", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@goNoGoPins", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@toolingBalls", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@spc", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tbCovers", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@GageRRFixtures", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@tbPads", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@assemble", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@slides", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@magnets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@pallets", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@hardware", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));
                            i++;
                            sql.Parameters.AddWithValue("@transportation", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(2))), 2));
                            sql.Parameters.AddWithValue("@lmi", Math.Round(Convert.ToDecimal(ugsReadDouble(sh.GetRow(i).GetCell(6))), 2));

                            sql.Parameters.AddWithValue("@ugsCostID", costID);
                            sql.Parameters.AddWithValue("@partLength", length);
                            sql.Parameters.AddWithValue("@partWidth", width);
                            sql.Parameters.AddWithValue("@partHeight", height);
                            sql.Parameters.AddWithValue("@id", oldQuoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from linkGeneralNoteToUGSQuote where gnuUGSQuoteID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", oldQuoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            for (int j = 0; j < generalNote.Count; j++)
                            {
                                sql.CommandText = "insert into linkGeneralNoteToUGSQuote (gnuGeneralNoteID, gnuUGSQuoteID, gnuCreated, gnuCreatedBy) ";
                                sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", generalNote[j]);
                                sql.Parameters.AddWithValue("@quoteID", oldQuoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }


                        }
                        for (int j = i; j < i + 500; j++)
                        {
                            if (sh.GetRow(i) != null)
                            {
                                //This is the last thing we should see on the upload sheet once we hit it we have no more quotes
                                if (master.readCellString(sh.GetRow(i).GetCell(2)) == "Select (X)")
                                {
                                    done = true;
                                    break;
                                }
                                //If we hit picture that means the next quote starts
                                else if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Picture")
                                {
                                    i++;
                                    break;
                                }
                            }
                            i++;
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception err)
            {
                //We just try to keep everything moving if there is a total error...
                context.Response.Write("Something went wrong trying to upload\n" + err.ToString());
            }
            
        }

        public double ugsReadDouble(NPOI.SS.UserModel.ICell c)
        {
            Site master = new Site();
            double cell = master.readCellDouble(c);
            if (cell != -1)
            {
                return cell;
            }
            return 0;
        }

        public void processHTSQuoteSheet(HttpContext context, XSSFSheet sh, Int64 rfq, string fileName)
        {
            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string user = master.getUserName();
            string plant = "";
            string program = "";
            Boolean tsgLogo = false;
            Boolean tsgName = false;
            if (master.readCellInt(sh.GetRow(2).GetCell(5)) != -1)
            {
                plant = master.readCellInt(sh.GetRow(2).GetCell(5)).ToString();
            }
            else
            {
                plant = master.readCellString(sh.GetRow(2).GetCell(5));
            }
            if (master.readCellInt(sh.GetRow(2).GetCell(7)) != -1)
            {
                program = master.readCellInt(sh.GetRow(2).GetCell(7)).ToString();
            }
            else
            {
                program = master.readCellString(sh.GetRow(2).GetCell(7));
            }
            if (master.readCellString(sh.GetRow(0).GetCell(3)) == "TSG")
            {
                tsgLogo = true;
            }
            if (master.readCellString(sh.GetRow(0).GetCell(5)) == "TSG")
            {
                tsgName = true;
            }

            int row = 5;
            List<string> generalNotes = new List<string>();
            for (int gen = 5; gen < 1000; gen++)
            {
                if (sh.GetRow(row) != null)
                {
                    try
                    {
                        if (master.readCellString(sh.GetRow(gen).GetCell(2)) == "Select (X)")
                        {
                            for (int k = gen + 1; k < gen + 40; k++)
                            {
                                if (sh.GetRow(k) != null && master.readCellString(sh.GetRow(k).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) != null)
                                {
                                    if (master.readCellString(sh.GetRow(k).GetCell(2)).ToLower() == "x")
                                    {
                                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoGeneralNote = @genNote and gnoCompany = @company";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@genNote", master.readCellString(sh.GetRow(k).GetCell(3)));
                                        sql.Parameters.AddWithValue("@company", "HTS");
                                        SqlDataReader genNotesDR = sql.ExecuteReader();
                                        if (genNotesDR.Read())
                                        {
                                            generalNotes.Add(genNotesDR.GetValue(0).ToString());
                                        }
                                        genNotesDR.Close();
                                    }
                                }
                            }
                            break;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            while (sh.GetRow(row) != null)
            {
                NPOI.SS.UserModel.IRow currentRow = sh.GetRow(row);
                string customerQuoteNumber = "";
                string lineNumber = "";
                string partNumber = "";
                string partName = "";
                string cavity = "";
                string process = "";
                string typeOfPart = "";
                string materialType = "";
                string countryOfOrigin = "";
                string estimator = "";
                string shippingTerms = "";
                string paymentTerms = "";
                string leadTime = "";
                string shippingLocation = "";
                string quoteId = "";
                string accessNum = "";
                string jobNum = "";

                customerQuoteNumber = master.readCellInt(currentRow.GetCell(1)).ToString();
                if (customerQuoteNumber == "-1")
                {
                    customerQuoteNumber = master.readCellString(currentRow.GetCell(1));
                }
                lineNumber = master.readCellInt(currentRow.GetCell(2)).ToString();
                partNumber = master.readCellInt(currentRow.GetCell(3)).ToString();
                if (partNumber == "-1")
                {
                    partNumber = master.readCellString(currentRow.GetCell(3));
                }
                partName = master.readCellInt(currentRow.GetCell(4)).ToString();
                if (partName == "-1")
                {
                    partName = master.readCellString(currentRow.GetCell(4));
                }
                cavity = master.readCellString(currentRow.GetCell(5));
                process = master.readCellString(currentRow.GetCell(6));
                typeOfPart = master.readCellString(currentRow.GetCell(7));
                materialType = master.readCellInt(currentRow.GetCell(8)).ToString();
                if (materialType == "-1")
                {
                    materialType = master.readCellString(currentRow.GetCell(8));
                }

                row += 2;
                currentRow = sh.GetRow(row);

                countryOfOrigin = master.readCellString(currentRow.GetCell(0));
                estimator = master.readCellString(currentRow.GetCell(1));
                shippingTerms = master.readCellString(currentRow.GetCell(2));
                paymentTerms = master.readCellString(currentRow.GetCell(3));
                leadTime = master.readCellInt(currentRow.GetCell(4)).ToString();
                if (leadTime == "-1")
                {
                    leadTime = master.readCellString(currentRow.GetCell(4));
                }
                shippingLocation = master.readCellString(currentRow.GetCell(5));
                quoteId = master.readCellInt(currentRow.GetCell(6)).ToString();
                if (quoteId == "-1")
                {
                    quoteId = "";
                }
                accessNum = master.readCellInt(currentRow.GetCell(7)).ToString();
                if (accessNum == "-1")
                {
                    accessNum = master.readCellString(currentRow.GetCell(7));
                }
                jobNum = master.readCellInt(currentRow.GetCell(8)).ToString();
                if (jobNum == "-1")
                {
                    jobNum = master.readCellString(currentRow.GetCell(8));
                }

                row++;
                currentRow = sh.GetRow(row);
                Boolean newVersion = false;
                if (master.readCellString(currentRow.GetCell(2)) == "Yes")
                {
                    newVersion = true;
                }

                int cavityID = 0;
                sql.CommandText = "Select cavCavityID from pktblCavity where cavCavityName = @name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", cavity);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    cavityID = System.Convert.ToInt32(dr["cavCavityID"].ToString());
                }
                dr.Close();

                int dieTypeID = 0;
                sql.CommandText = "Select DieTypeID from DieType where dtyFullName = @name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", process);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    dieTypeID = System.Convert.ToInt32(dr["DieTypeID"].ToString());
                }
                dr.Close();

                int partTypeID = 0;
                sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", typeOfPart);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    partTypeID = System.Convert.ToInt32(dr["ptyPartTypeID"].ToString());
                }
                dr.Close();

                int countryID = 0;
                sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", countryOfOrigin);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    countryID = System.Convert.ToInt32(dr["tcyToolCountryID"].ToString());
                }
                dr.Close();

                int estimatorID = 0;
                sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @email ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@email", estimator);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    estimatorID = System.Convert.ToInt32(dr["estEstimatorID"].ToString());
                }
                dr.Close();

                int shippingTermsID = 0;
                sql.CommandText = "Select steShippingTermsID from pktblShippingTerms where steShippingTerms = @ship ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ship", shippingTerms);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    shippingTermsID = System.Convert.ToInt32(dr["steShippingTermsID"].ToString());
                }
                dr.Close();

                int paymentTermsID = 0;
                sql.CommandText = "Select ptePaymentTermsID from pktblPaymentTerms where ptePaymentTerms = @terms ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@terms", paymentTerms);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    paymentTermsID = System.Convert.ToInt32(dr["ptePaymentTermsID"].ToString());
                }
                dr.Close();

                sql.CommandText = "Select ProgramID from Program where ProgramName = @name ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", program);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    program = dr["ProgramID"].ToString();
                }
                else
                {
                    program = "";
                }
                dr.Close();



                string productType = "", oem = "", vehicle = "", dueDate = "", customerID = "", salesman = "", plantID = "", customerContact = "";
                sql.CommandText = "Select rfqProductTypeID, rfqProgramID, rfqOEMID, rfqVehicleID, rfqDueDate, rfqCustomerID, rfqPlantID, rfqSalesman, Name from tblRFQ ";
                sql.CommandText += "left outer join CustomerContact on CustomerContactID = rfqCustomerContact ";
                sql.CommandText += "where rfqID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfq);
                SqlDataReader rfqDR = sql.ExecuteReader();
                if (rfqDR.Read())
                {
                    productType = rfqDR.GetValue(0).ToString();
                    if (program == "")
                    {
                        program = rfqDR.GetValue(1).ToString();
                    }
                    oem = rfqDR.GetValue(2).ToString();
                    vehicle = rfqDR.GetValue(3).ToString();
                    dueDate = rfqDR.GetValue(4).ToString();
                    customerID = rfqDR.GetValue(5).ToString();
                    plantID = rfqDR.GetValue(6).ToString();
                    salesman = rfqDR.GetValue(7).ToString();
                    customerContact = rfqDR["Name"].ToString();
                }
                rfqDR.Close();

                sql.CommandText = "Select CustomerLocationID, TSGSalesmanID from CustomerLocation where CustomerID = @customer and ShipCode = @code ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customer", customerID);
                sql.Parameters.AddWithValue("@code", plant);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    plantID = dr["CustomerLocationID"].ToString();
                    salesman = dr["TSGSalesmanID"].ToString();
                }
                dr.Close();

                string picture = "";
                string partId = "";
                sql.CommandText = "Select prtPARTID, prtPicture from tblPart ";
                sql.CommandText += "inner join linkPartToRFQ on ptrPartID = prtPARTID ";
                sql.CommandText += "where ptrRFQID = @rfq and prtRFQLineNumber = @lineNum ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfq);
                sql.Parameters.AddWithValue("@lineNum", lineNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    partId = dr["prtPARTID"].ToString();
                    picture = dr["prtPicture"].ToString();
                }
                dr.Close();


                int version = 1;
                if (quoteId != "")
                {
                    sql.CommandText = "Select hquQuoteVersion from tblHTSQuote where hquHTSQuoteID = @id ";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        version = System.Convert.ToInt32(dr["hquQuoteVersion"].ToString());
                    }
                    dr.Close();
                    if (newVersion)
                    {
                        version++;
                    }
                }

                List<int> noteIDs = new List<int>();
                row++;
                double totalAmount = 0;
                int errorFlag = 0;
                for (int k = 0; k < 100; k++)
                {
                    try
                    {
                        currentRow = sh.GetRow(row);
                        //when we find the next part number we break out of notes
                        if (currentRow != null && master.readCellString(currentRow.GetCell(2)) != "Line Number" && master.readCellString(currentRow.GetCell(2)) != "Select (X)")
                        {
                            if ((currentRow.GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || currentRow.GetCell(9, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null) && (master.readCellString(currentRow.GetCell(2)) != "Note"))
                            {
                                string description = master.readCellString(currentRow.GetCell(2));
                                double cost = master.readCellDouble(currentRow.GetCell(8));
                                double quantity = master.readCellDouble(currentRow.GetCell(7));
                                

                                if (description != "" || (cost != -1 && quantity != -1))
                                {
                                    if (quantity == -1)
                                    {
                                        quantity = 0;
                                    }
                                    if (cost == -1)
                                    {
                                        cost = 0;
                                    }

                                    totalAmount += quantity * cost;

                                    sql.CommandText = "insert into pktblHTSPreWordedNote (hpwNote, hpwQuantity, hpwUnitPrice, hpwCreated, hpwCreatedBy) ";
                                    sql.CommandText += "output inserted.hpwHTSPreWordedNoteID ";
                                    sql.CommandText += "values(@desc, @quantity, @unitPrice, GETDATE(), @user) ";
                                    
                                    sql.Parameters.AddWithValue("@desc", description);
                                    sql.Parameters.AddWithValue("@quantity", quantity);
                                    sql.Parameters.AddWithValue("@unitPrice", cost);
                                    sql.Parameters.AddWithValue("@user", user);
                                    int noteID = 0;
                                    try
                                    {
                                        noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "HTS Quote Upload"));
                                    }
                                    catch (Exception err)
                                    {
                                        errorFlag = 1;
                                        //Response.Write("<script>alert('Something went wrong trying to upload your notes, please check your upload sheet for any errors " + i + "');</script>");
                                        context.Response.Write(fileName + " - Something went wrong trying to upload your notes, please check your notes for any errors\n" + err.ToString());
                                        break;
                                    }
                                    sql.Parameters.Clear();
                                    noteIDs.Add(noteID);
                                }
                            }
                        }
                        else
                        {
                            //We either have no more rows or we hit th
                            break;
                        }
                        row++;
                    }
                    catch (Exception e)
                    {

                    }
                }
                if (errorFlag == 1)
                {
                    break;
                }
                if (estimatorID == 0)
                {
                    context.Response.Write("There was a problem getting the estimator for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (paymentTermsID == 0)
                {
                    context.Response.Write("There was a problem getting the payment terms for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (shippingTermsID == 0)
                {
                    context.Response.Write("There was a problem getting the shipping terms for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (partTypeID == 0)
                {
                    context.Response.Write("There was a problem getting the part type for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (dieTypeID == 0)
                {
                    context.Response.Write("There was a problem getting the die type for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (cavityID == 0)
                {
                    context.Response.Write("There was a problem getting the cavity for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }
                else if (countryID == 0)
                {
                    context.Response.Write("There was a problem getting the country for part " + partNumber + " please fix the issue or contact IT");
                    break;
                }


                if (quoteId == "" || newVersion)
                {
                    sql.CommandText = "INSERT INTO tblHTSQuote (hquRFQID, hquEstimatorID, hquVersion, hquJobNumberID, hquStatusID, hquPaymentTerms, ";
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
                    sql.Parameters.AddWithValue("@rfqID", rfq);
                    sql.Parameters.AddWithValue("@estID", estimatorID);
                    sql.Parameters.AddWithValue("@version", version.ToString("000"));
                    sql.Parameters.AddWithValue("@jobNum", jobNum);
                    sql.Parameters.AddWithValue("@status", 2);
                    sql.Parameters.AddWithValue("@payment", paymentTermsID);
                    sql.Parameters.AddWithValue("@shipping", shippingTermsID);
                    sql.Parameters.AddWithValue("@totalAmount", totalAmount);
                    sql.Parameters.AddWithValue("@annualAmount", 0);
                    sql.Parameters.AddWithValue("@productType", productType);
                    sql.Parameters.AddWithValue("@program", program);
                    sql.Parameters.AddWithValue("@oem", oem);
                    sql.Parameters.AddWithValue("@vehicle", vehicle);
                    sql.Parameters.AddWithValue("@dueDate", dueDate);
                    // Quote type 8 is hot stamp
                    sql.Parameters.AddWithValue("@quoteType", 8);
                    sql.Parameters.AddWithValue("@partType", partTypeID);
                    sql.Parameters.AddWithValue("@createdBy", user);
                    sql.Parameters.AddWithValue("@winLoss", "");
                    sql.Parameters.AddWithValue("@desc", "");
                    sql.Parameters.AddWithValue("@leadTime", leadTime);
                    sql.Parameters.AddWithValue("@salesman", salesman);
                    sql.Parameters.AddWithValue("@number", "");
                    sql.Parameters.AddWithValue("@useTSGLogo", tsgLogo);
                    sql.Parameters.AddWithValue("@useTSGName", tsgName);
                    sql.Parameters.AddWithValue("@partNums", partNumber);
                    sql.Parameters.AddWithValue("@currency", 1);
                    sql.Parameters.AddWithValue("@cust", customerID);
                    sql.Parameters.AddWithValue("@custLoc", plantID);
                    sql.Parameters.AddWithValue("@process", dieTypeID);
                    sql.Parameters.AddWithValue("@cavity", cavityID);
                    sql.Parameters.AddWithValue("@partName", partName);
                    sql.Parameters.AddWithValue("@picture", picture);
                    sql.Parameters.AddWithValue("@access", accessNum);
                    sql.Parameters.AddWithValue("@customerContact", customerContact);
                    sql.Parameters.AddWithValue("@custRFQ", customerQuoteNumber);
                    sql.Parameters.AddWithValue("@matType", materialType);
                    quoteId = master.ExecuteScalar(sql, "HTS Quote Upload").ToString();
                }
                else
                {
                    sql.CommandText = "update tblHTSQuote set hquPaymentTerms = @payment, hquShippingTerms = @shipping, hquQuoteTypeID = @quoteType, hquPartTypeID = @partType, hquLeadTime = @leadtime, ";
                    sql.CommandText += "hquNumber = @number, hquUseTSGLogo = @logo, hquUseTSGName = @name, hquPartNumbers = @nums, hquCustomerID = @cust, hquCustomerLocationID = @loc, ";
                    sql.CommandText += "hquProcess = @process, hquCavity = @cavity, hquPartName = @partName, hquModified = GETDATE(), hquModifiedBy = @modifiedBy, hquAccess = @access, hquCustomerContactName = @customerContact, hquCustomerRFQNum = @custRFQ, ";
                    sql.CommandText += "hquJobNumberID = @jobNum, hquMaterialType = @matType, hquSalesman = @salesman where hquHTSQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@payment", paymentTermsID);
                    sql.Parameters.AddWithValue("@shipping", shippingTermsID);
                    sql.Parameters.AddWithValue("@quoteType", 8);
                    sql.Parameters.AddWithValue("@partType", partTypeID);
                    sql.Parameters.AddWithValue("@leadTime", leadTime);
                    sql.Parameters.AddWithValue("@number", "");
                    sql.Parameters.AddWithValue("@logo", tsgLogo);
                    sql.Parameters.AddWithValue("@name", tsgName);
                    sql.Parameters.AddWithValue("@nums", partNumber);
                    sql.Parameters.AddWithValue("@cust", customerID);
                    sql.Parameters.AddWithValue("@loc", plantID);
                    sql.Parameters.AddWithValue("@process", dieTypeID);
                    sql.Parameters.AddWithValue("@cavity", cavityID);
                    sql.Parameters.AddWithValue("@partName", partName);
                    sql.Parameters.AddWithValue("@modifiedBy", user);
                    sql.Parameters.AddWithValue("@access", accessNum);
                    sql.Parameters.AddWithValue("@customerContact", customerContact);
                    sql.Parameters.AddWithValue("@custRFQ", customerQuoteNumber);
                    sql.Parameters.AddWithValue("@jobNum", jobNum);
                    sql.Parameters.AddWithValue("@matType", materialType);
                    sql.Parameters.AddWithValue("@salesman", salesman);
                    sql.Parameters.AddWithValue("@quoteID", quoteId);

                    master.ExecuteNonQuery(sql, "HTS Edit Quote");

                    sql.CommandText = "delete from linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteId);
                    master.ExecuteNonQuery(sql, "HTS Quote Upload");
                }

                sql.CommandText = "update tblHTSQuote set hquNumber = @quoteID where hquHTSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteId);
                sql.Parameters.AddWithValue("@id", quoteId);
                master.ExecuteNonQuery(sql, "HTS Quote Upload");

                for (int i = 0; i < noteIDs.Count; i++)
                {
                    sql.CommandText = "insert into linkHTSPWNToHTSQuote (pthHTSQuoteID, pthHTSPWNID, pthCreated, pthCreatedBy) ";
                    sql.CommandText += "values (@quote, @pwn, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quote", quoteId);
                    sql.Parameters.AddWithValue("@pwn", noteIDs[i]);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "HTS Quote Upload");
                }

                sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
                sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partId);
                sql.Parameters.AddWithValue("@quoteID", quoteId);
                sql.Parameters.AddWithValue("@createdBy", user);
                sql.Parameters.AddWithValue("@hts", true);
                master.ExecuteNonQuery(sql, "HTS Quote Upload");

                List<string> partIds = new List<string>();
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPartToPartDetail ";
                sql.CommandText += "where ppdPartID = @partID) and ppdPartID <> @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partId);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partIds.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < partIds.Count; i++)
                {
                    sql.CommandText = "insert into linkPartToQuote ( ptqPArtID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS ) ";
                    sql.CommandText += "Values (@partID, @quoteID, GETDATE(), @createdBy, @hts, 0, 0) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIds[i]);
                    sql.Parameters.AddWithValue("@quoteID", quoteId);
                    sql.Parameters.AddWithValue("@createdBy", user);
                    sql.Parameters.AddWithValue("@hts", true);
                    master.ExecuteNonQuery(sql, "HTS Quote Upload");
                }

                sql.CommandText = "insert into linkQuoteToRFQ (qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                sql.CommandText += "Values (@quoteID, @rfqID, GETDATE(), @createdBy, @hts, 0, 0) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteId);
                sql.Parameters.AddWithValue("@rfqID", rfq);
                sql.Parameters.AddWithValue("@createdBy", user);
                sql.Parameters.AddWithValue("@hts", true);
                master.ExecuteNonQuery(sql, "HTS Quote Upload");


                for (int i = 0; i < generalNotes.Count; i++)
                {
                    sql.CommandText = "insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                    sql.CommandText += "values (@note, @quote, GETDATE(), @user, 1) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@note", generalNotes[i]);
                    sql.Parameters.AddWithValue("@quote", quoteId);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "HTS Quote Upload");
                }

                if (master.readCellString(sh.GetRow(row).GetCell(2)) == "Select (X)")
                {
                    break;
                }
                row++;
            }

            connection.Close();
        }

        public void processNewQuoteSheet(HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 5; // skip the header rows
            int SalesOrderNumber = 0;
            Site master = new RFQ.Site();

            if(sh != null)
            {
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                List<string> generalNotes = new List<string>();
                while (sh.GetRow(i) != null)
                {
                    row = i;
                    if (i > 5000)
                    {
                        return;
                    }
                    try
                    {
                        if (SalesOrderNumber != master.readCellInt(sh.GetRow(2).GetCell(2)))
                        {
                            q = new Quote();
                            d = new DieInfo();
                        }
                        SqlCommand sql = new SqlCommand();
                        sql.Connection = connection;

                        //string adfasdfasdfasdfasdfas = master.readCellString(sh.GetRow(i - 1).GetCell(2));

                        try
                        {
                            if (master.readCellString(sh.GetRow(i - 1).GetCell(2)) == "Select (X)")
                            {
                                break;
                            }
                        }
                        catch { }
                        try
                        {
                            if (master.readCellString(sh.GetRow(i).GetCell(2)) == "Select (X)")
                            {
                                break;
                            }
                        }
                        catch { }

                        if (i == 5)
                        {
                            for (int gen = 5; gen < 1000; gen++)
                            {
                                if (sh.GetRow(i) != null)
                                {
                                    try
                                    {
                                        if (master.readCellString(sh.GetRow(gen).GetCell(2)) == "Select (X)")
                                        {
                                            for (int k = gen + 1; k < gen + 40; k++)
                                            {
                                                if (sh.GetRow(k) != null && master.readCellString(sh.GetRow(k).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) != null)
                                                {
                                                    if (master.readCellString(sh.GetRow(k).GetCell(2)).ToLower() == "x")
                                                    {
                                                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoGeneralNote = @genNote and gnoCompany = @company";
                                                        sql.Parameters.Clear();
                                                        sql.Parameters.AddWithValue("@genNote", master.readCellString(sh.GetRow(k).GetCell(3)));
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
                                                        SqlDataReader genNotesDR = sql.ExecuteReader();
                                                        if (genNotesDR.Read())
                                                        {
                                                            generalNotes.Add(genNotesDR.GetValue(0).ToString());
                                                        }
                                                        genNotesDR.Close();
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }

                        q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());

                        sql.CommandText = "Select ptePaymentTermsID, steShippingTermsID from pktblPaymentTerms, pktblShippingTerms where ";
                        sql.CommandText += "ptePaymentTerms = @paymentTerms and steShippingTerms = @shippingTerms";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i + 2).GetCell(7)));
                        sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i + 2).GetCell(6)));

                        string tempp = master.readCellString(sh.GetRow(i + 2).GetCell(7));
                        string temp2 = master.readCellString(sh.GetRow(i + 2).GetCell(6));

                        string steShipp = master.readCellString(sh.GetRow(i + 2).GetCell(6));
                        q.ShippingTerms = 0;
                        q.PaymentTerms = 0;
                        SqlDataReader dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                            q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(1));
                        }
                        dr.Close();

                        sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @lastName";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@lastName", master.readCellString(sh.GetRow(i + 2).GetCell(5)));
                        q.EstimatorID = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(7)));
                        q.PartType = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i + 2).GetCell(4)));
                        q.ToolCountry = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.Parameters.Clear();
                        sql.CommandText = "Select curCurrencyID, meaMeasurementID from pktblCurrency, pktblMeasurement where curCurrency = @cur ";
                        sql.CommandText += "and meaMeasurement = @mea";
                        sql.Parameters.AddWithValue("@cur", master.readCellString(sh.GetRow(i + 4).GetCell(9)));
                        sql.Parameters.AddWithValue("@mea", master.readCellString(sh.GetRow(i + 2).GetCell(9)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.currency = dr.GetValue(0).ToString();
                            q.measurement = dr.GetValue(1).ToString();
                        }
                        dr.Close();

                        //double FormSteelCoating = 0;
                        //double Qdc = 0;
                        //double EaryParts = 0;
                        //double Finance = 0;
                        //double Spare = 0;
                        //try
                        //{
                        //    FormSteelCoating = master.readCellDouble(sh.GetRow(i + 4).GetCell(7));
                        //    Qdc = master.readCellDouble(sh.GetRow(i + 6).GetCell(2));
                        //    EaryParts = master.readCellDouble(sh.GetRow(i + 6).GetCell(0));
                        //    Finance = master.readCellDouble(sh.GetRow(i + 6).GetCell(1));
                        //    Spare = master.readCellDouble(sh.GetRow(i + 6).GetCell(3));
                        //}
                        //catch
                        //{

                        //}

                        sql.Parameters.Clear();
                        sql.CommandText = "Select TSGSalesmanID, rfqCustomerID from CustomerLocation, tblRFQ where CustomerLocationID = rfqPlantID ";
                        sql.CommandText += "and rfqID = @rfq ";
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        dr = sql.ExecuteReader();
                        int salesmanID = 0;
                        int customerID = 0;
                        if (dr.Read())
                        {
                            salesmanID = System.Convert.ToInt32(dr.GetValue(0));
                            customerID = System.Convert.ToInt32(dr.GetValue(1));
                        }
                        dr.Close();

                        q.RFQID = System.Convert.ToInt32(rfq);

                        q.LeadTime = 0;
                        q.LeadTime = master.readCellInt(sh.GetRow(i + 2).GetCell(8));

                        if (q.LeadTime == -1)
                        {
                            q.LeadTimeString = master.readCellString(sh.GetRow(i + 2).GetCell(8));
                        }
                        SalesOrderNumber = master.readCellInt(sh.GetRow(i).GetCell(2));

                        int version = 0;
                        int quoteID = 0;
                        int number = 0;
                        //sql.CommandText = "Select count(ptqQuoteID), ptqQuoteID from linkPartToQuote where ptqPartID = @partID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 Group By ptqQuoteID";
                        //sql.Parameters.AddWithValue("@partID", partID);

                        //dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    version = System.Convert.ToInt32(dr.GetValue(0));
                        //    quoteID = System.Convert.ToInt32(dr.GetValue(1));
                        //}
                        //dr.Close();
                        //sql.Parameters.Clear();


                        //sql.CommandText = "Select quoNumber from tblQuote where quoQuoteID = @quoteID";
                        //sql.Parameters.AddWithValue("@quoteID", quoteID);
                        //dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    number = System.Convert.ToInt32(dr.GetValue(0));
                        //}
                        //dr.Close();
                        //sql.Parameters.Clear();

                        sql.Parameters.Clear();
                        sql.CommandText = "select prtPartID from tblPart, linkPartToRFQ where prtRFQLineNumber = @lineNum and ptrPartID = prtPartID ";
                        sql.CommandText += "and ptrRFQID = @rfq";
                        sql.Parameters.AddWithValue("@lineNum", master.readCellInt(sh.GetRow(i).GetCell(2)).ToString());
                        sql.Parameters.AddWithValue("@rfq", q.RFQID);
                        dr = sql.ExecuteReader();

                        //string tempdafsdfasdfa = master.readCellString(sh.GetRow(i).GetCell(2));
                        //string tempdafsdfasadfasdfdfa = master.readCellInt(sh.GetRow(i).GetCell(2)).ToString();

                        while (dr.Read())
                        {
                            partID = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        string temp = master.readCellString(sh.GetRow(i).GetCell(2));

                        string oldQuoteNumber = "", oldQuoteCompany = "", oldQuoteID = "";

                        sql.Parameters.Clear();
                        sql.CommandText = "Select ptqQuoteID, quoTSGCompanyID, quoVersion, quoNumber from linkPartToQuote, tblQuote where ptqPartID = @partID and ";
                        sql.CommandText += "ptqQuoteID = quoQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and quoTSGCompanyID = @companyID order by quoVersion desc ";
                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            oldQuoteID = dr.GetValue(0).ToString();
                            oldQuoteCompany = dr.GetValue(1).ToString();
                            version = System.Convert.ToInt32(dr.GetValue(2).ToString());
                            number = System.Convert.ToInt32(dr.GetValue(3).ToString());
                            //if (dr.GetValue(1).ToString() == master.getCompanyId().ToString())
                            //{
                            //    deleteQuote(dr.GetValue(0).ToString());
                            //}
                        }
                        dr.Close();
                        sql.Parameters.Clear();

                        if (oldQuoteID != "" && oldQuoteCompany == master.getCompanyId().ToString())
                        {
                            string tempQuoteID = "";
                            sql.CommandText = "Select qtrRFQID, prtRFQLineNumber, quoQuoteID, quoOldQuoteNumber from linkPartToQuote, tblPart, linkQuoteToRFQ, tblQuote where qtrQuoteID = @quoteID and ";
                            sql.CommandText += "qtrQuoteID = ptqQuoteID and ptqPartID = prtPARTID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and qtrHTS = 0 ";
                            sql.CommandText += "and qtrSTS = 0 and qtrUGS = 0 and ptqQuoteID = quoQuoteID and quoTSGCompanyID = @companyID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", oldQuoteID);
                            sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                oldQuoteNumber = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString();
                                if (dr.GetValue(3).ToString() != "")
                                {
                                    oldQuoteNumber = dr.GetValue(3).ToString();
                                }
                                tempQuoteID = dr.GetValue(2).ToString();
                            }
                            dr.Close();

                            if (master.readCellString(sh.GetRow(i + 5).GetCell(1)) == "No" || master.readCellString(sh.GetRow(i + 5).GetCell(1)) == "")
                            {
                                deleteQuote(tempQuoteID);
                            }
                            else
                            {
                                version++;
                            }
                        }

                        //string tempadfjaldkfaldjf = master.readCellString(sh.GetRow(i + 5).GetCell(0));

                        List<int> noteIDs = new List<int>();
                        if (master.readCellString(sh.GetRow(i + 5).GetCell(0)) == "Early parts cost")
                        {
                            row = i + 8;
                        }
                        else
                        {
                            row = i + 6;
                        }

                        q.TotalAmount = 0;

                        for (int k = 0; k < 100; k++)
                        {
                            try
                            {
                                //when we find the next part number we break out of notes
                                if (sh.GetRow(row) != null && (master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == null || master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == ""))
                                {
                                    if ((sh.GetRow(row).GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(row).GetCell(9, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null) && (master.readCellString(sh.GetRow(row).GetCell(2)) != "Note"))
                                    {
                                        if (master.readCellString(sh.GetRow(row).GetCell(2)) == "Select (X)")
                                        {
                                            break;
                                        }
                                        q.Description = "";
                                        string costNote = "";
                                        q.TotalAmount += master.readCellDouble(sh.GetRow(row).GetCell(9));
                                        costNote = master.readCellDouble(sh.GetRow(row).GetCell(9)).ToString();
                                        q.Description = master.readCellString(sh.GetRow(row).GetCell(2));

                                        if (q.Description == "Tooling Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Fixture Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Shipping Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Homeline Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Tryout Material Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == " " && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Transfer Bars and Fingers Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }

                                        if (costNote == "-1" || costNote == "0")
                                        {
                                            if (costNote == "-1")
                                            {
                                                q.TotalAmount++;
                                            }
                                            costNote = "";
                                        }
                                        if (q.Description != "" || costNote != "")
                                        {

                                            sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                            sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                                            sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                                            sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                            sql.Parameters.AddWithValue("@note", q.Description);
                                            sql.Parameters.AddWithValue("@costNote", costNote);
                                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                            int noteID = 0;
                                            try
                                            {
                                                noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                            }
                                            catch (Exception err)
                                            {
                                                errorFlag = 1;
                                                //Response.Write("<script>alert('Something went wrong trying to upload your notes, please check your upload sheet for any errors " + i + "');</script>");
                                                context.Response.Write(FileName + " - Something went wrong trying to upload your notes, please check your notes for any errors\n" + err.ToString());
                                                break;
                                            }
                                            sql.Parameters.Clear();
                                            noteIDs.Add(noteID);
                                        }
                                    }
                                }
                                else
                                {
                                    //We either have no more rows or we hit th
                                    break;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            row++;
                        }
                        if (errorFlag == 1)
                        {
                            break;
                        }
                        row++;

                        sql.Parameters.Clear();



                        double toolingCost = 0;
                        double tryoutMaterial = 0;
                        double transferBar = 0;
                        double fixtureCost = 0;
                        double dieSupport = 0;
                        double shippingCost = 0;
                        double additionalCost = 0;
                        string additionalCostDesc = "";
                        double FormSteelCoating = 0;
                        double Qdc = 0;
                        double EaryParts = 0;
                        double Finance = 0;
                        double Spare = 0;
                        toolingCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(0));
                        if (toolingCost == -1)
                        {
                            toolingCost = 0;
                        }
                        tryoutMaterial = master.readCellDouble(sh.GetRow(i + 4).GetCell(1));
                        if (tryoutMaterial == -1)
                        {
                            tryoutMaterial = 0;
                        }
                        transferBar = master.readCellDouble(sh.GetRow(i + 4).GetCell(2));
                        if (transferBar == -1)
                        {
                            transferBar = 0;
                        }
                        fixtureCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(3));
                        if (fixtureCost == -1)
                        {
                            fixtureCost = 0;
                        }
                        dieSupport = master.readCellDouble(sh.GetRow(i + 4).GetCell(4));
                        if (dieSupport == -1)
                        {
                            dieSupport = 0;
                        }
                        shippingCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(5));
                        if (shippingCost == -1)
                        {
                            shippingCost = 0;
                        }
                        additionalCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(7));
                        if (additionalCost == -1)
                        {
                            additionalCost = 0;
                        }
                        try
                        {
                            FormSteelCoating = master.readCellDouble(sh.GetRow(i + 4).GetCell(7));
                            if (FormSteelCoating == -1)
                            {
                                FormSteelCoating = 0;
                            }
                            Qdc = master.readCellDouble(sh.GetRow(i + 6).GetCell(2));
                            if (Qdc == -1)
                            {
                                Qdc = 0;
                            }
                            EaryParts = master.readCellDouble(sh.GetRow(i + 6).GetCell(0));
                            if (EaryParts == -1)
                            {
                                EaryParts = 0;
                            }
                            Finance = master.readCellDouble(sh.GetRow(i + 6).GetCell(1));
                            if (Finance == -1)
                            {
                                Finance = 0;
                            }
                            Spare = master.readCellDouble(sh.GetRow(i + 6).GetCell(3));
                            if (Spare == -1)
                            {
                                Spare = 0;
                            }
                        }
                        catch
                        {

                        }
                        string qdcmasterplate = "";
                        double esttoolweight = 0;
                        try {
                            qdcmasterplate = master.readCellString(sh.GetRow(21).GetCell(2));
                            esttoolweight = master.readCellDouble(sh.GetRow(16).GetCell(0));
                            if (esttoolweight == -1)
                            {
                                esttoolweight = 0;
                            }

                        }
                        catch
                        {

                        }


                        additionalCostDesc = master.readCellString(sh.GetRow(i + 4).GetCell(6));

                        if (master.readCellInt(sh.GetRow(i + 4).GetCell(10)) != -1 && master.readCellString(sh.GetRow(i + 7).GetCell(1)) == "No" ||
                            master.readCellString(sh.GetRow(i + 5).GetCell(1)) == "" || master.readCellInt(sh.GetRow(i + 4).GetCell(10)) != -1 && master.readCellString(sh.GetRow(i + 5).GetCell(1)) == "No")
                        {
                            try
                            {
                                sql.CommandText = "Select quoTSGCompanyID from tblQuote where quoQuoteID = @quoteID";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@quoteID", master.readCellInt(sh.GetRow(i + 4).GetCell(10)).ToString());
                                dr = sql.ExecuteReader();
                                if (dr.Read())
                                {
                                    if (System.Convert.ToInt32(dr.GetValue(0).ToString()) == master.getCompanyId())
                                    {
                                        deleteQuote(master.readCellInt(sh.GetRow(i + 4).GetCell(10)).ToString());
                                    }
                                }
                                dr.Close();
                            }
                            catch
                            {

                            }
                        }

                        string statusID = "2";
                        try
                        {
                            if (master.getCompanyId() == 8)
                            {
                                sql.CommandText = "Select qstQuoteStatusID from pktblQuoteStatus where qstQuoteStatusDescription = @status";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@status", master.readCellString(sh.GetRow(0).GetCell(7)));
                                dr = sql.ExecuteReader();
                                if (dr.Read())
                                {
                                    statusID = dr.GetValue(0).ToString();
                                }
                                dr.Close();
                            }
                        }
                        catch
                        {

                        }
                        
                        


                        //Insert into quote table
                        sql.CommandText = "insert into tblQuote (quoTSGCompanyID, quoRFQID, quoEstimatorID, quoPaymentTermsID, quoShippingTermsID, ";
                        sql.CommandText += "quoTotalAmount, quoPartTypeID, quoToolCountryID, quoLeadTime, quoCreated, quoCreatedBy, quoSalesman, quoStatusID, quoNumber, quoVersion, quoUseTSGLogo, quoToolingCost, quoTransferBarCost, ";
                        sql.CommandText += "quoFixtureCost, quoDieSupportCost, quoShippingCost, quoAdditCostDesc, quoAdditCost, quoUseTSGName, quoPartNumbers, quoCustomerQuoteNumber, quoCurrencyID, quoAccess, quoShippingLocation, quoOldQuoteNumber, quoPartName, quoFormSteelCost, quoQDCCost, quoTryoutCost, quoEarlyPartsCost, quoFinanceCost, quoSpareCost, quoQdcMasterPlate, quoEstToolWeight  ) ";
                        sql.CommandText += "Output inserted.quoQuoteID ";
                        sql.CommandText += "Values ( @company, @rfq, @estimator, @paymentTerms, @shippingTerms, @totalAmount, @partType, @toolCountry, @leadTime, GETDATE(), @createdBy, @salesman, @status, @number, @version, @logo, ";
                        sql.CommandText += "@toolingCost, @transferBar, @fixture, @dieSupport, @shippingCost, @addCostDesc, @addCost, @tsgName, @partNumbers, @custQuoteNum, @currency, @access, @shippingLocation, @oldQuoteNumber, @partName, @quoFormSteelCost, @quoQDCCost, @quoTryoutCost, @quoEarlyPartsCost, @quoFinanceCost, @quoSpareCost, @quoQdcMasterPlate, @quoEstToolWeight )";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        sql.Parameters.AddWithValue("@rfq", q.RFQID);
                        sql.Parameters.AddWithValue("@estimator", q.EstimatorID);
                        //sql.Parameters.AddWithValue("@jobNumber", q.JobNumber);
                        sql.Parameters.AddWithValue("@paymentTerms", q.PaymentTerms);
                        sql.Parameters.AddWithValue("@shippingTerms", q.ShippingTerms);
                        sql.Parameters.AddWithValue("@totalAmount", q.TotalAmount);
                        sql.Parameters.AddWithValue("@partType", q.PartType);
                        sql.Parameters.AddWithValue("@toolCountry", q.ToolCountry);
                        if (q.LeadTime == -1)
                        {
                            sql.Parameters.AddWithValue("@leadTime", q.LeadTimeString);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@leadTime", q.LeadTime);
                        }
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        sql.Parameters.AddWithValue("@salesman", salesmanID);
                        sql.Parameters.AddWithValue("@status", statusID);
                        sql.Parameters.AddWithValue("@number", number);
                        sql.Parameters.AddWithValue("@version", String.Format("{0:000}", version));
                        sql.Parameters.AddWithValue("@toolingCost", toolingCost);
                        sql.Parameters.AddWithValue("@transferBar", transferBar);
                        sql.Parameters.AddWithValue("@fixture", fixtureCost);
                        sql.Parameters.AddWithValue("@dieSupport", dieSupport);
                        sql.Parameters.AddWithValue("@shippingCost", shippingCost);
                        sql.Parameters.AddWithValue("@quoFormSteelCost", FormSteelCoating);
                        sql.Parameters.AddWithValue("@quoQDCCost", Qdc);
                        sql.Parameters.AddWithValue("@quoTryoutCost", EaryParts);
                        sql.Parameters.AddWithValue("@quoEarlyPartsCost", tryoutMaterial);
                        sql.Parameters.AddWithValue("@quoFinanceCost", Finance);
                        sql.Parameters.AddWithValue("@quoSpareCost", Spare);
                        sql.Parameters.AddWithValue("@addCostDesc", additionalCostDesc);
                        sql.Parameters.AddWithValue("@addCost", additionalCost);
                        sql.Parameters.AddWithValue("@quoQdcMasterPlate", qdcmasterplate);
                        sql.Parameters.AddWithValue("@quoEstToolWeight", esttoolweight);
                        if (master.readCellInt(sh.GetRow(i).GetCell(3)) != -1)
                        {
                            sql.Parameters.AddWithValue("@partNumbers", master.readCellInt(sh.GetRow(i).GetCell(3)));
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partNumbers", master.readCellString(sh.GetRow(i).GetCell(3)));
                        }

                        if (master.readCellInt(sh.GetRow(i).GetCell(4)) != -1)
                        {
                            sql.Parameters.AddWithValue("@partName", master.readCellInt(sh.GetRow(i).GetCell(4)));
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partName", master.readCellString(sh.GetRow(i).GetCell(4)));
                        }

                        sql.Parameters.AddWithValue("@custQuoteNum", master.readCellString(sh.GetRow(i).GetCell(1)));
                        sql.Parameters.AddWithValue("@currency", q.currency);
                        sql.Parameters.AddWithValue("@shippingLocation", master.readCellString(sh.GetRow(i + 2).GetCell(10)));
                        sql.Parameters.AddWithValue("@oldQuoteNumber", oldQuoteNumber);
                        if (master.readCellInt(sh.GetRow(i + 4).GetCell(11)) == -1)
                        {
                            sql.Parameters.AddWithValue("@access", master.readCellString(sh.GetRow(i + 4).GetCell(11)));
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@access", master.readCellInt(sh.GetRow(i + 4).GetCell(11)));

                        }

                        if (master.readCellString(sh.GetRow(0).GetCell(3)) == "TSG")
                        {
                            sql.Parameters.AddWithValue("@logo", 1);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@logo", 0);
                        }
                        if (master.readCellString(sh.GetRow(0).GetCell(5)) == "TSG")
                        {
                            sql.Parameters.AddWithValue("@tsgName", 1);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@tsgName", 0);
                        }
                        quoteID = 0;

                        //error checking
                        if (q.ShippingTerms == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write("Your shipping terms are incorrect or not filled out, please try to fill it out and reupload\n");
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            break;
                        }
                        else if (q.PaymentTerms == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write("Your payment terms are incorrect or not filled out, please try to fill it out and reupload\n");
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            break;
                        }
                        else if (q.EstimatorID == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write("Your estimator is not filled out, please try to fill it out and reupload\n");
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            break;
                        }
                        else if (q.PartType == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write("Your part type may be filled out incorrectly or not filled out, please try to fill it out and reupload\n");
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            break;
                        }
                        else if (q.ToolCountry == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write("Your tool country may be filled out incorrectly or not filled out, please try to fill it out and reupload\n");
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            break;
                        }

                        try
                        {
                            quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                            quoteIDs.Add(quoteID.ToString());
                        }
                        catch (Exception err)
                        {
                            string a = err.ToString();
                            errorFlag = 1;
                            context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors\n" + err.ToString());

                            //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            //Breaking so we dont try and insert anything else
                            break;
                        }
                        sql.Parameters.Clear();

                        if (number == 0 && version == 0)
                        {
                            sql.CommandText = "Update tblQuote set quoNumber = @number, quoVersion = @version where quoQuoteID = @quoteID";
                            sql.Parameters.AddWithValue("@number", quoteID);
                            sql.Parameters.AddWithValue("@version", String.Format("{0:000}", 1));
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "QuoteUpload");
                        }

                        sql.Parameters.Clear();
                        //Inserting link for all notes previously uploaded
                        for (int k = 0; k < noteIDs.Count; k++)
                        {
                            sql.Parameters.Clear();

                            sql.CommandText = "Insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                            sql.CommandText += "output inserted.pwqPWNToQuoteID ";
                            sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            master.ExecuteNonQuery(sql, "EditRFQ");
                        }

                        sql.Parameters.Clear();


                        //Linking part to quote
                        sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                        sql.CommandText += "output inserted.ptqPartToQuoteID ";
                        sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 0, 0);";
                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "EditRFQ");

                        sql.Parameters.Clear();

                        sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from ";
                        sql.CommandText += "linkPartToPartDetail where ppdPartID = @partID) and ppdPartID <> @partID";
                        sql.Parameters.AddWithValue("@partID", partID);
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
                            master.ExecuteNonQuery(sql, "EditRFQ");
                        }
                        sql.Parameters.Clear();

                        sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(8)));
                        dr = sql.ExecuteReader();

                        int matID = 0;
                        if (dr.Read())
                        {
                            matID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                        }
                        dr.Close();

                        if (matID == 0)
                        {
                            sql.CommandText = "insert into pktblMaterialType(mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                            sql.CommandText += "output inserted.mtyMaterialTypeID ";
                            sql.CommandText += "values(@matType, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(8)));
                            sql.Parameters.AddWithValue("@user", master.getUserName());
                            matID = System.Convert.ToInt32(master.ExecuteScalar(sql, "QuoteUpload"));
                        }


                        sql.CommandText = "Insert into pktblBlankInfo (binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, ";
                        sql.CommandText += "binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialWeightEnglish, binMaterialWeightMetric, binCreated, binCreatedBy) ";
                        sql.CommandText += "output inserted.binBlankInfoID ";
                        sql.CommandText += "Values(@matType, @thickEng, @thickMet, @pitchEng, @pitchMet, @widthEng, @widthMet, @weightEng, @weightMet, GETDATE(), @user)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", matID);
                        if (q.measurement == "1")
                        {
                            sql.Parameters.AddWithValue("@pitchEng", master.readCellDouble(sh.GetRow(i).GetCell(11)));
                            sql.Parameters.AddWithValue("@pitchMet", master.readCellDouble(sh.GetRow(i).GetCell(11)) * 25.4);
                            sql.Parameters.AddWithValue("@widthEng", master.readCellDouble(sh.GetRow(i).GetCell(10)));
                            sql.Parameters.AddWithValue("@widthMet", master.readCellDouble(sh.GetRow(i).GetCell(10)) * 25.4);
                            sql.Parameters.AddWithValue("@weightEng", 0);
                            //lbs to kg is 0.453592
                            sql.Parameters.AddWithValue("@weightMet", 0);
                            sql.Parameters.AddWithValue("@thickEng", master.readCellDouble(sh.GetRow(i).GetCell(9)) / 25.4);
                            //in to mm is 25.4
                            sql.Parameters.AddWithValue("@thickMet", master.readCellDouble(sh.GetRow(i).GetCell(9)));
                        }
                        else if (q.measurement == "2")
                        {
                            sql.Parameters.AddWithValue("@pitchEng", master.readCellDouble(sh.GetRow(i).GetCell(11)));
                            sql.Parameters.AddWithValue("@pitchMet", master.readCellDouble(sh.GetRow(i).GetCell(11)) * 25.4);
                            sql.Parameters.AddWithValue("@widthEng", master.readCellDouble(sh.GetRow(i).GetCell(10)));
                            sql.Parameters.AddWithValue("@widthMet", master.readCellDouble(sh.GetRow(i).GetCell(10)) * 25.4);
                            sql.Parameters.AddWithValue("@weightEng", 0);
                            //lbs to kg is 0.453592
                            sql.Parameters.AddWithValue("@weightMet", 0);

                            sql.Parameters.AddWithValue("@thickEng", master.readCellDouble(sh.GetRow(i).GetCell(9)));
                            //in to mm is 25.4
                            sql.Parameters.AddWithValue("@thickMet", master.readCellDouble(sh.GetRow(i).GetCell(9)) * 25.4);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@pitchEng", master.readCellDouble(sh.GetRow(i).GetCell(11)) * 0.0393701);
                            sql.Parameters.AddWithValue("@pitchMet", master.readCellDouble(sh.GetRow(i).GetCell(11)));
                            sql.Parameters.AddWithValue("@widthEng", master.readCellDouble(sh.GetRow(i).GetCell(10)) * 0.0393701);
                            sql.Parameters.AddWithValue("@widthMet", master.readCellDouble(sh.GetRow(i).GetCell(10)));
                            sql.Parameters.AddWithValue("@weightEng", 0);
                            //lbs to kg is 0.453592
                            sql.Parameters.AddWithValue("@weightMet", 0);

                            sql.Parameters.AddWithValue("@thickEng", master.readCellDouble(sh.GetRow(i).GetCell(9)) / 25.4);
                            //in to mm is 25.4
                            sql.Parameters.AddWithValue("@thickMet", master.readCellDouble(sh.GetRow(i).GetCell(9)));
                        }

                       

                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        int blankInfoID = 0;
                        try
                        {
                            blankInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Quote Upload"));
                        }
                        catch (Exception err)
                        {
                            //If we caught an error we want to make sure that we delete everything that was already inserted
                            //We first start with link tables so the queries don't fail because of foreign keys
                            errorFlag = 1;
                            context.Response.Write(FileName + " - Something went wrong trying to upload the stock size information, please check your upload sheet for any errors " + err.Message);

                            sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "EditRFQ");

                            sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "EditRFQ");

                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "EditRFQ");
                            //Breaking so we dont try and insert anything else
                            break;
                        }

                        sql.Parameters.Clear();
                        sql.CommandText = "Update tblQuote set quoBlankInfoID = @blankID where quoQuoteID = @quoteID";
                        sql.Parameters.AddWithValue("@blankID", blankInfoID);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        master.ExecuteNonQuery(sql, "Quote Upload");


                        //Starting on the die info
                        sql.CommandText = "Select DieTypeID, CavCavityID from pktblCavity, DieType where dtyFullName = @die and TSGCompanyID = @company and cavCavityName = @cavity";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@die", master.readCellString(sh.GetRow(i).GetCell(6)));
                        sql.Parameters.AddWithValue("@company", q.TSGCompanyID);
                        sql.Parameters.AddWithValue("@cavity", master.readCellString(sh.GetRow(i).GetCell(5)));
                        dr = sql.ExecuteReader();

                        string alkdjfadslkf = master.readCellString(sh.GetRow(i).GetCell(6));
                        string alkdjfadadsslkf = master.readCellString(sh.GetRow(i).GetCell(5));

                        while (dr.Read())
                        {
                            d.DieType = System.Convert.ToInt32(dr.GetValue(0));
                            d.CavityType = System.Convert.ToInt32(dr.GetValue(1));
                        }
                        dr.Close();

                        sql.Parameters.Clear();

                        if (q.measurement == "1" || q.measurement == "2")
                        {
                            d.FtoBEnglish = master.readCellDouble(sh.GetRow(i + 2).GetCell(1));
                            d.FtoBMetric = d.FtoBEnglish * 25.4;

                            d.LtoREnglish = master.readCellDouble(sh.GetRow(i + 2).GetCell(2));
                            d.LtoRMetric = d.LtoREnglish * 25.4;

                            d.ShutHeightEnglish = master.readCellDouble(sh.GetRow(i + 2).GetCell(3));
                            d.ShutHeightMetric = d.ShutHeightEnglish * 25.4;

                            d.NumberOfStations = master.readCellString(sh.GetRow(i + 2).GetCell(0));
                            if (d.NumberOfStations == "")
                            {
                                d.NumberOfStations = master.readCellInt(sh.GetRow(i + 2).GetCell(0)).ToString();
                            }
                        }
                        else
                        {
                            d.FtoBMetric = master.readCellDouble(sh.GetRow(i + 2).GetCell(1));
                            d.FtoBEnglish = d.FtoBMetric * 0.0393701;

                            d.LtoRMetric = master.readCellDouble(sh.GetRow(i + 2).GetCell(2));
                            d.LtoREnglish = d.LtoRMetric * 0.0393701;

                            d.ShutHeightMetric = master.readCellDouble(sh.GetRow(i + 2).GetCell(3));
                            d.ShutHeightEnglish = d.ShutHeightMetric * 0.0393701;

                            d.NumberOfStations = master.readCellString(sh.GetRow(i + 2).GetCell(0));
                            if (d.NumberOfStations == "")
                            {
                                d.NumberOfStations = master.readCellInt(sh.GetRow(i + 2).GetCell(0)).ToString();
                            }
                        }


                        //Inserting die info
                        sql.CommandText = "insert into tblDieInfo (dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, ";
                        sql.CommandText += "dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy) ";
                        sql.CommandText += "Output inserted.dinDieInfoID ";
                        sql.CommandText += "Values (@dieType, @cavity, @fToBEng, @fToBMet, @lToREng, @lToRMet, @shutHiehgtEng, @shutHeightMet, @numOfStations, GETDATE(), @createdBy )";

                        sql.Parameters.AddWithValue("@dieType", d.DieType);
                        sql.Parameters.AddWithValue("@cavity", d.CavityType);
                        sql.Parameters.AddWithValue("@fToBEng", d.FtoBEnglish);
                        sql.Parameters.AddWithValue("@fToBMet", d.FtoBMetric);
                        sql.Parameters.AddWithValue("@lToREng", d.LtoREnglish);
                        sql.Parameters.AddWithValue("@lToRMet", d.LtoRMetric);
                        sql.Parameters.AddWithValue("@shutHiehgtEng", d.ShutHeightEnglish);
                        sql.Parameters.AddWithValue("@shutHeightMet", d.ShutHeightMetric);
                        sql.Parameters.AddWithValue("@numOfStations", d.NumberOfStations);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                        if (d.DieType == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write(FileName + "Your die type / cavity may be filled out incorrectly or not at all, please check and reupload");

                            sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankInfoID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@blankInfoID", blankInfoID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");
                            //Breaking so we dont try and insert anything else
                            break;
                        }
                        else if (d.CavityType == 0)
                        {
                            errorFlag = 1;
                            context.Response.Write(FileName + "Your Process type may be filled out incorrectly or not at all, please check and reupload");

                            sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankInfoID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@blankInfoID", blankInfoID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");
                            //Breaking so we dont try and insert anything else
                            break;
                        }

                        int dieInfoID = 0;
                        try
                        {
                            dieInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Quote Upload"));
                        }
                        catch (Exception err)
                        {

                            //If we caught an error we want to make sure that we delete everything that was already inserted
                            //We first start with link tables so the queries don't fail because of foreign keys
                            errorFlag = 1;
                            context.Response.Write(FileName + " - Something went wrong trying to upload the die info, please check your upload sheet for any errors " + err.Message);

                            sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankInfoID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@blankInfoID", blankInfoID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");

                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            master.ExecuteNonQuery(sql, "Quote Upload");
                            //Breaking so we dont try and insert anything else
                            break;
                        }

                        sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partTypeDesc";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partTypeDesc", master.readCellString(sh.GetRow(i).GetCell(7)));
                        dr = sql.ExecuteReader();
                        string partTypeID = "";
                        while (dr.Read())
                        {
                            partTypeID = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        sql.CommandText = "Update tblPart set prtPartTypeID = @partTypeID where prtPARTID = @partID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partID);
                        sql.Parameters.AddWithValue("@partTypeID", partTypeID);
                        master.ExecuteNonQuery(sql, "Quote Upload");

                        sql.CommandText = "insert into linkQuoteToRFQ(qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                        sql.CommandText += "values(@quoteID, @rfqID, GETDATE(), @createdBy, 0, 0, 0)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfqID", rfq);
                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "Quote Upload");

                        sql.Parameters.Clear();

                        sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                        sql.CommandText += "output inserted.diqDieInfoToQuoteID ";
                        sql.CommandText += "values (@dieInfo, @quote, GETDATE(), @createdBy)";
                        sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                        sql.Parameters.AddWithValue("@quote", quoteID);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        master.ExecuteNonQuery(sql, "Quote Upload");

                        sql.Parameters.Clear();
                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoDefault = 1 and gnoCompany = @company";
                        sql.Parameters.AddWithValue("@company", "general");

                        dr = sql.ExecuteReader();
                        List<string> genIDs = new List<string>();
                        while (dr.Read())
                        {
                            genIDs.Add(dr.GetValue(0).ToString());
                        }
                        dr.Close();

                        string user = master.getUserName();

                        if(generalNotes.Count != 0)
                        {
                            for(int k = 0; k < generalNotes.Count; k++)
                            {
                                sql.CommandText = "insert into linkGeneralNoteToQuote(gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy, gnqHTS) ";
                                sql.CommandText += "values(@genNote, @quoteID, GETDATE(), @user, 0) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@genNote", generalNotes[k]);
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@user", user);
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                        }

                        sql.Parameters.Clear();
                        sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        master.ExecuteNonQuery(sql, "Quote Upload");



                    }
                    catch (Exception err)
                    {
                        //We just try to keep everything moving if there is a total error...
                        context.Response.Write(FileName + " - Something went wrong trying to upload\n" + err.ToString());
                        break;
                    }
                    i = row;
                }
                connection.Close();
            }
            if (errorFlag == 0)
            {
                string html = master.renderQuotingHTML(partID.ToString(), "1", rfq, true);
                context.Response.Write("OK|" + partID + "|" + html);
            }
            return;
        }

        public void ProcessSAWorkSheet(HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 5; // skip the header rows
            int SalesOrderNumber = 0;
            Site master = new RFQ.Site();
            Boolean tsgLogo = false;
            string finalQuoteID = "";

            if (sh != null)
            {
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                List<string> generalNotes = new List<string>();
                while (sh.GetRow(i) != null)
                {
                    row = i;
                    if (i > 1000)
                    {
                        return;
                    }
                    try
                    {
                        if (SalesOrderNumber != master.readCellInt(sh.GetRow(2).GetCell(2)))
                        {
                            q = new Quote();
                            d = new DieInfo();
                        }
                        SqlCommand sql = new SqlCommand();
                        sql.Connection = connection;

                        try
                        {
                            if (master.readCellString(sh.GetRow(i - 1).GetCell(2)) == "Select (X)")
                            {
                                break;
                            }
                        }
                        catch { }
                        try
                        {
                            if (master.readCellString(sh.GetRow(i).GetCell(2)) == "Select (X)")
                            {
                                break;
                            }
                        }
                        catch { }

                        if (i == 5)
                        {
                            for (int gen = 5; gen < 1000; gen++)
                            {
                                if (sh.GetRow(i) != null)
                                {
                                    try
                                    {
                                        if (master.readCellString(sh.GetRow(gen).GetCell(2)) == "Select (X)")
                                        {
                                            for (int k = gen + 1; k < gen + 40; k++)
                                            {
                                                if (sh.GetRow(k) != null && master.readCellString(sh.GetRow(k).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) != null)
                                                {
                                                    if (master.readCellString(sh.GetRow(k).GetCell(2)).ToLower() == "x")
                                                    {
                                                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoGeneralNote = @genNote and gnoCompany = @company";
                                                        sql.Parameters.Clear();
                                                        sql.Parameters.AddWithValue("@genNote", master.readCellString(sh.GetRow(k).GetCell(3)));
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
                                                        SqlDataReader genNotesDR = sql.ExecuteReader();
                                                        if (genNotesDR.Read())
                                                        {
                                                            generalNotes.Add(genNotesDR.GetValue(0).ToString());
                                                        }
                                                        genNotesDR.Close();
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }

                        q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());

                        sql.CommandText = "Select ptePaymentTermsID, steShippingTermsID from pktblPaymentTerms, pktblShippingTerms where ";
                        sql.CommandText += "ptePaymentTerms = @paymentTerms and steShippingTerms = @shippingTerms";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i + 2).GetCell(7)));
                        sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i + 2).GetCell(6)));

                        string steShipp = master.readCellString(sh.GetRow(i + 2).GetCell(6));
                        q.ShippingTerms = 0;
                        q.PaymentTerms = 0;
                        SqlDataReader dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                            q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(1));
                        }
                        dr.Close();

                        sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @lastName";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@lastName", master.readCellString(sh.GetRow(i + 2).GetCell(5)));
                        q.EstimatorID = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(7)));
                        q.PartType = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i + 2).GetCell(4)));
                        q.ToolCountry = 0;
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        sql.Parameters.Clear();
                        sql.CommandText = "Select curCurrencyID, meaMeasurementID from pktblCurrency, pktblMeasurement where curCurrency = @cur ";
                        sql.CommandText += "and meaMeasurement = @mea";
                        sql.Parameters.AddWithValue("@cur", master.readCellString(sh.GetRow(i + 4).GetCell(9)));
                        sql.Parameters.AddWithValue("@mea", master.readCellString(sh.GetRow(i + 2).GetCell(9)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.currency = dr.GetValue(0).ToString();
                            q.measurement = dr.GetValue(1).ToString();
                        }
                        dr.Close();

                        string customerID = "";
                        sql.Parameters.Clear();
                        sql.CommandText = " select CustomerID from Customer where CustomerName = @CustomerName ";
                        sql.Parameters.AddWithValue("@CustomerName", master.readCellString(sh.GetRow(2).GetCell(3)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerID = dr.GetValue(0).ToString();
                        }
                        dr.Close();


                        string customerLocation = "";
                        sql.Parameters.Clear();
                        sql.CommandText = " select CustomerLocationID from CustomerLocation where ShipToName = @CustomerLocation ";
                        sql.Parameters.AddWithValue("@CustomerLocation", master.readCellString(sh.GetRow(2).GetCell(5)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerLocation = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        string dieType = "";
                        sql.Parameters.Clear();
                        sql.CommandText = "select DieTypeID from DieType where Name = @process and TSGCompanyID = @TSGCompany ";
                        sql.Parameters.AddWithValue("@process", master.readCellString(sh.GetRow(5).GetCell(6)));
                        sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            dieType = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        string cavity = "";
                        sql.Parameters.Clear();
                        sql.CommandText = "select cavCavityID from pktblCavity where cavCavityName = @Cavity ";
                        sql.Parameters.AddWithValue("@Cavity", master.readCellString(sh.GetRow(5).GetCell(5)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            cavity = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(5).GetCell(8)));
                        dr = sql.ExecuteReader();

                        int matID = 0;
                        if (dr.Read())
                        {
                            matID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                        }
                        dr.Close();

                        if (matID == 0)
                        {
                            sql.CommandText = "insert into pktblMaterialType(mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                            sql.CommandText += "output inserted.mtyMaterialTypeID ";
                            sql.CommandText += "values(@matType, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(26)));
                            sql.Parameters.AddWithValue("@user", master.getUserName());
                            matID = System.Convert.ToInt32(master.ExecuteScalar(sql, "QuoteUpload"));
                        }
                        sql.Parameters.Clear();

                        if (master.readCellString(sh.GetRow(0).GetCell(3)) == "TSG")
                        {
                            tsgLogo = true;
                        }


                        List<int> noteIDs = new List<int>();
                        row = i + 6;
                        q.TotalAmount = 0;

                        for (int k = 0; k < 100; k++)
                        {
                            try
                            {
                                //when we find the next part number we break out of notes
                                if (sh.GetRow(row) != null && (master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == null || master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == ""))
                                {
                                    if ((sh.GetRow(row).GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(row).GetCell(9, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null) && (master.readCellString(sh.GetRow(row).GetCell(2)) != "Note"))
                                    {
                                        if (master.readCellString(sh.GetRow(row).GetCell(2)) == "Select (X)")
                                        {
                                            break;
                                        }
                                        q.Description = "";
                                        string costNote = "";
                                        q.TotalAmount += master.readCellDouble(sh.GetRow(row).GetCell(9));
                                        costNote = master.readCellDouble(sh.GetRow(row).GetCell(9)).ToString();
                                        q.Description = master.readCellString(sh.GetRow(row).GetCell(2));

                                        if (q.Description == "Tooling Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Fixture Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Shipping Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Homeline Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Tryout Material Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == " " && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }
                                        if (q.Description == "Transfer Bars and Fingers Cost:" && costNote == "0")
                                        {
                                            row++;
                                            continue;
                                        }

                                        if (costNote == "-1" || costNote == "0")
                                        {
                                            if (costNote == "-1")
                                            {
                                                q.TotalAmount++;
                                            }
                                            costNote = "";
                                        }
                                        if (q.Description != "" || costNote != "")
                                        {

                                            sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                            sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                                            sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                                            sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                            sql.Parameters.AddWithValue("@note", q.Description);
                                            sql.Parameters.AddWithValue("@costNote", costNote);
                                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                            int noteID = 0;
                                            try
                                            {
                                                noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                            }
                                            catch (Exception err)
                                            {
                                                errorFlag = 1;
                                                //Response.Write("<script>alert('Something went wrong trying to upload your notes, please check your upload sheet for any errors " + i + "');</script>");
                                                context.Response.Write(FileName + " - Something went wrong trying to upload your notes, please check your notes for any errors\n" + err.ToString());
                                                break;
                                            }
                                            sql.Parameters.Clear();
                                            noteIDs.Add(noteID);
                                        }
                                    }
                                }
                                else
                                {
                                    //We either have no more rows or we hit th
                                    break;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            row++;
                        }
                        if (errorFlag == 1)
                        {
                            break;
                        }
                        row++;

                        sql.Parameters.Clear();



                        double toolingCost = 0;
                        double tryoutMaterial = 0;
                        double transferBar = 0;
                        double fixtureCost = 0;
                        double dieSupport = 0;
                        double shippingCost = 0;
                        double additionalCost = 0;
                        string additionalCostDesc = "";
                        toolingCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(0));
                        if (toolingCost == -1)
                        {
                            toolingCost = 0;
                        }
                        tryoutMaterial = master.readCellDouble(sh.GetRow(i + 4).GetCell(1));
                        if (tryoutMaterial == -1)
                        {
                            tryoutMaterial = 0;
                        }
                        transferBar = master.readCellDouble(sh.GetRow(i + 4).GetCell(2));
                        if (transferBar == -1)
                        {
                            transferBar = 0;
                        }
                        fixtureCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(3));
                        if (fixtureCost == -1)
                        {
                            fixtureCost = 0;
                        }
                        dieSupport = master.readCellDouble(sh.GetRow(i + 4).GetCell(4));
                        if (dieSupport == -1)
                        {
                            dieSupport = 0;
                        }
                        shippingCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(5));
                        if (shippingCost == -1)
                        {
                            shippingCost = 0;
                        }
                        additionalCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(7));
                        if (additionalCost == -1)
                        {
                            additionalCost = 0;
                        }
                        additionalCostDesc = master.readCellString(sh.GetRow(i + 4).GetCell(6));



                        //Insert into quote table
                        sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                        sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                        sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqAccessNumber, ecqUseTSG, ecqVersion, ecqShippingLocation, ecqCustomerContactName) ";
                        sql.CommandText += "Output inserted.ecqECQuoteID ";
                        sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                        sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @accessNumber, @useTSG, @version, @shippingLocation, @custContact )";

                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partNum", master.readCellString(sh.GetRow(5).GetCell(3)));
                        sql.Parameters.AddWithValue("@partName", master.readCellString(sh.GetRow(5).GetCell(4)));
                        sql.Parameters.AddWithValue("@rfqNum", "");
                        sql.Parameters.AddWithValue("@customer", customerID);
                        sql.Parameters.AddWithValue("@CustomerLocation", customerLocation);
                        if (master.readCellInt(sh.GetRow(i).GetCell(4)) != -1)
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", master.readCellInt(sh.GetRow(i).GetCell(4)));
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", master.readCellString(sh.GetRow(i).GetCell(4)));
                        }
                        sql.Parameters.AddWithValue("@dieType", dieType);
                        sql.Parameters.AddWithValue("@cavity", cavity);


                        sql.Parameters.AddWithValue("@matType", matID);
                        sql.Parameters.AddWithValue("@matThkEng", master.readCellDouble(sh.GetRow(i + 2).GetCell(2)) / 25.4);
                        //in to mm is 25.4
                        sql.Parameters.AddWithValue("@matThkMet", master.readCellDouble(sh.GetRow(i + 2).GetCell(2)));
                        sql.Parameters.AddWithValue("@blankPitchEng", master.readCellDouble(sh.GetRow(i + 2).GetCell(1)));
                        sql.Parameters.AddWithValue("@blankPitchMet", master.readCellDouble(sh.GetRow(i + 2).GetCell(1)) * 25.4);
                        sql.Parameters.AddWithValue("@blankWidthEng", master.readCellDouble(sh.GetRow(i + 2).GetCell(0)));
                        sql.Parameters.AddWithValue("@blankWidthMet", master.readCellDouble(sh.GetRow(i + 2).GetCell(0)) * 25.4);
                        sql.Parameters.AddWithValue("@weightEng", 0);
                        //lbs to kg is 0.453592
                        sql.Parameters.AddWithValue("@weightMet", 0);



                        sql.Parameters.AddWithValue("@FBEng", d.FtoBEnglish);
                        sql.Parameters.AddWithValue("@FBMet", d.FtoBMetric);
                        sql.Parameters.AddWithValue("@LREng", d.LtoREnglish);
                        sql.Parameters.AddWithValue("@LRMet", d.LtoRMetric);
                        sql.Parameters.AddWithValue("@shutHeightEng", d.ShutHeightEnglish);
                        sql.Parameters.AddWithValue("@shutHeightMet", d.ShutHeightMetric);
                        //sql.Parameters.AddWithValue("@stations", d.NumberOfStations);
                        sql.Parameters.AddWithValue("@stations", master.readCellString(sh.GetRow(7).GetCell(0)));

                        if (q.LeadTime != -1)
                        {
                            sql.Parameters.AddWithValue("@leadTime", q.LeadTime);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@leadTime", q.LeadTimeString);
                        }
                        sql.Parameters.AddWithValue("@shipping", q.ShippingTerms);
                        sql.Parameters.AddWithValue("@payment", q.PaymentTerms);
                        sql.Parameters.AddWithValue("@country", q.ToolCountry);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                        sql.Parameters.AddWithValue("@totalCost", q.TotalAmount);
                        sql.Parameters.AddWithValue("@status", 2);
                        sql.Parameters.AddWithValue("@salesman", "12");
                        sql.Parameters.AddWithValue("@estimator", q.EstimatorID);
                        //sql.Parameters.AddWithValue("@jobNumber", jobNumber);
                        sql.Parameters.AddWithValue("@accessNumber", master.readCellString(sh.GetRow(i + 4).GetCell(6)));
                        sql.Parameters.AddWithValue("@useTSG", tsgLogo);
                        sql.Parameters.AddWithValue("@version", "001");
                        sql.Parameters.AddWithValue("@shippingLocation", master.readCellString(sh.GetRow(i + 4).GetCell(4)));
                        sql.Parameters.AddWithValue("@custContact", master.readCellString(sh.GetRow(i + 4).GetCell(7)));




                        int quoteID = 0;
                        try
                        {
                            if (errorFlag == 0)
                            {
                                quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                            }
                        }
                        catch (Exception err)
                        {
                            errorFlag = 1;
                            context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors " + err.ToString());

                            //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            //Breaking so we dont try and insert anything else
                        }

                        sql.Parameters.Clear();

                        String pictureName = "EC-" + quoteID + ".png";

                        sql.CommandText = "Update tblECQuote set ecqPicture = @picture, ecqQuoteNumber = @quoteNumber where ecqECQuoteID = @ecQuoteID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@ecQuoteID", quoteID);
                        sql.Parameters.AddWithValue("@picture", pictureName);
                        sql.Parameters.AddWithValue("@quoteNumber", quoteID);
                        master.ExecuteNonQuery(sql, "Edit Quote");


                        //Inserting link for all notes previously uploaded
                        if (errorFlag == 0)
                        {
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();

                                sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                                sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                                sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }

                            sql.Parameters.Clear();


                            List<string> genIDs = new List<string>();
                            genIDs.Add("3");
                            genIDs.Add("4");
                            genIDs.Add("5");
                            genIDs.Add("6");
                            genIDs.Add("7");
                            genIDs.Add("8");
                            for (int k = 0; k < genIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                                sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @created)";
                                sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@created", master.getUserName());
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            row++;
                        }
                        finalQuoteID = quoteID.ToString();
                    }


                    catch (Exception err)
                    {
                        //We just try to keep everything moving if there is a total error...
                        context.Response.Write(FileName + " - Something went wrong trying to upload\n" + err.ToString());
                        break;
                    }
                    i = row;
                }
                connection.Close();
            }
            if (errorFlag == 0)
            {
                string html = master.renderQuotingHTML(partID.ToString(), "1", rfq, true);
                //context.Response.Write("OK|" + partID + "|" + html);

                context.Response.Write("OK|" + partID + "|" + finalQuoteID);
            }
            return;
        }

        public void ProcessNCWorkSheet(HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 22; // skip the header rows
            //string QuoteNumber = "";
            Site master = new RFQ.Site();
            Boolean tsgLogo = false;
            string finalQuoteID = "";

            if (sh != null)
            {
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                List<string> generalNotes = new List<string>();
                //while (sh.GetRow(i) != null)
                //{
                    //row = i;
                    //if (i > 100)
                    //{
                    //    return;
                    //}
                    try
                    {
                        //QuoteNumber = master.readCellString(sh.GetRow(1).GetCell(10));
                        //if (QuoteNumber != master.readCellString(sh.GetRow(1).GetCell(10)))
                        //{
                            q = new Quote();
                            d = new DieInfo();
                        //}
                        SqlCommand sql = new SqlCommand();
                        sql.Connection = connection;

                        try
                        {
                            if (master.readCellString(sh.GetRow(i - 1).GetCell(2)) == "Select (X)")
                            {
                                //break;
                            }
                        }
                        catch { }
                        try
                        {
                            if (master.readCellString(sh.GetRow(i).GetCell(2)) == "Select (X)")
                            {
                                //break;
                            }
                        }
                        catch { }

                        if (i == 22)
                        {
                            for (int gen = 67; gen < 100; gen++)
                            {
                                if (sh.GetRow(i) != null)
                                {
                                    try
                                    {
                                        if (master.readCellString(sh.GetRow(gen).GetCell(1)) == "Select (X)")
                                        {
                                            for (int k = gen + 1; k < gen + 40; k++)
                                            {
                                                if (sh.GetRow(k) != null && master.readCellString(sh.GetRow(k).GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) != null)
                                                {
                                                    if (master.readCellString(sh.GetRow(k).GetCell(1)).ToLower() == "x")
                                                    {
                                                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoGeneralNote = @genNote and gnoCompany = @company";
                                                        sql.Parameters.Clear();
                                                        sql.Parameters.AddWithValue("@genNote", master.readCellString(sh.GetRow(k).GetCell(2)));
                                                        sql.Parameters.AddWithValue("@company", "NewCo");
                                                        
                                                        SqlDataReader genNotesDR = sql.ExecuteReader();
                                                        if (genNotesDR.Read())
                                                        {
                                                            generalNotes.Add(genNotesDR.GetValue(0).ToString());
                                                        }
                                                        genNotesDR.Close();
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }

                        q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());


                        //sql.CommandText = "Select ptePaymentTermsID, steShippingTermsID from pktblPaymentTerms, pktblShippingTerms where ";
                        //sql.CommandText += "ptePaymentTerms = @paymentTerms and steShippingTerms = @shippingTerms";
                        //sql.Parameters.Clear();
                        //sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i + 2).GetCell(7)));
                        //sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i + 2).GetCell(6)));

                        //string steShipp = master.readCellString(sh.GetRow(i + 2).GetCell(6));
                        //q.ShippingTerms = 0;
                        //q.PaymentTerms = 0;
                        //SqlDataReader dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                        //    q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(1));
                        //}
                        //dr.Close();

                        sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @lastName";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@lastName", master.getUserName());
                        q.EstimatorID = 0;
                        SqlDataReader dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                        }
                        dr.Close();

                        //sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                        //sql.Parameters.Clear();
                        //sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(7)));
                        //q.PartType = 0;
                        //dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                        //}
                        //dr.Close();

                        //sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                        //sql.Parameters.Clear();
                        //sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i + 2).GetCell(4)));
                        //q.ToolCountry = 0;
                        //dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                        //}
                        //dr.Close();

                        //sql.Parameters.Clear();
                        //sql.CommandText = "Select curCurrencyID, meaMeasurementID from pktblCurrency, pktblMeasurement where curCurrency = @cur ";
                        //sql.CommandText += "and meaMeasurement = @mea";
                        //sql.Parameters.AddWithValue("@cur", master.readCellString(sh.GetRow(i + 4).GetCell(9)));
                        //sql.Parameters.AddWithValue("@mea", master.readCellString(sh.GetRow(i + 2).GetCell(9)));
                        //dr = sql.ExecuteReader();
                        //if (dr.Read())
                        //{
                        //    q.currency = dr.GetValue(0).ToString();
                        //    q.measurement = dr.GetValue(1).ToString();
                        //}
                        //dr.Close();

                        string customerID = "";
                        sql.Parameters.Clear();
                        sql.CommandText = " select CustomerID from Customer where CustomerName = @CustomerName ";
                        sql.Parameters.AddWithValue("@CustomerName", master.readCellString(sh.GetRow(7).GetCell(1)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerID = dr.GetValue(0).ToString();
                        }
                        dr.Close();


                        string customerLocation = "";
                        sql.Parameters.Clear();
                        sql.CommandText = " select CustomerLocationID from CustomerLocation where ShipToName = @CustomerLocation ";
                        sql.Parameters.AddWithValue("@CustomerLocation", master.readCellString(sh.GetRow(8).GetCell(1)));
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerLocation = dr.GetValue(0).ToString();
                        }
                        dr.Close();


                        string customerContact = "";
                        sql.Parameters.Clear();
                        sql.CommandText = " select CustomerContactID from CustomerContact where Name = @CustomerContact and CustomerID = @customerID ";
                        sql.Parameters.AddWithValue("@CustomerContact", master.readCellString(sh.GetRow(8).GetCell(1)));
                        sql.Parameters.AddWithValue("@customerID", customerID);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerContact = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        //if (master.readCellString(sh.GetRow(0).GetCell(3)) == "TSG")
                        //{
                        //    tsgLogo = true;
                        //}


                        List<int> noteIDs = new List<int>();
                        row = 24;
                        q.TotalAmount = 0;
                        double ToolingTotalAmount = 0;
                        double CaptialTotalAmount = 0;
                        double PieceCostTotalAmount = 0;

                        for (int k = 0; k < 10; k++)
                        {
                            try
                            {
                                //when we find the next part number we break out of notes
                                if (sh.GetRow(row) != null && (master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == null || master.readCellString(sh.GetRow(row).GetCell(3, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL)) == ""))
                                {
                                    if ((sh.GetRow(row).GetCell(2, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(row).GetCell(9, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null) && (master.readCellString(sh.GetRow(row).GetCell(2)) != "Delivery Lead Time:"))
                                    {
                                        if (master.readCellString(sh.GetRow(row).GetCell(2)) == "Select (X)")
                                        {
                                            break;
                                        }
                                        q.Description = "";
                                        string ToolingcostNote = "";
                                        string CapitalcostNote = "";
                                        string PiececostNote = "";
                                        ToolingTotalAmount += master.readCellDouble(sh.GetRow(row).GetCell(9));
                                        CaptialTotalAmount += master.readCellDouble(sh.GetRow(row).GetCell(11));
                                        PieceCostTotalAmount += master.readCellDouble(sh.GetRow(row).GetCell(13));
                                        ToolingcostNote = master.readCellDouble(sh.GetRow(row).GetCell(9)).ToString();
                                        CapitalcostNote = master.readCellDouble(sh.GetRow(row).GetCell(11)).ToString();
                                        PiececostNote = master.readCellDouble(sh.GetRow(row).GetCell(13)).ToString();
                                        q.Description = master.readCellString(sh.GetRow(row).GetCell(1));

                                        if (ToolingcostNote == "-1" || ToolingcostNote == "0")
                                        {
                                            if (ToolingcostNote == "-1")
                                            {
                                                ToolingTotalAmount++;
                                            }
                                            ToolingcostNote = "0";
                                        }
                                        if (CapitalcostNote == "-1" || CapitalcostNote == "0")
                                        {
                                            if (CapitalcostNote == "-1")
                                            {
                                                CaptialTotalAmount++;
                                            }
                                            CapitalcostNote = "0";
                                        }
                                        if (PiececostNote == "-1" || PiececostNote == "0")
                                        {
                                            if (PiececostNote == "-1")
                                            {
                                                PieceCostTotalAmount++;
                                            }
                                            PiececostNote = "0";
                                        }
                                        if (q.Description != "" || ToolingcostNote != "")
                                        {

                                            sql.CommandText = "Insert into pktblPreWordedNoteNc (pwnncCompanyID, pwnncPreWordedNote, pwnncToolingCost, pwnncCapitalCost, pwnncPieceCost, pwnncCreated, pwnncCreatedBy) ";
                                            sql.CommandText += "Output inserted.pwnncPreWordedNoteID ";
                                            sql.CommandText += "Values (@TSGCompany, @note, @ToolingcostNote, @CapitalcostNote, @PiececostNote, GETDATE(), @createdBy)";

                                            sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                            sql.Parameters.AddWithValue("@note", q.Description);
                                            sql.Parameters.AddWithValue("@ToolingcostNote", ToolingcostNote);
                                            sql.Parameters.AddWithValue("@CapitalcostNote", CapitalcostNote);
                                            sql.Parameters.AddWithValue("@PiececostNote", PiececostNote);
                                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                            int noteID = 0;
                                            try
                                            {
                                                noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                            }
                                            catch (Exception err)
                                            {
                                                errorFlag = 1;
                                                //Response.Write("<script>alert('Something went wrong trying to upload your notes, please check your upload sheet for any errors " + i + "');</script>");
                                                context.Response.Write(FileName + " - Something went wrong trying to upload your notes, please check your notes for any errors\n" + err.ToString());
                                                break;
                                            }
                                            sql.Parameters.Clear();
                                            noteIDs.Add(noteID);
                                        }
                                    }
                                }
                                else
                                {
                                    //We either have no more rows or we hit th
                                    break;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                            row++;
                        }
                        if (errorFlag == 1)
                        {
                            //break;
                        }
                        row++;

                        sql.Parameters.Clear();



                        //double toolingCost = 0
                        //toolingCost = master.readCellDouble(sh.GetRow(i + 4).GetCell(0));
                        //if (toolingCost == -1)
                        //{
                        //    toolingCost = 0;
                        //}

                        //string additionalCostDesc = "";
                        //additionalCostDesc = master.readCellString(sh.GetRow(i + 4).GetCell(6));

                        string QuoteNumber = "";
                        QuoteNumber = master.readCellString(sh.GetRow(1).GetCell(10));
                        string QuoteType = "";
                        QuoteType = master.readCellString(sh.GetRow(2).GetCell(10));
                        string CustomerRFQ = "";
                        CustomerRFQ = master.readCellString(sh.GetRow(3).GetCell(11));
                        string Date = "";
                        Date = master.readCellString(sh.GetRow(4).GetCell(10));
                        string JobNumber = "";
                        JobNumber = master.readCellString(sh.GetRow(5).GetCell(10));
                        string QuoteProcess = "";
                        QuoteProcess = master.readCellString(sh.GetRow(12).GetCell(3));
                        string PartNumber = "";
                        PartNumber = master.readCellString(sh.GetRow(13).GetCell(3));
                        string ProgramName = "";
                        ProgramName = master.readCellString(sh.GetRow(14).GetCell(3));
                        string ProgramKickoff = "";
                        ProgramKickoff = master.readCellString(sh.GetRow(16).GetCell(3));
                        string ProgramPPAP = "";
                        ProgramPPAP = master.readCellString(sh.GetRow(17).GetCell(3));
                        string ProgramSOP = "";
                        ProgramSOP = master.readCellString(sh.GetRow(18).GetCell(3));
                        string YearsOfProduction = "";
                        YearsOfProduction = master.readCellString(sh.GetRow(19).GetCell(3));
                        string AnnualVolume = "";
                        AnnualVolume = master.readCellString(sh.GetRow(16).GetCell(8));
                        string QuotedLotSize = "";
                        QuotedLotSize = master.readCellString(sh.GetRow(17).GetCell(8));
                        string DataRecDate = "";
                        DataRecDate = master.readCellString(sh.GetRow(18).GetCell(8));
                        string DataFileName = "";
                        DataFileName = master.readCellString(sh.GetRow(19).GetCell(8));
                        string DesignReview = "";
                        DesignReview = master.readCellString(sh.GetRow(38).GetCell(5));
                        string Tryout = "";
                        Tryout = master.readCellString(sh.GetRow(39).GetCell(9));
                        string ShippingCompany = "";
                        ShippingCompany = master.readCellString(sh.GetRow(40).GetCell(5));
                        string Setup = "";
                        Setup = master.readCellString(sh.GetRow(41).GetCell(6));
                        string Buyoff = "";
                        Buyoff = master.readCellString(sh.GetRow(42).GetCell(5));
                        string PaymentTermsToolingEquipmentCapital = "";
                        PaymentTermsToolingEquipmentCapital = master.readCellString(sh.GetRow(45).GetCell(1));
                        string PaymentTermsPieceCost = "";
                        PaymentTermsPieceCost = master.readCellString(sh.GetRow(49).GetCell(1));
                        string RawMaterial = "";
                        RawMaterial = master.readCellString(sh.GetRow(54).GetCell(4));
                        string WIP = "";
                        WIP = master.readCellString(sh.GetRow(56).GetCell(4));
                        string FinnishedGoods = "";
                        FinnishedGoods = master.readCellString(sh.GetRow(57).GetCell(5));
                        string ShippingOfFinishedGoods = "";
                        ShippingOfFinishedGoods = master.readCellString(sh.GetRow(58).GetCell(5));

                    //Insert into quote table
                    sql.CommandText = "insert into tblNcQuote(ncqQuotationNumber, ncqQuotationType, ncqCustomerRFQ, ncqDate, ncqJobNumber, ncqCustomer, ncqPlant, ncqCustomerContact, ncqEstimatorID, ";
                        sql.CommandText += "ncqVersion, ncqQuoteProcess, ncqPartNumber, ncqProgramName, ncqProgramKickOffDate, ncqProgramPPAPDate, ncqProgramSOPDate, ncqYearsOfProduction, ";
                        sql.CommandText += "ncqAnnualVolume, ncqQuotedLotSize, ncqDataRecievedDate, ncqDataFileName, ncqDesignReview, ncqTryout, ncqShippingCompany, ncqSetup, ";
                        sql.CommandText += "ncqBuyoff, ncqPaymentTermsToolingEquipmentCapital, ncqPaymentTermsPieceCost, ncqRawMaterial, ncqWIP, ncqFinnishedGoods, ncqShippingOfFinishedGoods, ";
                        sql.CommandText += "ncqCreated, ncqCreatedBy, ncqTotalToolingCost, ncqTotalCapitalCost, ncqTotalPieceCost ) ";
                        sql.CommandText += " Output inserted.ncqQuoteID ";
                        sql.CommandText += "values(@QuotationNumber, @QuotationType, @CustomerRFQ, cast(@Date -2e as datetime), @JobNumber, @Customer, @Plant, @CustomerContact, @EstimatorID, ";
                        sql.CommandText += "@Version, @QuoteProcess, @PartNumber, @ProgramName, cast(@ProgramKickOffDate -2e as datetime), cast(@ProgramPPAPDate -2e as datetime), cast(@ProgramSOPDate -2e as datetime), @YearsOfProduction, ";
                        sql.CommandText += "@AnnualVolume, @QuotedLotSize, cast(@DataRecievedDate -2e as datetime), @DataFileName, @DesignReview, @Tryout, @ShippingCompany, @Setup, ";
                        sql.CommandText += "@Buyoff, @PaymentTermsToolingEquipmentCapital, @PaymentTermsPieceCost, @RawMaterial, @WIP, @FinnishedGoods, @ShippingOfFinishedGoods, ";
                        sql.CommandText += "getdate(), @CreatedBy, @TotalToolingCost, @TotalCapitalCost, @TotalPieceCost) ";
                        sql.Parameters.Clear();

                        sql.Parameters.AddWithValue("@QuotationNumber", QuoteNumber);
                        sql.Parameters.AddWithValue("@QuotationType", QuoteType);
                        sql.Parameters.AddWithValue("@CustomerRFQ", CustomerRFQ);
                        try { sql.Parameters.AddWithValue("@Date", Date); }
                        catch { sql.Parameters.AddWithValue("@Date", DBNull.Value); }
                        sql.Parameters.AddWithValue("@JobNumber", JobNumber);
                        sql.Parameters.AddWithValue("@customer", customerID);
                        sql.Parameters.AddWithValue("@Plant", customerLocation);
                        sql.Parameters.AddWithValue("@CustomerContact", customerContact);
                        sql.Parameters.AddWithValue("@EstimatorID", q.EstimatorID);
                        sql.Parameters.AddWithValue("@version", "001");
                        sql.Parameters.AddWithValue("@QuoteProcess", QuoteProcess);
                        sql.Parameters.AddWithValue("@PartNumber", PartNumber);
                        sql.Parameters.AddWithValue("@ProgramName", ProgramName);
                        try { sql.Parameters.AddWithValue("@ProgramKickOffDate", ProgramKickoff); }
                        catch { sql.Parameters.AddWithValue("@ProgramKickOffDate", DBNull.Value); }
                        try{ sql.Parameters.AddWithValue("@ProgramPPAPDate", ProgramPPAP); }
                        catch { sql.Parameters.AddWithValue("@ProgramPPAPDate", DBNull.Value); }
                        try { sql.Parameters.AddWithValue("@ProgramSOPDate", ProgramSOP); }
                        catch { sql.Parameters.AddWithValue("@ProgramSOPDate", DBNull.Value); }
                        sql.Parameters.AddWithValue("@YearsOfProduction", YearsOfProduction);
                        sql.Parameters.AddWithValue("@AnnualVolume", AnnualVolume);
                        sql.Parameters.AddWithValue("@QuotedLotSize", QuotedLotSize);
                        try { sql.Parameters.AddWithValue("@DataRecievedDate", DataRecDate); }
                        catch { sql.Parameters.AddWithValue("@DataRecievedDate", DBNull.Value); }
                        sql.Parameters.AddWithValue("@DataFileName", DataFileName);
                        sql.Parameters.AddWithValue("@DesignReview", DesignReview);
                        sql.Parameters.AddWithValue("@Tryout", Tryout);
                        sql.Parameters.AddWithValue("@ShippingCompany", ShippingCompany);
                        sql.Parameters.AddWithValue("@Setup", Setup);
                        sql.Parameters.AddWithValue("@Buyoff", Buyoff);
                        sql.Parameters.AddWithValue("@PaymentTermsToolingEquipmentCapital", PaymentTermsToolingEquipmentCapital);
                        sql.Parameters.AddWithValue("@PaymentTermsPieceCost", PaymentTermsPieceCost);
                        sql.Parameters.AddWithValue("@RawMaterial", RawMaterial);
                        sql.Parameters.AddWithValue("@WIP", WIP);
                        sql.Parameters.AddWithValue("@FinnishedGoods", FinnishedGoods);
                        sql.Parameters.AddWithValue("@ShippingOfFinishedGoods", ShippingOfFinishedGoods);
                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                        sql.Parameters.AddWithValue("@TotalToolingCost", ToolingTotalAmount);
                        sql.Parameters.AddWithValue("@TotalCapitalCost", CaptialTotalAmount);
                        sql.Parameters.AddWithValue("@TotalPieceCost", PieceCostTotalAmount);               
                        

                        int quoteID = 0;
                        try
                        {
                            if (errorFlag == 0)
                            {
                                quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                            }
                        }
                        catch (Exception err)
                        {
                            errorFlag = 1;
                            context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors " + err.ToString());

                            //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                            sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            //Breaking so we dont try and insert anything else
                        }

                        sql.Parameters.Clear();

                        String pictureName = "NCo-" + quoteID + ".png";

                        sql.CommandText = "Update tblNCQuote set ncqPicture = @picture where ncqQuoteID = @QuoteID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@QuoteID", quoteID);
                        sql.Parameters.AddWithValue("@picture", pictureName);
                        master.ExecuteNonQuery(sql, "Edit Quote");


                        //Inserting link for all notes previously uploaded
                        if (errorFlag == 0)
                        {
                            for (int k = 0; k < noteIDs.Count; k++)
                            {
                                sql.Parameters.Clear();

                                sql.CommandText = "Insert into LinkPWNToNcQuote (pwqncQuoteID, pwqncPreWordedNoteID, pwqncCreated, pwqncCreatedBy) ";
                                sql.CommandText += "output inserted.pwqncPWNToQuoteID ";
                                sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }

                            sql.Parameters.Clear();


                         
                            for (int k = 0; k < generalNotes.Count; k++)
                            {
                                string tem = generalNotes[k];
                                sql.Parameters.Clear();
                                sql.CommandText = "Insert into linkGeneralNoteToQuoteNc (gnqncGeneralNoteID, gnqncQuoteID, gnqncCreated, gnqncCreatedBy) ";
                                sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @created)";
                                sql.Parameters.AddWithValue("@noteID", generalNotes[k]);
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@created", master.getUserName());
                                master.ExecuteNonQuery(sql, "Quote Upload");
                            }
                            row++;
                        }
                        finalQuoteID = quoteID.ToString();
                    }


                    catch (Exception err)
                    {
                        //We just try to keep everything moving if there is a total error...
                        context.Response.Write(FileName + " - Something went wrong trying to upload\n" + err.ToString());
                        //break;
                    }
                    i = row;
                //}
                connection.Close();
            }
            if (errorFlag == 0)
            {
                //string html = master.renderQuotingHTML(partID.ToString(), "2", rfq, true);
                //context.Response.Write("OK|" + partID + "|" + html);
            }
            return;
        }


        public void deleteQuote(string quoteID)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            List<string> preWordedNotes = new List<string>();
            sql.CommandText = "Select pwqPreWordedNoteID from linkPWNToQuote where pwqQuoteID = @quote";
            sql.Parameters.AddWithValue("@quote", quoteID);
            SqlDataReader dr = sql.ExecuteReader();

            while (dr.Read())
            {
                preWordedNotes.Add(dr.GetValue(0).ToString());
            }
            dr.Close();
            sql.Parameters.Clear();

            sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quote";
            sql.Parameters.AddWithValue("@quote", quoteID);
            master.ExecuteNonQuery(sql, "editRFQ");

            for (int i = 0; i < preWordedNotes.Count; i++)
            {
                sql.Parameters.Clear();
                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                sql.Parameters.AddWithValue("@id", preWordedNotes[i]);
                master.ExecuteNonQuery(sql, "editRFQ");
            }
            sql.Parameters.Clear();

            sql.CommandText = "Select diqDieInfoID, quoBlankInfoID from linkDieInfoToQuote, tblQuote where diqQuoteID = @quote and quoQuoteID = @quote";
            sql.Parameters.AddWithValue("@quote", quoteID);
            dr = sql.ExecuteReader();

            int dieInfoID = 0;
            int blankInfoID = 0;
            if (dr.Read())
            {
                dieInfoID = System.Convert.ToInt32(dr.GetValue(0));
                blankInfoID = System.Convert.ToInt32(dr.GetValue(1));
            }
            dr.Close();

            sql.Parameters.Clear();

            sql.CommandText = "Delete from linkDieInfoToQuote where diqQuoteID = @quote";
            sql.Parameters.AddWithValue("@quote", quoteID);
            master.ExecuteNonQuery(sql, "editRFQ");

            sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankInfo";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@blankInfo", blankInfoID);
            master.ExecuteNonQuery(sql, "editRFQ");

            if (dieInfoID != 0)
            {
                sql.Parameters.Clear();
                sql.CommandText = "Delete from tblDieInfo where dinDieInfoID = @id";
                sql.Parameters.AddWithValue("@id", dieInfoID);
                master.ExecuteNonQuery(sql, "editRFQ");
            }

            sql.Parameters.Clear();
            sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quote";
            sql.Parameters.AddWithValue("@quote", quoteID);
            master.ExecuteNonQuery(sql, "editRFQ");

            sql.CommandText = "Delete from linkQuoteToRFQ where qtrQuoteID = @quote and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0";
            master.ExecuteNonQuery(sql, "editRFQ");

            sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quote and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
            master.ExecuteNonQuery(sql, "editRFQ");

            sql.CommandText = "Delete from tblQuote where quoQuoteID = @quote";
            master.ExecuteNonQuery(sql, "editRFQ");
            connection.Close();
        }

        //This is now only for ATS's quotes
        public void ProcessWorkSheet(HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 1; // skip the header row
            int lineNumber = 0;
            Site master = new RFQ.Site();
            if (sh != null)
            {
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                if (sh.GetRow(i) != null)
                {
                    try { 
                        lineNumber = master.readCellInt(sh.GetRow(i).GetCell(0));
                        
                        q = new Quote();
                        d = new DieInfo();
                        row = 0;
                        
                        if (row == 0)
                        {
                            SqlConnection connection = new SqlConnection(master.getConnectionString());
                            connection.Open();
                            SqlCommand sql = new SqlCommand();
                            sql.Connection = connection;
                            SqlDataReader dr;

                            q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());

                            sql.CommandText = "Select estEstimatorID from pktblEstimators where estLastName = @lastName";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@lastName", master.readCellString(sh.GetRow(i).GetCell(32)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptePaymentTermsID from pktblPaymentTerms where ptePaymentTerms = @paymentTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i).GetCell(58)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select steShippingTermsID from pktblShippingTerms where steShippingTerms = @shippingTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i).GetCell(59)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptyProductTypeID from pktblProductType where ptyProductType = @productType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@productType", master.readCellString(sh.GetRow(i).GetCell(14)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ProductType = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select prgProgramID from pktblProgram where prgProgramName = @programName";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@programName", master.readCellString(sh.GetRow(i).GetCell(7)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ProgramCode = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select OEMID from OEM where OEMName = @oem";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@oem", master.readCellString(sh.GetRow(i).GetCell(61)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.OEM = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(18)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i).GetCell(19)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.Parameters.Clear();

                            string currency = "";
                            if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("EURO"))
                            {
                                currency = "EUR";
                            }
                            else if(master.readCellString(sh.GetRow(i).GetCell(64)).Contains("USD"))
                            {
                                currency = "USD";
                            }
                            else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("CAD"))
                            {
                                currency = "CAD";
                            }
                            else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("GBP"))
                            {
                                currency = "GBP";
                            }

                            sql.CommandText = "Select curCurrencyID from pktblCurrency where curCurrency = @cur";
                            sql.Parameters.AddWithValue("@cur", currency);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.currency = dr.GetValue(0).ToString();
                            }
                            dr.Close();

                            int plantID = 0;

                            sql.Parameters.Clear();
                            sql.CommandText = "Select TSGSalesmanID, rfqCustomerID, rfqPlantID from CustomerLocation, tblRFQ where CustomerLocationID = rfqPlantID and rfqID = @rfq";
                            sql.Parameters.AddWithValue("@rfq", rfq);
                            dr = sql.ExecuteReader();
                            int salesmanID = 0;
                            int customerID = 0;
                            if (dr.Read())
                            {
                                salesmanID = System.Convert.ToInt32(dr.GetValue(0));
                                customerID = System.Convert.ToInt32(dr.GetValue(1));
                                plantID = System.Convert.ToInt32(dr.GetValue(2));
                            }
                            dr.Close();

                            //plant code in column 3
                            //Customer code in column 2

                            //BN for shipping location

                            sql.CommandText = "Select CustomerLocationID from CustomerLocation where CustomerID = @customerID and ShipCode = @shipCode";
                            sql.Parameters.AddWithValue("@customerID", customerID);
                            sql.Parameters.AddWithValue("@shipCode", master.readCellInt(sh.GetRow(i).GetCell(2)));
                            dr = sql.ExecuteReader();
                            if(dr.Read())
                            {
                                plantID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                            }
                            dr.Close();









                            //q.JobNumber = master.readCellInt(sh.GetRow(i).GetCell(0));
                            //if (q.JobNumber == -1)
                            //{
                            //    errorFlag = 1;
                            //    Response.Write("<script>alert('Your Job Number is not valid');</script>");
                            //    break;
                            //}
                            q.RFQID = System.Convert.ToInt32(rfq);

                            q.LeadTime = master.readCellInt(sh.GetRow(i).GetCell(27));
                            if (q.LeadTime == -1)
                            {
                                //errorFlag = 1;
                                q.LeadTimeString = master.readCellString(sh.GetRow(i).GetCell(27));
                                //context.Response.Write(FileName + " - Your lead time is not valid");
                            }
                            lineNumber = System.Convert.ToInt32(master.readCellString(sh.GetRow(i).GetCell(0)).Split('-')[1]);


                            List<int> noteIDs = new List<int>();
                            int j = i;
                            int loop = i;
                            q.TotalAmount = 0;
                            //Making sure to grab all the notes since they span over 47 lines for a single quote
                            loop += 67;
                            //loop += 59;

                            while (j < loop && errorFlag == 0)
                            {
                                try
                                {
                                    if ((sh.GetRow(j) != null))
                                    {
                                        int costColumn = 53;
                                        int costRow = j;
                                        //if (j >= 26)
                                        //{
                                        //    costColumn = 55;
                                        //    costRow--;
                                        //}
                                        if (sh.GetRow(j).GetCell(33, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(costRow).GetCell(costColumn, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null)
                                        {
                                            q.Description = "";
                                            string costNote = "";
                                            q.TotalAmount += master.readCellDouble(sh.GetRow(j).GetCell(costColumn));
                                            costNote = master.readCellDouble(sh.GetRow(j).GetCell(costColumn)).ToString();
                                            q.Description = master.readCellString(sh.GetRow(j).GetCell(33));

                                            if (costNote == "-1" || costNote == "0")
                                            {
                                                if (costNote == "-1")
                                                {
                                                    q.TotalAmount++;
                                                }
                                                costNote = "";
                                            }
                                            if (q.Description != "" || costNote != "")
                                            {
                                                int id = System.Convert.ToInt32(master.getCompanyId());
                                                //string lkjsdaf = master.getUserName();
                                                sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                                sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                                                sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                                                sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                                sql.Parameters.AddWithValue("@note", q.Description);
                                                sql.Parameters.AddWithValue("@costNote", costNote);
                                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                                int noteID = 0;
                                                try
                                                {
                                                    noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                                }
                                                catch
                                                {
                                                    errorFlag = 1;
                                                    context.Response.Write(FileName + "Something went wrong trying to upload your notes, please check your upload sheet for any errors");
                                                    break;
                                                }
                                                sql.Parameters.Clear();
                                                noteIDs.Add(noteID);
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }

                                j++;
                            }

                            sql.CommandText = "select prtPartID from tblPart, linkPartToRFQ where ptrPartID = prtPartID and ptrRFQID = @rfq and prtRFQLineNumber = @line";
                            sql.Parameters.AddWithValue("@rfq", q.RFQID);
                            sql.Parameters.AddWithValue("@line", lineNumber);

                            dr = sql.ExecuteReader();

                            while (dr.Read())
                            {
                                partID = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();
                            sql.Parameters.Clear();

                            int version = 0;
                            int quoteID = 0;
                            sql.CommandText = "Select count(ptqQuoteID), ptqQuoteID from linkPartToQuote where ptqPartID = @partID and ptqHTS = 0 and ptqUGS = 0 and ptqSTS = 0 Group By ptqQuoteID";
                            sql.Parameters.AddWithValue("@partID", partID);

                            dr = sql.ExecuteReader();
                            if(dr.Read())
                            {
                                version = System.Convert.ToInt32(dr.GetValue(0));
                                quoteID = System.Convert.ToInt32(dr.GetValue(1));
                            }
                            dr.Close();
                            sql.Parameters.Clear();

                            string oldQuoteNumber = "", oldQuoteCompany = "";

                            sql.Parameters.Clear();
                            sql.CommandText = "Select ptqQuoteID, quoTSGCompanyID, quoVersion, quoNumber from linkPartToQuote, tblQuote where ptqPartID = @partID and ";
                            sql.CommandText += "ptqQuoteID = quoQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and quoTSGCompanyID = 2 order by quoVersion desc";
                            sql.Parameters.AddWithValue("@partID", partID);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                oldQuoteNumber = dr.GetValue(0).ToString();
                                oldQuoteCompany = dr.GetValue(1).ToString();
                                version = System.Convert.ToInt32(dr.GetValue(2).ToString());
                                //number = System.Convert.ToInt32(dr.GetValue(3).ToString());
                                //if (dr.GetValue(1).ToString() == master.getCompanyId().ToString())
                                //{
                                //    deleteQuote(dr.GetValue(0).ToString());
                                //}
                            }
                            dr.Close();
                            sql.Parameters.Clear();

                            if (oldQuoteNumber != "" && oldQuoteCompany == master.getCompanyId().ToString())
                            {
                                string tempQuoteID = "";
                                sql.CommandText = "Select qtrRFQID, prtRFQLineNumber, quoQuoteID, quoOldQuoteNumber from linkPartToQuote, tblPart, linkQuoteToRFQ, tblQuote where qtrQuoteID = @quoteID and ";
                                sql.CommandText += "qtrQuoteID = ptqQuoteID and ptqPartID = prtPARTID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 and qtrHTS = 0 ";
                                sql.CommandText += "and qtrSTS = 0 and qtrUGS = 0 and ptqQuoteID = quoQuoteID";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@quoteID", oldQuoteNumber);
                                dr = sql.ExecuteReader();
                                if (dr.Read())
                                {
                                    oldQuoteNumber = dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString();
                                    if (dr.GetValue(3).ToString() != "")
                                    {
                                        oldQuoteNumber = dr.GetValue(3).ToString();
                                    }
                                    tempQuoteID = dr.GetValue(2).ToString();
                                }
                                dr.Close();
                                version++;

                                //deleteQuote(tempQuoteID);
                            }

                            int number = 0;

                            sql.CommandText = "Select quoNumber from tblQuote where quoQuoteID = @quoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                number = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();
                            sql.Parameters.Clear();

                            int logo = 0;
                            if(master.readCellString(sh.GetRow(i).GetCell(4)) != "ATS")
                            {
                                logo = 1;
                            }

                            double toolingCost = 0;
                            double tryoutMaterial = 0;
                            double transferBar = 0;
                            double fixtureCost = 0;
                            double dieSupport = 0;
                            double shippingCost = 0;
                            double additionalCost = 0;
                            //string additionalCostDesc = "";
                            double FormSteelCoating = 0;
                            double Qdc = 0;
                            double EaryParts = 0;
                            //double Finance = 0;
                            double Spare = 0;
                            toolingCost = master.readCellDouble(sh.GetRow(24).GetCell(53));
                            if (toolingCost == -1)
                            {
                                toolingCost = 0;
                            }
                            tryoutMaterial = master.readCellDouble(sh.GetRow(43).GetCell(53));
                            if (tryoutMaterial == -1)
                            {
                                tryoutMaterial = 0;
                            }
                            transferBar = master.readCellDouble(sh.GetRow(36).GetCell(53));
                            if (transferBar == -1)
                            {
                                transferBar = 0;
                            }
                            fixtureCost = master.readCellDouble(sh.GetRow(30).GetCell(53));
                            if (fixtureCost == -1)
                            {
                                fixtureCost = 0;
                            }
                            dieSupport = master.readCellDouble(sh.GetRow(27).GetCell(53));
                            if (dieSupport == -1)
                            {
                                dieSupport = 0;
                            }
                            shippingCost = master.readCellDouble(sh.GetRow(26).GetCell(53));
                            if (shippingCost == -1)
                            {
                                shippingCost = 0;
                            }
                            additionalCost = master.readCellDouble(sh.GetRow(62).GetCell(53));
                            if (additionalCost == -1)
                            {
                                additionalCost = 0;
                            }
                            try
                            {
                                FormSteelCoating = master.readCellDouble(sh.GetRow(29).GetCell(33));
                                if (FormSteelCoating == -1)
                                {
                                    FormSteelCoating = 0;
                                }
                                Qdc = master.readCellDouble(sh.GetRow(25).GetCell(53));
                                if (Qdc == -1)
                                {
                                    Qdc = 0;
                                }
                                EaryParts = master.readCellDouble(sh.GetRow(35).GetCell(53));
                                if (EaryParts == -1)
                                {
                                    EaryParts = 0;
                                }
                                //Finance = master.readCellDouble(sh.GetRow(i + 6).GetCell(1));
                                //if (Finance == -1)
                                //{
                                //    Finance = 0;
                                //}
                                Spare = master.readCellDouble(sh.GetRow(41).GetCell(53));
                                if (Spare == -1)
                                {
                                    Spare = 0;
                                }
                            }
                            catch
                            {

                            }

                            //Insert into quote table
                            sql.CommandText = "insert into tblQuote (quoTSGCompanyID, quoRFQID, quoEstimatorID, quoPaymentTermsID, quoShippingTermsID, ";
                            sql.CommandText += "quoTotalAmount, quoPartTypeID, quoToolCountryID, quoLeadTime, quoCreated, quoCreatedBy, quoSalesman, quoStatusID, quoNumber, quoVersion, quoUseTSGLogo, quoUseTSGName, quoPartNumbers, quoCurrencyID, quoCustomerQuoteNumber, quoOldQuoteNumber, quoPlant, quoShippingLocation, quoToolingCost, quoTransferBarCost, quoFixtureCost, quoDieSupportCost, quoShippingCost, quoAdditCost, quoFormSteelCost, quoQDCCost, quoEarlyPartsCost, quoSpareCost ) ";
                            sql.CommandText += "Output inserted.quoQuoteID ";
                            sql.CommandText += "Values ( @company, @rfq, @estimator, @paymentTerms, @shippingTerms, @totalAmount, @partType, @toolCountry, @leadTime, GETDATE(), @createdBy, @salesman, @status, @number, @version, @logo, @name, @partNumbers, @currency, @custQuote, @oldQuoteNumber, @quoPlant, @shippingLocation, @ToolingCost, @TransferBarCost, @FixtureCost, @DieSupportCost, @ShippingCost, @AdditCost, @FormSteelCost, @QDCCost, @EarlyPartsCost, @SpareCost )";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@company", master.getCompanyId());
                            sql.Parameters.AddWithValue("@rfq", q.RFQID);
                            sql.Parameters.AddWithValue("@estimator", q.EstimatorID);
                            //sql.Parameters.AddWithValue("@jobNumber", q.JobNumber);
                            sql.Parameters.AddWithValue("@paymentTerms", q.PaymentTerms);
                            sql.Parameters.AddWithValue("@shippingTerms", q.ShippingTerms);
                            sql.Parameters.AddWithValue("@totalAmount", q.TotalAmount);
                            sql.Parameters.AddWithValue("@partType", q.PartType);
                            sql.Parameters.AddWithValue("@toolCountry", q.ToolCountry);
                            if(q.LeadTime != -1)
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTime);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTimeString);
                            }
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            sql.Parameters.AddWithValue("@salesman", salesmanID);
                            sql.Parameters.AddWithValue("@status", 2);
                            sql.Parameters.AddWithValue("@number", number);
                            sql.Parameters.AddWithValue("@version", String.Format("{0:000}", version));
                            sql.Parameters.AddWithValue("@logo", logo);
                            sql.Parameters.AddWithValue("@name", logo);
                            sql.Parameters.AddWithValue("@partNumbers", master.readCellString(sh.GetRow(i).GetCell(5)));
                            sql.Parameters.AddWithValue("@currency", q.currency);
                            sql.Parameters.AddWithValue("@custQuote", master.readCellString(sh.GetRow(i).GetCell(23)));
                            sql.Parameters.AddWithValue("@oldQuoteNumber", oldQuoteNumber);
                            sql.Parameters.AddWithValue("@quoPlant", plantID);
                            sql.Parameters.AddWithValue("@shippingLocation", master.readCellString(sh.GetRow(i).GetCell(65)));
                            sql.Parameters.AddWithValue("@ToolingCost", toolingCost);
                            sql.Parameters.AddWithValue("@tryoutMaterial", tryoutMaterial);
                            sql.Parameters.AddWithValue("@TransferBarCost", transferBar);
                            sql.Parameters.AddWithValue("@fixtureCost", fixtureCost);
                            sql.Parameters.AddWithValue("@DieSupportCost", dieSupport);
                            sql.Parameters.AddWithValue("@shippingCost", shippingCost);
                            sql.Parameters.AddWithValue("@AdditCost", additionalCost);
                            sql.Parameters.AddWithValue("@FormSteelCost", FormSteelCoating);
                            sql.Parameters.AddWithValue("@QDCCost", Qdc);
                            sql.Parameters.AddWithValue("@EarlyPartsCost", EaryParts);
                            sql.Parameters.AddWithValue("@SpareCost", Spare);

                            if (q.ShippingTerms == 0)
                            {
                                errorFlag = 1;
                                context.Response.Write("Your shipping terms are incorrect or not filled out, please try to fill it out and reupload\n");
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                            }
                            else if (q.PaymentTerms == 0)
                            {
                                errorFlag = 1;
                                context.Response.Write("Your payment terms are incorrect or not filled out, please try to fill it out and reupload\n");
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                            }
                            else if (q.EstimatorID == 0)
                            {
                                errorFlag = 1;
                                context.Response.Write("Your estimator is not filled out, please try to fill it out and reupload\n");
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                            }
                            else if (q.PartType == 0)
                            {
                                errorFlag = 1;
                                context.Response.Write("Your part type may be filled out incorrectly or not filled out, please try to fill it out and reupload\n");
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                            }
                            else if (q.ToolCountry == 0)
                            {
                                errorFlag = 1;
                                context.Response.Write("Your tool country may be filled out incorrectly or not filled out, please try to fill it out and reupload\n");
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                            }

                            quoteID = 0;
                            try
                            {
                                if(errorFlag == 0)
                                {
                                    quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                }
                            }
                            catch (Exception err)
                            {
                                errorFlag = 1;
                                context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors " + err.ToString());

                                //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                                //Breaking so we dont try and insert anything else
                            }
                            sql.Parameters.Clear();

                            if(number == 0 && version == 0 && errorFlag == 0)
                            {
                                sql.CommandText = "Update tblQuote set quoNumber = @number, quoVersion = @version where quoQuoteID = @quoteID";
                                sql.Parameters.AddWithValue("@number", quoteID);
                                sql.Parameters.AddWithValue("@version", String.Format("{0:000}", 1));
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                master.ExecuteNonQuery(sql, "QuoteUpload");
                            }

                            sql.Parameters.Clear();
                            //Inserting link for all notes previously uploaded
                            if(errorFlag == 0)
                            {
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();

                                    sql.CommandText = "Insert into linkPWNToQuote (pwqQuoteID, pwqPreWordedNoteID, pwqCreated, pwqCreatedBy) ";
                                    sql.CommandText += "output inserted.pwqPWNToQuoteID ";
                                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }

                                sql.Parameters.Clear();

                                //Linking part to quote
                                sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                                sql.CommandText += "output inserted.ptqPartToQuoteID ";
                                sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, 0, 0, 0);";
                                sql.Parameters.AddWithValue("@partID", partID);
                                sql.Parameters.AddWithValue("@quoteID", quoteID);
                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                master.ExecuteNonQuery(sql, "EditRFQ");

                                sql.Parameters.Clear();

                                sql.CommandText = "Update tblPart set prtPartTypeID = @partTypeID, prtpartDescription = @partDesc where prtPARTID = @partID";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@partID", partID);
                                sql.Parameters.AddWithValue("@partTypeID", q.PartType);
                                sql.Parameters.AddWithValue("@partDesc", master.readCellString(sh.GetRow(i).GetCell(6)));
                                master.ExecuteNonQuery(sql, "Quote Upload");


                                sql.Parameters.Clear();

                                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and ppdPartID <> @partID";
                                sql.Parameters.AddWithValue("@partID", partID);
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
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                                sql.Parameters.Clear();


                                sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(26)));
                                dr = sql.ExecuteReader();

                                int matID = 0;
                                if (dr.Read())
                                {
                                    matID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                                }
                                dr.Close();

                                if (matID == 0)
                                {
                                    sql.CommandText = "insert into pktblMaterialType(mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                                    sql.CommandText += "output inserted.mtyMaterialTypeID ";
                                    sql.CommandText += "values(@matType, GETDATE(), @user) ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(26)));
                                    sql.Parameters.AddWithValue("@user", master.getUserName());
                                    matID = System.Convert.ToInt32(master.ExecuteScalar(sql, "QuoteUpload"));
                                }


                                sql.CommandText = "Insert into pktblBlankInfo (binBlankMaterialTypeID, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, ";
                                sql.CommandText += "binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialWeightEnglish, binMaterialWeightMetric, binCreated, binCreatedBy) ";
                                sql.CommandText += "Output inserted.binBlankInfoID ";
                                sql.CommandText += "Values(@matType, @thickEng, @thickMet, @pitchEng, @pitchMet, @widthEng, @widthMet, @weightEng, @weightMet, GETDATE(), @user)";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@matType", matID);
                                sql.Parameters.AddWithValue("@thickEng", master.readCellDouble(sh.GetRow(i).GetCell(39)) / 25.4);
                                //in to mm is 25.4
                                sql.Parameters.AddWithValue("@thickMet", master.readCellDouble(sh.GetRow(i).GetCell(39)));
                                sql.Parameters.AddWithValue("@pitchEng", master.readCellDouble(sh.GetRow(i).GetCell(36)));
                                sql.Parameters.AddWithValue("@pitchMet", master.readCellDouble(sh.GetRow(i).GetCell(36)) * 25.4);
                                sql.Parameters.AddWithValue("@widthEng", master.readCellDouble(sh.GetRow(i).GetCell(34)));
                                sql.Parameters.AddWithValue("@widthMet", master.readCellDouble(sh.GetRow(i).GetCell(34)) * 25.4);
                                sql.Parameters.AddWithValue("@weightEng", 0);
                                //lbs to kg is 0.453592
                                sql.Parameters.AddWithValue("@weightMet", 0);
                                sql.Parameters.AddWithValue("@user", master.getUserName());
                                int blankInfoID = 0;
                                try
                                {
                                    blankInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Quote Upload"));
                                }
                                catch (Exception err)
                                {
                                    //If we caught an error we want to make sure that we delete everything that was already inserted
                                    //We first start with link tables so the queries don't fail because of foreign keys
                                    errorFlag = 1;
                                    context.Response.Write(FileName + " - Something went wrong trying to upload the die info, please check your upload sheet for any errors " + err.Message);

                                    sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    master.ExecuteNonQuery(sql, "EditRFQ");

                                    sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    master.ExecuteNonQuery(sql, "EditRFQ");

                                    sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                    for (int k = 0; k < noteIDs.Count; k++)
                                    {
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                        master.ExecuteNonQuery(sql, "EditRFQ");
                                    }
                                    sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }

                                if(errorFlag == 0)
                                {
                                    sql.Parameters.Clear();
                                    sql.CommandText = "Update tblQuote set quoBlankInfoID = @blankID where quoQuoteID = @quoteID";
                                    sql.Parameters.AddWithValue("@blankID", blankInfoID);
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    master.ExecuteNonQuery(sql, "Quote Upload");

                                    string die = master.readCellString(sh.GetRow(i).GetCell(16));
                                    string cavityType = master.readCellString(sh.GetRow(i).GetCell(17));

                                    //Starting on the die info
                                    sql.CommandText = "Select DieTypeID, CavCavityID from pktblCavity, DieType where dtyFullName = '" + die + "' and TSGCompanyID = 2 and cavCavityName = '" + cavityType + "'";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@die", die);
                                    //sql.Parameters.AddWithValue("@company", q.TSGCompanyID);
                                    sql.Parameters.AddWithValue("@cavity", cavityType);
                                    dr = sql.ExecuteReader();

                                    while (dr.Read())
                                    {
                                        d.DieType = System.Convert.ToInt32(dr.GetValue(0));
                                        d.CavityType = System.Convert.ToInt32(dr.GetValue(1));
                                    }
                                    dr.Close();

                                    sql.Parameters.Clear();
                                    if(master.readCellDouble(sh.GetRow(i).GetCell(40)) == -1)
                                    {
                                        d.FtoBEnglish = master.readCellInt(sh.GetRow(i).GetCell(40));
                                    }
                                    else
                                    {
                                        d.FtoBEnglish = master.readCellDouble(sh.GetRow(i).GetCell(40));
                                    }
                                    if (d.FtoBEnglish == -1)
                                    {
                                        d.FtoBEnglish = 0;
                                    }

                                    d.FtoBMetric = d.FtoBEnglish * 25.4;

                                    if (master.readCellDouble(sh.GetRow(i).GetCell(42)) == -1)
                                    {
                                        d.LtoREnglish = master.readCellInt(sh.GetRow(i).GetCell(42));

                                    }
                                    else
                                    {
                                        d.LtoREnglish = master.readCellDouble(sh.GetRow(i).GetCell(42));
                                    }
                                    if (d.LtoREnglish == -1)
                                    {
                                        d.LtoREnglish = 0;
                                    }

                                    d.LtoRMetric = d.LtoREnglish * 25.4;

                                    if (master.readCellDouble(sh.GetRow(i).GetCell(46)) == -1)
                                    {
                                        d.ShutHeightEnglish = master.readCellInt(sh.GetRow(i).GetCell(46));
                                    }
                                    else
                                    {
                                        d.ShutHeightEnglish = master.readCellDouble(sh.GetRow(i).GetCell(46));
                                    }
                                    if(d.ShutHeightEnglish == -1)
                                    {
                                        d.ShutHeightEnglish = 0;
                                    }
                                    d.ShutHeightMetric = d.ShutHeightEnglish * 25.4;

                                    d.NumberOfStations = master.readCellString(sh.GetRow(i).GetCell(50));
                                    if (d.NumberOfStations == "")
                                    {
                                        d.NumberOfStations = master.readCellInt(sh.GetRow(i).GetCell(50)).ToString();
                                    }

                                    //Inserting die info
                                    sql.CommandText = "insert into tblDieInfo (dinDieType, dinCavityID, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, ";
                                    sql.CommandText += "dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, dinCreated, dinCreatedBy) ";
                                    sql.CommandText += "Output inserted.dinDieInfoID ";
                                    sql.CommandText += "Values (@dieType, @cavity, @fToBEng, @fToBMet, @lToREng, @lToRMet, @shutHiehgtEng, @shutHeightMet, @numOfStations, GETDATE(), @createdBy )";

                                    sql.Parameters.AddWithValue("@dieType", d.DieType);
                                    sql.Parameters.AddWithValue("@cavity", d.CavityType);
                                    sql.Parameters.AddWithValue("@fToBEng", d.FtoBEnglish);
                                    sql.Parameters.AddWithValue("@fToBMet", d.FtoBMetric);
                                    sql.Parameters.AddWithValue("@lToREng", d.LtoREnglish);
                                    sql.Parameters.AddWithValue("@lToRMet", d.LtoRMetric);
                                    sql.Parameters.AddWithValue("@shutHiehgtEng", d.ShutHeightEnglish);
                                    sql.Parameters.AddWithValue("@shutHeightMet", d.ShutHeightMetric);
                                    sql.Parameters.AddWithValue("@numOfStations", d.NumberOfStations);
                                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                                    if (d.DieType == 0)
                                    {
                                        errorFlag = 1;
                                        context.Response.Write(FileName + "Your die type may be filled out incorrectly or not at all, please check and reupload");

                                        sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@blankID", blankInfoID);
                                        master.ExecuteNonQuery(sql, "Quote Upload");

                                        sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                        for (int k = 0; k < noteIDs.Count; k++)
                                        {
                                            sql.Parameters.Clear();
                                            sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                            master.ExecuteNonQuery(sql, "EditRFQ");
                                        }
                                        sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");
                                        return;
                                    }
                                    else if (d.CavityType == 0)
                                    {
                                        errorFlag = 1;
                                        context.Response.Write(FileName + "Your cavity type may be filled out incorrectly or not at all, please check and reupload");

                                        sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@blankID", blankInfoID);
                                        master.ExecuteNonQuery(sql, "Quote Upload");

                                        sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                        for (int k = 0; k < noteIDs.Count; k++)
                                        {
                                            sql.Parameters.Clear();
                                            sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                            master.ExecuteNonQuery(sql, "EditRFQ");
                                        }
                                        sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");
                                        return;
                                    }

                                    int dieInfoID = 0;
                                    try
                                    {
                                        dieInfoID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                    }
                                    catch (Exception err)
                                    {

                                        //If we caught an error we want to make sure that we delete everything that was already inserted
                                        //We first start with link tables so the queries don't fail because of foreign keys
                                        errorFlag = 1;
                                        context.Response.Write(FileName + " - Something went wrong trying to upload the die info, please check your upload sheet for any errors " + err.Message);

                                        sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@blankID", blankInfoID);
                                        master.ExecuteNonQuery(sql, "Quote Upload");

                                        sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                        for (int k = 0; k < noteIDs.Count; k++)
                                        {
                                            sql.Parameters.Clear();
                                            sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                            master.ExecuteNonQuery(sql, "EditRFQ");
                                        }
                                        sql.CommandText = "Delete from tblQuote where quoQuoteID = @quoteID";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        master.ExecuteNonQuery(sql, "EditRFQ");
                                        return;
                                    }
                                    sql.Parameters.Clear();
                                    if (errorFlag == 0)
                                    {
                                        sql.CommandText = "insert into linkQuoteToRFQ(qtrQuoteID, qtrRFQID, qtrCreated, qtrCreatedBy, qtrHTS, qtrSTS, qtrUGS) ";
                                        sql.CommandText += "values(@quoteID, @rfqID, GETDATE(), @createdBy, 0, 0, 0)";
                                        sql.Parameters.AddWithValue("@rfqID", rfq);
                                        sql.Parameters.AddWithValue("@quoteID", quoteID);
                                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.Parameters.Clear();

                                        sql.CommandText = "insert into linkDieInfoToQuote (diqDieInfoID, diqQuoteID, diqCreated, diqCreatedBy) ";
                                        sql.CommandText += "output inserted.diqDieInfoToQuoteID ";
                                        sql.CommandText += "values (@dieInfo, @quote, GETDATE(), @createdBy)";
                                        sql.Parameters.AddWithValue("@dieInfo", dieInfoID);
                                        sql.Parameters.AddWithValue("@quote", quoteID);
                                        sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                        master.ExecuteNonQuery(sql, "EditRFQ");

                                        sql.Parameters.Clear();
                                        sql.CommandText = "Select gnoGeneralNoteID from pktblGeneralNote where gnoDefault = 1 and gnoCompany = @company";
                                        sql.Parameters.AddWithValue("@company", "general");


                                        dr = sql.ExecuteReader();
                                        List<string> genIDs = new List<string>();
                                        while (dr.Read())
                                        {
                                            genIDs.Add(dr.GetValue(0).ToString());
                                        }
                                        dr.Close();

                                        for (int k = 0; k < genIDs.Count; k++)
                                        {
                                            sql.Parameters.Clear();
                                            sql.CommandText = "Insert into linkGeneralNoteToQuote (gnqGeneralNoteID, gnqQuoteID, gnqCreated, gnqCreatedBy) ";
                                            sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @created)";
                                            sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                                            sql.Parameters.AddWithValue("@quoteID", quoteID);
                                            sql.Parameters.AddWithValue("@created", master.getUserName());
                                            master.ExecuteNonQuery(sql, "Quote Upload");
                                        }


                                        sql.Parameters.Clear();
                                        sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
                                        sql.Parameters.AddWithValue("@rfq", rfq);
                                        master.ExecuteNonQuery(sql, "GetLinkedParts");

                                        row++;
                                    }
                                }
                            }
                            connection.Close();
                        }
                    }
                    catch (Exception err)
                    {
                        
                    }
                }
            }
            if (errorFlag == 0)
            {
                string html = master.renderQuotingHTML(partID.ToString(), "1", rfq, true);
                context.Response.Write("OK|" + partID + "|" + html);
            }
            return;
        }


        //ATS's EC QUOTE UPLOAD
        public void ProcessATSEC(HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 1; // skip the header row
            string lineNumber = "0";
            Site master = new RFQ.Site();
            string finalQuoteID = "";
            if (sh != null)
            {
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                if (sh.GetRow(i) != null)
                {
                    try
                    {
                        lineNumber = master.readCellString(sh.GetRow(i).GetCell(0));

                        q = new Quote();
                        d = new DieInfo();
                        row = 0;

                        if (row == 0)
                        {
                            SqlConnection connection = new SqlConnection(master.getConnectionString());
                            connection.Open();
                            SqlCommand sql = new SqlCommand();
                            sql.Connection = connection;
                            SqlDataReader dr;

                            q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());

                            sql.CommandText = "Select estEstimatorID from pktblEstimators where estLastName = @lastName";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@lastName", master.readCellString(sh.GetRow(i).GetCell(32)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptePaymentTermsID from pktblPaymentTerms where ptePaymentTerms = @paymentTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i).GetCell(58)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select steShippingTermsID from pktblShippingTerms where steShippingTerms = @shippingTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i).GetCell(59)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptyProductTypeID from pktblProductType where ptyProductType = @productType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@productType", master.readCellString(sh.GetRow(i).GetCell(14)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ProductType = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select prgProgramID from pktblProgram where prgProgramName = @programName";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@programName", master.readCellString(sh.GetRow(i).GetCell(7)));
                            dr = sql.ExecuteReader();
                            q.ProgramCode = 2062;
                            if (dr.Read())
                            {
                                q.ProgramCode = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select OEMID from OEM where OEMName = @oem";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@oem", master.readCellString(sh.GetRow(i).GetCell(61)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.OEM = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(18)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i).GetCell(19)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            string dieType = "25";
                            sql.CommandText = "Select DieTypeID from DieType where TSGCompanyID = 2 and dtyFullName = @name";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@name", master.readCellString(sh.GetRow(i).GetCell(16)));
                            dr = sql.ExecuteReader();
                            if(dr.Read())
                            {
                                dieType = dr.GetValue(0).ToString();
                            }
                            dr.Close();

                            string cavity = "1";
                            sql.CommandText = "Select cavCavityID from pktblCavity where cavCavityName = @cavity";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@cavity", master.readCellString(sh.GetRow(i).GetCell(17)));
                            dr = sql.ExecuteReader();
                            if(dr.Read())
                            {
                                cavity = dr.GetValue(0).ToString();
                            }
                            dr.Close();

                            sql.Parameters.Clear();

                            string currency = "USD";
                            if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("EURO"))
                            {
                                currency = "EUR";
                            }
                            else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("USD"))
                            {
                                currency = "USD";
                            }
                            else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("CAD"))
                            {
                                currency = "CAD";
                            }
                            else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("GBP"))
                            {
                                currency = "GBP";
                            }

                            sql.CommandText = "Select curCurrencyID from pktblCurrency where curCurrency = @cur";
                            sql.Parameters.AddWithValue("@cur", currency);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.currency = dr.GetValue(0).ToString();
                            }
                            dr.Close();

                            string customernumber = master.readCellInt(sh.GetRow(i).GetCell(1)).ToString("0000000");
                            string shipCode = master.readCellInt(sh.GetRow(i).GetCell(2)).ToString("0000");

                            sql.Parameters.Clear();
                            sql.CommandText = "Select TSGSalesmanID, CustomerLocationID, Customer.CustomerID from Customer, CustomerLocation where Customer.CustomerNumber = @customerNumber and Customer.CustomerNumber = CustomerLocation.CustomerNumber and CustomerLocation.ShipCode = @shipCode";
                            sql.Parameters.AddWithValue("@customerNumber", customernumber);
                            sql.Parameters.AddWithValue("@shipCode", shipCode);
                            //sql.Parameters.AddWithValue("@customerNumber", "0000093");
                            //sql.Parameters.AddWithValue("@shipCode", "0019");
                            dr = sql.ExecuteReader();
                            int salesmanID = 0;
                            int customerID = master.readCellInt(sh.GetRow(i).GetCell(1));
                            int customerLocation = 0;

                            if (dr.Read())
                            {
                                salesmanID = System.Convert.ToInt32(dr.GetValue(0));
                                customerID = System.Convert.ToInt32(dr.GetValue(2));
                                customerLocation = System.Convert.ToInt32(dr.GetValue(1));
                            }
                            dr.Close();


                            sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(26)));
                            dr = sql.ExecuteReader();

                            int matID = 0;
                            if (dr.Read())
                            {
                                matID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                            }
                            dr.Close();

                            if (matID == 0)
                            {
                                sql.CommandText = "insert into pktblMaterialType(mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                                sql.CommandText += "output inserted.mtyMaterialTypeID ";
                                sql.CommandText += "values(@matType, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i).GetCell(26)));
                                sql.Parameters.AddWithValue("@user", master.getUserName());
                                matID = System.Convert.ToInt32(master.ExecuteScalar(sql, "QuoteUpload"));
                            }
                            sql.Parameters.Clear();

                            string jobNum = master.readCellInt(sh.GetRow(i).GetCell(57)).ToString();
                            if (jobNum == "-1")
                            {
                                jobNum = master.readCellString(sh.GetRow(i).GetCell(57));
                            }


                            q.RFQID = System.Convert.ToInt32(rfq);

                            q.LeadTime = master.readCellInt(sh.GetRow(i).GetCell(27));
                            if (q.LeadTime == -1)
                            {
                                q.LeadTimeString = master.readCellString(sh.GetRow(i).GetCell(27));
                                //errorFlag = 1;
                                //context.Response.Write(FileName + " - Your lead time is not valid");
                            }



                            List<int> noteIDs = new List<int>();
                            int j = i;
                            int loop = i;
                            q.TotalAmount = 0;
                            //Making sure to grab all the notes since they span over 47 lines for a single quote
                            loop += 66;

                            while (j < loop && errorFlag == 0)
                            {
                                try
                                {
                                    if ((sh.GetRow(j) != null))
                                    {
                                        int costColumn = 53;
                                        int costRow = j;
                                        //if (j >= 14)
                                        //{
                                        //    costColumn = 55;
                                        //    costRow--;
                                        //}
                                        if (sh.GetRow(j).GetCell(33, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(costRow).GetCell(costColumn, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null)
                                        {
                                            q.Description = "";
                                            string costNote = "";
                                            q.TotalAmount += master.readCellDouble(sh.GetRow(j).GetCell(costColumn));
                                            costNote = master.readCellDouble(sh.GetRow(costRow).GetCell(costColumn)).ToString();
                                            q.Description = master.readCellString(sh.GetRow(j).GetCell(33));

                                            if (costNote == "-1" || costNote == "0")
                                            {
                                                if (costNote == "-1")
                                                {
                                                    q.TotalAmount++;
                                                }
                                                costNote = "";
                                            }
                                            if (q.Description != "" || costNote != "")
                                            {
                                                int id = System.Convert.ToInt32(master.getCompanyId());
                                                string lkjsdaf = master.getUserName();
                                                sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                                sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                                                sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                                                sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                                sql.Parameters.AddWithValue("@note", q.Description);
                                                sql.Parameters.AddWithValue("@costNote", costNote);
                                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                                int noteID = 0;
                                                try
                                                {
                                                    noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                                }
                                                catch
                                                {
                                                    errorFlag = 1;
                                                    context.Response.Write(FileName + "Something went wrong trying to upload your notes, please check your upload sheet for any errors");
                                                    break;
                                                }
                                                sql.Parameters.Clear();
                                                noteIDs.Add(noteID);
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }

                                j++;
                            }

                            d.FtoBEnglish = master.readCellDouble(sh.GetRow(i).GetCell(40));
                            d.FtoBMetric = d.FtoBEnglish * 25.4;

                            d.LtoREnglish = master.readCellDouble(sh.GetRow(i).GetCell(42));
                            d.LtoRMetric = d.LtoREnglish * 25.4;

                            d.ShutHeightEnglish = master.readCellDouble(sh.GetRow(i).GetCell(46));
                            d.ShutHeightMetric = d.ShutHeightEnglish * 25.4;

                            d.NumberOfStations = master.readCellString(sh.GetRow(i).GetCell(50));
                            if (d.NumberOfStations == "")
                            {
                                d.NumberOfStations = master.readCellInt(sh.GetRow(i).GetCell(50)).ToString();
                            }


                            sql.Parameters.Clear();

                            int logo = 0;
                            if (master.readCellString(sh.GetRow(i).GetCell(4)) != "ATS")
                            {
                                logo = 1;
                            }

                            string oldQuoteNumber = master.readCellInt(sh.GetRow(i).GetCell(0)).ToString();
                            if (oldQuoteNumber == "-1")
                            {
                                oldQuoteNumber = master.readCellString(sh.GetRow(i).GetCell(0)).ToString();
                            }
                            try
                            {
                                oldQuoteNumber = oldQuoteNumber.Split('-')[0];
                            }
                            catch
                            {

                            }

                            int parse = 0;
                            int.TryParse(oldQuoteNumber, out parse);

                            string quoteVersion = "001";

                            if (oldQuoteNumber != "" && parse != 0)
                            {
                                sql.CommandText = "Select ecqVersion from tblECQuote where (ecqECQuoteID = @id or ecqQuoteNumber = @id) and ecqTSGCompanyID = 2 order by ecqVersion desc ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@id", oldQuoteNumber);
                                dr = sql.ExecuteReader();
                                if (dr.Read())
                                {
                                    quoteVersion = (System.Convert.ToInt32(dr.GetValue(0).ToString()) + 1).ToString("000");
                                }
                                dr.Close();
                            }


                            //Insert into quote table
                            sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                            sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                            sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqVersion, ecqCustomerContactName, ecqShippingLocation ) ";
                            sql.CommandText += "Output inserted.ecqECQuoteID ";
                            sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                            sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @version, @customerContact, @shippingLocation )";

                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partNum", master.readCellString(sh.GetRow(i).GetCell(5)));
                            sql.Parameters.AddWithValue("@partName", master.readCellString(sh.GetRow(i).GetCell(6)));
                            sql.Parameters.AddWithValue("@rfqNum", master.readCellString(sh.GetRow(i).GetCell(23)));
                            sql.Parameters.AddWithValue("@customer", customerID);
                            sql.Parameters.AddWithValue("@CustomerLocation", customerLocation);
                            sql.Parameters.AddWithValue("@customerRFQ", master.readCellString(sh.GetRow(i).GetCell(23)));
                            sql.Parameters.AddWithValue("@dieType", dieType);
                            sql.Parameters.AddWithValue("@cavity", cavity);


                            sql.Parameters.AddWithValue("@matType", matID);
                            sql.Parameters.AddWithValue("@matThkEng", master.readCellDouble(sh.GetRow(i).GetCell(39)) / 25.4);
                            //in to mm is 25.4
                            sql.Parameters.AddWithValue("@matThkMet", master.readCellDouble(sh.GetRow(i).GetCell(39)));
                            sql.Parameters.AddWithValue("@blankPitchEng", master.readCellDouble(sh.GetRow(i).GetCell(36)));
                            sql.Parameters.AddWithValue("@blankPitchMet", master.readCellDouble(sh.GetRow(i).GetCell(36)) * 25.4);
                            sql.Parameters.AddWithValue("@blankWidthEng", master.readCellDouble(sh.GetRow(i).GetCell(34)));
                            sql.Parameters.AddWithValue("@blankWidthMet", master.readCellDouble(sh.GetRow(i).GetCell(34)) * 25.4);
                            sql.Parameters.AddWithValue("@weightEng", 0);
                            //lbs to kg is 0.453592
                            sql.Parameters.AddWithValue("@weightMet", 0);



                            sql.Parameters.AddWithValue("@FBEng", d.FtoBEnglish);
                            sql.Parameters.AddWithValue("@FBMet", d.FtoBMetric);
                            sql.Parameters.AddWithValue("@LREng", d.LtoREnglish);
                            sql.Parameters.AddWithValue("@LRMet", d.LtoRMetric);
                            sql.Parameters.AddWithValue("@shutHeightEng", d.ShutHeightEnglish);
                            sql.Parameters.AddWithValue("@shutHeightMet", d.ShutHeightMetric);
                            sql.Parameters.AddWithValue("@stations", d.NumberOfStations);

                            if(q.LeadTime != -1)
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTime);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTimeString);
                            }
                            sql.Parameters.AddWithValue("@shipping", q.ShippingTerms);
                            sql.Parameters.AddWithValue("@payment", q.PaymentTerms);
                            sql.Parameters.AddWithValue("@country", q.ToolCountry);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                            sql.Parameters.AddWithValue("@totalCost", q.TotalAmount);
                            sql.Parameters.AddWithValue("@status", 2);
                            sql.Parameters.AddWithValue("@salesman", salesmanID);
                            sql.Parameters.AddWithValue("@estimator", q.EstimatorID);
                            sql.Parameters.AddWithValue("@jobNumber", jobNum);
                            sql.Parameters.AddWithValue("@accessNumber", "");
                            sql.Parameters.AddWithValue("@useTSG", logo);
                            sql.Parameters.AddWithValue("@version", quoteVersion);
                            sql.Parameters.AddWithValue("@customerContact", master.readCellString(sh.GetRow(i).GetCell(3)));
                            sql.Parameters.AddWithValue("@shippingLocation", master.readCellString(sh.GetRow(i).GetCell(65)));



                            int quoteID = 0;
                            try
                            {
                                if (errorFlag == 0)
                                {
                                    quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                }
                            }
                            catch (Exception err)
                            {
                                errorFlag = 1;
                                context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors " + err.ToString());

                                //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                                //Breaking so we dont try and insert anything else
                            }

                            sql.Parameters.Clear();

                            String pictureName = "EC-" + quoteID + ".png";

                            sql.CommandText = "Update tblECQuote set ecqPicture = @picture, ecqQuoteNumber = @quoteNumber where ecqECQuoteID = @ecQuoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@ecQuoteID", quoteID);
                            sql.Parameters.AddWithValue("@picture", pictureName);
                            if (quoteVersion != "001")
                            {
                                sql.Parameters.AddWithValue("@quoteNumber", oldQuoteNumber);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@quoteNumber", quoteID);
                            }
                            master.ExecuteNonQuery(sql, "Edit Quote");


                            //Inserting link for all notes previously uploaded
                            if (errorFlag == 0)
                            {
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();

                                    sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                                    sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }

                                sql.Parameters.Clear();


                                List<string> genIDs = new List<string>();
                                genIDs.Add("3");
                                genIDs.Add("4");
                                genIDs.Add("5");
                                genIDs.Add("6");
                                genIDs.Add("7");
                                genIDs.Add("8");
                                for (int k = 0; k < genIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                                    sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @created)";
                                    sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    sql.Parameters.AddWithValue("@created", master.getUserName());
                                    master.ExecuteNonQuery(sql, "Quote Upload");
                                }
                                row++;
                            }
                            finalQuoteID = quoteID.ToString();
                            connection.Close();
                        }
                        
                    }
                    catch
                    {

                    }
                }
            }
            if (errorFlag == 0)
            {
                string html = finalQuoteID;
                context.Response.Write("OK|" + partID + "|" + html);
            }
            return;
        }

        public void processEC (HttpContext context, XSSFSheet sh, Int64 rfq, string FileName)
        {
            int errorFlag = 0;
            int partID = 0;
            int i = 1; // skip the header row
            int lineNumber = 0;
            Site master = new RFQ.Site();
            string finalQuoteID = "";
            if (sh != null)
            {
                Quote q = new Quote();
                DieInfo d = new DieInfo();
                int row = 0;
                if (sh.GetRow(i) != null)
                {
                    try
                    {
                        lineNumber = master.readCellInt(sh.GetRow(i).GetCell(0));

                        q = new Quote();
                        d = new DieInfo();
                        row = 0;

                        if (row == 0)
                        {
                            SqlConnection connection = new SqlConnection(master.getConnectionString());
                            connection.Open();
                            SqlCommand sql = new SqlCommand();
                            sql.Connection = connection;
                            SqlDataReader dr;

                            q.TSGCompanyID = System.Convert.ToInt32(master.getCompanyId());

                            sql.CommandText = "Select estEstimatorID from pktblEstimators where estEmail = @email";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@email", master.readCellString(sh.GetRow(i).GetCell(5)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.EstimatorID = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select ptePaymentTermsID from pktblPaymentTerms where ptePaymentTerms = @paymentTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@paymentTerms", master.readCellString(sh.GetRow(i+4).GetCell(2)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.PaymentTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            sql.CommandText = "Select steShippingTermsID from pktblShippingTerms where steShippingTerms = @shippingTerms";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@shippingTerms", master.readCellString(sh.GetRow(i+4).GetCell(3)));
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.ShippingTerms = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();

                            //sql.CommandText = "Select ptyProductTypeID from pktblProductType where ptyProductType = @productType";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@productType", master.readCellString(sh.GetRow(i).GetCell(14)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    q.ProductType = System.Convert.ToInt32(dr.GetValue(0));
                            //}
                            //dr.Close();

                            //sql.CommandText = "Select prgProgramID from pktblProgram where prgProgramName = @programName";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@programName", master.readCellString(sh.GetRow(i).GetCell(7)));
                            //dr = sql.ExecuteReader();
                            //q.ProgramCode = 2062;
                            //if (dr.Read())
                            //{
                            //    q.ProgramCode = System.Convert.ToInt32(dr.GetValue(0));
                            //}
                            //dr.Close();

                            //sql.CommandText = "Select OEMID from OEM where OEMName = @oem";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@oem", master.readCellString(sh.GetRow(i).GetCell(61)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    q.OEM = System.Convert.ToInt32(dr.GetValue(0));
                            //}
                            //dr.Close();

                            //sql.CommandText = "Select ptyPartTypeID from pktblPartType where ptyPartTypeDescription = @partType";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@partType", master.readCellString(sh.GetRow(i).GetCell(18)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    q.PartType = System.Convert.ToInt32(dr.GetValue(0));
                            //}
                            //dr.Close();

                            //sql.CommandText = "Select tcyToolCountryID from pktblToolCountry where tcyToolCountry = @toolCountry";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@toolCountry", master.readCellString(sh.GetRow(i).GetCell(19)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    q.ToolCountry = System.Convert.ToInt32(dr.GetValue(0));
                            //}
                            //dr.Close();
                            if(q.TSGCompanyID == 3 || q.TSGCompanyID == 8)
                            {
                                q.ToolCountry = 3;
                            }
                            else
                            {
                                q.ToolCountry = 8;
                            }

                            string dieType = "";
                            //sql.CommandText = "Select DieTypeID from DieType where TSGCompanyID = 2 and Name = @name";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@name", master.readCellString(sh.GetRow(i).GetCell(16)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    dieType = dr.GetValue(0).ToString();
                            //}
                            //dr.Close();

                            if (q.TSGCompanyID == 3)
                            {
                                dieType = "94";
                            }
                            else if (q.TSGCompanyID == 5)
                            {
                                dieType = "27";
                            } 
                            else if (q.TSGCompanyID == 7)
                            {
                                dieType = "40";
                            }
                            else if (q.TSGCompanyID == 8)
                            {
                                dieType = "80";
                            }
                            else if (q.TSGCompanyID == 12)
                            {
                                dieType = "73";
                            }

                            string cavity = "18";
                            //sql.CommandText = "Select cavCavityID from pktblCavity where cavCavityName = @cavity";
                            //sql.Parameters.Clear();
                            //sql.Parameters.AddWithValue("@cavity", master.readCellString(sh.GetRow(i).GetCell(17)));
                            //dr = sql.ExecuteReader();
                            //if (dr.Read())
                            //{
                            //    cavity = dr.GetValue(0).ToString();
                            //}
                            //dr.Close();

                            sql.Parameters.Clear();

                            string currency = "USD";
                            //if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("EURO"))
                            //{
                            //    currency = "EUR";
                            //}
                            //else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("USD"))
                            //{
                            //    currency = "USD";
                            //}
                            //else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("CAD"))
                            //{
                            //    currency = "CAD";
                            //}
                            //else if (master.readCellString(sh.GetRow(i).GetCell(64)).Contains("GBP"))
                            //{
                            //    currency = "GBP";
                            //}

                            sql.CommandText = "Select curCurrencyID from pktblCurrency where curCurrency = @cur";
                            sql.Parameters.AddWithValue("@cur", currency);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                q.currency = dr.GetValue(0).ToString();
                            }
                            dr.Close();

                            string cu = master.readCellString(sh.GetRow(i).GetCell(2));
                            string lo = master.readCellString(sh.GetRow(i).GetCell(3));

                            sql.Parameters.Clear();
                            sql.CommandText = "Select TSGSalesmanID, CustomerLocationID, Customer.CustomerID from Customer, CustomerLocation ";
                            sql.CommandText += "where Customer.CustomerName = @customer and Customer.CustomerNumber = CustomerLocation.CustomerNumber and CustomerLocation.ShipToName = @location";
                            sql.Parameters.AddWithValue("@customer", master.readCellString(sh.GetRow(i).GetCell(2)));
                            sql.Parameters.AddWithValue("@location", master.readCellString(sh.GetRow(i).GetCell(3)));
                            dr = sql.ExecuteReader();
                            int salesmanID = 0;
                            int customerID = master.readCellInt(sh.GetRow(i).GetCell(1));
                            int customerLocation = 0;

                            if (dr.Read())
                            {
                                salesmanID = System.Convert.ToInt32(dr.GetValue(0));
                                customerID = System.Convert.ToInt32(dr.GetValue(2));
                                customerLocation = System.Convert.ToInt32(dr.GetValue(1));
                            }
                            dr.Close();


                            sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @matType";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i + 4).GetCell(0)));
                            dr = sql.ExecuteReader();

                            int matID = 0;
                            if (dr.Read())
                            {
                                matID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                            }
                            dr.Close();

                            if (matID == 0)
                            {
                                sql.CommandText = "insert into pktblMaterialType(mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                                sql.CommandText += "output inserted.mtyMaterialTypeID ";
                                sql.CommandText += "values(@matType, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@matType", master.readCellString(sh.GetRow(i + 4).GetCell(0)));
                                sql.Parameters.AddWithValue("@user", master.getUserName());
                                matID = System.Convert.ToInt32(master.ExecuteScalar(sql, "QuoteUpload"));
                            }
                            sql.Parameters.Clear();


                            string jobNumber = master.readCellString(sh.GetRow(i+4).GetCell(5));
                            //if (q.JobNumber == -1)
                            //{
                            //    errorFlag = 1;

                            //}

                            q.RFQID = 0;
                            //q.RFQID = System.Convert.ToInt32(rfq);

                            q.LeadTime = master.readCellInt(sh.GetRow(i+4).GetCell(1));
                            if (q.LeadTime == -1)
                            {
                                q.LeadTimeString = master.readCellString(sh.GetRow(i+4).GetCell(1));
                                //errorFlag = 1;
                                //context.Response.Write(FileName + " - Your lead time is not valid");
                            }



                            List<int> noteIDs = new List<int>();
                            int j = i +6;
                            int loop = i + 6;
                            q.TotalAmount = 0;
                            //Making sure to grab all the notes since they span over 47 lines for a single quote
                            loop += 100;

                            while (j < loop && errorFlag == 0)
                            {
                                try
                                {
                                    if ((sh.GetRow(j) != null))
                                    {
                                        int costColumn = 4;
                                        int costRow = j;
                                        if (j >= 26)
                                        {
                                            costColumn = 4;
                                            costRow--;
                                        }
                                        if (sh.GetRow(j).GetCell(0, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null || sh.GetRow(costRow).GetCell(costColumn, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL) != null)
                                        {
                                            q.Description = "";
                                            string costNote = "";
                                            q.TotalAmount += master.readCellDouble(sh.GetRow(j).GetCell(costColumn));
                                            costNote = master.readCellDouble(sh.GetRow(j).GetCell(costColumn)).ToString();
                                            q.Description = master.readCellString(sh.GetRow(j).GetCell(0));

                                            if (costNote == "-1" || costNote == "0")
                                            {
                                                if (costNote == "-1")
                                                {
                                                    q.TotalAmount++;
                                                }
                                                costNote = "";
                                            }
                                            if (q.Description != "" || costNote != "")
                                            {
                                                int id = System.Convert.ToInt32(master.getCompanyId());
                                                string lkjsdaf = master.getUserName();
                                                sql.CommandText = "Insert into pktblPreWordedNote (pwnCompanyID, pwnPreWordedNote, pwnCostNote, pwnCreated, pwnCreatedBy) ";
                                                sql.CommandText += "Output inserted.pwnPreWordedNoteID ";
                                                sql.CommandText += "Values (@TSGCompany, @note, @costNote, GETDATE(), @createdBy)";

                                                sql.Parameters.AddWithValue("@TSGCompany", System.Convert.ToInt32(master.getCompanyId()));
                                                sql.Parameters.AddWithValue("@note", q.Description);
                                                sql.Parameters.AddWithValue("@costNote", costNote);
                                                sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                                int noteID = 0;
                                                try
                                                {
                                                    noteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                                }
                                                catch
                                                {
                                                    errorFlag = 1;
                                                    context.Response.Write(FileName + "Something went wrong trying to upload your notes, please check your upload sheet for any errors");
                                                    break;
                                                }
                                                sql.Parameters.Clear();
                                                noteIDs.Add(noteID);
                                            }
                                        }
                                    }
                                }
                                catch
                                {

                                }

                                j++;
                            }

                            d.FtoBEnglish = master.readCellDouble(sh.GetRow(i+2).GetCell(3));
                            d.FtoBMetric = d.FtoBEnglish * 25.4;

                            d.LtoREnglish = master.readCellDouble(sh.GetRow(i+2).GetCell(4));
                            d.LtoRMetric = d.LtoREnglish * 25.4;

                            d.ShutHeightEnglish = master.readCellDouble(sh.GetRow(i+2).GetCell(5));
                            d.ShutHeightMetric = d.ShutHeightEnglish * 25.4;

                            d.NumberOfStations = master.readCellString(sh.GetRow(i+2).GetCell(6));
                            if (d.NumberOfStations == "")
                            {
                                d.NumberOfStations = master.readCellInt(sh.GetRow(i+2).GetCell(6)).ToString();
                            }


                            sql.Parameters.Clear();

                            int logo = 0;
                            if (master.readCellString(sh.GetRow(i).GetCell(6)) == "Yes")
                            {
                                logo = 1;
                            }

                            //Insert into quote table
                            sql.CommandText = "insert into tblECQuote(ecqPartNumber, ecqPartName, ecqRFQNumber, ecqCustomer, ecqCustomerLocation, ecqCustomerRFQNumber, ecqDieType, ecqCavity, ecqBlankWidthEng, ";
                            sql.CommandText += "ecqBlankWidthMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ";
                            sql.CommandText += "ecqShutHeightMet, ecqMaterialType, ecqNumberOfStations, ecqLeadTime, ecqShipping, ecqPayment, ecqCountryOfOrign, ecqCreated, ecqCreatedBy, ecqTSGCompanyID, ecqTotalCost, ecqStatus, ecqSalesmanID, ecqEstimator, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqVersion, ecqShippingLocation, ecqCustomerContactName) ";
                            sql.CommandText += "Output inserted.ecqECQuoteID ";
                            sql.CommandText += "values(@partNum, @partName, @rfqNum, @customer, @customerLocation, @customerRFQ, @dieType, @cavity, @blankWidthEng, @blankWidthMet, @blankPitchEng, @blankPitchMet, @matThkEng,";
                            sql.CommandText += "@matThkMet, @FBEng, @FBMet, @LREng, @LRMet, @shutHeightEng, @shutHeightMet, @matType, @stations, @leadTime, @shipping, @payment, @country, GETDATE(), @createdby, @companyID, @totalCost, @status, @salesman, @estimator, @jobNumber, @accessNumber, @useTSG, @version, @shippingLocation, @custContact )";

                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partNum", master.readCellString(sh.GetRow(i).GetCell(0)));
                            sql.Parameters.AddWithValue("@partName", master.readCellString(sh.GetRow(i).GetCell(1)));
                            sql.Parameters.AddWithValue("@rfqNum", "");
                            sql.Parameters.AddWithValue("@customer", customerID);
                            sql.Parameters.AddWithValue("@CustomerLocation", customerLocation);
                            if(master.readCellInt(sh.GetRow(i).GetCell(4)) != -1)
                            {
                                sql.Parameters.AddWithValue("@customerRFQ", master.readCellInt(sh.GetRow(i).GetCell(4)));
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@customerRFQ", master.readCellString(sh.GetRow(i).GetCell(4)));
                            }
                            sql.Parameters.AddWithValue("@dieType", dieType);
                            sql.Parameters.AddWithValue("@cavity", cavity);


                            sql.Parameters.AddWithValue("@matType", matID);
                            sql.Parameters.AddWithValue("@matThkEng", master.readCellDouble(sh.GetRow(i+2).GetCell(2)) / 25.4);
                            //in to mm is 25.4
                            sql.Parameters.AddWithValue("@matThkMet", master.readCellDouble(sh.GetRow(i+2).GetCell(2)));
                            sql.Parameters.AddWithValue("@blankPitchEng", master.readCellDouble(sh.GetRow(i+2).GetCell(1)));
                            sql.Parameters.AddWithValue("@blankPitchMet", master.readCellDouble(sh.GetRow(i+2).GetCell(1)) * 25.4);
                            sql.Parameters.AddWithValue("@blankWidthEng", master.readCellDouble(sh.GetRow(i+2).GetCell(0)));
                            sql.Parameters.AddWithValue("@blankWidthMet", master.readCellDouble(sh.GetRow(i+2).GetCell(0)) * 25.4);
                            sql.Parameters.AddWithValue("@weightEng", 0);
                            //lbs to kg is 0.453592
                            sql.Parameters.AddWithValue("@weightMet", 0);



                            sql.Parameters.AddWithValue("@FBEng", d.FtoBEnglish);
                            sql.Parameters.AddWithValue("@FBMet", d.FtoBMetric);
                            sql.Parameters.AddWithValue("@LREng", d.LtoREnglish);
                            sql.Parameters.AddWithValue("@LRMet", d.LtoRMetric);
                            sql.Parameters.AddWithValue("@shutHeightEng", d.ShutHeightEnglish);
                            sql.Parameters.AddWithValue("@shutHeightMet", d.ShutHeightMetric);
                            sql.Parameters.AddWithValue("@stations", d.NumberOfStations);

                            if (q.LeadTime != -1)
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTime);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@leadTime", q.LeadTimeString);
                            }
                            sql.Parameters.AddWithValue("@shipping", q.ShippingTerms);
                            sql.Parameters.AddWithValue("@payment", q.PaymentTerms);
                            sql.Parameters.AddWithValue("@country", q.ToolCountry);
                            sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                            sql.Parameters.AddWithValue("@companyID", master.getCompanyId());
                            sql.Parameters.AddWithValue("@totalCost", q.TotalAmount);
                            sql.Parameters.AddWithValue("@status", 2);
                            sql.Parameters.AddWithValue("@salesman", salesmanID);
                            sql.Parameters.AddWithValue("@estimator", q.EstimatorID);
                            sql.Parameters.AddWithValue("@jobNumber", jobNumber);
                            sql.Parameters.AddWithValue("@accessNumber", master.readCellString(sh.GetRow(i+4).GetCell(6)));
                            sql.Parameters.AddWithValue("@useTSG", logo);
                            sql.Parameters.AddWithValue("@version", "001");
                            sql.Parameters.AddWithValue("@shippingLocation", master.readCellString(sh.GetRow(i + 4).GetCell(4)));
                            sql.Parameters.AddWithValue("@custContact", master.readCellString(sh.GetRow(i + 4).GetCell(7)));




                            int quoteID = 0;
                            try
                            {
                                if (errorFlag == 0)
                                {
                                    quoteID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                                }
                            }
                            catch (Exception err)
                            {
                                errorFlag = 1;
                                context.Response.Write(FileName + " - Something went wrong trying to upload the quote information, please check your upload sheet for any errors " + err.ToString());

                                //Cleaning up after ourself and deleteing everything perviously inserted so we dont have lots of dead data floating around our databases
                                sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @noteID";
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }
                                //Breaking so we dont try and insert anything else
                            }

                            sql.Parameters.Clear();

                            String pictureName = "EC-" + quoteID + ".png";

                            sql.CommandText = "Update tblECQuote set ecqPicture = @picture, ecqQuoteNumber = @quoteNumber where ecqECQuoteID = @ecQuoteID";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@ecQuoteID", quoteID);
                            sql.Parameters.AddWithValue("@picture", pictureName);
                            sql.Parameters.AddWithValue("@quoteNumber", quoteID);
                            master.ExecuteNonQuery(sql, "Edit Quote");


                            //Inserting link for all notes previously uploaded
                            if (errorFlag == 0)
                            {
                                for (int k = 0; k < noteIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();

                                    sql.CommandText = "Insert into linkPWNToECQuote (peqECQuoteID, peqPreWordedNoteID, peqCreated, peqCreatedBy) ";
                                    sql.CommandText += "output inserted.peqPWNToECQuoteID ";
                                    sql.CommandText += "Values (@quoteID, @noteID, GETDATE(), @createdBy)";

                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    sql.Parameters.AddWithValue("@noteID", noteIDs[k]);
                                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                                    master.ExecuteNonQuery(sql, "EditRFQ");
                                }

                                sql.Parameters.Clear();


                                List<string> genIDs = new List<string>();
                                genIDs.Add("3");
                                genIDs.Add("4");
                                genIDs.Add("5");
                                genIDs.Add("6");
                                genIDs.Add("7");
                                genIDs.Add("8");
                                for (int k = 0; k < genIDs.Count; k++)
                                {
                                    sql.Parameters.Clear();
                                    sql.CommandText = "Insert into linkGeneralNoteToECQuote (gneGeneralNoteID, gneECQuoteID, gneCreated, gneCreatedBy) ";
                                    sql.CommandText += "values (@noteID, @quoteID, GETDATE(), @created)";
                                    sql.Parameters.AddWithValue("@noteID", genIDs[k]);
                                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                                    sql.Parameters.AddWithValue("@created", master.getUserName());
                                    master.ExecuteNonQuery(sql, "Quote Upload");
                                }
                                row++;
                            }
                            finalQuoteID = quoteID.ToString();
                            connection.Close();
                        }

                    }
                    catch
                    {

                    }
                }
            }
            if (errorFlag == 0)
            {

                context.Response.Write("OK|" + partID + "|" + finalQuoteID);
            }
            //context.Response.Write("Quote has been uploaded");
            return;
        }
    }
}