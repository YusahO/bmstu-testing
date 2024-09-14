using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IUserFavouriteRepository
{
	Task AddUserFavouritePlaylist(Guid userId, Guid playlistId);
}
