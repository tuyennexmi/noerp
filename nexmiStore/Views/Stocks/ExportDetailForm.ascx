<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<td>
<% 
    NEXMI.ExportDetails Detail = (NEXMI.ExportDetails)ViewData["Detail"];
    if (Detail == null)
    {
        Detail = new NEXMI.ExportDetails();
        Detail.Quantity = 1;
    }
%>
    <input type="hidden" id="txtOrdinalNumber" value="<%=Detail.OrdinalNumber%>" />
    <script type="text/javascript">
        $(function () {
            $("#txtQuantity").autoNumeric('init', { mDec: '3' });
            $("#txtSNs").tagit();
            $("#formExportDetail").jqxValidator({
                rules: [
                    { input: '#cbbProducts', message: 'Bạn chưa chọn sản phẩm', action: 'change', rule: function (input) {
                        if ($(input).jqxComboBox('getSelectedItem') == null)
                            return false;
                        return true;
                    }
                },
                { input: '#txtSNs', message: 'Bạn chưa nhập đủ số serial', rule: function (input) {
                    var value = $(input).val();
                    if (value != '') {
                        if (value.split(',').length != parseInt($('#txtQuantity').autoNumeric('get'))) {
                            alert('Bạn chưa nhập đủ số serial');
                            //$('#txtSNs').tagit('tagInput').focus();
                            return false;
                        }
                    }
                    return true;
                }
                }
                ]
            });
        });
    </script>
    <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = Detail.ProductId, label = NEXMI.NMCommon.GetName(Detail.ProductId, langId) });%>
</td>
<td>
    <label id="lbUnitName"><%=NEXMI.NMCommon.GetUnitNameById(Detail.UnitId)%></label>
    <%Html.RenderPartial("slUnits");%>
</td>
<td><input type="text" id="txtQuantity" value="<%=Detail.Quantity%>" style="width: 120px;" /></td>
<td><input type="text" readonly="readonly" id="txtGoodQuantityInStock" style="width: 120px;" /></td>
<% 
if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
{
%>
    <td><input type="text" id="txtSNs" value="<%=Session["SNs" + Detail.ProductId]%>" /></td>
<% 
}
else
{ 
%>
    <td style="display: none;"><label id="txtSNs"></label></td>
<%
}
%>
<td><input type="text" id="txtDetailDescription" style="width: 460px;" /></td>
<td class="actionCols">
    <button type="button" class="btActions save" onclick="javascript:fnSaveExportProductDetail()"></button>
    <button type="button" class="btActions reset" onclick="javascript:fnResetFormExportDetail()"></button>
</td>