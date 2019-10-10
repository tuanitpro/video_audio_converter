// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    var bar = $('.progress-bar');
    var percent = $('.progress-bar');
    var status = $('#status');
    var percentVal = '0%';
    $("#convertResult").hide();
    $(".progress").hide();
    $(".fileImg").change(function () {
        percentVal = '0%';
        $("#convertResult").hide();
        var ok = ValidateSingleInput(this); // check dieu kien neu can
        if (ok) {
            var data = new FormData();
            var files = $(".fileImg").get(0).files;
            if (files.length > 0) {
                for (let i = 0; i < files.length; i++) {                   
                    $("#chooseFile").append("<br/>" + files[i].name);
                    data.append("files", files[i]);
                }               
            }
         
            $.ajax({
                url: "/api/v1/fileupload",
                type: "POST",
                processData: false,
                contentType: false,
                data: data,

                beforeSend: function () {
                    status.empty();
                    $(".progress").show();
                    percentVal = '0%';
                    bar.width(percentVal)
                    percent.html(percentVal);
                },
                uploadProgress: function (event, position, total, percentComplete) {
                    var percentVal = percentComplete + '%';
                    bar.width(percentVal)
                    percent.html(percentVal);
                },

                success: function (response) {                                         
                    var percentVal = '100%';
                    bar.width(percentVal)
                    percent.html(percentVal);
                    var data = response.data;
                    for (let i = 0; i < data.length; i++) {
                        percentVal = '0%';
                        bar.width(percentVal)
                        percent.html(percentVal);
                        convertAudioWmaToMp3(data[i]);                        
                    }
                    $("#convertResult").show();
                },
                error: function () { },
                complete: function () { }
            });
        }
    });

    $('#chooseFile').click(function () {
        $(".fileImg").trigger("click");
    });

    function convertAudioWmaToMp3(fileName) {
        $.ajax({
            url: "/api/v1/convert_wma_mp3",
            type: "POST",
            dataType: "json",
            contentType: 'application/x-www-form-urlencoded',
            data: { FileName: fileName },

            beforeSend: function () {
                status.empty();
                $(".progress").show();
                percentVal = '0%';
                bar.width(percentVal)
                percent.html(percentVal);
            },
            uploadProgress: function (event, position, total, percentComplete) {
                var percentVal = percentComplete + '%';
                bar.width(percentVal)
                percent.html(percentVal);
            },

            success: function (response) {
                var percentVal = '100%';
                bar.width(percentVal)
                percent.html(percentVal);
                var data = response.data;
                $("#convertResult").append('<a target="_blank" href="' + data.fileUrl + '" type="application/octet-stream">' + data.fileName + '</a></br>');
            },
            error: function () { },
            complete: function () { }
        });
    }
  
});

var _validFileExtensions = [".wma", ".wav", ".mp4"];
function ValidateSingleInput(oInput) {
    if (oInput.type == "file") {
        var sFileName = oInput.value;
        if (sFileName.length > 0) {
            var blnValid = false;
            for (var j = 0; j < _validFileExtensions.length; j++) {
                var sCurExtension = _validFileExtensions[j];
                if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                    blnValid = true;
                    break;
                }
            }

            if (!blnValid) {
                alert("Lỗi, " + sFileName + " chỉ được chấp nhận định dạng: " + _validFileExtensions.join(", "));
                oInput.value = "";
                return false;
            }
        }
    }
    return true;
}

 