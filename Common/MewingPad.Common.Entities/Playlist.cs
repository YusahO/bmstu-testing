namespace MewingPad.Common.Entities;

public class Playlist
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid UserId { get; set; }

    public Playlist(Guid id, string title, Guid userId)
    {
        Id = id;
        Title = title;
        UserId = userId;
    }

    public Playlist(Playlist other)
    {
        Id = other.Id;
        Title = other.Title;
        UserId = other.UserId;
    }

    public Playlist()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Playlist)
        {
            return false;
        }

        Playlist other = (Playlist)obj;
        return other.Id == Id && 
               other.Title == Title &&
               other.UserId == UserId;
    }

    public override int GetHashCode() => base.GetHashCode();
}