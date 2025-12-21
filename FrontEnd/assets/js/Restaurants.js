AOS.init({
  offset: 140,
});

document.addEventListener("DOMContentLoaded", function () {
  const loader = document.querySelector('.loader');
  if (loader) {
    setTimeout(() => {
      loader.style.opacity = '0';
      loader.style.display = 'none';
    }, 3000);
  }
});

const logoutBtn = document.getElementById("logout");

if (logoutBtn) {
  logoutBtn.onclick = function () {

    localStorage.removeItem("loggedIn");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("userId");
    localStorage.removeItem("token");

    window.location.href = "login.html";
  };
}

const getHamburgerIcon = document.getElementById("hamburger");
const getHamburgerCrossIcon = document.getElementById("hamburger-cross");
const getMobileMenu = document.getElementById("mobile-menu");

function closeMenu() {
  if (getMobileMenu) {
    getMobileMenu.style.transform = "translateX(-100%)";
  }
}

if (getHamburgerIcon && getMobileMenu) {
  getHamburgerIcon.addEventListener("click", function () {
    getMobileMenu.style.transform = "translateX(0%)";
  });
}

if (getHamburgerCrossIcon) {
  getHamburgerCrossIcon.addEventListener("click", closeMenu);
}

document.addEventListener("click", function (event) {
  if (!getMobileMenu || !getHamburgerIcon) return;

  const isClickInsideMenu = getMobileMenu.contains(event.target);
  const isClickOnIcon = getHamburgerIcon.contains(event.target);

  if (!isClickInsideMenu && !isClickOnIcon) {
    closeMenu();
  }
});

const searchBtn = document.getElementById("searchBtn");
const searchBtnMobile = document.getElementById("searchBtnMobile");
const closeBtn = document.getElementById("search-close-btn");
const searchCon = document.getElementById("search-container");

if (searchBtn && searchCon) {
  searchBtn.addEventListener("click", (e) => {
    e.preventDefault();
    searchCon.classList.remove("d-none");
    requestAnimationFrame(() => searchCon.classList.add("show"));
  });
}

if (searchBtnMobile && searchCon) {
  searchBtnMobile.addEventListener("click", (e) => {
    e.preventDefault();
    searchCon.classList.remove("d-none");
    requestAnimationFrame(() => searchCon.classList.add("show"));
  });
}

if (closeBtn && searchCon) {
  closeBtn.addEventListener("click", () => {
    searchCon.classList.remove("show");
    setTimeout(() => searchCon.classList.add("d-none"), 500);
  });
}

document.addEventListener('DOMContentLoaded', () => {
  const header = document.querySelector('header');
  const headerClass = document.querySelector('.header');
  if (!header || !headerClass) return;

  const checkScroll = () => {
    if (window.scrollY > 10) {
      header.classList.add('scrolled');
      headerClass.classList.replace('my-3', 'my-2');
      sessionStorage.setItem('scrolled', 'true');
    } else {
      header.classList.remove('scrolled');
      headerClass.classList.replace('my-2', 'my-3');
      sessionStorage.removeItem('scrolled');
    }
  };

  window.addEventListener('scroll', checkScroll);
  checkScroll();
});

document.addEventListener("DOMContentLoaded", () => {
  const reservationId = sessionStorage.getItem("reservationIdForOrder");
  const tableId = sessionStorage.getItem("tableIdForOrder");

  if (reservationId) {
    console.log("هذا الطلب مرتبط بالحجز رقم:", reservationId);
  }

  updateCartUI();

  [1, 2, 3, 4, 5].forEach(loadCategoryItems);
});

$('#our-menus').slick({
  slidesToShow: 1,
  slidesToScroll: 1,
  arrows: false,
  fade: true,
  speed: 300,
  asNavFor: '.slider-indicators-wrapper',
  draggable: false,
  swipe: false,
});

$('.slider-indicators-wrapper').slick({
  slidesToShow: 5,
  slidesToScroll: 1,
  asNavFor: '#our-menus',
  dots: false,
  arrows: true,
  focusOnSelect: true,
  draggable: false,
  swipe: false,
  prevArrow: '<button class="slide-arrow prev-arrow"><i class="fas fa-chevron-left"></i></button>',
  nextArrow: '<button class="slide-arrow next-arrow"><i class="fas fa-chevron-right"></i></button>',
  responsive: [
    { breakpoint: 991, settings: { slidesToShow: 5 } },
    { breakpoint: 990, settings: { slidesToShow: 1, arrows: true } }
  ]
});

