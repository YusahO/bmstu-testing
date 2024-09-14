using System.Runtime.Serialization;

namespace MewingPad.Common.Enums;

public enum ReportStatus
{
    [EnumMember(Value = "Не рассмотрена")]
    NotViewed = 0,

    [EnumMember(Value = "Прочитано")]
    Viewed = 1,

    [EnumMember(Value = "Принята")]
    Accepted = 2,

    [EnumMember(Value = "Отклонена")]
    Declined = 3
}