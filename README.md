# Cumulative-Project
## Teachers MVP - ASP.NET Core Web API & MVC

## Overview
This project is a **Minimum Viable Product (MVP)** that focuses on the **Teachers** table of the provided **School Database**. It is built using **ASP.NET Core Web API and MVC** with **MySQL** as the database. The project implements only the **READ** functionality.

## Features
- **ASP.NET Core Web API** for fetching teacher data.
- **MVC Pages** for listing and displaying teacher details.
- **MySQL Integration** using `MySql.Data.MySqlClient`.
- **Structured Code** with separate **Controllers, Models, and Views**.

## Project Structure
```
📂 Cumulative 1
 ┣ 📂 Controllers
 ┃ ┣ 📜 TeacherAPIController.cs   # API Controller for fetching teacher data
 ┃ ┗ 📜 TeacherPageController.cs  # MVC Controller for rendering teacher views
 ┣ 📂 Models
 ┃ ┣ 📜 Teacher.cs         # Model representing Teacher data
 ┃ ┗ 📜 SchoolDbContext.cs # Database connection class
 ┣ 📂 Views
 ┃ ┗ 📂 TeacherPage
 ┃   ┣ 📜 List.cshtml  # Displays list of teachers
 ┃   ┗ 📜 Show.cshtml  # Displays a specific teacher
 ┗ 📜 README.md
```

## Setup & Installation
### Prerequisites
- **Visual Studio 2022**
- **.NET 6+**
- **XAMPP** (MySQL Server)

### Steps
1. Clone the repository:
   ```sh
   git clone https://github.com/yaoolee/Cumulative-Project.git
   ```
2. Open **XAMPP** and start the **MySQL** server.
3. Import the `schooldb.sql` file into **phpMyAdmin**.
4. Update `SchoolDbContext.cs` with the correct MySQL credentials.
5. Run the project in **Visual Studio 2022**.
6. Navigate to:
   - **API Endpoint:** `http://localhost:5000/api/TeacherAPI/ListTeachers`
   - **MVC List Page:** `http://localhost:5000/TeacherPage/List`
   - **MVC Show Page:** `http://localhost:5000/TeacherPage/Show/{id}`

## API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| **GET** | `/api/TeacherAPI/ListTeachers` | Fetch all teachers |
| **GET** | `/api/TeacherAPI/FindTeacher/{id}` | Fetch a teacher by ID |

## Future Enhancements
- Add **Create, Update, and Delete (CRUD)** operations.
- Improve **styling and UI enhancements**.
  



