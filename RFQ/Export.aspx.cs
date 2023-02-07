using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class Export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sqlText = Request["sql"];
            String[] op = {"from"};
            List<String>parts = sqlText.Split(op,StringSplitOptions.None).ToList<String>();

            String fieldpart = parts[0].Replace("select ","").Trim();
            while (fieldpart.Contains("coalesce"))
            {
                int startpos = fieldpart.IndexOf("coalesce");
                int endpos = fieldpart.IndexOf(" as ", startpos);
                fieldpart = fieldpart.Substring(0, startpos - 1) + fieldpart.Substring(endpos+3, fieldpart.Length-endpos-3);
            }
            while (fieldpart.Contains("concat"))
            {
                int startpos = fieldpart.IndexOf("concat");
                int endpos = fieldpart.IndexOf(" as ", startpos);
                fieldpart = fieldpart.Substring(0, startpos - 1) + fieldpart.Substring(endpos + 3, fieldpart.Length - endpos - 3);
            }
            while (fieldpart.Contains("convert"))
            {
                int startpos = fieldpart.IndexOf("convert");
                int endpos = fieldpart.IndexOf(" as ", startpos);
                fieldpart = fieldpart.Substring(0, startpos - 1) + fieldpart.Substring(endpos + 3, fieldpart.Length - endpos - 3);
            }
            List<String> fields = fieldpart.Split(',').ToList<String>();
            String comma="";
            String data = "";
            foreach (String field in fields) {
                data += comma;
                String myfield = field.Trim();
                if (myfield.IndexOf(" as ") > 0)
                {
                    String[] asop = { " as " };
                    List<String> fieldparts = field.Split(asop, StringSplitOptions.None).ToList<String>();
                    myfield = fieldparts[1];
                }
                if (myfield.IndexOf('.')>0) {
                    // spaces between uppercase letters
                    List<String> fieldparts = field.Split('.').ToList<String>();
                    data += System.Text.RegularExpressions.Regex.Replace(fieldparts[1].Replace("TSG", ""), "[A-Z]", " $0").Trim();
                } 
                else 
                {
                    // spaces between uppercase letters
                    data += System.Text.RegularExpressions.Regex.Replace(myfield.Replace("TSG", ""), "[A-Z]", " $0").Trim();
                }
                comma=",";
            }
            data += "\n";
            //Get properties using reflection.
            String connectionString = "Data Source=cqz02f6h9c.database.windows.net;Initial Catalog=TSGMaster;Persist Security Info=True;User ID=TSGTestdev;Password=CA09876ca";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;
            sql.CommandText = sqlText;
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read()) 
            {
                comma="";
                for (int i=0; i < dr.FieldCount; i++) 
                {
                    String val = dr.GetValue(i).ToString();
                    val = val.Replace(" 12:00:00 AM", "");
                    if (val.Contains(",") || val.Contains("\""))
                        val = '"' + val.Replace("\"", "\"\"") + '"';
                    data += comma;
                    data += val;
                    comma=",";
                }
                data += "\n";
            }
            connection.Close();
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=Export.csv");
            Response.Write(data);
            HttpContext.Current.Response.End();
        }
    }
}