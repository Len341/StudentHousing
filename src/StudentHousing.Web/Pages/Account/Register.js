
function registerTypeChange(e, sibling) {
    $(e.firstElementChild).attr("style", "border : 1px solid red !important; border-radius:5px;");
    $('input[name=registerType]').val($(e).data('registertype'));
    $(sibling.firstElementChild).attr("style", "border : 1px solid #dee2e6 !important; border - radius: 5px;");
    $('#registerBtn').removeAttr('disabled');
}

document.onreadystatechange = function () {

    $('#registerBtn').click(function () {
        $('.registerType').hide();
        $('.register').show();
    });

    $('#back').click(function () {
        location.reload();
    })
}