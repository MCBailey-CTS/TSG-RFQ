<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NCEditQuote.aspx.cs" Inherits="RFQ.NCEditQuote" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 50px"></div>
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
                <asp:HiddenField ID="hdnCompany" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="ui-widget">Quote ID: </td><td><asp:Label ID="lblquoteID" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Number: </td><td><asp:Label ID="lblQuoteNumber" runat="server"></asp:Label></td>
            <td class="ui-widget"></td><td><asp:Label ID="lblDateCreated" runat="server"></asp:Label></td>
        </tr>
            <td class="ui-widget">Quote Version: </td><td><asp:Label ID="lblVersion" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Status: </td><td><asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        <tr>
            <td class="ui-widget">Part Number: </td><td><asp:TextBox ID="txtPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblPartNumber" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Part Name: </td><td><asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblPartName" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">RFQ #: </td><td><asp:TextBox ID="txtRFQNumber" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblRfqNumber" runat="server" CssClass="ui-widget"></asp:Label></td>
        </tr>
        <%--<tr>
            <td class="ui-widget">Workbook Part Number: </td><td><asp:TextBox ID="txtWBPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Customer Quote Number: </td><td><asp:TextBox ID="txtCustQuoteNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Access #: </td><td><asp:TextBox ID="txtAccessNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>--%>
        <tr>
            <td class="ui-widget">Customer: </td><td><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><asp:Label ID="lblCustomer" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Plant: </td><td><asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget" ></asp:DropDownList><asp:Label ID="lblPlant" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Customer Contact: </td><td><asp:TextBox ID="txtCustomerContact" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">TSG Salesman: </td><td><asp:Label id="lblSalesman" runat="server"></asp:Label></td>
            <td class="ui-widget">Customer RFQ #: </td><td><asp:TextBox ID="txtCustomerRFQ" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblCustomerRFQ" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Estimator: </td><td><asp:DropDownList ID="ddlEstimator" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        </tr>
        <tr>
            <td class="ui-widget">Process: </td><td><asp:TextBox ID="txtProcess" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblProcess" runat="server" CssClass="ui-widget"></asp:Label></td>
            <%--<td class="ui-widget">Cavity: </td><td><asp:DropDownList ID="ddlCavity" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblCavity" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Use TSG Name: </td><td><asp:CheckBox runat="server" ID="cbUseTSG" CssClass="ui-widget" /></td>--%>
        </tr>
        <tr>
            <td class="ui-widget">Program Kickoff: </td><td><asp:TextBox ID="txtProgramKickoff" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Program PPAP: </td><td><asp:TextBox ID="txtProgramPPAP" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Program SOP: </td><td><asp:TextBox ID="txtProgramSOP" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <%--<td class="ui-widget">Program SOP: </td><td><asp:TextBox TextMode="Date" ID="TextBox1" runat="server" CssClass="ui-widget"></asp:TextBox></td>--%>
        </tr>
        <tr>
            <td class="ui-widget">Years of Production: </td><td><asp:TextBox ID="txtYOP" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Annual Volume: </td><td><asp:TextBox ID="txtVolume" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Quoted Lot Size: </td><td><asp:TextBox ID="txtLotSize" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Data Rec'd Date:  </td><td><asp:TextBox ID="txtDataRec" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Data File Name: </td><td><asp:TextBox ID="txtDataFileName" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <%--<td class="ui-widget">Die size L-R (in): </td><td><asp:TextBox ID="txtLRIn" runat="server" CssClass="ui-widget" onkeyup="dieLRChange()"></asp:TextBox></td>--%>
        </tr>
        <tr>
            <td class="ui-widget">Design review will be scheduled apporximately </td><td><asp:TextBox ID="txtDesignReview" runat="server" CssClass="ui-widget"></asp:TextBox> Weeks after acceptance of Purchase Order, Kickoff and Receipt of latest CAD and GD&T.</td>
            <td class="ui-widget">Tooling / Equipment / Cell / Etc. will be available for tryout at TSG / STS   approximately: </td><td><asp:TextBox ID="txtTryout" runat="server" CssClass="ui-widget"></asp:TextBox> weeks after receipt of signed off designs.</td>
            <td class="ui-widget">Teardown and Shipping to be done by:  </td><td><asp:TextBox ID="txtShippingCompany" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Setup at Customers Facility will take approximately: </td><td><asp:TextBox ID="txtSetup" runat="server" CssClass="ui-widget"></asp:TextBox> Weeks</td>
