using MewingPad.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace MewingPad.Tests.UnitTests.DataAccess;

public class MockDbContextFactory
{
    public Mock<MewingPadDbContext> MockContext { get; set; }

    public Mock<DbSet<UserDbModel>> MockUsersDbSet { get; set; }
    public Mock<DbSet<PlaylistDbModel>> MockPlaylistsDbSet { get; set; }
    public Mock<DbSet<AudiotrackDbModel>> MockAudiotracksDbSet { get; set; }
    public Mock<DbSet<TagDbModel>> MockTagsDbSet { get; set; }
    public Mock<DbSet<CommentaryDbModel>> MockCommentariesDbSet { get; set; }
    public Mock<DbSet<ReportDbModel>> MockReportsDbSet { get; set; }
    public Mock<DbSet<ScoreDbModel>> MockScoresDbSet { get; set; }

    public Mock<
        DbSet<UserFavouriteDbModel>
    > MockUsersFavouritesDbSet
    { get; set; }
    public Mock<
        DbSet<TagAudiotrackDbModel>
    > MockTagsAudiotracksDbModel
    { get; set; }
    public Mock<
        DbSet<PlaylistAudiotrackDbModel>
    > MockPlaylistsAudiotracksDbModel
    { get; set; }

    public MockDbContextFactory()
    {
        MockContext = new Mock<MewingPadDbContext>();

        MockUsersDbSet = SetupMockDbSet(new List<UserDbModel>());
        MockPlaylistsDbSet = SetupMockDbSet(new List<PlaylistDbModel>());
        MockAudiotracksDbSet = SetupMockDbSet(new List<AudiotrackDbModel>());
        MockTagsDbSet = SetupMockDbSet(new List<TagDbModel>());
        MockCommentariesDbSet = SetupMockDbSet(new List<CommentaryDbModel>());
        MockReportsDbSet = SetupMockDbSet(new List<ReportDbModel>());
        MockScoresDbSet = SetupMockDbSet(new List<ScoreDbModel>());

        MockUsersFavouritesDbSet = SetupMockDbSet(
            new List<UserFavouriteDbModel>()
        );
        MockTagsAudiotracksDbModel = SetupMockDbSet(
            new List<TagAudiotrackDbModel>()
        );
        MockPlaylistsAudiotracksDbModel = SetupMockDbSet(
            new List<PlaylistAudiotrackDbModel>()
        );

        MockContext.Setup(m => m.Users).Returns(MockUsersDbSet.Object);
        MockContext.Setup(m => m.Playlists).Returns(MockPlaylistsDbSet.Object);
        MockContext
            .Setup(m => m.Audiotracks)
            .Returns(MockAudiotracksDbSet.Object);
        MockContext.Setup(m => m.Tags).Returns(MockTagsDbSet.Object);
        MockContext
            .Setup(m => m.Commentaries)
            .Returns(MockCommentariesDbSet.Object);
        MockContext.Setup(m => m.Reports).Returns(MockReportsDbSet.Object);
        MockContext.Setup(m => m.Scores).Returns(MockScoresDbSet.Object);

        MockContext
            .Setup(m => m.UsersFavourites)
            .Returns(MockUsersFavouritesDbSet.Object);
        MockContext
            .Setup(m => m.TagsAudiotracks)
            .Returns(MockTagsAudiotracksDbModel.Object);
        MockContext
            .Setup(m => m.PlaylistsAudiotracks)
            .Returns(MockPlaylistsAudiotracksDbModel.Object);
    }

    public static Mock<DbSet<T>> SetupMockDbSet<T>(List<T> list)
        where T : class
    {
        var queryable = list.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(queryable.Provider);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
        mockDbSet
            .As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
        return mockDbSet;
    }

    public void SetUserList(IEnumerable<UserDbModel> users)
    {
        var userList = users.ToList();
        MockUsersDbSet = SetupMockDbSet(userList);
        MockContext.Setup(m => m.Users).Returns(MockUsersDbSet.Object);
    }
}
