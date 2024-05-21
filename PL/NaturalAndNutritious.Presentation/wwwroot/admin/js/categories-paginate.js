let pageSize = document.getElementById('pageSize');

pageSize.addEventListener('change', () => {
    window.location = 'https://localhost:7032/admin_panel/Categories/GetAllCategories?pageSize=${pageSize.value}';
})