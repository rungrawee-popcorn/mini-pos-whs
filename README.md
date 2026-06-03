# Mini POS & Warehouse System (WHS)

## 📌 Overview

ระบบ POS (Point of Sale) และ Warehouse Management System (WHS) สำหรับจัดการสินค้า การขาย และสต็อก พร้อม Dashboard สรุปข้อมูล และ REST API

## 🚀 Features

- Authentication (Admin)
- Product Management (CRUD)
- POS System (Search Product / Cart / Checkout)
- Auto Stock Deduction
- Stock Transaction History
- Sales History
- Dashboard Summary
- REST API
- Swagger API Documentation
- Unit Test
- SIT Testing
- UAT Checklist

## 🧱 Tech Stack

### Backend

- ASP.NET Core 8 MVC
- C#
- Entity Framework Core
- SQL Server

### Frontend

- Razor View
- Bootstrap 5
- JavaScript
- jQuery

### Security

- Cookie Authentication
- BCrypt Password Hashing
- AntiForgeryToken
- Session Timeout
- Global Exception Middleware

### Testing

- xUnit
- EF Core InMemory Database

## 🧪 Testing

### Unit Tests

- AuthService Tests
- Product Tests
- SaleService Tests
- Stock Behavior Tests

### System Testing

- SIT Testing
- UAT Checklist

### Test Result

Test Result:

- Unit tests implemented (xUnit)
- Coverage: Core services (AuthService, SaleService)

## ⚙️ How to Run

### Restore Packages

```bash
dotnet restore
```

### Build Project

```bash
dotnet build
```

### Run Application

```bash
dotnet run
```

### Open Browser

```text
https://localhost:xxxx
```

### Swagger

```text
https://localhost:xxxx/swagger
```

## 📂 Main Modules

- Authentication Module
- Dashboard Module
- Product Management Module
- POS Module
- Sales History Module
- Stock Transaction Module
- API Module

## 🔒 Security

- Password Hashing with BCrypt
- Cookie Authentication
- Authorization
- AntiForgeryToken
- Session Timeout / Cookie Expiration
- Global Exception Handling

## 📈 Dashboard

Dashboard provides:

- Total Products
- Total Sales Today
- Total Transactions
- Low Stock Products
- Daily Sales
- Top Selling Products
- Low Stock Alert

## 🌐 REST API

### Dashboard API

- GET /api/dashboard

### Product API

- GET /api/ProductApi
- GET /api/ProductApi/{id}

### Sales API

- GET /api/SalesApi
- GET /api/SalesApi/{id}
- GET /api/SalesApi/today

## 📋 Database Tables

- Users
- Products
- Sales
- SaleDetails
- StockTransactions

## 📦 Inventory Features

- Product Management
- Stock Tracking
- Automatic Stock Deduction
- Stock Transaction Logging
- Low Stock Monitoring

## 🛒 POS Features

- Product Search
- Add To Cart
- Checkout
- Sale Creation
- Sale Detail Creation
- Stock Update
- Transaction Logging

## 📊 Sales Features

- Sales History
- Sales Detail
- Search by Sale Number
- Filter by Date Range

## 🚀 Version

v1.3
