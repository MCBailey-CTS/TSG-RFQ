using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class Reporting : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            // status 1 and 2 are received and in process
            sql.CommandText = "Select distinct quoQuoteID, DATEDIFF(day,coalesce(quoCreated,current_timestamp),rfqDueDate) as DaysBetween ";
            sql.CommandText += "from  tblRFQ left outer join linkQuoteToRFQ on qtrRFQID = rfqID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 ";
            sql.CommandText += "left outer join tblQuote on quoQuoteID = qtrQuoteID ";
            sql.CommandText += "where quoQuoteID is not null and quoCreated >= dateadd(month, -6, getdate()) ";
            SqlDataReader dr = sql.ExecuteReader();
            double onTime = 0, lessThanWeekLate = 0, moreThanWeekLate = 0, count = 0;
            double NoQuoted = 0;
            Decimal TotalDollars = 0;
            while(dr.Read())
            {
                if(System.Convert.ToInt32(dr.GetValue(1)) >= 0)
                {
                    onTime++;
                }
                else if (System.Convert.ToInt32(dr.GetValue(1)) > -8)
                {
                    lessThanWeekLate++;
                }
                else
                {
                    moreThanWeekLate++;
                }
                count++;
            }
            dr.Close();
            double TotalParts = 0;  
            // status 1 = received, 9 = cancelled, 4 = on hold
            // exclude those from the count
            sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where ptrRFQID=rfqid and rfqstatus not in (1, 4, 9) ";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                TotalParts = System.Convert.ToDouble(dr.GetValue(0));
            }
            dr.Close();

            // status 1 = received, 9 = cancelled, 4 = on hold
            sql.CommandText = "select count(distinct prtPartId) from tblPart where prtPartID in (select nquPartID from tblnoQuote) and prtPartID not in (select ptqpartID from linkPartToQuote) ";
            sql.CommandText += " and prtPartID not in (select prcpartID from linkPartReservedToCompany) ";
            sql.CommandText += " and prtPartID Not in (select ptrPartID from linkPartToRFQ, tblRFQ where ptrRFQID=rfqid and rfqstatus in (1, 4, 9) )";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                NoQuoted += System.Convert.ToInt32(dr.GetValue(0).ToString());
            }
            dr.Close();

            lblNoQuotePerc.Text = (NoQuoted / TotalParts * 100).ToString("0.00") + "%";
            lblOnTimePerc.Text = (onTime / count * 100).ToString("0.00") + "%";
            lbl1to7Perc.Text = (lessThanWeekLate / count * 100).ToString("0.00") + "%";
            lblMoreThanPerc.Text = (moreThanWeekLate / count * 100).ToString("0.00") + "%";

            sql.CommandText = "SELECT DATEPART(wk, quoCreated) as week, Count(*) as WeekCount, DATEPART(year, quoCreated) as newYear ";
            sql.CommandText += "FROM tblQuote ";
            sql.CommandText += "where quoCreated >= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, current_timestamp)), 0) ";
            sql.CommandText += "GROUP BY DATEPART(wk, quoCreated), DATEPART(year, quoCreated) ";
            sql.CommandText += "ORDER BY week, newYear";

            dr = sql.ExecuteReader();

            List<string> dates = new List<string>();
            List<string> year = new List<string>();

            if (dr.HasRows)
            {
                litNumOfQuotes.Text = "<script>\n";
                litNumOfQuotes.Text += "var data1 = [ ";
                int i = 0;
                while (dr.Read())
                {
                    if (i > 0)
                    {
                        litNumOfQuotes.Text += ",";
                    }
                    litNumOfQuotes.Text += "\n";
                    litNumOfQuotes.Text += "['" + dr.GetValue(0).ToString() + "', " + dr.GetValue(1).ToString() + "]";
                    dates.Add(dr.GetValue(0).ToString());
                    year.Add(dr.GetValue(2).ToString());
                    i++;
                }
                litNumOfQuotes.Text += "];\n";
                litNumOfQuotes.Text += "var data= [";
                litNumOfQuotes.Text += "{ label: 'Number of Quotes',\n";
                litNumOfQuotes.Text += "data: data1,\n";
                litNumOfQuotes.Text += "lines: { \n";
                litNumOfQuotes.Text += "show: true,\n barWidth: .9,\n align: 'left',\n numbers: {show: true, rotate: 90},\n fill: .4,\n lineWidth: 1,\n order: 1},\n color: '#0000e6' }];";
                litNumOfQuotes.Text += "var placeholder = $('#quoteNum-placeholder');\n";
                litNumOfQuotes.Text += "$('#quoteNum-title').text('# Of Quotes');\n";
                litNumOfQuotes.Text += "$.plot(placeholder, data, {xaxis: {axisLabel: 'Application', ticks: \n[";
                for (int j = 0; j < dates.Count; j++)
                {
                    if (j > 0)
                    {
                        litNumOfQuotes.Text += ",\n";
                    }
                    litNumOfQuotes.Text += "[" + System.Convert.ToInt32(dates[j]) + ", '" + FirstDateOfWeek(System.Convert.ToInt32(year[j]), System.Convert.ToInt32(dates[j])).ToShortDateString() + "']";
                }

                litNumOfQuotes.Text += "], rotateTicks: 40}, series: {lines: {show: true, \n";
                litNumOfQuotes.Text += "}}});\n";
                litNumOfQuotes.Text += "</script>\n";
            }
            dr.Close();


            sql.CommandText = "SELECT DATEPART(wk, quoCreated) as week, SUM(quoTotalAmount) as Cost, DATEPART(year, quoCreated) as newYear ";
            sql.CommandText += "FROM tblQuote ";
            sql.CommandText += "where quoCreated >= Dateadd(Month, Datediff(Month, 0, DATEADD(m, -6, current_timestamp)), 0) ";
            sql.CommandText += "GROUP BY DATEPART(year, quoCreated), DATEPART(wk, quoCreated)  ";
            sql.CommandText += "ORDER BY newYear, week";

            dr = sql.ExecuteReader();

            dates.Clear();
            year.Clear();
            if (dr.HasRows)
            {
                litPriceOfQuotes.Text = "<script>\n";
                litPriceOfQuotes.Text += "var data1 = [ ";
                int i = 0;
                while (dr.Read())
                {
                    TotalDollars += System.Convert.ToDecimal(dr.GetValue(1));
                    if (i > 0)
                    {
                        litPriceOfQuotes.Text += ",";
                    }
                    litPriceOfQuotes.Text += "\n";
                    litPriceOfQuotes.Text += "['" + dr.GetValue(0).ToString() + "', " + (System.Convert.ToDouble(dr.GetValue(1)) / 1000).ToString() + "]";
                    dates.Add(dr.GetValue(0).ToString());
                    year.Add(dr.GetValue(2).ToString());
                    i++;
                }
                litPriceOfQuotes.Text += "];\n";
                litPriceOfQuotes.Text += "var data= [";
                litPriceOfQuotes.Text += "{ label: 'Thousands',\n";
                litPriceOfQuotes.Text += "data: data1,\n";
                litPriceOfQuotes.Text += "bars: { \n";
                litPriceOfQuotes.Text += "show: true,\n barWidth: .9,\n align: 'left',\n numbers: {show: true, rotate: 90},\n fill: .4,\n lineWidth: 1,\n order: 1},\n color: '#0000e6' }];";
                litPriceOfQuotes.Text += "var placeholder = $('#quotePrice-placeholder');\n";
                litPriceOfQuotes.Text += "$('#quotePrice-title').text('$ Quoted (Thousands)');\n";
                litPriceOfQuotes.Text += "$.plot(placeholder, data, {xaxis: {axisLabel: 'Application', ticks: \n[";
                for (int j = 0; j < dates.Count; j++)
                {
                    if (j > 0)
                    {
                        litPriceOfQuotes.Text += ",\n";
                    }
                    litPriceOfQuotes.Text += "[" + System.Convert.ToInt32(dates[j]) + ", '" + FirstDateOfWeek(System.Convert.ToInt32(year[j]), System.Convert.ToInt32(dates[j])).ToShortDateString() + "']";
                }

                litPriceOfQuotes.Text += "], rotateTicks: 40}, bars: {show: true,}  \n";
                litPriceOfQuotes.Text += "});\n";
                litPriceOfQuotes.Text += "</script>\n";
            }
            dr.Close();

            //All salesman and how many quotes they have for the last 6 months
            // Doing this as a list of class in case we need to graph it.
            List<SalesManData> SalesList = new List<SalesManData>();
            sql.CommandText = "select tsgSalesman.Name, count(*) as TotalQuotes,  sum(quoTotalAmount) as TotalDollars from tblrfq, tsgsalesman, tblquote where rfqSalesman=TSGSalesmanID and rfqid = quoRFQID and quoStatusID in (1, 2, 3) ";
            sql.CommandText += " and quoCreated >= DateAdd(m, -6, current_timestamp)";
            sql.CommandText += " group by TSGSalesman.Name";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                SalesManData newSalesmanData = new SalesManData();
                newSalesmanData.Name = dr.GetValue(0).ToString();
                newSalesmanData.OpenQuotes = System.Convert.ToInt32(dr.GetValue(1));
                newSalesmanData.QuoteDollars = System.Convert.ToDecimal(dr.GetValue(2));
                newSalesmanData.PercentOfQuotes = System.Convert.ToDecimal(newSalesmanData.OpenQuotes / count * 100);
                newSalesmanData.PercentOfDollars = System.Convert.ToDecimal(newSalesmanData.QuoteDollars / TotalDollars * 100);
                SalesList.Add(newSalesmanData);
            }
            dr.Close();

            gvSalesman.DataSource = SalesList;
            gvSalesman.DataBind();

            List<SalesManQuotes> QuoteList = new List<SalesManQuotes>();

            sql.CommandText = "select tsgSalesmanID, Name from tsgsalesman order by Name ";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                SalesManQuotes newQuotes = new SalesManQuotes();
                newQuotes.Name = dr.GetValue(1).ToString().Trim();
                newQuotes.ID = System.Convert.ToInt32(dr.GetValue(0));
                QuoteList.Add(newQuotes);
            }
            dr.Close();

            foreach (SalesManQuotes Quote in QuoteList)
            {
                sql.Parameters.Clear();
                // no quotes only if the rfq has been sent to the customer and does not appear on any quote
                sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where rfqSalesman=@id and rfqid=ptrRFQID ";
                sql.CommandText += " and ptrPartID not in (select ptqpartID from linkPartToQuote) ";
                sql.CommandText += " and rfqStatus in (3, 6, 7, 8, 10, 11) ";
                sql.Parameters.AddWithValue("@id", Quote.ID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.NoQuote = System.Convert.ToInt32(dr.GetValue(0));
                }
                dr.Close();
                sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where rfqSalesman=@id and rfqid=ptrRFQID and ptrPartID in (select ptqPartID from linkPartToQuote, tblQuote where ptqQuoteID=quoQuoteID and quoStatusID=7)  ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.Win = dr.GetInt32(0);
                }
                dr.Close();
                sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where rfqSalesman=@id and rfqid=ptrRFQID and ptrPartID in (select ptqPartID from linkPartToQuote, tblQuote where ptqQuoteID=quoQuoteID and quoStatusID=8)  ";
                sql.CommandText += " and ptrPartID not in (select ptqPartID from linkPartToQuote, tblQuote where ptqQuoteID=quoQuoteID and quoStatusID=7)  ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.Loss = dr.GetInt32(0);
                }
                dr.Close();

                // I am interpreting No Status as Not Closed, Not "No Quote", and Not "Quote Obsolete"
                sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where rfqSalesman=@id and rfqid=ptrRFQID and ptrPartID in (select ptqPartID from linkPartToQuote, tblQuote where ptqQuoteID=quoQuoteID and quoStatusID < 5)  ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.NoStatus = dr.GetInt32(0);
                }
                dr.Close();

                // if reserved and not quoted
                sql.CommandText = "select count(*) from linkPartToRFQ, tblRFQ where rfqSalesman=@id and rfqid=ptrRFQID ";
                sql.CommandText += " and ptrPartID not in (select ptqPartID from linkPartToQuote)  ";
                sql.CommandText += " and ptrPartID in (select prcpartID from linkPartReservedToCompany) ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.ToBeQuoted = dr.GetInt32(0);
                }
                dr.Close();

                sql.CommandText = "select count(*) from CustomerLocation, tblECQuote where CustomerLocation.CustomerLocationID=tblECQuote.ecqCustomerlocation and  TSGSalesmanID=@id ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Quote.ECQuotes  = dr.GetInt32(0);
                }
                dr.Close();
            }
            dr.Close();

            gvSalesmanQuotes.DataSource = QuoteList;
            gvSalesmanQuotes.DataBind();

            int cntr = 0;
            litJSSalesManQuote.Text = "<script>\n";
            litJSSalesManQuote.Text += "    var smqrows = new Array();\n";
            litJSSalesManQuote.Text += "    smqrows[0] = new Array();\n";
            litJSSalesManQuote.Text += "    smqrows[0][0]='No Quote';\n";
            litJSSalesManQuote.Text += "    smqrows[0][1]='Win';\n";
            litJSSalesManQuote.Text += "    smqrows[0][2]='Loss';\n";
            litJSSalesManQuote.Text += "    smqrows[0][3]='No Status';\n";
            litJSSalesManQuote.Text += "    smqrows[0][4]='To Be Quoted';\n";
            litJSSalesManQuote.Text += "    smqrows[0][5]='E/C';\n";
            litJSSalesManQuote.Text += "    var smqticks=Array();\n";
            foreach (SalesManQuotes Quote in QuoteList)
            {
                litJSSalesManQuote.Text += "    smqticks[" + cntr + "] = '" + Quote.Name + "';\n";
                cntr++;
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "] = new Array();\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][0] = " + Quote.NoQuote + ";\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][1] = " + Quote.Win + ";\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][2] = " + Quote.Loss + ";\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][3] = " + Quote.NoStatus + ";\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][4] = " + Quote.ToBeQuoted + ";\n";
                litJSSalesManQuote.Text += "    smqrows[" + cntr + "][5] = " + Quote.ECQuotes + ";\n";
            }
            litJSSalesManQuote.Text += "    drawChart('salesmanquotes',smqrows, smqticks,'bar');\n";
            litJSSalesManQuote.Text += "</script>\n";

            List<CompanyQuotes> CompanyQuoteList = new List<CompanyQuotes>();

            sql.CommandText = "Select  TSGCompanyAbbrev,  sum(iif(DATEDIFF(day,coalesce(quoCreated,current_timestamp),rfqDueDate) <= 0,1,0)) as OnTime, sum(iif(DATEDIFF(day,coalesce(quoCreated,current_timestamp),rfqDueDate) <= 7,1,0)) as OneWeek, count(*) as ReallyLate from TSGCompany, tblRFQ left outer join linkQuoteToRFQ on qtrRFQID = rfqID left outer join tblQuote on quoQuoteID = qtrQuoteID where (rfqStatus = 2 or rfqStatus = 1) and quoTSGCompanyID=TSGCompanyID group by TSGCompanyAbbrev";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                CompanyQuotes newQuote = new CompanyQuotes();
                newQuote.Name = dr.GetValue(0).ToString();
                newQuote.OnTime = dr.GetInt32(1);
                newQuote.WithinWeek  = dr.GetInt32(2) - newQuote.OnTime;
                newQuote.ReallyLate = dr.GetInt32(3) - newQuote.WithinWeek  - newQuote.OnTime;
                CompanyQuoteList.Add(newQuote);
            }
            dr.Close();

            sql.CommandText = "Select sum(iif(DATEDIFF(day,current_timestamp,rfqDueDate) <= 0,1,0)) as OnTime, sum(iif(DATEDIFF(day,current_timestamp,rfqDueDate) <= 7,1,0)) as OneWeek, count(*) as ReallyLate from  tblRFQ, linkPartToRFQ where rfqID=ptrRfqID and  (rfqStatus = 2 or rfqStatus = 1) and ptrPartID not in (select prcPartID from linkPartReservedToCompany) ";
            sql.CommandText += " and ptrPartID Not in (select ptrPartID  from linkPartToRFQ left outer join tblCompanyNotified on ptrRFQID=cnoRFQID left outer join tblNoQuote on ptrPartID=nquPartID group by ptrPartID having count(cnoID) != count(nquPartID)) ";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                CompanyQuotes newQuote = new CompanyQuotes();
                newQuote.Name = "Un Claimed";
                newQuote.OnTime = dr.GetInt32(0);
                newQuote.WithinWeek = dr.GetInt32(1) - newQuote.OnTime;
                newQuote.ReallyLate = dr.GetInt32(2) - newQuote.WithinWeek - newQuote.OnTime;
                CompanyQuoteList.Add(newQuote);
            }
            dr.Close();

            cntr = 0;
            litJSCompanyQuote.Text = "<script>\n";
            litJSCompanyQuote.Text += "    var cqrows = new Array();\n";
            litJSCompanyQuote.Text += "    cqrows[0] = new Array();\n";
            litJSCompanyQuote.Text += "    cqrows[0][0]='On Time';\n";
            litJSCompanyQuote.Text += "    cqrows[0][1]='1-7 Days Late';\n";
            litJSCompanyQuote.Text += "    cqrows[0][2]='> 7 Days Late';\n";
            litJSCompanyQuote.Text += "    var cqticks=Array();\n";
            foreach (CompanyQuotes Quote in CompanyQuoteList)
            {
                litJSCompanyQuote.Text += "    cqticks[" + cntr + "] = '" + Quote.Name + "';\n";
                cntr++;
                litJSCompanyQuote.Text += "    cqrows[" + cntr + "] = new Array();\n";
                litJSCompanyQuote.Text += "    cqrows[" + cntr + "][0] = " + Quote.OnTime  + ";\n";
                litJSCompanyQuote.Text += "    cqrows[" + cntr + "][1] = " + Quote.WithinWeek  + ";\n";
                litJSCompanyQuote.Text += "    cqrows[" + cntr + "][2] = " + Quote.ReallyLate  + ";\n";
            }
            litJSCompanyQuote.Text += "    drawChart('companyquotes',cqrows, cqticks,'bar');\n";
            litJSCompanyQuote.Text += "</script>\n";


            Int32 YearToUse = DateTime.Now.Year;
            // 5 is obsolete, 6 is no quote
            sql.CommandText = "Select DATEPART(dayofyear,rfqDueDate) as DY, sum(iif(DATEDIFF(day,coalesce(quoCreated,current_timestamp),rfqDueDate) ";
            sql.CommandText += "<= 0,1,0)) as OnTime,  count(*) as ReallyLate, sum(iif(DATEDIFF(day,coalesce(quoCreated,current_timestamp),rfqDueDate) ";
            sql.CommandText += "<= 7,1,0)) as OneWeek ";
            sql.CommandText += "from  tblRFQ ";
            sql.CommandText += "left outer join linkQuoteToRFQ on qtrRFQID = rfqID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 ";
            sql.CommandText += "left outer join tblQuote on quoQuoteID = qtrQuoteID where rfqStatus Not In (5, 6) and DATEPART(year,rfqDueDate) = @yr ";
            sql.CommandText += "group by DatePart(dayofyear,rfqDueDate) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@yr", YearToUse);
            dr = sql.ExecuteReader();
            List<OnTimeVersusLate> otvl = new List<OnTimeVersusLate>();
            while (dr.Read())
            {
                OnTimeVersusLate newotvl = new OnTimeVersusLate();
                newotvl.DayOfYear = dr.GetInt32(0);
                newotvl.OnTime = dr.GetInt32(1);
                newotvl.Late = dr.GetInt32(2) - newotvl.OnTime - newotvl.OneWeekLate;
                newotvl.OneWeekLate = dr.GetInt32(3) - newotvl.OnTime;
                otvl.Add(newotvl);
            }
            dr.Close();

            // on time versus late is mapped out for the entire year.
            // so have to make sure there is a day 1 and a day 365
            cntr = 0;
            litJSOnTimeVLate.Text = "<script>\n";
            litJSOnTimeVLate.Text += "    var otvlrows = new Array();\n";
            litJSOnTimeVLate.Text += "    otvlrows[0] = new Array();\n";
            litJSOnTimeVLate.Text += "    otvlrows[0][0]='On Time';\n";
            litJSOnTimeVLate.Text += "    otvlrows[0][1]='Late';\n";
            litJSOnTimeVLate.Text += "    var otvlticks=Array();\n";
            while (cntr < 366)
            {
                litJSOnTimeVLate.Text += "    otvlticks[" + cntr + "] = '';\n";
                cntr++;
                litJSOnTimeVLate.Text += "    otvlrows[" + cntr + "] = new Array();\n";
                litJSOnTimeVLate.Text += "    otvlrows[" + cntr + "][0] = 0;\n";
                litJSOnTimeVLate.Text += "    otvlrows[" + cntr + "][1] = 0;\n";
            }
            litJSOnTimeVLate.Text += "    otvlticks[5] = 'Jan';\n";
            litJSOnTimeVLate.Text += "    otvlticks[35] = 'Feb';\n";
            litJSOnTimeVLate.Text += "    otvlticks[65] = 'Mar';\n";
            litJSOnTimeVLate.Text += "    otvlticks[95] = 'Apr';\n";
            litJSOnTimeVLate.Text += "    otvlticks[125] = 'May';\n";
            litJSOnTimeVLate.Text += "    otvlticks[155] = 'Jun';\n";
            litJSOnTimeVLate.Text += "    otvlticks[185] = 'Jul';\n";
            litJSOnTimeVLate.Text += "    otvlticks[215] = 'Aug';\n";
            litJSOnTimeVLate.Text += "    otvlticks[245] = 'Sep';\n";
            litJSOnTimeVLate.Text += "    otvlticks[275] = 'Oct';\n";
            litJSOnTimeVLate.Text += "    otvlticks[305] = 'Nov';\n";
            litJSOnTimeVLate.Text += "    otvlticks[340] = 'Dec';\n";
            foreach (OnTimeVersusLate listotvl in otvl)
            {
                litJSOnTimeVLate.Text += "    otvlrows[" + listotvl.DayOfYear + "][0] = " + listotvl.OnTime + ";\n";
                litJSOnTimeVLate.Text += "    otvlrows[" + listotvl.DayOfYear + "][1] = " + listotvl.Late + ";\n";
            }
            litJSOnTimeVLate.Text += "    drawChart('ontimevlate',otvlrows, otvlticks,'line');\n";
            litJSOnTimeVLate.Text += "</script>\n";


            cntr = 0;
            litJSOnTimeVLateDetail.Text = "<script>\n";
            litJSOnTimeVLateDetail.Text += "    var otvldrows = new Array();\n";
            litJSOnTimeVLateDetail.Text += "    otvldrows[0] = new Array();\n";
            litJSOnTimeVLateDetail.Text += "    otvldrows[0][0]='On Time';\n";
            litJSOnTimeVLateDetail.Text += "    otvldrows[0][1]='1-7 Days Late';\n";
            litJSOnTimeVLateDetail.Text += "    otvldrows[0][2]='>7 Days Late';\n";
            litJSOnTimeVLateDetail.Text += "    var otvldticks=Array();\n";
            while (cntr < 366)
            {
                litJSOnTimeVLateDetail.Text += "    otvldticks[" + cntr + "] = '';\n";
                cntr++;
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + cntr + "] = new Array();\n";
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + cntr + "][0] = 0;\n";
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + cntr + "][1] = 0;\n";
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + cntr + "][2] = 0;\n";
            }
            litJSOnTimeVLateDetail.Text += "    otvldticks[5] = 'Jan';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[35] = 'Feb';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[65] = 'Mar';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[95] = 'Apr';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[125] = 'May';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[155] = 'Jun';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[185] = 'Jul';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[215] = 'Aug';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[245] = 'Sep';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[275] = 'Oct';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[305] = 'Nov';\n";
            litJSOnTimeVLateDetail.Text += "    otvldticks[340] = 'Dec';\n";
            foreach (OnTimeVersusLate listotvl in otvl)
            {
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + listotvl.DayOfYear + "][0] = " + listotvl.OnTime + ";\n";
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + listotvl.DayOfYear + "][1] = " + listotvl.OneWeekLate + ";\n";
                litJSOnTimeVLateDetail.Text += "    otvldrows[" + listotvl.DayOfYear + "][2] = " + listotvl.Late + ";\n";
            }
            litJSOnTimeVLateDetail.Text += "    drawChart('ontimevlatedetail',otvldrows, otvldticks,'line');\n";
            litJSOnTimeVLateDetail.Text += "</script>\n";


            cntr = 0;
            litJSNoQuote.Text = "<script>\n";
            litJSNoQuote.Text += "    var nqrows = new Array();\n";
            litJSNoQuote.Text += "    var nqticks=Array();\n";
            litJSNoQuote.Text += "    nqrows[0] = new Array();\n";

            sql.CommandText = "select  TSGCompanyID, TSGCompanyAbbrev from  TSGCompany ";
            cntr = 0;
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                litJSNoQuote.Text += "     companyIndex" + dr.GetValue(0).ToString() + "=" + cntr + ";\n";
                litJSNoQuote.Text += "     nqrows[0][" + cntr + "]='" + dr.GetValue(1).ToString() + "';\n";
                cntr++;
            }
            dr.Close();

            Int32 holdReason = 0;
            cntr = 0;
            sql.CommandText = "select nqrNoQuoteReasonNumber, nqrNoQuoteReason, nquCompanyID, count(*)  from tblnoquote, pktblNoQuoteReason, TSGCompany where nquNoQuoteReasonID=nqrNoQuoteReasonID and nquCompanyID=tsgcompanyid and nqrActive = 1 group by  nqrNoQuoteReasonNumber, nqrNoQUoteReason, nquCompanyID order by nqrNoQuoteReasonNumber ";
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (dr.GetInt32(0) != holdReason)
                {
                    litJSNoQuote.Text += "     nqticks[" + cntr + "] = '(" + dr.GetValue(0).ToString() + ") " + dr.GetValue(1).ToString() + "';\n";
                    cntr++;
                }
                holdReason = dr.GetInt32(0);
                litJSNoQuote.Text += "     if (typeof nqrows[" + cntr + "] === 'undefined') { nqrows[" + cntr + "] = new Array(); }\n";
                litJSNoQuote.Text += "     nqrows[" + cntr + "][companyIndex" + dr.GetValue(2).ToString() + "] = " + dr.GetValue(3).ToString() + ";\n";
            }
            dr.Close();


            litJSNoQuote.Text += "    drawChart('noquote',nqrows, nqticks,'bar', 135);\n";
            litJSNoQuote.Text += "</script>\n";

            //sql.CommandText = "select tsgcompanyabbrev, quoQuoteID, rfqProgramID,  quoRFQID, ShipToName, rfqCustomerRFQNumber,  ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID,  ";
            //sql.CommandText += "rfqDateReceived, quoDueDate, prtPartNumber, prtPartDescription, prtNote, Name ";
            //sql.CommandText += "from tblquote, tsgcompany,tblrfq, oem, Program, CustomerLocation, tblPart, linkPartToRFQ, CustomerContact, linkQuoteToRFQ, linkPartToQuote ";
            //sql.CommandText += "where qtrRFQID = rfqID and qtrQuoteID = quoQuoteID and quotsgcompanyid = tsgcompanyid and tblrfq.rfqOEMID = oem.oemid and rfqProgramID = ProgramID ";
            //sql.CommandText += "and rfqPlantID = CustomerLocationID and rfqid = ptrRFQID and ptrPartID = prtPartID and rfqCustomerContact = customerContactID and ptqPartID = prtPARTID and ptqQuoteID = quoQuoteID ";
            //sql.CommandText += "Order by tsgCompanyabbrev, quoQuoteID, prtPartNumber  ";
            //SqlDataReader openDR = sql.ExecuteReader();
            //gvCompanyQuotes.DataSource = openDR;
            //gvCompanyQuotes.DataBind();
            //openDR.Close();
            //connection.Close();
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        //protected void gvCompanyQuotes_DataBound(object sender, EventArgs e)
        //{
        //    String HoldCompany="";
        //    Int32 QuoteCount = 0;
        //    GridView gv = (GridView)sender;
        //    // you have to use this offset and increment it each time you use it.
        //    // because the values you have for rowindex do not get updated as you add a row
        //    Int32 offset = 1;
        //    foreach (GridViewRow gvRow in gvCompanyQuotes.Rows) {
        //        String ThisCompany = gvRow.Cells[0].Text;
        //        try
        //        {
        //            if (ThisCompany != HoldCompany)
        //            {
        //                if (HoldCompany != "")
        //                {
        //                    GridViewRow newRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Insert);
        //                    newRow.BackColor = System.Drawing.Color.Gray;
        //                    newRow.ForeColor = System.Drawing.Color.White;
        //                    TableCell HeaderCell = new TableCell();
        //                    HeaderCell.Text = QuoteCount.ToString();
        //                    HeaderCell.ColumnSpan = 1;
        //                    HeaderCell.HorizontalAlign = HorizontalAlign.Center;
        //                    newRow.Cells.Add(HeaderCell);
        //                    gv.Controls[0].Controls.AddAt(gvRow.RowIndex + offset, newRow);
        //                    offset++;
        //                }
        //                QuoteCount = 0;
        //                HoldCompany = ThisCompany;
        //            }
        //            QuoteCount++;
        //        }
        //        catch
        //        {
        //        }
                
        //    }
        //    if (HoldCompany != "")
        //    {
        //        GridViewRow newRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Insert);
        //        newRow.BackColor = System.Drawing.Color.Gray;
        //        newRow.ForeColor = System.Drawing.Color.White;
        //        TableCell HeaderCell = new TableCell();
        //        HeaderCell.Text = QuoteCount.ToString();
        //        HeaderCell.ColumnSpan = 1;
        //        HeaderCell.HorizontalAlign = HorizontalAlign.Center;
        //        newRow.Cells.Add(HeaderCell);
        //        gv.Controls[0].Controls.AddAt(gv.Rows.Count + 1, newRow);
        //    }
            
        //}

        protected void btnRefreshReport_Click(object sender, EventArgs e)
        {

        }

    }
    public class SalesManData
    {
        public String Name { get; set; }
        public Int32 OpenQuotes { get; set; }
        public Decimal QuoteDollars { get; set; }
        public Decimal PercentOfQuotes { get; set; }
        public Decimal PercentOfDollars { get; set; }
        public Decimal AverageDieCost
        {
            get
            {
                if (this.OpenQuotes == 0)
                {
                    return 0;
                }
                else 
                {
                    return this.QuoteDollars / this.OpenQuotes;
                }
            }
        }
    }

    public class SalesManQuotes
    {
        public String Name { get; set; }
        public Int32 ID { get; set; }
        public Int32 NoQuote { get; set; }
        public Int32 Win { get; set; }
        public Int32 Loss { get; set; }
        public Int32 NoStatus { get; set;  }
        public Int32 ToBeQuoted { get; set; }
        public Int32 ECQuotes { get; set; }
    }

    public class CompanyQuotes
    {
        public String Name { get; set; }
        public Int32 OnTime { get; set; }
        public Int32 WithinWeek { get; set; }
        public Int32 ReallyLate { get; set; }
    }
    public class OnTimeVersusLate
    {
        public Int32 DayOfYear { get; set; }
        public Int32 OnTime { get; set;  }
        public Int32 OneWeekLate { get; set; }
        public Int32 Late { get; set;  }
    }

}