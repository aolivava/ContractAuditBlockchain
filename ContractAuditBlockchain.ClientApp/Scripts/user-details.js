$(document).ready(function () {
    $(".editUserButton").click(function (e) {
        var participantId = $(e.currentTarget).data("participantid");
        if (participantId) {
            $("#" + userDetailsParams.editDiv).load(userDetailsParams.editUrl + "/" + participantId);
        }
    });
    $(".resetUserPasswordButton").click(function (e) {
        var r = confirm("Are you sure you want to reset this user's password?");
        if (r === true) {
            var participantId = $(e.currentTarget).data("participantid");
            if (participantId) {
                var data = { id: participantId };
                var a = $.ajax({
                    type: 'POST',
                    url: userDetailsParams.resetPasswordUrl,
                    data: AddAntiForgeryToken(data)
                }).then(function (data) {
                    if (data.success) {
                        $('#userResetPasswordErrors').addClass('hidden');
                        alert("Password has been reset.");
                    } else {
                        _showValidationError(data, 'userResetPassword');
                    }
                });
            }
        }
    });
    $(".detailsContractButton").click(function (e) {
        var contractId = $(e.currentTarget).data("contractid");
        if (contractId) {
            window.location = userDetailsParams.contractUrl + "/" + contractId;
        }
    });

    $(".createContractButton").click(function (e) {
        var participantId = $(e.currentTarget).data("participantid");
        if (participantId) {
            $("#" + userDetailsParams.contractDiv).load(userDetailsParams.createContractUrl + "/" + participantId);
        }
    });
});
