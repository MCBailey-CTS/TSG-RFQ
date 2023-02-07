<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="QuoteDashboard.aspx.cs" Inherits="RFQ.QuoteDashboard" enableEventValidation="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .cssPager td {
            padding-left: 6px;
            padding-right: 6px;
        }
    </style>

    <div style="min-height: 100px"></div>

    <table width="100%" cellpadding="0" cellspacing="0" border="0"  >
        <tr>
            <td valign="top" width="20%" style="border: 1px dotted black;" class="ui-widget">
            <div class="mybutton" onclick="showQuoteType();" >Create New Quote</div>
            <div class="mybutton" onclick="uploadQuote();" id="quoteUploadButton">Upload EC Quote</div>
                <br />
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
                            Quote Status: 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Quote Type: 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlQuoteType" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            RFQ: 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlRFQID" runat="server" CssClass="ui-widget"></asp:DropDownList>
                            <asp:TextBox ID="txtRFQ" runat="server" CssClass="ui-widget"></asp:TextBox>
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
                            Quote Num: 
                        </td>
                        <td>
                            <asp:TextBox ID="txtQuoteNumber" runat="server" CssClass="ui-widget"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Salesman: 
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlSalesman" runat="server" CssClass="ui-widget"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:CheckBox ID="chkDueDate" runat="server" CssClass="ui-widget" Text="Sort By Due Date" />
                        </td>
                    </tr>
                </table>
                <center>
                <asp:Button ID="btnFind" runat="server"  Text="Find" OnClick="btnFind_Click"  CssClass="mybutton"  />
                </center>
                <br />
                <asp:CheckBox ID="chkReserved" runat="server" Text="Reserved Parts" OnCheckedChanged="ReservedChecked" AutoPostBack="true" />
                <br />
                <asp:CheckBox ID="chkOpenQuotes" runat="server" Text="Open Quotes" OnCheckedChanged="OpenChecked" AutoPostBack="true"/>
                <br />
                <asp:CheckBox ID="chkUnreserved" runat="server" Text="Unreserved Parts" OnCheckedChanged="UnreservedChecked" AutoPostBack="true"/>
                <br />
                <asp:CheckBox ID="chkUnReservedList" runat="server" Text="Parts no longer reserved" OnCheckedChanged="chkUnReservedList_CheckedChanged" AutoPostBack="true" />
                <%--<asp:CheckBox ID="chkDisposition" runat="server" Text="Enter Disposition" OnCheckedChanged="dispositionChecked" AutoPostBack="true" />--%>
            </td>
            <td valign="top" width="85%" align="center">

                <asp:Literal ID="companiesNotified" runat="server"></asp:Literal>
        <font color="Red">The amount of quotes displayed has been limited, please use the search feature to find old quotes</font>
        <asp:GridView ID="dgReserved" runat="server" AutoGenerateColumns="false" HorizontalAlign="Center" Caption="Reserved Parts">
            <Columns>
                <asp:TemplateField HeaderText="View RFQ" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DynamicHyperLink ID="view" NavigateUrl='<%# String.Format("EditRFQ?id={0}", Eval("rfqID")) %>' runat="server" Target="_blank" Text="<div class='mybutton' style='cursor: pointer;'>View RFQ</div>"></asp:DynamicHyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="rfqID" HeaderText="RFQ ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partNumber" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partID" HeaderText="Part ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:hyperlinkfield headertext="Part Picture" datatextfield="partPicture" ItemStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget-content" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="tsgCompany" HeaderText="TSG Company" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HtmlEncode="false"></asp:BoundField>
                <asp:BoundField DataField="reservedBy" HeaderText="Reserved By" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="reserved" HeaderText="Reserved Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="dueDate" HeaderText="Due Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:TemplateField HeaderText="Part Notes" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox ID="txtPartNote" runat="server" Text='<%# Eval("partNote") %>' TextMode="MultiLine" Rows="2" Height="50px" Width="300px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
 
        <asp:Button ID="SaveButton1" runat="server"  Text="Save Part Notes" OnClick="savePartNotes"  CssClass="mybutton"  />
                <br />
                <br />

        <asp:GridView ID="dgResults" runat="server" AutoGenerateColumns="false" Caption="Open Quotes">
            <Columns>
                <%--<asp:TemplateField HeaderText="Edit Quote" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%--<asp:DynamicHyperLink ID="sel"   NavigateUrl='<%# Eval("realQuoteID","~/EditQuote.aspx?id={0}") %>'   runat="server" Target="_blank" Text="<div class='mybutton' style='cursor: pointer;'>Select</div>"></asp:DynamicHyperLink>--
                        <asp:DynamicHyperLink ID="sel"   NavigateUrl='<%# String.Format("~/EditQuote.aspx?id={0}&quoteType={1}", Eval("realQuoteID"), Eval("quoteTypeNum")) %>'   runat="server" Target="_blank" Text="<div class='mybutton' style='cursor: pointer;'>Edit Quote</div>"></asp:DynamicHyperLink>
                    </ItemTemplate>
                </asp:TemplateField>--%>
                <asp:hyperlinkfield headertext="Edit Quote" datatextfield="realQuoteID" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:hyperlinkfield headertext="View RFQ" datatextfield="rfqID" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:BoundField DataField="quoteID" HeaderText="Quote ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partNumber" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partID" HeaderText="Part ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:hyperlinkfield headertext="Part Picture" datatextfield="partPicture" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:BoundField DataField="status" HeaderText="Quote Status" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="tsgCompany" HeaderText="TSG Company" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="estimator" HeaderText="Estimator" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="created" HeaderText="Created Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="dueDate" HeaderText="Due Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
