# Kiến trúc Clean Architecture — AgriTrace Backend

Tài liệu này mô tả kiến trúc, cách phân chia các tầng (Layers) và luồng luân chuyển dữ liệu trong hệ thống **Agricultural Supply Chain Traceability System**. Dự án tuân thủ nguyên tắc **Clean Architecture**, tách biệt mã nguồn nghiệp vụ khỏi các công nghệ phụ thuộc bên ngoài và áp dụng mẫu **CQRS (MediatR)** cho xử lý luồng ghi/đọc.

> **Trạng thái hiện tại (Phiên bản mới nhất):** Toàn bộ 16 API Controller, CQRS Commands & Queries, DTO Mappings (Mapster), Validations (FluentValidation), Domain Services và Infrastructure Repositories đã được nối thông suốt end-to-end (Controller → CQRS Handler → Domain Service → EF Core Repository → SQL Server). Hệ thống đã hoàn thiện Authentication (JWT Access/Refresh token), Phân quyền 2-Layer (Role + OrgType/EventType matrix), Chuỗi băm SHA-256 (Append-only Event Ledger), Tra cứu công khai Public Trace/Lineage, và Báo cáo Analytics.

---

## Tổng quan hệ thống

Hệ thống quản lý toàn bộ vòng đời của lô nông sản (Batch) từ Farm → Processing → Packaging → Transportation → Distribution → Retail → Consumer. Mỗi Batch có mã định danh duy nhất và QR Code truy xuất. Mọi hoạt động trên Batch đều được ghi nhận dưới dạng **SupplyChainEvent** với cơ chế **Append-only + SHA-256 Hash Chain** để đảm bảo tính toàn vẹn dữ liệu.

### Actors & Roles
| Role | Mã Role | Vai trò |
|---|---|---|
| Admin | `Admin` | Quản trị hệ thống toàn quyền, quản lý tổ chức, người dùng, khởi tạo Recall toàn hệ thống |
| Manager | `Manager` | Quản lý nội bộ tổ chức (nhân sự, sản phẩm, lô hàng, báo cáo) |
| Staff | `Staff` | Nhân viên vận hành (tạo event theo phân quyền OrgType, split/merge batch) |
| Inspector | `Inspector` | Đơn vị kiểm định chất lượng (lập phiếu kiểm định QualityInspection, cấp/thu hồi Certificate) |
| Consumer | `Consumer` | Người tiêu dùng tra cứu công khai qua QR / Web Portal (không cần đăng nhập) |

---

## Mô hình kiến trúc thực tế

Hệ thống kết hợp **Clean Architecture** và **CQRS (Command Query Responsibility Segregation)**:

- **Domain**: Định nghĩa nghiệp vụ cốt lõi qua các **Domain Service** (interface `I*Service`) và các **Repository interface** (`I*Repository`) hướng ra ngoài (`Outbound`). Tuyệt đối không phụ thuộc vào bất kỳ thư viện bên ngoài nào.
- **Application**: Chịu trách nhiệm đăng ký DI (MediatR, Mapster, FluentValidation, Domain Services), chứa các DTOs (`Contracts/`), CQRS Commands/Queries (`Features/`) và pipeline validation behaviors.
- **Infrastructure.Sqlserver**: Hiện thực Repository bằng EF Core 10, ánh xạ 1-1 giữa `DataModel` và `Entity`, Fluent API Configuration, Migrations và Seed Data.
- **API**: Nhận HTTP request, điều phối qua MediatR Command/Query hoặc Domain Service qua Dependency Injection, và bọc kết quả trả về envelope `ApiResponse` chuẩn.

---

## Sơ đồ cấu trúc thư mục (Directory Structure)

