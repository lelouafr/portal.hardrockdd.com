/// <reference path="../plugins/pdfjs/pdf.worker.js" />
/// <reference path="../plugins/pdfjs/pdf.worker.js" />

/*
	----------------------------
	Control CONTENT TABLE
	----------------------------

	<!-- ======== GLOBAL SCRIPT SETTING ======== -->
	01. Custom Functions
	02. Handle Table
	03. Handle Form
	04. Handle Switch
	05. Handle Validate
	06. Handle Panes
	07. Handle Combo Buttons
	08. Handle Combo Init
*/

/* 01.  Custom Functions
-----------------------------------*/
//function updateElementValues(elem, model) {
//    //console.log('Server model:', model);
//    $.each(model, function (key, value) {
//        var updElem = $(elem).find("[name='" + key + "']"); 
//        $(updElem).each(function () {
//            var name = $(this).attr('name');
//            if (value === null) {
//                value = "";
//            }
//            //console.log('Name: ', name, $(this).val(), String(value));
//            if ($(this).is('div') || $(this).is('span')) {
//                var name = $(this).attr('name');
//                if (value.indexOf('/Date') >= 0) {
//                    value = dateTimeFormat(value);
//                }
//                $(this).html(String(value));
//            }
//            else {
//                var curVal = $(this).val();
//                curVal = curVal + "";
//                if (curVal !== String(value)) {
//                    var elemDisabled = $(this).prop("disabled");
//                    $(this).prop("disabled", false);
//                    var isbootstrapSelect = $(this).parent().hasClass('bootstrap-select');
//                    var isbootstrapDatePicker = $(this).data("datepickerid") != undefined;
//                    if (isbootstrapSelect) {
//                        //console.log('isbootstrapSelect: ', isbootstrapSelect, ' elemDisabled: ', elemDisabled);
//                        $(this).selectpicker('refresh');
//                    }
//                    if (value != null) {
//                        var val = value + "";

//                        if (val.indexOf('/Date') >= 0) {
//                            //val = parseJsonDate(value);
//                            value = dateTimeFormat(val);
//                        }
//                        if (curVal.indexOf("%") >= 0) {
//                            //console.log("Percent Value : ", value)
//                            value = Math.round(parseFloat(value) * 100.0);
//                        }
//                        if (curVal.indexOf("$") >= 0) {
//                            value = currencyFormat(parseFloat(value), 2);
//                        }
//                    }
//                    $(this).val(value);
//                    $(this).prop("disabled", elemDisabled);
//                    if (isbootstrapSelect) {
//                        $(this).selectpicker('refresh');
//                    }
//                    if (isbootstrapDatePicker) {
//                        //console.log('isbootstrapDatePicker', isbootstrapDatePicker);
//                        $(this).datepicker('destroy');
//                        var datepicker_options = {
//                            altFormat: "mm/dd/yyyy",
//                            dateFormat: "mm/dd/yy",
//                            yearRange: "1000:3000",
//                            orientation: 'bottom',
//                            changeMonth: true,
//                            changeYear: true,
//                            todayHighlight: true,
//                        };
//                        $(this).datepicker(datepicker_options);
//                    }
//                }
//            }
//            //Store current value to be sent for combo box
//            $(this).attr('data-currentvalue', model[name]);
//        });
//    });
//    $(elem).find("[data-combokeys]").each(function () {
//        initDropdown($(this), $(this).attr('data-currentvalue'));
//    });
//}
function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
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

function loadTable(caller, dataUrl, pageNum, pageSize, searchString) {
    var data = {
        CurrentPage: pageNum,
        PageSize: pageSize,
        SearchString: searchString
    };

    var target = $(caller).closest('.panel');
    var targetBody = $(target).find('.panel-body');
    var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';

    $(target).addClass('panel-loading');
    $(targetBody).prepend(spinnerHtml);
    $.ajax({
        type: "Get",
        dataType: "html",
        url: dataUrl,
        data: data,
        success: function (data) {
            $(target).removeClass('panel-loading');
            $(target).find('.panel-loader').remove();
            $(targetBody).html(data);
            initPanels();
        }
    });
}

//jQuery.fn.extend({
//    //alterClass: function (removals, additions) {

//    //    var self = this;

//    //    if (removals.indexOf('*') === -1) {
//    //        // Use native jQuery methods if there is no wildcard matching
//    //        self.removeClass(removals);
//    //        return !additions ? self : self.addClass(additions);
//    //    }

//    //    var patt = new RegExp('\\s' +
//    //        removals.
//    //            replace(/\*/g, '[A-Za-z0-9-_]+').
//    //            split(' ').
//    //            join('\\s|\\s') +
//    //        '\\s', 'g');

//    //    self.each(function (i, it) {
//    //        var cn = ' ' + it.className + ' ';
//    //        while (patt.test(cn)) {
//    //            cn = cn.replace(patt, ' ');
//    //        }
//    //        it.className = $.trim(cn);
//    //    });
//    //    return !additions ? self : self.addClass(additions);
//    //},
//    //Busy: function () {
//    //    if (this.is("button")) {
//    //        var button = this;
//    //        $(button).prop('disabled', true);
//    //        var iElem = $(button).find('i');
//    //        if (iElem.length === 0) {
//    //            //console.log('buttun i added');
//    //            $(button).prepend("<i class='fa fa-spinner fa-spin'></i>");
//    //            button.attr('data-oldclass', "fa");
//    //        }
//    //        else if (!$(iElem).hasClass('fa-spinner')) {            
//    //            var classList = "";
//    //            classList = $(iElem).attr('class');
//    //            $(iElem).alterClass('fa-*', 'fa-spinner').addClass('fa-spin');
//    //            button.attr('data-oldclass', classList);
//    //        }
//    //    }
//    //},
//    //NotBusy: function () {
//    //    if (this.is("button")) {
//    //        var button = this;
//    //        var iElem = $(button).find('i');
//    //        if ($(iElem).hasClass('fa-spinner')) {
//    //            var originalClass = button.attr('data-oldclass');
//    //            $(iElem).attr('class', originalClass);
//    //        }
//    //        $(button).prop('disabled', false);
//    //    }
//    //}
//});

/* 02.  Handle Table
-----------------------------------*/ 
//jQuery.fn.extend({
//    SerializeTableRow: function () {
//        var trValues = "";
//        $(this).find(":input:not(button)").each(function () {
            
//            var elemId = $(this).attr('name');
//            if (!elemId) {
//                var parentElem = $(this).parent();
//                if (!$(parentElem).hasClass('bs-searchbox')) {
//                    //console.log('Control has no name Attr:', $(this));
//                }
//            }
//            if (elemId) {
//                if (trValues !== '') {
//                    trValues += '&';
//                } 
//                switch ($(this).attr('type')) {
//                    case "checkbox":
//                        //elemId = elemId.replace(/_([0-9][0-9]{0,3}|10000)/g, '');
//                        //console.log(elemId, encodeURIComponent($(this).is(':checked')), $(this).is(':checked'));
//                        trValues += elemId + '=' + $(this).is(':checked');//encodeURIComponent($(this).is(':checked'));
//                        break;
//                    default:
//                        //elemId = elemId.replace(/_([0-9][0-9]{0,3}|10000)/g, '');
//                        var val = $(this).val();
//                        if ($.isArray(val)) {
//                            val = val.join("|");
//                        }
//                        if (val !== null) {
//                            var regExp = /[a-zA-Z]/g;
//                            if (!regExp.test(val)) {
//                                if ($(this).attr("data-type") == "number") {
//                                    var number = val.replace(/[^0-9.-]+/g, "");
//                                    number = number.replace(/,/g, '');
//                                    if (val.indexOf("%") >= 0) {
//                                        val = parseFloat(val) / 100.0;
//                                    }
//                                    else if (val.indexOf("$") >= 0) {
//                                        var isNegative = val.indexOf("(") >= 0 || val.indexOf("-") >= 0;
//                                        var floatVal = parseFloat(number);
//                                        val = floatVal * (isNegative ? -1.0 : 1.0);
//                                    }
//                                    else if (!isNaN(parseFloat(number))) {
//                                        var floatVal = parseFloat(number);
//                                        val = floatVal
//                                    }

//                                    //console.log(val, $(this).val());
//                                }
//                            }
//                        }
//                        if (val != null) {
//                            trValues += elemId + '=' + encodeURIComponent(val);

//                        }
//                }
//            }
//        });


//        var formData = trValues.split("&");
//        var data = {};
//        for (var key in formData) {
//            var keyname = formData[key].split("=")[0];
//            var keyVal = decodeURIComponent(formData[key].split("=")[1]);
//            if (keyVal !== null && keyVal !== "null") {
//                data[keyname] = keyVal;
//            }
//        }
//        return data;
//        //return trValues;
//    }
//}); 

function getMaxTabIndex () {
    var idx = -1;
    $('body [tabindex]').attr('tabindex', function (a, b) {
        idx = Math.max(idx, +b);
    });
    return idx;
}

var handleDHTMLXTableEvents = function (tableObj) {
    var sel_row;
    var treeGrid = tableObj;
    //console.log('handleDHTMLXTableEvents', tableObj)
    tableObj.events.on("CellClick", function (row, column, e) {
        var table = $(this._container).closest('.dhtmlx_treegrid');
        if (table.length == 0) {
            table = this._container;
        }
        var selected = this.selection._selectedCell;
        //console.log(this, sel_row, selected.row.id, this._container);
        if (sel_row != selected.row.id)
        {
            sel_row = selected.row.id;
            var dstId = $(table).data("dstselect");
            var url = $(table).data("urlselect");
            var validate = $(table).data("dstselectvalidate");
            if (!dstId || !url) {
                return;
            }

            var data = selected.row;
            var dst = $('#' + dstId);
            //console.log(url, data, dst);

            /*Loader*/
            var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';
            $(dst).addClass('panel-loading');
            $(dst).append(spinnerHtml);

            var urlArr = url.split(',');
            $.each(urlArr, function (index, value) {
                url = $.trim(value);
                $.ajax({
                    type: "Get",
                    dataType: "html",
                    url: url,
                    async: false,
                    data: data,
                    success: function (res) {
                        setTimeout(function () {
                            var divId = dstId + '_' + index;
                            var html = '<div id="' + divId + '">\r\n';
                            html += res;
                            html += '\r\n</div>';

                            if (index === 0)
                                dst.html(html);
                            else
                                dst.append(html);

                            if (validate) {

                                $(dst).PostValidate(this, function (valid) {

                                });
                                //setTimeout(function () {
                                //    dst.ready(function () {
                                //        $(dst).Validate(function (errorElem) {
                                //            errorElem = !errorElem ? $('#' + dstId) : errorElem;
                                //            $('html, body').animate({
                                //                scrollTop: $(errorElem).offset().top - 150
                                //            }, 300)
                                //        });
                                //    });
                                //}, 500);
                            }
                            var panel = $('#' + divId).find('.panel-body');
                            $(panel).find('table').each(function () {
                                Table().init(this);
                            });

                            var h = $("body").height();
                            h -= $(table).offset().top;
                            h -= 20;
                            if (h >= 600) {
                                $(table).height(h);
                            }
                            else {
                                $(table).height(600);
                            }
                            $(dst).removeClass('panel-loading');
                            $(dst).find('.panel-loader').remove();
                            $('html, body').animate({
                                scrollTop: ($(dst).offset().top - 150)
                            }, 300);
                        }, 100);
                    }
                });
            });
        }
    });
}

