<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script src="<%=Url.Content("~")%>Scripts/forApp/Stocks.js" type="text/javascript"></script>
<script type="text/javascript">
    var t;
    $("#txtKeywordStock").on("keyup", function (event) {
        clearTimeout(t);
        t = setTimeout(function () {
            fnReloadStock();
        }, 1000);
    });

    var source = {
        datatype: "json",
        datafields: [
            { name: 'Id' },
            { name: 'Name' },
            { name: 'Address' },
//            { name: 'TypeName' }
        ],
        url: appPath + 'Common/GetStocks',
        formatdata: function (data) {
            var keyword = $("#txtKeywordStock").val();
            return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder, keyword: keyword }
        },
        root: 'Rows',
        beforeprocessing: function (data) {
            source.totalrecords = data.TotalRows;
        },
        sort: function () {
            fnReloadStock();
        }
    };
    var actionrenderer = function (row, column, value) {
        var dataRecord = $("#StockGrid").jqxGrid('getrowdata', row);
        return '<a href="javascript:fnShowStockForm(\'' + dataRecord.Id + '\', \'\')"><img alt="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" class="gridButtons" /></a>'
            + '<a href="javascript:fnDeleteStock(\'' + dataRecord.Id + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
    }
    var dataAdapter = new $.jqx.dataAdapter(source);
    $("#StockGrid").jqxGrid({
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
            { text: '<%= NEXMI.NMCommon.GetInterface("CODE", langId) %>', datafield: 'Id', width: 100 },
            { text: '<%= NEXMI.NMCommon.GetInterface("NAME", langId) %>', datafield: 'Name' },
            { text: '<%= NEXMI.NMCommon.GetInterface("ADDRESS", langId) %>', datafield: 'Address' },
//            { text: 'Quy trình', datafield: 'TypeName' },
            { text: '', datafield: 'link', width: 50, cellsrenderer: actionrenderer, cellsalign: 'right', sortable: false },
        ]
    });
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordStock" name="txtKeywordStock" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], NEXMI.NMConstant.Functions.Stock, "Insert"))
                        {
                    %>
                    <button onclick="javascript:fnShowStockForm('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                        else
                        {
                    %>
                    <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <%
                        }    
                    %>
                    <button class="button" onclick="javascript:fnRefreshStock()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
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
                    <td>
                        <div id="StockGrid"></div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
