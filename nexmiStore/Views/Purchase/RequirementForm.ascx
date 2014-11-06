<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Requirements.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        //$("#tabs").tabs();
        $("#txtRequireDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $("#formRequirement").jqxValidator({
            rules: [
                        { input: '#txtResponseDays', message: 'Không được để trống.', action: 'keyup, blur', rule:
                                function (input, commit) {
                                    if ($(input).val() == '')
                                        return false;
                                    return true;
                                }
                        }
                    ]
        });
    });

    function fnSaveOrUpdateRequirement(saveMode) {
        if ($('#tbodyDetails tr').children().length > 0) {
            if ($('#formRequirement').jqxValidator('validate')) {
                var customer = $('#cbbCustomers').jqxComboBox('getSelectedItem');
                var customerId = '';
                if(customer != null)
                    customerId = customer.value;
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Purchase/SaveOrUpdateRequirement',
                    data: {
                        id: $('#txtId').text(),
                        type: $('#slRequirementTypes').val(),
                        date: $('#txtRequireDate').val(),
                        customer: customerId,
                        order: $('#txtReference').val(),
                        requireBy: $('#slUsers').val(),
                        responseDay: $('#txtResponseDays').val(),
                        status: $('#txtStatus').val(),
                        description: $('#txtDescription').val()
                    },
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == '') {
                            fnSuccess();
                            if (saveMode == '1')
                                fnResetFormRequirement();
                            else
                                fnLoadRequirementDetail(data.result);
                        }
                        else alert(data.error);
                    },
                    error: function () {
                        fnError();
                    },
                    complete: function () {
                        fnComplete();
                    }
                });
            }
            else {
                alert('Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!');
            }
        }
        else {
            alert('Không có sản phẩm nào');
        }
    }

    function fnResetFormRequirement() {
        $.post(appPath + 'Common/ClearSession', { sessionName: 'Details' });
        
        $('#tbodyDetails').html('');
    }

    function fnShowRequirementDetailForm(productId) {
        $.get(appPath + 'Purchase/RequirementDetailForm', { productId: productId }, function (data) {
            $('#formRequirementDetail').html(data); //.show();
        });
    }

    function fnSaveRequirementDetail() {
        if ($('#formRequirementDetail').jqxValidator('validate')) {
            var Product = $('#cbbProducts').jqxComboBox('getSelectedItem');
            var productId = Product.value;
            var productName = Product.label;
            var quantity = $('#txtQuantity').autoNumeric('get');
            var price = $('#txtPrice').autoNumeric('get');
            var description = $('#txtDetailDescription').val();
            $.post(appPath + 'Purchase/AddRQDetail', { productId: productId, quantity: quantity, price: price, description: description }, 
            function (data) {
                if (data.strError != '') {
                    alert(data.strError);
                }
                else {
                    var elm = document.getElementById(productId);
                    var row = '';
                    row = '<tr id="' + productId + '">';
                    row += '<td>' + productName + '</td>';  //tên
                    row += "<td>" + $('#lbUnitName').text() + "</td>"; // dvt
                    row += '<td>' + fnNumberFormat(quantity) + '</td>';
                    row += '<td>' + fnNumberFormat(price) + '</td>';
                    row += '<td align="right">' + data.detailAmount + '</td>';
                    row += '<td>' + description + '</td>';
                    row += "<td class='actionCols'><button type='button' class='btActions update' onclick='javascript:fnShowRequirementDetailForm(\"" + productId + "\")'></button> <button type='button' class='btActions delete' onclick='javascript:fnRemoveRequirementDetail(\"" + productId + "\")'></button></td>";
                    row += '</tr>';
                    if (elm == undefined) {
                        $('#tbRequirementDetail tbody').append(row);
                    }
                    else {
                        $('#' + productId).replaceWith(row);
                    }
                    $('#lbTotalAmount').html(data.totalAmount);
                    fnResetFormRequirementDetail();
                }
            });
        }
    }

    function fnRemoveRequirementDetail(productId) {
        $.post(appPath + 'Purchase/RemoveRequirementDetail', { productId: productId }, function (data) {
            var totalDiscount = $('#lbTotalDiscount').html().replace(/,/g, '');
            if (totalDiscount == '') {
                totalDiscount = 0;
            }
            
            totalAmount = parseFloat(totalAmount) - parseFloat(amount);            
            $('#lbTotalAmount').html(fnNumberFormat(totalAmount));
            calculateTotalAmount();
            $('#' + productId).remove();
        });
    }

    function fnResetFormRequirementDetail() {
        $('#cbbProducts').jqxComboBox('clearSelection');
        $('#txtDetailDescription').val('');
        $('#txtQuantity').autoNumeric('set', '1.00');
        $('#txtAmount').val('0.00');        
    }
        
