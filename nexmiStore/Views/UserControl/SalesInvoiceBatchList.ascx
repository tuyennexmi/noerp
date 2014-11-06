<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(document).ready(function () {
        var source = {
            datatype: "json",
            datafields: [
                { name: 'InvoiceBatchId' },
                { name: 'InvoiceBatchDate' },
                { name: 'BatchStatus' },
                { name: 'Description' },
                { name: 'CreatedBy' },
                { name: 'CreatedDate' }
            ],
            url: appPath + 'Common/GetSalesInvoiceBatchs',
            formatdata: function (data) {
                var keyword = $("#txtKeywordSIB").val();
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                    sortorder: data.sortorder, keyword: keyword
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadSIB();
            }
        };
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#SIBGrid").jqxGrid({
            width: '98%',
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
                { text: '<%= NEXMI.NMCommon.GetInterface("BILL_NO", langId) %>', datafield: 'InvoiceBatchId', width: 100 },
                { text: 'Ngày', datafield: 'InvoiceBatchDate', width: 120 },
                { text: '<%= NEXMI.NMCommon.GetInterface("STATUS", langId) %>', datafield: 'BatchStatus', sortable: false, width: 100 },
                { text: '<%= NEXMI.NMCommon.GetInterface("NOTE", langId) %>', datafield: 'Description', sortable: false },
                { text: '<%= NEXMI.NMCommon.GetInterface("CREATED_BY", langId) %>', datafield: 'CreatedBy', sortable: false, width: 120 },
                { text: '<%= NEXMI.NMCommon.GetInterface("CREATED_DATE", langId) %>', datafield: 'CreatedDate', sortable: false, width: 150 }
            ]
        });

        $("#SIBGrid").bind("rowdoubleclick", function (event) {
            var dataRecord = $("#SIBGrid").jqxGrid('getrowdata', event.args.rowindex);
            $("#txtSIBId").val(dataRecord.InvoiceBatchId);
            ClosePopup("dialog-SIB");
        });
    });

    function fnReloadSIB() {
        $("#SIBGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshSIBList() {
        $("#txtKeywordSIB").val("");
        $("#SIBGrid").jqxGrid('removesort');
        fnReloadSIB();
    }
</script>
Tìm kiếm: <input type="text" id="txtKeywordSIB" /> <button type="button" onclick="javascript:fnReloadSIB()">
    <img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Search-icon.png" /></button>
<div id="SIBGrid"></div>