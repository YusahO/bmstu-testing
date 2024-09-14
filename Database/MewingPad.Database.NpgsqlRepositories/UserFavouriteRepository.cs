using MewingPad.Common.Exceptions;
using MewingPad.Common.IRepositories;
using MewingPad.Database.Context;
using Serilog;

namespace MewingPad.Database.NpgsqlRepositories;

public class UserFavouriteRepository(MewingPadDbContext context) : IUserFavouriteRepository
{
	private readonly MewingPadDbContext _context = context;
	private readonly ILogger _logger = Log.ForContext<UserFavouriteRepository>();
	public async Task AddUserFavouritePlaylist(Guid userId, Guid playlistId)
	{
		_logger.Verbose("Entering AddUserFavouritePlaylist");

		try
		{
			await _context.UsersFavourites.AddAsync(new(userId, playlistId));
			await _context.SaveChangesAsync();
		}
		catch (Exception ex)
		{
			throw new RepositoryException(ex.Message, ex.InnerException);
		}

		_logger.Verbose("Exiting AddUserFavouritePlaylist");
	}
}
