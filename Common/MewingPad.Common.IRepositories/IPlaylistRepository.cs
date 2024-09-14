using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IPlaylistRepository
{
    Task AddPlaylist(Playlist playlist);
    Task<Playlist> UpdatePlaylist(Playlist playlist);
    Task DeletePlaylist(Guid playlistId);
    Task<List<Playlist>> GetUserPlaylists(Guid userId);
    Task<Playlist?> GetUserFavouritesPlaylist(Guid userId);
    Task<List<Playlist>> GetAllPlaylists();
    Task<Playlist?> GetPlaylistById(Guid playlistId);
}