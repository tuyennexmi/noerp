<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%--<% 
    string pos = "absolute";
    if (ViewData["WindowId"] == null)
    {
        pos = "static";
    }
%>
<div class="windowContent" style="position: <%=pos%> !important;">
    <script type="text/javascript">
        $(function () {
            $('#jqxTree').jqxTree({ theme: theme, width: '100%', height: '420px', allowDrag: false });
            var contextMenu = $('#jqxMenu').jqxMenu({ width: '120px', theme: theme, autoOpenPopup: false, mode: 'popup' });
            var clickedItem = null;
            $('#jqxTree li').on('mousedown', function (event) {
                var target = $(event.target).parents('li:first')[0];
                var rightClick = isRightClick(event);
                if (rightClick && target != null) {
                    $('#jqxTree').jqxTree('selectItem', target);
                    var scrollTop = $(window).scrollTop();
                    var scrollLeft = $(window).scrollLeft();
                    $('#jqxMenu').jqxMenu('open', parseInt(event.clientX) + 5 + scrollLeft, parseInt(event.clientY) + 5 + scrollTop);
                    return false;
                }
            });
            $('#jqxMenu').on('itemclick', function (event) {
                var selectedItem = $('#jqxTree').jqxTree('selectedItem');
                var currentItem = null;
                if (selectedItem != null) {
                    currentItem = $('#jqxTree').jqxTree('getItem', selectedItem.element);
                }
                var item = $(event.args).text();
                switch (item) {
                    case 'Thêm':
                        if (selectedItem != null) {
                            fnPopupAreaDialog();
                        }
                        break;
                    case '<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>':
                        if (currentItem != null) {
                            fnPopupAreaDialog(currentItem.id);
                        }
                        break;
                    case '<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>':
                        if (currentItem != null) {
                            fnDeleteArea(currentItem.id);
                        }
                        break;
                }
            });
            $('#jqxTree').bind('expand', function (event) {
                $.ajaxSetup({ async: false });
                var currentItem = $('#jqxTree').jqxTree('getItem', event.args.element);
                var $element = $(event.args.element);
                var loader = false;
                var loaderItem = null;
                var children = $element.find('li');
                $.each(children, function () {
                    var item = $('#jqxTree').jqxTree('getItem', this);
                    if (item.label == 'Loading...' && children.length <= 1) {
                        loaderItem = item;
                        loader = true;
                        return false
                    };
                });
                if (loader) {
                    $('#jqxTree').jqxTree('removeItem', loaderItem.element);
                    $.post(loaderItem.value, { parentId: currentItem.id }, function (data) {
                        for (var x = 0; x < data.length; x++) {
                            $('#jqxTree').jqxTree('addTo', { label: data[x].Name, id: data[x].Id }, $element[0]);
                            var elementByID = $('#jqxTree').find('#' + data[x].Id)[0];
                            $('#jqxTree').jqxTree('addTo', { label: 'Loading...', id: '', value: '<%=Url.Content("~")%>Directories/GetAreas' }, elementByID);
                        }
                    });
                }
                $.ajaxSetup({ async: true });
            });
            // disable the default browser's context menu.
            $(document).bind('contextmenu', function (e) {
                if ($(e.target).parents('.jqx-tree').length > 0) {
                    return false;
                }
                return true;
            });
        });
        function isRightClick(event) {
            var rightclick;
            if (!event) var event = window.event;
            if (event.which) rightclick = (event.which == 3);
            else if (event.button) rightclick = (event.button == 2);
            return rightclick;
        }
        function fnPopupAreaDialog(id) {
            openWindow('Thêm/<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %> Khu Vực', 'Directories/AreaForm', { id: id }, 500, 250);
        }
        function fnSaveArea() {
            if (NMValidator('formArea')) {
                var id = $('#txtAreaId').val();
                var name = $('#txtAreaName').val();
                var selectedItem = $('#jqxTree').jqxTree('selectedItem');
                var currentItem = $('#jqxTree').jqxTree('getItem', selectedItem.element);
                $.post(appPath + 'Directories/SaveOrUpdateArea', { id: id, name: name, parentId: currentItem.id }, function (data) {
                    if (data == '') {
                        $('#jqxTree').jqxTree('addTo', { label: name, id: id }, selectedItem.element);
                        fnResetArea();
                    } else {
                        alert(data);
                    }
                });
            }
        }
        function fnDeleteArea(id) {
            if (confirm('Bạn có muốn xóa?')) {
                var selectedItem = $('#jqxTree').jqxTree('selectedItem');
                $.post(appPath + 'Directories/DeleteArea', { id: id }, function (data) {
                    if (data == '') {
                        $('#jqxTree').jqxTree('removeItem', selectedItem.element);
                    } else {
                        alert(data);
                    }
                });
            }
        }
        function fnResetArea() {
            $('#txtAreaId').val('');
            $('#txtAreaName').val('');
            $('#txtAreaId').focus();
        }
        function fnSelect() {
            var selectedItem = $('#jqxTree').jqxTree('selectedItem');
            if (selectedItem != null) {
                var currentItem = $('#jqxTree').jqxTree('getItem', selectedItem.element);
                if (currentItem.label != '<%= NEXMI.NMCommon.GetInterface("AREA", langId) %>') {
                    fnSetAreaItem(currentItem.id, currentItem.label);
                    fnCancel();
                }
            } else {
                alert('Không có khu vực nào được chọn!');
            }
        }
        function fnCancel() {
            $('#jqxMenu').jqxMenu('destroy');
            closeWindow('<%=ViewData["WindowId"]%>');
        }
    </script>
    <div id="divAreas">
    <table style="width: 100%" class="tbTemplate">
        <tr>
            <td style="width: 50%" valign="top">
                <div id="jqxTree">
                    <ul>
                        <li><%= NEXMI.NMCommon.GetInterface("AREA", langId) %>
                            <ul>
                            <%                             
                                List<NEXMI.NMAreasWSI> WSIs = (List<NEXMI.NMAreasWSI>)ViewData["WSIs"];
                                if (WSIs != null)
                                {
                                    foreach (NEXMI.NMAreasWSI Item in WSIs)
                                    {
                            %>
                                <li id="<%=Item.Id%>"><%=Item.Name%>
                                    <ul>
                                        <li item-value="<%=Url.Content("~")%>Directories/GetAreas">Loading...</li>
                                    </ul>
                                </li>
                            <%
                                    }
                                }
                            %>
                            </ul>
                        </li>
                    </ul>
                </div>
                <div id='jqxMenu'>
                    <ul>
                        <li>Thêm</li>
                        <li type="separator"></li>
                        <li><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></li>
                        <li type="separator"></li>
                        <li><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></li>
                    </ul>
                </div>
            </td>
        </tr>
    </table>
    </div>