var handleTableAutoUpdate = function (tableElem) {
    var isBinding = $(tableElem).data("tableinit") === "true";
    if (!isBinding) {
        console.log('handleTableAutoUpdate: ', $(tableElem));
        var id = $(tableElem).attr('id');
        var idx = getMaxTabIndex();
        $(tableElem).find('tr').each(function () {
            $(this).find(':input').each(function () {
                $(this).attr('tabindex', idx);
                idx++;
            });
        });

        $(tableElem)
            .off('click', 'tr:has(td):not(.js-switch)')
            .on('click', 'tr:has(td):not(.js-switch)', function (e) {
                console.log('Row Selected OLD CODE!');
                var refreshPanel = false;
                var table = $(this).closest('table');
                var eventTargetElem = $(e.target);
                var dstId = $(table).data("dstselect");

                if (!$(this).hasClass('active')) {
                    if (!e.ctrlKey) {
                        $(table).find('tr.active').removeClass('active');
                    }
                    $(this).addClass('active');
                    refreshPanel = true;
                    //var dst = $('#' + dstId);
                    //dst.html('');
                }
                else {
                    return;
                }



                //if ($(this).hasClass('active') ) {
                //    var dstId = $(table).data("dstselect");
                //    var dst = $('#' + dstId);
                //    dst.html('');
                //}
                //else if (!$(this).hasClass('active')) {
                //    $(table).find('tr.active').removeClass('active');
                //    $(this).addClass('active');
                //    refreshPanel = true;
                //}


                if (!$(this).data("entitykey")) {
                    console.log('No refresh');
                    refreshPanel = false;
                }
                if ($(eventTargetElem).closest('.switchery').length > 0 ||
                    $(eventTargetElem).hasClass('.switchery')) {
                    refreshPanel = false;
                    $(table).find('tr.active').removeClass('active');
                }

                if (refreshPanel) {
                    var dstId = $(table).data("dstselect");
                    var url = $(table).data("urlselect");
                    var validate = $(table).data("dstselectvalidate");
                    
                    if ($(this).find('[name="EntityName"]')) {
                        var panel = $(table).closest('.panel');
                        var histBtn = $(panel).find('[data-click="panel-history"]');
                        var auditData = {
                            EntityName: $(this).find('[name="EntityName"]').val(),
                            EntityKeyString: $(this).find('[name="EntityKeyString"]').val()
                        };
                        //console.log('Log info Found', auditData, histBtn);
                        $(histBtn).data('auditentityname', auditData.EntityName);
                        $(histBtn).data('auditentitykeystring', auditData.EntityKeyString);
                        $(histBtn).show();
                    }
                    if (!dstId || !url) {

                    }
                    else {
                        var data = $(this).data("entitykey");
                        var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';

                        var urlArr = url.split(',');
                        var dstArr = dstId.split(',');
                        if ($(this).find(".is-invalid").length !== 0) {
                            validate = true;
                        }
                        //console.log('Load arrays: ',urlArr, dstArr);
                        $.each(urlArr, function (index, value) {
                            url = $.trim(value);
                            var dst = $('#' + dstArr[0].trim());
                            if (dstArr.length > 0) {
                                dst = $('#' + dstArr[index].trim());
                                dstId = dstArr[index].trim();
                            }
                            $(dst).addClass('panel-loading');
                            $(dst).append(spinnerHtml);

                            //console.log('Load sub panel', url, data, dstId);
                            $.ajax({
                                type: "Get",
                                dataType: "html",
                                url: url,
                                async: true,
                                data: data,
                                success: function (res) {
                                    //setTimeout(function () {
                                    var divId = dstId + '_' + index;
                                    var html = '<div id="' + divId + '">\r\n';
                                    html += res;
                                    html += '\r\n</div>';

                                    //if (index === 0)
                                    //    dst.html(html);
                                    //else
                                    //    dst.append(html);
                                    dst.html(html);
                                    if (validate) {
                                        $(dst).ready(function () {
                                            $(dst).PostValidate(this, function (valid) { });
                                        });
                                    }
                                    var panel = $('#' + divId).find('.panel-body');
                                    $(panel).find('table').each(function () {
                                        Table().init(this);
                                    });

                                    $(dst).removeClass('panel-loading');
                                    $(dst).find('.panel-loader').remove();
                                }
                            });
                        });
                    }
                }
            });

        $(tableElem)
            .off('change', 'tbody tr :Input:not(button):not(thead)')
            .on('change', 'tbody tr :Input:not(button):not(thead)', function () {
                //console.log('Table row updated');
                if (!$(this).parent().hasClass('bs-searchbox')) {
                    var table = $(this).closest('table');
                    var row = $(this).closest('tr');
                    var data = $(row).SerializeTableRow();
                    var url = $(table).data('urlupdate');
                    var rowKey = $(row).data('entitykey');

                    var panel = $(table).closest('.panel');
                    if (url != undefined) {
                        var linkedPanels = $(panel).data('reloadpanels');
                        if (linkedPanels) {
                            var urlArr = linkedPanels.split(',');
                            $.each(urlArr, function (index, value) {
                                var linkedPanel = $('#' + value);
                                var targetBody = $(linkedPanel).find('.panel-body');
                                //$(targetBody).empty();
                                var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';
                                $(linkedPanel).addClass('panel-loading');
                                $(targetBody).append(spinnerHtml);
                            });
                        }
                        console.log('Table row updated', $(this), url, data);
                        $.ajax({
                            type: "Post",
                            dataType: "json",
                            url: url,
                            async: true,
                            data: data,
                            success: function (res) {
                                if (rowKey) {
                                    $.UpdateElementValuesByKey(formKey, res.model);
                                    if (!panel) {
                                        panel = table;
                                    }
                                    $(panel).find('tr[data-entitykey="' + rowKey + '"]').each(function () {
                                        //$(this).UpdateElementValues(res.model);
                                        if ($(this).data("isnewrow") !== "true") {
                                            SetValidationErrorForObject(this, res.errorModel);
                                        }
                                    });
                                }
                                else {
                                    $(this).UpdateElementValues(res.model);
                                    if ($(this).data("isnewrow") !== "true") {
                                        SetValidationErrorForObject(row, res.errorModel);
                                    }
                                }
                                if (linkedPanels) {
                                    var urlArr = linkedPanels.split(',');
                                    $.each(urlArr, function (index, value) {
                                        var linkedPanel = $('#' + value);
                                        $(linkedPanel).removeClass('panel-loading');
                                        $(linkedPanel).find('.panel-loader').remove();
                                    });
                                }
                                var focused = $(':focus');
                                focused.select();
                                ReloadLinkedPanel(table);

                            },
                            error: function (res) {
                                swal.fire({
                                    title: 'Error',
                                    html: res.responseText,
                                    icon: 'error',
                                    buttons: {
                                        confirm: {
                                            text: 'Close',
                                            value: true,
                                            visible: true,
                                            className: 'btn btn-success',
                                            closeModal: true
                                        }
                                    }
                                });
                            }
                        });
                    }
                }
            });

        $(tableElem)
            .off('click', 'button.delRow')
            .on('click', 'button.delRow', function (e) {
                e.stopPropagation();
                var button = this;
                var table = $(this).closest('table');
                var tableid = $(table).attr('id');
                var row = $(this).closest('tr');
                var data = row.data('entitykey');
                var delRowKey = $(this).data('entitykey');

                $(button).SetButtonBusy();
                if (!delRowKey) {
                    delRowKey = data;
                }

                var token = $(row).find('[name="__RequestVerificationToken"]');
                if (token.length == 0) {
                    token = $(row).closest('[name="__RequestVerificationToken"]');
                }
                if (token.length == 0) {
                    token = $(document).find('[name="__RequestVerificationToken"]');
                }
                if (token.length != 0) {
                    data += '&__RequestVerificationToken=' + token.val();
                }
                var url = $(table).data('urldelete');
                console.log('Table row deleted', url, data);


                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    success: function (res) {
                        if (res.success === "true") {
                            $(button).SetButtonNotBusy();
                            $('#' + tableid + ' tbody tr').each(function () {//[data-entitykey~="' + delRowKey + '"]
                                var thisRowKey = $(this).data('entitykey');
                                var iskey = thisRowKey.search(delRowKey);
                                //console.log($(this).data('entitykey'), delRowKey, iskey);
                                if (thisRowKey.indexOf(delRowKey) != -1) {
                                    this.remove();
                                }
                            });

                            var dstId = $(table).data("dstselect");
                            var url = $(table).data("urlselect");
                            if (!dstId || !url) {
                                return;
                            }
                            var dst = $('#' + dstId);
                            dst.html("");
                        }
                        else {
                            console.log(res);
                        }
                    },
                    error: function (res) {
                        $(button).SetButtonNotBusy();
                        console.log(res);
                    }
                });

            });

        $(tableElem)
            .off('click', 'a.delRow')
            .on('click', 'a.delRow', function (e) {
                e.stopPropagation();
                var button = this;
                var table = $(this).closest('table');
                var tableid = $(table).attr('id');
                var row = $(this).closest('tr');
                var data = row.data('entitykey');
                var delRowKey = $(this).data('entitykey');

                $(button).SetButtonBusy();
                if (!delRowKey) {
                    delRowKey = data;
                }

                var token = $(row).find('[name="__RequestVerificationToken"]');
                if (token.length == 0) {
                    token = $(row).closest('[name="__RequestVerificationToken"]');
                }
                if (token.length == 0) {
                    token = $(document).find('[name="__RequestVerificationToken"]');
                }
                if (token.length != 0) {
                    data += '&__RequestVerificationToken=' + token.val();
                }
                var url = $(table).data('urldelete');
                console.log('Table row deleted', url, data);


                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    success: function (res) {
                        if (res.success === "true") {
                            $(button).SetButtonNotBusy();
                            $('#' + tableid + ' tbody tr').each(function () {//[data-entitykey~="' + delRowKey + '"]
                                var thisRowKey = $(this).data('entitykey');
                                var iskey = thisRowKey.search(delRowKey);
                                //console.log($(this).data('entitykey'), delRowKey, iskey);
                                if (thisRowKey.indexOf(delRowKey) != -1) {
                                    this.remove();
                                }
                            });

                            var dstId = $(table).data("dstselect");
                            var url = $(table).data("urlselect");
                            if (!dstId || !url) {
                                return;
                            }
                            var dst = $('#' + dstId);
                            dst.html("");
                        }
                        else {
                            console.log(res);
                        }
                    },
                    error: function (res) {
                        $(button).SetButtonNotBusy();
                        console.log(res);
                    }
                });

            });
        $(tableElem)
            .off('click', 'button.detailBtn')
            .on('click', 'button.detailBtn', function () {
                var table = $(this).closest('table');
                var tableid = $(table).attr('id');
                var row = $(this).closest('tr');
                var data = row.data('entitykey');
                var url = $(table).data('actionurl');
                //console.log('Row Detail Clicked', url, data);

                $.post(url, data, function (res) {
                    $(document).html(res);
                });
            });

        $(tableElem)
            .off('click', 'button.addRow')
            .on('click', 'button.addRow', function () {
                var table = $(this).closest('table');
                var btnAdd = this;
                var addrowlocation = $(btnAdd).data('addrowlocation');
                //console.log('addrowlocation', addrowlocation, btnAdd);
                if (!addrowlocation) {
                    addrowlocation = "bottom";
                }
                var row = $(this).closest('tr');
                var tableid = $(table).attr('id');
                var data = table.data('tablekey');
                var url = $(table).data('urlcreate');
                var tbody = $('#' + tableid + ' tbody');

                var token = $(row).find('[name="__RequestVerificationToken"]');
                if (token.length != 0) {
                    data += '&__RequestVerificationToken=' + token.val();
                }


                //console.log('Table row added', table, tableid, url, data);
                $(btnAdd).SetButtonBusy();
                $.ajax({
                    type: "Get",
                    dataType: "html",
                    url: url,
                    data: data,
                    success: function (data) {
                        //console.log(data);
                        if (!data.includes("Internal Server Error")) {

                            if (addrowlocation == "top") {
                                var firstRow = $(tbody).find('tr:first');
                                if (firstRow.length === 0) {
                                    tbody.append(data);
                                    var newRow = $('#' + tableid + ' tbody tr').last();
                                }
                                else {

                                    $(data).insertBefore(firstRow);
                                    var newRow = $('#' + tableid + ' tbody tr:first');
                                }
                            }
                            else {
                                tbody.append(data);
                                var newRow = $('#' + tableid + ' tbody tr').last();
                            }
                            newRow.click();
                            $(newRow).data("isnewrow", "true");
                            $(btnAdd).prop("disabled", false);
                            $(btnAdd).SetButtonNotBusy();
                            $('html, body').animate({
                                scrollTop: $(newRow).offset().top
                            }, 200);

                        }
                        else {
                            var tr = $("<tr></tr>");
                            var td = $('<td colspan="1">/td>');

                            $(data).appendTo(td);
                            $(td).appendTo(tr);

                            tbody.append(tr);
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //console.log(XMLHttpRequest);
                        $(btnAdd).SetButtonNotBusy();
                    }
                });
            });
        $(tableElem).data("tableinit", "true");
    }
};

