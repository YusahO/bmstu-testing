using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class TagDto(Guid id,
                    Guid authorId,
                    string name)
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = id;

    [JsonProperty("authorId")]
    public Guid AuthorId { get; set; } = authorId;

    [JsonProperty("name")]
    public string Name { get; set; } = name;
}
