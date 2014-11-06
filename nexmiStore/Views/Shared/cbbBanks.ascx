<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(document).ready(function () {
        var source = {
            datatype: "json",
            datafields: [
                { name: 'Id' },
                { name: 'Name' },
                { name: 'Address' }
            ],
            id: 'Id',
            url: appPath + "Common/GetBanksForAutocomplete"
        };
        var dataAdapter = new $.jqx.dataAdapter(source, {
            formatData: function (data) {
                return { keyword: $("#cbbBanks").jqxComboBox('searchString'), limit: 10, mode: "select" }
            }
        });
        $("#cbbBanks").jqxComboBox({
            theme: theme,
            width: 250,
            height: 22,
            source: dataAdapter,
            minLength: 0,
            remoteAutoComplete: true,
            remoteAutoCompleteDelay: 600,
            displayMember: "Name",
            valueMember: "Id",
            search: function (searchString) {
                dataAdapter.dataBind();
            }
        });
        $("#cbbBanks").bind('bindingComplete', function (event) {
            var bankId = $("#txtBankIdTemp").val();
            var bankName = $("#txtBankNameTemp").val();
            if (bankId != "") {
                fnSetBankItem(bankId, bankName);
            }
        });
        $('#cbbBanks').bind('select', function (event) {
            var args = event.args;
            if (args) {
                var index = args.index;
                var item = args.item;
                var value = item.value;
                var label = item.label;
                switch (value) {
                    case "Search":
                        
                        $("#cbbBanks").jqxComboBox('clearSelection');
                        break;
                    case "Create":
                        $("#cbbBanks").jqxComboBox('clearSelection');
                        fnPopupBankForm("");
                        break;
                    default:
                        try {

                        } catch (err) { }
                        break;
                }
            }
        });
    });

    function fnSetBankItem(value, label) {
        $('#cbbBanks').jqxComboBox('insertAt', { value: value, label: label }, 0);
        $('#cbbBanks').jqxComboBox('selectIndex', 0);
    }

    function fnPopupBankForm(id) {
        var windowId = fnRandomString();
        OpenPopup(windowId, "Thông tin ngân hàng", appPath + "UserControl/BankForm", { id: id }, true, true, true, true, 500, 400);
        var done = function () {
            fnSaveBank();
        }
        var cancel = function () {
            CloseCurrentPopup();
        }
        var reset = function () {
            fnResetBankForm();
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
</script>
<input type="hidden" id="txtBankIdTemp" value="<%=ViewData["BankId"]%>" />
<input type="hidden" id="txtBankNameTemp" value="<%=ViewData["BankName"]%>" />
<div id="cbbBanks"></div>
