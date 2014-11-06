<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMMonthlyGeneralJournalsWSI> lst = (List<NEXMI.NMMonthlyGeneralJournalsWSI>)ViewData["WSIs"];
    double beginCash = lst.Where(i => i.MGJ.AccountId == "1111" & i.MGJ.IsBegin == true).Sum(i => i.MGJ.DebitAmount);
    double received = lst.Where(i=>i.MGJ.AccountId=="1111" & i.MGJ.IsBegin == false).Sum(i=>i.MGJ.DebitAmount);
    double paid = lst.Where(i=>i.MGJ.AccountId=="1111" & i.MGJ.IsBegin == false).Sum(i=>i.MGJ.CreditAmount);
    
    double buy = lst.Where(i => i.MGJ.AccountId == "331" & i.MGJ.IsBegin == false).Sum(i => i.MGJ.CreditAmount);
    double supp_paid = lst.Where(i => i.MGJ.AccountId == "331" & i.MGJ.IsBegin == false).Sum(i => i.MGJ.DebitAmount);
    double ap = lst.Where(i => i.MGJ.AccountId == "331").Sum(i => i.MGJ.CreditAmount - i.MGJ.DebitAmount);
    
    double sales =lst.Where(i=>i.MGJ.AccountId=="131" & i.MGJ.IsBegin == false).Sum(i=>i.MGJ.DebitAmount);
    double cust_received = lst.Where(i => i.MGJ.AccountId == "131" & i.MGJ.IsBegin == false).Sum(i => i.MGJ.CreditAmount);
    double ar = lst.Where(i=>i.MGJ.AccountId=="131").Sum(i=>i.MGJ.DebitAmount - i.MGJ.CreditAmount);
%>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });
</script>

<form id="formCloseMonth" action="">
    <table class="frmInput">
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("FROM", langId) %> ngày</td>
            <td><input type="text" id="dtFrom" value="<%=ViewData["FromDate"] %>" style="width: 89px"/></td>
            <td class="lbright">Đến ngày</td>
            <td><input type="text" id="dtTo" value="<%=ViewData["ToDate"]%>" style="width: 89px"/></td>
        </tr>
        <tr>
            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
            <td colspan="5"><input type="text" id="txtDescriptions" value="" style="width: 689px"/></td>
        </tr>
        <tr>
            <td class="lbright">Số dư tiền đầu kỳ</td>
            <td><%=beginCash.ToString("N3")%></td>
            <td class="lbright">Số dư tiền cuối kỳ</td>
            <td><%=(beginCash + received - paid).ToString("N3") %></td>
        </tr>
        <tr>
            <td class="lbright">Thu</td>
            <td><%=received.ToString("N3")%></td>
            <td class="lbright">Chi</td>
            <td><%=paid.ToString("N3") %></td>
        </tr>
        <tr>
            <td class="lbright">Mua trong kỳ</td>
            <td><%=buy.ToString("N3")%></td>
            <td class="lbright">Trả trong kỳ</td>
            <td><%=supp_paid.ToString("N3")%></td>
            <td class="lbright">Nợ phải trả</td>
            <td><%=ap.ToString("N3")%></td>
        </tr>
        <tr>
            <td class="lbright">Bán trong kỳ</td>
            <td><%=sales.ToString("N3")%></td>
            <td class="lbright">Thu trong kỳ</td>
            <td><%=cust_received.ToString("N3")%></td>
            <td class="lbright">Nợ phải thu</td>
            <td><%=ar.ToString("N3")  %></td>
        </tr>
        <tr>
            <td class="lbright">Nhập</td>
            <td><%=lst.Where(i => i.MGJ.AccountId == "1561").Sum(i => i.MGJ.ImportQuantity).ToString("N3")%></td>
        </tr>
        <tr>
            <td class="lbright">Xuất</td>
            <td><%=lst.Where(i => i.MGJ.AccountId == "1561").Sum(i => i.MGJ.ExportQuantity).ToString("N3")%></td>
        </tr>
    </table>
</form>