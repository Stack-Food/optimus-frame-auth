using Amazon;
using Amazon.CognitoIdentityProvider;
using Microsoft.OpenApi.Models;
using OptimusFrame.Auth.Application.Interfaces;
using OptimusFrame.Auth.Application.UseCases;
using OptimusFrame.Auth.Infrastructure.Services;
using System.Diagnostics.CodeAnalysis;

namespace OptimusFrame.Auth.API
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Optimus Frame Auth API",
                    Version = "v1",
                    Description = "API for auth",
                    Contact = new OpenApiContact
                    {
                        Name = "Optimus Team",
                        Email = "team@stackfood.com"
                    }
                });

                c.AddServer(new OpenApiServer
                {
                    Url = "https://api.optimusframe.com.br/auth",
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddScoped<IAmazonCognitoIdentityProvider>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);

                return new AmazonCognitoIdentityProviderClient(region);
            });

            builder.Services.AddScoped<IIdentityProvider, CognitoIdentityProvider>();
            builder.Services.AddScoped<LoginUserUseCase>();
            builder.Services.AddScoped<RegisterUserUseCase>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}