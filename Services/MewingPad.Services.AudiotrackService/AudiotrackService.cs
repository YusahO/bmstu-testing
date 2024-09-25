using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Utils.AudioManager;
using Serilog;

namespace MewingPad.Services.AudiotrackService;

public class AudiotrackService(
    IAudiotrackRepository audiotrackRepository,
    IPlaylistAudiotrackRepository playlistAudiotrackRepository,
    ITagAudiotrackRepository tagAudiotrackRepository,
    AudioManager audioManager
) : IAudiotrackService
{
    private readonly IAudiotrackRepository _audiotrackRepository =
        audiotrackRepository;
    private readonly IPlaylistAudiotrackRepository _playlistAudiotrackRepository =
        playlistAudiotrackRepository;
    private readonly ITagAudiotrackRepository _tagAudiotrackRepository =
        tagAudiotrackRepository;
    private readonly AudioManager _audioManager = audioManager;
    private readonly ILogger _logger = Log.ForContext<AudiotrackService>();

    public async Task CreateAudiotrackWithStream(
        Stream audioStream,
        Audiotrack audiotrack
    )
    {
        _logger.Verbose("Entering CreateAudiotrackWithStream method");

        if (
            await _audiotrackRepository.GetAudiotrackById(audiotrack.Id)
            is not null
        )
        {
            _logger.Error(
                "Audiotrack {@Audio} already exists",
                new { audiotrack.Id, audiotrack.Title }
            );
            throw new AudiotrackExistsException(audiotrack.Id);
        }

        try
        {
            await _audiotrackRepository.AddAudiotrack(audiotrack);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Exception thrown {Message}");
            throw;
        }

        string filename = audiotrack.Filepath;
        if (
            !await _audioManager.CreateFileFromStreamAsync(
                audioStream,
                filename
            )
        )
        {
            _logger.Error(
                $"Failed to upload audiotrack with name \"{filename}\""
            );
            await _audiotrackRepository.DeleteAudiotrack(audiotrack.Id);
            throw new AudiotrackServerUploadException(
                $"Failed to upload audiotrack with name \"{filename}\""
            );
        }

        _logger.Information(
            "Created audiotrack {@Audio} in database",
            new { audiotrack.Id, audiotrack.Title }
        );

        _logger.Verbose("Exiting CreateAudiotrackWithStream method");
    }

    public async Task<Audiotrack> UpdateAudiotrackWithStream(
        Stream stream,
        Audiotrack audiotrack
    )
    {
        _logger.Verbose("Entering UpdateAudiotrackWithStream");

        var oldAudiotrack = await _audiotrackRepository.GetAudiotrackById(
            audiotrack.Id
        );
        if (oldAudiotrack is null)
        {
            _logger.Error(
                "Audiotrack {@Audio} not found",
                new { audiotrack.Id, audiotrack.Title }
            );
            throw new AudiotrackNotFoundException(audiotrack.Id);
        }

        await _audiotrackRepository.UpdateAudiotrack(audiotrack);

        _logger.Information(
            "Created audiotrack {@Audio} in database",
            new { audiotrack.Id, audiotrack.Title }
        );

        string filename = audiotrack.Filepath;
        if (!await _audioManager.UpdateFileFromStreamAsync(stream, filename))
        {
            _logger.Error(
                $"Failed to update audiotrack with name \"{filename}\""
            );
            await _audiotrackRepository.UpdateAudiotrack(oldAudiotrack);
            throw new AudiotrackServerUploadException(
                $"Failed to update audiotrack with name \"{filename}\""
            );
        }

        _logger.Verbose("Exiting UpdateAudiotrackWithStream");
        return audiotrack;
    }

    public async Task DeleteAudiotrack(Guid audiotrackId)
    {
        _logger.Information($"Entering DeleteAudiotrack({audiotrackId})");

        var audiotrack = await _audiotrackRepository.GetAudiotrackById(
            audiotrackId
        );
        if (audiotrack is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }

        if (!await _audioManager.DeleteFileAsync(audiotrack.Filepath))
        {
            _logger.Error(
                $"Failed to delete audiotrack with path \"{audiotrack.Filepath}\""
            );
            throw new AudiotrackServerDeleteException(
                $"Failed to delete audiotrack with path \"{audiotrack.Filepath}\""
            );
        }

        await _tagAudiotrackRepository.DeleteByAudiotrack(audiotrackId);
        await _playlistAudiotrackRepository.DeleteByAudiotrack(audiotrackId);
        await _audiotrackRepository.DeleteAudiotrack(audiotrackId);

        _logger.Verbose("Exiting DeleteAudiotrack");
    }

    public async Task<Audiotrack> GetAudiotrackById(Guid audiotrackId)
    {
        _logger.Verbose($"Entering GetAudiotrackById({audiotrackId})");

        var audiotrack = await _audiotrackRepository.GetAudiotrackById(
            audiotrackId
        );
        if (audiotrack is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }

        _logger.Verbose("Exiting GetAudiotrackById");
        return audiotrack;
    }

    public async Task<List<Audiotrack>> GetAllAudiotracks()
    {
        _logger.Verbose("Entering GetAllAudiotracks()");

        var audios = await _audiotrackRepository.GetAllAudiotracks();
        if (audios.Count == 0)
        {
            _logger.Warning("Database has no entries of Audiotrack");
        }

        _logger.Verbose("Exiting GetAllAudiotracks");
        return audios;
    }

    public async Task<List<Audiotrack>> GetAudiotracksByTitle(string title)
    {
        _logger.Verbose($"Entering GetAudiotracksByTitle({title})");

        var audios = await _audiotrackRepository.GetAudiotracksByTitle(title);
        if (audios.Count == 0)
        {
            _logger.Warning($"No audiotracks found with title \"{title}\"");
        }

        _logger.Verbose("Exiting GetAudiotracksByTitle");
        return audios;
    }

    public async Task<Stream> GetAudiotrackFileStream(string srcpath)
    {
        _logger.Verbose($"Entering GetAudiotrackFileStream({srcpath})");

        var stream = await _audioManager.GetFileStreamAsync(srcpath);
        if (stream is null)
        {
            _logger.Error(
                $"Failed to get audiotrack (\"{srcpath}\") from server"
            );
            throw new AudiotrackServerGetException($"{srcpath} does not exist");
        }

        _logger.Verbose("Exiting GetAudiotrackFileStream");
        return stream;
    }
}
