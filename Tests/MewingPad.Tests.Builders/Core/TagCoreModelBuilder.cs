namespace MewingPad.Tests.Builders.Core;

using MewingPad.Common.Entities;

public class TagCoreModelBuilder
{
    private Tag _tag = new();

    public TagCoreModelBuilder WithId(Guid id)
    {
        _tag.Id = id;
        return this;
    }

    public TagCoreModelBuilder WithAuthorId(Guid authorId)
    {
        _tag.AuthorId = authorId;
        return this;
    }

    public TagCoreModelBuilder WithName(string name)
    {
        _tag.Name = name;
        return this;
    }
    public Tag Build()
    {
        return _tag;
    }
}

