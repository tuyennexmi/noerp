<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseOrders.js" type="text/javascript"></script>

<script type="text/javascript">

    $(function () {
        $('#pagination-POrder').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadPOList(page);
            }
        });

        var t;
        $('#txtKeyword').on('keyup', function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadPOList();
            }, 1000);
        });
        $("#dtFrom, #dtTo").change(function () {
            fnReloadPOList();
        });
    });

    function fnRefreshPOList() {
        $('#txtKeyword').val('');
        fnReloadPOList();
    }

    function fnReloadPOList(pageNum) {
        if (pageNum == undefined)
            pageNum = $('#pagination-POrder input').first().val().split(' /')[0];
        LoadContentDynamic('OrderList', 'Purchase/PurchaseOrdersList', {
            pageNum: pageNum,
            status: $('#slStatus').val(),
            keyword: $('#txtKeyword').val(),
            typeId: '<%=ViewData["TypeId"] %>',
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        });
    }

    function fnPrintPOList() {
        var from = $('#dtFrom').val();
        var to = $('#dtTo').val();
        var keyword = $('#txtKeyword').val();
        var typeId = '<%=ViewData["TypeId"] %>';
        $.ajaxSetup({ async: false });

        $.get(appPath + 'Purchase/PurchaseOrdersList', {
            mode: 'Print',
            from: from,
            to: to,
            keyword: keyword,
            typeId: typeId,
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        }, function (data) {
            $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode(data), mode: '', orient: 'LAND' }, function (data2) {
                window.open(appPath + 'UserControl/PrintPage');
            });
        });

        $.ajaxSetup({ async: true });
    }

    function fnChanged() {
        fnReloadPOList();
    }

    function cbbCustomerChanged() {
        fnReloadPOList();
    }

</script>
<div>
    <input type="hidden" id="txtTypeId" value="<%=ViewData["TypeId"]%>" />
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
                    <%
                        string functionId = ViewData["FunctionId"].ToString();
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                        {
                    %>
                            <button onclick='javascript:fnShowPOForm("", "", "<%=ViewData["TypeId"]%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                    %>
                        <button onclick="javascript:fnRefreshPOList()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    <%
                        if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                        {
                    %>
                            <button onclick="javascript:fnPrintPOList()" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                    <%  } 
                    %>
                    <%
                        if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                        { 
                    %>
                            <button onclick="javascript:fnPOrders2Excel('<%=ViewData["TypeId"]%>', 'TH')" class="button" title='Bảng Excel tổng hợp'>Excel TH</button>
                            <button onclick="javascript:fnPOrders2Excel('<%=ViewData["TypeId"]%>', 'CT')" class="button" title='Bảng Excel chi tiết'>Excel CT</button>
                    <%
                        }
                    %>
                    
                </td>
                <td align="right">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "POrder" });%>
                    <%Html.RenderPartial("POSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "PurchaseOrders" }); %>
            </div>
        </div>

        <div class='divContentDetails'>
            <div id="OrderList">
                <%--<%  Html.RenderAction("PurchaseOrdersList", "Purchase", new { typeId = ViewData["TypeId"] });%>--%>
            </div>
        </div>
    </div>
</div>