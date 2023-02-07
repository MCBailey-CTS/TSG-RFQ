using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;
using Microsoft.SharePoint.Client;
using System.Net.Mail;
using NPOI.SS.UserModel;

namespace RFQ
{
    /// <summary>
    /// Summary description for EntireRFQSummary
    /// </summary>
    public class EntireRFQSummary : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Int64 RFQID = 0;
            int customer = 0;
            int plant = 0;
            string start = "";
            string end = "";
            try
            {
                RFQID = System.Convert.ToInt64(context.Request["rfq"]);
            }
            catch
            {
                //return;
            }
            try
            {
                customer = System.Convert.ToInt32(context.Request["customer"]);
            }
            catch
            {

            }
            try
            {
                plant = System.Convert.ToInt32(context.Request["plant"]);
            }
            catch
            {

            }
            try
            {
                start = context.Request["start"];
            }
            catch
            {

            }
            try
            {
                end = context.Request["end"];
            }
            catch
            {

            }
            if (RFQID == 0 && customer == 0 && plant == 0)
            {
                return;
            }
            double total = 0;

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select rfqCustomerRFQNumber, quoRFQID, quoTSGCompanyID, quoTotalAmount, quoQuoteID, TSGCompanyAbbrev, rfqDueDate, prtPartNumber, prtPartDescription,  ";
            sql.CommandText += "d.dtyFullName as ProcessName, prtPicture, prtPartMaterialType,   dinDieType, c.cavCavityName, dinSizeFrontToBackEnglish,dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish,  ";
            sql.CommandText += "dinSizeLeftToRightMetric, dinSizeShutHeightEnglish,dinSizeShutHeightMetric, dinNumberOfStations,  quoLeadTime, binMaterialWidthEnglish, binMaterialWidthMetric, binMaterialPitchEnglish,   ";
            sql.CommandText += "binMaterialPitchMetric, binMaterialThicknessEnglish, binMaterialThicknessMetric, m.mtyMaterialType, rfqID, prtRFQLineNumber, quoVersion, hquHTSQuoteID, hquMaterialType,  ";
            sql.CommandText += "d1.dtyFullName, c1.cavCavityName as cavCavityName1, hquLeadTime, (Select SUM(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote where hquHTSQuoteID = pthHTSQuoteID and hpwHTSPreWordedNoteID = pthHTSPWNID) as htsCost,  ";
            sql.CommandText += "squSTSQuoteID, squProcess, squLeadTime, (Select sum(pwnCostNote) from linkPWNToSTSQuote, pktblPreWordedNote where squSTSQuoteID = psqSTSQuoteID and pwnPreWordedNoteID = psqPreWordedNoteID) as stsCost, uquUGSQuoteID, d2.dtyFullName as dtyFullName1, ";
            sql.CommandText += "uquLeadTime, uquTotalPrice, quoQuoteID, hquVersion, squQuoteVersion, uquQuoteVersion, quoOldQuoteNumber, prtPARTID ";
            sql.CommandText += "from tblRFQ, tblPart, linkPartToRFQ ";
            sql.CommandText += "left outer join linkPartToQuote on ptqPartID = ptrPartID ";
            sql.CommandText += "left outer join tblQuote on quoQuoteID = ptqQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
            sql.CommandText += "left outer join linkDieInfoToQuote on diqQuoteID = quoQuoteID ";
            sql.CommandText += "left outer join tblDieInfo on dinDieInfoID = diqDieInfoID ";
            sql.CommandText += "left outer join DieType as d on dinDieType = d.DieTypeID ";
            sql.CommandText += "left outer join pktblCavity as c on dinCavityID = c.cavCavityID ";
            sql.CommandText += "left outer join TSGCompany on quoTSGCompanyID = TSGCompany.TSGCompanyID ";
            sql.CommandText += "left outer join pktblBlankInfo on quoBlankInfoID = binBlankInfoID ";
            sql.CommandText += "left outer join pktblMaterialType as m on binBlankMaterialTypeID = mtyMaterialTypeID ";
            sql.CommandText += "left outer join tblHTSQuote on hquHTSQuoteID = ptqQuoteID and ptqHTS = 1 ";
            sql.CommandText += "left outer join DieType as d1 on hquProcess = d1.DieTypeID ";
            sql.CommandText += "left outer join pktblCavity as c1 on c1.cavCavityID = hquCavity ";
            sql.CommandText += "left outer join tblSTSQuote on squSTSQuoteID = ptqQuoteID and ptqSTS = 1 ";
            sql.CommandText += "left outer join tblUGSQuote on uquUGSQuoteID = ptqQuoteID and ptqUGS = 1 ";
            sql.CommandText += "left outer join DieType as d2 on d2.DieTypeID = uquDieType ";
            sql.CommandText += "where ptrRFQID = rfqid ";
            sql.CommandText += "and ptrPartID = prtPARTID ";
            sql.CommandText += "and(prtPARTID = (Select min(ppdPartID) from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPArtToPartDetail where ppdPartID = prtPARTID)) ";
            sql.CommandText += "or(Select min(ppdPartID) from linkPartToPartDetail where ppdPartToPartID = (select ppdPartToPartID from linkPArtToPartDetail where ppdPartID = prtPARTID)) is null)  ";
            if (RFQID != 0)
            {
                sql.CommandText += "and rfqID = @rfq ";
                sql.Parameters.AddWithValue("@rfq", RFQID);
            }
            if (customer != 0 && customer != null)
            {
                sql.CommandText += "and rfqCustomerID = @customer ";
                sql.Parameters.AddWithValue("@customer", customer);
            }
            if (plant != 0 && plant != null)
            {
                sql.CommandText += "and rfqPlantID = @plant ";
                sql.Parameters.AddWithValue("@plant", plant);
            }
            if (start != "" && start != null)
            {
                sql.CommandText += "and quoCreated > @start ";
                sql.Parameters.AddWithValue("@start", start);
            }
            if (end != "" && end != null)
            {
                sql.CommandText += "and quoCreated < @end ";
                sql.Parameters.AddWithValue("@end", end);
            }
            sql.CommandText += "order by rfqID desc ";
            SqlDataReader dr = sql.ExecuteReader();
            Boolean HeaderWritten = false;

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Quote Summary");
            // This Patriarch is what is used to position pictures
            XSSFDrawing DrawingPatriarch = (XSSFDrawing)sh.CreateDrawingPatriarch();
            XSSFCellStyle CurrencyStyle;
            XSSFCell cell;
            // This will be used on the numbers
            CurrencyStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CurrencyStyle.DataFormat = CustomFormat.GetFormat("###,###,##0.00");
            XSSFFont headerFont = (XSSFFont)wb.CreateFont();
            headerFont.FontHeight = 14;
            // 700 is BOLD
            // 400 is NORMAL
            headerFont.Boldweight = 700;
            headerFont.IsItalic = true;
            XSSFFont titleFont = (XSSFFont)wb.CreateFont();
            titleFont.FontHeight = 10;
            titleFont.Boldweight = 700;
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
            Int32 currentRow = 0;
            XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);
            String[] CostTypeList = new String[] { "Blank Die", "Die", "Check Fixture", "Shipping", "Home Line", "Spare Pierces, Punches,Buttons" };
            NPOI.SS.Util.CellRangeAddressList CostTypeLocation = new NPOI.SS.Util.CellRangeAddressList(1, 1, 1, 1);
            NPOI.SS.UserModel.IDataValidationConstraint constraint = dvHelper.CreateExplicitListConstraint(CostTypeList);
            XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)constraint;
            XSSFDataValidationConstraint dvc = new XSSFDataValidationConstraint(CostTypeList);
            XSSFDataValidation dv = (XSSFDataValidation)dvHelper.CreateValidation(dvc, CostTypeLocation);
            dv.ShowErrorBox = true;
            //dv.SuppressDropDownArrow = false;            
            sh.AddValidationData(dv);
            //sh.AutoSizeColumn(0);

            // I have no idea what these units are but this makes it the right width
            sh.SetColumnWidth(0, 10000);
            int count = 1;
            while (count < 31)
            {
                sh.AutoSizeColumn(count);
                count++;
            }
            // I have no idea what these units are but this makes it the right width
            sh.SetColumnWidth(31, 4000);

            while (dr.Read())
            {
                if (!HeaderWritten)
                {
                    var row = sh.CreateRow(0);
                    row.CreateCell(0).SetCellValue("Tooling Systems Group");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row = sh.CreateRow(1);
                    row.CreateCell(0).SetCellValue(dr.GetValue(0).ToString() + " Engineering Estimate");
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.GetCell(0).RichStringCellValue.ApplyFont(0, dr.GetValue(0).ToString().Length, blueFont);
                    row = sh.CreateRow(2);
                    // TODO Format as Date
                    row.CreateCell(0).SetCellValue(DateTime.Now.ToString("d"));
                    row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                    row.CreateCell(12).SetCellValue("Shut");
                    row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                    row.CreateCell(13).SetCellValue("Shut");
                    row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                    if (master.getCompanyId() != 8)
                    {

                        row.CreateCell(30).SetCellValue("Spare");
                        row.GetCell(30).RichStringCellValue.ApplyFont(titleFont);
                    }

                    row = sh.CreateRow(3);
                    row.CreateCell(0);
                    row.CreateCell(1);
                    row.CreateCell(2);
                    row.CreateCell(3).SetCellValue("Customer");
                    row.CreateCell(4).SetCellValue("Due");
                    row.CreateCell(5);
                    row.CreateCell(6);
                    row.CreateCell(7);
                    row.CreateCell(8);
                    row.CreateCell(9).SetCellValue("F to B");
                    row.CreateCell(10).SetCellValue("F to B");
                    row.CreateCell(11).SetCellValue("L to R");
                    row.CreateCell(12).SetCellValue("L to R");
                    row.CreateCell(13).SetCellValue("Height");
                    row.CreateCell(14).SetCellValue("Height");
                    row.CreateCell(15).SetCellValue("Number");
                    row.CreateCell(16).SetCellValue("Width");
                    row.CreateCell(17).SetCellValue("Width");
                    row.CreateCell(18).SetCellValue("Pitch");
                    row.CreateCell(19).SetCellValue("Pitch");
                    row.CreateCell(20).SetCellValue("Thickness");
                    row.CreateCell(21).SetCellValue("Thickness");
                    row.CreateCell(22);
                    row.CreateCell(23).SetCellValue("Lead");
                    row.CreateCell(24).SetCellValue("Lead");
                    row.CreateCell(25);
                    row.CreateCell(26);
                    row.CreateCell(27);
                    row.CreateCell(28);
                    row.CreateCell(29);
                    row.CreateCell(30);
                    if (master.getCompanyId() != 8)
                    {
                        row.CreateCell(31).SetCellValue("Pierce, Punches");
                    }
                    for (int i = 0; i < 32; i++)
                    {
                        try
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        catch
                        {

                        }
                    }
                    row = sh.CreateRow(4);
                    row.CreateCell(0).SetCellValue("Picture");
                    row.CreateCell(1).SetCellValue("Quote#");
                    row.CreateCell(2).SetCellValue("Group");
                    row.CreateCell(3).SetCellValue("RFQ Number");
                    row.CreateCell(4).SetCellValue("Date");
                    row.CreateCell(5).SetCellValue("Part Number");
                    row.CreateCell(6).SetCellValue("Description");
                    row.CreateCell(7).SetCellValue("Process");
                    row.CreateCell(8).SetCellValue("Cavity");
                    row.CreateCell(9).SetCellValue("Inch");
                    row.CreateCell(10).SetCellValue("MM");
                    row.CreateCell(11).SetCellValue("Inch");
                    row.CreateCell(12).SetCellValue("MM");
                    row.CreateCell(13).SetCellValue("Inch");
                    row.CreateCell(14).SetCellValue("MM");
                    row.CreateCell(15).SetCellValue("Stations");
                    row.CreateCell(16).SetCellValue("Inch");
                    row.CreateCell(17).SetCellValue("MM");
                    row.CreateCell(18).SetCellValue("Inch");
                    row.CreateCell(19).SetCellValue("MM");
                    row.CreateCell(20).SetCellValue("Inch");
                    row.CreateCell(21).SetCellValue("MM");
                    row.CreateCell(22).SetCellValue("Material Type");
                    row.CreateCell(23).SetCellValue("Time");
                    row.CreateCell(24).SetCellValue("Time");
                    if (master.getCompanyId() == 8)
                    {
                        row.CreateCell(25).SetCellValue("Station Line Up");
                        sh.SetColumnWidth(25, 20000);
                        row.CreateCell(26).SetCellValue("Target");
                        row.CreateCell(27).SetCellValue("Check Fixture");
                        for (int i = 0; i < 28; i++)
                        {
                            try
                            {
                                row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        row.CreateCell(25).SetCellValue("Target");
                        row.CreateCell(26).SetCellValue("Blank Die");
                        row.CreateCell(27).SetCellValue("Die");
                        row.CreateCell(28).SetCellValue("Check Fixture");
                        row.CreateCell(29).SetCellValue("Shipping");
                        row.CreateCell(30).SetCellValue("Home Line");
                        row.CreateCell(31).SetCellValue("and Buttons");
                        row.CreateCell(32).SetCellValue("Total");
                        for (int i = 0; i < 33; i++)
                        {
                            try
                            {
                                row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                            }
                            catch
                            {

                            }
                        }
                    }
                    if (master.getCompanyId() == 8)
                    {
                        sh.SetColumnWidth(9, 0);
                        sh.SetColumnWidth(11, 0);
                        sh.SetColumnWidth(13, 0);
                        sh.SetColumnWidth(16, 0);
                        sh.SetColumnWidth(18, 0);
                        sh.SetColumnWidth(20, 0);
                    }

                    HeaderWritten = true;
                    currentRow = 4;
                }
                //If guo ji is downloading the sheet they only want their quotes so if the quote isnt from guo ji we just go to the next 
                //in the list before we progress to the next row in the sheet
                if (master.getCompanyId() == 8 && dr["TSGCompanyAbbrev"].ToString() != "GTS")
                {
                    continue;
                }
                currentRow++;
                var newRow = sh.CreateRow(currentRow);
                // This is in points which is whatever excel reports times 20 
                newRow.Height = 1500;
                newRow.CreateCell(0);

                // get picture from sharepoint and insert
                // This points to where the pictures are
                String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                String sharepointLibrary = "Part Pictures";
                byte[] pictureData;
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
                        // This anchor type will change the picture size as the cell changes size
                        // not using it currently seems to force the picture to overlap the next column
                        anchor.AnchorType = 0;
                        // this anchor type will not resize picture with cell
                        anchor.AnchorType = 2;
                        int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);

                        XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                        // The picture will not appear unless you run resize
                        // in this case, scaling to this value seems to work best
                        Picture.Resize(.4);

                    }
                    catch
                    {

                    }
                }

                if (dr["hquHTSQuoteID"].ToString() == "" && dr["squSTSQuoteID"].ToString() == "" && dr["uquUGSQuoteID"].ToString() == "" && dr["quoQuoteID"].ToString() == "")
                {
                    SqlCommand sql2 = new SqlCommand();
                    SqlConnection connection2 = new SqlConnection(master.getConnectionString());
                    connection2.Open();
                    sql2.Connection = connection2;

                    sql2.CommandText = "Select TSGCompanyAbbrev from linkPartReservedToCompany, TSGCompany where prcTSGCompanyID = TSGCompanyID and prcPartID = @partID ";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@partID", dr["prtPARTID"]);
                    SqlDataReader dr2 = sql2.ExecuteReader();
                    string reserved = "";
                    int reservedCount = 0;
                    while (dr2.Read())
                    {
                        if (reservedCount != 0)
                        {
                            reserved += ", " + dr2["TSGCompanyAbbrev"].ToString();
                        }
                        else
                        {
                            reserved = dr2["TSGCompanyAbbrev"].ToString();
                        }
                        reservedCount++;
                    }
                    dr2.Close();
                    if (reserved != "")
                    {
                        newRow.CreateCell(1).SetCellValue("Reserved By");
                        newRow.CreateCell(2).SetCellValue(reserved);
                    }
                    else
                    {
                        sql2.CommandText = "Select TSGCompanyAbbrev from tblNoQuote, TSGCompany where nquPartID = @partID and nquCompanyID = TSGCompanyID ";
                        sql2.Parameters.Clear();
                        sql2.Parameters.AddWithValue("@partID", dr["prtPARTID"].ToString());
                        dr2 = sql2.ExecuteReader();
                        string noQuoted = "";
                        int noQuoteCount = 0;
                        while (dr2.Read())
                        {
                            if (noQuoteCount != 0)
                            {
                                noQuoted += ", " + dr2["TSGCompanyAbbrev"].ToString();
                            }
                            else
                            {
                                noQuoted = dr2["TSGCompanyAbbrev"].ToString();
                            }
                            noQuoteCount++;
                        }
                        dr2.Close();
                        if (noQuoted != "")
                        {
                            newRow.CreateCell(1).SetCellValue("No Quoted by ");
                            newRow.CreateCell(2).SetCellValue(noQuoted);
                        }
                    }
                    connection2.Close();
                }
                else if (dr["hquHTSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString());
                    newRow.CreateCell(2).SetCellValue("HTS");
                }
                else if (dr["squSTSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString());
                    newRow.CreateCell(2).SetCellValue("STS");
                }
                else if (dr["uquUGSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString());
                    newRow.CreateCell(2).SetCellValue("UGS");
                }
                else
                {
                    if (dr["quoOldQuoteNumber"].ToString() != "")
                    {
                        newRow.CreateCell(1).SetCellValue(dr["quoOldQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString());
                    }
                    else
                    {
                        newRow.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString());
                    }
                    newRow.CreateCell(2).SetCellValue(dr["TSGCompanyAbbrev"].ToString());
                }
                string quotedate = "";
                try
                {
                    quotedate = System.Convert.ToDateTime(dr["rfqDueDate"]).ToString("d");
                }
                catch
                {

                }
                newRow.CreateCell(3).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                newRow.CreateCell(4).SetCellValue(quotedate);
                newRow.CreateCell(5).SetCellValue(dr["prtPartNumber"].ToString());
                newRow.CreateCell(6).SetCellValue(dr["prtPartDescription"].ToString());
                if (dr["quoQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["ProcessName"].ToString());
                    newRow.CreateCell(8).SetCellValue(dr["cavCavityName"].ToString());
                    newRow.CreateCell(9).SetCellValue(GetDoubleValue(dr["dinSizeFrontToBackEnglish"].ToString()).ToString("0.0"));
                    newRow.CreateCell(11).SetCellValue(GetDoubleValue(dr["dinSizeLeftToRightEnglish"].ToString()).ToString("0.0"));
                    newRow.CreateCell(13).SetCellValue(GetDoubleValue(dr["dinSizeShutHeightEnglish"].ToString()).ToString("0.0"));
                    newRow.CreateCell(16).SetCellValue(GetDoubleValue(dr["binMaterialWidthEnglish"].ToString()).ToString("0.0"));
                    newRow.CreateCell(18).SetCellValue(GetDoubleValue(dr["binMaterialPitchEnglish"].ToString()).ToString("0.0"));
                    newRow.CreateCell(20).SetCellValue(GetDoubleValue(dr["binMaterialThicknessEnglish"].ToString()).ToString("0.000"));
                    newRow.CreateCell(10).SetCellValue(GetDoubleValue(dr["dinSizeFrontToBackMetric"].ToString()).ToString("0.0"));
                    newRow.CreateCell(12).SetCellValue(GetDoubleValue(dr["dinSizeLeftToRightMetric"].ToString()).ToString("0.0"));
                    newRow.CreateCell(14).SetCellValue(GetDoubleValue(dr["dinSizeShutHeightMetric"].ToString()).ToString("0.0"));
                    newRow.CreateCell(15).SetCellValue(GetDoubleValue(dr["dinNumberOfStations"].ToString()).ToString("0.0"));
                    newRow.CreateCell(17).SetCellValue(GetDoubleValue(dr["binMaterialWidthMetric"].ToString()).ToString("0.0"));
                    newRow.CreateCell(19).SetCellValue(GetDoubleValue(dr["binMaterialPitchMetric"].ToString()).ToString("0.0"));
                    newRow.CreateCell(21).SetCellValue(GetDoubleValue(dr["binMaterialThicknessMetric"].ToString()).ToString("0.0##"));
                    newRow.CreateCell(22).SetCellValue(dr["mtyMaterialType"].ToString());
                    newRow.CreateCell(23).SetCellValue("Weeks");
                    newRow.CreateCell(24).SetCellValue(GetDoubleValue(dr["quoLeadTime"].ToString()));
                    SqlCommand sql2 = new SqlCommand();
                    SqlConnection connection2 = new SqlConnection(master.getConnectionString());
                    connection2.Open();
                    sql2.Connection = connection2;

                    sql2.CommandText = "Select pwnPreWordedNote from linkPWNToQuote, pktblPreWordedNote where pwqPreWordedNoteID = pwnPreWordedNoteID and pwqQuoteID = @quoteID ";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@quoteID", dr["quoQuoteID"].ToString());
                    SqlDataReader dr2 = sql2.ExecuteReader();
                    while (dr2.Read())
                    {
                        if (dr2.GetValue(0).ToString().Contains("1") && dr2.GetValue(0).ToString().Contains("2"))
                        {
                            newRow.CreateCell(25).SetCellValue(dr2.GetValue(0).ToString());
                            break;
                        }
                    }
                    dr2.Read();
                    connection2.Close();
                    if (master.getCompanyId() != 8)
                    {
                        cell = (XSSFCell)newRow.CreateCell(25);
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(26);
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(27);
                        cell.SetCellValue(GetDoubleValue(dr["quoTotalAmount"].ToString()));
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(28);
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(29);
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(30);
                        cell.CellStyle = CurrencyStyle;
                        cell = (XSSFCell)newRow.CreateCell(31);
                        cell.CellStyle = CurrencyStyle;
                        total += GetDoubleValue(dr["quoTotalAmount"].ToString());
                    }


                    //String formula = "SUM(AA" + (currentRow + 1).ToString() + ":AF" + (currentRow + 1).ToString() + ")";
                    //cell = (XSSFCell)newRow.CreateCell(32);
                    //cell.SetCellFormula(formula);
                    //cell.CellStyle = CurrencyStyle;
                }
                else if (dr["hquHTSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["dtyFullName"].ToString());
                    newRow.CreateCell(8).SetCellValue(dr["cavCavityName1"].ToString());
                    newRow.CreateCell(22).SetCellValue(dr["hquMaterialType"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["hquLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(System.Convert.ToDouble(dr["htsCost"].ToString()).ToString("0.00"));
                    total += GetDoubleValue(dr["htsCost"].ToString());
                    cell.CellStyle = CurrencyStyle;
                }
                else if (dr["squSTSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["squProcess"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["squLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(System.Convert.ToDouble(dr["stsCost"].ToString()).ToString("0.00"));
                    total += GetDoubleValue(dr["stsCost"].ToString());
                    cell.CellStyle = CurrencyStyle;
                }
                else if (dr["uquUGSQuoteID"].ToString() != "")
                {
                    newRow.CreateCell(7).SetCellValue(dr["dtyFullName1"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["uquLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(System.Convert.ToDouble(dr["uquTotalPrice"].ToString()).ToString("0.00"));
                    total += GetDoubleValue(dr["uquTotalPrice"].ToString());
                    cell.CellStyle = CurrencyStyle;
                }
            }
            dr.Close();


            if (customer != 0 && customer != null && RFQID == 0)
            {
                sql.Parameters.Clear();
                sql.CommandText = "Select ecqECQuoteID, ecqVersion, ecqQuoteNumber, TSGCompanyAbbrev, ecqCustomerRFQNumber, ecqPartNumber, ecqPartName, dtyFullName, cavCavityName, ";
                sql.CommandText += "ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ecqShutHeightMet, ecqNumberOfStations, ecqBlankWidthEng, ecqBlankWidthMet, ";
                sql.CommandText += "ecqBlankPitchEng, ecqBlankPitchMet, ecqMaterialThkEng, ecqMaterialThkMet, mtyMaterialType, ecqLeadTime, (Select sum(pwnCostNote) from ";
                sql.CommandText += "pktblPreWordedNote, linkPWNToECQuote where pwnPreWordedNoteID = peqPreWordedNoteID and peqECQuoteID = ecqECQuoteID) as cost, ecqPicture ";
                sql.CommandText += "from DieType, tblECQuote, pktblCavity, pktblMaterialType, Customer, CustomerLocation, TSGCompany, pktblEstimators ";
                sql.CommandText += "where ecqDieType = DieTypeID and Name <> 'EC' and ";
                sql.CommandText += "ecqCustomer = @customer and ";
                if (plant != 0 && plant != null)
                {
                    sql.CommandText += "ecqCustomerLocation = @plant and ";
                    sql.Parameters.AddWithValue("@plant", plant);
                }
                sql.CommandText += "ecqMaterialType = mtyMaterialTypeID and CustomerLocationID = ecqCustomerLocation and Customer.CustomerID = ecqCustomer and ";
                sql.CommandText += "ecqTSGCompanyID = TSGCompany.TSGCompanyID and cavCavityID = ecqCavity and estEstimatorID = ecqEstimator ";
                if (start != "" && start != null)
                {
                    sql.CommandText += "and ecqCreated >= @start ";
                    sql.Parameters.AddWithValue("@start", start);
                }
                if (end != "" && end != null)
                {
                    sql.CommandText += "and ecqCreated <= @end ";
                    sql.Parameters.AddWithValue("@end", end);
                }
                sql.Parameters.AddWithValue("@customer", customer);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (!HeaderWritten)
                    {
                        var row = sh.CreateRow(0);
                        row.CreateCell(0).SetCellValue("Tooling Systems Group");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row = sh.CreateRow(1);
                        row.CreateCell(0).SetCellValue(dr.GetValue(0).ToString() + " Engineering Estimate");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.GetCell(0).RichStringCellValue.ApplyFont(0, dr.GetValue(0).ToString().Length, blueFont);
                        row = sh.CreateRow(2);
                        // TODO Format as Date
                        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("d"));
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.CreateCell(12).SetCellValue("Shut");
                        row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(13).SetCellValue("Shut");
                        row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(30).SetCellValue("Spare");
                        row.GetCell(30).RichStringCellValue.ApplyFont(titleFont);
                        row = sh.CreateRow(3);
                        row.CreateCell(0);
                        row.CreateCell(1);
                        row.CreateCell(2);
                        row.CreateCell(3).SetCellValue("Customer");
                        row.CreateCell(4).SetCellValue("Due");
                        row.CreateCell(5);
                        row.CreateCell(6);
                        row.CreateCell(7);
                        row.CreateCell(8);
                        row.CreateCell(9).SetCellValue("F to B");
                        row.CreateCell(10).SetCellValue("F to B");
                        row.CreateCell(11).SetCellValue("L to R");
                        row.CreateCell(12).SetCellValue("L to R");
                        row.CreateCell(13).SetCellValue("Height");
                        row.CreateCell(14).SetCellValue("Height");
                        row.CreateCell(15).SetCellValue("Number");
                        row.CreateCell(16).SetCellValue("Width");
                        row.CreateCell(17).SetCellValue("Width");
                        row.CreateCell(18).SetCellValue("Pitch");
                        row.CreateCell(19).SetCellValue("Pitch");
                        row.CreateCell(20).SetCellValue("Thickness");
                        row.CreateCell(21).SetCellValue("Thickness");
                        row.CreateCell(22);
                        row.CreateCell(23).SetCellValue("Lead");
                        row.CreateCell(24).SetCellValue("Lead");
                        row.CreateCell(25);
                        row.CreateCell(26);
                        row.CreateCell(27);
                        row.CreateCell(28);
                        row.CreateCell(29);
                        row.CreateCell(30);
                        row.CreateCell(31).SetCellValue("Pierce, Punches");
                        for (int i = 0; i < 30; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        row = sh.CreateRow(4);
                        row.CreateCell(0).SetCellValue("Picture");
                        row.CreateCell(1).SetCellValue("Quote#");
                        row.CreateCell(2).SetCellValue("Group");
                        row.CreateCell(3).SetCellValue("RFQ Number");
                        row.CreateCell(4).SetCellValue("Date");
                        row.CreateCell(5).SetCellValue("Part Number");
                        row.CreateCell(6).SetCellValue("Description");
                        row.CreateCell(7).SetCellValue("Process");
                        row.CreateCell(8).SetCellValue("Cavity");
                        row.CreateCell(9).SetCellValue("Inch");
                        row.CreateCell(10).SetCellValue("MM");
                        row.CreateCell(11).SetCellValue("Inch");
                        row.CreateCell(12).SetCellValue("MM");
                        row.CreateCell(13).SetCellValue("Inch");
                        row.CreateCell(14).SetCellValue("MM");
                        row.CreateCell(15).SetCellValue("Stations");
                        row.CreateCell(16).SetCellValue("Inch");
                        row.CreateCell(17).SetCellValue("MM");
                        row.CreateCell(18).SetCellValue("Inch");
                        row.CreateCell(19).SetCellValue("MM");
                        row.CreateCell(20).SetCellValue("Inch");
                        row.CreateCell(21).SetCellValue("MM");
                        row.CreateCell(22).SetCellValue("Material Type");
                        row.CreateCell(23).SetCellValue("Time");
                        row.CreateCell(24).SetCellValue("Time");
                        row.CreateCell(25).SetCellValue("Target");
                        row.CreateCell(26).SetCellValue("Blank Die");
                        row.CreateCell(27).SetCellValue("Die");
                        row.CreateCell(28).SetCellValue("Check Fixture");
                        row.CreateCell(29).SetCellValue("Shipping");
                        row.CreateCell(30).SetCellValue("Home Line");
                        row.CreateCell(31).SetCellValue("and Buttons");
                        row.CreateCell(32).SetCellValue("Total");
                        for (int i = 0; i < 33; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        HeaderWritten = true;
                        currentRow = 4;
                    }
                    currentRow++;
                    var newRow = sh.CreateRow(currentRow);
                    // This is in points which is whatever excel reports times 20 
                    newRow.Height = 1500;
                    newRow.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                    String sharepointLibrary = "Part Pictures";
                    byte[] pictureData;
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["ecqPicture"].ToString());
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
                            // This anchor type will change the picture size as the cell changes size
                            // not using it currently seems to force the picture to overlap the next column
                            anchor.AnchorType = 0;
                            // this anchor type will not resize picture with cell
                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);

                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.4);
                        }
                        catch
                        {

                        }
                    }

                    if (dr["ecqQuoteNumber"].ToString() != "")
                    {
                        newRow.CreateCell(1).SetCellValue(dr["ecqQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"] + "-SA-" + dr["ecqVersion"].ToString());
                    }
                    else
                    {
                        newRow.CreateCell(1).SetCellValue(dr["ecqECQuoteID"].ToString() + "-" + dr["TSGCompanyAbbrev"] + "-SA-" + dr["ecqVersion"].ToString());
                    }
                    newRow.CreateCell(2).SetCellValue(dr["TSGCompanyAbbrev"].ToString());

                    newRow.CreateCell(3).SetCellValue(dr["ecqCustomerRFQNumber"].ToString());
                    newRow.CreateCell(5).SetCellValue(dr["ecqPartNumber"].ToString());
                    newRow.CreateCell(6).SetCellValue(dr["ecqPartName"].ToString());
                    newRow.CreateCell(7).SetCellValue(dr["dtyFullName"].ToString());
                    newRow.CreateCell(8).SetCellValue(dr["cavCavityName"].ToString());
                    newRow.CreateCell(9).SetCellValue(GetDoubleValue(dr["ecqDieFBEng"].ToString()).ToString("0.0"));
                    newRow.CreateCell(10).SetCellValue(GetDoubleValue(dr["ecqDieFBMet"].ToString()).ToString("0.0"));
                    newRow.CreateCell(11).SetCellValue(GetDoubleValue(dr["ecqDieLREng"].ToString()).ToString("0.0"));
                    newRow.CreateCell(12).SetCellValue(GetDoubleValue(dr["ecqDieLRMet"].ToString()).ToString("0.0"));
                    newRow.CreateCell(13).SetCellValue(GetDoubleValue(dr["ecqShutHeightEng"].ToString()).ToString("0.0"));
                    newRow.CreateCell(14).SetCellValue(GetDoubleValue(dr["ecqShutHeightMet"].ToString()).ToString("0.0"));
                    newRow.CreateCell(15).SetCellValue(GetDoubleValue(dr["ecqNumberOfStations"].ToString()).ToString("0.0"));
                    newRow.CreateCell(16).SetCellValue(GetDoubleValue(dr["ecqBlankWidthEng"].ToString()).ToString("0.0"));
                    newRow.CreateCell(17).SetCellValue(GetDoubleValue(dr["ecqBlankWidthMet"].ToString()).ToString("0.0"));
                    newRow.CreateCell(18).SetCellValue(GetDoubleValue(dr["ecqBlankPitchEng"].ToString()).ToString("0.0"));
                    newRow.CreateCell(19).SetCellValue(GetDoubleValue(dr["ecqBlankPitchMet"].ToString()).ToString("0.0"));
                    newRow.CreateCell(20).SetCellValue(GetDoubleValue(dr["ecqMaterialThkEng"].ToString()).ToString("0.000"));
                    newRow.CreateCell(21).SetCellValue(GetDoubleValue(dr["ecqMaterialThkMet"].ToString()).ToString("0.0##"));
                    newRow.CreateCell(22).SetCellValue(dr["mtyMaterialType"].ToString());
                    //newRow.CreateCell(23).SetCellValue("Weeks");
                    newRow.CreateCell(24).SetCellValue(GetDoubleValue(dr["ecqLeadTime"].ToString()));
                    cell = (XSSFCell)newRow.CreateCell(25);
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(26);
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(27);
                    cell.SetCellValue(GetDoubleValue(dr["cost"].ToString()).ToString("0.00"));
                    total += GetDoubleValue(dr["cost"].ToString());
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(28);
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(29);
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(30);
                    cell.CellStyle = CurrencyStyle;
                    cell = (XSSFCell)newRow.CreateCell(31);
                    cell.CellStyle = CurrencyStyle;

                    String formula = "SUM(AA" + (currentRow + 1).ToString() + ":AF" + (currentRow + 1).ToString() + ")";
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellFormula(formula);
                    cell.CellStyle = CurrencyStyle;
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select hquHTSQuoteID, hquVersion, dtyFullName, cavCavityName, hquMaterialType, hquLeadTime, (Select SUM(hpwQuantity * hpwUnitPrice) from ";
                sql.CommandText += "linkHTSPWNToHTSQuote, pktblHTSPreWordedNote where hquHTSQuoteID = pthHTSQuoteID and hpwHTSPreWordedNoteID = pthHTSPWNID) as htsCost, ";
                sql.CommandText += "hquPicture, hquNumber, hquCustomerQuoteNumber, hquPartNumbers, hquPartName ";
                sql.CommandText += "from tblHTSQuote, Customer, CustomerLocation, DieType, pktblCavity ";
                sql.CommandText += "where hquCustomerID = Customer.CustomerID and hquCustomerLocationID = CustomerLocationID and DieTypeID = hquProcess ";
                sql.CommandText += "and cavCavityID = hquCavity and(select distinct 1 from linkQuoteToRFQ where qtrHTS = 1 and qtrQuoteID = hquHTSQuoteID) is NULL ";
                sql.CommandText += "and hquCustomerID = @customer ";
                sql.Parameters.AddWithValue("@customer", customer);
                if (plant != 0 && plant != null)
                {
                    sql.CommandText += "and hquCustomerLocationID = @plant ";
                    sql.Parameters.AddWithValue("@plant", plant);
                }
                if (start != "" && start != null)
                {
                    sql.CommandText += "and hquCreated >= @start ";
                    sql.Parameters.AddWithValue("@start", start);
                }
                if (end != "" && end != null)
                {
                    sql.CommandText += "and hquCreated <= @end ";
                    sql.Parameters.AddWithValue("@end", end);
                }
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (!HeaderWritten)
                    {
                        var row = sh.CreateRow(0);
                        row.CreateCell(0).SetCellValue("Tooling Systems Group");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row = sh.CreateRow(1);
                        row.CreateCell(0).SetCellValue(dr.GetValue(0).ToString() + " Engineering Estimate");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.GetCell(0).RichStringCellValue.ApplyFont(0, dr.GetValue(0).ToString().Length, blueFont);
                        row = sh.CreateRow(2);
                        // TODO Format as Date
                        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("d"));
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.CreateCell(12).SetCellValue("Shut");
                        row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(13).SetCellValue("Shut");
                        row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(30).SetCellValue("Spare");
                        row.GetCell(30).RichStringCellValue.ApplyFont(titleFont);
                        row = sh.CreateRow(3);
                        row.CreateCell(0);
                        row.CreateCell(1);
                        row.CreateCell(2);
                        row.CreateCell(3).SetCellValue("Customer");
                        row.CreateCell(4).SetCellValue("Due");
                        row.CreateCell(5);
                        row.CreateCell(6);
                        row.CreateCell(7);
                        row.CreateCell(8);
                        row.CreateCell(9).SetCellValue("F to B");
                        row.CreateCell(10).SetCellValue("F to B");
                        row.CreateCell(11).SetCellValue("L to R");
                        row.CreateCell(12).SetCellValue("L to R");
                        row.CreateCell(13).SetCellValue("Height");
                        row.CreateCell(14).SetCellValue("Height");
                        row.CreateCell(15).SetCellValue("Number");
                        row.CreateCell(16).SetCellValue("Width");
                        row.CreateCell(17).SetCellValue("Width");
                        row.CreateCell(18).SetCellValue("Pitch");
                        row.CreateCell(19).SetCellValue("Pitch");
                        row.CreateCell(20).SetCellValue("Thickness");
                        row.CreateCell(21).SetCellValue("Thickness");
                        row.CreateCell(22);
                        row.CreateCell(23).SetCellValue("Lead");
                        row.CreateCell(24).SetCellValue("Lead");
                        row.CreateCell(25);
                        row.CreateCell(26);
                        row.CreateCell(27);
                        row.CreateCell(28);
                        row.CreateCell(29);
                        row.CreateCell(30);
                        row.CreateCell(31).SetCellValue("Pierce, Punches");
                        for (int i = 0; i < 30; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        row = sh.CreateRow(4);
                        row.CreateCell(0).SetCellValue("Picture");
                        row.CreateCell(1).SetCellValue("Quote#");
                        row.CreateCell(2).SetCellValue("Group");
                        row.CreateCell(3).SetCellValue("RFQ Number");
                        row.CreateCell(4).SetCellValue("Date");
                        row.CreateCell(5).SetCellValue("Part Number");
                        row.CreateCell(6).SetCellValue("Description");
                        row.CreateCell(7).SetCellValue("Process");
                        row.CreateCell(8).SetCellValue("Cavity");
                        row.CreateCell(9).SetCellValue("Inch");
                        row.CreateCell(10).SetCellValue("MM");
                        row.CreateCell(11).SetCellValue("Inch");
                        row.CreateCell(12).SetCellValue("MM");
                        row.CreateCell(13).SetCellValue("Inch");
                        row.CreateCell(14).SetCellValue("MM");
                        row.CreateCell(15).SetCellValue("Stations");
                        row.CreateCell(16).SetCellValue("Inch");
                        row.CreateCell(17).SetCellValue("MM");
                        row.CreateCell(18).SetCellValue("Inch");
                        row.CreateCell(19).SetCellValue("MM");
                        row.CreateCell(20).SetCellValue("Inch");
                        row.CreateCell(21).SetCellValue("MM");
                        row.CreateCell(22).SetCellValue("Material Type");
                        row.CreateCell(23).SetCellValue("Time");
                        row.CreateCell(24).SetCellValue("Time");
                        row.CreateCell(25).SetCellValue("Target");
                        row.CreateCell(26).SetCellValue("Blank Die");
                        row.CreateCell(27).SetCellValue("Die");
                        row.CreateCell(28).SetCellValue("Check Fixture");
                        row.CreateCell(29).SetCellValue("Shipping");
                        row.CreateCell(30).SetCellValue("Home Line");
                        row.CreateCell(31).SetCellValue("and Buttons");
                        row.CreateCell(32).SetCellValue("Total");
                        for (int i = 0; i < 33; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        HeaderWritten = true;
                        currentRow = 4;
                    }
                    currentRow++;
                    var newRow = sh.CreateRow(currentRow);
                    // This is in points which is whatever excel reports times 20 
                    newRow.Height = 1500;
                    newRow.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                    String sharepointLibrary = "Part Pictures";
                    byte[] pictureData;
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["hquPicture"].ToString());
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
                            // This anchor type will change the picture size as the cell changes size
                            // not using it currently seems to force the picture to overlap the next column
                            anchor.AnchorType = 0;
                            // this anchor type will not resize picture with cell
                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);

                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.4);
                        }
                        catch
                        {

                        }
                    }

                    if (dr["hquNumber"].ToString() != "")
                    {
                        newRow.CreateCell(1).SetCellValue(dr["hquNumber"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString());
                    }
                    else
                    {
                        newRow.CreateCell(1).SetCellValue(dr["hquHTSQuoteID"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString());
                    }
                    newRow.CreateCell(2).SetCellValue("HTS");

                    newRow.CreateCell(3).SetCellValue(dr["hquCustomerQuoteNumber"].ToString());

                    newRow.CreateCell(5).SetCellValue(dr["hquPartNumbers"].ToString());
                    newRow.CreateCell(6).SetCellValue(dr["hquPartName"].ToString());

                    newRow.CreateCell(7).SetCellValue(dr["dtyFullName"].ToString());
                    newRow.CreateCell(8).SetCellValue(dr["cavCavityName"].ToString());
                    newRow.CreateCell(22).SetCellValue(dr["hquMaterialType"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["hquLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(GetDoubleValue(dr["htsCost"].ToString()).ToString("#,###,##0.00"));
                    total += GetDoubleValue(dr["htsCost"].ToString());
                    //cell.SetCellValue(System.Convert.ToDouble(dr["htsCost"].ToString()).ToString("0.00"));
                    cell.CellStyle = CurrencyStyle;
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select squSTSQuoteID, squQuoteVersion, squQuoteNumber, squPartNumber, squPartName, squCustomerRFQNum, squProcess, squLeadTime, squPicture, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToSTSQuote, pktblPreWordedNote where squSTSQuoteID = psqSTSQuoteID and pwnPreWordedNoteID = psqPreWordedNoteID) as stsCost ";
                sql.CommandText += "from tblSTSQuote, Customer, CustomerLocation ";
                sql.CommandText += "where squCustomerID = Customer.CustomerID and squPlantID = CustomerLocationID and ";
                sql.CommandText += "(select distinct 1 from linkQuoteToRFQ where qtrSTS = 1 and qtrQuoteID = squSTSQuoteID) is null ";
                sql.CommandText += "and squCustomerID = @customer ";
                sql.Parameters.AddWithValue("@customer", customer);
                if (plant != 0 && plant != null)
                {
                    sql.CommandText += "and squPlantID = @plant ";
                    sql.Parameters.AddWithValue("@plant", plant);
                }
                if (start != "" && start != null)
                {
                    sql.CommandText += "and squCreated >= @start ";
                    sql.Parameters.AddWithValue("@start", start);
                }
                if (end != "" && end != null)
                {
                    sql.CommandText += "and squCreated <= @end ";
                    sql.Parameters.AddWithValue("@end", end);
                }
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (!HeaderWritten)
                    {
                        var row = sh.CreateRow(0);
                        row.CreateCell(0).SetCellValue("Tooling Systems Group");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row = sh.CreateRow(1);
                        row.CreateCell(0).SetCellValue(dr.GetValue(0).ToString() + " Engineering Estimate");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.GetCell(0).RichStringCellValue.ApplyFont(0, dr.GetValue(0).ToString().Length, blueFont);
                        row = sh.CreateRow(2);
                        // TODO Format as Date
                        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("d"));
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.CreateCell(12).SetCellValue("Shut");
                        row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(13).SetCellValue("Shut");
                        row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(30).SetCellValue("Spare");
                        row.GetCell(30).RichStringCellValue.ApplyFont(titleFont);
                        row = sh.CreateRow(3);
                        row.CreateCell(0);
                        row.CreateCell(1);
                        row.CreateCell(2);
                        row.CreateCell(3).SetCellValue("Customer");
                        row.CreateCell(4).SetCellValue("Due");
                        row.CreateCell(5);
                        row.CreateCell(6);
                        row.CreateCell(7);
                        row.CreateCell(8);
                        row.CreateCell(9).SetCellValue("F to B");
                        row.CreateCell(10).SetCellValue("F to B");
                        row.CreateCell(11).SetCellValue("L to R");
                        row.CreateCell(12).SetCellValue("L to R");
                        row.CreateCell(13).SetCellValue("Height");
                        row.CreateCell(14).SetCellValue("Height");
                        row.CreateCell(15).SetCellValue("Number");
                        row.CreateCell(16).SetCellValue("Width");
                        row.CreateCell(17).SetCellValue("Width");
                        row.CreateCell(18).SetCellValue("Pitch");
                        row.CreateCell(19).SetCellValue("Pitch");
                        row.CreateCell(20).SetCellValue("Thickness");
                        row.CreateCell(21).SetCellValue("Thickness");
                        row.CreateCell(22);
                        row.CreateCell(23).SetCellValue("Lead");
                        row.CreateCell(24).SetCellValue("Lead");
                        row.CreateCell(25);
                        row.CreateCell(26);
                        row.CreateCell(27);
                        row.CreateCell(28);
                        row.CreateCell(29);
                        row.CreateCell(30);
                        row.CreateCell(31).SetCellValue("Pierce, Punches");
                        for (int i = 0; i < 30; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        row = sh.CreateRow(4);
                        row.CreateCell(0).SetCellValue("Picture");
                        row.CreateCell(1).SetCellValue("Quote#");
                        row.CreateCell(2).SetCellValue("Group");
                        row.CreateCell(3).SetCellValue("RFQ Number");
                        row.CreateCell(4).SetCellValue("Date");
                        row.CreateCell(5).SetCellValue("Part Number");
                        row.CreateCell(6).SetCellValue("Description");
                        row.CreateCell(7).SetCellValue("Process");
                        row.CreateCell(8).SetCellValue("Cavity");
                        row.CreateCell(9).SetCellValue("Inch");
                        row.CreateCell(10).SetCellValue("MM");
                        row.CreateCell(11).SetCellValue("Inch");
                        row.CreateCell(12).SetCellValue("MM");
                        row.CreateCell(13).SetCellValue("Inch");
                        row.CreateCell(14).SetCellValue("MM");
                        row.CreateCell(15).SetCellValue("Stations");
                        row.CreateCell(16).SetCellValue("Inch");
                        row.CreateCell(17).SetCellValue("MM");
                        row.CreateCell(18).SetCellValue("Inch");
                        row.CreateCell(19).SetCellValue("MM");
                        row.CreateCell(20).SetCellValue("Inch");
                        row.CreateCell(21).SetCellValue("MM");
                        row.CreateCell(22).SetCellValue("Material Type");
                        row.CreateCell(23).SetCellValue("Time");
                        row.CreateCell(24).SetCellValue("Time");
                        row.CreateCell(25).SetCellValue("Target");
                        row.CreateCell(26).SetCellValue("Blank Die");
                        row.CreateCell(27).SetCellValue("Die");
                        row.CreateCell(28).SetCellValue("Check Fixture");
                        row.CreateCell(29).SetCellValue("Shipping");
                        row.CreateCell(30).SetCellValue("Home Line");
                        row.CreateCell(31).SetCellValue("and Buttons");
                        row.CreateCell(32).SetCellValue("Total");
                        for (int i = 0; i < 33; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        HeaderWritten = true;
                        currentRow = 4;
                    }
                    currentRow++;
                    var newRow = sh.CreateRow(currentRow);
                    // This is in points which is whatever excel reports times 20 
                    newRow.Height = 1500;
                    newRow.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                    String sharepointLibrary = "Part Pictures";
                    byte[] pictureData;
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["squPicture"].ToString());
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
                            // This anchor type will change the picture size as the cell changes size
                            // not using it currently seems to force the picture to overlap the next column
                            anchor.AnchorType = 0;
                            // this anchor type will not resize picture with cell
                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);

                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.4);
                        }
                        catch
                        {

                        }
                    }

                    if (dr["squQuoteNumber"].ToString() != "")
                    {
                        newRow.CreateCell(1).SetCellValue(dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString());
                    }
                    else
                    {
                        newRow.CreateCell(1).SetCellValue(dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString());
                    }
                    newRow.CreateCell(2).SetCellValue("STS");

                    newRow.CreateCell(3).SetCellValue(dr["squCustomerRFQNum"].ToString());

                    newRow.CreateCell(5).SetCellValue(dr["squPartNumber"].ToString());
                    newRow.CreateCell(6).SetCellValue(dr["squPartName"].ToString());

                    newRow.CreateCell(7).SetCellValue(dr["squProcess"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["squLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(GetDoubleValue(dr["stsCost"].ToString()).ToString("#,###,##0.00"));
                    total += GetDoubleValue(dr["stsCost"].ToString());
                    //cell.SetCellValue(System.Convert.ToDouble(dr["stsCost"].ToString()).ToString("0.00"));
                    cell.CellStyle = CurrencyStyle;
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select uquUGSQuoteID, uquQuoteVersion, uquQuoteNumber, uquPartNumber, uquPartName, uquCustomerRFQNumber, ";
                sql.CommandText += "dtyFullName, uquLeadTime, uquTotalPrice, uquPicture ";
                sql.CommandText += "from tblUGSQuote, Customer, CustomerLocation, DieType ";
                sql.CommandText += "where uquCustomerID = Customer.CustomerID and uquPlantID = CustomerLocationID and DieTypeID = uquDieType ";
                sql.CommandText += "and uquCustomerID = @customer ";
                sql.Parameters.AddWithValue("@customer", customer);
                if (plant != 0 && plant != null)
                {
                    sql.CommandText += "and uquPlantID = @plant ";
                    sql.Parameters.AddWithValue("@plant", plant);
                }
                if (start != "" && start != null)
                {
                    sql.CommandText += "and uquCreated >= @start ";
                    sql.Parameters.AddWithValue("@start", start);
                }
                if (end != "" && end != null)
                {
                    sql.CommandText += "and uquCreated <= @end ";
                    sql.Parameters.AddWithValue("@end", end);
                }
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (!HeaderWritten)
                    {
                        var row = sh.CreateRow(0);
                        row.CreateCell(0).SetCellValue("Tooling Systems Group");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row = sh.CreateRow(1);
                        row.CreateCell(0).SetCellValue(dr.GetValue(0).ToString() + " Engineering Estimate");
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.GetCell(0).RichStringCellValue.ApplyFont(0, dr.GetValue(0).ToString().Length, blueFont);
                        row = sh.CreateRow(2);
                        // TODO Format as Date
                        row.CreateCell(0).SetCellValue(DateTime.Now.ToString("d"));
                        row.GetCell(0).RichStringCellValue.ApplyFont(headerFont);
                        row.CreateCell(12).SetCellValue("Shut");
                        row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(13).SetCellValue("Shut");
                        row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                        row.CreateCell(30).SetCellValue("Spare");
                        row.GetCell(30).RichStringCellValue.ApplyFont(titleFont);
                        row = sh.CreateRow(3);
                        row.CreateCell(0);
                        row.CreateCell(1);
                        row.CreateCell(2);
                        row.CreateCell(3).SetCellValue("Customer");
                        row.CreateCell(4).SetCellValue("Due");
                        row.CreateCell(5);
                        row.CreateCell(6);
                        row.CreateCell(7);
                        row.CreateCell(8);
                        row.CreateCell(9).SetCellValue("F to B");
                        row.CreateCell(10).SetCellValue("F to B");
                        row.CreateCell(11).SetCellValue("L to R");
                        row.CreateCell(12).SetCellValue("L to R");
                        row.CreateCell(13).SetCellValue("Height");
                        row.CreateCell(14).SetCellValue("Height");
                        row.CreateCell(15).SetCellValue("Number");
                        row.CreateCell(16).SetCellValue("Width");
                        row.CreateCell(17).SetCellValue("Width");
                        row.CreateCell(18).SetCellValue("Pitch");
                        row.CreateCell(19).SetCellValue("Pitch");
                        row.CreateCell(20).SetCellValue("Thickness");
                        row.CreateCell(21).SetCellValue("Thickness");
                        row.CreateCell(22);
                        row.CreateCell(23).SetCellValue("Lead");
                        row.CreateCell(24).SetCellValue("Lead");
                        row.CreateCell(25);
                        row.CreateCell(26);
                        row.CreateCell(27);
                        row.CreateCell(28);
                        row.CreateCell(29);
                        row.CreateCell(30);
                        row.CreateCell(31).SetCellValue("Pierce, Punches");
                        for (int i = 0; i < 30; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        row = sh.CreateRow(4);
                        row.CreateCell(0).SetCellValue("Picture");
                        row.CreateCell(1).SetCellValue("Quote#");
                        row.CreateCell(2).SetCellValue("Group");
                        row.CreateCell(3).SetCellValue("RFQ Number");
                        row.CreateCell(4).SetCellValue("Date");
                        row.CreateCell(5).SetCellValue("Part Number");
                        row.CreateCell(6).SetCellValue("Description");
                        row.CreateCell(7).SetCellValue("Process");
                        row.CreateCell(8).SetCellValue("Cavity");
                        row.CreateCell(9).SetCellValue("Inch");
                        row.CreateCell(10).SetCellValue("MM");
                        row.CreateCell(11).SetCellValue("Inch");
                        row.CreateCell(12).SetCellValue("MM");
                        row.CreateCell(13).SetCellValue("Inch");
                        row.CreateCell(14).SetCellValue("MM");
                        row.CreateCell(15).SetCellValue("Stations");
                        row.CreateCell(16).SetCellValue("Inch");
                        row.CreateCell(17).SetCellValue("MM");
                        row.CreateCell(18).SetCellValue("Inch");
                        row.CreateCell(19).SetCellValue("MM");
                        row.CreateCell(20).SetCellValue("Inch");
                        row.CreateCell(21).SetCellValue("MM");
                        row.CreateCell(22).SetCellValue("Material Type");
                        row.CreateCell(23).SetCellValue("Time");
                        row.CreateCell(24).SetCellValue("Time");
                        row.CreateCell(25).SetCellValue("Target");
                        row.CreateCell(26).SetCellValue("Blank Die");
                        row.CreateCell(27).SetCellValue("Die");
                        row.CreateCell(28).SetCellValue("Check Fixture");
                        row.CreateCell(29).SetCellValue("Shipping");
                        row.CreateCell(30).SetCellValue("Home Line");
                        row.CreateCell(31).SetCellValue("and Buttons");
                        row.CreateCell(32).SetCellValue("Total");
                        for (int i = 0; i < 33; i++)
                        {
                            row.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                        }
                        HeaderWritten = true;
                        currentRow = 4;
                    }
                    currentRow++;
                    var newRow = sh.CreateRow(currentRow);
                    // This is in points which is whatever excel reports times 20 
                    newRow.Height = 1500;
                    newRow.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                    String sharepointLibrary = "Part Pictures";
                    byte[] pictureData;
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["uquPicture"].ToString());
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
                            // This anchor type will change the picture size as the cell changes size
                            // not using it currently seems to force the picture to overlap the next column
                            anchor.AnchorType = 0;
                            // this anchor type will not resize picture with cell
                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);

                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.4);
                        }
                        catch
                        {

                        }
                    }

                    if (dr["uquQuoteNumber"].ToString() != "")
                    {
                        newRow.CreateCell(1).SetCellValue(dr["uquQuoteNumber"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString());
                    }
                    else
                    {
                        newRow.CreateCell(1).SetCellValue(dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString());
                    }
                    newRow.CreateCell(2).SetCellValue("UGS");

                    newRow.CreateCell(3).SetCellValue(dr["uquCustomerRFQNumber"].ToString());

                    newRow.CreateCell(5).SetCellValue(dr["uquPartNumber"].ToString());
                    newRow.CreateCell(6).SetCellValue(dr["uquPartName"].ToString());

                    newRow.CreateCell(7).SetCellValue(dr["dtyFullName"].ToString());
                    newRow.CreateCell(24).SetCellValue(dr["uquLeadTime"].ToString());
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellValue(GetDoubleValue(dr["uquTotalPrice"].ToString()).ToString("#,###,##0.00"));
                    total += GetDoubleValue(dr["uquTotalPrice"].ToString());
                    //cell.SetCellValue(System.Convert.ToDouble(dr["uquTotalPrice"].ToString()).ToString("0.00"));
                    cell.CellStyle = CurrencyStyle;
                }
                dr.Close();
            }







            if (HeaderWritten)
            {
                // create grand total row
                currentRow++;
                var newRow = sh.CreateRow(currentRow);
                if (master.getCompanyId() != 8)
                {
                    newRow.CreateCell(31).SetCellValue("Totals");
                    String formula = "SUM(AG6:AG" + (currentRow).ToString() + ")";
                    cell = (XSSFCell)newRow.CreateCell(32);
                    cell.SetCellFormula(formula);
                    cell.CellStyle = CurrencyStyle;
                    cell.SetCellValue(total.ToString("#,###,###,##0.00"));
                }


                // I have no idea what these units are but this makes it the right width
                //sh.SetColumnWidth(0, 10000);
                //int i = 0;
                //while (i < 31)
                //{
                //    sh.AutoSizeColumn(i);
                //    i++;
                //}
                //// I have no idea what these units are but this makes it the right width
                //sh.SetColumnWidth(31, 4000);

                sh.CreateFreezePane(1, 5);
                sh.ForceFormulaRecalculation = true;
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                if (RFQID != 0)
                {
                    context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "RFQSummary-" + RFQID + ".xlsx"));
                }
                else
                {
                    context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Customer Summary.xlsx"));
                }
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);

                string plantName = "";
                if (customer != 0 && customer != null)
                {
                    SmtpClient server = new SmtpClient("smtp.office365.com");
                    server.UseDefaultCredentials = false;
                    server.Port = 587;
                    server.EnableSsl = true;
                    // TODO send as another user
                    server.Credentials = master.getNetworkCredentials();
                    server.Timeout = 50000;
                    server.TargetName = "STARTTLS/smtp.office365.com";
                    MailMessage mail = new MailMessage();
                    System.IO.MemoryStream ms2 = new System.IO.MemoryStream(ms.ToArray());

                    string customerName = "";
                    try
                    {
                        sql.CommandText = "Select CustomerName from Customer where CustomerID = @id";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", customer);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            customerName = dr.GetValue(0).ToString();
                        }
                        dr.Close();

                        if (plant != 0)
                        {
                            sql.CommandText = "Select ShipToName from CustomerLocation where CustomerLocationID = @plant ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@plant", plant);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                plantName = dr["ShipToName"].ToString();
                            }
                            dr.Close();
                        }
                    }
                    catch
                    {

                    }



                    if (plantName != "")
                    {
                        mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, "Customer Summary " + customerName + " " + plantName + ".xlsx"));
                    }
                    else
                    {
                        mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, "Customer Summary " + customerName + ".xlsx"));
                    }
                    //mail.Attachments.Add(System.Net.Mail.Attachment(ms, "RFQ-QUOTE" + StartDate.ToString("d").Replace("/", "-") + " to " + EndDate.ToString("d").Replace("/", "-") + ".xlsx"));


                    mail.From = master.getFromAddress();
                    if (master.getUserName() == "chris@netinflux.com")
                    {
                        mail.To.Add(new MailAddress("rmumford@toolingsystemsgroup.com"));
                    }
                    else
                    {
                        mail.To.Add(new MailAddress(master.getUserName()));
                    }
                    if (plantName != "")
                    {
                        mail.Subject = "Customer Report " + customerName + " " + plantName;
                    }
                    else
                    {
                        mail.Subject = "Customer Report " + customerName;
                    }
                    mail.Body = "Attached is the customer report you requested.<br />";
                    //mail.Body += "Please visit https://tsgrfq.azurewebsites.net/Reporting to view any of the graphs.<br />It will take around 15 seconds to load the webpage.";
                    //mail.Attachments.Add(attach);
                    mail.IsBodyHtml = true;
                    try
                    {
                        server.Send(mail);
                    }
                    catch (Exception err)
                    {

                    }
                }
                else
                {
                    context.Response.BinaryWrite(ms.ToArray());
                    context.Response.End();
                }
            }
            else
            {
                context.Response.Write("File Not Created. The most likely cause is that there are no quotes for this RFQ yet.");
            }
            connection.Close();
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