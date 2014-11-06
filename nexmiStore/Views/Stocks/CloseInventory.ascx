<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    string functionId = NEXMI.NMConstant.Functions.CloseInventory;
    bool viewAll = GetPermissions.GetViewAll((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId);
%>
<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadData();
            }, 1000);
        });
        $('#slStocks').on('change', function () {
            fnReloadData();
        });
        if ('<%=viewAll%>' == 'False') {
            $('#slStocks').attr('disabled', true);
        }
        fnReloadData();
    });

    function fnReloadData() {
        var categoryId = '';
        try {
            categoryId = $('#cbbCategories').jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divMIC', 'Stocks/CloseInventoryUC', { stockId: $('#slStocks').val(), categoryId: categoryId, keyword: $('#txtKeyword').val() });
    }

    function fnCloseInventory() {
        if (confirm('Xác nhận?')) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Stocks/SaveCloseInventory',
                data: { stockId: $('#slStocks').val() },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data == "") {
                        //fnSuccess();
                        $(window).hashchange();
                    }
                    else {
                        alert(data);
                    }
                },
                error: function () {
                    fnError();
                },
                complete: function () {
                    fnComplete();
                }
            });
        }
    }

    function ChangeCategory() {
        fnReloadData();
    }

    function fnPrint() {
        $.post(appPath + 'UserControl/GetDataUrl', { html: Base64.encode($('#divPrintMIC').html()), mode: '' }, function (data) {
            window.open(appPath + 'UserControl/PrintPage');
        });
    }
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <% 
                            if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                            {    
                        %>
                        <button class="button red" onclick="javascript:fnCloseInventory()">Kết sổ</button>
                        <% 
                            }
                            if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                            {    
                        %>
                        <button class="button" onclick="javascript:fnPrint()"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                        <% 
                            }
                        %>
                        <button class="button" onclick="javascript:fnReloadData()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    </div>
                </td>
                <td align="right">
                        
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <table>
                    <tr>
                        <td><%Html.RenderAction("slStocks", "UserControl", new { elementId = "slStocks", stockId = ((NEXMI.NMCustomersWSI)Session["UserInfo"]).Customer.StockId });%></td>
                        <td style="padding-left: 5px;"><%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%></td>
                    </tr>
                </table>
            </div>
        </div>
      
        <div class='divContentDetails'>
            <table class="frmInput">
                <tr>
                    <td valign="top" id="divPrintMIC">
                        <h4>BẢNG SỐ LIỆU NHẬP - XUẤT - TỒN</h4>
                        <div id="divMIC"></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>