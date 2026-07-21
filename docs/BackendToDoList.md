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
- [x] **Cấu hình kết nối DB**: Connection string trong appsettings.json

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
- [x] JWT Authentication (Access Token + Refresh Token)
- [x] `AuthService` (Login, RefreshToken, Logout, ChangePassword)
- [x] Role-based Authorization (RBAC): ADMIN, MANAGER, STAFF, FARMER, INSPECTOR, CONSUMER
- [x] Permission check theo OrganizationType + EventType
- [x] `CurrentUserService` (lấy thông tin user từ JWT claims)

### 1.5 API Contract & Response chuẩn
- [x] `ApiResponse` envelope (success, data, message, errors, timestamp)
- [x] `ApiResponseWrapperFilter` (auto-wrap responses)
- [x] `GlobalExceptionHandler` (404, 400, 500)
- [x] FluentValidation cho request models
- [x] API versioning (`/api/v1/`)

---

## Sprint 2: Tính năng cốt lõi

### 2.1 Organization Management
- [x] `POST /api/v1/organizations` - Tạo tổ chức
- [x] `GET /api/v1/organizations` - Danh sách (phân trang)
- [x] `GET /api/v1/organizations/{id}` - Chi tiết
- [x] `PUT /api/v1/organizations/{id}` - Cập nhật
- [x] `PATCH /api/v1/organizations/{id}/status` - Đổi trạng thái
- [x] `GET /api/v1/organizations/{id}/users` - Người dùng của tổ chức
- [x] `GET /api/v1/organizations/{id}/products` - Sản phẩm của tổ chức

### 2.2 User Management
- [x] `POST /api/v1/users` - Tạo user
- [x] `GET /api/v1/users` - Danh sách (phân trang, lọc)
- [x] `GET /api/v1/users/{id}` - Chi tiết
- [x] `PUT /api/v1/users/{id}` - Cập nhật
- [x] `PATCH /api/v1/users/{id}/status` - Kích hoạt/vô hiệu hóa
- [x] `GET /api/v1/users/profile` - Hồ sơ cá nhân
- [x] `PUT /api/v1/users/profile` - Cập nhật hồ sơ

### 2.3 Product Management
- [x] **Categories**: CRUD (thêm sửa xóa danh mục)
- [x] **Units**: CRUD (đơn vị tính)
- [x] **Products**: CRUD (sản phẩm thuộc tổ chức)
- [ ] Upload hình ảnh sản phẩm (endpoint stub, chưa lưu file thật)

### 2.4 Batch Management (Lô hàng) ⭐
- [x] `POST /api/v1/batches` - Tạo batch + sinh BatchCode + QR Code
- [x] `GET /api/v1/batches` - Danh sách (phân trang, lọc theo tổ chức/trạng thái)
- [x] `GET /api/v1/batches/{id}` - Chi tiết batch (kèm events, inspections, certificates)
- [x] `PUT /api/v1/batches/{id}` - Cập nhật thông tin
- [x] `PATCH /api/v1/batches/{id}/status` - Cập nhật trạng thái
- [x] Sinh mã QR chứa URL truy xuất

### 2.5 Supply Chain Events (Sự kiện chuỗi) ⭐
- [x] `POST /api/v1/batches/{id}/events` - Ghi nhận event mới
- [x] `GET /api/v1/batches/{id}/events` - Danh sách events của batch
- [x] **Hash Chain Mechanism**:
  - [x] Domain logic: `HashChainService` (SHA-256), `EventService` (GENESIS, VerifyHashChainAsync)
  - [x] Lưu PreviousHash + CurrentHash mỗi event
  - [x] Append-only (không update/delete event)
  - [x] `GET /api/v1/batches/{id}/events/verify` - Kiểm tra tính toàn vẹn hash chain
- [x] **Event permission**: kiểm tra OrganizationType có được tạo event type đó không

### 2.6 Batch Split & Merge
- [x] `POST /api/v1/batches/{id}/split` - Chia lô
- [x] `POST /api/v1/batches/merge` - Gộp lô
- [x] Cập nhật RemainingQuantity, tạo Batch con, ghi event SPLIT/MERGE

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
- [ ] Unit Tests cho Domain entities (guard clauses, business rules)
- [ ] Unit Tests cho HashChainService
- [ ] Unit Tests cho Application handlers (Commands/Queries)
- [ ] Unit Tests cho API Controllers
- [ ] Coverage tối thiểu 60% logic nghiệp vụ

