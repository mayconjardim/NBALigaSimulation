function initializeAutocomplete(availableTags, inputId) {
    var tagNames = availableTags.map(function(player) {
        return player.name + " " + "(" + player.id + ")";
    });

    $("#" + inputId).autocomplete({
        source: tagNames
    });
}