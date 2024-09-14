namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Entities;

public class PlaylistCoreModelBuilder
{
    private Playlist _playlist = new();

    public PlaylistCoreModelBuilder WithId(Guid id)
    {
        _playlist.Id = id;
        return this;
    }

    public PlaylistCoreModelBuilder WithTitle(string title)
    {
        _playlist.Title = title;
        return this;
    }

    public PlaylistCoreModelBuilder WithUserId(Guid userId)
    {
        _playlist.UserId = userId;
        return this;
    }
    public Playlist Build()
    {
        return _playlist;
    }
}

