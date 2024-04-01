global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using Microsoft.OpenApi.Models;
using AutoMapper;
using Infrastructure.Repositories;
using Infrastructure;
using Application.Services;
using Core.InterfacesOfServices;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Core.InterfacesOfRepo;
using Core.Models;



public class Program
{

    private static void Main(string[] args)
        
    {

        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        IConfiguration configuration = builder.Configuration;



        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddControllers();

        services.AddDbContext<ApplicationDbContext>(options =>

         options.UseSqlServer(connectionString)
         );


        services.Configure<Helpers.JWT>(configuration.GetSection("JWT"));
        //services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddIdentity<ApplicationUser, IdentityRole>()

        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
        services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser, IdentityRole, ApplicationDbContext>>();
        services.AddScoped<UserManager<ApplicationUser>, UserManager<ApplicationUser>>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                ClockSkew = TimeSpan.Zero
            };
        });


        //services.AddTransient<ILoggerMiddlewareRepo, LoggerMiddlewareRepo>();
        //services.AddTransient<ILoggerMiddlewareService, LoggerMiddlewareService>();


        services.AddTransient<IAuthService, AuthService>();

        services.AddTransient<IStudentRepo, StudentRepo>();
        services.AddTransient<IStudentService, StudentService>();


        // For Auto Mapping
        //////////////////////////////////////////////////////////
        var mapperCongigurtion = new MapperConfiguration((cfg) =>
        {
            //cfg.CreateMap<Student, UnversityManagement.Models.DTOs.StudentDto>().ReverseMap();
            cfg.AddProfile(new MappingProfile());
      

        });


        services.AddSingleton(mapperCongigurtion.CreateMapper());

        builder.Services.AddControllers();

        services.AddEndpointsApiExplorer();

        //builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            // Configure Swagger to include the JWT bearer token in the header
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
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
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
        }

       
        app.UseHttpsRedirection();

        //app.UseMiddleware<Middleware>();

        


        app.UseAuthentication();

       

        app.UseAuthorization();

       

        app.MapControllers();

        //app.UseMiddleware<RequestResponseLoggingMiddleware>();

        app.UseMiddleware<Middleware>();

        app.Run();




    }





}




