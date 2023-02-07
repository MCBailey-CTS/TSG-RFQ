<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HTSEditQuote.aspx.cs" Inherits="RFQ.HTSEditQuote" MasterPageFile="~/Site.Master" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 100px"></div>
    <div align="center">
    <table id="quoteTable" style="width: 1000px;">
        <tr>
            <td>

            </td>
            <td colspan="4">
                <center>
                    <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                </center>
            </td>
            <td>
                <input type="button" class="ui-widget mybutton" value="Create Job" onclick="sharepointSite();" />
            </td>
        </tr>
        <tr>
            <td class="ui-widget">Quote ID: </td><td><asp:Label ID="lblquoteID" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Number: </td><td><asp:Label ID="lblQuoteNumber" runat="server"></asp:Label></td>
        </tr>
            <td class="ui-widget">Quote Version: </td><td><asp:Label ID="lblVersion" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Status: </td><td><asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            <td class="ui-widget">Job Num: </td><td><asp:TextBox ID="txtJobNum" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        <tr>
            <td class="ui-widget">Part Number: </td><td><asp:TextBox ID="txtPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblPartNumber" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Part Name: </td><td><asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblPartName" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">RFQ #: </td><td><asp:TextBox ID="txtRFQNumber" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblRfqNumber" runat="server" CssClaa="ui-widget"></asp:Label></td>
        </tr>
        <tr>
            <%--<td class="ui-widget">Workbook Part Number: </td><td><asp:TextBox ID="txtWBPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>--%>
            <td class="ui-widget">Customer RFQ Number: </td><td><asp:TextBox ID="txtCustomerRFQ" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Use TSG Logo: </td><td><asp:CheckBox ID="cbUseTSGLogo" runat="server"/></td>
        </tr>
        <tr>
            <td class="ui-widget">Customer: </td><td><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><asp:Label ID="lblCustomer" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Plant: </td><td><asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget" ></asp:DropDownList><asp:Label ID="lblPlant" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Use TSG Name: </td><td><asp:CheckBox ID="cbUseTSGName" runat="server"/></td>
        </tr>
        <tr>
            <td class="ui-widget">TSG Salesman: </td><td><asp:Label id="lblSalesman" runat="server"></asp:Label></td>
            <%--<td class="ui-widget">Customer RFQ #: </td><td><asp:TextBox ID="txtCustomerRFQ" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblCustomerRFQ" runat="server" CssClaa="ui-widget"></asp:Label></td>--%>
            <td class="ui-widget">Estimator: </td><td><asp:DropDownList ID="ddlEstimator" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        </tr>
        <tr>
            <td class="ui-widget">Quote Type: </td><td><asp:DropDownList ID="ddlQuoteType" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            <td class="ui-widget">Part Type: </td><td><asp:DropDownList ID="ddlPartType" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            <td class="ui-widget">Access #: </td><td><asp:TextBox ID="txtAccess" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Process: </td><td><asp:DropDownList ID="ddlProcess" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblProcess" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Cavity: </td><td><asp:DropDownList ID="ddlCavity" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblCavity" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Lead Time: </td><td><asp:TextBox names="leadTime" ID="txtLeadTime" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Shipping: </td><td><asp:DropDownList ID="ddlShipping" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblShipping" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Payment Terms:</td><td><asp:DropDownList ID="ddlPayment" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblPayment" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Customer Contact: </td><td><asp:TextBox ID="txtCustomerContact" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Material Type: </td><td><asp:TextBox ID="txtMaterialType" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Currency Type: </td><td><asp:DropDownList ID="ddlCurrency" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        </tr>
        <tr class="blank_row">
            <td bgcolor="#FFFFFF" colspan="3">&nbsp;</td>
        </tr>
        <tr>
            <td class="ui-widget" colspan="4">Description</td>
            <td class="ui-widget">QTY</td>
            <td class="ui-widget">Unit Price</td>
        </tr>
    </table>
    <asp:TextBox runat="server" ID="txtTotalCost" ReadOnly="true"></asp:TextBox>

    <br />
    <div onclick="addNoteRow('', '');"  class="ui-widget mybutton"  style='float: right;' >Add Note Row</div><div id="addNoteRow"></div>
        <div onclick="add5NoteRow('', '');"  class="ui-widget mybutton"  style='float: right;' >Add 5 Note Rows</div><div id="add5NoteRow"></div>
    </ div>
    
    
    <center>
    <table>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote1" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote1" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote2" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote2" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote3" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote3" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote4" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote4" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote5" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote5" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
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
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote10" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote10" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="cbGeneralNote11" runat="server" />
            </td>
            <td>
                <asp:Label ID="lblGeneralNote11" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    </center>

    <asp:Button ID="btnSave_Click" runat="server"  Text="Save" CssClass="ui-widget mybutton" OnClick="btnSaveClick"/><br />
    <%--<asp:Button ID="btnFinalize" CssClass="ui-widget mybutton" runat="server" OnClick="btnFinalize_Click" Text="Finalize" />--%>
    <br />
    <br />
         <br />
    
    <label class="ui-widget">New Part Picture Upload: </label>
    <asp:FileUpload ID="filePicture" runat="server" />
    <%--<asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="downloadQuote();" />--%>
    <asp:Button ID="btnCopyQuote_Click" runat="server" Text="Copy Quote" CssClass="ui-widget mybutton" OnClick="btnCopyQuote" />
    <asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="javascript:window.open('CreateQuote.aspx?quoteNumber=' + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=' + 3 + '&rand=' + Math.random());  return false;" />
    <asp:Button ID="btnNewVersion_Click" runat="server"  Text="Create New Version" CssClass="ui-widget mybutton" OnClick="btncreateNewVersionClick"/>

    <asp:Literal ID="litScript" runat="server"></asp:Literal>

    <asp:HiddenField ID="hdnQuoteNumber" Value="0" runat="server" />

    <script type="text/javascript">
        var noteCount = 0;
        var trID = 0;

        function sharepointSite() {
            var url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html() + '&company=9';
            window.open(url);
        }

        function addNoteRow(note, qty, cost) {
            if (note == null || cost == null || qty == null) {
                note = '';
                cost = '';
                qty = '';
            }
            $('#quoteTable').append('<tr id=' + trID + '><td class="ui-widget" colspan="4"><textarea name="notes' + noteCount + '" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + note + '</textarea></asp:TextBox></td><td class="ui-widget"><textarea name="qty' + noteCount + '" rows="1" cols="10" id="txtQTY' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + qty + '</textarea></asp:TextBox></td><td class="ui-widget"><textarea name="unit' + noteCount + '" rows="1" cols="20" id="txtUnitPrice' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + cost + '</textarea></asp:TextBox></td><td><div id="add' + noteCount + '" onclick="addRow(' + noteCount + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + noteCount + '" onclick="deleteRow(' + noteCount + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
            document.getElementById('txtNotes' + noteCount).focus();
            //noteCount += 100;
            noteCount++;
            trID++;
            $('#MainContent_txtPartNumber').attr('id', '#MainContent_txtPN');
        }

        function downloadQuote() {
            url = "CreateQuote.aspx?quoteNumber=" + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=' + 3 + '&rand=' + Math.random();
            window.open(url);
        }

        function add5NoteRow() {
            for (i = 0; i < 5; i++) {
                addNoteRow('', '', '');
            }
        }

        function deleteRow(id, oldTR) {
            $('#quoteTable tr#' + oldTR).remove();

            for (i = id; i < 150; i++) {
                if ($('#txtNotes' + i).length) {
                    $('#txtNotes' + i).attr('name', 'notes' + Number(i - 1));
                    $('#txtNotes' + i).attr('id', 'txtNotes' + Number(i - 1));
                    $('#txtQTY' + i).attr('name', 'qty' + Number(i - 1));
                    $('#txtQTY' + i).attr('id', 'txtQTY' + Number(i - 1));
                    $('#txtUnitPrice' + i).attr('name', 'unit' + Number(i - 1));
                    $('#txtUnitPrice' + i).attr('id', 'txtUnitPrice' + Number(i - 1));

                    var str = $('#add' + i).attr('onclick');
                    str = str.split(',')[1];
                    str = str.split(')')[0];

                    $('#add' + i).attr('onclick', 'addRow(' + Number(i - 1) + ',' + str + ');');
                    $('#add' + i).attr('id', 'add' + Number(i - 1));
                    $('#remove' + i).attr('onclick', 'deleteRow(' + Number(i - 1) + ',' + str + ');');
                    $('#remove' + i).attr('id', 'remove' + Number(i - 1));
                }
            }

            noteCount--;
        }

        function addRow(id, oldTrID) {
            //incrementIDs(id + 1);
            var num = 150;
            if (trID < 150) {
                num = trID;
            }

            for (i = num; i > id; i--) {
                if ($('#txtNotes' + i).length) {
                    $('#txtNotes' + i).attr('name', 'notes' + Number(i + 1));
                    $('#txtNotes' + i).attr('id', 'txtNotes' + Number(i + 1));
                    $('#txtQTY' + i).attr('name', 'qty' + Number(i + 1));
                    $('#txtQTY' + i).attr('id', 'txtQTY' + Number(i + 1));
                    $('#txtUnitPrice' + i).attr('name', 'unit' + Number(i + 1));
                    $('#txtUnitPrice' + i).attr('id', 'txtUnitPrice' + Number(i + 1));
                    //$("#id").attr("onclick", "new_function_name()");

                    var str = $('#add' + i).attr('onclick');
                    str = str.split(',')[1];
                    str = str.split(')')[0];

                    $('#add' + i).attr('onclick', 'addRow(' + Number(i + 1) + ',' + str + ');');
                    $('#add' + i).attr('id', 'add' + Number(i + 1));
                    $('#remove' + i).attr('onclick', 'deleteRow(' + Number(i + 1) + ',' + str + ');');
                    $('#remove' + i).attr('id', 'remove' + Number(i + 1));
                }
            }

            $('#quoteTable tr#' + oldTrID).after('<tr id=' + trID + '><td class="ui-widget" colspan="4"><textarea name="notes' + Number(id + 1) + '" rows="1" cols="120" id="txtNotes' + Number(id + 1) + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td class="ui-widget"><textarea name="qty' + Number(id + 1) + '" rows="1" cols="10" id="txtQTY' + Number(id + 1) + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td class="ui-widget"><textarea name="unit' + Number(id + 1) + '" rows="1" cols="20" id="txtUnitPrice' + Number(id + 1) + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td><div id="add' + Number(id + 1) + '" onclick="addRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + Number(id + 1) + '" onclick="deleteRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
            trID++;
            noteCount++;
        }

        //function incrementIDs(id) {
        //    var count = id;
        //    for (i = 5; i >= id; i--) {
        //        //if ($('#Notes' + i).length) {
        //            //alert(i + ' Changing to ' + Number(i + 1));
        //        //}
        //        $('#txtNotes' + i).attr('id', 'txtNotes' + Number(i + 1));
        //        $('#txtQTY' + i).attr('id', 'txtQTY' + Number(i + 1));
        //        $('#txtUnitPrice' + i).attr('id', 'txtUnitPrice' + Number(i + 1));

        //        $("#id").attr("onclick", "new_function_name()");
        //    }
        //    //alert($('#5').length);
        //    $('.aiButton').attr('id', 'saveold');
        //}

        //function downloadQuote() {
        //    url = "CreateQuote.aspx?quoteNumber=" + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=' + $('#MainContent_hdnQuoteType').val() + '&rand=' + Math.random();
        //    window.open(url);
        //}

        function showGeneralNoteDialog() {
            $('#generalNotesDialog').dialog({ width: 800, height: 600 });
            $('#generalNotesDialog').parent().appendTo("form");
        }

        function updateCost() {
            var total = 0;
            for (i = 0; i < 100; i++) {
                if ($('#txtQTY' + i.toString()).val() === undefined || $('#txtUnitPrice' + i.toString()).val() === undefined) {

                }
                else {
                    total += (Number($('#txtQTY' + i.toString()).val() * Number($('#txtUnitPrice' + i.toString()).val())));
                    //alert(total.toString());
                }
            }

            //document.getElementById("MainContent_totalCost").value = "Total: " + total.toString();
            document.getElementById('MainContent_txtTotalCost').value = ("Total: $" + total.toString());
            //onkeyup = "sendCode()"
        }

    </script>
</asp:Content>
