using Microsoft.EntityFrameworkCore;
using MewingPad.Database.Models;

namespace MewingPad.Database.Context;

public class MewingPadDbContext : DbContext
{
    public DbSet<UserDbModel> Users { get; set; }
    public DbSet<PlaylistDbModel> Playlists { get; set; }
    public DbSet<AudiotrackDbModel> Audiotracks { get; set; }
    public DbSet<CommentaryDbModel> Commentaries { get; set; }
    public DbSet<ScoreDbModel> Scores { get; set; }
    public DbSet<ReportDbModel> Reports { get; set; }
    public DbSet<TagDbModel> Tags { get; set; }

    public DbSet<PlaylistAudiotrackDbModel> PlaylistsAudiotracks { get; set; }
    public DbSet<TagAudiotrackDbModel> TagsAudiotracks { get; set; }
    public DbSet<UserFavouriteDbModel> UsersFavourites { get; set; }

    public MewingPadDbContext(DbContextOptions<MewingPadDbContext> options) : base(options) { }
    protected MewingPadDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScoreDbModel>().HasKey(u => new { u.AuthorId, u.AudiotrackId });
        modelBuilder.Entity<UserFavouriteDbModel>().HasKey(uf => new { uf.UserId, uf.FavouriteId });

        modelBuilder.Entity<UserFavouriteDbModel>()
            .HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId);

        modelBuilder.Entity<UserFavouriteDbModel>()
            .HasOne(uf => uf.Playlist)
            .WithMany()
            .HasForeignKey(uf => uf.FavouriteId);

        modelBuilder.Entity<AudiotrackDbModel>()
            .HasMany(e => e.Playlists)
            .WithMany(e => e.Audiotracks)
            .UsingEntity<PlaylistAudiotrackDbModel>(
                l => l.HasOne(e => e.Playlist).WithMany(e => e.PlaylistsAudiotracks),
                r => r.HasOne(e => e.Audiotrack).WithMany(e => e.PlaylistsAudiotracks)
            );

        modelBuilder.Entity<AudiotrackDbModel>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Audiotracks)
            .UsingEntity<TagAudiotrackDbModel>(
                l => l.HasOne(e => e.Tag).WithMany(e => e.TagsAudiotracks),
                r => r.HasOne(e => e.Audiotrack).WithMany(e => e.TagsAudiotracks)
            );

        modelBuilder.Entity<UserDbModel>()
            .HasMany(u => u.Playlists)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<UserDbModel>()
            .HasMany(u => u.Scores)
            .WithOne(s => s.Author)
            .HasForeignKey(s => s.AuthorId);

        base.OnModelCreating(modelBuilder);
    }
}
