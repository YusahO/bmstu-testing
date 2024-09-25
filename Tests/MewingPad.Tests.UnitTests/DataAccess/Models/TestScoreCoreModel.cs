using MewingPad.Common.Exceptions;

namespace MewingPad.Tests.UnitTests.DataAccess.Models;

public class TestScoreCoreModel
{
    [Fact]
    public void TestSetValue_Ok()
    {
        // Arrange
        var score = new Score();

        // Act
        score.Value = 3;

        // Assert
        Assert.Equal(3, score.Value);
    }

    [Fact]
    public void TestSetValue_ValueLessThanAllowed()
    {
        // Arrange
        var score = new Score();

        // Act
        void Action() => score.Value = -1;

        // Assert
        Assert.Throws<ScoreInvalidValueException>(Action);
    }

    [Fact]
    public void TestSetValue_ValueMoreThanAllowed()
    {
        // Arrange
        var score = new Score();

        // Act
        void Action() => score.Value = 100;

        // Assert
        Assert.Throws<ScoreInvalidValueException>(Action);
    }
}
