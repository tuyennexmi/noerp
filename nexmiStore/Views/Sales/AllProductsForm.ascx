<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    NEXMI.NMProductsWSI WSI = new NEXMI.NMProductsWSI();
    NEXMI.NMProductsBL BL = new NEXMI.NMProductsBL();
    WSI.Mode = "SRC_OBJ";

    List<NEXMI.NMProductsWSI> WSIs = BL.callListBL(WSI);
    
    bool inputprice = true;// (bool)ViewData["PriceInput"];

    foreach (NEXMI.NMProductsWSI Item in WSIs)
    {
%>

<tr id='<%=Item.Product.ProductId %>'>
    <td>[<%=Item.Product.ProductCode %>] <%=Item.Translation.Name%></td>
    <td>
        <label id="lbUnitName"><%=NEXMI.NMCommon.GetProductUnitName(Item.Product.ProductId) %></label>
        <%--<%Html.RenderPartial("slUnits");%>--%>
    </td>
    <td>
        <input type="text" id="txtQuantity" value="0" style="width: 100px;" class="auto" />
    </td>
    <td>
<%if (inputprice)
  { 
  %>
        <input type="text" id="txtPrice" value="" style="width: 100px;" />
<%}
  else
  {
      %>
        <input type="text" id="txtPrice" value="0" style="width: 100px;" readonly="readonly" />
<%} %>
    </td>
    <td><input type="text" id="txtDiscount" value="0" style="width: 100px;" /></td>
    <td><input type="text" id="txtVATRate" value="<%=Item.Product.VATRate.ToString("N3")%>" style="width: 100px;" /></td>
    <td><input type="text" id="txtAmount" value="" style="width: 100px;" readonly="readonly" /></td>
    <td><input type="text" id="txtDescriptionDetail" value="<%=Item.Translation.Description%>" style="max-width: 150px;" /></td>
    <td class="actionCols">
        <button type="button" class="btActions save" onclick="javascript:fnAllProductsSaveSODetail('<%=Item.Product.ProductId %>')" title='Bấm lưu để ghi nhận sản phẩm này.'></button>
        <button type="button" class="btActions reset" onclick="javascript:fnResetSalesOrderDetail()" title='Bấm reset để xóa thông tin trong form này.'></button>
    </td>
</tr>
<%
  if (NEXMI.NMCommon.GetSetting("SHOW_EDIT_DETAILS_FORM"))
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

    function fnAllProductsSaveSODetail(productId) {
        if ($('#formSODetail').jqxValidator('validate')) {            
            var quantity = $('#txtQuantity').autoNumeric('get');
            var price = $('#txtPrice').autoNumeric('get');
            if (price == '') {
                price = 0;
            }
            var discount = $('#txtDiscount').autoNumeric('get');
            if (discount == '') {
                discount = 0;
            }
            var VATRate = $('#txtVATRate').autoNumeric('get');
            if (VATRate == '') {
                VATRate = 0;
            }
            var description = $('#txtDescriptionDetail').val();
            if (description != '')
                description = Base64.encode($('#txtDescriptionDetail').val());

            $.post(appPath + 'Sales/AddSalesOrderDetail', {
                id: $('#txtDetailID').val(), productId: productId,
                unitId: $('#slProductUnits').val(),
                description: description, quantity: quantity,
                price: price, discount: discount, VATRate: VATRate
            }, function (data) {
                $('#tbodyDetails').html(data);
                $('#lbAmount').html($('#txtLineAmount').val());
                $('#lbTotalVATRate').html($('#txtTaxAmount').val());
                $('#lbTotalDiscount').html($('#txtDiscountAmount').val());
                $('#lbTotalAmount').html($('#txtTotalAmount').val());
                fnResetSalesOrderDetail();
            });
        }
    }

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