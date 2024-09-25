using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestAudiotrackRepository : BaseRepositoryTestClass
{
    private readonly AudiotrackRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestAudiotrackRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    [Fact]
    public async Task AddAudiotrack_AddUnique_Ok()
    {
        // Arrange
        List<AudiotrackDbModel> actual = [];
        var audiotrack = CreateAudiotrackCoreModel(
            MakeGuid(1),
            "Title",
            MakeGuid(1),
            "file.mp3"
        );
        var audiotrackDbo = CreateAudiotrackDboFromCore(audiotrack);

        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.AddAsync(It.IsAny<AudiotrackDbModel>(), default)
            )
            .Callback<AudiotrackDbModel, CancellationToken>(
                (a, token) => actual.Add(a)
            );

        // Act
        await _repository.AddAudiotrack(audiotrack);

        // Assert
        Assert.Single(actual);
        Assert.Equal(audiotrack.Id, actual[0].Id);
        Assert.Equal(audiotrack.Title, actual[0].Title);
        Assert.Equal(audiotrack.AuthorId, actual[0].AuthorId);
        Assert.Equal(audiotrack.Filepath, actual[0].Filepath);
    }

    [Fact]
    public async Task AddAudiotrack_AddAudiotrackWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.AddAsync(It.IsAny<AudiotrackDbModel>(), default)
            )
            .Callback<AudiotrackDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddAudiotrack(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task DeleteAudiotrack_DeleteExisting_Ok()
    {
        // Arrange
        Guid audiotrackId = MakeGuid(1);
        List<AudiotrackDbModel> audiotrackDbos =
        [
            CreateAudiotrackDbo(MakeGuid(1), "Title", MakeGuid(1), "file.mp3"),
        ];

        _mockFactory
            .MockAudiotracksDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(audiotrackDbos[0]);
        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.Remove(It.IsAny<AudiotrackDbModel>())
            )
            .Callback((AudiotrackDbModel a) => audiotrackDbos.Remove(a));

        // Act
        await _repository.DeleteAudiotrack(audiotrackId);

        // Assert
        Assert.Empty(audiotrackDbos);
    }

    [Fact]
    public async Task DeleteAudiotrack_DeleteNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockAudiotracksDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(AudiotrackDbModel)!);
        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.Remove(It.IsAny<AudiotrackDbModel>())
            )
            .Callback((AudiotrackDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.DeleteAudiotrack(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task UpdateAudiotrack_UpdateExisting_Ok()
    {
        // Arrange
        var expectedId = MakeGuid(1);
        Guid expectedAuthorId = MakeGuid(1);
        const string expectedFilepath = "file.mp3";
        const string expectedTitle = "New";

        var audiotrack = CreateAudiotrackCoreModel(
            expectedId,
            "Old",
            expectedAuthorId,
            expectedFilepath
        );
        var audiotrackDbo = CreateAudiotrackDboFromCore(audiotrack);
        List<AudiotrackDbModel> audiotrackDbos = [audiotrackDbo];

        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.Update(It.IsAny<AudiotrackDbModel>())
            )
            .Callback(
                (AudiotrackDbModel a) =>
                    audiotrackDbos[0].Title = new(expectedTitle)
            );

        // Act
        await _repository.UpdateAudiotrack(audiotrack);

        // Assert
        Assert.Single(audiotrackDbos);
        Assert.Equal(expectedId, audiotrackDbos[0].Id);
        Assert.Equal(expectedTitle, audiotrackDbos[0].Title);
        Assert.Equal(expectedAuthorId, audiotrackDbos[0].AuthorId);
        Assert.Equal(expectedFilepath, audiotrackDbos[0].Filepath);
    }

    [Fact]
    public async Task UpdateAudiotrack_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockAudiotracksDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(AudiotrackDbModel)!);
        _mockFactory
            .MockAudiotracksDbSet.Setup(s =>
                s.Update(It.IsAny<AudiotrackDbModel>())
            )
            .Callback((AudiotrackDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateAudiotrack(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetAllAudiotracks_GetTestData()
    {
        yield return new object[]
        {
            new List<AudiotrackDbModel>(),
            new List<Audiotrack>(),
        };
        yield return new object[]
        {
            new List<AudiotrackDbModel>(
                [
                    CreateAudiotrackDbo(
                        MakeGuid(1),
                        "Title",
                        MakeGuid(1),
                        "file.mp3"
                    ),
                ]
            ),
            new List<Audiotrack>(
                [
                    CreateAudiotrackCoreModel(
                        MakeGuid(1),
                        "Title",
                        MakeGuid(1),
                        "file.mp3"
                    ),
                ]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllAudiotracks_GetTestData))]
    public async Task GetAllAudiotracks_ReturnsFound(
        List<AudiotrackDbModel> audiotrackDbos,
        List<Audiotrack> expectedAudiotracks
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Audiotracks)
            .ReturnsDbSet(audiotrackDbos);

        // Act
        var actual = await _repository.GetAllAudiotracks();

        // Assert
        Assert.Equal(expectedAudiotracks, actual);
    }

    public static IEnumerable<object[]> GetAudiotrackById_GetTestData()
    {
        yield return new object[] { new AudiotrackDbModel(), new Audiotrack() };
        yield return new object[]
        {
            CreateAudiotrackDbo(MakeGuid(1), "Title", MakeGuid(1), "file.mp3"),
            CreateAudiotrackCoreModel(
                MakeGuid(1),
                "Title",
                MakeGuid(1),
                "file.mp3"
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotrackById_GetTestData))]
    public async Task GetAudiotrackById_ReturnsFound(
        AudiotrackDbModel returnedAudiotrackDbo,
        Audiotrack expectedAudiotrack
    )
    {
        // Arrange
        _mockFactory
            .MockAudiotracksDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnedAudiotrackDbo);

        // Act
        var actual = await _repository.GetAudiotrackById(expectedAudiotrack.Id);

        // Assert
        Assert.Equal(expectedAudiotrack, actual);
    }

    public static IEnumerable<object[]> GetAudiotracksByTitle_GetTestData()
    {
        yield return new object[]
        {
            new List<AudiotrackDbModel>(),
            new List<Audiotrack>(),
        };
        yield return new object[]
        {
            new List<AudiotrackDbModel>(
                [
                    CreateAudiotrackDbo(
                        MakeGuid(1),
                        "Title",
                        MakeGuid(1),
                        "file.mp3"
                    ),
                ]
            ),
            new List<Audiotrack>(
                [
                    CreateAudiotrackCoreModel(
                        MakeGuid(1),
                        "Title",
                        MakeGuid(1),
                        "file.mp3"
                    ),
                ]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotracksByTitle_GetTestData))]
    public async Task GetAudiotracksByTitle_ReturnsFound(
        List<AudiotrackDbModel> returnedAudiotrackDbos,
        List<Audiotrack> expectedAudiotracks
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Audiotracks)
            .ReturnsDbSet(returnedAudiotrackDbos);

        // Act
        var actual = await _repository.GetAudiotracksByTitle("Tit");

        // Assert
        Assert.Equal(expectedAudiotracks, actual);
    }
}
