using GiaPha_Application.Common;
using GiaPha_Application.DTOs;
using MediatR;

namespace GiaPha_Application.Features.TaiKhoanNguoiDungs.Queries.GetTaiKhoanNguoiDungByid;
public record GetTaiKhoanNguoiDungQueryById(Guid Id) : IRequest<Result<TaiKhoanNguoiDungResponse>>;