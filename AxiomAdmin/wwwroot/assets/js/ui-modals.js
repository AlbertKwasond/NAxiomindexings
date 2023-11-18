/**
 * UI Modals
 */

'use strict';

(function () {
    // Get the modal dialog with the ID #youTubeModal
    const youTubeModal = document.querySelector('#youTubeModal');

    // Check if the modal dialog exists
    if (youTubeModal !== null) {
        // Get the iframe element inside the modal dialog
        const youTubeModalVideo = youTubeModal.querySelector('iframe');

        // Set up an event listener on the modal dialog to stop the video when it is hidden
        youTubeModal.addEventListener('hidden.bs.modal', function () {
            youTubeModalVideo.setAttribute('src', '');
        });
    }

    // Function to get and auto-play YouTube video
    const autoPlayYouTubeModal = function () {
        // Select all elements with the data-bs-toggle="modal" attribute
        const modalTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="modal"]'));

        // Set up an onclick event handler on each of the selected elements
        modalTriggerList.map(function (modalTriggerEl) {
            modalTriggerEl.onclick = function () {
                // Get the data-bs-target and data-theVideo attributes from the clicked element
                const theModal = this.getAttribute('data-bs-target'),
                    videoSRC = this.getAttribute('data-theVideo'),
                    // Construct the URL for the video with autoplay enabled
                    videoSRCauto = `${videoSRC}?autoplay=1`,
                    // Get the iframe element inside the target modal dialog
                    modalVideo = document.querySelector(`${theModal} iframe`);

                // Check if the modal dialog exists
                if (modalVideo !== null) {
                    // Set the src attribute of the iframe element to the constructed URL
                    modalVideo.setAttribute('src', videoSRCauto);
                }
            };
        });
    };

    // Call the function on load
    autoPlayYouTubeModal();
})();