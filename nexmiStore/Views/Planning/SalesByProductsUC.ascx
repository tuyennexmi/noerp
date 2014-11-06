<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.SbPInput').autoNumeric('init', { vMax: '100000000000000' });
        $('.SbPInput').on('change', function () {
            var elm = $(this);
            var partnerId = elm.parent().parent().attr('id');
            var amount = $('#txtAmount' + partnerId).autoNumeric('get');
            var productId = document.getElementsByName('cbbProducts')[0].value;
            if (productId == '') {
                alert('Bạn chưa chọn sản phẩm. Vui lòng chọn sản phẩm trước!');
            }
            else {
                $.post(appPath + 'Planning/AddSbPToList', { partnerId: partnerId, productId: productId, amount: amount }, function (data) {
                    if (data != '') {
                        $("#lbSbPTotalAmount").val(fnNumberFormat(data.total.toString()));
                        var total = $("#txtTotal").autoNumeric('get');
                        var rate = fnNumberFormat(amount / total);
                        $("#txtRating" + partnerId).val(rate);
                        $("#lbSbPRate").val(fnNumberFormat(data.total / total));
                    }
                    else {
                        alert(data);
                        elm.val('').focus();
                    }
                });
            }
        });

    });
</script>


<table style="width: 100%" id="tbUpdateStock" class="tbDetails">
    <thead>
        <tr>
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th>Sản lượng</th>
            <th>Tỷ lệ</th>
        </tr>
    </thead>
    <tbody id="rows">
    <%                    
        List<NEXMI.NMCustomersWSI> Customers = ((List<NEXMI.NMCustomersWSI>)ViewData["Partners"]);//.Where(i => i.Customer.GroupId == NEXMI.NMConstant.CustomerGroups.Customer).ToList();
        double totalAmount = 0;
        double plAmount = double.Parse(ViewData["Amount"].ToString());
        string product = ViewData["Product"].ToString();
        if (Session["SalesPLN"] != null)
        {
            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["SalesPLN"];
            int count = 0;
            totalAmount = List.Where(s => s.ProductId == product).Sum(i => i.CreditAmount);
            double amount = 0;
            foreach (NEXMI.NMCustomersWSI Item in Customers)
            {
                amount = List.Where(s => s.PartnerId == Item.Customer.CustomerId & s.ProductId == product).Sum(i => i.CreditAmount);
    %>
        <tr id="<%=Item.Customer.CustomerId%>">
            <td><%=++count %></td>
            <td><%=Item.Customer.Code%></td>
            <td ><div style='width:150px'> <%=Item.Customer.CompanyNameInVietnamese%></div></td>
            <td ><input type="text" class="SbPInput" value="<%=amount %>" id="txtAmount<%=Item.Customer.CustomerId%>" style="width: 186px; height: 12px; text-align: right;" /></td>
            <td ><input type="text" class="SbPInput" value="<%=amount/plAmount %>" id="txtRating<%=Item.Customer.CustomerId%>" style="width: 88px; height: 12px; text-align: right;" readonly="readonly"/></td> 
        </tr>
    <%
            }
        }
        else
        {
            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["SalesPLN"];
            int count = 0;
            NEXMI.NMCustomersWSI cus;
            totalAmount = List.FindAll(p => p.ProductId == product).Sum(i => i.CreditAmount);
            foreach (NEXMI.MasterPlanningDetails Item in List)
            {
                cus = Customers.Where(i => i.Customer.CustomerId == Item.PartnerId).FirstOrDefault();
    %>
        <tr id="<%=cus.Customer.CustomerId%>">
            <td><%=++count %></td>
            <td><%=cus.Customer.Code%></td>
            <td ><div style='width:150px'> <%=cus.Customer.CompanyNameInVietnamese%></div></td>
            <td ><input type="text" class="SbPInput" value="<%= Item.CreditAmount%>" id="txtAmount<%=cus.Customer.CustomerId%>" style="width: 186px; height: 12px; text-align: right;" /></td>
            <td ><input type="text" class="SbPInput" value="<%= Item.CreditAmount/plAmount %>" id="txtRating<%=cus.Customer.CustomerId%>" style="width: 88px; height: 12px; text-align: right;" readonly="readonly"/></td> 
        </tr>
    <%
            }
        }
    %>
        <tr>
            <td colspan='3'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></b></td>                        
            <td ><input type="text" value="<%= totalAmount.ToString("N3") %>" id="lbSbPTotalAmount" style="width: 186px; height: 12px; text-align: right;" readonly="readonly"/></td> 
            <td ><input type="text" value="<%= (totalAmount/plAmount).ToString("N3") %>" id="lbSbPRate" style="width: 88px; height: 12px; text-align: right;" readonly="readonly"/></td> 
        </tr>
    </tbody>
</table>

