<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="Dashboard.aspx.cs" Inherits="RFQ.Dashboard" enableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .cssPager td {
            padding-left: 6px;
            padding-right: 6px;
        }
    </style>
    <div style="min-height: 40px"></div>
    <asp:Label ID="lblBoolean" Visible="false" runat="server"></asp:Label>
    <table width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td valign="top" width="20%" style="border: 1px dotted black;" class="ui-widget">
                <asp:Button ID="btnFindTop" runat="server" Text="Find" OnClick="btnFind_Click"  CssClass="mybutton"  />
            <asp:Button ID="btnNewRFQ" runat="server" Text="Add New RFQ" OnClick="btnNewRFQ_Click" CssClass="mybutton" />
            <div class="mybutton" onclick="showQuoteType();" >Create New Quote</div>
                <br />
                <hr />
            <asp:DropDownList ID="ddlFilter" runat="server" CssClass="ui-widget">
            </asp:DropDownList><br />
                <div class="mybutton" onclick="window.open('EditFilter?id=0','newfilter');">New Filter</div>
                <div class="mybutton" onclick="editFilter()">Edit Filter</div>
                <div class="mybutton" onclick="setDefaultFilter()">Set As Default</div>
                <br />
                <hr />
            <asp:Button ID="btnHotList" runat="server" OnClick="btnHotList_Click" Text="Hot List" CssClass="mybutton" />
              &nbsp;&nbsp;
            <asp:Button ID="btnOverDue" runat="server"  Text="Overdue" OnClick="btnOverDue_Click" CssClass="mybutton" />
                <br />
                <hr />
                <table>
                    <tr>
                        <td>
                            Company:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlCompany" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Status: 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget">
                     </asp:DropDownList>

                        </td>
                    </tr>
                    <tr>
                        <td>
                            RFQ ID:

                        </td>
                        <td>
                            <asp:TextBox ID="txtRFQID" runat="server" CssClass="ui-widget" Width="60"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Customer RFQ ID:

                        </td>
                        <td>
                            <asp:TextBox ID="txtCustomerRFQID" runat="server" CssClass="ui-widget" Width="60"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Customer:

                        </td>
                        <td>
                            <asp:TextBox ID="txtCustomer" runat="server" CssClass="ui-widget"></asp:TextBox>
                        </td>
                    </tr>   
                    <tr>
                        <td>
                            Salesperson:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSalesman" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            OEM:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOEM" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <%--<tr>
                        <td>
                            Order By:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOrderBy" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Sort By:
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSortBy" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>--%>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <font color="Black" size="2px">Check to define which RFQs to show</font>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Black" size="2px">No previously quoted parts</font><br />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkBlack" runat="server" Checked="true" CssClass="ui-widget" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Blue" size="2px">Has history in the RFQ</font><br />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkBlue" runat="server" Checked="true" CssClass="ui-widget" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Red" size="2px">All parts have been no quoted</font><br />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkRed" runat="server" CssClass="ui-widget" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Aqua" size="2px">Some parts have been reserved or no quoted</font><br />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkAqua" runat="server" Checked="true" CssClass="ui-widget" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Orange" size="2px">Some parts have been responded to</font><br />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkOrange" runat="server" Checked="true" CssClass="ui-widget" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <font color="Green" size="2px">All parts have been quoted or no quoted</font></p>
                        </td>
                        <td>
                            <asp:CheckBox ID="chkGreen" runat="server" CssClass="ui-widget" />
                        </td>
                    </tr>
                </table>
                <center>
                <asp:Button ID="btnFind" runat="server"  Text="Find" OnClick="btnFind_Click"  CssClass="mybutton"  />

                </center>
            </td>

            <div style="top: 50px;" id="colorNotes" align="right" margin-left:auto; margin-right:0;>
                <font color="Black" size="2px">Black - No previously quoted parts</font><br />
                <font color="Blue" size="2px">Blue - Has history in the RFQ</font><br />
                <font color="Red" size="2px">Red - All parts have been no quoted</font><br />
                <font color="Aqua" size="2px">Aqua - Some parts have been reserved or no quoted</font><br />
                <font color="Orange" size="2px">Orange - Some parts have been responded to</font><br />
                <font color="Green" size="2px">Green - All parts have been quoted or no quoted</font></p>
                <asp:HyperLink runat="server" ID="hlTrainingLink" Target="_blank" Font-Size="X-Large"></asp:HyperLink>
                <br /><asp:Label ID="lblSTSPictureInfo" runat="server" Font-Size="small"></asp:Label>
            </div>

            <td valign="top" width="85%" align="center">
                <h4>Results</h4>
                    <asp:GridView ID="dgResults" runat="server" AutoGenerateColumns="false" ViewStateMode="Enabled" AllowSorting="true" OnSorting="OnSort" 
                            EnableSortingAndPagingCallbacks="false" AllowPaging="true" PageSize="50" OnPageIndexChanging="OnPaging">
                        <Columns>
                            <asp:TemplateField HeaderText="RFQId" SortExpression="rfqID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:HyperLink ID="lblRFQId" runat="server" NavigateUrl='<%# String.Format("EditRFQ.aspx?id={0}", Eval("rfqID")) %>' Text='<%# Eval("rfqid") %>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" SortExpression="CustomerName" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("customer") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer RFQ" SortExpression="rfqCustomerRFQNumber" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:Label ID="lblCustomerRfq" runat="server" Text='<%# Eval("customer_rfq") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Salesman" SortExpression="TSGSalesman.Name" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center" >
                                <ItemTemplate>
                                    <asp:Label ID="lblSalesman" runat="server" Text='<%# Eval("salesman") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Due" SortExpression="rfqDueDate" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header"  ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblDueDate" runat="server" Text='<%# Eval("date_due") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status" SortExpression="rstRFQStatusDescription" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notified" SortExpression="rfqID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNotified" runat="server" Text='<%# Eval("notified") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Number of Parts" SortExpression="numOfParts" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumberOfParts" runat="server" Text='<%# Eval("numberOfParts") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Number of Parts Reserved" SortExpression="total" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumberOfPartsReserved" runat="server" Text='<%# Eval("numberOfPartsReserved") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="# of Parts Quoted" SortExpression="numPartsQuoted" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumberOfPartsQuoted" runat="server" Text='<%# Eval("numberOfPartsQuoted") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Live Work" SortExpression="livework" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblLiveWork" runat="server" Text='<%# Eval("liveWork") %>' ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%--<asp:BoundField DataField="rfqid" HeaderText="rfqid" Visible="false" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top"></asp:BoundField>--%>
  <%--                          <asp:HyperLinkField DataNavigateUrlFields="rfqid" HeaderText="RFQ ID" DataNavigateUrlFormatString="https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id={0}" DataTextField="rfqid" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                            <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="customer_rfq" HeaderText="Customer RFQ" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="salesman" HeaderText="Salesman" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="date_due" HeaderText="Date Due" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="status" HeaderText="Status" HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-content" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="notified" HeaderText="Notified" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top" HtmlEncode="false"></asp:BoundField>
                            <asp:BoundField DataField="numberOfParts" HeaderText="Number of Parts" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="numberOfPartsReserved" HeaderText="Number of Parts Reserved" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="numberOfPartsQuoted" HeaderText="Number of Parts Quoted" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                            <asp:BoundField DataField="liveWork" HeaderText="Live Work" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:BoundField>--%>
                            <%--<asp:BoundField DataField="button" HeaderText="Select" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HtmlEncode="true" />--%>
                            <asp:TemplateField HeaderText="Select" SortExpression="rfqid" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-header" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%if (lblBoolean.Text == "1") { %>
                                        <div class="mybutton" onclick="showNoReason('<%# Eval("rfqID") %>')" id="nqRemainingPartsDiv" >No Quote</div>
                                    <% } else { %>
                                        <div class="mybutton" onclick="openSendQuoteDialog('<%# Eval("rfqID") %>')"  id="btnSendQuoteDialog" >Send Quotes To Customer</div>
                                    <% } %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <PagerStyle CssClass="cssPager" />
                        <PagerSettings Mode="NumericFirstLast" />
                    </asp:GridView>
                <asp:Literal ID="litResults" runat="server" Visible="false"></asp:Literal>
            </td>
        </tr>
    </table>
    <div id="messageDialog" style="display: none;"></div>
    <div id="QuoteDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
            <label class="ui-widget">Select Quote Type</label><br />
            <asp:DropDownList ID="ddlQuoteType" runat="server"></asp:DropDownList>
            <br /><br />
            <asp:Button ID="btnNewQuote" runat="server" Text="Create Quote" OnClick="btnNewQuote_Click" CssClass="mybutton" />
    </div>
    <div id="NoQuoteReasonDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <label class="ui-widget">Applies To: </label><br />
        <asp:TextBox id="txtNQRAppliesTo" runat="server" ReadOnly="true" CssClass="ui-widget"></asp:TextBox>
        <br />
        <br />
        Reason<br />
        <asp:DropDownList ID="ddlNoQuoteReason" runat="server"></asp:DropDownList>
        <br /><br />
        <button class="mybutton" onclick="ApplyNoQuote();"  >Apply</button>
    </div>
    <div id="SendQuotesDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
            <center>
                <label class="ui-widget">Sending all quotes to the Customer</label><br />
                <label class="ui-widget" style="visibility: hidden;" id="hiddenRFQID"></label>
            </center>
            <label class="ui-widget">Any other email address to send the quotes to? (Seperate by comma)</label><br />
            <asp:TextBox id="txtExtraEmail" runat="server" ReadOnly="false" CssClass="ui-widget" style="width:350px;"></asp:TextBox>
            <br />
            <label class="ui-widget">What would you like the message to say?</label><br />
            <asp:TextBox ID="txtMessageText" TextMode="multiline" runat="server" ReadOnly="false" CssClass="ui-widget" style="width:100%;" Height="200px" ColSpan="4" Width="600px"></asp:TextBox><br /><br />
            <br />
            <asp:HyperLink runat="server" ID="hlquoteAttachment" Target="_blank"></asp:HyperLink>
            <br />
            <div class="mybutton" onclick="sendQuotes();"  id="btnSendQuotes" >Send Quotes To Customer</div>
        </div>

    <script>
        function page_init() {
            $('.mybutton').button();
            $('.datepicker').datepicker();
            //$('#MainContent_ddlFilter').change(function () { window.open('EditFilter?id=' + this.value, 'editfilter');})
        };
    </script>

    <script>
        function showMessage(msg) {
            $('#messageDialog').html(msg);
            $('#messageDialog').dialog();
        }

        function openSendQuoteDialog(rfqID) {
            $('#SendQuotesDialog').dialog({ width: 700, height: 550 });
            $('#SendQuotesDialog').parent().appendTo("form");
            $('#hiddenRFQID').val(rfqID);

            //$('#hlquoteAttachment').val('https://toolingsystemsgroup.sharepoint.com/TSG/IT/Software Development Site/RFQAndQuotingApplicationProject/Shared Documents/RFQ Email Attachments/' + RFQID);
        }

        function sendQuotes() {
            url = 'Disposition?Message=' + encodeURIComponent($('#MainContent_txtMessageText').val()) + '&rfq=' + $('#hiddenRFQID').val() + '&emails=' + $('#MainContent_txtExtraEmail').val();
            $.ajax({ url: url, success: function (data) { } });
            $('#SendQuotesDialog').dialog('close');
        }

        function showQuoteType() {
            $('#QuoteDialog').dialog({
                width: 200, height: 200, appendTo: "form"
            });
        }
        function editFilter()
        {
            if ($('#MainContent_ddlFilter').val() == '')
            {
                alert('You must select a filter to edit');
            } else {
                if ($('#MainContent_ddlFilter').val() == '0') {
                    alert('You must select a filter to edit');
                } else {
                    window.open('EditFilter?id=' + $('#MainContent_ddlFilter').val(), 'editfilter');
                }
            }
        }
        function setDefaultFilter() {
            url = 'SetDefaultFilter?filter=' + $('#MainContent_ddlFilter').val() + '&rand=' + Math.random();
            $.ajax(url);
        }
        function showNoReason(rfqid) {
            $('#MainContent_txtNQRAppliesTo').val(rfqid);
            $('#NoQuoteReasonDialog').dialog({
                width: 500, height: 400, appendTo: "form"
            });
        }
        function ApplyNoQuote() {
            url = 'processNoQUote.aspx?rfq=' + $('#MainContent_txtNQRAppliesTo').val() + '&remove=no&applies=ALL&reason=' + $('#MainContent_ddlNoQuoteReason').val() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { processApplyNoQuoteResponse(data); } })
        }


    </script>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>



</asp:Content>