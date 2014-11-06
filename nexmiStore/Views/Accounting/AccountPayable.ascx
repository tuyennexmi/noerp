<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<script type="text/javascript">

    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnLoadAPDetail(supplierId) {
        openWindow('Chi tiết công nợ của nhà cung cấp', 'Accounting/APDetails', {
            supplierId: supplierId, 
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        }, 900, 500);
    }

    function fnReloadAPData() {
        LoadContentDynamic('AP-Containner', 'Accounting/AccountPayableUC', {
            from: $('#dtFrom').val(),
            to: $('#dtTo').val()
        });
    }

    function fnCreatePaymentRequire(supplierId) {
        $.get(appPath + 'Accounting/APDetails', { 
                supplierId: supplierId,
                from: $('#dtFrom').val(),
                to: $('#dtTo').val()
            }, function (data) {
            openWindow('Đề nghị thanh toán công nợ của nhà cung cấp', 'UserControl/SendEmailForm', { id: supplierId, type: 'PAYREQ', html: Base64.encode(data) }, 900, 500);
        });
    }

    function fnPrint() {
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Accounting/AccountPayableUC', { 
                mode: 'Print',
                from: $('#dtFrom').val(),
                to: $('#dtTo').val()
            }, function (data) {
                $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'ORD', orient: 'LAND' }, function (data2) {
                    window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

    function fnPrintAPDetails(supplierId) {
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Accounting/APDetails', {
                supplierId: supplierId, 
                mode: 'Print',
                from: $('#dtFrom').val(),
                to: $('#dtTo').val()
            }, 
            function (data) {
                $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: 'ORD' }, function (data2) {
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
                    <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                <%                         
                    if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.AccountPayable))
                    {    
                %>
                        <button class="button" onclick="javascript:fnPrint()"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                <% 
                    }
                %>
                        <button class="button" onclick="javascript:fnReloadAPData()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
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
                        <%--<td><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>:</td>
                        <td> <%Html.RenderAction("cbbAreas", "UserControl", new { areaId = "", areaName = "", elementId = "", holderText = "Chọn khu vực" });%></td>--%>
                        <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input style="width: 160px" type="text"  id="dtFrom" value="<%=from %>"/></td>
                        <td><%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd")%>" style="width: 160px" /></td>
                    </tr>
                </table>
            </div>
        </div>
        <div class='divContentDetails'>
        <div id="AP-Containner">
            <%Html.RenderAction("AccountPayableUC", new { from = from }); %>
        </div>
        </div>
    </div>
</div>