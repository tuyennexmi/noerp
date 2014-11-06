<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> WSIs = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    NEXMI.NMCustomersWSI Customer = (NEXMI.NMCustomersWSI)ViewData["Customer"];
    List<NEXMI.NMSalesInvoicesWSI> iList = (List<NEXMI.NMSalesInvoicesWSI>)ViewData["Invoices"];

    String localpath = "http://" + Request.Url.Authority + Url.Content("~") + "Content/avatars/" + NEXMI.NMCommon.GetCompany().Customer.Avatar;
%>

<table style="width:100%">
    <tr>
        <td width="40%">
            <img height="60px" alt="" src="<%=localpath%>" />
        </td>
        <td width="60%">
            <div style="text-align: center;" align="center">
                <h3><b>ĐỐI CHIẾU CÔNG NỢ KHÁCH HÀNG</b></h3>
                <label><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <%=DateTime.Parse(ViewData["dtFrom"].ToString()).ToString("dd/MM/yyyy") %> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <%=DateTime.Parse(ViewData["dtTo"].ToString()).ToString("dd/MM/yyyy")%></label>
            </div>
        </td>
    </tr>
</table>

<br />
<table class="tbDetails" border=".1">
    <thead>
        <tr>          
            <th width="9%"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th width="15%"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %> / PTVC</th>            
            <th width="4%"><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
            <th width="7%"><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th width="7%"><%= NEXMI.NMCommon.GetInterface("PROMOTION", langId) %></th>
            <th width="11%"><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
            <th width="12%"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></th>
        </tr>
    </thead>
    <tbody>
<% 
    if (WSIs.Count > 0)
    {
        double totalCredit = 0, totalDebit = 0;
        double totalAmount = 0, promote = 0;
        string productsName = "";
        double begin = WSIs.Where(i=>i.MGJ.IsBegin == true).Sum(i=>i.MGJ.DebitAmount);
        %>
        <tr>
            <td colspan="7"><strong>Nợ đầu kỳ</strong> </td>                
            <td align="right"><%=begin.ToString("N3")%></td>
            <td align="right"></td>
        </tr>
        <%      //foreach (NEXMI.NMSalesInvoicesWSI Item in iList)
        foreach (NEXMI.NMMonthlyGeneralJournalsWSI mgj in WSIs)
        {
            if (mgj.MGJ.IsBegin)
                continue;
            if (mgj.MGJ.SIID != null & mgj.MGJ.RPTID == null)
            {
                NEXMI.NMSalesInvoicesWSI Item = iList.Where(i => i.Invoice.InvoiceId == mgj.MGJ.SIID).FirstOrDefault();
                productsName = "";
                foreach (NEXMI.SalesInvoiceDetails dt in Item.Details)
                    productsName += NEXMI.NMCommon.GetName(dt.ProductId, langId);
                totalDebit += Item.Invoice.TotalAmount;
                totalAmount += Item.Details[0].Quantity;
                promote += Item.Details[0].Quantity * Item.Details[0].Discount / 100;
                //totalCredit += Item.Receipts.Sum(i => i.ReceiptAmount);                    
        %>
        
        <tr>
            <td><%=Item.Invoice.InvoiceDate.ToString("dd-MM-yyyy")%></td>
            <td><%=productsName%></td>            
            <td><%=Item.Invoice.SourceDocument%></td>
            <td ><%= NEXMI.NMCommon.GetUnitNameById(Item.Details[0].UnitId) %></td>
            <td align="right"><%=Item.Details[0].Quantity%></td>
            <td align="right"><%= (Item.Details[0].Quantity * Item.Details[0].Discount / 100).ToString("N3") %></td>
            <td align="right"><%=Item.Details[0].Price.ToString("N3")%></td>
            <td align="right"><%=Item.Details[0].TotalAmount.ToString("N3")%></td>
            <td align="right"><%--<%=totalCredit.ToString("N3")%>--%></td>
        </tr>
        <% 
            }
            else
            {
                totalCredit += mgj.MGJ.CreditAmount;
        %>
            <tr>
                <td><%=mgj.MGJ.IssueDate.ToString("dd-MM-yyyy")%></td>
                <td colspan="7"><%=mgj.MGJ.Descriptions %></td>
                <td align="right"><%=mgj.MGJ.CreditAmount.ToString("N3")%></td>                
            </tr>            
        <%  }
        } %>
            <tr>
                <td colspan="4"><strong>Tổng cộng trong kỳ</strong> </td>                
                <td align="right"><%= totalAmount.ToString("N3")%></td>
                <td align="right"><%= promote.ToString("N3")%></td>
                <td></td>
                <td align="right"><%= totalDebit.ToString("N3")%></td>
                <td align="right"><%= totalCredit.ToString("N3")%></td>
            </tr>            
            <tr>
                <td colspan="7"><strong>Nợ cuối kỳ</strong> </td>
                <td align="right"><%=(begin + totalDebit - totalCredit).ToString("N3")%></td>
                <td></td>                
            </tr>            
            <tr>
                <td>Bằng chữ:</td> 
                <td colspan='8'><%=NEXMI.NMCommon.ReadNum(begin + totalDebit - totalCredit) %>.</td>
            </tr>
    <%  }
        else { 
    %>
            <tr><td colspan="9" align="center"><h4>Không có dữ liệu.</h4></td></tr>
        <%
            }
        %>
    </tbody>
</table>
<br />
<table width='100%'>
    <tbody>
        <tr align="center">
            <td width="50%"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %>...... <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> ...... năm <%=DateTime.Today.Year %></td>
            <td width="50%" ><%= NEXMI.NMCommon.GetInterface("DATE", langId) %><%=DateTime.Today.Day %> <%= NEXMI.NMCommon.GetInterface("MONTH", langId) %> <%=DateTime.Today.Month %> năm <%=DateTime.Today.Year %></td>
        </tr>
        <tr align="center">
            <td width='50%'><%= ((NEXMI.NMCustomersWSI)ViewData["Customer"]).Customer.CompanyNameInVietnamese %><br />(ký nhận và ghi rõ họ tên)</td>
            <td width="50%" ><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %> <br />Đại diện công ty </td>
        </tr>
        <tr>
            <td>
                <br />
                <br />
                <br />
                
            </td>
            <td></td>
        </tr>
    </tbody>
</table>

<%Html.RenderPartial("ReportFooter"); %>