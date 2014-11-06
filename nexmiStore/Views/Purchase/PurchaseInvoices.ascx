<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseInvoices.js" type="text/javascript"></script>
<script src="<%=Url.Content("~")%>Scripts/forApp/Payments.js" type="text/javascript"></script>
<script src="<%=Url.Content("~")%>Scripts/forApp/Suppliers.js" type="text/javascript"></script>

<script type="text/javascript">
    $(function () {
        $('#pagination-Invoice').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadPIs(page);
            }
        });

        var t;
        $("#txtKeywordPI").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnLoadPIs();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnLoadPIs();
        });
    });

    function fnChanged() {
        fnLoadPIs();
    }

    function cbbCustomerChanged() {
        fnLoadPIs();
    }
</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
            </td><td></td>
            <td align="right">
                <input id="txtKeywordPI" name="txtKeywordPI" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
            </td>
        </tr>
        <tr>
            <td>
        <%
            string functionId = NEXMI.NMConstant.Functions.PI;
            //Kiểm tra quyền Insert
            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
        %>
                <button onclick="javascript:fnCreatePI('', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
            }
            %>
            <button onclick="javascript:fnLoadPIs()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            <%
            if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.PI))
            { %>                    
                <button onclick="javascript:fnExportPI2Excel('TH')" class="button" title='Xuất danh sách tổng hợp ra Excel.'>Excel TH</button>
                <button onclick="javascript:fnExportPI2Excel('CT')" class="button" title='Xuất danh sách chi tiết ra Excel.'>Excel CT</button>
        <%  } %>
                
            </td>
                
            <td align="right"></td>
                
            <td align="right" style="float: right;">
                <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Invoice" });%>
                <%Html.RenderPartial("PurchaseInvoiceSV");%>
            </td>
        </tr>
    </table>
</div>

<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "PurchaseInvoices" }); %>
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="PIGrid">
            <%--<%Html.RenderAction("PurchaseInvoiceList", "Purchase");%>--%>
        </div>
    </div>
</div>