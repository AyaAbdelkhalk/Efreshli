// Sidebar Toggle Functionality
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

// Navigation Active State Management - Enhanced for default Dashboard state
document.addEventListener('DOMContentLoaded', function () {
    const navItems = document.querySelectorAll('.nav-link');
    console.log('Found nav items:', navItems.length);

    // Function to clear all active states
    function clearAllActiveStates() {
        navItems.forEach(item => {
            item.classList.remove('active');
        });
    }

    // Function to set active state for specific item
    function setActiveItem(activeItem) {
        clearAllActiveStates();
        activeItem.classList.add('active');

        // Save the active state in localStorage
        const itemText = activeItem.querySelector('.sidebar-text')?.textContent;
        const itemHref = activeItem.getAttribute('href') || '';
        const itemController = activeItem.getAttribute('asp-controller') || '';
        const itemAction = activeItem.getAttribute('asp-action') || '';

        localStorage.setItem('activeNavItem', JSON.stringify({
            text: itemText,
            href: itemHref,
            controller: itemController,
            action: itemAction
        }));

        console.log('Active set to:', itemText);
    }

    // Function to set Dashboard as default active
    function setDashboardAsDefault() {
        const dashboardItem = document.querySelector('.nav-link[href*="Home"], .nav-link[asp-controller="Home"]');
        if (dashboardItem) {
            dashboardItem.classList.add('active');
            console.log('Dashboard set as default active');
        }
    }

    // Function to restore active state from localStorage
    function restoreActiveState() {
        const savedActiveItem = localStorage.getItem('activeNavItem');
        if (savedActiveItem) {
            try {
                const activeData = JSON.parse(savedActiveItem);

                // Find matching nav item
                let itemFound = false;
                navItems.forEach(item => {
                    const itemText = item.querySelector('.sidebar-text')?.textContent;
                    const itemHref = item.getAttribute('href') || '';
                    const itemController = item.getAttribute('asp-controller') || '';
                    const itemAction = item.getAttribute('asp-action') || '';

                    // Check if this item matches the saved one
                    if ((activeData.text === itemText) ||
                        (activeData.href && activeData.href === itemHref) ||
                        (activeData.controller === itemController && activeData.action === itemAction)) {
                        item.classList.add('active');
                        itemFound = true;
                        console.log('Restored active state to:', itemText);
                    }
                });

                // If no saved item found, set Dashboard as default
                if (!itemFound) {
                    setDashboardAsDefault();
                }
            } catch (e) {
                console.log('Error restoring active state:', e);
                setDashboardAsDefault();
            }
        } else {
            // No saved state, set Dashboard as default
            setDashboardAsDefault();
        }
    }

    // Function to set active based on current URL
    function setActiveFromCurrentUrl() {
        const currentPath = window.location.pathname.toLowerCase();
        let activeItemFound = false;

        navItems.forEach((item) => {
            const href = item.getAttribute('href') || '';
            const controller = item.getAttribute('asp-controller') || '';
            const action = item.getAttribute('asp-action') || '';

            // Check if current URL matches this nav item
            if (href && currentPath.includes(href.toLowerCase())) {
                setActiveItem(item);
                activeItemFound = true;
            } else if (controller && action) {
                const routePath = `/${controller.toLowerCase()}/${action.toLowerCase()}`;
                if (currentPath.includes(routePath) || currentPath.includes(controller.toLowerCase())) {
                    setActiveItem(item);
                    activeItemFound = true;
                }
            }
        });

        return activeItemFound;
    }

    // Set up click handlers
    navItems.forEach((item, index) => {
        item.addEventListener('click', function (e) {
            console.log(`Clicked item ${index}: ${this.querySelector('.sidebar-text')?.textContent}`);
            setActiveItem(this);
        });
    });

    // Initialize active state
    clearAllActiveStates();

    // Try to set active based on current URL first
    const urlActiveSet = setActiveFromCurrentUrl();

    // If no URL match, try to restore from localStorage or set Dashboard as default
    if (!urlActiveSet) {
        restoreActiveState();
    }

    console.log('Navigation initialized');
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