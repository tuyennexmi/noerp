<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<% string langId = Session["Lang"].ToString(); %>


<%
    List<NEXMI.NMMessagesWSI> WSIs = (List<NEXMI.NMMessagesWSI>)ViewData["WSIs"];
    //String Owner = "";
    //if (WSIs.Count > 0)
    //{
    //    Owner = WSIs[0].Message.Owner;
    //}
     %>
<div id="divLogs">
    <input type="hidden" id ="Owner" value="<%=ViewData["OwnerId"]%>" />
    <input type="hidden" id ="SendSMSTo" value="<%=ViewData["SendSMSTo"]%>" />
    <input type="hidden" id ="SendMsgTo" value="<%=ViewData["SendMSGTo"]%>" />

    <table style="width: 100%;" cellpadding="15">
        <tr>
            <td style="width: 70%;" valign="top">
                <table style="width: 100%;">
                    <tr>
                        <td id="commentContainer">
                            <%Html.RenderAction("WriteNote", "Messages"); %>
                        </td>
                    </tr>
        <%  
            if (WSIs != null)
            {
                foreach (NEXMI.NMMessagesWSI Item in WSIs.OrderByDescending(m => m.Message.CreatedDate))
                {
        %>
                    <tr>
                        <td>
                            <%  Html.RenderAction("RenderMessages", "Messages", new { msg = Item });%>
                        </td>
                    </tr>
        <%
                }
            }
        %>
                </table>
            </td>
            <td style="width: 30%;" valign="top">
            </td>
        </tr>
    </table>
</div>