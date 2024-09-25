using MewingPad.Database.Models.Converters;


namespace MewingPad.Tests.DataAccess.UnitTests.Converters;

public class TestUserConverter
{
    [Fact]
    public void TestCoreToDbModel_Ok()
    {
        // Arrange
        var user = new UserCoreModelBuilder()
            .WithId(new Guid())
            .WithUsername("username")
            .WithEmail("email")
            .WithPasswordHashed("password")
            .WithRole(Common.Enums.UserRole.Admin)
            .Build();

        // Act
        var actualResult = UserConverter.CoreToDbModel(user);

        // Assert
        Assert.Equal(user.Id, actualResult.Id);
        Assert.Equal(user.Username, actualResult.Username);
        Assert.Equal(user.Email, actualResult.Email);
        Assert.Equal(user.PasswordHashed, actualResult.PasswordHashed);
        Assert.Equal(user.Role, actualResult.Role);
    }

    [Fact]
    public void TestDbToDbModel_Ok()
    {
        // Arrange
        var userDbo = new UserDbModelBuilder()
            .WithId(new Guid())
            .WithUsername("username")
            .WithEmail("email")
            .WithPasswordHashed("password")
            .WithRole(Common.Enums.UserRole.Admin)
            .Build();

        // Act
        var actualResult = UserConverter.DbToCoreModel(userDbo);

        // Assert
        Assert.Equal(userDbo.Id, actualResult.Id);
        Assert.Equal(userDbo.Username, actualResult.Username);
        Assert.Equal(userDbo.Email, actualResult.Email);
        Assert.Equal(userDbo.PasswordHashed, actualResult.PasswordHashed);
        Assert.Equal(userDbo.Role, actualResult.Role);
    }

    [Fact]
    public void TestCoreToDbModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = UserConverter.CoreToDbModel(null);

        // Assert
        Assert.Null(actualResult);
    }

    [Fact]
    public void TestDbToCoreModelNull_Ok()
    {
        // Arrange

        // Act
        var actualResult = UserConverter.DbToCoreModel(null);

        // Assert
        Assert.Null(actualResult);
    }
}