<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.PISInput').autoNumeric('init', { vMin: '-100000000000000.000', vMax: '100000000000000.000' });
        $('.PISInput').on('change', function () {
            var elm = $(this);
            var accId = elm.parent().parent().attr('id');
            var amount = $('#txtAmount' + accId).autoNumeric('get');
            var bank = $('#txtBankId' + accId).val();
            $.post(appPath + 'Accounting/AddToList', { account: accId, amount: amount, bank: bank }, function (data) {
                if (data != '') {
                    alert(data);
                    elm.val('').focus();
                }
            });
        });
    });
</script>
<table style="width: 69%" class="tbTemplate">
    <tr>
        <td>Số dư tiền mặt</td>
    </tr>
    <tr>
        <td>
            <table style="width: 100%" id="tbUpdateStock" class="tbDetails">
                <thead>
                    <tr>
                        <th>Loại tiền</th>
                        <th>Số dư</th>
                    </tr>
                </thead>
                <tbody id="rows">
                <%                    
                    List<NEXMI.NMAccountNumbersWSI> WSIs = (List<NEXMI.NMAccountNumbersWSI>)ViewData["Cash"];
                    if (WSIs != null)
                    {                        
                        foreach (NEXMI.NMAccountNumbersWSI Item in WSIs)
                        {
                %>
                    <tr id="<%=Item.AccountNumber.Id %>">
                        <td><%=Item.AccountNumber.NameInVietnamese%></td>                        
                        <td style="width: 100px;">
                            <input type="text" class="PISInput" value="" id="txtAmount<%=Item.AccountNumber.Id%>" style="width: 200px; height: 12px; text-align: right;" />
                        </td>
                    </tr>
                <%
                        }
                    }
                %>
                </tbody>
            </table>
        </td>
    </tr>
</table>

<table style="width: 69%" class="tbTemplate">
    <tr>
        <td>Số dư tiền gửi</td>
    </tr>
    <tr>
        <td>
            <table style="width: 100%" id="Table1" class="tbDetails">
                <thead>
                    <tr>
                        <th>Loại tiền</th>                        
                        <th><%= NEXMI.NMCommon.GetInterface("BANK_NAME", langId) %></th>
                        <th>Số dư</th>                        
                    </tr>
                </thead>
                <tbody id="banks">
                <%                    
                    WSIs = (List<NEXMI.NMAccountNumbersWSI>)ViewData["BankAcc"];
                    List<NEXMI.NMBanksWSI> banks = (List<NEXMI.NMBanksWSI>)ViewData["Banks"];
                    
                    if (WSIs != null)
                    {
                        foreach (NEXMI.NMAccountNumbersWSI Item in WSIs)
                        {
                            foreach (NEXMI.NMBanksWSI bnk in banks)
                            {
                %>
                    <tr id="<%=Item.AccountNumber.Id%><%=bnk.Bank.Id %>">
                        <td><%=Item.AccountNumber.NameInVietnamese%></td>
                        <td><%=bnk.Bank.Name %></td>
                        <td style="width: 100px;">
                            <input type="hidden" id="txtBankId<%=Item.AccountNumber.Id%><%=bnk.Bank.Id %>" value="<%=bnk.Bank.Id%>" />
                            <input type="text" class="PISInput" value="" id="txtAmount<%=Item.AccountNumber.Id%><%=bnk.Bank.Id %>" style="width: 200px; height: 12px; text-align: right;" />
                        </td>
                    </tr>
                <%
}
                        }
                    }
                %>
                </tbody>
            </table>
        </td>
    </tr>
</table>