using MewingPad.Common.Entities;

namespace MewingPad.Tests.Factories.Core;

public class CommentaryFabric
{
    public static Commentary Create(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text
    )
    {
        return new Commentary(id, authorId, audiotrackId, text);
    }

    public static Commentary Copy(Commentary other)
    {
        return new Commentary(other);
    }

    public static Commentary CreateEmpty()
    {
        return new Commentary();
    }
}
