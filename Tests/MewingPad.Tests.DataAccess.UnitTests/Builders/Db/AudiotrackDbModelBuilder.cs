using MewingPad.Database.Models;

namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

public class AudiotrackDbModelBuilder
{
	private AudiotrackDbModel _audiotrackDbo = new();

	public AudiotrackDbModelBuilder WithId(Guid id)
	{
		_audiotrackDbo.Id = id;
		return this;
	}

	public AudiotrackDbModelBuilder WithTitle(string title)
	{
		_audiotrackDbo.Title = title;
		return this;
	}

	public AudiotrackDbModelBuilder WithAuthorId(Guid authorId)
	{
		_audiotrackDbo.AuthorId = authorId;
		return this;
	}

	public AudiotrackDbModelBuilder WithFilepath(string filepath)
	{
		_audiotrackDbo.Filepath = filepath;
		return this;
	}

	public AudiotrackDbModel Build()
	{
		return _audiotrackDbo;
	}
}