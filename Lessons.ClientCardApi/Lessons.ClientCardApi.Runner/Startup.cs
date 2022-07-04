using System;
using System.Text;
using Consul;
using Lessons.ClientCardApi.Abstraction.Data.Models;
using Lessons.ClientCardApi.Abstraction.Jwt;
using Lessons.ClientCardApi.CommonLogic.Facades.Registrations;
using Lessons.ClientCardApi.Data.AuthContext.AuthDbContext;
using Lessons.ClientCardApi.Data.AuthContext.Registrations;
using Lessons.ClientCardApi.Data.Context.Registrations;
using Lessons.ClientCardApi.NuGet.AutoMapper.Registrations;
using Lessons.ClientCardApi.NuGet.FluentValidator.Registrations;
using Lessons.ClientCardApi.NuGet.JwtBearer.Registrations;
using Lessons.ClientCardApi.UnitOfWork.Registrations;
using Lessons.ClientCardApi.Data.Repository.Registrations;
using Lessons.ClientCardApi.NuGet.Consul.Configurations;
using Lessons.ClientCardApi.NuGet.Consul.Registrations;
using Lessons.ClientCardApi.NuGet.JwtBearer.Jwt;
using Lessons.ClientCardApi.NuGet.Nest.Registrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Lessons.ClientCardApi.Runner
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("HealthRequestPolicy",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://example.com",
                            "http://www.contoso.com")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithMethods("GET");
                    });
            });
            
            services.AddMetrics();
            
            services.AddControllers();
            
            services.AddMvc(x =>
            {
                x.EnableEndpointRouting = false;
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireAuthenticatedUser()
                    .Build();
                x.Filters.Add(new AuthorizeFilter(policy));
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ClientCardApi", Version = "v1"});
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddIdentity<UserAuth, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5; //минимальная длинна пароля
                options.Password.RequireNonAlphanumeric = false; //требуется ли применять символы
                options.Password.RequireLowercase = false; //требуются ли символы в нижнем регистре
                options.Password.RequireUppercase = false; //требуются ли символя в верхнем регистре
                options.Password.RequireDigit = false; //требуются ли применять цифры в пароле
            })
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateAudience = false,
                            ValidateIssuer = false,
                        };
                    });
            
            services.AddTransient<IJwtGenerator, JwtGenerator>();
            services.RegisterElastic(Configuration);
            services.RegisterJwtGenerator();
            services.RegisterAuthPostgres(Configuration);
            services.RegisterRequestsValidators();
            services.RegisterPostgres(Configuration);
            services.RegisterMapper();
            services.RegisterRepositoryFacade();
            services.RegisterRepository();
            services.RegisterValidationFacade();
            services.RegisterUnitOfWork();
            services.RegisterConsulService();
            services.AddHealthChecks();
            services.Configure<ClientCardApiConfiguration>(Configuration.GetSection("ClientCardApi"));
            services.Configure<PostgresConfiguration>(Configuration.GetSection("Postgres"));
            services.Configure<ConsulConfiguration>(Configuration.GetSection("Consul"));
            services.Configure<ElasticConfiguration>(Configuration.GetSection("Elastic"));
            services.Configure<KibanaConfiguration>(Configuration.GetSection("Kibana"));
            services.Configure<GrafanaConfiguration>(Configuration.GetSection("Grafana"));
            services.Configure<PrometheusConfiguration>(Configuration.GetSection("Prometheus"));
            
            var consulAddress = Configuration.GetSection("Consul")["url"];
            services.AddSingleton<IConsulClient, ConsulClient>(provider =>
                new ConsulClient(conf =>
                    conf.Address = new Uri(consulAddress)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientCardApi_Development"));
            }

            if (env.IsStaging() || env.IsProduction())
            {
                app.UseExceptionHandler("/Error");
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientCardApi_Staging"));
            }

            app.UseRouting();
            
            app.UseCors();
            
            app.UseHttpsRedirection();

            app.UseAuthentication();
            
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}