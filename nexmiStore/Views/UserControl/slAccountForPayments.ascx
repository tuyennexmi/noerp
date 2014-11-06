<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%=Html.DropDownList(ViewData["ElementId"].ToString(), (IEnumerable<SelectListItem>)ViewData["List"])%>

<script type="text/javascript">
    $(function () {
        $("#slBankFor112").attr('disabled', false);

        $("#<%=ViewData["ElementId"].ToString() %>").on('change', function () {
            var accType = $("#<%=ViewData["ElementId"].ToString() %>").val();
            if (accType == '1121') {
                $("#slBankFor112").attr('disabled', false);
            }
            else {
                $("#slBankFor112").val('');
                $("#slBankFor112").attr('disabled', true);
            }
        });
    });

</script>