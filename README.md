# Iskolar Alert: PUP Incident Report & Lost and Found

A centralized web application for reporting campus incidents and managing lost-and-found items at the Polytechnic University of the Philippines (PUP).

## 📋 Overview

Iskolar Alert is an integrated platform that addresses the lack of a unified digital system for campus safety reporting and lost property management at PUP. The system provides students and administrators with streamlined tools for incident reporting, status tracking, and item recovery coordination.

## ✨ Key Features

### For Students
- **Incident Reporting**: Submit detailed incident reports with descriptions, locations, and photo evidence
- **Status Tracking**: Monitor report progress through structured workflow (Pending → Accepted → In-Progress → Resolved)
- **Lost & Found**: Report and browse lost or found items with search and filter capabilities
- **Notifications**: Receive real-time updates via in-app and email notifications
- **Feedback System**: Rate and provide feedback on resolved incidents

### For Administrators
- **Report Management**: Review, triage, and update incident statuses
- **Staff Assignment**: Assign security personnel to specific cases
- **Oversight Dashboard**: Monitor all system activity and user submissions
- **Lost & Found Administration**: Ensure content compliance and manage archived items
- **Analytics**: View feedback ratings and service quality metrics

## 🏗️ Technology Stack

- **Framework**: ASP.NET Core Web App (MVC)
- **Backend**: C#
- **Database**: Azure SQL Server with Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **Authentication**: Cookie-based authentication with PUP Webmail integration
- **Security**: BCrypt password hashing
- **Version Control**: Git

## 📦 Dependencies

```xml
- BCrypt.Net-Next (4.0.3)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.11)
- Microsoft.EntityFrameworkCore.Tools (8.0.11)
- System.Data.SqlClient (4.9.0)
- System.Data.Common (4.3.0)
```

## 🚀 Getting Started

### Prerequisites

- **Operating System**: Windows 10/11 (preferred) or macOS
- **IDE**: Visual Studio 2022 with ASP.NET and web development workload
- **.NET SDK**: Version 8.0 or later
- **Git**: Latest version
- **Hardware**: 
  - CPU: 1.8 GHz or faster 64-bit processor
  - RAM: 8 GB minimum (16 GB recommended)
  - Storage: 20 GB free disk space
- **Browser**: Latest version of Edge, Chrome, or Firefox
- **Internet**: Required for Azure SQL connectivity

### Installation

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd IskoAlert_WebApp
   ```

2. **Open Solution**
   - Double-click `IskoAlert_WebApp.sln` to open in Visual Studio

3. **Restore NuGet Packages**
   - Right-click the Solution in Solution Explorer
   - Select "Restore NuGet Packages"

4. **Configure Database**
   - Open `appsettings.json`
   - Update the `DefaultConnection` string with your Azure SQL credentials:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server.database.windows.net;Database=your-database;User Id=your-username;Password=your-password;"
     }
     ```

5. **Run Migrations**
   - Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)
   - Execute:
     ```powershell
     Add-Migration InitialCreate
     Update-Database
     ```

6. **Build and Run**
   - Press `Ctrl + Shift + B` to build
   - Press `F5` to run the application
   - The browser should automatically open to the login page

## 🔐 Authentication

### Student Registration
- **Email Format**: Must match `^[a-zA-Z0-9._%+-]+@iskolarngbayan\.pup\.edu\.ph$`
- **Student ID Format**: `YYYY-XXXXX-MN-0`
- Passwords are hashed using BCrypt before storage

### Role-Based Access
- **Students**: Redirected to `Home/Index` after login
- **Admins**: Redirected to `Admin/Index` after login
- Only accounts with `AccountStatus == Active` can log in

## 📱 Core Modules

### Incident Reporting Module
- Submit incident reports with type, location, description, and photos
- Automatic assignment of "Pending" status
- Structured status workflow management
- Feedback submission for resolved incidents

### Lost and Found Module
- Report lost or found items with detailed descriptions
- Browse and search active listings
- 30-day archive rule (items auto-archived after 30 days)
- Coordinate recovery via university webmail

## 👥 Target Users

- **Students**: Submit reports, track statuses, manage lost/found items
- **Administrators**: Manage reports, update statuses, assign staff, oversee system activity

## 🔔 Notifications

The system provides:
- In-app notifications for status updates
- Email notifications for important events
- Automatic alerts for potential lost-and-found matches

## 🛡️ Security Features

- Cookie-based authentication
- BCrypt password encryption
- PUP Webmail credential validation
- Role-based access control (RBAC)
- HTTPS enforcement for all communications

## 📊 Project Structure

```
IskoAlert_WebApp/
├── Controllers/        # MVC Controllers
├── Models/            # Data models and entities
├── Views/             # Razor views
├── Services/          # Business logic and services
├── Data/              # Database context and migrations
├── wwwroot/           # Static files (CSS, JS, images)
└── appsettings.json   # Configuration file
```

## 🎓 Academic Context

This project was developed as partial fulfillment for:
- **Course**: COMP 019 - Application Development and Emerging Technologies
- **Program**: BSCS 3-1
- **Institution**: Polytechnic University of the Philippines, College of Computer and Information Sciences

## 👨‍💻 Development Team

- Lasam, Vince Michael
- Mercado, Jeff Petterson
- Nicolas, John Rich M.
- Paredes, Lian

## 📄 License

This project is developed for academic purposes at the Polytechnic University of the Philippines.

## 🤝 Contributing

This is an academic project. For questions or suggestions, please contact the development team through official PUP channels.

## 📞 Support

For technical issues or questions:
- Contact the development team via PUP Webmail
- Refer to the comprehensive User Guide in the project documentation

---

**Note**: This application is intended for use within the PUP campus community and requires valid PUP Webmail credentials for access.
