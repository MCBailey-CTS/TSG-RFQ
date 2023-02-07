<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STSQuoteDashboard.aspx.cs" Inherits="RFQ.STSQuoteDashboard" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .cssPager td {
            padding-left: 6px;
            padding-right: 6px;
        }

        textarea { 
            width: 95%; 
            height: 9em; 
            border: 0; 
        }
    </style>
    <div style="height: 50px;"></div>
    <div class="container">
        <div class="col-lg-2">
            <div class="row">
                <div class="col-lg-3">
                    Estimator
                </div>
                <div class="col-lg-9">
                    <asp:DropDownList ID="ddlEstimator" runat="server" style="width: 160px;"></asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">
                    RFQ
                </div>
                <div class="col-lg-9">
                    <asp:TextBox ID="txtRFQ" runat="server" CssClass="ui-widget" style="width: 160px;"></asp:TextBox>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">
                    Salesman
                </div>
                <div class="col-lg-9">
                    <asp:DropDownList ID="ddlSalesman" runat="server" CssClass="ui-widget"  style="width: 160px;"></asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">
                    Customer
                </div>
                <div class="col-lg-9">
                    <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true" style="width: 160px;"></asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-3">
                    Plant
                </div>
                <div class="col-lg-9">
                    <asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget" style="width: 160px;"></asp:DropDownList>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-12">
                    <center>
                        <asp:Button ID="btnFind" runat="server" CssClass="ui-widget mybutton" OnClick="btnFind_Click" Text="Find" />
                    </center>
                </div>
            </div>
        </div>
        <div class="col-lg-10">
            <div class="row">
                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="false" ViewStateMode="Enabled" AllowSorting="true" OnSorting="OnSort"
                    EnableSortingAndPagingCallbacks="false" AllowPaging="true" PageSize="50" OnPageIndexChanging="OnPaging">
                    <Columns>
                        <asp:TemplateField HeaderText="Quote #" SortExpression="squSTSQuoteID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:HyperLink ID="hlQuoteNum" runat="server" NavigateUrl='<%# String.Format("CreateQuote.aspx?quoteNumber={0}&quoteType=4", Eval("QuoteID")) %>' Text='<%# Eval("QuoteNumber") %>' Target="_blank"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer" SortExpression="Customer.CustomerID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblCustomer" runat="server" Text='<%# Eval("Customer") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="RFQ" SortExpression="qtrRFQID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:HyperLink ID="lblRFQId" runat="server" NavigateUrl='<%# String.Format("EditRFQ.aspx?id={0}", Eval("Rfq")) %>' Text='<%# Eval("Rfq") %>' Target="_blank"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cust RFQ #" SortExpression="squCustomerRFQNum" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblCustRFQ" runat="server" Text='<%# Eval("CustomerRFQ") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Part" SortExpression="squPartNumber" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblPartNumber" runat="server" Text='<%# Eval("PartNumber") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Salesman" SortExpression="TSGSalesman.Name" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblSalesman" runat="server" Text='<%# Eval("Salesman") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estimator" SortExpression="estLastName" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblEstimator" runat="server" Text='<%# Eval("Estimator") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cost" SortExpression="Cost" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblCost" runat="server" Text='<%# Eval("Cost") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date Created" SortExpression="squSTSQuoteID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblCreatedDate" runat="server" Text='<%# Eval("DateCreated") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                       
                        <asp:HyperLinkField headertext="Create EC" datatextfield="CreateECButton" HeaderStyle-CssClass="ui-widget-content"/> 
                       
                        <asp:TemplateField HeaderText="Approvals" SortExpression="squSTSQuoteID" HeaderStyle-HorizontalAlign="Center" HeaderStyle-CssClass="ui-widget" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Literal ID="litApprovalButton" runat="server" Text='<%# Eval("ApprovalButton") %>'></asp:Literal>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <PagerStyle CssClass="cssPager" />
                    <PagerSettings Mode="NumericFirstLast" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="approvalDialog" style="display:none; padding: 20px; background-color: #D0D0D0; width: 800px; height: 400px;">

        <div class="row">
            <center>
                <label id="lblHistory" ></label>
            </center>
        </div>
        <div class="row">
            <div class="col-lg-12">
                &nbsp
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    Approval Comments
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <textarea id="txtApprovalComment" cols="50" rows="5" ></textarea>
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <center>
                    <button id="btnApprove" class="ui-widget mybutton" onclick="approveQuote();return false;">Approve</button>
                </center>
            </div>
            <div class="col-lg-6">
                <center>
                    <button id="btnReject" class="ui-widget mybutton" onclick="rejectQuote();return false;">Reject</button>
                </center>
            </div>
        </div>
    </div>

    <div id="submitApprovalDialog" style="display: none; padding: 20px; background-color: #D0D0D0; width: 900px; height: 400px;">
        <div class="row">
            <div class="col-lg-12">
                <center>
                    Please upload required quote files and any comments.
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <center>
                    Project Manager
                </center>
            </div>
            <%--<div class="col-lg-4">
                <center>
                    Customer Emails (separate by ;)
                </center>
            </div>--%>
            <div class="col-lg-6">
                <center>
                    Quote attachments
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <center>
                    <asp:DropDownList ID="ddlProjectManager" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </center>
            </div>
            <%--<div class="col-lg-4">
                <center>
                    <asp:TextBox ID="txtApproverEmail" runat="server" CssClass="ui-widget" TextMode="MultiLine" style="width: 300px; height: 150px;"></asp:TextBox>
                </center>
            </div>--%>
            <div class="col-lg-6">
                <center>
                    <asp:FileUpload ID="fuQuote" runat="server" AllowMultiple="true" />
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <asp:Button ID="btnApproval" runat="server" CssClass="ui-widget mybutton" Text="Send for Approval" OnClick="btnApproval_Click" style="visibility: hidden;" />
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <Button id="btnSubmitApproval" onclick="checkApproval(); return false;" class="mybutton">Submit for Approval</Button>
                </center>
            </div>
        </div>
    </div>

    <asp:Literal ID="litScript" runat="server"></asp:Literal>
    <asp:HiddenField ID="hdnQuoteID" runat="server" />

    <script>
 
        function openApprovalDialog() {
            $('#approvalDialog').dialog({ width: 900, height: 500 });
            $('#approvalDialog').parent().appendTo("form");
        }

        function checkApproval() {
//           if ($('#MainContent_fuQuote').val() == '') {
//            alert('Please upload quote document before submitting for approval.');
//            return;
//            }
           if ($('#MainContent_ddlProjectManager').val() == "Please Select") {
               alert('Please select a project manager before submitting for approval');
               return;
           }
           $('#MainContent_btnApproval').click();
        }

        function approval(quoteId) {
            var url = 'STSApprovals.aspx?quote=' + quoteId + '&history=true';
            $.ajax({
                url: url,
                success: function (response, status) {
                    $('#lblHistory').html(response);
                },
                failure: function (response) {
                    alert(response);
                }
            })

            $('#MainContent_hdnQuoteID').val(quoteId);

            $('#approvalDialog').dialog({ width: 1000, height: 500 });
        }

        function approveQuote() {
            var url = 'STSApprovals.aspx';
            $.ajax({
                url: url,
                data: {
                    quote: $('#MainContent_hdnQuoteID').val(),
                    approved: true,
                    approvalComments: $('#txtApprovalComment').val()
                },
                success: function (response, status) {
                    $('#approvalDialog').dialog('close');
                },
                failure: function (response) {
                    alert(response);
                }
            })

        }

        function rejectQuote() {
            var url = 'STSApprovals.aspx';
            $.ajax({
                url: url,
                data: {
                    quote: $('#MainContent_hdnQuoteID').val(),
                    approved: false,
                    approvalComments: $('#txtApprovalComment').val()
                },
                success: function (response, status) {
                    $('#approvalDialog').dialog('close');
                },
                failure: function (response) {
                    alert(response);
                }
            })
        }

        function submitForApproval(quoteId) {
            $('#submitApprovalDialog').dialog({ width: 900, height: 500 });
            $('#submitApprovalDialog').parent().appendTo("form");
            $('#MainContent_hdnQuoteID').val(quoteId);

            //var url = 'STSApprovals.aspx?quote=' + quoteId + '&submit=true';
            //$.ajax({
            //    url: url,
            //    success: function (response, status) {
            //        $('#btnApproval' + quoteId).replaceWith('Submitted for approval');
            //    },
            //    failure: function (response) {
            //        alert(response);
            //    }
            //})
        }
    </script>
</asp:Content>