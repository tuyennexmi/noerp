<%-- ManufactureOrder.ascx

* http://nexmi.com
* NoErp Project - Nexmi Open ERP
* Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
* This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*
--%>

<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript" src="<%=Url.Content("~") %>Scripts/forApp/Manufacturing.js"></script>

<script type="text/javascript">
    $(document).ready(function () {

        $("#dtFrom dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#txtTotal').autoNumeric('init', { vMax: '100000000000000' });

        $("#formManufactureOrder").jqxValidator({
            rules: [
                        { input: '#slManagedBy', message: 'Chọn người quản lý.', action: 'change', rule: function (input) {
                            if ($(input).val() == null)
                                return false;
                            return true;
                        }
                    },
                        { input: '#cbbProducts', message: 'Chọn sản phẩm cần sản xuất.', action: 'change', rule: function (input) {
                            if ($(input).jqxComboBox('getSelectedItem') == null)
                                return false;
                            return true;
                        }
                        },
                        
                        { input: '#txtTotal', message: 'Số lượng sản xuất phải lớn hơn 0.', action: 'keyup, blur', rule: function (input, commit) {
                            var amount = $('#txtTotal').autoNumeric('get');
                            var result = amount > 0;
                            return (amount > 0);
                        }
                        }
                    ]
        });
    });

    function fnSaveMO() {
        if ($("#formManufactureOrder").jqxValidator('validate')) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Manufacture/SaveManufactureOrder',
                data: {
                    id: $('#txtId').text(),
                    product: document.getElementsByName('cbbProducts')[0].value,
                    start: $('#dtFrom').val(),
                    description: $('#txtDescription').val(),
                    quantity: $('#txtTotal').val(),
                    reference: $('#txtReference').val(),
                    status: $('#txtStatus').val(),
                    managedBy: document.getElementsByName('slManagedBy')[0].value
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data.error == '') {
                        fnSuccess();
                        fnLoadMODetail(data.id);
                    }
                    else alert(data.error);
                },
                error: function () {
                    alert("Đã xảy ra lỗi nên không lưu được!");
                },
                complete: function () {
                    fnComplete();
                }
            });
        } else {
            alert("Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!");
        }
    }

    function fnMaterialCalculate() {
        LoadContentDynamic('MaterialsDiv', 'Manufacture/MaterialCalculate', {
            productId: document.getElementsByName('cbbProducts')[0].value,
            quantity: $('#txtTotal').val()
         });
    }

</script>
<%        NEXMI.NMManufactureOrdersWSI wsi = (NEXMI.NMManufactureOrdersWSI)ViewData["MO"];
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr><td></td></tr>
        <tr>
            <td>
                <button onclick="javascript:fnSaveMO()" class="color red button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">            
            <table>
                <tr>
                    
                </tr>
            </table>                        
        </div>
        <div class="divStatusBar">
            <input type="hidden" id="txtStatus" value="<%=wsi.ManufactureOrder.Status%>" />
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "ManufactureOrder", current = NEXMI.NMConstant.MOStatuses.Draft });%>
        </div>
    </div>
    
    <div class='divContentDetails'>
    <form id="formManufactureOrder" class="form-to-validate" action="" style="margin: 10px;">
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId">Lệnh sản xuất <label id="txtId"><%=wsi.ManufactureOrder.Id %></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
                            <td><%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = wsi.ManufactureOrder.ProductId, label = "" });%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                            <td><input type="text" id="txtReference" value="<%= wsi.ManufactureOrder.SaleReference %>"/></td>
                        </tr>
                        <tr>
                            <td class="lbright">Sản lượng</td>
                            <td><input type="text" id="txtTotal" value="<%= wsi.ManufactureOrder.Quantity %>" /></td>                            
                            <td class="lbright">Chịu trách nhiệm</td>                            
                            <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slManagedBy", userId = wsi.ManufactureOrder.ManagedBy }); %></td>
                            
                        </tr>
                        <tr>
                            <td class="lbright">Ngày dự kiến</td>
                            <td><input type="text"  id="dtFrom" value="<%=wsi.ManufactureOrder.StartDate.ToString("yyyy-MM-dd") %>" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                            <td><textarea id="txtDescription" rows="2 cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=wsi.ManufactureOrder.Descriptions %></textarea></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
    <div class="tabs" id='detailTabs'>
        <ul>
            <li style="margin-left: 25px;">Vật tư</li>
            <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
        </ul>
        <div>
            <table style="width: 88%" class="tbTemplate">    
                <tr>
                    <td>
                        <button onclick="javascript:fnMaterialCalculate()" class="color red button">Tính vật tư</button>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <div id="MaterialsDiv">
                            <%Html.RenderAction("MaterialCalculate", new { productId = wsi.ManufactureOrder.ProductId, quantity = wsi.ManufactureOrder.Quantity });%>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td valign="top">
                        <table class="frmInput">
                            <tr>
                                <td class="lbright">Ngày kết thúc</td>
                                <td><input type="text"  id="dtTo" value="<%=wsi.ManufactureOrder.StartDate.ToString("yyyy-MM-dd") %>" /></td>
                                <td class="lbright">Người duyệt</td>
                                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slApprovalBy", userId = wsi.ManufactureOrder.ApprovalBy }); %></td>
                            </tr>
                            <tr>
                                <td class="lbright">Độ ưu tiên</td>
                                <td><input type="text"  id="txtPriority" value="" /></td>
                                <td class="lbright">Khách hàng</td>
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.Customer, customerId = wsi.ManufactureOrder.CustomerId, customerName = "" });%></td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <div id="SalesPlanDiv">
                            <%--<%Html.RenderAction("SalesByProductsUC");%>--%>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </div>
</div>