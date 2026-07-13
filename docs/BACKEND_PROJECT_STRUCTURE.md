# Kiến trúc Clean Architecture & CQRS (.NET 10) — AgriTrace

Tài liệu này mô tả kiến trúc, cách phân chia các tầng (Layers) và luồng luân chuyển dữ liệu trong hệ thống **Agricultural Supply Chain Traceability System**. Dự án tuân thủ nghiêm ngặt nguyên tắc **Clean Architecture**, tách biệt mã nguồn nghiệp vụ khỏi các công nghệ phụ thuộc bên ngoài.

---

## Tổng quan hệ thống

Hệ thống quản lý toàn bộ vòng đời của lô nông sản (Batch) từ Farm → Processing → Packaging → Transportation → Distribution → Retail → Consumer. Mỗi Batch có mã định danh duy nhất và mã QR. Mọi hoạt động trên Batch đều được ghi nhận dưới dạng **SupplyChainEvent** với cơ chế **Append-only + SHA-256 Hash Chain** để đảm bảo tính toàn vẹn dữ liệu.

### Actors
| Actor | Vai trò |
|-------|---------|
| System Admin | Quản trị hệ thống, tổ chức, người dùng |
| Organization Admin | Quản lý tổ chức, sản phẩm, batch, báo cáo |
| Staff | Nhân viên vận hành (tạo event, split/merge batch) |
| Inspector | Kiểm định chất lượng, cấp chứng nhận |
| Consumer | Tra cứu công khai qua QR (không cần đăng nhập) |

---

## Sơ đồ cấu trúc thư mục (Directory Structure)

