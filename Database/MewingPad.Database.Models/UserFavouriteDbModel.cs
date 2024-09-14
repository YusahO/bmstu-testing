using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;


[Table("UsersFavourites")]
public class UserFavouriteDbModel(Guid userId,
                                  Guid favouriteId)
{
    [ForeignKey(nameof(User))]
    [Column("user_id")]
    public Guid UserId { get; set; } = userId;

    [ForeignKey(nameof(Playlist))]
    [Column("favourite_id")]
    public Guid FavouriteId { get; set; } = favouriteId;

    public void Deconstruct(out Guid userId, out Guid favouriteId)
    {
        userId = UserId;
        favouriteId = FavouriteId;
    }

    public UserDbModel? User { get; set; } = null!;
    public PlaylistDbModel? Playlist { get; set; } = null!;
}
