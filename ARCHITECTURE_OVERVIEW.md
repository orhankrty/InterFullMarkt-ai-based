# InterFullMarkt - Mimari Özet (Architecture Overview)

## 🏛️ Katmanlı Mimari (Layered Architecture)

```
┌─────────────────────────────────────────────────────────┐
│                      HTTP Client                         │
│                  (Web Browser / Mobile)                  │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                    WebUI Layer                           │
│  (ASP.NET Core Controllers, Views, API Endpoints)       │
└────────────────────────┬────────────────────────────────┘
                         │
     ┌───────────────────┼───────────────────┐
     │                   │                   │
     ▼                   ▼                   ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   Players    │  │    Clubs     │  │   Leagues    │
│ Controller   │  │  Controller  │  │  Controller  │
└──────┬───────┘  └──────┬───────┘  └──────┬───────┘
       │                 │                 │
       └─────────────────┼─────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                Application Layer                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │          MediatR (CQRS Pattern)                  │  │
│  │  ┌────────────────────────────────────────────┐  │  │
│  │  │  Request → ValidationBehavior              │  │  │
│  │  │         → CommandHandler                   │  │  │
│  │  │         → Response                         │  │  │
│  │  └────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │       AutoMapper (DTO ↔ Entity Mapping)         │  │
│  │  Entity → PlayerDto    (Read)                    │  │
│  │  CreatePlayerDto → Entity (CreateCommand)       │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │    FluentValidation (Business Rules)             │  │
│  │  - Field-level validation                        │  │
│  │  - Business logic validation                     │  │
│  │  - Custom rule validators                        │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                 Domain Layer                            │
│  ┌──────────────────────────────────────────────────┐  │
│  │        Aggregate Roots (Rich Models)             │  │
│  │  - Player (GetAge, UpdateMarketValue, Delete)    │  │
│  │  - Club (AddPlayer, RemovePlayer, Deduct...)    │  │
│  │  - League (UpdateCoefficient)                    │  │
│  │  - Transfer (ValidateTransfer, Complete)        │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │        Value Objects (Immutable)                 │  │
│  │  - Money (Amount + Currency)                     │  │
│  │  - Nationality (CountryName + CountryCode)      │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │         Enum & Interfaces                        │  │
│  │  - PlayerPosition (GK, CB, CM, ST)              │  │
│  │  - IAuditEntity (CreatedBy, UpdatedBy)         │  │
│  │  - ISoftDelete (IsDeleted, DeletedDate)        │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              Infrastructure Layer                       │
│  ┌──────────────────────────────────────────────────┐  │
│  │      EF Core DbContext (SQLite)                  │  │
│  │  - InterFullMarktDbContext                       │  │
│  │  - Global Query Filters (Soft Delete)            │  │
│  │  - Audit Interceptor (Auto CreatedDate/Updated) │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │     Entity Type Configurations (Fluent API)      │  │
│  │  - PlayerConfiguration                           │  │
│  │  - ClubConfiguration                             │  │
│  │  - LeagueConfiguration                           │  │
│  │  - TransferConfiguration                         │  │
│  │  (Owned Types, Foreign Keys, Indexes)           │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │          Seed Data (Initial Data)                │  │
│  │  - La Liga, Premier League                       │  │
│  │  - Real Madrid, Manchester City                  │  │
│  │  - Arda Güler, Erling Haaland                   │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  SQLite Database                        │
│            (./Data/FullMarkt.db)                        │
│                                                        │
│  Tables: Players, Clubs, Leagues, Transfers           │
│  - Soft Delete filtering otomatik                      │
│  - Audit columns otomatik                             │
└─────────────────────────────────────────────────────────┘
```

---

## 📊 Data Flow (Request → Response)

