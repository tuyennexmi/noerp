<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('#pagination-SInvoice').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadSI(page);
            }
        });

        var t;
        $("#txtKeywordSI").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadSI();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnReloadSI();
        });
    });

    function fnReloadSI(pageNum) {
        if (pageNum == undefined)
            pageNum = $('#pagination-SInvoice input').first().val().split(' /')[0];
        LoadContentDynamic('SIGrid', 'Sales/SalesInvoiceList', {
            pageNum: pageNum,
            status: $('#slStatus').val(),
            keyword: $('#txtKeywordSI').val(),
            from: $('#dtFrom').val(),
            to: $('#dtTo').val(),
            partnerId: document.getElementsByName('cbbCustomers')[0].value
        });
    }

    function fnRefreshSIList() {
        $("#txtSIKeyword").val("");
        fnReloadSI();
    }

    function fnChanged() {
        fnReloadSI();
    }
    function cbbCustomerChanged() {
        fnReloadSI();
    }

</script>

<div id="divSI">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordSI" name="txtKeywordSI" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
            <%string functionSI = NEXMI.NMConstant.Functions.SI;
                //Kiểm tra quyền Insert
                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionSI, "Insert"))
                {%>
                    <button onclick="javascript:fnCreateInvoice('', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% }%>
                    <button onclick="javascript:fnRefreshSIList()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            <%if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.SI))
                { %>                    
                    <button onclick="javascript:fnExport2Excel('TH')" class="button" title='Xuất danh sách tổng hợp ra Excel.'>Excel TH</button>
                    <button onclick="javascript:fnExport2Excel('CT')" class="button" title='Xuất danh sách chi tiết ra Excel.'>Excel CT</button>
            <%} %>
                </td>
                <td align="right" style="float: right;">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "SInvoice" });%>
                    <%Html.RenderPartial("SalesInvoiceSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "SalesInvoices" }); %>
            </div>
        </div>
        <div class='divContentDetails'>
            <div id="SIGrid">
                <%--<%  Html.RenderAction("SalesInvoiceList", "Sales");%>--%>
            </div>
        </div>
    </div>    
</div>

