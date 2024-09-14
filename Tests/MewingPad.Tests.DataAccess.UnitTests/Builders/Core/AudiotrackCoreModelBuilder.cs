namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Entities;

public class AudiotrackCoreModelBuilder
{
    private Audiotrack _audiotrack = new();

    public AudiotrackCoreModelBuilder WithId(Guid id)
    {
        _audiotrack.Id = id;
        return this;
    }

    public AudiotrackCoreModelBuilder WithTitle(string title)
    {
        _audiotrack.Title = title;
        return this;
    }

    public AudiotrackCoreModelBuilder WithAuthorId(Guid authorId)
    {
        _audiotrack.AuthorId = authorId;
        return this;
    }

    public AudiotrackCoreModelBuilder WithFilepath(string filepath)
    {
        _audiotrack.Filepath = filepath;
        return this;
    }
    public Audiotrack Build()
    {
        return _audiotrack;
    }
}

