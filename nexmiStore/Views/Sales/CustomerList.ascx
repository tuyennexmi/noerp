<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMCustomersWSI> Customers = (List<NEXMI.NMCustomersWSI>)ViewData["Customers"];                
    int totalPage = 1;
    try
    {
        int totalRows = int.Parse(ViewData["TotalRows"].ToString());
        if (totalRows % NEXMI.NMCommon.PageSize() == 0)
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize();
        }
        else
        {
            totalPage = totalRows / NEXMI.NMCommon.PageSize() + 1;
        }
    }
    catch { }
%>
<input type='hidden' id='txtGroupId' value='<%=ViewData["GroupId"].ToString() %>' />
<input type="hidden" id="ViewType"  value="<%=ViewData["ViewType"]%>" />

<table id="tbReceipts" class="tbDetails">
    <thead>
        <tr>            
            <th>#</th>
            <th><%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("TAX_CODE", langId) %></th>
            <th><%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %></th>            
            <th><%= NEXMI.NMCommon.GetInterface("TELEPHONE", langId) %></th>
            <th>Fax</th>
            <th>Email</th>
            <th style="width:6%"></th>
        </tr>
    </thead>
    <tbody>
<%  
    int count = 1;
    foreach (NEXMI.NMCustomersWSI Item in Customers)
    {
%>
        <tr ondblclick="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>')" id="<%=Item.Customer.CustomerId%>">            
            <td ><%=count++ %></td>
            <td >[<%=Item.Customer.Code%>] <%=Item.Customer.CompanyNameInVietnamese%></td>
            <td ><%=Item.Customer.TaxCode%></td>                
            <td ><%=Item.Customer.Address%></td>
            <td ><%=Item.Customer.Telephone%></td>
            <td ><%=Item.Customer.FaxNumber%></td>
            <td ><%=Item.Customer.EmailAddress %></td>
            <td >
                <a href="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>                    
                <a href="javascript:fnShowCustomerForm('<%=Item.Customer.CustomerId%>')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>                    
            </td>
        </tr>
<%
    }
%>
    </tbody>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Customer').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
        
        var t;
        $("#txtKeywordCustomerList").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                var keyword = $("#txtKeywordCustomerList").val();
                var area = '';
                try {
                    area = $('#cbbAreas').jqxComboBox('getSelectedItem').value;
                } catch (err) { }
                fnLoadCustomerList('', keyword, area, '<%=ViewData["GroupId"] %>');
            }, 1000);
        });
        $("#txtKeywordCustomerList").focus();

    });
        
</script>
