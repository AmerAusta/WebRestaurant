
const infoEl = document.getElementById("info");
const noDataEl = document.getElementById("noData");
const tableBody = document.querySelector("#bookingTable tbody");
const refreshBtn = document.getElementById("refreshBtn");

function getQueryParam(name) {
  try {
    const url = new URL(window.location.href);
    return url.searchParams.get(name);
  } catch {
    return null;
  }
}

function parseJwt(token) {
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
  } catch {
    return null;
  }
}

function parseDateSafe(dateString) {
  if (!dateString) return new Date(NaN);
  const d = new Date(dateString);
  if (!isNaN(d.getTime())) return d;
  const cleaned = dateString.replace(/\//g, '-').replace(' ', 'T');
  return new Date(cleaned);
}

function formatRemaining(minutes) {
  if (isNaN(minutes)) return "Unknown";
  if (minutes <= 0) return "Expired";
  const hrs = Math.floor(minutes / 60);
  const mins = Math.floor(minutes % 60);
  return hrs > 0 ? `${hrs} hour(s) ${mins} min(s)` : `${mins} min(s)`;
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


function getEffectiveUserId() {
  const qId = getQueryParam('id');
  if (qId && !isNaN(Number(qId))) return Number(qId);

  const stored = localStorage.getItem('userId');
  if (stored && !isNaN(Number(stored))) return Number(stored);

  const token = localStorage.getItem('token');
  if (token) {
    const payload = parseJwt(token);
    const p = payload?.UserID ?? payload?.userId ?? payload?.sub ?? payload?.nameid;
    if (p && !isNaN(Number(p))) return Number(p);
  }
  return null;
}

async function cancelReservation(reservationId) {
  const token = localStorage.getItem("token");
  if (!token) {
    alert("You must be logged in.");
    return false;
  }

  try {
    const res = await fetch(
      `${API_BASE}/CancelReservation/${reservationId}`,
      {
        method: "DELETE",
        headers: {
          "Authorization": "Bearer " + token
        }
      }
    );

    if (!res.ok) {
      const err = await res.json();
      alert(err.message || "Failed to cancel reservation");
      return false;
    }

    return true;
  } catch (e) {
    console.error("Cancel error:", e);
    alert("Server error while cancelling reservation.");
    return false;
  }
}

async function fetchBookings() {
  const token = localStorage.getItem("token");
  const id = getEffectiveUserId();

  if (!token) {
    noDataEl.style.display = "block";
    noDataEl.textContent = "You must log in first (token not found).";
    return [];
  }

  const url = id
    ? `${API_BASE}/GetReservationsByCustomerID/${id}`
    : `${API_BASE}/me/reservations`;

  try {
    const res = await fetch(url, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer " + token
      }
    });

    if (!res.ok) {
      if (res.status === 401) throw new Error("Invalid or expired token.");
      if (res.status === 403) throw new Error("Access forbidden (403).");
      if (res.status === 404) return [];
      throw new Error(`Server returned ${res.status}`);
    }

    const data = await res.json();
    if (Array.isArray(data)) return data;
    if (data?.RDTO) return Array.isArray(data.RDTO) ? data.RDTO : [data.RDTO];
    if (data?.reservations) return Array.isArray(data.reservations) ? data.reservations : [data.reservations];
    return data ? [data] : [];
  } catch (err) {
    console.error("fetchBookings error:", err);
    noDataEl.style.display = "block";
    noDataEl.textContent = "Failed to fetch data from server.";
    return [];
  }
}

async function loadAndRender() {
  tableBody.innerHTML = "";
  noDataEl.style.display = "none";
  infoEl.innerHTML = `<span class="muted">Loading bookings...</span>`;

  const bookings = await fetchBookings();

  if (!bookings || bookings.length === 0) {
    noDataEl.style.display = "block";
    infoEl.innerHTML = "";
    noDataEl.textContent = "No bookings available.";
    return;
  }

  infoEl.innerHTML = `<span class="muted">Showing ${bookings.length} booking(s)</span>`;

  bookings.forEach(item => {
    const reservationId = item.reservationId ?? item.id;
    const tableId = item.tableId;
    const statusId = item.statusId;
    const notes = item.notes ?? "-";
    const reservationDate = parseDateSafe(item.reservationDate);
    const createdAt = parseDateSafe(item.createdAt);
    const diffMinutes = (reservationDate - new Date()) / 1000 / 60;
    const canCancel = diffMinutes > 60;

    const statusInfo = statusInfoById(statusId);

    const tr = document.createElement("tr");
    const hasOrder =
    item.hasOrder === true ||
    item.orderId != null;

  const orderBtn = hasOrder
    ? `<button class="small-btn order-btn"
          data-reservation="${reservationId}">
          View Order
      </button>`
    : `<button class="primary-btn order-btn"
          data-reservation="${reservationId}"
          data-table="${tableId}">
          Add Order
      </button>`;

  tr.innerHTML = `
    <td>${reservationId ?? "-"}</td>
    <td>${tableId ?? "-"}</td>
    <td>${reservationDate.toLocaleString()}</td>
    <td>${formatRemaining(diffMinutes)}</td>
    <td><span class="status ${statusInfo.cls}">${statusInfo.name}</span></td>
    <td>${notes}</td>
    <td>${createdAt.toLocaleString()}</td>
    <td>
      ${canCancel
        ? `<button class="cancel-btn" data-id="${reservationId}">Cancel</button>`
        : `<button class="cancel-btn" disabled>Cannot cancel</button>`}
    </td>
    <td>
      ${orderBtn}
    </td>
  `;
    tableBody.appendChild(tr);
  });

  document.querySelectorAll(".cancel-btn").forEach(btn => {
    btn.addEventListener("click", async () => {
      if (btn.disabled) return alert("Cancellation not allowed.");
      if (!confirm("Are you sure you want to cancel this booking?")) return;

      const id = btn.dataset.id;
      const success = await cancelReservation(id);

      if (success) {
        alert("Reservation cancelled successfully.");
        loadAndRender();
      }
    });
  });
}

document.addEventListener("click", (e) => {
  if (!e.target.classList.contains("order-btn")) return;

  const reservationId = e.target.dataset.reservation;
  const tableId = e.target.dataset.table;

  if (tableId) {
    sessionStorage.setItem("reservationIdForOrder", reservationId);
    sessionStorage.setItem("tableIdForOrder", tableId);

    window.location.href = `index.html#Menu`;
  } else {

    window.location.href = `ViewOrders.html?reservationId=${reservationId}`;
  }
});

if (refreshBtn) refreshBtn.addEventListener("click", loadAndRender);
document.addEventListener("DOMContentLoaded", loadAndRender);
