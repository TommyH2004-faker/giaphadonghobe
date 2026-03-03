using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Auth.Queries.CheckExistUsername;

public record CheckExistUsernameQuery(string Username) : IRequest<Result<bool>>;