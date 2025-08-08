// Category Management System - JavaScript
/*
 * MVC Integration Guide:
 * 
 * 1. Controller Actions Required:
 *    - GET  /Category/Index              -> عرض الصفحة الرئيسية
 *    - GET  /Category/GetCategories      -> جلب الفئات (Ajax)
 *    - POST /Category/Create             -> إنشاء فئة جديدة
 *    - POST /Category/Edit/{id}          -> تعديل فئة
 *    - POST /Category/Delete/{id}        -> حذف فئة
 *    - GET  /Category/GetSubcategories/{parentId} -> جلب الفئات الفرعية
 * 
 * 2. Required DTOs:
 *    - CategoryDto: {CategoryId, NameAr, NameEn, ParentId, ImageUrl, ProductCount, Children}
 *    - CreateCategoryDto: {NameAr, NameEn, ParentId?, ImageUrl?}
 *    - UpdateCategoryDto: {CategoryId, NameAr, NameEn, ParentId?, ImageUrl?}
 * 
 * 3. Model Structure:
 *    public class Category {
 *        public int CategoryId { get; set; }
 *        public string NameAr { get; set; }
 *        public string NameEn { get; set; }
 *        public int? ParentId { get; set; }
 *        public string ImageUrl { get; set; }
 *        public int ProductCount { get; set; }
 *        public Category Parent { get; set; }
 *        public ICollection<Category> Children { get; set; }
 *        public string CreatedBy { get; set; }
 *        public DateTime CreatedAt { get; set; }
 *    }
 * 
 * 4. Integration Points:
 *    - Replace initializeDummyData() with AJAX call to controller
 *    - Replace add/edit/delete functions with AJAX calls
 *    - Add CSRF token to all POST requests
 */

class CategoryManager {
    constructor() {
        this.categories = this.initializeDummyData();
        this.currentLevel = [];
        this.currentParentId = null;
        this.editingCategoryId = null;
        this.categoryToDelete = null;
        this.searchTerm = '';

        this.init();
    }

    // Initialize the application
    init() {
        this.loadCategories();
        this.updateBreadcrumb();
        this.populateParentSelect();

        // For MVC: Replace with AJAX call
        // this.loadCategoriesFromServer();
    }

    // MVC Integration: Load categories from server
    // async loadCategoriesFromServer() {
    //     try {
    //         const response = await fetch('/Category/GetCategories', {
    //             method: 'GET',
    //             headers: {
    //                 'Content-Type': 'application/json'
    //             }
    //         });
    //         
    //         if (response.ok) {
    //             this.categories = await response.json();
    //             this.loadCategories();
    //             this.updateBreadcrumb();
    //             this.populateParentSelect();
    //         } else {
    //             console.error('Failed to load categories');
    //         }
    //     } catch (error) {
    //         console.error('Error loading categories:', error);
    //     }
    // }