</script>
<% 
    NEXMI.NMRequirementsWSI WSI = (NEXMI.NMRequirementsWSI)ViewData["WSI"];
    string id = WSI.Requirement.Id;
%>
<input type="hidden" id="txtStatus" value="<%=WSI.Requirement.Status%>" />
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <button onclick="javascript:fnSaveOrUpdateRequirement()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                        <button onclick="javascript:fnSaveOrUpdateRequirement('1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                        <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                    </div>
                </td>
                <td align="right">
                    <%//Html.RenderPartial("SalesOrderSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
            </div>     
            <div class="divStatusBar">
                <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Requirements", current = WSI.Requirement.Status });%>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formRequirement" class="form-to-validate" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId">Đề xuất vật tư <label id="txtId"><%=id%></label></label>                        
                        <table class="frmInput">
                            <tr>
                                <td class="lbright" >Lý do đề xuất</td>
                                <td><%Html.RenderAction("slTypes", "UserControl", new { elementId = "slRequirementTypes", objectName = "RequirementTypes", value = WSI.Requirement.RequirementTypeId });%></td>
                                <td class="lbright">Ngày đề xuất</td>
                                <td><input type="text"  id="txtRequireDate" value="<%=WSI.Requirement.RequireDate.ToString("yyyy-MM-dd") %>" /></td>                                
                            </tr>
                            <tr>
                                <td class="lbright" ><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>                                
                                <td><%Html.RenderAction("cbbCustomers", "UserControl", new { customerId = WSI.Requirement.CustomerId, customerName = NEXMI.NMCommon.GetCustomerName(WSI.Requirement.CustomerId), cgroupId = NEXMI.NMConstant.CustomerGroups.Customer });%></td>
                                <td class="lbright">Theo hợp đồng/đơn hàng số</td>
                                <td><input type="text" id="txtReference" value="<%=WSI.Requirement.OrderId %>" /></td>                                
                            </tr>
                            <tr>
                                <td class="lbright">Người đề xuất</td>
                                <td><%Html.RenderAction("slUsers", "UserControl", new { elementId = "slUsers", userId = WSI.Requirement.RequiredBy }); %></td>
                                <td class="lbright">Thời gian đáp ứng</td>
                                <td><input type="text" id="txtResponseDays" value="<%=WSI.Requirement.ResponseDays %>" /> (ngày)</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div class="tabs">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" class="tbDetails" id='tbRequirementDetail'>
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <% 
                                            List<NEXMI.RequirementDetails> Details = (List<NEXMI.RequirementDetails>)Session["Details"];
                                            if (Details != null)
                                            foreach (NEXMI.RequirementDetails Item in Details)
                                            {
                                        %>
                                        <tr id="<%=Item.ProductId%>">
                                            <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetProductUnitName(Item.ProductId)%></td>
                                            <td><%=Item.Quantity.ToString("N3")%></td>
                                            <td><%=Item.Price.ToString("N3")%></td>
                                            <td><%=Item.Amount.ToString("N3")%></td>
                                            <td><%=Item.Description%></td>
                                            <td>
                                                <button type="button" class="btActions update" onclick="javascript:fnShowRequirementDetail('<%=Item.ProductId%>')"></button>
                                                <button type="button" class="btActions delete" onclick="javascript:fnRemoveRequirementDetail('<%=Item.ProductId%>')"></button>
                                            </td>
                                        </tr>
                                        <% 
                                            }    
                                        %>
                                    </tbody>
                                    <tfoot id="formRequirementDetail" class="form-to-validate">
                                        <%Html.RenderAction("RequirementDetailForm", "Purchase");%>
                                    </tfoot>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td><textarea id="txtDescription" rows="5" cols="0" style="width: 100%" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."><%=WSI.Requirement.Description%></textarea></td>
                                        <td valign="top">
                                            <table style="width: 100%">                                                
                                                <tr>
                                                    <td colspan="6" align="right"><b>Tổng giá trị: </b></td>
                                                    <td align="right"><label id="lbTotalAmount"><%=WSI.Requirement.Amount.ToString("N3")%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"></td>
                            <td></td>
                            <td class="lbright"></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
        </div>
    </div>
</div>