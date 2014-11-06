<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    string YN = ViewData["Enable"].ToString();
%>
<script type="text/javascript">
    function fnDeliveryConfirm() {
        var arr = new Array();
        var flag = false;
        $('.txtQuantity').each(function () {
            var qty = $(this).autoNumeric('get');
            if (qty == '') qty = 0;
            arr.push(qty);
            if (qty > 0)
                flag = true;
        });
        if (flag) {
            $.ajax({
                type: 'POST',
                url: appPath + 'Stocks/DeliveryProducts',
                data: {
                    id: '<%=ViewData["Id"]%>',
                    quantities: arr.toString()
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data.error == "") {
                        closeWindow('<%=windowId%>');
                        fnLoadExportDetail(data.id);
                        $(window).hashchange();
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
        }
    }
</script>
<div class="windowContent">
<%  if (YN == "No")
    { 
%>
        <h3 style='padding-left:10px; color: #800000;'>Bạn không thể xuất đủ hàng vì tồn kho không còn đủ số lượng yêu cầu!</h3>
<%  }
%>
    <table class="tbDetails">
        <thead>
            <tr>
                <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                <th>Số lượng<br />tồn</th>
                <th>Số lượng<br />yêu cầu</th>
                <th>Số lượng<br />thực xuất</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
    <% 
        List<NEXMI.NMProductInStocksWSI> pisList = (List<NEXMI.NMProductInStocksWSI>)Session["PISList"];
        NEXMI.NMExportsWSI WSI = (NEXMI.NMExportsWSI)Session["ExportWSI"];
        NEXMI.NMProductInStocksWSI pisWSI;
        string status = "";
        foreach (NEXMI.ExportDetails Item in WSI.Details)
        {
            pisWSI = pisList.Find(i => i.PIS.ProductId == Item.ProductId);
            double inStock = pisWSI.PIS.BeginQuantity + pisWSI.PIS.ImportQuantity - pisWSI.PIS.ExportQuantity;
            if(inStock < Item.RequiredQuantity)
                status = "#CC3300";
            else
                status = "#0033CC";
    %>
            <tr style='color:<%=status %>'>
                <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                <td style="width: 120px;"><%=inStock.ToString("N3")%></td>
                <td style="width: 120px;"><%=Item.RequiredQuantity.ToString("N3")%></td>
                <td style="width: 120px;">
                    <input type="text" style="width: 100px; margin: 0; padding: 1px;" class="txtQuantity" id="txtNumber<%=Item.OrdinalNumber%>" value="<%=Item.RequiredQuantity.ToString("N3")%>" />
                    <script type="text/javascript">
                        $('#txtNumber<%=Item.OrdinalNumber%>').autoNumeric('init', { vMin: '0.000', vMax: '1000000.000' });
                    </script>
                </td>
                <td><button type="button" class="btActions reset" onclick="javascript:$('#txtNumber<%=Item.OrdinalNumber%>').val('0');"></button></td>
            </tr>
    <%
        }
    %>
        </tbody>
    </table>

</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnDeliveryConfirm()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>