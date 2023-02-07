<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeCompany.aspx.cs" Inherits="RFQ.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Company</title>
</head>
<body>
    <form runat="server">
    <center>
    <table id="t01">
            <tr>
                <td>
                    <asp:Label ID="lblname" runat="server" CssClass="ui-widget"></asp:Label>
  	            </td>	
                </tr>
        <tr></tr>
        <tr>
	            <td>
	                Set your company to: 
  	            
                    <asp:DropDownList ID="ddlTSGCompany" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </td>
            </tr>
        <tr></tr>
        <tr>
                <td><asp:Button ID="btnChange" runat="server" CssClass="ui-widget mybutton" Text="Change Company" OnClick="btnChangeCompany_Click" /></td>
                </tr>
        </table>
    </center>
        </form>
</body>
</html>
