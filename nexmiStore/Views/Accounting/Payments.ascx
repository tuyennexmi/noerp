<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('#pagination-Payment').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadPayment(page);
            }
        });
        $('#slPaymentTypes').on('change', function () {
            fnLoadPayment();
        });
        $('#dtFrom').on('change', function () {
            fnLoadPayment();
        });
        $('#dtTo').on('change', function () {
            fnLoadPayment();
        });
    });
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
                        <button onclick="javascript:fnShowPaymentForm('', '', '<%=ViewData["ACCType"] %>', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button onclick="javascript:fnLoadPayment()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right"><%Html.RenderAction("PaginationJS", "UserControl", new { id = "Payment" });%></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td>Loại</td>
                    <td><%Html.RenderAction("slAccountForPayments", "UserControl", new { elementId = "slPaymentTypes", objectName = "Payments", value = ViewData["ACCType"] });%></td>

                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd") %>" style="width: 89px"/></td>

                    <td><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></td>
                    <td>
                        <select id="slStatus" onchange="javascript:fnLoadPayment()">
                            <option value="">Tất cả</option>
                            <option value="<%=NEXMI.NMConstant.PaymentStatuses.Draft%>">Mới</option>
                            <option value="<%=NEXMI.NMConstant.PaymentStatuses.Approved%>">Đã duyệt</option>
                            <option value="<%=NEXMI.NMConstant.PaymentStatuses.Done%>">Đã chi</option>
                            <option value="<%=NEXMI.NMConstant.PaymentStatuses.Cancelled%>">Đã hủy</option>
                        </select>
                    </td>
                </tr>
            </table>
            
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="PaymentGrid">
            <%Html.RenderAction("PaymentGrid", "Accounting", new { type = ViewData["ACCType"] });%>
        </div>
    </div>
</div>