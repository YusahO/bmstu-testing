using MewingPad.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MewingPad.Database.Models;


[Table("Users")]
public class UserDbModel
{
    public UserDbModel(Guid id,
                             string username,
                             string passwordHashed,
                             string email,
                             UserRole role)
    {
        Id = id;
        Username = username;
        PasswordHashed = passwordHashed;
        Email = email;
        Role = role;
    }

    public UserDbModel()
    {
    }

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("username", TypeName = "varchar(64)")]
    public string Username { get; set; }

    [Required]
    [Column("password", TypeName = "varchar(128)")]
    public string PasswordHashed { get; set; }

    [Required]
    [Column("email", TypeName = "varchar(320)")]
    public string Email { get; set; }

    [Required]
    [Column("role", TypeName = "varchar(50)")]
    public UserRole Role { get; set; }

    public ICollection<PlaylistDbModel> Playlists { get; set; } = [];
    public ICollection<ScoreDbModel> Scores { get; set; } = [];
}
