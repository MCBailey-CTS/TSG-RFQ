using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using NPOI.XSSF.UserModel;

namespace RFQ
{
    /// <summary>
    /// Summary description for UploadCustomerResponsibility
    /// </summary>
    public class UploadCustomerResponsibility : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string toReturn = "";
            if (context.Request.Files.Count <= 0)
            {
                context.Response.Write("No file Uploaded");
            }
            else
            {
                for (int i = 0; i < context.Request.Files.Count; ++i)
                {
                    HttpPostedFile file = context.Request.Files[i];
                    XSSFWorkbook wb = new XSSFWorkbook(file.InputStream);
                    XSSFSheet sh = (XSSFSheet)wb.GetSheet("Customer Responsability List");
                    if (sh != null)
                    {
                        toReturn += processUpdate(context, sh);
                    }
                    sh = (XSSFSheet)wb.GetSheet("Add New Customers");
                    if (sh != null)
                    {
                        toReturn += processAdd(context, sh);
                    }
                    sh = (XSSFSheet)wb.GetSheet("Add New Competitor");
                    if (sh != null)
                    {
                        toReturn += competitorAdd(context, sh);
                    }
                    sh = (XSSFSheet)wb.GetSheet("Competitor Upload");
                    if (sh != null)
                    {
                        toReturn += competitorUpdate(context, sh);
                    }
                    sh = (XSSFSheet)wb.GetSheet("RFQ Update");
                    if (sh != null)
                    {
                        toReturn += processRFQUpdate(context, sh);
                    }
                    sh = (XSSFSheet)wb.GetSheet("Original");
                    if (sh != null)
                    {
                        toReturn += UpdateChristmas(context, sh);
                    }

                }
            }
            if (toReturn == "")
            {
                context.Response.Write("There was nothing flagged to update, delete or add");
            }
            else
            {
                context.Response.Write(toReturn);
            }
        }

        public string processRFQUpdate(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string stringToReturn = "Updated RFQs: ";
            string user = master.getUserName();

            if (sh != null)
            {
                int i = 1;
                while (sh.GetRow(i) != null)
                {
                    if (i > 40000)
                    {
                        connection.Close();
                        return "Hit file limit";
                    }
                    try
                    {
                        if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Update")
                        {
                            string rfqID = "";
                            string program = "";
                            string oem = "";
                            string vehicle = "";

                            try
                            {
                                rfqID = master.readCellInt(sh.GetRow(i).GetCell(1)).ToString();
                                if (rfqID == "-1")
                                {
                                    return "There was an incorrect character in the RFQ ID column.";
                                }
                            }
                            catch
                            {
                                return "There was an issue reading in the RFQ ID.";
                            }

                            program = master.readCellInt(sh.GetRow(i).GetCell(2)).ToString();
                            if (program == "-1")
                            {
                                program = master.readCellDouble(sh.GetRow(i).GetCell(2)).ToString();
                            }
                            if (program == "-1")
                            {
                                program = master.readCellString(sh.GetRow(i).GetCell(2)).ToString();
                            }

                            oem = master.readCellString(sh.GetRow(i).GetCell(3)).ToString();
                            vehicle = master.readCellInt(sh.GetRow(i).GetCell(4)).ToString();
                            if (vehicle == "-1")
                            {
                                vehicle = master.readCellDouble(sh.GetRow(i).GetCell(4)).ToString();
                            }
                            if (vehicle == "-1")
                            {
                                vehicle = master.readCellString(sh.GetRow(i).GetCell(4)).ToString();
                            }

                            int programID = 0;
                            int oemID = 0;
                            int vehicleID = 0;

                            sql.CommandText = "Select top 1 ProgramId, OEMID, vehVehicleID from OEM ";
                            sql.CommandText += "left outer join Program on ProgramName = @program ";
                            sql.CommandText += "left outer join pktblVehicle on vehVehicleName = @vehicle ";
                            sql.CommandText += "where OEMName = @oem ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@program", program);
                            sql.Parameters.AddWithValue("@vehicle", vehicle);
                            sql.Parameters.AddWithValue("@oem", oem);
                            SqlDataReader dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                if (dr.GetValue(0).ToString() != "")
                                {
                                    programID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                                }
                                oemID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                                if (dr.GetValue(2).ToString() != "")
                                {
                                    vehicleID = System.Convert.ToInt32(dr.GetValue(2).ToString());
                                }
                            }
                            dr.Close();

                            if (programID == 0)
                            {
                                sql.CommandText = "insert into Program (ProgramName, proCreated, proCreatedBy) ";
                                sql.CommandText += "output inserted.ProgramID ";
                                sql.CommandText += "values (@programName, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@programName", program);
                                sql.Parameters.AddWithValue("@user", user);
                                programID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Upload Customer Responsibility").ToString());
                            }
                            if (vehicleID == 0)
                            {
                                sql.CommandText = "insert into pktblVehicle (vehVehicleName, vehCreated, vehCreatedBy) ";
                                sql.CommandText += "output inserted.vehVehicleID ";
                                sql.CommandText += "values (@vehicle, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@vehicle", vehicle);
                                sql.Parameters.AddWithValue("@user", user);
                                vehicleID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Upload Customer Responsibility").ToString());
                            }
                            if (programID != 0 && oemID != 0 && vehicleID != 0)
                            {
                                sql.CommandText = "update tblRFQ set rfqProgramID = @programID, rfqOEMID = @oemID, rfqVehicleID = @vehicleID, ";
                                sql.CommandText += "rfqModified = GETDATE(), rfqModifiedBy = @user where rfqID = @rfqID ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@programID", programID);
                                sql.Parameters.AddWithValue("@oemID", oemID);
                                sql.Parameters.AddWithValue("@vehicleID", vehicleID);
                                sql.Parameters.AddWithValue("@user", user);
                                sql.Parameters.AddWithValue("@rfqID", rfqID);
                                master.ExecuteNonQuery(sql, "Upload Customer Responsibility");
                                if (stringToReturn != "Updated RFQs: ")
                                {
                                    stringToReturn += ", " + rfqID.ToString();
                                }
                                else
                                {
                                    stringToReturn += rfqID.ToString();
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        return "An error has occurred please give this error to an administrator. " + e.ToString();
                    }
                    i++;
                }
            }

            connection.Close();
            return stringToReturn;
        }

        public string processUpdate(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            Dictionary<string, string> salesmanLookup = new Dictionary<string, string>();
            sql.CommandText = "Select Name, TSGSalesmanID from TSGSalesman ";
            sql.Parameters.Clear();
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                salesmanLookup.Add(dr["Name"].ToString(), dr["TSGSalesmanID"].ToString());
            }
            dr.Close();

            string user = master.getUserName();

            String stringToReturn = "";
            if (sh != null)
            {
                // Skip header
                int i = 1;

                while (sh.GetRow(i) != null)
                {
                    if (i > 40000)
                    {
                        return "Hit limit of cells";
                    }
                    try
                    {
                        if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Update")
                        {
                            string customerName = "";
                            string customerNumber = "";
                            string plantName = "";
                            string shipCode = "";
                            string salesman = "";
                            string salesman2 = "";
                            string address1 = "";
                            string address2 = "";
                            string address3 = "";
                            string city = "";
                            string state = "";
                            string country = "";
                            string zip = "";
                            string rank = "";
                            string locationID = "";

                            customerName = master.readCellString(sh.GetRow(i).GetCell(1));
                            customerNumber = master.readCellString(sh.GetRow(i).GetCell(2));
                            plantName = master.readCellString(sh.GetRow(i).GetCell(3));
                            shipCode = master.readCellString(sh.GetRow(i).GetCell(4));
                            salesman = master.readCellString(sh.GetRow(i).GetCell(5));
                            salesman2 = master.readCellString(sh.GetRow(i).GetCell(6));
                            address1 = master.readCellString(sh.GetRow(i).GetCell(7));
                            address2 = master.readCellString(sh.GetRow(i).GetCell(8));
                            address3 = master.readCellString(sh.GetRow(i).GetCell(9));
                            city = master.readCellString(sh.GetRow(i).GetCell(10));
                            state = master.readCellString(sh.GetRow(i).GetCell(11));
                            country = master.readCellString(sh.GetRow(i).GetCell(12));
                            zip = master.readCellString(sh.GetRow(i).GetCell(13));
                            rank = master.readCellString(sh.GetRow(i).GetCell(14));
                            locationID = master.readCellString(sh.GetRow(i).GetCell(30));

                            int salesmanID = 0;
                            int salesmanID2 = 0;
                            int rankID = 0;
                            try
                            {
                                salesmanID = System.Convert.ToInt32(salesmanLookup[salesman]);
                            }
                            catch (Exception ex)
                            {
                                i++;
                                continue;
                            }

                            if (salesman2 != "")
                            {
                                try
                                {
                                    salesmanID2 = System.Convert.ToInt32(salesmanLookup[salesman2]);
                                }
                                catch
                                {
                                    i++;
                                    continue;
                                }
                            }

                            if (salesmanID != 0)
                            {
                                sql.CommandText = "Update CustomerLocation set ShipToName = @name, ShipCode = @code, TSGSalesmanID = @salesman, ";
                                sql.CommandText += "Address1 = @ad1, Address2 = @ad2, Address3 = @ad3, City = @city, State = @state, ";
                                sql.CommandText += "Country = @country, Zip = @zip, CustomerRankID = @rank ";
                                sql.CommandText += "where CustomerLocationID = @id ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@name", plantName);
                                sql.Parameters.AddWithValue("@code", shipCode);
                                sql.Parameters.AddWithValue("@salesman", salesmanID);
                                sql.Parameters.AddWithValue("@ad1", address1);
                                sql.Parameters.AddWithValue("@ad2", address2);
                                sql.Parameters.AddWithValue("@ad3", address3);
                                sql.Parameters.AddWithValue("@city", city);
                                sql.Parameters.AddWithValue("@state", state);
                                sql.Parameters.AddWithValue("@country", country);
                                sql.Parameters.AddWithValue("@zip", zip);
                                sql.Parameters.AddWithValue("@rank", rankID);
                                sql.Parameters.AddWithValue("@id", locationID);
                                master.ExecuteNonQuery(sql, "Upload Customer Responsibility");

                                sql.CommandText = "delete from linkSalesmanToCustomerLocation where sclCustomerLocationId = @plant ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@plant", locationID);
                                master.ExecuteNonQuery(sql, "Upload Customer Responsibility");

                                if (salesmanID2 != salesmanID && salesmanID2 != 0)
                                {
                                    sql.CommandText = "insert into linkSalesmanToCustomerLocation (sclSalesmanId, sclCustomerLocationId, sclCreated, sclCreatedBy) ";
                                    sql.CommandText += "values(@salesman, @plant, GETDATE(), @user) ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@salesman", salesmanID2);
                                    sql.Parameters.AddWithValue("@plant", locationID);
                                    sql.Parameters.AddWithValue("@user", user);
                                    master.ExecuteNonQuery(sql, "Update Customer Responsibility");
                                }
                                
                                stringToReturn += "Updated: " + customerName + " " + plantName + "\n";
                            }
                        }
                        else if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Delete")
                        {
                            string locationID = "";

                            locationID = master.readCellString(sh.GetRow(i).GetCell(30));

                            sql.CommandText = "Delete from CustomerLocation where CustomerLocationID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", locationID);
                            master.ExecuteNonQuery(sql, "Update Customer Responsibility List");

                            sql.CommandText = "delete from linkSalesmanToCustomerLocation where sclCustomerLocationId = @plant ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@plant", locationID);
                            master.ExecuteNonQuery(sql, "Upload Customer Responsibility");

                            stringToReturn += "Deleted: " + master.readCellString(sh.GetRow(i).GetCell(3)) + "\n";
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    if (stringToReturn != "")
                    {
                        stringToReturn = "The Responsibility list has been updated.";
                    }
                    i++;
                }
            }

            connection.Close();
            return stringToReturn;
        }

        public string processAdd(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            Dictionary<string, string> salesmanLookup = new Dictionary<string, string>();
            sql.CommandText = "Select Name, TSGSalesmanID from TSGSalesman ";
            sql.Parameters.Clear();
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                salesmanLookup.Add(dr["Name"].ToString(), dr["TSGSalesmanID"].ToString());
            }
            dr.Close();

            string user = master.getUserName();

            string toReturn = "";

            if (sh != null)
            {
                int i = 1;

                while (sh.GetRow(i) != null)
                {
                    if (i > 5000)
                    {
                        return "Hit limit of cells";
                    }
                    try
                    {
                        if (master.readCellString(sh.GetRow(i).GetCell(0)) != "")
                        {
                            Boolean exists = false;

                            //string customerNumber = master.readCellString(sh.GetRow(i).GetCell(1));
                            char pad = '0';
                            string customerNumber = master.readCellInt(sh.GetRow(i).GetCell(1)).ToString().PadLeft(7, pad);
                            string customerName = master.readCellString(sh.GetRow(i).GetCell(0));
                            int customerID = 0;
                            int salesmanID = 0;
                            int salesmanID2 = 0;
                            int rankID = 0;

                            sql.CommandText = "Select CustomerName, CustomerID from Customer where CustomerNumber = @number ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@number", customerNumber);
                            dr = sql.ExecuteReader();
                            if (dr.Read())
                            {
                                exists = true;
                                customerID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                            }
                            dr.Close();

                            if (!exists)
                            {
                                sql.CommandText = "insert into Customer (CustomerNumber, CustomerName, cusCreated, cusCreatedBy) ";
                                sql.CommandText += "output inserted.CustomerID ";
                                sql.CommandText += "values(@number, @name, GETDATE(), @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@number", customerNumber);
                                sql.Parameters.AddWithValue("@name", customerName);
                                sql.Parameters.AddWithValue("@user", master.getUserName());
                                try
                                {
                                    customerID = System.Convert.ToInt32(master.ExecuteScalar(sql, "Upload Customer Responsibility").ToString());
                                    toReturn += "Inserted Customer: " + customerName + " " + customerNumber + "\n";
                                }
                                catch
                                {
                                    toReturn += "We had an issue uploading " + customerName + " " + customerNumber + "\n";
                                }
                            }

                            try
                            {
                                salesmanID2 = System.Convert.ToInt32(salesmanLookup[master.readCellString(sh.GetRow(i).GetCell(5))]);
                            }
                            catch 
                            {
                                
                            }

                            try
                            {
                                sql.CommandText = "Select TSGSalesmanID, CustomerRankID from TSGSalesman, CustomerRank where name = @name and Rank = @rank ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@name", master.readCellString(sh.GetRow(i).GetCell(4)));
                                sql.Parameters.AddWithValue("@rank", master.readCellString(sh.GetRow(i).GetCell(13)));
                                dr = sql.ExecuteReader();
                                if (dr.Read())
                                {
                                    salesmanID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                                    rankID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                                }
                                dr.Close();


                                if (customerID != 0)
                                {
                                    sql.CommandText = "insert into Customerlocation (ShipToName, ShipCode, Address1, Address2, Address3, City, State, ";
                                    sql.CommandText += "zip, country, CustomerNumber, CustomerID, TSGSalesmanID, CustomerRankID, cloCreated, cloCreatedBy, TSGHouseAccountID) ";
                                    sql.CommandText += "output inserted.CustomerLocationId ";
                                    sql.CommandText += "values (@name, @code, @ad1, @ad2, @ad3, @city, @state, @zip, @country, @customerNumber, ";
                                    sql.CommandText += "@customerID, @salesman, @rank, GETDATE(), @user, '16') ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@name", master.readCellString(sh.GetRow(i).GetCell(2)));
                                    if (master.readCellInt(sh.GetRow(i).GetCell(3)) < 10)
                                    {
                                        sql.Parameters.AddWithValue("@code", "000" + master.readCellInt(sh.GetRow(i).GetCell(3)).ToString());
                                    }
                                    else if (master.readCellInt(sh.GetRow(i).GetCell(3)) < 100)
                                    {
                                        sql.Parameters.AddWithValue("@code", "00" + master.readCellInt(sh.GetRow(i).GetCell(3)).ToString());
                                    }
                                    else if (master.readCellInt(sh.GetRow(i).GetCell(3)) < 1000)
                                    {
                                        sql.Parameters.AddWithValue("@code", "0" + master.readCellInt(sh.GetRow(i).GetCell(3)).ToString());
                                    }
                                    else
                                    {
                                        sql.Parameters.AddWithValue("@code", master.readCellInt(sh.GetRow(i).GetCell(3)));
                                    }
                                    sql.Parameters.AddWithValue("@ad1", master.readCellString(sh.GetRow(i).GetCell(6)));
                                    sql.Parameters.AddWithValue("@ad2", master.readCellString(sh.GetRow(i).GetCell(7)));
                                    sql.Parameters.AddWithValue("@ad3", master.readCellString(sh.GetRow(i).GetCell(8)));
                                    sql.Parameters.AddWithValue("@city", master.readCellString(sh.GetRow(i).GetCell(9)));
                                    sql.Parameters.AddWithValue("@state", master.readCellString(sh.GetRow(i).GetCell(10)));
                                    sql.Parameters.AddWithValue("@country", master.readCellString(sh.GetRow(i).GetCell(11)));
                                    sql.Parameters.AddWithValue("@zip", master.readCellString(sh.GetRow(i).GetCell(12)));
                                    sql.Parameters.AddWithValue("@customerNumber", customerNumber);
                                    sql.Parameters.AddWithValue("@customerID", customerID);
                                    sql.Parameters.AddWithValue("@salesman", salesmanID);
                                    sql.Parameters.AddWithValue("@rank", rankID);
                                    sql.Parameters.AddWithValue("@user", master.getUserName());

                                    string locationID = master.ExecuteScalar(sql, "Upload Customer Responsibility").ToString();

                                    //if (master.readCellString(sh.GetRow(i).GetCell(13)) != "") {
                                    //    string programManager = master.readCellString(sh.GetRow(i).GetCell(13));
                                    //    string ProgramManagerID = "0";
                                    //    sql.CommandText = "Select TSGProgramManagerID from TSGProgramManager where Name = @ProgramManagerName ";
                                    //    sql.Parameters.AddWithValue("@ProgramManagerName", programManager);
                                    //    dr = sql.ExecuteReader();
                                    //    if (dr.Read())
                                    //    {
                                    //        ProgramManagerID = dr.GetValue(0).ToString();
                                    //    }
                                    //    dr.Close();
                                    //sql.Parameters.AddWithValue("@ProgramManager", ProgramManagerID);
                                    //}

                                    if (salesmanID2 != salesmanID && salesmanID2 != 0)
                                    {
                                        sql.CommandText = "insert into linkSalesmanToCustomerLocation (sclSalesmanId, sclCustomerLocationId, sclCreated, sclCreatedBy) ";
                                        sql.CommandText += "values(@salesman, @plant, GETDATE(), @user) ";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@salesman", salesmanID2);
                                        sql.Parameters.AddWithValue("@plant", locationID);
                                        sql.Parameters.AddWithValue("@user", user);
                                        master.ExecuteNonQuery(sql, "Update Customer Responsibility");
                                    }

                                    toReturn += "Inserted " + customerName + " " + master.readCellString(sh.GetRow(i).GetCell(2)) + "\n";
                                }
                            }
                            catch (Exception e)
                            {
                                toReturn += "We had an issue uploading " + customerName + " " + master.readCellString(sh.GetRow(i).GetCell(2)) + "\n";
                            }
                        }
                        i++;
                    }
                    catch
                    {

                    }
                }

            }


            connection.Close();
            return toReturn;
        }

        public string competitorAdd(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string toReturn = "";

            if (sh != null)
            {
                int i = 1;

                while (sh.GetRow(i) != null)
                {
                    if (i > 1000)
                    {
                        return "Hit limit of cells";
                    }
                    try
                    {
                        if (master.readCellString(sh.GetRow(i).GetCell(0)) != "")
                        {
                            string name = master.readCellString(sh.GetRow(i).GetCell(0)).ToString();
                            string shortName = master.readCellString(sh.GetRow(i).GetCell(1)).ToString();
                            string address1 = master.readCellString(sh.GetRow(i).GetCell(2)).ToString();
                            string address2 = master.readCellString(sh.GetRow(i).GetCell(3)).ToString();
                            string address3 = master.readCellString(sh.GetRow(i).GetCell(4)).ToString();
                            string city = master.readCellString(sh.GetRow(i).GetCell(5)).ToString();
                            string state = master.readCellString(sh.GetRow(i).GetCell(6)).ToString();
                            string zip = master.readCellString(sh.GetRow(i).GetCell(7));
                            //if (zip == "-1")
                            //{
                            //    zip = master.readCellString(sh.GetRow(i).GetCell(7));
                            //}
                            string countryCode = master.readCellString(sh.GetRow(i).GetCell(8)).ToString();
                            string country = master.readCellString(sh.GetRow(i).GetCell(9)).ToString();
                            string lcc = master.readCellString(sh.GetRow(i).GetCell(10)).ToString();
                            string commodity = master.readCellString(sh.GetRow(i).GetCell(11)).ToString();
                            string industry = master.readCellString(sh.GetRow(i).GetCell(12)).ToString();
                            string email = master.readCellString(sh.GetRow(i).GetCell(13)).ToString();
                            string contact = master.readCellString(sh.GetRow(i).GetCell(14)).ToString();
                            string title = master.readCellString(sh.GetRow(i).GetCell(15)).ToString();
                            string phone = master.readCellInt(sh.GetRow(i).GetCell(16)).ToString();
                            if (phone == "-1")
                            {
                                phone = master.readCellString(sh.GetRow(i).GetCell(16)).ToString();
                            }
                            string website = master.readCellString(sh.GetRow(i).GetCell(17)).ToString();
                            int annualSales = 0;
                            if (master.readCellInt(sh.GetRow(i).GetCell(18)) != -1)
                            {
                                annualSales = master.readCellInt(sh.GetRow(i).GetCell(18));
                            }


                            sql.CommandText = "insert into pktblCompetitor (comCompetitorName, comShortName, comAddress1, comAddress2, comAddress3, comAnnualSales, ";
                            sql.CommandText += "comCity, comState, comCountryCode, comCountry, comZip, comIndustry, comWebsite, comLCCSupplier, comEmail, comContact, ";
                            sql.CommandText += "comTitle, comPhone, comCommodity, comCreated, comCreatedBy) ";
                            sql.CommandText += "values (@name, @shortName, @ad1, @ad2, @ad3, @annualSales, @city, @state, @code, @country, @zip, @industry, ";
                            sql.CommandText += "@website, @lcc, @email, @contact, @title, @phone, @commodity, GETDATE(), @user) ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@name", name);
                            sql.Parameters.AddWithValue("@shortName", shortName);
                            sql.Parameters.AddWithValue("@ad1", address1);
                            sql.Parameters.AddWithValue("@ad2", address2);
                            sql.Parameters.AddWithValue("@ad3", address3);
                            sql.Parameters.AddWithValue("@annualSales", annualSales);
                            sql.Parameters.AddWithValue("@city", city);
                            sql.Parameters.AddWithValue("@state", state);
                            sql.Parameters.AddWithValue("@code", countryCode);
                            sql.Parameters.AddWithValue("@country", country);
                            sql.Parameters.AddWithValue("@zip", zip);
                            sql.Parameters.AddWithValue("@industry", industry);
                            sql.Parameters.AddWithValue("@website", website);
                            if (lcc.Trim().ToLower() == "y")
                            {
                                sql.Parameters.AddWithValue("@lcc", 1);
                            }
                            else 
                            {
                                sql.Parameters.AddWithValue("@lcc", 0);
                            }
                            sql.Parameters.AddWithValue("@email", email);
                            sql.Parameters.AddWithValue("@contact", contact);
                            sql.Parameters.AddWithValue("@title", title);
                            sql.Parameters.AddWithValue("@phone", phone);
                            sql.Parameters.AddWithValue("@commodity", commodity);
                            sql.Parameters.AddWithValue("@user", master.getUserName());
                            try
                            {
                                master.ExecuteNonQuery(sql, "Upload competitor");
                                toReturn += "Inserted " + name + "\n";
                            }
                            catch
                            {
                                toReturn += "There was an issue uploading " + name + "\n";
                            }
                        }
                        else
                        {
                            return toReturn;
                        }
                    }
                    catch
                    {

                    }
                    i++;
                }
            }

            connection.Close();
            return toReturn;
        }

        public string competitorUpdate(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string toReturn = "";

            if (sh != null)
            {
                int i = 1;
                while(sh.GetRow(i) != null)
                {
                    if (i > 1000)
                    {
                        return "Hit limit of cells";
                    }
                    try
                    {
                        if(master.readCellString(sh.GetRow(i).GetCell(1)) == "")
                        {
                            return toReturn;
                        }
                        if(master.readCellString(sh.GetRow(i).GetCell(0)) == "Update")
                        {
                            string id = master.readCellInt(sh.GetRow(i).GetCell(30)).ToString();
                            string name = master.readCellString(sh.GetRow(i).GetCell(1)).ToString();
                            string shortName = master.readCellString(sh.GetRow(i).GetCell(2)).ToString();
                            string address1 = master.readCellString(sh.GetRow(i).GetCell(3)).ToString();
                            string address2 = master.readCellString(sh.GetRow(i).GetCell(4)).ToString();
                            string address3 = master.readCellString(sh.GetRow(i).GetCell(5)).ToString();
                            string city = master.readCellString(sh.GetRow(i).GetCell(6)).ToString();
                            string state = master.readCellString(sh.GetRow(i).GetCell(7)).ToString();
                            string zip = master.readCellString(sh.GetRow(i).GetCell(8));
                            //if(zip == "-1")
                            //{
                            //    zip = master.readCellString(sh.GetRow(i).GetCell(8)).ToString();
                            //}
                            string countryCode = master.readCellString(sh.GetRow(i).GetCell(9)).ToString();
                            string country = master.readCellString(sh.GetRow(i).GetCell(10)).ToString();
                            string lcc = master.readCellString(sh.GetRow(i).GetCell(11)).ToString();
                            string commodity = master.readCellString(sh.GetRow(i).GetCell(12)).ToString();
                            string industry = master.readCellString(sh.GetRow(i).GetCell(13)).ToString();
                            string email = master.readCellString(sh.GetRow(i).GetCell(14)).ToString();
                            string contact = master.readCellString(sh.GetRow(i).GetCell(15)).ToString();
                            string title = master.readCellString(sh.GetRow(i).GetCell(16)).ToString();
                            string phone = master.readCellInt(sh.GetRow(i).GetCell(17)).ToString();
                            if(phone == "-1")
                            {
                                phone = master.readCellString(sh.GetRow(i).GetCell(17)).ToString();
                            }
                            string website = master.readCellString(sh.GetRow(i).GetCell(18)).ToString();
                            int annualSales = 0;
                            if (master.readCellInt(sh.GetRow(i).GetCell(19)) != -1)
                            {
                                annualSales = master.readCellInt(sh.GetRow(i).GetCell(10));
                            }

                            sql.CommandText = "update pktblCompetitor set comCompetitorName = @name, comShortName = @shortName, comAddress1 = @ad1, ";
                            sql.CommandText += "comAddress2 = @ad2, comAddress3 = @ad3, comAnnualSales = @annualSales, comCity = @city, comState = @state, ";
                            sql.CommandText += "comCountryCode = @code, comCountry = @country, comZip = @zip, comIndustry = @industry, comWebsite = @website, ";
                            sql.CommandText += "comLCCSupplier = @lcc, comEmail = @email, comContact = @contact, comTitle = @title, comPhone = @phone, ";
                            sql.CommandText += "comCommodity = @commodity, comModified = GETDATE(), comModifiedBy = @user where comCompetitorID = @id ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@id", id);
                            sql.Parameters.AddWithValue("@name", name);
                            sql.Parameters.AddWithValue("@shortName", shortName);
                            sql.Parameters.AddWithValue("@ad1", address1);
                            sql.Parameters.AddWithValue("@ad2", address2);
                            sql.Parameters.AddWithValue("@ad3", address3);
                            sql.Parameters.AddWithValue("@annualSales", annualSales);
                            sql.Parameters.AddWithValue("@city", city);
                            sql.Parameters.AddWithValue("@state", state);
                            sql.Parameters.AddWithValue("@code", countryCode);
                            sql.Parameters.AddWithValue("@country", country);
                            sql.Parameters.AddWithValue("@zip", zip);
                            sql.Parameters.AddWithValue("@industry", industry);
                            sql.Parameters.AddWithValue("@website", website);
                            if(lcc.Trim().ToLower() == "y")
                            {
                                sql.Parameters.AddWithValue("@lcc", 1);
                            }
                            else
                            {
                                sql.Parameters.AddWithValue("@lcc", 0);
                            }
                            sql.Parameters.AddWithValue("@email", email);
                            sql.Parameters.AddWithValue("@contact", contact);
                            sql.Parameters.AddWithValue("@title", title);
                            sql.Parameters.AddWithValue("@phone",phone);
                            sql.Parameters.AddWithValue("@commodity", commodity);
                            sql.Parameters.AddWithValue("@user", master.getUserName());

                            try
                            {
                                master.ExecuteNonQuery(sql, "Update Competitor");
                            }
                            catch (Exception e)
                            {

                            }

                            toReturn += "Updated " + name + "\n";
                        }
                        else if (master.readCellString(sh.GetRow(i).GetCell(0)) == "Delete")
                        {
                            string id = master.readCellInt(sh.GetRow(i).GetCell(30)).ToString();

                            try
                            {
                                sql.CommandText = "Delete from pktblCompetitor where comCompetitorID = @id";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@id", id);
                                master.ExecuteNonQuery(sql, "Delete Competitor");

                                toReturn += "Deleted " + master.readCellString(sh.GetRow(i).GetCell(1)).ToString() + "\n";
                            }
                            catch
                            {
                                toReturn += "Could not delete " + master.readCellString(sh.GetRow(i).GetCell(1)).ToString() + "\n";
                            }
                        }
                    }
                    catch
                    {

                    }
                    i++;
                }
            }

            connection.Close();
            return toReturn;
        }

        public string UpdateChristmas(HttpContext context, XSSFSheet sh)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string toReturn = "";

            if (sh != null)
            {
                int i = 1;

                while (sh.GetRow(i) != null)
                {
                    if (i > 5000)
                    {
                        return "Hit limit of cells";
                    }
                    try
                    {
                        if (master.readCellString(sh.GetRow(i).GetCell(0)) != "")
                        {
                            //Boolean exists = false;

                            string CompanyName = master.readCellString(sh.GetRow(i).GetCell(0));
                            string customerName = master.readCellString(sh.GetRow(i).GetCell(1));
                            string address = master.readCellString(sh.GetRow(i).GetCell(2));

                            string address1 = master.readCellString(sh.GetRow(i).GetCell(2));
                            string city = master.readCellString(sh.GetRow(i).GetCell(4));
                            string state = master.readCellString(sh.GetRow(i).GetCell(5));
                            string zip = master.readCellInt(sh.GetRow(i).GetCell(6)).ToString();
                            string country = master.readCellString(sh.GetRow(i).GetCell(7));

                            string customerDb = "";
                            string companyNumber = "";
                            string companyID = "";
                            int locationID = 0;

                            sql.CommandText = "select CustomerNumber, CustomerID from customer where CustomerName = @Company ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@Company", CompanyName);
                            SqlDataReader company = sql.ExecuteReader();
                            if (company.Read())
                            {
                                companyNumber = company.GetValue(0).ToString();
                                companyID = company.GetValue(1).ToString();

                            }
                            company.Close();

                            sql.CommandText = "select CustomerID, ccoPlant, name from customercontact where name = @customerName ";
                            sql.Parameters.Clear();
                            sql.Parameters.AddWithValue("@customerName", customerName);
                            SqlDataReader dr = sql.ExecuteReader();
                            if (dr.Read()){
                                //customerID = System.Convert.ToInt32(dr.GetValue(0).ToString());
                                //locationID = System.Convert.ToInt32(dr.GetValue(1).ToString());
                                customerDb = (dr.GetValue(2).ToString());
                            }
                            dr.Close();

                            if (customerDb == "")
                            {
                                string Address = master.readCellString(sh.GetRow(i).GetCell(2));

                                sql.CommandText = "select CustomerLocationID from customerlocation where address1 = @address ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@address", Address);
                                SqlDataReader dr3 = sql.ExecuteReader();
                                if (dr3.Read())
                                {
                                    locationID = System.Convert.ToInt32(dr3.GetValue(0).ToString());
                                }
                                    dr3.Close();

                                if (locationID == 0 && companyNumber != "")
                                {
                                    
                                    sql.CommandText = "insert into customerlocation (address1, City, State, Zip, Country, CustomerNumber) ";
                                    sql.CommandText += "values(@address1, @city, @state, @zip, @country, @CustomerNumber) ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@address1", address1);
                                    sql.Parameters.AddWithValue("@city", city);
                                    sql.Parameters.AddWithValue("@state", state);
                                    sql.Parameters.AddWithValue("@zip", zip);
                                    sql.Parameters.AddWithValue("@country", country);
                                    sql.Parameters.AddWithValue("@CustomerNumber", companyNumber);
                                    sql.ExecuteNonQuery();
                                }
                                else if (companyID != "" && customerName != "" && customerDb == "")
                                {
                                    //string salesmanId = "";

                                    //sql.CommandText = "select TSGSalesmanID from tsgsalesman where name = '@salesman' ";
                                    //sql.Parameters.Clear();
                                    //sql.Parameters.AddWithValue("@salesman", salesman);
                                    //SqlDataReader dr4 = sql.ExecuteReader();
                                    //while (dr4.Read())
                                    //{
                                    //    salesmanId = (dr4.GetValue(0).ToString());
                                    //}
                                    //    dr4.Close();

                                    sql.CommandText = "select CustomerLocationID from customerlocation where address1 = @address ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@address", Address);
                                    SqlDataReader dr5 = sql.ExecuteReader();
                                    if (dr5.Read())
                                    {
                                        locationID = System.Convert.ToInt32(dr5.GetValue(0).ToString());
                                    }
                                        dr5.Close();

                                    if (locationID != 0)
                                    {

                                        sql.CommandText = "insert into customercontact (CustomerID, Name, ccoPlant) ";
                                        sql.CommandText += "VALUES (@customerID, @customerName, @locationID) ";
                                        sql.Parameters.Clear();
                                        sql.Parameters.AddWithValue("@customerID", companyID);
                                        sql.Parameters.AddWithValue("@customerName", customerName);
                                        sql.Parameters.AddWithValue("@locationID", locationID);
                                        sql.ExecuteNonQuery();
                                    }

                                }
                            }
                            i++;
                        }

                    }
                    catch (Exception e) { toReturn = "test";

                    }
                }
            }

            connection.Close();
                    return toReturn;
                }
            
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}