using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using Microsoft.SharePoint.Client;
using System.Security;
using System.IO;
using NPOI.SS.UserModel;

namespace RFQ
{
    /// <summary>
    /// Creates an Excel File with all parts for this RFQ that have been reserved by the company
    /// </summary>
    public class QuoteSheet : IHttpHandler
    {
        Int32 maxRow = -1;
        public void ProcessRequest(HttpContext context)
        {
            Int64 RFQID = 0;
            Int64 Company = 1;
            Boolean onlyNewParts = false;
            try
            {
                RFQID = System.Convert.ToInt64(context.Request["rfq"]);
            }
            catch
            {
                return;
            }

            try
            {
                onlyNewParts = System.Convert.ToBoolean(context.Request["newParts"]);
            } catch { }

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            SqlCommand sql2 = new SqlCommand();
            connection2.Open();
            sql2.Connection = connection2;

            Company = master.getCompanyId();

            //Company = 5;

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Quote Sheet");
            XSSFSheet referenceSheet = (XSSFSheet)wb.CreateSheet("REFERENCES");
            // Build All Drop Down Lists

            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);

            // simplest kind - direct list 
            // all of the others go to separate worksheet, since Excel limits the total text in the list to 255 characters
            List<string> LogoOptionList = new List<string>();
            LogoOptionList.Add("TSG");
            LogoOptionList.Add("Company Logo");
            XSSFDataValidationConstraint constraintLogo = new XSSFDataValidationConstraint(LogoOptionList.ToArray());

            List<string> nameOptionList = new List<string>();
            nameOptionList.Add("Company Name");
            nameOptionList.Add("TSG");
            XSSFDataValidationConstraint constraintName = new XSSFDataValidationConstraint(nameOptionList.ToArray());

            List<string> quoteStatusOptionList = new List<string>();
            sql.CommandText = "Select qstQuoteStatusDescription from pktblQuoteStatus order by qstQuoteStatus";
            SqlDataReader sdr = sql.ExecuteReader();
            while(sdr.Read())
            {
                quoteStatusOptionList.Add(sdr.GetValue(0).ToString());
            }
            sdr.Close();
            XSSFDataValidationConstraint constraintQuoteStatus = new XSSFDataValidationConstraint(quoteStatusOptionList.ToArray());

            Int32 currentRow = 0;
            Int32 currentColumn = 0;
            sql.CommandText = " select cavCavityName from pktblCavity order by cavCavityName";
            sql.Parameters.Clear();
            SqlDataReader ldr = sql.ExecuteReader();
            NPOI.SS.UserModel.IRow rrow;
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            // 0x03 is the LIST type of constraint. Second value is where to pull from in Excel
            XSSFDataValidationConstraint constraintCavity = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$A$1:$A$" + currentRow);

            currentColumn++;
            currentRow = 0;
            sql.Parameters.Clear();
            if (Company == 1)
            {
                sql.CommandText = " select Distinct dtyFullName from DieType order by dtyFullName";
            }
            else
            {
                sql.CommandText = " select dtyFullName from DieType where TSGCompanyID=@company order by dtyFullName";
                sql.Parameters.AddWithValue("@company", Company);
            }
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintProcess = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$B$1:$B$" + currentRow);

            List<String> PartTypeList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = " select ptyPartTypeDescription from pktblPartType  order by ptyPartTypeDescription";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintPartType = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$C$1:$C$" + currentRow);

            // logos are in shared documents logos
            List<String> MaterialTypeList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = " select mtyMaterialType from pktblMaterialType  order by mtyMaterialType";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            rrow = GetOrCreateRow(referenceSheet, currentRow);
            rrow.CreateCell(currentColumn).SetCellValue("NEW MATERIAL");
            XSSFDataValidationConstraint constraintMaterialType = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$D$1:$D$" + currentRow);

            List<String> ToolCountryList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = " select tcyToolCountry from pktblToolCountry  order by tcyToolCountry";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintToolCountry = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$E1:$E$" + currentRow);

            List<String> PaymentTermsList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = " select ptePaymentTerms from pktblPaymentTerms  order by ptePaymentTerms";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintPaymentTerms = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$F$1:$F$" + currentRow);

            List<String> currencyList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = "Select curCurrency from pktblCurrency";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintCurrency = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$G$1:$G$" + currentRow);

            List<String> measurementList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = "Select meaMeasurement from pktblMeasurement";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintMeasurement = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$H$1:$H$" + currentRow);


            List<String> EstimatorsList = new List<string>();
            sql.Parameters.Clear();
            if (Company != 1)
            {
                sql.CommandText = " select estEmail from pktblEstimators where estCompanyID=@company order by estEmail";
                sql.Parameters.AddWithValue("@company", Company);
            }
            else
            {
                sql.CommandText = " select estEmail from pktblEstimators  order by estEmail";
            }
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();

            string LoggedInEst = "";

            sql.CommandText = " select estEmail from pktblEstimators  where estEmail = @email";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@email", master.getUserName());
            ldr = sql.ExecuteReader();
            if (ldr.Read())
            {
                LoggedInEst = ldr["estEmail"].ToString();

            }
            ldr.Close();

            XSSFDataValidationConstraint constraintEstimators = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$I$1:$I$" + currentRow);

            List<String> ShippingTermsList = new List<string>();
            sql.Parameters.Clear();
            sql.CommandText = " select steShippingTerms from pktblShippingTerms  order by steShippingTerms";
            currentColumn++;
            currentRow = 0;
            ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                rrow = GetOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(ldr.GetValue(0).ToString());
                currentRow++;
            }
            ldr.Close();
            XSSFDataValidationConstraint constraintShippingTerms = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$J$1:$J$" + currentRow);

