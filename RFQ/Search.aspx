<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Search.aspx.cs" Inherits="RFQ.Search" enableEventValidation="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 100px"></div>
    <table>
        <tr>
            <td>Start Date:</td><td><asp:TextBox ID="txtStart" runat="server" CssClass="datepicker"></asp:TextBox></td>
            <td>End Date:</td><td><asp:TextBox ID="txtEnd" runat="server" CssClass="datepicker"></asp:TextBox></td>
        </tr>
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
                Quote Status: 
            </td>
            <td>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
            <td>
                Program: 
            </td>
            <td>
                <asp:DropDownList ID="ddlProgram" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                Quote Type: 
            </td>
            <td>
                <asp:DropDownList ID="ddlQuoteType" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
            <td>
                Salesman: 
            </td>
            <td>
                <asp:DropDownList ID="ddlSalesman" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                RFQ Status: 
            </td>
            <td>
                <asp:DropDownList ID="ddlRFQStatus" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
            <td>
                Estimator: 
            </td>
            <td>
                <asp:DropDownList ID="ddlEstimator" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                Quote Number: 
            </td>
            <td>
                <asp:TextBox ID="txtQuoteNumber" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                Customer RFQ: 
            </td>
            <td>
                <asp:TextBox ID="txtCustomerRFQ" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="ddlCustomerRFQ" runat="server" CssClass="ui-widget" ></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                RFQ: 
            </td>
            <td>
                <asp:TextBox ID="txtRFQ" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td> 
            <td>
                Part / Assembly Number: 
            </td>
            <td>
                <asp:TextBox ID="txtPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="ddlPartNumber" runat="server" CssClass="ui-widget" ></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                OEM: 
            </td>
            <td>
                <asp:DropDownList ID="ddlOEM" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
            <td>
                Part / Assembly Name:
            </td>
            <td>
                <asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="ddlPartName" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>
        <tr>
                        <td>
                No Disposition
            </td>
            <td>
                <asp:CheckBox ID="cbDisposition" runat="server" CssClass="ui-widget" />
            </td>
            <td>
                Customer: 
            </td>
            <td>
                <asp:TextBox ID="txtCustomer" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="ddlcustomerSearch" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>

            </td>
            <td>

            </td>
            <td>
                Customer Location: 
            </td>
            <td>
                <asp:TextBox ID="txtCustomerLocation" runat="server" CssClass="ui-widget"></asp:TextBox>
            </td>
            <td>
                <asp:DropDownList ID="ddlCustomerLocation" runat="server" CssClass="ui-widget"></asp:DropDownList>
            </td>
        </tr>

        </center>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                What are you trying to find?
            </td>
        </tr>
        <tr>
            <td>
                Quote
            </td>
            <td>
                <asp:CheckBox ID="cbQuote" runat="server" CssClass="ui-widget" />
            </td>
        </tr>
        <tr>
            <td>
                RFQ
            </td>
            <td>
                <asp:CheckBox ID="cbRFQ" runat="server" CssClass="ui-widget" />
            </td>
        </tr>
        <tr>
            <td>
                Part Info
            </td>
            <td>
                <asp:CheckBox ID="cbPart" runat="server" CssClass="ui-widget" />
            </td>
        </tr>
    </table>
    <center>
    <asp:Button ID="btnFind" runat="server"  Text="Find" OnClick="btnFind_Click"  CssClass="mybutton"  />
        &nbsp;&nbsp;
        <asp:Button ID="btnExport" Text="Export" runat="server" OnClick="btnExport_Click" Visible="false" CssClass="mybutton" />
    </center>
            

    <center>
        <td valign="top" width="85%" align="center">
            <h4>Results</h4>
            <asp:GridView ID="dgResults" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="rfqID" HeaderText="RFQ ID" DataNavigateUrlFormatString="https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id={0}" DataTextField="rfqid" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                    <asp:BoundField DataField="rstRFQStatusDescription" HeaderText="RFQ Status" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="ShipToName" HeaderText="Plant" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="customerContact" HeaderText="Customer Contact" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="rfqCustomerRFQNumber" HeaderText="Customer RFQ #" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="ProgramName" HeaderText="Program" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="OEMName" HeaderText="OEM" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="vehVehicleName" HeaderText="Vehicle" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="rsoSourceName" HeaderText="RFQ Source" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="salesman" HeaderText="Salesman" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <%--<asp:BoundField DataField="rfqCreatedBy" HeaderText="Created By" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>--%>
                    <asp:BoundField DataField="rfqDueDate" HeaderText="Due Date" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="rfqDateReceived" HeaderText="Received Date" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="rfqCreated" HeaderText="Created Date" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                </Columns>
            </asp:GridView>

            <asp:GridView ID="dgQuote" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="quoteLink" HeaderText="Quote ID" DataTextField="quoteID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                    <asp:BoundField DataField="quoteNumber" HeaderText="Quote Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partNumber" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partDescription" HeaderText="Part Name" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:HyperLinkField DataNavigateUrlFields="rfqLink" HeaderText="RFQ ID / Sales Order #" DataTextField="rfqid" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                    <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="customerLocation" HeaderText="Plant" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="customerContact" HeaderText="Contact" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="customerRFQNum" HeaderText="Customer RFQ #" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="salesman" HeaderText="Salesman" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="estimator" HeaderText="Estimator" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="quoteStatus" HeaderText="Quote Status" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="price" HeaderText="Total Price" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="dieType" HeaderText="Die Type" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="cavity" HeaderText="Cavity" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="created" HeaderText="Created Date" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="sent" HeaderText="Date Sent" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="dispositionButton" HeaderText="Set Disposition" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HtmlEncode="false"></asp:BoundField>
                </Columns>
            </asp:GridView>

