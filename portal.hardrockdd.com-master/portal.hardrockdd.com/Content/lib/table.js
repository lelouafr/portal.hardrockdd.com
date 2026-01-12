
var Table = function () {
    return {
        init: function (tableElem) {
            var isDataTable = $(tableElem).hasClass('dataTable');
            if (isDataTable) {
                if ($(tableElem).data("tableinit") === "true")
                    this.unBindings($(tableElem));
                $(tableElem).data("tableinit", "false");
                tableElem = $(tableElem).closest('.dataTables_wrapper');
            }
            var isBinding = $(tableElem).data("tableinit") === "true";
            if (!isBinding) {
                //console.log("Table.Init:", $(tableElem));
                this.initTagId(tableElem);
                this.initColumnSearch(tableElem);
                this.handleRowSelect(tableElem);
                this.handleInputUpdate(tableElem);
                this.handleInsertRow(tableElem);
                this.handleCreateRow(tableElem);
                this.handleDeleteRow(tableElem);
                this.handleInputButtons(tableElem);
                this.initInputPlugins(tableElem);
                this.handleTabChange(tableElem);
                this.setTableHeight(tableElem);
                $(tableElem).data("tableinit", "true");
            }
        },
        initRow: function (tableRowElem) {
            this.handleDeleteRow(tableRowElem);
            this.initInputPlugins(tableRowElem);
            this.handleRowSelect(tableRowElem);
        },
        initTagId: function (tableElem) {
            var idx = $(tableElem).GetMaxTabIndex();
            $(tableElem).find(':input').each(function () {
                $(this).attr('tabindex', idx);
                idx++;
            });
        },
        initInputPlugins: function (tableElem) {
            $(tableElem).find('textarea').each(function () {
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

            $(tableElem).find('[data-selectinit="false"]').each(function () {
                Combo().init($(this));
                $(this).attr("data-selectinit", "true");
            });

            $(tableElem).find('[data-datepickerid]').each(function () {
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

            $(tableElem).find('[data-numberformat="percent"]').each(function () {
                var option = {
                    alias: "percentage"
                }
                $(this).inputmask(option);
            });

            $(tableElem).find('[data-switcheryid]:not(.checkboxsearch)').each(function () {
                $(this).InitSwitchery();
            });
            $(tableElem).find(':checkbox:not(.checkboxsearch)').each(function () {
                $(this).InitSwitchery();
            });
        },
        initColumnSearch: function (tableElem) {
            var initSearchTH = function (column, searchRow) {
                var colIdx = column.index();
                //console.log('Col Idx', colIdx);
                var dataContainer = '#' + $(tableElem).attr('id');
                var filterDefault = $(column.header()).data('datatable-filter-value');
                var filterId = "table_col_flt_" + $(tableElem).attr("id") + '_' + colIdx;
                var tableId = $(tableElem).attr("id");
                var cell = $(searchRow).find('th:eq(' + colIdx + ')');
                if (cell.length == 0) {
                    cell = $('<th style="padding-right: 0px;" data-colidx="' + colIdx + '"></th>').appendTo(searchRow);
                }
                $(cell).empty();
                var result = {
                    cell,
                    filterDefault,
                    filterId,
                    dataContainer,
                    tableId,
                }
                return result;
            }
            var initSelectFilter = function (column, settings) {
                var dataCnt = column.cache('search').unique().length;
                var select = $('<select id="' + settings.filterId + '"  class="selectpicker form-control input-xs" data-container="' + settings.dataContainer + '" data-none-selected-text="Search" data-style="btn-xs btn-white"></select>');
                //console.log('dataCnt', dataCnt);
                if (dataCnt > 10) {
                    $(select).data("live-search", "true");
                }
                var opt = '<option value="" selected ></option>';
                select.append(opt);
                column.cache('search').unique().sort().each(function (d, j) {

                    if (d !== null && d != "") {
                        opt = '<option value="' + d + '">' + d + '</option>';
                        if (settings.filterDefault != undefined) {
                            if (d.trim() == settings.filterDefault) {
                                opt = '<option value="' + d + '" selected>' + d + '</option>';
                            }
                        }
                        select.append(opt);
                    }
                });
                if (settings.filterDefault != undefined) {
                    console.log(settings.filterDefault);
                    var val = "^" + settings.filterDefault + "$";

                    column.search(val, true, false, true).draw();
                }
                $(select).appendTo($(settings.cell))
                    .on('changed.bs.select', function (e) {
                        var val = $(this).val();
                        column.search("^" + val + "$", true, false).draw();
                    });
                $(select).selectpicker('render');
            }
            var initMultiSelectFilter = function (column, settings) {
                var dataCnt = column.cache('search').unique().length;
                var select = $('<select id="' + settings.filterId + '" data-allow-clear="true" class="selectpicker form-control input-xs" data-container="' + settings.dataContainer + '" data-none-selected-text="Search" data-style="btn-xs btn-white" multiple></select>');
                if (dataCnt > 10) {
                    $(select).data("live-search", "true");
                }
                //var opt = '<option value="">search</option>';
                //select.append(opt);
                column.cache('search').unique().sort().each(function (d, j) {
                    if (d !== null && d != "") {
                        opt = '<option value="' + d + '">' + d + '</option>';
                        if (settings.filterDefault != undefined) {
                            var defaults = settings.filterDefault.split('|');
                            $.each(defaults, function (index, value) {
                                if (value === d) {
                                    opt = '<option value="' + d + '" selected>' + d + '</option>';
                                }
                            });
                        }
                        select.append(opt);
                    }
                });
                if (settings.filterDefault != undefined) {
                    console.log(settings.filterDefault);
                    var val = "^" + settings.filterDefault + "$";
                    
                    column.search(val, true, false, true).draw();
                }

                $(select).appendTo($(settings.cell))
                    .on('changed.bs.select', function (e) {
                        var val = $(this).val();
                        $.each(val, function (index, item) {
                            val[index] = "^" + val[index] + "$";
                        });
                        var fVal = val.join("|")
                        column.search(fVal, true, false, true).draw();
                    });
                $(select).selectpicker('render');
            }
            var initInputFilter = function (column, settings) {
                var input = $('<input id="' + settings.filterId + '" type="text" class="form-control input-xs" placeholder="Search"/>');

                $(input).appendTo($(settings.cell))
                    .on('keyup change', function (e) {
                        var value = this.value;
                        if (column.search() !== value) {
                            column.search("" + value, true, false, true).draw();
                        }
                    });
            }
            var initDateRangeFilter = function (column, settings) {
                var colIdx = column[0][0];
                var input = $('<input type="text" id="' + settings.filterId + '" class="form-control input-xs" value="" placeholder="click to select the date range" />');
                $(input).appendTo($(settings.cell))
                    .on('change', function () {
                        column.draw();
                    });
                $(input).daterangepicker({
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
                },
                    function (start, end, label) {
                        $('#' + settings.filterId + ' span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
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
                    function (dtsettings, data, dataIndex) {
                        //console.log('nTable.id', dtsettings.nTable.id, 'tableId', settings.tableId, 'settings', dtsettings);
                        //if (settings.nTable.id == settings.tableId) {
                            var fltVal = $('#' + settings.filterId).val();
                        //console.log('fltVal', fltVal);
                            if (fltVal !== '' && fltVal !== undefined) {
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
                        //}
                        return true;
                    }
                );
            }
            var initCheckBoxFilter = function (column, settings) {
                var input = $('<input id="' + settings.filterId + '" type="checkbox" class="form-control input-xs checkboxsearch" />');
                $(input).prop('indeterminate', true)
                $(input).data('checked', 1);
                $(input).appendTo($(settings.cell))
                    .on('click', function (e) {
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

            var isDataTable = $(tableElem).hasClass('dataTables_wrapper');
            if (isDataTable) {
                var table = $(tableElem).find('table');
                var thead = $(table).find('thead')[0];
                var dt = $(table).DataTable();
                //console.log('DT Rows', dt.data().count());
                if (dt.data().count() < 20) {
                    //console.log('To few records to warrent filter!');
                    return;
                }

                //Check if any filters have been defined
                if ($(thead).find('[data-datatable-filter-type="select"]').length === 0 &&
                    $(thead).find('[data-datatable-filter-type="multiselect"]').length === 0 &&
                    $(thead).find('[data-datatable-filter-type="daterange"]').length === 0 &&
                    $(thead).find('[data-datatable-filter-type="input"]').length === 0 &&
                    $(thead).find('[data-datatable-filter-type="checkbox"]').length === 0) {
                    console.log('No Filters Found');
                    return;
                }

                //Create The Search Row if not exists
                var searchRow = $(thead).find('tr:eq(1)');
                if (searchRow.length == 0) {
                    searchRow = $('<tr></tr>').appendTo(thead);
                }

                //Loop Through all Columns
                var columns = $(table).DataTable().columns();
                columns.every(function () {
                    var col = this;
                    if (col.visible()) {
                        var filterType = $(col.header()).data('datatable-filter-type');
                        var settings = initSearchTH(col, searchRow);
                        switch (filterType) {
                            case "select": initSelectFilter(col, settings); break;
                            case "multiselect": initMultiSelectFilter(col, settings); break;
                            case "input": initInputFilter(col, settings); break;
                            case "daterange": initDateRangeFilter(col, settings); break;
                            case "checkbox": initCheckBoxFilter(col, settings); break;
                            default:
                        }
                    }
                });
            }
        },
        isUpdating: function (tableElem, callback) {
            setTimeout(function () {
                if ($(tableElem).attr('data-fieldupdating') == "true") {
                    Table().isUpdating(tableElem, callback);
                }
                else {
                    callback();
                }
            }, 100);
        },
        unBindings: function (tableElem) {
            //console.log("Table.Init.UnBinding:", $(tableElem));
            $(tableElem).off('click', 'tr:has(td):not(.js-switch)');
            $(tableElem).off('change', 'tbody tr :Input:not(button):not(thead)');
            $(tableElem).off('click', 'button.addRow');
            $(tableElem).off('click', 'button.addRowPopUp');
            $(tableElem).off('click', 'button.delRow');
            $(tableElem).off('click', 'tr:has(td):not(.js-switch)');
        },
        handleRowSelect: function (tableElem) {
            $(tableElem)
                .off('click', 'tr:has(td):not(.js-switch)')
                .on('click', 'tr:has(td):not(.js-switch)', function (e) {
                    //console.log('Row Selected NEW CODE');
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
                    }
                    else {
                        return;
                    }

                    if (!$(this).data("entitykey")) {
                        refreshPanel = false;
                    }
                    if ($(eventTargetElem).closest('.switchery').length > 0 ||
                        $(eventTargetElem).hasClass('.switchery')) {
                        refreshPanel = false;
                        $(table).find('tr.selected').removeClass('selected');
                    }

                    if (refreshPanel) {
                        Table().rowSelect(this);
                    }
                });
        },
        rowSelect: function (row) {
            var timer = 0;
            var table = $(row).closest('table');
            var dstId = $(table).data("dstselect");
            var url = $(table).data("urlselect");
            var validate = $(table).data("dstselectvalidate");
            var rowEntityKey = $(row).data('entitykey');
            if (!dstId || !url) {

            }
            else {
                var data = $(row).data("entitykey");

                var urlArr = url.split(',');
                var dstArr = dstId.split(',');
                if ($(row).find(".is-invalid").length !== 0) {
                    validate = true;
                }
                $.each(urlArr, function (index, value) {
                    url = $.trim(value);
                    var dst = $('#' + dstArr[0].trim());
                    if (dstArr.length > 0) {
                        dst = $('#' + dstArr[index].trim());
                        dstId = dstArr[index].trim();
                    }
                    var settings = {
                        targetId: dstId,
                        ajaxUrl: url,
                        ajaxData: data,
                    };
                    $(dst).data('relatedentitykey', rowEntityKey);
                    $(dst).attr('data-relatedentitykey', rowEntityKey);
                    var a = handlePartialAjax(settings);
                });
            }
        },
        handleInputUpdate: function (tableElem) {
            $(tableElem)
                .off('change', 'tbody tr :Input:not(button):not(thead)')
                .on('change', 'tbody tr :Input:not(button):not(thead)', function () {
                    if (!$(this).parent().hasClass('bs-searchbox')) {
                        var table = $(this).closest('table');
                        var isDataTable = $(table).hasClass('dataTable');
                        var row = $(this).closest('tr');
                        $(table).data('fieldupdating', "true");
                        $(table).attr('data-fieldupdating', "true");

                        var data = $(row).SerializeTableRow();
                        if (isDataTable == true) {
                            var dt = $(table).DataTable();
                            var dtRow = dt.row(row);
                            var dtRowData = dtRow.data();

                            $.each(data, function (key, value) {
                                dtRowData[key] = value;
                            });
                            data = dtRowData;
                        }

                        var url = $(table).data('urlupdate');
                        var entitykey = $(row).data('entitykey');

                        var panel = $(table).closest('.panel');
                        if (url != undefined) {
                            //console.log('Table row updated', $(this), url, data);
                            var linkedPanels = $(panel).data('reloadpanels');
                            //if (linkedPanels) {
                            //    var urlArr = linkedPanels.split(',');
                            //    $.each(urlArr, function (index, value) {
                            //        var linkedPanel = $('#' + value);
                            //        var targetBody = $(linkedPanel).find('.panel-body');
                            //        var spinnerHtml = '<div class="panel-loader"><span class="spinner-small"></span></div>';
                            //        $(linkedPanel).addClass('panel-loading');
                            //        $(targetBody).append(spinnerHtml);
                            //    });
                            //}

                            $.ajax({
                                type: "Post",
                                dataType: "json",
                                url: url,
                                async: true,
                                data: data,
                                success: function (res) {
                                    if (entitykey) {
                                        $(this).UpdateElementValuesByKey(entitykey, res.model);
                                    }
                                    else {
                                        Table().updateRowValues(row, res.modal)
                                    }
                                    $(row).data('fieldupdating', "false");
                                    $(row).attr('data-fieldupdating', "false");
                                    
                                    var focused = $(':focus');
                                    focused.select();
                                    if (linkedPanels) {
                                        Table().rowSelect(row);
                                        //var urlArr = linkedPanels.split(',');
                                        //$.each(urlArr, function (index, value) {
                                        //    var linkedPanel = $('#' + value);
                                        //    var reload = $(linkedPanel).find('[data-click=panel-reload]');
                                        //    $(reload).click();
                                        //});
                                    }
                                },
                                error: function (res) {
                                    //swal.fire({
                                    //    title: 'Error',
                                    //    html: res.responseText,
                                    //    icon: 'error',
                                    //    buttons: {
                                    //        confirm: {
                                    //            text: 'Close',
                                    //            value: true,
                                    //            visible: true,
                                    //            className: 'btn btn-success',
                                    //            closeModal: true
                                    //        }
                                    //    }
                                    //});
                                    console.log(res);
                                    $(table).data('fieldupdating', "false");
                                    $(table).attr('data-fieldupdating', "false");
                                }
                            });
                        }
                    }
                });
        },
        handleInsertRow: function (tableElem) {
            $(tableElem)
                .off('click', 'button.addRow')
                .on('click', 'button.addRow', function () {
                    var btnAdd = this;
                    var tbody = $(tableElem).find('tbody');
                    var table = $(tbody).closest('table');
                    var addrowlocation = $(btnAdd).data('addrowlocation');

                    if (!addrowlocation) {
                        addrowlocation = "bottom";
                    }

                    var row = $(this).closest('tr');
                    //var tableid = $(table).attr('id');
                    var data = table.data('tablekey');
                    //var viewId = table.data('viewid');
                    //var istreegrid = table.data('istreegrid');
                    var url = $(table).data('urlcreate');
                    //var deleteurl = $(table).data('urldelete');
                    var token = $(row).find('[name="__RequestVerificationToken"]');
                    if (token.length != 0) {
                        data += '&__RequestVerificationToken=' + token.val();
                    }

                    //console.log('Table row added', table, url, data);
                    $(btnAdd).SetButtonBusy();
                    if (url == undefined || data == undefined) {
                        return;
                    }
                    $.ajax({
                        type: "Get",
                        dataType: "html",
                        url: url,
                        //headers: { 'DefinitionViewId': viewId, 'deleteurl': deleteurl, 'IsTreeGrid': istreegrid },
                        data: data,
                        success: function (data) {
                            var html = data + "";
                            try {
                                var obj = $.parseJSON(data);
                            } catch (e) {
                                var obj = undefined;
                            }
                            if (!html.includes("Internal Server Error")) {
                                if (obj) {
                                    var isDataTable = $(table).hasClass('dataTable');
                                    if (isDataTable) {
                                        var row = $(table).DataTable().row.add($(obj.data));
                                        row.draw();
                                    }
                                }
                                else {
                                    var newRow = $(html);
                                    $(newRow).data("isnewrow", "true");

                                    var firstRow = $(tbody).find('tr:first');
                                    if (addrowlocation == "top" && firstRow.length !== 0) {
                                        $(newRow).insertBefore(firstRow);
                                    }
                                    else {
                                        tbody.append(newRow);
                                    }

                                    var isDataTable = $(table).hasClass('dataTable');
                                    if (isDataTable) {
                                        console.log(html);
                                        $(table).DataTable().row.add($(html));
                                    }

                                    $(tbody).find('#emptytablerow').remove();
                                    Table().initRow($(newRow));

                                    newRow.click();
                                    $('html, body').animate({
                                        scrollTop: $(newRow).offset().top
                                    }, 200);
                                }

                                $(btnAdd).SetButtonNotBusy();

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
                            $(btnAdd).SetButtonNotBusy();
                        }
                    });
                });
        },
        handleCreateRow: function (tableElem) {
            $(tableElem)
                .off('click', 'button.addRowPopUp')
                .on('click', 'button.addRowPopUp', function () {
                    var button = this;
                    var tbody = $(tableElem).find('tbody');
                    var table = $(tbody).closest('table');
                    var addrowlocation = $(button).data('addrowlocation');

                    if (!addrowlocation) {
                        addrowlocation = "bottom";
                    }
                    var modal = $('#addRowPopUp');
                    var dst = $('#addRowPopUp').find('.modal-content');

                    var row = $(this).closest('tr');
                    var tableId = $(table).attr('id');
                    var data = table.data('entitykey');
                    //var viewId = table.data('viewid');
                    //var istreegrid = table.data('istreegrid');
                    var url = $(table).data('urlcreate');
                    //var deleteurl = $(table).data('urldelete');
                    var token = $(row).find('[name="__RequestVerificationToken"]');
                    if (token.length != 0) {
                        data += '&__RequestVerificationToken=' + token.val();
                    }

                    console.log('Table create model', modal, dst, url, data);
                    $(button).SetButtonBusy();
                    $.ajax({
                        type: "Get",
                        dataType: "html",
                        url: url,
                        //headers: { 'DefinitionViewId': viewId, 'deleteurl': deleteurl, 'IsTreeGrid': istreegrid },
                        data: data,
                        success: function (res) {
                            $(dst).html(res);
                            $(modal).attr("data-tableid", tableId);
                            $(button).SetButtonNotBusy();
                            $(modal).modal('show');
                            Table().handleCreateModal(modal);
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            $(button).SetButtonNotBusy();
                        }
                    });
                });
        },
        handleCreateModal: function (modalElem) {
            $(modalElem)
                .off('click', 'button.create')
                .on('click', 'button.create', function () {
                    var button = this;
                    var modal = $(modalElem);
                    var tableId = $(modal).data('tableid')
                    var tableElem = $('#' + tableId);
                    var tbody = $(tableElem).find('tbody');
                    var table = $(tbody).closest('table');

                    var data = $(modal).SerializeForm();
                    var url = $(table).data('urlcreate');

                    console.log('Table create model', modal, url, data);
                    $(button).SetButtonBusy();
                    $.ajax({
                        type: "Post",
                        dataType: "json",
                        url: url,
                        data: data,
                        success: function (res) {
                            console.log(res.url);
                            $(button).SetButtonNotBusy();
                            if (res.success === "true") {
                                $(modalElem).modal('hide');
                                $.ajax({
                                    type: "Get",
                                    dataType: "html",
                                    url: res.url,
                                    success: function (data) {
                                        if (!data.includes("Internal Server Error")) {
                                            var newRow = $(data);
                                            $(newRow).data("isnewrow", "true");
                                            tbody.append(newRow);
                                            var isDataTable = $(table).hasClass('dataTable');
                                            if (isDataTable) {
                                                $(table).DataTable().row.add($(data));
                                            }

                                            $(tbody).find('#emptytablerow').remove();
                                            Table().initRow($(newRow));

                                            newRow.click();

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
                                        $(btnAdd).SetButtonNotBusy();
                                    }
                                });
                            }


                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            $(button).SetButtonNotBusy();
                        }
                    });
                });
        },
        handleDeleteRow: function (tableElem) {
            $(tableElem)
                .off('click', 'button.delRow')
                .on('click', 'button.delRow', function (e) {
                    e.stopPropagation();
                    var isDataTable = $(tableElem).hasClass('dataTables_wrapper');
                    var button = this;
                    var table = $(this).closest('table');
                    var tableid = $(table).attr('id');
                    var row = $(this).closest('tr');
                    var data = row.data('entitykey');
                    var delentitykey = $(this).data('delentitykey');

                    $(button).SetButtonBusy();
                    if (!delentitykey) {
                        delentitykey = data;
                    }
                    else {
                        delentitykey = delentitykey.trim();
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
                    //console.log('Table row deleted', url, data);


                    $.ajax({
                        type: "post",
                        url: url,
                        data: data,
                        success: function (res) {
                            if (res.success === "true") {
                                $(button).SetButtonNotBusy();
                                if (isDataTable) {
                                    var dt = $(table).dataTable();
                                    var dtRow = dt.api().row($(row));
                                    dtRow.remove().draw();
                                }
                                $('#' + tableid + ' tbody tr').each(function () {
                                    var rowEntityKey = $(this).data('entitykey').trim();
                                    if (rowEntityKey !== undefined) {
                                        //console.log('Del rowEntityKey', '"' + rowEntityKey + '"', '"' + delentitykey + '"', rowEntityKey.indexOf(delentitykey));
                                        if (rowEntityKey.indexOf(delentitykey) != -1) {

                                           // console.log('Del row', rowEntityKey, delentitykey, $(this));
                                            $(this).remove();
                                        }
                                    }
                                });

                                var dstId = $(table).data("dstselect");
                                var url = $(table).data("urlselect");
                                if (!dstId || !url) {
                                    return;
                                }
                                var dst = $('#' + dstId);
                                if ($(dst).data('relatedentitykey') == delentitykey) {
                                    dst.html("");
                                }
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
        },
        handleTabChange: function (tableElem) {
            $(document)
                .off('shown.bs.tab')
                .on('shown.bs.tab', function (e) {
                    $(tableElem).find(".dataTable").each(function () {
                        $(this).DataTable().columns.adjust();
                        $(this).find('tr.selected').removeClass('selected');
                    });
                });
        },
        handleReload: function (tableElem) {

        },
        handleInputButtons: function (tableElem) {
            $(tableElem).off('click', 'button.btnEye')
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
        selectFirstRow: function (tableElem) {
            var firstRow = $(tableElem).find('tbody').find('tr:first');
            setTimeout(function () {
                $(firstRow).click();
            }, 250);
        },
        setTableHeight: function (tableElem) {
            console.log('setTableHeight');
            var bodyH = $(window).height();
            var contentH = $('.content').height();
            var pageLength = $(tableElem).find('.dataTables_length');
            var tableFilter = $(tableElem).find('.dataTables_filter');
            var tableHeader = $(tableElem).find('.dataTables_scrollHead'); 

            //console.log('contentH', contentH, 'bodyH', bodyH);
            var tbody = $(tableElem).find('.dataTables_scrollBody');
            var row = $(tbody).find('tbody>tr')[0];
            var rowH = $(row).height();
            var rowCnt = $(tbody).find('tr').length + 1;

            if ($(tbody).length == 0) {
                return;
            }
            var h = rowH * rowCnt;// $(tbody).height();
            var cnt = 0;

            if (h > bodyH)
                h = bodyH + 50;
            bodyH = bodyH + 10;
            console.log('resize:', h, contentH, bodyH, rowH, rowCnt, $(row));
            while ((contentH > bodyH || h >= contentH) && cnt <= 200 ) {
                if (contentH > bodyH || h >= contentH) {
                    h = h - rowH;
                    cnt++;
                    $(tbody).css('max-height', h);
                    $(tbody).css('height', h);
                    contentH = $('.content').height();
                    console.log('resize:', h, contentH, bodyH, rowH, rowCnt, $(row));

                }
            }

            //while (contentH < bodyH && cnt <= 200) {
            //    h = h + 5;
            //    cnt++;
            //    $(tbody).css('max-height', h);
            //    $(tbody).css('height', h);
            //    contentH = $('.content').height();
            //}

            if ($(pageLength).length !== 0 && $(pageLength).is(':visible')) {
                //console.log('pageLength', pageLength.height());
                //h = h - $(pageLength).height();
            }
            else if ($(tableFilter).length !== 0 && $(tableFilter).is(':visible')) {
                //console.log('tableFilter', tableFilter.height());
                //h = h - $(tableFilter).height();
            }

            if ($(tableHeader).length !== 0 && $(tableHeader).is(':visible')) {
                //console.log('tableHeader', tableHeader.height());
                h = h - $(tableHeader).height();
            }
            if (h < rowH)
                h = rowH * rowCnt;
            //h = h - 35;
            if (rowCnt > 20 && rowH * rowCnt > h)
                h = rowH * 20;

            if (rowCnt <= 20 && h <= rowH * rowCnt)
                h = rowH * rowCnt;

            console.log('h', h, rowH, rowCnt);
            if (h != 0) {
                $(tbody).css('max-height', h);
                $(tbody).css('height', h);

            }
            else {

                $(tbody).removeAttr('max-height');
                $(tbody).removeAttr('height');
            }
        },
        updateRowValues: function (tableRowElem, model) {
            var table = $(tableRowElem).closest('table');
            var isDataTable = $(table).hasClass('dataTable');
            if (isDataTable == true) {
                var isTreeGridOpen = $(tableRowElem).find('.treegrid-control-open');
                if (isTreeGridOpen.length != 0) {
                    $(isTreeGridOpen).click();
                }

                var dt = $(table).DataTable();
                var dtRow = dt.row(tableRowElem);

                if (dtRow.data() != undefined) {
                    $.each(model, function (key, value) {
                        if (dtRow.data()[key] !== value) {
                            //console.log('Old Value:', dtRow.data()[key], ' New Value:', value)
                            dtRow.data()[key] = value;
                        }
                    });
                    dtRow.invalidate().draw();

                    Table().initInputPlugins(tableRowElem);
                }

                if (isTreeGridOpen.length != 0) {
                    $(isTreeGridOpen).click();
                }
            }
            else {
                $(tableRowElem).UpdateElementValues(model);
            }
        }
    };
};




