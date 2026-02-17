# 🔧 Elmolez

> منصة حجز فنيين الصيانة والخدمات المنزلية  
> Built with ASP.NET Core & React

---

## 📌 About The Project

**Fixly** هي منصة ويب تربط بين العملاء والفنيين المتخصصين في الصيانة والخدمات المنزلية داخل محافظة واحدة كبداية، مع خطة توسع مستقبلية.

الهدف من المشروع:
- تسهيل الوصول لفني موثوق بسرعة
- تنظيم سوق الصيانة المحلي
- توفير مصدر دخل مستقر للفنيين
- إنشاء نموذج عمل قابل للتوسع والربحية

---

## 🚀 Business Model

### 💰 مصادر الربح

- 15% عمولة من كل طلب مكتمل
- 500 جنيه اشتراك شهري للفني
- Feature Listing (ظهور مميز) – مستقبلاً
- رسوم خدمة عاجلة – مستقبلاً

---

## 🏗️ Tech Stack

### 🔹 Backend
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- JWT Authentication
- Role-Based Authorization

### 🔹 Frontend
- React
- TypeScript
- Axios
- Context API / Redux

### 🔹 Infrastructure
- Azure / VPS Hosting
- SignalR (Real-time Notifications)
- GitHub Actions (CI/CD)

---

## 👥 System Roles

### 👤 User
- Register / Login
- Search Technician
- Create Service Request
- Track Order
- Rate Technician

### 🛠 Technician
- Create Account
- Manage Profile
- Accept / Reject Orders
- Update Order Status
- View Earnings
- Manage Subscription

### 🛡 Admin
- Approve Technicians
- Manage Users
- Monitor Orders
- View Reports
- Manage Subscriptions

---

## 🔄 Order Lifecycle



---

## 🗄 Database Structure (Simplified)

### Users
- Id
- Name
- Email
- PasswordHash
- Phone
- Role

### Technicians
- Id (FK User)
- Specialty
- Bio
- Rating
- SubscriptionStatus
- Location

### Orders
- Id
- UserId
- TechnicianId
- Description
- Price
- Status
- CreatedAt

### Reviews
- Id
- OrderId
- Rating
- Comment

---

## 📊 Initial Financial Projection (MVP Phase)

**Starting with:**
- 10 Technicians
- 1 Order per day each
- Avg service price: 200 EGP

Monthly Revenue:

- Total Sales = 52,000 EGP
- Commission (15%) = 7,800 EGP
- Subscriptions = 5,000 EGP
- Total Revenue = 12,800 EGP

Estimated Net Profit (after expenses) ≈ 2,800 EGP

---

## 🛠️ Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js
- SQL Server

---

### 🔹 Backend Setup

```bash
dotnet restore
dotnet ef database update
dotnet run