var rebuildSearchRow = function (column, setting) {

    if (column.visible()) {
        var colIdx = column[0][0];
        var header = $(column.header()).closest('thead');
        //console.log('rebuildSearchRow', header);
        
        if ($(header).find('[data-datatable-filter-type="select"]').length === 0 &&
            $(header).find('[data-datatable-filter-type="multiselect"]').length === 0) {
            console.log('rebuildSearchRow Noithing found');
            return;
        }
        var filterType = $(column.header()).data('datatable-filter-type');
        var filterDefault = $(column.header()).data('datatable-filter-value');
        var filterId = "table_col_flt_" + setting[0].id + '_' + colIdx;
        console.log('rebuildSearchRow something found', filterType, filterId, filterDefault, colIdx, setting[0].id, setting);
        if (filterType == 'select') {
            var select = $('#' + filterId);
            $(select).html('');

            $(select).selectpicker('destroy');
            var opt = '<option value="" selected ></option>';
            select.append(opt);
            column.data().unique().sort().each(function (d, j) {
                if (d !== null && d != "") {
                    opt = '<option value="' + d + '">' + d + '</option>';
                    if (filterDefault != undefined) {
                        console.log("opt val:", d, "Default Filter: ", filterDefault);
                    }
                    if (filterDefault != undefined && d == filterDefault) {
                        opt = '<option value="' + d + '" selected>' + d + '</option>';
                    }
                    select.append(opt);
                }
            });
            $(select).selectpicker('render');
        }
        else if (filterType == 'multiselect') {
            var select = $('#' + filterId);
            $(select).html('');
            var opt = '';
            $(select).selectpicker('destory');
            select.append(opt);
            column.data().unique().sort().each(function (d, j) {
                if (d !== null) {
                    opt = '<option value="' + d + '">' + d + '</option>';
                    if (filterDefault != undefined) {
                        var defaults = filterDefault.split('|');
                        $.each(defaults, function (index, value) {
                            if (value === d) {
                                opt = '<option value="' + d + '" selected>' + d + '</option>';
                            }
                        });
                    }
                    select.append(opt);
                }
            });

            $(select).selectpicker('render');
        }
        column.draw();
    }
}

