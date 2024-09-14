using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using MewingPad.Database.Models.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class TagRepository(MewingPadDbContext context) : ITagRepository
{
    private readonly MewingPadDbContext _context = context;

    private readonly ILogger _logger = Log.ForContext<TagRepository>();

    public async Task AddTag(Tag tag)
    {
        _logger.Verbose("Entering AddTag");

        try
        {
            await _context.Tags.AddAsync(TagConverter.CoreToDbModel(tag)!);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting AddTag");
    }

    public async Task DeleteTag(Guid tagId)
    {
        _logger.Verbose("Entering DeleteTag");

        try
        {
            var tagDbModel = await _context.Tags.FindAsync(tagId);
            _context.Tags.Remove(tagDbModel!);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteTag");
    }

    public async Task<List<Tag>> GetAllTags()
    {
        _logger.Verbose("Entering GetAllTags");

        List<Tag> tags;
        try
        {
            tags = await _context.Tags
                    .Select(t => TagConverter.DbToCoreModel(t))
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAllTags");
        return tags;
    }

    public async Task<Tag?> GetTagById(Guid tagId)
    {
        _logger.Verbose("Entering GetTagById");

        Tag? tag;
        try
        {
            var tagDbModel = await _context.Tags.FindAsync(tagId);
            tag = TagConverter.DbToCoreModel(tagDbModel);
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetTagById");
        return tag;
    }

    public async Task<Tag> UpdateTag(Tag tag)
    {
        _logger.Verbose("Entering UpdateTag");

        try
        {
            var tagDbModel = await _context.Tags.FindAsync(tag.Id);

            tagDbModel!.Id = tag.Id;
            tagDbModel!.AuthorId = tag.AuthorId;
            tagDbModel!.Name = tag.Name;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting UpdateTag");
        return tag;
    }
}