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
                    const cartCountMobileEl = document.getElementById('cart-count-mobile-header');
                    
                    if (cartCountEl) {
                        let count = parseInt(cartCountEl.textContent || '0', 10);
                        count++;
                        cartCountEl.textContent = count;
                        cartCountEl.style.display = count > 0 ? 'inline-block' : 'none';
                    }
                    
                    if (cartCountMobileEl) {
                        let count = parseInt(cartCountMobileEl.textContent || '0', 10);
                        count++;
                        cartCountMobileEl.textContent = count;
                        cartCountMobileEl.style.display = count > 0 ? 'inline-block' : 'none';
                    }
                }
            }
        });
    });

    // Scroll animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -100px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    document.querySelectorAll('.animate-on-scroll').forEach(el => {
        observer.observe(el);
    });

    // Counter animation for hero stats
    function animateCounter(element, target, duration = 2000) {
        const start = 0;
        const increment = target / (duration / 16);
        let current = start;

        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                element.textContent = target.toFixed(target % 1 === 0 ? 0 : 1);
                clearInterval(timer);
            } else {
                element.textContent = current.toFixed(target % 1 === 0 ? 0 : 1);
            }
        }, 16);
    }

    const statNumbers = document.querySelectorAll('.stat-number[data-count]');
    const statsObserver = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const target = parseFloat(entry.target.getAttribute('data-count'));
                animateCounter(entry.target, target);
                statsObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.5 });

    statNumbers.forEach(stat => {
        statsObserver.observe(stat);
    });

    // Smooth scroll for buttons
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            if (href !== '#' && href.length > 1) {
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // Scroll to top button
    const scrollTopBtn = document.getElementById('scrollTopBtn');
    if (scrollTopBtn) {
        // Show/hide button based on scroll position
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollTopBtn.classList.add('show');
            } else {
                scrollTopBtn.classList.remove('show');
            }
        });

        // Smooth scroll to top when button is clicked
        scrollTopBtn.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }

    // Анимация для payment badges
    const paymentBadges = document.querySelectorAll('.payment-badge-item');
    const paymentObserver = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animated');
                paymentObserver.unobserve(entry.target);
            }
        });
    }, { threshold: 0.2 });

    paymentBadges.forEach(badge => {
        paymentObserver.observe(badge);
    });

    // Анимация для datacenter pins при наведении
    const datacenterPins = document.querySelectorAll('.datacenter-pin');
    datacenterPins.forEach(pin => {
        pin.addEventListener('mouseenter', function() {
            this.style.animation = 'pin-pulse 0.5s ease-in-out';
        });
        pin.addEventListener('animationend', function() {
            this.style.animation = 'pin-pulse 2s ease-in-out infinite';
        });
    });
});
