# Kiến trúc Clean Architecture — AgriTrace

Tài liệu này mô tả kiến trúc, cách phân chia các tầng (Layers) và luồng luân chuyển dữ liệu trong hệ thống **Agricultural Supply Chain Traceability System**. Dự án tuân thủ nguyên tắc **Clean Architecture**, tách biệt mã nguồn nghiệp vụ khỏi các công nghệ phụ thuộc bên ngoài.

> **Trạng thái hiện tại (2026-07-14):** Khung kiến trúc (4 tầng) đã được dựng xong cùng các interface/giả lập cho toàn bộ thực thể. Module **Categories** được nối thông suốt end-to-end (Controller → CQRS Feature → Domain Service → Repository → SQL Server). Các module còn lại (Batches, Events, Organizations, Products, v.v.) đã có Entity / Service / Repository / Configuration tương ứng nhưng **chưa đăng ký vào DI** và **chưa có Controller** — đang được bổ sung dần theo cùng một khuôn mẫu.

---

## Tổng quan hệ thống

Hệ thống quản lý toàn bộ vòng đời của lô nông sản (Batch) từ Farm → Processing → Packaging → Transportation → Distribution → Retail → Consumer. Mỗi Batch có mã định danh duy nhất. Mọi hoạt động trên Batch đều được ghi nhận dưới dạng **SupplyChainEvent** với cơ chế **Append-only + SHA-256 Hash Chain** để đảm bảo tính toàn vẹn dữ liệu.

### Actors
| Actor | Vai trò |
|-------|---------|
| Admin | Quản trị hệ thống, tổ chức, người dùng |
| Manager | Quản lý tổ chức, sản phẩm, batch, báo cáo |
| Staff | Nhân viên vận hành (tạo event, split/merge batch) |
| Inspector | Kiểm định chất lượng, cấp chứng nhận |
| Consumer | Tra cứu công khai qua QR (không cần đăng nhập) |

---

## Mô hình kiến trúc thực tế

Hệ thống dùng mẫu **Service-Oriented Clean Architecture**:

- **Domain** định nghĩa nghiệp vụ cốt lõi qua các **Domain Service** (interface `I*Service`) và các **Repository interface** (`I*Repository`) hướng ra ngoài (`Outbound`).
- **Application** chịu trách nhiệm đăng ký DI (MediatR, Mapster, Domain Services), chứa các DTOs (`Contracts/`) và các CQRS Features (`Features/`).
- **Infrastructure.Sqlserver** hiện thực Repository bằng EF Core, ánh xạ thủ công giữa `DataModel` và `Entity`.
- **API** nhận HTTP request, gọi trực tiếp Domain Service qua constructor injection, và trả về envelope `ApiResponse` chuẩn.

> Lưu ý: `MediatR` đã được đăng ký trong `AddApplication()` và **đã có Command/Query cho module Categories**. Các module khác sẽ được bổ sung CQRS Features dần. Hiện tại Categories vẫn gọi Domain Service trực tiếp từ Controller.

### Sơ đồ cấu trúc thư mục (Directory Structure)

