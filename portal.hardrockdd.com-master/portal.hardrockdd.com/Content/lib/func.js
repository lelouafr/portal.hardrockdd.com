function updateElementValues(elem, model) {

    $(elem).UpdateElementValues(model);
    //console.log('Server model:', model);
    //$.each(model, function (key, value) {
    //    var updElem = $(elem).find("[name='" + key + "']");
    //    $(updElem).each(function () {
    //        var name = $(this).attr('name');
    //        if (value === null) {
    //            value = "";
    //        }
    //        //console.log('Name: ', name, $(this).val(), String(value));
    //        if ($(this).is('div') || $(this).is('span')) {
    //            var name = $(this).attr('name');
    //            var val = value + "";
                
    //            if (val.indexOf('/Date') >= 0) {
    //                val = dateTimeFormat(val);
    //            }
    //            $(this).html(String(val));
    //        }
    //        else {
    //            var curVal = $(this).val();
    //            curVal = curVal + "";
    //            if (curVal !== String(value)) {
    //                var elemDisabled = $(this).prop("disabled");
    //                $(this).prop("disabled", false);
    //                var isbootstrapSelect = $(this).parent().hasClass('bootstrap-select');
    //                var isbootstrapDatePicker = $(this).data("datepickerid") != undefined;
    //                if (isbootstrapSelect) {
    //                    //console.log('isbootstrapSelect: ', isbootstrapSelect, ' elemDisabled: ', elemDisabled);
    //                    $(this).selectpicker('refresh');
    //                }
    //                if (value != null) {
    //                    var val = value + "";

    //                    if (val.indexOf('/Date') >= 0) {
    //                        //val = parseJsonDate(value);
    //                        value = dateTimeFormat(val);
    //                    }
    //                    if (typeof $(this).data('datepickerid') !== 'undefined') {
    //                        value = dateTimeFormatFromStr(val);
    //                    }
    //                    if (curVal.indexOf("%") >= 0) {
    //                        //console.log("Percent Value : ", value)
    //                        value = Math.round(parseFloat(value) * 100.0);
    //                    }
    //                    if (curVal.indexOf("$") >= 0) {
    //                        value = currencyFormat(parseFloat(value), 2);
    //                    }
    //                }
    //                $(this).val(value);
    //                $(this).prop("disabled", elemDisabled);
    //                if (isbootstrapSelect) {
    //                    $(this).selectpicker('refresh');
    //                }
    //                if (isbootstrapDatePicker) {
    //                    //console.log('isbootstrapDatePicker', isbootstrapDatePicker);
    //                    $(this).datepicker('destroy');
    //                    var datepicker_options = {
    //                        altFormat: "mm/dd/yyyy",
    //                        dateFormat: "mm/dd/yy",
    //                        yearRange: "1000:3000",
    //                        orientation: 'bottom',
    //                        changeMonth: true,
    //                        changeYear: true,
    //                        todayHighlight: true,
    //                    };
    //                    $(this).datepicker(datepicker_options);
    //                }
    //            }
    //        }
    //        //Store current value to be sent for combo box
    //        $(this).attr('data-currentvalue', model[name]);
    //    });
    //});
    //$(elem).find("[data-combokeys]").each(function () {
    //    initDropdown($(this), $(this).attr('data-currentvalue'));
    //});
}

function tableHeight(table, itemCnt, rowHeight) {
    if (itemCnt <= 1) {
        itemCnt = 50;
    }
    var isInModal = $(table).closest('.modal').length != 0;
    var windowHeight = $(window).innerHeight();
    var tableTop = $(table).offset().top;
    var bufferHeight = 100;
    var availableWindowSize = windowHeight - tableTop - bufferHeight;
    var totalRowHeight = itemCnt * rowHeight;
    var minTableHight = 300;
    var resultHeight = 0;
    if (isInModal) {
        resultHeight = minTableHight;
    }
    if (availableWindowSize > totalRowHeight) {
        resultHeight = availableWindowSize;
    }

    if (availableWindowSize < totalRowHeight && availableWindowSize > minTableHight) {
        resultHeight = availableWindowSize;
    }

    if (resultHeight <= 300) {
        resultHeight = windowHeight;
    }
    //console.log(resultHeight, availableWindowSize, totalRowHeight, minTableHight, windowHeight, itemCnt);
    return resultHeight;

}

//function renderSwitcheryElem(elem) {
//    if ($(elem).attr("data-switcheryinit") !== "true") {
//        //console.log('init Switch');
//        var themeColor = COLOR_GREEN;
//        //console.log('Switch theme: ', $(elem).attr('data-theme'));
//        if ($(elem).attr('data-theme')) {
//            switch ($(elem).attr('data-theme')) {
//                case 'red': themeColor = COLOR_RED; break;
//                case 'blue': themeColor = COLOR_BLUE; break;
//                case 'purple': themeColor = COLOR_PURPLE; break;
//                case 'orange': themeColor = COLOR_ORANGE; break;
//                case 'black': themeColor = COLOR_BLACK; break;
//            }
//        }

