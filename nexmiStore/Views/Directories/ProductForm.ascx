<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string pos = "absolute", windowId = "";
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnSaveOrUpdateProduct('')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:fnSaveOrUpdateProduct('', '1')" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_NEW", langId) %></button>
                    <button onclick="javascript:history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                </div>
            </td>
            <td align="right">
                <%Html.RenderPartial("ProductSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
<% 
    }
    else
    {
        windowId = ViewData["WindowId"].ToString();
    }
%>
<div class="divStatus">
</div>
<div class='divContentDetails'>
<div class="windowContent" style="position: <%=pos%> !important;">
    <script type="text/javascript">
        $(document).ready(function () {
            <%if (!NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.PRICE_BY_STORE))
              { %>
                $('#txtPrice<%=windowId%>').autoNumeric('init', { vMax: '1000000000000' });
            <%} %>
            //$('#txtPrice<%=windowId%>, #txtCostPrice<%=windowId%>').autoNumeric('init', { vMax: '1000000000000' });
            $('#txtVAT<%=windowId%>').autoNumeric('init', { vMax: '100' });
            $('#formProduct<%=windowId%>').jqxValidator({
                rules: [
                    { input: '#txtProductCode<%=windowId%>', message: 'Nhập mã sản phẩm', action: 'keyup, blur', rule: 'required' },
                    { input: '#txtProductName<%=windowId%>vi', message: 'Nhập tên sản phẩm', action: 'keyup, blur', rule: 'required' }
                ]
            });
            $('#checkAllImages<%=windowId%>').click(function () {
                var elms = document.getElementsByName('cbImage<%=windowId%>');
                if ($(this).is(":checked")) {
                    for (var x in elms) {
                        elms[x].checked = true;
                    }
                }
                else {
                    for (var y in elms) {
                        elms[y].checked = false;
                    }
                }
            });
            $('#productTabs<%=windowId%>').jqxTabs({ theme: theme, keyboardNavigation: false });
            $('#txtId<%=windowId%>').focus();
        });

        function fnAddToList(fileName) {
            var row = '';
            row += '<td><input type="checkbox" name="cbImage<%=windowId%>" id="' + fileName + '" /></td>';
            row += '<td><img alt="" src="<%=Url.Content("~")%>uploads/_thumbs/' + fileName + '" /></td>';
            row += '<td><input type="radio" name="rdIsDefault<%=windowId%>" value="' + fileName + '" /></td>';
            row += '<td><a href="javascript:fnDeleteItem(\'' + fileName + '\');"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></td>';
            $('#tbImageList tbody').append('<tr id="row<%=windowId%>' + fileName + '">' + row + '</tr>');
        }

        function fnDeleteItem(fileName) {
            if (confirm("<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> hình này?")) {
                fnDelete(fileName);
            }
        }

        function fnDeleteSelectedItems() {
            if (confirm("<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> hình đã chọn?")) {
                var elms = document.getElementsByName('cbImage<%=windowId%>');
                for (var x = 0; x < elms.length; x++) {
                    if (elms[x].checked) {
                        fnDelete(elms[x].id);
                    }
                }
            }
        }

        function fnDelete(fileName) {
            $.post(appPath + 'Common/DeleteFile', { fileName: fileName, type: 'Images', owner: '<%=ViewData["ProductId"]%>' }, function (data) {
                if (data == '') {
                    var elm = document.getElementById('row<%=windowId%>' + fileName);
                    elm.parentNode.removeChild(elm);
                } else {
                    alert(data);
                }
            });
        }

        function fnSaveOrUpdateProduct(mode, saveMode) {
            if ($('#formProduct<%=windowId%>').jqxValidator('validate')) {
                var data = {};
                data.id = $('#txtProductId<%=windowId%>').val();
                data.productCode = $('#txtProductCode<%=windowId%>').val();
                data.unit = $('#slProductUnits').val();
                data.productCategory = ($('#cbbCategories<%=windowId%>').jqxComboBox('getSelectedItem') == null) ? "" : $('#cbbCategories<%=windowId%>').jqxComboBox('getSelectedItem').value;
                data.manufactureId = ($('#cbbCustomers<%=windowId%>').jqxComboBox('getSelectedItem') == null) ? "" : $('#cbbCustomers<%=windowId%>').jqxComboBox('getSelectedItem').value;
                data.VAT = $('#txtVAT<%=windowId%>').autoNumeric('get');
                <%if (!NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.PRICE_BY_STORE))
                  { %>
                data.price = $('#txtPrice<%=windowId%>').autoNumeric('get');
                <%} %>
                //data.costPrice = $('#txtCostPrice<%=windowId%>').autoNumeric('get');
                data.barCode = $('#txtBarCode<%=windowId%>').val();
                data.typeId = $('#slProductTypes').val();
                data.pgroupId = $('#txtGroupId<%=windowId%>').val();
                data.defaultDiscount = $('#txtDiscount<%=windowId%>').val();
                var images = '';
                var fileName = '';
                var imageDescription = '';
                var isDefault = '';
                var elms = document.getElementsByName('cbImage<%=windowId%>');
                for (var x = 0; x < elms.length; x++) {
                    fileName = elms[x].id;
                    imageDescription = '';
                    isDefault = 'false';
                    if ($('input[name=rdIsDefault<%=windowId%>]:checked').val() == fileName) {
                        isDefault = 'true';
                    }
                    if (images != '') {
                        images += 'ROWS';
                    }
                    images += fileName + 'COLS' + imageDescription + 'COLS' + isDefault;
                }
                data.images = images;
                data.isDiscontinued = (document.getElementById('cbDiscontinued<%=windowId%>').checked) ? 'true' : 'false';
                data.isHighlight = (document.getElementById('cbHighlight<%=windowId%>').checked) ? 'true' : 'false';
                data.isNew = (document.getElementById('cbNew<%=windowId%>').checked) ? 'true' : 'false';
                data.isEmpty = (document.getElementById('cbEmpty<%=windowId%>').checked) ? 'true' : 'false';
                var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
                for (var i = 0; i < allLanguageId.length - 1; i++) {
                    var temp = allLanguageId[i];
                    data['name' + temp] = $('#txtProductName<%=windowId%>' + temp).val();
                    data['warranty' + temp] = $('#txtWarranty<%=windowId%>' + temp).val();
                    data['shortDescription' + temp] = $('#txtProductShort<%=windowId%>' + temp).val();
                    data['description' + temp] = Base64.encode(CKEDITOR.instances['txtProductDescription<%=windowId%>' + temp].getData());
                }
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Directories/SaveOrUpdateProduct',
                    data: data,
                    beforeSend: function () {
                        fnDoing();
                    },
                    success: function (data) {
                        if (data.error == "") {
                            fnSuccess();
                            if (mode == 'popup') {
                                fnSetItemForCombobox('<%=ViewData["ParentId"]%>', data.id, Base64.encode(name));
                                closeWindow('<%=windowId%>');
                            } else {
                                if (saveMode == '1')
                                    fnResetFormProduct();
                                else
                                    fnLoadProductDetail(data.id);
                            }
                        }
                        else {
                            alert(data.error);
                        }
                    },
                    error: function () {
                        fnError();
                    },
                    complete: function () {
                        fnComplete();
                    }
                });
            } else {
                alert('Dữ liệu nhập không đúng!\nKiểm tra các ô bị đánh dấu!');
            }
        }

        function fnResetFormProduct() {
            $('#txtDescription<%=windowId%>').val('');
            $('.txtDescription<%=windowId%>').html('');
            $('#productTabs<%=windowId%>').jqxTabs({ selectedItem: 0 });
            $('#txtProductId<%=windowId%>').val('');
            $('#txtProductCode<%=windowId%>').val('');
            $('#txtProductName<%=windowId%>').val('');
            $('#txtPrice<%=windowId%>').autoNumeric('set', '');
            $('#txtVAT<%=windowId%>').autoNumeric('set', '');
            $('#txtDiscount<%=windowId%>').val('');
            $('#tbImageList<%=windowId%> tbody').html('');
            document.getElementById('cbDiscontinued<%=windowId%>').checked = false;
            document.getElementById('cbHighlight<%=windowId%>').checked = false;
            document.getElementById('cbNew<%=windowId%>').checked = false;
            document.getElementById('cbEmpty<%=windowId%>').checked = false;
            var allLanguageId = '<%=NEXMI.NMCommon.GetAllLanguageId()%>'.split(';');
            for (var i = 0; i < allLanguageId.length - 1; i++) {
                $('#txtProductName<%=windowId%>' + allLanguageId[i]).val('');
                $('#txtWarranty<%=windowId%>' + allLanguageId[i]).val('');
                $('#txtProductShort<%=windowId%>' + allLanguageId[i]).val('');
                CKEDITOR.instances['txtProductDescription<%=windowId%>' + allLanguageId[i]].setData('');
            }
            $('#txtProductCode<%=windowId%>').focus();
        }
    </script>
    <div>
        <form id="formProduct<%=windowId%>" action="">
            <div id="productTabs<%=windowId%>">
                <ul>
                    <li style="margin-left: 25px;">Thông tin chung</li>
                    <li><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></li>
                </ul>
                <div style="padding: 10px;">
                    <table style="width: 100%" class="frmInput">
                        <tr>
                            <td class="lbright">Mã sản phẩm</td>
                            <td>
                                <input type="hidden" id="txtProductId<%=windowId%>" value="<%=ViewData["ProductId"]%>" />    
                                <input type="hidden" id="txtGroupId<%=windowId%>" value="<%=ViewData["GroupId"]%>" />
                                <input type="text" id="txtProductCode<%=windowId%>" value="<%=ViewData["ProductCode"]%>" />
                            </td>
                            <td class="lbright">Nhóm sản phẩm</td>
                            <td><%Html.RenderAction("cbbCategories", "UserControl", new { currentId = ViewData["CategoryId"], currentName = ViewData["CategoryName"], elementId = windowId, objectName = "Products" });%></td>
                        </tr>
                        <tr>
                            <td class="lbright">Đơn vị tính</td>
                            <td><%Html.RenderPartial("slUnits");%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("VAT", langId) %></td>
                            <td><input type="text" id="txtVAT<%=windowId%>" value="<%=ViewData["VAT"]%>" /></td>
                        </tr>
                    <% if(!NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.PRICE_BY_STORE)) {%>
                        <tr>
                            <%--<td class="lbright">Giá gốc</td>--%>
                            <%--<td><input type="text" id="txtCostPrice<%=windowId%>" value="<%=ViewData["CostPrice"]%>" /></td>--%>
                            <td class="lbright">Giá bán</td>
                            <td><input type="text" id="txtPrice<%=windowId%>" value="<%=ViewData["Price"]%>" /></td>
                            <td></td><td></td>
                        </tr>
                    <%} %>
                        <tr>
                            <td class="lbright">Nhà sản xuất</td>
                            <td><%Html.RenderAction("cbbCustomers", "UserControl", new { cgroupId = NEXMI.NMConstant.CustomerGroups.Manufacturer, customerId = ViewData["CustomerId"], customerName = ViewData["CustomerName"], elementId = windowId });%></td>
                            <td class="lbright">Mã vạch</td>
                            <td><input type="text" id="txtBarCode<%=windowId%>" value="<%=ViewData["BarCode"]%>" /></td>
                        </tr>
                        <tr>
                            <%--<td class="lbright"><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</td>
                            <td><input type="text" id="txtDiscount<%=windowId%>" value="<%=ViewData["Discount"]%>" /></td>--%>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("KIND", langId) %></td>
                            <td><%Html.RenderAction("slInvoiceTypes", "UserControl", new { elementId = "slProductTypes", objectName = "PurchaseInvoices", value = ViewData["TypeId"] });%></td>
                        </tr>
                        <tr>
                            <td class="lbright"></td>
                            <td colspan="3">
                                <input type="checkbox" id="cbNew<%=windowId%>" <%=ViewData["IsNew"]%> />
                                <label for="cbNew<%=windowId%>"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %> mới</label>
                                &nbsp;
                                <input type="checkbox" id="cbHighlight<%=windowId%>" <%=ViewData["Highlight"]%> />
                                <label for="cbHighlight<%=windowId%>">Nổi bật</label>
                                &nbsp;
                                <input type="checkbox" id="cbEmpty<%=windowId%>" <%=ViewData["IsEmpty"]%> />
                                <label for="cbEmpty<%=windowId%>">Hết hàng</label>
                                &nbsp;
                                <input type="checkbox" id="cbDiscontinued<%=windowId%>" <%=ViewData["Discontinued"]%> />
                                <label for="cbDiscontinued<%=windowId%>">Ngưng bán</label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <%Html.RenderAction("ContentLanguagesForm", "UserControl", new { mode = "Product", data = ViewData["ProductId"], windowId = windowId });%>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding: 10px;">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 20%" valign="top"><% Html.RenderPartial("UploadUserControl"); %></td>
                            <td valign="top">
                                <button type="button" onclick="javascript:fnDeleteSelectedItems()" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %> ảnh đã chọn</button>
                                <table style="width: 100%" class="list" id="tbImageList<%=windowId%>">
                                    <thead>
                                        <tr>
                                            <th><input type="checkbox" id="checkAllImages<%=windowId%>" /></th>
                                            <th></th>
                                            <th>Ảnh chính</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <%
                                            if (ViewData["ImageWSIs"] != null)
                                            {
                                                List<NEXMI.NMImagesWSI> ImageWSIs = (List<NEXMI.NMImagesWSI>)ViewData["ImageWSIs"];
                                                string isDefault;
                                                foreach (NEXMI.NMImagesWSI Item in ImageWSIs)
                                                {
                                                    isDefault = "";
                                                    if (Item.IsDefault == "True")
                                                    {
                                                        isDefault = "checked";
                                                    }
                                        %>
                                        <tr id="row<%=Item.Name%>">
                                            <td><input type="checkbox" name="cbImage<%=windowId%>" id="<%=Item.Name%>" /></td>
                                            <td><img alt="" src="<%=Url.Content("~")%>uploads/_thumbs/<%=Item.Name%>" /></td>
                                            <td><input type="radio" name="rdIsDefault<%=windowId%>" value="<%=Item.Name%>" <%=isDefault%> /></td>
                                            <td class="actionCols"><a href="javascript:fnDeleteItem('<%=Item.Name%>')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></td>
                                        </tr>
                                        <%
                                                }
                                            }
                                        %>
                                    </tbody>
                                </table>            
                            </td>
                        </tr>
                    </table>
                </div>
            </div>   
        </form>
    </div>
</div>
</div>
<% 
    if (windowId != "")
    {
%>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSaveOrUpdateProduct('popup')" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:fnResetFormProduct()" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLEAR_ALL", langId) %></button>
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>
<% 
    }    
%>
</div>