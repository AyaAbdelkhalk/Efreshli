document.addEventListener('DOMContentLoaded', function() {
    // Get the data from the server
    const orderStatusData = window.dashboardData.orderStatus;
    const paymentMethodData = window.dashboardData.paymentMethod;
    const categoryProductCountData = window.dashboardData.categoryProductCount;
    
    // Process data for charts
    const orderStatusLabels = Object.keys(orderStatusData);
    const orderStatusValues = Object.values(orderStatusData);
    
    const paymentMethodLabels = Object.keys(paymentMethodData);
    const paymentMethodValues = Object.values(paymentMethodData);
    
    const categoryLabels = Object.keys(categoryProductCountData);
    const categoryValues = Object.values(categoryProductCountData);
    
    // Define soft, calm colors for charts (pastel tones)
    const backgroundColors = [
        'rgba(173, 216, 230, 0.8)', // Light blue
        'rgba(144, 238, 144, 0.8)', // Light green
        'rgba(255, 182, 193, 0.8)', // Light pink
        'rgba(221, 160, 221, 0.8)', // Plum
        'rgba(255, 218, 185, 0.8)', // Peach
        'rgba(175, 238, 238, 0.8)', // Pale turquoise
        'rgba(240, 230, 140, 0.8)', // Khaki
        'rgba(211, 211, 211, 0.8)'  // Light gray
    ];
    
    const borderColors = [
        'rgba(173, 216, 230, 1)', // Light blue
        'rgba(144, 238, 144, 1)', // Light green
        'rgba(255, 182, 193, 1)', // Light pink
        'rgba(221, 160, 221, 1)', // Plum
        'rgba(255, 218, 185, 1)', // Peach
        'rgba(175, 238, 238, 1)', // Pale turquoise
        'rgba(240, 230, 140, 1)', // Khaki
        'rgba(211, 211, 211, 1)'  // Light gray
    ];
    
    // Order Status Chart
    const orderStatusCtx = document.getElementById('orderStatusChart').getContext('2d');
    const orderStatusChart = new Chart(orderStatusCtx, {
        type: 'doughnut',
        data: {
            labels: orderStatusLabels,
            datasets: [{
                label: 'Order Status',
                data: orderStatusValues,
                backgroundColor: backgroundColors.slice(0, orderStatusLabels.length),
                borderColor: borderColors.slice(0, orderStatusLabels.length),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: 'Order Status Distribution'
                }
            }
        }
    });
    
    // Payment Method Chart
    const paymentMethodCtx = document.getElementById('paymentMethodChart').getContext('2d');
    const paymentMethodChart = new Chart(paymentMethodCtx, {
        type: 'bar',
        data: {
            labels: paymentMethodLabels,
            datasets: [{
                label: 'Payment Methods',
                data: paymentMethodValues,
                backgroundColor: backgroundColors.slice(0, paymentMethodLabels.length),
                borderColor: borderColors.slice(0, paymentMethodLabels.length),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                title: {
                    display: true,
                    text: 'Payment Methods Usage'
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        precision: 0
                    }
                }
            }
        }
    });
    
    // Category Distribution Chart
    const categoryCtx = document.getElementById('categoryChart').getContext('2d');
    const categoryChart = new Chart(categoryCtx, {
        type: 'pie',
        data: {
            labels: categoryLabels,
            datasets: [{
                label: 'Products by Category',
                data: categoryValues,
                backgroundColor: backgroundColors.slice(0, categoryLabels.length),
                borderColor: borderColors.slice(0, categoryLabels.length),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                },
                title: {
                    display: true,
                    text: 'Products Distribution by Category'
                }
            }
        }
    });
});