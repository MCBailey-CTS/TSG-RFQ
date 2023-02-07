using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.SharePoint.Client;

namespace RFQ
{
    public partial class SaveAssembly : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string assemblyId = "";
            if (Request["assemblyId"] != null && Request["assemblyId"].ToString() != "")
            {
                assemblyId = Request["assemblyId"].ToString().Replace("A", "");
            }

            Boolean delete = false;
            if (Request["delete"] != null && Request["delete"].ToString() != "")
            {
                delete = System.Convert.ToBoolean(Request["delete"].ToString());
            }

            string assemblyNumber = "";
            if (Request["assemblyNumber"] != null && Request["assemblyNumber"].ToString() != "")
            {
                assemblyNumber = Request["assemblyNumber"];
            }

            string assemblyDescription = "";
            if (Request["assemblyDescription"] != null && Request["assemblyDescription"].ToString() != "")
            {
                assemblyDescription = Request["assemblyDescription"].ToString();
            }

            string lineNumbersLinked = "";
            if (Request["lineNumbers"] != null && Request["lineNumbers"].ToString() != "")
            {
                lineNumbersLinked = Request["lineNumbers"].ToString();

            }

            string rfqId = "";
            if (Request["rfqId"] != null && Request["rfqId"].ToString() != "")
            {
                rfqId = Request["rfqId"].ToString();
            }

            //string pictureName = "";
            //if (Request["picName"] != null && Request["picName"].ToString() != "")
            //{
            //    rfqId = Request["picName"].ToString();
            //}

            var lineNumbers = lineNumbersLinked.Split(',');


            int assemblyType = 1;


            Site master = new Site();
            SqlCommand sql = new SqlCommand();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            sql.Connection = connection;

            string user = master.getUserName();

