<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbReceipts" class="tbDetails" width="100%">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PARTNER_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PARTNER_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
            <th></th>
        </tr>
    </thead>
    <tbody>
    <%
        List<NEXMI.NMReceiptsWSI> WSIs = (List<NEXMI.NMReceiptsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            string functionReceipt = NEXMI.NMConstant.Functions.Receipts;
            string customerId = "", customerName = "", strStatus = "";
            bool receiptStatus = false;
            int i = 1;
            double total = 0;            
            foreach (NEXMI.NMReceiptsWSI Item in WSIs)
            {
                customerId = ""; customerName = "";
                if (Item.Customer != null)
                {
                    customerId = Item.Customer.CustomerId; 
                    customerName = Item.Customer.CompanyNameInVietnamese;
                }
                total += Item.Receipt.ReceiptAmount;
    %>
        <tr ondblclick="javascript:fnLoadReceiptDetail('<%=Item.Receipt.ReceiptId%>')">
            <td style="width: 15px;"><%=i++%></td>
            <td style="width: 120px;"><div><%=Item.Receipt.ReceiptId%></div></td>
            <td style="width: 100px;"><div><%=Item.Receipt.ReceiptDate.ToString("dd-MM-yyyy")%></div></td>
            <td style="width: 120px;"><div><%=customerId%></div></td>
            <td><div title="<%=customerName%>"><%=customerName%></div></td>
            <td style="width: 120px;" align="right"><div><%=(Item.Receipt.ReceiptAmount).ToString("N3")%></div></td>
            <td><div title="<%=Item.Receipt.DescriptionInVietnamese%>"><%=Item.Receipt.DescriptionInVietnamese%></div></td>
            <td><div><%=NEXMI.NMCommon.GetStatusName(Item.Receipt.ReceiptStatus, langId)%></div></td>
            <td class="actionCols">
                <a href="javascript:fnLoadReceiptDetail('<%=Item.Receipt.ReceiptId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <% 
                if (Item.Receipt.ReceiptStatus == NEXMI.NMConstant.ReceiptStatuses.Draft)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                    {    
    %>
                <a href="javascript:fnShowReceiptForm('<%=Item.Receipt.ReceiptId%>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
    <% 
                    }
    %>
    <% 
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                    {
    %>
                <a href="javascript:fnDeleteReceipt('<%=Item.Receipt.ReceiptId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
    <% 
                    }
                }
    %>
            &nbsp;</td>
        </tr>
    <%
            }
    %>
        <tr>
            <td colspan='5'><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
            <td align="right"><%=total.ToString("N3") %></td>
        </tr>
    <% 
        }
        else
        { 
    %>
        <tr><td colspan="9" align="center"><h3>Không có dữ liệu.</h3></td></tr>
    <%
        }
    %>
    </tbody>    
</table>
<script type="text/javascript">
    $(function () {
        $('#pagination-Receipt').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>