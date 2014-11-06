<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<div style="margin:20px; width:100%">
<%
    string type = ViewData["Type"].ToString();
List<NEXMI.NMImagesWSI> ImageWSIs = (List<NEXMI.NMImagesWSI>)ViewData["ImageWSIs"];
if (ImageWSIs.Count > 0)
{
    var grpImages = ImageWSIs.GroupBy(g => g.TypeId);
    String path = "";
    foreach (var grp in grpImages)
    {   
        if (grp.FirstOrDefault().TypeId == NEXMI.NMConstant.FileTypes.Images)
        {
            foreach (NEXMI.NMImagesWSI Item in grp)
            {   
                path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/images/" + Item.Name;
%>
                <div class="imagebox">
                    <table>
                        <tr>
                            <td>
                        <%  if (Item.TypeId == NEXMI.NMConstant.FileTypes.Images)
                            { %>
                                <img title="<%=Item.Name %>" class="images" alt="<%=Item.Name%>" src="<%=Url.Content("~")%>uploads/_thumbs/<%=Item.Name%>" />
                        <%  }
                            else
                            { %>
                                <label><%=Item.Name%></label>
                        <%  } %>
                            </td>
                        </tr>
                        <tr>
                            <td><h6><%=Item.Description%></h6></td>
                        </tr>
                        <tr>
                            <td>
                                <a href="<%=path %>" target="_blank"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                                <a href="javascript:fnDeleteAttach('<%= Item.Id %>', '<%=Item.TypeId %>')"><img alt="Xóa file này" title="Xóa file này" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                            </td>
                        </tr>
                    </table>
                </div>
<%
            }
        }
        else
        {
            %>
            <table id="tblFiles" class="tbDetails" border=".1" style="width:100%">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Tên files</th>
                        <th>Mô tả</th>
                        <th>Người tải</th>
                        <th>Ngày tải</th>
                        <th></th>
                    </tr>
                </thead>
            <%
            int count = 1;
            foreach (NEXMI.NMImagesWSI Item in grp)
            {
                path = "http://" + Request.Url.Authority + Url.Content("~") + "uploads/files/" + Item.Name;
%>              
                <tr>
                    <td><%=count++ %></td>
                    <td><%=Item.Name %></td>
                    <td><%=Item.Description %></td>
                    <td><%=Item.CreatedBy %></td>
                    <td><%=Item.CreatedDate %></td>
                    <td>
                        <a href="<%=path %>" target="_blank"><img alt="Xem chi tiết" title="Xem chi tiết" src="<%=Url.Content("~")%>Content/UI_Images/16Detail-icon.png" /></a>
                        <a href="javascript:fnDeleteAttach('<%= Item.Id %>', '<%=Item.TypeId %>')"><img alt="Xóa file này" title="Xóa file này" src="<%=Url.Content("~")%>Content/UI_Images/16Delete-icon.png" /></a>
                    </td>
                </tr>
<%                
            }
    %>
            </table>
    <%
        }
    }
}
    %>

    <input type="hidden" id='txtConfirmMsg' value="Bạn muốn xóa file đính kèm này?" />
    <input type="hidden" id='txtOKMsg' value="Bạn đã xóa thành công file đính kèm này!" />
</div>

<script type="text/javascript">

    function fnDeleteAttach(id, type) {
        var answer = confirm($('#txtConfirmMsg').val());
        if (answer) {
            OpenProcessing();
            $.post(appPath + 'Common/DeleteFile', { id: id, type: type }, function (data) {
                CloseProcessing();
                if (data == '') {
                    alert($('#txtOKMsg').val());
                    LoadContentDynamic('<%=type%>Files', 'Utilities/ImagesList', { ownerId: $('#txtId').html(), type: '<%=type %>' });
                }
                else {
                    alert(data);
                }
            });
        }
    }

</script>