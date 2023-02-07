using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;

namespace RFQ
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (!IsPostBack)
            {
                txtEnd.Text = DateTime.Now.AddDays(1).ToString("d");
                txtStart.Text = DateTime.Now.AddMonths(-24).ToString("d");
                cbQuote.Checked = true;

                sql.CommandText = "Select wlrWinLossReason, wlrWinLossReasonID from pktblWinLossReason order by wlrWinLossReason ";
                sql.Parameters.Clear();
                SqlDataReader wlr = sql.ExecuteReader();
                ddlWinLossReason.DataSource = wlr;
                ddlWinLossReason.DataTextField = "wlrWinLossReason";
                ddlWinLossReason.DataValueField = "wlrWinLossReasonID";
                ddlWinLossReason.DataBind();
                wlr.Close();

                sql.CommandText = "Select wlsWinLossID, wlsWinLoss from pktblWinLoss ";
                sql.Parameters.Clear();
                SqlDataReader wl = sql.ExecuteReader();
                ddlWinLoss.DataSource = wl;
                ddlWinLoss.DataTextField = "wlsWinLoss";
                ddlWinLoss.DataValueField = "wlsWinLossID";
                ddlWinLoss.DataBind();
                wl.Close();


                sql.CommandText = "select OEMID, OEMName from OEM where OEMName not in ('nul;','undefined') order by OEMName";
                sql.Parameters.Clear();
                SqlDataReader oemDR = sql.ExecuteReader();
                ddlOEM.DataSource = oemDR;
                ddlOEM.DataTextField = "OEMName";
                ddlOEM.DataValueField = "OEMID";
                ddlOEM.SelectedValue = "39";
                ddlOEM.DataBind();
                oemDR.Close();
                ddlOEM.Items.Add("Any");
                ddlOEM.SelectedValue = "Any";

                sql.CommandText = "Select TSGCompanyAbbrev, TSGCompanyID from TSGCompany where tcoActive = 1";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                ddlCompany.DataSource = dr;
                ddlCompany.DataTextField = "TSGCompanyAbbrev";
                ddlCompany.DataValueField = "TSGCompanyID";
                ddlCompany.DataBind();
                dr.Close();

                sql.CommandText = "select ProgramID, ProgramName from Program where ProgramName not in ('0','  ADD NEW') order by ProgramName";
                sql.Parameters.Clear();
                SqlDataReader progDR = sql.ExecuteReader();
                ddlProgram.DataSource = progDR;
                ddlProgram.DataTextField = "ProgramName";
                ddlProgram.DataValueField = "ProgramID";
                ddlProgram.DataBind();
                progDR.Close();
                ddlProgram.Items.Add("All");
                ddlProgram.SelectedValue = "All";

                sql.CommandText = "Select qstQuoteStatusID, qstQuoteStatusDescription from pktblQuoteStatus order by qstQuoteStatus";
                sql.Parameters.Clear();
                SqlDataReader qsDR = sql.ExecuteReader();
                ddlStatus.DataSource = qsDR;
                ddlStatus.DataTextField = "qstQuoteStatusDescription";
                ddlStatus.DataValueField = "qstQuoteStatusID";
                ddlStatus.DataBind();
                qsDR.Close();
                ddlStatus.Items.Add("All");
                ddlStatus.SelectedValue = "All";

                sql.CommandText = "Select Name, TSGSalesmanID from TSGSalesman";
                SqlDataReader salDR = sql.ExecuteReader();
                ddlSalesman.DataSource = salDR;
                ddlSalesman.DataTextField = "Name";
                ddlSalesman.DataValueField = "TSGSalesmanID";
                ddlSalesman.DataBind();
                salDR.Close();
                ddlSalesman.Items.Add("Any");
                ddlSalesman.SelectedValue = "Any";

                sql.CommandText = "Select CONCAT(estFirstName, ' ', estLastName) as 'name', estEstimatorID from pktblEstimators";
                SqlDataReader estimatorDR = sql.ExecuteReader();
                ddlEstimator.DataSource = estimatorDR;
                ddlEstimator.DataTextField = "name";
                ddlEstimator.DataValueField = "estEstimatorID";
                ddlEstimator.DataBind();
                estimatorDR.Close();
                ddlEstimator.Items.Add("Any");
                ddlEstimator.SelectedValue = "Any";

                ddlQuoteType.Items.Insert(0, "All");
                //ddlQuoteType.Items.Insert(1, "New Tool");
                //ddlQuoteType.Items.Insert(2, "Stand Alone");
                //ddlQuoteType.Items.Insert(3, "HTS");
                //ddlQuoteType.Items.Insert(4, "Mas");

                ddlPartNumber.Items.Insert(0, "Contains");
                ddlPartNumber.Items.Insert(1, "Equals");
                ddlPartNumber.Items.Insert(2, "Begins with");
                ddlPartNumber.Items.Insert(3, "Ends with");

                ddlPartName.Items.Insert(0, "Contains");
                ddlPartName.Items.Insert(1, "Equals");
                ddlPartName.Items.Insert(2, "Begins with");
                ddlPartName.Items.Insert(3, "Ends with");

                ddlcustomerSearch.Items.Insert(0, "Contains");
                ddlcustomerSearch.Items.Insert(1, "Equals");
                ddlcustomerSearch.Items.Insert(2, "Begins with");
                ddlcustomerSearch.Items.Insert(3, "Ends with");

                ddlCustomerRFQ.Items.Insert(0, "Contains");
                ddlCustomerRFQ.Items.Insert(1, "Equals");
                ddlCustomerRFQ.Items.Insert(2, "Begins with");
                ddlCustomerRFQ.Items.Insert(3, "Ends with");

                ddlCustomerLocation.Items.Insert(0, "Contains");
                ddlCustomerLocation.Items.Insert(1, "Equals");
                ddlCustomerLocation.Items.Insert(2, "Begins with");
                ddlCustomerLocation.Items.Insert(3, "Ends with");

                sql.CommandText = "select rstRFQStatusID, rstRFQStatusDescription from pktblRFQStatus order by iif(rstRFQStatusDescription = 'RFQ Received',0,1), rstRFQStatusDescription";
                sql.Parameters.Clear();
                SqlDataReader statusDR = sql.ExecuteReader();
                ddlRFQStatus.DataSource = statusDR;
                ddlRFQStatus.DataTextField = "rstRFQStatusDescription";
                ddlRFQStatus.DataValueField = "rstRFQStatusID";
                ddlRFQStatus.SelectedValue = "2";
                ddlRFQStatus.DataBind();
                statusDR.Close();

                ddlRFQStatus.Items.Add("All");
                ddlRFQStatus.SelectedValue = "All";
            }
            connection.Close();
        }


        protected void btnFind_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            // make sure start and end are valid dates
            try
            {
                DateTime testDate = System.Convert.ToDateTime(txtStart.Text);
            }
            catch
            {
                txtStart.Text = DateTime.Now.AddMonths(-6).ToString("d");
            }
            try
            {
                DateTime testDate = System.Convert.ToDateTime(txtEnd.Text);
            }
            catch
            {
                txtEnd.Text = DateTime.Now.ToString("d");
            }

            //TODO part number into rfq
            if(cbRFQ.Checked)
            {
                lblTruncated.Text = "";

                lbltotalCost.Visible = false;
                dgResults.Visible = true;

                lbltotalCost.Visible = false;

                sql.CommandText = "Select Distinct rfqID, rstRFQStatusDescription, Customer.CustomerName, CustomerLocation.ShipToName, CustomerContact.Name as customerContact, rfqCustomerRFQNumber, ProgramName, OEMName, vehVehicleName, ptyProductType, rsoSourceName, ";
                sql.CommandText += "TSGSalesman.Name as salesman,  CONVERT(date, rfqDueDate) as rfqDueDate, convert(date, rfqDateReceived) as rfqDateReceived, convert(date, rfqCreated) as rfqCreated ";
                sql.CommandText += "from tblRFQ, tblCompanyNotified, Customer, CustomerLocation, pktblRFQStatus, pktblVehicle, pktblProductType, pktblRFQSource, TSGSalesman, CustomerContact, Program, OEM, linkPartToRFQ, tblPart ";
                sql.CommandText += "where rfqID = cnoRFQID and(cnoTSGCompanyID = @company or @company = 1) and(rfqCustomerRFQNumber like @customerRFQ or @customerRFQ is null) and (convert(varchar(10), rfqOEMID) = @oem or @oem = 'Any') and ";
                sql.CommandText += "(Customer.CustomerID = rfqCustomerID) and(Customer.CustomerName like @customer or @customer is null) and CustomerLocationID = rfqPlantID and rfqStatus = rstRFQStatusID ";
                sql.CommandText += "and(CONVERT(varchar(10), rstRFQStatusID) = @rfqStatus or @rfqStatus = 'All') and rfqVehicleID = vehVehicleID and rfqProductTypeID = ptyProductTypeID and rfqSourceID = rsoSourceID ";
                sql.CommandText += "and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and CustomerContactID = rfqCustomerContact and ProgramID = rfqProgramID and rfqOEMID = OEMID ";
                sql.CommandText += "and (CONVERT(varchar(10), TSGSalesman.TSGSalesmanID) = @salesman or @salesman = 'Any') and (CONVERT(varchar(10), rfqID) = @rfq or @rfq is null)  and (convert(varchar(10), rfqProgramID) = @program or @program = 'All') ";
                sql.CommandText += "and ptrRFQID = rfqID and prtPARTID = ptrPartID and (prtPartNumber like @partNum or @partNum is null) and (CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                sql.CommandText += "and (prtpartDescription like @partName or @partName is null) and rfqDateReceived >= @start and rfqDateReceived <= @end ";

                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                sql.Parameters.AddWithValue("@rfqStatus", ddlRFQStatus.SelectedValue);
                sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));
                if (txtRFQ.Text == "")
                {
                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                }
                if(txtCustomerRFQ.Text == "")
                {
                    sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                }
                if(txtCustomer.Text == "")
                {
                    sql.Parameters.AddWithValue("@customer", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                }
                if (txtCustomerLocation.Text == "")
                {
                    sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                }
                else 
                {
                    sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                }
                if (txtPartNumber.Text == "")
                {
                    sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                }
                else 
                {
                    sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                }
                if (txtPartName.Text == "")
                {
                    sql.Parameters.AddWithValue("@partName", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                }

                sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);

                

                //dgResults.DataSource = dr;
                DataTable table = new DataTable();
                table.Load(sql.ExecuteReader());


                dgResults.DataSource = table;
                dgResults.DataBind();
                //dr.Close();
            }
            else
            {
                dgResults.Visible = false;
            }
            List<string> quoteIDs = new List<string>();


            if (cbQuote.Checked)
            {
                lbltotalCost.Visible = true;
                lbltotalCost.Text = "";
                lblTruncated.Visible = true;
                lblTruncated.Text = "";
                long userCompanyID = master.getCompanyId();
                string userCompanyAbbrev = "";
                sql.Parameters.Clear();
                sql.CommandText = "Select TSGCompanyAbbrev from TSGCompany where @usercompany = TSGCompanyID";
                sql.Parameters.AddWithValue("@usercompany", userCompanyID);

                SqlDataReader dr1 = sql.ExecuteReader();
                while (dr1.Read())
                {
                    userCompanyAbbrev = dr1.GetValue(0).ToString();
                };

                dr1.Close();
                sql.Parameters.Clear();

                List<quoteInfo> quoteList = new List<quoteInfo>();

                dgQuote.Visible = true;

                lbltotalCost.Visible = true;
                double total = 0;

                SqlDataReader dr;

                if (ddlQuoteType.SelectedValue == "All" || ddlQuoteType.SelectedValue == "New Tool")
                {
                    string company = containsCompany(txtQuoteNumber.Text);
                    if (company != "9" && company != "13" && company != "15")
                    {
                        //sql.CommandText = "Select quoQuoteID, concat(rfqID, '-', prtRFQLineNumber, '-', TSGCompanyAbbrev, '-', quoVersion) as quoteNumber, rfqID, CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription, ";
                        //sql.CommandText += "(select sum(pwnCostNote) from pktblPreWordedNote, linkPWNToQuote where pwqQuoteID = quoQuoteID and pwqPreWordedNoteID = pwnPreWordedNoteID) as cost, dtyFullName, cavCavityName, quoCreated, qtrSent, CustomerContact.Name, rfqCustomerRFQNumber, ";
                        //sql.CommandText += "prtPartNumber, prtpartDescription, TSGSalesman.Name, quoOldQuoteNumber, quoTSGCompanyID, quoWinLossID, quoWinLossReasonID, quoPONumber, quoAwardedAmount, quoTargetPrice, quoDispositionNote ";
                        //sql.CommandText += "from tblQuote, linkQuoteToRFQ, tblRFQ, linkPartToQuote, tblPart, pktblQuoteStatus, pktblPartType, TSGCompany, pktblEstimators, linkDieInfoToQuote, tblDieInfo, DieType, pktblCavity, Customer, CustomerLocation, CustomerContact, TSGSalesman ";
                        //sql.CommandText += "where (quoTSGCompanyID = @company or @company = 1) and(convert(varchar(10), quoEstimatorID) = @estimator or quoEstimatorID is null or @estimator = 'Any') and (convert(varchar(10), quoStatusID) = @status or @status = 'All') ";
                        //sql.CommandText += "and qtrRFQID = rfqID and qtrQuoteID = quoQuoteID and ptqQuoteID = quoQuoteID and prtPARTID = ptqPartID and(Customer.CustomerID = rfqCustomerID) and(Customer.CustomerName like @customer or @customer is null) ";
                        //sql.CommandText += "and CustomerLocationID = rfqPlantID and (((CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) and (prtRFQLineNumber like @lineNum or @lineNum is null)) or (quoOldQuoteNumber like '%SA%' and quoOldQuoteNumber like @oldQuoteNumSA) or (quoOldQuoteNumber = @oldQuoteNum)) ";
                        //sql.CommandText += "and (rfqCustomerRFQNumber like @customerRFQ or @customerRFQ is null) and (CONVERT(varchar(10), quoSalesman) = @salesman or @salesman = 'Any') ";
                        //sql.CommandText += "and qstQuoteStatusID = quoStatusID and ptyPartTypeID = quoPartTypeID and TSGCompany.TSGCompanyID = quoTSGCompanyID and estEstimatorID = quoEstimatorID and diqQuoteID = quoQuoteID and dinDieInfoID = diqDieInfoID ";
                        //sql.CommandText += "and dinCavityID = cavCavityID and dinDieType = DieType.DieTypeID and (prtPartNumber like @partNum or @partNum is null) and (convert(varchar(10), rfqProgramID) = @program or @program = 'All') ";
                        //sql.CommandText += "and (Select (select distinct 1 from linkPartToPartDetail, linkPartReservedToCompany where ppd.ppdPartToPartID = ppdPartToPartID and ppd.ppdPartID <> ppdPartID and ppdPartID = prcPartID) from linkPartToPartDetail as ppd where ppdPartID = prtPartID) is null ";
                        //sql.CommandText += "and (CustomerContactID = rfqCustomerContact or rfqCustomerContact is null) and TSGSalesman.TSGSalesmanID = quoSalesman and (CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                        //sql.CommandText += "and quoCreated >= @start and quoCreated <= @end and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 and (prtpartDescription like @partName or @partName is null) and (TSGCompanyAbbrev = @abbrev or @abbrev is null) and (quoVersion = @version or @version is null) ";
                        //if (cbDisposition.Checked) {
                        //    sql.CommandText += "and quoWinLossID is null and (quoStatusID = 7 or quoStatusID = 8) ";
                        //}
                        //sql.CommandText += "order by rfqID, prtRFQLineNumber ";

                        sql.CommandText = "Select quoQuoteID, quoOldQuoteNumber, prtRFQLineNumber, quoVersion, qtrRFQID, TSGCompanyAbbrev, quoPartNumbers, quoPartName, rfqID, CustomerName,  ";
                        sql.CommandText += "rfqPlant.ShipToName as rfqLocation, quotePlant.ShipToName as quoteLocation, quoCustomerContact, CustomerContact.Name as contactName, rfqCustomerRFQNumber,  ";
                        sql.CommandText += "concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription,  ";
                        sql.CommandText += "(select sum(pwnCostNote) from pktblPreWordedNote, linkPWNToQuote where pwqQuoteID = quoQuoteID and pwqPreWordedNoteID = pwnPreWordedNoteID) as cost, ";
                        sql.CommandText += "dtyFullName, cavCavityName, quoCreated, qtrSent, TSGSalesman.Name as salesman ";
                        sql.CommandText += "from tblQuote ";
                        sql.CommandText += "inner join linkPartToQuote on ptqQuoteID = quoQuoteID and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0 ";
                        sql.CommandText += "inner join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "inner join linkQuoteToRFQ on qtrQuoteID = quoQuoteID and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0 ";
                        sql.CommandText += "inner join tblRFQ on rfqID = qtrRFQID ";
                        sql.CommandText += "inner join Customer on Customer.CustomerID = rfqCustomerID ";
                        sql.CommandText += "inner join CustomerLocation as rfqPlant on rfqPlant.CustomerLocationID = rfqPlantID ";
                        sql.CommandText += "left outer join CustomerLocation as quotePlant on quotePlant.CustomerLocationID = quoPlant ";
                        sql.CommandText += "inner join CustomerContact on CustomerContactID = rfqCustomerContact ";
                        sql.CommandText += "inner join TSGSalesman on TSGSalesman.TSGSalesmanID = rfqSalesman ";
                        sql.CommandText += "inner join pktblEstimators on estEstimatorID = quoEstimatorID ";
                        sql.CommandText += "inner join pktblQuoteStatus on qstQuoteStatusID = quoStatusID ";
                        sql.CommandText += "inner join TSGCompany on TSGCompanyID = quoTSGCompanyID ";
                        sql.CommandText += "inner join linkDieInfoToQuote on diqQuoteID = quoQuoteID ";
                        sql.CommandText += "inner join tblDieInfo on dinDieInfoID = diqDieInfoID ";
                        sql.CommandText += "inner join DieType on DieTypeID = dinDieType ";
                        sql.CommandText += "inner join pktblCavity on cavCavityID = dinCavityID ";
                        sql.CommandText += "where (quoCreated > @start and quoCreated <= @end) and(TSGCompany.TSGCompanyID = @company or @company = 1) and(quoStatusID = @status or @status is null) and ";
                        sql.CommandText += "(rfqProgramID = @program or @program is null) and (rfqSalesman = @salesman or @salesman is null) and ";
                        sql.CommandText += "(quoEstimatorID = @estimator or @estimator is null) and(((CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) and(prtRFQLineNumber like @lineNum or @lineNum is null)) or ";
                        sql.CommandText += "(quoOldQuoteNumber like '%SA%' and quoOldQuoteNumber like @oldQuoteNumSA) or(quoOldQuoteNumber = @oldQuoteNum)) and rfqCustomerRFQNumber like @customerRFQ  and ";
                        sql.CommandText += "(rfqOEMID = @oem or @oem is null) and quoPartNumbers like @partNum and quoPartName like @partName and CustomerName like @customer and(quotePlant.ShipToName like @plant or ";
                        sql.CommandText += "rfqPlant.ShipToName like @plant) and (TSGCompanyAbbrev = @abbrev or @abbrev is null) and (quoVersion = @version or @version is null) ";
                        sql.CommandText += "order by rfqID, prtRFQLineNumber ";
                        //and(quoQuoteTypeID = @type or @type is null)
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                        if (ddlStatus.SelectedItem.ToString() == "All")
                        {
                            sql.Parameters.AddWithValue("@status", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        }
                        if (ddlProgram.SelectedItem.ToString() == "All")
                        {
                            sql.Parameters.AddWithValue("@program", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                        }
                        sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));


                        if (txtQuoteNumber.Text != "")
                        {
                            string[] arr = txtQuoteNumber.Text.Split('-');
                            if (arr.Length >= 4)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNumSA", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                            }
                            else if (arr.Length == 3)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNumSA", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else if (arr.Length == 2)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNumSA", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else if (arr.Length == 1 && txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                //We either match the old quote num perfectly or we want something that deffinitely wont show up
                                sql.Parameters.AddWithValue("@oldQUoteNum", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@oldQuoteNumSA", "%" + arr[0] + "%");
                                sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else
                            {
                                if (txtRFQ.Text == "")
                                {
                                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                    //sql.Parameters.AddWithValue("@line", )
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                                }
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                sql.Parameters.AddWithValue("@oldQUoteNum", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@oldQuoteNumSA", "asdlfjk;asdf");
                                sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                        }
                        else
                        {
                            if (txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                //sql.Parameters.AddWithValue("@line", )
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                            }
                            sql.Parameters.AddWithValue("@oldQUoteNum", "asdlfjk;asdf");
                            sql.Parameters.AddWithValue("@oldQuoteNumSA", "asdlfjk;asdf");
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }

                        sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                        sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                        sql.Parameters.AddWithValue("@plant", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                        sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                        sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                        if (ddlOEM.SelectedItem.ToString() == "Any")
                        {
                            sql.Parameters.AddWithValue("@oem", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                        }
                        if (ddlSalesman.SelectedItem.ToString() == "Any")
                        {
                            sql.Parameters.AddWithValue("@salesman", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);
                        }
                        if (ddlEstimator.SelectedItem.ToString() == "Any")
                        {
                            sql.Parameters.AddWithValue("@estimator", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                        }

                        dr = sql.ExecuteReader();
                        List<string> quotes = new List<string>();
                        while (dr.Read())
                        {
                            if (quotes.Contains(dr["quoQuoteID"].ToString()))
                            {
                                continue;
                            }
                            quotes.Add(dr["quoQuoteID"].ToString());
                            quoteInfo qi = new quoteInfo();

                            if ((dr["TSGCompanyAbbrev"].ToString() == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                            {
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/EditQuote?id=" + dr["quoQuoteID"].ToString() + "&quoteType=2";
                                qi.quoteID = dr["quoQuoteID"].ToString();
                                qi.price = dr["cost"].ToString();
                                if (qi.price != "")
                                {
                                    total += System.Convert.ToDouble(qi.price);
                                    qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                                }
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }

                            int num;
                            bool results = Int32.TryParse(dr["quoOldQuoteNumber"].ToString(), out num);
                            if (!results && dr["quoOldQuoteNumber"].ToString() != "")
                            {
                                if (dr["quoOldQuoteNumber"].ToString().Contains("SA"))
                                {
                                    qi.quoteNumber = dr["quoOldQuoteNumber"].ToString();
                                }
                                else
                                {
                                    qi.quoteNumber = dr["quoOldQuoteNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString();
                                }
                            }
                            else
                            {
                                qi.quoteNumber = dr["qtrRFQID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-" + dr["TSGCompanyAbbrev"].ToString() + "-" + dr["quoVersion"].ToString();
                            }

                            qi.rfqID = dr["rfqID"].ToString();
                            qi.rfqLink = "https://tsgrfq.azurewebsites.net/EditRFQ?id=" + qi.rfqID;
                            qi.customer = dr["CustomerName"].ToString();
                            qi.customerLocation = dr["rfqLocation"].ToString();
                            if (dr["quoteLocation"].ToString() != "")
                            {
                                qi.customerLocation = dr["quoteLocation"].ToString();
                            }
                            qi.estimator = dr["estimator"].ToString();
                            qi.quoteStatus = dr["qstQuoteStatusDescription"].ToString();
                            qi.dieType = dr["dtyFullName"].ToString();
                            qi.cavity = dr["cavCavityName"].ToString();
                            qi.created = dr["quoCreated"].ToString();
                            qi.sent = dr["qtrSent"].ToString();
                            qi.customerContact = dr["quoCustomerContact"].ToString();
                            if (qi.customerContact == "")
                            {
                                qi.customerContact = dr["contactName"].ToString();
                            }
                            qi.customerRFQNum = dr["rfqCustomerRFQNumber"].ToString();
                            qi.partNumber = dr["quoPartNumbers"].ToString();
                            qi.partDescription = dr["quoPartName"].ToString();
                            qi.salesman = dr["salesman"].ToString();

                            //if (dr["quoOldQuoteNumber"].ToString() != "")
                            //{
                            //    qi.quoteNumber = dr["quoOldQuoteNumber"].ToString() + "-" + dr.GetValue(1).ToString().Split('-')[2] + "-" + dr.GetValue(1).ToString().Split('-')[3];
                            //}
                            //else
                            //{
                            //    qi.quoteNumber = dr.GetValue(1).ToString();
                            //}
                            //qi.rfqID = dr.GetValue(2).ToString();
                            //qi.rfqLink = "Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + qi.rfqID;
                            //qi.customer = dr.GetValue(3).ToString();
                            //qi.customerLocation = dr.GetValue(4).ToString();
                            //qi.estimator = dr.GetValue(5).ToString();
                            //qi.quoteStatus = dr.GetValue(6).ToString();
                            //qi.price = dr.GetValue(7).ToString();
                            //qi.dieType = dr.GetValue(8).ToString();
                            //qi.cavity = dr.GetValue(9).ToString();
                            //qi.created = dr.GetValue(10).ToString();
                            //qi.sent = dr.GetValue(11).ToString();
                            //qi.customerContact = dr.GetValue(12).ToString();
                            //qi.customerRFQNum = dr.GetValue(13).ToString();
                            //qi.partNumber = dr.GetValue(14).ToString();
                            //qi.partDescription = dr.GetValue(15).ToString();
                            //qi.salesman = dr.GetValue(16).ToString();
                            //qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-RFQ-" + dr["quoTSGCompanyID"].ToString() + "', '";
                            //qi.dispositionButton += dr["quoWinLossID"].ToString() + "', '" + dr["quoWinLossReasonID"].ToString() + "', '" + dr["quoPONumber"].ToString() + "', '" + dr["quoAwardedAmount"].ToString() + "', '" + dr["quoTargetPrice"].ToString() + "', '";
                            //qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["quoDispositionNote"].ToString()) + "');return false;\" >";

                            quoteList.Add(qi);
                        }
                        dr.Close();
                    }
                }

                if (ddlQuoteType.SelectedValue == "All")
                {
                    string company = containsCompany(txtQuoteNumber.Text);
                    if (company == "" || company == "15")
                    {
                        sql.CommandText = "Select uquUGSQuoteID, concat(rfqID, '-', prtRFQLineNumber, '-', TSGCompanyAbbrev, '-', uquQuoteVersion) as quoteNumber, rfqID, CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator,  ";
                        sql.CommandText += "qstQuoteStatusDescription, uquTotalPrice as cost, dtyFullName, uquCreated, qtrSent, uquCustomerContact as contactName, rfqCustomerRFQNumber, prtPartNumber, prtpartDescription, TSGSalesman.Name as salesmanName,  ";
                        sql.CommandText += "uquQuoteNumber, uquQuoteVersion, uquPartNumber, uquPartName, uquCustomerRFQNumber, uquWinLossID, uquWinLossReasonID, uquPoNumber, uquAwardedAmount, uquTargetPrice, uquDispositionNote ";
                        sql.CommandText += "from pktblQuoteStatus, TSGCompany, pktblEstimators, DieType, Customer, CustomerLocation, TSGSalesman, tblUGSQuote ";
                        sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = uquUGSQuoteID and qtrUGS = 1 ";
                        sql.CommandText += "left outer join tblRFQ on rfqID = qtrRFQID ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = uquUGSQuoteID and ptqUGS = 1 ";
                        sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "where(15 = @company or @company = 1) and(convert(varchar(10), uquEstimatorID) = @estimator or uquEstimatorID is null or @estimator = 'Any') and(convert(varchar(10), uquStatusID) = @status or @status = 'All') ";
                        sql.CommandText += "and(Customer.CustomerID = uquCustomerID) and(Customer.CustomerName like @customer or @customer is null) and CustomerLocationID = uquPlantID and((((CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) ";
                        sql.CommandText += "and(prtRFQLineNumber like @lineNum or @lineNum is null)) or(uquQuoteNumber = @oldQuoteNum)) and(uquUGSQuoteID = @quoteID or @quoteID is null)) and(uquCustomerRFQNumber like @customerRFQ or @customerRFQ is null) ";
                        sql.CommandText += "and(CONVERT(varchar(10), uquSalesmanID) = @salesman or @salesman = 'Any') and qstQuoteStatusID = uquStatusID and TSGCompany.TSGCompanyID = 15 and estEstimatorID = uquEstimatorID ";
                        sql.CommandText += "and uquDieType = DieType.DieTypeID and(uquPartNumber like @partNum or @partNum is null) and(convert(varchar(10), rfqProgramID) = @program or @program = 'All') ";
                        sql.CommandText += "and(Select(select distinct 1 from linkPartToPartDetail, linkPartReservedToCompany where ppd.ppdPartToPartID = ppdPartToPartID and ppd.ppdPartID <> ppdPartID and ppdPartID = prcPartID) from linkPartToPartDetail as ppd where ppdPartID = prtPartID) is null ";
                        sql.CommandText += "and TSGSalesman.TSGSalesmanID = uquSalesmanID and(CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) and (uquPartName like @partName or @partName is null) ";
                        sql.CommandText += "and uquCreated >= @start and uquCreated <= @end and (uquQuoteVersion = @version or @version is null) ";
                        if (cbDisposition.Checked)
                        {
                            sql.CommandText += "and uquWinLossID is null and (uquStatusID = 7 or uquStatusID = 8) ";
                        }
                        sql.Parameters.Clear();

                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                        sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                        sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));

                        if (txtQuoteNumber.Text != "")
                        {
                            string[] arr = txtQuoteNumber.Text.Split('-');
                            if (txtQuoteNumber.Text.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", "500");
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0]);
                                if (arr.Length > 3)
                                {
                                    sql.Parameters.AddWithValue("@version", arr[3]);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@version", DBNull.Value);
                                }
                            }
                            else if (arr.Length >= 4)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                //sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            }
                            else if (arr.Length == 3)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                //sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            }
                            else if (arr.Length == 2)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else if (arr.Length == 1 && txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                //We either match the old quote num perfectly or we wont something that deffinitely wont show up
                                sql.Parameters.AddWithValue("@oldQuoteNum", "asdadfasd");
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else
                            {
                                if (txtRFQ.Text == "")
                                {
                                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.ToString());
                                }
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                sql.Parameters.AddWithValue("@oldQuoteNum", "asdfasdfasdf");
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                        }
                        else
                        {
                            if (txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                            }
                            sql.Parameters.AddWithValue("@oldQuoteNum", "asdfasdfasdf");
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }

                        if (txtCustomerRFQ.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                        }
                        if (txtCustomer.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customer", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                        }
                        if (txtCustomerLocation.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                        }
                        if (txtPartNumber.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                        }
                        if (txtPartName.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partName", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                        }
                        sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                        sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);
                        sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);



                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            quoteInfo qi = new quoteInfo();

                            if ((userCompanyAbbrev == "UGS") || (userCompanyAbbrev == "TSG"))
                            {
                                qi.quoteID = dr["uquUGSQuoteID"].ToString();
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/UGSEditQuote?id=" + dr["uquUGSQuoteID"].ToString();

                                qi.price = dr["cost"].ToString();
                                if (qi.price == "")
                                {
                                    qi.price = "0";
                                }
                                total += System.Convert.ToDouble(qi.price);
                                qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }
                            
                            int num;
                            bool result = Int32.TryParse(dr["uquQuoteNumber"].ToString(), out num);
                            if (dr["rfqID"].ToString() == "")
                            {
                                if (dr["uquQuoteNumber"].ToString() != "")
                                {
                                    qi.quoteNumber = dr["uquQuoteNumber"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                                }
                                else
                                {
                                    qi.quoteNumber = dr["uquUGSQuoteID"].ToString() + "-UGS-SA-" + dr["uquQuoteVersion"].ToString();
                                }
                            }
                            else if (!result)
                            {
                                qi.quoteNumber = dr["uquQuoteNumber"].ToString() + "-UGS-" + dr["uquQuoteVersion"].ToString();
                            }
                            else
                            {
                                qi.quoteNumber = dr["quoteNumber"].ToString();
                            }
                            qi.rfqID = dr["rfqID"].ToString();
                            qi.rfqLink = "Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + qi.rfqID;
                            qi.customer = dr["CustomerName"].ToString();
                            qi.customerLocation = dr["ShipToName"].ToString();
                            qi.estimator = dr["estimator"].ToString();
                            qi.quoteStatus = dr["qstQuoteStatusDescription"].ToString();
                            qi.dieType = dr["dtyFullName"].ToString();
                            qi.cavity = "";
                            qi.created = dr["uquCreated"].ToString();
                            qi.sent = dr["qtrSent"].ToString();
                            qi.customerContact = dr["contactName"].ToString();
                            qi.customerRFQNum = dr["uquCustomerRFQNumber"].ToString();
                            qi.partNumber = dr["uquPartNumber"].ToString();
                            qi.partDescription = dr["uquPartName"].ToString();
                            qi.salesman = dr["salesmanName"].ToString();
                            qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-UGS-15', '";
                            qi.dispositionButton += dr["uquWinLossID"].ToString() + "', '" + dr["uquWinLossReasonID"].ToString() + "', '" + dr["uquPONumber"].ToString() + "', '" + dr["uquAwardedAmount"].ToString() + "', '" + dr["uquTargetPrice"].ToString() + "', '";
                            qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["uquDispositionNote"].ToString()) + "');return false;\" >";
                            quoteList.Add(qi);
                        }
                        dr.Close();
                    }

                    if (company == "" || company == "13")
                    {
                        sql.CommandText = "Select squSTSQuoteID, concat(rfqID, '-', prtRFQLineNumber, '-STS-', squQuoteVersion) as quoteNumber, squRFQNum, Customer.CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription,  ";
                        sql.CommandText += "(select sum(pwnCostNote) from linkPWNToSTSQuote, pktblPreWordedNote where psqPreWordedNoteID = pwnPreWordedNoteID and psqSTSQuoteID = squSTSQuoteID) as cost, squProcess, '',  ";
                        sql.CommandText += "squCreated, squCustomerContact, squCustomerRFQNum, qtrQuoteToRFQID, squPartNumber, squPartName, TSGSalesman.Name as salesmanName, squQuoteNumber, squQuoteVersion, rfqID, qtrSent, ";
                        sql.CommandText += "squWinLossID, squWinLossReasonID, squPONumber, squAwardedAmount, squTargetPrice, squDispositionNote ";
                        sql.CommandText += "from Customer, CustomerLocation, pktblEstimators, pktblQuoteStatus, TSGSalesman, tblSTSQuote ";
                        sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                        sql.CommandText += "left outer join tblRFQ on rfqID = qtrRFQID ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = squSTSQuoteID and ptqSTS = 1 ";
                        sql.CommandText += "left outer join tblPart on prtPARTID = ptqPartID ";
                        sql.CommandText += "where Customer.CustomerID = squCustomerID and CustomerLocationID = squPlantID and squEstimatorID = estEstimatorID and squStatusID = qstQuoteStatusID ";
                        sql.CommandText += "and(13 = @company or @company = 1) and(convert(varchar(10), squSTSQuoteID) = @status or @status = 'All') ";
                        sql.CommandText += "and(squRFQNum = @rfq or @rfq is null) and(squCustomerRFQNum like @customerRFQ or @customerRFQ is null) and(squPartNumber like @partNum or @partNum is null) and(Customer.CustomerName like @customer or @customer is null) ";
                        sql.CommandText += "and(convert(varchar(10), rfqOEMID) = @oem or @oem = 'Any') and(convert(varchar(10), rfqProgramID) = @program or @program = 'All') and(CONVERT(varchar(10), squSalesmanID) = @salesman or @salesman = 'Any') ";
                        sql.CommandText += "and(convert(varchar(10), squEstimatorID) = @estimator or squEstimatorID is null or @estimator = 'Any') and(convert(varchar(10), squSTSQuoteID) = @quoteID or @quoteID is null or @quoteID = squQuoteNumber) ";
                        sql.CommandText += "and(CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) and squCreated >= @start and squCreated <= @end and TSGSalesman.TSGSalesmanID = squSalesmanID and (CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) ";
                        sql.CommandText += "and (squPartName like @partName or @partName is null) and (squQuoteVersion = @version or @version is null) ";
                        if (cbDisposition.Checked)
                        {
                            sql.CommandText += "and squWinLossID is null and (squStatusID = 7 or squStatusID = 8) ";
                        }
                        sql.CommandText += "order by rfqID desc ";

                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                        sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                        sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);

                        if (txtQuoteNumber.Text != "")
                        {
                            string[] arr = txtQuoteNumber.Text.Split('-');
                            if (txtQuoteNumber.Text.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                sql.Parameters.AddWithValue("@oldQuoteNum", "asdadfasd");
                                if (arr.Length >= 4)
                                {
                                    sql.Parameters.AddWithValue("@version", arr[3]);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@version", DBNull.Value);
                                }
                            }
                            else if (arr.Length >= 4)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                            }
                            else if (arr.Length >= 2)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                sql.Parameters.AddWithValue("@oldQuoteNum", arr[0] + "-" + arr[1]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);

                            }
                            else if (arr.Length == 1 && txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                //We either match the old quote num perfectly or we wont something that deffinitely wont show up
                                sql.Parameters.AddWithValue("@oldQuoteNum", "asdadfasd");
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else
                            {
                                if (txtRFQ.Text == "")
                                {
                                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.ToString());
                                }
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                sql.Parameters.AddWithValue("@oldQuoteNum", "asdfasdfasdf");
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                        }
                        else
                        {
                            if (txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                            }
                            sql.Parameters.AddWithValue("@oldQuoteNum", "asdfasdfasdf");
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }

                        if (txtCustomerRFQ.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                        }
                        if (txtCustomer.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customer", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                        }
                        if (txtCustomerLocation.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                        }
                        if (txtPartNumber.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                        }
                        if (txtPartName.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partName", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                        }
                        sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);
                        sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);

                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            quoteInfo qi = new quoteInfo();

                            if ((userCompanyAbbrev == "STS") || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                            {
                                qi.quoteID = dr["squSTSQuoteID"].ToString();
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + qi.quoteID;

                                qi.price = dr["cost"].ToString();
                                if (qi.price == "")
                                {
                                    qi.price = "0";
                                }
                                total += System.Convert.ToDouble(qi.price);
                                qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }

                            int num;
                            bool result = Int32.TryParse(dr["squQuoteNumber"].ToString(), out num);
                            if (dr["rfqID"].ToString() == "")
                            {
                                if (dr["squQuoteNumber"].ToString() != "")
                                {
                                    qi.quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                                else
                                {
                                    qi.quoteNumber = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                            }
                            else if (!result)
                            {
                                qi.quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                            else
                            {
                                qi.quoteNumber = dr["quoteNumber"].ToString();
                            }
                            qi.rfqID = dr["rfqID"].ToString();
                            qi.rfqLink = "Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + qi.rfqID;
                            qi.customer = dr["CustomerName"].ToString();
                            qi.customerLocation = dr["ShipToName"].ToString();
                            qi.estimator = dr["estimator"].ToString();
                            qi.quoteStatus = dr["qstQuoteStatusDescription"].ToString();
                            qi.dieType = "";
                            qi.cavity = "";
                            qi.created = dr["squCreated"].ToString();
                            qi.sent = dr["qtrSent"].ToString();
                            qi.customerContact = dr["squCustomerContact"].ToString();
                            qi.customerRFQNum = dr["squCustomerRFQNum"].ToString();
                            qi.partNumber = dr["squPartNumber"].ToString();
                            qi.partDescription = dr["squPartName"].ToString();
                            qi.salesman = dr["salesmanName"].ToString();

                            qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-STS-13', '";
                            qi.dispositionButton += dr["squWinLossID"].ToString() + "', '" + dr["squWinLossReasonID"].ToString() + "', '" + dr["squPONumber"].ToString() + "', '" + dr["squAwardedAmount"].ToString() + "', '" + dr["squTargetPrice"].ToString() + "', '";
                            qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["squDispositionNote"].ToString()) + "');return false;\" >";
                            quoteList.Add(qi);
                        }
                        dr.Close();

                        sql.CommandText = "Select squSTSQuoteID, concat(rfqID, '-A', assLineNumber, '-STS-', squQuoteVersion) as quoteNumber, squRFQNum, Customer.CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription,   ";
                        sql.CommandText += "(select sum(pwnCostNote) from linkPWNToSTSQuote, pktblPreWordedNote where psqPreWordedNoteID = pwnPreWordedNoteID and psqSTSQuoteID = squSTSQuoteID) as cost, squProcess, '',   ";
                        sql.CommandText += "squCreated, squCustomerContact, squCustomerRFQNum, qtrQuoteToRFQID, squPartNumber, squPartName, TSGSalesman.Name as salesmanName, squQuoteNumber, squQuoteVersion, rfqID, qtrSent,  ";
                        sql.CommandText += "squWinLossID, squWinLossReasonID, squPONumber, squAwardedAmount, squTargetPrice, squDispositionNote ";
                        sql.CommandText += "from Customer, CustomerLocation, pktblEstimators, pktblQuoteStatus, TSGSalesman, tblSTSQuote ";
                        sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = squSTSQuoteID and qtrSTS = 1 ";
                        sql.CommandText += "left outer join tblRFQ on rfqID = qtrRFQID ";
                        sql.CommandText += "left outer join linkAssemblyToQuote on atqQuoteId = squSTSQuoteID and atqSTS = 1 ";
                        sql.CommandText += "left outer join tblAssembly on assAssemblyId = atqAssemblyId ";
                        sql.CommandText += "where Customer.CustomerID = squCustomerID and CustomerLocationID = squPlantID and squEstimatorID = estEstimatorID and squStatusID = qstQuoteStatusID ";
                        sql.CommandText += "and(13 = 1 or 1 = 1) and(convert(varchar(10), squSTSQuoteID) = @status or @status = 'All') ";
                        sql.CommandText += "and(squRFQNum = @rfq or @rfq is null) and(squCustomerRFQNum like @customerRFQ or @customerRFQ is null) and(squPartNumber like @partNum or @partNum is null) and(Customer.CustomerName like @customer or @customer is null) ";
                        sql.CommandText += "and(convert(varchar(10), rfqOEMID) = @oem or @oem = 'Any') and(convert(varchar(10), rfqProgramID) = @program or @program = 'All') and(CONVERT(varchar(10), squSalesmanID) = @salesman or @salesman = 'Any') ";
                        sql.CommandText += "and(convert(varchar(10), squEstimatorID) = @estimator or squEstimatorID is null or @estimator = 'Any') and(convert(varchar(10), squSTSQuoteID) = @quoteID or @quoteID is null or @quoteID = squQuoteNumber) ";
                        sql.CommandText += "and(CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) and squCreated >= @start and squCreated <= @end and TSGSalesman.TSGSalesmanID = squSalesmanID and(CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) ";
                        sql.CommandText += "and(squPartName like @partName or @partName is null) and(squQuoteVersion = @version or @version is null) ";
                        if (cbDisposition.Checked)
                        {
                            sql.CommandText += "and squWinLossID is null and (squStatusID = 7 or squStatusID = 8) ";
                        }
                        sql.CommandText += "order by rfqID desc ";

                        dr = sql.ExecuteReader();
                        string rfqID = "";
                        while (dr.Read())
                        {
                            quoteInfo qi = new quoteInfo();

                            if ((userCompanyAbbrev == "STS") || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                            {
                                qi.quoteID = dr["squSTSQuoteID"].ToString();
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/STSEditQuote?id=" + qi.quoteID;

                                qi.price = dr["cost"].ToString();
                                if (qi.price == "")
                                {
                                    qi.price = "0";
                                }
                                total += System.Convert.ToDouble(qi.price);
                                qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }

                            int num;
                            bool result = Int32.TryParse(dr["squQuoteNumber"].ToString(), out num);
                            if (dr["rfqID"].ToString() == "")
                            {
                                if (dr["squQuoteNumber"].ToString() != "")
                                {
                                    qi.quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                                else
                                {
                                    qi.quoteNumber = dr["squSTSQuoteID"].ToString() + "-STS-SA-" + dr["squQuoteVersion"].ToString();
                                }
                            }
                            else if (!result)
                            {
                                qi.quoteNumber = dr["squQuoteNumber"].ToString() + "-STS-" + dr["squQuoteVersion"].ToString();
                            }
                            else
                            {
                                qi.quoteNumber = dr["quoteNumber"].ToString();
                            }
                            qi.rfqID = dr["rfqID"].ToString();
                            qi.rfqLink = "Https://tsgrfq.azurewebsites.net/EditRFQ?id=" + qi.rfqID;
                            qi.customer = dr["CustomerName"].ToString();
                            qi.customerLocation = dr["ShipToName"].ToString();
                            qi.estimator = dr["estimator"].ToString();
                            qi.quoteStatus = dr["qstQuoteStatusDescription"].ToString();
                            qi.dieType = "";
                            qi.cavity = "";
                            qi.created = dr["squCreated"].ToString();
                            qi.sent = dr["qtrSent"].ToString();
                            qi.customerContact = dr["squCustomerContact"].ToString();
                            qi.customerRFQNum = dr["squCustomerRFQNum"].ToString();
                            qi.partNumber = dr["squPartNumber"].ToString();
                            qi.partDescription = dr["squPartName"].ToString();
                            qi.salesman = dr["salesmanName"].ToString();

                            qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-STS-13', '";
                            qi.dispositionButton += dr["squWinLossID"].ToString() + "', '" + dr["squWinLossReasonID"].ToString() + "', '" + dr["squPONumber"].ToString() + "', '" + dr["squAwardedAmount"].ToString() + "', '" + dr["squTargetPrice"].ToString() + "', '";
                            qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["squDispositionNote"].ToString()) + "');return false;\" >";
                            quoteList.Add(qi);
                        }
                        dr.Close();
                    }
                }



                if ((ddlQuoteType.SelectedValue == "All" || ddlQuoteType.SelectedValue == "Stand Alone") && ddlProgram.SelectedValue == "All")
                {
                    string company = containsCompany(txtQuoteNumber.Text);
                    if (company != "9" && company != "13" && company != "15")
                    {
                        sql.CommandText = "Select ecqECQuoteID, TSGCompanyAbbrev, ecqRFQNumber, Customer.CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription, ";
                        sql.CommandText += "(Select sum(pwnCostNote) from linkPWNToECQuote, pktblPreWordedNote where peqECQuoteID = ecqECQuoteID and peqPreWordedNoteID = pwnPreWordedNoteID) as cost, DieType.dtyFullName, cavCavityName, ecqCreated, ecqCustomerContactName, ";
                        sql.CommandText += "ecqCustomerRFQNumber, ecqPartNumber, ecqPartName, TSGSalesman.Name, ecqQuoteNumber, ecqVersion, TSGCompany.TSGCompanyID as companyID, ecqWinLossID, ecqWinLossReasonID, ecqPONumber, ecqAwardedAmount, ecqTargetPrice, ecqDispositionNote ";
                        sql.CommandText += "from tblECQuote, TSGCompany, Customer, CustomerLocation, pktblEstimators, pktblQuoteStatus, DieType, pktblCavity, TSGSalesman ";
                        sql.CommandText += "where TSGCompany.TSGCompanyID = ecqTSGCompanyID and ecqCustomer = Customer.CustomerID and ecqCustomerLocation = CustomerLocationID and ecqEstimator = estEstimatorID and qstQuoteStatusID = ecqStatus ";
                        sql.CommandText += "and ecqDieType = DieTypeID and ecqCavity = cavCavityID and(Customer.CustomerName like @customer or @customer is null) and (ecqTSGCompanyID = @company or @company = 1) and ((convert(varchar(10), ecqECQuoteID) = @quoteID or @quoteID is null) or ((convert(varchar(10), ecqQuoteNumber) = @quoteID or @quoteID is null))) ";
                        sql.CommandText += "and (convert(varchar(10), ecqStatus) = @status or @status = 'All') and(ecqCustomerRFQNumber like @customerRFQ or @customerRFQ is null) and(ecqPartNumber like @partNum or @partNum is null) and (CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                        sql.CommandText += "and (CONVERT(varchar(10), ecqSalesmanID) = @salesman or @salesman = 'Any') and (convert(varchar(10), ecqEstimator) = @estimator or ecqEstimator is null or @estimator = 'Any') and TSGSalesman.TSGSalesmanID = ecqSalesmanID ";
                        sql.CommandText += "and (ecqPartName like @partName or @partName is null) and ecqCreated >= @start and ecqCreated <= @end and (ecqVersion = @version or @version is null) and (TSGCompanyAbbrev = @abbrev or @abbrev is null) ";
                        if (cbDisposition.Checked)
                        {
                            sql.CommandText += "and ecqWinLossID is null and (ecqStatus = 7 or ecqStatus = 8) ";
                        }
                        sql.Parameters.Clear();
                        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                        sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        //sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                        sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));


                        if (txtQuoteNumber.Text != "")
                        {
                            string[] arr = txtQuoteNumber.Text.Split('-');
                            sql.Parameters.AddWithValue("@quoteID", arr[0]);
                            if (arr.Length >= 4)
                            {
                                sql.Parameters.AddWithValue("@abbrev", arr[1]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                            }
                            else if (arr.Length >= 2)
                            {
                                sql.Parameters.AddWithValue("@abbrev", arr[1]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                            //sql.Parameters.AddWithValue("@rfq")
                        }
                        if (txtCustomerRFQ.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                        }
                        if (txtCustomer.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customer", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                        }
                        if (txtCustomerLocation.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                        }
                        if (txtPartNumber.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                        }
                        if (txtPartName.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partName", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                        }

                        //sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                        sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);
                        sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);


                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            quoteInfo qi = new quoteInfo();

                            if (dr.GetValue(16).ToString() == "")
                            {
                                qi.quoteID = dr.GetValue(0).ToString();
                            }
                            else
                            {
                                qi.quoteID = dr.GetValue(0).ToString();
                            }

                            if ((dr["TSGCompanyAbbrev"].ToString() == userCompanyAbbrev) || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                            {
                                //                                qi.quoteID = dr["squSTSQuoteID"].ToString();
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/EditQuote?id=" + dr.GetValue(0).ToString() + "&quoteType=1";

                                qi.price = dr.GetValue(7).ToString();
                                if (qi.price == "")
                                {
                                    qi.price = "0";
                                }
                                total += System.Convert.ToDouble(qi.price);
                                qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }

                            if (dr["ecqQuoteNumber"].ToString() != "")
                            {
                                qi.quoteNumber = dr["ecqQuoteNumber"] + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["ecqVersion"].ToString();
                            }
                            else
                            {
                                qi.quoteNumber = qi.quoteID + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["ecqVersion"].ToString();
                            }
                            qi.rfqID = dr.GetValue(2).ToString();
                            qi.rfqLink = "";
                            qi.customer = dr.GetValue(3).ToString();
                            qi.customerLocation = dr.GetValue(4).ToString();
                            qi.estimator = dr.GetValue(5).ToString();
                            qi.quoteStatus = dr.GetValue(6).ToString();
                            qi.dieType = dr.GetValue(8).ToString();
                            qi.cavity = dr.GetValue(9).ToString();
                            qi.created = dr.GetValue(10).ToString();
                            qi.sent = "";
                            qi.customerContact = dr.GetValue(11).ToString();
                            qi.customerRFQNum = dr.GetValue(12).ToString();
                            qi.partNumber = dr.GetValue(13).ToString();
                            qi.partDescription = dr.GetValue(14).ToString();
                            qi.salesman = dr.GetValue(15).ToString();

                            qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-SA-" + dr["companyID"].ToString() + "', '";
                            qi.dispositionButton += dr["ecqWinLossID"].ToString() + "', '" + dr["ecqWinLossReasonID"].ToString() + "', '" + dr["ecqPONumber"].ToString() + "', '" + dr["ecqAwardedAmount"].ToString() + "', '" + dr["ecqTargetPrice"].ToString() + "', '";
                            qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["ecqDispositionNote"].ToString()) + "');return false;\" >";

                            quoteList.Add(qi);
                        }
                        dr.Close();
                    }
                }

                if (ddlQuoteType.SelectedValue == "All" || ddlQuoteType.SelectedValue == "HTS")
                {
                    string company = containsCompany(txtQuoteNumber.Text);
                    if (company == "" || company == "9")
                    {
                        sql.CommandText = "Select hquHTSQuoteID, 'HTS', hquRFQID, Customer.CustomerName, ShipToName, concat(estFirstName, ' ', estLastName) as estimator, qstQuoteStatusDescription, ";
                        sql.CommandText += "(select sum(hpwQuantity * hpwUnitPrice) from linkHTSPWNToHTSQuote, pktblHTSPreWordedNote where pthHTSQuoteID = hquHTSQuoteID and hpwHTSPreWordedNoteID = pthHTSPWNID), dtyFullName, cavCavityName, ";
                        sql.CommandText += "hquCreated, hquCustomerContactName, hquCustomerRFQNum, qtrQuoteToRFQID, prtPARTID, hquPartNumbers, hquPartName, TSGSalesman.Name, hquNumber, hquVersion, hquWinLossID, hquWinLossReasonID, hquPONumber, hquAwardedAmount, hquTargetPrice, hquDispositionNote, rfqID, prtRFQLineNumber ";
                        sql.CommandText += "from Customer, CustomerLocation, pktblEstimators, pktblQuoteStatus, DieType, pktblCavity, TSGSalesman, tblHTSQuote ";
                        sql.CommandText += "left outer join linkQuoteToRFQ on qtrQuoteID = hquHTSQuoteID and qtrHTS = 1 ";
                        sql.CommandText += "left outer join linkPartToQuote on ptqQuoteID = hquHTSQuoteID and ptqHTS = 1 ";
                        sql.CommandText += "left outer join tblPart on ptqPartID = prtPARTID ";
                        sql.CommandText += "left outer join tblRFQ on qtrRFQID = rfqID ";
                        sql.CommandText += "where Customer.CustomerID = hquCustomerID and CustomerLocationID = hquCustomerLocationID and hquEstimatorID = estEstimatorID and hquStatusID = qstQuoteStatusID ";
                        sql.CommandText += "and DieTypeID = hquProcess and hquCavity = cavCavityID and (9 = @company or @company = 1) and(convert(varchar(10), hquStatusID) = @status or @status = 'All') "; 
                        sql.CommandText += "and(hquRFQID = @rfq or @rfq is null) and(hquCustomerRFQNum like @customerRFQ or @customerRFQ is null) and(hquPartNumbers like @partNum or @partNum is null) and(Customer.CustomerName like @customer or @customer is null) ";
                        sql.CommandText += "and(convert(varchar(10), hquOEM) = @oem or @oem = 'Any') and(convert(varchar(10), hquProgramCodeID) = @program or @program = 'All') and(CONVERT(varchar(10), hquSalesman) = @salesman or @salesman = 'Any') ";
                        sql.CommandText += "and(convert(varchar(10), hquEstimatorID) = @estimator or hquEstimatorID is null or @estimator = 'Any')  and (CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                        sql.CommandText += "and hquCreated >= @start and hquCreated <= @end and TSGSalesman.TSGSalesmanID = hquSalesman and (hquPartName like @partName or @partName is null) and (hquVersion = @version or @version is null) ";
                        sql.CommandText += "and (((CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) and (prtRFQLineNumber like @lineNum or @lineNum is null)) and (convert(varchar(10), hquHTSQuoteID) = @quoteID or @quoteID is null or @quoteID = hquNumber)) ";
                        if (cbDisposition.Checked)
                        {
                            sql.CommandText += "and hquWinLossID is null and (hquStatusID = 7 or hquStatusID = 8) ";
                        }
                        sql.Parameters.Clear();

                        sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                        sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                        sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                        sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));



                        if (txtQuoteNumber.Text != "")
                        {
                            string[] arr = txtQuoteNumber.Text.Split('-');
                            if (txtQuoteNumber.Text.ToLower().Contains("sa"))
                            {
                                sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                if (arr.Length > 3)
                                {
                                    sql.Parameters.AddWithValue("@version", arr[3]);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@version", DBNull.Value);
                                }
                            }
                            else if (arr.Length >= 4)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                //sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", arr[3]);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            }
                            else if (arr.Length == 3)
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                //sql.Parameters.AddWithValue("@abbrev", arr[2]);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            }
                            else if (arr.Length == 2)
                            {
                                if (arr[1].ToLower() != "hts")
                                {
                                    sql.Parameters.AddWithValue("@rfq", arr[0]);
                                    sql.Parameters.AddWithValue("@lineNum", arr[1]);
                                    sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@quoteID", arr[0]);
                                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                    sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                }
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else if (arr.Length == 1 && txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", arr[0]);
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                //We either match the old quote num perfectly or we wont something that deffinitely wont show up
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                            else
                            {
                                if (txtRFQ.Text == "")
                                {
                                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                                }
                                else
                                {
                                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.ToString());
                                }
                                sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                                sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                                //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                                sql.Parameters.AddWithValue("@version", DBNull.Value);
                            }
                        }
                        else
                        {
                            if (txtRFQ.Text == "")
                            {
                                sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                            }
                            sql.Parameters.AddWithValue("@lineNum", DBNull.Value);
                            sql.Parameters.AddWithValue("@quoteID", DBNull.Value);
                            //sql.Parameters.AddWithValue("@abbrev", DBNull.Value);
                            sql.Parameters.AddWithValue("@version", DBNull.Value);
                        }

                        if (txtCustomerRFQ.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                        }
                        if (txtCustomer.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customer", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                        }
                        if (txtCustomerLocation.Text == "")
                        {
                            sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                        }
                        if (txtPartNumber.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                        }
                        if (txtPartName.Text == "")
                        {
                            sql.Parameters.AddWithValue("@partName", DBNull.Value);
                        }
                        else
                        {
                            sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                        }

                        sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                        sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);

                        sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);

                        dr = sql.ExecuteReader();
                        while (dr.Read())
                        {
                            quoteInfo qi = new quoteInfo();

                            if ((userCompanyAbbrev == "HTS") || (userCompanyAbbrev == "TSG") || (userCompanyAbbrev == "UGS"))
                            {
                                qi.quoteID = dr.GetValue(0).ToString();
                                qi.quoteLink = "Https://tsgrfq.azurewebsites.net/HTSEditQuote?id=" + dr.GetValue(0).ToString() + "&rfq=" + dr.GetValue(2).ToString() + "&partID=" + dr.GetValue(14).ToString();

                                qi.price = dr.GetValue(7).ToString();
                                try
                                {
                                    total += System.Convert.ToDouble(qi.price);
                                    qi.price = System.Convert.ToDouble(qi.price).ToString("C");
                                }
                                catch
                                {

                                }
                            }
                            else
                            {
                                qi.quoteLink = "";
                                qi.quoteID = "";
                                qi.price = "NA";
                                total += 0;
                            }

                            if (dr["rfqID"].ToString() == "")
                            {
                                if (dr["hquNumber"].ToString() != "")
                                {
                                    qi.quoteNumber = dr["hquNumber"].ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["hquVersion"].ToString();
                                }
                                else
                                {
                                    qi.quoteNumber = qi.quoteID + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["hquVersion"].ToString();
                                }
                            }
                            else
                            {
                                qi.quoteNumber = dr["rfqID"].ToString() + "-" + dr["prtRFQLineNumber"].ToString() + "-HTS-" + dr["hquVersion"].ToString();
                            }

                            //if (dr["hquNumber"].ToString() != "")
                            //{
                            //    qi.quoteNumber = dr["hquNumber"].ToString() + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["hquVersion"].ToString();
                            //}
                            //else
                            //{
                            //    qi.quoteNumber = qi.quoteID + "-" + dr.GetValue(1).ToString() + "-SA-" + dr["hquVersion"].ToString();
                            //}
                            qi.rfqID = dr.GetValue(2).ToString();
                            qi.rfqLink = "";
                            qi.customer = dr.GetValue(3).ToString();
                            qi.customerLocation = dr.GetValue(4).ToString();
                            qi.estimator = dr.GetValue(5).ToString();
                            qi.quoteStatus = dr.GetValue(6).ToString();
                            qi.dieType = dr.GetValue(8).ToString();
                            qi.cavity = dr.GetValue(9).ToString();
                            qi.created = dr.GetValue(10).ToString();
                            qi.sent = "";
                            qi.customerContact = dr.GetValue(11).ToString();
                            qi.customerRFQNum = dr.GetValue(12).ToString();
                            qi.partNumber = dr.GetValue(13).ToString();
                            qi.partDescription = dr.GetValue(14).ToString();
                            qi.salesman = dr.GetValue(16).ToString();

                            qi.dispositionButton = "<input type='button' class='mybutton' value='Set Disposition'  onClick=\"showDisposition('" + dr.GetValue(0).ToString() + "-HTS-9', '";
                            qi.dispositionButton += dr["hquWinLossID"].ToString() + "', '" + dr["hquWinLossReasonID"].ToString() + "', '" + dr["hquPONumber"].ToString() + "', '" + dr["hquAwardedAmount"].ToString() + "', '" + dr["hquTargetPrice"].ToString() + "', '";
                            qi.dispositionButton += HttpUtility.JavaScriptStringEncode(dr["hquDispositionNote"].ToString()) + "');return false;\" >";
                            quoteList.Add(qi);
                        }
                        dr.Close();
                    }
                }

                if (ddlQuoteType.SelectedValue == "All" || ddlQuoteType.SelectedValue == "Mas")
                {                    
                    //sql.CommandText = "Select distinct qhiQuoteHistoryID, qhiCustomerRFQNumber as quoteNumber, qhiSalesOrderNo, qhiBillToName, qhiShipToName, qhiEstimator, 'MAS History' as quoteStatus, qhiNonTaxableAmt, qhiToolType, qhiCavity, qhiDateCreated, ";
                    //sql.CommandText += "qhiCustomerRfqNum, qhiPartNumber, qhiPartDescription ";
                    //sql.CommandText += "from tblQuoteHistory ";
                    //sql.CommandText += "where (qhiBillToName like @customer or @customer is null) and (qhiSalesOrderNo = @quote or @quote is null) and (qhiCustomerRFQNum like @customerRFQ or @customerRFQ is null) ";
                    //sql.CommandText += "and (qhiPartNumber like @partNum or @partNum is null) and (qhiOEM = @oem or @oem = 'Any') and (convert(varchar(10), qhiProgramID) = @program or @program = 'All') ";
                    //sql.CommandText += "and (qhiEstimator = @estimator or @estimator = 'Any') and (qhiGroupCompany = @company or @company = 'TSG') and (qhiPartDescription like @partName or @partName is null) ";
                    //sql.CommandText += "and qhiDateCreated >= @start and qhiDateCreated <= @end and (qhiShipToName like @customerLocation or @customerLocation is null) ";
                    //sql.CommandText += "order by qhiSalesOrderNo desc";


                    //sql.Parameters.Clear();

                    //sql.Parameters.AddWithValue("@company", ddlCompany.SelectedItem.Text);
                    //sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                    //sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                    //sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));

                    //if (txtQuoteNumber.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@quote", DBNull.Value);
                    //}
                    //else
                    //{
                    //    sql.Parameters.AddWithValue("@quote", txtQuoteNumber.Text.Split('-')[0]);
                    //    //sql.Parameters.AddWithValue("@rfq")
                    //}
                    //if (txtCustomerRFQ.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                    //}
                    //else 
                    //{
                    //    sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                    //}
                    //if (txtCustomer.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@customer", DBNull.Value);
                    //}
                    //else 
                    //{
                    //    sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                    //}
                    //if (txtCustomerLocation.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                    //}
                    //else 
                    //{
                    //    sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                    //}
                    //if (txtPartNumber.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                    //}
                    //else 
                    //{
                    //    sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                    //}
                    //if (txtPartName.Text == "")
                    //{
                    //    sql.Parameters.AddWithValue("@partName", DBNull.Value);
                    //}
                    //else
                    //{
                    //    sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                    //}

                    //sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedItem.Text);

                    //if(ddlEstimator.SelectedValue != "Any")
                    //{
                    //    try
                    //    {
                    //        sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedItem.Text.Split(' ')[1]);
                    //    }
                    //    catch
                    //    {
                    //        sql.Parameters.AddWithValue("@estimator", "Any");
                    //    }
                    //}
                    //else
                    //{
                    //    sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);
                    //}

                    //int count = quoteList.Count;

                    //dr = sql.ExecuteReader();
                    //while (dr.Read())
                    //{
                    //    quoteInfo qi = new quoteInfo();

                    //    qi.quoteID = dr.GetValue(0).ToString();
                    //    qi.quoteLink = "https://tsgrfq.azurewebsites.net/EditQuote?id=" + dr.GetValue(0).ToString() + "&quoteType=2&quoteNumber=" + dr.GetValue(1).ToString();
                    //    qi.quoteNumber = dr.GetValue(1).ToString();
                    //    qi.rfqID = dr.GetValue(2).ToString();
                    //    qi.rfqLink = "";
                    //    qi.customer = dr.GetValue(3).ToString();
                    //    qi.customerLocation = dr.GetValue(4).ToString();
                    //    qi.estimator = dr.GetValue(5).ToString();
                    //    qi.quoteStatus = dr.GetValue(6).ToString();
                    //    qi.price = dr.GetValue(7).ToString();
                    //    qi.dieType = dr.GetValue(8).ToString();
                    //    qi.cavity = dr.GetValue(9).ToString();
                    //    qi.created = dr.GetValue(10).ToString();
                    //    qi.sent = "";
                    //    qi.customerContact = "";
                    //    qi.customerRFQNum = dr.GetValue(11).ToString();
                    //    qi.partNumber = dr.GetValue(12).ToString();
                    //    qi.partDescription = dr.GetValue(13).ToString();

                    //    total += System.Convert.ToDouble(qi.price);
                    //    qi.price = System.Convert.ToDouble(qi.price).ToString("C");


                    //    quoteList.Add(qi);
                    //}
                    //dr.Close();
                }

                lbltotalCost.Text += "Total Amount Quoted: " + String.Format("{0:C}", total);

                dgQuote.DataSource = quoteList;
                dgQuote.DataBind();
                dgQuote.Visible = true;

            }
            else
            {
                dgQuote.Visible = false;
                lbltotalCost.Visible = false;
                lblTruncated.Visible = false;
            }
            


            if(cbPart.Checked)
            {
                List<Unreserved> UnreservedList = new List<Unreserved>();

                sql.CommandText = "Select rfqID, prtPartNumber, prtPARTID, CustomerName, prtCreated, rfqDueDate, prtPicture, ShipToName, prtPartLength, prtPartWidth, prtPartHeight ";
                sql.CommandText += "from tblPart, linkPartToRFQ, tblRFQ, Customer, CustomerLocation, pktblRFQStatus ";
                sql.CommandText += "where rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocation.CustomerLocationID and ptrPartID = prtPARTID and ptrRFQID = rfqID and Customer.CustomerID = CustomerLocation.CustomerID ";
                sql.CommandText += "and ptrPartID = prtPARTID and ptrRFQID = rfqID and Customer.CustomerID = CustomerLocation.CustomerID and(Customer.CustomerName like @customer or @customer is null) ";
                sql.CommandText += "and(prtPartNumber like @partNum or @partNum is null) and(rfqCustomerRFQNumber like @customerRFQ or @customerRFQ is null) and(CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) ";
                sql.CommandText += "and(CONVERT(varchar(10), rstRFQStatusID) = @rfqStatus or @rfqStatus = 'All') and(convert(varchar(10), rfqOEMID) = @oem or @oem = 'Any') ";
                sql.CommandText += "and(convert(varchar(10), rfqProgramID) = @program or @program = 'All') and rstRFQStatusID = rfqStatus and (CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                sql.CommandText += "and rfqDateReceived >= @start and rfqDateReceived <= @end and (prtpartDescription like @partName or @partName is null) ";
                sql.CommandText += "order by rfqID ";
                sql.Parameters.Clear();

                sql.Parameters.AddWithValue("@company", ddlCompany.SelectedValue);
                sql.Parameters.AddWithValue("@rfqStatus", ddlRFQStatus.SelectedValue);
                sql.Parameters.AddWithValue("@program", ddlProgram.SelectedValue);
                sql.Parameters.AddWithValue("@start", System.Convert.ToDateTime(txtStart.Text).ToString("d"));
                sql.Parameters.AddWithValue("@end", System.Convert.ToDateTime(txtEnd.Text).ToString("d"));


                if (txtRFQ.Text == "")
                {
                    sql.Parameters.AddWithValue("@rfq", DBNull.Value);
                    //sql.Parameters.AddWithValue("@line", )
                }
                else
                {
                    sql.Parameters.AddWithValue("@rfq", txtRFQ.Text.Trim());
                }
                if (txtCustomerRFQ.Text == "")
                {
                    sql.Parameters.AddWithValue("@customerRFQ", DBNull.Value);
                }
                else 
                {
                    sql.Parameters.AddWithValue("@customerRFQ", sanatise(txtCustomerRFQ.Text, ddlCustomerRFQ.SelectedValue));
                }
                if (txtCustomerLocation.Text == "")
                {
                    sql.Parameters.AddWithValue("@customerLocation", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@customerLocation", sanatise(txtCustomerLocation.Text, ddlCustomerLocation.SelectedValue));
                }
                if (txtCustomer.Text == "")
                {
                    sql.Parameters.AddWithValue("@customer", DBNull.Value);
                }
                else 
                {
                    sql.Parameters.AddWithValue("@customer", sanatise(txtCustomer.Text, ddlcustomerSearch.SelectedValue));
                }
                if (txtPartNumber.Text == "")
                {
                    sql.Parameters.AddWithValue("@partNum", DBNull.Value);
                }
                else 
                {
                    sql.Parameters.AddWithValue("@partNum", sanatise(txtPartNumber.Text, ddlPartNumber.SelectedValue));
                }
                string test = sanatise(txtPartName.Text, ddlPartName.SelectedValue);
                if (txtPartName.Text == "")
                {
                    sql.Parameters.AddWithValue("@partName", DBNull.Value);
                }
                else
                {
                    sql.Parameters.AddWithValue("@partName", sanatise(txtPartName.Text, ddlPartName.SelectedValue));
                }

                sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                sql.Parameters.AddWithValue("@salesman", ddlSalesman.SelectedValue);

                sql.Parameters.AddWithValue("@estimator", ddlEstimator.SelectedValue);

                
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Unreserved unres = new Unreserved();
                    unres.rfqID = dr.GetValue(0).ToString();
                    unres.partNumber = dr.GetValue(1).ToString();
                    unres.partID = dr.GetValue(2).ToString();
                    unres.customer = dr.GetValue(3).ToString();
                    unres.created = System.Convert.ToDateTime(dr.GetValue(4)).ToString("d");
                    unres.dueDate = System.Convert.ToDateTime(dr.GetValue(5)).ToString("d");
                    unres.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(6).ToString() + "'>Picture";
                    unres.plant = dr.GetValue(7).ToString();
                    unres.partLength = dr.GetValue(8).ToString();
                    unres.partWidth = dr.GetValue(9).ToString();
                    unres.partHeight = dr.GetValue(10).ToString();
                    UnreservedList.Add(unres);
                }
                dr.Close();

                sql.CommandText = "Select rfqID, assNumber, assAssemblyId, CustomerName, assCreated, rfqDueDate, assPicture, ShipToName, 0, 0, 0  ";
                sql.CommandText += "from tblAssembly, linkAssemblyToRFQ, tblRFQ, Customer, CustomerLocation, pktblRFQStatus ";
                sql.CommandText += "where rfqCustomerID = Customer.CustomerID and rfqPlantID = CustomerLocation.CustomerLocationID and atrAssemblyId = assAssemblyId and atrRfqId = rfqID and Customer.CustomerID = CustomerLocation.CustomerID ";
                sql.CommandText += "and Customer.CustomerID = CustomerLocation.CustomerID and(Customer.CustomerName like @customer or @customer is null) ";
                sql.CommandText += "and(assNumber like @partNum or @partNum is null) and(rfqCustomerRFQNumber like @customerRFQ or @customerRFQ is null) and(CONVERT(varchar(10), rfqID) = @rfq or @rfq is null) ";
                sql.CommandText += "and(CONVERT(varchar(10), rstRFQStatusID) = @rfqStatus or @rfqStatus = 'All') and(convert(varchar(10), rfqOEMID) = @oem or @oem = 'Any') ";
                sql.CommandText += "and(convert(varchar(10), rfqProgramID) = @program or @program = 'All') and rstRFQStatusID = rfqStatus and(CustomerLocation.ShipToName like @customerLocation or @customerLocation is null) ";
                sql.CommandText += "and rfqDateReceived >= @start and rfqDateReceived <= @end and(assDescription like @partName or @partName is null) ";
                sql.CommandText += "order by rfqID ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    Unreserved unres = new Unreserved();
                    unres.rfqID = dr.GetValue(0).ToString();
                    unres.partNumber = dr.GetValue(1).ToString();
                    unres.partID = dr.GetValue(2).ToString();
                    unres.customer = dr.GetValue(3).ToString();
                    unres.created = System.Convert.ToDateTime(dr.GetValue(4)).ToString("d");
                    unres.dueDate = System.Convert.ToDateTime(dr.GetValue(5)).ToString("d");
                    unres.partPicture = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/part%20pictures/" + dr.GetValue(6).ToString() + "'>Picture";
                    unres.plant = dr.GetValue(7).ToString();
                    unres.partLength = dr.GetValue(8).ToString();
                    unres.partWidth = dr.GetValue(9).ToString();
                    unres.partHeight = dr.GetValue(10).ToString();
                    UnreservedList.Add(unres);
                }
                dr.Close();

                gvPart.DataSource = UnreservedList;
                gvPart.DataBind();
                gvPart.Visible = true;
            }
            else
            {
                gvPart.Visible = false;
            }
            btnExport.Visible = true;
            connection.Close();
        }

        protected string sanatise(string text, string filter)
        {
            if(filter == "Contains")
            {
                return "%" + text.Trim() + "%";
            }
            else if (filter == "Equals")
            {
                return text.Trim();
            }
            else if (filter == "Begins with")
            {
                return text.Trim() + "%";
            }
            else if (filter == "Ends with")
            {
                return "%" + text.Trim();
            }

            return "";
        }

        private string containsCompany(string text)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;
            string company = "";
            string[] arr = text.Split('-');
            
            try
            {
                //SA try and find company
                sql.CommandText = "Select TSGCompanyID from TSGCompany where TSGCompanyAbbrev = @abv ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@abv", arr[1]);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    company = dr.GetValue(0).ToString();
                }
                dr.Close();

                //RFQ quote try to find company
                if (company == "")
                {
                    sql.CommandText = "Select TSGCompanyID from TSGCompany where TSGCompanyAbbrev = @abv";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@abv", arr[2]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        company = dr.GetValue(0).ToString();
                    }
                    dr.Close();
                }
            }
            catch
            {

            }
            

            connection.Close();
            return company;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            // used to build autofilter cell range
            // extra zs so that it will not break with more than 26 columns
            string alphabet="ABCDEFGHIJKLMNOPQRSTUVWXYZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";

            XSSFWorkbook wb = new XSSFWorkbook();
            XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
            XSSFSheet rfqSheet = (XSSFSheet)wb.CreateSheet("RFQ");
            XSSFSheet quoteSheet = (XSSFSheet)wb.CreateSheet("Quote");
            XSSFSheet partSheet = (XSSFSheet)wb.CreateSheet("Part");
            NPOI.SS.UserModel.IRow rrow;
            XSSFFont titleFont = (XSSFFont)wb.CreateFont();
            titleFont.FontHeight = 12;
            titleFont.Boldweight = 700;
            titleFont.IsItalic = true;
            Int32 currentRow = 0;
            // RFQ Tab
            Int32 totalCols = 0;
            Int32 totalRows = dgResults.Rows.Count;
            GridViewRow headerRow = dgResults.HeaderRow;
            if (totalRows > 0)
            {
                totalCols = dgResults.Rows[0].Cells.Count;
                rrow = GetOrCreateRow(rfqSheet, currentRow);
                for (var i = 0; i < totalCols; i++)
                {
                    rrow.CreateCell(i).SetCellValue(headerRow.Cells[i].Text);
                    rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                }
                foreach (GridViewRow drow in dgResults.Rows)
                {
                    currentRow++;
                    rrow = GetOrCreateRow(rfqSheet, currentRow);
                    for (var i = 0; i < totalCols; i++)
                    {
                        if (drow.Cells[i].Text.Trim() == "")
                        {
                            try
                            {
                                HyperLink hfld = (HyperLink)drow.Cells[i].Controls[0];
                                rrow.CreateCell(i).SetCellValue(hfld.NavigateUrl.Split('=')[1].Split('&')[0]);
                            }
                            catch
                            {
                                rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                            }
                        }
                        else
                        {
                            rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                        }
                    }
                }
                rfqSheet.CreateFreezePane(1, 0);
                rfqSheet.ForceFormulaRecalculation = true;
                for (int i = 0; i < totalCols; i++)
                {
                    rfqSheet.AutoSizeColumn(i);
                }

                rfqSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("A1:" + alphabet[totalCols -1] + "1"));
            }

            currentRow = 0;
            // Quote Tab
            headerRow = dgQuote.HeaderRow;
            totalRows = dgQuote.Rows.Count;
            if (totalRows > 0)
            {
                totalCols = dgQuote.Rows[0].Cells.Count;
                rrow = GetOrCreateRow(quoteSheet, currentRow);
                for (var i = 0; i < totalCols; i++)
                {
                    rrow.CreateCell(i).SetCellValue(headerRow.Cells[i].Text);
                    rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                }
                foreach (GridViewRow drow in dgQuote.Rows)
                {
                    currentRow++;
                    rrow = GetOrCreateRow(quoteSheet, currentRow);
                    for (var i = 0; i < totalCols; i++)
                    {
                        if (drow.Cells[i].Text.Trim() == "")
                        {
                            try
                            {
                                HyperLink hfld = (HyperLink)drow.Cells[i].Controls[0];
                                rrow.CreateCell(i).SetCellValue(hfld.NavigateUrl.Split('=')[1].Split('&')[0]);
                            }
                            catch
                            {
                                rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                            }
                        }
                        else
                        {
                            rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                        }
                    }
                }
                quoteSheet.CreateFreezePane(1, 0);
                quoteSheet.ForceFormulaRecalculation = true;
                for (int i = 0; i < totalCols; i++)
                {
                    quoteSheet.AutoSizeColumn(i);
                }

                quoteSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("A1:" + alphabet[totalCols -1] + "1"));

            }

            currentRow = 0;
            // Part Tab
            headerRow = gvPart.HeaderRow;
            totalRows = gvPart.Rows.Count;
            if (totalRows > 0)
            {
                totalCols = gvPart.Rows[0].Cells.Count;
                rrow = GetOrCreateRow(partSheet, currentRow);
                for (var i = 0; i < totalCols; i++)
                {
                    rrow.CreateCell(i).SetCellValue(headerRow.Cells[i].Text);
                    rrow.GetCell(i).RichStringCellValue.ApplyFont(titleFont);
                }
                foreach (GridViewRow drow in gvPart.Rows)
                {
                    currentRow++;
                    rrow = GetOrCreateRow(partSheet, currentRow);
                    for (var i = 0; i < totalCols; i++)
                    {
                        if (drow.Cells[i].Text.Trim() == "")
                        {
                            try
                            {
                                HyperLink hfld = (HyperLink)drow.Cells[i].Controls[0];
                                rrow.CreateCell(i).SetCellValue(hfld.NavigateUrl.Split('=')[1].Split('&')[0]);
                            }
                            catch
                            {
                                rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                            }
                        }
                        else
                        {
                            rrow.CreateCell(i).SetCellValue(decode(drow.Cells[i].Text));
                        }
                    }
                }
                partSheet.CreateFreezePane(1, 0);
                partSheet.ForceFormulaRecalculation = true;
                for (int i = 0; i < totalCols; i++)
                {
                    partSheet.AutoSizeColumn(i);
                }

                partSheet.SetAutoFilter(NPOI.SS.Util.CellRangeAddress.ValueOf("A1:" + alphabet[totalCols - 1] + "1"));

            }

            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "TIMSSearchResults" + txtStart.Text.Replace("/", "-") + " to " + txtEnd.Text.Replace("/", "-") + ".xlsx"));
            Response.Clear();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            wb.Write(ms);
            Response.BinaryWrite(ms.ToArray());
            Response.End();


        }

        // wrote this because urldecode was not doing what I wanted
        public string decode(string value)
        {
            return value.Replace("&nbsp;", " ").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&gt;", ">").Replace("&lt;", "<");
        }

        public NPOI.SS.UserModel.IRow GetOrCreateRow(XSSFSheet referenceSheet, Int32 currentRow)
        {

            if (referenceSheet.GetRow(currentRow) == null)
            {
                return referenceSheet.CreateRow(currentRow);
            }
            else
            {
                return referenceSheet.GetRow(currentRow);
            }
        }
    }

    public class quoteInfo
    {
        public string quoteID { get; set; }
        public string quoteLink { get; set; }
        public string quoteNumber { get; set; }
        public string partNumber { get; set; }
        public string partDescription { get; set; }
        public string rfqID { get; set; }
        public string rfqLink { get; set; }
        public string customer { get; set; }
        public string customerLocation { get; set; }
        public string estimator { get; set; }
        public string quoteStatus { get; set; }
        public string price { get; set; }
        public string dieType { get; set; }
        public string cavity { get; set; }
        public string created { get; set; }
        public string sent { get; set; }
        public string customerContact { get; set; }
        public string customerRFQNum { get; set; }
        public string salesman { get; set; }
        public string dispositionButton { get; set; }
    }
}