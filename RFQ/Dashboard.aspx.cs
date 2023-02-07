using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFQ.Models;
using System.Data.SqlClient;
using System.Drawing;

namespace RFQ
{
    public partial class Dashboard : System.Web.UI.Page
    {
        public int pageIndex
        {
            get
            {
                int retVal = 0;
                if (ViewState["pageIndex"] != null)
                {
                    retVal = System.Convert.ToInt32(ViewState["pageIndex"].ToString());
                }
                return retVal;
            }
            set { ViewState["pageIndex"] = value; }
        }
        public string sort
        {
            get
            {
                string retVal = "";
                if (ViewState["sort"] != null)
                {
                    retVal = ViewState["sort"].ToString();
                }
                return retVal;
            }
            set { ViewState["sort"] = value; }
        }
        public string order
        {
            get
            {
                string retVal = "";
                if (ViewState["order"] != null)
                {
                    retVal = ViewState["order"].ToString();
                }
                return retVal;
            }
            set { ViewState["order"] = value; }
        }
        public Boolean findClicked
        {
            get
            {
                Boolean retVal = false;
                if (ViewState["findClicked"] != null)
                {
                    retVal = System.Convert.ToBoolean(ViewState["findClicked"].ToString());
                }
                return retVal;
            }
            set { ViewState["findClicked"] = value; }
        }
        public Boolean HotList
        {
            get
            {
                Boolean retVal = false;
                if (ViewState["HotList"] != null)
                {
                    retVal = System.Convert.ToBoolean(ViewState["HotList"].ToString());
                }
                return retVal;
            }
            set { ViewState["HotList"] = value; }
        }
        public Boolean OverDue
        {
            get
            {
                Boolean retVal = false;
                if (ViewState["OverDue"] != null)
                {
                    retVal = System.Convert.ToBoolean(ViewState["OverDue"].ToString());
                }
                return retVal;
            }
            set { ViewState["OverDue"] = value; }
        }
        public List<RFQItem> MasterList = new List<RFQItem>();
        public List<TSGCompany> CompanyList = new List<TSGCompany>();
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            hlTrainingLink.NavigateUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared%20Documents/TIMS%20Training";
            hlTrainingLink.Text = "Training Manual";
            Site master = new RFQ.Site();
            if(master.getUserRole() == 3)
            {
                lblBoolean.Text = "0";
            }
            else
            {
                lblBoolean.Text = "1";
            }
            txtMessageText.Text = "Thank you for your request for quote. The attached files contain our response.";

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            if (!IsPostBack)
            {
                sort = "rfqId";
                order = "desc";
                findClicked = false;
                sql.CommandText = "Select TSGCompanyAbbrev, TSGCompanyID from TSGCompany where tcoActive = 1 ";
                SqlDataReader companyDr = sql.ExecuteReader();
                ddlCompany.DataSource = companyDr;
                ddlCompany.DataTextField = "TSGCompanyAbbrev";
                ddlCompany.DataValueField = "TSGCompanyId";
                ddlCompany.DataBind();
                companyDr.Close();
                if (master.getUserName() == "jtgrotenrath@toolingsystemsgroup.com")
                {
                    ddlCompany.SelectedValue = "TSG";
                }
                else
                {
                    ddlCompany.SelectedValue = master.getCompanyId().ToString();
                }
                


                sql.CommandText = "select name from TSGSalesman order by Name";
                SqlDataReader salesman = sql.ExecuteReader();
                ddlSalesman.DataSource = salesman;
                ddlSalesman.DataValueField = "name";
                ddlSalesman.DataTextField = "name";
                ddlSalesman.DataBind();
                ddlSalesman.Items.Insert(0, new ListItem("Any", ""));
                ddlSalesman.SelectedIndex = 0;
                salesman.Close();

                string name = master.getUserName();

                sql.CommandText = "Select name from TSGSalesman where Email = @name and Email <> 'jmomber@toolingsystemsgroup.com' and Email <> 'kberry@toolingsystemsgroup.com' and Email <> 'jtgrotenrath@toolingsystemsgroup.com' and Email <> 'tbaker@toolingsystemsgroup.com'";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@name", name);
                salesman = sql.ExecuteReader();
                if(salesman.Read())
                {
                    ddlSalesman.SelectedValue = salesman.GetValue(0).ToString();
                }
                salesman.Close();

                sql.CommandText = "select nqrNoQuoteReasonID, nqrNoQuoteReason, nqrNoQuoteReasonNumber from pktblNoQuoteReason where nqrActive = 1 order by nqrNoQuoteReasonNumber";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    ddlNoQuoteReason.Items.Add(new System.Web.UI.WebControls.ListItem((dr.GetValue(2).ToString() + " - " + dr.GetValue(1).ToString()), dr.GetValue(0).ToString()));
                }
                dr.Close();

