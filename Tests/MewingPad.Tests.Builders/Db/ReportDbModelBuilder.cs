using MewingPad.Common.Enums;
using MewingPad.Database.Models;

namespace MewingPad.Tests.Builders.Db;

public class ReportDbModelBuilder
{
    private ReportDbModel _reportDbo = new();

    public ReportDbModelBuilder WithId(Guid id)
    {
        _reportDbo.Id = id;
        return this;
    }

    public ReportDbModelBuilder WithAuthorId(Guid authorId)
    {
        _reportDbo.AuthorId = authorId;
        return this;
    }

    public ReportDbModelBuilder WithAudiotrackId(Guid audiotrackId)
    {
        _reportDbo.AudiotrackId = audiotrackId;
        return this;
    }

    public ReportDbModelBuilder WithText(string text)
    {
        _reportDbo.Text = text;
        return this;
    }

    public ReportDbModelBuilder WithStatus(ReportStatus status)
    {
        _reportDbo.Status = status;
        return this;
    }

    public ReportDbModel Build()
    {
        return _reportDbo;
    }
}
