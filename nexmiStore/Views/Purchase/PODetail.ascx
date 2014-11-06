<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseOrders.js" type="text/javascript"></script>
<script src="<%=Url.Content("~")%>Scripts/forApp/PurchaseInvoices.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
    });
</script>
<% 
    string id = ViewData["Id"].ToString(), current = ViewData["StatusId"].ToString(),
        func = ViewData["FunctionId"].ToString();
    string typeId = ViewData["TypeId"].ToString();
    string previousId = NEXMI.NMCommon.PreviousId(id, "Id", "PurchaseOrders", "where OrderTypeId = '" + typeId + "'");
    string nextId = NEXMI.NMCommon.NextId(id, "Id", "PurchaseOrders", "where OrderTypeId = '" + typeId + "'");
    NEXMI.NMPurchaseOrdersWSI wsi = (NEXMI.NMPurchaseOrdersWSI)ViewData["WSI"];
%>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td></td>
                <td align="right">&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <div class="NMButtons">
                        <%  if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    { %>
                        <button onclick='javascript:fnShowPOForm("", "", "<%=typeId%>", "<%=current%>")' class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%  }
                    if (current == NEXMI.NMConstant.POStatuses.Draft || current == NEXMI.NMConstant.POStatuses.Order)
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {
                %>
                            <button onclick="javascript:fnShowPOForm('<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                <%      }
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        { %>
                            <button onclick="javascript:fnDeletePO('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                        <%} %>
                <%
                    }
                    else if (GetPermissions.GetReconcile((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                %>
                        <button onclick="javascript:fnShowPOForm('<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("RECONCILE", langId)%></button>
                <%
                    }
                    if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                    %>
                        <button onclick="javascript:fnShowPOForm('<%=id%>', 'Copy')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
                    <%}
                        if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    { %>
                        <button onclick="javascript:fnPrintPOrder('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %> PO</button>
                    <%} %>                    
                    </div>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadPODetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadPODetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>
                                <%Html.RenderPartial("POSV"); %>    
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons NMButtons">
                <%
                    string status = NEXMI.NMConstant.POStatuses.Order;
                    if (current == NEXMI.NMConstant.POStatuses.Draft)
                    {
                        %>
                        <button class="color red button" onclick="javascript:fnSendPQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("SEND_REQUIRE", langId) %></button>
                        <button class="button" onclick="javascript:fnConfirmPQuotation('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Order%>', '<%=NEXMI.NMConstant.POType.PurchaseOrder%>','<%=wsi.Supplier.CustomerId %>','<%=wsi.Supplier.MaxDebitAmount %>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %></button>
                        <button class="button" onclick="javascript:fnCancelQuotation('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Canceled%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                        <%
                    }
                    if(current == NEXMI.NMConstant.POStatuses.Sent)
                    {                        
                %>
                        <button class="button" onclick="javascript:fnSendPQuotation('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("SEND_REQUIRE", langId) %></button>
                        <button class="color red button" onclick="javascript:fnConfirmPQuotation('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Order%>', '<%=NEXMI.NMConstant.POType.PurchaseOrder%>','<%=wsi.Supplier.CustomerId %>','<%=wsi.Supplier.MaxDebitAmount %>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM_ORDER", langId) %></button>
                        <button class="button" onclick="javascript:fnCancelQuotation('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Canceled%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
                <%
                    }
                    else if (current == NEXMI.NMConstant.POStatuses.Canceled)
                    {
                %>
                        <button class="color red button" onclick="javascript:fnReactivelQuotation('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Draft%>')">Phục hồi đơn hàng</button>
                <%
                    }
                    else if (current == NEXMI.NMConstant.POStatuses.Done | current == NEXMI.NMConstant.POStatuses.Inventory | current==NEXMI.NMConstant.POStatuses.Invoice)
                    {
                        //if (createInvoice == NEXMI.NMConstant.CreateInvoice.OnDemand)
                        //{
                %>          
                            <button class="color red button" onclick="javascript:fnSaveImportFromPO('<%=id%>')">Nhận sản phẩm</button>
                            <button class="color red button" onclick="javascript:fnCreatePInvoice('', '<%=id%>')">Nhận hóa đơn</button>
                <%
                        //}
                    }
                    else
                    {   
                        if (current == NEXMI.NMConstant.POStatuses.Order)
                        {
                %>
                        <button class="color red button" onclick="javascript:fnSaveImportFromPO('<%=id%>')">Nhận sản phẩm</button>
                        <button class="color red button" onclick="javascript:fnCreatePInvoice('', '<%=id%>')">Nhận hóa đơn</button>
                        <button class="color red button" onclick="javascript:fnCancelPO('<%=id%>', '<%=NEXMI.NMConstant.POStatuses.Canceled%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> đơn hàng</button>
                <%
                        }
                    }
                %>
            </div>     
            <div class="divStatusBar">
                <%
                    Html.RenderAction("StatusBar", "UserControl", new { objectName = "PurchaseOrders", current = ViewData["StatusId"] });                
                %>
            </div>
        </div>
        
        <div class='divContentDetails'>
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%=ViewData["Tilte"] %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                                <td><%=ViewData["CustomerName"]%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ORDER_DATE", langId) %></td>
                                <td><%=((DateTime)ViewData["OrderDate"]).ToString("dd-MM-yyyy")%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %></td>
                                <td><%=ViewData["Reference"]%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DATE_EXPECTED", langId) %></td>
                                <td><%=((DateTime)ViewData["DeliveryDate"]).ToString("dd-MM-yyyy")%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                                <td><%=wsi.Order.Transportation%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td><%=ViewData["StockName"]%></td>
                            </tr>
                            <%--<tr>
                                <td class="lbright">Loại hàng mua</td>
                                <td><%=ViewData["invoiceType"]%></td>
                                <td></td>
                                <td></td>
                            </tr>--%>
                        </table>   
                    </td>
                </tr>
            </table>
            <div class="tabs">
                <ul>
                    <li style="margin-left: 15px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("INVOICE", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("IMPORT", langId) %></li>
                </ul>
                <div style="padding: 10px;">
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbPODetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %> (%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("VAT", langId) %>(%)</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                       <% 
                                           int count = 1;
                                           List<NEXMI.PurchaseOrderDetails> Details = (List<NEXMI.PurchaseOrderDetails>)Session["Details"];
                                           foreach (NEXMI.PurchaseOrderDetails Item in Details)
                                            {
                                        %>
                                         <tr>
                                            <td><%=count++  %></td>
                                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                                            <td><%=Item.Quantity.ToString("N3")%></td>
                                            <td><%=Item.Price.ToString("N3")%></td>
                                            <td><%=Item.Discount.ToString("N3") %></td>
                                            <td><%=Item.Tax.ToString("N3")%></td>
                                            <td align="right"><%=Item.Amount.ToString("N3")%></td>
                                            <td><%=Item.Description%></td>
                                        </tr>
                                        <% 
                                            }    
                                        %>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table style="width: 100%">
                                    <tr>
                                        <td><%=ViewData["Description"]%></td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("SUB_TOTAL", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbTotalAmount"><%=ViewData["Amount"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("DISCOUNT", langId) %>: </b></td>
                                                    <td align="right" style="width: 120px;"><label id="lbDiscountAmount"><%=ViewData["discountAmount"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("VAT_AMOUNT", langId) %>: </b></td>
                                                    <td align="right"><label id="lbTotalVATRate"><%=ViewData["Tax"]%></label></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="right"><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId)%>: </b></td>
                                                    <td align="right"><label id="lbInvoiceAmount"><%=ViewData["TotalAmount"]%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="padding: 10px;">
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BUYER", langId) %></td>
                            <td><%=ViewData["Buyer"]%></td>
                            <%--<td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_INVOICE", langId) %></td>
                            <td><input type="checkbox" /></td>--%>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_TIME", langId)%></td>
                            <td><%=((DateTime)ViewData["PaymentDate"]).ToString("dd-MM-yyyy")%></td>
                            <%--<td class="lbright">Chức vụ tài chính</td>
                            <td></td>--%>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_GOOD", langId) %></td>
                            <%if (wsi.Order.Delivered)
                              {%>
                                <td><input type="checkbox" disabled="disabled" checked="checked" /></td>
                            <%}
                              else
                              {%>
                                <td><input type="checkbox" disabled="disabled" /></td>
                            <%} %>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("RECEIVED_INVOICE", langId) %></td>
                            <%if (wsi.Order.InvoiceReceive)
                              {%>
                                <td><input type="checkbox" disabled="disabled" checked="checked" /></td>
                            <%}
                              else
                              {%>
                                <td><input type="checkbox" disabled="disabled" /></td>
                            <%} %>
                        </tr>
                        <%  ViewData["ModifiedDate"] = wsi.Order.ModifiedDate;
                            ViewData["ModifiedBy"] = wsi.Order.ModifiedBy;
                            ViewData["CreatedDate"] = wsi.Order.CreatedDate;
                            ViewData["CreatedBy"] = wsi.Order.CreatedBy;
                            Html.RenderPartial("LastModified"); %>
                    </table>
                </div>
                <div id='Invoice' style="padding:10px;">
                    <table id="tbReceipts" class="tbDetails">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th width="11%"><%= NEXMI.NMCommon.GetInterface("INVOICE_NO", langId) %></th>
                                <th width="10%"><%= NEXMI.NMCommon.GetInterface("INVOICE_DATE", langId) %></th>
                                <th width="12%"><%= NEXMI.NMCommon.GetInterface("ID", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %></th>
                                <th width="6%"></th>
                            </tr>
                        </thead>
                        <tbody>
                        <%          
                            double total = 0;
                            if (wsi.Invoices.Count > 0)
                            {   
                                count = 1;
                                double amount = 0;
                                foreach (NEXMI.PurchaseInvoices Item in wsi.Invoices)
                                {
                                    amount = Item.TotalAmount;
                                    total += amount;
                        %>
                                <tr ondblclick="javascript:fnLoadPIDetail('<%=Item.InvoiceId%>')">
                                    <td ><%=count++%></td>
                                    <td ><%=Item.InvoiceId%></td>
                                    <td ><%=Item.InvoiceDate.ToString("dd-MM-yyyy")%></td>
                                    <td ><%=Item.Reference%></td>
                                    <td ><%=Item.DescriptionInVietnamese%></td>
                                    <td align="right" ><%= amount.ToString("N3")%></td>
                                    <td >
                                            <a href="javascript:fnLoadPIDetail('<%=Item.InvoiceId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                    <%
                                        if (Item.InvoiceStatus == NEXMI.NMConstant.SIStatuses.Draft)    // | Item.Invoice.InvoiceStatus == NEXMI.NMConstant.PIStatuses.Open)
                                    {
                                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                                        {                        
                                    %>
                                            <a href="javascript:fnPopupPIDialog('<%=Item.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                    <%  }
                                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                                        {
                                    %>
                                            <a href="javascript:fnDeletePI('<%=Item.InvoiceId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                                    <% 
                                        }
                                    }
                                    %>
                                    </td>
                                </tr>
                        <%
                                }
                            }
                        %>
                                <tr>
                                    <td colspan='5'><b><%= NEXMI.NMCommon.GetInterface("TOTAL_AMOUNT", langId) %>:</b></td>
                                    <td align="right"><b><%=total.ToString("N3") %></b></td>
                                    <td align="right"></td>
                                </tr>
                        </tbody>
                    </table>
                </div>
                <div style="padding:10px;">
                    <table id="Table1" class="tbDetails" border=".1" style="width: 100%;">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th><%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                        <%  
                            if (wsi.Imports.Count > 0)
                            {
                                count = 1;
                                foreach (NEXMI.Imports Item in wsi.Imports)
                                {
                            %>
                                <tr ondblclick="javascript:fnLoadImportDetail('<%=Item.ImportId %>')">
                                    <td width="3%"><%=count++%></td>
                                    <td><%= Item.ImportId %></td>
                                    <td ><%= Item.ImportDate.ToString("dd-MM-yyyy")%></td>
                                    <td><%= Item.DescriptionInVietnamese %></td>
                                    <td ><%=NEXMI.NMCommon.GetStatusName(Item.ImportStatus, langId)%></td>
                                    <td >
                                            <a href="javascript:fnLoadImportDetail('<%=Item.ImportId %>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                    <%
                                        if (Item.ImportStatus == NEXMI.NMConstant.IMStatuses.Draft)
                                        {
                                            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                                            {                        
                                    %>
                                            <a href="javascript:fnPopupImportDialog('<%=Item.ImportId %>', '', '')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                    <%      }
                                            if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                                            {
                                    %>
                                            <a href="javascript:fnDeleteImport('<%=Item.ImportId %>')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                                    <% 
                                            }
                                        }
                                    %>
                                    </td>
                                <%} %>
                                </tr>
                        <%
                            }
                        %>
                        </tbody>
                    </table>
                </div>
            </div>
        
            <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = ViewData["SendTo"] });%>
        </div>
    </div>
</div>