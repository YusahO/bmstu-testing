using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Commentaries")]
public class CommentaryDbModel
{
    public CommentaryDbModel(Guid id,
                             Guid authorId,
                             Guid audiotrackId,
                             string text)
    {
        Id = id;
        AuthorId = authorId;
        AudiotrackId = audiotrackId;
        Text = text;
    }

    public CommentaryDbModel()
    {
    }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [ForeignKey("Author")]
    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [ForeignKey("Audiotrack")]
    [Column("audiotrack_id")]
    public Guid AudiotrackId { get; set; }

    [Required]
    [Column("text", TypeName = "text")]
    public string Text { get; set; }

    public UserDbModel? Author { get; set; }
    public AudiotrackDbModel? Audiotrack { get; set; }
}