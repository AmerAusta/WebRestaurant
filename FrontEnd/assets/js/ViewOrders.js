const tableBody = document.querySelector("#orderTable tbody");
const infoEl = document.getElementById("info");
const noDataEl = document.getElementById("noData");


const modal = document.getElementById("orderModal");
const modalItemsBody = document.getElementById("modalItemsBody");
const closeModalBtn = document.getElementById("closeModal");

const customerId = localStorage.getItem("userId");

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
  const diffMs = new Date(expiryDate) - new Date();
  if (diffMs <= 0) return "Expired";

  const totalMinutes = Math.floor(diffMs / 60000);
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;

  if (hours > 0) {
    return `${hours} h ${minutes} min`;
  } else {
    return `${minutes} min`;
  }
}

function openOrderDetails(items) {
  modalItemsBody.innerHTML = "";

  if (!items || !items.length) {
    modalItemsBody.innerHTML = `
      <tr>
        <td colspan="4">No items found</td>
      </tr>
    `;
  } else {
    items.forEach(item => {
      const tr = document.createElement("tr");
      tr.innerHTML = `
        <td>${item.name}</td>
        <td>${item.quantity}</td>
        <td>${item.unitPrice}</td>
        <td>${item.priceTotal}</td>
      `;
      modalItemsBody.appendChild(tr);
    });
  }

  modal.classList.add("show");
}

closeModalBtn.addEventListener("click", () => {
  modal.classList.remove("show");
});

modal.addEventListener("click", (e) => {
  if (e.target === modal) {
    modal.classList.remove("show");
  }
});

async function loadOrders() {
  const token = localStorage.getItem("token");

  if (!customerId) {
    noDataEl.style.display = "block";
    infoEl.innerHTML = "";
    return;
  }

  infoEl.innerHTML = `<span class="muted">Loading orders...</span>`;

  try {
    const res = await fetch(
      `${API_BASE}/GetCustomerOrders/${customerId}`,
      {
        headers: {
          "Authorization": "Bearer " + token
        }
      }
    );

    if (!res.ok) throw new Error("No orders");

    const data = await res.json();

    if (!data.length) {
      noDataEl.style.display = "block";
      infoEl.innerHTML = "";
      return;
    }

    tableBody.innerHTML = "";
    infoEl.innerHTML = `<span class="muted">Showing ${data.length} order(s)</span>`;

    data.forEach(wrapper => {
      const o = wrapper.order;
      const items = wrapper.items;
      const status = statusInfoById(o.statusId);

      const tr = document.createElement("tr");

      const detailsBtn = document.createElement("button");
      detailsBtn.className = "link-btn";
      detailsBtn.textContent = "View";
      detailsBtn.addEventListener("click", () => {
        openOrderDetails(items);
      });

      tr.innerHTML = `
        <td>${o.orderId}</td>
        <td>
          <span class="status ${status.cls}">
            ${status.name}
          </span>
        </td>
        <td>${o.totalAmount}</td>
        <td>${o.reservationId ?? "-"}</td>
        <td>${new Date(o.createdAt).toLocaleString()}</td>
        <td>${remainingTime(o.expiryDate)}</td>
        <td></td>
      `;

      tr.children[6].appendChild(detailsBtn);
      tableBody.appendChild(tr);
    });

  } catch (err) {
    console.error(err);
    noDataEl.style.display = "block";
    infoEl.innerHTML = "";
  }
}

document.addEventListener("DOMContentLoaded", loadOrders);
