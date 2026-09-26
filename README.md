# 🍽️ WebRestaurant - Restaurant Management API

A robust backend Web API built for restaurant operations management, implementing a clean **3-Tier Architecture** to ensure high maintainability, separation of concerns, and scalable data handling.

## 🚀 Features

* 📋 **Menu & Category Management:** Organize restaurant items, meals, and categories dynamically.
* 🛒 **Order Processing:** Handle customer orders and individual order items efficiently.
* 📅 **Table Reservations:** Manage restaurant booking and reservation schedules.
* 👥 **User Management:** Handle user accounts and authentication.
* 🏛️ **Clean 3-Tier Architecture:** Clear separation between API endpoints, business logic, and data access layers.

---

## 🛠️ Tech Stack

* **Language:** C#
* **Framework:** ASP.NET Core Web API
* **Architecture:** 3-Tier (Presentation/API, Business Layer, Data Access Layer)
* **Database & ORM:** SQL Server, ADO.net

---

## 📂 Project Structure

```text
WebRestaurant/
│
├── BackEnd/
│   ├── BusinessLayer/       # Business logic, services, and domain models
│   ├── DataAccessLayer/     # Data repositories, context, and database operations
│   └── RestaurantWebApi/    # API Controllers, program configuration, and endpoints
