using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using Microsoft.SharePoint.Client;
using System.Security;
using System.IO;
using CsvHelper;
using System.Globalization;


namespace RFQ
{
    /// <summary>
    /// Creates an Excel File with all parts for this RFQ that have been reserved by the company
    /// </summary>
    public class STSQuoteSheet : IHttpHandler
    {
        Int32 maxRow = -1;
        public void ProcessRequest(HttpContext context)
        {
            Int64 RFQID = 0;
            Int64 Company = 1;
            try
            {
                RFQID = System.Convert.ToInt64(context.Request["rfq"]);
            }
            catch
            {
                return;
            }


            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            SqlCommand sql2 = new SqlCommand();
            connection2.Open();
            sql2.Connection = connection2;

            Company = master.getCompanyId();

            // Get the STS Part info if it exists. Record is created when one or more STS RFQ Info questions have been answered
            float tmpannualVolume = 0;
            float tmpproductionDaysPerYear = 0;
            float tmpproductionDaysPerWeek = 0;
            float tmpproductionHoursPerShift = 0;
            float tmpshiftPerDay = 0;
            float tmpoverallEquipmentEfficiencyPct = 0;


            sql.CommandText = "select spiAnnualVolume as annualVolume, spiProductionDaysPerYear as productionDaysPerYear, ";
            sql.CommandText += "spiHoursPerShift as productionHoursPerShift, spiShiftsPerDay as shiftPerDay, spiOEE as overallEquipmentEfficiencyPct ";
            sql.CommandText += "from tblSTSPartInfo ";
            sql.CommandText += "where spiRFQID = @rfq  ";

            sql.Parameters.Clear();

            sql.Parameters.AddWithValue("@rfq", RFQID);

            SqlDataReader dr = sql.ExecuteReader();

            if (dr.Read())
            {
                if ((dr["annualVolume"].ToString() != "") && (dr["annualVolume"] != null))
                {
                    tmpannualVolume = Convert.ToSingle(dr["annualVolume"]);
                }
                else
                {
                    tmpannualVolume = 0;
                }
                if ((dr["productionDaysPerYear"].ToString() != "") && (dr["productionDaysPerYear"] != null))
                {
                    tmpproductionDaysPerYear = Convert.ToSingle(dr["productionDaysPerYear"]);
                }
                else
                {
                    tmpproductionDaysPerYear = 0;
                }

                if ((dr["productionDaysPerYear"].ToString() != "") && (dr["productionDaysPerYear"] != null))
                {
                    tmpproductionDaysPerWeek = Convert.ToSingle(dr["productionDaysPerYear"]) / 52;
                }
                else
                {
                    tmpproductionDaysPerWeek = 0;
                }

                if ((dr["productionHoursPerShift"].ToString() != "") && (dr["productionHoursPerShift"] != null))
                {
                    tmpproductionHoursPerShift = Convert.ToSingle(dr["productionHoursPerShift"]);
                }
                else
                {
                    tmpproductionHoursPerShift = 0;
                }


                if ((dr["shiftPerDay"].ToString() != "") && (dr["shiftPerDay"] != null))
                {
                    tmpshiftPerDay = Convert.ToSingle(dr["shiftPerDay"]);
                }
                else
                {
                    tmpshiftPerDay = 0;
                }

                if ((dr["overallEquipmentEfficiencyPct"].ToString() != "") && (dr["overallEquipmentEfficiencyPct"] != null))
                {
                    tmpoverallEquipmentEfficiencyPct = Convert.ToSingle(dr["overallEquipmentEfficiencyPct"]);
                }
                else
                {
                    tmpoverallEquipmentEfficiencyPct = 0;
                }
            }
            else
            {
                tmpannualVolume = 0;
                tmpproductionDaysPerYear = 0;
                tmpproductionDaysPerWeek = 0;
                tmpproductionHoursPerShift = 0;
                tmpshiftPerDay = 0;
                tmpoverallEquipmentEfficiencyPct = 0;

            }
            dr.Close();


            sql.CommandText = "select rfqID as quoteNumber, prtpartDescription as quoteDescription, rfqDueDate as dueDate, prcPartReservedToCompanyID, prtPartNumber as partNumber, prtpartDescription as partName, ";
            sql.CommandText += "t.Name AS salesPersonName, rfqCustomerContact, cl.Country as customerCountry, ";
            sql.CommandText += "cl.ShipToName as customerNameShipTo, cl.Address1 as customerAddressShipTo, cl.State as customerStateShipTo, cl.City as customerCityShipTo, cl.Zip as customerZipCodeShipTo, TSGCompanyAbbrev, c.CustomerName as CustomerName, TSGCompanyId ";
            sql.CommandText += "from tblRFQ ";
            sql.CommandText += "inner join Customer c on c.CustomerId = rfqCustomerId ";
            sql.CommandText += "inner join CustomerLocation cl on cl.CustomerLocationID = rfqPlantID ";
            sql.CommandText += "inner join linkPartReservedToCompany on prcRfqId = rfqID ";
            sql.CommandText += "inner join tblPart p on p.prtPARTID = prcPartID ";
            sql.CommandText += "inner join TSGCompany on TSGCompanyID = prcTSGCompanyID ";
            sql.CommandText += "INNER JOIN TSGSalesman t ON tblRFQ.rfqSalesman = t.TSGSalesmanID ";
            sql.CommandText += "where TSGCompanyID = 13 and not exists (Select * from linkPartToQuote where prcPartID = linkPartToQuote.ptqPartID) ";
            sql.CommandText += "AND rfqID = @rfq  ";

            sql.Parameters.Clear();

            sql.Parameters.AddWithValue("@rfq", RFQID);

            dr = sql.ExecuteReader();

            // Create the csv file header
            StringBuilder csvFileString = new StringBuilder();
            csvFileString.AppendLine("quoteNumber, quoteDescription, dueDate, partNumber, partName, calculatedCycleTime, annualVolume, productionDaysPerYear, productionDaysPerWeek, productionHoursPerShift, shiftPerDay, overallEquipmentEfficiencyPct, customerRequestedCycleTime, totalNoOfSpotWelds, totalNoOfFasteners, totalLengthOfMIGWeld, totalLengthOfMastic, totalLengthOfAdhesive, daysToInstall, salesPersonName, customerContact, markupPct, customerCountry, customerName, customerAddress, customerState, customerCity,customerZipCode,incoTerms,customerCountryShipTo,customerNameShipTo,customerAddressShipTo,customerStateShipTo,customerCityShipTo,customerZipCodeShipTo,incoTermsShipTo,remarks,totalNoOfOperators");
                

            List<ReservedPart> RPdata = new List<ReservedPart> { };

            string tmpquoteNumber = "";
            string tmpquoteDescription = "";
            DateTime tmpdueDate = DateTime.Now;
            string tmppartNumber = "";
            string tmppartName = "";
            float tmpcalculatedCycleTime = 0;
            float tmpcustomerRequestedCycleTime = 0;
            float tmptotalNoOfSpotWelds = 0;
            float tmptotalNoOfFasteners = 0;
            float tmptotalLengthOfMIGWeld = 0;
            float tmptotalLengthOfMastic = 0;
            float tmptotalLengthOfAdhesive = 0;
            float tmpdaysToInstall = 0;
            float tmpmarkupPct = 0;
            string tmpsalesPersonName = "";
            string tmpcustomerContact = "";
            string tmpcustomerContactID = "";
            string tmpcustomerCountry = "";
            string tmpcustomerName = "";
            string tmpcustomerAddress = "";
            string tmpcustomerState = "";
            string tmpcustomerCity = "";
            string tmpcustomerZipCode = "";
            string tmpcustomerCountryShipTo = "";
            string tmpcustomerNameShipTo = "";
            string tmpcustomerAddressShipTo = "";
            string tmpcustomerStateShipTo = "";
            string tmpcustomerCityShipTo = "";
            string tmpcustomerZipCodeShipTo = "";
            string tmpincoTerms = "";
            string tmpincoTermsShipTo = "";
            string tmpremarks = "";
            float tmptotalNoOfOperators = 0;

            if (dr.Read())
            {
                tmpquoteNumber = "'" + dr["quoteNumber"].ToString() + "'";
                tmpquoteDescription = dr["quoteDescription"].ToString();
                tmpdueDate = Convert.ToDateTime(dr["dueDate"]);
                tmppartNumber = dr["PartNumber"].ToString();
                tmppartName = dr["partName"].ToString();
                tmpcalculatedCycleTime = 0;

                tmpcustomerRequestedCycleTime = 0;
                tmptotalNoOfSpotWelds = 0;
                tmptotalNoOfFasteners = 0;
                tmptotalLengthOfMIGWeld = 0;
                tmptotalLengthOfMastic = 0;
                tmptotalLengthOfAdhesive = 0;
                tmpdaysToInstall = 0;
                tmpmarkupPct = 0;
                tmpsalesPersonName = dr["salesPersonName"].ToString();
                tmpcustomerContactID = dr["rfqCustomerContact"].ToString();
                tmpcustomerCountry = dr["customerCountry"].ToString();
                tmpcustomerName = dr["customerNameShipTo"].ToString();
                tmpcustomerAddress = dr["customerAddressShipTo"].ToString();
                tmpcustomerState = dr["customerStateShipTo"].ToString();
                tmpcustomerCity = dr["customerCityShipTo"].ToString();
                tmpcustomerZipCode = "'" + dr["customerZipCodeShipTo"].ToString() + "'";
                tmpcustomerCountryShipTo = dr["customerCountry"].ToString();
                tmpcustomerNameShipTo = dr["customerNameShipTo"].ToString();
                tmpcustomerAddressShipTo = dr["customerAddressShipTo"].ToString();
                tmpcustomerStateShipTo = dr["customerStateShipTo"].ToString();
                tmpcustomerCityShipTo = dr["customerCityShipTo"].ToString();
                tmpcustomerZipCodeShipTo = "'" + dr["customerZipCodeShipTo"].ToString() + "'";
                tmpincoTerms = ' '.ToString();
                tmpincoTermsShipTo = ' '.ToString();
                tmpremarks = ' '.ToString();
                tmptotalNoOfOperators = 0;
                dr.Close();

                sql.CommandText = "select cc.Name as ccName ";
                sql.CommandText += "from CustomerContact cc ";
                sql.CommandText += "where CustomerContactID = @cc  ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@cc", tmpcustomerContactID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                  {
                   tmpcustomerContact = dr["ccName"].ToString();
                  }
                  else
                  {
                    tmpcustomerContact = "";
                  }
                
                    RPdata.Add(new ReservedPart(tmpquoteNumber, tmpquoteDescription, tmpdueDate, tmppartNumber, tmppartName, tmpcalculatedCycleTime, tmpannualVolume, tmpproductionDaysPerYear, tmpproductionDaysPerWeek, 
                                                tmpproductionHoursPerShift, tmpshiftPerDay, tmpoverallEquipmentEfficiencyPct, tmpcustomerRequestedCycleTime, tmptotalNoOfSpotWelds, tmptotalNoOfFasteners, tmptotalLengthOfMIGWeld,
                                                tmptotalLengthOfMastic, tmptotalLengthOfAdhesive, tmpdaysToInstall, tmpsalesPersonName, tmpcustomerContact, tmpmarkupPct, tmpcustomerCountry, tmpcustomerName, tmpcustomerAddress, 
                                                tmpcustomerState, tmpcustomerCity, tmpcustomerZipCode, tmpincoTerms, tmpcustomerCountryShipTo, tmpcustomerNameShipTo, tmpcustomerAddressShipTo,tmpcustomerStateShipTo, tmpcustomerCityShipTo, 
                                                tmpcustomerZipCodeShipTo, tmpincoTermsShipTo, tmpremarks, tmptotalNoOfOperators));


                using (var mem = new MemoryStream())
                {
                    var writer = new StreamWriter(mem);

                    var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
                        
                            // csvWriter.Configuration.Delimiter = ",";
                            //csvWriter.Configuration.HasHeaderRecord = true;
                            csvWriter.Context.AutoMap<ReservedPart>();

                            csvWriter.WriteRecords(RPdata);

                            writer.Flush();


                                           var result = Encoding.UTF8.GetString(mem.ToArray());
                                            Console.WriteLine(result);
                        
                    
               

                context.Response.ContentType = "text/csv";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "QuoteSheet-RFQ" + RFQID + ".csv"));
                context.Response.Clear();
                context.Response.BinaryWrite(mem.ToArray());
                context.Response.End();
                };
            }
            else
            
