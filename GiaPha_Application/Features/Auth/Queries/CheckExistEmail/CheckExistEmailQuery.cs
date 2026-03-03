using GiaPha_Application.Common;
using MediatR;

namespace GiaPha_Application.Features.Auth.Queries.CheckExistEmail;

public record CheckExistEmailQuery(string Email) : IRequest<Result<bool>>;