/**
 * Toast Notification System
 * Replaces blocking alerts with non-blocking toast messages
 * Features: Success, info, warning, error types with queueing
 */

class ToastManager {
    constructor() {
        this.queue = [];
        this.isProcessing = false;
        this.maxToasts = 5;
        this.toastDuration = 5000; // 5 seconds
        this.container = this.createContainer();
        
        // Auto-start processing
        this.processQueue();
    }

    createContainer() {
        let container = document.getElementById('toast-container');
        
        if (!container) {
            container = document.createElement('div');
            container.id = 'toast-container';
            container.className = 'toast-container';
            container.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                max-width: 400px;
                pointer-events: none;
            `;
            document.body.appendChild(container);
        }
        
        return container;
    }

    show(type, message, options = {}) {
        const toast = {
            id: this.generateId(),
            type: type,
            message: message,
            duration: options.duration || this.toastDuration,
            persistent: options.persistent || false,
            action: options.action,
            timestamp: Date.now()
        };

        this.queue.push(toast);
        this.processQueue();
        
        return toast.id;
    }

    success(message, options = {}) {
        return this.show('success', message, options);
    }

    info(message, options = {}) {
        return this.show('info', message, options);
    }

    warning(message, options = {}) {
        return this.show('warning', message, options);
    }

    error(message, options = {}) {
        return this.show('error', message, options);
    }

    async processQueue() {
        if (this.isProcessing || this.queue.length === 0) {
            return;
        }

        this.isProcessing = true;

        while (this.queue.length > 0) {
            const activeToasts = this.container.querySelectorAll('.toast');
            
            if (activeToasts.length >= this.maxToasts) {
                // Remove oldest toast if we're at max capacity
                const oldestToast = activeToasts[0];
                this.removeToast(oldestToast);
            }

            const toast = this.queue.shift();
            await this.displayToast(toast);
            
            // Small delay between toasts
            await this.delay(100);
        }

        this.isProcessing = false;
    }

    async displayToast(toast) {
        const toastElement = this.createToastElement(toast);
        this.container.appendChild(toastElement);

        // Trigger animation
        await this.delay(10);
        toastElement.classList.add('show');

        // Auto-remove if not persistent
        if (!toast.persistent) {
            setTimeout(() => {
                this.removeToast(toastElement);
            }, toast.duration);
        }
    }

    createToastElement(toast) {
        const element = document.createElement('div');
        element.className = `toast toast-${toast.type}`;
        element.dataset.toastId = toast.id;
        element.style.cssText = `
            background: ${this.getToastColor(toast.type)};
            color: white;
            padding: 12px 16px;
            margin-bottom: 8px;
            border-radius: 6px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            transform: translateX(100%);
            transition: transform 0.3s ease;
            pointer-events: auto;
            display: flex;
            align-items: center;
            justify-content: space-between;
            min-width: 300px;
            max-width: 400px;
        `;

        const icon = this.getToastIcon(toast.type);
        const content = `
            <div style="display: flex; align-items: center; flex: 1;">
                <i class="${icon}" style="margin-right: 8px; font-size: 16px;"></i>
                <span style="flex: 1;">${this.escapeHtml(toast.message)}</span>
            </div>
            <button class="toast-close" style="
                background: none;
                border: none;
                color: white;
                cursor: pointer;
                margin-left: 8px;
                padding: 0;
                font-size: 18px;
                opacity: 0.7;
            ">&times;</button>
        `;

        element.innerHTML = content;

        // Add close button functionality
        const closeBtn = element.querySelector('.toast-close');
        closeBtn.addEventListener('click', () => {
            this.removeToast(element);
        });

        // Add action button if provided
        if (toast.action) {
            const actionBtn = document.createElement('button');
            actionBtn.textContent = toast.action.text;
            actionBtn.style.cssText = `
                background: rgba(255,255,255,0.2);
                border: 1px solid rgba(255,255,255,0.3);
                color: white;
                padding: 4px 8px;
                border-radius: 4px;
                cursor: pointer;
                margin-left: 8px;
                font-size: 12px;
            `;
            actionBtn.addEventListener('click', toast.action.callback);
            element.querySelector('div').appendChild(actionBtn);
        }

        return element;
    }

    removeToast(toastElement) {
        if (!toastElement) return;

        toastElement.classList.remove('show');
        toastElement.style.transform = 'translateX(100%)';
        
        setTimeout(() => {
            if (toastElement.parentNode) {
                toastElement.parentNode.removeChild(toastElement);
            }
        }, 300);
    }

    getToastColor(type) {
        const colors = {
            success: '#28a745',
            info: '#17a2b8',
            warning: '#ffc107',
            error: '#dc3545'
        };
        return colors[type] || colors.info;
    }

    getToastIcon(type) {
        const icons = {
            success: 'fas fa-check-circle',
            info: 'fas fa-info-circle',
            warning: 'fas fa-exclamation-triangle',
            error: 'fas fa-times-circle'
        };
        return icons[type] || icons.info;
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    generateId() {
        return 'toast_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    clear() {
        const toasts = this.container.querySelectorAll('.toast');
        toasts.forEach(toast => this.removeToast(toast));
        this.queue = [];
    }

    remove(id) {
        const toast = this.container.querySelector(`[data-toast-id="${id}"]`);
        if (toast) {
            this.removeToast(toast);
        }
        
        // Remove from queue if not yet displayed
        this.queue = this.queue.filter(t => t.id !== id);
    }
}

// Create global instance
const toastManager = new ToastManager();

// Global functions for easy access
window.showToast = (type, message, options) => toastManager.show(type, message, options);
window.showSuccess = (message, options) => toastManager.success(message, options);
window.showInfo = (message, options) => toastManager.info(message, options);
window.showWarning = (message, options) => toastManager.warning(message, options);
window.showError = (message, options) => toastManager.error(message, options);
window.clearToasts = () => toastManager.clear();

// Add CSS for toast animations
const style = document.createElement('style');
style.textContent = `
    .toast-container .toast {
        transform: translateX(100%);
        transition: transform 0.3s ease;
    }
    
    .toast-container .toast.show {
        transform: translateX(0);
    }
    
    .toast-container .toast:hover {
        transform: translateX(0) scale(1.02);
    }
    
    .toast-close:hover {
        opacity: 1 !important;
    }
`;
document.head.appendChild(style);

// Export for use as ES module
export default toastManager;
