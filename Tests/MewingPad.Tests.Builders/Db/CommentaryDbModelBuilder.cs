using MewingPad.Database.Models;

namespace MewingPad.Tests.Builders.Db;

public class CommentaryDbModelBuilder
{
    private CommentaryDbModel _commentaryDbo = new();

    public CommentaryDbModelBuilder WithId(Guid id)
    {
        _commentaryDbo.Id = id;
        return this;
    }

    public CommentaryDbModelBuilder WithAuthorId(Guid authorId)
    {
        _commentaryDbo.AuthorId = authorId;
        return this;
    }

    public CommentaryDbModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _commentaryDbo.AudiotrackId = audiotrackId;
        return this;
    }

    public CommentaryDbModelBuilder WithText(string text)
    {
        _commentaryDbo.Text = text;
        return this;
    }
    
    public CommentaryDbModel Build()
    {
        return _commentaryDbo;
    }
}

