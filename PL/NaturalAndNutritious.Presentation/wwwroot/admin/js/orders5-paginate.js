﻿let pageSize = document.getElementById('pageSize');
let baseUrl = window.location.origin;

pageSize.addEventListener('change', () => {
    window.location = `${baseUrl}/admin_panel/Orders/GetCancelledOrders?pageSize=${pageSize.value}`;
})