using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class UpdateFilter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // How To Call
            // UdateFilter.aspx?filter=0&name=TEST FILTER 1&uid=0&crit1=Date Received&op1=ge&cond1=11/01/2014&crit2=Part Number&op2=contains&cond2=3500&crit3=&op3=null&cond3=&crit4=&op4=eq&cond4=&crit5=&op5=eq&cond5=&ccount=5&anyall=0&fields=rpt1,rpt2,rpt5,rpt11,rpt18,rpt13,rpt19
            String filterID = Request["filter"];
            String FilterName=Request["name"];
            String AnyAll = Request["anyall"];
            String AppliesTo = Request["uid"];
            Int64 criteriaCount = System.Convert.ToInt64(Request["ccount"]);
            String FieldNames = Request["fields"];
            string[] Fields = FieldNames.Split(',');
            Site master = new Site();
            master.setGlobalVariables();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            if (filterID == "0")
            {
                FilterName = getUniqueFilterName(FilterName,"0");
                sql.Parameters.Clear();
                if (AppliesTo != "0") {
                    sql.CommandText = "insert into tblFilter (fltFilterName, fltTableName, fltmatchAll, filCreated, filCreatedBy, fltUID) ";
                    sql.CommandText += " OUTPUT INSERTED.fltFilterID ";
                    sql.CommandText += " values (@name, 'RFQ', @match, current_timestamp, @user, @applies) ";
                    sql.Parameters.AddWithValue("@name", FilterName);
                    sql.Parameters.AddWithValue("@match", AnyAll);
                    sql.Parameters.AddWithValue("@user", master.UserID);
                    sql.Parameters.AddWithValue("@applies", AppliesTo);
                } else {
                    sql.CommandText = "insert into tblFilter (fltFilterName, fltTableName, fltmatchAll, filCreated, filCreatedBy) ";
                    sql.CommandText += " OUTPUT INSERTED.fltFilterID ";
                    sql.CommandText += " values (@name, 'RFQ', @match, current_timestamp, @user) ";
                    sql.Parameters.AddWithValue("@name", FilterName);
                    sql.Parameters.AddWithValue("@match", AnyAll);
                    sql.Parameters.AddWithValue("@user", master.UserID.ToString());
                }
                filterID = (master.ExecuteScalar(sql,"UpdateFilter")).ToString();
            }
            else
            {
                FilterName=getUniqueFilterName(FilterName, filterID);
                sql.Parameters.Clear();
                sql.CommandText = "update tblfilter set fltFilterName=@name";
                sql.CommandText += ", fltMatchAll=@match";
                sql.CommandText += ", filModified=current_timestamp";
                sql.CommandText += ", filModifiedBy=@user";
                if (AppliesTo != "0") {
                    sql.CommandText += ", fltUID=@applies";
                    sql.Parameters.AddWithValue("@applies", AppliesTo);
                }
                sql.CommandText += " where fltFilterID=@filter";
                sql.Parameters.AddWithValue("@name", FilterName);
                sql.Parameters.AddWithValue("@match", AnyAll);
                sql.Parameters.AddWithValue("@user",master.UserID);
                sql.Parameters.AddWithValue("@filter",filterID);
                master.ExecuteNonQuery(sql,"UpdateFilter");
            }
            // first clear all fields for this filter
            sql.CommandText = "delete from pktblFilterField where ffiFilterID=@filter";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@filter", filterID);
            master.ExecuteNonQuery(sql,"UpdateFilter");
            Int64 Sequence = 10;
            foreach (String field in Fields) 
            {
                sql.CommandText = "select sfnDisplayName from pktblSystemFieldName where sfnSystemFieldNameID=@id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", field.Replace("rpt", ""));
                SqlDataReader fdr = sql.ExecuteReader();
                String FieldName = field;
                while (fdr.Read())
                {
                    FieldName = fdr.GetValue(0).ToString();
                }
                fdr.Close();
                sql.CommandText = "insert into pktblfilterfield (ffiFilterID, ffiFieldName, ffiDisplaySequence, ffiCreated, ffiCreatedBy) ";
                sql.CommandText += " values (@filter, @field, @seq, current_timestamp, @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", filterID);
                sql.Parameters.AddWithValue("@field", FieldName);
                sql.Parameters.AddWithValue("@seq", Sequence);
                sql.Parameters.AddWithValue("@user", master.UserID);
                Sequence += 10;
                master.ExecuteNonQuery(sql,"UpdateFilter");
            }
            // clear all existing conditions.
            sql.CommandText = "delete from pktblFilterCondition where fcnFilterID=@filter";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@filter", filterID);
            master.ExecuteNonQuery(sql,"UpdateFilter");
            Int64 i = 0;
            while (i < criteriaCount ) {
                i++;
                String Field = Request["crit" + i.ToString()];
                String Condition = Request["cond"+i.ToString()];
                String Operation = Request["op"+i.ToString()];
                if (Field != "")
                {
                    sql.CommandText = "insert into pktblFilterCondition (fcnFilterID, fcnFieldName, fcnOperation, fcnCondition, fcoCreated, fcoCreatedBy) ";
                    sql.CommandText += " values (@filter, @field, @op, @cond, current_timestamp, @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@filter", filterID);
                    sql.Parameters.AddWithValue("@field", Field);
                    sql.Parameters.AddWithValue("@op", Operation);
                    sql.Parameters.AddWithValue("@cond", Condition);
                    sql.Parameters.AddWithValue("@user", master.UserID);
                    master.ExecuteNonQuery(sql,"UpdateFilter");
                }
            }
            connection.Close();
        }

        // see if the name is taken if so add a suffix to the end so we do not have duplicate filter names
        protected String getUniqueFilterName(String FilterName, String FilterID)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            String TestName = FilterName;
            sql.CommandText = "select * from tblfilter where fltFilterName=@name";
            sql.CommandText += " and fltFilterId <> @filter";
            Boolean taken = true;
            Int64 suffix = 0;
            while (taken)
            {
                taken = false;
                if (suffix > 0)
                {
                    TestName = FilterName + "-" + suffix.ToString();
                }
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@filter", FilterID);
                sql.Parameters.AddWithValue("@name", TestName);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.HasRows) {
                    suffix++;
                    taken = true;
                }
                dr.Close();
            }
            connection.Close();
            return TestName;
        }
    }
}