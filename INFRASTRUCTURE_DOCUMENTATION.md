## InterFullMarkt Infrastructure Katmanı - Dokumentasyon

### 📊 Yapılandırılan Kompnentler

#### 1. **InterFullMarktDbContext** (`Data/InterFullMarktDbContext.cs`)
- **Temel DbContext** sınıfı
- **.NET 10.0 Primary Constructor** ile yazılmış
- **4 DbSet**: Players, Clubs, Leagues, Transfers
- **Global Query Filter**: ISoftDelete arayüzüne sahip tüm varlıklar `IsDeleted == false` ile filtrelenir
- **Automatic Audit**: SaveChanges/SaveChangesAsync sırasında IAuditEntity verilerini otomatik doldurur
- **SQLite Uyumlu**: Türleri SQLite'a dönüştürülmüştür

---

#### 2. **Entity Type Configurations** (`Data/Configurations/`)

##### **PlayerConfiguration.cs**
- Tablo: `dbo.Players`
- Owned Types:
  - `Nationality_CountryName`, `Nationality_CountryCode`, `Nationality_FlagEmoji`
  - `MarketValue_Amount`, `MarketValue_Currency`
- Foreign Keys:
  - CurrentClubId → Club (nullable, SetNull on delete)
- Indexes: FullName, CurrentClubId, IsDeleted
- Validasyonlar: FullName (Max 150), PreferredFoot (Default: "Right")

##### **ClubConfiguration.cs**
- Tablo: `dbo.Clubs`
- Owned Types:
  - `Budget_Amount`, `Budget_Currency` (nullable)
- Foreign Keys:
  - LeagueId → League (Required, Restrict on delete)
  - OutgoingTransfers, IncomingTransfers Collections
- Unique Indexes: Name, ShortName
- Validasyonlar: Name (Max 200), ShortName (Max 10)

##### **LeagueConfiguration.cs**
- Tablo: `dbo.Leagues`
- Owned Types:
  - `Country_CountryName`, `Country_CountryCode`, `Country_FlagEmoji`
- Foreign Keys: Clubs Collection
- Unique Index: Name
- Validasyonlar: Name (Max 200), Tier (Integer), Coefficient (Decimal)

##### **TransferConfiguration.cs**
- Tablo: `dbo.Transfers`
- Owned Types:
  - `Fee_Amount`, `Fee_Currency`
- Foreign Keys:
  - FromClubId → Club (Restrict on delete)
  - ToClubId → Club (Restrict on delete)
  - PlayerId → Player (Cascade on delete)
- Composite Index: PlayerId + TransferDate (Unique) - Aynı oyuncu aynı gün içinde tek transfer
- Validasyonlar: TransferType (Max 50, Default: "Permanent")

---

#### 3. **Seed Data** (`Data/Seeds/SeedData.cs`)

Başta yüklenen veriler:

| Varlık | Bilgi |
|--------|-------|
| **La Liga** | Spain, Tier 1, Coefficient: 8.5 |
| **Premier League** | United Kingdom, Tier 1, Coefficient: 9.0 |
| **Real Madrid** | La Liga, Budget: 800M EUR, İnceleme Yılı: 1902 |
| **Manchester City** | Premier League, Budget: 900M EUR, İnceleme Yılı: 1880 |
| **Arda Güler** | Real Madrid, CM (Ortaçalı), Doğum: 24/02/2005, Pazar Değeri: 40M EUR |
| **Erling Haaland** | Manchester City, ST (Santrfor), Doğum: 21/07/2000, Pazar Değeri: 180M EUR |

---

#### 4. **Audit Interceptor** (`Data/Interceptors/AuditInterceptor.cs`)

SaveChanges öncesinde:
- ✅ Entity Added: `CreatedDate` ve `UpdatedDate` = `DateTime.UtcNow`
- ✅ Entity Modified: `UpdatedDate` = `DateTime.UtcNow`
- ✅ `CreatedByUserId` ve `UpdatedByUserId` otomatik doldurur (şu an "System")

