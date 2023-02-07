using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.SharePoint.Client;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net;
using System.Security;

namespace RFQ
{
    public partial class PDFQuote : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // We need to get this when calling the function
            // Sample Quote Number is 85
            int quoteNumber = System.Convert.ToInt32(Request["quoteNumber"]);

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;


            //Select mtyMaterialType from pktblMaterialType, tblPart where prtPartMaterialType = mtyMaterialTypeID and prtPARTID = @partID

            sql.CommandText = "Select quoQuoteID, quoTSGCompanyID, TSGCompanyName, quoRFQID, quoTotalAmount, steShippingTerms, ptePaymentTerms, quoLeadTime, quoEstimatorID, ";
            sql.CommandText += "rfqCustomerID, rfqCustomerRFQNumber, dtyFullName, cavCavityName, dinSizeFrontToBackEnglish, dinSizeFrontToBackMetric, dinSizeLeftToRightEnglish, dinSizeLeftToRightMetric, ";
            sql.CommandText += "dinSizeShutHeightEnglish, dinSizeShutHeightMetric, dinNumberOfStations, prtPartNumber, prtpartDescription, prtPicture, CustomerName, ";
            sql.CommandText += "Address1, Address2, Address3, City, State, Zip, mtyMaterialType, tcyToolCountry, binMaterialThicknessEnglish, binMaterialThicknessMetric, binMaterialPitchEnglish, binMaterialPitchMetric, ";
            sql.CommandText += "binMaterialWidthEnglish, binMaterialWidthMetric ";
            sql.CommandText += "from tblQuote, tblRFQ, DieType, linkPartToQuote, linkDieInfoToQuote, Customer, CustomerLocation, tblPart, tblDieInfo, ";
            sql.CommandText += "TSGCompany, pktblShippingTerms, pktblPaymentTerms,  pktblCavity, pktblMaterialType, pktblToolCountry, pktblBlankInfo ";
            sql.CommandText += "where quoQuoteID = @quoteNum and quoQuoteID = ptqQuoteID and prtPartID = ptqPartID and dinDieInfoID = diqDieInfoID and quoQuoteID = diqQuoteID and rfqID = quoRFQID and ";
            sql.CommandText += "rfqCustomerID = Customer.CustomerID and rfqCustomerID = CustomerLocation.CustomerID and dinDieType = DieTypeID and quoTSGCompanyID = TSGCompany.TSGCompanyID and cavCavityID = dinCavityID and ";
            sql.CommandText += "quoShippingTermsID = steShippingTermsID and quoPaymentTermsID = ptePaymentTermsID and prtPartMaterialType = mtyMaterialTypeID and rfqToolCountryID = tcyToolCountryID and prtBlankInfoID = binBlankInfoID";

            sql.Parameters.AddWithValue("@quoteNum", quoteNumber);

            FullQuote q = new FullQuote();
            SqlDataReader dr3 = sql.ExecuteReader();
            //This should be all we need to create the quote and in the correct format except for pulling in pictures

            if (dr3.Read())
            {
                q.QuoteID = dr3.GetValue(0).ToString();
                q.TSGCompanyID = System.Convert.ToInt32(dr3.GetValue(1));
                q.TSGCompany = dr3.GetValue(2).ToString();
                q.RFQID = System.Convert.ToInt32(dr3.GetValue(3));
                q.Date = DateTime.Now.ToString("d");
                q.TotalCost = System.Convert.ToDouble(dr3.GetValue(4));
                q.ShippingTerms = dr3.GetValue(5).ToString();
                q.PaymentTerms = dr3.GetValue(6).ToString();
                q.LeadTime = dr3.GetValue(7).ToString();
                q.EstimatorID = System.Convert.ToInt32(dr3.GetValue(8));
                q.CustomerID = System.Convert.ToInt32(dr3.GetValue(9));
                q.CustomerRFQNumber = dr3.GetValue(10).ToString();
                q.DieType = dr3.GetValue(11).ToString();
                q.Cavity = dr3.GetValue(12).ToString();
                q.fbEng = System.Convert.ToDouble(dr3.GetValue(13));
                q.fbMet = System.Convert.ToDouble(dr3.GetValue(14));
                q.lrEng = System.Convert.ToDouble(dr3.GetValue(15));
                q.lrMet = System.Convert.ToDouble(dr3.GetValue(16));
                q.ShutHeightEng = System.Convert.ToDouble(dr3.GetValue(17));
                q.ShutHeightMet = System.Convert.ToDouble(dr3.GetValue(18));
                q.NumberOfStations = dr3.GetValue(19).ToString();
                q.PartNumber = dr3.GetValue(20).ToString();
                q.PartDescription = dr3.GetValue(21).ToString();
                q.PartPicture = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/" + dr3.GetValue(22).ToString();
                q.CustomerName = dr3.GetValue(23).ToString();
                q.CustomerAddress1 = dr3.GetValue(24).ToString();
                q.CustomerAddress2 = dr3.GetValue(25).ToString();
                q.CustomerAddress3 = dr3.GetValue(26).ToString();
                q.CustomerCity = dr3.GetValue(27).ToString();
                q.CustomerState = dr3.GetValue(28).ToString();
                q.CustomerZip = dr3.GetValue(29).ToString();
                q.MaterialType = dr3.GetValue(30).ToString();
                q.ToolCountry = dr3.GetValue(31).ToString();
                q.MaterialThicknessEnglish = System.Convert.ToDouble(dr3.GetValue(32).ToString());
            }

