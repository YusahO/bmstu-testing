using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Audiotracks")]
public class AudiotrackDbModel
{
    public AudiotrackDbModel()
    {
        Id = new();
        Title = "";
        AuthorId = new();
        Filepath = "";
    }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("title", TypeName = "varchar(64)")]
    public string Title { get; set; }

    [ForeignKey("Author")]
    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Required]
    [Column("filepath", TypeName = "text")]
    public string Filepath { get; set; }

    [Column("mean_score", TypeName = "float")]
    public float MeanScore { get; set; }

    public UserDbModel? Author { get; set; }

    public List<PlaylistDbModel> Playlists { get; } = [];
    public List<PlaylistAudiotrackDbModel> PlaylistsAudiotracks { get; set; } = [];

    public List<TagDbModel> Tags { get; } = [];
    public List<TagAudiotrackDbModel> TagsAudiotracks { get; set; } = [];

    public AudiotrackDbModel(Guid id,
                             string title,
                             Guid authorId,
                             string filepath,
                             float meanScore = 0.0f)
    {
        Id = id;
        Title = title;
        AuthorId = authorId;
        Filepath = filepath;
        MeanScore = meanScore;
    }
}