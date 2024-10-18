using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using Moq.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess.Repositories;

public class TestTagRepository : BaseRepositoryTestClass
{
    private readonly TagRepository _repository;
    private readonly MockDbContextFactory _mockFactory;

    public TestTagRepository()
    {
        _mockFactory = new MockDbContextFactory();
        _repository = new(_mockFactory.MockContext.Object);
    }

    private static Tag CreateTagCoreModel(Guid id, Guid authorId, string name)
    {
        return new TagCoreModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithName(name)
            .Build();
    }

    private static TagDbModel CreateTagDbo(Guid id, Guid authorId, string name)
    {
        return new TagDbModelBuilder()
            .WithId(id)
            .WithAuthorId(authorId)
            .WithName(name)
            .Build();
    }

    private static TagDbModel CreateTagDboFromCore(Tag tag)
    {
        return CreateTagDbo(tag.Id, tag.AuthorId, tag.Name);
    }

    [Fact]
    public async void AddTag_AddUnique_Ok()
    {
        // Arrange
        List<TagDbModel> actual = [];
        var tag = CreateTagCoreModel(MakeGuid(1), MakeGuid(1), "name");
        var tagDbo = CreateTagDboFromCore(tag);

        _mockFactory
            .MockTagsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<TagDbModel>(), default)
            )
            .Callback<TagDbModel, CancellationToken>(
                (s, token) => actual.Add(s)
            );

        // Act
        await _repository.AddTag(tag);

        // Assert
        Assert.Single(actual);
        Assert.Equal(tag.Id, actual[0].Id);
        Assert.Equal(tag.AuthorId, actual[0].AuthorId);
        Assert.Equal(tag.Name, actual[0].Name);
    }

    [Fact]
    public async void AddTag_AddWithSameId_Error()
    {
        // Arrange
        _mockFactory
            .MockTagsDbSet.Setup(s =>
                s.AddAsync(It.IsAny<TagDbModel>(), default)
            )
            .Callback<TagDbModel, CancellationToken>(
                (s, token) => throw new RepositoryException()
            );

        // Act
        async Task Action() => await _repository.AddTag(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeleteTag_DeleteExisting_Ok()
    {
        // Arrange
        Guid tagId = MakeGuid(1);
        List<TagDbModel> tagDbos =
        [
            CreateTagDbo(MakeGuid(1), MakeGuid(1), "name"),
        ];

        _mockFactory
            .MockTagsDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tagDbos[0]);
        _mockFactory
            .MockTagsDbSet.Setup(s => s.Remove(It.IsAny<TagDbModel>()))
            .Callback((TagDbModel t) => tagDbos.Remove(t));

        // Act
        await _repository.DeleteTag(tagId);

        // Assert
        Assert.Empty(tagDbos);
    }

    [Fact]
    public async void DeleteTag_DeleteNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockTagsDbSet.Setup(s => s.FindAsync(It.IsAny<Guid>()))
            .ReturnsAsync(default(TagDbModel)!);
        _mockFactory
            .MockTagsDbSet.Setup(s => s.Remove(It.IsAny<TagDbModel>()))
            .Callback((TagDbModel a) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.DeleteTag(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateTag_UpdateExisting_Ok()
    {
        // Arrange
        Guid expectedTagId = MakeGuid(1);
        Guid expectedAuthorId = MakeGuid(1);
        const string expectedName = "name";

        var tag = CreateTagCoreModel(expectedTagId, expectedAuthorId, "Old");
        var tagDbo = CreateTagDboFromCore(tag);
        List<TagDbModel> tagDbos = [tagDbo];

        _mockFactory.MockContext.Setup(m => m.Tags).ReturnsDbSet(tagDbos);

        tag.Name = expectedName;

        // Act
        await _repository.UpdateTag(tag);

        // Assert
        Assert.Single(tagDbos);
        Assert.Equal(tag.Id, tagDbos[0].Id);
        Assert.Equal(tag.AuthorId, tagDbos[0].AuthorId);
        Assert.Equal(expectedName, tagDbos[0].Name);
    }

    [Fact]
    public async void UpdateTag_UpdateNonexistent_Error()
    {
        // Arrange
        _mockFactory
            .MockTagsDbSet.Setup(s => s.Update(It.IsAny<TagDbModel>()))
            .Callback((TagDbModel s) => throw new RepositoryException());

        // Act
        async Task Action() => await _repository.UpdateTag(new());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    public static IEnumerable<object[]> GetTagById_GetTestData()
    {
        yield return new object[] { default(TagDbModel)!, default(Tag)! };
        yield return new object[]
        {
            CreateTagDbo(MakeGuid(1), MakeGuid(1), "name"),
            CreateTagCoreModel(MakeGuid(1), MakeGuid(1), "name"),
        };
    }

    [Theory]
    [MemberData(nameof(GetTagById_GetTestData))]
    public async void GetTagById_ReturnsFound(
        TagDbModel returnedTagDbo,
        Tag expectedTag
    )
    {
        // Arrange
        _mockFactory
            .MockTagsDbSet.Setup(x => x.FindAsync(It.IsAny<object?[]?>()))
            .ReturnsAsync(returnedTagDbo);

        // Act
        var actual = await _repository.GetTagById(new());

        // Assert
        Assert.Equal(expectedTag, actual);
    }

    public static IEnumerable<object[]> GetAllTags_GetTestData()
    {
        yield return new object[] { new List<TagDbModel>(), new List<Tag>() };
        yield return new object[]
        {
            new List<TagDbModel>(
                [CreateTagDbo(MakeGuid(1), MakeGuid(1), "name")]
            ),
            new List<Tag>(
                [CreateTagCoreModel(MakeGuid(1), MakeGuid(1), "name")]
            ),
        };
    }

    [Theory]
    [MemberData(nameof(GetAllTags_GetTestData))]
    public async void GetAllTags_ReturnsFound(
        List<TagDbModel> tagDbos,
        List<Tag> expectedTags
    )
    {
        // Arrange
        _mockFactory.MockContext.Setup(x => x.Tags).ReturnsDbSet(tagDbos);

        // Act
        var actual = await _repository.GetAllTags();

        // Assert
        Assert.Equal(expectedTags, actual);
    }
}
