namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class BaseRepositoryTestClass()
{
    protected static Guid MakeGuid(byte i) =>
        new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]);

    #region Audiotracks
    protected static Audiotrack CreateAudiotrackCoreModel(
        Guid id,
        string title,
        Guid authorId,
        string filepath
    )
    {
        return new AudiotrackCoreModelBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithAuthorId(authorId)
            .WithFilepath(filepath)
            .Build();
    }

    protected static AudiotrackDbModel CreateAudiotrackDbo(
        Guid id,
        string title,
        Guid authorId,
        string filepath
    )
    {
        return new AudiotrackDbModelBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithAuthorId(authorId)
            .WithFilepath(filepath)
            .Build();
    }

    protected static AudiotrackDbModel CreateAudiotrackDboFromCore(
        Audiotrack audiotrack
    )
    {
        return CreateAudiotrackDbo(
            audiotrack.Id,
            audiotrack.Title,
            audiotrack.AuthorId,
            audiotrack.Filepath
        );
    }
    #endregion

    #region Playlists
    protected static Playlist CreatePlaylistCoreModel(
        Guid id,
        string title,
        Guid userId
    )
    {
        return new PlaylistCoreModelBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithUserId(userId)
            .Build();
    }

    protected static PlaylistDbModel CreatePlaylistDbo(
        Guid id,
        string title,
        Guid userId
    )
    {
        return new PlaylistDbModelBuilder()
            .WithId(id)
            .WithTitle(title)
            .WithUserId(userId)
            .Build();
    }

    protected static PlaylistDbModel CreatePlaylistDboFromCore(
        Playlist playlist
    )
    {
        return CreatePlaylistDbo(playlist.Id, playlist.Title, playlist.UserId);
    }
    #endregion
}
