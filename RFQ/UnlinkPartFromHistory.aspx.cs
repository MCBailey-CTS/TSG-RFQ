using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class UnlinkPartFromHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;



            int partId = 0;
            string quoteId = "";
            int rfqID = 0;
            if (Request["partID"] != "")
            {
                partId = System.Convert.ToInt32(Request["partID"]);
            }

            if (Request["quoteID"] != "")
            {
                quoteId = Request["quoteID"];
            }

            string test = Request["rfqID"];

            if(Request["rfqID"] != "")
            {
                rfqID = System.Convert.ToInt32(Request["rfqID"]);
            }

            if (Request["link"] == "yes")
            {
                string[] history = quoteId.Split('-');
                int nq = 0, p = 0, mas = 0, q = 0, sa = 0;
                int hts = 0;
                int sts = 0;
                int ugs = 0;
                if (history[1].Contains("NQ"))
                {
                    nq = 1;
                }
                else if (history[1].Contains("part"))
                {
                    p = 1;
                }
                else if (history[1].Contains("MAS"))
                {
                    mas = 1;
                }
                else if (history[1].Contains("quo"))
                {
                    q = 1;
                }
                else if (history[1].Contains("SA"))
                {
                    sa = 1;
                }
                else if (history[1].Contains("HTS"))
                {
                    hts = 1;
                    sql.CommandText = "Select ptqPartToQuoteID from linkPartToQuote where ptqQuoteID = @id and ptqHTS = 1 ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", history[0]);
                    SqlDataReader dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        q = 1;
                    }
                    else
                    {
                        sa = 1;
                    }
                    dr.Close();
                }
                else if (history[1].Contains("STS"))
                {
                    sts = 1;
                    sql.CommandText = "Select ptqPartToQuoteID from linkPartToQuote where ptqQuoteID = @id and ptqSTS = 1 ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", history[0]);
                    SqlDataReader dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        q = 1;
                    }
                    else
                    {
                        sa = 1;
                    }
                    dr.Close();
                }
                else if (history[1].Contains("UGS"))
                {
                    ugs = 1;
                    sql.CommandText = "Select ptqPartToQuoteID from linkPartToQuote where ptqQuoteID = @id and ptqUGS = 1 ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", history[0]);
                    SqlDataReader dr = sql.ExecuteReader();
                    if(dr.Read())
                    {
                        q = 1;
                    }
                    else
                    {
                        sa = 1;
                    }
                    dr.Close();
                }


                sql.CommandText = "if not exists (Select 1 from linkPartToHistory where pthHistoryID = @history and pthPartID = @part and pthRFQID = @rfq and pthMass = @mass and pthQuote = @quote and pthNoQuote = @nq and pthPart = @p) ";
                sql.CommandText += "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@part, @rfq, @history, @mass, @quote, @nq, @p, GETDATE(), @user, @hts, @sts, @ugs, @sa) ";
                sql.Parameters.Clear();

                sql.Parameters.AddWithValue("@part", partId);
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@history", history[0]);
                sql.Parameters.AddWithValue("@mass", mas);
                sql.Parameters.AddWithValue("@quote", q);
                sql.Parameters.AddWithValue("@nq", nq);
                sql.Parameters.AddWithValue("@p", p);
                sql.Parameters.AddWithValue("@hts", hts);
                sql.Parameters.AddWithValue("@sts", sts);
                sql.Parameters.AddWithValue("@ugs", ugs);
                sql.Parameters.AddWithValue("@sa", sa);
                sql.Parameters.AddWithValue("@user", master.getUserName());

                sql.ExecuteNonQuery();
                master.LogDatabaseTransaction(sql, "linkPartToQuoteHistory", "UnlinkPartFromHistory");
            }
            else
            {
                string[] history = quoteId.Split('-');
                int nq = 0, p = 0, mas = 0, q = 0, sa = 0;
                if (history[1].Contains("NQ"))
                {
                    nq = 1;
                }
                else if (history[1].Contains("part"))
                {
                    p = 1;
                }
                else if (history[1].Contains("MAS"))
                {
                    mas = 1;
                }
                else if (history[1].Contains("quo"))
                {
                    q = 1;
                }
                else if (history[1].Contains("SA"))
                {
                    sa = 1;
                }


                sql.CommandText = "delete from linkPartToHistory where pthPartID = @part and pthRFQID = @rfq and pthHistoryID = @history and pthMass = @mass ";
                sql.CommandText += "and pthQuote = @quote and pthNoQuote = @nq and pthPart = @p and pthSA = @sa ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", partId);
                sql.Parameters.AddWithValue("@rfq", rfqID);
                sql.Parameters.AddWithValue("@history", history[0]);
                sql.Parameters.AddWithValue("@mass", mas);
                sql.Parameters.AddWithValue("@quote", q);
                sql.Parameters.AddWithValue("@nq", nq);
                sql.Parameters.AddWithValue("@p", p);
                sql.Parameters.AddWithValue("@sa", sa);
                master.ExecuteNonQuery(sql, "UnlinkPartFromHistory");
            }
          
            connection.Close();
        }
    }
}