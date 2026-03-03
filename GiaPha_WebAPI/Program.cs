using Microsoft.OpenApi.Models;
using FluentValidation;
using GiaPha_Application.Features.HoName.Command.CreateHo;
using GiaPha_Application.Repository;
using GiaPha_Infrastructure.Db;
using GiaPha_Infrastructure.Repository;
using GiaPha_WebAPI.Filters;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Behaviors;
using GiaPha_Application.Service;
using GiaPha_Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApp.Application.Service;
using GiaPha_Application.Common;
using GiaPha_Application.IntegrationEvents;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

#region Controllers + Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.WriteIndented = true;

});
#endregion


#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GiaPha API",
        Version = "v1"
    });
});
#endregion

#region Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is missing!");
}
builder.Services.AddDbContext<DbGiaPha>(options =>
{
    // options.UseMySql(
    //     connectionString,
    //     new MySqlServerVersion(new Version(8, 0, 29))
    // );
    options.UseMySql(
    connectionString,
    ServerVersion.AutoDetect(connectionString)
);
});
#endregion
// config DI cho RabbitMQ
builder.Services.AddSingleton<IEmailSender, BrevoEmailSender>();


#region Repositories
builder.Services.AddScoped<IHoRepository, HoRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IThanhVienRepository, ThanhVienRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditReopository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IQuanHeChaMeRepository, QuanHeChaMeRepository>();
builder.Services.AddScoped<IHonNhanRepository, HonNhanRepository>();
builder.Services.AddScoped<IGiaPhaRepository, GiaPhaRepository>();
builder.Services.AddScoped<IYeuCauThamGiaHoRepository, YeuCauThamGiaHoRepository>();
builder.Services.AddScoped<ISukienRepository, SuKienRepository>();

// ✅ IUnitOfWork trỏ đến CÙNG instance DbGiaPha với repositories
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DbGiaPha>());
#endregion
builder.Services.AddScoped<GiaPhaTreeBuilder>();
#region MediatR + Pipeline + Validation
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateHoCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(CreateHoCommand).Assembly);
#endregion
//  JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, BrevoEmailService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// 📧 Email Reminder Service (Clean Architecture)
builder.Services.AddScoped<ISuKienReminderService, GiaPha_Infrastructure.Service.SuKienReminderService>();
builder.Services.AddHostedService<GiaPha_WebAPI.BackgroundServices.SuKienEmailReminderBackgroundService>();


//  JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
    };
});
#region CORS
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendApp", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => origin.Contains("vercel.app") 
                                       || origin.Contains("minhhiep2534.id.vn")
                                       || origin.Contains("localhost"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
#endregion

var rabbitUri = builder.Configuration["RabbitMQ:Uri"];

// Nếu có RabbitMQ URI → dùng queue async. Nếu không → Producer tự dùng Brevo fallback.
builder.Services.AddSingleton<IRabbitMqEmailProducer>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<RabbitMqEmailProducer>>();
    var emailSender = sp.GetRequiredService<IEmailSender>();
    return new RabbitMqEmailProducer(rabbitUri, emailSender, logger);
});

if (!string.IsNullOrEmpty(rabbitUri))
{
    builder.Services.AddHostedService<RabbitMqEmailConsumer>();
}


builder.Services.AddAuthorization();

var app = builder.Build();

#region Middleware Pipeline

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GiaPha API v1");
});

if (app.Environment.IsDevelopment())
{
    // Development-only middleware có thể thêm ở đây
}
#endregion


// Health check endpoint cho Render
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseCors("AllowFrontendApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
