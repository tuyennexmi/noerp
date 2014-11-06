<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

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
    }
    
    bool isManageProject = NEXMI.NMCommon.GetSetting("MANAGE_PROJECT");
%>

 <script type="text/javascript">
     $(function () {
         $("#tabsCustomerDetailPopup").jqxTabs();

         //
         $('#tabsCustomerDetailPopup').bind('selected', function (event) {
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

<div style='width:100%; overflow:auto;height: 100%;'>
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

    <div id="tabsCustomerDetailPopup">
        <ul>                
            <li style="margin-left: 25px;"><%= NEXMI.NMCommon.GetInterface("CONTACT_PERSON", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("NOTE", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("OTHER_INFO", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("PICTURE", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("DOCUMENTS", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("ORDER", langId) %></li>
            <li><%= NEXMI.NMCommon.GetInterface("TRANSACTION_HISTORY", langId) %></li>
        <%if (isManageProject)
          { %>
            <li>Dự án</li>
        <%} %>
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
        
        <div id ="Orders" style="padding: 10px;">        </div>
        <div id ="Transactions" style="padding: 10px;">        </div>

    <%if (isManageProject)
    { %>
        <div id='Projects' style="padding: 10px;">        </div>
    <%} %>
        
    </div>
    
    <%Html.RenderAction("Logs", "Messages", new { ownerId = customerId, sendTo = ViewData["SendTo"] });%>
    </div>