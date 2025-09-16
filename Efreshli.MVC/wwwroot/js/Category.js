// Global variables
let categories = [];
let currentLevel = 0;
let navigationPath = [];
let editingCategoryId = null;
let categoryToDelete = null;

// Initialize page
document.addEventListener('DOMContentLoaded', function () {
    loadCategories();
    setupEventListeners();
});

// Setup event listeners
function setupEventListeners() {
    // Close modals when clicking outside
    document.addEventListener('click', function (e) {
        const categoryModal = document.getElementById('categoryModal');
        const deleteModal = document.getElementById('deleteModal');

        if (e.target === categoryModal) {
            closeModal();
        }
        if (e.target === deleteModal) {
            closeDeleteModal();
        }
    });

    // Close modals with Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            const categoryModal = document.getElementById('categoryModal');
            const deleteModal = document.getElementById('deleteModal');

            if (categoryModal && categoryModal.style.display === 'flex') {
                closeModal();
            }
            if (deleteModal && deleteModal.style.display === 'flex') {
                closeDeleteModal();
            }
        }
    });
}

// Load all categories from API
async function loadCategories() {
    showLoading();

    try {
        const response = await fetch('/Category/GetAllCategories');

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            categories = result.data || [];
            displayCategories();
            populateParentOptions();
        } else {
            showError(result.message || 'خطأ في تحميل البيانات');
            showEmptyState();
        }
    } catch (error) {
        console.error('Error loading categories:', error);
        showError('حدث خطأ في الاتصال بالخادم');
        showEmptyState();
    }

    hideLoading();
}

// Display categories in grid
function displayCategories() {
    const container = document.getElementById('categoriesGrid');
    container.innerHTML = '';
    const currentCategories = getCurrentLevelCategories();

    if (currentCategories.length === 0) {
        showEmptyState();
        return;
    }

    hideEmptyState();

    currentCategories.forEach(category => {
        const categoryElement = document.createElement('div');
        categoryElement.classList.add('category-item');
        const numOfChildren = categories.filter(cat => cat.parentId === category.categoryId).length;
        const productCount = category.productCount || 0;

        categoryElement.innerHTML = `
            <div class="category-card" data-id="${category.categoryId}">
                <div class="category-image">
                    ${category.imageUrl ?
                `<img src="${category.imageUrl}" class="category-image" alt="${category.nameAr}" onerror="this.src='/images/default-category.png'">` :
                `<div class="no-image"><i class="fas fa-image"></i></div>`
            }
                </div>
                <div class="category-content">
                    <h3>${window.translations?.categoryNameEn || 'English Name'} : ${category.nameEn}</h3>
                    <h4>${window.translations?.categoryNameAr || 'الاسم العربي'} : ${category.nameAr}</h4>
                    <div class="category-meta">
                        <div class="badge-container">
                            <span class="subcategory-badge">
                                <i class="fas fa-layer-group"></i>
                                ${numOfChildren} ${window.translations?.subCategories || 'فئات فرعية'}
                            </span>
                            <span class="product-badge">
                                <i class="fas fa-cube"></i>
                                ${productCount} ${window.translations?.products || 'منتجات'}
                            </span>
                        </div>
                    </div>

                    <div class="category-actions">
                        ${hasSubcategories(category.categoryId) ?
                `<button class="action-btn open-sub" onclick="navigateToSubcategories(${category.categoryId})" title="عرض الفئات الفرعية">
                                <i class="fas fa-folder-open"></i>
                            </button>` : ''
            }
                        <button class="action-btn add-sub" onclick="addSubcategory(${category.categoryId})" title="إضافة فئة فرعية">
                            <i class="fas fa-plus"></i>
                        </button>
                        <button class="action-btn edit" onclick="editCategory(${category.categoryId})" title="تعديل الفئة">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="action-btn delete" onclick="openDeleteModal(${category.categoryId}, '${category.nameAr.replace(/'/g, "\\\'")}')" title="حذف الفئة">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        container.appendChild(categoryElement);
    });
}

// Get categories for current level
function getCurrentLevelCategories() {
    if (currentLevel === 0) {
        return categories.filter(cat => !cat.parentId);
    }

    const currentParentId = navigationPath[currentLevel - 1];
    return categories.filter(cat => cat.parentId === currentParentId);
}

// Check if category has subcategories
function hasSubcategories(categoryId) {
    return categories.some(cat => cat.parentId === categoryId);
}

// Navigate to subcategories
function navigateToSubcategories(categoryId) {
    const category = categories.find(cat => cat.categoryId === categoryId);
    if (!category) return;

    navigationPath[currentLevel] = categoryId;
    currentLevel++;

    updateBreadcrumb();
    displayCategories();
}

// Add subcategory (opens modal with parent pre-selected)
function addSubcategory(parentId) {
    editingCategoryId = null;
    document.getElementById('modalTitle').textContent = 'إضافة فئة فرعية';
    document.getElementById('categoryForm').reset();
    populateParentOptions();
    document.getElementById('parentId').value = parentId;
    document.getElementById('categoryModal').style.display = 'flex';
}

// Navigate to specific level
function navigateToLevel(level) {
    currentLevel = level;
    navigationPath = navigationPath.slice(0, level);

    updateBreadcrumb();
    displayCategories();
}

// Update breadcrumb navigation
function updateBreadcrumb() {
    const breadcrumb = document.getElementById('breadcrumb');
    let html = `
        <li class="breadcrumb-item ${currentLevel === 0 ? 'active' : ''}" onclick="navigateToLevel(0)">
            <i class="fas fa-home"></i>
            ${window.translations?.mainCategories || 'الفئات الرئيسية'}
        </li>
    `;

    for (let i = 0; i < currentLevel; i++) {
        const categoryId = navigationPath[i];
        const category = categories.find(cat => cat.categoryId === categoryId);
        if (category) {
            const isActive = i === currentLevel - 1;
            html += `
                <li class="breadcrumb-item ${isActive ? 'active' : ''}" onclick="navigateToLevel(${i + 1})">
                    ${category.nameAr}
                </li>
            `;
        }
    }

    breadcrumb.innerHTML = html;
}

// Search categories
function searchCategories() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    if (!searchTerm) {
        displayCategories();
        return;
    }

    const filteredCategories = categories.filter(cat =>
        cat.nameAr.toLowerCase().includes(searchTerm) ||
        cat.nameEn.toLowerCase().includes(searchTerm)
    );

    const container = document.getElementById('categoriesGrid');

    if (filteredCategories.length === 0) {
        showEmptyState();
        return;
    }

    hideEmptyState();
    container.innerHTML = '';

    filteredCategories.forEach(category => {
        const categoryElement = document.createElement('div');
        categoryElement.classList.add('category-item');

        categoryElement.innerHTML = `
            <div class="category-card" data-id="${category.categoryId}">
                <div class="category-image">
                    ${category.imageUrl ?
                `<img class="category-image" src="${category.imageUrl}" alt="${category.nameAr}" onerror="this.src='/images/default-category.png'">` :
                `<div class="no-image"><i class="fas fa-image"></i></div>`
            }
                </div>
                <div class="category-content">
                    <h3>${window.translations?.categoryNameEn || 'English Name'} : ${category.nameEn}</h3>
                    <h4>${window.translations?.categoryNameAr || 'الاسم العربي'} : ${category.nameAr}</h4>
                    <div class="category-actions">
                        <button class="action-btn edit" onclick="editCategory(${category.categoryId})" title="تعديل الفئة">
                            <i class="fas fa-edit"></i>
                        </button>
                        <button class="action-btn delete" onclick="openDeleteModal(${category.categoryId}, '${category.nameAr.replace(/'/g, "\\\'")}')" title="حذف الفئة">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;

        container.appendChild(categoryElement);
    });
}

