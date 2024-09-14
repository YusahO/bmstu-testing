using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class CommentaryDto(Guid id,
                           Guid authorId,
                           string authorName,
                           Guid audiotrackId,
                           string text)
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = id;

    [JsonProperty("audiotrackId")]
    public Guid AudiotrackId { get; set; } = audiotrackId;

    [JsonProperty("authorId")]
    public Guid AuthorId { get; set; } = authorId;

    [JsonProperty("authorName")]
    public string AuthorName { get; set; } = authorName;

    [JsonProperty("text")]
    public string Text { get; set; } = text;
}
