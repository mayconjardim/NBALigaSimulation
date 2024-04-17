function initializeAutocomplete(availableTags, inputId) {
    var tagNames = availableTags.map(function(player) {
        return player.name + " " + "(" + player.id + ")";
    });

    $("#" + inputId).autocomplete({
        source: tagNames
    });
}

$('#closemodal').click(function() {
    $('#loginModel').modal('hide');
});

$('.nav-link').on('click',function() {
    $('.navbar-collapse').collapse('hide');
});