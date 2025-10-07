using Api_Orbis_Project.Data;
using Api_Orbis_Project.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using api.Custome;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------
// Database configuration (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------------------------------
// -------------------------------------------
// JWT configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine("JWT token received: " + context.Token);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// -------------------------------------------
// Custom services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<Utils>();

// -------------------------------------------
// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",  // React / Next.js
                "http://localhost:8081"   // Expo web
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------------------------------
// Controllers and endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------------------------------
// Swagger configuration with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Orbis API",
        Version = "v1",
        Description = "API for Orbis Project"
    });

    // Add JWT auth to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT like: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            new List<string>()
        }
    });
});
// HttpClient for LocationController
builder.Services.AddHttpClient();
// HttpClient for HuggingFaceService
builder.Services.AddHttpClient<HuggingFaceService>();

builder.Services.AddHttpClient("RestCountries", c => {
    c.BaseAddress = new Uri("https://restcountries.com/v3.1/");
});
builder.Services.AddHttpClient("WorldBank", c =>
{
    c.BaseAddress = new Uri("https://api.worldbank.org/v2/");
});

// -------------------------------------------
// App build and configuration
var app = builder.Build();

Console.WriteLine("Starting Orbis API...");

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseCors("AllowReactApp");

app.UseAuthentication(); // JWT validation
app.UseAuthorization();

app.MapControllers();

app.Run();