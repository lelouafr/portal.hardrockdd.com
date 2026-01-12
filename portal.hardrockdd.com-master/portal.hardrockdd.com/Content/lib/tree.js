var Tree = function () {
    return {
        init: function (elem) {

            var isBinding = $(elem).data("treeinit") === "true";
            if (!isBinding) {
                var tree = this.initTree(elem);
                this.handleNodeDrop(tree, elem);
            }
        },
        initTree: function (elem) {
            var url = $(elem).data('urlrefresh');
            var primaryKey = $(elem).data('keyfield');
            var childrenField = $(elem).data('childfield');
            var textField = $(elem).data('textfield');

            var tree = $(elem).tree({
                primaryKey: primaryKey,
                textField: textField,
                childrenField: childrenField,
                cascadeSelection: false,
                //uiLibrary: 'bootstrap',
                dataSource: url,
                dragAndDrop: true
            });

            return tree;
        },
        handleNodeDrop: function (tree, elem) {

            var url = $(elem).data('urlupdate');
            var token = $(elem).data('token');
            if (url !== undefined) {
                tree.on('nodeDrop', function (e, id, parentId, orderNumber) {
                    var params = { id: id, parentId: parentId, orderNumber: orderNumber, __RequestVerificationToken: token };
                    $.ajax({
                        type: "Post",
                        dataType: "json",
                        url: url,
                        async: true,
                        data: params,
                        success: function (res) {

                        },
                        error: function (res) {

                        }
                    })
                });
            }
        },
        handleRowSelect: function (tree, elem) {
            tree.on('select', function (e) {
                //console.log('Row Selected');
                var dstId = $(elem).data("dstselect");

                var dstId = $(elem).data("dstselect");
                var url = $(elem).data("urlselect");
                var validate = $(elem).data("dstselectvalidate");

                if (!dstId || !url) {

                }
                else {
                    var data = { [tree.primaryKey]: id };

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
                                var divId = dstId + '_' + index;
                                var html = '<div id="' + divId + '">\r\n';
                                html += res;
                                html += '\r\n</div>';
                                dst.html(html);
                                if (validate) {
                                    $(dst).ready(function () {
                                        $(dst).PostValidate(this, function (valid) { });
                                    });
                                }

                                $(dst).removeClass('panel-loading');
                                $(dst).find('.panel-loader').remove();
                            }
                        });
                    });
                }
            });
        }
    };
};