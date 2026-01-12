function Form() {
    return {
        init: function (formElem) {
            var id = $(formElem).attr('id');
            var isBinding = $(formElem).data("forminit") === "true";
            if (!isBinding) {
                //console.log('Form.init: ', $(formElem));
                this.initTagId(formElem);
                this.initInputUpdateHandle(formElem);
                this.initInputPlugins(formElem);
                this.handleInputButtons(formElem);
                $(formElem).data("forminit", "true");
            }
        },
        isUpdating: function (formElem, callback) {
            setTimeout(function () {
                if ($(formElem).attr('data-fieldupdating') == "true") {
                    Form().isUpdating(formElem, callback);
                }
                else {
                    callback();
                }
            }, 100);
        },
        initTagId: function (formElem) {
            var idx = $(formElem).GetMaxTabIndex();
            $(formElem).find(':input').each(function () {
                $(this).attr('tabindex', idx);
                idx++;
            });
        },
        initInputUpdateHandle: function(formElem) {
            $(formElem)
                .off('change', ':Input:not(button)')
                .on('change', ':Input:not(button)', function () {                
                    var elem = this;

                    if (!$(this).parent().hasClass('bs-searchbox')) {
                        var form = $(this).closest('form');
                        $(form).data('fieldupdating', "true");
                        $(form).attr('data-fieldupdating', "true");
                        var entitykey = $(form).data("entitykey")
                        var data = $(form).SerializeForm();
                        var url = $(form).data('urlupdate');
                        var postUpdateFunc = $(form).data('postupdate');
                    
                        //console.log('Form updated', $(this), url, data, entitykey);
                    
                        //$(form).LockInputs();
                        $.post(url, data, function (res) {
                            //$(document).find('[data-entitykey="' + entitykey + '"]').each(function () {
                            //    console.log('form: ', entitykey, 'updateElementValues');
                            //    $(this).UpdateElementValues(res.model);
                            //});
                            $(form).UpdateElementValuesByKey(entitykey, res.model);
                            //$(form).UnLockInputs();
                            SetValidationErrorForObject(form, res.errorModel);
                            //console.log('postUpdateFunc', postUpdateFunc);
                            if (postUpdateFunc !== '' && postUpdateFunc !== undefined) {
                                window[postUpdateFunc](elem);
                            }

                            $(form).data('fieldupdating', "false");
                            $(form).attr('data-fieldupdating', "false");
                        });
                    }
                });
            },
        initInputPlugins: function (formElem) {
            $(formElem).find('textarea').each(function () {
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

            $(formElem).find('[data-selectinit="false"]').each(function () {
                Combo().init($(this));
                $(this).attr("data-selectinit", "true");
            });

            $(formElem).find('[data-datepickerid]').each(function () {
                datepicker_options = {
                    altFormat: "mm/dd/yyyy",
                    dateFormat: "mm/dd/yy",
                    yearRange: "1000:3000",
                    changeMonth: true,
                    changeYear: true,
                    todayHighlight: true
                };
                $(this).datepicker(datepicker_options);

            });

            $(formElem).find('[data-numberformat="percent"]').each(function () {
                var option = {
                    alias: "percentage"
                }
                $(this).inputmask(option);
            });

            $(formElem).find('[data-switcheryid]').each(function () {
                $(this).InitSwitchery();
            });


        },
        handleInputButtons: function (formElem) {
            $(formElem).find('.btnInfo').each(function () {
                var inputGroup = $(this).closest('.input-group')
                if ($(inputGroup).find('.btnInfo').length > 0) {
                    $(inputGroup).on('click', 'button.btnInfo ', function () {
                        var url = $(this).data('urlinfo');
                        var fkeys = $(this).data('fkeys');
                        if (fkeys != undefined) {
                            fkeys = fkeys.replace(/\s/g, "");
                        }
                        else {
                            fkeys = "";
                        }
                        var errMsg = ""
                        var data = Combo().getParms(inputGroup, fkeys, errMsg);
                        if (url !== undefined) {
                            console.log(url, data, fkeys);
                            OpenPopUpWindow(url, $.param(data));
                        }
                    })
                }
            });
            $(formElem).off('click', 'button.btnEye')
                        .on('click', 'button.btnEye', function () {
                            //console.log('Eye was clicked');
                            var icon = $(this).find('i');
                            if ($(icon).hasClass('fa-eye')) {
                                var group = $(this).closest('.input-group');
                                if (group) {
                                    var input = $(group).find('input');
                                    $(input).attr('type', 'text');
                                }
                                $(icon).removeClass('fa-eye').addClass('fa-eye-slash');
                            }
                            else if ($(icon).hasClass('fa-eye-slash')) {
                                var group = $(this).closest('.input-group');
                                if (group) {
                                    var input = $(group).find('input');
                                    $(input).attr('type', 'password');
                                }
                                $(icon).removeClass('fa-eye-slash').addClass('fa-eye');
                            }
                            $(input).attr('originaltype', 'password');
                        });
        },
        updateFormValues: function (formElem, model) {
            $(formElem).UpdateElementValues(model);
        }
    }
}