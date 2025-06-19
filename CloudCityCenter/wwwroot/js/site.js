// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // Collapse the responsive menu when a link is clicked
    const navbarCollapse = document.querySelector('.navbar-collapse');
    if (navbarCollapse) {
        navbarCollapse.querySelectorAll('a').forEach(function (navLink) {
            navLink.addEventListener('click', function () {
                if (navbarCollapse.classList.contains('show')) {
                    bootstrap.Collapse.getOrCreateInstance(navbarCollapse).hide();
                }
            });
        });
    }

    // Bootstrap form validation
    const forms = document.querySelectorAll('.needs-validation');
    Array.prototype.slice.call(forms).forEach(function (form) {
        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});