            q.MaterialThicknessMetric = 2;
            q.BlankPitchEnglish = 3;
            q.BlankPitchMetric = 4;
            q.BlankWidthEnglish = 5;
            q.BlankWidthMetric = 6;


            dr3.Close();
            List<string> quoteNotes = new List<string>();
            List<string> costNotes = new List<string>();

            sql.CommandText = "Select pwnPreWordedNote, pwnCostNote ";
            sql.CommandText += "from linkPWNToQuote, pktblPreWordedNote ";
            sql.CommandText += "where pwqQuoteID = @quote and pwqPreWordedNoteID = pwnPreWordedNoteID";

            sql.Parameters.AddWithValue("@quote", q.QuoteID);

            dr3 = sql.ExecuteReader();

            while (dr3.Read())
            {
                quoteNotes.Add(dr3.GetValue(0).ToString());
                costNotes.Add(dr3.GetValue(1).ToString());
            }
            dr3.Close();
            //Creating quote document in downloads folder right now
            Document quotePDF = new Document();
            string path = "C:\\Users\\" + Environment.UserName + "\\Downloads\\test.pdf";
            FileStream fs = System.IO.File.Create(path);
            PdfWriter writer = PdfWriter.GetInstance(quotePDF, fs);



            //Change to specific company and quote number
            quotePDF.AddTitle("TSG Quote");
            quotePDF.SetMargins(30f, 30f, 30f, 30f);
            quotePDF.Open();

            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(new Uri("http://www.toolingsystemsgroup.com/images//template/logo.gif"));
            logo.ScaleAbsolute(100f, 50f);
            logo.SetAbsolutePosition(30, 775);

            // Get Picture from Sharepoint

            String pictureName = "RFQ" +  q.RFQID  + "_" + q.PartNumber  + ".png";


            // This points to where the pictures are
            String siteUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating";
            String sharepointLibrary = "Part Pictures";
            String sharepointSubFolder = "";
            // Needed in order to work with ITextSharp - they can work with bytes or absolute paths

