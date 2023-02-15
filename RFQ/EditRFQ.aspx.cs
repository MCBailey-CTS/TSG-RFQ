using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing;
using System.Net.Mail;
//using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage.Blob;
using System.Web.Services;
using System.Configuration;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Documents;

namespace RFQ
{
    public partial class EditRFQ : System.Web.UI.Page
    {
        public Int64 RFQID = 0;
        public List<String> ListOfColors = new List<String>();
        public Boolean IsMasterCompany = false;
        public long UserCompanyID = 0;

        protected static void AddDropDownListValues(SqlCommand sql, DropDownList ddlVehicle, string textField, string valueField, string selected_value = null)
        {
            using (SqlDataReader vDR = sql.ExecuteReader())
            {
                ddlVehicle.DataSource = vDR;
                ddlVehicle.DataTextField = "vehVehicleName";
                ddlVehicle.DataValueField = "vehVehicleID";
                if (!(selected_value is null))
                    ddlVehicle.SelectedValue = selected_value;
                ddlVehicle.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //hdnImportParts.Visible = false;
            ClientScript.GetPostBackEventReference(this, string.Empty);
            RFQID = System.Convert.ToInt64(Request["id"]);
            lblMessage.Text = "";
            if (!IsPostBack)
            {
                btnUnlockRFQ.Visible = false;
                populate_ListOfColors();
                Site master = new RFQ.Site();
                hdnCompanyID.Value = master.getCompanyId().ToString();
                IsMasterCompany = master.getMasterCompany();
                UserCompanyID = master.getCompanyId();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;


                var currentUser = master.getUserName();
                cbSendAsMe.Visible = currentUser != "pdavis@toolingsystemsgroup.com" || currentUser != "dmaguire@toolingsystemsgroup.com";
                

                if (currentUser != "rmumford@toolingsystemsgroup.com" && currentUser != "jdalman@toolingsystemsgroup.com" && currentUser != "bduemler@toolingsystemsgroup.com" && currentUser != "dmaguire@toolingsystemsgroup.com")
                    litScript.Text = "<script>document.getElementById('btnSendNoQuoteDialog').style.visibility = 'hidden';</script>";

                if (currentUser != "dparker@toolingsystemsgroup.com" && currentUser != "jdalman@toolingsystemsgroup.com")
                    calIntDueDate.Enabled = false;

                if (master.getCompanyId() != 1)
                    lblMessage.Text = "\n<script>$('#deleteRFQBut').hide();</script>";
                
                if (master.getCompanyId() != 15)
                    lblMessage.Text += "<script>$('#btnUGSMultiQuote').hide();</script><script>$('#btnUGSSummary').hide();</script>";
                
                try
                {

                    sql.CommandText = "select ProgramID, ProgramName from Program where ProgramName not in ('0','  ADD NEW') order by ProgramName";
                    sql.Parameters.Clear();
                    using (SqlDataReader progDR = sql.ExecuteReader())
                    {
                        ddlProgram.DataSource = progDR;
                        ddlProgram.DataTextField = "ProgramName";
                        ddlProgram.DataValueField = "ProgramID";
                        ddlProgram.SelectedValue = "2206";
                        ddlProgram.DataBind();
                    }

                    sql.CommandText = "select OEMID, OEMName from OEM where OEMName not in ('nul;','undefined') order by OEMName";
                    sql.Parameters.Clear();

                    using (SqlDataReader oemDR = sql.ExecuteReader())
                    {
                        ddlOEM.DataSource = oemDR;
                        ddlOEM.DataTextField = "OEMName";
                        ddlOEM.DataValueField = "OEMID";
                        ddlOEM.SelectedValue = "39";
                        ddlOEM.DataBind();
                    }

                    sql.CommandText = "Select rhaRFQHandlingDescription, rhaRFQHandlingID from pktblRFQHandling";
                    sql.Parameters.Clear();
                    using (SqlDataReader hanDR = sql.ExecuteReader())
                    {
                        ddlHandling.DataSource = hanDR;
                        ddlHandling.DataTextField = "rhaRFQHandlingDescription";
                        ddlHandling.DataValueField = "rhaRFQHandlingID";
                        ddlHandling.SelectedValue = "4";
                        ddlHandling.DataBind();
                    }

                    sql.CommandText = "Select CustomerContactID, Name from CustomerContact where (ccoInactive = 0 or ccoInactive is null) order by Name";
                    sql.Parameters.Clear();
                    using (SqlDataReader cusDR = sql.ExecuteReader())
                    {
                        ddlCustomerContact.DataSource = cusDR;
                        ddlCustomerContact.DataTextField = "Name";
                        ddlCustomerContact.DataValueField = "CustomerContactID";
                        ddlCustomerContact.SelectedValue = "1";
                        ddlCustomerContact.DataBind();
                    }

                    sql.CommandText = "select vehVehicleID, vehVehicleName from pktblVehicle order by vehVehicleName";
                    sql.Parameters.Clear();
                    SqlDataReader vDR = sql.ExecuteReader();
                    ddlVehicle.DataSource = vDR;
                    ddlVehicle.DataTextField = "vehVehicleName";
                    ddlVehicle.DataValueField = "vehVehicleID";
                    ddlVehicle.DataBind();
                    vDR.Close();
                    ddlVehicle.SelectedValue = "1";


                    sql.CommandText = "Select astAssemblyTypeId, astAssemblyType from  pktblAssemblyType order by astAssemblyType ";
                    sql.Parameters.Clear();
                    SqlDataReader assDr = sql.ExecuteReader();
                    ddlAssemblyType.DataSource = assDr;
                    ddlAssemblyType.DataTextField = "astAssemblyType";
                    ddlAssemblyType.DataValueField = "astAssemblyTypeId";
                    ddlAssemblyType.DataBind();
                    assDr.Close();




                    sql.CommandText = "select tcyToolCountryID, tcyToolCountry from pktblToolCountry order by tcyToolCountry";
                    sql.Parameters.Clear();
                    using (SqlDataReader tcDR = sql.ExecuteReader())
                    {
                        ddlToolCountry.DataSource = tcDR;
                        ddlToolCountry.DataTextField = "tcyToolCountry";
                        ddlToolCountry.DataValueField = "tcyToolCountryID";
                        ddlToolCountry.SelectedValue = "2";
                        ddlToolCountry.DataBind();
                    }

                    sql.CommandText = "select rstRFQStatusID, rstRFQStatusDescription from pktblRFQStatus order by iif(rstRFQStatusDescription = 'RFQ Received',0,1), rstRFQStatusDescription";
                    sql.Parameters.Clear();
                    using (SqlDataReader statusDR = sql.ExecuteReader())
                    {
                        ddlStatus.DataSource = statusDR;
                        ddlStatus.DataTextField = "rstRFQStatusDescription";
                        ddlStatus.DataValueField = "rstRFQStatusID";
                        ddlStatus.SelectedValue = "2";
                        ddlStatus.DataBind();
                    }

                    sql.CommandText = "select ptyProductTypeID, ptyProductType from pktblProductType order by ptyProductType";
                    sql.Parameters.Clear();
                    using (SqlDataReader ptDR = sql.ExecuteReader())
                    {
                        ddlProductType.DataSource = ptDR;
                        ddlProductType.DataTextField = "ptyProductType";
                        ddlProductType.DataValueField = "ptyProductTypeID";
                        ddlProductType.SelectedValue = "2";
                        ddlProductType.DataBind();
                    }

                    sql.CommandText = "select rchRFQcheckListID, rchRFQCheckListItemText from pktblRFQCheckList where rchRFQorPart='Part' order by rchRFQCheckListID";
                    sql.Parameters.Clear();

                    using (SqlDataReader PartCheckListDR = sql.ExecuteReader())
                    {
                        lbCheckList.Text = "<div align='left'>Select From the Following<BR>";
                        while (PartCheckListDR.Read())
                        {
                            lbCheckList.Text += $"<div align='left'><input type='checkbox' class='lbCheckListOption' value='{PartCheckListDR.GetValue(0).ToString()}'> ";
                            lbCheckList.Text += $"&nbsp;{PartCheckListDR.GetValue(1).ToString().Trim()}</div>";
                        }
                    }
                    lbCheckList.Text += "</div>";
                    lblRFQCheckList.Text = "<div align='left'>Select From the Following<BR>";
                    sql.CommandText = "select rchRFQCheckListID, rchRFQCheckListItemText from pktblRFQCheckList where rchRFQorPart='RFQ' order by rchRFQCheckListID";
                    sql.Parameters.Clear();
                    using (SqlDataReader RFQCheckListDR = sql.ExecuteReader())
                    {
                        while (RFQCheckListDR.Read())
                        {
                            lblRFQCheckList.Text += "<div align='left'><input type='checkbox' class='lbRFQCheckListOption' value='" + RFQCheckListDR.GetValue(0).ToString() + "'> ";
                            lblRFQCheckList.Text += $"&nbsp;{RFQCheckListDR.GetValue(1).ToString().Trim()}</div>";
                        }
                    }

                    sql.CommandText = "select CustomerID, concat(CustomerName,' (',CustomerNumber,')') as Name from Customer where cusInactive <> 1 or cusInactive is null order by CustomerName ";
                    using (SqlDataReader CustomerDR = sql.ExecuteReader())
                    {
                        ddlCustomer.DataSource = CustomerDR;
                        ddlCustomer.DataTextField = "Name";
                        ddlCustomer.DataValueField = "CustomerID";
                        ddlCustomer.DataBind();
                        
                    }

                    if (RFQID == 0)
                        ddlCustomer.Items.Insert(0, "Please Select");

                    sql.CommandText = "select rsoSourceID, rsoSourceName from pktblRFQSource order by rsoSourceName";
                    using (SqlDataReader sourceDR = sql.ExecuteReader())
                    {
                        ddlRFQSource.DataSource = sourceDR;
                        ddlRFQSource.DataTextField = "rsoSourceName";
                        ddlRFQSource.DataValueField = "rsoSourceID";
                        ddlRFQSource.SelectedValue = "24";
                        ddlRFQSource.DataBind();
                    }

                    sql.CommandText = "select rsoSourceID, rsoSourceName from pktblRFQSource order by rsoSourceName";
                    using (SqlDataReader sourceDR2 = sql.ExecuteReader())
                    {
                        ddlRFQSource2.DataSource = sourceDR2;
                        ddlRFQSource2.DataTextField = "rsoSourceName";
                        ddlRFQSource2.DataValueField = "rsoSourceID";
                        ddlRFQSource2.SelectedValue = "24";
                        ddlRFQSource2.DataBind();
                    }

                    sql.CommandText = "select ptyPartTypeID, ptyPartTypeDescription from pktblPartType order by ptyPartTypeDescription";
                    using (SqlDataReader partTypeDR = sql.ExecuteReader())
                    {
                        ddlPartType.DataSource = partTypeDR;
                        ddlPartType.DataTextField = "ptyPartTypeDescription";
                        ddlPartType.DataValueField = "ptyPartTypeID";
                        ddlPartType.SelectedValue = "33";
                        ddlPartType.DataBind();
                    }

                   

                    sql.Parameters.Clear();

                    sql.CommandText = "Select CONCAT(nqrNoQuoteReasonNumber, ' - ', nqrNoQuoteReason) from pktblNoQuoteReason where nqrActive = 1";
                    using (SqlDataReader nqrDR = sql.ExecuteReader())
                        while (nqrDR.Read())
                            txtNoQuoteText.Text += Server.HtmlDecode(nqrDR.GetValue(0).ToString() + "\n");
                    
                    sql.Parameters.Clear();

                    string emlCustomername = ddlCustomer.SelectedItem.ToString();
                    string emlSalesmanName = "";
                    string emlSalesmanEmail = "";
                    string emlSalesmanPhone = "";
                    string emlEstEmail = "";
                    string emlEstPhone = "";
                    string emlEstName = "";
                    string CusName = "";

                    sql.CommandText = "Select ps.Name, ps.Email, ps.MobilePhone, cus.CustomerName, est.estEmail, est.estOfficePhone, CONCAT (est.estFirstName, est.estLastName) as estname from TSGSalesman ps ";
                    sql.CommandText += "join tblRFQ rfq on rfq.rfqSalesman = ps.TSGSalesmanID ";
                    sql.CommandText += "join customer cus on cus.customerID = rfq.rfqCustomerID ";
                    sql.CommandText += "join tblSTSQuote stsquo on stsquo.squRFQNum = rfq.rfqID ";
                    sql.CommandText += "join pktblEstimators est on est.estEstimatorID = stsquo.squEstimatorID ";
                    sql.CommandText += "where stsquo.squRfqNum = @rfqid ";
                    sql.Parameters.AddWithValue("@rfqid", RFQID.ToString());
                    using (SqlDataReader salesdr = sql.ExecuteReader())
                        while (salesdr.Read())
                        {
                            emlSalesmanName = salesdr["Name"].ToString();
                            emlSalesmanEmail = salesdr["Email"].ToString();
                            emlSalesmanPhone = salesdr["MobilePhone"].ToString();
                            CusName = salesdr["CustomerName"].ToString();
                            emlEstEmail = salesdr["estEmail"].ToString();
                            emlEstPhone = salesdr["estOfficePhone"].ToString();
                            emlEstName = salesdr["estname"].ToString();
                        }                    

                    sql.CommandText = "";

                    sql.CommandText = $"select " +
                        $"cbDies, " +
                        $"cbNaBuild, cbHomeLineSupport, cbCheckFixture, cbBlended, cbShippingToPlant, cbHydroformTooling, cbKitDie, cbFormSteelCoatings, " +
                        $"cbMoldToolingTubeDies, cbLcc, cbSparePunchesButtons, cbEngineeringChange, cbSeeDocumentFromCustomer, cbIncludeEarlyParts, cbAssemblyToolingEquipment," +
                        $"cbIncludeFinanceCost, cbPrototypes, cbTsims, cbTurnkeySeeInternalTsgRfq, cbTransferFingers, cbBundleQuotesYes, txtSendQuotes " +
                        $"from tblRFQ where rfqID = {RFQID}";

                    sql.Parameters.AddWithValue("@cbHydroformTooling", cbHydroformTooling.Checked ? 1 : 0);
                    sql.Parameters.AddWithValue("@cbKitDie", cbKitDie.Checked ? 1 : 0);
                    sql.Parameters.AddWithValue("@cbFormSteelCoatings", cbFormSteelCoatings.Checked ? 1 : 0);

                    using (var reader = sql.ExecuteReader())
                    {
                        reader.Read();
                        cbDies.Checked = (bool)reader["cbDies"];
                        cbNaBuild.Checked = (bool)reader["cbNaBuild"];
                        cbHomeLineSupport.Checked = (bool)reader["cbHomeLineSupport"];
                        cbCheckFixture.Checked = (bool)reader["cbCheckFixture"];
                        cbBlended.Checked = (bool)reader["cbBlended"];
                        cbShippingToPlant.Checked = (bool)reader["cbShippingToPlant"];
                        cbHydroformTooling.Checked = (bool)reader["cbHydroformTooling"];
                        cbKitDie.Checked = (bool)reader["cbKitDie"];
                        cbFormSteelCoatings.Checked = (bool)reader["cbFormSteelCoatings"];

                        cbMoldToolingTubeDies.Checked = (bool)reader["cbMoldToolingTubeDies"];
                        cbSparePunchesButtons.Checked = (bool)reader["cbSparePunchesButtons"];
                        cbLcc.Checked = (bool)reader["cbLcc"];
                        cbEngineeringChange.Checked = (bool)reader["cbEngineeringChange"];
                        cbSeeDocumentFromCustomer.Checked = (bool)reader["cbSeeDocumentFromCustomer"];
                        cbIncludeEarlyParts.Checked = (bool)reader["cbIncludeEarlyParts"];
                        cbAssemblyToolingEquipment.Checked = (bool)reader["cbAssemblyToolingEquipment"];

                        cbIncludeFinanceCost.Checked = (bool)reader["cbIncludeFinanceCost"];
                        cbPrototypes.Checked = (bool)reader["cbPrototypes"];
                        cbTsims.Checked = (bool)reader["cbTsims"];
                        cbTurnkeySeeInternalTsgRfq.Checked = (bool)reader["cbTurnkeySeeInternalTsgRfq"];
                        cbTransferFingers.Checked = (bool)reader["cbTransferFingers"];
                        cbBundleQuotesYes.Checked = (bool)reader["cbBundleQuotesYes"];


                        var temp = reader["txtSendQuotes"];


                        txtSendQuotes.Enabled = cbBundleQuotesYes.Checked;

                        if (temp is string text)
                            txtSendQuotes.Text = text;

                    }



                    connection.Close();

                    //Temp();




                    if (UserCompanyID.Equals(13))
                    {

                        txtMessageText.Text = CusName;

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "Thank you for giving Specialty Tooling Systems(STS) the opportunity to quote the enclosed tooling.If you have any questions regarding the enclosed quotation, please feel free to contact us.";

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "Thank you again for giving us the opportunity.";

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "Sincerely,";

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlSalesmanName;

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlSalesmanEmail;

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlSalesmanPhone;

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlEstName;

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlEstEmail;

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += emlEstPhone;

                        txtMessageText.Text += Environment.NewLine;
                        txtMessageText.Text += " ";
                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "https://ToolingSystemsGroup.com";

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "https://SpecialtyToolingSystems.com";

                        txtMessageText.Text += Environment.NewLine;

                        txtMessageText.Text += "https://vimeo.com/user51659858";












                    }
                    else
                    {
                        txtMessageText.Text = "Thank you for your request for quote. The attached files contain our response.";
                    }
                    //txtMessageText.Text = "Thank you for your request for quote. The attached files contain our response.";
                    txtNoQuoteBody.Text = "Thank you for considering Tooling Systems Group.  After further review it has been determined that we will not be submitting a formal quote to your company on the above mentioned RFQ.  Please feel free to call or e-mail with any questions.";
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message + "<br>" + ex.StackTrace + "<br>" + sql.CommandText;
                    connection.Close();
                }
            }

            if (RFQID == 0)
            {
                rfqNumber.Text = "NEW RFQ";
                if (calDueDate.Text == "")
                    calDueDate.Text = DateTime.Now.AddDays(14).ToString("d");

                if (calReceivedDate.Text == "")
                    calReceivedDate.Text = DateTime.Now.ToString("d");

                if (calIntDueDate.Text == "")
                    calIntDueDate.Text = DateTime.Now.AddDays(14).ToString("d");

                btnImport.Visible = false;
                btnSavePart.Visible = false;
                uploadFile.Visible = false;
                viewAllQuotesButton.Visible = false;
                lblMessage.Text += "\n<script>\n";
                lblMessage.Text += "document.getElementById('addButton').style.display='none';\n";
                lblMessage.Text += "document.getElementById('quoteUploadButton').style.display='none';\n";
                lblMessage.Text += "document.getElementById('removeAllButton').style.display='none';\n";
                lblMessage.Text += "document.getElementById('notificationTR').style.display='none';\n";
                lblMessage.Text += "document.getElementById('nqRemainingPartsDiv').style.display='none';\n";
                lblMessage.Text += "document.getElementById('removeNoQuotesAllPartsDiv').style.display='none';\n";
                lblMessage.Text += "document.getElementById('reserveAllPartsDiv').style.display='none';\n";
                lblMessage.Text += "document.getElementById('quoteSheet').style.display='none';\n";
                lblMessage.Text += "</script>\n";
            }
            else
            {
                if (!IsPostBack)
                {
                    Site master = new RFQ.Site();
                    populate_Header();
                    populate_Parts();
                    if (ddlStatus.SelectedValue != "11" && master.getUserRole() != 1 && master.getUserRole() != 5)
                    {
                        btnImport.Visible = true;
                    }
                }
            }
            litDownloadQuotes.Text = "";

            //if (true)
            //{
            //    txtSendBundledTo.Visible = false;
            //    txtCCBundledTo.Visible = false;
            //    txtBCCBundledTo.Visible = false;

            //}

        }



        public void SaveCheckBoxes(Int64 rfqID)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;




            sql.CommandText = $"update tblRFQ set ";

            if (cbNaBuild.Checked)
                sql.CommandText = $"cbNaBuild = 1 ";
            else
                sql.CommandText = $"cbNaBuild = 0 ";



            sql.CommandText = $"where rfqID = {rfqID}";



            //sql.CommandText = "update tblRFQ set cbLcc = 1 where rfqID = 24868";

            sql.ExecuteNonQuery();

            connection.Close();
        }
        // puts some colors in the list
        // you can just keep adding to this 
        //Added the colors multiple times because repeating wont matter as long as they are not right next to eachother
        // source: http://condor.depaul.edu/sjost/it236/documents/colorNames.htm
        protected void populate_ListOfColors()
        {
            for (int i = 0; i < 10; i++)
            {
                ListOfColors.Add("AntiqueWhite");
                ListOfColors.Add("AquaMarine");
                //ListOfColors.Add("BlueViolet");
                ListOfColors.Add("BurlyWood");
                ListOfColors.Add("Coral");
                //ListOfColors.Add("CornflowerBlue");
                ListOfColors.Add("DarkGray");
                ListOfColors.Add("DarkKhaki");
                ListOfColors.Add("Gainsboro");
                ListOfColors.Add("GreenYellow");
                ListOfColors.Add("Lavender");
                ListOfColors.Add("Chartreuse");
                ListOfColors.Add("Aqua");
                ListOfColors.Add("Beige");
                ListOfColors.Add("Cyan");
                ListOfColors.Add("DarkSeaGreen");
                ListOfColors.Add("DarkSalmon");
                ListOfColors.Add("DodgerBlue");
                ListOfColors.Add("DarkSeaGreen");
                ListOfColors.Add("DarkTurquoise");
                ListOfColors.Add("DeepSkyBlue");
                ListOfColors.Add("Gainsboro");
                ListOfColors.Add("SkyBlue");
            }
        }

