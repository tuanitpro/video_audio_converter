// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    //Dropzone.autoDiscover = false;
    $("#id_dropzone").dropzone({
        maxFiles: 2000,
        url: "/api/v1/fileupload/",
        success: function (file, response) {
            console.log(response);
        }
    });

     
});


// var myDropzone = new Dropzone("#myId", { url: "/file/post" });