using Microsoft.EntityFrameworkCore; 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Playlists")]
public class PlaylistDbModel
{
    public PlaylistDbModel(Guid id,
                                 string title,
                                 Guid userId)
    {
        Id = id;
        Title = title;
        UserId = userId;
    }

    public PlaylistDbModel()
    {
    }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title", TypeName = "varchar(64)")]
    public string Title { get; set; }

    [ForeignKey(nameof(User))]
    [Column("user_id")]
    public Guid UserId { get; set; }

    public UserDbModel? User { get; set; }

    public List<AudiotrackDbModel> Audiotracks { get; } = [];
    public List<PlaylistAudiotrackDbModel> PlaylistsAudiotracks { get; } = [];
}