//        var option = {};
//        option.color = themeColor;
//        option.secondaryColor = ($(elem).attr('data-secondary-color')) ? $(elem).attr('data-secondary-color') : '#dfdfdf';
//        option.className = ($(elem).attr('data-classname')) ? $(elem).attr('data-classname') : 'switchery';
//        option.disabled = $(elem).attr('data-disabled') === "true";
//        option.size = ($(elem).attr('data-size')) ? $(elem).attr('data-size') : 'default';
//        option.disabledOpacity = ($(elem).attr('data-disabled-opacity')) ? parseFloat($(elem).attr('data-disabled-opacity')) : 0.5;
//        option.speed = ($(elem).attr('data-speed')) ? $(elem).attr('data-speed') : '0.5s';
//        //console.log('Switchery', option);
//        var switchery = new Switchery(elem.get(0), option);

//        $(elem).attr("data-switcheryinit", "true");
//        return switchery;
//    }
//};

function LogOff() {
    var url = '/Identity/Account/Logout?returnUrl=%2F'
    $.post(url).done(function (res) {
        window.location.href = res.url;
    });
}

function DZone() {
    return {
        init: function (elem, url) {
            var id = $(elem).attr('id');
            var isInit = $(elem).data("dropzoneinit") === "true";
            if (!isInit) {
                //console.log('DropZone.init: ', id, elem, url);
                this.render(id, elem, url);
                $(elem).data("dropzoneinit", "true");
            }
        },
        render: function (id, elem, url) {
            var dz = new Dropzone("#" + id,
                {
                    url: url,
                    maxFilesize: 5,
                    init: function () {
                        this.on("sending", function (file, xhr, data) {
                            var dataAttr = $(elem).data();
                            console.log(dataAttr);
                            for (var key in dataAttr) {
                                data.append(key, $(elem).data(key));
                            }
                        });
                    },
                });

            //$(elem).dropzone({ url: url });
            //$(elem).dropzone({
            //    url: url,
            //    uploadMultiple: false,
            //    parallelUploads: 1,
            //    timeout: 360000,
            //    maxFilesize: 5, // MB
            //    init: function () {
            //        dpzMultipleFiles = this;
            //        this.on('completemultiple', function (file, json) {
            //            $('.sortable').sortable('enable');
            //        });
            //        this.on('success', function (file, json) {
            //            //console.log(json);
            //            //var table = $('#TableCreditImportListViewModel');
            //            //table.ReloadPanel();
            //        });
            //        this.on('resetFiles', function () {
            //            dpzMultipleFiles.removeAllFiles();
            //            //var table = $('#TableCreditImportListViewModel');
            //            //$(table).ReloadPanel();

            //        });
            //        this.on("sending", function (file, xhr, data) {
            //            var dataAttr = $(elem).data();
            //            console.log(dataAttr);
            //            for (var key in dataAttr) {
            //                data.append(key, $(elem).data(key));
            //            }
            //        });
            //        this.on('queuecomplete', function (data) {
            //            //console.log(data);
            //            dpzMultipleFiles.emit("resetFiles");
            //        })
            //        this.on("addedfile", function (file) {
            //            //file.name = CreateGuid() + file.name;
            //        });
            //        this.on('drop', function (file) { });
            //        this.on("maxfilesexceeded", function (file) { });
            //    }
            //});
        }
    }
}

function currencyFormat(num, decimalplaces) {
    if (num === null) {
        return '';
    }
    var result = num.toFixed(decimalplaces).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
    result = num < 0 ? '(' + result.replace('-', '') + ')' : result;
    result = '$' + result;
    return result;
}

function numberFormat(num, decimalplaces) {
    if (num === null) {
        return '';
    }
    var result = num.toFixed(decimalplaces).replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
    //result = num < 0 ? '(' + result.replace('-', '') + ')' : result;   
    return result;
}

function dateTimeFormat(dateTimeValue) {
    //var dt = new Date(parseInt(dateTimeValue.replace(/(^.*\()|([+-].*$)/g, '')));
    //var dateTimeFormat = (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
    ////console.log(dateTimeValue, dt, dateTimeFormat);
    //return dateTimeFormat;

    //var ticksStr = dateTimeValue.replace(/(^.*\()|([+-].*$)/g, '');
    var ticksStr = dateTimeValue;
    ticksStr = ticksStr.replace('Date', '');
    ticksStr = ticksStr.replace('/(', '');
    ticksStr = ticksStr.replace(')/', '');
    var ticks = parseInt(ticksStr);
    var dt = new Date(ticks);

    var options = { year: 'numeric', month: '2-digit', day: '2-digit' };

    //console.log('Unmodified Date:', dt);
    var TimeZoned = new Date(dt.setTime(dt.getTime() + (dt.getTimezoneOffset() * 60000)));
    //console.log('Modified Date:', dt, 'TimeZoned: ', TimeZoned);

    var dateTimeFormat = TimeZoned.toLocaleDateString("en-US", options);//(dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
    //return dateTimeFormat;
    return dateTimeFormat;
    //return dt.toUTCString();
}

function dateTimeFormatFromStr(dateTimeValue) {
    //var dateVal = parseInt(dateTimeValue.replace(/(^.*\()|([+-].*$)/g, ''));
    var dt = new Date(dateTimeValue);
    var options = { year: 'numeric', month: '2-digit', day: '2-digit' };
    //console.log('dateTimeValue', dateTimeValue, 'dt:', dt);
    var dateTimeFormat = dt.toLocaleDateString("en-US", options);
    return dateTimeFormat;
}