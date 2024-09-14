using System.Data.SqlClient;
using MewingPad.Common.Entities;
using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using MewingPad.Database.Models.Converters;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class PlaylistAudiotrackRepository(MewingPadDbContext context) : IPlaylistAudiotrackRepository
{
    private readonly MewingPadDbContext _context = context;

    private readonly ILogger _logger = Log.ForContext<PlaylistAudiotrackRepository>();

    public async Task DeleteByPlaylist(Guid playlistId)
    {
        _logger.Verbose("Entering DeleteByPlaylist");

        try
        {
            var pairs = await _context.PlaylistsAudiotracks
                .Where(pa => pa.PlaylistId == playlistId)
                .ToListAsync();
            if (pairs.Count == 0)
            {
                return;
            }
            _context.RemoveRange(pairs);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteByPlaylist");
    }

    public async Task DeleteByAudiotrack(Guid audiotrackId)
    {
        _logger.Verbose("Entering DeleteByAudiotrack");

        try
        {
            var pairs = await _context.PlaylistsAudiotracks
                .Where(pa => pa.AudiotrackId == audiotrackId)
                .ToListAsync();
            if (pairs.Count == 0)
            {
                return;
            }
            _context.RemoveRange(pairs);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting DeleteByAudiotrack");
    }

    public async Task AddAudiotrackToPlaylist(Guid playlistId, Guid audiotrackId)
    {
        _logger.Verbose("Entering AddAudiotrackToPlaylist");

        try
        {
            await _context.PlaylistsAudiotracks
                    .AddAsync(new(playlistId, audiotrackId));
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting AddAudiotrackToPlaylist");
    }

    public async Task<List<Audiotrack>> GetAllAudiotracksFromPlaylist(Guid playlistId)
    {
        _logger.Verbose("Entering GetAllAudiotracksFromPlaylist");

        List<Audiotrack> audiotracks;
        try
        {
            audiotracks = await _context.PlaylistsAudiotracks
                    .Where(pa => pa.PlaylistId == playlistId)
                    .Include(pa => pa.Audiotrack)
                    .Select(pa => AudiotrackConverter.DbToCoreModel(pa.Audiotrack))
                    .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting GetAllAudiotracksFromPlaylist");
        return audiotracks!;
    }

    public async Task RemoveAudiotrackFromPlaylist(Guid playlistId, Guid audiotrackId)
    {
        _logger.Verbose("Entering RemoveAudiotrackFromPlaylist");

        try
        {
            _context.PlaylistsAudiotracks.Remove(new(playlistId, audiotrackId));
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting RemoveAudiotrackFromPlaylist");
    }

    public async Task RemoveAudiotracksFromPlaylist(Guid playlistId, List<Guid> audiotrackIds)
    {
        _logger.Verbose("Entering RemoveAudiotracksFromPlaylist");

        try
        {
            foreach (var aid in audiotrackIds)
            {
                var paDbModel = await _context.PlaylistsAudiotracks
                    .FirstOrDefaultAsync(pa => pa.PlaylistId == playlistId &&
                                               pa.AudiotrackId == aid);

                if (paDbModel is not null)
                {
                    _context.PlaylistsAudiotracks.Remove(paDbModel);
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting RemoveAudiotracksFromPlaylist");
    }

    public async Task<bool> IsAudiotrackInPlaylist(Guid audiotrackId, Guid playlistId)
    {
        _logger.Verbose("Entering IsAudiotrackInPlaylist");

        bool inPlaylist;
        try
        {
             inPlaylist = await _context.PlaylistsAudiotracks
                    .AnyAsync(pa => pa.AudiotrackId == audiotrackId &&
                                    pa.PlaylistId == playlistId);
        }
        catch (Exception ex)
        {
            throw new RepositoryException(ex.Message, ex.InnerException);
        }

        _logger.Verbose("Exiting IsAudiotrackInPlaylist");
        return inPlaylist;
    }
}