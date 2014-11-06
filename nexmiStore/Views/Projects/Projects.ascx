<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadProject();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
            { name: 'ProjectId' },
            { name: 'ProjectName' },
            { name: 'CustomerName' },
            { name: 'PlannedTime' },
            { name: 'TotalTime' },
            { name: 'TimeSpent' },
            { name: 'StatusName' }
        ],
            url: appPath + 'Common/GetProjects',
            formatdata: function (data) {
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, keyword: $("#txtKeyword").val() }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadProject();
            }
        };
        var actionrenderer = function (row, column, value) {
            var dataRecord = $("#ProjectGrid").jqxGrid('getrowdata', row);
            return '<a href="javascript:fnLoadProjectDetail(\'' + dataRecord.ProjectId + '\')"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" class="gridButtons" /></a>'
            + '<a href="javascript:fnShowProjectForm(\'' + dataRecord.ProjectId + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
            + '<a href="javascript:fnDeleteProject(\'' + dataRecord.ProjectId + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
        }
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#ProjectGrid").jqxGrid({
            width: '100%',
            autoheight: true,
            pagesize: 15,
            pagesizeoptions: ['15', '20', '30'],
            source: dataAdapter,
            theme: theme,
            //editable: true,
            pageable: true,
            sortable: true,
            rendergridrows: function () {
                return dataAdapter.records;
            },
            virtualmode: true,
            columnsresize: true,
            columns: [
            { text: 'ID', datafield: 'ProjectId', width: 100 },
            { text: 'Tên dự án', datafield: 'ProjectName' },
            { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>', datafield: 'CustomerName', sortable: false },
            { text: 'Thời gian dự kiến', datafield: 'PlannedTime', sortable: false, width: 130 },
            { text: 'Tổng thời gian', datafield: 'TotalTime', sortable: false, width: 130 },
            { text: 'Thời gian thực hiện', datafield: 'TimeSpent', sortable: false, width: 130 },
            { text: '<%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>', datafield: 'StatusName', sortable: false, width: 120 },
            { text: '', datafield: 'link', width: 75, cellsrenderer: actionrenderer, cellsalign: 'right', sortable: false }
        ]
        });

        $("#ProjectGrid").bind("rowdoubleclick", function (event) {
            var dataRecord = $("#ProjectGrid").jqxGrid('getrowdata', event.args.rowindex);
            fnLoadProjectDetail(dataRecord.ProjectId);
        });
        $('#ProjectGrid').on('bindingcomplete', function (event) {
            $(this).jqxGrid('localizestrings', language);
        });
    });

    function ChangeCategory() {
        fnReloadProject();
    }

    function fnReloadProject() {
        $("#ProjectGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshProject() {
        $("#txtKeywor").val("");
        $("#ProjectGrid").jqxGrid('removesort');
        fnReloadProject();
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
                    string functionId = NEXMI.NMConstant.Functions.Project;
                    //Kiểm tra quyền Insert
                    if (GetPermissions.GetInsert((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId))
                    {
                %>
                        <button onclick="javascript:fnShowProjectForm('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                <% 
                    }
                %>
                <button class="button" onclick="javascript:fnRefreshProject()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
            </td>
            <td align="right" style="float: right;">
                <%Html.RenderPartial("ProjectSV"); %>
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
    </div>
    <div class='divContentDetails'>
        <table style="width: 100%">
            <tr>
                <td>
                    <div id="ProjectGrid"></div>
                </td>
            </tr>
        </table>
    <</div>
</div>