**Not**: İleride HttpContext'ten veya JWT Claims'den user ID'sini alabilir.

---

#### 5. **Dependency Injection** (`DependencyInjection.cs`)

```csharp
// Program.cs içinde:
services.AddInfrastructure("./Data/FullMarkt.db");
```

Otomatik olarak:
- ✅ DbContext'i kaydeder
- ✅ SQLite bağlantısını yapılandırır
- ✅ AuditInterceptor'ı ekler
- ✅ Detailed Errors ve Sensitive Data Logging'i (Development'ta) aktif eder

Veritabanını migrate etmek için:
```csharp
// Program.cs içinde:
app.UseInfrastructure();
```

---

### 🗄️ Veritabanı Şeması

#### Kolonlar (SQLite Tipileriyle)

| Tablo | Kolon | Tip | Zorunlu |
|-------|-------|-----|---------|
| **Players** | Id | TEXT (GUID) | ✓ |
| | FullName | TEXT | ✓ |
| | Position | INTEGER (Enum) | ✓ |
| | DateOfBirth | TEXT (DateTime) | ✓ |
| | Height | INTEGER | ✓ |
| | Weight | REAL | ✓ |
| | PreferredFoot | TEXT | ✓ |
| | JerseyNumber | INTEGER | ✗ |
| | CurrentClubId | TEXT (GUID) | ✗ |
| | Nationality_CountryName | TEXT | ✓ |
| | Nationality_CountryCode | TEXT(2) | ✓ |
| | Nationality_FlagEmoji | TEXT | ✓ |
| | MarketValue_Amount | REAL | ✗ |
| | MarketValue_Currency | TEXT(3) | ✗ |
| | CreatedDate | TEXT (DateTime) | ✓ |
| | UpdatedDate | TEXT (DateTime) | ✓ |
| | CreatedByUserId | TEXT | ✓ |
| | UpdatedByUserId | TEXT | ✗ |
| | IsDeleted | BOOLEAN | ✓ |
| | DeletedDate | TEXT (DateTime) | ✗ |
| | DeletedByUserId | TEXT | ✗ |

(Diğer tablolar benzer yapıdadır...)

---

### ⚙️ Global Query Filter Örneği

```csharp
// Bu otomatik uygulanır:
var activePlayers = dbContext.Players.ToList();
// SQL WHERE'de: IsDeleted = 0 koşulu otomatik eklenir

// Silinmiş oyuncuları almak için:
var deletedPlayers = dbContext.Players.IgnoreQueryFilters().Where(p => p.IsDeleted).ToList();
```

---

### 🔄 Soft Delete Mekanizması

```csharp
var player = await dbContext.Players.FindAsync(playerId);
player.Delete("UserId123");
await dbContext.SaveChangesAsync();

// Geri Alma
player.Restore();
await dbContext.SaveChangesAsync();
```

---

### 📋 EF Core Versiyonu ve Dependencies

```xml
<!-- InterFullMarkt.Infrastructure.csproj -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
```

---

### 🚀 Program.cs Entegrasyonu

```csharp
var builder = WebApplication.CreateBuilder(args);

// Infrastructure'ı Register et
builder.Services.AddInfrastructure();

var app = builder.Build();

// Veritabanını Migrate et ve Seed et
app.UseInfrastructure();

app.Run();
```

---

### 🎯 Mimarı Prensipler

✅ **Dependency Rule**: Infrastructure, Domain'e bağlıdır (tersine değil)
✅ **Owned Types**: Value Objects doğru şekilde mapping yapılmış
✅ **Global Query Filters**: Soft Delete otomatize edilmiş
✅ **Audit Tracking**: Tüm değişiklikleri kaydeder
✅ **SQLite Uyumlu**: SQL Server'a geçişte tek satır değişecek (Connection String)
✅ **Primary Constructors**: .NET 10.0 modern syntax
✅ **Interceptors**: SaveChanges lifecycle'ı extend edilmiş

---

**Orhan Bey, InterFullMarkt'ın 'hafızası' artık sağlam temele oturmuştur!**
