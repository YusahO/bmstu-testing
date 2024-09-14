using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using Serilog;

namespace MewingPad.Services.TagService;

public class TagService(ITagRepository tagRepository,
                        IAudiotrackRepository audiofileRepository,
                        ITagAudiotrackRepository tagAudiotrackRepository) : ITagService
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly IAudiotrackRepository _audiofileRepository = audiofileRepository;
    private readonly ITagAudiotrackRepository _tagAudiotrackRepository = tagAudiotrackRepository;

    private readonly ILogger _logger = Log.ForContext<TagService>();

    public async Task CreateTag(Tag tag)
    {
        _logger.Verbose("Entering CreateTag({@Tag})", tag);

        if (await _tagRepository.GetTagById(tag.Id) is not null)
        {
            _logger.Error($"Tag (Id = {tag.Id}) already exists");
            throw new TagExistsException(tag.Id);
        }
        await _tagRepository.AddTag(tag);
        _logger.Information($"Tag (Id = {tag.Id}) added");

        _logger.Verbose("Exiting CreateTag");
    }

    public async Task<Tag> UpdateTagName(Guid tagId, string tagName)
    {
        _logger.Verbose($"Entering UpdateTagName({tagId}, {tagName})");

        var tag = await _tagRepository.GetTagById(tagId);
        if (tag is null)
        {
            _logger.Error($"Tag (Id = {tagId}) not found");
            throw new TagNotFoundException(tagId);
        }
        tag.Name = tagName;
        await _tagRepository.UpdateTag(tag);
        _logger.Information($"Tag (Id = {tag.Id}) updated");

        _logger.Verbose("Exiting UpdateTagName");
        return tag;
    }

    public async Task DeleteTag(Guid tagId)
    {
        _logger.Verbose($"Entering DeleteTag({tagId})");

        if (await _tagRepository.GetTagById(tagId) is null)
        {
            _logger.Error($"Tag (Id = {tagId}) not found");
            throw new TagNotFoundException(tagId);
        }

        await _tagAudiotrackRepository.DeleteByTag(tagId);
        await _tagRepository.DeleteTag(tagId);
        _logger.Information($"Tag (Id = {tagId}) deleted");

        _logger.Verbose("Exiting DeleteTag");
    }

    public async Task<Tag> GetTagById(Guid tagId)
    {
        _logger.Verbose($"Entering GetTagById({tagId})");

        var tag = await _tagRepository.GetTagById(tagId);
        if (tag is null)
        {
            _logger.Error($"Tag (Id = {tagId}) not found");
            throw new TagNotFoundException(tagId);
        }

        _logger.Verbose("Exiting GetTagById");
        return tag;
    }

    public async Task<List<Tag>> GetAudiotrackTags(Guid audiotrackId)
    {
        _logger.Verbose("Entering GetAudiotrackTags");

        if (await _audiofileRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }
        var tags = await _tagAudiotrackRepository.GetAudiotrackTags(audiotrackId);

        _logger.Verbose("Exiting GetAudiotrackTags");
        return tags;
    }

    public async Task<List<Tag>> GetAllTags()
    {
        _logger.Verbose("Entering GetAllTags");
        var tags = await _tagRepository.GetAllTags();
        _logger.Verbose("Exiting GetAllTags");
        return tags;
    }

    public async Task<List<Audiotrack>> GetAudiotracksWithTags(List<Guid> tagIds)
    {
        _logger.Verbose("Entering GetAudiotracksWithTags({Tags})", tagIds);
        foreach (var tid in tagIds)
        {
            if (await _tagRepository.GetTagById(tid) is null)
            {
                _logger.Error($"Tag (Id = {tid}) not found");
                throw new TagNotFoundException(tid);
            }
        }
        var audios = await _tagAudiotrackRepository.GetAudiotracksWithTags(tagIds);

        _logger.Verbose("Exiting GetAudiotracksWithTags");
        return audios;
    }

    public async Task AssignTagToAudiotrack(Guid audiotrackId, Guid tagId)
    {
        _logger.Verbose($"Entering AssignTagToAudiotrack({audiotrackId}, {tagId})");

        if (await _tagRepository.GetTagById(tagId) is null)
        {
            _logger.Error($"Tag (Id = {tagId}) not found");
            throw new TagNotFoundException(tagId);
        }
        if (await _audiofileRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }
        await _tagAudiotrackRepository.AssignTagToAudiotrack(audiotrackId, tagId);
        _logger.Information($"Tag (Id = {tagId}) assigned to audiotrack (Id = {audiotrackId})");

        _logger.Verbose("Exiting AssignTagToAudiotrack");
    }

    public async Task DeleteTagFromAudiotrack(Guid audiotrackId, Guid tagId)
    {
        _logger.Verbose($"Entering DeleteTagFromAudiotrack({audiotrackId}, {tagId})");

        if (await _tagRepository.GetTagById(tagId) is null)
        {
            _logger.Error($"Tag (Id = {tagId}) not found");
            throw new TagNotFoundException(tagId);
        }
        if (await _audiofileRepository.GetAudiotrackById(audiotrackId) is null)
        {
            _logger.Error($"Audiotrack (Id = {audiotrackId}) not found");
            throw new AudiotrackNotFoundException(audiotrackId);
        }
        await _tagAudiotrackRepository.RemoveTagFromAudiotrack(audiotrackId, tagId);
        _logger.Information($"Tag (Id = {tagId}) removed from audiotrack (Id = {audiotrackId})");

        _logger.Verbose("Exiting DeleteTagFromAudiotrack");
    }
}