</div>
<% 
    if (pos == "absolute")
    {    
%>
<div class="windowButtons">
    <div class="NMButtons">
        <button onclick="javascript:fnSelect()" class="button medium">Chọn</button>
        <button onclick="javascript:fnCancel()" class="button medium"><%= NEXMI.NMCommon.GetInterface("CLOSE", langId) %></button>
    </div>
</div>
<% 
    }
%>--%>
<script src="<%=Url.Content("~")%>Scripts/forApp/Areas.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        var t;
        $("#txtKeywordArea").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnReloadArea();
            }, 1000);
        });

        var source = {
            datatype: "json",
            datafields: [
            { name: 'Id' },
            { name: 'Name' },
            { name: 'FullName' },
            { name: 'ZipCode' }
        ],
            url: appPath + 'Common/GetAreas',
            formatdata: function (data) {
                var keyword = $("#txtKeywordArea").val();
                return { pagenum: data.pagenum, pagesize: data.pagesize, sortdatafield: data.sortdatafield,
                    sortorder: data.sortorder, keyword: keyword
                }
            },
            root: 'Rows',
            beforeprocessing: function (data) {
                source.totalrecords = data.TotalRows;
            },
            sort: function () {
                fnReloadArea();
            },
            deleterow: function (rowid, commit) {
                var dataRecord = $("#AreaGrid").jqxGrid('getrowdata', rowid);
                fnDeleteArea(dataRecord.Id);
            }
        };
        var dataAdapter = new $.jqx.dataAdapter(source);
        // initialize jqxGrid
        $("#AreaGrid").jqxGrid({
            width: '99%',
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
            { text: '<%= NEXMI.NMCommon.GetInterface("AREA_CODE", langId) %>', datafield: 'Id', width: 100 },
            { text: '<%= NEXMI.NMCommon.GetInterface("AREA", langId) %>', datafield: 'FullName', sortable: false },
            { text: 'Zip Code', datafield: 'ZipCode', sortable: false }
        ]
        });

        // create context menu
        var contextMenu = $("#AreaMenu").jqxMenu({ width: 200, height: 'auto', autoOpenPopup: false, mode: 'popup', theme: theme });
        $("#AreaGrid").bind('contextmenu', function () {
            return false;
        });
        // handle context menu clicks.
        contextMenu.bind('itemclick', function (event) {
            var args = event.args;
            var rowindex = $("#AreaGrid").jqxGrid('getselectedrowindex');
            var dataRecord = $("#AreaGrid").jqxGrid('getrowdata', rowindex);
            switch ($(args).text()) {
                case "Sửa":
                    fnPopupAreaDialog(dataRecord.Id);
                    break;
                case "Xóa":
                    var rowid = $("#AreaGrid").jqxGrid('getrowid', rowindex);
                    $("#AreaGrid").jqxGrid('deleterow', rowid);
                    break;
            }
        });
        $("#AreaGrid").bind('rowclick', function (event) {
            if (event.args.rightclick) {
                $("#AreaGrid").jqxGrid('selectrow', event.args.rowindex);
                var dataRecord = $("#AreaGrid").jqxGrid('getrowdata', event.args.rowindex);
                var scrollTop = $(window).scrollTop();
                var scrollLeft = $(window).scrollLeft();
                contextMenu.jqxMenu('open', parseInt(event.args.originalEvent.clientX) + 5 + scrollLeft, parseInt(event.args.originalEvent.clientY) + 5 + scrollTop);
                return false;
            } else {
                contextMenu.jqxMenu('close');
            }
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
                    <input id="txtKeywordArea" name="txtKeywordArea" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionId = NEXMI.NMConstant.Functions.Areas;
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                        {
                    %>
                    <button onclick="javascript:fnPopupAreaDialog('')" class="color red button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                    %>
                    
                    <button class="button" onclick="javascript:fnRefreshArea()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
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
            <div id="AreaGrid"></div>
        </div>
    <div id="AreaMenu">
        <ul>
            <li id="menuEdit"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Edit-icon.png" /><span><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></span></li>
            <li type="separator"></li>
            <li id="menuDelete"><img alt="" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /><span><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></span></li>
        </ul>
    </div>
    </div>
</div>
