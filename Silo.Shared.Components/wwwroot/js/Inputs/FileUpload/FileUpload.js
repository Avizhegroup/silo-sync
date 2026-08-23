function onUploaderInit(dotnetHelper, raiserId, uploadUrl) {
    var inputId = 'up-' + raiserId;

    $('#' + inputId).on('input',function (e) {
        e.stopPropagation();
        if ($(this).val() === '') {
            return;
        }

        dotnetHelper.invokeMethodAsync('InformStartUpload');

        var data = new FormData();
        data.append('file', $(this)[0].files[0]);

        $.ajax({
            type: "POST",
            url: uploadUrl,
            data: data,
            processData: false,
            contentType: false,
            success: function (path) {
                dotnetHelper.invokeMethodAsync('InformUploadPath', path);
                $('#' + inputId).val('');
            },
            fail: function () {
                dotnetHelper.invokeMethodAsync('InformFailUpload');
            }
        });
    });

    // Add drag and drop support for drop zone
    var dropZone = $('#' + raiserId).find('.silo-dropzone');
    if (dropZone.length > 0) {
        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropZone[0].addEventListener(eventName, preventDefaults, false);
        });

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        // Highlight drop zone when item is dragged over it
        ['dragenter', 'dragover'].forEach(eventName => {
            dropZone[0].addEventListener(eventName, highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropZone[0].addEventListener(eventName, unhighlight, false);
        });

        function highlight(e) {
            dropZone.addClass('dragging');
        }

        function unhighlight(e) {
            dropZone.removeClass('dragging');
        }

        // Handle dropped files - only for uploadUrl scenario (regular input)
        if (uploadUrl) {
            dropZone[0].addEventListener('drop', handleDrop, false);

            function handleDrop(e) {
                var dt = e.dataTransfer;
                var files = dt.files;

                if (files.length > 0) {
                    var input = $('#' + inputId)[0];
                    input.files = files;
                    $(input).trigger('input');
                }
            }
        } else {
            // For InputFile (no uploadUrl), handle drop to trigger the InputFile
            dropZone[0].addEventListener('drop', handleDropInputFile, false);

            function handleDropInputFile(e) {
                var dt = e.dataTransfer;
                var files = dt.files;

                if (files.length > 0) {
                    var input = $('#' + inputId)[0];
                    // Create a new FileList and assign it
                    var dataTransfer = new DataTransfer();
                    for (var i = 0; i < files.length; i++) {
                        dataTransfer.items.add(files[i]);
                    }
                    input.files = dataTransfer.files;
                    
                    // Trigger change event for Blazor InputFile
                    var event = new Event('change', { bubbles: true });
                    input.dispatchEvent(event);
                }
            }
        }
    }
}

function onUploaderClick(raiserId) {
    var inputId = 'up-' + raiserId;
    $('#' + inputId).val('');
    $('#' + inputId).trigger('click');
}