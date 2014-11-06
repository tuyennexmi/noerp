<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<tr>
    <td>
        <%
            string langId = Session["Lang"].ToString(); 
            NEXMI.PurchaseOrderDetails Detail = (NEXMI.PurchaseOrderDetails)ViewData["Detail"];
            double quantity = 1;
            if (Detail == null)
            {
                Detail = new NEXMI.PurchaseOrderDetails();
            }
            else
            {
                quantity = Detail.Quantity;
            }
            string productId = Detail.ProductId;
        %>
        <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = productId, label = NEXMI.NMCommon.GetName(productId, langId) });%>
    </td>
    <td><label id="lbUnitName"></label>
        <%Html.RenderPartial("slUnits");%>
    </td>
    <td><input type="text" id="txtQuantity" value="<%=quantity.ToString("N3")%>" style="width: 80px;" class="auto" /></td>
    <td><input type="text" id="txtPrice" value="<%=Detail.Price.ToString("N3")%>" style="width: 100px;"/></td>
    <td><input type="text" id="txtDiscount" value="<%=Detail.Discount%>" style="width: 80px;" /></td>
    <td><input type="text" id="txtVATRate" value="<%=Detail.Tax.ToString("N3")%>" style="width: 80px;" /></td>
    <td><input type="text" id="txtAmount" value="<%=Detail.Amount.ToString("N3")%>" style="width: 100px;" readonly="readonly" /></td>
    <td><input type="text" id="txtDetailDescription" value="<%=Detail.Description%>" style="max-width: 250px;" /></td>
    <td class="actionCols">
        <input type='hidden' id='txtDetailID' value='<%=Detail.Id %>' />
        <button type="button" class="btActions save" onclick="javascript:fnAddPODetail()"></button>
        <button type="button" class="btActions reset" onclick="javascript:fnResetFormPODetail()"></button>
    </td>
</tr>
<script type="text/javascript">
    $(function () {
        $("#formPODetail").jqxValidator({
            rules: [
                { input: '#cbbProducts', message: 'Bạn chưa chọn sản phẩm', action: 'change', rule: function (input) {
                    if ($(input).jqxComboBox('getSelectedItem') == null)
                        return false;
                    return true;
                }
                }
            ]
        });
        $('#txtQuantity, #txtPrice').autoNumeric('init', { mDec: '3' });
        $('#txtDiscount, #txtVATRate').autoNumeric('init', { vMin: '0.00', vMax: '100.00' });
        $('#txtQuantity, #txtVATRate, #txtPrice, #txtDiscount').on('keyup', function () {
            calculateAmount();
        });
        $('#cbbProducts').find(".jqx-combobox-input").focus();
    });

    function calculateAmount() {
        var quantity = $('#txtQuantity').autoNumeric('get');
        var price = $('#txtPrice').autoNumeric('get');
        if (price == '') {
            price = 0;
        }
        var tax = $('#txtVATRate').autoNumeric('get');
        var amount = quantity * price;
        var discount = $('#txtDiscount').autoNumeric('get');
        var discountAmount = amount * discount;
        var taxAmount = (amount - discountAmount) * tax / 100;
        $('#txtAmount').val(fnNumberFormat(amount));
    }
</script>