        // gets the first color in the list and removes it from the list so the next call gets the next one
        protected string getNextColor()
        {
            string nextColor = "HotPink"; // means no more colors
            if (ListOfColors.Count > 0)
            {
                nextColor = ListOfColors[0];
                ListOfColors.Remove(nextColor);
            }
            else
            {

            }
            return nextColor;
        }

        //Removing all buttons that can modify the rfq to lock a quote
        public void LockControlValues()
        {
            //
            //viewAllQuotesButton.Visible = false;


            //btnImport.Visible = false;
            //btnSave_Click.Visible = false;
            //btnSavePart.Visible = false;
            //uploadFile.Visible = false;
            //deletePartButton.Visible = false;
            lblMessage.Text += "\n<script>\n";
            lblMessage.Text += "$('#MainContent_btnImport').hide();\n";
            lblMessage.Text += "$('#MainContent_btnSave_Click').hide();\n";
            lblMessage.Text += "$('#MainContent_btnSavePart').hide();\n";
            lblMessage.Text += "$('#MainContent_uploadFile').hide();\n";
            lblMessage.Text += "$('#MainContent_deletePartButton').hide();\n";
            lblMessage.Text += "document.getElementById('addButton').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('quoteUploadButton').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('removeAllButton').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('notificationTR').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('nqRemainingPartsDiv').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('removeNoQuotesAllPartsDiv').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('reserveAllPartsDiv').style.visibility ='hidden';\n";
            lblMessage.Text += "document.getElementById('quoteSheet').style.visibility ='hidden';\n";

            lblMessage.Text += "</script>\n";
        }

        protected void unlockRFQ(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "update tblRFQ set rfqStatus = 12, rfqModified = GETDATE(), rfqModifiedBy = @user, rfqUnlocked = GETDATE(), ";
            sql.CommandText += "rfqUnlockedBy = @user where rfqID = @id ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", RFQID);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            master.ExecuteNonQuery(sql, "Edit RFQ");

            connection.Close();

            lblMessage.Text += "\n<script>\n";
            lblMessage.Text += "$('#MainContent_btnSave_Click').show();\n";
            lblMessage.Text += "document.getElementById('addButton').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('quoteUploadButton').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('removeAllButton').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('notificationTR').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('nqRemainingPartsDiv').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('removeNoQuotesAllPartsDiv').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('reserveAllPartsDiv').style.visibility='visible';\n";
            lblMessage.Text += "document.getElementById('quoteSheet').style.visibility='visibile';\n";
            lblMessage.Text += "</script>\n";
            //populate_Header();
        }

