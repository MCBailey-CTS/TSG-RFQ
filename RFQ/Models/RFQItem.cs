using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RFQ.Models
{
    public class RFQItem
    {
        public string rfqid { get; set; }
        public string owner { get; set; }
        public string customer { get; set; }
        public string customer_rfq { get; set; }
        public string rank { get; set; }
        public string salesman { get; set; }
        public string date_received { get; set; }
        public string date_due { get; set; }
        public string status { get; set; }
        public string numberOfParts { get; set; }
        public string numberOfPartsReserved { get; set; }
        public string numberOfPartsQuoted { get; set; }
        public string liveWork { get; set; }
        public Int64 item_count { get; set; }
        public string button { get; set; }
        public string rfqLink { get; set; }
        public List<TSGCompany> notification_list { get; set; }
        public string notified { get; set; }
        public string oem { get; set; }
        public RFQItem()
        {
            rfqid = "";
            owner = "";
            customer = "";
            customer_rfq = "";
            rank = "";
            salesman = "";
            date_received = "";
            date_due = "";
            status = "";
            item_count = 0;
            notification_list = new List<TSGCompany>();
            numberOfPartsQuoted = "";
            button = "";
            rfqLink = "";
        }
        public string AbbreviationList
        {
            get
            {
                String retval = "";
                foreach (TSGCompany co in notification_list)
                {
                    retval = retval + co.abbreviation + "\n";
                }
                return retval;

            }
        }
    }
}