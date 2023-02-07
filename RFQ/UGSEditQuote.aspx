<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UGSEditQuote.aspx.cs" Inherits="RFQ.UGSEditQuote" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="min-height: 50px"></div>
    <%--<button type="button"></button>--%>
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
            <td>
                <input type="button" class="ui-widget mybutton" value="Create EC Job" onclick="sharepointSiteEC();" />
            </td>
        </tr>
        <tr>
            <td class="ui-widget">Quote ID: </td><td><asp:Label ID="lblquoteID" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Number: </td><td><asp:Label ID="lblQuoteNumber" runat="server"></asp:Label></td>
        </tr>
            <td class="ui-widget">Quote Version: </td><td><asp:Label ID="lblVersion" runat="server"></asp:Label></td>
            <td class="ui-widget">Quote Status: </td><td><asp:DropDownList ID="ddlStatus" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
        <tr>
            <td class="ui-widget">Part Number: </td><td><asp:TextBox ID="txtPartNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Part Name: </td><td><asp:TextBox ID="txtPartName" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">RFQ #: </td><td><asp:TextBox ID="txtRFQNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Length (in)</td><td><asp:TextBox ID="txtLength" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Width (in)</td><td><asp:TextBox ID="txtWidth" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Height (in)</td><td><asp:TextBox ID="txtHeight" runat="server" CssClass="ui-widget"></asp:TextBox></td>
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
            <td class="ui-widget">Shipping: </td><td><asp:DropDownList ID="ddlShipping" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblShipping" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Payment Terms:</td><td><asp:DropDownList ID="ddlPayment" runat="server" CssClass="ui-widget"></asp:DropDownList><asp:Label ID="lblPayment" runat="server" CssClass="ui-widget"></asp:Label></td>
            <td class="ui-widget">Lead Time: </td><td><asp:TextBox names="leadTime" ID="txtLeadTime" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Job Number: </td><td><asp:TextBox ID="txtJobNumber" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Use TSG logo / name: </td><td><asp:CheckBox runat="server" ID="cbUseTSG" CssClass="ui-widget" /></td>
            <td class="ui-widget">Total Cost: </td><td><asp:TextBox ID="txtTotalCost" runat="server" CssClass="ui-widget"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="ui-widget">Shipping Location: </td><td><asp:TextBox ID="txtShipping" runat="server" CssClass="ui-widget"></asp:TextBox></td>
            <td class="ui-widget">Quote Type: </td><td><asp:DropDownList ID="ddlQuoteType" runat="server" CssClass="ui-widget"></asp:DropDownList></td>
            <td colspan="2">
                <center>
                    <div id="btnBudget" class="ui-widget mybutton" onclick="openBudget();" >Open Budget</div>
                </center>
            </td>
        </tr>
        <tr class="blank_row">
            <td bgcolor="#FFFFFF" colspan="3">&nbsp;</td>
        </tr>
<%--        <tr>
            <td class="ui-widget" colspan="5">Description</td>
            <td class="ui-widget">Cost</td>
        </tr>--%>
    </table>
    <br />
    <div onclick="addNoteRow('','');" class="ui-widget mybutton" style="float: right;">Add Note Row</div><div id="addNoteRow"></div>
    <br />
    <Center>
        <h4>Notes</h4>
    </Center>
    <textarea id="txtNotes" cols="400" rows="30" runat="server" style="max-width: 1000px; width: 1000px"></textarea><br /><br />


    <asp:Literal ID="litHideBtn" runat="server"></asp:Literal>
    

    <label class="ui-widget">New Part Picture Upload: </label>
    <asp:FileUpload ID="filePicture" runat="server" />
    <asp:Button ID="btnSave_Click" runat="server"  Text="Save" CssClass="ui-widget mybutton" OnClick="btnSaveClick"/>
    <asp:Button ID="btnNewVersion_Click" runat="server" Text="Create New Version" CssClass="ui-widget mybutton" OnClick="btncreateNewVersionClick"/><br />


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

<%--    <asp:HiddenField runat="server" ID="hdnManagement" />
    <asp:HiddenField runat="server" ID="hdnProjectEng" />
    <asp:HiddenField runat="server" ID="hdnReadData" />
    <asp:HiddenField runat="server" ID="hdn3DModel" />
    <asp:HiddenField runat="server" ID="hdnDrawing" />
    <asp:HiddenField runat="server" ID="hdnUpdates" />
    <asp:HiddenField runat="server" ID="hdnProgramming" />
    <asp:HiddenField runat="server" ID="hdnCNC" />
    <asp:HiddenField runat="server" ID="hdnCertification" />
    <asp:HiddenField runat="server" ID="hdnGageRRCMM" />
    <asp:HiddenField runat="server" ID="hdnPartLayouts" />
    <asp:HiddenField runat="server" ID="hdnBase" />
    <asp:HiddenField runat="server" ID="hdnDetails" />
    <asp:HiddenField runat="server" ID="hdnLocationPins" />
    <asp:HiddenField runat="server" ID="hdnGoNoGoPins" />
    <asp:HiddenField runat="server" ID="hdnSPC" />
    <asp:HiddenField runat="server" ID="hdnGageRRFixtures" />
    <asp:HiddenField runat="server" ID="hdnAssemble" />
    <asp:HiddenField runat="server" ID="hdnPallets" />
    <asp:HiddenField runat="server" ID="hdnTransportation" />
    <asp:HiddenField runat="server" ID="hdnBasePlate" />
    <asp:HiddenField runat="server" ID="hdnAluminum" />
    <asp:HiddenField runat="server" ID="hdnSteel" />
    <asp:HiddenField runat="server" ID="hdnFixturePlank" />
    <asp:HiddenField runat="server" ID="hdnWood" />
    <asp:HiddenField runat="server" ID="hdnBushings" />
    <asp:HiddenField runat="server" ID="hdnDrillBlanks" />
    <asp:HiddenField runat="server" ID="hdnClamps" />
    <asp:HiddenField runat="server" ID="hdnIndicator" />
    <asp:HiddenField runat="server" ID="hdnIndCollar" />
    <asp:HiddenField runat="server" ID="hdnIndStorCase" />
    <asp:HiddenField runat="server" ID="hdnZeroSet" />
    <asp:HiddenField runat="server" ID="hdnSpcTriggers" />
    <asp:HiddenField runat="server" ID="hdnTempDrops" />
    <asp:HiddenField runat="server" ID="hdnHingeDrops" />
    <asp:HiddenField runat="server" ID="hdnRisers" />
    <asp:HiddenField runat="server" ID="hdnHandles" />
    <asp:HiddenField runat="server" ID="hdnJigFeet" />
    <asp:HiddenField runat="server" ID="hdnToolingBalls" />
    <asp:HiddenField runat="server" ID="hdnTBCovers" />
    <asp:HiddenField runat="server" ID="hdnTBPads" />
    <asp:HiddenField runat="server" ID="hdnSlides" />
    <asp:HiddenField runat="server" ID="hdnMagnets" />
    <asp:HiddenField runat="server" ID="hdnHardware" />
    <asp:HiddenField runat="server" ID="hdnLMI" />
    <asp:HiddenField runat="server" ID="hdnAnnodizing" />
    <asp:HiddenField runat="server" ID="hdnBlackOxide" />
    <asp:HiddenField runat="server" ID="hdnHeatTreat" />
    <asp:HiddenField runat="server" ID="hdnEngrvdTags" />
    <asp:HiddenField runat="server" ID="hdnCNCServices" />
    <asp:HiddenField runat="server" ID="hdnGrinding" />
    <asp:HiddenField runat="server" ID="hdnShipping" />
    <asp:HiddenField runat="server" ID="hdnThirdPartyCMM" />
    <asp:HiddenField runat="server" ID="hdnWelding" />
    <asp:HiddenField runat="server" ID="hdnWireBurn" />
    <asp:HiddenField runat="server" ID="hdnRebates" />--%>


    <div id="budgetDialog" style="display: none; padding: 20px; background-color: #D0D0D0;">
        <table>
            <tr>
                <td colspan="3">
                    <center>
                        <b>Labor</ b>
                    </center>
                </td>
                <td colspan="3">
                    <center>
                        <b>Material</b>
                    </center>
                </td>
                <td colspan="3">
                    <center>
                        <b>Outsourcing</b>
                    </center>
                </td>
            </tr>