                sql.CommandText = "Select OEMID, OEMName from OEM order by OEMName";
                SqlDataReader pm = sql.ExecuteReader();
                ddlOEM.DataSource = pm;
                ddlOEM.DataValueField = "OEMID";
                ddlOEM.DataTextField = "OEMName";
                ddlOEM.DataBind();
                ddlOEM.Items.Insert(0, new ListItem("Any", "0"));
                ddlOEM.SelectedIndex = 0;
                pm.Close();

                sql.CommandText = "Select qtyQuoteType, qtyQuoteTypeID from pktblQuoteType order by qtyQuoteType";
                SqlDataReader qt = sql.ExecuteReader();
                ddlQuoteType.DataSource = qt;
                ddlQuoteType.DataValueField = "qtyQuoteTypeID";
                ddlQuoteType.DataTextField = "qtyQuoteType";
                ddlQuoteType.DataBind();
                ddlQuoteType.Items.Insert(0, new ListItem("Any", ""));
                ddlQuoteType.SelectedIndex = 0;
                qt.Close();

                sql.CommandText = "select rstRFQStatusDescription, rstRFQStatusID from pktblRFQStatus order by rstRFQStatus ";
                SqlDataReader st = sql.ExecuteReader();
                ddlStatus.DataSource = st;
                ddlStatus.DataValueField = "rstRFQStatusID";
                ddlStatus.DataTextField = "rstRFQStatusDescription";
                ddlStatus.DataBind();
                st.Close();
                ddlStatus.Items.Insert(0, new ListItem("All", ""));
                ddlStatus.SelectedIndex = 0;

                //ddlOrderBy.Items.Add("RFQ ID");
                //ddlOrderBy.Items.Add("Customer Name");

                //ddlSortBy.Items.Add("Ascending");
                //ddlSortBy.Items.Add("Descending");
                //ddlSortBy.SelectedValue = "Descending";

