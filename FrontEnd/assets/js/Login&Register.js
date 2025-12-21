document.addEventListener("DOMContentLoaded", function () {

  const LOGIN_ENDPOINT = `${API_BASE}/auth/login`;
  const REGISTER_ENDPOINT = `${API_BASE}/auth/register`;

  function showAlert(message) { alert(message); }

  function setLoading(button, isLoading) {
    if (!button) return;
    button.disabled = isLoading;
    if (isLoading) {
      button.dataset.origText = button.innerHTML;
      button.innerHTML = "Please wait...";
    } else {
      if (button.dataset.origText) button.innerHTML = button.dataset.origText;
    }
  }

  // JWT parser
  function parseJwt(token) {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
    } catch {
      return null;
    }
  }

  async function fetchWithTimeout(url, options = {}, timeoutMs = 10000) {
    const controller = new AbortController();
    const id = setTimeout(() => controller.abort(), timeoutMs);
    try {
      const res = await fetch(url, { ...options, signal: controller.signal });
      clearTimeout(id);
      return res;
    } catch (err) {
      clearTimeout(id);
      throw err;
    }
  }

  const loginForm = document.getElementById("resto-auth-login-form");
  const loginEmail = document.getElementById("resto-auth-email");
  const loginPassword = document.getElementById("resto-auth-password");
  const loginBtn = loginForm ? loginForm.querySelector('button[type="submit"]') : null;

  if (loginForm && loginEmail && loginPassword) {

    if (localStorage.getItem("loggedIn") === "true") {
      window.location.href = "index.html";
      return;
    }

    loginForm.addEventListener("submit", async function (e) {
      e.preventDefault();

      const email = loginEmail.value.trim();
      const password = loginPassword.value;

      if (!email || !password) {
        showAlert("Please enter both email and password.");
        return;
      }

      setLoading(loginBtn, true);

      try {
        const res = await fetchWithTimeout(LOGIN_ENDPOINT, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
          },
          body: JSON.stringify({ Email: email, Password: password })
        });

        if (!res.ok) {
          let errText = `Login failed (status ${res.status})`;
          try {
            const errJson = await res.json();
            if (errJson.message) errText = errJson.message;
          } catch (_) { }
          throw new Error(errText);
        }

        const data = await res.json();

        const token = data?.token ?? data?.accessToken ?? data?.Token ?? null;
        if (token) {
          localStorage.setItem("token", token);

          if (token) {
  const payload = parseJwt(token);

  const role =
    payload?.role ||
    payload?.Role ||
    payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

  if (role) {
    localStorage.setItem("role", role);
  }
}
          const payload = parseJwt(token);
          console.log("JWT Payload:", payload);

          const role =
            payload?.role ||
            payload?.Role ||
            payload?.roles ||
            payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

          if (role) {
            localStorage.setItem("role", role);
          }
        }

        let userId = data?.user?.UserId ?? data?.user?.id ?? data?.UserId ?? null;
        if (!userId && token) {
          const payload = parseJwt(token);
          userId = payload?.UserID ?? payload?.userId ?? payload?.sub ?? null;
        }

        localStorage.setItem("loggedIn", "true");
        if (userId) localStorage.setItem("userId", String(userId));
        if (data?.user?.email) localStorage.setItem("userEmail", data.user.email);
        else localStorage.setItem("userEmail", email);

        console.log("Login success:", {
          userId,
          role: localStorage.getItem("role"),
          hasToken: !!token
        });

        window.location.href = "index.html";

      } catch (err) {
        console.error("Login error:", err);
        if (err.name === 'AbortError') showAlert("Request timed out.");
        else showAlert(err.message || "Login failed.");
      } finally {
        setLoading(loginBtn, false);
      }
    });
  }

  const adminBtn = document.getElementById("adminDashboardBtn");
  const userRole = localStorage.getItem("role");

  console.log("Stored role:", userRole);

  if (userRole === "Admin" && adminBtn) {
    adminBtn.classList.remove("d-none");
  }

  document.getElementById("logoutBtn")?.addEventListener("click", () => {
    localStorage.clear();
    window.location.href = "index.html";
  });

  const registerForm = document.getElementById("resto-auth-register-form");
  const registerName = document.getElementById("resto-auth-name");
  const registerEmail = document.getElementById("resto-auth-email");
  const registerPassword = document.getElementById("resto-auth-password");
  const registerConfirm = document.getElementById("resto-auth-confirm");
  const registerBtn = registerForm ? registerForm.querySelector('button[type="submit"]') : null;

  if (registerForm && registerName && registerEmail && registerPassword && registerConfirm) {

    registerForm.addEventListener("submit", async function (e) {
      e.preventDefault();

      const fullName = registerName.value.trim();
      const email = registerEmail.value.trim();
      const password = registerPassword.value;
      const confirm = registerConfirm.value;

      if (!fullName || !email || !password || !confirm) {
        alert("Please fill all required fields.");
        return;
      }

      if (password.length < 2) {
        alert("Password must be at least 2 characters.");
        return;
      }

      if (password !== confirm) {
        alert("Passwords do not match!");
        return;
      }

      setLoading(registerBtn, true);

      try {
        const res = await fetch(REGISTER_ENDPOINT, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
          },
          body: JSON.stringify({ FullName: fullName, Email: email, Password: password })
        });

        const data = await res.json().catch(() => null);

        if (!res.ok) {
          let msg = `Register failed (status ${res.status})`;
          if (data && (data.message || data.Message)) msg = data.message || data.Message;
          throw new Error(msg);
        }

        //localStorage.setItem("loggedIn", "true");
        //localStorage.setItem("userEmail", data?.Email || email);
        alert("Registration successful. Redirecting...");
        window.location.href = "Login.html";

      } catch (err) {
        console.error("Register error:", err);
        alert(err.message || "Registration failed.");
      } finally {
        setLoading(registerBtn, false);
      }
    });
  }

});
