<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Hệ thống quản lý kinh doanh chuyên nghiệp NEXMI!
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h4 style="padding-left:18px">Chào mừng bạn đến với hệ thống quản lý doanh nghiệp <b>NEXMI</b>!</h4>
    <div class="divContent">
        <% 
        //Html.RenderAction("CustomerMap", "Customers");//, new { longitude = "106,629", latitude = "10,84" });
        //Html.RenderAction("SalesByDayOfMonthReport", "Reports", new { width = "1090px", height = "300px", IsTable = "none" });        
        %>
    </div>

</asp:Content>
