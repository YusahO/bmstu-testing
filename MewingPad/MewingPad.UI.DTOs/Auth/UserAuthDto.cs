using System.ComponentModel.DataAnnotations;

namespace MewingPad.DTOs.Auth;

public class UserAuthDto
{
    public UserDto? UserDto { get; set; }
    public string? Token { get; set; }
}