```
AgriTrace.slnx
│
├── AgriTrace.Domain/                           # 1. Tầng Lõi (Domain Layer)
│   ├── Common/
│   │   ├── BaseEntity.cs                       # Id, CreatedAt, UpdatedAt
│   │   └── PagedResult.cs                      # Generic paged container
│   ├── Entities/
│   │   ├── Organization.cs                     # Tổ chức tham gia chuỗi
│   │   ├── OrganizationType.cs                 # Value object: FARM, PROCESSOR...
│   │   ├── User.cs                             # Người dùng (JWT auth)
│   │   ├── Product.cs                          # Sản phẩm nông sản
│   │   ├── Category.cs                         # Danh mục sản phẩm
│   │   ├── Unit.cs                             # Đơn vị tính (kg, ton...)
│   │   ├── Batch.cs                            # *** Aggregate Root trung tâm ***
│   │   ├── SupplyChainEvent.cs                 # *** Append-only event ***
│   │   ├── EventType.cs                        # Value object: HARVEST, TRANSPORT...
│   │   ├── QualityInspection.cs                # Kiểm định chất lượng
│   │   ├── Certificate.cs                      # Chứng nhận
│   │   ├── Recall.cs                           # Thu hồi sản phẩm
│   │   ├── BatchSplit.cs                       # Chia lô
│   │   ├── BatchMerge.cs                       # Gộp lô
│   │   └── Notification.cs                     # Thông báo
│   ├── Interfaces/
│   │   ├── IOrganizationRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── IProductRepository.cs
│   │   ├── IBatchRepository.cs
│   │   ├── IEventRepository.cs
│   │   ├── IInspectionRepository.cs
│   │   ├── ICertificateRepository.cs
│   │   ├── IRecallRepository.cs
│   │   └── INotificationRepository.cs
│   └── Services/
│       ├── BatchService.cs                     # Tạo batch, split, merge
│       ├── HashChainService.cs                 # SHA-256 hash chain
│       ├── EventService.cs                     # Ghi nhận event + kiểm tra quyền
│       └── RecallService.cs                    # Thu hồi + notification
│
├── AgriTrace.Application/                      # 2. Tầng Nghiệp vụ (Application Layer)
│   ├── Common/Exceptions/
│   │   ├── NotFoundException.cs                # → 404
│   │   ├── ValidationException.cs              # → 400
│   │   ├── ConflictException.cs                # → 409
│   │   └── BusinessException.cs                # → 422
│   ├── Contracts/                              # DTOs
│   │   ├── OrganizationDto.cs
│   │   ├── UserDto.cs
│   │   ├── ProductDto.cs
│   │   ├── BatchDto.cs
│   │   ├── SupplyChainEventDto.cs
│   │   ├── QualityInspectionDto.cs
│   │   ├── CertificateDto.cs
│   │   ├── RecallDto.cs
│   │   └── PaginationResponse.cs
│   ├── Features/                               # CQRS - phân chia theo module
│   │   ├── Auth/
│   │   │   ├── Commands/LoginCommand.cs
│   │   │   └── Queries/GetCurrentUserQuery.cs
│   │   ├── Organizations/
│   │   │   ├── Commands/{Create,Update}OrganizationCommand.cs
│   │   │   └── Queries/{Get,GetById}OrganizationsQuery.cs
│   │   ├── Users/
│   │   │   ├── Commands/{Create,Update}UserCommand.cs
│   │   │   └── Queries/{Get,GetById}UsersQuery.cs
│   │   ├── Products/
│   │   │   ├── Commands/{Create,Update,Delete}ProductCommand.cs
│   │   │   └── Queries/{Get,GetById}ProductsQuery.cs
│   │   ├── Batches/
│   │   │   ├── Commands/{Create,Update,Split,Merge}BatchCommand.cs
│   │   │   └── Queries/{Get,GetById,GetLineage}BatchQuery.cs
│   │   ├── Events/
│   │   │   ├── Commands/CreateEventCommand.cs
│   │   │   └── Queries/{GetEvents,VerifyHashChain}Query.cs
│   │   ├── Inspections/
│   │   ├── Certificates/
│   │   ├── Recalls/
│   │   ├── Notifications/
│   │   ├── Analytics/
│   │   └── PublicTrace/
│   ├── Mappings/
│   │   └── ApplicationMappingRegister.cs       # Mapster: Entity → DTO
│   ├── Services/
│   │   ├── ICurrentUserService.cs              # Lấy user từ JWT claims
│   │   └── ITokenService.cs                    # Sinh/verify JWT
│   └── DependencyInjection.cs                  # MediatR + Mapster + Services
│
├── AgriTrace.Infrastructure.Sqlserver/         # 3. Tầng Hạ tầng (Infrastructure Layer)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs             # EF Core DbContext
│   │   └── SeedData.cs                         # Seed: OrganizationTypes, EventTypes...
│   ├── Configurations/                         # Fluent API cho từng bảng
│   │   ├── OrganizationConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── BatchConfiguration.cs
│   │   ├── SupplyChainEventConfiguration.cs
│   │   ├── QualityInspectionConfiguration.cs
│   │   ├── CertificateConfiguration.cs
│   │   ├── RecallConfiguration.cs
│   │   ├── BatchSplitConfiguration.cs
│   │   ├── BatchMergeConfiguration.cs
│   │   └── NotificationConfiguration.cs
│   ├── Models/                                 # Data Models (ánh xạ 1-1 với bảng)
│   │   ├── OrganizationDataModel.cs
│   │   ├── UserDataModel.cs
│   │   ├── ProductDataModel.cs
│   │   ├── BatchDataModel.cs
│   │   ├── SupplyChainEventDataModel.cs
│   │   ├── QualityInspectionDataModel.cs
│   │   ├── CertificateDataModel.cs
│   │   ├── RecallDataModel.cs
│   │   ├── BatchSplitDataModel.cs
│   │   ├── BatchMergeDataModel.cs
│   │   ├── NotificationDataModel.cs
│   │   └── RefreshTokenDataModel.cs
│   ├── Migrations/                             # EF Core migrations
│   ├── Repositories/                           # Implement Interfaces từ Domain
│   │   ├── OrganizationRepository.cs
│   │   ├── UserRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── BatchRepository.cs
│   │   ├── EventRepository.cs
│   │   ├── InspectionRepository.cs
│   │   ├── CertificateRepository.cs
│   │   ├── RecallRepository.cs
│   │   └── NotificationRepository.cs
│   └── DependencyInjection.cs                  # DbContext + Repositories
│
└── AgriTrace.API/                              # 4. Tầng Giao tiếp (Presentation Layer)
    ├── Controllers/
    │   ├── AuthController.cs                   # Login, Refresh, Logout
    │   ├── OrganizationsController.cs          # CRUD tổ chức
    │   ├── UsersController.cs                  # CRUD người dùng
    │   ├── ProductsController.cs               # CRUD sản phẩm
    │   ├── BatchesController.cs                # CRUD batch + split/merge
    │   ├── EventsController.cs                 # Event + hash verify
    │   ├── InspectionsController.cs
    │   ├── CertificatesController.cs
    │   ├── RecallsController.cs
    │   ├── NotificationsController.cs
    │   ├── AnalyticsController.cs
    │   ├── PublicTraceController.cs            # *** Public (không auth) ***
    │   └── LookupController.cs                 # Roles, types, units...
    ├── Models/                                 # Request/Response
    │   ├── Auth/
    │   │   ├── LoginRequest.cs
    │   │   └── TokenResponse.cs
    │   ├── Organizations/
    │   │   ├── OrganizationRequest.cs
    │   │   └── OrganizationResponse.cs
    │   ├── Users/
    │   ├── Products/
    │   ├── Batches/
    │   ├── Events/
    │   ├── Inspections/
    │   ├── Certificates/
    │   ├── Recalls/
    │   ├── Notifications/
    │   ├── Analytics/
    │   ├── PublicTrace/
    │   ├── ApiResponse.cs                      # Response envelope chuẩn
    │   └── PaginationRequest.cs
    ├── Common/
    │   ├── ApiResponseWrapperFilter.cs          # Auto-wrap response
    │   └── GlobalExceptionHandler.cs            # 400, 404, 409, 422, 500
    ├── Mappings/
    │   └── ApiMappingRegister.cs                # Mapster: Request → Command, DTO → Response
    ├── Middleware/
    │   └── RequestLoggingMiddleware.cs
    ├── DependencyInjection.cs                   # Controllers, Swagger, CORS
    ├── Program.cs                               # Entry point (tối giản)
    └── appsettings.json
```

