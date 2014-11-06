<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>\

<script type="text/javascript">
    $(function () {
        $('#pagination-Export').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadExport(page, '');
            }
        });
        var t;
        $("#txtKeywordExport").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadExport();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnReloadExport();
        });
    });

    function fnChanged() {
        fnReloadExport();
    }

    function cbbCustomerChanged() {
        fnReloadExport();
    }

</script>
<div id="divExport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordExport" name="txtKeywordExport" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
            <%
                string functionExport = NEXMI.NMConstant.Functions.Export;
                //Kiểm tra quyền Insert
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionExport))
                {
            %>
                    <button onclick="javascript:fnExportProduct('', '', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
            %>
                    <button class="button" onclick="javascript:fnRefreshExport()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            <% 
                if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionExport))
                {    
            %>
                    <button onclick="javascript:fnExports2Excel('TH')" class="button" title='Xuất bảng Tổng hợp ra Excel'>Excel TH</button>
                    <button onclick="javascript:fnExports2Excel('CT')" class="button" title='Xuất bảng Chi tiết ra Excel'>Excel CT</button>
            <% 
                }    
            %>
                </td>
                <td align="right" style="float: right;">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Export" });%>
                    <%Html.RenderPartial("ExportSV");%>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">            
                <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "Exports" }); %>
            </div>
        </div>

        <div class='divContentDetails'>
            <div id="ExportGrid">
                <%--<%  Html.RenderAction("ExportList", "Stocks");%>--%>
            </div>
        </div>
    </div>
</div>