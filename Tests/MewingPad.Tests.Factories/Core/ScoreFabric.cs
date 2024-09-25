using System.Diagnostics;
using MewingPad.Common.Entities;

namespace MewingPad.Tests.Factories.Core;

public class ScoreFabric
{
    public static Score Create(Guid id, Guid authorId, int value)
    {
        Trace.Assert(0 <= value && value <= 5, "Value must be in range [0; 5]");
        return new Score(id, authorId, value);
    }

    public static Score Copy(Score other)
    {
        return new Score(other);
    }

    public static Score CreateEmpty()
    {
        return new Score();
    }
}
