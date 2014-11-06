<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.PbPInput').autoNumeric('init', { vMax: '100000000000000' });
        $('.PbPInput').on('change', function () {
            var elm = $(this);
            var partnerId = elm.parent().parent().attr('id');
            var amount = $('#txtAmount' + partnerId).autoNumeric('get');
            var productId = document.getElementsByName('cbbProducts')[0].value;
            if (productId == '') {
                alert('Bạn chưa chọn sản phẩm. Vui lòng chọn sản phẩm trước!');
            }
            else {
                $.post(appPath + 'Planning/AddPbPToList', { partnerId: partnerId, productId: productId, amount: amount }, function (data) {
                    if (data != '') {
                        $("#lbPbPTotalAmount").val(fnNumberFormat(data.total.toString()));
                        $("#txtTotal").val(fnNumberFormat(data.total.toString()));
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
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th>Sản lượng</th>            
        </tr>
    </thead>
    <tbody id="rows">
    <%                    
        List<NEXMI.NMCustomersWSI> Suppliers = ((List<NEXMI.NMCustomersWSI>)ViewData["Partners"]);//.Where(i => i.Customer.GroupId == NEXMI.NMConstant.CustomerGroups.Supplier).ToList();
        double totalAmount = 0;        
        if (Session["PurchsesPLN"] != null)
        {
            int count = 0; double amount = 0; 
            string product ="";
            string productId = ViewData["Product"].ToString();
            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["PurchsesPLN"];
            NEXMI.MasterPlanningDetails detail;
            foreach (NEXMI.NMCustomersWSI Item in Suppliers)
            {
                detail = List.Where(s => s.PartnerId == Item.Customer.CustomerId & s.ProductId == productId).FirstOrDefault();
                if (detail != null)
                {
                    amount = detail.DebitAmount;
                    product = NEXMI.NMCommon.GetName(detail.ProductId, langId);
                }
                else
                {
                    amount = 0;
                    product = "";
                }
    %>
        <tr id="<%=Item.Customer.CustomerId%>">
            <td><%=++count %></td>
            <td><%=Item.Customer.Code%></td>
            <td ><div style='width:150px'> <%=Item.Customer.CompanyNameInVietnamese%></div></td>
            <td ><div style='width:150px'> <%=product%></div></td>
            <td ><input type="text" class="PbPInput" value="<%=amount %>" id="txtAmount<%=Item.Customer.CustomerId%>" style="width: 186px; height: 12px; text-align: right;" /></td>            
        </tr>
    <%
            }
        }
        else
        {
            List<NEXMI.MasterPlanningDetails> List = (List<NEXMI.MasterPlanningDetails>)Session["PurchsesPLN"];
            int count = 0;
            NEXMI.NMCustomersWSI cus;
            totalAmount = List.Sum(i=>i.DebitAmount);
            foreach (NEXMI.MasterPlanningDetails Item in List)
            {
                cus = Suppliers.Where(i => i.Customer.CustomerId == Item.PartnerId).FirstOrDefault();
    %>
        <tr id="<%=cus.Customer.CustomerId%>">
            <td><%=++count %></td>
            <td><%=cus.Customer.Code%></td>
            <td ><div > <%=cus.Customer.CompanyNameInVietnamese%></div></td>
            <td ><div > <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></div></td>
            <td ><input type="text" class="PbPInput" value="<%= Item.DebitAmount%>" id="txtAmount<%=cus.Customer.CustomerId%>" style="width: 186px; height: 12px; text-align: right;" /></td>            
        </tr>
    <%
            }
        }
    %>
        <tr>
            <td colspan='4'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></b></td>                        
            <td ><input type="text" value="<%= totalAmount.ToString("N3") %>" id="lbPbPTotalAmount" style="width: 186px; height: 12px; text-align: right;" readonly="readonly"/></td> 
        </tr>
    </tbody>
</table>

