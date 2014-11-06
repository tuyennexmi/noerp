<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<link href="<%=Url.Content("~")%>Content/POS.css" rel="stylesheet" type="text/css" />

<div style="position: fixed; top: 39px; left: 0px; bottom: 0px; right: 0px;" id="divPOS"> 
    <script type="text/javascript">
        var flag = false;
        $(function () {
            fnLoadCategories('');
            fnCheckCart();
            fnBegin();
            $("#Qty").addClass("green");
            $("#tbKeyboard").find("[name=type1]").click(function () {
                $("#tbKeyboard").find("[name=type1]").removeClass("green");
                $(this).addClass("green");
            });
            $("#tbKeyboard").find("[name=type2]").click(function () {
                flag = true;
                fnType2(this.innerHTML);
            });
            $("#decimal").click(function () {
                if ($(this).hasClass('decimal'))
                    $(this).removeClass('decimal');
                else {
                    $(this).addClass('decimal');
                    countDecimal = 0;
                }
            });
            $("#delete").click(function () {
                fnDelete();
            });
            $("#Cash").click(function () {
                if ($("#lbNoItem").text() == "") {
                    var totalAmount = $("#txtTotalAmount").text();
                    openWindow("Thanh toán bằng tiền mặt", "PointOfSale/Cash", { totalAmount: totalAmount }, 660, 550);
                } else {
                    alert('Không có sản phẩm nào!');
                }
            });

            $("#btNextOrder").click(function () {
                $.post(appPath + "Common/ClearSession", { sessionName: "OrderDetails" });
                $("#orderlinesPOS").html("");
                fnCheckCart();
                $("#divRight").show();
                $("#divReceipt").hide();
            });

            $("#btPrint").click(function () {
                $("#toPrint").printElement();
            });

            var t;
            $("#txtSearch").on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnLoadProducts('');
                }, 1200);
            });

            $(document).on('keydown', function (event) {
                if (!$(event.target).is('input')) {
                    switch (event.keyCode) {
                        case 113:
                            $("#txtSearch").focus();
                            return false;
                        case 114:
                            $("#txtBarCode").focus();
                            return false;
                        case 48: case 49: case 50: case 51: case 52: case 53: case 54: case 55: case 56: case 57:
                            fnType2(String.fromCharCode(event.keyCode));
                            return false;
                        case 8:
                            fnDelete();
                            return false;
                        case 190:
                            $('#decimal').click();
                            return false;
                        case 115:
                            $('#Qty').click();
                            return false;
                        case 116:
                            $('#Disc').click();
                            return false;
                        case 117:
                            $('#Price').click();
                            return false;
                        case 118:
                            $('#txtOrder').click();
                            return false;
                        case 119:
                            $('#Cash').click();
                            return false;
                        case 120:
                            $('#btImport').click();
                            return false;
                        case 121:
                            $('#btExport').click();
                            return false;
                        default:
                            //alert('Không sử dụng phím này!');
                            return true;
                    }
                } else {
                    switch (event.keyCode) {
                        case 113:
                            $("#txtSearch").focus();
                            return false;
                        case 114:
                            $("#txtBarCode").focus();
                            return false;
                        case 115:
                            $('#Qty').click();
                            return false;
                        case 116:
                            $('#Disc').click();
                            return false;
                        case 117:
                            $('#Price').click();
                            return false;
                        case 118:
                            $('#txtOrder').click();
                            return false;
                        case 119:
                            $('#Cash').click();
                            return false;
                        case 120:
                            $('#btImport').click();
                            return false;
                        case 121:
                            $('#btExport').click();
                            return false;
                        default:
                            //alert('Không sử dụng phím này!');
                            return true;
                    }
                }
                return true;
            });

            $('#txtBarCode').on('keydown', function (event) {
                if (event.keyCode == 13) {
                    $.post(appPath + 'UserControl/GetProductInfo', { barCode: $(this).val() }, function (data) {
                        if (data.error == "") {
                            fnItemClick(data.productId, data.productName, data.price, data.quantity, data.tax, data.discount, data.mode);
                        } else {
                            alert(data.error);
                        }
                        $('#txtBarCode').val('').focus();
                    });
                }
            });
        });

        var countInterger = 0, countDecimal = 0;

        function fnType2(value) {
            var item = $(".itemSelected")[0];
            var decimalItem = $(".decimal")[0];
            if (item != null) {
                var productId = item.id;
                var type1 = $(".green")[0].id;
                //var value = number.innerHTML;
                var arr, interger, decimal;
                switch (type1) {
                    case "Qty":
                        arr = $("#quantity" + productId).text().split('.');
                        interger = fnStripNonNumeric(arr[0]);
                        decimal = arr[1];
                        if (decimalItem == null)
                            if (countInterger == 0) {
                                interger = value;
                                countInterger = 1;
                            }
                            else
                                interger += value;
                        else {
                            if (countDecimal == 2) {
                                decimal = "";
                                countDecimal = 0;
                            }
                            if (countDecimal == 0) {
                                decimal = value + "0";
                                countDecimal = 1;
                            } else {
                                decimal = decimal[0] + value;
                                countDecimal = 2;
                                $("#decimal").removeClass('decimal');
                            }
                        }
                        $("#quantity" + productId).text(fnNumberFormat(interger + "." + decimal));
                        break;
                    case "Disc":
                        var discount = $("#discount" + productId).text();
                        discount = discount + value;
                        if (parseInt(discount) > 100) {
                            discount = "100";
                        }
                        if (parseInt(discount) == 0) {
                            discount = "";
                        }
                        $("#discount" + productId).text(parseInt(discount));
                        break;
                    case "Price":
                        arr = $("#price" + productId).text().split('.');
                        var price = fnStripNonNumeric(arr[0]) + value;
                        $("#price" + productId).text(fnNumberFormat(price));
                        break;
                }
                fnItemAmount(productId);
            }
        }

        function fnDelete() {
            var item = $(".itemSelected")[0];
            var decimalItem = $(".decimal")[0];
            if (item != null) {
                var productId = item.id;
                var type1 = $(".green")[0].id;
                var arr, interger, decimal;
                switch (type1) {
                    case "Qty":
                        arr = $("#quantity" + productId).text().split('.');
                        interger = fnStripNonNumeric(arr[0]);
                        decimal = arr[1];
                        if (interger == 1 && countInterger == 0) {
                            fnRemoveFromList(productId);
                            productId = "";
                        } else {
                            if (decimalItem == null) {
                                interger = interger.substring(0, interger.length - 1);
                                if (interger == 0) {
                                    interger = 1;
                                    countInterger = 0;
                                }
                            }
                            else {
                                if (decimal[1] == "0") {
                                    decimal = "00";
                                    countDecimal = 0;
                                } else {
                                    decimal = decimal[0] + "0";
                                    countDecimal = 1;
                                }
                            }
                            $("#quantity" + productId).text(fnNumberFormat(interger + "." + decimal));
                        }
                        break;
                    case "Disc":
                        var discount = $("#discount" + productId).text();
                        if (discount != "") {
                            discount = discount.substring(0, discount.length - 1);
                        }
                        $("#discount" + productId).text(discount);
                        break;
                    case "Price":
                        arr = $("#price" + productId).text().split('.');
                        var price = fnStripNonNumeric(arr[0]);
                        $("#price" + productId).text(fnNumberFormat(price.substring(0, price.length - 1)));
                        break;
                }
                if (productId != "") {
                    fnItemAmount(productId);
                }
            }
        }

        function fnCheckCart() {
            var child = $('#orderlinesPOS').children().length;
            if (child == 0) {
                $("#lbNoItem").text("Không có sản phẩm nào!");
                $("#txtDiscountAmount").text(fnNumberFormat(0));
                $("#txtTaxAmount").text(fnNumberFormat(0));
                $("#txtTotalAmount").text(fnNumberFormat(0));
            } else {
                $("#lbNoItem").text("");
                $("#orderlinesPOS tr:last-child").addClass("itemSelected");
            }
        }

        function fnBegin() {
            $("#txtDiscountAmount").text(fnNumberFormat(0));
            $("#txtTaxAmount").text(fnNumberFormat(0));
            $("#txtTotalAmount").text(fnNumberFormat(0));
        }

        function fnLoadCategories(parentId) {
            LoadContentDynamic("divMap", "PointOfSale/Map", { categoryId: parentId });
            LoadContentDynamic("divCategories", "PointOfSale/divCategories", { parentId: parentId });
            fnLoadProducts(parentId);
        }

        function fnLoadProducts(parentId) {
            var keyword = $("#txtSearch").val();
            LoadContentDynamic("divProducts", "PointOfSale/divProducts", { categoryId: parentId, keyword: keyword });
        }

        function fnItemAmount(productId) {
            var quantity = fnStripNonNumeric($("#quantity" + productId).text());
            var price = fnStripNonNumeric($("#price" + productId).text());
            var discount = $("#discount" + productId).text();
            var name = $("#name" + productId).val();
            var tax = $("#tax" + productId).val();
            fnAddToList(productId, name, price, quantity, tax, discount, "");
        }

        function fnTotalAmount(amount, taxAmount, discountAmount) {
            var total = fnStripNonNumeric($("#txtTotalAmount").text());
            $("#txtTotalAmount").text(fnNumberFormat(parseFloat(total) + parseFloat(fnStripNonNumeric(amount))));
            var tax = fnStripNonNumeric($("#txtTaxAmount").text());
            $("#txtTaxAmount").text(fnNumberFormat(parseFloat(tax) + parseFloat(fnStripNonNumeric(taxAmount))));
            var discount = fnStripNonNumeric($("#txtDiscountAmount").text());
            $("#txtDiscountAmount").text(fnNumberFormat(parseFloat(discount) + parseFloat(fnStripNonNumeric(discountAmount))));
        }

        function fnMinusTotal(amount, taxAmount, discountAmount) {
            var total = fnStripNonNumeric($("#txtTotalAmount").text());
            $("#txtTotalAmount").text(fnNumberFormat(parseFloat(total) - parseFloat(fnStripNonNumeric(amount))));
            var tax = fnStripNonNumeric($("#txtTaxAmount").text());
            $("#txtTaxAmount").text(fnNumberFormat(parseFloat(tax) - parseFloat(fnStripNonNumeric(taxAmount))));
            var discount = fnStripNonNumeric($("#txtDiscountAmount").text());
            $("#txtDiscountAmount").text(fnNumberFormat(parseFloat(discount) - parseFloat(fnStripNonNumeric(discountAmount))));
        }

        function fnRemoveFromList(productId) {
            $.post(appPath + "PointOfSale/RemoveFromList", { productId: productId }, function (data) {
                var getRow = document.getElementById(productId);
                if (getRow != null) {
                    $("#" + productId).remove();
                }
                fnRowClick(productId);
                fnMinusTotal(data.Amount, data.TaxAmount, data.DiscountAmount);
                fnCheckCart();
                $('.divOrderPOS').scrollTop($('.divOrderPOS').height());
            });
        }

        function fnClose() {
            document.location = appPath + "Sales/Customers?moduleId=AR&functionId=AR001";
        }

        function fnItemClick(productId, productName, price, quantity, tax, discount, mode) {
            $('#tbKeyboard').find('[name=type1]').removeClass('green');
            $('#Qty').addClass('green');
            var item = document.getElementById(productId);
            if (item != null) {
                mode = "";
                price = fnStripNonNumeric($('#price' + productId).text());
                quantity = parseFloat(fnStripNonNumeric($('#quantity' + productId).text())) + 1;
                discount = $('#discount' + productId).text();
            } else {
                countInterger = 0;
            }
            fnAddToList(productId, productName, price, quantity, tax, discount, mode);
        }

        function fnAddToList(productId, productName, price, quantity, tax, discount, mode) {
            $.post(appPath + 'PointOfSale/AddToList', { productId: productId, price: price, quantity: quantity, tax: tax, discount: discount, mode: mode }, function (data) {
                var row = '';
                row += '<td align="left"><input type="hidden" id="name' + productId + '" value="' + productName + '" /><input type="hidden" id="tax' + productId + '" value="' + tax + '" />' + Base64.decode(productName) + '</td>';
                row += '<td align="left" style="font-size: 12px;"><label id="quantity' + productId + '">' + data.Quantity + '</label></td>';
                row += '<td align="left" style="font-size: 12px;"><label id="price' + productId + '">' + fnNumberFormat(price) + '</label></td>';
                row += '<td align="left" style="font-size: 12px;"><label id="discount' + productId + '">' + discount + '</label></td>';
                row += '<td align="right">' + data.Amount + '</td>';
                //row += '<td></td>';
                var getRow = document.getElementById(productId);
                if (getRow != null) {
                    $('#' + productId).remove();
                }
                $('#orderlinesPOS').append('<tr id="' + productId + '" onclick="javascript:fnRowClick(\'' + productId + '\')">' + row + '</tr>');
                fnRowClick(productId);
                fnTotalAmount(data.newAmount, data.taxAmount, data.DiscountAmount);
                fnCheckCart();
                $('.divOrder').scrollTop($('.divOrder').height());
            });
        }

        function fnRowClick(id) {
            $('#orderlinesPOS').find('tr').removeClass('itemSelected');
            $('#' + id).addClass('itemSelected');
        }

        function fnImport() {
            if ($("#lbNoItem").text() == "") {
                openWindow('Thông tin nhập kho', 'PointOfSale/ImportForm', {}, 900, 500);
            } else {
                alert('Không có sản phẩm nào!');
            }
        }

        function fnExport() {
            if ($("#lbNoItem").text() == "") {
                openWindow('Thông tin xuất kho', 'PointOfSale/ExportForm', {}, 900, 500);
            } else {
                alert('Không có sản phẩm nào!');
            }
        }

        function fnOrder() {
            if ($("#lbNoItem").text() == "") {
                openWindow('<%= NEXMI.NMCommon.GetInterface("SO_TITLE", langId) %>', 'PointOfSale/OrderForm', {}, 900, 500);
            } else {
                alert('Không có sản phẩm nào!');
            }
        }
    </script>
	<div style="position: absolute; top: 0px; bottom: 0px; width: 100%; padding: 0px;">
        <table style="width: 100%; height: 100%;" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 40%; border-right: 1px solid #ddd;">
                    <div class="bg2" style="position: absolute; top: 0px; left: 0px; right: 60%; bottom: 0px; padding-top: 0; padding-right: 5px;">
                        <div class="divOrderPOS">
                            <input type="text" id="txtBarCode" style="margin: 3px 0 0 10px;" placeholder="Tìm theo mã vạch (F3)" />
                            <div class="ordersPOS">
                                <div class="summaryPOS">
                                    <table style="width: 100%; border-collapse: collapse;" cellpadding="3">
                                        <tr style="font-size: 15px; border-bottom: 1px solid #ddd;">
                                            <td align='left'>S. Phẩm</td>
                                            <td align='left'>S. L</td>
                                            <td align='left'>Đ. Giá</td>
                                            <td align='left'>G. Giá(%)</td>
                                            <td>T. Tiền</td>
                                            <%--<td></td>--%>
                                        </tr>
                                        <tbody id="orderlinesPOS" style="font-size: 14px;">
                                            
                                        </tbody>
                                        <tr>
                                            <td colspan="5"><label id="lbNoItem"></label></td>
                                        </tr>
                                    </table>
                                </div>
                                <div id="divAmout">
                                    <table style="width: 100%; border-collapse: collapse;" cellpadding="3">
                                        <tr style=" border-top: 1px solid #ddd;">
                                            <td align="right"><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>:</td>
                                            <td align="right" style="width: 40%"><label id="txtDiscountAmount"></label></td>
                                        </tr>
                                        <tr>
                                            <td align="right">Thuế:</td>
                                            <td align="right"><label id="txtTaxAmount"></label></td>
                                        </tr>
                                        <tr>
                                            <td align="right"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</td>
                                            <td align="right"><label id="txtTotalAmount"></label></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div style="position: absolute; height: 190px; bottom: 0px; right: 5px; overflow: hidden;">
                            <table cellpadding="3" id="tbKeyboard">                                
                                <tr>
                                    <td><button id="Cash" class="button keyboardPOS textPOS longPOS" title="Phím tắt: F8"><%= NEXMI.NMCommon.GetInterface("PAYMENT", langId) %></button></td>                                    
                                    <td><button name="type2" class="button keyboardPOS">4</button></td>
                                    <td><button name="type2" class="button keyboardPOS">5</button></td>
                                    <td><button name="type2" class="button keyboardPOS">6</button></td>
                                    <td><button id="Disc" name="type1" class="button keyboardPOS textPOS" title="Phím tắt: F5">Giảm<br />giá</button></td>
                                </tr>
                                <tr>                                    
                                    <td><button id="txtOrder" class="button keyboardPOS textPOS longPOS" onclick="javascript:fnOrder()" title="Phím tắt: F7"><%= NEXMI.NMCommon.GetInterface("ORDER", langId) %></button></td>
                                    <td><button name="type2" class="button keyboardPOS">1</button></td>
                                    <td><button name="type2" class="button keyboardPOS">2</button></td>
                                    <td><button name="type2" class="button keyboardPOS">3</button></td>
                                    <td><button id="Qty" name="type1" class="button keyboardPOS textPOS" title="Phím tắt: F4">Số<br />lượng</button></td>
                                </tr>
                                <tr>
                                    <td><button id="btImport" class="button keyboardPOS textPOS longPOS" onclick="javascript:fnImport()" title="Phím tắt: F9"><%= NEXMI.NMCommon.GetInterface("IMPORT", langId) %></button></td>
                                    <td><button name="type2" class="button keyboardPOS">7</button></td>
                                    <td><button name="type2" class="button keyboardPOS">8</button></td>
                                    <td><button name="type2" class="button keyboardPOS">9</button></td>
                                    <td><button id="Price" name="type1" class="button keyboardPOS textPOS" title="Phím tắt: F6">Đơn<br />giá</button></td>
                                </tr>
                                <tr>
                                    <td><button id="btExport" class="button keyboardPOS textPOS longPOS" onclick="javascript:fnExport()" title="Phím tắt: F10"><%= NEXMI.NMCommon.GetInterface("EXPORT", langId) %></button></td>
                                    <%--<td><button class="button red keyboardPOS textPOS longPOS" onclick="javascript:history.back();">ĐÓNG</button></td>--%>
                                    <td><button class="button keyboardPOS">+/-</button></td>
                                    <td><button name="type2" class="button keyboardPOS">0</button></td>
                                    <td><button id="decimal" class="button keyboardPOS">.</button></td>
                                    <td><button id="delete" class="button keyboardPOS textPOS"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </td>
                <td style="width: 60%;">
                    <div style="width: 100%; height: 100%;" id="divRight">
                        <div class="bg2" style="position: absolute; top: 0px; left: 40%; right: 0px; height: 30px; padding: 0;">
                            <table style="width: 100%;" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div id="divMap" style="height: 30px;"></div>
                                    </td>
                                    <td align="right"><input id="txtSearch" type="text" results="5" name="s" placeholder="Tìm sản phẩm (F2)" style="width: 150px;" /></td>
                                </tr>
                            </table>
                        </div>
                        <div class="bg3" id="divCategories">
                                
                        </div> 
                        <div class="bg4" id="divProducts">

                        </div>
                    </div>
                    <div style="width: 100%; height: 100%; display: none;" id="divReceipt">
                        <div class="bg2" style="position: absolute; top: 0px; left: 40%; right: 0px; height: 50px; padding-top: 5px; padding-left: 5px;">
                            <button id="btPrint" class="button large red"><b>Print</b></button>
                            <button id="btNextOrder" class="button large red"><b>Next</b></button>
                        </div>
                        <div style="position: absolute; top: 55px; left: 40%; right: 0px; bottom: 0px; overflow: auto;">
                            <center>
                                <div id="toPrint">
                                    
                                </div>
                            </center>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>