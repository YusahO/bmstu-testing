using MewingPad.Tests.Builders.Core;

namespace MewingPad.Tests.UnitTests.BusinessLogic.Services;

public class BaseServiceTestClass
{
    protected static Guid MakeGuid(byte i) =>
        new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]);

    protected static Audiotrack CreateAudiotrack(Guid audiotrackId, Guid authorId)
    {
        return new AudiotrackCoreModelBuilder()
            .WithId(audiotrackId)
            .WithTitle($"Title_{audiotrackId}")
            .WithAuthorId(authorId)
            .WithFilepath("/path/to/file")
            .Build();
    }
}
