using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class AudiotrackDto(Guid id,
                          string title,
                          Guid authorId,
                          string filepath,
                          float mean_score = 0.0f)
{
    [JsonProperty("audiotrackId")]
    public Guid Id { get; set; } = id;

    [JsonProperty("title")]
    public string Title { get; set; } = title;

    [JsonProperty("authorId")]
    public Guid AuthorId { get; set; } = authorId;

    [JsonProperty("filepath")]
    public string Filepath { get; set; } = filepath;

    [JsonProperty("meanScore")]
    public float MeanScore { get; set; } = mean_score;
}
