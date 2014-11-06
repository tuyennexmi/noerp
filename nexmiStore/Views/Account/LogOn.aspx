<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Đăng nhập hệ thống</title>
    <link href="<%=Url.Content("~")%>Content/LogOn.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~")%>Content/buttons.css" rel="stylesheet" type="text/css" />
</head>
<body>
<form method="post" action="">
    <div id="header">
        <a href="http://www.nexmi.com" target="_blank"> <img alt="" src="<%=Url.Content("~")%>Content/avatars/logo.png" style="max-width:268px; max-height:112px;"/></a>
        <h1>nexmi IBMs v2014</h1>
    </div>
    <div id="main">
        <table style="width: 100%">
            <tr>
                <td align="center">
                    <div id="divLogOn">
                        <table style="width: 100%;" cellpadding="5px" cellspacing="10px">
                            <tr>
                                <td colspan="2" align="center" style="color: Red; font-style: italic;"><%=ViewData["strError"]%>&nbsp;</td>
                            </tr>
                            <tr>
                                <td style="width: 35%; font-weight: bold;" align="right">Tên đăng nhập</td>
                                <td align="left"><input type="text" name="userCode" style="width: 200px;" value="<%=ViewData["userCode"]%>" /></td>
                            </tr>
                            <tr>
                                <td align="right" style="font-weight: bold;">Mật khẩu</td>
                                <td align="left"><input type="password" name="password" style="width: 200px;" /></td>
                            </tr>
                            <tr>
                                <td align="right" style="font-weight: bold;">Ngôn ngữ</td>
                                <td align="left">
                                    <select name="lang">
                                <%
                                NEXMI.NMTypesBL BL = new NEXMI.NMTypesBL();
                                NEXMI.NMTypesWSI WSI = new NEXMI.NMTypesWSI();
                                WSI.Mode = "SRC_OBJ";
                                WSI.ObjectName = "Languages";
                                List<NEXMI.NMTypesWSI> WSIs = BL.callListBL(WSI);
                                
                                foreach (NEXMI.NMTypesWSI Item in WSIs.OrderByDescending(t=>t.IsDefault))
                                {
                                %>
                                    <option value="<%=Item.Id %>"><%=Item.Name %></option>    
                                <%
                                }
                                 %>
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td align="left">
                                    <button type="submit" class="button" style='width: 200px; height: 40px;'><img style='vertical-align:middle' alt="" title="" src="<%=Url.Content("~")%>Content/UI_Images/16Keys-icon.png" /> Đăng nhập</button>
                                </td>
                            </tr>
                        </table>
                        <div id="powered">
                            Powered by <a href="http://www.nexmi.com" target="_blank" style="text-decoration: none; color: #f7941d">NEXMI Solutions</a>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</form>
</body>
</html>
