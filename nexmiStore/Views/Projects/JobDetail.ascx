<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string windowId = ViewData["WindowId"].ToString();
    string status = NEXMI.NMConstant.JobsStatus.Draft;
    NEXMI.NMJobsWSI WSI = new NEXMI.NMJobsWSI();
    if (ViewData["WSI"] != null)
    {
        WSI = (NEXMI.NMJobsWSI)ViewData["WSI"];
        status = WSI.Job.Status;
    }

%>
<div class="windowContent" style="position: absolute !important;">
    
    <div>
        <form id="form<%=windowId%>" action="">
            <table style="width: 100%" class="frmInput">
                <tr>
                    <td class="lbright">Tên công việc</td>
                    <td>
                        <%=WSI.Job.Name%>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Mục đích</td>
                    <td>
                        <%=WSI.Job.Purpose %>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Tiêu chí</td>
                    <td>
                        <%=WSI.Job.Criteria %>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Tóm tắt</td>
                    <td>
                        <%=WSI.Job.WorkSummary %>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Thời gian thực hiện</td>
                    <td>
                    <%=WSI.Job.TimeSpent %> (giờ)</td>
                </tr>
                <tr>
                    <td class="lbright">Các đánh dấu</td>
                    <td>
                        <%=WSI.Job.Tags %>
                    </td>
                </tr>
                <tr>
                    <td class="lbright">Hướng dẫn</td>
                    <td>
                        <%=WSI.Job.WorkGuids %>
                    </td>
                </tr>
            </table>
        </form>
    </div>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:closeWindow('<%=windowId%>')" class="button medium">Thoát</button>
    </div>
</div>