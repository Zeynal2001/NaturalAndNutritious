let pageSize = document.getElementById('pageSize');
let baseUrl = window.location.origin;

pageSize.addEventListener('change', () => {
    window.location = `${baseUrl}/admin_panel/Suppliers/GetAllSuppliers?pageSize=${pageSize.value}`;
})