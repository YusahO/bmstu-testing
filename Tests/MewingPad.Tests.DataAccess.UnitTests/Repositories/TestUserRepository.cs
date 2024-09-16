using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestUserRepository : BaseRepositoryTestClass
{
    private readonly UserRepository _repository;

    public TestUserRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddUser_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "User";

        var user = new UserCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithName(expectedName)
            .Build();

        // Act
        await _repository.AddUser(user);

        // Assert
        var actual = (from a in context.Users select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        Assert.Equal(expectedName, actual[0].Name);
    }

    [Fact]
    public async void AddUser_AddWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var user = new UserCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("User")
            .Build();
        await context.Users.AddAsync(
            new UserDbModelBuilder()
                .WithId(user.Id)
                .WithAuthorId(user.AuthorId)
                .WithName(user.Name)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddUser(user);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeleteUser_DeleteExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var user = new UserCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("User")
            .Build();
        await context.Users.AddAsync(
            new UserDbModelBuilder()
                .WithId(user.Id)
                .WithAuthorId(user.AuthorId)
                .WithName(user.Name)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteUser(user.Id);

        // Assert
        Assert.Empty((from a in context.Users select a).ToList());
    }

    [Fact]
    public async void DeleteUser_DeleteNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteUser(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateUser_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "New";

        var user = new UserCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithName("User")
            .Build();
        using (var context = Fixture.CreateContext())
        {
            await context.Users.AddAsync(
                new UserDbModelBuilder()
                    .WithId(user.Id)
                    .WithAuthorId(user.AuthorId)
                    .WithName(user.Name)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        user.Name = "New";

        // Act
        await _repository.UpdateUser(user);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Users select a).ToList();
            Assert.Single(actual);
            Assert.Equal(expectedId, actual[0].Id);
            Assert.Equal(expectedName, actual[0].Name);
            Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        }
    }

    [Fact]
    public async void UpdateUser_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var user = new UserCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("User")
            .Build();

        // Act
        async Task Action() => await _repository.UpdateUser(user);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetUserById_UserWithIdExists_ReturnsUser()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "User2";

        for (byte i = 1; i < 4; ++i)
        {
            await context.Users.AddAsync(
                new UserDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(expectedAuthorId)
                    .WithName($"User{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetUserById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedName, actual.Name);
        Assert.Equal(expectedAuthorId, actual.AuthorId);
    }

    [Fact]
    public async void GetUserById_NoUserWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(5);

        for (byte i = 1; i < 4; ++i)
        {
            await context.Users.AddAsync(
                new UserDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(DefaultUserId)
                    .WithName($"User{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetUserById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetUserById_NoUsers_Ok()
    {
        // Arrange

        // Act
        var actual = await _repository.GetUserById(new Guid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetAllUsers_UsersExist_ReturnsUsers()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        for (byte i = 1; i < 4; ++i)
        {
            await context.Users.AddAsync(
                new UserDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(DefaultUserId)
                    .WithName($"User{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllUsers();

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async void GetAllUsers_NoUsersExist_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllUsers();

        // Assert
        Assert.Empty(actual);
    }
}
