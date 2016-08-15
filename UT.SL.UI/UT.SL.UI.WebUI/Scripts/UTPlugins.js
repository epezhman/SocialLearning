function UTCheckKey(e) {
    e.stopPropagation();
    var n = e.keyCode;
    if (n == 13) {
        $(this).closest("form").submit();
    }
    else if (e.shiftKey || e.ctrlKey || e.altKey) {
        e.preventDefault();
    } else {
        if (!((n == 8)
                || (n == 46) || (n == 9) || (n == 189) || (n == 109)
                || (n >= 35 && n <= 40)
                || (n >= 48 && n <= 57)
                || (n >= 96 && n <= 105))
        ) {
            e.preventDefault();
        }
    }
}

function getCaret(el) {
    if (el.selectionStart) {
        return el.selectionStart;
    } else if (document.selection) {
        el.focus();

        var r = document.selection.createRange();
        if (r == null) {
            return 0;
        }
        var re = el.createTextRange(),
        rc = re.duplicate();
        re.moveToBookmark(r.getBookmark());
        rc.setEndPoint('EndToStart', re);
        return rc.text.length;
    }
    return 0;
}

function UTSubmit(e) {
    e.stopPropagation();
    var n = e.keyCode;
    if (n == 13 && e.shiftKey) {
        e.preventDefault();
        var content = this.value;
        var caret = getCaret(this);
        this.value = content.substring(0, caret) +
                      "\n" + content.substring(caret, content.length);
        e.stopPropagation();

    }
    else if (n == 13) {
        e.preventDefault();
        $(this).closest("form").submit();
    }

}

function UTSubmitEnter(e) {
    e.stopPropagation();
    var n = e.keyCode;
    if (n == 13) {
        e.preventDefault();
        $(this).closest("form").submit();
    }

}

function UTUpdateOtherField(e) {
    e.stopPropagation();
    $("#" + $(this).attr('data-updateid')).text($(this).val());
}

