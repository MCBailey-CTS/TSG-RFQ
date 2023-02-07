<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateJobSite.aspx.cs" Inherits="RFQ.CreateJobSite" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 50px"></div>


    <div>
        <center>
    	    <h3>Create a New Job.</h3>
        </center>
    </div>

    <center>
	    <table id="t01">
            <tr>
	            <td>
                    TSG Job number: 
  	            </td>	
                <td>
                    <asp:TextBox ID="txtJobNumber" runat="server" CssClass="ui-widget"></asp:TextBox>
                    <%--<asp:Label ID="lblJobNumber" runat="server" CssClass="ui-widget"></asp:Label>
                    <asp:Label ID="lblShortJobNum" runat="server" CssClass="ui-widget"></asp:Label>--%>
                </td>
                <%--<td>
                    <asp:Label ID="lblApndText" runat="server" CssClass="ui-widget">Append to end of Job Number:</asp:Label>
  	            </td>	--%>
                <%--<td>
                    <asp:TextBox ID="txtJobNumApnd" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>--%>
	            <td>
	                TSG company: 
  	            </td>	
                <td>
                    <asp:DropDownList ID="ddlTSGCompany" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
	        </tr>	
	        <tr>
	            <td>
                    TSG Project manager: &nbsp
  	            </td>	
                <td>
                    <asp:DropDownList ID="ddlProjectManager" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
	            <td>
                    TSG Estimating contact: 
  	            </td>	
                <td>
                    <asp:DropDownList ID="ddlEstimator" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
	        </tr>	
            <tr>
	            <td>
	                Select Customer: 
  	            </td>	
                <td>
                    <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                </td>
	            <td>
	                Customer location: 
	            </td>
                <td>
                    <asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
            </tr>
	        <tr>
	            <td>
                    TSG Sales Rep: 
  	            </td>	
                <td>
                    <asp:Label ID="lblSalesman" runat="server"></asp:Label>
                </td>
	            <td>
                    Customer Contact: 
	            </td>
                <td>
                    <asp:TextBox ID="txtCustomerContact" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>
	        </tr>	
	        <tr>
	            <td>
                    TSG Quote number: 
  	            </td>
                <td>
                    <asp:TextBox ID="txtQuoteNum" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>
	            <td>
                    Amount: 
  	            </td>
                <td>
                    <asp:TextBox ID="txtAmount" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>
	        </tr>
	        <tr>
	            <td>
	                Part name: 
  	            </td>
                <td>
                    <asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>
	            <td>
	                Customer Part Number: 
	            </td>
                <td>
                    <asp:TextBox ID="txtCustomerPartNumber" CssClass="ui-widget" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
	            <td>
	                Program: 
  	            </td>
                <td>
                    <asp:TextBox ID="txtProgram" runat="server" CssClass="ui-widget"></asp:TextBox>
                </td>
	            <td>
	                OEM: 
	            </td>
                <td>
                    <asp:DropDownList ID="ddlOEM" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Link to job: 
                </td>
                <td>
                    <asp:DropDownList ID="ddlLinkedJob" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    Note: This will not add the job to a TSG Program site.<br />
	                If the job is part of a program that has a TSG program<br />
                    site then add the job to the program site using<br />
	                the Add job to program application.<br><br> 
                </td>
                <td colspan="2">

                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <center>
                        <asp:Label ID="lblSiteCreated" runat="server"></asp:Label>
                    </center>
                </td>
            </tr>
        </table>
        <asp:CheckBox ID="chkSharepoint" runat="server" CssClass="ui-widget" Text="Create SharePoint Site" Checked="true" /><br/>
        <asp:CheckBox ID="chkMasterboard" runat="server" CssClass="ui-widget" Text="Create Masterboard / Capacity Planning" Checked="true" />
	    <br/><asp:Button ID="btnCreate" runat="server" CssClass="ui-widget mybutton" Text="Create Job" OnClick="createJob" />

	    App Version 1.0 - 7/18/2016</br/>
    </center>

</asp:Content>