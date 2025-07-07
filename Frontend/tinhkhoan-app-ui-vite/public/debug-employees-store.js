// Debug Employees trong Vue App
console.log('=== Debugging Employees Store ===');

// Test trong context của Vue app
setTimeout(() => {
    try {
        // Access Pinia store from window if available
        if (window.__VUE_DEVTOOLS_GLOBAL_HOOK__) {
            console.log('Vue DevTools detected');
        }

        // Log sau khi component mount
        console.log('Checking employeeStore state...');

        // Simulate kiểm tra trong mounted hook
        fetch('/api/Employees')
            .then(response => response.json())
            .then(data => {
                console.log('API Response (first employee):', data[0]);
                console.log('Total employees from API:', data.length);

                // Test mapping logic
                const mapped = data.map(employee => ({
                    Id: employee.Id,
                    EmployeeCode: employee.EmployeeCode || '',
                    FullName: employee.FullName || '',
                    Username: employee.Username || ''
                }));

                console.log('Mapped employees (first):', mapped[0]);
                console.log('All employees valid?', mapped.every(emp => emp.EmployeeCode && emp.FullName && emp.Username));
            })
            .catch(error => {
                console.error('API Error:', error);
            });

    } catch (error) {
        console.error('Debug error:', error);
    }
}, 2000);
