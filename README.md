# AgriTrace Backend — Agricultural Supply Chain Traceability System

Dự án Backend cho hệ thống truy xuất nguồn gốc nông sản **AgriTrace**, được xây dựng trên nền tảng **.NET 10 Web API** áp dụng kiến trúc **Clean Architecture (4 tầng)** kết hợp mô hình **CQRS (MediatR)**.

---

## 🚀 Công Nghệ Sử Dụng (Tech Stack)

- **Framework:** .NET 10 (ASP.NET Core Web API)
- **Kiến trúc:** Clean Architecture + CQRS Pattern (MediatR 12.x)
- **ORM / Database:** Entity Framework Core 10 (SQL Server Provider)
- **Authentication & Authorization:** JWT Bearer (Access Token & Refresh Token) + RBAC 2-Layer
- **DTO Mapping:** Mapster
- **Validation:** FluentValidation 11.x
- **API Documentation:** Swagger / OpenAPI UI với Bearer Token Security Document Filter
- **Security & Integrity:** SHA-256 Hash Chain Mechanism (Append-only Event Ledger)

---

## 🏗️ Kiến Trúc Hệ Thống (Solution Architecture)

Solution `AgriTrace.slnx` bao gồm 5 dự án thành phần:

```
AgriTrace-Backend-Group5/
├── AgriTrace.API/                      # Tầng Presentation (API Controllers, Filters, Middleware)
├── AgriTrace.Application/              # Tầng Nghiệp Vụ (CQRS Commands/Queries, DTOs, Validators)
├── AgriTrace.Domain/                   # Tầng Lõi (Entities, Enums, Interfaces, Domain Services)
├── AgriTrace.Infrastructure.Sqlserver/ # Tầng Hạ Tầng (EF Core DbContext, DataModels, Repositories)
└── AgriTrace.Tests/                    # Unit Tests & Integration Tests
```

---

## 🔑 Tài Khoản Mặc Định (Seed Data Accounts)

Sau khi khởi chạy ứng dụng và chạy Database Migration/Seed, hệ thống tự động khởi tạo các tài khoản mẫu với mật khẩu chung: **`Admin@123`**

| Role | Email | Ghi Chú |
|---|---|---|
| **Admin** | `admin@agritrace.com` | Quản trị hệ thống thuộc tổ chức `SYSTEM` |
| **Manager** | `manager@farm.com` | Quản lý trang trại |
| **Staff** | `staff@farm.com` | Nhân viên trang trại |
| **Inspector** | `inspector@agritrace.com` | Nhân viên kiểm định chất lượng |

---

## 🛠️ Hướng Dẫn Khởi Chạy (Quickstart Guide)

### 1. Yêu Cầu Tiền Đề
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Microsoft SQL Server (LocalDB hoặc SQL Server Management Studio/Docker SQL)

