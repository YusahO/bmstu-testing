using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class PlaylistConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Playlist? DbToCoreModel(PlaylistDbModel? model)
    {
        return model is not null
               ? new(id: model.Id,
                     title: model.Title,
                     userId: model.UserId)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static PlaylistDbModel? CoreToDbModel(Playlist? model)
    {
        return model is not null
               ? new(id: model.Id,
                     title: model.Title,
                     userId: model.UserId)
               : default;
    }
}