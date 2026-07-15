# Agricultural Supply Chain Traceability System
## Business Process Specification

**Version:** 2.0

## 1. System Overview

Hệ thống quản lý toàn bộ vòng đời của một lô nông sản từ:

1. Farm
2. Processing
3. Packaging
4. Transportation
5. Distribution
6. Retail
7. Consumer

Mỗi Batch có:

- Batch Code duy nhất
- QR Code
- Product
- Current Organization
- Supply Chain Timeline

Mọi hoạt động tạo ra một **SupplyChainEvent**.

**Event:**

- Không update
- Không delete
- Append-only

Tính toàn vẹn dùng: **SHA-256 Hash Chain**

## 2. Actors

### 2.1 Admin

Quản trị toàn hệ thống.

**Responsibilities:**

- Quản lý Organization
- Quản lý User
- Quản lý Role
- Quản lý Event Type
- Tạo Recall
- Monitor system

**Database:**

- Users
- Organizations
- EventTypes

### 2.2 Manager

Đại diện quản lý một doanh nghiệp.

**Ví dụ:**

- Da Lat Farm
- Vin Distribution
- ABC Retail

**Responsibilities:**

- Quản lý nhân viên trong tổ chức
- Quản lý sản phẩm
- Quản lý Batch
- Xem báo cáo

**Database:**

- Products
- Batches
- Users

### 2.3 Farmer

Đã tách riêng. Không phải Staff.

**Responsibilities:**

- Tạo Batch mới
- Nhập thông tin thu hoạch
- Tạo Harvest Event

**Ví dụ:**

```
Harvest tomato 1000kg
Generate QR
```

**Database:**

- Batches
- SupplyChainEvents

### 2.4 Staff

Nhân viên vận hành trong Processor / Distributor / Retailer.

**Ví dụ:**

- Nhân viên kho.

**Responsibilities:**

- Receive Batch
- Processing Event
- Packaging Event
- Transport Event
- Distribution Event
- Split Batch
- Merge Batch

**Database:**

- SupplyChainEvents
- BatchSplits
- BatchMerges

### 2.5 Inspector

Người kiểm định.

**Responsibilities:**

- Kiểm tra chất lượng
- Upload certificate
- Tạo inspection report

**Database:**

- QualityInspections
- Certificates

### 2.6 Consumer

Không login.

**Responsibilities:**

- Scan QR
- View timeline
- View certificate
- View recall

**Database (Read only):**

- Batches
- Events
- Certificates
- Recalls

## 3. Main Business Processes

### BP01 - User Authentication

**Actor:** User

**Flow:**

1. Login
2. Input Email Password
3. Validate
4. Generate JWT Access Token
5. Generate Refresh Token
6. Return User Profile

**Database:** Users

### BP02 - Organization Management

**Actor:** Admin

**Flow:**

1. Create Organization
2. Select Organization Type (FARM/PROCESSOR/DISTRIBUTOR/RETAILER)
3. Create Manager
4. Activate Organization

**Output:** Organization, User

**Database:**

- Organizations
- Users
- OrganizationTypes

### BP03 - Product Management

**Actor:** Manager

**Flow:**

1. Create Category
2. Create Unit
3. Create Product
4. Assign Product Owner Organization
5. Product Available

**Database:**

- Products
- Categories
- Units

### BP04 - Batch Creation

**Actor:** Farmer

**Flow:**

1. Create Batch
2. Select Product
3. Input Harvest Date
4. Input Quantity
5. Generate Batch Code
6. Generate QR Code
7. Save Batch
8. Create Harvest Event

**Database:**

- Batches
- SupplyChainEvents

### BP05 - Supply Chain Event Management ⭐

**Actor:** Staff, Farmer

**Flow:**

1. Scan QR
2. Find Batch
3. Validate Permission
4. Select Event Type
5. Input Event Data
6. Generate Previous Hash
7. Generate Current Hash
8. Save Event
9. Update Batch Status

**Example:**

- **Farmer:** HARVEST
- **Processor:** PROCESSING, PACKAGING
- **Distributor:** TRANSPORT, DISTRIBUTION
- **Retail:** RETAIL

**Database:** SupplyChainEvents

### BP06 - Batch Split

**Actor:** Staff

**Example:**

```
1000kg Rice
    ↓
Split
    ↓
400kg Shop A
600kg Shop B
```

**Flow:**

1. Select Batch
2. Input Split Quantity
3. Validate Remaining Quantity
4. Create Child Batch
5. Create BatchSplit
6. Create Split Event
7. Generate QR

**Database:**

- Batches
- BatchSplits
- BatchSplitDetails
- SupplyChainEvents

### BP07 - Batch Merge

**Actor:** Staff

**Example:**

```
Batch A + Batch B = Batch C
```

**Flow:**

1. Select Source Batches
2. Validate Same Product
3. Create New Batch
4. Create Merge Record
5. Create Merge Event
6. Generate QR

**Database:**

- Batches
- BatchMerges
- BatchMergeSources
- SupplyChainEvents

### BP08 - Quality Inspection

**Actor:** Inspector

**Flow:**

1. Select Batch
2. Perform Inspection
3. Input Result
4. Upload Certificate
5. Save Inspection

**Database:**

- QualityInspections
- Certificates

### BP09 - Product Recall

**Actor:** Admin, Inspector

**Flow:**

1. Select Batch
2. Input Reason
3. Select Severity
4. Create Recall
5. Find Related Organizations
6. Send Notification

**Database:**

- Recalls
- Notifications

### BP10 - Public Traceability

**Actor:** Consumer

**Flow:**

1. Scan QR
2. Open Public URL
3. Find Batch
4. Load Events
5. Load Inspection
6. Load Certificate
7. Load Recall Status
8. Display Timeline

**Database:**

- Batches
- SupplyChainEvents
- QualityInspections
- Certificates
- Recalls

## 4. Business Rules

### Batch Rule

Một Batch:

- belongs to one Product
- belongs to one Current Organization
- has one QR Code

### Event Rule

Một Event:

- belongs to one Batch
- belongs to one Organization
- belongs to one User
- belongs to one EventType

**Không được:**

- UPDATE
- DELETE

### Hash Chain Rule

**Ví dụ:**

```
Event 1:
  CurrentHash = ABC123

Event 2:
  PreviousHash = ABC123
  CurrentHash = XYZ789
```

Nếu sửa Event 1 → **Hash chain broken**

## 5. Database Mapping Check

| Business Process | Database |
|------------------|----------|
| Login | Users |
| Organization | Organizations |
| Product | Products |
| Batch | Batches |
| Event | SupplyChainEvents |
| Split | BatchSplits |
| Merge | BatchMerges |
| Inspection | QualityInspections |
| Certificate | Certificates |
| Recall | Recalls |
| Trace | All read tables |