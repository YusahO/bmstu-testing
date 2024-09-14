using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Scores")]
public class ScoreDbModel
{
    public ScoreDbModel(Guid authorId,
                              Guid audiotrackId,
                              int value)
    {
        AuthorId = authorId;
        AudiotrackId = audiotrackId;
        Value = value;
    }

    public ScoreDbModel()
    {
    }

    [ForeignKey("Author")]
    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [ForeignKey("Audiotrack")]
    [Column("audiotrack_id")]
    public Guid AudiotrackId { get; set; }

    [Required]
    [Column("value", TypeName = "integer")]
    public int Value { get; set; }

    public UserDbModel? Author { get; set; }
    public AudiotrackDbModel? Audiotrack { get; set; }
}