using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;

namespace RFQ
{
    /// <summary>
    /// Summary description for CustomerResponsibilityList
    /// </summary>
    public class CustomerResponsibilityList : IHttpHandler
    {
        Int32 maxRow = -1;
        public void ProcessRequest(HttpContext context)
        {
            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string sheetType = "";
            sheetType = context.Request["customer"].ToString();

            XSSFWorkbook wb = new XSSFWorkbook();
            if (sheetType == "Customer")
            {
                XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
                XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Customer Responsability List");
                XSSFSheet newSH = (XSSFSheet)wb.CreateSheet("Add New Customers");

                XSSFCellStyle ws = (XSSFCellStyle)wb.CreateCellStyle();
                ws.WrapText = true;

                XSSFFont headerFont = (XSSFFont)wb.CreateFont();
                headerFont.FontHeight = 14;
                headerFont.Boldweight = 700;
                headerFont.IsItalic = true;

                customerHeader(newSH, headerFont, false);
                customerHeader(sh, headerFont, true);

                List<string> salesman = new List<string>();
                sql.CommandText = "Select Name from TSGSalesman where tsaActive = 1 order by Name ";
                sql.Parameters.Clear();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    salesman.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                XSSFDataValidationConstraint salesmanConstraint = new XSSFDataValidationConstraint(salesman.ToArray());

                List<string> rank = new List<string>();
                sql.CommandText = "Select Rank from CustomerRank ";
                sql.Parameters.Clear();
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rank.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                XSSFDataValidationConstraint rankConstraint = new XSSFDataValidationConstraint(rank.ToArray());


                List<string> update = new List<string>();
                update.Add(" ");
                update.Add("Update");
                update.Add("Delete");
                XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(update.ToArray());

                List<string> extraSalesman = new List<string>();
                string currentLocationId = "";
                int count = 0;
                sql.CommandText = "Select cl.CustomerLocationID, Name from Customer c ";
                sql.CommandText += "inner join CustomerLocation cl on cl.CustomerID = c.CustomerID ";
                sql.CommandText += "left outer join linkSalesmanToCustomerLocation on sclCustomerLocationId = cl.CustomerLocationId ";
                sql.CommandText += "left outer join TSGSalesman on TSGSalesman.TSGSalesmanID = sclSalesmanId ";
                sql.CommandText += "order by c.CustomerName, cl.ShipCode ";
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (currentLocationId != dr["CustomerLocationID"].ToString())
                    {
                        extraSalesman.Add(dr["Name"].ToString());
                        count++;
                    }
                    else
                    {
                        extraSalesman[count] += ", " + dr["Name"].ToString();
                    }
                    currentLocationId = dr["CustomerLocationID"].ToString();
                }
                dr.Close();

                //Skip the header
                int currentRow = 1;
                sql.CommandText = "Select CustomerName, Customer.CustomerNumber, ShipToName, ShipCode, Name, Address1, Address2, ";
                sql.CommandText += "Address3, City, State, Country, CustomerLocationID, Customer.CustomerID, Zip, Rank ";
                sql.CommandText += "from Customer, CustomerLocation, TSGSalesman, CustomerRank ";
                sql.CommandText += "where Customer.CustomerID = CustomerLocation.CustomerID  and CustomerLocation.CustomerRankID = CustomerRank.CustomerRankID ";
                sql.CommandText += "and CustomerLocation.TSGSalesmanID = TSGSalesman.TSGSalesmanID and (cusInactive = 0 or cusInactive is null) ";
                sql.CommandText += "order by Customer.CustomerName, ShipCode asc";
                sql.Parameters.Clear();

                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    var newRow = sh.CreateRow(currentRow);

                    XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 0, 0);
                    XSSFDataValidation dv = (XSSFDataValidation)dvHelper.CreateValidation(constraint, loc);
                    dv.ShowErrorBox = true;
                    dv.EmptyCellAllowed = true;
                    sh.AddValidationData(dv);

                    newRow.CreateCell(0).SetCellValue("");
                    newRow.CreateCell(1).SetCellValue(dr["CustomerName"].ToString());
                    newRow.GetCell(1).CellStyle = ws;
                    newRow.CreateCell(2).SetCellValue(dr["CustomerNumber"].ToString());
                    newRow.GetCell(2).CellStyle = ws;
                    newRow.CreateCell(3).SetCellValue(dr["ShipToName"].ToString());
                    newRow.GetCell(3).CellStyle = ws;
                    newRow.CreateCell(4).SetCellValue(dr["ShipCode"].ToString());
                    newRow.GetCell(4).CellStyle = ws;

