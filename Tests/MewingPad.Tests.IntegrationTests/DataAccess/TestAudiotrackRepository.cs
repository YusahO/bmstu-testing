using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;

namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestAudiotrackRepository : BaseRepositoryTestClass
{
    private readonly AudiotrackRepository _repository;

    public TestAudiotrackRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async Task AddAudiotrack_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();

        // Act
        await _repository.AddAudiotrack(audiotrack);

        // Assert
        var actual = (from a in context.Audiotracks select a).ToList();
        Assert.Single(actual);
        Assert.Equal(audiotrack.Id, actual[0].Id);
        Assert.Equal(audiotrack.Title, actual[0].Title);
        Assert.Equal(audiotrack.AuthorId, actual[0].AuthorId);
        Assert.Equal(audiotrack.Filepath, actual[0].Filepath);
    }

    [Fact]
    public async Task AddAudiotrack_AddToExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        await context.Audiotracks.AddAsync(
            new AudiotrackDbModelBuilder()
                .WithId(Guid.NewGuid())
                .WithTitle($"HelloAnother")
                .WithAuthorId(DefaultUserId)
                .WithFilepath($"/path/to/file_another")
                .Build()
        );
        await context.SaveChangesAsync();

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("New")
            .WithAuthorId(DefaultUserId)
            .WithFilepath("/path/to/new")
            .Build();

        // Act
        await _repository.AddAudiotrack(audiotrack);

        // Assert
        var actual = (from a in context.Audiotracks select a).ToList();
        Assert.Equal(2, actual.Count);
    }

    [Fact]
    public async Task AddAudiotrack_AddAudiotrackWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();
        await context.Audiotracks.AddAsync(
            new AudiotrackDbModelBuilder()
                .WithId(audiotrack.Id)
                .WithTitle(audiotrack.Title)
                .WithAuthorId(audiotrack.AuthorId)
                .WithFilepath(audiotrack.Filepath)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddAudiotrack(audiotrack);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task DeleteAudiotrack_DeleteExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var audiotrackId = MakeGuid(1);

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(audiotrackId)
            .WithTitle("Hello")
            .WithAuthorId(DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();
        await context.Audiotracks.AddAsync(
            new AudiotrackDbModelBuilder()
                .WithId(audiotrack.Id)
                .WithTitle(audiotrack.Title)
                .WithAuthorId(audiotrack.AuthorId)
                .WithFilepath(audiotrack.Filepath)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteAudiotrack(audiotrackId);

        // Assert
        Assert.Empty((from a in context.Audiotracks select a).ToList());
    }

    [Fact]
    public async Task DeleteAudiotrack_DeleteNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteAudiotrack(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task UpdateAudiotrack_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(1);
        Guid expectedAuthorId = DefaultUserId;
        const string expectedFilepath = "/path/to/file";
        const string expectedTitle = "New";

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(expectedId)
            .WithTitle("Hello")
            .WithAuthorId(DefaultUserId)
            .WithFilepath(expectedFilepath)
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Audiotracks.Add(
                new AudiotrackDbModelBuilder()
                    .WithId(expectedId)
                    .WithTitle(audiotrack.Title)
                    .WithAuthorId(audiotrack.AuthorId)
                    .WithFilepath(audiotrack.Filepath)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        audiotrack.Title = expectedTitle;

        // Act
        await _repository.UpdateAudiotrack(audiotrack);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Audiotracks select a).ToList();
            Assert.Single(actual);
            Assert.Equal(expectedId, actual[0].Id);
            Assert.Equal(expectedTitle, actual[0].Title);
            Assert.Equal(expectedAuthorId, actual[0].AuthorId);
            Assert.Equal(expectedFilepath, actual[0].Filepath);
        }
    }

    [Fact]
    public async Task UpdateAudiotrack_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(new Guid())
            .WithTitle("Hello")
            .WithAuthorId(new Guid())
            .WithFilepath("/path/to/file")
            .Build();

        // Act
        async Task Action() => await _repository.UpdateAudiotrack(audiotrack);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task GetAllAudiotracks_NoAudiotracks_ReturnsEmpty()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetAllAudiotracks();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAllAudiotracks_AudiotracksExist_ReturnsAudiotracks()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(DefaultUserId)
                    .WithFilepath($"/path/to/file{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllAudiotracks();

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async Task GetAudiotrackById_AudiotrackExists_ReturnsAudiotrack()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(2);
        const string expectedTitle = "Hello2";
        const string expectedFilepath = "/path/to/file2";

        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(DefaultUserId)
                    .WithFilepath($"/path/to/file{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedTitle, actual.Title);
        Assert.Equal(DefaultUserId, actual.AuthorId);
        Assert.Equal(expectedFilepath, actual.Filepath);
    }

    [Fact]
    public async Task GetAudiotrackById_NoSuchAudiotrack_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(2);

        await context.Audiotracks.AddAsync(
            new AudiotrackDbModelBuilder()
                .WithId(MakeGuid(1))
                .WithTitle($"Hello")
                .WithAuthorId(DefaultUserId)
                .WithFilepath($"/path/to/file")
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetAudiotrackById_NoAudiotracks_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_OneAudiotrackWithTitle_ReturnsAudiotrack()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(1);
        const string expectedTitle = "Hello";
        const string expectedFilepath = "/path/to/file";

        await context.Audiotracks.AddAsync(
            new AudiotrackDbModelBuilder()
                .WithId(expectedId)
                .WithTitle($"Hello")
                .WithAuthorId(DefaultUserId)
                .WithFilepath($"/path/to/file")
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksByTitle(expectedTitle);

        // Assert
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedTitle, actual[0].Title);
        Assert.Equal(DefaultUserId, actual[0].AuthorId);
        Assert.Equal(expectedFilepath, actual[0].Filepath);
    }

    [Fact]
    public async Task GetAudiotracksByTitle_SomeAudiotracksWithTitle_ReturnsAudiotracks()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        const string expectedTitle = "Hello";
        const int expectedCount = 3;

        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithTitle($"Hello")
                    .WithAuthorId(DefaultUserId)
                    .WithFilepath($"/path/to/file")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksByTitle(expectedTitle);

        // Assert
        Assert.Equal(expectedCount, actual.Count);
        Assert.All(actual, (audio) => Assert.Equal(expectedTitle, audio.Title));
    }

    [Fact]
    public async Task GetAudiotracksByTitle_NoAudiotracksWithTitle_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        const string expectedTitle = "aaaa";

        using (var context = Fixture.CreateContext())
        {
            for (byte i = 1; i < 4; ++i)
            {
                await context.Audiotracks.AddAsync(
                    new AudiotrackDbModelBuilder()
                        .WithId(MakeGuid(i))
                        .WithTitle($"Hello{i}")
                        .WithAuthorId(DefaultUserId)
                        .WithFilepath($"/path/to/file{i}")
                        .Build()
                );
            }
            await context.SaveChangesAsync();
        }

        // Act
        var actual = await _repository.GetAudiotracksByTitle(expectedTitle);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task TestGetAudiotrackByTitle_EmptyOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }
}
