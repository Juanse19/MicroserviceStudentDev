using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Servicios.api.Seguridad.Core.Application;
using Servicios.api.Seguridad.Core.Entities;
using Servicios.api.Seguridad.Core.JwtLogic;
using Servicios.api.Seguridad.Core.Persistence;
using System;
using System.Text;

namespace Servicios.api.Seguridad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SeguridadContexto>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("conexionDB"));
            });

            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            identityBuilder.AddEntityFrameworkStores<SeguridadContexto>();
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.AddTransient<ISystemClock, SystemClock>();

            services.AddMediatR(typeof(Register.UsuarioRegisterCommand).Assembly);

            services.AddAutoMapper(typeof(Register.UsuarioRegisterHandler));

            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();

            services.AddControllers().AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Register>());


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("cfIwYG1QhqV4Od8dFyoBgxBxD8tuj4AZ"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            //services.AddCors(opt =>
            //{
            //    opt.AddPolicy("CorsRule", rule =>
            //    {
            //        rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
            //    });
            //});

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .WithOrigins(Configuration.GetValue<string>("frontend_url"))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders(new string[] { "Total-Registros" });
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Servicios.api.Seguridad", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Servicios.api.Seguridad v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