    // Dummy data following GetCategoryDto structure (for MVC binding)
    initializeDummyData() {
        return [
            {
                CategoryId: 1,
                NameAr: "خضروات",
                NameEn: "Vegetables",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1540420773420-3366772f4999?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 25, // عدد المنتجات في هذه الفئة
                Children: [
                    {
                        CategoryId: 11,
                        NameAr: "خضروات ورقية",
                        NameEn: "Leafy Greens",
                        ParentId: 1,
                        ImageUrl: "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=200",
                        Parent: null,
                        CreatedBy: "Admin",
                        ProductCount: 12, // عدد المنتجات في هذه الفئة الفرعية
                        Children: [
                            {
                                CategoryId: 111,
                                NameAr: "سبانخ",
                                NameEn: "Spinach",
                                ParentId: 11,
                                ImageUrl: "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=200",
                                Parent: null,
                                CreatedBy: "Admin",
                                ProductCount: 5,
                                Children: []
                            },
                            {
                                CategoryId: 112,
                                NameAr: "جرجير",
                                NameEn: "Arugula",
                                ParentId: 11,
                                ImageUrl: "https://images.unsplash.com/photo-1622205313162-be1d5712a43f?w=200",
                                Parent: null,
                                CreatedBy: "Admin",
                                ProductCount: 7,
                                Children: []
                            }
                        ]
                    },
                    {
                        CategoryId: 12,
                        NameAr: "خضروات جذرية",
                        NameEn: "Root Vegetables",
                        ParentId: 1,
                        ImageUrl: "https://images.unsplash.com/photo-1518977676601-b53f82aba655?w=200",
                        Parent: null,
                        CreatedBy: "Admin",
                        ProductCount: 8,
                        Children: []
                    }
                ]
            },
            {
                CategoryId: 2,
                NameAr: "فواكه",
                NameEn: "Fruits",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1619566636858-adf3ef46400b?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 18,
                Children: [
                    {
                        CategoryId: 21,
                        NameAr: "فواكه استوائية",
                        NameEn: "Tropical Fruits",
                        ParentId: 2,
                        ImageUrl: "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=200",
                        Parent: null,
                        CreatedBy: "Admin",
                        ProductCount: 10,
                        Children: []
                    },
                    {
                        CategoryId: 22,
                        NameAr: "حمضيات",
                        NameEn: "Citrus Fruits",
                        ParentId: 2,
                        ImageUrl: "https://images.unsplash.com/photo-1547514701-42782101795e?w=200",
                        Parent: null,
                        CreatedBy: "Admin",
                        ProductCount: 8,
                        Children: []
                    }
                ]
            },
            {
                CategoryId: 3,
                NameAr: "منتجات الألبان",
                NameEn: "Dairy Products",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 15,
                Children: [
                    {
                        CategoryId: 31,
                        NameAr: "جبن",
                        NameEn: "Cheese",
                        ParentId: 3,
                        ImageUrl: "https://images.unsplash.com/photo-1486297678162-eb2a19b0a32d?w=200",
                        Parent: null,
                        CreatedBy: "Admin",
                        ProductCount: 12,
                        Children: []
                    }
                ]
            },
            {
                CategoryId: 4,
                NameAr: "لحوم",
                NameEn: "Meat",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 22,
                Children: []
            },
            {
                CategoryId: 5,
                NameAr: "لحوم",
                NameEn: "Meat",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 22,
                Children: []
            }, {
                CategoryId: 6,
                NameAr: "لحوم",
                NameEn: "Meat",
                ParentId: null,
                ImageUrl: "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=200",
                Parent: null,
                CreatedBy: "Admin",
                ProductCount: 22,
                Children: []
            }
        ];
    }

    // Get categories by parent ID
    getCategoriesByParent(parentId = null) {
        const allCategories = this.flattenCategories(this.categories);
        return allCategories.filter(cat => cat.ParentId === parentId);
    }

    // Flatten nested categories structure
    flattenCategories(categories) {
        let flattened = [];
        categories.forEach(category => {
            flattened.push(category);
            if (category.Children && category.Children.length > 0) {
                flattened = flattened.concat(this.flattenCategories(category.Children));
            }
        });
        return flattened;
    }

    // Load and display categories
    loadCategories() {
        const grid = document.getElementById('categoriesGrid');
        const loadingSpinner = document.getElementById('loadingSpinner');
        const emptyState = document.getElementById('emptyState');

        // Show loading
        loadingSpinner.style.display = 'flex';
        grid.innerHTML = '';
        emptyState.style.display = 'none';

        // Simulate loading delay
        setTimeout(() => {
            const categories = this.getCategoriesByParent(this.currentParentId);
            const filteredCategories = this.filterCategories(categories);

            loadingSpinner.style.display = 'none';

            if (filteredCategories.length === 0) {
                emptyState.style.display = 'flex';
            } else {
                this.renderCategories(filteredCategories);
            }
        }, 500);
    }

    // Filter categories based on search term
    filterCategories(categories) {
        if (!this.searchTerm) return categories;

        return categories.filter(category =>
            category.NameAr.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            category.NameEn.toLowerCase().includes(this.searchTerm.toLowerCase())
        );
    }

    // Render categories in the grid
    renderCategories(categories) {
        const grid = document.getElementById('categoriesGrid');
        grid.innerHTML = '';

        categories.forEach((category, index) => {
            const categoryCard = this.createCategoryCard(category, index);
            grid.appendChild(categoryCard);
        });
    }