var addSearchRow = function (column, setting) {
    if (column.visible()) {
        var colIdx = column[0][0];
        var header = $(column.header()).closest('thead');
        var myPanel = $(header).closest('.dataTables_wrapper');

//        console.log(header);
        //If no filters defined skip build
        if ($(header).find('[data-datatable-filter-type="select"]').length === 0 &&
            $(header).find('[data-datatable-filter-type="multiselect"]').length === 0 &&
            $(header).find('[data-datatable-filter-type="daterange"]').length === 0 &&
            $(header).find('[data-datatable-filter-type="input"]').length === 0 &&
            $(header).find('[data-datatable-filter-type="checkbox"]').length === 0) {
            return;
        }
        //find/create searchrow
        var searchRow = $(header).find('tr:eq(1)');
        if (searchRow.length == 0) {           
            searchRow = $('<tr></tr>').appendTo(header);
        }

        //finds/add col search header
        var cell = $(searchRow).find('th:eq(' + colIdx + ')');
        if (cell.length == 0) {
            cell = $('<th style="padding-right: 0px;"></th>').appendTo(searchRow);
        }
        $(cell).empty();

        var model = $(header).closest('.modal');
        var isInModel = $(header).closest('.modal').length !== 0;

        var filterType = $(column.header()).data('datatable-filter-type');
        var filterDefault = $(column.header()).data('datatable-filter-value');
        var filterId = "table_col_flt_" + setting[0].id + '_' + colIdx;

        if (filterType == 'select') {
            var dataCnt = column.data().unique().length;
            var dataContainer = isInModel ? '#' + $(model).attr('id') : '#' + $(myPanel).attr('id');
            if (dataCnt > 10) {
                var select = $('<select id="' + filterId + '" class="selectpicker form-control input-xs" data-container="' + dataContainer + '" data-none-selected-text="Search" data-live-search="true" data-style="btn-xs btn-white"></select>');
            }
            else { 
                var select = $('<select id="' + filterId + '"  class="selectpicker form-control input-xs" data-container="' + dataContainer + '" data-none-selected-text="Search" data-style="btn-xs btn-white"></select>');
            }
            var opt = '<option value="" selected ></option>';
            //console.log(column.data());
            select.append(opt);
            column.data().unique().sort().each(function (d, j) {
                if (d !== null && d != "") {
                    opt = '<option value="' + d + '">' + d + '</option>';
                    if (filterDefault != undefined ) {
                        if (d.trim() == filterDefault) {
                            opt = '<option value="' + d + '" selected>' + d + '</option>';
                        }
                    }
                    select.append(opt);
                }
            });
            $(select).appendTo($(cell))
                .on('changed.bs.select', function (e) {
                    var val = $(this).val();
                    column.search("" + val, true, false).draw();
                });
            $(select).selectpicker('render');
        }
        else if (filterType == 'multiselect') {
            var dataCnt = column.data().unique().length;
            var dataContainer = isInModel ? '#' + $(model).attr('id') : 'body';
            if (dataCnt > 10) {
                var select = $('<select id="' + filterId + '"  class="selectpicker form-control input-xs" data-container="' + dataContainer + '" data-none-selected-text="Search" data-live-search="true" data-style="btn-xs btn-white" multiple></select>');
            }
            else {
                var select = $('<select id="' + filterId + '"  class="selectpicker form-control input-xs" data-container="' + dataContainer + '" data-none-selected-text="Search" data-style="btn-xs btn-white" multiple></select>');
            }

            var opt = '';//'<option value="All" selected >Show All</option>';
            select.append(opt);
            var searchArry = [];
            if (searchArry.length > 0) {
                var unique = searchArry.filter((item, i, ar) => ar.indexOf(item) === i);
                unique = unique.sort();
                console.log(searchArry, unique);
                unique.forEach(function (item) {
                    opt = '<option value="' + item + '">' + item + '</option>';
                    if (filterDefault != undefined) {
                        var defaults = filterDefault.split('|');
                        $.each(defaults, function (index, value) {
                            if (value.trim() === item.trim()) {
                                opt = '<option value="' + item + '" selected>' + item + '</option>';
                            }
                        });
                    }
                    select.append(opt);
                });
            }
            else {
                column.data().unique().sort().each(function (d, j) {
                    if (d !== null) {
                        opt = '<option value="' + d + '">' + d + '</option>';
                        if (filterDefault != undefined) {
                            var defaults = filterDefault.split('|');
                            $.each(defaults, function (index, value) {
                                if (value === d) {
                                    opt = '<option value="' + d + '" selected>' + d + '</option>';
                                }
                            });
                        }
                        select.append(opt);
                    }
                });

            }
            

            $(select).appendTo($(cell))
                .on('changed.bs.select', function (e) {
                    var val = $(this).val();
                    var fVal = val.join("|^")
                    column.search(fVal, true, false, true).draw();
                });
            $(select).selectpicker('render');
        }
        else if (filterType == 'input') {
            var select = $('<input id="' + filterId + '" type="text" class="form-control input-xs" placeholder="Search"/>');
            $(select).appendTo($(cell))
                .on('keyup change', function () {
                    var value = this.value;
                    if (column.search() !== value) {
                        column.search("" + value, true, false, true).draw();
                    }
                });
        }
        else if (filterType == 'daterange') {
            var input = $('<input type="text" id="' + filterId + '" class="form-control input-xs" value="" placeholder="click to select the date range" />');            
            $(input).appendTo($(cell))
                .on('change', function () {
                    column.draw();
                });

            $('#' + filterId).daterangepicker({
                linkedCalendars: false,
                autoUpdateInput: false,
                format: 'MM/DD/YYYY',
                startDate: moment().subtract(30, 'days'),
                endDate: moment().add(10, 'days'),
                minDate: '01/01/2000',
                maxDate: moment().add(365, 'days'),
                dateLimit: { days: 365 },
                showDropdowns: true,
                showWeekNumbers: true,
                timePicker: false,
                timePickerIncrement: 1,
                timePicker12Hour: true,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')],
                    'Last Year': [moment().subtract(365, 'days'), moment()],
                },
                opens: 'left',
                drops: 'down',
                buttonClasses: ['btn', 'btn-sm'],
                applyClass: 'btn-primary',
                cancelClass: 'btn-warning',
                separator: ' to ',
                locale: {
                    applyLabel: 'Submit',
                    cancelLabel: 'Clear',
                    fromLabel: 'From',
                    toLabel: 'To',
                    customRangeLabel: 'Custom',
                    daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                    monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                    firstDay: 1
                },
            }, function (start, end, label) {
                    $('#' + filterId + ' span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
            })
                .on('apply.daterangepicker', function (ev, picker) {
                $(this).val(picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY'));
                $(this).change();
            }).on('cancel.daterangepicker', function (ev, picker) {
                $(this).val('');
                $(this).change();
            });
            //console.log('Datatable Search Added:', setting[0].id);
            $.fn.dataTable.ext.search.push(
                function (settings, data, dataIndex) {
                    if (settings.nTable.id == setting[0].id) {
                        var fltVal = $('#' + filterId).val();
                        if (fltVal !== '') {
                            var val = fltVal.split("-", 2);
                            var min = new Date(val[0]);
                            var max = new Date(val[1]);
                            var date = new Date(data[colIdx]);
                            if ((min === null && max === null) ||
                                (min === null && date <= max) ||
                                (min <= date && max === null) ||
                                (min <= date && date <= max)) {
                                return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
            );
        }
        else if (filterType == 'checkbox') {
            var select = $('<input id="' + filterId + '" type="checkbox" class="form-control input-xs" />');
            $(select).prop('indeterminate', true)
            $(select).data('checked', 1);
            $(select).appendTo($(cell))
                .on('click', function () {
                    var el = $(this);
                    switch (el.data('checked')) {
                        case 0:
                            el.data('checked', 1);
                            el.prop('indeterminate', true);
                            var value = '';
                            break;
                        // indeterminate, going checked
                        case 1:
                            el.data('checked', 2);
                            el.prop('indeterminate', false);
                            el.prop('checked', true);
                            var value = true;
                            break;
                        // checked, going unchecked
                        default:
                            el.data('checked', 0);
                            el.prop('indeterminate', false);
                            el.prop('checked', false);
                            var value = false;
                    }
                    //console.log('Checkbox Value:', value);
                    if (column.search() !== value) {
                        column.search("^" + value, true, false, true).draw();
                    }
                });
        }

        if (filterDefault != undefined) {
            //console.log('filterDefault:', filterDefault);
            column.search("^" + filterDefault, true, false, true).draw();
        } 
        column.draw();
    }
}

var DataTablesMoveSearch = function (tableElem, toElem) {
    
    var table = $(tableElem).closest('.dataTables_wrapper');
    if ($(table).data("filterMoved") == "true") {
        return;
    }
    var filter = $(table).find('.dataTables_filter');
    //console.log($(filter));
    var dst = toElem;
    if ($(dst).find('form').length !== 0) {
        var form = $(dst).find('form');
        var formGrp = $(form).find('.form-group');
        if (formGrp.length !== 0)
            toElem = formGrp;
    }
    $(table).data("filterMoved", "true");
    var inputSeach = $(filter).find('label').find('input');

    var myDiv = $('<div class="col-md-4 input-group"></div>')
    var inputLabel = $('<label class="col-md-2 col-form-label input-group">Search</label>');
    $(myDiv).append(inputSeach)
    $(inputLabel).appendTo($(toElem))
    $(myDiv).appendTo($(toElem));

    $(filter).find('label').remove();
} 

function ReloadPanel(elem) {
    $(elem).ReloadPanel();

    //if (level == undefined)
    //    level = 0;

    //var target = $(elem).closest('.panel');
    //var reloadBtn = $(target).find('[data-click="panel-reload"]');
    //if (reloadBtn.length === 0 && level < 5) {
    //    level++;
    //    console.log('Reload Panel Level:', level, $(target).parent());
    //    ReloadPanel($(target).parent(), level++);
    //}
    //else {
    //    $(reloadBtn).click();
    //}

}

function ReloadTable(tableElem, data) {
    var target = $(tableElem).closest('.panel');
    if (!$(target).hasClass('panel-loading')) {
        var targetBody = $(target).find('.panel-body');
        var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';
        var targetReloadUrl = $(target).data('reloadurl');
        var targetInitPaneFunc = $(target).data('initpane');

        $(target).addClass('panel-loading');
        $(targetBody).append(spinnerHtml);
        if (targetReloadUrl !== '' && targetReloadUrl !== undefined) {
            if (data != null) {
                var url = targetReloadUrl.split("?");
                targetReloadUrl = url[0];

                var url = $(target).data('reloadurl');
                var urlRoute = "";
                $.each(data, function (key, value) {
                    if (urlRoute !== "") {
                        urlRoute += "&"
                    }
                    urlRoute += key + "=" + value;
                });
                var newURL = targetReloadUrl + "?" + urlRoute;
                //console.log(newURL);
                $(target).attr('data-reloadurl', newURL);
            }

            //console.log(targetReloadUrl, data);
            setTimeout(function () {
                $.ajax({
                    type: "Get",
                    dataType: "html",
                    url: targetReloadUrl,
                    data: data,
                    success: function (data) {
                        $(target).removeClass('panel-loading');
                        $(target).find('.panel-loader').remove();
                        $(targetBody).empty();
                        $(targetBody).append(data);
                        if (targetInitPaneFunc !== '' && targetInitPaneFunc !== undefined) {
                            //console.log('targetInitPaneFunc: ', targetInitPaneFunc);
                            window[targetInitPaneFunc]();
                        }
                        initPane(targetBody);
                    }
                });
            }, 200);
        }
        else {
            setTimeout(function () {
                $(target).removeClass('panel-loading');
                $(target).find('.panel-loader').remove();
            }, 2000);
        }
    }
}

/* 03. Handle Form
-----------------------------------*/
jQuery.fn.extend({
    SerializeJson: function () {
        var disabled = $(this).find(':input:disabled');
        disabled.removeAttr('disabled');
        var formData = $(this).serialize().split("&");
        disabled.attr('disabled', 'disabled');
        //console.log('formData', formData);
        var data = {};
        for (var key in formData) {
            var keyname = formData[key].split("=")[0];
            var keyVal = decodeURIComponent(formData[key].split("=")[1]);
            var regExp = /[a-zA-Z]/g;

            if (!regExp.test(keyVal) && $(this).attr("data-type") == "number") {
                var number = keyVal.replace(/[^0-9.-]+/g, "");
                number = number.replace(/,/g, '');
                if (keyVal.indexOf("%") >= 0) {
                    keyVal = parseFloat(val) / 100.0;
                }
                else if (keyVal.indexOf("$") >= 0) {
                    var isNegative = keyVal.indexOf("(") >= 0 || keyVal.indexOf("-") >= 0;
                    var floatVal = parseFloat(number);
                    keyVal = floatVal * (isNegative ? -1.0 : 1.0);
                }
                else if (!isNaN(parseFloat(number))) {
                    var floatVal = parseFloat(number);
                    keyVal = floatVal
                }
            }

            if (keyVal !== null && keyVal !== "null") {
                data[keyname] = keyVal;
            }
            //console.log(keyname, keyVal, $(this).attr("data-type"));
            //data[keyname] = keyVal;
        }
        //console.log('data', data);
        return data;
    },

    GetFormData: function (textValue) {
        if (textValue === undefined) {
            textValue = false;
        }
        var trValues = "";
        $(this).find(":input[name]:not(button)").each(function () {
            var elemId = $(this).attr('name');
            if (!elemId) {
                var parentElem = $(this).parent();
                if (!$(parentElem).hasClass('bs-searchbox')) {
                    //console.log('Control has no name Attr:', $(this));
                }
            }
            if (elemId) {
                if (trValues !== '') {
                    trValues += '&';
                }
                switch ($(this).attr('type')) {
                    case "checkbox":
                         //console.log(elemId, ' Checkbox Val:', $(this).is(':checked'), encodeURIComponent($(this).is(':checked')));
                        //elemId = elemId.replace(/_([0-9][0-9]{0,3}|10000)/g, '');
                        trValues += elemId + '=' + encodeURIComponent($(this).is(':checked'));
                        break;
                    default:
                        var val = $(this).val();
                        if ($.isArray(val)) {
                            val = val.join("|");
                        }
                        if ($(this).is('select')) {
                            if (textValue) {
                                val = $(this).find('option:selected').val();
                                if (val != "") {
                                    val = $(this).find('option:selected').text();
                                }
                            }
                        }

                        if (val !== null && val !== undefined) {

                            if (val.indexOf("%") >= 0) {
                                val = parseFloat(val) / 100.0;
                            }
                            else if (val.indexOf("$") >= 0) {
                                var isNegative = val.indexOf("(") >= 0 || val.indexOf("-") >= 0;
                                val = Number(val.replace(/[^0-9.-]+/g, "")) * (isNegative ? -1.0 : 1.0);
                            }
                            //val = val.replace(" %", "");

                            //if (val.indexOf("$ ") >= 0) {
                            //    val = val.replace("$ ", "");
                            //    val = val.replace(",", "");
                            //}
                            //if (val.indexOf("$") >= 0) {
                            //    val = val.replace("$", "");
                            //    val = val.replace(",", "");
                            //}
                        }
                        //console.log('Elem :', elemId, ', Val: ', val);
                        trValues += elemId + '=' + encodeURIComponent(val);
                        break;
                }
            }
        });
        //console.log(trValues);

        var formValues = trValues.split("&");
        //console.log(formData);
        var data = new FormData();
        for (var key in formValues) {
            var keyname = formValues[key].split("=")[0];
            var keyVal = decodeURIComponent(formValues[key].split("=")[1]);
            if (keyVal !== null && keyVal !== "null") {
                //data[keyname] = keyVal;
                data.append(keyname, keyVal);
            }
        }
        return data;
        //return trValues;
    },

    SerializeForm: function (textValue) {
        if (textValue === undefined) {
            textValue = false;
        }
        var trValues = "";
        $(this).find(":input[name]:not(button)").each(function () {
            var elemId = $(this).attr('name');
            if (!elemId) {
                var parentElem = $(this).parent();
                if (!$(parentElem).hasClass('bs-searchbox')) {
                    //console.log('Control has no name Attr:', $(this));
                }
            }
            if (elemId) {
                if (trValues !== '') {
                    trValues += '&';
                }
                switch ($(this).attr('type')) {
                    case "checkbox":
                         //console.log(elemId, ' Checkbox Val:', $(this).is(':checked'), encodeURIComponent($(this).is(':checked')));
                        //elemId = elemId.replace(/_([0-9][0-9]{0,3}|10000)/g, '');
                        trValues += elemId + '=' + encodeURIComponent($(this).is(':checked'));
                        break;
                    default:

                        var val = $(this).val();
                        if ($.isArray(val)) {
                            val = val.join("|");
                        }
                        if ($(this).is('select')) {
                            if (val == null) {
                                val = $(this).find('option:selected, option[disabled]:selected').val();
                            }
                            if (textValue) {
                                val = $(this).find('option:selected, option[disabled]:selected').val();

                                if (val != "") {
                                    val = $(this).find('option:selected, option[disabled]:selected').text();
                                }
                            }
                        }

                        if (val !== null && val !== undefined) {
                            var regExp = /[a-zA-Z]/g;
                            if (!regExp.test(val)) {
                                if ($(this).attr("data-type") == "number") {
                                    var number = val.replace(/[^0-9.-]+/g, "");
                                    number = number.replace(/,/g, '');
                                    if (val.indexOf("%") >= 0) {
                                        val = parseFloat(val) / 100.0;
                                    }
                                    else if (val.indexOf("$") >= 0) {
                                        var isNegative = val.indexOf("(") >= 0 || val.indexOf("-") >= 0;
                                        var floatVal = parseFloat(number);
                                        val = floatVal * (isNegative ? -1.0 : 1.0);
                                    }
                                    else if (!isNaN(parseFloat(number))) {
                                        var floatVal = parseFloat(number);
                                        val = floatVal
                                    }

                                    //console.log(elemId, val, $(this).val());
                                }
                            }
                        }
                        //console.log('Elem :', elemId, ', Val: ', val);
                        trValues += elemId + '=' + encodeURIComponent(val);
                        break;
                }
            }
        });
        //console.log(trValues);

        var formValues = trValues.split("&");
        //console.log(formData);
        var data = {};
        for (var key in formValues) {
            var keyname = formValues[key].split("=")[0];
            var keyVal = decodeURIComponent(formValues[key].split("=")[1]);
            if (keyVal !== null && keyVal !== "null") {
                data[keyname] = keyVal;
                //data.append(keyname, keyVal);
            }
        }
        return data;
        //return trValues;
    }
});

var handleFormAutoUpdate = function (formElem) { 

    var isBinding = $(formElem).data("forminit") === "true";
    if (!isBinding) {
        console.log('handleFormAutoUpdate: ', formElem);
        var idx = getMaxTabIndex();
        $(formElem).find(':input').each(function () {
            $(this).attr('tabindex', idx);
            idx++;
        });

        var id = $(formElem).attr('id');
        if ($(formElem).find('[name="EntityName"]')) {
            var panel = $(formElem).closest('.panel');
            var histBtn = $(panel).find('[data-click="panel-history"]');
            var auditData = {
                EntityName: $(formElem).find('[name="EntityName"]').val(),
                EntityKeyString: $(formElem).find('[name="EntityKeyString"]').val()
            };
            if (auditData.EntityName != undefined) {
                //console.log('Log info Found for Form', auditData, histBtn);
                $(histBtn).data('auditentityname', auditData.EntityName);
                $(histBtn).data('auditentitykeystring', auditData.EntityKeyString);
                $(histBtn).show();

            }
        }
        $(formElem)
            .off('change', ':Input:not(button)')
            .on('change', ':Input:not(button)', function () {
                var focused = currentFocus;
                //console.log('Input changed', $(focused));
                //console.log($(elem).attr("tabindex"));
                var elem = this;

                if (!$(this).parent().hasClass('bs-searchbox')) {
                    var form = $(this).closest('form');
                    var data = $(form).SerializeForm();
                    var url = $(form).data('urlupdate');
                    var postUpdateFunc = $(form).data('postupdate');
                    var formKey = $(form).data('entitykey');
                    var relatedPanel = $(form).data('relatedpanel');
                    if (url == undefined) {
                        if (postUpdateFunc !== '' && postUpdateFunc !== undefined) {
                            //console.log('Post Update Function Found!');
                            window[postUpdateFunc](elem);
                        }
                        return;
                    }

                    //console.log('Form updated', $(this), url, data);
                    $(form).data('fieldupdating', "true");
                    lockFieldSet(form);
                    $.post(url, data, function (res) {
                        $.UpdateElementValuesByKey(formKey, res.model);
                        //$(form).UpdateElementValues(res.model);
                        SetValidationErrorForObject(form, res.errorModel);
                        //if (formKey) {
                        //    var model = $(form).SerializeForm(true);
                        //    var panel = $('#' + relatedPanel);
                        //    var keyMatch = $(panel).find('tr[data-entitykey="' + formKey + '"]');
                        //    keyMatch.each(function () {
                        //        //console.log("Related Panel", this);
                        //        updateElementValues(this, model);
                        //        $(this).UpdateElementValues(model);
                        //    });
                        //}
                        unLockFieldSet(form);
                        if (postUpdateFunc !== '' && postUpdateFunc !== undefined) {
                            //console.log('Post Update Function Found!');
                            window[postUpdateFunc](elem);
                        }
                        ReloadLinkedPanel(form, currentFocus);
                        $(form).data('fieldupdating', "false");

                    });
                }
            });

        $(formElem)
            .off('focus', ':Input')
            .on('focus', ':Input', function () {
                var newFocus = $(':focus');
                if (newFocus) {
                    if (newFocus != currentFocus) {
                        currentFocus = $(':focus');
                    }
                }
            })

        $(formElem)
            .off('click', 'button.addRow')
            .on('click', 'button.addRow', function (btnAdd) {
                var formList = $(this).closest('.formList');
                var formid = $(formList).attr('id');
                var data = $('#' + formid).data('entitykey');
                var url = $(formList).data('urlcreate');
                //console.log('Table row added', table, tableid, url, data);
                $(btnAdd).prop("disabled", true);
                $.ajax({
                    type: "Get",
                    dataType: "html",
                    url: url,
                    data: data,
                    success: function (data) {
                        formList.append(data);
                        $(btnAdd).prop("disabled", false);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //console.log(XMLHttpRequest);
                    }
                });
            });

        $(formElem)
            .off('click', 'button.delRow')
            .on('click', 'button.delRow', function (delBtn) {
                var formList = $(this).closest('.formList');
                var formid = $(formList).attr('id');
                var form = $(this).closest('.formRow');
                var data = $(form).data('formKey');
                var delRowKey = data;
                var token = $(form).find('[name="__RequestVerificationToken"]');
                if (token.length != 0) {
                    data += '&__RequestVerificationToken=' + token.val();
                }
                var url = $(formList).data('urldelete');
                //console.log('Form row deleted', url, data);

                $(delBtn).hide();
                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    success: function (res) {
                        if (res.success === "true") {
                            $('#' + formid + '.formRow').each(function () {//[data-entitykey~="' + delRowKey + '"]
                                var thisRowKey = $(this).data('formKey');
                                if (thisRowKey.indexOf(delRowKey) != -1) {
                                    this.remove();
                                }
                            });
                        }
                    },
                    error: function (res) {
                        //console.log(res);
                        $(delBtn).show();
                    }
                });

            });


        $(formElem).data("forminit", "true");
    }
};

function lockFieldSet(elem) {
    $(elem).find(':Input:not(button)').each(function () {
        var parentElem = $(this).parent();
        if (!$(parentElem).hasClass('bootstrap-select')) {
            var isDisabled = $(this).prop('disabled');
            $(this).data('isdisabled', $(this).prop('disabled'));
            $(this).prop('disabled', true);
            if (!isDisabled) {
                $(this).css('background', 'white');
                $(this).data('isfocused', $(this).is(':focus'));
            }
        }
    });
}

function unLockFieldSet(elem) {
    $(elem).find(':Input:not(button)').each(function () {
        var parentElem = $(this).parent();
        if (!$(parentElem).hasClass('bootstrap-select')) {
            var isDisabled = $(this).data('isdisabled');
            $(this).prop('disabled', isDisabled);
            if (!isDisabled) {
                $(this).css('background', '');
            }
        }
    });
    var tabIdx = $(currentFocus).attr('tabindex');
    tabIdx++;
    $(elem).find('[tabindex=' + tabIdx + ']').focus();
}

/* 04. Handle Switch
-----------------------------------*/

//var renderAllSwitcher = function () {
//    if ($('[data-render=switchery]').length !== 0) {
//        $('[data-render=switchery]').each(function () {
//            renderSwitcheryElem(this);
//        });
//    }
//}; 

//function renderSwitcheryElem(elem) {
//    var themeColor = COLOR_GREEN;
//    //console.log('Switch theme: ', $(elem).attr('data-theme'));
//    if ($(elem).attr('data-theme')) {
//        switch ($(elem).attr('data-theme')) {
//            case 'red': themeColor = COLOR_RED; break;
//            case 'blue': themeColor = COLOR_BLUE; break;
//            case 'purple': themeColor = COLOR_PURPLE; break;
//            case 'orange': themeColor = COLOR_ORANGE; break;
//            case 'black': themeColor = COLOR_BLACK; break;
//        }
//    }
//    var option = {};
//    option.color = themeColor;
//    option.secondaryColor = ($(elem).attr('data-secondary-color')) ? $(elem).attr('data-secondary-color') : '#dfdfdf';
//    option.className = ($(elem).attr('data-classname')) ? $(elem).attr('data-classname') : 'switchery';
//    option.disabled = $(elem).attr('data-disabled') === "true";
//    option.size = ($(elem).attr('data-size')) ? $(elem).attr('data-size') : 'default';
//    option.disabledOpacity = ($(elem).attr('data-disabled-opacity')) ? parseFloat($(elem).attr('data-disabled-opacity')) : 0.5;
//    option.speed = ($(elem).attr('data-speed')) ? $(elem).attr('data-speed') : '0.5s';
//    var switchery = new Switchery(elem.get(0), option);
    

//    return switchery;
//};

/* 05. Handle Validate
-----------------------------------*/
if (typeof jQuery.when.all === 'undefined') {
    jQuery.when.all = function (deferreds) {
        return $.Deferred(function (def) {
            $.when.apply(jQuery, deferreds).then(
                function () {
                    def.resolveWith(this, [Array.prototype.slice.call(arguments)]);
                },
                function () {
                    def.rejectWith(this, [Array.prototype.slice.call(arguments)]);
                });
        });
    }
}

jQuery.fn.extend({
    Validate: function (callback) {
        var threads = [];
        var mainElem = this;
        $(this).find('.is-invalid').each(function () { $(this).removeClass('is-invalid'); });
        $(this).find('.validation-summary-valid').each(function () { $(this).html(''); });

        //if ($(mainElem).is('form'))
        //    validateForm(mainElem, threads);
        //else if ($(mainElem).is('table'))
        //    validateTable(mainElem, threads);
        $(mainElem).find('form, table').each(function () {
            var elem = this;
            if ($(elem).is('form'))
                validateForm(elem, threads);
            else if ($(elem).is('table'))
                validateTable(elem, threads);
        }).promise().done(function () {
            $.when.all(threads).then(function (objects) {
                var errorElem = $(mainElem).find('.is-invalid:visible').first()[0];
                if (errorElem === undefined) {
                    errorElem = $(mainElem).find('.is-invalid').first()[0];
                    if (errorElem !== undefined) {
                        $(errorElem).show();
                    }
                }
                if (errorElem !== undefined) {
                    $('html, body').animate({
                        scrollTop: $(errorElem).offset().top - 150
                    }, 500);
                }
                callback(errorElem);
            });
        });
    },
    PostValidate: function (button, valid) {
        
        $(button).SetButtonBusy();
        //console.log('PostValidate');
        $(document).find('.validation-summary-valid').html('');
        $(this).Validate(function (errorElem) {
            var isValid = errorElem === undefined;
            var validatationmsg = $(document).find('.validation-summary-valid').html();
            if ($(validatationmsg).length > 0 && isValid == true) {
                isValid = false;
            }
            $(button).SetButtonNotBusy();
            valid(isValid);
        });
    }
});

function validateForm(form, threads) {
    if ($(form).data('urlvalidate') != undefined) {
        var url = $(form).data('urlvalidate');
        var data = $(form).SerializeForm();
        //console.log("Form Validate: ", url, data);
        serverValidate(form, url, data, threads);
    }
}

function validateTable(table, threads) {
    var url = $(table).data('urlvalidate');
    if (url != undefined) {
        if ($(table).find('tbody>tr').length > 0) {
            $(table).find('tbody>tr').each(function () {
                //if ($(this).visible()) {
                    var tblRow = this;
                    var data = $(tblRow).SerializeTableRow();
                    //console.log('Table Row Validate:', url, data);
                    serverValidate(tblRow, url, data, threads);

                //}
            });
        }
    }
}

function serverValidate(elem, url, data, threads) {
    threads.push(
        $.ajax({
            type: "get",
            dataType: "json",
            url: url,
            data: data,
            success: function (success) {
                if (success.success === 'false') {
                    console.log('serverValidate:', success.success, url, data);
                    SetValidationErrorForObject(elem, success.errorModel);
                }
            },
            error: function (error) {
                console.log('ERROR VALIDATE', url, data);
            }

        })
    );
}

function SetValidationErrorForObject(elem, errorModel) {
    $(elem).find('.is-invalid').each(function () { $(this).removeClass('is-invalid'); });
    $(elem).closest('.validationContaner').find('.validation-summary-valid').each(function () { $(this).html(''); });

    //Find a container for the error display
    var elemContainer = $(elem).closest('.validationContaner');
    if ($(elemContainer).length == 0) {
        elemContainer = $(elem).closest('.panel');
        if ($(elemContainer).length == 0) {
            elemContainer = $(elem).closest('.card');
            if ($(elemContainer).length == 0) {
                elemContainer = $(elem).closest('.form');
                if ($(elemContainer).length == 0) {
                    elemContainer = $(elem).closest('body');
                }
            }
        }
    }

    var elemvalidationSummary = $(elemContainer).find('.validation-summary-valid');
    
    $.each(errorModel, function (key, value) {
        var isElementVisable = true;
        var elementFound = false;
        var isTable = $(elem).closest('table').length != 0;
        if (isTable) {
            var table = $(elem).closest('table');
            $(table).data('data-dstselectvalidate', "true");
        }

        var fieldId = value.key;
        //Correct for name with periods in it
        while (fieldId.indexOf(".") > 0) {
            var fieldDot = fieldId.indexOf(".");
            if (fieldDot > 0) {
                fieldId = fieldId.substring(fieldDot + 1);
            }
        }
        //Find the Field Element
        var fieldElem = $(elem).find("[name='" + fieldId + "']");
        if (fieldElem === undefined || fieldElem.length == 0) {
            fieldElem = $(elem).find('input[name="' + fieldId + '"]');
        }
        if (fieldElem.length != 0) {           
            isElementVisable = isElementVisable && $(fieldElem).is(":hidden") == false;
            if ($(fieldElem).is(":hidden") == false) {
                elementFound = true;
                $(elem).find('[data-valmsg-for=' + fieldId + ']').html(value.errors[0]);
                if ($(fieldElem).is('select')) {
                    if ($(fieldElem).parent('.bootstrap-select').length !== 0) {
                        $(fieldElem).addClass('is-invalid').selectpicker('setStyle', 'is-invalid', 'add');
                    }
                }
                fieldElem.addClass('is-invalid');
            }
        }

        if (elementFound == false && isTable == false) {
            if ($(elemvalidationSummary).length == 0) {
                elemvalidationSummary = $('<div class="col-xl-8 "><div class="validation-summary-valid text-danger" data-valmsg-summary="true"></div></div>');
                $(elemvalidationSummary).prependTo($(elemContainer));
                elemvalidationSummary = $(elemContainer).find('.validation-summary-valid');
            }
            var html = "<div class='note note-danger'>";
            html += "    <div class='note-icon'><i class='far fa-exclamation-triangle'></i></div>";
            html += "    <div class='note-content'>";
            html += "        <h5>Errors Please Review</h5>";
            html += "        <ul>";
            for (var i = 0; i < value.errors.length; i++) {
                html += "            <li>";
                html += value.errors[i];
                html += "            </li>";
            }
            elemvalidationSummary.find('ul').each(function () {
                html += $(this).html();
            });
            html += "        </ul>";
            html += "    </div>";
            html += "</div>";

            $(elemvalidationSummary).html(html);
            if (!isElementVisable) {
                $(elemvalidationSummary).show();
            }
        }
        else if (elementFound == false && isTable == true) {
            var row = $(elem).closest('tr');
            var firstcell = $(row).find('td');
            var errorText = $('<span class="validation-summary-valid invalid-feedback">' + value.errors[0] + '</span>');
            $(errorText).appendTo($(firstcell));
        }

    });
}

/* 06. Handle Panes
-----------------------------------*/
var initPane = function (panelBody) {
    //console.log("Init Pane", $(panelBody));
    $(panelBody).find('table').each(function () {
        Table().init(this);
    });
    $(panelBody).find('form').each(function () {
        Form().init(this);
    });
}

var initPanels = function () {
    $(document).find('.panel-body').each(function () {
        initPane(this);
    });
} 
var currentFocus;

function ReloadLinkedPanel(elem) {
    var panel = $(elem).closest('.panel');
    var linkedPanels = $(panel).data('reloadpanels');
    if (linkedPanels) {
        //console.log('reload panel current focus is:', currentFocus);
        var urlArr = linkedPanels.split(',');
        $.each(urlArr, function (index, value) {
            var linkedPanel = $('#' + value);
            var reload = $(linkedPanel).find('[data-click=panel-reload]');
            $(reload).click();
            reloadCombos($(linkedPanel));
            ReloadLinkedPanel(linkedPanel);
        }); 
    }

    return;
}

function OpenPopUpWindow(url, data, returnmodel = false, modelElem = undefined) {
    var randomnumber = Math.floor((Math.random() * 100) + 1);

    if (returnmodel) {
        data += "&isModel=true";
        if (modelElem == undefined) {
            modelElem = "auditLog";
        }
        var model = $('#' + modelElem);
        var dst = $(model).find('.modal-body');
        dst.html("");
        //console.log(url, data, model);
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            success: function (result) {
                dst.html(result);
                $(model).modal('show');
                handleComboButtonCreate(model);
            },
            error: function (result) {
                //console.log(data);
                dst.html(result);
                alert('error loading url');
            }
        });
    }
    else {
        if (data !== "") {
            url += "?";
            url += data;

        }
        window.open(url, '_blank', 'PopUp' + randomnumber + ',toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=1200,height=600');
    }
    
}

/* 07. Handle Combo Buttons
-----------------------------------*/
var handleComboButtonInfo = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonInfo");
    return;

    $(elem).on('click', 'button.btnInfo ', function () {
        console.log('button.btnInfo clicked');
        var elem = $(this);
        var parentElem = elem.closest('form');
        if (parentElem.length === 0) {
            parentElem = elem.closest('tr');
        }
        var url = $(this).data('urlinfo');
        var fkeys = $(this).data('fkeys').replace(/\s/g, "");
        var data = "";
        if (fkeys !== "") {
            data = buildParmsFromKeys(parentElem, fkeys, url);
        }
        if (url !== undefined) {
            OpenPopUpWindow(url, data);
        }
    });
};
var handleComboButtonAdd = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonAdd");
    return;

    $(elem).on('click', 'button.btnComboAdd ', function () {
        var elem = $(this);
        var formElem = elem.closest('form');
        var rowElem = elem.closest('tr');
        var parentElem;
        if (rowElem.length !== 0) {
            parentElem = rowElem;
        }
        else if (formElem.length !== 0) {
            parentElem = formElem;
        }

        var id = $(elem).closest('.input-group').attr('id');
        var url = $(this).data('url');
        var title = $(this).data('propertytitle');
        var fkeys = $(this).data('fkeys').replace(/\s/g, "");
        var data = "";
        var model = $('#comboAdd');
        var dst = $(model).find('.modal-body');
        var hdr = $(model).find('.modal-header');
        if (fkeys !== "") {
            data = buildParmsFromKeys(parentElem, fkeys, url);
        }

        dst.html("");
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            success: function (data) {
                dst.html(data);
                hdr.html("<h3>Add New " + title + "</h3>");
                $(model).modal('show');
                $(model).data('callerid', id);
                handleComboButtonCreate(model);
                $('html, body').animate({
                    scrollTop: $(dst).offset().top - 150
                }, 500)
            },
            error: function (data) {
                //console.log(data);
                dst.html(data);
                alert('error loading url');
            }
        });
    });
};
var handleComboButtonEdit = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonEdit");
    return;

    $(elem).on('click', 'button.btnComboEdit ', function () {
        var elem = $(this);
        var formElem = elem.closest('form');
        var rowElem = elem.closest('tr');
        var parentElem;
        if (rowElem.length !== 0) {
            parentElem = rowElem;
        }
        else if (formElem.length !== 0) {
            parentElem = formElem;
        }

        var id = $(elem).closest('.input-group').attr('id');
        var url = $(this).data('url');
        var title = $(this).data('propertytitle');
        var fkeys = $(this).data('fkeys').replace(/\s/g, "");
        var data = "";
        var model = $('#comboEdit');
        var dst = $(model).find('.modal-body');
        var hdr = $(model).find('.modal-header');
        if (fkeys !== "") {
            data = buildParmsFromKeys(parentElem, fkeys);
        }

        dst.html("");
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            success: function (data) {
                dst.html(data);
                hdr.html("<h3>Edit " + title + "</h3>");
                $(model).modal('show');
                $(model).data('callerid', id);
                handleComboButtonUpdate(model);
                $('html, body').animate({
                    scrollTop: $(dst).offset().top - 150
                }, 500)
            },
            error: function (data) {
                //console.log(data);
                dst.html(data);
                alert('error loading url');
            }
        });
    });
};
var handleComboButtonSearch = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonSearch");
    return;

    $(elem).off('click', 'button.btnComboSearch')
        .on('click', 'button.btnComboSearch', function () {
        //console.log('Load Combo Search');
        var elem = $(this);
        var formElem = elem.closest('form');
        var rowElem = elem.closest('tr');
        var parentElem;
        if (rowElem.length !== 0) {
            parentElem = rowElem;
        }
        else if (formElem.length !== 0) {
            parentElem = formElem;
        }

        var id = $(elem).closest('.input-group').attr('id');
        var url = $(this).data('url');
        var fkeys = $(this).data('fkeys').replace(/\s/g, "");
        var data = "";
        var model = $('#comboSearch');
        var dst = $(model).find('.modal-body');
        if (fkeys !== "") {
            data = buildParmsFromKeys(parentElem, fkeys, url);
        }

        dst.html("");
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            success: function (data) {
                dst.html(data);
                $(model).modal('show');
                $(model).data('callerid', id);
                Table().init(model);
                handleComboButtonSearchReturn(model);
                $('html, body').animate({
                    scrollTop: $(dst).offset().top - 150
                }, 500)
            },
            error: function (data) {
                //console.log(data);
                dst.html(data);
                alert('error loading url');
            }
        });
    });
};

