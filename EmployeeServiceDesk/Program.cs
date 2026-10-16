
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

            //Email
            builder.Services.AddScoped<IEmailService, EmailService>();
            // OpenAPI
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
            }

            app.UseHttpsRedirection();

            app.UseCors("Angularpolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
