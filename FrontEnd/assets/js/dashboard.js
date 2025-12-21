const token = localStorage.getItem("token");

function headers() {
    return {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
    };
}

document.addEventListener("DOMContentLoaded", () => {
    loadAllReservations();
    loadAllOrders();
});

function showSection(id) {
    document.querySelectorAll(".dash-section").forEach(sec =>
        sec.classList.add("d-none")
    );
    document.getElementById(id).classList.remove("d-none");
}

async function getUser() {
    const id = Number(userId.value);
    if (!id || id < 1) return alert("Enter valid User ID");

    const res = await fetch(`${API_BASE}/ID/${id}`, { headers: headers() });
    if (!res.ok) return alert("User not found");

    const data = await res.json();
    userForm.classList.remove("d-none");

    fullName.value = data.fullName;
    phone.value = data.phone;
    roleId.value = data.roleId;
    userIsActive.value = data.isActive.toString();
    password.value = "";
}

async function getUsername() {
    const name = (username.value);
    // if (!id || id < 1) return alert("Enter valid User ID");

    if (!name) return alert("Enter valid User name");

    const res = await fetch(`${API_BASE}/GetUserByFullName/${name}`, { headers: headers() });
    if (!res.ok) return alert("User not found");

    const data = await res.json();
    userForm.classList.remove("d-none");

    fullName.value = data.fullName;
    phone.value = data.phone;
    roleId.value = data.roleId;
    userIsActive.value = data.isActive.toString();
    password.value = "";
}

async function updateUser() {
    const payload = {
        userID: Number(userId.value),
        fullName: fullName.value.trim(),
        phone: phone.value.trim(),
        roleId: Number(roleId.value),
        isActive: userIsActive.value === "true",
        password: password.value.trim() === "" ? null : password.value
    };

    const res = await fetch(`${API_BASE}/UpdateUser`, {
        method: "PUT",
        headers: headers(),
        body: JSON.stringify(payload)
    });

    if (!res.ok) return alert(await res.text());
    alert("User Updated");
}

async function deleteUser() {
    const id = Number(userId.value);
    if (!id || id < 1) return alert("Invalid User ID");

    const res = await fetch(`${API_BASE}/DeleteUser/${id}`, {
        method: "DELETE",
        headers: headers()
    });

    if (!res.ok) return alert(await res.text());
    alert("User Deleted");
}

async function getReservationById() {
    const id = reservationId.value;
    if (!id) return alert("Enter Reservation ID");

    const res = await fetch(
        `${API_BASE}/GetReservationByReservationID/${id}`,
        { headers: headers() }
    );

    if (!res.ok) {
        reservationResult.innerHTML = "Reservation not found";
        return;
    }

    const r = await res.json();

    reservationResult.innerHTML = `
    <table class="nice-table">
        <thead>
            <tr>
                <th>ID</th>
                <th>Customer</th>
                <th>Table</th>
                <th>Date</th>
                <th>Status</th>
                <th>Created</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>${r.reservationId}</td>
                <td>${r.customerId}</td>
                <td>${r.tableId}</td>
                <td>${new Date(r.reservationDate).toLocaleString()}</td>
                <td>${r.statusId}</td>
                <td>${new Date(r.createdAt).toLocaleString()}</td>
            </tr>
        </tbody>
    </table>
    `;
}

function statusInfoById(id) {
    switch (Number(id)) {
        case 1: return { name: "Pending", cls: "pending" };
        case 2: return { name: "Cancelled", cls: "cancelled" };
        case 3: return { name: "Completed", cls: "completed" };
        case 4: return { name: "Active", cls: "active" };
        default: return { name: "Unknown", cls: "" };
    }
}

function remainingTime(expiryDate) {
    if (!expiryDate) return "-";
    const diff = new Date(expiryDate) - new Date();
    if (diff <= 0) return "Expired";
    const m = Math.floor(diff / 60000);
    const h = Math.floor(m / 60);
    return h > 0 ? `${h}h ${m % 60}m` : `${m}m`;
}

const modal = document.getElementById("orderModal");
const modalItemsBody = document.getElementById("modalItemsBody");
const closeModalBtn = document.getElementById("closeModal");

closeModalBtn.onclick = () => modal.classList.remove("show");
modal.onclick = e => e.target === modal && modal.classList.remove("show");

function openOrderDetails(items) {
    modalItemsBody.innerHTML = "";

    if (!items || !items.length) {
        modalItemsBody.innerHTML =
            `<tr><td colspan="4">No items</td></tr>`;
    } else {
        items.forEach(i => {
            modalItemsBody.innerHTML += `
            <tr>
                <td>${i.name}</td>
                <td>${i.quantity}</td>
                <td>${i.unitPrice}</td>
                <td>${i.priceTotal}</td>
            </tr>`;
        });
    }
    modal.classList.add("show");
}

const tableBody = document.querySelector("#orderTable tbody");
const tableEl = document.getElementById("orderTable");
const infoEl = document.getElementById("orderInfo");
const noDataEl = document.getElementById("noData");

