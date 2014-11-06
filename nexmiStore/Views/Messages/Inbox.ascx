<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    function fnComposeEmail(id) {
        openWindow("Soạn email", "Messages/ComposeEmail", { id: id }, 880, 550);
    }
</script>
<table>
    <tr>
        <td>
        <div class="divActions">
            <table style="width: 100%;">
                <tr>
                    <td>
            
                    </td>
                    <td align="right">&nbsp;
                        <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width:250px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <button onclick='javascript:fnComposeEmail("composemsg")' class="button">Gửi email</button>
                        <%--<button onclick='javascript:fnShowCustomerForm()' class="button"><%= NEXMI.NMCommon.GetInterface("SEND_MESSAGE", langId) %></button>--%>                
                    </td>
                    <td align="right">
                        <table>
                            <tr>
                                <%--<td>
                                    <input id="btNext" type="hidden" value="<%=nextId%>" />
                                    <ul class="button-group">
	                                    <li title="Trước"><button class="button" onclick="javascript:fnLoadCustomerDetail('<%=previousId%>', '<%=ViewData["GroupId"]%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/previous.png" class="btSwitchView" />&nbsp;</button></li>
                                        <li title="Sau"><button class="button" onclick="javascript:fnLoadCustomerDetail('<%=nextId%>', '<%=ViewData["GroupId"]%>')"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/next.png" class="btSwitchView" />&nbsp;</button></li>
                                    </ul>
                                </td>--%>
                                <td>   
                                    <%--<%Html.RenderPartial("CustomerSV"); %>--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        </td>
    </tr>

    <tr>
        <td>
            <div class="divContent">
                <table style="width: 100%; margin:16px 0 0 16px;" cellpadding="6">
                    <tr>
            <%
                List<NEXMI.NMMessagesWSI> WSIs = (List<NEXMI.NMMessagesWSI>)ViewData["WSIs"];
                 %>
                        <td style="width: 68%;" valign="top">
                            <table style="width: 100%;">                    
                                <%  if (WSIs != null)
                                    {
                                        foreach (NEXMI.NMMessagesWSI Item in WSIs.OrderByDescending(m => m.Message.CreatedDate))
                                        {
                                            if (Item.Message.TypeId != NEXMI.NMConstant.MessageTypes.SysLog)
                                            {
                                %>
                                                <tr>
                                                    <td>
                                                        <%Html.RenderAction("RenderMessages", "Messages", new { msg = Item });%>
                                                    </td>
                                                </tr>
                                <%
                                            }
                                        }
                                    }
                                %>
                            </table>
                        </td>
                        <td>            
                            
                        </td>
                    </tr>
                </table>
            </div>
        </td>
        
    </tr>
</table>