                sql.CommandText = "select fltFilterID, fltFilterName from tblFilter where fltUID is null or fltUID=@user";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@user", master.getUserID());
                SqlDataReader filt = sql.ExecuteReader();
                ddlFilter.DataSource = filt;
                ddlFilter.DataTextField = "fltFilterName";
                ddlFilter.DataValueField = "fltFilterID";
                ddlFilter.DataBind();
                ddlFilter.Items.Insert(0, new ListItem("No Filter", "0"));
                ddlFilter.SelectedIndex = 0;
                filt.Close();
                sql.CommandText = "select perDefaultFilterID from Permissions where UID=@user";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@user", master.getUserID());
                SqlDataReader filt2 = sql.ExecuteReader();
                if (filt2.Read())
                {
                    // try in case either null -or- the filter they set as default has been deleted.
                    try
                    {
                        ddlFilter.SelectedValue = filt2.GetValue(0).ToString();
                    }
                    catch
                    {

                    }
                }
                refreshPage();
            }
            connection.Close();
        }

        protected void refreshPage()
        {
            if (ddlFilter.SelectedValue != "0")
            {
                dgResults.Visible = false;
                populateFilter();
                litResults.Visible = true;
            }
            else
            {
                litResults.Visible = false;
                populateList();
                List<RFQItem> RFQList = new List<RFQItem>();
                foreach (RFQItem item in MasterList)
                {
                    if((!IsPostBack || !findClicked) && ddlFilter.SelectedIndex == 0)
                    {
                        if (ddlSalesman.SelectedValue.Equals("") || item.salesman.Contains(ddlSalesman.SelectedValue))
                        {
                            if ((ddlCompany.SelectedValue == "1") || (!item.notified.Contains("<font color='Red'>" + ddlCompany.SelectedItem.Text) && !item.notified.Contains("<font color='Green'>" + ddlCompany.SelectedItem.Text) && item.notified.Contains(ddlCompany.SelectedItem.Text)))
                            {
                                RFQList.Add(item);
                            }
                        }
                    }
                    else if (HotList)
                    {
                        if ((ddlCompany.SelectedValue == "1") || (item.notified.Contains(ddlCompany.SelectedItem.Text)))
                        {
                            if (item.status == "RFQ In Process")
                            {
                                try
                                {
                                    if (System.Convert.ToDateTime(item.date_due) < DateTime.Now.AddDays(7).ToUniversalTime())
                                    {
                                        RFQList.Add(item);
                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else
                    {
                        if (OverDue)
                        {
                            if ((ddlCompany.SelectedValue == "1") || (item.notified.Contains(ddlCompany.SelectedItem.Text)))
                            {
                                if ((item.status == "RFQ Received") || (item.status == "RFQ In Process"))
                                {
                                    try
                                    {
                                        if (System.Convert.ToDateTime(item.date_due) <= DateTime.Now)
                                        {
                                            RFQList.Add(item);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((ddlCompany.SelectedValue == "1") || (item.notified.Contains(ddlCompany.SelectedItem.Text)))
                            {
                                if ((ddlStatus.SelectedItem.ToString() == "All") || (item.status == ddlStatus.SelectedItem.ToString()))
                                {
                                    if ((txtCustomer.Text.Trim() == "") || (item.customer.ToUpper().Contains(txtCustomer.Text.ToUpper().Trim())))
                                    {
                                        if ((txtRFQID.Text.Trim() == "") || (item.rfqid.ToUpper().Equals(txtRFQID.Text.ToUpper().Trim())))
                                        {
                                            if(txtCustomerRFQID.Text.Trim() == "" || item.customer_rfq.ToUpper().Contains(txtCustomerRFQID.Text.ToUpper().Trim()))
                                            {
                                                if(ddlSalesman.SelectedValue.Equals("") || item.salesman.Contains(ddlSalesman.SelectedValue))
                                                {
                                                    if(ddlOEM.SelectedValue == "0" || ddlOEM.SelectedValue == item.oem)
                                                    {
                                                        if ((ddlCompany.SelectedValue == "1") || (((item.notified.Contains("<font color='Black'>" + ddlCompany.SelectedItem.Text)) && chkBlack.Checked) || ((item.notified.Contains("<font color='blue'>" + ddlCompany.SelectedItem.Text)) && chkBlue.Checked) ||
                                                            ((item.notified.Contains("<font color='Red'>" + ddlCompany.SelectedItem.Text)) && chkRed.Checked) || ((item.notified.Contains("<font color='Aqua'>" + ddlCompany.SelectedItem.Text)) && chkAqua.Checked) ||
                                                            ((item.notified.Contains("<font color='Orange'>" + ddlCompany.SelectedItem.Text)) && chkOrange.Checked) || ((item.notified.Contains("<font color='Green'>" + ddlCompany.SelectedItem.Text)) && chkGreen.Checked)))
                                                        {
                                                            RFQList.Add(item);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                dgResults.DataSource = RFQList;
                dgResults.DataBind();
                dgResults.Visible = true;
                paintDataGridText();
            }
        }

        protected void paintDataGridText()
        {
            //Site master = new Site();
            //SqlConnection connection = new SqlConnection(master.getConnectionString());
            //connection.Open();
            //SqlCommand sql = new SqlCommand();
            //sql.Connection = connection;
            //sql.Parameters.Clear();
            //foreach (GridViewRow row in dgResults.Rows)
            //{
            //    sql.CommandText = "Select ncoNotifiedColor from pktblNotifiedColor where ncoRFQID = @rfqID";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@rfqID", ((HyperLink) row.Cells[0].Controls[0]).Text);
            //    SqlDataReader dr = sql.ExecuteReader();

            //    if (dr.Read())
            //    {
            //        row.Cells[6].Text = dr.GetValue(0).ToString();
            //    }
            //    dr.Close();
            //}
            //connection.Close();
        }
    
        protected void OnPaging(object sender, GridViewPageEventArgs e)
        {
            pageIndex = e.NewPageIndex;
            dgResults.PageIndex = pageIndex;
            refreshPage();
        }

        protected void OnSort(object sender, GridViewSortEventArgs e)
        {
            if (sort == e.SortExpression)
            {
                if (order == "asc")
                {
                    order = "desc";
                }
                else
                {
                    order = "asc";
                }
            }
            else
            {
                order = "asc";
            }
            sort = e.SortExpression;
            dgResults.PageIndex = pageIndex;
            refreshPage();
        }

        protected void populateFilter()
        {
            Boolean matchAll = false;
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.Parameters.Clear();
            sql.CommandText = "select fltMatchAll from tblFilter where fltFilterID=@filter";
            sql.Parameters.AddWithValue("@filter", ddlFilter.SelectedValue);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                matchAll = System.Convert.ToBoolean(dr.GetValue(0));
            }
            dr.Close();
            sql.Parameters.Clear();
            String FilterSQL = "select rfqid";
            // one space for padding when append to rest of query
            String ExtraTables = "";
            List<String> ExtraTableList = new List<string>();
            String ExtraJoins = "";
            String HeaderText = "<table cellspacing='0' rules='all' border='1'  style='border-collapse:collapse;'><thead><tr class='ui-widget-content'><th>Select</th>";
            sql.CommandText = "select ffiFieldName, sfnTableName, sfnFieldName, sfnFieldtype, sfnLookupTable, sfnLookupField, sfnReturnField from pktblfilterfield, pktblSystemFieldName where ffiFieldName=sfnDisplayName  and ffiFilterID=@filter order by ffiDisplaySequence";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@filter", ddlFilter.SelectedValue);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                String DisplayName = dr.GetValue(0).ToString();
                String TableName = dr.GetValue(1).ToString();
                String FieldName = dr.GetValue(2).ToString();
                String FieldType = dr.GetValue(3).ToString();
                String LookupTable = dr.GetValue(4).ToString();
                String LookupField = dr.GetValue(5).ToString();
                String ReturnField = dr.GetValue(6).ToString();
                if (LookupTable != "")
                {
                    if (ReturnField.Contains(","))
                    {
                        String FinalName = "";
                        FilterSQL += ", CONCAT(''";
                        foreach (String FieldPart in ReturnField.Split(','))
                        {
                            FinalName = FieldPart;
                            FilterSQL += "," + LookupTable + "." + FieldPart + ",' '";
                        }
                        FilterSQL += ") as " + FinalName;
                    }
                    else
                    {
                        FilterSQL += ", " + LookupTable + "." + ReturnField;
                    }
                    // Check if I have already added this table.
                    // Needed to use a list because of tables such as Customer and CustomerLocation
                    if (! ExtraTableList.Contains(LookupTable))
                    {
                        ExtraTables += ", " + LookupTable;
                        ExtraTableList.Add(LookupTable);
                    }
                    ExtraJoins += " AND " + TableName + "." + FieldName + "=" + LookupTable + "." + LookupField;
                }
                else
                {
                    FilterSQL += ", " + TableName + "." + FieldName;
                }
                HeaderText += "<th>" + DisplayName + "</th>";
            }
            dr.Close();
            FilterSQL += " FROM tblRFQ ";
            FilterSQL += ExtraTables;
            // just here to make the extra joins work
            FilterSQL += " WHERE 1 > 0 ";
            FilterSQL += ExtraJoins;
            sql.Parameters.Clear();
            sql.CommandText = "select fcnOperation, fcnCondition, sfnTableName, sfnFieldName, sfnFieldtype from pktblfiltercondition, pktblSystemFieldName where fcnFieldName=sfnDisplayName and fcnFilterID=@filter";
            sql.Parameters.AddWithValue("@filter", ddlFilter.SelectedValue);
            dr = sql.ExecuteReader();
            HeaderText += "</tr></thead><tbody>";
            litResults.Text = HeaderText;
            String ConditionText = "";
            while (dr.Read())
            {
                if (ConditionText != "")
                {
                    ConditionText += " OR ";
                }
                String Operation = dr.GetValue(0).ToString();
                String Condition = dr.GetValue(1).ToString();
                String TableName = dr.GetValue(2).ToString();
                String FieldName = dr.GetValue(3).ToString();
                String FieldType = dr.GetValue(4).ToString();
                ConditionText += TableName + "." + FieldName + " ";
                if (Operation == "eq")
                {
                    ConditionText += "=";
                }
                if (Operation == "ne")
                {
                    ConditionText += "<>";
                }
                if (Operation == "ge")
                {
                    ConditionText += ">=";
                }
                if (Operation == "le")
                {
                    ConditionText += "<=";
                }
                if ((Operation == "contains") || (Operation == "starts") || (Operation == "ends")) {
                    ConditionText += " like ";
                    if (Operation == "contains")
                    {
                        ConditionText += "'%" + Condition + "%'";
                    }
                    if (Operation == "starts")
                    {
                        ConditionText += "'" + Condition + "%'";
                    }
                    if (Operation == "ends")
                    {
                        ConditionText += "'%" + Condition + "'";
                    }
                }
                else 
                {
                    if (FieldType != "N")
                    {
                        ConditionText += "'" + Condition + "'";
                    }
                    else
                    {
                        ConditionText += Condition;
                    }
                }
            }
            if (ConditionText != "")
            {
                FilterSQL += " AND (" + ConditionText + ") ";
            }
            dr.Close();
            sql.CommandText = FilterSQL;
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                litResults.Text += "<tr>";
                litResults.Text += "<td valign='top'>";
                litResults.Text += "<a class='mybutton' href='editRFQ.aspx?id=" + dr.GetValue(0).ToString() + "' target='_blank'>Select</a>";
                litResults.Text += "</td>";
                Int32 i = 1;
                while (i < dr.FieldCount )  
                {
                    litResults.Text += "<td valign='top'>";
                    litResults.Text += dr.GetValue(i).ToString();
                    litResults.Text += "</td>";
                    i++;
                }
                litResults.Text += "</tr>";
            }
            dr.Close();
            litResults.Text += "</tbody></table>";            
            connection.Close();
        }


        protected void populateList()
        {
            // create master list of RFQs
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            SqlCommand sql2 = new SqlCommand();
            sql2.Connection = connection2;

            sql.Parameters.Clear();
            sql.CommandText = "select rfqID, rstRFQStatusDescription, rfqCustomerRFQNumber, rfqDueDate, CustomerName, CustomerRank.Rank,  (Select concat(ps.Name, '<br>', ss.Name) from CustomerLocation cl join TSGSalesman ps on ps.TSGSalesmanID = cl.TSGSalesmanID  ";
            sql.CommandText += "left outer join linkSalesmanToCustomerLocation lstcl on cl.CustomerLocationID = lstcl.sclCustomerLocationId left outer join TSGSalesman ss on ss.TSGSalesmanID = lstcl.sclSalesmanId where cl.CustomerLocationID = CustomerLocation.CustomerLocationID), ";
            sql.CommandText += "rfqLiveWork, ShipToName, (select count(*) from linkPartToRFQ where ptrRFQID = rfqID) as numOfParts, ";
            sql.CommandText += "(select count(*) from linkPartToQuote, linkQuoteToRFQ where qtrRFQID = rfqID and ptqQuoteID = qtrQuoteID and ptqHTS = qtrHTS and ptqSTS = qtrSTS and ptqUGS = qtrUGS) as numPartsQuoted, ";
            sql.CommandText += "((Select count(distinct ppdPartID) from linkPartReservedToCompany, linkPartToPartDetail where prcRFQID = rfqID and ppdPartToPartID = (Select distinct top 1 ppdPartToPartID from linkPartToPartDetail where ppdPartID = prcPartID))  ";
            sql.CommandText += "+ (select count(distinct prcPartID) from linkPartReservedToCompany where prcRFQID = rfqID and prcPartID not in (Select ppdPartID from linkPartToPartDetail where ppdPartID = prcPartID))) as total, ncoNotifiedColor, rfqOEMID, ";
            sql.CommandText += "(select top 1 1 from tblSTSPartInfo where spiRFQID = rfqID and spiAnnualVolume <> '' and spiProductionDaysPerYear <> '' and spiShiftsPerDay <> '' and spiOEE <> '' and spiOEE <> '') as STSRFQInfo ";
            sql.CommandText += "from Customer, pktblRFQStatus, linkPartToRFQ, tblRFQ ";
            sql.CommandText += "left outer join CustomerLocation on rfqPlantID = CustomerLocationID ";
            sql.CommandText += "left outer join TSGSalesman ps on CustomerLocation.TSGSalesmanID = ps.TSGSalesmanID  ";
            sql.CommandText += "left outer join linkSalesmanToCustomerLocation scl on scl.sclCustomerLocationId = CustomerLocationID ";
            sql.CommandText += "left outer join TSGSalesman ss on ss.TSGSalesmanID = scl.sclSalesmanId ";
            sql.CommandText += "left outer join CustomerRank on CustomerLocation.CustomerRankID = CustomerRank.CustomerRankID ";
            sql.CommandText += "left outer join pktblNotifiedColor on ncoRFQID = rfqID ";
            sql.CommandText += "where rfqCustomerID = Customer.CustomerID and rfqStatus = rstRFQStatusID and ptrRFQID = rfqID ";
            if((!IsPostBack || !findClicked) && ddlFilter.SelectedIndex == 0)
            {
                sql.CommandText += "and (rfqStatus = 2 or rfqStatus = 1 or rfqStatus = 12) ";
            }
            if (ddlStatus.SelectedItem.ToString() != "All")
            {
                sql.CommandText += "and rfqStatus = @status ";
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
            }
            if(txtRFQID.Text.Trim() != "")
            {
                sql.CommandText += "and rfqID = @rfqID ";
                sql.Parameters.AddWithValue("@rfqID", txtRFQID.Text);
            }
            if (txtCustomer.Text.Trim() != "")
            {
                sql.CommandText += "and CustomerName like @customer ";
                sql.Parameters.AddWithValue("@customer", "%" + txtCustomer.Text.Trim() + "%");
            }
            sql.CommandText += "group by rfqID, rstRFQStatusDescription, rfqCustomerRFQNumber, rfqDueDate, CustomerName, CustomerRank.Rank, ps.Name, ss.Name, rfqLiveWork, ShipToName, ncoNotifiedColor, rfqOEMID, CustomerLocation.CustomerLocationID ";
            sql.CommandText += "order by " + sort + " " + order;


            Boolean showSTSImage = false;
            string userId = master.getUserID().ToString();
            // If you are STS, NIA, Data cordinator, Dan Jennings, Bryan, Derek or Raleigh you want to show the images on if STS has their RFQ info filled out
            if (ddlCompany.SelectedValue == "13" || ddlCompany.SelectedValue == "20" || master.getUserRole() == 1 || userId == "24" || userId == "1" || userId == "17" || userId == "181")
            {
                showSTSImage = true;
                lblSTSPictureInfo.Text = "<img src='issues.png' width='35' height='35' />: STS RFQ Info is not filled out<br><img src='Checkmark.png' width='35' height='35' />: STS RFQ Info filled out";
            }

            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                RFQItem newItem = new RFQItem();
                newItem.customer = dr.GetValue(4).ToString() + "<br />" + dr.GetValue(8).ToString();
                newItem.customer_rfq = dr.GetValue(2).ToString();
                try
                {
                    newItem.date_due = System.Convert.ToDateTime(dr.GetValue(3)).ToString("d");
                }
                catch
                {

                }
                newItem.rank = dr.GetValue(5).ToString();
                newItem.rfqid = dr.GetValue(0).ToString();
                //newItem.rfqLink = "<a href='https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id=" + dr.GetValue(0).ToString() + ">" + dr.GetValue(0).ToString() + "</ a>";

                newItem.salesman = dr.GetValue(6).ToString();
                newItem.status = dr.GetValue(1).ToString();

                if(System.Convert.ToBoolean(dr.GetValue(7)))
                {
                    newItem.liveWork = "Yes";
                }

                
                newItem.numberOfParts = dr.GetValue(9).ToString();
                newItem.numberOfPartsReserved = dr.GetValue(11).ToString();
                newItem.numberOfPartsQuoted = dr.GetValue(10).ToString();


                newItem.notified = dr.GetValue(12).ToString();
                if(newItem.notified == "")
                {
                    newItem.notified = "TSG";
                }
                else if (newItem.notified.Contains("STS"))
                {
                    if (dr["STSRFQInfo"].ToString() != "" && showSTSImage)
                    {
                        newItem.status += "<img src='Checkmark.png' width='35' height='35' />";
                    }
                    else if (showSTSImage)
                    {
                        newItem.status += "<br><img src='issues.png' width='35' height='35' />";
                    }
                }
                newItem.oem = dr.GetValue(13).ToString();

                newItem.notification_list = new List<TSGCompany>();
                MasterList.Add(newItem);
            }
            dr.Close();
            sql.Parameters.Clear();
            //foreach (RFQItem item in MasterList)
            //{
            //    sql.CommandText = "select TSGCompanyID, TSGCompanyAbbrev from linkRFQToCompany, TSGCompany where rtqRFQID=@rfq and rtqCompanyID=TSGCompanyID order by TSGCompanyAbbrev";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@rfq", item.rfqid);
            //    dr = sql.ExecuteReader();
            //    while (dr.Read())
            //    {
            //        item.notification_list.Add(new TSGCompany { company_id = System.Convert.ToInt64(dr.GetValue(0)), abbreviation = dr.GetValue(1).ToString() });
            //    }
            //    dr.Close();
            //}
            connection.Close();
            connection2.Close();
        }

        protected void btnHotList_Click(object sender, EventArgs e)
        {
            HotList = true;
            findClicked = true;
            refreshPage();
        }

        protected void btnNewRFQ_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "<script>window.open('EditRFQ.aspx?id=0','_blank');</script>";
        }

        protected void btnNewQuote_Click(object sender, EventArgs e)
        {
            if (ddlQuoteType.SelectedValue == "8")
            {
                lblMessage.Text = "<script>window.open('HTSEditQuote?quoteType=3');</script>";
            }
            else if (ddlQuoteType.SelectedValue == "9")
            {
                lblMessage.Text = "<script>window.open('STSEditQuote');</script>";
            }
            else if (ddlQuoteType.SelectedValue == "10")
            {
                lblMessage.Text = "<script>window.open('UGSEditQuote');</script>";
            }
            else if (ddlQuoteType.SelectedValue == "12")
            {
                lblMessage.Text = "<script>window.open('EditQuote?id=0&rfq=0&quoteType=1&notes=1');</script>";
            }
            else
            {
                lblMessage.Text = "<script>window.open('EditQuote?id=0&rfq=0&quoteType=1');</script>";
            }
        }

        protected void btnOverDue_Click(object sender, EventArgs e)
        {
            HotList = false;
            OverDue = true;
            findClicked = true;
            refreshPage();
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            OverDue = false;
            HotList = false;
            findClicked = true;
            pageIndex = 0;
            dgResults.PageIndex = pageIndex;
            refreshPage();
        }

    }
}
 