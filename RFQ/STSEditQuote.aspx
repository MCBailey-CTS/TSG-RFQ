<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STSEditQuote.aspx.cs" Inherits="RFQ.STSEditQuote" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 100px"></div>
    <div align="center">
    <table id="quoteTable" style="width: 1000px;">
        <tr>
            <td>
                &nbsp
            </td>
            <td colspan="4">
                <asp:Label ID="lblStatus" runat="server" Font-Size="X-Large" ></asp:Label>
            </td>
            <td>
                <input type="button" id="btnCreateSharePoint" class="ui-widget mybutton" value="Create Job" onclick="sharepointSite();" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
                &nbsp
            </td>
            <td>
                <button id="btnApp" class="mybutton" onclick="openApprovalDialog(); return false;">Submit Approval</button>
            </td>
        </tr>
        <tr>
            <td class="ui-widget">Quote ID: </td><td><asp:Label ID="lblquoteID" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Number: </td><td><asp:Label ID="lblQuoteNumber" runat="server"></asp:Label></td>
            <td class="ui-widget">EC Quote: </td><td><asp:CheckBox ID="cbECQuote" runat="server" CssClass="ui-widget" /></td>
        </tr>
            <td class="ui-widget">Quote Version: </td><td><asp:Label ID="lblVersion" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Status: </td><td><asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            <td class="ui-widget">Company: </td><td><asp:DropDownList ID="ddlCompany" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        <tr>
            <td class="ui-widget">Part Number: </td><td><asp:TextBox ID="txtPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Part Name: </td><td><asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">RFQ #: </td><td><asp:TextBox ID="txtRFQNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Customer: </td><td><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="ui-widget" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList><asp:Label ID="lblCustomer" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Plant: </td><td><asp:DropDownList ID="ddlPlant" runat="server" CssClass="ui-widget" ></asp:DropDownList><asp:Label ID="lblPlant" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Customer Contact: </td><td><asp:TextBox ID="txtCustomerContact" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">TSG Salesman: </td><td><asp:Label id="lblSalesman" runat="server"></asp:Label></td>
            <td class="ui-widget">Customer RFQ #: </td><td><asp:TextBox ID="txtCustomerRFQ" runat="server" CssClass="ui-widget"></asp:TextBox><asp:Label ID="lblCustomerRFQ" runat="server" CssClaa="ui-widget"></asp:Label></td>
            <td class="ui-widget">Estimator: </td><td><asp:DropDownList ID="ddlEstimator" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        </tr>
        <tr>
            <td class="ui-widget">EAV: </td><td><asp:TextBox ID="txtEAV" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Process: </td><td><asp:TextBox ID="txtProcess" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Calculated Machine Process Time: </td><td><asp:TextBox ID="txtMachineTime" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Shipping: </td><td><asp:DropDownList ID="ddlShipping" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblShipping" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Payment Terms:</td><td><asp:DropDownList ID="ddlPayment" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblPayment" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Lead Time: </td><td><asp:TextBox names="leadTime" ID="txtLeadTime" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Job Number: </td><td><asp:TextBox ID="txtJobNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Use TSG logo / name: </td><td><asp:CheckBox runat="server" ID="cbUseTSG" CssClass="ui-widget" /></td>
            <td class="ui-widget">Firm Quote: </td><td><asp:CheckBox ID="cbFirmQuote" runat="server" CssClass="ui-widget" /></td>
        </tr>
        <tr>
            <td colspan="6">
                <center>
                    <table style="border: 1px solid black;">
                        <tr>
                            <td>
                                Annual Volume
                            </td>
                            <td>
                                <asp:TextBox ID="txtAnnualVolume" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>
                                Tact Time (Available C'time)
                            </td>
                            <td>
                                <asp:TextBox ID="txtTactTime" runat="server" CssClass="ui-widget"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Days Per Year
                            </td>
                            <td>
                                <asp:TextBox ID="txtDaysPerYear" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>

                            </td>
                            <td>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                Hours per shift
                            </td>
                            <td>
                                <asp:TextBox ID="txtHoursPerShift" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>
                                Net Parts per Hours
                            </td>
                            <td>
                                <asp:TextBox ID="txtNetPartsPerHour" runat="server" CssClass="ui-widget"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Shifts per Day
                            </td>
                            <td>
                                <asp:TextBox ID="txtShiftsPerDay" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>
                                Gross Parts per Hour
                            </td>
                            <td>
                                <asp:TextBox ID="txtGrossPartsPerHour" runat="server" CssClass="ui-widget"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Efficiency
                            </td>
                            <td>
                                <asp:TextBox ID="txtEfficiency" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>

                            </td>
                            <td>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                Seconds per Hour
                            </td>
                            <td>
                                <asp:TextBox ID="txtSecondsPerHour" runat="server" onkeyup="updateLabel()" CssClass="ui-widget"></asp:TextBox>
                            </td>
                            <td>
                                Net Parts per Day
                            </td>
                            <td>
                                <asp:TextBox ID="txtNetPartsPerDay" runat="server" CssClass="ui-widget"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </center>
            </td>
        </tr>
        <tr class="blank_row">
            <td bgcolor="#FFFFFF" colspan="3">&nbsp;</td>
        </tr>
        <tr>
            <td class="ui-widget" colspan="3">Description</td>
            <td class="ui-widget"><asp:Label ID="lblToolingCosts" runat="server" Text="Tooling Costs"></asp:Label></td>
            <td class="ui-widget"><asp:Label ID="lblCapitalCosts" runat="server" Text="Capital Cost"></asp:Label></td>
            <td class="ui-widget"><asp:Label ID="lblSubtotal" runat="server" Text="Subtotal"></asp:Label></td>
        </tr>
    </table>
    <asp:TextBox runat="server" ID="txtTotalCost" ReadOnly="true"></asp:TextBox>


    <br />
    <div onclick="addNoteRow('', '');"  class="ui-widget mybutton"  style='float: right;' >Add Note Row</div><div id="addNoteRow"></div>
    <br />


    <label class="ui-widget">New Part Picture Upload: </label>
    <asp:FileUpload ID="filePicture" runat="server" />
    <br />
    <label class="ui-widget">Cell Picture Upload: </label>
    <asp:FileUpload ID="cellPictureUpload" runat="server" />
    <br />
    <label class="ui-widget">STS Detailed Quote Upload: </label>
    <asp:TextBox ID="txtDetailedQuote" runat="server"></asp:TextBox>
    <asp:FileUpload ID="STSDetailedQuoteUpload" runat="server" />
    
    <button id="btnSave" runat="server" class="ui-widget mybutton" onclick="save();return false;">Save</button>
    <br />
    <br />
    <br />
    <asp:Button ID="btnDelete_Click" runat="server" Text="Delete Quote" CssClass="ui-widget mybutton" OnClick="btnDeleteClick" /><br />
    <asp:Button ID="btnFinalize" CssClass="ui-widget mybutton" runat="server" OnClick="btnFinalize_Click" Text="Finalize" />
    <br />
        <br />
         <br />
    <button id="btnCreateVersion" class="ui-widget mybutton" onclick="openRevision(); return false;">Create New Version</button>
    <asp:Button ID="btnNewVersion_Click" runat="server" Text="Create New Version" CssClass="ui-widget mybutton" OnClick="btncreateNewVersionClick" style="visibility: hidden;" />
    <br />
    <asp:Button ID="btnSave_Click" runat="server"  Text="Save" CssClass="ui-widget mybutton" OnClick="btnSaveClick" style="visibility: hidden;"/>     
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
        </table>
    </center>

    <div id="RevisionDialog" style="display:none; padding: 20px; background-color: #D0D0D0; width: 800px; height: 400px;">
        <div class="row">
            <div class="col-lg-12">
                <center>
                    Please Enter the Revision Description
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <textarea id="txtRevisionDescription" name="RevisionDescription" style="width: 700px; height: 300px;" ></textarea>
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <button id="btnCreateRevision" class="mybutton" onclick="createRevision(); return false;">Create Revision</button
                </center>
            </div>
        </div>
    </div>

    <div id="approvalDialog" style="display: none; padding: 20px; background-color: #D0D0D0; width: 800px; height: 400px;">
        <div class="row">
            <div class="col-lg-12">
                <center>
                    Please upload required quote files and any comments.
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <center>
                    Project Manager
                </center>
            </div>
            <%--<div class="col-lg-4">
                <center>
                    Customer Emails (separate by ;)
                </center>
            </div>--%>
            <div class="col-lg-6">
                <center>
                    Quote attachments
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-6">
                <center>
                    <asp:DropDownList ID="ddlProjectManager" runat="server" CssClass="ui-widget"></asp:DropDownList>
                </center>
            </div>
            <%--<div class="col-lg-4">
                <center>
                    <asp:TextBox ID="txtApproverEmail" runat="server" CssClass="ui-widget" TextMode="MultiLine" style="width: 150px; height: 150px;"></asp:TextBox>
                </center>
            </div>--%>
            <div class="col-lg-6">
                <center>
                    <asp:FileUpload ID="fuQuote" runat="server" AllowMultiple="true" />
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <asp:Button ID="btnApproval" runat="server" CssClass="ui-widget mybutton" Text="Send for Approval" OnClick="btnApproval_Click" style="visibility: hidden;" />
                </center>
            </div>
        </div>
        <div class="row">
            <div class="col-lg-12">
                <center>
                    <Button id="btnSubmitApproval" onclick="checkApproval(); return false;" class="mybutton">Submit for Approval</Button>
                </center>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnNoteOrder" Value="0" runat="server" />
    <asp:HiddenField ID="hdnQuoteNumber" Value="0" runat="server" />
    <asp:HiddenField ID="hdnQuoteType" Value="new" runat="server" />
    <%--<asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="downloadQuote();" />--%>
        <br />
    <asp:CheckBox ID="chkCreatedDate" runat="server" Text="Use Created Date in PDF" Checked="true"/>
    <asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="javascript:window.open('CreateQuote.aspx?quoteNumber=' + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=4&dateCreated=' + $('#MainContent_chkCreatedDate').is(':checked') + '&rand=' + Math.random());  return false;" />
    
    <asp:Literal ID="litScript" runat="server"></asp:Literal>
    <script type="text/javascript">
        var noteCount = 0;
        var trID = 0;

        function save() {
            if ($('#MainContent_txtCustomerContact').val() == '') {
                alert('Please fill out a customer contact before you can save the quote.');
                return;
            }
            $('#MainContent_btnSave_Click').click();
        }


        function openApprovalDialog() {
            $('#approvalDialog').dialog({ width: 900, height: 500 });
            $('#approvalDialog').parent().appendTo("form");
        }

        function checkApproval() {
            if ($('#MainContent_fuQuote').val() == '') {
                alert('Please upload quote document before submitting for approval.');
                return;
            }
            if ($('#MainContent_ddlProjectManager').val() == "Please Select") {
                alert('Please select a project manager before submitting for approval');
                return;
            }
            //else if ($('#MainContent_txtApproverEmail').val() == '') {
            //    alert('Please enter the customer\'s email before submitting approval.');
            //    return;
            //}
            $('#MainContent_btnApproval').click();
        }

        function openRevision() {
            $('#RevisionDialog').dialog({ width: 500, height: 500 });
            $('#RevisionDialog').parent().appendTo("form");
        }

        function createRevision() {
            if ($('#txtRevisionDescription').val().trim() == '') {
                alert('Please fill in a description before we can create a new revision.');
            }
            else {
                $('#MainContent_btnNewVersion_Click').click();
            }
        }

        function sharepointSite() {
            var url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html() + '&company=13';
            window.open(url);
        }

        function addNoteRow(note, costNote) {
            if ($('#MainContent_hdnQuoteType').val() == 'old') {
                if (note == null || costNote == null) {
                    note = '';
                    costNote = '';
                }
                $('#quoteTable').append('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + noteCount + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + note + '</textarea></td><td class="ui-widget"><textarea name="price' + noteCount + '" rows="1" cols="20" id="txtCostNotes' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + costNote + '</textarea></asp:TextBox></td><td><div id="add' + noteCount + '" onclick="addRow(' + noteCount + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + noteCount + '" onclick="deleteRow(' + noteCount + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
                //$('#quoteTable').append('<tr><td class="ui-widget" colspan="5"><textarea name="notes' + noteCount + '" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + note + '</textarea></td><td class="ui-widget"><textarea name="price' + noteCount + '" rows="1" cols="20" id="txtCostNotes' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + costNote + '</textarea></asp:TextBox></td></tr>');
                document.getElementById('txtNotes' + noteCount).focus();
                noteCount++;
                trID++;
            }
            else {
                addNewNoteRow('', '', '');
            }
        }

        function addNewNoteRow(note, tooling, capital) {
            $('#quoteTable').append('<tr id="' + trID + '">' + getNoteTextArea(note) + getToolingCost(tooling) + getCapitalCost(capital) + getSubTotal(tooling, capital) + '</tr>');
            noteCount++;
            trID++;
        }

        function getNoteTextArea(note) {
            return '<td colspan="3"><textarea name="notes' + noteCount + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="height: 30px; max-width: 100%;" maxlength="800">' + note + '</textarea></td>';
        }

        function getToolingCost(cost) {
            return '<td ><textarea name="tooling' + noteCount + '" rows="1" cols="20" id="txtToolingCost' + noteCount + '" class="ui-widget" onkeyup="updateSubtotal()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + cost + '</textarea></td>';
        }

        function getCapitalCost(cost) {
            return '<td><textarea name="capital' + noteCount + '" rows="1" cols="20" id="txtCapitalCost' + noteCount + '" class="ui-widget" onkeyup="updateSubtotal()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + cost + '</textarea></td>';
        }

        function getSubTotal(tooling, capital) {
            var cost = Number(tooling) + Number(capital);
            return '<td><textarea name="subtotal' + noteCount + '" rows="1" cols="20" readonly id="txtSubtotal' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + cost + '</textarea></td>';
        }

        function updateLabel() {
            var annualVolume = $('#MainContent_txtAnnualVolume').val();
            annualVolume = annualVolume.replace(',', '');
            var daysPerYear = $('#MainContent_txtDaysPerYear').val();
            daysPerYear = daysPerYear.replace(',', '');
            var hoursPerShift = $('#MainContent_txtHoursPerShift').val();
            hoursPerShift = hoursPerShift.replace(',', '');
            var shiftsPerDay = $('#MainContent_txtShiftsPerDay').val();
            shiftsPerDay = shiftsPerDay.replace(',', '');
            var efficiency = $('#MainContent_txtEfficiency').val();
            efficiency = efficiency.replace(',', '').replace('%', '');
            var secondsPerHour = $('#MainContent_txtSecondsPerHour').val();
            secondsPerHour = secondsPerHour.replace(',', '');

            var tactTime = ((daysPerYear * hoursPerShift * shiftsPerDay * (efficiency / 100) * secondsPerHour) / annualVolume);
            $('#MainContent_txtTactTime').val(tactTime.toFixed(1));

            var netPartsPerHour = secondsPerHour / tactTime * (hoursPerShift / 8) * (efficiency / 100);
            $('#MainContent_txtNetPartsPerHour').val(netPartsPerHour.toFixed(1));

            var grossPartsPerHour = secondsPerHour / tactTime;
            $('#MainContent_txtGrossPartsPerHour').val(grossPartsPerHour.toFixed(1));

            var netPartsPerDay = netPartsPerHour * 8 * shiftsPerDay;
            $('#MainContent_txtNetPartsPerDay').val(netPartsPerDay.toFixed());

        }


        function deleteRow(id, oldTR) {
            $('#quoteTable tr#' + oldTR).remove();

            //var num = 50;
            //if (trID < 50) {
            //    num = trID;
            //}

            for (i = id; i < 50; i++) {
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
            var num = 50;
            if (trID < 50) {
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
            url = "CreateQuote.aspx?quoteNumber=" + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=4&rand=' + Math.random();
            window.open(url);
        }

        function showGeneralNoteDialog() {
            $('#generalNotesDialog').dialog({ width: 800, height: 600 });
            $('#generalNotesDialog').parent().appendTo("form");
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

        function updateSubtotal() {
            var total = 0;

            for (i = 0; i < 100; i++) {
                if ($('#txtToolingCost' + i.toString()).val() !== undefined) {
                    var tooling = Number($('#txtToolingCost' + i.toString()).val());
                    var capital = Number($('#txtCapitalCost' + i.toString()).val());

                    $('#txtSubtotal' + i.toString()).val(tooling + capital);
                    total += tooling + capital;
                }
            }

            document.getElementById('MainContent_txtTotalCost').value = ("Total: $" + total.toString());
        }
    </script>
</asp:Content>
