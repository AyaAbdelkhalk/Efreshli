// Sidebar Toggle Functionality
document.addEventListener('DOMContentLoaded', function() {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    let sidebarOpen = false;

    // Initialize sidebar state
    function initSidebar() {
        if (window.innerWidth >= 1024) {
            // Desktop: sidebar always visible
            sidebar.classList.remove('sidebar-collapsed');
            sidebar.classList.add('sidebar-open');
            if (sidebarOverlay) {
                sidebarOverlay.classList.add('hidden');
            }
        } else {
            // Mobile: sidebar hidden by default
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
        sidebarOverlay.addEventListener('click', function() {
            if (window.innerWidth < 1024) {
                sidebarOpen = false;
                sidebar.classList.add('sidebar-collapsed');
                sidebar.classList.remove('sidebar-open');
                sidebarOverlay.classList.add('hidden');
            }
        });
    }

    // Handle window resize
    window.addEventListener('resize', initSidebar);

    // Initialize on load
    initSidebar();
});

// Profile Dropdown Functionality
document.addEventListener('DOMContentLoaded', function() {
    const profileDropdown = document.getElementById('profileDropdown');
    const profileMenu = document.getElementById('profileMenu');
    
    if (profileDropdown && profileMenu) {
        profileDropdown.addEventListener('click', function(e) {
            e.stopPropagation();
            profileMenu.classList.toggle('hidden');
        });
        
        document.addEventListener('click', function() {
            profileMenu.classList.add('hidden');
        });
    }
});

// Navigation Active State Management
document.addEventListener('DOMContentLoaded', function() {
    const navItems = document.querySelectorAll('.nav-item, .nav-link');

    navItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();

            // Remove active class from all nav items
            navItems.forEach(navItem => {
                navItem.classList.remove('active');
                navItem.classList.remove('text-white', 'shadow-lg');
                navItem.classList.add('text-gray-600');
                navItem.classList.add('hover:text-gray-900');
                
                // Reset arrow visibility
                const arrow = navItem.querySelector('.ml-auto i');
                if (arrow) {
                    navItem.querySelector('.ml-auto').classList.add('opacity-0', 'group-hover:opacity-100');
                }
            });

            // Add active class to clicked item
            this.classList.add('active');
            this.classList.add('text-white', 'shadow-lg');
            this.classList.remove('text-gray-600', 'hover:text-gray-900');
            
            // Show arrow for active item
            const arrow = this.querySelector('.ml-auto i');
            if (arrow) {
                this.querySelector('.ml-auto').classList.remove('opacity-0', 'group-hover:opacity-100');
            }
        });
    });
});
