using DoomedDatabases.Postgres;
using MewingPad.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MewingPad.Tests.IntegrationTests;

public class DatabaseFixture : IDisposable
{
    public ITestDatabase TestDatabase { get; }
    public MewingPadDbContext Context { get; }

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        TestDatabase = new TestDatabaseBuilder().WithConfiguration(configuration).Build();
        TestDatabase.Create();

        var builder = new DbContextOptionsBuilder<MewingPadDbContext>();
        builder.UseNpgsql(TestDatabase.ConnectionString);
        Context = new MewingPadDbContext(builder.Options);
        
        Context.Database.Migrate();
        Context.SaveChanges();
    }

    public void Dispose()
    {
        TestDatabase.Drop();
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture>
{
}