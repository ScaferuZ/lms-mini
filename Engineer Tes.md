## **PRD Mini — Full Stack Monolith (.NET + Blazor)**

---

### **Project Module:**

**“Mini Assignment & Knowledge Check Module”**

---

### **Objective:**

Membuat **mini LMS sub-module**: user menerima **assignment** → membuka materi → menjawab quiz → submit → progress tercatat → manager bisa lihat hasil.

---

### **Stack & Architecture:**

- **Framework:** .NET 6+ ([ASP.NET](http://ASP.NET) Core)
- **Front-end:** Blazor Server atau Blazor WASM + API (pilih sesuai preferensi)
- **Database:** SQL Server atau PostgreSQL
- **Auth:** JWT atau Cookie-based (pilih salah satu)
- **Architecture:** Monolith clean-layered (Controller → Service → Repository)

---

### **User Stories:**

- **Learner**: melihat list assignment → buka materi + quiz → jawab → submit.
- **Manager**: melihat hasil assignment + skor user bawahan.

---

### **Fitur Utama:**

1️⃣ **Learner**

- **View List**: halaman Blazor dengan tabel assignment aktif.
- **Open Assignment**: halaman detail → materi (link PDF / embed video) + quiz (5 soal MCQ).
- **Submit Answer**: form submit, simpan skor.

2️⃣ **Manager**

- **Progress Report Page**: list user + assignment + skor + status (completed/pending).

---

### **Rules:**

- User hanya bisa submit jawaban **1x per assignment**.
- Skoring: 1 soal benar = 20 poin → total 5 soal.
- Progress tersimpan di table AssignmentProgress.
- Manager hanya bisa lihat user bawahan (contoh: filter RoleId).

---

### **Non-Functional:**

- Blazor page minimal: Home, List Assignment, Assignment Detail, Report.
- Backend: REST API di Controller.
- DB migration pakai EF Core.
- Unit test minimal untuk logic scoring.
- Authentication + Authorization basic.

---

### **Deliverables:**

✅ Blazor project source code  
 ✅ Simple SQL Migration atau EF Core migration script  
 ✅ Postman Collection (optional, untuk test API)  
 ✅ Readme cara run & schema ERD sederhana

---

### ✅ **Sukses Criteria:**

- Bisa **run local** ([localhost](http://localhost):port)
- Halaman Blazor clean (tidak perlu styling fancy, cukup fungsional)
- Routing login/logout & auth role basic
- CRUD logic modular, tidak hardcode di razor page
- Logic quiz/scoring jalan
- DB structure clean