namespace InterFullMarkt.Application.Common;

/// <summary>
/// Operasyonların sonucunu temsil eden generic Result sınıfı.
/// Success ve Failure durumlarını işler, Railway Oriented Programming pattern'ı kullanır.
/// </summary>
/// <typeparam name="T">Başarıda döndürülecek veri tipi</typeparam>
public abstract record Result<T>
{
    /// <summary>
    /// Başarılı operasyon sonucu
    /// </summary>
    public sealed record Success(T Data) : Result<T>;

    /// <summary>
    /// Başarısız operasyon sonucu
    /// </summary>
    public sealed record Failure(string Message, string Code = "ERROR", Exception? Exception = null) : Result<T>;

    /// <summary>
    /// Result'ı Match/Switch pattern'ı ile işlemek için helper metodu
    /// </summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, string, Exception?, TResult> onFailure) =>
        this switch
        {
            Success success => onSuccess(success.Data),
            Failure failure => onFailure(failure.Message, failure.Code, failure.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Result'ıAsync Match pattern'ı ile işlemek için helper metodu
    /// </summary>
    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<string, string, Exception?, Task<TResult>> onFailure) =>
        this switch
        {
            Success success => await onSuccess(success.Data),
            Failure failure => await onFailure(failure.Message, failure.Code, failure.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Result'ın başarılı olup olmadığını kontrol eder
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Result'ın başarısız olup olmadığını kontrol eder
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Data'yı al ya da exception fırlat
    /// </summary>
    public T GetDataOrThrow() =>
        this switch
        {
            Success success => success.Data,
            Failure failure => throw new InvalidOperationException(failure.Message, failure.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}

/// <summary>
/// Non-generic Result sınıfı (void operasyonları için)
/// </summary>
public abstract record Result
{
    /// <summary>
    /// Başarılı operasyon
    /// </summary>
    public sealed record Success : Result;

    /// <summary>
    /// Başarısız operasyon
    /// </summary>
    public sealed record Failure(string Message, string Code = "ERROR", Exception? Exception = null) : Result;

    /// <summary>
    /// Result'ı Match pattern'ı ile işlemek için helper metodu
    /// </summary>
    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<string, string, Exception?, TResult> onFailure) =>
        this switch
        {
            Success => onSuccess(),
            Failure failure => onFailure(failure.Message, failure.Code, failure.Exception),
            _ => throw new InvalidOperationException("Unknown result type")
        };

    /// <summary>
    /// Result'ın başarılı olup olmadığını kontrol eder
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Result'ın başarısız olup olmadığını kontrol eder
    /// </summary>
    public bool IsFailure => this is Failure;
}
