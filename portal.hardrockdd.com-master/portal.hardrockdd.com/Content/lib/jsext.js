jQuery.fn.extend({
    SetButtonBusy: function () {
        if (this.is("button")) {
            var button = this;
            $(button).prop('disabled', true);
            var iElem = $(button).find('i');
            if (iElem.length === 0) {
                //console.log('buttun i added');
                $(button).prepend("<i class='fa fa-spinner fa-spin'></i>");
                button.attr('data-oldclass', "fa");
            }
            else if (!$(iElem).hasClass('fa-spinner')) {
                var classList = "";
                classList = $(iElem).attr('class');
                $(iElem).AlterClass('fa-*', 'fa-spinner').addClass('fa-spin');
                button.attr('data-oldclass', classList);
            }
        }
    },
    SetButtonNotBusy: function () {
        if (this.is("button")) {
            var button = this;
            var iElem = $(button).find('i');
            if ($(iElem).hasClass('fa-spinner')) {
                var originalClass = button.attr('data-oldclass');
                $(iElem).attr('class', originalClass);
            }
            $(button).prop('disabled', false);
        }
    },
    InitSwitchery: function () {
        var elem = this;
        if ($(elem).attr("data-switcheryinit") !== "true") {
            //console.log('init Switch');
            var themeColor = COLOR_GREEN;
            //console.log('Switch theme: ', $(elem).attr('data-theme'));
            if ($(elem).attr('data-theme')) {
                switch ($(elem).attr('data-theme')) {
                    case 'red': themeColor = COLOR_RED; break;
                    case 'blue': themeColor = COLOR_BLUE; break;
                    case 'purple': themeColor = COLOR_PURPLE; break;
                    case 'orange': themeColor = COLOR_ORANGE; break;
                    case 'black': themeColor = COLOR_BLACK; break;
                }
            }

            var option = {};
            option.color = themeColor;
            option.secondaryColor = ($(elem).attr('data-secondary-color')) ? $(elem).attr('data-secondary-color') : '#dfdfdf';
            option.className = ($(elem).attr('data-classname')) ? $(elem).attr('data-classname') : 'switchery';
            option.disabled = $(elem).attr('data-disabled') === "true";
            option.size = ($(elem).attr('data-size')) ? $(elem).attr('data-size') : 'default';
            option.disabledOpacity = ($(elem).attr('data-disabled-opacity')) ? parseFloat($(elem).attr('data-disabled-opacity')) : 0.5;
            option.speed = ($(elem).attr('data-speed')) ? $(elem).attr('data-speed') : '0.5s';
            //console.log('Switchery', option);
            var switchery = new Switchery(elem.get(0), option);

            $(elem).attr("data-switcheryinit", "true");
            return switchery;
        }
        return undefined;
    },
    SerializeTableRow: function () {
        var trValues = "";
        var evalVal = function (elem, val, dataType) {
            if (val !== null) {
                if ($.isArray(val)) {
                    val = val.join("|");
                }

                var regExp = /[a-zA-Z]/g;
                if (!regExp.test(val)) {
                    if (dataType == "number") {
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
                    }
                }
            }
            return val;
        }
        $(this).find("td").each(function () {
            var elemId = $(this).attr('name');
            var val = $(this).attr('value');
            var dataType = $(this).attr("data-type");


            if (elemId && elemId !== "") {
                if (trValues !== '') {
                    trValues += '&';
                }
                val = evalVal(this, val, dataType);
                if (val != null) {
                    trValues += elemId + '=' + encodeURIComponent(val);
                }
            }
        });

        $(this).find(":input:not(button)").each(function () {

            var elemId = $(this).attr('name');
            if (!elemId) {
                var parentElem = $(this).parent();
                if (!$(parentElem).hasClass('bs-searchbox')) {
                    //console.log('Control has no name Attr:', $(this));
                }
            }
            if (elemId && elemId !== "") {
                if (trValues !== '') {
                    trValues += '&';
                }
                switch ($(this).attr('type')) {
                    case "checkbox":
                        trValues += elemId + '=' + $(this).is(':checked');
                        break;
                    default:
                        var val = $(this).val();
                        val = evalVal(this, val, $(this).attr("data-type"));
                        //if ($.isArray(val)) {
                        //    val = val.join("|");
                        //}
                        //if (val !== null) {
                        //    var regExp = /[a-zA-Z]/g;
                        //    if (!regExp.test(val)) {
                        //        if ($(this).attr("data-type") == "number") {
                        //            var number = val.replace(/[^0-9.-]+/g, "");
                        //            number = number.replace(/,/g, '');
                        //            if (val.indexOf("%") >= 0) {
                        //                val = parseFloat(val) / 100.0;
                        //            }
                        //            else if (val.indexOf("$") >= 0) {
                        //                var isNegative = val.indexOf("(") >= 0 || val.indexOf("-") >= 0;
                        //                var floatVal = parseFloat(number);
                        //                val = floatVal * (isNegative ? -1.0 : 1.0);
                        //            }
                        //            else if (!isNaN(parseFloat(number))) {
                        //                var floatVal = parseFloat(number);
                        //                val = floatVal
                        //            }

                        //            //console.log(val, $(this).val());
                        //        }
                        //    }
                        //}
                        if (val != null) {
                            trValues += elemId + '=' + encodeURIComponent(val);
                        }
                }
            }
        });


        var formData = trValues.split("&");
        var data = {};
        for (var key in formData) {
            var keyname = formData[key].split("=")[0];
            var keyVal = decodeURIComponent(formData[key].split("=")[1]);
            if (keyVal !== null && keyVal !== "null") {
                data[keyname] = keyVal;
            }
        }
        return data;
        //return trValues;
    },
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
                            if (val === undefined) {
                                val = null;
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
    },
    GetMaxTabIndex: function () {
        var idx = -1;
        $('body [tabindex]').attr('tabindex', function (a, b) {
            idx = Math.max(idx, +b);
        });
        return idx;
    },
    LockInputs: function (showDisabled) {
        var elem = this;
        if (showDisabled == undefined)
            showDisabled = false;
        $(elem).find(':Input:not(button)').each(function () {
            var parentElem = $(this).parent();
            //if (!$(parentElem).hasClass('bootstrap-select')) {
            var isDisabled = $(this).prop('disabled');
            $(this).data('isdisabled', $(this).prop('disabled'));
            $(this).prop('disabled', true);
            if (!isDisabled) {
                if (!showDisabled) {
                    $(this).css('background', 'white');
                    $(this).data('isfocused', $(this).is(':focus'));
                }
            }
           // }
        });
    },
    UnLockInputs: function () {
        var elem = this;
        $(elem).find(':Input:not(button)').each(function () {
            var isDisabled = $(this).data('isdisabled');
            $(this).removeAttr("disabled");            
            if (!isDisabled) {
                $(this).css('background', '');
            }
        });
        //var tabIdx = $(currentFocus).attr('tabindex');
        //tabIdx++;
        //$(elem).find('[tabindex=' + tabIdx + ']').focus();
    },
    TableInit: function (elem) {
        Table().init(elem);
    },
    AlterClass: function (removals, additions) {

        var self = this;

        if (removals.indexOf('*') === -1) {
            // Use native jQuery methods if there is no wildcard matching
            self.removeClass(removals);
            return !additions ? self : self.addClass(additions);
        }

        var patt = new RegExp('\\s' +
            removals.
                replace(/\*/g, '[A-Za-z0-9-_]+').
                split(' ').
                join('\\s|\\s') +
            '\\s', 'g');

        self.each(function (i, it) {
            var cn = ' ' + it.className + ' ';
            while (patt.test(cn)) {
                cn = cn.replace(patt, ' ');
            }
            it.className = $.trim(cn);
        });
        return !additions ? self : self.addClass(additions);
    },
    UpdateElementValuesByKey: function (entityKey, model) {
        $(document).find('[data-entitykey="' + entityKey + '"]').each(function () {
            var isForm = $(this).closest('form').length !== 0;
            var isTable = $(this).closest('table').length !== 0;

            if (isForm) {
                var form = $(this).closest('form');
                //console.log('Update Form Values', form);
                Form().updateFormValues(form, model);
            }
            else if (isTable) {
                var row = this;
                //console.log('Update Table Values', row);
                Table().updateRowValues(row, model);
            }
            else {
                //console.log('Update Values', this);
                $(this).UpdateElementValues(res.model);
            }
        });
    },
    UpdateElementValues: function(model) {
        var elem = this;
        $.each(model, function (key, value) {
            var updElem = $(elem).find("[name='" + key + "']");
            $(updElem).each(function () {
                var name = $(this).attr('name');
                if (value === null) {
                    value = "";
                }
                //console.log('Name: ', name, $(this).val(), String(value));
                if ($(this).is('div') || $(this).is('span')) {
                    var name = $(this).attr('name');
                    var val = value + "";

                    if (val.indexOf('/Date') >= 0) {
                        val = dateTimeFormat(val);
                    }
                    $(this).html(String(val));
                }
                else {
                    var curVal = $(this).val();
                    curVal = curVal + "";
                    if (curVal !== String(value)) {
                        var elemDisabled = $(this).prop("disabled");
                        $(this).prop("disabled", false);
                        var isbootstrapSelect = $(this).parent().hasClass('bootstrap-select');
                        var isbootstrapDatePicker = $(this).data("datepickerid") != undefined;
                        if (isbootstrapSelect) {
                            //console.log('isbootstrapSelect: ', isbootstrapSelect, ' elemDisabled: ', elemDisabled);
                            $(this).selectpicker('refresh');
                        }
                        if (value != null) {
                            var val = value + "";

                            if (val.indexOf('/Date') >= 0) {
                                //val = parseJsonDate(value);
                                value = dateTimeFormat(val);
                            }
                            if (typeof $(this).data('datepickerid') !== 'undefined') {
                                value = dateTimeFormatFromStr(val);
                            }
                            if (curVal.indexOf("%") >= 0) {
                                //console.log("Percent Value : ", value)
                                value = Math.round(parseFloat(value) * 100.0);
                            }
                            if (curVal.indexOf("$") >= 0) {
                                value = currencyFormat(parseFloat(value), 2);
                            }
                        }
                        $(this).val(value);
                        $(this).prop("disabled", elemDisabled);
                        if (isbootstrapSelect) {
                            $(this).selectpicker('refresh');
                        }
                        if (isbootstrapDatePicker) {
                            //console.log('isbootstrapDatePicker', isbootstrapDatePicker);
                            $(this).datepicker('destroy');
                            var datepicker_options = {
                                altFormat: "mm/dd/yyyy",
                                dateFormat: "mm/dd/yy",
                                yearRange: "1000:3000",
                                orientation: 'bottom',
                                changeMonth: true,
                                changeYear: true,
                                todayHighlight: true,
                            };
                            $(this).datepicker(datepicker_options);
                        }
                    }
                }
                //Store current value to be sent for combo box
                $(this).attr('data-currentvalue', model[name]);
            });
        });
        $(elem).find("[data-combokeys]").each(function () {
            $(this).data("comboinit", "false");
            $(this).data("selectinit", "false");
            Combo().init(this);
        });
    },
    ReloadPanel: function (url) {
        var func = function (target, url) {
            //console.log(target);
            if (!$(target).hasClass('panel-loading')) {
                if (url) {
                    $(target).data('reloadurl', url);
                    $(target).attr('data-reloadurl', url);
                }
                var targetBody = $(target).find('.panel-body');
                var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';
                var targetReloadUrl = $(target).data('reloadurl');
                var targetInitPaneFunc = $(target).data('initpane');
                $(target).addClass('panel-loading');
                $(targetBody).prepend(spinnerHtml);
                if (targetReloadUrl !== '' && targetReloadUrl !== undefined) {
                    setTimeout(function () {
                        $.ajax({
                            type: "Get",
                            dataType: "html",
                            url: targetReloadUrl,
                            data: "",
                            success: function (data) {
                                $(target).removeClass('panel-loading');
                                $(target).find('.panel-loader').remove();
                                //console.log('Pre Load Html');
                                $(targetBody).empty();
                                $(targetBody).append(data);
                                //$(targetBody).html(data);
                                //console.log('Post Load Html');
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
        var getTarget = function (elem, level, url) {
            if (level == undefined)
                level = 0;
            var target = $(elem).closest('.panel');
            var reloadBtn = $(target).find('[data-click="panel-reload"]');

            if (url === undefined) {
                url = $(target).data('reloadurl');
            }
            if (reloadBtn.length === 0 && level < 5) {
                level++;
                getTarget($(target).parent(), level++, url);
            }
            else {
                //console.log('Reload Panel Level:', level, url);

                if (url == undefined) {
                    var reloadBtn = $(target).find('[data-click="panel-reload"]');
                    $(reloadBtn).click();
                }
                else {
                    func($(target), url);
                }
            }
        }
        var target = getTarget($(this),0,url);
        //console.log($(target), url);
        //if ($(target).hasClass("panel")) {
        //    //if (url === undefined) {
        //    //    url = $(this).data('reloadurl');
        //    //}
        //    if (url == undefined) {
        //        var reloadBtn = $(target).find('[data-click="panel-reload"]');
        //        $(reloadBtn).click();
        //    }
        //    else {
        //        func($(target), url);
        //    }
        //}

    }
});