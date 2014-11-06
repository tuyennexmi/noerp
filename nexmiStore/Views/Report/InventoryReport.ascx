<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        ChangeCategory();
    });
    function ChangeCategory() {
        var categoryId = '';
        try {
            categoryId = $("#cbbCategories").jqxComboBox('getSelectedItem').value;
        } catch (err) { }
        LoadContentDynamic('divInventoryReportData', 'Report/InventoryReportUC', { categoryId: categoryId });
    }
</script>
<div class="divActions">
    
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <table style="width: 100%;">
                <tr>
                    <td>
                        <%Html.RenderAction("cbbCategories", "UserControl", new { elementId = "", objectName = "Products" });%>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <h4>BẢNG TỔNG HỢP SỐ LƯỢNG SẢN PHẨM TỒN CÁC KHO</h4>
    <div id="divInventoryReportData">
        
    </div>
</div>