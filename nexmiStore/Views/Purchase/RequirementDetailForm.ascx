<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<tr>
    <td>
        <% 
            NEXMI.RequirementDetails Detail = (NEXMI.RequirementDetails)ViewData["Detail"];
            if (Detail == null)
            {
                Detail = new NEXMI.RequirementDetails();
                Detail.Quantity = 1;
            }
            string productId = Detail.ProductId;
        %>
        <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = productId, label = NEXMI.NMCommon.GetName(productId, langId) });%>
    </td>
    <td><label id="lbUnitName"><%=NEXMI.NMCommon.GetProductUnitName(Detail.ProductId)%></label></td>
    <td><input type="text" id="txtQuantity" value="<%=Detail.Quantity.ToString("N3")%>" style="width: 100px;" /></td>
    <td><input type="text" id="txtPrice" value="<%=Detail.Price.ToString("N3")%>" style="width: 100px;"/> </td>
    <td><input type="text" id="txtAmount" value="<%=Detail.Amount.ToString("N3")%>" style="width: 100px;" readonly="readonly" /></td>
    <td><input type="text" id="txtDetailDescription" value="<%=Detail.Description%>" style="width: 280px;" /></td>
    <td class="actionCols">
        <button type="button" class="btActions save" onclick="javascript:fnSaveRequirementDetail()"></button>
        <button type="button" class="btActions reset" onclick="javascript:fnResetRequirementDetail()"></button>
    </td>
</tr>
<script type="text/javascript">
    $(function () {
        $("#formRequirementDetail").jqxValidator({
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
        $('#txtQuantity, #txtPrice').on('keyup', function () {
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