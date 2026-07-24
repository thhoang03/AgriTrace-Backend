# RBAC Backend Specification (Tài Liệu Đặc Tả Backend)

> **Hệ Thống Truy Xuất Nguồn Gốc Nông Sản — AgriTrace**
> **Phiên bản:** 1.0  
> **Cập nhật:** 2026-07-27  
> **Đối tượng:** Backend Development Team  
> **Ref:** `docs/rabc.md` (Frontend RBAC Doc)

---

## 1. Tổng Quan (Overview)

Hệ thống sử dụng mô hình phân quyền **RBAC (Role-Based Access Control)** với hai tầng quyền:
- **System-level:** Vai trò `Admin` thuộc tổ chức `SYSTEM`
- **Organization-level:** Vai trò `Manager` / `Staff` thuộc các tổ chức nghiệp vụ

Mọi request API **phải** được xác thực (JWT token) và kiểm tra quyền trước khi xử lý.

---

## 2. Mô Hình Dữ Liệu (Data Model)

### 2.1. User

| Field | Type | Required | Mô tả |
|-------|------|----------|-------|
| `id` | `string` | ✅ | UUID/định danh người dùng |
| `email` | `string` | ✅ | Email đăng nhập (duy nhất) |
| `name` | `string` | ✅ | Họ và tên |
| `role` | `enum` | ✅ | Xem mục 2.2 |
| `organizationId` | `string` | ❌ | ID tổ chức (null với Admin hệ thống) |
| `organizationType` | `enum` | ❌ | Xem mục 2.3 (null với Admin hệ thống) |
| `isActive` | `boolean` | ✅ | Trạng thái tài khoản |

### 2.2. UserRole (Hệ Thống)

| Value | Mô tả | Phạm vi |
|-------|-------|---------|
| `ADMIN` | Administrator tối cao, thuộc tổ chức `SYSTEM` | System-level |
| `MANAGER` | Quản lý tổ chức nghiệp vụ | Organization-level |
| `STAFF` | Nhân viên tổ chức nghiệp vụ | Organization-level |

### 2.3. OrganizationType (Loại Tổ Chức)

| Value | Mô tả | Ghi chú |
|-------|-------|---------|
| `FARM` | Trang trại / Cơ sở trồng trọt | |
| `PROCESSOR` | Nhà chế biến / Đóng gói | |
| `DISTRIBUTOR` | Nhà phân phối / Vận chuyển | |
| `RETAILER` | Nhà bán lẻ / Siêu thị | |
| `INSPECTION` | Đơn vị kiểm định / Kiểm nghiệm | |
| `SYSTEM` | Tổ chức mặc định hệ thống | ** Không cho phép đăng ký mới qua API công khai** |

### 2.4. EventType (Loại Sự Kiện Nông Sản)

| Value | Mô tả |
|-------|-------|
| `HARVEST` | Thu hoạch |
| `RECEIVE` | Tiếp nhận |
| `PROCESSING` | Chế biến |
| `PACKAGING` | Đóng gói |
| `TRANSPORT` | Vận chuyển |
| `DISTRIBUTION` | Phân phối |
| `RETAIL` | Bán lẻ |
| `INSPECTION` | Kiểm định |
| `SPLIT` | Tách lô |
| `MERGE` | Gộp lô |
| `RECALL` | Thu hồi sản phẩm |

### 2.5. Organization Request Payload

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `name` | `string` | ✅ | Tên tổ chức, không trống |
| `type` | `enum` | ✅ | Một trong: `FARM`, `PROCESSOR`, `DISTRIBUTOR`, `RETAILER`, `INSPECTION` |
| `address` | `string` | ❌ | Địa chỉ |

> **⚠️ Ràng buộc bảo mật:** `type = "SYSTEM"` bị cấm trong mọi request API công khai. Backend **phải** từ chối request tạo tổ chức `SYSTEM` với HTTP `403 Forbidden`.

### 2.6. Invite Staff Request Payload

| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `email` | `string` | ✅ | Email của nhân viên cần mời |

> **⚠️ Ràng buộc bảo mật (IDOR Prevention):** `organizationId` **không được phép** truyền trong Request Body hoặc Query Parameter khi thêm nhân viên. Backend **phải** tự động gán `orgId` từ `organizationId` của `Manager` đang thực hiện request.

---

## 3. Quy Tắc Phân Quyền Sự Kiện (Event RBAC Matrix)

Quyền khởi tạo sự kiện nông sản (`EventType`) dựa trên loại tổ chức (`OrganizationType`). **Cả `Manager` và `Staff`** thuộc cùng một tổ chức đều tuân theo ma trận này.