<%--            <td class="ui-widget"># of Stations: </td><td><asp:TextBox ID="txtStations" runat="server" CssClass="ui-widget"></asp:TextBox></td>--%>
            <td class="ui-widget">Final runoff and Buyoff at Customers Facility: </td><td><asp:TextBox ID="txtBuyoff" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Payment Terms Tooling, Equipment & Capital: 
 </td><td><asp:TextBox ID="txtPaymentTermsToolingEquipmentCapital" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Payment Terms Piece Cost: </td><td><asp:TextBox ID="txtPaymentTermsPieceCost" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Raw Material price is quoted at </td><td><asp:TextBox ID="txtRawMaterial" runat="server" CssClass="ui-widget"></asp:TextBox> per pound and will be reviewed and adjusted quarterly based on current market pricing.</td>
            <td class="ui-widget">WIP Dunnage to be supplied by:</td><td><asp:TextBox ID="txtWIP" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Finished Goods Dunnage to be supplied by:</td><td><asp:TextBox ID="txtFinnishedGoods" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr class="blank_row">
            <td class="ui-widget">Shipping of Finished Goods to be supplied by:</td><td><asp:TextBox ID="txtShippingOfFinishedGoods" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td bgcolor="#FFFFFF" colspan="4">&nbsp;</td>
        </tr>
        <tr>
            <td class="ui-widget" colspan="5">Description</td>
            <td class="ui-widget">Cost (Numbers only)</td>
        </tr>
    </table>

    <%--<asp:Label runat="server" ID="totalCost" ></asp:Label>--%>
    Total Cost (auto-calculated): <asp:TextBox runat="server" ID="txtTotalCost" ReadOnly="true"></asp:TextBox>

    <br />
    <div onclick="addNoteRow('', '');"  class="ui-widget mybutton"  style='float: right;' >Add Note Row</div><div id="addNoteRow"></div>
    <br />
    <br />
    <div onclick="showGeneralNoteDialog();"  class="ui-widget mybutton"  style='float: right;'>Select General Notes</div><div id="generalNotes"></div>
    <br />
    <br /><br />
    <div class="ui-widget mybutton" id="btnCheck" onclick="checkQuote();" >Save</div>
    <br />
        <br />
          <br />
         <br />
<%--    <asp:Button ID="btnFinalize" CssClass="ui-widget mybutton" runat="server" OnClick="btnFinalize_Click" Text="Finalize" tabindex="-1"  />--%>
     
<%--    <center><asp:Button ID="btnSave_Click" runat="server" Style="visibility: hidden" Text="Save" CssClass="ui-widget mybutton" OnClick="btnSaveClick"/></center><br />--%>
    <label class="ui-widget">New Part Picture Upload: </label>
    <asp:FileUpload ID="filePicture" runat="server" />
    <br />
    <asp:Label ID="lblWarning" runat="server" CssClass="ui-widget"></asp:Label>
    
    <br />
    <asp:CheckBox ID="chkCreatedDate" runat="server" Text="Use Created Date in PDF" Checked="true" />
    <br />
    <asp:Button ID="btnReloadPage_Click" runat="server" style="visibility: hidden;" CssClass="ui-widget mybutton" OnClick="btnReloadPage" />
    <asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="javascript:window.open('CreateQuote.aspx?quoteNumber=' + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=' + $('#MainContent_hdnQuoteType').val() + '&dateCreated=' + $('#MainContent_chkCreatedDate').is(':checked') + '&rand=' + Math.random());  return false;" />
