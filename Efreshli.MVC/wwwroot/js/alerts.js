// لازم يكون عندك SweetAlert2 محمّل في الصفحة
// <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

// حذف منتج بالكامل
function deleteProduct() {
    Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc2626',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, delete it',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log('Deleting product...');
            Swal.fire(
                'Deleted!',
                'The product has been deleted.',
                'success'
            );
        }
    });
}

// حذف لون إضافي
function deleteAdditionalColor(colorId) {
    Swal.fire({
        title: 'Are you sure?',
        text: "This color will be removed permanently.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc2626',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, delete it',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            console.log(`Deleting additional color ${colorId}...`);
            const colorElement = document.querySelector(`[data-color-id="${colorId}"]`);
            if (colorElement) {
                colorElement.style.animation = 'fadeOut 0.3s ease';
                setTimeout(() => colorElement.remove(), 300);
            }
            Swal.fire(
                'Deleted!',
                'The color has been removed.',
                'success'
            );
        }
    });
}

// مثال لفانكشن delete عامة (لو عندك حاجات تانية بنفس النمط)
function deleteItem(itemName, callback) {
    Swal.fire({
        title: `Delete this Item?`,
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc2626',
        cancelButtonColor: '#6b7280',
        confirmButtonText: 'Yes, delete it',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            callback();
            Swal.fire(
                'Deleted!',
                `${itemName} has been deleted.`,
                'success'
            );
        }
    });
}
// Image Modal Functions
function openImageModal(imageSrc) {
    const modal = document.getElementById('imageModal');
    const modalImg = document.getElementById('modalImage');
    modal.style.display = 'block';
    modalImg.src = imageSrc;
}

function closeImageModal() {
    document.getElementById('imageModal').style.display = 'none';
}

// Close modal with Escape key
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeImageModal();
    }
});

// Navigation Functions
function goBackToList() {
    if (confirm('Are you sure you want to go back? Any unsaved changes will be lost.')) {
        window.history.back();
        // Or redirect to specific URL: window.location.href = '/Products';
    }
}


// Basic Information Functions
function editBasicInfo() {
    // Redirect to edit page or open modal
    console.log('Editing basic information...');
    // Example: window.location.href = '/Products/Edit/' + productId;
}

function editField(fieldName) {
    const newValue = prompt(`Enter new value for ${fieldName}:`);
    if (newValue !== null) {
        // Add AJAX call to update field
        console.log(`Updating ${fieldName} to: ${newValue}`);
    }
}

function deleteField(fieldName) {
    if (confirm(`Are you sure you want to clear the ${fieldName} field?`)) {
        // Add AJAX call to clear field
        console.log(`Clearing ${fieldName}...`);
    }
}

// Product Item Functions
function editProductItem(itemId) {
    console.log(`Editing product item ${itemId}...`);
    // Add edit logic
}

function deleteProductItem(itemId) {
    if (confirm('Are you sure you want to delete this product item?')) {
        console.log(`Deleting product item ${itemId}...`);
        // Add AJAX call to delete item
    }
}

// Color Functions
function editWoodColor(itemId) {
    console.log(`Editing wood color for item ${itemId}...`);
}

function deleteWoodColor(itemId) {
    if (confirm('Are you sure you want to remove the wood color?')) {
        console.log(`Deleting wood color for item ${itemId}...`);
    }
}

function editFabricColor(itemId) {
    console.log(`Editing fabric color for item ${itemId}...`);
}

function deleteFabricColor(itemId) {
    if (confirm('Are you sure you want to remove the fabric color?')) {
        console.log(`Deleting fabric color for item ${itemId}...`);
    }
}

function addAdditionalColor(itemId) {
    console.log(`Adding additional color for item ${itemId}...`);
}

function editAdditionalColor(colorId) {
    console.log(`Editing additional color ${colorId}...`);
}


// Image Functions
function addProductImage() {
    console.log('Adding new product image...');
    // Open file picker or upload modal
}

function deleteProductImage(imageUrl) {
    if (confirm('Are you sure you want to delete this image?')) {
        console.log(`Deleting image: ${imageUrl}`);
        // Add AJAX call and remove element
        const imageElement = document.querySelector(`[data-image-url="${imageUrl}"]`);
        if (imageElement) {
            imageElement.style.animation = 'fadeOut 0.3s ease';
            setTimeout(() => imageElement.remove(), 300);
        }
    }
}

// Specification Functions
function addSpecification() {
    console.log('Adding new specification...');
}

function editSpecification(specId) {
    console.log(`Editing specification ${specId}...`);
}

function deleteSpecification(specId) {
    if (confirm('Are you sure you want to delete this specification?')) {
        console.log(`Deleting specification ${specId}...`);
        // Add AJAX call and remove element
        const specElement = document.querySelector(`[data-spec-id="${specId}"]`);
        if (specElement) {
            specElement.style.animation = 'fadeOut 0.3s ease';
            setTimeout(() => specElement.remove(), 300);
        }
    }
}

document.head.appendChild(style);