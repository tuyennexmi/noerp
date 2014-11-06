<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    List<NEXMI.Stages> Stages = (List<NEXMI.Stages>)ViewData["Stages"];
    string projectId = ViewData["ProjectId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadProject();
            }, 1000);
        });

        $(document).bind('click', function (e) {
            var $clicked = $(e.target);
            if (!$clicked.parents().hasClass("dropdown"))
                $(".dropdown dd ul").hide();
        });

        $(".dropdown dd ul li a").click(function () {
            $(".dropdown dd ul").hide();
        });

        $('.task-item').on('mouseover', function () {
            $(this).find('.dropdown').css({ display: '' });
        }).on('mouseout', function () {
            $(this).find('.dropdown').css({ display: 'none' });
        }).on('click', function (event) {
            var className = '';
            if ($(event.target).attr('class') != undefined)
                className = $(event.target).attr('class');
            if (className.indexOf('task-child') == -1) {
                fnLoadTaskDetail($(this).attr('id'));
            }
        });
    });

    function fnFoldClick(id) {
        $('#fold' + id).show();
        $('#unfold' + id).hide();
    }

    function fnUnfoldClick(id) {
        $('#fold' + id).hide();
        $('#unfold' + id).show();
    }

    function showMenu(elementId) {
        $(".dropdown dd ul").hide();
        $('#' + elementId).toggle();
    }

    function fnReloadTask() {
        $(window).haschange();
    }
</script>
<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td>
            </td>
            <td align="right">
                <input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
            </td>
        </tr>
        <tr>
            <td>
                <%
                    string functionId = NEXMI.NMConstant.Functions.Task;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                    {
                %>
                <button onclick="javascript:fnShowTaskForm('', '', '<%=projectId%>')" class="color red button">Thêm nhiệm vụ</button>
                <%  
                    }    
                %>
                <button class="button" onclick="javascript:fnPopupStageDialog('', '')">Thêm giai đoạn</button>
            </td>
            <td align="right" style="float: right;">
                <%//Html.RenderPartial("ProjectSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
    <table style="height: 102%; background-color: #F2F2F2; color: #333333; margin: -5px;" class="tbTask" cellpadding="0" cellspacing="0">
        <tr>
    <% 
        string foldDisplay = "none", unfoldDisplay = "";
        foreach (NEXMI.Stages Item in Stages)
        {    
            if(Item.Folded)
            {
                foldDisplay = ""; unfoldDisplay = "none";
            }
    %>
            <td valign="top" style="border-right: 1px solid #ccc; border-collapse: collapse;">
                <table style="width: 100%;">
                    <tr>
                        <td id="unfold<%=Item.StageId%>" style="display: <%=unfoldDisplay%>;">
                            <div class="unfold">
                                <table style="width: 100%;">
                                    <tr>
                                        <td><span style="font-size: 14pt;"><%=Item.StageName%></span></td>
                                        <td style="width: 16px;">
                                            <dl class="dropdown">
                                                <dt><a href="javascript:showMenu('menu<%=Item.StageId%>')"><img alt="" class="img-12px" src="<%=Url.Content("~")%>Content/UI_Images/32Arrows-Down-circular-icon.png" /></a></dt>
                                                <dd>
                                                    <ul id="menu<%=Item.StageId%>">
                                                        <li><a href="javascript:fnFoldClick('<%=Item.StageId%>')">Thu gọn</a></li>
                                                        <li><a href="javascript:fnPopupStageDialog('<%=Item.StageId%>', '')"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></a></li>
                                                        <li><a href="javascript:fnDeleteStage('<%=Item.StageId%>')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></li>
                                                    </ul>
                                                </dd>
                                            </dl>
                                        </td>
                                        <td style="width: 16px;"><a href="javascript:fnShowTaskForm('', '<%=Item.StageId%>', '<%=projectId%>')"><img alt="" class="img-12px" src="<%=Url.Content("~")%>Content/UI_Images/16Add-icon.png" /></a></td>
                                    </tr>
                                    <tr>
                                        <td colspan="3">
                                            <%Html.RenderAction("TasksOfStage", "Projects", new { stageId = Item.StageId, projectId = projectId });%>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td id="fold<%=Item.StageId%>" style="display: <%=foldDisplay%>;">
                            <div class="fold">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="padding-right: 10px;">
                                            <dl class="dropdown">
                                                <dt><a href="javascript:showMenu('menu1<%=Item.StageId%>')"><img alt="" class="img-12px" src="<%=Url.Content("~")%>Content/UI_Images/32Arrows-Down-circular-icon.png" /></a></dt>
                                                <dd>
                                                    <ul id="menu1<%=Item.StageId%>">
                                                        <li><a href="javascript:fnUnfoldClick('<%=Item.StageId%>')">Mở rộng</a></li>
                                                        <li><a href="javascript:fnPopupStageDialog('<%=Item.StageId%>', '')"><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></a></li>
                                                        <li><a href="javascript:fnDeleteStage('<%=Item.StageId%>')"><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></a></li>
                                                    </ul>
                                                </dd>
                                            </dl>
                                        </td>  
                                    </tr>
                                    <tr>
                                        <td><p class="vertical-text" style="font-size: 14pt;"><%=Item.StageName%></p></td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <% 
                }    
            %>
        </tr>
    </table>
    </div>
</div>
