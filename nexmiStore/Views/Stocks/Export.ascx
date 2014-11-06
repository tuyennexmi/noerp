<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    NEXMI.NMExportsWSI WSI = (NEXMI.NMExportsWSI)ViewData["WSI"];
    string id = WSI.Export.ExportId;
    string previousId = NEXMI.NMCommon.PreviousId(id, "ExportId", "Exports", "");
    string nextId = NEXMI.NMCommon.NextId(id, "ExportId", "Exports", ""); 
    string orderId = WSI.Export.Reference;
    string current = WSI.Export.ExportStatus;
    string functionId = NEXMI.NMConstant.Functions.Export;
%>
<div id="divExport">
    <script type="text/javascript">
        $(function () {
            $("#tabsExport").jqxTabs({ theme: theme, keyboardNavigation: false });
            if ('<%=WSI.Export.ExportTypeId%>' == '<%=NEXMI.NMConstant.ExportType.Transfer%>') {
                $('.importStock').show();
            } else {
                $('.importStock').hide();
            }
        });

        $('#tabsExport').bind('selected', function (event) {
            var item = event.args.item;
            switch (item) {
                case 2:
                    LoadContentDynamic('Accounting', 'Accounting/InventoryJournals', { IssueId: '<%=id %>' });
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
            <% 
                if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {    
            %>
                    <button onclick="javascript:fnExportProduct('', '', '', '<%=WSI.Export.ExportTypeId%>')" class="button color red"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
            <% 
                }
            %>
            <% 
                if (current == NEXMI.NMConstant.EXStatuses.Draft || current == NEXMI.NMConstant.EXStatuses.Ready)
                {
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
            %>
                    <button onclick="javascript:fnExportProduct('<%=id%>', '', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
            <% 
                    }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {    
            %>
                    <button onclick="javascript:fnDeleteExport('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
            <%     
                    }
                }
                if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {
            %>
                    <button onclick="javascript:fnExportProduct('<%=id%>', '', 'Copy', '')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
            <% 
                }
                if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                {    
            %>
                    <button onclick="javascript:fnPrintExport('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
            <% 
                }    
            %>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadExportDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadExportDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>
                                <%Html.RenderPartial("ExportSV");%>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <div class="NMButtons">
    <% 
        if (current == NEXMI.NMConstant.EXStatuses.Delivered)
        {
            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
            {
    %>
                <button onclick="javascript:fnExportProduct('<%=id%>', '', '', '')" class="button color red"><%= NEXMI.NMCommon.GetInterface("RECONCILE", langId)%></button>
    <% 
            }
            if (!String.IsNullOrEmpty(orderId))
            {
    %>
        <%--<button class="color red button" onclick="javascript:fnCreateInvoice('', '<%=orderId%>', '')"><%= NEXMI.NMCommon.GetInterface("CREATE_INVOICE", langId) %></button>--%>
    <% 
            }
            if (NEXMI.NMCommon.GetSettingValue("RETURN_PRODUCT") == 1)
            {
    %>
                <button class="button" onclick="javascript:fnReturn('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("RETURN_PRODUCTS", langId)%></button>
    <%      }
        }
        else
        {
            if (WSI.Export.ExportTypeId == NEXMI.NMConstant.ExportType.Transfer)
            {
    %>
                    <button class="color red button" onclick="javascript:fnConfirmTransfer('<%=id%>', '<%=NEXMI.NMConstant.EXStatuses.Delivered%>')"><%= NEXMI.NMCommon.GetInterface("STOCK_MOVE", langId) %></button>
    <%
            }
            else
            {
    %>
                    <button class="color red button" onclick="javascript:fnDelivery('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("DELIVERY", langId) %></button>
    <% 
            }    
    %>
                    <button class="button" onclick="javascript:fnCancelExport('<%=id%>', '<%=NEXMI.NMConstant.EXStatuses.Canceled%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
    <% 
        }    
    %>
                </div>
            </div>     
            <div class="divStatusBar">
                <%
                    Html.RenderAction("StatusBar", "UserControl", new { objectName = "Exports", current = current });
                %>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formExport" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%= NEXMI.NMCommon.GetInterface("EXPORT_ORDER", langId) %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <td>[<%=WSI.Customer.Code %>] <%=(WSI.Customer == null) ? "" : WSI.Customer.CompanyNameInVietnamese%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><%=orderId%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %></td>
                                <td><%=WSI.Export.ExportDate.ToString("dd-MM-yyyy")%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                                <td valign="top"><%=GlobalValues.GetType( WSI.Export.ExportTypeId)%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %></td>
                                <td>[<%=WSI.Stock.Id%>] <%=WSI.Stock.Translation.Name %></td>
                                <td class="lbright importStock"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td class="importStock"><%=NEXMI.NMCommon.GetName(WSI.Export.ImportStockId, langId)%></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsExport">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("WH_RECORD", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbExportProductDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("REQUIRE_QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                    <% 
                                        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
                                        {
                                    %>
                                            <th><%= NEXMI.NMCommon.GetInterface("SERIAL", langId) %></th>
                                    <% 
                                        }
                                    %>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                <% 
                                    int i = 1;
                                    foreach (NEXMI.ExportDetails Item in WSI.Details)
                                    {
                                        //ProductWSI = WSI.ProductWSIs.Select(x => x).Where(x => x.Product.ProductId == Item.ProductId).FirstOrDefault();
                                %>
                                        <tr><td><%=i++%></td>
                                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                                            <td><%=Item.RequiredQuantity.ToString("N3")%></td>
                                            <td><%=Item.Quantity.ToString("N3")%></td>
                                    <% 
                                        if (NEXMI.NMCommon.GetSetting(NEXMI.NMConstant.Settings.SERIALNUMBER))
                                        {
                                    %>
                                            <td><%=Session["SNs" + Item.ProductId]%></td>
                                    <% 
                                        }
                                    %>
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
                                        <td><%=WSI.Export.DescriptionInVietnamese%></td>
                                        <td valign="top"></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("DELIVERY_METHOD", langId) %></td>
                            <td><%=NEXMI.NMCommon.GetParameterName(WSI.Export.DeliveryMethod)%></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTER", langId) %></td>
                            <td><%=NEXMI.NMCommon.GetCustomerName(WSI.Export.CarrierId)%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                            <td><%=WSI.Export.Transport%></td>
                        </tr>
                        <%  ViewData["ModifiedDate"] = WSI.Export.ModifiedDate;
                        ViewData["ModifiedBy"] = WSI.Export.ModifiedBy;
                        ViewData["CreatedDate"] = WSI.Export.CreatedDate;
                        ViewData["CreatedBy"] = WSI.Export.CreatedBy ;
                        Html.RenderPartial("LastModified"); %>
                    </table>
                </div>
                <div id='Accounting'></div>
            </div>
        </form>    
        
            <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.Export.CreatedBy });%>
        </div>
    </div>
</div>
