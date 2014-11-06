<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    NEXMI.NMImportsWSI WSI = (NEXMI.NMImportsWSI)Session["ImportWSI"];
%>
<script type="text/javascript">
    function fnReceiveConfirm() {
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
                url: appPath + 'Stocks/ReceiveProducts',
                data: {
                    id: '<%=WSI.Import.ImportId%>',
                    quantities: arr.toString()
                },
                beforeSend: function () {
                    fnDoing();
                },
                success: function (data) {
                    if (data.error == "") {
                        closeWindow('<%=windowId%>');
                        fnLoadImportDetail(data.id);
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
        } else {
            alert('Bạn phải nhập số lượng cho ít nhất 1 sản phẩm!');
        }
    }
</script>
<div class="windowContent">
    <table class="tbDetails">
        <thead>
            <tr>
                <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                <th>Số lượng<br />yêu cầu</th>
                <th>Số lượng<br />thực tế</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
    <% 
        foreach (NEXMI.ImportDetails Item in WSI.Details)
        {
    %>
            <tr>
                <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                <td style="width: 120px;"><%=Item.RequiredQuantity.ToString("N3")%></td>
                <td style="width: 120px;">
                    <input type="text" style="width: 100px; margin: 0; padding: 1px;" class="txtQuantity" id="txtNumber<%=Item.OrdinalNumber%>" value="<%=Item.RequiredQuantity.ToString("N3")%>" />
                    <script type="text/javascript">
                        $('#txtNumber<%=Item.OrdinalNumber%>').autoNumeric('init', { vMin: '0.00', vMax: '<%=Item.RequiredQuantity%>' });
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
        <button onclick="javascript:fnReceiveConfirm()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>