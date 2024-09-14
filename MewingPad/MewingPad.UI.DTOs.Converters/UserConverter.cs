using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.DTOs.Converters;

public static class UserConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static User? DtoToCoreModel(UserDto? model)
    {
        return model is not null
               ? new(id: model.Id,
                    //  favouritesId: model.FavouritesId,
                     username: model.Username,
                     email: model.Email,
                     passwordHashed: model.Password,
                     role: model.Role)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static UserDto? CoreModelToDto(User? model)
    {
        return model is not null
               ? new(id: model.Id,
                     favouritesId: Guid.Empty,
                     username: model.Username,
                     email: model.Email,
                     password: model.PasswordHashed,
                     role: model.Role)
               : default;
    }
}
