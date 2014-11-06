<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<tr>
    <td>
        <% 
            NEXMI.SalesInvoiceDetails Detail = (NEXMI.SalesInvoiceDetails)ViewData["Detail"];
            if (Detail == null)
            {
                Detail = new NEXMI.SalesInvoiceDetails();
                Detail.Quantity = 1;
            }
            bool inputprice = (bool)ViewData["PriceInput"];
        %>
        <script type="text/javascript">
            $(function () {
                $("#formSIDetail").jqxValidator({
                    rules: [
                        { input: '#cbbProducts', message: 'Bạn chưa chọn sản phẩm', action: 'change', rule: function (input) {
                            if ($(input).jqxComboBox('getSelectedItem') == null)
                                return false;
                            return true;
                        }
                        }
                    ]
                });
                $("#txtQuantity, #txtPrice").autoNumeric('init', { mDec: '3' });
                $("#txtDiscount, #txtVATRate").autoNumeric('init', { vMin: '0.00', vMax: '100.00'});
                $("#txtPrice, #txtQuantity, #txtDiscount, #txtVATRate").on("keyup change", function () {
                    calculateAmount();
                });
                $('#cbbProducts').find(".jqx-combobox-input").focus();
            });
            function calculateAmount() {
                var quantity = $("#txtQuantity").autoNumeric('get');
                if (quantity == "") quantity = 0;
                var price = $("#txtPrice").autoNumeric('get');
                if (price == "") price = 0;
                $("#txtAmount").val(fnNumberFormat(quantity * price));
            }
        </script>
        <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = Detail.ProductId, label = NEXMI.NMCommon.GetName(Detail.ProductId, langId) });%>
    </td>
    <td>
        <label id="lbUnitName"></label>
        <%Html.RenderPartial("slUnits");%>
    </td>
    <td><input type="text" id="txtQuantity" value="<%=Detail.Quantity%>" style="width: 120px;" class="auto" /></td>
<%if (inputprice)
    { %>
    <td><input type="text" id="txtPrice" value="<%=Detail.Price%>" style="width: 120px;" /></td>
    <%}
      else
      { %>
    <td><input type="text" id="txtPrice" value="<%=Detail.Price%>" style="width: 120px;" readonly="readonly" /></td>
    <%} %>
    <td><input type="text" id="txtDiscount" value="<%=Detail.Discount%>" style="width: 120px;" /></td>
    <td><input type="text" id="txtVATRate" value="<%=Detail.Tax%>" style="width: 120px;" /></td>
    <td><input type="text" id="txtAmount" value="<%=Detail.Amount%>" style="width: 120px;" readonly="readonly" /></td>
    <td class="actionCols">
        <input type='hidden' id='txtOrdinalNumber' value='<%=Detail.OrdinalNumber %>' />
        <button type="button" class="btActions save" onclick="javascript:fnSaveSalesInvoiceDetail()" title='Bấm lưu để ghi nhận sản phẩm này.'></button>
        <button type="button" class="btActions reset" onclick="javascript:fnRemoveSalesInvoiceDetail()" title='Bấm reset để xóa thông tin trong form này.'></button>
    </td>
</tr>