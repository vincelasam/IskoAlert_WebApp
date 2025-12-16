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
Allows students and faculty to report campus emergencies or safety concerns directly to the administration.
 **Submit Reports:** Users can provide detailed descriptions, categorize incidents (e.g., hazard, theft), specify campus location, and upload photo evidence.
 **Status Tracking:** Reporters can track the progress of their reports via a personal dashboard (Pending → Accepted → In-Progress → Resolved).
 **Admin Dashboard:** Security personnel can filter reports, update statuses, and assign staff to respond to specific incidents.

### 2. Lost and Found Module
[cite_start]A centralized platform for reporting and recovering lost items within the campus.
 **Post Listings:** Users can report lost or found items with photos, descriptions, and last-known locations.
 **Browse & Search:** A searchable database of items that helps match owners with found belongings.
 **Auto-Expiration:** Listings automatically expire after 30 days to keep the database current.
 **Secure Contact:** Facilitates communication between parties using the official PUP Webmail system.

### 3. Integration
**Authentication:** Integrates with the main Iskolar Alert app, requiring valid **PUP Webmail** credentials for access.
**Notifications:** Sends alerts via email and in-app dashboards when report statuses change or matches are found.

---

## 🛠 Tech Stack
**Framework:** ASP.NET Core Web App (Model-View-Controller) 
**Database:** SQL Server (via Entity Framework Core) 
**Frontend:** HTML5, CSS3, JavaScript (Bootstrap)
 **Tools:** Visual Studio, NuGet Package Manager, Git

---

## ⚙️ Prerequisites
Before running this project, ensure you have the following installed:
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (with "ASP.NET and web development" workload)
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download) (or later)
* SQL Server (LocalDB or Express)
