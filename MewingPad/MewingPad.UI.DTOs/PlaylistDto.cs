using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class PlaylistDto(Guid id,
                         string title,
                         Guid userId)
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = id;

    [JsonProperty("title")]
    public string Title { get; set; } = title;

    [JsonProperty("userId")]
    public Guid UserId { get; set; } = userId;
}