### 2. Cấu Hình Chuỗi Kết Nối (Database Connection String)
Kiểm tra và cập nhật `ConnectionStrings:DefaultConnection` trong file [appsettings.json](file:///c:/Users/Admin/source/repos/Code_GroupFive/AgriTrace-Backend-Group5/AgriTrace.API/appsettings.json):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=AgriTraceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Cập Nhật Database (EF Core Migration)
Chạy lệnh sau tại thư mục gốc Backend để áp dụng migrations và khởi tạo dữ liệu mẫu (Seed Data):
```bash
dotnet ef database update --project AgriTrace.Infrastructure.Sqlserver --startup-project AgriTrace.API
```

### 4. Cấu Hình Secret (Gmail SMTP - Tùy chọn)
Để gửi email khôi phục mật khẩu hoặc thông báo, cấu hình SMTP Password:
```bash
dotnet user-secrets set "Smtp:Pass" "your-app-password" --project AgriTrace.API
```

### 5. Khởi Chạy Server API
Khởi chạy ứng dụng với profile HTTP (Cổng mặc định: `http://localhost:5103`):
```bash
dotnet run --project AgriTrace.API/AgriTrace.API.csproj --launch-profile http
```
Sau khi khởi chạy thành công, truy cập Swagger UI tại: **`http://localhost:5103/swagger`**

---

## 📋 Danh Sách API Controllers (16 Modules)

| STT | Controller | Route Base | Mô Tả Chức Năng |
|---|---|---|---|
| 1 | `AuthController` | `/api/v1/auth` | Đăng nhập, refresh token, đăng xuất, profile, đổi mật khẩu |
| 2 | `OrganizationsController` | `/api/v1/organizations` | Quản lý tổ chức (FARM, PROCESSOR, DISTRIBUTOR, RETAILER, INSPECTION) |
| 3 | `UsersController` | `/api/v1/users` | Quản lý người dùng, phân quyền role, kích hoạt/vô hiệu hóa |
| 4 | `CategoriesController` | `/api/v1/categories` | Danh mục loại nông sản |
| 5 | `ProductsController` | `/api/v1/products` | Sản phẩm nông sản thuộc sở hữu tổ chức |
| 6 | `BatchesController` | `/api/v1/batches` | Quản lý lô hàng (Batch), sinh mã QR, theo dõi trạng thái |
| 7 | `EventsController` | `/api/v1/batches/{id}/events` | Ghi nhận sự kiện chuỗi cung ứng + Kiểm tra toàn vẹn Hash Chain |
| 8 | `EventRequestsController` | `/api/v1/event-requests` | Yêu cầu phê duyệt sự kiện chuỗi cung ứng |
| 9 | `BatchSplitMergeController` | `/api/v1/batches` | Tách lô (Split) và Gộp lô (Merge) nông sản |
| 10 | `InspectionsController` | `/api/v1/inspections` | Kiểm định chất lượng lô hàng (Quality Inspection) |
| 11 | `CertificatesController` | `/api/v1/certificates` | Cấp và quản lý chứng nhận chất lượng (VietGAP, GlobalGAP...) |
| 12 | `RecallsController` | `/api/v1/recalls` | Khởi tạo và xử lý thu hồi lô hàng (Product Recall) |
| 13 | `NotificationsController` | `/api/v1/notifications` | Thông báo nội bộ cho người dùng |
| 14 | `PublicController` | `/api/v1/public/trace` | Tra cứu lịch sử & phả hệ công khai (không cần đăng nhập) |
| 15 | `AnalyticsController` | `/api/v1/analytics` | Báo cáo thống kê, tổng quan hệ thống & truy vết ngược |
| 16 | `LookupController` | `/api/v1/lookup` | Lấy dữ liệu danh mục tĩnh (Roles, OrgTypes, EventTypes, Units...) |

---

## 🔒 Cơ Chế Bảo Mật & Chuỗi Băm (Hash Chain Security)

Mỗi sự kiện `SupplyChainEvent` được ghi vào hệ thống dưới dạng **Append-only log** (không cho phép xóa/sửa). Mỗi event chứa:
- `PreviousHash`: Mã băm SHA-256 của sự kiện ngay trước đó trong lô hàng.
- `CurrentHash`: Mã băm SHA-256 tính toán từ (BatchId + EventType + Payload + Timestamp + PreviousHash).

API `GET /api/v1/batches/{batchId}/events/verify` cho phép kiểm tra tính toàn vẹn của toàn bộ chuỗi sự kiện. Nếu bất kỳ dữ liệu nào bị can thiệp trái phép, mã băm sẽ không khớp và phát hiện vi phạm lập tức.

---

## ⚠️ Khắc Phục Lỗi Thường Gặp (Troubleshooting)

### Lỗi Đụng Cổng 5103 (`PortConflict5103` / `Address already in use`)
- **Nguyên nhân:** Tiến trình `AgriTrace.API` cũ vẫn đang chạy ngầm trên port `5103`.
- **Cách xử lý:** Chạy lệnh PowerShell sau để tắt tiến trình đang chiếm cổng và chạy lại backend:
```powershell
powershell -Command "Get-Process -Name AgriTrace.API -ErrorAction SilentlyContinue | Stop-Process -Force"
dotnet run --project AgriTrace.API/AgriTrace.API.csproj --launch-profile http
```