var handleComboButtonCreate = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonCreate");
    return;

    $(elem).off('click', 'button.btnComboCreate')
        .on('click', 'button.btnComboCreate', function () {
            var elem = $(this);
            var modalElem = elem.closest('.modal');
            var formElem = modalElem.find('form');

            modalElem.Validate(function (errorElem) {
                if (errorElem === undefined) {
                    var data = $(formElem).SerializeForm();
                    var url = $(formElem).data('urlcreate');

                    $.post(url, data, function (res) {
                        if (res.success === "true") {
                            var modal = $(formElem).closest('.modal');
                            var callerElem = $('#' + $(modal).data('callerid'));//'[name="' + $(modal).data('callerid') + '"]'
                            var comboElem = $(callerElem).find('select');
                            var comboParent = comboElem.closest('form');
                            //If parent is empty load table row
                            if (comboParent.length == 0) {
                                comboParent = comboElem.closest('tr');
                            }
                            initSelectItems(comboElem, comboParent, res.value, true);
                            comboElem.val(res.value);
                            comboElem.change();
                            $(modal).modal('hide');
                        }
                        else {
                            SetValidationErrorForObject(formElem, res.errorModel);
                        }
                    });
                }
                else {
                    $('html, body').animate({
                        scrollTop: $(errorElem).offset().top - 150
                    }, 500)
                }
            })
        });
};
var handleComboButtonUpdate = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonUpdate");
    return;

    $(elem).off('click', 'button.btnComboEdit')
        .on('click', 'button.btnComboEdit', function () {
            var elem = $(this);
            var modalElem = elem.closest('.modal');
            var formElem = modalElem.find('form');
            modalElem.Validate(function (errorElem) {
                if (errorElem === undefined) {
                    var data = $(formElem).SerializeForm();
                    var url = $(formElem).data('urlsave');
                    $.post(url, data, function (res) {
                        if (res.success === "true") {
                            var modal = $(formElem).closest('.modal');

                            var callerElem = $('#' + $(modal).data('callerid'));
                            var comboElem = $(callerElem).find('select');


                            comboElem.selectpicker('destroy');
                            var comboParent = comboElem.closest('form');
                            if (comboParent.length == 0) {
                                comboParent = comboElem.closest('tr');
                            }
                            initSelectItems(comboElem, comboParent, comboElem.val(), true);
                            $(modal).modal('hide');
                        }
                    });
                }
                else {
                    $('html, body').animate({
                        scrollTop: $(errorElem).offset().top - 150
                    }, 500)
                }
            })
        });
};
var handleComboButtonSearchReturn = function (elem) {
    console.log("OLD COMBO INIT CALLED!!! *** handleComboButtonSearchReturn");
    return;

    //console.log(elem);
    $(elem).off('click', 'button.btnComboSearchSelect')
        .on('click', 'button.btnComboSearchSelect', function () {
            var elem = $(this);
            var modalElem = elem.closest('.modal');
            var tableElem = modalElem.find('table');
            var row = $(tableElem).find('tr.active');
            //var data = $(row).SerializeTableRow();
            var data = $(row).data('entitykey');
            var url = $(tableElem).data('urlsearchreturn');
            console.log(data, url);
            $.ajax({
                type: "Post",
                dataType: "json",
                url: url,
                data: data,
                async: false,
                success: function (res) {
                    if (res.success === "true") {
                        var callerElem = $('#' + $(modalElem).data('callerid'));
                        var comboElem = $(callerElem).find('select');
                        comboElem.attr('data-flushcache', 'True');
                        comboElem.data('flushcache', 'True');
                        //console.log(comboElem, res.value, comboElem.data('flushcache'));
                        initDropdown($(comboElem), res.value);

                        comboElem.val(res.value);
                        setTimeout(function () {
                            comboElem.change();
                        }, 200)
                        $(modalElem).modal('hide');
                    }

                },
                error: function (data) {
                    //console.log(data);
                }
            });
            //$.ajax(url, data, function (res) {
            //    if (res.success === "true") {
            //        var callerElem = $('#' + $(modalElem).data('callerid'));
            //        var comboElem = $(callerElem).find('select');
            //        comboElem.val(res.value);
            //        comboElem.change();
            //        $(modalElem).modal('hide');
            //    }
            //});

        });
};

