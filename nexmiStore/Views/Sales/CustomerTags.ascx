<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<input type="hidden" id="ViewType"  value="<%=ViewData["ViewType"]%>" />
<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <div>
                <ul class="ulList">
                <%
                    string fileName = "";
                    string address ="";
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
                    
                    foreach(NEXMI.NMCustomersWSI Item in Customers)
                    {
                        if (!string.IsNullOrEmpty(Item.Customer.Avatar))
                        {
                            fileName = Item.Customer.Avatar;
                        }
                        else
                        {
                            if (Item.Customer.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                            {
                                fileName = "company.png";
                            }
                            else
                            {
                                fileName = "personal.png";   
                            }
                        }
                        if (Item.Customer.Address != null)
                        {
                            address = Item.Customer.Address;
                            //address = (Item.Address.Length > 28) ? Item.Address.Substring(0, 28) : Item.Address;
                        }
                %>
                    <li class="allCorners">
                            <table style="width: 100%;">
                                <tr>
                                    <td valign=top style="width: 80px">
                                        <img alt="" class="avatar" src="<%=Url.Content("~")%>Content/avatars/<%=fileName%>" onclick="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>', '<%=ViewData["GroupId"] %>')" />
                                        <div style="padding:6px;">
                                            <a href="javascript:fnShowCustomerForm('<%=Item.Customer.CustomerId%>')">
                                            <img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title='<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>' src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                            <a href="javascript:fnSendEmail('<%=Item.Customer.CustomerId%>')">
                                            <img alt="Gửi email" title='Gửi email' src="<%=Url.Content("~")%>Content/UI_Images/sendmail.jpg" style="height: 16px;" /></a>
                                        </div>
                                    </td>
                                    <td valign="top">   
                                        <a href="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>', '<%=ViewData["GroupId"] %>')"><%=Item.Customer.CompanyNameInVietnamese%></a><br />
                                        <%--<a href="javascript:fnLoadCustomerDetail('<%=Item.Customer.CustomerId%>', '<%=ViewData["GroupId"] %>')"><%=Item.CompanyNameInVietnamese.Length > 28? Item.CompanyNameInVietnamese.Substring(0, 10) + "..." + Item.CompanyNameInVietnamese.Substring(Item.CompanyNameInVietnamese.Length-18, 18) : Item.CompanyNameInVietnamese %></a><br />--%>
                                        <div>
                                            [ <%=Item.Customer.Code %> ]<br />
                                            <%=address %>   <br />
                                            <%--<%=Item.EmailAddress%> <br />--%>
                                            <%=Item.Customer.Telephone %> <br />
                                            <%=Item.Customer.Cellphone %> <br />                                            
                                        </div>
                                    </td>
                                </tr>
                                
                            </table>
                        </li>
                <%
                    } 
                %>
                </ul>
            </div>
        </td>
    </tr>
</table>

<script type="text/javascript">
    $(function () {
        $('#pagination-Customer').jqPagination('option', { 'max_page': '<%=totalPage%>', trigger: false });
    });

    function fnSendEmail(customerId) {
        //openWindow('Gửi email', 'UserControl/SendEmailForm', { id: customerId }, 900, 500);
        openWindow("Soạn email", "Messages/ComposeEmail", { id: customerId }, 880, 550);
    }    
    
</script>