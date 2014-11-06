<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string functionImport = NEXMI.NMConstant.Functions.Import;
    string id = ViewData["importId"].ToString();
    string previousId = NEXMI.NMCommon.PreviousId(id, "ImportId", "Imports", "");
    string nextId = NEXMI.NMCommon.NextId(id, "ImportId", "Imports", "");
    string status = ViewData["status"].ToString();
    string importType = ViewData["ImportTypeId"].ToString();
    NEXMI.NMImportsWSI WSI = (NEXMI.NMImportsWSI)Session["CurrentObj"];
%>


<script type="text/javascript">
    $(function () {
        $("#tabsImport").jqxTabs({ theme: theme, keyboardNavigation: false });
    });

    $('#tabsImport').bind('selected', function (event) {
        var item = event.args.item;
        switch (item) {
            case 2:
                LoadContentDynamic('Accounting', 'Accounting/InventoryJournals', { IssueId: '<%=id %>' });
                break;
        }
    });

</script>

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
                <% 
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                    {
                %>
                        <button onclick="javascript:fnPopupImportDialog('', '')" class="button color red"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%  }
                    if (status == NEXMI.NMConstant.IMStatuses.Draft || status == NEXMI.NMConstant.IMStatuses.Ready)
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                        {
                %>
                        <button onclick="javascript:fnPopupImportDialog('<%=id%>', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                <%
                        }
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                        { 
                %>
                        <button onclick="javascript:fnDeleteImport('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                <%
                        }
                    }
                    if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                    {
                %>
                        <button onclick="javascript:fnPopupImportDialog('<%=id%>', 'Copy')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
                <%
                    }
                    if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
                    {
                %>
                        <button onclick="javascript:fnPrintImport('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("PRINT", langId) %></button>
                <% 
                    }
                %>
                    </div>
                </td>
                <td align="right">
                    <table>
                        <tr>
                            <td>
                                <input id="btNext" type="hidden" value="<%=nextId%>" />
                                <ul class="button-group">
	                                <li title="Trước"><button class="button" onclick="javascript:fnLoadImportDetail('<%=previousId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                    <li title="Sau"><button class="button" onclick="javascript:fnLoadImportDetail('<%=nextId%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                </ul>
                            </td>
                            <td>
                                <%Html.RenderPartial("ImportSV");%>
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
        if (status == NEXMI.NMConstant.IMStatuses.Draft || status == NEXMI.NMConstant.IMStatuses.Ready)
        {    
    %>
            <button class="color red button" onclick="javascript:fnReceive('<%=id%>', '<%=NEXMI.NMConstant.IMStatuses.Imported%>')"><%= NEXMI.NMCommon.GetInterface("CONFIRM", langId) %></button>
    <% 
            if (importType != NEXMI.NMConstant.ImportType.Transfer)
            {
    %>
                <button class="button" onclick="javascript:fnCancelImport('<%=id%>', '<%=NEXMI.NMConstant.IMStatuses.Canceled%>')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %></button>
    <% 
            }
        }
        else if (status == NEXMI.NMConstant.IMStatuses.Imported)
        {
            if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionImport))
            {
    %>
                <button onclick="javascript:fnPopupImportDialog('<%=id%>', '')" class="button color red"><%= NEXMI.NMCommon.GetInterface("RECONCILE", langId)%></button>
    <%      }
            if (NEXMI.NMCommon.GetSettingValue("RETURN_PRODUCT") == 1)
            {
    %>        
                <button class="button" onclick="javascript:fnImportReturn('<%=id%>')"><%= NEXMI.NMCommon.GetInterface("RETURN_PRODUCTS", langId)%></button>
    <%      }
        }
    %>
        </div>     
        <div class="divStatusBar">
            <%
                Html.RenderAction("StatusBar", "UserControl", new { objectName = "Imports", current = status });
            %>
        </div>
    </div>
    <div class='divContentDetails'>
        <form id="formImport" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId"><%= NEXMI.NMCommon.GetInterface("IMPORT_ORDER", langId) %> <label id="txtId"><%=id%></label></label>
                        <table class="frmInput">
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("SUPPLIER", langId) %></td>
                                <td>[<%=WSI.Supplier.Code %>] <%=ViewData["SupplierName"]%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                                <td><%=ViewData["Reference"]%></td>
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("IMPORT_DATE", langId) %></td>
                                <td><%=ViewData["importDate"]%></td>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TYPE", langId) %></td>
                                <td valign="top"><%= ViewData["ImportTypeId"]%></td>                                
                            </tr>
                            <tr>
                                <td class="lbright"><%= NEXMI.NMCommon.GetInterface("STORE", langId) %></td>
                                <td valign="top">[<%=WSI.Stock.Id %>] <%=ViewData["slStocks"]%></td>
                                <td></td>
                                <td></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div id="tabsImport">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("WH_RECORD", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" id="tbImportDetail" class="tbDetails">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("REQUIRE_QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("GOOD_QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("BAD_QUANTITY", langId) %></th>
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
                                    <tbody id="tBody">
                                <% 
                                    //IEnumerable<NEXMI.ImportDetails> Details = (IEnumerable<NEXMI.ImportDetails>)ViewData["Details"];
                                    int i = 1;
                                    foreach (NEXMI.ImportDetails Item in WSI.Details.OrderBy(d => d.OrdinalNumber))
                                    {
                                %>
                                        <tr>
                                            <td><%=i++%></td>
                                            <td>[<%= NEXMI.NMCommon.GetProductCode(Item.ProductId) %>] <%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetUnitNameById(Item.UnitId)%></td>
                                            <td><%=Item.RequiredQuantity.ToString("N3")%></td>
                                            <td><%=Item.GoodQuantity.ToString("N3")%></td>
                                            <td><%=Item.BadQuantity.ToString("N3")%></td>
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
                                        <td><%=ViewData["description"]%></td>
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
                            <td><%=NEXMI.NMCommon.GetParameterName(ViewData["DeliveryMethod"].ToString())%></td>
                    <%
                        if (!String.IsNullOrEmpty(ViewData["ExportStock"].ToString()))
                              
                        { 
                    %>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("EXPORT_STOCK", langId) %></td>
                            <td><%=ViewData["ExportStock"] %></td>
                    <%
                        } else
                        {
                    %>
                            <td></td>
                            <td></td>
                    <%
                        }
                    %>
                        </tr>
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTER", langId) %></td>
                            <td valign="top"><%=ViewData["CarrierName"]%></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TRANSPORTATION", langId) %></td>
                            <td valign="top"><%=ViewData["Transport"]%></td>
                        </tr>
                        <%  ViewData["ModifiedDate"] = WSI.Import.ModifiedDate;
                        ViewData["ModifiedBy"] = WSI.Import.ModifiedBy;
                        ViewData["CreatedDate"] = WSI.Import.CreatedDate;
                        ViewData["CreatedBy"] = WSI.Import.CreatedBy ;
                        Html.RenderPartial("LastModified"); %>
                    </table>                
                </div>
                <div id='Accounting'></div>
            </div>
        </form>  

        <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = ViewData["SendTo"] });%>
    </div>
    </div>
</div>