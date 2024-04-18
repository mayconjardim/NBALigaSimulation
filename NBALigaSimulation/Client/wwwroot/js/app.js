function initializeAutocomplete(availableTags, inputId) {
    var tagNames = availableTags.map(function(player) {
        return player.name + " " + "(" + player.id + ")";
    });

    $("#" + inputId).autocomplete({
        source: tagNames
    });
}

function fecharmodal() {
    var navbarTogglerButton = document.getElementById("modalButton");
    navbarTogglerButton.click();
}

function fecharmenu() {
    var navbarTogglerButton = document.getElementById("navbarTogglerButton");

    var navbarSupportedContent = document.getElementById("navbarSupportedContent");
    if (navbarSupportedContent.classList.contains('show')) {
        navbarTogglerButton.click();
    }
}