    // Create individual category card
    createCategoryCard(category, index) {
        const card = document.createElement('div');
        card.className = 'category-card';
        card.style.animationDelay = `${index * 0.1}s`;

        const subcategoryCount = category.Children ? category.Children.length : 0;
        const productCount = category.ProductCount || 0; // عدد المنتجات
        const hasImage = category.ImageUrl && category.ImageUrl !== '';

        card.innerHTML = `
            <div class="category-image">
                ${hasImage ?
                `<img src="${category.ImageUrl}" alt="${category.NameEn}" style="width: 100%; height: 100%; border-radius: 12px; object-fit: cover;">` :
                '<i class="fas fa-folder"></i>'
            }
            </div>
            <div class="category-content">
                <h3>${category.NameEn}</h3>
                <p>${category.NameAr}</p>
                <div class="category-meta">
                    <div class="badge-container">
                        <span class="subcategory-badge">
                            <i class="fas fa-layer-group"></i>
                            ${subcategoryCount} subcategories
                        </span>
                        <span class="product-badge">
                            <i class="fas fa-cube"></i>
                            ${productCount} products
                        </span>
                    </div>
                </div>
            </div>
            <div class="category-actions">
                <button class="action-btn add-sub" onclick="categoryManager.addSubcategory(${category.CategoryId})" title="Add Subcategory">
                    <i class="fas fa-plus"></i>
                </button>
                <button class="action-btn edit" onclick="categoryManager.editCategory(${category.CategoryId})" title="Edit Category">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="action-btn delete" onclick="categoryManager.deleteCategory(${category.CategoryId})" title="Delete Category">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        `;

        // Add click handler for navigation (except on action buttons)
        card.addEventListener('click', (e) => {
            if (!e.target.closest('.category-actions')) {
                this.navigateToCategory(category);
            }
        });

        return card;
    }

    // Navigate to category (show its subcategories)
    navigateToCategory(category) {
        if (category.Children && category.Children.length > 0) {
            this.currentLevel.push({
                id: category.CategoryId,
                nameEn: category.NameEn,
                nameAr: category.NameAr
            });
            this.currentParentId = category.CategoryId;
            this.updateBreadcrumb();
            this.loadCategories();
        }
    }

    // Navigate to specific level
    navigateToLevel(level) {
        if (level === 0) {
            this.currentLevel = [];
            this.currentParentId = null;
        } else if (level < this.currentLevel.length) {
            this.currentLevel = this.currentLevel.slice(0, level);
            this.currentParentId = this.currentLevel[this.currentLevel.length - 1].id;
        }
        this.updateBreadcrumb();
        this.loadCategories();
    }

    // Update breadcrumb navigation
    updateBreadcrumb() {
        const breadcrumb = document.getElementById('breadcrumb');
        breadcrumb.innerHTML = `
            <li class="breadcrumb-item ${this.currentLevel.length === 0 ? 'active' : ''}" onclick="categoryManager.navigateToLevel(0)">
                <i class="fas fa-home"></i>
                Main Categories
            </li>
        `;

        this.currentLevel.forEach((level, index) => {
            const isActive = index === this.currentLevel.length - 1;
            breadcrumb.innerHTML += `
                <li class="breadcrumb-item ${isActive ? 'active' : ''}" onclick="categoryManager.navigateToLevel(${index + 1})">
                    ${level.nameEn}
                </li>
            `;
        });
    }

    // Populate parent select dropdown
    populateParentSelect() {
        const select = document.getElementById('parentId');
        const allCategories = this.flattenCategories(this.categories);

        // Clear existing options except the first one
        select.innerHTML = '<option value="">None (Main Category)</option>';

        allCategories.forEach(category => {
            if (this.editingCategoryId !== category.CategoryId) { // Don't allow category to be its own parent
                const option = document.createElement('option');
                option.value = category.CategoryId;
                option.textContent = `${category.NameEn} (${category.NameAr})`;
                select.appendChild(option);
            }
        });
    }

    // Search categories
    searchCategories() {
        this.searchTerm = document.getElementById('searchInput').value.trim();
        this.loadCategories();
    }

    // Find category by ID
    findCategoryById(id, categories = this.categories) {
        for (const category of categories) {
            if (category.CategoryId === id) {
                return category;
            }
            if (category.Children && category.Children.length > 0) {
                const found = this.findCategoryById(id, category.Children);
                if (found) return found;
            }
        }
        return null;
    }

    // Generate new category ID
    generateNewId() {
        const allCategories = this.flattenCategories(this.categories);
        const maxId = Math.max(...allCategories.map(cat => cat.CategoryId));
        return maxId + 1;
    }

