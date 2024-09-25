// using MewingPad.Database.Context;
// using MewingPad.Database.Models;
// using Microsoft.EntityFrameworkCore;
// using Moq;

// namespace MewingPad.Tests.DataAccess.UnitTests;

// public class DatabaseFixture
// {
//     private const string ConnectionString =
//         @"User ID=postgres;Password=postgres;Server=localhost;Database=MewingPadDBTest;Include Error Detail=true";

//     public Mock<MewingPadDbContext> CreateContext() => new();

//     // public DatabaseFixture()
//     // {
//     //     using var context = CreateContext();
//     //     context.Database.EnsureDeleted();
//     //     context.Database.EnsureCreated();

//     //     Cleanup();
//     // }

//     // public void Cleanup()
//     // {
//     //     using var context = CreateContext();

//     //     context.Users.RemoveRange(context.Users);
//     //     context.Playlists.RemoveRange(context.Playlists);
//     //     context.UsersFavourites.RemoveRange(context.UsersFavourites);
//     //     context.SaveChanges();
//     // }
// }

// [CollectionDefinition("Test Database", DisableParallelization = true)]
// public class DatabaseCollection : ICollectionFixture<DatabaseFixture> { }
