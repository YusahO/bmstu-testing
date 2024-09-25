using MewingPad.Common.Entities;

namespace MewingPad.Common.IRepositories;

public interface IAudiotrackRepository
{
    Task AddAudiotrack(Audiotrack audiotrack);
    Task<Audiotrack> UpdateAudiotrack(Audiotrack audiotrack);
    Task DeleteAudiotrack(Guid id);
    Task<Audiotrack?> GetAudiotrackById(Guid id);
    Task<List<Audiotrack>> GetAllAudiotracks();
    Task<List<Audiotrack>> GetAudiotracksByTitle(string title);
}
