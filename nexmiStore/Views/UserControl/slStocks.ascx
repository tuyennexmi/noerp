<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        fnChanged();
    });
    function fnChanged() {
        try {
            fnGetQuantityInStock();
        } catch (err) { }
        try {
            fnGetProductInStockDymanic();
        } catch (err) { }
    }
</script>
<%=Html.DropDownList(ViewData["ElementId"].ToString(), (IEnumerable<SelectListItem>)ViewData["WSIs"], new { onchange = "javascript:fnChanged()" })%>