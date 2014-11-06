<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $("#dtFrom, #dtTo").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnGetProductInStock(productId) {
        var dtFrom = $("#dtFrom").val();
        var dtTo = $("#dtTo").val();
        LoadContentDynamic('divInventoryReportData', 'Report/InventoryByProductReportUC', { productId: productId, fromDate: dtFrom, toDate: dtTo });
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
                        <%Html.RenderAction("cbbProduct4Report", "UserControl", new { elementId = "cbbProducts", value = "", label = "" });%>
                    </td>
                    <td><%= NEXMI.NMCommon.GetInterface("FROM", langId) %>: <input type="text"  id="dtFrom"  style="width: 89px"/></td>
                    <td> <%= NEXMI.NMCommon.GetInterface("TO_DATE", langId) %>: <input type="text"  id="dtTo" value="<%=DateTime.Today.ToString("MM/dd/yyyy")%>" style="width: 89px"/></td>
                </tr>
            </table>
        </div>
    </div>
    <h4>BẢNG TỔNG HỢP SỐ LƯỢNG SẢN PHẨM TỒN CÁC KHO</h4>
    <div id="divInventoryReportData">
        
    </div>
</div>