---

## Vai trò và Quy tắc của từng Tầng (Layer Responsibilities)

### 1. Tầng Domain (Lõi trung tâm)

- **Nhiệm vụ:** Chứa các thực thể (Entities), value objects, business rules, domain services cốt lõi.
- **Quy tắc:** Tuyệt đối không phụ thuộc vào bất kỳ tầng nào khác hoặc thư viện bên ngoài. Các thuộc tính đóng gói (`private set`) để bảo vệ tính toàn vẹn dữ liệu.
- **Entities chính:**
  - `Batch` — Aggregate Root trung tâm (quản lý số lượng, trạng thái, tổ chức hiện tại)
  - `SupplyChainEvent` — Append-only, có PreviousHash/CurrentHash
  - `Organization`, `User`, `Product`, `QualityInspection`, `Recall`, v.v.

### 2. Tầng Application (Ứng dụng / Nghiệp vụ)

- **Nhiệm vụ:** Định nghĩa use-cases qua CQRS (MediatR Commands/Queries), DTOs, validation.
- **Quy tắc:** Chỉ phụ thuộc vào tầng **Domain**.
- **Cấu trúc:** Mỗi module nghiệp vụ (Organizations, Users, Batches, Events...) có riêng `Commands/` và `Queries/`.

### 3. Tầng Infrastructure.Sqlserver (Hạ tầng dữ liệu)

- **Nhiệm vụ:** Hiện thực hóa các Interface lưu trữ bằng EF Core + SQL Server.
- **Quy tắc:** Phụ thuộc vào tầng **Domain**. Data Model tách biệt với Domain Entity. Repository tự ánh xạ thủ công giữa DataModel ↔ Entity.
- **Hash Chain:** EventRepository đảm bảo append-only, không cho phép UPDATE/DELETE.

### 4. Tầng API / Presentation (Giao tiếp)

- **Nhiệm vụ:** Tiếp nhận HTTP request, authentication (JWT), authorization (RBAC), response formatting.
- **Quy tắc:** Phụ thuộc vào tầng **Application** (gửi Command/Query qua MediatR) và **Infrastructure** (DI tại Program.cs).
- **Public endpoints:** `/api/v1/public/trace/...` không yêu cầu auth, phục vụ consumer tra cứu QR.

---

## Luồng dữ liệu và Mapping (Data Flow)

```
[Client] ──(JSON)──► [Request Model] (API)
                            │
                     (Mapster Mapping)
                            ▼
                    [Command/Query] (Application — MediatR)
                            │
                     (Handler xử lý)
                            ▼
                    [Domain Entity] (Domain — business logic)
                            │
                     (Repository lưu)
                            ▼
                    [DataModel] (Infrastructure — EF Core → SQL Server)
                            │
                     (Truy vấn ngược)
                            ▼
                    [Dto] (Application — ra khỏi business)
                            │
                     (Mapster Mapping)
                            ▼
[Client] ◄──(JSON)── [Response Model] (API)
```

### Authentication Flow
```
[Client] ──POST /auth/login──► [AuthController]
                                      │
                               (Validate credentials)
                                      ▼
                              [User entity] (Domain)
                                      │
                               (Generate JWT)
                                      ▼
                         AccessToken + RefreshToken
                          (có: sub, email, role,
                           organizationId, organizationType)
```

### Hash Chain Flow (Ghi nhận Event)
```
1. Lấy event gần nhất của Batch → CurrentHash
2. Gán PreviousHash = CurrentHash đó
3. Tính CurrentHash = SHA256(PreviousHash + EventData)
4. Lưu event (INSERT — không UPDATE/DELETE)
5. Nếu sửa event cũ → Hash Chain bị gãy → hệ thống phát hiện
```

---

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|------------|-----------|
| Runtime | .NET 10 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server 2022 |
| CQRS | MediatR |
| Mapping | Mapster |
| Authentication | JWT Bearer (Access + Refresh Token) |
| Hashing | SHA-256 |
| Caching | Redis (cho public trace) |
| Container | Docker + docker-compose |
| Testing | xUnit + Moq |

## Dependency Injection

Mỗi tầng tự đóng gói dịch vụ nội bộ qua `DependencyInjection.cs`:

```csharp
// Program.cs — tối giản
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);
```

- `AddPresentation()`: Controllers, Swagger, CORS, API Mapper, Exception Handler, Response Wrapper
- `AddApplication()`: MediatR (scan handlers), Mapster global config, Application Services
- `AddInfrastructureSqlServer()`: DbContext (DefaultConnection), Repositories, Seed Data
