
using System.Linq;
using Cw7.Middlewares;
using Cw7.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Cw7
{
    public class Startup
    {
        private const string IndexHeader = "Index";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            TokenValidationParametersGenerator.AddConfiguration(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters =
                    TokenValidationParametersGenerator.GenerateTokenValidationParameters());

            services.AddTransient<ILogService, FileLogService>();
            services.AddScoped<IDbStudentService, MsSqlDbStudentService>();
            services.AddScoped<IAuthenticationService, MsSqlDbAuthenticationService>();
            services.AddSingleton<IEncryptionService, HashEncryptionService>();
            services.AddControllers();

            // Register the Swagger generator, defining Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Students API",
                    Version = "v1",
                    Description = "Students API prepared by Piotr Flis (s19841) for purposes of APBD.",
                    Contact = new OpenApiContact { Name = "Piotr Flis s19841" }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbStudentService dbService)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Students API"); });

            // Logging middleware is enabled after Swagger one to avoid logging requests for documentation.
            app.UseMiddleware<LogMiddleware>();

            app.Use(async (context, next) =>
            {
                if (dbService.GetAllStudents().ToList().Count == 0)
                {
                    await next();
                    return;
                }

                if (!context.Request.Headers.ContainsKey(IndexHeader))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("No headline with index number");
                    return;
                }

                var index = context.Request.Headers[IndexHeader].ToString();
                if (dbService.GetStudent(index) == null)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Index number not found in database");
                    return;
                }

                await next();
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}