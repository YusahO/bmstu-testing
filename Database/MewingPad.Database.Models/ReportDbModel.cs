using MewingPad.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Reports")]
public class ReportDbModel
{
    public ReportDbModel(Guid id,
                               Guid authorId,
                               Guid audiotrackId,
                               string text,
                               ReportStatus status)
    {
        Id = id;
        AuthorId = authorId;
        AudiotrackId = audiotrackId;
        Text = text;
        Status = status;
    }

    public ReportDbModel()
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

    [Required]
    [Column("status", TypeName = "varchar(50)")]
    public ReportStatus Status { get; set; }

    public UserDbModel? Author { get; set; }
    public AudiotrackDbModel? Audiotrack { get; set; }
}