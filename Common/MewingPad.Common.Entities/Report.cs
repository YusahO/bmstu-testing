using MewingPad.Common.Enums;
namespace MewingPad.Common.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid AudiotrackId { get; set; }
    public string Text { get; set; }
    public ReportStatus Status { get; set; }

    public Report(Guid id, Guid authorId, Guid audiotrackId, string text, ReportStatus status = ReportStatus.NotViewed)
    {
        Id = id;
        AuthorId = authorId;
        AudiotrackId = audiotrackId;
        Text = text;
        Status = status;
    }

    public Report(Report other)
    {
        Id = other.Id;
        AuthorId = other.AuthorId;
        AudiotrackId = other.AudiotrackId;
        Text = other.Text;
        Status = other.Status;
    }

    public Report()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Report)
        {
            return false;
        }

        Report other = (Report)obj;
        return other.Id == Id && 
               other.AuthorId == AuthorId &&
               other.AudiotrackId == AudiotrackId &&
               other.Text == Text &&
               other.Status == Status;
    }

    public override int GetHashCode() => base.GetHashCode();
}