using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class ScoreDto(Guid audiotrackId,
                      Guid authorId,
                      int value)
{
    [JsonProperty("audiotrackId")]
    public Guid AudiotrackId { get; set; } = audiotrackId;

    [JsonProperty("authorId")]
    public Guid AuthorId { get; set; } = authorId;

    [JsonProperty("value")]
    public int Value { get; set; } = value;
}