```
1. CLIENT SENDS REQUEST
   POST /api/players/create
   {
     "fullName": "Arda Güler",
     "position": 3,
     "nationalityCode": "TR",
     ...
   }
          ↓
2. WEBU LAYER
   PlayersController.CreatePlayer()
   - DTO null kontrol
   - CreatePlayerCommand oluştur
   - IMediator.Send()
          ↓
3. APPLICATION LAYER - PIPELINE
   ┌─ ValidationBehavior
   │    CreatePlayerCommandValidator
   │    - FullName kontrol ✅
   │    - Position kontrol ✅
   │    - NationalityCode kontrol ✅
   │    - Tarih kontrol ✅
   │    - Fiziksel ölçüler ✅
   │    (ALL PASS → proceed)
   │
   └─ CreatePlayerCommandHandler
       - Nationality.GetByCode("TR") → Nationality ✅
       - Position 3 → PlayerPosition.CM ✅
       - Club mevcut? → Real Madrid ✅
       - Squad count < 23 ✅
       - MarketValue Money oluştur
       - Player Entity oluştur
          ↓
4. DOMAIN LAYER
   - Player(fullName, position, nationality, ...)
   - Guard Clauses kontrol → Valid ✅
   - Entity state: Added
          ↓
5. INFRASTRUCTURE LAYER
   - DbContext.Players.AddAsync()
   - SaveChangesAsync()
     - AuditInterceptor → CreatedDate, UpdatedDate, CreatedByUserId
     - EF Core Mapping: Entity → SQL INSERT
     - SQLite execute
          ↓
6. DATABASE
   INSERT INTO Players (Id, FullName, Position, ...)
   VALUES (guid, 'Arda Güler', 3, ...)
   ✅ SUCCESS
          ↓
7. RESPONSE BACK
   CreatePlayerCommandHandler:
   - return CreatePlayerResult.SuccessResult(player.Id)
          ↓
   PlayersController:
   - result.IsSuccess = true
   - return Ok(result)
          ↓
   CLIENT:
   HTTP/1.1 200 OK
   {
     "isSuccess": true,
     "playerId": "30000000-...",
     "message": "'Arda Güler' başarılı bir şekilde sisteme eklendi."
   }
```

---

## 🔐 Dependency Rule (Clean Architecture)

```
VALID DEPENDENCIES:
WebUI       → Application, Infrastructure ✅
Application → Domain (only!) ✅
Infrastructure → Domain ✅
Domain      → (Nothing) ✅

INVALID DEPENDENCIES:
Domain → Infrastructure ❌ (FORBIDDEN)
Domain → Application ❌ (FORBIDDEN)
```

---

## 🎯 Design Patterns Kullanılan

| Pattern | Lokasyon | Amaç |
|---------|----------|------|
| **CQRS** | Application | Command/Query ayrışması (şu an Commands var) |
| **MediatR Pipeline** | Application | Validation otomatizasyonu |
| **AutoMapper** | Application | DTO ↔ Entity mapping |
| **Result Pattern** | Application | Error handling (Success/Failure) |
| **Value Objects** | Domain | Money, Nationality immutability |
| **Aggregate Root** | Domain | Player, Club, League, Transfer encapsulation |
| **Repository** | Infrastructure | DbSet'e wrapper (future TODO) |
| **Data Transfer Object** | Application | API contracts |
| **Dependency Injection** | WebUI | IoC Container |
| **Soft Delete** | Infrastructure | Global Query Filter |
| **Audit Trail** | Infrastructure | Interceptor otomatizasyonu |
| **Fluent API** | Infrastructure | Entity mapping configuration |

---

## 🔄 Circular Dependency Önleme

**Potensiyel Sıkıntı**: Player → Club → League → Transfer ← Player

**Çözüm**:
- Foreign Keys ile ilişkilendirme
- Navigation properties virtual ve lazy-loaded
- Explicit join queries için Include()
- EF Core'ın relationship tracking'i

**Örnek**:
```csharp
var playerWithClub = await _dbContext.Players
    .Include(p => p.CurrentClub)
    .Include(p => p.CurrentClub.League)
    .FirstOrDefaultAsync(p => p.Id == playerId);
```

---

## 📈 Scalability

### Şu an desteklenen:
✅ Player CRUD başlangıcı
✅ Validation + Error handling
✅ Audit trail
✅ Soft delete

### Genelleştirilebilir:
🔄 Diğer Entity'ler için Commands (Club, League, Transfer)
🔄 Query side (CQRS Read)
🔄 Batch operations
🔄 Event Sourcing
🔄 Caching layer
🔄 Unit of Work pattern

