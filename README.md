<div align="center">

# 🌙 Lunamaro

### Full Stack Restaurant Management & Online Ordering System

**Angular** · **ASP.NET Core Web API** · **SQL Server** · **Stripe** · **JWT**

[![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)](https://angular.io)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Stripe](https://img.shields.io/badge/Stripe-635BFF?style=for-the-badge&logo=stripe&logoColor=white)](https://stripe.com)
[![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)](https://jwt.io)

### [Test it Now → lunamaro.runasp.net](https://lunamaro.runasp.net)

</div>

---

## 📖 Overview

Lunamaro is a **production-ready restaurant management and online ordering platform** built with modern full-stack technologies. It serves two distinct audiences:

- **Customers** who want a smooth, end-to-end dining experience — from browsing the menu to receiving a reservation confirmation in their inbox.
- **Administrators** who need powerful, real-time tools to manage daily restaurant operations without friction.

The system is engineered around **reliability and automation**. Background services continuously monitor order states and dispatch email notifications, so neither staff nor customers have to chase updates manually. Payments are handled live through **Stripe**, with a full checkout and webhook flow, while **Pay on Delivery** ensures flexibility for all customer preferences.

---

## ✨ Features

### 👤 Customer Features

| Feature | Description |
|--------|-------------|
| 🍽️ Menu Browsing | Browse items by category with images, descriptions, and prices |
| 🔍 Search & Filter | Quickly find menu items using search and category filters |
| 🛒 Cart Management | Add, remove, and adjust item quantities before checkout |
| 💳 Stripe Payment | Live card payments using Stripe with full webhook support |
| 🚚 Pay on Delivery | Order and pay when the food arrives |
| 📅 Table Reservations | Select a date, time, and party size to book a table |
| 📧 Email Notifications | Auto-emails for order confirmations and reservation updates |
| 🔐 Secure Login | JWT-based authentication with token refresh |

### 🛠️ Admin Features

| Feature | Description |
|--------|-------------|
| 🧾 Menu Management | Add, edit, delete menu items and upload images |
| 📦 Order Tracking | View and update order statuses in real time |
| 📋 Reservation Control | Approve, modify, or cancel table reservations |
| 💰 Payment Settings | Toggle Stripe and Pay on Delivery availability site-wide |
| 🔐 Admin Auth | Role-based access with JWT tokens |

---

## ⚙️ Background Services

Two dedicated **ASP.NET Core Hosted Services** run continuously in the background:

### 🔄 Order Processing Service
- Monitors the order queue on a configurable schedule
- Automatically transitions orders from **Pending → Confirmed** after validation
- Enforces timeout rules — auto-cancels orders not acted on within a set window
- Logs all automated transitions for audit and debugging

### 📨 Email Dispatch Service
- Decouples email sending from the HTTP request/response cycle — API responses remain fast regardless of email status
- Processes an internal email queue populated by order and reservation events
- Renders **HTML email templates** with dynamic order/reservation data
- Includes retry logic for transient SMTP failures
- Gracefully flushes the queue before host shutdown to avoid lost notifications

---

## 💳 Live Stripe Payment Flow

```
Customer selects "Pay Online"
        ↓
API creates a Stripe Payment Intent (server-side)
        ↓
Stripe.js collects card details securely in the browser
        ↓
Payment confirmed client-side (no raw card data on your server)
        ↓
Stripe fires a webhook → API receives payment_intent.succeeded
        ↓
Order marked as Paid · Confirmation email queued
```

> **Webhook events handled:** `payment_intent.succeeded`, `payment_intent.payment_failed`

---

## 📧 Email Notification Triggers

| Trigger | Recipient | Content |
|---------|-----------|---------|
| Order placed | Customer | Itemized receipt + estimated handling time |
| Order status updated | Customer | New status (Preparing / Out for Delivery / Delivered) |
| Reservation created | Customer | Booking summary with date, time, and table details |
| Reservation status changed | Customer | Approval, modification, or cancellation notice |

---

## 🏗️ Architecture

### Backend — ASP.NET Core Web API
```
Controllers  →  Services  →  Repositories  →  SQL Server (EF Core)
                   ↑
          Background Services (Hosted Services)
                   ↑
             Stripe Webhooks · SMTP
```

- **Layered architecture** — Controllers handle HTTP; Services own business logic; Repositories abstract data access
- **Entity Framework Core** with code-first migrations
- **AutoMapper** for clean DTO ↔ domain model mapping
- **FluentValidation** for structured request validation
- **Stripe SDK** integrated as a scoped service with webhook signature verification

### Frontend — Angular SPA
```
AppModule
├── AuthModule        (Login, Register, JWT interceptor)
├── MenuModule        (Browse, Search, Filter)
├── CartModule        (Cart state, Checkout)
├── OrdersModule      (Order history, Status tracking)
├── ReservationsModule(Date picker, Booking form)
└── AdminModule       (Menu CRUD, Orders, Reservations, Settings)
```

- **Route guards** — `AuthGuard` and `RoleGuard` protect customer and admin routes
- **HTTP Interceptor** — auto-attaches JWT and handles 401 globally
- **Reactive Forms** — client-side validation before any API call
- **Service layer** — all API calls abstracted; components stay thin and presentational

---

## 🔐 Authentication & Security

- **JWT-based auth** — stateless, signed tokens issued on login containing identity and role claims
- **Role-based access control** — `Customer` and `Admin` roles enforced at the API layer with `[Authorize(Roles = "Admin")]`
- **Ownership checks** — customers can only access their own orders and reservations
- **Token expiry** — enforced server-side; expired tokens return `401` and redirect the client to login

---

## 🗂️ End-to-End Flow

```
Register / Login  →  JWT issued  →  Stored in Angular  →  Attached to every request
       ↓
Browse Menu  →  Add to Cart  →  Checkout
       ↓                             ↓                    ↓
  (browsing)             Stripe Payment          Pay on Delivery
                              ↓                        ↓
                   Webhook → Order marked paid    Order confirmed
                              ↓
                   Background Email Service  →  Confirmation email sent
                              ↓
                   Admin updates order status  →  Status email sent to customer

Table Reservation  →  API creates reservation  →  Confirmation email sent
       ↓
Admin approves / declines  →  Status email sent to customer
```

---

## 🛠️ Technology Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular (SPA, Reactive Forms, HTTP Interceptors) |
| Backend | ASP.NET Core Web API (REST, Layered Architecture) |
| Database | SQL Server + Entity Framework Core (Code-First) |
| Authentication | JWT — role-based, stateless |
| Payments | Stripe API (Payment Intents + Webhooks) |
| Email | SMTP (HTML templates via Background Service) |
| Background Jobs | ASP.NET Core `IHostedService` |

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/mohamedelhenawy-3/Lunamaro.git
cd Lunamaro
```

### 2. Configure the backend

Open `appsettings.json` and fill in your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your SQL Server connection string"
  },
  "JwtSettings": {
    "Secret": "your-secret-key",
    "ExpiryMinutes": 60
  },
  "Stripe": {
    "SecretKey": "sk_live_...",
    "WebhookSecret": "whsec_..."
  }
}
```

### 3. Apply database migrations

```bash
cd LunamaroAPI
dotnet ef database update
```

### 4. Run the backend

```bash
dotnet run
```

### 5. Run the frontend

```bash
cd lunamaro-angular
npm install
ng serve
```

The app will be available at `http://localhost:4200`

---

## 📁 Project Structure

```
Lunamaro/
├── LunamaroAPI/                  # ASP.NET Core Web API
│   ├── Controllers/              # API endpoints
│   ├── Services/                 # Business logic
│   ├── Repositories/             # Data access layer
│   ├── Models/                   # Domain models
│   ├── DTOs/                     # Data transfer objects
│   ├── BackgroundServices/       # Order & Email hosted services
│   └── Migrations/               # EF Core migrations
│
└── lunamaro-angular/             # Angular SPA
    ├── src/app/
    │   ├── auth/                 # Login, Register
    │   ├── menu/                 # Browse, Search
    │   ├── cart/                 # Cart, Checkout
    │   ├── orders/               # Order history
    │   ├── reservations/         # Table booking
    │   └── admin/                # Admin dashboard
    └── environments/             # API base URL config
```

---



This project is open source and available under the [MIT License](LICENSE).

---

<div align="center">


</div>
