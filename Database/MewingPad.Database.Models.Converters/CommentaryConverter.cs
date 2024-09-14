using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class CommentaryConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Commentary? DbToCoreModel(CommentaryDbModel? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     text: model.Text)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static CommentaryDbModel? CoreToDbModel(Commentary? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     text: model.Text)
               : default;
    }
}