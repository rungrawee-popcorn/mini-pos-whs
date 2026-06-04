# UAT CHECKLIST - MINI POS & WHS SYSTEM

## 👤 1. Authentication

- [ ] Admin login ได้
- [ ] Login ด้วย password ถูกต้องเท่านั้น
- [ ] Login ผิด → ต้องเข้าไม่ได้
- [ ] Logout แล้ว session หาย

---

## 📦 2. Product Management

- [ ] เพิ่มสินค้าได้
- [ ] ห้ามเพิ่ม ProductCode ซ้ำ
- [ ] แก้ไขสินค้าได้
- [ ] ลบสินค้าได้
- [ ] ค้นหาสินค้าได้
- [ ] ราคาห้ามติดลบ
- [ ] Stock ห้ามติดลบ

---

## 🛒 3. POS (Sales)

- [ ] เพิ่มสินค้าเข้าตะกร้าได้
- [ ] แสดงราคารวมถูกต้อง
- [ ] Checkout สำเร็จ
- [ ] สร้าง Sale record ได้
- [ ] สร้าง SaleDetail ได้
- [ ] ตัด Stock ถูกต้อง
- [ ] Stock ไม่ติดลบ
- [ ] ขายของเกิน stock → ต้อง error

---

## 📉 4. Stock Management

- [ ] Stock ลดเมื่อขาย
- [ ] Stock เพิ่มได้
- [ ] มี StockTransaction log
- [ ] Transaction type ถูกต้อง (IN / OUT)

---

## 📊 5. Dashboard

- [ ] แสดงจำนวนสินค้าทั้งหมด
- [ ] ยอดขายวันนี้ถูกต้อง
- [ ] สินค้าใกล้หมดแสดงถูกต้อง
- [ ] Top selling product ถูกต้อง

---

## 🔐 6. Security

- [ ] เข้าหน้า system โดยไม่ login → ต้อง block
- [ ] SQL injection ไม่ผ่าน
- [ ] XSS ไม่เกิด
- [ ] session timeout ทำงาน

---

## ⚙️ 7. System Behavior

- [ ] ระบบไม่ crash เมื่อข้อมูลผิด
- [ ] Error message เข้าใจง่าย
- [ ] ระบบทำงานต่อได้หลัง error

---

## 📱 8. Responsive UI

- [ ] Desktop ใช้งานได้
- [ ] Tablet layout ไม่พัง
- [ ] Mobile responsive

---

## ✅ FINAL RESULT

- [ ] System Ready for Production
- [ ] No critical bug
