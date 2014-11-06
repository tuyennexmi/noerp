<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>


<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('.button-group').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadProductGrid(page);
            }
        });
        $('img').error(function () {
            $(this).attr('src', '<%=Url.Content("~")%>Content/UI_Images/noimage.png');
        });
        $(document).bind('click', function (e) {
            var $clicked = $(e.target);
            if (!$clicked.parents().hasClass("dropdown"))
                $(".dropdown dd ul").hide();
        });

        $(".dropdown dd ul li a").click(function () {
            $(".dropdown dd ul").hide();
        });

        $('.project-item').on('mouseover', function () {
            $(this).find('.dropdown').css({ display: '' });
        }).on('mouseout', function () {
            $(this).find('.dropdown').css({ display: 'none' });
        }).on('click', function (event) {
            var className = '';
            if ($(event.target).attr('class') != undefined)
                className = $(event.target).attr('class');
            if (className.indexOf('project-child') == -1) {
                fnLoadProjectDetail($(this).attr('id'));
            }
        });
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnLoadProjectGrid();
            }, 1000);
        });
        $("#txtKeyword").focus();
    });

    function showMenu(elementId) {
        $(".dropdown dd ul").hide();
        $('#' + elementId).toggle();
    }
</script>
<%
    List<NEXMI.NMProjectsWSI> WSIs = (List<NEXMI.NMProjectsWSI>)ViewData["WSIs"];
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
<div class="divActions">
    <table>
        <tr>
            <td></td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" value="<%=ViewData["Keyword"]%>" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
        <%
            string functionId = NEXMI.NMConstant.Functions.Project;
            //Kiểm tra quyền Insert
            if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
            {
        %>
                <button onclick="javascript:fnShowProjectForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
        <% 
            }
        %>
                <button class="button" onclick="javascript:$(window).hashchange();"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td style="float: right;">
                <%Html.RenderAction("Pagination", "UserControl", new { currentPage = ViewData["Page"], totalPage = totalPage });%>
                &nbsp;
                <%Html.RenderPartial("ProjectSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <table style="width: 100%" class="tbTemplate">
        <tr>
            <td>
        <%
            foreach(NEXMI.NMProjectsWSI Item in WSIs)
            {
        %>
                <div class="project-item" id="<%=Item.Project.ProjectId%>">
                    <table style="width: 100%; height: 100%;">
                        <tr>
                            <td valign="top" style="height: 16px;">
                                <b><%=Item.Project.ProjectName%></b>
                                <dl class="dropdown" style="display: none;">
                                    <dt><a class="project-child" href="javascript:showMenu('menuProject<%=Item.Project.ProjectId%>')"><img alt="" class="img-12px project-child" src="<%=Url.Content("~")%>Content/UI_Images/32Arrows-Down-circular-icon.png" /></a></dt>
                                    <dd>
                                        <ul id="menuProject<%=Item.Project.ProjectId%>">
                                            <li><a class="project-child" href="javascript:fnShowProjectForm('<%=Item.Project.ProjectId%>', '', '')"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>...</a></li>
                                            <li><a class="project-child" href="javascript:fnDeleteProject('<%=Item.Project.ProjectId%>', '1')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></li>
                                        </ul>
                                    </dd>
                                </dl>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="height: 16px; padding-top: 8px;">
                                <a href="javascript:LoadContent('', 'Projects/Tasks?projectId=<%=Item.Project.ProjectId%>')"><%=Item.Tasks.Count%> Nhiệm vụ</a>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <a href="javascript:LoadContent('', 'Projects/Issues?projectId=<%=Item.Project.ProjectId%>')"><%=Item.Issues.Count%> Vấn đề</a>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="padding-top: 8px; height: 16px;">
                            <% 
                        if (Item.Project.EndDate != null)
                        { 
                            %>
                            <label class="tag-item"><%=Item.Project.EndDate.ToString("dd-MM-yyyy")%></label>
                            <%
                        }
                            %>
                            </td>
                        </tr>
                        <tr>
                            <td valign="top" style="padding-top: 8px">
                                <% 
                        foreach (NEXMI.Customers Member in Item.Teams)
                        {
                                %>
                                <a class="project-child" href="javascript:()"><img class="project-child" style="width: 24px; height: 24px; border: solid 1px #ccc; border-radius: 3px;" alt="" title="<%=Member.CompanyNameInVietnamese%>" src="<%=Url.Content("~")%>Content/avatars/<%=Member.Avatar%>" /></a>
                                <%
                            
                        }    
                                %>
                            </td>
                        </tr>
                    </table>
                </div>
                <%
                    } 
                %>
            </td>
        </tr>
    </table>
</div>