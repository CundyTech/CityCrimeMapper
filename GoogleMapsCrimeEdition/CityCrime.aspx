<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CityCrime.aspx.cs" Inherits="GoogleMapsCrimeEdition.CityCrime" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title>GoogleMapsCrimeEdition</title>

    <style>
        #map {
            width: 100%;
            height: 500px;
            background-color: grey;
        }

        .inlineblock {
            display: inline-block;
        }

        .ctrlwrapper {
            margin: 5px;
        }

        #controls {
            float: left;
        }

        #headerwrapper {
            width: 100%;
        }

     
        
    </style>

</head>
<body>
    <script>

        function addcrimepoints() {

            var crimejsonasstring = document.getElementById("HiddenField").value;

            if (crimejsonasstring) {
                debugger;
                var json = JSON.parse(crimejsonasstring);
                for (var i = 0; i < json.length; i++) {

                    var point = new mappoint(json[i].CrimeID, json[i].Month, parseFloat(json[i].Latitude),
                        parseFloat(json[i].Longitude), json[i].Location, json[i].Category, json[i].OutcomeStatus);

                    addpoints(point);
                }
            }
        }

        //Variables.
        var maplocation;
        var map;
        var marker;
        var infowindow;

        //Map point object.
        function mappoint(cid, mo, lat, lng, loc, cat, otcm) {

            this.CrimeID = cid,
            this.Month = mo,
            this.Latitude = lat,
            this.Longitude = lng,
            this.Location = loc,
            this.Category = cat,
            this.OutcomeStatus = otcm;
        }

        //Map Initialisation.
        function initMap() {
            maplocation = { lat: 51.4113822, lng: -0.7529996 };
            map = new google.maps.Map(document.getElementById('map'), {
                zoom: 13,
                center: maplocation,
                mapTypeId: 'roadmap'
            });

            addcrimepoints();
        }



        //Create and add point to google map.
        function addpoints(point) {
            debugger;

            var pos = { lat: point.Latitude, lng: point.Longitude };
            var marker = new window.google.maps.Marker({
                position: pos,
                map: map,
                clickable: true
            });

            var outcome;
            if (point.OutcomeStatus === "") {
                outcome = "Unknown";
            } else {
                outcome = point.OutcomeStatus;
            }

            var contentString =
                '<div id="content">' +
                    '<div id="siteNotice">' +
                    '</div>' +
                    '<h1 id="firstHeading" class="firstHeading">Crime Infomation</h1>' +
                    '<div id="bodyContent">' +
                    '<p><b>CrimeID</b>: ' + point.CrimeID +
                    '<p><b>Date (yyyy:mm)</b>: ' + point.Month +
                    '<p><b>Location</b>: ' + point.Location +
                    '<p><b>Category</b>: ' + point.Category +
                    '<p><b>Outcome</b>: ' + outcome +
                    '</p>' +
                    '<p>Source: <a href="https://data.police.uk/"> https://data.police.uk/ </a>' +
                    '</p>' +
                    '</div>' +
                    '</div>';


             infowindow = new google.maps.InfoWindow({
                content: contentString
            });

            marker.addListener('click', function () {
                infowindow.close();
                infowindow.open(map, marker);
            });
        }

    </script>
    <form id="form1" runat="server">
        <h3>Google Crime</h3>
        <div id="headerwrapper" class="inlineblock">
         <div id="controls" class="inlineblock">
                <div id="divtype" class="inlineblock ctrlwrapper">
                    <p class="inlineblock">Crime Type</p>
                    <asp:DropDownList runat="server" ID="cmbCrimeType" class="inlineblock">
                    </asp:DropDownList>
                </div>
                <div id="divstatus" class="inlineblock ctrlwrapper">
                    <p class="inlineblock">Crime Status</p>
                    <asp:DropDownList runat="server" ID="cmbCrimeStatus" class="inlineblock">
                    </asp:DropDownList>
                </div>
                <div id="divrefresh" class="inlineblock ctrlwrapper">
                    <button id="btnrefresh" runat="server" type="submit" onserverclick="btnrefresh_OnServerClick">Refresh</button>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="HiddenField" ClientIDMode="Static" runat="server" />
        <div id="map"></div>
        <script async defer type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyD_b-D0gd79AhyosCOwJvR_iPYtR-l7aHI&callback=initMap"></script>
    </form>
</body>
</html>
