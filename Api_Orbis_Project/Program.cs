using Api_Orbis_Project.Data;
using Api_Orbis_Project.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------
// Database configuration (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
                Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? ""))
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
builder.Services.AddScoped<api.Custome.Utils>();
builder.Services.AddHttpClient<IExpoPushService, ExpoPushService>();

// -------------------------------------------
// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:8081",
                "http://192.168.0.103:8081",
                "http://localhost:5089" 
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

// -------------------------------------------
// HttpClient services
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IHuggingFaceService, HuggingFaceService>();
builder.Services.AddHttpClient<IHuggingFaceService, HuggingFaceService>();

builder.Services.AddScoped<ITravelGuideService, TravelGuideService>();

builder.Services.AddScoped<IIAService, IAService>();
builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();

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

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var aiService = serviceProvider.GetService<IIAService>();
        var huggingService = serviceProvider.GetService<IHuggingFaceService>();
        var travelGuideService = serviceProvider.GetService<ITravelGuideService>();
        
        Console.WriteLine($"IIAService registrado: {aiService != null}");
        Console.WriteLine($"IHuggingFaceService registrado: {huggingService != null}");
        Console.WriteLine($"ITravelGuideService registrado: {travelGuideService != null}");
        
        try
{
    var controller = serviceProvider.GetRequiredService<Api_Orbis_Project.Controllers.AiController>();
    Console.WriteLine("✅ AiController se resolvió correctamente");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Error creando AiController:");
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error resolviendo servicios: {ex.Message}");
    }
}

// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();  

app.UseCors("AllowFrontend");  

app.UseAuthentication();  
app.UseAuthorization(); 

app.MapControllers();    

app.Run();