```
AgriTrace.slnx
│
├── AgriTrace.Domain/                           # 1. Tầng Lõi (Domain Layer)
│   ├── Common/
│   │   ├── BaseEntity.cs                       # Id (Guid), CreatedAt, UpdatedAt, MarkUpdated()
│   │   ├── PagedResult.cs                      # Generic paged container (Items, TotalCount, ...)
│   │   ├── UserRole.cs                         # Enum: vai trò người dùng
│   │   └── Enums/                              # BatchStatus, InspectionStatus,
│   │       ├── BatchStatus.cs                  #   OrganizationStatus, RecallSeverity, RecallStatus
│   │       ├── InspectionStatus.cs
│   │       ├── OrganizationStatus.cs
│   │       ├── RecallSeverity.cs
│   │       └── RecallStatus.cs
│   ├── Entities/
│   │   ├── Organization.cs                     # Tổ chức tham gia chuỗi
│   │   ├── OrganizationType.cs                 # Loại tổ chức (FARM, PROCESSOR...)
│   │   ├── User.cs                             # Người dùng
│   │   ├── Product.cs                          # Sản phẩm nông sản
│   │   ├── Category.cs                         # Danh mục sản phẩm
│   │   ├── Unit.cs                             # Đơn vị tính (kg, ton...)
│   │   ├── Batch.cs                            # *** Aggregate Root trung tâm ***
│   │   ├── SupplyChainEvent.cs                 # *** Append-only event (PreviousHash/CurrentHash) ***
│   │   ├── EventType.cs                        # Loại sự kiện (HARVEST, TRANSPORT...)
│   │   ├── QualityInspection.cs                # Kiểm định chất lượng
│   │   ├── Certificate.cs                      # Chứng nhận
│   │   ├── Recall.cs                           # Thu hồi sản phẩm
│   │   ├── BatchSplit.cs                       # Chia lô
│   │   ├── BatchSplitDetail.cs                 # Chi tiết chia lô
│   │   ├── BatchMerge.cs                       # Gộp lô
│   │   ├── BatchMergeSource.cs                 # Nguồn gộp lô
│   │   └── Notification.cs                     # Thông báo
│   ├── Interfaces/
│   │   ├── Inbound/                            # Interface dịch vụ nghiệp vụ (do API gọi)
│   │   │   ├── IService.cs                     # IService<TEntity,TKey> : IReadService, IWriteService
│   │   │   ├── IReadService.cs                 # GetById, GetAll, GetPaged
│   │   │   ├── IWriteService.cs                # Add, Update, Delete
│   │   │   ├── ISupplyChainEventService.cs     # Service cho SupplyChainEvent
│   │   │   ├── ICategoryService.cs
│   │   │   ├── IProductService.cs
│   │   │   ├── IBatchService.cs
│   │   │   ├── IBatchSplitService.cs
│   │   │   ├── IBatchMergeService.cs
│   │   │   ├── IEventService.cs
│   │   │   ├── IEventTypeService.cs
│   │   │   ├── IOrganizationService.cs
│   │   │   ├── IOrganizationTypeService.cs
│   │   │   ├── IQualityInspectionService.cs
│   │   │   ├── ICertificateService.cs
│   │   │   ├── IRecallService.cs
│   │   │   ├── INotificationService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── IUnitService.cs
│   │   │   └── IHashChainService.cs
│   │   └── Outbound/                           # Interface lưu trữ (do Infrastructure hiện thực)
│   │       ├── IRepository.cs                  # IRepository<TEntity,TKey> : IReadRepository, IWriteRepository
│   │       ├── IReadRepository.cs              # GetById, GetAll, GetPaged
│   │       ├── IWriteRepository.cs             # Add, Update, Delete
│   │       ├── ISupplyChainEventRepository.cs  # Repository cho SupplyChainEvent
│   │       ├── ICategoryRepository.cs
│   │       ├── IProductRepository.cs
│   │       ├── IBatchRepository.cs
│   │       ├── IBatchSplitRepository.cs
│   │       ├── IBatchMergeRepository.cs
│   │       ├── IEventRepository.cs
│   │       ├── IEventTypeRepository.cs
│   │       ├── IOrganizationRepository.cs
│   │       ├── IOrganizationTypeRepository.cs
│   │       ├── IQualityInspectionRepository.cs
│   │       ├── ICertificateRepository.cs
│   │       ├── IRecallRepository.cs
│   │       ├── INotificationRepository.cs
│   │       ├── IUserRepository.cs
│   │       └── IUnitRepository.cs
│   └── Services/                               # Hiện thực Inbound interfaces
│       ├── CategoryService.cs                  # ✓ đã nối DI (module tham chiếu)
│       ├── ProductService.cs
│       ├── BatchService.cs
│       ├── BatchSplitService.cs
│       ├── BatchMergeService.cs
│       ├── EventService.cs
│       ├── EventTypeService.cs
│       ├── OrganizationService.cs
│       ├── OrganizationTypeService.cs
│       ├── QualityInspectionService.cs
│       ├── CertificateService.cs
│       ├── RecallService.cs
│       ├── NotificationService.cs
│       ├── UserService.cs
│       ├── UnitService.cs
│       └── HashChainService.cs                 # SHA-256 hash chain
│
├── AgriTrace.Application/                      # 2. Tầng Nghiệp vụ (Application Layer)
│   ├── Common/Exceptions/
│   │   ├── NotFoundException.cs                # → 404
│   │   └── ConflictException.cs                # → 409
│   ├── Contracts/                              # DTOs (Data Transfer Objects)
│   │   └── CategoryDto.cs                      # Category response DTO
│   ├── Features/                               # CQRS Commands & Queries (MediatR)
│   │   └── Categories/
│   │       ├── Commands/
│   │       │   ├── CreateCategoryCommand.cs
│   │       │   ├── UpdateCategoryCommand.cs
│   │       │   ├── UpdateCategoryStatusCommand.cs
│   │       │   └── DeleteCategoryCommand.cs
│   │       └── Queries/
│   │           ├── GetCategoryByIdQuery.cs
│   │           └── GetCategoriesPagedQuery.cs
│   ├── Mappings/
│   │   └── ApplicationMappingRegister.cs       # Mapster IRegister (hiện để trống, mở rộng sau)
│   └── DependencyInjection.cs                  # MediatR scan + Mapster + Domain Services
│
├── AgriTrace.Infrastructure.Sqlserver/         # 3. Tầng Hạ tầng (Infrastructure Layer)
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs             # EF Core DbContext
│   │   └── SeedData.cs                         # Seed: OrganizationTypes, EventTypes...
│   ├── Configurations/                         # Fluent API cho từng bảng
│   │   ├── OrganizationConfiguration.cs
│   │   ├── OrganizationTypeConfiguration.cs
│   │   ├── UserConfiguration.cs
│   │   ├── ProductConfiguration.cs
│   │   ├── CategoryConfiguration.cs
│   │   ├── UnitConfiguration.cs
│   │   ├── BatchConfiguration.cs
│   │   ├── SupplyChainEventConfiguration.cs
│   │   ├── BatchSplitConfiguration.cs
│   │   ├── BatchSplitDetailConfiguration.cs
│   │   ├── BatchMergeConfiguration.cs
│   │   ├── BatchMergeSourceConfiguration.cs
│   │   ├── QualityInspectionConfiguration.cs
│   │   ├── CertificateConfiguration.cs
│   │   ├── EventTypeConfiguration.cs
│   │   ├── RecallConfiguration.cs
│   │   └── NotificationConfiguration.cs
│   ├── Models/                                 # Data Models (ánh xạ 1-1 với bảng)
│   │   ├── BaseDataModel.cs
│   │   ├── OrganizationDataModel.cs
│   │   ├── OrganizationTypeDataModel.cs
│   │   ├── UserDataModel.cs
│   │   ├── ProductDataModel.cs
│   │   ├── CategoryDataModel.cs
│   │   ├── UnitDataModel.cs
│   │   ├── BatchDataModel.cs
│   │   ├── SupplyChainEventDataModel.cs
│   │   ├── BatchSplitDataModel.cs
│   │   ├── BatchSplitDetailDataModel.cs
│   │   ├── BatchMergeDataModel.cs
│   │   ├── BatchMergeSourceDataModel.cs
│   │   ├── QualityInspectionDataModel.cs
│   │   ├── CertificateDataModel.cs
│   │   ├── EventTypeDataModel.cs
│   │   ├── RecallDataModel.cs
│   │   └── NotificationDataModel.cs
│   ├── Migrations/                             # EF Core migrations (initDb, SeedMasterData)
│   ├── Repositories/                           # Implement Outbound interfaces
│   │   ├── CategoryRepository.cs               # ✓ đã nối DI
│   │   ├── ProductRepository.cs
│   │   ├── BatchRepository.cs
│   │   ├── SupplyChainEventRepository.cs
│   │   ├── EventTypeRepository.cs
│   │   ├── OrganizationRepository.cs
│   │   ├── OrganizationTypeRepository.cs
│   │   ├── QualityInspectionRepository.cs
│   │   ├── CertificateRepository.cs
│   │   ├── RecallRepository.cs
│   │   ├── UnitRepository.cs
│   │   └── UserRepository.cs
│   └── DependencyInjection.cs                  # DbContext (DefaultConnection) + Repositories
│
└── AgriTrace.API/                              # 4. Tầng Giao tiếp (Presentation Layer)
    ├── Controllers/
    │   ├── CategoriesController.cs             # ✓ triển khai đầy đủ CRUD
    │   ├── FarmsController.cs                  # (stub — đang phát triển)
    │   └── CropsController.cs                  # (stub — đang phát triển)
    ├── Models/                                 # Request/Response
    │   ├── ApiResponse.cs                      # Envelope chuẩn (StatusCode, IsSuccess, ErrorMessages, Result)
    │   ├── PaginationRequest.cs
    │   ├── CategoryRequest.cs
    │   ├── CategoryResponse.cs
    │   ├── CategoryQuery.cs
    │   ├── UpdateCategoryStatusRequest.cs
    │   ├── FarmRequest.cs / FarmResponse.cs
    │   └── CropRequest.cs / CropResponse.cs
    ├── Common/
    │   ├── ApiResponseWrapperFilter.cs         # Auto-wrap mọi result vào ApiResponse
    │   └── GlobalExceptionHandler.cs           # 400, 404, 409, 500 → ApiResponse
    ├── Mappings/
    │   └── ApiMappingRegister.cs               # Mapster config (hiện để trống)
    ├── DependencyInjection.cs                  # Controllers, Swagger, CORS, API Mapper, Exception Handler, Response Wrapper
    ├── Program.cs                              # Entry point (tối giản)
    ├── appsettings.json
    └── appsettings.Development.json
```

