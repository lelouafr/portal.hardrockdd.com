
//var Table = function () {
//    return {
//        init: function (tableElem) {
//            var id = $(tableElem).attr('id');
//            var isDataTable = $(tableElem).hasClass('dataTable');
//            if (isDataTable) {
//                if ($(tableElem).data("tableinit") === "true")
//                    this.unBindings($(tableElem));

//                $(tableElem).data("tableinit", "true");
//                tableElem = $(tableElem).closest('.dataTables_wrapper');
//            }
//            var isBinding = $(tableElem).data("tableinit") === "true";
//            if (!isBinding) {
//                //console.log("Table.Init.Binding:", $(tableElem));
//                this.initTagId(tableElem);
//                this.handleRowSelect(tableElem);
//                this.handleInputUpdate(tableElem);
//                this.handleInsertRow(tableElem);
//                this.handleCreateRow(tableElem);
//                this.handleDeleteRow(tableElem);
//                this.handleInputButtons(tableElem);
//                //this.initInputPlugins(tableElem);
//                this.handleTabChange(tableElem);
//                $(tableElem).data("tableinit", "true");
//            }
//        },
//        initRow: function (tableRowElem) {
//            this.handleDeleteRow(tableRowElem);
//            //this.initInputPlugins(tableRowElem);
//            this.handleRowSelect(tableRowElem);
//        },
//        initTagId: function (tableElem) {
//            var idx = $(tableElem).GetMaxTabIndex();
//            $(tableElem).find(':input').each(function () {
//                $(this).attr('tabindex', idx);
//                idx++;
//            });
//        },
//        unBindings: function (tableElem) {
//            //console.log("Table.Init.UnBinding:", $(tableElem));
//            $(tableElem).off('click', 'tr:has(td):not(.js-switch)');
//            $(tableElem).off('change', 'tbody tr :Input:not(button):not(thead)');
//            $(tableElem).off('click', 'button.addRow');
//            $(tableElem).off('click', 'button.addRowPopUp');
//            $(tableElem).off('click', 'button.delRow');
//            $(tableElem).off('click', 'tr:has(td):not(.js-switch)');
//        },
//        handleRowSelect: function (tableElem) {
//            $(tableElem)
//                .off('click', 'tr:has(td):not(.js-switch)')
//                .on('click', 'tr:has(td):not(.js-switch)', function (e) {
//                    //console.log('Row Selected');
//                    var refreshPanel = false;
//                    var table = $(this).closest('table');
//                    var eventTargetElem = $(e.target);
//                    var dstId = $(table).data("dstselect");

//                    if (!$(this).hasClass('selected')) {
//                        if (!e.ctrlKey) {
//                            $(table).find('tr.selected').removeClass('selected');
//                        }
//                        $(this).addClass('selected');
//                        refreshPanel = true;
//                    }
//                    else {
//                        return;
//                    }

//                    if (!$(this).data("entitykey")) {
//                        refreshPanel = false;
//                    }
//                    if ($(eventTargetElem).closest('.switchery').length > 0 ||
//                        $(eventTargetElem).hasClass('.switchery')) {
//                        refreshPanel = false;
//                        $(table).find('tr.selected').removeClass('selected');
//                    }

//                    if (refreshPanel) {
//                        var dstId = $(table).data("dstselect");
//                        var url = $(table).data("urlselect");
//                        var validate = $(table).data("dstselectvalidate");

//                        if (!dstId || !url) {

//                        }
//                        else {
//                            var data = $(this).data("entitykey");

//                            var urlArr = url.split(','); 
//                            var dstArr = dstId.split(',');
//                            if ($(this).find(".is-invalid").length !== 0) {
//                                validate = true;
//                            }
//                            $.each(urlArr, function (index, value) {
//                                url = $.trim(value);
//                                var dst = $('#' + dstArr[0].trim());
//                                if (dstArr.length > 0) {
//                                    dst = $('#' + dstArr[index].trim());
//                                    dstId = dstArr[index].trim();
//                                }
//                                var settings = {
//                                    targetId: dstId,
//                                    ajaxUrl: url,
//                                    ajaxData: data,
//                                };
//                                var a = handlePartialAjax(settings);
//                            });
//                        }
//                    }
//                });
//        },
//        handleInputUpdate: function (tableElem) {
//            $(tableElem)
//                .off('change', 'tbody tr :Input:not(button):not(thead)')
//                .on('change', 'tbody tr :Input:not(button):not(thead)', function () {
//                    if (!$(this).parent().hasClass('bs-searchbox')) {
//                        var table = $(this).closest('table');
//                        var row = $(this).closest('tr');
//                        var data = $(row).SerializeTableRow();
//                        var url = $(table).data('urlupdate');
//                        var entitykey = $(row).data('entitykey');

