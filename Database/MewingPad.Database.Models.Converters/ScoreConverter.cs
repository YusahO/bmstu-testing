using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class ScoreConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Score? DbToCoreModel(ScoreDbModel? model)
    {
        return model is not null
               ? new(audiotrackId: model.AudiotrackId,
                     authorId: model.AuthorId,
                     value: model.Value)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static ScoreDbModel? CoreToDbModel(Score? model)
    {
        return model is not null
               ? new(audiotrackId: model.AudiotrackId,
                     authorId: model.AuthorId,
                     value: model.Value)
               : default;
    }
}