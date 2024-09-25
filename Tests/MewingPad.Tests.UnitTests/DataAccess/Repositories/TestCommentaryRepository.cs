using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestCommentaryRepository : BaseRepositoryTestClass
{
    private readonly CommentaryRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestCommentaryRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    private static Commentary CreateCommentaryCoreModel(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text
    )
    {
        return new CommentaryCoreModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithText(text)
            .Build();
    }

    private static CommentaryDbModel CreateCommentaryDbo(
        Guid id,
        Guid authorId,
        Guid audiotrackId,
        string text
    )
    {
        return new CommentaryDbModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithAudiotrackId(audiotrackId)
            .WithText(text)
            .Build();
    }

    private static CommentaryDbModel CreateCommentaryDboFromCore(
        Commentary commentary
    )
    {
        return CreateCommentaryDbo(
            commentary.Id,
            commentary.AuthorId,
            commentary.AudiotrackId,
            commentary.Text
        );
    }

    [Fact]
    public async void AddCommentary_AddUnique_Ok()
    {
        // Arrange
        List<CommentaryDbModel> actual = [];
        var commentary = CreateCommentaryCoreModel(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "Text"
        );
        var commentaryDbo = CreateCommentaryDboFromCore(commentary);

        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.AddAsync(It.IsAny<CommentaryDbModel>(), default)
            )
            .Callback<CommentaryDbModel, CancellationToken>(
                (u, token) => actual.Add(u)
            );

        // Act
        await _repository.AddCommentary(commentary);

        // Assert
        Assert.Single(actual);
        Assert.Equal(commentary.Id, actual[0].Id);
        Assert.Equal(commentary.Text, actual[0].Text);
        Assert.Equal(commentary.AuthorId, actual[0].AuthorId);
        Assert.Equal(commentary.AudiotrackId, actual[0].AudiotrackId);
    }

    [Fact]
    public async void AddCommentary_AddCommentaryWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.AddAsync(It.IsAny<CommentaryDbModel>(), default)
            )
            .Callback<CommentaryDbModel, CancellationToken>(
                (a, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddCommentary(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeleteCommentary_DeleteExisting_Ok()
    {
        // Arrange
        Guid expectedId = MakeGuid(1);
        var commentary = CreateCommentaryCoreModel(
            MakeGuid(1),
            MakeGuid(1),
            MakeGuid(1),
            "text"
        );
        var commentaryDbo = CreateCommentaryDboFromCore(commentary);
        List<CommentaryDbModel> commentaryDbos = [commentaryDbo];

        _mockFactory
            .MockCommentariesDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(commentaryDbo);
        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.Remove(It.IsAny<CommentaryDbModel>())
            )
            .Callback((CommentaryDbModel c) => commentaryDbos.Remove(c));

        // Act
        await _repository.DeleteCommentary(expectedId);

        // Assert
        Assert.Empty(commentaryDbos);
    }

    [Fact]
    public async void DeleteCommentary_DeleteNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockCommentariesDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(CommentaryDbModel)!);
        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.Remove(It.IsAny<CommentaryDbModel>())
            )
            .Callback((CommentaryDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.DeleteCommentary(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateCommentary_UpdateExisting_Ok()
    {
        // Arrange
        var expectedId = MakeGuid(1);
        Guid expectedAuthorId = MakeGuid(1);
        Guid expectedAudiotrackId = MakeGuid(1);
        const string expectedText = "text";

        var commentary = CreateCommentaryCoreModel(
            expectedId,
            expectedAuthorId,
            expectedAudiotrackId,
            expectedText
        );
        var commentaryDbo = CreateCommentaryDboFromCore(commentary);
        List<CommentaryDbModel> commentaryDbos = [commentaryDbo];

        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.Update(It.IsAny<CommentaryDbModel>())
            )
            .Callback(
                (CommentaryDbModel c) =>
                    commentaryDbos[0].Text = new(expectedText)
            );

        // Act
        await _repository.UpdateCommentary(commentary);

        // Assert
        Assert.Single(commentaryDbos);
        Assert.Equal(commentary.Id, commentaryDbos[0].Id);
        Assert.Equal(expectedText, commentaryDbos[0].Text);
        Assert.Equal(commentary.AuthorId, commentaryDbos[0].AuthorId);
        Assert.Equal(commentary.AudiotrackId, commentaryDbos[0].AudiotrackId);
    }

    [Fact]
    public async void UpdateCommentary_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockCommentariesDbSet.Setup(s =>
                s.Update(It.IsAny<CommentaryDbModel>())
            )
            .Callback((CommentaryDbModel c) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateCommentary(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetAudiotrackCommentaries_GetTestData()
    {
        yield return new object[]
        {
            new List<CommentaryDbModel>(),
            new List<Commentary>(),
        };
    }

    [Theory]
    [MemberData(nameof(GetAudiotrackCommentaries_GetTestData))]
    public async void GetAudiotrackCommentaries_ReturnsFound(
        List<CommentaryDbModel> commentaryDbos,
        List<Commentary> expectedCommentraries
    )
    {
        // Arrange
        _mockFactory
            .MockContext.Setup(x => x.Commentaries)
            .ReturnsDbSet(commentaryDbos);

        // Act
        var actual = await _repository.GetAudiotrackCommentaries(MakeGuid(1));

        // Assert
        Assert.Equal(expectedCommentraries, actual);
    }

    public static IEnumerable<object[]> GetCommentaryById_GetTestData()
    {
        yield return new object[] { new CommentaryDbModel(), new Commentary() };
        yield return new object[]
        {
            CreateCommentaryDbo(MakeGuid(1), MakeGuid(1), MakeGuid(1), "text"),
            CreateCommentaryCoreModel(
                MakeGuid(1),
                MakeGuid(1),
                MakeGuid(1),
                "text"
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetCommentaryById_GetTestData))]
    public async Task GetCommentaryById_ReturnsFound(
        CommentaryDbModel returnedCommentaryDbo,
        Commentary expectedCommentary
    )
    {
        // Arrange
        _mockFactory
            .MockCommentariesDbSet.Setup(x => x.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(returnedCommentaryDbo);

        // Act
        var actual = await _repository.GetCommentaryById(expectedCommentary.Id);

        // Assert
        Assert.Equal(expectedCommentary, actual);
    }
}
