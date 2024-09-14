using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;

namespace MewingPad.Tests.DataAccess.UnitTests;

public class TestScoreCoreModel
{
	[Fact]
	public void TestSetValue_Ok()
	{
		// Arrange
		var score = new Score();

		// Act
		score.SetValue(3);

		// Assert
		Assert.Equal(3, score.Value);
	}

	[Fact]
	public void TestSetValue_ValueLessThanAllowed()
	{
		// Arrange
		var score = new Score();

		// Act
		Action action = () => score.SetValue(-1);

		// Assert
		Assert.Throws<ScoreInvalidValueException>(action);
	}

	[Fact]
	public void TestSetValue_ValueMoreThanAllowed()
	{
		// Arrange
		var score = new Score();

		// Act
		Action action = () => score.SetValue(100);

		// Assert
		Assert.Throws<ScoreInvalidValueException>(action);
	}
}