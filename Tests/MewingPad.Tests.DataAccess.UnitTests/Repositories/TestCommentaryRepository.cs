using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestCommentaryRepository : IDisposable
{
    public DatabaseFixture Fixture { get; }
    public Guid AudiotrackId { get; } = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 1]);

    private readonly CommentaryRepository _repository;

    private void AddAudiotrack()
    {
        using var context = Fixture.CreateContext();
        context.Audiotracks.RemoveRange(context.Audiotracks);
        context.SaveChanges();

        var audiotrack = new AudiotrackDbModelBuilder()
            .WithId(AudiotrackId)
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();
        context.Audiotracks.Add(audiotrack);
        context.SaveChanges();
    }

    public TestCommentaryRepository(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new(Fixture.CreateContext());
        AddAudiotrack();
    }

    public void Dispose()
    {
        Fixture.Cleanup();
        AddAudiotrack();
    }

    [Fact]
    public async void TestAddCommentary_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithText("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .Build();

        // Act
        await _repository.AddCommentary(commentary);

        // Assert
        var actual = (from a in context.Commentaries select a).ToList();
        Assert.Single(actual);
    }

    [Fact]
    public async void TestAddCommentary_SameCommentaryError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithText("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
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
    public async void TestDeleteCommentary_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithText("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
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
    public async void TestDeleteCommentary_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteCommentary(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestUpdateCommentary_Ok()
    {
        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithText("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Commentaries.Add(
                new CommentaryDbModelBuilder()
                    .WithId(commentary.Id)
                    .WithText(commentary.Text)
                    .WithAuthorId(commentary.AuthorId)
                    .WithAudiotrackId(commentary.AudiotrackId)
                    .Build()
            );
            context.SaveChanges();
        }

        commentary.Text = "New";

        // Act
        await _repository.UpdateCommentary(commentary);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Commentaries select a).ToList();
            Assert.Single(actual);
            Assert.Equal(commentary.Id, actual[0].Id);
            Assert.Equal("New", actual[0].Text);
            Assert.Equal(commentary.AuthorId, actual[0].AuthorId);
            Assert.Equal(commentary.AudiotrackId, actual[0].AudiotrackId);
        }
    }

    [Fact]
    public async void TestUpdateCommentary_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var commentary = new CommentaryCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithText("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithAudiotrackId(AudiotrackId)
            .Build();

        // Act
        async Task Action() => await _repository.UpdateCommentary(commentary);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestGetAudiotrackCommentaries_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackCommentaries(AudiotrackId);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async void TestGetAudiotrackCommentaries_SomeOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (int i = 0; i < 3; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithText("Hello")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(AudiotrackId)
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetAudiotrackCommentaries(AudiotrackId);

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async void TestGetCommentaryById_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 2]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithText($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(AudiotrackId)
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetCommentaryById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal("Hello2", actual.Text);
        Assert.Equal(Fixture.DefaultUserId, actual.AuthorId);
        Assert.Equal(AudiotrackId, actual.AudiotrackId);
    }

    [Fact]
    public async void TestGetCommentaryById_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 5]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Commentaries.AddAsync(
                new CommentaryDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithText($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithAudiotrackId(AudiotrackId)
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
    public async void TestGetCommentaryById_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetCommentaryById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }
}
