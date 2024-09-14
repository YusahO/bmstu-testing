namespace MewingPad.Common.Entities;

public class Audiotrack
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public Guid AuthorId { get; set; }
    public string Filepath { get; set; }

    public Audiotrack(Guid id, string title, Guid authorId, string filepath)
    {
        Id = id;
        Title = title;
        AuthorId = authorId;
        Filepath = filepath;
    }

    public Audiotrack(Audiotrack other)
    {
        Id = other.Id;
        Title = other.Title;
        AuthorId = other.AuthorId;
        Filepath = other.Filepath;
    }

    public Audiotrack()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Audiotrack)
        {
            return false;
        }

        Audiotrack other = (Audiotrack)obj;
        return other.Id == Id && 
               other.Title == Title &&
               other.AuthorId == AuthorId &&
               other.Filepath == Filepath;
    }

    public override int GetHashCode() => base.GetHashCode();
}