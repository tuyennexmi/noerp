<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
    string mode = ViewData["Mode"].ToString();
%>
<script type="text/javascript">
    function fnImportFromExcel() {
        $('#formExcelUpload').ajaxSubmit({
            url: appPath + 'UserControl/ReadExcel',
            beforeSubmit: function () {
                OpenProcessing();
            },
            success: function (data) {
                CloseProcessing();
                if (data != '')
                    alert(data);
                else {
                    switch ('<%=mode%>') {
                        case 'Categories':
                            LoadContent('', 'Directories/ProductCategories');
                            break;
                        case 'Products':
                            LoadContent('', 'Directories/Products');
                            break;
                    }
                }
            }
        });
    }
</script>
<div>
    <div class="divActions">
        <table style="width: 100%;">
            <tr>
                <td>
                    <div id="progressbar"></div>
                    <div class="MNButtons">
                        <button onclick="javascript:fnImportFromExcel()" class="button">Hoàn tất</button>
                        <button onclick="javascript:history.back();" class="button">Thoát</button>
                    </div>
                </td>
                <td align="right" style="float: right;"></td>
            </tr>
        </table>
    </div>
    <div class="divContent">
        <form id="formExcelUpload" action="" method="post" enctype="multipart/form-data">
            <br style="clear: both;" />
            <input type="hidden" id="mode" name="mode" value="<%=mode%>" />
            <input type="file" id="excelUpload" name="excelUpload" />
        </form>
        <% 
            switch (mode)
            {
                case "Categories":
        %>
        <p>* <b>Tải tập tin mẫu <a href="<%=Url.Content("~")%>Content/Data/Template-CategoryList.xls" style="color: Red;">tại đây</a></b></p>
        <%
break;
                case "Products":
        %>
        <p>* <b>Tải tập tin mẫu <a href="<%=Url.Content("~")%>Content/Data/Template-ProductList.xls" style="color: Red;">tại đây</a></b></p>
        <%
break;
            }
        %>
    </div>
</div>