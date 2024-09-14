using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.DTOs.Converters;

public static class PlaylistConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Playlist? DtoToCoreModel(PlaylistDto? model)
    {
        return model is not null
               ? new(id: model.Id,
                     title: model.Title,
                     userId: model.UserId)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static PlaylistDto? CoreModelToDto(Playlist? model)
    {
        return model is not null
               ? new(id: model.Id,
                     title: model.Title,
                     userId: model.UserId)
               : default;
    }
}