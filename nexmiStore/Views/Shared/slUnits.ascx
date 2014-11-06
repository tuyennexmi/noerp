<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    NEXMI.NMProductUnitsBL BL = new NEXMI.NMProductUnitsBL();
    NEXMI.NMProductUnitsWSI WSI = new NEXMI.NMProductUnitsWSI();
    WSI.Mode = "SRC_OBJ";
    List<NEXMI.NMProductUnitsWSI> WSIs = BL.callListBL(WSI);
%>
<%=Html.DropDownList("slProductUnits", new SelectList(WSIs, "Id", "Name"), new { onchange = "", style = "width: 50px;" })%>

<%
    if (!NEXMI.NMCommon.GetSetting("ALLOW_MULTI_UNITS"))
    {
%>
    <script type='text/javascript'>
        $(function () {
            $('#slProductUnits').prop('disabled', true);
        });
    </script>
<%
    }
%>