```
AgriTrace.slnx
│
├── AgriTrace.Domain/                           # 1. Tầng Lõi (Domain Layer)
│   ├── Common/
│   │   ├── BaseEntity.cs                       # Id (Guid), CreatedAt, UpdatedAt, MarkUpdated()
│   │   ├── PagedResult.cs                      # Generic paged container
│   │   ├── UserRole.cs                         # Enum: Admin, Manager, Staff, Farmer, Processor...
│   │   └── Enums/                              # BatchStatus, InspectionStatus, OrganizationStatus, RecallSeverity, RecallStatus
│   ├── Entities/                               # 18 Domain Entities (Batch aggregate root, SupplyChainEvent append-only, User, Organization...)
│   ├── Interfaces/
│   │   ├── Inbound/                            # Domain Service Interfaces (ICategoryService, IBatchService, IEventService...)
│   │   └── Outbound/                           # Repository Interfaces (ICategoryRepository, IBatchRepository, ISupplyChainEventRepository...)
│   └── Services/                               # Hiện thực Domain Services (HashChainService, BatchService, EventService...)
│
├── AgriTrace.Application/                      # 2. Tầng Nghiệp Vụ (Application Layer)
│   ├── Common/
│   │   ├── Behaviors/ValidationBehavior.cs     # FluentValidation MediatR pipeline behavior
│   │   └── Exceptions/                         # NotFoundException (404), ConflictException (409)...
│   ├── Contracts/                              # DTOs (Request/Response models cho từng feature)
│   ├── Features/                               # CQRS Commands & Queries (16 Modules)
│   │   ├── Auth/                               # LoginCommand, RefreshTokenCommand, ChangePasswordCommand...
│   │   ├── Batches/                            # CreateBatchCommand, GetBatchByIdQuery, GetBatchesPagedQuery...
│   │   ├── Categories/                         # CreateCategoryCommand, UpdateCategoryCommand, GetCategoriesQuery...
│   │   ├── Events/                             # CreateEventCommand, VerifyEventHashChainQuery...
│   │   ├── EventRequests/                      # CreateEventRequestCommand, ApproveEventRequestCommand...
│   │   ├── Inspections/                        # CreateInspectionCommand, UpdateInspectionCommand...
│   │   ├── Organizations/                      # CreateOrganizationCommand, UpdateOrganizationCommand...
│   │   ├── Products/                           # CreateProductCommand, UpdateProductCommand...
│   │   ├── Recalls/                            # CreateRecallCommand, ResolveRecallCommand...
│   │   └── Users/                              # CreateUserCommand, UpdateUserCommand...
│   ├── Mappings/                               # Mapster IRegister mapping profiles
│   └── DependencyInjection.cs                  # MediatR + FluentValidation + Mapster + Services DI
│
├── AgriTrace.Infrastructure.Sqlserver/         # 3. Tầng Hạ Tầng (Infrastructure Layer)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs             # EF Core DbContext
│   │   └── SeedData.cs                         # Master & Test Seed Data (PasswordHash PBKDF2 chuẩn)
│   ├── Configurations/                         # Fluent API Configurations (17 tables)
│   ├── Models/                                 # Data Models (17 DataModels 1-1 với DB tables)
│   ├── Migrations/                             # EF Core DB Migrations
│   ├── Repositories/                           # Hiện thực Repositories cho EF Core SQL Server
│   └── DependencyInjection.cs                  # DbContext + Repositories DI
│
├── AgriTrace.API/                              # 4. Tầng Giao Tiếp (Presentation Layer)
│   ├── Controllers/                            # 16 Production Controllers
│   │   ├── AnalyticsController.cs              # Dashboard thống kê, traceback
│   │   ├── AuthController.cs                   # Login, Logout, Refresh, Profile, ChangePassword
│   │   ├── BatchSplitMergeController.cs        # Split / Merge lô hàng
│   │   ├── BatchesController.cs                # CRUD Batch + QR Code generator
│   │   ├── CategoriesController.cs             # CRUD Categories
│   │   ├── CertificatesController.cs           # Cấp & thu hồi chứng nhận
│   │   ├── EventRequestsController.cs          # Đăng ký & duyệt sự kiện
│   │   ├── EventsController.cs                 # Ghi nhận event & Hash chain verify
│   │   ├── InspectionsController.cs            # Kiểm định chất lượng
│   │   ├── LookupController.cs                 # Master data lookup endpoints
│   │   ├── NotificationsController.cs          # Thông báo người dùng
│   │   ├── OrganizationsController.cs          # Quản lý tổ chức
│   │   ├── ProductsController.cs               # Quản lý sản phẩm
│   │   ├── PublicController.cs                 # Tra cứu công khai public trace & lineage
│   │   ├── RecallsController.cs                # Quản lý lệnh thu hồi
│   │   └── UsersController.cs                  # Quản lý người dùng & phân quyền
│   ├── Common/
│   │   ├── ApiResponseWrapperFilter.cs         # Auto-wrap response dạng ApiResponse<T>
│   │   ├── GlobalExceptionHandler.cs           # Global Exception Handler middleware
│   │   └── CurrentUserService.cs               # Extract Claims từ Bearer JWT
│   ├── Swagger/
│   │   └── BearerSecurityRequirementDocumentFilter.cs # Custom Swagger Bearer auth filter
│   ├── Program.cs                              # WebApplication Entry point
│   └── appsettings.json                        # ConnectionStrings & JWT Settings
│
└── AgriTrace.Tests/                            # 5. Tầng Kiểm Thử (Test Layer)
```

