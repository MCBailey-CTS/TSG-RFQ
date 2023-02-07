using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class processNoQuote : System.Web.UI.Page
    {
        // This page processes Adding No Quotes, Removing No Quotes, and Adding Reservations for parts in an RFQ
        // url parameters
        // applies  = ALL or a specific part name
        protected void Page_Load(object sender, EventArgs e)
        {
            litResults.Text = "";
            String appliesTo = "";
            try
            {
                appliesTo = Request["applies"];
            }
            catch
            {

            }
            String remove = "";
            try
            {
                remove = Request["remove"];
            }
            catch
            {

            }
            String reason = "";
            try
            {
                reason = Request["reason"];
            }
            catch
            {

            }
            String rfqid = "";
            try
            {
                rfqid = Request["rfq"];
            }
            catch
            {

            }
            string reserve = "";
            try
            {
                reserve = Request["reserve"];
            }
            catch
            {

            }
            String company = "";
            try
            {
                company = Request["company"];
            }
            catch
            {
                
            }
            if (remove!="yes") {
                if (appliesTo != "")
                {
                    if (rfqid != "")
                    {
                        if (reserve != "yes")
                        {
                            if (reason != "")
                            {
                                ApplyNoQuote(rfqid, appliesTo, reason, company);
                            }
                        }
                        else
                        {
                            ApplyReservation(rfqid, appliesTo, company);
                        }
                    }
                }
            }
            else
            {
                RemoveNoQuote(rfqid, appliesTo);
            }          
        }

        protected void ApplyReservation(String rfqid, string appliesTo, string company)
        {
            Site master = new RFQ.Site();
            Double UserCompanyID = master.getCompanyId();
            // company 1 TSG cannot apply reservations
            // TODO change the zero below to a 1 once tested
            if (UserCompanyID != 0)
            {
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                if (appliesTo.ToUpper() == "ALL")
                {
                    sql.CommandText = "update linkPartToUnreserved set ptuRereserved = 1, ptuRereservedCompany = @company, ptuModified = GETDATE(), ptuModifiedBy = @user ";
                    sql.CommandText += "where ptuPartID in ( ";
                    sql.CommandText += "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID ";
                    sql.CommandText += "and ptrPartID not in (Select ptqPartID from linkQuoteToRFQ, linkPartToQuote where qtrRFQID = ptrRFQID and qtrQuoteID = ptqQuoteID) ";
                    sql.CommandText += "and ptrPartID not in (Select nquPartID from tblNoQuote where nquRFQID = ptrRFQID and(nquCompanyID = @company or nquCompanyID = 1)) ";
                    sql.CommandText += "and ptrPartID not in (Select prcPartID from linkPartReservedToCompany where prcTSGCompanyID = @company and prcRFQID = ptrRFQID) ";
                    sql.CommandText += "and ptrPartID not in (Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select top 1 ppd2.ppdPartToPartID ";
                    sql.CommandText += "from linkPartToPartDetail as ppd2 where ppd2.ppdPartID = ptrPartID) and ppdPartID <> (Select min(ppdPartID) from linkPartToPartDetail ";
                    sql.CommandText += "where ppdPartToPartID = (Select top 1 ppd2.ppdPartToPartID from linkPartToPartDetail as ppd2 where ppd2.ppdPartID = ptrPartID)))) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqid);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    if (company != null)
                    {
                        sql.Parameters.AddWithValue("@company", company);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                    }
                    master.ExecuteNonQuery(sql, "Process No QUote");

                    sql.CommandText = "insert into linkPartReservedToCompany (prcPartID, prcRFQID, prcTSGCompanyID, prcCreated, prcCreatedBy, prcModified, prcModifiedBy) ";
                    sql.CommandText += " select ptrPartID, ptrRFQID, @company, current_timestamp, @user, current_timestamp, @user ";
                    sql.CommandText += " from linkPartToRFQ ";
                    sql.CommandText += " where ptrRFQID=@rfq ";
                    sql.CommandText += " and ptrPartID not in (select ptqPartID from linkQuoteToRFQ, linkPartToQuote where qtrRFQID=@rfq and qtrQuoteID=ptqQuoteID) ";
                    sql.CommandText += " and ptrPartID not in (select nquPartID from tblNoQuote where nquRFQID=@rfq and (nquCompanyID=@company or nquCompanyID=1)) ";
                    sql.CommandText += " and ptrPartID not in (select prcPartID from linkPartReservedToCompany where prcTSGCompanyID=@company and prcRFQID=@rfq) ";
                    sql.CommandText += " and ptrPartID not in (select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select top 1 ppd2.ppdPartToPartID from linkPartToPartDetail as ppd2 where ppd2.ppdPartID = ptrPartID) and ppdPartID <> ";
                    sql.CommandText += " (select min(ppdPartID) from linkPartToPartDetail where ppdPartToPartID = (Select top 1 ppd2.ppdPartToPartID from linkPartToPartDetail as ppd2 where ppd2.ppdPartID = ptrPartID))) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    if (company != null)
                    {
                        sql.Parameters.AddWithValue("@company", company);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                    }
                    master.ExecuteNonQuery(sql,"processNoQuote");

                    Boolean linked = false;
                    sql.CommandText = "Select rtqRFQID from linkRFQToCompany where rtqCompanyID = @company and rtqRFQID = @rfq ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        linked = true;
                    }
                    dr.Close();

                    if (!linked)
                    {
                        sql.CommandText = "insert into linkRFQToCompany (rtqCompanyID, rtqRFQID, rtqCreated, rtqCreatedBy) ";
                        sql.CommandText += "values (@company, @rfq, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "Process No Quote");
                    }
                }
                else if (appliesTo.Contains("A"))
                {
                    appliesTo = appliesTo.Replace("A", "");

                    Boolean alreadyReserved = false;

                    sql.CommandText = "Select rasReserveAssemblyId from tblReserveAssembly where rasAssemblyId = @id and rasCompanyId = @company ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", appliesTo);
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        alreadyReserved = true;
                    }
                    dr.Close();

                    if (!alreadyReserved)
                    {
                        sql.CommandText = "insert into tblReserveAssembly (rasAssemblyId, rasCompanyId, rasRfqId, rasCreated, rasCreatedBy) ";
                        sql.CommandText += "values(@assembly, @company, @rfq, GETDATE(), @user)";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@assembly", appliesTo);
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "processNoQuote");
                    }

                    Boolean linked = false;
                    sql.CommandText = "Select rtqRFQID from linkRFQToCompany where rtqCompanyID = @company and rtqRFQID = @rfq ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    SqlDataReader sdr = sql.ExecuteReader();
                    if (sdr.Read())
                    {
                        linked = true;
                    }
                    sdr.Close();

                    if (!linked)
                    {
                        sql.CommandText = "insert into linkRFQToCompany (rtqCompanyID, rtqRFQID, rtqCreated, rtqCreatedBy) ";
                        sql.CommandText += "values (@company, @rfq, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "Process No Quote");
                    }
                }
                else
                {
                    sql.CommandText = "Update linkPartToUnreserved set ptuRereserved = 1, ptuRereservedCompany = @company, ptuModified = GETDATE(), ptuModifiedBy = @user where ptuPartID = @partID ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    sql.Parameters.AddWithValue("@partID", appliesTo);
                    master.ExecuteNonQuery(sql, "Process No Quote");

                    sql.CommandText = "insert into linkPartReservedToCompany (prcRFQID, prcPartID,  prcTSGCompanyID, prcCreated, prcCreatedBy, prcModified, prcModifiedBy) ";
                    sql.CommandText += " select ptrRFQID, ptrPartID, @company, current_timestamp, @user, current_timestamp, @user ";
                    sql.CommandText += " from linkPartToRFQ, tblPart ";
                    sql.CommandText += " where ptrRFQID=@rfq and ptrPartID=prtPartID ";
                    sql.CommandText += " and prtPARTID=@part ";
                    //Allow companies to reserve stuff that is already quoted
                    //sql.CommandText += " and ptrPartID not in (select ptqPartID from linkQuoteToRFQ, linkPartToQuote where qtrRFQID=@rfq and qtrQuoteID=ptqQuoteID) ";
                    sql.CommandText += " and ptrPartID not in (select nquPartID from tblNoQuote where nquRFQID=@rfq and (nquCompanyID=@company or nquCompanyID=1) ) ";
                    sql.CommandText += " and ptrPartID not in (select prcPartID from linkPartReservedToCompany where prcRFQID=@rfq and prcTSGCompanyID=@company ) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    if (company != null)
                    {
                        sql.Parameters.AddWithValue("@company", company);
                    }
                    else
                    {
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                    }
                    sql.Parameters.AddWithValue("@part", appliesTo);
                    master.ExecuteNonQuery(sql,"processNoQuote");

                    Boolean linked = false;
                    sql.CommandText = "Select rtqRFQID from linkRFQToCompany where rtqCompanyID = @company and rtqRFQID = @rfq ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@company", master.getCompanyId());
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        linked = true;
                    }
                    dr.Close();

                    if (!linked)
                    {
                        sql.CommandText = "insert into linkRFQToCompany (rtqCompanyID, rtqRFQID, rtqCreated, rtqCreatedBy) ";
                        sql.CommandText += "values (@company, @rfq, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", master.getCompanyId());
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@user", master.getUserName());
                        master.ExecuteNonQuery(sql, "Process No Quote");
                    }
                }
                connection.Close();
            }
        }

        public void ApplyNoQuote(String rfqid, String appliesTo, String reason, string company)
        {
            Site master = new RFQ.Site();
            Double UserCompanyID = 0;

            if (company == null)
            {
                UserCompanyID = master.getCompanyId();
            }
            else
            {
                UserCompanyID = System.Convert.ToDouble(company);
            }
            string user = master.getUserName();
            string uID = master.getUserID().ToString();

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (appliesTo.ToUpper() == "ALL")
            {
                List<string> quotedParts = new List<string>();
                sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ, linkPartToQuote, tblQuote where prtPartID = ptrPartID and ptrRFQID = @rfq and ptqPartID = prtPartID and quoQuoteID = ptqQuoteID and quoTSGCompanyID = @company";
                sql.Parameters.AddWithValue("@rfq", rfqid);
                sql.Parameters.AddWithValue("@company", UserCompanyID);
                SqlDataReader dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    quotedParts.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Select nquPartID from tblNoQuote where nquRFQID = @rfq and nquCompanyID = @company";
                sql.Parameters.AddWithValue("@rfq", rfqid);
                sql.Parameters.AddWithValue("@company", UserCompanyID);

                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    quotedParts.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.Parameters.Clear();
                List<string> allParts = new List<string>();
                sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ  where prtPartID = ptrPartID and ptrRFQID = @rfq and NOT EXISTS (select prcPartID from linkPartReservedToCompany where prcPartID = ptrPARTID and prcTSGCompanyID = @company) ";
                sql.CommandText += "and not exists (Select ptqPARTID from linkPartToQuote, tblQuote where ptqPartID = prtPARTID and quoQuoteID = ptqQuoteID and quoTSGCompanyID = @company)";
                sql.Parameters.AddWithValue("@rfq", rfqid);
                sql.Parameters.AddWithValue("@company", UserCompanyID);

                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    allParts.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                for (int i = 0; i < allParts.Count; i++)
                {
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@part", allParts[i]);
                    dr = sql.ExecuteReader();
                    List<string> temp = new List<string>();
                    while (dr.Read())
                    {
                        temp.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();
                    int count = 0;
                    for(int j = 0; j < temp.Count; j++)
                    {
                        if (allParts.Contains(temp[j]))
                        {
                            count++;
                        }
                    }
                    if(count != temp.Count)
                    {
                        for (int j = 0; j < temp.Count; j++)
                        {
                            try
                            {
                                allParts.Remove(temp[j]);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                
                for (int i = 0; i < allParts.Count; i++)
                {
                    if(!quotedParts.Contains(allParts[i]))
                    {
                        sql.CommandText = "insert into tblNoQuote (nquRFQID, nquPartID, nquNoQuoteReasonID, nquCompanyID, nquCreated, nquCreatedBy, nquModified, nquModifiedBy) ";
                        sql.CommandText += " Values ( @rfq, @part, @nqr, @company, GETDATE(), @user, GETDATE(), @user) ";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@user", user);
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                        sql.Parameters.AddWithValue("@nqr", reason);
                        sql.Parameters.AddWithValue("@part", allParts[i]);
                        master.ExecuteNonQuery(sql, "processNoQuote");

                        sql.CommandText = "Delete from linkPartReservedToCompany where prcRFQID = @rfq and prcTSGCompanyID = @company and prcPartID = @part";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@part", allParts[i]);
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                        master.ExecuteNonQuery(sql, "processNoQuote");

                        //We dont need to add it into the unreserved dashboard since no quoting all parts never removes for reservation
                    }
                }
            }
            else
            {
                sql.CommandText = "Select 1 from tblNoQuote where nquCompanyID = @company and nquPartID = @partID and nquRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", UserCompanyID);
                sql.Parameters.AddWithValue("@partID", appliesTo);
                sql.Parameters.AddWithValue("@rfqID", rfqid);
                SqlDataReader dr = sql.ExecuteReader();
                if(dr.Read())
                {
                    dr.Close();
                    connection.Close();
                    return;
                }
                dr.Close();

                DateTime initialReservedDate = DateTime.Now;
                sql.CommandText = "Select prcCreated from linkPartReservedToCompany where prcPartID = @partID and prcTSGCompanyID = @company ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", appliesTo);
                sql.Parameters.AddWithValue("@company", UserCompanyID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["prcCreated"].ToString() != "")
                    {
                        initialReservedDate = System.Convert.ToDateTime(dr["prcCreated"].ToString());
                    }
                }
                dr.Close();

                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", appliesTo);

                List<string> parts = new List<string>();
                dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    parts.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                if(parts.Count == 0)
                {
                    parts.Add(appliesTo);
                }
                for(int i = 0; i < parts.Count; i++)
                {
                    sql.CommandText = "insert into tblNoQuote (nquRFQID, nquPartID, nquNoQuoteReasonID, nquCompanyID, nquCreated, nquCreatedBy, nquModified, nquModifiedBy) ";
                    sql.CommandText += " Values ( @rfq, @part, @nqr, @company, GETDATE(), @user, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    sql.Parameters.AddWithValue("@company", UserCompanyID);
                    sql.Parameters.AddWithValue("@nqr", reason);
                    sql.Parameters.AddWithValue("@part", parts[i]);
                    master.ExecuteNonQuery(sql, "processNoQuote");

                    // remove any reservation for this part on this rfq
                    string reservedID = "";
                    sql.CommandText = "Select prcPartReservedToCompanyID from linkPartReservedToCompany where prcRFQID = @rfq and prcTSGCompanyID = @company and prcPartID = @part";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@part", parts[i]);
                    sql.Parameters.AddWithValue("@company", UserCompanyID);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        reservedID = dr["prcPartReservedToCompanyID"].ToString();
                    }
                    dr.Close();

                    if (reservedID != "")
                    {
                        sql.CommandText = "Delete from linkPartReservedToCompany where prcRFQID = @rfq and prcTSGCompanyID = @company and prcPartID = @part";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfq", rfqid);
                        sql.Parameters.AddWithValue("@part", parts[i]);
                        sql.Parameters.AddWithValue("@company", UserCompanyID);
                        master.ExecuteNonQuery(sql, "processNoQuote");

                        if (i == 0)
                        {
                            //GET DATE THAT THEY WERE FIRST RESERVED AND INSERT IT INTO HERE
                            sql.CommandText = "insert into linkPartToUnreserved (ptuPartID, ptuUID, ptuCompanyUnreserved, ptuRereserved, ptuInitialReservedDate, ptuCreated, ptuCreatedBy) ";
                            sql.CommandText += "values (@partID, @uid, @company, 0, @initial, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partID", parts[i]);
                            sql.Parameters.AddWithValue("@uid", uID);
                            sql.Parameters.AddWithValue("@company", UserCompanyID);
                            sql.Parameters.AddWithValue("@initial", initialReservedDate);
                            sql.Parameters.AddWithValue("@user", user);
                            master.ExecuteNonQuery(sql, "Process No Quote");
                        }
                    }



                }
            }
            connection.Close();
        }

        protected void RemoveNoQuote(String rfqid, String appliesTo)
        {
            Site master = new RFQ.Site();
            Double UserCompanyID = master.getCompanyId();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (appliesTo.ToUpper() == "ALL NO QUOTE")
            {



                sql.CommandText = "delete from tblNoQuote where nquRFQID=@rfq and nquCompanyID=@company";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", rfqid);
                sql.Parameters.AddWithValue("@company", UserCompanyID);
                master.ExecuteNonQuery(sql,"processNoQuote");
            }
            else
            {
                sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", appliesTo);

                List<string> parts = new List<string>();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    parts.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                if(parts.Count == 0)
                {
                    parts.Add(appliesTo);
                }

                for(int i = 0; i < parts.Count; i++)
                {
                    sql.CommandText = "Select nquNoQuoteID from tblNoQuote where nquRFQID=@rfq and nquCompanyID=@company and nquPartID = @part";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@company", UserCompanyID);
                    sql.Parameters.AddWithValue("@part", parts[i]);
                    SqlDataReader dr2 = sql.ExecuteReader();
                    int noQuoteID = 0;
                    if(dr2.Read())
                    {
                        noQuoteID = System.Convert.ToInt32(dr2.GetValue(0).ToString());
                    }
                    dr2.Close();

                    sql.CommandText = "delete from linkPartToOldNoQuote where onqNoQuoteID = @nqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@nqID", noQuoteID);
                    master.ExecuteNonQuery(sql, "Process No Quote");

                    sql.CommandText = "delete from tblNoQuote where nquRFQID=@rfq and nquCompanyID=@company and nquPartID in (select prtPartID from tblPart where prtPartID=@part) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfq", rfqid);
                    sql.Parameters.AddWithValue("@company", UserCompanyID);
                    sql.Parameters.AddWithValue("@part", parts[i]);
                    master.ExecuteNonQuery(sql, "processNoQuote");
                }
                
            }
            connection.Close();
        }


    }
}