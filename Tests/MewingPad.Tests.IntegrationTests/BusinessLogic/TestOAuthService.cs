using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.OAuthService;
using Microsoft.Extensions.Configuration;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class OAuthServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly OAuthService _service;
    private readonly UserRepository _userRepository;
    private readonly PlaylistRepository _playlistRepository;
    private readonly UserFavouriteRepository _userFavouriteRepository;

    public OAuthServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ApiSettings:FavouritesDefaultName", "Favourites" },
        };
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _userRepository = new(_context);
        _playlistRepository = new(_context);
        _userFavouriteRepository = new(_context);

        _service = new OAuthService(
            config,
            _userRepository,
            _playlistRepository,
            _userFavouriteRepository
        );
    }

    [Fact]
    public async Task RegisterUser_NoUserWithEmail_Ok()
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
        var actual = await _service.RegisterUser(new(user));

        // Assert
        Assert.Equal(expectedId, actual.Id);
    }

    [Fact]
    public async Task RegisterUser_UserWithEmailExists_Error()
    {
        // Array
        await AddDefaultUserWithPlaylist();

        var user = new UserCoreModelBuilder()
            .WithId(DefaultUserId)
            .WithUsername("Username")
            .WithEmail("user@example.com")
            .WithPasswordHashed("passwd")
            .Build();

        // Act
        async Task Action() => await _service.RegisterUser(user);

        // Assert
        await Assert.ThrowsAsync<UserRegisteredException>(Action);
    }

    [Fact]
    public async Task SignInUser_UserIsRegistered_Ok()
    {
        // Array
        await AddDefaultUserWithPlaylist();

        // Act
        var actual = 
            await _service.SignInUser("user@example.com", "passwd");

        // Assert
        Assert.Equal(actual.Id, DefaultUserId);
    }

    [Fact]
    public async Task SignInUser_WrongCredentials_Error()
    {
        using var context = Fixture.CreateContext();

        // Array
        await AddDefaultUserWithPlaylist();

        // Act
        async Task Action() => await _service.SignInUser("user@example.com", "aaa");

        // Assert
        await Assert.ThrowsAsync<UserCredentialsException>(Action);
    }

    [Fact]
    public async Task SignInUser_UserNotRegistered_Error()
    {
        // Arrays
        await AddDefaultUserWithPlaylist();

        // Act
        async Task Action() => await _service.SignInUser("", "");

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }
}