$('#our-menus').on('beforeChange', function(event, slick, currentSlide, nextSlide) {
  var $nextSlide = $(slick.$slides[nextSlide]);
  $nextSlide.css({ 'transform': 'translateY(10%)', 'opacity': 0 });
  setTimeout(function() {
    $nextSlide.css({ 'transform': 'translateY(0)', 'opacity': 1, 'transition': 'transform 0.3s ease-in-out, opacity 0.3s ease-in-out' });
  }, 50); 
});

function loadCategoryItems(categoryId) {
  const token = localStorage.getItem("token");
  if (!token) return;

  fetch(`${API_BASE}/GetAllMenuItemsByCategoryId/${categoryId}`, { headers: { Authorization: `Bearer ${token}` } })
    .then(res => res.json())
    .then(data => renderItems(categoryId, data))
    .catch(err => console.error(err));
}

function renderItems(categoryId, items) {
  const container = document.getElementById(`category-${categoryId}`);
  if (!container) return;

  container.innerHTML = "";
  items.forEach(item => {
    container.innerHTML += `
      <div class="item-wrapper d-flex justify-content-between mb-3">
        <div class="item-left">
          <h5>${item.name}</h5>
          <p>${item.description ?? ""}</p>
        </div>
        <div class="item-right text-end">
          <span class="item-price">$${item.price}</span>
          <div class="item-btn">
            <button onclick="addToCart(${item.menuItemId}, '${item.name}', ${item.price})">
              Order
            </button>
          </div>
        </div>
      </div>
    `;
  });
}

function addToCart(id, name, price) {
  let cart = JSON.parse(localStorage.getItem("cart")) || [];
  const item = cart.find(i => i.menuItemId === id);
  if (item) item.quantity++;
  else cart.push({ menuItemId: id, name, price, quantity: 1 });

  localStorage.setItem("cart", JSON.stringify(cart));
  updateCartUI();
}

function updateCartUI() {
  const cart = JSON.parse(localStorage.getItem("cart")) || [];
  const container = document.getElementById("cart-items");
  const totalEl = document.getElementById("cart-total");
  const countEl = document.getElementById("cart-count");

  container.innerHTML = "";
  let total = 0;
  let count = 0;

  cart.forEach(item => {
    total += item.price * item.quantity;
    count += item.quantity;
    container.innerHTML += `
      <div class="d-flex justify-content-between align-items-center mb-2">
        <div>
          <strong>${item.name}</strong><br>
          <small>$${item.price} × ${item.quantity}</small>
        </div>
        <button class="btn btn-sm btn-danger" onclick="removeItem(${item.menuItemId})">✕</button>
      </div>
    `;
  });

  totalEl.textContent = total.toFixed(2);
  countEl.textContent = count;
}

function removeItem(id) {
  let cart = JSON.parse(localStorage.getItem("cart")) || [];
  cart = cart.filter(item => item.menuItemId !== id);
  localStorage.setItem("cart", JSON.stringify(cart));
  updateCartUI();
}

function clearCart() {
  if (!confirm("Clear all items?")) return;
  localStorage.removeItem("cart");
  updateCartUI();
}

function confirmOrder() {
  const token = localStorage.getItem("token");
  const cart = JSON.parse(localStorage.getItem("cart")) || [];

  if (!token || cart.length === 0) {
    alert("Cart is empty or user not logged in");
    return;
  }

  const orderPayload = {
    customerId: parseInt(localStorage.getItem("userId")),
    reservationId: sessionStorage.getItem("reservationIdForOrder") || null, 
    orderItems: cart.map(item => ({ menuItemId: item.menuItemId, quantity: item.quantity }))
  };

  fetch(`${API_BASE}/AddOrder`, {
    method: "POST",
    headers: { "Content-Type": "application/json", "Authorization": `Bearer ${token}` },
    body: JSON.stringify(orderPayload)
  })
    .then(res => {
      if (!res.ok) throw new Error("Order failed");
      return res.json();
    })
    .then(() => {
      localStorage.removeItem("cart");
      updateCartUI();
      alert("Order sent successfully ✅");

      sessionStorage.removeItem("reservationIdForOrder");
      sessionStorage.removeItem("tableIdForOrder");
    })
    .catch(err => {
      console.error(err);
      alert("Error sending order ❌");
    });
}


