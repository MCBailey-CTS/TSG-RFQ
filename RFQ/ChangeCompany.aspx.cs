using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace RFQ
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string company = "";
            string user = "";
            string temp = "";

            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            bool permissions = false;
            sql.CommandText = "select EmailAddress from permissions where EmailAddress=@user";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@user", master.getUserName());
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (System.Convert.ToBoolean(dr["EmailAddress"].ToString() == "dmaguire@toolingsystemsgroup.com" || dr["EmailAddress"].ToString() == "jdalman@toolingsystemsgroup.com" 
                    || dr["EmailAddress"].ToString() == "bpingle@toolingsystemsgroup.com") || dr["EmailAddress"].ToString() == "bduemler@toolingsystemsgroup.com" || dr["EmailAddress"].ToString() == "gbrouwer@toolingsystemsgroup.com" || dr["EmailAddress"].ToString() == "eoele@toolingsystemsgroup.com")
                {
                    permissions = true;
                }
            }
            dr.Close();

            if (!permissions)
            {
                connection.Close();
                HttpContext.Current.Response.Redirect("~/Permissions.aspx", false);
                return;
            }

            if (!Page.IsPostBack)
            {
                company = master.getCompanyId().ToString();
                user = master.getUserName().ToString();

                sql.CommandText = "Select * from Permissions left join TSGCompany on CompanyID = TSGCompanyID where EmailAddress = @username ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@username", user);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    temp = dr["perName"].ToString() + " your current company is set to " + dr["TSGCompanyName"].ToString();
                }
                dr.Close();

                lblname.Text = temp;

                if (user == "gbrouwer@toolingsystemsgroup.com") {
                    sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID = 5 or TSGCompanyID = 9 ";
                    sql.CommandText += " order by TSGCompanyAbbrev";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    ddlTSGCompany.DataSource = dr;
                    ddlTSGCompany.DataTextField = "TSGCompanyAbbrev";
                    ddlTSGCompany.DataValueField = "TSGCompanyID";
                    ddlTSGCompany.DataBind();
                    dr.Close();
                }
                if (user == "eoele@toolingsystemsgroup.com")
                {
                    sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID = 2 or TSGCompanyID = 9 ";
                    sql.CommandText += " order by TSGCompanyAbbrev";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    ddlTSGCompany.DataSource = dr;
                    ddlTSGCompany.DataTextField = "TSGCompanyAbbrev";
                    ddlTSGCompany.DataValueField = "TSGCompanyID";
                    ddlTSGCompany.DataBind();
                    dr.Close();
                }
                else
                {
                    sql.CommandText = "Select TSGCompanyID, TSGCompanyAbbrev from TSGCompany where TSGCompanyID < 16 and TSGCompanyID <> 4 and TSGCompanyID <> 6 and ";
                    sql.CommandText += "TSGCompanyID <> 11  order by TSGCompanyAbbrev";
                    sql.Parameters.Clear();
                    dr = sql.ExecuteReader();
                    ddlTSGCompany.DataSource = dr;
                    ddlTSGCompany.DataTextField = "TSGCompanyAbbrev";
                    ddlTSGCompany.DataValueField = "TSGCompanyID";
                    ddlTSGCompany.DataBind();
                    dr.Close();
                }
                //ddlTSGCompany.Enabled = false;
                ddlTSGCompany.SelectedValue = company;
            }

        }

        protected void btnChangeCompany_Click(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            string user = master.getUserName().ToString();
            string company = ddlTSGCompany.SelectedValue.ToString();
            string temp = "";

            sql.CommandText = "Update Permissions set companyID = @company where EmailAddress = @username ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@username", user);
            sql.Parameters.AddWithValue("@company", company);
            SqlDataReader dr = sql.ExecuteReader();
            dr.Close();

            sql.CommandText = "Select * from Permissions left join TSGCompany on CompanyID = TSGCompanyID where EmailAddress = @username ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@username", user);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                temp = dr["perName"].ToString() + " your current company is set to " + dr["TSGCompanyName"].ToString();
            }
            dr.Close();

            lblname.Text = temp;
        }
    }
}