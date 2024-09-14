using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.DTOs.Converters;

public static class TagConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Tag? DtoToCoreModel(TagDto? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     name: model.Name)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static TagDto? CoreModelToDto(Tag? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     name: model.Name)
               : default;
    }
}