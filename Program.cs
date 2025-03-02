using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer is missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience is missing.");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Database connection string is missing.");

// ✅ Configure Database (EF Core)
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(connectionString));

// ✅ Configure Authentication & JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidateAudience = true,
    ValidAudience = builder.Configuration["Jwt:Audience"],  // ✅ Ensure this matches appsettings.json
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
    });

// ✅ Add Authorization Middleware
builder.Services.AddAuthorization();

// ✅ Configure Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Management System API", Version = "v1" });

    // 🔹 Enable JWT Authentication in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {your-token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// ✅ Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// ✅ Enable Swagger UI (For API Testing)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();  // ✅ Enable Authentication Middleware
app.UseAuthorization();   // ✅ Enable Authorization Middleware
app.MapControllers();

app.Run();