            if (delete)
            {
                sql.CommandText = "Delete from linkAssemblyToRFQ where atrAssemblyId = @assemblyId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                master.ExecuteNonQuery(sql, "Save Assembly");

                sql.CommandText = "Delete from linkAssemblyToPart where atpAssemblyId = @assemblyId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                master.ExecuteNonQuery(sql, "Save Assembly");

                sql.CommandText = "Delete from tblReserveAssembly where rasAssemblyId = @assemblyId  ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                master.ExecuteScalar(sql, "Save Assembly");

                sql.CommandText = "Delete from tblAssembly where assAssemblyId = @assemblyId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                master.ExecuteNonQuery(sql, "Save Assembly");
            }
            else if (assemblyId == "")
            {
                // setting the line number to whatever the max is + 1 or if there are none it will stay at 0
                int lineNum = 0;
                sql.CommandText = "select max(a.assLineNumber) as maxNum from linkAssemblyToRFQ atr ";
                sql.CommandText += "inner join tblAssembly a on a.assAssemblyId = atr.atrAssemblyId ";
                sql.CommandText += "where atr.atrRfqId = @rfqId ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqId", rfqId);
                SqlDataReader dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    if (dr["maxNum"].ToString() != "")
                    {
                        lineNum = System.Convert.ToInt32(dr["maxNum"].ToString()) + 1;
                    }
                }
                dr.Close();

                sql.CommandText = "insert into tblAssembly (assNumber, assDescription, assCreated, assCreatedBy, assType, assLineNumber) ";
                sql.CommandText += "output inserted.assAssemblyId ";
                sql.CommandText += "values(@number, @description, GETDATE(), @user, @type, @lineNum) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@number", assemblyNumber);
                sql.Parameters.AddWithValue("@description", assemblyDescription);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@type", assemblyType);
                sql.Parameters.AddWithValue("@lineNum", lineNum);
                assemblyId = master.ExecuteScalar(sql, "Save Assembly").ToString();

                sql.CommandText = "insert into linkAssemblyToRFQ (atrAssemblyId, atrRfqId, atrCreated, atrCreatedBy) ";
                sql.CommandText += "values(@assembly, @rfqId, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@assembly", assemblyId);
                sql.Parameters.AddWithValue("@rfqId", rfqId);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Save Assembly");

                for (int i = 0; i < lineNumbers.Length; i++)
                {
                    string partId = "";
                    sql.CommandText = "Select p.prtPARTID from linkPartToRFQ ptr ";
                    sql.CommandText += "inner join tblPart p on p.prtPARTID = ptr.ptrPartID ";
                    sql.CommandText += "where ptr.ptrRFQID = @rfqID and p.prtRFQLineNumber = @lineNum ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqId);
                    sql.Parameters.AddWithValue("@lineNum", lineNumbers[i]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        partId = dr["prtPARTID"].ToString();
                    }
                    dr.Close();

                    sql.CommandText = "insert into linkAssemblyToPart (atpAssemblyId, atpPartId, atpCreated, atpCreatedBy) ";
                    sql.CommandText += "values(@assemblyId, @partId, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                    sql.Parameters.AddWithValue("@partId", partId);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Save Assembly");
                }
                Response.Write(assemblyId);
            }
            else
            {
                sql.CommandText = "update tblAssembly set assNumber = @number, assDescription = @description, assType = @type, assModified = GETDATE(), assModifiedBy = @user ";
                sql.CommandText += "where assAssemblyId = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", assemblyId);
                sql.Parameters.AddWithValue("@number", assemblyNumber);
                sql.Parameters.AddWithValue("@description", assemblyDescription);
                sql.Parameters.AddWithValue("@type", assemblyType);
                sql.Parameters.AddWithValue("@user", user);
                master.ExecuteNonQuery(sql, "Save Assembly");

                sql.CommandText = "Delete from linkAssemblyToPart where atpAssemblyId = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", assemblyId);
                master.ExecuteNonQuery(sql, "Save Assembly");

                for (int i = 0; i < lineNumbers.Length; i++)
                {
                    string partId = "";
                    sql.CommandText = "Select p.prtPARTID from linkPartToRFQ ptr ";
                    sql.CommandText += "inner join tblPart p on p.prtPARTID = ptr.ptrPartID ";
                    sql.CommandText += "where ptr.ptrRFQID = @rfqID and p.prtRFQLineNumber = @lineNum ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqId);
                    sql.Parameters.AddWithValue("@lineNum", lineNumbers[i]);
                    SqlDataReader dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        partId = dr["prtPARTID"].ToString();
                    }
                    dr.Close();

                    sql.CommandText = "insert into linkAssemblyToPart (atpAssemblyId, atpPartId, atpCreated, atpCreatedBy) ";
                    sql.CommandText += "values(@assemblyId, @partId, GETDATE(), @user) ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@assemblyId", assemblyId);
                    sql.Parameters.AddWithValue("@partId", partId);
                    sql.Parameters.AddWithValue("@user", user);
                    master.ExecuteNonQuery(sql, "Save Assembly");
                }
            }

            // Upload image into SharePoint


            //String FileName = "";
            //try
            //{
            //    FileName = filePicture.PostedFile.FileName;
            //}
            //catch
            //{

            //}
            //if (FileName != "")
            //{
            //    ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
            //    ctx.Credentials = master.getSharePointCredentials();
            //    Web web = ctx.Web;
            //    ctx.Load(web);
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            //    Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("Part Pictures");
            //    byte[] fileData = null;
            //    using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
            //    {
            //        fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
            //    }
            //    System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
            //    FileCreationInformation newFile = new FileCreationInformation();
            //    newFile.ContentStream = newStream;
            //    newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part Pictures/" + pictureName;
            //    newFile.Overwrite = true;
            //    Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
            //    partPicturesList.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //    // set the Attributes
            //    Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
            //    newItem["assID"] = assemblyId;
            //    newItem["assNumber"] = assemblyNumber;
            //    newItem["assDescription"] = assemblyDescription;
            //    newItem["RFQ"] = "https://tsgrfq.azurewebsites.net/EditRFQ?id=" + rfqId;
            //    newItem.Update();
            //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

            //    sql.CommandText = "update tblassembly set assPicture = @picture, assModified = GETDATE(), assModifiedBy = @user where assAssemblyID = @part";
            //    sql.Parameters.Clear();
            //    sql.Parameters.AddWithValue("@user", master.getUserName());
            //    sql.Parameters.AddWithValue("@picture", pictureName);
            //    sql.Parameters.AddWithValue("@part", assemblyNumber);
            //    master.ExecuteNonQuery(sql, "Save Assembly");
            //}





            connection.Close();
        }
    }
}