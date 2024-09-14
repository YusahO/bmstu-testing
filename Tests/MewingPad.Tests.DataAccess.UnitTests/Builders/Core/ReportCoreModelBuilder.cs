namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Common.Entities;
using MewingPad.Common.Enums;

public class ReportCoreModelBuilder
{
    private Report _report = new();

    public ReportCoreModelBuilder WithId(Guid id)
    {
        _report.Id = id;
        return this;
    }

    public ReportCoreModelBuilder WithAuthorId(Guid authorId)
    {
        _report.AuthorId = authorId;
        return this;
    }

    public ReportCoreModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _report.AudiotrackId = audiotrackId;
        return this;
    }

    public ReportCoreModelBuilder WithText(string text)
    {
        _report.Text = text;
        return this;
    }

    public ReportCoreModelBuilder WithStatus(ReportStatus status)
    {
        _report.Status = status;
        return this;
    }
    public Report Build()
    {
        return _report;
    }
}

