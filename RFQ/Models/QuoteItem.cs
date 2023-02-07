using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RFQ.Models
{
    public class QuoteItem
    {
        public string rfqID { get; set; }
        public string quoteID { get; set; }
        public string status { get; set; }
        public string customer { get; set; }
        public string estimator { get; set; }
        public string created { get; set; }
        public string tsgCompany { get; set; }
        public string quoteType { get; set; }
        public int quoteTypeNum { get; set; }
        public string realQuoteID { get; set; }
        public int tsgCompanyNum { get; set; }
        public string partNumber { get; set; }
        public string partID { get; set; }
        public string dueDate { get; set; }
        public string partPicture { get; set; }
        public string partNote { get; set; }
        public string url { get; set; }
        public QuoteItem()
        {
            rfqID = "";
            quoteID = "";
            customer = "";
            status = "";
            customer = "";
            estimator = "";
            created = "";
            tsgCompany = "";
            partNumber = "";
            partID = "";
            dueDate = "";
            partPicture = "";
            realQuoteID = "";
            partNote = "";
        }
    }
}