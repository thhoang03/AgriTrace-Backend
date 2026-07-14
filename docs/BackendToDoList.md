# ToDoList - AgriTrace (Agricultural Supply Chain Traceability System)

## Tổng quan
- **Backend:** .NET 10 (ASP.NET Core Web API, Clean Architecture + CQRS)
- **Frontend:** React + TypeScript (Redux Toolkit)
- **Database:** SQL Server
- **Thời gian:** 6 tuần (3 sprint)
- **Nhóm:** 7 thành viên

---

## Sprint 1: Nền tảng & Thiết kế

### 1.1 Thiết lập dự án
- [x] Tạo Solution Clean Architecture (.NET 10): API / Application / Domain / Infrastructure
- [x] Cấu hình MediatR (CQRS), Mapster, EF Core 10
- [x] Thiết lập GitHub repo, CI/CD workflow (build + test)
- [x] Viết tài liệu kiến trúc (docs/)
- [ ] **Tạo Frontend project (React + TypeScript + Redux)**
- [ ] **Cấu hình Docker**: Dockerfile cho backend, docker-compose cho SQL Server + backend
- [ ] **Cấu hình kết nối DB**: Connection string trong appsettings.Development.json

### 1.2 Domain Layer
- [x] `BaseEntity` (Id, CreatedAt, UpdatedAt)
- [ ] `Farm` + `Crop` entity (mẫu - **cần xóa symlink**)
- [x] **Domain Entities thật cho hệ thống Traceability (18 entities):**
  - [x] `OrganizationType` (enum: FARM, PROCESSOR, DISTRIBUTOR, RETAILER, INSPECTION, SYSTEM)
  - [x] `Organization` entity (Name, Address, Type, Status)
  - [x] `User` entity (FullName, Email, PasswordHash, Role, OrganizationId)
  - [x] `Category` entity (Product categories)
  - [x] `Unit` entity (kg, ton, box...)
  - [x] `Product` entity (thuộc Organization + Category + Unit)
  - [x] `Batch` entity (**aggregate root trung tâm**): BatchCode, QRCode, Quantity, RemainingQuantity, Status, HarvestDate, ParentBatchId, RootBatchId
  - [x] `EventType` (enum: HARVEST, RECEIVE, PROCESSING, PACKAGING, TRANSPORT, DISTRIBUTION, RETAIL, SALE, INSPECTION, SPLIT, MERGE, RECALL)
  - [x] `SupplyChainEvent` entity (**append-only**): BatchId, EventType, OrganizationId, UserId, EventData, Location, PreviousHash, CurrentHash
  - [x] `QualityInspection` entity (BatchId, InspectorId, Status, Result, Notes)
  - [x] `Certificate` entity (BatchId, InspectionId, Type, FileUrl)
  - [x] `Recall` entity (BatchId, Reason, Severity, Status)
  - [x] `BatchSplit` / `BatchSplitDetail` entities
  - [x] `BatchMerge` / `BatchMergeSource` entities
  - [x] `Notification` entity (UserId, Title, Message, IsRead)
- [x] **Domain Interfaces**: Repositories + Services cho tất cả entities
- [x] **Domain Services**: `BatchService`, `BatchMergeService`, `BatchSplitService`, `EventService`, `HashChainService`, `RecallService`, `NotificationService`, ...

### 1.3 Database Design
- [x] Thiết kế ERD trong docs/
- [x] **EF Core DataModels** cho tất cả bảng (17 DataModels)
- [x] **Fluent API Configurations** cho từng bảng (PK, FK, indexes, constraints)
- [x] **Seed data**: OrganizationTypes, EventTypes
- [x] **Tạo Migration đầu tiên** (`Add-Migration Initial`)
- [x] **Cập nhật DB** (`Update-Database`)

### 1.4 Authentication & Authorization
- [ ] JWT Authentication (Access Token + Refresh Token)
- [ ] `AuthService` (Login, RefreshToken, Logout, ChangePassword)
- [ ] Role-based Authorization (RBAC): SYSTEM_ADMIN, ORG_ADMIN, STAFF, INSPECTOR
- [ ] Permission check theo OrganizationType + EventType
- [ ] `CurrentUserService` (lấy thông tin user từ JWT claims)

### 1.5 API Contract & Response chuẩn
- [x] `ApiResponse` envelope (success, data, message, errors, timestamp)
- [x] `ApiResponseWrapperFilter` (auto-wrap responses)
- [x] `GlobalExceptionHandler` (404, 400, 500)
- [ ] FluentValidation cho request models
- [ ] API versioning (`/api/v1/`)

