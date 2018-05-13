
//Autoresize textarea while typing
jQuery.each(jQuery('textarea[data-autoresize]'), function () {
    var offset = this.offsetHeight - this.clientHeight;

    var resizeTextarea = function (el) {
        jQuery(el).css('height', 'auto').css('height', el.scrollHeight + offset);
    };
    jQuery(this).on('keyup input', function () { resizeTextarea(this); }).removeAttr('data-autoresize');
});

$(function () {
    //toggle news form
    $('.body').focusin(function () {
        $('.body hr').show();
        $('.body input[type="text"]').show();
        $('.body button').show();
    });
      
    $('#file').change(function (event) {
        $('#imageName').html($(event.target).val().split('\\').pop());
    });
})

//post form on enter
function postOnEnter(event, element) {
    if (event.keyCode == 13) {
        $(element).parent('form')[0].submit();
        return false;
    }
}

function readMore(sender) {
    var id = $(sender).data('newsId');
    $('.full-content-' + id).show();
    $('.short-content-' + id).hide();
    return false;
}

function showComments(target) {
    var id = $(target).data('newsId');
    $('.comment-' + id).toggle();
}