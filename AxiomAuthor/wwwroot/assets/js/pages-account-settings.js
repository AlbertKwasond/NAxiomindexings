'use strict';

document.addEventListener('DOMContentLoaded', function (e) {
    (function () {
        const deactivateAcc = document.querySelector('#formAccountDeactivation');

        // Update/reset user image of account page
        let accountUserImage = document.getElementById('updateuploadedAvatar');
        const fileInput = document.querySelector('#uploads'),
            resetFileInput = document.querySelector('.account-image-reset');

        if (accountUserImage) {
            const resetImage = accountUserImage.src;
            console.log('Original image URL:', resetImage);

            fileInput.addEventListener('change', function () {
                console.log('File input change event triggered');
                if (fileInput.files && fileInput.files[0]) {
                    accountUserImage.src = window.URL.createObjectURL(fileInput.files[0]);
                }
            });

            resetFileInput.addEventListener('click', function () {
                console.log('Reset button click event triggered');
                fileInput.value = '';
                accountUserImage.src = resetImage;
            });
        }
    })();
});