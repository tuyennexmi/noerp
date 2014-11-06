<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/CashBalances.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function tabletoExcel(table, name, filename) {
        var uri = 'data:application/vnd.ms-excel;base64,'
          , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--></head><body><table>{table}</table></body></html>'
          , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))); }
          , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }); };
        if (!table.nodeType) table = document.getElementById(table);
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML };
        document.getElementById("dlink").href = uri + base64(format(template, ctx));
        document.getElementById("dlink").download = filename;
        document.getElementById("dlink").click();

    }

    function cbbCustomerChanged() {
        fnLoadContent();
    }

    function fnPrintCB() {
        var partnerId = document.getElementsByName('cbbCustomers')[0].value;
        var payMethod = $('#slPaymentMethods').val();
        var bankAcc = $('#slBankAccounts').val();
        var from = $('#dtFrom').val();
        var to = $('#dtTo').val();
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Accounting/CashBalancesUC', {
            payMethod: payMethod,
            bankAcc: bankAcc,
            partnerId: partnerId,
            from: from,
            to: to,
            mode: 'Print'
        }, function (data) {
            $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: '', orient: 'LAND' }, function (data2) {
                window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }
</script>

<%
    string from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01).ToString("yyyy-MM-dd");
    %>
<a id="dlink"  style="display:none;"></a>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
                <button onclick="javascript:fnLoadContent()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>                
                <button onclick="javascript:fnPrintCB()" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                <button onclick="javascript:tabletoExcel('tbCashBalances', 'CĐTC', 'Bảng cân đối thu chi.xls')" class="button">Xuất ra Excel</button>
                
                <%--<button onclick="javascript:()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>--%>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">            
            <table>
                <tr>
                    <td><%Html.RenderAction("slPaymentMethods", "UserControl", new { paymentMethod = ViewData["PaymentMethodId"] });%></td>
                    <td><%Html.RenderAction("slBankAccounts", "UserControl", new { bankAccount = ViewData["bankAccount"] });%></td>
                    <%--<td>Chọn khách hàng:</td>--%>
                    <td> <%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = "", customerName = "" });%></td>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input style="width: 160px" type="text"  id="dtFrom" value="<%=from %>"/></td>
                    <td><%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 160px" /></td>
                </tr>
            </table>            
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="CashBalancesUC"></div>
    </div>
</div>