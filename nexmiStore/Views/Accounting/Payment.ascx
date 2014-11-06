<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string id = ViewData["PaymentId"].ToString();
    string status = ViewData["Status"].ToString();
    NEXMI.NMPaymentsWSI WSI = (NEXMI.NMPaymentsWSI)ViewData["WSI"];
    string InvoiceId = WSI.Payment.InvoiceId;
%>

<script type="text/javascript">
    $(function () {
        $(".tab").jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    $('.tab').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 1:
                LoadContentDynamic('Accounting', 'Accounting/GeneralJournals', { IssueId: '<%=id %>' });
                break;
            case 2:
                LoadContentDynamic('Invoice', 'Accounting/GeneralJournals', { IssueId: '<%= InvoiceId %>' });
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
                string functionId = NEXMI.NMConstant.Functions.Payments;
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {
            %>
                    <button onclick="javascript:fnShowPaymentForm('', '', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
                if (status == NEXMI.NMConstant.PaymentStatuses.Draft || status == NEXMI.NMConstant.PaymentStatuses.Cancelled)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
            %>
                    <button onclick="javascript:fnShowPaymentForm('<%=id%>', '', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
            <% 
                    }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    { 
            %>
                    <button onclick="javascript:fnDeletePayment('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
            <%         
                    }
                }
                else
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
            %>
                    <button onclick="javascript:fnShowPaymentForm('<%=id%>', '', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("RECONCILE", langId)%></button>
            <%      
                    }
            %>
                    <button onclick="javascript:fnPrintPayment('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
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
            <% 
                if (status == NEXMI.NMConstant.PaymentStatuses.Draft)
                {
                    if (GetPermissions.GetApproval((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
            %>
                    <button onclick='javascript:fnApprovePayment("<%=id%>", "<%=NEXMI.NMConstant.PaymentStatuses.Approved%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("APPROVE", langId) %></button>
            <%      }
            %>

                    <button onclick='javascript:fnApprovePayment("<%=id%>", "<%=NEXMI.NMConstant.PaymentStatuses.Cancelled%>")' class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
            <% 
                }
                else if (status == NEXMI.NMConstant.PaymentStatuses.Approved)
                { 
            %>
                    <button onclick='javascript:fnConfirmPayment("<%=id%>", "<%=NEXMI.NMConstant.PaymentStatuses.Done%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>
                    <button onclick='javascript:fnApprovePayment("<%=id%>", "<%=NEXMI.NMConstant.PaymentStatuses.Draft%>")' class="button">Mở lại</button>
            <%     
                }
                else if (status == NEXMI.NMConstant.PaymentStatuses.Cancelled)
                { 
            %>
                    <button onclick='javascript:fnApprovePayment("<%=id%>", "<%=NEXMI.NMConstant.PaymentStatuses.Draft%>")' class="button">Mở lại</button>
            <%
                }
            %>
        </div>
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Payments", current = status });%>
        </div>
    </div>

    <div class='divContentDetails'>
    <table class="frmInput">
        <tr>
            <td>
                <label class="lbId"><%= NEXMI.NMCommon.GetInterface("PAY_RECEIPT", langId) %> <label id="txtId"><%=id%></label></label>
                <table class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECIPIENT", langId) %></td>
                        <td><%=ViewData["CustomerName"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                        <td><%=ViewData["InvoiceId"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                        <td><%=ViewData["PaymentDate"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAY_FOR", langId) %></td>
                        <td><%=ViewData["TypeName"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAY_AMOUNT", langId) %></td>
                        <td><%=ViewData["PaymentAmount"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></td>
                        <td><%=ViewData["PaymentMethodName"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                        <td colspan="3"><%=ViewData["Description"]%></td>
                    </tr>
                </table>
            </td>
        </tr>

        <tr>
            <td>
                <div class="tab">
                    <ul>
                        <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                        <li><%= NEXMI.NMCommon.GetInterface("ACCOUNTING", langId) %></li>
                        <li><%= NEXMI.NMCommon.GetInterface("INVOICE", langId) %></li>
                    </ul>
                    <div>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAY_BANK", langId) %></td>
                                <td><%= (WSI.PaymentBank != null)? WSI.PaymentBank.Name : "" %></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_BANK", langId) %></td>
                                <td><%= (WSI.ReceiveBank != null)? WSI.ReceiveBank.Name : "" %></td>
                            </tr>
                        <%  ViewData["ModifiedDate"] = WSI.Payment.ModifiedDate;
                            ViewData["ModifiedBy"] = WSI.Payment.ModifiedBy;
                            ViewData["CreatedDate"] = WSI.Payment.CreatedDate;
                            ViewData["CreatedBy"] = WSI.Payment.CreatedBy ;
                            Html.RenderPartial("LastModified"); %>
                        </table>
                    </div>
                    <div id='Accounting'>                    
                    </div>
                    <div id='Invoice'>                    
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = ViewData["CreatedBy"] });%>
    </div>
</div>
