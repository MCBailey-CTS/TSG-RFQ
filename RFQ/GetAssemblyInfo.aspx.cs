using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class GetAssemblyInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();

            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string assemblyId = Request["AssemblyId"].ToString();
            if (assemblyId.Contains("A"))
            {
                assemblyId = assemblyId.Replace("A", "");
            }
            string assNumber = "";
            string assDescription = "";
            string assemblyType = "";
            List<string> lineNum = new List<string>();


            sql.CommandText = "Select a.assNumber, a.assDescription, at.astAssemblyTypeId, p.prtRFQLineNumber, p.prtPARTID ";
            sql.CommandText += "from linkAssemblyToRFQ atr ";
            sql.CommandText += "inner join tblAssembly a on a.assAssemblyId = atr.atrAssemblyId ";
            sql.CommandText += "inner join pktblAssemblyType at on at.astAssemblyTypeId = a.assType ";
            sql.CommandText += "inner join linkAssemblyToPart atp on atp.atpAssemblyId = a.assAssemblyId ";
            sql.CommandText += "inner join tblPart p on p.prtPartId = atp.atpPartId ";
            sql.CommandText += "where a.assAssemblyId = @assemblyId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@assemblyId", assemblyId);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                assNumber = dr["assNumber"].ToString();
                assDescription = dr["assDescription"].ToString();
                assemblyType = dr["astAssemblyTypeId"].ToString();
                lineNum.Add(dr["prtPARTID"].ToString());
            }
            dr.Close();

            string results = "Number::::" + HttpUtility.JavaScriptStringEncode(assNumber) + "|Description::::" + HttpUtility.JavaScriptStringEncode(assDescription) + "|Type::::" + HttpUtility.JavaScriptStringEncode(assemblyType) + "|LineNumbers::::" + string.Join(",", lineNum);

            litMessages.Text = results;

            connection.Close();

        }
    }
}