        protected void removeAllReservations(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            string company = master.getCompanyId().ToString();
            string user = master.getUserName();
            string uID = master.getUserID().ToString();

            List<string> partIDs = new List<string>();
            List<string> reservedDate = new List<string>();
            sql.CommandText = "Select prcPartID, prcCreated from linkPartReservedToCompany where prcRFQID = @rfq and prcTSGCompanyID = @company and prcPartID not in (Select ptqPartID from linkPartToQuote, tblQuote ";
            sql.CommandText += "where ptqPartID = prcPartID and ptqQuoteID = quoQuoteID and quoTSGCompanyID = prcTSGCompanyID) ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@company", master.getCompanyId());
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                partIDs.Add(dr["prcPartID"].ToString());
                try
                {
                    reservedDate.Add(System.Convert.ToDateTime(dr["prcCreated"].ToString()).ToShortDateString());
                }
                catch
                {
                    reservedDate.Add(DateTime.Now.ToShortDateString());
                }
            }
            dr.Close();

            for (int i = 0; i < partIDs.Count; i++)
            {
                sql.CommandText = "insert into linkPartToUnreserved (ptuPartID, ptuUID, ptuCompanyUnreserved, ptuRereserved, ptuInitialReservedDate, ptuCreated, ptuCreatedBy) ";
                sql.CommandText += "values (@partID, @uid, @company, 0, @date, GETDATE(), @user) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partIDs[i]);
                sql.Parameters.AddWithValue("@uid", uID);
                sql.Parameters.AddWithValue("@company", company);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@date", reservedDate[i]);
                master.ExecuteNonQuery(sql, "Edit RFQ");
            }

            sql.CommandText = "Delete from linkPartReservedToCompany where prcRFQID = @rfq and prcTSGCompanyID = @company and prcPartID not in (Select ptqPartID from linkPartToQuote, tblQuote ";
            sql.CommandText += "where ptqPartID = prcPartID and ptqQuoteID = quoQuoteID and quoTSGCompanyID = prcTSGCompanyID)";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@company", master.getCompanyId());

            master.ExecuteNonQuery(sql, "Edit RFQ");
            connection.Close();
        }

        protected void deleteRFQ(object sender, EventArgs e)
        {
            int rfqID = System.Convert.ToInt32(rfqNumber.Text);
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            List<int> partIDSList = new List<int>();

            sql.CommandText = "Select count(prcPartID), (select count(distinct qtrQuoteID) from linkQuoteToRFQ where qtrRFQID = @rfqID) ";
            sql.CommandText += "from linkPartReservedToCompany where prcRFQID = @rfqID";
            sql.Parameters.AddWithValue("@rfqID", rfqID);
            SqlDataReader dr = sql.ExecuteReader();
            int reservedCount = 0;
            int quoteCount = 0;

            if (dr.Read())
            {
                reservedCount = System.Convert.ToInt32(dr.GetValue(0));
                quoteCount = System.Convert.ToInt32(dr.GetValue(1));
            }
            dr.Close();
            //sql.CommandText = "Select count(qtrQuoteID) from linkQuoteToRFQ where qtrRFQID = @rfqID";
            //dr = sql.ExecuteReader();
            //if (dr.Read())
            //{
            //    quoteCount = System.Convert.ToInt32(dr.GetValue(0));
            //}
            //dr.Close();

            if (reservedCount == 0 && quoteCount == 0)
            {
                sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";

                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    partIDSList.Add(System.Convert.ToInt32(dr.GetValue(0)));
                }
                dr.Close();

                sql.CommandText = "Delete from pktblNotifiedColor where ncoRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                master.ExecuteNonQuery(sql, "DashBoard");


                for (int i = 0; i < partIDSList.Count; i++)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToPartDetail where ppdPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "DashBoard");

                    sql.Parameters.Clear();
                    sql.CommandText = "Select ptrPartToRFQID from linkPartToRFQ where ptrPartID = @rfqID";
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    dr = sql.ExecuteReader();
                    int partToRFQID = 0;
                    if (dr.Read())
                    {
                        partToRFQID = System.Convert.ToInt32(dr.GetValue(0));
                    }
                    dr.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToRFQToRFQChecklist where prrPartToRFQID = @id";
                    sql.Parameters.AddWithValue("@id", partToRFQID);
                    master.ExecuteNonQuery(sql, "DashBoard");

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToRFQ where ptrPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "DashBoard");


                    sql.Parameters.Clear();
                    int ptpID = 0;
                    sql.CommandText = "Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        ptpID = System.Convert.ToInt32(dr.GetValue(0));
                    }

                    dr.Close();
                    sql.Parameters.Clear();

                    sql.CommandText = "Delete from linkPartToHistoricalQuote where phqPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "DashBoard");

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToPart where ptpPartToPartID = @ptpID";
                    sql.Parameters.AddWithValue("@ptpID", ptpID);
                    master.ExecuteNonQuery(sql, "DashBoard");
                    sql.Parameters.Clear();

                    sql.CommandText = "Delete from linkPartToQuotehistory where pqhPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "Dashboard");

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from tblPart where prtPARTID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "Dashboard");
                }
                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkRFQToRFQChecklist where rtcRFQID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                master.ExecuteNonQuery(sql, "Dashboard");

                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkRFQToCompany where rtqRFQID = @rfqID";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                master.ExecuteNonQuery(sql, "DashBoard");
                sql.Parameters.Clear();

                sql.CommandText = "Delete from tblRFQ where rfqID = @rfqID";
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                master.ExecuteNonQuery(sql, "DashBoard");

                Response.Redirect(Request.RawUrl);
            }
            else
            {
                Response.Write("<script>alert('We cannot delete a quote with any reserved parts or quotes associated with it.');</script>");
            }
            connection.Close();
        }

        protected void deleteQuote_click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            if (master.getCompanyId() != 9 && master.getCompanyId() != 13 && master.getCompanyId() != 15)
            {
                List<string> preWordedNotes = new List<string>();
                string quoteID = hdnQuoteToDelete.Value.ToString();
                sql.CommandText = "Select pwqPreWordedNoteID from linkPWNToQuote where pwqQuoteID = @quote";
                sql.Parameters.AddWithValue("@quote", quoteID);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    preWordedNotes.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                sql.Parameters.Clear();

                sql.CommandText = "Delete from linkPWNToQuote where pwqQuoteID = @quote";
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                for (int i = 0; i < preWordedNotes.Count; i++)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                    sql.Parameters.AddWithValue("@id", preWordedNotes[i]);
                    master.ExecuteNonQuery(sql, "editRFQ");
                }
                sql.Parameters.Clear();

                sql.CommandText = "Select diqDieInfoID, quoBlankInfoID from linkDieInfoToQuote, tblQuote where diqQuoteID = @quote and quoQuoteID = @quote";
                sql.Parameters.AddWithValue("@quote", quoteID);
                dr = sql.ExecuteReader();

                int dieInfoID = 0;
                int blankInfoID = 0;
                if (dr.Read())
                {
                    dieInfoID = System.Convert.ToInt32(dr.GetValue(0));
                    blankInfoID = System.Convert.ToInt32(dr.GetValue(1));
                }
                dr.Close();

                sql.Parameters.Clear();

                sql.CommandText = "Delete from linkDieInfoToQuote where diqQuoteID = @quote";
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from pktblBlankInfo where binBlankInfoID = @blankInfo";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@blankInfo", blankInfoID);
                master.ExecuteNonQuery(sql, "editRFQ");

                if (dieInfoID != 0)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from tblDieInfo where dinDieInfoID = @id";
                    sql.Parameters.AddWithValue("@id", dieInfoID);
                    master.ExecuteNonQuery(sql, "editRFQ");
                }

                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quote and gnqHTS = 0";
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from linkQuoteToRFQ where qtrQuoteID = @quote and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0";
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quote and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 0";
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from tblQuote where quoQuoteID = @quote";
                master.ExecuteNonQuery(sql, "editRFQ");

                Response.Redirect(Request.RawUrl);
            }
            else if (master.getCompanyId() == 9)
            {
                List<string> pwn = new List<string>();
                string quoteID = hdnQuoteToDelete.Value.ToString();
                sql.CommandText = "Select hpwHTSPreWordedNoteID from pktblHTSPreWordedNote, linkHTSPWNToHTSQuote ";
                sql.CommandText += "where hpwHTSPreWordedNoteID = pthHTSPWNID and pthHTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    pwn.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Delete from linkHTSPWNToHTSQuote where pthHTSQuoteID = @quoteID ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                for (int i = 0; i < pwn.Count; i++)
                {
                    sql.CommandText = "Delete from pktblHTSPreWordedNote where hpwHTSPreWordedNoteID = @pwn ";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@pwn", pwn[i]);
                    master.ExecuteNonQuery(sql, "Edit RFQ");
                }

                sql.CommandText = "Delete from linkGeneralNoteToQuote where gnqQuoteID = @quote and gnqHTS = 1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from linkQuoteToRFQ where qtrQuoteID = @quote and qtrHTS = 1 and qtrSTS = 0 and qtrUGS = 0";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quote and ptqHTS = 1 and ptqSTS = 0 and ptqUGS = 0";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from tblHTSQuote where hquHTSQuoteID = @quote ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                Response.Redirect(Request.RawUrl);
            }
            else if (master.getCompanyId() == 13)
            {
                String quoteID = hdnQuoteToDelete.Value.ToString();
                List<string> pwnIDs = new List<string>();

                sql.CommandText = "Select psqPreWordedNoteID from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    pwnIDs.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Delete from linkPWNToSTSQuote where psqSTSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");

                sql.CommandText = "delete from linkAssemblyToQuote where atqQuoteId = @quoteID and atqSTS = 1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                for (int i = 0; i < pwnIDs.Count; i++)
                {
                    sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @ID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@ID", pwnIDs[i]);
                    master.ExecuteNonQuery(sql, "STS Edit Quote");
                }

                sql.CommandText = "Delete from linkGeneralNoteToSTSQuote where gnsSTSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");

                sql.CommandText = "Delete from linkQuoteToRFQ where qtrQuoteID = @quote and qtrHTS = 0 and qtrSTS = 1 and qtrUGS = 0";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quote and ptqHTS = 0 and ptqSTS = 1 and ptqUGS = 0";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from tblSTSQuote where squSTSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "STS Edit Quote");

                Response.Redirect(Request.RawUrl);
            }
            else if (master.getCompanyId() == 15)
            {
                string quoteID = hdnQuoteToDelete.Value.ToString();
                List<string> pwnID = new List<string>();
                sql.CommandText = "Select pwnPreWordedNoteID from pktblPreWordedNote, linkPWNToUGSQuote where puqPreWordedNoteID = pwnPreWordedNoteID and puqUGSQuoteID = @id ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    pwnID.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "delete from linkPWNToUGSQuote where puqUGSQuoteID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", quoteID);
                master.ExecuteNonQuery(sql, "UGS Edit Quote");

                for (int i = 0; i < pwnID.Count; i++)
                {
                    sql.CommandText = "delete from pktblPreWordedNote where pwnPreWordedNoteID = @id";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@id", pwnID[i]);
                    master.ExecuteNonQuery(sql, "UGS Edit Quote");
                }

                sql.CommandText = "Delete from linkGeneralNoteToUGSQuote where gnuUGSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from linkQuoteToRFQ where qtrQuoteID = @quote and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "editRFQ");

                sql.CommandText = "Delete from linkPartToQuote where ptqQuoteID = @quote and ptqHTS = 0 and ptqSTS = 0 and ptqUGS = 1 ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quote", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from tblUGSQuote where uquUGSQuoteID = @quoteID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@quoteID", quoteID);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                Response.Redirect(Request.RawUrl);
            }


            connection.Close();
        }

        protected void deleteAllParts_click(object sender, EventArgs e)
        {
            deleteAllParts(System.Convert.ToInt32(this.RFQID));
        }

        protected void deleteAllParts(int rfqID)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "Select count(prcPartID) from linkPartReservedToCompany where prcRFQID = @rfqID";
            sql.Parameters.AddWithValue("@rfqID", rfqID);
            SqlDataReader dr = sql.ExecuteReader();
            int reservedCount = 0;
            int quoteCount = 0;
            int partCount = 0;

            if (dr.Read())
            {
                reservedCount = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();
            sql.CommandText = "Select count(qtrQuoteID) from linkQuoteToRFQ where qtrRFQID = @rfqID";
            dr = sql.ExecuteReader();

            if (dr.Read())
            {
                quoteCount = System.Convert.ToInt32(dr.GetValue(0));
            }

            dr.Close();

            sql.CommandText = "Select Count(*) from linkPartToRFQ where ptrRFQID = @rfqID";
            dr = sql.ExecuteReader();

            if (dr.Read())
            {
                partCount = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();

            if (partCount != 0)
            {
                //if (reservedCount == 0 && quoteCount == 0)
                //{

                sql.Parameters.Clear();
                List<int> partIDSList = new List<int>();
                sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";
                sql.Parameters.AddWithValue("@rfqID", rfqID);

                dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    partIDSList.Add(System.Convert.ToInt32(dr.GetValue(0)));
                }
                dr.Close();

                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkPartReservedToCompany where prcRFQID = @id";
                sql.Parameters.AddWithValue("@id", rfqID);
                master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                sql.Parameters.Clear();
                sql.CommandText = "Delete from tblNoQuote where nquRFQID = @id";
                sql.Parameters.AddWithValue("@id", rfqID);
                master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                int ptpID = 0;

                for (int i = 0; i < partIDSList.Count; i++)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Select ptrPartToRFQID from linkPartToRFQ where ptrPartID = @rfqID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@rfqID", rfqID);
                    dr = sql.ExecuteReader();
                    int partToRFQID = 0;
                    if (dr.Read())
                    {
                        partToRFQID = System.Convert.ToInt32(dr.GetValue(0));
                    }
                    dr.Close();

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToRFQToRFQChecklist where prrPartToRFQID = @id";
                    sql.Parameters.AddWithValue("@id", partToRFQID);
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                    sql.CommandText = "Delete from linkPartToOldNoQuote where onqPartID = @partID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToRFQ where ptrPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");


                    sql.Parameters.Clear();

                    sql.CommandText = "Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    dr = sql.ExecuteReader();
                    if (dr.Read())
                    {
                        ptpID = System.Convert.ToInt32(dr.GetValue(0));
                    }

                    dr.Close();

                    sql.CommandText = "Delete from linkPartToPartDetail where ppdPartID = @partID";
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                    sql.CommandText = "Delete from linkPartToHistoricalQuote where phqPartID = @partID";
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");
                    sql.Parameters.Clear();

                    sql.CommandText = "Delete from linkPartToQuotehistory where pqhPartID = @partID";
                    sql.Parameters.AddWithValue("@partID", partIDSList[i]);
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                    sql.CommandText = "Delete from linkPartReservedToCompany where prcPartId = @partID";
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");

                    sql.CommandText = "Delete from tblPart where prtPARTID = @partID";
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");
                }

                if (ptpID != 0)
                {
                    sql.Parameters.Clear();
                    sql.CommandText = "Delete from linkPartToPart where ptpPartToPartID = @ptpID";
                    sql.Parameters.AddWithValue("ptpID", ptpID);
                    master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");
                }
                Response.Redirect(Request.RawUrl);
                //}
                //else
                //{
                //    Response.Write("<script>alert('We cannot delete all parts because some parts have eithere a reservation or a quote linked to them.');</script>");
                //}
            }
            else
            {
                Response.Write("<script>alert('There are no parts in this quote to delete.');</script>");
            }

            connection.Close();
        }

        protected void btnDownloadCompanyQuotes_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            long company = master.getCompanyId();

            if (company == 9)
            {
                sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS from linkQuoteToRFQ where qtrRFQID = @rfq and qtrHTS=1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 3) + "</Script>";
                }
                dr.Close();
            }
            else if (company == 13)
            {
                sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS from linkQuoteToRFQ where qtrRFQID = @rfq and qtrSTS=1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 4) + "</Script>";
                }
                dr.Close();
            }
            else if (company == 15)
            {
                sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS from linkQuoteToRFQ where qtrRFQID = @rfq and qtrUGS=1";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 5) + "</Script>";
                }
                dr.Close();
            }
            else if (company == 1)
            {
                sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS from linkQuoteToRFQ where qtrRFQID = @rfq and qtrHTS = 0 and qtrSTS = 0 and qtrUGS = 0";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    if (dr.GetBoolean(1))
                    {
                        litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 3) + "</Script>";
                    }
                    else if (dr.GetBoolean(2))
                    {
                        litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 4) + "</Script>";
                    }
                    else if (dr.GetBoolean(3))
                    {
                        litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 5) + "</Script>";
                    }
                    else
                    {
                        litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 2) + "</Script>";
                    }
                }
                dr.Close();
            }
            else
            {
                sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS, qtrUGS from linkQuoteToRFQ, tblQuote where qtrRFQID = @rfq and qtrSTS=0 and qtrHTS=0 and qtrUGS=0 and qtrQuoteID = quoQuoteID and quoTSGCompanyID = @company";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                sql.Parameters.AddWithValue("@company", company);
                SqlDataReader dr = sql.ExecuteReader();

                while (dr.Read())
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 2) + "</Script>";
                }
                dr.Close();
            }

            connection.Close();
        }

        protected void btnDownloadQuotes_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "Select qtrQuoteID, qtrHTS, qtrSTS from linkQuoteToRFQ where qtrRFQID = @rfq";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            SqlDataReader dr = sql.ExecuteReader();

            while (dr.Read())
            {
                if (dr.GetBoolean(1))
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 3) + "</Script>";
                }
                else if (dr.GetBoolean(2))
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 4) + "</Script>";
                }
                else
                {
                    litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", dr.GetValue(0).ToString(), 2) + "</Script>";
                }
            }
            dr.Close();

            //litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", "352", "2") + "</Script>";
            //litDownloadQuotes.Text += "<script>" + String.Format("window.open('https://tsgrfq.azurewebsites.net/CreateQuote?quoteNumber={0}&quoteType={1}&individual=yes','_blank')", "243", "2") + "</Script>";

            connection.Close();
        }


        protected void deletePart_click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            //delete out no quotes or stop it from deleting if there are no quotes

            sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ where ptrRFQID = @rfqID and ptrPartID = prtPARTID and prtPartNumber = @partNum";
            sql.Parameters.AddWithValue("@rfqID", this.RFQID);
            sql.Parameters.AddWithValue("@partNum", txtPart.Text);

            SqlDataReader dr = sql.ExecuteReader();
            int partID = 0;
            if (dr.Read())
            {
                partID = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();

            sql.Parameters.Clear();
            sql.CommandText = "Select count(ptqQuoteID) from linkPartToQuote where ptqPartID = @partID";
            sql.Parameters.AddWithValue("@partID", partID);
            dr = sql.ExecuteReader();

            int quoteCount = 0;
            //int reservedCount = 0;

            if (dr.Read())
            {
                quoteCount = System.Convert.ToInt32(dr.GetValue(0));
            }
            dr.Close();

            //sql.Parameters.Clear();
            //sql.CommandText = "Select count(prcPartID) from linkPartReservedToCompany where prcPartID = @partID";
            //sql.Parameters.AddWithValue("@partID", partID);
            //dr = sql.ExecuteReader();

            //if (dr.Read())
            //{
            //    reservedCount = System.Convert.ToInt32(dr.GetValue(0));
            //}
            //dr.Close();

            sql.Parameters.Clear();
            sql.CommandText = "Delete from linkPartReservedToCompany where prcPartID = @id";
            sql.Parameters.AddWithValue("@id", partID);
            master.ExecuteNonQuery(sql, "EditRFQ DeleteAllParts");


            //if (quoteCount == 0 && reservedCount == 0)
            if (quoteCount == 0)
            {
                sql.CommandText = "Select ptrPartToRFQID from linkPartToRFQ where ptrPartID = @rfqID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfqID", this.RFQID);
                dr = sql.ExecuteReader();
                int partToRFQID = 0;
                if (dr.Read())
                {
                    partToRFQID = System.Convert.ToInt32(dr.GetValue(0));
                }
                dr.Close();


                sql.CommandText = "Delete from tblNoQuote where nquPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.CommandText = "Delete from linkPartToRFQToRFQChecklist where prrPartToRFQID = @id";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@id", partToRFQID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.CommandText = "Delete from linkPartToRFQ where ptrPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.Parameters.Clear();
                int ptpID = 0;
                sql.CommandText = "Select ppdPartToPartID from linkPartToPartDetail where ppdPartID = @partID";
                sql.Parameters.AddWithValue("@partID", partID);
                dr = sql.ExecuteReader();
                if (dr.Read())
                {
                    ptpID = System.Convert.ToInt32(dr.GetValue(0));
                }
                dr.Close();

                sql.CommandText = "Delete from linkPartToPartDetail where ppdPartToPartID = @ptpID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@ptpID", ptpID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.CommandText = "Delete from linkPartToPart where ptpPartToPartID = @ptpID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("ptpID", ptpID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkPartToHistoricalQuote where phqPartID = @partID";
                sql.Parameters.AddWithValue("@partID", partID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.Parameters.Clear();
                sql.CommandText = "Delete from linkPartToQuotehistory where pqhPartID = @partID";
                sql.Parameters.AddWithValue("@partID", partID);
                master.ExecuteNonQuery(sql, "EditRFQ");

                sql.CommandText = "Delete from tblPart where prtPARTID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                master.ExecuteNonQuery(sql, "EditRFQ");
            }
            else
            {
                //Response.Write("<script>alert('We cannot delete a part that is reserved or has a quote associated with it.');</script>");
                Response.Write("<script>alert('We cannot delete a part that has a quote associated with it.');</script>");
            }

            sql.Parameters.Clear();
            sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
            sql.Parameters.AddWithValue("@rfq", this.RFQID);

            master.ExecuteNonQuery(sql, "EditRFQ");

            Response.Redirect(Request.RawUrl);

            connection.Close();
        }

        //[WebMethod]
        [System.Web.Script.Services.ScriptMethod()]
        [System.Web.Services.WebMethod]
        public static string[] GetCustomers(string prefix, string rfq)
        {
            string customerid = "";
            List<string> customers = new List<string>();
            Site master = new RFQ.Site();

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "select rfqcustomerid from tblrfq where rfqid = @rfqid ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqid", rfq);
            SqlDataReader dr = sql.ExecuteReader();
            if (dr.Read())
            {
                customerid = dr.GetValue(0).ToString();
            }
            dr.Close();

            List<string> Emails = new List<string>();
            sql.CommandText = "select cccEmail from CustomCustomerContact where cccCreatedBy = @user and cccEmail Like @SearchText2 + '%'";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@SearchText2", prefix);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                customers.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            sql.CommandText = "select Email from CustomerContact where CustomerID = @customerID and Email Like @SearchText + '%'";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customerID", customerid);
            sql.Parameters.AddWithValue("@SearchText", prefix);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                customers.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            return customers.ToArray();
        }


        protected void populate_Header()
        {
            String CustomerID = "";
            rfqNumber.Text = RFQID.ToString().Trim();
            Site master = new RFQ.Site();

            if (ddlStatus.SelectedValue == "11" && master.getUserRole() != 1 && master.getUserRole() != 5)
            {
                btnImport.Visible = false;
            }
            else
            {
                btnImport.Visible = true;
            }

            DateTime dateRecieved = DateTime.Now;

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select rfqStatus, rfqCustomerID, rfqPlantID, rfqCustomerRFQNumber, rfqProgramID, rfqOEMID, rfqVehicleID, ";
            sql.CommandText += "rfqDueDate, rfqDateReceived, rfqBidDate, rfqEstimatedPODate, rfqPaymentTermsID, rfqShippingTermsID, ";
            sql.CommandText += "rfqToolCountryID, rfqEngineeringNumber, rfqProductTypeID, rfqNumberofParts, rfqNotes, rfqMeetingNotes, ";
            sql.CommandText += "rfqCreated, rfqCreatedBy, rfqModified, rfqModifiedBy, rfqPostedDate, rfqLiveWork, ";
            sql.CommandText += "rfqSourceID, rfqAdditionalSourceID, rfqInternalDueDate, rfqUseTSGLogo, rfqTurnkey, rfqGlobalProgram, rfqCustomerContact, ";
            sql.CommandText += "rfqATSReady, rfqBTSReady, rfqDTSReady, rfqETSReady, rfqGTSReady, rfqHTSReady, rfqRTSReady, rfqSTSReady, rfqUGSReady, rfqSendTo, rfqCCTo, rfqBCCTo ";
            sql.CommandText += "from tblRFQ ";
            sql.CommandText += "where rfqID=@rfq";
            sql.Parameters.AddWithValue("@rfq", RFQID);
            string cust = "";
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                ddlStatus.SelectedValue = dr.GetValue(0).ToString();
                if (ddlStatus.SelectedValue == "11" && master.getUserRole() != 1 && master.getUserRole() != 5)
                {
                    LockControlValues();
                }
                if (ddlStatus.SelectedValue.ToString() == "11")
                {
                    btnUnlockRFQ.Visible = true;
                }
                else
                {
                    btnUnlockRFQ.Visible = false;
                }
                ddlCustomer.SelectedValue = dr.GetValue(1).ToString();
                CustomerID = dr.GetValue(1).ToString();
                cust = dr.GetValue(1).ToString();
                txtCustomerRFQ.Text = dr.GetValue(3).ToString();
                txtCusRfq.Text = dr.GetValue(3).ToString();
                ddlProgram.SelectedValue = dr.GetValue(4).ToString();
                ddlOEM.SelectedValue = dr.GetValue(5).ToString();
                ddlVehicle.SelectedValue = dr.GetValue(6).ToString();
                try
                {
                    calDueDate.Text = System.Convert.ToDateTime(dr.GetValue(7)).ToString("d");
                }
                catch
                {

                }
                try
                {
                    calReceivedDate.Text = System.Convert.ToDateTime(dr.GetValue(8)).ToString("d");
                    dateRecieved = System.Convert.ToDateTime(dr.GetValue(8));
                }
                catch
                {

                }
                try
                {
                    if (System.Convert.ToDateTime(dr.GetValue(9)).ToString("d").Equals("1/1/1900"))
                    {
                        calBidDate.Text = "";
                    }
                    else
                    {
                        calBidDate.Text = System.Convert.ToDateTime(dr.GetValue(9)).ToString("d");
                    }
                }
                catch
                {
                    calBidDate.Text = "";
                }
                try
                {
                    if (System.Convert.ToDateTime(dr.GetValue(10)).ToString("d").Equals("1/1/1900"))
                    {
                        calPODate.Text = "";
                    }
                    else
                    {
                        calPODate.Text = System.Convert.ToDateTime(dr.GetValue(10)).ToString("d");
                    }
                }
                catch
                {
                    calPODate.Text = "";
                }
                ddlToolCountry.SelectedValue = dr.GetValue(13).ToString();
                txtEngineeringNumber.Text = dr.GetValue(14).ToString();
                ddlProductType.SelectedValue = dr.GetValue(15).ToString();
                // calculate num    ber of parts
                txtNotes.Text = dr.GetValue(17).ToString();
                //txtMeetingNotes.Text = dr.GetValue(18).ToString();
                create.Text = dr.GetValue(20).ToString() + " " + TimeZoneInfo.ConvertTimeFromUtc(System.Convert.ToDateTime(dr.GetValue(19)), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                try
                {
                    modify.Text = dr.GetValue(22).ToString() + " " + TimeZoneInfo.ConvertTimeFromUtc(System.Convert.ToDateTime(dr.GetValue(21)), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                }
                catch
                {

                }
                cbLiveWork.Checked = dr.GetBoolean(24);
                try
                {
                    ddlRFQSource.SelectedValue = dr.GetValue(25).ToString();
                }
                catch
                {

                }
                try
                {
                    ddlRFQSource2.SelectedValue = dr.GetValue(26).ToString();
                }
                catch
                {

                }
                //try
                //{
                //    lblIntDueDate.Text = System.Convert.ToDateTime(dr.GetValue(27)).ToString("d");
                //}
                //catch
                //{

                //}
                try
                {
                    calIntDueDate.Text = System.Convert.ToDateTime(dr.GetValue(27)).ToString("d");
                }
                catch
                {

                }
                cbUseTSGLogo.Checked = dr.GetBoolean(28);
                cbTurnkey.Checked = dr.GetBoolean(29);
                cbGlobalProgram.Checked = dr.GetBoolean(30);
                //ddlCustomerContact.SelectedValue = dr.GetValue(31).ToString();
                //cbATSReady.Checked = dr.GetBoolean(32);
                //cbBTSReady.Checked = dr.GetBoolean(33);
                //cbDTSReady.Checked = dr.GetBoolean(34);
                //cbETSReady.Checked = dr.GetBoolean(35);
                //cbGTSReady.Checked = dr.GetBoolean(36);
                //cbHTSReady.Checked = dr.GetBoolean(37);
                //cbRTSReady.Checked = dr.GetBoolean(38);
                //cbSTSReady.Checked = dr.GetBoolean(39);
                //cbUGSReady.Checked = dr.GetBoolean(40);
                //txtSendBundledTo.Text = dr.GetValue(41).ToString();
                //txtCCBundledTo.Text = dr.GetValue(42).ToString();
                //txtBCCBundledTo.Text = dr.GetValue(43).ToString();
            }
            dr.Close();


            //This is to re load the original customer list when the Customer is inactive
            if (ddlCustomer.SelectedValue != cust && cust != "")
            {
                sql.CommandText = "select CustomerID, concat(CustomerName,' (', CustomerNumber, ')') as Name from Customer ";
                sql.CommandText += "order by CustomerName";

                SqlDataReader CustomerDR = sql.ExecuteReader();
                ddlCustomer.DataSource = CustomerDR;
                ddlCustomer.DataTextField = "Name";
                ddlCustomer.DataValueField = "CustomerID";
                ddlCustomer.DataBind();
                ddlCustomer.Items.Insert(0, "Please Select");
                ddlCustomer.SelectedValue = cust;
                CustomerDR.Close();
                populate_Plants();
            }


            sql.CommandText = "Select cnoCreatedBy, cnoCreated, TSGCompanyAbbrev from tblCompanyNotified, TSGCompany where cnoRFQID = @id and cnoTSGCompanyID <> 1 and cnoTSGCompanyID = TSGCompanyID order by cnoID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@id", RFQID);
            dr = sql.ExecuteReader();
            int count = 0;
            while (dr.Read())
            {
                if (count == 0)
                {
                    lblNotified.Text = "Notifications sent: " + TimeZoneInfo.ConvertTimeFromUtc(System.Convert.ToDateTime(dr.GetValue(1)), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) +
                        " by: " + dr.GetValue(0).ToString() + " to " + dr.GetValue(2).ToString();
                }
                else
                {
                    lblNotified.Text += ", " + dr.GetValue(2);
                }
                count++;
            }
            dr.Close();

            sql.CommandText = "select nqrNoQuoteReasonID, nqrNoQuoteReason, nqrNoQuoteReasonNumber from pktblNoQuoteReason where nqrActive = 1 order by nqrNoQuoteReasonNumber";
            sql.Parameters.Clear();
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                ddlNoQuoteReason.Items.Add(new System.Web.UI.WebControls.ListItem((dr.GetValue(2).ToString() + " - " + dr.GetValue(1).ToString()), dr.GetValue(0).ToString()));
            }
            dr.Close();

            if (RFQID > 0)
            {
                populate_Plants();
                setSalesmanAndRank();
            }

            String CustomerName = "0";
            sql.CommandText = "select CustomerName, CustomerContactID from customer, CustomerContact where customer.customerId = @customer and customer.CustomerID = CustomerContact.CustomerID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", CustomerID);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                CustomerName = HttpUtility.HtmlEncode(dr.GetValue(0).ToString());
                //ddlCustomerContact.SelectedValue = dr.GetValue(1).ToString();
            }
            dr.Close();

            sql.CommandText = "Select Email from CustomerContact where CustomerContactID = @contact";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@contact", ddlCustomerContact.SelectedValue);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                txtExtraEmail.Text = dr.GetValue(0).ToString();
                txtNoQuoteTo.Text = dr["Email"].ToString();
            }
            dr.Close();



            sql.CommandText = "select TSGCompanyAbbrev, TSGCompanyID, rtqCompanyID from tsgCompany left outer join linkRFQToCompany on tsgCompany.TSGCompanyID=rtqCompanyID and rtqRFQID=@rfq where tsgCompanyAbbrev not in ('none','TSG') order by tsgCompanyAbbrev";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            dr = sql.ExecuteReader();
            lblNotificationCheckList.Text = "Group <input type=checkbox value=1 name=group onclick='setNotifyGroup(this.checked);'>&nbsp;&nbsp;";
            lblNotificationCheckList.Text += "Group+Fixture <input type=checkbox value=1 name=groupfixture onclick='setNotifyGroupFixture(this.checked);' >&nbsp;&nbsp;";
            lblReSendNotificationsScript.Text = "\n<script>\nfunction reSendNotifications(){\n";
            lblSendNotificationsScript.Text = "\n<script>\nfunction sendNotifications() {\n";
            lblSendNotificationsScript.Text += "    document.getElementById('sendNotificationsMessage').innerHTML='Sending...';\n";
            lblReSendNotificationsScript.Text += "    document.getElementById('sendNotificationsMessage').innerHTML='Sending...';\n";
            lblSendNotificationsScript.Text += " var colist='1';\n";
            lblReSendNotificationsScript.Text += " var colist='1';\n";
            while (dr.Read())
            {
                lblNotificationCheckList.Text += dr.GetValue(0).ToString();
                lblNotificationCheckList.Text += " <input type=checkbox id='notify" + dr.GetValue(0).ToString() + "' value='" + dr.GetValue(1).ToString() + "' ";
                //if (dr.GetValue(2).ToString() != "")
                //{
                //    lblNotificationCheckList.Text += " checked='checked'  ";
                //}
                //lblSendNotificationsScript.Text += "    if (! document.getElementById('notify" + dr.GetValue(0).ToString() + "').checked) {\n";
                //lblSendNotificationsScript.Text += "    }\n";


                if (dr.GetValue(2).ToString() != "")
                {
                    lblNotificationCheckList.Text += " checked='checked'  ";
                    lblSendNotificationsScript.Text += "    if (! document.getElementById('notify" + dr.GetValue(0).ToString() + "').checked) {\n";
                    lblSendNotificationsScript.Text += "    }\n";
                }
                else
                {
                    lblSendNotificationsScript.Text += "    if (document.getElementById('notify" + dr.GetValue(0).ToString() + "').checked) {\n";
                    lblSendNotificationsScript.Text += "        colist=colist + ',' + '" + dr.GetValue(1).ToString() + "';\n";
                    lblSendNotificationsScript.Text += "    }\n";
                }
                lblReSendNotificationsScript.Text += "    if (document.getElementById('notify" + dr.GetValue(0).ToString() + "').checked) {\n";
                lblReSendNotificationsScript.Text += "        colist=colist + ',' + '" + dr.GetValue(1).ToString() + "';\n";
                lblReSendNotificationsScript.Text += "    }\n";
                // This will open up the STS dialog for the data cordinator to enter extra info for STS
                if (dr["TSGCompanyID"].ToString() == "13" || dr["TSGCompanyID"].ToString() == "20")
                {
                    lblNotificationCheckList.Text += "  onclick=\"removeNotification('" + dr.GetValue(1).ToString() + "','" + RFQID + "');populateSTSRFQDialog();\" \n";
                }
                else
                {
                    lblNotificationCheckList.Text += "  onclick=\"removeNotification('" + dr.GetValue(1).ToString() + "','" + RFQID + "');\" \n";
                }
                lblNotificationCheckList.Text += ">&nbsp;&nbsp;";
            }
            dr.Close();
            // be sure to send to company TSG too
            lblSendNotificationsScript.Text += "    sendNotification(colist,'" + RFQID + "','1');\n";
            lblSendNotificationsScript.Text += "    document.getElementById('sendNotificationsMessage').innerHTML='Notifications Sent';\n";
            lblSendNotificationsScript.Text += "}\n</script>\n";
            lblReSendNotificationsScript.Text += "    sendNotification(colist,'" + RFQID + "','1');\n";
            lblReSendNotificationsScript.Text += "    document.getElementById('sendNotificationsMessage').innerHTML='Notifications Sent';\n";
            lblReSendNotificationsScript.Text += "}\n</script>\n";


            //CustomerName = CustomerName.Replace(' ', '%');
            String RFQdate = HttpUtility.HtmlEncode(calReceivedDate.Text.Replace('/', '-'));
            string tempCustName = CustomerName;
            //if (CustomerName != "Challenge Mfg. Company")
            //{
            //    tempCustName = CustomerName.Replace(".", "");
            //}

            tempCustName = tempCustName.TrimEnd('.');

            ClientContext clientcontext = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared%20Documents/RFQ%20Data/" + tempCustName.Trim() + "/" + RFQdate + " " + txtCustomerRFQ.Text.Trim());


            ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
            ctx.Credentials = master.getSharePointCredentials();
            Web web = ctx.Web;
            // if this does not exist we will get an error 
            var mainfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data");
            ctx.Load(web);
            //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            var customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + tempCustName.Trim());
            ctx.Load(web);
            try
            {
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
            catch
            {
                // assume need to create
                //lblMessage.Text += "Need to create customer folder";
                mainfolder.Folders.Add(CustomerName);
                ctx.Credentials = master.getSharePointCredentials();
                ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + tempCustName.Trim());
                ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }
            var sharepointUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared%20Documents/RFQ%20Data/" + tempCustName.Trim() + "/" + dateRecieved.ToString("MM-dd-yyyy") + " " + txtCustomerRFQ.Text.Trim();
            if (!CheckSharePointFolder(CustomerName, dateRecieved.ToString("MM-dd-yyyy")))
            {
                sharepointUrl = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + tempCustName.Trim() + "/" + dateRecieved.ToString("yyyy-MM-dd") + " " + txtCustomerRFQ.Text.Trim();
                var rfqfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + tempCustName.Trim() + "/" + dateRecieved.ToString("yyyy-MM-dd") + " " + txtCustomerRFQ.Text.Trim());
                ctx.Load(web);
                try
                {
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                }
                catch
                {
                    // assume need to create
                    //lblMessage.Text += "Need to create rfq folder";
                    ctx.Credentials = master.getSharePointCredentials();
                    customerfolder.Folders.Add(dateRecieved.ToString("yyyy-MM-dd") + " " + txtCustomerRFQ.Text.Trim());
                    ctx.Load(web);
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    ctx.Credentials = master.getSharePointCredentials();
                    rfqfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + CustomerName + "/" + dateRecieved.ToString("yyyy-MM-dd") + " " + txtCustomerRFQ.Text.Trim());
                    ctx.Load(web);
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                }

            }


            hlRFQLink.NavigateUrl = sharepointUrl;
            hlRFQLink.Text = hlRFQLink.NavigateUrl;

            clientcontext = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + RFQID);

            hlquoteAttachment.NavigateUrl = clientcontext.Url;
            hlquoteAttachment.Text = "<font color='blue'>" + clientcontext.Url + "</font>";

            //Create folder to hold the email attachments
            ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/");
            ctx.Credentials = master.getSharePointCredentials();
            web = ctx.Web;
            // if this does not exist we will get an error 
            mainfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/");
            ctx.Load(web);
            //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + RFQID);
            ctx.Load(web);
            ctx.Load(customerfolder);
            ctx.Load(customerfolder.Files);
            try
            {
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                foreach (var file in customerfolder.Files)
                {
                    if (lblUploadsToEmail.Text != "")
                    {
                        lblUploadsToEmail.Text += ", ";
                    }
                    lblUploadsToEmail.Text += file.Name;
                }
                if (!string.IsNullOrWhiteSpace(lblUploadsToEmail.Text))
                {
                    lblUploadsToEmail.Text = "These files will be emailed along with the quotes\n" + lblUploadsToEmail.Text;
                }
            }
            catch
            {
                // assume need to create
                //lblMessage.Text += "Need to create customer folder";
                mainfolder.Folders.Add(RFQID.ToString());
                ctx.Credentials = master.getSharePointCredentials();
                ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + RFQID);
                ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
            }

            string retval = "checklist.png";
            sql.CommandText = "select rtcRFQCheckListID from  linkRFQToRFQCheckList where rtcRFQID=@rfq ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            SqlDataReader rdr = sql.ExecuteReader();
            while (rdr.Read())
            {
                retval = "issues.png";
            }
            rdr.Close();
            connection.Close();
            lblMessage.Text += "\n<script>document.getElementById('rfqcl').src='" + retval + "';</script>\n";

            txtCustomerRFQ.Focus();
        }


        private Boolean CheckSharePointFolder(string CustomerName, string RFQdate)
        {
            Site master = new Site();
            ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
            ctx.Credentials = master.getSharePointCredentials();
            Web web = ctx.Web;


            var rfqfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Shared Documents/RFQ Data/" + CustomerName + "/" + RFQdate + " " + txtCustomerRFQ.Text.Trim());
            ctx.Load(web);
            try
            {
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected void sendUpdateNotification(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            string companies = "";
            int count = 0;

            sql.CommandText = "Select distinct rtqCompanyID from linkRFQToCompany where rtqRFQID = @rfqID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqID", RFQID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (count == 0)
                {
                    companies = dr.GetValue(0).ToString();
                }
                else
                {
                    companies += ", " + dr.GetValue(0).ToString();
                }
                count++;
            }
            dr.Close();

            string notificationID = "";
            sql.CommandText = "select nreNotificationReasonID from pktblNotificationReason where nreNotificationReason='RFQ Updated'";
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                notificationID = dr.GetValue(0).ToString();
            }
            dr.Close();

            RFQ.Models.Notification notification = new Models.Notification();
            notification.SendNotifications(companies, RFQID.ToString(), notificationID, master.getUserName());

            connection.Close();
        }

        protected void populate_Plants()
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select CustomerLocationID, Concat(ShipToName, ' (',ShipCode,')', ' - ', Address1,', ', City,', ', State) as Location from CustomerLocation where CustomerID=@customer  order by Location";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            SqlDataReader plantDR = sql.ExecuteReader();
            ddlPlant.DataSource = plantDR;
            ddlPlant.DataTextField = "Location";
            ddlPlant.DataValueField = "CustomerLocationID";
            ddlPlant.DataBind();
            plantDR.Close();
            //ddlPlant.SelectedValue = "0";

            string customerContactId = "";

            sql.CommandText = "Select rfqCustomerContact from tblRFQ where rfqID = @rfq";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            SqlDataReader custconDR = sql.ExecuteReader();
            if (custconDR.Read())
            {
                try
                {
                    customerContactId = custconDR.GetValue(0).ToString();
                }
                catch
                {

                }
            }
            custconDR.Close();

            sql.CommandText = "Select CustomerContactID, Name from CustomerContact where CustomerID = @customer and (ccoInactive = 0 or ccoInactive is null or CustomerContactID = @id) order by Name";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
            sql.Parameters.AddWithValue("@id", customerContactId);
            SqlDataReader ccDR = sql.ExecuteReader();
            ddlCustomerContact.DataSource = ccDR;
            ddlCustomerContact.DataTextField = "Name";
            ddlCustomerContact.DataValueField = "CustomerContactID";
            ddlCustomerContact.DataBind();
            ccDR.Close();

            try
            {
                ddlCustomerContact.SelectedValue = customerContactId;
            }
            catch
            {

            }


            sql.CommandText = "select rfqPlantID ";
            sql.CommandText += " from tblRFQ ";
            sql.CommandText += " where rfqID=@rfq";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                try
                {
                    ddlPlant.SelectedValue = dr.GetValue(0).ToString();
                }
                catch
                {

                }
            }
            dr.Close();
            connection.Close();
            setSalesmanAndRank();
        }

        protected void populate_Parts()
        {
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse( CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //var accnt = CloudStorageAccount.Parse("SVfDaejVhmhV9bFsTDRo/2skBaftLz4rBShLfXGZlK30072aN+s7wj2140RaSFTF4s28y/PJ9Dko9RCRZHRJbw==");
            //var client = accnt.CreateCloudBlobClient();
            //var blobs = client.GetContainerReference("partpictureblob").ListBlobs();
            //var urls

            List<RFQPart> partList = new List<RFQPart>();
            List<RFQPart> orderedList = new List<RFQPart>();
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            int count = 0;
            string assemblyId = "";

            string assId = "";

            sql.CommandText = "Select a.assNumber, a.assDescription, a.assPicture, at.astAssemblyType, a.assAssemblyId, p.prtRFQLineNumber, p.prtPartNumber, assNotes ";
            sql.CommandText += "from linkAssemblyToRFQ atr ";
            sql.CommandText += "inner join tblAssembly a on a.assAssemblyId = atr.atrAssemblyId ";
            sql.CommandText += "inner join pktblAssemblyType at on at.astAssemblyTypeId = a.assType ";
            sql.CommandText += "inner join linkAssemblyToPart atp on atp.atpAssemblyId = a.assAssemblyId ";
            sql.CommandText += "inner join tblPart p on p.prtPARTID = atp.atpPartId ";
            sql.CommandText += "where atr.atrRfqId = @rfqId ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqId", RFQID);
            SqlDataReader dr = sql.ExecuteReader();
            RFQPart newPart = new RFQPart();
            while (dr.Read())
            {
                if (assId != dr["assAssemblyId"].ToString())
                {
                    assId = dr["assAssemblyId"].ToString();
                    newPart = new RFQPart();
                    newPart.prtPartNumber = dr["assNumber"].ToString() + "\n\nLinked Parts\n" + dr["prtRFQLineNumber"].ToString() + ": " + dr["prtPartNumber"].ToString();
                    newPart.prtPartDescription = dr["assDescription"].ToString();
                    if (dr["assPicture"].ToString() != "")
                    {
                        newPart.prtPicture = dr["assPicture"].ToString();
                    }
                    else
                    {
                        newPart.prtPicture = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/RFQ358_22_51326-45010.png";
                    }
                    newPart.prtNote = dr["assNotes"].ToString();
                    newPart.MimeType = "image/png";
                    newPart.ptyPartDescription = dr["astAssemblyType"].ToString();
                    newPart.LineNumber = "A" + count.ToString();
                    newPart.PartId = "A" + dr["assAssemblyId"].ToString();
                    orderedList.Add(newPart);
                    if (count > 0)
                    {
                        assemblyId += ", '" + newPart.PartId + "'";
                    }
                    else
                    {
                        assemblyId += "<script>assemblyIds = ['" + newPart.PartId + "'";
                    }
                    count++;
                }
                else
                {
                    newPart.prtPartNumber += "\n" + dr["prtRFQLineNumber"].ToString() + ": " + dr["prtPartNumber"].ToString();
                }
            }
            dr.Close();

            hdnNextAssemblyNum.Value = count.ToString();
            if (assemblyId != "")
            {
                litLastAssemblyId.Text = assemblyId + "];</script>";
            }


            sql.CommandText = "select prtPartNumber, prtPartDescription, prtPicture, prtPartLength, prtPartWidth, prtPartHeight, m1.mtyMaterialType, prtPartWeight, prtPartThickness, ptyPartTypeDescription, m1.mtyMaterialType, ";
            sql.CommandText += "binMaterialThicknessEnglish, coalesce(ppdPartToPartID, 0) as linkPart, prtPartID, prtRFQLineNumber, prtNote, m2.mtyMaterialType, prtAnnualVolume ";
            sql.CommandText += "from linkPartToRFQ, tblPart ";
            sql.CommandText += "left outer join pktblPartType on prtPartTypeID = ptyPartTypeID ";
            sql.CommandText += "left outer join pktblBlankInfo on prtBlankInfoID = binBlankInfoID ";
            sql.CommandText += "left outer join pktblMaterialType as m1 on binBlankMaterialTypeID = m1.mtyMaterialTypeID ";
            sql.CommandText += "left outer join linkPartToPartDetail on ppdPartID = prtPartID ";
            sql.CommandText += "left outer join pktblMaterialType as m2 on m2.mtyMaterialTypeID = prtPartMaterialType ";
            sql.CommandText += "where ptrRFQID = @rfq and ptrPartID = prtPartID order by prtRFQLineNumber ASC, ptrPartToRFQID ";
            sql.Parameters.AddWithValue("@rfq", RFQID);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                newPart = new RFQPart();
                newPart.prtPartNumber = dr.GetValue(0).ToString();
                newPart.prtPartDescription = dr.GetValue(1).ToString();

                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "");
                GuidString = GuidString.Replace("+", "");

                newPart.prtPicture = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/" + dr.GetValue(2).ToString() + "?r=" + GuidString;
                newPart.Length = System.Convert.ToDouble(dr.GetValue(3));
                newPart.Width = System.Convert.ToDouble(dr.GetValue(4));
                newPart.Height = System.Convert.ToDouble(dr.GetValue(5));
                if (dr.GetValue(6).ToString() == "")
                {
                    newPart.MaterialType = dr.GetValue(16).ToString();
                }
                else
                {
                    newPart.MaterialType = dr.GetValue(6).ToString();
                }
                newPart.Weight = System.Convert.ToDouble(dr.GetValue(7));
                newPart.MaterialThickness = System.Convert.ToDouble(dr.GetValue(8));
                newPart.ptyPartDescription = dr.GetValue(9).ToString();
                newPart.BlankDescription = dr.GetValue(10).ToString() + " - " + dr.GetValue(11).ToString();
                newPart.LinkPart = dr.GetValue(12).ToString();
                newPart.BackGroundColor = "White";
                newPart.MimeType = "image/png";
                newPart.NQRHTML = "";
                newPart.PartId = dr.GetValue(13).ToString();
                litPartScripts.Text += "<script>url = 'GetHistory.aspx?create=&search=&part=" + newPart.prtPartNumber + "&partID=" + dr.GetValue(13).ToString() + "&rfq=" + RFQID + "&rand=' + Math.random();$.ajax({ url: url, success: function (data) { parseResults(data, 0, '" + newPart.prtPartNumber + "'); } });</script>\n";
                newPart.LineNumber = dr.GetValue(14).ToString();
                newPart.prtNote = dr.GetValue(15).ToString();
                newPart.annualVolume = System.Convert.ToInt32(dr["prtAnnualVolume"].ToString() == "" ? "0" : dr["prtAnnualVolume"].ToString());
                partList.Add(newPart);
            }
            dr.Close();
            List<LinkList> links = new List<LinkList>();

            //This logic is to try and order everything by the line number unless it is linked then we move the other part up to it
            //This is different than just sticking all linked parts up top
            foreach (RFQPart thispart in partList)
            {
                if (!orderedList.Contains(thispart))
                {
                    orderedList.Add(thispart);

                    if (thispart.LinkPart != "")
                    {
                        for (int i = partList.Count - 1; i > -1; i--)
                        {
                            if (thispart.LinkPart == "0")
                            {
                                break;
                            }
                            if (partList[i] != thispart && !orderedList.Contains(partList[i]) && partList[i].LinkPart == thispart.LinkPart)
                            {
                                orderedList.Add(partList[i]);
                            }
                        }
                    }
                }
                else
                {

                }
            }

            master.setGlobalVariables();

            int partChecklist = 0;
            sql.CommandText = "select Count(prrRFQCheckListID) from tblPart, linkPartToRFQ, linkPartToRFQToRFQCheckList where prtPartID=ptrPartId and ptrRFQID=180 and ptrPartToRFQID = prrPartToRFQID";
            SqlDataReader tempDR = sql.ExecuteReader();
            while (tempDR.Read())
            {
                partChecklist = System.Convert.ToInt32(tempDR.GetValue(0).ToString());
            }
            tempDR.Close();

            foreach (RFQPart thispart in orderedList)
            {
                // get check list status for each part
                string retval = "checklist.png";
                if (partChecklist != 0)
                {
                    sql.CommandText = "select prrRFQCheckListID from tblPart, linkPartToRFQ, linkPartToRFQToRFQCheckList where prtPartNumber=@part and prtPartID=ptrPartId and ptrRFQID=@rfq and ptrPartToRFQID = prrPartToRFQID";
                    sql.Parameters.Clear();
                    sql.Parameters.AddWithValue("@part", thispart.prtPartNumber);
                    sql.Parameters.AddWithValue("@rfq", RFQID);
                    SqlDataReader rdr = sql.ExecuteReader();
                    while (rdr.Read())
                    {
                        retval = "issues.png";
                    }
                    rdr.Close();
                }


                thispart.quotingHTML = "<div id='quoting" + thispart.PartId.ToString() + "'>";

                if (!thispart.PartId.Contains("A"))
                {
                    thispart.linkpartsHTML = "<input type='button' class='mybutton' value='Link Parts' onClick=\"showLinkPart('" + thispart.PartId + "');return false;\" >";
                    thispart.checklistHTML = "<a href=\"javascript:showCheckList('" + thispart.PartId + "');\"><img src='" + retval + "' id='clImage" + thispart.prtPartNumber + "' width='50'></a>";
                }
                // todo
                // if rfq matches and part is null, then this No Quote applies to ALL parts
                //sql.CommandText = "select ";
                //sql.CommandText += " from linkPartToRFQ, tblPart ";
                //sql.CommandText += " left outer join linkEstimatingInfo on einRFQID=ptrRFQID and einPartID is null";
                //sql.CommandText += " where ptrRFQID=@rfq and ptrPartID=prtPartID and prtPartNumber=@part order by ptrPartToRFQID";
                //sql.Parameters.AddWithValue("@rfq", RFQID);
                //sql.Parameters.AddWithValue("@part", thispart.prtPartNumber);

                // get color if already set, otherwise get new one
                foreach (LinkList link in links)
                {
                    if (link.LinkID == thispart.LinkPart)
                    {
                        thispart.BackGroundColor = link.LinkColor;
                    }
                }
                if (System.Convert.ToInt32(thispart.LinkPart) > 0)
                {
                    if (thispart.BackGroundColor == "White")
                    {
                        LinkList newLink = new LinkList();
                        newLink.LinkID = thispart.LinkPart;
                        newLink.LinkColor = getNextColor();
                        thispart.BackGroundColor = newLink.LinkColor;
                        links.Add(newLink);
                        //We are adding the buttons here so we dont get multiple sets of buttons for linked parts
                        thispart.quotingHTML += master.renderQuotingHTML(thispart.PartId, ddlStatus.SelectedValue, RFQID, true);
                    }
                    else
                    {
                        thispart.quotingHTML += master.renderQuotingHTML(thispart.PartId, ddlStatus.SelectedValue, RFQID, false);
                    }
                }
                else
                {
                    //If the parts are not linked we do the logic to display the buttons
                    thispart.quotingHTML += master.renderQuotingHTML(thispart.PartId, ddlStatus.SelectedValue, RFQID, true);
                }
                thispart.quotingHTML += "</div>";
            }

            List<string> partDetailIds = new List<string>();

            sql.CommandText = "Select prcPartID, ppdPartToPartID from linkPartReservedToCompany left join linkPartToPartDetail on ppdPartID = prcPartID where prcRFQID = @rfq and prcTSGCompanyID = @company ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@company", master.getCompanyId());
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (dr["ppdPartToPartID"].ToString() != "")
                {
                    partDetailIds.Add(dr["ppdPartToPartID"].ToString());
                }
                if (hdnReservedPartIds.Value != "")
                {
                    hdnReservedPartIds.Value += ",";
                }
                hdnReservedPartIds.Value += dr["prcPartID"].ToString();
            }
            dr.Close();

            if (partDetailIds.Count > 0)
            {
                sql.CommandText = $"Select ppdPartID from linkPartToPartDetail where ppdPartToPartID in ({string.Join(",", partDetailIds.ToArray())}) and ppdPartId not in (Select prcPartID from linkPartReservedToCompany where prcTSGCompanyID = @company and prcRFQID = @rfq) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@company", master.getCompanyId());
                sql.Parameters.AddWithValue("@rfq", RFQID);
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    hdnReservedPartIds.Value += "," + dr["ppdPartID"].ToString();
                }
                dr.Close();
            }

            sql.CommandText = "Select ptrPartID from linkPartToRfq where ptrRFQID = @rfq ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfq", RFQID);
            dr = sql.ExecuteReader();
            while (dr.Read())
            {
                if (hdnAllPartIds.Value != "")
                {
                    hdnAllPartIds.Value += ",";
                }
                hdnAllPartIds.Value += dr["ptrPartID"].ToString();
            }
            dr.Close();

            connection.Close();
            dgParts.DataSource = orderedList;
            dgParts.DataBind();

            lblNumberOfParts.Text = orderedList.Count.ToString();
        }

        protected void btnNewCustomerContact_click(object sender, EventArgs e)
        {
            lblMessage.Text = "\n<script>showEditDialog();</script>\n";
        }

        protected void addNewVehicle_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "insert into pktblVehicle ( vehVehicleName, vehCreated, vehCreatedBy) ";
            sql.CommandText += "output inserted.vehVehicleID ";
            sql.CommandText += "Values (@vehicle, GETDATE(), @createdBy)";
            sql.Parameters.AddWithValue("@vehicle", txtVehicle.Text);
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());

            string vehicleID = master.ExecuteScalar(sql, "EditRFQ").ToString();

            lblMessage.Text = "\n<script>$('#newProgramDialog').dialog('close');</script>";

            sql.CommandText = "select vehVehicleID, vehVehicleName from pktblVehicle";
            sql.Parameters.Clear();
            SqlDataReader progDR = sql.ExecuteReader();
            ddlVehicle.DataSource = progDR;
            ddlVehicle.DataTextField = "vehVehicleName";
            ddlVehicle.DataValueField = "vehVehicleID";
            ddlVehicle.DataBind();
            progDR.Close();

            ddlVehicle.SelectedValue = vehicleID;

            connection.Close();
        }

        protected void addNewProgram_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            sql.CommandText = "insert into program ( ProgramName, proCreated, proCreatedBy) ";
            sql.CommandText += "output inserted.programID ";
            sql.CommandText += "Values (@program, GETDATE(), @createdBy)";
            sql.Parameters.AddWithValue("@program", txtNewProgram.Text);
            sql.Parameters.AddWithValue("@createdBy", master.getUserName());

            string programID = master.ExecuteScalar(sql, "EditRFQ").ToString();

            lblMessage.Text = "\n<script>$('#newProgramDialog').dialog('close');</script>";

            sql.CommandText = "select ProgramID, ProgramName from Program where ProgramName not in ('0','  ADD NEW') order by ProgramName";
            sql.Parameters.Clear();
            SqlDataReader progDR = sql.ExecuteReader();
            ddlProgram.DataSource = progDR;
            ddlProgram.DataTextField = "ProgramName";
            ddlProgram.DataValueField = "ProgramID";
            ddlProgram.DataBind();
            progDR.Close();

            ddlProgram.SelectedValue = programID;
            connection.Close();
        }

        protected void duplicatePart_click(object sender, EventArgs e)
        {
            string partID = hdnPartID.Value;
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            partID.Replace("&nbsp;", " ");

            sql.CommandText = "Select top 1 prtRFQLineNumber from tblPart, linkPartToRFQ where ptrRFQID = @rfqID and ptrPartID = prtPARTID order by prtRFQLineNumber desc";
            sql.Parameters.AddWithValue("@rfqID", RFQID);
            SqlDataReader dr = sql.ExecuteReader();
            int lineNum = 0;
            if (dr.Read())
            {
                lineNum = System.Convert.ToInt32(dr.GetValue(0).ToString()) + 1;
            }
            dr.Close();

            sql.CommandText = "Select prtPARTID from tblPart, linkPartToRFQ where ptrRFQID = @rfqID and ptrPartID = prtPartID and prtPartNumber = @partNum";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partNum", partID);
            sql.Parameters.AddWithValue("@rfqID", RFQID);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                partID = dr.GetValue(0).ToString();
            }
            dr.Close();
            List<string> part = new List<string>();
            sql.CommandText = "Select prtPartNumber, prtpartDescription, prtPartTypeID, prtPicture, prtCreated, prtCreatedBy, prtModified, prtModifiedBy, prtPartLength, prtPartWidth, ";
            sql.CommandText += "prtPartHeight, prtPartMaterialType, prtPartWeight, prtPartThickness, prtPartRevLevEAU, prtPartName, @lineNum, prtNote from tblPart where prtPARTID = @partID ";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partID", partID);
            sql.Parameters.AddWithValue("@lineNum", lineNum);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                part.Add(dr.GetValue(0).ToString());
                part.Add(dr.GetValue(1).ToString());
                part.Add("33");
                part.Add(dr.GetValue(3).ToString());
                part.Add(dr.GetValue(4).ToString());
                part.Add(dr.GetValue(5).ToString());
                part.Add(dr.GetValue(6).ToString());
                part.Add(dr.GetValue(7).ToString());
                part.Add(dr.GetValue(8).ToString());
                part.Add(dr.GetValue(9).ToString());
                part.Add(dr.GetValue(10).ToString());
                part.Add(dr.GetValue(11).ToString());
                part.Add(dr.GetValue(12).ToString());
                part.Add(dr.GetValue(13).ToString());
                part.Add(dr.GetValue(14).ToString());
                part.Add(dr.GetValue(15).ToString());
                part.Add(dr.GetValue(16).ToString());
                part.Add(dr.GetValue(17).ToString());
            }
            dr.Close();


            sql.CommandText = "INSERT INTO tblPart (prtPartNumber, prtpartDescription, prtPartTypeID, prtPicture, prtCreated, prtCreatedBy, prtModified, ";
            sql.CommandText += "prtModifiedBy, prtPartLength, prtPartWidth, prtPartHeight, prtPartMaterialType, prtPartWeight, prtPartThickness, prtPartRevLevEAU, prtPartName, prtRFQLineNumber, prtNote) ";
            sql.CommandText += "output inserted.prtPARTID ";
            sql.CommandText += "VALUES(@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17)";
            sql.Parameters.Clear();
            for (int i = 0; i < part.Count; i++)
            {
                //if(i == 0)
                //{
                //    sql.Parameters.AddWithValue("@" + i.ToString() + "_2", part[i]);
                //}
                //else
                //{
                sql.Parameters.AddWithValue("@" + i.ToString(), part[i]);
                //}
            }
            partID = master.ExecuteScalar(sql, "Edit RFQ").ToString();

            sql.CommandText = "insert into linkPartToRFQ (ptrPartID, ptrRFQID, ptrCreated, ptrCreatedBy) ";
            sql.CommandText += "values (@partID, @rfq, GETDATE(), @user)";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@partID", partID);
            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@user", master.getUserName());
            master.ExecuteNonQuery(sql, "EditRFQ");

            connection.Close();

            Response.Redirect(Request.RawUrl);
        }

        protected void btnDeleteAllHistory(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;

            List<string> partID = new List<string>();

            sql.CommandText = "Select ptrPartID from linkPartToRFQ where ptrRFQID = @rfqID";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@rfqID", RFQID);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                partID.Add(dr.GetValue(0).ToString());
            }
            dr.Close();

            for (int i = 0; i < partID.Count; i++)
            {
                sql.CommandText = "Delete from linkPartToQuotehistory where pqhPartId = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID[i]);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from linkPartToHistoricalQuote where phqPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID[i]);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from linkPartToOldNoQuote where onqPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID[i]);
                master.ExecuteNonQuery(sql, "Edit RFQ");

                sql.CommandText = "Delete from linkPartToHistory where pthPartID = @partID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID[i]);
                master.ExecuteNonQuery(sql, "Edit RFQ");
            }

            connection.Close();
        }


        protected void cbBundleQuotesYes_Checked_Clicked(Object sender, EventArgs e)
        {
            txtSendQuotes.Enabled = cbBundleQuotesYes.Checked;
        }


        //protected string Get(CheckBox chk)
        //{
        //    return $"{chk.nam}"cbNaBuild ={ (cbNaBuild.Checked ? 1 : 0)}
        //}

        protected void btnSave_Click_Click(object sender, EventArgs e)
        {
            if (RFQID == 0)
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                Boolean doNotSell = false;

                sql.CommandText = "Select cusDoNotSell from Customer where CustomerId = @customer ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    if (dr["cusDoNotSell"].ToString() != "")
                    {
                        doNotSell = System.Convert.ToBoolean(dr["cusDoNotSell"].ToString());
                    }
                }
                dr.Close();

                if (doNotSell)
                {
                    connection.Close();
                    litScript.Text = "<script>alert('" + ddlCustomer.SelectedItem.ToString() + " is on the do not sell list.  You may not enter an RFQ for them.  This RFQ has not been saved.');</script>";
                    return;
                }

                sql.CommandText = "Select TSGSalesmanID from CustomerLocation where CustomerLocationID = @plant";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                dr = sql.ExecuteReader();
                int salesmanID = 0;
                if (dr.Read())
                {
                    salesmanID = System.Convert.ToInt32(dr.GetValue(0));
                }

                dr.Close();
                sql.Parameters.Clear();
                int programID = System.Convert.ToInt32(ddlProgram.SelectedValue);
                if (ddlProgram.SelectedValue == "0" && txtNewProgram.Text != "")
                {
                    sql.CommandText = "insert into Program (ProgramName, proCreated, proCreatedBy) OUTPUT inserted.ProgramID Values (@program, GETDATE(), @createdBy )";
                    sql.Parameters.AddWithValue("@program", txtNewProgram.Text);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    programID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                    //txts
                }


                sql.CommandText = "insert into tblRFQ ";
                sql.CommandText += "( rfqStatus, rfqCustomerID, rfqPlantID, rfqCustomerRFQNumber," +
                    " rfqProgramID, rfqOEMID, rfqVehicleID, rfqDueDate, rfqDateReceived, rfqEstimatedPODate, rfqBidDate, rfqToolCountryID," +
                    " rfqEngineeringNumber, rfqProductTypeID, rfqNumberOfParts,  rfqNotes, " +
                    "rfqMeetingNotes, rfqCreated, rfqCreatedBy,  rfqLiveWork, rfqSourceID, rfqAdditionalSourceID, " +
                    "rfqSalesman, rfqInternalDueDate, rfqHandlingID, rfqCheckBit, rfqUseTSGLogo, " +
                    "rfqCustomerContact, rfqTurnkey, rfqGlobalProgram, " +
                    "cbDies, " +
                    "cbNaBuild, cbHomeLineSupport, cbCheckFixture, cbBlended, cbShippingToPlant," +
                    "cbHydroformTooling, cbKitDie, cbFormSteelCoatings," +
                    "cbMoldToolingTubeDies, cbLcc, cbSparePunchesButtons, cbEngineeringChange, cbSeeDocumentFromCustomer, cbIncludeEarlyParts, cbAssemblyToolingEquipment," +
                    "cbIncludeFinanceCost, cbPrototypes, cbTsims, cbTurnkeySeeInternalTsgRfq, cbTransferFingers, cbBundleQuotesYes, txtSendQuotes ) ";
                //sql.CommandText += " rfqATSReady, rfqBTSReady, rfqDTSReady, rfqETSReady, rfqGTSReady, rfqHTSReady, rfqRTSReady, rfqSTSReady, rfqUGSReady, rfqSendTo, rfqCCTo, rfqBCCTo ) ";
                sql.CommandText += " OUTPUT inserted.rfqID ";
                sql.CommandText += " values ( @status, @customer, @plant, @rfq, @program, @oem, @vehicle, @due, @received, @podate, @biddate, ";
                //sql.CommandText += " @country, @eng, @type, @parts, @notes, @meetingnotes, current_timestamp, @createdby,  @livework, @src, @srctwo, @salesman, DateAdd(DD, 7,GETDATE()), @handling, 1, @logo, @contact, @turnKey, @global, ";
                sql.CommandText += " @country, @eng, @type, @parts, @notes, @meetingnotes, current_timestamp, @createdby, " +
                    " @livework, @src, @srctwo, @salesman, @internalduedate, @handling, 1, @logo, @contact, @turnKey, @global, " +
                    "@cbDies, " +
                    "@cbNaBuild, @cbHomeLineSupport, @cbCheckFixture, @cbBlended, @cbShippingToPlant, " +
                    "@cbHydroformTooling, @cbKitDie, @cbFormSteelCoatings," +
                    "@cbMoldToolingTubeDies, @cbLcc, @cbSparePunchesButtons, @cbEngineeringChange, @cbSeeDocumentFromCustomer, @cbIncludeEarlyParts, @cbAssemblyToolingEquipment," +
                    "@cbIncludeFinanceCost, @cbPrototypes, @cbTsims, @cbTurnkeySeeInternalTsgRfq, @cbTransferFingers, @cbBundleQuotesYes, @txtSendQuotes ) ";



                //sql.CommandText += "@ats, @bts, @dts, @ets, @gts, @hts, @rts, @sts, @ugs, @sendTo, @cc, @bcc ) ";
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
                sql.Parameters.AddWithValue("@rfq", txtCustomerRFQ.Text.Trim());
                sql.Parameters.AddWithValue("@program", programID);
                sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                sql.Parameters.AddWithValue("@vehicle", ddlVehicle.SelectedValue);
                sql.Parameters.AddWithValue("@due", calDueDate.Text);
                sql.Parameters.AddWithValue("@internalduedate", calIntDueDate.Text);
                sql.Parameters.AddWithValue("@received", calReceivedDate.Text);
                sql.Parameters.AddWithValue("@podate", calPODate.Text);
                sql.Parameters.AddWithValue("@biddate", calBidDate.Text);
                sql.Parameters.AddWithValue("@country", ddlToolCountry.SelectedValue);
                sql.Parameters.AddWithValue("@eng", txtEngineeringNumber.Text.Trim());
                sql.Parameters.AddWithValue("@type", ddlProductType.SelectedValue);
                sql.Parameters.AddWithValue("@parts", 0);
                sql.Parameters.AddWithValue("@notes", txtNotes.Text.Trim());
                sql.Parameters.AddWithValue("@meetingnotes", "");
                //sql.Parameters.AddWithValue("@meetingnotes", txtMeetingNotes.Text.Trim());
                sql.Parameters.AddWithValue("@src", ddlRFQSource.SelectedValue);
                sql.Parameters.AddWithValue("@srctwo", ddlRFQSource2.SelectedValue);
                sql.Parameters.AddWithValue("@salesman", salesmanID);
                sql.Parameters.AddWithValue("@handling", ddlHandling.SelectedValue);
                sql.Parameters.AddWithValue("@contact", ddlCustomerContact.SelectedValue);



                //sql.Parameters.AddWithValue("@ats", cbATSReady.Checked);
                //sql.Parameters.AddWithValue("@bts", cbBTSReady.Checked);
                //sql.Parameters.AddWithValue("@dts", cbDTSReady.Checked);
                //sql.Parameters.AddWithValue("@ets", cbETSReady.Checked);
                //sql.Parameters.AddWithValue("@gts", cbGTSReady.Checked);
                //sql.Parameters.AddWithValue("@hts", cbHTSReady.Checked);
                //sql.Parameters.AddWithValue("@rts", cbRTSReady.Checked);
                //sql.Parameters.AddWithValue("@sts", cbSTSReady.Checked);
                //sql.Parameters.AddWithValue("@ugs", cbUGSReady.Checked);
                //sql.Parameters.AddWithValue("@sendTo", txtSendBundledTo.Text);
                //sql.Parameters.AddWithValue("@cc", txtCCBundledTo.Text);
                //sql.Parameters.AddWithValue("@bcc", txtBCCBundledTo.Text);


                if (cbUseTSGLogo.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                }
                if (cbTurnkey.Checked)
                {
                    sql.Parameters.AddWithValue("@turnkey", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@turnkey", 0);
                }

                if (cbLiveWork.Checked)
                {
                    sql.Parameters.AddWithValue("@livework", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@livework", 0);
                }
                if (cbGlobalProgram.Checked)
                {
                    sql.Parameters.AddWithValue("@global", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@global", 0);
                }

                sql.Parameters.AddWithValue("@cbDies", cbDies.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbNaBuild", cbNaBuild.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbHomeLineSupport", cbHomeLineSupport.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbCheckFixture", cbCheckFixture.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbBlended", cbBlended.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbShippingToPlant", cbShippingToPlant.Checked ? 1 : 0);

                sql.Parameters.AddWithValue("@cbHydroformTooling", cbHydroformTooling.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbKitDie", cbKitDie.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbFormSteelCoatings", cbFormSteelCoatings.Checked ? 1 : 0);

                sql.Parameters.AddWithValue("@cbMoldToolingTubeDies", cbMoldToolingTubeDies.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbLcc", cbLcc.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbSparePunchesButtons", cbSparePunchesButtons.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbEngineeringChange", cbEngineeringChange.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbSeeDocumentFromCustomer", cbSeeDocumentFromCustomer.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbIncludeEarlyParts", cbIncludeEarlyParts.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbAssemblyToolingEquipment", cbAssemblyToolingEquipment.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbIncludeFinanceCost", cbIncludeFinanceCost.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbPrototypes", cbPrototypes.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbTsims", cbTsims.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbTurnkeySeeInternalTsgRfq", cbTurnkeySeeInternalTsgRfq.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbTransferFingers", cbTransferFingers.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@cbBundleQuotesYes", cbBundleQuotesYes.Checked ? 1 : 0);
                sql.Parameters.AddWithValue("@txtSendQuotes", txtSendQuotes.Text);




                sql.Parameters.AddWithValue("@createdby", Context.User.Identity.Name);



                //if(cbBundleQuotesYes.Checked && txtSendQuotes.Text.Length == 0)
                //{
                //    ScriptManager.RegisterClientScriptBlock
                //}

                //ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Insert is successfull')", true);


                try
                {
                    Int64 newID = System.Convert.ToInt64(master.ExecuteScalar(sql, "editRFQ"));
                    connection.Close();

                    //SaveCheckBoxes(newID);


                    Response.Redirect("~/EditRFQ?id=" + newID);
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                    lblMessage.Text += "<BR>";
                    lblMessage.Text += sql.CommandText;
                    connection.Close();
                }



            }
            else
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                int programID = System.Convert.ToInt32(ddlProgram.SelectedValue);
                if (ddlProgram.SelectedValue == "0" && txtNewProgram.Text != "")
                {
                    sql.CommandText = "insert int Program (ProgramName, proCreated, proCreatedBy) OUTPUT inserted.ProgramID Values(@program, GETDATE(), @createdBy )";
                    sql.Parameters.AddWithValue("@program", txtNewProgram.Text);
                    sql.Parameters.AddWithValue("@createdBy", master.getUserName());
                    programID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                }

                sql.Connection = connection;
                sql.CommandText = "update tblRFQ set ";
                sql.CommandText += "rfqStatus = @status, rfqCustomerID=@customer,  rfqCustomerRFQNumber=@rfq, rfqProgramID=@program, rfqOEMID=@oem, rfqVehicleID=@vehicle, rfqDueDate=@due, rfqDateReceived=@received, rfqEstimatedPODate=@podate, rfqBidDate=@biddate, ";
                sql.CommandText += "rfqToolCountryID=@country, rfqEngineeringNumber=@eng, rfqProductTypeID=@type, rfqNumberOfParts=@parts,  rfqNotes=@notes, rfqMeetingNotes=@meetingnotes, rfqModified=current_timestamp, rfqModifiedBy=@modby,  rfqLiveWork =@livework, ";
                sql.CommandText += $"rfqNumberOfQuotes=@quote, " +
                    $"rfqSourceID=@src, " +
                    $"rfqAdditionalSourceID=@srctwo, " +
                    $"rfqHandlingID = @handling, " +
                    $"rfqCheckBit = 1, " +
                    $"rfqUseTSGLogo = @logo, " +
                    $"rfqTurnkey = @turnkey, " +
                    $"rfqGlobalProgram = @global, " +
                    $"rfqCustomerContact = @custContact, " +
                    $"rfqPlantID = @plant, " +
                    $"cbNaBuild={(cbNaBuild.Checked ? 1 : 0)}, " +
                    $"cbHomeLineSupport={(cbHomeLineSupport.Checked ? 1 : 0)}, " +
                    $"cbCheckFixture={(cbCheckFixture.Checked ? 1 : 0)}, " +
                    $"cbBlended={(cbBlended.Checked ? 1 : 0)}, " +
                    $"cbShippingToPlant={(cbShippingToPlant.Checked ? 1 : 0)}, " +
                    $"cbHydroformTooling={(cbHydroformTooling.Checked ? 1 : 0)}, " +
                    $"cbKitDie={(cbKitDie.Checked ? 1 : 0)}, " +
                    $"cbFormSteelCoatings={(cbFormSteelCoatings.Checked ? 1 : 0)}, " +
                    $"cbMoldToolingTubeDies={(cbMoldToolingTubeDies.Checked ? 1 : 0)}, " +
                    $"cbLcc={(cbLcc.Checked ? 1 : 0)}, " +
                    $"cbSparePunchesButtons={(cbSparePunchesButtons.Checked ? 1 : 0)}, " +
                    $"cbEngineeringChange={(cbEngineeringChange.Checked ? 1 : 0)}, " +
                    $"cbSeeDocumentFromCustomer={(cbSeeDocumentFromCustomer.Checked ? 1 : 0)}, " +
                    $"cbIncludeEarlyParts={(cbIncludeEarlyParts.Checked ? 1 : 0)}, " +
                    $"cbAssemblyToolingEquipment={(cbAssemblyToolingEquipment.Checked ? 1 : 0)}, " +
                    $"cbIncludeFinanceCost={(cbIncludeFinanceCost.Checked ? 1 : 0)}, " +
                    $"cbPrototypes={(cbPrototypes.Checked ? 1 : 0)}, " +
                    $"cbTsims={(cbTsims.Checked ? 1 : 0)}, " +
                    $"cbTurnkeySeeInternalTsgRfq={(cbTurnkeySeeInternalTsgRfq.Checked ? 1 : 0)}, " +
                    $"cbTransferFingers={(cbTransferFingers.Checked ? 1 : 0)}, " +
                    $"cbBundleQuotesYes={(cbBundleQuotesYes.Checked ? 1 : 0)}, " +
                    $"cbDies={(cbDies.Checked ? 1 : 0)}, " +
                    //$"txtSendQuotes={txtSendQuotes.Text}, ";
                    $"txtSendQuotes=@txtSendQuotes, ";

                sql.Parameters.AddWithValue("@txtSendQuotes", txtSendQuotes.Text);

                sql.CommandText += " rfqInternalDueDate = @internalduedate ";
                sql.CommandText += " where rfqID=@id ";
                sql.Parameters.AddWithValue("@status", ddlStatus.SelectedValue); // Received
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@rfq", txtCustomerRFQ.Text.Trim());
                sql.Parameters.AddWithValue("@program", programID);
                sql.Parameters.AddWithValue("@oem", ddlOEM.SelectedValue);
                sql.Parameters.AddWithValue("@vehicle", ddlVehicle.SelectedValue);
                sql.Parameters.AddWithValue("@due", calDueDate.Text);
                sql.Parameters.AddWithValue("@internalduedate", calIntDueDate.Text);
                sql.Parameters.AddWithValue("@received", calReceivedDate.Text);
                sql.Parameters.AddWithValue("@podate", calPODate.Text);
                sql.Parameters.AddWithValue("@biddate", calBidDate.Text);
                sql.Parameters.AddWithValue("@country", ddlToolCountry.SelectedValue);
                sql.Parameters.AddWithValue("@eng", txtEngineeringNumber.Text.Trim());
                sql.Parameters.AddWithValue("@type", ddlProductType.SelectedValue);
                sql.Parameters.AddWithValue("@src", ddlRFQSource.SelectedValue);
                sql.Parameters.AddWithValue("@srctwo", ddlRFQSource2.SelectedValue);
                Int64 partCount = 0;
                Int64 quoteCount = 0;
                sql.Parameters.AddWithValue("@parts", partCount);
                sql.Parameters.AddWithValue("@quote", quoteCount);
                sql.Parameters.AddWithValue("@notes", txtNotes.Text.Trim());
                sql.Parameters.AddWithValue("@meetingnotes", "");
                sql.Parameters.AddWithValue("@handling", ddlHandling.SelectedValue);
                sql.Parameters.AddWithValue("@custContact", ddlCustomerContact.SelectedValue);
                sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);


                if (cbLiveWork.Checked)
                {
                    sql.Parameters.AddWithValue("@livework", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@livework", 0);
                }
                if (cbUseTSGLogo.Checked)
                {
                    sql.Parameters.AddWithValue("@logo", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@logo", 0);
                }
                if (cbTurnkey.Checked)
                {
                    sql.Parameters.AddWithValue("@turnkey", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@turnkey", 0);
                }
                if (cbGlobalProgram.Checked)
                {
                    sql.Parameters.AddWithValue("@global", 1);
                }
                else
                {
                    sql.Parameters.AddWithValue("@global", 0);
                }

                sql.Parameters.AddWithValue("@modby", Context.User.Identity.Name);
                sql.Parameters.AddWithValue("@id", RFQID);
                try
                {
                    master.ExecuteNonQuery(sql, "EditRFQ");
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                }
                connection.Close();
                populate_Header();
            }
            Response.Redirect(Request.RawUrl);
        }


        protected void importFiles_click(object sender, EventArgs e)
        {
            Site master = new Site();
            if (attachmentUpload.HasFiles)
            {
                foreach (var attachment in attachmentUpload.PostedFiles)
                {
                    Microsoft.SharePoint.Client.ClientContext ctx = new Microsoft.SharePoint.Client.ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/");
                    ctx.Credentials = master.getSharePointCredentials();
                    Microsoft.SharePoint.Client.Web web = ctx.Web;
                    // if this does not exist we will get an error 
                    var mainfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/");
                    ctx.Load(web);

                    Microsoft.SharePoint.Client.List list = ctx.Web.Lists.GetByTitle("Documents");
                    Microsoft.SharePoint.Client.ListItem list2 = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + RFQID).ListItemAllFields;

                    ctx.Load(list);
                    ctx.Load(list.RootFolder);
                    ctx.Load(list.RootFolder.Folders);
                    ctx.Load(list.RootFolder.Files);
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                    Microsoft.SharePoint.Client.Folder fo = list2.Folder;
                    Microsoft.SharePoint.Client.FileCollection files = fo.Files;

                    ctx.Load(files);
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                    FileCreationInformation newFile = new FileCreationInformation();
                    newFile.ContentStream = attachment.InputStream;
                    newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/RFQ%20Email%20Attachments/" + RFQID + "/" + attachment.FileName;
                    newFile.Overwrite = true;

                    Microsoft.SharePoint.Client.File file = list.RootFolder.Files.Add(newFile);
                    list.Update();

                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);


                    ////Create folder to hold the email attachments
                    //ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject");
                    //ctx.Credentials = master.getSharePointCredentials();
                    //Web web = ctx.Web;
                    //// if this does not exist we will get an error 
                    //var mainfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject/Shared Documents/RFQ Email Attachments/");
                    //ctx.Load(web);
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    //var customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject/Shared Documents/RFQ Email Attachments/" + RFQID);
                    //ctx.Load(web);
                    //try
                    //{
                    //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    //}
                    //catch
                    //{
                    //    // assume need to create
                    //    //lblMessage.Text += "Need to create customer folder";
                    //    mainfolder.Folders.Add(RFQID.ToString());
                    //    ctx.Credentials = master.getSharePointCredentials();
                    //    ctx.Load(web);
                    //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    //    customerfolder = web.GetFolderByServerRelativeUrl("https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject/Shared Documents/RFQ Email Attachments/" + RFQID);
                    //    ctx.Load(web);
                    //    SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                    //}

                    //byte[] fileData = null;
                    //using (var binaryReader = new System.IO.BinaryReader(attachment.InputStream))
                    //{
                    //    fileData = binaryReader.ReadBytes((int)attachment.InputStream.Length);
                    //}
                    //System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                    //FileCreationInformation newFile = new FileCreationInformation();
                    //newFile.ContentStream = newStream;
                    //newFile.Url = "https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject/Shared Documents/RFQ Email Attachments/" + RFQID + "/" + attachment.FileName;
                    //newFile.Overwrite = true;
                    ////Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle(RFQID.ToString());
                    ////Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                    ////partPicturesList.Update();
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                    //// set the Attributes
                    //Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                    //newItem["Title"] = txtPart.Text;
                    //newItem["PartID"] = UpdateRecord;
                    //newItem["PartNumber"] = txtPart.Text;
                    //newItem["PartDescription"] = txtDescription.Text;
                    //newItem["RFQ"] = "https://tsgrfq.azurewebsites.net/EditRFQ?id=" + RFQID;
                    //newItem.Update();
                    //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                }
            }
        }


        // process Uploaded Spreadsheet and create Parts for this RFQ
        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                // you have a file
            }
            else
            {
                Response.Write("<script>alert('Please enter a file to import!');</script>");
                return;
            }
            Site master = new RFQ.Site();
            lblMessage.Text = "";
            // Get The File that was uploaded.  Must be an Excel Workbook
            String FileName = fileUpload.PostedFile.FileName;
            XSSFWorkbook wb = new XSSFWorkbook(fileUpload.PostedFile.InputStream);
            // The Tab/Sheet we want is ALWAYS named CAD INFO
            XSSFSheet sh = (XSSFSheet)wb.GetSheet("CAD INFO");
            int i = 1; // skip the header row
            if (sh != null)
            {
                // Get the column where the part picture is located.
                // all other data is relative to this column
                Int32 PictureColumn = 1;

                XSSFDrawing drawing = (XSSFDrawing)sh.CreateDrawingPatriarch();
                Boolean GotFirstPicture = false;
                foreach (XSSFShape shape in drawing.GetShapes())
                {
                    if (!GotFirstPicture)
                    {
                        try
                        {
                            XSSFPicture picture = (XSSFPicture)shape;
                            XSSFClientAnchor anchor = (XSSFClientAnchor)picture.GetAnchor();
                            PictureColumn = anchor.Col1;
                            GotFirstPicture = true;
                        }
                        catch
                        {

                        }
                    }
                }

                // need to start the line number with the next sequential line number
                // that means we need to get the current max line number from this rfq/part combination
                SqlConnection MaxLineConnection = new SqlConnection(master.getConnectionString());
                MaxLineConnection.Open();
                SqlCommand MaxLineSql = new SqlCommand();
                MaxLineSql.Connection = MaxLineConnection;

                int CurrentMaxLineNumber = 0;
                MaxLineSql.CommandText = "Select coalesce(max(prtRFQLineNumber),0) from tblPart, linkPartToRFQ where ptrPartID=prtPartID and ptrRFQID=@rfq";

                MaxLineSql.Parameters.AddWithValue("@rfq", RFQID);

                SqlDataReader MaxLineDR = MaxLineSql.ExecuteReader();
                while (MaxLineDR.Read())
                {
                    CurrentMaxLineNumber = MaxLineDR.GetInt32(0);
                }
                int count = 1;
                int minLineNum = 0;
                // loop through each row
                while (sh.GetRow(i) != null)
                {
                    // wrap this with a try any error just don't load that row
                    try
                    {
                        // Apparently, the part number can contain the * character, which separates MULTIPLE parts
                        String RawPartNumber = "";
                        try
                        {
                            RawPartNumber = sh.GetRow(i).GetCell(PictureColumn + 2).StringCellValue;
                        }
                        catch
                        {
                            try
                            {
                                RawPartNumber = sh.GetRow(i).GetCell(PictureColumn + 2).NumericCellValue.ToString();
                            }
                            catch
                            {
                                break;
                            }
                        }
                        // each part within the asterisks will be linked together.
                        // This LinkID field will hold the common id from linkPartToPart table
                        // Only Need To Link if there is an asterisk in the part number
                        Int32 LinkID = 0;
                        Boolean NeedToLink = (RawPartNumber.Split('*').Count() > 1);
                        if (i == 1)
                        {
                            minLineNum = System.Convert.ToInt32((count + CurrentMaxLineNumber).ToString()) - 1;
                        }
                        foreach (String eachpart in RawPartNumber.Split('*'))
                        {
                            // Just using this class to make sure that all fields are picked up for that part
                            RFQPart newPart = new RFQPart();
                            // reset some values
                            String pictureName = "";
                            Int64 newID = 0;
                            // notice that if there are multiple parts, they get the same part index.
                            // this is intentional, to match them each up with the picture in their row
                            newPart.PartIndex = i;
                            newPart.prtPartNumber = eachpart.Replace("!", "").Replace("$", "").Replace("#", "").Replace("%", "").Replace("/", "").Replace(":", "").Trim();
                            newPart.prtPartDescription = master.readCellString(sh.GetRow(i).GetCell(PictureColumn + 3));
                            newPart.LineNumber = (count + CurrentMaxLineNumber).ToString();
                            newPart.MaterialType = "";
                            newPart.Length = master.readCellDouble(sh.GetRow(i).GetCell(PictureColumn + 4), 0);
                            newPart.Width = master.readCellDouble(sh.GetRow(i).GetCell(PictureColumn + 5), 0);
                            newPart.Height = master.readCellDouble(sh.GetRow(i).GetCell(PictureColumn + 6), 0);
                            newPart.MaterialThickness = master.readCellDouble(sh.GetRow(i).GetCell(PictureColumn + 8), 0);
                            newPart.Weight = master.readCellDouble(sh.GetRow(i).GetCell(PictureColumn + 9), 0);
                            newPart.annualVolume = master.readCellInt(sh.GetRow(i).GetCell(PictureColumn + 10), 0);
                            newPart.MaterialType = master.readCellString(sh.GetRow(i).GetCell(PictureColumn + 7));
                            newPart.BlankDescription = newPart.Length + " x " + newPart.Width + " x " + newPart.Height;
                            newPart.ptyPartDescription = "";
                            string note = master.readCellString(sh.GetRow(i).GetCell(PictureColumn + 15));
                            pictureName = "RFQ" + Request["id"] + "_" + newPart.LineNumber + "_" + newPart.prtPartNumber + ".png";
                            SqlConnection connection = new SqlConnection(master.getConnectionString());
                            connection.Open();
                            SqlCommand sql = new SqlCommand();
                            sql.Connection = connection;

                            sql.CommandText = "Select mtyMaterialTypeID from pktblMaterialType where mtyMaterialType = @mat";
                            sql.Parameters.AddWithValue("@mat", newPart.MaterialType);

                            SqlDataReader dr = sql.ExecuteReader();

                            int materialID = 0;
                            if (dr.Read())
                            {
                                materialID = System.Convert.ToInt32(dr.GetValue(0));
                            }
                            dr.Close();
                            sql.Parameters.Clear();


                            if (materialID == 0)
                            {
                                sql.CommandText = "Insert into pktblMaterialType (mtyMaterialType, mtyCreated, mtyCreatedBy) ";
                                sql.CommandText += "Output inserted.mtyMaterialTypeID ";
                                sql.CommandText += "values (@mat, GETDATE(), @created)";
                                sql.Parameters.AddWithValue("@mat", newPart.MaterialType);
                                sql.Parameters.AddWithValue("@created", master.getUserName());
                                materialID = System.Convert.ToInt32(master.ExecuteScalar(sql, "EditRFQ"));
                            }

                            sql.Parameters.Clear();

                            // add the part to the tblPart table
                            sql.CommandText = "insert into tblPart (prtPartNumber, prtPartDescription, prtPicture, prtCreated, prtCreatedBy, prtPartLength, prtPartWidth, prtPartHeight, prtPartMaterialType, prtPartThickness, prtPartWeight, prtRFQLineNumber, prtNote, prtAnnualVolume) ";
                            sql.CommandText += " OUTPUT inserted.prtPartID ";
                            sql.CommandText += " values (@part, @desc, @pic, current_timestamp, @createdby, @len, @wid, @height, @type, @thick, @weight, @line, @note, @annualVolume) ";
                            sql.Parameters.AddWithValue("@part", newPart.prtPartNumber);
                            sql.Parameters.AddWithValue("@desc", newPart.prtPartDescription);
                            sql.Parameters.AddWithValue("@pic", pictureName);
                            sql.Parameters.AddWithValue("@createdby", Context.User.Identity.Name);
                            sql.Parameters.AddWithValue("@len", newPart.Length.ToString());
                            sql.Parameters.AddWithValue("@wid", newPart.Width.ToString());
                            sql.Parameters.AddWithValue("@height", newPart.Height.ToString());
                            sql.Parameters.AddWithValue("@type", materialID);
                            sql.Parameters.AddWithValue("@thick", newPart.MaterialThickness.ToString());
                            sql.Parameters.AddWithValue("@weight", newPart.Weight.ToString());
                            sql.Parameters.AddWithValue("@line", newPart.LineNumber);
                            sql.Parameters.AddWithValue("@note", note);
                            sql.Parameters.AddWithValue("@annualVolume", newPart.annualVolume.ToString());
                            try
                            {
                                newID = System.Convert.ToInt64(master.ExecuteScalar(sql, "EditRFQ"));
                            }
                            catch (Exception ex)
                            {
                                lblMessage.Text = ex.Message;
                                lblMessage.Text += "<BR>";
                                lblMessage.Text += sql.CommandText;
                            }
                            if (newID > 0)
                            {
                                // if we sucessfully added the part, link it to the RFQ
                                sql.CommandText = "insert into linkPartToRFQ (ptrPartID, ptrRFQID, ptrCreated, ptrCreatedBy) ";
                                sql.CommandText += " values (@part, @rfq, current_timestamp, @who) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@part", newID);
                                sql.Parameters.AddWithValue("@rfq", RFQID);
                                sql.Parameters.AddWithValue("@who", Context.User.Identity.Name);
                                master.ExecuteNonQuery(sql, "EditRFQ");
                                // put in the script that will go get the history records for this part
                                //lblMessage.Text += "\n<script>$('#txtFindPartNumber').val('');partToLink='" + newPart.prtPartNumber.Replace("+", "%2B") + "';findParts();</script>\n";
                                try
                                {
                                    linkPartToHistory(newPart.prtPartNumber, RFQID.ToString(), newID.ToString());
                                }
                                catch (Exception err)
                                {

                                }
                            }
                            //litPartScripts.Text += "<script>url = 'GetHistory.aspx?create=yes&search=" + newPart.prtPartNumber.Replace("+", "%2B") + "&part=" + newPart.prtPartNumber.Replace("+", "%2B") + "&rfq=" + RFQID + "&rand=' + Math.random();$.ajax({ url: url, success: function (data) { parseResults(data, 0, '" + newPart.prtPartNumber + "'); } });</script>\n";
                            // Now Link the parts together - if need to
                            if (NeedToLink)
                            {
                                if (LinkID == 0)
                                {
                                    // if linkid is zero, create the first link and save the value to LinkID
                                    sql.CommandText = "insert into linkPartToPart (ptpCreated, ptpCreatedBy, ptpModified, ptpModifiedBy) ";
                                    sql.CommandText += " output inserted.ptpPartToPartID ";
                                    sql.CommandText += " values (current_timestamp, @user, current_timestamp, @user) ";
                                    sql.Parameters.Clear();
                                    sql.Parameters.AddWithValue("@user", master.getUserName());
                                    LinkID = (int)master.ExecuteScalar(sql, "EditRFQ");
                                }
                                // we add each part to the detail table so that we know they are linked.
                                sql.CommandText = "insert into linkPartToPartDetail (ppdPartToPartId, ppdPartID, ppdCreated, ppdCreatedBy, ppdModified, ppdModifiedBy) ";
                                sql.CommandText += " values (@link, @part, current_timestamp, @user, current_timestamp, @user) ";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@part", newID);
                                sql.Parameters.AddWithValue("@link", LinkID);
                                sql.Parameters.AddWithValue("@user", master.getUserName());
                                master.ExecuteNonQuery(sql, "EditRFQ");
                            }
                            // walk through each picture in the CAD INFO Worksheet
                            Boolean uploadedPicture = false;
                            foreach (XSSFShape shape in drawing.GetShapes())
                            {
                                try
                                {
                                    XSSFPicture picture = (XSSFPicture)shape;
                                    XSSFClientAnchor anchor = (XSSFClientAnchor)picture.GetAnchor();
                                    XSSFPictureData pdata = (XSSFPictureData)picture.PictureData;
                                    String PictureRawPartNumber = "";
                                    try
                                    {
                                        PictureRawPartNumber = sh.GetRow(anchor.Row1).GetCell(PictureColumn + 2).StringCellValue;
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            PictureRawPartNumber = sh.GetRow(anchor.Row1).GetCell(PictureColumn + 2).NumericCellValue.ToString();
                                        }
                                        catch
                                        {
                                            //break;
                                        }
                                    }

                                    String PictureLineNumber = count.ToString();
                                    if ((PictureRawPartNumber == RawPartNumber) && (PictureLineNumber == (System.Convert.ToInt32(newPart.LineNumber) - minLineNum).ToString()))
                                    {
                                        uploadedPicture = true;
                                        newPart.MimeType = pdata.MimeType;
                                        newPart.PictureData = pdata.Data;
                                        // Upload image into SharePoint
                                        // Note: Could not find a way to add the meta data at the same time as adding the picture

                                        ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
                                        ctx.Credentials = master.getSharePointCredentials();
                                        Web web = ctx.Web;
                                        ctx.Load(web);
                                        //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                                        SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                                        Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("Part Pictures");
                                        System.IO.MemoryStream newStream = new System.IO.MemoryStream(newPart.PictureData);
                                        FileCreationInformation newFile = new FileCreationInformation();
                                        newFile.ContentStream = newStream;
                                        newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part Pictures/" + pictureName;
                                        newFile.Overwrite = true;

                                        Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                                        partPicturesList.Update();

                                        //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                                        SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                                        // set the Attributes - again could not find a way to do this without first adding the picture
                                        Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                                        newItem["Title"] = newPart.prtPartNumber;
                                        newItem["PartID"] = newID.ToString();
                                        newItem["PartNumber"] = newPart.prtPartNumber;
                                        newItem["PartDescription"] = newPart.prtPartDescription;
                                        newItem["RFQ"] = "https://tsgrfq.azurewebsites.net/EditRFQ?id=" + RFQID;
                                        newItem.Update();
                                        //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                                        SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                                        newPart.prtPicture = newFile.Url;
                                    }
                                }
                                catch
                                {

                                }
                            }
                            if (!uploadedPicture)
                            {
                                sql.CommandText = "update tblPart set prtPicture = @pic where prtPARTID = @id";
                                sql.Parameters.Clear();
                                sql.Parameters.AddWithValue("@pic", "NO CAD - SEE PRINT.png");
                                sql.Parameters.AddWithValue("@id", newID);
                                master.ExecuteNonQuery(sql, "Edit RFQ");
                            }
                            connection.Close();
                            count++;
                        }
                    }
                    catch (Exception err)
                    {
                        lblMessage.Text += "Error Loading Row " + i.ToString() + ": " + err.Message;
                    }
                    i++;
                }
            }
            SqlConnection connection2 = new SqlConnection(master.getConnectionString());
            connection2.Open();
            SqlCommand sql2 = new SqlCommand();
            sql2.Connection = connection2;

            sql2.Parameters.Clear();
            sql2.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
            sql2.Parameters.AddWithValue("@rfq", RFQID);

            master.ExecuteNonQuery(sql2, "EditRFQ");
            // Doing this here so that user cannot accidentally just hit refresh and reimport a bunch of parts
            // Random Number used to bypass page cache
            Random rand = new Random();
            //Response.Redirect("~/EditRFQ?id=" + RFQID + "&rand=" + rand.NextDouble());
            //header('Location: http://www.example.com/');
            //litPartScripts.Text += "<script>document.location.href = document.location.href;</script>";
            connection2.Close();


            populate_Header();
            populate_Parts();
        }

        public void linkPartToHistoryLucene(string partNum, string rfqID, string partID)
        {

        }

        //Initial linking for the history
        public void linkPartToHistory(string partNum, string rfqID, string partID)
        {
            if (partNum.Contains("npn"))
            {
                return;
            }
            Site master = new Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            SqlCommand sql = new SqlCommand();
            connection.Open();
            sql.Connection = connection;

            char[] delimiterChars = { ' ', '.', '_', '-', ',', '\\', '/', '+' };      // add in / \ 
            // possible TODO - instead  of partNum, tokens would use searchString instead
            // only if users do not like how it works now (searching for searchString and partNum)
            String[] tokens = partNum.Trim().Split(delimiterChars);
            String removedEndTag = "%";
            string partNumber = "";
            //removing all tokens are replacing with wildcard
            for (int i = 0; i < tokens.Length; i++)
            {
                if (i != 0)
                {
                    partNumber += "%" + tokens[i];
                }
                else
                {
                    partNumber += "%" + tokens[i];
                }
            }
            partNumber += "%";
            //Removing end tag and replacing with wildcard
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                removedEndTag += tokens[i] + "%";
            }
            //Getting Logest part Num
            string longest = tokens.OrderByDescending(s => s.Length).First();

            List<string> search = new List<string>();
            search.Add(partNumber.Trim());
            search.Add(removedEndTag.Trim());
            //search.Add(longest.Trim());
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Length > 4)
                {
                    search.Add("%" + tokens[i] + "%");
                }
            }
            //{ partNumber.Trim(), removedEndTag.Trim(), longest.Trim()};

            List<string> quoteID = new List<string>();
            List<Boolean> hts = new List<Boolean>();
            List<Boolean> sts = new List<Boolean>();
            List<Boolean> ugs = new List<Boolean>();
            List<string> mass = new List<string>();
            List<string> parts = new List<string>();
            List<string> noQuote = new List<string>();
            List<string> SA = new List<string>();
            List<string> htsSA = new List<string>();
            List<string> stsSA = new List<string>();
            List<string> ugsSA = new List<string>();

            SqlDataReader dr;

            for (int i = 0; i < search.Count; i++)
            {
                if (search[i].Length < 7)
                {
                    continue;
                }
                //Mass history
                //sql.CommandText = "Select top 25 qhiQuoteHistoryID, qhiGroupCompany, qhiPartNumber, qhiPartDescription, qhiRFQNumber, qhiCustomerRFQNumber, qhiCustomerRfqNum, qhiQuoteOrNoQuote from tblQuoteHistory ";
                //sql.CommandText += "where qhiPartNumber like @partNum and DATEADD(MONTH, -18, GETDATE()) < qhiDateDue order by qhiDateDue desc";
                //sql.Parameters.Clear();
                //sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");

                //dr = sql.ExecuteReader();
                //while(dr.Read())
                //{
                //    if(!mass.Contains(dr.GetValue(0).ToString()))
                //    {
                //        mass.Add(dr.GetValue(0).ToString());
                //    }
                //}
                //dr.Close();

                //Everything else
                sql.CommandText = "Select top 50 ptqQuoteID, prcPartID, nquNoQuoteID, prtPARTID, ptqPartID, nquPartID, ptqHTS, ptqSTS, ptqUGS ";
                sql.CommandText += "from linkPartToRFQ, Customer, tblRFQ, tblPart ";
                sql.CommandText += "left outer join linkPartToQuote on prtPARTID = ptqPartID ";
                sql.CommandText += "left outer join tblQuote on ptqQuoteID = quoQuoteID ";
                sql.CommandText += "left outer join linkPartReservedToCompany on prcPartID = prtPARTID ";
                sql.CommandText += "left outer join tblNoQuote on nquPartID = prtPARTID ";
                //sql.CommandText += "where ptrPARTID = prtPARTID and prtPartNumber like @partNum and ptrRFQID = rfqID and rfqID <> @rfqID and rfqCustomerID = CustomerID and DATEADD(MONTH, -18, GETDATE()) < prtCreated order by prtPARTID desc";
                sql.CommandText += "where ptrPARTID = prtPARTID and prtPartNumber like @partNum and ptrRFQID = rfqID and rfqID <> @rfqID and rfqCustomerID = CustomerID order by prtPARTID desc";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");
                sql.Parameters.AddWithValue("@rfqID", RFQID);
                dr = sql.ExecuteReader();

                List<string> partsDeltWith = new List<string>();
                while (dr.Read())
                {
                    if (dr.GetValue(0).ToString() != "")
                    {
                        if (!quoteID.Contains(dr.GetValue(0).ToString()))
                        {
                            quoteID.Add(dr.GetValue(0).ToString());
                            partsDeltWith.Add(dr.GetValue(4).ToString());
                            hts.Add(dr.GetBoolean(6));
                            sts.Add(dr.GetBoolean(7));
                            ugs.Add(dr.GetBoolean(8));
                        }
                    }
                    //if (dr.GetValue(2).ToString() != "")
                    //{
                    //    if (!noQuote.Contains(dr.GetValue(2).ToString()))
                    //    {
                    //        noQuote.Add(dr.GetValue(2).ToString());
                    //        partsDeltWith.Add(dr.GetValue(5).ToString());
                    //    }
                    //}
                    if (dr.GetValue(1).ToString() != "")
                    {
                        if (!parts.Contains(dr.GetValue(1).ToString()) && !partsDeltWith.Contains(dr.GetValue(1).ToString()))
                        {
                            parts.Add(dr.GetValue(1).ToString());
                        }
                    }
                    if (dr.GetValue(3).ToString() != "" && dr.GetValue(1).ToString() == "")
                    {
                        if (!parts.Contains(dr.GetValue(3).ToString()) && !partsDeltWith.Contains(dr.GetValue(3).ToString()))
                        {
                            parts.Add(dr.GetValue(3).ToString());
                        }
                    }
                }
                dr.Close();

                sql.CommandText = "Select top 20 ecqECQuoteID from tblECQuote where ecqPartNumber like @partNum";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    SA.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select top 10 hquHTSQuoteID from tblHTSQuote where hquPartNumbers like @partNum and hquRFQID = '' ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    htsSA.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select top 10 squSTSQuoteID from tblSTSQuote where squPartNumber like @partNum and squSTSQuoteID not in (select qtrQuoteID from linkQuoteToRFQ where qtrSTS = 1 and qtrQuoteID = squSTSQuoteID)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    stsSA.Add(dr.GetValue(0).ToString());
                }
                dr.Close();

                sql.CommandText = "Select top 10 uquUGSQuoteID from tblUGSQuote where uquPartNumber like @partNum and uquRFQID = ''";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partNum", "%" + search[i] + "%");
                dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    ugsSA.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
            }


            string user = master.getUserName();

            for (int i = 0; i < quoteID.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 1, 0, 0, GETDATE(), @user, @hts, @sts, @ugs, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", quoteID[i]);
                sql.Parameters.AddWithValue("@user", user);
                sql.Parameters.AddWithValue("@hts", hts[i]);
                sql.Parameters.AddWithValue("@sts", sts[i]);
                sql.Parameters.AddWithValue("@ugs", ugs[i]);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < mass.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 1, 0, 0, 0, GETDATE(), @user, 0, 0, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", mass[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < parts.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 0, 1, GETDATE(), @user, 0, 0, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", parts[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < noQuote.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 1, 0, GETDATE(), @user, 0, 0, 0, 0)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", noQuote[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < SA.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 0, 0, GETDATE(), @user, 0, 0, 0, 1)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", SA[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < htsSA.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 0, 0, GETDATE(), @user, 1, 0, 0, 1)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", htsSA[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < stsSA.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 0, 0, GETDATE(), @user, 0, 1, 0, 1)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", stsSA[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }
            for (int i = 0; i < ugsSA.Count; i++)
            {
                sql.CommandText = "insert into linkPartToHistory (pthPartID, pthRFQID, pthHistoryID, pthMass, pthQuote, pthNoQuote, pthPart, pthCreated, pthCreatedBy, pthHTS, pthSTS, pthUGS, pthSA) ";
                sql.CommandText += "values(@partID, @rfqID, @history, 0, 0, 0, 0, GETDATE(), @user, 0, 0, 1, 1)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@partID", partID);
                sql.Parameters.AddWithValue("@rfqID", rfqID);
                sql.Parameters.AddWithValue("@history", ugsSA[i]);
                sql.Parameters.AddWithValue("@user", user);

                master.ExecuteNonQuery(sql, "Edit RFQ");
            }

            connection.Close();
        }


        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        //protected void Edit_Click(object sender, CommandEventArgs e)
        //{
        //    if (e.CommandName == "edit")
        //    {
        //        Site master = new RFQ.Site();
        //        SqlConnection connection = new SqlConnection(master.getConnectionString());
        //        connection.Open();
        //        SqlCommand sql = new SqlCommand();
        //        sql.Connection = connection;
        //        sql.CommandText = "select prtPartNumber, prtPartDescription, prtPartTypeID, prtBlankInfoID, prtPicture, prtPartLength, prtPartWidth, prtPartHeight, prtPartMaterialType, prtPartWeight, prtPartThickness, prtNote, prtRFQLineNumber from tblPart, linkPartToRFQ   where prtPartNumber=@part and ptrRFQID=@rfq and ptrPartID=prtPartID  ";
        //        sql.Parameters.AddWithValue("@rfq", RFQID);
        //        sql.Parameters.AddWithValue("@part", e.CommandArgument);
        //        SqlDataReader dr = sql.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            txtPart.Text = dr.GetValue(0).ToString();
        //            txtDescription.Text = dr.GetValue(1).ToString();
        //            try
        //            {
        //                ddlPartType.SelectedValue = dr.GetValue(2).ToString();
        //            }
        //            catch
        //            {
        //            }
        //            try
        //            {
        //                //ddlBlankInfo.SelectedValue = dr.GetValue(3).ToString();
        //            }
        //            catch
        //            {
        //            }
        //            lblPicture.Text = "<a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/";
        //            lblPicture.Text += dr.GetValue(4).ToString() + "?rand=1' target='_blank'><img width='100' src='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/";
        //            lblPicture.Text += dr.GetValue(4).ToString() + "?rand=1' style='border: 0;'></a>";
        //            txtLength.Text = dr.GetValue(5).ToString();
        //            txtWidth.Text = dr.GetValue(6).ToString();
        //            txtHeight.Text = dr.GetValue(7).ToString();
        //            try
        //            {
        //                ddlMaterialType.SelectedValue = dr.GetValue(8).ToString();
        //            }
        //            catch
        //            {

        //            }
        //            txtWeight.Text = dr.GetValue(9).ToString();
        //            txtThickness.Text = dr.GetValue(10).ToString();
        //            txtPartNotesDia.Text = dr.GetValue(11).ToString();
        //            txtLineNumber.Text = dr.GetValue(12).ToString();
        //        }

        //        sql.Parameters.Clear();
        //        sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
        //        sql.Parameters.AddWithValue("@rfq", this.RFQID);

        //        master.ExecuteNonQuery(sql, "EditRFQ");

        //        dr.Close();
        //        connection.Close();
        //        lblMessage.Text = "\n<script>showEditDialog();</script>\n";
        //    }
        //}

        protected void addNewCustomer_Click(object sender, EventArgs e)
        {
            if (ddlCustomer.SelectedValue != "Please Select")
            {
                Site master = new RFQ.Site();
                SqlConnection connection = new SqlConnection(master.getConnectionString());
                connection.Open();
                SqlCommand sql = new SqlCommand();
                sql.Connection = connection;

                sql.CommandText = "insert into CustomerContact (CustomerID, Name, Title, OfficePhone, MobilePhone, Email, Notes, ccoCreated, ccoCreatedBy) ";
                sql.CommandText += "output inserted.CustomerContactID ";
                sql.CommandText += "values (@customerID, @name, @title, @officePhone, @mobilePhone, @email, @notes, GETDATE(), @createdBy)";
                sql.Parameters.AddWithValue("@customerID", ddlCustomer.SelectedValue);
                sql.Parameters.AddWithValue("@name", txtContactName.Text);
                sql.Parameters.AddWithValue("@title", txtContactTitle.Text);
                sql.Parameters.AddWithValue("@officePhone", txtContactOfficeNumber.Text);
                sql.Parameters.AddWithValue("@mobilePhone", txtContactMobileNumber.Text);
                sql.Parameters.AddWithValue("@email", txtContactEmail.Text);
                sql.Parameters.AddWithValue("@notes", txtCustomerContactNotes.Text);
                sql.Parameters.AddWithValue("@createdBy", master.getUserName());

                string contactID = master.ExecuteScalar(sql, "EditRFQ").ToString();

                lblMessage.Text = "\n<script>$('#newCustomerDialog').dialog('close');</script>";

                sql.Parameters.Clear();
                sql.CommandText = "Select CustomerContactID, Name from CustomerContact where CustomerID = @customer and (ccoInactive = 0 or ccoInactive is null) order by Name";
                sql.Parameters.AddWithValue("@customer", ddlCustomer.SelectedValue);
                SqlDataReader cusDR = sql.ExecuteReader();
                ddlCustomerContact.DataSource = cusDR;
                ddlCustomerContact.DataTextField = "Name";
                ddlCustomerContact.DataValueField = "CustomerContactID";
                ddlCustomerContact.DataBind();
                cusDR.Close();
                connection.Close();
                ddlCustomerContact.SelectedValue = contactID;
            }
            else
            {
                Response.Write("<script>alert('Please select a customer before adding a contact!');</script>");
            }
        }


        protected void btnSavePart_Click(object sender, EventArgs e)
        {
            Site master = new RFQ.Site();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.CommandText = "select prtPartID, prtPartNumber, prtRFQLineNumber, prtPicture from tblPart, linkPartToRFQ   where prtRFQLineNumber=@line and ptrRFQID=@rfq and ptrPartID=prtPartID";
            sql.Parameters.AddWithValue("@rfq", RFQID);
            sql.Parameters.AddWithValue("@line", hdnLineNum.Value);
            SqlDataReader dr = sql.ExecuteReader();
            Int64 UpdateRecord = 0;
            lblMessage.Text = sql.CommandText.Replace("@rfq", RFQID.ToString()).Replace("@part", txtPart.Text);
            string lineNumber = "";
            String pictureName = $"RFQ{RFQID}_{lineNumber}_{txtPart.Text.Trim()}.png";

            while (dr.Read())
            {
                UpdateRecord = System.Convert.ToInt64(dr.GetValue(0));
                //txtPart.Text = dr.GetValue(1).ToString();
                lblMessage.Text = "Part Updated";
                lineNumber = dr.GetValue(2).ToString();
                pictureName = dr.GetValue(3).ToString();
            }
            dr.Close();
            pictureName = $"RFQ{RFQID}_{lineNumber}_{txtPart.Text.Trim()}.png";


            string MaterialTypeId = "";
            sql.CommandText = "Select mtyMaterialTypeID from pktblmaterialtype where mtyMaterialType = @MaterialType";
            sql.Parameters.Clear();
            sql.Parameters.AddWithValue("@MaterialType", txtMaterialType.Text);
            dr = sql.ExecuteReader();
            if (dr.Read())
            {
                MaterialTypeId = dr["mtyMaterialTypeID"].ToString();
            }
            dr.Close();
            if (MaterialTypeId == "")
            {
                sql.CommandText = "Insert into pktblmaterialtype (mtyMaterialType) OUTPUT inserted.mtyMaterialTypeID values (@MaterialType)";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@MaterialType", txtMaterialType.Text);
                MaterialTypeId = master.ExecuteScalar(sql, "editRfq").ToString();

            }

            if (UpdateRecord == 0)
            {
                sql.CommandText = "Select max(prtRFQLineNumber) from linkPartToRFQ, tblPart where ptrRFQID = @rfq and prtPartID = ptrPartID";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@rfq", RFQID);
                dr = sql.ExecuteReader();
                int maxLineNum = 0;
                if (dr.Read())
                {
                    if (dr.GetValue(0).ToString() != "")
                    {
                        maxLineNum = System.Convert.ToInt32(dr.GetValue(0).ToString());
                    }
                }
                dr.Close();
                maxLineNum++;

                pictureName = "RFQ" + RFQID + "_" + maxLineNum + "_" + txtPart.Text.Trim() + ".png";

                sql.Parameters.Clear();
                sql.CommandText = "insert into tblPart (prtPartNumber, prtPartDescription, prtPartTypeId, prtCreated, prtCreatedBy, prtPartLength, prtPartWidth, prtPartHeight, prtPartMaterialType, prtPartWeight, prtPartThickness, prtPicture, prtNote, prtRFQLineNumber, prtAnnualVolume) ";
                sql.CommandText += " OUTPUT inserted.prtPartID ";
                sql.CommandText += " values (@part, @desc, @type, current_timestamp, @by, @length, @width, @height, @material, @weight, @thick, @pic, @note, @lineNum, @annualVolume) ";
                sql.Parameters.AddWithValue("@part", txtPart.Text.Trim());
                sql.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
                sql.Parameters.AddWithValue("@type", ddlPartType.SelectedValue);
                //sql.Parameters.AddWithValue("@blank", ddlBlankInfo.SelectedValue);
                sql.Parameters.AddWithValue("@by", Context.User.Identity.Name);
                if (txtLength.Text.Trim() == "")
                {
                    txtLength.Text = "0";
                }
                sql.Parameters.AddWithValue("@length", txtLength.Text.Trim());
                if (txtWidth.Text.Trim() == "")
                {
                    txtWidth.Text = "0";
                }
                sql.Parameters.AddWithValue("@width", txtWidth.Text.Trim());
                if (txtHeight.Text.Trim() == "")
                {
                    txtHeight.Text = "0";
                }
                sql.Parameters.AddWithValue("@height", txtHeight.Text.Trim());
                if (txtWeight.Text.Trim() == "")
                {
                    txtWeight.Text = "0";
                }
                sql.Parameters.AddWithValue("@weight", txtWeight.Text.Trim());
                if (txtThickness.Text.Trim() == "")
                {
                    txtThickness.Text = "0";
                }
                sql.Parameters.AddWithValue("@thick", txtThickness.Text.Trim());
                sql.Parameters.AddWithValue("@material", MaterialTypeId);
                sql.Parameters.AddWithValue("@pic", pictureName);
                sql.Parameters.AddWithValue("@note", txtPartNotesDia.Text);
                sql.Parameters.AddWithValue("@lineNum", maxLineNum);
                if (txtPartAnnualVolume.Text.Trim() == "")
                {
                    txtPartAnnualVolume.Text = "0";
                }
                sql.Parameters.AddWithValue("@annualVolume", txtPartAnnualVolume.Text.Trim());

                lblMessage.Text += sql.CommandText;
                UpdateRecord = System.Convert.ToInt64(master.ExecuteScalar(sql, "EditRFQ"));
                sql.CommandText = "insert into linkPartToRFQ (ptrPartID, ptrRFQID, ptrCreated, ptrCreatedBy) ";
                sql.CommandText += " values (@part, @rfq, current_timestamp, @who) ";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@part", UpdateRecord);
                sql.Parameters.AddWithValue("@rfq", RFQID);
                sql.Parameters.AddWithValue("@who", Context.User.Identity.Name);
                master.ExecuteNonQuery(sql, "EditRFQ");
            }
            else
            {
                sql.Parameters.Clear();
                sql.CommandText = "update tblPart set prtPartNumber=@number, prtPartDescription=@desc, prtNote = @note, prtPartTypeId=@type, prtModified=current_timestamp, prtModifiedBy=@by, prtPartLength=@length, ";
                sql.CommandText += "prtPartWidth=@width, prtPartHeight=@height, prtPartMaterialType=@material, prtPartWeight=@weight, prtPartThickness=@thick, prtAnnualVolume = @annualVolume ";
                sql.CommandText += "where prtPartID=@part";
                sql.Parameters.AddWithValue("@part", UpdateRecord);
                sql.Parameters.AddWithValue("@number", txtPart.Text);
                sql.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
                sql.Parameters.AddWithValue("@type", ddlPartType.SelectedValue);
                //sql.Parameters.AddWithValue("@blank", ddlBlankInfo.SelectedValue);
                sql.Parameters.AddWithValue("@by", Context.User.Identity.Name);
                sql.Parameters.AddWithValue("@note", txtPartNotesDia.Text);
                if (txtLength.Text.Trim() == "")
                {
                    txtLength.Text = "0";
                }
                sql.Parameters.AddWithValue("@length", txtLength.Text.Trim());
                if (txtWidth.Text.Trim() == "")
                {
                    txtWidth.Text = "0";
                }
                sql.Parameters.AddWithValue("@width", txtWidth.Text.Trim());
                if (txtHeight.Text.Trim() == "")
                {
                    txtHeight.Text = "0";
                }
                sql.Parameters.AddWithValue("@height", txtHeight.Text.Trim());
                if (txtWeight.Text.Trim() == "")
                {
                    txtWeight.Text = "0";
                }
                sql.Parameters.AddWithValue("@weight", txtWeight.Text.Trim());
                if (txtThickness.Text.Trim() == "")
                {
                    txtThickness.Text = "0";
                }
                sql.Parameters.AddWithValue("@thick", txtThickness.Text.Trim());
                sql.Parameters.AddWithValue("@material", MaterialTypeId);
                if (txtPartAnnualVolume.Text.Trim() == "")
                {
                    txtPartAnnualVolume.Text = "0";
                }
                sql.Parameters.AddWithValue("@annualVolume", txtPartAnnualVolume.Text.Trim());

                master.ExecuteNonQuery(sql, "EditRFQ");
            }
            // Upload image into SharePoint
            String FileName = "";
            try
            {
                FileName = filePicture.PostedFile.FileName;
            }
            catch
            {

            }
            if (FileName != "")
            {
                ClientContext ctx = new ClientContext("https://toolingsystemsgroup.sharepoint.com/sites/Estimating");
                ctx.Credentials = master.getSharePointCredentials();
                Web web = ctx.Web;
                ctx.Load(web);
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                Microsoft.SharePoint.Client.List partPicturesList = web.Lists.GetByTitle("Part Pictures");
                byte[] fileData = null;
                using (var binaryReader = new System.IO.BinaryReader(filePicture.PostedFile.InputStream))
                {
                    fileData = binaryReader.ReadBytes((int)filePicture.PostedFile.InputStream.Length);
                }
                System.IO.MemoryStream newStream = new System.IO.MemoryStream(fileData);
                FileCreationInformation newFile = new FileCreationInformation();
                newFile.ContentStream = newStream;
                newFile.Url = "https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part Pictures/" + pictureName;
                newFile.Overwrite = true;
                Microsoft.SharePoint.Client.File file = partPicturesList.RootFolder.Files.Add(newFile);
                partPicturesList.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                // set the Attributes
                Microsoft.SharePoint.Client.ListItem newItem = file.ListItemAllFields;
                newItem["Title"] = txtPart.Text;
                newItem["PartID"] = UpdateRecord;
                newItem["PartNumber"] = txtPart.Text;
                newItem["PartDescription"] = txtDescription.Text;
                newItem["RFQ"] = "https://tsgrfq.azurewebsites.net/EditRFQ?id=" + RFQID;
                newItem.Update();
                //SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);
                SpQuery.ExecuteQueryWithIncrementalRetry(ctx, 5, 30000);

                sql.CommandText = "update tblPart set prtPicture = @picture, prtModified = GETDATE(), prtModifiedBy = @user where prtPartID = @part";
                sql.Parameters.Clear();
                sql.Parameters.AddWithValue("@user", master.getUserName());
                sql.Parameters.AddWithValue("@picture", pictureName);
                sql.Parameters.AddWithValue("@part", UpdateRecord);
                master.ExecuteNonQuery(sql, "EditRFQ");
            }

            sql.Parameters.Clear();
            sql.CommandText = "Update tblRFQ set rfqCheckBit = 1 where rfqID = @rfq";
            sql.Parameters.AddWithValue("@rfq", this.RFQID);

            master.ExecuteNonQuery(sql, "EditRFQ");

            connection.Close();
            lblMessage.Text = "<script>alert('" + lblMessage.Text + "');</script>";

            Response.Redirect(Request.RawUrl);
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            populate_Plants();
        }

        // Need to put a unique ID on each TR element so that I can place another TR element underneath it via jQuery
        protected void dgParts_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                // get the Part Number, that will make the id unique
                Label BackGroundColor = (Label)e.Item.FindControl("lblBackGroundColor");
                e.Item.BackColor = System.Drawing.Color.FromName(BackGroundColor.Text.Trim());
                Label PartID = (Label)e.Item.FindControl("PartID");
                DataGridItem row = e.Item;
                row.Attributes["id"] = "line" + PartID.Text.ToString();
            }
        }

        protected void ddlPlant_SelectedIndexChanged(object sender, EventArgs e)
        {
            setSalesmanAndRank();
        }

        protected void setSalesmanAndRank()
        {
            Site master = new RFQ.Site();
            IsMasterCompany = master.getMasterCompany();
            UserCompanyID = master.getCompanyId();
            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            lblSalesman.Text = "Not Set";
            lblRank.Text = "Not Set";
            sql.Parameters.Clear();
            sql.CommandText = "Select Rank, ps.Name as PrimarySalesman, ss.Name as SecondarySalesman from CustomerLocation cl ";
            sql.CommandText += "join CustomerRank cr on cl.CustomerRankID = cr.CustomerRankID ";
            sql.CommandText += "join TSGSalesman ps on ps.TSGSalesmanID = cl.TSGSalesmanID ";
            sql.CommandText += "left outer join linkSalesmanToCustomerLocation lstcl on cl.CustomerLocationID = lstcl.sclCustomerLocationId ";
            sql.CommandText += "left outer join TSGSalesman ss on ss.TSGSalesmanID = lstcl.sclSalesmanId ";
            sql.CommandText += "where cl.CustomerLocationID = @plant";
            sql.Parameters.AddWithValue("@plant", ddlPlant.SelectedValue);
            SqlDataReader dr = sql.ExecuteReader();
            while (dr.Read())
            {
                lblRank.Text = dr["Rank"].ToString();
                lblSalesman.Text = dr["PrimarySalesman"].ToString();
                if (dr["SecondarySalesman"].ToString() != "")
                {
                    lblSalesman.Text += ", " + dr["SecondarySalesman"].ToString();
                }
            }
            dr.Close();
            connection.Close();
        }

        protected void sendNoQuotesToCustomer(object sender, EventArgs e)
        {
            Site master = new Site();

            SmtpClient server = new SmtpClient("smtp.office365.com");
            server.UseDefaultCredentials = false;
            server.Port = 587;
            server.EnableSsl = true;
            // TODO send as another user
            server.Credentials = master.getNetworkCredentials();
            server.Timeout = 120000;
            server.TargetName = "STARTTLS/smtp.office365.com";
            MailMessage mail = new MailMessage();
            mail.From = master.getFromAddress();
            var to = txtNoQuoteTo.Text.Trim().Split(',');
            foreach (var address in to)
            {
                if (!string.IsNullOrWhiteSpace(address) && !mail.To.Contains(new MailAddress(address)))
                {
                    mail.To.Add(new MailAddress(address));
                }
            }
            mail.CC.Add(new MailAddress("jdalman@toolingsystemsgroup.com"));
            var cc = txtNoQuoteCC.Text.Trim().Split(',');
            foreach (var address in cc)
            {
                if (!string.IsNullOrWhiteSpace(address) && !mail.CC.Contains(new MailAddress(address)))
                {
                    mail.CC.Add(new MailAddress(address));
                }
            }

            var salesman = lblSalesman.Text.Trim().Split(',');

            SqlConnection connection = new SqlConnection(master.getConnectionString());
            connection.Open();
            SqlCommand sql = new SqlCommand();
            sql.Connection = connection;
            sql.Parameters.Clear();
            sql.CommandText += "select Email from TSGSalesman ";
            sql.CommandText += "where Name = @name";

            foreach (var name in salesman)
            {
                sql.Parameters.AddWithValue("@name", name);
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    mail.CC.Add(new MailAddress(dr["Email"].ToString()));
                }
                dr.Close();
            }

            connection.Close();



            mail.Bcc.Add("dmaguire@toolingsystemsgroup.com");
            mail.Subject = txtCusRfq.Text + txtNoQuoteSubject.Text;
            mail.Body = txtNoQuoteBody.Text.Replace("\r\n", "<br/>") + "<br/><br/>";

            mail.IsBodyHtml = true;

            server.Send(mail);
        }
    }
    public class RFQPart
    {
        public int PartIndex { get; set; }
        public string PartId { get; set; }
        public string prtPicture { get; set; }
        public byte[] PictureData { get; set; }
        public String MimeType { get; set; }
        public String LineNumber { get; set; }
        public String prtPartNumber { get; set; }
        public String prtPartDescription { get; set; }
        public String ptyPartDescription { get; set; }
        public Double Length { get; set; }
        public Double Width { get; set; }
        public Double Height { get; set; }
        public String MaterialType { get; set; }
        public Double MaterialThickness { get; set; }
        public Double Weight { get; set; }
        public String BlankDescription { get; set; }
        public String LinkPart { get; set; }
        public String BackGroundColor { get; set; }
        public String NQRHTML { get; set; }
        public String checklistHTML { get; set; }
        public String quotingHTML { get; set; }
        public String linkpartsHTML { get; set; }
        public string prtNote { get; set; }
        public int annualVolume { get; set; }
    }

    public class Quote
    {
        public int TSGCompanyID { get; set; }
        public int RFQID { get; set; }
        public int EstimatorID { get; set; }
        public int JobNumber { get; set; }
        public int PaymentTerms { get; set; }
        public int ShippingTerms { get; set; }
        public double TotalAmount { get; set; }
        public int ProductType { get; set; }
        public int ProgramCode { get; set; }
        public int OEM { get; set; }
        public int PartType { get; set; }
        public int ToolCountry { get; set; }
        public int LeadTime { get; set; }
        public string LeadTimeString { get; set; }
        public string Description { get; set; }
        public string currency { get; set; }
        public string measurement { get; set; }
    }

    public class DieInfo
    {
        public int DieType { get; set; }
        public int CavityType { get; set; }
        public double FtoBEnglish { get; set; }
        public double FtoBMetric { get; set; }
        public double LtoREnglish { get; set; }
        public double LtoRMetric { get; set; }
        public double ShutHeightEnglish { get; set; }
        public double ShutHeightMetric { get; set; }
        public string NumberOfStations { get; set; }
    }

    public class LinkList
    {
        public String LinkID { get; set; }
        public String LinkColor { get; set; }
    }
}