---

## Luồng Luân Chuyển Dữ Liệu (End-to-End Data Flow)

```
[HTTP Request Client]
        │
        ▼ (JWT Bearer Auth Middleware)
[API Controller Endpoint]
        │
        ▼ (Send Command / Query via MediatR)
[ValidationBehavior (FluentValidation)]
        │
        ▼ (Handler Call)
[CQRS Command/Query Handler]
        │
        ▼ (Domain Logic Execution)
[Domain Service] (e.g. HashChainService / BatchService)
        │
        ▼ (Outbound Repository Call)
[EF Core Repository] (Infrastructure)
        │
        ▼ (EF Core Mapping Entity ↔ DataModel)
[SQL Server Database]
        │
        ▼ (Mapster DTO Mapping)
[ApiResponse Envelope Wrapper Filter]
        │
        ▼ (HTTP 200/400/404/500 JSON Response)
[Client / Frontend]
```

### Chuẩn Trả Về Response (ApiResponse Envelope)
Tất cả các API endpoint đều sử dụng chung một chuẩn bọc dữ liệu:
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "errorMessages": [],
  "result": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "batchCode": "BTH-2026-001",
    "quantity": 1000.0,
    "status": "HARVESTED"
  }
}
```

---

## Ma Trận Phân Quyền Event (Layer 2 Permission Matrix)

Tầng Application kiểm tra quyền ghi sự kiện `SupplyChainEvent` theo loại tổ chức (`OrganizationType`):

| OrganizationType | HARVEST | RECEIVE | PROCESSING | PACKAGING | TRANSPORT | DISTRIBUTION | RETAIL | INSPECTION | SPLIT | MERGE | RECALL |
|---|---|---|---|---|---|---|---|---|---|---|---|
| `FARM` | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `PROCESSOR` | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| `DISTRIBUTOR` | ❌ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ |
| `RETAILER` | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| `INSPECTION` | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| `SYSTEM` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

---

## Đăng Ký Dependency Injection (Program.cs & Extension Methods)

Solution được cấu hình gọn gàng qua các extension methods:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

var app = builder.Build();
app.UseCustomExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

- **`AddApplication()`**: Đăng ký MediatR cho tất cả handlers trong assembly, đăng ký FluentValidation behaviors, Mapster global configuration, và toàn bộ 17 Domain Services.
- **`AddInfrastructureSqlServer()`**: Đăng ký `ApplicationDbContext` với SQL Server connection string và toàn bộ 17 Repositories.
- **`AddPresentation()`**: Đăng ký Controllers, `ApiResponseWrapperFilter`, `CurrentUserService`, và Swagger UI với custom `BearerSecurityRequirementDocumentFilter`.
