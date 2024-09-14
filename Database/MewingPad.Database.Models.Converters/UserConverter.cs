using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class UserConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static User? DbToCoreModel(UserDbModel? model)
    {
        return model is not null
               ? new(id: model.Id,
                    //  favouritesId: model.FavouritesId,
                     username: model.Username,
                     email: model.Email,
                     passwordHashed: model.PasswordHashed,
                     role: model.Role)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static UserDbModel? CoreToDbModel(User? model)
    {
        return model is not null
               ? new(id: model.Id,
                    //  favouritesId: model.FavouritesId,
                     username: model.Username,
                     email: model.Email,
                     passwordHashed: model.PasswordHashed,
                     role: model.Role)
               : default;
    }
}
