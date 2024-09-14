using MewingPad.Common.Enums;
using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class ReportDto(Guid id,
                       Guid authorId,
                       Guid audiotrackId,
                       string text,
                       ReportStatus status)
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = id;

    [JsonProperty("authorId")]
    public Guid AuthorId { get; set; } = authorId;

    [JsonProperty("audiotrackId")]
    public Guid AudiotrackId { get; set; } = audiotrackId;

    [JsonProperty("text")]
    public string Text { get; set; } = text;

    [JsonProperty("status")]
    public ReportStatus Status { get; set; } = status;
}
