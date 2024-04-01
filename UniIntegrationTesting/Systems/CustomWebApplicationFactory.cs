using Application.Services;
using Core.InterfacesOfRepo;
using Core.InterfacesOfServices;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;



namespace IntegrationTesting.Systems
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                services.Remove(dbContextDescriptor);

                services.AddAutoMapper(typeof(Program)); // Add AutoMapper with assembly containing AutoMapper profiles
                services.AddScoped<IStudentService, StudentService>();
                services.AddScoped<IStudentRepo, StudentRepo>();

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    var connectionString = "Server=A1SOFTECHPRO\\SQLEXPRESS;Database=University;Trusted_Connection=True;TrustServerCertificate=True";
                    options.UseSqlServer(connectionString);
                });

                //services.AddAuthorization(config =>
                //{
                //    config.DefaultPolicy = new AuthorizationPolicyBuilder(config.DefaultPolicy)
                //        .AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme)
                //        .RequireAuthenticatedUser()
                //        .Build();
                //});
                services.AddControllers();
            });

            builder.UseEnvironment("Development");
        }
    }


}

