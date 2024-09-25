using MewingPad.Common.Entities;

namespace MewingPad.Services.AudiotrackService;

public interface IAudiotrackService
{
    Task CreateAudiotrackWithStream(Stream stream, Audiotrack audiotrack);
    Task<Audiotrack> UpdateAudiotrackWithStream(Stream stream, Audiotrack audiotrack);
    Task DeleteAudiotrack(Guid audiotrackId);
    Task<Stream> GetAudiotrackFileStream(string srcpath);
    Task<Audiotrack> GetAudiotrackById(Guid audiotrackId);
    Task<List<Audiotrack>> GetAllAudiotracks();
    Task<List<Audiotrack>> GetAudiotracksByTitle(string title);
}
