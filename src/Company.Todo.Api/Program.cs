using Company.Todo.Api.Data;
using Company.Todo.Api.Repositories;
using Company.Todo.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Configuration & Services ---

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories & Services (DI)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddControllers();

// CORS (adjust for your frontends)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Company Todo API", Version = "v1" });

    // Optional: Swagger OAuth2 (JWT Bearer)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[]{} }
    });
});

// Optional SSO via external IdP (e.g., ADFS) using OpenID Connect / JWT Bearer
// If you don't need SSO yet, you can comment out the following block.
var authSection = builder.Configuration.GetSection("Authentication");
if (authSection.Exists() && !string.IsNullOrWhiteSpace(authSection["Authority"]))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = authSection["Authority"];   // e.g., https://adfs.yourdomain.com/adfs
            options.Audience = authSection["Audience"];     // e.g., api://company.todo
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(authSection["Audience"]);
        });
}
else
{
    // If no auth configured, we still add authentication so [Authorize] compiles, but it won't block requests.
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => { options.Events = new JwtBearerEvents(); });
}

var app = builder.Build();

// --- Middleware ---
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
