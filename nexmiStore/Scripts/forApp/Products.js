function fnShowProductForm(id, typeId) {
    LoadContent('', 'Directories/ProductForm?id=' + id + '&typeId=' + typeId);
}

function fnPopupProductDialog(id, parentId) {
    if (parentId == undefined) parentId = '';
    var title = 'Thêm sản phẩm';
    if (id != '')
        title = 'Sửa sản phẩm';
    openWindow(title, 'Directories/ProductForm', { id: id, parentId: parentId }, '80%', '');
}

function fnDeleteProduct(id) {
    var answer = confirm("Bạn muốn xóa sản phẩm này?");
    if (answer) {
        OpenProcessing();
        $.post(appPath + "Directories/DeleteProduct", { id: id }, function (data) {
            CloseProcessing();
            if (data == "") {
                fnRefreshProduct();
            }
            else {
                alert(data);
            }
        });
    }
}

function fnGetProductUnits() {
    var productCategoryId = $("#slProductCategories1").val();
    if (productCategoryId != "") {
        $.post(appPath + "Directories/GetProductUnits", { productCategoryId: productCategoryId },
        function(data) {
            if (data.length > 0) {
                $("#slProductUnits").html("");
                var PUNTWSI;
                for (var i in data) {
                    PUNTWSI = data[i];
                    $("#slProductUnits").append(new Option(PUNTWSI.Name, PUNTWSI.Id, false, false));
                }
            }
            else {
                $("#slProductUnits").append(new Option("--/--", "", false, false));
            }
        });
        var productUnit = $("#txtUnit").val();
        $("#slProductUnits").val(productUnit);
    }
}

function fnLoadContent(productCategoryId) {
    if (productCategoryId == undefined) {
        productCategoryId = $("#slProductCategories").val();
    }
    LoadContent("", "Directories/Products?productCategoryId=" + productCategoryId);
}

function fnLoadProductDetail(id) {
    if (id == "") {
        var rowindex = $("#ProductGrid").jqxGrid('getselectedrowindex');
        if (rowindex == -1) {
            rowindex = 0;
        }
        var dataRecord = $("#ProductGrid").jqxGrid('getrowdata', rowindex);
        if (dataRecord != null) {
            id = dataRecord.ProductId;
        }
    }
    if (id != "") {
        LoadContent("", "Directories/ProductDetail?id=" + id);
    }
}

function fnSaveProductPrice(productId, price) {
    $.post(appPath + "Directories/SaveProductPrice", { productId: productId, price: price },
    function (data) {
        if (data == "") {
        }
        else {
            alert(data);
        }
    });
}

function fnPopupPricesForSalesInvoiceHistoryDialog(productId) {
    var windowId = fnRandomString();
    OpenPopup(windowId, "", appPath + "Directories/PricesForSalesInvoiceHistory", { productId: productId }, true, true, true, true, 600, 500);
    var cancel = function () {
        ClosePopup(windowId);
    }
    var dialogOpts = {
        buttons: {
            "Đóng": cancel
        }
    };
    $("#" + windowId).dialog(dialogOpts);
}

function fnExport2Excel() {
    var categoryId = "";
    try {
        categoryId = $("#cbbCategories").jqxComboBox('getSelectedItem').value;
    } catch (err) { }
    $.download(appPath + 'Common/Products2Excel', 'keyword=' + $("#txtKeywordProduct").val() + '&categoryId=' + categoryId);
}

function fnReloadProduct(pageNum) {
    if (pageNum == undefined)
        pageNum = '';
    var categoryId = '';
    try {
        categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
    } catch (err) { }
    LoadContentDynamic('ProductGrid', 'Directories/ProductList', {
        pageNum: pageNum,
        categoryId: categoryId,
        keyword: Base64.encode($('#txtKeywordProduct').val()),
        view: $('#productViewType').val()
    });
}

function fnLoadProductGrid(pageNum) {
    if (pageNum == undefined)
        pageNum = '';
    var categoryId = '';
    try {
        categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
    } catch (err) { }
    LoadContentDynamic('ProductGrid', 'Directories/ProductList', {
        pageNum: pageNum,
        categoryId: categoryId,
        keyword: Base64.encode($('#txtKeywordProduct').val()),
        view: 'kanban'
    });
}

function fnLoadProductList(pageNum) {
    if (pageNum == undefined)
        pageNum = '';
    var categoryId = '';
    try {
        categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
    } catch (err) { }
    LoadContentDynamic('ProductGrid', 'Directories/ProductList', {
        pageNum: pageNum,
        categoryId: categoryId,
        keyword: Base64.encode($('#txtKeywordProduct').val()),
        view: 'list'
    });
}

function fnShowProducts(parentId) {
    openWindow("Danh sách sản phẩm", "UserControl/ProductList", { parentId: parentId }, "80%", "90%");
}