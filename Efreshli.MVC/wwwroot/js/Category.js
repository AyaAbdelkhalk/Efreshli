// Global variables

let categories = [];
let currentLevel = 0;
let navigationPath = [];
let editingCategoryId = null;

// Initialize page
document.addEventListener('DOMContentLoaded', function () {
    loadCategories();
});

// Load all categories from API
async function loadCategories() {
    showLoading();

    try {
        const response = await fetch('/Category/GetAllCategories');
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
//function displayCategories() {
//    const grid = document.getElementById('categoriesGrid');
//    const currentCategories = getCurrentLevelCategories();

//    if (currentCategories.length === 0) {
//        showEmptyState();
//        return;
//    }

//    hideEmptyState();

//    console.log('Displaying categories:', currentCategories);
//    grid.innerHTML = currentCategories.map(category => `
//        <div class="category-card" data-id="${category.categoryId}" onclick="navigateToSubcategories(${category.categoryId})">
//            <div class="category-image">
//                ${category.imageUrl ?
//        `<img src="${category.imageUrl}" class="category-image" alt="${category.nameAr}" onerror="this.src='/images/default-category.png'">` :
//            `<div class="no-image"><i class="fas fa-image"></i></div>`
//        }
//            </div>
//            <div class="category-content">
//                <h3 class="category-title">
//                    <span class="ar-title" dir="rtl">${category.nameAr}</span>
//                    <span class="en-title">${category.nameEn}</span>
//                </h3>
//                <div class="category-actions">
//                    ${hasSubcategories(category.categoryId) ?
//            `<button class="btn btn-sm btn-outline" onclick="navigateToSubcategories(${category.categoryId})">
//                            <i class="fas fa-folder-open"></i>
//                        </button>` : ''
//        }
//                    <button class="btn btn-sm btn-outline" onclick="editCategory(${category.categoryId})">
//                        <i class="fas fa-edit"></i>

//                    </button>
//                    <button class="btn btn-sm btn-danger" onclick="deleteCategory(${category.categoryId})">
//                        <i class="fas fa-trash"></i>

//                    </button>
//                </div>
//            </div>
//        </div>
//    `).join('');
//}

function displayCategories() {
    const container = document.getElementById('categoriesGrid');
    container.innerHTML = ''; // Clear previous content
    const currentCategories = getCurrentLevelCategories();
    if (currentCategories.length === 0) {
        showEmptyState();
        return;
    }

    hideEmptyState();

    currentCategories.forEach(category => {
        const categoryElement = document.createElement('div');
        categoryElement.classList.add('category-item');
        //categoryElement.setAttribute('onclick', `navigateToSubcategories(${category.categoryId})`);
        const numOfChildren = categories.filter(cat => cat.parentId === category.categoryId).length;
        const productCount = category.products ? category.products.length : 0;
        categoryElement.innerHTML = `
                <div class="category-card" data-id="${category.categoryId}">
                    <div class="category-image">
                        ${category.imageUrl ?
                `<img src="${category.imageUrl}" class="category-image" alt="${category.nameAr}" onerror="this.src='/images/default-category.png'">` :
                    `<div class="no-image"><i class="fas fa-image"></i></div>`
                }
                    </div>
                    <div class="category-content">
                           <h3>${window.translations.categoryName} : ${category.nameEn}</h3>
                            <h4>${window.translations.categoryName} : ${category.nameAr}</h4>
                            <div class="category-meta">
                    <div class="badge-container">
                        <span class="subcategory-badge">
                            <i class="fas fa-layer-group"></i>
                            ${numOfChildren} ${window.translations.subCategories}
                        </span>
                        <span class="product-badge">
                            <i class="fas fa-cube"></i>
                            ${productCount} ${window.translations.products}
                        </span>
                    </div>
                </div>

                <div class="category-actions">
                ${hasSubcategories(category.categoryId) ?
                `<button class="action-btn open-sub"  onclick="navigateToSubcategories(${category.categoryId})">
                                    <i class="fas fa-folder-open"></i>
                    </button>` : ''
                }
                <button class="action-btn add-sub" onclick="navigateToSubcategories(${category.categoryId})" title="Add Subcategory">
                    <i class="fas fa-plus"></i>
                </button>
                 
                <button class="action-btn edit" onclick="editCategory(${category.categoryId})" title="Edit Category">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="action-btn delete" onclick="deleteCategory(${category.categoryId})" title="Delete Category">
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
            الفئات الرئيسية
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

    const grid = document.getElementById('categoriesGrid');

    if (filteredCategories.length === 0) {
        showEmptyState();
        return;
    }

    hideEmptyState();

    grid.innerHTML = filteredCategories.map(category => `
        <div class="category-card" data-id="${category.categoryId}">
            <div class="category-image">
                ${category.imageUrl ?
            `<img class="category-image" src="${category.imageUrl}" alt="${category.nameAr}" onerror="this.src='/images/default-category.png'">` :
            `<div class="no-image"><i class="fas fa-image"></i></div>`
        }
            </div>
            <div class="category-content">
                <h3 class="category-title">
                    <span class="ar-title" dir="rtl">${category.nameAr}</span>
                    <span class="en-title">${category.nameEn}</span>
                </h3>
                <div class="category-actions">
                    <button class="btn btn-sm btn-outline" onclick="editCategory(${category.categoryId})">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="deleteCategory(${category.categoryId})">
                        <i class="fas fa-trash"></i>
                        
                    </button>
                </div>
            </div>
        </div>
    `).join('');
}

// Open add/edit modal
function openAddModal() {
    editingCategoryId = null;
    document.getElementById('modalTitle').textContent = 'إضافة فئة جديدة';
    document.getElementById('categoryForm').reset();
    populateParentOptions();
    const modal = document.getElementById('categoryModal');
    modal.style.display = 'flex';

}

function editCategory(categoryId) {
    const category = categories.find(cat => cat.categoryId === categoryId);
    if (!category) return;

    editingCategoryId = categoryId;
    document.getElementById('modalTitle').textContent = 'تعديل الفئة';
    document.getElementById('nameAr').value = category.nameAr;
    document.getElementById('nameEn').value = category.nameEn;
    document.getElementById('imageUrl').value = category.image?.url || '';
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
        formData.append('Image', imageFile);
    }

    // Remove imageUrl if we have a file
    if (imageFile) {
        formData.delete('imageUrl');
    }

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

        const result = await response.json();

        if (result.success) {
            showSuccess(result.message);
            closeModal();
            await loadCategories(); // Reload data
        } else {
            showError(result.message);
        }

    } catch (error) {
        console.error('Error saving category:', error);
        showError('حدث خطأ في حفظ البيانات');
    }
}

// Delete category
function deleteCategory(categoryId) {
    const category = categories.find(cat => cat.categoryId === categoryId);
    if (!category) return;

    // Set the category to delete
    window.categoryToDelete = categoryId;
    document.getElementById('deleteModal').style.display = 'flex';
    modal.classList.add('active');
}

async function confirmDelete() {
    if (!window.categoryToDelete) return;

    try {
        const response = await fetch(`/Category/DeleteCategory/${window.categoryToDelete}`, {
            method: 'DELETE'
        });

        const result = await response.json();

        if (result.success) {
            showSuccess(result.message);
            closeDeleteModal();
            await loadCategories(); // Reload data
        } else {
            showError(result.message);
        }

    } catch (error) {
        console.error('Error deleting category:', error);
        showError('حدث خطأ في حذف الفئة');
    }
}

// Modal controls
function closeModal() {
    document.getElementById('categoryModal').style.display = 'none';
}

function closeDeleteModal() {
    document.getElementById('deleteModal').style.display = 'none';
    window.categoryToDelete = null;
}

// UI state management
function showLoading() {
    document.getElementById('loadingSpinner').style.display = 'block';
    document.getElementById('categoriesGrid').style.display = 'none';
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

// Notification functions
function showSuccess(message) {
    // يمكنك استخدام أي مكتبة notifications مثل toastr
    alert('نجح: ' + message);
}

function showError(message) {
    alert('خطأ: ' + message);
}


function scrollModalIntoView(modal) {
    const modalContent = modal.querySelector('.modal');
    if (modalContent) {
        modalContent.scrollIntoView({
            behavior: 'smooth',
            block: 'center', // Center the modal vertically in the viewport
            inline: 'center' // Center the modal horizontally in the viewport
        });
    }
}
