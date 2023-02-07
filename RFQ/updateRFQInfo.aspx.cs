using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class updateRFQInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            string program = "", oem = "", rfq = "", vehicle = "";

            if(Request["program"] != "" && Request["program"] != null)
            {
                program = Request["program"];
            }
            if (Request["oem"] != "" && Request["oem"] != null)
            {
                oem = Request["oem"];
            }
            if (Request["vehicle"] != "" && Request["vehicle"] != null)
            {
                vehicle = Request["vehicle"];
            }
            rfq = Request["rfq"];

            sql.CommandText = "update tblRFQ set rfqProgramID = @program, rfqOEMID = @oem, rfqVehicleID = @vehicle where rfqID = @rfq";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@program", program);
            sql.Parameters.AddWithValue("@oem", oem);
            sql.Parameters.AddWithValue("@vehicle", vehicle);
            sql.Parameters.AddWithValue("@rfq", rfq);
            master.ExecuteNonQuery(sql, "Update RFQ Info");

            connection.Close();
        }
    }
}