namespace InterFullMarkt.Application.Common.Behaviors;

using FluentValidation;
using MediatR;

/// <summary>
/// MediatR Pipeline Behavior - Request'ten önce validasyon yapılmasını sağlar.
/// Tüm IRequest'leri otomatik olarak ilişkili validator'larla doğrular.
/// </summary>
/// <typeparam name="TRequest">MediatR Request tipi</typeparam>
/// <typeparam name="TResponse">MediatR Response tipi</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}
