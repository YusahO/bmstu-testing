using System.Diagnostics.CodeAnalysis;
using MewingPad.Common.Entities;

namespace MewingPad.Database.Models.Converters;

public static class ReportConverter
{
    [return: NotNullIfNotNull(nameof(model))]
    public static Report? DbToCoreModel(ReportDbModel? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     text: model.Text,
                     status: model.Status)
               : default;
    }

    [return: NotNullIfNotNull(nameof(model))]
    public static ReportDbModel? CoreToDbModel(Report? model)
    {
        return model is not null
               ? new(id: model.Id,
                     authorId: model.AuthorId,
                     audiotrackId: model.AudiotrackId,
                     text: model.Text,
                     status: model.Status)
               : default;
    }
}