function buildParmsFromKeys_OLD(parentElem, foreignKeys, url) {
    var data = "";
    var errMsg = "";
    foreignKeys = foreignKeys.replace(/ /g, "");
    var keys = foreignKeys.split(",");
    for (var i = 0; i < keys.length; i++) {
        var elemName = keys[i];
        var parmName = keys[i];
        var elemNameArr = keys[i].split("=");
        if (elemNameArr.length > 1) {
            parmName = elemNameArr[0];
            elemName = elemNameArr[1];
        }
        var keyElem = $(parentElem).find('[name="' + elemName + '"]');
        if (data !== '') {
            data += '&';
        }
        if (elemName.indexOf("'") !== -1) {
            elemName = elemName.replace(/'/g, '');
            data += parmName + '=' + elemName;
        }
        else if (keyElem.length !== 0) {
            var val = $(keyElem).val();
            data += parmName + '=' + val;
        }
        else {
            errMsg += "Key [" + elemName + "] Could not be found combo will not load";
        }
    }

    if (errMsg !== "") {
        console.log(url, errMsg);
        return "";
    }
    return data;
}

/* 08.Handle Combo Init
-----------------------------------*/
function reloadCombos(parentElem) {
    console.log("OLD COMBO INIT CALLED!!! *** reloadCombos");
    return;

    $(parentElem).find(":input:not(button)").each(function () {
        if ($(this).data('combourl') != undefined) {
            //console.log($(this).data('combourl'), $(this).val());
            initSelectItems(this, parentElem, $(this).val());
        }
    });
}
 
function initDropdown(elem, currentValue) {
    console.log("OLD COMBO INIT CALLED!!! *** initDropdown");

    return;
    var inputGroup = $(elem).closest('.input-group')
    var parentElem = $(elem).closest('form');
    if (parentElem.length === 0) {
        parentElem = $(elem).closest('tr');
    }
    if (parentElem.length === 0) {
        parentElem = $(elem).closest('.row');
    }
    if (parentElem.length === 0) {
        parentElem = $(elem).parent();
    }

    var flushCache = false;
    if (parentElem.length === 0) {
        parentElem = $(elem).closest('tr');
        if (parentElem.length === 0) {
            parentElem = $(elem).parent();
        }
    }
    if (elem.data("flushcache") == "True") {
        flushCache = true;
    }
    //console.log(elem, currentValue, elem.data("flushcache"));
    //console.log(elem, parentElem, currentValue);
    
    initSelectItems(elem, parentElem, currentValue, flushCache, false);
    if ($(elem).data("initbuttons") === undefined) {
        $(elem).data("initbuttons", true);
        handleComboButtonInfo($(inputGroup));
        handleComboButtonAdd($(inputGroup));
        handleComboButtonEdit($(inputGroup));
        handleComboButtonSearch($(inputGroup));
    }
}

function initInputButtons(elem) {
    console.log("OLD COMBO INIT CALLED!!! *** initInputButtons");
    return;
    var inputGroup = $(elem).closest('.input-group')
    if ($(elem).data("initbuttons") === undefined) {
        $(elem).data("initbuttons", true);
        handleComboButtonInfo($(inputGroup));
        handleComboButtonAdd($(inputGroup));
        handleComboButtonEdit($(inputGroup));
        handleComboButtonSearch($(inputGroup));
    }
}

function getComboData(elem) {
    console.log("OLD COMBO INIT CALLED!!! *** getComboData");
    return;
    var parentElem = $(elem).closest('form');
    if (parentElem.length === 0) {
        parentElem = $(elem).closest('tr');
        if (parentElem.length === 0) {
            parentElem = $(elem).parent();
        }
    }

    var url = $(elem).data('combourl');
    var combokeys = $(elem).data('combokeys');

    if (!url) {
        return;
    }
    var data = "";
    var cacheKey = url;
    if (combokeys) {
        combokeys = combokeys.replace(' ', '');
        var keys = combokeys.split(",");
        for (var i = 0; i < keys.length; i++) {
            var keyElem = $(parentElem).find('[name="' + keys[i] + '"]');
            if (data !== '') {
                data += '&';
            }
            if (keyElem.length !== 0) {
                var val = $(keyElem).val();
                data += keys[i] + '=' + val;
            }
            else {
                errMsg += "Key [" + keys[i] + "] Could not be found combo will not load";
            }
        }
    }

    console.log('getComboData', url, data);
    cacheKey += data;
    if (lscache.get(cacheKey)) {
        var itemList = JSON.parse(lscache.get(cacheKey));
    }
    else {
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            async: false,
            success: function (data, textStatus, xhr) {
                //lscache.set(cacheKey, data, 10);
                var itemList = jQuery.parseJSON(data);
            },
        });
    }

    return itemList;
}

