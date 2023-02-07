using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RFQ.Models
{    
    public class TSGCompany
    {
        public Int64 company_id { get; set; }
        public string abbreviation { get; set; }
        public string company_name { get; set; }
    }
    public class TSGCompanyList
    {
        public List<TSGCompany> ReturnCompanyList 
        {
            get
            {
                List<TSGCompany> CompanyList = new List<TSGCompany>();
                CompanyList.Add(new TSGCompany { company_id = 1, abbreviation = "TSG" });
                CompanyList.Add(new TSGCompany { company_id = 2, abbreviation = "ATS" });
                CompanyList.Add(new TSGCompany { company_id = 3, abbreviation = "BTS" });
                CompanyList.Add(new TSGCompany { company_id = 4, abbreviation = "CTS" });
                CompanyList.Add(new TSGCompany { company_id = 5, abbreviation = "DTS" });
                CompanyList.Add(new TSGCompany { company_id = 6, abbreviation = "EIG" });
                CompanyList.Add(new TSGCompany { company_id = 7, abbreviation = "ETS" });
                CompanyList.Add(new TSGCompany { company_id = 8, abbreviation = "GTS" });
                CompanyList.Add(new TSGCompany { company_id = 9, abbreviation = "HTS" });
                CompanyList.Add(new TSGCompany { company_id = 10, abbreviation = "LTS" });
                CompanyList.Add(new TSGCompany { company_id = 11, abbreviation = "MTS" });
                CompanyList.Add(new TSGCompany { company_id = 12, abbreviation = "RTS" });
                CompanyList.Add(new TSGCompany { company_id = 13, abbreviation = "STS" });
                CompanyList.Add(new TSGCompany { company_id = 14, abbreviation = "3DM" });
                CompanyList.Add(new TSGCompany { company_id = 15, abbreviation = "UGS" });
                return CompanyList;
            }
        } 
    }
}