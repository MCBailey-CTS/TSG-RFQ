using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace RFQ.Models
{
    // Includes all extended information for rfq and part combinations
    // In other words, it joins all of the normalized tables together so that you 
    // can get both the ID of the attribute, as well as the actual readable value

    public class RFQPartFullInformation
    {
        public Int64 rfqID { get; set; }
        public Boolean fullyNoQuoted { get; set; }
        public Int64 rfqStatusID { get; set; }
        public String rfqStatusCode { get; set; }
        public String rfqStatusDescription { get; set; }
        public Int64 rfqCustomerID { get; set; }
        public String rfqCustomerName { get; set; }
        public String rfqCustomerNumber { get; set; }
        public String rfqPlantID {  get; set;}
        public String rfqShipToName { get; set; }
        public String rfqShipToCode { get; set; }
        public String rfqAddress1 { get; set; }
        public String rfqAddress2 { get; set; }
        public String rfqAddress3 { get; set; }
        public String rfqCity { get; set; }
        public String rfqState { get; set; }
        public String rfqZip { get; set; }
        public String rfqCountry { get; set; }
        public Int64 rfqCustomerRankID { get; set; }
        public String rfqCustomerRank { get; set; }
        public Int64 rfqHouseAccountID { get; set; }
        public String rfqHouseAccountAbbreviation { get; set; }
        public String rfqHouseAccountname { get; set; }
        // these are from the customer location table - they are the default values for that location
        // the actual values assigned to the rfq are later
        public Int64 rqfDefaultProgramManagerID { get; set; }
        public String rfqDefaultProgramManagerName { get; set;}
        public Int64 rfqDefaultSalesmanID {get; set;}
        public String rfqDefaultSalesmanName { get; set; }
        public String rfqDefaultSalesmanEmail { get; set; }
        public String rfqDefaultSalesmanPhone { get; set; }
        public String customerRFQ { get; set; }
        public Int64 rfqOEMID { get; set; }
        public String rfqOEMName { get; set; }
        public Int64 rfqVehicleID { get; set; }
        public String rfqVehicle { get; set; }
        public DateTime rfqDueDate { get; set; }
        public DateTime rfqDateReceived { get; set; }
        public DateTime rfqPODate { get; set; }
        public DateTime rfqBidDate { get; set; }
        public Int64 rfqPaymentTermsID { get; set; }
        public String rfqPaymentTerms { get; set; }
        public Int64 rfqShippingTermsID { get; set; }
        public String rfqShippingTerms { get; set; }
        public Int64 rfqToolCountryID { get; set; }
        public String rfqToolCountry { get; set; }
        public String rfqEngineeringNumber { get; set; }
        public Int64 rfqProductTypeID { get; set; }
        public String rfqProductType { get; set; }
        public String rfqIndustry { get { return rfqProductType; } }
        public Int64 rfqNumberOfParts { get; set;  }
        public Int64 rfqNumberOfQuotes { get; set; }
        public String rfqNotes { get; set; }
        public String rfqMeetingNotes { get; set; }
        public Int64 rfqNumberOfQuotesCompleted { get; set; }
        public DateTime rfqPostDate { get; set; }
        public DateTime rfqCompletedDate { get; set; }
        public Boolean rfqLiveWork { get; set; }
        public Int64 rfqSourceID { get; set; }
        public String rfqSource { get; set; }
        public Int64 rfqAdditionalSourceID { get; set; }
        public String rfqAdditionalSource { get; set; }
        public DateTime rfqPostedDate { get; set; }
        public Int64 rfqSalesmanID { get; set; }
        public String rfqSalesman { get; set; }
        public DateTime rfqInternalDueDate { get; set; }
        public Boolean rfqCheckBit { get; set; }
        public DateTime rfqSLANotificationSent { get; set; }
        public DateTime rfqAllPartsNotificationSent { get; set; }
        public Boolean rfqUseTSGLogo { get; set; }
        public Int64 rfqCustomerContactID { get; set; }
        public String rfqCustomerContact { get; set; }
        public Boolean rfqTurnkey { get; set; }
        public Boolean rfqGlobalProgam { get; set; }
        public List<RFQParts> rfqPartList { get; set; }        

        public Boolean ConvertZeroToBoolean(Int64 value) 
        {

            if (value==0) {
                return false;
            } else {
                return true;
            }
        }
        public List<RFQPartFullInformation> ListRFQParts(String SelectBy="", String SelectValue="") 
        {
            List<RFQPartFullInformation> listRFQ = new List<RFQPartFullInformation>();
            return listRFQ;
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            // if by company, then there is no selection criteria here. Decision to add the rfq to listrfq is made if there is the company in quote, noquote, or reserved
            sql.CommandText = "select tblrfq.*,  TSGSalesman.Email as Email, TSGSalesman.Name as Salesman, Customer.*, CustomerLocation.*, ptyProductType, ptyPartTypeDescription, vehVehicleName, tsgcompanyabbrev, tsgcompanyname, rfqProgramID,  rfqCustomerRFQNumber,  ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID, rfqDateReceived, rfqDueDate, prtPartID, prtPartNumber, prtPartDescription, prtPartTypeID,  prtPicture, prtPartLength, prtPartWidth, prtPartHeight, prtPartMaterialType, prtPartWeight, prtPartThickness, prtPartName, prtRFQLineNumber, prtNote, CustomerContact.Name as CustomerContactName, mp.mtyMaterialType as PartMaterialType from  tsgcompany,tblrfq, oem, Program, CustomerLocation, tblPart, linkPartToRFQ, CustomerContact,  pktblMaterialType mp,  pktblVehicle, pktblProductType, pktblPartType, Customer, TSGSalesman, pktblestimators  where rfqCompanyid=TSGCompany.tsgcompanyid and tblrfq.rfqOEMID=oem.oemid and rfqProgramID=ProgramID and rfqPlantID=CustomerLocationID and rfqid=ptrRFQID and ptrPartID=prtPartID and rfqCustomerContact=customerContactID  and prtPartMaterialType=mp.mtyMaterialTypeID  and rfqVehicleID=vehVehicleID and rfqProductTypeId=ptyProductTypeId and prtPartTypeId=ptyPartTypeId and rfqCustomerID=Customer.CustomerID and rfqSalesman=TSGSalesman.TSGSalesmanID  Order by rfqid,  prtRFQLineNumber ";
            sql.Parameters.Clear();
            if (SelectBy == "OEM")
            {
                sql.CommandText = "select tblrfq.*, TSGSalesman.Email as Email, TSGSalesman.Name as Salesman, Customer.*, CustomerLocation.*, ptyProductType, ptyPartTypeDescription, vehVehicleName, tsgcompanyabbrev, tsgcompanyname, rfqProgramID,  rfqCustomerRFQNumber,  ProgramName, OEMName, rfqMeetingNotes, rfqVehicleID, rfqDateReceived, rfqDueDate, prtPartNumber, prtPartDescription, prtNote, CustomerContact.Name as CustomerContactName   from  tsgcompany,tblrfq, oem, Program, CustomerLocation, tblPart, linkPartToRFQ, CustomerContact,  pktblMaterialType, pktblVehicle, pktblProductType, pktblPartType, Customer, TSGSalesman, pktblestimators where rfqCompanyid=TSGCompany.tsgcompanyid and tblrfq.rfqOEMID=oem.oemid and rfqProgramID=ProgramID and rfqPlantID=CustomerLocationID and rfqid=ptrRFQID and ptrPartID=prtPartID and rfqCustomerContact=customerContactID  and prtPartMaterialType=mtyMaterialTypeID and rfqVehicleID=vehVehicleID and rfqProductTypeId=ptyProductTypeId and prtPartTypeId=ptyPartTypeId and rfqCustomerID=Customer.CustomerID and rfqSalesman=TSGSalesman.TSGSalesmanID and rfqOEMID=@oem Order by rfqid,  prtRFQLineNumber ";
                sql.Parameters.AddWithValue("@oem", SelectValue);
            }
            SqlDataReader ldr = sql.ExecuteReader();
            Int64 HoldRFQ = 0;
            RFQPartFullInformation rfq = new RFQPartFullInformation();
            while (ldr.Read())
            {
                if (System.Convert.ToInt64(ldr["rfqID"]) != HoldRFQ)
                {
                    if (HoldRFQ > 0)
                    {
                        if (rfq.rfqPartList.Count > 0)
                        {
                            listRFQ.Add(rfq);
                        }
                    }
                    rfq = new RFQPartFullInformation();
                    rfq.rfqID = System.Convert.ToInt64(ldr["rfqID"]);
                    rfq.customerRFQ = ldr["rfqCustomerRFQNumber"].ToString();
                    rfq.fullyNoQuoted  = false;
                    rfq.rfqAdditionalSourceID = System.Convert.ToInt64(ldr["rfqAddtionalSourceID"]);
                    // todo  get name for additional source
                    rfq.rfqAddress1 = ldr["Address1"].ToString();
                    rfq.rfqAddress2 = ldr["Address2"].ToString();
                    rfq.rfqAddress3 = ldr["Address3"].ToString();
                    rfq.rfqAllPartsNotificationSent =  System.Convert.ToDateTime((ldr["rfqAllPartsQuotedNotificationSent"]));
                    try 
                    {
                        rfq.rfqBidDate = System.Convert.ToDateTime(ldr["rfqBidDate"]);
                    }
                    catch 
                    {
                    }
                    rfq.rfqCheckBit = ConvertZeroToBoolean(System.Convert.ToInt64(ldr["rfqCheckBit"]));
                    rfq.rfqCity  = ldr["City"].ToString();
                    try 
                    {
                        rfq.rfqCompletedDate = System.Convert.ToDateTime(ldr["rfqCompletedDate"]);
                    }
                    catch 
                    {
                    }
                    try 
                    {
                        rfq.rfqInternalDueDate  = System.Convert.ToDateTime(ldr["rfqInternalDueDate"]);
                    }
                    catch 
                    {

                    }
                    rfq.rfqCountry = ldr["Country"].ToString();
                    rfq.rfqCustomerContact = ldr["CustomerContact"].ToString();
                    rfq.rfqCustomerContactID = System.Convert.ToInt64(ldr["rfqCustomerContact"]);
                    rfq.rfqCustomerID  = System.Convert.ToInt64(ldr["rfqCustomerID"]);
                    rfq.rfqCustomerName = ldr["CustomerName"].ToString();
                    rfq.rfqCustomerNumber = ldr["CustomerNumber"].ToString();
                    rfq.rfqCustomerRankID = System.Convert.ToInt64(ldr["CustomerRankID"]);
                    // todo get rank name
                    rfq.rfqDateReceived = System.Convert.ToDateTime(ldr["rfqDateReceived"]);
                    // todo get manager name
                    rfq.rfqDefaultProgramManagerName = "";
                    rfq.rfqDefaultSalesmanEmail = ldr["Email"].ToString();
                    rfq.rfqDefaultSalesmanID = System.Convert.ToInt64(ldr["TSGSalesmanID"]);
                    rfq.rfqDefaultSalesmanName = ldr["Salesman"].ToString();
                    rfq.rfqDefaultSalesmanPhone = ldr["MobilePhone"].ToString();
                    rfq.rfqDueDate = System.Convert.ToDateTime(ldr["rfqDueDate"]);
                    rfq.rfqEngineeringNumber = "";
                    rfq.rfqGlobalProgam = ConvertZeroToBoolean(System.Convert.ToInt64(ldr["rfqGlobalProgram"]));
                    // todo house account stuff
                    rfq.rfqHouseAccountAbbreviation = "";
                    rfq.rfqHouseAccountID = 0;
                    rfq.rfqHouseAccountname = "";
                    rfq.rfqLiveWork = ConvertZeroToBoolean(System.Convert.ToInt64(ldr["rfqLiveWork"]));
                    rfq.rfqMeetingNotes = ldr["rfqMeetingNotes"].ToString();
                    rfq.rfqNotes = ldr["rfqNotes"].ToString();
                    rfq.rfqNumberOfParts = System.Convert.ToInt64(ldr["rfqNumberOfParts"]);
                    rfq.rfqNumberOfQuotes = System.Convert.ToInt64(ldr["rfqNumberOfQuotes"]);
                    rfq.rfqNumberOfQuotesCompleted = System.Convert.ToInt64(ldr["rfqNumberOfQuotesCompleted"]);
                    rfq.rfqOEMID = System.Convert.ToInt64(ldr["rfqOEMID"]);
                    rfq.rfqOEMName = ldr["OEMName"].ToString();
                    // todo get payment terms
                    rfq.rfqPaymentTerms = "";
                    rfq.rfqPaymentTermsID = System.Convert.ToInt64(ldr["rfqPaymentTermsId"]);
                    rfq.rfqPlantID = ldr["rfqPlantID"].ToString();
                    // todo podate, posted, date, post date
                    rfq.rfqProductType = ldr["ptyProductType"].ToString();
                    rfq.rfqProductTypeID = System.Convert.ToInt64(ldr["rfqProductTypeId"]);
                    rfq.rfqSalesman = ldr["Salesman"].ToString();
                    rfq.rfqSalesmanID = System.Convert.ToInt64(ldr["rfqSalesman"]);
                    // todo shipping terms
                    rfq.rfqShippingTerms ="";
                    rfq.rfqShippingTermsID = System.Convert.ToInt64(ldr["rfqShippingTermsID"]);
                    rfq.rfqShipToCode = ldr["ShipCode"].ToString();
                    rfq.rfqShipToName = ldr["ShipToName"].ToString();
                    try {
                        rfq.rfqSLANotificationSent = System.Convert.ToDateTime(ldr["rfqSLANotificationSent"]);
                    }
                    catch {

                    }
                    rfq.rfqSource = "";
                    rfq.rfqSourceID = System.Convert.ToInt64(ldr["rfqSourceId"]);
                    // todo source name, additional source name
                    rfq.rfqState = ldr["State"].ToString();
                    // todo get status
                    rfq.rfqStatusCode = ldr["rfqStatus"].ToString();
                    rfq.rfqStatusDescription = ldr["rfqStatus"].ToString();
                    rfq.rfqStatusID = System.Convert.ToInt64(ldr["rfqStatus"]);
                    rfq.rfqToolCountryID = System.Convert.ToInt64(ldr["rfqToolCountryID"]);
                    rfq.rfqVehicle = ldr["vehVehicleName"].ToString();
                    rfq.rfqVehicleID = System.Convert.ToInt64(ldr["rfqVehicleID"]);
                    // todo TurnKey, ToolCounty, Use TSG logo
                    rfq.rfqZip = ldr["Zip"].ToString();
                    rfq.rfqPartList = new List<RFQParts>();
                }
                RFQParts newPart = new RFQParts();
                newPart.partNumber = ldr["prtPartNumber"].ToString();
                newPart.partDescription = ldr["prtPartDescription"].ToString();
                newPart.partID = System.Convert.ToInt64(ldr["prtPartID"]);
                newPart.partTypeID = System.Convert.ToInt64(ldr["prtPartTypeID"]);                
                newPart.picture = ldr["prtPicture"].ToString();
                newPart.length = System.Convert.ToDecimal(ldr["prtPartLength"]);
                newPart.width = System.Convert.ToDecimal(ldr["prtPartWidth"]);
                newPart.height = System.Convert.ToDecimal(ldr["prtPartHeight"]);
                newPart.materialTypeID = System.Convert.ToInt64(ldr["prtPartMaterialType"]);
                newPart.materialType = ldr["PartMaterialType"].ToString();
                newPart.weight = System.Convert.ToDecimal(ldr["prtPartWeight"]);
                newPart.thickness = System.Convert.ToDecimal(ldr["prtPartThickness"]);
                newPart.partLevEAU = ldr["prtPartRevLevEAU"].ToString();
                newPart.lineNumber = System.Convert.ToInt64(ldr["prtRFQLineNumber"]);
                newPart.Note = ldr["prtNote"].ToString();
                newPart.partTypeID = System.Convert.ToInt64(ldr["prtPartTypeID"]);
                newPart.partType = ldr["ptyPartTypeDescription"].ToString();

                newPart.Quotes = new List<PartQuotes>();
                newPart.NoQuotes = new List<PartNoQuote>();
                newPart.Reserved = new List<PartReserved>();
                // todo get quotes for this part

                SqlConnection subconnection = new SqlConnection(master.getConnectionString());
                subconnection.Open();
                SqlCommand subsql = new SqlCommand();
                subsql.Connection = subconnection;
                subsql.CommandText = "select tblquote.*, ptePaymentTerms, steShippingTerms, mtyMaterialType, pktblblankinfo.*, concat(estFirstName,' ',estLastName) as Estimator, TSGSalesman.Name as Salesman, tblDieInfo.*,  cavCavityName, tsgcompanyabbrev, tsgcompanyname,  dtyFullName, DieType.Name as DieTypeName   from tblquote, tsgcompany,   linkDieInfoToQuote, tblDieInfo, DieType, pktblCavity, TSGSalesman, pktblestimators, pktblblankinfo, pktblMaterialtype, pktblPaymentTerms, pktblShippingTerms, linkparttoquote where ptqPartID=@part and quotsgcompanyid=TSGCompany.tsgcompanyid and  diqQuoteID=quoQuoteID and diqDieInfoID=dinDieInfoID and dinDieType=DieTypeID and dinCavityID=cavCavityID and quoBlankInfoID=binBlankInfoID and binBlankMaterialTypeID=mtyMaterialTypeID and   quoSalesman=TSGSalesman.TSGSalesmanID and quoEstimatorID=estEstimatorID and quoBlankInfoID=binBlankInfoID and  quoPaymentTermsID=ptePaymentTermsID and quoShippingTermsID=steShippingTermsID order by quoQuoteID  ";
                sql.Parameters.AddWithValue("@part", newPart.partID);
                SqlDataReader sdr = subsql.ExecuteReader();
                while (sdr.Read())
                {
                    PartQuotes quote = new PartQuotes();
                    quote.quoteID = System.Convert.ToInt64(ldr["quoQuoteID"]);
                    quote.companyID = System.Convert.ToInt64(ldr["quoTSGCompanyID"]);
                    quote.companyName = ldr["tsgcompanyname"].ToString();
                    quote.companyAbbreviation = ldr["tsgcompanyabbrev"].ToString();
                    quote.estimatorID = System.Convert.ToInt64(ldr["quoEstimatorID"]);
                    quote.estimatorName = ldr["Estimator"].ToString();
                    quote.version = ldr["quoVersion"].ToString();
                    quote.jobNumber = System.Convert.ToInt64(ldr["quoJobNumberID"]);
                    try
                    {
                        quote.estimatedPODate = System.Convert.ToDateTime(ldr["quoEstimatedPODate"]);
                    }
                    catch
                    {

                    }
                    quote.statusID = System.Convert.ToInt64(sdr["quoStatusID"]);
                    quote.paymentTermsID = System.Convert.ToInt64(sdr["quoPaymentTermsID"]);
                    quote.paymentTerms = sdr["ptePaymentTerms"].ToString();
                    quote.shippingTermsID = System.Convert.ToInt64(sdr["quoShippingTermsID"]);
                    quote.shippingTerms = sdr["steShippingTerms"].ToString();
                    quote.blankInfoID = System.Convert.ToInt64(sdr["quoBlankInfoID"]);
                    quote.blankInfoMaterialTypeID = System.Convert.ToInt64(sdr["binBlankMaterialTypeID"]);
                    quote.blankMaterialType = sdr["mtyMaterialType"].ToString();
                    quote.blankThicknessEnglish = System.Convert.ToDecimal(sdr["binMaterialThicknessEnglish"]);
                    quote.blankThicknessMetric = System.Convert.ToDecimal(sdr["binMaterialThicknessMetric"]);
                    quote.blankPitchEnglish = System.Convert.ToDecimal(sdr["binMaterialPitchEnglish"]);
                    quote.blankPitchMetric = System.Convert.ToDecimal(sdr["binMaterialPitchMetric"]);
                    quote.blankWidthEnglish = System.Convert.ToDecimal(sdr["binMaterialWidthEnglish"]);
                    quote.blankWidthMetric = System.Convert.ToDecimal(sdr["binMaterialWidthMetric"]);
                    quote.blankWeightEnglish = System.Convert.ToDecimal(sdr["binMaterialWeightEnglish"]);
                    quote.blankWeightMetric = System.Convert.ToDecimal(sdr["binMaterialWeightMetric"]);
                    quote.totalAmount = System.Convert.ToDecimal(sdr["quoTotalAmount"]);
                    quote.annualVolume = System.Convert.ToInt64(sdr["quoAnnualVolume"]);
                    quote.toolToQuote = "NEW TOOLING";
                    // others can be EC or HTS
                    quote.partTypeID = System.Convert.ToInt64(sdr["quoPartTypeID"]);
                    quote.partType = sdr["ptyPartType"].ToString();
                    quote.toolCountryID = System.Convert.ToInt64(sdr["quoToolCountryID"]);
                    // TODO get country name, win loss stuff
                    quote.leadTime = System.Convert.ToInt64(sdr["quoLeadTime"]);
                    quote.salesmanID = System.Convert.ToInt64(sdr["quoSalesman"]);
                    quote.salesman = sdr["Salesman"].ToString();
                    quote.quoteNumber = System.Convert.ToInt64(sdr["quoNumber"]);
                    quote.toolingCost = System.Convert.ToDecimal(sdr["quoToolingCost"]);
                    quote.useTSGLogo = ConvertZeroToBoolean(System.Convert.ToInt64(sdr["quoUseTSGLogo"]));
                    quote.transferBarCost = System.Convert.ToDecimal(sdr["quoTransferBarCost"]);
                    quote.fixtureCost = System.Convert.ToDecimal(sdr["quoFixtureCost"]);
                    quote.dieSupportCost = System.Convert.ToDecimal(sdr["quoDieSupportCost"]);
                    quote.shippingCost = System.Convert.ToDecimal(sdr["quoShippingCost"]);
                    quote.additionalCost = System.Convert.ToDecimal(sdr["quoAdditCost"]);
                    quote.useTSGName = ConvertZeroToBoolean(System.Convert.ToInt64(sdr["quoUseTSGName"]));
                    quote.customerQuoteNumber = sdr["quoCustomerQuoteNumber"].ToString();
                    quote.dieTypeFullName  = sdr["dtyFullName"].ToString();
                    // todo get cavity id
                    quote.cavityName = sdr["cavCavityName"].ToString();
                    quote.FTBEnglish = System.Convert.ToDecimal(sdr["dinSizeFrontToBackEnglish"]);
                    quote.FTBMetric = System.Convert.ToDecimal(sdr["dinSizeFrontToBackMetric"]);
                    quote.LTREnglish = System.Convert.ToDecimal(sdr["dinSizeLeftToRightEnglish"]);
                    quote.LTRMetric = System.Convert.ToDecimal(sdr["dinSizeLeftToRightMetric"]);
                    quote.shutHeightEnglish = System.Convert.ToDecimal(sdr["dinSizeShutHeightEnglish"]);
                    quote.shutHeightMetric = System.Convert.ToDecimal(sdr["dinSizeShutHeightMetric"]);
                    quote.numberOfStations = System.Convert.ToInt64(sdr["dinNumberOfStations"]);
                    quote.dieTypeName = sdr["DieTypeName"].ToString();
                    if (SelectBy == "Company")
                    {
                        if (SelectValue == quote.companyID.ToString())
                        {
                            newPart.Quotes.Add(quote);
                        }
                    }
                    else
                    {
                        newPart.Quotes.Add(quote);
                    }
                }
                sdr.Close();
                // do not add no quotes or reservations if the part is quoted
                if (newPart.Quotes.Count == 0)
                {
                    subsql.Parameters.Clear();
                    subsql.CommandText = "select linkpartreservedtocompany.*, TSGCompanyName, TSGCompanyAbbrev from linkpartreservedtocompany, tsgcompany where prcPartID=@part and  prcCompanyID=TSGCompanyID ";
                    subsql.Parameters.AddWithValue("@part", newPart.partID);
                    sdr = subsql.ExecuteReader();
                    while (sdr.Read())
                    {
                        PartReserved reserve = new PartReserved();
                        // todo get reserves 
                        reserve.companyID = System.Convert.ToInt64(sdr["prcCompanyID"]);
                        reserve.companyAbbreviation = sdr["TSGCompanyAbbrev"].ToString();
                        reserve.companyName = sdr["TSGCompanyName"].ToString();
                        reserve.reservedBy = sdr["prcCreatedBy"].ToString();
                        if (SelectBy == "Company")
                        {
                            if (SelectValue == reserve.companyID.ToString())
                            {
                                newPart.Reserved.Add(reserve);
                            }
                        }
                    }
                    sdr.Close();
                    // do not add no quotes if part is reserved
                    if (newPart.Reserved.Count == 0) {
                        // todo get no quotes
                        subsql.Parameters.Clear();
                        subsql.CommandText = "select tblnoquote.*, TSGCompanyName, TSGCompanyAbbrev, nqrNoQuoteReasonNumber, nqrNoQuoteReason from tblnoquote, pktblnoquotereason, tsgcompany where nquPartID=@part and nquNoQuoteReasonID=nqrNoQuoteReasonID and nquCompanyID=TSGCompanyID order by nquNoQuoteID ";
                        subsql.Parameters.AddWithValue("@part", newPart.partID);
                        sdr = subsql.ExecuteReader();
                        while (sdr.Read())
                        {
                            PartNoQuote noquote = new PartNoQuote();
                            noquote.companyID = System.Convert.ToInt64(sdr["nquCompanyID"]);
                            noquote.companyAbbreviation = sdr["TSGCompanyAbbrev"].ToString();
                            noquote.companyName = sdr["TSGCompanyName"].ToString();
                            noquote.noQuoteBy = sdr["nquCreatedBy"].ToString();
                            noquote.noQuoteReasonID = System.Convert.ToInt64(sdr["nquNoQuoteReasonID"]);
                            noquote.noQuoteReason = sdr["nqrNoQuoteReason"].ToString();
                            noquote.noQuoteReasonNumber = System.Convert.ToInt64(sdr["nqrNoQuoteReasonNumber"]);
                            if (SelectBy == "Company")
                            {
                                if (SelectValue == noquote.companyID.ToString())
                                {
                                    newPart.NoQuotes.Add(noquote);
                                }
                            }
                            else
                            {
                                newPart.NoQuotes.Add(noquote);
                            }
                        }
                        sdr.Close();
                    }
                }
                subconnection.Close();
                // Any items, add the part to the list.
                if (newPart.Quotes.Count + newPart.NoQuotes.Count + newPart.Reserved.Count > 0)
                {
                    rfq.rfqPartList.Add(newPart);
                }

                //currentRow++;
            }
            if (HoldRFQ > 0)
            {
                if (rfq.rfqPartList.Count > 0)
                {
                    listRFQ.Add(rfq);
                }
            }
            return listRFQ;
        }

    }
    public class RFQParts
    {
        public Int64 partID { get; set; }
        public String partNumber { get; set; }
        public String partDescription { get; set; }
        public Int64 partTypeID { get; set; }
        public String partType { get; set; }
        public String picture { get; set; }
        public Decimal length { get; set; }
        public Decimal width { get; set; }
        public Decimal height { get; set; }
        public Int64 materialTypeID { get; set; }
        public String materialType { get; set; }
        public Decimal weight { get; set; }
        public Decimal thickness { get; set; }
        public String partLevEAU { get; set; }
        public String partName { get; set; }
        public Int64 lineNumber { get; set; }
        public String Note { get; set; }
        // Quote Information
        public List<PartQuotes> Quotes { get; set; }
        // Reserved Information
        public List<PartReserved> Reserved { get; set; }
        // No Quote Information
        public List<PartNoQuote> NoQuotes { get; set; }
    }

    public class PartNoQuote
    {
        public Int64 companyID { get; set; }
        public String companyName { get; set; }
        public String companyAbbreviation { get; set; }
        public Int64 noQuoteReasonID { get; set; }
        public String noQuoteReason { get; set; }
        public Int64 noQuoteReasonNumber { get; set; }
        public String noQuoteBy { get; set;}
    }
    public class PartReserved
    {
        public Int64 companyID { get; set; }
        public String companyName { get; set; }
        public String companyAbbreviation { get; set; }
        public String reservedBy { get; set; }
    }
    public class PartQuotes
    {
        public Int64 quoteID { get; set; }
        public String toolToQuote { get; set; }
        public Boolean isHTS { get; set; }
        public Int64 companyID { get; set; }
        public String companyAbbreviation { get; set; }
        public String companyName { get; set; }
        public Int64 estimatorID { get; set; }
        public String estimatorName { get; set; }
        public String version { get; set; }
        public Int64 jobNumber { get; set; }
        public DateTime estimatedPODate { get; set; }
        public Int64 statusID { get; set; }
        public String status { get; set; }
        public Int64 paymentTermsID { get; set; }
        public String paymentTerms { get; set; }
        public Int64 shippingTermsID { get; set; }
        public String shippingTerms { get; set; }
        public Decimal totalAmount { get; set; }
        public Int64 annualVolume { get; set; }
        public Int64 partTypeID { get; set; }
        public String partType { get; set; }
        public Int64 toolCountryID { get; set; }
        public String toolCountry { get; set; }
        public Int64 leadTime { get; set; }
        public Int64 salesmanID { get; set; }
        public String salesman { get; set; }
        public Int64 quoteNumber { get; set; }
        public String customerQuoteNumber { get; set; }
        public Boolean useTSGLogo { get; set; }
        public Decimal toolingCost { get; set; }
        public Decimal transferBarCost { get; set; }
        public Decimal fixtureCost { get; set; }
        public Decimal dieSupportCost { get; set; }
        public Decimal shippingCost { get; set; }
        public Decimal additionalCost { get; set; }
        public Boolean useTSGName { get; set; }
        public Int64 dieInfoID { get; set; }
        public Int64 dieTypeID { get; set; }
        public Int64 cavityID { get; set;  }
        public String cavityName { get; set; }
        public Decimal FTBEnglish { get; set; }
        public Decimal FTBMetric { get; set; }
        public Decimal LTREnglish { get; set; }
        public Decimal LTRMetric { get; set; }
        public Decimal shutHeightEnglish { get; set; }
        public Decimal shutHeightMetric { get; set; }
        public Int64 numberOfStations { get; set; }
        public String dieTypeName { get; set; }
        public String dieTypeFullName { get; set; }
        public String process { get { return dieTypeFullName; } }
        public Int64 blankInfoID { get; set; }
        public Int64 blankInfoMaterialTypeID { get; set; }
        public String blankMaterialType { get; set; }
        public Decimal blankThicknessEnglish { get; set; }
        public Decimal blankThicknessMetric { get; set; }
        public Decimal blankPitchEnglish { get; set; }
        public Decimal blankPitchMetric { get; set; }
        public Decimal blankWidthEnglish { get; set; }
        public Decimal blankWidthMetric { get; set; }
        public Decimal blankWeightEnglish { get; set; }
        public Decimal blankWeightMetric { get; set; }
        public Int64 winLossReasonID { get; set; }
        public String winLossReason { get; set; }
    }
}