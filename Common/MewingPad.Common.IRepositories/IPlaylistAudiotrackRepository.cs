using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IPlaylistAudiotrackRepository
{
    Task DeleteByPlaylist(Guid playlistId);
    Task DeleteByAudiotrack(Guid audiotrackId);
    Task AddAudiotrackToPlaylist(Guid playlistId, Guid audiotrackId);
    Task<List<Audiotrack>> GetAllAudiotracksFromPlaylist(Guid playlistId);
    Task RemoveAudiotrackFromPlaylist(Guid playlistId, Guid audiotrackId);
    Task<bool> IsAudiotrackInPlaylist(Guid audiotrackId, Guid playlistId);
}