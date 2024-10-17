using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;

namespace MewingPad.Tests.IntegrationTests.DataAccess;

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
        var expectedId = DefaultUserId;
        const string expectedUsername = "Username";
        const string expectedEmail = "user@example.com";
        const string expectedPassword = "passwd";
        const Common.Enums.UserRole expectedRole = Common.Enums.UserRole.User;

        var playlistDbo = new PlaylistDbModelBuilder()
            .WithId(DefaultPlaylistId)
            .WithTitle("Title")
            .WithUserId(expectedId)
            .Build();
        await context.Playlists.AddAsync(playlistDbo);
        await context.UsersFavourites.AddAsync(new(expectedId, playlistDbo.Id));

        var user = new UserCoreModelBuilder()
            .WithId(expectedId)
            .WithUsername(expectedUsername)
            .WithEmail(expectedEmail)
            .WithPasswordHashed(expectedPassword)
            .WithRole(expectedRole)
            .Build();

        // Act
        await _repository.AddUser(user);

        // Assert
        var actual = (from a in context.Users select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedUsername, actual[0].Username);
        Assert.Equal(expectedEmail, actual[0].Email);
        Assert.Equal(expectedPassword, actual[0].PasswordHashed);
        Assert.Equal(expectedRole, actual[0].Role);
    }

    [Fact]
    public async void AddUser_AddWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = DefaultUserId;
        const string expectedUsername = "Username";
        const string expectedEmail = "user@example.com";
        const string expectedPassword = "passwd";
        const Common.Enums.UserRole expectedRole = Common.Enums.UserRole.User;

        var user = new UserCoreModelBuilder()
            .WithId(expectedId)
            .WithUsername(expectedUsername)
            .WithEmail(expectedEmail)
            .WithPasswordHashed(expectedPassword)
            .WithRole(expectedRole)
            .Build();

        // Act
        async Task Action() => await _repository.AddUser(user);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateUser_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = DefaultUserId;
        const string expectedUsername = "New";
        const string expectedEmail = "user@example.com";
        const string expectedPassword = "passwd";
        const Common.Enums.UserRole expectedRole = Common.Enums.UserRole.User;

        var user = new UserCoreModelBuilder()
            .WithId(expectedId)
            .WithUsername(expectedUsername)
            .WithEmail(expectedEmail)
            .WithPasswordHashed(expectedPassword)
            .WithRole(expectedRole)
            .Build();

        // Act
        await _repository.UpdateUser(user);

        // Assert
        using var context = Fixture.CreateContext();
        var actual = (from a in context.Users select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedUsername, actual[0].Username);
        Assert.Equal(expectedEmail, actual[0].Email);
        Assert.Equal(expectedPassword, actual[0].PasswordHashed);
        Assert.Equal(expectedRole, actual[0].Role);
    }

    [Fact]
    public async void UpdateUser_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var user = new UserCoreModelBuilder()
            .WithId(DefaultUserId)
            .WithUsername("username")
            .WithEmail("email")
            .WithPasswordHashed("password")
            .WithRole(Common.Enums.UserRole.Guest)
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

        var expectedId = DefaultUserId;
        const string expectedUsername = "username";
        const string expectedEmail = "user@example.com";
        const string expectedPassword = "passwd";
        const Common.Enums.UserRole expectedRole = Common.Enums.UserRole.User;

        // Act
        var actual = await _repository.GetUserById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedUsername, actual.Username);
        Assert.Equal(expectedEmail, actual.Email);
        Assert.Equal(expectedPassword, actual.PasswordHashed);
        Assert.Equal(expectedRole, actual.Role);
    }

    [Fact]
    public async void GetUserById_NoUserWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = new();

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
    public async void GetUserByEmail_UserWithEmailExists_ReturnsUser()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = DefaultUserId;
        const string expectedUsername = "username";
        const string expectedEmail = "user@example.com";
        const string expectedPassword = "passwd";
        const Common.Enums.UserRole expectedRole = Common.Enums.UserRole.User;

        // Act
        var actual = await _repository.GetUserByEmail(expectedEmail);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedUsername, actual.Username);
        Assert.Equal(expectedEmail, actual.Email);
        Assert.Equal(expectedPassword, actual.PasswordHashed);
        Assert.Equal(expectedRole, actual.Role);
    }

    [Fact]
    public async void GetUserByEmail_NoUserWithEmail_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        const string expectedEmail = "";

        // Act
        var actual = await _repository.GetUserByEmail(expectedEmail);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetUserByEmail_NoUsers_Ok()
    {
        // Arrange

        // Act
        var actual = await _repository.GetUserByEmail("");

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetAdmins_AdminsExist_ReturnsUsers()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddUserWithPlaylistWithIds(
            MakeGuid(2),
            MakeGuid(2),
            Common.Enums.UserRole.Admin
        );

        // Act
        var actual = await _repository.GetAdmins();

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAdmins_NoAdminsExist_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = await _repository.GetAdmins();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAllUsers_UsersExist_ReturnsUsers()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddUserWithPlaylistWithIds(MakeGuid(2), MakeGuid(2));

        // Act
        var actual = await _repository.GetAllUsers();

        // Assert
        Assert.Equal(2, actual.Count);
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
