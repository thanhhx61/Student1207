//modelDelete and ChangeStatus
var eventRegist = {};
function getEvent(id) {
    $('#basicModal').modal('show');
    $('button#confirm').click(function (e) {
        $('#basicModal').modal('hide');
        $.ajax({
            url: "/Events/Delete/" + id,
            success: function (result) {
                if (result) {
                    $('#cfModal').modal('show');
                }
            }
        });
    });
    return false;
};
function changeStatus23(id) {
    $('#basicModal').modal('show');
    $('button#confirm').click(function (e) {
        $('#basicModal').modal('hide');
        $.ajax({
            url: "/Events/ChangeStatus23/" + id,
            success: function (result) {
                if (result) {
                    $('#cfModal').modal('show');
                }
            }
        });
    });
    return false;
};
eventRegist.reload = function () {
    location.reload();
}
function getChange12(id) {
    $.ajax({
        url: "/Events/ChangeStatus12/" + id,
        type: "GET",

        success: function (data) {
            if (data) {
                window.location.href = "/Events/Edit/" + id;
            } else {
                location.reload();
            }
        }
    });
}