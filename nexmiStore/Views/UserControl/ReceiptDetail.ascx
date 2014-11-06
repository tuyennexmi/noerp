<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string status = ViewData["Status"].ToString();
    NEXMI.NMReceiptsWSI WSI = (NEXMI.NMReceiptsWSI)ViewData["WSI"];
    string id = WSI.Receipt.ReceiptId;
    string InvoiceId = WSI.Receipt.InvoiceId;
%>

<script type="text/javascript">
    $(function () {
        $(".tab").jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    $('.tab').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 1:
                LoadContentDynamic('Accounting', 'Accounting/GeneralJournals', { IssueId: '<%= id %>' });
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
                string functionReceipt = NEXMI.NMConstant.Functions.Receipts;
                //Kiểm tra quyền Insert
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                {
            %>
                    <button onclick="javascript:fnShowReceiptForm('', '', '')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                } 
                if (status == NEXMI.NMConstant.ReceiptStatuses.Draft || status == NEXMI.NMConstant.ReceiptStatuses.Cancelled)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                    {
            %>
                    <button onclick='javascript:fnShowReceiptForm("<%=ViewData["ReceiptId"]%>", "", "")' class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
            <% 
                    }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                    { 
            %>
                    <button onclick='javascript:fnDeleteReceipt("<%=ViewData["ReceiptId"]%>")' class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
            <%         
                    }
                }
                else
                {
           
                    if (GetPermissions.GetReconcile((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionReceipt))
                    {
            %>
                    <button onclick='javascript:fnShowReceiptForm("<%=ViewData["ReceiptId"]%>", "", "")' class="button"><%= NEXMI.NMCommon.GetInterface("RECONCILE", langId)%></button>
            <% 
                    }
            %>
                    <button onclick="javascript:fnPrintReceipt('<%=id%>', '<%= Session["Lang"].ToString() %>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
            <%         
                }
            %>
                </div>
            </td>
            <td align="right"></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            <% 
                if ((status == NEXMI.NMConstant.ReceiptStatuses.Draft)|(status == NEXMI.NMConstant.ReceiptStatuses.Approved))
                {
            %>
                <button onclick='javascript:fnConfirmReceipt("<%=ViewData["ReceiptId"]%>", "<%=NEXMI.NMConstant.ReceiptStatuses.Done%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>
                <button onclick='javascript:fnApproveReceipt("<%=ViewData["ReceiptId"]%>", "<%=NEXMI.NMConstant.ReceiptStatuses.Cancelled%>")' class="button"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
            <%
                }
                else if (status == NEXMI.NMConstant.ReceiptStatuses.Cancelled)
                { 
            %>
                <button onclick='javascript:fnApproveReceipt("<%=ViewData["ReceiptId"]%>", "<%=NEXMI.NMConstant.ReceiptStatuses.Draft%>")' class="button">Mở lại</button>
            <%
                }
            %>
        </div>
        <div class="divStatusBar">
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Receipts", current = status });%>
        </div>
    </div>
    <div class='divContentDetails'>
    <table class="frmInput">
        <tr>
            <td>
                <label class="lbId"><%= NEXMI.NMCommon.GetInterface("RECEIPT", langId) %> <label id="txtId"><%=ViewData["ReceiptId"]%></label></label>
                <table class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYER", langId) %></td>
                        <td><%=ViewData["CustomerName"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                        <td><%=ViewData["InvoiceId"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE", langId) %></td>
                        <td><%=ViewData["ReceiptDate"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                        <td><%=ViewData["TypeName"]%></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAY_AMOUNT", langId) %></td>
                        <td><%=ViewData["ReceiptAmount"]%></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_METHOD", langId) %></td>
                        <td><%=ViewData["PaymentMethodName"]%></td>
                    </tr>
                    <%--<tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></td>
                        <td><input type="text" id="txtTotalAmount" name="txtTotalAmount" readonly="readonly" value="<%=ViewData["TotalAmount"]%>" /></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DEBT", langId) %></td>
                        <td><input type="text" id="txtBalance" name="txtBalance" readonly="readonly" value="<%=ViewData["Balance"]%>" /></td>
                    </tr>--%>
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
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_BANK", langId) %></td>
                                <td><%= (WSI.ReceiveBank != null)? WSI.ReceiveBank.Name : "" %></td>
                            </tr>
                        <%  ViewData["ModifiedDate"] = WSI.Receipt.ModifiedDate;
                            ViewData["ModifiedBy"] = WSI.Receipt.ModifiedBy;
                            ViewData["CreatedDate"] = WSI.Receipt.CreatedDate;
                            ViewData["CreatedBy"] = WSI.Receipt.CreatedBy ;
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
        <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = ViewData["CreateBy"] });%>
    </div>
</div>