function initSelectItems(elem, parentElem, currentValue, flushCache, verbose) {
    console.log("OLD COMBO INIT CALLED!!! *** initSelectItems");
    return;
    if (flushCache === undefined) {
        flushCache = false;
    }
    var funcCacheCombo = function (input, url, data, cacheKey, flushCache, verbose) {
        //verbose = false;
       
        if (verbose) {
            console.log('combo cache flushed', flushCache);
        }
        var funcDownloadList = function (rebose) {
            $.ajax({
                type: "Get",
                dataType: "html",
                url: url,
                data: data,
                async: false,
                success: function (data, textStatus, xhr) {
                    if (verbose) {
                        //console.log("data: ", data)
                    }
                    try {
                        if (data.indexOf("<html>") != -1 || data.indexOf("Error 500") != -1) {
                            //console.log("Combo Url Not found");
                            data = [
                                {
                                    Disabled: false,
                                    Group: null,
                                    Selected: true,
                                    Text: "Combo Url Not Found",
                                    Value: null,
                                }
                            ];
                            itemList = data;
                            $(input).hide().after("<div class='text-red text-center font-weight-bold'>Invalid parms/url</div>")
                            //buildCombo(itemList, input, currentValue);
                        }
                        else {
                            if (data !== "") {
                                var itemList = jQuery.parseJSON(data);
                                //lscache.set(cacheKey, data, 10);
                                //console.log('Combo Item List saved', cacheKey);
                                buildCombo(itemList, input, currentValue, cacheKey, flushCache, verbose);
                            }
                        }
                    } catch (e) {
                        $(input).append(data);
                    }

                },
                error: function (data) {
                    //console.log(data);
                }
            });
        };
        if (flushCache) {
            lscache.remove(cacheKey);
        }
        var itemList = lscache.get(cacheKey);
        if (itemList) {
            var itemList = JSON.parse(itemList);
            if (itemList.length === 0) {
                if (verbose) {
                    console.log('Combo List is Empty, Rebuild List', cacheKey);
                }
                funcDownloadList();
            }
            if (verbose) { 
                console.log('Combo Item List retrieved from cache', cacheKey, itemList.length);
            }
            buildCombo(itemList, input, currentValue, cacheKey, flushCache, verbose);
        }
        else {
            if (verbose) {
                console.log('Combo List Downloaded', cacheKey);
            }
            funcDownloadList(verbose);
        }
    }
    var funcCombo = function (input, url, data) {
        //console.log('Combo List Downloaded');
        var cacheKey = url + '?';        
        $.ajax({
            type: "Get",
            dataType: "html",
            url: url,
            data: data,
            async: false,
            success: function (data, textStatus, xhr) {
                try {
                    if (data.indexOf("<html>") != -1) {
                        $(input).hide().after("<div class='text-red text-center font-weight-bold'>ERROR</div>")
                    }
                    else {
                        if (data !== "") {
                            var itemList = jQuery.parseJSON(data);
                            buildCombo(itemList, input, currentValue, cacheKey, flushCache);
                        }
                    }
                } catch (e) {
                    $(input).append(data);
                }

            },
            error: function (data) {
                //console.log(data);
            }
        });
    }

    var url = $(elem).data('combourl');

    if (verbose) {
        console.log("**************************************Verbose Started for " + url + "**************************************");
    }
    var combokeys = $(elem).data('combokeys');
    var originalCacheKey = $(elem).attr("data-cachekey");
    var cacheUrl = true;
    if ($(elem).attr("data-cacheurl")) {
        cacheUrl = $(elem).attr("data-cacheurl") == "true" || $(elem).attr("data-cacheurl") == "True";
        //console.log('Combo cache found!', cacheUrl);
    }
    if (!url) {
        return;
    }
    if (flushCache == true) {
        console.log('Flush Combo, ', combokeys);
    }

    var updateCombo = false;
    var input = elem;
    var data = "";
    var cacheKey = url +'?';
    if (combokeys) {
        data = buildParmsFromKeys(parentElem, combokeys, url);
        //console.log('data parms:', data);
    }
    cacheKey += data;
    if (originalCacheKey === undefined) {
        updateCombo = true;
        $(input).attr("data-cachekey", cacheKey);
    }
    else if (originalCacheKey !== cacheKey || flushCache) {
        updateCombo = true;
        $(input).attr("data-cachekey", cacheKey);
    }

    if (updateCombo) {
        if (cacheUrl) {
            if (verbose) {
                console.log('url:', url, 'parms: ', data, " originalCacheKey:", originalCacheKey, "cacheKey:", cacheKey);
            }
            funcCacheCombo(input, url, data, cacheKey, flushCache, verbose);
        }
        else {
            funcCombo(input, url, data);
        }
    }
}

function buildComboElem(itemList, cacheKey, flushCache, isSearch, verbose) {
    console.log("OLD COMBO INIT CALLED!!! *** buildComboElem");
    return;

    cacheKey += "_select";
    if (flushCache || true) {
        lscache.remove(cacheKey);
    }
    var markup = lscache.get(cacheKey);
    var elem = $('<select></select>').html(markup);
    //console.log('Combo option List cache count', cacheKey, $(elem).find('option').length);
    if (markup && $(elem).find('option').length > 1) {
        if (verbose) {
            console.log('Combo option List retrieved from cache', cacheKey);
        }
        return markup;
    }
    else {
        markup = "";
        var myList = itemList;
        if (verbose) {
            console.log('Combo current List retrieved from cache', itemList.length);
        }
        var group_to_values = myList.reduce(function (obj, item) {
            if (item.Group != null) {
                var key = item.Group.Name + '|' + item.Group.Disabled;
                obj[key] = obj[key] || [];
                obj[key].push(item);
                return obj;

            }
            else {
                obj["|false"] = obj["|false"] || [];
                obj["|false"].push(item);
                return obj;
            }
        }, {});
        var groups = Object.keys(group_to_values).map(function (key) {
            var splitKey = key.split('|');
            group = splitKey[0];
            disabled = splitKey[1] === "true" ? true : false;
            return {
                group: group,
                disabled: disabled,
                listItems: group_to_values[key]
            };
        });
        if (verbose) {
            //console.log('Group count:', groups.length);
        }
        
        for (var g = 0; g < groups.length; g++) {
            if (groups[g].group !== "") {
                markup += "<optgroup label='" + groups[g].group + "'>";
            }
            var groupList = groups[g].listItems;

            if (verbose) {
                //console.log('Group Length:', groupList.length, cacheKey);
            }
            for (x = 0; x < groupList.length; x++) {
                var itemValue = String(groupList[x].Value);
                if (itemValue === 'null') {
                    itemValue = null;
                }
                markup += "<option";
                if (groupList[x].Value !== null) {
                    markup += " value='" + groupList[x].Value + "'";
                }
                else {
                    markup += " value=''"
                }

                if (groups[g].disabled === true) {
                    markup += " disabled ";
                }
                markup += ">";

                if (isSearch == true && (itemValue === null || itemValue === '')) {

                }
                else {
                    markup += groupList[x].Text;
                }
                markup += "</option>";
            }

            if (groups[g].group !== "") {
                markup += "</optgroup>";
            }
        }
        if (verbose) {
            //console.log('Combo option List saved to cache', cacheKey, $(elem).find('option').length, markup);
        }
        if (itemList.length > 1) {
            elem = $('<select></select>').html(markup);
            if (verbose) {
                console.log('Combo option List saved to cache', cacheKey, $(elem).find('option').length);
            }
           // lscache.set(cacheKey, markup, 10);
        }
        return markup;
    }
}