### 3.1. Event Permission Matrix

| OrganizationType | HARVEST | RECEIVE | PROCESSING | PACKAGING | TRANSPORT | DISTRIBUTION | RETAIL | INSPECTION | SPLIT | MERGE | RECALL |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **`FARM`** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **`PROCESSOR`** | ❌ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| **`DISTRIBUTOR`** | ❌ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ | ❌ |
| **`RETAILER`** | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ✅ | ❌ | ❌ |
| **`INSPECTION`** | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| **`SYSTEM`** (Admin) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |

### 3.2. Backend Enforcement Rule

Với mọi request tạo sự kiện (`POST /batches/{batchId}/events` hoặc tương đương), backend **phải**:

1. Giải mã JWT token để lấy `user.role`, `user.organizationType`, `user.organizationId`
2. Truy vấn `OrganizationType` của người dùng
3. Kiểm tra `EventType` trong request có nằm trong danh sách được phép of `OrganizationType` đó không
4. Nếu **không** được phép → trả về HTTP `403 Forbidden` với body:

```json
{
  "status": 403,
  "message": "Your organization type does not have permission to create this event type"
}
```

---

## 4. Quy Tắc Bảo Mật (Security Rules)

### 4.1. Staff Management (Thêm Nhân Viên)

**Endpoint:** `POST /organizations/{orgId}/users` (hoặc tương đương)

| Rule | Mô tả | Backend Implementation |
|------|-------|----------------------|
| **R1** | Chỉ `role == MANAGER` mới được thêm Staff | Kiểm tra `user.role === "MANAGER"`. Nếu không → `403 Forbidden` |
| **R2** | `orgId` của Staff mới phải lấy tự động từ `orgId` của Manager | Gán `organizationId` = `user.organizationId` (từ token). **Bỏ qua** bất kỳ `orgId` nào trong request body/query |
| **R3** | Không cho phép MANAGER thêm Staff vào tổ chức khác | Nếu `requested orgId != user.organizationId` → `403 Forbidden` |

**Request Body:**
```json
{
  "email": "staff@example.com"
}
```
⚠️ Không có field `organizationId` trong body. Nếu client gửi kèm, backend **phải** bỏ qua.

**Response (Success — 201 Created):**
```json
{
  "userId": "USR-001",
  "email": "staff@example.com",
  "role": "STAFF",
  "organizationId": "<orgId của Manager>",
  "organizationType": "<type của tổ chức Manager>"
}
```

### 4.2. Recall Event Logic (Thu Hồi Sản Phẩm)

**Endpoint:** `POST /recalls` (hoặc tương đương)

Recall **chỉ** được phép bởi tổ chức `SYSTEM` (Admin):

| Rule | Mô tả | Backend Implementation |
|------|-------|----------------------|
| **R4** | Chỉ `role == ADMIN` và `organizationType == SYSTEM` mới được create RECALL | Kiểm tra `user.role === "ADMIN" && user.organizationType === "SYSTEM"`. Nếu không → `403 Forbidden` |

**Response (Forbidden — 403):**
```json
{
  "status": 403,
  "message": "Only system administrator can create recall events"
}
```

> **Lưu ý:** Hiện tại không có data scope phức tạp cho INSPECTION org đối với RECALL. INSPECTION chỉ được phép tạo INSPECTION events. Nếu sau này cần mở rộng recall cho INSPECTION, cần cập nhật specification này và thêm data scope check (kiểm tra INSPECTION org đã từng kiểm định lô hàng đó với kết quả FAILED/REJECTED).

### 4.3. Route Access Control

Không phải route nào cũng cần org-type check. Tuy nhiên, mọi route bên trong `/app/*` (trừ `/app/login`) đều yêu cầu xác thực. Backend cần:

| Route Prefix | Auth Required | Role Constraint |
|-------------|---------------|-----------------|
| `/auth/login` | ❌ | Không |
| `/auth/refresh` | ✅ | Không |
| `/batches/*` | ✅ | User phải thuộc một tổ chức |
| `/recalls/*` | ✅ | Chỉ `ADMIN` + `SYSTEM` |
| `/organizations/*` | ✅ | Chỉ `MANAGER` + `ADMIN` |
| `/users/*` (quản lý nhân sự) | ✅ | Chỉ `MANAGER` + `ADMIN` |
| `/supply-chain/*` | ✅ | User thuộc bất kỳ tổ chức nào |
| `/inspections/*` | ✅ | `INSPECTION` hoặc `SYSTEM` |
| `/reports/*` | ✅ | `ADMIN`, `MANAGER` |

---

