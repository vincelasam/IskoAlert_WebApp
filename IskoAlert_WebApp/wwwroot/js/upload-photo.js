document.addEventListener('DOMContentLoaded', function () {
    const fileInput = document.getElementById('dropzone-file');
    const uploadPlaceholder = document.getElementById('upload-placeholder');
    const fileSelectedDisplay = document.getElementById('file-selected-display');
    const fileName = document.getElementById('file-name');
    const fileSize = document.getElementById('file-size');
    const removeFileBtn = document.getElementById('remove-file-btn');
    const imagePreview = document.getElementById('image-preview');
    const previewImg = document.getElementById('preview-img');

    if (!fileInput) return; // ✅ prevents errors on pages without this form

    // Handle file selection
    fileInput.addEventListener('change', function (e) {
        const file = e.target.files[0];
        if (!file) return;

        const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];

        if (!allowedTypes.includes(file.type)) {
            alert('Only JPG and PNG formats are allowed.');
            fileInput.value = '';
            return;
        }

        if (file.size > 2 * 1024 * 1024) {
            alert('Image size must not exceed 2MB.');
            fileInput.value = '';
            return;
        }

        fileName.textContent = file.name;
        fileSize.textContent = formatFileSize(file.size);

        if (file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImg.src = e.target.result;
                imagePreview.classList.remove('hidden');
            };
            reader.readAsDataURL(file);
        }

        uploadPlaceholder.style.display = 'none';
        fileSelectedDisplay.classList.remove('hidden');
    });

    removeFileBtn.addEventListener('click', function () {
        fileInput.value = '';
        uploadPlaceholder.style.display = 'flex';
        fileSelectedDisplay.classList.add('hidden');
        imagePreview.classList.add('hidden');
        previewImg.src = '';
    });

    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';
        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
    }
});