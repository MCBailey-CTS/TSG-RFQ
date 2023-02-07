<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UGSMultiQuote.aspx.cs" Inherits="RFQ.UGSMultiQuote" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 100px"></div>
    <asp:HiddenField ID="hdnNumberOfParts" runat="server" />
    <center><h3>Common Quote Information</h3></center>
    <center>
        <table>
            <tr>
                <td>Customer: </td><td><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td>Plant: </td><td><asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
                <td>Use TSG Logo / Name: </td><td><asp:CheckBox ID="chkUseTSG" runat="server" CssClass="ui-widget" /></td>
            </tr>
            <tr>
                <td>TSG Salesman: </td><td><asp:Label runat="server" ID="lblSalesman"></asp:Label></td>
                <td>Customer RFQ #: </td><td><asp:TextBox ID="txtCustRFQNum" runat="server"></asp:TextBox></td>
                <td>Estimator</td><td><asp:DropDownList ID="ddlEstimator" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>Shipping: </td><td><asp:DropDownList ID="ddlShipping" runat="server"></asp:DropDownList></td>
                <td>Payment: </td><td><asp:DropDownList ID="ddlPayment" runat="server"></asp:DropDownList></td>
                <td>Customer Contact: </td><td><asp:TextBox ID="txtCustContact" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Shipping Location: </td><td><asp:TextBox ID="txtShippingLocation" runat="server" ></asp:TextBox></td>
                <td>Quote Type: </td><td><asp:DropDownList ID="ddlQuoteType" runat="server"></asp:DropDownList></td>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td colspan="6">
                    <center>
                        <textarea id="txtNotes" cols="400" rows="20" runat="server" style="max-width: 1000px; width: 1000px"></textarea><br /><br />
                    </center>
                </td>
            </tr>
        </table>
    </center>
    <br />
        <center>
        <table>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote1" runat="server" Checked="true" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote1" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote2" runat="server" Checked="true" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote2" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote3" runat="server" Checked="true" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote3" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote4" runat="server" Checked="true" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote4" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote5" runat="server" Checked="true" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote5" runat="server"></asp:Label>
                </td>
            </tr>
    <%--        <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote6" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote6" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote7" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote7" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote8" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote8" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="cbGeneralNote9" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblGeneralNote9" runat="server"></asp:Label>
                </td>
            </tr>--%>
        </table>
    </center>
    <br />
    <br />
    <asp:Table ID="tblResults" runat="server"></asp:Table>
    <center>
        <div class="ui-widget mybutton" id="btnCheck" onclick="checkQuotes();" >Save</div>
        <br />
        <asp:Button ID="btnSave" runat="server" CssClass="ui-widget mybutton" style="visibility: hidden;" Text="Save" OnClick="save" />
    </center>

    <asp:Literal ID="litScript" runat="server"></asp:Literal>

    <script>
        //Checkes to make sure that each has a total price that is a number

        function checkQuotes() {
            //for (i = 0 ; i < $('#MainContent_hdnNumberOfParts').val() ; i++) {
            //    if (isNaN($('#txtTotalPrice' + i).val()) || $('#txtTotalPrice' + i).val() == '') {
            //        alert('Cannot save ' + $('#txtPartNumber' + i).val() + ' does not have a price');
            //        return;
            //    }
            //}
            $('#MainContent_btnSave').click();
        }

        function updateLeadTime(num) {
            if ($('#leadTime' + num).val() != 'Please Select') {
                $('#txtLeadTime' + num).val($('#leadTime' + num).val());
            }
            else {
                $('#txtLeadTime' + num).val('');
            }
        }
    </script>

</asp:Content>