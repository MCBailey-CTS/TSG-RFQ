<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditFilter.aspx.cs" Inherits="RFQ.EditFilter" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
.chooseColumns {
	border: 1px solid black;
	color: white;
	background-color: #328fcd;
	text-align: left;
	padding: 4px;
	float: left;
	list-style: none;
}
.reportColumns {
	border: 1px solid black;
	color: black;
	background-color: #f0f0f0;
	text-align: left;
	padding: 4px;
	float: left;
}
</style>
<div style="min-height: 100px"></div>
<div align="center">
    <div style="float: left; border: 1px solid black; padding: 10px; margin: 10px; ">
        <label for="txtFilterName" class="ui-widget">Filter Name</label><br />
        <asp:TextBox runat="server" ID="txtFilterName" CssClass="ui-widget-content"></asp:TextBox>
        <br /><br />
        <label for="rbAppliesTo" class="ui-widget">Applies To</label><br />
        <asp:RadioButtonList ID="rbAppliesTo" runat="server">
            <asp:ListItem Value="0" Text="Everyone" Selected></asp:ListItem>
        </asp:RadioButtonList>
        <br /><br />
        <div class="mybutton" onclick="saveFilter();">Save</div><br /><br /><br />
        <div class="mybutton danger" onclick="self.close();">Cancel</div>
        <div class="mybutton btn-danger" onclick="deleteFilter()">Delete This Filter</div>
    </div>
    <div style="float: left; border: 1px solid black; padding: 10px; margin: 10px;">
            <h4>Filter Criteria</h4>
            <label for="ddlMatchAll" class="ui-widget">Matching</label><br />
            <asp:DropDownList ID="ddlMatchAll" runat="server" CssClass="ui-widget-content">
                <asp:ListItem Value="0" Text="Any Can Match"></asp:ListItem>
                <asp:ListItem Value="1" Text="All Must Match"></asp:ListItem>
            </asp:DropDownList>
        <br /><br>
            <asp:Literal ID="litCriteria" runat="server"></asp:Literal>
    </div>            
    <div style="clear: both;">&nbsp;</div>
    <div style="border: 1px solid black; padding: 10px; margin: 10px; min-width: 90%">
                <label for="ddlColumns" class="ui-widget">Columns</label><br />
        <asp:DropDownList ID="ddlColumns" runat="server" CssClass="ui-widget-content"  ></asp:DropDownList>
        <br /><br />
        Select a column from the dropdown to add to the list. Drag to change the order of the columns. 
        Click <font color="red">X</font> to remove from list.
        <hr />
            <h4>Filter Columns</h4>
            <asp:Literal ID="litReportColumns" runat="server"></asp:Literal>
    </div>
</div>
    <div style="clear: both;">&nbsp;</div>


    <script type="text/javascript">
        var filterID = 0;
        function page_init()
        {
            $('#MainContent_ddlColumns').change(function() {selectThis(this.value);});
            $('html,body').scrollTop(0);
            $("#reportColumnList").disableSelection();
            $("#reportColumnList").sortable();
            var $_GET = {};

            document.location.search.replace(/\??(?:([^=]+)=([^&]*)&?)/g, function () {
                function decode(s) {
                    return decodeURIComponent(s.split("+").join(" "));
                }

                $_GET[decode(arguments[1])] = decode(arguments[2]);
            });
            filterID = $_GET['id'];
        }
        function selectThis(columnID) {
            if (columnID != '') {
                objid = 'rpt' + columnID;
                columnTitle = $("#MainContent_ddlColumns option:selected").text();
                // add to the list of columns ordered and allow to be sorted.
                newli = "<li class='chooseColumns' id='" + objid + "' >" + columnTitle + "<sup><div onClick=\"removeThis('" + objid + "');\" style='cursor: pointer; display: inline; z-index: 500; color: red;' alt='Click to Delete'>X</div></sup>" + "</li>";
                $('#reportColumnList').append(newli);
                $('#reportColumnList').show();
                $('html,body').scrollTop(0);
            }
        }
        function removeThis(columnTitle) {
            // remove from the list of columns ordered
            $('#' + columnTitle).remove();
            $('html,body').scrollTop(0);
        }
        var currentCondition = 0;
        function setValueType(idx) {
            currentCondition = idx;
            crit = $('#crit' + idx).val();
            cndt = $('#condition' + idx).val();
            opr = $('#operation' + idx).val();
            url = 'SetValueType.aspx?idx=' + idx + '&rand=' + Math.random() + '&crit=' + crit + '&cndt=' + cndt + '&op=' + opr;
            $.ajax({ url: url, success: function (data) { $('#conditionWrapper' + currentCondition).html(data); } });
        }
        function deleteCondition(idx) {
            $('#crit' + idx).val('');
            $('#condition' + idx).val('')
            $('#operation' + idx).val('');
        }

        function deleteFilter() {
            if (confirm('Really Delete this Filter?')) {
                url = 'DeleteFilter?filter=' + filterID;
                $.ajax({ url: url, success: function (data) { self.close(); } });
            }

        }

        function saveFilter() {
            var url = "UpdateFilter.aspx"
            var formdata = "filter=" + filterID;
            formdata = formdata + '&name=' + $('#MainContent_txtFilterName').val();
            uid = 0;
            if (document.getElementById('MainContent_rbAppliesTo_1').checked) {
                uid = document.getElementById('MainContent_rbAppliesTo_1').value;
            }
            formdata = formdata + '&uid=' + uid;
            ccount = 0;
            var endoflist = 0;
            while (!endoflist) {
                ccount++;
                if (document.getElementById('crit' + ccount)) {
                    formdata = formdata + '&crit' + ccount + '=' + $('#crit' + ccount).val();
                    formdata = formdata + '&op' + ccount + '=' + $('#operation' + ccount).val();
                    formdata = formdata + '&cond' + ccount + '=' + $('#condition' + ccount).val();
                } else {
                    endoflist = 1;
                    ccount--;
                }
            }
            formdata = formdata + '&ccount=' + ccount;
            formdata = formdata + '&anyall=' + $('#MainContent_ddlMatchAll').val();
            formdata = formdata + '&fields=';
            comma = '';
            $('#reportColumnList li').each(
                function (index) {
                    formdata = formdata + comma;
                    ttl = $(this).attr('id');
                    formdata = formdata + ttl;
                    comma = ',';
                }
            );
            url = url + '?' + formdata;
            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    alert('Filter Updated');
                    self.close();
                }
            });
        }
    </script>
</asp:Content>
