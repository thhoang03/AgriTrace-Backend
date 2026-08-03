# ToDoList - AgriTrace Backend (Agricultural Supply Chain Traceability System)

## Tổng quan dự án
- **Backend:** .NET 10 (ASP.NET Core Web API, Clean Architecture + CQRS với MediatR)
- **Frontend:** React 19 + TypeScript + Vite (Zustand + TanStack React Query + Tailwind CSS 4)
- **Database:** SQL Server (EF Core 10)
- **Trạng thái hiện tại:** **Toàn bộ 16 Backend API Controllers và CQRS Modules đã được triển khai hoàn chỉnh.**

---

## 📊 Bảng Tiến Độ Tổng Quan (Sprint Progress)

| Sprint | Nội dung chính | Trạng thái |
|---|---|---|
| **Sprint 1** | Nền tảng kiến trúc 4 tầng, Domain Entities, EF Core DataModels, Migrations & Authentication | **ĐÃ HOÀN THÀNH (100%)** |
| **Sprint 2** | Tính năng cốt lõi (16 Controllers, Organization, User, Product, Batch, Event, Hash Chain, Split/Merge) | **ĐÃ HOÀN THÀNH (100%)** |
| **Sprint 3** | Nâng cao (Inspection, Certificate, Recall, Public Trace, Lineage, Analytics, Notifications, Lookup) | **ĐÃ HOÀN THÀNH (95%)** |

---

## 🛠️ Sprint 1: Nền tảng & Thiết kế ✅

### 1.1 Thiết lập dự án
- [x] Tạo Solution Clean Architecture (.NET 10): `API` / `Application` / `Domain` / `Infrastructure.Sqlserver` / `Tests`
- [x] Cấu hình MediatR 12.x (CQRS), Mapster, EF Core 10
- [x] Thiết lập GitHub repo & tài liệu kiến trúc Clean Architecture trong `docs/`
- [x] **Cấu hình kết nối DB**: Connection string trong `appsettings.json`
- [x] **Cấu hình Swagger**: Tích hợp Swagger UI với JWT Bearer Security Document Filter