                {
                    context.Response.Write("File Not Created. The most likely cause is that your company has not reserved any of the parts.");
                }
            dr.Close();

            connection.Close();
        }

        public class ReservedPart
        {
            private string quoteNumber;
            private string quoteDescription;
            private DateTime dueDate;
            private string partNumber;
            private string partName;
            private float calculatedCycleTime;
            private float annualVolume;
            private float productionDaysPerYear;
            private float productionDaysPerWeek;
            private float productionHoursPerShift;
            private float shiftPerDay;
            private float overallEquipmentEfficiencyPct;
            private float customerRequestedCycleTime;
            private float totalNoOfSpotWelds;
            private float totalNoOfFasteners;
            private float totalLengthOfMIGWeld;
            private float totalLengthOfMastic;
            private float totalLengthOfAdhesive;
            private float daysToInstall;
            private string salesPersonName;
            private string customerContact;
            private float markupPct;
            private string customerCountry;
            private string customerName;
            private string customerAddress;
            private string customerState;
            private string customerCity;
            private string customerZipCode;
            private string incoTerms;
            private string customerCountryShipTo;
            private string customerNameShipTo;
            private string customerAddressShipTo;
            private string customerStateShipTo;
            private string customerCityShipTo;
            private string customerZipCodeShipTo;
            private string incoTermsShipTo;
            private string remarks;
            private float totalNoOfOperators;

            public ReservedPart(string quoteNumber, string quoteDescription, DateTime dueDate, string partNumber, string partName, float calculatedCycleTime, float annualVolume,
                                float productionDaysPerYear, float productionDaysPerWeek, float productionHoursPerShift, float shiftPerDay, float overallEquipmentEfficiencyPct,
                                float customerRequestedCycleTime, float totalNoOfSpotWelds, float totalNoOfFasteners, float totalLengthOfMIGWeld, float totalLengthOfMastic,
                                float totalLengthOfAdhesive, float daysToInstall, string salesPersonName, string customerContact, float markupPct, string customerCountry, string customerName,
                                string customerAddress, string customerState, string customerCity, string customerZipCode, string incoTerms, string customerCountryShipTo, string customerNameShipTo,
                                string customerAddressShipTo, string customerStateShipTo, string customerCityShipTo, string customerZipCodeShipTo, string incoTermsShipTo,
                                string remarks, float totalNoOfOperators)
                {
                    this.quoteNumber = quoteNumber;
                    this.quoteDescription = quoteDescription;
                    this.dueDate = dueDate;
                    this.partNumber = partNumber;
                    this.partName = partName;
                    this.calculatedCycleTime = calculatedCycleTime;
                    this.annualVolume = annualVolume;
                    this.productionDaysPerYear = productionDaysPerYear;
                    this.productionDaysPerWeek = productionDaysPerWeek;
                    this.productionHoursPerShift = productionHoursPerShift;
                    this.shiftPerDay = shiftPerDay;
                    this.overallEquipmentEfficiencyPct = overallEquipmentEfficiencyPct;
                    this.customerRequestedCycleTime = customerRequestedCycleTime;
                    this.totalNoOfSpotWelds = totalNoOfSpotWelds;
                    this.totalNoOfFasteners = totalNoOfFasteners;
                    this.totalLengthOfMIGWeld = totalLengthOfMIGWeld;
                    this.totalLengthOfMastic = totalLengthOfMastic;
                    this.totalLengthOfAdhesive = totalLengthOfAdhesive;
                    this.daysToInstall = daysToInstall;
                    this.salesPersonName = salesPersonName;
                    this.customerContact = customerContact;
                    this.markupPct = markupPct;
                    this.customerCountry = customerCountry;
                    this.customerName = customerName;
                    this.customerAddress = customerAddress;
                    this.customerState = customerState;
                    this.customerCity = customerCity;
                    this.customerZipCode = customerZipCode;
                    this.incoTerms = incoTerms;
                    this.customerCountryShipTo = customerCountryShipTo;
                    this.customerNameShipTo = customerNameShipTo;
                    this.customerAddressShipTo = customerAddressShipTo;
                    this.customerStateShipTo = customerStateShipTo;
                    this.customerCityShipTo = customerCityShipTo;
                    this.customerZipCodeShipTo = customerZipCodeShipTo;
                    this.incoTermsShipTo = incoTermsShipTo;
                    this.remarks = remarks;
                    this.totalNoOfOperators = totalNoOfOperators;
                }

            public string QuoteNumber
            {
                get { return quoteNumber; }
                set { quoteNumber = value; }
            }
            public string QuoteDescription 
            {
                get { return quoteDescription; }
                set { quoteDescription = value; }
            }
            public DateTime DueDate
            {
                get { return dueDate; }
                set { dueDate = value; }
            }
            public string PartNumber
            {
                get { return partNumber; }
                set { partNumber = value; }
            }
            public string PartName
            {
                get { return partName; }
                set { partName = value; }
            }
            public float CalculatedCycleTime
            {
                get { return calculatedCycleTime; }
                set { calculatedCycleTime = value; }
            }

            public float AnnualVolume
            {
                get { return annualVolume; }
                set { annualVolume = value; }
            }
            public float ProductionDaysPerYear
            {
                get { return productionDaysPerYear; }
                set { productionDaysPerYear = value; }
            }
            public float ProductionDaysPerWeek
            {
                get { return productionDaysPerWeek; }
                set { productionDaysPerWeek = value; }
            }
            public float ProductionHoursPerShift
            {
                get { return productionHoursPerShift; }
                set { productionHoursPerShift = value; }
            }
            public float ShiftPerDay
            {
                get { return shiftPerDay; }
                set { shiftPerDay = value; }
            }
            public float OverallEquipmentEfficiencyPct
            {
                get { return overallEquipmentEfficiencyPct; }
                set { overallEquipmentEfficiencyPct = value; }
            }
            public float CustomerRequestedCycleTime
            {
                get { return customerRequestedCycleTime; }
                set { customerRequestedCycleTime = value; }
            }
            public float TotalNoOfSpotWelds
            {
                get { return totalNoOfSpotWelds; }
                set { totalNoOfSpotWelds = value; }
            }
            public float TotalNoOfFasteners
            {
                get { return totalNoOfFasteners; }
                set { totalNoOfFasteners = value; }
            }
            public float TotalLengthOfMIGWeld
            {
                get { return totalLengthOfMIGWeld; }
                set { totalLengthOfMIGWeld = value; }
            }
            public float TotalLengthOfMastic
            {
                get { return totalLengthOfMastic; }
                set { totalLengthOfMastic = value; }
            }
            public float TotalLengthOfAdhesive
            {
                get { return totalLengthOfAdhesive; }
                set { totalLengthOfAdhesive = value; }
            }
            public float DaysToInstall
            {
                get { return daysToInstall; }
                set { daysToInstall = value; }
            }
            public string SalesPersonName
            {
                get { return salesPersonName; }
                set { salesPersonName = value; }
            }
            public string CustomerContact
            {
                get { return customerContact; }
                set { customerContact = value; }
            }
            public float MarkupPct
            {
                get { return markupPct; }
                set { markupPct = value; }
            }
            public string CustomerCountry
            {
                get { return customerCountry; }
                set { customerCountry = value; }
            }
            public string CustomerName
            {
                get { return customerName; }
                set { customerName = value; }
            }
            public string CustomerAddress
            {
                get { return customerAddress; }
                set { customerAddress = value; }
            }
            public string CustomerState
            {
                get { return customerState; }
                set { customerState = value; }
            }
            public string CustomerCity
            {
                get { return customerCity; }
                set { customerCity = value; }
            }
            public string CustomerZipCode
            {
                get { return customerZipCode; }
                set { customerZipCode = value; }
            }
            public string IncoTerms
            {
                get { return incoTerms; }
                set { incoTerms = value; }
            }
            public string CustomerCountryShipTo
            {
               get { return customerCountryShipTo; }
               set { customerCountryShipTo = value; }
            }
            public string CustomerNameShipTo
            {
               get { return customerNameShipTo; }
               set { customerNameShipTo = value; }
            }
            public string CustomerAddressShipTo
            {
                get { return customerAddressShipTo; }
                set { customerAddressShipTo = value; }
            }
            public string CustomerStateShipTo
            {
                get { return customerStateShipTo; }
                set { customerStateShipTo = value; }
            }
            public string CustomerCityShipTo
            {
                get { return customerCityShipTo; }
                set { customerCityShipTo = value; }
            }
            public string CustomerZipCodeShipTo
            {
                get { return customerZipCodeShipTo; }
                set { customerZipCodeShipTo = value; }
            }
            public string IncoTermsShipTo
            {
                get { return incoTermsShipTo; }
                set { incoTermsShipTo = value; }
            }
            public string Remarks
            {
                get { return remarks; }
                set { remarks = value; }
            }
            public float TotalNoOfOperators
            {
                get { return totalNoOfOperators; }
                set { totalNoOfOperators = value; }
            }
        }
        public class ReservedPart2
        {
            public string rfqID { get; set; }
            public string quoteNumber { get; set; }
            public string quoteDescription { get; set; }
            public DateTime dueDate { get; set; }
            public string partNumber { get; set; }
            public string partName { get; set; }
            public float calculatedCycleTime { get; set; }
            public float annualVolume { get; set; }
            public float productionDaysPerYear { get; set; }
            public float productionDaysPerWeek { get; set; }
            public float productionHoursPerShift { get; set; }
            public float shiftPerDay { get; set; }
            public float overallEquipmentEfficiencyPct { get; set; }
            public float customerRequestedCycleTime { get; set; }
            public float totalNoOfSpotWelds { get; set; }
            public float totalNoOfFasteners { get; set; }
            public float totalLengthOfMIGWeld { get; set; }
            public float totalLengthOfMastic { get; set; }
            public float totalLengthOfAdhesive { get; set; }
            public float daysToInstall { get; set; }
            public float markupPct { get; set; }
            public string salesPersonName { get; set; }
            public string customerContact { get; set; }
            public string customerCountry { get; set; }
            public string customerName { get; set; }
            public string customerAddress { get; set; }
            public string customerState { get; set; }
            public string customerCity { get; set; }
            public string customerZipCode { get; set; }
            public string customerCountryShipTo { get; set; }
            public string customerNameShipTo { get; set; }
            public string customerAddressShipTo { get; set; }
            public string customerStateShipTo { get; set; }
            public string customerCityShipTo { get; set; }
            public string customerZipCodeShipTo { get; set; }
            public string incoTerms { get; set; }
            public string incoTermsShipTo { get; set; }
            public string remarks { get; set; }
            public float totalNoOfOperators { get; set; }
            public string firstReserved { get; set; }
            public string unreservedDate { get; set; }
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