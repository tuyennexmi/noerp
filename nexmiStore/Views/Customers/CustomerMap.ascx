<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script src="http://maps.google.com/maps/api/js?sensor=false" type="text/javascript"></script>


<script type="text/javascript">

    var x = document.getElementById("getCurrentLoc");
    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition, showError);
        }
        else { x.innerHTML = "Geolocation is not supported by this browser."; }
    }

    function showPosition(position) {
        lat = position.coords.latitude;
        lon = position.coords.longitude;
        latlon = new google.maps.LatLng(lat, lon)
        mapholder = document.getElementById('mapholder')
        mapholder.style.height = '250px';
        mapholder.style.width = '100%';

        var myOptions = {
            center: latlon, zoom: 14,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapTypeControl: false,
            navigationControlOptions: { style: google.maps.NavigationControlStyle.SMALL }
        };
        var map = new google.maps.Map(document.getElementById("mapholder"), myOptions);
        var marker = new google.maps.Marker({ position: latlon, map: map, title: "You are here!" });
    }

    function showError(error) {
        switch (error.code) {
            case error.PERMISSION_DENIED:
                x.innerHTML = "User denied the request for Geolocation."
                break;
            case error.POSITION_UNAVAILABLE:
                x.innerHTML = "Location information is unavailable."
                break;
            case error.TIMEOUT:
                x.innerHTML = "The request to get user location timed out."
                break;
            case error.UNKNOWN_ERROR:
                x.innerHTML = "An unknown error occurred."
                break;
        }
    }

</script>

<div style="padding:20px">
    <%
        String latlon = ViewData["Latitude"] + "," + ViewData["Longitude"];
        if (double.Parse(ViewData["Latitude"].ToString()) != 0.0)
        {
        %>  
            <img alt="" src="http://maps.googleapis.com/maps/api/staticmap?center=<%=latlon %>&zoom=14&size=900x300&sensor=false" /> 
            <%}
        else
        { %>
            <input type="hidden" id="lat" value="<%=ViewData["Latitude"] %>" />
            <input type="hidden" id="lon" value="<%=ViewData["Longitude"] %>" />
            <p class="intro" id="getCurrentLoc"><button onclick="getLocation()">Lấy tọa độ</button></p>
            <div id="mapholder"></div>
    <%} %>
    
</div>
