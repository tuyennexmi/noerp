<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {

        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('#pagination-Receipt').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadReceipt(page);
            }
        });
        $('#slPaymentTypes').on('change', function() {
            fnLoadReceipt();
        });
        $('#dtFrom').on('change', function () {
            fnLoadReceipt();
        });
        $('#dtTo').on('change', function () {
            fnLoadReceipt();
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
                string functionReceipt = NEXMI.NMConstant.Functions.Receipts;
                //Kiểm tra quyền Insert
                if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt, "Insert"))
                {
            %>
                    <button onclick="javascript:fnShowReceiptForm('', '', '<%=ViewData["ACCType"]%>', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
            %>
                <button onclick="javascript:fnLoadReceipt()" class="button"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right"><%Html.RenderAction("PaginationJS", "UserControl", new { id = "Receipt" });%></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table>
                <tr>
                    <td>Loại</td>
                    <td><%Html.RenderAction("slAccountTypes", "UserControl", new { elementId = "slPaymentTypes", objectName = "Receipts", value = ViewData["ACCType"]  });%></td>
                    
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom" value="" style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("yyyy-MM-dd") %>" style="width: 89px"/></td>

                    <td><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></td>
                    <td>
                        <select id="slStatus" onchange="javascript:fnLoadReceipt()">
                            <option value="">Tất cả</option>
                            <option value="<%=NEXMI.NMConstant.ReceiptStatuses.Draft%>">Mới</option>
                            <option value="<%=NEXMI.NMConstant.ReceiptStatuses.Approved%>">Đã duyệt</option>
                            <option value="<%=NEXMI.NMConstant.ReceiptStatuses.Done%>">Đã thu</option>
                            <option value="<%=NEXMI.NMConstant.ReceiptStatuses.Cancelled%>">Đã hủy</option>
                        </select>
                    </td>
                </tr>
            </table>
            
        </div>
    </div>
    <div class='divContentDetails'>
        <div id="ReceiptGrid">
            <%Html.RenderAction("ReceiptGrid", "Accounting", new{ type = ViewData["ACCType"]});%>
        </div>
    </div>
</div>