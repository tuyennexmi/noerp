<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeyword").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadMessage();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
                { name: 'MessageId' },
                { name: 'Content' },
                { name: 'CreatedBy' },
                { name: 'CreatedDate' }
            ],
            url: appPath + 'Common/GetMessages',
            formatdata: function (data) {
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield, sortorder: data.sortorder,
                    keyword: $('#txtKeyword').val(),
                    typeId: '<%=NEXMI.NMConstant.MessageTypes.Comment%>'
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadMessage();
            }
        };
        var actionrenderer = function (row, column, value) {
            var dataRecord = $("#MessageGrid").jqxGrid('getrowdata', row);
            return '<a href="javascript:fnDeleteMessage(\'' + dataRecord.MessageId + '\')"><img alt="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" title="<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" class="gridButtons" /></a>';
        }
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#MessageGrid").jqxGrid({
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
                { text: 'ID', datafield: 'MessageId', width: 150 },
                { text: 'Nội dung', datafield: 'Content', sortable: false },
                { text: 'Người đăng', datafield: 'CreatedBy' },
                { text: 'Ngày đăng', datafield: 'CreatedDate' },
                { text: '', datafield: 'link', width: 50, cellsrenderer: actionrenderer, cellsalign: 'right', sortable: false },
            ]
        });
        $('#MessageGrid').on('bindingcomplete', function (event) {
            $(this).jqxGrid('localizestrings', language);
        });
    });

    function fnDeleteMessage(id) {
        var answer = confirm('Bạn muốn xóa dòng này?');
        if (answer) {
            OpenProcessing();
            $.post(appPath + 'CMS/DeleteMessage', { id: id }, function (data) {
                CloseProcessing();
                if (data == "") {
                    fnReloadMessage();
                }
                else {
                    alert(data);
                }
            });
        }
    }

    function fnReloadMessage() {
        $("#MessageGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshMessage() {
        $("#txtKeyword").val("");
        $("#MessageGrid").jqxGrid('removesort');
        fnReloadMessage();
    }
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
                    <button class="button" onclick="javascript:fnRefreshMessage()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                </td>
                <td align="right" style="float: right;"></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <table style="width: 100%">
            <tr>
                <td><div id="MessageGrid"></div></td>
            </tr>
        </table>
    </div>
</div>