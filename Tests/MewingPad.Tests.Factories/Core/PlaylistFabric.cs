using MewingPad.Common.Entities;

namespace MewingPad.Tests.Factories.Core;

public class PlaylistFabric
{
    public static Playlist Create(Guid id, string title, Guid userId)
    {
        return new Playlist(id, title, userId);
    }

    public static Playlist Copy(Playlist other)
    {
        return new Playlist(other);
    }

    public static Playlist CreateEmpty()
    {
        return new Playlist();
    }
}
