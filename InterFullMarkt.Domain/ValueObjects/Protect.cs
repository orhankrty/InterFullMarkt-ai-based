namespace InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Guard clause'ları içeren statik yardımcı sınıfı.
/// Value Object'ler ve Entity'ler tarafından parametre validasyonu için kullanılır.
/// </summary>
public static class Protect
{
    public static class Against
    {
        /// <summary>
        /// Null olmayan string değeri kontrol eder
        /// </summary>
        public static string NullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{paramName}' boş veya null olamaz", paramName);
            return value;
        }

        /// <summary>
        /// Negatif veya sıfır değeri kontrol eder
        /// </summary>
        public static decimal NegativeOrZero(decimal value, string paramName)
        {
            if (value <= 0)
                throw new ArgumentException($"'{paramName}' pozitif bir sayı olmalıdır", paramName);
            return value;
        }

        /// <summary>
        /// Negatif değeri kontrol eder
        /// </summary>
        public static decimal Negative(decimal value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"'{paramName}' negatif olamaz", paramName);
            return value;
        }

        /// <summary>
        /// Negatif değeri kontrol eder (int)
        /// </summary>
        public static int Negative(int value, string paramName)
        {
            if (value < 0)
                throw new ArgumentException($"'{paramName}' negatif olamaz", paramName);
            return value;
        }

        /// <summary>
        /// Null değeri kontrol eder
        /// </summary>
        public static T Null<T>(T? value, string paramName) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(paramName, $"'{paramName}' null olamaz");
            return value;
        }

        /// <summary>
        /// Gelecek tarih olup olmadığını kontrol eder
        /// </summary>
        public static DateTime InTheFuture(DateTime dateTime, string paramName)
        {
            if (dateTime > DateTime.UtcNow)
                throw new ArgumentException($"'{paramName}' gelecek bir tarih olamaz", paramName);
            return dateTime;
        }

        /// <summary>
        /// Geçmiş tarih olup olmadığını kontrol eder
        /// </summary>
        public static DateTime InThePast(DateTime dateTime, string paramName)
        {
            if (dateTime < DateTime.UtcNow)
                throw new ArgumentException($"'{paramName}' geçmiş bir tarih olamaz", paramName);
            return dateTime;
        }
    }
}
