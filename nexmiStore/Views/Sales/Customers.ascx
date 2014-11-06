<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<script type="text/javascript">
    $(function () {
        $('#pagination-Customer').jqPagination({
            page_string: '{current_page} / {max_page} <%= NEXMI.NMCommon.GetInterface("PAGE", langId) %>',
            paged: function (page) {
                fnLoadCustomers(page);
            }
        });

        var t;
        $("#txtKeywordCustomerList").on("keyup", function (event) {
            clearTimeout(t);
            t = setTimeout(function () {
                fnLoadCustomers();
            }, 1000);
        });
    });        

    function fnReloadCustomers() {
        //$("#CustomerGrid").jqxGrid('updatebounddata');
    }

    function fnRefreshCustomers() {
        $("#txtKeywordCustomerList").val("");
        $("#CustomerGrid").jqxGrid('removesort');
        fnLoadCustomers();
    }
</script>
<div>
    <div class="divActions">
        <input type="hidden" id="CustomerGroupId"  value="<%=ViewData["GroupId"]%>" />
        
        <table style="width: 100%;">
            <tr>
                <td>
                </td>
                <td align="right">
                    <input id="txtKeywordCustomerList" value='<%=Session["Keyword"] %>' type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" />
                </td>
            </tr>
            <tr>
                <td>
                    <%
                        string functionId = ViewData["FunctionId"].ToString();
                        
                        //Kiểm tra quyền Insert
                        if (GetPermissions.Get((List<NEXMI.NMPermissionsWSI>)Session["Permissions"], functionId, "Insert"))
                        {
                    %>
                            <button onclick="javascript:fnShowCustomerForm('')" class="button"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <% 
                        }
                        else
                        {
                    %>
                    <button class="button jqx-fill-state-disabled"><%= NEXMI.NMCommon.GetInterface("ADD_NEW", langId) %></button>
                    <%
                        }    
                    %>
                    <button class="button" onclick="javascript:fnLoadCustomers()"><%= NEXMI.NMCommon.GetInterface("REFRESH", langId) %></button>
                </td>
                <td align="right">
                    <%Html.RenderAction("PaginationJS", "UserControl", new { id = "Customer" });%>
                    <%Html.RenderPartial("CustomerSV"); %>
                </td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <div class="divStatus">
            <div class="divButtons">
                <table>
                    <tr>
                        <%--<td><%= NEXMI.NMCommon.GetInterface("SELECT_AREA", langId) %>: </td>--%>
                        <td><%Html.RenderAction("cbbAreas", "UserControl", new { areaId = ViewData["cbbAreas"], areaName = ViewData["cbbAreaName"], elementId = "", holderText = NEXMI.NMCommon.GetInterface("SELECT_AREA", langId) });%></td>                        
                    </tr>
                </table>
            </div>
        </div>
    <div class='divContentDetails'>
        <div id="CustomerGrid">
            <%Html.RenderAction("CustomerList", "Sales", new { groupId = ViewData["GroupId"], viewType = ViewData["ViewType"], keyword = Session["Keyword"], functionId = NEXMI.NMConstant.Functions.Customer }); %>
        </div>
    </div>
    </div>
</div>
