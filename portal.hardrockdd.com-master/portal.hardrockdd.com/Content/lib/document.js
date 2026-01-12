function AttachmentLoadDoc(dst, fileData, newWindow) {

    var currentUrl = $(dst).attr('data-currenturl');
    var createWindow = $(dst).attr('data-attachmentloaded');;
    //console.log('fileData.Url', fileData.url);
    if (currentUrl != fileData.url) {
        if (newWindow !== true) {
            $(dst).show();
        }
        switch (fileData.Mime) {
            case 'image/jpeg':
            case 'image/jpg':
            case 'image/bmp':
            case 'image/gif':
            case 'image/png':
            case 'image/pjpeg':
                AttachmentLoadPicture(dst, fileData);
                createWindow = true;

                break;
            case 'application/pdf':
                AttachmentLoadPdf(dst, fileData);
                createWindow = true;
                break;
            case 'application/vnd.ms-excel':
            case 'application/msword':
            case "application/vnd.ms-excel.sheet.macroEnabled.12":
            case 'application/vnd.openxmlformats-officedocument.wordprocessingml.document':
            case 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet':
            case 'application/vnd.openxmlformats-officedocument.presentationml.presentation':
                AttachmentLoadMSFile(dst, fileData);
                createWindow = true;
                break;
            case 'application/vnd.ms-project':
                AttachmentLoadOpenButton(dst, fileData);
                break;
            case 'application/octet-stream':
                var fileName = fileData.OriginalName;
                if (fileName.indexOf('.kmz') > -1 || fileName.indexOf('.kml') > -1) {
                    AttachmentLoadKMZ(dst, fileData);
                    createWindow = true;
                }
                else {
                    AttachmentLoadOpenButton(dst, fileData);
                }
                break;
            case 'application/txt':
            case 'htaccess':
            case 'log':
            case 'sql':
            case 'php':
            case 'js':
            case 'json':
            case 'css':
            case 'html':
            default:
                AttachmentLoadOpenButton(dst, fileData, newWindow);
                //console.log(fileData.Mime);
                break;
        }
        $(dst).attr('data-currenturl', fileData.url);
        $(dst).attr('data-attachmentloaded', createWindow);
    }
    if (newWindow && createWindow) {
        $(dst).hide();
        console.log('Open New Window');
        var randomnumber = Math.floor((Math.random() * 100) + 1);
        var w = window.open("", '_blank', 'PopUp' + randomnumber + ',toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1200,height=600');
        var html = $(dst).html();
        $(w.document.body).html(html);
        $(w.document.body).find('iframe').css('height', '100%');
        $(w.document.body).find('img').css('height', '');
        $(w.document.body).find('img').css('width', '100%');
        $(w.document.body).find('img').css('max-width', '1200px;');
        $(w.document.body).find('img').css('left', '');
        $(w.document.body).find('img').css('top', '');
        $(w.document.body).find('img').css('padding', '');
        $(w.document.body).find('img').css('max-height', '');
        $(w.document.body).find('img').css('cursor', '');
        $(w.document.body).find('img').data('magnify', '');
        $(w.document.body).find('img').data('display', '');
    }
}

function AttachmentLoadMSFile(dst, fileData) {

    $(dst).html('');
    $(dst).hide();
    var msElem = $('<iframe frameborder="0" style="width: 100%; height: 470px;" allowfullscreen></iframe>');

    $(msElem).appendTo($(dst));

    var officeUrl = "https://view.officeapps.live.com/op/embed.aspx?src="
    var origin = window.location.origin;
    origin = "https://portal.hardrockdd.com/";

    var fileUrl = origin + fileData.url;
    var officeUrl = officeUrl + fileUrl

    $(dst).fadeIn(500);
    $(msElem).attr('src', officeUrl);

}

function AttachmentLoadGoogleFile(dst, fileData) {

    $(dst).html('');
    $(dst).hide();
    var msElem = $('<iframe frameborder="0" style="width: 100%; height: 470px;" allowfullscreen></iframe>');

    $(msElem).appendTo($(dst));

    var officeUrl = "https://docs.google.com/viewer?url="
    var origin = window.location.origin;
    origin = "https://portal.hardrockdd.com/";

    var fileUrl = origin + fileData.url;
    var officeUrl = officeUrl + fileUrl
    officeUrl = officeUrl + "&embedded=true";

    $(dst).fadeIn(500);
    $(msElem).attr('src', officeUrl);

}

