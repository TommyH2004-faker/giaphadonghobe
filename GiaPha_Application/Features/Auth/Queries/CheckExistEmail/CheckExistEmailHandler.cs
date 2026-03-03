using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.Auth.Queries.CheckExistEmail;

public class CheckExistEmailHandler : IRequestHandler<CheckExistEmailQuery, Result<bool>>
{
    private readonly IAuthRepository _authRepository;

    public CheckExistEmailHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result<bool>> Handle(CheckExistEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByEmailAsync(request.Email);
        var exists = user!= null;
        return Result<bool>.Success(exists);
    }
}