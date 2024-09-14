namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Database.Models;

public class ScoreDbModelBuilder
{
    private ScoreDbModel _scoreDbo = new();

    public ScoreDbModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _scoreDbo.AudiotrackId = audiotrackId;
        return this;
    }

    public ScoreDbModelBuilder WithAuthorId(Guid authorId)
    {
        _scoreDbo.AuthorId = authorId;
        return this;
    }

    public ScoreDbModelBuilder WithValue(int value)
    {
        _scoreDbo.Value = value;
        return this;
    }
    
    public ScoreDbModel Build()
    {
        return _scoreDbo;
    }
}

