# InterFullMarkt - API Test Examples

## 🚀 Players Endpoint

### Base URL
```
POST /api/players/create
Content-Type: application/json
```

---

## ✅ Başarılı Senaryo 1: Tam Veri ile Oyuncu Oluşturma

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

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

### Response (200 OK)
```json
{
  "isSuccess": true,
  "playerId": "30000000-0000-0000-0000-000000000003",
  "errorMessage": null,
  "errorCode": null,
  "message": "'Arda Güler' başarılı bir şekilde sisteme eklendi."
}
```

---

## ✅ Başarılı Senaryo 2: Minimal Veri ile Oyuncu (Kulüpsüz)

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "Vinicius Jr",
  "position": 4,
  "nationalityCode": "BR",
  "dateOfBirth": "2000-07-12",
  "height": 180,
  "weight": 80
}
```

### Response (200 OK)
```json
{
  "isSuccess": true,
  "playerId": "30000000-0000-0000-0000-000000000004",
  "message": "'Vinicius Jr' başarılı bir şekilde sisteme eklendi."
}
```

---

## ❌ Hata Senaryo 1: Validasyon Hatası (Boş Bilgiler)

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "",
  "position": 0,
  "nationalityCode": "XX",
  "dateOfBirth": "2030-01-01",
  "height": 100,
  "weight": 10
}
```

### Response (400 Bad Request)
```json
{
  "message": "Validasyon hatası",
  "errors": {
    "FullName": [
      "Oyuncu adı boş olamaz",
      "Oyuncu adı en az 3 karakter olmalıdır"
    ],
    "Position": [
      "Geçersiz pozisyon değeri (1=GK, 2=CB, 3=CM, 4=ST)"
    ],
    "NationalityCode": [
      "Milliyeti kodu 2 karakter olmalıdır (ISO 3166-1)"
    ],
    "DateOfBirth": [
      "Doğum tarihi gelecekte olamaz"
    ],
    "Height": [
      "Boy 150 cm'den küçük olamaz"
    ],
    "Weight": [
      "Kilo 40 kg'dan az olamaz"
    ]
  }
}
```

---

## ❌ Hata Senaryo 2: Bilinmeyen Milliyeti

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "John Doe",
  "position": 2,
  "nationalityCode": "ZZ",
  "dateOfBirth": "2000-01-01",
  "height": 185,
  "weight": 85
}
```

### Response (400 Bad Request)
```json
{
  "isSuccess": false,
  "playerId": null,
  "errorMessage": "'ZZ' kodu için milliyeti bulunamadı.",
  "errorCode": "INVALID_NATIONALITY",
  "message": null
}
```

---

## ❌ Hata Senaryo 3: Kulüp Bulunamadı

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "Test Player",
  "position": 3,
  "nationalityCode": "TR",
  "dateOfBirth": "2000-01-01",
  "height": 180,
  "weight": 75,
  "currentClubId": "99999999-9999-9999-9999-999999999999"
}
```

### Response (400 Bad Request)
```json
{
  "isSuccess": false,
  "errorMessage": "ID'si 99999999-9999-9999-9999-999999999999 olan kulüp bulunamadı.",
  "errorCode": "CLUB_NOT_FOUND"
}
```

---

## ❌ Hata Senaryo 4: Kadro Dolu

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "Player 24",
  "position": 1,
  "nationalityCode": "TR",
  "dateOfBirth": "2000-01-01",
  "height": 195,
  "weight": 90,
  "currentClubId": "20000000-0000-0000-0000-000000000001"
}
```

(Real Madrid'de zaten 23 oyuncu varsa)

### Response (400 Bad Request)
```json
{
  "isSuccess": false,
  "errorMessage": "Real Madrid Club de Fútbol kadrosu dolu (Max: 23 oyuncu).",
  "errorCode": "SQUAD_FULL"
}
```

---

## ❌ Hata Senaryo 5: İngilizce Olmayan Karakterler (Regex Hatası)

### Request
```http
POST /api/players/create HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "fullName": "Müller123",
  "position": 3,
  "nationalityCode": "DE",
  "dateOfBirth": "2000-01-01",
  "height": 185,
  "weight": 80
}
```

### Response (400 Bad Request)
```json
{
  "message": "Validasyon hatası",
  "errors": {
    "FullName": [
      "Oyuncu adı yalnızca harfler, boşluklar, tire ve kesme işaretleri içerebilir"
    ]
  }
}
```

---

## ✨ Özel Karakterleri Destekleyen İsmler

✅ Aşağıdakiler **VALID**:
- "José María" (Aksanlı harfler)
- "Erling Braut Haaland" (Boşluk)
- "Jean-Claude Van Damme" (Tire)
- "O'Neill" (Kesme işareti)
- "François" (ç, è, é karakterleri)
- "Jöhannes Müller" (ö, ü karakterleri)

❌ Aşağıdakiler **INVALID**:
- "João123" (Sayılar)
- "Mário_Silva" (Underscore)
- "José@Perez" (Özel semboller)

---

## 📊 Position Enum Değerleri

| Değer | Pozisyon | Açıklama |
|-------|----------|----------|
| **1** | GK | Goalkeeper (Kaleci) |
| **2** | CB | Center Back (Stoper) |
| **3** | CM | Central Midfielder (Ortaçalı) |
| **4** | ST | Striker (Santrfor) |

---

## 🌍 Örnek Milliyeti Kodları

| Kod | Ülke | Flag |
|-----|------|------|
| **TR** | Turkey | 🇹🇷 |
| **BR** | Brazil | 🇧🇷 |
| **DE** | Germany | 🇩🇪 |
| **ES** | Spain | 🇪🇸 |
| **FR** | France | 🇫🇷 |
| **GB** | United Kingdom | 🇬🇧 |
| **AR** | Argentina | 🇦🇷 |
| **PT** | Portugal | 🇵🇹 |

---

## 💡 cURL Örneği

```bash
curl -X POST http://localhost:5000/api/players/create \
  -H "Content-Type: application/json" \
  -d '{
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
  }'
```

---

## 🧪 Postman Collection

**Pre-request Script** (Postman):
```javascript
// Random kulüp ID'sini seç
const clubIds = [
  "20000000-0000-0000-0000-000000000001",  // Real Madrid
  "20000000-0000-0000-0000-000000000002"   // Manchester City
];
pm.environment.set("clubId", clubIds[Math.floor(Math.random() * clubIds.length)]);

// Timestamp ile unique name
pm.environment.set("playerName", "Test Player " + new Date().getTime());
```

**Body** (Postman):
```json
{
  "fullName": "{{playerName}}",
  "position": 3,
  "nationalityCode": "TR",
  "dateOfBirth": "2000-01-01",
  "height": 180,
  "weight": 75,
  "currentClubId": "{{clubId}}"
}
```

---

## ✅ Test Checklist

- [ ] Başarılı oyuncu oluşturma
- [ ] Minimal veri ile oyuncu oluşturma (kulüpsüz)
- [ ] Boş alan validasyonu
- [ ] Bilinmeyen milliyeti hata mesajı
- [ ] Kulüp bulunamadı hatası
- [ ] Kadro dolu hatası
- [ ] İngilizce olmayan karakter validasyonu
- [ ] Negatif piyasa değeri reddedilmesi
- [ ] Gelecek tarih doğum tarihi reddedilmesi
- [ ] Forma numarası (1-99) validasyonu

---

**InterFullMarkt API'niz şimdi tam çalışır durumda!** 🚀