<tr>
     <td>
         Management
     </td>
     <td>
         <asp:TextBox ID="txtManagement" Text="0" runat="server" onkeyup="updateLabel('txtManagement', 'Management', 'tManagement')" onkeypress="return EnterEvent(event, 'txtProjectEng')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tManagement" onkeyup="updateLabel('txtManagement', 'Management', 'tManagement')" onkeypress="return EnterEvent(event, 'txtProjectEng')" />
     </td>
     <td>
         <asp:TextBox ID="Management" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Base Plate
     </td>
     <td>
         <asp:TextBox ID="txtBasePlate" Text="0" runat="server" onkeyup="updateLabel('txtBasePlate', 'BasePlate', 'tBasePlate')" onkeypress="return EnterEvent(event, 'txtAluminum')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tBasePlate" onkeyup="updateLabel('txtBasePlate', 'BasePlate', 'tBasePlate')" onkeypress="return EnterEvent(event, 'txtAluminum')" />
     </ td>
     <td>
         <asp:TextBox ID="BasePlate" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Annodizing
     </td>
     <td>
         <asp:TextBox ID="txtAnnodizing" Text="0" runat="server" onkeyup="updateLabel('txtAnnodizing', 'Annodizing', 'tAnnodizing')" onkeypress="return EnterEvent(event, 'txtBlackOxide')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tAnnodizing" onkeyup="updateLabel('txtAnnodizing', 'Annodizing', 'tAnnodizing')" onkeypress="return EnterEvent(event, 'txtBlackOxide')" />
     </ td>
     <td>
         <asp:TextBox ID="Annodizing" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Project ENG
     </td>
     <td>
         <asp:TextBox ID="txtProjectEng" Text="0" runat="server" onkeyup="updateLabel('txtProjectEng', 'ProjectEng', 'tProjectEng')" onkeypress="return EnterEvent(event, 'txtReadData')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tProjectEng" onkeyup="updateLabel('txtProjectEng', 'ProjectEng', 'tProjectEng')" onkeypress="return EnterEvent(event, 'txtReadData')" />
     </ td>
     <td>
         <asp:TextBox ID="ProjectEng" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Aluminum
     </td>
     <td>
         <asp:TextBox ID="txtAluminum" Text="0" runat="server" onkeyup="updateLabel('txtAluminum', 'Aluminum', 'tAluminum')" onkeypress="return EnterEvent(event, 'txtSteel')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tAluminum" onkeyup="updateLabel('txtAluminum', 'Aluminum', 'tAluminum')" onkeypress="return EnterEvent(event, 'txtSteel')" />
     </ td>
     <td>
         <asp:TextBox ID="Aluminum" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Black Oxide
     </td>
     <td>
         <asp:TextBox ID="txtBlackOxide" Text="0" runat="server" onkeyup="updateLabel('txtBlackOxide', 'BlackOxide', 'tBlackOxide')" onkeypress="return EnterEvent(event, 'txtHeatTreat')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tBlackOxide" onkeyup="updateLabel('txtBlackOxide', 'BlackOxide', 'tBlackOxide')" onkeypress="return EnterEvent(event, 'txtHeatTreat')" />
     </ td>
     <td>
         <asp:TextBox ID="BlackOxide" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td colspan="4">
         <center>
             <b>Design</b>
         </center>
     </td>
     <td>
         Steel
     </td>
     <td>
         <asp:TextBox ID="txtSteel" Text="0" runat="server" onkeyup="updateLabel('txtSteel', 'Steel', 'tSteel')" onkeypress="return EnterEvent(event, 'txtFixturePlank')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tSteel" onkeyup="updateLabel('txtSteel', 'Steel', 'tSteel')" onkeypress="return EnterEvent(event, 'txtFixturePlank')" />
     </ td>
     <td>
         <asp:TextBox ID="Steel" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Heat Treat
     </td>
     <td>
         <asp:TextBox ID="txtHeatTreat" Text="0" runat="server" onkeyup="updateLabel('txtHeatTreat', 'HeatTreat', 'tHeatTreat')" onkeypress="return EnterEvent(event, 'txtEngrvdTags')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tHeatTreat" onkeyup="updateLabel('txtHeatTreat', 'HeatTreat', 'tHeatTreat')" onkeypress="return EnterEvent(event, 'txtEngrvdTags')" />
     </ td>
     <td>
         <asp:TextBox ID="HeatTreat" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Read Data
     </td>
     <td>
         <asp:TextBox ID="txtReadData" Text="0" runat="server" onkeyup="updateLabel('txtReadData', 'ReadData', 'tReadData')" onkeypress="return EnterEvent(event, 'txt3DModel')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tReadData" onkeyup="updateLabel('txtReadData', 'ReadData', 'tReadData')" onkeypress="return EnterEvent(event, 'txt3DModel')" />
     </ td>
     <td>
         <asp:TextBox ID="ReadData" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Fixture Plank
     </td>
     <td>
         <asp:TextBox ID="txtFixturePlank" Text="0" runat="server" onkeyup="updateLabel('txtFixturePlank', 'FixturePlank', 'tFixturePlank')" onkeypress="return EnterEvent(event, 'txtWood')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tFixturePlank" onkeyup="updateLabel('txtFixturePlank', 'FixturePlank', 'tFixturePlank')" onkeypress="return EnterEvent(event, 'txtWood')" />
     </ td>
     <td>
         <asp:TextBox ID="FixturePlank" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Engrvd Tags
     </td>
     <td>
         <asp:TextBox ID="txtEngrvdTags" Text="0" runat="server" onkeyup="updateLabel('txtEngrvdTags', 'EngrvdTags', 'tEngrvdTags')" onkeypress="return EnterEvent(event, 'txtCNCServices')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tEngrvdTags" onkeyup="updateLabel('txtEngrvdTags', 'EngrvdTags', 'tEngrvdTags')" onkeypress="return EnterEvent(event, 'txtCNCServices')" />
     </ td>
     <td>
         <asp:TextBox ID="EngrvdTags" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         3-D Model
     </td>
     <td>
         <asp:TextBox ID="txt3DModel" Text="0" runat="server" onkeyup="updateLabel('txt3DModel', 'Model', 't3DModel')" onkeypress="return EnterEvent(event, 'txtDrawings')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="t3DModel" onkeyup="updateLabel('txt3DModel', 'Model', 't3DModel')" onkeypress="return EnterEvent(event, 'txtDrawings')" />
     </ td>
     <td>
         <asp:TextBox ID="Model" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Wood
     </td>
     <td>
         <asp:TextBox ID="txtWood" Text="0" runat="server" onkeyup="updateLabel('txtWood', 'Wood', 'tWood')" onkeypress="return EnterEvent(event, 'txtBushings')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tWood" onkeyup="updateLabel('txtWood', 'Wood', 'tWood')" onkeypress="return EnterEvent(event, 'txtBushings')" />
     </ td>
     <td>
         <asp:TextBox ID="Wood" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         CNC Services
     </td>
     <td>
         <asp:TextBox ID="txtCNCServices" Text="0" runat="server" onkeyup="updateLabel('txtCNCServices', 'CNCServices', 'tCNCServices')" onkeypress="return EnterEvent(event, 'txtGrinding')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tCNCServices" onkeyup="updateLabel('txtCNCServices', 'CNCServices', 'tCNCServices')" onkeypress="return EnterEvent(event, 'txtGrinding')" />
     </ td>
     <td>
         <asp:TextBox ID="CNCServices" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Drawings
     </td>
     <td>
         <asp:TextBox ID="txtDrawings" Text="0" runat="server" onkeyup="updateLabel('txtDrawings', 'Drawings', 'tDrawing')" onkeypress="return EnterEvent(event, 'txtUpdates')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tDrawing" onkeyup="updateLabel('txtDrawings', 'Drawings', 'tDrawing')" onkeypress="return EnterEvent(event, 'txtUpdates')" />
     </ td>
     <td>
         <asp:TextBox ID="Drawings" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Bushings
     </td>
     <td>
         <asp:TextBox ID="txtBushings" Text="0" runat="server" onkeyup="updateLabel('txtBushings', 'Bushings', 'tBushings')" onkeypress="return EnterEvent(event, 'txtDrillBlanks')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tBushings" onkeyup="updateLabel('txtBushings', 'Bushings', 'tBushings')" onkeypress="return EnterEvent(event, 'txtDrillBlanks')" />
     </ td>
     <td>
         <asp:TextBox ID="Bushings" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         ID/OD Grinding
     </td>
     <td>
         <asp:TextBox ID="txtGrinding" Text="0" runat="server" onkeyup="updateLabel('txtGrinding', 'Grinding', 'tGrinding')" onkeypress="return EnterEvent(event, 'txtShippingCalc')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tGrinding" onkeyup="updateLabel('txtGrinding', 'Grinding', 'tGrinding')" onkeypress="return EnterEvent(event, 'txtShippingCalc')" />
     </ td>
     <td>
         <asp:TextBox ID="Grinding" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Updates
     </td>
     <td>
         <asp:TextBox ID="txtUpdates" Text="0" runat="server" onkeyup="updateLabel('txtUpdates', 'Updates', 'tUpdates')" onkeypress="return EnterEvent(event, 'txtProgramming')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tUpdates" onkeyup="updateLabel('txtUpdates', 'Updates', 'tUpdates')" onkeypress="return EnterEvent(event, 'txtProgramming')" />
     </ td>
     <td>
         <asp:TextBox ID="Updates" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Drill Blanks
     </td>
     <td>
         <asp:TextBox ID="txtDrillBlanks" Text="0" runat="server" onkeyup="updateLabel('txtDrillBlanks', 'DrillBlanks', 'tDrillBlanks')" onkeypress="return EnterEvent(event, 'txtClamps')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tDrillBlanks" onkeyup="updateLabel('txtDrillBlanks', 'DrillBlanks', 'tDrillBlanks')" onkeypress="return EnterEvent(event, 'txtClamps')" />
     </ td>
     <td>
         <asp:TextBox ID="DrillBlanks" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Shipping
     </td>
     <td>
         <asp:TextBox ID="txtShippingCalc" Text="0" runat="server" onkeyup="updateLabel('txtShippingCalc', 'ShippingCalc', 'tShipping')" onkeypress="return EnterEvent(event, 'txtThirdPartyCMM')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tShipping" onkeyup="updateLabel('txtShippingCalc', 'ShippingCalc', 'tShipping')" onkeypress="return EnterEvent(event, 'txtThirdPartyCMM')" />
     </ td>
     <td>
         <asp:TextBox ID="ShippingCalc" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td colspan="4">
         <center>
             <b>CNC</b>
         </center>
     </td>
     <td>
         Clamps
     </td>
     <td>
         <asp:TextBox ID="txtClamps" Text="0" runat="server" onkeyup="updateLabel('txtClamps', 'Clamps', 'tClamps')" onkeypress="return EnterEvent(event, 'txtIndicator')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tClamps" onkeyup="updateLabel('txtClamps', 'Clamps', 'tClamps')" onkeypress="return EnterEvent(event, 'txtIndicator')" />
     </ td>
     <td>
         <asp:TextBox ID="Clamps" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Third Party CMM
     </td>
     <td>
         <asp:TextBox ID="txtThirdPartyCMM" Text="0" runat="server" onkeyup="updateLabel('txtThirdPartyCMM', 'ThirdPartyCMM', 'tThirdPartyCMM')" onkeypress="return EnterEvent(event, 'txtWelding')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tThirdPartyCMM" onkeyup="updateLabel('txtThirdPartyCMM', 'ThirdPartyCMM', 'tThirdPartyCMM')" onkeypress="return EnterEvent(event, 'txtWelding')" />
     </ td>
     <td>
         <asp:TextBox ID="ThirdPartyCMM" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Programming
     </td>
     <td>
         <asp:TextBox ID="txtProgramming" Text="0" runat="server" onkeyup="updateLabel('txtProgramming', 'Programming', 'tProgramming')" onkeypress="return EnterEvent(event, 'txtCNC')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tProgramming" onkeyup="updateLabel('txtProgramming', 'Programming', 'tProgramming')" onkeypress="return EnterEvent(event, 'txtCNC')" />
     </ td>
     <td>
         <asp:TextBox ID="Programming" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Indicator
     </td>
     <td>
         <asp:TextBox ID="txtIndicator" Text="0" runat="server" onkeyup="updateLabel('txtIndicator', 'Indicator', 'tIndicator')" onkeypress="return EnterEvent(event, 'txtIndCollar')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tIndicator" onkeyup="updateLabel('txtIndicator', 'Indicator', 'tIndicator')" onkeypress="return EnterEvent(event, 'txtIndCollar')" />
     </ td>
     <td>
         <asp:TextBox ID="Indicator" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Welding
     </td>
     <td>
         <asp:TextBox ID="txtWelding" Text="0" runat="server" onkeyup="updateLabel('txtWelding', 'Welding', 'tWelding')" onkeypress="return EnterEvent(event, 'txtWireBurn')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tWelding" onkeyup="updateLabel('txtWelding', 'Welding', 'tWelding')" onkeypress="return EnterEvent(event, 'txtWireBurn')" />
     </ td>
     <td>
         <asp:TextBox ID="Welding" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         CNC
     </td>
     <td>
         <asp:TextBox ID="txtCNC" Text="0" runat="server" onkeyup="updateLabel('txtCNC', 'CNC', 'tCNC')" onkeypress="return EnterEvent(event, 'txtCertification')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tCNC" onkeyup="updateLabel('txtCNC', 'CNC', 'tCNC')" onkeypress="return EnterEvent(event, 'txtCertification')" />
     </ td>
     <td>
         <asp:TextBox ID="CNC" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Ind Collar
     </td>
     <td>
         <asp:TextBox ID="txtIndCollar" Text="0" runat="server" onkeyup="updateLabel('txtIndCollar', 'IndCollar', 'tIndCollar')" onkeypress="return EnterEvent(event, 'txtIndStorCase')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tIndCollar" onkeyup="updateLabel('txtIndCollar', 'IndCollar', 'tIndCollar')" onkeypress="return EnterEvent(event, 'txtIndStorCase')" />
     </ td>
     <td>
         <asp:TextBox ID="IndCollar" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Wire Burn
     </td>
     <td>
         <asp:TextBox ID="txtWireBurn" Text="0" runat="server" onkeyup="updateLabel('txtWireBurn', 'WireBurn', 'tWireBurn')" onkeypress="return EnterEvent(event, 'txtRebates')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tWireBurn" onkeyup="updateLabel('txtWireBurn', 'WireBurn', 'tWireBurn')" onkeypress="return EnterEvent(event, 'txtRebates')" />
     </ td>
     <td>
         <asp:TextBox ID="WireBurn" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td colspan="4">
         <center>
             <b>CMM</b>
         </center>
     </td>
     <td>
         Ind Stor Case
     </td>
     <td>
         <asp:TextBox ID="txtIndStorCase" Text="0" runat="server" onkeyup="updateLabel('txtIndStorCase', 'IndStorCase', 'tIndStorCase')" onkeypress="return EnterEvent(event, 'txtZeroSet')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tIndStorCase" onkeyup="updateLabel('txtIndStorCase', 'IndStorCase', 'tIndStorCase')" onkeypress="return EnterEvent(event, 'txtZeroSet')" />
     </ td>
     <td>
         <asp:TextBox ID="IndStorCase" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td colspan="3">
         <center>
             <b>Total-Outside Services</b>
         </center>
     </td>
     <td>
         <asp:TextBox ID="TotalOutsideServices" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Certification
     </td>
     <td>
         <asp:TextBox ID="txtCertification" Text="0" runat="server" onkeyup="updateLabel('txtCertification', 'Certification', 'tCertification')" onkeypress="return EnterEvent(event, 'txtGageRR')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tCertification" onkeyup="updateLabel('txtCertification', 'Certification', 'tCertification')" onkeypress="return EnterEvent(event, 'txtGageRR')" />
     </ td>
     <td>
         <asp:TextBox ID="Certification" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Zero Set
     </td>
     <td>
         <asp:TextBox ID="txtZeroSet" Text="0" runat="server" onkeyup="updateLabel('txtZeroSet', 'ZeroSet', 'tZeroSet')" onkeypress="return EnterEvent(event, 'txtSpcTriggers')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tZeroSet" onkeyup="updateLabel('txtZeroSet', 'ZeroSet', 'tZeroSet')" onkeypress="return EnterEvent(event, 'txtSpcTriggers')" />
     </ td>
     <td>
         <asp:TextBox ID="ZeroSet" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td colspan="4">
         <center>
             <b>Rebates</b>
         </center>
     </td>
 </tr>
 <tr>
     <td>
         Gage R&R
     </td>
     <td>
         <asp:TextBox ID="txtGageRR" Text="0" runat="server" onkeyup="updateLabel('txtGageRR', 'GageRR', 'tGageRRCMM')" onkeypress="return EnterEvent(event, 'txtPartLayouts')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tGageRRCMM" onkeyup="updateLabel('txtGageRR', 'GageRR', 'tGageRRCMM')" onkeypress="return EnterEvent(event, 'txtPartLayouts')" />
     </ td>
     <td>
         <asp:TextBox ID="GageRR" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Spc Triggers
     </td>
     <td>
         <asp:TextBox ID="txtSpcTriggers" Text="0" runat="server" onkeyup="updateLabel('txtSpcTriggers', 'SpcTriggers', 'tSpcTriggers')" onkeypress="return EnterEvent(event, 'txtTempDrops')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tSpcTriggers" onkeyup="updateLabel('txtSpcTriggers', 'SpcTriggers', 'tSpcTriggers')" onkeypress="return EnterEvent(event, 'txtTempDrops')" />
     </ td>
     <td>
         <asp:TextBox ID="SpcTriggers" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Rebates
     </td>
     <td>
         <asp:TextBox ID="txtRebates" Text="0" runat="server" onkeyup="updateLabel('txtRebates', 'Rebates', 'tRebates')" onkeypress="return EnterEvent(event, 'txtRebates')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tRebates" onkeyup="updateLabel('txtRebates', 'Rebates', 'tRebates')" onkeypress="return EnterEvent(event, 'txtRebates')" />
     </ td>
     <td>
         <asp:TextBox ID="Rebates" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Part Layouts
     </td>
     <td>
         <asp:TextBox ID="txtPartLayouts" Text="0" runat="server" onkeyup="updateLabel('txtPartLayouts', 'PartLayouts', 'tPartLayouts')" onkeypress="return EnterEvent(event, 'txtBase')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tPartLayouts" onkeyup="updateLabel('txtPartLayouts', 'PartLayouts', 'tPartLayouts')" onkeypress="return EnterEvent(event, 'txtBase')" />
     </ td>
     <td>
         <asp:TextBox ID="PartLayouts" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Temp Drops
     </td>
     <td>
         <asp:TextBox ID="txtTempDrops" Text="0" runat="server" onkeyup="updateLabel('txtTempDrops', 'TempDrops', 'tTempDrops')" onkeypress="return EnterEvent(event, 'txtHingeDrops')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tTempDrops" onkeyup="updateLabel('txtTempDrops', 'TempDrops', 'tTempDrops')" onkeypress="return EnterEvent(event, 'txtHingeDrops')" />
     </ td>
     <td>
         <asp:TextBox ID="TempDrops" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td colspan="4">
         <center>
             <b>Fixtures</b>
         </center>
     </td>
     <td>
         Hinge Drops
     </td>
     <td>
         <asp:TextBox ID="txtHingeDrops" Text="0" runat="server" onkeyup="updateLabel('txtHingeDrops', 'HingeDrops', 'tHingeDrops')" onkeypress="return EnterEvent(event, 'txtRisers')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tHingeDrops" onkeyup="updateLabel('txtHingeDrops', 'HingeDrops', 'tHingeDrops')" onkeypress="return EnterEvent(event, 'txtRisers')" />
     </ td>
     <td>
         <asp:TextBox ID="HingeDrops" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Base
     </td>
     <td>
         <asp:TextBox ID="txtBase" Text="0" runat="server" onkeyup="updateLabel('txtBase', 'Base', 'tBase')" onkeypress="return EnterEvent(event, 'txtDetails')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tBase" onkeyup="updateLabel('txtBase', 'Base', 'tBase')" onkeypress="return EnterEvent(event, 'txtDetails')" />
     </ td>
     <td>
         <asp:TextBox ID="Base" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Risers
     </td>
     <td>
         <asp:TextBox ID="txtRisers" Text="0" runat="server" onkeyup="updateLabel('txtRisers', 'Risers', 'tRisers')" onkeypress="return EnterEvent(event, 'txtHandles')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tRisers" onkeyup="updateLabel('txtRisers', 'Risers', 'tRisers')" onkeypress="return EnterEvent(event, 'txtHandles')" />
     </ td>
     <td>
         <asp:TextBox ID="Risers" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Details
     </td>
     <td>
         <asp:TextBox ID="txtDetails" Text="0" runat="server" onkeyup="updateLabel('txtDetails', 'Details', 'tDetails')" onkeypress="return EnterEvent(event, 'txtLocationPins')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tDetails" onkeyup="updateLabel('txtDetails', 'Details', 'tDetails')" onkeypress="return EnterEvent(event, 'txtLocationPins')" />
     </ td>
     <td>
         <asp:TextBox ID="Details" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Handles
     </td>
     <td>
         <asp:TextBox ID="txtHandles" Text="0" runat="server" onkeyup="updateLabel('txtHandles', 'Handles', 'tHandles')" onkeypress="return EnterEvent(event, 'txtJigFeet')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tHandles" onkeyup="updateLabel('txtHandles', 'Handles', 'tHandles')" onkeypress="return EnterEvent(event, 'txtJigFeet')" />
     </ td>
     <td>
         <asp:TextBox ID="Handles" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Location Pins
     </td>
     <td>
         <asp:TextBox ID="txtLocationPins" Text="0" runat="server" onkeyup="updateLabel('txtLocationPins', 'LocationPins', 'tLocationPins')" onkeypress="return EnterEvent(event, 'txtGoNoGoPins')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tLocationPins" onkeyup="updateLabel('txtLocationPins', 'LocationPins', 'tLocationPins')" onkeypress="return EnterEvent(event, 'txtGoNoGoPins')" />
     </ td>
     <td>
         <asp:TextBox ID="LocationPins" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Jig Feet
     </td>
     <td>
         <asp:TextBox ID="txtJigFeet" Text="0" runat="server" onkeyup="updateLabel('txtJigFeet', 'JigFeet', 'tJigFeet')" onkeypress="return EnterEvent(event, 'txtToolingBalls')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tJigFeet" onkeyup="updateLabel('txtJigFeet', 'JigFeet', 'tJigFeet')" onkeypress="return EnterEvent(event, 'txtToolingBalls')" />
     </ td>
     <td>
         <asp:TextBox ID="JigFeet" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Go/No Go Pins
     </td>
     <td>
         <asp:TextBox ID="txtGoNoGoPins" Text="0" runat="server" onkeyup="updateLabel('txtGoNoGoPins', 'GoNoGoPins', 'tGoNoGoPins')" onkeypress="return EnterEvent(event, 'txtSPC')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tGoNoGoPins" onkeyup="updateLabel('txtGoNoGoPins', 'GoNoGoPins', 'tGoNoGoPins')" onkeypress="return EnterEvent(event, 'txtSPC')" />
     </ td>
     <td>
         <asp:TextBox ID="GoNoGoPins" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Tooling Balls
     </td>
     <td>
         <asp:TextBox ID="txtToolingBalls" Text="0" runat="server" onkeyup="updateLabel('txtToolingBalls', 'ToolingBalls', 'tToolingBalls')" onkeypress="return EnterEvent(event, 'txtTBCovers')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tToolingBalls" onkeyup="updateLabel('txtToolingBalls', 'ToolingBalls', 'tToolingBalls')" onkeypress="return EnterEvent(event, 'txtTBCovers')" />
     </ td>
     <td>
         <asp:TextBox ID="ToolingBalls" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         SPC's
     </td>
     <td>
         <asp:TextBox ID="txtSPC" Text="0" runat="server" onkeyup="updateLabel('txtSPC', 'SPC', 'tSPC')" onkeypress="return EnterEvent(event, 'txtGageRRF')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tSPC" onkeyup="updateLabel('txtSPC', 'SPC', 'tSPC')" onkeypress="return EnterEvent(event, 'txtGageRRF')" />
     </ td>
     <td>
         <asp:TextBox ID="SPC" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         TB Covers
     </td>
     <td>
         <asp:TextBox ID="txtTBCovers" Text="0" runat="server" onkeyup="updateLabel('txtTBCovers', 'TBCovers', 'tTBCovers')" onkeypress="return EnterEvent(event, 'txtTBPads')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tTBCovers" onkeyup="updateLabel('txtTBCovers', 'TBCovers', 'tTBCovers')" onkeypress="return EnterEvent(event, 'txtTBPads')" />
     </ td>
     <td>
         <asp:TextBox ID="TBCovers" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Gage R&R
     </td>
     <td>
         <asp:TextBox ID="txtGageRRF" Text="0" runat="server" onkeyup="updateLabel('txtGageRRF', 'GageRRF', 'tGageRRFixtures')" onkeypress="return EnterEvent(event, 'txtAssemble')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tGageRRFixtures" onkeyup="updateLabel('txtGageRRF', 'GageRRF', 'tGageRRFixtures')" onkeypress="return EnterEvent(event, 'txtAssemble')" />
     </ td>
     <td>
         <asp:TextBox ID="GageRRF" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         TB Pads
     </td>
     <td>
         <asp:TextBox ID="txtTBPads" Text="0" runat="server" onkeyup="updateLabel('txtTBPads', 'TBPads', 'tTBPads')" onkeypress="return EnterEvent(event, 'txtSlides')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tTBPads" onkeyup="updateLabel('txtTBPads', 'TBPads', 'tTBPads')" onkeypress="return EnterEvent(event, 'txtSlides')" />
     </ td>
     <td>
         <asp:TextBox ID="TBPads" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Assemble
     </td>
     <td>
         <asp:TextBox ID="txtAssemble" Text="0" runat="server" onkeyup="updateLabel('txtAssemble', 'Assemble', 'tAssemble')" onkeypress="return EnterEvent(event, 'txtPallets')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tAssemble" onkeyup="updateLabel('txtAssemble', 'Assemble', 'tAssemble')" onkeypress="return EnterEvent(event, 'txtPallets')" />
     </ td>
     <td>
         <asp:TextBox ID="Assemble" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Slides
     </td>
     <td>
         <asp:TextBox ID="txtSlides" Text="0" runat="server" onkeyup="updateLabel('txtSlides', 'Slides', 'tSlides')" onkeypress="return EnterEvent(event, 'txtMagnets')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tSlides" onkeyup="updateLabel('txtSlides', 'Slides', 'tSlides')" onkeypress="return EnterEvent(event, 'txtMagnets')" />
     </ td>
     <td>
         <asp:TextBox ID="Slides" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td colspan="4">
         <center>
             <b>General</b>
         </center>
     </td>
     <td>
         Magnets
     </td>
     <td>
         <asp:TextBox ID="txtMagnets" Text="0" runat="server" onkeyup="updateLabel('txtMagnets', 'Magnets', 'tMagnets')" onkeypress="return EnterEvent(event, 'txtHardware')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tMagnets" onkeyup="updateLabel('txtMagnets', 'Magnets', 'tMagnets')" onkeypress="return EnterEvent(event, 'txtHardware')" />
     </ td>
     <td>
         <asp:TextBox ID="Magnets" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Pallets & Crates
     </td>
     <td>
         <asp:TextBox ID="txtPallets" Text="0" runat="server" onkeyup="updateLabel('txtPallets', 'Pallets', 'tPallets')" onkeypress="return EnterEvent(event, 'txtTransportation')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tPallets" onkeyup="updateLabel('txtPallets', 'Pallets', 'tPallets')" onkeypress="return EnterEvent(event, 'txtTransportation')" />
     </ td>
     <td>
         <asp:TextBox ID="Pallets" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         Hardware
     </td>
     <td>
         <asp:TextBox ID="txtHardware" Text="0" runat="server" onkeyup="updateLabel('txtHardware', 'Hardware', 'tHardware')" onkeypress="return EnterEvent(event, 'txtLMI')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tHardware" onkeyup="updateLabel('txtHardware', 'Hardware', 'tHardware')" onkeypress="return EnterEvent(event, 'txtLMI')" />
     </ td>
     <td>
         <asp:TextBox ID="Hardware" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
 </tr>
 <tr>
     <td>
         Transportation
     </td>
     <td>
         <asp:TextBox ID="txtTransportation" Text="0" runat="server" onkeyup="updateLabel('txtTransportation', 'Transportation', 'tTransportation')" onkeypress="return EnterEvent(event, 'txtTransportation')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tTransportation" onkeyup="updateLabel('txtTransportation', 'Transportation', 'tTransportation')" onkeypress="return EnterEvent(event, 'txtTransportation')" />
     </ td>
     <td>
         <asp:TextBox ID="Transportation" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td>
         LMI
     </td>
     <td>
         <asp:TextBox ID="txtLMI" Text="0" runat="server" onkeyup="updateLabel('txtLMI', 'LMI', 'tLMI')" onkeypress="return EnterEvent(event, 'txtLMI')" style="width: 40px;"></asp:TextBox>
     </td>
     <td>
         <asp:TextBox runat="server" style="width: 50px;" ID="tLMI" onkeyup="updateLabel('txtLMI', 'LMI', 'tLMI')" onkeypress="return EnterEvent(event, 'txtLMI')" />
     </ td>
     <td>
         <asp:TextBox ID="LMI" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
     </td>
     <td colspan="2">
         <Center>
             <b>Total</b>
         </Center>
     </td>
     <td colspan="2">
         <asp:TextBox ID="Total" runat="server" style="width: 100px;"></asp:TextBox>
     </td>
 </tr>
            <tr>
                <td colspan="2">
                    <Center>
                        <b>Total Labor</b>
                    </Center>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="TotalLabor" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
                </td>
                <td colspan="2">
                    <Center>
                        <b>Total Material Items</b>
                    </Center>
                </td>
                <td colspan="2">
                    <asp:TextBox ID="TotalMaterial" runat="server" ReadOnly="true" style="width: 80px;"></asp:TextBox>
                </td>
                <td colspan="2">

                </td>
                <td colspan="2">
                    <center>
                        <div class="mybutton" onclick="applyTotal();">Apply Cost</div>
                    </center>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="HiddenField1" Value="0" runat="server" />

    <asp:HiddenField ID="hdnNoteOrder" Value="0" runat="server" />
    <asp:HiddenField ID="hdnQuoteNumber" Value="0" runat="server" />
    <%--<asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="downloadQuote();" />--%>
    <asp:CheckBox ID="chkCreatedDate" runat="server" Text="Use Created Date in PDF" Checked="true" />
    <asp:Button ID="btnSaveQuote_Click" runat="server" Text="Download Quote PDF" CssClass="ui-widget mybutton" OnClientClick="javascript:window.open('CreateQuote.aspx?quoteNumber=' + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=5&dateCreated=' + $('#MainContent_chkCreatedDate').is(':checked') + '&rand=' + Math.random());  return false;" />
    <br />
        <br />
         <br />
   <%-- <asp:Button ID="btnFinalize" CssClass="ui-widget mybutton" runat="server" OnClick="btnFinalize_Click" Text="Finalize" />--%>

    <script type="text/javascript">
        //captures enter event
        function EnterEvent(e, name) {
            if (e.keyCode == 13) {
                $('#MainContent_' + name).focus();
                return false;
            }
        }

        var noteCount = 0;
        var trID = 0;
        var lastID = 0;

        function addNoteRow(note, costNote) {
            if (note == null || costNote == null) {
                note = '';
                costNote = '';
            }

            $('#quoteTable').append('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + noteCount + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + note + '</textarea></td><td class="ui-widget"><textarea name="price' + noteCount + '" rows="1" cols="20" id="txtCostNotes' + noteCount + '" class="ui-widget" onkeyup="updateTotal();" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + costNote + '</textarea></asp:TextBox></td><td><div id="add' + noteCount + '" onclick="addRow(' + noteCount + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + noteCount + '" onclick="deleteRow(' + noteCount + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');

            document.getElementById('txtNotes' + noteCount).focus();
            trID++;
            noteCount++;

            //$('#quoteTable').append('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + noteCount + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + noteCount + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + note + '</textarea></td><td class="ui-widget"><textarea name="price' + noteCount + '" rows="1" cols="20" id="txtCostNotes' + noteCount + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + costNote + '</textarea></asp:TextBox></td><td><div id="add' + noteCount + '" onclick="addRow(' + noteCount + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + noteCount + '" onclick="deleteRow(' + noteCount + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
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
            updateTotal();
        }

        function addRow(id, oldTrID) {
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

            $('#quoteTable tr#' + oldTrID).after('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + Number(id + 1) + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + Number(id + 1) + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + '' + '</textarea></td><td class="ui-widget"><textarea name="price' + Number(id + 1) + '" rows="1" cols="20" id="txtCostNotes' + Number(id + 1) + '" class="ui-widget" onkeyup="updateTotal();" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td><div id="add' + Number(id + 1) + '" onclick="addRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + Number(id + 1) + '" onclick="deleteRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
            trID++;
            noteCount++;

            updateTotal();
            //$('#quoteTable tr#' + oldTrID).after('<tr id="' + trID + '"><td class="ui-widget" colspan="5"><textarea name="notes' + Number(id + 1) + '" onfocus="getID();" rows="1" cols="120" id="txtNotes' + Number(id + 1) + '" class="ui-widget" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px; maxlength="1000"">' + '' + '</textarea></td><td class="ui-widget"><textarea name="price' + Number(id + 1) + '" rows="1" cols="20" id="txtCostNotes' + Number(id + 1) + '" class="ui-widget" onkeyup="updateCost()" style="max-width: 100%; margin-top: 0px; margin-bottom: 0px; height: 30px;">' + '' + '</textarea></asp:TextBox></td><td><div id="add' + Number(id + 1) + '" onclick="addRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5">+</ font></div></td><td><div id="remove' + Number(id + 1) + '" onclick="deleteRow(' + Number(id + 1) + ',' + trID + ');" ><font size="5" color="red">-</ font></div></td></tr>');
        }

        function updateTotal() {
            var total = 0;
            for (i = 0; i < 200; i++) {
                if ($('#txtCostNotes' + i.toString()).val() === undefined) {

                }
                else {
                    total += Number($('#txtCostNotes' + i).val());
                    //alert(total.toString());
                }
            }
            $('#MainContent_txtTotalCost').val(total);
        }

        function getID() {
            lastID = $(document.activeElement).attr('id');
        }

        function sharepointSite() {
            var url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html() + '&company=15';
            window.open(url);
        }

        function sharepointSiteEC() {
            var url = 'CreateJobSite?id=' + $('#MainContent_lblquoteID').html() + '&company=15&EC=True';
            window.open(url);
        }

        function downloadQuote() {
            url = "CreateQuote.aspx?quoteNumber=" + $('#MainContent_hdnQuoteNumber').val() + '&quoteType=5&rand=' + Math.random();
            window.open(url);
        }

        function calculateLabor() {
            document.getElementById('MainContent_TotalLabor').value = '$' + String(Number(Number($('#MainContent_Management').val().replace("$", "")) + Number($('#MainContent_ProjectEng').val().replace("$", "")) + Number($('#MainContent_ReadData').val().replace("$", "")) + Number($('#MainContent_Model').val().replace("$", "")) + Number($('#MainContent_Drawings').val().replace("$", "")) + Number($('#MainContent_Updates').val().replace("$", "")) + Number($('#MainContent_Programming').val().replace("$", "")) + Number($('#MainContent_CNC').val().replace("$", "")) + Number($('#MainContent_Certification').val().replace("$", "")) + Number($('#MainContent_GageRR').val().replace("$", "")) + Number($('#MainContent_PartLayouts').val().replace("$", "")) + Number($('#MainContent_Base').val().replace("$", "")) + Number($('#MainContent_Details').val().replace("$", "")) + Number($('#MainContent_LocationPins').val().replace("$", "")) + Number($('#MainContent_GoNoGoPins').val().replace("$", "")) + Number($('#MainContent_SPC').val().replace("$", "")) + Number($('#MainContent_GageRRF').val().replace("$", "")) + Number($('#MainContent_Assemble').val().replace("$", "")) + Number($('#MainContent_Pallets').val().replace("$", "")) + Number($('#MainContent_Transportation').val().replace("$", ""))).toFixed(2));
        }

        function calculateMaterial() {
            document.getElementById('MainContent_TotalMaterial').value = '$' + String(Number(Number($('#MainContent_BasePlate').val().replace("$", "")) + Number($('#MainContent_Aluminum').val().replace("$", "")) + Number($('#MainContent_Steel').val().replace("$", "")) + Number($('#MainContent_FixturePlank').val().replace("$", "")) + Number($('#MainContent_Wood').val().replace("$", "")) + Number($('#MainContent_Bushings').val().replace("$", "")) + Number($('#MainContent_DrillBlanks').val().replace("$", "")) + Number($('#MainContent_Clamps').val().replace("$", "")) + Number($('#MainContent_Indicator').val().replace("$", "")) + Number($('#MainContent_IndCollar').val().replace("$", "")) + Number($('#MainContent_IndStorCase').val().replace("$", "")) + Number($('#MainContent_ZeroSet').val().replace("$", "")) + Number($('#MainContent_SpcTriggers').val().replace("$", "")) + Number($('#MainContent_TempDrops').val().replace("$", "")) + Number($('#MainContent_HingeDrops').val().replace("$", "")) + Number($('#MainContent_Risers').val().replace("$", "")) + Number($('#MainContent_Handles').val().replace("$", "")) + Number($('#MainContent_JigFeet').val().replace("$", "")) + Number($('#MainContent_ToolingBalls').val().replace("$", "")) + Number($('#MainContent_TBCovers').val().replace("$", "")) + Number($('#MainContent_TBPads').val().replace("$", "")) + Number($('#MainContent_Slides').val().replace("$", "")) + Number($('#MainContent_Magnets').val().replace("$", "")) + Number($('#MainContent_Hardware').val().replace("$", "")) + Number($('#MainContent_LMI').val().replace("$", ""))).toFixed(2));
        }

        function calculateOutsourcing() {
            document.getElementById('MainContent_TotalOutsideServices').value = '$' + String(Number(Number($('#MainContent_Annodizing').val().replace("$", "")) + Number($('#MainContent_BlackOxide').val().replace("$", "")) + Number($('#MainContent_HeatTreat').val().replace("$", "")) + Number($('#MainContent_EngrvdTags').val().replace("$", "")) + Number($('#MainContent_CNCServices').val().replace("$", "")) + Number($('#MainContent_Grinding').val().replace("$", "")) + Number($('#MainContent_ShippingCalc').val().replace("$", "")) + Number($('#MainContent_ThirdPartyCMM').val().replace("$", "")) + Number($('#MainContent_Welding').val().replace("$", "")) + Number($('#MainContent_WireBurn').val().replace("$", ""))).toFixed(2));
        }

        function calculateTotal() {
            document.getElementById('MainContent_Total').value = '$' + String(Number(Number($('#MainContent_TotalLabor').val().replace("$", "")) + Number($('#MainContent_TotalMaterial').val().replace("$", "")) + Number($('#MainContent_TotalOutsideServices').val().replace("$", "")) - Number($('#MainContent_Rebates').val().replace("$", ""))).toFixed(2));
        }

        function applyTotal() {
            $('#MainContent_txtTotalCost').val(String(Number($('#MainContent_Total').val().replace("$", "")).toFixed(2)));
            $('#budgetDialog').dialog('close');
        }

        function openBudget() {
            $('#budgetDialog').dialog({ width: 900, height: 800 });
            $('#budgetDialog').parent().appendTo("form");
        }

        // This is to correctly update the costs when pulling from the batabase
        function keyup(total) {
            $('#MainContent_txtManagement').keyup(); $('#MainContent_txtProjectEng').keyup(); $('#MainContent_txtReadData').keyup(); $('#MainContent_txt3DModel').keyup();
            $('#MainContent_txtDrawings').keyup(); $('#MainContent_txtUpdates').keyup(); $('#MainContent_txtProgramming').keyup(); $('#MainContent_txtCNC').keyup();
            $('#MainContent_txtCertification').keyup(); $('#MainContent_txtGageRR').keyup(); $('#MainContent_txtPartLayouts').keyup(); $('#MainContent_txtBase').keyup();
            $('#MainContent_txtDetails').keyup(); $('#MainContent_txtLocationPins').keyup(); $('#MainContent_txtGoNoGoPins').keyup(); $('#MainContent_txtSPC').keyup();
            $('#MainContent_txtGageRRF').keyup(); $('#MainContent_txtAssemble').keyup(); $('#MainContent_txtPallets').keyup(); $('#MainContent_txtTransportation').keyup();
            $('#MainContent_txtBasePlate').keyup(); $('#MainContent_txtAluminum').keyup(); $('#MainContent_txtSteel').keyup(); $('#MainContent_txtFixturePlank').keyup();
            $('#MainContent_txtWood').keyup(); $('#MainContent_txtBushings').keyup(); $('#MainContent_txtDrillBlanks').keyup(); $('#MainContent_txtClamps').keyup();
            $('#MainContent_txtIndicator').keyup(); $('#MainContent_txtIndCollar').keyup(); $('#MainContent_txtIndStorCase').keyup(); $('#MainContent_txtZeroSet').keyup();
            $('#MainContent_txtSpcTriggers').keyup(); $('#MainContent_txtTempDrops').keyup(); $('#MainContent_txtHingeDrops').keyup(); $('#MainContent_txtRisers').keyup();
            $('#MainContent_txtHandles').keyup(); $('#MainContent_txtJigFeet').keyup(); $('#MainContent_txtToolingBalls').keyup(); $('#MainContent_txtTBCovers').keyup();
            $('#MainContent_txtTBPads').keyup(); $('#MainContent_txtSlides').keyup(); $('#MainContent_txtMagnets').keyup(); $('#MainContent_txtHardware').keyup();
            $('#MainContent_txtLMI').keyup(); $('#MainContent_txtAnnodizing').keyup(); $('#MainContent_txtBlackOxide').keyup(); $('#MainContent_txtHeatTreat').keyup();
            $('#MainContent_txtEngrvdTags').keyup(); $('#MainContent_txtCNCServices').keyup(); $('#MainContent_txtGrinding').keyup(); $('#MainContent_txtShippingCalc').keyup();
            $('#MainContent_txtThirdPartyCMM').keyup(); $('#MainContent_txtWelding').keyup(); $('#MainContent_txtWireBurn').keyup(); $('#MainContent_txtRebates').keyup();
            $('#MainContent_Total').val(total);
        }

        function updateLabel(count, update, cost) {
            //$('#MainContent_' + update).val('$' + Number($(String('#MainContent_' + count)).val()) * cost);
            document.getElementById('MainContent_' + update).value = '$' + (Number($(String('#MainContent_' + count)).val()) * Number($(String('#MainContent_' + cost)).val())).toFixed(2);
            //onkeyup = "sendCode()"
            calculateLabor();
            calculateMaterial();
            calculateOutsourcing();
            calculateTotal();
        }
    </script>

    <asp:Literal runat="server" ID="litKeyUp"></asp:Literal>
</asp:Content>
