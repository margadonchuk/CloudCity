// Web3Forms Handler для отправки форм
// Получите ваш API ключ на https://web3forms.com

(function() {
    'use strict';

    // Конфигурация - замените на ваш API ключ из Web3Forms
    // Получите ключ на https://web3forms.com (введите support@cloudcity.center)
    const WEB3FORMS_ACCESS_KEY = 'YOUR_WEB3FORMS_ACCESS_KEY'; // ⚠️ ЗАМЕНИТЕ НА ВАШ КЛЮЧ!
    const WEB3FORMS_API_URL = 'https://api.web3forms.com/submit';
    const RECIPIENT_EMAIL = 'support@cloudcity.center';
    
    // Проверка, что ключ заменен
    if (WEB3FORMS_ACCESS_KEY === 'YOUR_WEB3FORMS_ACCESS_KEY') {
        console.error('❌ Web3Forms: API ключ не установлен! Получите ключ на https://web3forms.com и обновите web3forms-handler.js');
        console.error('❌ Формы не будут работать до установки API ключа!');
    }

    // Инициализация всех форм с классом 'web3form'
    function initWeb3Forms() {
        const forms = document.querySelectorAll('form.web3form');
        
        forms.forEach(form => {
            form.addEventListener('submit', handleFormSubmit);
            
            // Добавляем скрытое поле с access_key, если его еще нет
            if (!form.querySelector('input[name="access_key"]')) {
                const accessKeyInput = document.createElement('input');
                accessKeyInput.type = 'hidden';
                accessKeyInput.name = 'access_key';
                accessKeyInput.value = WEB3FORMS_ACCESS_KEY;
                form.appendChild(accessKeyInput);
            }

            // Добавляем скрытое поле с email получателя
            if (!form.querySelector('input[name="to_email"]')) {
                const toEmailInput = document.createElement('input');
                toEmailInput.type = 'hidden';
                toEmailInput.name = 'to_email';
                toEmailInput.value = RECIPIENT_EMAIL;
                form.appendChild(toEmailInput);
            }

            // Добавляем поле subject для идентификации источника
            if (!form.querySelector('input[name="subject"]')) {
                const subjectInput = document.createElement('input');
                subjectInput.type = 'hidden';
                subjectInput.name = 'subject';
                subjectInput.value = form.dataset.sourcePage || 'Contact Form';
                form.appendChild(subjectInput);
            }
        });
    }

    async function handleFormSubmit(e) {
        e.preventDefault();
        
        const form = e.target;
        const submitButton = form.querySelector('button[type="submit"]');
        const originalButtonText = submitButton ? submitButton.innerHTML : '';
        
        // Показываем индикатор загрузки
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Отправка...';
        }

        // Собираем данные формы
        const formData = new FormData(form);
        
        // Добавляем дополнительные данные
        formData.append('from_name', formData.get('Name') || 'Неизвестно');
        formData.append('access_key', WEB3FORMS_ACCESS_KEY);

        try {
            const response = await fetch(WEB3FORMS_API_URL, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                showMessage(form, 'success', 'Сообщение успешно отправлено! Мы свяжемся с вами в ближайшее время.');
                form.reset();
                console.log('✅ Web3Forms: Письмо успешно отправлено');
            } else {
                const errorMsg = result.message || 'Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.';
                showMessage(form, 'error', errorMsg);
                console.error('❌ Web3Forms error:', result);
                console.error('Error details:', JSON.stringify(result, null, 2));
            }
        }         catch (error) {
            const errorMsg = error.message || 'Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.';
            showMessage(form, 'error', errorMsg);
            console.error('❌ Form submission error:', error);
            console.error('Error stack:', error.stack);
            
            // Дополнительная диагностика
            if (error.message && error.message.includes('Failed to fetch')) {
                console.error('❌ Проблема с сетевым подключением к Web3Forms API');
            }
        } finally {
            // Восстанавливаем кнопку
            if (submitButton) {
                submitButton.disabled = false;
                submitButton.innerHTML = originalButtonText;
            }
        }
    }

    function showMessage(form, type, message) {
        // Удаляем предыдущие сообщения
        const existingMessage = form.parentElement.querySelector('.form-message');
        if (existingMessage) {
            existingMessage.remove();
        }

        // Создаем новое сообщение
        const messageDiv = document.createElement('div');
        messageDiv.className = `form-message alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show`;
        messageDiv.setAttribute('role', 'alert');
        messageDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;

        // Вставляем сообщение перед формой
        form.parentElement.insertBefore(messageDiv, form);

        // Автоматически скрываем сообщение через 5 секунд
        setTimeout(() => {
            if (messageDiv.parentElement) {
                messageDiv.remove();
            }
        }, 5000);
    }

    // Инициализация при загрузке DOM
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initWeb3Forms);
    } else {
        initWeb3Forms();
    }
})();