$('.testimonials .slider-content').slick({
  slidesToShow: 1,
  slidesToScroll: 1,
  arrows: false,
  fade: false,
  speed: 300,
  asNavFor: '.testimonials .slider-nav',
  draggable: true,
  swipe: true,
});

// Navigation Slider for Testimonials
$('.testimonials .slider-nav').slick({
  slidesToShow: 3,
  slidesToScroll: 1,
  asNavFor: '.testimonials .slider-content',
  dots: false,
  focusOnSelect: true,
  centerMode: true, 
  centerPadding: '0px',
  draggable: true,
  swipe: true,
  arrows: false, 
  infinite: true,
});

// Our Chefs Slider
$('.our-chefs .our-chef-slider-wrapper').slick({
  slidesToShow: 3,
  slidesToScroll: 1,
  arrows: true,
  focusOnSelect: true,
  centerMode: true, 
  centerPadding: '0px',
  fade: false,
  speed: 300,
  draggable: false,
  swipe: false,
  prevArrow: '<button class="slide-arrow prev-arrow"><i class="fas fa-chevron-left"></i></button>',
  nextArrow: '<button class="slide-arrow next-arrow"><i class="fas fa-chevron-right"></i></button>',
  responsive: [
    {
      breakpoint: 990,
      settings: {
        slidesToShow: 1,
      }
    }
  ]
});

$('.story-content').slick({
  slidesToShow: 1,
  slidesToScroll: 1,
  arrows: false,
  fade: false,
  speed: 300,
  asNavFor: '.story-indicators .row',
  draggable: true,
  swipe: true,
});

$('.story-indicators > .row').slick({
  slidesToShow: 6,
  slidesToScroll: 1,
  asNavFor: '.story-content',
  dots: false,
  focusOnSelect: true,
  centerPadding: '0px',
  draggable: true,
  swipe: true,
  arrows: false,
  infinite: true,
  prevArrow: '<button class="slide-arrow prev-arrow"><i class="fas fa-chevron-left"></i></button>',
  nextArrow: '<button class="slide-arrow next-arrow"><i class="fas fa-chevron-right"></i></button>',
  responsive: [
    {
      breakpoint: 768,
      settings: {
        slidesToShow: 2,
      }
    }
  ]
});

$('.partner-slider').slick({
  slidesToShow: 6,
  slidesToScroll: 1,
  arrows: false,
  fade: false,
  speed: 300,
  draggable: true,
  swipe: true,
  responsive: [
    {
      breakpoint: 1200,
      settings: {
        slidesToShow: 4,
        slidesToScroll: 1,
      }
    },
    {
      breakpoint: 768,
      settings: {
        slidesToShow: 3,
        slidesToScroll: 1,
      }
    },
    {
      breakpoint: 480,
      settings: {
        slidesToShow: 1,
        slidesToScroll: 1,
      }
    }
  ]
});


$('.chef-choise-slider').slick({
  slidesToShow: 3,
  vertical: true,
  slidesToScroll: 1,
  arrows: false,
  fade: false,
  speed: 300,
  draggable: true,
  swipe: true,
  responsive: [
    {
      breakpoint: 786,
      settings: {
        slidesToShow: 1.7,
        slidesToScroll: 1,
      }
    }
  ]
});


const reserveBtn = document.getElementById("reserveBtn");

if (reserveBtn) {
  reserveBtn.addEventListener("click", async function () {

    const phone = document.getElementById("phone")?.value.trim();
    const notes = document.getElementById("Note")?.value.trim();
    const date = document.getElementById("date")?.value;
    const time = document.getElementById("time")?.value;


    const customerId = parseInt(localStorage.getItem("userId"));
    const token = localStorage.getItem("token");

    if (!customerId || !token || !phone || !date || !time) {
      alert("❌ Please check your data");
      return;
    }

    const reservationDate = new Date(`${date} ${time}`);
    reservationDate.setHours(reservationDate.getHours() + 3);
    if (isNaN(reservationDate)) return;

    try {
      const response = await fetch(`${API_BASE}/AddNewReservation`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer " + token
        },
        body: JSON.stringify({
          customerId,
          reservationDate,
          notes,
          phone
        })
      });

      if (!response.ok) throw new Error();
      alert("✔ تمت عملية الحجز بنجاح!");
    } catch {
      alert("✖ Error sending reservation.");
    }
  });
}

const copyright =
  document.getElementById('copyrightCurrentYear');

if (copyright) {
  copyright.textContent = new Date().getFullYear();
}
console.log()