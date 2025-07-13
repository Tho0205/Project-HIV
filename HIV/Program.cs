using DemoSWP391.Services;
using HIV.Hubs;
using HIV.Interfaces;
using HIV.Interfaces.ARVinterfaces;
using HIV.Models;
using HIV.Repository;
using HIV.Services;
using MDP.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace HIV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddUserSecrets<Program>()
              .AddEnvironmentVariables();


            builder.Services.AddSignalR();

            //Add Db context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity services

            //App Automapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // cấu hình jwt
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretkey = jwtSettings["SecretKey"] ?? throw new Exception("JWT secret key not found");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
             //cấu hình của gg được lưu trên cookie
            .AddCookie()
            .AddGoogle(options =>
            {
                var clientID = builder.Configuration["Authentication:Google:ClientId"];

                if (clientID == null)
                {
                    throw new ArgumentNullException(nameof(clientID));
                }

                var clientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

                if (clientSecret == null)
                {
                    throw new ArgumentNullException(nameof(clientSecret));
                }

                options.ClientId = clientID;
                options.ClientSecret = clientSecret;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                options.Scope.Add("profile");
                options.Scope.Add("email");

            })
            //cấu hình của jwt
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "HIV API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                     // cái này với mục đích là để cấp quyền để lấy dữ liệu từ swagger
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token vào đây: Bearer {your token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //Adding the repository and service layer
            builder.Services.AddScoped(typeof(ICommonOperation<>), typeof(CommonOperation<>));

            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAdminManagementAccount, AdminAccountService>();

            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IEducationalResourcesService, EducationalResourcesService>();
            builder.Services.AddScoped<IHIVExaminationService, HIVExaminationService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IArvService, ArvService>();
            builder.Services.AddScoped<IARVProtocolService, ARVProtocolService>();
            builder.Services.AddScoped<ICustomizedArvProtocolService, CustomizedArvProtocolService>();
            builder.Services.AddScoped<IDoctorInfoService, DoctorInfoService>();
            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            builder.Services.AddScoped<IDoctorMangamentPatient, DoctorPatientService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IJwtService, JWTService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000", "https://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            var app = builder.Build();

            //Upload Images
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads"
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
                RequestPath = "/uploads"
            });

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseCors("AllowReact");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapHub<ChatHub>("/chathub");

            // cấu hình login bằng gg
            app.MapGet("/api/account/login/google", async (
                [FromQuery] string returnUrl,
                [FromServices] IAccountService accountService,
                [FromServices] LinkGenerator linkGenerator,
                HttpContext context) =>
            {
                var redirectUrl = linkGenerator.GetUriByName(context, "GoogleLoginCallback",
                    values: new { returnUrl = returnUrl });

                var properties = new AuthenticationProperties
                {
                    RedirectUri = redirectUrl
                };

                return Results.Challenge(properties, new[] { "Google" });
            });

            app.MapGet("/api/account/login/callback", async (
                [FromQuery] string returnUrl,
                [FromServices] IAccountService accountService,
                HttpContext context) =>
            {
                var result = await context.AuthenticateAsync("Google");

                if (!result.Succeeded)
                {
                    return Results.Redirect($"{returnUrl}?error=authentication_failed");
                }

                try
                {
                    var token = await accountService.LoginWithGoogleAsync(result.Principal);

                    // Set the token in a secure cookie
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    context.Response.Cookies.Append("AuthToken", token, cookieOptions);

                    return Results.Redirect($"{returnUrl}?token={token}");
                }
                catch (Exception ex)
                {
                    return Results.Redirect($"{returnUrl}?error=login_failed&message={Uri.EscapeDataString(ex.Message)}");
                }

            }).WithName("GoogleLoginCallback");

            app.MapPost("/api/account/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // logout Google/Cookie
                context.Response.Cookies.Delete("AuthToken"); // optional: xóa JWT nếu lưu bằng cookie
                return Results.Ok(new { message = "Logged out successfully" });
            });


            app.Run();
        }
    }
}