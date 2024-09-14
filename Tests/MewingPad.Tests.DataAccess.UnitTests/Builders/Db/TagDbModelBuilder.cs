namespace MewingPad.Tests.DataAccess.UnitTests.Builders;

using MewingPad.Database.Models;

public class TagDbModelBuilder
{
    private TagDbModel _tagDbo = new();

    public TagDbModelBuilder WithId(Guid id)
    {
        _tagDbo.Id = id;
        return this;
    }

    public TagDbModelBuilder WithAuthorId(Guid authorId)
    {
        _tagDbo.AuthorId = authorId;
        return this;
    }

    public TagDbModelBuilder WithName(string name)
    {
        _tagDbo.Name = name;
        return this;
    }

    public TagDbModel Build()
    {
        return _tagDbo;
    }
}

