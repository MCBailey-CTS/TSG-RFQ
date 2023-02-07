<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="RFQ.Admin" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div align="center" style="position: relative; top: 100px;">
       <asp:Label ID="lblTasks" runat="server"></asp:Label>
        <br />
        <br />
        <div id="divCustomerResponsibility" class="ui-widget mybutton" onclick="responsibility();" >Download Customer Responsibility</div>
        <div id="divCompetitor" class="ui-widget mybutton" onclick="competitor();">Download Competitor List</div>
        <div id="divRFQ" class="ui-widget mybutton" onclick="rfqUpdate();">Download RFQ Update</div>
        <div class="mybutton" onclick="uploadQuote();" id="responsibilityUpload">Upload responsibility or competitor list</div>

        <asp:FileUpload ID="uploadFile" runat="server" AllowMultiple="true" style="opacity: 0; visibility: hidden;" />


    </div>

    <script>
        function supportAjaxUploadWithProgress() {
            return supportFileAPI() && supportAjaxUploadProgressEvents() && supportFormData();

            function supportFileAPI() {
                var fi = document.createElement('INPUT');
                fi.type = 'file';
                return 'files' in fi;
            };

            function supportAjaxUploadProgressEvents() {
                var xhr = new XMLHttpRequest();
                return !!(xhr && ('upload' in xhr) && ('onprogress' in xhr.upload));
            };

            function supportFormData() {
                return !!window.FormData;
            }
        }

        function page_init() {
            $("#MainContent_uploadFile").fileupload({
                url: 'UploadCustomerResponsibility.ashx?&rand=' + Math.random(),
                add: function (e, data) {
                    data.submit();
                },
                success: function (response, status) {
                    if (response.substring(0, 2) == 'OK') {
                        // Good Response is OK|PartID|quotinghtml
                        //responseParts = response.split('|');
                        //document.getElementById('quoting' + responseParts[1]).innerHTML = responseParts[2];
                    } else {
                        alert(response);
                    }
                },
                error: function (error) {
                    // this error means the page actually errored out and you need to figure out what the error was
                    alert('Error Accessing Responsibility List');
                }
            });
        }

        function responsibility() {
            window.open('CustomerResponsibilityList.ashx?customer=Customer');
        }

        function competitor() {
            window.open('CustomerResponsibilityList.ashx?customer=Competitor');
        }

        function rfqUpdate() {
            window.open('CustomerResponsibilityList.ashx?customer=RFQ')
        }

        function uploadQuote() {
            $('#MainContent_uploadFile').click();
        }
    </script>

    <script src="blueimp/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="blueimp/js/jquery.fileupload.js" type="text/javascript"></script>

</asp:Content>
