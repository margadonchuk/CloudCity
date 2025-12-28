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

    // Функция для показа toast-уведомления
    function showToast(message, type = 'success') {
        // Создаем контейнер для toast, если его еще нет
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'position-fixed end-0 p-3 toast-container-below-nav';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }

        // Создаем toast элемент
        const toastId = 'toast-' + Date.now();
        const toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-circle'} me-2"></i>
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement, { delay: 3000 });
        toast.show();
        
        // Удаляем элемент после скрытия
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    }

    document.querySelectorAll('.add-to-cart-form').forEach(function (form) {
        form.addEventListener('submit', async function (e) {
            if (form.dataset.ajax === 'true') {
                e.preventDefault();
                
                const submitButton = form.querySelector('button[type="submit"]');
                const originalButtonText = submitButton ? submitButton.innerHTML : '';
                const originalButtonDisabled = submitButton ? submitButton.disabled : false;
                
                // Отключаем кнопку и меняем текст
                if (submitButton) {
                    submitButton.disabled = true;
                    const addingText = window.cartLocalization?.adding || 'Adding...';
                    submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>' + addingText;
                }
                
                try {
                    const formData = new FormData(form);
                    
                    // Проверяем, нужно ли добавить услугу настройки
                    const includeSetup = form.dataset.includeSetup === 'true';
                    if (includeSetup) {
                        const setupCheckbox = form.closest('.card').querySelector('.setup-service-checkbox');
                        if (setupCheckbox && setupCheckbox.checked) {
                            const setupPrice = parseFloat(formData.get('setupServicePrice') || '0');
                            formData.set('includeSetupService', 'true');
                            formData.set('setupServicePrice', setupPrice.toString());
                        } else {
                            formData.set('includeSetupService', 'false');
                        }
                    }
                    
                    const response = await fetch(form.action, {
                        method: 'POST',
                        headers: {
                            'X-Requested-With': 'XMLHttpRequest'
                        },
                        body: formData,
                        credentials: 'same-origin'
                    });
                    
                    if (response.ok) {
                        const result = await response.json();
                        
                        // Обновляем счетчики корзины
                        const cartCountEl = document.getElementById('cart-count');
                        const cartCountMobileEl = document.getElementById('cart-count-mobile-header');
                        const cartCountOffcanvasEl = document.getElementById('cart-count-offcanvas');
                        const cartCountMobileMenuEl = document.getElementById('cart-count-mobile');
                        
                        // Обновляем счетчики корзины (учитываем, что может быть добавлено 2 товара: сервер + услуга)
                        const itemsAdded = result.itemsAdded || 1;
                        
                        if (cartCountEl) {
                            let count = parseInt(cartCountEl.textContent || '0', 10);
                            count += itemsAdded;
                            cartCountEl.textContent = count;
                            cartCountEl.style.display = count > 0 ? 'inline-block' : 'none';
                        }
                        
                        if (cartCountMobileEl) {
                            let count = parseInt(cartCountMobileEl.textContent || '0', 10);
                            count += itemsAdded;
                            cartCountMobileEl.textContent = count;
                            cartCountMobileEl.style.display = count > 0 ? 'inline-block' : 'none';
                        }
                        
                        if (cartCountOffcanvasEl) {
                            let count = parseInt(cartCountOffcanvasEl.textContent || '0', 10);
                            count += itemsAdded;
                            cartCountOffcanvasEl.textContent = count;
                            cartCountOffcanvasEl.style.display = count > 0 ? 'inline-block' : 'none';
                        }
                        
                        if (cartCountMobileMenuEl) {
                            let count = parseInt(cartCountMobileMenuEl.textContent || '0', 10);
                            count += itemsAdded;
                            cartCountMobileMenuEl.textContent = count;
                            cartCountMobileMenuEl.style.display = count > 0 ? 'inline-flex' : 'none';
                        }
                        
                        // Показываем уведомление об успехе
                        const successMessage = result.message || window.cartLocalization?.productAddedToCart || 'Product successfully added to cart!';
                        showToast(successMessage, 'success');
                        
                        // Визуальная обратная связь на кнопке
                        if (submitButton) {
                            const addedText = window.cartLocalization?.added || 'Added!';
                            submitButton.innerHTML = '<i class="bi bi-check-circle me-2"></i>' + addedText;
                            submitButton.classList.remove('btn-primary');
                            submitButton.classList.add('btn-success');
                            
                            // Возвращаем исходное состояние через 2 секунды
                            setTimeout(() => {
                                submitButton.innerHTML = originalButtonText;
                                submitButton.classList.remove('btn-success');
                                submitButton.classList.add('btn-primary');
                                submitButton.disabled = originalButtonDisabled;
                            }, 2000);
                        }
                    } else {
                        // Обработка ошибки
                        const errorMessage = window.cartLocalization?.errorAddingToCart || 'Error adding product to cart';
                        showToast(errorMessage, 'danger');
                        if (submitButton) {
                            submitButton.disabled = originalButtonDisabled;
                            submitButton.innerHTML = originalButtonText;
                        }
                    }
                } catch (error) {
                    console.error('Error adding to cart:', error);
                    const errorMessage = window.cartLocalization?.errorOccurred || 'An error occurred. Please try again.';
                    showToast(errorMessage, 'danger');
                    if (submitButton) {
                        submitButton.disabled = originalButtonDisabled;
                        submitButton.innerHTML = originalButtonText;
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

    // Initialize language dropdown
    const languageDropdown = document.getElementById('languageDropdown');
    if (languageDropdown && typeof bootstrap !== 'undefined') {
        // Ensure Bootstrap dropdown is initialized
        const dropdownElement = languageDropdown.closest('.dropdown');
        if (dropdownElement) {
            // Initialize dropdown if not already initialized
            try {
                new bootstrap.Dropdown(languageDropdown);
            } catch (e) {
                // Dropdown might already be initialized, that's okay
                console.log('Language dropdown already initialized');
            }
        }
    }
});