//                        var panel = $(table).closest('.panel');
//                        if (url != undefined) {
//                            //console.log('Table row updated', $(this), url, data);
//                            $.ajax({
//                                type: "Post",
//                                dataType: "json",
//                                url: url,
//                                async: true,
//                                data: data,
//                                success: function (res) {
//                                    if (entitykey) {
//                                        if (!panel) {
//                                            panel = table;
//                                        }
//                                        $(document).find('[data-entitykey="' + entitykey + '"]').each(function () {
//                                            updateElementValues(this, res.model);
//                                        });
//                                    }
//                                    else {
//                                        updateElementValues(row, res.model);
//                                    }
//                                    var focused = $(':focus');
//                                    focused.select();

//                                },
//                                error: function (res) {
//                                    //swal.fire({
//                                    //    title: 'Error',
//                                    //    html: res.responseText,
//                                    //    icon: 'error',
//                                    //    buttons: {
//                                    //        confirm: {
//                                    //            text: 'Close',
//                                    //            value: true,
//                                    //            visible: true,
//                                    //            className: 'btn btn-success',
//                                    //            closeModal: true
//                                    //        }
//                                    //    }
//                                    //});
//                                }
//                            });
//                        }
//                    }
//                });
//        },
//        handleInsertRow: function (tableElem) {
//            $(tableElem)
//                .off('click', 'button.addRow')
//                .on('click', 'button.addRow', function () {
//                    var btnAdd = this;
//                    var tbody = $(tableElem).find('tbody');
//                    var table = $(tbody).closest('table');
//                    var addrowlocation = $(btnAdd).data('addrowlocation');

//                    if (!addrowlocation) {
//                        addrowlocation = "bottom";
//                    }

//                    var row = $(this).closest('tr');
//                    //var tableid = $(table).attr('id');
//                    var data = table.data('entitykey');
//                    //var viewId = table.data('viewid');
//                    //var istreegrid = table.data('istreegrid');
//                    var url = $(table).data('urlinsert');
//                    //var deleteurl = $(table).data('urldelete');
//                    var token = $(row).find('[name="__RequestVerificationToken"]');
//                    if (token.length != 0) {
//                        data += '&__RequestVerificationToken=' + token.val();
//                    }

//                    //console.log('Table row added', table, tableid, url, data);
//                    $(btnAdd).SetButtonBusy();
//                    $.ajax({
//                        type: "Get",
//                        dataType: "html",
//                        url: url,
//                        //headers: { 'DefinitionViewId': viewId, 'deleteurl': deleteurl, 'IsTreeGrid': istreegrid },
//                        data: data,
//                        success: function (data) {
//                            if (!data.includes("Internal Server Error")) {
//                                var newRow = $(data);
//                                $(newRow).data("isnewrow", "true");

//                                var firstRow = $(tbody).find('tr:first');
//                                if (addrowlocation == "top" && firstRow.length !== 0) {
//                                    $(newRow).insertBefore(firstRow);
//                                }
//                                else {
//                                    tbody.append(newRow);
//                                }

//                                var isDataTable = $(table).hasClass('dataTable');
//                                if (isDataTable) {
//                                    $(table).DataTable().row.add($(data));
//                                }

//                                $(tbody).find('#emptytablerow').remove();
//                                Table().initRow($(newRow));

//                                newRow.click();

//                                $(btnAdd).SetButtonNotBusy();
//                                $('html, body').animate({
//                                    scrollTop: $(newRow).offset().top
//                                }, 200);

//                            }
//                            else {
//                                var tr = $("<tr></tr>");
//                                var td = $('<td colspan="1">/td>');

