using MewingPad.Common.Exceptions;
using MewingPad.Database.Context;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Services.CommentaryService;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

[Collection("Test Database")]
public class CommentaryServiceIntegrationTest : BaseServiceTestClass
{
    private readonly MewingPadDbContext _context;
    private readonly CommentaryService _service;
    private readonly CommentaryRepository _commentaryRepository;
    private readonly AudiotrackRepository _audiotrackRepository;

    private readonly Guid DefaultCommentaryId = MakeGuid(1);

    private async Task AddDefaultCommentary()
    {
        await _context.Commentaries.AddAsync(
            new CommentaryDbModelBuilder()
                .WithId(DefaultCommentaryId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithAuthorId(DefaultUserId)
                .WithText("Text")
                .Build()
        );
        await _context.SaveChangesAsync();
    }

    public CommentaryServiceIntegrationTest(DatabaseFixture fixture)
        : base(fixture)
    {
        _context = Fixture.CreateContext();

        _commentaryRepository = new(_context);
        _audiotrackRepository = new(_context);

        _service = new(_commentaryRepository, _audiotrackRepository);
    }

    [Fact]
    public async Task CreateCommentary_CommentaryUnique_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedComment = new CommentaryCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithText("Text")
            .Build();

        // Act
        await _service.CreateCommentary(expectedComment);

        // Assert
        var actual = (from c in context.Commentaries select c).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedComment.Id, actual[0].Id);
    }

    [Fact]
    public async Task CreateCommentary_CommentaryExists_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var comment = new CommentaryCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithText("Text")
            .Build();

        await context.Commentaries.AddAsync(
            new CommentaryDbModelBuilder()
                .WithId(comment.Id)
                .WithAudiotrackId(comment.AudiotrackId)
                .WithAuthorId(comment.AuthorId)
                .WithText(comment.Text)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _service.CreateCommentary(comment);

        // Assert
        await Assert.ThrowsAsync<CommentaryExistsException>(Action);
    }

    [Fact]
    public async Task UpdateCommentary_CommentaryExists_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var comment = new CommentaryDbModelBuilder()
            .WithId(DefaultCommentaryId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithText("AAAA")
            .Build();

        context.Add(comment);
        context.SaveChanges();

        // Act
        var actual = await _service.UpdateCommentary(
            new CommentaryCoreModelBuilder()
                .WithId(DefaultCommentaryId)
                .WithAudiotrackId(DefaultAudiotrackId)
                .WithAuthorId(DefaultUserId)
                .WithText("Text")
                .Build()
        );

        // Assert
        Assert.Equal(comment.Id, actual.Id);
        Assert.Equal(comment.Text, actual.Text);
    }

    [Fact]
    public async Task UpdateCommentary_CommentaryNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        var expectedComment = new CommentaryCoreModelBuilder()
            .WithId(DefaultCommentaryId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithText("AAAA")
            .Build();

        // Act
        async Task Action() => await _service.UpdateCommentary(expectedComment);

        // Assert
        await Assert.ThrowsAsync<CommentaryNotFoundException>(Action);
    }

    [Fact]
    public async Task DeleteCommentary_CommentaryExists_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultCommentary();

        var expectedComment = new CommentaryCoreModelBuilder()
            .WithId(DefaultCommentaryId)
            .WithAudiotrackId(DefaultAudiotrackId)
            .WithAuthorId(DefaultUserId)
            .WithText("Text")
            .Build();

        // Act
        await _service.DeleteCommentary(DefaultCommentaryId);

        // Assert
        var actual = (from c in context.Commentaries select c).ToList();
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteCommentary_CommentaryNonexistent_Error()
    {
        // Arrange

        // Act
        async Task Action() =>
            await _service.DeleteCommentary(DefaultCommentaryId);

        // Assert
        await Assert.ThrowsAsync<CommentaryNotFoundException>(Action);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_CommentariesExist_ReturnsCommentaries()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();
        await AddDefaultCommentary();

        // Act
        var actual = await _service.GetAudiotrackCommentaries(
            DefaultAudiotrackId
        );

        // Assert
        Assert.Single(actual);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_NoCommentaries_ReturnsEmpty()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();
        await AddDefaultAudiotrack();

        // Act
        var actual = await _service.GetAudiotrackCommentaries(
            DefaultAudiotrackId
        );

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task GetAudiotrackCommentaries_NoAudiotrackWithId_Error()
    {
        // Arrange

        // Act
        async Task Action() => await _service.GetAudiotrackCommentaries(new());

        // Assert
        await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
    }
}
