# RBAC Guide — AgriTrace Backend

## 1. Tổng quan

RBAC (Role-Based Access Control) trong AgriTrace Backend được tổ chức theo 2 lớp:

- **Layer 1 — Role-based (JWT + `[Authorize]`):** Kiểm tra quyền truy cập endpoint dựa trên role (`Admin`, `Manager`, `Staff`) thông qua JWT claims.
- **Layer 2 — OrganizationType ↔ EventType matrix:** Kiểm tra quyền nghiệp vụ chi tiết: một user có được phép tạo loại event nào hay không, phụ thuộc vào `OrganizationType` của tổ chức họ.

## 2. Các vai trò (Roles)

| Role | Mô tả | Đặc quyền Layer 1 |
| --- | --- | --- |
| `Admin` | Quản trị hệ thống | Toàn quyền; bypass Layer 2 event check (vẫn phải tuân thủ batch ownership guard, trừ INSPECTION cross-org). |
| `Manager` | Quản lý tổ chức | Xem analytics, quản lý category, tạo recall. |
| `Staff` | Nhân viên thực thi | Tạo events theo loại tổ chức, xem batch/events. |

## 3. Các loại tổ chức (Organization Types)

| Code | Tên | Mô tả |
| --- | --- | --- |
| `FARM` | Farm | Nông trại |
| `PROCESSOR` | Processor | Cơ sở chế biến |
| `DISTRIBUTOR` | Distributor | Phân phối |
| `RETAILER` | Retailer | Bán lẻ |
| `INSPECTION` | Inspection | Kiểm định chất lượng |
| `SYSTEM` | System | Hệ thống (internal) |

## 4. Các loại Event (Event Types)

| Code | Tên |
| --- | --- |
| `HARVEST` | Thu hoạch |
| `RECEIVE` | Nhận hàng |
| `PROCESSING` | Chế biến |
| `PACKAGING` | Đóng gói |
| `TRANSPORT` | Vận chuyển |
| `DISTRIBUTION` | Phân phối |
| `RETAIL` | Bán lẻ |
| `INSPECTION` | Kiểm định |
| `SPLIT` | Tách batch |
| `MERGE` | Gộp batch |
| `RECALL` | Thu hồi |

## 5. Ma trận quyền Event (Layer 2)

`EventPermissionRules.IsAllowed(orgTypeCode, eventTypeCode)` kiểm tra quyền tạo event:

| OrganizationType | HARVEST | RECEIVE | PROCESSING | PACKAGING | TRANSPORT | DISTRIBUTION | RETAIL | INSPECTION | SPLIT | MERGE | RECALL |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `FARM` | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `PROCESSOR` | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| `DISTRIBUTOR` | ❌ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ |
| `RETAILER` | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| `INSPECTION` | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| `SYSTEM` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

> **Lưu ý:**
> - `Admin` bypass Layer 2 event check.
> - `INSPECTION` org có quyền tạo `INSPECTION` event **xuyên tổ chức** (cross-org), bỏ qua batch ownership guard.
> - Mọi user (trừ `INSPECTION` cross-org) phải tuân thủ **batch ownership guard**: `batch.CurrentOrganizationId == user.OrganizationId`.

## 6. Luồng xác thực (Authentication Flow)

```
Client → JWT Login → TokenService.GenerateAccessToken(user)
         ↓
   Token chứa claims:
     - sub (UserId)
     - email
     - role (Admin/Manager/Staff)
     - organizationId (nếu có)
         ↓
   Middleware: JWT Bearer Authentication
         ↓
   Controller: [Authorize(Roles = "Admin,Manager")]
         ↓
   Command Handler: kiểm tra Layer 2 (EventPermissionRules + ownership guard)
```

## 7. Cấu trúc code liên quan

| File | Vai trò |
| --- | --- |
| `AgriTrace.Domain/Entities/Users/UserRole.cs` | Enum 3 vai trò. |
| `AgriTrace.Domain/Common/EventPermissionRules.cs` | Ma trận quyền Layer 2. |
| `AgriTrace.Application/Features/Events/Commands/CreateEventCommand.cs` | Handler thực thi Layer 2 + ownership guard. |
| `AgriTrace.Application/Common/Exceptions/ForbiddenException.cs` | Exception 403 Forbidden. |
| `AgriTrace.API/Common/GlobalExceptionHandler.cs` | Map `ForbiddenException` → HTTP 403. |
| `AgriTrace.API/Controllers/CertificatesController.cs` | Narrow `[Authorize]`: `Admin` cho issue/revoke cert. |
| `AgriTrace.API/Controllers/InspectionsController.cs` | Narrow `[Authorize]`: `Admin` cho create/update inspection. |
| `AgriTrace.Infrastructure.Sqlserver/Migrations/20260724081545_MigrateObsoleteRoles.cs` | Migration chuyển `Farmer`/`Inspector`/`Consumer` → `Staff`. |
| `AgriTrace.Infrastructure.Sqlserver/Persistence/SeedData.cs` | Seed data đã cập nhật role + email. |

## 8. Quy tắc đặc biệt

### 8.1 Batch Ownership Guard
Khi tạo event:
- User **phải** thuộc tổ chức hiện tại đang sở hữu batch (`batch.CurrentOrganizationId == user.OrganizationId`).
- **Exception:** User thuộc `INSPECTION` org tạo `INSPECTION` event → được phép cross-org.

### 8.2 Admin Bypass
- `Admin` được **miễn** Layer 2 event permission check.
- `Admin` **vẫn phải** tuân thủ batch ownership guard (trừ trường hợp `INSPECTION` cross-org).

### 8.3 Controller Auth Rules
| Controller / Action | Role yêu cầu |
| --- | --- |
| `AnalyticsController` | `Admin,Manager` |
| `CategoriesController` (Create/Update/Delete) | `Admin,Manager` |
| `CertificatesController::IssueCertificate` | `Admin` |
| `CertificatesController::RevokeCertificate` | `Admin` |
| `InspectionsController::CreateInspection` | `Admin` |
| `InspectionsController::UpdateInspection` | `Admin` |
| `OrganizationsController` (mutations) | `Admin` |
| `RecallsController::Create` | `Admin,Manager` |
| `RecallsController::Resolve` | `Admin,Manager` |
| `UsersController` (mutations) | `Admin` |

## 9. Migration dữ liệu

Chạy lệnh để áp dụng migration:

```bash
dotnet ef database update --project AgriTrace.Infrastructure.Sqlserver --startup-project AgriTrace.API
```

Migration `MigrateObsoleteRoles` sẽ:
- Cập nhật seed users (`Id: ...0002` và `...0004`) về `Staff`.
- Chạy raw SQL: `UPDATE Users SET Role = 'Staff' WHERE Role IN ('Farmer', 'Inspector', 'Consumer')`.

## 10. Testing

### Unit tests cho Layer 2
File: `AgriTrace.Tests/Domain/Common/EventPermissionRulesTests.cs`

```bash
dotnet test --filter "EventPermissionRulesTests"
```

### Toàn bộ test suite
```bash
dotnet test
```

## 11. Lưu ý phát triển

- **Không thêm role mới** vào `UserRole` enum mà không cập nhật `EventPermissionRules` + seed data + migration.
- **Không hardcode** role strings trong controller; dùng `[Authorize(Roles = "...")]` hoặc kiểm tra qua `ICurrentUserService`.
- Khi thêm `EventType` mới, cần cập nhật ma trận trong `EventPermissionRules.cs`.
- JWT claims được tạo bởi `TokenService.GenerateAccessToken()`; role claim tự động lấy từ `UserRole.ToString()`.
