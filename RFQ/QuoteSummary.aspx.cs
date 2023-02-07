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
    public partial class QuoteSummary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            litJSOpenReport.Text = "";
            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.AddMonths(-6).ToString("d");
                txtEndDate.Text = DateTime.Now.ToString("d");

                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer order by CustomerName ";
                SqlDataReader CustomerDR = sql.ExecuteReader();
                ddlCustomer.DataSource = CustomerDR;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "Please Select");
                CustomerDR.Close();

                sql.CommandText = "Select TSGCompanyAbbrev, TSGCompanyID from TSGCompany where tcoActive = 1 ";
                SqlDataReader dr = sql.ExecuteReader();
                ddlCompany.DataSource = dr;
                ddlCompany.DataTextField = "TSGCompanyAbbrev";
                ddlCompany.DataValueField = "TSGCompanyID";
                ddlCompany.DataBind();
                dr.Close();

                ddlPlant.Items.Insert(0, "Please Select");
            }
        }
        protected void btnQuoteRecap_Click(object sender, EventArgs e)
        {
            //litJSOpenReport.Text = "\n<script>window.open('/QuoteRecap.ashx?reserved=0&start=" + txtStartDate.Text + "&end=" + txtEndDate.Text + "');</script>\n";
            litJSOpenReport.Text = "\n<script>quoteSummary('" + txtStartDate.Text + "', '" + txtEndDate.Text + "');</script>";
        }

        protected void btnRecap_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Reporting");
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }

        protected void populate_Plants()
        {
            if(ddlCustomer.SelectedItem.ToString() != "Please Select")
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;
                sql.CommandText = "select CustomerLocationID, Concat(ShipToName, ' (',ShipCode,')', ' - ', Address1,', ', City,', ', State) as Location from CustomerLocation where CustomerID=@customer  order by Location";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                SqlDataReader plantDR = sql.ExecuteReader();
                ddlPlant.DataSource = plantDR;
                ddlPlant.DataTextField = "Location";
                ddlPlant.DataValueField = "CustomerLocationID";
                ddlPlant.DataBind();
                plantDR.Close();
                ddlPlant.Items.Insert(0, "Please Select");
                ddlPlant.SelectedIndex = 0;

                connection.Close();
            }
            else
            {
                ddlPlant.Items.Clear();
                ddlPlant.Items.Insert(0, "Please Select");
            }
        }
    }

}