---

## Sprint 3: Nâng cao & Triển khai

### 3.1 Quality Inspection & Certificate
- [x] `POST /api/v1/batches/{id}/inspections` - Tạo kiểm định
- [x] `GET /api/v1/batches/{id}/inspections` - Danh sách kiểm định
- [x] `GET /api/v1/inspections/{id}` - Chi tiết kiểm định
- [x] `PUT /api/v1/inspections/{id}` - Cập nhật
- [x] `POST /api/v1/batches/{id}/certificates` - Cấp chứng nhận
- [x] `GET /api/v1/batches/{id}/certificates` - Danh sách chứng nhận
- [x] `DELETE /api/v1/certificates/{id}` - Thu hồi chứng nhận
- [ ] Upload file chứng nhận (PDF/image) (endpoint stub, chưa lưu file thật)

### 3.2 Recall Management (Thu hồi)
- [x] `POST /api/v1/recalls` - Tạo recall
- [x] `GET /api/v1/recalls` - Danh sách
- [x] `GET /api/v1/recalls/{id}` - Chi tiết
- [x] `PATCH /api/v1/recalls/{id}/resolve` - Kết thúc recall
- [x] Tìm tổ chức liên quan và gửi notification
- [x] Cập nhật trạng thái batch → RECALLED

### 3.3 Public Traceability (Tra cứu công khai) ⭐
- [x] `GET /api/v1/public/trace/{batchId}` - Tra cứu batch (không cần auth)
- [x] `GET /api/v1/public/trace/{batchId}/lineage` - Phả hệ batch (split/merge history)
- [x] Timeline view: hiển thị dòng thời gian events
- [x] Hiển thị certificates, inspections, recall status
- [ ] **Redis Cache** cho public trace (TTL 5 phút)
- [ ] **Frontend Public Portal**: trang tra cứu tối ưu mobile, quét QR

### 3.4 Notification System
- [x] `GET /api/v1/notifications` - Danh sách thông báo
- [x] `PATCH /api/v1/notifications/{id}/read` - Đánh dấu đã đọc
- [x] `PATCH /api/v1/notifications/read-all` - Đánh dấu tất cả
- [x] `GET /api/v1/notifications/unread-count` - Số chưa đọc
- [x] Tự động gửi notification khi có recall

### 3.5 Analytics Dashboard
- [x] `GET /api/v1/analytics/overview` - Dashboard tổng quan
- [x] `GET /api/v1/analytics/batch-distribution` - Thống kê batch theo trạng thái
- [x] `GET /api/v1/analytics/processing-time` - Thời gian xử lý trung bình
- [x] `GET /api/v1/analytics/traceback/{batchId}` - Truy vết ngược

### 3.6 Lookup APIs
- [x] `GET /api/v1/roles`
- [x] `GET /api/v1/organization-types`
- [x] `GET /api/v1/event-types`
- [x] `GET /api/v1/units`
- [x] `GET /api/v1/inspection-results`
- [x] `GET /api/v1/certificate-types`
- [x] `GET /api/v1/recall-severities`

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

## User Stories (Backend) — cần làm / theo dõi

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
- [ ] **US-AUTH-07** — Là người dùng, tôi muốn quên mật khẩu và đặt lại qua email (forgot/reset password).

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
- [ ] **US-MGR-05** — Là Manager, tôi muốn giới hạn dữ liệu chỉ thấy được của tổ chức mình (data scoping/tenant filter theo OrganizationId).
- [ ] **US-MGR-06** — Là Manager, tôi muốn upload hình ảnh sản phẩm/batch thật (lưu trữ file/cloud storage).

### Staff / Farmer (nghiệp vụ theo Organization Type)
- [x] **US-STF-01** — Là Staff, tôi muốn tạo lô hàng mới (Batch) kèm sinh BatchCode + QR Code.
- [x] **US-STF-02** — Là Staff, tôi muốn ghi nhận sự kiện chuỗi cung ứng (Event) cho lô hàng.
- [x] **US-STF-03** — Là Staff, tôi muốn chỉ được tạo loại event phù hợp với Organization Type (event permission).
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
- [ ] **US-INS-06** — Là Inspector, tôi muốn inspectorId được lấy tự động từ JWT thay vì truyền trong body.

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
- [ ] **US-QA-01** — Là dev, tôi muốn có Unit Test cho Domain entities & HashChainService để đảm bảo business rule.
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