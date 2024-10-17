using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;


namespace MewingPad.Tests.IntegrationTests.DataAccess;

[Collection("Test Database")]
public class TestTagRepository : BaseRepositoryTestClass
{
    private readonly TagRepository _repository;

    public TestTagRepository(DatabaseFixture fixture)
        : base(fixture)
    {
        _repository = new(Fixture.CreateContext());
    }

    [Fact]
    public async void AddTag_AddSingle_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "Tag";

        var tag = new TagCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithName(expectedName)
            .Build();

        // Act
        await _repository.AddTag(tag);

        // Assert
        var actual = (from a in context.Tags select a).ToList();
        Assert.Single(actual);
        Assert.Equal(expectedId, actual[0].Id);
        Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        Assert.Equal(expectedName, actual[0].Name);
    }

    [Fact]
    public async void AddTag_AddWithSameId_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var tag = new TagCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("Tag")
            .Build();
        await context.Tags.AddAsync(
            new TagDbModelBuilder()
                .WithId(tag.Id)
                .WithAuthorId(tag.AuthorId)
                .WithName(tag.Name)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        async Task Action() => await _repository.AddTag(tag);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void DeleteTag_DeleteExisting_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var tag = new TagCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("Tag")
            .Build();
        await context.Tags.AddAsync(
            new TagDbModelBuilder()
                .WithId(tag.Id)
                .WithAuthorId(tag.AuthorId)
                .WithName(tag.Name)
                .Build()
        );
        await context.SaveChangesAsync();

        // Act
        await _repository.DeleteTag(tag.Id);

        // Assert
        Assert.Empty((from a in context.Tags select a).ToList());
    }

    [Fact]
    public async void DeleteTag_DeleteNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteTag(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void UpdateTag_UpdateExisting_Ok()
    {
        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(1);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "New";

        var tag = new TagCoreModelBuilder()
            .WithId(expectedId)
            .WithAuthorId(expectedAuthorId)
            .WithName("Tag")
            .Build();
        using (var context = Fixture.CreateContext())
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(tag.Id)
                    .WithAuthorId(tag.AuthorId)
                    .WithName(tag.Name)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        tag.Name = "New";

        // Act
        await _repository.UpdateTag(tag);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Tags select a).ToList();
            Assert.Single(actual);
            Assert.Equal(expectedId, actual[0].Id);
            Assert.Equal(expectedName, actual[0].Name);
            Assert.Equal(expectedAuthorId, actual[0].AuthorId);
        }
    }

    [Fact]
    public async void UpdateTag_UpdateNonexistent_Error()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var tag = new TagCoreModelBuilder()
            .WithId(MakeGuid(1))
            .WithAuthorId(DefaultUserId)
            .WithName("Tag")
            .Build();

        // Act
        async Task Action() => await _repository.UpdateTag(tag);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void GetTagById_TagWithIdExists_ReturnsTag()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        var expectedId = MakeGuid(2);
        var expectedAuthorId = DefaultUserId;
        const string expectedName = "Tag2";

        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(expectedAuthorId)
                    .WithName($"Tag{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetTagById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal(expectedName, actual.Name);
        Assert.Equal(expectedAuthorId, actual.AuthorId);
    }

    [Fact]
    public async void GetTagById_NoTagWithId_ReturnsNull()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        Guid expectedId = MakeGuid(5);

        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(DefaultUserId)
                    .WithName($"Tag{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetTagById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetTagById_NoTags_Ok()
    {
        // Arrange

        // Act
        var actual = await _repository.GetTagById(new Guid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void GetAllTags_TagsExist_ReturnsTags()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        await AddDefaultUserWithPlaylist();

        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(MakeGuid(i))
                    .WithAuthorId(DefaultUserId)
                    .WithName($"Tag{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAllTags();

        // Assert
        Assert.Equal(3, actual.Count);
    }

    [Fact]
    public async void GetAllTags_NoTagsExist_ReturnsEmpty()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllTags();

        // Assert
        Assert.Empty(actual);
    }
}
