<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% string langId = Session["Lang"].ToString(); %>

<%--<link href="../../Scripts/fullcalendar-1.5.4/fullcalendar/fullcalendar.css" rel="stylesheet" type="text/css" />--%>
<%--<script src="../../Scripts/fullcalendar-1.5.4/fullcalendar/fullcalendar.js" type="text/javascript"></script>--%>
<link href="../../Scripts/fullcalendar-1.6.4/fullcalendar.css" rel="stylesheet" type="text/css" />
<script src="../../Scripts/fullcalendar-1.6.4/fullcalendar.js" type="text/javascript"></script>

<%
    String FirstandLastName = "Họ và tên: ";
    String PhoneNumber = "Số điện thoại: ";
    String Email = "Email: ";
    String Address = NEXMI.NMCommon.GetInterface("ADDRESS", langId);
    String Title = "Tiêu đề: ";
    String Content = "Nội dung yêu cầu: ";
    String IllegalMessage = "Ngày hẹn của Quý khách không hợp lệ. Xin Quý khách vui lòng đặt lại ngày khác!";
    String DialogTitle = "Thêm một sự kiện";
    String SaveButton = "Lưu";
    String CloseButton = "Thoát";
    String NameValidate = "Quý khách chưa nhập họ tên!";
    String PhoneValidate = "Quý khách chưa nhập số điện thoại!";
    String EmailValidate = "Quý khách chưa nhập email hoặc email của Quý khách không đúng định dạng!";
    String TitleValidate = "Quý khách chưa nhập tiêu đề!";
    String ContentValidate = "Quý khách chưa nhập nội dung!";
    String ExpiredTitle = "Ông/bà: ";
    if((String)ViewData["Language"]=="EN")
    {
        FirstandLastName = "Full Name: ";
        PhoneNumber = "Phone Number: ";
        Email = "Email: ";
        Address = "Address: ";
        Title = "Title: ";
        Content = "Content: ";
        IllegalMessage = "Your date is not valid. Could you please put on other!";
        DialogTitle = "Basic Info";
        SaveButton = "Save";
        CloseButton = "Close";
        NameValidate = "You did not enter your name!";
        PhoneValidate = "You did not enter your phone!";
        EmailValidate = "You did not enter your email or your email is not correct format!";
        TitleValidate = "You did not enter title!";
        ContentValidate = "You did not enter content!";
        ExpiredTitle = "Mr/Ms: ";
    }
%>

<script type="text/javascript">

    $(document).ready(function () {

        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        var calendar = $('#calendar').fullCalendar({
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            },
            height: 500,
            selectable: true,
            selectHelper: true,
            select: function (start, end, allDay) {
                var title = prompt('Event Title:');
                if (title) {
                    calendar.fullCalendar('renderEvent',
						{
						    title: title,
						    start: start,
						    end: end,
						    allDay: allDay
						},
						true // make the event "stick"
					);
                }
                calendar.fullCalendar('unselect');
            },
            editable: true
            
        });

        LoadSchedules();
        var e = jQuery.Event("click");

        // trigger an artificial click event
        $('<span class="fc-button fc-button-today fc-state-default fc-corner-left fc-corner-right fc-state-disabled" unselectable="on">today</span>').trigger(e);
    });


    	
    function LoadSchedules(start, end) {
        $.post(appPath + "Messages/LoadEvents", { start: start, end: end }, function (data) {
            if (data.error == "") {
                var arr = data.Schedule.split("|");
                var JsonArr = new Array();
                for (var i = 0; i < arr.length - 1; i++) {
                    JsonArr[i] = new Object();
                    JsonArr[i].id = arr[i].split("^")[0];
                    JsonArr[i].title = "Tiêu đề:" + arr[i].split("^")[3] + " - " + arr[i].split("^")[4];                    
                    JsonArr[i].start = arr[i].split('^')[1];
                    JsonArr[i].end = arr[i].split('^')[2];
                    JsonArr[i].allDay = false;
                }

                //$('#calendar').fullCalendar('removeEvents');
                $('#calendar').fullCalendar('addEventSource', JsonArr);
                $('#calendar').fullCalendar('rerenderEvents');
            }
            else {
                alert(data.error);
            }
        });
    }

    function isEmailvalid(Email) {
        var Pattern = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
        return Pattern.test(Email);
    }
</script>

<style type="text/css">
    #dateScheduleFilter
    {
        margin:10px 10px 10px 20px;
        padding-bottom:20px;
        border-bottom:dotted 1px #435DA4;
    }
    
    #dateScheduleFilter #ScheduleFilter
    {
        width:60px;
        background-color: #D8D9DA;
    }
    
    .customerInput
    {
        
    }
</style>

<div class="divActions">
    <table style="width: 100%;">
        <tr>
            <td></td>
            <td align="right"><input id="txtKeyword" name="txtKeyword" type="search" results="5" placeholder="<%= NEXMI.NMCommon.GetInterface("KEYWORD", langId) %>" style="width: 250px;" /></td>
        </tr>
        <tr>
            <td>
                
            </td>
            <td align="right">
            </td>
        </tr>
    </table>
</div>
<div class="divContent">
    <div class="divStatus">
        <div class="divButtons">
            
        </div>
    </div>
    <%--<%// Create new Calendar control.
        Calendar calendar1 = new Calendar();

        // Set Calendar properties.
        calendar1.OtherMonthDayStyle.ForeColor = System.Drawing.Color.LightGray;
        calendar1.TitleStyle.BackColor = System.Drawing.Color.Blue;
        calendar1.TitleStyle.ForeColor = System.Drawing.Color.White;
        calendar1.DayStyle.BackColor = System.Drawing.Color.Gray;
        calendar1.SelectionMode = CalendarSelectionMode.DayWeekMonth;
        calendar1.ShowNextPrevMonth = true;

        // Add Calendar control to Controls collection.
        Form1.Controls.Add(calendar1);
         %>
     <form id="Form1" runat="server">

      <h3>Calendar Example</h3>

   </form>--%>
    <div id="calendar"></div>

</div>


<div id="InsertSchedule" style="display:none; font-size:14px;">
    <table border="0" cellpadding="0" cellspacing="0" width="700px">        
        <tr>
            <td><%=Title %></td>
            <td><input type="text" class="customerInput" id="Title" style="width:500px"/></td>
        </tr>
        <tr valign="top">
            <td><%=Content %></td>
            <td><textarea cols="" style="width:500px; resize:none" rows="2" id="Content"></textarea></td>
        </tr>
    </table>
</div>