---

## Sprint 2: Tính năng cốt lõi

### 2.1 Organization Management
- [ ] `POST /api/v1/organizations` - Tạo tổ chức
- [ ] `GET /api/v1/organizations` - Danh sách (phân trang)
- [ ] `GET /api/v1/organizations/{id}` - Chi tiết
- [ ] `PUT /api/v1/organizations/{id}` - Cập nhật
- [ ] `PATCH /api/v1/organizations/{id}/status` - Đổi trạng thái
- [ ] `GET /api/v1/organizations/{id}/users` - Người dùng của tổ chức
- [ ] `GET /api/v1/organizations/{id}/products` - Sản phẩm của tổ chức

### 2.2 User Management
- [ ] `POST /api/v1/users` - Tạo user
- [ ] `GET /api/v1/users` - Danh sách (phân trang, lọc)
- [ ] `GET /api/v1/users/{id}` - Chi tiết
- [ ] `PUT /api/v1/users/{id}` - Cập nhật
- [ ] `PATCH /api/v1/users/{id}/status` - Kích hoạt/vô hiệu hóa
- [ ] `GET /api/v1/users/profile` - Hồ sơ cá nhân
- [ ] `PUT /api/v1/users/profile` - Cập nhật hồ sơ

### 2.3 Product Management
- [ ] **Categories**: CRUD (thêm sửa xóa danh mục)
- [ ] **Units**: CRUD (đơn vị tính)
- [ ] **Products**: CRUD (sản phẩm thuộc tổ chức)
- [ ] Upload hình ảnh sản phẩm

### 2.4 Batch Management (Lô hàng) ⭐
- [ ] `POST /api/v1/batches` - Tạo batch + sinh BatchCode + QR Code
- [ ] `GET /api/v1/batches` - Danh sách (phân trang, lọc theo tổ chức/trạng thái)
- [ ] `GET /api/v1/batches/{id}` - Chi tiết batch (kèm events, inspections, certificates)
- [ ] `PUT /api/v1/batches/{id}` - Cập nhật thông tin
- [ ] `PATCH /api/v1/batches/{id}/status` - Cập nhật trạng thái
- [ ] Sinh mã QR chứa URL truy xuất

### 2.5 Supply Chain Events (Sự kiện chuỗi) ⭐
- [ ] `POST /api/v1/batches/{id}/events` - Ghi nhận event mới
- [ ] `GET /api/v1/batches/{id}/events` - Danh sách events của batch
- [ ] **Hash Chain Mechanism**:
  - [x] Domain logic: `HashChainService` (SHA-256), `EventService` (GENESIS, VerifyHashChainAsync)
  - [ ] Lưu PreviousHash + CurrentHash mỗi event
  - [ ] Append-only (không update/delete event)
  - [ ] `GET /api/v1/batches/{id}/events/verify` - Kiểm tra tính toàn vẹn hash chain
- [ ] **Event permission**: kiểm tra OrganizationType có được tạo event type đó không

### 2.6 Batch Split & Merge
- [ ] `POST /api/v1/batches/{id}/split` - Chia lô
- [ ] `POST /api/v1/batches/merge` - Gộp lô
- [ ] Cập nhật RemainingQuantity, tạo Batch con, ghi event SPLIT/MERGE

### 2.7 Application Layer (CQRS)
- [ ] **Commands/Queries** cho tất cả features (MediatR handlers)
- [ ] **Mapping** (Mapster) giữa Domain ↔ DataModel ↔ DTO
- [ ] **Validation** (FluentValidation) cho requests
- [ ] **Repositories** đăng ký DI + implement GenericRepository

### 2.8 API Controllers
- [ ] `AuthController` (login, refresh, logout, me)
- [ ] `OrganizationsController`
- [ ] `UsersController`
- [ ] `CategoriesController`
- [ ] `ProductsController`
- [ ] `BatchesController`
- [ ] `EventsController`
- [ ] `InspectionsController`
- [ ] `CertificatesController`
- [ ] `RecallsController`
- [ ] `NotificationsController`
- [ ] `PublicController` (trace, lineage)
- [ ] `AnalyticsController`

### 2.9 Testing Sprint 2
- [ ] Unit Tests cho Domain entities (guard clauses, business rules)
- [ ] Unit Tests cho HashChainService
- [ ] Unit Tests cho Application handlers (Commands/Queries)
- [ ] Unit Tests cho API Controllers
- [ ] Coverage tối thiểu 60% logic nghiệp vụ

---

## Sprint 3: Nâng cao & Triển khai

