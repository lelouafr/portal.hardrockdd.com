
var storedList = [];
var Combo = function () {
    return {
        init: function (comboElem) {
            var isBuilt = $(comboElem).data("comboinit") === "true";
            var isTag = $(comboElem).data("role") === "tagsinput"
            if (isTag == undefined)
                isTag = false;
            //console.log('Is Tag', isTag);
            if (!isBuilt) {
                if (isTag) {
                    Combo().getComboList($(comboElem)).then(function (values) {
                        var taglist = new Bloodhound({
                            datumTokenizer: Bloodhound.tokenizers.obj.whitespace('Text'),
                            queryTokenizer: Bloodhound.tokenizers.whitespace,
                            local: values
                        });
                        taglist.initialize();
                        $(comboElem).tagsinput({
                            itemValue: 'Value',
                            itemText: 'Text',
                            typeaheadjs: {
                                displayKey: 'Text',
                                source: taglist.ttAdapter()
                            }
                        });
                        $(comboElem).data("comboinit", "true");
                    });
                }
                else if ($(comboElem).is('select')) {
                    var currentValue = $(comboElem).find(":selected").val();
                    this.initOptions(comboElem).then(function (options) {
                        $(comboElem).html("");
                        $(comboElem).html(options);
                        Combo().setSelectedItem(comboElem, currentValue);
                        $(comboElem).show();
                        $(comboElem).selectpicker('destroy');
                        if ($(comboElem).find('option').length > 15) {
                            $(comboElem).selectpicker('render');
                        }
                        else if ($(comboElem).find('option').length < 2) {
                            $(comboElem).hide();
                        }
                        $(comboElem).data("comboinit", "true");
                    });
                }
                else {
                    var currentValue = $(comboElem).data('value');
                    this.getComboCacheList(comboElem).then(function (result, reject) {
                        if (currentValue !== "") {
                            var item = result.filter(function (x) { return x.Value === currentValue; });

                            if (item === undefined && !isNaN(currentValue)) {
                                var selectVal = parseFloat(currentValue);
                                item = itemList.filter(function (x) { return parseFloat(x.Value) === selectVal; });
                            }
                            if (Array.isArray(item)) {
                                item = item[0];
                            }
                            if (item !== undefined) {
                                $(comboElem).html(item.Text); 
                            }
                        }
                    });
                }

                this.handleComboButtonInfo(comboElem);
                this.handleComboButtonAdd(comboElem);
                this.handleComboButtonEdit(comboElem);
                this.handleComboButtonSearch(comboElem);
            }
        },
        setSelectedItem: function (comboElem, currentValue) {
            $(comboElem).val(currentValue);
            var selectElem = $(comboElem).find('option[selected="selected"]');
            if ($(selectElem).length === 0) {
                $(comboElem).find('option').each(function () {
                    itemValue = $(this).attr('value');
                    if (itemValue === currentValue) {
                        $(this).attr('selected', 'selected');
                        return;
                    }
                    else if (!isNaN(currentValue) && !isNaN(itemValue)) {
                        var comboVal = parseFloat(itemValue);
                        var selectVal = parseFloat(currentValue);
                        if (comboVal === selectVal) {
                            $(this).attr('selected', 'selected');
                            return;
                        }
                    }
                });
            }
        },
        initOptions: function (comboElem) {
            return new Promise((resolve, reject) => {
                var createOptions = function (myList) {
                    markup = "";
                    //console.log('myList: ', myList);
                    //return;
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

                    for (var g = 0; g < groups.length; g++) {
                        if (groups[g].group !== "") {
                            markup += "<optgroup label='" + groups[g].group + "'>";
                        }
                        var groupList = groups[g].listItems;

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
                            markup += groupList[x].Text;
                            markup += "</option>";
                        }

                        if (groups[g].group !== "") {
                            markup += "</optgroup>";
                        }
                    }

                    //console.log('markup returned: ', $(comboElem));
                    resolve(markup);
                };
                this.getComboCacheList(comboElem).then(function (result, reject) {
                    
                    createOptions(result);
                });
            });
        },
        getComboList: function (comboElem) {
            //$.when(this.getComboAjaxList(comboElem), this.getComboCacheList(comboElem))
            //    .done(function (result, cachelist) {
            //        console.log('ajaxList:', result[0]);
            //        console.log('cachelist', cachelist);
            //        try {
            //            itemList = jQuery.parseJSON(result);
            //        }
            //        catch {
            //            itemList = {};
            //        }

            //        console.log('itemList result: ', itemList);
            //        return itemList;
            //    });

            var list = this.getComboCacheList(comboElem);
            console.log('itemList: ', list);

            return list;
        },
        getComboCacheList: function (comboElem) {
            return new Promise((resolve, reject) => {
                var now = new Date();
                var errMsg = "";
                var url = this.getComboUrl(comboElem);
                var comboKeys = $(comboElem).data('combokeys');
                var flushCache = $(comboElem).attr('data-flushcombo');
                var cacheKey = url;

                if (comboKeys) {
                    var data = this.getParms(comboElem, comboKeys, errMsg);
                    cacheKey += $.param(data);
                }

                if (flushCache === "true") {
                    //console.log('clear stored list: ', cacheKey)
                    sessionStorage.removeItem(cacheKey);
                    $(comboElem).removeAttr('data-flushcombo');
                }

                try {
                    var item = JSON.parse(sessionStorage[cacheKey]);
                    //console.log('Cached Item: ', item);
                    if (now.getTime() > item.expire) {
                        //console.log('Combo is expire, removing from local storage', cacheKey);
                        localStorage.removeItem(cacheKey);
                        itemList = undefined;
                    }
                    else {
                        itemList = item.data;
                    }
                    //console.log('Cached itemList: ', itemList);
                }
                catch (e) {
                    itemList = undefined;
                }

                if (itemList) {
                    //console.log('Item list: ', itemList);
                    resolve(itemList);
                }
                else {
                    $.when(this.getComboAjaxList(comboElem))
                        .done(function (result) {
                            try {
                                var item = {
                                    key: cacheKey,
                                    data: result,
                                    expire: now.getTime() + (1000 * 60 * 20) //expire the list after 60 minutes.
                                }
                                sessionStorage[cacheKey] = JSON.stringify(item);
                            }
                            catch (e) {
                                console.log('ex: ', e);
                            }
                            resolve(result);
                        }).fail(function (jqXHR, textStatus) {
                            reject(textStatus);
                        });              
                }
            });
        },
        getComboAjaxList: function (comboElem) {
            var errMsg = "";
            var url = this.getComboUrl(comboElem);
            var fkeys = $(comboElem).data('combokeys');
            if (fkeys != undefined) {
                fkeys = fkeys.replace(/\s/g, "");
            }
            else {
                fkeys = "";
            }
            var data = this.getParms(comboElem, fkeys, errMsg);
            if (errMsg !== "") {
            }
            //console.log(url, data);
            return $.ajax({
                        type: "Get",
                        dataType: "json",
                        url: url,
                        data: data,
                        async: true,
                    });

            
        },
        getComboUrl: function (comboElem) {
            var errMsg = "";
            var url = $(comboElem).data('combourl');
            //var parms = this.getComboParms(comboElem, errMsg);
            //var query = $.param(parms);
            //url += "?" + query;
            return url;
        },        
        getParms: function (comboElem, fkString, errMsg) {
            var parentElem = this.getParentElem(comboElem);
            var parms = {};
            if (fkString === undefined) {
                return parms;
            }
            errMsg = "";
            fkString = fkString.replace(/ /g, "");
            var keys = fkString.split(",");

            for (var i = 0; i < keys.length; i++) {
                if (keys[i] !== "") {                   
                    var elemName = keys[i];
                    var parmName = keys[i];
                    var elemNameArr = keys[i].split("=");
                    if (elemNameArr.length > 1) {
                        parmName = elemNameArr[0];
                        elemName = elemNameArr[1];
                    }

                    if (elemName.indexOf("'") !== -1) {
                        elemName = elemName.replace(/'/g, '');
                        parms[parmName] = elemName;
                    }
                    else {
                        var keyElem = $(parentElem).find('[name="' + elemName + '"]');
                        if (keyElem.length !== 0) {
                            var val = $(keyElem).val();
                            parms[parmName] = val;
                        }
                        else {
                            parms[parmName] = undefined;
                            errMsg += "Key [" + elemName + "] Could not be found combo will not load";
                        }
                    }
                }
            }

            if (errMsg !== "") {
                var url = this.getComboUrl(comboElem);
                console.log(url, errMsg);
                return "";
            }
            return parms;
        },
        getParentElem: function (comboElem) {
            var parentElem = $(comboElem).closest('form');

            if (parentElem.length === 0) {
                parentElem = $(comboElem).closest('tr');
            }

            if (parentElem.length === 0) {
                parentElem = $(comboElem).closest('.row');
            }

            if (parentElem.length === 0) {
                parentElem = $(comboElem).parent();
            }

            return parentElem;
        },
        handleComboButtonInfo: function (comboElem) {
            var inputGroup = $(comboElem).closest('.input-group')
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
                    var data = Combo().getParms(comboElem, fkeys, errMsg);
                    if (url !== undefined) {
                        console.log(url, data, fkeys);
                        OpenPopUpWindow(url, $.param(data));
                    }
                })
            }
        },
        handleComboButtonAdd: function (comboElem) {
            var inputGroup = $(comboElem).closest('.input-group')
            if ($(inputGroup).find('.btnComboAdd').length > 0) {
                $(inputGroup).on('click', 'button.btnComboAdd ', function () {
                    var id = $(inputGroup).attr('id');
                    var url = $(this).data('url');
                    var title = $(this).data('propertytitle');
                    var fkeys = $(this).data('fkeys');
                    if (fkeys != undefined) {
                        fkeys = fkeys.replace(/\s/g, "");
                    }
                    else {
                        fkeys = "";
                    }
                    var errMsg = ""
                    var data = Combo().getParms(comboElem, fkeys, errMsg);
                    var model = $('#comboAdd');
                    var dst = $(model).find('.modal-body');
                    var hdr = $(model).find('.modal-header');
                    console.log(url, data);
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
                            Combo().handleComboButtonCreate(comboElem, model);
                            $('html, body').animate({
                                scrollTop: $(dst).offset().top - 150
                            }, 500)
                        },
                        error: function (data) {
                            dst.html(data);
                            alert('error loading url');
                        }
                    });
                });
            }
        },
        handleComboButtonEdit: function (comboElem) {
            var inputGroup = $(comboElem).closest('.input-group')
            if ($(inputGroup).find('.btnComboEdit').length > 0) {
                $(inputGroup).on('click', 'button.btnComboEdit ', function () {
                    var id = $(this).closest('.input-group').attr('id');
                    var url = $(this).data('url');
                    var title = $(this).data('propertytitle');
                    var fkeys = $(this).data('fkeys');
                    if (fkeys != undefined) {
                        fkeys = fkeys.replace(/\s/g, "");
                    }
                    else {
                        fkeys = "";
                    }
                    var errMsg = ""
                    var data = Combo().getParms(comboElem, fkeys, errMsg);
                    var model = $('#comboEdit');
                    var dst = $(model).find('.modal-body');
                    var hdr = $(model).find('.modal-header');

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
                            Combo().handleComboButtonUpdate(comboElem, model);
                            $('html, body').animate({
                                scrollTop: $(dst).offset().top - 150
                            }, 500)
                        },
                        error: function (data) {
                            dst.html(data);
                            alert('error loading url');
                        }
                    });
                });
            }
        },
        handleComboButtonSearch: function (comboElem) {
            var inputGroup = $(comboElem).closest('.input-group')
            if ($(inputGroup).find('.btnComboSearch').length > 0) {
                $(inputGroup).off('click', 'button.btnComboSearch')
                    .on('click', 'button.btnComboSearch', function () {
                        var id = $(this).closest('.input-group').attr('id');
                        var url = $(this).data('url');
                        var fkeys = $(this).data('fkeys');
                        if (fkeys != undefined) {
                            fkeys = fkeys.replace(/\s/g, "");
                        }
                        else {
                            fkeys = "";
                        }
                        var errMsg = ""
                        var data = Combo().getParms(comboElem, fkeys, errMsg);
                        var model = $('#comboSearch');
                        var dst = $(model).find('.modal-body');

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
                                Combo().handleComboButtonSearchReturn(comboElem, model);
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
            }
        },
        handleComboButtonCreate: function (comboElem, modelElem) {
            $(modelElem).off('click', 'button.btnComboCreate')
                    .on('click', 'button.btnComboCreate', function () {
                    var formElem = $(modelElem).find('form');

                    $(modelElem).Validate(function (errorElem) {
                        if (errorElem === undefined) {
                            var data = $(formElem).SerializeForm();
                            var url = $(formElem).data('urlcreate');

                            $.post(url, data, function (res) {
                                if (res.success === "true") {
                                    var callerElem = $('#' + $(modelElem).data('callerid'));
                                    //var comboElem = $(callerElem).find('select');
                                    $(comboElem).attr('data-flushcombo', 'true');
                                    $(comboElem).data('flushcombo', 'true');
                                    $(comboElem).attr('data-comboinit', 'false');
                                    $(comboElem).data('comboinit', 'false');
                                    $(comboElem).val(res.value);
                                    Combo().init($(comboElem));
                                    $(comboElem).change();
                                    $(modelElem).modal('hide');
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
        },
        handleComboButtonUpdate: function (comboElem, modelElem) {
            $(modelElem).off('click', 'button.btnComboEdit')
                .on('click', 'button.btnComboEdit', function () {
                    var formElem = $(modelElem).find('form');

                    $(modelElem).Validate(function (errorElem) {
                        if (errorElem === undefined) {
                            var data = $(formElem).SerializeForm();
                            var url = $(formElem).data('urlsave');
                            $.post(url, data, function (res) {
                                if (res.success === "true") {
                                    $(comboElem).attr('data-flushcombo', 'true');
                                    $(comboElem).data('flushcombo', 'true');
                                    $(comboElem).attr('data-comboinit', 'false');
                                    $(comboElem).data('comboinit', 'false');
                                    Combo().init(comboElem);
                                    $(modelElem).modal('hide');
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
        },
        handleComboButtonSearchReturn: function (comboElem, modelElem) {
            $(modelElem).off('click', 'button.btnComboSearchSelect')
                .on('click', 'button.btnComboSearchSelect', function () {
                    var tableElem = $(modelElem).find('table');
                    var row = $(tableElem).find('tr.active');

                    var data = $(row).data('entitykey');
                    var url = $(tableElem).data('urlsearchreturn');
                    $.ajax({
                        type: "Post",
                        dataType: "json",
                        url: url,
                        data: data,
                        async: false,
                        success: function (res) {
                            if (res.success === "true") {
                                Combo().init(comboElem);
                                comboElem.val(res.value);
                                setTimeout(function () {
                                    comboElem.change();
                                }, 200)
                                $(modelElem).modal('hide');
                            }

                        },
                        error: function (data) {
                        }
                    });
                });
        },
    };
};
