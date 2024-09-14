namespace MewingPad.Common.Entities;

public class Commentary
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid AudiotrackId { get; set; }
    public string Text { get; set; }

    public Commentary(Guid id, Guid authorId, Guid audiotrackId, string text)
    {
        Id = id;
        AuthorId = authorId;
        AudiotrackId = audiotrackId;
        Text = text;
    }

    public Commentary(Commentary other)
    {
        Id = other.Id;
        AuthorId = other.AuthorId;
        AudiotrackId = other.AudiotrackId;
        Text = other.Text;
    }

    public Commentary()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Commentary)
        {
            return false;
        }

        Commentary other = (Commentary)obj;
        return other.Id == Id && 
               other.AuthorId == AuthorId &&
               other.AudiotrackId == AudiotrackId &&
               other.AuthorId == AuthorId &&
               other.Text == Text;
    }

    public override int GetHashCode() => base.GetHashCode();
}