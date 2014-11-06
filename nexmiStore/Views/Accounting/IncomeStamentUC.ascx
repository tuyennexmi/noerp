<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<%  
    string from = ViewData["From"].ToString();
    string to = ViewData["To"].ToString();
    NEXMI.NMAccountNumbersBL aBL = new NEXMI.NMAccountNumbersBL();
    NEXMI.NMAccountNumbersWSI aWsi = new NEXMI.NMAccountNumbersWSI();
    aWsi.Mode = "SRC_OBJ";
    aWsi.AccountNumber.ParentId = "511";
    
    List<NEXMI.NMAccountNumbersWSI> aList = aBL.callListBL(aWsi);
%>

<table>
<tr>
        <td><b><%=NEXMI.NMCommon.GetCompany().Customer.CompanyNameInVietnamese %></b></td>
    </tr>
    <tr>
        <td><b>Báo cáo kết quả hoạt động kinh doanh</b></td>
    </tr>    
    <tr><td><br /></td></tr>
</table>

<table class="tbDetails" style="width: 60%">    
    <tr>
        <td><b>Doanh thu</b></td>
    </tr>
    <%  double revenue = 0, cost = 0;
        
        NEXMI.NMMonthlyGeneralJournalsBL bl = new NEXMI.NMMonthlyGeneralJournalsBL();
        NEXMI.NMMonthlyGeneralJournalsWSI wsi = new NEXMI.NMMonthlyGeneralJournalsWSI();
        List<NEXMI.NMMonthlyGeneralJournalsWSI> pList;
        wsi.Mode = "SRC_OBJ";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        double total = 0;
        foreach (NEXMI.NMAccountNumbersWSI itm in aList)
        {                
            wsi.MGJ.AccountId = itm.AccountNumber.Id;
            pList = bl.callListBL(wsi);
            total = pList.Sum(i => i.MGJ.CreditAmount);
            revenue += total;
     %>
    <tr>
        <td>&nbsp;&nbsp;&nbsp; <%=itm.AccountNumber.NameInVietnamese %></td>
        <td></td>
        <td align="right"><%=total.ToString("N3") %></td>
    </tr>        
    <% } %>
    <tr>
        <td><b>Giá vốn hàng bán</b></td>
    
    <%  aWsi.AccountNumber.Id = "632";
        aWsi = aBL.callSingleBL(aWsi);
        wsi.MGJ.AccountId = "632";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        pList = bl.callListBL(wsi);
        cost = pList.Sum(i => i.MGJ.DebitAmount);
         %>
        <%--<td><%=aWsi.AccountNumber.NameInVietnamese %></td>--%>
        <td align="right"><%=cost.ToString("N3") %></td>
        <td></td>
    </tr>
    <tr>
        <td><b>Lãi gộp</b></td>
        <td></td>
        <td align="right"><%=(revenue - cost).ToString("N3") %></td>
    </tr>
    <tr>
        <td><b>Chi phí hoạt động</b></td>
    </tr>
    <tr>
        <td><b>&nbsp;&nbsp;&nbsp; Chi phí bán hàng</b></td>
    </tr>
    <%
        aWsi.AccountNumber.ParentId = "641";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        aList = aBL.callListBL(aWsi);
        if (aList.Count > 0)
        {
            List<NEXMI.NMMonthlyGeneralJournalsWSI> wsi641 = new List<NEXMI.NMMonthlyGeneralJournalsWSI>();
            List<NEXMI.NMAccountNumbersWSI> subAcc;
            foreach (NEXMI.NMAccountNumbersWSI itm in aList)
            {
                wsi.MGJ.AccountId = itm.AccountNumber.Id;
                pList = bl.callListBL(wsi);
                total = pList.Sum(i => i.MGJ.DebitAmount);
                cost += total;
         %>
                <tr>
                    <td>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <%=itm.AccountNumber.NameInVietnamese%></td>
                    <td align="right"><%=total.ToString("N3")%></td>
                    <td></td>
                </tr>        
    <%          aWsi.AccountNumber.ParentId = itm.AccountNumber.Id;
                wsi.Filter.FromDate = from;
                wsi.Filter.ToDate = to;
                subAcc = aBL.callListBL(aWsi);
                if (subAcc.Count > 0)
                {
                    foreach (NEXMI.NMAccountNumbersWSI subitm in subAcc)
                    {
                        wsi.MGJ.AccountId = subitm.AccountNumber.Id;
                        pList = bl.callListBL(wsi);
                        total = pList.Sum(i => i.MGJ.DebitAmount);
                        cost += total;
                    %>
                    <tr>
                        <td>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <%=subitm.AccountNumber.NameInVietnamese%></td>
                        <td align="right"><%=total.ToString("N3")%></td>
                        <td></td>
                    </tr>        
                        <%
                    }
                }
            }
        }
            %>
    <tr>
        <td> <b>&nbsp;&nbsp;&nbsp; Chi phí quản lý</b></td>
    </tr>
    <%
        aWsi.AccountNumber.ParentId = "642";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        aList = aBL.callListBL(aWsi);
        if (aList.Count > 0)
            foreach (NEXMI.NMAccountNumbersWSI itm in aList)
            {
                wsi.MGJ.AccountId = itm.AccountNumber.Id;
                pList = bl.callListBL(wsi);
                total = pList.Sum(i => i.MGJ.DebitAmount);
                cost += total;
         %>
         <tr>
        <td>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <%=itm.AccountNumber.NameInVietnamese%></td>
        <td align="right"><%=total.ToString("N3")%></td>
        <td></td>
    </tr>        
    <% } %>
    <tr>
        <td><b>Thu nhập và chi phí khác</b></td>
    </tr>
    <tr>    <td><b>&nbsp;&nbsp;&nbsp; Thu nhập khác</b></td>
    </tr>
    <%
        aWsi.AccountNumber.ParentId = "711";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        aList = aBL.callListBL(aWsi);
        if(aList.Count >0)
            foreach (NEXMI.NMAccountNumbersWSI itm in aList)
            {
                wsi.MGJ.AccountId = itm.AccountNumber.Id;
                pList = bl.callListBL(wsi);
                total = pList.Sum(i => i.MGJ.CreditAmount);
                revenue += total;
         %>
    <tr>
        <td>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <%=itm.AccountNumber.NameInVietnamese%></td>
        <td></td>
        <td align="right"><%=total.ToString("N3")%></td>
    </tr>            
    <% } %>
    <tr>    
        <td><b>&nbsp;&nbsp;&nbsp; Chi phí khác</b></td>
    </tr>
    <%
        aWsi.AccountNumber.ParentId = "811";
        wsi.Filter.FromDate = from;
        wsi.Filter.ToDate = to;
        aList = aBL.callListBL(aWsi);
        if (aList.Count > 0)
            foreach (NEXMI.NMAccountNumbersWSI itm in aList)
            {
                wsi.MGJ.AccountId = itm.AccountNumber.Id;
                pList = bl.callListBL(wsi);
                total = pList.Sum(i => i.MGJ.DebitAmount);
                cost += total;
         %>
    <tr>
        <td>&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp; <%=itm.AccountNumber.NameInVietnamese%></td>
        <td align="right"><%=total.ToString("N3")%></td>
        <td></td>
    </tr>        
    <% } %>
    <tr>
        <td><b>Lợi tức ròng</b></td>
        <td></td>
        <td align="right"><%=(revenue-cost).ToString("N3") %></td>
    </tr>
</table>