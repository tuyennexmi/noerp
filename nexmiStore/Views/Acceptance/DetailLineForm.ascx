<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.AcceptanceDetails Detail = (NEXMI.AcceptanceDetails)ViewData["Detail"];
    
    if (Detail == null)
    {
        Detail = new NEXMI.AcceptanceDetails();
    }
    
    //Detail.UnitId
%>

    <tr id="<%=Detail.ProductId%>">
        
        <td>[<%= NEXMI.NMCommon.GetProductCode(Detail.ProductId) %>] <%=NEXMI.NMCommon.GetName(Detail.ProductId, langId)%></td>
        <td><%=NEXMI.NMCommon.GetUnitNameById(Detail.UnitId) %></td>
        <td><input type="text" id="Quantity" name="Quantity" value="<%=Detail.Quantity.ToString("N3")%>" style="width: 100px;" class="auto" /></td>
        <td><input type="text" id="Price" name="Price" value="<%=Detail.Price.ToString("N3")%>" style="width: 100px;" /></td>
        <td><input type="text" id="Discount" name="Discount" value="<%=Detail.Discount.ToString("N3")%>" style="width: 100px;" /></td>
        <td><input type="text" id="Tax" name="Tax" value="<%=Detail.Tax.ToString("N3")%>" style="width: 100px;" /></td>
        <td><input type="text" id="Amount" name="Amount" value="<%=Detail.Amount.ToString("N3")%>" style="width: 100px;" readonly="readonly" /></td>
        <td><input type="text" id="LineDescription" name="Description" value="<%=Detail.Description%>" style="max-width: 150px;" /></td>

        <td class="actionCols">
            <input type='hidden' id='AcceptanceId' name="AcceptanceId" value='<%=Detail.AcceptanceId%>' />
            <input type="hidden" id="ProductId" name="ProductId" value="<%=Detail.ProductId%>" />
            <input type='hidden' id='UnitId' name="UnitId" value='<%=Detail.UnitId%>' />
            <button type="button" class="btActions save" onclick="javascript:fnSaveDetailLine('<%=Detail.ProductId %>')" title='Bấm lưu để ghi nhận sản phẩm này.'></button>
            <button type="button" class="btActions reset" onclick="javascript:fnResetSalesOrderDetail()" title='Bấm reset để xóa thông tin trong form này.'></button>
        </td>
        
    </tr>

<%
    if(NEXMI.NMCommon.GetSetting("SHOW_EDIT_DETAILS_FORM"))
    {
%>
    <%--<script type="text/javascript">
        $('#txtLineDescription').focus(function () {
            var productId = $('#ProductId').val();
            if (productId != '') {
                openWindow('Soạn chi tiết thông tin hàng hóa?', 'Sales/EditSODetailDescriptions', { productId: productId }, 600, 400);
            }
        });
    </script>--%>
<%
    }
%>
<script type="text/javascript">
    $(function () {
        $('#Quantity, #Price, #Amount').autoNumeric('init', { mDec: '3' });
        $('#Discount, #Tax').autoNumeric('init', { vMin: '0.00', vMax: '100.00' });
        $('#Price, #Quantity, #Discount, #Tax').on('keyup', function () {
            calculateAmount();
        });
    });

    function fnSaveDetailLine(id) {
        //var postData = $("#formLine").serializeArray();
        //var postData = $(":input").serializeArray();
        $.ajax({
            url: appPath + "Acceptance/SaveOrUpdateLine",
            type: "POST",
            data: { 
                Quantity: $('#Quantity').val(),
                Price: $('#Price').val(),
                Discount: $('#Discount').val(),
                Tax: $('#Tax').val(),
                Description: $('#LineDescription').val(),
                ProductId: $('#ProductId').val(),
                UnitId: $('#UnitId').val(),
                AcceptanceId: $('#AcceptanceId').val()
            },
            success: function (data, textStatus, jqXHR) {
                $("#tbodyDetails").html(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#tbodyDetails").html(errorThrown);
            }
        });
    }

    function calculateAmount() {
        var quantity = $('#Quantity').autoNumeric('get');
        var price = $('#Price').autoNumeric('get');
        if (price == '') {
            price = 0;
        }
        var tax = $('#Tax').autoNumeric('get');
        var amount = quantity * price;
        var discount = $('#Discount').autoNumeric('get');
        var discountAmount = amount * discount;
        var taxAmount = (amount - discountAmount) * tax / 100;
        $('#Amount').val(fnNumberFormat(amount));
    }
</script>