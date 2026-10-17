
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

            // Register controllers
            builder.Services.AddControllers();

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

            // Configure HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("Angularpolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
