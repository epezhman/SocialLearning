var globalDragableTracker = false;
var isShowing = true;

function onSelect(e) {
}

function onRemove(e) {
    var toUpdateId = e.sender.element[0].attributes['data-updateid'].value;
    $("#" + toUpdateId).val('');
    $("#" + toUpdateId).closest('form').find('.disabledItem').attr('disabled', 'disabled');
}

function onSuccess(e) {
    if (e.response != "0") {
        var toUpdateId = e.sender.element[0].attributes['data-updateid'].value;
        $("#" + toUpdateId).val(e.response);
        var attr = $("#" + toUpdateId).val();
        $("#" + toUpdateId).closest('form').find('.disabledItem').removeAttr('disabled');
        if (e.operation == "upload") {
            var currentUrl = e.sender.options.async.removeUrl;
            var indexId = currentUrl.lastIndexOf("?id=");
            if (indexId > 0) {
                var newUrl = currentUrl.substr(0, indexId);
                newUrl += "?id=" + e.response;
                e.sender.options.async.removeUrl = newUrl;
            }
        }
    }
}

function _OLDinitializeUploader() {
    $(document).ready(function () {

        $("[data-multiselecwithdrop]").kendoMultiSelect();

        $("[data-kendouploader]").each(function () {
            $(this).kendoUpload({
                async: {
                    saveUrl: $(this).attr('data-saveurl'),
                    removeUrl: $(this).attr('data-deleteurl') + "?id=0",
                    autoUpload: true
                },
                localization: {
                    select: selectTitle,
                    cancel: cancelTitle,
                    remove: removeTitle,
                    dropFilesHere: dropFilesHereTitle,
                    retry: retryTitle,
                    statusFailed: statusFailedTitle,
                    statusUploaded: statusUploadedTitle,
                    statusUploading: statusUploadingTitle,
                    uploadSelectedFiles: uploadSelectedFilesTitle
                },
                multiple: false,
                remove: onRemove,
                select: onSelect,
                success: onSuccess
            });
        });

        $("div").on("click", "[data-hidekendoupload]", function (e) {
            e.stopPropagation();
            $('.k-upload-files').hide();
        });

    });
}

function initializeUploaderWithDon(dom) {

    $('[data-dom="' + dom + '"]').find("[data-multiselecwithdrop]").kendoMultiSelect();

    $('[data-dom="' + dom + '"]').find("[data-kendouploader]").each(function () {

        $(this).kendoUpload({
            async: {
                saveUrl: $(this).attr('data-saveurl'),
                removeUrl: $(this).attr('data-deleteurl') + "?id=0",
                autoUpload: true
            },
            localization: {
                select: selectTitle,
                cancel: cancelTitle,
                remove: removeTitle,
                dropFilesHere: dropFilesHereTitle,
                retry: retryTitle,
                statusFailed: statusFailedTitle,
                statusUploaded: statusUploadedTitle,
                statusUploading: statusUploadingTitle,
                uploadSelectedFiles: uploadSelectedFilesTitle
            },
            multiple: false,
            remove: onRemove,
            select: onSelect,
            success: onSuccess
        });
    });

    $("div").on("click", "[data-hidekendoupload]", function (e) {
        e.stopPropagation();
        $('.k-upload-files').hide();
    });
}

function resetForm(id) {
    $("#" + id).closest('form').reset();
    $("#" + id).closest('form').find("[data-resetonpost]").val('');
}

