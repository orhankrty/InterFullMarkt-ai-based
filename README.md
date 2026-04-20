<div align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10.0" />
  <img src="https://img.shields.io/badge/Architecture-Clean-brightgreen?style=for-the-badge" alt="Clean Architecture" />
  <img src="https://img.shields.io/badge/Pattern-CQRS-blue?style=for-the-badge" alt="CQRS" />
  <img src="https://img.shields.io/badge/Library-MediatR-yellow?style=for-the-badge" alt="MediatR" />

  <h1>🏟️ InterFullMarkt <br/> <small>AI-Powered International Football Market Portal</small></h1>

  <p><strong>Yeni Nesil B2B & B2C Veri, Transfer ve Yetenek Keşif (Scouting) Platformu</strong></p>
</div>

<br/>

**InterFullMarkt**, uluslararası futbol dünyasının piyasa değerlerini ve transfer dinamiklerini analiz eden **Yapay Zeka (AI) destekli** şeffaf bir platformdur. 

Proje, güncel yazılım mühendisliği pratikleri ışığında; **Clean Architecture (Temiz Mimari)** ve **Domain-Driven Design (DDD)** ilkelerine sadık kalınarak kurumsal standartlarda geliştirilmiştir. Arayüzünde modern ve premium *"Glassmorphism"* tasarım ögeleri barındırır.

## ✨ Öne Çıkan Özellikler (Core Features)

- 🧠 **Yapay Zeka Destekli Tahminler:** Oyuncuların biyometrik verileri, taktiksel ölçümleri ve kulüp gücüne dayanarak gerçeğe en yakın potansiyel piyasa değeri (Mock) üretir.
- 🔄 **Akıllı Transfer Motoru:** Transfer süreçlerinde bütçe, kapasite ve sözleşmeleri eş zamanlı doğrular. Süreç, `MediatR Command` ve `Domain Events` ile asenkron yürütülür.
- 🛡️ **Zengin Domain Modelleri:** İş kurallarını (Business Logic) kendi çekirdeğinde muhafaza eden zengin varlıklar sınıfıyla (Rich Domain) veri bütünlüğünü sağlar.
- 🚀 **MediatR & CQRS Pipeline:** Read ve Write operasyonlarının kesin izolasyonu; `AutoMapper` ve `FluentValidation` entegrasyonu ile merkezi doğrulama.
- 🌐 **Global Audit & Soft Delete:** Sistem genelinde silinmiş verilerin izolasyonu ve `SaveChanges` sırasında çalışan otomatik Audit Trail (Serilog destekli).

---

## 🏛️ Mimari Tasarım (Clean Architecture)

Proje, bağımlılıkların yalnızca merkeze (Domain) doğru ilerlediği 4 temel katmanda dizayn edilmiştir:

1. **🎯 Domain Katmanı (Core):** Projenin kalbi. Varlıklar, Enum'lar ve Değer Nesneleri (`Money`) harici bağımlılık olmadan burada yer alır.
2. **🧠 Application Katmanı (Use Cases):** CQRS yaklaşımıyla ayrıştırılan komut/sorgular ve DTO eşleştirmeleri MediatR vasıtasıyla orkestre edilir.
3. **🧱 Infrastructure Katmanı (Data):** Veri erişim modülü (EF Core 10), interceptor'lar ve dış sistem bağlantılarını içerir.
4. **🌐 WebUI Katmanı (Presentation):** ASP.NET Core MVC ile tasarlanmış modern bir yapı. Büyüleyici Controller ve Global Exception Handling mekanizmalarıyla doğrudan kullanıcı deneyimine (UX/UI) odaklanır.

---

## ⚙️ Teknoloji Yığını (Tech Stack)

| Kategori         | Kullanılan Teknolojiler |
| :---             | :--- |
| **Backend**      | .NET 10.0 (C# 14), ASP.NET Core MVC |
| **Database**     | Entity Framework Core 10, SQLite, SQL Server |
| **Architecture** | Clean Architecture, CQRS, DDD |
| **Frontend**     | Razor Pages, Bootstrap 5, Chart.js, Glassmorphism |

---

## 🚀 Kurulum (Getting Started)

1. Projeyi bilgisayarınıza klonlayın:
   ```bash
   git clone https://github.com/your-username/InterFullMarkt--ai-based-.git
   ```
2. Web uygulaması klasörüne geçiş yapın:
   ```bash
   cd InterFullMarkt-ai-based/InterFullMarkt.WebUI
   ```
3. Projeyi başlatın:
   ```bash
   dotnet watch
   ```
   > 💡 *Sistem başlatıldığında `FullMarkt.db` veritabanı otomatik oluşturulur ve test/deneyim verileri (Galatasaray, Real Madrid vb.) doğrudan tohumlanarak (Seed) sisteme eklenir.*

---

## 🗺️ Yol Haritası (Roadmap)

- [x] Temel varlıkların kurulması ve Rich Domain altyapısı.
- [x] CQRS Pipeline, DTO eşleştirmesi ve Fluent Validation konfigürasyonu.
- [x] Akıllı transfer motoru ve MediatR Domain Event mantığı.
- [x] Premium (Glassmorphism) kullanıcı arayüzü tasarımı.
- [ ] **Grafiksel Analiz:** Chart.js kullanarak detay ekranlarında dinamik piyasa zaman çizelgelerinin görüntülenmesi.
- [ ] **Birim Testleri:** `xUnit` ve `Moq` kütüphaneleriyle kapsamlı birim (Unit) test süreçleri.
- [ ] **Güvenlik & Rolleme:** ASP.NET Core Identity ile Admin / Kullanıcı kimlik doğrulamasının projeye kazandırılması.

---

<div align="center">
  <p><i>"Veri tabanından Kullanıcı Arayüzü'ne, kodun her satırı performans, zarafet ve zafer için yazıldı."</i></p>
  <p>Tasarım ve Geliştirme: <b>Hüseyin Orhan Kırtay</b></p>
</div>