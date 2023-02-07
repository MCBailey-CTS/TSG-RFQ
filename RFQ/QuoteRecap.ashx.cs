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
using System.Net.Mail;
using System.Data.SqlClient;
using System.Xml.Linq;
using NPOI.SS.UserModel;

namespace RFQ
{
    /// <summary>
    /// Creates an Excel File with all quotes for this date range
    /// There are 2 tabs - Estimates and OEM
    /// </summary>
    public class QuoteRecap : IHttpHandler
    {
        public void testSendWeeklyRecap(HttpContext context)
        {

            return;

            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            List<string> salesmenID = new List<string>();

            sql.CommandText = "select TSGSalesmanID from TSGSalesman where tsaActive = 1";
            SqlDataReader dr = sql.ExecuteReader();

            while (dr.Read())
            {
                salesmenID.Add(Convert.ToString(dr.GetValue(0)));
            }
            dr.Close();
            salesmenID.Clear();
            salesmenID.Add("1");
            for (int i = 0; i < salesmenID.Count; i++)
            {

                XSSFWorkbook wb = new XSSFWorkbook();
                XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
                XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Weekly Recap");
                XSSFSheet sh1 = (XSSFSheet)wb.CreateSheet("Weekly Reserved");
                XSSFSheet sh2 = (XSSFSheet)wb.CreateSheet("Unreserved");
                XSSFSheet sh3 = (XSSFSheet)wb.CreateSheet("Parts no longer reserved");


                NPOI.SS.UserModel.IRow row;

                XSSFFont titleFont = (XSSFFont)wb.CreateFont();
                titleFont.FontHeight = 12;
                titleFont.Boldweight = 700;
                titleFont.IsItalic = true;

                XSSFCellStyle CenterStyle = (XSSFCellStyle)wb.CreateCellStyle();
                CenterStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

                XSSFCellStyle RightStyle = (XSSFCellStyle)wb.CreateCellStyle();
                RightStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

                XSSFCellStyle LateStyle = (XSSFCellStyle)wb.CreateCellStyle();
                LateStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
                LateStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Yellow.Index;
                LateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

                XSSFCellStyle ReallyLateStyle = (XSSFCellStyle)wb.CreateCellStyle();
                ReallyLateStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
                ReallyLateStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Red.Index;
                ReallyLateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

                XSSFCellStyle OnTimeStyle = (XSSFCellStyle)wb.CreateCellStyle();
                OnTimeStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
                OnTimeStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Green.Index;
                OnTimeStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

                XSSFCellStyle DayStyle = (XSSFCellStyle)wb.CreateCellStyle();

                XSSFFont LinkFont = (XSSFFont)wb.CreateFont();
                LinkFont.Underline = NPOI.SS.UserModel.FontUnderlineType.Single;
                XSSFColor LinkColor = new XSSFColor();
                byte[] Blue = { 0, 0, 128 };
                LinkColor.SetRgb(Blue);
                LinkFont.SetColor(LinkColor);

                int currentRow = 0;
                row = GetOrCreateRow(sh, currentRow);
                row.CreateCell(0).SetCellValue("Picture");
                row.GetCell(0).CellStyle = CenterStyle;
                row.GetCell(0).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(1).SetCellValue("QuoteNumber");
                row.GetCell(1).CellStyle = CenterStyle;
                row.GetCell(1).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(2).SetCellValue("Customer RFQ #");
                row.GetCell(2).CellStyle = CenterStyle;
                row.GetCell(2).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(3).SetCellValue("Customer");
                row.GetCell(3).CellStyle = CenterStyle;
                row.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(4).SetCellValue("Customer Location");
                row.GetCell(4).CellStyle = CenterStyle;
                row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(5).SetCellValue("Due Date");
                row.GetCell(5).CellStyle = CenterStyle;
                row.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(6).SetCellValue("Part Number");
                row.GetCell(6).CellStyle = CenterStyle;
                row.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(7).SetCellValue("Part Description");
                row.GetCell(7).CellStyle = CenterStyle;
                row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(8).SetCellValue("Die Type");
                row.GetCell(8).CellStyle = CenterStyle;
                row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(9).SetCellValue("Cavity");
                row.GetCell(9).CellStyle = CenterStyle;
                row.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(10).SetCellValue("F-B (in)");
                row.GetCell(10).CellStyle = CenterStyle;
                row.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(11).SetCellValue("L-R (in)");
                row.GetCell(11).CellStyle = CenterStyle;
                row.GetCell(11).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(12).SetCellValue("Shut Height");
                row.GetCell(12).CellStyle = CenterStyle;
                row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(13).SetCellValue("Stations #");
                row.GetCell(13).CellStyle = CenterStyle;
                row.GetCell(13).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(14).SetCellValue("Part Width");
                row.GetCell(14).CellStyle = CenterStyle;
                row.GetCell(14).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(15).SetCellValue("Part Pitch");
                row.GetCell(15).CellStyle = CenterStyle;
                row.GetCell(15).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(16).SetCellValue("Thickness");
                row.GetCell(16).CellStyle = CenterStyle;
                row.GetCell(16).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(17).SetCellValue("Lead Time");
                row.GetCell(17).CellStyle = CenterStyle;
                row.GetCell(17).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(18).SetCellValue("Total Cost");
                row.GetCell(18).CellStyle = CenterStyle;
                row.GetCell(18).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(19).SetCellValue("Salesman");
                row.GetCell(19).CellStyle = CenterStyle;
                row.GetCell(19).RichStringCellValue.ApplyFont(titleFont);

                sh.SetColumnWidth(0, 5000);
                sh.SetColumnWidth(1, 4000);
                sh.SetColumnWidth(2, 10000);
                sh.SetColumnWidth(3, 6500);
                sh.SetColumnWidth(4, 6500);
                sh.SetColumnWidth(5, 4000);
                sh.SetColumnWidth(6, 6500);
                sh.SetColumnWidth(7, 6500);
                sh.SetColumnWidth(8, 4500);
                sh.SetColumnWidth(9, 4500);
                sh.SetColumnWidth(10, 4500);
                sh.SetColumnWidth(11, 3000);
                sh.SetColumnWidth(12, 3000);
                sh.SetColumnWidth(13, 3000);
                sh.SetColumnWidth(14, 3000);
                sh.SetColumnWidth(15, 3000);
                sh.SetColumnWidth(16, 3000);
                sh.SetColumnWidth(17, 3000);
                sh.SetColumnWidth(18, 5500);
                sh.SetColumnWidth(19, 5500);

                currentRow += 2;



                sql.CommandText = "Select q.quoQuoteID, (Select top 1 prtPicture from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = q.quoQuoteId and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 order by prtRFQLineNumber) as picture, ";
                sql.CommandText += "(Select top 1 prtRFQLineNumber from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = q.quoQuoteId and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 order by prtRFQLineNumber) as lineNumber, ";
                sql.CommandText += "tc.TSGCompanyAbbrev, r.rfqCustomerRFQNumber, r.rfqDueDate, q.quoPartNumbers, q.quoPartName, dt.dtyFullName, c.cavCavityName, ";
                sql.CommandText += "di.dinSizeFrontToBackEnglish, di.dinSizeLeftToRightEnglish, di.dinSizeShutHeightEnglish, di.dinNumberOfStations, bi.binMaterialWidthEnglish, bi.binMaterialPitchEnglish, ";
                sql.CommandText += "bi.binMaterialThicknessEnglish, q.quoLeadTime, q.quoOldQuoteNumber, q.quoVersion, r.rfqID, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToQuote inner join pktblPreWordedNote on pwqPreWordedNoteID = pwnPreWordedNoteID where pwqQuoteID = q.quoQuoteId) as TotalCost, ";
                sql.CommandText += "cu.CustomerName, cul.ShipToName, sm.Name as SalesmanName ";
                sql.CommandText += "from tblQuote q ";
                sql.CommandText += "inner join tblRFQ r on r.rfqID = q.quoRFQID ";
                sql.CommandText += "inner join linkDieInfoToQuote diq on diq.diqQuoteID = q.quoQuoteID ";
                sql.CommandText += "inner join tblDieInfo di on di.dinDieInfoID = diq.diqDieInfoID ";
                sql.CommandText += "inner join pktblBlankInfo bi on bi.binBlankInfoID = q.quoBlankInfoID ";
                sql.CommandText += "inner join TSGCompany tc on tc.TSGCompanyID = q.quoTSGCompanyID ";
                sql.CommandText += "inner join DieType dt on dt.DieTypeID = di.dinDieType ";
                sql.CommandText += "inner join pktblCavity c on c.cavCavityID = di.dinCavityID ";
                sql.CommandText += "inner join Customer cu on cu.CustomerID = r.rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation cul on cul.CustomerLocationID = r.rfqPlantID ";
                sql.CommandText += "inner join TSGSalesman sm on sm.TSGSalesmanID = q.quoSalesman ";
                sql.CommandText += "where quoCreated > DATEADD(WEEK, -1, GETDATE()) ";
                sql.CommandText += "and quoSalesman = @salesman ";
                sql.CommandText += "order by quoTSGCompanyID, quoRFQID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);

                String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                XSSFDrawing DrawingPatriarch = (XSSFDrawing)sh.CreateDrawingPatriarch();
                XSSFDrawing DrawingPatriarch1 = (XSSFDrawing)sh1.CreateDrawingPatriarch();
                XSSFDrawing DrawingPatriarch2 = (XSSFDrawing)sh2.CreateDrawingPatriarch();
                XSSFDrawing DrawingPatriarch3 = (XSSFDrawing)sh3.CreateDrawingPatriarch();

                string currentCompany = "";

                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (currentCompany != dr["TSGCompanyAbbrev"].ToString())
                    {
                        currentCompany = dr["TSGCompanyAbbrev"].ToString();
                        currentRow += 2;
                    }
                    row = GetOrCreateRow(sh, currentRow);
                    //row.CreateCell(0).SetCellValue(dr["picture"].ToString());

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["picture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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

                    if (dr["quoOldQuoteNumber"].ToString() != "")
                    {
                        row.CreateCell(1).SetCellValue(dr["quoOldQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString());
                    }
                    else
                    {
                        row.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["lineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString());
                    }

                    row.CreateCell(2).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(5).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(6).SetCellValue(dr["quoPartNumbers"].ToString());
                    row.CreateCell(7).SetCellValue(dr["quoPartName"].ToString());
                    row.CreateCell(8).SetCellValue(dr["dtyFullName"].ToString());
                    row.CreateCell(9).SetCellValue(dr["cavCavityName"].ToString());
                    row.CreateCell(10).SetCellValue(dr["dinSizeFrontToBackEnglish"].ToString());
                    row.CreateCell(11).SetCellValue(dr["dinSizeLeftToRightEnglish"].ToString());
                    row.CreateCell(12).SetCellValue(dr["dinSizeShutHeightEnglish"].ToString());
                    row.CreateCell(13).SetCellValue(dr["dinNumberOfStations"].ToString());
                    row.CreateCell(14).SetCellValue(dr["binMaterialWidthEnglish"].ToString());
                    row.CreateCell(15).SetCellValue(dr["binMaterialPitchEnglish"].ToString());
                    row.CreateCell(16).SetCellValue(dr["binMaterialThicknessEnglish"].ToString());
                    row.CreateCell(17).SetCellValue(dr["quoLeadTime"].ToString() + " Weeks");
                    row.CreateCell(18).SetCellValue(System.Convert.ToDouble(dr["TotalCost"].ToString()).ToString("$###,###,###.##"));
                    row.CreateCell(19).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow++;
                }
                dr.Close();

                currentRow += 2;

                sql.CommandText = "Select h.hquHTSQuoteID, (Select top 1 prtPicture from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = h.hquHTSQuoteID and ptqHTS = 1 order by prtRFQLineNumber) as picture, ";
                sql.CommandText += "(Select top 1 prtRFQLineNumber from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = h.hquHTSQuoteID and ptqHTS = 1 order by prtRFQLineNumber) as lineNumber, ";
                sql.CommandText += "r.rfqCustomerRFQNumber, r.rfqDueDate, h.hquPartNumbers, h.hquPartName, dt.dtyFullName, c.cavCavityName, h.hquLeadTime, h.hquNumber, h.hquVersion, r.rfqID, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkHTSPWNToHTSQuote inner join pktblPreWordedNote on pthHTSPWNID = pwnPreWordedNoteID where pthHTSQuoteID = h.hquHTSQuoteID) as TotalCost, ";
                sql.CommandText += "cu.CustomerName, cul.ShipToName, sm.Name as SalesmanName ";
                sql.CommandText += "from tblHTSQuote h ";
                sql.CommandText += "inner join linkQuoteToRFQ qtr on qtr.qtrQuoteID = h.hquHTSQuoteID and qtr.qtrHTS = 1 ";
                sql.CommandText += "inner join tblRFQ r on r.RFQID = qtr.qtrRFQID ";
                sql.CommandText += "inner join DieType dt on dt.DieTypeID = h.hquProcess ";
                sql.CommandText += "inner join pktblCavity c on c.cavCavityID = h.hquCavity ";
                sql.CommandText += "inner join Customer cu on cu.CustomerID = r.rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation cul on cul.CustomerLocationID = r.rfqPlantID ";
                sql.CommandText += "inner join TSGSalesman sm on sm.TSGSalesmanID = h.hquSalesman ";
                sql.CommandText += "where h.hquCreated > DATEADD(WEEK, -1, GETDATE()) ";
                sql.CommandText += "and h.hquSalesman = @salesman ";
                sql.CommandText += "order by r.rfqID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    row = GetOrCreateRow(sh, currentRow);
                    //row.CreateCell(0).SetCellValue(dr["picture"].ToString());

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["picture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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


                    int n;
                    if (int.TryParse(dr["hquNumber"].ToString(), out n))
                    {
                        row.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["lineNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString());
                    }
                    else
                    {
                        row.CreateCell(1).SetCellValue(dr["hquNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString());
                    }

                    row.CreateCell(2).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(5).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(6).SetCellValue(dr["hquPartNumbers"].ToString());
                    row.CreateCell(7).SetCellValue(dr["hquPartName"].ToString());
                    row.CreateCell(8).SetCellValue(dr["dtyFullName"].ToString());
                    row.CreateCell(9).SetCellValue(dr["cavCavityName"].ToString());

                    row.CreateCell(17).SetCellValue(dr["hquLeadTime"].ToString() + " Weeks");
                    row.CreateCell(18).SetCellValue(System.Convert.ToDouble(dr["TotalCost"].ToString()).ToString("$###,###,###.##"));
                    row.CreateCell(19).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow++;
                }
                dr.Close();

                currentRow += 2;

                sql.CommandText = "Select s.squSTSQuoteID, (Select top 1 prtPicture from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteId = s.squSTSQuoteID and ptqSTS = 1 order by prtRFQLineNumber) as picture, ";
                sql.CommandText += "(Select top 1 prtRFQLineNumber from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = s.squSTSQuoteID and ptqSTS = 1 order by prtRFQLineNumber) as lineNumber, ";
                sql.CommandText += "r.rfqCustomerRFQNumber, r.rfqDueDate, s.squPartNumber, s.squPartName, s.squProcess, s.squLeadTime, s.squQuoteVersion, rfqID, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToSTSQuote inner join pktblPreWordedNote on psqPreWordedNoteID = pwnPreWordedNoteID where psqSTSQuoteID = s.squSTSQuoteID) as TotalCost, ";
                sql.CommandText += "cu.CustomerName, cul.ShipToName, sm.Name as SalesmanName ";
                sql.CommandText += "from tblSTSQuote s ";
                sql.CommandText += "inner join linkQuoteToRFQ qtr on qtr.qtrQuoteID = s.squSTSQuoteID and qtr.qtrSTS = 1 ";
                sql.CommandText += "inner join tblRFQ r on r.rfqID = qtr.qtrRFQID ";
                sql.CommandText += "inner join Customer cu on cu.CustomerID = r.rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation cul on cul.CustomerLocationID = r.rfqPlantID ";
                sql.CommandText += "inner join TSGSalesman sm on sm.TSGSalesmanID = s.squSalesmanID ";
                sql.CommandText += "where s.squCreated > DATEADD(Week, -1, GETDATE()) ";
                sql.CommandText += "and s.squSalesmanID = @salesman ";
                sql.CommandText += "order by r.rfqID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    row = GetOrCreateRow(sh, currentRow);
                    //row.CreateCell(0).SetCellValue(dr["picture"].ToString());

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["picture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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

                    row.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["lineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString());
                    row.CreateCell(2).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(5).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(6).SetCellValue(dr["squPartNumber"].ToString());
                    row.CreateCell(7).SetCellValue(dr["squPartName"].ToString());
                    row.CreateCell(8).SetCellValue(dr["squProcess"].ToString());

                    row.CreateCell(17).SetCellValue(dr["squLeadTime"].ToString() + " Weeks");
                    row.CreateCell(18).SetCellValue(System.Convert.ToDouble(dr["TotalCost"].ToString()).ToString("$###,###,###.##"));
                    row.CreateCell(19).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow++;
                }
                dr.Close();

                currentRow += 2;

                sql.CommandText = "Select u.uquUGSQuoteID, (Select top 1 prtPicture from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = u.uquUGSQuoteID and ptqUGS = 1 order by prtRFQLineNumber) as picture, ";
                sql.CommandText += "(Select top 1 prtRFQLineNumber from tblPart inner join linkPartToQuote on ptqPartId = prtPARTID and ptqQuoteID = u.uquUGSQuoteID and ptqUGS = 1 order by prtRFQLineNumber) as lineNumber, ";
                sql.CommandText += "r.rfqCustomerRFQNumber, r.rfqDueDate, u.uquPartNumber, u.uquPartName, dt.dtyFullName, u.uquLeadTime, u.uquQuoteNumber, u.uquQuoteVersion, u.uquTotalPrice, r.rfqID, ";
                sql.CommandText += "cu.CustomerName, cul.ShipToName, sm.Name as SalesmanName ";
                sql.CommandText += "from tblUGSQuote u ";
                sql.CommandText += "inner join linkQuoteToRFQ qtr on qtr.qtrQuoteID = u.uquUGSQuoteID and qtr.qtrUGS = 1 ";
                sql.CommandText += "inner join tblRFQ r on r.rfqID = qtr.qtrRFQID ";
                sql.CommandText += "inner join DieType dt on dt.DieTypeID = u.uquDieType ";
                sql.CommandText += "inner join Customer cu on cu.CustomerID = r.rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation cul on cul.CustomerLocationID = r.rfqPlantID ";
                sql.CommandText += "inner join TSGSalesman sm on sm.TSGSalesmanID = u.uquSalesmanID ";
                sql.CommandText += "where u.uquCreated > DATEADD(WEEK, -1, GETDATE()) ";
                sql.CommandText += "and u.uquSalesmanID = @salesman ";
                sql.CommandText += "order by r.rfqID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    row = GetOrCreateRow(sh, currentRow);

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["picture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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

                    row.CreateCell(1).SetCellValue(dr["rfqID"].ToString() + "-" + dr["lineNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString());

                    row.CreateCell(2).SetCellValue(dr["rfqCustomerRFQNumber"].ToString());
                    row.CreateCell(3).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(4).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(5).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(6).SetCellValue(dr["uquPartNumber"].ToString());
                    row.CreateCell(7).SetCellValue(dr["uquPartName"].ToString());
                    row.CreateCell(8).SetCellValue(dr["dtyFullName"].ToString());
                    row.CreateCell(17).SetCellValue(dr["uquLeadTime"].ToString());
                    row.CreateCell(18).SetCellValue(System.Convert.ToDouble(dr["uquTotalPrice"].ToString()).ToString("$###,###,###.##"));
                    row.CreateCell(19).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow++;
                }
                dr.Close();
                                
                int currentRow1 = 0;
                row = GetOrCreateRow(sh1, currentRow1);
                row.CreateCell(0).SetCellValue("Picture");
                row.GetCell(0).CellStyle = CenterStyle;
                row.GetCell(0).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(1).SetCellValue("PartNumber");
                row.GetCell(1).CellStyle = CenterStyle;
                row.GetCell(1).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(2).SetCellValue("Part Name");
                row.GetCell(2).CellStyle = CenterStyle;
                row.GetCell(2).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(3).SetCellValue("RFQ ID");
                row.GetCell(3).CellStyle = CenterStyle;
                row.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(4).SetCellValue("TSG Company");
                row.GetCell(4).CellStyle = CenterStyle;
                row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(5).SetCellValue("Customer");
                row.GetCell(5).CellStyle = CenterStyle;
                row.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(6).SetCellValue("Customer Location");
                row.GetCell(6).CellStyle = CenterStyle;
                row.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(7).SetCellValue("Reserved By");
                row.GetCell(7).CellStyle = CenterStyle;
                row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(8).SetCellValue("Reserved Date");
                row.GetCell(8).CellStyle = CenterStyle;
                row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(9).SetCellValue("Due Date");
                row.GetCell(9).CellStyle = CenterStyle;
                row.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(10).SetCellValue("Part Notes");
                row.GetCell(10).CellStyle = CenterStyle;
                row.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(11).SetCellValue("Salesman");
                row.GetCell(11).CellStyle = CenterStyle;
                row.GetCell(11).RichStringCellValue.ApplyFont(titleFont);

                sh1.SetColumnWidth(0, 5000);
                sh1.SetColumnWidth(1, 4000);
                sh1.SetColumnWidth(2, 10000);
                sh1.SetColumnWidth(3, 6500);
                sh1.SetColumnWidth(4, 6500);
                sh1.SetColumnWidth(5, 4000);
                sh1.SetColumnWidth(6, 6500);
                sh1.SetColumnWidth(7, 6500);
                sh1.SetColumnWidth(8, 4500);
                sh1.SetColumnWidth(9, 4500);
                sh1.SetColumnWidth(10, 4500);
                sh1.SetColumnWidth(11, 3000);
                
                currentRow1 += 2;


                sql.CommandText = "Select rfqId, prtPartNumber, prtPartDescription, prtPicture, TSGCompanyAbbrev, cust.CustomerName, ShipToName, prcCreated, rfqDueDate, prtNote, tsg.Name as SalesmanName, perName ";
                sql.CommandText += "from linkPartReservedToCompany ";
                sql.CommandText += "inner join linkPartToRFQ on ptrPartID = prcPartID ";
                sql.CommandText += "inner join tblPart on prtPARTID = ptrPartID ";
                sql.CommandText += "inner join tblRFQ on rfqID = ptrRFQID ";
                sql.CommandText += "inner join TSGCompany on TSGCompanyID = prcTSGCompanyID ";
                sql.CommandText += "inner join Customer cust on cust.CustomerID = rfqCustomerID ";
                sql.CommandText += "inner join CustomerLocation on CustomerLocationID = rfqPlantID ";
                sql.CommandText += "inner join Permissions on EmailAddress = prcCreatedBy ";
                sql.CommandText += "inner Join tsgSalesman tsg on tsg.tsgSalesmanID = rfqSalesman ";
                sql.CommandText += "where prcCreated > DATEADD(WEEK, -1, GETDATE()) ";
                sql.CommandText += "and rfqSalesman = @salesman order by TSGCompanyabbrev asc";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();

               
                while (dr.Read())
                {
                    row = GetOrCreateRow(sh1, currentRow1);

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["prtPicture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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
                            XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, currentRow1, 0, currentRow1);
                            
                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch1.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.22);
                        }
                        catch
                        {

                        }
                    }

              

                    row.CreateCell(1).SetCellValue(dr["prtPartNumber"].ToString());
                    row.CreateCell(2).SetCellValue(dr["prtPartDescription"].ToString());
                    row.CreateCell(3).SetCellValue(dr["RFQID"].ToString());
                    row.CreateCell(4).SetCellValue(dr["TSGCompanyAbbrev"].ToString());
                    row.CreateCell(5).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(6).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(7).SetCellValue(dr["perName"].ToString());
                    row.CreateCell(8).SetCellValue(System.Convert.ToDateTime(dr["PrcCreated"].ToString()).ToShortDateString());
                    row.CreateCell(9).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(10).SetCellValue(dr["prtNote"].ToString());
                    row.CreateCell(11).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow1++;
                }
                dr.Close();

                //this is for unreserved quotes

                int currentRow2 = 0;
                row = GetOrCreateRow(sh2, currentRow2);
                row.CreateCell(0).SetCellValue("Picture");
                row.GetCell(0).CellStyle = CenterStyle;
                row.GetCell(0).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(1).SetCellValue("PartNumber");
                row.GetCell(1).CellStyle = CenterStyle;
                row.GetCell(1).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(2).SetCellValue("Part Name");
                row.GetCell(2).CellStyle = CenterStyle;
                row.GetCell(2).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(3).SetCellValue("RFQ ID");
                row.GetCell(3).CellStyle = CenterStyle;
                row.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
                //row.CreateCell(4).SetCellValue("TSG Company");
                //row.GetCell(4).CellStyle = CenterStyle;
                //row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(4).SetCellValue("Customer");
                row.GetCell(4).CellStyle = CenterStyle;
                row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(5).SetCellValue("Customer Location");
                row.GetCell(5).CellStyle = CenterStyle;
                row.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
                //row.CreateCell(7).SetCellValue("Reserved By");
                //row.GetCell(7).CellStyle = CenterStyle;
                //row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                //row.CreateCell(8).SetCellValue("Reserved Date");
                //row.GetCell(8).CellStyle = CenterStyle;
                //row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(6).SetCellValue("Due Date");
                row.GetCell(6).CellStyle = CenterStyle;
                row.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(7).SetCellValue("Part Notes");
                row.GetCell(7).CellStyle = CenterStyle;
                row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(8).SetCellValue("Salesman");
                row.GetCell(8).CellStyle = CenterStyle;
                row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);

                sh2.SetColumnWidth(0, 5000);
                sh2.SetColumnWidth(1, 5000);
                sh2.SetColumnWidth(2, 10000);
                sh2.SetColumnWidth(3, 4000);
                sh2.SetColumnWidth(4, 5000);
                sh2.SetColumnWidth(5, 7500);
                sh2.SetColumnWidth(6, 4500);
                sh2.SetColumnWidth(7, 6500);
                sh2.SetColumnWidth(8, 6500);
                //sh2.SetColumnWidth(9, 4500);
                //sh2.SetColumnWidth(10, 4500);
                //sh2.SetColumnWidth(11, 3000);

                currentRow2 += 2;


                sql.CommandText = "Select rfqID, prtPartNumber, prtPartDescription, prtPARTID, CustomerName, prtCreated, rfqDueDate, prtPicture, ShipToName, prtPartLength, prtPartWidth, prtPartHeight, prtNote, tsg.Name as SalesmanName ";
                sql.CommandText += "from tblPart, linkPartToRFQ, tblRFQ, Customer, CustomerLocation, TSGSalesman tsg where rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocation.CustomerLocationID and ptrPartID = prtPARTID and ptrRFQID = rfqID and Customer.CustomerID = CustomerLocation.CustomerID ";
                sql.CommandText += "and not EXISTS (Select * from linkPartToQuote where ptqPartID = prtPARTID) ";
                sql.CommandText += "and not exists (Select * from linkPartReservedToCompany where prcPartID = prtPARTID) ";
                sql.CommandText += "and (Select (select distinct 1 from linkPartToPartDetail, linkPartReservedToCompany where ppd.ppdPartToPartID = ppdPartToPartID and ppd.ppdPartID <> ppdPartID and ppdPartID = prcPartID) from linkPartToPartDetail as ppd where ppdPartID = prtPartID) is null ";
                sql.CommandText += "and not exists (select 1 where (select count(nquNoQuoteID) from tblNoQuote where nquPartID = prtPARTID) >= (Select (count(*) - 1) from linkRFQToCompany where rtqRFQID = rfqID)) ";
                sql.CommandText += "and rfqCreated > DATEADD(Week, -1, GETDATE()) and rfqSalesman = @salesman and rfqSalesman = tsg.TSGSalesmanID ";
                sql.CommandText += "order by rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();



                while (dr.Read())
                {
                    row = GetOrCreateRow(sh2, currentRow2);

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["prtPicture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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
                            XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, currentRow2, 0, currentRow2);

                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch2.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.22);
                        }
                        catch
                        {

                        }
                    }

                    row.CreateCell(1).SetCellValue(dr["prtPartNumber"].ToString());
                    row.CreateCell(2).SetCellValue(dr["prtPartDescription"].ToString());
                    row.CreateCell(3).SetCellValue(dr["RFQID"].ToString());
                    //row.CreateCell(4).SetCellValue(dr["TSGCompanyAbbrev"].ToString());
                    row.CreateCell(4).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(5).SetCellValue(dr["ShipToName"].ToString());
                    //row.CreateCell(7).SetCellValue(dr["perName"].ToString());
                    //row.CreateCell(8).SetCellValue(System.Convert.ToDateTime(dr["PrcCreated"].ToString()).ToShortDateString());
                    row.CreateCell(6).SetCellValue(System.Convert.ToDateTime(dr["rfqDueDate"].ToString()).ToShortDateString());
                    row.CreateCell(7).SetCellValue(dr["prtNote"].ToString());
                    row.CreateCell(8).SetCellValue(dr["SalesmanName"].ToString());
                    currentRow2++;
                }
                dr.Close();


                //this is for unreserved quotes

                int currentRow3 = 0;
                row = GetOrCreateRow(sh3, currentRow3);
                row.CreateCell(0).SetCellValue("Picture");
                row.GetCell(0).CellStyle = CenterStyle;
                row.GetCell(0).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(1).SetCellValue("PartNumber");
                row.GetCell(1).CellStyle = CenterStyle;
                row.GetCell(1).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(2).SetCellValue("Part Name");
                row.GetCell(2).CellStyle = CenterStyle;
                row.GetCell(2).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(3).SetCellValue("RFQ ID");
                row.GetCell(3).CellStyle = CenterStyle;
                row.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(4).SetCellValue("Part Length");
                row.GetCell(4).CellStyle = CenterStyle;
                row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(5).SetCellValue("Part Width");
                row.GetCell(5).CellStyle = CenterStyle;
                row.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(6).SetCellValue("Part Height");
                row.GetCell(6).CellStyle = CenterStyle;
                row.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(7).SetCellValue("Customer");
                row.GetCell(7).CellStyle = CenterStyle;
                row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(8).SetCellValue("Plant");
                row.GetCell(8).CellStyle = CenterStyle;
                row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(8).SetCellValue("First Reserved");
                row.GetCell(8).CellStyle = CenterStyle;
                row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(9).SetCellValue("RFQ Due Date");
                row.GetCell(9).CellStyle = CenterStyle;
                row.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(10).SetCellValue("UnReserved Date");
                row.GetCell(10).CellStyle = CenterStyle;
                row.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(11).SetCellValue("Company");
                row.GetCell(11).CellStyle = CenterStyle;
                row.GetCell(11).RichStringCellValue.ApplyFont(titleFont);
                row.CreateCell(12).SetCellValue("Name");
                row.GetCell(12).CellStyle = CenterStyle;
                row.GetCell(12).RichStringCellValue.ApplyFont(titleFont);

                sh3.SetColumnWidth(0, 5000);
                sh3.SetColumnWidth(1, 4000);
                sh3.SetColumnWidth(2, 10000);
                sh3.SetColumnWidth(3, 3500);
                sh3.SetColumnWidth(4, 3500);
                sh3.SetColumnWidth(5, 3500);
                sh3.SetColumnWidth(6, 7000);
                sh3.SetColumnWidth(7, 6500);
                sh3.SetColumnWidth(8, 4500);
                sh3.SetColumnWidth(9, 4500);
                sh3.SetColumnWidth(10, 4500);
                sh3.SetColumnWidth(11, 3000);
                sh3.SetColumnWidth(12, 3500);

                currentRow3 += 2;

                sql.CommandText = "Select rfqID, prtPartNumber, prtpartDescription, prtPartLength, prtPartWidth, prtPartHeight, CustomerName, ShipToName, prtCreated,  ";
                sql.CommandText += "ptuInitialReservedDate, convert(date, rfqDueDate) as dueDate, convert(date, ptuCreated) as UnReservedDate, perName, TSGCompanyAbbrev, tsg.name as SalesmanName, prtPicture  ";
                sql.CommandText += "from linkPartToUnreserved, linkPartToRFQ, tblPart, tblRFQ, Customer, CustomerLocation, Permissions, TSGCompany, tsgSalesman tsg ";
                sql.CommandText += "where ptuPartID = ptrPartID and ptrRFQID = rfqID and rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocationID and  ";
                sql.CommandText += "prtPARTID = ptrPartID and ptuRereserved = 0 and ptuCreated > DATEADD(MONTH, -2, GETDATE()) and ptuUID = UID and TSGCompanyID = ptuCompanyUnreserved  ";
                sql.CommandText += "and ptucreated > DATEADD(MONTH, -1, getdate()) and rfqSalesman = @salesman and rfqSalesman = tsg.TSGSalesmanID ";
                sql.CommandText += "order by rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@salesman", salesmenID[i]);
                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    row = GetOrCreateRow(sh3, currentRow3);

                    row.Height = 1000;
                    row.CreateCell(0);
                    // get picture from sharepoint and insert
                    // This points to where the pictures are
                    string sharepointLibrary = "Part Pictures";
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr["prtPicture"].ToString());
                        // open the file as binary
                        try
                        {
                            byte[] pictureData;
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
                            XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 0, currentRow3, 0, currentRow3);

                            anchor.AnchorType = 2;
                            int PictureIndex = wb.AddPicture(pictureData, NPOI.SS.UserModel.PictureType.PNG);
                            XSSFPicture Picture = (XSSFPicture)DrawingPatriarch3.CreatePicture(anchor, PictureIndex);
                            // The picture will not appear unless you run resize
                            // in this case, scaling to this value seems to work best
                            Picture.Resize(.22);
                        }
                        catch
                        {

                        }
                    }

