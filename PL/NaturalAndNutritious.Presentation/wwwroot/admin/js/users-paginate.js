let pageSize = document.getElementById('pageSize');
let baseUrl = window.location.origin;

pageSize.addEventListener('change', () => {
    window.location = `https://localhost:7032/admin_panel/Users/GetAllUsers?pageSize=${pageSize.value}`;
})
/**
 * let pageSize = document.getElementById('pageSize');
let baseUrl = window.location.origin;

pageSize.addEventListener('change', () => {
    window.location = `${baseUrl}/admin_panel/Users/GetAllUsers?pageSize=${pageSize.value}`;
})
 */