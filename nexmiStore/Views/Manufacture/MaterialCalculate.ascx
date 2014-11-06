<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">

    function fnRemoveLine(productId) {
        if (confirm('Xác nhận?')) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Manufacture/RemoveMaterialLine',
                data: {
                    parentId: $('#txtProductId').val(),
                    productId: productId
                },
                success: function (data) {
                    if (data == '') {
                        var elm = document.getElementById('row' + productId);
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

</script>

<table style="width: 60%" class="tbDetails">
    <thead>
        <tr>
            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
            <th></th>
        </tr>
    </thead>
    <tbody id="tbodyDetails">
        <% 
            List<NEXMI.MOMaterialDetails> BoMs = (List<NEXMI.MOMaterialDetails>)Session["BoMs"];
            //double quantity = double.Parse(ViewData["Quantity"].ToString());
            foreach (NEXMI.MOMaterialDetails Item in BoMs)
            {
        %>
        <tr id="row<%=Item.ProductId%>">
            <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
            <td><%=Item.Quantity.ToString("N3") %></td>
            <td class="actionCols">
                <button type="button" class="btActions delete" onclick="javascript:fnRemoveLine('<%=Item.ProductId%>')"></button>
            </td>
        </tr>
        <% 
            }    
        %>
    </tbody>
</table>