//                                $(data).appendTo(td);
//                                $(td).appendTo(tr);

//                                tbody.append(tr);
//                            }
//                        },
//                        error: function (XMLHttpRequest, textStatus, errorThrown) {
//                            $(btnAdd).SetButtonNotBusy();
//                        }
//                    });
//                });
//        },
//        handleCreateRow: function (tableElem) {
//            $(tableElem)
//                .off('click', 'button.addRowPopUp')
//                .on('click', 'button.addRowPopUp', function () {
//                    var button = this;
//                    var tbody = $(tableElem).find('tbody');
//                    var table = $(tbody).closest('table');
//                    var addrowlocation = $(btnAdd).data('addrowlocation');

//                    if (!addrowlocation) {
//                        addrowlocation = "bottom";
//                    }
//                    var modal = $('#addRowPupUp');
//                    var dst = $('#addRowPupUp').find('.modal-content');

//                    var row = $(this).closest('tr');
//                    var tableId = $(table).attr('id');
//                    var data = table.data('entitykey');
//                    //var viewId = table.data('viewid');
//                    //var istreegrid = table.data('istreegrid');
//                    var url = $(table).data('urlcreate');
//                    //var deleteurl = $(table).data('urldelete');
//                    var token = $(row).find('[name="__RequestVerificationToken"]');
//                    if (token.length != 0) {
//                        data += '&__RequestVerificationToken=' + token.val();
//                    }

//                    //console.log('Table row added', table, tableid, url, data);
//                    $(button).SetButtonBusy();
//                    $.ajax({
//                        type: "Get",
//                        dataType: "html",
//                        url: url,
//                        //headers: { 'DefinitionViewId': viewId, 'deleteurl': deleteurl, 'IsTreeGrid': istreegrid },
//                        data: data,
//                        success: function (res) {
//                            $(dst).html(res);
//                            $(modal).attr("data-tableid", tableId);
//                            $(button).SetButtonNotBusy();

//                            handleInsertRow(modal);
//                        },
//                        error: function (XMLHttpRequest, textStatus, errorThrown) {
//                            $(button).SetButtonNotBusy();
//                        }
//                    });
//                });
//        },
//        handleDeleteRow: function (tableElem) {
//            $(tableElem)
//                .off('click', 'button.delRow')
//                .on('click', 'button.delRow', function (e) {
//                    e.stopPropagation();
//                    var isDataTable = $(tableElem).hasClass('dataTables_wrapper');
//                    var button = this;
//                    var table = $(this).closest('table');
//                    var tableid = $(table).attr('id');
//                    var row = $(this).closest('tr');
//                    var data = row.data('entitykey');
//                    var delentitykey = $(this).data('entitykey');

//                    $(button).SetButtonBusy();
//                    if (!delentitykey) {
//                        delentitykey = data;
//                    }

//                    var token = $(row).find('[name="__RequestVerificationToken"]');
//                    if (token.length == 0) {
//                        token = $(row).closest('[name="__RequestVerificationToken"]');
//                    }
//                    if (token.length == 0) {
//                        token = $(document).find('[name="__RequestVerificationToken"]');
//                    }
//                    if (token.length != 0) {
//                        data += '&__RequestVerificationToken=' + token.val();
//                    }
//                    var url = $(table).data('urldelete');
//                    //console.log('Table row deleted', url, data);


//                    $.ajax({
//                        type: "post",
//                        url: url,
//                        data: data,
//                        success: function (res) {
//                            if (res.success === "true") {
//                                $(button).SetButtonNotBusy();
//                                if (isDataTable) {
//                                    var dt = $(table).dataTable();
//                                    var dtRow = dt.api().row($(row));
//                                    dtRow.remove().draw();
//                                }
//                                $('#' + tableid + ' tbody tr').each(function () {
//                                    var rowEntityKey = $(this).data('entitykey');
//                                    if (rowEntityKey != undefined) {
//                                        if (rowEntityKey.indexOf(delentitykey) != -1) {
//                                            this.remove();
//                                        }
//                                    }
//                                });

