
# Database & Business Specification

## Agricultural Supply Chain Traceability System (v4.0)

* **Database:** `AgriTraceabilityDB`
* **DBMS:** SQL Server
* **Architecture:** Clean Architecture + DDD
* **Backend:** ASP.NET Core Web API (.NET 10)
* **Frontend:** React 


---


# MỤC LỤC TÀI LIỆU (TABLE OF CONTENTS)

- [1. Overview](#1-overview)
  - [1.1 System Description](#11-system-description)
  - [1.2 Main Objectives](#12-main-objectives)
- [2. Business Actors & Roles](#2-business-actors--roles)
  - [2.1 System Admin](#21-system-admin)
  - [2.2 Organization Admin](#22-organization-admin)
  - [2.3 Staff](#23-staff)
  - [2.4 Inspector](#24-inspector)
  - [2.5 Consumer](#25-consumer)
- [3. Organization Model](#3-organization-model)
  - [3.1 Organization Types](#31-organization-types)
  - [3.2 Organization & User Relationship](#32-organization--user-relationship)
- [4. Authentication & Authorization](#4-authentication--authorization)
  - [4.1 Authentication](#41-authentication)
  - [4.2 Authorization](#42-authorization)
- [5. Database Core Design](#5-database-core-design)
- [6. Product Management](#6-product-management)
- [7. Batch Management](#7-batch-management)
  - [7.1 Định nghĩa](#71-%C4%91%E1%BB%8Bnh-ngh%C4%A9a)
  - [7.2 Quy trình khởi tạo Lô hàng (Batch Creation)](#72-quy-tr%C3%ACnh-kh%E1%BB%9Fi-t%E1%BA%A1o-l%C3%B4-h%C3%A0ng-batch-creation)
  - [7.3 Cấu trúc dữ liệu bảng Batches](#73-c%E1%BA%A5u-tr%C3%BAc-d%E1%BB%AF-li%E1%BB%87u-b%E1%BA%A3ng-batches)
  - [7.4 Quy tắc nghiệp vụ các trường dữ liệu quan trọng](#74-quy-t%E1%BA%AFc-nghi%E1%BB%87p-v%E1%BB%A5-c%C3%A1c-tr%C6%B0%E1%BB%9Dng-d%E1%BB%AF-li%E1%BB%87u-quan-tr%E1%BB%8Dng)
- [8. Supply Chain Event Management](#8-supply-chain-event-management)
  - [8.1 Định nghĩa](#81-%C4%91%E1%BB%8Bnh-ngh%C4%A9a)
  - [8.2 Danh mục sự kiện cấu hình (EventTypes)](#82-danh-m%E1%BB%A5c-s%E1%BB%B1-ki%E1%BB%87n-c%E1%BA%A5u-h%C3%ACnh-eventtypes)
  - [8.3 Cấu trúc dữ liệu bảng SupplyChainEvents](#83-c%E1%BA%A5u-tr%C3%BAc-d%E1%BB%AF-li%E1%BB%87u-b%E1%BA%A3ng-supplychainevents)
- [9. Hash Chain Mechanism](#9-hash-chain-mechanism)
  - [9.1 Nguyên lý bảo mật dữ liệu](#91-nguy%C3%AAn-l%C3%BD-b%E1%BA%A3o-m%E1%BA%ADt-d%E1%BB%AF-li%E1%BB%87u)
  - [9.2 Luồng liên kết mã băm (Hash Logic)](#92-lu%E1%BB%93ng-li%C3%AAn-k%E1%BA%BFt-m%C3%A3-b%C4%83m-hash-logic)
  - [9.3 Công thức tính toán](#93-c%C3%B4ng-th%E1%BB%A9c-t%C3%ADnh-to%C3%A1n)
  - [9.4 Quy tắc hệ thống (Immutable Rules)](#94-quy-t%E1%BA%AFc-h%E1%BB%87-th%E1%BB%91ng-immutable-rules)
- [10. Batch Split Management](#10-batch-split-management)
  - [10.1 Nghiệp vụ tách lô](#101-nghi%E1%BB%87p-v%E1%BB%A5-t%C3%A1ch-l%C3%B4)
  - [10.2 Luồng xử lý dữ liệu (Split Flow)](#102-lu%E1%BB%93ng-x%E1%BB%AD-l%C3%BD-d%E1%BB%AF-li%E1%BB%87u-split-flow)
  - [10.3 Cấu trúc cơ sở dữ liệu phân tách lô](#103-c%E1%BA%A5u-tr%C3%BAc-c%C6%A1-s%E1%BB%9F-d%E1%BB%AF-li%E1%BB%87u-ph%C3%A2n-t%C3%A1ch-l%C3%B4)
- [11. Batch Merge Management (Bổ sung theo mạch tài liệu)](#11-batch-merge-management)
- [12. Quality Inspection](#12-quality-inspection)
- [13. Certificate Management](#13-certificate-management)
- [14. Recall Management](#14-recall-management)
- [15. Public Traceability](#15-public-traceability)
- [16. Notification](#16-notification)
- [17. Database Schema Overview](#17-database-schema-overview)
- [18. Business Rules](#18-business-rules)
- [19. API Mapping Recommendation](#19-api-mapping-recommendation)
- [20. Clean Architecture Mapping](#20-clean-architecture-mapping)

---

## 1. Overview

### 1.1 System Description

**Agricultural Supply Chain Traceability System** là hệ thống quản lý truy xuất nguồn gốc nông sản xuyên suốt chuỗi cung ứng. Hệ thống quản lý toàn bộ vòng đời của một **Batch** (lô hàng):

$$\text{Farm} \longrightarrow \text{Processing} \longrightarrow \text{Packaging} \longrightarrow \text{Transportation} \longrightarrow \text{Distribution} \longrightarrow \text{Retail} \longrightarrow \text{Consumer}$$

Mỗi **Batch** được tạo ra sẽ sở hữu:

* Mã duy nhất định danh lô hàng (`Unique Batch Code`).
* Mã phản hồi nhanh phục vụ truy xuất (`QR Code`).
* Dòng thời gian chuỗi cung ứng (`Supply Chain Timeline`).

Mọi hoạt động tác động lên Batch đều phát sinh một hành vi hệ thống được ghi nhận là `SupplyChainEvent`.

* *Ví dụ:* Người nông dân thu hoạch cà chua $\rightarrow$ Event: `HARVEST` $\rightarrow$ Cơ sở chế biến tiếp nhận $\rightarrow$ Event: `RECEIVE` $\rightarrow$ Đơn vị phân phối vận chuyển $\rightarrow$ Event: `TRANSPORT`.

### 1.2 Main Objectives

1. **Traceability:** Cho phép người tiêu dùng quét mã QR (`Scan QR Code`) để xem toàn bộ lịch sử sản phẩm bao gồm: Nguồn gốc trang trại, ngày thu hoạch, lịch sử chế biến/vận chuyển, kết quả kiểm định, chứng chỉ đi kèm và trạng thái thu hồi.
2. **Data Integrity:** Lịch sử `SupplyChainEvent` tuân thủ nguyên tắc **Append-only** (không được phép sửa hoặc xóa). Mỗi sự kiện được liên kết bảo vệ bằng cơ chế chuỗi mã hóa **SHA-256 Hash Chain**.
3. **Supply Chain Management:** Hỗ trợ các tác nhân tham gia (`Farmer`, `Processor`, `Distributor`, `Retailer`, `Inspector`) ghi nhận đầy đủ hoạt động thực tế.
4. **Product Safety:** Quản lý chặt chẽ công tác kiểm định chất lượng (`Quality Inspection`), cấp chứng chỉ (`Certificate`), xử lý khủng hoảng thu hồi nông sản (`Recall`) và thông báo (`Notification`).

---

## 2. Business Actors & Roles

| Actor | Description | Responsibilities |
| --- | --- | --- |
| **System Admin** | Người quản trị toàn hệ thống. | Quản lý tổ chức, phân quyền người dùng, danh mục cấu hình, khởi tạo lệnh thu hồi sản phẩm toàn hệ thống. |
| **Organization Admin** | Quản trị viên nội bộ của một tổ chức (Farm, Processor, Distributor,...). | Quản lý nhân viên trực thuộc, danh mục sản phẩm và các lô hàng thuộc phạm vi tổ chức sở hữu. |
| **Staff** | Nhân viên trực tiếp thực hiện nghiệp vụ tại các mắt xích cung ứng. | Ghi nhận sự kiện (`Events`). Hệ thống không chia cụ thể vai trò *Farmer/Processor/Distributor* ở tầng Role mà xác định dựa trên thuộc tính **Organization Type** của tổ chức họ trực thuộc. |
| **Inspector** | Đơn vị kiểm định chất lượng độc lập. | Thực hiện kiểm định, cập nhật báo cáo chất lượng và đính kèm chứng chỉ hợp quy. |
| **Consumer** | Người tiêu dùng cuối cùng. | Không cần đăng nhập, chỉ có quyền đọc (`Read-only`) để xem dòng thời gian (`Timeline`) truy xuất. |

---

## 3. Organization Model

### 3.1 Organization Types

Đại diện cho vai trò vị trí chức năng của đơn vị trong chuỗi cung ứng, được cấu hình trong bảng `OrganizationTypes`:

| Code | Description |
| --- | --- |
| `FARM` | Nông trại |
| `PROCESSOR` | Cơ sở sơ chế, chế biến |
| `DISTRIBUTOR` | Nhà phân phối |
| `RETAILER` | Cửa hàng bán lẻ |
| `INSPECTION` | Đơn vị kiểm định chất lượng |
| `SYSTEM` | Hệ thống quản trị tổng |

### 3.2 Organization & User Relationship

Mỗi `Organization` đại diện cho một pháp nhân hoặc cơ sở vật lý tham gia chuỗi. Khách thể `User` quan hệ phụ thuộc với `Organization` theo kiến trúc một-nhiều: Một User bắt buộc thuộc về một Tổ chức và nắm giữ một Role nhất định.

> **Ví dụ phân định nghiệp vụ:**
> Nguyễn Văn A có Role là `Staff`, thuộc tổ chức `Đà Lạt Farm` có loại hình là `FARM` $\rightarrow$ Hệ thống định danh nghiệp vụ thực tế của đối tượng này là **Farmer**.

---

## 4. Authentication & Authorization

### 4.1 Authentication

* Cơ chế xác thực dựa trên **JWT (JSON Web Token)**.
* Quy trình: `User Login` $\rightarrow$ Kiểm tra thông tin định danh $\rightarrow$ Cấp phát cặp Token (`Access Token` ngắn hạn & `Refresh Token` dài hạn lưu DB) $\rightarrow$ Trả về thông tin phiên làm việc.

### 4.2 Authorization

Hệ thống kiểm soát đặc quyền qua 2 lớp bảo mật tích hợp:

* **Layer 1 (Role Permission):** Kiểm tra vai trò định danh hệ thống (`SystemAdmin`, `OrganizationAdmin`, `Staff`, `Inspector`).
* **Layer 2 (Organization Event Permission):** Kiểm tra chéo giữa Loại hình tổ chức (`OrganizationType`) và Loại sự kiện (`EventType`) để quyết định quyền ghi dữ liệu.
* *Ví dụ:* Chỉ User thuộc tổ chức `FARM` mới được thực thi quyền tạo sự kiện `HARVEST`. Tổ chức `DISTRIBUTOR` được cấp quyền cho các sự kiện `RECEIVE`, `TRANSPORT`, `DISTRIBUTION`, `SPLIT`, `MERGE`.



---

## 5. Database Core Design

Cấu trúc phân hệ cơ sở dữ liệu được tổ chức thành các nhóm logic sau:

* **Identity (Định danh):** `Users`, `Roles`, `RefreshTokens`
* **Organization (Tổ chức):** `Organizations`, `OrganizationTypes`, `OrganizationTypeEvents`
* **Product (Sản phẩm):** `Categories`, `Products`, `UnitTypes`, `Units`
* **Traceability Core (Lõi truy xuất):** `Batches`, `SupplyChainEvents`
* **Advanced Traceability (Nghiệp vụ lô biến đổi):** `BatchSplits`, `BatchSplitDetails`, `BatchMerges`, `BatchMergeSources`
* **Quality (Chất lượng):** `QualityInspections`, `Certificates`
* **System & Safety (An toàn & Hệ thống):** `Recalls`, `Notifications`

---

## 6. Product Management

* **Mục đích:** Quản lý danh mục các sản phẩm nông nghiệp được phép lưu hành và tham gia chuỗi cung ứng.
* **Phân cấp thực thể:** `Category` (Nhóm sản phẩm) $\rightarrow$ `Product` (Sản phẩm cụ thể) $\rightarrow$ Định lượng bằng `Unit` (Đơn vị tính).
* **Quy tắc nghiệp vụ:**
* Quan hệ một - nhiều giữa `Category` và `Product` (Một loại nông sản chỉ thuộc duy nhất một danh mục cha).
* `Product` gắn liền với tổ chức khởi tạo (`OrganizationId`) và không được thay đổi trong suốt vòng đời của `Batch`.
* Để đảm bảo toàn vẹn dữ liệu quá khứ, các `Unit` đã phát sinh giao dịch **không được xóa** mà chỉ chuyển đổi trạng thái hoạt động (`IsActive = 0`).



---

## 7. Batch Management

### 7.1 Định nghĩa

`Batch` (Lô hàng) là thực thể trung tâm quản trị toàn bộ trạng thái vật lý của nông sản trên đường di chuyển.

```text
Mẫu mã Batch: TOMATO-2026-001 | Số lượng: 500 KG | QR Định danh bảo mật

```

### 7.2 Quy trình khởi tạo Lô hàng (Batch Creation)

Tác nhân `Staff` thuộc đơn vị `FARM` nhập thông tin sản phẩm, ngày thu hoạch, số lượng và đơn vị tính $\rightarrow$ Hệ thống tự động sinh `BatchCode` và mã hóa `QRCode` $\rightarrow$ Lưu thông tin thực thể `Batches` đồng thời kích hoạt tự động sự kiện đầu tiên: `HARVEST Event`.

### 7.3 Cấu trúc dữ liệu bảng `Batches`

```sql
CREATE TABLE Batches
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    CreatedOrganizationId UNIQUEIDENTIFIER NOT NULL,
    CurrentOrganizationId UNIQUEIDENTIFIER NOT NULL,
    BatchCode VARCHAR(100) UNIQUE NOT NULL,
    QRCode VARCHAR(500),
    Quantity DECIMAL(18,2),
    UnitId UNIQUEIDENTIFIER,
    RemainingQuantity DECIMAL(18,2),
    Status INT,
    ParentBatchId UNIQUEIDENTIFIER NULL,
    RootBatchId UNIQUEIDENTIFIER NULL,
    HarvestDate DATETIME2,
    ExpirationDate DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

```

### 7.4 Quy tắc nghiệp vụ các trường dữ liệu quan trọng

* **`Quantity`:** Số lượng gốc lúc thiết lập lô. Tuyệt đối **không UPDATE** trường này khi hao hụt hoặc chế biến. Mọi biến động số lượng phải được ghi nhận qua sự kiện tăng/giảm phụ thuộc (`QuantityAfter`).
* **`RemainingQuantity`:** Số lượng khả dụng còn lại của lô hàng, phục vụ trực tiếp cho các kịch bản tách lô (`Batch Split`). Khi lượng phân tách đạt tối đa, trường này trả về giá trị `0`.
* **`CurrentOrganizationId`:** Định vị tổ chức đang chịu trách nhiệm lưu kho/sở hữu lô hàng tại thời điểm hiện tại (Cập nhật liên tục khi hoàn thành sự kiện giao nhận `RECEIVE`).

---

## 8. Supply Chain Event Management

### 8.1 Định nghĩa

`SupplyChainEvent` chịu trách nhiệm ghi lại toàn bộ dấu vết lịch sử hoạt động tác động lên lô hàng.

### 8.2 Danh mục sự kiện cấu hình (`EventTypes`)

* `HARVEST` (Thu hoạch)
* `RECEIVE` (Nhận hàng)
* `PROCESSING` (Sơ chế/Chế biến)
* `PACKAGING` (Đóng gói)
* `TRANSPORT` (Vận chuyển)
* `DISTRIBUTION` (Phân phối)
* `RETAIL` (Bán lẻ)
* `SALE` (Bán hàng thành phẩm)
* `INSPECTION` (Kiểm định)
* `SPLIT` (Chia lô)
* `MERGE` (Gộp lô)
* `RECALL` (Thu hồi)

### 8.3 Cấu trúc dữ liệu bảng `SupplyChainEvents`

```sql
CREATE TABLE SupplyChainEvents
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BatchId UNIQUEIDENTIFIER NOT NULL,
    EventTypeId INT NOT NULL,
    OrganizationId UNIQUEIDENTIFIER NOT NULL,
    PerformedByUserId UNIQUEIDENTIFIER NOT NULL,
    Description NVARCHAR(500),
    QuantityAfter DECIMAL(18,2),
    UnitAfterId UNIQUEIDENTIFIER,
    Location NVARCHAR(200),
    PreviousHash VARCHAR(500),
    CurrentHash VARCHAR(500),
    EventTime DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

```

---

## 9. Hash Chain Mechanism

### 9.1 Nguyên lý bảo mật dữ liệu

Để đảm bảo tính toàn vẹn dữ liệu tối cao mà không cần vận hành một mạng lưới Blockchain phức tạp, hệ thống áp dụng cơ chế chuỗi mã hóa liên kết ngược **SHA-256 Hash Chain** ngay trong cơ sở dữ liệu quan hệ.

### 9.2 Luồng liên kết mã băm (Hash Logic)

Mỗi sự kiện được thêm vào chuỗi bắt buộc phải mang theo mã băm của sự kiện liền trước nó trong cùng một lô hàng (`BatchId`).

```text
[Event 1: HARVEST]     ───► CurrentHash: AAA111
                                   │
                                   ▼
[Event 2: PROCESSING]  ───► PreviousHash: AAA111  ───► CurrentHash: BBB222
                                                               │
                                                               ▼
[Event 3: TRANSPORT]   ───► PreviousHash: BBB222  ───► CurrentHash: CCC333

```

### 9.3 Công thức tính toán

Mã băm hiện tại được tạo lập bằng cách mã hóa tổng chuỗi ký tự của mã băm cũ kết hợp cùng toàn bộ dữ liệu trọng tải của sự kiện mới:

$$\text{CurrentHash} = \text{SHA256}(\text{PreviousHash} + \text{EventData})$$

### 9.4 Quy tắc hệ thống (Immutable Rules)

Bảng sự kiện tuân thủ nghiêm ngặt chỉ cho phép hành vi `INSERT`. Bất kỳ hành vi can thiệp thô nhằm `UPDATE` hoặc `DELETE` một bản ghi sự kiện trong quá khứ sẽ làm sai lệch chuỗi giá trị mã hóa liên tầng ($H_{\text{old}} \neq H_{\text{new}}$). Hệ thống sẽ ngay lập tức phát hiện chuỗi băm bị đứt gãy (`Hash Chain Broken`) và đưa ra cảnh báo gian lận dữ liệu.

---

## 10. Batch Split Management

### 10.1 Nghiệp vụ tách lô

Áp dụng khi một lô hàng lớn (ví dụ: lô nguyên liệu thô nhập kho) được chia nhỏ thành nhiều lô thành phẩm hoặc bán thành phẩm để vận chuyển tới các đơn vị bán lẻ khác nhau. Tác nhân thực hiện thường thuộc nhóm `Processor` hoặc `Distributor`.

### 10.2 Luồng xử lý dữ liệu (Split Flow)

1. Chọn Lô cha cần tách (`Parent Batch`).
2. Nhập số lượng mong muốn cho lô con và kiểm tra điều kiện kiểm soát: $\text{Quantity}_{\text{child}} \le \text{RemainingQuantity}_{\text{parent}}$.
3. Kích hoạt sự kiện nghiệp vụ `SPLIT`.
4. Hệ thống tự động trừ `RemainingQuantity` của lô cha, khởi tạo các bản ghi lô con mới tương ứng kèm mã QR độc lập.

### 10.3 Cấu trúc cơ sở dữ liệu phân tách lô

#### Bảng `BatchSplits`

```sql
CREATE TABLE BatchSplits
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ParentBatchId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

```

#### Bảng `BatchSplitDetails`

```sql
CREATE TABLE BatchSplitDetails
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SplitId UNIQUEIDENTIFIER NOT NULL,
    ChildBatchId UNIQUEIDENTIFIER NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL
);

```