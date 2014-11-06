<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMAcceptanceWSI WSI = (NEXMI.NMAcceptanceWSI)ViewData["WSI"];
    //NEXMI.NMSalesOrdersWSI SOWSI = (NEXMI.NMSalesOrdersWSI)ViewData["SOWSI"];
    
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
            </td>
            <td align="right">&nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
                    <button onclick="javascript:fnConfirmAcceptance()" class="button"><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId)%></button>
                    <button onclick="javascript:fnDeleteAcceptance()" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId)%></button>
                    <button onclick="javascript:fnPrintAcceptance()" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId)%></button>
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
        </div>     
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Acceptance", current = WSI.Acceptance.StatusId });%>
        </div>
    </div>
    <div class='divContentDetails'>

    <form id="formAcceptance" action="" method="post">
        <input type="hidden" id="StatusId" name="StatusId" value="<%=WSI.Acceptance.StatusId%>" />
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId">Biên bản nghiệm thu <label id="txtId"><%=WSI.Acceptance.Id%></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SO_TITLE", langId)%></td>
                            <td><%=WSI.Acceptance.SalesOrderId%></td>
                            <%--<td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>--%>
                            <td></td>
                            <td><%--<%=SOWSI.Customer.CompanyNameInVietnamese%>--%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCEPTANCE_DATE", langId)%></td>
                            <td><%=WSI.Acceptance.AcceptanceDate.ToString("yyyy-MM-dd")%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId)%></td>
                            <td><%=WSI.Acceptance.Reference%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                            <td colspan='3'><%=WSI.Acceptance.Description%>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>

        <div class="tabs">
            <ul>
                <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
            </ul>
            <div>
                <table class="frmInput">
                    <tr>
                        <td>
                            <table style="width: 100%" id="tbSalesOrderDetail" class="tbDetails">
                                <thead>
                                    <tr>
                                        <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                        <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                        <th><%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId)%></th>
                                        
                                    </tr>
                                </thead>
                                <tbody id="tbodyDetails">
                                    <%Html.RenderPartial("DetailLines", "Acceptance");%>
                                </tbody>
                                <tfoot id="formSODetail" class="form-to-validate">
                                    <%--<%Html.RenderAction("SalesOrderDetailForm", "Sales");%>--%>
                                </tfoot>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table style="width: 100%">
                                <tr>
                                    <td valign="top">
                                        <table style="width: 100%">
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                <td align="right" style="width: 120px;"><label id="lbAmount"><%=WSI.Acceptance.Amount.ToString("N3")%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalDiscount"><%=WSI.Acceptance.Discount.ToString("N3")%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalVATRate"><%=WSI.Acceptance.Tax.ToString("N3")%></label></td>
                                            </tr>
                                            <tr>
                                                <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>: </b></td>
                                                <td align="right"><label id="lbTotalAmount"><%=WSI.Acceptance.TotalAmount.ToString("N3")%></label></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    <%Html.RenderAction("Logs", "Messages", new { ownerId = WSI.Acceptance.Id, sendTo = WSI.Acceptance.CreatedBy });%>
    
    </div>

</div>

<script type="text/javascript">

    $(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });        
    });

    function fnConfirmAcceptance() {
        var id = $('#txtId').html();
        $.post(appPath + 'Acceptance/ConfirmFinish', { id: id }, function (data) {
            if (data.error != '') {
                alert(data.error);
            } else {
                $(window).hashchange();
            }
        });
    }

    function fnDeleteAcceptance() {
        var id = $('#txtId').html();
        $.post(appPath + 'Acceptance/Delete', { id: id }, function (data) {
            if (data.error != '') {
                alert(data.error);
            } else {
                alert("Đã xóa thành công !");
                history.back();
            }
        });
    }

    function fnPrintAcceptance() {

    }
</script>
