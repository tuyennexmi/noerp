<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div id="divSalesOrder">
    <input type="hidden" id="txtTypeId" value="<%=ViewData["SOType"]%>" />
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordSalesOrder" name="txtKeywordSalesOrder" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionSalesOrder = ViewData["SOFunc"].ToString();
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionSalesOrder, "Insert"))
                        {
                    %>
                            <button onclick="javascript:fnPopupSalesOrderDialog('', '', '<%=ViewData["SOType"]%>')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }%>
                        
                            <button onclick="javascript:fnRefreshSalesOrderList()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>

                    <%if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionSalesOrder))
                      {     
                    %>
                            <button onclick="javascript:fnPrintSOList('<%=ViewData["SOType"]%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                    <%} %>
                    <%if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionSalesOrder))
                      {     
                    %>
                            <button onclick="javascript:fnOrders2Excel('<%=ViewData["SOType"]%>', 'TH')" class="button" title='Bảng Excel tổng hợp'>Excel TH</button>
                            <button onclick="javascript:fnOrders2Excel('<%=ViewData["SOType"]%>', 'CT')" class="button" title='Bảng Excel chi tiêt'>Excel CT</button>
                    <%} %>
                    
                </td>
                <td align="right">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "SOrder" });%>
                    <%Html.RenderPartial("SalesOrderSV"); %>
                </td>
            </tr>
        </table>
    </div>
<script type="text/javascript">
    $(function () {
        $('#pagination-SOrder').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadSOList(page);
            }
        });

        var t;
        $('#txtKeywordSalesOrder').on('keyup', function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadSOList();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnReloadSOList();
        });
    });

    function fnReloadSOList(pageNum) {
        if (pageNum == undefined)
            pageNum = $('#pagination-SOrder input').first().val().split(' /')[0];
        LoadContentDynamic('OrderList', 'Sales/SalesOrderList', {
            pageNum: pageNum,
            status: $('#slStatus').val(),
            keyword: $('#txtKeywordSalesOrder').val(),
            typeId: '<%=ViewData["SOType"] %>',
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        });
    }
    

    function fnRefreshSalesOrderList() {
        $('#txtKeywordSalesOrder').val('');
        fnReloadSOList();
    }

    function fnPrintSOList() {
        var from = $('#dtFrom').val();
        var to = $('#dtTo').val();
        var keyword = $('#txtKeywordSalesOrder').val();
        var typeId = '<%=ViewData["SOType"] %>';
        var status = $('#slStatus').val();
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Sales/SalesOrderList', {
            mode: 'Print',
            status: status,
            from: from,
            to: to,
            keyword: keyword,
            typeId: typeId,
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        }, function (data) {
            $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: '', orient:'LAND' }, function (data2) {
                window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

    function fnChanged() {
        fnReloadSOList();
    }

    function cbbCustomerChanged() {
        fnReloadSOList();
    }
</script>

<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "SalesOrders" }); %>
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="OrderList">
            <%--<%  Html.RenderAction("SalesOrderList", "Sales", new { typeId = ViewData["SOType"] });%>--%>
        </div>
    </div>  
</div>
</div>