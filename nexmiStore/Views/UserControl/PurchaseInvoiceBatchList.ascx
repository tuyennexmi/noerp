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
            url: appPath + 'Common/GetPurchaseInvoiceBatchs',
            formatdata: function (data) {
                var txtKeyword = $("#txtPIBKeyword").val();
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                    sortorder: data.sortorder, txtKeyword: txtKeyword
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadPIB();
            }
        };
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#PIBGrid").jqxGrid({
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

        $("#PIBGrid").bind("rowdoubleclick", function (event) {
            var dataRecord = $("#PIBGrid").jqxGrid('getrowdata', event.args.rowindex);
            $("#txtPIBId").val(dataRecord.InvoiceBatchId);
            ClosePopup("dialog-PIB");
        });
    });

    function fnReloadPIB() {
        $("#PIBGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshPIBList() {
        $("#txtPIBKeyword").val("");
        $("#PIBGrid").jqxGrid('removesort');
        fnReloadPIB();
    }
</script>
<input type="text" id="txtPIBKeyword" /> <button type="button" onclick="javascript:fnReloadPIB()">
    <img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Search-icon.png" /></button>
<div id="PIBGrid"></div>