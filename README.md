# LibriVerse - Online Digital Library 📚💻

## Description

LibriVerse is a web application built with ASP.NET Core MVC that functions as an online digital library. It allows a single administrator to manage a curated collection of digital books and grant timed access (digital loans) to registered users (Patrons). Patrons can browse the catalog, borrow books, and access their loaned content directly through the site.

This project was developed primarily as a learning exercise for ASP.NET Core concepts.

---

## Features ✨

* **Admin Management:**
    * Secure Admin Dashboard with key library statistics (Total Books, Active Loans, Patron Count, etc.).
    * Full CRUD (Create, Read, Update, Delete) operations for Books, including metadata (Description, Year, Genre) and cover image uploads.
    * User management interface (view users, manage roles).
    * View all active loans and full loan history across all users.
    * Ability to manually end a user's digital access.
* **Patron/User Features:**
    * Secure user registration and login using ASP.NET Core Identity.
    * Public-facing Catalog for browsing books (with search functionality).
    * Detailed view for each book including cover, description, and metadata.
    * Self-service digital checkout (borrowing) of books with a fixed loan duration (e.g., 14 days).
    * "My Loans" page showing currently active digital loans.
    * Simulated "Read" page to access borrowed digital content (access controlled by loan status).
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

## Setup and Installation 🚀

1.  **Clone the Repository:**
    ```bash
    git clone <your-repository-url>
    cd LibriVerse
    ```
2.  **Database Connection:**
    * Ensure you have SQL Server (or SQL Server Express) installed.
    * Update the connection string in `appsettings.json`. Find the `"ConnectionStrings"` section and modify the `"LibraryConnection"` (or similar name) value to point to your local SQL Server instance and desired database name.
        ```json
        "ConnectionStrings": {
          "LibraryConnection": "Server=(localdb)\\mssqllocaldb;Database=LibriVerseDB;Trusted_Connection=True;MultipleActiveResultSets=true" 
        }
        ```
3.  **Restore Dependencies:**
    ```bash
    dotnet restore
    ```
4.  **Apply Migrations:** This will create the database schema based on the Entity Framework Core migrations.
    ```bash
    dotnet ef database update
    ```
5.  **Run the Application:**
    ```bash
    dotnet run
    ```
    Alternatively, run the project from your IDE (like Visual Studio).

---

## Usage Guide 📖

1.  **Admin Access:**
    * The application seeds a default Admin user on first run.
    * **Email:** `admin@library.com`
    * **Password:** `AdminPassword123!` (or whatever you set in `DbInitializer.cs`).
    * Log in as the Admin to access the Admin Dashboard and management sections (Books, Users, Admin Loans).
2.  **Patron Access:**
    * Navigate to the **/Register** page to create a standard user account.
    * Log in as a Patron. You will be redirected to the **Catalog**.
    * Browse books, view details, and use the **Borrow Digital Copy** button.
    * Access borrowed books via the **My Loans** link in the navigation bar.

---

*(Optional: Add License section if needed)*