<%--            <asp:GridView ID="dgECQuote" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="ecqECQuoteID" HeaderText="Quote ID" DataNavigateUrlFormatString="https://tsgrfq.azurewebsites.net/EditQuote.aspx?id={0}&quoteType=1" DataTextField="ecqECQuoteID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                    <asp:BoundField DataField="quoteNumber" HeaderText="Quote Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <%--<asp:BoundField DataField="quoteNumber" HeaderText="Quote Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <%--<asp:HyperLinkField DataNavigateUrlFields="rfqID" HeaderText="RFQ ID" DataNavigateUrlFormatString="https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id={0}" DataTextField="rfqid" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="ecqCustomerContactName" HeaderText="Customer Contact" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="ecqCustomerRFQNumber" HeaderText="Customer RFQ #" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="estimator" HeaderText="Estimator" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="qstQuoteStatusDescription" HeaderText="Quote Status" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="cost" HeaderText="Total Price" DataFormatString="{0:C}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="dtyFullName" HeaderText="Die Type" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="cavCavityName" HeaderText="Cavity" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="ecqCreated" HeaderText="Created Date" dataformatstring="{0:MM/dd/yyyy}" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                </Columns>
            </asp:GridView>--%>


            <asp:GridView ID="gvPart" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField HeaderText="View RFQ" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:DynamicHyperLink ID="view" NavigateUrl='<%# String.Format("EditRFQ?id={0}", Eval("rfqID")) %>' runat="server" Target="_blank" Text="<div class='mybutton' style='cursor: pointer;'>View RFQ</div>"></asp:DynamicHyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="rfqID" HeaderText="RFQ ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partNumber" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partID" HeaderText="Part ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partLength" HeaderText="Part Length" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partWidth" HeaderText="Part Width" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="partHeight" HeaderText="Part Height" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:hyperlinkfield headertext="Part Picture" datatextfield="partPicture" />
                    <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="plant" HeaderText="Plant" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="created" HeaderText="Created Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                    <asp:BoundField DataField="dueDate" HeaderText="Due Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                </Columns>
            </asp:GridView>



            <asp:Label ID="lbltotalCost" runat="server"></asp:Label>
            <br />
            <asp:Label ID="lblTruncated" runat="server"></asp:Label>
        </td>
    </center>

    <div id="dispositionDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <%--<label class="ui-widget">Applies To: </label><br />--%>
        <asp:TextBox id="txtQuoteAppliesTo" runat="server" style="visibility: hidden;" ReadOnly="true" CssClass="ui-widget"></asp:TextBox><br />
        Win / Loss<br />
        <asp:DropDownList ID="ddlWinLoss" runat="server"></asp:DropDownList><br />
        Reason<br />
        <asp:DropDownList ID="ddlWinLossReason" runat="server"></asp:DropDownList><br />
        PO #<br />
        <asp:TextBox ID="txtPONum" runat="server" CssClass="ui-widget"></asp:TextBox><br />
        Awarded Amount<br />
        <asp:TextBox ID="txtAwarded" runat="server" CssClass="ui-widget"></asp:TextBox><br />
        Target Price<br />
        <asp:TextBox ID="txtTargetPrice" runat="server"></asp:TextBox><br />
        Notes<br />
        <textarea id="dispositionNotes" runat="server" rows="6" style="max-width: 400px; width: 400px"></textarea><br />
        <button class="mybutton" onclick="applyDisposition();"  >Apply</button>
    </div>


    <script>
        function showDisposition(quoteInfo, winLoss, winLossReason, po, aa, tp, notes) {
            if (quoteInfo != $('#MainContent_txtQuoteAppliesTo').val()) {
                $('#MainContent_txtPONum').val('');
                $('#MainContent_txtAwarded').val('');
                $('#MainContent_txtTargetPrice').val('');
                $('#MainContent_dispositionNotes').val('');
                $('#MainContent_ddlWinLoss').val(1);
                $('#MainContent_ddlWinLossReason').val(1);
            }
            $('#MainContent_txtQuoteAppliesTo').val(quoteInfo);
            if (winLoss != '' && winLoss != '0') {
                if (winLoss != '') {
                    $('#MainContent_ddlWinLoss').val(winLoss);
                }
                else {
                    $('#MainContent_ddlWinLoss').val(1);
                }
                if (winLossReason != '' && winLossReason != '0') {
                    $('#MainContent_ddlWinLossReason').val(winLossReason);
                }
                else {
                    $('#MainContent_ddlWinLossReason').val(1);
                }
                $('#MainContent_txtPONum').val(po);
                $('#MainContent_txtAwarded').val(aa);
                $('#MainContent_txtTargetPrice').val(tp);
                $('#MainContent_dispositionNotes').val(notes);
            }
            $('#dispositionDialog').dialog({
                width: 500, height: 600, appendTo: "form"
            });
        }

        function applyDisposition() {
            url = 'SetDisposition.aspx?quoteID=' + $('#MainContent_txtQuoteAppliesTo').val() + '&winLoss=' + $('#MainContent_ddlWinLoss').val() + '&winLossReason=' + $('#MainContent_ddlWinLossReason').val() + '&targetPrice=' + $('#MainContent_txtTargetPrice').val() + '&PO=' + $('#MainContent_txtPONum').val() + '&Awarded=' + $('#MainContent_txtAwarded').val() + '&notes=' + escape($('#MainContent_dispositionNotes').val()) + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { } })
        }
    </script>
</asp:Content>