// Open add modal
function openAddModal() {
    editingCategoryId = null;
    document.getElementById('modalTitle').textContent = 'إضافة فئة جديدة';
    document.getElementById('categoryForm').reset();
    populateParentOptions();

    // Set current level as parent if we're in a subcategory
    if (currentLevel > 0) {
        const currentParentId = navigationPath[currentLevel - 1];
        document.getElementById('parentId').value = currentParentId;
    }

    document.getElementById('categoryModal').style.display = 'flex';
}

// Edit category
function editCategory(categoryId) {
    const category = categories.find(cat => cat.categoryId === categoryId);
    if (!category) return;

    editingCategoryId = categoryId;
    document.getElementById('modalTitle').textContent = 'تعديل الفئة';
    document.getElementById('nameAr').value = category.nameAr;
    document.getElementById('nameEn').value = category.nameEn;
    document.getElementById('parentId').value = category.parentId || '';

    populateParentOptions();
    document.getElementById('categoryModal').style.display = 'flex';
}

// Populate parent category options
function populateParentOptions() {
    const select = document.getElementById('parentId');
    const currentParentId = navigationPath[currentLevel - 1] || null;

    let html = '<option value="">لا يوجد (فئة رئيسية)</option>';

    categories.filter(cat => cat.categoryId !== editingCategoryId).forEach(category => {
        const selected = category.categoryId === currentParentId ? 'selected' : '';
        html += `<option value="${category.categoryId}" ${selected}>${category.nameAr}</option>`;
    });

    select.innerHTML = html;
}

// Handle form submission
async function handleFormSubmit(event) {
    event.preventDefault();

    const formData = new FormData(event.target);
    const imageFile = document.getElementById('imageFile')?.files[0];

    if (imageFile) {
        formData.append('ImageFile', imageFile);
    }

    // Show loading state
    const submitButton = event.target.querySelector('button[type="submit"]');
    const originalText = submitButton.innerHTML;
    submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> جاري الحفظ...';
    submitButton.disabled = true;

    try {
        let response;

        if (editingCategoryId) {
            // Update existing category
            response = await fetch(`/Category/UpdateCategory/${editingCategoryId}`, {
                method: 'PUT',
                body: formData
            });
        } else {
            // Add new category
            response = await fetch('/Category/AddCategory', {
                method: 'POST',
                body: formData
            });
        }

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            showSuccess(result.message);
            closeModal();
            await loadCategories();
        } else {
            showError(result.message);
        }

    } catch (error) {
        console.error('Error saving category:', error);
        showError('حدث خطأ في حفظ البيانات');
    } finally {
        // Reset button state
        submitButton.innerHTML = originalText;
        submitButton.disabled = false;
    }
}