---

## Vai trò và Quy tắc của từng Tầng (Layer Responsibilities)

### 1. Tầng Domain (Lõi trung tâm)

- **Nhiệm vụ:** Chứa các thực thể (Entities), enum, business rules, domain services cốt lõi.
- **Quy tắc:** Tuyệt đối không phụ thuộc vào bất kỳ tầng nào khác hoặc thư viện bên ngoài. Các thuộc tính đóng gói (`private set`) để bảo vệ tính toàn vẹn dữ liệu; thay đổi trạng thái qua phương thức nghiệp vụ gọi `MarkUpdated()`.
- **Phân chia interface:**
  - `Interfaces/Inbound/` — hợp đồng dịch vụ nghiệp vụ mà tầng ngoài (API) gọi. Cơ sở chung `IService<TEntity,TKey> : IReadService<TEntity,TKey>, IWriteService<TEntity,TKey>`.
  - `Interfaces/Outbound/` — hợp đồng lưu trữ do Infrastructure hiện thực. Cơ sở chung `IRepository<TEntity,TKey> : IReadRepository<TEntity,TKey>, IWriteRepository<TEntity,TKey>`.
- **Entities chính:**
  - `Batch` — Aggregate Root trung tâm (quản lý số lượng, trạng thái, tổ chức hiện tại).
  - `SupplyChainEvent` — Append-only, có `PreviousHash`/`CurrentHash`.
  - `Organization`, `User`, `Product`, `Category`, `Unit`, `QualityInspection`, `Recall`, v.v.