---

## 🚀 Production Readiness

| Aspect | Status |
|--------|--------|
| **Error Handling** | ✅ 80% |
| **Logging** | 🔄 Basic (ILogger) |
| **Validation** | ✅ 100% (FluentValidation) |
| **Security** | 🔄 TODO (Auth, Authorization) |
| **Performance** | 🔄 TODO (Caching, Query optimization) |
| **Testing** | 🔄 TODO (Unit, Integration tests) |
| **Documentation** | ✅ Comprehensive |
| **Database** | ✅ SQLite (Easy to SQL Server migration) |

---

## 🛠️ Teknoloji Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| **Frontend** | ASP.NET Core MVC/Razor | 10.0 |
| **API** | ASP.NET Core Web API | 10.0 |
| **CQRS** | MediatR | 12.4.1 |
| **Mapping** | AutoMapper | 13.1.0 |
| **Validation** | FluentValidation | 11.9.2 |
| **ORM** | Entity Framework Core | 10.0.0 |
| **Database** | SQLite | 3.x |
| **Language** | C# 14 | .NET 10.0 |

---

## 📝 Klasör Hiyerarşisi

```
InterFullMarkt-ai-based/
├── InterFullMarkt.Domain/
│   ├── Common/
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   ├── Player.cs
│   │   ├── Club.cs
│   │   ├── League.cs
│   │   └── Transfer.cs
│   ├── ValueObjects/
│   │   ├── Money.cs
│   │   ├── Nationality.cs
│   │   └── Protect.cs
│   ├── Enums/
│   │   └── PlayerPosition.cs
│   ├── IAuditEntity.cs
│   └── ISoftDelete.cs
│
├── InterFullMarkt.Application/
│   ├── DTOs/
│   │   ├── PlayerDto.cs
│   │   └── CreatePlayerDto.cs
│   ├── Mappings/
│   │   └── PlayerProfile.cs
│   ├── Features/
│   │   └── Players/
│   │       └── Commands/
│   │           └── CreatePlayer/
│   │               ├── CreatePlayerCommand.cs
│   │               ├── CreatePlayerCommandHandler.cs
│   │               └── CreatePlayerCommandValidator.cs
│   ├── Common/
│   │   ├── Result.cs
│   │   └── Behaviors/
│   │       └── ValidationBehavior.cs
│   └── ServiceRegistration.cs
│
├── InterFullMarkt.Infrastructure/
│   ├── Data/
│   │   ├── InterFullMarktDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── PlayerConfiguration.cs
│   │   │   ├── ClubConfiguration.cs
│   │   │   ├── LeagueConfiguration.cs
│   │   │   └── TransferConfiguration.cs
│   │   ├── Interceptors/
│   │   │   └── AuditInterceptor.cs
│   │   └── Seeds/
│   │       └── SeedData.cs
│   └── DependencyInjection.cs
│
├── InterFullMarkt.WebUI/
│   ├── Controllers/
│   │   └── PlayersController.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── Views/
│       └── (existing MVC views)
│
└── InterFullMarkt.sln
```

---

## 🎓 Öğrenme Kaynakları (References)

- **Clean Architecture**: Robert C. Martin (Uncle Bob)
- **CQRS Pattern**: Greg Young
- **Domain-Driven Design**: Eric Evans
- **MediatR**: Jimmy Bogard
- **FluentValidation**: Jeremy Skinner
- **AutoMapper**: Jimmy Bogard

---

## ✨ Next Steps

1. **Transfer Command** - Oyuncuyu bir kulüpten diğerine transfer et
2. **Query Handlers** - Oyuncu listesi, kulüp detayları getir
3. **Unit Tests** - xUnit + Moq ile test coverage
4. **Integration Tests** - API endpoint testleri
5. **Authentication** - JWT/OAuth entegrasyonu
6. **Authorization** - Role-based access control
7. **API Documentation** - Swagger/OpenAPI
8. **Frontend UI** - Razor Pages veya Blazor

---

**InterFullMarkt, mimarisinin temelinde pırıl pırıl! 🌟**
