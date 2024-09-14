using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestTagRepository : IDisposable
{
    public DatabaseFixture Fixture { get; }
    private readonly TagRepository _repository;

    public TestTagRepository(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new(Fixture.CreateContext());
    }

    public void Dispose() => Fixture.Cleanup();

    [Fact]
    public async void TestAddTag_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithName("Tag")
            .Build();

        // Act
        await _repository.AddTag(tag);

        // Assert
        var actual = (from a in context.Tags select a).ToList();
        Assert.Single(actual);
    }

    [Fact]
    public async void TestAddTag_SameTagError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
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
    public async void TestDeleteTag_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithName("Tag")
            .Build();
        await context.Tags.AddAsync(
            new TagDbModelBuilder()
                .WithId(tag.Id)
                .WithAuthorId(tag.AuthorId)
                .WithName(tag.Name)
                .Build()
        );
        context.SaveChanges();

        // Act
        await _repository.DeleteTag(tag.Id);

        // Assert
        Assert.Empty((from a in context.Tags select a).ToList());
    }

    [Fact]
    public async void TestDeleteTag_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteTag(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestUpdateTag_Ok()
    {
        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithName("Tag")
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Tags.Add(
                new TagDbModelBuilder()
                    .WithId(tag.Id)
                    .WithAuthorId(tag.AuthorId)
                    .WithName(tag.Name)
                    .Build()
            );
            context.SaveChanges();
        }

        tag.Name = "New";

        // Act
        await _repository.UpdateTag(tag);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = (from a in context.Tags select a).ToList();
            Assert.Single(actual);
            Assert.Equal(tag.Id, actual[0].Id);
            Assert.Equal("New", actual[0].Name);
            Assert.Equal(tag.AuthorId, actual[0].AuthorId);
        }
    }

    [Fact]
    public async void TestUpdateTag_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var tag = new TagCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithAuthorId(Fixture.DefaultUserId)
            .WithName("Tag")
            .Build();

        // Act
        async Task Action() => await _repository.UpdateTag(tag);

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async void TestGetTagById_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 2]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithName($"Tag{i}")
                    .Build()
            );
        }
        context.SaveChanges();

        // Act
        var actual = await _repository.GetTagById(expectedId);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(expectedId, actual.Id);
        Assert.Equal("Tag2", actual.Name);
        Assert.Equal(Fixture.DefaultUserId, actual.AuthorId);
    }

    [Fact]
    public async void TestGetTagById_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 5]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithAuthorId(Fixture.DefaultUserId)
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
    public async void TestGetTagById_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetTagById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async void TestGetAllTags_SomeOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (byte i = 1; i < 4; ++i)
        {
            await context.Tags.AddAsync(
                new TagDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithAuthorId(Fixture.DefaultUserId)
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
    public async void TestGetAllTags_EmptyOk()
    {
        // Arrange

        // Act
        var actual = await _repository.GetAllTags();

        // Assert
        Assert.Empty(actual);
    }
}
