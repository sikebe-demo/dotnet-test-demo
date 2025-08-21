// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Enhanced hamburger menu accessibility features
document.addEventListener('DOMContentLoaded', function() {
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (!navbarToggler || !navbarCollapse) return;
    
    // Handle Esc key to close menu
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && navbarCollapse.classList.contains('show')) {
            navbarToggler.click();
            navbarToggler.focus();
        }
    });
    
    // Basic focus trap for mobile menu
    navbarCollapse.addEventListener('shown.bs.collapse', function() {
        // When menu opens, focus on first link
        const firstLink = navbarCollapse.querySelector('a, button');
        if (firstLink) {
            firstLink.focus();
        }
    });
    
    // Update aria-expanded attribute properly
    navbarToggler.addEventListener('click', function() {
        setTimeout(function() {
            const isExpanded = navbarCollapse.classList.contains('show');
            navbarToggler.setAttribute('aria-expanded', isExpanded.toString());
        }, 10);
    });
});
