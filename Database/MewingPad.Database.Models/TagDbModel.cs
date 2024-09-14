using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;

[Table("Tags")]
public class TagDbModel
{
    public TagDbModel(Guid id,
                            Guid authorId,
                            string name)
    {
        Id = id;
        AuthorId = authorId;
        Name = name;
    }

    public TagDbModel()
    {
    }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [ForeignKey("Author")]
    [Column("author_id")]
    public Guid AuthorId { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(64)")]
    public string Name { get; set; }

    public UserDbModel? Author { get; set; }

    public List<AudiotrackDbModel> Audiotracks { get; } = [];
    public List<TagAudiotrackDbModel> TagsAudiotracks { get; set; } = [];
}