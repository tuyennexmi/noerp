<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/SalesOrders.js" type="text/javascript"></script>
<div id="divSalesOrder">
    <input type="hidden" id="txtTypeId" value="<%=ViewData["SOType"]%>" />
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordSalesOrder" value='<%=Session["Keyword"] %>' type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
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
                    <button onclick="javascript:fnCreateLeaseOrderDialog('', '', '<%=ViewData["SOType"]%>')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }%>
                        
                    <button onclick="javascript:fnRefreshLeaseOrderList()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>

            <%if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionSalesOrder))
                {     
            %>
                    <button onclick="javascript:fnPrintLOList('<%=ViewData["SOType"]%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
            <%} %>
                </td>
                <td align="right">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "LOrder" });%>
                    <%Html.RenderPartial("SalesOrderSV"); %>
                </td>
            </tr>
        </table>
    </div>
<script type="text/javascript">
    $(function () {
        $('#pagination-LOrder').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnRefreshLeaseOrderList(page);
            }
        });

        var t;
        $('#txtKeywordSalesOrder').on('keyup', function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnRefreshLeaseOrderList();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnRefreshLeaseOrderList();
        });

    });
    
    function fnChanged() {
        fnRefreshLeaseOrderList();
    }

    function cbbCustomerChanged() {
        fnRefreshLeaseOrderList();
    }
</script>

<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "LeaseOrders" }); %>            
        </div>
    </div>
    <div class='divContentDetails'>
    <div id="OrderList">
        <%--<%  Html.RenderAction("LeaseOrderList", "Sales", new { typeId = ViewData["SOType"], keyword = Session["Keyword"] });%>--%>
    </div>
    </div>        
    </div>
</div>