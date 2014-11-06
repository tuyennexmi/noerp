<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });

        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#txtTotal').autoNumeric('init', { vMax: '100000000000000' });

        $('#detailTabs').bind('selected', function (event) {
            var item = event.args.item;
            switch (item) {
                case 0:
                    fnReloadPurchase();
                    break;
                case 1:
                    fnReloadCustomers();
                    break;
            }
        });

        $('#cbbProducts').on('change', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    default:
                        if (value != "" && value != undefined) {
                            fnGetProductAmount(value);
                        }
                        break;
                }
            }
        });
    });
    
    function fnSaveSalesPlanCustomerByProduct() {
        $.ajax({
            type: 'POST',
            url: appPath + 'Planning/SaveSalesPlanningCustomerByProduct',
            data: {
                id: $('#txtId').text(),
                begin: $('#dtFrom').val(),
                end: $('#dtTo').val(),
                title: $('#txtTitle').val(),
                description: $('#txtDescription').val(),
                salesAmount: $('#txtTotal').val()
            },
            beforeSend: function () {
                fnDoing();
            },
            success: function (data) {
                if (data.error == "") {
                    alert("Đã lưu thành công!");
                    fnPlanDetail(data.id);
                }
                else {
                    alert(data.error);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi nên không lưu được!");
            },
            complete: function () {
            }
        });
    }

    function fnPlanByProducts() {
        LoadContentDynamic('SalesPlanDiv', 'Planning/PlanningByProductsUC', '');
    }

    function fnReloadCustomers() {
        var area = document.getElementsByName('cbbAreas')[0].value;
        var product = document.getElementsByName('cbbProducts')[0].value;
        var Amount = $('#txtTotal').val();
        LoadContentDynamic('SalesPlanDiv', 'Planning/SalesByProductsUC', {
            area: area,
            amount: Amount,
            productId: product
        });
    }

    function fnGetProductAmount(productId) {
        $.post(appPath + "Planning/GetPurchaseAmount", { productId: productId }, function (data) {
            try { $("#txtTotal").autoNumeric('set', data); } catch (Error) { }
            fnReloadCustomers();
            fnReloadPurchase();
        });
    }

    function fnReloadPurchase() {
//        var area = document.getElementsByName('cbbAreas')[0].value;
        var product = document.getElementsByName('cbbProducts')[0].value;
//        var Amount = $('#txtTotal').val();
        LoadContentDynamic('PurchasePlanDiv', 'Planning/PurchaseByProductsUC', {           
            productId: product
        });
    }


</script>
<%        NEXMI.NMMasterPlanningsWSI wsi = (NEXMI.NMMasterPlanningsWSI)ViewData["Plan"];
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr><td></td></tr>
        <tr>
            <td>
                <button onclick="javascript:fnSaveSalesPlanCustomerByProduct()" class="color red button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
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
        <input type="hidden" id="txtStatus" value="<%=wsi.Planning.Status%>" />
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Planning", current = "PLN01" });%>
        </div>
    </div>
    
    <div class='divContentDetails'>
    <form id="formSalesOrder" class="form-to-validate" action="" style="margin: 10px;">
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId">Lập kế hoạch <label id="txtId"><%=wsi.Planning.Id %></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
                            <td><input type="text" id="txtTitle" value="<%= wsi.Planning.Title %>" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
                            <td><input type="text" id="txtDescription" value="<%= wsi.Planning.Descriptions %>"/></td>
                        </tr>
                        <tr>
                            <td class="lbright">Ngày bắt đầu</td>
                            <td><input type="text"  id="dtFrom" value="<%=wsi.Planning.BeginDate.ToString("yyyy-MM-dd")%>" /></td>
                            <td class="lbright">Ngày kết thúc</td>
                            <td><input type="text"  id="dtTo" value="<%=wsi.Planning.EndDate.ToString("yyyy-MM-dd")%>" /></td>                            
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
                            <td><%Html.RenderAction("cbbProducts", "UserControl", new { elementId = "cbbProducts", value = ViewData["Product"], label = NEXMI.NMCommon.GetName(ViewData["Product"].ToString(), langId) });%></td>
                            <td class="lbright">Tổng sản lượng mong đợi</td>
                            <td><input type="text" id="txtTotal" value="<%= ViewData["Amount"] %>" readonly="readonly" /></td>                            
                        </tr>                        
                    </table>
                </td>
            </tr>
        </table>
    </form>
    <div class="tabs" id='detailTabs'>
        <ul>
            <li style="margin-left: 25px;">Mua hàng</li>
            <li>Bán hàng</li>
        </ul>
        <div>
            <table style="width: 88%" class="tbTemplate">    
                <tr>
                    <td valign="top">
                        <div id="PurchasePlanDiv">
                            <%Html.RenderAction("PurchaseByProductsUC", new { productId = ViewData["Product"] });%>
                        </div>
            </td></tr></table>
        </div>
        <div>
            <table style="width: 100%" class="tbTemplate">    
                <tr>
                    <td valign="top">
                        <table>
                            <tr>                            
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA", langId) %></td>
                                <td><%Html.RenderAction("cbbAreas", "UserControl", new { areaId = "", areaName = "", elementId = "", holderText = NEXMI.NMCommon.GetInterface("SELECT_AREA", langId) });%></td>
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