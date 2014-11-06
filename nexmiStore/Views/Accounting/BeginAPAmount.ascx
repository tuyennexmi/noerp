<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.PISInput').autoNumeric('init', { vMin: '-100000000000000.000', vMax: '100000000000000.000' });
        $('.PISInput').on('change', function () {
            var elm = $(this);
            var partnerId = elm.parent().parent().attr('id');
            var amount = $('#txtAmount' + partnerId).autoNumeric('get');

            $.post(appPath + 'Accounting/AddAPToList', { partnerId: partnerId, amount: amount }, function (data) {
                if (data != '') {
                    alert(data);
                    elm.val('').focus();
                }
            });
        });
    });
</script>
<table style="width: 60%" class="tbTemplate">
    <tr>
        <td>Số dư công nợ phải trả</td>
    </tr>
    <tr>
        <td>
            <table style="width: 100%" id="tbUpdateStock" class="tbDetails">
                <thead>
                    <tr>
                        <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
                        <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_NAME", langId) %></th>
                        <th>Số dư</th>
                    </tr>
                </thead>
                <tbody id="rows">
                <%                    
                    List<NEXMI.NMCustomersWSI> Customers = (List<NEXMI.NMCustomersWSI>)ViewData["Customers"];
                    if (Customers != null)
                    {                        
                        foreach (NEXMI.NMCustomersWSI Item in Customers)
                        {
                %>
                    <tr id="<%=Item.Customer.CustomerId%>">                        
                        <td><%=Item.Customer.Code%></td>
                        <td><%=Item.Customer.CompanyNameInVietnamese%></td>
                        <td style="width: 100px;">
                            <input type="text" class="PISInput" value="" id="txtAmount<%=Item.Customer.CustomerId%>" style="width: 200px; height: 12px; text-align: right;" />
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
