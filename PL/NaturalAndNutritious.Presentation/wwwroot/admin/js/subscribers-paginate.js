let pageSize = document.getElementById('pageSize');
let baseUrl = window.location.origin;

pageSize.addEventListener('change', () => {
    window.location = `${baseUrl}/admin_panel/Subscribers/GetAllSubscribers?pageSize=${pageSize.value}`;
})