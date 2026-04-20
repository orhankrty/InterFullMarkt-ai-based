<div align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet" alt=".NET 10.0 Badge" />
  <img src="https://img.shields.io/badge/Architecture-Clean-brightgreen?style=for-the-badge" alt="Clean Architecture Badge" />
  <img src="https://img.shields.io/badge/Pattern-CQRS-blue?style=for-the-badge" alt="CQRS Badge" />
  <img src="https://img.shields.io/badge/Library-MediatR-yellow?style=for-the-badge" alt="MediatR Badge" />

  <h1>🏟️ InterFullMarkt <br/> <small>AI-Powered International Football Market Portal</small></h1>

  <p><strong>Yeni Nesil B2B & B2C Veri, Transfer ve Yetenek Keşif (Scouting) Platformu</strong></p>
</div>

<br/>

**InterFullMarkt**, uluslararası futbol dünyasının piyasa değerlerini, transfer dinamiklerini ve oyuncu istatistiklerini profesyonel ve yenilikçi bir perspektifle analiz etmek amacıyla geliştirilmiş, **Yapay Zeka (AI) destekli** şeffaf bir platformdur. 

Proje, güncel yazılım mühendisliği pratikleri ışığında; tamamen **Clean Architecture (Temiz Mimari)** ve **Domain-Driven Design (DDD)** ilkelerine sadık kalınarak "Kurumsal Düzey" (Enterprise-Level) standartlarda kodlanmıştır. Ön yüzünde ise kullanıcıyı içine çeken, Premium *"Glassmorphism"* ve modern tasarım ögeleri barındırır.

## ✨ Öne Çıkan Özellikler (Core Features)

- 🧠 **Yapay Zeka Destekli Tahminler (AI Market Forecast):** Oyuncuların biyometrik verileri, taktiksel performans ölçümleri ve güncel forma giydikleri kulübün gücüne dayanarak; bir sonraki sezon için gerçeğe en yakın **potansiyel piyasa değeri** tahminleri (Mock) üretir.
- 🔄 **Akıllı Transfer Motoru (Smart Transfer Engine):** Transfer süreçleri sırasında kulübün bütçesini, takım kadrosu kapasitesini ve aktif sözleşmeleri eş zamanlı olarak doğrular. İşlem, `MediatR Command` ve `Domain Events` uçları kullanılarak zırhlı ve asenkron biçimde yürütülür.
- 🛡️ **Zengin & Katı Domain Modelleri (Rich Domain Models):** İş kurallarını (Business Logic) kendi çekirdeğinde muhafaza eden (`CompleteTransfer`, `DeductFromBudget` vb.) zengin sınıflar; böylece "Anemik Model" tuzağından uzak ve veri bütünlüğü yüksek bir mimari inşa eder.
- 🚀 **MediatR & CQRS Pipeline:** Read (Okuma) ve Write (Yazma) operasyonlarının kesin bir şekilde izolasyonu. Ayrıca süreç `AutoMapper` ve `FluentValidation` entegrasyonu ile merkezi doğrulama davranışında işlenir.
- 🌐 **Global Audit & Güvenli Silme (Soft Delete):** EF Core query (sorgu) filtreleri ile sistem genelinde silinmiş verilerin güvenli izolasyonu. `SaveChanges` sırasında devreye giren otomatik Loglama / Audit Trail mekanizması (Serilog destekli).

---

## 🏛️ Mimari Tasarım (Clean Architecture)

Proje, bağımlılıkların yalnızca merkeze (Domain) doğru ilerlediği, dış araçlardan izole 4 temel katmanda dizayn edilmiştir:

<details>
<summary><b>1. 🎯 Domain Katmanı (Core)</b></summary>
Projenin kalbi burasıdır. Varlıklar (`Player`, `Club`), Enum türleri, Değer Nesneleri (Value Objects - `Money`, `Nationality`) ve sistem arayüzleri yalnızca bu katmanda yer alır. Harici hiçbir kütüphaneye veya veritabanı sürücüsüne (EF Core vb.) bağımlılığa sahip değildir.
</details>

