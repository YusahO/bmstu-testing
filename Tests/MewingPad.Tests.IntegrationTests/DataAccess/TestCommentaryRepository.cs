using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestCommentaryRepository : BaseRepositoryTestClass
{
    private readonly CommentaryRepository _repository;

    public TestCommentaryRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddCommentary_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);

        var commentary = new CommentaryCoreModelBuilder()
            .WithId(expectedId)
            .WithText("Text")
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .Build();

        // Act
        await _repository.AddCommentary(commentary);

        // Assert
        var actual = (from a in context.Commentaries select a).ToList();
        Assert.Single(actual);
    }

    [Fact]
    public async void AddCommentary_AddCommentaryWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);

        var commentary = new CommentaryCoreModelBuilder()
            .WithId(expectedId)
            .WithText("Text")
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .Build();
        await context.Commentaries.AddAsync(
            new CommentaryDbModelBuilder()
                .WithId(commentary.Id)
                .WithText(commentary.Text)
                .WithAuthorId(commentary.AuthorId)
                .WithAudiotrackId(commentary.AudiotrackId)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddCommentary(commentary);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeleteCommentary_DeleteExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);

        var commentary = new CommentaryCoreModelBuilder()
            .WithId(expectedId)
            .WithText("Text")
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .Build();
        await context.Commentaries.AddAsync(
            new CommentaryDbModelBuilder()
                .WithId(commentary.Id)
                .WithText(commentary.Text)
                .WithAuthorId(commentary.AuthorId)
                .WithAudiotrackId(commentary.AudiotrackId)
                .Build()
        );
        context.SaveChanges();

        // Act
        await _repository.DeleteCommentary(commentary.Id);

        // Assert
        Assert.Empty((from a in context.Commentaries select a).ToList());
    }

    [Fact]
    public async void DeleteCommentary_DeleteNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteCommentary(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateCommentary_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);
        Guid expectedAuthorId = DefaultUserId;
        Guid expectedAudiotrackId = DefaultAudiotrackId;
        const string expectedText = "New";

        var commentary = new CommentaryCoreModelBuilder()
            .WithId(expectedId)
            .WithText("Text")
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .Build();
        using (var context = Fixture.CreateContext())
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(commentary.Id)
                    .WithText(commentary.Text)
                    .WithAuthorId(commentary.AuthorId)
                    .WithAudiotrackId(commentary.AudiotrackId)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        commentary.Text = expectedText;

        // Act
        await _repository.UpdateCommentary(commentary);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Commentaries select a).ToList();
            Assert.Single(actual);
            Assert.Equal(expectedId, actual[0].Id);
            Assert.Equal(expectedText, actual[0].Text);
            Assert.Equal(expectedAuthorId, actual[0].AuthorId);
            Assert.Equal(expectedAudiotrackId, actual[0].AudiotrackId);
        }
    }

    [Fact]
    public async void UpdateCommentary_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);

        var commentary = new CommentaryCoreModelBuilder()
            .WithId(expectedId)
            .WithText("Text")
            .WithAuthorId(DefaultUserId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .Build();

        // Act
        async Task Action() => await _repository.UpdateCommentary(commentary);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetAudiotrackCommentaries_NoCommentaries_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackCommentaries(
            DefaultAudiotrackId
        );

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async void GetAudiotrackCommentaries_CommetariesExist_ReturnsCommentaries()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(1);
        Guid expectedAudiotrackId = DefaultAudiotrackId;

        for (byte i = 1; i < 4; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithText("Text")
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackCommentaries(
            DefaultAudiotrackId
        );

        // Assert
        Assert.Equal(3, actual.Count);
        Assert.All(
            actual,
            commentary =>
                Assert.Equal(expectedAudiotrackId, commentary.AudiotrackId)
        );
    }

    [Fact]
    public async void GetCommentaryById_CommentariesExist_ReturnsCommentary()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(3);
        var expectedAuthorId = DefaultUserId;
        var expectedAudiotrackId = DefaultAudiotrackId;
        const string expectedText = "Text3";

        for (byte i = 1; i < 4; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithText($"Text{i}")
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetCommentaryById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedText, actual.Text);
        Assert.Equal(expectedAuthorId, actual.AuthorId);
        Assert.Equal(expectedAudiotrackId, actual.AudiotrackId);
    }

    [Fact]
    public async void GetCommentaryById_NoCommentariesWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedId = MakeGuid(5);

        for (byte i = 1; i < 4; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithText($"Text{i}")
                    .WithAuthorId(DefaultUserId)
                    .WithAudiotrackId(DefaultAudiotrackId)
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetCommentaryById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetCommentaryById_NoCommentaries_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetCommentaryById(new Guid());

        // Assert
        Assert.Null(actual);
    }
}
