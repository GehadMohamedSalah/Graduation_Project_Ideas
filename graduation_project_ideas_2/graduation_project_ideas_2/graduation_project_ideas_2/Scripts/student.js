
$("#register-btn").click(function () {
    $.ajax({
        type: "POST",
        url: "/Student/AddMember",
        success: function (response) {
            $("#register-form").slideToggle(1000);
        }

    })

})



$("#update-btn").click(function () {
    $.ajax({
        type: "POST",
        url: "/Student/AddMember",
        success: function (response) {
            $("#update-members").slideToggle(1000);
        }

    })

})
