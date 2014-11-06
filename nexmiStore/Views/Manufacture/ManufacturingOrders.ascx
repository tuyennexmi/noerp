<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript" src="<%=Url.Content("~") %>Scripts/forApp/Manufacturing.js"></script>

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

<%  List<NEXMI.NMProjectsWSI> WSIs = (List<NEXMI.NMProjectsWSI>)ViewData["WSIs"];
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
                    string functionId = NEXMI.NMConstant.Functions.ManufactureOrders;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                    {
                %>
                        <button onclick="javascript:fnShowMOForm('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
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
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
        <%Html.RenderAction("ManufacturingOrderList"); %>
    </div>
</div>