using MewingPad.Database.Models;

namespace MewingPad.Tests.Builders.Db;

public class PlaylistDbModelBuilder
{
    private PlaylistDbModel _playlistDbo = new() { Title = "PlaylistTitle" };

    public PlaylistDbModelBuilder WithId(Guid id)
    {
        _playlistDbo.Id = id;
        return this;
    }

    public PlaylistDbModelBuilder WithTitle(string title)
    {
        _playlistDbo.Title = title;
        return this;
    }

    public PlaylistDbModelBuilder WithUserId(Guid userId)
    {
        _playlistDbo.UserId = userId;
        return this;
    }

    public PlaylistDbModel Build()
    {
        return _playlistDbo;
    }
}
