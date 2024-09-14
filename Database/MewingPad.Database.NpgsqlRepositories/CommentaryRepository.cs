using System.Data.SqlClient;
using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using MewingPad.Database.Models.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class CommentaryRepository(MewingPadDbContext context) : ICommentaryRepository
{
    private readonly MewingPadDbContext _context = context;

    private readonly ILogger _logger = Log.ForContext<CommentaryRepository>();

    public async Task AddCommentary(Commentary commentary)
    {
        _logger.Verbose("Entering AddCommentary");

        try
        {
            await _context.Commentaries.AddAsync(CommentaryConverter.CoreToDbModel(commentary)!);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting AddCommentary");
    }

    public async Task DeleteCommentary(Guid commentaryId)
    {
        _logger.Verbose("Entering DeleteCommentary");

        try
        {
            var commentaryDbModel = await _context.Commentaries.FindAsync(commentaryId);
            _context.Commentaries.Remove(commentaryDbModel!);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteCommentary");
    }

    public async Task<List<Commentary>> GetAudiotrackCommentaries(Guid audiotrackId)
    {
        _logger.Verbose("Entering GetAudiotrackCommentaries");

        List<Commentary> commentaries;
        try
        {
            commentaries = await _context.Commentaries
                    .Where(c => c.AudiotrackId == audiotrackId)
                    .Select(c => CommentaryConverter.DbToCoreModel(c))
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAudiotrackCommentaries");
        return commentaries;
    }

    public async Task<Commentary?> GetCommentaryById(Guid commentaryId)
    {
        _logger.Verbose("Entering GetCommentaryById");

        Commentary? commentary;
        try
        {
            var commentaryDbModel = await _context.Commentaries.FindAsync(commentaryId);
            commentary = CommentaryConverter.DbToCoreModel(commentaryDbModel);
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetCommentaryById");
        return commentary;
    }

    public async Task<Commentary> UpdateCommentary(Commentary commentary)
    {
        _logger.Verbose("Entering UpdateCommentary");

        try
        {
            var commentaryDbModel = await _context.Commentaries.FindAsync(commentary.Id);

            commentaryDbModel!.Id = commentary.Id;
            commentaryDbModel!.AuthorId = commentary.AuthorId;
            commentaryDbModel!.AudiotrackId = commentary.AudiotrackId;
            commentaryDbModel!.Text = commentary.Text;

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting UpdateCommentary");
        return commentary;
    }
}