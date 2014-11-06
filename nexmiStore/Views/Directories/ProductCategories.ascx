<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/ProductCategories.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeywordPC").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadPC();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
            { name: 'Id' },
            { name: 'Code' },
            { name: 'Name' },
            { name: 'FullName' },
            { name: 'Note' }
        ],
            url: appPath + 'Common/GetPCs',
            formatdata: function (data) {
                var keyword = $("#txtKeywordPC").val();
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                    sortorder: data.sortorder, keyword: keyword, parentId: ''
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadPC();
            }
        };
        var actionrenderer = function (row, column, value) {
            var dataRecord = $("#PCGrid").jqxGrid('getrowdata', row);
            return '<a href="javascript:fnShowPCForm(\'' + dataRecord.Id + '\', \'\')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
            + '<a href="javascript:fnDeletePC(\'' + dataRecord.Id + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
        }
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#PCGrid").jqxGrid({
            width: '100%',
            autoheight: true,
            pagesize: 15,
            pagesizeoptions: ['15', '20', '30'],
            source: dataAdapter,
            theme: theme,
            pageable: true,
            sortable: true,
            rendergridrows: function () {
                return dataAdapter.records;
            },
            virtualmode: true,
            columnsresize: true,
            columns: [
                { text: 'ID', datafield: 'Id', width: 100 },
                { text: '<%= NEXMI.NMCommon.GetInterface("PC_CODE", langId) %>', datafield: 'Code', width: 100 },
                { text: '<%= NEXMI.NMCommon.GetInterface("PC_NAME", langId) %>', datafield: 'FullName', sortable: false },
                { text: '<%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>', datafield: 'Note', sortable: false },
                { text: '', datafield: 'link', width: 50, cellsrenderer: actionrenderer, cellsalign: 'right' }
            ]
        });

        $('#PCGrid').on('bindingcomplete', function (event) {
            $(this).jqxGrid('localizestrings', language);
        });
    });

    function fnOrderCategories() {
        LoadContent('', 'UserControl/OrderObjects');
    }
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordPC" name="txtKeywordPC" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionId = NEXMI.NMConstant.Functions.ProductCategories;
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                        {
                    %>
                    <button onclick="javascript:fnShowPCForm('', '', 'Products')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                    %>
                    <button class="button" onclick="javascript:fnRefreshPC()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    <button onclick="javascript:fnLoadImportFromExcel('Categories')" class="button">Nhập từ Excel</button>
                    <button class="button" onclick="javascript:fnOrderCategories()">Sắp xếp</button>
                </td>
                <td align="right" style="float: right;">
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
                    <td><div id="PCGrid"></div></td>
                </tr>
            </table>
        </div>
    </div>
</div>
