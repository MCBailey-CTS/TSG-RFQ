using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class STSPartInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            sql.Connection = connection;
            connection.Open();
            SqlDataReader dr;

            string RFQ = "";
            if (Request["RFQ"] != null)
            {
                RFQ = Request["RFQ"].ToString();
            }

            string PartID = "";
            if (Request["PartID"] != null)
            {
                PartID = Request["PartID"].ToString();
            }

            string Get = "";
            if (Request["Get"] != null)
            {
                Get = Request["Get"].ToString();
            }

            if (Get != "")
            {
                string results = "";
                sql.CommandText = "Select spiAnnualVolume, spiProductionDaysPerYear, spiShiftsPerDay, spiHoursPerShift, spiOEE, spiAwardDate, spiRunoff, spiDeliveryDate, spiPointOfInstallation, ";
                sql.CommandText += "spiUnionWorkplace, spiAvailableData, spiAvailableGDT, spiControlsPLC, spiRobots, spiWelders, spiPositioners, spiCNCMachine ";
                sql.CommandText += "from tblSTSPartInfo where spiRFQID = @rfq  ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQ);
                if (PartID != "")
                {
                    sql.CommandText += "and spiPartID = @part ";
                    sql.Parameters.AddWithValue("@part", PartID);
                }
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    string aDate = dr["spiAwardDate"].ToString();
                    string dDate = dr["spiDeliveryDate"].ToString();
                    if (aDate != "")
                    {
                        aDate = System.Convert.ToDateTime(aDate).ToShortDateString(); 
                    }
                    if (dDate != "")
                    {
                        dDate = System.Convert.ToDateTime(dDate).ToShortDateString();
                    }
                    results = "AnnualVolume||" + dr["spiAnnualVolume"].ToString() + "/r/nProductionDaysPerYear||" + dr["spiProductionDaysPerYear"].ToString() + "/r/nShiftsPerDay||" + dr["spiShiftsPerDay"].ToString();
                    results += "/r/nHoursPerShift||" + dr["spiHoursPerShift"].ToString() + "/r/nOEE||" + dr["spiOEE"].ToString() + "/r/nAwardDate||" + aDate + "/r/nRunoff||" + dr["spiRunoff"].ToString();
                    results += "/r/nDeliveryDate||" + dDate + "/r/nPointOfInstallation||" + dr["spiPointOfInstallation"].ToString() + "/r/nUnionWorkplace||" + dr["spiUnionWorkplace"].ToString();
                    results += "/r/nAvailableData||" + dr["spiAvailableData"].ToString() + "/r/nAvailableGDT||" + dr["spiAvailableGDT"].ToString() + "/r/nControlsPLC||" + dr["spiControlsPLC"].ToString() + "/r/nRobots||" + dr["spiRobots"].ToString();
                    results += "/r/nWelders||" + dr["spiWelders"].ToString() + "/r/nPositioners||" + dr["spiPositioners"].ToString() + "/r/nCNCMachine||" + dr["spiCNCMachine"].ToString();
                }
                dr.Close();
                litResults.Text = results;
                connection.Close();
                return;
            }

            string AnnualVolume = "";
            if (Request["AnnualVolume"] != null)
            {
                AnnualVolume = Request["AnnualVolume"].ToString();
            }

            string ProductionDaysPerYear = "";
            if (Request["ProductionDaysPerYear"] != null)
            {
                ProductionDaysPerYear = Request["ProductionDaysPerYear"].ToString();
            }

            string ShiftsPerDay = "";
            if (Request["ShiftsPerDay"] != null)
            {
                ShiftsPerDay = Request["ShiftsPerDay"].ToString();
            }

            string HoursPerShift = "";
            if (Request["HoursPerShift"] != null)
            {
                HoursPerShift = Request["HoursPerShift"].ToString();
            }

            string OEE = "";
            if (Request["OEE"] != null)
            {
                OEE = Request["OEE"].ToString();
            }

            string AwardDate = "";
            if (Request["AwardDate"] != null)
            {
                AwardDate = Request["AwardDate"].ToString();
            }

            string Runoff = "";
            if (Request["Runoff"] != null)
            {
                Runoff = Request["Runoff"].ToString();
            }

            string DeliveryDate = "";
            if (Request["DeliveryDate"] != null)
            {
                DeliveryDate = Request["DeliveryDate"].ToString();
            }

            string PointOfInstallation = "";
            if (Request["PointOfInstallation"] != null)
            {
                PointOfInstallation = Request["PointOfInstallation"].ToString();
            }

            string UnionWorkplace = "";
            if (Request["UnionWorkplace"] != null)
            {
                UnionWorkplace = Request["UnionWorkplace"].ToString();
            }

            string AvailableData = "";
            if (Request["AvailableData"] != null)
            {
                AvailableData = Request["AvailableData"].ToString();
            }

            string AvailableGDT = "";
            if (Request["AvailableGDT"] != null)
            {
                AvailableGDT = Request["AvailableGDT"].ToString();
            }

            string ControlsPLC = "";
            if (Request["ControlsPLC"] != null)
            {
                ControlsPLC = Request["ControlsPLC"].ToString();
            }

            string Robots = "";
            if (Request["Robots"] != null)
            {
                Robots = Request["Robots"].ToString();
            }

            string Welders = "";
            if (Request["Welders"] != null)
            {
                Welders = Request["Welders"].ToString();
            }

            string Positioners = "";
            if (Request["Positioners"] != null)
            {
                Positioners = Request["Positioners"].ToString();
            }

            string CNCMachine = "";
            if (Request["CNCMachine"] != null)
            {
                CNCMachine = Request["CNCMachine"].ToString();
            }

            string id = "";
            sql.Parameters.Clear();
            sql.CommandText = "Select spiSTSPartInfoID from tblSTSPartInfo where spiRFQID = @rfq ";
            if (PartID != "")
            {
                sql.CommandText += "and spiPartID = @part ";
                sql.Parameters.AddWithValue("@part", PartID);
            }
            sql.Parameters.AddWithValue("@rfq", RFQ);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                id = dr["spiSTSPartInfoID"].ToString();
            }
            dr.Close();

            if (id == "")
            {
                sql.CommandText = "insert into tblSTSPartInfo (spiRFQID, spiPartID, spiAnnualVolume, spiProductionDaysPerYear, spiShiftsPerDay, spiHoursPerShift, spiOEE, spiAwardDate, spiRunoff, ";
                sql.CommandText += "spiDeliveryDate, spiPointOfInstallation, spiUnionWorkplace, spiAvailableData, spiAvailableGDT, spiControlsPLC, spiRobots, spiWelders, spiPositioners, spiCNCMachine, spiCreated, spiCreatedBy) ";
                sql.CommandText += "values (@rfq, @part, @annualVolume, @productionDaysPerYear, @shiftsPerDay, @hoursPerShift, @oee, @awardDate, @runoff, ";
                sql.CommandText += "@deliveryDate, @pointOfInstallation, @unionWorkplace, @availableData, @availableGDT, @controlsPLC, @robots, @welders, @positioners, @cncMachine, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQ);
                if (PartID == "") { sql.Parameters.AddWithValue("@part", DBNull.Value); }
                else { sql.Parameters.AddWithValue("@part", PartID); }
                sql.Parameters.AddWithValue("@annualVolume", AnnualVolume);
                sql.Parameters.AddWithValue("@productionDaysPerYear", ProductionDaysPerYear);
                sql.Parameters.AddWithValue("@shiftsPerDay", ShiftsPerDay);
                sql.Parameters.AddWithValue("@hoursPerShift", HoursPerShift);
                sql.Parameters.AddWithValue("@oee", OEE);
                try { sql.Parameters.AddWithValue("@awardDate", System.Convert.ToDateTime(AwardDate)); }
                catch { sql.Parameters.AddWithValue("@awardDate", DBNull.Value); }
                sql.Parameters.AddWithValue("@runoff", Runoff);
                try { sql.Parameters.AddWithValue("@deliveryDate", System.Convert.ToDateTime(DeliveryDate)); }
                catch { sql.Parameters.AddWithValue("@deliveryDate", DBNull.Value); }
                sql.Parameters.AddWithValue("@pointOfInstallation", PointOfInstallation);
                sql.Parameters.AddWithValue("@unionWorkplace", UnionWorkplace);
                sql.Parameters.AddWithValue("@availableData", AvailableData);
                sql.Parameters.AddWithValue("@availableGDT", AvailableGDT);
                sql.Parameters.AddWithValue("@controlsPLC", ControlsPLC);
                sql.Parameters.AddWithValue("@robots", Robots);
                sql.Parameters.AddWithValue("@welders", Welders);
                sql.Parameters.AddWithValue("@positioners", Positioners);
                sql.Parameters.AddWithValue("@cncMachine", CNCMachine);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            else
            {
                sql.CommandText = "update tblSTSPartInfo set spiAnnualVolume = @annualVolume, spiProductionDaysPerYear = @productionDaysPerYear, spiShiftsPerDay = @shiftsPerDay, spiHoursPerShift = @hoursPerShift, ";
                sql.CommandText += "spiOEE = @oee, spiAwardDate = @awardDate, spiRunoff = @runoff, spiDeliveryDate = @deliveryDate, spiPointOfInstallation = @pointOfInstallation, spiUnionWorkplace = @unionWorkplace, ";
                sql.CommandText += "spiAvailableData = @availableData, spiAvailableGDT = @availableGDT, spiControlsPLC = @controlsPLC, spiRobots = @robots, spiWelders = @welders, ";
                sql.CommandText += "spiPositioners = @positioners, spiCNCMachine = @cncMachine, spiModified = GETDATE(), spiModifiedBy = @user where spiSTSPartInfoID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", id);
                sql.Parameters.AddWithValue("@annualVolume", AnnualVolume);
                sql.Parameters.AddWithValue("@productionDaysPerYear", ProductionDaysPerYear);
                sql.Parameters.AddWithValue("@shiftsPerDay", ShiftsPerDay);
                sql.Parameters.AddWithValue("@hoursPerShift", HoursPerShift);
                sql.Parameters.AddWithValue("@oee", OEE);
                try { sql.Parameters.AddWithValue("@awardDate", System.Convert.ToDateTime(AwardDate)); }
                catch { sql.Parameters.AddWithValue("@awardDate", DBNull.Value); }
                sql.Parameters.AddWithValue("@runoff", Runoff);
                try { sql.Parameters.AddWithValue("@deliveryDate", System.Convert.ToDateTime(DeliveryDate)); }
                catch { sql.Parameters.AddWithValue("@deliveryDate", DBNull.Value); }
                sql.Parameters.AddWithValue("@pointOfInstallation", PointOfInstallation);
                sql.Parameters.AddWithValue("@unionWorkplace", UnionWorkplace);
                sql.Parameters.AddWithValue("@availableData", AvailableData);
                sql.Parameters.AddWithValue("@availableGDT", AvailableGDT);
                sql.Parameters.AddWithValue("@controlsPLC", ControlsPLC);
                sql.Parameters.AddWithValue("@robots", Robots);
                sql.Parameters.AddWithValue("@welders", Welders);
                sql.Parameters.AddWithValue("@positioners", Positioners);
                sql.Parameters.AddWithValue("@cncMachine", CNCMachine);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Edit RFQ");
            }

            connection.Close();
        }
    }
}