using MewingPad.Database.Context;
using MewingPad.Database.Models;

namespace MewingPad.Tests.IntegrationTests;

[Collection("Database")]
public class TriggerTests(DatabaseFixture databaseFixture)
{
    private MewingPadDbContext _context = databaseFixture.Context;

    [Fact]
    public async Task TestInsertSuccess()
    {
        var user = new UserDbModel(Guid.NewGuid(), "username", "123", "email", Common.Enums.UserRole.Admin);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user.Id, "filepath");
        var score = new ScoreDbModel(user.Id, audiotrack.Id, 5);

        await _context.Users.AddAsync(user);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score);
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(5.0f, actualMeanScore);
    }

    [Fact]
    public async Task TestUpdateSuccess()
    {
        var user = new UserDbModel(Guid.NewGuid(), "username", "123", "email", Common.Enums.UserRole.Admin);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user.Id, "filepath");
        var score = new ScoreDbModel(user.Id, audiotrack.Id, 5);

        await _context.Users.AddAsync(user);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score);
        await _context.SaveChangesAsync();

        var scoreDbModel = await _context.Scores.FindAsync([user.Id, audiotrack.Id]);
        scoreDbModel!.Value = 4;
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(4.0f, actualMeanScore);
    }

    [Fact]
    public async Task TestDeleteSuccess()
    {
        var user = new UserDbModel(Guid.NewGuid(), "username", "123", "email", Common.Enums.UserRole.Admin);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user.Id, "filepath");
        var score = new ScoreDbModel(user.Id, audiotrack.Id, 5);

        await _context.Users.AddAsync(user);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score);
        await _context.SaveChangesAsync();

        _context.Scores.Remove(score);
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(0.0f, actualMeanScore);
    }

    [Fact]
    public async Task TestInsertToManySuccess()
    {
        var user1 = new UserDbModel(Guid.NewGuid(), "username1", "123", "email", Common.Enums.UserRole.Admin);
        var user2 = new UserDbModel(Guid.NewGuid(), "username2", "123", "email", Common.Enums.UserRole.User);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user1.Id, "filepath");

        var score1 = new ScoreDbModel(user1.Id, audiotrack.Id, 5);
        var score2 = new ScoreDbModel(user2.Id, audiotrack.Id, 2);

        await _context.Users.AddAsync(user1);
        await _context.Users.AddAsync(user2);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score1);
        await _context.Scores.AddAsync(score2);
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(3.5f, actualMeanScore);
    }

    [Fact]
    public async Task TestUpdateToManySuccess()
    {
        var user1 = new UserDbModel(Guid.NewGuid(), "username1", "123", "email", Common.Enums.UserRole.Admin);
        var user2 = new UserDbModel(Guid.NewGuid(), "username2", "123", "email", Common.Enums.UserRole.User);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user1.Id, "filepath");

        var score1 = new ScoreDbModel(user1.Id, audiotrack.Id, 5);
        var score2 = new ScoreDbModel(user2.Id, audiotrack.Id, 2);

        await _context.Users.AddAsync(user1);
        await _context.Users.AddAsync(user2);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score1);
        await _context.Scores.AddAsync(score2);
        await _context.SaveChangesAsync();

        var scoreDbModel = await _context.Scores.FindAsync([user2.Id, audiotrack.Id]);
        scoreDbModel!.Value = 4;
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(4.5f, actualMeanScore);
    }

    [Fact]
    public async Task TestDeleteToManySuccess()
    {
        var user1 = new UserDbModel(Guid.NewGuid(), "username1", "123", "email", Common.Enums.UserRole.Admin);
        var user2 = new UserDbModel(Guid.NewGuid(), "username2", "123", "email", Common.Enums.UserRole.User);
        var audiotrack = new AudiotrackDbModel(Guid.NewGuid(), "title", user1.Id, "filepath");

        var score1 = new ScoreDbModel(user1.Id, audiotrack.Id, 5);
        var score2 = new ScoreDbModel(user2.Id, audiotrack.Id, 2);

        await _context.Users.AddAsync(user1);
        await _context.Users.AddAsync(user2);
        await _context.Audiotracks.AddAsync(audiotrack);
        await _context.SaveChangesAsync();

        await _context.Scores.AddAsync(score1);
        await _context.Scores.AddAsync(score2);
        await _context.SaveChangesAsync();

        _context.Scores.Remove(score1);
        await _context.SaveChangesAsync();

        var actualMeanScore = _context.Audiotracks
            .Where(a => a.Id == audiotrack.Id)
            .Select(a => a.MeanScore)
            .First();
        
        Assert.Equal(2.0f, actualMeanScore);
    }
}