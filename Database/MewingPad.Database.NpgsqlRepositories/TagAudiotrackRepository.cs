using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using MewingPad.Database.Models.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class TagAudiotrackRepository(MewingPadDbContext context) : ITagAudiotrackRepository
{
    private readonly MewingPadDbContext _context = context;

    private readonly ILogger _logger = Log.ForContext<TagAudiotrackRepository>();

    public async Task DeleteByTag(Guid tagId)
    {
        _logger.Verbose("Entering DeleteByTag");

        try
        {
            var pairs = await _context.TagsAudiotracks
                .Where(ta => ta.TagId == tagId)
                .ToListAsync();
            if (pairs.Count == 0)
            {
                return;
            }
            _context.TagsAudiotracks.RemoveRange(pairs);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteByTag");
    }

    public async Task DeleteByAudiotrack(Guid audiotrackId)
    {
        _logger.Verbose("Entering DeleteByTag");

        try
        {
            var pairs = await _context.TagsAudiotracks
                .Where(ta => ta.TagId == audiotrackId)
                .ToListAsync();
            if (pairs.Count == 0)
            {
                return;
            }
            _context.TagsAudiotracks.RemoveRange(pairs);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteByTag");
    }

    public async Task AssignTagToAudiotrack(Guid audiotrackId, Guid tagId)
    {
        _logger.Verbose($"Entering AssignTagToAudiotrack({audiotrackId}, {tagId})");

        try
        {
            await _context.TagsAudiotracks.AddAsync(new(tagId, audiotrackId));
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting AssignTagToAudiotrack");
    }

    public async Task<List<Audiotrack>> GetAudiotracksWithTags(List<Guid> tagIds)
    {
        _logger.Verbose("Entering GetAudiotracksWithTags({Ids})", tagIds);

        List<Audiotrack?> audiotracks;
        try
        {
            audiotracks = await _context.TagsAudiotracks
                .Where(ta => tagIds.Contains(ta.TagId))
                .Include(ta => ta.Audiotrack)
                .Select(ta => AudiotrackConverter.DbToCoreModel(ta.Audiotrack))
                .Distinct()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAudiotracksWithTags");
        return audiotracks!;
    }

    public async Task<List<Tag>> GetAudiotrackTags(Guid audiotrackId)
    {
        _logger.Verbose($"Entering GetAudiotrackTags({audiotrackId})");

        List<Tag?> tags;
        try
        {
            tags = await _context.TagsAudiotracks
                    .Where(ta => ta.AudiotrackId == audiotrackId)
                    .Include(ta => ta.Tag)
                    .Select(ta => TagConverter.DbToCoreModel(ta.Tag))
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAudiotrackTags");
        return tags!;
    }

    public async Task RemoveTagFromAudiotrack(Guid audiotrackId, Guid tagId)
    {
        _logger.Verbose($"Entering RemoveTagFromAudiotrack({audiotrackId}, {tagId})");

        try
        {
            _context.TagsAudiotracks.Remove(new(tagId, audiotrackId));
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting RemoveTagFromAudiotrack");

    }
}