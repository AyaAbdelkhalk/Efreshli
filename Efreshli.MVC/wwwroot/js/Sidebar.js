document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    let sidebarOpen = false;

    // Initialize sidebar state
    function initSidebar() {
        if (window.innerWidth >= 1024) {
            sidebar.classList.remove('sidebar-collapsed');
            sidebar.classList.add('sidebar-open');
            if (sidebarOverlay) {
                sidebarOverlay.classList.add('hidden');
            }
        } else {
            sidebar.classList.add('sidebar-collapsed');
            sidebar.classList.remove('sidebar-open');
            if (sidebarOverlay) {
                sidebarOverlay.classList.add('hidden');
            }
            sidebarOpen = false;
        }
    }

    // Toggle sidebar function
    function toggleSidebar() {
        if (window.innerWidth < 1024) {
            sidebarOpen = !sidebarOpen;
            if (sidebarOpen) {
                sidebar.classList.remove('sidebar-collapsed');
                sidebar.classList.add('sidebar-open');
                if (sidebarOverlay) {
                    sidebarOverlay.classList.remove('hidden');
                }
            } else {
                sidebar.classList.add('sidebar-collapsed');
                sidebar.classList.remove('sidebar-open');
                if (sidebarOverlay) {
                    sidebarOverlay.classList.add('hidden');
                }
            }
        }
    }

    // Event listeners
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }

    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', function () {
            if (window.innerWidth < 1024) {
                sidebarOpen = false;
                sidebar.classList.add('sidebar-collapsed');
                sidebar.classList.remove('sidebar-open');
                sidebarOverlay.classList.add('hidden');
            }
        });
    }

    window.addEventListener('resize', initSidebar);
    initSidebar();
});

// Profile Dropdown Functionality
document.addEventListener('DOMContentLoaded', function () {
    const profileDropdown = document.getElementById('profileDropdown');
    const profileMenu = document.getElementById('profileMenu');

    if (profileDropdown && profileMenu) {
        profileDropdown.addEventListener('click', function (e) {
            e.stopPropagation();
            profileMenu.classList.toggle('hidden');
        });

        document.addEventListener('click', function () {
            profileMenu.classList.add('hidden');
        });
    }
});

// Navigation Active State Management - Fixed to only highlight based on current route
document.addEventListener('DOMContentLoaded', function () {
    const navItems = document.querySelectorAll('.nav-link');
    console.log('Found nav items:', navItems.length);

    // Function to clear all active states
    function clearAllActiveStates() {
        navItems.forEach(item => {
            // Only remove JavaScript-added active classes, preserve server-side ones
            if (!item.hasAttribute('data-server-active')) {
                item.classList.remove('active');
            }
        });
    }

    // Function to set active based on current URL/Route
    function setActiveFromCurrentRoute() {
        const currentPath = window.location.pathname.toLowerCase();
        let activeItemFound = false;

        console.log('Current path:', currentPath);

        // First, mark any existing server-side active states
        navItems.forEach((item) => {
            if (item.classList.contains('active')) {
                item.setAttribute('data-server-active', 'true');
                activeItemFound = true;
                console.log('Found server-side active state:', item.querySelector('.sidebar-text')?.textContent);
            }
        });

        // If no server-side active state found, try to set based on URL
        if (!activeItemFound) {
            navItems.forEach((item) => {
                const controller = item.getAttribute('asp-controller') || '';
                const action = item.getAttribute('asp-action') || '';
                const href = item.getAttribute('href') || '';

                // Check if current URL matches this nav item's controller
                if (controller && currentPath.includes(`/${controller.toLowerCase()}`)) {
                    item.classList.add('active');
                    activeItemFound = true;
                    console.log(`Set active via JS: ${controller} - Current path: ${currentPath}`);
                }

                // Fallback: check href for direct matches (for links without controllers)
                else if (href && href !== '#' && currentPath === href.toLowerCase()) {
                    item.classList.add('active');
                    activeItemFound = true;
                    console.log(`Set active via href: ${href}`);
                }

                // Special case for Home/Dashboard - check if we're at root
                else if (controller.toLowerCase() === 'home' && (currentPath === '/' || currentPath === '' || currentPath === '/home' || currentPath.includes('/home/'))) {
                    item.classList.add('active');
                    activeItemFound = true;
                    console.log('Set active: Home (root path)');
                }
            });
        }

        // If still no match found, set Dashboard/Home as default
        if (!activeItemFound) {
            const dashboardItem = document.querySelector('.nav-link[asp-controller="Home"]');
            if (dashboardItem) {
                dashboardItem.classList.add('active');
                console.log('Set default active: Dashboard');
            }
        }

        return activeItemFound;
    }

    // Only set up click handlers for actual navigation (not all clicks)
    navItems.forEach((item) => {
        item.addEventListener('click', function (e) {
            const href = this.getAttribute('href');
            const controller = this.getAttribute('asp-controller');

            // Only handle navigation clicks, not buttons or other elements
            if (href && href !== '#' || controller) {
                console.log(`Navigation clicked: ${controller || href}`);
                // Don't set active state here - let the page reload and handle it naturally
                // The active state will be set correctly when the new page loads
            }
        });
    });

    // Initialize active state based on current route only
    setActiveFromCurrentRoute();

    console.log('Navigation initialized - checking for active states...');

    // Debug: Show which items are currently active
    navItems.forEach((item, index) => {
        if (item.classList.contains('active')) {
            console.log(`Active item ${index}:`, item.querySelector('.sidebar-text')?.textContent);
        }
    });
});

// Search functionality (optional enhancement)
document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.querySelector('input[placeholder="Search..."]');

    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const searchTerm = e.target.value.toLowerCase();
            const navItems = document.querySelectorAll('.nav-link');

            navItems.forEach(item => {
                const text = item.querySelector('.sidebar-text')?.textContent.toLowerCase() || '';
                const parent = item.parentElement;

                if (text.includes(searchTerm) || searchTerm === '') {
                    item.style.display = 'flex';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }
});

// Enhanced smooth scrolling for better UX
document.addEventListener('DOMContentLoaded', function () {
    const links = document.querySelectorAll('a[href^="#"]');

    links.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);

            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});