<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.SalesOrderDetails Detail = (NEXMI.SalesOrderDetails)ViewData["Detail"];
    double quantity = 1;
    if (Detail == null)
    {
        Detail = new NEXMI.SalesOrderDetails();
    }
    else
    {
        quantity = Detail.Quantity;
    }
    string productId = Detail.ProductId;
    bool inputprice = (bool)ViewData["PriceInput"];
%>

<tr>
    <td>
        <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = productId, label = NEXMI.NMCommon.GetName(productId, langId) });%>
    </td>
    <td>
        <label id="lbUnitName"></label>
        <%Html.RenderPartial("slUnits");%>
    </td>
    <td>
        <input type="text" id="txtQuantity" value="<%=quantity.ToString("N3")%>" style="width: 100px;" class="auto" />
    </td>
    <td>
<%if (inputprice)
{ %>
        <input type="text" id="txtPrice" value="<%=Detail.Price.ToString("N3")%>" style="width: 100px;" />
<%}
else
{ %>
        <input type="text" id="txtPrice" value="<%=Detail.Price.ToString("N3")%>" style="width: 100px;" readonly="readonly" />
<%} %>
    </td>
    <td><input type="text" id="txtDiscount" value="<%=Detail.Discount.ToString("N3")%>" style="width: 100px;" /></td>
    <td><input type="text" id="txtVATRate" value="<%=Detail.Tax.ToString("N3")%>" style="width: 100px;" /></td>
    <td><input type="text" id="txtAmount" value="<%=Detail.Amount.ToString("N3")%>" style="width: 100px;" readonly="readonly" /></td>
    <td><input type="text" id="txtDescriptionDetail" value="<%=Detail.Description%>" style="max-width: 150px;" /></td>
    <td class="actionCols">
        <input type='hidden' id='txtDetailID' value='<%=Detail.Id %>' />
        <button type="button" class="btActions save" onclick="javascript:fnSaveSalesOrderDetail()" title='Bấm lưu để ghi nhận sản phẩm này.'></button>
        <button type="button" class="btActions reset" onclick="javascript:fnResetSalesOrderDetail()" title='Bấm reset để xóa thông tin trong form này.'></button>
    </td>
</tr>
<%
    if(NEXMI.NMCommon.GetSetting("SHOW_EDIT_DETAILS_FORM"))
    {
%>
    <script type="text/javascript">
        $('#txtDescriptionDetail').focus(function () {
            var productId = document.getElementsByName('cbbProducts')[0].value;
            if (productId != '') {
                openWindow('Soạn chi tiết thông tin hàng hóa?', 'Sales/EditSODetailDescriptions', { productId: productId }, 600, 400);
            }
        });
    </script>
<%
    }
%>
<script type="text/javascript">
    $(function () {
        $("#formSODetail").jqxValidator({
            rules: [
                { input: '#cbbProducts', message: 'Bạn chưa chọn sản phẩm', action: 'change', rule: function (input) {
                    if ($(input).jqxComboBox('getSelectedItem') == null)
                        return false;
                    return true;
                }
                }
            ]
        });
        $('#txtQuantity, #txtPrice, #txtAmount').autoNumeric('init', { mDec: '3' });
        $('#txtDiscount, #txtVATRate').autoNumeric('init', { vMin: '0.00', vMax: '100.00' });
        $('#txtPrice, #txtQuantity, #txtDiscount, #txtVATRate').on('keyup', function () {
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