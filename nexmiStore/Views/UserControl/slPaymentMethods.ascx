<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%=Html.DropDownList("slPaymentMethods", (IEnumerable<SelectListItem>)ViewData["WSIs"])%>


<script type="text/javascript">
    $(function () {
        $("#slBankAccounts").attr('disabled', true);

        $("#slPaymentMethods").on('change', function () {
            var pM = $("#slPaymentMethods").val();
            if (pM == 'PAY02') {
                $("#slBankAccounts").attr('disabled', false);
            }
            else {
                $("#slBankAccounts").val('');
                $("#slBankAccounts").attr('disabled', true);
            }
        });
    });

</script>