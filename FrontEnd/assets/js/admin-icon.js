document.addEventListener("DOMContentLoaded", () => {
    const adminBtn = document.getElementById("adminDashboardBtn");
    if (!adminBtn) return;

    const role = localStorage.getItem("role");
    console.log("ROLE =", role);

    if (role === "Admin") {
        adminBtn.classList.remove("d-none");
        adminBtn.style.display = "inline-block";
    }
});



