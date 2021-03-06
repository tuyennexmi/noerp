﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<div class="windowContent">
    <script type="text/javascript">
        $(function () {
            var source = {
                datatype: "json",
                datafields: [
                    { name: 'Id' },
                    { name: 'Name' },
                    { name: 'FullName' }
                ],
                url: appPath + 'Common/GetPCs',
                formatdata: function (data) {
                    var keyword = $("#txtKeywordCategory").val();
                    return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, keyword: keyword, objectName: '<%=ViewData["ObjectName"]%>' }
                },
                root: 'Rows',
                beforeprocessing: function (data) {
                    source.totalrecords = data.TotalRows;
                },
                sort: function () {
                    fnReloadCategory();
                }
            };
            var dataAdapter = new $.jqx.dataAdapter(source);
            // initialize jqxGrid
            $("#CategoryListGrid").jqxGrid({
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
                    { text: '<%= NEXMI.NMCommon.GetInterface("PC_CODE", langId) %>', datafield: 'Id', width: 100 },
                    { text: '<%= NEXMI.NMCommon.GetInterface("PC_NAME", langId) %>', datafield: 'FullName', sortable: false }
                ]
            });

            $("#CategoryListGrid").bind("rowdoubleclick", function (event) {
                var dataRecord = $("#CategoryListGrid").jqxGrid('getrowdata', event.args.rowindex);
                try {
                    fnSetItemForCombobox('<%=ViewData["ParentId"]%>', dataRecord.Id, Base64.encode(dataRecord.FullName));
                } catch (err) { }
                closeWindow('<%=ViewData["WindowId"]%>');
            });

            var t;
            $("#txtKeywordCategory").on("keyup", function (event) {
                clearTimeout(t);
                t = setTimeout(function () {
                    fnReloadCategory();
                }, 1000);
            });
        });

        function fnReloadCategory() {
            $("#CategoryListGrid").jqxGrid('updatebounddata');
        }
    </script>
    <table style="width: 100%;">
        <tr>
            <td align="right"><input id="txtKeywordCategory" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td><div id="CategoryListGrid"></div></td>
        </tr>
    </table>
</div>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick='javascript:closeWindow("<%=ViewData["WindowId"]%>")' class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>