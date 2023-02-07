using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFQ
{
    public partial class SetDisposition : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            string quoteID = "";
            try
            {
                quoteID = Request["quoteID"];
            }
            catch
            {

            }
            string winLoss = "";
            try
            {
                winLoss = Request["winLoss"];
            }
            catch
            {

            }
            string winLossReason = "";
            try
            {
                winLossReason = Request["winLossReason"];
            }
            catch
            {

            }
            string targetPrice = "";
            try
            {
                targetPrice = Request["targetPrice"];
            }
            catch
            {

            }
            string notes = "";
            try
            {
                notes = Request["notes"];
            }
            catch
            {

            }
            string po = "";
            try
            {
                po = Request["PO"];
            }
            catch
            {

            }
            string awarded = "";
            try
            {
                awarded = Request["Awarded"];
            }
            catch
            {

            }

            if(quoteID.Split('-')[1] == "RFQ")
            {
                sql.CommandText = "Update tblQuote set quoStatusID = @status, quoWinLossID = @winLoss, quoWinLossReasonID = @reason, quoTargetPrice = @target, quoDispositionNote = @note, quoAwardedAmount = @award, ";
                sql.CommandText += "quoPONumber = @po, quoModified = GETDATE(), quoModifiedBy = @user where quoQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID.Split('-')[0]);
                if (winLoss == "1")
                {
                    sql.Parameters.AddWithValue("@status", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@status", 8);
                }
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@reason", winLossReason);
                sql.Parameters.AddWithValue("@target", targetPrice);
                sql.Parameters.AddWithValue("@note", notes);
                sql.Parameters.AddWithValue("@award", awarded);
                sql.Parameters.AddWithValue("@po", po);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Set Disposition");
            }
            else if (quoteID.Split('-')[1] == "SA")
            {
                sql.CommandText = "Update tblECQuote set ecqStatus = @status, ecqWinLossID = @winLoss, ecqWinLossReasonID = @reason, ecqTargetPrice = @target, ecqDispositionNote = @note, ecqAwardedAmount = @award, ";
                sql.CommandText += "ecqPONumber = @po, ecqModified = GETDATE(), ecqModifiedBy = @user where ecqECQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID.Split('-')[0]);
                if (winLoss == "1")
                {
                    sql.Parameters.AddWithValue("@status", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@status", 8);
                }
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@reason", winLossReason);
                sql.Parameters.AddWithValue("@target", targetPrice);
                sql.Parameters.AddWithValue("@note", notes);
                sql.Parameters.AddWithValue("@award", awarded);
                sql.Parameters.AddWithValue("@po", po);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                master.ExecuteNonQuery(sql, "Set Disposition");
            }
            else if (quoteID.Split('-')[1] == "HTS")
            {
                sql.CommandText = "update tblHTSQuote set hquStatusID = @status, hquWinLossID = @winLoss, hquWinLossReasonID = @reason, hquPONumber = @po, hquAwardedAmount = @aa, hquTargetPrice = @target, ";
                sql.CommandText += "hquDispositionNote = @note, hquModified = GETDATE(), hquModifiedBy = @user where hquHTSQuoteID = @id ";
                sql.Parameters.Clear();
                if (winLoss == "1")
                {
                    sql.Parameters.AddWithValue("@status", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@status", 8);
                }
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@reason", winLossReason);
                sql.Parameters.AddWithValue("@target", targetPrice);
                sql.Parameters.AddWithValue("@po", po);
                sql.Parameters.AddWithValue("@aa", awarded);
                sql.Parameters.AddWithValue("@note", notes);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@id", quoteID.Split('-')[0]);
                master.ExecuteNonQuery(sql, "Set Disposition");
            }
            else if (quoteID.Split('-')[1] == "STS")
            {
                sql.CommandText = "update tblSTSQuote set squStatusID = @status, squWinLossID = @winLoss, squWinLossReasonID = @reason, squPONumber = @po, squAwardedAmount = @awarded, squTargetPrice = @target, ";
                sql.CommandText += "squDispositionNote = @note, squModified = GETDATE(), squModifiedBy = @user where squSTSQuoteID = @id ";
                sql.Parameters.Clear();
                if (winLoss == "1")
                {
                    sql.Parameters.AddWithValue("@status", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@status", 8);
                }
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@reason", winLossReason);
                sql.Parameters.AddWithValue("@po", po);
                sql.Parameters.AddWithValue("@awarded", awarded);
                sql.Parameters.AddWithValue("@target", targetPrice);
                sql.Parameters.AddWithValue("@note", notes);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@id", quoteID.Split('-')[0]);
                master.ExecuteNonQuery(sql, "Set Disposition");
            }
            else if (quoteID.Split('-')[1] == "UGS")
            {
                sql.CommandText = "update tblUGSQuote set uquStatusID = @status, uquWinLossID = @winLoss, uquWinLossReasonID = @reason, uquPONumber = @po, uquAwardedAmount = @awarded, uquTargetPrice = @target, ";
                sql.CommandText += "uquDispositionNote = @note, uquModified = GETDATE(), uquModifiedBy = @user where uquUGSQuoteID = @id ";
                sql.Parameters.Clear();
                if (winLoss == "1")
                {
                    sql.Parameters.AddWithValue("@status", 7);
                }
                else
                {
                    sql.Parameters.AddWithValue("@status", 8);
                }
                sql.Parameters.AddWithValue("@winLoss", winLoss);
                sql.Parameters.AddWithValue("@reason", winLossReason);
                sql.Parameters.AddWithValue("@po", po);
                sql.Parameters.AddWithValue("@awarded", awarded);
                sql.Parameters.AddWithValue("@target", targetPrice);
                sql.Parameters.AddWithValue("@note", notes);
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@id", quoteID.Split('-')[0]);
                master.ExecuteNonQuery(sql, "Set Disposition");
            }
            

            connection.Close();
        }
    }
}