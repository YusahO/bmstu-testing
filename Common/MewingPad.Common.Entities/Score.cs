using MewingPad.Common.Exceptions;

namespace MewingPad.Common.Entities;

public class Score
{
    private int _value;
    public Guid AudiotrackId { get; set; }
    public Guid AuthorId { get; set; }
    public int Value
    {
        get => _value;
        set =>
            _value =
                (0 <= value && value <= 5)
                    ? value
                    : throw new ScoreInvalidValueException(value);
    }

    public Score(Guid audiotrackId, Guid authorId, int value)
    {
        AudiotrackId = audiotrackId;
        AuthorId = authorId;
        Value = value;
    }

    public Score(Score other)
    {
        AudiotrackId = other.AudiotrackId;
        AuthorId = other.AuthorId;
        Value = other.Value;
    }

    public Score() { }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Score)
        {
            return false;
        }

        Score other = (Score)obj;
        return other.AuthorId == AuthorId
            && other.AudiotrackId == AudiotrackId
            && other.Value == Value;
    }

    public override int GetHashCode() => base.GetHashCode();
}
