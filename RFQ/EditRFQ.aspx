<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditRFQ.aspx.cs" Inherits="RFQ.EditRFQ" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="jqueryui/jquery-ui.min.css" rel="stylesheet" type="text/css">
    <a href="Content/">Content/</a>
    <div style="min-height: 40px"></div>
    <center>
        <table style="background-color: lightgrey">
            <tr>
                <td colspan="4" valign="top" align="center" class="ui-widget">TSG RFQ Number:
                    <asp:Label ID="rfqNumber" runat="server" CssClass="ui-widget"></asp:Label>&nbsp;&nbsp;</td>
                <td class="ui-widget">RFQ Checklist</td>
                <td>
                    <div style="cursor: pointer" onclick="showRFQCheckList();">
                        <img id="rfqcl" src="checklist.png" width="50" height="75" />
                    </div>
                </td>
            </tr>
            <tr>
                <td class="ui-widget">Customer: </td>
                <td>
                    <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                <td class="ui-widget">Plant: </td>
                <td>
                    <asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlPlant_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                <td class="ui-widget">Status:</td>
                <td>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="ui-widget">Customer Rank: </td>
                <td>
                    <asp:Label ID="lblRank" runat="server"></asp:Label></td>
                <td class="ui-widget">TSG Salesman: </td>
                <td>
                    <asp:Label ID="lblSalesman" runat="server"></asp:Label></td>
                <td class="ui-widget">When To Send Quotes: </td>
                <td>
                    <asp:DropDownList ID="ddlHandling" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            </tr>
            <tr>
                <td class="ui-widget">Customer RFQ</td>
                <td>
                    <asp:TextBox ID="txtCustomerRFQ" onkeyup="custRFQcheck()" runat="server" CssClass="ui-widget" MaxLength="120"></asp:TextBox></td>
                <td valign="top" class="ui-widget">Date Received:</td>
                <td valign="top">
                    <asp:TextBox ID="calReceivedDate" CssClass="ui-widget datepicker" runat="server"></asp:TextBox></td>
                <td class="ui-widget">Use TSG Logo </td>
                <td>
                    <asp:CheckBox ID="cbUseTSGLogo" runat="server" /></td>
            </tr>
            <tr>
                <td class="ui-widget">Vehicle ID: </td>
                <td>
                    <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="ui-widget"></asp:DropDownList><div class="mybutton" onclick="newVehicle();" id="newVehicleButton">Add New Vehicle</div>
                </td>
                <td valign="top" class="ui-widget">Due Date:</td>
                <td valign="top">
                    <asp:TextBox ID="calDueDate" CssClass="ui-widget datepicker" runat="server"></asp:TextBox></td>
                <td class="ui-widget">Potential Turnkey </td>
                <td>
                    <asp:CheckBox ID="cbTurnkey" runat="server" Checked="False" /></td>
            </tr>
            <tr>
                <td class="ui-widget">OEM ID: </td>
                <td>
                    <asp:DropDownList ID="ddlOEM" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td valign="top" class="ui-widget">Internal Due Date: </td>
                <%--<td valign="top"><asp:Label ID="lblIntDueDate" runat="server"></asp:Label></td>--%>
                <td valign="top">
                    <asp:TextBox ID="calIntDueDate" CssClass="ui-widget datepicker" runat="server"></asp:TextBox></td>
                <td class="ui-widget">Global Program </td>
                <td valign="top">
                    <asp:CheckBox ID="cbGlobalProgram" runat="server" /></td>
            </tr>
            <tr>
                <td class="ui-widget">Program: </td>
                <td>
                    <asp:DropDownList ID="ddlProgram" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td valign="top" class="ui-widget">Bid Date:</td>
                <td valign="top">
                    <asp:TextBox ID="calBidDate" CssClass="ui-widget datepicker" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="ui-widget"></td>
                <td>
                    <div class="mybutton" onclick="newProgram();" id="newProgramButton">Add New Program</div>
                </td>
                <td valign="top" class="ui-widget">Estimated PO Date: </td>
                <td valign="top">
                    <asp:TextBox ID="calPODate" CssClass="ui-widget datepicker" runat="server"></asp:TextBox></td>
                <td class="ui-widget">Live Work</td>
                <td>
                    <asp:CheckBox ID="cbLiveWork" runat="server" /></td>
            </tr>
            <tr>
                <td class="ui-widget">Tool Country</td>
                <td>
                    <asp:DropDownList ID="ddlToolCountry" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td class="ui-widget">Product Type:</td>
                <td>
                    <asp:DropDownList ID="ddlProductType" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td class="ui-widget">Engineering Number: </td>
                <td>
                    <asp:TextBox ID="txtEngineeringNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="ui-widget">RFQ Source</td>
                <td>
                    <asp:DropDownList ID="ddlRFQSource" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td class="ui-widget">Additional RFQ Source:</td>
                <td>
                    <asp:DropDownList ID="ddlRFQSource2" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td class="ui-widget">Customer Contact:</td>
                <td>
                    <asp:DropDownList ID="ddlCustomerContact" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            </tr>

            <tr>

                <td valign="bottom" class="ui-widget">
                    <asp:Button ID="btnUpdateNotification" runat="server" CssClass="ui-widget mybutton" OnClick="sendUpdateNotification" Text="Send Update Notification" />
                </td>
                <td class="ui-widget" valign="bottom">
                    <div class="mybutton" onclick="deleteRFQ();" id="deleteRFQBut">Delete RFQ</div>
                </td>
                <td class="ui-widget" valign="bottom">
                    <div class="mybutton" onclick="newCustomerContact();" id="newCustomerContactButton">New Customer Contact</div>
                </td>
                <td class="ui-widget" valign="bottom">
                    <asp:Button CssClass="ui-widget mybutton" OnClick="unlockRFQ" runat="Server" ID="btnUnlockRFQ" Text="Unlock RFQ" />
                </td>
            </tr>
            <tr>
                <td valign="top" class="ui-widget">Notes:</td>

                <td>
                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="20" Columns="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="6" style="background-color: sandybrown; font-weight: bold; padding: 4px;">RFQ Link: 
                <asp:HyperLink runat="server" ID="hlRFQLink" Target="_blank"></asp:HyperLink>

                </td>
            </tr>
            <tr>
                <td colspan="6" style="align-self: center">Created:
                    <asp:Label ID="create" runat="server" CssClass="ui-widget"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                Modified:
                    <asp:Label ID="modify" runat="server" CssClass="ui-widget"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <asp:Label ID="lblNotified" runat="server" CssClass="ui-widget"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ui-widget">Number of Parts: </td>
                <td>
                    <asp:Label ID="lblNumberOfParts" runat="server" CssClass="ui-widget"></asp:Label></td>
                <td class="ui-widget" hidden="hidden">Number of Tools:</td>
                <td>
                    <asp:Label ID="lblNumberOfTools" runat="server" CssClass="ui-widget" Visible="false"></asp:Label></td>
            </tr>
            <tr id="notificationTR">
                <td colspan="6" style="background-color: sandybrown; font-weight: bold; padding: 4px;">
                    <div id="sendNotificationsMessage"></div>
                    <div style='float: left;'>
                        Notification:
                        <div onclick="reSendNotifications();" class="ui-widget mybutton" style='float: right;'>Re-Send Notifications</div>
                        <div onclick="sendNotifications();" class="ui-widget mybutton" style='float: right;'>Send Notifications</div>
                    </div>
                    <div>&nbsp;</div>
                    <div>&nbsp;</div>
                    <asp:Label ID="lblNotificationCheckList" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center">
                    <asp:Button ID="btnSave_Click" runat="server" Text="Save" CssClass="ui-widget mybutton" OnClick="btnSave_Click_Click" /></td>
            </tr>
        </table>
    </center>
    <hr />
    <center>
        <asp:Button ID="hdnImportParts" runat="server" Style="visibility: hidden;" Text="Import Parts" CssClass="ui-widget mybutton" OnClick="btnImport_Click" />
        <asp:Button ID="htnAttachmentUpload" runat="server" Style="visibility: hidden;" Text="Upload Parts" CssClass="ui-widget mybutton" OnClick="importFiles_click" />
        <asp:FileUpload ID="uploadFile" runat="server" AllowMultiple="true" Style="opacity: 0; visibility: hidden;" />
        <asp:FileUpload ID="fileUpload" runat="server" Style="opacity: 0; visibility: hidden;" onChange='$("#MainContent_hdnImportParts").click();' />

        <label class="ui-widget">Sales Buttons</label>
        <br />

        <button class="mybutton" onclick="downloadSummary();">Quote Summary (Excel)</button>
        <button class="mybutton" onclick="downloadPartSummary();">Part Summary (Excel)</button>
        <asp:Button ID="viewAllQuotesButton" runat="server" Text="View All Quotes" CssClass="ui-widget mybutton" OnClientClick="viewAllQuotes();" />
        <asp:Button ID="downloadCompanyQuotesButton" runat="server" Text="Download my Company's PDFs" CssClass="ui-widget mybutton" OnClick="btnDownloadCompanyQuotes_Click" />
        <br />
        <div class="mybutton" onclick="openSendQuoteDialog();" id="btnSendQuoteDialog">Send Quotes To Customer</div>
        <asp:Button ID="downloadAllQuotesButton" runat="server" Text="Download All Quotes PDFs" CssClass="ui-widget mybutton" OnClick="btnDownloadQuotes_Click" />
        <asp:Button ID="viewOnlyMyQuotesButton" runat="server" Text="View My Companies Quotes (1 PDF)" CssClass="ui-widget mybutton" OnClientClick="onlyMyCompaniesQuoteOnePDF();" />
        <div class="mybutton" onclick="openNoQuotesDialog();" id="btnShowNoQuotesDialog">View No Quote Reasons</div>

        <hr />

        <label class="ui-widget">Data Cordinator Buttons</label>
        <br />
        <div class="mybutton" id="btnImport" runat="server" onclick="importParts();">Import Parts</div>
        <div class="mybutton" onclick="showAddPart();" id="addButton">Add Part</div>
        <br />
        <div class="mybutton" onclick="removeAllParts();" id="removeAllButton">Remove All Parts</div>
        <asp:Button ID="btnRemoveHistory" runat="server" OnClick="btnDeleteAllHistory" CssClass="ui-widget mybutton" Text="Remove All History" />
        <br />
        <div class="mybutton" id="btnSTSRfqInfo" onclick="populateSTSRFQDialog();return false;">STS RFQ Info</div>
        <%--<asp:Button runat="server" CssClass="mybutton" OnClick="sendNoQuotesToCustomer" Text="Send No Quotes to Customer" ID="btnSendNoQuotesToCustomer" />--%>
        <div class="mybutton" onclick="openSendNoQuoteDialog();" id="btnSendNoQuoteDialog">Send No Quotes to Customer</div>
        <%--<asp:Button runat="server" CssClass="mybutton" OnClientClick="openSendNoQuoteDialog()" Text="Send No Quotes to Customer" ID="btnSendNoQuotesToCustomer" />--%>


        <hr />
        <label class="ui-widget">Estimator Buttons</label>
        <br />
        <div class="mybutton" onclick="uploadQuote();" id="quoteUploadButton">Upload Quote</div>
        <div class="mybutton" onclick="showNoReason();" id="nqRemainingPartsDiv">No Quote Remaining Parts</div>
        <div class="mybutton" onclick="showRemoveNoQuotes();" id="removeNoQuotesAllPartsDiv">Remove No Quotes All Parts</div>
        <br />
        <div class="mybutton" onclick="reserveAllParts();" id="reserveAllPartsDiv">Reserve All Remaining Parts</div>
        <asp:Button ID="btnRemoveAllReservations" runat="server" OnClick="removeAllReservations" CssClass="ui-widget mybutton" Text="Remove All Reservations" />
        <div class="mybutton" onclick="downloadNewQuoteSheet();" id="newQuoteSheet">New Parts Quote Sheet</div>
        <div class="mybutton" onclick="downloadQuoteSheet();" id="quoteSheet">Quote Sheet (Excel)</div>
        <div class="mybutton" onclick="downloadQuoteSheet();" id="STSquoteSheet">STS Quote Engine Sheet (Excel)</div>
        <%--<div class="mybutton" onclick="downloadUGSQuoteSheet();" id="UGSQuoteSheet">UGS Quote Sheet</div>--%>
        <br />
        <div class="mybutton" onclick="openAssembly();" id="btnCreateAssembly">Add Assembly</div>
        <div class="mybutton" onclick="ugsMultiQuote();" id="btnUGSMultiQuote">UGS Multi Quote</div>
        <div class="mybutton" onclick="ugsSummary();" id="btnUGSSummary">UGS Quote Summary</div>
        <br />
        <div class="mybutton" onclick="hideAllParts();showReservedParts();" id="btnShowOnlyReservedParts">Show Only Reserved Parts</div>
        <div class="mybutton" onclick="showAllParts();hideReservedParts();" id="btnShowUnreservedParts">Show Only Unreserved Parts</div>
        <div class="mybutton" onclick="showAllParts();showReservedParts();" id="btnShowAllParts">Show All Parts</div>
        <div class="mybutton" onclick="hideAllHistory();" id="btnHideAllHistory">Hide History</div>
        <div class="mybutton" onclick="showAllHistory();" id="btnShowAllHistory">Show History</div>

    </center>
    <center>
        <asp:DataGrid ID="dgParts" runat="server" AutoGenerateColumns="false" OnItemDataBound="dgParts_ItemDataBound" Width="1600px">
            <Columns>
                <asp:TemplateColumn Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="lblBackGroundColor" Text='<%# Eval("BackGroundColor") %>' runat="server"></asp:Label>
                        <asp:Label ID="PartID" Text='<%# Eval("PartId") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Edit" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:HyperLink ID="btnEdit" runat="server" ImageUrl="~/edit.png" onclick="editPart(this.id, $(this).closest('tr').attr('id'));" Style="cursor: pointer;"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Line" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="LineNumber" runat="server" Text='<%# Eval("LineNumber") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Picture">
                    <ItemTemplate>
                        <span class="SharepointLogin">Be sure you are logged in to Sharepoint</span>
                        <a href='<%# Eval("prtPicture") %>' target="_blank">
                            <asp:Image ID="imgPart" runat="server" ImageUrl='<%# Eval("prtPicture") %>' onerror="imgError(this)" CssClass="PartPic" Width="310px" Height="230px" />
                        </a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Part" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox runat="server" ID="PartNumber" Text='<%# Eval("prtPartNumber") %>' Wrap="true" ReadOnly="true" Rows="11" BackColor="Transparent" BorderStyle="None" TextMode="MultiLine" Width="300px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Description" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox runat="server" ID="PartDescription" Text='<%# Eval("prtPartDescription") %>' Wrap="true" ReadOnly="true" BackColor="Transparent" Rows="11" BorderStyle="None" TextMode="MultiLine" Width="300px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Part Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PartType" Text='<%# Eval("ptyPartDescription") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Length" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PartLength" Text='<%# Eval("Length") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Width" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PartWidth" Text='<%# Eval("Width") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Height" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PartHeight" Text='<%# Eval("Height") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Material Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="MaterialType" Text='<%# Eval("MaterialType") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Thickness" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Thickness" Text='<%# Eval("MaterialThickness") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Weight" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="PartWeight" Text='<%# Eval("Weight") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Annual Volume" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="AnnualVolume" Text='<%# Eval("annualVolume") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <%--            <asp:TemplateColumn HeaderText="Part Check List" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="checklist" runat="server" Text='<%# Eval("checklistHTML") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateColumn>--%>
                <asp:TemplateColumn HeaderText="Quoting" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="linkquotebutton" runat="server" Text='<%# Eval("quotingHTML") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Link Parts" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label ID="linkpartsbutton" runat="server" Text='<%# Eval("linkpartsHTML") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Part Note" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox runat="server" ID="txtPartNote" Text='<%# Eval("prtNote") %>' Wrap="true" ReadOnly="true" Rows="11" BackColor="Transparent" BorderStyle="None" TextMode="MultiLine" Width="200px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </center>
    <div id="FindPartDialog" style="display: none; padding: 20px; background-color: #D0D0D0; width: 1600px; height: 800px;">
        <asp:HiddenField ID="hdnPartID" Value="0" runat="server" />
        <div style="float: left;">
            <label class="ui-widget">Part Number: </label>
            <br />
            <input type="text" class="ui-widget-content" id="txtFindPartNumber" value="" />
        </div>
        <div style="float: left;">
            <label class="ui-widget">Customer: </label>
            <br />
            <input type="text" class="ui-widget-content" id="txtFindPartCustomer" value="" />
        </div>
        <div style="float: left;">
            <label class="ui-widget">Part Desc: </label>
            <br />
            <input type="text" class="ui-widget-content" id="txtFindPartDesc" value="" />
        </div>
        <div style="float: left;">
            <label class="ui-widget">Quote Number: </label>
            <br />
            <input type="text" class="ui-widget-content" id="txtQuoteNumber" value="" />
        </div>
        <div style="float: left;">&nbsp;</div>
        <br />
        <br />
        <br />
        <div style="float: left;">
            <label class="ui-widget">Customer RFQ #: </label>
            <br />
            <asp:TextBox ID="txtCustomerRFQNumber" CssClass="ui-widget" runat="server" value=""></asp:TextBox>
        </div>
        <div style="float: left;">
            <label class="ui-widget">Start Date: </label>
            <br />
            <asp:TextBox ID="txtFindStartDate" CssClass="ui-widget datepicker" runat="server" value=""></asp:TextBox>
        </div>
        <div style="float: left;">
            <label class="ui-widget">End Date: </label>
            <br />
            <asp:TextBox ID="txtFindEndDate" CssClass="ui-widget datepicker" runat="server" value=""></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <br />
        <br />
        <br />
        <div style="float: right;">
            <input type="button" class="mybutton" onclick="fineNewPartsNoLink();" value="Find" />
        </div>
        <div style="clear: both;"></div>
        <div id="tblFindResults"></div>
        <div style="float: right;">
            <input type="button" class="mybutton" onclick="processHistoryResults(); hideFindPartDialog();" value="Apply" />
        </div>
    </div>
    <div id="LinkPartsDialog" style="display: none; background-color: #D0D0D0;">
    </div>
    <div id="addAssemblyDialog" style="display: none; background-color: #D0D0D0;">
        <center>
            <h3>Add Assembly</h3>
            <br />
            <table>
                <tr>
                    <td>Assembly Number
                    </td>
                    <td>
                        <asp:TextBox ID="txtAssemblyNum" runat="server" CssClass="ui-widget"></asp:TextBox>
                    </td>
                    <td>Assembly Description
                    </td>
                    <td>
                        <asp:TextBox ID="txtAssemblyDescription" runat="server" CssClass="ui-widget"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>Assembly Type
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAssemblyType" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </td>
                    <td>
                        <%--Upload Picture--%>
                    </td>
                    <td>
                        <%--<asp:FileUpload ID="assemblyPictureUpload" runat="server" />--%>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <h3>Available Parts</h3>
            <br />
            <asp:Label ID="lblAssemblyTable" runat="server"></asp:Label>

            <div id="btnSaveAssembly" class="ui-widget mybutton" onclick="createAssembly();">Create Assembly</div>
            <div id="btnDeleteAssembly" class="ui-widget mybutton" onclick="deleteAssembly();">Delete Assembly</div>
        </center>
    </div>
    <div id="SendQuotesDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <%--<center>
                <asp:label class="ui-widget" ID="lblEmailCustomerAddress" runat="server">Sending all quotes to -</asp:label><br />
            </center>--%>
        <asp:CheckBox ID="cbSendAsMe" runat="server" Text="Send email as me" /><br />
        <asp:CheckBox ID="cbSendOnlyMyQuotes" onchange="onlyMyCompanies();" runat="server" Text="Send only my company's quotes (All Quotes from your company)" /><br />
        <asp:CheckBox ID="cbSendUpdatedQuotes" onchange="onlyMyCompaniesNew();" runat="server" Text="Send only my company's new quotes (Only quotes that have not been sent yet)" Checked="true" /><br />
        <asp:CheckBox ID="cbSendAll" onchange="allQuotesForRFQ();" runat="server" Text="Send all quotes for this RFQ" /><br />
        <asp:CheckBox ID="cbIndividualPDF" runat="server" Text="Split into individual PDFs" /><br />
        <label class="ui-widget">Email addresses to send the quotes to? (Seperate by comma)</label><br />
        <asp:TextBox ID="txtExtraEmail" runat="server" ReadOnly="false" CssClass="ui-widget" Style="width: 350px;" autocomplete="off"></asp:TextBox>
        <br />
        <label class="ui-widget">Any other email address to cc? (Seperate by comma)</label><br />
        <asp:TextBox ID="txtccEmail" runat="server" ReadOnly="false" CssClass="ui-widget" Style="width: 350px;" autocomplete="off"></asp:TextBox>
        <br />
        <label class="ui-widget">Any other email address to bcc? (Seperate by comma)</label><br />
        <asp:TextBox ID="txtbccEmail" runat="server" ReadOnly="false" CssClass="ui-widget" Style="width: 350px;" autocomplete="off"></asp:TextBox>
        <br />
        <label class="ui-widget">Email Subject</label><br />
        <asp:TextBox ID="txtSubject" runat="server" ReadOnly="false" CssClass="ui-widget" Style="width: 350px;"></asp:TextBox>
        <br />
        <label class="ui-widget">What would you like the message to say?</label><br />
        <asp:TextBox ID="txtMessageText" TextMode="multiline" runat="server" ReadOnly="false" CssClass="ui-widget" Style="width: 100%;" Height="200px" ColSpan="4" Width="600px"></asp:TextBox><br />
        <br />
        <br />
        <label class="ui-widget">Upload attachments to quote (Screen will auto refresh when the files have uploaded)</label>
        <asp:FileUpload ID="attachmentUpload" runat="server" AllowMultiple="true" onChange='$("#MainContent_htnAttachmentUpload").click();' />
        <asp:HyperLink runat="server" ID="hlquoteAttachment" Target="_blank"></asp:HyperLink>
        <br />
        <asp:Label ID="lblUploadsToEmail" runat="server"></asp:Label>
        <br />
        <br />
        <div class="mybutton" onclick="sendQuotes();" id="btnSendQuotes">Send Quotes To Customer</div>
    </div>
    <div id="sendNoQuoteDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <label>Email addresses to send no quote to (Seperated by comma)</label><br />
        <asp:TextBox ID="txtNoQuoteTo" runat="server" CssClass="ui-widget" Style="width: 600px; max-width: 600px;"></asp:TextBox>
        <br />
        <label>Email addresses to CC (Seperated by comma)</label><br />
        <asp:TextBox ID="txtNoQuoteCC" runat="server" CssClass="ui-widget" Style="width: 600px; max-width: 600px;"></asp:TextBox>
        <br />
        <label>Customer RFQ</label><br />
        <asp:TextBox ID="txtCusRfq" runat="server" CssClass="ui-widget" Style="width: 600px; max-width: 600px;"></asp:TextBox>
        <br />
        <label>Subject</label><br />
        <asp:TextBox ID="txtNoQuoteSubject" runat="server" CssClass="ui-widget" Style="width: 600px; max-width: 600px;"></asp:TextBox>
        <br />
        <label>Body</label><br />
        <asp:TextBox ID="txtNoQuoteBody" runat="server" TextMode="MultiLine" CssClass="ui-widget" Style="max-width: 600px;" Height="200px" ColSpan="4" Width="600px"></asp:TextBox>

        <br />
        <br />
        <asp:Button runat="server" CssClass="mybutton" OnClick="sendNoQuotesToCustomer" Text="Send No Quotes to Customer" ID="btnSendNoQuotesToCustomer" />
    </div>
    <div id="NoQuoteReasonDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <label class="ui-widget">Applies To: </label>
        <br />
        <asp:TextBox ID="txtNQRAppliesTo" runat="server" ReadOnly="true" CssClass="ui-widget"></asp:TextBox>
        <br />
        <br />
        Reason<br />
        <asp:DropDownList ID="ddlNoQuoteReason" runat="server"></asp:DropDownList>
        <br />
        <br />
        <button class="mybutton" onclick="ApplyNoQuote();">Apply</button>
    </div>
    <div id="RemoveNoQuoteDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <label class="ui-widget">Applies To: </label>
        <br />
        <asp:TextBox ID="txtRemoveNQAppliesTo" runat="server" ReadOnly="true" CssClass="ui-widget"></asp:TextBox>
        <br />
        <br />
        Pleased Click Confirm if you really want to remove no quote(s) for your company.<br />
        <button class="mybutton" onclick="RemoveNoQuote();">Confirm</button>
    </div>
    <div id="noQuoteReasonsDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <asp:TextBox ID="txtNoQuoteText" runat="server" ReadOnly="true" BorderStyle="None" TextMode="MultiLine" Rows="20" Columns="20" Width="400"></asp:TextBox>
        <%--<asp:Literal ID="textNoQuoteTXT" runat="server" ></asp:Literal>--%>
    </div>
    <div id="EditPartDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <div style="float: left;">
            <label class="ui-widget">Part: </label>
            <br />
            <asp:TextBox ID="txtPart" runat="server" CssClass="ui-widget" Width="250"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Line number: </label>
            <br />
            <asp:TextBox ID="txtLineNumber" runat="server" CssClass="ui-widget" Width="50" ReadOnly="true"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Description: </label>
            <br />
            <asp:TextBox ID="txtDescription" runat="server" CssClass="ui-widget" Width="500"></asp:TextBox>
        </div>
        <div style="clear: both;">
        </div>
        <div style="float: left;">
            <label class="ui-widget">Part Type: </label>
            <br />
            <asp:DropDownList runat="server" ID="ddlPartType"></asp:DropDownList>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Length &nbsp&nbsp&nbsp&nbsp x &nbsp&nbsp&nbsp&nbsp Width &nbsp&nbsp&nbsp&nbsp x &nbsp&nbsp&nbsp&nbsp Height &nbsp&nbsp&nbsp&nbsp Annual Volume</label><br />
            <asp:TextBox ID="txtLength" runat="server" CssClass="ui-widget" Width="80"></asp:TextBox>
            x 
                <asp:TextBox ID="txtWidth" runat="server" CssClass="ui-widget" Width="80"></asp:TextBox>
            x 
                <asp:TextBox ID="txtHeight" runat="server" CssClass="ui-widget" Width="80"></asp:TextBox>&nbsp&nbsp&nbsp
                <asp:TextBox ID="txtPartAnnualVolume" runat="server" CssClass="ui-widget" Width="120"></asp:TextBox>
        </div>
        <div style="clear: both;">
        </div>
        <div style="float: left;">
            <label class="ui-widget">Material Type: </label>
            <br />
            <asp:TextBox ID="txtMaterialType" runat="server"></asp:TextBox><br />
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Weight: </label>
            <br />
            <asp:TextBox ID="txtWeight" runat="server" CssClass="ui-widget" Width="80"></asp:TextBox><br />
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Thickness: </label>
            <br />
            <asp:TextBox ID="txtThickness" runat="server" CssClass="ui-widget" Width="80"></asp:TextBox><br />
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Part Notes: </label>
            <br />
            <asp:TextBox ID="txtPartNotesDia" runat="server" CssClass="ui-widget" Width="300" TextMode="MultiLine" Rows="3"></asp:TextBox>
        </div>
        <div style="float: left;">
            <label class="ui-widget">Picture: </label>
            <br />
            <asp:FileUpload ID="filePicture" runat="server" />
            <asp:Label ID="lblPicture" runat="server"></asp:Label>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: right;">
            <asp:Button ID="btnSavePart" runat="server" Text="Save" CssClass="mybutton" OnClick="btnSavePart_Click" />
            <asp:Button ID="deletePartButton" runat="server" Text="Delete Part" CssClass="ui-widget mybutton" OnClick="deletePart_click" Visible="true" />
            <asp:Button ID="duplicatePart" runat="server" Text="Duplicate Part" CssClass="mybutton" OnClick="duplicatePart_click" />
        </div>
        <div style="clear: both;"></div>
        <asp:HiddenField ID="hdnLineNum" runat="server" />
        <asp:HiddenField ID="hdnPartNum" runat="server" />
        <asp:HiddenField ID="hdnAssemblyId" runat="server" />
        <asp:HiddenField ID="hdnNextAssemblyNum" runat="server" />
        <asp:Literal ID="litLastAssemblyId" runat="server"></asp:Literal>
    </div>

    <div id="newCustomerDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <asp:Label ID="lblCompany" runat="server"></asp:Label>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Contact Name: </label>
            <br />
            <asp:TextBox ID="txtContactName" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Contact Title: </label>
            <br />
            <asp:TextBox ID="txtContactTitle" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="clear: both;">
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Email: </label>
            <br />
            <asp:TextBox ID="txtContactEmail" runat="server" CssClass="ui-widget" Width="400"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Contact Office Number: </label>
            <br />
            <asp:TextBox ID="txtContactOfficeNumber" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Contact Mobile Number: </label>
            <br />
            <asp:TextBox ID="txtContactMobileNumber" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="clear: both;">
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="float: left">
            <label class="ui-widget">Notes: </label>
            <br />
            <td colspan="4">
                <asp:TextBox ID="txtCustomerContactNotes" TextMode="MultiLine" runat="server" CssClass="ui-widget" Rows="4" Width="800"></asp:TextBox></td>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="clear: both;">
        </div>
        <div style="float: left">
            <asp:Button ID="btnNewCustomerContact" runat="server" Text="Add New Customer" CssClass="mybutton" OnClick="addNewCustomer_Click" />
        </div>
    </div>
    <div id="newVehicleDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">New Vehicle: </label>
            <br />
            <asp:TextBox ID="txtVehicle" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="clear: both;">
        </div>
        <div style="float: left">
            <asp:Button ID="btnNewVehicle" runat="server" Text="Save" CssClass="mybutton" OnClick="addNewVehicle_Click" />
        </div>
    </div>
    <div id="newProgramDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <div style="float: left;">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">New Program: </label>
            <br />
            <asp:TextBox ID="txtNewProgram" runat="server" CssClass="ui-widget" Width="200"></asp:TextBox>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="clear: both;">
        </div>
        <div style="float: left">
            <asp:Button ID="btnNewProgram" runat="server" Text="Save" CssClass="mybutton" OnClick="addNewProgram_Click" />
        </div>
    </div>
    <div id="deleteRFQDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <div style="float: left">&nbsp;</div>
        <div style="float: left;">
            <label class="ui-widget">Are you sure you want to delete the RFQ?</label>
        </div>
        <div style="float: left;">&nbsp;</div>
        <div style="clear: both;">
        </div>
        <div style="float: left;">
            <asp:Button ID="deleteRFQButton" runat="server" Text="Delete RFQ" CssClass="ui-widget mybutton" OnClick="deleteRFQ" Visible="true" />
        </div>
    </div>
    <div id="checklistDialog" style="display: none; min-width: 600px; padding: 10px;">
        <center>
            <div id="clPart"></div>
            <asp:Label ID="lbCheckList" runat="server" Width="100%"></asp:Label>
            <hr />
            <input type="checkbox" id="allParts" class="ui-widget" />
            Apply to All Parts
                <br />
            <div class="mybutton" onclick="applyCheckList()">Save</div>
        </center>
    </div>
    <div id="RFQchecklistDialog" style="display: none;">
        <center>
            <div>
                <h4>RFQ Checklist</h4>
            </div>
            <asp:Label ID="lblRFQCheckList" runat="server" Width="100%"></asp:Label>
            <hr />
            <br />
            <div class="mybutton" onclick="applyRFQCheckList()">Save</div>
        </center>
    </div>
    <div id="DeleteAllPartsDialog" style="display: none;">
        <center>
            <div>
                <h4>Are you sure you want to delete all the parts.  This will not work if there are quotes or reservations associated with the parts.</h4>
            </div>
            <br />
            <asp:Button ID="deleteButton" runat="server" Text="Yes" CssClass="ui-widget mybutton" OnClick="deleteAllParts_click" Visible="true" />
        </center>
    </div>

    <div id="STSPartInfoDialog" style="display: none; padding: 20px; background-color: #D0D0D0; width: 1000px; height: 500px;">
        <asp:HiddenField ID="HiddenField1" Value="0" runat="server" />
        <div class="container">
            <div class="row">
                <h4>Mandatory</h4>
            </div>
            <div class="row">
                <div class="col-lg-3">Annual Volume</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtAnnualVolume" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">Production Days per Year</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtProductionDaysPerYear" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Shifts per Day</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtShiftsPerDay" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">Hours per Shift</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtHoursPerShift" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">OEE %</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtOEE" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <h4>Requested</h4>
            </div>
            <div class="row">
                <div class="col-lg-3">Award Date</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtAwardDate" runat="server" CssClass="ui-widget datepicker"></asp:TextBox>
                </div>
                <div class="col-lg-3">Runoff</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtRunoff" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Delivery Date</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="ui-widget datepicker"></asp:TextBox>
                </div>
                <div class="col-lg-3">Point of Installation</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtPointOfInstallation" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Union Workplace</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtUnionWorkplace" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">Available Data</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtAvailableData" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Available GD&%</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtAvailableGDT" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">Controls PLC</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtControlsPLC" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Robots</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtRobots" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">Welders</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtWelders" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">Positioners</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtPositioners" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
                <div class="col-lg-3">CNC Machine</div>
                <div class="col-lg-3">
                    <asp:TextBox ID="txtCNCMachine" runat="server" CssClass="ui-widget"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <center>
                        <button id="btnSaveSTSInfo" class="ui-widget mybutton" onclick="saveStsInfo();return false;">Save</button>
                    </center>
                </div>
            </div>
        </div>
    </div>

    <asp:Label ID="lblSendNotificationsScript" runat="server"></asp:Label>
    <asp:Label ID="lblReSendNotificationsScript" runat="server"></asp:Label>
    <asp:HiddenField ID="hdnQuoteToDelete" runat="server" />
    <asp:HiddenField ID="hdnCompanyID" runat="server" />
    <asp:Button ID="btnDeleteQuote" runat="server" CssClass="ui-widget mybutton" OnClick="deleteQuote_click" Style="visibility: hidden" />
    <asp:HiddenField ID="hdnReservedPartIds" runat="server" />
    <asp:HiddenField ID="hdnAllPartIds" runat="server" />

    <div id="messageDialog" style="display: none;"></div>
    <div id="NoQuoteDialog" style="display: none;"></div>
    </div>

    <asp:Literal ID="litScript" runat="server"></asp:Literal>
    <script>
        // these functions are used to detect if we can do the ajax upload or must use the old way, which will redraw the page

        var assemblyIds = [];

        function supportAjaxUploadWithProgress() {
            return supportFileAPI() && supportAjaxUploadProgressEvents() && supportFormData();

            function supportFileAPI() {
                var fi = document.createElement('INPUT');
                fi.type = 'file';
                return 'files' in fi;
            };

            function supportAjaxUploadProgressEvents() {
                var xhr = new XMLHttpRequest();
                return !!(xhr && ('upload' in xhr) && ('onprogress' in xhr.upload));
            };

            function supportFormData() {
                return !!window.FormData;
            }
        }
        var pageX = 0;
        var pageY = 0;
        // this function is always called by the footer.aspx
        function page_init() {
            $(".PartPic").one("load", function () {
                $(".SharepointLogin").hide();
            }).each(function () {
                if (this.complete) $(this).load();
            });
            $('img').click(function (e) {
                pageX = e.pageX;
                pageY = e.pageY;
            });
            $("#MainContent_uploadFile").fileupload({
                //console.log('first test');
                url: 'QuoteUpload.ashx?rfqID=' + $('#MainContent_rfqNumber').html() + '&rand=' + Math.random(),
                add: function (e, data) {
                    data.submit();
                },
                success: function (response, status) {
                    console.log('success');
                    if (response.substring(0, 2) == 'OK') {
                        // Good Response is OK|PartID|quotinghtml
                        responseParts = response.split('|');
                        document.getElementById('quoting' + responseParts[1]).innerHTML = responseParts[2];
                    } else {
                        console.log('failure');
                        alert(response);
                    }
                },
                error: function (error) {
                    // this error means the page actually errored out and you need to figure out what the error was
                    alert('Error Accessing Quote Upload Page');
                }
            });

            //This function stops the auto postback when they hit the enter key in a textbox
            $(function () {
                $(':text').bind('keydown', function (e) {
                    //on keydown for all textboxes
                    if (e.target.className != "searchtextbox") {
                        if (e.keyCode == 13) { //if this is enter key
                            e.preventDefault();
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return true;
                });
            });
        }

        function saveStsInfo() {
            var url = 'STSPartInfo.aspx';
            $.ajax({
                url: url,
                data: {
                    RFQ: $('#MainContent_rfqNumber').text(),
                    //PartID: ,
                    AnnualVolume: $('#MainContent_txtAnnualVolume').val(),
                    ProductionDaysPerYear: $('#MainContent_txtProductionDaysPerYear').val(),
                    ShiftsPerDay: $('#MainContent_txtShiftsPerDay').val(),
                    HoursPerShift: $('#MainContent_txtHoursPerShift').val(),
                    OEE: $('#MainContent_txtOEE').val(),
                    AwardDate: $('#MainContent_txtAwardDate').val(),
                    Runoff: $('#MainContent_txtRunoff').val(),
                    DeliveryDate: $('#MainContent_txtDeliveryDate').val(),
                    PointOfInstallation: $('#MainContent_txtPointOfInstallation').val(),
                    UnionWorkplace: $('#MainContent_txtUnionWorkplace').val(),
                    AvailableData: $('#MainContent_txtAvailableData').val(),
                    AvailableGDT: $('#MainContent_txtAvailableGDT').val(),
                    ControlsPLC: $('#MainContent_txtControlsPLC').val(),
                    Robots: $('#MainContent_txtRobots').val(),
                    Welders: $('#MainContent_txtWelders').val(),
                    Positioners: $('#MainContent_txtPositioners').val(),
                    CNCMachine: $('#MainContent_txtCNCMachine').val()
                },
                success: function (response, status) {
                    $('#STSPartInfoDialog').dialog('close');
                },
                failure: function (response) {
                    alert(response);
                }
            })
        }

        function populateSTSRFQDialog() {
            var url = 'STSPartInfo.aspx?RFQ=' + $('#MainContent_rfqNumber').text() + '&Get=GET';
            $.ajax({
                url: url,
                success: function (response, status) {
                    ParseSTSPartInfo(response);
                },
                failure: function (response) {
                    alert(response);
                }
            })
        }

        function ParseSTSPartInfo(data) {
            var array = data.split('/r/n');
            for (i = 0; i < array.length; i++) {
                var split = array[i].split('||');
                if (split[0] == 'AnnualVolume') {
                    $('#MainContent_txtAnnualVolume').val(split[1]);
                }
                else if (split[0] == 'ProductionDaysPerYear') {
                    $('#MainContent_txtProductionDaysPerYear').val(split[1]);
                }
                else if (split[0] == 'ShiftsPerDay') {
                    $('#MainContent_txtShiftsPerDay').val(split[1]);
                }
                else if (split[0] == 'HoursPerShift') {
                    $('#MainContent_txtHoursPerShift').val(split[1]);
                }
                else if (split[0] == 'OEE') {
                    $('#MainContent_txtOEE').val(split[1]);
                }
                else if (split[0] == 'AwardDate') {
                    $('#MainContent_txtAwardDate').val(split[1]);
                }
                else if (split[0] == 'Runoff') {
                    $('#MainContent_txtRunoff').val(split[1]);
                }
                else if (split[0] == 'DeliveryDate') {
                    $('#MainContent_txtDeliveryDate').val(split[1]);
                }
                else if (split[0] == 'PointOfInstallation') {
                    $('#MainContent_txtPointOfInstallation').val(split[1]);
                }
                else if (split[0] == 'UnionWorkplace') {
                    $('#MainContent_txtUnionWorkplace').val(split[1]);
                }
                else if (split[0] == 'AvailableData') {
                    $('#MainContent_txtAvailableData').val(split[1]);
                }
                else if (split[0] == 'AvailableGDT') {
                    $('#MainContent_txtAvailableGDT').val(split[1]);
                }
                else if (split[0] == 'ControlsPLC') {
                    $('#MainContent_txtControlsPLC').val(split[1]);
                }
                else if (split[0] == 'Robots') {
                    $('#MainContent_txtRobots').val(split[1]);
                }
                else if (split[0] == 'Welders') {
                    $('#MainContent_txtWelders').val(split[1]);
                }
                else if (split[0] == 'Positioners') {
                    $('#MainContent_txtPositioners').val(split[1]);
                }
                else if (split[0] == 'CNCMachine') {
                    $('#MainContent_txtCNCMachine').val(split[1]);
                }
            }

            $('#STSPartInfoDialog').dialog({ width: 1000, height: 500 });
        }

        function openNoQuotesDialog() {
            $('#noQuoteReasonsDialog').dialog({ width: 360, height: 550 });
        }

        function onlyMyCompanies() {
            if ($('#MainContent_cbSendOnlyMyQuotes').is(":checked")) {
                $('#MainContent_cbSendUpdatedQuotes').prop('checked', false);
                $('#MainContent_cbSendAll').prop('checked', false);
            }
        }

        function onlyMyCompaniesNew() {
            if ($('#MainContent_cbSendUpdatedQuotes').is(":checked")) {
                $('#MainContent_cbSendOnlyMyQuotes').prop('checked', false);
                $('#MainContent_cbSendAll').prop('checked', false);
            }
        }

        function allQuotesForRFQ() {
            if ($('#MainContent_cbSendAll').is(":checked")) {
                $('#MainContent_cbSendOnlyMyQuotes').prop('checked', false);
                $('#MainContent_cbSendUpdatedQuotes').prop('checked', false);
            }
        }

        //replaces broken links with the no cad see print png
        function imgError(image) {
            image.onerror = "";
            image.src = "/NO CAD - SEE PRINT.png";
            return true;
        }

        function newCustomerContact() {
            $('#newCustomerDialog').dialog({ width: 800, height: 400 });
            $('#newCustomerDialog').parent().appendTo("form");
        }

        function custRFQcheck() {
            var temp = $('#MainContent_txtCustomerRFQ').val();
            if (temp.indexOf('/') > -1 || temp.indexOf('#') > -1 || temp.indexOf('\\') > -1 || temp.indexOf('!') > -1 || temp.indexOf('$') > -1 || temp.indexOf('%') > -1 || temp.indexOf('&') > -1 || temp.indexOf(':') > -1) {
                alert('Invalid character in Customer RFQ #');
            }
        }

        function deleteRFQ() {
            $('#deleteRFQDialog').dialog({ width: 800, height: 400 });
            $('#deleteRFQDialog').parent().appendTo("form");
        }

        function newProgram() {
            $('#newProgramDialog').dialog({ width: 600, height: 400 });
            $('#newProgramDialog').parent().appendTo("form");
        }

        function newVehicle() {
            $('#newVehicleDialog').dialog({ width: 600, height: 400 });
            $('#newVehicleDialog').parent().appendTo("form");
        }

        function openSendQuoteDialog() {
            $('#SendQuotesDialog').dialog({ width: 700, height: 800 });
            $('#SendQuotesDialog').parent().appendTo("form");
        }

        function openSendNoQuoteDialog() {
            $('#sendNoQuoteDialog').dialog({ width: 700, height: 800 });
            $('#sendNoQuoteDialog').parent().appendTo('form');
        }


        function openAssembly() {
            $('#addAssemblyDialog').dialog({ width: 800, height: 800 });
            $('#addAssemblyDialog').parent().appendTo("form");

            $("#MainContent_hdnAssemblyId").val('');

            var table = '<table id="assemblySelectTable" align="center" width="100%" cellpadding="1" class="table table-striped">';
            table += "<tr><td></td><td>Line Number</td><td>Part Number</td><td>Part Description</td></tr>";

            $('#MainContent_txtAssemblyNum').val('');
            $('#MainContent_txtAssemblyDescription').val('');

            var grid = document.getElementById('MainContent_dgParts');

            grid.rows[1].id
            for (i = 1; i < grid.rows.length; i++) {
                if (grid.rows[i].id.indexOf('History') < 0 && grid.rows[i].id != '' && grid.rows[i].id.indexOf('lineA') < 0) {
                    var cell = grid.rows[i].cells;
                    var input = '<input type="checkbox" id="checkbox' + grid.rows[i].id.split('line')[1] + '"></input>';
                    var lineNum = cell[1].innerHTML;
                    var partNumber = cell[3].innerHTML.split('>')[1].split('<')[0];
                    var partDescription = cell[4].innerHTML.split('>')[1].split('<')[0];
                    table += "<tr><td>" + input + "</td><td>" + lineNum + "</td><td>" + partNumber + "</td><td>" + partDescription + "</td></tr>";
                }
            }
            table += "</table>";
            $('#MainContent_lblAssemblyTable').html(table);
        }

        function deleteAssembly() {
            var id = $('#MainContent_hdnAssemblyId').val();
            if (id != '') {
                var url = 'SaveAssembly.aspx?Delete=true&assemblyId=' + encodeURIComponent(id);
                $.ajax({
                    url: url,
                    success: function (data) {
                        $('#addAssemblyDialog').dialog('close');
                    },
                    failure: function (response) {
                        alert(response);
                    }
                });
                $('#line' + id).remove();
                var count = $('#MainContent_hdnNextAssemblyNum').val();
                count--;
                $('#MainContent_hdnNextAssemblyNum').val(count);
                assemblyIds.splice(-1, 1);
            }
        }

        function createAssembly() {
            var tableHtml = $('#assemblySelectTable').html();

            var lineNums = '';

            var rows = tableHtml.split('<tr>');

            var linkedPartNums = '';

            // Skips header
            for (i = 2; i < rows.length; i++) {
                var checkboxId = rows[i].split('id="')[1].split('"')[0];
                if ($('#' + checkboxId).prop('checked')) {
                    if (lineNums != '') {
                        lineNums += ',' + rows[i].split('<td>')[2].split('>')[1].split('</span')[0];
                        linkedPartNums += '<br />' + rows[i].split('<td>')[3].split('</td>')[0];
                    }
                    else {
                        lineNums += rows[i].split('<td>')[2].split('>')[1].split('</span')[0];
                        linkedPartNums += '<br /><br />Linked Parts<br />' + rows[i].split('<td>')[3].split('</td>')[0];
                    }

                    //lineNums.push(rows[i].split('<td>')[2].split('>')[1].split('</span')[0]);
                }
            }
            if (lineNums == '') {
                alert('Please select at least one part to create an assembly');
                return;
            }

            var assemblyNum = $('#MainContent_txtAssemblyNum').val();
            var assemblyDesc = $('#MainContent_txtAssemblyDescription').val();

            url = 'SaveAssembly.aspx?assemblyNumber=' + encodeURIComponent(assemblyNum) + '&assemblyDescription=' + encodeURIComponent(assemblyDesc) + '&lineNumbers=' + lineNums + '&rfqId=' + $('#MainContent_rfqNumber').html();

            if ($("#MainContent_hdnAssemblyId").val() != '') {
                url += '&assemblyId=' + encodeURIComponent($('#MainContent_hdnAssemblyId').val());
            }

            // I think you have to do something like this to send the picture data
            //var formData = new FormData(this);
            //formData.append("Picture", $('#MainContent_assemblyPictureUpload')[0].files[0]);

            $.ajax({
                url: url,
                success: function (data) {
                    $('#addAssemblyDialog').dialog('close');
                    if ($('#MainContent_hdnAssemblyId').val() == '') {
                        var count = $('#MainContent_hdnNextAssemblyNum').val();
                        var newLineHtml = '<tr id="lineA' + data + '"><td><asp:HyperLink ID="btnEdit" runat="server" ImageUrl="~/edit.png" onclick="editPart(this.id, $(this).closest(\'tr\').attr(\'id\'));" style="cursor: pointer;"></asp:HyperLink></td><td>A' + count + '</td><td>';
                        newLineHtml += '<img id="MainContent_dgParts_imgPart_0" class="PartPic" onerror="imgError(this)" src="https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/RFQ358_22_51326-45010.png" style="height:230px;width:310px;"></td>';
                        newLineHtml += "<td>" + assemblyNum + linkedPartNums + "</td><td>" + assemblyDesc + "</td><td>Assembly</td><td></td><td></td><td></td><td></td><td></td><td></td><td></td><td><div id='quotingA" + data + "' >" + "<input type='button' class='mybutton' value='Reserve' onClick=\"reservePart('A" + data + "');return false;\" ></div></td><td></td><td></td></tr>";

                        if (assemblyIds.length - 1 < 0) {
                            $('#MainContent_dgParts tbody tr:first').after(newLineHtml);
                        }
                        else {
                            $('#line' + assemblyIds[assemblyIds.length - 1]).after(newLineHtml);
                        }
                        count++;
                        $('#MainContent_hdnNextAssemblyNum').val(count);
                        assemblyIds.push('A' + data);
                        //$('#MainContent_litLastAssemblyId').val('A' + data);
                        $('.mybutton').button();
                    }
                },
                failure: function (response) {
                    alert(response);
                }
            });
        }

        function ProcessUploadPic() {
            if (document.getElementById("inputPic").files.length === 0) {
                alert("Select a file!");
                return;
            }
            var parts = document.getElementById("inputPic").value.split("\\");
            var filename = parts[parts.length - 1];
            var fileInput = document.getElementById("inputPic").files[0];
            var picReader = new FileReader();
            picReader.addEventListener("load", function (event) {
                var picFile = event.target;
                var div = document.createElement("div");
                div.innerHTML = "<img class='thumbnail' src='" + picFile.result + "'" + "title='" + picFile.name + "'/>";
                PerformUploadPic(filename, div)
            });
            picReader.readAsDataURL(fileInput);
        }
        function PerformUploadPic(filename, fileData) {
            var url = document.URL.split('/');
            url = url[0] + "//" + url[2] + "/" + url[3] + "/";
            $.ajax({
                url: url + "_api/web/getfolderbyserverrelativeurl('Image')/files/add(url='" + filename + "', overwrite=true)",
                method: "POST",
                binaryStringRequestBody: true,
                body: fileData,
                headers: {
                    "accept": "application/json; odata=verbose",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "content-length": fileData.byteLength
                },
                success: function (data) {
                    alert("Success! Your Picture was uploaded to SharePoint.");
                },
                error: function onQueryErrorAQ(xhr, ajaxOptions, thrownError) {
                    alert('Error:\n' + xhr.status + '\n' + thrownError + '\n' + xhr.responseText);
                },
                state: "Update"
            });
        }

        function sendQuotes() {
            var sendAsMe = false;
            if (document.getElementById("MainContent_cbSendAsMe") != null) {
                sendAsMe = document.getElementById("MainContent_cbSendAsMe").checked;
            }
            url = 'Disposition?Message=' + encodeURIComponent($('#MainContent_txtMessageText').val()) + '&rfq=' + $('#MainContent_rfqNumber').html() + '&cc=' + $('#MainContent_txtccEmail').val() +
                '&bcc=' + $('#MainContent_txtbccEmail').val() + '&subject=' + $('#MainContent_txtSubject').val() + '&emails=' + $('#MainContent_txtExtraEmail').val() + '&company=' + $('#MainContent_hdnCompanyID').val() +
                '&all=' + document.getElementById("MainContent_cbSendAll").checked + '&updated=' + document.getElementById("MainContent_cbSendUpdatedQuotes").checked + '&individual=' + document.getElementById("MainContent_cbIndividualPDF").checked +
                '&me=' + sendAsMe;
            $.ajax({
                url: url,
                success: function (data) { },
                failure: function (response) {
                    alert(response);
                }
            });
            $('#SendQuotesDialog').dialog('close');
        }

        function sendNoQuotes() {
            var customerContact = document.getElementById("MainContent_ddlCustomerContact");
            var customerName = customerContact.options[customerContact.selectedIndex].value;
            url = 'Disposition?&rfq=' + $('#MainContent_rfqNumber').html() + '&Cusrfq=' + $('#MainContent_txtCusRfq').val() + '&noquote=true' + '&customer=' + customerName;
            $.ajax({
                url: url,
                success: function (data) { },
                failure: function (response) {
                    alert(response);
                }
            });
            $('#SendNoQuotesDialog').dialog('close');
        }

        function showMessage(msg) {
            $('#messageDialog').html(msg);
            $('#messageDialog').dialog();
        }

        function importParts() {
            $('#MainContent_fileUpload').click();
        }

        function uploadQuote() {
            $('#MainContent_uploadFile').click();
        }

        var clPartID = '';
        function showCheckList(part) {
            clPartID = part;
            $('#clPart').html('<B>' + part + '</b>');
            url = 'processPartCheckList?get=1&rfq=' + $('#MainContent_rfqNumber').html() + '&part=' + clPartID;
            $.ajax({ url: url, success: function (data) { processPartCheckList(data); } });
        }

        function processPartCheckList(data) {
            data = data.trim();
            $('.lbCheckListOption').each(function () {
                this.checked = false;
            }
            );
            if (data != '') {
                $.each(data.split(','),
                    function (i, val) {
                        $('.lbCheckListOption').each(function () {
                            if (val == this.value) {
                                this.checked = true;
                            }
                        }
                        );
                    }
                );
            }
            $('#checklistDialog').dialog({ width: '600px' });
        }

        function removeQuote(quoteID) {
            $("#MainContent_hdnQuoteToDelete").val(quoteID);
            $("#MainContent_btnDeleteQuote").click();
        }

        function ugsMultiQuote() {
            url = "UGSMultiQuote.aspx?rfqID=" + $('#MainContent_rfqNumber').html();
            window.open(url);
        }

        function ugsSummary() {
            url = "CreateQuote.aspx?rfqID=" + $('#MainContent_rfqNumber').html() + "&quoteType=6";
            window.open(url);
        }

        function showRFQCheckList() {
            url = 'processPartCheckList?rfq=' + $('#MainContent_rfqNumber').html() + '&part=&get=1';
            $.ajax({ url: url, success: function (data) { processRFQCheckList(data); } });
        }

        function processRFQCheckList(data) {
            data = data.trim();
            $('.lbRFQCheckListOption').each(function () {
                this.checked = false;
            }
            );
            if (data != '') {
                $.each(data.split(','),
                    function (i, val) {
                        $('.lbRFQCheckListOption').each(function () {
                            if (val == this.value) {
                                this.checked = true;
                            }
                        }
                        );
                    }
                );
            }
            $('#RFQchecklistDialog').dialog({ width: '600px' });
        }


        function applyCheckList() {
            url = 'processPartCheckList?get=0&rfq=' + $('#MainContent_rfqNumber').html();
            if (document.getElementById('allParts').checked) {
                url = url + '&all=1';
            } else {
                url = url + '&all=0';
            }
            url = url + '&part=' + clPartID + '&val=';
            separator = '';
            $('.lbCheckListOption').each(function () {
                if (this.checked == true) {
                    url = url + separator + this.value;
                    separator = ',';
                }
            }
            );
            $.ajax({ url: url, success: function (data) { processPartCheckListUpdate(data) } });
            $('#checklistDialog').dialog('close');
        }

        function processPartCheckListUpdate(data) {
            $('#clImage' + clPartID).attr("src", data);
        }

        function applyRFQCheckList() {
            url = 'processPartCheckList?get=0&rfq=' + $('#MainContent_rfqNumber').html() + '&part=&val=';
            separator = '';
            $('.lbRFQCheckListOption').each(function () {
                if (this.checked == true) {
                    url = url + separator + this.value;
                    separator = ',';
                }
            }
            );
            $.ajax({ url: url, success: function (data) { processRFQCheckListUpdate(data) } });
            $('#RFQchecklistDialog').dialog('close');
        }

        function processRFQCheckListUpdate(data) {
            $('#rfqcl').attr("src", data);

        }

        function editAssembly(id) {
            openAssembly();
            var url = 'GetAssemblyInfo?AssemblyId=' + id;
            $.ajax({
                url: url, success: function (data) {
                    var array = data.split('|');
                    for (i = 0; i < array.length; i++) {
                        var split = array[i].split('::::');
                        if (split[0] == "Number") {
                            $('#MainContent_txtAssemblyNum').val(split[1]);
                        }
                        else if (split[0] == "Description") {
                            $('#MainContent_txtAssemblyDescription').val(split[1]);
                        }
                        else if (split[0] == "Type") {
                            $('#MainContent_ddlAssemblyType').val(split[1]);
                        }
                        else if (split[0] == "LineNumbers") {
                            var lineNum = split[1].split(',');
                            for (j = 0; j < lineNum.length; j++) {
                                $('#checkbox' + lineNum[j]).prop('checked', true);
                            }
                        }
                    }
                    $("#MainContent_hdnAssemblyId").val(id);
                }
            });

            //$('#MainContent_txtAssemblyNum').val()

        }

        function editPart(btnid, lineId) {
            if (lineId.split('line')[1].indexOf('A') >= 0) {
                editAssembly(lineId.split('line')[1]);
                return;
            }
            newid = '#' + btnid.replace('btnEdit', 'PartNumber');
            $('#MainContent_txtPart').val($(newid).html());

            $('#MainContent_hdnPartID').val($(newid).html().replace('&nbsp;', ' '));
            newid = '#' + btnid.replace('btnEdit', 'LineNumber');
            //alert(newid);
            $('#MainContent_txtLineNumber').val($(newid).html());
            $('#MainContent_hdnLineNum').val($(newid).html());
            //alert($(newid).html());
            //document.getElementById('MainContent_txtLineNumber').value = $(newid).html();
            newid = '#' + btnid.replace('btnEdit', 'PartDescription');
            $('#MainContent_txtDescription').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'PartLength');
            $('#MainContent_txtLength').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'PartWidth');
            $('#MainContent_txtWidth').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'PartHeight');
            $('#MainContent_txtHeight').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'Thickness');
            $('#MainContent_txtThickness').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'PartWeight');
            $('#MainContent_txtWeight').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'txtPartNote');
            $('#MainContent_txtPartNotesDia').val($(newid).html());

            newid = btnid.replace('btnEdit', 'imgPart');
            $('#MainContent_lblPicture').html("<a href='" + document.getElementById(newid).src + "' target='_blank'><img src='" + document.getElementById(newid).src + "' width='100'></a>");
            newid = '#' + btnid.replace('btnEdit', 'PartType');
            $('#MainContent_ddlPartType option').each(function () {
                if ($(this).text() == $(newid).html()) {
                    $(this).attr('selected', 'selected');
                }
            });
            //newid = '#' + btnid.replace('btnEdit', 'BlankInfo');
            //$('#MainContent_ddlBlankInfo option').each(function () {
            //    if ($(this).text() == $(newid).html()) {
            //        $(this).attr('selected', 'selected');
            //    }
            //});
            newid = '#' + btnid.replace('btnEdit', 'MaterialType');
            //$('#MainContent_ddlMaterialType option').each(function () {
            //    if ($(this).text() == $(newid).html()) {
            //        $(this).attr('selected', 'selected');
            //    }
            //});
            $('#MainContent_txtMaterialType').val($(newid).html());
            newid = '#' + btnid.replace('btnEdit', 'AnnualVolume');
            $('#MainContent_txtPartAnnualVolume').val($(newid).html());
            showEditDialog();
        }

        function showEditDialog() {
            $('#EditPartDialog').dialog({ width: 800, height: 500 });
            $('#EditPartDialog').parent().appendTo("form");
        }

        function hideEditDialog() {
            $('#EditPartDialog').dialog('close');
        }
        function hideFindPartDialog() {
            $('#FindPartDialog').dialog('close');
        }

        function showAddPart() {
            $('#MainContent_txtPart').val('');
            $('#MainContent_txtDescription').val('');
            $('#MainContent_txtLength').val('0');
            $('#MainContent_txtWidth').val('0');
            $('#MainContent_txtHeight').val('0');
            $('#MainContent_txtThickness').val('0');
            $('#MainContent_txtWeight').val('0');
            $('#MainContent_lblPicture').html('');
            $('#MainContent_hdnLineNum').val('');
            $('#MainContent_txtLineNumber').val('');
            $('#MainContent_txtAnnualVolume').val('');
            $('#MainContent_txtPartAnnualVolume').val('0');
            showEditDialog();
        }

        function removeAllParts() {
            $('#DeleteAllPartsDialog').dialog({ width: '600px' });
            $('#DeleteAllPartsDialog').parent().appendTo("form");
        }

        function viewAllQuotes() {
            url = "CreateQuote.aspx?rfqID=" + $('#MainContent_rfqNumber').html() + '&quoteType=' + 2 + '&rand=' + Math.random();
            window.open(url);
        }

        function onlyMyCompaniesQuoteOnePDF() {
            url = "CreateQuote.aspx?rfqID=" + $('#MainContent_rfqNumber').html() + "&quoteType=2&onlyMyCompany=true&rand=" + Math.random();
            window.open(url);
            return false;
        }

        // this variable is needed to know which part to link the results to when the Find button is printed in the FindPartDialog
        var partToLink = '';
        function showFindPart(part) {
            partToLink = part;
            $('#MainContent_hdnPartID').val(part);
            $('#txtFindPartNumber').val('');
            $('#tblFindResults').html('');
            $('#FindPartDialog').dialog({ width: 1600, height: 600 });
        }

        function showLinkPart(part) {
            url = "GetLinkedParts.aspx?delete=no&create=no&part=" + part + "&rfq=" + $('#MainContent_rfqNumber').html() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { showLinkLookupResults(data) } })
        }

        // build Dialog that shows parts and any linked to the part row clicked.
        // includes call to function to add links or remove links
        function showLinkLookupResults(data) {
            xmlDoc = $.parseXML(data);
            // History is the container for all of the rows
            var rows = $(xmlDoc).find("LinkedParts");
            var tbl = '<table align="center" width="100%" cellpadding="1" class="table table-striped"><thead><tr><th>Select</th><th>Line Number</th><th>Part Number</th><th>Description</th></tr></thead><tbody>';
            frCounter = 0;
            var results = '';
            var linkid = 0;
            $.each(rows, function () {
                if (frCounter == 0) {
                    results = "<center>Parts To Link to " + $(this).find("PartName").text() + " " + $(this).find("PartDescription").text() + "</center>\n";
                    linkid = $(this).find("PartId").text();
                } else {
                    tbl = tbl + "<tr>";
                    // \x22 is the double quote symbol
                    tbl = tbl + "<td><input type='checkbox' id='linksel" + frCounter + "' value='1' ";
                    if ($(this).find("LinkID").text() != "0") {
                        tbl = tbl + "checked = 'checked' ";
                    }
                    tbl = tbl + " onclick=\x22applyLinkToPart('" + linkid + "',this.checked,'" + $(this).find("PartId").text() + "');\x22></td>";
                    tbl = tbl + "<td>" + $(this).find("LineNumber").text() + "</td>";
                    tbl = tbl + "<td><a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/RFQ" + partRFQ[frCounter] + "_" + partNumber[frCounter] + ".png' target='_blank'>" + $(this).find("PartName").text() + "</a></td>";
                    tbl = tbl + "<td>";
                    tbl = tbl + $(this).find("PartDescription").text();
                    tbl = tbl + "</td>";
                    tbl = tbl + "</tr>\n";
                }
                frCounter++;
            });
            results = results + tbl + "</tbody></table>";
            $('#LinkPartsDialog').html(results);
            $('#LinkPartsDialog').dialog({ width: 1000, height: 800 });
        }

        var globalPartID = 0;

        // make ajax call to get any matching history parts.
        function findParts() {
            globalPartID = $('#MainContent_hdnPartID').val();

            url = "GetHistory.aspx?create=yes&search=" + $('#txtFindPartNumber').val().replace("+", "%2B") + "&part=" + $('#MainContent_hdnPartID').val() + '&rfq=' + $('#MainContent_rfqNumber').html() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { parseResults(data, 1, partToLink); } });
        }

        function fineNewPartsNoLink() {
            globalPartID = $('#MainContent_hdnPartID').val();

            url = "GetHistory.aspx?create=no&search=" + $('#txtFindPartNumber').val().replace("+", "%2B") + "&part=" + $('#MainContent_hdnPartID').val().replace("+", "%2B") + '&rfq=' + $('#MainContent_rfqNumber').html().replace("+", "%2B");
            url += '&desc=' + $('#txtFindPartDesc').val().replace("+", "%2B") + '&cust=' + $('#txtFindPartCustomer').val().replace("+", "%2B") + '&custRFQ=' + $('#MainContent_txtCustomerRFQNumber').val().replace("+", "%2B") + '&start=' + $('#MainContent_txtFindStartDate').val();
            url += '&end=' + $('#MainContent_txtFindEndDate').val() + '&quoteNum=' + $("#txtQuoteNumber").val() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { parseResults(data, 1, partToLink); } });
        }

        // Find Results Counter and an array for each field.
        var frCounter = 0;
        var partNumber = Array();
        var partDescription = Array();
        var partRFQ = Array();
        var partQuote = Array();
        var partCustomer = Array();
        var partCustomerQuote = Array();
        var partNoQuote = Array();
        var partID = '';
        var partCompany = Array();
        var partPicture = Array();
        var partHistoryID;
        var quoteName = Array();
        var quoteID = Array();
        var quoteNumber = Array();
        var rfqID = Array();

        // data plus flag 0 or 1 to create the checkbox part
        // if not creating the checkbox part, then we are filling these results inline underneath the part row
        function parseResults(data, useDialog, partUsed) {
            //alert(data + ' ' + useDialog + ' ' + partUsed);
            frCounter = 0;
            if (data.length < 50) {
                results = "<b>No History Matches</b>";
                partID = data;
            } else {
                xmlDoc = $.parseXML(data);
                // History is the container for all of the rows
                var rows = $(xmlDoc).find("History");
                var results = '<table align="center" width="100%" cellpadding="1" class="table table-striped"><thead><tr><th>Select</th><th>Company</th><th>Part Number</th><th>Description</th><th>RFQ ID</th><th>Customer</th><th>Customer RFQ</th><th>Quote ID</th><th>Status</th><th>No Quote Reason</th></tr></thead><tbody>';
                $.each(rows, function () {

                    //dt.Columns.Add("PartID", typeof(int));
                    //dt.Columns.Add("PartNumber", typeof(string));
                    //dt.Columns.Add("PartDescription", typeof(string));
                    //dt.Columns.Add("RFQID", typeof(string));
                    //dt.Columns.Add("QuoteID", typeof(string));
                    //dt.Columns.Add("Customer", typeof(string));
                    //dt.Columns.Add("CustomerRFQNumber", typeof(string));
                    //dt.Columns.Add("Status", typeof(string));
                    //dt.Columns.Add("NoQuoteReason", typeof(string));
                    //dt.Columns.Add("TSGCompany", typeof(string));
                    //dt.Columns.Add("PartHistoryID", typeof(string));
                    //dt.Columns.Add("QuoteNumber", typeof(string));
                    //dt.Columns.Add("picture", typeof(string));

                    partID = $(this).find("PartID").text().trim();
                    partNumber[frCounter] = $(this).find("PartNumber").text();
                    partDescription[frCounter] = $(this).find("PartDescription").text();
                    rfqID[frCounter] = $(this).find("RFQID").text();
                    quoteID[frCounter] = $(this).find("QuoteID").text();
                    partCustomer[frCounter] = $(this).find("Customer").text();
                    partCustomerQuote[frCounter] = $(this).find("CustomerRFQNumber").text();
                    var status = $(this).find("Status").text();
                    partNoQuote[frCounter] = $(this).find("NoQuoteReason").text();
                    partCompany[frCounter] = $(this).find("TSGCompany").text();
                    partHistoryID = $(this).find("PartHistoryID").text();
                    quoteNumber[frCounter] = $(this).find("QuoteNumber").text();
                    partPicture[frCounter] = $(this).find("picture").text();

                    results = results + "<tr>";
                    // \x22 is the double quote symbol
                    if (useDialog == 1) {
                        if (partHistoryID != '') {
                            results = results + "<td><input type='checkbox' id='sel" + frCounter + "' value='0' onclick=\x22applyLinkToQuote('" + partID + "',this.checked,'" + partHistoryID + "');\x22></td>";
                        }
                        else {
                            results = results + "<td><input type='checkbox' id='sel" + frCounter + "' value='0' onclick=\x22applyLinkToQuote('" + partID + "',this.checked,'" + partHistoryID + "');\x22></td>";
                        }
                    }
                    else {
                        if (partHistoryID != '') {
                            results = results + "<td><input type='checkbox' id='sel" + frCounter + "' value='0' checked='false' onclick=\x22applyLinkToQuote('" + partID + "',this.checked,'" + partHistoryID + "');\x22></td>";
                        }
                        else {
                            results = results + "<td><input type='checkbox' id='sel" + frCounter + "' value='0' checked='false' onclick=\x22applyLinkToQuote('" + partID + "',this.checked,'" + partHistoryID + "');\x22></td>";
                        }
                    }
                    results = results + "<td>" + partCompany[frCounter] + "</td>";
                    results = results + "<td><a href='https://toolingsystemsgroup.sharepoint.com/sites/Estimating/Part%20Pictures/" + partPicture[frCounter] + "' target='_blank'>" + partNumber[frCounter] + "</a></td>";
                    results = results + "<td>";
                    results = results + partDescription[frCounter];
                    results = results + "</td>";
                    results = results + "<td>";
                    if (rfqID[frCounter] != "Stand Alone") {
                        results = results + "<a href='https://tsgrfq.azurewebsites.net/EditRFQ?id=" + rfqID[frCounter] + "' target='_blank'>" + rfqID[frCounter] + "</a>";
                    }
                    else {
                        results = results + rfqID[frCounter];
                    }
                    results = results + "</td>";
                    results = results + "<td>";
                    results = results + partCustomer[frCounter];
                    results = results + "</td>";
                    results = results + "<td>";
                    results = results + partCustomerQuote[frCounter];
                    results = results + "</td>";
                    results = results + "<td>";
                    userCompany = "";
                    switch ($('#MainContent_hdnCompanyID').val()) {
                        case "1":
                            userCompany = "TSG"
                            break;
                        case "2":
                            userCompany = "ATS"
                            break;
                        case "3":
                            userCompany = "BTS"
                            break;
                        case "5":
                            userCompany = "DTS"
                            break;
                        case "7":
                            userCompany = "ETS"
                            break;
                        case "8":
                            userCompany = "GTS"
                            break;
                        case "9":
                            userCompany = "HTS"
                            break;
                        case "12":
                            userCompany = "RTS"
                            break;
                        case "13":
                            userCompany = "STS"
                            break;
                        case "15":
                            userCompany = "UGS"
                            break;
                        case "20":
                            userCompany = "NIA"
                            break;
                        case "21":
                            userCompany = "NRS"
                            break;
                    }
                    //                    alert("$('#MainContent_hdnCompanyID').val(): " + $('#MainContent_hdnCompanyID').val() + "  userCompany:" + userCompany);
                    if (quoteNumber[frCounter].indexOf("OPEN RFQ") != -1) {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='EditRFQ.aspx?id=" + rfqID[frCounter] + "' target='_blank'>" + rfqID[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }

                    else if (status != 'History from MAS' && quoteNumber[frCounter].indexOf("UGS") >= 0) {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='UGSEditQuote.aspx?id=" + quoteID[frCounter] + "' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }

                    else if (status != 'History from MAS' && quoteNumber[frCounter].indexOf("STS") >= 0) {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='STSEditQuote.aspx?id=" + quoteID[frCounter] + "' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }

                    else if (status != 'History from MAS' && quoteNumber[frCounter].indexOf("HTS") >= 0) {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='HTSEditQuote.aspx?id=" + quoteID[frCounter] + "' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }
                    else if (status != 'History from MAS' && quoteNumber[frCounter].indexOf("SA") != -1) {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='EditQuote.aspx?id=" + quoteID[frCounter].split("-")[0] + "&quoteType=1' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }
                    else if (status != 'History from MAS') {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='EditQuote.aspx?id=" + quoteID[frCounter] + "&quoteType=2' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }
                    else {
                        if (userCompany == partCompany[frCounter] || userCompany == "TSG") {
                            results = results + "<a href='EditQuote.aspx?id=" + quoteID[frCounter] + "&quoteType=2" + "&quoteNumber=" + quoteNumber[frCounter] + "' target='_blank'>" + quoteNumber[frCounter] + "</a></td>";
                        }
                        else {
                            results = results + quoteNumber[frCounter] + "</td>";
                        }
                    }
                    results = results + "</td>";
                    results = results + "<td>";
                    results = results + status;
                    results = results + "</td>";
                    results = results + "<td>";
                    results = results + partNoQuote[frCounter];
                    results = results + "</td>";
                    results = results + "<td>";
                    if (partNoQuote[frCounter] != "" || status == "Reserved" || status == "Open Part") {

                    }
                    else {
                        results += "<input type='checkbox' id='cbKeep" + frCounter + "_" + partID + "' >Keep Quote Number</input>";
                        if (status == 'History from MAS') {
                            results = results + "<input type='button' class='mybutton' value='Copy Quote to RFQ' onClick=\"copyQuote('" + partID + "','" + quoteID[frCounter] + "','" + quoteNumber[frCounter] + "-MAS', " + frCounter + ");return false;\" >";
                        }
                        else {
                            results = results + "<input type='button' class='mybutton' value='Copy Quote to RFQ' onClick=\"copyQuote('" + partID + "','" + quoteID[frCounter] + "','" + quoteNumber[frCounter] + "', " + frCounter + ");return false;\" >";
                        }
                    }
                    results = results + "</tr>";
                    frCounter++;
                });
                results = results + "</tbody></table>";
            }
            if (useDialog == 1) {
                $('#tblFindResults').html(results);
            } else {
                //alert(this.partNumber);
                if ($('#lineHistory' + partID).length) {
                    // delete the existing one
                    $('#lineHistory' + partID).remove();
                }
                partToLink = $(this).find("PartNumber").text();
                tblResults = results;
                results = "<input type='button' class='mybutton' value='Search History'  onClick=";
                results = results + '"';
                results = results + "showFindPart('" + partUsed + "');return false;";
                results = results + '"';
                results = results + "> <input type='button' class='mybutton' id='historyBtn" + partID + "' value='Hide History' onClick='toggleHistory(" + partID + ");return false;'><BR>";
                results = results + "<div id='History" + partID + "'>" + tblResults + "</div>";

                //$('#lineHistory' + partHistoryID).remove();
                $('#line' + partID).after("<tr id='lineHistory" + partID.replace(/\s/g, "") + "'><td></td><td colspan='12'>" + results + "<td colspan='3'></td></tr>");

                $('.mybutton').button();
            }
            //document.location.href = document.location.href;
        }

        function toggleHistory(partID) {
            if ($('#historyBtn' + partID).val() == "Hide History") {
                $('#History' + partID).hide();
                $('#historyBtn' + partID).val('Show History');
            }
            else {
                $('#History' + partID).show();
                $('#historyBtn' + partID).val('Hide History');
            }

        }

        function copyQuote(partID, quoteID, quoteNum, count) {
            if (document.getElementById('cbKeep' + count + '_' + partID).checked) {
                url = "CopyQuoteToRFQ.aspx?partID=" + partID + "&quoteID=" + quoteID + "&quoteNum=" + quoteNum + "&rfqID=" + $('#MainContent_rfqNumber').html() + '&keep=yes';
            }
            else {
                url = "CopyQuoteToRFQ.aspx?partID=" + partID + "&quoteID=" + quoteID + "&quoteNum=" + quoteNum + "&rfqID=" + $('#MainContent_rfqNumber').html() + '&keep=no';
            }
            $.ajax({ url: url, success: function (data) { } });
        }

        function processHistoryResults() {
            url = "GetHistory.aspx?create=&search=&part=" + partToLink + '&rfq=' + $('#MainContent_rfqNumber').html() + '&rand=' + Math.random();
            // in this case,we want to send to the row under the part
            globalPartID = $('#MainContent_hdnPartID').val();
            //alert(globalPartID);
            $.ajax({ url: url, success: function (data) { parseResults(data, 0, partToLink); } });
        }

        function addThePart() {
            document.form1.submit();
        }


        function showNoReason() {
            $('#MainContent_txtNQRAppliesTo').val('ALL REMAINING');
            $('#NoQuoteReasonDialog').dialog({
                width: 500, height: 400, appendTo: "form"
            });
        }

        function showRemoveNoQuotes() {
            $('#MainContent_txtRemoveNQAppliesTo').val('ALL NO QUOTE');
            $('#RemoveNoQuoteDialog').dialog({ width: 500, height: 400 });
        }

        function reserveAllParts() {
            reservePart('ALL');
        }

        function removeNoQuotePart(part) {
            $('#MainContent_txtRemoveNQAppliesTo').val(part);
            $('#RemoveNoQuoteDialog').dialog({ width: 500, height: 400 });
        }

        function applyNoQuotePart(part) {
            $('#MainContent_txtNQRAppliesTo').val(part);
            $('#NoQuoteReasonDialog').dialog({ width: 500, height: 400 });
        }

        function ApplyNoQuote() {
            if ($('#MainContent_txtNQRAppliesTo').val() == 'ALL REMAINING') {
                $('#MainContent_txtNQRAppliesTo').val('ALL');
            }
            url = 'processNoQUote.aspx?rfq=' + $('#MainContent_rfqNumber').html() + '&remove=no&applies=' + $('#MainContent_txtNQRAppliesTo').val() + '&reason=' + $('#MainContent_ddlNoQuoteReason').val() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { processApplyNoQuoteResponse(data); } })
            //window.location.reload();
        }

        function processApplyNoQuoteResponse(data) {
            // if all, reload the page - do not use reload as this would reprocess any postbacks
            $('#NoQuoteReasonDialog').dialog('close');
            if ($('#MainContent_txtNQRAppliesTo').val() == 'ALL') {
                document.location.href = document.location.href;
            } else {
                // if specific part, then update the quoting div tag to only show remove the no quote
                results = "<input type='button' class='mybutton' value='Remove No Quote'  onClick=\"removeNoQuotePart('" + $('#MainContent_txtNQRAppliesTo').val() + "');return false;\" >";
                divtag = '#quoting' + $('#MainContent_txtNQRAppliesTo').val();
                $(divtag).html(results);
                $('.mybutton').button();
            }
        }

        function RemoveNoQuote() {
            if ($('#MainContent_txtRemoveNQAppliesTo').val() == 'ALL NO QUOTES') {
                $('#MainContent_txtRemoveNQAppliesTo').val('ALL');
            }
            url = 'processNoQUote.aspx?rfq=' + $('#MainContent_rfqNumber').html() + '&remove=yes&applies=' + $('#MainContent_txtRemoveNQAppliesTo').val() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { processRemoveNoQuoteResponse(data); } })
            window.location.reload();
        }

        function processRemoveNoQuoteResponse(data) {
            // if all, reload the page - do not use reload as this would reprocess any postbacks
            $('#RemoveNoQuoteDialog').dialog('close');
            if ($('#MainContent_txtRemoveNQAppliesTo').val() == 'ALL') {
                document.location.href = document.location.href;
            } else {
                // if specific part, then update the quoting div tag to only show remove the no quote
                results = "<input type='button' class='mybutton' value='No Quote'  onClick=\"applyNoQuotePart('" + $('#MainContent_txtRemoveNQAppliesTo').val() + "');return false;\" >";;
                results = results + "<input type='button' class='mybutton' value='Reserve' onClick=\"reservePart('" + $('#MainContent_txtRemoveNQAppliesTo').val() + "');return false;\" >";
                $('#quoting' + $('#MainContent_txtRemoveNQAppliesTo').val()).html(results);
                $('.mybutton').button();
            }
        }

        var globalReservePart = '';
        function reservePart(partToReserve) {
            globalReservePart = partToReserve;
            url = 'processNoQuote.aspx?rfq=' + $('#MainContent_rfqNumber').html() + '&reserve=yes&remove=no&applies=' + partToReserve + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { processReserveResponse(data); } })
        }

        function processReserveResponse(data) {
            ;
            // if all, reload the page - do not use reload as this would reprocess any postbacks
            //alert(globalReservePart);
            if (globalReservePart == 'ALL') {
                document.location.href = document.location.href;
            }
            else if (globalReservePart.indexOf('A') > -1) {
                results = "Reserved";
                var co = '';
                if ($('#MainContent_hdnCompanyID').val() == 13) {
                    co = 'STS';
                }
                else if ($('#MainContent_hdnCompanyID').val() == 20) {
                    co = 'NIA';
                }
                results = 'Reserved By ' + co + '<br />';
                if ($('#MainContent_hdnCompanyID').val() == 13 || $('#MainContent_hdnCompanyID').val() == 20) {
                    results += '<a href="STSEditQuote?rfq=' + $('#MainContent_rfqNumber').html() + '&assemblyId=' + globalReservePart + '" target="_blank">Quote</a></ br>';
                }
                $('#quoting' + globalReservePart).html(results);
            }
            else {
                // if specific part, then update the quoting div tag to only show remove the no quote
                results = "<input type='button' class='mybutton' value='No Quote'  onClick=\"applyNoQuotePart('" + globalReservePart + "');return false;\" >";
                if ($('#MainContent_hdnCompanyID').val() == 9) {
                    results = results + "<a href='HTSEditQuote?rfq=" + $('#MainContent_rfqNumber').html() + '&partID=' + globalReservePart + "' target='_blank'>Quote</a></ br>";
                }
                else if ($('#MainContent_hdnCompanyID').val() == 13 || $('#MainContent_hdnCompanyID').val() == 20) {
                    results = results + "<a href='STSEditQuote?rfq=" + $('#MainContent_rfqNumber').html() + "&partID=" + globalReservePart + "' target='_blank'>Quote</a></ br>";
                }
                else if ($('#MainContent_hdnCompanyID').val() == 15) {
                    results = results + "<a href='UGSEditQuote?rfq=" + $('#MainContent_rfqNumber').html() + "&partID=" + globalReservePart + "' target='_blank'>Quote</a></ br>";
                }
                else {
                    results = results + "<div class='mybutton' onclick='uploadQuote();' id='quoteUploadButton'>Upload Quote</div>";
                }
                $('#quoting' + globalReservePart).html(results);
                $('.mybutton').button();
            }
        }

        function quotePart(part) {
            url = "EditQuote.aspx?id=0" + "&rfq=" + $('#MainContent_rfqNumber').html() + "&partID=" + part + '&quoteType=' + 2 + '&rand=' + Math.random();
            window.open(url);
        }

        function showNoQuote() {
            $('#NoQuoteDialog').dialog();
        }

        function applyLinkToQuote(partid, linkit, quoteid) {
            if (linkit) {
                url = 'UnlinkPartFromHistory.aspx?partID=' + partid + '&quoteID=' + quoteid + '&link=yes&rfqID=' + $('#MainContent_rfqNumber').html();
            } else {
                url = 'UnlinkPartFromHistory.aspx?partID=' + partid + '&quoteID=' + quoteid + '&link=&rfqID=' + $('#MainContent_rfqNumber').html();
            }
            $.ajax(url);
        }
        var colorCount = -1;

        function getColor() {
            var color = ['blue', 'red', 'yellow', 'green', 'GreenYellow', 'IndianRed', 'OrangeRed', 'violet', 'SteelBlue', 'Teal'];
            if (colorCount >= color.length - 1) {
                colorCount = -1;
            }
            colorCount++;
            return color[colorCount];
        }

        function applyLinkToPart(partid, linkit, newpart) {
            if (linkit) {
                url = 'GetlinkedParts.aspx?part=' + partid + '&create=yes&link=' + newpart + '&delete=no&rfq=' + $('#MainContent_rfqNumber').html();
                var col = getColor();
                $('#line' + partid).css("backgroundColor", col);
                $('#line' + newpart).css("backgroundColor", col);
                $('#line' + newpart).insertAfter('#lineHistory' + partid);
                $('#lineHistory' + newpart).insertAfter('#line' + newpart);
            } else {
                url = 'GetlinkedParts.aspx?part=' + partid + '&create=no&link=' + newpart + '&delete=yes&rfq=' + $('#MainContent_rfqNumber').html();
                $('#line' + partid).css("backgroundColor", "White");
                $('#line' + newpart).css("backgroundColor", "White");
            }
            $.ajax(url);


        }

        function setNotifyGroup(ckvalue) {
            var myCompanyArray = ["ATS", "BTS", "DTS", "ETS", "GTS", "RTS"];
            var arrayLength = myCompanyArray.length;
            for (var i = 0; i < myCompanyArray.length; i++) {
                try {
                    document.getElementById('notify' + myCompanyArray[i]).checked = ckvalue;
                }
                catch (err) {

                }
            }
        }

        function setNotifyGroupFixture(ckvalue) {
            setNotifyGroup(ckvalue);
            try {
                document.getElementById('notifyUGS').checked = ckvalue;
            }
            catch (err) {

            }
        }

        // co can be a comma delimited list of companies
        function sendNotification(co, rfq, reallysend) {
            url = "NotifyCompanyRFQ.aspx?remove=0&company=" + co + "&rfq=" + rfq + "&really=" + reallysend + "&rand=" + Math.random();
            $.ajax(url);
        }

        // co is one company here
        function removeNotification(co, rfq) {
            url = "NotifyCompanyRFQ.aspx?remove=1&company=" + co + "&rfq=" + rfq + "&really=1&rand=" + Math.random();
            $.ajax(url);
        }
        function selectLocation(newcustomer) {
            $.ajax({ url: 'GetLocations?customer=' + newcustomer + '&rand=' + Math.random(), type: "GET", success: function (data) { setLocation(data); } });
        }

        function setLocation(data) {
            var mySelect = document.getElementById('MainContent_ddlPlant');
            mySelect.options.length = 0;
            var optlist = data.split('\r\n');
            for (i = 0; i < optlist.length; i++) {
                var myvalues = optlist[i].split(':')
                if (myvalues.length > 1) {
                    var option = document.createElement("option");
                    option.value = myvalues[1];
                    option.text = myvalues[2];
                    mySelect.appendChild(option);
                }
            }
        }

        function downloadSummary() {
            if ($('#MainContent_rfqNumber').html() != '') {
                if ($('#MainContent_rfqNumber').html() != '0') {
                    window.open('RFQSummary.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                }
            }
        }

        function downloadPartSummary() {
            if ($('#MainContent_rfqNumber').html() != '') {
                if ($('#MainContent_rfqNumber').html() != '0') {
                    window.open('RFQSummary.ashx?rfq=' + $('#MainContent_rfqNumber').html() + '&IncludeParts=true', 'RFQSummary');
                }
            }
        }

        function downloadQuoteSheet() {
            if ($('#MainContent_rfqNumber').html() != '') {
                if ($('#MainContent_rfqNumber').html() != '0') {
                    if ($('#MainContent_hdnCompanyID').val() == '15') {
                        window.open('UGSQuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                    }
                    else if ($('#MainContent_hdnCompanyID').val() == '9') {
                        window.open('HTSQuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                    }
                    else if ($('#MainContent_hdnCompanyID').val() == '13') {
                        window.open('STSQuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                    }
                    else {
                        window.open('QuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                    }
                }
            }
        }

        function downloadUGSQuoteSheet() {
            if ($('#MainContent_rfqNumber').html() != '') {
                if ($('#MainContent_rfqNumber').html() != '0') {
                    window.open('UGSQuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html(), 'RFQSummary');
                }
            }
        }

        function downloadNewQuoteSheet() {
            if ($('#MainContent_rfqNumber').html() != '') {
                if ($('#MainContent_rfqNumber').html() != '0') {
                    if ($('#MainContent_hdnCompanyID').val() == '15') {
                        window.open('UGSQuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html() + '&newParts=true', 'RFQSummary');
                    }
                    else {
                        window.open('QuoteSheet.ashx?rfq=' + $('#MainContent_rfqNumber').html() + '&newParts=true', 'RFQSummary');
                    }
                }
            }
        }

        function hideReservedParts() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                if ($('#MainContent_hdnReservedPartIds').val() != '') {
                    var parts = $('#MainContent_hdnReservedPartIds').val().split(',');
                    for (i = 0; i < parts.length; i++) {
                        $('#line' + parts[i]).hide();
                        $('#lineHistory' + parts[i]).hide();
                    }
                }
            }
        }

        function showReservedParts() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                if ($('#MainContent_hdnReservedPartIds').val() != '') {
                    var parts = $('#MainContent_hdnReservedPartIds').val().split(',');
                    for (i = 0; i < parts.length; i++) {
                        $('#line' + parts[i]).show();
                        $('#lineHistory' + parts[i]).show();
                    }
                }
            }
        }

        function hideAllParts() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                var parts = $('#MainContent_hdnAllPartIds').val().split(',');
                for (i = 0; i < parts.length; i++) {
                    $('#line' + parts[i]).hide();
                    $('#lineHistory' + parts[i]).hide();
                }
            }
        }

        function showAllParts() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                var parts = $('#MainContent_hdnAllPartIds').val().split(',');
                for (i = 0; i < parts.length; i++) {
                    $('#line' + parts[i]).show();
                    $('#lineHistory' + parts[i]).show();
                }
            }
        }

        function hideAllHistory() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                var parts = $('#MainContent_hdnAllPartIds').val().split(',');
                for (i = 0; i < parts.length; i++) {
                    $('#lineHistory' + parts[i]).hide();
                }
            }
        }

        function showAllHistory() {
            if ($('#MainContent_hdnAllPartIds').val() != '') {
                var parts = $('#MainContent_hdnAllPartIds').val().split(',');
                for (i = 0; i < parts.length; i++) {
                    $('#lineHistory' + parts[i]).show();
                }
            }
        }

    </script>
    <script src="blueimp/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.fileupload.js" type="text/javascript"></script>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    <asp:Literal ID="litPartScripts" runat="server"></asp:Literal>
    <asp:Literal ID="litDownloadQuotes" runat="server"></asp:Literal>
    <%--<script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-3.3.1.min.js"
        asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
        asp-fallback-test="window.jQuery"
        crossorigin="anonymous"
        integrity="sha384-tsQFqpEReu7ZLhBV2VZlAu7zcOV+rXbYlF2cqB8txI/8aZajjp4Bqd+V6D5IgvKT">
</script>--%>
    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
    <script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
    <link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css"
        rel="Stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $("[id$=txtExtraEmail]").autocomplete({
                source: function (request, response) {

                    var searchText = extractLast(request.term);
                    var rfqid = document.getElementById('<%=rfqNumber.ClientID%>').innerText;

                    $.ajax({
                        url: '<%=ResolveUrl("~/EditRFQ.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + searchText + "' , 'rfq': '" + rfqid + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        //response($.map(data.d, function (item) {
                        //    return {
                        //        label: item.split('-')[0]
                        //    }
                        //}))
                        response(data.d);
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
                }, focus: function () {
                    return false;
                },
                select: function (e, i) {
                    var terms = split($("[id$=txtExtraEmail]").val());

                    terms.pop();

                    terms.push(i.item.label);

                    terms.push("");
                    $("[id$=txtExtraEmail]").val(terms.join(", "));
                    return false;
                },
                //minLength: 1
            });
        });

        function split(val) {
            return val.toString().split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $("[id$=txtccEmail]").autocomplete({
                source: function (request, response) {

                    var searchText = extractLast(request.term);
                    var rfqid = document.getElementById('<%=rfqNumber.ClientID%>').innerText;

                    $.ajax({
                        url: '<%=ResolveUrl("~/EditRFQ.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + searchText + "' , 'rfq': '" + rfqid + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        //response($.map(data.d, function (item) {
                        //    return {
                        //        label: item.split('-')[0]
                        //    }
                        //}))
                        response(data.d);
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
                }, focus: function () {
                    return false;
                },
                select: function (e, i) {
                    var terms = split($("[id$=txtccEmail]").val());

                    terms.pop();

                    terms.push(i.item.label);

                    terms.push("");
                    $("[id$=txtccEmail]").val(terms.join(", "));
                    return false;
                },
                //minLength: 1
            });
        });

        function split(val) {
            return val.toString().split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }
    </script>
    <script type="text/javascript">
        $(function () {
            $("[id$=txtbccEmail]").autocomplete({
                source: function (request, response) {

                    var searchText = extractLast(request.term);
                    var rfqid = document.getElementById('<%=rfqNumber.ClientID%>').innerText;

                    $.ajax({
                        url: '<%=ResolveUrl("~/EditRFQ.aspx/GetCustomers") %>',
                    data: "{ 'prefix': '" + searchText + "' , 'rfq': '" + rfqid + "' }",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        //response($.map(data.d, function (item) {
                        //    return {
                        //        label: item.split('-')[0]
                        //    }
                        //}))
                        response(data.d);
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
                }, focus: function () {
                    return false;
                },
                select: function (e, i) {
                    var terms = split($("[id$=txtbccEmail]").val());

                    terms.pop();

                    terms.push(i.item.label);

                    terms.push("");
                    $("[id$=txtbccEmail]").val(terms.join(", "));
                    return false;
                },
                //minLength: 1
            });
        });

        function split(val) {
            return val.toString().split(/,\s*/);
        }
        function extractLast(term) {
            return split(term).pop();
        }
    </script>

</asp:Content>
