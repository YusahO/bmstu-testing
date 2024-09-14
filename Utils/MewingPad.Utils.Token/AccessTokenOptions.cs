using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MewingPad.Utils.Token;

public static class AccessTokenOptions
{
    public const string ISSUER = "http://localhost:9898";
    public const string AUDIENCE = "http://localhost:3000";
    private const string SECRET = "my-32-character-ultra-secure-and-ultra-long-secret";
    public const int LIFETIME = 30 * 60 * 60;

    public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.ASCII.GetBytes(SECRET));
}