                    row.CreateCell(1).SetCellValue(dr["prtPartNumber"].ToString());
                    row.CreateCell(2).SetCellValue(dr["prtPartDescription"].ToString());
                    row.CreateCell(3).SetCellValue(dr["RFQID"].ToString());
                    row.CreateCell(4).SetCellValue(dr["prtPartLength"].ToString());
                    row.CreateCell(4).SetCellValue(dr["prtPartWidth"].ToString());
                    row.CreateCell(5).SetCellValue(dr["prtPartHeight"].ToString());
                    row.CreateCell(6).SetCellValue(dr["CustomerName"].ToString());
                    row.CreateCell(7).SetCellValue(dr["ShipToName"].ToString());
                    row.CreateCell(8).SetCellValue(System.Convert.ToDateTime(dr["ptuInitialReservedDate"].ToString()).ToShortDateString());
                    row.CreateCell(9).SetCellValue(System.Convert.ToDateTime(dr["dueDate"].ToString()).ToShortDateString());
                    row.CreateCell(10).SetCellValue(System.Convert.ToDateTime(dr["UnReservedDate"].ToString()).ToShortDateString());

                    row.CreateCell(11).SetCellValue(dr["TSGCompanyAbbrev"].ToString());
                    row.CreateCell(12).SetCellValue(dr["perName"].ToString());
                    currentRow3++;
                }
                dr.Close(); 

                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "QuoteSheet-RFQ" + "test" + ".xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();

            }
            connection.Close();
        }

        public void QuotesOnly(HttpContext context)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Quotes");

            NPOI.SS.UserModel.IRow rrow;

            XSSFFont titleFont = (XSSFFont)wb.CreateFont();
            titleFont.FontHeight = 12;
            titleFont.Boldweight = 700;
            titleFont.IsItalic = true;

            XSSFCellStyle CenterStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CenterStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            string company = "";
            company = context.Request["company"].ToString();
            String Companyabrv = "";

            DateTime StartDate = System.Convert.ToDateTime("1/1/2000");
            DateTime EndDate = DateTime.Now;
            try
            {
                StartDate = System.Convert.ToDateTime(context.Request["start"]);
            }
            catch
            {

            }
            try
            {
                EndDate = System.Convert.ToDateTime(context.Request["end"]);
            }
            catch
            {
            }

            Int16 currentRow = 0;
            rrow = GetOrCreateRow(sh, currentRow);
            rrow.CreateCell(1).SetCellValue("Quote Number");
            rrow.CreateCell(2).SetCellValue("Customer Name");
            rrow.CreateCell(3).SetCellValue("Plant");
            rrow.CreateCell(4).SetCellValue("Customer Contact");
            rrow.CreateCell(5).SetCellValue("Salesman");
            rrow.CreateCell(6).SetCellValue("Estimator");
            rrow.CreateCell(7).SetCellValue("Rfq ID");
            rrow.CreateCell(8).SetCellValue("Status");
            rrow.CreateCell(9).SetCellValue("Part Number");
            rrow.CreateCell(10).SetCellValue("Part Name");
            rrow.CreateCell(11).SetCellValue("Customer RFQ #");
            rrow.CreateCell(12).SetCellValue("Total");
            rrow.CreateCell(13).SetCellValue("Created");
            currentRow++;

                if (company != "9" && company != "13" && company != "15" && company != "20")
                {
                sql.CommandText = "select TSGCompanyAbbrev from TSGCompany where TSGCompanyID = @company ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", company);
                SqlDataReader dr2 = sql.ExecuteReader();
                if (dr2.Read())
                {
                    Companyabrv = dr2["TSGCompanyAbbrev"].ToString();
                }
                dr2.Close();

                sql.CommandText = "Select QuoteNumber, CustomerName, Plant, Contact, Salesman, Estimator, RfqID, Status, PartNumber, PartName, CustomerRFQNumber, Cost, DateCreated from vDieShopQuotes ";
                sql.CommandText += "where DateCreated >= @start and DateCreated <= @end ";
                if (company != "1")
                {
                    sql.CommandText += " and Company = @company ";
                }
                sql.CommandText += "order by Company asc, DateCreated desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate);
                sql.Parameters.AddWithValue("@end", EndDate);
                if (company != "1")
                {
                    sql.Parameters.AddWithValue("@company", Companyabrv);
                }
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(sh, currentRow);
                    rrow.CreateCell(1).SetCellValue(dr["QuoteNumber"].ToString());
                    rrow.CreateCell(2).SetCellValue(dr["CustomerName"].ToString());
                    rrow.CreateCell(3).SetCellValue(dr["Plant"].ToString());
                    rrow.CreateCell(4).SetCellValue(dr["Contact"].ToString());
                    rrow.CreateCell(5).SetCellValue(dr["Salesman"].ToString());
                    rrow.CreateCell(6).SetCellValue(dr["Estimator"].ToString());
                    rrow.CreateCell(7).SetCellValue(dr["RFQID"].ToString());
                    rrow.CreateCell(8).SetCellValue(dr["Status"].ToString());
                    rrow.CreateCell(9).SetCellValue(dr["PartNumber"].ToString());
                    rrow.CreateCell(10).SetCellValue(dr["PartName"].ToString());
                    rrow.CreateCell(11).SetCellValue(dr["CustomerRFQNumber"].ToString());
                    if (dr["Cost"].ToString() != "")
                    {
                        rrow.CreateCell(12).SetCellValue(System.Convert.ToDouble(dr["Cost"].ToString()).ToString("$###,###,###,###.##"));
                    }
                    if (dr["DateCreated"].ToString() != "")
                    {
                        rrow.CreateCell(13).SetCellValue(System.Convert.ToDateTime(dr["DateCreated"].ToString()).ToShortDateString());
                    }
                    currentRow++;
                }
                dr.Close();
            }
            if (company == "1" || company == "9")
            {
                sql.CommandText = "Select QuoteNumber, CustomerName, Plant, Contact, Salesman, Estimator, RfqID, Status, PartNumber, PartName, CustomerRFQNumber, Cost, DateCreated from vHTSQuotes ";
                sql.CommandText += "where DateCreated >= @start and DateCreated <= @end  ";
                sql.CommandText += "order by DateCreated desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate);
                sql.Parameters.AddWithValue("@end", EndDate);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(sh, currentRow);
                    rrow.CreateCell(1).SetCellValue(dr["QuoteNumber"].ToString());
                    rrow.CreateCell(2).SetCellValue(dr["CustomerName"].ToString());
                    rrow.CreateCell(3).SetCellValue(dr["Plant"].ToString());
                    rrow.CreateCell(4).SetCellValue(dr["Contact"].ToString());
                    rrow.CreateCell(5).SetCellValue(dr["Salesman"].ToString());
                    rrow.CreateCell(6).SetCellValue(dr["Estimator"].ToString());
                    rrow.CreateCell(7).SetCellValue(dr["RFQID"].ToString());
                    rrow.CreateCell(8).SetCellValue(dr["Status"].ToString());
                    rrow.CreateCell(9).SetCellValue(dr["PartNumber"].ToString());
                    rrow.CreateCell(10).SetCellValue(dr["PartName"].ToString());
                    rrow.CreateCell(11).SetCellValue(dr["CustomerRFQNumber"].ToString());
                    if (dr["Cost"].ToString() != "")
                    {
                        rrow.CreateCell(12).SetCellValue(System.Convert.ToDouble(dr["Cost"].ToString()).ToString("$###,###,###,###.##"));
                    }
                    if (dr["DateCreated"].ToString() != "")
                    {
                        rrow.CreateCell(13).SetCellValue(System.Convert.ToDateTime(dr["DateCreated"].ToString()).ToShortDateString());
                    }
                    currentRow++;
                }
                dr.Close();
            }
            if (company == "1" || company == "13" || company == "20")
            {
                sql.CommandText = "Select QuoteNumber, CustomerName, Plant, Contact, Salesman, Estimator, RfqID, Status, PartNumber, PartName, CustomerRFQNumber, Cost, Cost2, DateCreated from vSTSQuotes ";
                sql.CommandText += "where DateCreated >= @start and DateCreated <= @end  ";
                sql.CommandText += "order by DateCreated desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate);
                sql.Parameters.AddWithValue("@end", EndDate);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(sh, currentRow);
                    rrow.CreateCell(1).SetCellValue(dr["QuoteNumber"].ToString());
                    rrow.CreateCell(2).SetCellValue(dr["CustomerName"].ToString());
                    rrow.CreateCell(3).SetCellValue(dr["Plant"].ToString());
                    rrow.CreateCell(4).SetCellValue(dr["Contact"].ToString());
                    rrow.CreateCell(5).SetCellValue(dr["Salesman"].ToString());
                    rrow.CreateCell(6).SetCellValue(dr["Estimator"].ToString());
                    rrow.CreateCell(7).SetCellValue(dr["RFQID"].ToString());
                    rrow.CreateCell(8).SetCellValue(dr["Status"].ToString());
                    rrow.CreateCell(9).SetCellValue(dr["PartNumber"].ToString());
                    rrow.CreateCell(10).SetCellValue(dr["PartName"].ToString());
                    rrow.CreateCell(11).SetCellValue(dr["CustomerRFQNumber"].ToString());
                    if (dr["Cost"].ToString() != "")
                    {
                        rrow.CreateCell(12).SetCellValue(System.Convert.ToDouble(dr["Cost"].ToString()).ToString("$###,###,###,###.##"));
                    }
                    if (dr["Cost2"].ToString() != "")
                    {
                        rrow.CreateCell(12).SetCellValue(System.Convert.ToDouble(dr["Cost2"].ToString()).ToString("$###,###,###,###.##"));
                    }
                    if (dr["DateCreated"].ToString() != "")
                    {
                        rrow.CreateCell(13).SetCellValue(System.Convert.ToDateTime(dr["DateCreated"].ToString()).ToShortDateString());
                    }
                    currentRow++;
                }
                dr.Close();
            }
            if (company == "1" || company == "15")
            {
                sql.CommandText = "Select QuoteNumber, CustomerName, Plant, Contact, Salesman, Estimator, RfqID, Status, PartNumber, PartName, CustomerRFQNumber, Cost, DateCreated from vUGSQuotes ";
                sql.CommandText += "where DateCreated >= @start and DateCreated <= @end  ";
                sql.CommandText += "order by DateCreated desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate);
                sql.Parameters.AddWithValue("@end", EndDate);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(sh, currentRow);
                    rrow.CreateCell(1).SetCellValue(dr["QuoteNumber"].ToString());
                    rrow.CreateCell(2).SetCellValue(dr["CustomerName"].ToString());
                    rrow.CreateCell(3).SetCellValue(dr["Plant"].ToString());
                    rrow.CreateCell(4).SetCellValue(dr["Contact"].ToString());
                    rrow.CreateCell(5).SetCellValue(dr["Salesman"].ToString());
                    rrow.CreateCell(6).SetCellValue(dr["Estimator"].ToString());
                    rrow.CreateCell(7).SetCellValue(dr["RFQID"].ToString());
                    rrow.CreateCell(8).SetCellValue(dr["Status"].ToString());
                    rrow.CreateCell(9).SetCellValue(dr["PartNumber"].ToString());
                    rrow.CreateCell(10).SetCellValue(dr["PartName"].ToString());
                    rrow.CreateCell(11).SetCellValue(dr["CustomerRFQNumber"].ToString());
                    if (dr["Cost"].ToString() != "")
                    {
                        rrow.CreateCell(12).SetCellValue(System.Convert.ToDouble(dr["Cost"].ToString()).ToString("$###,###,###,###.##"));
                    }
                    if (dr["DateCreated"].ToString() != "")
                    {
                        rrow.CreateCell(13).SetCellValue(System.Convert.ToDateTime(dr["DateCreated"].ToString()).ToShortDateString());
                    }
                    currentRow++;
                }
                dr.Close();
            }

            sh.AutoSizeColumn(1);
            sh.AutoSizeColumn(2);
            sh.AutoSizeColumn(3);
            sh.AutoSizeColumn(4);
            sh.AutoSizeColumn(5);
            sh.AutoSizeColumn(6);
            sh.AutoSizeColumn(7);
            sh.AutoSizeColumn(8);
            sh.AutoSizeColumn(9);
            sh.AutoSizeColumn(10);
            sh.AutoSizeColumn(11);


            connection.Close();

            using (var ms = new System.IO.MemoryStream())
            {
                wb.Write(ms);
                MemoryStream ms2 = new MemoryStream(ms.ToArray());

                SmtpClient server = new SmtpClient("smtp.office365.com");
                server.UseDefaultCredentials = false;
                server.Port = 587;
                server.EnableSsl = true;
                // TODO send as another user
                server.Credentials = master.getNetworkCredentials();
                server.Timeout = 50000;
                server.TargetName = "STARTTLS/smtp.office365.com";
                MailMessage mail = new MailMessage();

                mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, "Quotes.xlsx"));

                mail.From = master.getFromAddress();
                if (master.getUserName() == "chris@netinflux.com")
                {
                    mail.To.Add(new MailAddress("rmumford@toolingsystemsgroup.com"));
                }
                else
                {
                    mail.To.Add(new MailAddress(master.getUserName()));
                }
                mail.Subject = "Quote Report";
                mail.Body = "Attached is the report on quoting activity.<br />";
                mail.IsBodyHtml = true;
                try
                {
                    server.Send(mail);
                }
                catch
                {
                    try
                    {
                        server.Send(mail);
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            //testSendWeeklyRecap(context);
            //return;
            try
            {
                if (context.Request["OnlyQuotes"].ToString() != "")
                {
                    QuotesOnly(context);
                }
                return;
            }
            catch (Exception err)
            {

            }


            DateTime StartDate = System.Convert.ToDateTime("1/1/" + DateTime.Now.Year.ToString());
            DateTime EndDate = DateTime.Now;
            try
            {
                StartDate = System.Convert.ToDateTime(context.Request["start"]);
            }
            catch
            {

            }
            try
            {
                EndDate = System.Convert.ToDateTime(context.Request["end"]);
            }
            catch
            {
            }
            Int64 Company = 1;
            try
            {
                //Company = System.Convert.ToInt64(context.Request["company"]);
            }
            catch
            {
            }
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            Company = master.getCompanyId();
            
            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet estimatingSheet = (XSSFSheet)wb.CreateSheet("Estimating");
            XSSFSheet unreservedSheet = (XSSFSheet)wb.CreateSheet("Unreserved");
            XSSFSheet noquoteSheet = (XSSFSheet)wb.CreateSheet("No Quote");
            XSSFSheet oemSheet = (XSSFSheet)wb.CreateSheet("OEM");
            XSSFSheet stsSheet = (XSSFSheet)wb.CreateSheet("STS");
            XSSFSheet ugsSheet = (XSSFSheet)wb.CreateSheet("UGS");
            XSSFSheet saSheet = (XSSFSheet)wb.CreateSheet("SA");


            NPOI.SS.UserModel.IRow rrow;

            XSSFFont titleFont = (XSSFFont)wb.CreateFont();
            titleFont.FontHeight = 12;
            titleFont.Boldweight = 700;
            titleFont.IsItalic = true;

            XSSFCellStyle CenterStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CenterStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            XSSFCellStyle RightStyle = (XSSFCellStyle)wb.CreateCellStyle();
            RightStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

            XSSFCellStyle LateStyle = (XSSFCellStyle)wb.CreateCellStyle();
            LateStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
            LateStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Yellow.Index;
            LateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

            XSSFCellStyle ReallyLateStyle = (XSSFCellStyle)wb.CreateCellStyle();
            ReallyLateStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
            ReallyLateStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Red.Index;
            ReallyLateStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

            XSSFCellStyle OnTimeStyle = (XSSFCellStyle)wb.CreateCellStyle();
            OnTimeStyle.FillPattern = NPOI.SS.UserModel.FillPattern.LessDots;
            OnTimeStyle.FillBackgroundColor = NPOI.SS.UserModel.IndexedColors.Green.Index;
            OnTimeStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

            XSSFCellStyle DayStyle = (XSSFCellStyle)wb.CreateCellStyle();

            XSSFFont LinkFont = (XSSFFont)wb.CreateFont();
            LinkFont.Underline = NPOI.SS.UserModel.FontUnderlineType.Single;
            XSSFColor LinkColor = new XSSFColor();
            byte[] Blue = { 0, 0, 128 };
            LinkColor.SetRgb(Blue);
            LinkFont.SetColor(LinkColor);

            //CreateEstimatingTitles(estimatingSheet,  titleFont,  EndDate, CenterStyle);

            estimatingSheet.CreateFreezePane(1, 3);


            Int32 currentRow = 3;
            String HoldCompany = "";
            Int64 QuoteCount = 0;

            sql.CommandText = "select rfqid, tsgcompanyabbrev, prtRFQLineNumber, pktblBlankInfo.*,  mtyMaterialType,  vehVehicleName,   rfqProgramID, ";
            sql.CommandText += "ShipToName, rfqCustomerRFQNumber,  ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID, rfqDateReceived, ";
            sql.CommandText += "rfqDueDate as DueDate, prtPartNumber, prtPartDescription, prtNote, CustomerContact.Name, ptyPartTypeDescription, TSGSalesman.Name as Salesman, CustomerName ";
            sql.CommandText += "from  tsgcompany, tblrfq, tsgsalesman, oem,  Program, CustomerLocation,  linkPartToRFQ, CustomerContact,  pktblVehicle,  linkPartReservedToCompany, Customer, tblPart ";
            sql.CommandText += " left outer join pktblBlankInfo on prtBlankInfoID=binBlankInfoID left outer join pktblMaterialType on prtPartMaterialType=mtyMaterialTypeID left outer join pktblPartType on prtPartTypeID=ptyPartTypeID ";
            sql.CommandText += " where prtPartID=prcPartID and  prcTSGcompanyid=TSGCompany.tsgcompanyid and tblrfq.rfqOEMID=oem.oemid and rfqProgramID=ProgramID and rfqPlantID=CustomerLocationID and rfqid=ptrRFQID and ptrPartID=prtPartID and rfqCustomerContact=customerContactID  and rfqVehicleID=vehVehicleID ";
            sql.CommandText += " and prtPartId not in (select ptqPartID from linkPartToQuote, tblQuote where quoTSGCompanyID = TSGCompanyID and ptqQuoteID = quoQuoteID) ";
            sql.CommandText += " and prtPARTID not in (Select ptqPartID from linkPartToQuote, tblHTSQuote where hquHTSQuoteID = ptqQuoteID and TSGCompanyID = 9) ";
            sql.CommandText += " and prtPARTID not in (select ptqPartID from linkPartToQuote, tblSTSQuote where squSTSQuoteID = ptqQuoteID and TSGCompanyID = 13) ";
            sql.CommandText += " and prtPARTID not in (select ptqPartID from linkPartToQuote, tblUGSQuote where uquUGSQuoteID = ptqQuoteID and TSGCompanyID = 15) ";
            sql.CommandText += " and rfqSalesman=TSGSalesman.TSGSalesmanID ";
            sql.CommandText += " and rfqCustomerID=Customer.CustomerID ";
            sql.CommandText += " and rfqStatus in (1,2,12) ";
            sql.CommandText += " Order by tsgcompanyabbrev, CustomerName, ShipToName, rfqid, prtRFQLineNumber ";

            SqlDataReader ldr = sql.ExecuteReader();
            while (ldr.Read())
            {
                if (ldr["tsgCompanyAbbrev"].ToString() != HoldCompany)
                {
                    if (HoldCompany != "")
                    {
                        rrow = GetOrCreateRow(estimatingSheet, currentRow);
                        rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                        currentRow++;
                    }
                    HoldCompany = ldr["tsgCompanyAbbrev"].ToString();
                    QuoteCount = 0;
                }
                QuoteCount++;
                rrow = GetOrCreateRow(estimatingSheet, currentRow);
                NPOI.SS.UserModel.ICell cell = rrow.CreateCell(2);
                cell.SetCellValue(ldr["rfqid"].ToString());
                NPOI.XSSF.UserModel.XSSFHyperlink link = new XSSFHyperlink(NPOI.SS.UserModel.HyperlinkType.Url);
                link.Address = ("https://tsgrfq.azurewebsites.net/EditRFQ?id=" + ldr["rfqid"].ToString());
                cell.Hyperlink = (link);
                cell.RichStringCellValue.ApplyFont(0, ldr["rfqid"].ToString().Length, LinkFont);

                rrow.CreateCell(4).SetCellValue(ldr["tsgCompanyAbbrev"].ToString());
                rrow.CreateCell(5).SetCellValue(ldr["rfqDateReceived"].ToString().Split(' ')[0]);
                rrow.CreateCell(6).SetCellValue(ldr["DueDate"].ToString().Split(' ')[0]);
                String DueDate = ldr["DueDate"].ToString();
                DateTime RFQSent = EndDate;
                DayStyle = RightStyle;
                Double DaysLate = (RFQSent - System.Convert.ToDateTime(ldr["DueDate"].ToString())).TotalDays;
                rrow.CreateCell(7).SetCellValue(DaysLate.ToString());
                if (DaysLate > 0)
                {
                    if (DaysLate > 7)
                    {
                        rrow.GetCell(7).CellStyle = ReallyLateStyle;
                    }
                    else
                    {
                        rrow.GetCell(7).CellStyle = LateStyle;

                    }
                }
                else
                {
                    rrow.GetCell(7).CellStyle = DayStyle;
                }
                rrow.CreateCell(8).SetCellValue(ldr["rfqMeetingNotes"].ToString() + " " + ldr["prtNote"].ToString());
                rrow.CreateCell(9).SetCellValue(ldr["ShipToName"].ToString());
                rrow.CreateCell(10).SetCellValue(ldr["prtPartNumber"].ToString());
                rrow.CreateCell(11).SetCellValue(ldr["prtPartDescription"].ToString());
                rrow.CreateCell(12).SetCellValue(ldr["Salesman"].ToString());
                rrow.CreateCell(13).SetCellValue(ldr["binMaterialWidthEnglish"].ToString());
                rrow.CreateCell(14).SetCellValue(ldr["binMaterialWidthMetric"].ToString());
                rrow.CreateCell(15).SetCellValue(ldr["binMaterialPitchEnglish"].ToString());
                rrow.CreateCell(16).SetCellValue(ldr["binMaterialPitchMetric"].ToString());
                rrow.CreateCell(17).SetCellValue(ldr["binMaterialThicknessEnglish"].ToString());
                rrow.CreateCell(18).SetCellValue(ldr["binMaterialThicknessMetric"].ToString());
                rrow.CreateCell(19).SetCellValue(ldr["mtyMaterialType"].ToString());
                rrow.CreateCell(20).SetCellValue(ldr["rfqCustomerRFQNumber"].ToString());
                rrow.CreateCell(21).SetCellValue(ldr["OEMName"].ToString());
                rrow.CreateCell(22).SetCellValue(ldr["vehVehicleName"].ToString());
                rrow.CreateCell(23).SetCellValue(ldr["ProgramName"].ToString());
                rrow.CreateCell(24).SetCellValue(ldr["Name"].ToString());
                currentRow++;
            }
            ldr.Close();
            
            
            if (QuoteCount > 0) 
            {
                rrow = GetOrCreateRow(estimatingSheet,currentRow);
                rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                currentRow++;
            }
            
            estimatingSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 25; i++ )
            {
                estimatingSheet.AutoSizeColumn(i);
            }
            estimatingSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:Y3"));

            // Unreserved Worksheet
            // These are all parts that have not been quoted, but at least one company has not no quoted it
            // No Quote Worksheet
            // These are all parts that have not been quoted, and all companies have no quoted it

            CreateEstimatingTitles(unreservedSheet, titleFont, EndDate, CenterStyle);
            CreateEstimatingTitles(noquoteSheet, titleFont, EndDate, CenterStyle);

            XSSFFont redFont = (XSSFFont)wb.CreateFont();
            redFont.FontHeight = 14;
            redFont.Boldweight = 700;
            redFont.IsItalic = true;
            XSSFColor ColorRed = new XSSFColor();
            byte[] red = { 128, 0, 0 };
            ColorRed.SetRgb(red);
            redFont.SetColor(ColorRed);

            XSSFFont blackFont = (XSSFFont)wb.CreateFont();
            blackFont.FontHeight = 14;
            blackFont.Boldweight = 700;
            blackFont.IsItalic = true;
            XSSFColor colorBlack = new XSSFColor();
            byte[] black = { 0, 0, 0 };
            colorBlack.SetRgb(black);
            blackFont.SetColor(colorBlack);


            Int32 uCurrentRow = 3;
            Int32 nCurrentRow = 3;
            String HoldPart = "";
            Int32 uQuoteCount = 0;
            Int32 nQuoteCount = 0;
            Boolean AllNoQuotes = true;
            Boolean TSGNoQuoted = false;
            HoldCompany = "";
            List<String> NoQuoteList = new List<string>();
            List<String> NotRespondedList = new List<string>();
            EstimatingRow newRow = new EstimatingRow();

            // had to do all of these inner joins specifically so I count left outer join tblnoquote on 2 tables
            sql.CommandText = "select rfqid, tsgcompanyabbrev, prtRFQLineNumber, pktblBlankInfo.*,  mtyMaterialType,  vehVehicleName,   rfqProgramID, ";
            sql.CommandText += "ShipToName, rfqCustomerRFQNumber,  ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID, rfqDateReceived, ";
            sql.CommandText += "rfqDueDate as DueDate, prtPartNumber, prtPartDescription, prtNote, CustomerContact.Name, ptyPartTypeDescription, nquCompanyID, tsgsalesman.name as Salesman, CustomerName, TSGCompanyID ";
            sql.CommandText += "from tblrfq  ";
            sql.CommandText += " inner join tsgsalesman on rfqSalesman=TSGSalesman.TSGSalesmanID ";
            sql.CommandText += "inner join linkrfqtocompany on rfqid=rtqRFQID ";
            sql.CommandText += "inner join tsgcompany on rtqCompanyID=TSGCompanyId ";
            sql.CommandText += "inner join oem on tblrfq.rfqOEMID=oem.oemid ";
            sql.CommandText += "inner join Program on rfqProgramID=ProgramID ";
            sql.CommandText += "inner join CustomerLocation on rfqPlantID=CustomerLocationID ";
            sql.CommandText += "inner join linkPartToRFQ on ptrRFQID=rfqid ";
            sql.CommandText += "inner join CustomerContact on rfqCustomerContact=customerContactID  ";
            sql.CommandText += "inner join Customer on rfqCustomerID=Customer.CustomerID ";
            sql.CommandText += "inner join pktblVehicle on rfqVehicleID=vehVehicleID ";
            sql.CommandText += "inner join tblPart on ptrPartID=prtPartID ";
            sql.CommandText += "left outer join pktblBlankInfo on prtBlankInfoID=binBlankInfoID  ";
            sql.CommandText += "left outer join pktblMaterialType on prtPartMaterialType=mtyMaterialTypeID  ";
            sql.CommandText += "left outer join pktblPartType on prtPartTypeID=ptyPartTypeID  ";
            sql.CommandText += "left outer join tblNoQuote on prtPartID=nquPartID and nquCompanyID=linkRFQToCompany.rtqCompanyID  ";
            sql.CommandText += "where  prtPartId not in (select ptqPartID from linkPartToQuote)  ";
            sql.CommandText += " and  prtPartId not in (select prcPartId from linkpartReservedToCompany)  ";
            sql.CommandText += " and prtPartId not in (select ppdPartId from linkparttopartdetail where ppdPartToPartId in (select ppdPartToPartId from linkparttopartdetail, linkpartReservedToCompany where ppdPartId=prcPartId) )";
            sql.CommandText += " and rfqCreated > @start and rfqCreated < @end ";
            sql.CommandText += " Order by CustomerName, ShipToName, rfqid, prtRFQLineNumber, prtPartNumber, tsgCompanyAbbrev   ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
            sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));

            ldr = sql.ExecuteReader();


            while (ldr.Read())
            {
                if (ldr["prtPartNumber"].ToString() != HoldPart)
                {
                    if (HoldPart != "")
                    {
                        // Code for if BTS never responds
                        //if (NotRespondedList.Count == 1)
                        //{
                        //    if (NotRespondedList[0] == "BTS")
                        //    {
                        //        NoQuoteList.Add("BTS");
                        //        NotRespondedList.Clear();
                        //        AllNoQuotes = true;
                        //    }
                        //}
                        if (NotRespondedList.Count == 0)
                        {
                            if (NoQuoteList.Count == 0)
                            {
                                AllNoQuotes = false;
                            }
                        }
                        // if all companies were no quote, goes on the no quote sheet, otherwise the unreserved
                        if ((AllNoQuotes) || (TSGNoQuoted ))
                        {
                            rrow = GetOrCreateRow(noquoteSheet, nCurrentRow);
                            nCurrentRow++;
                            nQuoteCount++;
                            if (TSGNoQuoted)
                            {
                                foreach (String nr in NotRespondedList)
                                {
                                    if (!NoQuoteList.Contains(nr))
                                    {
                                        NoQuoteList.Add(nr);
                                    }
                                }
                                NotRespondedList.Clear();
                            }
                        }
                        else
                        {
                            rrow = GetOrCreateRow(unreservedSheet, uCurrentRow);
                            uCurrentRow++;
                            uQuoteCount++;
                        }
                        CreateSheetRow(rrow, newRow, NoQuoteList, NotRespondedList, EndDate, RightStyle, LateStyle, ReallyLateStyle, redFont, LinkFont, blackFont);
                    }
                    newRow = new EstimatingRow();
                    newRow.Salesman = ldr["Salesman"].ToString();
                    newRow.binMaterialPitchEnglish = ldr["binMaterialPitchEnglish"].ToString();
                    newRow.binMaterialPitchMetric = ldr["binMaterialPitchMetric"].ToString();
                    newRow.binMaterialThicknessEnglish = ldr["binMaterialThicknessEnglish"].ToString();
                    newRow.binMaterialThicknessMetric = ldr["binMaterialThicknessMetric"].ToString();
                    newRow.binMaterialWidthEnglish = ldr["binMaterialWidthEnglish"].ToString();
                    newRow.binMaterialWidthMetric = ldr["binMaterialWidthMetric"].ToString();
                    newRow.DueDate = ldr["DueDate"].ToString();
                    newRow.mtyMaterialType = ldr["mtyMaterialType"].ToString();
                    newRow.Name = ldr["Name"].ToString();
                    newRow.OEMName = ldr["OEMName"].ToString();
                    newRow.ProgramName = ldr["ProgramName"].ToString();
                    newRow.prtNote = ldr["prtNote"].ToString();
                    newRow.prtPartDescription = ldr["prtPartDescription"].ToString();
                    newRow.prtPartNumber = ldr["prtPartNumber"].ToString();
                    newRow.rfqCustomerRFQNumber = ldr["rfqCustomerRFQNumber"].ToString();
                    newRow.rfqDateReceived = ldr["rfqDateReceived"].ToString();
                    newRow.rfqid = ldr["rfqid"].ToString();
                    newRow.rfqMeetingNotes = ldr["rfqMeetingNotes"].ToString();
                    newRow.ShipToName = ldr["ShipToName"].ToString();
                    newRow.vehVehicleName = ldr["vehVehicleName"].ToString();

                    AllNoQuotes = true;
                    TSGNoQuoted = false;
                    NoQuoteList = new List<string>();
                    NotRespondedList = new List<string>();
                    HoldPart = ldr["prtPartNumber"].ToString();
                }
                // no quote, add to the no quote list otherwise not responded - 
                // if it is tsg, set boolean (because this forces it to the no quote sheet)
                if (ldr["nquCompanyID"].ToString() != "" && ldr["TSGCompanyID"].ToString() == ldr["nquCompanyID"].ToString())
                {
                    if (!NoQuoteList.Contains(ldr["tsgCompanyAbbrev"].ToString()))
                    {
                        NoQuoteList.Add(ldr["tsgCompanyAbbrev"].ToString());
                    }
                    if (ldr["tsgCompanyAbbrev"].ToString() == "TSG")
                    {
                        TSGNoQuoted = true;
                    }
                }
                else
                {
                    string testRFQID = ldr["rfqID"].ToString();
                    if (ldr["tsgCompanyAbbrev"].ToString() != "TSG")
                    {
                        AllNoQuotes = false;
                        if (!NotRespondedList.Contains(ldr["tsgCompanyAbbrev"].ToString()))
                        {
                            NotRespondedList.Add(ldr["tsgCompanyAbbrev"].ToString());
                        }
                    }
                }
            }

            if (HoldPart != "")
            {
                // if all companies were no quote, goes on the no quote sheet, otherwise the unreserved
                if ((AllNoQuotes) || (TSGNoQuoted))
                {
                    rrow = GetOrCreateRow(noquoteSheet, nCurrentRow);
                    nCurrentRow++;
                    nQuoteCount++;
                    if (TSGNoQuoted)
                    {
                        foreach (String nr in NotRespondedList)
                        {
                            if (!NoQuoteList.Contains(nr))
                            {
                                NoQuoteList.Add(nr);
                            }
                        }
                        NotRespondedList.Clear();
                    }
                }
                else
                {
                    rrow = GetOrCreateRow(unreservedSheet, uCurrentRow);
                    uCurrentRow++;
                    uQuoteCount++;
                }
                CreateSheetRow(rrow, newRow, NoQuoteList, NotRespondedList, EndDate, RightStyle, LateStyle, ReallyLateStyle, redFont, LinkFont, blackFont);
            }

            ldr.Close();

            if (nQuoteCount > 0) {
                rrow = GetOrCreateRow(noquoteSheet, nCurrentRow);
                rrow.CreateCell(3).SetCellValue(nQuoteCount.ToString());
                nCurrentRow++;
            }
            if (uQuoteCount > 0) {
                uCurrentRow++;
                rrow = GetOrCreateRow(noquoteSheet, nCurrentRow);
                rrow.CreateCell(3).SetCellValue(nQuoteCount.ToString());
            }

            unreservedSheet.CreateFreezePane(2, 3);
            noquoteSheet.CreateFreezePane(2, 3);
            unreservedSheet.ForceFormulaRecalculation = true;
            noquoteSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 25; i++)
            {
                unreservedSheet.AutoSizeColumn(i);
                noquoteSheet.AutoSizeColumn(i);
            }
            unreservedSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:Y3"));
            noquoteSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:Y3"));
            

            // NOW Do the OEM and STS Worksheet
            CreateOEMTitles(oemSheet, titleFont, EndDate, CenterStyle);
            CreateSTSTitles(stsSheet, titleFont, EndDate, CenterStyle);
            CreateUGSTitles(ugsSheet, titleFont, EndDate, CenterStyle);
            CreateOEMTitles(saSheet, titleFont, EndDate, CenterStyle);

            currentRow = 3;
            int stsRow = 3;
            int ugsRow = 3;
            int saRow = 3;

            List<OEMRow> oemRows = new List<OEMRow>();

            List<string> oemID = new List<string>();

            string test = StartDate.ToString("d");

            //Gets a list of all OEM IDs in the past 6 months ordered, this is so we can encorporate HTS and UGS to the OEM Tab
            sql.CommandText = "Select distinct OEMID from tblRFQ, OEM where rfqOEMID = OEMID and rfqCreated > @start and rfqCreated < @end order by OEMID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
            sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));
            SqlDataReader dr = sql.ExecuteReader();
            while(dr.Read())
            {
                oemID.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            String HoldOEM = "";
            QuoteCount = 0;
            for(int i = 0; i < oemID.Count; i++)
            {
                sql.CommandText = "select  concat(estFirstName,' ',estLastName) as Estimator, TSGSalesman.Name as Salesman, quoTotalAmount, CustomerName, quoLeadTime, ptyProductType, ptyPartTypeDescription, ";
                sql.CommandText += "tblDieInfo.*, pktblBlankInfo.*, cavCavityName, mtyMaterialType, quoAnnualVolume, vehVehicleName, tsgcompanyabbrev, quoQuoteID, rfqProgramID,  quoRFQID, ShipToName, rfqCustomerRFQNumber, ";
                sql.CommandText += "ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID, rfqDateReceived, coalesce(quoDueDate,rfqDueDate) as DueDate, prtPartNumber, prtPartDescription, prtNote, CustomerContact.Name, dtyFullName, ";
                sql.CommandText += "prtRFQLineNumber, quoVersion, quoToolingCost as DieCost, quoTransferBarCost as transferBar, quoFixtureCost as CheckFixture, quoShippingCost as shippingCost, quoDieSupportCost as homeLine, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToQuote, pktblPreWordedNote where pwqQuoteID = quoQuoteID and pwqPreWordedNoteID = pwnPreWordedNoteID) as total, qstQuoteStatusDescription ";
                sql.CommandText += "from tblquote  ";
                sql.CommandText += "inner join tsgcompany on tsgcompany.tsgcompanyid=quotsgcompanyid ";
                sql.CommandText += "inner join tblrfq on quorfqid=rfqid  ";
                sql.CommandText += "inner join oem on tblrfq.rfqOEMID=oem.oemid ";
                sql.CommandText += "inner join Program on rfqProgramID=ProgramID  ";
                sql.CommandText += "inner join CustomerLocation on rfqPlantID=CustomerLocationID ";
                sql.CommandText += "inner join linkPartToRFQ on rfqid=ptrRFQID  ";
                sql.CommandText += "inner join CustomerContact on  rfqCustomerContact=customerContactID ";
                sql.CommandText += "inner join tblPart on ptrPartID=prtPartID ";
                sql.CommandText += "inner join linkDieInfoToQuote on quoQuoteID=diqQuoteID ";
                sql.CommandText += "inner join tblDieInfo on diqDieInfoID = dinDieInfoID  ";
                sql.CommandText += "inner join DieType on dinDieType = DieTypeID ";
                sql.CommandText += "inner join pktblCavity on dinCavityID= cavCavityID ";
                sql.CommandText += "inner join pktblBlankInfo on quoBlankInfoID=binBlankInfoId ";
                sql.CommandText += "inner join pktblMaterialType on binBlankMaterialTypeID=mtyMaterialTypeID ";
                sql.CommandText += "inner join pktblVehicle on rfqVehicleID = vehVehicleID ";
                sql.CommandText += "inner join pktblProductType on rfqProductTypeId=ptyProductTypeID ";
                sql.CommandText += "inner join pktblPartType on prtPartTypeId=ptyPartTypeID ";
                sql.CommandText += "inner join Customer on rfqCustomerID=Customer.CustomerID ";
                sql.CommandText += "inner join TSGSalesman on rfqSalesman=TSGSalesman.TSGSalesmanID ";
                sql.CommandText += "inner join pktblestimators on quoEstimatorID=estEstimatorID ";
                sql.CommandText += "inner join linkPartToQuote on prtPartID=ptqPartID and ptqQuoteID = quoQuoteID ";
                sql.CommandText += "inner join pktblQuoteStatus on quoStatusID = qstQuoteStatusID ";
                sql.CommandText += "where rfqCreated > @start and rfqCreated < @end and rfqOEMID = @oem ";
                if (Company != 1)
                {
                    sql.CommandText += "and quoTSGCompanyID = @company ";
                }
                sql.CommandText += "Order by OEMName, vehVehicleName,ProgramName, CustomerContact.Name, rfqid, quoQuoteID ";
                sql.Parameters.Clear();

                if (Company != 1)
                {
                    sql.Parameters.AddWithValue("@company", Company);
                }

                sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
                sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));
                sql.Parameters.AddWithValue("@oem", oemID[i]);

                ldr = sql.ExecuteReader();

                while (ldr.Read())
                {
                    OEMRow orow = new OEMRow();
                    orow.OEMName = ldr["OEMName"].ToString();
                    orow.vehVehicleName = ldr["vehVehicleName"].ToString();
                    orow.programName = ldr["ProgramName"].ToString();
                    orow.rfqid = ldr["quoRFQID"].ToString();
                    orow.quoteid = ldr["quoRFQID"].ToString() + "-" + ldr["prtRFQLineNumber"].ToString() + "-" + ldr["tsgCompanyAbbrev"].ToString() + "-" + ldr["quoVersion"].ToString();
                    orow.company = ldr["tsgCompanyAbbrev"].ToString();
                    orow.dateReceived = ldr["rfqDateReceived"].ToString().Split(' ')[0];
                    orow.dueDate = ldr["DueDate"].ToString().Split(' ')[0];
                    orow.leadTime = ldr["quoLeadTime"].ToString();
                    orow.rfqMeetingNotes = ldr["rfqMeetingNotes"].ToString() + " " + ldr["prtNote"].ToString();
                    orow.shipToName = ldr["ShipToName"].ToString();
                    orow.customerName = ldr["CustomerName"].ToString();
                    orow.partNumber = ldr["prtPartNumber"].ToString();
                    orow.partDescription = ldr["prtPartDescription"].ToString();
                    orow.dieType = ldr["dtyFullName"].ToString();
                    orow.cavity = ldr["cavCavityName"].ToString();
                    orow.salesman = ldr["Salesman"].ToString();
                    orow.estimator = ldr["Estimator"].ToString();
                    orow.FTBEnglish = ldr["dinSizeFrontToBackEnglish"].ToString();
                    orow.FTBMetric = ldr["dinSizeFrontToBackMetric"].ToString();
                    orow.LTREnglish = ldr["dinSizeLeftToRightEnglish"].ToString();
                    orow.LTRMetric = ldr["dinSizeLeftToRightMetric"].ToString();
                    orow.shutHeightEnglish = ldr["dinSizeShutHeightEnglish"].ToString();
                    orow.shutHeightMetric = ldr["dinSizeShutHeightMetric"].ToString();
                    orow.numberOfStations = ldr["dinNumberOfStations"].ToString();
                    orow.widthEnglish = ldr["binMaterialWidthEnglish"].ToString();
                    orow.widthMetric = ldr["binMaterialWidthMetric"].ToString();
                    orow.pitchEnglish = ldr["binMaterialPitchEnglish"].ToString();
                    orow.pitchMetric = ldr["binMaterialPitchMetric"].ToString();
                    orow.thicknessEnglish = ldr["binMaterialThicknessEnglish"].ToString();
                    orow.thicknessMetric = ldr["binMaterialThicknessMetric"].ToString();
                    orow.materialType = ldr["mtyMaterialType"].ToString();
                    orow.annualVolume = ldr["quoAnnualVolume"].ToString();
                    orow.customerRFQ = ldr["rfqCustomerRFQNumber"].ToString();
                    orow.contactName = ldr["Name"].ToString();
                    orow.leadTime = ldr["quoLeadTime"].ToString();
                    orow.dieCost = ldr["DieCost"].ToString();
                    orow.transferBar = ldr["transferBar"].ToString();
                    orow.checkFixture = ldr["CheckFixture"].ToString();
                    orow.shippingCost = ldr["shippingCost"].ToString();
                    orow.homeLine = ldr["homeLine"].ToString();
                    orow.totalCost = ldr["total"].ToString();
                    orow.winLoss = ldr["qstQuoteStatusDescription"].ToString();
                    oemRows.Add(orow);
                }
                ldr.Close();


                if (Company == 1 || Company == 9)
                {
                   sql.CommandText = "Select OEMName, vehVehicleName, ProgramName, rfqID, hquHTSQuoteID, 'HTS' as abbrev, rfqDateReceived, rfqDueDate as DueDate, hquLeadTime, rfqMeetingNotes, prtNote, ShipToName, ";
                   sql.CommandText += "CustomerName, prtPartNumber, prtpartDescription, dtyFullName, cavCavityName, TSGSalesman.Name as Salesman, concat(estFirstName, ' ', estLastName) as Estimator, ";
                   sql.CommandText += "hquMaterialType, rfqCustomerRFQNumber, Customercontact.Name, hquVersion, prtRFQLineNumber, (Select sum(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote ";
                   sql.CommandText += "where hpwHTSPreWordedNoteID = pthHTSPWNID and pthHTSQuoteID = hquHTSQuoteID) as totalCost ";
                   sql.CommandText += "from tblHTSQuote, linkQuoteToRFQ, tblRFQ, OEM, Program, pktblVehicle, Customer, CustomerLocation, CustomerContact, pktblEstimators, linkPartToQuote, tblPart, pktblCavity, DieType, ";
                   sql.CommandText += "TSGSalesman ";
                   sql.CommandText += "where hquHTSQuoteID = qtrQuoteID and qtrHTS = 1 and qtrRFQID = rfqID and rfqOEMID = OEMID and ProgramID = rfqProgramID and rfqVehicleID = vehVehicleID ";
                   sql.CommandText += "and rfqCustomerID = Customer.CustomerID and CustomerLocationID = rfqPlantID and rfqCustomerContact = CustomerContactID and hquEstimatorID = estEstimatorID ";
                   sql.CommandText += "and ptqQuoteId = hquHTSQuoteID and ptqHTS = 1 and ptqPartID = prtPartID and hquProcess = DieTypeID and hquCavity = cavCavityID and rfqSalesman = TSGSalesman.TSGSalesmanID ";
                   sql.CommandText += "and rfqCreated > @start and rfqCreated < @end and rfqOEMID = @oem ";
                   sql.Parameters.Clear();
                   sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
                   sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));
                   sql.Parameters.AddWithValue("@oem", oemID[i]);

                   ldr = sql.ExecuteReader();

                    while (ldr.Read())
                    {
                        OEMRow orow = new OEMRow();
                        orow.OEMName = ldr["OEMName"].ToString();
                        orow.vehVehicleName = ldr["vehVehicleName"].ToString();
                        orow.programName = ldr["ProgramName"].ToString();
                        orow.rfqid = ldr["rfqID"].ToString();
                        orow.quoteid = ldr["rfqID"].ToString() + "-" + ldr["prtRFQLineNumber"].ToString() + "-HTS-" + ldr["hquVersion"].ToString();
                        orow.company = ldr["abbrev"].ToString();
                        orow.dateReceived = ldr["rfqDateReceived"].ToString().Split(' ')[0];
                        orow.dueDate = ldr["DueDate"].ToString().Split(' ')[0];
                        orow.leadTime = ldr["hquLeadTime"].ToString();
                        orow.rfqMeetingNotes = ldr["rfqMeetingNotes"].ToString() + " " + ldr["prtNote"].ToString();
                        orow.shipToName = ldr["ShipToName"].ToString();
                        orow.customerName = ldr["CustomerName"].ToString();
                        orow.partNumber = ldr["prtPartNumber"].ToString();
                        orow.partDescription = ldr["prtPartDescription"].ToString();
                        orow.dieType = ldr["dtyFullName"].ToString();
                        orow.cavity = ldr["cavCavityName"].ToString();
                        orow.salesman = ldr["Salesman"].ToString();
                        orow.estimator = ldr["Estimator"].ToString();
                        orow.FTBEnglish = "";
                        orow.FTBMetric = "";
                        orow.LTREnglish = "";
                        orow.LTRMetric = "";
                        orow.shutHeightEnglish = "";
                        orow.shutHeightMetric = "";
                        orow.numberOfStations = "";
                        orow.widthEnglish = "";
                        orow.widthMetric = "";
                        orow.pitchEnglish = "";
                        orow.pitchMetric = "";
                        orow.thicknessEnglish = "";
                        orow.thicknessMetric = "";
                        orow.materialType = ldr["hquMaterialType"].ToString();
                        orow.annualVolume = "";
                        orow.customerRFQ = ldr["rfqCustomerRFQNumber"].ToString();
                        orow.contactName = ldr["Name"].ToString();
                        orow.totalCost = ldr["totalCost"].ToString();
                        oemRows.Add(orow);
                    }
                  ldr.Close();
                }
            }

            //Doing STS outside of the loop since we want all of their Stand alone quotes and their quotes associated with RFQS and they go on a seperate tab

            if (Company == 1 || Company == 13)
            {
                sql.CommandText = "Select OEMName, vehVehicleName, ProgramName, rfqID, squSTSQuoteID, 'STS' as abbrev, rfqDateReceived, rfqDueDate as DueDate, squLeadTime, rfqMeetingNotes, prtNote, ShipToName, CustomerName, rfqCustomerRFQNumber, ";
                sql.CommandText += "squPartNumber, squPartName, TSGSalesman.Name as Salesman, concat(estFirstName, ' ', estLastName) as Estimator, squEAV, squProcess, squMachineTime, squCustomerRFQNum, squCustomerContact, CustomerContact.Name, ";
                sql.CommandText += "GETDATE() as date, squQuoteVersion, prtRFQLineNumber, (Select sum(pwnCostNote) from pktblPreWordedNote, linkPWNToSTSQuote where squSTSQuoteID = psqSTSQuoteID and pwnPreWordedNoteID = psqPreWordedNoteID) as totalCost ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and (Select top 1 ptqPartID from linkPartToQuote where ptqQuoteID = squSTSQuoteID order by ptqPartID) = ptqPartID and ptqSTS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                sql.CommandText += "left outer join tblRFQ on qtrRFQID = rfqID ";
                sql.CommandText += "left outer join OEM on rfqOEMID = OEMID ";
                sql.CommandText += "left outer join pktblVehicle on rfqVehicleID = vehVehicleID ";
                sql.CommandText += "left outer join Program on rfqProgramID = ProgramID ";
                sql.CommandText += "left outer join Customer on squCustomerID = Customer.CustomerID ";
                sql.CommandText += "left outer join CustomerLocation on squPlantID = CustomerLocationID ";
                sql.CommandText += "left outer join pktblEstimators on squEstimatorID = estEstimatorID ";
                sql.CommandText += "left outer join TSGSalesman on squSalesmanID = TSGSalesman.TSGSalesmanID ";
                sql.CommandText += "left outer join CustomerContact on rfqCustomerContact = CustomerContactID ";
                sql.CommandText += "where squCreated > @start and squCreated < @end ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
                sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));

                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    OEMRow orow = new OEMRow();
                    orow.OEMName = dr["OEMName"].ToString();
                    orow.vehVehicleName = dr["vehVehicleName"].ToString();
                    orow.programName = dr["ProgramName"].ToString();
                    orow.rfqid = dr["rfqID"].ToString();
                    if (dr["rfqID"].ToString() != "")
                    {
                        orow.quoteid = dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                    }
                    else
                    {
                        orow.quoteid = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                    }
                    orow.company = dr["abbrev"].ToString();
                    orow.dateReceived = dr["rfqDateReceived"].ToString().Split(' ')[0];
                    if (dr["DueDate"].ToString().Split(' ')[0] != "")
                    {
                        orow.dueDate = dr["DueDate"].ToString().Split(' ')[0];
                    }
                    else
                    {
                        orow.dueDate = dr["date"].ToString().Split(' ')[0];
                    }
                    orow.leadTime = dr["squLeadTime"].ToString();
                    orow.rfqMeetingNotes = dr["rfqMeetingNotes"].ToString() + " " + dr["prtNote"].ToString();
                    orow.shipToName = dr["ShipToName"].ToString();
                    orow.customerName = dr["CustomerName"].ToString();
                    orow.partNumber = dr["squPartNumber"].ToString();
                    orow.partDescription = dr["squPartName"].ToString();
                    orow.dieType = dr["squProcess"].ToString();
                    orow.cavity = dr["squMachineTime"].ToString();
                    orow.salesman = dr["Salesman"].ToString();
                    orow.estimator = dr["Estimator"].ToString();
                    orow.FTBEnglish = dr["squEAV"].ToString();
                    orow.FTBMetric = "";
                    orow.LTREnglish = "";
                    orow.LTRMetric = "";
                    orow.shutHeightEnglish = "";
                    orow.shutHeightMetric = "";
                    orow.numberOfStations = "";
                    orow.widthEnglish = "";
                    orow.widthMetric = "";
                    orow.pitchEnglish = "";
                    orow.pitchMetric = "";
                    orow.thicknessEnglish = "";
                    orow.thicknessMetric = "";
                    orow.materialType = "";
                    orow.annualVolume = "";
                    if (dr["rfqCustomerRFQNumber"].ToString() != "")
                    {
                        orow.customerRFQ = dr["rfqCustomerRFQNumber"].ToString();
                    }
                    else
                    {
                        orow.customerRFQ = dr["squCustomerRFQNum"].ToString();
                    }
                    if (dr["Name"].ToString() != "")
                    {
                        orow.contactName = dr["Name"].ToString();
                    }
                    else
                    {
                        orow.contactName = dr["squCustomerContact"].ToString();
                    }
                    orow.totalCost = dr["totalCost"].ToString();
                    oemRows.Add(orow);
                }
                dr.Close();
            }
            if (Company == 1 || Company == 15)
            {
                sql.CommandText = "Select OEMName, vehVehicleName, ProgramName, rfqID, uquUGSQuoteID, 'UGS' as abbrev, rfqDateReceived, rfqDueDate as DueDate, uquLeadTime, rfqMeetingNotes, prtNote, ShipToName, CustomerName, rfqCustomerRFQNumber,  ";
                sql.CommandText += "uquPartNumber, uquPartName, TSGSalesman.Name as Salesman, concat(estFirstName, ' ', estLastName) as Estimator, uquCustomerRFQNumber, uquCustomerContact, CustomerContact.Name,  ";
                sql.CommandText += "GETDATE() as date, uquQuoteVersion, prtRFQLineNumber, uquTotalPrice ";
                sql.CommandText += "from tblUGSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = uquUGSQuoteID and qtrUGS = 1 ";
                sql.CommandText += "left outer join tblRFQ on qtrRFQID = rfqID ";
                sql.CommandText += "left outer join OEM on rfqOEMID = OEMID ";
                sql.CommandText += "left outer join pktblVehicle on rfqVehicleID = vehVehicleID ";
                sql.CommandText += "left outer join Program on rfqProgramID = ProgramID ";
                sql.CommandText += "left outer join Customer on uquCustomerID = Customer.CustomerID ";
                sql.CommandText += "left outer join CustomerLocation on uquPlantID = CustomerLocationID ";
                sql.CommandText += "left outer join pktblEstimators on uquEstimatorID = estEstimatorID ";
                sql.CommandText += "left outer join TSGSalesman on uquSalesmanID = TSGSalesman.TSGSalesmanID ";
                sql.CommandText += "left outer join CustomerContact on rfqCustomerContact = CustomerContactID ";
                sql.CommandText += "where uquCreated > @start and uquCreated < @end ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@start", StartDate.ToString("d"));
                sql.Parameters.AddWithValue("@end", EndDate.ToString("d"));


                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    OEMRow orow = new OEMRow();
                    orow.OEMName = dr["OEMName"].ToString();
                    orow.vehVehicleName = dr["vehVehicleName"].ToString();
                    orow.programName = dr["ProgramName"].ToString();
                    orow.rfqid = dr["rfqID"].ToString();
                    if (dr["rfqID"].ToString() != "")
                    {
                        orow.quoteid = dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString();
                    }
                    else
                    {
                        orow.quoteid = dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                    }
                    orow.company = dr["abbrev"].ToString();
                    orow.dateReceived = dr["rfqDateReceived"].ToString().Split(' ')[0];
                    if (dr["DueDate"].ToString().Split(' ')[0] != "")
                    {
                        orow.dueDate = dr["DueDate"].ToString().Split(' ')[0];
                    }
                    else
                    {
                        orow.dueDate = dr["date"].ToString().Split(' ')[0];
                    }
                    orow.leadTime = dr["uquLeadTime"].ToString();
                    orow.rfqMeetingNotes = dr["rfqMeetingNotes"].ToString() + " " + dr["prtNote"].ToString();
                    orow.shipToName = dr["ShipToName"].ToString();
                    orow.customerName = dr["CustomerName"].ToString();
                    orow.partNumber = dr["uquPartNumber"].ToString();
                    orow.partDescription = dr["uquPartName"].ToString();
                    orow.dieType = "";
                    orow.cavity = "";
                    orow.salesman = dr["Salesman"].ToString();
                    orow.estimator = dr["Estimator"].ToString();
                    orow.FTBEnglish = "";
                    orow.FTBMetric = "";
                    orow.LTREnglish = "";
                    orow.LTRMetric = "";
                    orow.shutHeightEnglish = "";
                    orow.shutHeightMetric = "";
                    orow.numberOfStations = "";
                    orow.widthEnglish = "";
                    orow.widthMetric = "";
                    orow.pitchEnglish = "";
                    orow.pitchMetric = "";
                    orow.thicknessEnglish = "";
                    orow.thicknessMetric = "";
                    orow.materialType = "";
                    orow.annualVolume = "";
                    if (dr["rfqCustomerRFQNumber"].ToString() != "")
                    {
                        orow.customerRFQ = dr["rfqCustomerRFQNumber"].ToString();
                    }
                    else
                    {
                        orow.customerRFQ = dr["uquCustomerRFQNumber"].ToString();
                    }
                    if (dr["Name"].ToString() != "")
                    {
                        orow.contactName = dr["Name"].ToString();
                    }
                    else
                    {
                        orow.contactName = dr["uquCustomerContact"].ToString();
                    }
                    orow.totalCost = dr["uquTotalPrice"].ToString();
                    string temp = dr["uquTotalPrice"].ToString();
                    oemRows.Add(orow);
                }
                dr.Close();
            }

                sql.CommandText = "Select ecqECQuoteID, ecqPartNumber, ecqPartName, ecqRFQNumber, CustomerName, ShipToName, ecqCustomerRFQNumber, cavCavityName, dtyFullName, ecqBlankWidthEng, ecqBlankPitchEng, ";
                sql.CommandText += "ecqMaterialThkMet, ecqDieFBEng, ecqDieLREng, ecqShutHeightEng, mtyMaterialType, ecqNumberOfStations, ecqLeadTime, ecqCustomerContactName, ecqVersion, ecqQuoteNumber, tsgCompanyAbbrev, ";
                sql.CommandText += "ecqCreated, TSGSalesman.Name as Salesman, concat(estFirstName, ' ', estLastName) as Estimator, ecqBlankWidthMet, ecqBlankPitchMet, ecqMaterialThkEng, ecqDieFBMet, ecqDieLRMet, ecqShutHeightMet, ";
                sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToECQuote, pktblPreWordedNote where pwnPreWordedNoteID = peqPreWordedNoteID and peqECQuoteID = ecqECQuoteID) as totalCost ";
                sql.CommandText += "from tblECQuote, Customer, CustomerLocation, DieType, pktblCavity, pktblMaterialType, TSGCompany, TSGSalesman, pktblEstimators ";
                sql.CommandText += "where ecqCustomer = Customer.CustomerID and ecqCustomerLocation = CustomerLocationID and DieTypeID = ecqDieType and cavCavityID = ecqCavity and ecqMaterialType = mtyMaterialTypeID ";
                sql.CommandText += "and TSGCompany.TSGCompanyID = ecqTSGCompanyID and TSGSalesman.TSGSalesmanID = ecqSalesmanID and ecqEstimator = estEstimatorID ";
                if (Company != 1)
                {
                    sql.CommandText += "and ecqTSGCompanyID = @company1 ";
                    sql.Parameters.AddWithValue("@company1", Company);
                }


                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    OEMRow orow = new OEMRow();
                    orow.OEMName = "";
                    orow.vehVehicleName = "";
                    orow.programName = "";
                    orow.rfqid = "";
                    orow.quoteid = dr["ecqECQuoteID"].ToString() + "-" + dr["tsgCompanyAbbrev"].ToString() + "-SA-" + dr["ecqVersion"].ToString();
                    orow.company = dr["tsgCompanyAbbrev"].ToString();
                    orow.dateReceived = "";
                    orow.dueDate = dr["ecqCreated"].ToString().Split(' ')[0];
                    orow.leadTime = dr["ecqLeadTime"].ToString();
                    orow.rfqMeetingNotes = "";
                    orow.shipToName = dr["ShipToName"].ToString();
                    orow.customerName = dr["CustomerName"].ToString();
                    orow.partNumber = dr["ecqPartNumber"].ToString();
                    orow.partDescription = dr["ecqPartName"].ToString();
                    orow.dieType = dr["dtyFullName"].ToString();
                    orow.cavity = dr["cavCavityName"].ToString();
                    orow.salesman = dr["Salesman"].ToString();
                    orow.estimator = dr["Estimator"].ToString();
                    orow.FTBEnglish = dr["ecqDieFBEng"].ToString();
                    orow.FTBMetric = dr["ecqDieFBMet"].ToString();
                    orow.LTREnglish = dr["ecqDieLREng"].ToString();
                    orow.LTRMetric = dr["ecqDieLRMet"].ToString();
                    orow.shutHeightEnglish = dr["ecqShutHeightEng"].ToString();
                    orow.shutHeightMetric = dr["ecqShutHeightMet"].ToString();
                    orow.numberOfStations = dr["ecqNumberOfStations"].ToString();
                    orow.widthEnglish = dr["ecqBlankWidthEng"].ToString();
                    orow.widthMetric = dr["ecqBlankWidthMet"].ToString();
                    orow.pitchEnglish = dr["ecqBlankPitchEng"].ToString();
                    orow.pitchMetric = dr["ecqBlankPitchMet"].ToString();
                    orow.thicknessEnglish = dr["ecqMaterialThkEng"].ToString();
                    orow.thicknessMetric = dr["ecqMaterialThkMet"].ToString();
                    orow.materialType = dr["mtyMaterialType"].ToString();
                    orow.annualVolume = "";
                    orow.customerRFQ = dr["ecqCustomerRFQNumber"].ToString();
                    orow.contactName = dr["ecqCustomerContactName"].ToString();
                    orow.leadTime = dr["ecqLeadTime"].ToString();
                    orow.totalCost = dr["totalCost"].ToString();
                    oemRows.Add(orow);
                }
                dr.Close();

            if (Company == 1 || Company == 9)
            {

                sql.CommandText = "Select hquHTSQuoteID, hquPartNumbers, hquPartName, CustomerName, ShipToName, hquCustomerRFQNum, cavCavityName, dtyFullName, hquCreated as DueDate, hquLeadTime, TSGSalesman.Name as Salesman, ";
                sql.CommandText += "concat(estFirstName, ' ', estLastName) as Estimator, hquMaterialType, hquCustomerContactName, hquVersion, (Select sum(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote ";
                sql.CommandText += "where hpwHTSPreWordedNoteID = pthHTSPWNID and pthHTSQuoteID = hquHTSQuoteID) as totalCost ";
                sql.CommandText += "from tblHTSQuote, Customer, CustomerLocation, pktblEstimators, TSGSalesman, pktblCavity, DieType ";
                sql.CommandText += "where hquRFQID = '' and hquCustomerID = Customer.CustomerID and hquCustomerLocationID = CustomerLocationID and hquCavity = cavCavityID and hquProcess = DieTypeID and hquEstimatorID = estEstimatorID ";
                sql.CommandText += "and hquSalesman = CustomerLocation.TSGSalesmanID and hquSalesman = TSGSalesman.TSGSalesmanID";

                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    OEMRow orow = new OEMRow();
                    orow.OEMName = "";
                    orow.vehVehicleName = "";
                    orow.programName = "";
                    orow.rfqid = "";
                    orow.quoteid = dr["hquHTSQuoteID"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString();
                    orow.company = "HTS";
                    orow.dateReceived = "";
                    orow.dueDate = dr["DueDate"].ToString().Split(' ')[0];
                    orow.leadTime = dr["hquLeadTime"].ToString();
                    orow.rfqMeetingNotes = "";
                    orow.shipToName = dr["ShipToName"].ToString();
                    orow.customerName = dr["CustomerName"].ToString();
                    orow.partNumber = dr["hquPartNumbers"].ToString();
                    orow.partDescription = dr["hquPartName"].ToString();
                    orow.dieType = dr["dtyFullName"].ToString();
                    orow.cavity = dr["cavCavityName"].ToString();
                    orow.salesman = dr["Salesman"].ToString();
                    orow.estimator = dr["Estimator"].ToString();
                    orow.FTBEnglish = "";
                    orow.FTBMetric = "";
                    orow.LTREnglish = "";
                    orow.LTRMetric = "";
                    orow.shutHeightEnglish = "";
                    orow.shutHeightMetric = "";
                    orow.numberOfStations = "";
                    orow.widthEnglish = "";
                    orow.widthMetric = "";
                    orow.pitchEnglish = "";
                    orow.pitchMetric = "";
                    orow.thicknessEnglish = "";
                    orow.thicknessMetric = "";
                    orow.materialType = dr["hquMaterialType"].ToString();
                    orow.annualVolume = "";
                    orow.customerRFQ = dr["hquCustomerRFQNum"].ToString();
                    orow.contactName = dr["hquCustomerContactName"].ToString();
                    orow.totalCost = dr["totalCost"].ToString();
                    oemRows.Add(orow);
                }
                dr.Close();
            }
            String holdCustomer = "";
            String holdShipTo = "";
            string holdSalesman = "";
            foreach (OEMRow orow in oemRows)
            {
                if (orow.salesman == "")
                {
                    if ((holdCustomer != orow.customerName) || (holdShipTo != orow.shipToName))
                    {
                        holdSalesman = "";
                        sql.CommandText = "select TSGSalesman.Name from customer, customerLocation, tsgsalesman where  CustomerName='" + orow.customerName + "' and customer.customerID=customerlocation.customerid and ShipToName='" + orow.shipToName + "' and customerlocation.tsgsalesmanid=tsgsalesman.tsgsalesmanid ";
                        ldr = sql.ExecuteReader();
                        while (ldr.Read())
                        {
                            holdSalesman = ldr["Name"].ToString();
                        }
                        ldr.Close();
                    }
                    orow.salesman = holdSalesman;
                }
            }
            connection.Close();

            oemRows.OrderBy(p => p.OEMName).ThenBy(p => p.vehVehicleName).ThenBy(p => p.programName).ThenBy(p => p.company).ThenBy(p => p.customerName).ThenBy(p => p.shipToName);

            int stsQuoteCount = 0;
            int ugsQuoteCount = 0;
            int saQuoteCount = 0;
            String LastCompany = "";
            foreach (OEMRow orow in oemRows)
            {
                if (orow.OEMName != HoldOEM)
                {
                    if (HoldOEM != "")
                    {
                        rrow = GetOrCreateRow(oemSheet, currentRow);
                        rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                        currentRow++;
                        if (orow.company == "STS")
                        {
                            rrow = GetOrCreateRow(stsSheet, stsRow);
                            rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                            currentRow++;
                        }
                        else if (orow.company == "UGS")
                        {
                            rrow = GetOrCreateRow(ugsSheet, ugsRow);
                            rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                            currentRow++;
                        }
                        else if (orow.quoteid.Contains("SA"))
                        {
                            rrow = GetOrCreateRow(saSheet, saRow);
                            rrow.CreateCell(3).SetCellValue(saQuoteCount.ToString());
                            currentRow++;
                        }
                    }
                    HoldOEM = orow.OEMName;
                    QuoteCount = 0;
                    stsQuoteCount = 0;
                }
                CreateOEMRow(oemSheet, currentRow, orow, LateStyle, ReallyLateStyle, RightStyle, EndDate, LinkFont);
                QuoteCount++;
                currentRow++;
                if (orow.company == "STS")
                {
                    CreateSTSRow(stsSheet, stsRow, orow, LateStyle, ReallyLateStyle, RightStyle, EndDate, LinkFont);
                    stsQuoteCount++;
                    stsRow++;
                }
                else if (orow.company == "UGS")
                {
                    createUGSRow(ugsSheet, ugsRow, orow, LateStyle, ReallyLateStyle, RightStyle, EndDate, LinkFont);
                    ugsQuoteCount++;
                    ugsRow++;
                }
                else if (orow.quoteid.Contains("SA"))
                {
                    CreateOEMRow(saSheet, saRow, orow, LateStyle, ReallyLateStyle, RightStyle, EndDate, LinkFont);
                    saQuoteCount++;
                    saRow++;
                }
            }
            ldr.Close();

            connection.Close();

            if (QuoteCount > 0)
            {
                rrow = GetOrCreateRow(oemSheet, currentRow);
                rrow.CreateCell(3).SetCellValue(QuoteCount.ToString());
                currentRow++;
            }

            if (stsQuoteCount > 0)
            {
                rrow = GetOrCreateRow(stsSheet, stsRow);
                rrow.CreateCell(3).SetCellValue(stsQuoteCount.ToString());
                stsRow++;
            }

            if(ugsQuoteCount > 0)
            {
                rrow = GetOrCreateRow(ugsSheet, ugsRow);
                rrow.CreateCell(3).SetCellValue(ugsQuoteCount.ToString());
                ugsRow++;
            }

            oemSheet.CreateFreezePane(2, 3);
            oemSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 57; i++)
            {
                oemSheet.AutoSizeColumn(i);
            }
            stsSheet.CreateFreezePane(2, 3);
            stsSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 57; i++)
            {
                stsSheet.AutoSizeColumn(i);
            }

            ugsSheet.CreateFreezePane(2, 3);
            ugsSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 57; i++)
            {
                ugsSheet.AutoSizeColumn(i);
            }

            saSheet.CreateFreezePane(2, 3);
            saSheet.ForceFormulaRecalculation = true;
            for (int i = 2; i < 57; i++)
            {
                saSheet.AutoSizeColumn(i);
            }

            oemSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:BA3"));
            stsSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:BA3"));
            ugsSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:BA3"));
            saSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("C3:BA3"));

            connection.Close();

            //context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "RFQ-QUOTE" + StartDate.ToString("d").Replace("/", "-") + " to " + EndDate.ToString("d").Replace("/", "-") + ".xlsx"));
            //context.Response.Clear();
            using (var ms = new System.IO.MemoryStream())
            {
                wb.Write(ms);
                //context.Response.BinaryWrite(ms.ToArray());
                //context.Response.End();
                MemoryStream ms2 = new MemoryStream(ms.ToArray());

                SmtpClient server = new SmtpClient("smtp.office365.com");
                server.UseDefaultCredentials = false;
                server.Port = 587;
                server.EnableSsl = true;
                // TODO send as another user
                server.Credentials = master.getNetworkCredentials();
                server.Timeout = 50000;
                server.TargetName = "STARTTLS/smtp.office365.com";
                MailMessage mail = new MailMessage();

                mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, "RFQ-QUOTE" + StartDate.ToString("d").Replace("/", "-") + " to " + EndDate.ToString("d").Replace("/", "-") + ".xlsx"));

                //mail.Attachments.Add(System.Net.Mail.Attachment(ms, "RFQ-QUOTE" + StartDate.ToString("d").Replace("/", "-") + " to " + EndDate.ToString("d").Replace("/", "-") + ".xlsx"));


                mail.From = master.getFromAddress();
                if(master.getUserName() == "chris@netinflux.com")
                {
                    mail.To.Add(new MailAddress("rmumford@toolingsystemsgroup.com"));
                }
                else
                {
                    mail.To.Add(new MailAddress(master.getUserName()));
                }
                mail.Subject = "RFQ Weekly Report";
                mail.Body = "Attached is the weekly report on quoting activity.<br />";
                mail.Body += "Please visit https://tsgrfq.azurewebsites.net/Reporting to view any of the graphs.<br />It will take around 15 seconds to load the webpage.";
                //mail.Attachments.Add(attach);
                mail.IsBodyHtml = true;
                try
                {
                    server.Send(mail);
                }
                catch
                {
                    try
                    {
                        server.Send(mail);
                    }
                    catch (Exception err)
                    {

                    }
                }
            }
        }

        public void CreateSheetRow(NPOI.SS.UserModel.IRow rrow, EstimatingRow eRow, List<String> NoQuoteList, List<String>NotRespondedList, DateTime EndDate, XSSFCellStyle RightStyle, XSSFCellStyle LateStyle, XSSFCellStyle ReallyLateStyle, XSSFFont redFont, XSSFFont LinkFont, XSSFFont blackFont)
        {
            NPOI.SS.UserModel.ICell cell = rrow.CreateCell(2);
            cell.SetCellValue(eRow.rfqid);
            NPOI.XSSF.UserModel.XSSFHyperlink link = new XSSFHyperlink(NPOI.SS.UserModel.HyperlinkType.Url);
            link.Address = ("https://tsgrfq.azurewebsites.net/EditRFQ?id=" + eRow.rfqid.ToString());
            cell.Hyperlink = (link);
            cell.RichStringCellValue.ApplyFont(0, eRow.rfqid.ToString().Length, LinkFont);
            String HoldCompany = "";

            int end = 0;
            foreach (String company in NoQuoteList)
            {
                if (HoldCompany.Length > 0)
                {
                    HoldCompany += ", ";
                    end += 2;
                }
                HoldCompany += company;
                end += company.Length;
            }
            // this is all the no quotes
            if(NotRespondedList.Count != 0)
            {
                HoldCompany = "";
            }

            foreach (String company in NotRespondedList)
            {
                if (HoldCompany.Length > 0)
                {
                    HoldCompany += ", ";
                }
                HoldCompany += company;
            }
            rrow.CreateCell(4).SetCellValue(HoldCompany);

            //if(end != 0)
            //{
            //rrow.GetCell(4).RichStringCellValue.ApplyFont(0, end, redFont);
            //}
            //else
            //{
            //    rrow.GetCell(4).RichStringCellValue.ApplyFont(0, HoldCompany.Length, blackFont);
            //}
            //if(end != HoldCompany.Length)
            //{
            //}
            rrow.GetCell(4).RichStringCellValue.ApplyFont(0, HoldCompany.Length, blackFont);


            rrow.CreateCell(5).SetCellValue(eRow.rfqDateReceived.Split(' ')[0]);
            rrow.CreateCell(6).SetCellValue(eRow.DueDate.ToString().Split(' ')[0]);
            String DueDate = eRow.DueDate;
            DateTime RFQSent = EndDate;
            XSSFCellStyle DayStyle = RightStyle;
            Double DaysLate = (RFQSent - System.Convert.ToDateTime(eRow.DueDate)).TotalDays;
            rrow.CreateCell(7).SetCellValue(DaysLate.ToString());
            if (DaysLate > 0)
            {
                if (DaysLate > 7)
                {
                    rrow.GetCell(7).CellStyle = ReallyLateStyle;
                }
                else
                {
                    rrow.GetCell(7).CellStyle = LateStyle;

                }
            }
            else
            {
                rrow.GetCell(7).CellStyle = DayStyle;
            }
            rrow.CreateCell(8).SetCellValue(eRow.rfqMeetingNotes + " " + eRow.prtNote);
            rrow.CreateCell(9).SetCellValue(eRow.ShipToName);
            rrow.CreateCell(10).SetCellValue(eRow.prtPartNumber);
            rrow.CreateCell(11).SetCellValue(eRow.prtPartDescription);
            rrow.CreateCell(12).SetCellValue(eRow.Salesman);
            rrow.CreateCell(13).SetCellValue(eRow.binMaterialWidthEnglish);
            rrow.CreateCell(14).SetCellValue(eRow.binMaterialWidthMetric);
            rrow.CreateCell(15).SetCellValue(eRow.binMaterialPitchEnglish);
            rrow.CreateCell(16).SetCellValue(eRow.binMaterialPitchMetric);
            rrow.CreateCell(17).SetCellValue(eRow.binMaterialThicknessEnglish);
            rrow.CreateCell(18).SetCellValue(eRow.binMaterialThicknessMetric);
            rrow.CreateCell(19).SetCellValue(eRow.mtyMaterialType);
            rrow.CreateCell(20).SetCellValue(eRow.rfqCustomerRFQNumber);
            rrow.CreateCell(21).SetCellValue(eRow.OEMName);
            rrow.CreateCell(22).SetCellValue(eRow.vehVehicleName);
            rrow.CreateCell(23).SetCellValue(eRow.ProgramName);
            rrow.CreateCell(24).SetCellValue(eRow.Name);
        }

        public void CreateOEMRow(XSSFSheet sheet, Int32 currentRow, OEMRow orow, XSSFCellStyle LateStyle, XSSFCellStyle ReallyLateStyle, XSSFCellStyle RightStyle, DateTime EndDate, XSSFFont LinkFont)
        {
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue(orow.OEMName);
            rrow.CreateCell(3).SetCellValue(orow.vehVehicleName);
            rrow.CreateCell(4).SetCellValue(orow.programName);
            NPOI.SS.UserModel.ICell cell = rrow.CreateCell(5);
            cell.SetCellValue(orow.rfqid);
            NPOI.XSSF.UserModel.XSSFHyperlink link = new XSSFHyperlink(NPOI.SS.UserModel.HyperlinkType.Url);
            link.Address = ("https://tsgrfq.azurewebsites.net/EditRFQ?id=" + orow.rfqid.ToString());
            cell.Hyperlink = (link);
            cell.RichStringCellValue.ApplyFont(0, orow.rfqid.ToString().Length, LinkFont);
            rrow.CreateCell(6).SetCellValue(orow.quoteid);
            rrow.CreateCell(8).SetCellValue(orow.company);
            rrow.CreateCell(9).SetCellValue(orow.dateReceived);
            rrow.CreateCell(10).SetCellValue(orow.dueDate);
            rrow.CreateCell(11).SetCellValue(orow.shipToName);
            rrow.CreateCell(12).SetCellValue(orow.customerName);
            rrow.CreateCell(13).SetCellValue(orow.partNumber);
            rrow.CreateCell(14).SetCellValue(orow.partDescription);
            rrow.CreateCell(15).SetCellValue(orow.salesman);
            rrow.CreateCell(16).SetCellValue(orow.estimator);
            rrow.CreateCell(17).SetCellValue(orow.dieType);
            rrow.CreateCell(18).SetCellValue(orow.cavity);
            rrow.CreateCell(19).SetCellValue(orow.FTBEnglish);
            rrow.CreateCell(20).SetCellValue(orow.LTREnglish);
            rrow.CreateCell(21).SetCellValue(orow.shutHeightEnglish);
            rrow.CreateCell(22).SetCellValue(orow.numberOfStations);
            rrow.CreateCell(23).SetCellValue(orow.widthEnglish);
            rrow.CreateCell(24).SetCellValue(orow.pitchEnglish);
            rrow.CreateCell(25).SetCellValue(orow.thicknessMetric);
            rrow.CreateCell(26).SetCellValue(orow.materialType);
            rrow.CreateCell(27).SetCellValue(orow.customerRFQ);
            rrow.CreateCell(28).SetCellValue(orow.leadTime);
            rrow.CreateCell(31).SetCellValue(orow.dieCost);
            rrow.CreateCell(33).SetCellValue(orow.transferBar);
            rrow.CreateCell(34).SetCellValue(orow.checkFixture);
            rrow.CreateCell(35).SetCellValue(orow.shippingCost);
            rrow.CreateCell(36).SetCellValue(orow.homeLine);
            rrow.CreateCell(38).SetCellValue(orow.totalCost);
            rrow.CreateCell(41).SetCellValue(orow.contactName);
            rrow.CreateCell(43).SetCellValue(orow.winLoss);
        }

        public void CreateOEMTitles(XSSFSheet sheet, XSSFFont titleFont, DateTime EndDate, XSSFCellStyle CenterStyle)
        {
            try
            {
                Int16 currentRow = 0;
                NPOI.SS.UserModel.IRow rrow;
                rrow = GetOrCreateRow(sheet, currentRow);
                rrow.CreateCell(2).SetCellValue("Report");
                rrow.CreateCell(3).SetCellValue(EndDate.ToString("d"));

                
                rrow.CreateCell(37).SetCellValue("Spare");
                rrow.GetCell(37).CellStyle = CenterStyle;
                rrow.GetCell(37).RichStringCellValue.ApplyFont(titleFont);

                currentRow++;
                rrow = GetOrCreateRow(sheet, currentRow);
                rrow.CreateCell(7).SetCellValue("Package");
                rrow.GetCell(7).CellStyle = CenterStyle;
                rrow.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(9).SetCellValue("Order");
                rrow.GetCell(9).CellStyle = CenterStyle;
                rrow.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(10).SetCellValue("Due");
                rrow.GetCell(10).CellStyle = CenterStyle;
                rrow.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(19).SetCellValue("F to B");
                rrow.GetCell(19).CellStyle = CenterStyle;
                rrow.GetCell(19).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(20).SetCellValue("L to R");
                rrow.GetCell(20).CellStyle = CenterStyle;
                rrow.GetCell(20).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(21).SetCellValue("Shut Height");
                rrow.GetCell(21).CellStyle = CenterStyle;
                rrow.GetCell(21).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(22).SetCellValue("Number");
                rrow.GetCell(22).CellStyle = CenterStyle;
                rrow.GetCell(22).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(23).SetCellValue("Width");
                rrow.GetCell(23).CellStyle = CenterStyle;
                rrow.GetCell(23).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(24).SetCellValue("Pitch");
                rrow.GetCell(24).CellStyle = CenterStyle;
                rrow.GetCell(24).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(25).SetCellValue("Thickness");
                rrow.GetCell(25).CellStyle = CenterStyle;
                rrow.GetCell(25).RichStringCellValue.ApplyFont(titleFont);

                rrow.CreateCell(27).SetCellValue("Customer");
                rrow.GetCell(27).CellStyle = CenterStyle;
                rrow.GetCell(27).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(28).SetCellValue("Lead");
                rrow.GetCell(28).CellStyle = CenterStyle;
                rrow.GetCell(28).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(29).SetCellValue("Lead");
                rrow.GetCell(29).CellStyle = CenterStyle;
                rrow.GetCell(29).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(33).SetCellValue("Transfer Bars");
                rrow.GetCell(33).RichStringCellValue.ApplyFont(titleFont);
                rrow.GetCell(33).CellStyle = CenterStyle;
                rrow.CreateCell(37).SetCellValue("Pierce, Punches");
                rrow.GetCell(37).RichStringCellValue.ApplyFont(titleFont);
                rrow.GetCell(37).CellStyle = CenterStyle;
                rrow.CreateCell(40).SetCellValue("Customer");
                rrow.GetCell(40).CellStyle = CenterStyle;
                rrow.GetCell(40).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(41).SetCellValue("Shipping");
                rrow.GetCell(41).CellStyle = CenterStyle;
                rrow.GetCell(41).RichStringCellValue.ApplyFont(titleFont);
                rrow.CreateCell(42).SetCellValue("No Quote");
                rrow.GetCell(42).CellStyle = CenterStyle;
                rrow.GetCell(42).RichStringCellValue.ApplyFont(titleFont);

                currentRow++;
                rrow = GetOrCreateRow(sheet, currentRow);
                rrow.CreateCell(1).SetCellValue("Chng");
                rrow.CreateCell(2).SetCellValue("OEM");
                rrow.CreateCell(3).SetCellValue("Vehicle");
                rrow.CreateCell(4).SetCellValue("Program");
                rrow.CreateCell(5).SetCellValue("RFQ");
                rrow.CreateCell(6).SetCellValue("Quote");
                rrow.CreateCell(7).SetCellValue("Package");
                rrow.CreateCell(8).SetCellValue("Group");
                rrow.CreateCell(9).SetCellValue("Date");
                rrow.CreateCell(10).SetCellValue("Date");
                rrow.CreateCell(11).SetCellValue("Ship To Name");
                rrow.CreateCell(12).SetCellValue("Corporate Name");
                rrow.CreateCell(13).SetCellValue("Part Number");
                rrow.CreateCell(14).SetCellValue("Description");
                rrow.CreateCell(15).SetCellValue("Salesman");
                rrow.CreateCell(16).SetCellValue("Estimator");
                rrow.CreateCell(17).SetCellValue("Process");
                rrow.CreateCell(18).SetCellValue("Cavity");
                rrow.CreateCell(19).SetCellValue("Inch");
                rrow.CreateCell(20).SetCellValue("Inch");
                rrow.CreateCell(21).SetCellValue("Inch");
                rrow.CreateCell(22).SetCellValue("Stations");
                rrow.CreateCell(23).SetCellValue("Inch");
                rrow.CreateCell(24).SetCellValue("Inch");
                rrow.CreateCell(25).SetCellValue("MM");
                rrow.CreateCell(26).SetCellValue("Material Type");
                rrow.CreateCell(27).SetCellValue("RFQ Number");
                rrow.CreateCell(28).SetCellValue("Time");
                rrow.CreateCell(29).SetCellValue("Time");
                rrow.CreateCell(30).SetCellValue("Target");
                rrow.CreateCell(31).SetCellValue("Die Cost");
                rrow.CreateCell(32).SetCellValue("Coating Cost");
                rrow.CreateCell(33).SetCellValue("and Fingers");
                rrow.CreateCell(34).SetCellValue("Check Fixture");
                rrow.CreateCell(35).SetCellValue("Shipping");
                rrow.CreateCell(36).SetCellValue("Home Line");
                rrow.CreateCell(37).SetCellValue("and Buttons");
                rrow.CreateCell(38).SetCellValue("Total Die Cost");
                rrow.CreateCell(39).SetCellValue("Awarded");
                rrow.CreateCell(40).SetCellValue("Status");
                rrow.CreateCell(41).SetCellValue("Confirm To");
                rrow.CreateCell(42).SetCellValue("Reason");
                rrow.CreateCell(43).SetCellValue("Win/Loss");

                for (int i = 1; i < 44; i++)
                {
                    try
                    {
                        rrow.GetCell(i).CellStyle = CenterStyle;
                        rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception er)
            {

            }
        }

        public void CreateSTSTitles(XSSFSheet sheet, XSSFFont titleFont, DateTime EndDate, XSSFCellStyle CenterStyle)
        {
            Int16 currentRow = 0;
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue("Report");
            rrow.CreateCell(3).SetCellValue(EndDate.ToString("d"));
            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(7).SetCellValue("Package");
            rrow.GetCell(7).CellStyle = CenterStyle;
            rrow.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(9).SetCellValue("Order");
            rrow.GetCell(9).CellStyle = CenterStyle;
            rrow.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(10).SetCellValue("Due");
            rrow.GetCell(10).CellStyle = CenterStyle;
            rrow.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(19).SetCellValue("Tool To");
            rrow.GetCell(19).CellStyle = CenterStyle;
            rrow.GetCell(19).RichStringCellValue.ApplyFont(titleFont);

            rrow.CreateCell(23).SetCellValue("Customer");
            rrow.GetCell(23).CellStyle = CenterStyle;
            rrow.GetCell(23).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(24).SetCellValue("Lead");
            rrow.GetCell(24).CellStyle = CenterStyle;
            rrow.GetCell(24).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(25).SetCellValue("Lead");
            rrow.GetCell(25).CellStyle = CenterStyle;
            rrow.GetCell(25).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(29).SetCellValue("Customer");
            rrow.GetCell(29).CellStyle = CenterStyle;
            rrow.GetCell(29).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(31).SetCellValue("Type Of");
            rrow.GetCell(31).CellStyle = CenterStyle;
            rrow.GetCell(31).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(32).SetCellValue("Shipping");
            rrow.GetCell(32).CellStyle = CenterStyle;
            rrow.GetCell(32).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(34).SetCellValue("No Quote");
            rrow.GetCell(34).CellStyle = CenterStyle;
            rrow.GetCell(34).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(35).SetCellValue("Win/Loss");
            rrow.GetCell(35).CellStyle = CenterStyle;
            rrow.GetCell(35).RichStringCellValue.ApplyFont(titleFont);

            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(1).SetCellValue("Chng");
            rrow.CreateCell(2).SetCellValue("OEM");
            rrow.CreateCell(3).SetCellValue("Vehicle");
            rrow.CreateCell(4).SetCellValue("Program");
            rrow.CreateCell(5).SetCellValue("RFQ");
            rrow.CreateCell(6).SetCellValue("Quote");
            rrow.CreateCell(7).SetCellValue("Package");
            rrow.CreateCell(8).SetCellValue("Group");
            rrow.CreateCell(9).SetCellValue("Date");
            rrow.CreateCell(10).SetCellValue("Date");
            rrow.CreateCell(11).SetCellValue("+-Days");
            rrow.CreateCell(12).SetCellValue("Friday Notes");
            rrow.CreateCell(13).SetCellValue("Ship To Name");
            rrow.CreateCell(14).SetCellValue("Corporate Name");
            rrow.CreateCell(15).SetCellValue("Part Number");
            rrow.CreateCell(16).SetCellValue("Description");
            rrow.CreateCell(17).SetCellValue("Salesman");
            rrow.CreateCell(18).SetCellValue("Estimator");
            rrow.CreateCell(19).SetCellValue("Quote");
            rrow.CreateCell(20).SetCellValue("Process");
            rrow.CreateCell(21).SetCellValue("Machine Time");
            rrow.CreateCell(22).SetCellValue("EAV");
            //rrow.CreateCell(23).SetCellValue("MM");
            //rrow.CreateCell(24).SetCellValue("Inch");
            //rrow.CreateCell(25).SetCellValue("MM");
            //rrow.CreateCell(26).SetCellValue("Inch");
            //rrow.CreateCell(27).SetCellValue("MM");
            //rrow.CreateCell(28).SetCellValue("Stations");
            //rrow.CreateCell(29).SetCellValue("Inch");
            //rrow.CreateCell(30).SetCellValue("MM");
            //rrow.CreateCell(31).SetCellValue("Inch");
            //rrow.CreateCell(32).SetCellValue("MM");
            //rrow.CreateCell(33).SetCellValue("Inch");
            //rrow.CreateCell(34).SetCellValue("MM");
            //rrow.CreateCell(35).SetCellValue("Material Type");
            //rrow.CreateCell(36).SetCellValue("Aluminum");
            //rrow.CreateCell(37).SetCellValue("Eng. No");
            //rrow.CreateCell(38).SetCellValue("Volume");
            rrow.CreateCell(23).SetCellValue("RFQ Number");
            rrow.CreateCell(24).SetCellValue("Time");
            rrow.CreateCell(25).SetCellValue("Time");
            rrow.CreateCell(26).SetCellValue("Target");
            rrow.CreateCell(27).SetCellValue("Awarded");
            rrow.CreateCell(28).SetCellValue("Amount");
            rrow.CreateCell(29).SetCellValue("Status");
            rrow.CreateCell(30).SetCellValue("Part");
            rrow.CreateCell(47).SetCellValue("Industry");
            rrow.CreateCell(31).SetCellValue("Method");
            rrow.CreateCell(32).SetCellValue("Confirm To");
            rrow.CreateCell(33).SetCellValue("Reason");
            rrow.CreateCell(34).SetCellValue("Reason");
            rrow.CreateCell(35).SetCellValue("Notes");
            for (int i = 1; i < 35; i++)
            {
                try
                {
                    rrow.GetCell(i).CellStyle = CenterStyle;
                    rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                }
                catch
                {

                }
            }
        }

        public void CreateUGSTitles(XSSFSheet sheet, XSSFFont titleFont, DateTime EndDate, XSSFCellStyle CenterStyle)
        {
            Int16 currentRow = 0;
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue("Report");
            rrow.CreateCell(3).SetCellValue(EndDate.ToString("d"));
            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(7).SetCellValue("Package");
            rrow.GetCell(7).CellStyle = CenterStyle;
            rrow.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(9).SetCellValue("Order");
            rrow.GetCell(9).CellStyle = CenterStyle;
            rrow.GetCell(9).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(10).SetCellValue("Due");
            rrow.GetCell(10).CellStyle = CenterStyle;
            rrow.GetCell(10).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(19).SetCellValue("Tool To");
            rrow.GetCell(19).CellStyle = CenterStyle;
            rrow.GetCell(19).RichStringCellValue.ApplyFont(titleFont);

            rrow.CreateCell(20).SetCellValue("Customer");
            rrow.GetCell(20).CellStyle = CenterStyle;
            rrow.GetCell(20).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(21).SetCellValue("Lead");
            rrow.GetCell(21).CellStyle = CenterStyle;
            rrow.GetCell(21).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(22).SetCellValue("Lead");
            rrow.GetCell(22).CellStyle = CenterStyle;
            rrow.GetCell(22).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(26).SetCellValue("Customer");
            rrow.GetCell(26).CellStyle = CenterStyle;
            rrow.GetCell(26).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(28).SetCellValue("Type Of");
            rrow.GetCell(28).CellStyle = CenterStyle;
            rrow.GetCell(28).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(29).SetCellValue("Shipping");
            rrow.GetCell(29).CellStyle = CenterStyle;
            rrow.GetCell(29).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(31).SetCellValue("No Quote");
            rrow.GetCell(31).CellStyle = CenterStyle;
            rrow.GetCell(31).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(32).SetCellValue("Win/Loss");
            rrow.GetCell(32).CellStyle = CenterStyle;
            rrow.GetCell(32).RichStringCellValue.ApplyFont(titleFont);

            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(1).SetCellValue("Chng");
            rrow.CreateCell(2).SetCellValue("OEM");
            rrow.CreateCell(3).SetCellValue("Vehicle");
            rrow.CreateCell(4).SetCellValue("Program");
            rrow.CreateCell(5).SetCellValue("RFQ");
            rrow.CreateCell(6).SetCellValue("Quote");
            rrow.CreateCell(7).SetCellValue("Package");
            rrow.CreateCell(8).SetCellValue("Group");
            rrow.CreateCell(9).SetCellValue("Date");
            rrow.CreateCell(10).SetCellValue("Date");
            rrow.CreateCell(11).SetCellValue("+-Days");
            rrow.CreateCell(12).SetCellValue("Friday Notes");
            rrow.CreateCell(13).SetCellValue("Ship To Name");
            rrow.CreateCell(14).SetCellValue("Corporate Name");
            rrow.CreateCell(15).SetCellValue("Part Number");
            rrow.CreateCell(16).SetCellValue("Description");
            rrow.CreateCell(17).SetCellValue("Salesman");
            rrow.CreateCell(18).SetCellValue("Estimator");
            rrow.CreateCell(19).SetCellValue("Quote");
            rrow.CreateCell(20).SetCellValue("RFQ Number");
            rrow.CreateCell(21).SetCellValue("Time");
            rrow.CreateCell(22).SetCellValue("Time");
            rrow.CreateCell(23).SetCellValue("Target");
            rrow.CreateCell(24).SetCellValue("Awarded");
            rrow.CreateCell(25).SetCellValue("Amount");
            rrow.CreateCell(26).SetCellValue("Status");
            rrow.CreateCell(27).SetCellValue("Part");
            rrow.CreateCell(28).SetCellValue("Industry");
            rrow.CreateCell(29).SetCellValue("Method");
            rrow.CreateCell(30).SetCellValue("Confirm To");
            rrow.CreateCell(31).SetCellValue("Reason");
            rrow.CreateCell(32).SetCellValue("Reason");
            rrow.CreateCell(33).SetCellValue("Notes");
            for (int i = 1; i < 37; i++)
            {
                try
                {
                    rrow.GetCell(i).CellStyle = CenterStyle;
                    rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                }
                catch
                {

                }
            }
        }

        public void CreateSTSRow(XSSFSheet sheet, Int32 currentRow, OEMRow orow, XSSFCellStyle LateStyle, XSSFCellStyle ReallyLateStyle, XSSFCellStyle RightStyle, DateTime EndDate, XSSFFont LinkFont)
        {
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue(orow.OEMName);
            rrow.CreateCell(3).SetCellValue(orow.vehVehicleName);
            rrow.CreateCell(4).SetCellValue(orow.programName);
            NPOI.SS.UserModel.ICell cell = rrow.CreateCell(5);
            cell.SetCellValue(orow.rfqid);
            NPOI.XSSF.UserModel.XSSFHyperlink link = new XSSFHyperlink(NPOI.SS.UserModel.HyperlinkType.Url);
            link.Address = ("https://tsgrfq.azurewebsites.net/EditRFQ?id=" + orow.rfqid.ToString());
            cell.Hyperlink = (link);
            cell.RichStringCellValue.ApplyFont(0, orow.rfqid.ToString().Length, LinkFont);
            rrow.CreateCell(6).SetCellValue(orow.quoteid);
            rrow.CreateCell(8).SetCellValue(orow.company);
            rrow.CreateCell(9).SetCellValue(orow.dateReceived);
            rrow.CreateCell(10).SetCellValue(orow.dueDate);
            Double DaysLate = (EndDate - System.Convert.ToDateTime(orow.dueDate)).TotalDays;
            rrow.CreateCell(11).SetCellValue(DaysLate.ToString());
            if (DaysLate > 0)
            {
                if (DaysLate > 7)
                {
                    rrow.GetCell(11).CellStyle = ReallyLateStyle;
                }
                else
                {
                    rrow.GetCell(11).CellStyle = LateStyle;

                }
            }
            else
            {
                rrow.GetCell(11).CellStyle = RightStyle;
            }
            rrow.CreateCell(12).SetCellValue(orow.rfqMeetingNotes);
            rrow.CreateCell(13).SetCellValue(orow.shipToName);
            rrow.CreateCell(14).SetCellValue(orow.customerName);
            rrow.CreateCell(15).SetCellValue(orow.partNumber);
            rrow.CreateCell(16).SetCellValue(orow.partDescription);
            rrow.CreateCell(17).SetCellValue(orow.salesman);
            rrow.CreateCell(18).SetCellValue(orow.estimator);
            rrow.CreateCell(19).SetCellValue(orow.quoteid);
            rrow.CreateCell(20).SetCellValue(orow.dieType);
            rrow.CreateCell(21).SetCellValue(orow.cavity);
            rrow.CreateCell(22).SetCellValue(orow.FTBEnglish);
            rrow.CreateCell(23).SetCellValue(orow.customerRFQ);
            rrow.CreateCell(24).SetCellValue("weeks");
            rrow.CreateCell(25).SetCellValue(orow.leadTime);
            rrow.CreateCell(32).SetCellValue(orow.contactName);
            rrow.CreateCell(28).SetCellValue(orow.totalCost);

        }

        public void createUGSRow(XSSFSheet sheet, Int32 currentRow, OEMRow orow, XSSFCellStyle LateStyle, XSSFCellStyle ReallyLateStyle, XSSFCellStyle RightStyle, DateTime EndDate, XSSFFont LinkFont)
        {
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue(orow.OEMName);
            rrow.CreateCell(3).SetCellValue(orow.vehVehicleName);
            rrow.CreateCell(4).SetCellValue(orow.programName);
            NPOI.SS.UserModel.ICell cell = rrow.CreateCell(5);
            cell.SetCellValue(orow.rfqid);
            NPOI.XSSF.UserModel.XSSFHyperlink link = new XSSFHyperlink(NPOI.SS.UserModel.HyperlinkType.Url);
            link.Address = ("https://tsgrfq.azurewebsites.net/EditRFQ?id=" + orow.rfqid.ToString());
            cell.Hyperlink = (link);
            cell.RichStringCellValue.ApplyFont(0, orow.rfqid.ToString().Length, LinkFont);
            rrow.CreateCell(6).SetCellValue(orow.quoteid);
            rrow.CreateCell(8).SetCellValue(orow.company);
            rrow.CreateCell(9).SetCellValue(orow.dateReceived);
            rrow.CreateCell(10).SetCellValue(orow.dueDate);
            Double DaysLate = (EndDate - System.Convert.ToDateTime(orow.dueDate)).TotalDays;
            rrow.CreateCell(11).SetCellValue(DaysLate.ToString());
            if (DaysLate > 0)
            {
                if (DaysLate > 7)
                {
                    rrow.GetCell(11).CellStyle = ReallyLateStyle;
                }
                else
                {
                    rrow.GetCell(11).CellStyle = LateStyle;

                }
            }
            else
            {
                rrow.GetCell(11).CellStyle = RightStyle;
            }
            rrow.CreateCell(12).SetCellValue(orow.rfqMeetingNotes);
            rrow.CreateCell(13).SetCellValue(orow.shipToName);
            rrow.CreateCell(14).SetCellValue(orow.customerName);
            rrow.CreateCell(15).SetCellValue(orow.partNumber);
            rrow.CreateCell(16).SetCellValue(orow.partDescription);
            rrow.CreateCell(17).SetCellValue(orow.salesman);
            rrow.CreateCell(18).SetCellValue(orow.estimator);
            rrow.CreateCell(19).SetCellValue(orow.quoteid);
            rrow.CreateCell(20).SetCellValue(orow.customerRFQ);
            rrow.CreateCell(21).SetCellValue("weeks");
            rrow.CreateCell(22).SetCellValue(orow.leadTime);
            rrow.CreateCell(30).SetCellValue(orow.contactName);

        }

        public void CreateEstimatingTitles(XSSFSheet sheet, XSSFFont titleFont, DateTime EndDate, XSSFCellStyle CenterStyle)
        {
            Int16 currentRow = 0;
            NPOI.SS.UserModel.IRow rrow;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(2).SetCellValue("Report");
            rrow.CreateCell(3).SetCellValue(EndDate.ToString("d"));
            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(3).SetCellValue("Package");
            rrow.GetCell(3).CellStyle = CenterStyle;
            rrow.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(5).SetCellValue("Order");
            rrow.GetCell(5).CellStyle = CenterStyle;
            rrow.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(6).SetCellValue("Due");
            rrow.GetCell(6).CellStyle = CenterStyle;
            rrow.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
            rrow.CreateCell(13).SetCellValue("Width");
            rrow.CreateCell(14).SetCellValue("Width");
            rrow.CreateCell(15).SetCellValue("Pitch");
            rrow.CreateCell(16).SetCellValue("Pitch");
            rrow.CreateCell(17).SetCellValue("Thickness");
            rrow.CreateCell(18).SetCellValue("Thickness");
            for (int i = 13; i < 19; i++)
            {
                rrow.GetCell(i).CellStyle = CenterStyle;
                rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
            }
            rrow.CreateCell(20).SetCellValue("Customer");
            rrow.GetCell(20).CellStyle = CenterStyle;
            rrow.GetCell(20).RichStringCellValue.ApplyFont(titleFont);
            currentRow++;
            rrow = GetOrCreateRow(sheet, currentRow);
            rrow.CreateCell(1).SetCellValue("Chng");
            rrow.CreateCell(2).SetCellValue("RFQ");
            rrow.CreateCell(3).SetCellValue("Number");
            rrow.CreateCell(4).SetCellValue("Group");
            rrow.CreateCell(5).SetCellValue("Date");
            rrow.CreateCell(6).SetCellValue("Date");
            rrow.CreateCell(7).SetCellValue("+-Days");
            rrow.CreateCell(8).SetCellValue("Friday Notes");
            rrow.CreateCell(9).SetCellValue("Ship To Name");
            rrow.CreateCell(10).SetCellValue("Part Number");
            rrow.CreateCell(11).SetCellValue("Description");
            rrow.CreateCell(12).SetCellValue("Salesman");
            rrow.CreateCell(13).SetCellValue("Inch");
            rrow.CreateCell(14).SetCellValue("MM");
            rrow.CreateCell(15).SetCellValue("Inch");
            rrow.CreateCell(16).SetCellValue("MM");
            rrow.CreateCell(17).SetCellValue("Inch");
            rrow.CreateCell(18).SetCellValue("MM");
            rrow.CreateCell(19).SetCellValue("Material Type");
            rrow.CreateCell(20).SetCellValue("RFQ Number");
            rrow.CreateCell(21).SetCellValue("OEM");
            rrow.CreateCell(22).SetCellValue("Vehicle");
            rrow.CreateCell(23).SetCellValue("Program");
            rrow.CreateCell(24).SetCellValue("Confirm To");
            for (int i = 1; i < 25; i++)
            {
                rrow.GetCell(i).CellStyle = CenterStyle;
                rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
            }

        }
        public NPOI.SS.UserModel.IRow GetOrCreateRow(XSSFSheet referenceSheet, Int32 currentRow)
        {

            if (referenceSheet.GetRow(currentRow) == null)
            {
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
    public class EstimatingRow
    {
        public string rfqid { get; set;}
        public string HoldCompany { get; set;}
        public string rfqDateReceived { get; set;}
        public string DueDate { get; set;}
        public string rfqMeetingNotes { get; set;}
        public string prtNote { get; set;}
        public string ShipToName { get; set;}
        public string Salesman { get; set; }       
        public string prtPartNumber {get; set;}
        public string prtPartDescription {get; set;}
        public string binMaterialWidthEnglish {get; set;}
        public string binMaterialWidthMetric {get; set;}
        public string binMaterialPitchEnglish {get; set;}
        public string binMaterialPitchMetric {get; set;}
        public string binMaterialThicknessEnglish {get; set;}
        public string binMaterialThicknessMetric {get; set;}
        public string mtyMaterialType {get; set;}
        public string rfqCustomerRFQNumber {get; set;}
        public string OEMName {get; set;}
        public string vehVehicleName {get; set;}
        public string ProgramName {get; set;}
        public string Name {get; set;}
    }
    public class OEMRow
    {
        public string OEMName { get; set;}
        public string vehVehicleName { get; set;}
        public string programName { get; set;}
        public string rfqid { get; set;}
        public string quoteid { get; set;}
        public string company { get; set; }
        public string dateReceived { get; set;}
        public string dueDate { get; set;}
        public string salesman { get; set; }
        public string estimator { get; set; }
        public string rfqMeetingNotes { get;set;}
        public string shipToName { get; set;}
        public string customerName { get; set; }
        public string partNumber { get;set; }
        public string partDescription { get; set;}
        public string dieType { get; set;}
        public string cavity { get; set;}
        public string FTBEnglish { get; set;}
        public string FTBMetric { get; set;}
        public string LTREnglish { get; set;}
        public string LTRMetric { get; set;}
        public string shutHeightEnglish { get; set; }
        public string shutHeightMetric {get; set;}
        public string numberOfStations { get; set;}
        public string widthEnglish { get; set;}
        public string widthMetric { get; set;}
        public string pitchEnglish { get; set;}
        public string pitchMetric { get; set;}
        public string thicknessEnglish { get; set;}
        public string thicknessMetric { get; set;}
        public string materialType { get; set;}
        public string annualVolume { get; set;}
        public string customerRFQ { get; set;}
        public string contactName { get; set;}
        public string leadTime { get; set; }
        public string dieCost { get; set; }
        public string transferBar { get; set; }
        public string checkFixture { get; set; }
        public string shippingCost { get; set; }
        public string homeLine { get; set; }
        public string totalCost { get; set; }
        public string winLoss { get; set; }
    }
}