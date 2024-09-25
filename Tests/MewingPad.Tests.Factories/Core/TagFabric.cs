using MewingPad.Common.Entities;

namespace MewingPad.Tests.Factories.Core;

public class TagFabric
{
    public static Tag Create(Guid id, Guid authorId, string name)
    {
        return new Tag(id, authorId, name);
    }

    public static Tag Copy(Tag other)
    {
        return new Tag(other);
    }

    public static Tag CreateEmpty()
    {
        return new Tag();
    }
}
