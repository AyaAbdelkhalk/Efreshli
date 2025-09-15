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
    
    // Define colors for charts
    const backgroundColors = [
        'rgba(255, 99, 132, 0.8)',
        'rgba(54, 162, 235, 0.8)',
        'rgba(255, 206, 86, 0.8)',
        'rgba(75, 192, 192, 0.8)',
        'rgba(153, 102, 255, 0.8)',
        'rgba(255, 159, 64, 0.8)',
        'rgba(199, 199, 199, 0.8)'
    ];
    
    const borderColors = [
        'rgba(255, 99, 132, 1)',
        'rgba(54, 162, 235, 1)',
        'rgba(255, 206, 86, 1)',
        'rgba(75, 192, 192, 1)',
        'rgba(153, 102, 255, 1)',
        'rgba(255, 159, 64, 1)',
        'rgba(199, 199, 199, 1)'
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