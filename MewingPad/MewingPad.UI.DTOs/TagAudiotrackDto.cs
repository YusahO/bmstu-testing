using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class TagAudiotrackDto(Guid audiotrackId, Guid tagId)
{
	[JsonProperty("audiotrackId")]
	public Guid AudiotrackId { get; set; } = audiotrackId;

	[JsonProperty("tagId")]
	public Guid TagId { get; set; } = tagId;
}