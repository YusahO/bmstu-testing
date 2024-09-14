using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("PlaylistsAudiotracks")]
public class PlaylistAudiotrackDbModel(Guid playlistId,
                                       Guid audiotrackId)
{
    [Column("playlist_id")]
    public Guid PlaylistId { get; set; } = playlistId;
    [Column("audiotrack_id")]
    public Guid AudiotrackId { get; set; } = audiotrackId;

    public PlaylistDbModel Playlist { get; set; } = null!;
    public AudiotrackDbModel Audiotrack { get; set; } = null!;
}