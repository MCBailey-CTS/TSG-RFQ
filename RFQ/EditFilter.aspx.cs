using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Optimization;
using System.Data.SqlClient;
using System.Security;

namespace RFQ
{
    public partial class EditFilter : System.Web.UI.Page
    {
        public Int64 filterID = 0;
        public long UserCompanyID = 0;
        List<String> FieldList = new List<String>();
        List<String> OperationList = new List<String>();

        protected void Page_Load(object sender, EventArgs e)
        {

            filterID = System.Convert.ToInt64(Request["id"]);
            
            if (!IsPostBack)
            {

                // create list of operations used in render filter condtion function
                OperationList.Add("eq");
                OperationList.Add("ne");
                OperationList.Add("starts");
                OperationList.Add("ends");
                OperationList.Add("contains");
                OperationList.Add("le");
                OperationList.Add("ge");

                Site master = new RFQ.Site();
                master.setGlobalVariables();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;                
                // get the list of field names to be used by the render filter condition function
                sql.CommandText = "select sfnDisplayName from pktblSystemFieldName order by sfnDisplaySequence ";
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read()) {
                    FieldList.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                if (master.getUserRole()!=5 && filterID != 0) 
                {
                    // If filter is for anyone, then anyone can change it.
                    // Otherwise, you cannot edit it if it is not set for use by you.
                    // there is a protection built into the delete that will not let them delete a filter if they 
                    // are not the ones who created it
                    sql.CommandText = "select fltFilterName from tblFilter where fltFilterID=@filter and  (fltUID=@user  or coalesce(fltUID,0)=0)";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@filter", filterID);
                    sql.Parameters.AddWithValue("@user", master.getUserID());
                    dr = sql.ExecuteReader();
                    Boolean TheyOwnIt = dr.HasRows;
                    dr.Close();
                    if (!TheyOwnIt)
                    {
                        //Response.Redirect("~/Default");
                    }
                }
                // get current values
                sql.CommandText = "select fltFilterName, fltUID, fltMatchAll from tblFilter where fltFilterID=@filter ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", filterID);
                dr = sql.ExecuteReader();
                rbAppliesTo.Items.Add(new ListItem("Me Only", master.getUserID().ToString()));
                while (dr.Read())
                {
                    txtFilterName.Text = dr.GetValue(0).ToString();
                    if (dr.GetValue(1).ToString() != "")
                    {
                        rbAppliesTo.SelectedValue = master.getUserID().ToString();
                    }
                    ddlMatchAll.SelectedValue = dr.GetValue(2).ToString();
                }
                dr.Close();
                // get all current fields defined for this filter
                sql.CommandText = "select sfnSystemFieldNameID, ffiFieldName from pktblfilterfield, pktblSystemFieldName  where ffiFilterID=@filter and ffiFieldName=sfnDisplayName order by ffiDisplaySequence ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", filterID);
                litReportColumns.Text = "<ul id='reportColumnList' style='list-style: none;'>\n";
                dr = sql.ExecuteReader();
                while (dr.Read()) 
                {
                    litReportColumns.Text += "<li  class='chooseColumns' id='rpt" + dr.GetValue(0).ToString() + "' >";
                    litReportColumns.Text += dr.GetValue(1).ToString();
                    litReportColumns.Text += "<sup><div onClick=\"removeThis('rpt" + dr.GetValue(0).ToString() + "');\" style='cursor: pointer; display: inline; z-index: 500; color: red;' alt='Click to Delete'>X</div></sup>";
                    litReportColumns.Text += "</li>\n";
                }
                dr.Close();
                litReportColumns.Text += "</ul>";

                // get all current criteria for this filter
                litCriteria.Text = "<table>\n";
                sql.CommandText = "select fcnConditionID, fcnFieldName, fcnOperation, fcnCondition from pktblfiltercondition where fcnFilterID=@filter order by fcnConditionID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", filterID);
                dr = sql.ExecuteReader();
                Int64 conditionIndex = 0;
                while (dr.Read()) 
                {
                    conditionIndex++;
                    litCriteria.Text += renderCondition(conditionIndex,dr.GetValue(1).ToString(),dr.GetValue(2).ToString(), dr.GetValue(3).ToString());
                }
                dr.Close();

                // render 5 blank criteria rows
                Int64 topval=conditionIndex+5;
                while (conditionIndex < topval) 
                {
                    conditionIndex++;
                    litCriteria.Text += renderCondition(conditionIndex,"","","");
                }
                litCriteria.Text += "</table>";

                // list all available fields that can be added to the filter
                sql.CommandText = "select sfnSystemFieldNameID, sfnDisplayName from pktblSystemFieldName ";
                sql.CommandText += "order by sfnDisplaySequence";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                ddlColumns.DataSource = dr;
                ddlColumns.DataTextField = "sfnDisplayName";
                ddlColumns.DataValueField = "sfnSystemFieldNameID";
                ddlColumns.DataBind();
                ddlColumns.Items.Insert(0,new ListItem("Please Select",""));
                dr.Close();
                connection.Close();
            }
            else
            {
            }
        }

        // render condition rows for the table
        protected string renderCondition(Int64 i, String FieldName, String Operation, String Condition)
        {
            String returnValue = "";
            returnValue += "<tr>";
            returnValue += "<td valign='top'>";
            returnValue += "<div onclick=\"deleteCondition('" + i + "');\"><img src='delete.png' border='0'></div>";
            returnValue += "</td>";
            returnValue += "<td valign='top'>";
            returnValue += "<select id='crit" + i + "' name='crit" + i + "' onchange=\"setValueType('" + i + "');\">";
            returnValue += "<option value=''></option>";
            foreach (String field in FieldList)
            {
                returnValue += "<option ";
                if (field == FieldName)
                {
                    returnValue += " selected ";
                }
                returnValue += " value ='" + field + "'>" + field + "</option>";
            }
            returnValue += "</select>";
            returnValue += "</td>";
            returnValue += "<td valign='top'>";
            returnValue += "<select name='operation" + i + "' id='operation" + i + "'>";
            foreach (String op in OperationList)
            {
                returnValue += "<option value='" + op + "' ";
                if (Operation == op)
                {
                    returnValue += " selected ";
                }
                returnValue += ">";
                if (op == "eq")
                {
                    returnValue += "Equal";
                }
                if (op == "ne")
                {
                    returnValue += "Not Equal";
                }
                if (op == "starts")
                {
                    returnValue += "Starts With";
                }
                if (op == "ends")
                {
                    returnValue += "Ends With";
                }
                if (op == "contains")
                {
                    returnValue += "Contains";
                }
                if (op == "le")
                {
                    returnValue += "Less Than or Equal";
                }
                if (op == "ge")
                {
                    returnValue += "Greater Than or Equal";
                }
                returnValue += "</option>";
            }
            returnValue += "</select>";
            returnValue += "</td>";
            returnValue += "<td valign='top'>";
            returnValue += "<div id='conditionWrapper" + i + "'>";
            if (FieldName != "")
            {
                Site master = new RFQ.Site();
                returnValue += master.renderField(i, FieldName, Condition);
            }
            else
            {
                returnValue += "<input type='hidden' name='condition" + i + "' id='condition" + i + "' value=''>";
            }
            returnValue += "</div>";
            returnValue += "</td>";
            returnValue += "</tr>\n";
            return returnValue;
        }
    }

}