            List<string> defaultNotes = new List<string>();
            List<string> noteOrder = new List<string>();
            sql.CommandText = "Select dqnDefaultQuoteNote, dqnOrder from pktblDefaultQuoteNotes where dqnCompanyID = @company and (dqnQuoteType = 0 or dqnQuoteType = 2) order by dqnOrder ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@company", Company);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                defaultNotes.Add(dr.GetValue(0).ToString());
                noteOrder.Add(dr.GetValue(1).ToString());
            }
            dr.Close();

            sql.CommandText = "select distinct CustomerName, ShipCode, ProgramName, rfqID, tcyToolCountry, ptePaymentTerms, steShippingTerms, rfqCustomerRFQNumber, quoTotalAmount,  ";
            sql.CommandText += "quoCustomerQuoteNumber, quoQuoteID, prtRFQLineNumber, quoProgramCodeID, estFirstName, estLastName, estEmail, TSGCompanyAbbrev, prtPartNumber,  ";
            sql.CommandText += "prtPartDescription, dtyFullName as Process, prtPicture,prtPartMaterialType, dinDieType, cavCavityName, dinSizeFrontToBackEnglish,dinSizeFrontToBackMetric,  ";
            sql.CommandText += "dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish,dinSizeShutHeightMetric, dinNumberOfStations,  quoLeadTime, binMaterialWidthEnglish,  ";
            sql.CommandText += "binMaterialWidthMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialThicknessMetric, binMaterialThicknessEnglish, binMaterialWeightEnglish,  ";
            sql.CommandText += "binMaterialWeightMetric, m1.mtyMaterialType as matType, prtPartRevLevEAU, prtPartWeight, ptyPartTypeDescription, quoUseTSGLogo, curCurrency, quoAccess, ";
            sql.CommandText += "prtPARTID, m2.mtyMaterialType as materialType, prtPartThickness, tblquote.quoQuoteID ";
            sql.CommandText += "from tblRFQ, Customer, TSGCompany, CustomerLocation, Program, linkPartToRFQ, linkPartReservedToCompany,  tblPart ";
            sql.CommandText += "left outer join linkPartToQuote on prtPartiD = ptqPartID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
            sql.CommandText += "left outer join pktblPartType on prtPartTypeID = ptyPartTypeID ";
            sql.CommandText += "left outer join tblQuote on ptqQuoteID = quoQuoteID and quoTSGCompanyID = @company ";
            sql.CommandText += "left outer join pktblEstimators on quoEstimatorID = estEstimatorID ";
            sql.CommandText += "left outer join pktblPaymentTerms on quoPaymentTermsID = ptePaymentTermsID ";
            sql.CommandText += "left outer join pktblShippingTerms on quoShippingTermsID = steShippingTermsID ";
            sql.CommandText += "left outer join pktblToolCountry on quoToolCountryID = tcyToolCountryID ";
            sql.CommandText += "left outer join pktblBlankInfo on quoBlankInfoID = binBlankInfoID ";
            sql.CommandText += "left outer join pktblMaterialType as m1 on binBlankMaterialTypeID = m1.mtyMaterialTypeID ";
            sql.CommandText += "left outer join pktblMaterialType as m2 on prtPartMaterialType = m2.mtyMaterialTypeID ";
            sql.CommandText += "left outer join linkDieInfoToQuote on quoQuoteID = diqQuoteID ";
            sql.CommandText += "left outer join tblDieInfo on diqDieInfoID = dinDieInfoID ";
            sql.CommandText += "left outer join DieType on DieTypeID = dinDieType ";
            sql.CommandText += "left outer join pktblCavity on dinCavityID = cavCavityID ";
            sql.CommandText += "left outer join pktblCurrency on quoCurrencyID = curCurrencyID ";
            //sql.CommandText += " left outer join pktblMeasurement on quoMeasurementID=meaMeasurementID ";
            sql.CommandText += "where rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocationID and rfqProgramID = ProgramID ";
            sql.CommandText += "and ptrRFQID = rfqid and ptrPartID = prtPartID and prtPartID = prcPartID  and prcTSGCompanyID = TSGCompany.TSGCompanyID ";
            sql.CommandText += "and RFQID = @rfq ";
            //and tblquote.quoQuoteID is not NULL
            sql.CommandText += "and prcTSGCompanyID = @company order by prtRFQLineNumber, tblquote.quoQuoteID";
            sql.Parameters.Clear();

            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@company", Company);
            dr = sql.ExecuteReader();
            Boolean HeaderWritten = false;

            // This Patriarch is what is used to position pictures
            XSSFDrawing DrawingPatriarch = (XSSFDrawing)sh.CreateDrawingPatriarch();
            XSSFCellStyle CurrencyStyle;
            // This will be used on the numbers
            CurrencyStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CurrencyStyle.DataFormat = CustomFormat.GetFormat("###,###,##0.00");

            XSSFCellStyle ws;
            ws = (XSSFCellStyle)wb.CreateCellStyle();
            ws.WrapText = true;

            XSSFFont headerFont = (XSSFFont)wb.CreateFont();
            headerFont.FontHeight = 14;
            // 700 is BOLD
            // 400 is NORMAL
            headerFont.Boldweight = 700;
            headerFont.IsItalic = true;
            XSSFFont titleFont = (XSSFFont)wb.CreateFont();
            titleFont.FontHeight = 10;
            titleFont.Boldweight = 700;
            XSSFCellStyle CenterStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CenterStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            XSSFCellStyle RequiredStyle = (XSSFCellStyle)wb.CreateCellStyle();
            RequiredStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
            RequiredStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.LightYellow.Index;
            // in here for documentation purposes
            //titleFont.Underline = NPOI.SS.UserModel.FontUnderlineType.Single;
            XSSFFont blueFont = (XSSFFont)wb.CreateFont();
            blueFont.FontHeight = 14;
            blueFont.Boldweight = 700;
            blueFont.IsItalic = true;
            XSSFColor ColorBlue = new XSSFColor();
            byte[] Blue = { 0, 0, 128 };
            ColorBlue.SetRgb(Blue);
            blueFont.SetColor(ColorBlue);
            currentRow = 0;
            // This points to where the pictures are
            String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
            String sharepointLibrary = "Shared Documents/Logos";
            byte[] pictureData;
            // used to get general notes
            String FirstQuoteID = "0";
            while (dr.Read())
            {
                if (onlyNewParts && dr["quoQuoteID"].ToString() != "")
                {
                    continue;
                }
                if (!HeaderWritten)
                {
                    var row = sh.CreateRow(0);
                    // get picture from sharepoint and insert
                    // company and TSG
                    try
                    {
                        using (var clientContext = new ClientContext(siteUrl))
                        {
                            clientContext.Credentials = master.getSharePointCredentials();
                            var url = new Uri(siteUrl);
                            var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "TSG.png");
                            // open the file as binary
                            try
                            {
                                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                                using (var memstr = new System.IO.MemoryStream())
                                {
                                    var buf = new byte[1024 * 16];
                                    int byteSize;
                                    while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                    {
                                        memstr.Write(buf, 0, byteSize);
                                    }
                                    pictureData = memstr.ToArray();
                                }
                                XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, 0, 0, 0);
                                anchor.AnchorType = 2;
                                int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                                XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                                // The picture will not appear unless you run resize
                                Picture.Resize(.058);
                            }
                            catch
                            {

                            }
                            if (dr["TSGCompanyAbbrev"].ToString() != "TSG")
                            {
                                clientContext.Credentials = master.getSharePointCredentials();
                                url = new Uri(siteUrl);
                                relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["TSGCompanyAbbrev"].ToString() + ".png");
                                // open the file as binary
                                try
                                {
                                    using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                    // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                                    using (var memstr = new System.IO.MemoryStream())
                                    {
                                        var buf = new byte[1024 * 16];
                                        int byteSize;
                                        while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                        {
                                            memstr.Write(buf, 0, byteSize);
                                        }
                                        pictureData = memstr.ToArray();
                                    }
                                    XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 1, 0, 1, 0);
                                    anchor.AnchorType = 2;
                                    int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                                    XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                                    Double Factor = 0.03;
                                    //if (dr["TSGCompanyAbbrev"].ToString() == "RTS")
                                    //{
                                    //    Factor = 0.05;
                                    //}
                                    //if (dr["TSGCompanyAbbrev"].ToString() == "EIG")
                                    //{
                                    //    Factor = 0.15;
                                    //}
                                    //if (dr["TSGCompanyAbbrev"].ToString() == "DTS")
                                    //{
                                    //    Factor = 0.04;
                                    //}
                                    //if (dr["TSGCompanyAbbrev"].ToString() == "CTS")
                                    //{
                                    //    Factor = 0.15;
                                    //}
                                    //if (dr["TSGCompanyAbbrev"].ToString() == "BTS")
                                    //{
                                    //    Factor = .04;
                                    //}
                                    Picture.Resize(Factor);
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
                    row.CreateCell(2).SetCellValue("Logo for Quote");
                    row.CreateCell(4).SetCellValue("Use TSG Name or Company Name");

                    NPOI.SS.Util.CellRangeAddressList logoloc = new NPOI.SS.Util.CellRangeAddressList(0, 0, 3, 3);
                    XSSFDataValidation logodv = (XSSFDataValidation)dvHelper.CreateValidation(constraintLogo, logoloc);
                    logodv.ShowErrorBox = true;
                    logodv.EmptyCellAllowed = false;
                    sh.AddValidationData(logodv);
                    if (dr["quoUseTSGLogo"].ToString() == "True")
                    {
                        row.CreateCell(3).SetCellValue("TSG");
                    }
                    else
                    {
                        row.CreateCell(3).SetCellValue("Company Logo");
                    }
                    NPOI.SS.Util.CellRangeAddressList nameloc = new NPOI.SS.Util.CellRangeAddressList(0, 0, 5, 5);
                    XSSFDataValidation namedv = (XSSFDataValidation)dvHelper.CreateValidation(constraintName, nameloc);
                    namedv.ShowErrorBox = true;
                    namedv.EmptyCellAllowed = false;
                    sh.AddValidationData(namedv);
                    row.CreateCell(5).SetCellValue("Company Name");

                    if(Company == 8)
                    {
                        row.CreateCell(6).SetCellValue("Quote Status");

                        NPOI.SS.Util.CellRangeAddressList statusloc = new NPOI.SS.Util.CellRangeAddressList(0, 0, 7, 7);
                        XSSFDataValidation statusdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintQuoteStatus, statusloc);
                        statusdv.ShowErrorBox = true;
                        statusdv.EmptyCellAllowed = false;
                        sh.AddValidationData(statusdv);

                        row.CreateCell(7).SetCellValue("Out To Bid");

                        //constraintQuoteStatus
                    }


                    row.Height = 1500;
                    row = sh.CreateRow(2);
                    row.CreateCell(0).SetCellValue(dr["rfqCustomerRFQNumber"].ToString() + " Engineering Estimate");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.GetCell(0).RichStringCellValue.ApplyFont(0, dr["rfqCustomerRFQNumber"].ToString().Length, blueFont);
                    row.CreateCell(2).SetCellValue("Customer");
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue("Plant");
                    row.CreateCell(5).SetCellValue(dr["ShipCode"].ToString());
                    row.CreateCell(6).SetCellValue("Program");
                    row.CreateCell(7).SetCellValue(dr["ProgramName"].ToString());

                    currentRow = 4;

                    HeaderWritten = true;
                }
                header(sh, currentRow, RequiredStyle, headerFont, ws, Company);


                currentRow++;
                var newRow = sh.CreateRow(currentRow);
                // This is in points which is whatever excel reports times 20 
                newRow.Height = 1000;
                newRow.CreateCell(0);
                newRow.CreateCell(1).SetCellValue(dr["quoCustomerQuoteNumber"].ToString());
                newRow.CreateCell(0);
                // get picture from sharepoint and insert
                // This points to where the pictures are
                sharepointLibrary = "Part Pictures";
                using (var clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = master.getSharePointCredentials();
                    var url = new Uri(siteUrl);
                    var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["prtPicture"].ToString());
                    // open the file as binary
                    try
                    {
                        using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                        // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                        using (var memstr = new System.IO.MemoryStream())
                        {
                            var buf = new byte[1024 * 16];
                            int byteSize;
                            while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                            {
                                memstr.Write(buf, 0, byteSize);
                            }
                            pictureData = memstr.ToArray();
                        }
                        XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, currentRow, 0, currentRow);
                        anchor.AnchorType = 2;
                        int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                        XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                        // The picture will not appear unless you run resize
                        // in this case, scaling to this value seems to work best
                        Picture.Resize(.22);
                    }
                    catch
                    {

                    }
                }
                newRow.CreateCell(2).SetCellValue(dr["prtRFQLineNumber"].ToString());
                XSSFDataValidationConstraint c2Constraint = new XSSFDataValidationConstraint(new String[] { dr["prtRFQLineNumber"].ToString() });
                NPOI.SS.Util.CellRangeAddressList c2Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                XSSFDataValidation c2dv = (XSSFDataValidation)dvHelper.CreateValidation(c2Constraint, c2Loc);
                c2dv.EmptyCellAllowed = false;
                c2dv.SuppressDropDownArrow = true;
                c2dv.ShowErrorBox = true;
                sh.AddValidationData(c2dv);
                //XSSFDataValidationConstraint c3Constraint = new XSSFDataValidationConstraint(new String[] { dr["prtPartNumber"].ToString() });
                //NPOI.SS.Util.CellRangeAddressList c3Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 3, 3);
                //XSSFDataValidation c3dv = (XSSFDataValidation)dvHelper.CreateValidation(c3Constraint, c3Loc);
                //c3dv.EmptyCellAllowed = false;
                //c3dv.SuppressDropDownArrow = true;
                //c3dv.ShowErrorBox = true;
                //sh.AddValidationData(c3dv);
                string partNum = dr["prtPartNumber"].ToString();
                string partID = dr["prtPARTID"].ToString();
                sql2.CommandText = "Select prtPartNumber from tblPart, linkPartToPartDetail where ppdPartID = prtPARTID and ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and prtPARTID <> @partID";
                sql2.Parameters.Clear();
                sql2.Parameters.AddWithValue("@partID", partID);
                SqlDataReader dr3 = sql2.ExecuteReader();
                while(dr3.Read())
                {
                    partNum += " - " + dr3.GetValue(0).ToString();
                }
                dr3.Close();
                newRow.CreateCell(3).SetCellValue(partNum);
                newRow.GetCell(3).CellStyle = ws;
                //newRow.CreateCell(4).SetCellValue(dr["prtPartRevLevEAU"].ToString());
                newRow.CreateCell(4).SetCellValue(dr["prtPartDescription"].ToString());
                newRow.GetCell(4).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList cavityloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 5, 5);
                XSSFDataValidation cavitydv = (XSSFDataValidation)dvHelper.CreateValidation(constraintCavity, cavityloc);
                cavitydv.ShowErrorBox = true;
                cavitydv.EmptyCellAllowed = true;
                sh.AddValidationData(cavitydv);
                if (dr["cavCavityName"].ToString() != "")
                {
                    newRow.CreateCell(5).SetCellValue(dr["cavCavityName"].ToString());
                }
                else if (Company ==  12)
                {
                    newRow.CreateCell(5).SetCellValue("2 OUT R/L");
                }
                else
                {
                    newRow.CreateCell(5).SetCellValue(dr["cavCavityName"].ToString());
                }
                newRow.GetCell(5).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList processloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                XSSFDataValidation processdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintProcess, processloc);
                processdv.ShowErrorBox = true;
                processdv.EmptyCellAllowed = true;
                sh.AddValidationData(processdv);
                if (dr["Process"].ToString() != "")
                {
                    newRow.CreateCell(6).SetCellValue(dr["Process"].ToString());
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(6).SetCellValue("Progressive");
                }
                else
                {
                    newRow.CreateCell(6).SetCellValue(dr["Process"].ToString());
                }
                newRow.GetCell(6).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList typeloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 7, 7);
                XSSFDataValidation typedv = (XSSFDataValidation)dvHelper.CreateValidation(constraintPartType, typeloc);
                typedv.ShowErrorBox = true;
                typedv.EmptyCellAllowed = true;
                sh.AddValidationData(typedv);
                if(dr["ptyPartTypeDescription"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["ptyPartTypeDescription"].ToString());
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(7).SetCellValue("BRACKET");
                }
                else
                {
                    newRow.CreateCell(7).SetCellValue(dr["ptyPartTypeDescription"].ToString());
                }
                newRow.GetCell(7).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList mtypeloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 8, 8);
                XSSFDataValidation mtypedv = (XSSFDataValidation)dvHelper.CreateValidation(constraintMaterialType, mtypeloc);
                mtypedv.ShowErrorBox = true;
                mtypedv.EmptyCellAllowed = true;
                // for now, do not use this drop down
                // sh.AddValidationData(mtypedv);
                if(dr["matType"].ToString() == "")
                {
                    newRow.CreateCell(8).SetCellValue(dr["materialType"].ToString());
                }
                else
                {
                    newRow.CreateCell(8).SetCellValue(dr["matType"].ToString());
                }
                newRow.GetCell(8).CellStyle = ws;
                if (Company == 7)
                {
                    if (dr["binMaterialThicknessMetric"].ToString() == "")
                    {
                        newRow.CreateCell(9).SetCellValue(GetDoubleValue(dr["prtPartThickness"].ToString()).ToString("0.00"));
                    }
                    else
                    {
                        newRow.CreateCell(9).SetCellValue(GetDoubleValue(dr["binMaterialThicknessMetric"].ToString()).ToString("0.00"));
                    }
                }
                else
                {
                    newRow.CreateCell(9).SetCellValue(GetDoubleValue(dr["binMaterialThicknessMetric"].ToString()).ToString("0.00"));
                }

                if (Company == 8 || Company == 3)
                {
                    newRow.CreateCell(10).SetCellValue(GetDoubleValue(dr["binMaterialWidthMetric"].ToString()).ToString("0.##"));
                    newRow.CreateCell(11).SetCellValue(GetDoubleValue(dr["binMaterialPitchMetric"].ToString()).ToString("0.##"));

                }
                else
                {
                    newRow.CreateCell(10).SetCellValue(GetDoubleValue(dr["binMaterialWidthEnglish"].ToString()).ToString("0.##"));
                    newRow.CreateCell(11).SetCellValue(GetDoubleValue(dr["binMaterialPitchEnglish"].ToString()).ToString("0.##"));
                }
                //newRow.CreateCell(13).SetCellValue(GetDoubleValue(dr["binMaterialWeightEnglish"].ToString()).ToString("0.##"));
                currentRow = currentRow + 2;
                newRow = sh.CreateRow(currentRow);
                newRow.CreateCell(0).SetCellValue(GetDoubleValue(dr["dinNumberOfStations"].ToString()).ToString("0.##"));
                if (Company == 8 || Company == 3)
                {
                    newRow.CreateCell(1).SetCellValue(GetDoubleValue(dr["dinSizeFrontToBackMetric"].ToString()).ToString("0.##"));
                    newRow.CreateCell(2).SetCellValue(GetDoubleValue(dr["dinSizeLeftToRightMetric"].ToString()).ToString("0.##"));
                }
                else
                {
                    newRow.CreateCell(1).SetCellValue(GetDoubleValue(dr["dinSizeFrontToBackEnglish"].ToString()).ToString("0.##"));
                    newRow.CreateCell(2).SetCellValue(GetDoubleValue(dr["dinSizeLeftToRightEnglish"].ToString()).ToString("0.##"));
                }
                if (dr["dinSizeShutHeightEnglish"].ToString() != "")
                {
                    newRow.CreateCell(3).SetCellValue(GetDoubleValue(dr["dinSizeShutHeightEnglish"].ToString()).ToString("0.##"));
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(3).SetCellValue("18");
                }
                else
                {
                    newRow.CreateCell(3).SetCellValue(GetDoubleValue(dr["dinSizeShutHeightEnglish"].ToString()).ToString("0.##"));
                }

                Double TotalAmount = GetDoubleValue(dr["quoTotalAmount"].ToString());
                Double FtoB = GetDoubleValue(dr["dinSizeFrontToBackEnglish"].ToString());
                Double LtoR = GetDoubleValue(dr["dinSizeLeftToRightEnglish"].ToString());
                // formula if we have values, otherwise set to total amount
                XSSFCell TotalCell = (XSSFCell)newRow.CreateCell(19);
                TotalCell.CellStyle = CurrencyStyle;
                //if (FtoB > 0)
                //{
                //    if (LtoR > 0)
                //    {
                //        newRow.CreateCell(18).SetCellValue(Math.Round((TotalAmount / FtoB / LtoR), 6));
                //    }
                //    else
                //    {
                //        newRow.CreateCell(18).SetCellValue(0);
                //    }
                //}
                //else
                //{
                //    newRow.CreateCell(18).SetCellValue(0);
                //}
                NPOI.SS.Util.CellRangeAddressList toolcountryloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 4, 4);
                XSSFDataValidation toolcountrydv = (XSSFDataValidation)dvHelper.CreateValidation(constraintToolCountry, toolcountryloc);
                toolcountrydv.ShowErrorBox = true;
                toolcountrydv.EmptyCellAllowed = true;
                sh.AddValidationData(toolcountrydv);
                if (dr["tcyToolCountry"].ToString() != "")
                {
                    newRow.CreateCell(4).SetCellValue(dr["tcyToolCountry"].ToString());
                }
                else if (Company == 7)
                {
                    newRow.CreateCell(4).SetCellValue("NA TOOL");
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(4).SetCellValue("TBD");
                }
                else if (Company == 3 || Company == 8)
                {
                    newRow.CreateCell(4).SetCellValue("LCC TOOL");
                }else
                {
                    newRow.CreateCell(4);
                }

                newRow.GetCell(4).CellStyle = ws;

                if (dr["quoLeadTime"].ToString() != "")
                {
                    newRow.CreateCell(8).SetCellValue(dr["quoLeadTime"].ToString());
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(8).SetCellValue("22");
                }
                else if (Company == 3)
                {
                    newRow.CreateCell(8).SetCellValue("34");
                }
                else if (Company == 8)
                {
                    newRow.CreateCell(8).SetCellValue("28");
                }
                else
                {
                    newRow.CreateCell(8);
                }


                NPOI.SS.Util.CellRangeAddressList estloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 5, 5);
                XSSFDataValidation estdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintEstimators, estloc);
                estdv.ShowErrorBox = true;
                estdv.EmptyCellAllowed = true;
                sh.AddValidationData(estdv);
                if (dr["estEmail"].ToString() != "")
                {
                    newRow.CreateCell(5).SetCellValue(dr["estEmail"].ToString());
                }
                else if (LoggedInEst != "")
                {
                    newRow.CreateCell(5).SetCellValue(LoggedInEst);
                }
                else if (Company == 7)
                {
                    newRow.CreateCell(5).SetCellValue("kcoleman@toolingsystemsgroup.com");
                }
                else if (Company == 12)
                {
                    newRow.CreateCell(5).SetCellValue("mheuker@toolingsystemsgroup.com");
                }
                else if (Company == 3)
                {
                    newRow.CreateCell(5).SetCellValue("pqiu@toolingsystemsgroup.com");
                }
                else if (Company == 8)
                {
                    newRow.CreateCell(5).SetCellValue("jsapp@toolingsystemsgroup.com");
                }
                else
                {
                    newRow.CreateCell(5);
                }
                newRow.GetCell(5).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList stloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                XSSFDataValidation stdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintShippingTerms, stloc);
                stdv.ShowErrorBox = true;
                stdv.EmptyCellAllowed = true;
                sh.AddValidationData(stdv);
                if (dr["steShippingTerms"].ToString() != "")
                {
                    newRow.CreateCell(6).SetCellValue(dr["steShippingTerms"].ToString());
                }
                else if (Company == 7 || Company == 12)
                {
                    newRow.CreateCell(6).SetCellValue("Free On Board (FOB):");
                }
                else if (Company == 3 || Company == 8)
                {
                    newRow.CreateCell(6).SetCellValue("Delivered Duty Paid (DDP):");
                }
                else
                {
                    newRow.CreateCell(6);
                }
                newRow.GetCell(6).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList ptloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 7, 7);
                XSSFDataValidation ptdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintPaymentTerms, ptloc);
                ptdv.ShowErrorBox = true;
                ptdv.EmptyCellAllowed = true;
                sh.AddValidationData(ptdv);

                

                if (dr["ptePaymentTerms"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["ptePaymentTerms"].ToString());
                }
                else if (Company == 7 || Company == 12 || Company == 8)
                {
                    newRow.CreateCell(7).SetCellValue("30% @ Design / 30% @ 50% Build / 30% @ Buy Off / 10% @ Buy Off Customer  Net 30 days");
                }
                else if (Company == 3)
                {
                    newRow.CreateCell(7).SetCellValue("See notes");
                }
                else
                {
                    newRow.CreateCell(7);
                }
                newRow.GetCell(7).CellStyle = ws;

                NPOI.SS.Util.CellRangeAddressList meaLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 9, 9);
                XSSFDataValidation medv = (XSSFDataValidation)dvHelper.CreateValidation(constraintMeasurement, meaLoc);
                medv.ShowErrorBox = true;
                medv.EmptyCellAllowed = true;
                sh.AddValidationData(medv);
                if (Company == 3 || Company == 8)
                {
                    newRow.CreateCell(9).SetCellValue("All Metric");
                }
                else
                {
                    newRow.CreateCell(9).SetCellValue("Standard (English and thickness Metric)");
                }

                newRow.GetCell(9).CellStyle = ws;
                newRow.CreateCell(11).SetCellValue(" ");

                currentRow += 2;
                newRow = sh.CreateRow(currentRow);
                newRow.CreateCell(11).SetCellValue(" ");

                NPOI.SS.Util.CellRangeAddressList curLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 9, 9);
                XSSFDataValidation cudv = (XSSFDataValidation)dvHelper.CreateValidation(constraintCurrency, curLoc);
                cudv.ShowErrorBox = true;
                cudv.EmptyCellAllowed = true;
                sh.AddValidationData(cudv);
                if (dr["curCurrency"].ToString() != "")
                {
                    newRow.CreateCell(9).SetCellValue(dr["curCurrency"].ToString());
                }
                else
                {
                    newRow.CreateCell(9).SetCellValue("USD");
                }
                newRow.GetCell(9).CellStyle = ws;
                newRow.CreateCell(6).SetCellValue(" ");

                if (Company == 12)
                {
                    newRow.CreateCell(1).SetCellValue(0);
                    newRow.CreateCell(3).SetCellValue(0);
                    newRow.CreateCell(5).SetCellValue(2600);
                    newRow.CreateCell(6).SetCellValue("Coating Budget: ");
                    newRow.CreateCell(7).SetCellValue(350);
                    newRow.CreateCell(4).SetCellValue(4500);
                }

                newRow.CreateCell(11).SetCellValue(dr["quoAccess"].ToString());
                newRow.GetCell(11).CellStyle = ws;

                XSSFCell TotalCostCell = (XSSFCell)newRow.CreateCell(8);
                TotalCostCell.CellStyle = CurrencyStyle;
                if (Company == 12)
                {
                    String tcFormula = "SUM(";
                    tcFormula += "J" + (currentRow + 3).ToString();
                    tcFormula += ":";
                    tcFormula += "J" + (currentRow + 28).ToString();
                    tcFormula += ")";
                    TotalCostCell.SetCellFormula(tcFormula);
                }
                else
                {
                    String tcFormula = "SUM(";
                    tcFormula += "A" + (currentRow + 1).ToString();
                    tcFormula += ":";
                    tcFormula += "H" + (currentRow + 1).ToString();
                    tcFormula += ")";
                    TotalCostCell.SetCellFormula(tcFormula);
                }


                String QuoteID = dr["quoQuoteID"].ToString().Trim();
                XSSFDataValidationConstraint c39Constraint = new XSSFDataValidationConstraint(new String[] { QuoteID });
                NPOI.SS.Util.CellRangeAddressList c39Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 10, 10);
                XSSFDataValidation c39dv = (XSSFDataValidation)dvHelper.CreateValidation(c39Constraint, c39Loc);
                c39dv.EmptyCellAllowed = false;
                c39dv.SuppressDropDownArrow = true;
                c39dv.ShowErrorBox = true;
                sh.AddValidationData(c39dv);

                newRow.CreateCell(10).SetCellValue(QuoteID);
                currentRow++;
                newRow = sh.CreateRow(currentRow);
                newRow.Height = 260;
                XSSFDataValidationConstraint noteConstraint = new XSSFDataValidationConstraint(new String[] { "Note" });
                NPOI.SS.Util.CellRangeAddressList noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                XSSFDataValidation notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                newRow.CreateCell(2).SetCellValue("Note");
                newRow.GetCell(2).CellStyle = RequiredStyle;
                noteConstraint = new XSSFDataValidationConstraint(new String[] { "Dollars (Optional)" });
                noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 9, 9);
                notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                newRow.CreateCell(9).SetCellValue("Dollars (Optional)");
                newRow.GetCell(9).CellStyle = RequiredStyle;


                newRow.CreateCell(0).SetCellValue("Upload as new Version");
                newRow.CreateCell(1).SetCellValue("No");
                XSSFDataValidationConstraint constraintVersion = new XSSFDataValidationConstraint(new String[] { "No", "Yes" });
                NPOI.SS.Util.CellRangeAddressList verLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 1, 1);
                XSSFDataValidation verdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintVersion, verLoc);
                verdv.ShowErrorBox = true;
                verdv.EmptyCellAllowed = false;
                sh.AddValidationData(verdv);
                newRow.GetCell(1).CellStyle = ws;


                Int64 CurrentIndex = 0;
                if (QuoteID != "")
                {
                    sql2.CommandText = "Select quoTSGCompanyID from tblQuote where quoQuoteID = @quoteID";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@quoteID", QuoteID);
                    SqlDataReader dr2 = sql2.ExecuteReader();
                    string companyid = "";
                    if (dr2.Read())
                    {
                        companyid = dr2.GetValue(0).ToString();
                    }
                    dr2.Close();
                    if (System.Convert.ToInt64(companyid) == Company)
                    {
                        if (FirstQuoteID == "0")
                        {
                            FirstQuoteID = QuoteID;
                        }
                        SqlConnection NoteConnection = new SqlConnection(master.getConnectionString());
                        NoteConnection.Open();
                        SqlCommand NoteSQL = new SqlCommand();
                        NoteSQL.Connection = NoteConnection;
                        NoteSQL.Parameters.Clear();
                        NoteSQL.CommandText = "select pwnPreWordedNote, pwnCostNote from linkPWNToQuote, pktblPreWordedNote where pwqQuoteID=@quote and pwqPreWordedNoteID=pwnPreWordedNoteID order by pwnPreWordedNoteID";
                        NoteSQL.Parameters.AddWithValue("@quote", QuoteID);
                        SqlDataReader NoteDR = NoteSQL.ExecuteReader();
                        while (NoteDR.Read())
                        {
                            currentRow++;
                            CurrentIndex++;
                            newRow = sh.CreateRow(currentRow);
                            newRow.Height = 260;
                            newRow.CreateCell(2).SetCellValue(NoteDR["pwnPreWordedNote"].ToString());
                            if (NoteDR["pwnCostNote"].ToString().Trim() != "")
                            {
                                // try in case they put gibberish in there
                                try
                                {
                                    newRow.CreateCell(9).SetCellValue(System.Convert.ToDecimal(NoteDR["pwnCostNote"].ToString()).ToString("0.00"));
                                }
                                catch
                                {

                                }
                            }
                        }
                        NoteDR.Close();
                        NoteConnection.Close();
                    }
                }
                else if (Company == 12)
                {
                    for(int i = 0; i < defaultNotes.Count; i++)
                    {
                        currentRow++;
                        CurrentIndex++;
                        newRow = sh.CreateRow(currentRow);
                        newRow.Height = 260;
                        newRow.CreateCell(2).SetCellValue(defaultNotes[i]);
                        if (defaultNotes[i].Contains("Tooling Cost"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(A" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                        else if (defaultNotes[i].Contains("Shipping Cost"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(F" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                        else if (defaultNotes[i].Contains("Coating Budget"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(H" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                        else if (defaultNotes[i].Contains("Cost for Up To (1) Days Homeline Support"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(E" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                        else if (defaultNotes[i].Contains("Fixture Cost:"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(D" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                        else if (defaultNotes[i].Contains("Tryout Material Cost"))
                        {
                            XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                            costCell.CellStyle = CurrencyStyle;
                            string costFormula = "SUM(B" + (currentRow - (System.Convert.ToInt32(noteOrder[i]) + 1)).ToString() + ")";
                            costCell.SetCellFormula(costFormula);
                        }
                    }


                }
                else if (Company == 3)
                {
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("");
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Tool Cost");

                    XSSFCell toolCostCell = (XSSFCell)newRow.CreateCell(9);
                    toolCostCell.CellStyle = CurrencyStyle;
                    String toolCostFormula = "SUM(";
                    toolCostFormula += "A" + (currentRow - 2).ToString();
                    //tcFormula += ":";
                    //tcFormula += "AH" + (currentRow + 1).ToString();
                    toolCostFormula += ")";
                    toolCostCell.SetCellFormula(toolCostFormula);
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Fixture Cost");
                    XSSFCell fixtureCostCell = (XSSFCell)newRow.CreateCell(9);
                    fixtureCostCell.CellStyle = CurrencyStyle;
                    String fixtureCostFormula = "SUM(D" + (currentRow - 3).ToString() + ")";
                    fixtureCostCell.SetCellFormula(fixtureCostFormula);
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Shipping Cost");
                    XSSFCell shippingCostCell = (XSSFCell)newRow.CreateCell(9);
                    shippingCostCell.CellStyle = CurrencyStyle;
                    String shippingCostFormula = "SUM(F" + (currentRow - 4).ToString() + ")";
                    shippingCostCell.SetCellFormula(shippingCostFormula);
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Homeline Cost");
                    XSSFCell homelineCostCell = (XSSFCell)newRow.CreateCell(9);
                    homelineCostCell.CellStyle = CurrencyStyle;
                    String homelineCostFormula = "SUM(E" + (currentRow - 5).ToString() + ")";
                    homelineCostCell.SetCellFormula(homelineCostFormula);
                }
                else
                {
                    headerFont.Boldweight = 400;
                    headerFont.IsItalic = false;
                    headerFont.FontHeight = 12;
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Station Line Up");
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    XSSFCell stationLineUpCell = (XSSFCell)newRow.CreateCell(2);
                    stationLineUpCell.SetCellFormula("L" + (currentRow - 4).ToString());
                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("*");

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Tooling Cost:");
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell toolCostCell = (XSSFCell)newRow.CreateCell(9);
                    toolCostCell.CellStyle = CurrencyStyle;
                    String toolCostFormula = "SUM(";
                    toolCostFormula += "A" + (currentRow - 4).ToString();
                    toolCostFormula += ")";
                    toolCostCell.SetCellFormula(toolCostFormula);

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Fixture Cost:");
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell fixtureCostCell = (XSSFCell)newRow.CreateCell(9);
                    fixtureCostCell.CellStyle = CurrencyStyle;
                    String fixtureCostFormula = "SUM(D" + (currentRow - 5).ToString() + ")";
                    fixtureCostCell.SetCellFormula(fixtureCostFormula);

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Shipping Cost:");
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell shippingCostCell = (XSSFCell)newRow.CreateCell(9);
                    shippingCostCell.CellStyle = CurrencyStyle;
                    String shippingCostFormula = "SUM(F" + (currentRow - 6).ToString() + ")";
                    shippingCostCell.SetCellFormula(shippingCostFormula);

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    if(Company == 7)
                    {
                        newRow.CreateCell(2).SetCellValue("Cost for (1 week) of support at homeline buy off: ");
                    }
                    else
                    {
                        newRow.CreateCell(2).SetCellValue("Homeline Cost:");
                    }
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell homelineCostCell = (XSSFCell)newRow.CreateCell(9);
                    homelineCostCell.CellStyle = CurrencyStyle;
                    String homelineCostFormula = "SUM(E" + (currentRow - 7).ToString() + ")";
                    homelineCostCell.SetCellFormula(homelineCostFormula);

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    newRow.CreateCell(2).SetCellValue("Tryout Material Cost:");
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell tryoutCostFormula = (XSSFCell)newRow.CreateCell(9);
                    tryoutCostFormula.CellStyle = CurrencyStyle;
                    tryoutCostFormula.SetCellFormula("B" + (currentRow - 8).ToString());

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    if (Company == 8)
                    {
                        newRow.CreateCell(2).SetCellValue("Transfer Bars and Fingers Cost (includes budget of $3,500 for Kinematic Simulation): ");
                    }
                    else
                    {
                        newRow.CreateCell(2).SetCellValue("Transfer Bars and Fingers Cost:");
                    }
                    newRow.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell transferBarCostFormula = (XSSFCell)newRow.CreateCell(9);
                    transferBarCostFormula.CellStyle = CurrencyStyle;
                    transferBarCostFormula.SetCellFormula("C" + (currentRow - 9).ToString());

                    currentRow++;
                    CurrentIndex++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                    XSSFCell additionalCostTextCell = (XSSFCell)newRow.CreateCell(2);
                    additionalCostTextCell.SetCellFormula("G" + (currentRow - 10).ToString());
                    XSSFCell additionalCostCell = (XSSFCell)newRow.CreateCell(9);
                    additionalCostCell.CellStyle = CurrencyStyle;
                    additionalCostCell.SetCellFormula("H" + (currentRow - 10).ToString());
                    //quote reflects data received 

                    headerFont.Boldweight = 700;
                    headerFont.IsItalic = true;
                    headerFont.FontHeight = 14;
                }
                // this will give us one blank row even if there are 20 or more note lines
                currentRow++;
                newRow = sh.CreateRow(currentRow);
                newRow.Height = 260;

                for (Int64 i = CurrentIndex; i < 19; i++)
                {
                    currentRow++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.Height = 260;
                }
            }
            dr.Close();
            if (HeaderWritten)
            {
                sh.SetColumnWidth(0, 6500);
                sh.SetColumnWidth(1, 6500);
                sh.SetColumnWidth(2, 3500);
                sh.SetColumnWidth(3, 10000);
                sh.SetColumnWidth(4, 6500);
                sh.SetColumnWidth(5, 3500);
                sh.SetColumnWidth(6, 4500);
                sh.SetColumnWidth(6, 4500);
                sh.SetColumnWidth(7, 8500);
                sh.SetColumnWidth(8, 4500);
                sh.SetColumnWidth(9, 4500);
                sh.SetColumnWidth(10, 4500);
                sh.SetColumnWidth(11, 4500);
                int i = 4;
                while (i < 40)
                {
                    //sh.AutoSizeColumn(i);
                    i++;
                }
                if ((Company == 3) || (Company == 8))
                {
                    sql.CommandText = "select gnoGeneralNote, coalesce(gnqQuoteID,0), gnoDefault from pktblGeneralNote left outer join linkGeneralNoteToQuote on gnqGeneralNoteID=gnoGeneralNoteID and gnqQuoteID=@quote where gnoCompany='LCC' order by gnoGeneralNoteID ";
                }
                else
                {
                    if (Company == 9)
                    {
                        sql.CommandText = "select gnoGeneralNote, coalesce(gnqQuoteID,0) from pktblGeneralNote left outer join linkGeneralNoteToQuote on gnqGeneralNoteID=gnoGeneralNoteID and gnqQuoteID=@quote where gnoCompany='HTS' order by gnoGeneralNoteID ";
                    }
                    else
                    {
                        sql.CommandText = "select gnoGeneralNote, coalesce(gnqQuoteID,0) from pktblGeneralNote left outer join linkGeneralNoteToQuote on gnqGeneralNoteID=gnoGeneralNoteID and gnqQuoteID=@quote where gnoCompany='general' order by gnoGeneralNoteID ";
                    }
                }
                currentRow++;
                var newRow = sh.CreateRow(currentRow);
                newRow.Height = 260;
                XSSFDataValidationConstraint noteConstraint = new XSSFDataValidationConstraint(new String[] { "General Notes (apply to all parts)" });
                NPOI.SS.Util.CellRangeAddressList noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 3, 3);
                XSSFDataValidation notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                newRow.CreateCell(3).SetCellValue("General Notes (apply to all parts)");
                noteConstraint = new XSSFDataValidationConstraint(new String[] { "Select (X)" });
                noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                newRow.CreateCell(2).SetCellValue("Select (X)");
                sql.Parameters.Clear();
                // it will be zero and thus not match if there is no quote yet for the system
                sql.Parameters.AddWithValue("@quote", FirstQuoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    currentRow++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.CreateCell(3).SetCellValue(dr["gnoGeneralNote"].ToString());
                    if (dr.GetValue(0).ToString() != "0")
                    {
                        if (Company == 3)
                        {
                            if (dr.GetBoolean(2))
                            {
                                newRow.CreateCell(2).SetCellValue("X");
                            }
                            else
                            {
                                newRow.CreateCell(2).SetCellValue("");
                            }
                        }
                        else
                        {
                            newRow.CreateCell(2).SetCellValue("X");
                        }
                    } else
                    {
                        newRow.CreateCell(2).SetCellValue("");
                    }
                    newRow.GetCell(2).CellStyle = CenterStyle;
                }
                dr.Close();
                //sh.CreateFreezePane(2, 5);
                sh.ForceFormulaRecalculation = true;
                wb.SetSheetHidden(1, true);
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "QuoteSheet-RFQ" + RFQID + ".xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();
            }
            else
            {
                context.Response.Write("File Not Created. The most likely cause is that your company has not reserved any of the parts.");
            }
            connection.Close();
        }

        public void header (XSSFSheet sh, int currentRow, XSSFCellStyle RequiredStyle, XSSFFont headerFont, XSSFCellStyle ws, long company)
        {
            var row = sh.CreateRow(currentRow);


            //dead fields
            //row.CreateCell(4).SetCellValue("Cost/Square Inch");
            //row.CreateCell(12).SetCellValue("Blank Weight");
            //row.CreateCell(9).SetCellValue("Min Tonnage");
            //row.CreateCell(10).SetCellValue("Station Lineup");
            //row.CreateCell(0).SetCellValue("SPM");



            row.CreateCell(0).SetCellValue("Picture");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(1).SetCellValue("Customer Quote #");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(2).SetCellValue("Line Number");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(3).SetCellValue("Part Number");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(4).SetCellValue("Part Name");
            row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(5).SetCellValue("Cavity");
            row.GetCell(5).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(5).CellStyle = RequiredStyle;
            row.CreateCell(6).SetCellValue("Process");
            row.GetCell(6).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(6).CellStyle = RequiredStyle;
            row.CreateCell(7).SetCellValue("Type of Part");
            row.GetCell(7).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(7).CellStyle = RequiredStyle;
            row.CreateCell(8).SetCellValue("Material Type");
            row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(8).CellStyle = RequiredStyle;
            row.CreateCell(9).SetCellValue("Blank Thickness");
            row.GetCell(9).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(9).CellStyle = RequiredStyle;
            row.CreateCell(10).SetCellValue("Blank Width");
            row.GetCell(10).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(10).CellStyle = RequiredStyle;
            row.CreateCell(11).SetCellValue("Blank Pitch");
            row.GetCell(11).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(11).CellStyle = RequiredStyle;

            row = sh.CreateRow(currentRow + 2);
            row.CreateCell(0).SetCellValue("# Of Stations");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(0).CellStyle = RequiredStyle;
            row.CreateCell(1).SetCellValue("F to B");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(1).CellStyle = RequiredStyle;
            row.CreateCell(2).SetCellValue("L to R");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(2).CellStyle = RequiredStyle;
            row.CreateCell(3).SetCellValue("Shut Height");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(3).CellStyle = RequiredStyle;

            row.CreateCell(4).SetCellValue("US or Blend?");
            row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(4).CellStyle = RequiredStyle;
            row.CreateCell(5).SetCellValue("Estimator");
            row.GetCell(5).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(5).CellStyle = RequiredStyle;
            row.CreateCell(6).SetCellValue("Shipping Terms");
            row.GetCell(6).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(6).CellStyle = RequiredStyle;
            row.CreateCell(7).SetCellValue("Payment Terms");
            row.GetCell(7).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(7).CellStyle = RequiredStyle;
            row.CreateCell(8).SetCellValue("Timing (Weeks)");
            row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(8).CellStyle = RequiredStyle;
            row.CreateCell(9).SetCellValue("Measurements");
            row.GetCell(9).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(10).SetCellValue("Shipping Location");
            row.GetCell(10).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(11).SetCellValue("Station Line Up: ");
            row.GetCell(11).RichStringCellValue.ApplyFont(headerFont);



            row = sh.CreateRow(currentRow + 4);
            row.CreateCell(0).SetCellValue("Tooling Cost");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(1).SetCellValue("Tryout Material Cost");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(2).SetCellValue("Transfer Bar and Finger Cost");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(3).SetCellValue("Fixture Cost");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(4).SetCellValue("Die Support");
            row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(5).SetCellValue("Shipping Cost");
            row.GetCell(5).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(6).SetCellValue("Additional Cost Description");
            row.GetCell(6).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(6).CellStyle = ws;
            row.CreateCell(7).SetCellValue("Additional Cost Dollars");
            row.GetCell(7).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(8).SetCellValue("Total Cost");
            row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(9).SetCellValue("Currency");
            row.GetCell(9).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(10).SetCellValue("Quote ID");
            row.GetCell(10).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(11).SetCellValue("Access #");
            row.GetCell(11).RichStringCellValue.ApplyFont(headerFont);
        }

        public NPOI.SS.UserModel.IRow GetOrCreateRow(XSSFSheet referenceSheet, Int32 currentRow)
        {
            if (currentRow > maxRow)
            {
                maxRow = currentRow;
                return referenceSheet.CreateRow(currentRow);
            }
            else
            {
                return referenceSheet.GetRow(currentRow);
            }

        }
        public Double GetDoubleValue(string input)
        {
            Double val = 0;
            try
            {
                val = System.Convert.ToDouble(input);
            }
            catch
            {

            }
            return val;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}