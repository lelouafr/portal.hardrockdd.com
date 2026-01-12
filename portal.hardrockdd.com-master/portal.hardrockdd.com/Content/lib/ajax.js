
var handlePartialAjax = function (setting) {

    //var targetId = setting.targetId;
    //var ajaxUrl = setting.ajaxUrl;

    //if (targetId === undefined)
    //    var targetContainer = $('#' + app.content.id);
    //else
    var targetContainer = $('#' + setting.targetId);
    //console.log('handlePartialAjax');

    //console.log(setting.ajaxUrl, setting.ajaxData, setting.targetId);
    renderAjax(setting.ajaxUrl, setting.ajaxData, setting.targetid);

    function checkLoading(load) {
        if (load == false) {
            var width = $(targetContainer).innerWidth();
            var height = $(targetContainer).innerHeight();
            if (height === 0)
                height = 300;
            $(targetContainer).html('<div class="d-flex align-items-center justify-content-center" style="height:' + height + 'px; width:' + width + 'px;"><i class="fa fa-circle-notch fa-spin fa-3x"></i></div>');

        } else {
           // $(app.ajax.loader.id).remove();
           // $(targetContainer).removeClass(app.ajax.loader.class);
        }
    }

    function emptyElement() {;
        var width = $(targetContainer).innerWidth();
        var height = $(targetContainer).innerHeight();
        if (height === 0)
            height = 300;
        $(targetContainer).html('<div class="d-flex align-items-center justify-content-center" style="height:' + height + 'px; width:' + width + 'px;"></div>');
    }

    function renderAjax(url, ajaxData, targetId) {
        emptyElement();
        checkLoading(false);

        var targetUrl = url.replace('#', '');
        var targetType = (setting.ajaxType) ? setting.ajaxType : 'GET';
        $.ajax({
            url: targetUrl,
            type: targetType,
            datatype: "json",
            data: ajaxData,
            success: function (data) {
                $(targetContainer).html(data);
                $(targetContainer).fadeIn(300);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $(targetContainer).html(app.ajax.error.html);
            }
        }).done(function () {
            checkLoading(true);
        });
    }
}

var handleTabShow = function () {

    $(document)
        .off('hide.bs.collapse', '.panel')
        .on('hide.bs.collapse', '.panel', function (e) {
            console.log('hide.bs.collapse');
            //e.preventDefault();
        });


    $(document)
        .off('shown.bs.collapse')
        .on('shown.bs.collapse', function (e) {
            $(e.target).find(".dataTable").each(function () {
                if ($(this).attr("data-tabresized") !== "true") {
                    $(this).attr("data-tabresized", "true");
                    $(this).DataTable().columns.adjust();
                }
            });
        });

    $(document)
        .off('shown.bs.tab')
        .on('shown.bs.tab', function (e) {
            console.log($(e.target).attr("href"));
            $($(e.target).attr("href")).find(".dataTable").each(function () {
                if ($(this).attr("data-tabresized") !== "true") {
                    $(this).attr("data-tabresized", "true");
                    $(this).DataTable().columns.adjust();
                }
            });
        });
}