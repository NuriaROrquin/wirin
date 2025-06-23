using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Wirin.Application;
using Wirin.Domain.Providers;
using Wirin.Domain.Repositories;
using Wirin.Domain.Services;
using Wirin.Infrastructure;
using Wirin.Infrastructure.Context;
using Wirin.Infrastructure.Entities;
using Wirin.Infrastructure.Loaders;
using Wirin.Infrastructure.Providers;
using Wirin.Infrastructure.Repositories;
using Wirin.Infrastructure.Seeders;
using Wirin.Infrastructure.Selectors;
using Wirin.Infrastructure.Selectors.Interfaces;
using Wirin.Infrastructure.Senders;
using Wirin.Infrastructure.Services;
using Wirin.Infrastructure.Strategies.Local;

var builder = WebApplication.CreateBuilder(args);

// Configurar el puerto asignado por Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// Add services to the container.

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Services.AddDbContext<WirinDbContext>(
    options => options.UseSqlServer(connectionString));
/*, 
    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            )*/

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentity<UserEntity, IdentityRole<string>>()
    .AddEntityFrameworkStores<WirinDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
});

builder.Services.AddAuthorization();

var jwtKey = builder.Configuration["AppConfig:Jwt:Key"];
var jwtIssuer = builder.Configuration["AppConfig:Jwt:Issuer"];
var jwtAudience = builder.Configuration["AppConfig:Jwt:Audience"];



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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

builder.Services.AddTransient<IEmailSender<UserEntity>, NoOpEmailSender<UserEntity>>();

// Registramos los motores OCR individuales como implementaciones concretas

builder.Services.AddScoped<LocalOcrEngine>();

// Registramos el selector que utiliza los motores específicos (sin dependencia circular)
builder.Services.AddScoped<IOcrEngineSelector, OcrEngineSelector>();

// Registramos el servicio que depende del selector
builder.Services.AddScoped<IOcrProvider, OcrProvider>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderManagmentRepository, OrderManagmentRepository>();
builder.Services.AddScoped<IOrderParagraphRepository, OrderParagraphRepository>();
builder.Services.AddScoped<IOrderDeliveryRepository, OrderDeliveryRepository>();
builder.Services.AddScoped<IOrderSequenceRepository, OrderSequenceRepository>();
builder.Services.AddScoped<IParagraphAnnotationRepository, ParagraphAnnotationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOrderTrasabilityRepository, OrderTrasabilityRepository>();
builder.Services.AddScoped<IStudentDeliveryRepository, StudentDeliveryRepository>();
builder.Services.AddScoped<ICareerRepository, CareerRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderDeliveryService>();
builder.Services.AddScoped<OrderManagmentService>();
builder.Services.AddScoped<OrderParagraphService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<ParagraphAnnotationService>();
builder.Services.AddScoped<OrderTrasabilityService>();
builder.Services.AddScoped<StudentDeliveryService>();
builder.Services.AddScoped<CareerService>();
builder.Services.AddScoped<SubjectService>();

builder.Services.AddScoped<IOrderFeedbackRepository, OrderFeedbackRepository>();
builder.Services.AddScoped<OrderFeedbackService>();

builder.Services.AddTransient<ProcessWithLocalOcrUseCase>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wirin API", Version = "v1" });

    // 🔒 Configuración JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT con el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


//CORS
var frontURL = builder.Configuration["AppConfig:FrontEndUrl"] ?? "http://localhost:4200";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularFrontend", policy =>
    {
        policy.WithOrigins(frontURL) 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Solo si us�s cookies o auth por header
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AngularFrontend");

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.MapGroup("/identity").MapIdentityApi<User>();

app.MapControllers();

// Seed the database with roles and a super admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        services.GetRequiredService<WirinDbContext>().Database.Migrate(); // Ensure database is created and migrated

        await IdentitySeeder.SeedRolesAsync(services);
        await IdentitySeeder.SeedUsersAsync(services);
        await CareerAndSubjectSeeder.SeedCareersAndSubjectsAsync(services);
        await OrderSeeder.SeedOrdersAsync(services);

        logger.LogInformation("Database seeded successfully with roles, super admin user, careers and subjects.");
    }
    catch(Exception ex)
    {
        logger.LogError(ex,"Pincho en el program");

    }

}

app.Run();

public partial class Program
{
    // This class is for the integration tests to use the same entry point as the main application.
    // It is intentionally left empty.
}

