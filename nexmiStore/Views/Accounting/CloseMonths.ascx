<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('#pagination-CloseMonth').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnReLoadCloseMonth(page);
            }
        });
        $('#slPaymentTypes').on('change', function () {
            fnReLoadCloseMonth();
        });
        $('#dtFrom').on('change', function () {
            fnReLoadCloseMonth();
        });
        $('#dtTo').on('change', function () {
            fnReLoadCloseMonth();
        });
    });

    function fnShowCloseMonthForm() {
        LoadContent('', 'Accounting/CloseMonthForm');
    }

    function fnShowCloseMonthDetail(id) {
        LoadContent('', 'Accounting/CloseMonthDetails?closeMonth=' + id );
    }

    function fnReLoadCloseMonth() {
        LoadContentDynamic('CloseMonthGrid', 'Accounting/CloseMonthGrid');
    }
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionPayment = NEXMI.NMConstant.Functions.Payments;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionPayment))
                    {
                %>
                        <button onclick="javascript:fnShowCloseMonthForm()" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button onclick="javascript:fnReLoadCloseMonth()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right"><%Html.RenderAction("PaginationJS", "UserControl", new { id = "CloseMonth" });%></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd") %>" style="width: 89px"/></td>
                </tr>
            </table>
            
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="CloseMonthGrid">
            <%Html.RenderAction("CloseMonthGrid", "Accounting");%>
        </div>
    </div>
</div>