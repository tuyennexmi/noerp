<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    function fnSaveProvince() {
        var selectedItem = $("#jqxTree").jqxTree('selectedItem');
        var itemData = $("#jqxTree").jqxTree('getItem', selectedItem.element);
        var countryId = itemData.id;
        var id = $("#txtProvinceId").val();
        var name = $("#txtProvinceName").val();
        showProcessing();
        $.post(appPath + "UserControl/SaveProvince", { countryId: countryId, id: id, name: name }, function (data) {
            if (data == "") {
                $("#jqxTree").jqxTree('addTo', { label: name }, selectedItem.element);
                showSuccess();
            } else {
                alert(data);
            }
            showButtons();
        });
    }

    function fnResetProvince() {
        $("#txtProvinceId").val("");
        $("#txtProvinceName").val("");
    }
</script>
<table style="width: 100%">
    <tr>
        <td class="lbright">Mã tỉnh/thành phố</td>
        <td><input type="text" id="txtProvinceId" /></td>
    </tr>
    <tr>
        <td class="lbright">Tên tỉnh/thành phố</td>
        <td><input type="text" id="txtProvinceName" /></td>
    </tr>
</table>