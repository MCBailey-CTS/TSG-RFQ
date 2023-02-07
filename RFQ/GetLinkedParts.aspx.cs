using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    // Either returns all linked parts to the requestor, or if create=yes, creates link between the parts or if delete=yes, deletes link between the parts
    public partial class GetLinkedParts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            Int32 partID = 0;
            String rfqID = "0";
            Int32 partToLinkID = 0;
            string createLink = "";
            string deleteLink = "";
            if (Request["part"] != "")
            {
                //first part
                partID = System.Convert.ToInt32(Request["part"]);
            }
            if (Request["rfq"] != "" && Request["rfq"] != null)
            {
                rfqID = Request["rfq"];
            }
            if(Request["link"] != "")
            {
                //second part
                partToLinkID = System.Convert.ToInt32(Request["link"]);
            }
            if (Request["create"] != "")
            {
                createLink = Request["create"].ToLower();
            }
            if (Request["delete"] != "")
            {
                deleteLink = Request["delete"].ToLower();
            }

            Int32 LinkID = 0;
            if (createLink == "yes")
            {
                Boolean existingLink = false;
                LinkID = GetCurrentLink(rfqID, partID);
                if (LinkID == 0)
                {
                    sql.CommandText = "insert into linkPartToPart (ptpCreated, ptpCreatedBy, ptpModified, ptpModifiedBy) ";
                    sql.CommandText += " output inserted.ptpPartToPartID ";
                    sql.CommandText += " values (current_timestamp, @user, current_timestamp, @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    LinkID = (int)master.ExecuteScalar(sql, "GetLinkedParts");
                }
                else
                {
                    existingLink = true;
                }
                sql.CommandText = "Select ptqQuoteID, ptqHTS, ptqSTS, ptqUGS from linkPartToQuote where ptqPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                SqlDataReader dr = sql.ExecuteReader();
                string quoteID = "", hts = "", sts = "", ugs = "";
                if(dr.Read())
                {
                    quoteID = dr.GetValue(0).ToString();
                    hts = dr.GetValue(1).ToString();
                    sts = dr.GetValue(2).ToString();
                    ugs = dr.GetValue(3).ToString();
                }
                dr.Close();
                if (quoteID != "")
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "insert into linkPartToQuote (ptqPartID, ptqQuoteID, ptqCreated, ptqCreatedBy, ptqHTS, ptqSTS, ptqUGS) ";
                    sql.CommandText += "output inserted.ptqPartToQuoteID ";
                    sql.CommandText += "values (@partID, @quoteID, GETDATE(), @createdBy, @hts, @sts, @ugs);";
                    sql.Parameters.AddWithValue("@partID", partToLinkID);
                    sql.Parameters.AddWithValue("@quoteID", quoteID);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    sql.Parameters.AddWithValue("@hts", hts);
                    sql.Parameters.AddWithValue("@sts", sts);
                    sql.Parameters.AddWithValue("@ugs", ugs);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");

                    sql.Parameters.Clear();
                    sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
                    sql.Parameters.AddWithValue("@rfq", rfqID);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");
                    sql.Parameters.Clear();
                }
                if(!existingLink)
                {
                    CreatePartLink(LinkID, partID);
                }
                CreatePartLink(LinkID, partToLinkID);
                sql.CommandText = "Delete from linkPartReservedToCompany where prcPartID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", partToLinkID);
                master.ExecuteNonQuery(sql, "GetLinkedParts");
            }
            if (deleteLink == "yes")
            {
                LinkID = GetCurrentLink(rfqID, partID);
                int count = 0;
                sql.CommandText = "Select count(*) from linkPartToPartDetail where ppdPartToPartID = @link";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@link", LinkID);
                SqlDataReader dr2 = sql.ExecuteReader();
                if(dr2.Read())
                {
                    count = System.Convert.ToInt32(dr2.GetValue(0).ToString());
                }
                dr2.Close();

                if (LinkID > 0) {
                    sql.CommandText = "delete from linkPartToPartDetail where ppdPartId=@part and ppdPartToPartId=@link";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@part", partToLinkID);
                    sql.Parameters.AddWithValue("@link", LinkID);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");

                    sql.Parameters.Clear();
                    sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
                    sql.Parameters.AddWithValue("@rfq", rfqID);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");

                    sql.Parameters.Clear();
                    //sql.CommandText = "Delete from linkPartToQUote where ptqPartID = @partID";
                    //sql.Parameters.AddWithValue("@partID", LinkID);
                    //master.ExecuteNonQuery(sql, "GetLinkedParts");
                }
                if(count == 2)
                {
                    sql.CommandText = "delete from linkPartToPartDetail where ppdPartId=@part and ppdPartToPartId=@link";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@part", partID);
                    sql.Parameters.AddWithValue("@link", LinkID);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");

                    sql.CommandText = "delete from linkPartToPart where ptpPartToPartID=@link ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@link", LinkID);
                    master.ExecuteNonQuery(sql, "GetLinkedParts");
                }
                //sql.CommandText = "Select ptqQuoteID from linkPartToQuote where ptqpartID = @partID";
                //sql.Parameters.AddWithValue("@partID", partID);
                //SqlDataReader dr = sql.ExecuteReader();

                //string quoteID = "";
                //if(dr.Read())
                //{
                //    quoteID = dr.GetValue(0).ToString();
                //}
                //dr.Close();

                //if(quoteID != "")
                //{
                //    //sql.Parameters.Clear();
                //    //sql.CommandText = "delete from linkPartToQuote where ptqQuoteID = @quoteID and ptqPartID = @partID";
                //    //sql.Parameters.AddWithValue("@quoteID", quoteID);
                //    //sql.Parameters.AddWithValue("@partID", partToLinkID);

                //    //master.ExecuteNonQuery(sql, "GetLinkedParts");
                //}

                //// if there is only one item in the parttopartdetail for that link, then delete it and the parttopart record as well.
                //sql.CommandText = "select count(*) from linkPartToPartDetail where ppdPartToPartID=@link";
                //sql.Parameters.Clear();
                //sql.Parameters.AddWithValue("@link", LinkID);
                //SqlDataReader drRecordCount = sql.ExecuteReader();
                //Int32 recordCount = 0;
                //while (drRecordCount.Read())
                //{
                //    recordCount = drRecordCount.GetInt32(0);
                //}
                //drRecordCount.Close();
                //if (recordCount <= 1)
                //{
                //    // if there is only one record left with that link id, then delete it as it is no longer linked to anything else
                //    // just go ahead and do the delete - may or may not be there still
                //    sql.CommandText = "delete from linkPartToPartDetail where ppdPartToPartID=@link ";
                //    sql.Parameters.Clear();
                //    sql.Parameters.AddWithValue("@link", LinkID);
                //    master.ExecuteNonQuery(sql, "GetLinkedParts");
                //    // delete the header record now since there is no detail using it.
                //    sql.CommandText = "delete from linkPartToPart where ptpPartToPartID=@link ";
                //    sql.Parameters.Clear();
                //    sql.Parameters.AddWithValue("@link", LinkID);
                //    master.ExecuteNonQuery(sql, "GetLinkedParts");

                //    sql.Parameters.Clear();
                //    sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
                //    sql.Parameters.AddWithValue("@rfq", rfqID);
                //    master.ExecuteNonQuery(sql, "GetLinkedParts");
                //}
            }
            sql.Parameters.Clear();
            LinkID = 0;
            sql.CommandText = "select ppdPartToPartId from linkPartToPartDetail, linkPartToRFQ  ";
            sql.CommandText += " where ppdPartID = @part and ppdPartID = ptrPartId and ptrRFQId=@rfq ";
            sql.Parameters.AddWithValue("@rfq", rfqID);
            sql.Parameters.AddWithValue("@part", partID);
            SqlDataReader drLink = sql.ExecuteReader();
            while (drLink.Read())
            {
                LinkID = drLink.GetInt32(0);
            }
            drLink.Close();

            sql.CommandText = "select prtRFQLineNumber from tblPart where prtPARTID = @partID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partID", partID);
            drLink = sql.ExecuteReader();
            string lineNum = "";
            if(drLink.Read())
            {
                lineNum = drLink.GetValue(0).ToString();
            }
            drLink.Close();

            // Whether Link ID is zero or not, this should reutrn all of the parts in the RFQ
            // That are NOT linked to other parts
            sql.Parameters.Clear();
            sql.CommandText = "select prtPartNumber as PartName,  prtPartID as PartId, prtPartDescription as PartDescription, coalesce(ppdPartToPartID,0) as LinkID, prtRFQLineNumber as LineNumber from linKPartToRFQ, tblPart left outer join linkPartToPartDetail on prtPartId=ppdPartID and ppdPartToPartID=@link ";
            sql.CommandText += " where ptrPartID=prtPartId and ptrRFQID=@rfq ";
            sql.CommandText += " and prtPartID not in (select ppdPartID from linkPartToPartDetail where ppdPartToPartId != @link)  ";
            sql.CommandText += " order by iif(prtRFQLineNumber=@lineNum,0,1), prtPartName ";
            sql.CommandText += " FOR XML PATH('LinkedParts'), ROOT('Part'), ELEMENTS xsinil;";
            sql.Parameters.AddWithValue("@rfq", rfqID);
            sql.Parameters.AddWithValue("@part", partID);
            sql.Parameters.AddWithValue("@link", LinkID);
            sql.Parameters.AddWithValue("@lineNum", lineNum);
            drLink = sql.ExecuteReader();
            while (drLink.Read())
            {
                //We are checking to see if this result is already in the literal
                //If it is not then we want to insert it into the literal
                if (!litReturn.ToString().Contains(drLink.GetValue(0).ToString()) && !drLink.GetValue(0).ToString().Equals(""))
                {
                    litReturn.Text += drLink.GetValue(0).ToString();
                }
            }
            drLink.Close();
            connection.Close();
        }
        protected Int32 GetCurrentLink(String rfq, Int32 partID)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            Int32 ReturnLink = 0;
            sql.Parameters.Clear();
            sql.CommandText = "select ppdPartToPartId from linkPartToPartDetail  ";
            sql.CommandText += "where ppdPartID = @part";
            //sql.Parameters.AddWithValue("@rfq", rfq);
            sql.Parameters.AddWithValue("@part", partID);
            SqlDataReader dr = sql.ExecuteReader();

            if (dr.Read())
            {
                ReturnLink = dr.GetInt32(0);
            }
            dr.Close();
            connection.Close();
            return ReturnLink;
        }
        protected void CreatePartLink(Int32 linkID, Int32 partID ) {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            sql.Parameters.Clear();
            sql.CommandText = "select * from linkPartToPartDetail where ppdPartID=@part and ppdPartToPartID=@link";
            Boolean exists = false;
            sql.Parameters.AddWithValue("@part", partID);
            sql.Parameters.AddWithValue("@link", linkID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read() ) 
            {
                exists = true;
            }
            dr.Close();
            if (! exists) {
                sql.CommandText = "insert into linkPartToPartDetail (ppdPartToPartId, ppdPartID, ppdCreated, ppdCreatedBy, ppdModified, ppdModifiedBy) ";
                sql.CommandText += " values (@link, @part, current_timestamp, @user, current_timestamp, @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", partID);
                sql.Parameters.AddWithValue("@link", linkID);
                sql.Parameters.AddWithValue("@user",master.getUserName());
                master.ExecuteNonQuery(sql, "Get Linked Parts");
            }

            connection.Close();
        }
    }
}