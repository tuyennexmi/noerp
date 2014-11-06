<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('#pagination-Import').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReloadImport(page, '');
            }
        });
        var t;
        $("#txtKeywordImport").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadImport();
            }, 1000);
        });

        $("#dtFrom, #dtTo").change(function () {
            fnReloadImport();
        });
    });

    function fnChanged() {
        fnReloadImport();
    }

    function cbbCustomerChanged() {
        fnReloadImport();
    }

</script>


<div id="divImport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordImport" name="txtKeywordImport" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
            <%
                string functionImport = NEXMI.NMConstant.Functions.Import;
                //Kiểm tra quyền Insert
                if(GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport, "Insert"))
                {
            %>
                        <button onclick="javascript:fnPopupImportDialog('', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
            %>
                        <button class="button" onclick="javascript:fnRefreshImport()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            <%
                if (GetPermissions.GetExport((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                {
            %>
                        <button onclick="javascript:fnImports2Excel('TH')" class="button" title='Xuất bảng Tổng hợp ra Excel'>Excel TH</button>
                        <button onclick="javascript:fnImports2Excel('CT')" class="button" title='Xuất bảng Chi tiết ra Excel'>Excel CT</button>
            <%
                }
            %>
                </td>
                <td align="right" style="float: right;">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Import" });%>
                    <%Html.RenderPartial("ImportSV");%>
                </td>
            </tr>
        </table>
    </div>

    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <% Html.RenderAction("FilterUC", "UserControl", new { objectName = "Imports" }); %>
            </div>
        </div>

        <div class='divContentDetails'>
            <div id="ImportGrid">
            <%--<%  Html.RenderAction("ImportList", "Stocks");%>--%>
            </div>
        </div>
    </div>
</div>
