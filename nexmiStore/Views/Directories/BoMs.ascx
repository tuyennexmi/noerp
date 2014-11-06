<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div>
    <% 
        string elementId = ViewData["ElementId"].ToString();    
    %>
    <input type="hidden" id="txtProductId<%=elementId%>" value="<%=ViewData["ProductId"]%>" />
    <script type="text/javascript">
        $(function () {
            //$('#txtQuantity<%=elementId%>').autoNumeric('init', { mDec: '3' });
            $('#tbodyDetails<%=elementId%>').find('.btActions').hide();
            $('#formBoMLine<%=elementId%>').jqxValidator({
                rules: [
                    { input: '#cbbProducts<%=elementId%>', message: 'Chọn sản phẩm', action: 'change', rule: function (input) {
                        if ($(input).jqxComboBox('getSelectedItem') == null)
                            return false;
                        return true;
                    }
                    },
                    { input: '#txtQuantity<%=elementId%>', message: 'Nhập số lượng', action: 'keyup, blur', rule:
                            function (input, commit) {
                                if ($(input).val() == '')
                                    return false;
                                return true;
                            }
                    }
                ]
            });
        });

        function fnSaveLine() {
            if ($('#formBoMLine<%=elementId%>').jqxValidator('validate')) {
                var parentId = $('#txtProductId<%=elementId%>').val();
                var product = $('#cbbProducts<%=elementId%>').jqxComboBox('getSelectedItem');
                var productId = product.value;
                var productName = product.label;
                if (parentId == productId) {
                    alert('<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %> không được trùng với đối tượng chính!');
                }
                else {
                    var quantity = $('#txtQuantity<%=elementId%>').val();
                    var validFrom = $('#txtValidFrom<%=elementId%>').val();
                    var validUntil = $('#txtValidUntil<%=elementId%>').val();
                    var lossRate = $('#txtLossRate<%=elementId%>').val();
                    var replacement = $('#txtReplacement<%=elementId%>').val();
                    var description = $('#txtDescription<%=elementId%>').val();

                    $.ajax({
                        type: 'POST',
                        url: appPath + 'Directories/SaveBoMLine',
                        data: {
                            parentId: parentId,
                            productId: productId,
                            quantity: quantity,
                            lossRate: lossRate,
                            replacement: replacement,
                            description: description,
                            validFrom: validFrom,
                            validUntil: validUntil
                        },
                        success: function (data) {
                            if (data == '') {
//                                var strDate1 = '', strDate2 = '';
//                                if (validFrom != '')
//                                    strDate1 = $.datepicker.formatDate('dd/mm/yy', new Date(validFrom));
//                                if (validUntil != '')
//                                    strDate2 = $.datepicker.formatDate('dd/mm/yy', new Date(validUntil));
                                //var row = '<tr id="row<%=elementId%>' + productId + '"><td>' + productName + '</td><td>' + quantity + '</td><td>' + strDate1 + '</td><td>' + strDate2 + '</td><td><button type="button" class="btActions update" onclick="javascript:fnSetLine(\'' + productId + '\', \'' + productName + '\', \'' + quantity + '\', \'' + validFrom + '\' ,\'' + validUntil + '\')"></button>&nbsp;&nbsp;<button type="button" class="btActions delete" onclick="javascript:fnRemoveLine(\'' + productId + '\')"></button></td></tr>';
                                var row = '<tr id="row<%=elementId%>' + productId + '"><td></td><td>' + productName + '</td><td>' + quantity + '</td><td>' + lossRate + '</td><td>' + replacement + '</td><td>' + description + '</td><td class="actionCols"><button type="button" class="btActions update" onclick="javascript:fnSetLine(\'' + productId + '\', \'' + productName + '\', \'' + quantity + '\', \'' + validFrom + '\' ,\'' + validUntil + '\')"></button>&nbsp;&nbsp;<button type="button" class="btActions delete" onclick="javascript:fnRemoveLine(\'' + productId + '\')"></button></td></tr>';
                                var elm = document.getElementById('row<%=elementId%>' + productId);
                                if (elm == null) {
                                    $('#tbodyDetails<%=elementId%>').append(row);
                                } else {
                                    $(elm).replaceWith(row);
                                }
                                fnResetLine();
                            }
                            else {
                                alert(data);
                            }
                        }
                    });
                }
            } else
                alert('Dữ liệu nhập không đúng!\nVui lòng kiểm tra các ô bị đánh dấu!');
        }

        function fnSetLine(productId, productName, quantity, lossRate, replacement, description, validFrom, validUntil) {
            fnShowForm();
            fnSetItemForCombobox('cbbProducts<%=elementId%>', productId, productName);
            $('#txtQuantity<%=elementId%>').val(quantity);
            $('#txtLossRate<%=elementId%>').val(lossRate);
            $('#txtReplacement<%=elementId%>').val(replacement);
            $('#txtDescription<%=elementId%>').val(description);
            //$('#txtValidFrom<%=elementId%>').val(validFrom);
            //$('#txtValidUntil<%=elementId%>').val(validUntil);
        }

        function fnRemoveLine(productId) {
            if (confirm('Xác nhận?')) {
                $.ajax({
                    type: 'POST',
                    url: appPath + 'Directories/RemoveBoMLine',
                    data: {
                        parentId: $('#txtProductId<%=elementId%>').val(),
                        productId: productId
                    },
                    success: function (data) {
                        if (data == '') {
                            var elm = document.getElementById('row<%=elementId%>' + productId);
                            if (elm != null) {
                                $(elm).remove();
                            }
                        }
                        else {
                            alert(data);
                        }
                    }
                });
            }
        }

        function fnResetLine() {
            $('#cbbProducts<%=elementId%>').jqxComboBox('clearSelection');
            $('#txtQuantity<%=elementId%>').val('1');
            //$('#txtValidFrom<%=elementId%>').val('');
            //$('#txtValidUntil<%=elementId%>').val('');

        }

        function fnShowForm() {
            $('#formBoMLine<%=elementId%>').show();
        }

        function fnShowActionButtons() {
            $('#tbodyDetails<%=elementId%>').find('.btActions').show();
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td style="width: 150px;" valign="top" align="center">
                <table style="width: 68%;">
                    <tr>
                        <td><button class="button" onclick="javascript:fnShowForm()" style="width: 100%;">Thêm</button></td>
                    </tr>
                    <tr>
                        <td><button class="button" onclick="javascript:fnShowActionButtons()" style="width: 100%;"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button></td>
                    </tr>
                </table>
            </td>
            <td valign="top">
                <table style="width: 90%" class="tbDetails">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Vật tư/Linh kiện</th>
                            <th>Định mức</th>
                            <th>Tỷ lệ hao hụt</th>
                            <th>Thời điểm thay</th>
                            <th>Hướng dẫn thao tác</th>
                            <%--<th>Ngày hiệu lực</th>
                            <th>Ngày hết hạn</th>--%>
                            <th></th>
                        </tr>
                        <tr id="formBoMLine<%=elementId%>" class="form-to-validate" style="display: none;">
                            <th></th>
                            <th style="width: 260px;"><%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts" + elementId });%></th>
                            <th><input type="text" id="txtQuantity<%=elementId%>" style="width: 86px" /></th>
                            <th><input type="text" id="txtLossRate<%=elementId%>" style="width: 86px" /></th>
                            <th><input type="text" id="txtReplacement<%=elementId%>" style="width: 86px" /></th>
                            <th><input type="text" id="txtDescription<%=elementId%>" /></th>
                            <%--<th><input type="text"  id="txtValidFrom<%=elementId%>" /></th>
                            <th><input type="text"  id="txtValidUntil<%=elementId%>" /></th>--%>
                            <th class="actionCols">
                                <button type="button" class="btActions save" onclick="javascript:fnSaveLine()" title="<%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %>"></button>&nbsp;
                                <button type="button" class="btActions reset" onclick="javascript:fnResetLine()" title="Nhập lại"></button>
                            </th>
                        </tr>
                    </thead>
                    <tbody id="tbodyDetails<%=elementId%>">
                        <% 
                            List<NEXMI.ProductBOMs> objs = (List<NEXMI.ProductBOMs>)ViewData["Objs"];
                            int count = 0;
                            foreach (NEXMI.ProductBOMs Item in objs)
                            {
                        %>
                        <tr id="row<%=elementId%><%=Item.ProductId%>">
                            <td><%= ++count %></td>
                            <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                            <td><%=Item.Quantity%></td>
                            <td><%=Item.LossRate%></td>
                            <td><%=Item.ReplacementTime%></td>
                            <td><%=Item.Description%></td>
                            <%--<td><%=(Item.ValidFrom == null) ? "" : Item.ValidFrom.Value.ToString("dd-MM-yyyy")%></td>
                            <td><%=(Item.ValidUntil == null) ? "" : Item.ValidUntil.Value.ToString("dd-MM-yyyy")%></td>--%>
                            <td class="actionCols">
                                <%--<button type="button" class="btActions update" onclick='javascript:fnSetLine("<%=Item.ProductId%>", "<%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%>", "<%=Item.Quantity%>", "<%=(Item.ValidFrom == null) ? "" : Item.ValidFrom.Value.ToString("yyyy-MM-dd")%>", "<%=(Item.ValidUntil == null) ? "" : Item.ValidUntil.Value.ToString("yyyy-MM-dd")%>")'></button>&nbsp;--%>
                                <button type="button" class="btActions update" onclick='javascript:fnSetLine("<%=Item.ProductId%>", "<%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%>", "<%=Item.Quantity%>", "<%=Item.LossRate%>", "<%=Item.ReplacementTime%>", "<%=Item.Description%>")'></button>&nbsp;
                                <button type="button" class="btActions delete" onclick="javascript:fnRemoveLine('<%=Item.ProductId%>')"></button>
                            </td>
                        </tr>
                        <% 
                            }
                        %>
                    </tbody>
                </table>    
            </td>
        </tr>
    </table>
</div>