# 🌾 Tài Liệu Nghiệp Vụ Chuẩn Hóa GS1 GTIN & Batch (GS1 GTIN & Batch Business Specification)

> **Hệ thống Truy xuất Nguồn gốc Nông sản AgriTrace**  
> **Phiên bản:** 1.0  
> **Kiến trúc sư/Chuyên gia phân tích:** User / Nhóm Phát Triển AgriTrace  

---

## 📋 1. Khái Niệm Cốt Lõi (Core Concepts)

### 1.1 GTIN là gì?
**GTIN (Global Trade Item Number)** là mã định danh thương mại toàn cầu của GS1 dùng để nhận diện một **loại sản phẩm/thương phẩm (Trade Item)** một cách duy nhất trên toàn cầu.

👉 **Quy tắc vàng:** GTIN nhận diện **sản phẩm (Product)**, còn Batch/Lot nhận diện **lô hàng cụ thể (Batch)**.

### 1.2 Phân biệt GTIN, Barcode và QR Code
- **GTIN**: Là chuỗi định danh số học (VD: `08931234500012`).
- **Barcode**: Là phương thức biểu diễn hình ảnh của GTIN (VD: mã vạch tuyến tính EAN-13, UPC-A) giúp máy quét quang học đọc được.
- **QR Code / GS1 Digital Link**: Là mã vạch 2D có khả năng cung cấp thông tin kỹ thuật số, truy xuất nguồn gốc và có thể chứa/liên kết tới định danh GS1 cũng như định danh Lô.
- **Quy tắc**: `GTIN ≠ Barcode ≠ QR Code`

---

## 🔄 2. Quy Trình Nghiệp Vụ GTIN (Business Flow)

Chu trình quản lý vòng đời sản phẩm với GTIN trải qua 6 bước:

1. **Đăng ký doanh nghiệp**: Tổ chức đăng ký hệ thống truy xuất.
2. **Cấp mã GS1**: Tổ chức trở thành thành viên GS1 và được cấp mã tiền tố doanh nghiệp (GS1 Company Prefix - VD: `89312345`).
3. **Tạo sản phẩm**: Khởi tạo danh mục sản phẩm (VD: Gạo ST25 5kg).
4. **Gán GTIN**: Gán số GTIN cho sản phẩm dựa trên Prefix (VD: `8931234500012`).
5. **Sản xuất (Batches)**: Ghi nhận các mẻ thu hoạch/sản xuất định kỳ, kế thừa GTIN từ sản phẩm.
6. **Truy xuất (Traceability)**: In tem QR, mã vạch để phân phối và quét truy xuất.

---

## 📐 3. Mô Hình Dữ Liệu (Data Modeling)

AgriTrace tuân thủ mô hình phân tách rõ ràng giữa **Thương phẩm (Product)** và **Lô sản xuất (Batch)**. Tuyệt đối không nhúng (duplicate) dữ liệu sai nguyên tắc.

### Sơ đồ Kiến trúc Định danh

```mermaid
graph TD
    O[Organization\n(GS1 Prefix: 89312345)] --> P[Product\n(Gạo ST25 5kg)]
    P -->|Gán duy nhất| G[GTIN: 8931234500012]
    
    G --> B1[Batch: LOT-2026-001\nQuantity: 10,000 KG]
    G --> B2[Batch: LOT-2026-002\nQuantity: 8,000 KG]
    G --> B3[Batch: LOT-2026-003\nQuantity: 15,000 KG]
```

### Nguyên tắc Cơ sở dữ liệu:
- **Bảng `Products`**: Lưu trữ trường `GTIN`. Một GTIN xác định duy nhất 1 bản ghi Trade Item trong phạm vi hệ thống GS1.
- **Bảng `Batches`**: Lưu trữ `BatchCode` và `ProductId`. Tuyệt đối **không lưu trữ riêng lẻ / copy cột GTIN vào Batch**. Khi cần truy xuất, Batch sẽ kế thừa động `GTIN` từ liên kết với `Product`.

---

## ⚖️ 4. Quy Tắc Nghiệp Vụ (Business Rules)

| STT | Tên Rule | Mô tả Nghiệp vụ |
|:---|:---|:---|
| 1 | **GTIN Bắt buộc** | Các sản phẩm đưa ra thị trường thương mại lớn (siêu thị, xuất khẩu) bắt buộc phải có GTIN hợp lệ. |
| 2 | **GTIN Unique (Duy nhất)** | Không cho phép 2 bản ghi `Product` sử dụng chung 1 mã GTIN. |
| 3 | **GTIN Không Đổi Tùy Tiện** | Không cho phép tự ý sửa mã GTIN sau khi sản phẩm đã phát hành hoặc đã có lô hàng phát sinh. Nếu sản phẩm thay đổi quy cách (từ 5kg sang 10kg), bắt buộc tạo `Product` mới và cấp GTIN mới. |
| 4 | **Kế Thừa Định Danh** | Lô hàng (`Batch`) không sở hữu GTIN độc lập mà phải kế thừa GTIN của Sản phẩm cha (`Product`). |
| 5 | **BatchCode Unique** | Mã lô (`BatchCode`) phải là duy nhất để truy xuất chính xác mẻ sản xuất. |
| 6 | **GS1 Identification** | Quét mã Barcode GTIN $\rightarrow$ Trả về `Product`. Quét mã QR có (GTIN + Batch) $\rightarrow$ Trả về chính xác thông tin 1 `Batch`. |
| 7 | **Recall Theo Batch** | Hệ thống Cảnh báo/Thu hồi khẩn cấp (Recall) phải được thiết kế trên cấp độ `Batch`, tuyệt đối không recall "toàn bộ GTIN" một cách tự động, để giới hạn thiệt hại kinh tế. |

---

## 🚨 5. Ứng Dụng Trong Nghiệp Vụ Thu Hồi (Recall)

Mô hình GTIN + Batch là xương sống của tính năng **Recall Management**.

Nếu phát hiện lô hàng `LOT-002` bị nhiễm nấm mốc:
1. Xác định `GTIN` (VD: Gạo ST25 5kg).
2. Khoanh vùng **Affected Batch** là `LOT-002`. Lô `LOT-001` và `LOT-003` hoàn toàn an toàn và được tiếp tục bán.
3. Hệ thống AgriTrace sẽ lần theo vết băm (Hash chain) của riêng `LOT-002` qua các node: `Processor` $\rightarrow$ `Distributor` $\rightarrow$ `Retailer` để gửi thông báo thu hồi chính xác.

---
**Khuyến nghị quan trọng**: Không coi mã QR nội bộ là mã định danh chính của sản phẩm. Mã QR chỉ là phương tiện chứa **Digital Link**, còn định danh gốc phải là `GTIN` (Product) và `BatchCode` (Batch).
