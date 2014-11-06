<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('#cbbAreas').on('change', function () {
            var area = document.getElementsByName('cbbAreas')[0].value;
            LoadContentDynamic('AR-Containner', 'Accounting/AccountReceivableUC', { 
                area: area,
                from: $('#dtFrom').val(),
                to: $('#dtTo').val()
            });
        });
    });

    function fnReloadARData() {
        var area = document.getElementsByName('cbbAreas')[0].value;
        LoadContentDynamic('AR-Containner', 'Accounting/AccountReceivableUC', { 
            area: area ,
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        });
    }

    function fnLoadARDetail(customerId) {
        openWindow('Chi tiết công nợ của khách hàng', 'Accounting/ARDetails', { 
            customerId: customerId,
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        }, 900, 500);
    }

    function fnLoadARCompare(customerId) {
        var from = $('#dtFrom').val();
        var to = $('#dtTo').val();
        LoadContent('', 'Accounting/ARDetails?customerId=' + customerId + '&from=' + from + '&to=' + to + '&mode=detail');
    }

    function fnCreatePaymentReqrire(customerId) {
        $.ajaxSetup({ async: false });
        $.get(appPath + 'Accounting/PaymentRequire', { 
                customerId: customerId,
                from: $('#dtFrom').val(),
                to: $('#dtTo').val()
            }, function (data) {
                $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'PAYREQ' }, function (data2) {
                    window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

    function fnSendARCompare2Email(customerId) {
        $.get(appPath + 'Accounting/ARDetails', {
            customerId: customerId, 
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            mode: 'detail' 
            }, function (data) {
                openWindow('Đối chiếu công nợ khách hàng', 'UserControl/SendEmailForm', { id: customerId, type: 'ARCMP', html: Base64.encode(data) }, 900, 500);
        });
    }

    function fnPrintARCompare2Email(customerId) {
        $.ajaxSetup({ async: false });
        $.get(appPath + 'Accounting/ARDetails', {
            customerId: customerId, 
            mode: 'detail',
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
            }, function (data) {
                $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'PAYREQ', orient: 'LAND' }, function (data2) {
                    window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

    function fnAR2Excel(customerId) {
        var from = $('#dtFrom').val();
        var to = $('#dtTo').val();
        $.download(appPath + 'Common/AR2Excel', 'customerId=' + customerId + 'from=' + from + 'to=' + to);
    }

    function fnPrint() {
        var area = document.getElementsByName('cbbAreas')[0].value;
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Accounting/AccountReceivableUC', {
            area: area, 
            mode: 'Print',
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
            }, function (data) {
                $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'PAYREQ', orient: 'LAND' }, function (data2) {
                    window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

</script>

<%
    string from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01).ToString("yyyy-MM-dd");
    %>

<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <%--<input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />--%>
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">                        
                <%
                    if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.AccountReceivable))
                    {    
                %>
                        <button class="button" onclick="javascript:fnPrint()"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                <% 
                    }
                %>
                        <button class="button" onclick="javascript:fnReloadARData()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    </div>
                </td>
                <td align="right">
                        
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <table>
                    <tr>
                        <td><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>:</td>
                        <td> <%Html.RenderAction("cbbAreas", "UserControl", new { areaId = "", areaName = "", elementId = "", holderText = NEXMI.NMCommon.GetInterface("SELECT_AREA", langId) });%></td>
                        <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input style="width: 160px" type="text"  id="dtFrom" value="<%=from %>"/></td>
                        <td><%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 160px" /></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class='divContentDetails'>
        <div id="AR-Containner">
            <%Html.RenderAction("AccountReceivableUC", new { from = from }); %>
        </div>
        </div>
    </div>
</div>