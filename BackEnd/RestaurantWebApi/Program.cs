//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;
//using RestaurantWebApi.Helper; // تأكد من المسار الصحيح للفئة ReservationStatusUpdater

//var builder = WebApplication.CreateBuilder(args);

//// ===== Services =====
//builder.Services.AddControllers();

//// سجل الـ Background Service هنا
//builder.Services.AddHostedService<ReservationStatusUpdater>();

//// ---- CORS ----
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.WithOrigins(
//            "https://cork-features-added-import.trycloudflare.com",
//            "http://localhost:5500",
//            "http://127.0.0.1:5500"
//        )
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials();
//    });
//});

//// ---- JWT Authentication ----
//var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourStrongSecretKeyHere123!");
//builder.Services.AddAuthentication(options =>
//{
//    builder.Services.AddAuthorization(options =>
//    {
//        options.AddPolicy("AdminOrSameUser", policy =>
//            policy.RequireAssertion(context =>
//            {
//                var httpContext = context.Resource as HttpContext;
//                if (httpContext == null)
//                    return false;

//                // ✅ Admin bypass
//                if (context.User.IsInRole("Admin"))
//                    return true;

//                // 🔍 جرّب userId أو customerId
//                string? routeId =
//                    httpContext.Request.RouteValues.TryGetValue("userId", out var userId)
//                        ? userId?.ToString()
//                        : httpContext.Request.RouteValues.TryGetValue("customerId", out var customerId)
//                            ? customerId?.ToString()
//                            : null;

//                if (routeId == null)
//                    return false;

//                // 🔐 UserID من التوكن
//                var tokenUserId = context.User.FindFirst("UserID")?.Value;

//                return tokenUserId == routeId;
//            }));
//    });

//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});

//// ---- Swagger مع دعم JWT ----
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant API", Version = "v1" });

//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "أدخل JWT هنا بهذا الشكل: Bearer {token}"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            Array.Empty<string>()
//        }
//    });
//});

//var app = builder.Build();

//// ===== Middleware =====
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
//    c.RoutePrefix = "swagger";
//});

//app.UseCors();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using RestaurantWebApi.Helper; // تأكد من المسار الصحيح للفئة ReservationStatusUpdater

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddControllers();

// سجل الـ Background Service هنا
builder.Services.AddHostedService<ReservationStatusUpdater>();

// ---- CORS ----
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "https://cork-features-added-import.trycloudflare.com",
            "http://localhost:5500",
            "http://127.0.0.1:5500"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ---- Authorization Policy ----
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrSameUser", policy =>
        policy.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null)
                return false;

            // ✅ Admin bypass
            if (context.User.IsInRole("Admin"))
                return true;

            // 🔍 جرّب userId أو customerId
            string? routeId =
                httpContext.Request.RouteValues.TryGetValue("UserID", out var userId)
                    ? userId?.ToString()
                    : httpContext.Request.RouteValues.TryGetValue("CustomerID", out var customerId)
                        ? customerId?.ToString()
                        : null;

            if (routeId == null)
                return false;

            // 🔐 UserID من التوكن
            var tokenUserId = context.User.FindFirst("UserID")?.Value;

            return tokenUserId == routeId;
        }));
});

// ---- JWT Authentication ----
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourStrongSecretKeyHere123!");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ---- Swagger مع دعم JWT ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Restaurant API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "أدخل JWT هنا بهذا الشكل: Bearer {token}"
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ===== Middleware =====
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
