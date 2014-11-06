<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.NMStagesWSI> WSIs = (List<NEXMI.NMStagesWSI>)ViewData["WSIs"];
    string elementId = ViewData["ElementId"].ToString();
%>
<script src="<%=Url.Content("~")%>Scripts/forApp/Stage.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        //$('#tb' + elementId + ' tbody').droppable().draggable();
    });

    function fnAddStageToSession(id, name, statusName) {
        if (document.getElementById(id) == null) {
            $('#tbody<%=elementId%>').append('<tr id="' + id + '" class="<%=elementId%>"><td>' + name + '</td><td>' + statusName + '</td><td><a href="javascript:fnRemoveStageFromSession(\'' + id + '\');" style="font-weight: bold;">x</a></td></tr>');
//            $.post(appPath + 'UserControl/AddStageToSession', { id: id }, function () {
//                $('#tbody<%=elementId%>').append('<tr id="' + id + '" class="<%=elementId%>"><td>' + name + '</td><td></td><td><a href="javascript:fnRemoveStageFromSession(\'' + id + '\');" style="font-weight: bold;">x</a></td></tr>');
//            });
        }
    }

    function fnRemoveStageFromSession(id) {
        $('#' + id).remove();
//        $.post(appPath + 'UserControl/RemoveStageFromSession', { id: id }, function () {
//            $('#' + id).remove();
//        });
    }
</script>
<table style="width: 100%;" cellpadding="5">
    <tr>
        <td valign="top">
            <select multiple="multiple" id="<%=elementId%>" style="height: 150px;">
                <% 
                    string statusName;
                    foreach (NEXMI.NMStagesWSI Item in WSIs)
                    {
                        statusName = (Item.Status != null) ? Item.Status.Name : "";     
                %>
                <option onclick="javascript:fnAddStageToSession('<%=Item.Stage.StageId%>', '<%=Item.Stage.StageName%>', '<%=statusName%>');" ><%=Item.Stage.StageName%></option>
                <%
                    }
                %>
            </select>
            <input type="button" class="button" value="<%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %>" onclick="javascript:fnPopupStageDialog('', '<%=elementId%>')" />
        </td>  
        <td style="width: 90%;" valign="top">
            <table style="width" 100%;" class="tbDetails" id="tb<%=elementId%>">
                <tr>
                    <th>Tên giai đoạn</th>
                    <th><%= NEXMI.NMCommon.GetInterface("STATUS", langId) %></th>
                    <th></th>
                </tr>
                <tbody id="tbody<%=elementId%>">
                <% 
                    if (TempData["CurrentStage"] == null)
                    {
                        foreach (NEXMI.NMStagesWSI Item in WSIs)
                        {
                            if (Item.Stage.IsCommon)
                            {
                %>
                <tr id="<%=Item.Stage.StageId%>" class="<%=elementId%>">
                    <td><%=Item.Stage.StageName%></td>
                    <td><%=(Item.Status == null) ? "" : Item.Status.Name%></td>
                    <td><a href="javascript:fnRemoveStageFromSession('<%=Item.Stage.StageId%>');" style="font-weight: bold;">x</a></td>
                </tr>
                <%
                            }
                        }
                    }
                    else
                    {
                        List<NEXMI.Stages> Stages = (List<NEXMI.Stages>)TempData["CurrentStage"];
                        foreach (NEXMI.Stages Item in Stages)
                        {
                %>
                <tr id="<%=Item.StageId%>" class="<%=elementId%>">
                    <td><%=Item.StageName%></td>
                    <td><%=NEXMI.NMCommon.GetStatusName(Item.RelatedStatus, langId)%></td>
                    <td><a href="javascript:fnRemoveStageFromSession('<%=Item.StageId%>');" style="font-weight: bold;">x</a></td>
                </tr>
                <%
                        }
                    } 
                %>
                </tbody>
            </table>
        </td>
    </tr>
</table>