// Open delete modal
function openDeleteModal(categoryId, categoryName) {
    categoryToDelete = categoryId;

    // Update the modal text to show the category name
    const modalText = document.querySelector('#deleteModal .delete-confirmation p');
    if (modalText && categoryName) {
        modalText.textContent = `هل أنت متأكد من حذف فئة "${categoryName}"؟`;
    }

    document.getElementById('deleteModal').style.display = 'flex';
}

// Confirm delete
async function confirmDelete() {
    if (!categoryToDelete) return;

    // Show loading state
    const deleteButton = document.querySelector('#deleteModal .btn-danger');
    const originalText = deleteButton.innerHTML;
    deleteButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> جاري الحذف...';
    deleteButton.disabled = true;

    try {
        const response = await fetch(`/Category/DeleteCategory/${categoryToDelete}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            showSuccess(result.message);
            closeDeleteModal();
            await loadCategories();
        } else {
            showError(result.message);
        }

    } catch (error) {
        console.error('Error deleting category:', error);
        showError('حدث خطأ في حذف الفئة');
    } finally {
        // Reset button state
        deleteButton.innerHTML = originalText;
        deleteButton.disabled = false;
    }
}

// Modal controls
function closeModal() {
    document.getElementById('categoryModal').style.display = 'none';
    editingCategoryId = null;
}

function closeDeleteModal() {
    document.getElementById('deleteModal').style.display = 'none';
    categoryToDelete = null;
}

// UI state management
function showLoading() {
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('categoriesGrid').style.display = 'none';
    hideEmptyState();
}

function hideLoading() {
    document.getElementById('loadingSpinner').style.display = 'none';
    document.getElementById('categoriesGrid').style.display = 'grid';
}

function showEmptyState() {
    document.getElementById('emptyState').style.display = 'block';
    document.getElementById('categoriesGrid').style.display = 'none';
}

function hideEmptyState() {
    document.getElementById('emptyState').style.display = 'none';
    document.getElementById('categoriesGrid').style.display = 'grid';
}

// Enhanced notification functions with better UX
function showSuccess(message) {
    showNotification(message, 'success');
}

function showError(message) {
    showNotification(message, 'error');
}

function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.notification');
    existingNotifications.forEach(n => n.remove());

    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <div class="notification-content">
            <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'error' ? 'fa-exclamation-circle' : 'fa-info-circle'}"></i>
            <span>${message}</span>
            <button class="notification-close" onclick="this.parentElement.parentElement.remove()">
                <i class="fas fa-times"></i>
            </button>
        </div>
    `;

    // Add styles if not already present
    if (!document.querySelector('#notification-styles')) {
        const style = document.createElement('style');
        style.id = 'notification-styles';
        style.textContent = `
            .notification {
                position: fixed;
                top: 20px;
                right: 20px;
                min-width: 300px;
                max-width: 500px;
                background: white;
                border-radius: 8px;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                transform: translateX(120%);
                transition: transform 0.3s ease-in-out;
                z-index: 9999;
                border-left: 4px solid #007bff;
            }
            .notification.show { transform: translateX(0); }
            .notification-success { border-left-color: #28a745; }
            .notification-error { border-left-color: #dc3545; }
            .notification-content {
                display: flex;
                align-items: center;
                padding: 15px 20px;
                gap: 10px;
            }
            .notification-content i:first-child {
                font-size: 18px;
                color: inherit;
            }
            .notification-success .notification-content i:first-child { color: #28a745; }
            .notification-error .notification-content i:first-child { color: #dc3545; }
            .notification-content span {
                flex-grow: 1;
                font-weight: 500;
                color: #333;
            }
            .notification-close {
                background: none;
                border: none;
                cursor: pointer;
                padding: 0;
                color: #999;
            }
            .notification-close:hover { color: #333; }
        `;
        document.head.appendChild(style);
    }

    // Add to page
    document.body.appendChild(notification);

    // Show notification
    setTimeout(() => {
        notification.classList.add('show');
    }, 100);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.classList.remove('show');
            setTimeout(() => {
                if (notification.parentNode) {
                    document.body.removeChild(notification);
                }
            }, 300);
        }
    }, 5000);
}

// Make functions globally available
window.openAddModal = openAddModal;
window.editCategory = editCategory;
window.openDeleteModal = openDeleteModal;
window.confirmDelete = confirmDelete;
window.closeModal = closeModal;
window.closeDeleteModal = closeDeleteModal;
window.handleFormSubmit = handleFormSubmit;
window.searchCategories = searchCategories;
window.navigateToLevel = navigateToLevel;
window.navigateToSubcategories = navigateToSubcategories;
window.addSubcategory = addSubcategory;