

# Agricultural Supply Chain Traceability System

## Database Design v4.0

### 1. Architecture Overview

Database gồm các nhóm phân hệ sau:

* **Master Data:** `OrganizationTypes`, `EventTypes`, `Categories`, `Units`
* **Identity:** `Users`
* **Business Core:** `Organizations`, `Products`, `Batches`
* **Traceability:** `SupplyChainEvents`, `BatchSplit`, `BatchMerge`
* **Quality:** `QualityInspections`, `Certificates`
* **Safety:** `Recalls`, `Notifications`

---

### 2. Organization Domain

#### OrganizationTypes

Lưu loại tổ chức trong chuỗi cung ứng.

```sql
CREATE TABLE OrganizationTypes
(
    Id INT IDENTITY PRIMARY KEY,
    Code VARCHAR(50) UNIQUE NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500)
);

```

**Ví dụ dữ liệu:**

| Code | Name |
| --- | --- |
| `FARM` | Farm |
| `PROCESSOR` | Processor |
| `DISTRIBUTOR` | Distributor |
| `RETAILER` | Retailer |

#### Organizations

Đại diện cho các đơn vị tham gia chuỗi.

```sql
CREATE TABLE Organizations
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrganizationTypeId INT NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500),
    Status INT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT FK_Organizations_Types
    FOREIGN KEY(OrganizationTypeId) REFERENCES OrganizationTypes(Id)
);

```

**Ví dụ dữ liệu:**

* **Name:** Đà Lạt Farm
* **Type:** `FARM`

---

### 3. User & Authentication

#### Users

Phục vụ JWT Authentication.

```sql
CREATE TABLE Users
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrganizationId UNIQUEIDENTIFIER NULL,
    FullName NVARCHAR(200),
    Email VARCHAR(200) UNIQUE NOT NULL,
    PasswordHash VARCHAR(500),
    Role VARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    CONSTRAINT FK_Users_Organizations
    FOREIGN KEY(OrganizationId) REFERENCES Organizations(Id)
);

```

**Các Role hệ thống:**

* `SYSTEM_ADMIN`
* `ORG_ADMIN`
* `FARMER`
* `OPERATOR`
* `INSPECTOR`

---

### 4. Product Domain

#### Categories

```sql
CREATE TABLE Categories
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100),
    Description NVARCHAR(500)
);

```

#### Units

**Ví dụ dữ liệu:** `kg`, `ton`, `box`.

```sql
CREATE TABLE Units
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Code VARCHAR(20),
    Name NVARCHAR(100)
);

```

#### Products

```sql
CREATE TABLE Products
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrganizationId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER,
    UnitId UNIQUEIDENTIFIER,
    Name NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(OrganizationId) REFERENCES Organizations(Id),
    FOREIGN KEY(CategoryId) REFERENCES Categories(Id),
    FOREIGN KEY(UnitId) REFERENCES Units(Id)
);

```

---

### 5. Batch Aggregate Root ⭐

Đây là bảng quan trọng nhất trong hệ thống quản lý chuỗi cung ứng.

```sql
CREATE TABLE Batches
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    CurrentOrganizationId UNIQUEIDENTIFIER NOT NULL,
    BatchCode VARCHAR(100) UNIQUE NOT NULL,
    HarvestDate DATETIME2,
    Quantity DECIMAL(18,2),
    RemainingQuantity DECIMAL(18,2),
    Status INT,
    QRCode VARCHAR(500),
    ParentBatchId UNIQUEIDENTIFIER NULL,
    RootBatchId UNIQUEIDENTIFIER NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(ProductId) REFERENCES Products(Id),
    FOREIGN KEY(CurrentOrganizationId) REFERENCES Organizations(Id),
    FOREIGN KEY(ParentBatchId) REFERENCES Batches(Id)
);

```

**Các trạng thái (Status):**

* `CREATED`
* `HARVESTED`
* `PROCESSING`
* `TRANSPORTING`
* `DISTRIBUTED`
* `RETAIL`
* `RECALLED`

---

### 6. Event Type

#### EventTypes

```sql
CREATE TABLE EventTypes
(
    Id INT IDENTITY PRIMARY KEY,
    Code VARCHAR(50) UNIQUE,
    Name NVARCHAR(100)
);

```

**Dữ liệu mẫu (Code):**

* `HARVEST`
* `PROCESSING`
* `PACKAGING`
* `TRANSPORT`
* `DISTRIBUTION`
* `RETAIL`
* `INSPECTION`
* `RECALL`

---

### 7. Supply Chain Event ⭐⭐⭐