<details>
<summary><b>2. 🧠 Application Katmanı (Use Cases)</b></summary>
İş zekası ve sistemin kullanım senaryoları burada yürütülür. CQRS yaklaşımıyla ayrıştırılan sorgular/komutlar (Query/Command) ve DTO eşleştirmeleri MediatR vasıtasıyla orkestre edilir. Ayrıca Custom Pipeline Behavior ve Validation ayarları da bu katmandadır.
</details>

<details>
<summary><b>3. 🧱 Infrastructure Katmanı (Data & External)</b></summary>
Sistemin dış dünya ile haberleştiği bölüm. Veri erişim katmanı (Entity Framework Core 10), veritabanı interceptor'ları, Fluent API konfigürasyonları ve dış API sistem tasarımlarını ihtiva eder.
</details>

<details>
<summary><b>4. 🌐 WebUI Katmanı (Presentation)</b></summary>
Kullanıcı deneyiminin (UX/UI) vücut bulduğu alandır. ASP.NET Core yapısıyla kurulan Controller'lar, büyüleyici bir arayüze sahip gelişmiş Razor View'lar, Global Exception Handling mekanizması (Middleware) ile süslenmiştir.
</details>

---

## ⚙️ Teknoloji Yığını (Tech Stack)

| Kategori         | Kullanılan Teknolojiler & Araçlar|
| :---             | :--- |
| **Backend**      | .NET 10.0 (C# 14), ASP.NET Core MVC |
| **Database**     | Entity Framework Core 10, SQLite (Test/Dev), SQL Server |
| **Architecture** | Clean Architecture, CQRS Pattern, Domain-Driven Design |
| **Frontend**     | Razor Pages, Bootstrap 5, Chart.js, Glassmorphism Styling |
| **Packages**     | MediatR, AutoMapper, FluentValidation, Serilog |

---

## 🚀 Kurulum (Getting Started)

Projeyi bilgisayarınızda ayağa kaldırıp derlemek için aşağıdaki adımları kullanabilirsiniz:

### Gereksinimler:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2022 (Tercihen) veya VS Code

### Adımlar:
1. **Çalışmayı başlatmak için projeyi makinenize klonlayın:**
   ```bash
   git clone https://github.com/your-username/InterFullMarkt--ai-based-.git
   ```

2. **Web (UI) uygulamasının barındığı klasöre girin:**
   ```bash
   cd InterFullMarkt-ai-based/InterFullMarkt.WebUI
   ```

3. **Gerekli bağımlılıkları indirin ve projeyi başlatın:**
   ```bash
   dotnet watch
   ```
   > 💡 *Not: Proje başarıyla başlatıldığında, Entity Framework Core otomatik bir şekilde `FullMarkt.db` veritabanını oluşturur ve arkasından Galatasaray, Real Madrid gibi efsanevi kulüpleri/oyuncuları platforma tohumlayarak (Seed Data) test için kullanımınıza sunar.*

4. Açılan komut penceresinde belirtilen lokal adrese (Genellikle `https://localhost:5001` veya `http://localhost:5000`) gidip sistemi keşfetmeye başlayabilirsiniz!

---

## 🗺️ Yol Haritası (Roadmap)

- [x] Temel Varlıkların (Entities) Kurulması ve Rich Domain Altyapısı.
- [x] CQRS Pipeline, AutoMapper DTO Eşleştirmesi ve Fluent Validation Sistemi.
- [x] Gelişmiş Transfer Motoru, MediatR Domain Event Publisher Mantığı.
- [x] Premium (Glassmorphism & Yeni Nesil) Kullanıcı Arayüzü Tasarımı.
- [ ] **UI Geliştirmesi:** Chart.js kütüphanesi kullanarak detay ekranlarında dinamik zaman çizelgeli Market Value grafik analizlerinin yapılması.
- [ ] **Birim Testleri (Testing):** `xUnit` ve `Moq` kütüphanelerinin sisteme dahil edilmesi; Application/Domain modülleri için kapsamlı test süreci.
- [ ] **Güvenlik (Security):** ASP.NET Core Identity kimlik doğrulamasının projeye dahil edilerek Admin / Kullanıcı rollerinin oturtulması.

---

<div align="center">
  <p><i>"Veri tabanından Kullanıcı Arayüzü'ne, kodun her satırı performans, zarafet ve zafer için yazıldı."</i></p>
  <p>Geliştiren: <b>Orhan & AI Team</b></p>
</div>