using System.Text;
using MewingPad.Database.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MewingPad.Tests.E2ETests;

internal class Settings
{
    public Settings() { }

    public string SymmetricFuncTestKey = "";
}

public class PgWebApplicationFactory<T> : WebApplicationFactory<T>
    where T : class
{
    private const string ConnectionString =
        @"Host=172.19.0.2;Port=5432;Username=postgres;Password=postgres;Database=testdb;Include Error Detail=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var settings = new Settings
        {
            SymmetricFuncTestKey =
                "my-32-character-ultra-secure-and-ultra-long-secret",
        };

        builder
            .UseEnvironment("Testing")
            .ConfigureServices(services =>
            {
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        settings.SymmetricFuncTestKey
                                    )
                                ),
                                ValidateIssuerSigningKey = true,
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidIssuer = "http://localhost:9898",
                                ValidAudience = "http://localhost:3000",
                            };
                    }
                );
            })
            .ConfigureTestServices(services =>
            {
                var options = new DbContextOptionsBuilder<MewingPadDbContext>()
                    .UseNpgsql(ConnectionString)
                    .EnableSensitiveDataLogging()
                    .Options;

                services.AddScoped<MewingPadDbContext>(
                    provider => new MewingPadTestContext(options)
                );

                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var scopedService = scope.ServiceProvider;
                var db = scopedService.GetRequiredService<MewingPadDbContext>();
                db.Database.EnsureCreated();
            });
    }
}
