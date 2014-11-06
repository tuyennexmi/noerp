<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" border=".1">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("ID", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("SETUP_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("MAINTAIN_DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            
            <th><%= NEXMI.NMCommon.GetInterface("SETUP_FEE", langId)%></th>
            <th><%= NEXMI.NMCommon.GetInterface("ADVANCE", langId)%></th>
            <th><%= NEXMI.NMCommon.GetInterface("LEASE_FEE", langId)%></th>
            
            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
    <%  if (ViewData["Mode"].ToString() != "Print")
        {%>
            <th>Th báo</th>
            <th></th>
    <%  } %>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMSalesOrdersWSI> WSIs = (List<NEXMI.NMSalesOrdersWSI>)ViewData["WSIs"];
        int totalPage = 1;
        double amount = 0, total = 0;
        string func = NEXMI.NMConstant.Functions.LeaseOrder;
        if (ViewData["SOType"].ToString() == NEXMI.NMConstant.SOType.Quotation)
        {                
            func = NEXMI.NMConstant.Functions.Quotation;
        }
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            int count = 1;
            foreach (NEXMI.NMSalesOrdersWSI Item in WSIs.OrderBy(i=>i.Order.MaintainDate))
            {
                amount = Item.Order.OrderDetailsList.Sum(i=>i.TotalAmount);
                total += amount;
    %>
            <tr ondblclick="javascript:fnLoadLeaseOrderDetail('<%=Item.Order.OrderId%>')" id="<%=Item.Order.OrderId%>">
                <td width="3%"><%=count++%></td>
                <td width="4%"><%=Item.Order.Reference%> </td>
                <td ><%=Item.Order.OrderDate.ToString("dd-MM-yyyy")%></td>
                <td >
                <%
                if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                {
                    %>
                    <input value='<%=(Item.Order.MaintainDate != null)? Item.Order.MaintainDate.Value.ToString("yyyy-MM-dd") : ""%>' type="text" class="maintainDate" id="txtMD<%=Item.Order.OrderId%>" style="width: 89px" />
                <%}
                else
                {
                    %>
                    <%=Item.Order.MaintainDate.Value.ToString("dd-MM-yyyy") %>
                <%}
                    %>
                </td>
                <td width="28%">
                <%  if (ViewData["Mode"].ToString() != "Print")
                    {%>
                    <a href="javascript:fnPopupCustomerDetail('<%=Item.Customer.CustomerId%>','POPUP')">
                        [<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%>
                    </a>
                    <%}
                    else
                    {  %>
                        [<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%>
                    <%} %>
                </td>
                
                <td ><%=Item.Order.SetupFee%></td>
                <td ><%=Item.Order.Advances%></td>
                <td align="right" ><%= amount.ToString("N3")%></td>

                <td ><%=Item.Order.Description%></td>
                <td ><%=NEXMI.NMCommon.GetStatusName(Item.Order.OrderStatus, langId)%></td>
        <%  if (ViewData["Mode"].ToString() != "Print")
            {%>
                <td>
            <%if (Item.Invoices.Count > 0)
                  if (string.IsNullOrEmpty(Item.Invoices[Item.Invoices.Count - 1].SourceDocument))
                  { %>
                    <img alt="Xem chi tiết" title="Chưa xuất hóa đơn!" src="<%=Url.Content("~")%>Content/UI_Images/16invoice.png" />
                <%} %>
                <%
              if ((DateTime.Today.Month - Item.Order.OrderDate.Month) == 3 |
                  (DateTime.Today.Month - Item.Order.OrderDate.Month) == 6 |
                  (DateTime.Today.Month - Item.Order.OrderDate.Month) == 8 |
                  (DateTime.Today.Month - Item.Order.OrderDate.Month) == 12)
              {
                %>
                    <a href="javascript:fnCreateExportSparePartsfromSO('<%=Item.Order.OrderId%>')"><img alt="Xuất kho lọc thay thế" title="Xuất kho lọc thay thế" src="<%=Url.Content("~")%>Content/UI_Images/16config.png" /></a>
                <%} %>
                </td>
                <td >
                    <a href="javascript:fnLoadLeaseOrderDetail('<%=Item.Order.OrderId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                    
                    <a href="javascript:fnCreateInvoice('', '<%=Item.Order.OrderId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/billing-icon.png" /></a>
                    
                <%
              if (Item.Order.OrderStatus == NEXMI.NMConstant.LOStatuses.Draft | Item.Order.OrderStatus == NEXMI.NMConstant.LOStatuses.Order)
              {
                  if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                  {                        
                %>
                        <a href="javascript:fnPopupLeaseOrderDialog('<%=Item.Order.OrderId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> đơn hàng" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                <%  }
                  if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                  {
                %>
                        <a href="javascript:fnDeleteLeaseOrder('<%=Item.Order.OrderId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                <% 
                  }
              }
                %>
                </td>
                <%} %>
            </tr>
    <%
            }
        }
    %>
            <tr>
                <td colspan='7'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                <td align="right"><%=total.ToString("N3") %></td>
            </tr>
    </tbody>
</table>

<%  if (ViewData["Mode"].ToString() != "Print")
    {%>
<script type="text/javascript">
    $(function () {
        $('#pagination-LOrder').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
        $('.maintainDate').datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('.maintainDate').on('change', function () {
            var answer = confirm('Bạn có chắc là muốn thay đổi ngày vệ sinh của khách hàng này?');
            if (answer) {
                var elm = $(this);
                var partnerId = elm.parent().parent().attr('id');
                var amount = $('#txtMD' + partnerId).val();
                
                fnSaveMaintainDateChange(partnerId, amount);
            }
        });
    });

    function fnCreateExportSparePartsfromSO(orderId) {
        LoadContent('', 'Stocks/ExportPartsFromLO?orderId=' + orderId);        
    }

    function fnSaveMaintainDateChange(Id, mdate) {
        $.ajax({
            type: 'POST',
            url: appPath + 'Sales/SaveMaintainDateChange',
            data: {
                orderId: Id,
                maintainDate: mdate
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data == 'True') {
                    $(window).hashchange();
                }
                else {
                    alert("Không thể cập nhật được, vui lòng thử lại!");
                }
            },
            error: function () {
                fnError();
            },
            complete: function () {
                fnComplete();
            }
        });
    }
    
</script>

<%  } %>