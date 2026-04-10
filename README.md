# 🏟️ InterFullMarkt (International Football Market)

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen?style=for-the-badge)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-blue?style=for-the-badge)
![MediatR](https://img.shields.io/badge/Library-MediatR-yellow?style=for-the-badge)

**InterFullMarkt**, uluslararası futbol dünyasının piyasa değerlerini, transfer dinamiklerini ve oyuncu istatistiklerini izlemek amacıyla geliştirilmiş, **AI destekli** yeni nesil bir B2B / B2C veri portalıdır. 

Sistem; Rich Domain Model, CQRS Pattern ve Clean Architecture ilkeleri göz önünde bulundurularak "Enterprise-Level" standartlarda tasarlanmıştır.

## 🌟 Temel Özellikler (Core Features)

- **🧠 AI Market Forecast (Mock):** Oyuncunun biyometrik verileri, yaşı, performansı ve mevcut kulübünün ağırlığını baz alarak bir sonraki sezon için tahmini piyasa değeri (Market Value) sinyalleri üretir.
- **🔄 Akıllı Transfer Motoru:** Bir kulübün bütçesini, kadro limitini ve oyuncu sözleşmelerini dikkate alan zırhlı transfer süreçleri. (MediatR Command + Domain Events üzerinden işler).
- **🛡️ Solid & Rich Domain Models:** Anemik modellerden (Anemic Domain Models) uzak, iş kurallarını (Business Logic) kendi içinde barındıran (`CompleteTransfer`, `DeductFromBudget`) güçlü Entity ve Value Object'ler (`Money`, `Nationality`).
- **🚀 CQRS & Mediator Pipeline:** Read/Write operasyonlarının sıkı izolasyonu, AutoMapper ve FluentValidation entegrasyonu ile merkezi doğrulama davranışı (ValidationBehavior).
- **🌐 Global Audit & Soft Delete:** Veritabanı üzerinde global query filter'lar ile silinmiş (IsDeleted = true) verilerin izolasyonu ve SaveChanges esnasında devreye giren otomatik Audit Trail mekanizması.

---

## 🏛️ Mimari Tasarım (Clean Architecture)

Proje, bağımlılıkların yalnızca içe (Domain'e) doğru aktığı 4 temel katmandan oluşmaktadır:

1. **`Domain` Katmanı:** Projenin kalbi. Entity'ler (`Player`, `Club`), Enum'lar, Value Object'ler (`Money`) ve sistem arayüzleri burada yer alır. Hiçbir dış teknolojiye (EF Core vb.) bağımlılığı yoktur.
2. **`Application` Katmanı:** İş zekası ve orkestrasyon. Use-case senaryoları CQRS (Command/Query) mantığı ile MediatR üzerinden yönetilir. DTO maplemeleri ve kural doğrulamaları buradadır.
3. **`Infrastructure` Katmanı:** Veri erişim teknolojisi. Entity Framework Core (SQLite), Interceptor'lar, Fluent API konfigurasyonları ve dış servis implementasyonları yer alır.
4. **`WebUI` Katmanı:** Kullanıcı etkileşiminin başladığı nokta. ASP.NET Core MVC yapısı, Controller'lar, Razor View'lar ve modern UI elementleri (Chart.js vb.).

---

## ⚙️ Teknoloji Yığını (Tech Stack)

- **Framework:** .NET 10.0 (C# 14 özellikleriyle birlikte)
- **Frontend:** ASP.NET Core MVC, Razor Pages, Bootstrap 5, Chart.js
- **ORM:** Entity Framework Core 10
- **Veritabanı:** SQLite (Geliştirme / Hızlı Başlangıç için), SQL Server (Scale edilebilir)
- **Kütüphaneler:** 
  - `MediatR` (CQRS ve Domain Events)
  - `AutoMapper` (Object-to-Object Mapping)
  - `FluentValidation` (Gelişmiş kural doğrulaması)

---

## 🚀 Kurulum & Başlangıç (Getting Started)

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

### Ön Koşullar:
- .NET 10.0 SDK
- Tercihen Visual Studio 2022 veya VS Code

### Çalıştırma Adımları:
1. Depoyu bilgisayarınıza klonlayın.
   ```bash
   git clone https://github.com/your-username/InterFullMarkt--ai-based-.git
   ```
2. Uygulamanın çalışacağı ana WebUI dizinine gidin.
   ```bash
   cd InterFullMarkt-ai-based/InterFullMarkt.WebUI
   ```
3. Gerekli NuGet paketlerinin yüklenmesi ve uygulamanın Hot-Reload ile başlatılması için:
   ```bash
   dotnet watch
   ```
4. Proje çalıştığında EF Core, `FullMarkt.db` veritabanını oluşturup içine otomatik olarak Seed Data (Örn: Real Madrid, Manchester City ve elit oyuncular) ekleyecektir.
5. Tarayıcınızda `http://localhost:5000` (veya ilgili port) adresine giderek uygulamaya erişin!

---

## 🔍 İleriye Dönük Yol Haritası (Roadmap)

- [x] Temel Entity'lerin ve Rich Domain kurallarının kurgulanması.
- [x] CQRS Pipeline, AutoMapper ve Validation altyapısı.
- [x] Transfer Motoru ve Domain Event Publisher mimarisi.
- [ ] UI: Chart.js ile "Player Details" ekranında 12 aylık Market Value grafiği çizimi.
- [ ] Testing: `xUnit` ve `Moq` kütüphaneleriyle Application katmanındaki Command Handler'lar için Unit Test yazılması.
- [ ] Security: ASP.NET Core Identity ile Auth süreçlerinin (Admin / User) projeye dahil edilmesi.

---

<div align="center">
  <p><i>"Veri tabanından UI'a, kodun her satırı zafer için yazıldı."</i></p>
  <p><b>Orhan & AI Team</b></p>
</div>