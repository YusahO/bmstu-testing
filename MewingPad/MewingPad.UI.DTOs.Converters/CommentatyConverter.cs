using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.DTOs.Converters;

public static class CommentaryConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Commentary? DtoToCoreModel(CommentaryDto? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     text: model.Text)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static CommentaryDto? CoreModelToDto(Commentary? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     authorName: "",
                     audiotrackId: model.AudiotrackId,
                     text: model.Text)
               : default;
    }
}