                    XSSFDataValidationHelper dvHelperSalesman = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList salesmanLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 5, 5);
                    XSSFDataValidation dvSalesman = (XSSFDataValidation)dvHelperSalesman.CreateValidation(salesmanConstraint, salesmanLoc);
                    dvSalesman.ShowErrorBox = true;
                    dvSalesman.EmptyCellAllowed = false;
                    sh.AddValidationData(dvSalesman);

                    newRow.CreateCell(5).SetCellValue(dr["Name"].ToString());

                    XSSFDataValidationHelper dvHelperSalesman2 = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList salesman2Loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 6, 6);
                    XSSFDataValidation dvSalesman2 = (XSSFDataValidation)dvHelperSalesman2.CreateValidation(salesmanConstraint, salesman2Loc);
                    dvSalesman2.ShowErrorBox = true;
                    dvSalesman2.EmptyCellAllowed = false;
                    sh.AddValidationData(dvSalesman2);

                    if (extraSalesman[currentRow - 1].Split(',')[0].Trim() != dr["Name"].ToString())
                    {
                        newRow.CreateCell(6).SetCellValue(extraSalesman[currentRow - 1].Split(',')[0].Trim());
                    }


                    newRow.CreateCell(7).SetCellValue(dr["Address1"].ToString());
                    newRow.GetCell(7).CellStyle = ws;
                    newRow.CreateCell(8).SetCellValue(dr["Address2"].ToString());
                    newRow.GetCell(8).CellStyle = ws;
                    newRow.CreateCell(9).SetCellValue(dr["Address3"].ToString());
                    newRow.GetCell(9).CellStyle = ws;
                    newRow.CreateCell(10).SetCellValue(dr["City"].ToString());
                    newRow.GetCell(10).CellStyle = ws;
                    newRow.CreateCell(11).SetCellValue(dr["State"].ToString());
                    newRow.GetCell(11).CellStyle = ws;
                    newRow.CreateCell(12).SetCellValue(dr["Country"].ToString());
                    newRow.GetCell(12).CellStyle = ws;
                    newRow.CreateCell(13).SetCellValue(dr["Zip"].ToString());
                    newRow.GetCell(13).CellStyle = ws;

                    XSSFDataValidationHelper dvHelperRank = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList rankLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 14, 14);
                    XSSFDataValidation dvRank = (XSSFDataValidation)dvHelperRank.CreateValidation(rankConstraint, rankLoc);
                    dvRank.ShowErrorBox = true;
                    dvRank.EmptyCellAllowed = false;
                    sh.AddValidationData(dvRank);

                    newRow.CreateCell(14).SetCellValue(dr["Rank"].ToString());


                    //Move these far over to the right so people dont accidently mess with it
                    newRow.CreateCell(30).SetCellValue(dr["CustomerLocationID"].ToString());
                    newRow.GetCell(30).CellStyle = ws;
                    newRow.CreateCell(31).SetCellValue(dr["CustomerID"].ToString());
                    newRow.GetCell(31).CellStyle = ws;

                    currentRow++;
                }
                dr.Close();
                connection.Close();


                for (int i = 1; i < 100; i++)
                {
                    var newRow = newSH.CreateRow(i);

                    XSSFDataValidationHelper dvHelperSalesman = new XSSFDataValidationHelper(newSH);
                    NPOI.SS.Util.CellRangeAddressList salesmanLoc = new NPOI.SS.Util.CellRangeAddressList(i, i, 4, 4);
                    XSSFDataValidation dvSalesman = (XSSFDataValidation)dvHelperSalesman.CreateValidation(salesmanConstraint, salesmanLoc);
                    dvSalesman.ShowErrorBox = true;
                    dvSalesman.EmptyCellAllowed = false;
                    newSH.AddValidationData(dvSalesman);

                    newRow.CreateCell(4).SetCellValue(salesman[0]);

                    XSSFDataValidationHelper dvHelperSalesman2 = new XSSFDataValidationHelper(newSH);
                    NPOI.SS.Util.CellRangeAddressList salesman2Loc = new NPOI.SS.Util.CellRangeAddressList(i, i, 5, 5);
                    XSSFDataValidation dvSalesman2 = (XSSFDataValidation)dvHelperSalesman2.CreateValidation(salesmanConstraint, salesman2Loc);
                    dvSalesman.ShowErrorBox = true;
                    dvSalesman.EmptyCellAllowed = true;
                    newSH.AddValidationData(dvSalesman2);

                    newRow.CreateCell(5).SetCellValue("");

                    XSSFDataValidationHelper dvHelperRank = new XSSFDataValidationHelper(newSH);
                    NPOI.SS.Util.CellRangeAddressList rankLoc = new NPOI.SS.Util.CellRangeAddressList(i, i, 13, 13);
                    XSSFDataValidation dvRank = (XSSFDataValidation)dvHelperRank.CreateValidation(rankConstraint, rankLoc);
                    dvRank.ShowErrorBox = true;
                    dvRank.EmptyCellAllowed = false;
                    newSH.AddValidationData(dvRank);

                    newRow.CreateCell(13).SetCellValue("Unranked");
                }


                //The numbers dont really mean anything, this is just a good way to space out the columns
                newSH.SetColumnWidth(0, 9000);
                newSH.SetColumnWidth(1, 3000);
                newSH.SetColumnWidth(2, 11000);
                newSH.SetColumnWidth(3, 1500);
                newSH.SetColumnWidth(4, 5000);
                newSH.SetColumnWidth(5, 5000);
                newSH.SetColumnWidth(6, 8000);
                newSH.SetColumnWidth(7, 8000);
                newSH.SetColumnWidth(8, 6000);
                newSH.SetColumnWidth(9, 6000);
                newSH.SetColumnWidth(10, 2000);
                newSH.SetColumnWidth(11, 3500);
                newSH.SetColumnWidth(12, 2000);
                newSH.SetColumnWidth(13, 2000);


                sh.SetColumnWidth(0, 2000);
                sh.SetColumnWidth(1, 9000);
                sh.SetColumnWidth(2, 3000);
                sh.SetColumnWidth(3, 11000);
                sh.SetColumnWidth(4, 1500);
                sh.SetColumnWidth(5, 5000);
                sh.SetColumnWidth(6, 5000);
                sh.SetColumnWidth(7, 8000);
                sh.SetColumnWidth(8, 8000);
                sh.SetColumnWidth(9, 6000);
                sh.SetColumnWidth(10, 6000);
                sh.SetColumnWidth(11, 2000);
                sh.SetColumnWidth(12, 3500);
                sh.SetColumnWidth(13, 2000);
                sh.SetColumnWidth(14, 2000);

                sh.ForceFormulaRecalculation = true;
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Customer Responsibility List.xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();
            }
            else if (sheetType == "Competitor")
            {
                XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
                XSSFSheet sh = (XSSFSheet)wb.CreateSheet("Competitor Upload");
                XSSFSheet newSH = (XSSFSheet)wb.CreateSheet("Add New Competitor");

                XSSFCellStyle ws = (XSSFCellStyle)wb.CreateCellStyle();
                ws.WrapText = true;

                XSSFFont headerFont = (XSSFFont)wb.CreateFont();
                headerFont.FontHeight = 14;
                headerFont.Boldweight = 700;
                headerFont.IsItalic = true;

                competitorHeader(newSH, headerFont, false);
                competitorHeader(sh, headerFont, true);


                List<string> update = new List<string>();
                update.Add(" ");
                update.Add("Update");
                update.Add("Delete");
                XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(update.ToArray());

                List<string> lcc = new List<string>();
                lcc.Add("n");
                lcc.Add("y");
                XSSFDataValidationConstraint lccConstraint = new XSSFDataValidationConstraint(lcc.ToArray());

                sql.CommandText = "Select comCompetitorName, comShortName, comAddress1, comAddress2, comAddress3, comAnnualSales, ";
                sql.CommandText += "comCity, comState, comCountryCode, comCountry, comZip, comIndustry, comWebsite, comLCCSupplier, ";
                sql.CommandText += "comEmail, comContact, comTitle, comPhone, comCommodity, comCompetitorID ";
                sql.CommandText += "from pktblCompetitor ";
                sql.Parameters.Clear();
                //Header is row 0
                int currentRow = 1;

                SqlDataReader dr = sql.ExecuteReader();
                while(dr.Read())
                {
                    var newRow = sh.CreateRow(currentRow);

                    XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 0, 0);
                    XSSFDataValidation dv = (XSSFDataValidation)dvHelper.CreateValidation(constraint, loc);
                    dv.ShowErrorBox = true;
                    dv.EmptyCellAllowed = true;
                    sh.AddValidationData(dv);

                    newRow.CreateCell(0).SetCellValue("");
                    newRow.GetCell(0).CellStyle = ws;

                    newRow.CreateCell(1).SetCellValue(dr["comCompetitorName"].ToString());
                    newRow.GetCell(1).CellStyle = ws;
                    newRow.CreateCell(2).SetCellValue(dr["comShortName"].ToString());
                    newRow.GetCell(2).CellStyle = ws;
                    newRow.CreateCell(3).SetCellValue(dr["comAddress1"].ToString());
                    newRow.GetCell(3).CellStyle = ws;
                    newRow.CreateCell(4).SetCellValue(dr["comAddress2"].ToString());
                    newRow.GetCell(4).CellStyle = ws;
                    newRow.CreateCell(5).SetCellValue(dr["comAddress3"].ToString());
                    newRow.GetCell(5).CellStyle = ws;
                    newRow.CreateCell(6).SetCellValue(dr["comCity"].ToString());
                    newRow.GetCell(6).CellStyle = ws;
                    newRow.CreateCell(7).SetCellValue(dr["comState"].ToString());
                    newRow.GetCell(7).CellStyle = ws;
                    newRow.CreateCell(8).SetCellValue(dr["comZip"].ToString());
                    newRow.GetCell(8).CellStyle = ws;
                    newRow.CreateCell(9).SetCellValue(dr["comCountryCode"].ToString());
                    newRow.GetCell(9).CellStyle = ws;
                    newRow.CreateCell(10).SetCellValue(dr["comCountry"].ToString());
                    newRow.GetCell(10).CellStyle = ws;

                    XSSFDataValidationHelper lccHelper = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList lccLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 11, 11);
                    XSSFDataValidation lccDV = (XSSFDataValidation)lccHelper.CreateValidation(lccConstraint, lccLoc);
                    lccDV.ShowErrorBox = true;
                    lccDV.EmptyCellAllowed = false;
                    sh.AddValidationData(lccDV);
                    if(dr.GetValue(13) != null && dr.GetBoolean(13))
                    {
                        newRow.CreateCell(11).SetCellValue("y");
                    }
                    else
                    {
                        newRow.CreateCell(11).SetCellValue("n");
                    }
                    newRow.GetCell(11).CellStyle = ws;
                    newRow.CreateCell(12).SetCellValue(dr["comCommodity"].ToString());
                    newRow.GetCell(12).CellStyle = ws;
                    newRow.CreateCell(13).SetCellValue(dr["comIndustry"].ToString());
                    newRow.GetCell(13).CellStyle = ws;
                    newRow.CreateCell(14).SetCellValue(dr["comEmail"].ToString());
                    newRow.GetCell(14).CellStyle = ws;
                    newRow.CreateCell(15).SetCellValue(dr["comContact"].ToString());
                    newRow.GetCell(15).CellStyle = ws;
                    newRow.CreateCell(16).SetCellValue(dr["comTitle"].ToString());
                    newRow.GetCell(16).CellStyle = ws;
                    newRow.CreateCell(17).SetCellValue(dr["comPhone"].ToString());
                    newRow.GetCell(17).CellStyle = ws;
                    newRow.CreateCell(18).SetCellValue(dr["comWebsite"].ToString());
                    newRow.GetCell(18).CellStyle = ws;
                    newRow.CreateCell(19).SetCellValue(dr["comAnnualSales"].ToString());
                    newRow.GetCell(19).CellStyle = ws;
                    newRow.CreateCell(30).SetCellValue(dr["comCompetitorID"].ToString());
                    newRow.GetCell(30).CellStyle = ws;
                    currentRow++;
                }
                dr.Close();

                for (int i = 1; i < 1000; i++)
                {
                    var newRow = newSH.CreateRow(i);

                    XSSFDataValidationHelper lccHelper = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList lccLoc = new NPOI.SS.Util.CellRangeAddressList(i, i, 10, 10);
                    XSSFDataValidation lccDV = (XSSFDataValidation)lccHelper.CreateValidation(lccConstraint, lccLoc);
                    lccDV.ShowErrorBox = true;
                    lccDV.EmptyCellAllowed = false;
                    newSH.AddValidationData(lccDV);
                    newRow.CreateCell(10).SetCellValue("n");
                }

                

                sh.ForceFormulaRecalculation = true;
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Competitor List.xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();
            }
            else if (sheetType == "RFQ")
            {
                int currentColumn = 0;
                int referenceRow = 0;
                NPOI.SS.UserModel.IRow rrow;


                XSSFDataFormat CustomFormat = (XSSFDataFormat)wb.CreateDataFormat();
                XSSFSheet sh = (XSSFSheet)wb.CreateSheet("RFQ Update");
                XSSFSheet referenceSheet = (XSSFSheet)wb.CreateSheet("REFERENCES");

                XSSFCellStyle ws = (XSSFCellStyle)wb.CreateCellStyle();
                ws.WrapText = true;

                XSSFFont headerFont = (XSSFFont)wb.CreateFont();
                headerFont.FontHeight = 14;
                headerFont.Boldweight = 700;
                headerFont.IsItalic = true;

                rfqHeader(sh, headerFont);


                List<string> update = new List<string>();
                update.Add(" ");
                update.Add("Update");
                XSSFDataValidationConstraint constraint = new XSSFDataValidationConstraint(update.ToArray());

                sql.CommandText = "Select distinct ProgramName from Program where ProgramName <> '   ADD NEW' and (ProgramName <> '0' or ProgramID = 5) order by ProgramName asc ";
                sql.Parameters.Clear();
                referenceRow = 0;
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(referenceSheet, referenceRow);
                    rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                    referenceRow++;
                }
                dr.Close();
                XSSFDataValidationConstraint programConstraint = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$A$1:$A$" + referenceRow);

                sql.CommandText = "Select OEMName from OEM order by OEMName asc ";
                sql.Parameters.Clear();
                referenceRow = 0;
                currentColumn++;
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(referenceSheet, referenceRow);
                    rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                    referenceRow++;
                }
                dr.Close();
                XSSFDataValidationConstraint oemConstraint = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$B$1:$B$" + referenceRow);

                sql.CommandText = "Select vehVehicleName from pktblVehicle order by vehVehicleName asc ";
                sql.Parameters.Clear();
                referenceRow = 0;
                currentColumn++;
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    rrow = GetOrCreateRow(referenceSheet, referenceRow);
                    rrow.CreateCell(currentColumn).SetCellValue(dr.GetValue(0).ToString());
                    referenceRow++;
                }
                dr.Close();
                XSSFDataValidationConstraint vehicleConstraint = new XSSFDataValidationConstraint(0x03, "=REFERENCES!$C$1:$C$" + referenceRow);

                sql.CommandText = "Select rfqID, ProgramName, OEMName, vehVehicleName from tblRFQ, Program, OEM, pktblVehicle ";
                sql.CommandText += "where rfqProgramID = ProgramID and rfqOEMID = OEMID and rfqVehicleID = vehVehicleID order by rfqID desc ";
                sql.Parameters.Clear();
                //Header is row 0
                int currentRow = 1;

                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    var newRow = sh.CreateRow(currentRow);

                    XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList loc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 0, 0);
                    XSSFDataValidation dv = (XSSFDataValidation)dvHelper.CreateValidation(constraint, loc);
                    dv.ShowErrorBox = true;
                    dv.EmptyCellAllowed = true;
                    sh.AddValidationData(dv);

                    newRow.CreateCell(0).SetCellValue("");

                    List<string> rfq = new List<string>();
                    rfq.Add(" ");
                    rfq.Add(dr["rfqID"].ToString());
                    XSSFDataValidationConstraint rfqConstraint = new XSSFDataValidationConstraint(rfq.ToArray());


                    XSSFDataValidationHelper dvHelperRFQ = new XSSFDataValidationHelper(sh);
                    NPOI.SS.Util.CellRangeAddressList rfqLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 1, 1);
                    XSSFDataValidation dvRFQ = (XSSFDataValidation)dvHelper.CreateValidation(rfqConstraint, rfqLoc);
                    dvRFQ.EmptyCellAllowed = false;
                    dvRFQ.SuppressDropDownArrow = true;
                    dvRFQ.ShowErrorBox = true;
                    sh.AddValidationData(dvRFQ);
                    newRow.CreateCell(1).SetCellValue(dr["rfqID"].ToString());


                    NPOI.SS.Util.CellRangeAddressList programLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 2, 2);
                    XSSFDataValidation dvProgram = (XSSFDataValidation)dvHelper.CreateValidation(programConstraint, programLoc);
                    dvProgram.ShowErrorBox = true;
                    dvProgram.EmptyCellAllowed = false;
                    //  Uncomment this to change it back to a drop down
                    //  sh.AddValidationData(dvProgram);
                    newRow.CreateCell(2).SetCellValue(dr["ProgramName"].ToString());


                    NPOI.SS.Util.CellRangeAddressList oemLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 3, 3);
                    XSSFDataValidation dvOEM = (XSSFDataValidation)dvHelper.CreateValidation(oemConstraint, oemLoc);
                    dvOEM.ShowErrorBox = true;
                    dvOEM.EmptyCellAllowed = false;
                    sh.AddValidationData(dvOEM);
                    newRow.CreateCell(3).SetCellValue(dr["OEMName"].ToString());


                    NPOI.SS.Util.CellRangeAddressList vehicleLoc = new NPOI.SS.Util.CellRangeAddressList(currentRow, currentRow, 4, 4);
                    XSSFDataValidation dvVehicle = (XSSFDataValidation)dvHelper.CreateValidation(vehicleConstraint, vehicleLoc);
                    dvVehicle.ShowErrorBox = true;
                    dvVehicle.EmptyCellAllowed = false;
                    //  Uncomment this to change it back to a drop down
                    //  sh.AddValidationData(dvVehicle);
                    newRow.CreateCell(4).SetCellValue(dr["vehVehicleName"].ToString());
                    currentRow++;
                }
                dr.Close();



                sh.SetColumnWidth(0, 3000);
                sh.SetColumnWidth(1, 3000);
                sh.SetColumnWidth(2, 8000);
                sh.SetColumnWidth(3, 8000);
                sh.SetColumnWidth(4, 8000);

                wb.SetSheetHidden(1, true);

                sh.ForceFormulaRecalculation = true;
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "RFQ Update.xlsx"));
                context.Response.Clear();
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                wb.Write(ms);
                context.Response.BinaryWrite(ms.ToArray());
                context.Response.End();
            }
            
            


            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
        }

        public void customerHeader (XSSFSheet sh, XSSFFont headerFont, Boolean update)
        {
            var row = sh.CreateRow(0);
            int count = 0;
            if(update)
            {
                row.CreateCell(count).SetCellValue("Update");
                row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
                count++;
            }
            row.CreateCell(count).SetCellValue("Customer Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Customer Number");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Plant Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Ship Code");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Salesman Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Salesman 2");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 1");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 2");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 3");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("City");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("State");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Country");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Zip");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Rank");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            if(update)
            {
                row.CreateCell(30).SetCellValue("Customer Location ID");
                row.GetCell(30).RichStringCellValue.ApplyFont(headerFont);
                row.CreateCell(31).SetCellValue("Customer ID");
                row.GetCell(31).RichStringCellValue.ApplyFont(headerFont);
            }
        }

        public void competitorHeader (XSSFSheet sh, XSSFFont headerFont, Boolean update)
        {
            var row = sh.CreateRow(0);
            int count = 0;
            if(update)
            {
                row.CreateCell(count).SetCellValue("Update");
                row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
                count++;
            }
            row.CreateCell(count).SetCellValue("Competitor Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Competitor Short Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 1");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 2");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Address 3");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("City");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("State");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Zip");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Country Code");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Country");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("LCC Supplier");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Commodity");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Industry");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Email");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Contact Name");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Title");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Phone");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Website");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Annual Sales");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
        }

        public void rfqHeader (XSSFSheet sh, XSSFFont headerFont)
        {
            var row = sh.CreateRow(0);
            int count = 0;
            row.CreateCell(count).SetCellValue("Update");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("RFQ ID");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Program");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("OEM");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
            count++;
            row.CreateCell(count).SetCellValue("Vehicle");
            row.GetCell(count).RichStringCellValue.ApplyFont(headerFont);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public NPOI.SS.UserModel.IRow GetOrCreateRow(XSSFSheet referenceSheet, Int32 currentRow)
        {
            if (currentRow > maxRow)
            {
                maxRow = currentRow;
                return referenceSheet.CreateRow(currentRow);
            }
            else
            {
                return referenceSheet.GetRow(currentRow);
            }
        }
    }
}