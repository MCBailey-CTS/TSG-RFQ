using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using System.Net.Mail;
using System.IO;

namespace RFQ
{
    /// <summary>
    /// Summary description for TestReport
    /// </summary>
    public class TestReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();

            sql.CommandText = "Select chiCallDate, CustomerName, ShipToName, CustomerContact.Name, perName, ProgramName, creContactReason, chiCompanies, chiCallDetails ";
            sql.CommandText += "from tblContactHistory ";
            sql.CommandText += "left outer join CustomerContact on CustomerContactID = chiContactID ";
            sql.CommandText += "inner join Permissions on EmailAddress = chiCreatedBy ";
            sql.CommandText += "inner join Customer on Customer.CustomerID = chiCustomerID ";
            sql.CommandText += "left outer join CustomerLocation on CustomerLocationID = chiPlantID ";
            sql.CommandText += "inner join pktblContactReason on creContactReasonID = chiContactReasonID ";
            sql.CommandText += "left outer join Program on ProgramID = chiProgram ";
            sql.CommandText += "where chiCreated >= DATEADD(DAY, -7, GETDATE()) and chiCreated <= GETDATE() ";
            sql.Parameters.Clear();
            SqlDataReader dr = sql.ExecuteReader();

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Call Summary");
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
            XSSFCellStyle CenterStyle = (XSSFCellStyle)wb.CreateCellStyle();
            CenterStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            XSSFColor ColorBlue = new XSSFColor();
            byte[] Blue = { 0, 0, 128 };
            ColorBlue.SetRgb(Blue);
            blueFont.SetColor(ColorBlue);

            XSSFCellStyle wrapStyle = (XSSFCellStyle)wb.CreateCellStyle();
            wrapStyle.WrapText = true;

            sh = WriteHeader(sh, titleFont, CenterStyle);

            int currentRow = 3;
            while (dr.Read())
            {
                NPOI.SS.UserModel.IRow row = GetOrCreateRow(sh, currentRow);
                row.Height = 1000;
                row.CreateCell(0).SetCellValue(System.Convert.ToDateTime(dr["chiCallDate"].ToString()).ToShortDateString());
                row.CreateCell(1).SetCellValue(dr["CustomerName"].ToString());
                row.CreateCell(2).SetCellValue(dr["ShipToName"].ToString());
                row.CreateCell(3).SetCellValue(dr["Name"].ToString());
                row.CreateCell(4).SetCellValue(dr["perName"].ToString());
                row.CreateCell(5).SetCellValue(dr["ProgramName"].ToString());
                row.CreateCell(6).SetCellValue(dr["creContactReason"].ToString());
                row.CreateCell(7).SetCellValue(dr["chiCompanies"].ToString());
                row.GetCell(7).CellStyle = wrapStyle;
                row.CreateCell(8).SetCellValue(dr["chiCallDetails"].ToString());
                row.GetCell(8).CellStyle = wrapStyle;
                row.Height = (short)-1;
                currentRow++;
            }
            dr.Close();
            sh.AutoSizeColumn(0);
            sh.AutoSizeColumn(1);
            sh.AutoSizeColumn(2);
            sh.AutoSizeColumn(3);
            sh.AutoSizeColumn(4);
            sh.AutoSizeColumn(5);
            sh.AutoSizeColumn(6);
            sh.AutoSizeColumn(7);
            sh.AutoSizeColumn(8);

            SendReport(wb);

            connection.Close();
        }

        private XSSFSheet WriteHeader(XSSFSheet sheet, XSSFFont titleFont, XSSFCellStyle CenterStyle)
        {
            int currentRow = 0;
            NPOI.SS.UserModel.IRow row = GetOrCreateRow(sheet, currentRow);
            row.CreateCell(0).SetCellValue("Weekly Call Report");
            row.CreateCell(1).SetCellValue(System.DateTime.Now.ToShortDateString());
            currentRow += 2;
            row = GetOrCreateRow(sheet, currentRow);
            row.CreateCell(0).SetCellValue("Call Date");
            row.GetCell(0).CellStyle = CenterStyle;
            row.GetCell(0).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(1).SetCellValue("Customer");
            row.GetCell(1).CellStyle = CenterStyle;
            row.GetCell(1).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(2).SetCellValue("Plant");
            row.GetCell(2).CellStyle = CenterStyle;
            row.GetCell(2).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(3).SetCellValue("Contact");
            row.GetCell(3).CellStyle = CenterStyle;
            row.GetCell(3).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(4).SetCellValue("Salesman");
            row.GetCell(4).CellStyle = CenterStyle;
            row.GetCell(4).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(5).SetCellValue("Program");
            row.GetCell(5).CellStyle = CenterStyle;
            row.GetCell(5).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(6).SetCellValue("Contact Reeason");
            row.GetCell(6).CellStyle = CenterStyle;
            row.GetCell(6).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(7).SetCellValue("TSG Companies");
            row.GetCell(7).CellStyle = CenterStyle;
            row.GetCell(7).RichStringCellValue.ApplyFont(titleFont);
            row.CreateCell(8).SetCellValue("Call Details");
            row.GetCell(8).CellStyle = CenterStyle;
            row.GetCell(8).RichStringCellValue.ApplyFont(titleFont);

            return sheet;
        }

        private void SendReport(XSSFWorkbook wb)
        {
            Site master = new Site();
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

                mail.Attachments.Add(new System.Net.Mail.Attachment(ms2, "Call Report " + DateTime.Now.ToShortDateString().Replace('/', '-') + ".xlsx"));

                //mail.Attachments.Add(System.Net.Mail.Attachment(ms, "RFQ-QUOTE" + StartDate.ToString("d").Replace("/", "-") + " to " + EndDate.ToString("d").Replace("/", "-") + ".xlsx"));


                mail.From = master.getFromAddress();
                //if (master.getUserName() == "chris@netinflux.com")
                //{
                //    mail.To.Add(new MailAddress("rmumford@toolingsystemsgroup.com"));
                //}
                //else
                //{
                //    mail.To.Add(new MailAddress(master.getUserName()));
                //}
                mail.To.Add(new MailAddress("rmumford@toolingsystemsgroup.com"));
                mail.Subject = "Call Report " + DateTime.Now.ToShortDateString();
                mail.Body += "Attached is the weekly customer call report.";
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}