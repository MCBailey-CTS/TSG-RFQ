using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


//THIS IS NOT BEING USED 
namespace RFQ
{
    public partial class Bundler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            List<string> rfqIDs = new List<string>();
            List<int> handling = new List<int>();

            //Finding all rfq ids that were created more than 24 hours from now with the status recieved or in progress
            //Also making sure to exclude staurday or sunday
            sql.CommandText = "Select rfqID from tblRFQ where DATEADD(HOUR, 24, rfqCreated) < GETDATE() and (rfqStatus = 2 or rfqStatus = 1) and (DATEPART(WEEKDAY, GETDATE()) <> 1 ";
            sql.CommandText += "and DATEPART(WEEKDAY, GETDATE()) <> 7) and rfqSLANotificationSent is null";
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rfqIDs.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            areAllPartsReserved(rfqIDs);

            rfqIDs.Clear();

            sql.CommandText = "Select rfqID, rfqHandlingID from tblRFQ where rfqCheckBit = 1 and (rfqStatus = 1 or rfqStatus = 2) and (DATEPART(WEEKDAY, GETDATE()) <> 1 and DATEPART(WEEKDAY, GETDATE()) <> 7) and ";
            sql.CommandText += "rfqAllPartsQuotedNotificationSent is null";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                rfqIDs.Add(dr.GetValue(0).ToString());
                handling.Add(System.Convert.ToInt32(dr.GetValue(1)));
            }
            dr.Close();

            areAllPartsQuoted(rfqIDs, handling);

            //if rfqStatus is recieved or in progress and check bit is set we want to run the bundler on the rfq
            //We are deciding on the due date by taking whichever is larger the internal due date or the customer due date
            //We want to check the due date regardless of the checkbit
            sql.CommandText = "Select rfqID, rfqHandlingID, rfqDueDate, rfqInternalDueDate from tblRFQ where (rfqStatus = 1 or rfqStatus = 2) and (rfqDueDate < DATEADD(HOUR, 24, GetDate()) or rfqInternalDueDate < DATEADD(HOUR, 24, GetDate())) ";
            sql.CommandText += "and(DATEPART(WEEKDAY, GETDATE()) <> 1 and DATEPART(WEEKDAY, GETDATE()) <> 7) and rfqDueDateNotificationSent is null";

            rfqIDs.Clear();

            dr = sql.ExecuteReader();
            handling.Clear();

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            SqlCommand sql2 = new SqlCommand();
            connection2.Open();
            sql2.Connection = connection2;

            while (dr.Read())
            {
                string rfqID = dr.GetValue(0).ToString();
                handling.Add(System.Convert.ToInt32(dr.GetValue(1)));
                DateTime dueDate;
                try
                {
                    if (System.Convert.ToDateTime(dr.GetValue(2)) > System.Convert.ToDateTime(dr.GetValue(3)))
                    {
                        dueDate = System.Convert.ToDateTime(dr.GetValue(2));
                    }
                    else
                    {
                        dueDate = System.Convert.ToDateTime(dr.GetValue(3));
                    }
                }
                catch
                {
                    dueDate = System.Convert.ToDateTime(dr.GetValue(2));
                }

                //If we have passed the due date we want to notify and update when we sent the notification
                if (DateTime.Now.ToUniversalTime().Date >= dueDate.Date)
                {
                    //if we want to send when due date is here then we set status to ready for distribution
                    if (dr.GetValue(1).ToString() == "1" || dr.GetValue(1).ToString() == "3")
                    {
                        sql2.Parameters.Clear();
                        sql2.CommandText = "Update tblRFQ set rfqDueDateNotificationSent = GETDATE(), rfqStatus = 10, rfqCheckBit = 1 where rfqID = @rfqID";
                        sql2.Parameters.AddWithValue("@rfqID", rfqID);

                        master.ExecuteNonQuery(sql2, "Bundler");
                    }
                    //We just want to notify here
                    else
                    {
                        sql2.Parameters.Clear();
                        sql2.CommandText = "Update tblRFQ set rfqDueDateNotificationSent = GETDATE(), rfqCheckBit = 0 where rfqID = @rfqID";
                        sql2.Parameters.AddWithValue("@rfqID", rfqID);

                        master.ExecuteNonQuery(sql2, "Bundler");
                        sql2.Parameters.Clear();

                        sql2.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                        sql2.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                        sql2.Parameters.AddWithValue("@rfqID", rfqID);
                        string companyList = "";
                        SqlDataReader dr2 = sql2.ExecuteReader();

                        int count = 0;
                        while (dr2.Read())
                        {
                            if (count != 0)
                            {
                                companyList += ",";
                            }
                            else
                            {

                            }
                            companyList += dr2.GetValue(2).ToString();

                            count++;
                        }
                        dr2.Close();

                        RFQ.Models.Notification notification = new Models.Notification();
                        notification.SendNotifications(companyList, rfqID, "3", master.getUserName());
                    }
                }
                //Send warning that due date is in one day
                else if (DateTime.Now.ToUniversalTime().AddDays(1).Date == dueDate.Date)
                {
                    sql2.Parameters.Clear();
                    sql2.CommandText = "Update tblRFQ set rfqDueDateNotificationSent = GETDATE(), rfqCheckBit = 0 where rfqID = @rfqID";
                    sql2.Parameters.AddWithValue("@rfqID", rfqID);

                    master.ExecuteNonQuery(sql2, "Bundler");
                    sql2.Parameters.Clear();
                    sql2.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                    sql2.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                    sql2.Parameters.AddWithValue("@rfqID", rfqID);
                    string companyList = "";
                    SqlDataReader dr2 = sql2.ExecuteReader();

                    int count = 0;
                    while (dr2.Read())
                    {
                        if (count != 0)
                        {
                            companyList += ",";
                        }
                        else
                        {

                        }
                        companyList += dr2.GetValue(2).ToString();

                        count++;
                    }
                    dr2.Close();

                    RFQ.Models.Notification notification = new Models.Notification();
                    notification.SendNotifications(companyList, rfqID, "4", master.getUserName());
                }
                else
                {
                    sql2.Parameters.Clear();
                    sql2.CommandText = "update tblRFQ set rfqCheckBit = 0 where rfqID = @rfqID";
                    sql2.Parameters.AddWithValue("@rfqID", rfqID);

                    master.ExecuteNonQuery(sql2, "Bundler");
                }
            }
            dr.Close();

            setColors();
            connection.Close();
            connection2.Close();
        }

        protected void setColors()
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "select rfqID from tblRFQ where rfqStatus = 2 or rfqStatus = 1";
            SqlDataReader dr = sql.ExecuteReader();
            List<string> rfqIDs = new List<string>();
            while (dr.Read())
            {
                rfqIDs.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            foreach (string rfq in rfqIDs)
            {
                int count = 0;

                sql.CommandText = "Select TSGCompanyAbbrev from linkRFQToCompany, TSGCompany where rtqCompanyID = TSGCompanyID and rtqRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfq);

                List<string> companies = new List<string>();
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    companies.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfq);

                dr = sql.ExecuteReader();
                List<string> partIDs = new List<string>();
                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();


                string[] colors = new string[companies.Count];
                for (int i = 0; i < companies.Count; i++)
                {
                    colors[i] = "";
                    sql.CommandText = "Select count(*) from linkPartReservedToCompany, TSGCompany where prcTSGCompanyID = TSGCompanyID and TSGCompanyAbbrev = @companyAbv and prcRFQID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfq);
                    sql.Parameters.AddWithValue("@companyAbv", companies[i]);

                    dr = sql.ExecuteReader();

                    int reserved = 0;

                    if (dr.Read())
                    {
                        reserved = System.Convert.ToInt32(dr.GetValue(0).ToString());
                    }
                    dr.Close();
                    if(reserved != 0)
                    {
                      sql.CommandText = "Select Count(distinct quoQuoteID) from linkQuoteToRFQ, tblQuote, TSGCompany where qtrRFQID = @rfqID and quoTSGCompanyID = TSGCompanyID and TSGCompanyAbbrev = @abv";
                      sql.Parameters.Clear();
                      sql.Parameters.AddWithValue("@rfqID", rfq);
                      sql.Parameters.AddWithValue("@abv", companies[i]);
                      dr = sql.ExecuteReader();

                      if (dr.Read())
                      {
                          if (reserved == System.Convert.ToInt32(dr.GetValue(0).ToString()) && reserved != 0)
                          {
                              colors[i] = "Green";
                              count++;
                          }
                          else if (System.Convert.ToInt32(dr.GetValue(0).ToString()) > 0)
                          {
                              colors[i] = "Orange";
                              count++;
                          }
                          else if (reserved != 0)
                          {
                              colors[i] = "Purple";
                              count++;
                          }
                      }
                      dr.Close();
                    }

                    if (colors[i] == "")
                    {
                        sql.CommandText = "Select Count(*) from tblNoQuote, TSGCompany where TSGCompanyID = nquCompanyID and TSGCompanyAbbrev = @abv and nquRFQID = @rfqID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@abv", companies[i]);
                        sql.Parameters.AddWithValue("@rfqID", rfq);

                        dr = sql.ExecuteReader();
                        if (dr.Read())
                        {
                            if (System.Convert.ToInt32(dr.GetValue(0).ToString()) == partIDs.Count)
                            {
                                colors[i] = "Red";
                                count++;
                            }
                        }
                        dr.Close();
                    }
                }

                //History logic We will put this back in with a table so it doesnt take forever
                for (int j = 0; j < companies.Count && count != companies.Count; j++)
                {
                    if (colors[j] == "")
                    {
                        for (int i = 0; i < partIDs.Count; i++)
                        {
                            sql.CommandText = "Select count(*) from linkPartToHistoricalQuote, tblQuoteHistory where phqPartID = @partID and phqHistoricalQuoteID = qhiQuoteHistoryID and (qhiQuoteOrNoQuote is null or qhiQuoteOrNoQuote = 'WILL QUOTE') and qhiGroupCompany = @company";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@partID", partIDs[i]);
                            sql.Parameters.AddWithValue("@company", companies[j]);

                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                if (System.Convert.ToInt32(dr.GetValue(0).ToString()) > 0)
                                {
                                    colors[j] = "blue";
                                    count++;
                                    dr.Close();
                                    break;
                                }
                            }
                            dr.Close();

                            sql.Parameters.Clear();
                            sql.CommandText = "Select Count(*) from linkPartToQuotehistory, tblQuote, TSGCompany where pqhQuoteID = quoQuoteID and TSGCompanyID = quoTSGCompanyID and pqhPartID = @partID and TSGCompanyAbbrev = @abv";
                            sql.Parameters.AddWithValue("@partID", partIDs[i]);
                            sql.Parameters.AddWithValue("@abv", companies[j]);

                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                if (System.Convert.ToInt32(dr.GetValue(0).ToString()) > 0)
                                {
                                    colors[j] = "blue";
                                    count++;
                                    dr.Close();
                                    break;
                                }
                            }
                            dr.Close();
                        }
                    }
                }



                string companiesWithColors = "";
                //Coloring and adding text
                for (int i = 0; i < companies.Count; i++)
                {
                    if (colors[i] == "")
                    {
                        companiesWithColors += "<font color='Black'>" + companies[i] + "</font><br />";
                    }
                    else
                    {
                        companiesWithColors += "<font color='" + colors[i] + "'>" + companies[i] + "</font><br />";
                    }
                }

                sql.CommandText = "Select ncoNotifiedColorID from pktblNotifiedColor where ncoRfqID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfq);
                dr = sql.ExecuteReader();
                int id = 0;
                if(dr.Read())
                {
                    id = System.Convert.ToInt32(dr.GetValue(0).ToString());
                }
                dr.Close();
                if(id != 0)
                {
                    sql.CommandText = "Update pktblNotifiedColor set ncoNotifiedColor = @text where ncoNotifiedColorID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@text", companiesWithColors);
                    sql.Parameters.AddWithValue("@id", id);
                    master.ExecuteNonQuery(sql, "Bundler");
                }
                else
                {
                    sql.CommandText = "insert into pktblNotifiedColor ( ncoNotifiedColor, ncoRfqID, ncoCreated, ncoCreatedBy) ";
                    sql.CommandText += "values (@text, @rfqID, GETDATE(), @user)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@text", companiesWithColors);
                    sql.Parameters.AddWithValue("@rfqID", rfq);
                    sql.Parameters.AddWithValue("@user", master.getUserName());
                    master.ExecuteNonQuery(sql, "Bundler");
                }
            }
            connection.Close();
        }

        protected void areAllPartsQuoted(List<string> rfqID, List<int> handling)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            for(int i = 0; i < rfqID.Count; i++)
            {
                List<string> partIDs = new List<string>();
                sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID[i]);

                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                sql.Parameters.Clear();

                Boolean allQuotes = true;

                if (partIDs.Count == 0)
                {
                    allQuotes = false;
                }

                sql.CommandText = "select Count(*) from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                sql.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID[i]);
                dr = sql.ExecuteReader();
                int companyNotifiedNumber = 0;
                if(dr.Read())
                {
                    companyNotifiedNumber = System.Convert.ToInt32(dr.GetValue(0));
                }
                dr.Close();
                if(companyNotifiedNumber == 0)
                {
                    companyNotifiedNumber = 1;
                }
                SqlConnection connection2 = new SqlConnection(master.getConnectionString());
                SqlCommand sql2 = new SqlCommand();
                connection2.Open();
                sql2.Connection = connection2;


                for (int j = 0; j < partIDs.Count(); j++)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Select Count(*) from linkPartToQuote where ptqPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDs[j]);

                    sql2.Parameters.Clear();
                    sql2.CommandText = "select count(*) from tblNoQuote where nquPartID = @partID";
                    sql2.Parameters.AddWithValue("@partID", partIDs[j]);

                    SqlDataReader dr2 = sql2.ExecuteReader();

                    dr = sql.ExecuteReader();

                    if (dr.Read())
                    {
                        Boolean noQuoted = false;
                        if(dr2.Read())
                        {
                            if (dr2.GetValue(0).ToString() == companyNotifiedNumber.ToString())
                            {
                                noQuoted = true;
                            }
                        }
                        if (dr.GetValue(0).ToString() == "0" && !noQuoted)
                        {
                            allQuotes = false;
                            dr.Close();
                            break;
                        }
                        noQuoted = false;
                    }
                    dr.Close();
                    dr2.Close();

                    sql.Parameters.Clear();
                    connection2.Close();
                }
                //If they have a quote for every part set ready for distribution and update date
                if(allQuotes)
                {
                    //If we want to send out when we have all quotes in we want to set status to ready for distribution
                    if(handling[i] == 1 || handling[i] == 2)
                    {
                        sql.CommandText = "Update tblRFQ set rfqAllPartsQuotedNotificationSent = GETDATE(), rfqStatus = 10 where rfqID = @rfqID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfqID", rfqID[i]);

                        master.ExecuteNonQuery(sql, "Bundler");
                        sql.Parameters.Clear();
                        sql.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                        sql.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                        sql.Parameters.AddWithValue("@rfqID", rfqID[i]);
                        string companyList = "";
                        dr = sql.ExecuteReader();

                        int count = 0;
                        while (dr.Read())
                        {
                            if (count != 0)
                            {
                                companyList += ",";
                            }
                            else
                            {

                            }
                            companyList += dr.GetValue(2).ToString();

                            count++;
                        }
                        dr.Close();

                        RFQ.Models.Notification notification = new Models.Notification();
                        notification.SendNotifications(companyList, rfqID[i], "6", master.getUserName());
                    }
                    //We just want to notify
                    else
                    {
                        sql.CommandText = "Update tblRFQ set rfqAllPartsQuotedNotificationSent = GETDATE(), rfqStatus = 10 where rfqID = @rfqID";
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@rfqID", rfqID[i]);

                        master.ExecuteNonQuery(sql, "Bundler");

                        sql.Parameters.Clear();
                        sql.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                        sql.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                        sql.Parameters.AddWithValue("@rfqID", rfqID[i]);
                        
                        string companyList = "";
                        dr = sql.ExecuteReader();

                        int count = 0;
                        while (dr.Read())
                        {
                            if (count != 0)
                            {
                                companyList += ",";
                            }
                            else
                            {

                            }
                            companyList += dr.GetValue(2).ToString();

                            count++;
                        }
                        dr.Close();
                        if(companyList == "")
                        {
                            companyList = "1";
                        }
                        sql.Parameters.Clear();

                        RFQ.Models.Notification notification = new Models.Notification();
                        notification.SendNotifications(companyList, rfqID[i], "6", master.getUserName());
                    }
                }
                //else
                //{
                //    sql.CommandText = "Update tblRFQ set rfqAllPartsQuotedNotificationSent = GETDATE() where rfqID = @rfqID";
                //    sql.Parameters.Clear();
                //    sql.Parameters.AddWithValue("@rfqID", rfqID[i]);
                //    master.ExecuteNonQuery(sql, "Bundler");
                //    sql.Parameters.Clear();
                //}
            }
            connection.Close();
        }



        protected void areAllPartsReserved(List<string> rfqID)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            for (int j = 0; j < rfqID.Count; j++)
            {
                //Retrieving all part ids in the rfq
                sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";
                sql.Parameters.AddWithValue("@rfqID", rfqID[j]);

                List<string> partIDs = new List<string>();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    partIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                sql.Parameters.Clear();

                List<string> reservedParts = new List<string>();
                for (int i = 0; i < partIDs.Count; i++)
                {
                    sql.Parameters.Clear();
                    //Checking to see if the part is reserved
                    sql.CommandText = "Select prcPartID from linkPartReservedToCompany where prcPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDs[i]);

                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        reservedParts.Add(dr.GetValue(0).ToString());
                    }
                    dr.Close();
                }

                for (int i = 0; i < reservedParts.Count; i++)
                {
                    sql.CommandText = "Select ppdPartID from linkPartToPartDetail where ppdPartToPartID = (Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @part)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@part", reservedParts[i]);
                    dr = sql.ExecuteReader();
                    List<string> temp = new List<string>();
                    while (dr.Read())
                    {
                        if(!reservedParts.Contains(dr.GetValue(0).ToString()))
                        {
                            reservedParts.Add(dr.GetValue(0).ToString());
                        }
                    }
                    dr.Close();
                }

                //All parts are reserved so we dont need to notify anything we just set the date so we dont check again next time it runs
                if (reservedParts.Count == partIDs.Count)
                {
                    sql.CommandText = "Update tblRFQ set rfqSLANotificationSent = GETDATE() where rfqID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqID[j]);

                    master.ExecuteNonQuery(sql, "Bundler");
                    sql.Parameters.Clear();
                }
                //We need to notify that not all parts have been sent for this rfq
                else
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Update tblRFQ set rfqSLANotificationSent = GETDATE(), rfqCheckBit = 0 where rfqID = @rfqID";
                    sql.Parameters.AddWithValue("@rfqID", rfqID[j]);

                    master.ExecuteNonQuery(sql, "Bundler");
                    sql.Parameters.Clear();

                    sql.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID = rtqCompanyID and ";
                    sql.CommandText += "rtqRFQID = @rfqID where tsgCompanyAbbrev not in ('none','TSG') and rtqCompanyID is not null order by tsgCompanyAbbrev";
                    sql.Parameters.AddWithValue("@rfqID", rfqID[j]);
                    string companyList = "";
                    dr = sql.ExecuteReader();

                    int count = 0;
                    while(dr.Read())
                    {
                        if(count != 0)
                        {
                            companyList += ",";
                        }
                        else
                        {

                        }
                        companyList += dr.GetValue(2).ToString();

                        count++;
                    }
                    dr.Close();
                    sql.Parameters.Clear();

                    RFQ.Models.Notification notification = new Models.Notification();
                    notification.SendNotifications(companyList, rfqID[j], "5", master.getUserName());
                }
            }
            connection.Close();
        }
    }
}
