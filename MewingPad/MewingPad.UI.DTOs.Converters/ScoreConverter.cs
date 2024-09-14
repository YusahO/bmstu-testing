using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.DTOs.Converters;

public static class ScoreConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Score? DtoToCoreModel(ScoreDto? model)
    {
        return model is not null
               ? new(authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     value: model.Value)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static ScoreDto? CoreModelToDto(Score? model)
    {
        return model is not null
               ? new(authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     value: model.Value)
               : default;
    }
}