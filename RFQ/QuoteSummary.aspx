<%@ Page Title="Quote Summary" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuoteSummary.aspx.cs" Inherits="RFQ.QuoteSummary" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        @media screen and (min-width: 300px) {
            .graph-placeholder {
                width: 300px;
                height: 112px;
            }
        }
        @media screen and (min-width: 600px) {
            .graph-placeholder {
                width: 600px;
                height: 225px;
            }
        }
        @media screen and (min-width: 800px) {
            .graph-placeholder {
                width: 800px;
                height: 300px;
            }
        }
        @media screen and (min-width: 1200px) {
            .graph-placeholder {
                width: 1200px;
                height: 450px;
            }
        }

        .header-right {
            text-align: right;
        }
    </style>
    <div style="min-height: 50px;"></div>
    
    <div align='center' style="padding: 4px;">
        <br />
        <h3>Reports</h3>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Report</th>
                    <th>Start Date</th>
                    <th>End Date</th>
                    <th>Customer</th>
                    <th>Plant</th>
                    <th>Create Report</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Quote Summary</td>
                    <td>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="datepicker" ></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="datepicker"></asp:TextBox>
                    </td>
                    <td colspan="2"></td>
                    <td>
                        <div class="ui-widget mybutton" onclick="quoteSummary();" >Generate Spreadsheet</div>
                        <%--<asp:Button ID="btnQuoteRecap" runat="server" OnClick="quoteSummary();" Text="Generate Spreadsheet" CssClass="mybutton" />--%>
                    </td>
                    <td>

                    </td>
                </tr>
                <tr>
                    <td>
                        6 Month Recap
                    </td>
                    <td colspan="4">
                    </td>
                    <td>
                        <asp:Button ID="btnRecap" runat="server" OnClick="btnRecap_Click" Text="Run Report" CssClass="mybutton" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Quote Report
                    </td>
                    <td>
                        <asp:TextBox ID="txtST" runat="server" CssClass="datepicker" ></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEN" runat="server" CssClass="datepicker"></asp:TextBox>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlCompany" runat="server"></asp:DropDownList>
                    </td>
                    <td></td>
                    <td>
                        <div class="ui-widget mybutton" id="btnQuoteReport" onclick="quoteReport();return false;">Quote Report</div>
                    </td>
                </tr>
                <tr>
                    <td>Customer Summary</td>
                    <td>
                        <asp:TextBox ID="txtStart" runat="server" CssClass="datepicker" ></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtEnd" runat="server" CssClass="datepicker"></asp:TextBox>
                    </td>
                    <td><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                    <td><asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                    <td>
                        <div class="ui-widget mybutton" id="btnCustomerSummary" onclick="customerReport()">Generate Customer Report</div>
                    </td>
                </tr>
                <tr>
                    <td>Won Quotes Report</td>
                    <td>
                        <asp:TextBox ID="txtS" runat="server" CssClass="datepicker"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtE" runat="server" CssClass="datepicker"></asp:TextBox>
                    </td>
                    <td colspan="2"></td>
                    <td>
                        <div class="ui-widget mybutton" id="btnWonReprot" onclick="wonReport();return false;">Won Quotes Report</div>
                    </td>
                </tr>
<%--                <tr>
                    <div id="testReport">
                        <td>Test Report</td>
                        <td colspan="4"></td>
                        <td>
                            <div class="ui-widget mybutton" id="btnTestReport" onclick="testReport();return false;">Test Report</div>
                        </td>
                    </div>
                </tr>--%>
            </tbody>
        </table>
         <asp:Literal runat="server" ID="litJSOpenReport"></asp:Literal>
</div>

    <script>
        function customerReport() {
            if ($('#MainContent_ddlCustomer :selected').text() == "Please Select") {
                alert('Please Choose a Customer');
            }
            else if ($('#MainContent_ddlPlant :selected').text() == "Please Select") {
                var url = 'RFQSummary.ashx?rfq=' + 0 + '&customer=' + $('#MainContent_ddlCustomer').val() + '&start=' + $('#MainContent_txtStart').val() + '&end=' + $('#MainContent_txtEnd').val();
                $.ajax({ url: url, success: function (data) { } });
                alert("The report will be emailed to you after it is generated, it may take several minuites");
            }
            else {
                var url = 'RFQSummary.ashx?rfq=' + 0 + '&customer=' + $('#MainContent_ddlCustomer').val() + '&plant=' + $('#MainContent_ddlPlant').val() + '&start=' + $('#MainContent_txtStart').val() + '&end=' + $('#MainContent_txtEnd').val();
                $.ajax({ url: url, success: function (data) { } });
                alert("The report will be emailed to you after it is generated, it may take several minuites");
            }
        }

        function quoteReport() {
            url = 'QuoteRecap.ashx?OnlyQuotes=yes&start=' + $('#MainContent_txtST').val() + '&end=' + $('#MainContent_txtEN').val() + '&company=' + $('#MainContent_ddlCompany').val();
            $.ajax({ url: url, success: function (data) { } });
            alert("The report will be emailed to you after it is generated, it will take several minuites");
        }

        function quoteSummary() {
            url = 'QuoteRecap.ashx?reserved=0&start=' + $('#MainContent_txtStartDate').val() + '&end=' + $('#MainContent_txtEndDate').val();
            $.ajax({ url: url, success: function (data) { } });
            alert("The report will be emailed to you after it is generated, it will take several minuites");
        }

        function wonReport() {
            url = 'WonQuotesReport.ashx?start=' + $('#MainContent_txtS').val() + '&end=' + $('#MainContent_txtE').val();
            $.ajax({ url: url, success: function (data) { } });
            alert("The report will be eamiled to you after it is generated, it may take several minutes");
        }

        function testReport() {
            url = 'TestReport.ashx';
            $.ajax({ url: url, success: function (data) { } });
            alert('The report will be emailed to you after it is generated, it may take several minutes');
        }
    </script>

</asp:Content>