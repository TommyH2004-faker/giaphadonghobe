using GiaPha_Application.Common;
using GiaPha_Application.Repository;
using MediatR;

namespace GiaPha_Application.Features.Auth.Queries.CheckExistUsername;

public class CheckExistUsernameHandler : IRequestHandler<CheckExistUsernameQuery, Result<bool>>
{
    private readonly IAuthRepository _authRepository;

    public CheckExistUsernameHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<Result<bool>> Handle(CheckExistUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _authRepository.GetUserByUsernameAsync(request.Username);
        if (user == null)
        {
            return Result<bool>.Success(false);
        }
        return Result<bool>.Success(true);
    }
}