### 2. Tầng Application (Ứng dụng / Nghiệp vụ)

- **Nhiệm vụ:** Đăng ký Dependency Injection (MediatR scan, Mapster, Domain Services), định nghĩa các exception chuẩn, chứa DTOs (`Contracts/`) và CQRS Features (`Features/`).
- **Quy tắc:** Phụ thuộc vào tầng **Domain**.
- **Hiện tại:** `AddApplication()` đăng ký `ICategoryService → CategoryService` và MediatR handlers cho module Categories (`Features/Categories/`). Các service và feature khác sẽ được bổ sung khi module tương ứng được kích hoạt.

### 3. Tầng Infrastructure.Sqlserver (Hạ tầng dữ liệu)

- **Nhiệm vụ:** Hiện thực hóa các Repository interface bằng EF Core + SQL Server.
- **Quy tắc:** Phụ thuộc vào tầng **Domain**. Mỗi Repository tự ánh xạ thủ công giữa `DataModel` ↔ `Entity` (xem `CategoryRepository.ToEntity`/`ToModel`).
- **Hash Chain:** `SupplyChainEvent` lưu `PreviousHash`/`CurrentHash`; repository tuân thủ append-only (event không được UPDATE/DELETE trong nghiệp vụ).

### 4. Tầng API / Presentation (Giao tiếp)