function updateCourseResource(data) {
    resetForm("coursePostSubmit");
    //$(".hiddenFileUpload").hide();
    //$(".hiddenShareOption").hide();    
    if (data != "0") {
        var url = $("#resourceUpdateUrl").val();
        $.ajax({
            url: url + "?id=" + data + "&type=2",
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#postswrapper").prepend(html);
                    $(".resourceRow" + data).show("blind", 1000);
                    $('.cancelClick').click();
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    }
}

function updateCourseAssignment(data) {
    resetForm("assignmentPostSubmit");

    if (data != "0") {
        var url = $("#resourceUpdateUrl").val();
        $.ajax({
            url: url + "?id=" + data + "&type=6",
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#postswrapper").prepend(html);
                    $(".resourceRow" + data).show("blind", 1000);
                    $('.cancelClick').click();
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    }
}

function updatePostSubmissions(id, url, url2) {
    //6 is ObjectType of Assignment
    //$('#NewSubmissionViewFooter6' + id).hide('blind', 1000, function () { $(this).html('') });
    //$.ajax({
    //    url: url,
    //    success: function (html) {
    //        if (html) {
    //            $('#GetSpecificPanel' + id).html(html);
    //        }
    //    }
    //});
    //$.ajax({
    //    url: url2,
    //    success: function (html) {
    //        if (html) {
    //            $('#SubmissionFooter6' + id).html(html);
    //        }
    //    }
    //});
    $('#NewSubmissionViewFooter6' + id).hide('blind', 1000, function () { $(this).html('') });
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#GetSpecificPanel' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: url2,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#SubmissionFooter6' + id).show();
                $('#SubmissionFooter6' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

//function updateSubmissionGrade(id, url) {
//    //22 is ObjectType of AssignmentSubmission

//    $.ajax({
//        url: url,
//        success: function (html) {
//            if (html) {
//                $('#GradeSubmissionFooter22' + id).html(html);
//            }
//        }
//    });

//}

function updateSubmissionGrade(id, url) {
    //22 is ObjectType of AssignmentSubmission

    $('#NewGradeFooter22' + id).hide('blind', 1000, function () { $(this).html('') });
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#assignmentRowThread' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });

}

function updateCourseForum(data) {
    resetForm("forumPostSubmit");
    $('#forumPostSubmit').find('FormObject_GradeFrom').attr('disabled', 'disabled');
    //$(".hiddenFileUpload").hide();
    //$(".hiddenShareOption").hide();    
    if (data != "0") {
        var url = $("#resourceUpdateUrl").val();
        $.ajax({
            url: url + "?id=" + data + "&type=5",
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#postswrapper").prepend(html);
                    $(".resourceRow" + data).show("blind", 1000);
                    $('.cancelClick').click();
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    }
}

function updateDiscussions(id, url, url2) {
    $('#NewDiscussionViewFooter5' + id).hide('blind', 1000, function () { $(this).html('') });
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#allDiscussion' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: url2,
        beforeSend: function () {
        },
        success: function (html) {
            if (html) {
                $('#addDiscCnt' + id).html(html);
            }
        },
        complete: function () {
        }
    });
}

function updateDiscussionsFirst(id, url, url2) {
    $('#NewDiscussionViewFooter5' + id).hide('blind', 1000, function () { $(this).html('') });
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#DiscussionFooter5' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: url2,
        beforeSend: function () {
        },
        success: function (html) {
            if (html) {
                $('#addDiscCnt' + id).html(html);
            }
        },
        complete: function () {
        }
    });
}
function showHideSubmissionPart(id) {

    if (!$('#SubmissionFooter6' + id).is(":visible"))
        $('#SubmissionFooter6' + id).show('blind', 1000);
    else
        $('#SubmissionFooter6' + id).hide('blind', 1000);

}

function updateSubmissions(id, url, url2) {
    //6 is ObjectType of Assignment
    $('#NewSubmissionViewFooter6' + id).hide('blind', 1000, function () { $(this).html('') });
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#SubmissionFooter6' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: url2,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#addSubCnt' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

//function updateDiscussionsReplies(url) {
//    $.ajax({
//        url: url,
//        beforeSend: function () {
//            $('#loadingAjax').show();
//        },
//        success: function (html) {
//            $('#discussionsReplisForm').find('.unexpandForm').click();
//            if (html) {
//                $('#discussionsReplis').html(html);
//            }
//        },
//        complete: function () {
//            $('#loadingAjax').hide();
//        }
//    });
//}

function updateDiscussionsReplies(id, url) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {

            if (html) {
                $('#oneDiscussion' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function updateDiscussionsReplyPost(id, url, url2) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#allPosts' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: url2,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#allPostsCount' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function updateDiscussionsReplyPostCount(id, url) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('#allPostsCount' + id).html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function updateHomeResource(data) {
    resetForm("coursePostSubmit");
    //$(".hiddenFileUpload").hide();
    //$(".hiddenShareOption").hide();
    $("#shareError").hide();
    if (data == "-1") {
        $(".hiddenShareOption").show();
        $("#shareError").show();
    }
    else if (data != "0") {
        var url = $("#resourceUpdateUrl").val();
        $.ajax({
            url: url + "?id=" + data,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#postswrapper").prepend(html);
                    $(".resourceRow" + data).show("blind", 1000);
                }
                $('.cancelClick').click();
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    }
}

function onSuccessPicUpload(e) {
    window.location.reload();
}

function initializeKendoUploader() {
    $(document).ready(function () {

        $("[data-kendouploaderwithunknownid]").each(function () {
            var contentid = $(this).attr('data-contentid');

            $(this).kendoUpload({
                async: {
                    saveUrl: $(this).attr('data-saveurl') + "?id=" + contentid,
                    removeUrl: $(this).attr('data-deleteurl'),
                    autoUpload: true
                },
                localization: {
                    select: selectTitle,
                    cancel: cancelTitle,
                    remove: removeTitle,
                    dropFilesHere: dropFilesHereTitle,
                    retry: retryTitle,
                    statusFailed: statusFailedTitle,
                    statusUploaded: statusUploadedTitle,
                    statusUploading: statusUploadingTitle,
                    uploadSelectedFiles: uploadSelectedFilesTitle
                },
                multiple: false,
                success: onSuccessPicUploadWithSubmit
            });
        });
    });
}

function onSuccessPicUploadWithSubmit(e) {
    $('#searchFormCategory').submit();
}

function initializeUploaderUserPic() {
    $(document).ready(function () {
        $("[data-kendouploader]").each(function () {

            $(this).kendoUpload({
                async: {
                    saveUrl: $(this).attr('data-saveurl'),
                    removeUrl: $(this).attr('data-deleteurl'),
                    autoUpload: true
                },
                localization: {
                    select: selectTitle,
                    cancel: cancelTitle,
                    remove: removeTitle,
                    dropFilesHere: dropFilesHereTitle,
                    retry: retryTitle,
                    statusFailed: statusFailedTitle,
                    statusUploaded: statusUploadedTitle,
                    statusUploading: statusUploadingTitle,
                    uploadSelectedFiles: uploadSelectedFilesTitle
                },
                multiple: false,
                success: onSuccessPicUpload
            });
        });
    });
}

function updateCommentList(data) {
    var n = data.split(",");
    if (n.length == 4) {
        var commentId = parseInt(n[0]);
        var objectId = parseInt(n[1]);
        var objectType = parseInt(n[2]);
        var cnt = n[3];
        $("#resourcecommentcount" + objectType + objectId).text(cnt);
        if (commentId > 0) {
            $.ajax({
                url: "/Admin/Comment/GetOneComment?commentId=" + commentId,
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    if (html) {
                        $("#commentThread" + objectType + objectId).before(html);
                        $("#commentRowShow" + commentId).show("blind", 1000);
                        $("#commentArea" + objectType + objectId).val('');
                    }
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
        }
        else if (commentId < 0) {
            $.ajax({
                url: "/Admin/Comment/GetOneCommentAfterEdit?commentId=" + (commentId * -1),
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    $("#commentRowThread" + commentId * -1).html(html);
                    $("#commentRowThread" + commentId * -1).parent().find('.editcommentview').hide('blind', 500);
                    $("#commentRowThread" + commentId * -1).parent().find('.newcommentview').show('blind', 500);
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
        }
    }
}

function updateAccountView(id) {
    $.ajax({
        url: "/Admin/App_User/ViewAccount?Id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#tab-1").html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });

}

function updateCourseView(id) {
    $.ajax({
        url: "/Admin/Course/ViewCourseMadeOfAbstract?Id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#tab-1").html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: "/Admin/Course/EditDetails?Id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#tab-3").html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function closeCreditsinfo() {
    setTimeout(function (e) { $("#credits").html(''); }, 2000);
}

function updateTagCountResource(data) {
    var n = data.split("_");
    if (n.length == 2 || n.length == 3) {
        var objectId = parseInt(n[0]);
        var objectType = parseInt(n[1]);
        $('#TagViewFooter' + objectType + objectId).hide('blind', 1000);
        $.ajax({
            url: "/Admin/TagMapper/TopOnlyTags?objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#TagsFooter" + objectType + objectId).html(html);
                    $("#TagsFooter" + objectType + objectId).closest('.feedDivFooterTags').show();
                    $('#TagsFooter' + objectType + objectId).show();

                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagComponentResource?newTags=1&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tag" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        if (n.length == 3) {
            toast(threeTagsMax);
        }
    }
}

function updateTagCountResourceType2(data) {
    var n = data.split("_");
    if (n.length == 2 || n.length == 3) {
        var objectId = parseInt(n[0]);
        var objectType = parseInt(n[1]);
        $('#TagViewFooter' + objectType + objectId).hide('blind', 1000);
        $.ajax({
            url: "/Admin/TagMapper/TopOnlyTags?tagType=2&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#TagsFooter" + objectType + objectId).html(html);
                    $("#TagsFooter" + objectType + objectId).closest('.feedDivFooterTags').show();
                    $('#TagsFooter' + objectType + objectId).show();

                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagComponentResource?tagType=2&newTags=1&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tag" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        if (n.length == 3) {
            toast(threeTagsMax);
        }
    }
}

function updateTagCountResourceAfterDelete(data) {
    var n = data.split("_");
    if (n.length == 2 || n.length == 3) {
        var objectId = parseInt(n[0]);
        var objectType = parseInt(n[1]);
        $.ajax({
            url: "/Admin/TagMapper/TopOnlyTags?objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#TagsFooter" + objectType + objectId).html(html);
                    $("#TagsFooter" + objectType + objectId).closest('.feedDivFooterTags').show();
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagComponentResource?newTags=1&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tag" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagsForViewAndDelete?objectId=" + objectId + "&objectType=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tagToViewAndDelete" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        if (n.length == 3) {
            toast(threeTagsMax);
        }
    }
}

function updateTagCountResourceType2AfterDelete(data) {
    var n = data.split("_");
    if (n.length == 2 || n.length == 3) {
        var objectId = parseInt(n[0]);
        var objectType = parseInt(n[1]);
        $.ajax({
            url: "/Admin/TagMapper/TopOnlyTags?tagType=2&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#TagsFooter" + objectType + objectId).html(html);
                    $("#TagsFooter" + objectType + objectId).closest('.feedDivFooterTags').show();
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagComponentResource?tagType=2&newTags=1&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tag" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: "/Admin/TagMapper/TagsForViewAndDelete?tagType=2&objectId=" + objectId + "&type=" + objectType,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $(".tag" + objectType + objectId).each(function () {
                        $(this).html(html);
                    });
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        if (n.length == 3) {
            toast(threeTagsMax);
        }
    }
}

function updateTagCount(objectId, objectType) {
    $.ajax({
        url: "/Admin/TagMapper/TagComponent?objectId=" + objectId + "&type=" + objectType,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $(".tag" + objectType + objectId).each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });

}

function updateAuthorMessage(url) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#newMessagesSection").append(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function updateThreadView(url, id) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $('[data-singlethread=thread' + id + ']').html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function updateThreads(url) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("[data-messageopenmenu]").find('.inboxmessages').html(html);
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function highLight(menuType, linkType, openTab, highLightHeader) {
    if (openTab) {
        $('#Div' + menuType).addClass('in');
    }
    if (menuType > 0 && highLightHeader) {
        $('#headerLink' + menuType).closest('.panel').find('.panel-heading').css('background-color', '#FFFAF0 !important');
    }
    if (linkType > 0 && menuType == 7) {
        $('#navLinkLink' + linkType).css('background-color', '#FFFAF0 !important');
    }
    else if (linkType > 0 && menuType == 8) {
        $('#navLinkLinkC' + linkType).css('background-color', '#FFFAF0 !important');
    }
    else if (linkType > 0) {
        $('#navLinkLink' + linkType).css('background-color', '#FFFAF0 !important');
        $('#navLinkLinkC' + linkType).css('background-color', '#FFFAF0 !important');
    }
}

function UpdateSocialGroupMemebrsSections(id) {
    $.ajax({
        url: "/Admin/SocialGroup/GetTopMembersForPanel?id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#groupDiv" + id).each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: "/Admin/SocialGroup/GetMembersForManageForPanel?id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#membersInGroupEdit").each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $("#Member" + id).val('');
}

function UpdateLearningGroupMemebrsSections(id) {
    $.ajax({
        url: "/Admin/LearningGroup/GetTopMembersForPanel?id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#groupDiv" + id).each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: "/Admin/LearningGroup/GetMembersForManageForPanel?id=" + id,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#membersInGroupEdit").each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $("#Member" + id).val('');
}

function UpdateMemebrsSections(id, memberType) {
    $.ajax({
        url: "/Admin/Course/GetTopMembersForPanel?id=" + id + "&memberType=" + memberType,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#groupDiv" + memberType).each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: "/Admin/Course/GetMembersForManageForPanel?id=" + id + "&memberType=" + memberType,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#membersInGroupEdit").each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $.ajax({
        url: "/Admin/Course/GetMembersForPanelCount?id=" + id + "&memberType=" + memberType,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            if (html) {
                $("#memberCount" + memberType).each(function () {
                    $(this).html(html);
                });
            }
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
    $("#Member" + memberType).val('');
}

function DragDrop(url, groupId, updateType, extraId) {

    $(".droppableItem").droppable({
        activeClass: "scaleDouble",
        accept: ".draggableItem",
        tolerance: "touch",
        drop: function (event, ui) {

            $(".draggableItem").draggable("destroy");
            var thisId = $(ui.draggable).attr('data-id');
            if (confirm(areYouSure)) {
                $(ui.draggable).tooltip('destroy');
                $(ui.draggable).remove();
                $.ajax({
                    url: url + "?userId=" + thisId + "&gId=" + groupId,
                    type: "post",
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (html) {
                        if (updateType == 1) {
                            UpdateSocialGroupMemebrsSections(groupId)
                        }
                        else if (updateType == 2) {
                            UpdateLearningGroupMemebrsSections(groupId);
                        }
                        else if (updateType == 3) {
                            UpdateMemebrsSections(groupId, extraId);
                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });
                globalDragableTracker = false;
            }
            else {
                globalDragableTracker = true;
            }
        }
    });
}

function UpdatePanel(url) {
    $.ajax({
        url: url,
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (html) {
            $('.panelCompo').html(html);
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function DeleteResource(id) {
    var height = $('#' + id).closest('.contentLoad').height();
    var width = $('#' + id).closest('.contentLoad').width();
    $('#' + id).closest('.contentLoad').css('height', height);
    $('#' + id).closest('.contentLoad').css('width', width);
    $('#' + id).closest('.contentLoad').css('text-align', 'center');
    $('#' + id).closest('.contentLoad').css('display', 'table');
    $('#' + id).closest('.contentLoad').css('overflow', 'hidden');
    $('#' + id).closest('.contentLoad').html('<div style="display: table-cell; vertical-align: middle;"><i class="icon-trash"></i><br /> ' + postDeleted + '</div>');
}

function DeleteDiscussion(id) {
    $('#oneDiscussionRow' + id).slideUp(500);
    $('#oneDiscussionRow' + id).html('');
}

function DeleteFirstDiscussion(id) {
    $('#oneDiscussionRow' + id).slideUp(500);
    $('#oneDiscussionRow' + id).html('');
}

function DeleteDiscussionPost(id) {
    $('#oneDiscussionPostRow' + id).slideUp(500);
    $('#oneDiscussionPostRow' + id).html('');
}

function showOneDiscussion(id) {
    $("#allDiscussion" + id).effect('slide', { direction: 'left', mode: 'hide' }, 150, function () {
        $("#oneDiscussion" + id).effect('slide', { direction: 'right', mode: 'show' }, 150);
    });
}

function showAllDiscussion(id) {
    $("#oneDiscussion" + id).effect('slide', { direction: 'right', mode: 'hide' }, 150, function () {
        $("#allDiscussion" + id).effect('slide', { direction: 'left', mode: 'show' }, 150);
        $("#oneDiscussion" + id).html('');
    });
}

function showEditSection(contentId, editId) {
    $('#' + contentId).hide();
    $('#' + editId).show();
}

function hideEditSection(contentId, editId) {
    $('#' + editId).hide();
    $('#' + contentId).show();
}

function closeEditSection(cellId, url) {
    $.ajax({
        url: url, type: 'get',
        beforeSend: function () {
            $('#loadingAjax').show();
        },
        success: function (data) {
            $("#" + cellId).html(data);
        },
        complete: function () {
            $('#loadingAjax').hide();
        }
    });
}

function resetFields() {
    //$('#Password').val('');
    //$('#ConfirmPassword').val('');
    $('input:password').each(function () {
        $(this).val('');
    });
}

function toast(msg) {
    $('#footerMessage').html(msg).fadeIn(700).delay(3000).fadeOut(700);
}

function createPopovers() {
    $('[data-turnpopover]').each(function () {
        $(this).popover({
            trigger: "hover",
            html: true,
            content: function () {
                return $(this).next().html();
            }
        });
    });
}

function closeMessagePart() {
    $('#messageMenu1').hide();
    $('[data-messagebutton]').removeAttr('data-messageopen');
    $('#messageMenu1').removeAttr('data-messageopenmenu');
    $('#messageMenu1').find('.inboxmessages').html('');
    $('#messageMenu1').find('.inboxmessagesloading').show();
    $('#messageMenu2').hide();
    $('[data-messagebutton]').removeAttr('data-messageopen');
    $('#messageMenu2').removeAttr('data-messageopenmenu');
    $('#messageMenu2').find('.inboxmessages').html('');
    $('#messageMenu2').find('.inboxmessagesloading').show();
}

function closeNotifPart() {
    $('#notificationMenu1').hide();
    $('[data-notifbutton]').removeAttr('data-notifopen');
    $('#notificationMenu1').removeAttr('data-notifopenmenu');
    $('#notificationMenu1').find('.notifmessages').html('');
    $('#notificationMenu1').find('.notifmessagesloading').show();
    $('#notificationMenu2').hide();
    $('[data-notifbutton]').removeAttr('data-notifopen');
    $('#notificationMenu2').removeAttr('data-notifopenmenu');
    $('#notificationMenu2').find('.notifmessages').html('');
    $('#notificationMenu2').find('.notifmessagesloading').show();
}

$(function () {

    $("#langs a").click(function () {
        $.cookie("_culture", $(this).attr("class"), { expires: 365, path: '/' });
        window.location.reload();
    });

    $("div").on("change", "#CategoryTag", function (e) {
        e.stopPropagation();
        var thisOne = $(this);
        var id = $(this).find(":selected").val();
        var url = $(this).attr('data-lookupurl');
        url += '?id=' + id;
        var url2 = $(this).attr('data-lookupurl2');
        url2 += '?id=' + id;
        $.ajax({
            url: url, type: 'get',
            dataType: 'json',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data) {
                thisOne.closest("form").find('#SubjectTag').html('');
                thisOne.closest("form").find('#SubjectTag').append('<option value="0">' + chooseTitle + '</option>');
                $(data).each(function () {
                    thisOne.closest("form").find('#SubjectTag').append('<option value="' + this.id + '">' + this.title + '</option>');
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
        $.ajax({
            url: url2, type: 'get',
            dataType: 'json',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data) {
                thisOne.closest("form").find('#TagTag').html('');
                thisOne.closest("form").find('#TagTag').append('<option value="0">' + chooseTitle + '</option>');
                $(data).each(function () {
                    thisOne.closest("form").find('#TagTag').append('<option value="' + this.id + '">' + this.title + '</option>');
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("change", "#SubjectTag", function (e) {
        e.stopPropagation();
        var thisOne = $(this);
        var id = $(this).find(":selected").val();
        var url = $(this).attr('data-lookupurl');
        url += '?id=' + id;
        $.ajax({
            url: url, type: 'get',
            dataType: 'json',
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (data) {
                thisOne.closest("form").find('#TagTag').html('');
                thisOne.closest("form").find('#TagTag').append('<option value="0">' + chooseTitle + '</option>');
                $(data).each(function () {
                    thisOne.closest("form").find('#TagTag').append('<option value="' + this.id + '">' + this.title + '</option>');
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("change", "#TagTag", function (e) {
        e.stopPropagation();
        var thisOne = $(this);
        var idTag = $(this).find(":selected").val();
        var textTag = $(this).find(":selected").text();
        var textSubject = thisOne.closest("form").find("#SubjectTag").find(":selected").text();
        var idSubject = thisOne.closest("form").find("#SubjectTag").find(":selected").val();
        var idCategory = thisOne.closest("form").find("#CategoryTag").find(":selected").text();
        if (idTag != "0") {
            if (idSubject != "0")
                thisOne.closest("form").find("#FormObject_Tags").val($("#FormObject_Tags").val() + idCategory + "-" + textSubject + "-" + textTag + ", ");
            else
                thisOne.closest("form").find("#FormObject_Tags").val($("#FormObject_Tags").val() + idCategory + "-" + textTag + ", ");
        }
    });

    $("div").on("click", ".sendComment", function (e) {
        e.stopPropagation();
        var thisItem = $(this);
        if (!thisItem.closest('.feedDiv').find(".commentPostForm").is(":visible")) {
            thisItem.closest('.feedDiv').find(".commentPostForm").show();
            if (!thisItem.closest('.feedDiv').find(".feedDivFooterComment").is(":visible")) {
                thisItem.closest('.feedDiv').find(".feedDivFooterComment").toggle("blind", 1000);
            }
        } else {
            thisItem.closest('.feedDiv').find(".feedDivFooterComment").toggle("blind", 1000);
        }
    });

    $("div").on("click", ".showhiddendiv", function (e) {
        e.stopPropagation();
        var thisItem = $(this).closest('.span6');
        thisItem.find(".hiddendiv").toggle("blind", 500);
    });

    $("div").on("click", ".showhiddendiv2", function (e) {
        e.stopPropagation();
        $(this).parent().parent().parent().find(".hiddendiv2").toggle("blind", 500);
        if ($(this).parent().parent().parent().find("[data-changeicon]").attr('class') == "icon-arrow-down") {
            $(this).parent().parent().parent().find("[data-changeicon]").attr('class', 'icon-arrow-up');
        }
        else {
            $(this).parent().parent().parent().find("[data-changeicon]").attr('class', 'icon-arrow-down');
        }
    });

    $("div").on("click", ".showhiddendiv3", function (e) {
        e.stopPropagation();
        var thisItem = $(this);
        thisItem.find(".hiddendiv").toggle("blind", 500);
    });

    $("div").on("focus", ".commentTextarea", function (e) {
        e.stopPropagation();
        $(this).select();
        $(this).animate({ height: '60' }, 600);
    });

    $("div").on("blur", ".commentTextarea", function (e) {
        e.stopPropagation();
        $(this).animate({ height: '30' }, 600);
    });

    $("div").on("click", ".reactionUpdater", function (e) {
        e.stopPropagation();
        var id = $(this).attr('data-reactionid');
        var url = $(this).closest('ul').attr('data-lookupurl');
        var updateid = $(this).closest('ul').attr('data-updateTargetId');
        url += '&vote=1&voteValue=' + id;
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
    });

    $("div").on("click", ".gradeUpdater", function (e) {
        e.stopPropagation();
        var id = $(this).attr('data-gradeid');
        var url = $(this).closest('ul').attr('data-lookupurl');
        var updateid = $(this).closest('ul').attr('data-updatetargetid');
        url += '&grade=' + id;
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
    });

    $("[data-kendodropdownwithtemplate]").each(function () {
        var categoryId = $(this).attr('data-categoryid');
        var url = $(this).attr("data-updateurl");
        var toFillId = $(this).attr("data-updateid");
        var updateDialog = $(this).attr("data-updatedialog");
        $(this).kendoDropDownList({
            dataTextField: "CategoryTitle",
            dataValueField: "CategoryId",
            // define custom template
            template: '<span style="font-weight:bold">${ data.CategoryTitle }</span>' +
                       '${ data.CourseTitle }',
            change: function (e) {
                var chosenId = e.sender._selectedValue;
                if (chosenId != "0") {
                    $.ajax({
                        url: url + "?id=" + chosenId,
                        type: 'get',
                        beforeSend: function () {
                            $('#loadingAjax').show();
                        },
                        success: function (data) {
                            $('#' + toFillId).html(data);
                            if (updateDialog != null) {
                                $('#' + toFillId).dialog('open');
                            }
                        },
                        complete: function () {
                            $('#loadingAjax').hide();
                        }
                    });
                }
            },
            dataSource: {
                transport: {
                    read: {
                        dataType: "json",
                        // url: "@Url.Action("GetSubCategories", "Category", new { area = "admin" })" + "?id=" + categoryId,
                    }
                }
            }
        });
    });

    $("div").on("click", ".commentHiddenBlockShow", function (e) { ////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.row').siblings('.commentHiddenBlockDiv').each(function () {
            $(this).show("blind", 1000);
            $(thisBlock).closest('.row').siblings('.commentHiddenBlockHideContain').show();
            $(thisBlock).closest('.row').hide();
        });
    });

    $("div").on("click", ".commentHiddenBlockHide", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.row').siblings('.commentHiddenBlockDiv').each(function () {
            $(this).hide("blind", 1000);
            $(thisBlock).closest('.row').siblings('.commentHiddenBlockShowContain').show();
            $(thisBlock).closest('.row').hide();
        });
    });

    $("div").on("click", ".discussionHiddenBlockShow", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).parent().siblings('.discussiontHiddenBlockDiv').each(function () {
            $(this).show("blind", 1000);
        })
        $(thisBlock).parent().siblings('.discussionHiddenBlockHideContain').show();
        $(thisBlock).parent().hide();
    });

    $("div").on("click", ".discussionHiddenBlockHide", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).parent().siblings('.discussiontHiddenBlockDiv').each(function () {
            $(this).hide("blind", 1000);
        })
        $(thisBlock).parent().siblings('.discussionHiddenBlockShowContain').show();
        $(thisBlock).parent().hide();
    });

    $("div").on("click", ".discussionHiddenBlockShowItems", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.row').siblings('.discussiontHiddenBlockDiv').each(function () {
            $(this).show("blind", 1000);
        })
        $(thisBlock).closest('.row').siblings('.discussionHiddenBlockHideContain').show();
        $(thisBlock).closest('.row').hide();
    });

    $("div").on("click", ".discussionHiddenBlockHideItems", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.row').siblings('.discussiontHiddenBlockDiv').each(function () {
            $(this).hide("blind", 1000);
        })
        $(thisBlock).parent().siblings('.discussionHiddenBlockShowContain').show();
        $(thisBlock).parent().hide();
    });


    $("div").on("click", ".submissionHiddenBlockShow", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).parent().siblings('.submissionHiddenBlockDiv').each(function () {
            $(this).show("blind", 1000);
        })
        $(thisBlock).parent().siblings('.submissionHiddenBlockHideContain').show();
        $(thisBlock).parent().hide();

    });

    $("div").on("click", ".submissionHiddenBlockHide", function (e) {////////////////////////////will change
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).parent().siblings('.submissionHiddenBlockDiv').each(function () {
            $(this).hide("blind", 1000);
        })
        $(thisBlock).parent().siblings('.submissionHiddenBlockShowContain').show();
        $(thisBlock).parent().hide();
    });

    $("div").on("click", ".closecommentedior", function (e) {
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.editcommentview').hide('blind', 500);
        $(thisBlock).closest('.commentPostForm').find('.newcommentview').show('blind', 500);
    });

    $("div").on("click", ".closesharepart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterShareView').hide('blind', 1000, function () { $(this).html('') });
    });

    $("div").on("click", ".closetagpart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterTagView').hide('blind', 1000, function () { $(this).html('') });
    });

    $("div").on("click", ".closeNewDiscussionPart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterNewDiscussionView').hide('blind', 1000, function () { $(this).html('') });
    });

    $("div").on("click", ".closeNewSubmissionPart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterNewSubmissionView').hide('blind', 1000, function () { $(this).html('') });

        var val = $(thisBlock).closest('form').find('.hiddenId').val();
        var url = $(thisBlock).closest('form').find('.hiddenId').attr('data-removeAttr');
        if (!(typeof val == 'undefined' || val == false)) {
            $.ajax({
                url: url + "?id=" + val,
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    if (html) {

                    }
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
        }
    });

    $("div").on("click", ".closeNewGradePart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterGradeSubmissionView').hide('blind', 1000, function () { $(this).html('') });

        var val = $(thisBlock).closest('form').find('.hiddenId').val();
        var url = $(thisBlock).closest('form').find('.hiddenId').attr('data-removeAttr');
        if (!(typeof val == 'undefined' || val == false)) {
            $.ajax({
                url: url + "?id=" + val,
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    if (html) {

                    }
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
        }
    });

    $("div").on("click", ".closeimageview", function (e) {
        e.stopPropagation();
        var thisBlock = this;
        $(thisBlock).closest('.feedDivFooterImageView').html('');
    });

    $("div").on("click", ".showFileUpload", function (e) {
        e.stopPropagation();
        $(".hiddenFileUpload").toggle("blind", 500);
    });

    $("div").on("click", ".showShareOption", function (e) {
        e.stopPropagation();
        $(".hiddenShareOption").toggle("blind", 500);
    });

    var objDistance = 0;

    if ($("#navigationBar").length > 0) {
        objDistance = $("#navigationBar").offset().top;
    }

    window.onresize = function (e) {
        var menuWidth = $("[data-nav]").width();
        if (menuWidth <= 960) {
            var myDistance = $(window).scrollTop();
            if (myDistance > objDistance) {
                $("#navigationBar").css("margin-top", 0);
                $("#navigationPanel").css("margin-top", 0);
                $('.fever').slideDown(250);
            }
            if (objDistance > myDistance) {
                $("#navigationBar").css("margin-top", 52 - myDistance);
                $("#navigationPanel").css("margin-top", 52 - myDistance);
                $('.fever').slideUp(250);
            }
        }
    }

    var menuWidth = $("[data-nav]").width();
    var myDistance = $(window).scrollTop();
    if (myDistance > objDistance) {
        $("#navigationBar").css("margin-top", 0);
        $("#navigationPanel").css("margin-top", 0);
        //$('.fever').slideDown(250);
        $('.fever').css('display', '');
    }
    if (objDistance > myDistance) {
        $("#navigationBar").css("margin-top", 52 - myDistance);
        $("#navigationPanel").css("margin-top", 52 - myDistance);
        //$('.fever').slideUp(250);
        $('.fever').css('display', 'none');
    }

    $(window).scroll(function () {
        var myDistance = $(window).scrollTop();
        if (myDistance > objDistance) {
            $("#navigationBar").css("margin-top", 0);
            $("#navigationPanel").css("margin-top", 0);
            //$('.fever').slideDown(250);
            $('.fever').css('display', '');
        }
        if (objDistance > myDistance) {
            if ($(window).width() > 979) {
                $("#navigationBar").css("margin-top", 52 - myDistance);
                $("#navigationPanel").css("margin-top", 52 - myDistance);
            }
            else {
                $("#navigationBar").css("margin-top", 52 - myDistance);
                $("#navigationPanel").css("margin-top", 52 - myDistance);
            }
            //$('.fever').slideUp(250);
            $('.fever').css('display', 'none');
        }
    });

    $("div").on("click , mouseenter", ".showmenu", function (e) {
        e.stopPropagation();
        var options = {};
        if ($("#navigationPanel").css('right') == "0px") {
            options = { direction: 'right', mode: 'show' };
        }
        else {
            options = { direction: 'left', mode: 'show' };
        }
        $("#navigationPanel").show('slide', options, 150);
    });

    $("div").on("mouseleave", "#navigationPanel", function (e) {
        e.stopPropagation();
        var options = {};
        if ($("#navigationPanel").css('right') == "0px") {
            options = { direction: 'right', mode: 'hide' };
        }
        else {
            options = { direction: 'left', mode: 'hide' };
        }
        $("#navigationPanel").hide('slide', options, 150);
    });

    $("div").on("click", "[data-target='.navbar-responsive-collapse']", function (e) {
        e.stopPropagation();
        $("#navigationBar").toggle();
    });

    $("div").on("click", ".backToGroups", function (e) {
        e.stopPropagation();
        $('#groupMembersDiv').hide(1000, function () { $('#GroupAll').show(1000); })
    });

    $("div").on("click", "#showMoreGroups", function (e) {
        e.stopPropagation();
        $('#moreGroups').show(1000, function () { $('#showLessGroups').show(); $('#showMoreGroups').hide(); })
    });

    $("div").on("click", "#showLessGroups", function (e) {
        e.stopPropagation();
        $('#moreGroups').hide(1000, function () { $('#showMoreGroups').show(); $('#showLessGroups').hide(); })
    });

    $("div").on("click", ".hideItDiv", function (e) {
        e.stopPropagation();
        $(this).closest('.hideShowDiv').find('.hideItDiv').slideUp(1000);
        $(this).closest('.hideShowDiv').find('.showItDiv').slideDown(1000);
    });

    $("div").on("click", ".closeNewDiscussionReplyPart", function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(this).closest('.hideShowDiv').find('.showItDiv').slideUp(1000);
        $(this).closest('.hideShowDiv').find('.hideItDiv').slideDown(1000);
    });

    $("div").on("click", ".showMoreItem", function (e) {
        e.stopPropagation();
        var thisOne = $(this);
        thisOne.closest('.AllItems').show(1000, function () { thisOne.closest('.AllItems').find('.showLessItems').show(); thisOne.hide(); })
    });

    $("div").on("click", ".showLessItems", function (e) {
        e.stopPropagation();
        var thisOne = $(this);
        thisOne.closest('.AllItems').hide(1000, function () { thisOne.closest('.AllItems').find('.showMoreItem').show(); thisOne.hide(); })
    });

    $("div").on("mouseenter", ".draggableItem", function (e) {
        e.stopPropagation();
        $(".draggableItem").draggable({
            revert: true,
            revertDuration: 500,
            start: function (event, ui) {
                $(ui.helper).tooltip('destroy');
                globalDragableTracker = false;
            }
        });
    });

    $("div").on("click", ".draggableItem", function (e) {
        e.stopPropagation();
        if (globalDragableTracker) {
            e.preventDefault();
        }
    });

    $("div").on("click", ".changeLongText", function (e) {
        e.stopPropagation();
        var txt = $(this).text();
        if (txt == moreTitle)
            $(this).text(lessTitle);
        if (txt == lessTitle)
            $(this).text(moreTitle);
        $(this).closest('div').find('.toToggle').slideToggle();
    });

    $("div").on("click", "#gradeChanger", function (e) {
        e.stopPropagation();
        var element = $(this).closest('div').find('input[type="text"]');
        if (element.is(':disabled'))
            element.removeAttr('disabled');
        else
            element.attr('disabled', 'disabled');

    });

    //$("div").on("click", "#textbox", function (e) {
    //    e.stopPropagation();
    //    var element = $(this).closest('div').find('input[type="text"]');
    //    if (element.is(':disabled'))
    //        element.removeAttr('disabled');
    //    else
    //        element.attr('disabled', 'disabled');

    //});

    $("div").on("click", ".back", function (e) {
        e.stopPropagation();
        showAllDiscussion($(this).attr('data-id'));
    });

    $("div").on("focus", ".expandIt", function (e) {
        e.stopPropagation();
        $(this).closest('.expandForm').find('.hiddenDiv').slideDown('500');
        $(this).closest('.expandForm').find('.postDisccusion').css({ height: '100px' });
    });

    $("div").on("click", ".unexpandForm", function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(this).closest('form').reset();
        $(this).closest('.expandForm').find('.hiddenDiv').slideUp('500');
    });

    $("div").on("click", ".unexpandIt", function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(this).closest('form').reset();
        $(this).closest('.discussionReplyForm').slideUp(function () {
            var attr = $(this).closest('.postRow').attr('data-none');
            if (!(typeof attr == 'undefined' || attr == false)) {
                $(this).closest('.postRow').hide();
            }
        });
    });

    $("div").on("click", ".viewDiscussionPostForm", function (e) {
        e.stopPropagation();
        e.preventDefault();
        var id = $(this).attr('data-discussionId');
        var user = $(this).attr('data-user');
        if (user != "" && user != null) {
            $(this).closest('.allDiscussions').find('#postRow' + id).find('.postDisccusionTextArea').val('@' + user + ' ');
        }
        else {
            $(this).closest('.allDiscussions').find('#postRow' + id).find('.postDisccusionTextArea').val('');
        }

        $(this).closest('.allDiscussions').find('#postRow' + id).show();
        $(this).closest('.allDiscussions').find('#discussionReplyForm' + id).slideDown(function () {
            $(this).closest('.allDiscussions').find('#postRow' + id).find('.postDisccusionTextArea').focus();
            var areaT = $(this).closest('.allDiscussions').find('#postRow' + id).find('.postDisccusionTextArea');
            $.scrollTo(areaT, { duration: 500, offset: -150 });

        });
    });

    $("div").on("click", ".closeEditSection", function (e) {
        e.stopPropagation();
        e.preventDefault();
        closeEditSection($(this).attr('data-cellid'), $(this).attr('data-updateurl'));
    });

    $("div").on("click", ".deleteFileOnCancel", function (e) {
        e.stopPropagation();

        var deleteUrl = $(this).closest('form').find('[type="file"]').attr('data-deleteurl');
        var delteId = $(this).closest('form').find('.fileId').val();

        var form = $(this).closest('form').closest('.parentDiv').find('[data-antiforgery]');
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        if (!(typeof delteId == 'undefined' || delteId == false)) {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                data: {
                    __RequestVerificationToken: token,
                    id: delteId
                },
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
        }
        $(this).closest('form').reset();
        $(this).closest('form').find("[data-resetonpost]").val('');
    });

    $("div").on("click", ".feedbackSubmit", function (e) {
        e.stopPropagation();
        $.ajax({
            url: "/Admin/Feedback/Create",
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#feedbackDialog").html(html);
                    $.DialogOpen('feedbackDialog', 'Feedback');
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("mouseenter", "[data-star]", function (e) {
        e.stopPropagation();
        var starVal = parseInt($(this).attr('data-star'));
        if (starVal == 1) {
            $(this).closest('.rating').find('.star1').removeClass('starRating').addClass('starRatingFull');
        }
        else if (starVal == 2) {
            $(this).closest('.rating').find('.star1').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star2').removeClass('starRating').addClass('starRatingFull');
        }
        else if (starVal == 3) {
            $(this).closest('.rating').find('.star1').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star2').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star3').removeClass('starRating').addClass('starRatingFull');
        }
        else if (starVal == 4) {
            $(this).closest('.rating').find('.star1').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star2').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star3').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star4').removeClass('starRating').addClass('starRatingFull');
        }
        else if (starVal == 5) {
            $(this).closest('.rating').find('.star1').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star2').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star3').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star4').removeClass('starRating').addClass('starRatingFull');
            $(this).closest('.rating').find('.star5').removeClass('starRating').addClass('starRatingFull');
        }
    });

    $("div").on("mouseleave", "[data-star]", function (e) {
        e.stopPropagation();
        var starVal = parseInt($(this).attr('data-star'));
        if (starVal == 1) {
            $(this).closest('.rating').find('.star1').removeClass('starRatingFull').addClass('starRating');
        }
        else if (starVal == 2) {
            $(this).closest('.rating').find('.star1').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star2').removeClass('starRatingFull').addClass('starRating');
        }
        else if (starVal == 3) {
            $(this).closest('.rating').find('.star1').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star2').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star3').removeClass('starRatingFull').addClass('starRating');
        }
        else if (starVal == 4) {
            $(this).closest('.rating').find('.star1').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star2').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star3').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star4').removeClass('starRatingFull').addClass('starRating');
        }
        else if (starVal == 5) {
            $(this).closest('.rating').find('.star1').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star2').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star3').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star4').removeClass('starRatingFull').addClass('starRating');
            $(this).closest('.rating').find('.star5').removeClass('starRatingFull').addClass('starRating');
        }
    });

    $("div").on("mouseenter", ".rating", function (e) {
        e.stopPropagation();
        $(this).find('[data-star]').removeClass('starRatingFull').addClass('starRating');
    });

    $("div").on("mouseleave", ".rating", function (e) {
        e.stopPropagation();
        $(this).closest('.surveyRating').find('[data-star]').each(function () {
            var val = $(this).attr('data-constant');
            if (!(typeof val == 'undefined' || val == false)) {
                $(this).removeClass('starRating').addClass('starRatingFull');
            }
        });
    });

    $("div").on("click", "[data-star]", function (e) {
        e.stopPropagation();
        var ratingContent = $(this).closest('.ratingContent');
        var surveyId = $(this).attr('data-surveyid');
        var questionId = $(this).attr('data-quationid');
        var starVal = $(this).attr('data-star');
        $.ajax({
            url: "/Admin/Survey/AddAnswer?surveyId=" + surveyId + "&questionId=" + questionId + "&answerVal=" + starVal,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    ratingContent.html(html);
                }

                $.ajax({
                    url: "/Admin/Survey/QuestionsLeft?surveyId=" + surveyId,
                    beforeSend: function () {
                    },
                    success: function (html) {
                        if (html) {
                            $("#questionLeft").text(html);
                            if (html == "D") {
                                $("#questionLeft").text('0');
                                $("#finalSubmit").removeAttr('disabled');
                                $("#finalSubmitTitle").removeAttr('title');
                                $('#finalSubmit').popover('show')
                            }
                        }
                    },
                    complete: function () {
                    }
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("click", "[data-circle]", function (e) {
        e.stopPropagation();
        var ratingContent = $(this).closest('.ratingContent');
        var surveyId = $(this).attr('data-surveyid');
        var questionId = $(this).attr('data-quationid');
        var circlVal = $(this).attr('data-circle');
        $.ajax({
            url: "/Admin/Survey/AddAnswerCircle?surveyId=" + surveyId + "&questionId=" + questionId + "&answerVal=" + circlVal,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    ratingContent.html(html);
                }

                $.ajax({
                    url: "/Admin/Survey/QuestionsLeft?surveyId=" + surveyId,
                    beforeSend: function () {
                    },
                    success: function (html) {
                        if (html) {
                            $("#questionLeft").text(html);
                            if (html == "D") {
                                $("#questionLeft").text('0');
                                $("#finalSubmit").removeAttr('disabled');
                                $("#finalSubmitTitle").removeAttr('title');
                                $('#finalSubmit').popover('show')
                            }
                        }
                    },
                    complete: function () {
                    }
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("change", ".radioQuestion", function (e) {
        e.stopPropagation();
        var ratingContent = $(this).closest('.SuveryRadioOptions');
        var surveyId = ratingContent.attr('data-surveyid');
        var questionId = ratingContent.attr('data-questionid');
        var val = $(this).val();
        $.ajax({
            url: "/Admin/Survey/AddAnswerRadio?surveyId=" + surveyId + "&questionId=" + questionId + "&answerVal=" + val,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                }
                $.ajax({
                    url: "/Admin/Survey/QuestionsLeft?surveyId=" + surveyId,
                    beforeSend: function () {
                    },
                    success: function (html) {
                        if (html) {
                            $("#questionLeft").text(html);
                            if (html == "D") {
                                $("#questionLeft").text('0');
                                $("#finalSubmit").removeAttr('disabled');
                                $("#finalSubmitTitle").removeAttr('title');
                                $('#finalSubmit').popover('show')
                            }
                        }
                    },
                    complete: function () {
                    }
                });
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("mouseup", "[data-hidemenu]", function (e) {
        e.stopPropagation();
        if (isShowing) {
            $('#navigationBar').hide();
            isShowing = false;
        }
        else {
            $('#navigationBar').show();
            isShowing = true;
        }
    });

    $("div").on("click", "[data-showpostpanel]", function (e) {
        e.stopPropagation();
        $('#previewOneItemArea').slideUp(500, function () { $('#postItemsArea').slideDown(500); });
    });

    $("div").on("click", "[data-closepanel]", function (e) {
        e.stopPropagation();
        var panelId = $(this).attr('data-paneltocloseid');
        $('#' + panelId).html('');
        //$('#' + panelId).slideUp(500, function () { $('#' + panelId).html(''); });
    });

    $("div").on("click", "[data-updateleader]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-updateaddress');
        $.ajax({
            url: url,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#leadersPanel").html(html);
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $('body').click(function (evt) {

        if (evt.target.id == "messageMenu1")
            return;
        if ($(evt.target).closest('#messageMenu1').length)
            return;
        if (evt.target.id == "messageMenu2")
            return;
        if ($(evt.target).closest('#messageMenu2').length)
            return;
        if (evt.target.id == "modalDialog1")
            return;
        if ($(evt.target).closest('#modalDialog1').length)
            return;
        closeMessagePart();

        if (evt.target.id == "notificationMenu1")
            return;
        if ($(evt.target).closest('#notificationMenu1').length)
            return;
        if (evt.target.id == "notificationMenu2")
            return;
        if ($(evt.target).closest('#notificationMenu2').length)
            return;

        closeNotifPart();

    });

    $("div").on("click", "[data-messagebutton]", function (e) {
        e.stopPropagation();
        closeNotifPart();
        var type = $(this).attr('data-messagebuttontype');
        var open = $(this).attr('data-messageopen');
        var url = $(this).attr('data-messageurl');
        if (type == '1') {
            if (typeof open == 'undefined' || open == false) {
                $('#messageMenu1').show();
                $(this).attr('data-messageopen', 'true');
                $.ajax({
                    url: url,
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (html) {
                        if (html) {
                            $('#messageMenu1').find('.inboxmessages').html(html);
                            $('#messageMenu1').find('.inboxmessagesloading').hide();
                            $('#messageMenu1').attr('data-messageopenmenu', 'true');
                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });

            }
            else {
                $('#messageMenu1').hide();
                $(this).removeAttr('data-messageopen');
                $('#messageMenu1').removeAttr('data-messageopenmenu');
                $('#messageMenu1').find('.inboxmessages').html('');
                $('#messageMenu1').find('.inboxmessagesloading').show();
            }

        }
        else if (type == '2') {
            if (typeof open == 'undefined' || open == false) {
                $('#messageMenu2').show();
                $(this).attr('data-messageopen', 'true');
                $.ajax({
                    url: url,
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (html) {
                        if (html) {
                            $('#messageMenu2').find('.inboxmessages').html(html);
                            $('#messageMenu2').find('.inboxmessagesloading').hide();
                            $('#messageMenu2').attr('data-messageopenmenu', 'true');

                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });
            }
            else {
                $('#messageMenu2').hide();
                $(this).removeAttr('data-messageopen');
                $('#messageMenu2').removeAttr('data-messageopenmenu');
                $('#messageMenu2').find('.inboxmessages').html('');
                $('#messageMenu2').find('.inboxmessagesloading').show();
            }
        }

    });

    $("div").on("click", "[data-closemodal]", function (e) {
        e.stopPropagation();
        $(this).closest('.modal').modal('hide');
        $(this).closest('.modal-content').html('');
    });

    $("div").on("click", "[data-openmessage]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-openmessage');
        var thisItem = $(this);
        $.ajax({
            url: url,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $('#modalDialogContent1').html(html);
                    $('#modalDialog1').modal('show');
                    thisItem.find('.messageDiv').removeClass('messageDivUnSeen').addClass('messageDivSeen');
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("click", "[data-getmore]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-getmore');
        var page = $(this).attr('data-page');
        var lastId = $(this).attr('data-lastmessageid');
        var cnt = $(this).attr('data-cnt');
        var thisItem = $(this);
        if (parseInt(page) > 0)
            $.ajax({
                url: url + '?page=' + page + '&lastId=' + lastId + '&cnt=' + cnt,
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    if (html == "OK") {
                        thisItem.find('.moreToShow').hide();
                        thisItem.find('.noMoreToShow').show();
                        thisItem.attr('data-page', '-1');
                        thisItem.css('cursor', 'default');
                    }
                    else {
                        $("#moreMessages").prepend(html);
                        thisItem.attr('data-page', parseInt(page) + 1);
                    }
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
    });

    $("div").on("click", "[data-notifbutton]", function (e) {
        e.stopPropagation();
        closeMessagePart();
        var type = $(this).attr('data-notifbuttontype');
        var open = $(this).attr('data-notifopen');
        var url = $(this).attr('data-notifurl');
        if (type == '1') {
            if (typeof open == 'undefined' || open == false) {
                $('#notificationMenu1').show();
                $(this).attr('data-notifopen', 'true');
                $.ajax({
                    url: url,
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (html) {
                        if (html) {
                            $('#notificationMenu1').find('.notifmessages').html(html);
                            $('#notificationMenu1').find('.notifmessagesloading').hide();
                            $('#notificationMenu1').attr('data-notifopenmenu', 'true');
                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });
            }
            else {
                $('#notificationMenu1').hide();
                $(this).removeAttr('data-notifopen');
                $('#notificationMenu1').removeAttr('data-notifopenmenu');
                $('#notificationMenu1').find('.notifmessages').html('');
                $('#notificationMenu1').find('.notifmessagesloading').show();
            }
        }
        else if (type == '2') {
            if (typeof open == 'undefined' || open == false) {
                $('#notificationMenu2').show();
                $(this).attr('data-notifopen', 'true');
                $.ajax({
                    url: url,
                    beforeSend: function () {
                        $('#loadingAjax').show();
                    },
                    success: function (html) {
                        if (html) {
                            $('#notificationMenu2').find('.notifmessages').html(html);
                            $('#notificationMenu2').find('.notifmessagesloading').hide();
                            $('#notificationMenu2').attr('data-notifopenmenu', 'true');

                        }
                    },
                    complete: function () {
                        $('#loadingAjax').hide();
                    }
                });
            }
            else {
                $('#notificationMenu2').hide();
                $(this).removeAttr('data-notifopen');
                $('#notificationMenu2').removeAttr('data-notifopenmenu');
                $('#notificationMenu2').find('.notifmessages').html('');
                $('#notificationMenu2').find('.notifmessagesloading').show();
            }
        }

    });

    $("div").on("click", "[data-getmorenotif]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-getmorenotif');
        var page = $(this).attr('data-page');
        var thisItem = $(this);
        if (parseInt(page) > 0)
            $.ajax({
                url: url + '?page=' + page,
                beforeSend: function () {
                    $('#loadingAjax').show();
                },
                success: function (html) {
                    if (html == "OK") {
                        thisItem.find('.moreNotifToShow').hide();
                        thisItem.find('.noMoreNotifToShow').show();
                        thisItem.attr('data-page', '-1');
                        thisItem.css('cursor', 'default');
                    }
                    else {
                        $("#moreNotifications").prepend(html);
                        thisItem.attr('data-page', parseInt(page) + 1);
                    }
                },
                complete: function () {
                    $('#loadingAjax').hide();
                }
            });
    });

    $("div").on("click", ".notifDivUnSeen", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-updateurl');
        var thisItem = $(this);
        $.ajax({
            url: url,
            beforeSend: function () {
                // $('#loadingAjax').show();
            },
            success: function (html) {
                if (html == "OK") {
                    thisItem.removeClass('notifDivUnSeen');
                    thisItem.addClass('notifDivSeen');
                }
            },
            complete: function () {
                // $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("click", "[data-objectlookupurl]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-objectlookupurl');
        var thisItem = $(this);
        $.ajax({
            url: url,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $("#previewOneItemArea").html(html);
                    $("#previewOneItemArea").show();

                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("click", "[data-togglecontent]", function (e) {
        e.stopPropagation();
        var thisItem = $(this);
        if (thisItem.parents('.lowerPart').length) {
            thisItem.parents('.lowerPart').slideUp(600);
            thisItem.parents('.lowerPart').siblings('.upperPart').slideDown(600, function () {
                //$(this).html('');
            });
        }

    });

    $("div").on("click", "[data-showcoursesummary]", function (e) {
        e.stopPropagation();
        var url = $(this).attr('data-showcoursesummary');
        var thisItem = $(this);
        $.ajax({
            url: url,
            beforeSend: function () {
                $('#loadingAjax').show();
            },
            success: function (html) {
                if (html) {
                    $('#coursePreview').html(html);
                }
            },
            complete: function () {
                $('#loadingAjax').hide();
            }
        });
    });

    $("div").on("click", "[data-courseprevdelete]", function (e) {
        e.stopPropagation();
        $('#coursePreview').html('');
    });

});

