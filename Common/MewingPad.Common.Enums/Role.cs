using System.Runtime.Serialization;

namespace MewingPad.Common.Enums;

public enum UserRole
{
    [EnumMember(Value = "Гость")]
    Guest = 0,

    [EnumMember(Value = "Пользователь")]
    User = 1,

    [EnumMember(Value = "Администратор")]
    Admin = 2
}