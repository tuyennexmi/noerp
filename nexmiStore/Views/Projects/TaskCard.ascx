<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>


<%
    NEXMI.NMTasksWSI WSI = (NEXMI.NMTasksWSI)ViewData["WSI"];
%>

<div style="font-size: medium">

    <div style="text-align: right;">
        <h1>THẺ GIAO VIỆC</h1>
        <label style="text-align: right">Liên 1: Giao cho người nhận.</label>
    </div>

    <div>
    <table style="width:100%; ">
        <tr>
            <td width="50%" valign='top'>
                <table>
                    <tr>
                        <td><b>Hướng dẫn:</b></td>
                    </tr>
                    <tr>
                        <td><%= (WSI.Task.Description != null)? WSI.Task.Description.Replace(";", "<br />") : "" %><br />
                        </td>
                    </tr>
                    <tr>
                        <td><b>Yêu cầu/phản hồi:</b> 
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr valign="bottom">
                        <td><i>Trân trọng cảm ơn bạn! Mong rằng bạn sẽ sớm hoàn thành nhiệm vụ quan trọng này!</i></td>
                    </tr>
                </table>
            </td>
            <td width="50%">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td width="36%">Tên nhiệm vụ:</td>
                        <td ><%=WSI.Task.TaskName%></td>
                    </tr>
                    <tr>
                        <td width="36%"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %>:</td>
                        <td ><%=WSI.Task.TaskId%></td>
                    </tr>
                    <tr>
                        <td width="36%">Mục đích:</td>
                        <td ><%=WSI.Task.Purpose%></td>
                    </tr>
                    <tr>
                        <td width="36%">Tiêu chí:</td>
                        <td ><%=WSI.Task.Criteria%></td>
                    </tr>
                    <tr>
                        <td width="36%">Giao cho:</td>
                        <td ><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Thuộc dự án:</td>
                        <td ><%= (WSI.Project != null)? WSI.Project.ProjectName : "" %></td>
                    </tr>
                    <tr>
                        <td width="36%">Hạng mục:</td>
                        <td ><%=WSI.Task.Category%></td>
                    </tr>
                    <tr>
                        <td width="36%">Người kiểm:</td>
                        <td ><%=WSI.CheckedBy.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày bắt đầu:</td>
                        <td ><%=WSI.Task.StartDate.Value.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày hết hạn:</td>
                        <td ><%=WSI.Task.Deadline.ToString("dd-MM-yyyy")%></td>
                    </tr>
                </table>
            </td>
            <%--<td valign="top">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td >Tên nhiệm vụ:</td>
                        <td ><%=WSI.Task.TaskName%></td>
                        <td ><%= NEXMI.NMCommon.GetInterface("CODE", langId) %>:</td>
                        <td ><%=WSI.Task.TaskId%></td>
                    </tr>
                    <tr>
                        <td >Mục đích:</td>
                        <td ><%=WSI.Task.Purpose%></td>
                        <td >Tiêu chí:</td>
                        <td ><%=WSI.Task.Criteria%></td>
                    </tr>
                    <tr>
                        <td >Giao cho:</td>
                        <td ><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                        <td >Người kiểm:</td>
                        <td ><%=WSI.CheckedBy.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td >Thuộc dự án:</td>
                        <td ><%=WSI.Project.ProjectName%></td>
                        <td >Hạng mục:</td>
                        <td ><%=WSI.Task.Category%></td>
                    </tr>
                    <tr>
                        <td >Ngày bắt đầu:</td>
                        <td ><%=WSI.Task.StartDate.Value.ToString("dd-MM-yyyy")%></td>
                        <td >Ngày kết thúc:</td>
                        <td ><%=WSI.Task.EndDate.Value.ToString("dd-MM-yyyy")%></td>
                    </tr>
                </table>
            </td>--%>
        </tr>
    </table>
    
    <table style="width:100%; text-align:center">
        <tr align="center">
            <td style="width:50%; ">Người nhận:</td>
            <td style="width:50%; ">Người kiểm:</td>
            <td style="width:50%; ">Người giao:</td>
        </tr>
        <tr>    
            <td> <br /><br /></td>
            <td></td>
            <td></td>
        </tr>
        <tr align="center">
            <td style="width:50%;"><%= WSI.AssignedUser.CompanyNameInVietnamese%></td>
            <td style="width:50%;"><%= WSI.CheckedBy.CompanyNameInVietnamese%></td>
            <td style="width:50%;"><%= WSI.Manager.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    </div>
    <br />
    <div>
    <table>
        <tr>
            <td>
                <div style="text-align: center;">
                    <h1>THẺ GIAO VIỆC</h1>
                    <label style="text-align: center">Liên 2: Giao cho người kiểm.</label>
                </div>
            </td>
            <td>
                <div style="text-align: center;">
                    <h1>THẺ GIAO VIỆC</h1>
                    <label style="text-align: center">Liên 3: Lưu.</label>
                </div>
            </td>
        </tr>
    </table>
    <table style="width:100%; ">
        <tr>
            <td width="50%">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td width="36%">Tên nhiệm vụ:</td>
                        <td ><%=WSI.Task.TaskName%></td>
                    </tr>
                    <tr>
                        <td width="36%"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %>:</td>
                        <td ><%=WSI.Task.TaskId%></td>
                    </tr>
                    <tr>
                        <td width="36%">Mục đích:</td>
                        <td ><%=WSI.Task.Purpose%></td>
                    </tr>
                    <tr>
                        <td width="36%">Tiêu chí:</td>
                        <td ><%=WSI.Task.Criteria%></td>
                    </tr>
                    <tr>
                        <td width="36%">Giao cho:</td>
                        <td ><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Thuộc dự án:</td>
                        <td ><%= (WSI.Project != null)? WSI.Project.ProjectName : "" %></td>
                    </tr>
                    <tr>
                        <td width="36%">Hạng mục:</td>
                        <td ><%=WSI.Task.Category%></td>
                    </tr>
                    <tr>
                        <td width="36%">Người kiểm:</td>
                        <td ><%=WSI.CheckedBy.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày bắt đầu:</td>
                        <td ><%=WSI.Task.StartDate.Value.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày hết hạn:</td>
                        <td ><%=WSI.Task.Deadline.ToString("dd-MM-yyyy")%></td>
                    </tr>
                </table>
            </td>
            <td width="50%">
                <table width="100%" style="text-align: right;" >
                    <tr>
                        <td width="36%">Tên nhiệm vụ:</td>
                        <td ><%=WSI.Task.TaskName%></td>
                    </tr>
                    <tr>
                        <td width="36%"><%= NEXMI.NMCommon.GetInterface("CODE", langId) %>:</td>
                        <td ><%=WSI.Task.TaskId%></td>
                    </tr>
                    <tr>
                        <td width="36%">Mục đích:</td>
                        <td ><%=WSI.Task.Purpose%></td>
                    </tr>
                    <tr>
                        <td width="36%">Tiêu chí:</td>
                        <td ><%=WSI.Task.Criteria%></td>
                    </tr>
                    <tr>
                        <td width="36%">Giao cho:</td>
                        <td ><%=WSI.AssignedUser.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Thuộc dự án:</td>
                        <td ><%= (WSI.Project != null)? WSI.Project.ProjectName : "" %></td>
                    </tr>
                    <tr>
                        <td width="36%">Hạng mục:</td>
                        <td ><%=WSI.Task.Category%></td>
                    </tr>
                    <tr>
                        <td width="36%">Người kiểm:</td>
                        <td ><%=WSI.CheckedBy.CompanyNameInVietnamese%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày bắt đầu:</td>
                        <td ><%=WSI.Task.StartDate.Value.ToString("dd-MM-yyyy")%></td>
                    </tr>
                    <tr>
                        <td width="36%">Ngày hết hạn:</td>
                        <td ><%=WSI.Task.Deadline.ToString("dd-MM-yyyy")%></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table style="width:100%; text-align:center">
        <tr align="center">
            <td style="width:50%; ">Người Kiểm tra:</td>
            <td style="width:50%; ">Người giao:</td>
            <td style="width:50%; ">Người nhận:</td>
            <td style="width:50%; ">Người kiểm:</td>
        </tr>
        <tr>
            <td> <br /><br /></td>
            <td></td>
            <td> <br /><br /></td>
            <td></td>
        </tr>
        <tr align="center">
            <td style="width:50%;"><%= WSI.CheckedBy.CompanyNameInVietnamese%></td>
            <td style="width:50%;"><%= WSI.Manager.CompanyNameInVietnamese%></td>
            <td style="width:50%;"><%= WSI.AssignedUser.CompanyNameInVietnamese%></td>
            <td style="width:50%;"><%= WSI.CheckedBy.CompanyNameInVietnamese%></td>
        </tr>
    </table>
    </div>
</div>