function AttachmentLoadPdf(dst, fileData) {
    $(dst).html('');
    $(dst).hide();
    var pdfElem = $('<iframe frameborder="0" style="width: 100%; height: 600px;" allowfullscreen></iframe>');

    $(pdfElem).appendTo($(dst));
    //origin = "https://portal.hardrockdd.com";
    var origin = window.location.origin;
    var fileUrl = origin + fileData.url;
    var officeUrl = "https://docs.google.com/viewer?url=" + fileUrl + "&embedded=true"
    officeUrl = fileUrl;
    $(dst).fadeIn(500);
    //console.log(officeUrl);
    $(pdfElem).attr('src', officeUrl);
}

function AttachmentLoadPicture(dst, fileData) {
    $(dst).html('');
    $(dst).hide();
    var elem = $('<img src="data:inode/x-empty;base64," data-magnify="gallery"alt="" style="display:block; left:50%; top:50%; padding:0; max-height:450%; cursor: zoom-in;">');
    var pElem = $('<p>Click image to enlarge</p>');
    $(elem).appendTo($(dst));
    // $(pElem).appendTo($(dst));

    var origin = window.location.origin;
    var fileUrl = origin + fileData.url;

    $(dst).fadeIn(500);
    $(elem).attr('src', fileUrl);

    $(elem).attr('src', fileUrl)
        .on('load', function () {
            resizeImage(this, dst);
        });
    $(elem).attr("data-src", fileUrl);
}

function AttachmentLoadOpenButton(dst, fileData) {
    $(dst).html('');
    var elem = $('<a href="#" class="btn btn-primary btn-lg" target="_blank">Button</a>');
    $(elem).appendTo($(dst));

    var origin = window.location.origin;
    var fileUrl = origin + fileData.url;
    $(elem).attr('href', fileUrl);
    $(elem).html("Open " + fileData.Title);
}

function AttachmentLoadKMZ(dst, fileData) {
    //console.log('Load KMZ');
    var origin = window.location.origin;
    //origin = "https://portal.hardrockdd.com";
    var kmzUrl = new URL(fileData.url, origin); //localized url
    $(dst).html('');
    var elem = $('<div class="google-map" style="position: relative;"></div>');
    var btnElem = $('<a href="#" class="btn btn-primary btn-lg" target="_blank">Button</a>');
    var googleElem = $('<div id="googlemap"></div>');
    $(elem).appendTo($(dst));
    $(btnElem).appendTo($(dst));
    $(googleElem).appendTo($(elem));

    $(btnElem).attr('href', fileData.url);
    $(btnElem).html("Open " + fileData.Title);
    var latlng = new google.maps.LatLng(-34.397, 150.644);
    var mapOptions = {
        center: latlng,
        scaleControl: true,
        tiltControl: true,
        //mapTypeControl: false,
        tilt: 45,
        zoom: 5,
        mapTypeControlOptions: { style: google.maps.MapTypeControlStyle.DROPDOWN_MENU },
        mapTypeId: google.maps.MapTypeId.HYBRID,
        navigationControl: false,
    }
    var mapDst = document.getElementById("googlemap");
    var map = new google.maps.Map(mapDst, mapOptions); // get the div by id

    var kmlLayer = new google.maps.KmlLayer(kmzUrl.href, {
        suppressInfoWindows: true,
        preserveViewport: false,
        map: map
    });
    if (kmlLayer.status === "FETCH_ERROR") {
        $(mapDst).hide();
    }
    else {
        $(mapDst).show();
    }
    google.maps.event.addListener(kmlLayer, "status_changed", function () {
        //console.log('kmlLayer', kmlLayer.getStatus());
        if (kmlLayer.getStatus() === "FETCH_ERROR") {
            $(mapDst).hide();
        }
    });
    $(mapDst).fadeIn(500);
}