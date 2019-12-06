$(function () {

    var limit = 20;
    var offset = 0;
    var ajaxDone = true;
    var morePages = true;
    var editId = 0;
    var postIdForComment = 0;
    var isComment = false;

    getUserPosts();

    $("main").on('click', ".comment-count", showComments);
    $(window).scroll(scrolled);
    $("#add-post").click(showAddPostPopup);
    $("#post-cancel").click(hideAddPostPopup);
    $("#post-submit").click(addPost);
    $("#post-delete").click(deletePost);
    $("main").on("click", ".edit", showEditPostPopup);
    $("main").on("click", ".add-comment", showAddCommentPopup);
    $("main").on("click", ".edit-comment", showEditCommentPopup);

           function showEditCommentPopup() {
        $("#post-delete").show();
        let text = $(this).parent().find(".comment-text").text();
        showEditPostPopup.call($(this).parent().parent().find(".add-comment")[0], text);
        isComment = true;
        postIdForComment = parseInt($(this).parent().parent().find(".comment-count").attr("href"));
        editId = $(this).data("id");
    }

    function showAddCommentPopup() {
        $("#post-delete").hide();
        showAddPostPopup.call($("#add-post")[0]);
        isComment = true;
        postIdForComment = parseInt($(this).parent().find(".comment-count").attr("href"));
    }

    function deletePost() {
        let url = "";

        let data = {
            
        }
        if (!isComment) {
            url = "user-posts/delete-post";
        }
        else {
            url = "user-posts/delete-comment";
        }
        
        $.ajax({
            url: url,
            method: "get",
            dataType: "json",
            data: {
                id: editId
            },
                error: ajaxError,
                success: function (data) {
                    if (data.status) {
                        hideAddPostPopup();
                        $("main #user-post-template ~ .user-post").remove();
                        offset = 0;
                        morePages = true;
                        getUserPosts()
                    } else {
                        alert("Didn't work")
                    }
                }
            });
        }

    function addPost() {
        let url = "";

        let data = {
            text: $("#new-post-popup textarea").val()

        }
        if (!isComment) {
            if (editId > 0) {
                url = "user-posts/update-post"
                data.id = editId;
            } else {
                url = "user-posts/new-post"
            }
        } else {
            data.postId = postIdForComment;
            if (editId > 0) {
                url = "user-posts/update-comment"
                data.id = editId;
            } else {
                url = "user-posts/new-comment";

            }

            $.ajax({
                url: url,
                method: "post",
                dataType: "json",
                data: data,
                error: ajaxError,
                success: function (data) {
                    if (data.status) {
                        hideAddPostPopup();
                        $("main #user-post-template ~ .user-post").remove();
                        offset = 0;
                        morePages = true;
                        getUserPosts()
                    } else {
                        alert("Didn't work")
                    }
                }
            });
        }
    }
        


    function hideAddPostPopup() {
        $("#new-post-popup").hide();
        $("#add-post").removeAttr("disabled");
    }

    function showAddPostPopup() {
        $("#post-delete").hide();
        editId = 0;
        isComment = false;
        $(this).attr("disabled", "disabled");
        $("#new-post-popup textarea").val("");
        $("#new-post-popup").show();
        $("#new-post-popup textarea").focus();

        //console.log("test");
    }

    function showEditPostPopup(text) {
        $("#post-delete").show();
        editId = parseInt($(this).parent().find(".comment-count").attr("href"));
        isComment = false;
        if (typeof text !== "string") {
            var text = $(this).parent().find(".post-content > p").text();
        }
        $("#new-post-popup textarea").val(text);
        $("#new-post-popup").show();
        $("#new-post-popup textarea").focus();
                
    }

    function scrolled() {
        if (ajaxDone && morePages) {
            let docHeight = $(document).height();
            let winHeight = $(window).height();
            let top = $(window).scrollTop();

            if (top >= docHeight - winHeight * 3) {
                //net page of results
                offset += limit;
                getUserPosts();
            }

        }
    }
    function showComments() {
        if ($(this).parent().parent().find(".post-comments-template").next().length == 0) {


            let $this = $(this);
            $.ajax({
                url: "user-posts/get-comments",
                dataType: "json",
                data: {
                    userPostId: $(this).attr("href")
                },
                error: ajaxError,
                success: function (data) {
                    buildComments(data, $this)
                }
            });

        } else {
           // $(this).parent().parent().find(".post-comments:not(.post-comments-template)").remove();
            $(this).parent().parent().find(".post-comments-template ~ .post-comments)").remove();
        }
        return false;
    }

    function  buildComments(data, $atag) {
        console.log(data);

        for (let i = 0; i < data.length; i++) {
            let cmt = data[i];
            let $comment = $atag.parent().next().clone();
            $comment.removeClass("post-comments-template");
            $comment.find("time").text(cmt.edited);
            $comment.find(".username").text(cmt.user.userName);
            $comment.find(".comment-text").text(cmt.text);
            $comment.find(".edit-comment").data("id", cmt.commentId);
            $atag.parent().parent().append($comment);
        }
    }

    function getUserPosts() {
        ajaxDone = false;
        $.ajax({
            url: "user-posts/get-user-posts",
            dataType: "json",
            data: {
                limit: limit,
                offset: offset
            },
            error: function () {
                ajaxDone = true;
                ajaxError();
             },
            success: function (data) {
                ajaxDone = true;
                buildUserPosts(data);
            }
        })
    }

    function ajaxError() {
        alert("Ajax error!");
    }

    function buildUserPosts(data) {
        console.log(data);
        if (data.length < limit) {
            morePages = false;
        }
        for (var i = 0; i < data.length; i++) {
            let postData = data[i]
            let post = $("#user-post-template").clone();
            post.removeAttr("id");
            post.find("figcaption").text(postData.user.userName);
            post.find(".post-content > p").text(postData.text);
            post.find(".post-details > time").text(postData.edited);
            post.find(".post-details > .comment-count").prepend(postData.commentCount)
                .attr("href", postData.userPostId);
            if (!postData.editable) {
                post.find(".edit").remove();
            }
            $("main").append(post);
        }
    }
});