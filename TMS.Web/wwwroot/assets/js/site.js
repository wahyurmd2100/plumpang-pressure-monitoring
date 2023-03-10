// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });

    $(document).ready(function () {
        var trigger = $('.hamburger'),
            overlay = $('.overlay'),
            isClosed = false;

        trigger.click(function () {
            hamburger_cross();
        });

        function hamburger_cross() {
            if (isClosed == true) {
                overlay.hide();
                trigger.removeClass('is-open');
                trigger.addClass('is-closed');
                isClosed = false;
            } else {
                overlay.show();
                trigger.removeClass('is-closed');
                trigger.addClass('is-open');
                isClosed = true;
            }
        }

        $('[data-toggle="offcanvas"]').click(function () {
            $('#wrapper').toggleClass('toggled');
        });
    });
});

showInPopup = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            console.log(res);
            jQuery.noConflict();
            $("#form-modal .modal-body").html(res);
            $("#form-modal .modal-title").html(title);
            $("#form-modal").modal('show');
        }
    })
}

showInPopupLg = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal-lg .modal-body").html(res);
            $("#form-modal-lg .modal-title").html(title);
            $("#form-modal-lg").modal('show');
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    console.log(res);
                    $('#view-all').html(res.html);
                    location.reload(true);
                    $('#form-modal').modal('close');
                //    $('#form-modal-lg').modal('close');
                //    oTable = $('#tbl-grid').DataTable();
                //    oTable.ajax.reload(null, false);
                //    M.toast({ html: 'Record saved successfully' })
                }
                else {
                    //if (res.html != '') {
                        $('#form-modal .modal-body').html(res.html);
                    //}
                }
            },
            error: function (err) {
                console.log(err);
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex);
    }
}

jQueryAjaxDelete = form => {
    if (confirm('Are you sure want to delete this record ?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#view-all').html(res.html);
                    location.reload(true);
                    M.toast({ html: 'Record deleted successfully' })
                },
                error: function (err) {
                    console.log(err);
                }
            })
        } catch (ex) {
            console.log(ex);
        }
    }

    //prevent default form submit event
    return false;
}

refreshTable = (tblName) => {
    oTable = $('#' + tblName).DataTable();
    oTable.ajax.reload(null, false);
}