async function getCustomerOrders() {
    const customerId = customerIdInput.value;
    if (!customerId) return alert("Enter Customer ID");

    tableBody.innerHTML = "";
    tableEl.classList.add("d-none");
    noDataEl.style.display = "none";
    infoEl.textContent = "Loading...";

    try {
        const res = await fetch(
            `${API_BASE}/GetCustomerOrders/${customerId}`,
            { headers: headers() }
        );

        if (!res.ok) throw new Error();

        const data = await res.json();
        data.forEach(w => {
            const o = w.order;
            const status = statusInfoById(o.statusId);

            const tr = document.createElement("tr");
            tr.innerHTML = `
            <td>${o.orderId}</td>
            <td><span class="status ${status.cls}">${status.name}</span></td>
            <td>${o.totalAmount}</td>
            <td>${o.reservationId ?? "-"}</td>
            <td>${new Date(o.createdAt).toLocaleString()}</td>
            <td>${remainingTime(o.expiryDate)}</td>
            <td><button class="link-btn">View</button></td>
            `;

            tr.querySelector("button").onclick =
                () => openOrderDetails(w.items);

            tableBody.appendChild(tr);
        });

        infoEl.textContent = `Showing ${data.length} orders`;
        tableEl.classList.remove("d-none");

    } catch {
        infoEl.textContent = "";
        noDataEl.style.display = "block";
    }
}

async function loadAllReservations() {
    const tbody = document.querySelector("#allReservationsTable tbody");
    tbody.innerHTML = "";

    const res = await fetch(`${API_BASE}/GetAllReservations`, {
        headers: headers()
    });

    if (!res.ok) return;

    const data = await res.json();
    data.forEach(r => {
        tbody.innerHTML += `
        <tr>
            <td>${r.reservationId}</td>
            <td>${r.customerId}</td>
            <td>${r.tableId}</td>
            <td>${new Date(r.reservationDate).toLocaleString()}</td>
            <td>${r.statusId}</td>
            <td>${new Date(r.createdAt).toLocaleString()}</td>
        </tr>`;
    });
}

async function loadAllOrders() {
    const tbody = document.querySelector("#allOrdersTable tbody");
    tbody.innerHTML = "";

    const res = await fetch(`${API_BASE}/GetAllOrders`, {
        headers: headers()
    });

    if (!res.ok) return;

    const data = await res.json();
    data.forEach(w => {
        const o = w.order;
        const status = statusInfoById(o.statusId);

        tbody.innerHTML += `
        <tr>
            <td>${o.orderId}</td>
            <td>${o.customerId}</td>
            <td><span class="status ${status.cls}">${status.name}</span></td>
            <td>${o.totalAmount}</td>
            <td>${o.reservationId ?? "-"}</td>
            <td>${new Date(o.createdAt).toLocaleString()}</td>
            <td>${remainingTime(o.expiryDate)}</td>
            <td>
            <button class="link-btn"
                onclick='openOrderDetails(${JSON.stringify(w.items)})'>
                View
            </button>
            </td>
        </tr>`;
    });
}

async function getMenuItem() {
    const id = Number(menuItemId.value);
    if (!id) return alert("Invalid ID");

    const res = await fetch(
        `${API_BASE}/GetMenuItemsByMenuItemID/${id}`,
        { headers: headers() }
    );

    if (!res.ok) return alert("Not found");

    const d = await res.json();
    menuName.value = d.name ?? "";
    menuDesc.value = d.description ?? "";
    menuCategory.value = d.categoryId ?? "";
    menuPrice.value = d.price ?? "";
    menuTime.value = d.perparationTime ?? "";
    menuIsActive.value = (d.isActive ?? false).toString();
}

async function addMenuItem() {
    const payload = {
        name: menuName.value,
        description: menuDesc.value,
        categoryId: Number(menuCategory.value),
        price: Number(menuPrice.value),
        perparationTime: Number(menuTime.value),
        isActive: menuIsActive.value === "true"
    };

    const res = await fetch(`${API_BASE}/AddMenuItem`, {
        method: "POST",
        headers: headers(),
        body: JSON.stringify(payload)
    });

    if (!res.ok) return alert(await res.text());
    alert("Menu Item Added");
}

async function updateMenuItem() {
    const payload = {
        MenuItemId: Number(menuItemId.value),
        Name: menuName.value,
        Description: menuDesc.value || null,
        CategoryId: Number(menuCategory.value),
        Price: Number(menuPrice.value),
        PerparationTime: Number(menuTime.value),
        IsActive: menuIsActive.value === "true"
    };

    const res = await fetch(`${API_BASE}/UpdaterMenuItem`, {
        method: "PUT",
        headers: headers(),
        body: JSON.stringify(payload)
    });

    if (!res.ok) return alert(await res.text());
    alert("Menu Item Updated");
}

async function deleteMenuItem() {
    const id = Number(menuItemId.value);
    if (!id) return alert("Invalid ID");

    const res = await fetch(`${API_BASE}/DeleteMenuItem/${id}`, {
        method: "DELETE",
        headers: headers()
    });

    if (!res.ok) return alert(await res.text());
    alert("Menu Item Deleted");
}

