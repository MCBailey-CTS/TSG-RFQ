using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace RFQ
{
    public class RFQFields
    {
        public class RFQFieldName
        {
            public string tableName { get; set; }
            public string fieldName { get; set; }
            public string displayName { get; set; }
            public string fieldType { get; set; }
            public string lookupTable { get; set; }
            public string lookupField { get; set; }
            public string returnField { get; set; }
        }
        public List<RFQFieldName> getFieldList()
        {
            List<RFQFieldName> fieldList = new List<RFQFieldName>();
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            sql.CommandText = "select sfnTableName, sfnFieldName, sfnDisplayName, sfnFieldType, sfnLookupTable, sfnLookupField, sfnReturnField from pktblSystemFieldName order by sfnDisplaySequence";
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                fieldList.Add(new RFQFieldName() 
                { 
                    tableName = dr.GetValue(0).ToString(), 
                    fieldName = dr.GetValue(1).ToString(), 
                    displayName = dr.GetValue(2).ToString(), 
                    fieldType = dr.GetValue(3).ToString(),
                    lookupTable = dr.GetValue(4).ToString(), 
                    lookupField = dr.GetValue(5).ToString(), 
                    returnField = dr.GetValue(6).ToString()
                });
            }
            connection.Close();
            return fieldList;
        }
    }
}