function buildCombo(itemList, input, currentValue, cacheKey, flushCache, verbose) {
    console.log("OLD COMBO INIT CALLED!!! *** buildCombo");
    return;

    //if (verbose) {
    //    console.log('Build Combo', cacheKey, $(input), 'Is select', $(input).is('select'), itemList.length);
    //}
    currentValue = String(currentValue);
    var markup = "";
    var isInModel = $(input).closest('.modal').length !== 0;
    var isInWizrd = $(input).closest('.modal').length !== 0;
    if (flushCache) {
        if (verbose) {
            console.log('combo cache flushed', cacheKey);
        }
    }

    if ($(input).is('select')) {
        if (verbose) {
            console.log('combo is select input', cacheKey);
        }
        var isSearchstr = $(input).attr("data-search");
        var isSearch = false;
        if (isSearchstr === 'True') {
            isSearch = true;
        }
        $(input).html(markup);
        markup = buildComboElem(itemList, cacheKey, flushCache, isSearch, verbose);
        $(input).html(markup);
        if (currentValue !== undefined && currentValue !== '') {
            $(input).find('option').removeAttr('selected');
            $(input).find('option[value="' + currentValue + '"]').attr('selected', 'selected');
            var selectElem = $(input).find('option[selected="selected"]');
            if ($(selectElem).length === 0) {
                console.log('Option for each search', cacheKey, currentValue);
                $(input).find('option').each(function () {
                    itemValue = $(this).attr('value');
                    if (itemValue === currentValue) {
                        $(this).attr('selected', 'selected');
                        return false;
                    }
                    else if (!isNaN(currentValue) && !isNaN(itemValue)) {
                        var comboVal = parseFloat(itemValue);
                        var selectVal = parseFloat(currentValue);
                        if (comboVal === selectVal) {
                            markup += " selected='selected'";
                            $(this).attr('selected', 'selected');
                            return false;
                        }
                    }
                });
            }
            //console.log('currentValue', currentValue);
            
            //$(input).find('option[value=' + currentValue + ']').attr('selected', 'selected');
        }

        if (itemList.length <= 1) {
            $(input).hide();
            var parent = $(input).parent();
            $(parent).find('.btnInfo').each(function () {
                $(this).hide();
            });
            $(parent).find('.btnComboAdd').each(function () {
                $(input).show();
                //$(this).hide();
            });
            $(parent).find('.btnComboEdit').each(function () {
                $(this).hide();
            });
        }
        if (itemList.length > 1) {
            $(input).show();
        }
        if (true) {       
            $(input).each(function () {
                $(this).selectpicker('destroy');
                if (isInModel) {
                    $(this).data('container','');
                }
                if ($(this).data("role") !== "tagsinput") {
                    if ($(this).find('option').length > 15 && isSearch != true) {
                        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                            if (itemList.length > 200) {
                                //$(input).selectpicker('mobile');
                            }
                            //$(input).selectpicker('mobile');
                        }
                        else {
                            $(input).selectpicker('render');
                        }
                    }
                }
            });
        }
    }    
    else {
        if (currentValue !== "") {
            //var item = itemList.find(x => x.Value === currentValue);
            var item = itemList.filter(function (x) { return x.Value === currentValue; });
            if (item === undefined && !isNaN(currentValue)) {
                var selectVal = parseFloat(currentValue);
                //item = itemList.find(x => parseFloat(x.Value) === selectVal);
                item = itemList.filter(function (x) { return parseFloat(x.Value) === selectVal; });
            }
            if (Array.isArray(item)) {
                item = item[0];
            }
            //console.log(currentValue, item);
            if (item !== undefined) {
                $(input).html(item.Text); //Updates text of input for display only combo
            }
        }
    }
}

/* 09. Handle Tags Init */


function initTags(elem, currentValue) {
    var data = getComboData(elem);
    var list = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.whitespace,
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        local: data,
        identify: function (obj) { return obj.Value; },
    });
    list.initialize();
    var elt = $(elem);
    elt.tagsinput({
        itemValue: 'Value',
        itemText: 'Text',
        typeaheadjs: {
            name: 'myList',
            displayKey: 'Text',
            source: list.ttAdapter()
        }
    });
    //console.log(elt, list);
} 

/* 10. Custom Functions*/

function resizeImage(imgElem, container) {
    //console.log('resizeImage' , imgElem);
    var img = $(imgElem);
    if (container === null) {
        container = $(img).closest('.panel-body');
    }
    var maxWidth = $(container).width() *.9 ; // Max width for the image
   
    var ratio = 0;  // Used for aspect ratio
    var width = $(img)[0].naturalWidth;    // Current image width
    var height = $(img)[0].naturalHeight;  // Current image height
    //ratio = width / height;
    if (width > maxWidth) {
        ratio = maxWidth / width;   // get ratio for scaling image
        height = height * ratio;    // Reset height to match scaled image
        width = maxWidth
        //$(img).css("width", maxWidth); // Set new width
        //$(img).css("height", height);  // Scale height based on ratio
    }

    $(img).css('height', height);
    $(img).css('width', width);
    //$(img).closest('.image').css('height', height * .9);
    //$(img).closest('.image').css('width', width * .9);
    //console.log(width, height, maxWidth, ratio);
}

function getArrayBuffer(file) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();

        reader.onloadend = function (e) {
            resolve(e.target.result);
        };
        reader.onerror = function (e) {
            reject(e.target.error);
        };
        reader.readAsArrayBuffer(file);
    });
}

function convertPdfToThumbnailForDropZone(pdfData, file) {
    //console.log('pdfData:', pdfData);
    pdfjsLib.GlobalWorkerOptions.workerSrc = '/Content/assets/plugins/pdfjs/pdf.worker.js';//~/Content/assets/plugins/pdfjs/pdf.worker.js
    pdfjsLib.getDocument(pdfData).promise.then(function (doc) {
        var pages = []; while (pages.length < 1) pages.push(pages.length + 1);
        return Promise.all(pages.map(function (num) {
            //var div = document.createElement("div");
            //document.getElementById("reviewPanel").appendChild(div);
            return doc.getPage(num).then(makeThumb)
                .then(function (canvas) {
                    //div.appendChild(canvas);
                    //use canvas data to add a new file to the dropzone and add the file to the queue 
                    //canvas.toBlob(resultBlob => {
                    //    var name = file.name;
                    //    name = name.replace(".pdf", "_THUMBNAIL.png");
                    //    resultBlob.lastModifiedDate = file.lastModifiedDate;
                    //    resultBlob.name = name;

                    //    // add converted file to upload
                    //    //console.log('File Converted', resultBlob.name);
                    //    dpzMultipleFiles.handleFiles([resultBlob]);
                    //});

                    canvas.toBlob(function (resultBlob) {
                        var name = file.name.replace(".pdf", "_THUMBNAIL.png");
                        var fullpath = file.fullPath;
                        if (fullpath == undefined)
                            fullpath = name;

                        fullpath = fullpath.replace(".pdf", "_THUMBNAIL.png");
                        resultBlob.lastModifiedDate = file.lastModifiedDate;
                        resultBlob.name = name;
                        resultBlob.fullPath = fullpath;
                        resultBlob.IsThumbnail = true;
                        // add converted file to upload
                        //console.log('File Converted', resultBlob.name);
                        dpzMultipleFiles.handleFiles([resultBlob]);
                    });
                });
        }));
    }).catch(console.error)
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

function CreateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}  

function CallBtnAction(button, callback) {
    $(button).SetButtonBusy();
    var url = $(button).data('url');
    var validate = $(button).data('validate');
    var form = $(button).closest('form');
    if (form === undefined || form.length == 0) {
        console.log('No form found looking for row elem');
        form = $(button).closest('.row');
    }
    var token = $(form).find('[name="__RequestVerificationToken"]');
    var data = {
        __RequestVerificationToken: token.val(),
    };

    [].forEach.call(button.attributes, function (attr) {
        if (/^data-/.test(attr.name)) {
            var camelCaseName = attr.name.substr(5).replace(/-(.)/g, function ($0, $1) {
                return $1.toUpperCase();
            });
            data[camelCaseName] = attr.value;
        }
    });
    var func = function (button, url, data, callback) {
        console.log(url, data);
        $.post(url, data, function (res) {
            if (res.success === "true") {
                callback();
            }
            $(button).SetButtonNotBusy();
        });
    }
    if (validate) {
        $(form).PostValidate(button, function (valid) {
            if (valid) {
                func(button, url, data, callback);
            }
        });
    }
    else {
        func(button, url, data, callback);
    }
}
// UMD

function makeThumb(page) {
    // draw page to fit into 96x96 canvas
    var vp = page.getViewport({ scale: 1, });
    var canvas = document.createElement("canvas");
    var scalesize = 1.5;
    canvas.width = vp.width * scalesize;
    canvas.height = vp.height * scalesize;
    var scale = Math.min(canvas.width / vp.width, canvas.height / vp.height);
    //console.log(vp.width, vp.height, scale);
    return page.render({ canvasContext: canvas.getContext("2d"), viewport: page.getViewport({ scale: scale }) }).promise.then(function () {
        return canvas;
    });
}

function ConvertPDfToThumbNail(file) {

    getArrayBuffer(file).then(function (pdfData) {
        pdfjsLib.GlobalWorkerOptions.workerSrc = '~/Content/assets/plugins/pdfjs/pdf.worker.js';
        pdfjsLib.getDocument(pdfData).promise.then(function (doc) {

            var pages = []; while (pages.length < 1) pages.push(pages.length + 1);
            return Promise.all(pages.map(function (num) {
                return doc.getPage(num)
                    .then(makeThumb)
                    .then(function (canvas) {
                        canvas.toBlob(resultBlob => {
                            var name = file.name.replace(".pdf", "_THUMBNAIL.png");
                            var fullpath = file.fullPath.replace(".pdf", "._THUMBNAIL.png");
                            resultBlob.lastModifiedDate = file.lastModifiedDate;
                            resultBlob.name = name;
                            resultBlob.fullPath = fullpath;
                            resultBlob.IsThumbnail = true;
                            return [resultBlob];
                        });
                    });
            }));
        }).catch(console.error);
    });
    
}

function ConvertHEICFileToGif(file) {
    setTimeout(function () {
        heic2any({
            blob: file,
            toType: "image/gif"
        }).then(resultBlob => {
            var fullpath = file.fullPath.replace(".heic", ".gif");
            var name = file.name.replace(".heic", ".gif");
            resultBlob.lastModifiedDate = file.lastModifiedDate;
            resultBlob.name = name;
            resultBlob.fullPath = fullpath;
            return [resultBlob];
        }).catch((e) => {
            return null;

            alert('Error on HEIC transformation: "' + e + '"');
        });
    }, 10);
}
function dataURItoBlob(dataURI) {
    // convert base64 to raw binary data held in a string
    // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
    var byteString = atob(dataURI.split(',')[1]);

    // separate out the mime component
    var mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];

    // write the bytes of the string to an ArrayBuffer
    var ab = new ArrayBuffer(byteString.length);
    var ia = new Uint8Array(ab);
    for (var i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
    }

    //Old Code
    //write the ArrayBuffer to a blob, and you're done
    //var bb = new BlobBuilder();
    //bb.append(ab);
    //return bb.getBlob(mimeString);

    //New Code
    return new Blob([ab], { type: mimeString });


}

function GenerateThumbnail(file) {
    var boundBox = [100, 100];
    if (!boundBox || boundBox.length != 2) {
        throw "You need to give the boundBox"
    }
    var reader = new FileReader();
    var canvas = document.createElement("canvas")
    var ctx = canvas.getContext('2d');

    return new Promise((resolve, reject) => {
        reader.onload = function (event) {
            var img = new Image();
            img.onload = function () {
                var scaleRatio = Math.min(boundBox[0] / img.width, boundBox[1] / img.height, 1);
                let w = img.width * scaleRatio;
                let h = img.height * scaleRatio;
                canvas.width = w;
                canvas.height = h;
                ctx.drawImage(img, 0, 0, w, h);
                return resolve(canvas);
                //return resolve(canvas.toDataURL(file.type))
            }
            //console.log('event.target.result', event.target.result);
            img.src = event.target.result;
        }
        reader.readAsDataURL(file);
    });
}

