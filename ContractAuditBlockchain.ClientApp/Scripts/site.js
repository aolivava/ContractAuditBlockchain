function _showValidationError(data, prefix) {
    var msg = data.message;

    $('#' + prefix + 'ErrorList').html(msg);
    $('#' + prefix + 'Errors').removeClass('hidden');
    $('#' + prefix + ' button[type="submit"]').button('reset');
}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
