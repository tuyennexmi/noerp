<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function() {
        var tree = $('#jqxTree');
        tree.jqxTree({ theme: theme, width: '100%', allowDrag: false });
        tree.bind('expand', function (event) {
            var currentItem = tree.jqxTree('getItem', event.args.element);
            if (currentItem.label == "<%= NEXMI.NMCommon.GetInterface("AREA", langId) %>")
                return;
            var $element = $(event.args.element);
            var loader = false;
            var loaderItem = null;
            var children = $element.find('li');
            $.each(children, function () {
                var item = tree.jqxTree('getItem', this);
                if (item.label == 'Loading...' && children.length <= 1) {
                    loaderItem = item;
                    loader = true;
                    return false
                };
            });
            if (loader) {
                tree.jqxTree('removeItem', loaderItem.element);
                $.post(loaderItem.value, { parentId: currentItem.id }, function (data) {
                    for (var x = 0; x < data.length; x++) {
                        tree.jqxTree('addTo', { label: data[x].Name, id: data[x].Id }, $element[0]);
                        var elementByID = $('#jqxTree').find("#" + data[x].Id)[0];
                        tree.jqxTree('addTo', { label: 'Loading...', id: '', value: '<%=Url.Content("~")%>Directories/GetAreas' }, elementByID);
                    }
                });
            }
        });

        var contextMenu = $("#jqxMenu").jqxMenu({ width: '120px', theme: theme, autoOpenPopup: false, mode: 'popup' });
        var clickedItem = null;
        // open the context menu when the user presses the mouse right button.
        $("#jqxTree li").live('mousedown', function (event) {
            var target = $(event.target).parents('li:first')[0];
            var rightClick = isRightClick(event);
            if (rightClick && target != null) {
                tree.jqxTree('selectItem', target);
                var selectedItem = tree.jqxTree('selectedItem');
                var scrollTop = $(window).scrollTop();
                var scrollLeft = $(window).scrollLeft();
                contextMenu.jqxMenu('open', parseInt(event.clientX) + 5 + scrollLeft, parseInt(event.clientY) + 5 + scrollTop);
                return false;
            }
        });
        contextMenu.bind('itemclick', function (event) {
            var item = $(event.args).text();
            var selectedItem = tree.jqxTree('selectedItem');
            var currentItem = tree.jqxTree('getItem', selectedItem.element);
            switch (item) {
                case "Thêm":
                    if (selectedItem != null) {
                        fnPopupAreaDialog();
                    }
                    break;
                case "<%= NEXMI.NMCommon.GetInterface("EDIT", langId) %>":
                    if (selectedItem != null) {
                        fnPopupAreaDialog(currentItem.id);
                    }
                    break;
                case "<%= NEXMI.NMCommon.GetInterface("DELETE", langId) %>":
                    if (selectedItem != null) {
                        fnDeleteArea(currentItem.id);
                    }
                    break;
            }
        });
        // disable the default browser's context menu.
        $(document).bind('contextmenu', function (e) {
            if ($(e.target).parents('.jqx-tree').length > 0) {
                return false;
            }
            return true;
        });
        function isRightClick(event) {
            var rightclick;
            if (!event) var event = window.event;
            if (event.which) rightclick = (event.which == 3);
            else if (event.button) rightclick = (event.button == 2);
            return rightclick;
        }
    });

    function fnPopupAreaDialog(id) {
        var windowId = fnRandomString();
        OpenPopup(windowId, "", appPath + "Directories/AreaForm", { id: id }, true, true, true, true, 500, 250);
        var done = function () {
            fnSaveArea();
        }
        var cancel = function () {
            ClosePopup(windowId);
        }
        var reset = function () {
            fnResetArea();
        };
        var dialogOpts = {
            buttons: {
                "Hoàn tất": done,
                "Đóng": cancel,
                "Nhập lại": reset
            }
        };
        $("#" + windowId).dialog(dialogOpts);
    }

    function fnSaveArea() {
        if (NMValidator("formArea")) {
            var id = $("#txtAreaId").val();
            var name = $("#txtAreaName").val();
            var selectedItem = tree.jqxTree('selectedItem');
            var currentItem = tree.jqxTree('getItem', selectedItem.element);
            $.post(appPath + "Directories/SaveOrUpdateArea", { id: id, name: name, parentId: currentItem.id }, function (data) {
                if (data == "") {
                    tree.jqxTree('addTo', { label: name, id: id }, selectedItem.element);
                    fnResetArea();
                } else {
                    alert(data);
                }
            });
        }
    }
    function fnDeleteArea(id) {
        if (confirm("Bạn có muốn xóa?")) {
            var selectedItem = tree.jqxTree('selectedItem');
            $.post(appPath + "Directories/DeleteArea", { id: id }, function (data) {
                if (data == "") {
                    tree.jqxTree('removeItem', selectedItem.element);
                } else {
                    alert(data);
                }
            });
        }
    }
    function fnResetArea() {
        $("#txtAreaId").val("");
        $("#txtAreaName").val("");
        $("#txtAreaId").focus();
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
                            NEXMI.NMAreasBL BL = new NEXMI.NMAreasBL();
                            NEXMI.NMAreasWSI WSI = new NEXMI.NMAreasWSI();
                            WSI.Mode = "SRC_OBJ";
                            WSI.Area.ParentId = "root";
                            List<NEXMI.NMAreasWSI> WSIs = BL.callListBL(WSI);
                            foreach (NEXMI.NMAreasWSI Item in WSIs)
                            {
                        %>
                            <li id="<%=Item.Id%>"><%=Item.Area.Name%>
                                <ul>
                                    <li item-value="<%=Url.Content("~")%>Directories/GetAreas">Loading...</li>
                                </ul>
                            </li>
                        <%
                            }
                        %>
                        </ul>
                    </li>
                </ul>
            </div>
            <div id='jqxMenu'>
                <ul>
                    <li>Thêm</li>
                    <li><%= NEXMI.NMCommon.GetInterface("EDIT", langId) %></li>
                    <li><%= NEXMI.NMCommon.GetInterface("DELETE", langId) %></li>
                </ul>
            </div>
        </td>
    </tr>
</table>
</div>
<div id="divAreaDetail"></div>