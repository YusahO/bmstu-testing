namespace MewingPad.Common.Entities;

public class Tag : BaseEntity
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Name { get; set; }

    public Tag(Guid id, Guid authorId, string name)
    {
        Id = id;
        AuthorId = authorId;
        Name = name;
    }

    public Tag(Tag other)
    {
        Id = other.Id;
        AuthorId = other.AuthorId;
        Name = other.Name;
    }

    public Tag()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Tag)
        {
            return false;
        }

        Tag other = (Tag)obj;
        return other.Id == Id &&
               other.AuthorId == AuthorId &&
               other.Name == Name;
    }

    public override int GetHashCode() => base.GetHashCode();
}