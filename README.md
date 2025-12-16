# Iskolar Alert: PUP Incident Reporting & Lost and Found App

**IskoAlert** is a centralized campus safety and community service platform designed for the **Polytechnic University of the Philippines (PUP)**. This repository contains the **Incident Reporting** and **Lost & Found** modules, built using **C# ASP.NET Core MVC**.

[cite_start]These modules are designed to integrate into the main *IskoAlert* ecosystem, utilizing shared authentication and database infrastructure to enhance campus security and facilitate item recovery[cite: 7, 8].

---

## 📋 Table of Contents
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [The Team](#-the-team)

---

## 🚀 Features

### 1. Incident Reporting Module
[cite_start]Allows students and faculty to report campus emergencies or safety concerns directly to the administration[cite: 11].
* [cite_start]**Submit Reports:** Users can provide detailed descriptions, categorize incidents (e.g., hazard, theft), specify campus location, and upload photo evidence[cite: 12].
* [cite_start]**Status Tracking:** Reporters can track the progress of their reports via a personal dashboard (Pending → Accepted → In-Progress → Resolved)[cite: 13, 102].
* [cite_start]**Admin Dashboard:** Security personnel can filter reports, update statuses, and assign staff to respond to specific incidents[cite: 13, 98].

### 2. Lost and Found Module
[cite_start]A centralized platform for reporting and recovering lost items within the campus[cite: 14].
* [cite_start]**Post Listings:** Users can report lost or found items with photos, descriptions, and last-known locations[cite: 15, 106].
* [cite_start]**Browse & Search:** A searchable database of items that helps match owners with found belongings[cite: 16].
* [cite_start]**Auto-Expiration:** Listings automatically expire after 30 days to keep the database current[cite: 15].
* [cite_start]**Secure Contact:** Facilitates communication between parties using the official PUP Webmail system[cite: 16].

### 3. Integration
* [cite_start]**Authentication:** Integrates with the main Iskolar Alert app, requiring valid **PUP Webmail** credentials for access[cite: 9].
* [cite_start]**Notifications:** Sends alerts via email and in-app dashboards when report statuses change or matches are found[cite: 18, 19].

---

## 🛠 Tech Stack
* [cite_start]**Framework:** ASP.NET Core Web App (Model-View-Controller) [cite: 7]
* **Language:** C#
* [cite_start]**Database:** SQL Server (via Entity Framework Core) [cite: 24, 293]
* **Frontend:** HTML5, CSS3, JavaScript (Bootstrap)
* **Tools:** Visual Studio, NuGet Package Manager, Git

---

## ⚙️ Prerequisites
Before running this project, ensure you have the following installed:
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (with "ASP.NET and web development" workload)
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
* SQL Server (LocalDB or Express)
