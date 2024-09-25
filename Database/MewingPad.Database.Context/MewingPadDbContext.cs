using MewingPad.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Database.Context;

public class MewingPadDbContext : DbContext
{
    public virtual DbSet<UserDbModel> Users { get; set; }
    public virtual DbSet<PlaylistDbModel> Playlists { get; set; }
    public virtual DbSet<AudiotrackDbModel> Audiotracks { get; set; }
    public virtual DbSet<CommentaryDbModel> Commentaries { get; set; }
    public virtual DbSet<ScoreDbModel> Scores { get; set; }
    public virtual DbSet<ReportDbModel> Reports { get; set; }
    public virtual DbSet<TagDbModel> Tags { get; set; }

    public virtual DbSet<PlaylistAudiotrackDbModel> PlaylistsAudiotracks { get; set; }
    public virtual DbSet<TagAudiotrackDbModel> TagsAudiotracks { get; set; }
    public virtual DbSet<UserFavouriteDbModel> UsersFavourites { get; set; }

    public MewingPadDbContext() { }

    public MewingPadDbContext(DbContextOptions<MewingPadDbContext> options)
        : base(options) { }

    protected MewingPadDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<ScoreDbModel>()
            .HasKey(u => new { u.AuthorId, u.AudiotrackId });
        modelBuilder
            .Entity<UserFavouriteDbModel>()
            .HasKey(uf => new { uf.UserId, uf.FavouriteId });

        modelBuilder
            .Entity<UserFavouriteDbModel>()
            .HasOne(uf => uf.User)
            .WithMany()
            .HasForeignKey(uf => uf.UserId);

        modelBuilder
            .Entity<UserFavouriteDbModel>()
            .HasOne(uf => uf.Playlist)
            .WithMany()
            .HasForeignKey(uf => uf.FavouriteId);

        modelBuilder
            .Entity<AudiotrackDbModel>()
            .HasMany(e => e.Playlists)
            .WithMany(e => e.Audiotracks)
            .UsingEntity<PlaylistAudiotrackDbModel>(
                l =>
                    l.HasOne(e => e.Playlist)
                        .WithMany(e => e.PlaylistsAudiotracks),
                r =>
                    r.HasOne(e => e.Audiotrack)
                        .WithMany(e => e.PlaylistsAudiotracks)
            );

        modelBuilder
            .Entity<AudiotrackDbModel>()
            .HasMany(e => e.Tags)
            .WithMany(e => e.Audiotracks)
            .UsingEntity<TagAudiotrackDbModel>(
                l => l.HasOne(e => e.Tag).WithMany(e => e.TagsAudiotracks),
                r =>
                    r.HasOne(e => e.Audiotrack).WithMany(e => e.TagsAudiotracks)
            );

        modelBuilder
            .Entity<UserDbModel>()
            .HasMany(u => u.Playlists)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        modelBuilder
            .Entity<UserDbModel>()
            .HasMany(u => u.Scores)
            .WithOne(s => s.Author)
            .HasForeignKey(s => s.AuthorId);

        base.OnModelCreating(modelBuilder);
    }
}
