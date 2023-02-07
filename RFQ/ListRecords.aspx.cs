using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class ListRecords : System.Web.UI.Page
    {
        public String sortExpression = "";
        public String exportSQL = "";
        // indicates whether or not to remove 3 characters from the columns in this table.
        // initial tables were created before we went to the 3 letter prefix standard
        Boolean cleanNames = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            if (!master.getCanAdmin())
            {
                Response.Redirect("~/Default");
            }
            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.ToString("d");
                txtEndDate.Text = DateTime.Now.ToString("d");
                searchLimit.Text = "20";
            }
            refreshPage();
        }

        public String returnAlignment(String FieldType)
        {
            String Alignment = "";
            if ((FieldType == "bit") || (FieldType == "date") || (FieldType == "datetime"))
            {
                Alignment = " align='center' ";
            }
            if ((FieldType == "int") || (FieldType == "bigint") || (FieldType == "float") || (FieldType == "decimal"))
            {
                Alignment = " align='right' ";
            }
            return Alignment;
        }
        protected void refreshPage()
        {
            lblMessage.Text = "";
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection();
            Site master = new Site();
            connection.ConnectionString = master.getConnectionString();
            connection.Open();
            sql.Connection = connection;
            sql.Parameters.Clear();


            String Action = "";
            try
            {
                Action = Request["a"].ToString();
            }
            catch
            {

            }
            List<String> LegacyTables = new List<string>();
            LegacyTables.Add("CustomerContact");
            LegacyTables.Add("Customer");
            LegacyTables.Add("Capacity");
            LegacyTables.Add("CustomerLocation");
            LegacyTables.Add("CustomerDesignSpec");
            String TableName = Request["table"];
            if (LegacyTables.Contains(TableName))
            {
                cleanNames = false;
            }

            if (Action != "")
            {
                SearchFor.Text = Request["SearchFor"];
                pageNumber.Text = Request["pageNumber"];
                searchLimit.Text = Request["searchLimit"];
            }
            lblAddTable.Text = "\n<script>$('#form1').append('<input type=\"hidden\" name=\"table\" value=\"" + TableName + "\"/>');</script>\n";
            string searchText = "";
            string searchString = "%" + SearchFor.Text.Trim() + "%";
            try
            {
                sortExpression = Request["sort"].ToString();
            }
            catch
            {
                sortExpression = "";
            }
            sql.CommandText = " SELECT ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = @table ORDER BY ORDINAL_POSITION ASC ";
            sql.Parameters.AddWithValue("@table", TableName);
            SqlDataReader tdr = sql.ExecuteReader();
            String Title = System.Text.RegularExpressions.Regex.Replace(TableName.Replace("TSG", "").Replace("pktbl", ""), "[A-Z]", " $0").Trim();
            tblResults.Text = "<center><h2>" + Title + "</h2></center>";

            tblResults.Text += "<table cellpadding='4' align='center'><thead><tr>";
            
            String joinlist = "";
            string selectText = " select ";
            String subsearch = "";
            Boolean useSearchDate = false;
            Boolean useSearchLike = false;
            Int64 columnCount = 0;
            String[] AlignmentList = new String[999];
            String[] FieldTypeList = new String[999];
            String[] ColumnList = new String[999];
            Int64[] MaxLengthList = new Int64[999];
            String[] DefaultList = new String[999];
            editDialogContents.Text = "</form><form name=updform method='POST' action='ListRecords'>";
            editDialogContents.Text += "<input type='hidden' name='table' value='" + TableName + "'>";
            editDialogContents.Text += "<input type='hidden' name='a' value='update'>";
            editDialogContents.Text += "<input type='hidden' name='SearchFor' value='" + SearchFor.Text + "'>";
            editDialogContents.Text += "<input type='hidden' name='pageNumber' value='" + pageNumber.Text + "'>";
            editDialogContents.Text += "<input type='hidden' name='searchLimit' value='" + searchLimit.Text + "'>";
            String UpdateString = "";
            String UpdateComma="";
            String UpdateWhen = "";
            String InsertValues = "";
            SqlConnection UpdateConnection = new SqlConnection(master.getConnectionString());
            SqlCommand UpdateSql = new SqlCommand();
            UpdateSql.Connection = UpdateConnection;
            while (tdr.Read())
            {
                String Position = tdr.GetValue(0).ToString();
                String ColumnName = tdr.GetValue(1).ToString();
                ColumnList[columnCount] = ColumnName;
                String FieldTitleStart = ColumnName.Substring(3);
                if (!cleanNames)
                {
                    FieldTitleStart = ColumnName;
                }
                String FieldTitle = System.Text.RegularExpressions.Regex.Replace(FieldTitleStart.Replace("Id", "").Replace("ID", "").Replace("TSG", ""), "[A-Z]", " $0").Trim();
                FieldTitle = FieldTitle.Replace("R F Q", "RFQ");
                if (columnCount == 0) {
                    editDialogContents.Text += "<input type='hidden' name='" + ColumnName + "' id='" + ColumnName + "' value='0'>";
                    if (Action != "") 
                    {
                        if (Request[ColumnName] == "0")
                        {
                            UpdateString = "insert into " + TableName + " (";
                        }
                        else
                        {
                            UpdateWhen = " where " + ColumnName + " = @key";
                            UpdateSql.Parameters.AddWithValue("@key",Request[ColumnName]);
                            UpdateString = "update " + TableName + " set ";
                        }
                    }
                }
                else
                {
                    editDialogContents.Text += "<div style='float: left; width: 30%; text-align: left;'><label class='ui-widget' for='" + ColumnName+"'>" + FieldTitle + "</label><br>";
                    if (Action != "")
                    {
                        String NewValue = Request[ColumnName];
                        if (UpdateWhen == "")
                        {
                            UpdateString += UpdateComma + " " + ColumnName;
                            InsertValues += UpdateComma + " @val" + columnCount;
                            if(NewValue == null)
                            {
                                UpdateSql.Parameters.AddWithValue("@val" + columnCount, DBNull.Value);
                            }
                            else
                            {
                                UpdateSql.Parameters.AddWithValue("@val" + columnCount, NewValue);
                            }
                        }
                        else
                        {
                            UpdateString += UpdateComma + " " + ColumnName + " = @val" + columnCount;
                            var t = tdr.GetValue(2).ToString();
                            if (ColumnName == "ccoPrimaryDecisionMaker" || tdr.GetValue(2).ToString() == "bit" )
                            {
                                if (NewValue == null)
                                {
                                    UpdateSql.Parameters.AddWithValue("@val" + columnCount, 0);
                                }
                                else
                                {
                                    UpdateSql.Parameters.AddWithValue("@val" + columnCount, NewValue);
                                }
                            }
                            else
                            {
                                UpdateSql.Parameters.AddWithValue("@val" + columnCount, NewValue);
                            }
                        }
                        UpdateComma = ", ";
                    }
                }
                if (sortExpression.Trim()=="")
                {
                    sortExpression = TableName + "." + ColumnName;
                }
                String FieldType = tdr.GetValue(2).ToString();
                FieldTypeList[columnCount] = FieldType;
                Int64 Maxlength = 0;
                if (tdr.GetValue(3) != DBNull.Value)
                {
                    Maxlength = System.Convert.ToInt64(tdr.GetValue(3));
                }
                if (Maxlength < 0)
                {
                    // -1 indicates varchar(max)
                    Maxlength = 8000;
                }
                MaxLengthList[columnCount] = Maxlength;
                String DefaultValue = "";
                if (tdr.GetValue(4) != DBNull.Value) {
                    DefaultValue=tdr.GetValue(4).ToString();
                }
                if (FieldType == "bit" || FieldType =="int" || FieldType == "bigint" || FieldType=="float" || FieldType == "decimal") 
                {
                    if (DefaultValue=="") 
                    {
                        DefaultValue="0";
                    }
                }
                DefaultList[columnCount] = DefaultValue;
                AlignmentList[columnCount] = returnAlignment(FieldType);
                tblResults.Text += "<th " + returnAlignment(FieldType) + ">";
                // this Regex places spaces between the upper case letters
                if (Position == "1")
                {
                    tblResults.Text += "Action";
                }
                else {
                    tblResults.Text += FieldTitle;
                }
                tblResults.Text += "</th>";
                String LookupTable = "";
                String LookupField = "";
                String LookupDisplay = "";
                SqlConnection lconn = new SqlConnection(master.getConnectionString());
                SqlCommand lsql = new SqlCommand();
                lconn.Open();
                lsql.Connection = lconn;
                lsql.CommandText = "SELECT KCU1.TABLE_NAME,  KCU1.COLUMN_NAME, KCU2.TABLE_NAME,  KCU2.COLUMN_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU1 ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG     AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA    AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU2 ON KCU2.CONSTRAINT_CATALOG =  RC.UNIQUE_CONSTRAINT_CATALOG  AND KCU2.CONSTRAINT_SCHEMA =  RC.UNIQUE_CONSTRAINT_SCHEMA    AND KCU2.CONSTRAINT_NAME =  RC.UNIQUE_CONSTRAINT_NAME    AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION";
                lsql.CommandText += " where KCU1.TABLE_NAME=@table and KCU1.COLUMN_NAME=@field";
                lsql.Parameters.AddWithValue("@table", TableName);
                lsql.Parameters.AddWithValue("@field", ColumnName);
                SqlDataReader ldr = lsql.ExecuteReader();
                while (ldr.Read())
                {
                    LookupTable = ldr.GetValue(2).ToString();
                    LookupField = ldr.GetValue(3).ToString();
                    joinlist += " Left Outer Join " + LookupTable + " On " + TableName + "." + ColumnName + " = " + LookupTable + "." + LookupField + " ";
                    SqlConnection dconn = new SqlConnection(master.getConnectionString());
                    dconn.Open();
                    SqlCommand dsql = new SqlCommand();
                    dsql.Connection = dconn;
                    dsql.CommandText = " SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = @table and ORDINAL_POSITION=2 ";
                    dsql.Parameters.AddWithValue("@table", LookupTable);
                    SqlDataReader ddr = dsql.ExecuteReader();
                    while (ddr.Read())
                    {
                        LookupDisplay = ddr.GetValue(0).ToString();
                    }
                    if (LookupTable.ToUpper() == "PROCESS")
                    {
                        joinlist += " left Outer Join DieType on Process.TSGCompanyId=DieType.TSGCompanyID and Process.DieTypeId=DieType.DieTypeId";
                        LookupDisplay = "concat(TSGCompanyAbbrev,'-',ProcessName,'-',dtyFullName) as CompanyProcessDie ";
                    }
                    ddr.Close();
                    dconn.Close();
                }
                lconn.Close();
                if (selectText != " select ")
                {
                    selectText += ", ";
                }
                if (LookupDisplay != "")
                {
                    FieldTypeList[columnCount] = "lookup";
                    if (LookupDisplay.Length > 5)
                    {
                        if (LookupDisplay.Substring(0, 6) == "concat")
                        {
                            selectText += LookupDisplay;
                        }
                        else
                        {
                            selectText += LookupTable + "." + LookupDisplay;

                        }
                    }
                    else
                    {
                        selectText += LookupTable + "." + LookupDisplay;
                    }
                    if (SearchFor.Text.Trim() != "") {
                        if (subsearch != "")
                        {
                            subsearch += " or ";
                        }
                        if (LookupDisplay.Length > 5)
                        {
                            if (LookupDisplay.Substring(0, 6) == "concat")
                            {
                                subsearch += LookupDisplay + " like @srch ";
                            }
                            else
                            {
                                subsearch += LookupTable + "." + LookupDisplay + " like @srch ";

                            }
                        }
                        else
                        {
                            subsearch += LookupTable + "." + LookupDisplay + " like @srch ";

                        }
                        useSearchLike = true;
                    }
                    AlignmentList[columnCount] = "";
                    editDialogContents.Text += "<select name='" + ColumnName + "' id='" + ColumnName + "'>";
                    SqlConnection dconn = new SqlConnection(master.getConnectionString());
                    dconn.Open();
                    SqlCommand dsql = new SqlCommand();
                    dsql.Connection = dconn;
                    dsql.CommandText = " SELECT " + LookupField + ", " +  LookupDisplay + "  FROM " + LookupTable + " order by " + LookupDisplay;
                    // special for this table
                    if (LookupTable.ToUpper() == "PROCESS")
                    {
                        dsql.CommandText = "SELECT ProcessId, concat(TSGCompanyAbbrev,'-',ProcessName,'-',dtyFullName) as DISPLAYVALUE FROM Process, TSGCompany, DieType where Process.TSGCompanyId=TSGCompany.TSGCompanyId and Process.TSGCompanyId=DieType.TSGCompanyId and Process.DieTypeId=DieType.DieTypeID order by TSGCompanyAbbrev, ProcessName, DieType.Name";
                    }
                    SqlDataReader ddr = dsql.ExecuteReader();
                    while (ddr.Read())
                    {
                        editDialogContents.Text += "<option value='" + ddr.GetValue(0).ToString() + "'>" + ddr.GetValue(1).ToString() + "</option>";
                    }
                    ddr.Close();
                    dconn.Close();

                    editDialogContents.Text += "</select>";
                }
                else
                {

                    selectText += TableName + "." + ColumnName;
                    if (FieldType == "bit")
                    {
                        if (columnCount > 0) {
                            editDialogContents.Text += "<input type='checkbox' name='" + ColumnName + "' id='" + ColumnName + "' value='1' class='ui-widget-content'>";
                        }
                    }
                    else
                    {
                        if (columnCount > 0)
                        {
                            if (Maxlength > 100)
                            {
                                editDialogContents.Text += "<textarea name='" + ColumnName + "' id='" + ColumnName + "' class='ui-widget-content' rows='3' cols='40'></textarea>";
                            }
                            else 
                            {
                                editDialogContents.Text += "<input type='text' name='" + ColumnName + "' id='" + ColumnName + "' value='' class='ui-widget-content ";
                            }
                        }
                    }
                    if ((FieldType == "date") || (FieldType == "datetime") )
                    {
                        if (columnCount > 0)
                        {
                            editDialogContents.Text += " datepicker'>";
                        }
                        try
                        {
                            // currently this is done to specifically skip the search dates.
                            if (useSearchDate)
                            {
                                if (subsearch != "")
                                {
                                    subsearch += " and ";
                                }
                                subsearch += TableName + "." + ColumnName + " >= @srchstart and " + TableName + "." + ColumnName + " <= @srchend  ";
                                useSearchDate = true;
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        if (FieldType != "bit" && FieldType != "bigint" && FieldType != "int" & FieldType != "float" && FieldType != "decimal" && Maxlength <= 100)
                        {
                            if (columnCount > 0)
                            {
                                editDialogContents.Text += "'>";
                            }
                            if (SearchFor.Text.Trim() != "")
                            {
                                if (subsearch != "")
                                {
                                    subsearch += " or ";
                                }
                                subsearch += TableName + "." + ColumnName + " like @srch ";
                                useSearchLike = true;
                            }
                        }
                        else
                        {
                            if ((FieldType != "bit") && (Maxlength <= 100))
                            {
                                // this should work for most numbers
                                if (columnCount > 0)
                                {
                                    editDialogContents.Text += "' style='text-align: right; width: 80px;'>";
                                }
                            }
                            if (SearchFor.Text.Trim() != "")
                            {
                                if (subsearch != "")
                                {
                                    subsearch += " or ";
                                }
                                if (LookupDisplay.Length > 5)
                                {
                                    if (LookupDisplay.Substring(0, 6) == "concat")
                                    {
                                        subsearch += TableName + "." + ColumnName + " like @srch ";
                                    }
                                    else
                                    {
                                        subsearch += TableName + "." + ColumnName + " like @srch ";

                                    }
                                }
                                else
                                {
                                    subsearch += ColumnName + " like @srch ";

                                }
                                useSearchLike = true;
                            }

                        }
                    }
                }
                if (columnCount > 0) {
                    editDialogContents.Text += "</div>";
                    if (columnCount % 3 == 0)
                    {
                        editDialogContents.Text += "<div style='clear: both;'></div><BR>";
                    }
                }
                columnCount++;
            }
            tdr.Close();
            if (Action != "")
            {
                if (UpdateWhen == "")
                {
                    UpdateString += ") values (" + InsertValues + ") ";
                }
                else
                {
                    UpdateString += " " + UpdateWhen;
                }
                UpdateSql.CommandText = UpdateString;
                UpdateConnection.Open();
                //master.ExecuteNonQuery(UpdateSql, "ListRecords");
                UpdateSql.ExecuteNonQuery();
                UpdateConnection.Close();
            }
            tblResults.Text += "</tr></thead><tbody>";
            editDialogContents.Text += "<br><div align='center' class='mybutton' onclick='document.updform.submit();'>Save</div>";
            sql.CommandText = selectText + " from " + TableName + " " + joinlist + " ";
            if (subsearch != ""  )
            {
                searchText = " where  (" + subsearch + ") ";
                if (useSearchDate) 
                {
                    DateTime sdate = System.Convert.ToDateTime(txtStartDate.Text.Trim());
                    sql.Parameters.AddWithValue("@srchstart", sdate.ToString("d"));
                    DateTime edate = System.Convert.ToDateTime(txtEndDate.Text.Trim());
                    sql.Parameters.AddWithValue("@srchend", edate.ToString("d"));
                }
                if (useSearchLike) 
                {
                    sql.Parameters.AddWithValue("@srch", searchString);
                }
            }
            if (searchLimit.Text == "")
            {
                searchLimit.Text = "20";
            }
            sql.CommandText += searchText;
            sql.CommandText += " order by " + sortExpression;
            exportSQL = sql.CommandText;
            if (pageNumber.Text != "")
            {
                Int64 skipPages = (System.Convert.ToInt64(pageNumber.Text) - 1) * System.Convert.ToInt64(searchLimit.Text);
                sql.CommandText += " offset " + skipPages.ToString() + " rows fetch next " + searchLimit.Text + " rows only ";
            }
            else
            {
                sql.CommandText += " offset  0 rows fetch next " + searchLimit.Text + " rows only ";
            }
            SqlDataReader dr = sql.ExecuteReader();
            Int64 RowsReturned = 0;
            newRecordScript.Text += "<script>\n";
            newRecordScript.Text += "    function newRecord() {\n";
            newRecordScript.Text += "      $('#" + ColumnList[0] + "').val(0);\n";

            deleteRecordScript.Text += "<script>\n";
            deleteRecordScript.Text += "    function deleteRecord(nbr) {\n";
            deleteRecordScript.Text += "        if (confirm('Are You Sure? (OK=Delete, Cancel=Do Not Delete)')) {\n";
            deleteRecordScript.Text += "                url='DeleteRecord?code=8383&table=" + TableName + "&key="+  ColumnList[0] +"&id='+nbr;\n";
            deleteRecordScript.Text += "                $.ajax({url: url, success: function (data) {document.location.reload();}});\n";
            deleteRecordScript.Text += "        }\n";
            deleteRecordScript.Text += "    }\n";
            deleteRecordScript.Text += "</script>\n";
            editRecordScript.Text = "";
            editRecordScript.Text += "<script>\n";
            editRecordScript.Text += "    function editRecord(nbr) {\n";
            editRecordScript.Text += "      $('#" + ColumnList[0] + "').val(nbr);\n";
            while (dr.Read())
            {
                RowsReturned++;
                tblResults.Text += "<tr id='tr" + dr.GetValue(0).ToString() + "'><td><a href=\"javascript:editRecord('" + dr.GetValue(0).ToString() + "');\"><img src='edit.png' width='24'></a></td>";
                editRecordScript.Text += "      if (nbr=='" + dr.GetValue(0).ToString() + "') {\n";
                for (int i = 1; i < dr.FieldCount; i++)
                {
                    tblResults.Text += "<td " + AlignmentList[i] + ">";
                    if ((FieldTypeList[i]=="date") || (FieldTypeList[i]=="datetime")) 
                    {
                        try
                        {
                            tblResults.Text += System.Convert.ToDateTime(dr.GetValue(i)).ToString("d");
                        }
                        catch
                        {

                        }
                    }
                    else 
                    {
                        tblResults.Text += dr.GetValue(i).ToString();
                    }
                    tblResults.Text += "</td>";
                    if (FieldTypeList[i] == "bit")
                    {
                        var test = dr.GetValue(i).ToString();
                        if (RowsReturned == 1) 
                        {
                            newRecordScript.Text += "           document.getElementById('" + ColumnList[i] + "').checked = false;\n";
                        }
                        if (dr.GetValue(i).ToString() == "True")
                        {
                            editRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').checked = true;\n";
                        }
                        else
                        {
                            editRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').checked = false;\n";
                        }
                    }
                    else
                    {
                        if (RowsReturned == 1)
                        {
                            newRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').value='" + DefaultList[i] + "';\n";
                        }
                        if (FieldTypeList[i] == "date" || FieldTypeList[i] == "datetime")
                        {
                            if (dr.GetValue(i) == DBNull.Value)
                            {
                                editRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').value='';\n";
                            }
                            else 
                            {
                                editRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').value='" + System.Convert.ToDateTime(dr.GetValue(i)).ToString("d") + "';\n";
                            }
                        }
                        else
                        {
                            if (FieldTypeList[i] == "lookup")
                            {
                                editRecordScript.Text += "          $('#" + ColumnList[i] + " option:contains(\"" +  dr.GetValue(i).ToString() + "\")').attr('selected','selected');\n";
                            }
                            else 
                            {
                                editRecordScript.Text += "          document.getElementById('" + ColumnList[i] + "').value='" + dr.GetValue(i).ToString().Replace(Environment.NewLine,"\\n").Replace("'", "") + "';\n";
                            }
                        }
                    }
                }
                tblResults.Text += "<td><a href=\"javascript:deleteRecord('" + dr.GetValue(0).ToString() + "');\"><img src='delete.png' width='18'></a></td>";
                tblResults.Text += "</tr>\n";
                editRecordScript.Text += "      }\n";
            }
            tblResults.Text += "</tbody></table>";
            lblNumberRows.Text = RowsReturned.ToString();
            connection.Close();    
            editRecordScript.Text += "      $('#editDialog').dialog({ width: 1000, height: 550 });\n";
            editRecordScript.Text += "  }";
            editRecordScript.Text += "</script>\n";
            newRecordScript.Text += "      $('#editDialog').dialog({ width: 1000, height: 550 });\n";
            newRecordScript.Text += "  }";
            newRecordScript.Text += "</script>\n";
        }

        protected void btnApply_Clicked(object sender, EventArgs e)
        {
            refreshPage();
        }

        protected void btnExport_Clicked(object sender, EventArgs e)
        {
            lblMessage.Text += "<script>window.open(\"Export.aspx?sql=" + exportSQL + "\", \"export\");</script>\n";
        }

        protected void btnPrevious_Clicked(object sender, EventArgs e)
        {
            if (System.Convert.ToInt64(pageNumber.Text) > 1)
            {
                pageNumber.Text = (System.Convert.ToInt64(pageNumber.Text) - 1).ToString();
                refreshPage();
            }

        }

        protected void btnNext_Clicked(object sender, EventArgs e)
        {
            if (System.Convert.ToInt64(lblNumberRows.Text) >= System.Convert.ToInt64(searchLimit.Text))
            {
                pageNumber.Text = (System.Convert.ToInt64(pageNumber.Text) + 1).ToString();
                refreshPage();
            }

        }


    }
    public class FieldName
    {
        public string table { get; set; }
        public string name { get; set; }
        public string fieldType { get; set; }
        public string Alignment { get; set; }
    }
}