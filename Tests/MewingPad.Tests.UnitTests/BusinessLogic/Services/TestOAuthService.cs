using MewingPad.Common.Exceptions;
using MewingPad.Services.OAuthService;
using MewingPad.Tests.Factories.Core;
using Microsoft.Extensions.Configuration;

namespace MewingPad.Tests.UnitTests.BusinessLogic.Services;

public class OAuthServiceUnitTest : BaseServiceTestClass
{
    private readonly OAuthService _oauthService;
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IPlaylistRepository> _mockPlaylistRepository = new();
    private readonly Mock<IUserFavouriteRepository> _mockUserFavouriteRepository =
        new();

    public OAuthServiceUnitTest()
    {
        var _mockConfig = new Mock<IConfiguration>();
        _oauthService = new OAuthService(
            _mockConfig.Object,
            _mockUserRepository.Object,
            _mockPlaylistRepository.Object,
            _mockUserFavouriteRepository.Object
        );
    }

    [Fact]
    public async Task RegisterUser_NoUserWithEmail_Ok()
    {
        // Array
        List<User> users = [];
        List<Playlist> playlists = [];
        List<Tuple<Guid, Guid>> usersFavourites = [];

        var expectedUser = UserFabric.Create(
            MakeGuid(1),
            "Username",
            "Email",
            "passwd"
        );
        var expectedPlaylist = PlaylistFabric.Create(
            MakeGuid(1),
            "Title",
            expectedUser.Id
        );

        _mockUserRepository
            .Setup(s => s.GetUserByEmail(expectedUser.Email))
            .ReturnsAsync(default(User)!);
        _mockUserRepository
            .Setup(s => s.AddUser(It.IsAny<User>()))
            .Callback((User u) => users.Add(expectedUser));
        _mockPlaylistRepository
            .Setup(s => s.AddPlaylist(It.IsAny<Playlist>()))
            .Callback((Playlist p) => playlists.Add(expectedPlaylist));
        _mockUserFavouriteRepository
            .Setup(s =>
                s.AddUserFavouritePlaylist(It.IsAny<Guid>(), It.IsAny<Guid>())
            )
            .Callback(
                (Guid uid, Guid pid) => usersFavourites.Add(new(uid, pid))
            );

        // Act
        var actual = await _oauthService.RegisterUser(new(expectedUser));

        // Assert
        Assert.Single(users);
        Assert.Single(playlists);
        Assert.Single(usersFavourites);
        Assert.Equal(expectedUser.Id, usersFavourites[0].Item1);
    }

    [Fact]
    public async Task RegisterUser_UserWithEmailExists_Error()
    {
        // Array
        _mockUserRepository
            .Setup(s => s.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(UserFabric.CreateEmpty());

        // Act
        async Task Action() =>
            await _oauthService.RegisterUser(UserFabric.CreateEmpty());

        // Assert
        await Assert.ThrowsAsync<UserRegisteredException>(Action);
    }

    [Fact]
    public async Task SignInUser_UserIsRegistered_Ok()
    {
        // Array
        List<User> users =
        [
            UserFabric.Create(
                MakeGuid(1),
                "Username",
                "Email",
                "$2a$11$hNEJrsowk93rMboSuIkb5u3WpRn0M1flrw4oVnl/54xRkLE1Uxdaa"
            ),
        ];
        List<Playlist> playlists =
        [
            PlaylistFabric.Create(MakeGuid(1), "Title", users[0].Id),
        ];
        List<Tuple<Guid, Guid>> usersFavourites =
        [
            Tuple.Create(users[0].Id, playlists[0].Id),
        ];

        _mockUserRepository
            .Setup(s => s.GetUserByEmail(users[0].Email))
            .ReturnsAsync(users[0]);

        // Act
        var actual = new User(
            await _oauthService.SignInUser(users[0].Email, "passwd")
        );

        // Assert
        Assert.Equal(actual, users[0]);
    }

    [Fact]
    public async Task SignInUser_WrongCredentials_Error()
    {
        // Array
        List<User> users =
        [
            UserFabric.Create(
                MakeGuid(1),
                "Username",
                "Email",
                "$2a$11$hNEJrsowk93rMboSuIkb5u3WpRn0M1flrw4oVnl/54xRkLE1Uxdaa"
            ),
        ];
        List<Playlist> playlists =
        [
            PlaylistFabric.Create(MakeGuid(1), "Title", users[0].Id),
        ];
        List<Tuple<Guid, Guid>> usersFavourites =
        [
            Tuple.Create(users[0].Id, playlists[0].Id),
        ];

        _mockUserRepository
            .Setup(s => s.GetUserByEmail(users[0].Email))
            .ReturnsAsync(users[0]);

        // Act
        async Task Action() =>
            await _oauthService.SignInUser(users[0].Email, "aaa");

        // Assert
        await Assert.ThrowsAsync<UserCredentialsException>(Action);
    }

    [Fact]
    public async Task SignInUser_UserNotRegistered_Error()
    {
        // Array
        _mockUserRepository
            .Setup(s => s.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(default(User)!);

        // Act
        async Task Action() => await _oauthService.SignInUser("", "");

        // Assert
        await Assert.ThrowsAsync<UserNotFoundException>(Action);
    }
}
