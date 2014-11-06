<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%
    NEXMI.NMCustomersWSI WSI = (NEXMI.NMCustomersWSI)ViewData["Customer"];
    string customerId = ViewData["CustomerId"].ToString();
    string previousId = NEXMI.NMCommon.PreviousId(customerId, "CustomerId", "Customers", "where GroupId = '" + ViewData["GroupId"] + "'");
    string nextId = NEXMI.NMCommon.NextId(customerId, "CustomerId", "Customers", "where GroupId = '" + ViewData["GroupId"] + "'");
    string functionCustomer = "", transAction = "", idParam = "", orderPath = "", typeId = "";

    if (WSI.Customer.GroupId == NEXMI.NMConstant.CustomerGroups.Customer)
    {
        functionCustomer = NEXMI.NMConstant.Functions.Customer;
        transAction = "Accounting/ARDetails";
        idParam = "customerId";
        orderPath = "Sales/OrdersHictory";
        typeId = NEXMI.NMConstant.SOType.SalesOrder;
    }
    else if (WSI.Customer.GroupId == NEXMI.NMConstant.CustomerGroups.Manufacturer)
    {
        functionCustomer = NEXMI.NMConstant.Functions.Manufacturers;
        transAction = "Accounting/ARDetails";
        idParam = "customerId";
        orderPath = "Sales/OrdersHictory";
        typeId = NEXMI.NMConstant.SOType.SalesOrder;
    }
    else if (WSI.Customer.GroupId == NEXMI.NMConstant.CustomerGroups.Supplier)
    {
        functionCustomer = NEXMI.NMConstant.Functions.Suppilers;
        transAction = "Accounting/APDetails";
        idParam = "supplierId";
        orderPath = "Purchase/PurchaseOrdersList";
        typeId = NEXMI.NMConstant.POType.PurchaseOrder;
    }
    else
    {
        functionCustomer = NEXMI.NMConstant.Functions.Users;
        transAction = "Accounting/ARDetails";
        idParam = "customerId";
        orderPath = "Sales/OrdersHictory";
        typeId = NEXMI.NMConstant.SOType.SalesOrder;
    }
    
    bool isManageProject = NEXMI.NMCommon.GetSetting("MANAGE_PROJECT");
%>
<script type="text/javascript">
    $(function () {
        $("#tabsCustomerDetail").jqxTabs();

        //
        $('#tabsCustomerDetail').bind('selected', function (event) {
            var item = event.args.item;
            switch (item) {
                case 5:
                    LoadContentDynamic('Orders', '<%=orderPath %>', { partnerId: '<%=customerId %>', typeId: '<%=typeId%>' });
                    break;
                case 6:
                    LoadContentDynamic('Transactions', '<%=transAction %>', { '<%=idParam %>': '<%=customerId %>' });
                    break;
                case 7:
                    LoadContentDynamic('Projects', 'Projects/ProjectList', { customer: '<%=customerId %>' });
                    break;
            }
        });
    });