<%--                <asp:TemplateField HeaderText="Part Notes" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox ID="txtPartNote" runat="server" Text="" Height="50px" Width="300px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>--%>
            </Columns>
        </asp:GridView>
        <%--<asp:Button ID="SaveButton2" runat="server"  Text="Save Part Notes" OnClick="savePartNotes"  CssClass="mybutton"  />--%>

        <br />
        <br />
        <asp:GridView ID="dgUnreserved" runat="server" AutoGenerateColumns="false" OnRowDataBound="OnRowDataBound" Caption="Unreserved Parts">
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
                <asp:TemplateField HeaderText="Reserve" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlReserve" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="No Quote" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlNoQuote" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part Notes" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:TextBox ID="txtPartNote" runat="server" Text='<%# Eval("partNote") %>' Height="50px" Width="300px"></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Button ID="SaveButton3" runat="server"  Text="Save Reservations and notes" OnClick="reserve"  CssClass="mybutton"  />


        <asp:GridView ID="dgDisposition" runat="server" AutoGenerateColumns="false" Caption="Disposition">
            <Columns>
                <asp:hyperlinkfield headertext="Edit Quote" datatextfield="realQuoteID" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:hyperlinkfield headertext="View RFQ" datatextfield="rfqID" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:BoundField DataField="quoteID" HeaderText="Quote ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <%--<asp:BoundField DataField="partNumber" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>--%>
                <%--<asp:hyperlinkfield headertext="Part Picture" datatextfield="partPicture" HeaderStyle-CssClass="ui-widget-content"/>--%>
                <asp:BoundField DataField="status" HeaderText="Quote Status" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="tsgCompany" HeaderText="TSG Company" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="estimator" HeaderText="Estimator" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField headertext="Salesman" DataField="partPicture" HeaderStyle-CssClass="ui-widget-content"/>
                <asp:BoundField DataField="created" HeaderText="Created Date" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="quoteType" HeaderText="Disposition" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HtmlEncode="false"></asp:BoundField>
            </Columns>
        </asp:GridView>

        <asp:GridView ID="gvUnReserved" runat="server" CssClass="align-center" ViewStateMode="Enabled" AutoGenerateColumns="false" AllowPaging="true" PageSize="50" OnPageIndexChanging="OnPaging" >
            <Columns>
                <asp:TemplateField HeaderText="View RFQ" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DynamicHyperLink ID="view" NavigateUrl='<%# String.Format("EditRFQ?id={0}", Eval("rfqID")) %>' runat="server" Target="_blank" Text="<div class='mybutton' style='cursor: pointer;'>View RFQ</div>"></asp:DynamicHyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="partNum" HeaderText="Part Number" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partName" HeaderText="Part Name" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partLength" HeaderText="Part Length" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partWidth" HeaderText="Part Width" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="partHeight" HeaderText="Part Height" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <%--<asp:hyperlinkfield headertext="Part Picture" datatextfield="partPicture" />--%>
                <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="plant" HeaderText="Plant" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="firstReserved" HeaderText="First Reserved" DataFormatString="{0:d}" HtmlEncode=false ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="dueDate" HeaderText="RFQ Due Date" DataFormatString="{0:d}" HtmlEncode=false ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>
                <asp:BoundField DataField="unreservedDate" HeaderText="UnReserved Date" DataFormatString="{0:d}" HtmlEncode=false ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="company" HeaderText="Company" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="name" HeaderText="Name" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
            </Columns>

            <PagerStyle CssClass="cssPager" />
            <PagerSettings Mode="NumericFirstLast" />
        </asp:GridView>

        <asp:Button ID="nextButton" runat="server" Text="Next" OnClick="next" CssClass="mybutton" />

        </td>
        </tr>
    </table>

    <asp:FileUpload ID="uploadFile" runat="server" AllowMultiple="true" style="opacity: 0; visibility: hidden;" />

    <div id="messageDialog" style="display: none;"></div>
    <asp:Literal ID="litReserve" runat="server" ></asp:Literal>
    <div id="QuoteDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
            <label class="ui-widget">Select Quote Type</label><br />
            <asp:DropDownList ID="ddlQuoteType2" runat="server"></asp:DropDownList>
            <br /><br />
            <asp:Button ID="btnNewQuote" runat="server" Text="Create Quote" OnClick="btnNewQuote_Click" CssClass="mybutton" />
    </div>
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

        // this function is always called by the footer.aspx
        function page_init() {
            $("#MainContent_uploadFile").fileupload({
                url: 'QuoteUpload.ashx?rfqID=0&rand=' + Math.random(),
                add: function (e, data) {
                    data.submit();
                },
                success: function (response, status) {
                    if (response.substring(0, 2) == 'OK') {
                        if (response.split("|")[2] != "") {
                            url = "https://tsgrfq.azurewebsites.net/EditQuote.aspx?id=" + response.split("|")[2] + "&quoteType=1";
                            window.open(url);
                        }
                        
                        //alert(response);
                    } else {
                        alert(response);
                    }
                },
                error: function (error) {
                    // this error means the page actually errored out and you need to figure out what the error was
                    alert('Error Accessing Quote Upload Page');
                }
            });


        }
    </script>

    <script>
        function showMessage(msg) {
            $('#messageDialog').html(msg);
            $('#messageDialog').dialog();
        }

        function showDisposition(partID) {
            $('#MainContent_txtQuoteAppliesTo').val(partID);
            $('#dispositionDialog').dialog({
                width: 500, height: 600, appendTo: "form"
            });
        }

        function showQuoteType() {
            $('#QuoteDialog').dialog({
                width: 200, height: 200, appendTo: "form"
            });
        }

        function uploadQuote() {
            $('#MainContent_uploadFile').click();
        }

        function applyDisposition() {
            url = 'SetDisposition.aspx?quoteID=' + $('#MainContent_txtQuoteAppliesTo').val() + '&winLoss=' + $('#MainContent_ddlWinLoss').val() + '&winLossReason=' + $('#MainContent_ddlWinLossReason').val() + '&targetPrice=' + $('#MainContent_txtTargetPrice').val() + '&PO=' + $('#MainContent_txtPONum').val() + '&Awarded=' + $('#MainContent_txtAwarded').val() + '&notes=' + $('#MainContent_dispositionNotes').val() + '&rand=' + Math.random();
            $.ajax({ url: url, success: function (data) { } })
        }

    </script>
    <script src="blueimp/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.fileupload.js" type="text/javascript"></script>
        <asp:Label ID="lblMessage" runat="server"></asp:Label>

</asp:Content>