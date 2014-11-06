<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<table style="width: 100%" class="tbTemplate">
    <tr>
        <td>
            <div>
                <ul class="ulList">
                <%
                    string fileName = "";
                    string address ="";
                    List<NEXMI.Customers> Customers = (List<NEXMI.Customers>)ViewData["cusList"];
                    foreach(NEXMI.Customers Item in Customers)
                    {
                        if (!string.IsNullOrEmpty(Item.Avatar))
                        {
                            fileName = Item.Avatar;
                        }
                        else
                        {
                            if (Item.CustomerTypeId == NEXMI.NMConstant.CustomerTypes.Enterprise)
                            {
                                fileName = "company.png";
                            }
                            else
                            {
                                fileName = "personal.png";   
                            }
                        }
                        if (Item.Address != null)
                        {
                            address = Item.Address;
                            //address = (Item.Address.Length > 28) ? Item.Address.Substring(0, 28) : Item.Address;
                        }
                %>
                    <li class="allCorners">
                            <table style="width: 100%;">
                                <tr>
                                    <td valign=top style="width: 80px">
                                        <img alt="" class="avatar" src="<%=Url.Content("~")%>Content/avatars/<%=fileName%>" onclick="javascript:fnLoadCustomerDetail('<%=Item.CustomerId%>', '<%=ViewData["GroupId"] %>')" />
                                    </td>
                                    <td valign="top">   
                                        <a href="javascript:fnLoadCustomerDetail('<%=Item.CustomerId%>', '<%=ViewData["GroupId"] %>')"><%=Item.CompanyNameInVietnamese%></a><br />
                                        <%--<a href="javascript:fnLoadCustomerDetail('<%=Item.CustomerId%>', '<%=ViewData["GroupId"] %>')"><%=Item.CompanyNameInVietnamese.Length > 28? Item.CompanyNameInVietnamese.Substring(0, 10) + "..." + Item.CompanyNameInVietnamese.Substring(Item.CompanyNameInVietnamese.Length-18, 18) : Item.CompanyNameInVietnamese %></a><br />--%>
                                        <div>
                                            <%=Item.Code %> <br />
                                            <%=address %>   <br />
                                            <%--<%=Item.EmailAddress%> <br />--%>
                                            <%=Item.Telephone %> <br />
                                            <%=Item.Cellphone %> <br />                                            
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="right">
                                        <div style="padding:6px;">
                                            <a href="javascript:fnShowCustomerForm('<%=Item.CustomerId%>', '')">
                                            <img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title='<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>' src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /></a>
                                            <a href="javascript:fnSendEmail('<%=Item.CustomerId%>')">
                                            <img alt="Gửi email" title='Gửi email' src="<%=Url.Content("~")%>Content/UI_Images/sendmail.jpg" style="height: 16px;" /></a>
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

    function fnSendEmail(customerId) {
        //openWindow('Gửi email', 'UserControl/SendEmailForm', { id: customerId }, 900, 500);
        openWindow("Soạn email", "Messages/ComposeEmail", { id: customerId }, 880, 550);
    }    
    
</script>