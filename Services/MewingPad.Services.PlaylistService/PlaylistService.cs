using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using Serilog;

namespace MewingPad.Services.PlaylistService;

public class PlaylistService(IPlaylistRepository playlistRepository,
                             IAudiotrackRepository audiofileRepository,
                             IUserRepository userRepository,
                             IPlaylistAudiotrackRepository playlistAudiotrackRepository) : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepository = playlistRepository
                                                               ?? throw new ArgumentNullException();
    private readonly IAudiotrackRepository _audiotrackRepository = audiofileRepository
                                                                 ?? throw new ArgumentNullException();
    private readonly IUserRepository _userRepository = userRepository
                                                       ?? throw new ArgumentNullException();
    private readonly IPlaylistAudiotrackRepository _playlistAudiotrackRepository = playlistAudiotrackRepository
                                                                                   ?? throw new ArgumentNullException();

    private readonly ILogger _logger = Log.ForContext<PlaylistService>();

    public async Task CreatePlaylist(Playlist playlist)
    {
        _logger.Verbose("Entering CreatePlaylist({@Playlist})", playlist);

        if (await _playlistRepository.GetPlaylistById(playlist.Id) is not null)
        {
            _logger.Error($"Playlist (Id = {playlist.Id}) already exists");
            throw new PlaylistExistsException(playlist.Id);
        }
        await _playlistRepository.AddPlaylist(playlist);
        _logger.Information($"Playlist (Id = {playlist.Id}) added");

        _logger.Verbose("Exiting CreatePlaylist method");
    }

    public async Task<Playlist> UpdatePlaylistTitle(Guid playlistId, string title)
    {
        _logger.Verbose($"Entering UpdatePlaylistTitle({playlistId}, {title})");

        var playlist = await _playlistRepository.GetPlaylistById(playlistId);
        if (playlist is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        playlist.Title = title;

        await _playlistRepository.UpdatePlaylist(playlist);
        _logger.Information($"Playlist (Id = {playlist.Id}) updated");
        _logger.Verbose("Exiting UpdatePlaylistTitle");
        return playlist;
    }

    public async Task DeletePlaylist(Guid playlistId)
    {
        _logger.Verbose($"Entering DeletePlaylist({playlistId})");

        if (await _playlistRepository.GetPlaylistById(playlistId) is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }

        await _playlistAudiotrackRepository.DeleteByPlaylist(playlistId);
        await _playlistRepository.DeletePlaylist(playlistId);
        _logger.Information($"Playlist (Id = {playlistId}) deleted");

        _logger.Verbose("Exiting DeletePlaylist");
    }

    public async Task AddAudiotrackToPlaylist(Guid playlistId, Guid audiotrackId)
    {
        _logger.Verbose($"Entering AddAudiotrackToPlaylist({playlistId}, {audiotrackId})");

        if (await _playlistRepository.GetPlaylistById(playlistId) is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        if (await _audiotrackRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }

        if (await _playlistAudiotrackRepository.IsAudiotrackInPlaylist(playlistId, audiotrackId))
        {
            _logger.Warning($"Audiotrack (Id = {audiotrackId}) is already in playlist (Id = {playlistId})");
            throw new AudiotrackExistsInPlaylistException(playlistId, audiotrackId);
        }
        await _playlistAudiotrackRepository.AddAudiotrackToPlaylist(playlistId, audiotrackId);
        _logger.Information($"Audiotrack ({audiotrackId}) added to playlist (Id = {playlistId})");

        _logger.Verbose("Exiting AddAudiotrackToPlaylist");
    }

    public async Task RemoveAudiotrackFromPlaylist(Guid playlistId, Guid audiotrackId)
    {
        _logger.Verbose($"Entering RemoveAudiotrackFromPlaylist({playlistId}, {audiotrackId})");

        if (await _playlistRepository.GetPlaylistById(playlistId) is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        if (await _audiotrackRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }
        if (!await _playlistAudiotrackRepository.IsAudiotrackInPlaylist(audiotrackId, playlistId))
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) is not in playlist (Id = {playlistId})");
            throw new AudiotrackNotFoundInPlaylistException(playlistId, audiotrackId);
        }
        await _playlistAudiotrackRepository.RemoveAudiotrackFromPlaylist(playlistId, audiotrackId);
        _logger.Information($"Audiotrack ({audiotrackId}) removed from playlist (Id = {playlistId})");

        _logger.Verbose("Exiting RemoveAudiotrackFromPlaylist");
    }

    public async Task RemoveAudiotracksFromPlaylist(Guid playlistId, List<Guid> audiotrackIds)
    {
        _logger.Verbose("Entering RemoveAudiotracksFromPlaylist({@PlaylistId}, {@Audios})", playlistId, audiotrackIds);

        if (await _playlistRepository.GetPlaylistById(playlistId) is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        foreach (var aid in audiotrackIds)
        {
            if (await _audiotrackRepository.GetAudiotrackById(aid) is null)
            {
                _logger.Error($"Audiotrack (Id = {aid}) not found");
                throw new AudiotrackNotFoundException(aid);
            }
            if (!await _playlistAudiotrackRepository.IsAudiotrackInPlaylist(playlistId, aid))
            {
                _logger.Error($"Audiotrack (Id = {aid}) is not in playlist (Id = {playlistId})");
                throw new AudiotrackNotFoundInPlaylistException(playlistId, aid);
            }
        }
        await _playlistAudiotrackRepository.RemoveAudiotracksFromPlaylist(playlistId, audiotrackIds);
        _logger.Information("Removed audiotracks {@Audios} from playlist (Id = {@PlaylistId})", audiotrackIds, playlistId);
        _logger.Verbose("Exiting RemoveAudiotracksFromPlaylist");
    }

    public async Task<Playlist> GetUserFavouritesPlaylist(Guid userId)
    {
        _logger.Verbose($"Entering GetUserFavouritesPlaylist({userId})");

        var user = await _userRepository.GetUserById(userId);
        if (user is null)
        {
            _logger.Error($"User (Id = {userId}) not found");
            throw new UserNotFoundException(userId);
        }

        var playlist = await _playlistRepository.GetUserFavouritesPlaylist(user.Id);
        if (playlist is null)
        {
            _logger.Error($"Favourites playlist for user (Id = {user.Id}) not found");
            throw new PlaylistNotFoundException($"Favourites playlist for user (Id = {user.Id}) not found");
        }
        _logger.Verbose("Exiting GetUserFavouritesPlaylist");
        return playlist!;
    }

    public async Task<List<Audiotrack>> GetAllAudiotracksFromPlaylist(Guid playlistId)
    {
        _logger.Verbose($"Entering GetAllAudiotracksFromPlaylist({playlistId})");

        if (await _playlistRepository.GetPlaylistById(playlistId) is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        var audios = await _playlistAudiotrackRepository.GetAllAudiotracksFromPlaylist(playlistId);
        _logger.Verbose("Exiting GetAllAudiotracksFromPlaylist");
        return audios;
    }

    public async Task<Playlist> GetPlaylistById(Guid playlistId)
    {
        _logger.Verbose($"Entering GetPlaylistById({playlistId})");

        var playlist = await _playlistRepository.GetPlaylistById(playlistId);
        if (playlist is null)
        {
            _logger.Error($"Playlist (Id = {playlistId}) not found");
            throw new PlaylistNotFoundException(playlistId);
        }
        _logger.Verbose("Exiting GetPlaylistById");
        return playlist;
    }

    public async Task<List<Playlist>> GetUserPlaylists(Guid userId)
    {
        _logger.Verbose($"Entering GetUserPlaylists({userId})");

        if (await _userRepository.GetUserById(userId) is null)
        {
            _logger.Error($"User (Id = {userId}) not found");
            throw new UserNotFoundException(userId);
        }
        var playlists = await _playlistRepository.GetUserPlaylists(userId);

        _logger.Verbose("Exiting GetUserPlaylists");
        return playlists;
    }

    public async Task<List<Playlist>> GetUserPlaylistsContainingAudiotrack(Guid userId, Guid audiotrackId)
    {
        _logger.Verbose($"Entering GetPlaylistsContainingAudiotrack({audiotrackId})");

        if (await _audiotrackRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }
        if (await _userRepository.GetUserById(userId) is null)
        {
            _logger.Error($"User (Id = {userId}) not found");
            throw new UserNotFoundException(userId);
        }

        var playlists = await _playlistRepository.GetUserPlaylists(userId);
        var resultingPlaylists = new List<Playlist>();
        foreach (var p in playlists)
        {
            if (await _playlistAudiotrackRepository.IsAudiotrackInPlaylist(audiotrackId, p.Id))
            {
                resultingPlaylists.Add(p);
            }
        }
        _logger.Verbose($"Exiting GetPlaylistsContainingAudiotrack");
        return resultingPlaylists;
    }
}