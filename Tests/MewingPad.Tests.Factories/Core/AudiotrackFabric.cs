using MewingPad.Common.Entities;

namespace MewingPad.Tests.Factories.Core;

public class AudiotrackFabric
{
    public static Audiotrack Create(
        Guid id,
        string title,
        Guid authorId,
        string filepath
    )
    {
        return new Audiotrack(id, title, authorId, filepath);
    }

    public static Audiotrack Copy(Audiotrack other)
    {
        return new Audiotrack(other);
    }

    public static Audiotrack CreateEmpty()
    {
        return new Audiotrack();
    }
}
