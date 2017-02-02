﻿document.getElementById("submitBtn").addEventListener("click", function () {
    addText();
});
setFocusToTextBox();

function scrollToBottom() {
    // Scroll the output container to the bottom
    $("#outputOuter").animate({
        scrollTop: $("#outputOuter").prop("scrollHeight")
    }, 350);
}
// Remove loading styling
function updateContainer() {

    scrollToBottom();

    // Remove the loading style and re-enable the inputs
    $("#mainInput").toggleClass("loading-background");
    $("#mainInput").prop("disabled", false);
    $("#submitBtn").prop("disabled", false);
    setFocusToTextBox();

    // Check if there any modals that need to be shown
    checkShowModal();

    // Check if any of those modals have select2 boxes
    checkFormatSelect2();
}

function checkModalWithSearch() {
    // Check if there any modals that need to be shown
    checkShowModal();
    scrollToBottom();
    // Check if any of those modals have select2 boxes
    checkFormatSelect2();
}
// pop the searchbar text up into the feed
function addText() {
    var div = document.getElementById('outputContainer');
    var searchTerm = document.getElementById('mainInput').value;
    var htmlCode = '<div class="bubbleLine"><div class="bubble rightBubble">' + searchTerm + '</div></div>';
    div.innerHTML = div.innerHTML + htmlCode;
    scrollToBottom();
}

// Show loading styling (loading circle, disabled box)
function startLoading() {
    // Remove error states
    $("#mainInputGroup").removeClass("has-error");
    $("#submitBtn").removeClass("btn-danger");

    // Disable the inputs
    $("#mainInput").prop("disabled", true);
    $("#submitBtn").prop("disabled", true);

    // Clear the textbox
    $("#mainInput").val("");

    // Format the textbox
    $("#mainInput").toggleClass("loading-background");
}

// Show style for an error state
function ajaxFailure() {
    $("#mainInputGroup").addClass("has-error");
    $("#submitBtn").addClass("btn-danger");
}

function showLoadingButton(id) {
    var button = $(document.getElementById(id)).find(".loading-button");
    $(button).find(".loading-button-icon").css("display", "inline-block");
    $(button).prop("disabled", true);
}


function setFocusToTextBox() {
    document.getElementById("mainInput").focus();
}


// Check if the webpage has any modals to display
function checkShowModal() {
    var node = $("#outputContainer").find(".showModal");
    if (node.length > 0) {
        $(node).modal();
        $(node).removeClass("showModal");

        //focus the first input field
        $(node).on('shown.bs.modal', function () {
            $('.modal-body input:text[value=""]:first').focus();
        });

        //delete modals as soon as they are hidden
        $(node).on('hidden.bs.modal', function (e) {
            $(this).remove();
            setFocusToTextBox();
        });
    }
}

function checkFormatSelect2() {
    checkFormatSelect2Donor();
    checkFormatSelect2Batch();
}

function checkFormatSelect2Donor() {
    var node = $("#outputContainer").find(".format-donor-select2");
    if (node.length > 0) {

        $(node).select2({
            ajax: {
                url: "/Donors/SearchDonors/",
                dataType: 'json',
                delay: 500,
                data: function (params) {
                    return {
                        searchKey: params.term
                    };
                },
                cache: true
            },
            placeholder: { id: -1, text: "Select a donor" },
            templateResult: function (state) {
                if (state.id === undefined) {
                    return 'Searching...';
                }

                return state.firstName + " " + state.lastName;
            },
            templateSelection: function (state) {
                if (state.id === -1) {
                    return state.text;
                }
                return state.firstName || state.text;
            },
            minimumInputLength: 1,
            dropdownParent: $(node).parents(".modal")
        });

        $(node).removeClass("format-donor-select2");
    }
}

function checkFormatSelect2Batch() {
    var node = $("#outputContainer").find(".format-batch-select2");
    if (node.length > 0) {

        $(node).select2({
            ajax: {
                url: "/Batch/SearchBatches/",
                dataType: 'json',
                delay: 500,
                data: function (params) {
                    return {
                        searchKey: params.term
                    };
                },
                cache: true
            },
            placeholder: { id: -1, text: "Select a batch" },
            templateResult: function (state) {
                if (state.id === undefined) {
                    return 'Searching...';
                }

                return state.title;
            },
            templateSelection: function (state) {
                if (state.id === -1) {
                    return state.text;
                }
                return state.title || state.text;
            },
            minimumInputLength: 1,
            dropdownParent: $(node).parents(".modal")
        });

        $(node).removeClass("format-batch-select2");
    }
}

function closeModal(id) {
    $('#' + id).modal('hide');
    scrollToBottom();
}

var clickedButtonMap = {};
function bounceInput(id) {
    if (clickedButtonMap[id] === true) {
        return false;
    }

    clickedButtonMap[id] = true;
}

function enableButtonAndCheckShowModal(id) {
    clickedButtonMap[id] = false;
    checkShowModal();
}

function enableButtonAndCheckSearch(id) {
    clickedButtonMap[id] = false;
    checkModalWithSearch();
}

function enableButtonAndScroll(id) {
    clickedButtonMap[id] = false;
    scrollToBottom();
}

/*$(function () {
    $.fn.modal.Constructor.prototype.enforceFocus = function () { };
})*/