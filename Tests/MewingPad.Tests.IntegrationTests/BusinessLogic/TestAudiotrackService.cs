// using MewingPad.Common.Entities;
// using MewingPad.Common.Exceptions;
// using MewingPad.Database.Context;
// using MewingPad.Database.NpgsqlRepositories;
// using MewingPad.Services.AudiotrackService;
// using MewingPad.Tests.Factories.Core;

// namespace MewingPad.Tests.IntegrationTests.BusinessLogic;

// [Collection("Test Database")]
// public class TestAudiotrackService : BaseServiceTestClass
// {
//     private MewingPadDbContext _context;
//     private readonly AudiotrackService _service;
//     private readonly AudiotrackRepository _audiotrackRepository;
//     private readonly PlaylistAudiotrackRepository _playlistAudiotrackRepository;
//     private readonly TagAudiotrackRepository _tagAudiotrackRepository;

//     public TestAudiotrackService(DatabaseFixture fixture)
//         : base(fixture)
//     {
//         _context = Fixture.CreateContext();

//         _audiotrackRepository = new(_context);
//         _playlistAudiotrackRepository = new(_context);
//         _tagAudiotrackRepository = new(_context);

//         _service = new(
//             _audiotrackRepository,
//             _playlistAudiotrackRepository,
//             _tagAudiotrackRepository,
//             new()
//         );
//     }

//     [Fact]
//     public async Task DeleteAudiotrack_AudiotrackExists_Ok()
//     {
//         using var context = Fixture.CreateContext();
//         // Arrange
//         await AddDefaultUserWithPlaylist();

//         var expectedId = MakeGuid(1);
//         context.Audiotracks.Add(
//             new AudiotrackDbModelBuilder()
//                 .WithId(expectedId)
//                 .WithAuthorId(MakeGuid(1))
//                 .WithTitle("Title")
//                 .WithFilepath("Filepath")
//                 .Build()
//         );
//         context.SaveChanges();

//         // Act
//         await _service.DeleteAudiotrack(expectedId);

//         // Assert
//         Assert.Null(context.Audiotracks.Find(expectedId));
//     }

// //     [Fact]
// //     public async Task DeleteAudiotrack_AudiotrackNonexistent_Error()
// //     {
// //         // Arrange

// //         // Act
// //         async Task Action() => await _service.DeleteAudiotrack(new Guid());

// //         // Assert
// //         await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
// //     }

// //     [Fact]
// //     public async Task GetAudiotrackById_AudiotrackExists_ReturnsAudiotrack()
// //     {
// //         // Arrange
// //         await AddDefaultUserWithPlaylist();

// //         var expectedId = MakeGuid(1);
// //         using (var context = Fixture.CreateContext())
// //         {
// //             context.Audiotracks.Add(
// //                 new AudiotrackDbModelBuilder()
// //                     .WithId(expectedId)
// //                     .WithAuthorId(MakeGuid(1))
// //                     .WithTitle("Title")
// //                     .WithFilepath("Filepath")
// //                     .Build()
// //             );
// //             context.SaveChanges();
// //         }

// //         // Act
// //         var actual = await _service.GetAudiotrackById(expectedId);

// //         // Assert
// //         Assert.Equal(expectedId, actual.Id);
// //     }

// //     [Fact]
// //     public async Task GetAudiotrackById_NoAudiotrackWithId_Error()
// //     {
// //         // Arrange

// //         // Act
// //         async Task Action() => await _service.GetAudiotrackById(new Guid());

// //         // Assert
// //         await Assert.ThrowsAsync<AudiotrackNotFoundException>(Action);
// //     }

// //     [Fact]
// //     public async Task GetAudiotracksByTitle_AudiotracksExist_ReturnsAudiotracks()
// //     {
// //         // Arrange
// //         await AddDefaultUserWithPlaylist();
// //         const string expectedTitle = "Title";

// //         using (var context = Fixture.CreateContext())
// //         {
// //             context.Audiotracks.Add(
// //                 new AudiotrackDbModelBuilder()
// //                     .WithId(MakeGuid(1))
// //                     .WithAuthorId(MakeGuid(1))
// //                     .WithTitle(expectedTitle)
// //                     .WithFilepath("Filepath")
// //                     .Build()
// //             );
// //             context.SaveChanges();
// //         }

// //         // Act
// //         var actual = await _service.GetAudiotracksByTitle(expectedTitle);

// //         // Assert
// //         Assert.Equal(expectedTitle, actual.First().Title);
// //     }

// //     [Fact]
// //     public async Task GetAudiotracksByTitle_NoAudiotracksWithTitle_ReturnsEmpty()
// //     {
// //         // Arrange

// //         // Act
// //         var actual = await _service.GetAudiotracksByTitle("");

// //         // Assert
// //         Assert.Empty(actual);
// //     }

// //     [Fact]
// //     public async Task GetAllAudiotracks_AudiotracksExist_ReturnsAudiotracks()
// //     {
// //         // Arrange
// //         await AddDefaultUserWithPlaylist();
// //         var expectedTracks = new List<Audiotrack>(
// //             [
// //                 AudiotrackFabric.Create(
// //                     MakeGuid(1),
// //                     "Title",
// //                     MakeGuid(1),
// //                     "Filepath"
// //                 ),
// //             ]
// //         );

// //         using (var context = Fixture.CreateContext())
// //         {
// //             context.Audiotracks.Add(
// //                 new AudiotrackDbModelBuilder()
// //                     .WithId(MakeGuid(1))
// //                     .WithAuthorId(MakeGuid(1))
// //                     .WithTitle("Title")
// //                     .WithFilepath("Filepath")
// //                     .Build()
// //             );
// //             context.SaveChanges();
// //         }

// //         // Act
// //         var actual = await _service.GetAllAudiotracks();

// //         // Assert
// //         Assert.Single(actual);
// //         Assert.Equal(expectedTracks, actual);
// //     }

// //     [Fact]
// //     public async Task GetAllAudiotracks_NoAudiotracks_ReturnsEmpty()
// //     {
// //         // Arrange

// //         // Act
// //         var actual = await _service.GetAllAudiotracks();

// //         // Assert
// //         Assert.Empty(actual);
// //     }
// }
