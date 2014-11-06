<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    $(function () {
        $('#<%=ViewData["ToUserId"].ToString()%>MSG').focus();
    });
</script>
<div class="nmChatWindow"> 
    <div id="<%=ViewData["ToUserId"].ToString()+"CON"%>" class="auto"> </div>
    <div>
        <input id="<%=ViewData["ToUserId"].ToString()+"MSG"%>" type="text" style="width:98%;" placeholder="Enter để gửi..." onKeydown="if (event.keyCode == 13) SendMessage('<%=ViewData["ToUserId"].ToString()%>');"/>
    </div>
</div>