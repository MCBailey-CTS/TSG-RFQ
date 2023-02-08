using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Security;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Security.Authentication;

namespace RFQ
{
    public partial class Site : MasterPage
    {
        public Boolean Testing = true;
        public Int64 UserID = 0;
        public Int64 UserCompanyID = 0;
        public Boolean CanAdmin = false;
        public Boolean CanEnterJobs = false;
        public String UserCompanyAbbrev = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            
            setGlobalVariables();
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            string UserName = getUserName();
            UserID = getUserID();
            if (!CanAdmin)
            {
                A8.Visible = false;
            }
            if (!IsPostBack)
            {
                A5.Visible = false;
                long userId = getUserID();
                if (getCompanyId() == 13 || getCompanyId() == 20 || getUserRole() == 1 || userId == 24 || userId == 1 || userId == 17 || userId == 181)
                {
                    A5.Visible = true;
                }

                sql.CommandText = "select count(*) from tblMessage where msgUID=@uid and msgViewed is null";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@uid", UserID);
                Int32 MessageCount = (int) ExecuteScalar(sql, "Site Master");
                if (MessageCount > 0)
                {
                    litMessageCount.Text = "\n<script>try { document.getElementById('MessageCount').innerHTML='" + MessageCount.ToString() + " ';} catch (err) {}</script>\n";
                }
                sql.CommandText = "select distinct nreNotificationReasonID, nreNotificationReason, coalesce(UID,0) from pktblNotificationReason left outer join tblUserNotificationReasons on nreNotificationReasonID=unrReasonID and unrUserId=@uid left outer join Permissions on unrUserID=UID and UID=@uid where (UID is null or UID=@uid) order by nreNotificationReason ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@user", UserName);
                sql.Parameters.AddWithValue("@uid", UserID);
                SqlDataReader dr = sql.ExecuteReader();
                Int64 i = 0;
                lblWhenNotifiedList.Text = "";
                while (dr.Read())
                {
                    string reasonID = dr.GetValue(0).ToString();
                    string reasonText = dr.GetValue(1).ToString();
                    string ischecked = dr.GetValue(2).ToString();
                    i++;
                    lblWhenNotifiedList.Text += "<input type=checkbox id='cbNotifyWhen" + i.ToString() + "' value='" + reasonID + "' ";
                    if (ischecked != "0")
                    {
                        lblWhenNotifiedList.Text += " checked='checked' ";
                    }
                    lblWhenNotifiedList.Text += "> " + reasonText + "<br>";
                }
                dr.Close();
                lblWhenNotifiedList.Text += "<input type='hidden' id='numberNotifyWhen' value='" + i + "'>";
                sql.CommandText = "select unoUserNotificationTypeID from tblUserNotification where unoUID=@uid";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@uid", UserID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (dr.GetValue(0).ToString() == "1")
                    {
                        cbEmail.Checked = true;
                    }
                    if (dr.GetValue(0).ToString() == "2")
                    {
                        cbMessaging.Checked = true;
                    }
                    if (dr.GetValue(0).ToString() == "3")
                    {
                        cbTexting.Checked = true;
                    }
                }
            }
            sc.Close();
        }


        public void setGlobalVariables()
        {
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select CompanyID, CanAdmin, CanEnterJobs, UID from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", getUserName());
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                UserCompanyID = System.Convert.ToInt64(dr.GetValue(0));
                CanAdmin = System.Convert.ToBoolean(dr.GetValue(1));
                CanEnterJobs = System.Convert.ToBoolean(dr.GetValue(2));
                UserID = System.Convert.ToInt64(dr.GetValue(3));
            }

            dr.Close();
            sql.Parameters.Clear();
            sql.CommandText = "select TSGCompanyAbbrev from TSGCompany where TSGCompanyID=@userCompanyID";
            sql.Parameters.AddWithValue("@userCompanyID", UserCompanyID);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                UserCompanyAbbrev = (dr.GetValue(0).ToString());
            }

            dr.Close();
            sc.Close();
        }

        public NetworkCredential GetDefaultNetworkCredentials()
        {
            WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            using (identity.Impersonate())
            {
                var cred = CredentialCache.DefaultNetworkCredentials;
            }
            return CredentialCache.DefaultNetworkCredentials;
        }

        public void setAllMessagesViewed(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(this.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "select msgMessage, msgMessageID, msgSent from tblMessage where msgActiveMessage=1 and msgUID=@user and msgViewed is null order by msgSent";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@user", getUserID());
            litViewMessages.Text = "";

            SqlDataReader dr = sql.ExecuteReader();
            CheckBox chk = (CheckBox)FindControl("cbCheckAll");
            while (dr.Read())
            {
                litViewMessages.Text += "setMessageViewed(" + dr.GetValue(1).ToString() + ",!$('#cbCheckAll').is(':checked'));\n";
            }
            litMessageCount.Text = "";
            connection.Close();
        }


        public Int64 getUserID()
        {
            SqlConnection connection = new SqlConnection(this.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.Parameters.Clear();
            sql.CommandText = "select CompanyID, CanAdmin, CanEnterJobs, UID, perRFQ from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", getUserName());
            SqlDataReader dr = sql.ExecuteReader();
            Int64 UserID = 0;
            while (dr.Read())
            {
                UserID = System.Convert.ToInt64(dr.GetValue(3));
                if (!System.Convert.ToBoolean(dr.GetValue(4).ToString()))
                {
                    HttpContext.Current.Response.Redirect("~/Permissions.aspx", false);
                }
            }
            dr.Close();
            connection.Close();
            if (UserID == 0)
            {
                HttpContext.Current.Response.Redirect("~/Permissions.aspx", false);
                UserID = 5;
            }
            return UserID;
        }

        public string getUserName()
        {
            String UserName = Context.User.Identity.Name;
            if (Context.User.Identity.Name == "")
            {
                UserName = "mcbailey@toolingsystemsgroup.com";
                //UserName = "sking@toolingsystemsgroup.com";
            }
            if (Context.User.Identity.Name == null)
            {
                UserName = "mcbailey@toolingsystemsgroup.com";
                //UserName = "sking@toolingsystemsgroup.com";
            }
            return UserName;
        }

        public string getName()
        {
            SqlConnection connection = new SqlConnection(this.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.Parameters.Clear();
            sql.CommandText = "select perName from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", getUserName());
            SqlDataReader dr = sql.ExecuteReader();
            string name = "";
            while (dr.Read())
            {
                name = dr["perName"].ToString();
            }
            dr.Close();
            connection.Close();
            return name;
        }

        public int getUserRole()
        {
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.CommandText = "Select UserRoleID from Permissions where UID = @user";
            sql.Parameters.AddWithValue("@user", getUserID());
            int role = 4;
            SqlDataReader dr = sql.ExecuteReader();
            if(dr.Read())
            {
                role = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();
            sc.Close();
            return role;
        }

        public string getUserEmailAddress(string userid)
        {
            String EmailAddress = "mcbailey@toolingsystemsgroup.com";
            //String EmailAddress = "sking@toolingsystemsgroup.com";
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select EmailAddress from permissions where UID=@user";
            sql.Parameters.AddWithValue("@user", userid);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                EmailAddress = dr.GetValue(0).ToString();
            }
            dr.Close();
            sc.Close();
            return EmailAddress;
        }
        public string getUserTextAddress(string userid)
        {
            String TextAddress = "";
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select PhoneNumber, proEmailDomain from permissions, pktblProviders where UID=@user and ProviderID=proProviderID";
            sql.Parameters.AddWithValue("@user", userid);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                TextAddress = dr.GetValue(0).ToString() + "@" + dr.GetValue(1).ToString();
            }
            dr.Close();
            sc.Close();
            return TextAddress;
        }

        protected void Unnamed_LoggingOut(object sender, LoginCancelEventArgs e)
        {
            WsFederationConfiguration config = FederatedAuthentication.FederationConfiguration.WsFederationConfiguration;
            // Redirect to ~/Account/SignOut after signing out.
            string callbackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Response.ApplyAppPathModifier("~/Account/SignOut");
            SignOutRequestMessage signoutMessage = new SignOutRequestMessage(new Uri(config.Issuer), callbackUrl);
            signoutMessage.SetParameter("wtrealm", IdentityConfig.Realm ?? config.Realm);
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            Response.Redirect(signoutMessage.WriteQueryString());
        }

        public Boolean getMasterCompany()
        {
            if (getCompanyId() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Int64 getCompanyId()
        {
            string UserName = getUserName();
            UserCompanyID = 13;
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select CompanyID, CanAdmin, CanEnterJobs from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", UserName);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                UserCompanyID = System.Convert.ToInt64(dr.GetValue(0));
                CanAdmin = System.Convert.ToBoolean(dr.GetValue(1));
                CanEnterJobs = System.Convert.ToBoolean(dr.GetValue(2));
            }
            dr.Close();
            sc.Close();
            return UserCompanyID;
        }

        public Boolean getCanAdmin()
        {
            string UserName = getUserName();
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select CompanyID, CanAdmin, CanEnterJobs from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", UserName);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                UserCompanyID = System.Convert.ToInt64(dr.GetValue(0));
                CanAdmin = System.Convert.ToBoolean(dr.GetValue(1));
                CanEnterJobs = System.Convert.ToBoolean(dr.GetValue(2));
            }
            dr.Close();
            sc.Close();
            return CanAdmin;
        }

        public Boolean getCanEnterJobs()
        {
            string UserName = getUserName();
            SqlConnection sc = new SqlConnection(this.getConnectionString());
            sc.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = sc;
            sql.Parameters.Clear();
            sql.CommandText = "select CompanyID, CanAdmin, CanEnterJobs from permissions where EmailAddress=@user";
            sql.Parameters.AddWithValue("@user", UserName);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                UserCompanyID = System.Convert.ToInt64(dr.GetValue(0));
                CanAdmin = System.Convert.ToBoolean(dr.GetValue(1));
                CanEnterJobs = System.Convert.ToBoolean(dr.GetValue(2));
            }
            dr.Close();
            sc.Close();
            return CanEnterJobs;
        }

        public string getConnectionString()
        {
            string connectionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSGMaster;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca; MultipleActiveResultSets=true; Connection Timeout=120";
            if (Testing)
            {
                connectionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSGMaster_Dev;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca;  MultipleActiveResultSets=true";
            }
            else
            {
                connectionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSGMaster;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca;  MultipleActiveResultSets=true";
            }
            return connectionString;
        }
        public string debugSQLStatement(SqlCommand sql)
        {
            string returnValue = sql.CommandText.ToString();
            foreach (SqlParameter p in sql.Parameters)
            {
                returnValue = returnValue.Replace(p.ParameterName.ToString(), "'" + p.Value.ToString() + "'");
            }
            return returnValue;
        }

        // renders Field given field name
        // looks up field type and renders as appropriate
        // it is in the site master because it is used by multiple programs

        public string renderField(Int64 i, String FieldName, String CurrentValue = "")
        {
            String retval ="";
            SqlConnection conn = new SqlConnection(getConnectionString());
            conn.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = conn;
            sql.CommandText = "select sfnTableName, sfnFieldName,  sfnFieldType, sfnLookupTable, sfnLookupField, sfnReturnField from pktblSystemFieldName where sfnDisplayName=@field";
            sql.Parameters.AddWithValue("@field",FieldName);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                String dbTable = dr.GetValue(0).ToString();
                String dbField = dr.GetValue(1).ToString();
                String fieldType = dr.GetValue(2).ToString();
                String lookupTable = dr.GetValue(3).ToString();
                String lookupField = dr.GetValue(4).ToString();
                String returnField = dr.GetValue(5).ToString();
                if (fieldType == "B") {
                    // Boolean
                    retval += "<input type='checkbox value='1' ";
                    if (CurrentValue == "1") {
                        retval += " checked='checked' ";
                    }
                    retval+= " name='condition" + i + "' id='condition" + i + "'>";
                }
                if (lookupTable != "")
                {
                    // need to render select with correct lookup values
                    SqlConnection conn2 = new SqlConnection(getConnectionString());
                    conn2.Open();
                    SqlCommand sql2 = new SqlCommand();
                    sql2.Connection = conn2;
                    sql2.CommandText = "select distinct " + lookupField + ", " + returnField + " from " + lookupTable + " order by " + returnField;
                    SqlDataReader dr2 = sql2.ExecuteReader();
                    retval += "<select id='condition" + i + "' name='condition" + i + "' style='border: 0px solid black;' onclick=\"this.style.border='1px solid black';\" onchange=\"this.style.border='1px solid black';\">";
                    retval += "<option value='";
                    if (fieldType == "N")
                    {
                        retval += "0";
                    }
                    retval += "'>Please Select</option>";
                    while (dr2.Read()) {
                        retval+="<option value='" + dr2.GetValue(0).ToString()+ "'";
                        if (dr2.GetValue(0).ToString()== CurrentValue )
                        {
                            retval += " selected ";
                        }
                        retval += ">" + dr2.GetValue(1).ToString() + "</option>";
                    }
                    dr2.Close();
                    conn2.Close();
                    retval+="</select>";
                } else
                {
                    // not a lookup
                    if (fieldType == "DATE") {
                        retval+="<input type='text' id='condition" + i+ "' name='condition" + i + "' class='datepicker' readonly size='16' value='" + CurrentValue + "'>";
                        retval+="\n<script>$('#condition" + i + "').datepicker();</script>\n";
                    } else {
                        retval+="<input type='text' id='condition" + i+ "' name='condition" + i + "'  value='" + CurrentValue + "'>";
                    }
                }
            }
            else
            {
                retval += "<input type='hidden' id='condition" + i + "' name='condition" + i + "' value=''>";
            }
            sql.Connection = conn;
            conn.Close();
            return retval;
        }

        // Function to Log and Execute a sql scalar function and return the scalar value
        // I would think this is always an insert statement
        // expections Connection Property to already be set in sqlcommand
        public object ExecuteScalar(SqlCommand sql, String reference)
        {
            var returnValue = sql.ExecuteScalar();
            ParseSql(sql, reference);
            return returnValue;
        }
        // Function to Log and Execute a sql NonQUery function
        // I would think this is always an insert statement
        // expections Connection Property to already be set in sqlcommand
        public void ExecuteNonQuery(SqlCommand sql, String reference)
        {
            sql.ExecuteNonQuery();
            ParseSql(sql, reference);
        }
        public void ParseSql(SqlCommand sql, string reference)
        {
            String sqlStatement = debugSQLStatement(sql);
            String[] statementParts = sqlStatement.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            String TableName = "";
            if ((statementParts[0].ToLower() == "insert") || (statementParts[0].ToLower() == "delete"))
            {
                TableName = statementParts[2];
            }
            if (statementParts[0].ToLower() == "update ")
            {
                TableName = statementParts[1];
            }
            LogDatabaseTransaction(sql, TableName, reference);
        }

        public void LogDatabaseTransaction(SqlCommand sql, String TableName, String reference)
        {
            String sqlStatement = debugSQLStatement(sql);
            SqlCommand tsql = new SqlCommand();
            tsql.Connection = sql.Connection;
            tsql.CommandText = "insert into tblDatabaseTransaction (dtrTable, dtrReference, dtrUserID, dtrSqlStatement, dtrTransactionDate) ";
            tsql.CommandText += " values (@table, @ref, @user, @sql, current_timestamp) ";
            tsql.Parameters.Clear();
            tsql.Parameters.AddWithValue("@table", TableName);
            tsql.Parameters.AddWithValue("@ref", reference);
            tsql.Parameters.AddWithValue("@user", getUserID());
            tsql.Parameters.AddWithValue("@sql", sqlStatement);
            tsql.ExecuteNonQuery();
        }
        public int readCellInt(NPOI.SS.UserModel.ICell cell, Int32 DefaultValue = -1)
        {
            try
            {
                if ((cell.CellType == NPOI.SS.UserModel.CellType.Numeric) || (cell.CellType == NPOI.SS.UserModel.CellType.Formula))
                {
                    return System.Convert.ToInt32(cell.NumericCellValue);
                }
                else
                {
                    return System.Convert.ToInt32(cell.StringCellValue);
                }
            }
            catch
            {

            }

            return DefaultValue;
        }

        public String readCellString(NPOI.SS.UserModel.ICell cell, String DefaultValue = "")
        {
            try
            {
                
                cell.SetCellType(NPOI.SS.UserModel.CellType.String);

                if (cell.CellType == NPOI.SS.UserModel.CellType.String)
                {
                    return cell.StringCellValue.ToString();
                }
                else
                {
                    return cell.StringCellValue;
                }
            }
            catch
            {


            }
            return DefaultValue;
        }
        public double readCellDouble(NPOI.SS.UserModel.ICell cell, Double DefaultValue = -1)
        {
            try
            {
                if ((cell.CellType == NPOI.SS.UserModel.CellType.Numeric) || (cell.CellType == NPOI.SS.UserModel.CellType.Formula))
                {
                    return System.Convert.ToDouble(cell.NumericCellValue);
                }
                else
                {
                    if (cell.CellType == NPOI.SS.UserModel.CellType.String)
                    {
                        return System.Convert.ToDouble(cell.StringCellValue);
                    }
                }
            }
            catch
            {

            }
            return DefaultValue;
        }

        public string renderQuotingHTML(string PartId, string status, Int64 RFQID, Boolean RenderQuoteButtons = true)
        {
            string html = "";
            string company = "";

            SqlConnection connection = new SqlConnection(getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            List<string> noQuote = new List<string>();
            List<string> reserved = new List<string>();
            List<string> quotedID = new List<string>();
            List<string> quotedComp = new List<string>();
            List<string> quoteVersion = new List<string>();
            List<string> oldQuoteNumber = new List<string>();
            List<string> htsQuoteNumber = new List<string>();
            List<string> stsQuoteNumber = new List<string>();
            List<string> ugsQuoteNumber = new List<string>();
            List<string> noQuoteReason = new List<string>();
            List<string> sent = new List<string>();
            string lineNumber = "";
            string linkPart = "";

            if (PartId.Contains("A"))
            {
                PartId = PartId.Replace("A", "");

                sql.CommandText = "Select tc.TSGCompanyAbbrev as reservedAbbrev, tcCurrent.TSGCompanyAbbrev as currentAbbrev, atqQuoteId, ";
                sql.CommandText += "quoteCompany.TSGCompanyAbbrev as quoteAbbrev, stsCompany.TSGCompanyAbbrev as stsQuoteAbbrev, atqHTS, atqUGS, ";
                sql.CommandText += "squQuoteVersion, squQuoteNumber, a.assLineNumber ";
                sql.CommandText += "from tblReserveAssembly ra ";
                sql.CommandText += "inner join tblAssembly a on a.assAssemblyId = ra.rasAssemblyId ";
                sql.CommandText += "inner join TSGCompany tc on tc.TSGCompanyID = ra.rasCompanyId ";
                sql.CommandText += "left outer join linkAssemblyToQuote atq on atq.atqAssemblyId = ra.rasAssemblyId ";
                sql.CommandText += "left outer join tblQuote q on atq.atqHTS = 0 and atq.atqSTS = 0 and atq.atqUGS = 0 and q.quoQuoteId = atq.atqQuoteId ";
                sql.CommandText += "left outer join TSGCompany quoteCompany on quoteCompany.TSGCompanyId = q.quoTSGCompanyID ";
                sql.CommandText += "left outer join tblHTSQuote hq on hq.hquHTSQuoteId = atq.atqQuoteId and atq.atqHTS = 1 ";
                sql.CommandText += "left outer join tblSTSQuote sq on sq.squSTSQuoteId = atq.atqQuoteId and atq.atqSTS = 1 ";
                sql.CommandText += "left outer join TSGCompany stsCompany on stsCompany.TSGCompanyID = sq.squCompanyId ";
                sql.CommandText += "left outer join tblUGSQuote uq on uq.uquUGSQuoteId = atq.atqQuoteId and atq.atqUGS = 1 ";
                sql.CommandText += "left outer join TSGCompany tcCurrent on tcCurrent.TSGCompanyId = @companyId ";
                sql.CommandText += "where ra.rasAssemblyId = @assemblyId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assemblyId", PartId);
                sql.Parameters.AddWithValue("@companyId", UserCompanyID);

                SqlDataReader sdr = sql.ExecuteReader();
                while(sdr.Read())
                {
                    lineNumber = sdr["assLineNumber"].ToString();
                    if (sdr["reservedAbbrev"].ToString() != "")
                    {
                        if (!reserved.Contains(sdr["reservedAbbrev"].ToString()))
                        {
                            reserved.Add(sdr["reservedAbbrev"].ToString());
                        }
                    }
                    if (sdr["atqQuoteId"].ToString() != "")
                    {
                        if (sdr["quoteAbbrev"].ToString() != "")
                        {
                            quotedComp.Add(sdr["quoteAbbrev"].ToString());
                        }
                        else if (sdr["stsQuoteAbbrev"].ToString() != "")
                        {
                            quotedComp.Add(sdr["stsQuoteAbbrev"].ToString());
                        }
                        else if (System.Convert.ToBoolean(sdr["atqHTS"].ToString()))
                        {
                            quotedComp.Add("HTS");
                        }
                        else if (System.Convert.ToBoolean(sdr["atqUGS"].ToString()))
                        {
                            quotedComp.Add("UGS");
                        }
                        quotedID.Add(sdr["atqQuoteId"].ToString());
                        stsQuoteNumber.Add(sdr["squQuoteNumber"].ToString());
                        quoteVersion.Add(sdr["squQuoteVersion"].ToString());
                    }
                    
                    company = sdr["currentAbbrev"].ToString();

                }
                sdr.Close();

                for (int i = 0; i < reserved.Count(); i++)
                {
                    html += "Reserved By " + reserved[i] + "<br />";
                    if (company == reserved[i])
                    {
                        //if (company == "HTS" && !quotedComp.Contains(company))
                        //{
                        //    html += "<a href='HTSEditQuote?rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quote</a><br/>";
                        //}
                        if ((company == "STS" || company == "NIA") && !quotedComp.Contains(company))
                        {
                            html += "<a href='STSEditQuote?rfq=" + RFQID + "&assemblyId=" + PartId + "' target='_blank'>Quote</a><br/>";
                        }

                        //else if (company == "UGS" && !quotedComp.Contains(company))
                        //{
                        //    html += "<a href='UGSEditQuote?rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quote</a><br/>";
                        //}
                        //else if (!quotedComp.Contains(company))
                        //{
                        //    html += "<div class='mybutton' onclick='uploadQuote();' id='quoteUploadButton'>Upload Quote</div>";
                        //}
                    }
                }
                for (int i = 0; i < quotedComp.Count(); i++)
                {
                    if (quotedComp[i] == "STS" || quotedComp[i] == "NIA")
                    {
                        int num;
                        bool results = Int32.TryParse(stsQuoteNumber[i], out num);
                        if (!results)
                        {
                            html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&assemblyId=" + PartId + "' target='_blank'>Quoted #" + stsQuoteNumber[i] + "-STS-" + quoteVersion[i] + "</a><br />";
                        }
                        else
                        {
                            html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&assemblyId=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-A" + lineNumber + "-STS-" + quoteVersion[i] + "</a><br />";
                        }
                    }
                }

                if (!reserved.Contains(company))
                {
                    // Adding the A back into the part ID so we can tell its an assembly
                    html += "<input type='button' class='mybutton' value='Reserve' onClick=\"reservePart('A" + PartId + "');return false;\" >";
                }

                connection.Close();
                return html;
            }

            sql.CommandText = "Select (select TSGCompanyAbbrev from TSGCompany where TSGCompanyID = nquCompanyID), (select TSGCompanyAbbrev from TSGCompany where TSGCompanyID = prcTSGCompanyID), ptqQuoteID, ";
            sql.CommandText += "(select TSGCompanyAbbrev from TSGCompany where TSGCompanyID = quoTSGCompanyID), hquHTSQuoteID, squSTSQuoteID, uquUGSQuoteID, prtRFQLineNumber, quoVersion, hquVersion, squQuoteVersion, ";
            sql.CommandText += "uquQuoteVersion, tsg.TSGCompanyAbbrev, nqrNoQuoteReasonNumber, ";
            sql.CommandText += "(select prtRFQLineNumber from tblPart where prtPARTID = (Select min(ppdPartID) from linkPartToPartDetail, tblPart where ppdPartToPartID = (Select TOP 1 ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID))), ";
            sql.CommandText += "qtrSent, quoOldQuoteNumber, hquNumber, squQuoteNumber, uquQuoteNumber, stsCompany.TSGCompanyAbbrev as stsComp ";
            sql.CommandText += "from TSGCompany tsg, tblPart ";
            sql.CommandText += "left outer join tblNoQuote on nquPartID = prtPARTID ";
            sql.CommandText += "left outer join pktblNoQuoteReason on nqrNoQuoteReasonID = nquNoQuoteReasonID ";
            sql.CommandText += "left outer join linkPartReservedToCompany on prcPartID = prtPARTID ";
            sql.CommandText += "left outer join linkPartToQuote on ptqPartID = prtPARTID ";
            sql.CommandText += "left outer join linkQuoteToRFQ on ptqQuoteID = qtrQuoteID and ptqHTS = qtrHTS and ptqSTS = qtrSTS and ptqUGS = qtrUGS ";
            sql.CommandText += "left outer join tblQuote on ptqQuoteID = quoQuoteID and ptqHTS <> 1 and ptqSTS <> 1 and ptqUGS <> 1 ";
            sql.CommandText += "left outer join tblHTSQuote on hquHTSQuoteID = ptqQuoteID and ptqHTS = 1 ";
            sql.CommandText += "left outer join tblSTSQuote on squSTSQuoteID = ptqQuoteID and ptqSTS = 1 ";
            sql.CommandText += "left outer join TSGCompany stsCompany on stsCompany.TSGCompanyID = squCompanyId ";
            sql.CommandText += "left outer join tblUGSQuote on uquUGSQuoteID = ptqQuoteID and ptqUGS = 1 ";
            sql.CommandText += "where prtPartID = @partID and tsg.TSGCompanyID = @tsg ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partID", PartId);
            sql.Parameters.AddWithValue("@tsg", UserCompanyID);
            SqlDataReader dr = sql.ExecuteReader();
            while(dr.Read())
            {
                if (!noQuote.Contains(dr.GetValue(0).ToString()) && dr.GetValue(0).ToString() != "")
                {
                    noQuote.Add(dr.GetValue(0).ToString());
                    noQuoteReason.Add(dr.GetValue(13).ToString());
                }
                if (!reserved.Contains(dr.GetValue(1).ToString()) && dr.GetValue(1).ToString() != "")
                {
                    reserved.Add(dr.GetValue(1).ToString());
                }
                if (!quotedID.Contains(dr.GetValue(2).ToString()) && dr.GetValue(2).ToString() != "")
                {
                    quotedID.Add(dr.GetValue(2).ToString());
                    sent.Add(dr.GetValue(15).ToString());
                    oldQuoteNumber.Add(dr.GetValue(16).ToString());
                    if (dr.GetValue(4).ToString() != "")
                    {
                        quotedComp.Add("HTS");
                        quoteVersion.Add(dr.GetValue(9).ToString());
                    }
                    else if (dr.GetValue(5).ToString() != "")
                    {
                        quotedComp.Add(dr["stsComp"].ToString());
                        quoteVersion.Add(dr.GetValue(10).ToString());
                    }
                    else if (dr.GetValue(6).ToString() != "")
                    {
                        quotedComp.Add("UGS");
                        quoteVersion.Add(dr.GetValue(11).ToString());
                    }
                    else
                    {
                        quotedComp.Add(dr.GetValue(3).ToString());
                        quoteVersion.Add(dr.GetValue(8).ToString());
                    }
                }
                lineNumber = dr.GetValue(7).ToString();
                company = dr.GetValue(12).ToString();
                linkPart = dr.GetValue(14).ToString();
                htsQuoteNumber.Add(dr["hquNumber"].ToString());
                stsQuoteNumber.Add(dr["squQuoteNumber"].ToString());
                ugsQuoteNumber.Add(dr["uquQuoteNumber"].ToString());
            }
            dr.Close();

            for (int i = 0; i < reserved.Count; i++)
            {
                html += "Reserved By " + reserved[i] + "<BR>";
                if (company == reserved[i])
                {
                    if(company == "HTS" && !quotedComp.Contains(company))
                    {
                        html += "<a href='HTSEditQuote?rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quote</a><br/>";
                    }
                    else if ((company == "STS" || company == "NIA") && !quotedComp.Contains(company))
                    {
                        html += "<a href='STSEditQuote?rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quote</a><br/>";
                    }
                    else if (company == "UGS" && !quotedComp.Contains(company))
                    {
                        html += "<a href='UGSEditQuote?rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quote</a><br/>";
                    }
                    else if (!quotedComp.Contains(company))
                    {
                        html += "<div class='mybutton' onclick='uploadQuote();' id='quoteUploadButton'>Upload Quote</div>";
                    }
                }
            }

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            for (int i = 0; i < quotedID.Count; i++)
            {
                if(linkPart != "")
                {
                    if (quotedComp[i] == "HTS")
                    {
                        int num;
                        bool results = Int32.TryParse(htsQuoteNumber[i], out num);
                        if(!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='HTSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + htsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + htsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='HTSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + linkPart + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + htsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else if (quotedComp[i] == "STS" || quotedComp[i] == "NIA" || quotedComp[i] == "NRS")
                    {
                        int num;
                        bool results = Int32.TryParse(stsQuoteNumber[i], out num);
                        if(!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + stsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + stsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + linkPart + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + stsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else if (quotedComp[i] == "UGS")
                    {
                        int num;
                        bool results = Int32.TryParse(ugsQuoteNumber[i], out num);
                        if (!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='UGSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + ugsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + ugsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='UGSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + linkPart + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + ugsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else
                    {
                        if(oldQuoteNumber[i] != "")
                        {
                            if(oldQuoteNumber[i].Contains("SA"))
                            {
                                if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                                {
                                    html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + oldQuoteNumber[i] + "</a><br />";
                                }
                                else
                                {
                                    html += "Quoted # " + oldQuoteNumber[i] + "<br />";
                                }
                            }
                            else
                            {
                                if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                                {
                                    html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + oldQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                                }
                                else
                                {
                                    html += "Quoted # " + oldQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                                }
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + RFQID + "-" + linkPart + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + RFQID + "-" + linkPart + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    if (company == quotedComp[i] || (company == "STS" && (quotedComp[i] == "NIA" || quotedComp[i] == "NRS")))
                    {
                        html += "<input type='button' class='mybutton' value='Remove Quote'  onClick=\"removeQuote('" + quotedID[i] + "');return false;\" >";
                    }
                    if (sent[i] != "")
                    {
                        html += "Sent " + quotedComp[i] + " - " + TimeZoneInfo.ConvertTimeFromUtc(System.Convert.ToDateTime(sent[i]), easternZone).ToString() + "<br />";
                    }
                }
                else
                {
                    if (quotedComp[i] == "HTS")
                    {
                        int num;
                        bool results = Int32.TryParse(htsQuoteNumber[i], out num);
                        if (!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='HTSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + htsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + htsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='HTSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else if (quotedComp[i] == "STS" || quotedComp[i] == "NIA")
                    {
                        int num;
                        bool results = Int32.TryParse(stsQuoteNumber[i], out num);
                        if (!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + stsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + stsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='STSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else if (quotedComp[i] == "UGS")
                    {
                        int num;
                        bool results = Int32.TryParse(ugsQuoteNumber[i], out num);
                        if (!results)
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG")
                            {
                                html += "<a href='UGSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + ugsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + ugsQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG")
                            {
                                html += "<a href='UGSEditQuote?id=" + quotedID[i] + "&rfq=" + RFQID + "&partID=" + PartId + "' target='_blank'>Quoted #" + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    else
                    {
                        if (oldQuoteNumber[i] != "")
                        {
                            if (oldQuoteNumber[i].Contains("SA"))
                            {
                                if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                                {
                                    html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + oldQuoteNumber[i] + "</a><br />";
                                }
                                else
                                {
                                    html += "Quoted # " + oldQuoteNumber[i] + "<br />";
                                }
                            }
                            else
                            {
                                if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                                {
                                    html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + oldQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                                }
                                else
                                {
                                    html += "Quoted # " + oldQuoteNumber[i] + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                                }
                            }
                        }
                        else
                        {
                            if (UserCompanyAbbrev == quotedComp[i] || UserCompanyAbbrev == "TSG" || UserCompanyAbbrev == "UGS")
                            {
                                html += "<a href='EditQuote?id=" + quotedID[i] + "&quoteType=2" + "' target='_blank'>Quoted  #" + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "</a><br />";
                            }
                            else
                            {
                                html += "Quoted # " + RFQID + "-" + lineNumber + "-" + quotedComp[i] + "-" + quoteVersion[i] + "<br />";
                            }
                        }
                    }
                    if (company == quotedComp[i] || (company == "STS" && (quotedComp[i] == "NIA" || quotedComp[i] == "NRS")))
                    {
                        html += "<input type='button' class='mybutton' value='Remove Quote'  onClick=\"removeQuote('" + quotedID[i] + "');return false;\" >";
                    }
                    if (sent[i] != "")
                    {
                        html += "Sent " + quotedComp[i] + " - " + TimeZoneInfo.ConvertTimeFromUtc(System.Convert.ToDateTime(sent[i]), easternZone).ToString() + "<br />";
                    }
                }
            }

            if (linkPart == "" || System.Convert.ToInt32(lineNumber) == System.Convert.ToInt32(linkPart))
            {
                if (!quotedComp.Contains(company) && !noQuote.Contains(company))
                {
                    html += "<input type='button' class='mybutton' value='No Quote'  onClick=\"applyNoQuotePart('" + PartId + "');return false;\" >";
                }

                if (!quotedComp.Contains(company) && !reserved.Contains(company) && !noQuote.Contains(company))
                {
                    html += "<input type='button' class='mybutton' value='Reserve' onClick=\"reservePart('" + PartId + "');return false;\" >";
                }

                if (noQuote.Contains(company))
                {
                    html += "<input type='button' class='mybutton' value='Remove No Quote'  onClick=\"removeNoQuotePart('" + PartId + "');return false;\" >";
                }
            }


            for (int i = 0; i < noQuote.Count; i++)
            {
                html += "<BR><font color='Black'>No Quoted By " + noQuote[i] + " - " + noQuoteReason[i] + "</font><br />";
            }

            html += "</div>";
            connection.Close();
            return html;
        }

        public SharePointOnlineCredentials getSharePointCredentials()
        {
            SecureString password = new SecureString();
            foreach (char c in "Zuxo9269".ToCharArray()) password.AppendChar(c);
            SharePointOnlineCredentials cred =  new Microsoft.SharePoint.Client.SharePointOnlineCredentials("tsgrfqadmin@toolingsystemsgroup.com", password);
            return cred;
        }

        public SharePointOnlineCredentials getSharePointCredentialsugs()
        {
            SecureString password = new SecureString();
            foreach (char c in "r8oavilZELd16fs".ToCharArray()) password.AppendChar(c);
            SharePointOnlineCredentials cred = new Microsoft.SharePoint.Client.SharePointOnlineCredentials("ugsjobcreation@toolingsystemsgroup.com", password);
            return cred;
        }

        public System.Net.NetworkCredential getNetworkCredentials()
        {
            return new System.Net.NetworkCredential("tsgrfqadmin@toolingsystemsgroup.com", "Zuxo9269");
        }

        public System.Net.Mail.MailAddress getFromAddress()
        {
            return new System.Net.Mail.MailAddress("tsgrfqadmin@toolingsystemsgroup.com");
        }
    }
    public static class SpQuery{

        public static void ExecuteQueryWithIncrementalRetry(this ClientContext context, int retryCount, int delay)
        {
            int retryAttempts = 0;
            int backoffInterval = delay;
            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");

            if (delay <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            // Do while retry attempt is less than retry count
            while (retryAttempts < retryCount)
            {
                try
                {
                    const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
                    const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
                    ServicePointManager.SecurityProtocol = Tls12;

                    context.ExecuteQuery();
                    return;

                }
                catch (WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;
                    // Check if request was throttled - http status code 429
                    // Check is request failed due to server unavailable - http status code 503
                    if (response != null && (response.StatusCode == (HttpStatusCode)429 || response.StatusCode == (HttpStatusCode)503))
                    {
                        // Output status to console. Should be changed as Debug.WriteLine for production usage.
                        Console.WriteLine(string.Format("CSOM request frequency exceeded usage limits. Sleeping for {0} seconds before retrying.",
                                        backoffInterval));

                        //Add delay for retry
                        System.Threading.Thread.Sleep(backoffInterval);

                        //Add to retry count and increase delay.
                        retryAttempts++;
                        backoffInterval = backoffInterval * 2;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            //throw new MaximumRetryAttemptedException(string.Format("Maximum retry attempts {0}, has be attempted.", retryCount));
            return;
        }
    }
}