### 3.1 Quality Inspection & Certificate
- [ ] `POST /api/v1/batches/{id}/inspections` - Tạo kiểm định
- [ ] `GET /api/v1/batches/{id}/inspections` - Danh sách kiểm định
- [ ] `GET /api/v1/inspections/{id}` - Chi tiết kiểm định
- [ ] `PUT /api/v1/inspections/{id}` - Cập nhật
- [ ] `POST /api/v1/batches/{id}/certificates` - Cấp chứng nhận
- [ ] `GET /api/v1/batches/{id}/certificates` - Danh sách chứng nhận
- [ ] `DELETE /api/v1/certificates/{id}` - Thu hồi chứng nhận
- [ ] Upload file chứng nhận (PDF/image)

### 3.2 Recall Management (Thu hồi)
- [ ] `POST /api/v1/recalls` - Tạo recall
- [ ] `GET /api/v1/recalls` - Danh sách
- [ ] `GET /api/v1/recalls/{id}` - Chi tiết
- [ ] `PATCH /api/v1/recalls/{id}/resolve` - Kết thúc recall
- [ ] Tìm tổ chức liên quan và gửi notification
- [ ] Cập nhật trạng thái batch → RECALLED

### 3.3 Public Traceability (Tra cứu công khai) ⭐
- [ ] `GET /api/v1/public/trace/{batchId}` - Tra cứu batch (không cần auth)
- [ ] `GET /api/v1/public/trace/{batchId}/lineage` - Phả hệ batch (split/merge history)
- [ ] Timeline view: hiển thị dòng thời gian events
- [ ] Hiển thị certificates, inspections, recall status
- [ ] **Redis Cache** cho public trace (TTL 5 phút)
- [ ] **Frontend Public Portal**: trang tra cứu tối ưu mobile, quét QR

### 3.4 Notification System
- [ ] `GET /api/v1/notifications` - Danh sách thông báo
- [ ] `PATCH /api/v1/notifications/{id}/read` - Đánh dấu đã đọc
- [ ] `PATCH /api/v1/notifications/read-all` - Đánh dấu tất cả
- [ ] `GET /api/v1/notifications/unread-count` - Số chưa đọc
- [ ] Tự động gửi notification khi có recall

### 3.5 Analytics Dashboard
- [ ] `GET /api/v1/analytics/overview` - Dashboard tổng quan
- [ ] `GET /api/v1/analytics/batch-distribution` - Thống kê batch theo trạng thái
- [ ] `GET /api/v1/analytics/processing-time` - Thời gian xử lý trung bình
- [ ] `GET /api/v1/analytics/traceback/{batchId}` - Truy vết ngược

### 3.6 Lookup APIs
- [ ] `GET /api/v1/roles`
- [ ] `GET /api/v1/organization-types`
- [ ] `GET /api/v1/event-types`
- [ ] `GET /api/v1/units`
- [ ] `GET /api/v1/inspection-results`
- [ ] `GET /api/v1/certificate-types`
- [ ] `GET /api/v1/recall-severities`

### 3.7 Testing & Deployment
- [ ] Integration Tests (API → DB)
- [ ] E2E Tests (Frontend → API → DB)
- [ ] User Acceptance Testing (UAT)
- [ ] **Docker Compose** hoàn chỉnh (backend + SQL Server + Redis + frontend)
- [ ] Deploy lên cloud (Azure / Render / Railway)
- [ ] Tối ưu public trace performance (< 1.5s)
- [ ] Tài liệu hướng dẫn cài đặt & vận hành
- [ ] Slide thuyết trình + kịch bản demo

---

## Tính năng nâng cao (Optional - tăng điểm)
- [ ] **QR Code Generator & Scanner** (frontend tích hợp camera)
- [ ] **Blockchain Simulation** (Hash-chaining + audit log validation)
- [ ] **Geospatial Tracking** (Google Maps / Leaflet hiển thị lộ trình)
- [ ] **Automated PDF Report Export** (ký điện tử, mã băm)
- [ ] **Real-time notifications** (SignalR)

---

## Ký hiệu
- [x] Đã hoàn thành
- [ ] Chưa làm / Cần làm
- ⭐ Tính năng quan trọng nhất

---

## Ghi chú cập nhật (2026-07-13)

- Domain layer hoàn chỉnh: 18 entities + 17 domain services + interfaces
- Infrastructure: 17 DataModels + Fluent API Configs + SeedData + Migration
- **Chưa có:** CQRS handlers, Controllers, Auth/JWT, Tests, Docker
- **Lưu ý:** Farm/Crop là symlink mẫu, cần xóa trước khi triển khai thật