<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<% 
    string id = ViewData["OrderId"].ToString();
%>
<script type="text/javascript">
    $(function () {
        var orderId = '<%=id%>';
        var source = {
            datatype: "json",
            datafields: [
                { name: 'ExportId' },
                { name: 'ExportDate' },
                { name: 'CustomerName' },
                { name: 'Details' },
                { name: 'Note' },
                { name: 'CreatedBy' },
                { name: 'Status' }
            ],
            url: appPath + 'Common/GetExports',
            formatdata: function (data) {
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                    sortorder: data.sortorder, referenceId: orderId
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadExport();
            },
            deleterow: function (rowid, commit) {
                var dataRecord = $("#ExportGrid").jqxGrid('getrowdata', rowid);
                fnDeleteExport(dataRecord.ExportId);
            }
        };
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#ExportGrid").jqxGrid({
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
                                { text: '<%= NEXMI.NMCommon.GetInterface("ID", langId) %> ', datafield: 'ExportId', width: 150 },
                                { text: '<%= NEXMI.NMCommon.GetInterface("EXPORT_DATE", langId) %>', datafield: 'ExportDate', width: 150 },
                                { text: '<%= NEXMI.NMCommon.GetInterface("CUSTOMER", langId) %>', datafield: 'CustomerName', width: 200 },
                                { text: '<%= NEXMI.NMCommon.GetInterface("PRODUCT_NAME", langId) %>', datafield: 'Details', width: 80 },
                                { text: '<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>', datafield: 'Note' },
                                { text: '<%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %>', datafield: 'CreatedBy', sortable: false, width: 120 },
                                { text: '<%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>', datafield: 'Status', width: 80 }
                            ]
        });

        $("#ExportGrid").bind("rowdoubleclick", function (event) {
            var dataRecord = $("#ExportGrid").jqxGrid('getrowdata', event.args.rowindex);
            fnLoadExportDetail(dataRecord.ExportId);
        });
    });

    function fnReloadExport() {
        $("#ExportGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshExport() {
        $("#txtExportKeyword").val("");
        $("#ExportGrid").jqxGrid('removesort');
        fnReloadExport();
    }
</script>
<div id="divExport">
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionExport = NEXMI.NMConstant.Functions.Export;
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionExport, "Insert"))
                        {
                    %>
                    <button onclick="javascript:fnExportProduct('', '<%=id%>', '', '')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                        else
                        {
                    %>
                    <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <%
                        }    
                    %>
                    <button class="button" onclick="javascript:fnRefreshExport()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                </td>
                <td align="right" style="float: right;">
                    <%Html.RenderPartial("ExportSV");%>
                </td>
            </tr>
        </table>
    </div>
    <table style="width: 100%" class="tbTemplate">
        <tr>
            <td>
                <div id="ExportGrid"></div>
            </td>
        </tr>
    </table>
</div>