<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Documents.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadDocument();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
                { name: 'DocumentId' },
                { name: 'Name' },
                { name: 'Short' }
            ],
            url: appPath + 'Common/GetDocuments',
            formatdata: function (data) {
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder,
                    keyword: $('#txtKeyword').val(),
                    typeId: $('#txtTypeId').val()
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadDocument();
            }
        };
        var actionrenderer = function (row, column, value) {
            var dataRecord = $("#DocumentGrid").jqxGrid('getrowdata', row);
            return '<a href="javascript:fnShowDocumentForm(\'' + dataRecord.DocumentId + '\', \'\')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
            + '<a href="javascript:fnDeleteDocument(\'' + dataRecord.DocumentId + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
        }
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#DocumentGrid").jqxGrid({
            width: '100%',
            autoheight: true,
            pagesize: 15,
            pagesizeoptions: ['15', '20', '30'],
            source: dataAdapter,
            theme: theme,
            enableTooltips: true,
            pageable: true,
            sortable: true,
            rendergridrows: function () {
                return dataAdapter.records;
            },
            virtualmode: true,
            columnsresize: true,
            columns: [
                { text: 'ID', datafield: 'DocumentId', width: 150 },
                { text: 'Tên bài viết', datafield: 'Name', sortable: false },
                { text: '<%= NEXMI.NMCommon.GetInterface("DESCRIPTION", langId) %>', datafield: 'Short', sortable: false },
                { text: '', datafield: 'link', width: 50, cellsrenderer: actionrenderer, cellsalign: 'right', sortable: false },
            ]
        });
        $('#DocumentGrid').bind('rowdoubleclick', function (event) {
            var dataRecord = $('#DocumentGrid').jqxGrid('getrowdata', event.args.rowindex);
            fnLoadDocumentDetail(dataRecord.DocumentId);
        });
        $('#DocumentGrid').on('bindingcomplete', function (event) {
            $(this).jqxGrid('localizestrings', language);
        });
    });
</script>
<div>
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
                        string functionId = NEXMI.NMConstant.Functions.Documents;
                        string typeId = ViewData["TypeId"].ToString();
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                        {
                    %>
                    <button onclick="javascript:fnShowDocumentForm('', '<%=typeId%>')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                    %>
                    
                    <button class="button" onclick="javascript:fnRefreshDocument()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                    <%--<button onclick="javascript:fnExport2Excel()" class="button">Xuất ra Excel</button>--%>
                </td>
                <td align="right" style="float: right;">
                    <%//Html.RenderPartial("DocumentSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <input type="hidden" id="txtTypeId" value="<%=typeId%>"/>
        <table style="width: 100%">
            <tr>
                <td><div id="DocumentGrid"></div></td>
            </tr>
        </table>
    </div>
</div>