- **Nhiệm vụ:** Tiếp nhận HTTP request, gọi Domain Service, formatting response chuẩn.
- **Quy tắc:** Phụ thuộc vào tầng **Application** (DI) và **Infrastructure** (DI tại `Program.cs`). Controller ánh xạ Entity → Response thủ công (vd. `CategoriesController.ToResponse`).
- **Envelope chuẩn:** Mọi response được bọc bởi `ApiResponseWrapperFilter` thành `ApiResponse { statusCode, isSuccess, errorMessages, result }`; lỗi chưa xử lý được `GlobalExceptionHandler` chuyển về cùng envelope.

---

## Luồng dữ liệu và Mapping (Data Flow)

```
[Client] ──(JSON)──► [Request Model] (API)
                             │
                      (validate + map thủ công thành Entity)
                             ▼
                    [Domain Service] (Application DI → Domain)
                             │
                      (gọi Repository)
                             ▼
                    [Outbound Repository] (Domain interface)
                             │
                      (EF Core: DataModel ↔ Entity)
                             ▼
                    [DataModel] (Infrastructure — SQL Server)
                             │
                      (truy vấn ngược + map Entity → Response)
                             ▼
[Client] ◄──(JSON)── [ApiResponse] (API — bọc bởi ApiResponseWrapperFilter)
```

### Response Envelope
```json
{
  "statusCode": 200,
  "isSuccess": true,
  "errorMessages": [],
  "result": { "...": "dữ liệu" }
}
```

### Hash Chain Flow (Ghi nhận Event)
```
1. Lấy event gần nhất của Batch → CurrentHash
2. Gán PreviousHash = CurrentHash đó
3. Tính CurrentHash = SHA256(PreviousHash + EventData)   // HashChainService.ComputeHash
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
| DI Container | Built-in Microsoft.Extensions.DependencyInjection |
| Mapping | Mapster (đã đăng ký, chưa dùng nhiều) |
| CQRS | MediatR (đã đăng ký, có handler cho Categories) |
| Hashing | SHA-256 (`HashChainService`) |
| API Docs | Swagger / OpenAPI |
| Testing | xUnit + Moq (dự kiến) |

> Các thành phần như JWT Auth, Redis cache, Public QR trace endpoints **chưa được triển khai** trong mã nguồn hiện tại — chỉ mới có khung hạ tầng sẵn sàng.

## Dependency Injection

Mỗi tầng tự đóng gói dịch vụ nội bộ qua `DependencyInjection.cs`:

```csharp
// Program.cs — tối giản
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);
```

- `AddPresentation()`: Controllers (+ `ApiResponseWrapperFilter`), `GlobalExceptionHandler`, Swagger, Mapster scan (API).
- `AddApplication()`: MediatR (scan handlers), Mapster global config, Domain Services (hiện tại: `ICategoryService`).
- `AddInfrastructureSqlServer()`: DbContext (`DefaultConnection`), Repositories (hiện tại: `ICategoryRepository`).
