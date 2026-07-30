using FluentValidation;
using MediatR;

namespace AgriTrace.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs all registered FluentValidation validators
/// before the request handler is invoked.
/// If any validator fails, an <see cref="ValidationException"/> is thrown,
/// which is mapped to HTTP 400 by GlobalExceptionHandler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
        _validators.Select(v => v.ValidateAsync(context, cancellationToken))); // ✅

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
             .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
