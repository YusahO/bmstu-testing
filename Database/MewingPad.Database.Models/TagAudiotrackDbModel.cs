using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;


[Table("TagsAudiotracks")]
public class TagAudiotrackDbModel(Guid tagId,
                                  Guid audiotrackId)
{
    [ForeignKey("Tag")]
    [Column("tag_id")]
    public Guid TagId { get; set; } = tagId;

    [ForeignKey("Audiotrack")]
    [Column("audiotrack_id")]
    public Guid AudiotrackId { get; set; } = audiotrackId;

    public TagDbModel? Tag { get; set; }
    public AudiotrackDbModel? Audiotrack { get; set; }
}