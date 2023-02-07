using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RFQ.Models
{
    public class ReservedItem
    {
        public string rfqID { get; set; }
        public string partNumber { get; set; }
        public string partID { get; set; }
        public string customer { get; set; }
        public string reservedBy { get; set; }
        public string reserved { get; set; }
        public string tsgCompany { get; set; }
        public int tsgCompanyNum { get; set; }
        public string dueDate { get; set; }
        public string partPicture { get; set; }
        public string partNote { get; set; }
        public ReservedItem()
        {
            rfqID = "";
            partNumber = "";
            customer = "";
            reservedBy = "";
            reserved = "";
            tsgCompany = "";
            tsgCompanyNum = 1;
            partID = "";
            dueDate = "";
            partPicture = "";
            partNote = "";
        }


        //prcRFQID, prtPartNumber, TSGCompanyAbbrev, CustomerName, prcCreatedBy, prcCreated
    }
}