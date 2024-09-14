using MewingPad.Common.Entities;
namespace MewingPad.Services.PlaylistService;

public interface IPlaylistService
{
    Task CreatePlaylist(Playlist playlist);
    Task<Playlist> UpdatePlaylistTitle(Guid playlistId, string title);
    Task DeletePlaylist(Guid playlistId);
    Task<Playlist> GetPlaylistById(Guid playlistId);
    Task<List<Playlist>> GetUserPlaylists(Guid userId);
    Task<Playlist> GetUserFavouritesPlaylist(Guid userId);
    Task<List<Audiotrack>> GetAllAudiotracksFromPlaylist(Guid playlistId);
    Task<List<Playlist>> GetUserPlaylistsContainingAudiotrack(Guid userId, Guid audiotrackId);
    Task AddAudiotrackToPlaylist(Guid playlistId, Guid audiotrackId);
    Task RemoveAudiotrackFromPlaylist(Guid playlistId, Guid audiotrackId);
    Task RemoveAudiotracksFromPlaylist(Guid playlistId, List<Guid> audiotrackIds);
}
