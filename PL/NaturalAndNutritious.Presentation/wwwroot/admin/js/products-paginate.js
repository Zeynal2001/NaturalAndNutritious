let pageSize = document.getElementById('pageSize');

pageSize.addEventListener('change', () => {
    window.location = 'https://localhost:7032/admin_panel/Products/GetAllProducts?pageSize=${pageSize.value}';
})