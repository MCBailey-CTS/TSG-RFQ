using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace RFQ
{
    public partial class CreateQuote : System.Web.UI.Page
    {
        Document quotePDF = new Document();
        MemoryStream ms = new MemoryStream();
        PdfWriter writer;
        //var writer = new PdfCopy;
        Boolean dateCreated = false;
        Boolean debugmode = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            int quoteNumber = 0;
            int quoteType = 0;
            int rfqID = 0;
            int page = 1;
            int numberOfQuotes = 0;

            //System.Diagnostics.Debug.WriteLine("This is a debug test");

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            bool permissions = false;
            sql.CommandText = "select perRFQ from permissions where EmailAddress=@user";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@user", master.getUserName());
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (System.Convert.ToBoolean(dr["perRFQ"].ToString()))
                {
                    permissions = true;
                }
            }
            dr.Close();

            if (!permissions)
            {
                connection.Close();
                HttpContext.Current.Response.Redirect("~/Permissions.aspx", false);
                return;
            }

            // Start building the quote pdf

            quotePDF.AddTitle("TSG Quote");
            quotePDF.SetMargins(30f, 30f, 30f, 30f);

            writer = PdfWriter.GetInstance(quotePDF, ms);
            //var writer = new PdfCopy.GetInstance(quotePDF, ms);

            if (Request["quoteNumber"] != "" && Request["quoteNumber"] != null)
            {
                quoteNumber = System.Convert.ToInt32(Request["quoteNumber"]);
            }
            if (Request["quoteType"] != "" && Request["quoteType"] != null)
            {
                quoteType = System.Convert.ToInt32(Request["quoteType"]);
            }
            if (Request["dateCreated"] != null)
            {
                if (Request["dateCreated"].ToString().ToLower() == "true")
                {
                    dateCreated = true;
                }
            }
            // Form the pdfName
            string pdfName = "";
            if (quoteType == 3)
            {
                sql.CommandText = "Select hquNumber, prtRFQLineNumber, hquVersion, qtrRFQID, prtPartNumber ";
                sql.CommandText += "from tblHTSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = hquHTSQuoteID and ptqHTS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = hquHTSQuoteID and qtrHTS = 1 ";
                sql.CommandText += "where hquHTSQuoteID = @id order by prtRFQLineNumber asc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["prtRFQLineNumber"].ToString() != "")
                    {
                        int num;
                        bool results = Int32.TryParse(dr["hquNumber"].ToString(), out num);
                        if (!results && dr["qtrRFQID"].ToString() != "")
                        {
                            pdfName = dr["hquNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString();
                        }
                        else if (!results)
                        {
                            pdfName = dr["hquNumber"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString();
                        }
                        else
                        {
                            pdfName = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString() + "_" + dr["prtPartNumber"].ToString();
                        }
                    }
                    else
                    {
                        pdfName = quoteNumber + "-HTS-SA-" + dr["hquVersion"].ToString();
                    }
                }
                dr.Close();
            }
            else if (quoteType == 4)
            {
                sql.CommandText = "Select squQuoteNumber, squDetailedQuotePdf, prtRFQLineNumber, squQuoteVersion, squECQuote, squECQuoteNumber, squECBaseQuoteId, qtrRFQID, assLineNumber ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = squSTSQuoteID and atqSTS = 1 ";
                sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                sql.CommandText += "where squSTSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["prtRFQLineNumber"].ToString() != "")
                    {
                        int num;
                        bool results = Int32.TryParse(dr["squQuoteNumber"].ToString(), out num);
                        if (!results)
                        {
                            // BD - add ec to quote pdf Name
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                            }
                            else
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                        else
                        {
                            // BD - add ec to quote pdf Name
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                            }
                            else
                            {
                                pdfName = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                    }
                    else if (dr["assLineNumber"].ToString() != "")
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                        }
                        else
                        {
                            pdfName = dr["qtrRFQID"].ToString() + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = quoteNumber + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                        }
                        else
                        {
                            pdfName = quoteNumber + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    
                }
                dr.Close();
            }
            else if (quoteType == 5)
            {
                sql.CommandText = "Select uquQuoteNumber, prtRFQLineNumber, uquQuoteVersion, qtrRFQID, prtPartNumber ";
                sql.CommandText += "from tblUGSQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = uquUGSQuoteID and qtrUGS = 1 ";
                sql.CommandText += "where uquUGSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["prtRFQLineNumber"].ToString() != "")
                    {
                        int num;
                        bool results = Int32.TryParse(dr["uquQuoteNumber"].ToString(), out num);
                        if (!results)
                        {
                            pdfName = dr["uquQuoteNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString();
                        }
                        else
                        {
                            pdfName = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString() + "_" + dr["prtPartNumber"].ToString();
                        }
                    }
                    else
                    {
                        pdfName = quoteNumber + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                    }
                }
                dr.Close();
            }
            else if (quoteType == 1)
            {
                sql.CommandText = "Select ecqQuoteNumber, ecqVersion, TSGCompanyAbbrev ";
                sql.CommandText += "from tblECQuote ";
                sql.CommandText += "inner join TSGCompany on TSGCompanyID = ecqTSGCompanyID ";
                sql.CommandText += "where ecqECQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (System.Convert.ToInt32(dr["ecqVersion"].ToString()) == 1)
                    {
                        pdfName = quoteNumber + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["ecqVersion"].ToString();
                    }
                    else
                    {
                        pdfName = dr["ecqQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-SA-" + dr["ecqVersion"].ToString();
                    }
                }
                dr.Close();
            }
            else
            {
                sql.CommandText = "Select quoOldQuoteNumber, prtRFQLineNumber, quoVersion, qtrRFQID, TSGCompanyAbbrev, prtPartNumber ";
                sql.CommandText += "from tblQuote ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = quoQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = quoQuoteID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 ";
                sql.CommandText += "inner join TSGCompany on TSGCompanyID = quoTSGCompanyID ";
                sql.CommandText += "where quoQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteNumber);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    int num;
                    bool results = Int32.TryParse(dr["quoOldQuoteNumber"].ToString(), out num);
                    if (!results && dr["quoOldQuoteNumber"].ToString() != "")
                    {
                        if (dr["quoOldQuoteNumber"].ToString().Contains("SA"))
                        {
                            pdfName = dr["quoOldQuoteNumber"].ToString();
                        }
                        else
                        {
                            pdfName = dr["quoOldQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString() + "_" + dr["prtPartNumber"].ToString();
                        }
                    }
                    else
                    {
                        pdfName = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString() + "_" + dr["prtPartNumber"].ToString().Replace(",", "");
                    }
                }
                dr.Close();
            }

            //This is for UGS's quote sumamry sheet
            if (quoteType == 6)
            {
                quotePDF.Open();

                quotePDF.NewPage();

                Font link = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);

                if (Request["rfqID"] != "" && Request["rfqID"] != null)
                {
                    rfqID = System.Convert.ToInt32(Request["rfqID"]);
                }
                else
                {
                    return;
                }

                string custRFQ = "";
                sql.CommandText = "Select rfqCustomerRFQNumber from tblRFQ where rfqID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                SqlDataReader sdr = sql.ExecuteReader();
                if (sdr.Read())
                {
                    custRFQ = sdr.GetValue(0).ToString();
                }
                sdr.Close();

                PdfPTable table = new PdfPTable(4);
                table.DefaultCell.Padding = 2;
                table.DefaultCell.HorizontalAlignment = 1;
                table.DefaultCell.VerticalAlignment = 1;
                PdfPCell cell = new PdfPCell(new Phrase("UGS Quote Summary\n" + "TSG RFQ # " + rfqID + "\nCustomer RFQ # " + custRFQ));
                cell.Colspan = 4;
                //0 = left, 1 = center, 2 = right
                cell.HorizontalAlignment = 1;
                table.AddCell(cell);
                table.AddCell("Picture");
                table.AddCell("Part Description");
                table.AddCell("Part Number");
                table.AddCell("Total Cost");


                sql.CommandText = "Select qtrQuoteID, uquQuoteNumber, uquQuoteVersion, uquPartNumber, TSGCompanyAbbrev, prtRFQLineNumber, uquTotalPrice, prtPicture, uquPartName from linkQuoteToRFQ, tblUGSQuote, linkPartToQuote, tblPart, TSGCompany ";
                sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = uquUGSQuoteID and ptqQuoteID = uquUGSQuoteID and ptqPartID = prtPartID and TSGCompanyID = 15 and qtrHTS <> 1 and qtrSTS <> 1 and qtrUGS = 1 ";
                sql.CommandText += "and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS = 1 order by prtRFQLineNumber ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);



                dr = sql.ExecuteReader();
                //Paragraph p4 = new Paragraph();

                //p4.Add(new Chunk("Quote Summary"));
                //p4.Add(new Chunk("\n\n\n"));
                //p4.Alignment = Element.ALIGN_CENTER;

                List<string> quotes = new List<string>();
                double total = 0;

                while (dr.Read())
                {
                    if (!quotes.Contains(dr.GetValue(0).ToString()))
                    {
                        //p4.Add(new Chunk(dr.GetValue(3).ToString() + "  Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(4).ToString() + "-" + dr.GetValue(2).ToString() + " " + 
                        //    System.Convert.ToDouble(dr["uquTotalPrice"].ToString()).ToString("C", System.Globalization.CultureInfo.CurrentCulture), link).SetLocalGoto(dr.GetValue(0).ToString()));
                        //p4.Add(new Chunk("\n\n"));
                        //total += System.Convert.ToDouble(dr["uquTotalPrice"].ToString());

                        byte[] partPictureData;

                        String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                        String sharepointLibrary = "Part Pictures";
                        bool pictureAdded = false;
                        using (var clientContext = new ClientContext(siteUrl))
                        {
                            clientContext.Credentials = master.getSharePointCredentials();
                            var relativeUrl = "";
                            var url = new Uri(siteUrl);

                            relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, dr.GetValue(7).ToString());

                            // open the file as binary
                            try
                            {
                                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                                using (var memstr = new MemoryStream())
                                {
                                    var buf = new byte[1024 * 16];
                                    int byteSize;
                                    while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                    {
                                        memstr.Write(buf, 0, byteSize);
                                    }
                                    partPictureData = memstr.ToArray();
                                }
                                // bulid the itext picture with the byte array
                                iTextSharp.text.Image prtPicture = iTextSharp.text.Image.GetInstance(partPictureData);
                                // make it fit in our tight quote format.
                                //prtPicture.SetAbsolutePosition(175, 650);
                                //quotePDF.Add(prtPicture);

                                prtPicture.ScaleAbsolute(75, 37);
                                PdfPCell imageCell = new PdfPCell(prtPicture);
                                imageCell.Colspan = 1;
                                imageCell.VerticalAlignment = 1;
                                imageCell.HorizontalAlignment = 1;
                                imageCell.Padding = 4;
                                //imageCell.Border = 0;
                                table.AddCell(imageCell);
                                pictureAdded = true;
                            }
                            catch
                            {

                            }
                        }
                        if (!pictureAdded)
                        {
                            table.AddCell("");
                        }
                        table.AddCell(dr.GetValue(8).ToString());
                        table.AddCell(dr.GetValue(3).ToString());
                        table.AddCell(System.Convert.ToDouble(dr.GetValue(6).ToString()).ToString("c"));
                        total += System.Convert.ToDouble(dr.GetValue(6).ToString());

                    }
                    quotes.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                cell = new PdfPCell(new Phrase("Total: " + total.ToString("c")));
                cell.Colspan = 4;
                cell.HorizontalAlignment = 2;
                table.AddCell(cell);

                quotePDF.Add(table);

                //p4.Add(new Chunk("RFQ Total: " + total.ToString("C", System.Globalization.CultureInfo.CurrentCulture), link));
                //p4.Add(new Chunk("\n\n"));

                //quotePDF.Add(p4);
            }
            // multiple responses to the page so we can download multiple files at once... or we need a bunch of pages which will then just write the file then close
            else if (Request["individual"] != "" && Request["individual"] != null)
            {
                quotePDF.Open();

                quotePDF.NewPage();

                page += createSingleQuote(quoteNumber, quoteType, page);

                quotePDF.Close();

                Response.ContentType = "application/pdf";

                Response.ClearHeaders();
                Response.AddHeader("Content-disposition", "attachment;filename=Quote " + pdfName + ".pdf");
                Byte[] buff = ms.ToArray();
                Response.BinaryWrite(buff);
                connection.Close();
                return;
            }
            else if (Request["rfqID"] != "" && Request["rfqID"] != null)
            {
                rfqID = System.Convert.ToInt32(Request["rfqID"]);

                Boolean onlyMyCompany = false;
                int companyID = System.Convert.ToInt32(master.getCompanyId());
                if (Request["onlyMyCompany"] != null && Request["onlyMyCompany"] != "")
                {
                    onlyMyCompany = System.Convert.ToBoolean(Request["onlyMyCompany"]);
                }

                sql.CommandText = "Select Count(qtrQuoteID) from linkQuoteToRFQ where qtrRFQID = @rfqID";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                dr = sql.ExecuteReader();

                if (dr.Read())
                {
                    numberOfQuotes = System.Convert.ToInt32(dr.GetValue(0));
                }
                dr.Close();

                sql.Parameters.Clear();
                List<string> quoteIDs = new List<string>();

                if(numberOfQuotes != 0)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS, quoTSGCompanyId ";
                    sql.CommandText += "from linkQuoteToRFQ ";
                    sql.CommandText += "left outer join linkPartToQuote on ptqQuoteId = qtrQuoteId and qtrHTS = ptqHTS and qtrSTS = ptqSTS and qtrUGS = ptqUGS ";
                    sql.CommandText += "left outer join tblPart on prtPartId = ptqPartID ";
                    sql.CommandText += "left outer join tblQuote on quoQuoteId = ptqQuoteId ";
                    sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = qtrQuoteId and qtrSTS = atqSTS and qtrHTS = atqHTS and qtrUGS = atqUGS ";
                    sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                    sql.CommandText += "where qtrRFQID = @rfqID ";
                    if (onlyMyCompany && companyID == 9)
                    {
                        sql.CommandText += "and qtrHTS = 1 ";
                    }
                    else if (onlyMyCompany && companyID == 13)
                    {
                        sql.CommandText += "and qtrSTS = 1 ";
                    }
                    else if (onlyMyCompany && companyID == 15)
                    {
                        sql.CommandText += "and qtrUGS = 1 ";
                    }
                    else if (onlyMyCompany && companyID != 1)
                    {
                        sql.CommandText += "and quoTSGCompanyID = @company ";
                        sql.Parameters.AddWithValue("@company", companyID);
                    }
                    sql.CommandText += "order by case when assLineNumber is null then 1 else 0 end, prtRFQLineNumber ";
                    sql.Parameters.AddWithValue("@rfqID", rfqID);


                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        if(quoteIDs.Contains(dr.GetValue(0).ToString()))
                        {
                            continue;
                        }
                        quoteIDs.Add(dr.GetValue(0).ToString());
                        if (page == 1)
                        {
                            quotePDF.Open();

                            quotePDF.NewPage();

                            createFrontPage(rfqID);

                            quotePDF.NewPage();

                            if(!onlyMyCompany)
                            {
                                createTableOfContents(numberOfQuotes, rfqID, "1", false);
                            }
                            else
                            {
                                createTableOfContents(numberOfQuotes, rfqID, companyID.ToString(), false);
                            }

                        }

                        quotePDF.NewPage();

                        if(dr.GetBoolean(1))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 3, page);
                        }
                        else if(dr.GetBoolean(2))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 4, page);
                        }
                        else if(dr.GetBoolean(3))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 5, page);
                        }
                        else
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), quoteType, page);
                        }
                    }
                    dr.Close();
                }
                else
                {
                    quotePDF.Open();

                    quotePDF.NewPage();

                    createFrontPage(rfqID);

                    System.Diagnostics.Debug.WriteLine("Is this the create front page being used? else statement");
                }
            }
            if (quoteNumber != 0)
            {
                quotePDF.Open();

                quotePDF.NewPage();

                page += createSingleQuote(quoteNumber, quoteType, page);
            }

            quotePDF.NewPage();


            //page += createNoQuotes(rfqID);

            //Takes the memory stream and writes the byte array to the browser to display the PDF
            System.Threading.Thread.Sleep(100);

            quotePDF.Close();

            Byte[] buffer = ms.ToArray();

            Response.ContentType = "application/pdf";
            if(rfqID != 0)
            {
                sql.CommandText = "Select rfqCustomerRFQNumber from tblRFQ where rfqID = @rfq";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqID);
                dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    Response.AddHeader("Content-disposition", "inline;filename=RFQ " + dr.GetValue(0).ToString().Replace( "," , "" ) + ".pdf");
                }
            }
            else if(quoteNumber != 0)
            {
                Response.AddHeader("Content-disposition", "inline;filename=Quote " + pdfName + ".pdf");
            }
            Response.BinaryWrite(buffer);
            connection.Close();
        
        }

        public System.Net.Mail.Attachment getPDFAtachment(int rfqID, string companyID, Boolean updated)
        {
            int page = 1;
            int numberOfQuotes = 0;

            quotePDF.AddTitle("TSG Quote");
            quotePDF.SetMargins(30f, 30f, 30f, 30f);

            writer = PdfWriter.GetInstance(quotePDF, ms);

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            sql.Parameters.Clear();
            if(companyID != "1")
            {
                sql.CommandText = "Select Count(qtrQuoteID) from linkQuoteToRFQ, tblQuote where qtrRFQID = @rfqID and qtrQuoteID = quoQuoteID and quoTSGCompanyID = @company and quoSent is null and quoStatusID <> 9 ";
                sql.Parameters.AddWithValue("@company", companyID);
            }
            else
            {
                sql.CommandText = "Select Count(qtrQuoteID) from linkQuoteToRFQ where qtrRFQID = @rfqID";
            }
            sql.Parameters.AddWithValue("@rfqID", rfqID);
            SqlDataReader dr = sql.ExecuteReader();

            if (dr.Read())
            {
                numberOfQuotes = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();

            sql.Parameters.Clear();

            List<string> quotesToUpdate = new List<string>();
            List<string> quotesToUpdateHTS = new List<string>();
            List<string> quotesToUpdateSTS = new List<string>();

            //sql.CommandText = "select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS, prtRFQLineNumber from linkQuoteToRFQ, linkPartToQuote, tblPart where qtrRFQID = @rfqID and ptqQuoteID = qtrQuoteID and ptqPartID = prtPARTID ";
            //sql.CommandText += "and qtrHTS = ptqHTS and qtrUGS = ptqUGS and qtrSTS = ptqSTS and ";
            //sql.CommandText += "prtRFQLineNumber = (Select min(prtRFQLineNumber) from linkPartToQuote as ptq, tblPart where ptq.ptqQuoteID = qtrQuoteID and ptq.ptqHTS = qtrHTS and ptq.ptqUGS = qtrUGS and ptq.ptqSTS = qtrSTS and prtPARTID = ptq.ptqPartID) ";
            //if(updated)
            //{
            //    sql.CommandText += "and qtrSent is null ";
            //}
            //sql.CommandText += " order by prtRFQLineNumber ";

            sql.CommandText = "select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS, prtRFQLineNumber, assLineNumber ";
            sql.CommandText += "from linkQuoteToRFQ ";
            sql.CommandText += "left outer join linkPartToQuote on ptqQuoteId = qtrQuoteId and qtrHTS = ptqHTS and qtrUGS = ptqUGS and qtrSTS = ptqSTS ";
            sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartId and prtRFQLineNumber = (Select min(prtRFQLineNumber) from linkPartToQuote as ptq, tblPart where ptq.ptqQuoteID = qtrQuoteID and ptq.ptqHTS = qtrHTS and ptq.ptqUGS = qtrUGS and ptq.ptqSTS = qtrSTS and prtPARTID = ptq.ptqPartID) ";
            sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = qtrQuoteId ";
            sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
            sql.CommandText += "where qtrRFQID = @rfqID and (prtRFQLineNumber is not null or assLineNumber is not null) ";
            if (updated)
            {
                sql.CommandText += "and qtrSent is null ";
            }
            sql.CommandText += "order by case when assLineNumber is null then 1 else 0 end, prtRFQLineNumber ";

            sql.Parameters.AddWithValue("@rfqID", rfqID);
            dr = sql.ExecuteReader();

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            SqlCommand sql2 = new SqlCommand();
            sql2.Connection = connection2;

            while (dr.Read())
            {
                if (dr.GetBoolean(1) || dr.GetBoolean(2) || dr.GetBoolean(3))
                {
                    if(companyID == "9" || companyID == "1" || companyID == "13" || companyID == "15")
                    {
                        if (page == 1)
                        {
                            quotePDF.Open();

                            quotePDF.NewPage();

                            createFrontPage(rfqID);

                            quotePDF.NewPage();

                            createTableOfContents(numberOfQuotes, rfqID, companyID, updated);

                            quotePDF.NewPage();
                        }
                        string quote = dr.GetValue(0).ToString();
                        if (dr.GetBoolean(1))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 3, page);
                            quotesToUpdateHTS.Add(dr.GetValue(0).ToString());
                        }
                        else if (dr.GetBoolean(2))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 4, page);
                            quotesToUpdateSTS.Add(dr.GetValue(0).ToString());
                        }
                        else if (dr.GetBoolean(3))
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 5, page);
                            quotesToUpdateSTS.Add(dr.GetValue(0).ToString());
                        }
                        else
                        {
                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 2, page);
                            quotesToUpdate.Add(dr.GetValue(0).ToString());
                        }
                        quotePDF.NewPage();
                    }
                }
                else
                {
                    sql2.CommandText = "Select quoTSGCompanyID from tblQuote where quoQuoteID = @quoteID and quoStatusID <> 9";
                    sql2.Parameters.Clear();
                    sql2.Parameters.AddWithValue("@quoteID", dr.GetValue(0).ToString());
                    SqlDataReader dr2 = sql2.ExecuteReader();
                    if(dr2.Read())
                    {
                        if(dr2.GetValue(0).ToString() == companyID || companyID == "1")
                        {
                            if (page == 1)
                            {
                                quotePDF.Open();

                                quotePDF.NewPage();

                                createFrontPage(rfqID);

                                quotePDF.NewPage();

                                createTableOfContents(numberOfQuotes, rfqID, companyID, updated);

                                quotePDF.NewPage();
                            }
                            string quote = dr.GetValue(0).ToString();

                            page += createSingleQuote(System.Convert.ToInt32(dr.GetValue(0)), 2, page);
                            
                            quotePDF.NewPage();
                        }
                    }
                    dr2.Close();
                }
            }

            dr.Close();
            writer.CloseStream = false;
            quotePDF.Close();

            ms.Position = 0;

            sql.Parameters.Clear();
            sql.CommandText = "Select rfqCustomerRFQNumber from tblRFQ where rfqID = @rfqID";
            sql.Parameters.AddWithValue("@rfqID", rfqID);
            dr = sql.ExecuteReader();
            string custRFQNumber = "";
            if(dr.Read())
            {
                custRFQNumber = dr.GetValue(0).ToString();
            }
            dr.Close();

            connection.Close();
            connection2.Close();

            if(custRFQNumber != "")
            {
                return new System.Net.Mail.Attachment(ms, custRFQNumber + ".pdf");
            }
            else
            {
                return new System.Net.Mail.Attachment(ms, rfqID + ".pdf");
            }
        }

        public System.Net.Mail.Attachment getIndividualPDFAtachment(int quoteID, string companyID, int rfqID)
        {
            int page = 1;
            int numberOfQuotes = 0;

            quotePDF.AddTitle("TSG Quote");
            quotePDF.SetMargins(30f, 30f, 30f, 30f);

            writer = PdfWriter.GetInstance(quotePDF, ms);

            quotePDF.Open();
            quotePDF.NewPage();

            //HTS
            if (companyID == "9")
            {
                page += createSingleQuote(quoteID, 3, page);
            }
            //STS
            else if (companyID == "13")
            {
                page += createSingleQuote(quoteID, 4, page);
            }
            //UGS
            else if (companyID == "15")
            {
                page += createSingleQuote(quoteID, 5, page);
            }
            //Die shops
            else
            {
                page += createSingleQuote(quoteID, 2, page);
            }




            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            sql.Parameters.Clear();



            sql.Parameters.Clear();
            sql.CommandText = "Select prtRFQLineNumber, quoVersion, hquVersion, squQuoteVersion, uquQuoteVersion, t1.TSGCompanyAbbrev, quoOldQuoteNumber, t2.TSGCompanyAbbrev as stsCompany, squQuoteNumber, squSTSQuoteID, squECQuote, squECBaseQuoteId, assLineNumber, prtPartNumber ";
            sql.CommandText += "from tblPart, linkPartToQuote ";
            sql.CommandText += "left outer join tblQuote on quoQuoteID = ptqQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
            sql.CommandText += "left outer join TSGCompany as t1 on quoTSGCompanyID = TSGCompanyID ";
            sql.CommandText += "left outer join tblHTSQuote on hquHTSQuoteID = ptqQuoteID and ptqHTS = 1 ";
            sql.CommandText += "left outer join tblSTSQuote on squSTSQuoteID = ptqQuoteID and ptqSTS = 1 ";
            sql.CommandText += "left outer join tblUGSQuote on uquUGSQuoteID = ptqQuoteID and ptqUGS = 1 ";
            sql.CommandText += "left outer join TsgCompany as t2 on t2.tsgCompanyID = squCompanyID ";
            sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteID = squSTSQuoteID and atqSTS = 1 ";
            sql.CommandText += "left outer join tblAssembly on assAssemblyID = atqAssemblyId ";
            sql.CommandText += "where ptqPartID = prtPARTID and ptqQuoteID = @quoteID ";
            sql.Parameters.AddWithValue("@quoteID", quoteID);
            SqlDataReader dr = sql.ExecuteReader();
            string pdfName = "";
            while (dr.Read())
            {
                if (dr.GetValue(1).ToString() != "" && companyID != "9" && companyID != "13" && companyID != "15")
                {
                    if (dr.GetValue(6).ToString() != "")
                    {
                        //pdfName = dr.GetValue(6).ToString() + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(1).ToString();
                        pdfName = dr["quoOldQuoteNumber"].ToString() + "-" + dr["t1.TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString() + "_" + dr["prtPartNumber"].ToString().Replace(",", "");
                    }
                    else
                    {
                        pdfName = rfqID + "-" + dr.GetValue(0).ToString() + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(1).ToString();
                        pdfName = dr["prtRFQLineNumber"].ToString() + "-" + dr["t1.TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString() + "_" + dr["prtPartNumber"].ToString().Replace(",", "");
                    }
                    break;
                }
                else if (companyID == "9" && dr.GetValue(2).ToString() != "")
                {
                    pdfName = rfqID + "-" + dr.GetValue(0).ToString() + "-HTS-" + dr.GetValue(2).ToString() + "_" + dr["prtPartNumber"].ToString().Replace(",", "");
                    break;
                }
                else    if ((companyID == "13" || companyID == "20") && dr.GetValue(3).ToString() != "")
                {
                    // BD - add EC if an EC quote
                    if (dr["squECQuote"].ToString() == "True")
                    {
                        pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + " - " + dr["stsCompany"].ToString() + "-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                    }
                    else
                    {
                        pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + " - " + dr["stsCompany"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                    }

                    if (dr["squQuoteNumber"].ToString().Contains("-"))
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                        }
                        else
                        {
                            pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else if (dr["assLineNumber"].ToString() != "")
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = rfqID + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                        }
                        else
                        {
                            pdfName = rfqID + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else if (rfqID == 0)
                    {
                        if (dr["squQuoteNumber"].ToString() == "")
                        {
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                            }
                            else
                            {
                                pdfName = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                        else
                        {
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                            }
                            else
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                    }
                    else
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                        }
                        else
                        {
                            pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    break;
                }
                else if (companyID == "15" && dr.GetValue(4).ToString() != "")
                {
                    pdfName = rfqID + "-" + dr.GetValue(0).ToString() + "-UGS-" + dr.GetValue(4).ToString();
                    break;
                }
            }
            dr.Close();

            if ((pdfName == "" || pdfName == "0") && (companyID == "13" || companyID == "20"))
            {
                sql.CommandText = "Select squSTSQuoteID, squQuoteNumber, TSGCompanyAbbrev, squQuoteVersion, assLineNumber, prtRFQLineNumber, squECQuote, squECBaseQuoteId, squECQuoteNumber ";
                sql.CommandText += "from tblSTSQuote ";
                sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = squSTSQuoteID and atqSTS = 1 ";
                sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join TSGCompany on TSGCompanyID = squCompanyId ";
                sql.CommandText += "where squSTSQuoteID = @quoteId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteId", quoteID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["squQuoteNumber"].ToString().Contains("-"))
                    {
                        // BD - add EC if an EC quote
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString() + "-EC-" + dr["squECQuoteNumber"].ToString();
                        }
                        else
                        {
                            pdfName = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else if (rfqID == 0)
                    {
                        if (dr["squQuoteNumber"].ToString() == "")
                        {
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                            }
                            else
                            {
                                pdfName = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                        else
                        {
                            if (dr["squECQuote"].ToString() == "True")
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                            }
                            else
                            {
                                pdfName = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                            }
                        }
                    }
                    else if (dr["assLineNumber"].ToString() != "")
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = rfqID + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString()+"-EC-1";
                        }
                        else
                        {
                            pdfName = rfqID + "-A" + dr["assLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                    else
                    {
                        if (dr["squECQuote"].ToString() == "True")
                        {
                            pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + " - " + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString() + "-EC-1";
                        }
                        else
                        {
                            pdfName = rfqID + "-" + dr["prtRFQLineNumber"].ToString() + " - " + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString();
                        }
                    }
                }
                dr.Close();
                // BD-Todo - Right here is where the STS Detailed Quote needs to be apended

            }

            connection.Close();

            writer.CloseStream = false;
            quotePDF.Close();
            ms.Position = 0;

            if (pdfName != "")
            {
                return new System.Net.Mail.Attachment(ms, pdfName + ".pdf");
            }
            else
            {
                return new System.Net.Mail.Attachment(ms, rfqID + ".pdf");
            }
        }

        protected void createFrontPage(int rfqID)
        {
            string customerRFQNumber = "";
            string customerName = "";
            string salesmanName = "";
            string salesmanEmail = "";
            string salesmanMobile = "";
            string estimatorName = "";
            string estimatorPhone = "";
            string estimatorEmail = "";
            int uquEstimatorID = 0;
            int EstimatorID = 0;

            Site master = new Site();

            int companyID = System.Convert.ToInt32(master.getCompanyId());

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            sql.CommandText = "Select rfqCustomerRFQNumber, CustomerName, Name, MobilePhone, Email, rfqCustomerID, TSGCompany.TSGCompanyID";
            sql.CommandText += " from tblRFQ, Customer, TSGSalesman, TSGCompany ";
            sql.CommandText += "where rfqCustomerID = CustomerID and rfqID = @id and rfqSalesman = TSGSalesmanID";
            sql.Parameters.AddWithValue("@id", rfqID);

            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                customerRFQNumber = dr.GetValue(0).ToString();
                customerName = dr.GetValue(1).ToString();
                salesmanName = dr.GetValue(2).ToString();
                salesmanMobile = dr.GetValue(3).ToString();
                salesmanEmail = dr.GetValue(4).ToString();
                //System.Diagnostics.Debug.WriteLine("write Debug TSG company id is" + TSGCompany);
            }
            else
            {
                customerRFQNumber = rfqID.ToString();
            }
            dr.Close();

            byte[] logoData;

            //q.PartPicture = q.PartPicture.Replace(" ", "%20");
            String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
            String sharepointLibrary = "shared documents/logos";

            using (var clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = master.getSharePointCredentials();
                var relativeUrl = "";
                var url = new Uri(siteUrl);

                relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "TSG" + ".png");

                // open the file as binary
                try
                {
                    using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                    // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                    using (var memstr = new MemoryStream())
                    {
                        var buf = new byte[1024 * 16];
                        int byteSize;
                        while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                        {
                            memstr.Write(buf, 0, byteSize);
                        }
                        logoData = memstr.ToArray();
                    }
                    // bulid the itext picture with the byte array
                    iTextSharp.text.Image logoPicture = iTextSharp.text.Image.GetInstance(logoData);
                    // make it fit in our tight quote format.
                    logoPicture.ScaleAbsolute(260, 125);
                    logoPicture.SetAbsolutePosition(175, 650);
                    quotePDF.Add(logoPicture);
                }
                catch
                {

                }
            }

            BaseFont basefont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont boldFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            PdfContentByte cb = writer.DirectContent;
            //try
            //{
            //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
            //    {
            cb.BeginText();
            System.Threading.Thread.Sleep(100);

            //    }
            //}
            //catch
            //{
            //    cb.BeginText();
            //}

            cb.SetFontAndSize(basefont, 14);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "RFQ Response For", 300, 550, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, customerName + " RFQ # " + customerRFQNumber, 300, 530, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "Thank you for considering Tooling Systems Group.", 300, 450, 0);

            //testing values changes 8-1-17
            //System.Diagnostics.Debug.WriteLine("Debug TSG company id is" + TSGCompany);
            if (companyID == 15)
            {
                cb.ShowTextAligned(Element.ALIGN_CENTER, "Please contact the UGS sales team with any questions.", 300, 360, 0);
                //cb.ShowTextAligned(Element.ALIGN_CENTER, "or Chad Gould with any questions.", 300, 340, 0);
                //cb.ShowTextAligned(Element.ALIGN_CENTER, estimatorPhone, 300, 320, 0);
            }
            else if (salesmanName != "None Selected")
            {
                //System.Diagnostics.Debug.WriteLine("faild Debug TSG company id is" + TSGCompany);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "Please contact " + salesmanName + " with any questions.", 300, 360, 0);
                cb.ShowTextAligned(Element.ALIGN_CENTER, salesmanEmail, 300, 340, 0);
                cb.ShowTextAligned(Element.ALIGN_CENTER, salesmanMobile, 300, 320, 0);
            }
            else
            {
                cb.ShowTextAligned(Element.ALIGN_CENTER, "Please email your contact with any questions.", 300, 360, 0);
            }
            if (companyID != 15)
            {
                cb.ShowTextAligned(Element.ALIGN_CENTER, "Please send any future RFQ's to TSGRFQ@toolingsystemsgroup.com", 300, 300, 0);
            }

            //cb.ShowTextAligned(Element.ALIGN_CENTER, "DAN NEEDS TO GIVE US REAL WORDING", 300, 430, 0);

            //try
            //{
            //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
            //    {
            cb.EndText();
            System.Threading.Thread.Sleep(100);

            //        }
            //    }
            //    catch
            //    {
            //        cb.EndText();
            //    }
            connection.Close();
        }

        //Used to create go to links in the quote
        protected void createTableOfContents(int numOfQuotes, int rfqID, string companyID, Boolean updated)
        {
            Font link = FontFactory.GetFont("Arial", 12, Font.UNDERLINE, BaseColor.BLUE);

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            Paragraph p4 = new Paragraph();

            p4.Add(new Chunk("Table of Contents"));
            p4.Add(new Chunk("\n\n\n"));
            p4.Alignment = Element.ALIGN_CENTER;

            List<string> quotes = new List<string>();

            sql.CommandText = "Select squSTSQuoteID, squQuoteVersion, assNumber, 'STS', squECQuote, squECBaseQuoteId, squECQuoteNumber, assLineNumber, TSGCompanyAbbrev ";
            sql.CommandText += "from linkAssemblyToRFQ ";
            sql.CommandText += "inner join linkAssemblyToQuote on atqAssemblyId = atrAssemblyId ";
            sql.CommandText += "inner join tblSTSQuote on squSTSQuoteID = atqQuoteId and atqSTS = 1 ";
            sql.CommandText += "inner join tblAssembly on assAssemblyId = atrAssemblyId ";
            sql.CommandText += "inner join TSGCompany on squCompanyId = TSGCompanyId ";
            sql.CommandText += "where atrRfqId = @rfqId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqId", rfqID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (!quotes.Contains(dr.GetValue(0).ToString()))
                {
                    quotes.Add(dr["squSTSQuoteId"].ToString());
                    p4.Add(new Chunk(dr["assNumber"].ToString() + " Quote # " + rfqID + "-A" + dr["assLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["squQuoteVersion"].ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                    p4.Add(new Chunk("\n\n"));
                }
            }
            dr.Close();

            if (companyID == "1")
            {
                sql.CommandText = "Select qtrQuoteID, quoNumber, quoVersion, prtPartNumber, t1.TSGCompanyAbbrev, prtRFQLineNumber, ";
                sql.CommandText += "hquNumber, hquVersion, squQuoteNumber, squQuoteVersion, uquQuoteNumber, uquQuoteVersion, quoStatusID, t2.TSGCompanyAbbrev as stsCompany ";
                sql.CommandText += "from linkPartToQuote, tblPart, linkQuoteToRFQ ";
                sql.CommandText += "left outer join tblQuote on qtrQuoteID = quoQuoteID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 ";
                sql.CommandText += "left outer join TSGCompany as t1 on t1.TSGCompanyID = quoTSGCompanyID ";
                sql.CommandText += "left outer join tblHTSQuote on qtrQuoteID = hquHTSQuoteID and qtrHTS = 1 ";
                sql.CommandText += "left outer join tblSTSQuote on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                sql.CommandText += "left outer join tblUGSQuote on qtrQuoteID = uquUGSQuoteID and qtrUGS = 1 ";
                sql.CommandText += "left outer join TsgCompany as t2 on t2.tsgCompanyID = squCompanyID ";
                sql.CommandText += "where qtrRFQID = @rfq and ptqQuoteID = qtrQuoteID and ptqHTS = qtrHTS and ptqSTS = qtrSTS and ptqUGS = qtrUGS ";
                sql.CommandText += "and prtPARTID = ptqPartID order by prtRFQLineNumber asc, qtrQuoteID desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqID);
            }
            else if (companyID != "9" && companyID != "13" && companyID != "15" && companyID != "20")
            {
                sql.CommandText = "Select qtrQuoteID, quoNumber, quoVersion, prtPartNumber, TSGCompanyAbbrev, prtRFQLineNumber from linkQuoteToRFQ, tblQuote, linkPartToQuote, tblPart, TSGCompany ";
                sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = quoQuoteID and ptqQuoteID = quoQuoteID and ptqPartID = prtPartID and TSGCompanyID = quoTSGCompanyID and qtrUGS <> 1 and qtrHTS <> 1 and qtrSTS <> 1 ";
                sql.Parameters.Clear();
                if (updated)
                {
                    sql.CommandText += "and qtrSent is null ";
                }
                if (companyID != "")
                {
                    sql.CommandText += "and (quoTSGCompanyID = @companyID or @companyID = 1) ";
                    sql.Parameters.AddWithValue("@companyID", companyID);
                }
                sql.CommandText += "and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS <> 1 order by prtRFQLineNumber asc, qtrQuoteID desc ";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
            }
            else if (companyID == "9")
            {
                sql.CommandText = "Select qtrQuoteID, hquNumber, hquVersion, prtPartNumber, TSGCompanyAbbrev, prtRFQLineNumber from linkQuoteToRFQ, tblHTSQuote, linkPartToQuote, tblPart, TSGCompany ";
                sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = hquHTSQuoteID and ptqQuoteID = hquHTSQuoteID and ptqPartID = prtPartID and TSGCompanyID = 9 and qtrHTS = 1 ";
                if (updated)
                {
                    sql.CommandText += "and qtrSent is null ";
                }
                sql.CommandText += "and qtrSTS <> 1 and qtrUGS <> 1 and qtrHTS = 1 and ptqHTS = 1 and ptqSTS <> 1 and ptqUGS <> 1 order by prtRFQLineNumber asc, qtrQuoteID desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
            }
            else if (companyID == "13" || companyID == "20")
            {
                sql.CommandText = "Select qtrQuoteID, squQuoteNumber, squQuoteVersion, prtPartNumber, t1.TSGCompanyAbbrev, prtRFQLineNumber, t2.TSGCompanyAbbrev as stsCompany from linkQuoteToRFQ, linkPartToQuote, tblPart, TSGCompany as t1, tblSTSQuote ";
                sql.CommandText += "left outer join TsgCompany as t2 on t2.tsgCompanyID = squCompanyID ";
                sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = squSTSQuoteID and ptqQuoteID = squSTSQuoteID and ptqPartID = prtPartID and t2.TSGCompanyID = 13 and qtrHTS <> 1 and qtrSTS = 1 and qtrUGS <> 1 ";
                if (updated)
                {
                    sql.CommandText += "and qtrSent is null ";
                }
                sql.CommandText += "and ptqHTS <> 1 and ptqSTS = 1 and ptqUGS <> 1 order by prtRFQLineNumber asc, qtrQuoteID desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
            }
            else if (companyID == "15")
            {
                sql.CommandText = "Select qtrQuoteID, uquQuoteNumber, uquQuoteVersion, prtPartNumber, TSGCompanyAbbrev, prtRFQLineNumber from linkQuoteToRFQ, tblUGSQuote, linkPartToQuote, tblPart, TSGCompany ";
                sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = uquUGSQuoteID and ptqQuoteID = uquUGSQuoteID and ptqPartID = prtPartID and TSGCompanyID = 15 and qtrHTS <> 1 and qtrSTS <> 1 and qtrUGS = 1 ";
                if (updated)
                {
                    sql.CommandText += "and qtrSent is null ";
                }
                sql.CommandText += "and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS = 1 order by prtRFQLineNumber asc, qtrQuoteID desc ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
            }
            dr = sql.ExecuteReader();

            while (dr.Read())
            {
                if(companyID == "1")
                {
                    if (!quotes.Contains(dr.GetValue(0).ToString()))
                    {
                        //You cannot send any quotes if they are marked as Out To Bit to stop guo ji from sending out $0 quotes
                        if(dr.GetValue(12).ToString() != "9")
                        {
                            if (dr.GetValue(2).ToString() != "")
                            {
                                p4.Add(new Chunk(dr.GetValue(3).ToString() + " Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(4).ToString() + "-" + dr.GetValue(2).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                                p4.Add(new Chunk("\n\n"));
                            }
                            else if (dr.GetValue(7).ToString() != "")
                            {
                                p4.Add(new Chunk(dr.GetValue(3).ToString() + " Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-HTS-" + dr.GetValue(7).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                                p4.Add(new Chunk("\n\n"));
                            }
                            else if (dr.GetValue(9).ToString() != "")
                            {
                                p4.Add(new Chunk(dr.GetValue(3).ToString() + " Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-" + dr["stsCompany"].ToString() + "-" + dr.GetValue(9).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                                p4.Add(new Chunk("\n\n"));
                            }
                            else if (dr.GetValue(11).ToString() != "")
                            {
                                p4.Add(new Chunk(dr.GetValue(3).ToString() + " Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-UGS-" + dr.GetValue(11).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                                p4.Add(new Chunk("\n\n"));
                            }
                        }
                        quotes.Add(dr.GetValue(0).ToString());
                    }
                }
                else
                {
                    if (!quotes.Contains(dr.GetValue(0).ToString()))
                    {
                        p4.Add(new Chunk(dr.GetValue(3).ToString() + "  Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(4).ToString() + "-" + dr.GetValue(2).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
                        p4.Add(new Chunk("\n\n"));
                    }
                    quotes.Add(dr.GetValue(0).ToString());
                }
                
            }
            dr.Close();

            //sql.CommandText = "Select qtrQuoteID, squQuoteNumber, squQuoteVersion, prtPartNumber, TSGCompanyAbbrev, prtRFQLineNumber ";
            //sql.CommandText += "from linkQuoteToRFQ, tblSTSQuote, linkPartToQuote, tblPart, TSGCompany ";
            //sql.CommandText += "where qtrRFQID = @rfqID and qtrQuoteID = squSTSQuoteID and ptqQuoteID = squSTSQuoteID and ptqPartID = prtPartID and TSGCompanyID = 13 order by prtRFQLineNumber";
            //sql.Parameters.Clear();
            //sql.Parameters.AddWithValue("@rfqID", rfqID);
            //dr = sql.ExecuteReader();
            //while (dr.Read())
            //{
            //    if (!quotes.Contains(dr.GetValue(0).ToString()))
            //    {
            //        p4.Add(new Chunk(dr.GetValue(3).ToString() + "  Quote # " + rfqID + "-" + dr.GetValue(5).ToString() + "-" + dr.GetValue(4).ToString() + "-" + dr.GetValue(2).ToString(), link).SetLocalGoto(dr.GetValue(0).ToString()));
            //        p4.Add(new Chunk("\n\n"));
            //    }
            //    quotes.Add(dr.GetValue(0).ToString());
            //}
            //dr.Close();

            sql.CommandText = "";

            quotePDF.Add(p4);
            connection.Close();
        }

        protected int createSingleQuote(int quoteNumber, int quoteType, int page)
        {
            int numOfPages = 1;
            double partHeight = 0;
            try
            {
                List<string> quoteNotes = new List<string>();
                List<string> costNotes = new List<string>();
                List<string> qtyNotes = new List<string>();
                List<string> generalNotes = new List<string>();
                List<string> toolingCostNotes = new List<string>();
                List<string> capitalCostNotes = new List<string>();
                //gives UGS the line item notes
                List<string> ugsNotes = new List<string>();
                FullQuote q = new FullQuote();
                string accessNum = "";
                Site master = new Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                SqlCommand sql = new SqlCommand();
                connection.Open();
                sql.Connection = connection;
                string firstPartName = "", customerCountry = "", salesmanEmail = "";
                string eav = "", machineProcesTime = "";
                Boolean masQuote = false;
                int annualVolume = 0;
                int daysPerYear = 0;
                decimal hoursPerShift = 0;
                decimal shiftsPerDay = 0;
                decimal efficiency = 0;
                decimal secondsPerHour = 0;
                decimal tactTime = 0;
                decimal netPartsPerHour = 0;
                decimal grossPartsPerHour = 0;
                decimal netPartsPerDay = 0;
                string  STSDetailedQuotePdfFileName = "";

                string stsQuoteCompany = "";

                //Select mtyMaterialType from pktblMaterialType, tblPart where prtPartMaterialType = mtyMaterialTypeID and prtPARTID = @partID


                //Getting info for the stand alone and EC quotes
                if (quoteType == 1)
                {
                    sql.CommandText = "Select ecqECQuoteID, ecqTSGCompanyID, TSGCompanyName, ecqRFQNumber, ecqTotalCost, steShippingTerms, ptePaymentTerms, ecqLeadTime, ecqEstimator, Customer.CustomerID, ecqCustomerRFQNumber, ";
                    sql.CommandText += "DieType.Name, cavCavityName, ecqDieFBEng, ecqDieFBMet, ecqDieLREng, ecqDieLRMet, ecqShutHeightEng, ecqShutHeightMet, ecqNumberOfStations, ecqPartNumber, ecqPartName, ShipToName, ";
                    sql.CommandText += "Address1, Address2, Address3, City, State, Zip, mtyMaterialType, tcyToolCountry, tcoAddress1, tcoAddress2, tcoAddress3, tcoCity, tcoState, tcoZip, ecqMaterialThkEng, ";
                    sql.CommandText += "ecqMaterialThkMet, ecqBlankPitchEng, ecqBlankPitchMet, ecqBlankWidthEng, ecqBlankWidthMet, TSGCompanyAbbrev, ecqPicture, TSGSalesman.Name, estFirstName, estLastName, tcoPhoneNumber, ";
                    sql.CommandText += "estEmail, ecqJobNumber, ecqAccessNumber, ecqUseTSG, ecqCustomerContactName, ecqQuoteNumber, ecqVersion, ecqMasQuote, ecqShippingLocation, ecqCreated ";
                    sql.CommandText += "from tblECQuote, Customer, CustomerLocation, TSGCompany, pktblShippingTerms, pktblPaymentTerms, pktblToolCountry, DieType, pktblCavity, pktblMaterialType, TSGSalesman, pktblEstimators ";
                    sql.CommandText += "where ecqECQuoteID = @quoteID and ecqCustomer = Customer.CustomerID and ecqCustomerLocation = CustomerLocationID and TSGCompany.TSGCompanyID = ecqTSGCompanyID and steShippingTermsID = ecqShipping and ";
                    sql.CommandText += "ptePaymentTermsID = ecqPayment and ecqCountryOfOrign = tcyToolCountryID and DieTypeID = ecqDieType and cavCavityID = ecqCavity and mtyMaterialTypeID = ecqMaterialType and ";
                    sql.CommandText += "ecqSalesmanID = TSGSalesman.TSGSalesmanID and ecqEstimator = estEstimatorID";

                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    SqlDataReader dr3 = sql.ExecuteReader();

                    if (dr3.Read())
                    {
                        q.QuoteID = dr3.GetValue(0).ToString() + "-" + dr3.GetValue(43).ToString() + "-SA";
                        try { q.TSGCompanyID = System.Convert.ToInt32(dr3.GetValue(1)); } catch { q.TSGCompanyID = 1; }
                        q.TSGCompany = dr3.GetValue(2).ToString();
                        try { q.RFQID = System.Convert.ToInt32(dr3.GetValue(3)); } catch { q.RFQID = 0; }
                        q.Date = DateTime.Now.ToString("d");
                        if (dateCreated)
                        {
                            try
                            {
                                q.Date = System.Convert.ToDateTime(dr3["ecqCreated"].ToString()).ToString("d");
                            }
                            catch { }
                        }
                        try { q.TotalCost = System.Convert.ToDouble(dr3.GetValue(4)); } catch { q.TotalCost = 0; }
                        q.ShippingTerms = dr3.GetValue(5).ToString();
                        q.PaymentTerms = dr3.GetValue(6).ToString();
                        try { q.LeadTime = dr3.GetValue(7).ToString(); } catch { q.LeadTime = "0"; }
                        try { q.EstimatorID = System.Convert.ToInt32(dr3.GetValue(8)); } catch { q.EstimatorID = 19; }
                        try { q.CustomerID = System.Convert.ToInt32(dr3.GetValue(9)); } catch { q.CustomerID = 0; };
                        q.CustomerRFQNumber = dr3.GetValue(10).ToString();
                        q.DieType = dr3.GetValue(11).ToString();
                        q.Cavity = dr3.GetValue(12).ToString();
                        try { q.fbEng = System.Convert.ToDouble(dr3.GetValue(13)); } catch { q.fbEng = 0; }
                        try { q.fbMet = System.Convert.ToDouble(dr3.GetValue(14)); } catch { q.fbMet = 0; }
                        try { q.lrEng = System.Convert.ToDouble(dr3.GetValue(15)); } catch { q.lrEng = 0; }
                        try { q.lrMet = System.Convert.ToDouble(dr3.GetValue(16)); } catch { q.lrMet = 0; }
                        try { q.ShutHeightEng = System.Convert.ToDouble(dr3.GetValue(17)); } catch { q.ShutHeightEng = 0; }
                        try { q.ShutHeightMet = System.Convert.ToDouble(dr3.GetValue(18)); } catch { q.ShutHeightMet = 0; }
                        q.NumberOfStations = dr3.GetValue(19).ToString();
                        q.PartNumber = dr3.GetValue(20).ToString();
                        q.PartDescription = dr3.GetValue(21).ToString();
                        q.CustomerName = dr3.GetValue(22).ToString();
                        q.CustomerAddress1 = dr3.GetValue(23).ToString();
                        q.CustomerAddress2 = dr3.GetValue(24).ToString();
                        q.CustomerAddress3 = dr3.GetValue(25).ToString();
                        q.CustomerCity = dr3.GetValue(26).ToString();
                        q.CustomerState = dr3.GetValue(27).ToString();
                        q.CustomerZip = dr3.GetValue(28).ToString();
                        q.MaterialType = dr3.GetValue(29).ToString();
                        q.ToolCountry = dr3.GetValue(30).ToString();
                        q.TSGAddress1 = dr3.GetValue(31).ToString();
                        q.TSGAddress2 = dr3.GetValue(32).ToString();
                        q.TSGAddress3 = dr3.GetValue(33).ToString();
                        q.TSGCity = dr3.GetValue(34).ToString();
                        q.TSGState = dr3.GetValue(35).ToString();
                        q.TSGZip = dr3.GetValue(36).ToString();
                        try { q.MaterialThicknessEnglish = System.Convert.ToDouble(dr3.GetValue(37).ToString()); } catch { q.MaterialThicknessEnglish = 0; }
                        try { q.MaterialThicknessMetric = System.Convert.ToDouble(dr3.GetValue(38).ToString()); } catch { q.MaterialThicknessMetric = 0; }
                        try { q.BlankPitchEnglish = System.Convert.ToDouble(dr3.GetValue(39).ToString()); } catch { q.BlankPitchEnglish = 0; }
                        try { q.BlankPitchMetric = System.Convert.ToDouble(dr3.GetValue(40).ToString()); } catch { q.BlankPitchMetric = 0; }
                        try { q.BlankWidthEnglish = System.Convert.ToDouble(dr3.GetValue(41).ToString()); } catch { q.BlankWidthEnglish = 0; }
                        try { q.BlankWidthMetric = System.Convert.ToDouble(dr3.GetValue(42).ToString()); } catch { q.BlankWidthMetric = 0; }
                        q.TSGCompanyAbbrev = dr3.GetValue(43).ToString();
                        q.PartPicture = dr3.GetValue(44).ToString();
                        q.salesman = dr3.GetValue(45).ToString();
                        q.estimatorName = dr3.GetValue(46).ToString() + " " + dr3.GetValue(47).ToString();
                        q.TSGPhone = dr3.GetValue(48).ToString();
                        q.estimatorEmail = dr3.GetValue(49).ToString();
                        q.jobNumber = dr3.GetValue(50).ToString();
                        accessNum = dr3.GetValue(51).ToString();
                        q.logo = dr3.GetValue(52).ToString();
                        q.CustomerContactName = dr3.GetValue(53).ToString();
                        if(dr3.GetValue(54).ToString() != dr3.GetValue(0).ToString())
                        {
                            q.QuoteID = dr3.GetValue(54).ToString() + "-" + dr3.GetValue(43).ToString() + "-SA";
                        }
                        q.QuoteNumber = dr3.GetValue(54).ToString();
                        q.QuoteID += "-" + dr3.GetValue(55).ToString();
                        q.QuoteVersion = dr3.GetValue(55).ToString();
                        try
                        {
                            masQuote = dr3.GetBoolean(56);
                        }
                        catch
                        {

                        }
                        q.shippingLocation = dr3.GetValue(57).ToString();
                    }
                    dr3.Close();

                    if(q.logo == "True")
                    {
                        sql.CommandText = "Select TSGCompanyName, tcoAddress1, tcoCity, tcoState, tcoCountry, tcoZip from TSGCompany where TSGCompanyID = 1";
                        sql.Parameters.Clear();
                        dr3 = sql.ExecuteReader();
                        if(dr3.Read())
                        {
                            q.TSGCompany = dr3.GetValue(0).ToString();
                            q.TSGAddress1 = dr3.GetValue(1).ToString();
                            q.TSGCity = dr3.GetValue(2).ToString();
                            q.TSGState = dr3.GetValue(3).ToString();
                            q.TSGZip = dr3.GetValue(5).ToString();
                        }
                        dr3.Close();
                    }


                    sql.CommandText = "Select pwnPreWordedNote, pwnCostNote from pktblPreWordedNote, linkPWNToECQuote where peqECQuoteID = @quoteID and peqPreWordedNoteID = pwnPreWordedNoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    dr3 = sql.ExecuteReader();

                    while (dr3.Read())
                    {
                        quoteNotes.Add(dr3.GetValue(0).ToString());
                        costNotes.Add(dr3.GetValue(1).ToString());
                    }
                    dr3.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Select gnoGeneralNote from pktblGeneralNote, linkGeneralNoteToECQuote where gneECQuoteID = @quoteID and gneGeneralNoteID = gnoGeneralNoteID";
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr3 = sql.ExecuteReader();

                    while (dr3.Read())
                    {
                        generalNotes.Add(dr3.GetValue(0).ToString());
                    }
                    dr3.Close();
                }
                //Die shop quotes
                else if (quoteType == 2)
                {
                    sql.CommandText = "Select quoQuoteID, quoTSGCompanyID, TSGCompanyName, quoRFQID, quoTotalAmount, quoLeadTime, quoEstimatorID, rfqCustomerID, rfqCustomerRFQNumber, ";
                    sql.CommandText += "prtPartNumber, prtpartDescription, prtPicture, ShipToName, Address1, Address2, Address3, City, State, Zip, ";
                    sql.CommandText += "tcoAddress1, tcoAddress2, tcoAddress3, tcoCity, tcoState, tcoZip, quoToolCountryID, prtPartMaterialType, quoNumber, quoVersion, prtRFQLineNumber, TSGCompanyAbbrev, ";
                    sql.CommandText += "CustomerContact.Name, quoUseTSGLogo, quoUseTSGName, quoPartNumbers, TSGSalesman.Name, estFirstName, estLastName, quoCustomerQuoteNumber, tcoPhoneNumber, estEmail, ";
                    sql.CommandText += "curCurrency, quoAccess, CustomerLocation.Country, TSGSalesman.Email, quoShippingLocation, quoOldQuoteNumber, quoPlant, quoPartName, quoCustomerContact, quoJobNum, quoCreated ";
                    sql.CommandText += "from tblQuote, linkPartToQuote, Customer, CustomerLocation, tblPart, TSGCompany, pktblEstimators, TSGSalesman, pktblCurrency, tblRFQ ";
                    sql.CommandText += "left outer join CustomerContact on CustomerContactID = rfqCustomerContact ";
                    sql.CommandText += "where quoQuoteID = @quoteNum and quoQuoteID = ptqQuoteID and prtPartID = ptqPartID and rfqID = quoRFQID and ";
                    sql.CommandText += "rfqCustomerID = Customer.CustomerID and rfqCustomerID = CustomerLocation.CustomerID and quoTSGCompanyID = TSGCompany.TSGCompanyID and ";
                    sql.CommandText += "quoEstimatorID = estEstimatorID and quoSalesman = TSGSalesman.TSGSalesmanID and rfqPlantID = CustomerLocationID and quoCurrencyID = curCurrencyID ";

                    sql.Parameters.AddWithValue("@quoteNum", quoteNumber);


                    SqlDataReader dr3 = sql.ExecuteReader();
                    //This should be all we need to create the quote and in the correct format except for pulling in pictures
                    string tempPlant = "";
                    if (dr3.Read())
                    {
                        q.QuoteID = dr3.GetValue(0).ToString();
                        try { q.TSGCompanyID = System.Convert.ToInt32(dr3.GetValue(1)); } catch { q.TSGCompanyID = 1; }
                        q.TSGCompany = dr3.GetValue(2).ToString();
                        try { q.RFQID = System.Convert.ToInt32(dr3.GetValue(3)); } catch { q.RFQID = 0; }
                        q.Date = DateTime.Now.ToString("d");
                        if (dateCreated)
                        {
                            try
                            {
                                q.Date = System.Convert.ToDateTime(dr3["quoCreated"].ToString()).ToString("d");
                            }
                            catch { }
                        }
                        try { q.TotalCost = System.Convert.ToDouble(dr3.GetValue(4)); } catch { q.TotalCost = 0; }
                        try { q.LeadTime = dr3.GetValue(5).ToString(); } catch { q.LeadTime = "0"; }
                        try { q.EstimatorID = System.Convert.ToInt32(dr3.GetValue(6)); } catch { q.EstimatorID = 19; }
                        try { q.CustomerID = System.Convert.ToInt32(dr3.GetValue(7)); } catch { q.CustomerID = 0; }
                        q.CustomerRFQNumber = dr3.GetValue(8).ToString();
                        q.PartNumber = dr3.GetValue(9).ToString().Replace("(TSG)", "");
                        q.PartDescription = dr3.GetValue(10).ToString();
                        q.PartPicture = dr3.GetValue(11).ToString();
                        q.CustomerName = dr3.GetValue(12).ToString();
                        q.CustomerAddress1 = dr3.GetValue(13).ToString();
                        q.CustomerAddress2 = dr3.GetValue(14).ToString();
                        q.CustomerAddress3 = dr3.GetValue(15).ToString();
                        q.CustomerCity = dr3.GetValue(16).ToString();
                        q.CustomerState = dr3.GetValue(17).ToString();
                        q.CustomerZip = dr3.GetValue(18).ToString();
                        q.TSGAddress1 = dr3.GetValue(19).ToString();
                        q.TSGAddress2 = dr3.GetValue(20).ToString();
                        q.TSGAddress3 = dr3.GetValue(21).ToString();
                        q.TSGCity = dr3.GetValue(22).ToString();
                        q.TSGState = dr3.GetValue(23).ToString();
                        q.TSGZip = dr3.GetValue(24).ToString();
                        q.ToolCountry = dr3.GetValue(25).ToString();
                        q.MaterialType = dr3.GetValue(26).ToString();
                        q.QuoteNumber = dr3.GetValue(27).ToString();
                        q.QuoteVersion = dr3.GetValue(28).ToString();
                        q.LineNumber = dr3.GetValue(29).ToString();
                        q.TSGCompanyAbbrev = dr3.GetValue(30).ToString();
                        q.CustomerContactName = dr3.GetValue(31).ToString();
                        q.logo = dr3.GetValue(32).ToString();
                        if (System.Convert.ToBoolean(dr3.GetValue(33).ToString()))
                        {
                            q.TSGAddress1 = "555 Plymouth";
                            q.TSGCity = "Grand Rapids";
                            q.TSGState = "MI";
                            q.TSGZip = "49505";
                            q.TSGCompany = "Tooling Systems Group";
                        }
                        q.customerPartNumbers = dr3.GetValue(34).ToString();
                        q.salesman = dr3.GetValue(35).ToString();
                        q.estimatorName = dr3.GetValue(36).ToString() + " " + dr3.GetValue(37).ToString();
                        if (dr3.GetValue(38).ToString() != null && dr3.GetValue(38).ToString() != "")
                        {
                            q.CustomerRFQNumber = dr3.GetValue(38).ToString();
                        }
                        q.TSGPhone = dr3.GetValue(39).ToString();
                        q.estimatorEmail = dr3.GetValue(40).ToString();
                        q.currency = dr3.GetValue(41).ToString();
                        accessNum = dr3.GetValue(42).ToString();
                        customerCountry = dr3.GetValue(43).ToString();
                        salesmanEmail = dr3.GetValue(44).ToString();
                        q.shippingLocation = dr3.GetValue(45).ToString();
                        q.oldQuoteNumber = dr3.GetValue(46).ToString();
                        tempPlant = dr3.GetValue(47).ToString();
                        if(dr3.GetValue(48).ToString() != "")
                        {
                            q.PartDescription = dr3.GetValue(48).ToString();
                        }
                        if(dr3.GetValue(49).ToString() != "")
                        {
                            q.CustomerContactName = dr3.GetValue(49).ToString();
                        }
                        q.jobNumber = dr3.GetValue(50).ToString();
                    }
                    dr3.Close();

                    if(tempPlant != "")
                    {
                        sql.CommandText = "Select ShipToName, Address1, Address2, Address3, City, State, Zip from CustomerLocation where CustomerLocationID = @plant";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@plant", tempPlant);
                        dr3 = sql.ExecuteReader();
                        if(dr3.Read())
                        {
                            q.CustomerName = dr3.GetValue(0).ToString();
                            q.CustomerAddress1 = dr3.GetValue(1).ToString();
                            q.CustomerAddress2 = dr3.GetValue(2).ToString();
                            q.CustomerAddress3 = dr3.GetValue(3).ToString();
                            q.CustomerCity = dr3.GetValue(4).ToString();
                            q.CustomerState = dr3.GetValue(5).ToString();
                            q.CustomerZip = dr3.GetValue(6).ToString();
                        }
                        dr3.Close();
                    }

                    if (q.customerPartNumbers == "")
                    {
                        q.PartNumber = "";
                        sql.CommandText = "Select prtPartNumber from linkPartToQuote, tblPart where ptqQuoteID = @quoteID and ptqPartID = prtPARTID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                        dr3 = sql.ExecuteReader();
                        int count = 0;
                        while (dr3.Read())
                        {
                            if (count != 0)
                            {
                                q.PartNumber += " - ";
                            }
                            else
                            {
                                //To get the first part name to retrieve the correct picture from sharepoint
                                firstPartName = dr3.GetValue(0).ToString();
                            }
                            q.PartNumber += dr3.GetValue(0).ToString();
                            count++;
                        }
                        dr3.Close();
                    }
                    else
                    {
                        q.PartNumber = q.customerPartNumbers;
                        sql.CommandText = "Select prtPartNumber from linkPartToQuote, tblPart where ptqQuoteID = @quoteID and ptqPartID = prtPARTID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                        dr3 = sql.ExecuteReader();
                        int count = 0;
                        if (dr3.Read())
                        {
                            //To get the first part name to retrieve the correct picture from sharepoint
                            firstPartName = dr3.GetValue(0).ToString();
                            count++;
                        }
                        dr3.Close();
                    }

                    sql.CommandText = "Select steShippingTerms, ptePaymentTerms from pktblShippingTerms, pktblPaymentTerms, tblQuote ";
                    sql.CommandText += "where quoQuoteID = @quoteID and quoShippingTermsID = steShippingTermsID and quoPaymentTermsID = ptePaymentTermsID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    try
                    {
                        dr3 = sql.ExecuteReader();
                        while (dr3.Read())
                        {
                            q.ShippingTerms = dr3.GetValue(0).ToString();
                            q.PaymentTerms = dr3.GetValue(1).ToString();
                        }
                    }
                    catch
                    {
                        q.ShippingTerms = "";
                        q.PaymentTerms = "";
                    }

                    dr3.Close();

                    sql.CommandText = "Select dtyFullName, cavCavityName, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, dinSizeShutHeightEnglish, ";
                    sql.CommandText += "dinSizeShutHeightMetric, dinNumberOfStations from tblDieInfo, DieType, linkDieInfoToQuote, pktblCavity where dinDieType = DieTypeID and ";
                    sql.CommandText += "diqDieInfoID = dinDieInfoID and dinCavityID = cavCavityID and diqQuoteID = @quoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    try
                    {
                        dr3 = sql.ExecuteReader();
                        if (dr3.Read())
                        {
                            q.DieType = dr3.GetValue(0).ToString();
                            q.Cavity = dr3.GetValue(1).ToString();
                            q.fbEng = System.Convert.ToDouble(dr3.GetValue(2));
                            q.fbMet = System.Convert.ToDouble(dr3.GetValue(3));
                            q.lrEng = System.Convert.ToDouble(dr3.GetValue(4));
                            q.lrMet = System.Convert.ToDouble(dr3.GetValue(5));
                            q.ShutHeightEng = System.Convert.ToDouble(dr3.GetValue(6));
                            q.ShutHeightMet = System.Convert.ToDouble(dr3.GetValue(7));
                            q.NumberOfStations = dr3.GetValue(8).ToString();
                        }
                        else
                        {
                            q.DieType = "";
                            q.Cavity = "";
                            q.fbEng = 0;
                            q.fbMet = 0;
                            q.lrEng = 0;
                            q.lrMet = 0;
                            q.ShutHeightEng = 0;
                            q.ShutHeightMet = 0;
                            q.NumberOfStations = "";
                        }
                    }
                    catch
                    {
                        q.DieType = "";
                        q.Cavity = "";
                        q.fbEng = 0;
                        q.fbMet = 0;
                        q.lrEng = 0;
                        q.lrMet = 0;
                        q.ShutHeightEng = 0;
                        q.ShutHeightMet = 0;
                        q.NumberOfStations = "";
                    }

                    dr3.Close();

                    sql.CommandText = "Select tcyToolCountry from pktblToolCountry where tcyToolCountryID = @toolCountry";
                    sql.Parameters.AddWithValue("@toolCountry", q.ToolCountry);

                    try
                    {
                        dr3 = sql.ExecuteReader();
                        if (dr3.Read())
                        {
                            q.ToolCountry = dr3.GetValue(0).ToString();
                        }
                        else
                        {
                            q.ToolCountry = "";
                        }
                    }
                    catch
                    {
                        q.ToolCountry = "";
                    }
                    dr3.Close();
                    sql.Parameters.Clear();
                    sql.CommandText = "Select mtyMaterialType from pktblMaterialType where mtyMaterialTypeID = @matType";
                    sql.Parameters.AddWithValue("@matType", q.MaterialType);
                    try
                    {
                        dr3 = sql.ExecuteReader();
                        if (dr3.Read())
                        {
                            q.MaterialType = dr3.GetValue(0).ToString();
                        }
                        else
                        {
                            q.MaterialType = "";
                        }
                    }
                    catch
                    {
                        q.MaterialType = "";
                    }
                    dr3.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Select binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, binMaterialWidthEnglish, binMaterialWidthMetric, mtyMaterialType ";
                    sql.CommandText += "from pktblBlankInfo, tblQuote, pktblMaterialType ";
                    sql.CommandText += "where quoQuoteID = @quote and quoBlankInfoID = binBlankInfoID and binBlankMaterialTypeID = mtyMaterialTypeID";

                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quote", quoteNumber);

                    dr3 = sql.ExecuteReader();

                    if (dr3.Read())
                    {
                        q.MaterialThicknessEnglish = System.Convert.ToDouble(dr3.GetValue(0).ToString());
                        q.MaterialThicknessMetric = System.Convert.ToDouble(dr3.GetValue(1).ToString());
                        q.BlankPitchEnglish = System.Convert.ToDouble(dr3.GetValue(2).ToString());
                        q.BlankPitchMetric = System.Convert.ToDouble(dr3.GetValue(3).ToString());
                        q.BlankWidthEnglish = System.Convert.ToDouble(dr3.GetValue(4).ToString());
                        q.BlankWidthMetric = System.Convert.ToDouble(dr3.GetValue(5).ToString());
                        q.MaterialType = dr3.GetValue(6).ToString();
                    }
                    else
                    {
                        q.MaterialThicknessEnglish = 0;
                        q.MaterialThicknessMetric = 0;
                        q.BlankPitchEnglish = 0;
                        q.BlankPitchMetric = 0;
                        q.BlankWidthEnglish = 0;
                        q.BlankWidthMetric = 0;
                    }


                    dr3.Close();


                    sql.Parameters.Clear();

                    sql.CommandText = "Select pwnPreWordedNote, pwnCostNote ";
                    sql.CommandText += "from linkPWNToQuote, pktblPreWordedNote ";
                    sql.CommandText += "where pwqQuoteID = @quote and pwqPreWordedNoteID = pwnPreWordedNoteID";

                    sql.Parameters.AddWithValue("@quote", quoteNumber);

                    dr3 = sql.ExecuteReader();

                    while (dr3.Read())
                    {
                        quoteNotes.Add(dr3.GetValue(0).ToString());
                        costNotes.Add(dr3.GetValue(1).ToString());
                    }
                    dr3.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Select gnoGeneralNote from pktblGeneralNote, linkGeneralNoteToQuote where gnqQuoteID = @quoteID and gnqGeneralNoteID = gnoGeneralNoteID  and (gnqHTS = 0 or gnqHTS is null)";
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr3 = sql.ExecuteReader();

                    while (dr3.Read())
                    {
                        generalNotes.Add(dr3.GetValue(0).ToString());
                    }
                    dr3.Close();
                }
                //HTS Quotes
                else if(quoteType == 3)
                {
                    sql.CommandText = "Select hquHTSQuoteID, 9, TSGCompanyName, hquRFQID, steShippingTerms, ptePaymentTerms, hquLeadTime, hquEstimatorID, hquCustomerID, hquCustomerRFQNum,  ";
                    sql.CommandText += "DieType.Name, cavCavityName, hquPartNumbers, hquPartName, ShipToName, Address1, Address2, Address3, City, State, Zip, tcoAddress1, tcoAddress2,  ";
                    sql.CommandText += "tcoAddress3, tcoCity, tcoState, tcoZip, TSGCompanyAbbrev, hquPicture, TSGSalesman.Name, estFirstName, estLastName, tcoPhoneNumber, hquJobNumberID,  ";
                    sql.CommandText += "hquUseTSGLogo, hquUseTSGName, hquCreated, estEmail, hquVersion, hquCustomerContactName, hquAccess, hquNumber, curCurrency ";
                    sql.CommandText += "from tblHTSQuote, TSGCompany, pktblShippingTerms, pktblPaymentTerms, DieType, pktblCavity, CustomerLocation, TSGSalesman, pktblEstimators, pktblCurrency ";
                    sql.CommandText += "where TSGCompany.TSGCompanyID = 9 and steShippingTermsID = hquShippingTerms and ptePaymentTermsID = hquPaymentTerms and DieTypeID = hquProcess and cavCavityID = hquCavity and ";
                    sql.CommandText += "CustomerLocationID = hquCustomerLocationID and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and hquEstimatorID = estEstimatorID and hquHTSQuoteID = @quoteID and curCurrencyID = hquCurrencyID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    SqlDataReader dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        q.QuoteID = dr.GetValue(0).ToString();
                        q.TSGCompanyID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        q.TSGCompany = dr.GetValue(2).ToString();
                        q.CustomerRFQNumber = dr.GetValue(3).ToString();
                        q.ShippingTerms = dr.GetValue(4).ToString();
                        q.PaymentTerms = dr.GetValue(5).ToString();
                        q.LeadTime = dr.GetValue(6).ToString();
                        q.EstimatorID = System.Convert.ToInt32(dr.GetValue(7).ToString());
                        q.CustomerID = System.Convert.ToInt32(dr.GetValue(8).ToString());
                        q.CustomerRFQNumber = dr.GetValue(9).ToString();
                        q.DieType = dr.GetValue(10).ToString();
                        q.Cavity = dr.GetValue(11).ToString();
                        q.PartNumber = dr.GetValue(12).ToString();
                        q.PartDescription = dr.GetValue(13).ToString();
                        q.CustomerName = dr.GetValue(14).ToString();
                        q.CustomerAddress1 = dr.GetValue(15).ToString();
                        q.CustomerAddress2 = dr.GetValue(16).ToString();
                        q.CustomerAddress3 = dr.GetValue(17).ToString();
                        q.CustomerCity = dr.GetValue(18).ToString();
                        q.CustomerState = dr.GetValue(19).ToString();
                        q.CustomerZip = dr.GetValue(20).ToString();
                        q.TSGAddress1 = dr.GetValue(21).ToString();
                        q.TSGAddress2 = dr.GetValue(22).ToString();
                        q.TSGAddress3 = dr.GetValue(23).ToString();
                        q.TSGCity = dr.GetValue(24).ToString();
                        q.TSGState = dr.GetValue(25).ToString();
                        q.TSGZip = dr.GetValue(26).ToString();
                        q.TSGCompanyAbbrev = dr.GetValue(27).ToString();
                        q.PartPicture = dr.GetValue(28).ToString();
                        q.salesman = dr.GetValue(29).ToString();
                        q.estimatorName = dr.GetValue(30).ToString() + " " + dr.GetValue(31).ToString();
                        q.TSGPhone = dr.GetValue(32).ToString();
                        q.jobNumber = dr.GetValue(33).ToString();
                        q.logo = dr.GetValue(34).ToString();
                        if (System.Convert.ToBoolean(dr.GetValue(35).ToString()))
                        {
                            q.TSGAddress1 = "555 Plymouth";
                            q.TSGCity = "Grand Rapids";
                            q.TSGState = "MI";
                            q.TSGZip = "49505";
                            q.TSGCompany = "Tooling Systems Group";
                        }
                        q.Date = System.Convert.ToDateTime(dr.GetValue(36).ToString()).Date.ToString("d");
                        q.ToolCountry = "NA TOOL";
                        q.estimatorEmail = dr.GetValue(37).ToString();
                        q.QuoteVersion = dr.GetValue(38).ToString();
                        q.CustomerContactName = dr.GetValue(39).ToString();
                        accessNum = dr.GetValue(40).ToString();
                        q.QuoteNumber = dr.GetValue(41).ToString();

                        //This lets us determine if we need an old rfq and line number or just an old quote id from the stand alone quotes
                        int num;
                        bool result = Int32.TryParse(q.QuoteNumber, out num);
                        if (!result)
                        {
                            q.oldQuoteNumber = q.QuoteNumber;
                        }
                        q.currency = dr["curCurrency"].ToString();
                    }
                    dr.Close();



                    string partID = "";

                    sql.CommandText = "Select prtRFQLineNumber, qtrRFQID, rfqCustomerRFQNumber, Name, prtPartNumber, prtpartDescription, prtPartID from linkPartToQuote, tblPart, linkQuoteToRFQ, tblRFQ, CustomerContact where ptqQuoteID = @quoteID and ptqHTS = 1  and qtrHTS = 1 and ptqPartID = prtPARTID and qtrQuoteID = ptqQuoteID and qtrRFQID = rfqID and rfqCustomerContact = CustomerContactID and qtrHTS = 1";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        q.LineNumber = dr.GetValue(0).ToString();
                        q.RFQID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        if(q.CustomerRFQNumber == "")
                        {
                            q.CustomerRFQNumber = dr.GetValue(2).ToString();
                        }
                        if (q.CustomerContactName == "")
                        {
                            q.CustomerContactName = dr.GetValue(3).ToString();
                        }

                        if (q.PartNumber == "")
                        {
                            q.PartNumber = dr.GetValue(4).ToString();
                            partID = dr.GetValue(6).ToString();
                        }
                        if (q.PartDescription == "")
                        {
                            q.PartDescription = dr.GetValue(5).ToString();
                        }
                    }
                    dr.Close();

                    if(partID != "")
                    {
                        sql.CommandText = "Select distinct prtPartNumber from linkPartToPartDetail, tblPart where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID) and prtPARTID = ppdPartID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@partID", partID);
                        dr = sql.ExecuteReader();
                        int count = 0;
                        while (dr.Read())
                        {
                            if (count == 0)
                            {
                                q.PartNumber = dr.GetValue(0).ToString();
                            }
                            else
                            {
                                q.PartNumber += " - " + dr.GetValue(0).ToString();
                            }
                            count++;
                        }
                        dr.Close();
                    }



                    sql.CommandText = "Select hpwNote, hpwQuantity, hpwUnitPrice from pktblHTSPreWordedNote, linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID and pthHTSPWNID = hpwHTSPreWordedNoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    while(dr.Read())
                    {
                        if(dr.GetValue(0).ToString() == "")
                        {
                            quoteNotes.Add("\n");
                        }
                        else
                        {
                            quoteNotes.Add(dr.GetValue(0).ToString());
                        }
                        qtyNotes.Add(dr.GetValue(1).ToString());
                        costNotes.Add(dr.GetValue(2).ToString());
                    }
                    dr.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Select gnoGeneralNote from linkGeneralNoteToQuote, pktblGeneralNote where gnqQuoteID = @quoteID and gnqHTS = 1 and gnoGeneralNoteID = gnqGeneralNoteID";
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        generalNotes.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();
                }
                //STS Quotes
                else if (quoteType == 4)
                {
                    //TODO modify query to allow quote to show as nachi
                    sql.CommandText = "select squSTSQuoteID, t1.TSGCompanyID, t1.TSGCompanyName, squRFQNum, squLeadTime, squEstimatorID, squCustomerID, squCustomerRFQNum,  ";
                    sql.CommandText += "squPartNumber, squPartName, squPicture, ShipToName, Address1, Address2, Address3, City, State, Zip, t1.tcoAddress1, t1.tcoAddress2, t1.tcoAddress3, t1.tcoCity,  ";
                    sql.CommandText += "t1.tcoState, t1.tcoZip, squQuoteNumber, squQuoteVersion, t1.TSGCompanyAbbrev, squCustomerContact, TSGSalesman.Name, ";
                    sql.CommandText += "estFirstName, estLastName, t1.tcoPhoneNumber, estEmail, curCurrency, CustomerLocation.Country, TSGSalesman.Email, squProcess, GETDATE(), squEAV, squMachineTime, ";
                    sql.CommandText += "steShippingTerms, ptePaymentTerms, squUseTSG, t2.TSGCompanyAbbrev as quoteAbbrev, squAnnualVolume, squDaysPerYear, squHoursPerShift, squShiftsPerDay, squEfficiency, ";
                    sql.CommandText += "squSecondsPerHour, squTactTime, squNetPartsPerHour, squGrossPartsPerHour, squNetPartsPerDay, squCellPicture, squFirmQuote, squCreated, squDetailedQuotePdf,squECQuote, squECBaseQuoteId, squECQuoteNumber ";
                    sql.CommandText += "from Customer, CustomerLocation, TSGCompany as t1, TSGSalesman, pktblEstimators, pktblCurrency, pktblShippingTerms, pktblPaymentTerms, tblSTSQuote ";
                    sql.CommandText += "left outer join TSGCompany as t2 on t2.TSGCompanyID = squCompanyID ";
                    sql.CommandText += "Where squSTSQuoteID = @quoteID and squCustomerID = Customer.CustomerID and CustomerLocationID = squPlantID and t1.TSGCompanyID = 13 and curCurrencyID = 1 and estEstimatorID = squEstimatorID ";
                    sql.CommandText += "and squSalesmanID = TSGSalesman.TSGSalesmanID and squPaymentID = ptePaymentTermsID and steShippingTermsID = squShippingID ";

                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    SqlDataReader dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        annualVolume = System.Convert.ToInt32(dr["squAnnualVolume"].ToString());
                        daysPerYear = System.Convert.ToInt32(dr["squDaysPerYear"].ToString());
                        hoursPerShift = System.Convert.ToDecimal(dr["squHoursPerShift"].ToString());
                        shiftsPerDay = System.Convert.ToDecimal(dr["squShiftsPerDay"].ToString());
                        efficiency = System.Convert.ToDecimal(dr["squEfficiency"].ToString());
                        secondsPerHour = System.Convert.ToDecimal(dr["squSecondsPerHour"].ToString());
                        tactTime = System.Convert.ToDecimal(dr["squTactTime"].ToString());
                        netPartsPerHour = System.Convert.ToDecimal(dr["squNetPartsPerHour"].ToString());
                        grossPartsPerHour = System.Convert.ToDecimal(dr["squGrossPartsPerHour"].ToString());
                        netPartsPerDay = System.Convert.ToDecimal(dr["squNetPartsPerDay"].ToString());

                        q.QuoteID = dr.GetValue(0).ToString();
                        q.TSGCompanyID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        q.TSGCompany = dr.GetValue(2).ToString();
                        try
                        {
                            q.RFQID = System.Convert.ToInt32(dr.GetValue(3).ToString());
                        }
                        catch
                        {
                            q.RFQID = 0;
                        }
                        q.LeadTime = dr.GetValue(4).ToString();
                        q.EstimatorID = System.Convert.ToInt32(dr.GetValue(5).ToString());
                        q.CustomerID = System.Convert.ToInt32(dr.GetValue(6).ToString());
                        q.CustomerRFQNumber = dr.GetValue(7).ToString();
                        q.PartNumber = dr.GetValue(8).ToString();
                        q.PartDescription = dr.GetValue(9).ToString();
                        q.PartPicture = dr.GetValue(10).ToString();
                        q.CustomerName = dr.GetValue(11).ToString();
                        q.CustomerAddress1 = dr.GetValue(12).ToString();
                        q.CustomerAddress2 = dr.GetValue(13).ToString();
                        q.CustomerAddress3 = dr.GetValue(14).ToString();
                        q.CustomerCity = dr.GetValue(15).ToString();
                        q.CustomerState = dr.GetValue(16).ToString();
                        q.CustomerZip = dr.GetValue(17).ToString();
                        q.TSGAddress1 = dr.GetValue(18).ToString();
                        q.TSGAddress2 = dr.GetValue(19).ToString();
                        q.TSGAddress3 = dr.GetValue(20).ToString();
                        q.TSGCity = dr.GetValue(21).ToString();
                        q.TSGState = dr.GetValue(22).ToString();
                        q.TSGZip = dr.GetValue(23).ToString();
                        q.QuoteNumber = dr.GetValue(24).ToString();
                        q.QuoteVersion = dr.GetValue(25).ToString();
                        q.TSGCompanyAbbrev = dr.GetValue(26).ToString();
                        stsQuoteCompany = dr["quoteAbbrev"].ToString();
                        q.CustomerContactName = dr.GetValue(27).ToString();
                        q.salesman = dr.GetValue(28).ToString();
                        q.estimatorName = dr.GetValue(29).ToString() + " " + dr.GetValue(30).ToString();
                        q.TSGPhone = dr.GetValue(31).ToString();
                        q.estimatorEmail = dr.GetValue(32).ToString();
                        q.currency = dr.GetValue(33).ToString();
                        customerCountry = dr.GetValue(34).ToString();
                        salesmanEmail = dr.GetValue(35).ToString();
                        //For STS this is their process
                        q.DieType = dr.GetValue(36).ToString();
                        q.Date = DateTime.Now.ToString("d");
                        if (dateCreated)
                        {
                            try
                            {
                                q.Date = System.Convert.ToDateTime(dr["squCreated"].ToString()).ToString("d");
                            }
                            catch { }
                        }
                        //q.Date = System.Convert.ToDateTime(dr.GetValue(37).ToString()).Date.ToString("d");
                        eav = dr.GetValue(38).ToString();
                        machineProcesTime = dr.GetValue(39).ToString();
                        q.ShippingTerms = dr.GetValue(40).ToString();
                        q.PaymentTerms = dr.GetValue(41).ToString();
                        q.logo = dr.GetValue(42).ToString();
                        q.cellPicture = dr["squCellPicture"].ToString();
                        if (System.Convert.ToBoolean(dr.GetValue(42).ToString()))
                        {
                            q.TSGAddress1 = "555 Plymouth";
                            q.TSGCity = "Grand Rapids";
                            q.TSGState = "MI";
                            q.TSGZip = "49505";
                            q.TSGCompany = "Tooling Systems Group";
                        }
                        if (dr["squFirmQuote"].ToString() != "")
                        {
                            q.Firm = System.Convert.ToBoolean(dr["squFirmQuote"].ToString());
                        }
                        else
                        {
                            q.Firm = null;
                        }
// BD - Get EC quote info
                        q.ECBaseQuoteId = dr["squECBaseQuoteId"].ToString();
                        q.ECQuote = dr["squECQuote"].ToString();
                        q.ECQuoteNumber = dr["squECQuoteNumber"].ToString();
                        //This lets us determine if we need an old rfq and line number or just an old quote id from the stand alone quotes
                        int num;
                        bool result = Int32.TryParse(q.QuoteNumber, out num);
                        if (!result)
                        {
                            q.oldQuoteNumber = q.QuoteNumber;
                        }
                        
                        if (dr["squFirmQuote"].ToString() != "")
                        {
                            q.Firm = System.Convert.ToBoolean(dr["squFirmQuote"].ToString());
                        }
 
                        if (dr["squDetailedQuotePdf"].ToString() != "")
                        {
                            STSDetailedQuotePdfFileName = System.Convert.ToString(dr["squDetailedQuotePdf"].ToString());
                        }
                        
                    }
                    dr.Close();

                    sql.CommandText = "Select prtRFQLineNumber, qtrRFQID, rfqCustomerRFQNumber, Name from linkPartToQuote, tblPart, linkQuoteToRFQ, tblRFQ, CustomerContact where ptqQuoteID = @quoteID and ptqSTS = 1 and ptqPartID = prtPARTID and qtrQuoteID = ptqQuoteID and qtrRFQID = rfqID and rfqCustomerContact = CustomerContactID and qtrSTS = 1";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        q.LineNumber = dr.GetValue(0).ToString();
                        q.RFQID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        if (q.CustomerRFQNumber == "")
                        {
                            q.CustomerRFQNumber = dr.GetValue(2).ToString();
                        }
                        if (q.CustomerContactName == "")
                        {
                            q.CustomerContactName = dr.GetValue(3).ToString();
                        }
                    }
                    dr.Close();

                    if (q.LineNumber == null)
                    {
                        sql.CommandText = "Select assLineNumber as lineNumber, rfqID, rfqCustomerRFQNumber, Name from linkAssemblyToRFQ, tblAssembly, linkAssemblyToQuote, tblRFQ, CustomerContact ";
                        sql.CommandText += "where atqQuoteId = @id and atqSTS = 1 and atrAssemblyId = atqAssemblyId and assAssemblyId = atqAssemblyId and atrRfqId = rfqID and rfqCustomerContact = CustomerContactID ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@id", quoteNumber);
                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            q.LineNumber = "A" + dr["lineNumber"].ToString();
                            q.RFQID = System.Convert.ToInt32(dr["rfqID"].ToString());
                            if (q.CustomerRFQNumber == "")
                            {
                                q.CustomerRFQNumber = dr["rfqCustomerRFQNumber"].ToString();
                            }
                            if (q.CustomerContactName == "")
                            {
                                q.CustomerContactName = dr["Name"].ToString();
                            }
                        }
                        dr.Close();
                    }


                    sql.CommandText = "Select pwnPreWordedNote, pwnCostNote from pktblPreWordedNote, linkPWNToSTSQuote where psqSTSQuoteID = @quoteID and psqPreWordedNoteID = pwnPreWordedNoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        quoteNotes.Add(dr.GetValue(0).ToString());
                        costNotes.Add(dr.GetValue(1).ToString());
                    }
                    dr.Close();


                    sql.CommandText = "Select sqnSTSQuoteNotesID, sqnDescription, sqnToolingCosts, sqnCapitalCosts from pktblSTSQuoteNotes where sqnQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        quoteNotes.Add(dr["sqnDescription"].ToString());
                        toolingCostNotes.Add(dr["sqnToolingCosts"].ToString());
                        capitalCostNotes.Add(dr["sqnCapitalCosts"].ToString());
                    }
                    dr.Close();


                    sql.CommandText = "Select gnoGeneralNote from linkGeneralNoteToSTSQuote, pktblGeneralNote where gnsSTSQuoteID = @quoteID and gnoGeneralNoteID = gnsGeneralNoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        generalNotes.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    sql.CommandText = "Select TSGCompanyName, TSGCompanyAbbrev, tcoAddress1, tcoAddress2, tcoAddress3, tcoCity, tcoState, tcoCountry, tcoZip, tcoPhoneNumber from TSGCompany where TSGCompanyAbbrev = @abbrev";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@abbrev", stsQuoteCompany);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        q.TSGCompany = dr["TSGCompanyName"].ToString();
                        q.TSGCompanyAbbrev = dr["TSGCompanyAbbrev"].ToString();
                        q.TSGAddress1 = dr["tcoAddress1"].ToString();
                        q.TSGAddress2 = dr["tcoAddress2"].ToString();
                        q.TSGAddress3 = dr["tcoAddress3"].ToString();
                        q.TSGCity = dr["tcoCity"].ToString();
                        q.TSGState = dr["tcoState"].ToString();
                        q.TSGZip = dr["tcoZip"].ToString();
                        q.TSGPhone = dr["tcoPhoneNumber"].ToString();
                    }
                    dr.Close();
                }
                //UGS Quotes
                else if (quoteType == 5)
                {
                    sql.CommandText = "select uquUGSQuoteID, TSGCompany.TSGCompanyID, TSGCompanyName, uquRFQID, uquLeadTime, uquEstimatorID, uquCustomerID, uquCustomerRFQNumber,  ";
                    sql.CommandText += "uquPartNumber, uquPartName, uquPicture, ShipToName, Address1, Address2, Address3, City, State, Zip, tcoAddress1, tcoAddress2, tcoAddress3, tcoCity,   ";
                    sql.CommandText += "tcoState, tcoZip, uquQuoteNumber, uquQuoteVersion, TSGCompanyAbbrev, uquCustomerContact, TSGSalesman.Name,  ";
                    sql.CommandText += "estFirstName, estLastName, tcoPhoneNumber, estEmail, curCurrency, CustomerLocation.Country, TSGSalesman.Email, GETDATE(), steShippingTerms, ";
                    sql.CommandText += "ptePaymentTerms, uquUseTSG, uquNotes, uquTotalPrice, uquShippingLocation, dtyFullName, uquPartLength, uquPartWidth, uquPartHeight, uquCreated ";
                    sql.CommandText += "from tblUGSQuote, Customer, CustomerLocation, TSGCompany, TSGSalesman, pktblEstimators, pktblCurrency, pktblShippingTerms, pktblPaymentTerms, DieType ";
                    sql.CommandText += "Where uquUGSQuoteID = @quoteID and uquCustomerID = Customer.CustomerID and CustomerLocationID = uquPlantID and TSGCompany.TSGCompanyID = 15 and curCurrencyID = 1 and estEstimatorID = uquEstimatorID ";
                    sql.CommandText += "and uquSalesmanID = TSGSalesman.TSGSalesmanID and uquPaymentID = ptePaymentTermsID and steShippingTermsID = uquShippingID and DieTypeID = uquDieType ";

                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);

                    SqlDataReader dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        q.QuoteID = dr.GetValue(0).ToString();
                        q.TSGCompanyID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        q.TSGCompany = dr.GetValue(2).ToString();
                        try
                        {
                            q.RFQID = System.Convert.ToInt32(dr.GetValue(3).ToString());
                        }
                        catch
                        {
                            q.RFQID = 0;
                        }
                        q.LeadTime = dr.GetValue(4).ToString();
                        q.EstimatorID = System.Convert.ToInt32(dr.GetValue(5).ToString());
                        q.CustomerID = System.Convert.ToInt32(dr.GetValue(6).ToString());
                        q.CustomerRFQNumber = dr.GetValue(7).ToString();
                        q.PartNumber = dr.GetValue(8).ToString();
                        q.PartDescription = dr.GetValue(9).ToString();
                        q.PartPicture = dr.GetValue(10).ToString();
                        q.CustomerName = dr.GetValue(11).ToString();
                        q.CustomerAddress1 = dr.GetValue(12).ToString();
                        q.CustomerAddress2 = dr.GetValue(13).ToString();
                        q.CustomerAddress3 = dr.GetValue(14).ToString();
                        q.CustomerCity = dr.GetValue(15).ToString();
                        q.CustomerState = dr.GetValue(16).ToString();
                        q.CustomerZip = dr.GetValue(17).ToString();
                        q.TSGAddress1 = dr.GetValue(18).ToString();
                        q.TSGAddress2 = dr.GetValue(19).ToString();
                        q.TSGAddress3 = dr.GetValue(20).ToString();
                        q.TSGCity = dr.GetValue(21).ToString();
                        q.TSGState = dr.GetValue(22).ToString();
                        q.TSGZip = dr.GetValue(23).ToString();
                        q.QuoteNumber = dr.GetValue(24).ToString();
                        q.QuoteVersion = dr.GetValue(25).ToString();
                        q.TSGCompanyAbbrev = dr.GetValue(26).ToString();
                        q.CustomerContactName = dr.GetValue(27).ToString();
                        q.salesman = dr.GetValue(28).ToString();
                        q.estimatorName = dr.GetValue(29).ToString() + " " + dr.GetValue(30).ToString();
                        q.TSGPhone = dr.GetValue(31).ToString();
                        q.estimatorEmail = dr.GetValue(32).ToString();
                        q.currency = dr.GetValue(33).ToString();
                        customerCountry = dr.GetValue(34).ToString();
                        salesmanEmail = dr.GetValue(35).ToString();
                        //For STS this is their process
                        //q.DieType = dr.GetValue(36).ToString();
                        //q.Date = System.Convert.ToDateTime(dr.GetValue(36).ToString()).Date.ToString("d");
                        //eav = dr.GetValue(38).ToString();
                        //machineProcesTime = dr.GetValue(39).ToString();
                        q.ShippingTerms = dr.GetValue(37).ToString();
                        q.PaymentTerms = dr.GetValue(38).ToString();
                        q.logo = dr.GetValue(39).ToString();
                        if (System.Convert.ToBoolean(dr.GetValue(39).ToString()))
                        {
                            q.TSGAddress1 = "555 Plymouth";
                            q.TSGCity = "Grand Rapids";
                            q.TSGState = "MI";
                            q.TSGZip = "49505";
                            q.TSGCompany = "Tooling Systems Group";


                            //q.TSGAddress1 = "555 Plymouth";
                            //q.TSGCity = "Grand Rapids";
                            //q.TSGState = "MI";
                            //q.TSGZip = "49505";
                            //q.TSGCompany = "Hot Stamping Tooling Systems";
                        }
                        quoteNotes.Add(dr.GetValue(40).ToString());
                        q.TotalCost = System.Convert.ToDouble(dr.GetValue(41));
                        q.shippingLocation = dr.GetValue(42).ToString();
                        q.DieType = dr.GetValue(43).ToString();
                        if (dr.GetValue(44).ToString() != "")
                        {
                            q.BlankPitchEnglish = System.Convert.ToDouble(dr.GetValue(44).ToString());
                        }
                        else
                        {
                            q.BlankPitchEnglish = 0;
                        }
                        if(dr.GetValue(45).ToString() != "")
                        {
                            q.BlankWidthEnglish = System.Convert.ToDouble(dr.GetValue(45).ToString());
                        }
                        else
                        {
                            q.BlankWidthEnglish = 0;
                        }
                        if(dr.GetValue(46).ToString() != "")
                        {
                            partHeight = System.Convert.ToDouble(dr.GetValue(46).ToString());
                        }
                        q.Created = System.Convert.ToDateTime(dr["uquCreated"].ToString());
                        //This lets us determine if we need an old rfq and line number or just an old quote id from the stand alone quotes
                        int num;
                        bool result = Int32.TryParse(q.QuoteNumber, out num);
                        if (!result)
                        {
                            q.oldQuoteNumber = q.QuoteNumber;
                        }
                        if (dateCreated)
                        {
                            try
                            {
                                q.Date = System.Convert.ToDateTime(dr["uquCreated"].ToString()).ToString("d");
                            }
                            catch { }
                        }
                    }
                    dr.Close();
                    //q.DieType = "Temp";
                    q.Cavity = "";

                    sql.CommandText = "Select prtRFQLineNumber, qtrRFQID, rfqCustomerRFQNumber, Name from linkPartToQuote, tblPart, linkQuoteToRFQ, tblRFQ, CustomerContact where ptqQuoteID = @quoteID and ptqUGS = 1 and ptqPartID = prtPARTID and qtrQuoteID = ptqQuoteID and qtrRFQID = rfqID and rfqCustomerContact = CustomerContactID and qtrUGS = 1";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        q.LineNumber = dr.GetValue(0).ToString();
                        q.RFQID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                        if (q.CustomerRFQNumber == "")
                        {
                            q.CustomerRFQNumber = dr.GetValue(2).ToString();
                        }
                        if (q.CustomerContactName == "")
                        {
                            q.CustomerContactName = dr.GetValue(3).ToString();
                        }
                    }
                    dr.Close();


                    sql.CommandText = "Select gnoGeneralNote from linkGeneralNoteToUGSQuote, pktblGeneralNote where gnuUGSQuoteID = @quoteID and gnoGeneralNoteID = gnuGeneralNoteID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();

                    while (dr.Read())
                    {
                        generalNotes.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();

                    //ugsNotes

                    sql.CommandText = "Select pwnPreWordedNote, pwnCostNote from linkPWNToUGSQuote, pktblPreWordedNote ";
                    sql.CommandText += "where puqPreWordedNoteID = pwnPreWordedNoteID and puqUGSQuoteID = @quoteID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@quoteID", quoteNumber);
                    dr = sql.ExecuteReader();
                    while(dr.Read())
                    {
                        ugsNotes.Add(dr.GetValue(0).ToString());
                        costNotes.Add(dr.GetValue(1).ToString());
                    }
                    dr.Close();


                    //dr.Close();
                }


                Paragraph p5 = new Paragraph();
                p5.Add(new Chunk(" ").SetLocalDestination(q.QuoteID));

                quotePDF.Add(p5);


                if(q.TSGCompanyAbbrev != "BTS")
                {
                    q.TSGAddress2 = "";
                    q.TSGAddress3 = "";
                }

                connection.Close();

                byte[] logoData;

                //q.PartPicture = q.PartPicture.Replace(" ", "%20");
                String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                String sharepointLibrary = "shared documents/logos";
                String sharepointSubFolder = "";


                //https://toolingsystemsgroup.sharepoint.com/sites/Estimating/shared documents/logos

                using (var clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = master.getSharePointCredentials();
                    var relativeUrl = "";
                    var url = new Uri(siteUrl);
                    if (q.logo != "True")
                    {
                        relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, q.TSGCompanyAbbrev + ".png");
                    }
                    else
                    {
                        relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "TSG" + ".png");
                    }
                    // open the file as binary
                    try
                    {
                        using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                        // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                        using (var memstr = new MemoryStream())
                        {
                            var buf = new byte[1024 * 16];
                            int byteSize;
                            while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                            {
                                memstr.Write(buf, 0, byteSize);
                            }
                            logoData = memstr.ToArray();
                        }
                        // bulid the itext picture with the byte array
                        iTextSharp.text.Image logoPicture = iTextSharp.text.Image.GetInstance(logoData);
                        // make it fit in our tight quote format.
                        logoPicture.ScaleAbsolute(125, 60);
                        logoPicture.SetAbsolutePosition(30, 770);
                        quotePDF.Add(logoPicture);
                    }
                    catch
                    {

                    }
                }

                byte[] UGSPicture;


                if (q.TSGCompanyAbbrev == "UGS")
                {
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();
                        var relativeUrl = "";
                        var url = new Uri(siteUrl);

                        relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "L-A-B_Div_A-S-B_Acred_ISO_Sm.jpg");
                        if (q.Created >= System.Convert.ToDateTime("2/8/2018"))
                        {
                            //relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "ANAB-Dim-Meas-Color.jpg");
                            relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "ANAB_2022.png");
                        }

                        // open the file as binary
                        try
                        {
                            using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                            // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                            using (var memstr = new MemoryStream())
                            {
                                var buf = new byte[1024 * 16];
                                int byteSize;
                                while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    memstr.Write(buf, 0, byteSize);
                                }
                                UGSPicture = memstr.ToArray();
                            }
                            // bulid the itext picture with the byte array
                            iTextSharp.text.Image logoPicture = iTextSharp.text.Image.GetInstance(UGSPicture);
                            // make it fit in our tight quote format.
                            if (q.Created >= System.Convert.ToDateTime("2/8/2018"))
                            {
                                logoPicture.ScaleAbsolute(75, 45);
                                logoPicture.SetAbsolutePosition(438, 604);
                            }
                            else
                            {
                                logoPicture.ScaleAbsolute(150, 35);
                                logoPicture.SetAbsolutePosition(400, 610);
                            }
                            quotePDF.Add(logoPicture);
                        }
                        catch
                        {

                        }
                    }
                }

                //L-A-B_Div_A-S-B_Acred_ISO_Sm.jpg

                //    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(new Uri("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/shared documents/logos"));
                //logo.ScaleAbsolute(100f, 50f);
                //logo.SetAbsolutePosition(30, 775);
                //quotePDF.Add(logo);

                //Taking the first part name instead of the two parts linked together
                String pictureName = q.PartPicture;
                string cellPicture = q.cellPicture;
                //if (q.LineNumber != "")
                //{
                //    pictureName = "RFQ" + q.RFQID + "_" + q.LineNumber + "_" + firstPartName + ".png";
                //}
                //else
                //{
                //    pictureName = "RFQ" + q.RFQID + "_" + firstPartName + ".png";
                //}

                //if (quoteType != 4)
                //{
                try
                {
                    pictureName = pictureName.Replace(" ", "%20");

                    if (cellPicture != null && cellPicture != "")
                    {
                        cellPicture = cellPicture.Replace(" ", "%20");
                    }


                    // This points to where the pictures are
                    if (quoteType <= 2)
                    {
                        sharepointLibrary = "Part Pictures";
                    }
                    else if (quoteType == 4)
                    {
                        sharepointLibrary = "STS Quote Pictures";
                    }
                    else if (quoteType == 5)
                    {
                        if(pictureName.StartsWith("RFQ"))
                        {
                            sharepointLibrary = "Part Pictures";
                        }
                        else
                        {
                            sharepointLibrary = "UGS Pictures";
                        }
                    }
                    else
                    {
                        sharepointLibrary = "HTS Quote Pictures";
                    }

                    // Needed in order to work with ITextSharp - they can work with bytes or absolute paths


                    byte[] pictureData;
                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        clientContext.Credentials = master.getSharePointCredentials();

                        var url = new Uri(siteUrl);
                        var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, pictureName);
                        // open the file as binary
                        try
                        {
                            using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                            // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                            using (var memstr = new MemoryStream())
                            {
                                var buf = new byte[1024 * 16];
                                int byteSize;
                                while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    memstr.Write(buf, 0, byteSize);
                                }
                                pictureData = memstr.ToArray();
                            }
                            // bulid the itext picture with the byte array
                            iTextSharp.text.Image partPicture = iTextSharp.text.Image.GetInstance(pictureData);
                            // make it fit in our tight quote format.
                            partPicture.ScaleAbsolute(150f, 100f);
                            partPicture.SetAbsolutePosition(400, 650);
                            quotePDF.Add(partPicture);
                        }
                        catch
                        {

                        }

                        if (cellPicture != "")
                        {
                            // Starting the cell picture code
                            clientContext.Credentials = master.getSharePointCredentials();

                            url = new Uri(siteUrl);
                            relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, cellPicture);

                            try
                            {
                                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                                using (var memstr = new MemoryStream())
                                {
                                    var buf = new byte[1024 * 16];
                                    int byteSize;
                                    while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                    {
                                        memstr.Write(buf, 0, byteSize);
                                    }
                                    pictureData = memstr.ToArray();
                                }
                                iTextSharp.text.Image partPicture = iTextSharp.text.Image.GetInstance(pictureData);
                                partPicture.ScaleAbsolute(150f, 100f);
                                partPicture.SetAbsolutePosition(415, 520);
                                quotePDF.Add(partPicture);
                            }
                            catch { }
                        }
                    }
                }
                catch
                {

                }
                //}





                BaseFont basefont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont boldFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                PdfContentByte cb = writer.DirectContent;
                //try
                //{
                //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
                //    {
                cb.BeginText();
                System.Threading.Thread.Sleep(100);

                //    }
                //}
                //catch
                //{
                //    cb.BeginText();
                //}

                cb.SetFontAndSize(boldFont, 9);

                cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCompany, 180, 815, 0);
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Quotation #:", 450, 815, 0);
                if (quoteType != 4)
                {
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Customer RFQ #:", 450, 800, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Date:", 450, 785, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Job #:", 450, 770, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Access Database:", 450, 755, 0);
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Customer RFQ #:", 450, 785, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Date:", 450, 770, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Job #:", 450, 755, 0);
                }

                cb.SetFontAndSize(basefont, 9);

                if (q.TSGCompanyAbbrev == "BTS")
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGAddress1, 180, 805, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGAddress2, 180, 795, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCity + ", " + q.TSGState + " " + q.TSGZip, 180, 785, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGPhone, 180, 775, 0);
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGAddress1, 180, 805, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCity + ", " + q.TSGState + " " + q.TSGZip, 180, 795, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGPhone, 180, 785, 0);
                }
                

                if(masQuote)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion, 460, 815, 0);
                }
                else if (quoteType == 2 || (quoteType == 3 && q.LineNumber != "" && q.LineNumber != null) || (quoteType == 4 && q.LineNumber != "" && q.LineNumber != null && q.RFQID != 0) || (quoteType == 5 && q.LineNumber != "" && q.LineNumber != null && q.RFQID != 0))
                {
                    string abbrev = q.TSGCompanyAbbrev;
                    if (quoteType == 4)
                    {
                        abbrev = stsQuoteCompany;
                    }

                    if (q.oldQuoteNumber != "" && q.oldQuoteNumber != null)
                    {
                        if (q.oldQuoteNumber.Contains("SA"))
                        {
                            if (q.ECQuote == "True")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString(), 460, 815, 0);
                            }
                        }
                        else
                        {
                            if (q.ECQuote == "True")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-" + abbrev + "-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-" + abbrev + "-" + q.QuoteVersion, 460, 815, 0);
                            }
                        }
                    }
                    else
                    {
                        if (q.ECQuote == "True")
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.RFQID + "-" + q.LineNumber + "-" + abbrev + "-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.RFQID + "-" + q.LineNumber + "-" + abbrev + "-" + q.QuoteVersion, 460, 815, 0);
                        }
                    }
                }
                else
                {
                    if (quoteType == 3)
                    {
                        if (q.QuoteNumber != "")
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-HTS-SA-" + q.QuoteVersion, 460, 815, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-HTS-SA-" + q.QuoteVersion, 460, 815, 0);
                        }
                    }
                    else if (quoteType == 4)
                    {
                        if (q.QuoteNumber != "")
                        {
                            if (q.ECQuote == "True")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion, 460, 815, 0);
                            }
                        }
                        else
                        {
                            if (q.ECQuote == "True")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion, 460, 815, 0);
                            }
                        }
                    }
                    else if (quoteType == 5)
                    {
                        if (q.QuoteNumber != "")
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-UGS-SA-" + q.QuoteVersion, 460, 815, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-UGS-SA-" + q.QuoteVersion, 460, 815, 0);
                        }
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString(), 460, 815, 0);
                    }
                }

                if (q.Date == null)
                {
                    q.Date = DateTime.Now.ToShortDateString();
                }
                if (quoteType != 4)
                {
                    if (q.CustomerRFQNumber.Length > 25)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(0, q.CustomerRFQNumber.Length / 2).Trim(), 460, 803, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(q.CustomerRFQNumber.Length / 2).Trim(), 460, 795, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber, 460, 800, 0);
                    }
                    cb.SetFontAndSize(basefont, 9);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.Date, 460, 785, 0);
                    //This will need to be filled out when we start to do all of the job information stuff
                    if (q.jobNumber != "" && q.jobNumber != null && q.jobNumber != "0")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.jobNumber, 460, 770, 0);
                    }
                }
                else
                {
                    if (q.Firm != null)
                    {
                        if ((bool)q.Firm)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Firm", 460, 800, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "Budgetary", 460, 800, 0);
                        }
                    }
                    if (q.CustomerRFQNumber.Length > 25)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(0, q.CustomerRFQNumber.Length / 2).Trim(), 460, 788, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(q.CustomerRFQNumber.Length / 2).Trim(), 460, 780, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber, 460, 785, 0);
                    }
                    cb.SetFontAndSize(basefont, 9);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.Date, 460, 770, 0);
                    //This will need to be filled out when we start to do all of the job information stuff
                    if (q.jobNumber != "" && q.jobNumber != null && q.jobNumber != "0")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.jobNumber, 460, 755, 0);
                    }
                }


                int customerLineCount = 4;
                //This is for DTS's Access Database #

                if(q.oldQuoteNumber != "" && q.oldQuoteNumber != null)
                {
                    if(q.oldQuoteNumber.Split('-')[0] != q.RFQID.ToString() || q.oldQuoteNumber.Split('-')[1] != q.LineNumber)
                    {
                        if (accessNum != "0" && accessNum != "")
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "(" + q.RFQID + "-" + q.LineNumber + ")" + accessNum, 460, 755, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, "(" + q.RFQID + "-" + q.LineNumber + ")", 460, 755, 0);
                        }
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, accessNum, 460, 755, 0);
                    }
                }
                else if(accessNum != "0")
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, accessNum, 460, 755, 0);
                }

                //Customer information
                cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerName, 40, 745, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress1, 40, 735, 0);
                int y = 725;
                if (q.CustomerAddress2 != "")
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress2, 40, y, 0);
                    customerLineCount++;
                    y -= 10;
                }
                if (q.CustomerAddress3 != "")
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress3, 40, y, 0);
                    customerLineCount++;
                    y -= 10;
                }
                cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerCity + ", " + q.CustomerState + " " + q.CustomerZip, 40, y, 0);
                //We need to hook up customer contact
                cb.ShowTextAligned(Element.ALIGN_LEFT, "Customer Contact: " + q.CustomerContactName, 40, y - 10, 0);

                cb.SetFontAndSize(boldFont, 9);
                cb.SetColorFill(BaseColor.BLACK);

                int height = 720 - (customerLineCount - 3) * 10;

                if (quoteType != 4)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Type Of Quote: ", 30, height - 15, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Number: ", 30, height - 30, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Name: ", 30, height - 45, 0);
                    if(quoteType == 5 && q.CustomerAddress3 == "")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Length: ", 30, height - 60, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Width: ", 30, height - 75, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Height: ", 30, height - 90, 0);
                    }
                    else if (quoteType == 5)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Length: ", 30, height - 60, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Width: ", 30, height - 70, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Height: ", 30, height - 80, 0);
                    }

                    cb.SetFontAndSize(basefont, 9);
                    cb.SetColorFill(BaseColor.BLUE);
                    if (q.Cavity == "N/A")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.DieType, 100, height - 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.DieType + " " + q.Cavity, 100, height - 15, 0);
                    }
                    cb.SetColorFill(BaseColor.BLACK);

                    if (q.PartNumber.Length > 60)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber.Substring(0, q.PartNumber.Length / 2), 100, height - 27, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber.Substring(q.PartNumber.Length / 2), 100, height - 33, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber, 100, height - 30, 0);
                    }

                    cb.SetFontAndSize(basefont, 9);

                    if (q.PartDescription.Length > 120)
                    {//25
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(0, q.PartDescription.Length / 3), 100, height - 42, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(q.PartDescription.Length / 3, (q.PartDescription.Length / 3)), 100, height - 48, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring((q.PartDescription.Length * 2 / 3)), 100, height - 52, 0);
                    }
                    else if (q.PartDescription.Length > 60)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(0, q.PartDescription.Length / 2), 100, height - 42, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(q.PartDescription.Length / 2), 100, height - 48, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription, 100, height - 45, 0);
                    }

                    //for ugs to show their part size
                    if(quoteType == 5 && q.CustomerAddress3 == "")
                    {
                        if (q.BlankPitchEnglish != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchEnglish.ToString() + "\"", 100, height - 60, 0);
                        }
                        if (q.BlankWidthEnglish != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthEnglish.ToString() + "\"", 100, height - 75, 0);
                        }
                        if (partHeight != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, partHeight.ToString() + "\"", 100, height - 90, 0);
                        }
                    }
                    else if (quoteType == 5)
                    {
                        if (q.BlankPitchEnglish != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchEnglish.ToString() + "\"", 100, height - 60, 0);
                        }
                        if (q.BlankWidthEnglish != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthEnglish.ToString() + "\"", 100, height - 70, 0);
                        }
                        if (partHeight != 0)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, partHeight.ToString() + "\"", 100, height - 80, 0);
                        }
                    }
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Type Of Quote: ", 30, height - 15, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Number: ", 30, height - 30, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Name: ", 30, height - 45, 0);

                    cb.SetFontAndSize(basefont, 9);
                    cb.SetColorFill(BaseColor.BLUE);
                    if (q.Cavity == "N/A")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.DieType, 100, height - 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.DieType + " " + q.Cavity, 100, height - 15, 0);
                    }
                    cb.SetColorFill(BaseColor.BLACK);

                    if (q.PartNumber.Length > 60)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber.Substring(0, q.PartNumber.Length / 2), 100, height - 27, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber.Substring(q.PartNumber.Length / 2), 100, height - 33, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber, 100, height - 30, 0);
                    }

                    cb.SetFontAndSize(basefont, 9);

                    if (q.PartDescription.Length > 120)
                    {//25
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(0, q.PartDescription.Length / 3), 100, height - 42, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(q.PartDescription.Length / 3, (q.PartDescription.Length / 3)), 100, height - 48, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring((q.PartDescription.Length * 2 / 3)), 100, height - 52, 0);
                    }
                    else if (q.PartDescription.Length > 60)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(0, q.PartDescription.Length / 2), 100, height - 42, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription.Substring(q.PartDescription.Length / 2), 100, height - 48, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription, 100, height - 45, 0);
                    }
                }

                cb.SetFontAndSize(basefont, 9);

                double totalCost = 0;

                //changing the height to the bottom of part name then removing another 20 to space it
                height -= 65;

                cb.SetFontAndSize(boldFont, 10);
                cb.EndText();
                Boolean newPage = false;
                int newPageLinesWritten = 0;

                //cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription, 100, 625, 0);
                if (quoteType <= 2)
                {
                    cb.BeginText();
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 130, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 170, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 245, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 285, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 380, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 420, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Mat'l Type", 460, height, 0);

                    cb.SetFontAndSize(boldFont, 8);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Stock Size: *", 30, height-20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Width:", 95, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Pitch:", 210, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Mat'l Thk:", 325, height - 20, 0);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Die Size: *", 30, height - 40, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "F to B:", 95, height - 40, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "L to R:", 210, height - 40, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Shut Height:", 325, height - 40, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "# of Stations:", 460, height - 40, 0);

                    cb.SetFontAndSize(basefont, 8);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthEnglish.ToString("#,##0.000"), 130, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthMetric.ToString("#,##0.00"), 170, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchEnglish.ToString("#,##0.000"), 245, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchMetric.ToString("#,##0.00"), 285, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialThicknessEnglish.ToString("#,##0.000"), 380, height - 20, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialThicknessMetric.ToString("#,##0.000"), 420, height - 20, 0);
                    if (q.MaterialType.Length > 28)
                    {
                        cb.SetFontAndSize(basefont, 6);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialType.Substring(0, q.MaterialType.Length / 2), 460, height - 15, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialType.Substring(q.MaterialType.Length / 2), 460, height - 22, 0);
                        cb.SetFontAndSize(basefont, 8);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialType, 460, height - 20, 0);
                    }
                    if(q.fbEng != 0 || q.fbEng == -1)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.fbEng.ToString("#,##0.000"), 130, height - 40, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.fbMet.ToString("#,##0.00"), 170, height - 40, 0);
                    }
                    if (q.lrEng != 0 || q.lrEng == -1)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.lrEng.ToString("#,##0.000"), 245, height - 40, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.lrMet.ToString("#,##0.00"), 285, height - 40, 0);
                    }
                    if(q.ShutHeightEng != 0 || q.ShutHeightEng == -1)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShutHeightEng.ToString("#,##0.000"), 380, height - 40, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShutHeightMet.ToString("#,##0.00"), 420, height - 40, 0);
                    }
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.NumberOfStations, 515, height - 40, 0);



                    cb.SetFontAndSize(basefont, 6);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, "* Stock size and die size are approximate", 30, height - 46, 0);
                    //start of the description and cost
                    height -= 65;
                    cb.SetFontAndSize(boldFont, 12);

                    cb.ShowTextAligned(Element.ALIGN_CENTER, "DESCRIPTION", 230, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "COST", 508, height, 0);

                    cb.SetFontAndSize(basefont, 8);

                    y = height - 20;
                    cb.EndText();
                    System.Threading.Thread.Sleep(100);


                    //Displaying all notes since they are dynamically sized
                    string text = "";
                    for (int i = 0; i < quoteNotes.Count; i++)
                    {
                        ColumnText colText = new ColumnText(cb);
                        colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 430, y + 10, 10, Element.ALIGN_LEFT);
                        colText.Go();
                        int linesWritten = colText.LinesWritten;
                        cb.SetFontAndSize(basefont, 8);

                        cb.BeginText();
                        try
                        {
                            if (System.Convert.ToDouble(costNotes[i]) != 0)
                            {
                                totalCost += System.Convert.ToDouble(costNotes[i]);
                                var culture = new System.Globalization.CultureInfo("en-US");
                                if (q.currency == "EUR")
                                {
                                    culture = new System.Globalization.CultureInfo("de-DE");
                                }
                                else if (q.currency == "GBP")
                                {
                                    culture = new System.Globalization.CultureInfo("en-GB");
                                }
                                culture.NumberFormat.CurrencyNegativePattern = 1;
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format(culture, "{0:C}", System.Convert.ToDouble(costNotes[i])), 570, y, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, "", 570, y, 0);
                            }
                        }
                        catch
                        {
                            cb.ShowTextAligned(Element.ALIGN_RIGHT, costNotes[i], 570, y, 0);
                        }
                        cb.EndText();
                        y -= 10 * linesWritten;
                        if(newPage)
                        {
                            newPageLinesWritten += 10 * linesWritten;
                        }

                        if(y <= 30)
                        {
                            cb.SaveState();
                            cb.SetColorFill(BaseColor.LIGHT_GRAY);
                            cb.SetLineWidth(3);
                            cb.SetColorStroke(BaseColor.GREEN);

                            //Customer Information Box
                            cb.MoveTo(30, 760);
                            cb.LineTo(350, 760);
                            cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                            cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                            cb.ClosePathStroke();

                            cb.SetLineWidth((float)1.5);
                            cb.SetColorStroke(BaseColor.BLACK);

                            //Description Box
                            cb.MoveTo(30, height + 15);
                            cb.LineTo(430, height + 15);
                            cb.LineTo(430, height - 5);
                            cb.LineTo(30, height - 5);
                            cb.ClosePathStroke();

                            //Cost Box
                            cb.MoveTo(445, height + 15);
                            cb.LineTo(570, height + 15);
                            cb.LineTo(570, height - 5);
                            cb.LineTo(445, height - 5);
                            cb.ClosePathStroke();
                            cb.RestoreState();

                            quotePDF.NewPage();
                            newPage = true;
                            y = 730;
                        }
                    }


                    y -= 10;
                    cb.BeginText();

                    cb.SetFontAndSize(basefont, 6);

                    int temp = 0;
                    if (q.TSGCompanyID == 9)
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    else if (q.TSGCompanyID == 3 || q.TSGCompanyID == 8)
                    {
                        temp = (generalNotes.Count + 6) * 10 - 120;
                    }
                    else
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    if(q.TSGCompanyID == 8)
                    {
                        temp -= 100;
                    }
                    if (y < 258 - temp)
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    cb.EndText();
                }
                else if (quoteType == 4)
                {
                    cb.SetFontAndSize(boldFont, 9);


                    height += 10;
                    cb.BeginText();
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "EAV: ", 30, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Machine Process Time: ", 30, height-15, 0);

                    cb.SetFontAndSize(basefont, 9);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, eav, 100, height, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, machineProcesTime, 140, height-15, 0);

                    cb.SetFontAndSize(basefont, 6);

                    if (String.IsNullOrWhiteSpace(machineProcesTime))
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "* Averaged machine through-put is based on this quoted concept part to part = XX seconds. Part to part time will be operator dependent based on the load / unload times.", 30, height - 25, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "* Averaged machine through-put is based on this quoted concept part to part = " + machineProcesTime + " seconds. Part to part time will be operator dependent based on the load / unload times.", 30, height - 25, 0);
                    }

                    cb.EndText();

                    cb.MoveTo(30, 620);
                    cb.LineTo(400, 620);
                    cb.LineTo(400, 524);
                    cb.LineTo(30, 524);
                    cb.ClosePathStroke();
                    cb.MoveTo(130, 620);
                    cb.LineTo(130, 524);
                    cb.ClosePathStroke();
                    cb.MoveTo(200, 620);
                    cb.LineTo(200, 524);
                    cb.ClosePathStroke();
                    cb.MoveTo(340, 620);
                    cb.LineTo(340, 524);
                    cb.ClosePathStroke();

                    cb.MoveTo(30, 604);
                    cb.LineTo(400, 604);
                    cb.ClosePathStroke();
                    cb.MoveTo(30, 588);
                    cb.LineTo(400, 588);
                    cb.ClosePathStroke();
                    cb.MoveTo(30, 572);
                    cb.LineTo(400, 572);
                    cb.ClosePathStroke();
                    cb.MoveTo(30, 556);
                    cb.LineTo(400, 556);
                    cb.ClosePathStroke();
                    cb.MoveTo(30, 540);
                    cb.LineTo(400, 540);
                    cb.ClosePathStroke();

                    cb.BeginText();
                    cb.SetFontAndSize(basefont, 9);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Annual Volume", 32, 608, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Days Per Year", 32, 592, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Hours Per Shift", 32, 576, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Shifts Per Day", 32, 560, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Efficiency", 32, 544, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Seconds Per Hour", 32, 528, 0);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, annualVolume.ToString("###,###,###,###"), 132, 608, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, daysPerYear.ToString(), 132, 592, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, hoursPerShift.ToString("#,###,###.####"), 132, 576, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, shiftsPerDay.ToString("#,###,###.####"), 132, 560, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, efficiency.ToString("#,###,###.####") + "%", 132, 544, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, secondsPerHour.ToString("#,###,###.####"), 132, 528, 0);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Tact Time (Available C'time)", 202, 608, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Net Parts per Hour", 202, 576, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Gross Parts per Hour", 202, 560, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Net Parts Per Day", 202, 528, 0);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, tactTime.ToString("#,###,###.####"), 342, 608, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, netPartsPerHour.ToString("#,###,###.####"), 342, 576, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, grossPartsPerHour.ToString("#,###,###.####"), 342, 560, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, netPartsPerDay.ToString("#,###,###.####"), 342, 528, 0);


                    cb.EndText();

                    height -= 160;
                    y = height-25;
                    cb.SetFontAndSize(basefont, 8);


                    Boolean newQuoteType = true;

                    if (newQuoteType && toolingCostNotes.Count > 0)
                    {
                        double toolingSubtotal = 0;
                        double capitalSubtotal = 0;
                        for (int i = 0; i < quoteNotes.Count; i++)
                        {
                            ColumnText colText = new ColumnText(cb);
                            colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 360, y + 10, 10, Element.ALIGN_LEFT);
                            colText.Go();
                            int linesWritten = colText.LinesWritten;

                            cb.BeginText();
                            try
                            {

                                double tooling = System.Convert.ToDouble(toolingCostNotes[i] == "" ? "0" : toolingCostNotes[i]);
                                double capital = System.Convert.ToDouble(capitalCostNotes[i] == "" ? "0" : capitalCostNotes[i]);
                                toolingSubtotal += tooling;
                                capitalSubtotal += capital;

                                totalCost += tooling + capital;
                                string subtotal = "";
                                string toolingCost = "";
                                string capitalCost = "";

                                var cul = new System.Globalization.CultureInfo("en-US");

                                if (q.currency == "EUR")
                                {
                                    cul = new System.Globalization.CultureInfo("de-DE");
                                }
                                else if (q.currency == "GBP")
                                {
                                    cul = new System.Globalization.CultureInfo("en-GB");
                                }
                                cul.NumberFormat.CurrencyNegativePattern = 1;
                                subtotal = String.Format(cul, "{0:C}", (tooling + capital));
                                toolingCost = String.Format(cul, "{0:C}", tooling);
                                capitalCost = String.Format(cul, "{0:C}", capital);

                                toolingCost = tooling == 0 ? "" : toolingCost;
                                capitalCost = capital == 0 ? "" : capitalCost;
                                subtotal = (tooling == 0 && capital == 0) ? "" : subtotal;

                                cb.ShowTextAligned(Element.ALIGN_RIGHT, toolingCost, 430, y, 0);
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, capitalCost, 500, y, 0);
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, subtotal, 570, y, 0);

                                cb.EndText();
                                y -= 10 * linesWritten;
                            }
                            catch { }
                        }

                        ColumnText cText = new ColumnText(cb);
                        cText.SetSimpleColumn(new Phrase(new Chunk("Subtotal", FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 360, y, 10, Element.ALIGN_LEFT);
                        cText.Go();
                        cb.BeginText();
                        string toolingSubtotalString = "";
                        string capitalSubtotalString = "";
                        var culture = new System.Globalization.CultureInfo("en-US");
                        if (q.currency == "EUR")
                        {
                            culture = new System.Globalization.CultureInfo("de-DE");
                        }
                        else if (q.currency == "GBP")
                        {
                            culture = new System.Globalization.CultureInfo("en-GB");
                        }

                        culture.NumberFormat.CurrencyNegativePattern = 1;
                        toolingSubtotalString = String.Format(culture, "{0:C}", toolingSubtotal);
                        capitalSubtotalString = String.Format(culture, "{0:C}", capitalSubtotal);

                        cb.ShowTextAligned(Element.ALIGN_RIGHT, toolingSubtotalString, 430, y - 10, 0);
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, capitalSubtotalString, 500, y - 10, 0);
                        cb.EndText();

                        cb.SetLineWidth((float)1.5);
                        cb.SetColorStroke(BaseColor.BLACK);

                        //Description Box
                        cb.MoveTo(30, y + 3);
                        cb.LineTo(570, y + 3);
                        cb.ClosePathStroke();

                        y -= 20;
                    }
                    else
                    {
                        for (int i = 0; i < quoteNotes.Count; i++)
                        {
                            ColumnText colText = new ColumnText(cb);
                            colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 430, y + 10, 10, Element.ALIGN_LEFT);
                            colText.Go();
                            int linesWritten = colText.LinesWritten;

                            cb.BeginText();
                            try
                            {
                                if (System.Convert.ToDouble(costNotes[i]) != 0)
                                {
                                    totalCost += System.Convert.ToDouble(costNotes[i]);
                                    var culture = new System.Globalization.CultureInfo("en-US");
                                    if (q.currency == "EUR")
                                    {
                                        culture = new System.Globalization.CultureInfo("de-DE");
                                    }
                                    else if (q.currency == "GBP")
                                    {
                                        culture = new System.Globalization.CultureInfo("en-GB");
                                    }

                                    culture.NumberFormat.CurrencyNegativePattern = 1;
                                    cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format(culture, "{0:C}", System.Convert.ToDouble(costNotes[i])), 570, y, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "", 570, y, 0);
                                }
                            }
                            catch
                            {
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, costNotes[i], 570, y, 0);
                            }
                            cb.EndText();
                            y -= 10 * linesWritten;
                        }
                    }


                    y -= 30;
                    cb.BeginText();

                    cb.SetFontAndSize(basefont, 6);

                    int temp = 0;
                    if (q.TSGCompanyID == 9)
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    else if (q.TSGCompanyID == 3 || q.TSGCompanyID == 8)
                    {
                        temp = (generalNotes.Count + 6) * 10 - 120;
                    }
                    else
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    if (q.TSGCompanyID == 8)
                    {
                        temp -= 100;
                    }
                    if (y < 358 - temp)
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }

                    cb.SetFontAndSize(boldFont, 11);

                    if (newQuoteType && toolingCostNotes.Count > 0)
                    {

                        cb.ShowTextAligned(Element.ALIGN_CENTER, "DESCRIPTION", 195, height, 0);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "TOOLING", 397, height, 0);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "CAPITAL", 467, height, 0);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "SUBTOTAL", 537, height, 0);

                        // 360, 430, 500, 570

                        cb.EndText();

                        cb.SetLineWidth((float)1.5);
                        cb.SetColorStroke(BaseColor.BLACK);

                        //Description Box
                        cb.MoveTo(30, height + 15);
                        cb.LineTo(360, height + 15);
                        cb.LineTo(360, height - 5);
                        cb.LineTo(30, height - 5);
                        cb.ClosePathStroke();

                        //Tooling Box
                        cb.MoveTo(365, height + 15);
                        cb.LineTo(430, height + 15);
                        cb.LineTo(430, height - 5);
                        cb.LineTo(365, height - 5);
                        cb.ClosePathStroke();

                        //Capital Box
                        cb.MoveTo(435, height + 15);
                        cb.LineTo(500, height + 15);
                        cb.LineTo(500, height - 5);
                        cb.LineTo(435, height - 5);
                        cb.ClosePathStroke();

                        //Capital Box
                        cb.MoveTo(505, height + 15);
                        cb.LineTo(570, height + 15);
                        cb.LineTo(570, height - 5);
                        cb.LineTo(505, height - 5);
                        cb.ClosePathStroke();
                    }
                    else
                    {

                        cb.ShowTextAligned(Element.ALIGN_CENTER, "DESCRIPTION", 230, height, 0);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "COST", 508, height, 0);

                        cb.EndText();

                        cb.SetLineWidth((float)1.5);
                        cb.SetColorStroke(BaseColor.BLACK);

                        //Description Box
                        cb.MoveTo(30, height + 15);
                        cb.LineTo(430, height + 15);
                        cb.LineTo(430, height - 5);
                        cb.LineTo(30, height - 5);
                        cb.ClosePathStroke();

                        //Cost Box
                        cb.MoveTo(445, height + 15);
                        cb.LineTo(570, height + 15);
                        cb.LineTo(570, height - 5);
                        cb.LineTo(445, height - 5);
                        cb.ClosePathStroke();
                    }

                    cb.SetColorFill(BaseColor.LIGHT_GRAY);
                    cb.SetLineWidth(3);
                    cb.SetColorStroke(BaseColor.GREEN);

                    //Customer Information Box
                    cb.MoveTo(30, 760);
                    cb.LineTo(350, 760);
                    cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                    cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                    cb.ClosePathStroke();

                }
                else if (quoteType == 3)
                {
                    cb.BeginText();
                    y = height-10;
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "DESCRIPTION", 200, y, 0);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "QTY", 400, y, 0);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Unit Price", 460, y, 0);
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Sub Total", 540, y, 0);
                    y -= 20;

                    cb.EndText();
                    for (int i = 0; i < quoteNotes.Count; i++)
                    {
                        Boolean fullLine = true;
                        for (int j = i; j < i + 1; j++)
                        {
                            if(qtyNotes[j] != "0" && costNotes[j] != "0" && qtyNotes[j] != "0.0")
                            {
                                fullLine = false;
                            }
                        }
                        ColumnText colText;
                        if (fullLine)
                        {
                            colText = new ColumnText(cb);
                            colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 570, y + 10, 10, Element.ALIGN_LEFT);
                            colText.Go();
                            int linesWritten = colText.LinesWritten;

                            y -= 10 * linesWritten;
                            if (newPage)
                            {
                                newPageLinesWritten += 10 * linesWritten;
                            }
                        }
                        else
                        {
                            colText = new ColumnText(cb);
                            colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 30, 30, 380, y + 10, 10, Element.ALIGN_LEFT);
                            colText.Go();
                            int linesWritten = colText.LinesWritten;
                            cb.SetFontAndSize(basefont, 8);

                            cb.BeginText();
                            if (qtyNotes[i] != "0" && qtyNotes[i] != "0.0")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, System.Convert.ToDouble(qtyNotes[i].Trim()).ToString("#.#"), 395, y, 0);
                            }
                            if (costNotes[i] != "0.0000")
                            {
                                var culture = new System.Globalization.CultureInfo("en-US");
                                if (q.currency == "EUR")
                                {
                                    culture = new System.Globalization.CultureInfo("de-DE");
                                }
                                else if (q.currency == "GBP")
                                {
                                    culture = new System.Globalization.CultureInfo("en-GB");
                                }

                                culture.NumberFormat.CurrencyNegativePattern = 1;
                                cb.ShowTextAligned(Element.ALIGN_LEFT, String.Format(culture, "{0:C}", System.Convert.ToDouble(costNotes[i])).Trim(), 440, y, 0);
                            }
                            try
                            {
                                if (System.Convert.ToDouble(costNotes[i]) != 0)
                                {
                                    totalCost += System.Convert.ToDouble(costNotes[i]) * System.Convert.ToDouble(qtyNotes[i]);
                                    var culture = new System.Globalization.CultureInfo("en-US");
                                    if (q.currency == "EUR")
                                    {
                                        culture = new System.Globalization.CultureInfo("de-DE");
                                    }
                                    else if (q.currency == "GBP")
                                    {
                                        culture = new System.Globalization.CultureInfo("en-GB");
                                    }

                                    culture.NumberFormat.CurrencyNegativePattern = 1;
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, String.Format(culture, "{0:C}", System.Convert.ToDouble(costNotes[i]) * System.Convert.ToDouble(qtyNotes[i])).Trim(), 525, y, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, "", 525, y, 0);
                                }
                            }
                            catch
                            {
                                //cb.ShowTextAligned(Element.ALIGN_RIGHT, costNotes[i], 590, y, 0);
                            }
                            cb.EndText();
                            y -= 10 * linesWritten;
                            if (newPage)
                            {
                                newPageLinesWritten += 10 * linesWritten;
                            }

                        }



                        if (y <= 30)
                        {
                            cb.SaveState();
                            cb.SetColorFill(BaseColor.LIGHT_GRAY);
                            cb.SetLineWidth(3);
                            cb.SetColorStroke(BaseColor.GREEN);

                            //Customer Information Box
                            cb.MoveTo(30, 760);
                            cb.LineTo(350, 760);
                            cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                            cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                            cb.ClosePathStroke();

                            cb.SetLineWidth((float)1.5);
                            cb.SetColorStroke(BaseColor.BLACK);

                            cb.RestoreState();

                            quotePDF.NewPage();
                            newPage = true;
                            y = 730;
                        }
                    }
                    y -= 10;
                    cb.BeginText();

                    cb.SetFontAndSize(basefont, 6);

                    int temp = 0;
                    if (q.TSGCompanyID == 9)
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    else if (q.TSGCompanyID == 3 || q.TSGCompanyID == 8)
                    {
                        temp = (generalNotes.Count + 6) * 10 - 120;
                    }
                    else
                    {
                        temp = (generalNotes.Count + 2) * 10 - 120;
                    }
                    if(q.TSGCompanyID == 8)
                    {
                        temp -= 100;
                    }
                    if (y < 258 - temp)
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    cb.EndText();

                    if (!newPage)
                    {
                        cb.SetColorFill(BaseColor.LIGHT_GRAY);
                        cb.SetLineWidth(3);
                        cb.SetColorStroke(BaseColor.GREEN);

                        //Customer Information Box
                        cb.MoveTo(30, 760);
                        cb.LineTo(350, 760);
                        cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                        cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                        cb.ClosePathStroke();

                        cb.SetLineWidth((float)1.5);
                        cb.SetColorStroke(BaseColor.BLACK);
                    }

                }
                else if(quoteType == 5)
                {
                    y = 570;

                    double costTotal = 0;

                    for (int i = 0; i < ugsNotes.Count; i++)
                    {
                        ColumnText colText = new ColumnText(cb);
                        colText.SetSimpleColumn(new Phrase(new Chunk(ugsNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))),
                            30, 30, 430, y + 10, 10, Element.ALIGN_LEFT);
                        colText.Go();
                        int linesWritten = colText.LinesWritten;
                        cb.SetFontAndSize(basefont, 8);

                        cb.BeginText();
                        try
                        {
                            if (System.Convert.ToDouble(costNotes[i]) != 0)
                            {
                                costTotal += System.Convert.ToDouble(costNotes[i]);
                                //q.TotalCost += System.Convert.ToDouble(costNotes[i]);
                                totalCost += System.Convert.ToDouble(costNotes[i]);
                                var culture = new System.Globalization.CultureInfo("en-US");
                                culture.NumberFormat.CurrencyNegativePattern = 1;
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format(culture, "{0:C}", System.Convert.ToDouble(costNotes[i])), 570, y, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_RIGHT, "", 570, y, 0);
                            }
                        }
                        catch
                        {
                            cb.ShowTextAligned(Element.ALIGN_RIGHT, costNotes[i], 570, y, 0);
                        }
                        cb.EndText();
                        y -= 10 * linesWritten;
                        if (newPage)
                        {
                            newPageLinesWritten += 10 * linesWritten;
                        }
                    }
                    if(costTotal != 0)
                    {
                        q.TotalCost = costTotal;
                    }

                    //Large block of text from the quote
                    string text = "";
                    for (int i = 0; i < quoteNotes.Count; i++)
                    {
                        ColumnText colText = new ColumnText(cb);
                        colText.SetSimpleColumn(new Phrase(new Chunk(quoteNotes[i], FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))),
                            30, 30, 570, y + 10, 10, Element.ALIGN_LEFT);
                        colText.Go();
                        int linesWritten = colText.LinesWritten;

                        y -= 10 * linesWritten;
                    }
                    totalCost = q.TotalCost;


                    y -= 15;
                    cb.BeginText();

                    cb.SetFontAndSize(basefont, 6);

                    int temp = 0;
                    temp = generalNotes.Count * 10 - 120;

                    if (q.TSGCompanyID == 8)
                    {
                        temp -= 100;
                    }
                    if (y < 358 - temp)
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + page.ToString(), 570, 15, 0);
                    }
                    cb.SetFontAndSize(boldFont, 12);

                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Notes", 300, 590, 0);
                    //cb.ShowTextAligned(Element.ALIGN_CENTER, "COST", 508, 565, 0);

                    cb.EndText();
                    cb.SetColorFill(BaseColor.LIGHT_GRAY);
                    cb.SetLineWidth(3);
                    cb.SetColorStroke(BaseColor.GREEN);

                    //Customer Information Box
                    cb.MoveTo(30, 760);
                    cb.LineTo(350, 760);
                    cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                    cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                    cb.ClosePathStroke();

                    cb.SetLineWidth((float)1.5);
                    cb.SetColorStroke(BaseColor.BLACK);

                    //Description Box
                    cb.MoveTo(30, 605);
                    cb.LineTo(570, 605);
                    cb.LineTo(570, 585);
                    cb.LineTo(30, 585);
                    cb.ClosePathStroke();

                    ////Cost Box
                    //cb.MoveTo(445, 580);
                    //cb.LineTo(570, 580);
                    //cb.LineTo(570, 560);
                    //cb.LineTo(445, 560);
                    //cb.ClosePathStroke();
                }




                //try
                //{
                //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
                //    {
                System.Threading.Thread.Sleep(100);

                //    }
                //}
                //catch
                //{
                //    cb.EndText();
                //}

                

                if(quoteType <= 2 && !newPage)
                {
                    cb.SaveState();
                    cb.SetColorFill(BaseColor.LIGHT_GRAY);
                    cb.SetLineWidth(3);
                    cb.SetColorStroke(BaseColor.GREEN);

                    //Customer Information Box
                    cb.MoveTo(30, 760);
                    cb.LineTo(350, 760);
                    cb.LineTo(350, 720 - (customerLineCount - 3) * 10);
                    cb.LineTo(30, 720 - (customerLineCount - 3) * 10);
                    cb.ClosePathStroke();

                    cb.SetLineWidth((float)1.5);
                    cb.SetColorStroke(BaseColor.BLACK);
                    //Description Box
                    cb.MoveTo(30, height + 15);
                    cb.LineTo(430, height + 15);
                    cb.LineTo(430, height - 5);
                    cb.LineTo(30, height - 5);
                    cb.ClosePathStroke();

                    //Cost Box
                    cb.MoveTo(445, height + 15);
                    cb.LineTo(570, height + 15);
                    cb.LineTo(570, height - 5);
                    cb.LineTo(445, height - 5);
                    cb.ClosePathStroke();
                    cb.RestoreState();

                }

                //cb.RestoreState();

                int tempY = 0;
                height -= 10;

                if ((y < 500 && (q.TSGCompanyID == 8 || q.TSGCompanyID == 3)) || (y <= 250 && (q.TSGCompanyID != 8 || q.TSGCompanyID != 3)))
                {
                    numOfPages = 2;
                    quotePDF.NewPage();
                    y = 740;
                }
                
                if(newPage)
                {
                    numOfPages = 2;
                    y = 720 - newPageLinesWritten;
                }
                tempY = y;

                //try
                //{
                //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
                //    {
                cb.BeginText();
                System.Threading.Thread.Sleep(100);

                //    }
                //}
                //catch
                //{
                //    cb.BeginText();
                //}

                cb.SetFontAndSize(basefont, 6);
                if (y == 740 || newPage)
                {
                    cb.SetFontAndSize(basefont, 6);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page " + (page + 1).ToString(), 570, 815, 0);

                    cb.SetFontAndSize(boldFont, 9);

                    if(q.TSGCompany != null)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCompany, 180, 815, 0);
                    }
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Quotation #:", 450, 815, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Customer RFQ #:", 450, 800, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Date:", 450, 785, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Job #:", 450, 770, 0);
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, "Access Database:", 450, 755, 0);

                    cb.SetFontAndSize(basefont, 9);

                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGAddress1, 180, 805, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCity + ", " + q.TSGState + "" + q.TSGZip, 180, 795, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGPhone, 180, 785, 0);

                    if (masQuote)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion, 460, 815, 0);
                    }
                    else if (quoteType == 2 || (quoteType == 3 && q.LineNumber != "" && q.LineNumber != null) || (quoteType == 4 && q.LineNumber != "" && q.LineNumber != null && q.RFQID != 0) || (quoteType == 5 && q.LineNumber != "" && q.LineNumber != null && q.RFQID != 0))
                    {
                        if (q.oldQuoteNumber != "" && q.oldQuoteNumber != null)
                        {
                            if (q.oldQuoteNumber.Contains("SA"))
                            {
                                if (q.ECQuote == "True")
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString(), 460, 815, 0);
                                }
                            }
                            else
                            {
                                if (q.ECQuote == "True")
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.oldQuoteNumber.ToString() + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion, 460, 815, 0);
                                }
                            }
                        }
                        else
                        {
                            if (q.ECQuote == "True")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.RFQID + "-" + q.LineNumber + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.RFQID + "-" + q.LineNumber + "-" + q.TSGCompanyAbbrev + "-" + q.QuoteVersion, 460, 815, 0);
                            }
                        }
                    }
                    else
                    {
                        if (quoteType == 3)
                        {
                            if (q.QuoteNumber != "")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-HTS-SA-" + q.QuoteVersion, 460, 815, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-HTS-SA-" + q.QuoteVersion, 460, 815, 0);
                            }
                        }
                        else if (quoteType == 4)
                        {
                            if (q.QuoteNumber != "")
                            {
                                if (q.ECQuote == "True")
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteNumber.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion, 460, 815, 0);
                                }
                            }
                            else
                            {
                                if (q.ECQuote == "True")
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion + "-EC-" + q.ECQuoteNumber, 460, 815, 0);
                                }
                                else
                                {
                                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-" + stsQuoteCompany + "-SA-" + q.QuoteVersion, 460, 815, 0);
                                }
                            }
                        }
                        else if (quoteType == 5)
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString() + "-UGS-SA-" + q.QuoteVersion, 460, 815, 0);
                        }
                        else
                        {
                            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString(), 460, 815, 0);
                        }
                    }




                    if (q.CustomerRFQNumber.Length > 25)
                    {
                        cb.SetFontAndSize(basefont, 7);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(0, q.CustomerRFQNumber.Length / 2).Trim(), 460, 803, 0);
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber.Substring(q.CustomerRFQNumber.Length / 2).Trim(), 460, 795, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber, 460, 800, 0);
                    }
                    cb.SetFontAndSize(basefont, 9);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.Date, 460, 785, 0);
                    //This will need to be filled out when we start to do all of the job information stuff
                    if (q.jobNumber != "" && q.jobNumber != null && q.jobNumber != "0")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.jobNumber, 460, 770, 0);

                    }

                    customerLineCount = 4;
                    //This is for DTS's Access Database #

                    if (q.oldQuoteNumber != "" && q.oldQuoteNumber != null)
                    {
                        try
                        {

                        }
                        catch
                        {

                        }
                        if (q.oldQuoteNumber.Split('-')[0] != q.RFQID.ToString() || q.oldQuoteNumber.Split('-')[1] != q.LineNumber)
                        {
                            if (accessNum != "0" && accessNum != "")
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, "(" + q.RFQID + "-" + q.LineNumber + ")" + accessNum, 460, 755, 0);
                            }
                            else
                            {
                                cb.ShowTextAligned(Element.ALIGN_LEFT, "(" + q.RFQID + "-" + q.LineNumber + ")", 460, 755, 0);
                            }
                        }

                    }
                    else if (accessNum != "0")
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, accessNum, 460, 755, 0);
                    }

                    cb.EndText();

                    using (var clientContext = new ClientContext(siteUrl))
                    {
                        try
                        {
                            sharepointLibrary = "shared documents/logos";
                            siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                            clientContext.Credentials = master.getSharePointCredentials();
                            var relativeUrl = "";
                            var url = new Uri(siteUrl);
                            if (q.logo != "True")
                            {
                                relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, q.TSGCompanyAbbrev + ".png");
                            }
                            else
                            {
                                relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "TSG" + ".png");
                            }
                            using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                            // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                            using (var memstr = new MemoryStream())
                            {
                                var buf = new byte[1024 * 16];
                                int byteSize;
                                while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    memstr.Write(buf, 0, byteSize);
                                }
                                logoData = memstr.ToArray();
                            }

                            iTextSharp.text.Image logoPicture = iTextSharp.text.Image.GetInstance(logoData);
                            // make it fit in our tight quote format.
                            logoPicture.ScaleAbsolute(125, 60);
                            logoPicture.SetAbsolutePosition(30, 770);
                            quotePDF.Add(logoPicture);
                        }
                        catch
                        {

                        }
                    }


                    //if (q.TSGCompanyAbbrev == "UGS")
                    //{
                    //    using (var clientContext = new ClientContext(siteUrl))
                    //    {
                    //        clientContext.Credentials = master.getSharePointCredentials();
                    //        var relativeUrl = "";
                    //        var url = new Uri(siteUrl);

                    //        relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, "L-A-B_Div_A-S-B_Acred_ISO_Sm.jpg");

                    //        // open the file as binary
                    //        try
                    //        {
                    //            using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                    //            // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                    //            using (var memstr = new MemoryStream())
                    //            {
                    //                var buf = new byte[1024 * 16];
                    //                int byteSize;
                    //                while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                    //                {
                    //                    memstr.Write(buf, 0, byteSize);
                    //                }
                    //                UGSPicture = memstr.ToArray();
                    //            }
                    //            // bulid the itext picture with the byte array
                    //            iTextSharp.text.Image logoPicture = iTextSharp.text.Image.GetInstance(UGSPicture);
                    //            // make it fit in our tight quote format.
                    //            logoPicture.ScaleAbsolute(150, 35);
                    //            logoPicture.SetAbsolutePosition(400, 610);
                    //            quotePDF.Add(logoPicture);
                    //        }
                    //        catch
                    //        {

                    //        }
                    //    }
                    //}
                }
                else
                {
                    cb.EndText();
                    System.Threading.Thread.Sleep(100);

                }
                cb.BeginText();
                cb.SetFontAndSize(boldFont, 10);
                cb.SetColorFill(BaseColor.RED);
                y += 10;
                try
                {
                    if (q.TSGCompanyAbbrev == "GTS")
                    {
                        int leadTime = System.Convert.ToInt32(q.LeadTime);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "Lead Time to Die Delivery " + q.LeadTime + " Weeks", 230, y - 13, 0);
                    }
                    else if (q.TSGCompanyAbbrev == "STS")
                    {
                        int leadTime = System.Convert.ToInt32(q.LeadTime);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "Lead Time to Buyoff at STS " + q.LeadTime + " Weeks", 230, y - 13, 0);
                    }
                    else
                    {
                        int leadTime = System.Convert.ToInt32(q.LeadTime);
                        cb.ShowTextAligned(Element.ALIGN_CENTER, "Lead Time to Die Shop Buyoff " + q.LeadTime + " Weeks", 230, y - 13, 0);
                    }
                    
                }
                catch
                {
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Lead Time of " + q.LeadTime, 230, y - 13, 0);
                }
                cb.SetColorFill(BaseColor.BLUE);
                if(q.currency == "" || q.currency == null)
                {
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Total (USD)", 460, y - 13, 0);
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_CENTER, "Total (" + q.currency + ")", 460, y - 13, 0);
                }
                var cultureFormat = new System.Globalization.CultureInfo("en-US");
                if (q.currency == "EUR")
                {
                    cultureFormat = new System.Globalization.CultureInfo("de-DE");
                }
                else if (q.currency == "GBP")
                {
                    cultureFormat = new System.Globalization.CultureInfo("en-GB");
                }

                cultureFormat.NumberFormat.CurrencyNegativePattern = 1;
                cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format(cultureFormat, "{0:C}", totalCost), 565, y - 13, 0);

                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(boldFont, 8);

                cb.ShowTextAligned(Element.ALIGN_LEFT, "Shipping: " , 30, y - 25, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "Payment Terms:", 30, y - 35, 0);
                if(quoteType != 4 && quoteType != 5)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Country of Orign:", 30, y - 45, 0);
                }

                cb.SetFontAndSize(basefont, 8);

                if (q.ShippingTerms == null)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "", 100, y - 25, 0);
                }
                else
                {
                    if(q.shippingLocation != null)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShippingTerms + " " + q.shippingLocation, 100, y - 25, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShippingTerms, 100, y - 25, 0);
                    }
                }
                if (q.PaymentTerms == null)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "", 100, y - 35, 0);
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.PaymentTerms, 100, y - 35, 0);
                }
                if(quoteType != 4 && quoteType != 5)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.ToolCountry, 100, y - 45, 0);

                }

                cb.SetFontAndSize(basefont, 6);
                cb.EndText();
                string text2 = "";
                text2 = "";
                int genNoteCount = 0;

                if (q.TSGCompanyID == 15)
                {
                    genNoteCount = 2;
                    //text2 += "Acceptance/In Tolerance determination made based on the “Shared Risk Method” defined in ILAC-G8:1996" + "\n";
                    //text2 += "This means that the Dimensional Inspection Report does not take into account the effect of uncertainty on the assessment of compliance." +"\n";
                    text2 += "Measurement Uncertainty is not applied when making In Tolerance / Out of Tolerance determinations." + "\n";
                }

                for (int i = 0; i < generalNotes.Count; i++)
                {
                    text2 += generalNotes[i] + "\n";

                    genNoteCount++;
                    //y -= 10;
                }
                ColumnText ct = new ColumnText(cb);
                ct.SetSimpleColumn(new Phrase(new Chunk(text2, FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL))), 35, 35, 565, y - 55, 10, Element.ALIGN_LEFT);

                cb.SetFontAndSize(basefont, 8);

                if (q.TSGCompanyID == 9)
                {
                    y = y - (genNoteCount + 2) * 10 - 53;
                }
                else if (q.TSGCompanyID == 3 || q.TSGCompanyID == 8)
                {
                    y = y - (genNoteCount + 8) * 10 - 53;
                }
                else
                {
                    y = y - (genNoteCount + 2) * 10 - 53;
                }
                ct.Go();

                cb.BeginText();


                if (q.Firm != null)
                {
                    if ((bool)q.Firm)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Company Representative: Dave Ruthven", 30, y - 62, 0);
                    }
                    else
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, "Company Representative: Jamey Moore", 30, y - 62, 0);
                    }
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Estimator: " + q.estimatorName + " " + q.estimatorEmail, 30, y - 72, 0);
                }
                else
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, "Company Representative: " + q.estimatorName, 30, y - 62, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, q.estimatorEmail, 30, y - 72, 0);
                }

                cb.ShowTextAligned(Element.ALIGN_LEFT, "Salesman: " + q.salesman + " " + salesmanEmail, 30, y - 82, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, "Authorization to Proceed Signature", 310, y - 62, 0);
                cb.SetColorFill(BaseColor.BLUE);
                cb.ShowTextAligned(Element.ALIGN_CENTER, "** THIS QUOTATION IS VALID FOR 30 DAYS UNLESS OTHERWISE SPECIFIED **", 300, y - 92, 0);
                cb.SetColorFill(BaseColor.BLACK);
                cb.SetFontAndSize(basefont, 5);

                cb.ShowTextAligned(Element.ALIGN_CENTER, "Thank you for your consideration. We appreciate your business. Work shall commence at the receipt of your purchase order or signed authorization to proceed by an authorized representative for the terms listed on the quote", 300, y - 10, 0);

                cb.SetLineWidth(1);
                cb.EndText();




                //try
                //{
                //    if (Request.Browser.Type.ToUpper().Contains("INTERNETEXPLORER"))
                //    {
                System.Threading.Thread.Sleep(100);

                //    }
                //}
                //catch
                //{
                //    cb.EndText();
                //}

                cb.SaveState();
                tempY += 10;
                //Total Box
                cb.MoveTo(430, tempY);
                cb.LineTo(490, tempY);
                cb.LineTo(490, tempY - 18);
                cb.LineTo(430, tempY - 18);
                cb.ClosePathStroke();

                //Price Box
                cb.MoveTo(490, tempY);
                cb.LineTo(570, tempY);
                cb.LineTo(570, tempY - 18);
                cb.LineTo(490, tempY - 18);
                cb.ClosePathStroke();


                //General Notes Box
                cb.MoveTo(30, tempY - 53);
                cb.LineTo(570, tempY - 53);
                cb.LineTo(570, y);
                cb.LineTo(30, y);


                cb.ClosePathStroke();
                cb.SetColorStroke(BaseColor.YELLOW);

                //THANK YOU BOX
                cb.MoveTo(44, y - 3);
                cb.LineTo(556, y - 3);
                cb.LineTo(556, y - 13);
                cb.LineTo(44, y - 13);
                cb.ClosePathStroke();
                cb.SetColorStroke(BaseColor.BLACK);

                iTextSharp.text.Image signature;

                byte[] signatureData;
                using (var clientContext = new ClientContext(siteUrl))
                {
                    clientContext.Credentials = master.getSharePointCredentials();

                    var url = new Uri(siteUrl);
                    var relativeUrl = String.Format("{0}/{1}/{2}", url.AbsolutePath, sharepointLibrary, pictureName);
                    // open the file as binary
                    try
                    {
                        url = new Uri("https://toolingsystemsgroup.sharepoint.com/TSG/Internal%20IT/Software%20Development%20Site");
                        sharepointLibrary = "RFQAndQuotingApplicationProject/Shared%20Documents";
                        sharepointSubFolder = "Images";
                        if (quoteType != 4)
                        {
                            pictureName = q.estimatorName + ".bmp";
                        }
                        else
                        {
                            if (q.Firm != null)
                            {
                                if ((bool)q.Firm)
                                {
                                    pictureName = "Dave Ruthven.bmp";
                                }
                                else
                                {
                                    pictureName = "Jamey Moore.bmp";
                                }
                            }
                        }
                        relativeUrl = String.Format("{0}/{1}/{2}/{3}", url.AbsolutePath, sharepointLibrary, sharepointSubFolder, pictureName);
                        using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                        using (var memstr = new MemoryStream())
                        {
                            var buf = new byte[1024 * 16];
                            int byteSize;
                            while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                            {
                                memstr.Write(buf, 0, byteSize);
                            }
                            signatureData = memstr.ToArray();

                            signature = iTextSharp.text.Image.GetInstance(signatureData);
                            if(q.estimatorName.Length <= 10)
                            {
                                signature.ScaleAbsolute(120f, 30f);
                                signature.SetAbsolutePosition(103, y - 52);
                            }
                            else if(q.estimatorName.Length <= 20)
                            {
                                signature.ScaleAbsolute(180f, 37f);
                                signature.SetAbsolutePosition(72, y - 52);
                            }
                            else
                            {
                                signature.ScaleAbsolute(240f, 40f);
                                signature.SetAbsolutePosition(40, y - 52);
                            }

                            quotePDF.Add(signature);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                //Signature lines
                cb.MoveTo(30, y - 50);
                cb.LineTo(290, y - 50);
                cb.Stroke();

                cb.MoveTo(310, y - 50);
                cb.LineTo(570, y - 50);
                cb.Stroke();
                cb.RestoreState();
                // BD - Here is where to append the STS Detailed Quote PDF
                if (quoteType == 4)
                {
                    if (STSDetailedQuotePdfFileName != "")
                    {
                        // BD - appendSTSDetailedQuote(numOfPages, quoteNumber);
                        String sharepointLibrary1 = "STS Detailed Quotes";
                        String siteUrl1 = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
                        //MemoryStream ms1 = new MemoryStream();
                        PdfPTable table = new PdfPTable(1);

                        table.WidthPercentage = 100;
 //                       table.setBorder(BorderStyle.No_BORDER);

                        

                        using (var clientContext1 = new ClientContext(siteUrl1))
                        {
                            Site master1 = new Site();
                            clientContext1.Credentials = master1.getSharePointCredentials();
                            var url1 = new Uri(siteUrl1);
                            var relativeUrl1 = String.Format("{0}/{1}/{2}", url1.AbsolutePath, sharepointLibrary1, STSDetailedQuotePdfFileName);
                            var result = new MemoryStream();
                            // open the file as binary
                            try
                            {

                                Document document = new Document();

                                document.Open();
                                var fileInfo1 = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext1, relativeUrl1);

                                // Convert the BinaryStream of the file to a ITexSharp PdfReader
                                PdfReader reader = new PdfReader(fileInfo1.Stream);
                                PdfImportedPage newPage1;


                                int n = reader.NumberOfPages;
                                // Loop through each page in the current PDF file
                                for (int page1 = 0; page1 < n;)
                                {
                                    PdfPCell cell1 = new PdfPCell();
                                    //Import the page to the PDF document in the memory stream.
                                    newPage1 = writer.GetImportedPage(reader, ++page1);
                                    cell1.AddElement(iTextSharp.text.Image.GetInstance(newPage1));
                                    cell1.Border = 0;
                                    cell1.Rotation = 0;
                                    cell1.PaddingBottom = 5;
                                    cell1.PaddingTop = 5;
                                    cell1.PaddingLeft = 5;
                                    cell1.PaddingRight = 5;

                                    //table.AddCell(iTextSharp.text.Image.GetInstance(newPage1));
                                    //page++;
                                    table.AddCell(cell1);
                                    numOfPages++;
                                }
//                                cell1.Rotate();
//                                table.DefaultCell.Rotation = 180;
                            }
                            catch (Exception err)
                            {
                                string x = "";

                                x = err.ToString();

                            }
                        }
                        // Get the cost information from the STS Detailed quote and add it to the TIMS quote sheet

                        quotePDF.NewPage();
                        quotePDF.Add(table);


                    }

                }
// BD - 1-13-2021                quotePDF.Close();
            }
            catch (Exception err)
            {

            }

            return numOfPages;
        }
    }

    public class FullQuote
        {
            public string QuoteID { get; set; }
            public string QuoteNumber { get; set; }
            public string QuoteVersion { get; set; }
            public int RFQID { get; set; }
            public string Date { get; set; }
            public string QuoteDescription { get; set; }
            public double TotalCost { get; set; }
            public string ShippingTerms { get; set; }
            public string PaymentTerms { get; set; }
            public string LeadTime { get; set; }
            public int EstimatorID { get; set; }
            public int CustomerID { get; set; }
            public string CustomerRFQNumber { get; set; }
            public string DieType { get; set; }
            public string Cavity { get; set; }
            public double fbEng { get; set; }
            public double fbMet { get; set; }
            public double lrEng { get; set; }
            public double lrMet { get; set; }
            public double ShutHeightEng { get; set; }
            public double ShutHeightMet { get; set; }
            public string NumberOfStations { get; set; }
            public string PartNumber { get; set; }
            public string PartDescription { get; set; }
            public double BlankWidthEnglish { get; set; }
            public double BlankWidthMetric { get; set; }
            public double BlankPitchEnglish { get; set; }
            public double BlankPitchMetric { get; set; }
            public double MaterialThicknessEnglish { get; set; }
            public double MaterialThicknessMetric { get; set; }
            public string PartPicture { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAddress1 { get; set; }
            public string CustomerAddress2 { get; set; }
            public string CustomerAddress3 { get; set; }
            public string CustomerCity { get; set; }
            public string CustomerState { get; set; }
            public string CustomerZip { get; set; }
            public string MaterialType { get; set; }
            public string ToolCountry { get; set; }
            public int TSGCompanyID { get; set; }
            public string TSGCompany { get; set; }
            public string TSGCompanyAbbrev { get; set; }
            public string TSGAddress1 { get; set; }
            public string TSGAddress2 { get; set; }
            public string TSGAddress3 { get; set; }
            public string TSGCity { get; set; }
            public string TSGState { get; set; }
            public string TSGZip { get; set; }
            public string TSGPhone { get; set; }
            public string LineNumber { get; set; }
            public string CustomerContactName { get; set; }
            public string logo { get; set; }
            public string customerPartNumbers { get; set; }
            public string salesman { get; set; }
            public string estimatorName { get; set; }
            public string estimatorEmail { get; set; }
            public string currency { get; set; }
            public string jobNumber { get; set; }
            public string shippingLocation { get; set; }
            public string oldQuoteNumber { get; set; }
            public DateTime Created { get; set; }
            public string cellPicture { get; set; }
            public bool? Firm { get; set; }
            public string ECQuote { get; set; }
            public string ECBaseQuoteId { get; set; }
            public string ECQuoteNumber { get; set; }
    }
}