            byte[] pictureData;
            using (var clientContext = new ClientContext(siteUrl))
            {
                clientContext.Credentials = master.getSharePointCredentials();
 
                var url = new Uri(siteUrl);
                var relativeUrl = String.Format("{0}/{1}/{2}/{3}", url.AbsolutePath, sharepointLibrary, sharepointSubFolder, pictureName ); 
                // open the file as binary
                using (FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl))
                // loop through without first getting file length - do not really need it as long as we check length gt 0 on read
                using (var ms = new MemoryStream())
                {
                    var buf = new byte[1024 * 16];
                    int byteSize;
                    while ((byteSize = fileInfo.Stream.Read(buf, 0, buf.Length)) > 0)
                    {
                        ms.Write(buf, 0, byteSize);
                    }
                    pictureData = ms.ToArray();
                }
            }
            // bulid the itext picture with the byte array
            iTextSharp.text.Image partPicture = iTextSharp.text.Image.GetInstance(pictureData);
            // make it fit in our tight quote format.
            partPicture.ScaleAbsolute(150f, 100f);
            partPicture.SetAbsolutePosition(400, 630);
            quotePDF.Add(partPicture);
            //iTextSharp.text.Image signature = iTextSharp.text.Image.GetInstance(new Uri("https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software%20Development%20Site/RFQAndQuotingApplicationProject/_layouts/15/Lightbox.aspx?url=https%3A%2F%2Ftoolingsystemsgroup.sharepoint.com%2FTSG%2FIT%2FSoftware%20Development%20Site%2FRFQAndQuotingApplicationProject%2FShared%20Documents%2FImages%2FChad%20Gould-signature.jpg"));
            //signature.ScaleAbsolute(150f, 100f);
            //signature.SetAbsolutePosition(400, 630);

            quotePDF.Add(logo);



            BaseFont basefont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            BaseFont boldFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            PdfContentByte cb = writer.DirectContent;

            cb.BeginText();

            cb.SetFontAndSize(boldFont, 9);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.TSGCompany, 180, 815, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, "Quotation #:", 450, 815, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, "Customer RFQ:", 450, 800, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, "Date:", 450, 785, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, "Job #:", 450, 770, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, "Access Database:", 450, 755, 0);

            cb.SetFontAndSize(basefont, 9);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "555 Plymouth Ave", 180, 805, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Grand Rapids, Michigan", 180, 795, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "49504", 180, 785, 0);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.QuoteID.ToString(), 460, 815, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerRFQNumber, 460, 800, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.Date, 460, 785, 0);
            //This will need to be filled out when we start to do all of the job information stuff
            cb.ShowTextAligned(Element.ALIGN_LEFT, "", 460, 770, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TESTACCESS", 460, 755, 0);

            //Customer information
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerName, 40, 745, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress1, 40, 735, 0);
            int y = 725;
            if (q.CustomerAddress2 != "")
            {
                cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress2, 40, y, 0);
                y -= 10;
            }
            if (q.CustomerAddress3 != "")
            {
                cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerAddress2, 40, y, 0);
                y -= 10;
            }
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.CustomerCity + ", " + q.CustomerState + " " + q.CustomerZip, 40, y, 0);
            //We need to hook up customer contact
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Customer Contact: WE NEED TO GET THIS DYNAMICALLY", 40, y - 10, 0);

            cb.SetFontAndSize(boldFont, 9);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Process", 30, 655, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Number: ", 30, 640, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Part Name: ", 30, 625, 0);

            cb.SetFontAndSize(basefont, 9);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.DieType + " " + q.Cavity, 100, 655, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartNumber, 100, 640, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.PartDescription, 100, 625, 0);


            cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 140, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 180, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 260, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 300, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Inch", 390, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "mm", 430, 605, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Mat'l Type", 470, 605, 0);

            cb.SetFontAndSize(boldFont, 8);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "Blank Size: *", 30, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Width:", 105, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Pitch:", 225, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Mat'l Thk:", 340, 585, 0);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "Die Size: *", 30, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "F to B:", 105, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "L to R:", 225, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Shut Height:", 340, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "# of Stations:", 470, 565, 0);

            cb.SetFontAndSize(basefont, 8);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthEnglish.ToString("#,##0.000"), 140, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankWidthMetric.ToString("#,##0.00"), 180, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchEnglish.ToString("#,##0.000"), 260, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.BlankPitchMetric.ToString("#,##0.00"), 300, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialThicknessEnglish.ToString("#,##0.000"), 390, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialThicknessMetric.ToString("#,##0.00"), 430, 585, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.MaterialType, 470, 585, 0);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.fbEng.ToString("#,##0.000"), 140, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.fbMet.ToString("#,##0.00"), 180, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.lrEng.ToString("#,##0.000"), 260, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.lrMet.ToString("#,##0.00"), 300, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShutHeightEng.ToString("#,##0.000"), 390, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShutHeightMet.ToString("#,##0.00"), 430, 565, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.NumberOfStations.ToString(), 525, 565, 0);

            cb.SetFontAndSize(basefont, 6);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "* Stock size and die size are approximate", 30, 555, 0);

            cb.SetFontAndSize(boldFont, 12);

            cb.ShowTextAligned(Element.ALIGN_CENTER, "DESCRIPTION", 230, 535, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "COST", 508, 535, 0);

            cb.SetFontAndSize(basefont, 8);

            y = 518;

            //Displaying all notes since they are dynamically sized
            for (int i = 0; i < quoteNotes.Count; i++)
            {
                cb.ShowTextAligned(Element.ALIGN_LEFT, quoteNotes[i], 30, y, 0);
                try
                {
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format("{0:C}", System.Convert.ToDouble(costNotes[i])), 570, y, 0);
                }
                catch
                {
                    cb.ShowTextAligned(Element.ALIGN_RIGHT, costNotes[i], 570, y, 0);
                }

                y -= 10;
            }

            cb.SetFontAndSize(basefont, 6);

            if (y < 318)
            {
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page 1 of 2", 570, 815, 0);
            }
            else
            {
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page 1 of 1", 570, 815, 0);
            }

            cb.EndText();

            cb.SaveState();
            cb.SetColorFill(BaseColor.LIGHT_GRAY);
            cb.SetLineWidth(3);

            //Customer Information Box
            cb.MoveTo(30, 760);
            cb.LineTo(350, 760);
            cb.LineTo(350, 680);
            cb.LineTo(30, 680);
            cb.ClosePathStroke();

            cb.SetLineWidth((float)1.5);
            cb.SetColorStroke(BaseColor.BLACK);

            //Description Box
            cb.MoveTo(30, 550);
            cb.LineTo(430, 550);
            cb.LineTo(430, 530);
            cb.LineTo(30, 530);
            cb.ClosePathStroke();

            //Cost Box
            cb.MoveTo(445, 550);
            cb.LineTo(570, 550);
            cb.LineTo(570, 530);
            cb.LineTo(445, 530);
            cb.ClosePathStroke();
            cb.RestoreState();

            if (y < 318)
            {
                quotePDF.NewPage();
                y = 800;
            }

            cb.BeginText();

            cb.SetFontAndSize(basefont, 6);
            if (y == 800)
            {
                cb.SetFontAndSize(basefont, 6);
                cb.ShowTextAligned(Element.ALIGN_RIGHT, "Page 2 of 2", 570, 815, 0);
            }

            cb.SetFontAndSize(boldFont, 10);

            cb.ShowTextAligned(Element.ALIGN_CENTER, "Lead Time of " + q.LeadTime + " Weeks", 230, y - 13, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "Total (USD)", 460, y - 13, 0);
            cb.ShowTextAligned(Element.ALIGN_RIGHT, String.Format("{0:C}", q.TotalCost), 565, y - 13, 0);

            cb.SetFontAndSize(boldFont, 8);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "Shipping:", 30, y - 25, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Payment Terms:", 30, y - 35, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Country of Orign:", 30, y - 45, 0);

            cb.SetFontAndSize(basefont, 8);

            cb.ShowTextAligned(Element.ALIGN_LEFT, q.ShippingTerms, 100, y - 25, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.PaymentTerms, 100, y - 35, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, q.ToolCountry, 100, y - 45, 0);

            cb.SetFontAndSize(basefont, 6);

            cb.ShowTextAligned(Element.ALIGN_CENTER, "General Notes", 300, y - 60, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Lead time starts after:", 34, y - 70, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "-Receipt of purchase order", 45, y - 80, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "-Receipt of finalized complete surfaced math data", 45, y - 90, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "-Review of GD&T requirements", 45, y - 100, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "-Customer approval of process and approval of simulation", 45, y - 110, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Simulation may be required to prove process and part feasibility", 34, y - 120, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Part concessions may be required in order for part to be production feasible", 34, y - 130, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Early part requirements prior to die buyoff will be quoted as an additional cost unless noted above", 34, y - 140, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "No scrap conveyors or shakers are quoted - must be added later at an additional cost if the press or bolster requires", 34, y - 150, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TSG to receive check fixture and production grade tryout material 50% through die build", 34, y - 160, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TSG remains harmless of any cost associated with delivery implications caused by acts of God and any issues beyond our control", 34, y - 170, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "TSG Terms and Conditions of Sale can be found at toolingystemsgroup.com", 34, y - 180, 0);

            cb.SetFontAndSize(basefont, 8);

            cb.ShowTextAligned(Element.ALIGN_LEFT, "Company Representative: ", 30, y - 268, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Salesman: ", 30, y - 278, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "Authorization to Proceed Signature", 310, y - 268, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, "** THIS QUOTATION IS VALID FOR 30 DAYS UNLESS OTHERWISE SPECIFIED **", 30, y - 288, 0);

            cb.SetFontAndSize(basefont, 7);

            cb.ShowTextAligned(Element.ALIGN_CENTER, "Thank you for your consideration. We appreciate your business. Work shall", 440, y - 196, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "commence at the receipt of your Purchase Order or Signed Authorization to", 440, y - 206, 0);
            cb.ShowTextAligned(Element.ALIGN_CENTER, "Proceed by an authorized representative for the terms listed on the Quote:", 440, y - 216, 0);

            cb.SetLineWidth(1);

            cb.EndText();

            //Total Box
            cb.MoveTo(430, y);
            cb.LineTo(490, y);
            cb.LineTo(490, y - 18);
            cb.LineTo(430, y - 18);
            cb.ClosePathStroke();

            //Price Box
            cb.MoveTo(490, y);
            cb.LineTo(570, y);
            cb.LineTo(570, y - 18);
            cb.LineTo(490, y - 18);
            cb.ClosePathStroke();

            //General Notes Header Box
            cb.MoveTo(30, y - 53);
            cb.LineTo(570, y - 53);
            cb.LineTo(570, y - 63);
            cb.LineTo(30, y - 63);
            cb.ClosePathStroke();

            //General Notes Box
            cb.MoveTo(30, y - 63);
            cb.LineTo(570, y - 63);
            cb.LineTo(570, y - 183);
            cb.LineTo(30, y - 183);
            cb.ClosePathStroke();

            //THANK YOU BOX
            cb.MoveTo(310, y - 188);
            cb.LineTo(570, y - 188);
            cb.LineTo(570, y - 223);
            cb.LineTo(310, y - 223);
            cb.ClosePathStroke();

            //Signature lines
            cb.MoveTo(30, y - 258);
            cb.LineTo(290, y - 258);
            cb.Stroke();

            cb.MoveTo(310, y - 258);
            cb.LineTo(570, y - 258);
            cb.Stroke();

            quotePDF.Close();
            connection.Close();
        }
    }

}