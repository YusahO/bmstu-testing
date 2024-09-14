using MewingPad.Common.Exceptions;
using MewingPad.Database.NpgsqlRepositories;
using MewingPad.Tests.DataAccess.UnitTests.Builders;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.DataAccess.UnitTests.Repositories;

[Collection("Test Database")]
public class TestAudiotrackRepository : IDisposable
{
    public DatabaseFixture Fixture { get; }
    private readonly AudiotrackRepository _repository;

    public TestAudiotrackRepository(DatabaseFixture fixture)
    {
        Fixture = fixture;
        _repository = new(Fixture.CreateContext());
    }

    public void Dispose() => Fixture.Cleanup();

    [Fact]
    public async Task TestAddSingleAudiotrack_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
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
    public async Task TestAddAudiotrackToMany_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (int i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithFilepath($"/path/to/file{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("New")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithFilepath("/path/to/new")
            .Build();

        // Act
        await _repository.AddAudiotrack(audiotrack);

        // Assert
        var actual = (from a in context.Audiotracks select a).ToList();
        Assert.Equal(4, actual.Count);
    }

    [Fact]
    public async Task TestAddAudiotrack_SameAudiotrackError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
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
    public async Task TestDeleteAudiotrack_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
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
        await _repository.DeleteAudiotrack(audiotrack.Id);

        // Assert
        Assert.Empty((from a in context.Audiotracks select a).ToList());
    }

    [Fact]
    public async Task TestDeleteAudiotrack_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        async Task Action() => await _repository.DeleteAudiotrack(new Guid());

        // Assert
        await Assert.ThrowsAsync<RepositoryException>(Action);
    }

    [Fact]
    public async Task TestUpdateAudiotrack_Ok()
    {
        // Arrange
        var audiotrack = new AudiotrackCoreModelBuilder()
            .WithId(Guid.NewGuid())
            .WithTitle("Hello")
            .WithAuthorId(Fixture.DefaultUserId)
            .WithFilepath("/path/to/file")
            .Build();
        using (var context = Fixture.CreateContext())
        {
            context.Audiotracks.Add(
                new AudiotrackDbModelBuilder()
                    .WithId(audiotrack.Id)
                    .WithTitle(audiotrack.Title)
                    .WithAuthorId(audiotrack.AuthorId)
                    .WithFilepath(audiotrack.Filepath)
                    .Build()
            );
            await context.SaveChangesAsync();
        }

        audiotrack.Title = "New";

        // Act
        await _repository.UpdateAudiotrack(audiotrack);

        // Assert
        using (var context = Fixture.CreateContext())
        {
            var actual = context
                .Audiotracks.Where(x => x.Id == audiotrack.Id)
                .ToList();
            Assert.Single(actual);
            Assert.Equal(audiotrack.Id, actual[0].Id);
            Assert.Equal("New", actual[0].Title);
            Assert.Equal(audiotrack.AuthorId, actual[0].AuthorId);
            Assert.Equal(audiotrack.Filepath, actual[0].Filepath);
        }
    }

    [Fact]
    public async Task TestUpdateAudiotrack_NonexistentError()
    {
        using var context = Fixture.CreateContext();

        // Arrange
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
    public async Task TestGetAllAudiotracksEmpty_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetAllAudiotracks();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task TestGetAllAudiotracksSome_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (int i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(Guid.NewGuid())
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
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
    public async Task TestGetAudiotrackById_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 2]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
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
        Assert.Equal("Hello2", actual.Title);
        Assert.Equal(Fixture.DefaultUserId, actual.AuthorId);
        Assert.Equal("/path/to/file2", actual.Filepath);
    }

    [Fact]
    public async Task TestGetAudiotrackById_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 4]);
        for (byte i = 1; i < 4; ++i)
        {
            Console.WriteLine(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]));
            // await context.Audiotracks.AddAsync(
            //     new AudiotrackDbModelBuilder()
            //         .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
            //         .WithTitle($"Hello{i}")
            //         .WithAuthorId(Fixture.DefaultUserId)
            //         .WithFilepath($"/path/to/file{i}")
            //         .Build()
            // );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackById(expectedId);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task TestGetAudiotrackByIdEmpty_Ok()
    {
        using var context = Fixture.CreateContext();

        // Arrange

        // Act
        var actual = await _repository.GetAudiotrackById(Guid.NewGuid());

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task TestGetAudiotrackByTitle_SingleOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithFilepath($"/path/to/file{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksByTitle("Hello2");

        // Assert
        Assert.Single(actual);
        Assert.Equal(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 2]), actual[0].Id);
        Assert.Equal("Hello2", actual[0].Title);
        Assert.Equal(Fixture.DefaultUserId, actual[0].AuthorId);
        Assert.Equal("/path/to/file2", actual[0].Filepath);
    }

    [Fact]
    public async Task TestGetAudiotrackByTitle_SomeOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithFilepath($"/path/to/file")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotracksByTitle("Hello");

        // Assert
        Assert.Equal(3, actual.Count);
        Assert.All(actual, (audio) => Assert.Equal("Hello", audio.Title));
    }

    [Fact]
    public async Task TestGetAudiotrackByTitle_NoneFoundOk()
    {
        using var context = Fixture.CreateContext();

        // Arrange
        Guid expectedId = new(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, 4]);
        for (byte i = 1; i < 4; ++i)
        {
            await context.Audiotracks.AddAsync(
                new AudiotrackDbModelBuilder()
                    .WithId(new Guid(0, 0, 0, [0, 0, 0, 0, 0, 0, 0, i]))
                    .WithTitle($"Hello{i}")
                    .WithAuthorId(Fixture.DefaultUserId)
                    .WithFilepath($"/path/to/file{i}")
                    .Build()
            );
        }
        await context.SaveChangesAsync();

        // Act
        var actual = await _repository.GetAudiotrackById(expectedId);

        // Assert
        Assert.Null(actual);
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
