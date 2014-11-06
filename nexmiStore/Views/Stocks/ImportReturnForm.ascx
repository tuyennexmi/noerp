<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    NEXMI.NMImportsWSI WSI = (NEXMI.NMImportsWSI)Session["ImportWSI"];
%>
<script type="text/javascript">
    function fnImportReturnConfirm() {
        var arr = new Array();
        $('.txtQuantity').each(function () {
            arr.push($(this).autoNumeric('get'));
        });
        $.ajax({
            type: 'POST',
            url: appPath + 'Stocks/ImportReturnProducts',
            data: {
                id: '<%=WSI.Import.ImportId%>',
                quantities: arr.toString(),
                description: $('#txtImportReturnDescription').val()
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
</script>
<div class="windowContent">
    <table class="tbDetails">
        <thead>
            <tr>
                <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                <th>Số lượng<br />đã nhập</th>
                <th>Số lượng<br />trả</th>
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
                    <input type="text" style="width: 100px; margin: 0; padding: 1px;" class="txtQuantity" id="txtNumber<%=Item.OrdinalNumber%>" value="0" />
                    <script type="text/javascript">
                        $('#txtNumber<%=Item.OrdinalNumber%>').autoNumeric('init', { vMin: '0.00', vMax: '1000000.00' });
                    </script>
                </td>
                <td><button type="button" class="btActions reset" onclick="javascript:$('#txtNumber<%=Item.OrdinalNumber%>').val('0');"></button></td>
            </tr>
    <%
        }
    %>
        </tbody>
    </table>
    <textarea id="txtImportReturnDescription" cols="1" rows="4" style="width: 98%;" placeholder="<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>..."></textarea>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnImportReturnConfirm()" class="button medium"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>