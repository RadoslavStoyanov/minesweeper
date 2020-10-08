$(document).ready(function () {
    document.oncontextmenu = function () {
        return false;
    };
});

$(document).on('mousedown', ".element", function (e) {

    if ($('#GameEnded')[0].value === 'True') {
        return;
    }

    var hasBeenFlagged = false;

    if (e.button == 2) { // check if right mouse button was pressed
        hasBeenFlagged = true;
    }

    $.ajax({
        type: 'POST',
        url: 'Home/CheckElement',
        data: {
            row: $(this).data('row'),
            column: $(this).data('column'),
            hasBeenFlagged: hasBeenFlagged
        },
        success: function (data) {
            var newTarget = $('#minesweeper', data).html();
            $('#minesweeper').html(newTarget);
        }
    });
});

function submitform() {
    $('form').submit();
}