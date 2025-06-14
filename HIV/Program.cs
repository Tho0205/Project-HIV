
using HIV.Models;
using Microsoft.EntityFrameworkCore;
using HIV.Interfaces;
using HIV.Repository;

using System;
using DemoSWP391.Services;
using HIV.Interfaces.ARVinterfaces;
using Microsoft.Extensions.FileProviders;
namespace HIV
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add Dd context
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //App Automapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            //Adding the repository and service layer
            builder.Services.AddScoped(typeof(ICommonOperation<>), typeof(CommonOperation<>));

            builder.Services.AddScoped<IAppointmentService, AppointmentService>();

            builder.Services.AddScoped<IBlogService, BlogService>();

            builder.Services.AddScoped<IEducationalResourcesService, EducationalResourcesService>();

            builder.Services.AddScoped<IExaminationService, ExaminationService>();

            builder.Services.AddScoped<IScheduleService, ScheduleService>();

            builder.Services.AddScoped<IArvService, ArvService>();

            builder.Services.AddScoped<IARVProtocolService, ARVProtocolService>();

            builder.Services.AddScoped<IARVProtocolDetailService, ARVProtocolDetailService>();

            builder.Services.AddScoped<ICustomizedArvProtocolService, CustomizedArvProtocolService>();

            builder.Services.AddScoped<ICustomizedArvProtocolDetailService, CustomizedArvProtocolDetailService>();

            builder.Services.AddScoped<IDoctorInfoService, DoctorInfoService>();

            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

            builder.Services.AddScoped<ICommentService, CommentService>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            //Upload Images
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
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

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
