using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace RFQ
{
    public partial class GetHistory : System.Web.UI.Page
    {
        //Initial linking is done in the edit rfq page to get rid of the need to have javascript run
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            SqlCommand sql2 = new SqlCommand();
            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            sql2.Connection = connection2;

            var ds = new DataSet("Part");
            var dt = ds.Tables.Add("History");
            dt.Columns.Add("PartID", typeof(int));
            dt.Columns.Add("PartNumber", typeof(string));
            dt.Columns.Add("PartDescription", typeof(string));
            dt.Columns.Add("RFQID", typeof(string));
            dt.Columns.Add("QuoteID", typeof(string));
            dt.Columns.Add("Customer", typeof(string));
            dt.Columns.Add("CustomerRFQNumber", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("NoQuoteReason", typeof(string));
            dt.Columns.Add("TSGCompany", typeof(string));
            dt.Columns.Add("PartHistoryID", typeof(string));
            dt.Columns.Add("QuoteNumber", typeof(string));
            dt.Columns.Add("picture", typeof(string));


            String partNum = "";
            String rfqID = "0";
            string searchString = "";
            string createLinks = "";
            int partID = 0;
            string desc = "";
            string cust = "";
            string custRFQ = "";
            string quoteNumber = "";
            DateTime start = System.Convert.ToDateTime("2000-11-11");
            DateTime end = System.Convert.ToDateTime("2060-11-11"); ;


            if (Request["part"] != "")
            {
                partNum = Request["part"];
            }
            if (Request["rfq"] != "" && Request["rfq"] != null)
            {
                rfqID = Request["rfq"];
            }
            if (Request["search"] != "" && Request["search"] != "")
            {
                searchString = Request["search"];
            }
            if (Request["create"] != "")
            {
                createLinks = Request["create"].ToLower();
            }
            if(Request["desc"] != null && Request["desc"] != "")
            {
                desc = Request["desc"];
            }
            if(Request["cust"] != null && Request["cust"] != "")
            {
                cust = Request["cust"];
            }
            if(Request["custRFQ"] != null && Request["custRFQ"] != "")
            {
                custRFQ = Request["custRFQ"];
            }
            if(Request["start"] != null && Request["start"] != "undefined" && Request["start"] != "")
            {
                start = System.Convert.ToDateTime(Request["start"]);
            }
            if(Request["end"] != null && Request["end"] != "undefined" && Request["end"] != "")
            {
                end = System.Convert.ToDateTime(Request["end"]);
            }
            if (Request["partID"] != null)
            {
                partID = System.Convert.ToInt32(Request["partID"]);
            }
            else
            {
                sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ where ptrRFQID = @rfq and prtPartNumber = @partNum and prtPARTID = ptrPartID";
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@partNum", partNum);

                SqlDataReader partIDDR = sql.ExecuteReader();

                if (partIDDR.Read())
                {
                    partID = System.Convert.ToInt32(partIDDR.GetValue(0));
                }
                partIDDR.Close();
            }
            if (Request["quoteNum"] != null && Request["quoteNum"] != "")
            {
                quoteNumber = Request["quoteNum"];
            }
            //else
            //{
            //    sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ where ptrRFQID = @rfq and prtPartNumber = @partNum and prtPARTID = ptrPartID";
            //    sql.Parameters.AddWithValue("@rfq", rfqID);
            //    sql.Parameters.AddWithValue("@partNum", partNum);

            //    SqlDataReader partIDDR = sql.ExecuteReader();

            //    if (partIDDR.Read())
            //    {
            //        partID = System.Convert.ToInt32(partIDDR.GetValue(0));
            //    }
            //    partIDDR.Close();
            //}

            
            // eliminate first string and change to first and second making the delim as wildcard %

            char[] delimiterChars = { ' ', '.', '_', '-', ',', '\\', '/','+'};      // add in / \ 
            // possible TODO - instead  of partNum, tokens would use searchString instead
            // only if users do not like how it works now (searching for searchString and partNum)
            String[] tokens = partNum.Split(delimiterChars);
            String removedEndTag = "%";
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                removedEndTag += tokens[i] + "%";
            }

            string longest = tokens.OrderByDescending(s => s.Length).First();

            string first = tokens[0];
            if(partNum.Trim().Length < 4)
            {
                partNum = "";
            }
            if (removedEndTag.Trim().Length < 4)
            {
                removedEndTag = "";
            }
            if(longest.Trim().Length < 4)
            {
                removedEndTag = "";
            }
            if(searchString.Trim().Length < 4)
            {
                searchString = "";
            }

            String[] search = { partNum.Trim(), removedEndTag.Trim(), longest.Trim(), searchString.Trim() };
            litHistory.Text = "";

            if(createLinks == "no")
            { 
                int company = System.Convert.ToInt32(master.getCompanyId());
                if (quoteNumber != "")
                {
                    if (!quoteNumber.ToUpper().Contains("HTS") && !quoteNumber.ToUpper().Contains("STS") && !quoteNumber.ToUpper().Contains("UGS") && !quoteNumber.ToUpper().Contains("SA"))
                    {
                        sql.CommandText = "Select TSGCompanyAbbrev, quoPartNumbers, quoPartName, quoRFQID, CustomerName, quoCustomerQuoteNumber, quoQuoteID, qstQuoteStatusDescription, ";
                        sql.CommandText += "TSGCompanyAbbrev, quoOldQuoteNumber, concat(rfqID, '-', prtRFQLineNumber, '-', TSGCompanyAbbrev, '-', quoVersion) as quoteNumber, prtPicture ";
                        sql.CommandText += "from tblQuote, tblRFQ, linkQuoteToRFQ, Customer, pktblQuoteStatus, TSGCompany, linkPartToQuote, tblPart ";
                        sql.CommandText += "where quoQuoteID = qtrQuoteID and rfqID = qtrRFQID and CustomerID = rfqCustomerID and qstQuoteStatusID = quoStatusID ";
                        sql.CommandText += "and TSGCompanyID = quoTSGCompanyID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
                        sql.CommandText += "and ptqPartID = prtPARTID and ptqQuoteID = quoQuoteID and ((rfqID = @rfq and (prtRFQLineNumber = @lineNum or @lineNum is null)) or  ";
                        sql.CommandText += "quoOldQuoteNumber = @oldQuoteNum) and (TSGCompanyAbbrev = @abbrev or @abbrev is null) and (quoVersion = @version or @version is null) ";
                        sql.Parameters.Clear();
                        string[] arr = quoteNumber.Split('-');
                        if (arr.Length >= 4)
                        {
                            sql.Parameters.AddWithValue("@rfq", arr[0]);
                            sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                            sql.Parameters.AddWithValue("@abbrev", arr[2]);
                            sql.Parameters.AddWithValue("@version", arr[3]);
                        }
                        else if (arr.Length == 3)
                        {
                            sql.Parameters.AddWithValue("@rfq", arr[0]);
                            sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                            sql.Parameters.AddWithValue("@abbrev", arr[2]);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }
                        else if (arr.Length == 2)
                        {
                            sql.Parameters.AddWithValue("@rfq", arr[0]);
                            sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }
                        else if (arr.Length == 1)
                        {
                            sql.Parameters.AddWithValue("@rfq", arr[0]);
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            //We either match the old quote num perfectly or we want something that deffinitely wont show up
                            sql.Parameters.AddWithValue("@oldQUoteNum", "asdlfjk;asdf");
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            sql.Parameters.AddWithValue("@oldQuoteNum", "adfl;kjasdfkl;");
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }

                        SqlDataReader dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            string quoteNum = "";
                            if (dr["quoOldQuoteNumber"].ToString() != "")
                            {
                                quoteNum = dr["quoOldQuoteNumber"].ToString() + "-" + dr["quoteNumber"].ToString().Split('-')[2] + "-" + dr["quoteNumber"].ToString().Split('-')[3];
                            }
                            else
                            {
                                quoteNum = dr["quoteNumber"].ToString();
                            }
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                        dr.GetValue(7).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-quo", quoteNum, dr["prtPicture"].ToString());
                        }
                        dr.Close();
                    }
                    else if (!quoteNumber.ToUpper().Contains("HTS") && !quoteNumber.ToUpper().Contains("STS") && !quoteNumber.ToUpper().Contains("UGS"))
                    {
                        sql.CommandText = "Select TSGCompanyAbbrev, ecqPartNumber, ecqPartName, '', CustomerName, ecqCustomerRFQNumber, ecqECQuoteID, qstQuoteStatusDescription, ";
                        sql.CommandText += "TSGCompanyAbbrev, ecqQuoteNumber, concat(ecqECQuoteID, '-', TSGCompanyAbbrev, '-SA-', ecqVersion) as quoteNumber, ecqPicture ";
                        sql.CommandText += "from tblECQuote, TSGCompany, Customer, pktblQuoteStatus ";
                        sql.CommandText += "where ecqTSGCompanyID = TSGCompanyID and ecqCustomer = CustomerID and qstQuoteStatusID = ecqStatus and (ecqECQuoteID = @quoteID or ";
                        sql.CommandText += "ecqQuoteNumber = @quoteID) and (TSGCompanyAbbrev = @abbrev or @abbrev is null) and (ecqVersion = @version or @version is null) ";
                        sql.Parameters.Clear();
                        string[] arr = quoteNumber.Split('-');
                        if (arr.Length >= 4)
                        {
                            sql.Parameters.AddWithValue("@quoteID", arr[0]);
                            sql.Parameters.AddWithValue("@abbrev", arr[1]);
                            sql.Parameters.AddWithValue("@version", arr[3]);
                        }
                        else if (arr.Length >= 2)
                        {
                            sql.Parameters.AddWithValue("@quoteID", arr[0]);
                            sql.Parameters.AddWithValue("@abbrev", arr[1]);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }
                        else if (arr.Length == 1)
                        {
                            sql.Parameters.AddWithValue("@quoteID", arr[0]);
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }
                        SqlDataReader dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            string quoteNum = "";
                            if (dr["ecqQuoteNumber"].ToString() != "")
                            {
                                quoteNum = dr["ecqQuoteNumber"].ToString() + "-" + dr["quoteNumber"].ToString().Split('-')[1] + "-" + dr["quoteNumber"].ToString().Split('-')[2] + "-" + dr["quoteNumber"].ToString().Split('-')[3];
                            }
                            else
                            {
                                quoteNum = dr["quoteNumber"].ToString();
                            }
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                        dr.GetValue(7).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-SA", quoteNum, dr["ecqPicture"].ToString());
                        }
                        dr.Close();
                    }
                    else if (quoteNumber.ToUpper().Contains("HTS"))
                    {
                        sql.CommandText = "Select 'HTS', hquPartNumbers, hquPartName, hquRFQID, CustomerName, hquCustomerRFQNum, hquHTSQuoteID, qstQuoteStatusDescription, ";
                        sql.CommandText += "'HTS', hquNumber, hquVersion, hquPicture, prtRFQLineNumber ";
                        sql.CommandText += "from Customer, pktblQuoteStatus, tblHTSQuote ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = hquHTSQuoteID and ptqHTS = 1 ";
                        sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "where hquCustomerID = CustomerID and hquStatusID = qstQuoteStatusID and hquRFQID = @rfqID and (hquVersion = @version ";
                        sql.CommandText += "or @version is null) and (hquHTSQuoteID = @quoteID or hquNumber = @quoteID or @quoteID is null) and (prtRFQLineNumber = @lineNum or ";
                        sql.CommandText += "@lineNum is null) ";
                        sql.Parameters.Clear();
                        string[] arr = quoteNumber.Split('-');

                        if (arr.Length >= 4)
                        {
                            if (quoteNumber.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@rfqID", "");
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length >= 2)
                        {
                            if (arr[1].ToLower() == "hts")
                            {
                                sql.Parameters.AddWithValue("@rfqID", "");
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length == 1)
                        {
                            sql.Parameters.AddWithValue("@rfqID", arr[0]);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                        }
                        SqlDataReader dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            string quoteNum = "";
                            //stand alone
                            if (quoteNumber.ToLower().Contains("sa") || (arr.Length == 2 && arr[1].ToLower() == "hts"))
                            {
                                if (dr["hquNumber"].ToString() != "")
                                {
                                    quoteNum = dr["hquNumber"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString();
                                }
                                else
                                {
                                    quoteNum = dr["hquHTSQuoteID"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString();
                                }
                            }
                            else
                            {
                                quoteNum = dr["hquRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString();
                            }
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                        dr.GetValue(7).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-HTS", quoteNum, dr["hquPicture"].ToString());
                        }
                        dr.Close();
                    }
                    else if (quoteNumber.ToUpper().Contains("STS"))
                    {
                        sql.CommandText = "Select 'STS', squPartNumber, squPartName, squRFQNum, CustomerName, squCustomerRFQNum, squSTSQuoteID, qstQuoteStatusDescription, ";
                        sql.CommandText += "'STS', squQuoteNumber, squQuoteVersion, squPicture, prtRFQLineNumber ";
                        sql.CommandText += "from Customer, pktblQuoteStatus, tblSTSQuote ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                        sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "where squCustomerID = CustomerID and squStatusID = qstQuoteStatusID and squRFQNum = @rfqID and (squQuoteVersion = @version ";
                        sql.CommandText += "or @version is null) and (squSTSQuoteID = @quoteID or squQuoteNumber = @quoteID or @quoteID is null) and ";
                        sql.CommandText += "(prtRFQLineNumber = @lineNum or @lineNum is null) ";
                        sql.Parameters.Clear();
                        string[] arr = quoteNumber.Split('-');

                        if (arr.Length >= 4)
                        {
                            if (quoteNumber.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@rfqID", "");
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length >= 2)
                        {
                            if (arr[1].ToLower() == "hts")
                            {
                                sql.Parameters.AddWithValue("@rfqID", "");
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length == 1)
                        {
                            sql.Parameters.AddWithValue("@rfqID", arr[0]);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                        }
                        SqlDataReader dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            string quoteNum = "";
                            if (quoteNumber.ToLower().Contains("sa") || (arr.Length == 2 && arr[1].ToLower() == "sts"))
                            {
                                if (dr["squQuoteNumber"].ToString() != "")
                                {
                                    quoteNum = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                                else
                                {
                                    quoteNum = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                            }
                            else
                            {
                                quoteNum = dr["squRFQNum"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                        dr.GetValue(7).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-STS", quoteNum, dr["squPicture"].ToString());
                        }
                        dr.Close();
                    }
                    else if (quoteNumber.ToUpper().Contains("UGS"))
                    {
                        sql.CommandText = "Select 'UGS', uquPartNumber, uquPartName, uquRFQID, CustomerName, uquCustomerRFQNumber, uquUGSQuoteID, qstQuoteStatusDescription, ";
                        sql.CommandText += "'UGS', uquQuoteNumber, uquQuoteVersion, uquPicture, prtRFQLineNumber ";
                        sql.CommandText += "from Customer, pktblQuoteStatus, tblUGSQuote ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                        sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "where uquCustomerID = CustomerID and uquStatusID = qstQuoteStatusID and uquRFQID = @rfqID and (uquQuoteVersion = @version or ";
                        sql.CommandText += "@version is null) and (uquUGSQuoteID = @quoteID or uquQuoteNumber = @quoteID or @quoteID is null) and (prtRFQLineNumber = @lineNum ";
                        sql.CommandText += "or @lineNum is null) ";
                        sql.Parameters.Clear();
                        string[] arr = quoteNumber.Split('-');

                        if (arr.Length >= 4)
                        {
                            if (quoteNumber.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@rfqID", "0");
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length >= 2)
                        {
                            if (arr[1].ToLower() == "ugs")
                            {
                                sql.Parameters.AddWithValue("@rfqID", "0");
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfqID", arr[0]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                            }
                        }
                        else if (arr.Length == 1)
                        {
                            sql.Parameters.AddWithValue("@rfqID", arr[0]);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                        }
                        SqlDataReader dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            string quoteNum = "";
                            if (quoteNumber.ToLower().Contains("sa") || (arr.Length == 2 && arr[1].ToLower() == "ugs"))
                            {
                                if (dr["uquQuoteNumber"].ToString() != "")
                                {
                                    quoteNum = dr["uquQuoteNumber"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                                }
                                else
                                {
                                    quoteNum = dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                                }
                            }
                            else
                            {
                                if (dr["uquQuoteNumber"].ToString().Contains("-"))
                                {
                                    quoteNum = dr["uquQuoteNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString();
                                }
                                else
                                {
                                    quoteNum = dr["uquRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString();
                                }
                            }
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                        dr.GetValue(7).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-UGS", quoteNum, dr["uquPicture"].ToString());
                        }
                        dr.Close();
                    }
                    



                }
                if (searchString != "" || cust.Trim() != "" || desc != "" || custRFQ != "")
                {
                    sql.CommandText = "Select TSGCompanyAbbrev, prtPartNumber, prtPartDescription, rfqID, CustomerName, rfqCustomerRFQNumber, quoQuoteID, ";
                    sql.CommandText += "prtRFQLineNumber, quoVersion, qs1.qstQuoteStatusDescription, prtPicture, prtPARTID, ptqHTS, ptqSTS, ptqUGS, hquHTSQuoteID, ";
                    sql.CommandText += "hquVersion, qs2.qstQuoteStatusDescription, squSTSQuoteID, squQuoteVersion, qs3.qstQuoteStatusDescription, uquUGSQuoteID, ";
                    sql.CommandText += "uquQuoteVersion, qs4.qstQuoteStatusDescription ";
                    sql.CommandText += "from tblRFQ, linkPartToRFQ, tblPart, Customer, linkPartToQuote ";
                    sql.CommandText += "left outer join tblQuote on ptqQuoteID = quoQuoteID and quoCreated > @start and quoCreated < @end ";
                    sql.CommandText += "and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
                    sql.CommandText += "left outer join TSGCompany on quoTSGCompanyID = TSGCompanyID ";
                    sql.CommandText += "left outer join pktblQuoteStatus as qs1 on quoStatusID = qs1.qstQuoteStatusID ";
                    sql.CommandText += "left outer join tblHTSQuote on hquHTSQuoteID = ptqQuoteID and hquCreated > @start and hquCreated < @end ";
                    sql.CommandText += "and ptqHTS = 1 and ptqSTS = 0 and ptqUGS = 0 ";
                    sql.CommandText += "left outer join pktblQuoteStatus as qs2 on hquStatusID = qs2.qstQuoteStatusID ";
                    sql.CommandText += "left outer join tblSTSQuote on squSTSQuoteID = ptqQuoteID and squCreated > @start and squCreated < @end ";
                    sql.CommandText += "and ptqHTS = 0 and ptqSTS = 1 and ptqUGS = 0 ";
                    sql.CommandText += "left outer join pktblQuoteStatus as qs3 on squStatusID = qs3.qstQuoteStatusID ";
                    sql.CommandText += "left outer join tblUGSQuote on uquUGSQuoteID = ptqQuoteID and uquCreated > @start and uquCreated < @end ";
                    sql.CommandText += "and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 1 ";
                    sql.CommandText += "left outer join pktblQuoteStatus as qs4 on uquStatusID = qs4.qstQuoteStatusID ";
                    sql.CommandText += "where rfqID = ptrRFQID and ptrPartID = prtPARTID and ptqPartID = prtPARTID and ";
                    sql.CommandText += "(prtPartNumber like @searchField or @searchField is null) and rfqCustomerID = CustomerID and ptrPartID = prtPARTID ";
                    sql.CommandText += "and rfqCustomerID = CustomerID and (rfqCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                    sql.CommandText += "and (CustomerName like @cust or @cust is null) and (@desc is null or prtPartDescription like @desc) ";
                    if (company != 9)
                    {
                        sql.CommandText += "and rfqID <> @rfqid ";
                    }
                    sql.Parameters.Clear();

                    if (searchString == "")
                    {
                        sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                    }
                    if (cust.Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@cust", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@rfqid", rfqID.Trim());
                    if (desc == "")
                    {
                        sql.Parameters.AddWithValue("@desc", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                    }
                    if (custRFQ == "")
                    {
                        sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@start", start);
                    sql.Parameters.AddWithValue("@end", end);

                    SqlDataReader dr = sql.ExecuteReader();

                    //Deals with all quotes
                    while (dr.Read())
                    {
                        if (dr.GetBoolean(12))
                        {
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(15).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                dr.GetValue(17).ToString(), "", "HTS", dr.GetValue(15).ToString() + "-HTS", dr.GetValue(3).ToString() + "-" + dr.GetValue(7).ToString() + "-HTS-" + dr.GetValue(16).ToString(), dr.GetValue(10).ToString());
                        }
                        else if (dr.GetBoolean(13))
                        {
                            if (company != 9)
                            {
                                dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(18).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                    dr.GetValue(20).ToString(), "", "STS", dr.GetValue(18).ToString() + "-STS", dr.GetValue(3).ToString() + "-" + dr.GetValue(7).ToString() + "-STS-" + dr.GetValue(19).ToString(), dr.GetValue(10).ToString());
                            }

                        }
                        else if (dr.GetBoolean(14))
                        {
                            if (company != 9)
                            {
                                dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(21).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                    dr.GetValue(23).ToString(), "", "UGS", dr.GetValue(21).ToString() + "-UGS", dr.GetValue(3).ToString() + "-" + dr.GetValue(7).ToString() + "-UGS-" + dr.GetValue(22).ToString(), dr.GetValue(10).ToString());
                            }
                        }
                        else
                        {
                            if (company != 9)
                            {
                                dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                    dr.GetValue(9).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-quo", dr.GetValue(3).ToString() + "-" + dr.GetValue(7).ToString() + "-" + dr.GetValue(0).ToString() + "-" + dr.GetValue(8).ToString(), dr.GetValue(10).ToString());
                            }
                        }
                        litHistory.Text += dr.GetValue(11).ToString();
                    }
                    dr.Close();


                    if (company != 9)
                    {
                        sql.CommandText = "Select TSGCompanyAbbrev, ecqPartNumber, ecqPartName, '', CustomerName, ecqCustomerRFQNumber, ecqECQuoteID, '', ecqVersion, qstQuoteStatusDescription, ecqPicture, ecqQuoteNumber ";
                        sql.CommandText += "from tblECQuote, TSGCompany, pktblQuoteStatus, Customer ";
                        sql.CommandText += "where (ecqPartNumber like @searchField or @searchField is null) and (ecqCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                        sql.CommandText += "and TSGCompanyID = ecqTSGCompanyID and qstQuoteStatusID = ecqStatus and ecqCreated > @start and ecqCreated < @end ";
                        sql.CommandText += "and ecqCustomer = CustomerID and (CustomerName like @cust or @cust is null) and (ecqPartName like @desc or @desc is null) ";
                        sql.Parameters.Clear();

                        if (searchString == "")
                        {
                            sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                        }
                        if (cust.Trim() == "")
                        {
                            sql.Parameters.AddWithValue("@cust", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                        }
                        sql.Parameters.AddWithValue("@rfqid", rfqID.Trim());
                        if (desc == "")
                        {
                            sql.Parameters.AddWithValue("@desc", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                        }
                        if (custRFQ == "")
                        {
                            sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                        }
                        sql.Parameters.AddWithValue("@start", start);
                        sql.Parameters.AddWithValue("@end", end);

                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr.GetValue(11).ToString() != "")
                            {
                                dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), "Stand Alone", dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                    dr.GetValue(9).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(11).ToString() + "-SA", dr.GetValue(6).ToString() + "-" + dr.GetValue(0).ToString() + "-SA-" + dr.GetValue(8).ToString(), dr.GetValue(10).ToString());
                            }
                            else
                            {
                                dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), "Stand Alone", dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                                    dr.GetValue(9).ToString(), "", dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-SA", dr.GetValue(6).ToString() + "-" + dr.GetValue(0).ToString() + "-SA-" + dr.GetValue(8).ToString(), dr.GetValue(10).ToString());
                            }
                        }
                        dr.Close();
                    }

                    sql.CommandText = "Select 'HTS', hquPartNumbers, hquPartName, '', CustomerName, hquCustomerRFQNum, hquHTSQuoteID, '', hquVersion, hquNumber, hquPicture, qstQuoteStatusDescription ";
                    sql.CommandText += "from tblHTSQuote, Customer, CustomerLocation, pktblQuoteStatus ";
                    sql.CommandText += "where hquRFQID = '' and hquCustomerID = Customer.CustomerID and hquCustomerLocationID = CustomerLocationID ";
                    sql.CommandText += "and (hquPartNumbers like @searchField or @searchField is null) and (CustomerName like @cust or @cust is null) ";
                    sql.CommandText += "and (hquPartName like @desc or @desc is null) and (hquCustomerRFQNum like @custRFQ or @custRFQ is null) ";
                    sql.CommandText += "and hquCreated > @start and hquCreated < @end and qstQuoteStatusID = hquStatusID ";
                    sql.Parameters.Clear();
                    if (searchString == "")
                    {
                        sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                    }
                    if (cust.Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@cust", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                    }
                    if (desc == "")
                    {
                        sql.Parameters.AddWithValue("@desc", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                    }
                    if (custRFQ == "")
                    {
                        sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@start", start);
                    sql.Parameters.AddWithValue("@end", end);

                    try
                    {
                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            if (dr["hquNumber"].ToString() != "")
                            {
                                dt.Rows.Add(partID, dr["hquPartNumbers"].ToString(), dr["hquPartName"].ToString(), "Stand Alone", dr["hquHTSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                    dr["hquCustomerRFQNum"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "HTS", dr["hquHTSQuoteID"].ToString() + "-HTS",
                                    dr["hquNumber"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString(), dr["hquPicture"].ToString());
                            }
                            else
                            {
                                dt.Rows.Add(partID, dr["hquPartNumbers"].ToString(), dr["hquPartName"].ToString(), "Stand Alone", dr["hquHTSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                    dr["hquCustomerRFQNum"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "HTS", dr["hquHTSQuoteID"].ToString() + "-HTS",
                                    dr["hquHTSQuoteID"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString(), dr["hquPicture"].ToString());
                            }

                        }
                        dr.Close();
                    }
                    catch (Exception er)
                    {

                    }


                    if (company != 9)
                    {
                        sql.CommandText = "Select 'STS', squPartNumber, squPartName, '', CustomerName, squCustomerRFQNum, squSTSQuoteID, '', squQuoteVersion, squQuoteNumber, squPicture, qstQuoteStatusDescription ";
                        sql.CommandText += "from tblSTSQuote, Customer, CustomerLocation, pktblQuoteStatus ";
                        sql.CommandText += "where squCustomerID = Customer.CustomerID and squPlantID = CustomerLocationID and squStatusID = qstQuoteStatusID ";
                        sql.CommandText += "and (squPartNumber like @searchField or @searchField is null) and (CustomerName like @cust or @cust is null) ";
                        sql.CommandText += "and (squPartName like @desc or @desc is null) and (squCustomerRFQNum like @custRFQ or @custRFQ is null) ";
                        sql.CommandText += "and squCreated > @start and squCreated < @end and squRFQNum = '' ";
                        sql.Parameters.Clear();
                        if (searchString == "")
                        {
                            sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                        }
                        if (cust.Trim() == "")
                        {
                            sql.Parameters.AddWithValue("@cust", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                        }
                        if (desc == "")
                        {
                            sql.Parameters.AddWithValue("@desc", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                        }
                        if (custRFQ == "")
                        {
                            sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                        }
                        sql.Parameters.AddWithValue("@start", start);
                        sql.Parameters.AddWithValue("@end", end);

                        try
                        {
                            dr = sql.ExecuteReader();
                            while (dr.Read())
                            {
                                if (dr["squQuoteNumber"].ToString() != "")
                                {
                                    dt.Rows.Add(partID, dr["squPartNumber"].ToString(), dr["squPartName"].ToString(), "Stand Alone", dr["squSTSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                        dr["squCustomerRFQNum"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "STS", dr["squSTSQuoteID"].ToString() + "-STS",
                                        dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString(), dr["squPicture"].ToString());
                                }
                                else
                                {
                                    dt.Rows.Add(partID, dr["squPartNumber"].ToString(), dr["squPartName"].ToString(), "Stand Alone", dr["squSTSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                        dr["squCustomerRFQNum"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "STS", dr["squSTSQuoteID"].ToString() + "-STS",
                                        dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString(), dr["squPicture"].ToString());
                                }
                            }
                            dr.Close();
                        }
                        catch (Exception er)
                        {

                        }

                        sql.CommandText = "Select 'UGS', uquPartNumber, uquPartName, '', CustomerName, uquCustomerRFQNumber, uquUGSQuoteID, '', uquQuoteVersion, uquQuoteNumber, uquPicture, qstQuoteStatusDescription ";
                        sql.CommandText += "from tblUGSQuote, Customer, CustomerLocation, pktblQuoteStatus ";
                        sql.CommandText += "where uquRFQID = '' and uquCustomerID = Customer.CustomerID and uquPlantID = CustomerLocationID and qstQuoteStatusID = uquStatusID ";
                        sql.CommandText += "and (uquPartNumber like @searchField or @searchField is null) and (CustomerName like @cust or @cust is null) ";
                        sql.CommandText += "and (uquPartName like @desc or @desc is null) and (uquCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                        sql.CommandText += "and uquCreated > @start and uquCreated < @end ";
                        sql.Parameters.Clear();

                        if (searchString == "")
                        {
                            sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                        }
                        if (cust.Trim() == "")
                        {
                            sql.Parameters.AddWithValue("@cust", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                        }
                        if (desc == "")
                        {
                            sql.Parameters.AddWithValue("@desc", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                        }
                        if (custRFQ == "")
                        {
                            sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                        }
                        sql.Parameters.AddWithValue("@start", start);
                        sql.Parameters.AddWithValue("@end", end);

                        try
                        {
                            dr = sql.ExecuteReader();
                            while (dr.Read())
                            {
                                if (dr["uquQuoteNumber"].ToString() != "")
                                {
                                    dt.Rows.Add(partID, dr["uquPartNumber"].ToString(), dr["uquPartName"].ToString(), "Stand Alone", dr["uquUGSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                        dr["uquCustomerRFQNumber"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "UGS", dr["uquUGSQuoteID"].ToString() + "-UGS",
                                        dr["uquQuoteNumber"].ToString() + "-UGS=SA-" + dr["uquQuoteVersion"].ToString(), dr["uquPicture"].ToString());
                                }
                                else
                                {
                                    dt.Rows.Add(partID, dr["uquPartNumber"].ToString(), dr["uquPartName"].ToString(), "Stand Alone", dr["uquUGSQuoteID"].ToString(), dr["CustomerName"].ToString(),
                                        dr["uquCustomerRFQNumber"].ToString(), dr["qstQuoteStatusDescription"].ToString(), "", "UGS", dr["uquUGSQuoteID"].ToString() + "-UGS",
                                        dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString(), dr["uquPictures"].ToString());
                                }
                            }
                            dr.Close();
                        }
                        catch (Exception er)
                        {

                        }
                    }

                    sql.CommandText = "Select TSGCompanyAbbrev, prtPartNumber, prtpartDescription, rfqID, CustomerName, rfqCustomerRFQNumber, nquNoQuoteID, prtRFQLineNumber, nquNoQuoteReasonID, prtPicture, prtPARTID ";
                    sql.CommandText += "FROM tblRFQ, linkPartToRFQ, tblPart, tblNoQuote, Customer, TSGCompany ";
                    sql.CommandText += "where rfqID = ptrRFQID and ptrPartID = prtPARTID and nquPartID = prtPARTID and ";
                    sql.CommandText += "(prtPartNumber like @searchField or @searchField is null) and rfqCustomerID = CustomerID and ptrPartID = prtPARTID ";
                    sql.CommandText += "and DATEADD(MONTH, -6, GETDATE()) < prtCreated and rfqID <> @rfqid and rfqCustomerID = CustomerID and(rfqCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                    sql.CommandText += "and(CustomerName like @cust or @cust is null) and nquCreated > @start and nquCreated < @end and(@desc is null or prtpartDescription like @desc) ";
                    sql.CommandText += "and nquCompanyID = TSGCompanyID ";
                    sql.Parameters.Clear();

                    if (searchString == "")
                    {
                        sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                    }
                    if (cust.Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@cust", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@rfqid", rfqID.Trim());
                    if (desc == "")
                    {
                        sql.Parameters.AddWithValue("@desc", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                    }
                    if (custRFQ == "")
                    {
                        sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@start", start);
                    sql.Parameters.AddWithValue("@end", end);


                    try
                    {
                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            dt.Rows.Add(partID, dr.GetValue(1).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), dr.GetValue(6).ToString(), dr.GetValue(4).ToString(), dr.GetValue(5).ToString(),
                            "", "NQ-" + dr.GetValue(8).ToString(), dr.GetValue(0).ToString(), dr.GetValue(6).ToString() + "-NQ", "", dr.GetValue(9).ToString());

                            litHistory.Text += dr.GetValue(10).ToString();
                        }
                        dr.Close();
                    }
                    catch (Exception er)
                    {

                    }


                    sql.CommandText = "Select prtPartNumber, prtpartDescription, rfqID, CustomerName, rfqCustomerRFQNumber, prtPARTID, prtPicture ";
                    sql.CommandText += "FROM tblRFQ, linkPartToRFQ, Customer, tblPart ";
                    sql.CommandText += "where rfqID = ptrRFQID and ptrPartID = prtPARTID and ";
                    sql.CommandText += "(prtPartNumber like @searchField or @searchField is null) and rfqCustomerID = CustomerID and ptrPartID = prtPARTID ";
                    sql.CommandText += "and DATEADD(MONTH, -6, GETDATE()) < prtCreated and rfqID <> @rfqid and rfqCustomerID = CustomerID and(rfqCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                    sql.CommandText += "and(CustomerName like @cust or @cust is null) and prtCreated > @start and prtCreated < @end and(@desc is null or prtpartDescription like @desc) ";
                    sql.Parameters.Clear();

                    if (searchString == "")
                    {
                        sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                    }
                    if (cust.Trim() == "")
                    {
                        sql.Parameters.AddWithValue("@cust", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@rfqid", rfqID.Trim());
                    if (desc == "")
                    {
                        sql.Parameters.AddWithValue("@desc", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                    }
                    if (custRFQ == "")
                    {
                        sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                    }
                    sql.Parameters.AddWithValue("@start", start);
                    sql.Parameters.AddWithValue("@end", end);

                    dr = sql.ExecuteReader();
                    while (dr.Read())
                    {
                        if (!litHistory.Text.Contains(dr.GetValue(5).ToString()))
                        {
                            dt.Rows.Add(partID, dr.GetValue(0).ToString(), dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), "", dr.GetValue(3).ToString(), dr.GetValue(4).ToString(), "Open Part", "", "", dr.GetValue(5).ToString() + "-part", "", dr.GetValue(6).ToString());
                        }
                    }
                    dr.Close();
                }
                

                sql.Parameters.Clear();

                //sql.CommandText = "Select top 50 qhiQuoteHistoryID, qhiGroupCompany, qhiPartNumber, qhiPartDescription, qhiRFQNumber, qhiCustomerRfqNum, qhiQuoteOrNoQuote, qhiBillToName from tblQuoteHistory  ";
                //sql.CommandText += "where(qhiPartNumber like @searchField or @searchField is null) and(qhiBillToName like @cust or @cust is null) and(qhiPartDescription like @desc or @desc is null) and ";
                //sql.CommandText += "(qhiCustomerRfqNum like @custRFQ or @custRFQ is null) and qhiDateDue > @start and qhiDateDue < @end ";
                //sql.CommandText += "order by qhiDateDue desc";

                //if (searchString == "")
                //{
                //    sql.Parameters.AddWithValue("@searchField", DBNull.Value);
                //}
                //else
                //{
                //    sql.Parameters.AddWithValue("@searchField", "%" + searchString.Trim() + "%");
                //}
                //if (cust.Trim() == "")
                //{
                //    sql.Parameters.AddWithValue("@cust", DBNull.Value);
                //}
                //else
                //{
                //    sql.Parameters.AddWithValue("@cust", "%" + cust.Trim() + "%");
                //}
                //if (desc == "")
                //{
                //    sql.Parameters.AddWithValue("@desc", DBNull.Value);
                //}
                //else
                //{
                //    sql.Parameters.AddWithValue("@desc", "%" + desc.Trim() + "%");
                //}
                //if (custRFQ == "")
                //{
                //    sql.Parameters.AddWithValue("@custRFQ", DBNull.Value);
                //}
                //else
                //{
                //    sql.Parameters.AddWithValue("@custRFQ", "%" + custRFQ.Trim() + "%");
                //}
                //sql.Parameters.AddWithValue("@start", start);
                //sql.Parameters.AddWithValue("@end", end);

                ////          0               1                   2               3           4                   5               6                   7
                ////qhiQuoteHistoryID, qhiGroupCompany, qhiPartNumber, qhiPartDescription, qhiRFQNumber, qhiCustomerRfqNum, qhiQuoteOrNoQuote, qhiBillToName
                //dr = sql.ExecuteReader();
                //while (dr.Read())
                //{
                //    if (!litHistory.Text.Contains(dr.GetValue(5).ToString()))
                //    {
                //        //              0               1                           2             3             4                           5                       6                   7                           8             9   10  11  12
                //        dt.Rows.Add(partID, dr.GetValue(2).ToString(), dr.GetValue(3).ToString(), "", dr.GetValue(4).ToString(), dr.GetValue(7).ToString(), dr.GetValue(5).ToString(), "Mass History", dr.GetValue(6).ToString(), "", dr.GetValue(0).ToString() + "-MAS", "", "");
                //    }
                //}
                //dr.Close();


                var stream = new StringWriter();
                ds.WriteXml(stream);

                litHistory.Text = stream.ToString();

                if (litHistory.Text.Length < 30)
                {
                    litHistory.Text = partID.ToString().Trim();
                }


                //sql.CommandText = "Select TSGCompanyAbbrev, ecqPartNumber, ecqPartName, ecqRFQNumber, CustomerName, ecqCustomerRFQNumber, ecqECQuoteID, '', ecqVersion, qstQuoteStatusDescription, ecqPicture, '' ";
                //sql.CommandText += "from tblECQuote, TSGCompany, pktblQuoteStatus, Customer, CustomerLocation ";
                //sql.CommandText += "where qstQuoteStatusID = qstQuoteStatusID and TSGCompanyID = ecqTSGCompanyID and CustomerID = ecqCustomer and CustomerLocationID = ecqCustomerLocation ";
                //sql.CommandText += "and(prtPartNumber like @searchField or @searchField is null) and DATEADD(MONTH, -6, GETDATE()) < ecqCreated and(ecqCustomerRFQNumber like @custRFQ or @custRFQ is null) ";
                //sql.CommandText += "and(CustomerName like @cust or @cust is null) and quoCreated > @start and quoCreated < @end and(@desc is null or prtpartDescription like @desc) ";
            }
            else
            {
                sql.Parameters.Clear();
                //                          0            1          2       3           4           5               6               7                   8               9                   10              11                      12
                sql.CommandText = "Select pthPartID, pthMass, pthQuote, pthNoQuote, pthPart, pthHistoryID, p1.prtPartNumber, p1.prtpartDescription, ptrRFQID, t1.TSGCompanyAbbrev, c1.CustomerName, r1.rfqCustomerRFQNumber, rstRFQStatusDescription, ";
                //                          13                 14                  15          16                  17              18              19              20              21              22                  23
                sql.CommandText += "t1.TSGCompanyAbbrev, qhiQuoteHistoryID, qhiSalesOrderNo, qhiPartNumber, qhiPartDescription, qhiBillToName, qhiRFQNumber, qhiCustomerRfqNum, quoQuoteID, t2.TSGCompanyAbbrev, p2.prtRFQLineNumber,  ";
                //                          24                  25              26              27                  28                  29                  30                      31              32                  33
                sql.CommandText += "p2.prtPartNumber, p2.prtpartDescription, r2.rfqID, c2.CustomerName, r2.rfqCustomerRFQNumber, t3.TSGCompanyAbbrev, p3.prtPartNumber, p3.prtpartDescription, c3.CustomerName, r3.rfqCustomerRFQNumber, ";
                //                          34            35            36                  37                  38              39              40          41            42            43              44                  45
                sql.CommandText += "p1.prtPicture, p2.prtPicture, p3.prtPicture, qs1.qstQuoteStatusDescription, r3.rfqID, nquNoQuoteReasonID, quoVersion, nquNoQuoteID, p1.prtPARTID, hquHTSQuoteID, hquVersion, qs2.qstQuoteStatusDescription, ";
                //                      46              47                          48                  49              50                          51                      52              53              54              55              56
                sql.CommandText += "squSTSQuoteID, squQuoteVersion, qs3.qstQuoteStatusDescription, uquUGSQuoteID, uquQuoteVersion, qs4.qstQuoteStatusDescription, t4.TSGCompanyAbbrev, ecqPartNumber, ecqPartName, c4.CustomerName, ecqCustomerRFQNumber, ";
                //                              57                      58           59       60        61             62            63             64              65                  66        67              68              69
                sql.CommandText += "qs5.qstQuoteStatusDescription, ecqECQuoteID, ecqVersion, pthSA, ecqPicture, hquPartNumbers, hquPartName, c5.CustomerName, hquCustomerRFQNum, hquPicture, squPartNumber, squPartName, c6.CustomerName, ";
                //                      70                 71          72               73              74              75                 76       77      78      79      80          81
                sql.CommandText += "squCustomerRFQNum, squPicture, uquPartNumber, uquPartName, c7.CustomerName, uquCustomerRFQNumber, uquPicture, pthHTS, pthSTS, pthUGS, pthSA, quoOldQuoteNumber ";
                sql.CommandText += "from linkPartToHistory ";
                sql.CommandText += "left outer join tblPart as p1 on p1.prtPARTID = pthHistoryID and pthPart = 1 ";
                sql.CommandText += "left outer join linkPartToRFQ on p1.prtPARTID = ptrPartID ";
                sql.CommandText += "left outer join tblRFQ as r1 on rfqID = ptrRFQID ";
                sql.CommandText += "left outer join Customer as c1 on r1.rfqCustomerID = CustomerID ";
                sql.CommandText += "left outer join pktblRFQStatus on r1.rfqStatus = rstRFQStatusID ";
                sql.CommandText += "left outer join linkPartReservedToCompany on prcPARTID = pthHistoryID and pthPart = 1 ";
                sql.CommandText += "left outer join TSGCompany as t1 on t1.TSGCompanyID = prcTSGCompanyID and pthPart = 1 ";
                sql.CommandText += "left outer join tblQuoteHistory on qhiQuoteHistoryID = pthHistoryID and pthMass = 1 ";
                sql.CommandText += "left outer join tblQuote on quoQuoteID = pthHistoryID and pthQuote = 1 ";
                sql.CommandText += "left outer join pktblQuoteStatus as qs1 on quoStatusID = qs1.qstQuoteStatusID and pthQuote = 1 ";
                sql.CommandText += "left outer join TSGCompany as t2 on t2.TSGCompanyID = quoTSGCompanyID and pthQuote = 1 ";
                sql.CommandText += "left outer join linkPartToQuote on pthHistoryID = ptqQuoteID and pthQuote = 1 and pthQuote = 1 and ptqHTS = pthHTS and ptqSTS = pthSTS and ptqUGS = pthUGS ";
                sql.CommandText += "left outer join tblPart as p2 on p2.prtPARTID = ptqPartID and pthQuote = 1 ";
                sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = pthHistoryID and pthQuote = 1 and qtrHTS = pthHTS and qtrSTS = pthSTS and qtrUGS = pthUGS ";
                sql.CommandText += "left outer join tblRFQ as r2 on qtrRFQID = r2.rfqID and pthQuote = 1 ";
                sql.CommandText += "left outer join Customer as c2 on r2.rfqCustomerID = c2.CustomerID and pthQuote = 1 ";
                sql.CommandText += "left outer join tblNoQuote on nquNoQuoteID = pthHistoryID and pthNoQuote = 1 ";
                sql.CommandText += "left outer join TSGCompany as t3 on nquCompanyID = t3.TSGCompanyID and pthNoQuote = 1 ";
                sql.CommandText += "left outer join tblRFQ as r3 on nquRFQID = r3.rfqID and pthNoQuote = 1 ";
                sql.CommandText += "left outer join tblPart as p3 on nquPartID = p3.prtPARTID and pthNoQuote = 1 ";
                sql.CommandText += "left outer join Customer as c3 on r3.rfqCustomerID = c3.CustomerID and pthNoQuote = 1 ";
                sql.CommandText += "left outer join tblHTSQuote on(ptqQuoteID = hquHTSQuoteID and pthQuote = 1 and ptqHTS = 1 and pthHTS = 1) or(pthHistoryID = hquHTSQuoteID and pthSA = 1 and pthHTS = 1) ";
                sql.CommandText += "left outer join pktblQuoteStatus as qs2 on hquStatusID = qs2.qstQuoteStatusID and pthQuote = 1 ";
                sql.CommandText += "left outer join tblSTSQuote on(ptqQuoteID = squSTSQuoteID and pthQuote = 1 and ptqSTS = 1 and pthSTS = 1) or(pthHistoryID = squSTSQuoteID and pthSA = 1 and pthSTS = 1) ";
                sql.CommandText += "left outer join pktblQuoteStatus as qs3 on squStatusID = qs3.qstQuoteStatusID and pthQuote = 1 ";
                sql.CommandText += "left outer join tblUGSQuote on(ptqQuoteID = uquUGSQuoteID and pthQuote = 1 and ptqUGS = 1 and pthUGS = 1) or(pthHistoryID = uquUGSQuoteID and pthSA = 1 and pthUGS = 1) ";
                sql.CommandText += "left outer join pktblQuoteStatus as qs4 on uquStatusID = qs4.qstQuoteStatusID and pthQuote = 1 ";
                sql.CommandText += "left outer join tblECQuote on ecqECQuoteID = pthHistoryID and pthSA = 1 and pthHTS <> 1 and pthSTS <> 1 and pthUGS <> 1 ";
                sql.CommandText += "left outer join TSGCompany as t4 on t4.TSGCompanyID = ecqTSGCompanyID and pthSA = 1 ";
                sql.CommandText += "left outer join Customer as c4 on c4.CustomerID = ecqCustomer and pthSA = 1 ";
                sql.CommandText += "left outer join pktblQuoteStatus as qs5 on qs5.qstQuoteStatusID = ecqStatus and pthSA = 1 ";
                sql.CommandText += "left outer join Customer as c5 on c5.CustomerID = hquCustomerID and pthSA = 1 and pthHTS = 1 ";
                sql.CommandText += "left outer join Customer as c6 on c6.CustomerID = squCustomerID and pthSA = 1 and pthSTS = 1 ";
                sql.CommandText += "left outer join Customer as c7 on c7.CustomerId = uquCustomerID and pthSA = 1 and pthUGS = 1 ";
                sql.CommandText += "where pthPartID = @partID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);

                SqlDataReader dr = sql.ExecuteReader();

                List<string> quoteids = new List<string>();

                while (dr.Read())
                {
                    if(dr.GetBoolean(1))
                    {
                        dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(16).ToString(), dr.GetValue(17).ToString(), "", dr.GetValue(15).ToString(), dr.GetValue(18).ToString(), dr.GetValue(20).ToString(), "History from MAS",
                                "", "", dr.GetValue(14).ToString() + "-MAS", dr.GetValue(19).ToString(), "");
                    }
                    else if (dr.GetBoolean(2))
                    {
                        if (quoteids.Contains(dr["quoQuoteId"].ToString())) {
                            continue;
                        }
                        quoteids.Add(dr["quoQuoteId"].ToString());
                        //Normal Quote
                        if(!dr.GetBoolean(77) && !dr.GetBoolean(78) && !dr.GetBoolean(79) && !dr.GetBoolean(80))
                        {
                            if (dr["quoOldQuoteNumber"].ToString() != "")
                            {
                                dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(24).ToString(), dr.GetValue(25).ToString(), dr.GetValue(26).ToString(), dr.GetValue(21).ToString(), dr.GetValue(27).ToString(), dr.GetValue(28).ToString(),
                                    dr.GetValue(37).ToString(), "", dr.GetValue(22).ToString(), dr.GetValue(21).ToString() + "-quo", dr["quoOldQuoteNumber"].ToString() + "-" + dr.GetValue(22).ToString() + "-" + dr.GetValue(40).ToString(), dr.GetValue(35).ToString());
                            }
                            else
                            {
                                dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(24).ToString(), dr.GetValue(25).ToString(), dr.GetValue(26).ToString(), dr.GetValue(21).ToString(), dr.GetValue(27).ToString(), dr.GetValue(28).ToString(),
                                    dr.GetValue(37).ToString(), "", dr.GetValue(22).ToString(), dr.GetValue(21).ToString() + "-quo", dr.GetValue(26).ToString() + "-" + dr.GetValue(23).ToString() + "-" + dr.GetValue(22).ToString() + "-" + dr.GetValue(40).ToString(), dr.GetValue(35).ToString());
                            }
                            
                        }
                        //HTS
                        else if (dr.GetBoolean(77))
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(24).ToString(), dr.GetValue(25).ToString(), dr.GetValue(26).ToString(), dr["hquHTSQuoteID"].ToString(), dr.GetValue(27).ToString(), dr.GetValue(28).ToString(),
                                dr.GetValue(45).ToString(), "", "HTS", dr["hquHTSQuoteID"].ToString() + "-quo", dr.GetValue(26).ToString() + "-" + dr.GetValue(23).ToString() + "-HTS-" + dr["hquVersion"].ToString(), dr.GetValue(35).ToString());
                        }
                        //STS
                        else if (dr.GetBoolean(78))
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(24).ToString(), dr.GetValue(25).ToString(), dr.GetValue(26).ToString(), dr["squSTSQuoteID"].ToString(), dr.GetValue(27).ToString(), dr.GetValue(28).ToString(),
                                dr.GetValue(48).ToString(), "", "STS", dr["squSTSQuoteID"].ToString() + "-quo", dr.GetValue(26).ToString() + "-" + dr.GetValue(23).ToString() + "-STS-" + dr["squQuoteVersion"].ToString(), dr.GetValue(35).ToString());
                        }
                        //UGS
                        else if (dr.GetBoolean(79))
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(24).ToString(), dr.GetValue(25).ToString(), dr.GetValue(26).ToString(), dr["uquUGSQuoteID"].ToString(), dr.GetValue(27).ToString(), dr.GetValue(28).ToString(),
                                dr.GetValue(51).ToString(), "", "UGS", dr["uquUGSQuoteID"].ToString() + "-quo", dr.GetValue(26).ToString() + "-" + dr.GetValue(23).ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString(), dr.GetValue(35).ToString());
                        }
                    }
                    //else if (dr.GetBoolean(3))
                    //{
                    //    dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(30).ToString(), dr.GetValue(31).ToString(), dr.GetValue(38).ToString(), "NQ - " + dr.GetValue(39).ToString(), dr.GetValue(32).ToString(), dr.GetValue(33).ToString(),
                    //        "", "NQ - " + dr.GetValue(39).ToString(), dr.GetValue(29).ToString(), dr.GetValue(41).ToString() + "-NQ", "", dr.GetValue(36).ToString());
                    //}
                    else if (dr.GetBoolean(4))
                    {
                        if (dr.GetValue(13).ToString() == "")
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(6).ToString(), dr.GetValue(7).ToString(), dr.GetValue(8).ToString(), "", dr.GetValue(10).ToString(), dr.GetValue(11).ToString(), "Open Part",
                                "", dr.GetValue(13).ToString(), dr.GetValue(42).ToString() + "-part", "", dr.GetValue(34).ToString());
                        }
                        else
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(6).ToString(), dr.GetValue(7).ToString(), dr.GetValue(8).ToString(), "", dr.GetValue(10).ToString(), dr.GetValue(11).ToString(), "Reserved",
                                "", dr.GetValue(13).ToString(), dr.GetValue(42).ToString() + "-part", "", dr.GetValue(34).ToString());
                        }
                    }
                    else if (dr.GetBoolean(60))
                    {
                        if(dr["ecqECQuoteID"].ToString() != "")
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(53).ToString(), dr.GetValue(54).ToString(), "Stand Alone", dr["ecqECQuoteID"].ToString(), dr.GetValue(55).ToString(), dr.GetValue(56).ToString(), 
                                dr.GetValue(57).ToString(), "", dr.GetValue(52).ToString(), dr["ecqECQuoteID"].ToString() + "-SA", dr["ecqECQuoteID"].ToString() + "-" + dr.GetValue(52).ToString() + "-SA-" + dr.GetValue(59).ToString(), dr.GetValue(35).ToString() );
                        }
                        else if (dr["hquHTSQuoteID"].ToString() != "")
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(62).ToString(), dr.GetValue(63).ToString(), "Stand Alone", dr["hquHTSQuoteID"].ToString(), dr.GetValue(64).ToString(), dr.GetValue(65).ToString(),
                                dr.GetValue(45).ToString(), "", "HTS", dr["hquHTSQuoteID"].ToString() + "-SA", dr["hquHTSQuoteID"].ToString() + "-HTS-SA-" + dr["hquVersion"].ToString(), dr.GetValue(35).ToString());
                        }
                        else if (dr["squSTSQuoteID"].ToString() != "")
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(67).ToString(), dr.GetValue(68).ToString(), "Stand Alone", dr["squSTSQuoteID"].ToString(), dr.GetValue(69).ToString(), dr.GetValue(70).ToString(),
                                dr.GetValue(48).ToString(), "", "STS", dr["squSTSQuoteID"].ToString() + "-SA", dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString(), dr.GetValue(71).ToString());
                        }
                        else if (dr["uquUGSQuoteID"].ToString() != "")
                        {
                            dt.Rows.Add(dr.GetValue(0).ToString(), dr.GetValue(72).ToString(), dr.GetValue(73).ToString(), "Stand Alone", dr["uquUGSQuoteID"].ToString(), dr.GetValue(74).ToString(), dr.GetValue(75).ToString(),
                                dr.GetValue(51).ToString(), "", "UGS", dr["uquUGSQuoteID"].ToString() + "-SA", dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString(), dr.GetValue(76).ToString());
                        }
                    }
                }
                dr.Close();

                //dt.Rows.Add(partID, drGetHistory.GetValue(1).ToString(), drGetHistory.GetValue(2).ToString(), drGetHistory.GetValue(3).ToString(), drGetHistory.GetValue(4).ToString(),
                //drGetHistory.GetValue(5).ToString(), drGetHistory.GetValue(6).ToString(), drGetHistory.GetValue(7).ToString(), drGetHistory.GetValue(8).ToString(), drGetHistory.GetValue(9).ToString(), drGetHistory.GetValue(10).ToString(),
                //drGetHistory.GetValue(11).ToString(), drGetHistory.GetValue(12).ToString(), drGetHistory.GetValue(13).ToString(), drGetHistory.GetValue(14).ToString(), drGetHistory.GetValue(15).ToString(), drGetHistory.GetValue(4).ToString() +
                //"-" + drGetHistory.GetValue(16).ToString() + "-" + drGetHistory.GetValue(12).ToString() + "-" + drGetHistory.GetValue(17).ToString());

                var stream = new StringWriter();
                ds.WriteXml(stream);

                litHistory.Text = stream.ToString();

                if (litHistory.Text.Length < 30)
                {
                    litHistory.Text = partID.ToString().Trim();
                }
            }
            connection.Close();
            connection2.Close();
        }
    }
}