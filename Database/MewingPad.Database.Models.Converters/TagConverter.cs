using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class TagConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Tag? DbToCoreModel(TagDbModel? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     name: model.Name)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static TagDbModel? CoreToDbModel(Tag? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     name: model.Name)
               : default;
    }
}