### 1.2 Domain Layer
- [x] `BaseEntity` (Id, CreatedAt, UpdatedAt)
- [x] ~~`Farm` + `Crop` entity (mẫu)~~ - **Đã xóa symlink mẫu**
- [x] **Domain Entities thật cho hệ thống Traceability (18 entities):**
  - [x] `OrganizationType` (enum: FARM, PROCESSOR, DISTRIBUTOR, RETAILER, INSPECTION, SYSTEM)
  - [x] `Organization` entity (Name, Address, Type, Status)
  - [x] `User` entity (FullName, Email, PasswordHash, Role, OrganizationId)
  - [x] `Category` entity (Product categories)
  - [x] `Unit` entity (kg, ton, box...)
  - [x] `Product` entity (thuộc Organization + Category + Unit)
  - [x] `Batch` entity (**aggregate root trung tâm**): BatchCode, QRCode, Quantity, RemainingQuantity, Status, HarvestDate, ParentBatchId, RootBatchId
  - [x] `EventType` (entity, 11 event types: HARVEST, RECEIVE, PROCESSING, PACKAGING, TRANSPORT, DISTRIBUTION, RETAIL, INSPECTION, SPLIT, MERGE, RECALL)
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
- [x] JWT Authentication (Access Token + Refresh Token)
- [x] `AuthService` (Login, RefreshToken, Logout, ChangePassword)
- [x] **2-layer RBAC**:
  - [x] **Layer 1 — Role-based**: `[Authorize]` + JWT claims (`Admin`, `Manager`, `Staff`); old roles (`Farmer`, `Inspector`, `Consumer`) migrated to `Staff` via `MigrateObsoleteRoles` migration
  - [x] **Layer 2 — OrganizationType × EventType matrix**: `EventPermissionRules.IsAllowed(orgTypeCode, eventTypeCode)` enforces event-type permissions per org type
  - [x] **Batch ownership guard**: `batch.CurrentOrganizationId == user.OrganizationId` (exception: `INSPECTION` org's cross-org INSPECTION events)
  - [x] **Admin bypass**: Admin skips Layer 2 event check but still obeys batch ownership guard
- [x] `CurrentUserService` (lấy thông tin user từ JWT claims)

### 1.5 API Contract & Response chuẩn
- [x] `ApiResponse` envelope (success, data, message, errors, timestamp)
- [x] `ApiResponseWrapperFilter` (auto-wrap responses)
- [x] `GlobalExceptionHandler` (404, 400, 500)
- [x] FluentValidation cho request models
- [x] API versioning (`/api/v1/`)
=======
### 1.2 Domain Layer & Database Design
- [x] `BaseEntity` (Id Guid, CreatedAt, UpdatedAt, MarkUpdated)
- [x] **Domain Entities (18 entities):**
  - [x] `OrganizationType`, `Organization`, `User`
  - [x] `Category`, `Unit`, `Product`
  - [x] `Batch` (**aggregate root**), `SupplyChainEvent` (**append-only, SHA-256 hash chain**)
  - [x] `QualityInspection`, `Certificate`, `Recall`
  - [x] `BatchSplit`, `BatchSplitDetail`, `BatchMerge`, `BatchMergeSource`
  - [x] `Notification`, `EventType`, `EventRequest`
- [x] **Domain Interfaces & Services**: Repositories + Services cho tất cả 18 thực thể
- [x] **EF Core DataModels & Configurations**: Fluent API Configurations cho 17 bảng
- [x] **Seed Data & Migration**: Seed data chuẩn PBKDF2 cho tài khoản mẫu với mật khẩu **`Admin@123`**

### 1.3 Authentication & Authorization
- [x] JWT Authentication (Access Token + Refresh Token trong `TokenService`)
- [x] `AuthService` (Login, RefreshToken, Logout, ChangePassword, GetProfile)
- [x] Role-based Authorization (RBAC Layer 1): Admin, Manager, Staff, Inspector, Consumer
- [x] Layer 2 Authorization: Permision Matrix giữa OrganizationType ↔ EventType
- [x] `CurrentUserService` (trích xuất thông tin user từ ClaimsPrincipal JWT)
>>>>>>> 50363ec (feat: implement Event Requests feature and fix EventType entity rehydration)

---

## ⚡ Sprint 2: Tính năng cốt lõi (Core Features) ✅

### 2.1 Management Modules
- [x] **Organization Management**: CRUD `/api/v1/organizations`, lọc phân trang, đổi trạng thái, danh sách users/products thuộc org
- [x] **User Management**: CRUD `/api/v1/users`, phân quyền role, kích hoạt/vô hiệu hóa, đổi mật khẩu, xem profile
- [x] **Product Management**: CRUD Categories, Units, Products thuộc sở hữu của tổ chức
- [x] **Batch Management**: Tạo batch, tự động sinh `BatchCode` và `QRCode`, cập nhật thông tin và trạng thái

### 2.2 Supply Chain Events & Hash Chain Mechanism ⭐
- [x] `POST /api/v1/batches/{id}/events` — Ghi nhận sự kiện mới
- [x] `GET /api/v1/batches/{id}/events` — Danh sách sự kiện của lô hàng
- [x] **Hash Chain Mechanism**:
  - [x] `HashChainService` (SHA-256): Tính toán `PreviousHash` và `CurrentHash`
  - [x] Append-only ledger (cấm sửa/xóa event)
  - [x] `GET /api/v1/batches/{id}/events/verify` — Kiểm tra toàn vẹn chuỗi băm
- [x] **Event Request Workflow**:
  - [x] `POST /api/v1/event-requests` — Đăng ký yêu cầu tạo event
  - [x] `PATCH /api/v1/event-requests/{id}/approve` — Phê duyệt event request

### 2.3 Batch Split & Merge (Tách / Gộp lô)
- [x] `POST /api/v1/batches/{id}/split` — Tách batch thành nhiều batch con, trừ `RemainingQuantity`
- [x] `POST /api/v1/batches/merge` — Gộp nhiều batch nguồn thành batch mới
- [x] Tự động ghi nhận event `SPLIT` / `MERGE` vào chuỗi băm

### 2.7 Application Layer (CQRS)
- [x] **Commands/Queries** cho tất cả features (MediatR handlers)
- [x] **Mapping** (Mapster) giữa Domain ↔ DataModel ↔ DTO
- [x] **Validation** (FluentValidation) cho requests
- [x] **Repositories** đăng ký DI + implement GenericRepository

### 2.8 API Controllers
- [x] `AuthController` (login, refresh, logout, me)
- [x] `OrganizationsController`
- [x] `UsersController`
- [x] `CategoriesController`
- [x] `ProductsController`
- [x] `BatchesController`
- [x] `EventsController`
- [x] `InspectionsController`
- [x] `CertificatesController`
- [x] `RecallsController`
- [x] `NotificationsController`
- [x] `PublicController` (trace, lineage)
- [x] `AnalyticsController`
- [x] `LookupController` (roles, organization-types, event-types, units, ...)

### 2.9 Testing Sprint 2
- [x] Unit Tests cho Domain entities (guard clauses, business rules)
- [x] Unit Tests cho HashChainService
- [x] Unit Tests cho Application handlers (Commands/Queries)
- [x] Unit Tests cho EventPermissionRules (Layer 2 RBAC matrix)
- [ ] Unit Tests cho API Controllers
- [ ] Coverage tối thiểu 60% logic nghiệp vụ
=======
### 2.4 API Controllers & Application Layer
- [x] **CQRS Commands/Queries**: Phân tách logic ghi/đọc bằng MediatR
- [x] **FluentValidation**: Validation pipeline behavior tự động kiểm tra request model
- [x] **Envelope Standard**: Bọc toàn bộ response bằng `ApiResponseWrapperFilter`
- [x] **Global Exception Handler**: Bắt lỗi tập trung (400, 404, 409, 500)
>>>>>>> 50363ec (feat: implement Event Requests feature and fix EventType entity rehydration)

---

## 🏆 Sprint 3: Nâng cao & Public Traceability ✅

### 3.1 Quality Inspection & Certificate
- [x] `POST/GET/PUT /api/v1/inspections` — Tạo, danh sách và cập nhật phiếu kiểm định chất lượng
- [x] `POST/GET/DELETE /api/v1/certificates` — Cấp chứng nhận chất lượng (VietGAP, Organic...) và thu hồi

### 3.2 Product Recall Management
- [x] `POST/GET /api/v1/recalls` — Khởi tạo lệnh thu hồi sản phẩm khẩn cấp (dành cho Admin / SYSTEM)
- [x] `PATCH /api/v1/recalls/{id}/resolve` — Kết thúc xử lý thu hồi
- [x] Tự động cập nhật trạng thái Batch → `RECALLED` và phát thông báo nội bộ

### 3.3 Public Traceability & Analytics ⭐
- [x] `GET /api/v1/public/trace/{batchId}` — Tra cứu công khai lô hàng (không cần Auth)
- [x] `GET /api/v1/public/trace/{batchId}/lineage` — Xem phả hệ lịch sử split/merge của lô hàng
- [x] `GET /api/v1/analytics/overview` — Dashboard báo cáo thống kê tổng quan
- [x] `GET /api/v1/analytics/batch-distribution` — Thống kê phân bố lô hàng theo trạng thái
- [x] `GET /api/v1/analytics/processing-time` — Thống kê thời gian xử lý qua từng mắt xích
- [x] `GET /api/v1/analytics/traceback/{batchId}` — Phân tích truy vết ngược

### 3.4 Notifications & Master Lookup APIs
- [x] `/api/v1/notifications` — Danh sách thông báo, số chưa đọc, đánh dấu đã đọc
- [x] `/api/v1/lookup` — Master lookup data (Roles, OrgTypes, EventTypes, Units, Severities)

---

## 📌 Nhật Ký Cập Nhật Mới Nhất

### Cập nhật 2026-07-28 & Gần Đây:
1. **Fix Lỗi Password Hash Seed Data:**
   - Đã khắc phục lỗi hash cũ khiến tài khoản mặc định không thể đăng nhập. Tất cả tài khoản mẫu hiện dùng chuẩn băm PBKDF2 với password mặc định **`Admin@123`**.
2. **Fix Security Requirement trong Swagger UI:**
   - Thêm `BearerSecurityRequirementDocumentFilter.cs` xử lý chuẩn tương thích với Swashbuckle 10.x, đảm bảo gửi kèm header `Authorization: Bearer <token>` trên Swagger UI.
3. **Hoàn thiện 16 Controllers & MediatR CQRS Handlers:**
   - 100% API controllers đã được đăng ký và nối thông suốt với EF Core database.

---

## 🔮 Hạng Mục Nâng Cấp Tiếp Theo (Optional / RoadMap)

> Định dạng: **Là** [vai trò], **tôi muốn** [tính năng], **để** [mục tiêu].
> Actor hệ thống: `Admin`, `Manager`, `Staff/Farmer`, `Inspector`, `Consumer`.
> Trạng thái `[x]` = API backend đã có; `[ ]` = chưa làm / cần bổ sung.

### Authentication & Account (mọi actor có tài khoản)
- [x] **US-AUTH-01** — Là người dùng, tôi muốn đăng nhập bằng email + mật khẩu để nhận JWT truy cập hệ thống.
- [x] **US-AUTH-02** — Là người dùng, tôi muốn refresh access token bằng refresh token để không phải đăng nhập lại liên tục.
- [x] **US-AUTH-03** — Là người dùng, tôi muốn đăng xuất để thu hồi phiên hiện tại.
- [x] **US-AUTH-04** — Là người dùng, tôi muốn đổi mật khẩu để bảo mật tài khoản.
- [x] **US-AUTH-05** — Là người dùng, tôi muốn xem/cập nhật hồ sơ cá nhân (profile).
- [ ] **US-AUTH-06** — Là người dùng, tôi muốn nhận thông báo lỗi rõ ràng khi sai thông tin đăng nhập / token hết hạn (chuẩn hóa mã lỗi).
- [x] **US-AUTH-07** — Là người dùng, tôi muốn quên mật khẩu và đặt lại qua email (forgot/reset password).

### Admin
- [x] **US-ADM-01** — Là Admin, tôi muốn quản lý (CRUD) tổ chức để cấu hình các mắt xích chuỗi cung ứng.
- [x] **US-ADM-02** — Là Admin, tôi muốn kích hoạt/vô hiệu hóa trạng thái tổ chức.
- [x] **US-ADM-03** — Là Admin, tôi muốn quản lý (CRUD) người dùng và phân quyền role.
- [x] **US-ADM-04** — Là Admin, tôi muốn quản lý danh mục cấu hình (Categories, Units) toàn hệ thống.
- [x] **US-ADM-05** — Là Admin, tôi muốn khởi tạo lệnh thu hồi (Recall) ở phạm vi toàn hệ thống.
- [x] **US-ADM-06** — Là Admin, tôi muốn xem dashboard analytics tổng quan toàn hệ thống.
- [ ] **US-ADM-07** — Là Admin, tôi muốn xem audit log các thao tác quản trị để giám sát.
- [ ] **US-ADM-08** — Là Admin, tôi muốn phân quyền chi tiết (permission theo endpoint) thay vì chỉ theo role.

### Manager (quản trị nội bộ 1 tổ chức)
- [x] **US-MGR-01** — Là Manager, tôi muốn quản lý nhân viên trực thuộc tổ chức của mình.
- [x] **US-MGR-02** — Là Manager, tôi muốn quản lý (CRUD) sản phẩm thuộc tổ chức.
- [x] **US-MGR-03** — Là Manager, tôi muốn quản lý các lô hàng (Batch) thuộc phạm vi tổ chức sở hữu.
- [x] **US-MGR-04** — Là Manager, tôi muốn xem danh sách người dùng và sản phẩm của tổ chức.
- [ ] **US-MGR-05** — Là Manager, tôi muốn giới hạn dữ liệu chỉ thấy được của tổ chức mình (data scoping/tenant filter theo OrganizationId, batch ownership guard).
- [ ] **US-MGR-06** — Là Manager, tôi muốn upload hình ảnh sản phẩm/batch thật (lưu trữ file/cloud storage).

### Staff / Farmer (nghiệp vụ theo Organization Type)
- [x] **US-STF-01** — Là Staff, tôi muốn tạo lô hàng mới (Batch) kèm sinh BatchCode + QR Code.
- [x] **US-STF-02** — Là Staff, tôi muốn ghi nhận sự kiện chuỗi cung ứng (Event) cho lô hàng.
- [x] **US-STF-03** — Là Staff, tôi muốn chỉ được tạo loại event phù hợp với Organization Type (event permission, Layer 2 RBAC matrix).
- [x] **US-STF-04** — Là Staff, tôi muốn chia lô (Split) và gộp lô (Merge) với cập nhật số lượng còn lại.
- [x] **US-STF-05** — Là Staff, tôi muốn cập nhật trạng thái lô hàng.
- [x] **US-STF-06** — Là Staff, tôi muốn mỗi event được ký hash chain (PreviousHash/CurrentHash) và append-only để đảm bảo toàn vẹn.
- [ ] **US-STF-07** — Là Staff, tôi muốn nhận cảnh báo/thông báo khi lô hàng liên quan bị thu hồi.

### Inspector (kiểm định độc lập)
- [x] **US-INS-01** — Là Inspector, tôi muốn tạo phiếu kiểm định chất lượng cho lô hàng.
- [x] **US-INS-02** — Là Inspector, tôi muốn cập nhật kết quả/ghi chú kiểm định.
- [x] **US-INS-03** — Là Inspector, tôi muốn cấp chứng nhận (Certificate) cho lô hàng đạt yêu cầu.
- [x] **US-INS-04** — Là Inspector, tôi muốn thu hồi chứng nhận đã cấp.
- [ ] **US-INS-05** — Là Inspector, tôi muốn upload file chứng nhận thật (PDF/ảnh) và đính kèm mã băm.
- [x] **US-INS-06** — Là Inspector, tôi muốn inspectorId được lấy tự động từ JWT thay vì truyền trong body.

### Consumer (công khai, read-only, không đăng nhập)
- [x] **US-CON-01** — Là Consumer, tôi muốn quét QR / mở public URL để tra cứu lô hàng mà không cần đăng nhập.
- [x] **US-CON-02** — Là Consumer, tôi muốn xem timeline truy xuất (events, inspections, certificates, recall status).
- [x] **US-CON-03** — Là Consumer, tôi muốn xem phả hệ lô hàng (lineage: lịch sử split/merge).
- [ ] **US-CON-04** — Là Consumer, tôi muốn trang tra cứu phản hồi nhanh (< 1.5s) nhờ Redis cache (TTL 5 phút).

### Notifications & Analytics (xuyên actor)
- [x] **US-NOTI-01** — Là người dùng, tôi muốn xem danh sách thông báo và số thông báo chưa đọc.
- [x] **US-NOTI-02** — Là người dùng, tôi muốn đánh dấu đã đọc từng thông báo / tất cả.
- [x] **US-NOTI-03** — Là hệ thống, tôi muốn tự động gửi thông báo tới tổ chức liên quan khi có recall.
- [ ] **US-NOTI-04** — Là người dùng, tôi muốn nhận thông báo real-time (SignalR) thay vì phải refresh.
- [x] **US-ANA-01** — Là Admin/Manager, tôi muốn xem thống kê phân bố batch, thời gian xử lý, truy vết ngược.

### Chất lượng & Vận hành (non-functional / cần bổ sung)
- [x] **US-QA-01** — Là dev, tôi muốn có Unit Test cho Domain entities & HashChainService để đảm bảo business rule.
- [ ] **US-QA-02** — Là dev, tôi muốn có Unit/Integration Test cho Application handlers & Controllers (coverage ≥ 60%).
- [ ] **US-QA-03** — Là dev, tôi muốn có Integration Test API → DB và E2E test.
- [ ] **US-OPS-01** — Là dev, tôi muốn Dockerfile + docker-compose (backend + SQL Server + Redis) để chạy đồng nhất.
- [ ] **US-OPS-02** — Là dev, tôi muốn cấu hình CI/CD tự động build + test + (deploy).
- [ ] **US-OPS-03** — Là dev, tôi muốn secret production (JWT, connection string) quản lý an toàn (user-secrets / env / vault), không hardcode.
- [ ] **US-OPS-04** — Là dev, tôi muốn deploy lên cloud và tối ưu performance public trace.

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

---

## Ghi chú cập nhật (2026-07-20)

### Đã hoàn thành
- **Application layer (CQRS)**: Commands/Queries + MediatR handlers cho toàn bộ features; Mapster mapping; FluentValidation (12 validators); repositories + DI.
- **15 API Controllers** đầy đủ: Auth, Organizations, Users, Categories, Products, Batches, Events, Inspections, Certificates, Recalls, Notifications, Public, Analytics, Lookup, BatchSplitMerge.
- **Authentication & Authorization**: JWT (access + refresh token), `TokenService`, `CurrentUserService`, RBAC theo role, login/refresh/logout/change-password/me.
- **API contract**: `ApiResponse` envelope, wrapper filter, global exception handler, FluentValidation, versioning `/api/v1/`.
- **Farm/Crop mẫu** đã được xóa.

### Fix quan trọng trong đợt này (Authentication + Swagger)
1. **Seeded users sai password hash** (`PasswordHash = "123"` trong DB đã migrate) khiến login luôn thất bại.
   - Tạo migration `20260719152848_FixSeedUserPasswordHashes` cập nhật hash PBKDF2 đúng cho 4 user, đã `database update`.
   - Sửa migration `updateSeedData` + `SeedData.cs` để fresh deploy nhất quán.
   - Tất cả seeded user dùng password: **`Admin@123`**.
2. **Swagger không gửi header `Authorization`** dù đã bấm Authorize.
   - Nguyên nhân: Swashbuckle 10.x / Microsoft.OpenApi 2.x serialize `security` thành object thay vì array.
   - Fix: thêm `Swagger/BearerSecurityRequirementDocumentFilter.cs`, thay `AddSecurityRequirement(...)` bằng `DocumentFilter`. Nay `swagger.json` có `"security": [ { "BearerAuth": [] } ]` đúng chuẩn.
- Đã verify: login `admin@agritrace.com` / `Admin@123` → nhận JWT; gọi endpoint bảo vệ với Bearer token → 200.

### Còn lại (chưa làm)
- **Testing**: Unit / Integration / E2E tests, coverage.
- **Upload file thật**: hình ảnh sản phẩm/batch, file chứng nhận (hiện là stub).
- **Redis cache** cho public trace.
- **Docker**: Dockerfile + docker-compose (backend + SQL Server + Redis + frontend).
- **Frontend** (React + TypeScript + Redux) + Public Portal.
- **Deploy** cloud + tối ưu performance + tài liệu vận hành + slide demo.
- **Tính năng nâng cao (optional)**: QR scanner FE, blockchain simulation, geospatial, PDF export ký số, SignalR real-time.

---

## Ghi chú cập nhật (2026-07-30)

### Đã hoàn thành sau 2026-07-20
- **Forgot/Reset Password**: `ForgotPasswordCommand`, `ResetPasswordCommand` implemented; `AuthController` has `POST /api/v1/auth/forgot-password` and `POST /api/v1/auth/reset-password` endpoints; email templates (`PasswordResetEmailTemplate`) and email services (`GmailEmailService`, `SendGridEmailService`) wired up.
- **inspectorId from JWT**: `InspectionsController` now gets `inspectorId` from `_currentUser.UserId` (JWT claims) instead of request body.
- **Unit Tests**: 27 test files in `AgriTrace.Tests` covering Domain entities (User, Batch, Organization, Product, SupplyChainEvent), HashChainService, EventService, SupplyChainEventWriteService, Application Commands/Queries (CreateBatch, MergeBatch, SplitBatch, CreateOrganization, CreateProduct, CreateUser, CreateRecall, CreateSupplyChainEvent, etc.), and FluentValidation validators (13 validator tests).
- **Migrations**: 61+ migrations reflecting schema evolution through 2026-07-30 (seed data expansions, model redesigns, user status/must-change-password fields, quality inspection refactoring, unique constraints).
- **BatchSplitMergeController** added as a dedicated controller (split/merge endpoints).
=======
- [ ] Viết Unit Tests & Integration Tests tự động cho Domain services và CQRS handlers (Target coverage: ≥ 70%).
- [ ] Cấu hình Redis Cache cho API Tra cứu công khai (`GET /api/v1/public/trace`).
- [ ] Đóng gói Docker Compose (Backend API + SQL Server + Redis + Nginx Frontend).
- [ ] Triển khai Real-time notifications qua SignalR.
