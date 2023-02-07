<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Update.aspx.cs" Inherits="RFQ.Update" MasterPageFile="~/Site.Master"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 100px"></div>
    <center>

<%--        <asp:TextBox ID="txtProgram" runat="server" CssClass="ui-widget"></asp:TextBox>
        <asp:Button ID="btnProgram" runat="server" OnClick="addProgram" Text="Add Program" CssClass="ui-widget mybutton" />
        <asp:TextBox ID="txtOEM" runat="server" CssClass="ui-widget"></asp:TextBox>
        <asp:Button ID="btnOEM" runat="server" OnClick="addOEM" Text="Add OEM" CssClass="ui-widget mybutton" />

        <br />
        <br />--%>

        

        <asp:GridView ID="dgResults" runat="server" AutoGenerateColumns="false" ondatabound="OnRowDataBound">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="rfqid" HeaderText="RFQ ID" DataNavigateUrlFormatString="https://tsgrfq.azurewebsites.net/EditRFQ.aspx?id={0}" DataTextField="rfqid" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign ="Center" HeaderStyle-VerticalAlign="Top"></asp:HyperLinkField>
                <%--<asp:BoundField DataField="rfqID" HeaderText="RFQ ID" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"></asp:BoundField>--%>
                <asp:BoundField DataField="customer" HeaderText="Customer" ItemStyle-Width="20%" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top"></asp:BoundField>
                <asp:BoundField DataField="plant" HeaderText="Plant" ItemStyle-Width="20%" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top" />
                <asp:BoundField DataField="custRFQNum" HeaderText="Customer RFQ #" ItemStyle-Width="20%" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top" />
                <asp:TemplateField HeaderText="Program" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlProgram" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="OEM" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlOEM" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Vehicle" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:DropDownList ID="ddlVehicle" runat="server" CssClass="ui-widget"></asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="update" HeaderText="UPDATE" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" HtmlEncode="false" HeaderStyle-VerticalAlign="Top" />
                <%--<asp:BoundField DataField="button" HeaderText="Select" ItemStyle-CssClass="ui-widget" HeaderStyle-CssClass="ui-widget-content" ItemStyle-HorizontalAlign="Center" HtmlEncode="true" />--%>
            </Columns>
        </asp:GridView>

    </center>

    <script>
        function updateProgram(num, rfq) {
            url = 'updateRFQInfo.aspx?program=' + $('#MainContent_dgResults_ddlProgram_' + num).val() + '&oem=' + $('#MainContent_dgResults_ddlOEM_' + num).val() + '&vehicle=' +
                $('#MainContent_dgResults_ddlVehicle_' + num).val() + '&rfq=' + rfq
            $.ajax({ url: url, success: function (data) { } })
        }
    </script>

</asp:Content>