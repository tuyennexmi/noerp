<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    
    string status = "";//ViewData["Status"].ToString();
    NEXMI.NMCloseMonthsWSI WSI = (NEXMI.NMCloseMonthsWSI)ViewData["WSI"];
    string id = WSI.CloseMonth.CloseMonth;
%>

<script type="text/javascript">
    $(function () {
        $(".tab").jqxTabs({ theme: theme, keyboardNavigation: false });
        LoadContentDynamic('CashBalanced', 'Accounting/CloseMonthBalanced', { closeMonth: '<%=id %>', account: '1111' });
    });

    $('.tab').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 0:
                LoadContentDynamic('CashBalanced', 'Accounting/CloseMonthBalanced', { closeMonth: '<%=id %>', account: '1111' });
                break;
            case 1:
                LoadContentDynamic('BankBalanced', 'Accounting/CloseMonthBalanced', { closeMonth: '<%=id %>', account: '1121' });
                break;
            case 2:
                LoadContentDynamic('CloseMonthAR', 'Accounting/CloseMonthAR', { closeMonth: '<%=id %>' });
                break;
            case 3:
                LoadContentDynamic('CloseMonthAP', 'Accounting/CloseMonthAP', { closeMonth: '<%= id %>' });
                break;
            case 4:
                LoadContentDynamic('CloseMonthPIS', 'Accounting/CloseMonthIC', { closeMonth: '<%= id %>' });
                break;
        }
    });

</script>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right">&nbsp;</td>
        </tr>
        <tr>
            <td>
                <div class="NMButtons">
            <%
                string functionId = NEXMI.NMConstant.Functions.CloseMonth;
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {
            %>
                    <button onclick="javascript:fnShowCloseMonthForm()" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
            %>
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
            <%--<%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Payments", current = status });%>--%>
        </div>
    </div>
    <table class="frmInput">
        <tr>
            <td>
                <label class="lbId">Sổ kế toán kỳ <label id="txtId"><%=id%></label></label>
                <table class="frmInput">
                    <%--<tr>
                        <td class="lbright">Số dư tiền đầu kỳ</td>
                        <td><%=WSI.CloseMonth.BeginAmount%></td>
                        <td class="lbright">Số dư tiền cuối kỳ</td>
                        <td><%=WSI.CloseMonth.EndAmount%></td>
                    </tr>
                    <tr>
                        <td class="lbright">Mua trong kỳ</td>
                        <td><%=WSI.CloseMonth.PurchaseAmount%></td>
                        <td class="lbright">Trả trong kỳ</td>
                        <td><%=WSI.CloseMonth.PaidAmount%></td>
                    </tr>
                    <tr>
                        <td class="lbright">Bán trong kỳ</td>
                        <td><%=WSI.CloseMonth.SalesAmount%></td>
                        <td class="lbright">Thu trong kỳ</td>
                        <td><%=WSI.CloseMonth.ReceivedAmount%></td>
                    </tr>--%>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                        <td><%=WSI.CloseMonth.Descriptions%></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="tab">
                    <ul>
                        <li style="margin-left: 25px;">Tiền mặt</li>
                        <li>Ngân hàng</li>
                        <li>Công nợ khách hàng</li>
                        <li>Công nợ nhà cung cấp</li>
                        <li>Tình hình tồn kho</li>
                        <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    </ul>
                    <div id='CashBalanced'>
                    </div>
                    <div id='BankBalanced'>
                    </div>
                    <div id='CloseMonthAR'>
                    </div>
                    <div id='CloseMonthAP'>
                    </div>
                    <div id='CloseMonthPIS'>
                    </div>
                    <div>
                        <table class="frmInput">                            
                        <%  ViewData["ModifiedDate"] = WSI.CloseMonth.ModifiedDate;
                            ViewData["ModifiedBy"] = WSI.CloseMonth.ModifiedBy;
                            ViewData["CreatedDate"] = WSI.CloseMonth.CreatedDate;
                            ViewData["CreatedBy"] = WSI.CloseMonth.CreatedBy;
                            Html.RenderPartial("LastModified"); %>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <%        Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.CloseMonth.CreatedBy });%>
</div>