## 5. Cấu Hình API Response Format

### 5.1. Error Response (RBAC Denied)

```json
{
  "status": 403,
  "message": "Forbidden: insufficient permissions",
  "error": {
    "code": "RBAC_FORBIDDEN",
    "details": "Organization type FARM does not allow event type RECALL"
  }
}
```

### 5.2. Error Codes

| Code | HTTP | Scenario |
|------|------|----------|
| `RBAC_FORBIDDEN` | 403 | User không có quyền cho thao tác |
| `RBAC_EVENT_DENIED` | 403 | User không được tạo event type cụ thể |
| `RBAC_ORG_PROHIBITED` | 403 | Tạo tổ chức SYSTEM bị cấm |
| `RBAC_IDOR_ATTEMPT` | 403 | Client cố gắng truyền orgId trái phép |
| `RBAC_INVALID_ROLE` | 401 | Role không hợp lệ hoặc không xác định |

---

## 6. Validation Tóm Tắt (Kiểm Tra Nhanh)

Backend implementation **phải** kiểm tra các trường hợp sau với mọi request tạo dữ liệu:

### Event Creation (`POST /batches/{batchId}/events`)
```
1. Xác thực token → lấy user
2. Lấy user.organizationType
3. Lấy eventType từ request body
4. Tra ORG_EVENT_PERMISSIONS[organizationType]
5. Nếu eventType ∉ danh sách → 403
```

### Staff Invite (`POST /organizations/{orgId}/users`)
```
1. Xác thực token → lấy user
2. Kiểm tra user.role == MANAGER → nếu không → 403
3. Kiểm tra orgId từ URL == user.organizationId → nếu không → 403
4. Tạo user mới với organizationId = user.organizationId (từ token, BẦT TỪ BODY)
5. Gán role = STAFF
```

### Recall Creation (`POST /recalls`)
```
1. Xác thực token → lấy user
2. Kiểm tra user.role == ADMIN && user.organizationType == SYSTEM
3. Nếu sai → 403 "Only system administrator can create recall events"
```

### Organization Creation (`POST /organizations`)
```
1. Xác thực token → lấy user
2. Kiểm tra user.role == ADMIN
3. Kiểm tra request body.type != "SYSTEM" → nếu là SYSTEM → 403
4. Tạo tổ chức
```

---

## 7. Phụ Lục A: Mã Hóa Trước (Frontend → Backend Mapping)

| Frontend Constant | Giá trị | Backend Cần Kiểm Tra |
|-------------------|---------|---------------------|
| `ORGANIZATION_TYPE.FARM` | `"FARM"` | |
| `ORGANIZATION_TYPE.PROCESSOR` | `"PROCESSOR"` | |
| `ORGANIZATION_TYPE.DISTRIBUTOR` | `"DISTRIBUTOR"` | |
| `ORGANIZATION_TYPE.RETAILER` | `"RETAILER"` | |
| `ORGANIZATION_TYPE.INSPECTION` | `"INSPECTION"` | |
| `ORGANIZATION_TYPE.SYSTEM` | `"SYSTEM"` | Cấm đăng ký mới qua API công khai |
| `USER_ROLE.ADMIN` | `"ADMIN"` | |
| `USER_ROLE.MANAGER` | `"MANAGER"` | |
| `USER_ROLE.STAFF` | `"STAFF"` | |
| `EVENT_TYPE.HARVEST` | `"HARVEST"` | |
| `EVENT_TYPE.RECEIVE` | `"RECEIVE"` | |
| `EVENT_TYPE.PROCESSING` | `"PROCESSING"` | |
| `EVENT_TYPE.PACKAGING` | `"PACKAGING"` | |
| `EVENT_TYPE.TRANSPORT` | `"TRANSPORT"` | |
| `EVENT_TYPE.DISTRIBUTION` | `"DISTRIBUTION"` | |
| `EVENT_TYPE.RETAIL` | `"RETAIL"` | |
| `EVENT_TYPE.INSPECTION` | `"INSPECTION"` | |
| `EVENT_TYPE.SPLIT` | `"SPLIT"` | |
| `EVENT_TYPE.MERGE` | `"MERGE"` | |
| `EVENT_TYPE.RECALL` | `"RECALL"` | Chỉ SYSTEM admin |

---

## 8. Phụ Lục B: Changelog

| Version | Date | Change | Author |
|---------|------|--------|--------|
| 1.0 | 2026-07-27 | Initial backend-facing RBAC spec (recall: SYSTEM admin only) | AgriTrace Team |
| — | 2026-07-22 | INSPECTION had RECALL permission (old spec) | — |
