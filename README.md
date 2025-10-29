# LibriVerse - Online Digital Library 📚💻

**Landing Page:**
![LibriVerse Landing Page](docs/images/landing2.png "LibriVerse Home")
![LibriVerse Landing Page](docs/images/landing-page.png "LibriVerse Home")

---
## Description

LibriVerse is a web application built with ASP.NET Core MVC that functions as an online digital library. It allows a single administrator to manage a curated collection of digital books and grant timed access (digital loans) to registered users (Patrons). Patrons can browse the catalog, borrow books, and access their loaned content directly through the site.

This project was developed primarily as a learning exercise for ASP.NET Core concepts.

---

**Sample Screen:**
![LibriVerse Landing Page](docs/images/oliver.png "SampleScreen")

---

## Features ✨

* **Admin Management:**
    * Secure Admin Dashboard with key library statistics (Total Books, Active Loans, Patron Count, etc.).
    * Full CRUD (Create, Read, Update, Delete) operations for Books, including metadata (Description, Year, Genre) and cover image uploads.
    * User management interface (view users, manage roles).
    * View all active loans and full loan history across all users.
    * Ability to manually end a user's digital access.


**Admin Dashboard:**
![LibriVerse Admin Dashboard](docs/images/admin-dashboard.png "Admin Control Panel")

---
* **Patron/User Features:**
    * Secure user registration and login using ASP.NET Core Identity.
    * Public-facing Catalog for browsing books (with search functionality).
    * Detailed view for each book including cover, description, and metadata.
    * Self-service digital checkout (borrowing) of books with a fixed loan duration (e.g., 14 days).
    * "My Loans" page showing currently active digital loans.
    * Simulated "Read" page to access borrowed digital content (access controlled by loan status).

**Book Catalog (Card View):**
![LibriVerse Catalog View](docs/images/catalog-view.png "Public Book Catalog")
![LibriVerse Catalog View](docs/images/catalog2.png "Public Book Catalog")

---



* **Security:**
    * Role-based authorization restricting administrative sections to the "Admin" role.
    * Authentication handled by ASP.NET Core Identity.
* **UI:**
    * Custom theme applied globally using CSS variables.
    * Responsive design using Bootstrap.
    * Separate, themed full-screen pages for Login and Registration.

---

## Technologies Used 🛠️

* **Backend:** ASP.NET Core MVC (.NET 9.0)
* **Database:** Entity Framework Core, SQL Server
* **Authentication:** ASP.NET Core Identity (Users, Roles)
* **Frontend:** Razor Views, HTML, CSS, Bootstrap 5, Font Awesome
* **Language:** C#

---
