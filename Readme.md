# API 
dotnet add package Swashbuckle.AspNetCore

dotnet add package FluentValidation
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0

dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0

dotnet add package Swashbuckle.AspNetCore
# Application Layer
dotnet add package BCrypt.Net-Next
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
dotnet add package MediatR --version 11.1.0
dotnet add package MediatR.Extensions.Microsoft.DependencyInjection 

# Infras 
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package SendGrid
dotnet add package System.IdentityModel.Tokens.Jwt

dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version .0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

# Add reference giữa các layer (chuẩn Clean Architecture)
dotnet add GiaPha_WebAPI reference GiaPha_Application

dotnet add GiaPha_Application reference GiaPha_Domain

dotnet add GiaPha_Infrastructure reference GiaPha_Application
dotnet add GiaPha_Infrastructure reference GiaPha_Domain
dotnet ef migrations add fixUser --startup-project ../GiaPha_WebAPI
dotnet add GiaPha_WebAPI reference GiaPha_Infrastructure
dotnet ef migrations add fixThanhVien --startup-project ../GiaPha_WebAPI
# migrations
dotnet ef migrations remove --startup-project ../GiaPha_WebAPI
dotnet ef migrations add InitialCreate --startup-project ../GiaPha_WebAPI
dotnet ef migrations add AddColumnsToHonNhan --startup-project ../GiaPha_WebAPI
dotnet ef database update
dotnet ef migrations add fixDatabaseTree --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseNew --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseEnums --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseThanhVien --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseAudit --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseHo --startup-project ../GiaPha_WebAPI
dotnet ef migrations add add Fixtaikhoannguoi --startup-project ../GiaPha_WebAPI
dotnet ef migrations add add FixDb --startup-project ../GiaPha_WebAPI
dotnet ef migrations add RemoveHoIdFromTaiKhoanNguoiDung --startup-project ../GiaPha_WebAPI
dotnet ef migrations add add FixThanhVienDatabase --startup-project ../GiaPha_WebAPI
dotnet ef migrations add DeleteChiHo --startup-project ../GiaPha_WebAPI
dotnet ef migrations add AddThanhVienIdToTaiKhoanNguoiDung --startup-project 
dotnet ef migrations add addSoDienThoai --startup-project ../GiaPha_WebAPI
ChangeThanhVienIdToHoId dotnet ef migrations add ChangeThanhVienIdToHoId  --startup-project ../GiaPha_WebAPI
dotnet ef migrations add RemoveColumnsDoiToThanhVien --startup-project ../GiaPha_WebAPI
dotnet ef migrations add fixDatabaseTvienwithHo --startup-project ../GiaPha_WebAPI
# chuyển hết về net 8.0 rồi sau đó chạy 
dotnet clean
dotnet restore
dotnet build
dotnet ef migrations add Notification --startup-project ../GiaPha_WebAPI

dotnet ef migrations add RemoveHoIdFromTaiKhoanNguoiDung1 --startup-project ../GiaPha_WebAPI
dotnet ef migrations add FixRelationships --startup-project ../GiaPha_WebAPI
Handler  →  Result<T>
Controller → return Ok(result)
Frontend → check isSuccess
dotnet ef migrations add fixDatabase --startup-project ../GiaPha_WebAPI
dotnet ef database update --startup-project ../GiaPha_WebAPI

dotnet ef migrations add addXinVaoHo --startup-project ../GiaPha_WebAPI
dotnet ef migrations add addFix --startup-project ../GiaPha_WebAPI

dotnet ef migrations add addGmailInThanhVien --startup-project ../GiaPha_WebAPI
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

 