    // Add new category
    addCategory(formData) {
        // For MVC: Replace this with AJAX call
        // this.addCategoryToServer(formData);

        const newCategory = {
            CategoryId: this.generateNewId(),
            NameAr: formData.nameAr,
            NameEn: formData.nameEn,
            ParentId: formData.parentId ? parseInt(formData.parentId) : null,
            ImageUrl: formData.imageUrl || '',
            Parent: null,
            CreatedBy: "Admin",
            ProductCount: 0, // عدد المنتجات الأولي
            Children: []
        };

        if (newCategory.ParentId) {
            // Add to parent's children
            const parent = this.findCategoryById(newCategory.ParentId);
            if (parent) {
                parent.Children.push(newCategory);
            }
        } else {
            // Add as main category
            this.categories.push(newCategory);
        }

        this.loadCategories();
        this.populateParentSelect();
    }

    // MVC Integration: Add category to server
    // async addCategoryToServer(formData) {
    //     try {
    //         const response = await fetch('/Category/Create', {
    //             method: 'POST',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //                 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
    //             },
    //             body: JSON.stringify({
    //                 NameAr: formData.nameAr,
    //                 NameEn: formData.nameEn,
    //                 ParentId: formData.parentId ? parseInt(formData.parentId) : null,
    //                 ImageUrl: formData.imageUrl || ''
    //             })
    //         });
    //         
    //         if (response.ok) {
    //             const result = await response.json();
    //             if (result.success) {
    //                 // Reload categories from server
    //                 await this.loadCategoriesFromServer();
    //                 // Show success message
    //                 console.log('Category added successfully');
    //             } else {
    //                 console.error('Failed to add category:', result.message);
    //             }
    //         }
    //     } catch (error) {
    //         console.error('Error adding category:', error);
    //     }
    // }

    // Update existing category
    updateCategory(id, formData) {
        // For MVC: Replace with AJAX call
        // this.updateCategoryOnServer(id, formData);

        const category = this.findCategoryById(id);
        if (category) {
            const oldParentId = category.ParentId;
            const newParentId = formData.parentId ? parseInt(formData.parentId) : null;

            // Update category data
            category.NameAr = formData.nameAr;
            category.NameEn = formData.nameEn;
            category.ImageUrl = formData.imageUrl || '';

            // Handle parent change
            if (oldParentId !== newParentId) {
                // Remove from old parent
                if (oldParentId) {
                    const oldParent = this.findCategoryById(oldParentId);
                    if (oldParent) {
                        oldParent.Children = oldParent.Children.filter(child => child.CategoryId !== id);
                    }
                } else {
                    this.categories = this.categories.filter(cat => cat.CategoryId !== id);
                }

                // Add to new parent
                category.ParentId = newParentId;
                if (newParentId) {
                    const newParent = this.findCategoryById(newParentId);
                    if (newParent) {
                        newParent.Children.push(category);
                    }
                } else {
                    this.categories.push(category);
                }
            }

            this.loadCategories();
            this.populateParentSelect();
        }
    }

    // MVC Integration: Update category on server
    // async updateCategoryOnServer(id, formData) {
    //     try {
    //         const response = await fetch(`/Category/Edit/${id}`, {
    //             method: 'POST',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //                 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
    //             },
    //             body: JSON.stringify({
    //                 CategoryId: id,
    //                 NameAr: formData.nameAr,
    //                 NameEn: formData.nameEn,
    //                 ParentId: formData.parentId ? parseInt(formData.parentId) : null,
    //                 ImageUrl: formData.imageUrl || ''
    //             })
    //         });
    //         
    //         if (response.ok) {
    //             const result = await response.json();
    //             if (result.success) {
    //                 await this.loadCategoriesFromServer();
    //                 console.log('Category updated successfully');
    //             } else {
    //                 console.error('Failed to update category:', result.message);
    //             }
    //         }
    //     } catch (error) {
    //         console.error('Error updating category:', error);
    //     }
    // }

    // Remove category and its children
    removeCategory(id) {
        // For MVC: Replace with AJAX call
        // this.deleteCategoryFromServer(id);

        const removeFromArray = (categories) => {
            for (let i = 0; i < categories.length; i++) {
                if (categories[i].CategoryId === id) {
                    categories.splice(i, 1);
                    return true;
                }
                if (categories[i].Children && categories[i].Children.length > 0) {
                    if (removeFromArray(categories[i].Children)) {
                        return true;
                    }
                }
            }
            return false;
        };

        removeFromArray(this.categories);
        this.loadCategories();
        this.populateParentSelect();
    }

