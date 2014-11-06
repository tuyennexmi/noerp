<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
	string elementId = ViewData["ElementId"].ToString();
	string owner = ViewData["Owner"].ToString();
	string type = ViewData["Type"].ToString();
	string buttonText = "Select Files", fileTypeDesc = "All Files", fileTypeExts = "*.*";
	switch (type)
	{
	    case "IMG":
	        buttonText = "Select Images";
	        fileTypeDesc = "Image Files";
	        fileTypeExts = "*.gif; *.jpg; *.png; *.bmp";
	        break;
	    case "VID":
	        buttonText = "Select Videos";
	        fileTypeDesc = "Video Files";
	        fileTypeExts = "*.mp4; *.flv; *.avi; *.ogg; *.wmv";
	        break;
	}
%>

<table>
    <tr>
        <td>
            <input type="file" id="<%=elementId%>" name="<%=elementId%>" />
        </td>
        <td id='<%=type%>Files'>
            <%Html.RenderAction("ImagesList", "Utilities", new { ownerId = owner, type = type }); %>
        </td>
    </tr>
</table>
<script type="text/javascript">
    $('#<%=elementId%>').uploadify({
        'swf': appPath + 'Scripts/uploadify/uploadify.swf',
        'uploader': appPath + 'Files/UploadDynamic',
        'formData': { 'owner': '<%=owner%>', 'type': '<%=type%>' },
        'buttonText': '<%=buttonText%>',
        'multi': true,
        'fileTypeDesc': '<%=fileTypeDesc%>',
        'fileTypeExts': '<%=fileTypeExts%>',
        'fileSizeLimit': '100MB',
        'removeTimeout': 1,
        'onUploadSuccess': function (file, data, response) {
            //UploadSuccess('<%=elementId%>');
            alert('Đã lưu thành công');
            LoadContentDynamic('<%=type%>Files', 'Utilities/ImagesList', { ownerId: '<%=owner %>', type: '<%=type%>' });
        }
    });
</script>
