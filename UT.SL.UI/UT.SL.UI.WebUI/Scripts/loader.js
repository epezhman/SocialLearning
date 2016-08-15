var pagingCount = 1;
var loadUrl;
var loadUrlForNewPosts;
var lastCheckDate;
var resourceSelections;
var resourceFilter;
var processing = false;

//var onChange = function (e) {
//    resourceFilter = $("#resourceFilter").data("kendoMultiSelect");
//    console.log(resourceFilter.value());
//    resourceSelections = resourceFilter.value();
//    $.ajax({
//        url: loadUrl + "&resourceSelections=" + resourceSelections,
//        success: function (html) {
//            if (html) {
//                pagingCount = 1;
//                $("#postswrapper").html(html);
//                $('#loadingAjax').hide();
//            } else {
//                $('#loadingAjax').html('<center>' + noMorePosts + '</center>');
//            }
//        }
//    });
//};


///Load more contents based on scrolling
$(function () {
    $(window).scroll(function () {
        if ($(window).scrollTop() >= $(document).height() - $(window).height() - 400) {
            if (!processing) {
                $.ajax({
                    url: loadUrl + "&page=" + pagingCount + "&resourceSelections=" + filterss,
                    beforeSend: function () {
                        $('#loadingAjax').show();
                        processing = true;
                    },
                    success: function (html) {
                        if (html) {
                            pagingCount++;
                            $("#postswrapper").append(html);
                        } else {
                            $('#noMorePosts').show();
                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                        processing = false;
                    }
                });
            }
        }
    });

    setInterval(function () {
        $.ajax({
            url: loadUrlForNewPosts + "?date=" + lastCheckDate + "&resourceSelections=" + filterss,
            success: function (html) {
                if (html) {
                    $("#postswrapper").prepend(html);
                    $("[data-hiddennewpost]").each(function () {
                        $(this).show("blind", 1000);
                        $(this).removeAttr('data-hiddennewpost');
                    });
                }
            }
        });
    }, 60000);

    //$("#resourceFilter").kendoMultiSelect({
    //    change: onChange
    //});

});
