namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Entities;

public class ScoreCoreModelBuilder
{
    private Score _score = new();

    public ScoreCoreModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _score.AudiotrackId = audiotrackId;
        return this;
    }

    public ScoreCoreModelBuilder WithAuthorId(Guid authorId)
    {
        _score.AuthorId = authorId;
        return this;
    }

    public ScoreCoreModelBuilder WithValue(int value)
    {
        _score.SetValue(value);
        return this;
    }

    public Score Build()
    {
        return _score;
    }
}