    // MVC Integration: Delete category from server
    // async deleteCategoryFromServer(id) {
    //     try {
    //         const response = await fetch(`/Category/Delete/${id}`, {
    //             method: 'POST',
    //             headers: {
    //                 'Content-Type': 'application/json',
    //                 'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
    //             }
    //         });
    //         
    //         if (response.ok) {
    //             const result = await response.json();
    //             if (result.success) {
    //                 await this.loadCategoriesFromServer();
    //                 console.log('Category deleted successfully');
    //             } else {
    //                 console.error('Failed to delete category:', result.message);
    //             }
    //         }
    //     } catch (error) {
    //         console.error('Error deleting category:', error);
    //     }
    // }

    // Edit category
    editCategory(id) {
        const category = this.findCategoryById(id);
        if (category) {
            this.editingCategoryId = id;
            this.populateParentSelect();

            // Populate form
            document.getElementById('modalTitle').textContent = 'Edit Category';
            document.getElementById('nameAr').value = category.NameAr;
            document.getElementById('nameEn').value = category.NameEn;
            document.getElementById('imageUrl').value = category.ImageUrl || '';
            document.getElementById('parentId').value = category.ParentId || '';

            this.openModal();
        }
    }

    // Add subcategory directly to a parent category
    addSubcategory(parentId) {
        this.editingCategoryId = null;
        this.populateParentSelect();

        // Set parent automatically and disable the select
        document.getElementById('modalTitle').textContent = 'Add Subcategory';
        document.getElementById('parentId').value = parentId;
        document.getElementById('parentId').disabled = true;

        this.openModal();
    }

    // Delete category
    deleteCategory(id) {
        this.categoryToDelete = id;
        document.getElementById('deleteModal').classList.add('active');
    }

    // Open add/edit modal
    openModal() {
        document.getElementById('categoryModal').classList.add('active');
    }

    // Close modal
    closeModal() {
        document.getElementById('categoryModal').classList.remove('active');
        this.resetForm();
    }

    // Close delete modal
    closeDeleteModal() {
        document.getElementById('deleteModal').classList.remove('active');
        this.categoryToDelete = null;
    }

    // Confirm delete
    confirmDelete() {
        if (this.categoryToDelete) {
            this.removeCategory(this.categoryToDelete);
            this.closeDeleteModal();
        }
    }

    // Reset form
    resetForm() {
        document.getElementById('categoryForm').reset();
        document.getElementById('modalTitle').textContent = 'Add Category';
        document.getElementById('parentId').disabled = false; // إعادة تفعيل select
        this.editingCategoryId = null;
        this.populateParentSelect();
    }

    // Handle form submission
    handleFormSubmit(event) {
        event.preventDefault();

        const formData = new FormData(event.target);
        const data = Object.fromEntries(formData.entries());

        if (this.editingCategoryId) {
            this.updateCategory(this.editingCategoryId, data);
        } else {
            this.addCategory(data);
        }

        this.closeModal();
    }
}

// Global functions for HTML event handlers
let categoryManager;

function openAddModal() {
    categoryManager.resetForm();
    categoryManager.openModal();
}

function closeModal() {
    categoryManager.closeModal();
}

function closeDeleteModal() {
    categoryManager.closeDeleteModal();
}

function confirmDelete() {
    categoryManager.confirmDelete();
}

function searchCategories() {
    categoryManager.searchCategories();
}

function navigateToLevel(level) {
    categoryManager.navigateToLevel(level);
}

function handleFormSubmit(event) {
    categoryManager.handleFormSubmit(event);
}

// Add subcategory function for HTML onclick
function addSubcategory(parentId) {
    categoryManager.addSubcategory(parentId);
}

// Initialize the application when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    categoryManager = new CategoryManager();
});

// Close modals when clicking outside
document.addEventListener('click', (e) => {
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
document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
        const categoryModal = document.getElementById('categoryModal');
        const deleteModal = document.getElementById('deleteModal');

        if (categoryModal.classList.contains('active')) {
            closeModal();
        }
        if (deleteModal.classList.contains('active')) {
            closeDeleteModal();
        }
    }
});
