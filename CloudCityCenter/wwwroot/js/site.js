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

    const addAllForm = document.getElementById('add-all-form');
    if (addAllForm) {
        addAllForm.addEventListener('submit', async function (e) {
            if (addAllForm.dataset.ajax === 'true') {
                e.preventDefault();
                const formData = new FormData(addAllForm);
                await fetch(addAllForm.action, {
                    method: 'POST',
                    body: formData,
                    credentials: 'same-origin'
                });
                window.location.href = '/Cart';
            }
        });
    }

    document.querySelectorAll('.add-to-cart-form').forEach(function (form) {
        form.addEventListener('submit', async function (e) {
            if (form.dataset.ajax === 'true') {
                e.preventDefault();
                const formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    credentials: 'same-origin'
                });
                if (response.ok) {
                    const cartCountEl = document.getElementById('cart-count');
                    if (cartCountEl) {
                        let count = parseInt(cartCountEl.textContent || '0', 10);
                        count++;
                        cartCountEl.textContent = count;
                        cartCountEl.style.display = count > 0 ? 'inline-block' : 'none';
                    }
                }
            }
        });
    });
});
