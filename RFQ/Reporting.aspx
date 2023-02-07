<%@ Page Title="Reporting" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reporting.aspx.cs" Inherits="RFQ.Reporting" %>


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

    <table>
        <tr>
            <td class="ui-widget">No Quote: &nbsp&nbsp&nbsp</td><td><asp:Label ID="lblNoQuotePerc" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td class="ui-widget">On Time: &nbsp&nbsp&nbsp</td><td><asp:Label ID="lblOnTimePerc" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td class="ui-widget">1-7 Days Late: &nbsp&nbsp&nbsp</td><td><asp:Label ID="lbl1to7Perc" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td class="ui-widget">> 7 Days Late: &nbsp&nbsp&nbsp</td><td><asp:Label ID="lblMoreThanPerc" runat="server"></asp:Label></td>
        </tr>
        
    </table>

    <asp:GridView ID="gvSalesman" runat="server" AutoGenerateColumns="false" CssClass ="table table-striped" HeaderStyle-CssClass="header-right" >
        <Columns>
            <asp:BoundField HeaderText ="Salesman" DataField="Name"  />
            <asp:BoundField HeaderText ="Open Quotes" DataField="OpenQuotes" DataFormatString="{0:n0}"  ItemStyle-HorizontalAlign="right" />
            <asp:BoundField HeaderText ="Value" DataField="QuoteDollars"  DataFormatString="{0:n0}"  ItemStyle-HorizontalAlign="right" HeaderStyle-HorizontalAlign="right"/>
            <asp:BoundField HeaderText="% # Quote" DataField="PercentOfQuotes"  DataFormatString="{0:n2}%"  ItemStyle-HorizontalAlign="right" HeaderStyle-HorizontalAlign="right"/>
            <asp:BoundField HeaderText="% $ Quote" DataField="PercentOfDollars"  DataFormatString="{0:n2}%"  ItemStyle-HorizontalAlign="right" HeaderStyle-HorizontalAlign="right"/>
            <asp:BoundField HeaderText ="Average Die Cost" DataField ="AverageDieCost"  DataFormatString="{0:n0}"  ItemStyle-HorizontalAlign="right" HeaderStyle-HorizontalAlign="right"/>
        </Columns>
    </asp:GridView>

    <asp:GridView ID="gvSalesmanQuotes" runat="server">
    </asp:GridView>

    <%--<asp:GridView ID="gvCompanyQuotes" runat="server" OnDataBound="gvCompanyQuotes_DataBound" Visible="false"></asp:GridView>--%>
    <div style="float: left;">
        <h3 id="quoteNum-title"></h3>
        <div id="quoteNum-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="float: left;">
        <h3 id="quotePrice-title"></h3>
        <div id="quotePrice-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="clear: both;"></div>
    <div style="float: left;">
        <h3 id="salesmanquotes-title">Quotes By Sales Rep</h3>
        <div id="salesmanquotes-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="float: left;">
        <h3 id="companyquotes-title">Quotes By Company</h3>
        <div id="companyquotes-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="clear: both;"></div>
    <div style="float: left;">
        <h3 id="ontimevlate-title">On Time vs. Late YTD</h3>
        <div id="ontimevlate-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="clear: both;"></div>
    <div style="float: left;">
        <h3 id="ontimevlatedetail-title">On Time vs. Late YTD</h3>
        <div id="ontimevlatedetail-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="clear: both;"></div>
    <div style="float: left;">
        <h3 id="noquote-title">No Quotes</h3>
        <div id="noquote-placeholder" class="graph-placeholder"></div>
    </div>
    <div style="clear: both;"></div>


	<script type="text/javascript" src="Scripts/jquery.flot.js"></script>
	<script type="text/javascript" src="Scripts/jquery.flot.pie.js"></script>
    <script type="text/javascript" src="Scripts/jquery.flot.time.js"></script>
    <script type="text/javascript" src="Scripts/jquery.flot.tickrotor.js"></script>
    <script type="text/javascript" src="Scripts/jquery.flot.barnumbers.js"></script>

    <script type="text/javascript">

        // this will draw a barchart or line chart
        // div is the id of the div tag where to draw the chart
        // data is an array of arrays 
        // first array is the legend
        // ticks are the titles underneath the bars
        // showbars and showlines are boolean
        function drawChart(div, data, ticks, bar_or_line,  rotate) {
            if (typeof rotate === 'undefined') { rotate = 0; }
            var legend = data[0];
            var datarows = Array(legend.length);
            var myticks = [];
            // should not matter what this is set to. by not setting the width, flot will make it fit the container.
            var bwidth = 1;
            for (i=0; i < datarows.length; i++) 
            {
                datarows[i]= [];
            }
            offset = bwidth * (legend.length + 2);
            base = 5 + bwidth / 2 * legend.length;
            for (i = 0; i < ticks.length; i++) {
                myticks.push([base, ticks[i]]);
                base = base + offset;
            }
            base = 5;
            for (row = 1; row < data.length; row++) {
                for (i=0; i < legend.length; i++) 
                {
                    idx = i * bwidth;
                    datarows[i].push([base+idx, data[row][i]]);
                }
                base = base + offset;
            }
            
            alldata = Array();
            for (i = 0; i < legend.length; i++) {
                if (bar_or_line == 'bar') {
                    alldata[i] = { data: datarows[i], label: legend[i], bars: { show: true, fill: true, barWidth: bwidth }};
                } else {
                    alldata[i] = { data: datarows[i], label: legend[i] };
                }
            }

            $.plot("#"+ div + "-placeholder", alldata, 
             { xaxis: { ticks: myticks, tickColor: 'white', rotateTicks: rotate  } }
            );
        }
    </script>

    <asp:Literal ID="litJSSalesManQuote" runat="server"></asp:Literal>
    <asp:Literal ID="litJSCompanyQuote" runat="server"></asp:Literal>
    <asp:Literal ID="litJSOnTimeVLate" runat="server"></asp:Literal>
    <asp:Literal ID="litJSOnTimeVLateDetail" runat="server"></asp:Literal>
    <asp:Literal ID="litJSNoQuote" runat="server"></asp:Literal>
    <asp:Literal ID="litNumOfQuotes" runat="server"></asp:Literal>
    <asp:Literal ID="litPriceOfQuotes" runat="server"></asp:Literal>
    <asp:Literal ID="litQuoteOnTime" runat="server"></asp:Literal>
    <asp:Literal ID="litOEM" runat="server"></asp:Literal>


</asp:Content>