</script>
<input type="hidden" id="CustomerGroupId"  value="<%=WSI.Customer.GroupId%>" />
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
                <%
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionCustomer))
                    {
                %>                    
                    <button onclick='javascript:fnShowCustomerForm("", "<%=ViewData["GroupId"]%>")' class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <%
                    }    
                    //Kiểm tra quyền Update
                    if (GetPermissions.GetUpdate((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionCustomer))
                    {
                %>
                <button onclick="javascript:fnShowCustomerForm('<%=customerId%>', '<%=ViewData["GroupId"]%>')" class="button"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></button>
                <%
                    }    
                    //Kiểm tra quyền Delete
                    if (GetPermissions.GetDelete((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionCustomer))
                    {
                %>
                <button onclick='javascript:fnDeleteCustomer("<%=customerId%>", "<%=ViewData["GroupId"]%>")' class="button"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
                <% 
                    }
                    else
                    {
                %>
                <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></button>
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
	                            <li title="Trước"><button class="button" onclick="javascript:fnLoadCustomerDetail('<%=previousId%>', '<%=ViewData["GroupId"]%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                <li title="Sau"><button class="button" onclick="javascript:fnLoadCustomerDetail('<%=nextId%>', '<%=ViewData["GroupId"]%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                            </ul>
                        </td>
                        <td>   
                            <%Html.RenderPartial("CustomerSV"); %>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</div>

<div class="divContent">
    <div class="divStatus">
        <div class="divStatusBar">  
        <%
            Html.RenderAction("StatusBar", "UserControl", new { objectName = "Customers", current = "CUS03" });
        %>
        </div>
    </div>
    <div class='divContentDetails'>
    <table style="width: 100%" class="frmInput">
        <tr>
            <td>
                <table style="width: 100%" class="frmInput">
                    <tr>
                        <td style="width: 120px;" align="center"><img style="width: 100px; height: 100px;" alt="" src="<%=ViewData["avatar"]%>" /></td>
                        <td valign="top"><h1><%=ViewData["CompanyNameInVietnamese"]%> / <%=ViewData["CustomerCode"]%> <br />(<%=ViewData["CustomerId"]%>)</h1></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table style="width: 100%" class="frmInput">
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %></td>
                        <td style="width: 20%;"><label><%=ViewData["TaxCode"]%></label></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></td>
                        <td style="width: 20%;"><label><%=ViewData["Telephone"]%></label></td>
                        <td class="lbright"><%=ViewData["DOBName"]%></td>
                        <td style="width: 20%;"><label><%=ViewData["DateOfBirth"]%></label></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></td>
                        <td><label><%=ViewData["Address"]%></label></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MOBILEPHONE", langId) %></td>
                        <td><label><%=ViewData["Cellphone"]%></label></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("CATEGORIES", langId) %></td>
                        <td><label><%=ViewData["KOI"]%></label></td>
                    </tr>
                    <tr>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("AREA", langId) %></td>
                        <td><%=ViewData["Area"]%></td>
                        <td class="lbright">Fax</td>
                        <td><label><%=ViewData["FaxNumber"]%></label></td>
                        <%--Kiểm tra nếu là doanh nghiệp mới hiển thị--%>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("KIND_OF_ENTERPRISE", langId) %></td>
                        <td><label><%=ViewData["KOE"]%></label></td>
                    </tr>
                    <tr>
                        <td class="lbright">Website</td>
                        <td><a target="_blank" href="http://<%=ViewData["Website"]%>"><%=ViewData["Website"]%></a></td>
                        <td class="lbright">Email</td>
                        <td><label><%=ViewData["EmailAddress"]%></label></td>
                        <td class="lbright"><%= NEXMI.NMCommon.GetInterface("BONUS_POINT", langId) %></td>
                        <td><%=ViewData["BonusPoints"]%></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

    <div id="tabsCustomerDetail">
        <ul>                
            <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("CONTACT_PERSON", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("DOCUMENTS", langId) %></li>
            <%--<li><%= NEXMI.NMCommon.GetInterface("MAP", langId)%></li>--%>
            <li><%= NEXMI.NMCommon.GetInterface("ORDER", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("TRANSACTION_HISTORY", langId) %></li>
    <%
        if (isManageProject)
        { %>
            <li>Dự án</li>
    <%  } %>
            <%--<li>Khoản thu</li>
            <li>Khoản chi</li>--%>
        </ul>
        <div style="padding: 10px;">                
            <%Html.RenderAction("CustomerTag", "Customers", new { cusList = (List<NEXMI.Customers>)ViewData["ContactPersons"] });%>
        </div>
        <div style="padding: 10px;">
            <%=ViewData["Description"]%>
        </div>
        <div style="padding: 10px;">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("ACCOUNT_MANAGER", langId) %></td>
                    <td style="min-width: 250px;"><%=ViewData["ManagedBy"]%></td>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("PAYMENT_DATE", langId) %></td>
                    <td style="min-width: 250px;"><%=ViewData["DueDate"]%></td>
                </tr>
                <tr>
                    <td class="lbright"><%= NEXMI.NMCommon.GetInterface("MAX_DEBT", langId) %></td>
                    <td><%=ViewData["MaxDebitAmount"]%></td>
                    <td class="lbright"></td>
                    <td></td>
                </tr>
            </table>
        </div>
        <div style="padding: 10px;">
            <%Html.RenderAction("DynamicUploader", "Files", new { elementId = "img_uploader", owner = customerId, type = NEXMI.NMConstant.FileTypes.Images });%>                
        </div>
        <div style="padding: 10px;">
            <%Html.RenderAction("DynamicUploader", "Files", new { elementId = "doc_uploader", owner = customerId, type = NEXMI.NMConstant.FileTypes.Docs });%>
        </div>
        <%--<div style="padding: 10px;">             
            <% Html.RenderAction("CustomerMap", "Customers", new { longitude = ViewData["Longitude"], latitude = ViewData["Latitude"] });%>
        </div>--%>
        <div id ="Orders" style="padding: 10px;">        </div>
        <div id ="Transactions" style="padding: 10px;">        </div>

    <%if (isManageProject)
    { %>
        <div id='Projects' style="padding: 10px;">        </div>
    <%} %>
        <%--<div style="padding: 10px;"></div>
        <div style="padding: 10px;"></div>--%>
    </div>
    
    <%Html.RenderAction("Logs", "Messages", new { ownerId = customerId, sendTo = ViewData["SendTo"], sendSMSTo = WSI.Customer.CustomerId });%>
    </div>
</div>