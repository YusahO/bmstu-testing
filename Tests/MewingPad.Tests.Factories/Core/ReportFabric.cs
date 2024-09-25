using MewingPad.Common.Entities;
using MewingPad.Common.Enums;

namespace MewingPad.Tests.Factories.Core;

public class ReportFabric
{
    public static Report Create(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text,
        ReportStatus reportStatus
    )
    {
        return new Report(id, authorId, audiotrackId, text, reportStatus);
    }

    public static Report Copy(Report other)
    {
        return new Report(other);
    }

    public static Report CreateEmpty()
    {
        return new Report();
    }
}
