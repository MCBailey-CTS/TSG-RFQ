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
    public class HTSQuoteSheet : IHttpHandler
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
            }
            catch { }

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
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("HTS Quote Sheet");
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
            while (sdr.Read())
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

            sql.CommandText = "Select rfqID, prtPartNumber, prtPartDescription, rfqCustomerRFQNumber, CustomerName, ShipToName, CustomerContact.Name, mtyMaterialType, prtRFQLineNumber, ";
            sql.CommandText += "hquHTSQuoteID, hquVersion, hquPartNumbers, hquPartName, ptyPartTypeDescription, dtyFullName, cavCavityName, steShippingTerms, ptePaymentTerms, ShipCode, ";
            sql.CommandText += "hquLeadTime, hquCustomerContactName, hquAccess, hquMaterialType, prtPicture, hquPicture, hquUseTSGLogo, hquUseTSGName, hquCustomerRFQNum, tcyToolCountry, ";
            sql.CommandText += "ProgramName, prtPARTID, estEmail, hquJobNumberID ";
            sql.CommandText += "from tblRFQ ";
            sql.CommandText += "inner join linkPartToRFQ on ptrRFQID = rfqID ";
            sql.CommandText += "inner join tblPart on prtPARTID = ptrPartID ";
            sql.CommandText += "inner join Customer on Customer.CustomerID = rfqCustomerID ";
            sql.CommandText += "inner join CustomerLocation on CustomerLocationID = rfqPlantID ";
            sql.CommandText += "inner join TSGCompany on TSGCompanyID = 9 ";
            sql.CommandText += "inner join linkPartReservedToCompany on prcPartID = prtPARTID and prcTSGCompanyID = TSGCompanyID ";
            sql.CommandText += "left outer join Program on ProgramID = rfqProgramID ";
            sql.CommandText += "left outer join CustomerContact on CustomerContactID = rfqCustomerContact ";
            sql.CommandText += "left outer join linkPartToQuote on ptqPartID = prtPARTID and ptqHTS = 1 and ptqSTS = 0 and ptqUGS = 0 ";
            sql.CommandText += "left outer join tblHTSQuote on hquHTSQuoteID = ptqQuoteID ";
            sql.CommandText += "left outer join pktblPartType on ptyPartTypeID = hquPartTypeID ";
            sql.CommandText += "left outer join pktblEstimators on estEstimatorID = hquEstimatorID ";
            sql.CommandText += "left outer join pktblPaymentTerms on ptePaymentTermsID = hquPaymentTerms ";
            sql.CommandText += "left outer join pktblShippingTerms on steShippingTermsID = hquShippingTerms ";
            sql.CommandText += "left outer join pktblCavity on cavCavityID = hquCavity ";
            sql.CommandText += "left outer join DieType on DieTypeID = hquProcess ";
            sql.CommandText += "left outer join pktblMaterialType on mtyMaterialTypeID = prtPartMaterialType ";
            sql.CommandText += "left outer join pktblToolCountry on tcyToolCountryID = hquToolCountryID ";
            sql.CommandText += "where rfqID = @rfq ";
            sql.Parameters.Clear();

            sql.Parameters.AddWithValue("@rfq", RFQID);
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
                if (onlyNewParts && dr["hquHTSQuoteID"].ToString() != "")
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
                                Picture.Resize(.15);
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
                                    Double Factor = 0.5;
                                    if (dr["TSGCompanyAbbrev"].ToString() == "HTS")
                                    {
                                        Factor = 0.15;
                                    }
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
                    if (dr["hquUseTSGLogo"].ToString() == "True")
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
                if (dr["hquCustomerRFQNum"].ToString() != "")
                {
                    newRow.CreateCell(1).SetCellValue(dr["hquCustomerRFQNum"].ToString());
                }
                else
                {
                    newRow.CreateCell(1).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                }
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
                while (dr3.Read())
                {
                    partNum += " - " + dr3.GetValue(0).ToString();
                }
                dr3.Close();
                if (dr["hquPartNumbers"].ToString() == "")
                {
                    newRow.CreateCell(3).SetCellValue(partNum);
                }
                else
                {
                    newRow.CreateCell(3).SetCellValue(dr["hquPartNumbers"].ToString());
                }
                newRow.GetCell(3).CellStyle = ws;
                //newRow.CreateCell(4).SetCellValue(dr["prtPartRevLevEAU"].ToString());
                if (dr["hquPartName"].ToString() == "")
                {
                    newRow.CreateCell(4).SetCellValue(dr["prtPartDescription"].ToString());
                }
                else
                {
                    newRow.CreateCell(4).SetCellValue(dr["hquPartName"].ToString());
                }
                newRow.GetCell(4).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList cavityloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 5, 5);
                XSSFDataValidation cavitydv = (XSSFDataValidation)dvHelper.CreateValidation(constraintCavity, cavityloc);
                cavitydv.ShowErrorBox = true;
                cavitydv.EmptyCellAllowed = true;
                sh.AddValidationData(cavitydv);
                newRow.CreateCell(5).SetCellValue(dr["cavCavityName"].ToString());
                newRow.GetCell(5).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList processloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                XSSFDataValidation processdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintProcess, processloc);
                processdv.ShowErrorBox = true;
                processdv.EmptyCellAllowed = true;
                sh.AddValidationData(processdv);
                newRow.CreateCell(6).SetCellValue(dr["dtyFullName"].ToString());
                newRow.GetCell(6).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList typeloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 7, 7);
                XSSFDataValidation typedv = (XSSFDataValidation)dvHelper.CreateValidation(constraintPartType, typeloc);
                typedv.ShowErrorBox = true;
                typedv.EmptyCellAllowed = true;
                sh.AddValidationData(typedv);
                newRow.CreateCell(7).SetCellValue(dr["ptyPartTypeDescription"].ToString());
                newRow.GetCell(7).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList mtypeloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 8, 8);
                XSSFDataValidation mtypedv = (XSSFDataValidation)dvHelper.CreateValidation(constraintMaterialType, mtypeloc);
                mtypedv.ShowErrorBox = true;
                mtypedv.EmptyCellAllowed = true;
                // for now, do not use this drop down
                // sh.AddValidationData(mtypedv);
                if (dr["hquMaterialType"].ToString() == "")
                {
                    newRow.CreateCell(8).SetCellValue(dr["mtyMaterialType"].ToString());
                }
                else
                {
                    newRow.CreateCell(8).SetCellValue(dr["hquMaterialType"].ToString());
                }
                newRow.GetCell(8).CellStyle = ws;

                currentRow = currentRow + 2;
                newRow = sh.CreateRow(currentRow);

                NPOI.SS.Util.CellRangeAddressList toolcountryloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 0, 0);
                XSSFDataValidation toolcountrydv = (XSSFDataValidation)dvHelper.CreateValidation(constraintToolCountry, toolcountryloc);
                toolcountrydv.ShowErrorBox = true;
                toolcountrydv.EmptyCellAllowed = true;
                sh.AddValidationData(toolcountrydv);
                if (dr["tcyToolCountry"].ToString() != "")
                {
                    newRow.CreateCell(0).SetCellValue(dr["tcyToolCountry"].ToString());
                }
                else
                {
                    newRow.CreateCell(0).SetCellValue("NA TOOL");
                }

                newRow.GetCell(0).CellStyle = ws;

                if (dr["hquLeadTime"].ToString() != "")
                {
                    newRow.CreateCell(4).SetCellValue(dr["hquLeadTime"].ToString());
                }
                else
                {
                    newRow.CreateCell(4);
                }


                NPOI.SS.Util.CellRangeAddressList estloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 1, 1);
                XSSFDataValidation estdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintEstimators, estloc);
                estdv.ShowErrorBox = true;
                estdv.EmptyCellAllowed = true;
                sh.AddValidationData(estdv);
                if (dr["estEmail"].ToString() != "")
                {
                    newRow.CreateCell(1).SetCellValue(dr["estEmail"].ToString());
                }
                else if (LoggedInEst != "")
                {
                    newRow.CreateCell(1).SetCellValue(LoggedInEst);
                }
                else
                {
                    newRow.CreateCell(1);
                }
                newRow.GetCell(1).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList stloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                XSSFDataValidation stdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintShippingTerms, stloc);
                stdv.ShowErrorBox = true;
                stdv.EmptyCellAllowed = true;
                sh.AddValidationData(stdv);
                if (dr["steShippingTerms"].ToString() != "")
                {
                    newRow.CreateCell(2).SetCellValue(dr["steShippingTerms"].ToString());
                }
                else
                {
                    newRow.CreateCell(2);
                }
                newRow.GetCell(2).CellStyle = ws;
                NPOI.SS.Util.CellRangeAddressList ptloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 3, 3);
                XSSFDataValidation ptdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintPaymentTerms, ptloc);
                ptdv.ShowErrorBox = true;
                ptdv.EmptyCellAllowed = true;
                sh.AddValidationData(ptdv);
                if (dr["ptePaymentTerms"].ToString() != "")
                {
                    newRow.CreateCell(3).SetCellValue(dr["ptePaymentTerms"].ToString());
                }
                else
                {
                    newRow.CreateCell(3);
                }
                newRow.GetCell(3).CellStyle = ws;

                String QuoteID = dr["hquHTSQuoteID"].ToString().Trim();
                XSSFDataValidationConstraint c39Constraint = new XSSFDataValidationConstraint(new String[] { QuoteID });
                NPOI.SS.Util.CellRangeAddressList c39Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                XSSFDataValidation c39dv = (XSSFDataValidation)dvHelper.CreateValidation(c39Constraint, c39Loc);
                c39dv.EmptyCellAllowed = false;
                c39dv.SuppressDropDownArrow = true;
                c39dv.ShowErrorBox = true;
                sh.AddValidationData(c39dv);

                newRow.CreateCell(8).SetCellValue(dr["hquJobNumberID"].ToString());
                newRow.CreateCell(7).SetCellValue(dr["hquAccess"].ToString());

                newRow.CreateCell(6).SetCellValue(QuoteID);
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

                XSSFDataValidationConstraint quantityConstraint = new XSSFDataValidationConstraint(new String[] { "Quantity" });
                NPOI.SS.Util.CellRangeAddressList quantityLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 7, 7);
                XSSFDataValidation quantitydv = (XSSFDataValidation)dvHelper.CreateValidation(quantityConstraint, quantityLoc);
                quantitydv.EmptyCellAllowed = false;
                quantitydv.SuppressDropDownArrow = true;
                quantitydv.ShowErrorBox = true;
                sh.AddValidationData(quantitydv);
                newRow.CreateCell(7).SetCellValue("Quantity");
                newRow.GetCell(7).CellStyle = RequiredStyle;

                XSSFDataValidationConstraint unitPriceConstraint = new XSSFDataValidationConstraint(new String[] { "Unit Price" });
                NPOI.SS.Util.CellRangeAddressList unitPriceLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 8, 8);
                XSSFDataValidation unitPricedv = (XSSFDataValidation)dvHelper.CreateValidation(unitPriceConstraint, unitPriceLoc);
                unitPricedv.EmptyCellAllowed = false;
                unitPricedv.SuppressDropDownArrow = true;
                unitPricedv.ShowErrorBox = true;
                sh.AddValidationData(unitPricedv);
                newRow.CreateCell(8).SetCellValue("Unit Price");
                newRow.GetCell(8).CellStyle = RequiredStyle;

                noteConstraint = new XSSFDataValidationConstraint(new String[] { "Dollars" });
                noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 9, 9);
                notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                newRow.CreateCell(9).SetCellValue("Dollars");
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
                    string companyid = "9";
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
                        NoteSQL.CommandText = "Select hpwNote, hpwQuantity, hpwUnitPrice from linkHTSPWNToHTSQuote ";
                        NoteSQL.CommandText += "inner join pktblHTSPreWordedNote on hpwHTSPreWordedNoteID = pthHTSPWNID ";
                        NoteSQL.CommandText += "where pthHTSQuoteID = @id";
                        NoteSQL.Parameters.AddWithValue("@id", QuoteID);
                        SqlDataReader NoteDR = NoteSQL.ExecuteReader();
                        while (NoteDR.Read())
                        {
                            currentRow++;
                            CurrentIndex++;
                            newRow = sh.CreateRow(currentRow);
                            newRow.Height = 260;
                            newRow.CreateCell(2).SetCellValue(NoteDR["hpwNote"].ToString());
                            if (NoteDR["hpwQuantity"].ToString().Trim() != "")
                            {
                                // try in case they put gibberish in there
                                try
                                {
                                    newRow.CreateCell(7).SetCellValue(System.Convert.ToDecimal(NoteDR["hpwQuantity"].ToString()).ToString("0"));
                                }
                                catch
                                {

                                }
                                try
                                {
                                    newRow.CreateCell(8).SetCellValue(System.Convert.ToDecimal(NoteDR["hpwUnitPrice"].ToString()).ToString("0.00"));
                                }
                                catch
                                {

                                }
                                XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                                costCell.CellStyle = CurrencyStyle;
                                string costFormula = "H" + (currentRow + 1).ToString() + "*I" + (currentRow + 1).ToString();
                                costCell.SetCellFormula(costFormula);
                            }
                        }
                        NoteDR.Close();
                        NoteConnection.Close();
                    }
                }

                for (Int64 i = CurrentIndex; i < 19; i++)
                {
                    currentRow++;
                    newRow = sh.CreateRow(currentRow);
                    XSSFCell costCell = (XSSFCell)newRow.CreateCell(9);
                    costCell.CellStyle = CurrencyStyle;
                    string costFormula = "H" + (currentRow + 1).ToString() + "*I" + (currentRow + 1).ToString();
                    costCell.SetCellFormula(costFormula);
                    newRow.Height = 260;
                }
                // This makes sure we at least have one blank line between parts / quotes
                currentRow++;
                newRow = sh.CreateRow(currentRow);
                newRow.Height = 260;
                currentRow++;
                newRow = sh.CreateRow(currentRow);
                newRow.Height = 260;

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
                int i = 40;
                //while (i < 40)
                //{
                //    //sh.AutoSizeColumn(i);
                //    i++;
                //}

                sql.CommandText = "select gnoGeneralNote, coalesce(gnqQuoteID,0) from pktblGeneralNote left outer join linkGeneralNoteToQuote on gnqGeneralNoteID=gnoGeneralNoteID and gnqQuoteID=@quote where gnoCompany='HTS' order by gnoGeneralNoteID ";
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
                    }
                    else
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

        public void header(XSSFSheet sh, int currentRow, XSSFCellStyle RequiredStyle, XSSFFont headerFont, XSSFCellStyle ws, long company)
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

            row = sh.CreateRow(currentRow + 2);
            row.CreateCell(0).SetCellValue("US or Blend?");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(0).CellStyle = RequiredStyle;
            row.CreateCell(1).SetCellValue("Estimator");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(1).CellStyle = RequiredStyle;
            row.CreateCell(2).SetCellValue("Shipping Terms");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(2).CellStyle = RequiredStyle;
            row.CreateCell(3).SetCellValue("Payment Terms");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(3).CellStyle = RequiredStyle;
            row.CreateCell(4).SetCellValue("Timing (Weeks)");
            row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
            row.GetCell(4).CellStyle = RequiredStyle;
            row.CreateCell(5).SetCellValue("Shipping Location");
            row.GetCell(5).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(6).SetCellValue("Quote ID");
            row.GetCell(6).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(7).SetCellValue("Access #");
            row.GetCell(7).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(8).SetCellValue("Job #");
            row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
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