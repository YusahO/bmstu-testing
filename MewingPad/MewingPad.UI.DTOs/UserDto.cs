using MewingPad.Common.Enums;
using Newtonsoft.Json;

namespace MewingPad.DTOs;

[JsonObject]
public class UserDto(Guid id,
                     Guid favouritesId,
                     string username,
                     string email,
                     string password,
                     UserRole role)
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = id;

    [JsonProperty("favouritesId")]
    public Guid FavouritesId { get; set; } = favouritesId;

    [JsonProperty("username")]
    public string Username { get; set; } = username;

    [JsonProperty("email")]
    public string Email { get; set; } = email;

    [JsonProperty("password")]
    public string Password { get; set; } = password;

    [JsonProperty("role")]
    public UserRole Role { get; set; } = role;
}
