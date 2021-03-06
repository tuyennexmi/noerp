﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMAcceptanceWSI WSI = (NEXMI.NMAcceptanceWSI)ViewData["WSI"];
    NEXMI.NMSalesOrdersWSI SOWSI = (NEXMI.NMSalesOrdersWSI)ViewData["SOWSI"];
    
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
                    <button onclick="javascript:fnSaveOrUpdateAcceptance()" class="button"><%= NEXMI.NMCommon.GetInterface("SAVE_AND_VIEW", langId) %></button>
                    <button onclick="javascript:$('#formAcceptance').jqxValidator('hide'); history.back();" class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
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
                    <label class="lbId">Biên bản nghiệm thu<label id="txtId"><%=WSI.Acceptance.Id%></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SO_TITLE", langId)%></td>
                            <td><input type="text" name="SalesOrderId" value="<%=WSI.Acceptance.SalesOrderId%>" readonly="readonly"/></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                            <td><%=SOWSI.Customer.CompanyNameInVietnamese%></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCEPTANCE_DATE", langId)%></td>
                            <td><input type="text" id="AcceptanceDate" name="AcceptanceDate" value="<%=WSI.Acceptance.AcceptanceDate.ToString("yyyy-MM-dd")%>" /></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REFERENCE", langId)%></td>
                            <td><input type="text" name="Reference" value="<%=WSI.Acceptance.Reference%>" /></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                            <td colspan='3'>
                                <textarea id="Description" name="Description" cols="1" rows="3" style="width: 100%"><%=WSI.Acceptance.Description%></textarea>
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
                                        <th></th>
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
    
    </div>
</div>

<script type="text/javascript">

    $(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $("#AcceptanceDate").datepicker({ changeMonth: true, changeYear: true, showButtonPanel: true, dateFormat: "yy-mm-dd" });
    });

    function fnSaveOrUpdateAcceptance() {
        var postData = $("#formAcceptance").serializeArray();
        $.ajax({
            url: appPath + "Acceptance/SaveOrUpdateAcceptance",
            type: "POST",
            data: postData,
            success: function (data, textStatus, jqXHR) {
                //$("#mainContent").html(data);
                if (data.error == '')
                    LoadContent('', 'Acceptance/Details?id=' + data.id);
                else
                    alert(data.error);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $("#mainContent").html(errorThrown);
            }
        });
    }

    function fnShowDetailLineForm(productId) {
        $('#' + productId).remove();
        $.ajax({
            url: appPath + 'Acceptance/DetailLineForm',
            type: "POST",
            data: { productId: productId },
            success: function (data, textStatus, jqXHR) {
                $('#tbodyDetails').append(data);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert('Không thể tải dữ liệu.\nVui lòng thử lại hoặc liên hệ người quản trị để khắc phục.\nChi tiết lỗi: ' + errorThrown);
            }
        });
    }

</script>