function UTFormValidate(e) {
    if (!$(this).hasClass('dropdown-toggle')) {
        e.stopPropagation();
        $("[data-removezero]").each(function () {
            if ($(this).val() == 0) {
                $(this).val('');
            }
        });
        $(this).closest("form").removeData("validator");
        $(this).closest("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($(this).closest("form"));
    }
}

function UTUpdateDropdown(e) {
    e.stopPropagation();
    var id = $(this).find(":selected").val();
    var url = $(this).attr('data-lookupurl');
    var toFillId = $(this).attr('data-updateid');
    url += '?id=' + id;
    $.ajax({
        url: url, type: 'get',
        dataType: 'json',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $('#' + toFillId).html('');
            $(data).each(function () {
                $('#' + toFillId).append('<option value="' + this.id + '">' + this.title + '</option>');
            });
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function UTTooltip(e) {
    e.stopPropagation();
    $(this).tooltip({ trigger: 'manual' }).tooltip('show');
}

function UTTooltipLeave(e) {
    e.stopPropagation();
    $(this).tooltip('destroy');
}

function UTDateTime(e) {
    e.stopPropagation();
    var n = e.keyCode;
    if (n == 13) {
        $(this).closest("form").submit();
    }
    else {
        e.preventDefault();
    }
    $(this).datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "yy/mm/dd",
        //altField: "#FormObject_EndDate",
        //altFormat: 'yy/mm/dd',
        onSelect: function (dateText, inst) {
            //var epoch = $.datepicker.formatDate('yy/mm/dd', $(this).datepicker('getDate'));
            //var thisDate = $.datepicker.parseDate('yy/mm/dd', dateText);
            //$('#FormObject_EndDate').val($.datepicker.formatDate('yy/mm/dd', new Date(($(this).datepicker('getDate')).getYear(), ($(this).datepicker('getDate')).getMonth(), ($(this).datepicker('getDate')).getDay())));
        },
        yearRange: $(this).attr("yearRange")
    });
}

function UTDateTimeWithPic(e) {
    e.stopPropagation();
    var n = e.keyCode;
    if (n == 13) {
        $(this).closest("form").submit();
    }
    else {
        e.preventDefault();
    }
    $(this).datepicker({
        changeMonth: true,
        changeYear: true,
        //dateFormat: "yy/mm/dd",
        showOn: "button",
        buttonImage: "/Images/content/calendar.gif",
        yearRange: $(this).attr("yearRange")
    });
}

function UTNationalId(e) {
    e.stopPropagation();
    if (!(typeof $(this).attr('maxlength') !== 'undefined' && $(this).attr('maxlength') !== false)) {
        $(this).attr('maxlength', '10');
    }
    if ((typeof $(this).attr('data-val-length-min') !== 'undefined' && $(this).attr('data-val-length-min') !== false)) {
        $(this).attr('data-val-length-min', '8');
        $(this).attr('data-val-length', 'حد اقل و حداکثر طول کدملی برای جست و جو 8 و 10 حرف است');
    }
}

function UTRemoveZero(e) {
    e.stopPropagation();
    if ($(this).val() == 0) {
        $(this).val('');
    }
}

function UTUpdate(e) {
    e.stopPropagation();
    var url = $(this).attr('data-updateurl');
    var updateid = $(this).attr('data-updateid');
    $.ajax({
        url: url, type: 'get',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $('#' + updateid).html(data);
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function UTUpdateTwo(e) {
    e.stopPropagation();
    var url = $(this).attr('data-updateurl');
    var updateid = $(this).attr('data-updateid');
    $.ajax({
        url: url, type: 'get',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $('#' + updateid).html(data);
        }
    });
    var url2 = $(this).attr('data-updateurl2');
    var updateid2 = $(this).attr('data-updateid2');
    $.ajax({
        url: url2, type: 'get',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $('#' + updateid2).html(data);
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function UTFancy(e) {
    e.stopPropagation();
    e.preventDefault();
    var url = $(this).attr('href');
    var idToOpen = $(this).attr('data-updateid');
    //$.fancybox.open({
    //    href: url,
    //    type: 'image',
    //    transitionIn: 'none',
    //    transitionOut: 'none'
    //});
    //$.fancybox.open({
    //    href: url,
    //    type: 'iframe',
    //    openEffect: 'none',
    //    closeEffect: 'none',
    //    iframe: {
    //        preload: false
    //    }
    //});
    $.ajax({
        url: url, type: 'get',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $('#' + idToOpen).show();
            $('#' + idToOpen).html('');
            $('#' + idToOpen).html(data);
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function UTFancyPDF(e) {
    e.stopPropagation();
    e.preventDefault();
    var url = $(this).attr('href');
    $.fancybox.open({
        href: url,
        type: 'iframe',
        openEffect: 'none',
        closeEffect: 'none',
        iframe: {
            preload: false
        }
    });
}

function UTMakeAutoComplete(e) {
    e.stopPropagation();
    var luurl = $(this).attr('data-lookupurl');
    var thisElemt = $(this);
    if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
        $(this).kendoAutoComplete({
            minLength: 3,
            separator: ", ",
            dataTextField: "title",
            select: function (e) {
                var selectedOne = this.dataItem(e.item.index());
                //console.log(kendo.stringify(selectedOne.id));
            },
            dataSource: new kendo.data.DataSource({
                serverFiltering: true,
                serverPaging: true,
                pageSize: 20,
                transport: {
                    read: luurl,
                    dataType: "json",
                    parameterMap: function (data) {
                        return { title: thisElemt.val() };
                    },
                    schema: {
                        model: {
                            id: "id",
                            fields: {
                                id: { type: "id" },
                                title: { type: "string" }
                            }
                        }
                    }
                }
            })
        });
    }
}

function UTDestroyAutoComplete(e) {
    e.stopPropagation();
    $(this).data("kendoAutoComplete").destroy();

}


var parameterfy = (function () {
    var pattern = /function[^(]*\(([^)]*)\)/;
    return function (func) {
        var args = func.toString().match(pattern)[1].split(/,\s*/);
        return function () {
            var named_params = arguments[arguments.length - 1];
            if (typeof named_params === 'object') {
                var params = [].slice.call(arguments, 0, -1);
                if (params.length < args.length) {
                    for (var i = params.length, l = args.length; i < l; i++) {
                        params.push(named_params[args[i]]);
                    }
                    return func.apply(null, params);
                }
            }
            return func.apply(null, arguments);
        };
    };
}());

function UTShowEditDelete(e) {
    e.stopPropagation();
    $(this).find(".hiddenoption").css("visibility", "visible");
    $(this).find(".paleItNew").addClass('paleIt');
    $(this).find(".paleIt").css("opacity", "1");
}

function UTHideEditDelete(e) {
    e.stopPropagation();
    $(this).find(".hiddenoption").css("visibility", "hidden");
    $(this).find(".paleItNew").addClass('paleIt');
    $(this).find(".paleIt").css("opacity", ".3");
}

function UTShowOnEdge(e) {
    e.stopPropagation();
    $(this).find(".innerP").css("opacity", ".9");
    $(this).find(".innerPBottom").css("opacity", ".9");
    $(this).find(".innerPBottom").parent().addClass("innerP2Plus");
    $(this).find(".innerPT").css("opacity", ".9");
    $(this).find(".innerPTBottom").css("opacity", ".9");
    $(this).find(".innerPTBottom").parent().addClass("innerP2TPlus")
}

function UTHideOnEdge(e) {
    e.stopPropagation();
    $(this).find(".innerPBottom").css("opacity", "0");
    $(this).find(".innerP").css("opacity", ".2");
    $(this).find(".innerPBottom").parent().removeClass("innerP2Plus");
    $(this).find(".innerPTBottom").css("opacity", "0");
    $(this).find(".innerPT").css("opacity", ".2");
    $(this).find(".innerPTBottom").parent().removeClass("innerP2TPlus");
}

function UTShowOnWhole(e) {
    e.stopPropagation();
    $(this).find(".innerP").css("opacity", ".9");
    $(this).find(".innerPBottom").css("opacity", ".9");
    $(this).find(".innerPBottom").parent().addClass("innerP2Plus");
    $(this).find(".innerPT").css("opacity", ".9");
    $(this).find(".innerPTBottom").css("opacity", ".9");
    $(this).find(".innerPTBottom").parent().addClass("innerP2TPlus")
    $(this).find(".paleItByWhole").css("opacity", "1");

}

function UTHideOnWhole(e) {
    e.stopPropagation();
    $(this).find(".innerPBottom").css("opacity", "0");
    $(this).find(".innerP").css("opacity", ".2");
    $(this).find(".innerPBottom").parent().removeClass("innerP2Plus");
    $(this).find(".innerPTBottom").css("opacity", "0");
    $(this).find(".innerPT").css("opacity", ".2");
    $(this).find(".innerPTBottom").parent().removeClass("innerP2TPlus");
    $(this).find(".paleItByWhole").css("opacity", ".3");
}

function UTShowOpacity(e) {
    e.stopPropagation();
    $(this).find(".opacity02").css("opacity", ".9");
}

function UTHideOpacity(e) {
    e.stopPropagation();
    $(this).find(".opacity02").css("opacity", ".2");
}

function UTScrollTo(e) {
    e.stopPropagation();
    var id = $(this).attr('data-scrollToId');
    $.scrollTo(id, { duration: 500, offset: -150 });
    //$(id).focus();
}

function UTShow(e) {
    e.stopPropagation();
    var thisItem = $(this);
    $("#" + thisItem.attr("data-hide")).hide();
    $("#" + thisItem.attr("data-show")).show();
    if ($.trim($("#stufftobemovedhere").html()) != '') {
        $("#stufftobemoved").show();
    }
    $("#stufftobemoved").slideUp('fast', function () {
        $("#" + thisItem.attr("data-toggleclass")).switchClass("col-md-6", "col-md-12", 1500, function () {
        });
    });

    //$("#" + $(this).attr("data-hide")).hide();
    //$("#" + $(this).attr("data-show")).show();
    //$("#" + $(this).attr("data-toggleclass")).switchClass("span6", "span12", 1500);
    //if ($.trim($("#stufftobemovedhere").html()) != '') {
    //    $("#stufftobemoved").html($("#stufftobemovedhere").html());
    //    $("#stufftobemovedhere").html('');
    //    $("#stufftobemoved").show();
    //    $("#hiddenRow").hide();
    //}
    //$("#stufftobemovedhere").html($("#stufftobemoved").html());
    //$("#stufftobemoved").html('');
    //$("#stufftobemoved").hide();
    //$("#hiddenRow").show();
}

function UTHide(e) {
    e.stopPropagation();

    $("#" + $(this).attr("data-hide")).hide();
    $("#" + $(this).attr("data-show")).show();
    if ($.trim($("#stufftobemoved").html()) != '') {
        $("#stufftobemoved").hide();
    }
    $("#" + $(this).attr("data-toggleclass")).switchClass("col-md-12", "col-md-6", 1500, function () {
        $("#stufftobemoved").slideDown('fast');
    });

    //$("#" + $(this).attr("data-hide")).hide();
    //$("#" + $(this).attr("data-show")).show();
    //if ($.trim($("#stufftobemoved").html()) != '') {
    //    $("#stufftobemovedhere").html($("#stufftobemoved").html());
    //    $("#stufftobemoved").html('');
    //    $("#stufftobemoved").hide();
    //    $("#hiddenRow").show();
    //}
    //$("#" + $(this).attr("data-toggleclass")).switchClass("span12", "span6", 1500, function () {
    //    $("#stufftobemoved").html($("#stufftobemovedhere").html());
    //    $("#stufftobemovedhere").html('');
    //    $("#stufftobemoved").show();
    //    $("#hiddenRow").hide();
    //});

}

function autocomplete_select(e) {
    $("[data-toSubmit]").submit();
}

function UTUpdateDisabled(e) {
    e.stopPropagation();
    if ($(this).is(':checked')) {
        $('[data-canbedisabled]').each(function () {
            $(this).removeAttr('disabled');
        });
    }
    else {
        $('[data-canbedisabled]').each(function () {
            $(this).attr('disabled', 'disabled');
            $(this).val('');
        });
    }
}

(function ($, undefined) {

    jQuery.fn.reset = function () {
        $(this).each(function () { this.reset(); });
    }

    $.loadAjaxUpdate = parameterfy(function () {
        var url = "";
        var urlPramas = "";
        var trId = "tr";
        var id = "";
        for (var i = 0; i < arguments.length; i++) {
            if (typeof arguments[i] === 'object') {
                var keys = Object.keys(arguments[i]);
                for (var j = 0; j < keys.length; j++) {
                    if (keys[j].toString().toLowerCase() == 'id') {
                        id = arguments[i][keys[j]];
                        urlPramas += "id=" + arguments[i][keys[j]] + "&";
                    }
                    else if (keys[j].toString().toLowerCase() == 'rowid') {
                        trId = arguments[i][keys[j]];
                    }
                    else {
                        urlPramas += keys[j] + "=" + arguments[i][keys[j]] + "&";
                    }
                }
            }
            else {
                url += "/" + arguments[i];
            }
        }
        $.ajax({
            url: url + "?" + urlPramas, type: 'get',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data, status) {
                $("#" + trId + id).html(data);
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $.loadAjaxUpdateWithServerUrl = parameterfy(function () {
        var url = "";
        var urlPramas = "";
        var trId = "tr";
        var id = "";
        for (var i = 0; i < arguments.length; i++) {
            if (typeof arguments[i] === 'object') {
                var keys = Object.keys(arguments[i]);
                for (var j = 0; j < keys.length; j++) {
                    if (keys[j].toString().toLowerCase() == 'id') {
                        id = arguments[i][keys[j]];
                        urlPramas += "id=" + arguments[i][keys[j]] + "&";
                    }
                    else if (keys[j].toString().toLowerCase() == 'rowid') {
                        trId = arguments[i][keys[j]];
                    }
                    else {
                        urlPramas += keys[j] + "=" + arguments[i][keys[j]] + "&";
                    }
                }
            }
            else {
                url += arguments[i];
            }
        }
        $.ajax({
            url: url + "?" + urlPramas, type: 'get',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data, status) {
                $("#" + trId + id).html(data);
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $.loadAjaxDialog = parameterfy(function () {
        var url = "";
        var urlPramas = "";
        var dialogId = "dialog";
        for (var i = 0; i < arguments.length; i++) {
            if (typeof arguments[i] === 'object') {
                var keys = Object.keys(arguments[i]);
                for (var j = 0; j < keys.length; j++) {
                    if (keys[j].toString().toLowerCase() == 'dialogid') {
                        dialogId = arguments[i][keys[j]];
                    }
                    else {
                        urlPramas += keys[j] + "=" + arguments[i][keys[j]] + "&";
                    }
                }
            }
            else {
                url += "/" + arguments[i];
            }
        }
        $.ajax({
            url: url + "?" + urlPramas, type: 'get',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data, status) {
                $("#" + dialogId).html(data);
                $("#" + dialogId).dialog('open');
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $.makeUiDialog = function (id, width, title) {
        $("#" + id).dialog({
            autoOpen: false,
            width: width,
            modal: true,
            draggable: true,
            title: title,
            position: 'top'
        });
    }

    $.DialogOpen = function (id, title) {
        $('#' + id).dialog('option', 'title', title);
        $("#" + id).dialog('open');
    }


    $.InitialComponents = function () {
        //Create Kendo Editor
        $("[data-editor]").each(function () {
            $(this).kendoEditor({
                tools: [
                "justifyLeft",
                "justifyCenter",
                "justifyRight",
                "justifyFull",
                "insertUnorderedList",
                "insertOrderedList",
                "indent",
                "outdent",
                "formatBlock",
                "createLink",
                "unlink",
                "insertImage",
                "viewHtml"
                ]
            });
        });

        //Create Kendo Editor
        $("[data-editorcustom]").each(function () {
            $(this).kendoEditor();
        });

        //Create Kendo MultiSelect
        $("[data-multiselect]").each(function () {
            if (!(typeof $(this).attr('data-role') !== 'undefined' && $(this).attr('data-role') !== false)) {
                $(this).kendoMultiSelect();
            }
        });

        //Create Kendo AutoComplete
        $("[data-autocomplete]").each(function () {
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    separator: ", ",
                    dataTextField: "title",
                    select: function (e) {
                        var selectedOne = this.dataItem(e.item.index());
                      //console.log(kendo.stringify(selectedOne.id));
                    },
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

        //Create Kendo AutoComplete
        $("[data-autocompletemultiplewithpics]").each(function () {
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    separator: ", ",
                    dataTextField: "title",
                    template: '<img src=\"../../../Admin/App_User/GetPic/${data.id}\" alt=\"${data.id}\" class=\"userAutoCompletePic img-rounded\"/>' +
                             '<span class=\"autocompSpan\">${ data.title }</span>',
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

        //Create Kendo AutoComplete
        $("[data-autocompletenonmultiple]").each(function () {
            var updateid = $(this).attr('data-updateid');
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    dataTextField: "title",
                    select: function (e) {
                        var selectedOne = this.dataItem(e.item.index());
                        $("#" + updateid).val(selectedOne.id);
                    },
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

        //Create Kendo AutoComplete
        $("[data-autocompletewithpics]").each(function () {
            var updateid = $(this).attr('data-updateid');
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {

                $(this).kendoAutoComplete({
                    minLength: 3,
                    dataTextField: "title",
                    template: '<img src=\"../../../Admin/App_User/GetPic/${data.id}\" alt=\"${data.id}\" class=\"userAutoCompletePic img-rounded\"/>' +
                            '<span class=\"autocompSpan\">${ data.title }</span>',
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    }),
                    height: 370,
                }).data("kendoAutoComplete");//.bind("select", autocomplete_select);
            }
        });
    }


    $.InitialComponentsWithDom = function (dom) {
        //Create Kendo Editor
        $('[data-dom="' + dom + '"]').find("[data-editor]").each(function () {
            $(this).kendoEditor({
                tools: [
                "justifyLeft",
                "justifyCenter",
                "justifyRight",
                "justifyFull",
                "insertUnorderedList",
                "insertOrderedList",
                "indent",
                "outdent",
                "formatBlock",
                "createLink",
                "unlink",
                "insertImage",
                "viewHtml"
                ]
            });
        });

        //Create Kendo Editor
        $('[data-dom="' + dom + '"]').find("[data-editorcustom]").each(function () {
            $(this).kendoEditor();
        });

        //Create Kendo MultiSelect
        $('[data-dom="' + dom + '"]').find("[data-multiselect]").each(function () {
            if (!(typeof $(this).attr('data-role') !== 'undefined' && $(this).attr('data-role') !== false)) {
                $(this).kendoMultiSelect();
            }
        });

        //Create Kendo AutoComplete
        $('[data-dom="' + dom + '"]').find("[data-autocomplete]").each(function () {
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    separator: ", ",
                    dataTextField: "title",
                    select: function (e) {
                        var selectedOne = this.dataItem(e.item.index());
                        //console.log(kendo.stringify(selectedOne.id));
                    },
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

        //Create Kendo AutoComplete
        $('[data-dom="' + dom + '"]').find("[data-autocompletemultiplewithpics]").each(function () {
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    separator: ", ",
                    dataTextField: "title",
                    template: '<img src=\"../../../Admin/App_User/GetPic/${data.id}\" alt=\"${data.id}\" class=\"userAutoCompletePic img-rounded\"/>' +
                             '<span class=\"autocompSpan\">${ data.title }</span>',
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

         //Create Kendo AutoComplete
        $('[data-dom="' + dom + '"]').find("[data-autocompletemultiplewithpicsusername]").each(function () {
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    separator: ", ",
                    dataTextField: "username",
                    template: '<img src=\"../../../Admin/App_User/GetPic/${data.id}\" alt=\"${data.id}\" class=\"userAutoCompletePic img-rounded\"/>' +
                             '<span class=\"autocompSpan\">${ data.title }</span>',
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" },
                                        username: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });


        //Create Kendo AutoComplete
        $('[data-dom="' + dom + '"]').find("[data-autocompletenonmultiple]").each(function () {
            var updateid = $(this).attr('data-updateid');
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {
                $(this).kendoAutoComplete({
                    minLength: 3,
                    dataTextField: "title",
                    select: function (e) {
                        var selectedOne = this.dataItem(e.item.index());
                        $("#" + updateid).val(selectedOne.id);
                    },
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    })
                });
            }
        });

        //Create Kendo AutoComplete
        $('[data-dom="' + dom + '"]').find("[data-autocompletewithpics]").each(function () {
            var updateid = $(this).attr('data-updateid');
            var luurl = $(this).attr('data-lookupurl');
            var thisElemt = $(this);
            if (!(typeof $(this).attr('autocomplete') !== 'undefined' && $(this).attr('autocomplete') !== false)) {

                $(this).kendoAutoComplete({
                    minLength: 3,
                    dataTextField: "title",
                    template: '<img src=\"../../../Admin/App_User/GetPic/${data.id}\" alt=\"${data.id}\" class=\"userAutoCompletePic img-rounded\"/>' +
                            '<span class=\"autocompSpan\">${ data.title }</span>',
                    dataSource: new kendo.data.DataSource({
                        serverFiltering: true,
                        serverPaging: true,
                        pageSize: 20,
                        transport: {
                            read: luurl,
                            dataType: "json",
                            parameterMap: function (data) {
                                return { title: thisElemt.val() };
                            },
                            schema: {
                                model: {
                                    id: "id",
                                    fields: {
                                        id: { type: "id" },
                                        title: { type: "string" }
                                    }
                                }
                            }
                        }
                    }),
                    height: 370,
                }).data("kendoAutoComplete");//.bind("select", autocomplete_select);
            }
        });
    }

    //Create Image Annotation
    $.InitialImageAnnotation = function () {
        $("[data-imageannotation]").each(function () {
            var geturl = $(this).attr('data-getUrl');
            var saveurl = $(this).attr('data-saveUrl');
            var deleteurl = $(this).attr('data-deleteUrl');
            //if ($(this).is(":visible")) {
            $(this).annotateImage({
                getUrl: geturl,
                saveUrl: saveurl,
                deleteUrl: deleteurl,
                editable: true
            });

        });
    }

    $.InitialTabs = function () {
        $("[data-tabs]").each(function () {
            $(this).tabs();
        });
    }

})(jQuery);

$(function () {
    //for Inserintg only Numbers
    $("div").on("keydown", "[data-number], [data-num], [data-numtype], [type='number'] , [data-nationalid]", UTCheckKey);
    //Shift & Enter Goes to Next line, Enter submits form
    $("div").on("keydown", "[data-submitonenter]", UTSubmit);
    //Updates other Element
    $("div").on("keyup", "[data-updateonchange]", UTUpdateOtherField);
    //Validates Form
    $("div").on("click", ":submit", UTFormValidate);

    $("div").on("submit", "[data-checkvalidation]", UTFormValidate);
    //Submit forsm with Enter
    $("div").on("keydown", "[data-submitwithenterkey]", UTSubmitEnter);
    //Updates second dropdown basedon first dropdown change
    $("div").on("change", "[data-dropdownchange]", UTUpdateDropdown);
    //Creates Tooltip
    $("div").on("mouseenter", "[title]", UTTooltip);
    //Destroys Tooltip
    $("div").on("mouseleave", "[title]", UTTooltipLeave);
    //Creates DatePicker
    $("div").on("focus", "[data-datetime] , [type='datetime'] , [type='date'], [data-datetimeui]", UTDateTime);
    //Creates DatePicker with calendar pic
    $("div").on("focus", "[data-datetimewithpic]", UTDateTimeWithPic);
    //Checks input if it's a valid National code
    $("div").on("focus", "[data-nationalid]", UTNationalId);
    //Removes Zero from integer input 
    $("div").on("focus", "[data-removezero]", UTRemoveZero);
    //Updates other Element with Ajax
    $("div").on("click", "[data-updateable]", UTUpdate);
    //Updates 2 Element with Ajax
    $("div").on("click", "[data-updateabletwo]", UTUpdateTwo);
    //open images for annotating in MyHome or Course pages
    $("div").on("click", "[data-fancypic]", UTFancy);
    //open PDF in same page in MyHome or Course pages
    $("div").on("click", "[data-fancypdf]", UTFancyPDF);
    //show on mouseenter
    $("div").on("mouseenter", "[data-showeditdelete]", UTShowEditDelete);
    //hide on mouseleave
    $("div").on("mouseleave", "[data-showeditdelete]", UTHideEditDelete);
    //show hidden div
    $("div").on("click", "[data-showit]", UTShow);
    //show hidden div
    $("div").on("click", "[data-hideit]", UTHide);

    $("div").on("mouseenter", "[data-showonedge]", UTShowOnEdge);

    $("div").on("mouseleave", "[data-showonedge]", UTHideOnEdge);

    $("div").on("mouseenter", "[data-showonwhole]", UTShowOnWhole);

    $("div").on("mouseleave", "[data-showonwhole]", UTHideOnWhole);

    $("div").on("mouseenter", "[data-showopacity]", UTShowOpacity);

    $("div").on("mouseleave", "[data-showopacity]", UTHideOpacity);

    $("div").on("click", "[data-scrollto]", UTScrollTo);

    $("div").on("change", "[data-enableonchecked]", UTUpdateDisabled);

    $.cookie("_screen", $(window).width() + "_" + $(window).height(), { expires: 365, path: '/' });

    window.onresize = function (e) {
        $.cookie("_screen", $(window).width() + "_" + $(window).height(), { expires: 365, path: '/' });
    }

    //Creates Jquery UI Dialog
    $("[data-dialog]").each(function () {
        $(this).dialog({
            autoOpen: false,
            width: $(this).attr('data-dialogwidth'),
            height: $(this).attr('data-dialogheight'),
            overflow: scroll,
            modal: true,
            draggable: true,
            title: $(this).attr('data-dialogtitle'),
            position: [350, 60]
        });
    });

    //Creates Jquery UI Tabs
    $("[data-tabs]").each(function () {
        $(this).tabs();
    });

    //Creates Jquery UI Sortable
    $("[data-sortable]").each(function () {
        $(this).sortable();
        $(this).disableSelection();
    });

    //Creates Kendo Dropdown
    $("[data-kendodropdown]").each(function () {
        var url = $(this).attr("data-updateurl");
        var toFillId = $(this).attr("data-updateid");
        $(this).kendoDropDownList({
            change: function (e) {
                var chosenId = e.sender._selectedValue;
                $.ajax({
                    url: url + "?id=" + chosenId,
                    type: 'get',
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (data) {
                        $('#' + toFillId).html(data);
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });
            }
        });
    });



});