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
    /// Summary description for UGSQuoteSheet
    /// </summary>
    public class UGSQuoteSheet : IHttpHandler
    {
        int maxRow = -1;
        public void ProcessRequest(HttpContext context)
        {
            int rfqID = 0;
            Boolean onlyNewParts = false;
            try
            {
                rfqID = System.Convert.ToInt32(context.Request["rfq"]);
            }
            catch
            {
                return;
            }
            try
            {
                onlyNewParts = System.Convert.ToBoolean(context.Request["newParts"]);
            }
            catch
            {

            }

            Site master = new RFQ.Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            SqlCommand sql2 = new SqlCommand();
            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            sql2.Connection = connection2;

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("UGS Quote Sheet");
            XSSFSheet referenceSheet = (XSSFSheet)wb.CreateSheet("REFERENCES");
            // Build All Drop Down Lists

            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);

            int currentRow = 0;
            int currentColumn = 0;

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

            List<string> yesNoList = new List<string>();
            yesNoList.Add("No");
            yesNoList.Add("Yes");
            XSSFDataValidationConstraint constraintYesNo = new XSSFDataValidationConstraint(yesNoList.ToArray());

            NPOI.SS.UserModel.IRow rrow;
            sql.CommandText = "Select Distinct dtyFullName from DieType where TSGCompanyID = 15 order by dtyFullName ";
            sql.Parameters.Clear();
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rrow = getOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                currentRow++;
            }
            dr.Close();
            XSSFDataValidationConstraint dieTypeConstraint = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$A$1:$A$" + currentRow);

            currentRow = 0;
            currentColumn++;
            sql.CommandText = "Select ptePaymentTerms from pktblPaymentTerms order by ptePaymentTerms ";
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rrow = getOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                currentRow++;
            }
            dr.Close();
            XSSFDataValidationConstraint constraintPaymentTerms = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$B$1:$B$" + currentRow);

            currentRow = 0;
            currentColumn++;
            sql.CommandText = "Select concat(estFirstName, ' ', estLastName) as est from pktblEstimators where estCompanyID = 15 order by estEmail ";
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rrow = getOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                currentRow++;
            }
            dr.Close();
            XSSFDataValidationConstraint estimatorConstraint = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$C$1:$C$" + currentRow);

            sql.Parameters.Clear();
            sql.CommandText = " select steShippingTerms from pktblShippingTerms  order by steShippingTerms";
            currentColumn++;
            currentRow = 0;
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rrow = getOrCreateRow(referenceSheet, currentRow);
                rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                currentRow++;
            }
            dr.Close();
            XSSFDataValidationConstraint constraintShippingTerms = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$D$1:$D$" + currentRow);

            string quoteID = "";

            Boolean headerWritten = false;
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

            maxRow = -1;
            //context.Response.Write("File Not Created. The most likely cause is that your company has not reserved any of the parts.");
            int partsOnSheet = 0;
            int lengthWidthHeightLine = 0;

            sql.CommandText = "Select distinct CustomerName, ShipCode, rfqID, ptePaymentTerms, steShippingTerms, rfqCustomerRFQNumber, uquTotalPrice, uquCustomerRFQNumber, prtPARTID, ";
            sql.CommandText += "uquUGSQuoteID, prtRFQLineNumber, concat(estFirstName, ' ', estLastName) as est, estEmail, TSGCompanyAbbrev, prtPartNumber, prtPartDescription, dtyFullName, ";
            sql.CommandText += "uquPicture, uquDieType, uquPartLength, uquPartWidth, uquPartHeight, uquCustomerContact, uquLeadTime, uquJobNumber, uquUseTSG, uquShippingLocation, ";
            sql.CommandText += "uquPartNumber, uquPartName, (Select Name from CustomerContact where rfqCustomerContact = CustomerContactID) as contact, uquCustomerContact, prtPicture, ";
            sql.CommandText += "prtPartLength, prtPartWidth, prtPartHeight, uquNotes, uquHoles ";
            sql.CommandText += "from tblRFQ, Customer, TSGCompany, CustomerLocation, Program, linkPartToRFQ, linkPartReservedToCompany, tblPart ";
            sql.CommandText += "left outer join linkPartToQuote on ptqPartID = prtPARTID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 1 ";
            sql.CommandText += "left outer join tblUGSQuote on uquUGSQuoteID = ptqQuoteID ";
            sql.CommandText += "left outer join DieType on uquDieType = DieTypeID ";
            sql.CommandText += "left outer join  pktblEstimators on uquEstimatorID = estEstimatorID ";
            sql.CommandText += "left outer join pktblPaymentTerms on ptePaymentTermsID = uquPaymentID ";
            sql.CommandText += "left outer join pktblShippingTerms on steShippingTermsID = uquShippingID ";
            sql.CommandText += "left outer join pktblUGSCost on ucoUGSCostID = uquUGSQuoteID ";
            sql.CommandText += "where rfqID = @rfq and rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocationID and rfqProgramID = ProgramID and ";
            sql.CommandText += "ptrRFQID = rfqID and prtPARTID = ptrPartID and prcTSGCompanyID = TSGCompany.TSGCompanyID and TSGCompany.TSGCompanyID = 15 and prcPartID = prtPARTID ";
            sql.CommandText += "order by prtRFQLineNumber ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", rfqID);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                partsOnSheet++;
                if (onlyNewParts && dr["uquUGSQuoteID"].ToString() != "")
                {
                    continue;
                }
                if (quoteID == "")
                {
                    quoteID = dr["uquUGSQuoteID"].ToString();
                }
                if (!headerWritten)
                {
                    var row = sh.CreateRow(0);
                    // Company Logos

                    try
                    {
                        using (var clientContext = new ClientContext(siteUrl))
                        {
                            // TSG Picture
                            clientContext.Credentials = master.getSharePointCredentials();
                            var url = new Uri(siteUrl);
                            var relativeUrl = string.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "TSG.png");
                            try
                            {
                                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                using (var memstr = new System.IO.MemoryStream())
                                {
                                    var buffer = new byte[1024 * 16];
                                    int byteSize;
                                    while ((byteSize = fileInfo.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memstr.Write(buffer, 0, byteSize);
                                    }
                                    pictureData = memstr.ToArray();
                                }
                                XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, 0, 0, 0);
                                anchor.AnchorType = 2;
                                int pictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                                XSSFPicture picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, pictureIndex);
                                picture.Resize(.048);
                            }
                            catch { }


                            // UGS Picture
                            clientContext.Credentials = master.getSharePointCredentials();
                            url = new Uri(siteUrl);
                            relativeUrl = string.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "UGS.png");
                            try
                            {
                                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                using (var memstr = new System.IO.MemoryStream())
                                {
                                    var buffer = new byte[1024 * 16];
                                    int byteSize;
                                    while ((byteSize = fileInfo.Stream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memstr.Write(buffer, 0, byteSize);
                                    }
                                    pictureData = memstr.ToArray();
                                }
                                XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 1, 0, 1, 0);
                                anchor.AnchorType = 2;
                                int pictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                                XSSFPicture picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, pictureIndex);
                                picture.Resize(.038);
                            }
                            catch { }
                        }
                    }
                    catch { }

                    row.CreateCell(2).SetCellValue("Logo for quote");
                    row.CreateCell(4).SetCellValue("Use TSG name or company name");

                    NPOI.SS.Util.CellRangeAddressList logoloc = new NPOI.SS.Util.CellRangeAddressList(0, 0, 3, 3);
                    XSSFDataValidation logoDV = (XSSFDataValidation)dvHelper.CreateValidation(constraintLogo, logoloc);
                    logoDV.ShowErrorBox = true;
                    logoDV.EmptyCellAllowed = false;
                    sh.AddValidationData(logoDV);

                    NPOI.SS.Util.CellRangeAddressList nameloc = new NPOI.SS.Util.CellRangeAddressList(0, 0, 5, 5);
                    XSSFDataValidation nameDV = (XSSFDataValidation)dvHelper.CreateValidation(constraintName, nameloc);
                    nameDV.ShowErrorBox = true;
                    nameDV.EmptyCellAllowed = false;
                    sh.AddValidationData(nameDV);

                    

                    if (dr["uquUseTSG"].ToString() == "True")
                    {
                        row.CreateCell(3).SetCellValue("TSG");
                        row.CreateCell(5).SetCellValue("TSG");
                    }
                    else
                    {
                        row.CreateCell(3).SetCellValue("Company Logo");
                        row.CreateCell(5).SetCellValue("Company Name");
                    }

                    row.Height = 1500;
                    currentRow += 2;
                    row = sh.CreateRow(currentRow);
                    row.CreateCell(0).SetCellValue(dr["rfqCustomerRFQNumber"].ToString() + " Engineering Estimate");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.GetCell(0).RichStringCellValue.ApplyFont(0, dr["rfqCustomerRFQNumber"].ToString().Length, blueFont);

                    row.CreateCell(3).SetCellValue("Customer");
                    row.CreateCell(4).SetCellValue("Plant");
                    row.CreateCell(5).SetCellValue("Contact");
                    row.CreateCell(6).SetCellValue("Shipping Terms");
                    row.CreateCell(7).SetCellValue("Payment Terms");
                    currentRow++;
                    NPOI.SS.Util.CellRangeAddressList stloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                    XSSFDataValidation stdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintShippingTerms, stloc);
                    stdv.ShowErrorBox = true;
                    stdv.EmptyCellAllowed = true;
                    sh.AddValidationData(stdv);

                    NPOI.SS.Util.CellRangeAddressList ptloc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 7, 7);
                    XSSFDataValidation ptdv = (XSSFDataValidation)dvHelper.CreateValidation(constraintPaymentTerms, ptloc);
                    ptdv.ShowErrorBox = true;
                    ptdv.EmptyCellAllowed = true;
                    sh.AddValidationData(ptdv);
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue(dr["ShipCode"].ToString());
                    if (dr["uquCustomerContact"].ToString() != "")
                    {
                        row.CreateCell(5).SetCellValue(dr["uquCustomerContact"].ToString());
                    }
                    else
                    {
                        row.CreateCell(5).SetCellValue(dr["contact"].ToString());
                    }
                    if ( dr["steShippingTerms"].ToString() != "")
                    {
                        row.CreateCell(6).SetCellValue(dr["steShippingTerms"].ToString());
                    }
                    else
                    {
                        row.CreateCell(6).SetCellValue("FOB OUR DOCK");
                    }
                    row.GetCell(6).CellStyle = ws;
                    if (dr["ptePaymentTerms"].ToString() != "")
                    {
                        row.CreateCell(7).SetCellValue(dr["ptePaymentTerms"].ToString());
                    }
                    else
                    {
                        row.CreateCell(7).SetCellValue("NET 30");
                    }
                    row.GetCell(6).CellStyle = ws;

                    

                    currentRow += 2;
                    headerWritten = true;
                }
                header(sh, currentRow, RequiredStyle, headerFont, ws, 15);

                currentRow++;
                var r = sh.CreateRow(currentRow);
                r.Height = 1000;
                r.CreateCell(0);
                r.CreateCell(1).SetCellValue(dr["uquCustomerRFQNumber"].ToString());

                // Part Pictures
                sharepointLibrary = "Part Pictures";
                using (var clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = master.getSharePointCredentials();
                    var url = new Uri(siteUrl);
                    var relativeUrl = string.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["prtPicture"].ToString());
                    try
                    {
                        using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                        using (var memstr = new System.IO.MemoryStream())
                        {
                            var buffer = new byte[1024 * 16];
                            int byteSize;
                            while ((byteSize = fileInfo.Stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                memstr.Write(buffer, 0, byteSize);
                            }
                            pictureData = memstr.ToArray();
                        }
                        XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, currentRow, 0, currentRow);
                        anchor.AnchorType = 2;
                        int pictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                        XSSFPicture picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, pictureIndex);
                        picture.Resize(.22);
                    }
                    catch
                    {

                    }
                }

                r.CreateCell(2).SetCellValue(dr["prtRFQLineNumber"].ToString());
                XSSFDataValidationConstraint c2Constraint = new XSSFDataValidationConstraint(new string[] { dr["prtRFQLineNumber"].ToString() });
                NPOI.SS.Util.CellRangeAddressList c2Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                XSSFDataValidation c2DV = (XSSFDataValidation)dvHelper.CreateValidation(c2Constraint, c2Loc);
                c2DV.EmptyCellAllowed = false;
                c2DV.SuppressDropDownArrow = true;
                c2DV.ShowErrorBox = true;
                sh.AddValidationData(c2DV);
                SqlDataReader dr2;
                string partNum = "";
                if (dr["uquPartNumber"].ToString() != "")
                {
                    partNum = dr["uquPartNumber"].ToString();
                }
                else
                {
                    partNum = dr["prtPartNumber"].ToString();
                    string partID = dr["prtPARTID"].ToString();
                    sql2.CommandText = "Select prtPartNumber from tblPart, linkPartToPartDetail where ppdPartID = prtPARTID and ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and prtPARTID <> @partID ";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@partID", partID);
                    dr2 = sql2.ExecuteReader();
                    while (dr2.Read())
                    {
                        partNum += " - " + dr2["prtPartNumber"].ToString();
                    }
                    dr2.Close();
                }
                r.CreateCell(3).SetCellValue(partNum);
                r.GetCell(3).CellStyle = ws;

                if (dr["uquPartName"].ToString() != "")
                {
                    r.CreateCell(4).SetCellValue(dr["uquPartName"].ToString());
                }
                else
                {
                    r.CreateCell(4).SetCellValue(dr["prtPartDescription"].ToString());
                }
                r.GetCell(4).CellStyle = ws;

                NPOI.SS.Util.CellRangeAddressList quoteTypeLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 5, 5);
                XSSFDataValidation quoteTypeDV = (XSSFDataValidation)dvHelper.CreateValidation(dieTypeConstraint, quoteTypeLoc);
                quoteTypeDV.ShowErrorBox = true;
                quoteTypeDV.EmptyCellAllowed = true;
                sh.AddValidationData(quoteTypeDV);
                //r.CreateCell(7);
                //r.GetCell(7).CellStyle = ws;

                if (dr["dtyFullName"].ToString() != "")
                {
                    r.CreateCell(5).SetCellValue(dr["dtyFullName"].ToString());
                }
                else
                {
                    r.CreateCell(5).SetCellValue("Attribute Fixture");
                }
                r.GetCell(5).CellStyle = ws;

                NPOI.SS.Util.CellRangeAddressList estLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                XSSFDataValidation estDV = (XSSFDataValidation)dvHelper.CreateValidation(estimatorConstraint, estLoc);
                estDV.ShowErrorBox = true;
                estDV.EmptyCellAllowed = true;
                sh.AddValidationData(estDV);

                string user = "";
                sql2.CommandText = "Select concat(estFirstName, ' ', estLastName) as est from pktblEstimators where estCompanyID = 15 and estEmail = @email ";
                sql2.Parameters.Clear();
                sql2.Parameters.AddWithValue("@email", master.getUserName());
                dr2 = sql2.ExecuteReader();
                if (dr2.Read())
                {
                    user = dr2.GetValue(0).ToString();
                }
                dr2.Close();

                if (dr["est"].ToString() != " ")
                {
                    r.CreateCell(6).SetCellValue(dr["est"].ToString());
                }
                else if (user != "")
                {
                    r.CreateCell(6).SetCellValue(user);
                }
                else
                {
                    r.CreateCell(6).SetCellValue("Jeff Momber");
                }
                r.GetCell(6).CellStyle = ws;

                lengthWidthHeightLine = currentRow + 1;

                if (dr["uquPartLength"].ToString() != "")
                {
                    r.CreateCell(7).SetCellValue(dr["uquPartLength"].ToString());
                }
                else
                {
                    r.CreateCell(7).SetCellValue(dr["prtPartLength"].ToString());
                }
                r.GetCell(7).CellStyle = ws;
                if (dr["uquPartWidth"].ToString() != "")
                {
                    r.CreateCell(8).SetCellValue(dr["uquPartWidth"].ToString());
                }
                else
                {
                    r.CreateCell(8).SetCellValue(dr["prtPartWidth"].ToString());
                }
                r.GetCell(8).CellStyle = ws;
                if (dr["uquPartHeight"].ToString() != "")
                {
                    r.CreateCell(9).SetCellValue(dr["uquPartHeight"].ToString());
                }
                else
                {
                    r.CreateCell(9).SetCellValue(dr["prtPartHeight"].ToString());
                }
                r.GetCell(9).CellStyle = ws;

                if (dr["uquLeadTime"].ToString() != "")
                {
                    r.CreateCell(10).SetCellValue(dr["uquLeadTime"].ToString());
                }
                else
                {
                    r.CreateCell(10).SetCellValue("2 week design 7 weeks build");
                }
                r.GetCell(10).CellStyle = ws;

                currentRow += 2;
                r = sh.CreateRow(currentRow);

                r.CreateCell(0).SetCellValue(dr["uquJobNumber"].ToString());
                r.CreateCell(1).SetCellValue(dr["uquTotalPrice"].ToString());
                r.CreateCell(2).SetCellValue(dr["uquShippingLocation"].ToString());
                r.CreateCell(3).SetCellValue(dr["uquHoles"].ToString());

                currentRow++;

                r = sh.CreateRow(currentRow);
                r.CreateCell(0).SetCellValue("Upload as new version");
                NPOI.SS.Util.CellRangeAddressList yesNoLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 1, 1);
                XSSFDataValidation yesNoDV = (XSSFDataValidation)dvHelper.CreateValidation(constraintYesNo, yesNoLoc);
                yesNoDV.ShowErrorBox = true;
                yesNoDV.EmptyCellAllowed = true;
                sh.AddValidationData(yesNoDV);
                r.CreateCell(1).SetCellValue("No");

                r.Height = 260;
                XSSFDataValidationConstraint noteConstraint = new XSSFDataValidationConstraint(new String[] { "Note" });
                NPOI.SS.Util.CellRangeAddressList noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                XSSFDataValidation notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                r.CreateCell(2).SetCellValue("Note");
                r.GetCell(2).CellStyle = RequiredStyle;
                noteConstraint = new XSSFDataValidationConstraint(new String[] { "Dollars (Optional)" });
                noteLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 9, 9);
                notedv = (XSSFDataValidation)dvHelper.CreateValidation(noteConstraint, noteLoc);
                notedv.EmptyCellAllowed = false;
                notedv.SuppressDropDownArrow = true;
                notedv.ShowErrorBox = true;
                sh.AddValidationData(notedv);
                r.CreateCell(9).SetCellValue("Dollars (Optional)");
                r.GetCell(9).CellStyle = RequiredStyle;
                currentRow++;


                if (dr["uquUGSQuoteID"].ToString() != "")
                {
                    sql2.CommandText = "Select pwnPreWordedNote, pwnCostNote from linkPWNToUGSQuote ";
                    sql2.CommandText += "inner join pktblPreWordedNote on pwnPreWordedNoteID = puqPreWordedNoteID ";
                    sql2.CommandText += "where puqUGSQuoteID = @quoteId ";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@quoteId", quoteID);
                    SqlDataReader sdr = sql2.ExecuteReader();
                    while (sdr.Read())
                    {
                        r = getOrCreateRow(sh, currentRow);
                        string note = sdr["pwnPreWordedNote"].ToString();
                        double cost = System.Convert.ToDouble(sdr["pwnCostNote"].ToString());
                        r.CreateCell(2).SetCellValue(note);
                        if (cost != 0)
                        {
                            r.CreateCell(9).SetCellValue(cost.ToString("0.00"));
                        }
                        currentRow++;
                    }
                    sdr.Close();
                    string notes = dr["uquNotes"].ToString();
                    string[] tokenizedNotes = notes.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    for (int i = 0; i < tokenizedNotes.Count(); i++)
                    {
                        r = getOrCreateRow(sh, currentRow);
                        r.CreateCell(2).SetCellValue(tokenizedNotes[i]);
                        currentRow++;
                    }
                }
                else
                {
                    sql2.CommandText = "Select dqnDefaultQuoteNote, dqnCost from pktblDefaultQuoteNotes where dqnCompanyID = 15 order by dqnOrder ";
                    sql2.Parameters.Clear();
                    SqlDataReader sdr = sql2.ExecuteReader();
                    while (sdr.Read())
                    {
                        r = getOrCreateRow(sh, currentRow);
                        string note = sdr["dqnDefaultQuoteNote"].ToString();
                        double cost = System.Convert.ToDouble(sdr["dqnCost"].ToString());
                        r.CreateCell(2).SetCellValue(note);
                        if (cost != 0)
                        {
                            r.CreateCell(9).SetCellValue(cost.ToString("0.00"));
                        }
                        currentRow++;
                    }
                    sdr.Close();
                }

                currentRow = budget(sh, currentRow, dr["uquUGSQuoteID"].ToString(), RequiredStyle, headerFont, ws, lengthWidthHeightLine);
            }
            dr.Close();
            
            if (headerWritten)
            {
                sh.SetColumnWidth(0, 6500);
                sh.SetColumnWidth(1, 6500);
                sh.SetColumnWidth(2, 3500);
                sh.SetColumnWidth(3, 10000);
                sh.SetColumnWidth(4, 7000);
                sh.SetColumnWidth(5, 4000);
                sh.SetColumnWidth(6, 4500);
                sh.SetColumnWidth(6, 4500);
                sh.SetColumnWidth(7, 8500);
                sh.SetColumnWidth(8, 4500);
                sh.SetColumnWidth(9, 4500);
                sh.SetColumnWidth(10, 4500);
                sh.SetColumnWidth(11, 4500);

                currentRow += 3;
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

                sql.CommandText = "Select gnoGeneralNote, gnoDefault, gnuUGSQuoteID ";
                sql.CommandText += "from pktblGeneralNote ";
                sql.CommandText += "left outer join linkGeneralNoteToUGSQuote on gnuGeneralNoteID = gnoGeneralNoteID and gnuUGSQuoteID = @quoteID ";
                sql.CommandText += "where gnoCompany = 'UGS' ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    currentRow++;
                    newRow = sh.CreateRow(currentRow);
                    newRow.CreateCell(3).SetCellValue(dr["gnoGeneralNote"].ToString());
                    if (quoteID != "")
                    {
                        if (dr["gnuUGSQuoteID"].ToString() != "")
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
                        if (dr.GetBoolean(1))
                        {
                            newRow.CreateCell(2).SetCellValue("X");
                        }
                        else
                        {
                            newRow.CreateCell(2).SetCellValue("");
                        }
                    }
                    newRow.GetCell(2).CellStyle = CenterStyle;
                }
                dr.Close();
            }


            connection.Close();
            connection2.Close();

            if (partsOnSheet == 0)
            {
                context.Response.Write("File Not Created. The most likely cause is that your company has not reserved any of the parts.");
            }
            else
            {
                sh.ForceFormulaRecalculation = true;
                wb.SetSheetHidden(1, true);
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "QuoteSheet-RFQ" + rfqID + ".xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();
            }
        }

        public void header(XSSFSheet sh, int currentRow, XSSFCellStyle RequiredStyle, XSSFFont headerFont, XSSFCellStyle ws, int company)
        {
            var row = sh.CreateRow(currentRow);
            

            row.CreateCell(0).SetCellValue("Picture");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(1).SetCellValue("Customer RFQ #");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(2).SetCellValue("Line Number");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(3).SetCellValue("Part Number");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(4).SetCellValue("Part Name");
            row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(5).SetCellValue("Quote Type");
            row.GetCell(5).RichStringCellValue.ApplyFont(headerFont);
            //row.GetCell(5).CellStyle = RequiredStyle;
            row.CreateCell(6).SetCellValue("Estimator");
            row.GetCell(6).RichStringCellValue.ApplyFont(headerFont);
            //row.GetCell(6).CellStyle = RequiredStyle;
            row.CreateCell(7).SetCellValue("Length");
            row.GetCell(7).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(8).SetCellValue("Width");
            row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(9).SetCellValue("Height");
            row.GetCell(9).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(10).SetCellValue("Lead Time");
            row.GetCell(10).RichStringCellValue.ApplyFont(headerFont);

            row = sh.CreateRow(currentRow + 2);
            row.CreateCell(0).SetCellValue("Job Number");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(1).SetCellValue("Total Cost");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);

            row = sh.CreateRow(currentRow + 2);
            row.CreateCell(0).SetCellValue("Job Number");
            row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(1).SetCellValue("Total Cost");
            row.GetCell(1).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(2).SetCellValue("Shipping Location");
            row.GetCell(2).RichStringCellValue.ApplyFont(headerFont);
            row.CreateCell(3).SetCellValue("# of Holes");
            row.GetCell(3).RichStringCellValue.ApplyFont(headerFont);
        }

        public int budget(XSSFSheet sh, int currentRow, string quoteID, XSSFCellStyle RequiredStyle, XSSFFont headerFont, XSSFCellStyle ws, int lengthWidthHeightLine)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            currentRow += 2;
            var row = sh.CreateRow(currentRow);
            if (quoteID == "")
            {
                sql.CommandText = "Select ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ucoCNC, ucoCertification, ucoGageRRCMM, ";
                sql.CommandText += "ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ";
                sql.CommandText += "ucoBasePlate, ucoAluminum, ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ucoIndStorCase, ";
                sql.CommandText += "ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ucoToolingBalls, ucoTBCovers, ucoTBPads, ucoSlides, ";
                sql.CommandText += "ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ucoHeatTreat, ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ";
                sql.CommandText += "ucoWelding, ucoWireBurn, ucoRebates ";
                sql.CommandText += "from pktblUGSCost where ucoUGSCostID = 1 ";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Labor");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(4).SetCellValue("Material");
                    row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(8).SetCellValue("Outsourcing");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);

                    string length = "H" + lengthWidthHeightLine.ToString();
                    string width = "I" + lengthWidthHeightLine.ToString();
                    string holes = "D" + (lengthWidthHeightLine + 2).ToString();
                    currentRow++;
                    int linesStart = currentRow;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Management", dr["ucoManagement"].ToString(),"0");
                    budgetFormulaLine(row, 4, currentRow, "Base Plate", "1", "IF((" + length + "+16)*(" + width + "+16)>30*30,((" + length + "+16)*(" + width + "+16)*1.2),((" + length + "+16)*(" + width + "+16)))"); //
                    budgetLine(row, 8, currentRow, "Annodizing", dr["ucoAnnodizing"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Project Eng", dr["ucoProjectEng"].ToString(), "0");
                    budgetFormulaLine(row, 4, currentRow, "Aluminum", dr["ucoAluminum"].ToString(), length + " * " + width + " * " + "J" + lengthWidthHeightLine.ToString() + " * .1 * 3"); //
                    budgetLine(row, 8, currentRow, "Black Oxide", dr["ucoBlackOxide"].ToString(), "1");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Design");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Steel", dr["ucoSteel"].ToString(), "4");
                    budgetLine(row, 8, currentRow, "Heat Treat", dr["ucoHeatTreat"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Read Data", dr["ucoReadData"].ToString(), "0");
                    //budgetFormulaLine(row, 0, currentRow, "Read Data", dr["ucoReadData"].ToString(), ".2 * C" + (currentRow + 7).ToString());
                    budgetLine(row, 4, currentRow, "Fixture Plank", dr["ucoFixturePlank"].ToString(), "0");
                    budgetLine(row, 8, currentRow, "Engraved Tags", dr["ucoEngrvdTags"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "3-D Model", dr["uco3dModel"].ToString(), "0"); //
                    //budgetFormulaLine(row, 0, currentRow, "3-D Model", dr["uco3dModel"].ToString(), ".2 * C" + (currentRow + 6).ToString());
                    budgetLine(row, 4, currentRow, "Wood", dr["ucoWood"].ToString(), "0"); 
                    budgetLine(row, 8, currentRow, "CNC Services", dr["ucoCNCServices"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    //budgetLine(row, 0, currentRow, "Drawings", dr["ucoDrawing"].ToString(), ""); //
                    budgetFormulaLine(row, 0, currentRow, "Drawings", dr["ucoDrawing"].ToString(), ".8 * C" + (currentRow + 5).ToString());
                    budgetFormulaLine(row, 4, currentRow, "Bushings", dr["ucoBushings"].ToString(), holes);
                    budgetLine(row, 8, currentRow, "ID/OD Gridning", dr["ucoGrinding"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Updates", dr["ucoUpdates"].ToString(), "0");
                    //budgetFormulaLine(row, 0, currentRow, "Updates", dr["ucoUpdates"].ToString(), ".2 * C" + (currentRow + 4).ToString());
                    budgetFormulaLine(row, 4, currentRow, "Drill Blanks", dr["ucoDrillBlanks"].ToString(), holes);
                    budgetLine(row, 8, currentRow, "Shipping", dr["ucoShipping"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("CNC");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Clamps", dr["ucoClamps"].ToString(), "4");
                    budgetLine(row, 8, currentRow, "Third Party CMM", dr["ucoThirdPartyCMM"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Programming", dr["ucoProgramming"].ToString(), "0");
                    budgetLine(row, 4, currentRow, "Indicator", dr["ucoIndicator"].ToString(), "0");
                    budgetLine(row, 8, currentRow, "Welding", dr["ucoWelding"].ToString(), "1");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetFormulaLine(row, 0, currentRow, "CNC", dr["ucoCNC"].ToString(), "(I" + lengthWidthHeightLine.ToString() + "+J" + lengthWidthHeightLine.ToString() + ") * " + "H" + lengthWidthHeightLine.ToString() + " / 5");
                    //budgetLine(row, 0, currentRow, "CNC", dr["ucoCNC"].ToString(), ""); //
                    budgetLine(row, 4, currentRow, "Ind Collar", dr["ucoIndCollar"].ToString(), "0");
                    budgetLine(row, 8, currentRow, "Wire Burn", dr["ucoWireBurn"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("CMM");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Ind Store Case", dr["ucoIndStorCase"].ToString(), "0");
                    row.CreateCell(8).SetCellValue("Total-Service");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell c = (XSSFCell)row.CreateCell(11);
                    c.SetCellFormula("Sum(L" + (linesStart + 1).ToString() + ":L" + (currentRow).ToString() + ")");
                    int totalService = currentRow - 1;

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Certification", dr["ucoCertification"].ToString(), ""); //Ask if we want a 14 default or not
                    budgetLine(row, 4, currentRow, "Zero Set", dr["ucoZeroSet"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    //budgetLine(row, 0, currentRow, "Gage R&R", dr["ucoGageRRCMM"].ToString(), ""); //
                    budgetFormulaLine(row, 0, currentRow, "Gage R&R", dr["ucoGageRRCMM"].ToString(), ".8 * C" + (currentRow - 2).ToString());
                    budgetLine(row, 4, currentRow, "Spc Triggers", dr["ucoSpcTriggers"].ToString(), "0");
                    budgetLine(row, 8, currentRow, "Rebates", dr["ucoRebates"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Part Layouts", dr["ucoPartLayouts"].ToString(), "0"); //
                    budgetLine(row, 4, currentRow, "Temp Drops", dr["ucoTempDrops"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Fixtures");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Hinge Drops", dr["ucoHingeDrops"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Base", dr["ucoBase"].ToString(), "2");
                    budgetLine(row, 4, currentRow, "Risers", dr["ucoRisers"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    //budgetLine(row, 0, currentRow, "Details", dr["ucoDetails"].ToString(), "12");
                    budgetFormulaLine(row, 0, currentRow, "Details", dr["ucoDetails"].ToString(), ".7 * C" + (currentRow - 6).ToString());
                    budgetLine(row, 4, currentRow, "Handles", dr["ucoHandles"].ToString(), "2");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetFormulaLine(row, 0, currentRow, "Location Pins", dr["ucoLocationPins"].ToString(), holes + " * 2");
                    budgetLine(row, 4, currentRow, "Jig Feet", dr["ucoJigFeet"].ToString(), "4");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetFormulaLine(row, 0, currentRow, "Go / No Go Pins", dr["ucoGoNoGoPins"].ToString(), holes + " * 2");
                    budgetLine(row, 4, currentRow, "Tooling Balls", dr["ucoToolingBalls"].ToString(), "3");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "SPC's", dr["ucoSpc"].ToString(), "0");
                    budgetLine(row, 4, currentRow, "TB Covers", dr["ucoTBCovers"].ToString(), "3");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Gage R&R", dr["ucoGageRRFixtures"].ToString(), "0");
                    budgetLine(row, 4, currentRow, "TB Pads", dr["ucoTBPads"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    //budgetLine(row, 0, currentRow, "Assemble", dr["ucoAssemble"].ToString(), "12");
                    budgetFormulaLine(row, 0, currentRow, "Assemble", dr["ucoAssemble"].ToString(), ".7 * C" + (currentRow - 11).ToString());
                    budgetLine(row, 4, currentRow, "Slides", dr["ucoSlides"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("General");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Magnets", dr["ucoMagnets"].ToString(), "0");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Pallets & Crates", dr["ucoPallets"].ToString(), "1");
                    budgetLine(row, 4, currentRow, "Hardware", dr["ucoHardware"].ToString(), "1");
                    row.CreateCell(8).SetCellValue("Sub Total");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("D" + (currentRow + 3).ToString() + "+H" + (currentRow + 3).ToString() + "+L" + (totalService + 2).ToString() + "-L" + (totalService + 4).ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Transportation", dr["ucoTransportation"].ToString(), "1");
                    budgetLine(row, 4, currentRow, "LMI", dr["ucoLMI"].ToString(), "0");
                    row.CreateCell(8).SetCellValue("Margin");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("L" + (currentRow).ToString() + "*.15");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Total Labor");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(3).SetCellFormula("Sum(D" + (linesStart + 1).ToString() + ":D" + (currentRow).ToString() + ")");
                    row.CreateCell(4).SetCellValue("Total Material");
                    row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(7).SetCellFormula("Sum(H" + (linesStart + 1).ToString() + ":H" + (currentRow).ToString() + ")");
                    row.CreateCell(8).SetCellValue("Total");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("L" + (currentRow - 1).ToString() + "+L" + (currentRow));
                }
                dr.Close();
            }
            else
            {
                sql.CommandText = "Select ucoManagement, ucoProjectEng, ucoReadData, uco3DModel, ucoDrawing, ucoUpdates, ucoProgramming, ucoCNC, ucoCertification, ucoGageRRCMM,  ";
                sql.CommandText += "ucoPartLayouts, ucoBase, ucoDetails, ucoLocationPins, ucoGoNoGoPins, ucoSPC, ucoGageRRFixtures, ucoAssemble, ucoPallets, ucoTransportation, ";
                sql.CommandText += "ucoBasePlate, ucoAluminum, ucoSteel, ucoFixturePlank, ucoWood, ucoBushings, ucoDrillBlanks, ucoClamps, ucoIndicator, ucoIndCollar, ucoIndStorCase, ";
                sql.CommandText += "ucoZeroSet, ucoSpcTriggers, ucoTempDrops, ucoHingeDrops, ucoRisers, ucoHandles, ucoJigFeet, ucoToolingBalls, ucoTBCovers, ucoTBPads, ucoSlides, ";
                sql.CommandText += "ucoMagnets, ucoHardware, ucoLMI, ucoAnnodizing, ucoBlackOxide, ucoHeatTreat, ucoEngrvdTags, ucoCNCServices, ucoGrinding, ucoShipping, ucoThirdPartyCMM, ";
                sql.CommandText += "ucoWelding, ucoWireBurn, ucoRebates, uquManagement, uquProjectEng, uquReadData, uqu3DModel, uquDrawing, uquUpdates, uquPrograming, uquCNC, uquCertification, ";
                sql.CommandText += "uquGageRRCMM, uquPartLayouts, uquBase, uquDetails, uquLocationPins, uquGoNoGoPins, uquSPC, uquGageRRFixtures, uquAssemble, uquPallets, uquTransportation, ";
                sql.CommandText += "uquBasePlate, uquAluminum, uquSteel, uquFixturePlank, uquWood, uquBushings, uquDrillBlanks, uquClamps, uquIndicator, uquIndCollar, uquIndStorCase,  ";
                sql.CommandText += "uquZeroSet, uquSpcTriggers, uquTempDrops, uquHingeDrops, uquRisers, uquHandles, uquJigFeet, uquToolingBalls, uquTBCovers, uquTBPads, uquSlides, ";
                sql.CommandText += "uquMagnets, uquHardware, uquLMI, uquAnnodizing, uquBlackOxide, uquHeatTreat, uquEngrvdTags, uquCNCServices, uquGrinding, uquShipping, uquThirdPartyCMM, ";
                sql.CommandText += "uquWelding, uquWireBurn, uquRebates ";
                sql.CommandText += "from tblUGSQuote, pktblUGSCost where ucoUGSCostID = uquUGSCostID and uquUGSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Labor");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(4).SetCellValue("Material");
                    row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(8).SetCellValue("Outsourcing");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);

                    currentRow++;
                    int linesStart = currentRow;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Management", dr["ucoManagement"].ToString(), dr["uquManagement"].ToString());
                    budgetLine(row, 4, currentRow, "Base Plate", dr["ucoBasePlate"].ToString(), dr["uquBasePlate"].ToString());
                    budgetLine(row, 8, currentRow, "Annodizing", dr["ucoAnnodizing"].ToString(), dr["uquAnnodizing"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Project Eng", dr["ucoProjectEng"].ToString(), dr["uquProjectEng"].ToString());
                    budgetLine(row, 4, currentRow, "Aluminum", dr["ucoAluminum"].ToString(), dr["uquAluminum"].ToString());
                    budgetLine(row, 8, currentRow, "Black Oxide", dr["ucoBlackOxide"].ToString(), dr["uquBlackOxide"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Design");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Steel", dr["ucoSteel"].ToString(), dr["uquSteel"].ToString());
                    budgetLine(row, 8, currentRow, "Heat Treat", dr["ucoHeatTreat"].ToString(), dr["uquHeatTreat"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Read Data", dr["ucoReadData"].ToString(), dr["uquReadData"].ToString());
                    budgetLine(row, 4, currentRow, "Fixture Plank", dr["ucoFixturePlank"].ToString(), dr["uquFixturePlank"].ToString());
                    budgetLine(row, 8, currentRow, "Engraved Tags", dr["ucoEngrvdTags"].ToString(), dr["uquEngrvdTags"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "3-D Model", dr["uco3dModel"].ToString(), dr["uqu3dModel"].ToString());
                    budgetLine(row, 4, currentRow, "Wood", dr["ucoWood"].ToString(), dr["uquWood"].ToString());
                    budgetLine(row, 8, currentRow, "CNC Services", dr["ucoCNCServices"].ToString(), dr["uquCNCServices"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Drawings", dr["ucoDrawing"].ToString(), dr["uquDrawing"].ToString());
                    budgetLine(row, 4, currentRow, "Bushings", dr["ucoBushings"].ToString(), dr["uquBushings"].ToString());
                    budgetLine(row, 8, currentRow, "ID/OD Gridning", dr["ucoGrinding"].ToString(), dr["uquGrinding"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Updates", dr["ucoUpdates"].ToString(), dr["uquUpdates"].ToString());
                    budgetLine(row, 4, currentRow, "Drill Blanks", dr["ucoDrillBlanks"].ToString(), dr["uquDrillBlanks"].ToString());
                    budgetLine(row, 8, currentRow, "Shipping", dr["ucoShipping"].ToString(), dr["uquShipping"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("CNC");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Clamps", dr["ucoClamps"].ToString(), dr["uquClamps"].ToString());
                    budgetLine(row, 8, currentRow, "Third Party CMM", dr["ucoThirdPartyCMM"].ToString(), dr["uquThirdPartyCMM"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Programming", dr["ucoProgramming"].ToString(), dr["uquPrograming"].ToString());
                    budgetLine(row, 4, currentRow, "Indicator", dr["ucoIndicator"].ToString(), dr["uquIndicator"].ToString());
                    budgetLine(row, 8, currentRow, "Welding", dr["ucoWelding"].ToString(), dr["uquWelding"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "CNC", dr["ucoCNC"].ToString(), dr["uquCNC"].ToString());
                    budgetLine(row, 4, currentRow, "Ind Collar", dr["ucoIndCollar"].ToString(), dr["uquIndCollar"].ToString());
                    budgetLine(row, 8, currentRow, "Wire Burn", dr["ucoWireBurn"].ToString(), dr["uquWireBurn"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("CMM");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Ind Store Case", dr["ucoIndStorCase"].ToString(), dr["uquIndStorCase"].ToString());
                    row.CreateCell(8).SetCellValue("Total-Service");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    XSSFCell c = (XSSFCell)row.CreateCell(11);
                    c.SetCellFormula("Sum(L" + (linesStart + 1).ToString() + ":L" + (currentRow).ToString() + ")");
                    int totalService = currentRow - 1;

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Certification", dr["ucoCertification"].ToString(), dr["uquCertification"].ToString());
                    budgetLine(row, 4, currentRow, "Zero Set", dr["ucoZeroSet"].ToString(), dr["uquZeroSet"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Gage R&R", dr["ucoGageRRCMM"].ToString(), dr["uquGageRRCMM"].ToString());
                    budgetLine(row, 4, currentRow, "Spc Triggers", dr["ucoSpcTriggers"].ToString(), dr["uquSpcTriggers"].ToString());
                    budgetLine(row, 8, currentRow, "Rebates", dr["ucoRebates"].ToString(), dr["uquRebates"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Part Layouts", dr["ucoPartLayouts"].ToString(), dr["uquPartLayouts"].ToString());
                    budgetLine(row, 4, currentRow, "Temp Drops", dr["ucoTempDrops"].ToString(), dr["uquTempDrops"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Fixtures");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Hinge Drops", dr["ucoHingeDrops"].ToString(), dr["uquHingeDrops"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Base", dr["ucoBase"].ToString(), dr["uquBase"].ToString());
                    budgetLine(row, 4, currentRow, "Risers", dr["ucoRisers"].ToString(), dr["uquRisers"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Details", dr["ucoDetails"].ToString(), dr["uquDetails"].ToString());
                    budgetLine(row, 4, currentRow, "Handles", dr["ucoHandles"].ToString(), dr["uquHandles"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Location Pins", dr["ucoLocationPins"].ToString(), dr["uquLocationPins"].ToString());
                    budgetLine(row, 4, currentRow, "Jig Feet", dr["ucoJigFeet"].ToString(), dr["uquJigFeet"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Go / No Go Pins", dr["ucoGoNoGoPins"].ToString(), dr["uquGoNoGoPins"].ToString());
                    budgetLine(row, 4, currentRow, "Tooling Balls", dr["ucoToolingBalls"].ToString(), dr["uquToolingBalls"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "SPC's", dr["ucoSpc"].ToString(), dr["uquSpc"].ToString());
                    budgetLine(row, 4, currentRow, "TB Covers", dr["ucoTBCovers"].ToString(), dr["uquTBCovers"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Gage R&R", dr["ucoGageRRFixtures"].ToString(), dr["uquGageRRFixtures"].ToString());
                    budgetLine(row, 4, currentRow, "TB Pads", dr["ucoTBPads"].ToString(), dr["uquTBPads"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Assemble", dr["ucoAssemble"].ToString(), dr["uquAssemble"].ToString());
                    budgetLine(row, 4, currentRow, "Slides", dr["ucoSlides"].ToString(), dr["uquSlides"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("General");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    budgetLine(row, 4, currentRow, "Magnets", dr["ucoMagnets"].ToString(), dr["uquMagnets"].ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Pallets & Crates", dr["ucoPallets"].ToString(), dr["uquPallets"].ToString());
                    budgetLine(row, 4, currentRow, "Hardware", dr["ucoHardware"].ToString(), dr["uquHardware"].ToString());
                    row.CreateCell(8).SetCellValue("Sub Total");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("D" + (currentRow + 3).ToString() + "+H" + (currentRow + 3).ToString() + "+L" + (totalService + 2).ToString() + "-L" + (totalService + 4).ToString());

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    budgetLine(row, 0, currentRow, "Transportation", dr["ucoTransportation"].ToString(), dr["uquTransportation"].ToString());
                    budgetLine(row, 4, currentRow, "LMI", dr["ucoLMI"].ToString(), dr["uquLMI"].ToString());
                    row.CreateCell(8).SetCellValue("Margin");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("L" + (currentRow).ToString() + "*.15");

                    currentRow++;
                    row = getOrCreateRow(sh, currentRow);
                    row.CreateCell(0).SetCellValue("Total Labor");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(3).SetCellFormula("Sum(D" + (linesStart + 1).ToString() + ":D" + (currentRow).ToString() + ")");
                    row.CreateCell(4).SetCellValue("Total Material");
                    row.GetCell(4).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(7).SetCellFormula("Sum(H" + (linesStart + 1).ToString() + ":H" + (currentRow).ToString() + ")");
                    row.CreateCell(8).SetCellValue("Total");
                    row.GetCell(8).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(11).SetCellFormula("L" + (currentRow - 1).ToString() + "+L" + (currentRow));
                }
                dr.Close();
            }


            connection.Close();

            return currentRow + 2;
        }

        public void budgetLine(NPOI.SS.UserModel.IRow row, int columnStart, int currentRow, string name, string amount, string cost)
        {
            row.CreateCell(columnStart).SetCellValue(name);
            row.CreateCell(columnStart + 1).SetCellValue(amount);
            row.CreateCell(columnStart + 2).SetCellValue(cost);
            if (columnStart == 0)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("B" + (currentRow + 1).ToString() + "*C" + (currentRow + 1).ToString());
            }
            else if (columnStart == 4)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("F" + (currentRow + 1).ToString() + "*G" + (currentRow + 1).ToString());
            }
            else if (columnStart == 8)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("J" + (currentRow + 1).ToString() + "*K" + (currentRow + 1).ToString());
            }
        }

        public void budgetFormulaLine (NPOI.SS.UserModel.IRow row, int columnStart, int currentRow, string name, string amount, string cost)
        {
            row.CreateCell(columnStart).SetCellValue(name);
            row.CreateCell(columnStart + 1).SetCellValue(amount);
            XSSFCell c = (XSSFCell)row.CreateCell(columnStart + 2);
            //c.SetCellFormula("Sum(L" + (linesStart + 1).ToString() + ":L" + (currentRow).ToString() + ")");
            c.SetCellFormula(cost);
            if (columnStart == 0)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("B" + (currentRow + 1).ToString() + "*C" + (currentRow + 1).ToString());
            }
            else if (columnStart == 4)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("F" + (currentRow + 1).ToString() + "*G" + (currentRow + 1).ToString());
            }
            else if (columnStart == 8)
            {
                XSSFCell cell = (XSSFCell)row.CreateCell(columnStart + 3);
                cell.SetCellFormula("J" + (currentRow + 1).ToString() + "*K" + (currentRow + 1).ToString());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public NPOI.SS.UserModel.IRow getOrCreateRow(XSSFSheet referenceSheet, int currentRow)
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
    }
}