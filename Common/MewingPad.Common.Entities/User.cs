using MewingPad.Common.Enums;

namespace MewingPad.Common.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHashed { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    public User(Guid id, string username, string email, string passwordHashed, UserRole role = UserRole.Guest)
    {
        Id = id;
        // FavouritesId = favouritesId;
        Username = username;
        PasswordHashed = passwordHashed;
        Email = email;
        Role = role;
    }

    public User(User other)
    {
        Id = other.Id;
        // FavouritesId = other.FavouritesId;
        Username = other.Username;
        PasswordHashed = other.PasswordHashed;
        Email = other.Email;
        Role = other.Role;
    }

    public User()
    {
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not User)
        {
            return false;
        }

        User other = (User)obj;
        return Id == other.Id &&
            //    FavouritesId == other.FavouritesId &&
               Username == other.Username &&
               PasswordHashed == other.PasswordHashed &&
               Email == other.Email &&
               Role == other.Role;
    }

    public override int GetHashCode() => base.GetHashCode();
}
