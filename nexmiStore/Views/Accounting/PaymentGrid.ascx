<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<table id="tbPayments" class="tbDetails" width="100%">
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
        string functionId = NEXMI.NMConstant.Functions.Payments;
        List<NEXMI.NMPaymentsWSI> WSIs = (List<NEXMI.NMPaymentsWSI>)ViewData["WSIs"];
        int totalPage = 1;
        if (WSIs.Count > 0)
        {
            int totalRows = WSIs[0].TotalRows;
            if (totalRows % NEXMI.NMCommon.PageSize() == 0)
                totalPage = totalRows / NEXMI.NMCommon.PageSize();
            else
                totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
            
            string customerId = "", customerName = "", strStatus = "";
            int i = 1;
            double total = 0;
            foreach (NEXMI.NMPaymentsWSI Item in WSIs)
            {
                customerId = ""; customerName = "";
                if (Item.Customer != null)
                {
                    customerId = Item.Customer.CustomerId; 
                    customerName = Item.Customer.CompanyNameInVietnamese;
                }
                total += Item.Payment.PaymentAmount;
    %>
        <tr ondblclick="javascript:fnLoadPaymentDetail('<%=Item.Payment.PaymentId%>')">
            <td style="width: 15px;"><%=i++%></td>
            <td style="width: 120px;"><div><%=Item.Payment.PaymentId%></div></td>
            <td style="width: 100px;"><div><%=Item.Payment.PaymentDate.ToString("dd-MM-yyyy")%></div></td>
            <td style="width: 120px;"><div><%=customerId%></div></td>
            <td><div title="<%=customerName%>"><%=customerName%></div></td>
            <td style="width: 120px;" align="right"><div><%=(Item.Payment.PaymentAmount).ToString("N3")%></div></td>
            <td><div title="<%=Item.Payment.DescriptionInVietnamese%>"><%=Item.Payment.DescriptionInVietnamese%></div></td>
            <td><div><%=NEXMI.NMCommon.GetStatusName(Item.Payment.PaymentStatus, langId)%></div></td>
            <td class="actionCols">
                <a href="javascript:fnLoadPaymentDetail('<%=Item.Payment.PaymentId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
    <% 
                if (Item.Payment.PaymentStatus == NEXMI.NMConstant.PaymentStatuses.Draft)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {    
    %>
                <a href="javascript:fnShowPaymentForm('<%=Item.Payment.PaymentId%>', '', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>          
    <% 
                    }
    %>
    <% 
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
    %>
                <a href="javascript:fnDeletePayment('<%=Item.Payment.PaymentId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
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
        $('#pagination-Payment').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });
</script>