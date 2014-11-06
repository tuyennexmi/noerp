<%-- ManufactureOrderDetail.ascx

* http://nexmi.com
* NoErp Project - Nexmi Open ERP
* Copyright (C) 2012, Nguyễn Quang Tuyến (tuyen.nq@nexmi.com), AUTHOR.txt (http://nexmi.com/about)
* This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; 
either version 2 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
You should have received a copy of the GNU General Public License along with this library; if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*
--%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript" src="<%=Url.Content("~") %>Scripts/forApp/Manufacturing.js"></script>

<script type="text/javascript">
    $(document).ready(function () {
        $('.tabs').jqxTabs({ theme: theme, keyboardNavigation: false });
        $('#txtTotal').autoNumeric('init', { vMax: '100000000000000' });

    });
</script>
<%        NEXMI.NMManufactureOrdersWSI wsi = (NEXMI.NMManufactureOrdersWSI)ViewData["MO"];
%>
<div class="divActions">
    <table style="width: 100%;">
        <tr><td></td></tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.ManufactureOrders;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnShowMOForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnShowMOForm('<%=wsi.ManufactureOrder.Id %>')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                <%  }
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnDeleteMO('<%=wsi.ManufactureOrder.Id %>')" class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                <%  } %>
            </td>
            <td></td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">            
            <table>
                <tr>
                    
                </tr>
            </table>                        
        </div>
        <div class="divStatusBar">
            <input type="hidden" id="txtStatus" value="<%=wsi.ManufactureOrder.Status%>" />
            <%Html.RenderAction("StatusBar", "UserControl", new { objectName = "ManufactureOrder", current = NEXMI.NMConstant.MOStatuses.Draft });%>
        </div>
    </div>
    
    <div class='divContentDetails'>
    <form id="formSalesOrder" class="form-to-validate" action="" style="margin: 10px;">
        <table class="frmInput">
            <tr>
                <td>
                    <label class="lbId">Lệnh sản xuất <label id="txtId"><%= wsi.ManufactureOrder.Id %></label></label>
                    <table class="frmInput">
                        <tr>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></td>
                            <td><%= wsi.ManufactureOrder.ProductId %></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("REF_DOC", langId) %></td>
                            <td><%= wsi.ManufactureOrder.SaleReference %></td>
                        </tr>
                        <tr>
                            <td class="lbright">Sản lượng</td>
                            <td><%= wsi.ManufactureOrder.Quantity %></td>
                            <td class="lbright">Chịu trách nhiệm</td>                            
                            <td><%= wsi.ManagedBy.CompanyNameInVietnamese %></td>
                        </tr>
                        <tr>
                            <td class="lbright">Ngày dự kiến</td>
                            <td><%=wsi.ManufactureOrder.StartDate.ToString("yyyy-MM-dd") %></td>
                            <td class="lbright"><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></td>
                            <td><%=wsi.ManufactureOrder.Descriptions %></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
    <div class="tabs" id='detailTabs'>
        <ul>
            <li style="margin-left: 25px;">Vật tư</li>
            <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
        </ul>
        <div>
            <table style="width: 100%" class="tbTemplate">
                <tr>
                    <td valign="top">
                        <table style="width: 60%" class="tbDetails">
                            <thead>
                                <tr>
                                    <th><%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %></th>
                                    <th><%= NEXMI.NMCommon.GetInterface("QUANTITY", langId) %></th>
                                </tr>
                            </thead>
                            <tbody id="tbodyDetails">
                                <% 
                                    List<NEXMI.MOMaterialDetails> Md = wsi.ManufactureOrder.MaterialDetails.ToList();                                    
                                    foreach (NEXMI.MOMaterialDetails Item in Md)
                                    {
                                %>
                                <tr id="row<%=Item.ProductId%>">
                                    <td><%=NEXMI.NMCommon.GetName(Item.ProductId, langId)%></td>
                                    <td><%=(Item.Quantity).ToString("N3") %></td>
                                </tr>
                                <% 
                                    }    
                                %>
                            </tbody>
                        </table>
            </td></tr></table>
        </div>
        <div>
            
        </div>
    </div>

    <%Html.RenderAction("Logs", "Messages", new { ownerId = wsi.ManufactureOrder.Id, sendTo = wsi.ManufactureOrder.CreatedBy });%>
    </div>
</div>