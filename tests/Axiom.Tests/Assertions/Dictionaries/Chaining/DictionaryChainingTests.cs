using Axiom.Assertions;
using Axiom.Assertions.Extensions;

namespace Axiom.Tests.Assertions.Dictionaries.Chaining;

public sealed class DictionaryChainingTests
{
    [Fact]
    public void ContainKey_WhoseValue_CanBeUsedToAssertExtractedValue()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
        };

        var extracted = values.Should().ContainKey("alpha").WhoseValue;

        Assert.Equal(1, extracted);
    }

    [Fact]
    public void DictionaryChain_CanBeComposed()
    {
        Dictionary<string, int> values = new()
        {
            ["alpha"] = 1,
            ["beta"] = 2,
            ["gamma"] = 3,
        };

        values.Should()
            .ContainKey("alpha").And
            .ContainValue(2).And
            .ContainEntry("gamma", 3).And
            .NotContainEntry("alpha", 2).And
            .NotContainKey("delta").And
            .NotContainValue(9);
    }
}
