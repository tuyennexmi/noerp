<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Requirements.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {        
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
    });
        
</script>
<% 
    NEXMI.NMRequirementsWSI WSI = (NEXMI.NMRequirementsWSI)ViewData["WSI"];
    string id = WSI.Requirement.Id;
    string func = NEXMI.NMConstant.Functions.Requirement;
    string current = WSI.Requirement.Status;
%>
<input type="hidden" id="txtStatus" value="<%=WSI.Requirement.Status%>" />
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
                        <button onclick="javascript:fnShowRequirementForm('', '')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%  }
                    if (current == NEXMI.NMConstant.RequirementStatuses.Draft|| current == NEXMI.NMConstant.ReceiptStatuses.Cancelled)
                    {
                        if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {%>
                            <button onclick="javascript:fnShowRequirementForm('<%=id%>', '')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                        <%}
                        if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        { %>
                            <button onclick="javascript:fnDeleteRequirement('<%=id%>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                        <%} %>
                <%
                    }

                    if (GetPermissions.GetDuplicate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                    {
                    %>
                        <button onclick="javascript:fnShowRequirementForm('<%=id%>', 'Copy')" class="button"><%= NEXMI.NMCommon.GetInterface("COPY", langId) %></button>
                    <%}
                        if (GetPermissions.GetPPrint((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        { %>
                       
                    <%  } %>
                    </div>
                </td>
                <td align="right">
                    <%//Html.RenderPartial("SalesOrderSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <div class="divButtons NMButtons">
                <%  if ((current == NEXMI.NMConstant.RequirementStatuses.Draft) | (current == NEXMI.NMConstant.RequirementStatuses.Sent))
                    {   
                        %>
                        <button title='Gửi đề xuất này qua email!' class="color red button" onclick="javascript:fnSendRQ('<%=id%>')">Gửi đề xuất </button>
                        <button class="button" onclick="javascript:fnConfirmRQ('<%=id%>', '<%=NEXMI.NMConstant.RequirementStatuses.Cancelled%>', 'hủy')"><%= NEXMI.NMCommon.GetInterface("CANCEL", langId) %> đề xuất</button>
                    
                    <% if (GetPermissions.GetApproval((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], func))
                        {
                         %>                        
                        <button class="color red button" onclick="javascript:fnConfirmRQ('<%=id%>', '<%=NEXMI.NMConstant.RequirementStatuses.Approved%>', 'duyệt')"><%= NEXMI.NMCommon.GetInterface("APPROVE", langId) %></button>
                        <button class="color red button" onclick="javascript:fnConfirmRQ('<%=id%>', '<%=NEXMI.NMConstant.RequirementStatuses.NotApproved%>', 'không duyệt')"><%= NEXMI.NMCommon.GetInterface("NOT_APPROVE", langId) %></button>                        
                        <%
                        }
                    }
                    else if ((current == NEXMI.NMConstant.RequirementStatuses.Cancelled)|(current == NEXMI.NMConstant.RequirementStatuses.NotApproved))
                    {
                %>
                        <button class="color red button" onclick="javascript:fnConfirmRQ('<%=id%>', '<%=NEXMI.NMConstant.RequirementStatuses.Draft%>', 'phục hồi')">Phục hồi</button>
                <%
                    }
                    %>
                </div>
            </div>     
            <div class="divStatusBar">
                <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "Requirements", current = WSI.Requirement.Status });%>
            </div>
        </div>
        <div class='divContentDetails'>
        <form id="formRequirement" class="form-to-validate" action="" style="margin: 10px;">
            <table class="frmInput">
                <tr>
                    <td>
                        <label class="lbId">Đề xuất vật tư <label id="txtId"><%=id%></label></label>                        
                        <table class="frmInput">
                            <tr>
                                <td class="lbright" >Lý do đề xuất</td>
                                <td><%= GlobalValues.GetType(WSI.Requirement.RequirementTypeId) %></td>
                                <td class="lbright">Ngày đề xuất</td>
                                <td><%=WSI.Requirement.RequireDate.ToString("dd-MM-yyyy") %> </td>                                
                            </tr>
                            <tr>
                                <td class="lbright" ><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></td>
                                <%if (WSI.Customer != null)
                                  { %>
                                    <td><%= WSI.Customer.CompanyNameInVietnamese%></td>
                                <%}
                                  else
                                  { %>
                                    <td></td>
                                <%} %>
                                <td class="lbright">Theo hợp đồng/đơn hàng số</td>
                                <td><%=WSI.Requirement.OrderId %> </td>                                
                            </tr>
                            <tr>
                                <td class="lbright">Người đề xuất</td>
                                <td><%= WSI.RequiredBy.CompanyNameInVietnamese %></td>
                                <td class="lbright">Thời gian đáp ứng</td>
                                <td><%=WSI.Requirement.ResponseDays %> (ngày)</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div class="tabs">
                <ul>
                    <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("DETAIL", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
                </ul>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td>
                                <table style="width: 100%" class="tbDetails" id='tbRequirementDetail'>
                                    <thead>
                                        <tr>
                                            <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("UNIT_PRICE", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("LINE_AMOUNT", langId) %></th>
                                            <th><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></th>                                            
                                        </tr>
                                    </thead>
                                    <tbody id="tbodyDetails">
                                        <% 
                                            List<NEXMI.RequirementDetails> Details = (List<NEXMI.RequirementDetails>)Session["Details"];
                                            if (Details != null)
                                            foreach (NEXMI.RequirementDetails Item in Details)
                                            {
                                        %>
                                        <tr id="<%=Item.ProductId%>">
                                            <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                            <td><%=NEXMI.NMCommon.GetProductUnitName(Item.ProductId)%></td>
                                            <td><%=Item.Quantity.ToString("N3")%></td>
                                            <td><%=Item.Price.ToString("N3")%></td>
                                            <td><%=Item.Amount.ToString("N3")%></td>
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
                                        <td valign="top" width='70%'>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td><b><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>: </b></td>                                                    
                                                </tr>
                                                <tr>
                                                    <td><%=WSI.Requirement.Description%></td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td valign="top">
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="right"><b>Tổng giá trị: </b></td>
                                                    <td align="right"><label id="lbTotalAmount"><%=WSI.Requirement.Amount.ToString("N3")%></label></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"></td>
                            <td></td>
                            <td class="lbright"></td>
                            <td></td>
                        </tr>
                    </table>
                </div>
            </div>
        </form>
    
    
        <div class="logs">
            <%Html.RenderAction("Logs", "Messages", new { ownerId = id, sendTo = WSI.Requirement.CreatedBy });%>
        </div>
        </div>
    </div>
    
</div>