<%--    <asp:Button ID="btnNewVersion_Click" runat="server"  Text="Create New Version" CssClass="ui-widget mybutton" OnClick="btncreateNewVersionClick"/>
    <asp:Button ID="btnDuplicateQuote_Click" runat="server" Text="Duplicate EC Quote" CssClass="ui-widget mybutton" OnClick="duplicateQuote" />
    <asp:Button ID="btnDeleteQuote_Click" runat="server" Text="Delete Quote" CssClass="ui-widget mybutton" OnClick="btnDeleteQuoteClick" />--%>
    <asp:Literal ID="litScript" runat="server"></asp:Literal>
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    <asp:Literal ID="litQuoteScripts" runat="server"></asp:Literal>
    <asp:HiddenField ID="hdnblankInfoID" Value="0" runat="server" />
    <asp:HiddenField ID="hdnoemID" Value="0" runat="server" />
    <asp:HiddenField ID="hdnpartTypeID" Value="0" runat="server" />
    <asp:HiddenField ID="hdnproductTypeID" Value="0" runat="server" />
    <asp:HiddenField ID="hdnpartID" Value="0" runat="server" />
    <asp:HiddenField ID="hdndieInfoID" Value="0" runat="server" />
    <asp:HiddenField ID="hdnNoteOrder" Value="0" runat="server" />
    <asp:HiddenField ID="hdnQuoteType" Value="0" runat="server" />
    <asp:HiddenField ID="hdnQuoteNumber" Value="0" runat="server" />
    </div>
    <div id="generalNotesDialog" style="display:none; padding: 20px; background-color: #D0D0D0;" >
        <asp:GridView ID="dgGeneralNotes" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:TemplateField HeaderText=" ID " >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="id" Text='<%# Eval("ids") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" Select ">
                    <ItemTemplate>
                        <asp:CheckBox ID="num" runat="server" Checked='<%# System.Convert.ToBoolean(Eval("num")) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText=" GeneralNote ">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="GeneralNote" Text='<%# Eval("GeneralNote") %>' ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        
        <asp:Button ID="btnSaveGeneralNotes" runat="server" Text="Save" CssClass="ui-widget mybutton" OnClick="saveGeneralNotes_Click" />
    </div>

    <script type="text/javascript">
        var noteCount = 0;
        var trID = 0;
        var lastID = 0;
        function addNoteRow(note, costNote) {
            if (note == null || costNote == null) {
                note = '';
                costNote = '';
            }

            $('#quoteTable').append('<tr id="' + trID+ '"><td class="ui-widget" colspan="5"><textarea name="notes' + noteCount + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + note + '</textarea></td><td class="ui-widget"><textarea name="price' + noteCount + '" rows="1" cols="20" id="txtCostNotes' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + costNote + '</textarea></asp:TextBox></td><td><div id="add' + noteCount + '" onclick="addRow(' + noteCount + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + noteCount + '" onclick="deleteRow(' + noteCount + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');

            document.getElementById('txtNotes' + noteCount).focus();
            trID++;
            noteCount++;
        }

        function sharepointSite() {
            var url = '';
            if ($('#MainContent_lblquoteID').html().indexOf('EC') != -1) {
                url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html().split(' EC')[0] + '&company=' + $('#MainContent_hdnCompany').val() + '&SA=true';
                //url = 'https://tsgdashboards.azurewebsites.net/ATPForm?quoteId=' + $('#MainContent_lblquoteID').html().split(' EC')[0] + '&company=' + $('#MainContent_hdncompany').val() + '&SA=true';
            }
            else {
                url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html().split(' EC')[0] + '&company=' + $('#MainContent_hdnCompany').val() + '&SA=false';
                //url = 'https://tsgdashboards.azurewebsites.net/ATPForm?quoteId=' + $('#MainContent_lblquoteID').html().split(' EC')[0] + '&company=' + $('#MainContent_hdncompany').val() + '&SA=false';
            }
            if (url != '') {
                window.open(url);
            }
        }

        function deleteRow(id, oldTR) {
            $('#quoteTable tr#' + oldTR).remove();

            //var num = 50;
            //if (trID < 50) {
            //    num = trID;
            //}

            for (i = id; i < 200; i++) {
                //if ($('#txtNotes' + i).length) {
                //    alert(i + ' Changing to ' + Number(i - 1));
                //}
                if ($('#txtNotes' + i).length) {
                    $('#txtNotes' + i).attr('name', 'notes' + Number(i - 1));
                    $('#txtNotes' + i).attr('id', 'txtNotes' + Number(i - 1));
                    $('#txtCostNotes' + i).attr('name', 'price' + Number(i - 1));
                    $('#txtCostNotes' + i).attr('id', 'txtCostNotes' + Number(i - 1));


                    var str = $('#add' + i).attr('onclick');
                    //alert(str);
                    str = str.split(',')[1];
                    str = str.split(')')[0];

                    $('#add' + i).attr('onclick', 'addRow(' + Number(i - 1) + ',' + str + ');');
                    $('#add' + i).attr('id', 'add' + Number(i - 1));

                    $('#remove' + i).attr('onclick', 'deleteRow(' + Number(i - 1) + ',' + str + ');');
                    $('#remove' + i).attr('id', 'remove' + Number(i - 1));

                }
                //else {
                //    break;
                //}



                //$("#id").attr("onclick", "new_function_name()");
            }

            noteCount--;
        }

        function addRow(id, oldTrID) {
            //incrementIDs(id + 1);
            var num = 200;
            if (trID < 200) {
                num = trID;
            }

            for (i = num; i > id; i--) {
                //if ($('#txtNotes' + i).length) {
                //    alert(i + ' Changing to ' + Number(i + 1));
                //}
                if ($('#txtNotes' + i).length) {
                    $('#txtNotes' + i).attr('name', 'notes' + Number(i + 1));
                    $('#txtNotes' + i).attr('id', 'txtNotes' + Number(i + 1));
                    $('#txtCostNotes' + i).attr('name', 'price' + Number(i + 1));
                    $('#txtCostNotes' + i).attr('id', 'txtCostNotes' + Number(i + 1));

                    //$("#id").attr("onclick", "new_function_name()");


                    var str = $('#add' + i).attr('onclick');
                    //alert(str);
                    str = str.split(',')[1];
                    str = str.split(')')[0];

                    $('#add' + i).attr('onclick', 'addRow(' + Number(i + 1) + ',' + str + ');');
                    $('#add' + i).attr('id', 'add' + Number(i + 1));

                    $('#remove' + i).attr('onclick', 'deleteRow(' + Number(i + 1) + ',' + str + ');');
                    $('#remove' + i).attr('id', 'remove' + Number(i + 1));
                    //alert('changed ' + i + ' ' + 'addRow(' + Number(i + 1) + ',' + str + ');');

                }
            }
            //alert($('#5').length);

            $('#quoteTable tr#' + oldTrID).after('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + Number(id + 1) + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + Number(id + 1) + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + '' + '</textarea></td><td class="ui-widget"><textarea name="price' + Number(id + 1) + '" rows="1" cols="20" id="txtCostNotes' + Number(id + 1) + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td><div id="add' + Number(id + 1) + '" onclick="addRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + Number(id + 1) + '" onclick="deleteRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
            trID++;
            noteCount++;
        }

        function downloadQuote() {
            url = "CreateQuote.aspx?quoteNumber=" + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=' + $('#MainContent_hdnQuoteType').val() + '&rand=' + Math.random();
            window.open(url);
            return false;
        }

        function checkQuote() {
            if($('#MainContent_ddlCustomer').val() == 'Please Select') {
                alert('Please Select a Customer');
                return;
            }
            $('#MainContent_btnSave_Click').click();
        }

        function showGeneralNoteDialog() {
            $('#generalNotesDialog').dialog({ width: 800, height: 600 });
            $('#generalNotesDialog').parent().appendTo("form");
        }

        function blankWidthChange() {
            var a = $('#MainContent_txtBlankWidthIn').val();
            $('#MainContent_txtBlankWidthMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtBlankWidthIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function blankPitchChange() {
            var a = $('#MainContent_txtBlankPitchIn').val();
            $('#MainContent_txtBlankPitchMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtBlankPitchIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function matThicknessChange() {
            var a = $('#MainContent_txtMaterialThkIn').val();
            $('#MainContent_txtMaterialThkMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtMaterialThkIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function dieFBChange() {
            var a = $('#MainContent_txtFBIn').val();
            $('#MainContent_txtFBMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtFBIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function dieLRChange() {
            var a = $('#MainContent_txtLRIn').val();
            $('#MainContent_txtLRMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtLRIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function shutHeightChange() {
            var a = $('#MainContent_txtShutIn').val();
            $('#MainContent_txtShutMm').val((a * 25.4).toFixed(1));
            if (!$.isNumeric($('#MainContent_txtShutIn').val())) {
                alert('This needs to be a number.');
            }
        }

        function getID() {
            lastID = $(document.activeElement).attr('id');
        }

        function updateCost() {
            var total = 0;
            for (i = 0; i < 100; i++) {
                if ($('#txtCostNotes' + i.toString()).val() === undefined) {
                    
                }
                else {
                    total += Number($('#txtCostNotes' + i).val());
                    //alert(total.toString());
                }
            }
            
            //document.getElementById("MainContent_totalCost").value = "Total: " + total.toString();
            document.getElementById('MainContent_txtTotalCost').value = ("Total: $" + total.toString());
            //onkeyup = "sendCode()"
        }

    </script>
</asp:Content>
