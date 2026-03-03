using FluentValidation;
using MediatR;

namespace TodoApp.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        // Tất cả validators cho TRequest
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
            // Nếu không có validator nào, skip validation
            if (!_validators.Any())
            {
                return await next();
            }

            // Tạo validation context
            var context = new ValidationContext<TRequest>(request);

            // Chạy tất cả validators song song
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Lấy tất cả lỗi validation
            var failures = validationResults
                .Where(r => !r.IsValid)
                .SelectMany(r => r.Errors)
                .ToList();

            // Nếu có lỗi, throw ValidationException
            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
            // Nếu validation pass, tiếp tục đến Handler
            return await next();
        }
    }
}