//                                var dstId = $(table).data("dstselect");
//                                var url = $(table).data("urlselect");
//                                if (!dstId || !url) {
//                                    return;
//                                }
//                                var dst = $('#' + dstId);
//                                dst.html("");
//                            }
//                            else {
//                                console.log(res);
//                            }
//                        },
//                        error: function (res) {
//                            $(button).SetButtonNotBusy();
//                            console.log(res);
//                        }
//                    });

//                });
//        },
//        initInputPlugins: function (tableElem) {
//            $(tableElem).find('textarea').each(function () {
//                var elem = this;
//                var scrollHeight = $(elem).prop('scrollHeight');
//                var topBorderHeight = parseFloat($(elem).css("borderTopWidth"));
//                var bottomBorderHeight = parseFloat($(elem).css("borderBottomWidth"));
//                var insideHeight = scrollHeight + topBorderHeight + bottomBorderHeight;
//                var boxHeight = $(elem).outerHeight() + 1;

//                //console.log(boxHeight, insideHeight);
//                if (boxHeight < insideHeight && boxHeight > 45) {
//                    $(elem).height(insideHeight + 1);
//                }
//                else {
//                    $(elem).height(45);
//                }
//            });

//            $(tableElem).find('[data-selectinit="false"]').each(function () {
//                var elem = this;
//                if ($(elem).attr("data-selectinit") !== "true" && $(elem).is('select')) {
//                    if ($(elem).find('option').length > 15)
//                        $(elem).select2();
//                    $(elem).attr("data-selectinit", "true");
//                }
//            });

//            $(tableElem).find('[data-datepickerid]').each(function () {
//                datepicker_options = {
//                    altFormat: "mm/dd/yyyy",
//                    dateFormat: "mm/dd/yy",
//                    yearRange: "1000:3000",
//                    changeMonth: true,
//                    changeYear: true,
//                    todayHighlight: true
//                };
//                inputmask_options = {
//                    mask: "99/99/9999",
//                    alias: "date",
//                    placeholder: "mm/dd/yyyy",
//                    insertMode: false
//                };

//                $(this).datepicker(datepicker_options)
//                    .off('change')
//                    .on('change', function () {
//                        $('.datepicker').hide();
//                        date = $(this).datepicker('getDate');
//                        date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
//                    });
//            });

//            $(tableElem).find('[data-numberformat="percent"]').each(function () {
//                var option = {
//                    alias: "percentage"
//                }
//                $(this).inputmask(option);
//            });

//            $(tableElem).find('[data-switcheryid]').each(function () {
//                $(this).InitSwitchery();
//            });
//        },
//        selectFirstRow: function (tableElem) {
//            var firstRow = $(tableElem).find('tbody').find('tr:first');
//            setTimeout(function () {
//                $(firstRow).click();
//            }, 250);
//        },
//        handleTabChange: function (tableElem) {
//            $(document)
//                .off('shown.bs.tab')
//                .on('shown.bs.tab', function (e) {
//                    $(tableElem).find(".dataTable").each(function () {
//                        $(this).DataTable().columns.adjust();
//                        $(this).find('tr.selected').removeClass('selected');
//                    });
//                });
//        },
//        handleReload: function (tableElem) {

//        },
//        setTableHeight: function (tableElem) {
//            var isDataTable = $(tableElem).hasClass('dataTables_wrapper');
//            if (isDataTable) {
//                var dt = $(table).dataTable();
                
//            }
//        },
//        handleInputButtons: function (tableElem) {
//            $(tableElem).off('click', 'button.btnEye')
//                .on('click', 'button.btnEye', function () {
//                    //console.log('Eye was clicked');
//                    var icon = $(this).find('i');
//                    if ($(icon).hasClass('fa-eye')) {
//                        var group = $(this).closest('.input-group');
//                        if (group) {
//                            var input = $(group).find('input');
//                            $(input).attr('type', 'text');
//                        }
//                        $(icon).removeClass('fa-eye').addClass('fa-eye-slash');
//                    }
//                    else if ($(icon).hasClass('fa-eye-slash')) {
//                        var group = $(this).closest('.input-group');
//                        if (group) {
//                            var input = $(group).find('input');
//                            $(input).attr('type', 'password');
//                        }
//                        $(icon).removeClass('fa-eye-slash').addClass('fa-eye');
//                    }
//                    $(input).attr('originaltype', 'password');

//                });
//        }
//    }
//}