Bảng ghi vết dạng **Append-only** (chỉ thêm, không sửa/xóa).

```sql
CREATE TABLE SupplyChainEvents
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BatchId UNIQUEIDENTIFIER NOT NULL,
    EventTypeId INT NOT NULL,
    OrganizationId UNIQUEIDENTIFIER NOT NULL,
    PerformedByUserId UNIQUEIDENTIFIER NOT NULL,
    EventData NVARCHAR(MAX),
    Location NVARCHAR(200),
    PreviousHash VARCHAR(500),
    CurrentHash VARCHAR(500),
    EventTime DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(BatchId) REFERENCES Batches(Id),
    FOREIGN KEY(EventTypeId) REFERENCES EventTypes(Id),
    FOREIGN KEY(OrganizationId) REFERENCES Organizations(Id),
    FOREIGN KEY(PerformedByUserId) REFERENCES Users(Id)
);

```

**Ví dụ cấu trúc cấu hình động cho `Harvest Event` lưu tại trường `EventData`:**

```json
{
  "temperature": 25,
  "farm": "Da Lat",
  "worker": 10
}

```

---

### 8. Quality Inspection

```sql
CREATE TABLE QualityInspections
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BatchId UNIQUEIDENTIFIER NOT NULL,
    InspectorId UNIQUEIDENTIFIER NOT NULL,
    Status INT,
    Result NVARCHAR(500),
    Notes NVARCHAR(MAX),
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(BatchId) REFERENCES Batches(Id),
    FOREIGN KEY(InspectorId) REFERENCES Users(Id)
);

```

---

### 9. Certificate

```sql
CREATE TABLE Certificates
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BatchId UNIQUEIDENTIFIER NOT NULL,
    InspectionId UNIQUEIDENTIFIER NULL,
    CertificateType NVARCHAR(100),
    FileUrl VARCHAR(500),
    IssuedDate DATETIME2,

    FOREIGN KEY(BatchId) REFERENCES Batches(Id),
    FOREIGN KEY(InspectionId) REFERENCES QualityInspections(Id)
);

```

---

### 10. Recall

```sql
CREATE TABLE Recalls
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BatchId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(500),
    Severity INT,
    Status INT,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(BatchId) REFERENCES Batches(Id),
    FOREIGN KEY(CreatedBy) REFERENCES Users(Id)
);

```

**Mức độ nghiêm trọng (Severity):**

* `LOW`
* `MEDIUM`
* `HIGH`
* `CRITICAL`

---

### 11. Split Batch

**Ví dụ kịch bản tách lô:**

```
Batch A (1000kg) ───[ Split ]───► Batch B (400kg)
                                └──► Batch C (600kg)

```

#### BatchSplits

```sql
CREATE TABLE BatchSplits
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SourceBatchId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(SourceBatchId) REFERENCES Batches(Id)
);

```

#### BatchSplitDetails

```sql
CREATE TABLE BatchSplitDetails
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SplitId UNIQUEIDENTIFIER,
    TargetBatchId UNIQUEIDENTIFIER,
    Quantity DECIMAL(18,2),

    FOREIGN KEY(SplitId) REFERENCES BatchSplits(Id),
    FOREIGN KEY(TargetBatchId) REFERENCES Batches(Id)
);

```

---

### 12. Merge Batch

**Ví dụ kịch bản gộp lô:**

```
Apple A (Batch) ──┐
                  ├───[ Merge ]───► Apple C (New Batch)
Apple B (Batch) ──┘

```

#### BatchMerges

```sql
CREATE TABLE BatchMerges
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    NewBatchId UNIQUEIDENTIFIER,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);

```

#### BatchMergeSources

```sql
CREATE TABLE BatchMergeSources
(
    BatchMergeId UNIQUEIDENTIFIER,
    SourceBatchId UNIQUEIDENTIFIER,
    Quantity DECIMAL(18,2),

    PRIMARY KEY (BatchMergeId, SourceBatchId)
);

```

---

### 13. Notification

```sql
CREATE TABLE Notifications
(
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER,
    Title NVARCHAR(200),
    Message NVARCHAR(MAX),
    IsRead BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETDATE(),

    FOREIGN KEY(UserId) REFERENCES Users(Id)
);

```

---

### ERD Tổng Quan (Dạng text)

```text
OrganizationTypes
       │
       ▼
 Organizations
       │
       ▼
     Users


 Organizations
       │
       ▼
    Products
       │
       ▼
    Batches ◄─── BatchSplits / BatchMerges
       │
       ├─► SupplyChainEvents
       ├─► QualityInspections ──► Certificates
       └─► Recalls


     Users ──► Notifications

```