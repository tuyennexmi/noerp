<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div id="divLogs">
<div class="divContent">
    <table style="width: 50%; margin:16px 0 0 16px;" cellpadding="6">
        <%--<tr style="background-color:Silver">
            <th>Tin nhắn ngắn</th>
            <th>Nhật ký hệ thống</th>
        </tr>--%>
        <tr>
        <%
            List<NEXMI.NMMessagesWSI> WSIs = (List<NEXMI.NMMessagesWSI>)ViewData["WSIs"];
             %>
            <td style="width: 50%;" valign="top">
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
                            <%  Html.RenderAction("RenderMessages", "Messages", new { msg = Item });
                            %>                            
                        </td>
                    </tr>
                    <%
                                }
                            }
                        }
                    %>
                </table>
            </td>
            <%--<td style="width: 50%;" valign="top">
                <table style="width: 100%;">                    
                    <%  if (WSIs != null)
                        {
                            foreach (NEXMI.NMMessagesWSI Item in WSIs.OrderByDescending(m => m.Message.CreatedDate))
                            {
                                if (Item.Message.TypeId == NEXMI.NMConstant.MessageTypes.Note)
                                {
                    %>
                    <tr>
                        <td>
                            <%  Html.RenderAction("RenderMessages", "Messages", new { msg = Item });
                             %>
                        </td>
                    </tr>
                    <%
                                }
                            }
                        }
                    %>
                </table>
            </td>--%>
        </tr>
    </table>
</div>
<//div>