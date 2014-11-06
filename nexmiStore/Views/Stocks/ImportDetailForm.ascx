<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<tr>
    <td width="22%" >
<%
    NEXMI.ImportDetails Detail = (NEXMI.ImportDetails)ViewData["Detail"];
    if (Detail == null)
    {
        Detail = new NEXMI.ImportDetails();
        Detail.GoodQuantity = 1;
    }
%>
    <%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = Detail.ProductId, label = NEXMI.NMCommon.GetName(Detail.ProductId, langId) });%>
    <input type="hidden" id="txtOrdinalNumber" value="<%=Detail.OrdinalNumber%>" />
    </td>
    <td><label id="lbUnitName"></label>
        <%Html.RenderPartial("slUnits");%>
    </td>
    <td><input type="text" id="txtGoodQuantity" value="<%=Detail.GoodQuantity%>" style="width: 100px;" /></td>
    <td><input type="text" id="txtBadQuantity" value="<%=Detail.BadQuantity%>" style="width: 100px;" /></td>
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
    <td><input type="text" id="txtDetailDescription" style="width: 480px;" value="<%=Detail.Description%>" /></td>
    <td class="actionCols">
        <button type="button" class="btActions save" onclick="javascript:fnSaveImportDetail()"></button>
        <button type="button" class="btActions reset" onclick="javascript:fnResetFormImportDetail()"></button>
    </td>
</tr>
<script type="text/javascript">
    $(function () {
        $("#txtGoodQuantity, #txtBadQuantity").autoNumeric('init', { mDec: '3' });
        $("#txtSNs").tagit();
        $("#formImportDetail").jqxValidator({
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
                        if (value.split(',').length != parseInt($('#txtGoodQuantity').autoNumeric('get'))) {
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