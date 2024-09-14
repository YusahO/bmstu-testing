using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using Serilog;

namespace MewingPad.Services.CommentaryService;

public class CommentaryService(ICommentaryRepository commentaryRepository,
                               IAudiotrackRepository audiofileRepository) : ICommentaryService
{
    private readonly ICommentaryRepository _commentaryRepository = commentaryRepository;
    private readonly IAudiotrackRepository _audiofileRepository = audiofileRepository;
    private readonly ILogger _logger = Log.ForContext<CommentaryService>();

    public async Task CreateCommentary(Commentary commentary)
    {
        _logger.Verbose("Entering CreateCommentary({@Commentary})");

        if (await _commentaryRepository.GetCommentaryById(commentary.Id) is not null)
        {
            _logger.Error($"Commentary (Id = {commentary.Id}) already exists");
            throw new CommentaryExistsException(commentary.Id);
        }
        await _commentaryRepository.AddCommentary(commentary);
        _logger.Information($"Commentary (Id = {commentary.Id}) added");
        _logger.Verbose("Exiting CreateCommentary");
    }

    public async Task DeleteCommentary(Guid commentaryId)
    {
        _logger.Verbose($"Entering DeleteCommentary({commentaryId})");

        if (await _commentaryRepository.GetCommentaryById(commentaryId) is null)
        {
            _logger.Error($"Commentary (Id = {commentaryId}) not found");
            throw new CommentaryNotFoundException(commentaryId);
        }

        await _commentaryRepository.DeleteCommentary(commentaryId);
        _logger.Information($"Commentary (Id = {commentaryId}) deleted");
        _logger.Verbose("Exiting DeleteCommentary method");
    }

    public async Task<List<Commentary>> GetAudiotrackCommentaries(Guid audiotrackId)
    {
        _logger.Verbose($"Entering GetAudiotrackCommentaries({audiotrackId})");

        if (await _audiofileRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }

        var commentaries = await _commentaryRepository.GetAudiotrackCommentaries(audiotrackId);
        _logger.Verbose("Exiting GetAudiotrackCommentaries");
        return commentaries;
    }

    public async Task<Commentary> GetCommentaryById(Guid commentaryId)
    {
        _logger.Verbose($"Entering GetCommentaryById({commentaryId})");

        Commentary? commentary;
        if ((commentary = await _commentaryRepository.GetCommentaryById(commentaryId)) is null)
        {
            _logger.Error($"Commentary (Id = {commentaryId}) not found");
            throw new CommentaryNotFoundException(commentaryId);
        }

        _logger.Verbose("Exiting GetCommentaryById");
        return commentary;
    }

    public async Task<Commentary> UpdateCommentary(Commentary commentary)
    {
        _logger.Verbose("Entering UpdateCommentary({@Commentary})", commentary);

        if (await _commentaryRepository.GetCommentaryById(commentary.Id) is null)
        {
            _logger.Error($"Commentary (Id = {commentary.Id}) not found");
            throw new CommentaryNotFoundException(commentary.Id);
        }

        await _commentaryRepository.UpdateCommentary(commentary);
        _logger.Information($"Commentary (Id = {commentary.Id}) updated");
        _logger.Verbose("Exiting UpdateCommentary");
        return commentary;
    }
}
