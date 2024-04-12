// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function confirmDelete(uniqueId, isDeleteClicked) {
//    var deleteSpan = 'deleteSpan_' + uniqueId;
//    var confirmDeleteSpan = 'confirmDeleteSpan_' + uniqueId;

//    if (isDeleteClicked) {
//        $('#' + deleteSpan).hide();
//        $('#' + confirmDeleteSpan).show();
//    } else {
//        $('#' + deleteSpan).show();
//        $('#' + confirmDeleteSpan).hide();
//    }
//}

function confirmDelete(productId, confirm) {
    var confirmSpan = document.getElementById('confirmDeleteSpan_' + productId);
    var deleteSpan = document.getElementById('deleteSpan_' + productId);

    if (confirm) {
        confirmSpan.style.display = 'inline';
        deleteSpan.style.display = 'none';
    } else {
        confirmSpan.style.display = 'none';
        deleteSpan.style.display = 'inline';
    }
}

function submitDelete(productId) {
    var form = document.getElementById('deleteForm_' + productId);
    form.submit();
}
