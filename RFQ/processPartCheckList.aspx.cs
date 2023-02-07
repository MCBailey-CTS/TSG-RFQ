using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RFQ
{
    public partial class processPartCheckList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string rfq = Request["rfq"];
            string part = Request["part"];
            string get = Request["get"];
            if (part == "")
            {
                processRFQ(rfq, get);
            }
            else
            {
                string allParts = Request["all"];
                processPart(rfq, part, get, allParts);
            }
        }

        protected void processRFQ(string rfq, string get)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            if (get == "1")
            {
                sql.CommandText = "select rtcRFQCheckListID from   linkRFQToRFQCheckList where rtcRFQID=@rfq  ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfq);
                SqlDataReader dr = sql.ExecuteReader();
                string separator = "";
                while (dr.Read())
                {
                    Response.Write(separator);
                    Response.Write(dr.GetValue(0).ToString());
                    separator = ",";
                }
                dr.Close();
            }
            else
            {
                foreach (string clvalue in Request["val"].Split(','))
                {
                    if ((clvalue != null) && (clvalue.Trim() != ""))
                    {
                        sql.CommandText = "select rtcRFQCheckListID from linkRFQToRFQCheckList where  rtcRFQID=@rfq and rtcRFQCheckListID=@cl ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        sql.Parameters.AddWithValue("@cl", clvalue);
                        SqlDataReader dr = sql.ExecuteReader();
                        Boolean exists = false;
                        while (dr.Read())
                        {
                            exists = true;
                        }
                        dr.Close();
                        if (!exists)
                        {
                            sql.CommandText = "insert into linkRFQToRFQCheckList(rtcRFQID, rtcRFQCheckListID) values (@rfq, @cl) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@rfq", rfq);
                            sql.Parameters.AddWithValue("@cl", clvalue);
                            master.ExecuteNonQuery(sql,"processPartCheckList");
                        }
                    }
                }
                if (Request["val"].ToString() != "")
                {
                    sql.CommandText = "delete from linkRFQToRFQCheckList where rtcRFQID=@rfq and rtcRFQCheckListID Not in (" + Request["val"] + ") ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfq);
                    master.ExecuteNonQuery(sql,"processPartCheckList");
                }
                else
                {
                    sql.CommandText = "delete from linkRFQToRFQCheckList where rtcRFQID=@rfq ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfq);
                    master.ExecuteNonQuery(sql,"processPartCheckList");
                }
                string retval = "checklist.png";
                sql.CommandText = "select rtcRFQCheckListID from linkRFQToRFQCheckList where  rtcRFQID=@rfq "; 
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfq);
                SqlDataReader rdr = sql.ExecuteReader();
                while (rdr.Read())
                {
                    retval = "issues.png";
                }
                rdr.Close();
                Response.Write(retval);
            }
            connection.Close();
        }

        protected void processPart(string rfq, string part, string get, string allParts) 
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            if (get == "1") 
            {
                sql.CommandText = "select prrRFQCheckListID from tblPart, linkPartToRFQ, linkPartToRFQToRFQCheckList where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq and ptrPartToRFQID = prrPartToRFQID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", part);
                sql.Parameters.AddWithValue("@rfq", rfq);
                SqlDataReader dr = sql.ExecuteReader();
                string separator = "";
                while (dr.Read())
                {
                    Response.Write(separator);
                    Response.Write(dr.GetValue(0).ToString());
                    separator=",";
                }
                dr.Close();
            }
            else
            {
                foreach (string clvalue in Request["val"].Split(','))
                {
                    if ((clvalue != null ) && (clvalue.Trim() != "")) 
                    {
                        if (allParts == "1")
                        {
                            List<String> PartList = new List<string>();
                            sql.CommandText = "select prtPartID from tblPart, linkPartToRFQ where prtPartID=ptrPartID and ptrRFQID=@rfq";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@rfq", rfq);
                            SqlDataReader dr = sql.ExecuteReader();
                            while (dr.Read())
                            {
                                PartList.Add(dr.GetValue(0).ToString());
                            }
                            dr.Close();
                            foreach (String PartID in PartList)
                            {
                                ApplyToPart(PartID, rfq, clvalue);
                            }
                        }
                        else
                        {
                            ApplyToPart(part, rfq, clvalue);
                        }
                    }
                }
                if (Request["val"].ToString() != "") 
                {
                    if (allParts != "1") 
                    {
                        sql.CommandText = "delete from linkPartToRFQToRFQCheckList where prrPartToRFQID in (select ptrPartToRFQID from tblPart, linkPartToRFQ where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq) and prrRFQCheckListID Not in (" + Request["val"] + ") ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@part", part);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        master.ExecuteNonQuery(sql, "processPartCheckList");
                    }
                }
                else
                {
                    if (allParts != "1")
                    {
                        sql.CommandText = "delete from linkPartToRFQToRFQCheckList where prrPartToRFQID in (select ptrPartToRFQID from tblPart, linkPartToRFQ where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@part", part);
                        sql.Parameters.AddWithValue("@rfq", rfq);
                        master.ExecuteNonQuery(sql, "processPartCheckList");
                    }
                }
                string retval = "checklist.png";
                sql.CommandText = "select prrRFQCheckListID from tblPart, linkPartToRFQ, linkPartToRFQToRFQCheckList where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq and ptrPartToRFQID = prrPartToRFQID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", part);
                sql.Parameters.AddWithValue("@rfq", rfq);
                SqlDataReader rdr = sql.ExecuteReader();
                while (rdr.Read())
                {
                    retval = "issues.png";
                }
                rdr.Close();
                Response.Write(retval);
            }
            connection.Close();
        }
        protected void ApplyToPart(String part, String rfq, String CheckListID)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            sql.CommandText = "select prrRFQCheckListID from tblPart, linkPartToRFQ, linkPartToRFQToRFQCheckList where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq and ptrPartToRFQID = prrPartToRFQID and prrRFQCheckListID=@cl ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@part", part);
            sql.Parameters.AddWithValue("@rfq", rfq);
            sql.Parameters.AddWithValue("@cl", CheckListID);
            SqlDataReader dr = sql.ExecuteReader();
            Boolean exists = false;
            while (dr.Read())
            {
                exists = true;
            }
            dr.Close();
            if (!exists)
            {
                sql.CommandText = "insert into linkPartToRFQToRFQCheckList(prrPartToRFQID, prrRFQCheckListID) select ptrPartToRFQID, @cl from tblPart, linkPartToRFQ where prtPARTID=@part and prtPartID=ptrPartId and ptrRFQID=@rfq  ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", part);
                sql.Parameters.AddWithValue("@rfq", rfq);
                sql.Parameters.AddWithValue("@cl", CheckListID);
                master.ExecuteNonQuery(sql, "processPartCheckList");
            }
            connection.Close();
        }
    }
}