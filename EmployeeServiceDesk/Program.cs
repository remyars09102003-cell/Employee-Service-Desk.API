using System.Text.Json.Serialization;
using Serilog;
using ServiceDesk.API.Middleware;
using ServiceDesk.Application;
using ServiceDesk.Infrastructure;
using EmployeeServiceDesk.Application.Mapper;
using EmployeeServiceDesk.Application.ServiceInterface;
using EmployeeServiceDesk.Application.Services;
using EmployeeServiceDesk.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

using EmployeeServiceDesk.Application.Interfaces;
using EmployeeServiceDesk.Application.Services;
using EmployeeServiceDesk.Domain.Interfaces;
using EmployeeServiceDesk.Infrastructure.Data;
using EmployeeServiceDesk.Infrastructure.Repositories;
using EmployeeServiceDesk.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeServiceDesk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, logger) => logger
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services
    .AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

            // Configure SQL Server database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                ));

            // Asset Management
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<IAssetRepository, AssetRepository>();

            // Dashboard service
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            // Add controllers
            builder.Services.AddControllers();

            // Database connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Defaultconnection")));

            // Register repository
            builder.Services.AddScoped<ISLARepository, SLARepository>();
            builder.Services.AddScoped<IKnowledgeArticleRepository, KnowledgeArticleRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

            // Register service
            builder.Services.AddScoped<ISLAService, SLAService>();
            builder.Services.AddScoped<IKnowledgeArticleService, KnowledgeArticleService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            // Feedback and Audit repositories
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IAuditRepository, AuditRepository>();

            // Feedback and Audit services
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IAuditService, AuditService>();

            // Swagger and OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

            // CORS for Angular
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Angularpolicy", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
            // Configure HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

app.UseHttpsRedirection();

// TODO(Member 1): app.UseAuthentication(); once JWT is wired up.
app.UseAuthorization();
            app.UseCors("Angularpolicy");

            app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { } // for integration tests
app.Run();