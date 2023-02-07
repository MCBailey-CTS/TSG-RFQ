<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RFQ.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .pie-placeholder {
		    width: 450px;
            height: 450px;
	    }
        /*.bar-placeholder {
            width: 450px;
            height: 450px;
        }*/
    </style>

    <div style="min-height: 150px;"></div>

    <center><h1>Welcome to TSG's Request For Quote Application</h1></center>
    <center>
        <div style="float: none; margin-left: 20px;">
            <h3 id="IP-title"></h3>
		    <div id="IP-placeholder" class="pie-placeholder"></div>    
        </div>
        <%--<div style="float: none;">
            <h3 id="quoteNum-title"></h3>
            <div id="quoteNum-placeholder" class="bar-placeholder"></div>
        </div>--%>
        <div style="clear: both;"></div>
	    <script  type="text/javascript" src="Scripts/jquery.flot.js"></script>
	    <script  type="text/javascript" src="Scripts/jquery.flot.pie.js"></script>
        <script type="text/javascript" src="Scripts/jquery.flot.time.js"></script>
    	<asp:Literal ID="litIPChart" runat="server"></asp:Literal>
    </center>
    
    <br />
    <br />
    <br />

    <center><h2>TSG's Sites</h2></center>
    <center><h5>If there are any sites that you feel you should have access  please contact an administrator.</h5></center>
    <table>
<%--        <tr>
            <td width="50%" style="padding: 50px;" >
                <center><h2><a href="https://tsgrfq.azurewebsites.net" target="_blank">TSG RFQ and Quoting</a></h2></center>
                <center>
                    <h4>This is where all RFQs are processed and uploaded to.  This site is used for creating new RFQ and standalone quotes along with searching for old quotes and several custom reports.</h4>
                </center>
            </td>--%>
            <td width="50%" style="padding: 50px;">
                <center><h2><a href="https://tsgdashboards.azurewebsites.net" target="_blank">TSG Dashboards</a></h2></center>
                <center>
                    <h4>This was created to give a quick screen to search though any current jobs your shop may have and link to any helpful information.
                    From this site there are links to the sharepoint job site, masterboards, design statuses, capacity planning, atp forms and shipping forms.</h4>
                </center>
            </td>
            <td width="50%" style="padding: 50px;">
                <center><h2><a href="https://capacityplanning.azurewebsites.net" target="_blank">Capacity Planning</a></h2></center>
                <center>
                    <h4>This application was built to help track job schedules and graph current and future capcity estimations.</h4>
                </center>
            </td>
        </tr>
        <tr>
            <td width="50%" style="padding: 50px;">
                <center><h2><a href="https://tsgmasterboards.azurewebsites.net" target="_blank">TSG Masterboard</a></h2></center>
                <center>
                    <h4>The masterboards were brought online to give visibility for what is going on with each job on a weekly basis.</h4>
                </center>
            </td>
            <td width="50%" style="padding: 50px;">
                <center><h2><a href="https://tsgcrm.azurewebsites.net" target="_blank">TSG CRM</a></h2></center>
                <center>
                    <h4>This application was created to allow sales quickly access information on any of their given customers and keep track of visits and details.</h4>
                </center>

            </td>
        </tr>
    </table>
</asp:Content>
