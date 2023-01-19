// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("#Amount").change(function () {
    let amount = this.value;
    const formatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    });
    $("#confirm-amount").html(formatter.format(amount));
});

$("#Comment").change(function () {
    let comment = this.value;
    $("#confirm-comment").html(comment);
});

$("#confirm-account").change(function () {
    let account = this.value;
    $("#confirm-account").html(account);
});
