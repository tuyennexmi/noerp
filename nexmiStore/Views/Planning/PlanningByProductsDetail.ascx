<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~") %>Scripts/forApp/Planning.js" type="text/javascript"></script>
<script type="text/javascript">

    $(document).ready(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
    });

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

    function fnGetProductAmount(productId) {
        $.post(appPath + "Planning/GetPurchaseAmount", { productId: productId }, function (data) {
            try { $("#txtTotal").val(data); } catch (Error) { }
            //fnReloadCustomers();
            fnReloadPurchase();
        });
    }

    function fnReloadCustomers() {
        var product = document.getElementsByName('cbbProducts')[0].value;
        LoadContentDynamic('SalesPlanDiv', 'Planning/SaleByProductDetail', {
            begin: $('#txtBegin').html(),
            to: $('#txtTo').html(),
            product: product
        });
    }

    function fnReloadPurchase() {
        var product = document.getElementsByName('cbbProducts')[0].value;
        LoadContentDynamic('PurchasePlanDiv', 'Planning/PurchaseByProductDetail', {
            begin: $('#txtBegin').html(),
            to: $('#txtTo').html(),
            product: product
        });
    }

</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Plans;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnPlanForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }                    
                %>
                
            </td>
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
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Planning", current = "PLN01" });%>
        </div>
    </div>
    <%        
        NEXMI.NMMasterPlanningsWSI wsi = (NEXMI.NMMasterPlanningsWSI)ViewData["Plan"];
         %>
        
    <div class='divContentDetails'>
    <form id="formSalesOrder" class="form-to-validate" action="" style="margin: 10px;">
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId">Lập kế hoạch <label id="txtId"><%=wsi.Planning.Id %></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TITLE", langId) %></td>
                            <td><%= wsi.Planning.Title %></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %></td>
                            <td><%= wsi.Planning.Descriptions %></td>
                        </tr>
                        <tr>
                            <td class="lbright">Ngày bắt đầu</td>
                            <td><label id="txtBegin"><%=wsi.Planning.BeginDate.ToString("dd/MM/yyyy")%></label></td>
                            <td class="lbright">Ngày kết thúc</td>
                            <td><label id="txtTo"><%=wsi.Planning.EndDate.ToString("dd/MM/yyyy")%></label></td>
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
        <div id="PurchasePlanDiv">
            <%--<%Html.RenderAction("PurchaseByProductDetail", "Planning", new { product = "", begin = wsi.Planning.BeginDate.ToString(), to = wsi.Planning.EndDate.ToString() }); %>--%>
        </div>
        <div id='SalesPlanDiv'>
            <%--<%Html.RenderAction("SaleByProductDetail", "Planning", new { product = "" }); %>--%>
        </div>
    </div>
    
    <%Html.RenderAction("Logs", "Messages", new { ownerId = wsi.Planning.Id, sendTo = wsi.Planning.CreatedBy });%>
    </div>
</div>