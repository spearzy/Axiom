using System.Linq.Expressions;

namespace Axiom.Assertions.Equivalency;

public sealed partial class EquivalencyOptions
{
    public EquivalencyOptions MatchMemberName(string actualMember, string expectedMember)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actualMember);
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedMember);

        AddMemberNameMapping(actualMember.Trim(), expectedMember.Trim());
        return this;
    }

    public EquivalencyOptions MatchMember<TActual, TExpected>(
        Expression<Func<TActual, object?>> actualMemberSelector,
        Expression<Func<TExpected, object?>> expectedMemberSelector)
    {
        ArgumentNullException.ThrowIfNull(actualMemberSelector);
        ArgumentNullException.ThrowIfNull(expectedMemberSelector);

        var actualPath = EquivalencySelectorPath.Create(actualMemberSelector, nameof(actualMemberSelector));
        var expectedPath = EquivalencySelectorPath.Create(expectedMemberSelector, nameof(expectedMemberSelector));

        ValidateTypedMemberMappingDepth(actualPath, expectedPath);
        AddTypedMemberMapping(actualPath, expectedPath);
        return this;
    }

    internal bool TryGetMappedExpectedMemberName(string actualMember, out string expectedMember)
    {
        return _actualToExpectedMemberNames.TryGetValue(actualMember, out expectedMember!);
    }

    internal bool TryGetMappedActualMemberName(string expectedMember, out string actualMember)
    {
        return _expectedToActualMemberNames.TryGetValue(expectedMember, out actualMember!);
    }

    internal bool TryGetMappedExpectedMemberPath(string actualPath, out string expectedPath)
    {
        return _actualToExpectedMemberPaths.TryGetValue(actualPath, out expectedPath!);
    }

    internal bool TryGetMappedActualMemberPath(string expectedPath, out string actualPath)
    {
        return _expectedToActualMemberPaths.TryGetValue(expectedPath, out actualPath!);
    }

    private void AddMemberNameMapping(string actualMember, string expectedMember)
    {
        // Keep mappings one-to-one so comparisons stay deterministic.
        if (_actualToExpectedMemberNames.TryGetValue(actualMember, out var existingExpected))
        {
            _expectedToActualMemberNames.Remove(existingExpected);
        }

        if (_expectedToActualMemberNames.TryGetValue(expectedMember, out var existingActual))
        {
            _actualToExpectedMemberNames.Remove(existingActual);
        }

        _actualToExpectedMemberNames[actualMember] = expectedMember;
        _expectedToActualMemberNames[expectedMember] = actualMember;
    }

    private void AddTypedMemberMapping(string actualPath, string expectedPath)
    {
        var actualSegments = actualPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var expectedSegments = expectedPath.Split('.', StringSplitOptions.RemoveEmptyEntries);

        for (var index = 0; index < actualSegments.Length; index++)
        {
            AddTypedPathMapping(
                string.Join(".", actualSegments[..(index + 1)]),
                string.Join(".", expectedSegments[..(index + 1)]));
        }
    }

    private void AddTypedPathMapping(string actualPath, string expectedPath)
    {
        if (_actualToExpectedMemberPaths.TryGetValue(actualPath, out var existingExpected))
        {
            _expectedToActualMemberPaths.Remove(existingExpected);
        }

        if (_expectedToActualMemberPaths.TryGetValue(expectedPath, out var existingActual))
        {
            _actualToExpectedMemberPaths.Remove(existingActual);
        }

        _actualToExpectedMemberPaths[actualPath] = expectedPath;
        _expectedToActualMemberPaths[expectedPath] = actualPath;
    }

    private static void ValidateTypedMemberMappingDepth(string actualPath, string expectedPath)
    {
        var actualDepth = actualPath.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;
        var expectedDepth = expectedPath.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;
        if (actualDepth != expectedDepth)
        {
            throw new ArgumentException(
                "actualMemberSelector and expectedMemberSelector must select member paths with the same depth.");
        }
    }
}
