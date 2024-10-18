using MewingPad.Common.Enums;
using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestUserRepository : BaseRepositoryTestClass
{
    private readonly UserRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestUserRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    private static User CreateUserCoreModel(
        Guid id,
        string username,
        string email,
        string password,
        UserRole role = UserRole.User
    )
    {
        return new UserCoreModelBuilder()
            .WithId(id)
            .WithUsername(username)
            .WithEmail(email)
            .WithPasswordHashed(password)
            .WithRole(role)
            .Build();
    }

    private static UserDbModel CreateUserDbo(
        Guid id,
        string username,
        string email,
        string password,
        UserRole role = UserRole.User
    )
    {
        return new UserDbModelBuilder()
            .WithId(id)
            .WithUsername(username)
            .WithEmail(email)
            .WithPasswordHashed(password)
            .WithRole(role)
            .Build();
    }

    private static UserDbModel CreateUserDboFromCore(User user)
    {
        return CreateUserDbo(
            user.Id,
            user.Username,
            user.Email,
            user.PasswordHashed,
            user.Role
        );
    }

    [Fact]
    public async void AddUser_AddUnique_Ok()
    {
        // Arrange
        List<UserDbModel> actual = [];
        var user = CreateUserCoreModel(
            MakeGuid(1),
            "username",
            "email",
            "password"
        );
        var userDbo = CreateUserDboFromCore(user);

        _mockFactory
            .MockUsersDbSet.Setup(s =>
                s.AddAsync(It.IsAny<UserDbModel>(), default)
            )
            .Callback<UserDbModel, CancellationToken>(
                (u, token) => actual.Add(u)
            );

        // Act
        await _repository.AddUser(user);

        // Assert
        Assert.Single(actual);
        Assert.Equal(user.Id, actual[0].Id);
        Assert.Equal(user.Username, actual[0].Username);
        Assert.Equal(user.Email, actual[0].Email);
        Assert.Equal(user.PasswordHashed, actual[0].PasswordHashed);
        Assert.Equal(user.Role, actual[0].Role);
    }

    [Fact]
    public async void AddUser_AddWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockUsersDbSet.Setup(s =>
                s.AddAsync(It.IsAny<UserDbModel>(), default)
            )
            .Callback<UserDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddUser(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateUser_UpdateExisting_Ok()
    {
        // Arrange
        var expectedId = MakeGuid(1);
        const string expectedUsername = "username";
        const string expectedEmail = "email";
        const string expectedPassword = "password";
        const UserRole expectedRole = UserRole.Admin;

        var user = CreateUserCoreModel(
            expectedId,
            expectedUsername,
            "old",
            expectedPassword,
            expectedRole
        );
        var userDbo = CreateUserDboFromCore(user);
        List<UserDbModel> userDbos = [userDbo];

        _mockFactory.MockContext.Setup(m => m.Users).ReturnsDbSet(userDbos);

        user.Email = expectedEmail;

        // Act
        await _repository.UpdateUser(user);

        // Assert
        Assert.Single(userDbos);
        Assert.Equal(expectedId, userDbos[0].Id);
        Assert.Equal(expectedUsername, userDbos[0].Username);
        Assert.Equal(expectedEmail, userDbos[0].Email);
        Assert.Equal(expectedPassword, userDbos[0].PasswordHashed);
        Assert.Equal(expectedRole, userDbos[0].Role);
    }

    [Fact]
    public async void UpdateUser_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockUsersDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(UserDbModel)!);
        _mockFactory
            .MockUsersDbSet.Setup(s => s.Update(It.IsAny<UserDbModel>()))
            .Callback((UserDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateUser(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetUserById_GetTestData()
    {
        yield return new object[] { new UserDbModel(), new User() };
        yield return new object[]
        {
            CreateUserDbo(MakeGuid(1), "username", "email", "password"),
            CreateUserCoreModel(MakeGuid(1), "username", "email", "password"),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserById_GetTestData))]
    public async Task GetUserById_ReturnsFound(
        UserDbModel returnedUserDbo,
        User expectedUser
    )
    {
        // Arrange
        _mockFactory
            .MockUsersDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnedUserDbo);

        // Act
        var actual = await _repository.GetUserById(expectedUser.Id);

        // Assert
        Assert.Equal(expectedUser, actual);
    }

    public static IEnumerable<object[]> GetUserByEmail_GetTestData()
    {
        yield return new object[] { new List<UserDbModel>([]), default(User)! };
        yield return new object[]
        {
            new List<UserDbModel>(
                [CreateUserDbo(MakeGuid(1), "username", "email", "password")]
            ),
            CreateUserCoreModel(MakeGuid(1), "username", "email", "password"),
        };
    }

    [Theory]
    [MemberData(nameof(GetUserByEmail_GetTestData))]
    public async Task GetUserByEmail_ReturnsFound(
        List<UserDbModel> returnedUserDbos,
        User expectedUser
    )
    {
        // Arrange
        var expectedEmail = "email";
        _mockFactory
            .MockContext.Setup(x => x.Users)
            .ReturnsDbSet(returnedUserDbos);

        // Act
        var actual = await _repository.GetUserByEmail(expectedEmail);

        // Assert
        Assert.Equal(expectedUser, actual);
    }

    public static IEnumerable<object[]> GetAdmins_GetTestData()
    {
        yield return new object[]
        {
            new List<UserDbModel>([]),
            new List<User>([]),
        };
        yield return new object[]
        {
            new List<UserDbModel>(
                [
                    CreateUserDbo(
                        MakeGuid(1),
                        "username",
                        "email",
                        "password",
                        UserRole.Guest
                    ),
                ]
            ),
            new List<User>([]),
        };
        yield return new object[]
        {
            new List<UserDbModel>(
                [
                    CreateUserDbo(
                        MakeGuid(1),
                        "username",
                        "email",
                        "password",
                        UserRole.Admin
                    ),
                ]
            ),
            new List<User>(
                [
                    CreateUserCoreModel(
                        MakeGuid(1),
                        "username",
                        "email",
                        "password",
                        UserRole.Admin
                    ),
                ]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAdmins_GetTestData))]
    public async void GetAdmins_ReturnsFound(
        List<UserDbModel> returnedUserDbos,
        List<User> expectedUsers
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Users)
            .ReturnsDbSet(returnedUserDbos);

        // Act
        var actual = await _repository.GetAdmins();

        // Assert
        Assert.Equal(expectedUsers, actual);
    }

    public static IEnumerable<object[]> GetAllUsers_GetTestData()
    {
        yield return new object[]
        {
            new List<UserDbModel>([]),
            new List<User>([]),
        };
        yield return new object[]
        {
            new List<UserDbModel>(
                [CreateUserDbo(MakeGuid(1), "username", "email", "password")]
            ),
            new List<User>(
                [
                    CreateUserCoreModel(
                        MakeGuid(1),
                        "username",
                        "email",
                        "password"
                    ),
                ]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllUsers_GetTestData))]
    public async void GetAllUsers_ReturnsFound(
        List<UserDbModel> returnedUserDbos,
        List<User> expectedUsers
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Users)
            .ReturnsDbSet(returnedUserDbos);

        // Act
        var actual = await _repository.GetAllUsers();

        // Assert
        Assert.Equal(expectedUsers, actual);
    }
}
