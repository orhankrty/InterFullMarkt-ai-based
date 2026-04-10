# InterFullMarkt Application Katmanı - Dokumentasyon

## 📋 Genel Bakış

Application katmanı, **InterFullMarkt'ın orkestra şefi** olarak çalışır. Domain katmanından gelen rich models'ı HTTP istekleri ile DTO'lara dönüştürür ve CQRS (Command Query Responsibility Segregation) pattern'ı ile merkezi bir işlem akışı sağlar.

---

## 🏗️ Katman Mimarisi

```
HTTP Client/Request
        ↓
API Controller
        ↓
MediatR (IRequest)
        ↓
ValidationBehavior (FluentValidation)
        ↓
CommandHandler (Business Logic)
        ↓
Database (Infrastructure/EF Core)
        ↓
Response (Result DTO)
```

---

## 📦 Oluşturulan Kompnentler

### 1. **DTOs (Data Transfer Objects)**

#### **PlayerDto** (`DTOs/PlayerDto.cs`)
- Oyuncu bilgilerini **Client'a geri döndürmek** için kullanılır
- **Salt veri taşır**, hiçbir iş mantığı olmaz
- Özellikleri: FullName, Position, Nationality, Age, MarketValue, etc.

#### **CreatePlayerDto** (`DTOs/CreatePlayerDto.cs`)
- **HTTP POST request'inde** Client'dan gelen verileri taşır
- Fluent Validation tarafından doğrulanır
- Fields: FullName, Position, NationalityCode, DateOfBirth, Height, Weight, etc.

---

### 2. **AutoMapper Profiling**

#### **PlayerProfile** (`Mappings/PlayerProfile.cs`)
```csharp
// Player Entity → PlayerDto
.ForMember(dest => dest.Position,
    opt => opt.MapFrom(src => src.Position.ToString()))
.ForMember(dest => dest.Nationality,
    opt => opt.MapFrom(src => $"{src.Nationality.FlagEmoji} {src.Nationality.CountryName}"))
.ForMember(dest => dest.Age,
    opt => opt.MapFrom(src => src.GetAge()))
.ForMember(dest => dest.MarketValue,
    opt => opt.MapFrom(src => src.MarketValue != null
        ? $"{src.MarketValue.Amount} {src.MarketValue.Currency}" : null))
```

**Dönüşümler**:
- PlayerPosition Enum → String ("GK", "CB", "CM", "ST")
- Nationality VO → Bayrak + Ülke Adı ("🇹🇷 Turkey")
- MarketValue Money VO → Formatted String ("50.000.000 EUR")

---

### 3. **Result Pattern** (`Common/Result.cs`)

**Railway Oriented Programming** pattern'ı kullanarak Error Handling'i yapılandırır.

#### **Generic Result<T>**
```csharp
public abstract record Result<T>
{
    public sealed record Success(T Data) : Result<T>;
    public sealed record Failure(string Message, string Code, Exception? Exception) : Result<T>;
}

// Kullanım:
var result = await someAsync();
return result.Match(
    onSuccess: data => Ok(data),
    onFailure: (msg, code, ex) => BadRequest(new { message = msg, code = code })
);
```

#### **Non-Generic Result**
```csharp
public abstract record Result
{
    public sealed record Success : Result;
    public sealed record Failure(string Message, string Code) : Result;
}
```

---

### 4. **MediatR CQRS Implementation**

#### **CreatePlayerCommand** (`Features/Players/Commands/CreatePlayer/CreatePlayerCommand.cs`)
```csharp
public sealed class CreatePlayerCommand : IRequest<CreatePlayerResult>
{
    public required CreatePlayerDto PlayerData { get; set; }
    public string CreatedByUserId { get; set; } = "System";
}
```

**Handler Flow**:
1. ✅ DTO'dan verileri çıkar
2. ✅ Nationality'i bulur (ISO 3166-1 validation)
3. ✅ Pozisyon Enum'ını dönüştürür
4. ✅ Player Entity'sini oluşturur
5. ✅ Kulübün kadro limitini kontrol eder
6. ✅ Veri tabanına kaydeder
7. ✅ Success/Failure Result'ı döndürür

