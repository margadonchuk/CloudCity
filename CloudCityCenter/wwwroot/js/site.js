// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // #region agent log helpers
    const agentLogEndpoint = '/ingest/38680bd6-8223-4b5d-9453-913b38e9d421';
    const agentSession = 'debug-session';
    const agentRun = 'pre-fix';
    const agentLog = (hypothesisId, location, message, data) => {
        fetch(agentLogEndpoint, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                sessionId: agentSession,
                runId: agentRun,
                hypothesisId,
                location,
                message,
                data,
                timestamp: Date.now()
            })
        }).catch(() => { });
    };
    // #endregion

    // Collapse the responsive menu when a link is clicked
    const navbarCollapse = document.querySelector('.navbar-collapse');
    const navbarToggler = document.querySelector('.navbar-toggler');

    agentLog(
        'H1-bootstrap-or-dom',
        'site.js:DOMContentLoaded',
        'init state',
        {
            bootstrapAvailable: typeof bootstrap !== 'undefined',
            navbarCollapseFound: !!navbarCollapse,
            navbarTogglerFound: !!navbarToggler
        }
    );

    if (navbarCollapse) {
        navbarCollapse.querySelectorAll('a').forEach(function (navLink) {
            navLink.addEventListener('click', function () {
                if (navbarCollapse.classList.contains('show')) {
                    bootstrap.Collapse.getOrCreateInstance(navbarCollapse).hide();
                }
            });
        });

        ['show.bs.collapse', 'shown.bs.collapse'].forEach(function (evt) {
            navbarCollapse.addEventListener(evt, function () {
                const styles = getComputedStyle(navbarCollapse);
                agentLog(
                    'H3-collapse-hidden-by-css',
                    'site.js:navbarCollapse ' + evt,
                    'collapse event',
                    {
                        event: evt,
                        display: styles.display,
                        visibility: styles.visibility,
                        maxHeight: styles.maxHeight,
                        isShownClass: navbarCollapse.classList.contains('show')
                    }
                );
            });
        });
    }

    if (navbarToggler) {
        navbarToggler.addEventListener('click', function () {
            const styles = getComputedStyle(navbarToggler);
            agentLog(
                'H2-toggler-click',
                'site.js:navbarToggler click',
                'toggler clicked',
                {
                    pointerEvents: styles.pointerEvents,
                    zIndex: styles.zIndex,
                    opacity: styles.opacity,
                    collapseHasShowClass: navbarCollapse ? navbarCollapse.classList.contains('show') : null
                }
            );
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
