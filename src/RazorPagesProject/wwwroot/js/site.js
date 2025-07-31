// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Clear All confirmation dialog functionality
$(document).ready(function () {
    $('#deleteAllBtn').on('click', function (e) {
        e.preventDefault(); // Prevent immediate form submission
        
        // Count the number of messages
        var messageCount = $('.message-list').length;
        
        if (messageCount === 0) {
            // No messages to delete, just show a message
            alert('There are no messages to delete.');
            return false;
        }
        
        // Create confirmation message with count
        var confirmMessage = messageCount === 1 
            ? 'Are you sure you want to delete 1 message? This action cannot be undone.'
            : 'Are you sure you want to delete ' + messageCount + ' messages? This action cannot be undone.';
        
        // Show confirmation dialog
        if (confirm(confirmMessage)) {
            // User confirmed, submit the form
            $(this).closest('form').submit();
        }
        // If user cancelled, do nothing (return false is implicit)
    });
});
