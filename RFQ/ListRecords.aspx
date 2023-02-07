<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ListRecords.aspx.cs" Inherits="RFQ.ListRecords" enableEventValidation="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<link href="jqueryui/jquery-ui.min.css" rel="stylesheet" type="text/css">
<script src="Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
<script src="Scripts/bootstrap.min.js" type="text/javascript"></script>
<script src="Scripts/respond.min.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript" src="jqueryui/jquery-ui.min.js"></script>
    <br /><br />
    <center><p>
            Start Date: <asp:TextBox ID="txtStartDate" runat="server" CssClass="ui-widget datepicker"></asp:TextBox>&nbsp;&nbsp;
            End Date: <asp:TextBox ID="txtEndDate" runat="server" CssClass="ui-widget datepicker"></asp:TextBox>&nbsp;&nbsp;
            </p>
        <p>
        Enter Search value: <asp:TextBox ID="SearchFor" runat="server"></asp:TextBox>&nbsp;&nbsp;
		<asp:Button 
			ID="btnApply" Text="Find/Apply" runat="server" CssClass="ui-widget mybutton" onclick="btnApply_Clicked" />&nbsp;&nbsp;
			<asp:Button 
			ID="btnExport" Text="Export" runat="server"  CssClass="ui-widget mybutton" onclick="btnExport_Clicked" />&nbsp;&nbsp;
			<asp:TextBox ID="searchLimit" value="50"  style="width: 60px;" runat="server"></asp:TextBox>  
			&nbsp;
			<asp:Button ID="btnPrevious" runat="server"  CssClass="ui-widget mybutton" Text="<< Previous" OnClick="btnPrevious_Clicked" />
			Page: <asp:TextBox ID="pageNumber" value="1"  style="width: 60px;" runat="server"></asp:TextBox>
			<asp:Button ID="btnNext" runat="server" Text="Next >>" OnClick="btnNext_Clicked"  CssClass="ui-widget mybutton" />
            <asp:Label ID="lblAddTable" runat="server"></asp:Label>
    <div class="mybutton" onclick="newRecord()" style="cursor: pointer; ">Add New</div>
</p></center>    
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    <asp:Label ID="tblResults" runat="server"></asp:Label>
    <br /><asp:Label ID="lblNumberRows" runat="server"></asp:Label> Record(s) returned.<br />
<div id="editDialog" style="display: none;">
    <asp:Label ID="editDialogContents" runat="server"></asp:Label>
</div>
    <div id="deleteDialog" style="display: none" align="center">
        Confirm Delete<br /><br />
        <div class="mybutton" onclick="reallyDelete();">Yes</div>
        <div class="mybutton" onclick="cancelDelete();">No</div>
    </div>

<asp:Literal ID="editRecordScript" runat="server"></asp:Literal>
<asp:Label ID="deleteRecordScript" runat="server"></asp:Label>
<asp:Label ID="newRecordScript" runat="server"></asp:Label>
 <script>
    $(document).ready(function () {
        $('.mybutton').button();
        $('.datepicker').datepicker();
    });
</script>
</asp:Content>
