using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Globalization;

namespace RFQ
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			litIPChart.Text = "";
			Site master = new Site();
			SqlConnection connection = new SqlConnection(master.getConnectionString());
			connection.Open();
			SqlCommand sql = new SqlCommand();
			sql.Connection = connection;
			sql.CommandText = "select TSGCompanyAbbrev, coalesce(count(*),0), TSGCompanyID from tblRFQ, linkRFQToCompany, TSGCompany where RFQStatus=2 and rtqCompanyID=TSGCompany.TSGCompanyID and rtqRFQID=rfqID and TSGCompanyID <> 1 group by TSGCompanyAbbrev, TSGCompanyID order by TSGCompanyID ";
			SqlDataReader dr = sql.ExecuteReader();
			if (dr.HasRows)
			{
				litIPChart.Text = "<script>\n";
				litIPChart.Text += "var data = [ ";
				Boolean WriteComma = false;
				while (dr.Read() ) {
					if (WriteComma) {
						litIPChart.Text += ",";
					} 
					// for readability
					litIPChart.Text += "\n";
					litIPChart.Text += "{ label: '" + dr.GetValue(0).ToString() + " (" + dr.GetValue(1).ToString() + ")', data: " + dr.GetValue(1).ToString() + "}";
					WriteComma=true;
				}
				litIPChart.Text += "];\n";
				litIPChart.Text += "var placeholder = $('#IP-placeholder');\n";
				litIPChart.Text += "$('#IP-title').text('RFQs In Process');\n";
				litIPChart.Text += "$.plot(placeholder, data, { series: {pie: { show: true, \n";
				litIPChart.Text += "}}});\n";
				litIPChart.Text += "</script>\n";
		   }
			dr.Close();
			connection.Close();
		}
	}
}