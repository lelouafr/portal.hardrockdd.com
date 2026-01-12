var initInputPlugins = function (elem) {
    //console.log('initInputPlugins', $(elem));

    //$(elem).find('table').each(function () {
    //    //console.log('init tables', $(this));
    //    handleTableAutoUpdate(this);
    //});

    //$(elem).find('form').each(function () {
    //    handleFormAutoUpdate($(this));
    //});

    $(elem).find('textarea').each(function () {
        var elem = this;
        var scrollHeight = $(elem).prop('scrollHeight');
        var topBorderHeight = parseFloat($(elem).css("borderTopWidth"));
        var bottomBorderHeight = parseFloat($(elem).css("borderBottomWidth"));
        var insideHeight = scrollHeight + topBorderHeight + bottomBorderHeight;
        var boxHeight = $(elem).outerHeight() + 1;

        //console.log(boxHeight, insideHeight);
        if (boxHeight < insideHeight && boxHeight > 45) {
            $(elem).height(insideHeight + 1);
        }
        else {
            $(elem).height(45);
        }
    });

    $(elem).find('[data-selectinit="false"]').each(function () {
        var elem = this;
        //initDropdown($(elem), $(elem).val());

        //$(elem).attr("data-selectinit", "true");
    });

    $(elem).find('[data-datepickerid]').each(function () {
        datepicker_options = {
            altFormat: "mm/dd/yyyy",
            dateFormat: "mm/dd/yy",
            yearRange: "1000:3000",
            changeMonth: true,
            changeYear: true,
            todayHighlight: true
        };
        inputmask_options = {
            mask: "99/99/9999",
            alias: "date",
            placeholder: "mm/dd/yyyy",
            insertMode: false
        };

        $(this).datepicker(datepicker_options)
            .off('change')
            .on('change', function () {
                $('.datepicker').hide();
                date = $(this).datepicker('getDate');
                date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
            });
    });

    $(elem).find('[data-numberformat="percent"]').each(function () {
        var option = {
            alias: "percentage",
            //digits: 2,
            //digitsOptional: true,
            //radixPoint: ".",
            //placeholder: "0",
            //autoGroup: false,
            //min: -1000,
            //max: 1000,
            //suffix: " %",
            //allowMinus: true
        }
        $(this).inputmask(option);
    });

    $(elem).find('[data-switcheryid]').each(function () {
        //console.log('InitSwitchery New');
        $(this).InitSwitchery();
    });
    $(elem).find(':checkbox').each(function () {
        //console.log('InitSwitchery New');
        $(this).InitSwitchery();
    });

    $(elem).find('[data-dropzone]').each(function () {


        DZone().init(this, $(this).data('uploadurl'));
    });
}