#### **CreatePlayerCommandHandler** (`Features/Players/Commands/CreatePlayer/CreatePlayerCommandHandler.cs`)
- **4 DI Parameter**: DbContext, AutoMapper, Logger, IValidator
- **Step-by-Step Validation**: Milliyeti, pozisyon, kulüp mevcut mu?
- **Business Rules Enforcement**: Kadro 23 oyuncuyu geçemez
- **Exception Handling**: Try-catch, hata mesajı + kodu

#### **CreatePlayerCommandValidator** (`Features/Players/Commands/CreatePlayer/CreatePlayerCommandValidator.cs`)

FluentValidation Rules:

| Field | Rules |
|-------|-------|
| **FullName** | NotEmpty, MinLength 3, MaxLength 150, Regex ^[a-zA-Z\s\-'àâäéèêëïîôùûüœæçñ]+$ |
| **Position** | Must be valid enum (1-4) |
| **NationalityCode** | Length 2, ISO 3166-1 |
| **DateOfBirth** | LessThan(Now), GreaterThan(Now - 50 years) |
| **Height** | GreaterThanOrEqualTo 150, LessThanOrEqualTo 220 |
| **Weight** | GreaterThan 40, LessThan 150 |
| **PreferredFoot** | Must be "Left", "Right", or "Ambidextrous" |
| **InitialMarketValue** | GreaterThan 0 (if provided) |
| **Currency** | Length 3, uppercase (EUR, USD, GBP) |
| **JerseyNumber** | GreaterThanOrEqualTo 1, LessThanOrEqualTo 99 (if provided) |

---

### 5. **Validation Pipeline Behavior**

#### **ValidationBehavior** (`Common/Behaviors/ValidationBehavior.cs`)
```csharp
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    // MediatR Pipeline'a otomatik doğrulama ekler
    // Tüm Command'lar Handler'a gelmeden önce validate edilir
}
```

**Flow**:
```
Request → ValidationBehavior → Validators → Handler → Response
     ↓
   (if errors) → ValidationException
```

---

### 6. **Dependency Injection** (`ServiceRegistration.cs`)

```csharp
services.AddApplication();
```

Registers:
- ✅ **AutoMapper**: Profils + Type Safety
- ✅ **MediatR**: Assembly scanning, Handlers + Behaviors
- ✅ **FluentValidation**: All validators from assembly
- ✅ **ValidationBehavior**: Pipeline integration

---

### 7. **API Controller** (`Controllers/PlayersController.cs`)

```csharp
[HttpPost("create")]
public async Task<IActionResult> CreatePlayer(CreatePlayerDto dto)
{
    var command = new CreatePlayerCommand(dto, "Api.Client");
    var result = await _mediator.Send(command);
    
    return result.IsSuccess 
        ? Ok(result) 
        : BadRequest(result);
}
```

**Error Handling**:
- ✅ ValidationException → 400 BadRequest (Detailed errors)
- ✅ Business Logic Failure → 400 BadRequest (Message + Code)
- ✅ Unhandled Exception → 500 InternalServerError

---

## 🔄 Request Flow Örneği

### 1. HTTP POST /api/players/create
```json
{
  "fullName": "Arda Güler",
  "position": 3,
  "nationalityCode": "TR",
  "dateOfBirth": "2005-02-24",
  "height": 178,
  "weight": 72,
  "preferredFoot": "Right",
  "initialMarketValue": 40000000,
  "currency": "EUR",
  "jerseyNumber": 42,
  "currentClubId": "20000000-0000-0000-0000-000000000001"
}
```

### 2. PlayersController.CreatePlayer()
- DTO'yu null kontrol eder
- CreatePlayerCommand oluşturur
- IMediator.Send() çağırır

### 3. MediatR Pipeline
```
↓ ValidationBehavior
  ↓ CreatePlayerCommandValidator.Validate()
    - FullName: "Arda Güler" ✅
    - Position: 3 (CM) ✅
    - NationalityCode: "TR" (2 char) ✅
    - DateOfBirth: 2005-02-24 (geçmişte) ✅
    - Height: 178 (150-220) ✅
    - Weight: 72 (40-150) ✅
    - Currency: "EUR" (3 char) ✅
  ↓ (Hata yoksa Handler'a geç)

↓ CreatePlayerCommandHandler.Handle()
  1. Nationality.GetByCode("TR") → Türkiye ✅
  2. Position 3 → PlayerPosition.CM ✅
  3. Club mevcut mi? → Real Madrid ✅
  4. Squad count < 23 ✅
  5. Player entity oluştur ✅
  6. DbContext.SaveChangesAsync() ✅
  7. Return Success(playerId)

↓ Response
  {
    "isSuccess": true,
    "playerId": "30000000-0000-0000-0000-000000000003",
    "message": "'Arda Güler' başarılı bir şekilde sisteme eklendi."
  }
```

---

## ⚠️ Error Scenarios

### Senaryo 1: Validasyon Hatası
```json
Request:
{
  "fullName": "",  // Boş!
  "position": 5    // Geçersiz (1-4)
}

Response (400):
{
  "message": "Validasyon hatası",
  "errors": {
    "FullName": ["Oyuncu adı boş olamaz"],
    "Position": ["Geçersiz pozisyon değeri (1=GK, 2=CB, 3=CM, 4=ST)"]
  }
}
```

### Senaryo 2: Milliyeti Bulunamadı
```json
Request:
{
  "nationalityCode": "XX"  // Bilinmeyen
}

Response (400):
{
  "isSuccess": false,
  "errorMessage": "'XX' kodu için milliyeti bulunamadı.",
  "errorCode": "INVALID_NATIONALITY"
}
```

### Senaryo 3: Kadro Dolu
```json
Response (400):
{
  "isSuccess": false,
  "errorMessage": "Real Madrid kadrosu dolu (Max: 23 oyuncu).",
  "errorCode": "SQUAD_FULL"
}
```

---

## 🏆 Teknik Standartlar Kontrol Listesi

| Standart | Statü | Detaylar |
|----------|-------|----------|
| **.NET 10.0 Modern Syntax** | ✅ | Record types, Required members, async/await |
| **CQRS Pattern** | ✅ | Command Handler ayrılmış |
| **MediatR Pipeline** | ✅ | ValidationBehavior otomatik integration |
| **AutoMapper Profiles** | ✅ | Type-safe mapping, ForMember conventions |
| **FluentValidation Rules** | ✅ | 10+ kuralı ile profesyonel doğrulama |
| **Error Handling** | ✅ | Result<T>, ValidationException, Try-catch |
| **Dependency Injection** | ✅ | Assembly scanning, automatic registration |
| **Logging** | ✅ | ILogger<T> dependency injection |
| **DTO Separation** | ✅ | Domain logic yok, vanilla data carriers |
| **Two-Layer Validation** | ✅ | FluentValidation + Handler logic |

---

## 📊 Dosya Yapısı

```
InterFullMarkt.Application/
├── DTOs/
│   ├── PlayerDto.cs                          (Okuma)
│   └── CreatePlayerDto.cs                    (Yazma)
├── Mappings/
│   └── PlayerProfile.cs                      (AutoMapper)
├── Features/
│   └── Players/
│       └── Commands/
│           └── CreatePlayer/
│               ├── CreatePlayerCommand.cs    (MediatR Command + Result)
│               ├── CreatePlayerCommandHandler.cs  (Business Logic)
│               └── CreatePlayerCommandValidator.cs (Rules)
├── Common/
│   ├── Result.cs                             (Railway Pattern)
│   └── Behaviors/
│       └── ValidationBehavior.cs             (Pipeline)
└── ServiceRegistration.cs                    (DI Setup)
```

---

## 🚀 WebUI Integration

### Program.cs
```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
```

### PlayersController
```csharp
[HttpPost("create")]
public async Task<IActionResult> CreatePlayer(CreatePlayerDto dto)
{
    var result = await _mediator.Send(new CreatePlayerCommand(dto));
    return result.IsSuccess ? Ok(result) : BadRequest(result);
}
```

---

## 🔮 İlerideki Genişletmeler

| Feature | Status |
|---------|--------|
| **Queries (Read)** | 🔄 TODO |
| **Update Player Command** | 🔄 TODO |
| **Delete Player Command** | 🔄 TODO |
| **Transfer Command** | 🔄 TODO |
| **Logging Behavior** | 🔄 TODO |
| **Caching Behavior** | 🔄 TODO |
| **Unit of Work Pattern** | 🔄 TODO |

---

**InterFullMarkt'ın orkestramızı tam vites ile çalıştırıyor!** 🎼

Sonraki adım: **Queries (CQRS Read Side)** ve **Transfer Domain Logic'i** execute etmeliyiz.
