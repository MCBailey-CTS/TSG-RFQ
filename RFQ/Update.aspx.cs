using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class Update : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        protected void load()
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            List<updateList> l = new List<updateList>();
            sql.CommandText = "Select rfqID, CustomerName, ShipToName, rfqCustomerRFQNumber from tblRFQ, Customer, CustomerLocation where CustomerLocationID = rfqPlantID and Customer.CustomerID = rfqCustomerID ";
            sql.CommandText += "order by rfqID desc";
            SqlDataReader dr = sql.ExecuteReader();
            int count = 0;
            while (dr.Read())
            {
                updateList update = new updateList();
                update.rfqID = dr.GetValue(0).ToString();
                update.customer = dr.GetValue(1).ToString();
                update.plant = dr.GetValue(2).ToString();
                update.custRFQNum = dr.GetValue(3).ToString();
                update.update = "<div id='btn'" + count.ToString() + " class='mybutton' onclick='updateProgram(" + count.ToString() + ", " + dr.GetValue(0).ToString() + ")'>UPDATE</div>";
                l.Add(update);
                count++;
            }
            dr.Close();

            dgResults.DataSource = l;
            dgResults.DataBind();

            connection.Close();
        }

        protected void addProgram(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "insert into Program (ProgramName, proCreated, proCreatedBy) ";
            sql.CommandText += "values(@name, GETDATE(), @user)";
            //sql.Parameters.AddWithValue("@name", txtProgram.Text);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            master.ExecuteNonQuery(sql, "Update");

            connection.Close();

            load();
        }

        protected void addOEM(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "insert into OEM (OEMNAme, oemCreated, oemCreatedBy) ";
            sql.CommandText += "values(@name, GETDATE(), @user)";
            //sql.Parameters.AddWithValue("@name", txtOEM.Text);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            master.ExecuteNonQuery(sql, "Update");

            connection.Close();

            load();
        }

        protected void OnRowDataBound(Object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "Select ProgramName, ProgramID from Program order by ProgramName";
            SqlDataReader tsgCompanyDR = sql.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(tsgCompanyDR);

            foreach (GridViewRow row in dgResults.Rows)
            {
                //Find the DropDownList in the Row
                DropDownList ddlReserve = (row.FindControl("ddlProgram") as DropDownList);
                ddlReserve.DataSource = dt;
                ddlReserve.DataTextField = "ProgramName";
                ddlReserve.DataValueField = "ProgramID";
                ddlReserve.DataBind();
            }
            tsgCompanyDR.Close();

            sql.CommandText = "Select OEMName, OEMID from OEM order by OEMName";
            sql.Parameters.Clear();
            SqlDataReader dr = sql.ExecuteReader();
            dt = new DataTable();
            dt.Load(dr);

            foreach (GridViewRow row in dgResults.Rows)
            {
                DropDownList ddlOEM = (row.FindControl("ddlOEM") as DropDownList);

                ddlOEM.DataSource = dt;
                ddlOEM.DataTextField = "OEMName";
                ddlOEM.DataValueField = "OEMID";
                ddlOEM.DataBind();
            }
            dr.Close();

            sql.CommandText = "Select vehVehicleName, vehVehicleID from pktblVehicle order by vehVehicleName";
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            dt = new DataTable();
            dt.Load(dr);
            foreach (GridViewRow row in dgResults.Rows)
            {
                DropDownList ddlVehicle = (row.FindControl("ddlVehicle") as DropDownList);

                ddlVehicle.DataSource = dt;
                ddlVehicle.DataTextField = "vehVehicleName";
                ddlVehicle.DataValueField = "vehVehicleID";
                ddlVehicle.DataBind();
            }
            dr.Close();

            List<string> program = new List<string>();
            List<string> oem = new List<string>();
            List<string> vehicle = new List<string>();

            sql.CommandText = "Select rfqProgramID, rfqOEMID, rfqVehicleID from tblRFQ order by rfqID desc";
            sql.Parameters.Clear();
            tsgCompanyDR = sql.ExecuteReader();
            while (tsgCompanyDR.Read())
            {
                program.Add(tsgCompanyDR.GetValue(0).ToString());
                oem.Add(tsgCompanyDR.GetValue(1).ToString());
                vehicle.Add(tsgCompanyDR.GetValue(2).ToString());
            }
            tsgCompanyDR.Close();


            int count = 0;
            foreach (GridViewRow row in dgResults.Rows)
            {
                DropDownList ddlReserve = (row.FindControl("ddlProgram") as DropDownList);
                DropDownList ddlOEM = (row.FindControl("ddlOEM") as DropDownList);
                DropDownList ddlVehicle = (row.FindControl("ddlVehicle") as DropDownList);

                ddlOEM.SelectedValue = oem[count];
                ddlReserve.SelectedValue = program[count];
                ddlVehicle.SelectedValue = vehicle[count];
                count++;
            }

            connection.Close();

        }
    }

    public class updateList
    {
        public string rfqID { get; set; }
        public string customer { get; set; }
        public string plant { get; set; }
        public string custRFQNum { get; set; }
        public string update { get; set; }
        public updateList()
        {
            rfqID = "";
            customer = "";
            plant = "";
            custRFQNum = "";
            update = "";
        }
    }
}