namespace MewingPad.Tests.Builders.Core;

using MewingPad.Common.Entities;

public class CommentaryCoreModelBuilder
{
    private Commentary _commentary = new();

    public CommentaryCoreModelBuilder WithId(Guid id)
    {
        _commentary.Id = id;
        return this;
    }

    public CommentaryCoreModelBuilder WithAuthorId(Guid authorId)
    {
        _commentary.AuthorId = authorId;
        return this;
    }

    public CommentaryCoreModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _commentary.AudiotrackId = audiotrackId;
        return this;
    }

    public CommentaryCoreModelBuilder WithText(string text)
    {
        _commentary.Text = text;
        return this;
    }
    public Commentary Build()
    {
        return _commentary;
    }
}

