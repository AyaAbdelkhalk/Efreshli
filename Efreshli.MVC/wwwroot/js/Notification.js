// Enhanced Notification System
class NotificationManager {
    constructor() {
        this.notifications = [];
        this.container = this.createContainer();
    }

    createContainer() {
        const container = document.createElement('div');
        container.className = 'notifications-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 2000;
            pointer-events: none;
        `;
        document.body.appendChild(container);
        return container;
    }

    show(message, type = 'info', duration = 5000) {
        const notification = this.createNotification(message, type);
        this.container.appendChild(notification);
        this.notifications.push(notification);

        // Trigger animation
        setTimeout(() => {
            notification.classList.add('show');
        }, 100);

        // Auto remove
        if (duration > 0) {
            setTimeout(() => {
                this.remove(notification);
            }, duration);
        }

        return notification;
    }

    createNotification(message, type) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.style.cssText = `
            background: ${this.getBackgroundColor(type)};
            color: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
            transform: translateX(400px);
            transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
            pointer-events: auto;
            display: flex;
            align-items: center;
            gap: 10px;
            max-width: 350px;
            word-wrap: break-word;
        `;

        const icon = document.createElement('i');
        icon.className = this.getIcon(type);
        icon.style.fontSize = '18px';

        const content = document.createElement('span');
        content.textContent = message;
        content.style.flex = '1';

        const closeBtn = document.createElement('button');
        closeBtn.innerHTML = '&times;';
        closeBtn.className = 'notification-close';
        closeBtn.style.cssText = `
            background: none;
            border: none;
            color: white;
            font-size: 20px;
            cursor: pointer;
            opacity: 0.8;
            padding: 0;
            margin-left: auto;
        `;

        closeBtn.addEventListener('click', () => this.remove(notification));

        notification.appendChild(icon);
        notification.appendChild(content);
        notification.appendChild(closeBtn);

        return notification;
    }

    getBackgroundColor(type) {
        const colors = {
            success: 'linear-gradient(135deg, #28a745, #20c997)',
            error: 'linear-gradient(135deg, #dc3545, #e74c3c)',
            warning: 'linear-gradient(135deg, #ffc107, #fd7e14)',
            info: 'linear-gradient(135deg, #007bff, #17a2b8)'
        };
        return colors[type] || colors.info;
    }

    getIcon(type) {
        const icons = {
            success: 'fas fa-check-circle',
            error: 'fas fa-exclamation-circle',
            warning: 'fas fa-exclamation-triangle',
            info: 'fas fa-info-circle'
        };
        return icons[type] || icons.info;
    }

    remove(notification) {
        if (!notification || !notification.parentNode) return;

        notification.classList.remove('show');
        notification.style.transform = 'translateX(400px)';

        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }

            const index = this.notifications.indexOf(notification);
            if (index > -1) {
                this.notifications.splice(index, 1);
            }
        }, 300);
    }

    success(message, duration) {
        return this.show(message, 'success', duration);
    }

    error(message, duration) {
        return this.show(message, 'error', duration);
    }

    warning(message, duration) {
        return this.show(message, 'warning', duration);
    }

    info(message, duration) {
        return this.show(message, 'info', duration);
    }

    clear() {
        this.notifications.forEach(notification => this.remove(notification));
    }
}

// Create global instance
window.notifications = new NotificationManager();

// Enhanced showSuccess and showError functions
function showSuccess(message, duration = 5000) {
    window.notifications.success(message, duration);
}

function showError(message, duration = 7000) {
    window.notifications.error(message, duration);
}

function showWarning(message, duration = 6000) {
    window.notifications.warning(message, duration);
}

function showInfo(message, duration = 5000) {
    window.notifications.info(message, duration);
}

// Loading button utility
function setButtonLoading(button, loading = true) {
    if (loading) {
        button.classList.add('btn-loading');
        button.disabled = true;
        button.dataset.originalText = button.textContent;
        button.textContent = 'جاري المعالجة...';
    } else {
        button.classList.remove('btn-loading');
        button.disabled = false;
        if (button.dataset.originalText) {
            button.textContent = button.dataset.originalText;
            delete button.dataset.originalText;
        }
    }
}

// Image preview utility
function previewImage(input, previewElement) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            previewElement.innerHTML = `
                <img src="${e.target.result}" style="max-width: 100%; max-height: 150px; border-radius: 8px; object-fit: cover;">
                <button type="button" class="btn btn-sm btn-danger" onclick="clearImagePreview('${input.id}', '${previewElement.id}')" style="margin-top: 10px;">
                    <i class="fas fa-trash"></i>
                    إزالة الصورة
                </button>
            `;
            previewElement.style.display = 'block';
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function clearImagePreview(inputId, previewId) {
    document.getElementById(inputId).value = '';
    document.getElementById(previewId).style.display = 'none';
    document.getElementById(previewId).innerHTML = '';

    // Reset file upload container
    resetFileUpload();
}

// Confirmation dialog utility
function showConfirmDialog(title, message, onConfirm, onCancel) {
    const modal = document.createElement('div');
    modal.className = 'modal-overlay';
    modal.style.display = 'flex';

    modal.innerHTML = `
        <div class="modal modal-small">
            <div class="modal-header">
                <h2 class="modal-title">${title}</h2>
                <button class="modal-close" onclick="this.closest('.modal-overlay').remove()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="modal-body">
                <div class="delete-confirmation">
                    <i class="fas fa-question-circle" style="font-size: 48px; color: #007bff; margin-bottom: 15px;"></i>
                    <p>${message}</p>
                </div>
                <div class="modal-actions">
                    <button type="button" class="btn btn-secondary cancel-btn">إلغاء</button>
                    <button type="button" class="btn btn-primary confirm-btn">تأكيد</button>
                </div>
            </div>
        </div>
    `;

    document.body.appendChild(modal);

    // Handle confirm
    modal.querySelector('.confirm-btn').addEventListener('click', () => {
        modal.remove();
        if (onConfirm) onConfirm();
    });

    // Handle cancel
    modal.querySelector('.cancel-btn').addEventListener('click', () => {
        modal.remove();
        if (onCancel) onCancel();
    });

    // Handle overlay click
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.remove();
            if (onCancel) onCancel();
        }
    });

    return modal;
}

// Form validation utility
function validateForm(formElement) {
    const errors = [];
    const inputs = formElement.querySelectorAll('input[required], select[required]');

    inputs.forEach(input => {
        if (!input.value.trim()) {
            errors.push(`حقل "${input.previousElementSibling.textContent}" مطلوب`);
            input.classList.add('error');
        } else {
            input.classList.remove('error');
        }

        // Email validation
        if (input.type === 'email' && input.value && !isValidEmail(input.value)) {
            errors.push('البريد الإلكتروني غير صحيح');
            input.classList.add('error');
        }

        // URL validation
        if (input.type === 'url' && input.value && !isValidUrl(input.value)) {
            errors.push('الرابط غير صحيح');
            input.classList.add('error');
        }
    });

    return {
        isValid: errors.length === 0,
        errors: errors
    };
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function isValidUrl(url) {
    try {
        new URL(url);
        return true;
    } catch {
        return false;
    }
}

// Add CSS for error states
const errorStyles = document.createElement('style');
errorStyles.textContent = `
    .form-group input.error,
    .form-group select.error {
        border-color: #dc3545 !important;
        box-shadow: 0 0 0 0.2rem rgba(220, 53, 69, 0.25);
    }
    
    .btn-loading {
        position: relative;
        pointer-events: none;
        opacity: 0.7;
    }
    
    .btn-loading::after {
        content: '';
        position: absolute;
        top: 50%;
        left: 50%;
        width: 16px;
        height: 16px;
        border: 2px solid transparent;
        border-top: 2px solid currentColor;
        border-radius: 50%;
        transform: translate(-50%, -50%);
        animation: btn-spin 1s linear infinite;
    }
    
    @keyframes btn-spin {
        0% { transform: translate(-50%, -50%) rotate(0deg); }
        100% { transform: translate(-50%, -50%) rotate(360deg); }
    }
    
    .notifications-container .notification {
        margin-bottom: 10px;
    }
    
    .notifications-container .notification.show {
        transform: translateX(0) !important;
    }
`;

document.head.appendChild(errorStyles);